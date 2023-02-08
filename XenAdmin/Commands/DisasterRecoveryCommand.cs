/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.DRWizards;
using XenAPI;


namespace XenAdmin.Commands
{
    internal class DisasterRecoveryCommand : Command
    {
        private DRFailoverWizard _wizard;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public DisasterRecoveryCommand()
        {
        }

        public DisasterRecoveryCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DRFailoverCommand"/> class. 
        /// </summary>
        /// <param name="mainWindow">The main window interface. It can be found at MainWindow.CommandInterface.</param>
        public DisasterRecoveryCommand(IMainWindow mainWindow)
            : base(mainWindow)
        {
        }

        public DisasterRecoveryCommand(IMainWindow mainWindow, IXenObject selection)
            : base(mainWindow, selection)
        {
        }


        protected override void RunCore(SelectedItemCollection selection)
        {

            var pool = Helpers.GetPoolOfOne(selection.FirstAsXenObject.Connection);
            if (pool != null)
            {
                if (Helpers.FeatureForbidden(pool.Connection, Host.RestrictDR))
                {
                    UpsellDialog.ShowUpsellDialog(Messages.UPSELL_BLURB_DR, Parent);
                }
                else
                {
                    _wizard = new DRFailoverWizard(pool);
                    this.MainWindowCommandInterface.ShowPerConnectionWizard(pool.Connection, _wizard);
                }
            }
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
			return selection.FirstAsXenObject != null && selection.FirstAsXenObject.Connection != null &&  selection.FirstAsXenObject.Connection.IsConnected
				&& (selection.PoolAncestor != null || selection.HostAncestor != null); //CA-61207: this check ensures there's no cross-pool selection
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.DR_WIZARD_AMP;
            }
        }
    }

    /// <summary>
    /// This command is only used by the MainWindow DR menu item containing the items that launch the DR wizards
    /// </summary>
    internal class DRCommand : Command
    {
        public DRCommand()
        {
        }

        public DRCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return new DRConfigureCommand(MainWindowCommandInterface, selection).CanRun()
                   || new DisasterRecoveryCommand(MainWindowCommandInterface, selection).CanRun();
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.DISASTER_RECOVERY_CONTEXT_MENU;
            }
        }
    }
}
