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
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;
using System.Windows.Forms;
using XenAdmin.Dialogs;
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
            : this(mainWindow, vms.Select(v => new SelectedItem(v)).ToList())
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
            Parent = parent;
        }

        public InstallToolsCommand(IMainWindow mainWindow, VM vm)
            : this(mainWindow, vm, null)
        {
        }

        private void InstallToolsOnOneVm(VM vm)
        {
            if (vm.FindVMCDROM() == null)
            {
                DialogResult dialogResult;
                using (var dlg = new NoIconDialog(string.Format(Messages.NEW_DVD_DRIVE_REQUIRED, BrandManager.VmTools),
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

            using (var dlg = new WarningDialog(string.Format(Messages.XS_TOOLS_MESSAGE_ONE_VM,
                    BrandManager.BrandConsole, BrandManager.VmTools),
                new ThreeButtonDialog.TBDButton(string.Format(Messages.INSTALL_XENSERVER_TOOLS_BUTTON, BrandManager.VmTools),
                    DialogResult.OK, ThreeButtonDialog.ButtonType.ACCEPT, true),
                ThreeButtonDialog.ButtonCancel)
            {
                ShowLinkLabel = true,
                LinkText = string.Format(Messages.INSTALLTOOLS_READ_MORE, BrandManager.VmTools),
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

        private void InstallToolsOnManyVms(List<VM> vms)
        {
            bool newDvdDrivesRequired = false;
            foreach (VM vm in vms)
            {
                if (CanRun(vm) && vm.FindVMCDROM() == null)
                {
                    newDvdDrivesRequired = true;
                    break;
                }
            }

            if (newDvdDrivesRequired)
            {
                DialogResult dialogResult;
                using (var dlg = new WarningDialog(string.Format(Messages.NEW_DVD_DRIVES_REQUIRED, BrandManager.VmTools),
                    ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo))
                {
                    dialogResult = dlg.ShowDialog(Parent);
                }
                if (dialogResult == DialogResult.Yes)
                {
                    foreach (VM vm in vms)
                    {
                        if (CanRun(vm) && vm.FindVMCDROM() == null)
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
                using (var dlg = new WarningDialog(string.Format(Messages.XS_TOOLS_MESSAGE_MORE_THAN_ONE_VM, BrandManager.VmTools),
                    new ThreeButtonDialog.TBDButton(string.Format(Messages.INSTALL_XENSERVER_TOOLS_BUTTON, BrandManager.VmTools),
                        DialogResult.OK, ThreeButtonDialog.ButtonType.ACCEPT, true),
                    ThreeButtonDialog.ButtonCancel)
                {
                    ShowLinkLabel = true,
                    LinkText = string.Format(Messages.INSTALLTOOLS_READ_MORE, BrandManager.VmTools),
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
                        using (var dlg = new WarningDialog(string.Format(Messages.BROKEN_TOOLS_PROMPT, BrandManager.BrandConsole),
                                ThreeButtonDialog.ButtonOK, ThreeButtonDialog.ButtonCancel)
                            {WindowTitle = string.Format(Messages.INSTALL_XENSERVER_TOOLS, BrandManager.VmTools)})
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
                using (var dlg = new InformationDialog(string.Format(Messages.NEW_DVD_DRIVE_REBOOT_TOOLS, BrandManager.VmTools)))
                    dlg.ShowDialog(Parent);
            }
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            List<VM> vms = selection.AsXenObjects<VM>(CanRun);

            if (vms.Count == 1)
                InstallToolsOnOneVm(vms[0]);
            else
                InstallToolsOnManyVms(vms);
        }

        public static bool CanRun(VM vm)
        {
            if (vm == null || vm.is_a_template || vm.Locked || vm.power_state != vm_power_state.Running)
                return false;

            var vStatus = vm.GetVirtualisationStatus(out _);

            if (vStatus.HasFlag(VM.VirtualisationStatus.UNKNOWN) ||
                vStatus.HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED) && vStatus.HasFlag(VM.VirtualisationStatus.MANAGEMENT_INSTALLED))
                return false;

            var vmHome = vm.Home();
            if (vmHome != null && Helpers.IsOlderThanCoordinator(vmHome))
                return false;

            //whether RBAC allows connection to the VM's console
            return vm.Connection.Session != null &&
                   Role.CanPerform(new RbacMethodList("http/connect_console"), vm.Connection, out _);
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return selection.Count > 0 &&
                   selection.All(v => v.XenObject is VM vm &&
                                      !Helpers.StockholmOrGreater(vm.Connection) &&
                                      CanRun(vm));
        }

        public override string MenuText => string.Format(Messages.MAINWINDOW_INSTALL_TOOLS, BrandManager.VmTools);
    }
}
