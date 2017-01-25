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
using XenAPI;
using XenAdmin.Core;
using System.Drawing;
using XenAdmin.Network;
using XenAdmin.Actions;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using XenAdmin.Properties;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shuts down the selected VMs or Hosts.
    /// </summary>
    internal class ShutDownCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ShutDownCommand()
        {
        }

        public ShutDownCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Command cmd = new ShutDownHostCommand(MainWindowCommandInterface, selection);

            if (cmd.CanExecute())
            {
                cmd.Execute();
            }
            else
            {
                cmd = new ShutDownVMCommand(MainWindowCommandInterface, selection);

                if (cmd.CanExecute())
                {
                    cmd.Execute();
                }
            }
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return new ShutDownVMCommand(MainWindowCommandInterface, selection).CanExecute() || new ShutDownHostCommand(MainWindowCommandInterface, selection).CanExecute();
        }

        public override Image ToolBarImage
        {
            get
            {
                return Images.StaticImages._001_ShutDown_h32bit_24;
            }
        }

        protected override string EnabledToolTipText
        {
            get
            {
                ReadOnlyCollection<SelectedItem> selection = GetSelection();
                if (selection.Count == 1 && selection[0].XenObject is Host)
                {
                    return Messages.MAINWINDOW_TOOLBAR_SHUTDOWNSERVER;
                }
                return Messages.MAINWINDOW_TOOLBAR_SHUTDOWNVM;
            }
        }

        public override string ToolBarText
        {
            get
            {
                return Messages.MAINWINDOW_TOOLBAR_SHUTDOWN;
            }
        }
    }
}
