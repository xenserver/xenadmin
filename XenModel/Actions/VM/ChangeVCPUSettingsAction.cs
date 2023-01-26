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
using XenAPI;

namespace XenAdmin.Actions
{
    public class ChangeVCPUSettingsAction : AsyncAction
    {
        private readonly long m_VCPUs_max;
        private readonly long m_VCPUs_at_startup;

        public ChangeVCPUSettingsAction(VM vm, long VCPUs_max, long VCPUs_at_startup)
            : base(vm.Connection, "", true)
        {
            VM = vm;
            m_VCPUs_max = VCPUs_max;
            m_VCPUs_at_startup = VCPUs_at_startup;

            if (VM.power_state == vm_power_state.Running)
            {
                if (VM.VCPUs_at_startup <= m_VCPUs_at_startup)
                    ApiMethodsToRoleCheck.Add("VM.set_VCPUs_number_live");
            }
            else
            {
                ApiMethodsToRoleCheck.AddRange("VM.set_VCPUs_at_startup", "VM.set_VCPUs_max");
            }
        }

        protected override void Run()
        {
            // get the VM from the cache again, to check its vCPU fields before trying to change them
            VM = Connection.Resolve(new XenRef<VM>(VM.opaque_ref));
            if (VM == null) // VM has disappeared
                return;

            if (VM.power_state == vm_power_state.Running) // if the VM is running, we can only change the vCPUs number, not the max.
            {
                if (VM.VCPUs_at_startup > m_VCPUs_at_startup) // reducing VCPU_at_startup is not allowed for live VMs
                {
                    throw new Exception(string.Format(Messages.VM_VCPU_CANNOT_UNPLUG_LIVE, VM.VCPUs_at_startup));
                }
                VM.set_VCPUs_number_live(Session, VM.opaque_ref, m_VCPUs_at_startup);
                return;
            }

            if (VM.VCPUs_at_startup > m_VCPUs_at_startup) // reducing VCPUs_at_startup: we need to change this value first, and then the VCPUs_max
            {
                VM.set_VCPUs_at_startup(Session, VM.opaque_ref, m_VCPUs_at_startup);
                VM.set_VCPUs_max(Session, VM.opaque_ref, m_VCPUs_max);
            }
            else // increasing VCPUs_at_startup: we need to change the VCPUs_max first
            {
                VM.set_VCPUs_max(Session, VM.opaque_ref, m_VCPUs_max);
                VM.set_VCPUs_at_startup(Session, VM.opaque_ref, m_VCPUs_at_startup);
            }
        }
    }
}
