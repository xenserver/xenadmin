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

using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Network;
using System.Collections.ObjectModel;


namespace XenAdmin.Commands
{
    internal class MigrateVMToolStripMenuItem : VMOperationToolStripMenuItem
    {

        public MigrateVMToolStripMenuItem()
            : base(new MigrateVMCommand2(), false, vm_operations.pool_migrate)
        {
        }

        public MigrateVMToolStripMenuItem(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, bool inContextMenu)
            : base(new MigrateVMCommand2(mainWindow, selection), inContextMenu, vm_operations.pool_migrate)
        {
        }

        protected override void AddAdditionalMenuItems(SelectedItemCollection selection)
        {
            if (selection.ToList().All(item => Helpers.TampaOrGreater(item.Connection) && !Helpers.CrossPoolMigrationRestrictedWithWlb(item.Connection)))
            {
                VMOperationCommand cmd = new CrossPoolMigrateCommand(Command.MainWindowCommandInterface, selection);
                DropDownItems.Add(new ToolStripSeparator());
                VMOperationToolStripMenuSubItem lastItem = new VMOperationToolStripMenuSubItem(cmd);
                DropDownItems.Add(lastItem); 
            }            
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

        private class MigrateVMCommand2 : Command
        {
            public MigrateVMCommand2()
            {
            }

            public MigrateVMCommand2(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
                : base(mainWindow, selection)
            {
            }

            private static bool CanExecute(VM vm)
            {
                if (vm.Connection != null && vm.Connection.IsConnected)
                    if (Helpers.FeatureForbidden(vm.Connection, Host.RestrictInterPoolMigrate))
                        return false;

                if (vm != null && !vm.is_a_template && !vm.Locked)
                {
                    if(Helpers.TampaOrGreater(vm.Connection))
                        return vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.pool_migrate);

                    return SelectionInVisiblePool(vm.Connection) && vm.Connection.Cache.HostCount > 1 && vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.pool_migrate);
                    
                }
                return false;
            }

            protected override bool CanExecuteCore(SelectedItemCollection selection)
            {
                if (!selection.AllItemsAre<VM>())
                {
                    return false;
                }

                IXenConnection connection = null;

                bool atLeaseOneCanExecute = false;
                foreach (SelectedItem item in selection)
                {
                    VM vm = (VM)item.XenObject;

                    // all VMs must be on the same connection
                    if (connection != null && vm.Connection != connection)
                    {
                        return false;
                    }

                    if (CanExecute(item.XenObject as VM))
                    {
                        atLeaseOneCanExecute = true;
                    }
                }
                return atLeaseOneCanExecute;
            }

            private static bool SelectionInVisiblePool(IXenConnection connection)
            {
                if (connection == null)
                    return false;

                Pool pool = Helpers.GetPoolOfOne(connection);
                if (pool == null)
                    return false;

                return pool!=null&&pool.IsVisible;
            }

            public override string MenuText
            {
                get
                {
                    return Messages.MAINWINDOW_MIGRATE_TO_SERVER;
                }
            }
        }
    }
}
