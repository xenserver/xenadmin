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


namespace XenAdmin.Actions
{
    public class VMSnapshotRevertAction : PureAsyncAction
    {
        private VM m_Snapshot;
        private Host previousHost; // The host the VM was running on before the snapshot

        public VMSnapshotRevertAction(VM snapshot)
            : base(snapshot.Connection, String.Format(Messages.ACTION_VM_REVERT_SNAPSHOT_TITLE, snapshot.Name))
        {
            this.VM = Connection.Resolve<VM>(snapshot.snapshot_of);
            previousHost = Connection.Resolve<Host>(VM.resident_on);
            this.m_Snapshot = snapshot;
            Description = String.Format(Messages.VM_REVERTING, m_Snapshot.Name);

        }
        private bool _finished = false;
        protected override void Run()
        {


            Description = String.Format(Messages.VM_REVERTING, m_Snapshot.Name);
            RelatedTask = XenAPI.VM.async_revert(Session, m_Snapshot.opaque_ref);
            PollToCompletion();
            _finished = true;
            PercentComplete = 90;
            Description = String.Format(Messages.REVERTING_POWER_STATE, VM.Name);
            try
            {
                RevertPowerState(m_Snapshot, VM);
            }
            catch (Exception)
            { }
            PercentComplete = 100;
            Description = String.Format(Messages.VM_REVERTED, m_Snapshot.Name);
        }

        public override int PercentComplete
        {
            get
            {
                return base.PercentComplete;
            }
            set
            {
                if ((value < 90 && !_finished) || (value >= 90 && _finished))
                    base.PercentComplete = value;
            }
        }


        private void RevertPowerState(VM snapshot, VM vm)
        {
            string state;
            if (snapshot.snapshot_info.TryGetValue("power-state-at-snapshot",
                                                   out state))
            {
                if (state == vm_power_state.Running.ToString())
                {
                    if (vm.power_state == vm_power_state.Halted)
                    {
                        if (previousHost != null && VMCanBootOnHost(vm, previousHost))
                        {
                            RelatedTask = XenAPI.VM.async_start_on(Session,
                                vm.opaque_ref, previousHost.opaque_ref, false, false);
                        }
                        else
                        {
                            RelatedTask = XenAPI.VM.async_start(Session,
                                vm.opaque_ref, false, false);
                        }

                    }
                    else if (vm.power_state == vm_power_state.Suspended)
                    {
                        if (previousHost != null && VMCanBootOnHost(vm, previousHost))
                        {
                            RelatedTask = XenAPI.VM.async_resume_on(Session, vm.opaque_ref, previousHost.opaque_ref,
                                false, false);
                        }
                        else
                        {
                            RelatedTask = XenAPI.VM.async_resume(Session, vm.opaque_ref, false, false);
                        }

                    }
                    PollToCompletion();
                }
            }
        }

        private bool VMCanBootOnHost(VM vm, Host host)
        {
            try
            {
                VM.assert_can_boot_here(Session, vm.opaque_ref, host.opaque_ref);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}