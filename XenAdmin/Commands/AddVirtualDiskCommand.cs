﻿/* Copyright (c) Citrix Systems, Inc. 
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
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the add-virtual-disk dialog for the selected VM or SR.
    /// </summary>
    internal class AddVirtualDiskCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public AddVirtualDiskCommand()
        {
        }

        public AddVirtualDiskCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public AddVirtualDiskCommand(IMainWindow mainWindow, VM vm)
            : base(mainWindow, vm)
        {
        }

        public AddVirtualDiskCommand(IMainWindow mainWindow, SR sr)
            : base(mainWindow, sr)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            if (selection[0].XenObject is VM vm)
            {
                if (vm.VBDs.Count < vm.MaxVBDsAllowed())
                {
                    new NewDiskDialog(vm.Connection, vm).ShowPerXenObject(vm, Program.MainWindow);
                }
                else
                {
                    using (var dlg = new ErrorDialog(FriendlyErrorNames.VBDS_MAX_ALLOWED)
                        {WindowTitle = Messages.DISK_ADD})
                    {
                        dlg.ShowDialog(Program.MainWindow);
                    }
                }
            }
            else if (selection[0].XenObject is SR sr)
            {
                MainWindowCommandInterface.ShowPerConnectionWizard(sr.Connection, new NewDiskDialog(sr.Connection, sr));
            }
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                if (selection[0].XenObject is VM vm)
                    return !vm.is_a_snapshot && !vm.Locked;

                if (selection[0].XenObject is SR sr)
                    return !sr.Locked;
            }

            return false;
        }
    }
}
