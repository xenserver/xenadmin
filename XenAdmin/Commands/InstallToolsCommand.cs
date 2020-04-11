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
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using System.Drawing;
using System.Linq;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Installs XenServer Tools on the selected VM.
    /// </summary>
    internal class InstallToolsCommand : Command
    {
        public event Action<AsyncAction> InstallTools;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public InstallToolsCommand()
        {
        }

        public InstallToolsCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public InstallToolsCommand(IMainWindow mainWindow, IEnumerable<VM> vms)
            : this(mainWindow, ConvertToSelection(vms))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallToolsCommand"/> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="vm">The vm.</param>
        /// <param name="parent">The parent window for any UI.</param>
        public InstallToolsCommand(IMainWindow mainWindow, VM vm, Control parent)
            : base(mainWindow, vm)
        {
            SetParent(parent);
        }

        public InstallToolsCommand(IMainWindow mainWindow, VM vm)
            : this(mainWindow, vm, null)
        {
        }

        /// <summary>
        /// Attempts to install tools on the vm
        /// </summary>
        /// <param name="vm"></param>
        /// <returns>null if user cancels or an AsyncAction. This is either the InstallPVToolsAction or the CreateCdDriveAction if the VM needed a DVD drive.</returns>
        private void SingleVMExecute(VM vm)
        {
            if (vm.FindVMCDROM() == null)
            {
                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(
                            null,
                            Messages.NEW_DVD_DRIVE_REQUIRED,
                            Messages.XENCENTER),
                        ThreeButtonDialog.ButtonYes,
                        ThreeButtonDialog.ButtonNo))
                {
                    dialogResult = dlg.ShowDialog(Parent);
                }
                if (dialogResult == DialogResult.Yes)
                {
                    //do not register the event ShowUserInstruction; we show explicitly a message afterwards
                    var createDriveAction = new CreateCdDriveAction(vm);

                    using (var dlg = new ActionProgressDialog(createDriveAction, ProgressBarStyle.Marquee))
                        dlg.ShowDialog(Parent);

                    if (createDriveAction.Succeeded)
                        ShowMustRebootBox();

                    InstallTools?.Invoke(createDriveAction);
                }
                return;
            }

            using (var dlg = new ThreeButtonDialog(
                new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.XS_TOOLS_MESSAGE_ONE_VM, Messages.XENCENTER),
                new ThreeButtonDialog.TBDButton(Messages.INSTALL_XENSERVER_TOOLS_BUTTON, DialogResult.OK,
                    ThreeButtonDialog.ButtonType.ACCEPT, true), ThreeButtonDialog.ButtonCancel)
            {
                ShowLinkLabel = true,
                LinkText = Messages.INSTALLTOOLS_READ_MORE,
                LinkAction = () => Help.HelpManager.Launch("InstallToolsWarningDialog")
            })
                if (dlg.ShowDialog(Parent) == DialogResult.OK && CheckToolSrs(vm))
                {
                    var installToolsAction = new InstallPVToolsAction(vm, Properties.Settings.Default.ShowHiddenVMs);
                    installToolsAction.Completed += InstallToolsActionCompleted;

                    installToolsAction.RunAsync();
                    InstallTools?.Invoke(installToolsAction);
                }
        }

        /// <summary>
        /// Attempts to install tools on several VMs
        /// </summary>
        /// <param name="vms"></param>
        /// <returns>Whether the action was launched (i.e., the user didn't Cancel)</returns>
        private void MultipleVMExecute(List<VM> vms)
        {
            bool newDvdDrivesRequired = false;
            foreach (VM vm in vms)
            {
                if (CanExecute(vm) && vm.FindVMCDROM() == null)
                {
                    newDvdDrivesRequired = true;
                    break;
                }
            }

            if (newDvdDrivesRequired)
            {
                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.NEW_DVD_DRIVES_REQUIRED, Messages.XENCENTER),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo))
                {
                    dialogResult = dlg.ShowDialog(Parent);
                }
                if (dialogResult == DialogResult.Yes)
                {
                    foreach (VM vm in vms)
                    {
                        if (CanExecute(vm) && vm.FindVMCDROM() == null)
                        {
                            //do not register the event ShowUserInstruction; we show explicitly a message afterwards
                            var createDriveAction = new CreateCdDriveAction(vm);

                            using (var dlg = new ActionProgressDialog(createDriveAction, ProgressBarStyle.Marquee))
                            {
                                dlg.ShowDialog(Parent);
                            }
                        }
                    }
                    ShowMustRebootBox();
                    InstallTools?.Invoke(null);
                }
            }
            else
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.XS_TOOLS_MESSAGE_MORE_THAN_ONE_VM, Messages.XENCENTER),
                    new ThreeButtonDialog.TBDButton(Messages.INSTALL_XENSERVER_TOOLS_BUTTON, DialogResult.OK,
                        ThreeButtonDialog.ButtonType.ACCEPT, true),
                    ThreeButtonDialog.ButtonCancel)
                {
                    ShowLinkLabel = true,
                    LinkText = Messages.INSTALLTOOLS_READ_MORE,
                    LinkAction = () => Help.HelpManager.Launch("InstallToolsWarningDialog")
                })
                    if (dlg.ShowDialog(Parent) == DialogResult.OK && CheckToolSrs(vms.ToArray()))
                    {
                        foreach (VM vm in vms)
                        {
                            var installToolsAction = new InstallPVToolsAction(vm, Properties.Settings.Default.ShowHiddenVMs);

                            if (vms.IndexOf(vm) == 0)
                            {
                                installToolsAction.Completed += FirstInstallToolsActionCompleted;
                            }
                            else
                            {
                                installToolsAction.Completed += InstallToolsActionCompleted;
                            }

                            installToolsAction.RunAsync();
                        }

                        InstallTools?.Invoke(null);
                    }
            }
        }

        private bool CheckToolSrs(params VM[] vms)
        {
            // check all connections to make sure they don't have any broken SRs.
            // If we find one tell the user we are going to fix it.
            foreach (var vm in vms)
            {
                foreach (SR sr in vm.Connection.Cache.SRs)
                {
                    if (sr.IsToolsSR() && sr.IsBroken())
                    {
                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.BROKEN_TOOLS_PROMPT,
                                Messages.INSTALL_XENSERVER_TOOLS),
                            ThreeButtonDialog.ButtonOK,
                            ThreeButtonDialog.ButtonCancel))
                        {
                            var dialogResult = dlg.ShowDialog(Parent);
                            return dialogResult == DialogResult.OK;
                        }
                    }
                }
            }
            return true;
        }

        private void FirstInstallToolsActionCompleted(ActionBase sender)
        {
            InstallPVToolsAction action = (InstallPVToolsAction)sender;

            MainWindowCommandInterface.Invoke(delegate
            {
                // Try to select the VM on which the PV tools are to be installed
                if (MainWindowCommandInterface.SelectObjectInTree(action.VM))
                {
                    // Switch to the VM's console tab
                    MainWindowCommandInterface.SwitchToTab(MainWindow.Tab.Console);
                }
            });
        }

        private void InstallToolsActionCompleted(ActionBase sender)
        {
            InstallPVToolsAction action = (InstallPVToolsAction)sender;
           
            MainWindowCommandInterface.Invoke(delegate
            {
                Program.MainWindow.SelectObjectInTree(action.VM);
                Program.MainWindow.TheTabControl.SelectedTab = Program.MainWindow.TabPageConsole;
            });
        }

        private void ShowMustRebootBox()
        {
            if (!MainWindowCommandInterface.RunInAutomatedTestMode)
            {
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Information, Messages.NEW_DVD_DRIVE_REBOOT_TOOLS)))
                {
                    dlg.ShowDialog(Parent);
                }
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

        public static bool CanExecute(VM vm)
        {
            if (vm == null || vm.is_a_template || vm.Locked || vm.power_state != vm_power_state.Running)
                return false;

            var vStatus = vm.GetVirtualisationStatus();

            if (vStatus.HasFlag(VM.VirtualisationStatus.UNKNOWN) ||
                vStatus.HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED) && vStatus.HasFlag(VM.VirtualisationStatus.MANAGEMENT_INSTALLED))
                return false;

            var vmHome = vm.Home();
            if (vmHome != null && Helpers.IsOlderThanMaster(vmHome))
                return false;

            //whether RBAC allows connection to the VM's console
            return vm.Connection.Session != null &&
                   Role.CanPerform(new RbacMethodList("http/connect_console"), vm.Connection, out _, false);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.Count > 0 &&
                   selection.All(v => v.XenObject is VM vm &&
                                      !Helpers.StockholmOrGreater(vm.Connection) &&
                                      CanExecute(vm));
        }

        public override string MenuText => Messages.MAINWINDOW_INSTALL_TOOLS;
    }
}
