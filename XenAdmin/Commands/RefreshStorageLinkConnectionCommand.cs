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
using XenAdmin.Network.StorageLink;
using XenAdmin.Actions;
using System.Threading;


namespace XenAdmin.Commands
{
    internal class RefreshStorageLinkConnectionCommand : Command
    {
        public RefreshStorageLinkConnectionCommand()
        {
        }

        public RefreshStorageLinkConnectionCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            if (selection.AllItemsAre<IStorageLinkObject>())
            {
                var connections = new List<StorageLinkConnection>();

                foreach (IStorageLinkObject s in selection.AsXenObjects<IStorageLinkObject>())
                {
                    if (s.StorageLinkConnection.ConnectionState != StorageLinkConnectionState.Connected)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var connections = new List<StorageLinkConnection>();

            foreach (IStorageLinkObject s in selection.AsXenObjects<IStorageLinkObject>())
            {
                if (!connections.Contains(s.StorageLinkConnection))
                {
                    connections.Add(s.StorageLinkConnection);
                }
            }

            foreach (StorageLinkConnection c in connections)
            {
                string title = string.Format(Messages.REFRESH_STORAGELINK_SYSTEMS_ACTION_TITLE, c.Host);
                string startDes = Messages.REFRESH_STORAGELINK_SYSTEMS_ACTION_START;
                string endDes = Messages.REFRESH_STORAGELINK_SYSTEMS_ACTION_END;

                var action = new DelegatedAsyncAction(null, title, startDes, endDes, s =>
                    {
                        c.Refresh();

                        // wait for refresh to finish so the user can see that
                        // the refresh is still in progress in the logs tab.
                        for (int i = 0; i < 60 && c.RefreshInProgress; i++)
                        {
                            Thread.Sleep(500);
                        }
                    });


                foreach (IStorageLinkObject s in selection.AsXenObjects<IStorageLinkObject>())
                {
                    if (s.StorageLinkConnection == c)
                    {
                        action.AppliesTo.Add(s.opaque_ref);
                    }
                }

                action.RunAsync();
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.REFRESH_STORAGELINK_SYSTEMS;
            }
        }
    }
}
