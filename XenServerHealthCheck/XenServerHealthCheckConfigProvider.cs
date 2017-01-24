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
using System.IO;
using System.Net;
using XenAdmin;
using XenAdmin.Actions;
using XenAdmin.Network;
using XenAdmin.ServerDBs;
using XenAPI;

namespace XenServerHealthCheck
{
    public class XenServerHealthCheckConfigProvider : IXenAdminConfigProvider
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Func<List<Role>, IXenConnection, string, AsyncAction.SudoElevationResult> SudoDialogDelegate { get { return FakeSudoDialog; } }

        public int ConnectionTimeout
        {
            get { return Properties.Settings.Default.ConnectionTimeout; }
        }

        public Session CreateActionSession(Session session, IXenConnection connection)
        {
            return SessionFactory.CreateSession(session, connection, ConnectionTimeout);
        }

        public bool Exiting
        {
            get { return false; }
        }

        public bool ForcedExiting
        {
            get { return false; }
        }

        public string XenCenterUUID
        {
            get { return ""; }
        }

        public bool DontSudo
        {
            get { return false; }
        }

        public bool ShowHiddenVMs
        {
            get { return false; }
        }

        public int GetProxyTimeout(bool timeout)
        {
            return timeout ? Properties.Settings.Default.HttpTimeout : 0;
        }

        public IWebProxy GetProxyFromSettings(IXenConnection connection)
        {
            try
            {
                if (connection != null && connection.Session != null && connection.Session.uuid == "dummy")
                    return new XenServerHealthCheckSimulatorWebProxy(DbProxy.proxys[connection]);

                switch ((HTTPHelper.ProxyStyle)Properties.Settings.Default.ProxySetting)
                {
                    case HTTPHelper.ProxyStyle.SpecifiedProxy:
                        return new WebProxy(string.Format("http://{0}:{1}",
                            Properties.Settings.Default.ProxyAddress,
                            Properties.Settings.Default.ProxyPort),
                            Properties.Settings.Default.BypassProxyForLocal);

                    case HTTPHelper.ProxyStyle.SystemProxy:
                        return WebRequest.GetSystemWebProxy();

                    default:
                        return null;
                }
            }
            catch (ConfigurationErrorsException e)
            {
                log.Error("Error parsing 'ProxySetting' from settings - settings file deemed corrupt", e);
                Environment.Exit(1);
            }
            return null;
        }

        public void ShowObject(string newVMRef)
        {
        }

        public void HideObject(string newVMRef)
        {
        }

        public bool ObjectIsHidden(string opaqueRef)
        {
            return false;
        }

        public string GetLogFile()
        {
            return "";
        }

        public void UpdateServerHistory(string hostnameWithPort)
        {
        }

        public void SaveSettingsIfRequired()
        {
        }

        private static AsyncAction.SudoElevationResult FakeSudoDialog(List<Role> roles, IXenConnection connection, string actionTitle)
        {
            return new AsyncAction.SudoElevationResult(true, string.Empty, string.Empty, null);
        }


    }

    internal class XenServerHealthCheckSimulatorWebProxy : SimulatorWebProxy
    {
        public XenServerHealthCheckSimulatorWebProxy(DbProxy proxy) : base(proxy)
        {
        }

        public override Stream GetStream(Uri uri)
        {
            return new SimulatorWebStream(uri, this);
        }
    }

}
