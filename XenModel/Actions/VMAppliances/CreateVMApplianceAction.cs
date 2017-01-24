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
    public class CreateVMApplianceAction: AsyncAction
    {
        private VM_appliance _record;
        private List<VM> _vms;
        public CreateVMApplianceAction(VM_appliance record, List<VM> vms)
            : base(record.Connection, Messages.CREATE_VM_APPLIANCE)
        {
            _record = record;
            _vms = vms;
            Pool = Helpers.GetPool(record.Connection);
            ApiMethodsToRoleCheck.Add("VM_appliance.async_create");
            ApiMethodsToRoleCheck.Add("VM.set_appliance");
        }

        protected override void Run()
        {
            Description = string.Format(Messages.CREATING_VM_APPLIANCE, _record.Name);
            RelatedTask = VM_appliance.async_create(Session, _record);
            PollToCompletion();
            var vmApplianceRef = new XenRef<VM_appliance>(Result);
            Connection.WaitForCache(vmApplianceRef);
            foreach (var selectedVM in _vms)
            {
                VM.set_appliance(Session, selectedVM.opaque_ref, vmApplianceRef.opaque_ref);
            }
            Description = string.Format(Messages.CREATED_VM_APPLIANCE, _record.Name);
            PercentComplete = 100;
        }
    }
}
