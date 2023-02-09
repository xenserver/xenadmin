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
using System.Collections.Generic;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Disable changed block tracking of the selected VM.
    /// </summary>
    internal class DisableChangedBlockTrackingCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public DisableChangedBlockTrackingCommand()
        {
        }

        public DisableChangedBlockTrackingCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public DisableChangedBlockTrackingCommand(IMainWindow mainWindow, IXenObject xenObject)
            : base(mainWindow, xenObject)
        {
        }

        private void Run(IList<VM> vms)
        {
            var actions = new List<AsyncAction>();

            foreach (var vm in vms)
            {
                if (vm.is_a_template)
                    continue;
                foreach (VBD vbd in vm.Connection.ResolveAll(vm.VBDs))
                {
                    VDI vdi = vm.Connection.Resolve(vbd.VDI);
                    if (vdi != null && vdi.cbt_enabled)
                        actions.Add(new VDIDisableCbtAction(vm, vdi));
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
                        Messages.ACTION_DISABLE_CHANGED_BLOCK_TRACKING,
                        Messages.ACTION_DISABLING_CHANGED_BLOCK_TRACKING,
                        Messages.ACTION_DISABLED_CHANGED_BLOCK_TRACKING,
                        actions).RunAsync();
                }
            }
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            Run(selection.AsXenObjects<VM>());
        }

        private bool CbtLicensed(VM vm)
        {
            return !Helpers.FeatureForbidden(vm.Connection, Host.RestrictChangedBlockTracking);
        }

        private bool CanRun(VM vm)
        {
            return vm != null &&
                !vm.is_a_template &&
                vm.Connection.ResolveAll(vm.VBDs).Any(vbd => vm.Connection.Resolve(vbd.VDI) != null && vm.Connection.Resolve(vbd.VDI).cbt_enabled);
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            // Can run criteria: A selection of VMs in the same pool which has CBT feature licensed, where at least one VM having CBT enabled
            return selection.AllItemsAre<VM>(CbtLicensed) &&
                selection.GetConnectionOfAllItems() != null &&
                selection.AtLeastOneXenObjectCan<VM>(CanRun);
        }

        public override string MenuText => Messages.MAINWINDOW_DISABLE_CHANGED_BLOCK_TRACKING;

        protected override bool ConfirmationRequired => true;

        protected override string ConfirmationDialogText => GetSelection().Count == 1
            ? string.Format(Messages.CONFIRM_DISABLE_CBT_VM, BrandManager.BrandConsole)
            : string.Format(Messages.CONFIRM_DISABLE_CBT_VMS, BrandManager.BrandConsole);

        protected override string ConfirmationDialogTitle => GetSelection().Count == 1
                ? String.Format(Messages.CONFIRM_DISABLE_CBT_VM_TITLE, GetSelection().AsXenObjects<VM>()[0].Name())
                : Messages.CONFIRM_DISABLE_CBT_VMS_TITLE;

        protected override string ConfirmationDialogHelpId => "WarningVmDisableChangedBlockTracking";
    }
}
