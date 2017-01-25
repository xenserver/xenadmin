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

using XenAPI;
using System;
using System.Timers;
using Timer = System.Timers.Timer;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public class UpgradeManualHostPlanAction : RebootPlanAction
    {
        private readonly Host _host;

        public Timer timer = new Timer();

        public UpgradeManualHostPlanAction(Host host)
            : base(host.Connection, new XenRef<Host>(host.opaque_ref), string.Format(Messages.UPGRADING_SERVER, host))
        {
            TitlePlan = Messages.UPGRADING;
            _host = host;
            timer.Interval = 20 * 60000;
            timer.AutoReset = true;
            timer.Elapsed += timer_Elapsed;
        }

        protected override Host CurrentHost
        {
            get { return _host; }
        }

        public new Host Host { get { return TryResolveWithTimeout(base.Host); } }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (rebooting && Timeout != null)
            {
                Timeout(this, null);
            }
        }

        public event EventHandler Timeout;

        private bool rebooting = false;


        protected override void RunWithSession(ref Session session)
        {
            try
            {
                Status = Messages.PLAN_ACTION_STATUS_DISABLING_HOST_SERVER;

                if (Host.enabled)
                {
                    log.DebugFormat("Disabling host {0}", _host.Name);
                    XenAPI.Host.disable(session, Host.opaque_ref);
                }

                timer.Start();
                rebooting = true;

                log.DebugFormat("Upgrading host {0}", _host.Name);
                Status = Messages.PLAN_ACTION_STATUS_INSTALLING_XENSERVER;

                log.DebugFormat("Waiting for host {0} to reboot", _host.Name);
                WaitForReboot(ref session, _session => XenAPI.Host.async_reboot(_session, Host.opaque_ref));

                Status = Messages.PLAN_ACTION_STATUS_RECONNECTING_STORAGE;
                foreach (var host in _host.Connection.Cache.Hosts)
                    host.CheckAndPlugPBDs();  // Wait for PBDs to become plugged on all hosts
                
                rebooting = false;
                log.DebugFormat("Host {0} rebooted", _host.Name);
                
                Status = Messages.PLAN_ACTION_STATUS_HOST_UPGRADED;
                log.DebugFormat("Upgraded host {0}", _host.Name);
            }
            finally
            {
                _host.Connection.ExpectDisruption = false;
                timer.Stop();
            }
        }

        internal void RunExternal(Session session)
        {
            RunWithSession(ref session);
        }
    }
}
