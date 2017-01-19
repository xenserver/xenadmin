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

using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.VMProblem;
using XenAPI;


namespace XenAdmin.Diagnostics.Checks.DR
{
    public class ExistingVmCheck: PoolCheck
    {
        private readonly VM Vm;

        /// <summary>
        /// Check if the specified VM exists and is running on specified pool.
        /// </summary>
        /// <param name="vm">the VM for which to check the running state</param>
        /// <param name="pool">the pool on which to check if the specified VM exists and is running</param>
        public ExistingVmCheck(VM vm, Pool pool)
            : base (pool)
        {
            Vm = vm;
        }

        protected override Problem RunCheck()
        {
            foreach (VM existingVm in Pool.Connection.Cache.VMs)
            {
                if (existingVm.uuid == Vm.uuid)
                {
                    // if the existing VM's version is >= the version in the metadata import, then the used must destroy the VM first 
                    if (existingVm.version >= Vm.version)
                        return new ExistingVmProblem(this, existingVm);
                    
                    // if the existing VM is currently running, then the user must force shutdown this VM first
                    if (VmIsRunning(existingVm))
                        return new RunningVMProblem(this, existingVm, true);
                
                    // warn the user that the existing VM will be replaced 
                    return new ExistingVmWarning(this, existingVm);
                }
            }
            return null;
        }

        private bool VmIsRunning(VM vm)
        {
            return vm.power_state == vm_power_state.Running
                    || vm.power_state == vm_power_state.Paused
                    || vm.power_state == vm_power_state.Suspended;
        }

        public override string Description
        {
            get
            {
                return string.Format(Messages.DR_WIZARD_VM_CHECK_DESCRIPTION, Vm.Name);
            }
        }
    }
}
