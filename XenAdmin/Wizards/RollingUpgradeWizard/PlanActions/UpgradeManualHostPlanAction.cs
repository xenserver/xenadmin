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
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using Timer = System.Timers.Timer;


namespace XenAdmin.Wizards.RollingUpgradeWizard.PlanActions
{
    public class UpgradeManualHostPlanAction : RebootPlanAction
    {
        public Timer timer = new Timer();

        public UpgradeManualHostPlanAction(Host host)
            : base(host, string.Format(Messages.UPGRADING_SERVER, host))
        {
            TitlePlan = Messages.UPGRADING;
            timer.Interval = 20 * 60000;
            timer.AutoReset = true;
            timer.Elapsed += timer_Elapsed;
        }

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
                var hostObj = GetResolvedHost();

                if (hostObj.enabled)
                {
                    Status = Messages.PLAN_ACTION_STATUS_DISABLING_HOST_SERVER;
                    log.DebugFormat("Disabling host {0}", hostObj.Name());
                    Host.disable(session, HostXenRef.opaque_ref);
                }

                timer.Start();
                rebooting = true;

                log.DebugFormat("Upgrading host {0}", hostObj.Name());
                Status = Messages.PLAN_ACTION_STATUS_INSTALLING_XENSERVER;

                log.DebugFormat("Waiting for host {0} to reboot", hostObj.Name());
                WaitForReboot(ref session, Host.BootTime, s => Host.async_reboot(s, HostXenRef.opaque_ref));

                Status = Messages.PLAN_ACTION_STATUS_RECONNECTING_STORAGE;
                foreach (var host in Connection.Cache.Hosts)
                    host.CheckAndPlugPBDs();  // Wait for PBDs to become plugged on all hosts
                
                rebooting = false;
                log.DebugFormat("Host {0} rebooted", hostObj.Name());
                
                Status = Messages.PLAN_ACTION_STATUS_HOST_UPGRADED;
                log.DebugFormat("Upgraded host {0}", hostObj.Name());
            }
            finally
            {
                Connection.ExpectDisruption = false;
                timer.Stop();
            }
        }
    }
}
