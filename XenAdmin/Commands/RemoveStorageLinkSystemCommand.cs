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

using XenAdmin.Actions;
using XenAPI;
using System.Collections.Generic;
using XenAdmin.Dialogs;
using System.Diagnostics;
using XenAdmin.Network;
using System.Windows.Forms;


namespace XenAdmin.Commands
{
    internal class RemoveStorageLinkSystemCommand : Command
    {
        public RemoveStorageLinkSystemCommand()
        {
        }

        public RemoveStorageLinkSystemCommand(IMainWindow mainWindow, SelectedItemCollection selection)
            : base(mainWindow, selection)
        {
        }

        public RemoveStorageLinkSystemCommand(IMainWindow mainWindow, StorageLinkSystem system)
            : base(mainWindow, system)
        {
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.AllItemsAre<StorageLinkSystem>();
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            // find SRs that are using the storage-systems. First look in XC.

            var srsInUse = new List<IXenObject>();

            foreach (StorageLinkSystem system in GetSelection().AsXenObjects())
            {
                foreach (IXenConnection connection in ConnectionsManager.XenConnections.FindAll(c => c.IsConnected))
                {
                    foreach (SR sr in connection.Cache.SRs)
                    {
                        foreach (XenRef<PBD> pbdRef in sr.PBDs)
                        {
                            PBD pbd = sr.Connection.Resolve<PBD>(pbdRef);

                            if (pbd != null && pbd.device_config.ContainsKey("storageSystemId") && pbd.device_config["storageSystemId"] == system.opaque_ref)
                            {
                                srsInUse.Add(sr);
                                break;
                            }
                        }
                    }
                }
            }

            string title = GetSelection().Count == 1 ? Messages.MAINWINDOW_CONFIRM_REMOVE_STORAGE_SYSTEM_TITLE : Messages.MAINWINDOW_CONFIRM_REMOVE_STORAGE_SYSTEMS_TITLE;

            if (srsInUse.Count == 0)
            {
                // now check for SRs using the the storage-systems in SL.

                DelegatedAsyncAction scanAction = null;
                scanAction = new DelegatedAsyncAction(null, ConfirmationDialogTitle, "", "", s =>
                {
                    foreach (StorageLinkSystem system in GetSelection().AsXenObjects())
                    {
                        scanAction.Title = string.Format(Messages.STORAGELINK_SCANNING_FOR_SRS, system);
                        scanAction.Description = string.Format(Messages.STORAGELINK_SCANNING_FOR_SRS, system);

                        var systemSRs = system.StorageLinkConnection.FullSRRescan().FindAll(ss => ss.StorageLinkSystemId == system.StorageSystemId);

                        srsInUse.AddRange(systemSRs.ConvertAll(slr => (IXenObject)slr));
                    }
                }, true);

                scanAction.AppliesTo.AddRange(GetSelection().AsXenObjects().ConvertAll(s => s.opaque_ref));

                new ActionProgressDialog(scanAction, ProgressBarStyle.Marquee).ShowDialog();

                if (!scanAction.Succeeded)
                {
                    // scan failed. A message will have been displayed.
                    return;
                }
            }

            if (srsInUse.Count > 0)
            {
                // show confirmation dialog.

                string text = Messages.MAINWINDOW_CONFIRM_REMOVE_STORAGE_SYSTEMS_TEXT;

                if (GetSelection().Count == 1)
                {
                    text = string.Format(Messages.MAINWINDOW_CONFIRM_REMOVE_STORAGE_SYSTEM_TEXT, GetSelection()[0].XenObject);
                }
                    
                var reasons = new Dictionary<SelectedItem, string>();

                foreach (IXenObject x in srsInUse)
                {
                    reasons[new SelectedItem(x)] = Messages.STORAGELINK_IN_USE;
                }

                var dialog = new CommandErrorDialog(title, text, reasons, CommandErrorDialog.DialogMode.OKCancel);

                if (dialog.ShowDialog(Parent) != DialogResult.OK)
                {
                    return;
                }
            }

            // remove storage-system
            var actions = new List<AsyncAction>();

            foreach (StorageLinkSystem system in GetSelection().AsXenObjects())
            {
                actions.Add(new RemoveStorageLinkSystemAction(system));
            }

            RunMultipleActions(actions,
                Messages.REMOVE_STORAGE_LINK_SYSTEMS_ACTION_TITLE,
                Messages.REMOVE_STORAGE_LINK_SYSTEMS_ACTION_START_DESCRIPTION,
                Messages.REMOVE_STORAGE_LINK_SYSTEMS_ACTION_END_DESCRIPTION, true);
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.REMOVE;
            }
        }

        public override string MenuText
        {
            get
            {
                if (GetSelection().Count > 1)
                {
                    return Messages.REMOVE_STORAGE_SYSTEMS;
                }

                return Messages.REMOVE_STORAGE_SYSTEM;
            }
        }
    }
}
