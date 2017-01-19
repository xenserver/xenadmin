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
using System.Text;
using XenAPI;
using System.Collections.ObjectModel;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the ExportVMDialog dialog for the selected snapshot.
    /// </summary>
    internal class ExportSnapshotAsTemplateCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ExportSnapshotAsTemplateCommand()
        {
        }

        public ExportSnapshotAsTemplateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ExportSnapshotAsTemplateCommand(IMainWindow mainWindow, VM snapshot)
            : base(mainWindow, new SelectedItem(snapshot, snapshot.Connection, null, null))
        {

        }

        private void Execute(VM snapshot)
        {
            if (snapshot != null)
            {
                new ExportVMCommand(MainWindowCommandInterface, new SelectedItem(snapshot, snapshot.Connection, null, null)).Execute();
            }
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            VM vm = (VM)selection[0].XenObject;
            Execute(vm);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.Count == 1)
            {
                VM vm = selection[0].XenObject as VM;

                return vm != null && vm.is_a_snapshot;
            }
            return false;
        }

        public override string MenuText
        {
            get
            {
                return Messages.EXPORT_SNAPSHOT_AS_TEMPLATE_MENU_ITEM;
            }
        }
    }
}
