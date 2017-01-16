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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using XenAdmin.Network;
using XenAdmin.Core;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Properties;


namespace XenAdmin.Commands
{
    internal class AddHostToSelectedPoolToolStripMenuItem : CommandToolStripMenuItem
    {
        public AddHostToSelectedPoolToolStripMenuItem()
            : base(new AddHostToSelectedPoolCommand(), false)
        {
            base.DropDownItems.Add(new ToolStripMenuItem());
        }

        public AddHostToSelectedPoolToolStripMenuItem(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, bool inContextMenu)
            : base(new AddHostToSelectedPoolCommand(mainWindow, selection), inContextMenu)
        {
            base.DropDownItems.Add(new ToolStripMenuItem());
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Command Command
        {
            get
            {
                return base.Command;
            }
        }

        /// <summary>
        /// Gets a list of hosts to display in the drop down list sorted by name_label.
        /// </summary>
        private List<Host> GetSortedHostList()
        {
            List<Host> output = new List<Host>();

            // Add a menu item for each connection we might add to this pool
            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
            {
                if (Helpers.GetPool(c) == null)
                {
                    Host host = Helpers.GetMaster(c);

                    if (host != null)
                    {
                        output.Add(host);
                    }
                }
            }

            output.Sort();

            return output;
        }

        protected override void OnDropDownOpening(EventArgs e)
        {
            base.DropDownItems.Clear();

            SelectedItemCollection selection = Command.GetSelection();
            IXenConnection connection = selection[0].Connection;

            // Add a menu item for each connection we might add to this pool
            foreach (Host host in GetSortedHostList())
            {
                string hostName = Helpers.GetName(host.Connection);

                if (host.RestrictPooling)
                {
                    hostName = String.Format(Messages.HOST_MENU_ADD_SERVER, hostName);
                }

                AddHostToPoolCommand cmd = new AddHostToPoolCommand(Command.MainWindowCommandInterface, new Host[] { host }, Helpers.GetPool(connection), true);
                CommandToolStripMenuItem hostMenuItem = new CommandToolStripMenuItem(cmd, hostName.EscapeAmpersands(), Images.StaticImages._000_TreeConnected_h32bit_16);

                base.DropDownItems.Add(hostMenuItem);
            }
            
            if (Helpers.GetPool(connection) != null)
            {
                if (base.DropDownItems.Count > 0)
                    base.DropDownItems.Add(new ToolStripSeparator());

                // Add a final option for connecting a new server and adding it to the pool in one action
                AddNewHostToPoolCommand cmd = new AddNewHostToPoolCommand(Command.MainWindowCommandInterface, Helpers.GetPool(connection));
                CommandToolStripMenuItem connectAndAddToPoolMenuItem = new CommandToolStripMenuItem(cmd);

                base.DropDownItems.Add(connectAndAddToPoolMenuItem);
            }
        }

        /// <summary>
        /// Private Command which the parent toolstrip button is hard-coded to use.
        /// </summary>
        private class AddHostToSelectedPoolCommand : Command
        {
            public AddHostToSelectedPoolCommand()
            {
            }

            public AddHostToSelectedPoolCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
                : base(mainWindow, selection)
            {
            }

            protected override bool CanExecuteCore(SelectedItemCollection selection)
            {
                if (selection.Count == 1)
                {
                    IXenConnection connection = selection[0].Connection;
                    bool inPool = selection[0].PoolAncestor != null;
                    

                    return inPool &&  connection != null;
                }

                return false;
            }

            public override string MenuText
            {
                get
                {
                    return Messages.MAINWINDOW_CONTEXTMENU_ADD_SERVER;
                }
            }

            public override string ContextMenuText
            {
                get
                {
                    return Messages.MAINWINDOW_CONTEXTMENU_ADD_SERVER;
                }
            }
        }
    }
}
