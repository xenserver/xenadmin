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


namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the New VM Wizard.
    /// </summary>
    internal class NewVMCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public NewVMCommand()
        {
        }

        public NewVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public NewVMCommand(IMainWindow mainWindow, IXenConnection connection, Host defaultAffinity, VM defaultTemplate)
            : base(mainWindow, new SelectedItem(defaultTemplate, connection, defaultAffinity, null))
        {

        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            VM template = null;

            foreach (SelectedItem item in selection)
            {
                VM vm = item.XenObject as VM;

                if (vm != null && vm.is_a_template)
                {
                    template = vm;
                    break;
                }
            }

            var connection = selection[0].Connection;
            Host host = null;

            if (template != null)
            {
                host = template.Home();
            }
            else
            {
                host = selection[0].HostAncestor;
            }

            Execute(connection, host, template);
        }

        private void Execute(IXenConnection connection, Host DefaultAffinity, VM DefaultTemplate)
        {
            MainWindowCommandInterface.ShowPerConnectionWizard(connection, new NewVMWizard(connection, DefaultTemplate, DefaultAffinity));
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            Host hostAncestor = selection.HostAncestor;
            Pool poolAncestor = selection.PoolAncestor;

            if (hostAncestor != null && hostAncestor.enabled && hostAncestor.IsLive && selection[0].Connection.IsConnected)
            {
                return true;
            }
            else if (hostAncestor == null && poolAncestor != null && Helpers.PoolHasEnabledHosts(poolAncestor))
            {
                return true;
            }
            return false;
        }

        public override Image ToolBarImage
        {
            get
            {
                return Images.StaticImages._000_CreateVM_h32bit_24;
            }
        }

        public override Image MenuImage
        {
            get
            {
                return Images.StaticImages._001_CreateVM_h32bit_16;
            }
        }

        public override string MenuText
        {
            get
            {
                return Messages.MAINWINDOW_NEW_VM;
            }
        }

        public override Keys ShortcutKeys
        {
            get
            {
                return Keys.Control | Keys.N;
            }
        }

        public override string ShortcutKeyDisplayString
        {
            get
            {
                return Messages.MAINWINDOW_CTRL_N;
            }
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.MAINWINDOW_NEW_VM_CONTEXT_MENU;
            }
        }
    }
}
