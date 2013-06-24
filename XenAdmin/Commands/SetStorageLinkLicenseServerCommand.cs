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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAdmin.Network.StorageLink;
using XenAPI;


namespace XenAdmin.Commands
{
    internal class SetStorageLinkLicenseServerCommand : Command
    {
        public SetStorageLinkLicenseServerCommand()
        {
        }

        public SetStorageLinkLicenseServerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            // you can only remove a storagelink server from XC if there isn't an SR using it.

            List<StorageLinkConnection> slCons = new List<StorageLinkConnection>();

            if (selection.AllItemsAre<IStorageLinkObject>())
            {
                foreach (IStorageLinkObject s in selection.AsXenObjects())
                {
                    if (!slCons.Contains(s.StorageLinkConnection))
                    {
                        slCons.Add(s.StorageLinkConnection);
                    }
                }
            }
            return slCons.Count == 1;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            IStorageLinkObject sl = (IStorageLinkObject)selection[0].XenObject;
            StorageLinkServer server = sl.StorageLinkConnection.Cache.Server;

            if (server != null)
            {
                string address = string.Empty;
                int port = 27000;
                var getLicAction = new DelegatedAsyncAction(null,
                    string.Format(Messages.SET_STORAGELINK_LICENSE_SERVER_ACTION_TITLE, server.FriendlyName),
                    string.Format(Messages.SET_STORAGELINK_LICENSE_SERVER_ACTION_START, server.FriendlyName),
                    string.Format(Messages.SET_STORAGELINK_LICENSE_SERVER_ACTION_TITLE, server.FriendlyName),
                    session => server.StorageLinkConnection.GetLicenseServer(out address, out port));
                new ActionProgressDialog(getLicAction, ProgressBarStyle.Marquee).ShowDialog(Parent ?? Program.MainWindow);

                if (!getLicAction.Succeeded)
                {
                    address = string.Empty;
                    port = 27000;
                }

                var dialog = new SetStorageLinkLicenseServerDialog(address, port);

                dialog.FormClosing += (s, e) =>
                    {
                        if (dialog.DialogResult == DialogResult.OK)
                        {
                            address = dialog.Host;
                            port = dialog.Port;

                            var action = new DelegatedAsyncAction(null,
                                string.Format(Messages.SET_STORAGELINK_LICENSE_SERVER_ACTION_TITLE, server.FriendlyName),
                                string.Format(Messages.SET_STORAGELINK_LICENSE_SERVER_ACTION_START, server.FriendlyName),
                                string.Format(Messages.SET_STORAGELINK_LICENSE_SERVER_ACTION_TITLE, server.FriendlyName),
                                session => server.StorageLinkConnection.SetLicenseServer(address, port));

                            action.AppliesTo.Add(server.opaque_ref);

                            new ActionProgressDialog(action, ProgressBarStyle.Marquee).ShowDialog(Parent);
                            e.Cancel = !action.Succeeded;
                        }
                    };

                dialog.Show(Parent ?? Program.MainWindow);
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_SET_STORAGELINK_LICENSE_SERVER;
            }
        }
    }
}
