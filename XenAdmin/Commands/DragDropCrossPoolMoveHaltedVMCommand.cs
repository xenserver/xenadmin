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
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Core;
using System.Windows.Forms;
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    internal class DragDropCrossPoolMoveHaltedVMCommand : DragDropCommand
    {
        public DragDropCrossPoolMoveHaltedVMCommand(IMainWindow mainWindow, VirtualTreeNode targetNode, IDataObject dragData)
            : base(mainWindow, targetNode, dragData)
        {

        }

        public override string StatusBarText
        {
            get
            {
                var targetHost = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Host>();
                if (targetHost == null)
                    return null;

                var draggedVMs = GetDraggedItemsAsXenObjects<VM>();

                foreach (VM vm in draggedVMs)
                {
                    if (vm == null)
                        continue;

                    if (!LiveMigrateAllowedInVersion(targetHost, vm))
                    {
                        if (IsACrossPoolMigrate(targetHost, vm))
                            return string.Format(Messages.MIGRATION_NOT_ALLOWED_OUTSIDE_POOL, vm.Name().Ellipsise(50));

                        if (vm.GetStorageHost(true) != null) //Non-agile
                            return string.Format(Messages.MIGRATION_NOT_ALLOWED_NO_SHARED_STORAGE, vm.Name().Ellipsise(50));
                    }

                    var homeHost = vm.Home();

                    if (Helpers.ProductVersionCompare(Helpers.HostProductVersion(targetHost),
                            Helpers.HostProductVersion(homeHost ?? Helpers.GetCoordinator(vm.Connection))) < 0)
                        return Messages.OLDER_THAN_CURRENT_SERVER;

                    if (targetHost != homeHost && VMOperationHostCommand.VmCpuIncompatibleWithHost(targetHost, vm))
                        return string.Format(Messages.MIGRATION_NOT_ALLOWED_CPU_FEATURES, vm.Name().Ellipsise(50));
                }

                return null;
            }
        }

        private bool LiveMigrateAllowedInVersion(Host targetHost, VM draggedVM)
        {
            return !Helpers.FeatureForbidden(targetHost.Connection, Host.RestrictCrossPoolMigrate) &&
                   !Helpers.FeatureForbidden(draggedVM.Connection, Host.RestrictCrossPoolMigrate);
        }

        private bool IsACrossPoolMigrate(Host targetHost, VM draggedVM )
        {
            Pool draggedVMPool = Helpers.GetPool(draggedVM.Connection);
            Pool targetPool = targetHost == null ? null : Helpers.GetPool(targetHost.Connection);
            return targetHost != null && (targetPool == null || draggedVMPool == null || targetPool.opaque_ref != draggedVMPool.opaque_ref);
        }

        protected override bool CanRunCore()
        {
            Host targetHost = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Host>();
            if (targetHost == null || !targetHost.IsLive() || !targetHost.Connection.IsConnected)
                return false;

            List<VM> draggedVMs = GetDraggedItemsAsXenObjects<VM>();
            if (draggedVMs.Count <= 0)
                return false;

            foreach (VM vm in draggedVMs)
            {
                if (vm == null || vm.is_a_template || vm.Locked || !(vm.power_state == vm_power_state.Halted || vm.power_state == vm_power_state.Suspended))
                    return false;

                var homeHost = vm.Home();
                if (homeHost != null && homeHost == targetHost)
                    return false;

                if (vm.allowed_operations == null || !vm.allowed_operations.Contains(vm_operations.migrate_send))
                    return false;

                if (Helpers.ProductVersionCompare(Helpers.HostProductVersion(targetHost), Helpers.HostProductVersion(homeHost ?? Helpers.GetCoordinator(vm.Connection))) < 0)
                    return false;

                if (VMOperationHostCommand.VmCpuIncompatibleWithHost(targetHost, vm))
                    return false;
            }

            return true;

        }

        protected override void RunCore()
        {
            Host targetHost = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Host>();
            List<VM> draggedVMs = GetDraggedItemsAsXenObjects<VM>();

            foreach (VM draggedVM in draggedVMs)
            {
                //Check if agile vm.
                VBD cd = draggedVM.FindVMCDROM();
                if (cd != null)
                {
                    VDI vdi = cd.Connection.Resolve<VDI>(cd.VDI);
                    if (vdi != null)
                    {
                        SR sr = cd.Connection.Resolve<SR>(vdi.SR);
                        if (sr != null && !sr.shared)
                        {
                            using (var dlg = new WarningDialog(Messages.DRAG_DROP_LOCAL_CD_LOADED)
                                {WindowTitle = Messages.DRAG_DROP_LOCAL_CD_LOADED_TITLE})
                            {
                                dlg.ShowDialog(MainWindowCommandInterface.Form);
                            }
                            return;
                        }
                    }
                }
            }

            if (draggedVMs.TrueForAll(vm => LiveMigrateAllowedInVersion(targetHost, vm)))
            {
                List<SelectedItem> selectedItems = new List<SelectedItem>();
                draggedVMs.ForEach(vm => selectedItems.Add(new SelectedItem(vm)));
                    
                new CrossPoolMoveVMCommand(MainWindowCommandInterface, selectedItems, targetHost)
                    .Run();
            }
        }

        public override VirtualTreeNode HighlightNode => GetTargetNodeAncestor<Host>();
    }
}
