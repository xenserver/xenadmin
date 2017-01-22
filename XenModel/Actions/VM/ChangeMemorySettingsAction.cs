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
using XenAPI;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Actions
{
    public class ChangeMemorySettingsAction : AsyncAction
    {
        long static_min, static_max, dynamic_min, dynamic_max;
        private Action<VMStartAbstractAction, Failure> _startDiagnosticForm;
        private Action<VM, bool> _warningDialogHAInvalidConfig;

        public ChangeMemorySettingsAction(VM vm, string title,
            long static_min, long dynamic_min, long dynamic_max, long static_max, Action<VM, bool> warningDialogHAInvalidConfig, Action<VMStartAbstractAction, Failure> startDiagnosticForm, bool suppressHistory)
            : base(vm.Connection, title, suppressHistory)
        {
            _warningDialogHAInvalidConfig = warningDialogHAInvalidConfig;
            _startDiagnosticForm = startDiagnosticForm;
            VM = vm;
            this.static_min = static_min;
            this.dynamic_min = dynamic_min;
            this.dynamic_max = dynamic_max;
            this.static_max = static_max;

            #region RBAC Dependencies

            if (staticChanged)
                ApiMethodsToRoleCheck.Add("vm.set_memory_limits");
            else
                ApiMethodsToRoleCheck.Add("vm.set_memory_dynamic_range");

            if (needReboot)
            {
                if (VM.allowed_operations.Contains(XenAPI.vm_operations.clean_shutdown))
                    ApiMethodsToRoleCheck.Add("vm.clean_shutdown");
                else
                    ApiMethodsToRoleCheck.Add("vm.hard_shutdown");
                ApiMethodsToRoleCheck.Add("vm.start_on");
            }

            #endregion
        }

        private bool staticChanged
        {
            get
            {
                return (static_min != VM.memory_static_min || static_max != VM.memory_static_max);
            }
        }

        private bool needReboot
        {
            get
            {
                return (staticChanged && VM.power_state != vm_power_state.Halted);
            }
        }

        protected override void Run()
        {
            // If either of the static memories has changed, we need to shut down the VM
            // before and reboot afterwards. The user has already been warned about this.
            bool reboot = needReboot;
            XenAPI.Host vmHost = null;
            if (reboot)
            {
                vmHost = VM.Home();
                AsyncAction action = null;
                if (VM.allowed_operations.Contains(XenAPI.vm_operations.clean_shutdown))
                    action = new VMCleanShutdown(VM);
                else
                    action = new VMHardShutdown(VM);
                action.RunExternal(Session);
            }

            // Now save the memory settings. We can't use VM.SaveChanges() for this,
            // because we have to do the operations simultaneously or we will
            // violate the memory ordering constraints.
            try
            {
                if (staticChanged)
                    XenAPI.VM.set_memory_limits(Session, VM.opaque_ref, static_min, static_max, dynamic_min, dynamic_max);
                else
                    XenAPI.VM.set_memory_dynamic_range(Session, VM.opaque_ref, dynamic_min, dynamic_max);
            }

            // Reboot the VM, even if we failed to change the memory settings
            finally
            {
                if (reboot)
                {
                    var action = new VMStartOnAction(VM, vmHost, _warningDialogHAInvalidConfig, _startDiagnosticForm);
                    action.RunExternal(Session);
                }
            }

            Description = string.Format(Messages.ACTION_CHANGE_MEMORY_SETTINGS_DONE, VM.Name);
        }

      
    }
}
