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
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions
{
    public class AssignVMsToPolicyAction : AsyncAction
    {

        private readonly VMSS _policy;
        private readonly List<XenRef<VM>> _selectedVMs;

        public AssignVMsToPolicyAction(VMSS policy, List<XenRef<VM>> selectedVMs, bool suppressHistory)
            : base(policy.Connection, Messages.ASSIGN_VMSS_POLICY_NOAMP, suppressHistory)
        {
            _policy = policy;
            _selectedVMs = selectedVMs;
            Pool = Helpers.GetPool(policy.Connection);
            ApiMethodsToRoleCheck.Add("VM.set_snapshot_schedule");
        }

        protected override void Run()
        {
            Description = Messages.ASSIGNING_VMSS_POLICY;

            var removedItems = _policy.VMs.Except(_selectedVMs);
            foreach (var xenRef in removedItems)
            {
                VM.set_snapshot_schedule(Session, xenRef, null);
            }

            foreach (var vmRef in _selectedVMs)
            {
                var vm = _policy.Connection.Resolve(vmRef);
                if (vm != null && (vm.snapshot_schedule == null || vm.snapshot_schedule.opaque_ref != _policy.opaque_ref))
                    VM.set_snapshot_schedule(Session, vm.opaque_ref, _policy.opaque_ref);
            }

            Description = Messages.ASSIGNED_VMSS_POLICY;
        }
    }

    public class RemoveVMsFromPolicyAction : AsyncAction
    {
        private readonly List<XenRef<VM>> _selectedVMs;
        private readonly VMSS _policy;

        public RemoveVMsFromPolicyAction(VMSS policy, List<XenRef<VM>> selectedVMs)
            : base(policy.Connection, 
            selectedVMs.Count == 1 ?
            string.Format(Messages.REMOVE_VM_FROM_VMSS, policy.Connection.Resolve(selectedVMs[0]), policy.Name()) :
            string.Format(Messages.REMOVE_VMS_FROM_VMSS, policy.Name()))
        {
            _policy = policy;
            _selectedVMs = selectedVMs;
            Pool = Helpers.GetPool(policy.Connection);
            ApiMethodsToRoleCheck.Add("VM.set_snapshot_schedule");
        }

        protected override void Run()
        {
            Description = Messages.REMOVING_VMS_FROM_VMSS;

            foreach (var vmRef in _selectedVMs)
                VM.set_snapshot_schedule(Session, vmRef, null);

            Description = Messages.REMOVED_VMS_FROM_VMSS;
        }
    }
}
