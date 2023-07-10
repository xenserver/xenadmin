/* Copyright (c) Cloud Software Group, Inc. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Threading;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using XenCenterLib;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;


namespace XenAdmin.Network
{
    [DebuggerDisplay("IXenConnection :{HostnameWithPort}")]
    public class XenConnection : IXenConnection, IXmlSerializable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Hostname { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FriendlyName { get; set; }

        /// <summary>
        /// The last known name for the pool, in the form "'Pool friendly name' (hostname or IP)".
        /// </summary>
        public string LastConnectionFullName = "";

        /// <summary>
        /// Whether this connection is saved in a disconnected state (i.e. don't reconnect on session restore).
        /// </summary>
        public bool SaveDisconnected { get; set; }

        /// <summary>
        /// Indicates whether this connection was created in the Add Server dialog.
        /// </summary>
        public bool fromDialog = false;

        private volatile bool _expectedDisruption;
        private volatile bool _suppressErrors;
        private volatile bool _preventResettingPasswordPrompt;
        private volatile bool _coordinatorMayChange;

        /// <summary>
        /// Whether we're expecting network disruption, say because we're reconfiguring the network at this time.  In that case, we ignore
        /// keepalive failures, and expect task polling to be disrupted.
        /// </summary>
        public bool ExpectDisruption
        {
            get => _expectedDisruption;
            set => _expectedDisruption = value;
        }

        /// <summary>
        /// If we are 'expecting' this connection's Password property to contain the correct password
        /// (i.e. false if the user has just entered the password, true if it was restored from the saved session).
        /// </summary>
        public bool ExpectPasswordIsCorrect { get; set; }

        /// <summary>
        /// Used by the patch wizard, suppress any errors coming from reconnect attempts
        /// </summary>
        public bool SuppressErrors
        {
            get => _suppressErrors;
            set => _suppressErrors = value;
        }

        /// <summary>
        /// The password prompting function (<see cref="_promptForNewPassword"/>) is set to null when the connection is closed.
        /// Set this value to true in order to prevent that from happening.
        /// n.b.: remember to set the value back to false once it's not needed anymore
        /// </summary>
        public bool PreventResettingPasswordPrompt
        {
            get => _preventResettingPasswordPrompt;
            set => _preventResettingPasswordPrompt = value;
        }

        /// <summary>
        /// Indicates whether we are expecting the pool coordinator to change soon (e.g. when explicitly designating a new coordinator).
        /// </summary>
        public bool CoordinatorMayChange
        {
            get => _coordinatorMayChange;
            set => _coordinatorMayChange = value;
        }

        /// <summary>
        /// Set when we detect that Event.next() has become blocked and we need to reset the connection. See CA-33145.
        /// </summary>
        private bool EventNextBlocked = false;

        /// <summary>
        /// A cache of the pool's opaque_ref, last time this connection was connected.  This will be
        /// Helper.NullOpaqueRef if it's never been connected.
        /// </summary>
        private string PoolOpaqueRef = Helper.NullOpaqueRef;
        /// <summary>
        /// Set after a successful connection attempt, before the cache is populated.
        /// </summary>
        private string CoordinatorIPAddress = "";

        /// <summary>
        /// The lock that must be taken around connectTask and heartbeat.
        /// </summary>
        private readonly object connectTaskLock = new object();
        private ConnectTask connectTask = null;

        /// <summary>
        /// This is the metrics monitor. Has to be accessed within the connectTaskLock.
        /// </summary>
        private Heartbeat heartbeat;

        /// <summary>
        /// Whether we are trying to automatically connect to the new coordinator. Set in HandleConnectionLost.
        /// Note: I think we are not using this correctly -- see CA-37864 for details -- but I'm not going
        /// to fix it unless it gives rise to a reported bug, because I can't test the fix.
        /// </summary>
        private volatile bool FindingNewCoordinator = false;

        /// <summary>
        /// The time at which we started looking for the new coordinator.
        /// </summary>
        private DateTime FindingNewCoordinatorStartedAt = DateTime.MinValue;

        /// <summary>
        /// Timeout before we consider that Event.next() has got blocked: see CA-33145
        /// </summary>
        private const int EVENT_NEXT_TIMEOUT = 120 * 1000;  // 2 minutes

        private string LastCoordinatorHostname = "";
        public readonly object PoolMembersLock = new object();
        private List<string> _poolMembers = new List<string>();
        public List<string> PoolMembers { get { return _poolMembers; } set { _poolMembers = value; } }
        private int PoolMemberIndex = 0;
        private System.Threading.Timer ReconnectionTimer = null;

        private ActionBase ConnectAction;

        private DateTime m_startTime = DateTime.MinValue;
        private int m_lastDebug;
        private TimeSpan _serverTimeOffset = TimeSpan.Zero;
        private object ServerTimeOffsetLock = new object();
        /// <summary>
        /// The offset between the clock at the client and the clock at the server.  server time + ServerTimeOffset = client time.
        /// This does not take the local timezone into account -- all calculations should be in UTC.
        /// For Orlando and greater, this value is set by XenMetricsMonitor, calling Host.get_servertime on a heartbeat.
        /// </summary>
        public TimeSpan ServerTimeOffset
        {
            get
            {
                lock (ServerTimeOffsetLock)
                {
                    return _serverTimeOffset;
                }
            }
            set
            {
                lock (ServerTimeOffsetLock)
                {
                    TimeSpan diff = _serverTimeOffset - value;

                    if (diff.TotalSeconds < -1 || 1 < diff.TotalSeconds)
                    {
                        var now = DateTime.UtcNow;
                        var debugMsg = string.Format("Time offset for {0} is now {1}.  It's now {2} UTC here, and {3} UTC on the server.",
                                                     Hostname, value,
                                                     now.ToString("o", CultureInfo.InvariantCulture),
                                                     (now.Subtract(value)).ToString("o", CultureInfo.InvariantCulture));

                        if (m_startTime.Ticks == 0)//log it the first time it is detected
                        {
                            m_startTime = now;
                            log.Info(debugMsg);
                        }

                        //then log every 5mins
                        int currDebug = (int)((now - m_startTime).TotalSeconds) / 300;
                        if (currDebug > m_lastDebug)
                        {
                            m_lastDebug = currDebug;
                            log.InfoFormat(debugMsg);
                        }
                    }

                    _serverTimeOffset = value;
                }
            }
        }

        public string HostnameWithPort
        {
            get
            {
                return Port == ConnectionsManager.DEFAULT_XEN_PORT ? Hostname : Hostname + ':' + Port.ToString();
            }
        }

        public string UriScheme
        {
            get
            {
                return Port == 8080 || Port == 80 ? Uri.UriSchemeHttp : Uri.UriSchemeHttps;
            }
        }

        public string Name
        {
            get
            {
                string result = Helpers.GetName(Helpers.GetPoolOfOne(this));
                return !string.IsNullOrEmpty(result)
                           ? result
                           : !string.IsNullOrEmpty(FriendlyName) ? FriendlyName : Hostname;
            }
        }

        /// <summary>
        /// The cache of XenAPI objects for this connection.
        /// </summary>
        public ICache Cache { get; } = new Cache();

        private readonly LockFreeQueue<ObjectChange> eventQueue = new LockFreeQueue<ObjectChange>();
        private readonly System.Threading.Timer cacheUpdateTimer;

        /// <summary>
        /// Whether the cache for this connection has been populated.
        /// </summary>
        public bool CacheIsPopulated
        {
            get => _cacheIsPopulated;
            private set
            {
                _cacheIsPopulated = value;

                if (_cacheIsPopulated)
                    CachePopulated?.Invoke(this);
            }
        }

        private bool _cacheIsPopulated;

        private bool cacheUpdaterRunning = false;
        private bool updatesWaiting = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="XenConnection"/> class.
        /// </summary>
        public XenConnection()
        {
            Port = ConnectionsManager.DEFAULT_XEN_PORT;
            Username = "root";
            SaveDisconnected = false;
            ExpectPasswordIsCorrect = true; 
            cacheUpdateTimer = new Timer(cacheUpdater);
        }

        #region Events

        /// <summary>
        /// Fired just before the cache is cleared (i.e. the cache is still populated).
        /// </summary>
        public event Action<IXenConnection> ClearingCache;
        public event Action<IXenConnection> CachePopulated;
        public event EventHandler<ConnectionResultEventArgs> ConnectionResult;
        public event Action<IXenConnection> ConnectionStateChanged;
        public event Action<IXenConnection> ConnectionLost;
        public event Action<IXenConnection> ConnectionClosed;
        public event Action<IXenConnection> ConnectionReconnecting;
        public event Action<IXenConnection> BeforeConnectionEnd;
        public event Action<IXenConnection, string> ConnectionMessageChanged;
        public event Action<IXenConnection, bool> BeforeMajorChange;
        public event Action<IXenConnection, bool> AfterMajorChange;

        /// <summary>
        /// Fired on the UI thread, once per batch of events in CacheUpdater.
        /// </summary>
        public event EventHandler<EventArgs> XenObjectsUpdated;

        #endregion

        public void LoadFromDatabaseFile(string databaseFile)
        {
            IsSimulatedConnection = true;

            var foundConn = ConnectionsManager.XenConnections.Cast<XenConnection>().FirstOrDefault(c =>
                    c.Hostname == databaseFile && c.CoordinatorIPAddress == CoordinatorIPAddress);
            if (foundConn != null && foundConn.IsConnected)
                throw new ConnectionExists(this);

            var document = new XmlDocument();
            using (var sr = new StreamReader(databaseFile))
                document.LoadXml(sr.ReadToEnd());

            var db = new Db(document);
            var events = db.Tables.SelectMany(t => t.Rows).Select(r => r.ObjectChange).ToList();

            var session = new Session(this, Path.GetFileName(Hostname), Port);
            connectTask = new ConnectTask(Hostname, Port) { Connected = true, Session = session };

            OnBeforeMajorChange(false);
            Cache.Clear();
            CacheIsPopulated = false;
            OnAfterMajorChange(false);

            OnBeforeMajorChange(false);
            Cache.UpdateFrom(this, events);
            OnAfterMajorChange(false);
            CacheIsPopulated = true;

            var pool = Cache.Pools[0];
            CoordinatorIPAddress = Cache.Hosts.FirstOrDefault(h => h.opaque_ref == pool.master)?.address;
            
            HandleSuccessfulConnection(connectTask, pool);
            MarkConnectActionComplete();
            OnXenObjectsUpdated();
        }

        public NetworkCredential NetworkCredential { get; set; }

        public bool IsSimulatedConnection { get; private set; }

        public bool IsConnected
        {
            get
            {
                // sneaky, but avoids a lock
                ConnectTask t = connectTask;
                return t != null && t.Connected;
            }
        }

        public bool InProgress
        {
            get { return connectTask != null; }
        }

        /// <summary>
        /// Gets the Session passed to ConnectWorkerThread and used to update the cache in XenObjectDownloader.
        /// May return null.
        /// </summary>
        public Session Session
        {
            get
            {
                ConnectTask t = connectTask;
                return t == null ? null : t.Session;
            }
        }

        /// <summary>
        /// Create a duplicate of the Session that this connection is currently using.
        /// This will use a separate TCP stream, but the same authentication credentials.
        /// </summary>
        /// <returns></returns>
        public Session DuplicateSession()
        {
            return DuplicateSession(Session.STANDARD_TIMEOUT);
        }

        public Session DuplicateSession(int timeout)
        {
            Session s = Session;
            if (s == null)
                throw new DisconnectionException();
            return new Session(s, this) { Timeout = timeout };
        }

        /// <summary>
        /// For retrieving an extra session using different credentials to those stored in the connection. Used for the sudo
        /// function in actions. Does not prompt for new credentials if the authentication fails. 
        /// Does not change connection's Username and Password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Session ElevatedSession(string username, string password)
        {
            return GetNewSession(Hostname, Port, username, password, true);
        }

        /// <summary>
        /// For retrieving a new session. 
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="isElevated"></param>
        /// <returns>null if the server password has changed, the user has been prompted for the new password,
        /// but the user has clicked cancel on the dialog.</returns>
        private Session GetNewSession(string hostname, int port, string username, string password, bool isElevated)
        {
            const int DELAY = 250; // unit = ms
            int attempt = 0;
            while (true)
            {
                attempt++;
                string uname = isElevated ? username : Username;
                string pwd = isElevated ? password : Password; // Keep the password that we're using for this iteration, as it may
                // be changed by another thread handling an authentication failure.
                // For elevated session we use the elevated username and password passed into this function, 
                // as the connection's Username and Password are not updated.

                Session session = new Session(this, hostname, port);
                if (isElevated)
                    session.IsElevatedSession = true;

                try
                {
                    session.login_with_password(uname, pwd, Helper.APIVersionString(API_Version.LATEST), Session.UserAgent);
                    NetworkCredential = new NetworkCredential(uname, pwd);
                    return session;
                }
                catch (Failure f)
                {
                    if (connectTask == null || connectTask.Cancelled) // the user has clicked cancel on the connection to server dialog
                    {
                        attempt = DEFAULT_MAX_SESSION_LOGIN_ATTEMPTS; // make sure we throw rather than try again
                        throw new CancelledException(); // make the dialog pop up again
                    }
                    else if (f.ErrorDescription.Count > 0)
                    {
                        switch (f.ErrorDescription[0])
                        {
                            case Failure.SESSION_AUTHENTICATION_FAILED:
                                if (isElevated)
                                    throw;
                                if (PromptForNewPassword(pwd))
                                    attempt = 0;
                                else
                                {
                                    //user cannot provide correct credentials, we d/c now to save the confusion of having the server available
                                    //but unusable.
                                    EndConnect();
                                    throw new CancelledException();
                                }
                                break;
                            case Failure.HOST_IS_SLAVE:
                            // we know it is a supporter so there there is no need to try and connect again, we need to connect to the coordinator
                            case Failure.RBAC_PERMISSION_DENIED:
                            // No point retrying this, the user needs the read only role at least to log in
                            case Failure.HOST_UNKNOWN_TO_MASTER:
                                // Will never succeed, CA-74718
                                throw;
                            default:
                                if (isElevated)
                                {
                                    if (attempt >= DEFAULT_MAX_SESSION_LOGIN_ATTEMPTS)
                                        throw;
                                    else
                                        break;
                                }
                                else
                                    break;
                        }
                    }
                    else
                    {
                        if (attempt >= DEFAULT_MAX_SESSION_LOGIN_ATTEMPTS)
                            throw;
                    }
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.TrustFailure)
                        throw new CancelledException();
                    if (e.Status == WebExceptionStatus.NameResolutionFailure || e.Status == WebExceptionStatus.ProtocolError || attempt >= DEFAULT_MAX_SESSION_LOGIN_ATTEMPTS)
                        throw;
                }
                catch (UriFormatException)
                {
                    // No point trying to connect more than once to a duff URI
                    throw;
                }
                catch (Exception)
                {
                    if (attempt >= DEFAULT_MAX_SESSION_LOGIN_ATTEMPTS)
                        throw;
                }
                Thread.Sleep(DELAY);
            }
        }

        // This CompareTo() mimics the sort order we get from XenSearch, with
        // pools first, then hosts, then disconnected connections. It is used
        // in some dialogs where we don't do a proper search: see CA-57131 &
        // CA-60517 for examples.
        public int CompareTo(IXenConnection other)
        {
            if (this == other)
                return 0;

            if (other == null)
                return -1;

            Pool thisPool = Helpers.GetPool(this);
            Pool otherPool = Helpers.GetPool(other);

            int thisClass = (this.IsConnected ?
                (thisPool == null ? 2 : 1) : 3);
            int otherClass = (other.IsConnected ?
                (otherPool == null ? 2 : 1) : 3);

            if (thisClass != otherClass)
                return thisClass - otherClass;

            int result = StringUtility.NaturalCompare(Name, other.Name);
            if (result != 0)
                return result;

            Pool p1 = Helpers.GetPoolOfOne(this);
            Pool p2 = Helpers.GetPoolOfOne(other);
            if (p1 == null || p2 == null)
                return 0;  // shouldn't happen once connected, but let's be safe

            return p1.opaque_ref.CompareTo(p2.opaque_ref);
        }

        private void SetPoolAndHostInAction(ActionBase action, Pool pool, string poolopaqueref)
        {
            if (pool != null && !string.IsNullOrEmpty(poolopaqueref))
            {
                Pool p = new Pool();
                p.Connection = this;
                p.opaque_ref = poolopaqueref;
                p.UpdateFrom(pool);

                Host h = new Host();
                h.Connection = this;
                h.opaque_ref = pool.master.opaque_ref;
                h.name_label = Hostname;

                action.Pool = p;
                action.Host = h;
            }
            else
            {
                // Match the hack in XenSearch/GroupAlg.cs.  We are creating fake Host objects to
                // represent the disconnected server, with the opaque_ref set to the connection's HostnameWithPort.
                // We need to do the same here, so that Actions for disconnected hosts (like failed connections)
                // are attached to the disconnected server correctly.
                Host host = new Host();
                host.Connection = this;
                host.opaque_ref = HostnameWithPort;
                action.Host = host;
            }
        }

        private Func<IXenConnection, string, bool> _promptForNewPassword;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initiateCoordinatorSearch">If true, if connection to the coordinator fails we will start trying to connect to
        /// each remembered supporter in turn.</param>
        /// <param name="promptForNewPassword">A function that prompts the user for the changed password for a server.</param>
        public void BeginConnect(bool initiateCoordinatorSearch, Func<IXenConnection, string, bool> promptForNewPassword)
        {
            _promptForNewPassword = promptForNewPassword;

            //InvokeHelper.Synchronizer is used for synchronizing the cache update. Must not be null at this point. It can be initialized through InvokeHelper.Initialize()
            Trace.Assert(InvokeHelper.Synchronizer != null);

            InvokeHelper.AssertOnEventThread();

            if (initiateCoordinatorSearch)
            {
                FindingNewCoordinator = true;
                FindingNewCoordinatorStartedAt = DateTime.Now;
            }
            CoordinatorMayChange = false;

            if (!HandlePromptForNewPassword())
                return;

            lock (connectTaskLock)
            {
                //if connectTask != null a connection is already in progress

                if (connectTask == null)
                {
                    ClearEventQueue();
                    OnBeforeMajorChange(false);
                    Cache.Clear();
                    OnAfterMajorChange(false);
                    connectTask = new ConnectTask(Hostname, Port);

                    heartbeat?.Stop();
                    heartbeat = null;
                    heartbeat = new Heartbeat(this, XenAdminConfigManager.Provider.ConnectionTimeout);

                    Thread t = new Thread(ConnectWorkerThread);
                    t.Name = "Connection to " + Hostname;
                    t.IsBackground = true;
                    t.Start(connectTask);
                }
            }
        }

        private static object WaitForMonitor = new object();
        private static int WaitForEventRegistered = 0;
        private static object WaitForEventRegisteredLock = new object();
        public void WaitFor(Func<bool> predicate, Func<bool> cancelling)
        {
            lock (WaitForEventRegisteredLock)
            {
                if (WaitForEventRegistered == 0)
                    XenObjectsUpdated += WakeWaitFor;
                WaitForEventRegistered++;
            }
            try
            {
                for (int i = 0; i < 120; i++)
                {
                    lock (WaitForMonitor)
                    {
                        if (predicate() || (cancelling != null && cancelling()))
                            return;
                        System.Threading.Monitor.Wait(WaitForMonitor, 500);
                    }
                }
            }
            finally
            {
                lock (WaitForEventRegisteredLock)
                {
                    WaitForEventRegistered--;
                    if (WaitForEventRegistered == 0)
                        XenObjectsUpdated -= WakeWaitFor;
                }
            }
        }

        private void WakeWaitFor(object sender, EventArgs e)
        {
            lock (WaitForMonitor)
            {
                System.Threading.Monitor.PulseAll(WaitForMonitor);
            }
        }

        /// <summary>
        /// Equivalent to WaitForCache(xenref, null).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xenref"></param>
        /// <returns></returns>
        public T WaitForCache<T>(XenRef<T> xenref) where T : XenObject<T>
        {
            return WaitForCache(xenref, null);
        }

        /// <summary>
        /// Wait for the given object to arrive in our cache.  Returns the XenObject, or null if it does
        /// not appear within the timeout (1 minute).
        /// 
        /// This blocks, so must only be used on a background thread.
        /// </summary>
        /// <param name="xenref"></param>
        /// <param name="cancelling">A delegate to check whether to cancel.  May be null, in which case it's ignored</param>
        public T WaitForCache<T>(XenRef<T> xenref, Func<bool> cancelling) where T : XenObject<T>
        {
            lock (WaitForEventRegisteredLock)
            {
                if (WaitForEventRegistered == 0)
                    XenObjectsUpdated += WakeWaitFor;
                WaitForEventRegistered++;
            }
            try
            {
                for (int i = 0; i < 120; i++)
                {
                    lock (WaitForMonitor)
                    {
                        T result = Resolve<T>(xenref);
                        if (result != null || (cancelling != null && cancelling()))
                            return result;
                        System.Threading.Monitor.Wait(WaitForMonitor, 500);
                    }
                }
                return null;
            }
            finally
            {
                lock (WaitForEventRegisteredLock)
                {
                    WaitForEventRegistered--;
                    if (WaitForEventRegistered == 0)
                        XenObjectsUpdated -= WakeWaitFor;
                }
            }
        }

        /// <param name="clearCache">Whether the cache should be cleared (requires invoking onto the GUI thread)</param>
        /// <param name="exiting"></param>
        public void EndConnect(bool clearCache = true, bool exiting = false)
        {
            ConnectTask t = connectTask;
            connectTask = null;
            EndConnect(clearCache, t, exiting);
        }

        /// <summary>
        /// Closes the connecting dialog, stops the XenMetricsMonitor thread, marks this.task as Cancelled and
        /// logs out of the task's Session on a background thread.
        /// </summary>
        /// <param name="clearCache">Whether the cache should be cleared (requires invoking onto the GUI thread)</param>
        /// <param name="task"></param>
        /// <param name="exiting"></param>
        private void EndConnect(bool clearCache, ConnectTask task, bool exiting)
        {
            OnBeforeConnectionEnd();

            lock (connectTaskLock)
            {
                heartbeat?.Stop();
                heartbeat = null;

                if (task != null)
                {
                    task.Cancelled = true;
                    Session session = task.Session;
                    task.Session = null;
                    if (session != null)
                    {
                        Logout(session, exiting);
                    }
                }
            }

            MarkConnectActionComplete();
            log.Info($"Connection to {Hostname} is ended.");

            // Save list of addresses of current hosts in pool
            List<string> members = new List<string>();
            foreach (Host host in Cache.Hosts)
            {
                members.Add(host.address);
            }
            lock (PoolMembersLock)
            {
                PoolMembers = members;
            }

            // Clear the XenAPI object cache
            if (clearCache)
            {
                ClearCache();
            }
            if (!PreventResettingPasswordPrompt)
            {
                // CA-371356: Preventing the reset of the prompt allows
                // for it to be shown when attempting to reconnect to a host
                // whose password has changed since last login
                _promptForNewPassword = null;
            }
            OnConnectionClosed();
        }

        /// <summary>
        /// Try to logout the given session. This will cause any threads blocking on Event.next() to get
        /// a XenAPI.Failure (which is better than them freezing around forever).
        /// Do on a background thread - otherwise, if the coordinator has died, then this will block
        /// until the timeout is reached (default 20s).
        /// However, in the case of exiting, the thread need to be set as foreground. 
        /// Otherwise the logging out operation can be terminated when other foreground threads finish.
        /// </summary>
        /// <param name="session">May be null, in which case nothing happens.</param>
        /// <param name="exiting"></param>
        public void Logout(Session session, bool exiting = false)
        {
            if (session == null || session.opaque_ref == null)
                return;

            Thread t = new Thread(Logout_);
            t.Name = string.Format("Logging out session {0}", session.opaque_ref);
            if (exiting)
            {
                t.IsBackground = false;
                t.Priority = ThreadPriority.AboveNormal;
            }
            else
            {
                t.IsBackground = true;
                t.Priority = ThreadPriority.Lowest;
            }
            t.Start(session);
        }

        public void Logout()
        {
            Logout(Session);
        }

        private static void Logout_(object o)
        {
            Session session = (Session)o;
            try
            {
                log.Debug("Trying Session.logout() on background thread");
                session.logout();
                log.Debug("Session.logout() succeeded");
            }
            catch (Exception e)
            {
                log.Debug("Session.logout() failed", e);
            }
        }

        public void Interrupt()
        {
            ConnectTask t = connectTask;
            ICache coll = Cache;
            connectTask = null;
            if (t != null && t.Connected)
            {
                string poolopaqueref = null;
                Pool pool = coll == null ? null : getAPool(coll, out poolopaqueref);
                t.Cancelled = true;
                HandleConnectionLost(t, pool, poolopaqueref);
            }
        }

        private void ClearCache()
        {
            OnClearingCache();
            ClearEventQueue();
            // This call to Clear needs to occur on the background thread, otherwise the event firing in response to all the changes
            // block in one big lump rather than the smaller pieces that you get when invoking onto the event thread on a finer
            // granularity.  If you do all this on the event thread, then the app tends to go (Not Responding) when you lose a connection.
            // It doesn't actually occur on the background thread all the time.  There's a path from AddServerDialog.ConnectToServer.
            Cache.Clear();
        }

        private string GetReason(Exception error)
        {
            if (error is ArgumentException)
            {
                // This happens if the server API is incompatible with our bindings.  This should
                // never happen in production, but will happen during development if a field
                // changes type, for example.
                return string.Format(Messages.SERVER_API_INCOMPATIBLE, BrandManager.BrandConsole);
            }
            else if (error is WebException)
            {
                WebException w = error as WebException;
                if (w.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    return string.Format(Messages.CONNECT_RESOLUTION_FAILURE, this.Hostname);
                }
                else if (w.Status == WebExceptionStatus.ConnectFailure)
                {
                    return string.Format(Messages.CONNCET_CONNECTION_FAILURE, this.Hostname);
                }
                else if (w.Status == WebExceptionStatus.ReceiveFailure)
                {
                    return string.Format(Messages.ERROR_NO_XENSERVER, this.Hostname);
                }
                else if (w.Status == WebExceptionStatus.SecureChannelFailure)
                {
                    return string.Format(Messages.ERROR_SECURE_CHANNEL_FAILURE, this.Hostname);
                }
                else
                {
                    return w.Message;
                }
            }
            else if (error is Failure && error != null && !string.IsNullOrEmpty(error.Message))
            {
                Failure f = error as Failure;
                if (f.ErrorDescription[0] == Failure.RBAC_PERMISSION_DENIED)
                {
                    // we use a different error message here from the standard one in friendly names
                    return Messages.ERROR_NO_PERMISSION;
                }
                return error.Message;
            }
            else if (error is NullReferenceException && error.Source.StartsWith("CookComputing"))
            {
                return string.Format(Messages.CONNCET_CONNECTION_FAILURE, this.Hostname);
            }
            else if (error != null && !string.IsNullOrEmpty(error.Message))
            {
                return error.Message;
            }
            else
            {
                return null;
            }
        }

        private void HandleSuccessfulConnection(ConnectTask task, Pool pool)
        {
            // Remove any other (disconnected) entries for this server from the tree

            var existingConnections = ConnectionsManager.XenConnections.Where(c =>
                c.Hostname.Equals(task.Hostname) && !c.IsConnected).ToList();

            foreach (var conn in existingConnections)
                ConnectionsManager.ClearCacheAndRemoveConnection(conn);

            // add server name to history (if it's not already there)
            XenAdminConfigManager.Provider.UpdateServerHistory(HostnameWithPort);

            if (!ConnectionsManager.XenConnectionsContains(this))
            {
                lock (ConnectionsManager.ConnectionsLock)
                {
                    ConnectionsManager.XenConnections.Add(this);
                }

                InvokeHelper.Invoke(XenAdminConfigManager.Provider.SaveSettingsIfRequired);
            }

            string name = string.IsNullOrEmpty(FriendlyName) || FriendlyName == task.Hostname
                              ? task.Hostname
                              : string.Format("{0} ({1})", FriendlyName, task.Hostname);
            string title = string.Format(Messages.CONNECTING_NOTICE_TITLE, name);
            string msg = string.Format(Messages.CONNECTING_NOTICE_TEXT, name);

            ConnectAction = new DummyAction(title, msg);
            SetPoolAndHostInAction(ConnectAction, pool, PoolOpaqueRef);

            ExpectPasswordIsCorrect = true;
            OnConnectionResult(true, null, null);

            log.InfoFormat("Completed connection phase for pool {0} ({1}:{2}, {3}).",
                FriendlyName, task.Hostname, task.Port, PoolOpaqueRef);
        }

        /// <summary>
        /// Check the password isn't null, which happens when the session is restored without remembering passwords.
        /// </summary>
        /// <returns>Whether to continue.</returns>
        private bool HandlePromptForNewPassword()
        {
            if (Password == null && !PromptForNewPassword(Password))
            {
                // if false the user has cancelled, set the password back to null and return
                Password = null;
                return false;
            }
            return true;
        }

        private readonly object PromptLock = new object();
        /// <summary>
        /// Prompts the user for the changed password for a server.
        /// </summary>
        /// <param name="old_password"></param>
        /// <returns></returns>
        private bool PromptForNewPassword(string old_password)
        {
            // Serialise prompting for new passwords, so that we don't get multiple dialogs pop up.
            lock (PromptLock)
            {
                if (Password != old_password)
                {
                    // Some other thread has changed the password already.  Retry using that one.
                    return true;
                }

                bool result = (_promptForNewPassword != null) ? _promptForNewPassword(this, old_password) : false;
                return result;
            }
        }

        private void ClearEventQueue()
        {
            while (eventQueue.NotEmpty)
            {
                eventQueue.Dequeue();
            }

            // Suspend the cache update timer
            cacheUpdateTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void EventsPending()
        {
            lock (cacheUpdateTimer)
            {
                if (cacheUpdaterRunning)
                    updatesWaiting = true;
                else
                    cacheUpdateTimer.Change(50, -1);
            }
        }

        private void cacheUpdater(object state)
        {
            lock (cacheUpdateTimer)
            {
                if (cacheUpdaterRunning)
                {
                    // there is a race-condition here which can be observed under high load: It is possible for the timer to fire even when 
                    // cacheUpdaterRunning = true. The check ensures we don't get multiple threads calling cacheUpdater
                    // on the same connection.
                    updatesWaiting = true;
                    return;
                }

                cacheUpdaterRunning = true;
                updatesWaiting = false;
            }

            try
            {
                cacheUpdater_();
            }
            finally
            {
                bool waiting;
                lock (cacheUpdateTimer)
                {
                    waiting = updatesWaiting;
                    cacheUpdaterRunning = false;
                }
                if (waiting)
                    cacheUpdater(null);
            }
        }

        private void cacheUpdater_()
        {
            // Copy events off the event queue
            // as we don't want events delivered while we're
            // on the GUI thread to be included in this set of
            // updates, otherwise we might hose the gui thread
            // during an event storm (ie deleting 1000 vms)

            List<ObjectChange> events = new List<ObjectChange>();

            while (eventQueue.NotEmpty)
                events.Add(eventQueue.Dequeue());

            if (events.Count > 0)
            {
                InvokeHelper.Invoke(() =>
                {
                    OnBeforeMajorChange(false);
                    bool fire = Cache.UpdateFrom(this, events);
                    OnAfterMajorChange(false);

                    if (fire)
                        OnXenObjectsUpdated();
                });

                if (!CacheIsPopulated)
                {
                    lock (connectTaskLock)
                        heartbeat?.Start();

                    CacheIsPopulated = true;
                    MarkConnectActionComplete();
                    log.Info($"Connection to {Hostname} successful.");
                }
            }
        }

        /// <summary>
        /// if not called the action will never finish and the gui will never close
        /// </summary>
        private void MarkConnectActionComplete()
        {
            if (ConnectAction != null && !ConnectAction.IsCompleted)
            {
                string title = string.Format(Messages.CONNECTION_OK_NOTICE_TITLE, Hostname);
                string msg = string.Format(Messages.CONNECTION_OK_NOTICE_TEXT, Hostname);
                ConnectAction.Title = title;
                ConnectAction.Description = msg;

                Pool pool = Helpers.GetPoolOfOne(this);
                if (pool != null)
                    SetPoolAndHostInAction(ConnectAction, pool, PoolOpaqueRef);

                // mark the connect action as completed
                ConnectAction.Finished = DateTime.Now;
                ConnectAction.PercentComplete = 100;
                ConnectAction.IsCompleted = true;
            }
        }

        private const int DEFAULT_MAX_SESSION_LOGIN_ATTEMPTS = 3;

        private readonly string eventNextConnectionGroupName = Guid.NewGuid().ToString();

        /// <summary>
        /// The main method for a connection to a XenServer. This method runs until the connection is lost, and
        /// contains a loop that after doing an initial cache fill processes events off the wire when they arrive.
        /// </summary>
        /// <param name="o"></param>
        private void ConnectWorkerThread(object o)
        {
            ConnectTask task = (ConnectTask)o;
            Exception error = null;
            Pool pool = null;

            try
            {
                log.DebugFormat("IXenConnection: trying to connect to {0}", HostnameWithPort);

                Session session = GetNewSession(task.Hostname, task.Port, Username, Password, false);
                // Save the session so we can log it out later
                task.Session = session;

                if (session.APIVersion < API_Version.API_2_5)
                    throw new ServerNotSupported();

                // Event.next uses a different session with a shorter timeout: see CA-33145.
                Session eventNextSession = DuplicateSession(EVENT_NEXT_TIMEOUT);
                eventNextSession.ConnectionGroupName = eventNextConnectionGroupName; // this will force the eventNextSession onto its own set of TCP streams (see CA-108676)

                CacheIsPopulated = false;
                session.CacheWarming = true;

                string token = "";
                bool eventsExceptionLogged = false;

                while (true)
                {
                    if (task.Cancelled)
                        break;

                    EventNextBlocked = false;

                    if (session.CacheWarming)
                    {
                        if (!task.Connected)
                        {
                            // We've started cache sync: update the dialog text
                            OnConnectionMessageChanged(string.Format(Messages.LABEL_SYNC, this.Hostname));
                        }

                        log.Debug("Cache is warming. Starting XenObjectDownloader.GetAllObjects");
                        XenObjectDownloader.GetAllObjects(session, eventQueue, task.GetCancelled, ref token);
                        log.Debug("Cache is warming. XenObjectDownloader.GetAllObjects finished successfully");
                        session.CacheWarming = false;
                    }
                    else
                    {
                        try
                        {
                            log.Debug("Starting XenObjectDownloader.GetEvents");
                            XenObjectDownloader.GetEvents(eventNextSession, eventQueue, task.GetCancelled, ref token);
                            log.Debug("Starting XenObjectDownloader.GetEvents finished successfully");
                            eventsExceptionLogged = false;
                        }
                        catch (Exception exn)
                        {
                            if (!ExpectDisruption)
                                throw;

                            log.DebugFormat("Exception (disruption is expected) in XenObjectDownloader.GetEvents: {0}", exn.GetType().Name);

                            // ignoring some exceptions when disruption is expected
                            if (exn is IOException ||
                                (exn is WebException webEx && (webEx.Status == WebExceptionStatus.KeepAliveFailure || webEx.Status == WebExceptionStatus.ConnectFailure)))
                            {
                                if (!eventsExceptionLogged)
                                {
                                    log.Debug("Ignoring keepalive/connect failure, because disruption is expected");
                                    eventsExceptionLogged = true;
                                }
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }

                    // check if requested to cancel
                    if (task.Cancelled)
                        break;

                    if (!task.Connected)
                    {
                        lock (ConnectionsManager.ConnectionsLock)
                        {
                            pool = ObjectChange.GetPool(eventQueue, out PoolOpaqueRef);
                            Host coordinator = ObjectChange.GetCoordinator(eventQueue);

                            CoordinatorIPAddress = coordinator.address;

                            foreach (IXenConnection iconn in ConnectionsManager.XenConnections)
                            {
                                XenConnection connection = iconn as XenConnection;
                                Trace.Assert(connection != null);
                                if (!connection.IsConnected)
                                    continue;

                                bool sameRef = PoolOpaqueRef == connection.PoolOpaqueRef;

                                if (!sameRef)
                                    continue;

                                bool sameCoordinator = CoordinatorIPAddress == connection.CoordinatorIPAddress;

                                if (sameRef && sameCoordinator)
                                    throw new ConnectionExists(connection);

                                // CA-15633: XenCenter does not allow connection to host on which backup is restored.
                                throw new BadRestoreDetected(connection);
                            }

                            task.Connected = true;

                            string poolName = pool.Name();

                            FriendlyName = !string.IsNullOrEmpty(poolName)
                                ? poolName
                                : !string.IsNullOrEmpty(coordinator.Name())
                                    ? coordinator.Name()
                                    : task.Hostname;
                        } // ConnectionsLock

                        log.DebugFormat("Getting server time for pool {0} ({1})...", FriendlyName, PoolOpaqueRef);
                        SetServerTimeOffset(session, pool.master.opaque_ref);

                        HandleSuccessfulConnection(task, pool);
                    }

                    EventsPending();
                }
            }
            catch (Failure e)
            {
                if (task.Cancelled && e.ErrorDescription.Count > 0 && e.ErrorDescription[0] == Failure.SESSION_INVALID)
                {
                    // Do nothing: this is probably a result of the user disconnecting, and us calling session.logout()
                }
                else
                {
                    error = e;
                    log.Warn(e);
                }
            }
            catch (WebException e)
            {
                error = e;
                log.Debug(e.Message);
            }
            catch (TargetInvocationException e)
            {
                error = e.InnerException;
                log.Error("TargetInvocationException", e);
            }
            catch (UriFormatException e)
            {
                // This can happen when the user types gobbledy-gook into the host-name field
                // of the add server dialog...
                error = e;
                log.Debug(e.Message);
            }
            catch (CancelledException)
            {
                task.Cancelled = true;
            }
            catch (DisconnectionException e)
            {
                error = e;
                log.Debug(e.Message);
            }
            catch (EventFromBlockedException e)
            {
                EventNextBlocked = true;
                error = e;
                log.Error(e, e);
            }
            catch (Exception e)
            {
                error = e;
                log.Error(e, e);
            }
            finally
            {
                HandleConnectionResult(task, error, pool);
            }
        }

        private void HandleConnectionResult(ConnectTask task, Exception error, Pool pool)
        {
            if (task != connectTask)
            {
                // We've been superseded by a newer ConnectTask. Exit silently without firing events.
                // Can happen when user disconnects while sync is taking place, then reconnects
                // (creating a new _connectTask) before the sync is complete.
            }
            else
            {
                ClearEventQueue();

                connectTask = null;

                // clean up action so we don't stay open forever
                if (ConnectAction != null)
                    ConnectAction.IsCompleted = true;

                if (error is ServerNotSupported)
                {
                    EndConnect(true, task, false);
                    log.Info(error.Message);
                    OnConnectionResult(false, error.Message, error);
                }
                else if (task.Cancelled)
                {
                    task.Connected = false;
                    log.InfoFormat("IXenConnection: closing connection to {0}", this.HostnameWithPort);
                    EndConnect(true, task, false);
                    OnConnectionClosed();
                }
                else if (task.Connected)
                {
                    HandleConnectionLost(task, pool, PoolOpaqueRef);
                }
                else
                {
                    // We never connected
                    string reason = GetReason(error);
                    log.WarnFormat("IXenConnection: failed to connect to {0}: {1}", HostnameWithPort, reason);

                    Failure f = error as Failure;
                    if (f != null && f.ErrorDescription[0] == Failure.HOST_IS_SLAVE)
                    {
                        //do not log an event in this case
                    }
                    else if (error is ConnectionExists)
                    {
                        //do not log an event in this case
                    }
                    else
                    {
                        // Create a new log message to say the connection attempt failed
                        string title = string.Format(Messages.CONNECTION_FAILED_TITLE, HostnameWithPort);
                        var action = new DummyAction(title, reason, reason);
                        SetPoolAndHostInAction(action, pool, PoolOpaqueRef);
                        action.Run();
                    }

                    // We only want to continue the coordinator search in certain circumstances
                    if (FindingNewCoordinator && (error is WebException || (f != null && f.ErrorDescription[0] != Failure.RBAC_PERMISSION_DENIED)))
                    {
                        if (f != null)
                        {
                            if (f.ErrorDescription[0] == XenAPI.Failure.HOST_IS_SLAVE)
                            {
                                log.DebugFormat("Found a member of {0} at {1}; redirecting to the coordinator at {2}",
                                                LastCoordinatorHostname, Hostname, f.ErrorDescription[1]);
                                Hostname = f.ErrorDescription[1];
                                OnConnectionMessageChanged(string.Format(Messages.CONNECTION_REDIRECTING, LastCoordinatorHostname, Hostname));
                                ReconnectCoordinator();
                            }
                            else if (f.ErrorDescription[0] == XenAPI.Failure.HOST_STILL_BOOTING)
                            {
                                log.DebugFormat("Found a member of {0} at {1}, but it's still booting; trying the next pool member",
                                                LastCoordinatorHostname, Hostname);
                                MaybeStartNextPoolMemberTimer(reason, error);
                            }
                            else
                            {
                                log.DebugFormat("Found a member of {0} at {1}, but got a failure; trying the next pool member",
                                                LastCoordinatorHostname, Hostname);
                                MaybeStartNextPoolMemberTimer(reason, error);
                            }
                        }
                        else if (PoolMemberRemaining())
                        {
                            log.DebugFormat("Connection to {0} failed; trying the next pool member", Hostname);
                            MaybeStartNextPoolMemberTimer(reason, error);
                        }
                        else
                        {
                            if (ExpectDisruption || DateTime.Now - FindingNewCoordinatorStartedAt < SEARCH_NEW_COORDINATOR_STOP_AFTER)
                            {
                                log.DebugFormat("While trying to find a connection for {0}, tried to connect to every remembered host. Will now loop back through pool members again.",
                                    this.HostnameWithPort);
                                lock (PoolMembersLock)
                                {
                                    PoolMemberIndex = 0;
                                }
                                MaybeStartNextPoolMemberTimer(reason, error);
                            }
                            else if (LastCoordinatorHostname != "")
                            {
                                log.DebugFormat("Stopping search for new coordinator for {0}: timeout reached without success. Trying the old coordinator one last time",
                                                LastConnectionFullName);
                                FindingNewCoordinator = false;
                                Hostname = LastCoordinatorHostname;
                                ReconnectCoordinator();
                            }
                            else
                            {
                                OnConnectionResult(false, reason, error);
                            }
                        }
                    }
                    else
                    {
                        OnConnectionResult(false, reason, error);
                    }
                }
            }
        }

        private void SetServerTimeOffset(Session session, string coordinator_opaqueref)
        {
            DateTime t = Host.get_servertime(session, coordinator_opaqueref);
            ServerTimeOffset = DateTime.UtcNow - t;
        }

        /// <summary>
        /// Called when a connection that had been made successfully is then lost.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="pool"></param>
        /// <param name="poolopaqueref"></param>
        private void HandleConnectionLost(ConnectTask task, Pool pool, string poolopaqueref)
        {
            task.Connected = false;
            log.WarnFormat("Lost connection to {0}", this.HostnameWithPort);

            // Cancel all current actions to do with this connection
            if (!ExpectDisruption)
            {
                InvokeHelper.Invoke(() => ConnectionsManager.CancelAllActions(this));
            }
            // Save list of addresses of current hosts in pool
            List<string> members = new List<string>();
            foreach (Host host in Cache.Hosts)
            {
                if (!string.IsNullOrEmpty(host.address))
                    members.Add(host.address);
            }

            // Save coordinator's address so we don't try to reconnect to it first
            Host coordinator = Helpers.GetCoordinator(this);
            // Save ha_enabled status before we clear the cache
            bool ha_enabled = IsHAEnabled();

            // NB line below clears the cache
            EndConnect(true, task, false);

            string description;
            LastCoordinatorHostname = Hostname;
            string poolName = pool.Name();
            if (string.IsNullOrEmpty(poolName))
            {
                LastConnectionFullName = HostnameWithPort;
            }
            else
            {
                LastConnectionFullName = string.Format("'{0}' ({1})", poolName, HostnameWithPort);
            }
            if (!EventNextBlocked && (CoordinatorMayChange || ha_enabled) && members.Count > 1)
            {
                log.DebugFormat("Will now try to connect to another pool member");

                lock (PoolMembersLock)
                {
                    PoolMembers.Clear();
                    PoolMembers.AddRange(members);
                    PoolMemberIndex = 0;
                    // Don't reconnect to the coordinator straight away, try a supporter first
                    if (coordinator != null && PoolMembers[0] == coordinator.address && PoolMembers.Count > 1)
                    {
                        PoolMemberIndex = 1;
                    }
                }
                FindingNewCoordinator = true;
                // Record the time at which we started the new coordinator search.
                FindingNewCoordinatorStartedAt = DateTime.Now;
                StartReconnectCoordinatorTimer();
                description = string.Format(Messages.CONNECTION_LOST_NOTICE_COORDINATOR_IN_X_SECONDS, LastConnectionFullName, XenConnection.SEARCH_NEW_COORDINATOR_TIMEOUT_MS / 1000);
                log.DebugFormat("Beginning search for new coordinator; will give up after {0} seconds", SEARCH_NEW_COORDINATOR_STOP_AFTER.TotalSeconds);
            }
            else
            {
                log.DebugFormat("Will retry connection to {0} in {1} ms.", LastConnectionFullName, ReconnectHostTimeoutMs);

                StartReconnectSingleHostTimer();

                description = string.Format(Messages.CONNECTION_LOST_RECONNECT_IN_X_SECONDS, LastConnectionFullName, ReconnectHostTimeoutMs / 1000);
            }

            string title = string.Format(Messages.CONNECTION_LOST_NOTICE_TITLE,
                                         LastConnectionFullName);
            var action = new DummyAction(title, description, description);
            SetPoolAndHostInAction(action, pool, poolopaqueref);
            action.Run();
            OnConnectionLost();
        }

        private bool PoolMemberRemaining()
        {
            lock (PoolMembersLock)
            {
                return PoolMemberIndex < PoolMembers.Count;
            }
        }

        private bool IsHAEnabled()
        {
            Pool pool = Helpers.GetPoolOfOne(this);
            return pool != null && pool.ha_enabled;
        }

        /// <summary>
        /// When we lose connection to a non-HA host, the timeout before we try reconnecting.
        /// </summary>
        private const int RECONNECT_HOST_TIMEOUT_MS = 120 * 1000;
        private const int RECONNECT_SHORT_TIMEOUT_MS = 5 * 1000;
        private int ReconnectHostTimeoutMs
        {
            get
            {
                if (EventNextBlocked || IsSimulatedConnection)
                    return RECONNECT_SHORT_TIMEOUT_MS;
                
                return RECONNECT_HOST_TIMEOUT_MS;
            }
        }

        /// <summary>
        /// When HA is enabled, the timeout after losing connection to the coordinator before we start searching for a new coordinator.
        /// i.e. This should be the time it takes coordinator failover to be sorted out on the server, plus a margin.
        /// NB we already have an additional built-in delay - it takes time for us to decide that the host is not responding,
        /// and stop the connection to the dead host, before starting the search.
        /// </summary>
        private const int SEARCH_NEW_COORDINATOR_TIMEOUT_MS = 60 * 1000;
        /// <summary>
        /// When HA is enabled, and going through each of the supporters to try and find the new coordinator, the time between failing
        /// to connect to one supporter and trying to connect to the next in the list.
        /// </summary>
        private const int SEARCH_NEXT_SUPPORTER_TIMEOUT_MS = 15 * 1000;
        /// <summary>
        /// When going through each of the remembered members of the pool looking for the new coordinator, don't start another pass
        /// through connecting to each of the hosts if we've already been looking for this long.
        /// </summary>
        private static readonly TimeSpan SEARCH_NEW_COORDINATOR_STOP_AFTER = TimeSpan.FromMinutes(6);

        private void StartReconnectSingleHostTimer()
        {
            ReconnectionTimer =
                new System.Threading.Timer((TimerCallback)ReconnectSingleHostTimer, null,
                ReconnectHostTimeoutMs, ReconnectHostTimeoutMs);
        }

        private void StartReconnectCoordinatorTimer()
        {
            StartReconnectCoordinatorTimer(SEARCH_NEW_COORDINATOR_TIMEOUT_MS);
        }

        private void MaybeStartNextPoolMemberTimer(string reason, Exception error)
        {
            if (PoolMemberRemaining())
                StartReconnectCoordinatorTimer(SEARCH_NEXT_SUPPORTER_TIMEOUT_MS);
            else
                OnConnectionResult(false, reason, error);
        }

        private void StartReconnectCoordinatorTimer(int timeout)
        {
            OnConnectionMessageChanged(string.Format(Messages.CONNECTION_WILL_RETRY_SUPPORTER, LastConnectionFullName.Ellipsise(25), timeout / 1000));
            ReconnectionTimer =
                new System.Threading.Timer((TimerCallback)ReconnectCoordinatorTimer, null,
                                           timeout, Timeout.Infinite);
        }

        private void ReconnectSingleHostTimer(object state)
        {
            if (IsConnected || !ConnectionsManager.XenConnectionsContains(this))
            {
                log.DebugFormat("Host {0} already reconnected", Hostname);
                if (ReconnectionTimer != null)
                {
                    ReconnectionTimer.Dispose();
                    ReconnectionTimer = null;
                }
                return;
            }

            if (!ExpectDisruption)  // only try once unless expect disruption
            {
                if (ReconnectionTimer != null)
                {
                    ReconnectionTimer.Dispose();
                    ReconnectionTimer = null;
                }
            }

            log.DebugFormat("Reconnecting to server {0}...", Hostname);
            if (!XenAdminConfigManager.Provider.Exiting)
            {
                InvokeHelper.Invoke(delegate ()
                {
                    /*ConnectionResult += new EventHandler<ConnectionResultEventArgs>(XenConnection_ConnectionResult);
                    CachePopulated += new EventHandler<EventArgs>(XenConnection_CachePopulated);*/
                    BeginConnect(false, _promptForNewPassword);
                });
                OnConnectionReconnecting();
            }
        }

        private void ReconnectCoordinatorTimer(object state)
        {
            if (IsConnected || !ConnectionsManager.XenConnectionsContains(this))
            {
                log.DebugFormat("Coordinator has been found for {0} at {1}", LastCoordinatorHostname, Hostname);
                return;
            }

            lock (PoolMembersLock)
            {
                if (PoolMemberIndex < PoolMembers.Count)
                {
                    Hostname = PoolMembers[PoolMemberIndex];
                    PoolMemberIndex++;
                }
            }

            OnConnectionMessageChanged(string.Format(Messages.CONNECTION_RETRYING_SUPPORTER, LastConnectionFullName.Ellipsise(25), Hostname));
            ReconnectCoordinator();
        }

        private void ReconnectCoordinator()
        {
            // Add an informational entry to the log
            string title = string.Format(Messages.CONNECTION_FINDING_COORDINATOR_TITLE, LastConnectionFullName);
            string descr = string.Format(Messages.CONNECTION_FINDING_COORDINATOR_DESCRIPTION, LastConnectionFullName, Hostname);
            var action = new DummyAction(title, descr);
            SetPoolAndHostInAction(action, null, PoolOpaqueRef);
            action.Run();
            log.DebugFormat("Looking for coordinator for {0} on {1}...", LastConnectionFullName, Hostname);

            if (!XenAdminConfigManager.Provider.Exiting)
            {
                InvokeHelper.Invoke(() => BeginConnect(false, _promptForNewPassword));
                OnConnectionReconnecting();
            }
        }

        private Pool getAPool(ICache objects, out string opaqueref)
        {
            var pools = objects.Pools;
            if (pools.Length > 0)
            {
                var pool = pools.First();
                opaqueref = pool.opaque_ref;
                return pool;
            }
            Trace.Assert(false);
            opaqueref = null;
            return null;
        }

        private void OnClearingCache()
        {
            ClearingCache?.Invoke(this);
        }

        private void OnConnectionResult(bool connected, string reason, Exception error)
        {
            if (ConnectionResult != null)
                ConnectionResult(this, new ConnectionResultEventArgs(connected, reason, error));
            OnConnectionStateChanged();
        }

        private void OnConnectionClosed()
        {
            ConnectionClosed?.Invoke(this);
            OnConnectionStateChanged();
        }

        private void OnConnectionLost()
        {
            ConnectionLost?.Invoke(this);
            OnConnectionStateChanged();
        }

        private void OnConnectionReconnecting()
        {
            ConnectionReconnecting?.Invoke(this);
            OnConnectionStateChanged();
        }

        private void OnBeforeConnectionEnd()
        {
            BeforeConnectionEnd?.Invoke(this);
        }

        private void OnConnectionStateChanged()
        {
            ConnectionStateChanged?.Invoke(this);
        }

        private void OnConnectionMessageChanged(string message)
        {
            ConnectionMessageChanged?.Invoke(this, message);
        }

        public void OnBeforeMajorChange(bool background)
        {
            BeforeMajorChange?.Invoke(this, background);
        }

        public void OnAfterMajorChange(bool background)
        {
            AfterMajorChange?.Invoke(this, background);
        }

        private void OnXenObjectsUpdated()
        {
            // Using BeginInvoke here means that the XenObjectsUpdated event gets fired after any
            // CollectionChanged events fired by ChangeableDictionary during Cache.UpdateFrom.
            InvokeHelper.BeginInvoke(delegate ()
            {
                if (XenObjectsUpdated != null)
                    XenObjectsUpdated(this, null);
            });
        }

        public T TryResolveWithTimeout<T>(XenRef<T> t) where T : XenObject<T>
        {
            log.DebugFormat("Resolving {0} {1}", t, t.opaque_ref);
            int timeout = 120; // two minutes;

            while (timeout > 0)
            {
                T obj = Resolve(t);
                if (obj != null)
                    return obj;

                Thread.Sleep(1000);
                timeout--;
            }

            if (typeof(T) == typeof(Host))
                throw new Failure(Failure.HOST_OFFLINE);
            throw new Failure(Failure.HANDLE_INVALID, typeof(T).Name, t.opaque_ref);
        }

        /// <summary>
        /// Stub to Cache.Resolve
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xenRef">May be null, in which case null is returned.</param>
        /// <returns></returns>
        public virtual T Resolve<T>(XenRef<T> xenRef) where T : XenObject<T>
        {
            return Cache.Resolve(xenRef);
        }

        /// <summary>
        /// Resolve every object in the given list.  Skip any references that don't resolve.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xenRefs">May be null, in which case the empty list is returned.</param>
        /// <returns></returns>
        public List<T> ResolveAll<T>(IEnumerable<XenRef<T>> xenRefs) where T : XenObject<T>
        {
            List<T> result = new List<T>();
            if (xenRefs != null)
            {
                foreach (XenRef<T> xenRef in xenRefs)
                {
                    T o = Resolve(xenRef);
                    if (o != null)
                        result.Add(o);
                }
            }
            return result;
        }

        public List<VDI> ResolveAllShownXenModelObjects(List<XenRef<VDI>> xenRefs, bool showHiddenObjects)
        {
            List<VDI> result = ResolveAll(xenRefs);
            result.RemoveAll(vdi => !vdi.Show(showHiddenObjects));
            return result;
        }

        public static T FindByUUIDXenObject<T>(string uuid) where T : XenObject<T>
        {
            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
            {
                T o = c.Cache.Find_By_Uuid<T>(uuid);
                if (o != null)
                    return o;
            }
            return null;
        }

        /// <summary>
        /// Find a XenObject corresponding to the given XenRef, or null if no such object is found.
        /// </summary>
        public static T FindByRef<T>(XenRef<T> needle) where T : XenObject<T>
        {
            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
            {
                T o = c.Resolve<T>(needle);
                if (o != null)
                    return o;
            }
            return null;
        }

        public static string ConnectedElsewhere(string hostname)
        {
            lock (ConnectionsManager.ConnectionsLock)
            {
                foreach (IXenConnection connection in ConnectionsManager.XenConnections)
                {
                    Pool pool = Helpers.GetPoolOfOne(connection);
                    if (pool == null)
                        continue;

                    Host coordinator = connection.Resolve(pool.master);
                    if (coordinator == null)
                        continue;

                    if (coordinator.address == hostname)
                    {
                        // we have tried to connect to a supporter that is a member of a pool we are already connected to.
                        return pool.Name();
                    }
                }
            }
            return null;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            Hostname = reader["Hostname"];
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Hostname", Hostname);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposing this class will make it unusable - make sure you want to do this
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ClearingCache = null;
                    CachePopulated = null;
                    ConnectionResult = null;
                    ConnectionStateChanged = null;
                    ConnectionLost = null;
                    ConnectionClosed = null;
                    ConnectionReconnecting = null;
                    BeforeConnectionEnd = null;
                    ConnectionMessageChanged = null;
                    BeforeMajorChange = null;
                    AfterMajorChange = null;
                    XenObjectsUpdated = null;

                    if (ReconnectionTimer != null)
                        ReconnectionTimer.Dispose();
                    if (cacheUpdateTimer != null)
                        cacheUpdateTimer.Dispose();
                }
                disposed = true;
            }
        }


        #endregion
    }

    public class ServerNotSupported : DisconnectionException
    {
        public override string Message => string.Format(Messages.SERVER_TOO_OLD,
            BrandManager.BrandConsole, BrandManager.ProductVersion70);
    }

    public class ConnectionExists : DisconnectionException
    {
        protected readonly IXenConnection Connection;

        public ConnectionExists(IXenConnection connection)
        {
            Connection = connection;
        }

        public override string Message
        {
            get
            {
                if (Connection == null)
                    return Messages.CONNECTION_EXISTS_NULL;

                return string.Format(Messages.CONNECTION_EXISTS, Connection.Hostname);

            }
        }

        public virtual string GetDialogMessage()
        {
            if (Connection == null)
                return Messages.CONNECTION_EXISTS_NULL;

            Pool p = Helpers.GetPool(Connection);
            if (p == null)
                return string.Format(Messages.ALREADY_CONNECTED, Connection.Hostname);

            return string.Format(Messages.SUPPORTER_ALREADY_CONNECTED, Connection.Hostname, p.Name());
        }
    }

    internal class BadRestoreDetected : ConnectionExists
    {
        public BadRestoreDetected(IXenConnection xc)
            : base(xc)
        {
        }

        public override string Message
        {
            get
            {
                if (Connection == null)
                    return Messages.CONNECTION_EXISTS_NULL;

                return string.Format(Messages.BAD_RESTORE_DETECTED, Connection.Name);
            }
        }

        public override string GetDialogMessage()
        {
            return Message;
        }
    }
}
