/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Linq;
using XenAPI;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Actions
{
    public class ChangeMemorySettingsAction : AsyncAction
    {
        private readonly long _staticMin, _staticMax, _dynamicMin, _dynamicMax;

        private Action<VMStartAbstractAction, Failure> _startDiagnosticForm;
        private Action<VM, bool> _warningDialogHaInvalidConfig;

        private readonly bool _staticChanged;
        private readonly bool _needReboot;
        private readonly Host _vmHost;

        public ChangeMemorySettingsAction(VM vm, string title,
            long staticMin, long dynamicMin, long dynamicMax, long staticMax, Action<VM, bool> warningDialogHaInvalidConfig, Action<VMStartAbstractAction, Failure> startDiagnosticForm, bool suppressHistory)
            : base(vm.Connection, title, suppressHistory)
        {
            _warningDialogHaInvalidConfig = warningDialogHaInvalidConfig;
            _startDiagnosticForm = startDiagnosticForm;
            VM = vm;

            _staticMin = staticMin;
            _dynamicMin = dynamicMin;
            _dynamicMax = dynamicMax;
            _staticMax = staticMax;

            _vmHost = VM.Home();
            _staticChanged = staticMin != VM.memory_static_min || staticMax != VM.memory_static_max;

            if (_staticChanged)
                _needReboot = VM.power_state != vm_power_state.Halted;
            else
                _needReboot = VM.power_state != vm_power_state.Halted && VM.power_state != vm_power_state.Running;

            #region RBAC Dependencies

            if (_staticChanged)
                ApiMethodsToRoleCheck.Add("vm.set_memory_limits");
            else
                ApiMethodsToRoleCheck.Add("vm.set_memory_dynamic_range");

            if (_needReboot)
            {
                if (VM.allowed_operations.Contains(vm_operations.clean_shutdown))
                    ApiMethodsToRoleCheck.Add("vm.clean_shutdown");
                else
                    ApiMethodsToRoleCheck.Add("vm.hard_shutdown");

                if (_vmHost == null)
                    ApiMethodsToRoleCheck.Add("vm.start");
                else
                    ApiMethodsToRoleCheck.Add("vm.start_on");
            }

            #endregion
        }

        protected override void Run()
        {
            // If either of the static memories has changed, we need to shut down the VM
            // before and reboot afterwards. The user has already been warned about this.

            if (_needReboot)
            {
                AsyncAction action;
                if (VM.allowed_operations.Contains(vm_operations.clean_shutdown))
                    action = new VMCleanShutdown(VM);
                else
                    action = new VMHardShutdown(VM);

                try
                {
                    action.RunSync(Session);
                }
                catch
                {
                    if (VM.power_state == vm_power_state.Halted || VM.current_operations.Any(op =>
                        op.Value == vm_operations.clean_shutdown || op.Value == vm_operations.hard_shutdown ||
                        op.Value == vm_operations.shutdown))
                    {
                        //ignore if it got already powered off or there are other shutting down tasks in progress
                    }
                    else
                        throw;
                }

                VM.Connection.WaitFor(() => VM.power_state == vm_power_state.Halted, GetCancelling);
            }

            // Now save the memory settings. We can't use VM.SaveChanges() for this,
            // because we have to do the operations simultaneously or we will
            // violate the memory ordering constraints.

            try
            {
                if (_staticChanged)
                    VM.set_memory_limits(Session, VM.opaque_ref, _staticMin, _staticMax, _dynamicMin, _dynamicMax);
                else
                    VM.set_memory_dynamic_range(Session, VM.opaque_ref, _dynamicMin, _dynamicMax);
            }
            finally
            {
                if (_needReboot)
                {
                    // boot the VM, even if we failed to change the memory settings after the shutdown
                    VMStartAbstractAction action;
                    if (_vmHost == null)
                        action = new VMStartAction(VM, _warningDialogHaInvalidConfig, _startDiagnosticForm);
                    else
                        action = new VMStartOnAction(VM, _vmHost, _warningDialogHaInvalidConfig, _startDiagnosticForm);

                    action.RunSync(Session);
                }
            }

            Description = string.Format(Messages.ACTION_CHANGE_MEMORY_SETTINGS_DONE, VM.Name());
        }
    }
}
