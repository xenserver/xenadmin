﻿/* Copyright (c) Citrix Systems, Inc. 
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
using XenAdmin;
using XenAdmin.Network;
using Newtonsoft.Json.Linq;


namespace XenAPI
{
    public partial class Session
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool IsElevatedSession = false;

        private Session(int timeout, IXenConnection connection, string url)
            : this(CreateProxy(url, timeout), connection)
        {
            proxy.RequestEvent += LogRequest;
            proxy.ResponseEvent += LogResponse;
        }

        public Session(Proxy proxy, IXenConnection connection)
        {
            Connection = connection;
            this.proxy = proxy;
        }

        public Session(int timeout, IXenConnection connection, string host, int port)
            : this(timeout, connection, GetUrl(host, port))
        {
        }

        public Session(Session session, Proxy proxy, IXenConnection connection)
            : this(proxy, connection)
        {
            InitAD(session);
        }

        /// <summary>
        /// Create a new Session instance, using the given instance and timeout.  The connection details and Xen-API session handle will be
        /// copied from the given instance, but a new connection will be created.  Use this if you want a duplicate connection to a host,
        /// for example when you need to cancel an operation that is blocking the primary connection.
        /// </summary>
        public Session(Session session, IXenConnection connection, int timeout)
        {
            if (session.JsonRpcClient != null)
            {
                JsonRpcClient = new JsonRpcClient(session.Url) {JsonRpcVersion = session.JsonRpcClient.JsonRpcVersion};
                JsonRpcClient.RequestEvent += LogJsonRequest;
            }
            else
            {
                proxy = CreateProxy(session.Url, timeout);
                proxy.RequestEvent += LogRequest;
                proxy.ResponseEvent += LogResponse;
            }

            Connection = connection;
            InitAD(session);
        }

        private void InitAD(Session session)
        {
            opaque_ref = session.opaque_ref;
            APIVersion = session.APIVersion;
            _userSid = session.UserSid;
            _subject = session.Subject;
            _isLocalSuperuser = session.IsLocalSuperuser;
            roles = session.Roles;
            permissions = session.Permissions;
        }

        private static Proxy CreateProxy(string url, int timeout)
        {
            var xmlrpcProxy = XmlRpcProxyGen.Create<Proxy>();
            xmlrpcProxy.Url = url;
            xmlrpcProxy.NonStandard = XmlRpcNonStandard.All;
            xmlrpcProxy.Timeout = timeout;
            xmlrpcProxy.UseIndentation = false;
            xmlrpcProxy.UserAgent = UserAgent;
            xmlrpcProxy.KeepAlive = true;

            xmlrpcProxy.Proxy = Proxy;
            // reverted because of CA-137829/CA-137959: _proxy.ConnectionGroupName = Guid.NewGuid().ToString(); // this will force the Session onto a different set of TCP streams (see CA-108676)
            return xmlrpcProxy;
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

        /// <summary>
        /// Do not log while downloading objects;
        /// also exclude calls occurring frequently, we don't need to know about them
        /// </summary>
        private bool CanLogCall(string call)
        {
            return CacheWarming ||
                   call == "event.next" || call == "event.from" || call == "host.get_servertime" ||
                   call.StartsWith("task.get_");
        }

        private void LogJsonRequest(string json)
        {
#if DEBUG
            string methodName = "";
            string parameters = "";

            try
            {
                JObject obj = JObject.Parse(json);
                methodName = obj.Property("method").Value.ToString();
                parameters = obj.Property("params").Value.ToString();
            }
            catch
            {
                //ignore
            }

            // only log the full parameters at Debug level because it may contain sensitive data
            if (CanLogCall(methodName))
                log.DebugFormat("Invoking JSON-RPC method '{0}' with params: {1}", methodName, parameters);
            else
                log.InfoFormat("Invoking JSON-RPC method '{0}'", methodName);
#else
            string methodName = json;

            if (CanLogCall(methodName))
                log.DebugFormat("Invoking JSON-RPC method '{0}'", methodName);
            else
                log.InfoFormat("Invoking JSON-RPC method '{0}'", methodName);
#endif
        }

        private void LogRequest(object o, XmlRpcRequestEventArgs args)
        {
            string xml = DumpStream(args.RequestStream, String.Empty);

            // Find the method name within the XML
            string methodName = "";
            int methodNameStart = xml.IndexOf("<methodName>");
            if (methodNameStart >= 0)
            {
                methodNameStart += 12; // skip past "<methodName>"
                int methodNameEnd = xml.IndexOf('<', methodNameStart);
                if (methodNameEnd > methodNameStart)
                    methodName = xml.Substring(methodNameStart, methodNameEnd - methodNameStart);
            }

            // do not log while downloading objects
            // also exclude calls occurring frequently; we don't need to know about them;
            // only log the full XML at Debug level because it may have sensitive data in: CA-80174

            if (CanLogCall(methodName))
            {
                log.DebugFormat("Invoking XML-RPC method {0}: {1}", methodName, xml);
            }
            else
            {
                log.InfoFormat("Invoking XML-RPC method {0}", methodName);
            }
        }

        private void LogResponse(object o, XmlRpcResponseEventArgs args)
        {
            if(log.IsDebugEnabled)
                log.DebugFormat(DumpStream(args.ResponseStream, "XML-RPC response: "));
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
                log.DebugFormat("Session ran out of memory while trying to log the XML response stream: {0}", ex.Message);
                return String.Empty;
            }
        }

        /// <summary>
        /// The i18n'd string for the 'Logged in as:' username (AD or local root).
        /// </summary>
        public string UserFriendlyName()
        {
            if (IsLocalSuperuser)
                return Messages.AD_LOCAL_ROOT_ACCOUNT;

            if (!string.IsNullOrEmpty(CurrentUserDetails.UserDisplayName))
                return CurrentUserDetails.UserDisplayName.Ellipsise(50);

            if (!string.IsNullOrEmpty(CurrentUserDetails.UserName))
                return CurrentUserDetails.UserName.Ellipsise(50);

            return Messages.UNKNOWN_AD_USER;
        }

        /// <summary>
        /// Useful as a unique name for logging purposes
        /// </summary>
        public string UserLogName()
        {
            if (IsLocalSuperuser)
                return Messages.AD_LOCAL_ROOT_ACCOUNT;

            if (!string.IsNullOrEmpty(CurrentUserDetails.UserName))
                return CurrentUserDetails.UserName;

            return UserSid;
        }

        /// <summary>
        /// This gives either a friendly csv list of the sessions roles or a friendly string for Local root account.
        /// If Pre MR gives Pool Admin for AD users.
        /// </summary>
        public string FriendlyRoleDescription()
        {
            if (IsLocalSuperuser || XenAdmin.Core.Helpers.GetMaster(Connection).external_auth_type != Auth.AUTH_TYPE_AD)
                return Messages.AD_LOCAL_ROOT_ACCOUNT;

            return Role.FriendlyCSVRoleList(Roles);
        }

        /// <summary>
        /// This gives either a friendly string for the dominant role on the session or a friendly string for Local root account.
        /// If Pre MR gives Pool Admin for AD users.
        /// </summary>
        public string FriendlySingleRoleDescription()
        {
            if (IsLocalSuperuser || XenAdmin.Core.Helpers.GetMaster(Connection).external_auth_type != Auth.AUTH_TYPE_AD)
                return Messages.AD_LOCAL_ROOT_ACCOUNT;

            //Sort roles from highest to lowest
            roles.Sort((r1, r2) => { return r2.CompareTo(r1); });
            //Take the highest role
            return roles[0].FriendlyName();
        }
    }
}
