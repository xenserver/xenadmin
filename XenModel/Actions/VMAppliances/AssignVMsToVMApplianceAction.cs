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


namespace XenAdmin.Actions
{
    public class AssignVMsToVMApplianceAction : PureAsyncAction
    {
        private VM_appliance _vmAppliance;
        private List<XenRef<VM>> _selectedVMs;

        public AssignVMsToVMApplianceAction(VM_appliance vmAppliance, List<XenRef<VM>> selectedVMs, bool suppressHistory)
            : base(vmAppliance.Connection, selectedVMs.Count == 1 ?
            string.Format(Messages.ASSIGN_VM_TO_VAPP, vmAppliance.Connection.Resolve(selectedVMs[0]), vmAppliance.Name)
            : string.Format(Messages.ASSIGN_VMS_TO_VAPP, vmAppliance.Name), suppressHistory)
        {
            _vmAppliance = vmAppliance;
            _selectedVMs = selectedVMs;
            Pool = Helpers.GetPool(vmAppliance.Connection);

        }

        protected override void Run()
        {
            Description = Messages.ASSIGNING_VM_APPLIANCE;
            foreach (var xenRef in _vmAppliance.VMs)
            {
                VM.set_appliance(Session, xenRef, null);
            }
            foreach (var xenRef in _selectedVMs)
            {
                VM.set_appliance(Session, xenRef, _vmAppliance.opaque_ref);
            }
            Description = Messages.ASSIGNED_VM_APPLIANCE;
        }
    }

    public class RemoveVMsFromVMApplianceAction : PureAsyncAction
    {
        private List<XenRef<VM>> _selectedVMs;

        public RemoveVMsFromVMApplianceAction(VM_appliance vmAppliance, List<XenRef<VM>> selectedVMs)
            : base(vmAppliance.Connection, selectedVMs.Count == 1 ?
            string.Format(Messages.REMOVE_VM_FROM_APPLIANCE, vmAppliance.Connection.Resolve(selectedVMs[0]), vmAppliance.Name)
            : string.Format(Messages.REMOVE_VMS_FROM_APPLIANCE, vmAppliance.Name))
        {
            _selectedVMs = selectedVMs;
            Pool = Helpers.GetPool(vmAppliance.Connection);

        }

        protected override void Run()
        {
            Description = Messages.REMOVING_VMS_FROM_APPLIANCE;
            foreach (var xenRef in _selectedVMs)
            {
                VM.set_appliance(Session, xenRef, null);
            }
            Description = Messages.REMOVED_VMS_FROM_APPLIANCE;
        }
    }

}