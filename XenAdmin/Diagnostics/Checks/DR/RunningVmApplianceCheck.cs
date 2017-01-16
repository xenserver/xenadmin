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
using System.Text;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.VmApplianceProblem;
using XenAPI;


namespace XenAdmin.Diagnostics.Checks.DR
{
    public class RunningVmApplianceCheck: PoolCheck
    {
        private readonly VM_appliance vmAppliance;

        /// <summary>
        /// Check if an appliance is running in specified pool. 
        /// </summary>
        /// <param name="vmAppliance">the appliance</param>
        /// <param name="pool">the pool on which to check if the specified appliance is running</param>
        public RunningVmApplianceCheck(VM_appliance vmAppliance, Pool pool)
            : base (pool)
        {
            this.vmAppliance = vmAppliance;
        }

        protected override Problem RunCheck()
        {
            if (vmAppliance != null)
            {
                foreach (VM_appliance appliance in Pool.Connection.Cache.VM_appliances)
                {
                    if (appliance.uuid == vmAppliance.uuid)
                        return appliance.IsRunning ? new RunningVmApplianceProblem(this, appliance, false) : null;
                }
            }
            return null;
        }

        public override string Description
        {
            get
            {
                return string.Format(Messages.DR_WIZARD_APPLIANCE_CHECK_DESCRIPTION, vmAppliance.Name); 
            }
        }
    }
}
