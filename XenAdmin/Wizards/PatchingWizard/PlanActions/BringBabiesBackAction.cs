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
using System.Threading;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public class BringBabiesBackAction : PlanActionWithSession, IAvoidRestartHostsAware
    {
        private readonly XenRef<Host> _host;
        private readonly Host currentHost;
        private readonly List<XenRef<VM>> _vms;
        private readonly bool _enableOnly = false;
        public List<string> AvoidRestartHosts { private get; set; }

        public BringBabiesBackAction(List<XenRef<VM>> vms, Host host,bool enableOnly)
            : base(host.Connection, string.Format(Messages.UPDATES_WIZARD_EXITING_MAINTENANCE_MODE,host.Name))
        {
            base.TitlePlan = string.Format(Messages.EXIT_SERVER_FROM_MAINTENANCE_MODE,host.Name); 
            this._host = new XenRef<Host>(host);
            this._vms = vms;
            this._enableOnly = enableOnly;
            currentHost = host;
            visible = false;
        }

        protected override Host CurrentHost
        {
            get { return currentHost; }
        }

        protected override void RunWithSession(ref Session session)
        {
            // If there are no patches that require reboot, we skip the evacuate-reboot-bringbabiesback sequence
            if (Helpers.ElyOrGreater(currentHost) && AvoidRestartHosts != null && AvoidRestartHosts.Contains(currentHost.uuid))
            {
                log.Debug("Skipped scheduled restart (livepatching succeeded), BringBabiesBackAction is skipped.");

                return;
            }
            
            visible = true;

            Status = Messages.PLAN_ACTION_STATUS_RECONNECTING_STORAGE;
            PBD.CheckAndBestEffortPlugPBDsFor(Connection, _vms);

            //
            // CA-17428: Apply hotfixes to a pool of hosts through XenCenter fails.
            //
            // Host do reenable themselves anyway, so just wait 1 min for that,  
            // occasionally poking it.
            //

            int retries = 0;

            Status = Messages.PLAN_ACTION_STATUS_REENABLING_HOST;
            while (!Host.get_enabled(session, _host.opaque_ref))
            {
                retries++;

                Thread.Sleep(1000);

                try
                {
                    Host.enable(session, _host.opaque_ref);
                }
                catch (Exception e)
                {
                    if (retries > 60)
                        throw;

                    log.Debug(string.Format("Cannot enable host {0}. Retrying in 1 sec.", _host.opaque_ref), e);
                }
            }

            if(_enableOnly)
                return;

            int vmCount = _vms.Count;
            int vmNumber = 0;

            foreach (VM vm in Connection.ResolveAll(_vms))
            {
                int tries = 0;

                if(vm.power_state != vm_power_state.Running)
                    continue; // vm may have been shutdown or suspended.

                do
                {
                    tries++;

                    try
                    {
                        Status = string.Format(Messages.PLAN_ACTION_STATUS_MIGRATING_VM_X_OF_Y, vmNumber + 1, vmCount);
                        
                        log.DebugFormat("Migrating VM '{0}' back to Host '{1}'", Helpers.GetName(vm),
                                        Helpers.GetName(Connection.Resolve(_host)));
                        
                        PollTaskForResultAndDestroy(Connection, ref session,
                            VM.async_live_migrate(session, vm.opaque_ref, _host.opaque_ref),
                            (vmNumber * 100) / vmCount, ((vmNumber + 1) * 100) / vmCount);
                        
                        vmNumber++;
                    }
                    catch (Failure e)
                    {
                        // When trying to put the first vm back, we get all sorts 
                        // of errors ie storage not plugged yet etc.  Just ignore them for now

                        if (vmNumber > 0 || tries > 24)
                            throw;

                        log.Debug(string.Format("Error migrating VM '{0}' back to Host '{1}'",
                            Helpers.GetName(vm), Helpers.GetName(Connection.Resolve(_host))), e);

                        Thread.Sleep(5000);
                    }
                } while (vmNumber == 0);
            }

            Host hostModelObject = Connection.Resolve(_host);
            if (hostModelObject != null)
            {
                log.DebugFormat("Cleaning up evacuated VMs from Host '{0}'", hostModelObject.Name);
                hostModelObject.ClearEvacuatedVMs(session);
            }
        }
    }
}
