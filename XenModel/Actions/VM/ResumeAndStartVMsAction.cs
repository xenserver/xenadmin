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
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Network;

namespace XenAdmin.Actions.VMActions
{
    public class ResumeAndStartVMsAction : AsyncAction
    {
        private readonly List<VM> VmsToResume;
        private readonly List<VM> VmsToStart;

        private int ActionCountTotal;
        private int ActionCountCompleted;
        private Action<VMStartAbstractAction, Failure> _startDiagnosisForm;
        private Action<VM, bool> _warningDialogHAInvalidConfig;

        public ResumeAndStartVMsAction(IXenConnection connection, Host host, List<VM> vmsToResume, List<VM> vmsToStart,Action<VM, bool> warningDialogHAInvalidConfig, Action<VMStartAbstractAction, Failure> startDiagnosisForm)
            : base(connection, Messages.ACTION_RESUMEANDSTARTVMS_PREPARING, true)
        {
            Host = host;
            VmsToResume = vmsToResume;
            VmsToStart = vmsToStart;
            _warningDialogHAInvalidConfig = warningDialogHAInvalidConfig;
            _startDiagnosisForm = startDiagnosisForm;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);

            if (vmsToResume.Count > 0)
                ApiMethodsToRoleCheck.Add("vm.resume_on");
            if (vmsToStart.Count > 0)
                ApiMethodsToRoleCheck.Add("vm.start_on");
            #endregion
        }

        protected override void Run()
        {
            ActionCountCompleted = 0;
            ActionCountTotal = VmsToResume.Count + VmsToStart.Count;

            foreach (VM vm in VmsToResume)
            {
                Description = string.Format(Messages.ACTION_RESUMEANDSTARTVMS_RESUMINGN, ActionCountCompleted, VmsToResume.Count);

                AsyncAction action;
                if (CanVMBootOnHost(vm, Host))
                    action = new VMResumeOnAction(vm, Host, _warningDialogHAInvalidConfig, _startDiagnosisForm);
                else
                    action = new VMResumeAction(vm, _warningDialogHAInvalidConfig, _startDiagnosisForm);

                action.Changed += action_Changed;
                action.RunExternal(Session);
                ActionCountCompleted++;
            }

            foreach (VM vm in VmsToStart)
            {
                Description = string.Format(Messages.ACTION_RESUMEANDSTARTVMS_STARTINGN, ActionCountCompleted - VmsToResume.Count, VmsToStart.Count);

                AsyncAction action;
                if (CanVMBootOnHost(vm, Host))
                    action = new VMStartOnAction(vm, Host, _warningDialogHAInvalidConfig, _startDiagnosisForm);
                else
                    action = new VMStartAction(vm, _warningDialogHAInvalidConfig, _startDiagnosisForm);

                action.Changed += action_Changed;
                action.RunExternal(Session);
                ActionCountCompleted++;
            }
        }

        private bool CanVMBootOnHost(VM vm, Host host)
        {
            try
            {
                VM.assert_can_boot_here(Connection.Session, vm.opaque_ref, host.opaque_ref);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void action_Changed(ActionBase a)
        {
            PercentComplete = ((ActionCountCompleted * 100) + a.PercentComplete) / ActionCountTotal;
        }
    }
}
