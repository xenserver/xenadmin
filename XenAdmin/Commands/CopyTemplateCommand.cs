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

using System.Collections.Generic;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Actions.VMActions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the Copy-Template dialog for the specified VM.
    /// </summary>
    internal class CopyTemplateCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public CopyTemplateCommand()
        {
        }

        public CopyTemplateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        private bool CheckRbacPermissions(VM vm, RbacMethodList methodList, string warningMessage)
        {
            if (vm.Connection.Session.IsLocalSuperuser)
                return true;

            var currentRoles = vm.Connection.Session.Roles;
            var validRoles = Role.ValidRoleList(methodList, vm.Connection);

            if (currentRoles.Any(currentRole => validRoles.Contains(currentRole)))
                return true;

            currentRoles.Sort();

            using (var dlg = new ErrorDialog(string.Format(warningMessage, currentRoles[0].FriendlyName())))
                dlg.ShowDialog(Parent);

            return false;
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            var template = (VM)selection[0].XenObject;

            if (CrossPoolCopyTemplateCommand.CanRun(template, null))
            {
                new CrossPoolCopyTemplateCommand(MainWindowCommandInterface, selection).Run();
            }
            else
            {
                var rbac = new RbacMethodList();
                rbac.AddRange(SrRefreshAction.StaticRBACDependencies);
                rbac.AddRange(VMCopyAction.StaticRBACDependencies);
                rbac.AddRange(VMCloneAction.StaticRBACDependencies);

                if (CheckRbacPermissions(template, rbac, Messages.RBAC_INTRA_POOL_COPY_TEMPLATE_BLOCKED))
                    new CopyVMDialog(template).ShowPerXenObject(template, Program.MainWindow);
            }
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return selection.ContainsOneItemOfType<VM>() && selection.AtLeastOneXenObjectCan<VM>(CanRun);
        }

        private static bool CanRun(VM vm)
        {
            if (vm != null && vm.is_a_template && !vm.is_a_snapshot && !vm.Locked && vm.allowed_operations != null && !vm.InternalTemplate())
            {
                if (CrossPoolCopyTemplateCommand.CanRun(vm, null))
                    return true;
                if (vm.allowed_operations.Contains(vm_operations.clone) || vm.allowed_operations.Contains(vm_operations.copy))
                    return true;
            }
            return false;
        }

        public override string MenuText => Messages.MAINWINDOW_COPY_TEMPLATE;
    }
}
