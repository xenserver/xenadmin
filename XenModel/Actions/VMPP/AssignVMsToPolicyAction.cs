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
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    public class AssignVMsToPolicyAction<T>:PureAsyncAction where T : XenObject<T>
    {

        private IVMPolicy _policy;
        private List<XenRef<VM>> _selectedVMs;

        public AssignVMsToPolicyAction(IVMPolicy policy, List<XenRef<VM>> selectedVMs, bool suppressHistory)
            : base(policy.Connection, (typeof(T) == typeof(VMPP) ? Messages.ASSIGN_PROTECTION_POLICY_NOAMP : Messages.ASSIGN_VMSS_POLICY_NOAMP), suppressHistory)
        {
            _policy = policy;
            _selectedVMs = selectedVMs;
            Pool = Helpers.GetPool(policy.Connection);

        }

        protected override void Run()
        {
            Description = typeof(T) == typeof(VMPP) ? Messages.ASSIGNING_PROTECTION_POLICY : Messages.ASSIGNING_VMSS_POLICY;

            foreach (var xenRef in _policy.VMs)
            {
                _policy.set_vm_policy(Session, xenRef, null);
            }

            foreach (var xenRef in _selectedVMs)
            {
                _policy.set_vm_policy(Session, xenRef, _policy.opaque_ref);
            }

            Description = (typeof(T) == typeof(VMPP)) ? Messages.ASSIGNED_PROTECTION_POLICY : Messages.ASSIGNED_VMSS_POLICY;
        }
    }

    public class RemoveVMsFromPolicyAction<T> : PureAsyncAction where T : XenObject<T>
    {
        private List<XenRef<VM>> _selectedVMs;
        private IVMPolicy _policy;

        public RemoveVMsFromPolicyAction(IVMPolicy policy, List<XenRef<VM>> selectedVMs)
            : base(policy.Connection, 
            selectedVMs.Count == 1 ?
            string.Format(typeof(T) == typeof(VMPP) ? Messages.REMOVE_VM_FROM_POLICY : Messages.REMOVE_VM_FROM_VMSS, policy.Connection.Resolve(selectedVMs[0]), policy.Name) :
            string.Format(typeof(T) == typeof(VMPP) ? Messages.REMOVE_VMS_FROM_POLICY : Messages.REMOVE_VMS_FROM_VMSS, policy.Name))
        {
            _policy = policy;
            _selectedVMs = selectedVMs;
            Pool = Helpers.GetPool(policy.Connection);
        }

        protected override void Run()
        {
            Description = typeof(T) == typeof(VMPP) ? Messages.REMOVING_VMS_FROM_POLICY : Messages.REMOVING_VMS_FROM_VMSS;

            foreach (var xenRef in _selectedVMs)
                _policy.set_vm_policy(Session, xenRef, null);
                

            Description = typeof(T) == typeof(VMPP) ? Messages.REMOVED_VMS_FROM_POLICY : Messages.REMOVED_VMS_FROM_VMSS;
        }
    }
}
