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

using System.Collections.Generic;
using XenAdmin.Actions;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Deletes the selected snapshots. Shows a confirmation dialog.
    /// </summary>
    internal class DeleteSnapshotCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public DeleteSnapshotCommand()
        {
        }

        public DeleteSnapshotCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public DeleteSnapshotCommand(IMainWindow mainWindow, SelectedItem selection)
            : base(mainWindow, selection)
        {
        }

        public DeleteSnapshotCommand(IMainWindow mainWindow, VM snapshot)
            : base(mainWindow, snapshot)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (VM vm in selection.AsXenObjects<VM>())
            {
                actions.Add(new VMSnapshotDeleteAction(vm));
            }
            RunMultipleActions(actions, Messages.ACTION_VM_DELETE_SNAPSHOTS_TITLE, Messages.SNAPSHOT_DELETING, Messages.SNAPSHOTS_DELETED, true);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<VM>(v => v != null && v.is_a_snapshot);
        }

        public override string MenuText
        {
            get
            {
                if (GetSelection().Count == 1)
                {
                    return Messages.DELETE_SNAPSHOT_MENU_ITEM;
                }

                return Messages.MAINWINDOW_DELETE_OBJECTS;
            }
        }

        protected override bool ConfirmationRequired
        {
            get
            {
                return true;
            }
        }

        protected override string ConfirmationDialogText
        {
            get
            {
                var snapshots = GetSelection().AsXenObjects<VM>();
                var snapshotsString= snapshots.Count == 1 ? string.Format(Messages.THE_SNAPSHOT, snapshots[0]) : string.Format(Messages.NUMBER_SNAPSHOTS, snapshots.Count);
                return string.Format(Messages.REMOVE_SNAPSHOT_TEXT, snapshotsString);
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                return Messages.XENCENTER;
            }
        }
    }
}
