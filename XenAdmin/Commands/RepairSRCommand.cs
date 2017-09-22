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
using System.Text;
using XenAdmin.Core;
using XenAPI;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAdmin.Properties;
using System.Drawing;
using System.Collections.ObjectModel;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the Repair SR dialog for the selected SR.
    /// </summary>
    internal class RepairSRCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public RepairSRCommand()
        {
        }

        public RepairSRCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public RepairSRCommand(IMainWindow mainWindow, SR selection)
            : base(mainWindow, selection)
        {
        }

        public RepairSRCommand(IMainWindow mainWindow, IEnumerable<SR> selection)
            : base(mainWindow, ConvertToSelection<SR>(selection))
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<SR> srList = selection.AsXenObjects<SR>(CanExecute);

            if (srList.Find(s => !s.MultipathAOK()) != null)
            {
                using (var dlg = new ThreeButtonDialog(
                               new ThreeButtonDialog.Details(
                                   SystemIcons.Warning,
                                   Messages.MULTIPATH_FAILED,
                                   Messages.MULTIPATHING)))
                {
                    dlg.ShowDialog(Parent);
                }
            }

            new RepairSRDialog(srList).Show(Parent);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<SR>() && selection.AtLeastOneXenObjectCan<SR>(CanExecute);
        }

        private bool CanExecute(SR sr)
        {
            return sr != null && sr.HasPBDs() && (sr.IsBroken() || !sr.MultipathAOK()) && !HelpersGUI.GetActionInProgress(sr) && sr.CanRepairAfterUpgradeFromLegacySL();
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._000_StorageBroken_h32bit_16;
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_REPAIR_SR;
            }
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.MAINWINDOW_REPAIR_SR_CONTEXT_MENU;
            }
        }
    }
}
