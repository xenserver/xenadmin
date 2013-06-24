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

using System;
using System.Collections.Generic;
using System.Text;
using XenAPI;
using XenAdmin.Dialogs;
using System.Windows.Forms;
using XenAdmin.Actions;


namespace XenAdmin.Commands
{
    internal class ImportStorageLinkVolumeCommand : Command
    {
        public ImportStorageLinkVolumeCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ImportStorageLinkVolumeCommand(IMainWindow mainWindow, StorageLinkRepository slr)
            : base(mainWindow, slr)
        {
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.ContainsOneItemOfType<SR>(sr => sr.StorageLinkRepository(Program.StorageLinkConnections) != null) ||
                selection.ContainsOneItemOfType<StorageLinkRepository>();
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            StorageLinkRepository slr = selection[0].XenObject as StorageLinkRepository;

            if (slr == null)
            {
                var sr = selection[0].XenObject as SR;
                slr = sr != null ? sr.StorageLinkRepository(Program.StorageLinkConnections) : null;
            }

            if (slr != null)
            {
                var dialog = new ImportStorageLinkVolumeDialog(slr);

                dialog.FormClosing += (s, e) =>
                    {
                        if (dialog.DialogResult == DialogResult.OK)
                        {
                            var volumes = dialog.Volumes;
                            if (volumes.Count > 0)
                            {
                                var action = new DelegatedAsyncAction(null,
                                    string.Format(Messages.IMPORT_STORAGELINK_VOLUME_ACTION_TITLE, slr), 
                                    string.Format(Messages.IMPORT_STORAGELINK_VOLUME_ACTION_START, slr),
                                    string.Format(Messages.IMPORT_STORAGELINK_VOLUME_ACTION_FINSH, slr), 
                                    session => slr.StorageLinkConnection.AddStorageVolumesToStorageRepository(slr, volumes));
                                
                                action.AppliesTo.Add(slr.opaque_ref);
                                action.AppliesTo.Add(slr.StorageLinkConnection.Cache.Server.opaque_ref);
                                action.RunAsync();
                            }
                        }
                    };

                dialog.Show(Parent);
            }
        }
    }
}
