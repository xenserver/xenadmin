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
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Commands
{
    internal class VtpmCommand : Command
    {
        public VtpmCommand()
        {
        }

        public VtpmCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        public VtpmCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return selection.Count == 1 &&
                   selection[0].XenObject is VM vm &&
                   vm.IsRealVm() &&
                   !Helpers.FeatureForbidden(vm, Host.RestrictVtpm) &&
                   Helpers.XapiEqualOrGreater_22_26_0(vm.Connection) &&
                   vm.IsHVM() && vm.IsDefaultBootModeUefi();
        }

        protected override string GetCantRunReasonCore(IXenObject item)
        {
            if (item is VM vm &&
                !Helpers.FeatureForbidden(vm, Host.RestrictVtpm) &&
                Helpers.XapiEqualOrGreater_22_26_0(vm.Connection) &&
                !(vm.IsHVM() && vm.IsDefaultBootModeUefi()))
                return Messages.COMMAND_VTPM_DISABLED_NON_UEFI;

            return base.GetCantRunReasonCore(item);
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            var vm = selection[0].XenObject as VM;
            new VtpmManagementDialog(vm).ShowPerXenObject(vm, MainWindowCommandInterface.Form);
        }

        public override string ContextMenuText => MenuText;

        public override string MenuText => Messages.COMMAND_VTPM_MENU;
    }
}
