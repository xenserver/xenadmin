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

using System.Collections.Generic;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    public class AssignVMsToPolicyAction:PureAsyncAction
    {

        private VMPP _vmpp;
        private List<XenRef<VM>> _selectedVMs;

        public AssignVMsToPolicyAction(VMPP vmpp, List<XenRef<VM>> selectedVMs)
            : base(vmpp.Connection, Messages.ASSIGN_PROTECTION_POLICY_NOAMP)
        {
            _vmpp = vmpp;
            _selectedVMs = selectedVMs;
            Pool = Helpers.GetPool(vmpp.Connection);

        }

        protected override void Run()
        {
            Description = Messages.ASSIGNING_PROTECTION_POLICY;
            foreach (var xenRef in _vmpp.VMs)
            {
                VM.set_protection_policy(Session, xenRef, null);
            }
            foreach (var xenRef in _selectedVMs)
            {
                VM.set_protection_policy(Session, xenRef, _vmpp.opaque_ref);
            }
            Description = Messages.ASSIGNED_PROTECTION_POLICY;
        }
    }

    public class RemoveVMsFromPolicyAction : PureAsyncAction
    {
        private List<XenRef<VM>> _selectedVMs;

        public RemoveVMsFromPolicyAction(VMPP vmpp, List<XenRef<VM>> selectedVMs)
            : base(vmpp.Connection, selectedVMs.Count == 1 ?
            string.Format(Messages.REMOVE_VM_FROM_POLICY, vmpp.Connection.Resolve(selectedVMs[0]), vmpp.Name)
            : string.Format(Messages.REMOVE_VMS_FROM_POLICY, vmpp.Name))
        {
            _selectedVMs = selectedVMs;
            Pool = Helpers.GetPool(vmpp.Connection);
        }

        protected override void Run()
        {
            Description = Messages.REMOVING_VMS_FROM_POLICY;

            foreach (var xenRef in _selectedVMs)
                VM.set_protection_policy(Session, xenRef, null);

            Description = Messages.REMOVED_VMS_FROM_POLICY;
        }
    }
}
