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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Network.StorageLink;
using XenAdmin.Properties;
using XenAPI;


namespace XenAdmin.Commands
{
    internal class ChangeStorageLinkPasswordCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ChangeStorageLinkPasswordCommand()
        {
        }

        public ChangeStorageLinkPasswordCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ChangeStorageLinkPasswordCommand(IMainWindow mainWindow, StorageLinkServer server)
            : base(mainWindow, server)
        {
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.AllItemsAre<IStorageLinkObject>())
            {
                StorageLinkConnection con = ((IStorageLinkObject)selection.First).StorageLinkConnection;

                // check all selected SL items are on the same SL connection.
                if (con != null &&
                    con.ConnectionState == StorageLinkConnectionState.Connected &&
                    selection.AllItemsAre<IStorageLinkObject>(s => s.StorageLinkConnection.Equals(con)))
                {
                    return true;
                }
            }
            return false;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var slCon = ((IStorageLinkObject)selection.First).StorageLinkConnection;
            var dialog = new ChangeStorageLinkPasswordDialog(slCon);

            dialog.FormClosing += (s, e) =>
                {
                    if (dialog.DialogResult == DialogResult.OK)
                    {
                        var action = new ChangeStorageLinkPasswordAction(ConnectionsManager.XenConnectionsCopy,slCon, dialog.Username, dialog.OldPassword, dialog.NewPassword);
                        new ActionProgressDialog(action, ProgressBarStyle.Marquee).ShowDialog();
                        e.Cancel = !action.Succeeded;
                    }
                };

            dialog.Show(Program.MainWindow);
        }

        public override string MenuText
        {
            get
            {
                return Messages.CHANGE_SL_PASSWORD;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Resources.change_password_16;
            }
        }
    }
}
