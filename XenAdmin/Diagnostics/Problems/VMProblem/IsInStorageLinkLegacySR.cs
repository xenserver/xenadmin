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

using System;
using System.Collections.Generic;
using System.Drawing;
using XenAdmin.Actions;
using XenAdmin.Actions.VMActions;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Dialogs;
using XenAdmin.Properties;
using XenAPI;


namespace XenAdmin.Diagnostics.Problems.VMProblem
{
    class IsInStorageLinkLegacySR : Problem
    {
        // Case 1: VMs using one of the CSLG adapters we can upgrade
        private readonly List<VM> VMs = new List<VM>();
        public IsInStorageLinkLegacySR(Check check, List<VM> vm)
            : base(check)
        {
            VMs = vm;
        }

        // Case 2: VM using one of the CSLG adapters we can't upgrade 
        private readonly VM NotSMISVM;
        public IsInStorageLinkLegacySR(Check check, VM vm)
            : base(check)
        {
            NotSMISVM = vm;
        }

        public override string Title
        {
            get { return Description; }
        }

        public override string Description
        {
            get
            {
                if (NotSMISVM != null)
                    return Messages.ONLY_VMS_USING_STORAGELINK_SMIS;
                return Messages.IS_IN_STORAGELINK_SR;
            }
        }

        public override string HelpMessage
        {
            get
            {
                if (NotSMISVM != null)
                    return Messages.TELL_ME_MORE;
                else
                    return Messages.SHUTDOWN_VM;
            }
        }

        protected override AsyncAction CreateAction(out bool cancelled)
        {
            cancelled = false;
            if (VMs.Count == 1)
            {
                if (VMs[0].power_state == vm_power_state.Suspended)
                    return new VMHardShutdown(VMs[0]);
                return new VMCleanShutdown(VMs[0]);
            }
            if (VMs.Count > 1)
            {
                var listShutdownActions = new List<AsyncAction>();
                VMs.ForEach((vm) =>
                {
                    if (vm.power_state == vm_power_state.Suspended)
                        listShutdownActions.Add(new VMHardShutdown(vm));
                    else
                        listShutdownActions.Add(new VMCleanShutdown(vm));
                });
                return new MultipleAction(VMs[0].Connection, Messages.ACTION_VM_SHUTTING_DOWN, Messages.ACTION_VM_SHUTTING_DOWN
                    , Messages.ACTION_VM_SHUT_DOWN, listShutdownActions);
            }
            return null;
        }
    }
}
