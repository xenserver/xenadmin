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
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class EnableHostAction:HostAbstractAction
    {
        private readonly bool _resumeVMs;

        public EnableHostAction(Host host, bool resumeVMs,Func<Pool, Host, long, long, bool> acceptNTolChangesOnEnable)
            : base(host.Connection, Messages.HOST_ENABLE, Messages.WAITING, null, acceptNTolChangesOnEnable)
        {
            if (host == null)
                throw new ArgumentNullException("host");
            _resumeVMs = resumeVMs;
            this.Host = host;
            AddCommonAPIMethodsToRoleCheck();
            ApiMethodsToRoleCheck.Add("pool.ha_compute_hypothetical_max_host_failures_to_tolerate");
            ApiMethodsToRoleCheck.Add("pool.set_ha_host_failures_to_tolerate");

            ApiMethodsToRoleCheck.Add("vm.assert_agile");
            ApiMethodsToRoleCheck.Add("vm.pool_migrate");
            ApiMethodsToRoleCheck.Add("vm.start_on");
            ApiMethodsToRoleCheck.Add("vm.resume_on");

            ApiMethodsToRoleCheck.Add("host.remove_from_other_config");
            ApiMethodsToRoleCheck.Add("host.enable");
        }

        protected override void Run()
        {
            this.Description = string.Format(Messages.HOSTACTION_EXITING_MAINTENANCE_MODE, Helpers.GetName(Host));
            

            Enable(0, _resumeVMs ? 10 : 100, true);

            if (_resumeVMs)
            {
                List<VM> migratedVMs = Host.GetMigratedEvacuatedVMs();
                List<VM> haltedVMs = Host.GetHaltedEvacuatedVMs();
                List<VM> suspendedVMs = Host.GetSuspendedEvacuatedVMs();
                Host.ClearEvacuatedVMs(Session);

                if (migratedVMs.Count + haltedVMs.Count + suspendedVMs.Count > 0)
                {
                    int start = 10;
                    int each = 90 / (migratedVMs.Count + haltedVMs.Count + suspendedVMs.Count);

                    foreach (VM vm in migratedVMs)
                    {
                        RelatedTask = XenAPI.VM.async_live_migrate(Session, vm.opaque_ref, Host.opaque_ref);
                        PollToCompletion(start, start + each);
                        start += each;
                    }

                    foreach (VM vm in haltedVMs)
                    {
                        RelatedTask = XenAPI.VM.async_start_on(Session, vm.opaque_ref, Host.opaque_ref, false, false);
                        PollToCompletion(start, start + each);
                        start += each;
                    }

                    foreach (VM vm in suspendedVMs)
                    {
                        RelatedTask = XenAPI.VM.async_resume_on(Session, vm.opaque_ref, Host.opaque_ref, false, false);
                        PollToCompletion(start, start + each);
                        start += each;
                    }
                }
                else
                {
                    Host.ClearEvacuatedVMs(Session);
                }
            }

            this.Description = string.Format(Messages.HOSTACTION_EXITED_MAINTENANCE_MODE, Helpers.GetName(Host));
        }
    }
}