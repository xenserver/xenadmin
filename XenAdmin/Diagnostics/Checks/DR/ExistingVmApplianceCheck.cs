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
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.VmApplianceProblem;
using XenAPI;


namespace XenAdmin.Diagnostics.Checks.DR
{
    public class ExistingVmApplianceCheck: PoolCheck
    {
        private readonly VM_appliance applianceToRecover;
        private readonly List<VM> vmsToRecover;

        /// <summary>
        /// Check if an appliance exists in specified pool. 
        /// </summary>
        /// <param name="vmAppliance">the appliance</param>
        /// <param name="vms">vms in this appliance</param>
        /// <param name="pool">the pool on which to check if the specified appliance exists</param>
        public ExistingVmApplianceCheck(VM_appliance vmAppliance, List<VM> vms, Pool pool)
            : base (pool)
        {
            applianceToRecover = vmAppliance;
            vmsToRecover = vms;
        }

        protected override Problem RunCheck()
        {
            if (applianceToRecover != null)
            {
                // if its VMs have version >= the version in the metadata import, then the used must destroy the VMs first
                var vmsToDestroy = new List<VM>();
                foreach (VM vmToRecover in vmsToRecover)
                {
                    var vmToDestroy = NewerVm(vmToRecover);
                    if (vmToDestroy != null)
                        vmsToDestroy.Add(vmToDestroy);
                }
                if (vmsToDestroy.Count > 0)
                    return new ExistingVmApplianceProblem(this, applianceToRecover, vmsToDestroy);

                foreach (VM_appliance appliance in Pool.Connection.Cache.VM_appliances)
                {
                    if (appliance.uuid == applianceToRecover.uuid)
                    {
                        // if the existing appliance is currently running, then the user must force shutdown this appliance first
                        if (appliance.IsRunning)
                            return new RunningVmApplianceProblem(this, appliance, true);

                        // warn the user that the existing appliance will be replaced (but we still need to delete the appliance before recovery)
                        return new ExistingVmApplianceWarning(this, appliance);
                    }
                }
            }
            return null;
        }

        private VM NewerVm(VM vmToRecover)
        {
            foreach (VM existingVm in Pool.Connection.Cache.VMs)
            {
                if (existingVm.uuid == vmToRecover.uuid)
                {
                    // if the existing VM's version is >= the version in the metadata import, then the used must destroy the VM first 
                    if (existingVm.version >= vmToRecover.version)
                        return existingVm;
                    break;
                }
            }
            return null;
        }

        public override string Description
        {
            get
            {
                return string.Format(Messages.DR_WIZARD_APPLIANCE_CHECK_DESCRIPTION, applianceToRecover.Name); 
            }
        }
    }
}
