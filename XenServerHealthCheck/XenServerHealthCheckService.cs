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
using System.ServiceProcess;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenServerHealthCheck
{
    public partial class XenServerHealthCheckService : ServiceBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public XenServerHealthCheckService()
        {
            InitializeComponent();
            AutoLog = false;
            if (!System.Diagnostics.EventLog.SourceExists("XenServerHealthCheck"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "XenServerHealthCheck", "XenServerHealthCheckLog");
            }
        }

        protected override void OnStart(string[] args)
        {
            // Set up a timer to trigger the uploading service.
            log.Info("XenServer Health Check Service starting...");
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 30 * 60000; // 30 minitues
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
            log.Info("XenServer Health Check Service stopping...");
        }

        

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            log.Info("XenServer Health Check Service start to refresh uploading tasks");
            List<IXenConnection> Connections = ServerListHelper.GetServerList();
            foreach (IXenConnection connection in Connections)
            {
                log.InfoFormat("Check server {0} with user {1}", connection.Hostname, connection.Username);
                Session _session = new Session(connection.Hostname, 80);
                _session.APIVersion = API_Version.LATEST;
                try
                {
                    _session.login_with_password(connection.Username, connection.Password);
                    
                    if (_session != null)
                    {
                        connection.LoadCache(_session);
                        Dictionary<string, string> gui_config = Helpers.GetGuiConfig(connection.Cache.Pools[0]);
                        _session.logout();
                    }
                }
                catch (Exception exn)
                {
                    log.Error(exn, exn);
                }
            }
        }
    }
}
