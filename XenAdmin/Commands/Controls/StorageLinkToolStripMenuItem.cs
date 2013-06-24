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
using System.ComponentModel;
using System.Windows.Forms;

namespace XenAdmin.Commands
{
    internal class StorageLinkToolStripMenuItem : CommandToolStripMenuItem
    {
        public StorageLinkToolStripMenuItem()
            : base(new StorageLinkToolStripMenuItemCommand(), false)
        {
            base.DropDownItems.Add(new ToolStripMenuItem());
        }

        protected sealed override void OnDropDownOpening(EventArgs e)
        {
            base.DropDownItems.Clear();

            Command cmd = new ChangeStorageLinkPasswordCommand(Command.MainWindowCommandInterface, Command.GetSelection());
            CommandToolStripMenuItem item = new CommandToolStripMenuItem(cmd);

            base.DropDownItems.Add(item);

            cmd = new SetStorageLinkLicenseServerCommand(Command.MainWindowCommandInterface, Command.GetSelection());
            item = new CommandToolStripMenuItem(cmd);

            base.DropDownItems.Add(item);
            base.DropDownItems.Add(new ToolStripSeparator());

            cmd = new AddStorageLinkSystemCommand(Command.MainWindowCommandInterface, Command.GetSelection());
            item = new CommandToolStripMenuItem(cmd);

            base.DropDownItems.Add(item);

            cmd = new RemoveStorageLinkSystemCommand(Command.MainWindowCommandInterface, Command.GetSelection());
            item = new CommandToolStripMenuItem(cmd);

            base.DropDownItems.Add(item);

            cmd = new RefreshStorageLinkConnectionCommand(Command.MainWindowCommandInterface, Command.GetSelection());
            item = new CommandToolStripMenuItem(cmd);

            base.DropDownItems.Add(item);
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

        private class StorageLinkToolStripMenuItemCommand : Command
        {
            public override string MenuText
            {
                get
                {
                    return "Storage&Link";
                }
            }

            protected override bool CanExecuteCore(SelectedItemCollection selection)
            {
                return selection.AllItemsAre<IStorageLinkObject>();
            }
        }
    }
}
