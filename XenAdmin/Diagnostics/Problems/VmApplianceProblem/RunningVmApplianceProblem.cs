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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Actions.VMActions;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs.VMAppliances;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.VmApplianceProblem
{
    public class RunningVmApplianceProblem : VmApplianceProblem
    {
        private readonly bool hardShutdown;

        public RunningVmApplianceProblem(Check check, VM_appliance vmAppliance, bool hardShutdown)
            : base(check, vmAppliance)
        {
            this.hardShutdown = hardShutdown;
        }

        public override string Description
        {
            get
            {
                return String.Format(Messages.DR_WIZARD_PROBLEM_RUNNING_APPLIANCE, Helpers.GetPoolOfOne(VmAppliance.Connection).Name); 
            } 
        }

        public override string HelpMessage
        {
            get { return Messages.DR_WIZARD_PROBLEM_RUNNING_APPLIANCE_HELPMESSAGE; } 
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            Program.AssertOnEventThread();
            cancelled = false;

            //now check for fate-sharing VMs
            var fateSharingVms = VmAppliance.GetFateSharingVMs();
            bool shutdown = false;

            if (fateSharingVms.Count > 0)
            {
                using (var dlog = new FateSharingVMsDialog())
                {
                    dlog.Populate(fateSharingVms);
                    shutdown = (dlog.ShowDialog(Program.MainWindow) == DialogResult.Yes);
                }
            }

            //shut down appliance action
            AsyncAction shutDownAction = hardShutdown
                                             ? (AsyncAction) new HardShutDownApplianceAction(VmAppliance)
                                             : new ShutDownApplianceAction(VmAppliance);

            if (!shutdown)
                return shutDownAction;

            //shut down fate-sharing VMs
            var actions = new List<AsyncAction> {shutDownAction};

            foreach (var vm in fateSharingVms)
            {
                if (vm.allowed_operations.Contains(vm_operations.clean_shutdown))
                    actions.Add(new VMCleanShutdown(vm));
                else
                    actions.Add(new VMHardShutdown(vm));
            }
            return new MultipleAction(VmAppliance.Connection, Messages.ACTION_SHUTTING_DOWN,
                                      Messages.ACTION_SHUTTING_DOWN, Messages.ACTION_SHUT_DOWN, actions);
        }
    }
}
