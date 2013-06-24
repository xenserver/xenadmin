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

using System.Collections.Generic;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Dialogs;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Upgrades the selected SR.
    /// </summary>
    internal class UpgradeSRCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public UpgradeSRCommand()
        {
        }

        public UpgradeSRCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public UpgradeSRCommand(IMainWindow mainWindow, SR selection)
            : base(mainWindow, selection)
        {
        }

        public UpgradeSRCommand(IMainWindow mainWindow, IEnumerable<SR> selection)
            : base(mainWindow, ConvertToSelection<SR>(selection))
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (SR sr in selection.AsXenObjects<SR>(CanExecute))
            {
                actions.Add(new SrAction(SrActionKind.UpgradeLVM, sr));
            }
            RunMultipleActions(actions, Messages.ACTION_SRS_UPGRADE, Messages.ACTION_SRS_UPGRADING, Messages.ACTION_SRS_UPGRADED, true);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<SR>() && selection.AtLeastOneXenObjectCan<SR>(CanExecute);
        }

        private static bool CanExecute(SR sr)
        {
            return sr.NeedsUpgrading && !HelpersGUI.GetActionInProgress(sr);
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_UPGRADE_SR;
            }
        }

        protected override bool ConfirmationRequired
        {
            get
            {
                return true;
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.UPGRADE_SR_PROMPT_TITLE;
                }
                return Messages.UPGRADE_SRS_PROMPT_TITLE;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.UPGRADE_SR_PROMPT;
                }
                return Messages.UPGRADE_SRS_PROMPT;
            }
        }

        protected override CommandErrorDialog GetErrorDialogCore(IDictionary<SelectedItem, string> cantExecuteReasons)
        {
            return new CommandErrorDialog(Messages.ERROR_DIALOG_UPGRADE_SR_TITLE, Messages.ERROR_DIALOG_UPGRADE_SR_TEXT, cantExecuteReasons);
        }

        protected override string GetCantExecuteReasonCore(SelectedItem item)
        {
            SR sr = item.XenObject as SR;
            if (sr == null)
            {
                return base.GetCantExecuteReasonCore(item);
            }
            if (!sr.NeedsUpgrading)
            {
                return Messages.SR_DOES_NOT_NEED_UPGRADE;
            }
            else if (HelpersGUI.GetActionInProgress(sr))
            {
                return Messages.SR_ACTION_IN_PROGRESS;
            }
            return base.GetCantExecuteReasonCore(item);
        }
    }
}
