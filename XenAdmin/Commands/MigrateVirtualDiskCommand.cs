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

using System;
using System.Collections.Generic;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Commands
{
    class MigrateVirtualDiskCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public MigrateVirtualDiskCommand()
        {
        }

        public MigrateVirtualDiskCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public override string ContextMenuText
        {
            get { return GetSelection().Count > 1 ? Messages.MAINWINDOW_MOVE_OBJECTS : Messages.MOVE_VDI_CONTEXT_MENU; }
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<VDI> vdis = selection.AsXenObjects<VDI>();

            bool featureForbidden = vdis.TrueForAll(vdi => Helpers.FeatureForbidden(vdi.Connection, Host.RestrictCrossPoolMigrate));
            if (featureForbidden)
            {
                string theText = HiddenFeatures.LinkLabelHidden
                    ? Messages.MIGRATE_VDI_UPSELL_BLURB
                    : Messages.MIGRATE_VDI_UPSELL_BLURB + Messages.UPSELL_BLURB_TRIAL;

                using (var dlg = new UpsellDialog(theText, InvisibleMessages.UPSELL_LEARNMOREURL_TRIAL))
                    dlg.ShowDialog(Parent);
            }
            else
            {
                new MoveVirtualDiskDialog(selection.FirstAsXenObject.Connection, null, vdis).Show(Program.MainWindow);
            }
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.Count > 0 && selection.All(v => CanBeMigrated(v.XenObject as VDI));
        }

        private bool CanBeMigrated(VDI vdi)
        {
            if (vdi == null || vdi.is_a_snapshot || vdi.Locked || vdi.IsHaType() || vdi.cbt_enabled)
                return false;

            if(vdi.Connection.ResolveAll(vdi.VBDs).Count == 0)
                return false;
            if (vdi.GetVMs().Any(vm => !vm.IsRunning()) && !Helpers.DundeeOrGreater(vdi.Connection))
                return false;

            SR sr = vdi.Connection.Resolve(vdi.SR);
            if (sr == null || sr.HBALunPerVDI())
                return false;
            if (Helpers.DundeePlusOrGreater(vdi.Connection) && !sr.allowed_operations.Contains(storage_operations.vdi_mirror))
                return false;

            return true;
        }

        protected override string GetCantExecuteReasonCore(SelectedItem item)
        {
            VDI vdi = item.XenObject as VDI;
            if (vdi == null)
                return base.GetCantExecuteReasonCore(item);

            if (vdi.is_a_snapshot)
                return Messages.CANNOT_MOVE_VDI_IS_SNAPSHOT;
            if (vdi.Locked)
                return Messages.CANNOT_MOVE_VDI_IN_USE;
            if (vdi.IsHaType())
                return Messages.CANNOT_MOVE_HA_VD;
            if (vdi.cbt_enabled)
                return Messages.CANNOT_MOVE_CBT_ENABLED_VDI;
            if (vdi.IsMetadataForDR())
                return Messages.CANNOT_MOVE_DR_VD;
            if (vdi.GetVMs().Any(vm => !vm.IsRunning()) && !Helpers.DundeeOrGreater(vdi.Connection))
                return Messages.CANNOT_MIGRATE_VDI_NON_RUNNING_VM;

            SR sr = vdi.Connection.Resolve(vdi.SR);
            if (sr == null)
                return base.GetCantExecuteReasonCore(item);
            if (sr.HBALunPerVDI())
                return Messages.UNSUPPORTED_SR_TYPE;
            if (Helpers.DundeePlusOrGreater(vdi.Connection) && !sr.allowed_operations.Contains(storage_operations.vdi_mirror))
                return Messages.UNSUPPORTED_SR_TYPE;

            return base.GetCantExecuteReasonCore(item);
        }
    }
}
