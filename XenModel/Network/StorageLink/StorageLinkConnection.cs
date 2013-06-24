/* Copyright (c) Citrix Systems Inc. 
 * All rights reserved. 
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
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Services.Protocols;
using XenAdmin.Network.StorageLink.Service;
using XenAPI;
using XenAdmin.Core;
using System.Xml;
using System.Text;


namespace XenAdmin.Network.StorageLink
{
    public enum StorageLinkConnectionState
    {
        Connecting,
        Connected,
        Disconnected
    }

    public partial class StorageLinkConnection
    {
        private Exception savedException = null;

        public Exception SavedException
        {
            get { return savedException; }
            set { savedException = value; }
        }
        

        #region DirectSynchronizer class

        private class DirectSynchronizer : ISynchronizeInvoke
        {
            #region ISynchronizeInvoke Members

            public IAsyncResult BeginInvoke(Delegate method, object[] args)
            {
                throw new NotImplementedException();
            }

            public object EndInvoke(IAsyncResult result)
            {
                throw new NotImplementedException();
            }

            public object Invoke(Delegate method, object[] args)
            {
                return method.DynamicInvoke(args);
            }

            public bool InvokeRequired
            {
                get { throw new NotImplementedException(); }
            }

            #endregion
        }

        #endregion

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly EventListener _listener;
        private readonly ISynchronizeInvoke _synchronizer;
        private readonly object _syncObject = new object();
        private bool _ending;
        private IStorageLinkWebService _service;
        private StorageLinkConnectionState _connectionState = StorageLinkConnectionState.Disconnected;
        private bool refreshInProgress;

        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Host { get; private set; }
        public StorageLinkCache Cache { get; private set; }
        public string Error { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageLinkConnection"/> class.
        /// </summary>
        /// <param name="synchronizer">The synchronizer used for synchronizing the cache update. Usually <see cref="MainWindow"/>.</param>
        /// <param name="host">The StorageLink server host.</param>
        /// <param name="username">The StorageLink server username.</param>
        /// <param name="password">The StorageLink server password.</param>
        public StorageLinkConnection(ISynchronizeInvoke synchronizer, string host, string username, string password)
        {
            Util.ThrowIfStringParameterNullOrEmpty(host, "host");
            Util.ThrowIfStringParameterNullOrEmpty(username, "username");

            _listener = new EventListener(this);
            Cache = new StorageLinkCache();
            _synchronizer = synchronizer ?? new DirectSynchronizer();
            Host = host;
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageLinkConnection"/> class. This constructor allows a mock
        /// web service to be passed in.
        /// </summary>
        /// <param name="synchronizer">The synchronizer used for synchronizing the cache update. Usually <see cref="MainWindow"/>.</param>
        /// <param name="service">The web service.</param>
        public StorageLinkConnection(ISynchronizeInvoke synchronizer, IStorageLinkWebService service) :
            this(synchronizer, service.Host, service.Username, service.Password)
        {
            Util.ThrowIfParameterNull(service, "service");
            _service = service;
        }

        protected virtual void OnConnectionStateChanged(ConnectionStateChangedEventArgs e)
        {
            var handler = ConnectionStateChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public StorageLinkConnectionState ConnectionState
        {
            get
            {
                lock (_syncObject)
                {
                    return _connectionState;
                }
            }
            private set
            {
                if (value != _connectionState)
                {
                    lock (_syncObject)
                    {
                        _connectionState = value;
                    }
                    OnConnectionStateChanged(new ConnectionStateChangedEventArgs(value));
                }
            }
        }

        public void BeginConnect()
        {
            if (_synchronizer != null)
            {
                lock (_syncObject)
                {
                    if (ConnectionState != StorageLinkConnectionState.Disconnected)
                    {
                        return;
                    }
                    _connectionState = StorageLinkConnectionState.Connecting;
                }
                OnConnectionStateChanged(new ConnectionStateChangedEventArgs(_connectionState));

                ThreadPool.QueueUserWorkItem(RunCacheLoop);
       
            }
        }

        public void EndConnect()
        {
            lock (_syncObject)
            {
                if (_ending)
                {
                    return;
                }

                _ending = true;
            }
        }

        public bool IsConnectionEnding
        {
            get
            {
                lock (_syncObject)
                {
                    return _ending;
                }
            }
        }

        public StorageLinkJobInfo AddStorageSystem(StorageLinkAdapter adapter, int port, string address, string username, string password, string ns)
        {
            Util.ThrowIfParameterNull(adapter, "adapter");

            managementCredentials cred = new managementCredentials();
            cred.storageAdapterId = adapter.opaque_ref;
            cred.portNumber = port;
            cred.ipAddress = address;
            cred.username = username;
            cred.password = password;
            cred.ns = ns;

            managementCredentials credInfo = new managementCredentials();

            try
            {
                jobInfo jobInfo = _service.addStorageManagementCredentials(cred, (int)StorageLinkEnums.FlagsType.ASYNC, out credInfo);
                return GetJobInfo(jobInfo.jobId);
            }
            catch (SoapException e)
            {
                throw ConvertSoapException(e);
            }
        }

        public void CancelJob(string jobId)
        {
            try
            {
                _service.cancelJob(jobId);
            }
            catch (SoapException)
            {
            }
            catch (WebException)
            {
            }
        }

        public StorageLinkJobInfo GetJobInfo(string jobId)
        {
            jobInfo jobInfo = _service.getJobInfo(jobId, string.Empty, 0);
            string errorText = null;

            if (jobInfo.errInfo != null)
            {
                var sb = new StringBuilder();

                if (jobInfo.errInfo.errorMessage != null)
                {
                    sb.AppendLine("Message: " + jobInfo.errInfo.errorMessage.defaultMessage);
                }
                sb.AppendLine("File:    " + jobInfo.errInfo.errorFile);
                sb.AppendLine("Line:    " + jobInfo.errInfo.errorLine);
                sb.AppendLine("Func:    " + jobInfo.errInfo.errorFunction);

                log.Error("CSLG Exception " + sb.ToString());

                errorText = jobInfo.errInfo.errorMessage.defaultMessage;
            }

            return new StorageLinkJobInfo(this, jobId, (int)jobInfo.progress, jobInfo.name.defaultMessage, jobInfo.description.defaultMessage, jobInfo.completedDateStamp, (StorageLinkEnums.JobState)jobInfo.jobStatus, errorText);
        }

        private static Exception ConvertSoapException(SoapException e)
        {
            //log.Error("CSLG Exception", e);

            Match m = Regex.Match(e.Message, @"<Fault>(.+)</Fault>", RegexOptions.Singleline);

            if (m.Success)
            {
                string text = m.Groups[1].Value;
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(m.Value);
                    text = doc.InnerText;
                }
                catch (XmlException)
                {
                }
                return new Failure(text);
            }
            return e;
        }

        public void SetLicenseServer(string address, int port)
        {
            Util.ThrowIfStringParameterNullOrEmpty(address, "address");

            try
            {
                _service.setLicenseServerInfo(address, port);
            }
            catch (SoapException e)
            {
                throw ConvertSoapException(e);
            }
        }

        public void GetLicenseServer(out string address, out int port)
        {
            licenseServerInfo info = _service.getLicenseServerInfo();

            address = info.server;
            port = (int)info.port;
        }

        public List<StorageLinkRepository> FullSRRescan()
        {
            try
            {
                var repositories = new List<StorageLinkRepository>();

                storageRepositoryInfo[] srs;
                _service.enumStorageRepositories(string.Empty, string.Empty, (int)StorageLinkEnums.FlagsType.REFRESH_CACHE, out srs);

                foreach (storageRepositoryInfo sr in srs)
                {
                    if (sr.storageSystemInfo != null && sr.storagePoolInfo != null && !string.IsNullOrEmpty(sr.storageSystemInfo.objectId) && !string.IsNullOrEmpty(sr.storagePoolInfo.objectId))
                    {
                        repositories.Add(new StorageLinkRepository(this,
                            sr.objectId,
                            sr.friendlyName,
                            Cache.Server,
                            sr.storageSystemId,
                            sr.storagePoolId,
                            (StorageLinkEnums.RaidType)sr.raidType,
                            (StorageLinkEnums.ProvisioningType)sr.provisioningType,
                            (StorageLinkEnums.ProvisioningOptions)sr.useDeduplication,
                            sr.hostGroupUuid));
                    }
                }

                return repositories;

            }
            catch (SoapException e)
            {
                throw ConvertSoapException(e);
            }
        }

        public StorageLinkJobInfo RemoveStorageSystem(StorageLinkSystem system)
        {
            Util.ThrowIfParameterNull(system, "system");

            try
            {
                foreach (managementCredentials creds in _service.enumStorageManagementCredentials())
                {
                    foreach (string ssid in creds.ssidList)
                    {
                        if (ssid == system.opaque_ref)
                        {
                            jobInfo jobInfo = _service.removeStorageManagementCredentials(string.Empty, creds.uuid, (int)StorageLinkEnums.FlagsType.ASYNC);
                            return GetJobInfo(jobInfo.jobId);
                        }
                    }
                }
            }
            catch (SoapException e)
            {
                throw ConvertSoapException(e);
            }
            return null;
        }

        private void AddHost(string address, string username, string password)
        {
            try
            {
                hostInfo hostInfo;
                _service.addHost(address, username, password, (int)StorageLinkEnums.HypervisorType.XEN, (int)StorageLinkEnums.LicenseType.LICENSE_ALL, (int)StorageLinkEnums.FlagsType.NONE, out hostInfo);
            }
            catch (SoapException e)
            {
                if (e.Detail == null || !e.Detail.InnerText.Contains("ALREADY_MANAGED_BY_THE_SERVICE"))
                {
                    throw;
                }
                // already added to SL service. Ignore.
            }
        }

        public void AddStorageVolumesToStorageRepository(StorageLinkRepository storageLinkRepository, IEnumerable<string> storageVolumes)
        {
            _service.addStorageToRepository(storageLinkRepository.opaque_ref, new List<string>(storageVolumes).ToArray(), 0);
        }

        public void RemoveStorageVolumesFromStorageRepository(StorageLinkRepository storageLinkRepository, IEnumerable<string> storageVolumes)
        {
            _service.removeStorageFromRepository(storageLinkRepository.opaque_ref, new List<string>(storageVolumes).ToArray(), 0);
        }

        public void AddXenConnection(IXenConnection connection)
        {
            Util.ThrowIfParameterNull(connection, "connection");

            try
            {
                AddHost(connection.Hostname, connection.Username, connection.Password);
            }
            catch (SoapException e)
            {
                if (e.Detail != null && e.Detail.InnerText.Contains("DNS_LOOKUP_FAILED"))
                {
                    // couldn't find host from name. Try again using IP address.
                    Host master = Helpers.GetMaster(connection);
                    if (master != null)
                    {
                        AddHost(master.address, connection.Username, connection.Password);
                    }
                }
                else
                {
                    throw ConvertSoapException(e);
                }
            }
        }

        public void Refresh()
        {
            RefreshInProgress = true;
        }

        public bool RefreshInProgress
        {
            get
            {
                lock (_syncObject)
                {
                    return refreshInProgress;
                }
            }
            private set
            {
                lock (_syncObject)
                {
                    refreshInProgress = value;
                }
            }
        }

        public void SetPassword(string username, string oldPassword, string newPassword)
        {
            Util.ThrowIfStringParameterNullOrEmpty(username, "username");
            Util.ThrowIfParameterNull(oldPassword, "oldPassword");
            Util.ThrowIfParameterNull(newPassword, "newPassword");

            try
            {
                if (oldPassword != Password)
                {
                    throw new InvalidOperationException(Messages.INCORRECT_OLD_PASSWORD);
                }

                _service.setServicePassword(username, newPassword, (int)StorageLinkEnums.FlagsType.NONE);

                Username = username;
                Password = newPassword;

                // now disconnect and reconnect
                EndConnect();
                Debug.Assert(_ending);

                while (true)
                {
                    lock (_syncObject)
                    {
                        if (!_ending)
                        {
                            break;
                        }
                    }
                }

                BeginConnect();

            }
            catch (SoapException e)
            {
                throw ConvertSoapException(e);
            }
        }

        private void Invoke(Action method)
        {
            _synchronizer.Invoke(method, new object[0]);
        }

        private void RunCacheLoop(object state)
        {
            try
            {
                if (_service == null)
                {
                    try
                    {
                        _service = new StorageLinkSSLWebService(Username, Password, Host);
                    }
                    catch( UriFormatException e )
                    {
                        savedException = e;
                        ConnectionState = StorageLinkConnectionState.Disconnected;
                        EndConnect();
                    }
                }

                while (!IsConnectionEnding)
                {
                    if (ConnectionState != StorageLinkConnectionState.Connected || RefreshInProgress)
                    {
                        // not connected: try and update entire cache
                        try
                        {
                            Update();
                            Error = string.Empty;
                            ConnectionState = StorageLinkConnectionState.Connected;
                        }
                        catch (SoapException e)
                        {
                            Error = ConvertSoapException(e).Message;
                            ConnectionState = StorageLinkConnectionState.Disconnected;
                        }
                        catch (WebException e)
                        {
                            Error = e.Message;
                            ConnectionState = StorageLinkConnectionState.Disconnected;
                        }
                        RefreshInProgress = false;
                    }

                    if (ConnectionState == StorageLinkConnectionState.Connected)
                    {
                        try
                        {
                            _listener.ProcessEvents();
                        }
                        catch (SoapException e)
                        {
                            Error = ConvertSoapException(e).Message;
                            ConnectionState = StorageLinkConnectionState.Disconnected;
                        }
                        catch (WebException e)
                        {
                            Error = e.Message;
                            ConnectionState = StorageLinkConnectionState.Disconnected;
                        }
                    }

                    Thread.Sleep(ConnectionState == StorageLinkConnectionState.Connected ? 2000 : 5000);
                }
            }
            finally
            {
                if (_service != null)
                {
                    _service.Dispose();
                    _service = null;
                }
                lock (_syncObject)
                {
                    _ending = false;
                    _connectionState = StorageLinkConnectionState.Disconnected;
                }
                OnConnectionStateChanged(new ConnectionStateChangedEventArgs(_connectionState));
            }
        }

        private void Update()
        {
            var systems = new List<StorageLinkSystem>();
            var pools = new List<StorageLinkPool>();
            var adapters = new List<StorageLinkAdapter>();
            var volumes = new List<StorageLinkVolume>();
            var repositories = new List<StorageLinkRepository>();
            var server = new StorageLinkServer(this, Host + " " + Username, Host);

            if (Cache.Server == null)
            {
                Invoke(() => Cache.Update(server, new List<StorageLinkSystem>(), new List<StorageLinkPool>(), new List<StorageLinkAdapter>(), new List<StorageLinkVolume>(), new List<StorageLinkRepository>()));
            }

            var flags = StorageLinkEnums.FlagsType.NONE;

            if (RefreshInProgress)
            {
                flags = StorageLinkEnums.FlagsType.DEEP_REFRESH_CACHE;
            }

            try
            {
                foreach (storageAdapterInfo adapterInfo in _service.enumStorageAdapters())
                {
                    adapters.Add(new StorageLinkAdapter(this, adapterInfo.objectId, adapterInfo.friendlyName, adapterInfo.name, adapterInfo.storageAdapterCredInputOptions, adapterInfo.defaultNamespace, adapterInfo.defaultPortNumber, adapterInfo.isSMIS));
                }

                storageSystemInfo[] storageSystems;
                _service.enumStorageSystems(string.Empty, (int)flags, out storageSystems);

                server = new StorageLinkServer(this, _service.Host + " " + Username, Host);

                foreach (storageSystemInfo systemInfo in storageSystems)
                {
                    StorageLinkSystem system = new StorageLinkSystem(this,
                        systemInfo.objectId,
                        systemInfo.friendlyName,
                        server,
                        systemInfo.storageSystemId,
                        systemInfo.serialNum,
                        systemInfo.model,
                        systemInfo.displayName,systemInfo.storageAdapterId,
                        (StorageLinkEnums.StorageSystemCapabilities)systemInfo.capabilities);

                    try
                    {
                        storagePoolInfo[] storagePools;
                        _service.enumStoragePools(systemInfo.storageSystemId, string.Empty, (int)StorageLinkEnums.FlagsType.NONE, out storagePools);

                        storageVolumeInfo[] vols;
                        _service.enumStorageNodes(systemInfo.storageSystemId, string.Empty, string.Empty, (int)flags, out vols);

                        foreach (storagePoolInfo poolInfo in storagePools)
                        {
                            StorageLinkEnums.RaidType raidType = (StorageLinkEnums.RaidType)poolInfo.supportedRaidTypes;
                            StorageLinkEnums.ProvisioningType provTypes = (StorageLinkEnums.ProvisioningType)poolInfo.supportedProvisioningTypes;

                            pools.Add(new StorageLinkPool(this,
                                poolInfo.objectId,
                                poolInfo.friendlyName,
                                poolInfo.parentPool,
                                poolInfo.storageSystemId,
                                poolInfo.sizeInMB,
                                poolInfo.sizeInMB - poolInfo.freeSpaceInMB,
                                (StorageLinkEnums.RaidType)poolInfo.supportedRaidTypes,
                                (StorageLinkEnums.ProvisioningType)poolInfo.supportedProvisioningTypes));
                        }

                        volumes.AddRange(Array.ConvertAll(vols, v => new StorageLinkVolume(this, v.objectId, server, v.friendlyName, v.storageSystemId, v.storagePoolId, v.sizeInMB, v.usedSpaceInMB)));
                    }
                    catch (SoapException)
                    {
                        //log.Error(e.Message, e);
                        continue;
                    }
                    catch (WebException)
                    {
                        //log.Error(e.Message, e);
                        continue;
                    }
                    systems.Add(system);
                }

                storageRepositoryInfo[] srs;
                _service.enumStorageRepositories(string.Empty, string.Empty, (int)flags, out srs);

                foreach (storageRepositoryInfo sr in srs)
                {
                    if (systems.Find(s => s.opaque_ref == sr.storageSystemId) != null && pools.Find(p => p.opaque_ref == sr.storagePoolId) != null)
                    {
                        repositories.Add(new StorageLinkRepository(this,
                            sr.objectId,
                            sr.friendlyName,
                            server,
                            sr.storageSystemId,
                            sr.storagePoolId,
                            (StorageLinkEnums.RaidType)sr.raidType,
                            (StorageLinkEnums.ProvisioningType)sr.provisioningType,
                            (StorageLinkEnums.ProvisioningOptions)sr.useDeduplication,
                            sr.hostGroupUuid));
                    }
                }
            }
            finally
            {
                Invoke(() => Cache.Update(server, systems, pools, adapters, volumes, repositories));
            }
        }

        public override bool Equals(object obj)
        {
            StorageLinkConnection other = obj as StorageLinkConnection;
            return other != null && other.Host == Host && other.Username == Username;
        }

        public override int GetHashCode()
        {
            return (Host + " " + Username).GetHashCode();
        }
    }

    public class ConnectionStateChangedEventArgs : EventArgs
    {
        public StorageLinkConnectionState ConnectionState { get; private set; }

        public ConnectionStateChangedEventArgs(StorageLinkConnectionState state)
        {
            ConnectionState = state;
        }
    }
}
