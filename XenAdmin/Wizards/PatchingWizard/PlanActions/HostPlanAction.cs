﻿/* Copyright (c) Citrix Systems, Inc. 
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
using System.Threading;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public abstract class HostPlanAction : PlanActionWithSession
    {
        private readonly Host _currentHost;
        protected readonly XenRef<Host> HostXenRef;

        protected HostPlanAction(Host host)
            : base(host.Connection)
        {
            _currentHost = host;
            HostXenRef = new XenRef<Host>(host);
        }

        protected internal Host CurrentHost
        {
            get { return _currentHost; }
        }

        public Host GetResolvedHost()
        {
           return Connection.TryResolveWithTimeout(HostXenRef);
        }

        protected void EvacuateHost(ref Session session)
        {
            var hostObj = GetResolvedHost();

            var vms = hostObj.GetRunningVMs();

            if (hostObj.enabled)
            {
                AddProgressStep(string.Format(Messages.UPDATES_WIZARD_ENTERING_MAINTENANCE_MODE, hostObj.Name()));
                log.DebugFormat("Disabling host {0}", hostObj.Name());
                Host.disable(session, HostXenRef.opaque_ref);
            }

            if (vms.Count > 0)
            {
                PBD.CheckPlugPBDsForVMs(Connection, vms);

                try
                {
                    AddProgressStep(string.Format(Messages.PLANACTION_VMS_MIGRATING, hostObj.Name()));
                    log.DebugFormat("Migrating VMs from host {0}", hostObj.Name());

                    XenRef<Task> task = Host.async_evacuate(session, HostXenRef.opaque_ref);
                    PollTaskForResultAndDestroy(Connection, ref session, task);
                }
                catch (Failure f)
                {
                    if (f.ErrorDescription.Count > 0 && f.ErrorDescription[0] == Failure.HOST_NOT_ENOUGH_FREE_MEMORY)
                    {
                        log.WarnFormat("Host {0} cannot be evacuated: {1}", hostObj.Name(), f.Message);
                        throw new Exception(string.Format(Messages.PLAN_ACTION_FAILURE_NOT_ENOUGH_MEMORY, hostObj.Name()), f);
                    }

                    if (f.ErrorDescription.Count > 0 && f.ErrorDescription[0] == Failure.NO_HOSTS_AVAILABLE)
                    {
                        log.WarnFormat("Host {0} cannot be evacuated: {1}", hostObj.Name(), f.Message);
                        if (hostObj.Connection.Cache.Hosts.Any(h=>h.updates_requiring_reboot.Count > 0 && !h.enabled))
                            throw new Exception(string.Format(Messages.PLAN_ACTION_FAILURE_NO_HOSTS_AVAILABLE, hostObj.Name()));
                    }
                    throw;
                }
            }
        }

        protected void BringBabiesBack(ref Session session, List<XenRef<VM>> vmRefs, bool enableOnly)
        {
            // CA-17428: Apply hotfixes to a pool of hosts through XenCenter fails.
            // Hosts do reenable themselves anyway, so just wait 1 min for that,  
            // occasionally poking it.

            WaitForHostToBecomeEnabled(session, true);
            
            if (enableOnly)
                return;

            //CA-334860: Attempt repatriating only VMs that have actually been evacuated
            //and are currently residing on a different host

            var nonResidingRefs = (from XenRef<VM> vmRef in vmRefs
                let vm = Connection.TryResolveWithTimeout(vmRef)
                where vm != null && vm.resident_on != HostXenRef.opaque_ref
                select vmRef).ToList();

            if (nonResidingRefs.Count == 0)
                return;

            var hostObj = GetResolvedHost();
            AddProgressStep(string.Format(Messages.PLAN_ACTION_STATUS_REPATRIATING_VMS, hostObj.Name()));
            PBD.CheckPlugPBDsForVMs(Connection, nonResidingRefs, true);

            int vmCount = nonResidingRefs.Count;
            int vmNumber = 0;

            foreach (var vmRef in nonResidingRefs)
            {
                var vm = Connection.Resolve(vmRef);
                if (vm == null)
                    continue;

                int tries = 0;

                if (vm.power_state != vm_power_state.Running)
                    continue; // vm may have been shutdown or suspended.

                if (vm.resident_on == HostXenRef.opaque_ref)
                    continue; // vm may have been migrated back manually in the meantime

                do
                {
                    tries++;

                    try
                    {
                        log.DebugFormat("Migrating VM '{0}' back to Host '{1}'", vm.Name(), hostObj.Name());

                        PollTaskForResultAndDestroy(Connection, ref session,
                            VM.async_live_migrate(session, vm.opaque_ref, HostXenRef.opaque_ref),
                            (vmNumber * 100) / vmCount, ((vmNumber + 1) * 100) / vmCount);

                        vmNumber++;
                    }
                    catch (Failure e)
                    {
                        // When trying to put the first vm back, we get all sorts 
                        // of errors ie storage not plugged yet etc.  Just ignore them for now

                        if (vmNumber > 0 || tries > 24)
                            throw;

                        log.Debug(string.Format("Error migrating VM '{0}' back to Host '{1}'", vm.Name(), hostObj.Name()), e);

                        Thread.Sleep(5000);
                    }
                } while (vmNumber == 0);
            }

            log.DebugFormat("Cleaning up evacuated VMs from Host '{0}'", hostObj.Name());
            Host.ClearEvacuatedVMs(session, HostXenRef);
        }

        protected void WaitForHostToBecomeEnabled(Session session, bool attemptEnable)
        {
            int retries = 0;
            while (!Host.get_enabled(session, HostXenRef.opaque_ref))
            {
                var hostObj = GetResolvedHost();
                if (retries == 0)
                    AddProgressStep(string.Format(Messages.UPDATES_WIZARD_EXITING_MAINTENANCE_MODE, hostObj.Name()));

                retries++;
                var isLastTry = retries > 60;

                Thread.Sleep(5000);

                if (!attemptEnable)
                {
                    if (isLastTry)
                    {
                        log.DebugFormat("Timed out waiting for host {0} to become enabled.", hostObj.Name());
                        break;
                    }

                    continue;
                }

                try
                {
                    Host.enable(session, HostXenRef.opaque_ref);
                }
                catch (Exception e)
                {
                    if (isLastTry)
                        throw;

                    log.Debug(string.Format("Cannot enable host {0}. Retrying in 5 sec.", hostObj), e);
                }
            }
        }
    }
}
