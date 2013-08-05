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
using XenAPI;
using System.Windows.Forms;
using System.Drawing;
using XenAdmin.Properties;
using XenAdmin.Dialogs;
using XenAdmin.Actions;
using XenAdmin.Network.StorageLink;
using System.Threading;

namespace XenAdmin.Commands
{
    internal class AddStorageLinkSystemCommand : Command
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event EventHandler<CompletedEventArgs> Completed;

        public AddStorageLinkSystemCommand()
        {
        }

        public AddStorageLinkSystemCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public AddStorageLinkSystemCommand(IMainWindow mainWindow, StorageLinkServer server)
            : base(mainWindow, server)
        {
        }

        public AddStorageLinkSystemCommand(IMainWindow mainWindow, StorageLinkServer server, Control parent)
            : base(mainWindow, server)
        {
            SetParent(parent);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.AllItemsAre<IStorageLinkObject>())
            {
                StorageLinkConnection con = ((IStorageLinkObject)selection.First).StorageLinkConnection;

                // check all selected SL items are on the same SL connection.
                if (con != null && con.ConnectionState == StorageLinkConnectionState.Connected && selection.AllItemsAre<IStorageLinkObject>(s => s.StorageLinkConnection.Equals(con)))
                {
                    return true;
                }
            }
            return false;

        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            IXenObject selected = selection.FirstAsXenObject;
            var server = GetStorageLinkServer(selected);
            if (server == null)
                return;
            var dialog = new AddStorageLinkSystemDialog(server.StorageLinkConnection);
            var parent = Parent ?? (Control)Program.MainWindow;

            dialog.FormClosing += (s, e) =>
                {
                    if (dialog.DialogResult == DialogResult.OK)
                    {
                        var adapter = dialog.StorageAdapter;
                        var port = dialog.StorageSystemPort;
                        var address = dialog.StorageSystemAddress;
                        var username = dialog.StorageSystemUsername;
                        var password = dialog.StorageSystemPassword;
                        var ns = dialog.StorageSystemNamespace;

                        // There are no RBAC complexities here since only pool-operator and above can access the password for the storagelink service. 
                        // Therefore only pool-operator and above can get here. These roles are permitted to add and remove storage systems.

                        var action = new StorageLinkDelegatedAsyncAction(
                            () => server.StorageLinkConnection.AddStorageSystem(adapter, port, address, username, password, ns),
                            string.Format(Messages.ADD_STORAGE_LINK_SYSTEM_ACTION_TITLE, address),
                            string.Format(Messages.ADD_STORAGE_LINK_SYSTEM_ACTION_START_DESCRIPTION, address),
                            string.Format(Messages.ADD_STORAGE_LINK_SYSTEM_ACTION_END_DESCRIPTION, address));

                        var actionWithWait = new DelegatedAsyncAction(null,
                            string.Format(Messages.ADD_STORAGE_LINK_SYSTEM_ACTION_TITLE, address),
                            string.Format(Messages.ADD_STORAGE_LINK_SYSTEM_ACTION_START_DESCRIPTION, address),
                            string.Format(Messages.ADD_STORAGE_LINK_SYSTEM_ACTION_END_DESCRIPTION, address), ss =>
                            {
                                int storageSystemCountBefore = server.StorageLinkConnection.Cache.StorageSystems.Count;
                                action.RunExternal(ss);

                                for (int i = 0; i < 60 && action.Succeeded && server.StorageLinkConnection.Cache.StorageSystems.Count == storageSystemCountBefore; i++)
                                {
                                    if((i%5)==0)
                                    {
                                        log.Info("Waiting for StorageLink storage-system to be added to cache."); 
                                    }

                                    Thread.Sleep(500);
                                }
                            }, true);

                        actionWithWait.AppliesTo.Add(server.opaque_ref);
                        actionWithWait.Completed += ss => OnCompleted(new CompletedEventArgs(actionWithWait.Succeeded));

                        new ActionProgressDialog(actionWithWait, ProgressBarStyle.Continuous) { ShowCancel = true }.ShowDialog(dialog);

                        // keep creds dialog open if it failed.
                        e.Cancel = !actionWithWait.Succeeded;
                    }
                };

            dialog.Show(parent);
        }

        private StorageLinkServer GetStorageLinkServer(IXenObject xo)
        {
            if (xo is StorageLinkServer)
            {
                return (StorageLinkServer)xo;
            }
            else if (xo is StorageLinkSystem)
            {
                StorageLinkSystem system = (StorageLinkSystem)xo;
                return system.StorageLinkServer;
            }
            else if (xo is StorageLinkPool)
            {
                StorageLinkPool pool = (StorageLinkPool)xo;
                if (pool.Parent == null)
                {
                    return pool.StorageLinkSystem.StorageLinkServer;
                }
                else
                {
                    return GetStorageLinkServer(pool.Parent);
                }
            }
            return null;
        }

        public override Image MenuImage
        {
            get
            {
                return Resources.sl_add_storage_system_16;
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.ADD_STORAGE_LINK_SYSTEM_MENU;
            }
        }

        protected virtual void OnCompleted(CompletedEventArgs e)
        {
            var handler = Completed;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        internal class CompletedEventArgs : EventArgs
        {
            public bool Success { get; private set; }

            public CompletedEventArgs(bool success)
            {
                Success = success;
            }
        }
    }
}
