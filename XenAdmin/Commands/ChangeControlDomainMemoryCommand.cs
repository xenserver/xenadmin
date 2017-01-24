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
using XenAdmin.Network;
using XenAdmin.Wizards.NewVMWizard;
using XenAdmin.Properties;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAdmin.Actions;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the Control Domain Memory dialog.
    /// </summary>
    internal class ChangeControlDomainMemoryCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ChangeControlDomainMemoryCommand()
        {
        }

        public ChangeControlDomainMemoryCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }


        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            Host host = selection[0].HostAncestor;
            using (var dlg = new ControlDomainMemoryDialog(host))
                dlg.ShowDialog(Program.MainWindow);
        }

        protected override bool ConfirmationRequired
        {
            get { return false; }
        }
        
        private bool CanExecute(Host host)
        {
            return host != null && host.IsLive && Helpers.ElyOrGreater(host);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.ContainsOneItemOfType<Host>() && selection.AtLeastOneXenObjectCan<Host>(CanExecute);
        }

        public override string MenuText
        {
            get
            {
                return Messages.HOST_MENU_CONTROL_DOMAIN_MEMORY;
            }
        }
    }
}
