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

using System.Collections.Generic;
using System.Windows.Forms;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Dialogs;

namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the Manage CPU group dialog.
    /// </summary>
    internal class ManageCPUGroupCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ManageCPUGroupCommand()
        {
        }

        public ManageCPUGroupCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ManageCPUGroupCommand(IMainWindow mainWindow, IXenConnection connection)
            : base(mainWindow, Helpers.GetPoolOfOne(connection))
        {
        }

        public ManageCPUGroupCommand(IMainWindow mainWindow, Pool pool)
            : base(mainWindow, pool)
        {
        }

        public ManageCPUGroupCommand(IMainWindow mainWindow, Host host)
            : base(mainWindow, host)
        {
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            // this check ensures there's no cross-pool
            return (selection.FirstAsXenObject != null && selection.FirstAsXenObject.Connection != null && selection.FirstAsXenObject.Connection.IsConnected &&
                        (selection.PoolAncestor != null || selection.HostAncestor != null)) ;
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var pool = Helpers.GetPoolOfOne(selection.FirstAsXenObject.Connection);
            if (pool != null)
            {
                this.MainWindowCommandInterface.ShowPerConnectionWizard(pool.Connection, new ManageCPUGroupDialog(pool));
            }
        }

        private void ShowUpsellDialog(IWin32Window parent)
        {
            using (var dlg = new UpsellDialog(HiddenFeatures.LinkLabelHidden ? Messages.UPSELL_BLURB_HA : Messages.UPSELL_BLURB_HA + Messages.UPSELL_BLURB_HA_MORE,
                                    InvisibleMessages.UPSELL_LEARNMOREURL_HA))
                dlg.ShowDialog(Parent);
        }
    }
}
