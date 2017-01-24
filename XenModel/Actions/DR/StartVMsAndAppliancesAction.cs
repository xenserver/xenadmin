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
using XenAdmin.Actions;
using XenAdmin.Actions.VMActions;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.DR
{
    public class StartVMsAndAppliancesAction : AsyncAction
    {
        private readonly List<VM> VmsToStart;
        private readonly List<VM_appliance> VmAppliancesToStart;
        private readonly bool paused;

        private int ActionCountTotal;
        private int ActionCountCompleted;
        private Action<VMStartAbstractAction, Failure> _startDiagnosisForm;
        private Action<VM, bool> _warningDialogHAInvalidConfig;

        public StartVMsAndAppliancesAction(IXenConnection connection, List<VM> vmsToStart, List<VM_appliance> vmAppliancesToStart, Action<VM, bool> warningDialogHAInvalidConfig, Action<VMStartAbstractAction, Failure> startDiagnosisForm, bool paused)
            : base(connection, Messages.ACTION_START_VMS_AND_APPLIANCES_TITLE, true)
        {
            VmsToStart = vmsToStart;
            VmAppliancesToStart = vmAppliancesToStart;
            _warningDialogHAInvalidConfig = warningDialogHAInvalidConfig;
            _startDiagnosisForm = startDiagnosisForm;
            this.paused = paused;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);

            if (vmsToStart.Count > 0)
                ApiMethodsToRoleCheck.Add("vm.start");
            if (vmAppliancesToStart.Count > 0)
                ApiMethodsToRoleCheck.Add("VM_appliance.start");
            #endregion
        }

        protected override void Run()
        {
            if (VmsToStart.Count == 0 && VmAppliancesToStart.Count == 0)
            {
                throw new Exception(Messages.ACTION_START_VMS_AND_APPLIANCES_NONE_SELECTED);
            }

            ActionCountCompleted = 0;
            ActionCountTotal = 100 / (VmsToStart.Count + VmAppliancesToStart.Count);
            
            foreach (VM vm in VmsToStart)
            {
                VMStartAbstractAction action;
                if (paused) 
                    action = new VMStartPausedAction(vm, _warningDialogHAInvalidConfig, _startDiagnosisForm);
                else 
                    action = new VMStartAction(vm, _warningDialogHAInvalidConfig, _startDiagnosisForm);
                Description = action.Title;
                action.Changed += action_Changed;
                action.RunExternal(Session);
                ActionCountCompleted++;
            }

            foreach (VM_appliance vmAppliance in VmAppliancesToStart)
            {
                var action = new StartApplianceAction(vmAppliance, paused);
                Description = action.Title;
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
