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
using XenAPI;
using XenAdmin.Wizards.CrossPoolMigrateWizard;


namespace XenAdmin.Commands
{
    internal class CrossPoolCopyVMCommand : CrossPoolMigrateCommand
    {
        public CrossPoolCopyVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : this(mainWindow, selection, null)
        { }

        public CrossPoolCopyVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, Host preSelectedHost)
            : base(mainWindow, selection, preSelectedHost)
        {
        }

        public override string  MenuText 
        {
            get { return Messages.MAINWINDOW_COPY_VM; }
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            var con = selection.GetConnectionOfFirstItem();

            MainWindowCommandInterface.ShowPerConnectionWizard(con,
                new CrossPoolMigrateWizard(con, selection, preSelectedHost, WizardMode.Copy));
        }

        protected override bool CanRun(VM vm)
        {
            return CanRun(vm, preSelectedHost);
        }

        public static bool CanRun(VM vm, Host preSelectedHost)
        {
            if (vm == null || vm.is_a_template || vm.Locked || vm.power_state != vm_power_state.Halted)
                return false;

            return CrossPoolMigrateCommand.CanRun(vm, preSelectedHost);
        }
    }

    internal class CrossPoolCopyTemplateCommand : CrossPoolCopyVMCommand
    {
        public CrossPoolCopyTemplateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : this(mainWindow, selection, null)
        { }

        public CrossPoolCopyTemplateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, Host preSelectedHost)
            : base(mainWindow, selection, preSelectedHost)
        {
        }

        public override string MenuText
        {
            get { return Messages.MAINWINDOW_COPY_TEMPLATE; }
        }

        public new static bool CanRun(VM vm, Host preSelectedHost)
        {
            if (vm == null || !vm.is_a_template || vm.DefaultTemplate() || vm.Locked)
                return false;

            return CrossPoolMigrateCommand.CanRun(vm, preSelectedHost);
        }
    }
}
