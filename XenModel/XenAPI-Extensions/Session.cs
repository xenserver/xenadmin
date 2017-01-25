/* Copyright (c) Citrix Systems, Inc. 
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
using System.IO;
using System.Text;
using CookComputing.XmlRpc;
using log4net.Core;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;


namespace XenAPI
{
    public partial class Session
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool IsElevatedSession = false;

        private Session(int timeout, IXenConnection connection, string url)
        {
            Connection = connection;
            _proxy = XmlRpcProxyGen.Create<Proxy>();
            _proxy.Url = url;
            _proxy.NonStandard = XmlRpcNonStandard.All;
            _proxy.Timeout = timeout;
            _proxy.UseIndentation = false;
            _proxy.UserAgent = UserAgent;
            _proxy.KeepAlive = true;
            _proxy.RequestEvent += LogRequest;
            _proxy.ResponseEvent += LogResponse;
            _proxy.Proxy = Proxy;
            // reverted because of CA-137829/CA-137959: _proxy.ConnectionGroupName = Guid.NewGuid().ToString(); // this will force the Session onto a different set of TCP streams (see CA-108676)
        }

        public Session(Proxy proxy, IXenConnection connection)
        {
            Connection = connection;
            _proxy = proxy;
        }

        public Session(Session session, Proxy proxy, IXenConnection connection)
            : this(proxy, connection)
        {
            InitAD(session);
        }

        public Session(int timeout, IXenConnection connection, string host, int port)
            : this(timeout, connection, GetUrl(host, port))
        {
        }

        /// <summary>
        /// Create a new Session instance, using the given instance and timeout.  The connection details and Xen-API session handle will be
        /// copied from the given instance, but a new connection will be created.  Use this if you want a duplicate connection to a host,
        /// for example when you need to cancel an operation that is blocking the primary connection.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="timeout"></param>
        public Session(Session session, IXenConnection connection, int timeout)
            : this(timeout, connection, session.Url)
        {
            InitAD(session);
        }

        private void InitAD(Session session)
        {
            _uuid = session.uuid;
            APIVersion = session.APIVersion;
            _userSid = session.UserSid;
            _subject = session.Subject;
            _isLocalSuperuser = session.IsLocalSuperuser;
            roles = session.Roles;
            permissions = session.Permissions;
        }

        /// <summary>
        /// When the CacheWarming flag is set, we output logging at Debug rather than Info level.
        /// This means that we don't spam the logs when the application starts.
        /// </summary>
        private bool _cacheWarming = false;
        public bool CacheWarming
        {
            get { return _cacheWarming; }
            set { _cacheWarming = value; }
        }

        private void LogRequest(object o, XmlRpcRequestEventArgs args)
        {
            Level logLevel;
            string xml = DumpStream(args.RequestStream, String.Empty);

            // Find the method name within the XML
            string methodName = "";
            int methodNameStart = xml.IndexOf("<methodName>");
            if (methodNameStart >= 0)
            {
                methodNameStart += 12;  // skip past "<methodName>"
                int methodNameEnd = xml.IndexOf('<', methodNameStart);
                if (methodNameEnd > methodNameStart)
                    methodName = xml.Substring(methodNameStart, methodNameEnd - methodNameStart);
            }

            if (CacheWarming)
                logLevel = Level.Debug;
            else if (methodName == "event.next" || methodName == "event.from" || methodName == "host.get_servertime" || methodName.StartsWith("task.get_"))  // these occur frequently and we don't need to know about them
                logLevel = Level.Debug;
            else
                logLevel = Level.Info;

            // Only log the full XML at Debug level because it may have sensitive data in: CA-80174
            if (logLevel == Level.Debug)
                LogMsg(logLevel, "Invoking XML-RPC method " +  methodName + ": " + xml);
            else
                LogMsg(logLevel, "Invoking XML-RPC method " + methodName);
        }

        private void LogResponse(object o, XmlRpcResponseEventArgs args)
        {
            if(log.IsDebugEnabled)
                LogMsg(Level.Debug, DumpStream(args.ResponseStream, "XML-RPC response: "));
        }

        private string DumpStream(Stream s, string header)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder(header);
                using (TextReader r = new StreamReader(s))
                {
                    string l;
                    while ((l = r.ReadLine()) != null)
                        stringBuilder.Append(l);
                }
                return stringBuilder.ToString();   
            }
            catch(OutOfMemoryException ex)
            {
                LogMsg(Level.Debug, "Session ran out of memory while trying to log the XML response stream: " + ex.Message);
                return String.Empty;
            }
        }

        private void LogMsg(Level logLevel, String msg)
        {
            if (logLevel == Level.Debug)
                log.Debug(msg);
            else if (logLevel == Level.Info)
                log.Info(msg);
            else
                System.Diagnostics.Trace.Assert(false, "Missing log level");
        }

        /// <summary>
        /// The i18n'd string for the 'Logged in as:' username (AD or local root).
        /// </summary>
        public string UserFriendlyName
        {
            get
            {
                if (IsLocalSuperuser)
                    return Messages.AD_LOCAL_ROOT_ACCOUNT;

                if (!string.IsNullOrEmpty(CurrentUserDetails.UserDisplayName))
                    return CurrentUserDetails.UserDisplayName.Ellipsise(50);

                if (!string.IsNullOrEmpty(CurrentUserDetails.UserName))
                    return CurrentUserDetails.UserName.Ellipsise(50);

                return Messages.UNKNOWN_AD_USER;
            }
        }

        /// <summary>
        /// Useful as a unique name for logging purposes
        /// </summary>
        public string UserLogName
        {
            get
            {
                if (IsLocalSuperuser)
                    return Messages.AD_LOCAL_ROOT_ACCOUNT;

                if (!string.IsNullOrEmpty(CurrentUserDetails.UserName))
                    return CurrentUserDetails.UserName;

                return UserSid;
            }
        }

        /// <summary>
        /// This gives either a friendly csv list of the sessions roles or a friendly string for Local root account.
        /// If Pre MR gives Pool Admin for AD users.
        /// </summary>
        public string FriendlyRoleDescription
        {
            get
            {
                if (IsLocalSuperuser || XenAdmin.Core.Helpers.GetMaster(Connection).external_auth_type != Auth.AUTH_TYPE_AD)
                    return Messages.AD_LOCAL_ROOT_ACCOUNT;

                return Role.FriendlyCSVRoleList(Roles);
            }
        }

        /// <summary>
        /// This gives either a friendly string for the dominant role on the session or a friendly string for Local root account.
        /// If Pre MR gives Pool Admin for AD users.
        /// </summary>
        public string FriendlySingleRoleDescription
        {
            get
            {
                if (IsLocalSuperuser || XenAdmin.Core.Helpers.GetMaster(Connection).external_auth_type != Auth.AUTH_TYPE_AD)
                    return Messages.AD_LOCAL_ROOT_ACCOUNT;

                //Sort roles from highest to lowest
                roles.Sort((r1, r2) => { return r2.CompareTo(r1); });
                //Take the highest role
                return roles[0].FriendlyName;
            }
        }

        public string ConnectionGroupName
        {
            get { return _proxy.ConnectionGroupName; }
            set { _proxy.ConnectionGroupName = value; }
        }
    }
}
