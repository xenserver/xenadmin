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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Plugins;
using XenAdmin.ServerDBs;
using XenAPI;
using XenCenterLib;

namespace XenAdmin
{
    public class WinformsXenAdminConfigProvider : IXenAdminConfigProvider
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly List<string> HiddenObjects = new List<string>();
        private static readonly object HiddenObjectsLock = new object();

        public Func<List<Role>, IXenConnection, string, AsyncAction.SudoElevationResult> ElevatedSessionDelegate => GetElevatedSession;

        public int ConnectionTimeout => Properties.Settings.Default.ConnectionTimeout;

        public Session CreateActionSession(Session session, IXenConnection connection)
        {
            return SessionFactory.DuplicateSession(session, connection, ConnectionTimeout);
        }

        public bool Exiting => Program.Exiting;

        public bool ForcedExiting => Program.ForcedExiting;

        public string XenCenterUUID => Program.XenCenterUUID;

        public bool DontSudo => Registry.DontSudo;

        public IWebProxy GetProxyFromSettings(IXenConnection connection)
        {
            return GetProxyFromSettings(connection, true);
        }

        public IWebProxy GetProxyFromSettings(IXenConnection connection, bool isForXenServer)
        {
            try
            {
                if (connection != null && connection.Session != null && connection.Session.opaque_ref == "dummy")
                    return new XenAdminSimulatorWebProxy(DbProxy.proxys[connection]);

                switch ((HTTPHelper.ProxyStyle)XenAdmin.Properties.Settings.Default.ProxySetting)
                {
                    case HTTPHelper.ProxyStyle.SpecifiedProxy:
                        if (isForXenServer && Properties.Settings.Default.BypassProxyForServers)
                            return null;

                        string address = string.Format("http://{0}:{1}",
                            Properties.Settings.Default.ProxyAddress,
                            Properties.Settings.Default.ProxyPort);

                        if (Properties.Settings.Default.ProvideProxyAuthentication)
                        {
                            // checks for empty default username/password which starts out unencrypted

                            string username = "";
                            try
                            {
                                string protectedUsername = Properties.Settings.Default.ProxyUsername;
                                if (!string.IsNullOrEmpty(protectedUsername))
                                    username = EncryptionUtils.Unprotect(protectedUsername);
                            }
                            catch (Exception e)
                            {
                                Log.Warn("Could not unprotect internet proxy username.", e);
                            }

                            string password = "";
                            try
                            {
                                string protectedPassword = Properties.Settings.Default.ProxyPassword;
                                if (!string.IsNullOrEmpty(protectedPassword))
                                    password = EncryptionUtils.Unprotect(protectedPassword);
                            }
                            catch (Exception e)
                            {
                                Log.Warn("Could not unprotect internet proxy password.", e);
                            }

                            return new WebProxy(address, false, null, new NetworkCredential(username, password));
                        }
                        
                        return new WebProxy(address, false);

                    case HTTPHelper.ProxyStyle.SystemProxy:
                        return WebRequest.GetSystemWebProxy();

                    default:
                        return null;
                }
            }
            catch (ConfigurationErrorsException e)
            {
                Log.Error("Error parsing 'ProxySetting' from settings - settings file deemed corrupt", e);
                using (var dlg = new ErrorDialog(string.Format(Messages.MESSAGEBOX_LOAD_CORRUPTED, Settings.GetUserConfigPath()))
                    {WindowTitle = Messages.MESSAGEBOX_LOAD_CORRUPTED_TITLE})
                {
                    dlg.ShowDialog();
                }

                Environment.Exit(1);
            }
            return null;
        }

        public int GetProxyTimeout(bool timeout)
        {
            return timeout ? Properties.Settings.Default.HttpTimeout : 0;
        }

        public void ShowObject(string opaqueRef)
        {
            lock (HiddenObjectsLock)
                HiddenObjects.Remove(opaqueRef);

            Program.MainWindow?.RequestRefreshTreeView();
        }

        public void HideObject(string opaqueRef)
        {
            lock (HiddenObjectsLock)
                HiddenObjects.Add(opaqueRef);

            Program.MainWindow?.RequestRefreshTreeView();
        }

        public bool ObjectIsHidden(string opaqueRef)
        {
            lock (HiddenObjectsLock)
                return HiddenObjects.Contains(opaqueRef);
        }

        public string GetLogFile()
        {
            return Program.GetLogFile();
        }

        public void UpdateServerHistory(string hostnameWithPort)
        {
            Settings.UpdateServerHistory(hostnameWithPort);
        }

        public void SaveSettingsIfRequired()
        {
            Settings.SaveServerList();
        }

        private AsyncAction.SudoElevationResult GetElevatedSession(List<Role> allowedRoles,
            IXenConnection connection, string actionTitle)
        {
            AsyncAction.SudoElevationResult result = null;

            Program.Invoke(Program.MainWindow, () =>
            {
                Form owner;
                try
                {
                    //CA-337323: make an attempt to find the right owning form
                    //most likely it will be the last one opened
                    owner = Application.OpenForms.Cast<Form>().Last();
                }
                catch
                {
                    owner = Program.MainWindow;
                }

                using (var d = new RoleElevationDialog(connection, connection.Session, allowedRoles, actionTitle))
                    if (d.ShowDialog(owner) == DialogResult.OK)
                        result = new AsyncAction.SudoElevationResult(d.elevatedUsername, d.elevatedPassword, d.elevatedSession);
            });

            return result;
        }

        public bool ShowHiddenVMs => Properties.Settings.Default.ShowHiddenVMs;

        public PluginManager PluginManager;

        public string GetXenCenterMetadata(bool isForXenCenter)
        {
            return Metadata.Generate(PluginManager, isForXenCenter);
        }

        public string GetCustomUpdatesXmlLocation()
        {
            return Registry.GetCustomUpdatesXmlLocation();
        }

        public string GetInternalStageAuthToken()
        {
            return Registry.GetInternalStageAuthToken();
        }

        public string GetInternalStageAuthTokenName()
        {
            return Registry.AuthTokenName;
        }
    }
}
