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
using XenAdmin.Actions;


namespace XenAdmin.Commands
{
    internal class RemoveStorageLinkVolumeCommand : Command
    {
        private readonly StorageLinkRepository _slr;

        public RemoveStorageLinkVolumeCommand(IMainWindow mainWindow, StorageLinkRepository slr, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
            _slr = slr;
        }

        public RemoveStorageLinkVolumeCommand(IMainWindow mainWindow, StorageLinkRepository slr, VDI vdi)
            : base(mainWindow, vdi)
        {
            _slr = slr;
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (_slr != null && selection.ContainsOneItemOfType<VDI>())
            {
                VDI vdi = (VDI)selection[0].XenObject;
                var deleteCommand = new DeleteVirtualDiskCommand(MainWindowCommandInterface, vdi);
                
                return vdi.sm_config.ContainsKey("SVID") && !string.IsNullOrEmpty(vdi.sm_config["SVID"]) && deleteCommand.CanExecute();
            }
            return false;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var vdi = (VDI)selection[0].XenObject;
            string svid = vdi.sm_config["SVID"];
            var storageLinkVolume = vdi.StorageLinkVolume(Program.StorageLinkConnections.GetCopy());
            string volumeName = storageLinkVolume==null? "":storageLinkVolume.Name;

            var action = new DelegatedAsyncAction(
                vdi.Connection,
                string.Format(Messages.REMOVE_STORAGELINK_VOLUME_ACTION_TITLE, volumeName, _slr),
                string.Format(Messages.REMOVE_STORAGELINK_VOLUME_ACTION_START, volumeName, _slr),
                string.Format(Messages.REMOVE_STORAGELINK_VOLUME_ACTION_FINSH, volumeName, _slr), 
                s => _slr.StorageLinkConnection.RemoveStorageVolumesFromStorageRepository(_slr, new[] { svid }));
            
            action.AppliesTo.Add(vdi.opaque_ref);
            action.AppliesTo.Add(svid);
            action.AppliesTo.Add(_slr.opaque_ref);

            SR sr = _slr.SR(ConnectionsManager.XenConnectionsCopy);

            if(sr != null)
            {
                action.AppliesTo.Add(sr.opaque_ref);
            }

            action.RunAsync();
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
                var vdi = (VDI)GetSelection()[0].XenObject;
                var storageLinkVolume = vdi.StorageLinkVolume(Program.StorageLinkConnections.GetCopy());
                var volumeName = storageLinkVolume == null ? "" : storageLinkVolume.Name;
                return string.Format(Messages.REMOVE_STORAGELINK_VOLUME_MESSAGEBOX_TEXT, volumeName);
            }
        }

        protected override string ConfirmationDialogTitle
        {
            get
            {
                return Messages.REMOVE_STORAGELINK_VOLUME_MESSAGEBOX_TITLE;
            }
        }
    }
}
