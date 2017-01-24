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

using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions.VMActions
{
    public abstract class VMShutdownAction : PureAsyncAction
    {
        protected VMShutdownAction(VM vm,string title)
            : base(vm.Connection, title)
        {
            this.Description = Messages.ACTION_PREPARING;
            this.VM = vm;
            this.Host = vm.Home();
            this.Pool = Core.Helpers.GetPool(vm.Connection);
        }

        /// <summary>
        /// Enables or disables HA protection for a VM (VM.ha_always_run). Also does a pool.sync_database afterwards.
        /// May throw a XenAPI.Failure.
        /// </summary>
        /// <param name="protect"></param>
        /// <param name="act"></param>
        /// <param name="session"></param>
        /// <param name="vm"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        protected static void SetHaProtection(bool protect, AsyncAction action, VM vm, int start, int end)
        {
        	// Do database sync. Helps to ensure that the change persists over master failover.
            action.RelatedTask = XenAPI.Pool.async_sync_database(action.Session);
            action.PollToCompletion(start, end);
        }


    }

    public class VMCleanShutdown : VMShutdownAction
    {
        public VMCleanShutdown(VM vm)
            : base(vm, string.Format(Messages.ACTION_VM_SHUTTING_DOWN_TITLE, vm.Name, vm.Home() == null ? Helpers.GetName(vm.Connection) : vm.Home().Name))
        {
        }

        protected override void Run()
        {
            bool vmHasSavedRestartPriority = VM.HasSavedRestartPriority;
            this.Description = Messages.ACTION_VM_SHUTTING_DOWN;

            if (vmHasSavedRestartPriority)
            {
                // Disable HA protection
                SetHaProtection(false, this, VM, 0, 50);
            }

            RelatedTask = XenAPI.VM.async_clean_shutdown(Session, VM.opaque_ref);
            PollToCompletion(vmHasSavedRestartPriority ? 50 : 0, 100);
            this.Description = Messages.ACTION_VM_SHUT_DOWN;
        }
    }

    public class VMHardShutdown : VMShutdownAction
    {
        public VMHardShutdown(VM vm)
            : base(vm, string.Format(Messages.ACTION_VM_SHUTTING_DOWN_TITLE, vm.Name, vm.Home() == null ? Helpers.GetName(vm.Connection) : vm.Home().Name))
        {
        }

        protected override void Run()
        {
            bool vmHasSavedRestartPriority =VM.HasSavedRestartPriority;
            this.Description = Messages.ACTION_VM_SHUTTING_DOWN;

            if (vmHasSavedRestartPriority)
            {
                // Disable HA protection
                SetHaProtection(false, this, VM, 0, 70);
            }
            RelatedTask = XenAPI.VM.async_hard_shutdown(Session, VM.opaque_ref);
            PollToCompletion(vmHasSavedRestartPriority ? 70 : 0, 100);

            this.Description = Messages.ACTION_VM_SHUT_DOWN;
        }
    }
}
