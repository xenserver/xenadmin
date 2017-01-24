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
using XenAdmin.Core;


namespace XenAdmin.Actions.VMActions
{
    public abstract class VMStartAbstractAction : AsyncAction
    {
        protected Action<VM, bool> _WarningDialogHAInvalidConfig;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected Action<VMStartAbstractAction, Failure> _StartDiagnosisForm;

        protected VMStartAbstractAction(VM vm, string title, Action<VM, bool> warningDialogHAInvalidConfig, Action<VMStartAbstractAction, Failure> startDiagnosisForm)
            : base(vm.Connection, title)
        {
            this.Description = Messages.ACTION_PREPARING;
            this.VM = vm;
            _WarningDialogHAInvalidConfig = warningDialogHAInvalidConfig;
            _StartDiagnosisForm = startDiagnosisForm;
            this.Host = vm.Home();
            this.Pool = Core.Helpers.GetPool(vm.Connection);
            AddCommonAPIMethodsToRoleCheck();
        }
        #region Helpers

        public abstract bool IsStart { get; }

        protected abstract void DoAction(int start, int end);

        /// <summary>
        /// Starts or resumes the given VM, updating the given AsyncAction. If appropriate, also enables HA protection and does a pool db sync.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="vm"></param>
        /// <param name="host">May be null if the action is not a ResumeOn/StartOn.</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected void StartOrResumeVmWithHa(int start, int end)
        {

            try
            {
                Pool pool = Helpers.GetPoolOfOne(VM.Connection);
                if (pool != null && pool.ha_enabled && VM.HaPriorityIsRestart())
                {
                    try
                    {
                        XenAPI.VM.assert_agile(Session, VM.opaque_ref);
                    }
                    catch (Failure exn)
                    {
                        // VM is not agile, but it is 'Protected' by HA. This is an inconsistent state (see CA-20820).
                        // Warn the user about this and ask if they wish to fix it.
                        log.Debug("VM is not agile, but protected", exn);
                        _WarningDialogHAInvalidConfig(VM,IsStart);
                        new HAUnprotectVMAction(VM).RunExternal(Session);
                    }
                }

                DoAction(start, end);
            }
            catch (Failure f)
            {
                _StartDiagnosisForm(this, f);
                throw;
            }
        }

        public abstract VMStartAbstractAction Clone();

        #endregion
    }

    public class VMStartAction : VMStartAbstractAction
    {
        public VMStartAction(VM vm, Action<VM, bool> warningDialogHAInvalidConfig,Action<VMStartAbstractAction,Failure> startDiagnosticForm)
            : base(vm, string.Format(Messages.ACTION_VM_STARTING_TITLE, vm.Name), warningDialogHAInvalidConfig, startDiagnosticForm)
        {
            ApiMethodsToRoleCheck.Add("vm.start");
        }

        public override bool IsStart
        {
            get { return true; }
        }

        protected override void Run()
        {
            this.Description = Messages.ACTION_VM_STARTING;
            StartOrResumeVmWithHa(0, 100);
            this.Description = Messages.ACTION_VM_STARTED;
        }

        protected override void DoAction(int start, int end)
        {
            RelatedTask = XenAPI.VM.async_start(Session, VM.opaque_ref, false, false);
            PollToCompletion(start, end);
        }

        public override VMStartAbstractAction Clone()
        {
            return new VMStartAction(VM, _WarningDialogHAInvalidConfig, _StartDiagnosisForm);
        }
    }

    public class VMStartOnAction : VMStartAbstractAction
    {
        public VMStartOnAction(VM vm, Host hostToStart, Action<VM, bool> warningDialogHAInvalidConfig, Action<VMStartAbstractAction, Failure> startDiagnosticForm)
            : base(vm, string.Format(Messages.ACTION_VM_STARTING_ON_TITLE, vm.Name, hostToStart.Name),warningDialogHAInvalidConfig,startDiagnosticForm)
        {
            Host = hostToStart;
            ApiMethodsToRoleCheck.Add("vm.start_on");
        }

        public override bool IsStart
        {
            get { return true; }
        }

        protected override void Run()
        {
            this.Description = Messages.ACTION_VM_STARTING;
            StartOrResumeVmWithHa(0, 100);
            this.Description = Messages.ACTION_VM_STARTED;
        }

        protected override void DoAction(int start, int end)
        {
            RelatedTask = XenAPI.VM.async_start_on(Session, VM.opaque_ref, Host.opaque_ref, false, false);
            PollToCompletion(start, end);
        }

        public override VMStartAbstractAction Clone()
        {
            return new VMStartOnAction(VM, Host, _WarningDialogHAInvalidConfig, _StartDiagnosisForm);
        }
    }

    public class VMResumeAction : VMStartAbstractAction
    {
        public VMResumeAction(VM vm, Action<VM, bool> warningDialogHAInvalidConfig,Action<VMStartAbstractAction, Failure> startDiagnosticForm)
            : base(vm,string.Format(Messages.ACTION_VM_RESUMING_TITLE, vm.Name),warningDialogHAInvalidConfig,startDiagnosticForm)
        {

            ApiMethodsToRoleCheck.Add("vm.resume");
        }

        public override bool IsStart
        {
            get { return false; }
        }

        protected override void Run()
        {
            this.Description = Messages.ACTION_VM_RESUMING;
            StartOrResumeVmWithHa(0, 100);
            this.Description = Messages.ACTION_VM_RESUMED;
        }

        protected override void DoAction(int start, int end)
        {
            RelatedTask = XenAPI.VM.async_resume(Session, VM.opaque_ref, false, false);
            PollToCompletion(start, end);
        }

        public override VMStartAbstractAction Clone()
        {
            return new VMResumeAction(VM, _WarningDialogHAInvalidConfig, _StartDiagnosisForm);
        }
    }

    public class VMResumeOnAction : VMStartAbstractAction
    {
        public VMResumeOnAction(VM vm, Host hostToStart, Action<VM, bool> warningDialogHAInvalidConfig,Action<VMStartAbstractAction, Failure> startDiagnosticForm)
            : base(vm, string.Format(Messages.ACTION_VM_RESUMING_ON_TITLE, vm.Name, hostToStart.Name), warningDialogHAInvalidConfig,startDiagnosticForm)
        {
            Host = hostToStart; 
            ApiMethodsToRoleCheck.Add("vm.resume_on");
        }

        public override bool IsStart
        {
            get { return false; }
        }

        protected override void Run()
        {
            this.Description = Messages.ACTION_VM_RESUMING;
            StartOrResumeVmWithHa(0, 100);
            this.Description = Messages.ACTION_VM_RESUMED;
        }

        protected override void DoAction(int start, int end)
        {
            RelatedTask = XenAPI.VM.async_resume_on(Session, VM.opaque_ref, Host.opaque_ref, false, false);
            PollToCompletion(start, end);
        }

        public override VMStartAbstractAction Clone()
        {
            return new VMResumeOnAction(VM, Host, _WarningDialogHAInvalidConfig, _StartDiagnosisForm);
        }
    }

    public class VMStartPausedAction : VMStartAbstractAction
    {
        public VMStartPausedAction(VM vm, Action<VM, bool> warningDialogHAInvalidConfig,Action<VMStartAbstractAction,Failure> startDiagnosticForm)
            : base(vm, string.Format(Messages.ACTION_VM_STARTING_PAUSED_TITLE, vm.Name), warningDialogHAInvalidConfig, startDiagnosticForm)
        {
            ApiMethodsToRoleCheck.Add("vm.start");
        }

        public override bool IsStart
        {
            get { return true; }
        }

        protected override void Run()
        {
            this.Description = Messages.ACTION_VM_STARTING_PAUSED;
            StartOrResumeVmWithHa(0, 100);
            this.Description = Messages.ACTION_VM_STARTED_PAUSED;
        }

        protected override void DoAction(int start, int end)
        {
            RelatedTask = XenAPI.VM.async_start(Session, VM.opaque_ref, true, false);
            PollToCompletion(start, end);
        }

        public override VMStartAbstractAction Clone()
        {
            return new VMStartPausedAction(VM, _WarningDialogHAInvalidConfig, _StartDiagnosisForm);
        }
    }

    public class VMStartPausedOnAction  : VMStartAbstractAction
    {
        public VMStartPausedOnAction(VM vm, Host hostToStart, Action<VM, bool> warningDialogHAInvalidConfig, Action<VMStartAbstractAction, Failure> startDiagnosticForm)
            : base(vm, string.Format(Messages.ACTION_VM_STARTING_PAUSED_ON_TITLE, vm.Name, hostToStart.Name), warningDialogHAInvalidConfig, startDiagnosticForm)
        {
            Host = hostToStart;
            ApiMethodsToRoleCheck.Add("vm.start_on");
        }

        public override bool IsStart
        {
            get { return true; }
        }

        protected override void Run()
        {
            this.Description = Messages.ACTION_VM_STARTING_PAUSED;
            StartOrResumeVmWithHa(0, 100);
            this.Description = Messages.ACTION_VM_STARTED_PAUSED;
        }

        protected override void DoAction(int start, int end)
        {
            RelatedTask = XenAPI.VM.async_start_on(Session, VM.opaque_ref, Host.opaque_ref, true, false);
            PollToCompletion(start, end);
        }

        public override VMStartAbstractAction Clone()
        {
            return new VMStartPausedOnAction(VM, Host, _WarningDialogHAInvalidConfig, _StartDiagnosisForm);
        }
    }
}
