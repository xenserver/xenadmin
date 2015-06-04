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
using System.Linq;
using System.Configuration;
using System.IO;
using System.Xml;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin;

namespace XenServerHealthCheck
{
    class ServerListHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<IXenConnection> GetServerList()
        {
            List<FileInfo> XCConfiglist = FindAllConfig();
            List<string> ServerList = new List<string>();

            foreach (FileInfo configFile in XCConfiglist)
            {
                List<string> encServerList = GetServerListString(configFile);
                foreach (string encServer in encServerList)
                {
                    try
                    {
                        string decryptedInfo = EncryptionUtils.Unprotect(encServer);
                        ServerList.Add(decryptedInfo);
                    }
                    catch (Exception exn)
                    {
                        log.Error("Decrypt server information failed", exn);
                    }
                }
            }
            return ConstructConnection(ServerList);
        }

        private static string ProfileListKey = @"SOFTWARE\\Microsoft\Windows NT\\CurrentVersion\\ProfileList\\";
        private static string ShellFolderKey = @"\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders\\";
        private static string UserShellFolderKey = @"\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders\\";
        
        private static List<string> GetAllPossiblePath()
        {
            List<string> PathList = new List<string>();
            
            Microsoft.Win32.RegistryKey ProfileList = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(ProfileListKey);
            if (ProfileList == null)
                return PathList;

            foreach (string ProfileKey in ProfileList.GetSubKeyNames())
            {
                Microsoft.Win32.RegistryKey profileKey = ProfileList.OpenSubKey(ProfileKey);
                if (profileKey == null)
                    continue;

                string profile = profileKey.GetValue("ProfileImagePath").ToString();
                profileKey.Close();

                Microsoft.Win32.RegistryKey shellfolder = Microsoft.Win32.Registry.Users.OpenSubKey(ProfileKey + ShellFolderKey);
                if (shellfolder == null)
                    continue;

                var AppData = shellfolder.GetValue("AppData");
                shellfolder.Close();
                if (AppData == null)
                {
                    shellfolder = Microsoft.Win32.Registry.Users.OpenSubKey(ProfileKey + UserShellFolderKey);
                    if (profileKey == null)
                        continue;
                    AppData = shellfolder.GetValue("AppData");
                    shellfolder.Close();
                }

                if (AppData == null)
                    continue;

                PathList.Add(AppData.ToString().Replace("%USERPROFILE%", profile));
            }
            ProfileList.Close();

            PathList = PathList.Distinct().ToList();
            return PathList;
        }
        
        private static string XenCenterDomainName = "XenCenterMain.exe";
        private static List<FileInfo> FindAllConfig()
        {
            List<string> PathList = GetAllPossiblePath();
            List<FileInfo> fileList = new List<FileInfo>();

            foreach (var path in PathList)
            {
                DirectoryInfo currentConfigFolder = new DirectoryInfo(path);
                foreach (DirectoryInfo subDir in currentConfigFolder.GetDirectories("*" + XenCenterDomainName + "*", SearchOption.AllDirectories))
                    foreach (FileInfo file in subDir.GetFiles("user.config", SearchOption.AllDirectories))
                        fileList.Add(file);
            }

            return fileList;
        }

        private static List<string> GetServerListString(FileInfo fileInfo)
        {
            List<string> encServerList = new List<string>();
            StreamReader reader = new StreamReader(new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));
            XmlDocument ConfigXML = new XmlDocument();
            string ConfigContent = reader.ReadToEnd();
            reader.Close();
            ConfigXML.LoadXml(ConfigContent);

            try
            {
                foreach (XmlNode setting in ConfigXML.GetElementsByTagName("setting"))
                {
                    if (setting.Attributes["name"].Value == "ServerList")
                    {
                        XmlDocument serilizedData = new XmlDocument();
                        serilizedData.LoadXml(setting.InnerXml);
                        foreach (XmlNode ServerInfo in serilizedData.GetElementsByTagName("string"))
                        {
                            encServerList.Add(ServerInfo.InnerText);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                log.Error("Get server list from XenCenter failed", exp);
            }

            return encServerList;
        }

        private const char SEPARATOR = '\x202f'; // narrow non-breaking space.
        private const string VNC_INDICATOR = "$VNC$";
        private static List<IXenConnection> ConstructConnection(List<string> ServerList)
        {
            List<IXenConnection> Connections = new List<IXenConnection>();
            foreach (string entry in ServerList)
            {
                string[] entryComps = entry.Split(SEPARATOR);
                if (entryComps.Length < 3 || entryComps.Length > 7)
                {
                    return Connections;
                }
            }

            foreach (string entry in ServerList)
            {
                // $VNC$, VM UUID, unused, password
                // username, hostname, port, password
                // username, hostname, port, password, connected
                // username, hostname, port, password, connected, friendly name
                // username, hostname, port, password, connected, friendly name, pool members
                // If the user cancels the restore dialog, we use the ServerAddressList instead:
                // hostname, port, friendly name

                string[] entryComps = entry.Split(SEPARATOR);

                int port;
                if (!int.TryParse(entryComps[2], out port))
                {
                    port = ConnectionsManager.DEFAULT_XEN_PORT;
                }

                if (entryComps[0] == VNC_INDICATOR)
                {
                    continue;
                }
                else if (entryComps.Length > 3)
                {
                    bool newItem = true;
                    if (!int.TryParse(entryComps[1], out port))
                    {
                        port = ConnectionsManager.DEFAULT_XEN_PORT;
                    }

                    IXenConnection connection = new XenConnection();
                    connection.Username = entryComps[0];
                    connection.Hostname = entryComps[1];
                    connection.Port = port;
                    connection.Password = entryComps[3];
                    connection.FriendlyName = entryComps[2];
                    
                    foreach (var con in Connections)
                    {
                        if (con.Hostname == connection.Hostname && connection.Username == con.Username)
                        {
                            newItem = false;
                            break;
                        }

                    }
                    if(newItem)
                        Connections.Add(connection);
                }
            }
            return Connections;
        }
    }
}
