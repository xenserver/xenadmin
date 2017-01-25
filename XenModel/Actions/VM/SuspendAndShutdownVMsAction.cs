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
using System.Text;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.VMActions
{
    public class SuspendAndShutdownVMsAction : AsyncAction
    {
        private List<VM> VmsToSuspend;
        private List<VM> VmsToShutdown;

        private int ActionCountTotal;
        private int ActionCountCompleted;

        public SuspendAndShutdownVMsAction(IXenConnection connection, Host host, List<VM> vmsToSuspend, List<VM> vmsToShutdown)
            : base(connection, Messages.SUSPEND_SHUTDOWN_VMS_ACTION_DESC, true)
        {
            Host = host;
            VmsToSuspend = vmsToSuspend;
            VmsToShutdown = vmsToShutdown;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);

            if (vmsToSuspend.Count > 0)
                ApiMethodsToRoleCheck.Add("vm.suspend");
            if (vmsToShutdown.Count > 0)
                ApiMethodsToRoleCheck.Add("vm.hard_shutdown");
            #endregion
        }

        protected override void Run()
        {
            ActionCountCompleted = 0;
            ActionCountTotal = VmsToSuspend.Count + VmsToShutdown.Count;

            foreach (VM vm in VmsToSuspend)
            {
                Description = string.Format(Messages.SUSPENDING_VM_OUT_OF, ActionCountCompleted + 1, VmsToSuspend.Count);
                var action = new VMSuspendAction(vm);
                action.Changed += action_Changed;
                action.RunExternal(Session);
                ActionCountCompleted++;
            }

            foreach (VM vm in VmsToShutdown)
            {
                Description = string.Format(Messages.SHUTTING_DOWN_VM_OUT_OF, ActionCountCompleted - VmsToSuspend.Count + 1, VmsToShutdown.Count);
                var action = new VMHardShutdown(vm);
                action.Changed += action_Changed;
                action.RunExternal(Session);
                ActionCountCompleted++;
            }
        }

        void action_Changed(ActionBase a)
        {
            PercentComplete = ((ActionCountCompleted * 100) + a.PercentComplete) / ActionCountTotal;
        }
    }
}
