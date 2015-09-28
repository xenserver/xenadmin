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

using System;
using System.Collections.Generic;
using System.Text;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using System.Collections.ObjectModel;
using XenAdmin.Network;
using System.Drawing;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Installs XenServer Tools on the selected VM.
    /// </summary>
    internal class RunXenPrepCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public RunXenPrepCommand()
        {
        }

        public RunXenPrepCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public RunXenPrepCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {

        }

        /// <summary>
        /// Runs XenPrep on a single VM
        /// </summary>
        /// <returns>The <see cref="AsyncAction"/> for the Command.</returns>
        public AsyncAction ExecuteGetAction()
        {
            SelectedItemCollection selection = GetSelection();

            if (selection.Count > 1)
            {
                throw new InvalidOperationException("This method can only be used with a single VM selected.");
            }

            if (selection.ContainsOneItemOfType<VM>())
            {
                VM vm = (VM)selection[0].XenObject;

                if (CanExecute(vm))
                {
                    return SingleVMExecute(vm);
                }
            }

            return null;
        }

        /// <summary>
        /// Attempts to run XenPrep on a VM
        /// </summary>
        /// <param name="vm"></param>
        private AsyncAction SingleVMExecute(VM vm)
        {
            if (vm.FindVMCDROM() == null)
            {
                if (new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            null,
                            Messages.NEW_DVD_DRIVE_REQUIRED_XENPREP,
                            Messages.XENCENTER),
                        ThreeButtonDialog.ButtonYes,
                        ThreeButtonDialog.ButtonNo).ShowDialog(Parent) == DialogResult.Yes)
                {
                    CreateCdDriveAction createDriveAction = new CreateCdDriveAction(vm, true,NewDiskDialog.ShowMustRebootBoxCD,NewDiskDialog.ShowVBDWarningBox);
                    new ActionProgressDialog(createDriveAction, ProgressBarStyle.Marquee).ShowDialog(Parent);

                    if (createDriveAction.Succeeded)
                    {
                        ShowMustRebootBox();
                    }
                    return createDriveAction;
                }
            }
            else
            {
                DialogResult dr = new RunXenPrepWarningDialog(vm.Connection).ShowDialog(Parent);
                if (dr == DialogResult.Yes)
                {
                    var runXenPrepAction = new RunXenPrepAction(vm);
                    //runXenPrepAction.Completed += 

                    runXenPrepAction.RunAsync();
                    return runXenPrepAction;
                }
            }
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vms"></param>
        /// <returns>Whether the action was launched (i.e., the user didn't Cancel)</returns>
        private bool MultipleVMExecute(List<VM> vms)
        {
            throw new NotImplementedException();
        }

        private void ShowMustRebootBox()
        {
            if (!MainWindowCommandInterface.RunInAutomatedTestMode)
            {
                new ThreeButtonDialog(
                       new ThreeButtonDialog.Details(
                           SystemIcons.Information,
                           Messages.NEW_DVD_DRIVE_REBOOT_XENPREP,
                           Messages.NEW_DVD_DRIVE_CREATED)).ShowDialog(Parent);
            }
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<VM> vms = selection.AsXenObjects<VM>(CanExecute);

            if (vms.Count == 1)
                SingleVMExecute(vms[0]);
            else
                MultipleVMExecute(vms);
        }

        public bool ConfirmAndExecute()
        {
            List<VM> vms = GetSelection().AsXenObjects<VM>(CanExecute);

            if (vms.Count == 0)
                return true;

            if (vms.Count == 1)
                return (SingleVMExecute(vms[0]) != null);
            
            return MultipleVMExecute(vms);
        }

        public static bool CanExecute(VM vm)
        {
            return vm != null && !vm.is_a_template && !vm.Locked &&
                vm.GetVirtualisationStatus != VM.VirtualisationStatus.UNKNOWN &&
                
                vm.power_state == vm_power_state.Running;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<VM>() && selection.AtLeastOneXenObjectCan<VM>(CanExecute);
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_RUN_XENPREP;
            }
        }
    }
}
