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
using System.Linq;
using XenAdmin.Core;

namespace XenAPI
{
    public partial class VM_appliance
    {
        public override string Name
        {
            get { return name_label; }
        }

        public override string Description
        {
            get { return name_description; }
        }

        public List<VM> GetFateSharingVMs()
        {
            //find VMs that do not belong in this appliance
            var vmsNotInCurApp = from VM vm in this.Connection.Cache.VMs
                                 where vm.appliance != this.opaque_ref
                                 select vm;

            var fateSharingVms = new List<VM>();

            foreach (var vmRef in this.VMs)
            {
                VM thisVm = this.Connection.Resolve(vmRef);
                if (thisVm == null)
                    continue;

                foreach (var otherVm in vmsNotInCurApp)
                {
                    if (otherVm.is_a_real_vm && otherVm.power_state != vm_power_state.Halted && otherVm.SRs.Intersect(thisVm.SRs).FirstOrDefault() != null && !fateSharingVms.Contains(otherVm))
                        fateSharingVms.Add(otherVm);
                }
            }

            return fateSharingVms;
        }

        public bool IsRunning
        {
            get
            {
                foreach (var vmRef in VMs)
                {
                    VM vm = Connection.Resolve(vmRef);
                    if (vm == null)
                        continue;

                    if (vm.power_state == vm_power_state.Running
                        || vm.power_state == vm_power_state.Paused
                        || vm.power_state == vm_power_state.Suspended)
                        return true;
                }
                return false;
            }
        }

        public static List<XenRef<SR>> GetDRMissingSRs(Session session, string vm, Session sessionTo)
        {
            return Helpers.CreedenceOrGreater(sessionTo.Connection)
                       ? VM_appliance.get_SRs_required_for_recovery(session, vm, sessionTo.uuid)
                       : null;
        }
    }
}
