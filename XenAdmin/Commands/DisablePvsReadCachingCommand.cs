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

using System.Collections.Generic;
using XenAPI;
using System.Linq;
using XenAdmin.Actions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the properties dialog for the selected XenObject.
    /// </summary>
    internal class DisablePvsReadCachingCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        internal DisablePvsReadCachingCommand()
        {
        }

        public DisablePvsReadCachingCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public DisablePvsReadCachingCommand(IMainWindow mainWindow, IXenObject xenObject)
            : base(mainWindow, xenObject)
        {
        }

        protected virtual void Execute(IList<VM> vms)
        {
            var actions = new List<AsyncAction>();

            foreach (var vm in vms)
            {
                var action = GetAsyncActionForVm(vm);
                if (action != null)
                {
                    actions.Add(action);
                }
            }

            if (actions.Any())
            {
                if (actions.Count == 1)
                {
                    actions[0].RunAsync();
                }
                else
                {
                    new ParallelAction(
                        Messages.ACTION_DISABLE_PVS_READ_CACHING,
                        Messages.ACTION_DISABLING_PVS_READ_CACHING,
                        Messages.ACTION_DISABLED_PVS_READ_CACHING,
                        actions).RunAsync();
                }
            }
        }

        /// <summary>
        /// If the VM can have PVS read-caching disabled, returns an action to do so. Else returns null
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        private AsyncAction GetAsyncActionForVm(VM vm)
        {
            var pvsProxy = vm.Connection.Cache.PVS_proxies.FirstOrDefault(p => p.VM.Equals(vm)); // null if no proxy

            if (pvsProxy == null) return null; // No proxy to disable

            return new PvsProxyDestroyAction(pvsProxy);
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Execute(selection.AsXenObjects<VM>());
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            // Can execute criteria: A selection of VMs in the same pool, where at least one doesn't have PVS read caching enabled
            if (selection.Any() &&  selection.AllItemsAre<VM>() && selection.GetConnectionOfAllItems() != null)
            {
                var vms = selection.AsXenObjects<VM>();
                if (vms.Any(vm => vm.PvsProxy != null))
                {
                    return true;
                }
            }

            return false;
        }

        public override string ButtonText
        {
            get { return Messages.DISABLE_PVS_READ_CACHING_BUTTON; }
        }

        public override string MenuText
        {
            get { return Messages.DISABLE_PVS_READ_CACHING_MENU; }
        }
    }
}