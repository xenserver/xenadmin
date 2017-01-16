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
using System.Text;
using XenAdmin;
using XenAdmin.Commands;
using System.Windows.Forms;

namespace XenAdminTests
{
    internal class StartShutdownMenuWrapper : TestWrapper<MainWindow>
    {
        public StartShutdownMenuWrapper(MainWindow mainWindow)
            : base(mainWindow)
        {
        }

        private CommandToolStripMenuItem GetItemWithCommand<TCommand>()
        {
            foreach (ToolStripItem item in new MainWindowWrapper(Item).VMMenuItems.StartShutdownMenu.DropDownItems)
            {
                CommandToolStripMenuItem commandItem = item as CommandToolStripMenuItem;

                if (commandItem != null && commandItem.Command is TCommand)
                {
                    return commandItem;
                }
            }
            return null;
        }

        public CommandToolStripMenuItem ForceShutdownToolStripMenuItem
        {
            get
            {
                return GetItemWithCommand<ForceVMShutDownCommand>();
            }
        }

        public CommandToolStripMenuItem ShutDownVMToolStripMenuItem
        {
            get
            {
                return GetItemWithCommand<ShutDownVMCommand>();
            }
        }

        public CommandToolStripMenuItem StartToolStripMenuItem
        {
            get
            {
                return GetItemWithCommand<StartVMCommand>();
            }
        }

        public CommandToolStripMenuItem ResumeToolStripMenuItem
        {
            get
            {
                return GetItemWithCommand<ResumeVMCommand>();
            }
        }

        public CommandToolStripMenuItem SuspendToolStripMenuItem
        {
            get
            {
                return GetItemWithCommand<SuspendVMCommand>();
            }
        }

        public CommandToolStripMenuItem RestartToolStripMenuItem
        {
            get
            {
                return GetItemWithCommand<RebootVMCommand>();
            }
        }
    }
}
