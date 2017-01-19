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
using XenAPI;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public class EvacuateHostPlanAction : PlanActionWithSession, IAvoidRestartHostsAware
    {
        private readonly XenRef<Host> _host;
        private readonly Host currentHost;
        public List<string> AvoidRestartHosts { private get; set; }
        
        public EvacuateHostPlanAction(Host host)
            : base(host.Connection, string.Format(Messages.PLANACTION_VMS_MIGRATING, host.Name))
        {
            base.TitlePlan = string.Format(Messages.MIGRATE_VMS_OFF_SERVER, host.Name);
            this._host = new XenRef<Host>(host);
            currentHost = host;
            visible = false;
        }

        protected override Host CurrentHost
        {
            get { return currentHost; }
        }

        protected override void RunWithSession(ref Session session)
        {
            Host hostObject = TryResolveWithTimeout(_host);

            // If there are no patches that require reboot, we skip the evacuate-reboot-bringbabiesback sequence
            // But we only do this if we indicated that host restart should be avoided (by initializing the AvoidRestartHosts property)
            if (Helpers.ElyOrGreater(hostObject) && AvoidRestartHosts != null)
            {
                log.DebugFormat("Checking host.patches_requiring_reboot now on '{0}'...", hostObject);

                if (hostObject.updates_requiring_reboot.Count > 0)
                {
                    AvoidRestartHosts.Remove(hostObject.uuid);

                    log.DebugFormat("Restart is needed now (hostObject.updates_requiring_reboot has {0} items in it). Evacuating now. Will restart after.", hostObject.updates_requiring_reboot.Count);
                }
                else
                {
                    if (!AvoidRestartHosts.Contains(hostObject.uuid))
                        AvoidRestartHosts.Add(hostObject.uuid);

                    log.Debug("Will skip scheduled restart (livepatching succeeded), because hostObject.patches_requiring_reboot is empty.");

                    return;
                }
            }

            visible = true;

            PBD.CheckAndPlugPBDsFor(Connection.ResolveAll(hostObject.resident_VMs));

            log.DebugFormat("Disabling host {0}", hostObject.Name);
            Host.disable(session, _host.opaque_ref);

            Status = Messages.PLAN_ACTION_STATUS_MIGRATING_VMS_FROM_HOST;
            log.DebugFormat("Migrating VMs from host {0}", hostObject.Name);
            XenRef<Task> task = Host.async_evacuate(session, _host.opaque_ref);

            PollTaskForResultAndDestroy(Connection, ref session, task);
        }
    }
}
