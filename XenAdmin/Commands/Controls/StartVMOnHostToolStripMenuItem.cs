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
using System.Linq;
using System.Text;
using System.ComponentModel;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Commands
{
    internal class StartVMOnHostToolStripMenuItem : VMOperationToolStripMenuItem
    {
        public StartVMOnHostToolStripMenuItem()
            : base(new StartVMOnHostCommand2(), false, vm_operations.start_on)
        {
        }

        public StartVMOnHostToolStripMenuItem(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, bool inContextMenu)
            : base(new StartVMOnHostCommand2(mainWindow, selection), inContextMenu, vm_operations.start_on)
        {
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Command Command
        {
            get
            {
                return base.Command;
            }
        }

        private class StartVMOnHostCommand2 : Command
        {
            public StartVMOnHostCommand2()
            {
            }

            public StartVMOnHostCommand2(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
                : base(mainWindow, selection)
            {
            }

            protected override bool CanExecuteCore(SelectedItemCollection selection)
            {
                return selection.Count > 0 && selection.AllItemsAre<VM>()
                       && selection.GetConnectionOfAllItems() != null && selection.Any(CanExecute);
            }

            private bool CanExecute(SelectedItem selection)
            {
                VM vm = selection.XenObject as VM;

                if (vm != null && !vm.is_a_template && !vm.Locked)
                {
                    if (vm.allowed_operations != null && vm.allowed_operations.Contains(XenAPI.vm_operations.start)
                        && Helpers.EnabledTargetExists(selection.HostAncestor, selection.Connection))
                    {
                        return selection.Connection != null && selection.Connection.Cache.HostCount > 1;
                    }
                }
                return false;
            }

            public override string MenuText
            {
                get
                {
                    return Messages.MAINWINDOW_START_ON_HOST;
                }
            }
        }
    }

}
