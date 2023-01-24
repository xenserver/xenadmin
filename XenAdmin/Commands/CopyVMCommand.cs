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

using System.Collections.Generic;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Actions.VMActions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Wizards.CrossPoolMigrateWizard;
using XenAPI;

namespace XenAdmin.Commands
{
    internal abstract class CopyVmTemplateCommandBase : Command
    {
        protected CopyVmTemplateCommandBase()
        {
        }

        protected CopyVmTemplateCommandBase(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return selection.ContainsOneItemOfType<VM>() && selection.AtLeastOneXenObjectCan<VM>(CanRun);
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            var vm = (VM)selection[0].XenObject;

            if (CanLaunchMigrateWizard(vm))
            {
                MainWindowCommandInterface.ShowPerConnectionWizard(vm.Connection,
                    new CrossPoolMigrateWizard(vm.Connection, selection, null, WizardMode.Copy));
                return;
            }

            if (CheckRbacPermissions(vm.Connection))
                new CopyVMDialog(vm).ShowPerXenObject(vm, Program.MainWindow);
        }

        private bool CheckRbacPermissions(IXenConnection connection)
        {
            if (connection.Session.IsLocalSuperuser)
                return true;

            var methodList = new RbacMethodList();
            methodList.AddRange(SrRefreshAction.StaticRBACDependencies);
            methodList.AddRange(VMCopyAction.StaticRBACDependencies);
            methodList.AddRange(VMCloneAction.StaticRBACDependencies);

            var currentRoles = connection.Session.Roles;
            var validRoles = Role.ValidRoleList(methodList, connection);

            if (currentRoles.Any(currentRole => validRoles.Contains(currentRole)))
                return true;

            currentRoles.Sort();

            using (var dlg = new ErrorDialog(string.Format(RbacMessage, currentRoles[0].FriendlyName())))
                dlg.ShowDialog(Parent);

            return false;
        }

        protected abstract string RbacMessage { get; }
        protected abstract bool CanRun(VM vm);
        protected abstract bool CanLaunchMigrateWizard(VM vm);
    }


    internal class CopyVMCommand : CopyVmTemplateCommandBase
    {
        public CopyVMCommand()
        {
        }

        public CopyVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override bool CanRun(VM vm)
        {
            if (vm == null || vm.is_a_template || vm.Locked || vm.allowed_operations == null)
                return false;

            if (CanLaunchMigrateWizard(vm))
                return true;

            return vm.allowed_operations.Contains(vm_operations.export) && vm.power_state != vm_power_state.Suspended;
        }

        protected override bool CanLaunchMigrateWizard(VM vm)
        {
            return vm.power_state == vm_power_state.Halted && CrossPoolMigrateCommand.CanRun(vm, null, out _);
        }

        public override string MenuText => Messages.MAINWINDOW_COPY_VM;
        protected override string RbacMessage => Messages.RBAC_INTRA_POOL_COPY_VM_BLOCKED;
    }


    internal class CopyTemplateCommand : CopyVmTemplateCommandBase
    {
        public CopyTemplateCommand()
        {
        }

        public CopyTemplateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override bool CanRun(VM vm)
        {
            if (vm == null || !vm.is_a_template || vm.is_a_snapshot || vm.Locked || vm.allowed_operations == null || vm.InternalTemplate()) return false;

            if (CanLaunchMigrateWizard(vm))
                return true;
            
            return vm.allowed_operations.Contains(vm_operations.clone) || vm.allowed_operations.Contains(vm_operations.copy);
        }

        protected override bool CanLaunchMigrateWizard(VM vm)
        {
            return !vm.DefaultTemplate() && CrossPoolMigrateCommand.CanRun(vm, null, out _);
        }

        public override string MenuText => Messages.MAINWINDOW_COPY_TEMPLATE;
        protected override string RbacMessage => Messages.RBAC_INTRA_POOL_COPY_TEMPLATE_BLOCKED;
    }
}
