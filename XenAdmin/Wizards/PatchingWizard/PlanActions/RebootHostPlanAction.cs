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

using System.Collections.Generic;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public class RebootHostPlanAction : RebootPlanAction, IAvoidRestartHostsAware
    {

        private readonly Host _host;
        public List<string> AvoidRestartHosts { private get; set; }
        
        public RebootHostPlanAction(Host host)
            : base(host.Connection, new XenRef<Host>(host.opaque_ref), string.Format(Messages.UPDATES_WIZARD_REBOOTING, host))
        {
            _host = host;
            visible = false;
        }

        protected override void RunWithSession(ref Session session)
        {
            // If there are no patches that require reboot, we skip the evacuate-reboot-bringbabiesback sequence
            if (Helpers.ElyOrGreater(_host) && AvoidRestartHosts != null && AvoidRestartHosts.Contains(_host.uuid))
            {
                log.Debug("Skipping scheduled restart (livepatching succeeded). RebootHostPlanAction is skipped.");

                return;
            }

            visible = true;

            _host.Connection.ExpectDisruption = true;
            try
            {
                base.WaitForReboot(ref session, _session => XenAPI.Host.async_reboot(_session, Host.opaque_ref));
                foreach (var host in _host.Connection.Cache.Hosts)
                    host.CheckAndPlugPBDs();  // Wait for PBDs to become plugged on all hosts
            }
            finally
            {
                _host.Connection.ExpectDisruption = false;
            }
        }

        internal void RunExternal(Session session)
        {
            RunWithSession(ref session);
        }
    }
}