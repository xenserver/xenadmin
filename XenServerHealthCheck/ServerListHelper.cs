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
using XenAdmin.Core;
using XenAdmin.Network;

namespace XenServerHealthCheck
{
    public class ServerListHelper
    {
        private ServerListHelper() { }
        public static readonly ServerListHelper instance = new ServerListHelper();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<IXenConnection> serverList;

        private static readonly Object serverListLock = new Object();
        public List<IXenConnection> GetServerList()
        {
            List<IXenConnection> currentList;
            lock (serverListLock)
            {
                currentList = new List<IXenConnection>(serverList);
            }
            return currentList;
        }

        public void Init()
        {
            lock (serverListLock)
            {
                serverList = new List<IXenConnection>();
                string[] encServerList = Properties.Settings.Default.ServerList ?? new string[0];
                foreach (string encServer in encServerList)
                {
                    string decryptCredential = EncryptionUtils.Unprotect(encServer);
                    string[] decryptCredentialComps = decryptCredential.Split(SEPARATOR);
                    IXenConnection connection = new XenConnection();
                    connection.Hostname = decryptCredentialComps[0];
                    connection.Username = decryptCredentialComps[1];
                    connection.Password = decryptCredentialComps[2];
                    serverList.Add(connection);
                }
            }
        }

        private const char SEPARATOR = '\x202f'; // narrow non-breaking space.
        private string ProtectCredential(IXenConnection connection)
        {
            string Host = connection.Hostname ?? string.Empty;
            string username = connection.Username ?? string.Empty;
            string passwordSecret = connection.Password ?? string.Empty;

            return EncryptionUtils.Protect(String.Join(SEPARATOR.ToString(), new[] { Host, username, passwordSecret }));
        }

        public void removeServerCredential(IXenConnection connection)
        {
            lock (serverListLock)
            {
                foreach (IXenConnection con in serverList)
                {
                    if (connection.Hostname == con.Hostname)
                    {
                        serverList.Remove(con);
                        break;
                    }
                }
                updateServerList();
            }
        }

        private void updateServerList()
        {
            List<string> encList = new List<string>();
            foreach (IXenConnection connection in serverList)
            {
                encList.Add(ProtectCredential(connection));
            }
            Properties.Settings.Default.ServerList = encList.ToArray();
            Properties.Settings.Default.Save();
        }

        public void UpdateServerCredential(string credential)
        {
            log.Info("Receive credential update message");

            string decryptCredential = EncryptionUtils.UnprotectForLocalMachine(credential);
            string[] decryptCredentialComps = decryptCredential.Split(SEPARATOR);

            lock (serverListLock)
            {
                if (decryptCredentialComps.Length == 3)
                {//Add credential
                    foreach (IXenConnection connection in serverList)
                    {
                        if (connection.Hostname == decryptCredentialComps[0])
                        {
                            serverList.Remove(connection);
                            log.Info("Refresh credential");
                            break;
                        }
                    }
                    IXenConnection newConnection = new XenConnection();
                    newConnection.Hostname = decryptCredentialComps[0];
                    newConnection.Username = decryptCredentialComps[1];
                    newConnection.Password = decryptCredentialComps[2];
                    serverList.Add(newConnection);

                    log.InfoFormat("Add credential, current credential size is {0}", serverList.Count);
                }
                else if (decryptCredentialComps.Length == 1)
                {//remove credential
                    foreach (IXenConnection connection in serverList)
                    {
                        if (connection.Hostname == decryptCredentialComps[0])
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
    }
}
