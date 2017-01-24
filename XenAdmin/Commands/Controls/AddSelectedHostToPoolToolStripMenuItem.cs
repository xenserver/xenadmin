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
using System.Windows.Forms;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Properties;


namespace XenAdmin.Commands
{
    internal class AddSelectedHostToPoolToolStripMenuItem : CommandToolStripMenuItem
    {
        public AddSelectedHostToPoolToolStripMenuItem()
            : base(new AddSelectedHostToPoolCommand(), false)
        {
            base.DropDownItems.Add(new ToolStripMenuItem());
        }

        public AddSelectedHostToPoolToolStripMenuItem(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, bool inContextMenu)
            : base(new AddSelectedHostToPoolCommand(mainWindow, selection), inContextMenu)
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

        protected override void OnDropDownOpening(EventArgs e)
        {
            base.DropDownItems.Clear();

            SelectedItemCollection selection = Command.GetSelection();
            
            List<IXenConnection> connections = ConnectionsManager.XenConnectionsCopy;
            connections.Sort();

            foreach (IXenConnection connection in connections)
            {
                if (Helpers.GetPool(connection) != null)
                {
                    String poolName = Helpers.GetName(connection).EscapeAmpersands().Ellipsise(Helpers.DEFAULT_NAME_TRIM_LENGTH);
                    AddHostToPoolCommand cmd = new AddHostToPoolCommand(Command.MainWindowCommandInterface, selection.AsXenObjects<Host>(), Helpers.GetPool(connection), true);
                    base.DropDownItems.Add(new CommandToolStripMenuItem(cmd, poolName, Images.StaticImages._000_PoolConnected_h32bit_16));
                }
            }

            if (base.DropDownItems.Count > 0)
                base.DropDownItems.Add(new ToolStripSeparator());

            base.DropDownItems.Add(new CommandToolStripMenuItem(new NewPoolCommand(Command.MainWindowCommandInterface, selection)));
        }

        private class AddSelectedHostToPoolCommand : Command
        {
            public AddSelectedHostToPoolCommand()
            {
            }

            public AddSelectedHostToPoolCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
                : base(mainWindow, selection)
            {
            }

            protected override bool CanExecuteCore(SelectedItemCollection selection)
            {
                return selection.AllItemsAre<Host>(CanExecute);
            }

            private bool CanExecute(Host host)
            {
                return host != null && host.Connection != null && host.Connection.IsConnected && Helpers.GetPool(host.Connection) == null && !host.RestrictPooling;
            }

            public override string MenuText
            {
                get
                {
                    foreach (SelectedItem item in GetSelection())
                    {
                        Host host = item.XenObject as Host;

                        if (host != null && host.Connection != null && host.Connection.IsConnected && Helpers.GetPool(host.Connection) == null && host.RestrictPooling)
                        {
                            return Messages.HOST_MENU_ADD_TO_POOL_LICENSE_RESTRICTION;
                        }
                    }
                    return Messages.HOST_MENU_ADD_TO_POOL;
                }
            }

            public override string ContextMenuText
            {
                get
                {
                    return Messages.HOST_MENU_ADD_TO_POOL_CONTEXT_MENU;
                }
            }
        }
    }
}
