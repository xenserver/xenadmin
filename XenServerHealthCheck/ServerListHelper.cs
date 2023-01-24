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
using XenAPI;
using XenCenterLib;

namespace XenServerHealthCheck
{
    public class ServerInfo
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public System.Threading.Tasks.Task task { get; set; }
    }

    public class ServerListHelper
    {
        private ServerListHelper() { }
        public static readonly ServerListHelper instance = new ServerListHelper();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<ServerInfo> serverList;

        private static readonly Object serverListLock = new Object();
        public List<ServerInfo> GetServerList()
        {
            List<ServerInfo> currentList;
            lock (serverListLock)
            {
                currentList = new List<ServerInfo>(serverList);
            }
            return currentList;
        }

        public void Init()
        {
            lock (serverListLock)
            {
                serverList = new List<ServerInfo>();
                string[] encServerList = Properties.Settings.Default.ServerList ?? new string[0];
                foreach (string encServer in encServerList)
                {
                    string decryptCredential;
                    try
                    {
                        decryptCredential = EncryptionUtils.Unprotect(encServer);
                    }
                    catch (Exception e)
                    {
                        log.Warn("Could not unprotect server list.", e);
                        return; //return and not continue; same behaviour as XenCenter settings
                    }

                    string[] decryptCredentialComps = decryptCredential.Split(SEPARATOR);
                    if (decryptCredentialComps.Length < 3)
                    {
                        log.Warn("Server list was not decrypted to valid entries.");
                        return; //return and not continue; same behaviour as XenCenter settings
                    }
                    
                    var connection = new ServerInfo
                    {
                        HostName = decryptCredentialComps[0],
                        UserName = decryptCredentialComps[1],
                        Password = decryptCredentialComps[2],
                        task = null
                    };
                    serverList.Add(connection);
                }
            }
        }

        public const char SEPARATOR = '\x202f'; // narrow non-breaking space.
        private string ProtectCredential(ServerInfo connection)
        {
            string Host = connection.HostName ?? string.Empty;
            string username = connection.UserName ?? string.Empty;
            string passwordSecret = connection.Password ?? string.Empty;

            return EncryptionUtils.Protect(String.Join(SEPARATOR.ToString(), new[] { Host, username, passwordSecret }));
        }

        public void removeServerCredential(string HostName)
        {
            lock (serverListLock)
            {
                foreach (ServerInfo con in serverList)
                {
                    if (HostName == con.HostName)
                    {
                        serverList.Remove(con);
                        updateServerList();
                        return;
                    }
                }
            }
        }

        public bool UpdateServerCredential(ServerInfo server, string coordinatorName)
        {
            lock (serverListLock)
            {
                bool needRefresh = true;
                foreach (ServerInfo con in serverList)
                {
                    if (coordinatorName == con.HostName)
                    {
                        needRefresh = false;
                        break;
                    }
                }

                serverList.Remove(server);
                if (needRefresh)
                {
                    server.HostName = coordinatorName;
                    serverList.Add(server);
                }
                
                updateServerList();
                return needRefresh;
            }
        }

        private void updateServerList()
        {
            List<string> encList = new List<string>();
            foreach (ServerInfo connection in serverList)
            {
                encList.Add(ProtectCredential(connection));
            }
            Properties.Settings.Default.ServerList = encList.ToArray();
            Properties.Settings.Default.Save();
        }

        public bool UpdateServerInfo(ServerInfo server)
        {
            lock (serverListLock)
            {
                foreach (ServerInfo con in serverList)
                {
                    if (server.HostName == con.HostName)
                    {
                        serverList.Remove(con);
                        serverList.Add(server);
                        updateServerList();
                        return true;
                    }
                }
                return false;
            }
        }

        public void ClearServerList()
        {
            lock (serverListLock)
            {
                serverList.Clear();
                updateServerList();
            }
        }

        public void UpdateServerCredential(string credential)
        {
            log.Info("Receive credential update message");

            string decryptCredential;
            try
            {
                decryptCredential = EncryptionUtils.UnprotectForLocalMachine(credential);
            }
            catch (Exception e)
            {
                log.Warn("Could not unprotect credential.", e);
                return;
            }

            string[] decryptCredentialComps = decryptCredential.Split(SEPARATOR);

            if (decryptCredentialComps.Length != 1 && decryptCredentialComps.Length != 3)
                return;

            if (decryptCredentialComps.Length == 3 && (string.IsNullOrEmpty(decryptCredentialComps[1]) || string.IsNullOrEmpty(decryptCredentialComps[2])))
                return; //ignore null or empty username and password

            lock (serverListLock)
            {
                if (decryptCredentialComps.Length == 3)
                {//Add credential
                    System.Threading.Tasks.Task originalTask = null;
                    foreach (ServerInfo connection in serverList)
                    {
                        if (connection.HostName == decryptCredentialComps[0])
                        {
                            originalTask = connection.task;
                            serverList.Remove(connection);
                            log.Info("Refresh credential");
                            break;
                        }
                    }
                    ServerInfo newConnection = new ServerInfo();
                    newConnection.HostName = decryptCredentialComps[0];
                    newConnection.UserName = decryptCredentialComps[1];
                    newConnection.Password = decryptCredentialComps[2];
                    newConnection.task = originalTask;
                    serverList.Add(newConnection);
                    log.InfoFormat("Add credential, current credential size is {0}", serverList.Count);
                }
                else if (decryptCredentialComps.Length == 1)
                {//remove credential
                    foreach (ServerInfo connection in serverList)
                    {
                        if (connection.HostName == decryptCredentialComps[0])
                        {
                            serverList.Remove(connection);
                            log.InfoFormat("Remove credential, current credential size is {0}", serverList.Count);
                            break;
                        }
                    }
                }

                updateServerList();
            }
        }

        public void UpdateProxy(string proxy)
        {
            log.Info("Receive proxy update message");

            try
            {
                string[] proxySettings = proxy.Split(SEPARATOR);
                if (proxySettings.Length < 2)
                    return;

                HTTPHelper.ProxyStyle proxyStyle = (HTTPHelper.ProxyStyle)Int32.Parse(proxySettings[1]);

                switch (proxyStyle)
                {
                    case HTTPHelper.ProxyStyle.SpecifiedProxy:
                        Properties.Settings.Default.ProxySetting = (Int32)proxyStyle;
                        Properties.Settings.Default.ProxyAddress = proxySettings[2];
                        Properties.Settings.Default.ProxyPort = Int32.Parse(proxySettings[3]);
                        Properties.Settings.Default.ConnectionTimeout = Int32.Parse(proxySettings[4]);
                        Properties.Settings.Default.BypassProxyForServers = bool.Parse(proxySettings[5]);
                        Properties.Settings.Default.ProvideProxyAuthentication = bool.Parse(proxySettings[6]);
                        Properties.Settings.Default.ProxyUsername = EncryptionUtils.Protect(EncryptionUtils.UnprotectForLocalMachine(proxySettings[7]));
                        Properties.Settings.Default.ProxyPassword = EncryptionUtils.Protect(EncryptionUtils.UnprotectForLocalMachine(proxySettings[8]));
                        Properties.Settings.Default.ProxyAuthenticationMethod = Int32.Parse(proxySettings[9]);
                        break;

                    case HTTPHelper.ProxyStyle.SystemProxy:
                        Properties.Settings.Default.ProxySetting = (Int32)proxyStyle;
                        break;

                    default:
                        Properties.Settings.Default.ProxySetting = (Int32)proxyStyle;
                        break;
                }

                Properties.Settings.Default.Save();
                XenServerHealthCheckService.ReconfigureConnectionSettings();
            }
            catch (Exception e)
            {
                log.Error("Error parsing 'ProxySetting' from XenCenter", e);
            }
        }

        public void UpdateXenCenterMetadata(string message)
        {
            log.Info("Receive XenCenter metadata update message");

            try
            {
                string[] metadata = message.Split(SEPARATOR);
                if (metadata.Length != 2) 
                    return;
                Properties.Settings.Default.XenCenterMetadata = EncryptionUtils.UnprotectForLocalMachine(metadata[1]);
                Properties.Settings.Default.Save();
            }
            catch (Exception e)
            {
                log.Error("Error parsing 'XenCenterMetadata' from XenCenter", e);
            }
        }


        public string XenCenterMetadata
        {
            get { return Properties.Settings.Default.XenCenterMetadata; }
        }
    }
}