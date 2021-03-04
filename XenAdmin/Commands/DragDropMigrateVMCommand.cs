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
using System.Linq;
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Core;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAdmin.Actions.VMActions;


namespace XenAdmin.Commands
{
    internal class DragDropMigrateVMCommand : DragDropCommand
    {
        public DragDropMigrateVMCommand(IMainWindow mainWindow, VirtualTreeNode targetNode, IDataObject dragData)
            : base(mainWindow, targetNode, dragData)
        {

        }

        public override string StatusBarText
        {
            get
            {
                if (!CanExecute())
                {
                    Host targetHost = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Host>();
                    
                    if (targetHost != null)
                    {
                        foreach (VM draggedVM in GetDraggedItemsAsXenObjects<VM>())
                        {
                            Pool draggedVMPool = Helpers.GetPool(draggedVM.Connection);
                            Host draggedVMHome = draggedVM.Home();

                            if (draggedVM == null || draggedVMPool == null || 
                                draggedVMHome == null || 
                                !draggedVM.allowed_operations.Contains(vm_operations.migrate_send))
                            {
                                if (draggedVM.power_state == vm_power_state.Running)
                                    return Messages.MIGRATION_NOT_ALLOWED;
                                return null;
                            }

                            if(!LiveMigrateAllowedInVersion(targetHost, draggedVM))
                            {
                                if (IsACrossPoolMigrate(targetHost, draggedVM))
                                {
                                    return Messages.MIGRATION_NOT_ALLOWED_OUTSIDE_POOL;
                                }
                                if (draggedVM.GetStorageHost(true) != null)
                                {
                                    // Non-agile.
                                    return Messages.MIGRATION_NOT_ALLOWED_NO_SHARED_STORAGE;
                                }
                            }

                            if (Helpers.productVersionCompare(Helpers.HostProductVersion(targetHost), Helpers.HostProductVersion(draggedVMHome)) < 0)
                                return Messages.OLDER_THAN_CURRENT_SERVER;

                            if (targetHost != draggedVMHome && VMOperationHostCommand.VmCpuIncompatibleWithHost(targetHost, draggedVM))
                                return Messages.MIGRATION_NOT_ALLOWED_CPU_FEATURES;
                        }
                    }
                }
                return null;
            }
        }

        private bool LiveMigrateAllowedInVersion(Host targetHost, VM draggedVM)
        {
            return !Helpers.FeatureForbidden(targetHost.Connection, Host.RestrictCrossPoolMigrate) &&
                   !Helpers.FeatureForbidden(draggedVM.Connection, Host.RestrictCrossPoolMigrate);
        }

        private bool IsAnIntraPoolMigrate(Host targetHost, VM draggedVM)
        {
            List<SR> draggedSRs = new List<SR>(draggedVM.SRs());
            bool allStorageShared = draggedSRs.TrueForAll(sr => sr.shared);
            return !IsACrossPoolMigrate(targetHost, draggedVM) && !allStorageShared;
        }

        private bool IsACrossPoolMigrate(Host targetHost, VM draggedVM )
        {
            Pool draggedVMPool = Helpers.GetPool(draggedVM.Connection);
            Pool targetPool = targetHost == null ? null : Helpers.GetPool(targetHost.Connection);
            return targetHost != null && (targetPool == null || draggedVMPool == null || targetPool.opaque_ref != draggedVMPool.opaque_ref);
        }

        protected override bool CanExecuteCore()
        {
            Host targetHost = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Host>();


            if (targetHost != null)
            {
                List<VM> draggedVMs = GetDraggedItemsAsXenObjects<VM>();

                if (draggedVMs.Count > 0)
                {
                    foreach (VM draggedVM in draggedVMs)
                    {
                        Host draggedVMHome = draggedVM.Home();

                        if (draggedVM.Connection == null || !draggedVM.Connection.IsConnected)
                        {
                            return false;
                        }

                        if(!LiveMigrateAllowedInVersion(targetHost, draggedVM))
                        {
                            Pool targetPool = targetHost == null ? null : Helpers.GetPool(targetHost.Connection);
                            
                            if(targetPool == null)
                                return false;

                            Pool draggedVMPool = Helpers.GetPool(draggedVM.Connection);
                            
                            if (draggedVMPool == null || draggedVMPool.opaque_ref != targetPool.opaque_ref)
                            {
                                // dragged VM must be in same pool as target
                                return false;
                            }
                            if (draggedVM.GetStorageHost(true) != null)
                            {
                                // dragged VM must be agile
                                return false;
                            }
                        }
                        else
                        {
                            if (Helpers.FeatureForbidden(draggedVM.Connection, Host.RestrictIntraPoolMigrate))
                                return false;
                        }

                        if (draggedVM.allowed_operations == null || !draggedVM.allowed_operations.Contains(vm_operations.pool_migrate))
                        {
                            // migrate not allowed
                            return false;
                        }
                        
                        if (draggedVMHome == null || draggedVMHome == targetHost)
                        {
                            // dragged VM must currently be shown below a host
                            return false;
                        }

                        if (Helpers.productVersionCompare(Helpers.HostProductVersion(targetHost), Helpers.HostProductVersion(draggedVMHome)) < 0)
                            return false;

                        if (VMOperationHostCommand.VmCpuIncompatibleWithHost(targetHost, draggedVM))
                            return false;
                    }

                    return true;
                }
            }
            return false;
        }

        protected override void ExecuteCore()
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

            if( draggedVMs.TrueForAll(vm => LiveMigrateAllowedInVersion(targetHost, vm)))
            {
               if (draggedVMs.Any(vm => IsACrossPoolMigrate(targetHost, vm)) ||
                   draggedVMs.Any(vm => IsAnIntraPoolMigrate(targetHost, vm)))
                {
                    List<SelectedItem> selectedItems = new List<SelectedItem>();
                    draggedVMs.ForEach(vm => selectedItems.Add(new SelectedItem(vm)));
                    new CrossPoolMigrateCommand(MainWindowCommandInterface, selectedItems, targetHost).Execute();
                    return;
                } 
            }
            
            if (Confirm())
            {
                foreach (VM draggedVM in draggedVMs)
                {
                    if (draggedVM.Connection != null && draggedVM.Connection.IsConnected)
                    {
                        new VMMigrateAction(draggedVM, targetHost).RunAsync();
                    }
                }
            }
        }

        private bool Confirm()
        {
            if (Program.RunInAutomatedTestMode)
                return true;

            List<VM> draggedVMs = GetDraggedItemsAsXenObjects<VM>();
            Host targetHost = GetTargetNodeAncestorAsXenObjectOrGroupingTag<Host>();

            var msg = draggedVMs.Count == 1
                ? string.Format(Messages.MAINWINDOW_CONFIRM_MIGRATE, draggedVMs[0].Name().Ellipsise(50), targetHost.Name().Ellipsise(50))
                : string.Format(Messages.MAINWINDOW_CONFIRM_MIGRATE_MULTIPLE, targetHost.Name().Ellipsise(50));

            using (var dialog = new WarningDialog(msg, ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                {WindowTitle = Messages.MESSAGEBOX_CONFIRM})
            {
                return dialog.ShowDialog(Program.MainWindow) == DialogResult.Yes;
            }
        }

        public override VirtualTreeNode HighlightNode => CanExecute() ? GetTargetNodeAncestor<Host>() : null;
    }
}
