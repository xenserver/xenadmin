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
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.DRWizards;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Launches the Failover Wizard.
    /// </summary>
    internal class DRFailoverCommand : Command
    {
        private DRFailoverWizard _wizard;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required in the derived
        /// class if it is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public DRFailoverCommand()
        {
        }

        public DRFailoverCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DRFailoverCommand"/> class. 
        /// </summary>
        /// <param name="mainWindow">The main window interface. It can be found at MainWindow.CommandInterface.</param>
        public DRFailoverCommand(IMainWindow mainWindow)
            : base(mainWindow)
        {
        }

        public DRFailoverCommand(IMainWindow mainWindow, IXenObject selection)
            : base(mainWindow, selection)
        {
        }


        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            
            var pool = Helpers.GetPoolOfOne(selection.FirstAsXenObject.Connection);
            if (pool != null)
            {
                if (Helpers.FeatureForbidden(pool.Connection, Host.RestrictDR)) 
                {
                    ShowUpsellDialog(Parent);
                }
                else
                {
                    _wizard = new DRFailoverWizard(pool, DRWizardType.Failover);
                    this.MainWindowCommandInterface.ShowPerConnectionWizard(pool.Connection, _wizard);
                }
            }
        }

        public static void ShowUpsellDialog(IWin32Window parent)
        {
            // Show upsell dialog
            using (var dlg = new UpsellDialog(HiddenFeatures.LinkLabelHidden ? Messages.UPSELL_BLURB_DR : Messages.UPSELL_BLURB_DR + Messages.UPSELL_BLURB_DR_MORE,
                                                InvisibleMessages.UPSELL_LEARNMOREURL_DR))
                dlg.ShowDialog(parent);
        }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            return selection.Count==1 && selection.FirstAsXenObject != null && selection.FirstAsXenObject.Connection != null; 
        }

        public override string ContextMenuText
        {
            get
            {
                return Messages.DR_FAILOVER_AMP; 
            }
        }
    }
}
