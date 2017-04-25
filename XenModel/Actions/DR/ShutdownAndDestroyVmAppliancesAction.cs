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
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.DR
{
    public class ShutdownAndDestroyVmAppliancesAction : AsyncAction
    {
        private List<VM_appliance> selectedToDelete;

        public ShutdownAndDestroyVmAppliancesAction(IXenConnection connection, List<VM_appliance> deleteVMAppliances)
            : base(connection, Messages.DELETE_VM_APPLIANCES)
        {
            selectedToDelete = deleteVMAppliances;

            Pool = Helpers.GetPool(connection);
            #region RBAC Dependencies
            if (selectedToDelete.Count > 0)
            {
                ApiMethodsToRoleCheck.Add("VM_appliance.hard_shutdown");
                ApiMethodsToRoleCheck.Add("VM_appliance.destroy");
                ApiMethodsToRoleCheck.Add("VM.destroy");
                ApiMethodsToRoleCheck.Add("VBD.unplug");
                ApiMethodsToRoleCheck.Add("VBD.destroy");
                ApiMethodsToRoleCheck.Add("VIF.unplug");
                ApiMethodsToRoleCheck.Add("VIF.destroy");
            }
            #endregion
        }

        protected override void Run()
        {
            int increment = 0;
            if (selectedToDelete.Count > 0)
                increment = 100 / (selectedToDelete.Count * 3);

            foreach (var vmAppliance in selectedToDelete)
            {
                // shutdown appliance
                if (vmAppliance.allowed_operations.Contains(vm_appliance_operation.hard_shutdown))
                {
                    Description = string.Format(Messages.VM_APPLIANCE_SHUTTING_DOWN, vmAppliance.Name);
                    VM_appliance.hard_shutdown(Session, vmAppliance.opaque_ref);
                    Description = string.Format(Messages.VM_APPLIANCE_SHUTTING_DOWN_COMPLETED, vmAppliance.Name);
                }

                PercentComplete += increment;

                Description = string.Format(Messages.DELETING_VM_APPLIANCE, vmAppliance.Name);
                // destroy VMs
                foreach (var vm in Connection.ResolveAll<VM>(vmAppliance.VMs))
                {
                    ShutdownAndDestroyVMsAction.DestroyVM(Session, vm);
                }

                PercentComplete += increment;

                // destroy appliance
                try
                {
                    VM_appliance.destroy(Session, vmAppliance.opaque_ref);
                }
                catch (Exception e)
                {
                    if (!e.Message.StartsWith("Object has been deleted"))
                        throw;
                }
                PercentComplete += increment;
            }
            Description = Messages.DELETED_VM_APPLIANCES;
            PercentComplete = 100;
        }
    }
}
