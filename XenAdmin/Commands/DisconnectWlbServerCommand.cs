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
using System.Linq;
using System.Text;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Forms;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Deconfigures Workload Balancing for the selected Pool.
    /// </summary>
    internal class DisconnectWlbServerCommand : Command
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public DisconnectWlbServerCommand()
        {
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            if (Helpers.FeatureForbidden(selection[0].XenObject, Host.RestrictWLB))
            {
                // Show upsell dialog
                using (var dlg = new UpsellDialog(HiddenFeatures.LinkLabelHidden ? Messages.UPSELL_BLURB_WLB : Messages.UPSELL_BLURB_WLB + Messages.UPSELL_BLURB_WLB_MORE,
                                                    InvisibleMessages.UPSELL_LEARNMOREURL_WLB))
                    dlg.ShowDialog(Parent);
                return;
            }

                try
                {
                    Dialogs.Wlb.DisableWLBDialog disableDialog = new XenAdmin.Dialogs.Wlb.DisableWLBDialog(string.Empty);
                    DialogResult dr = disableDialog.ShowDialog(MainWindow.ActiveForm);

                    if (dr == DialogResult.OK)
                    {
                        Actions.Wlb.DisableWLBAction action = new Actions.Wlb.DisableWLBAction(selection[0].PoolAncestor, true);
                        action.Completed += Program.MainWindow.action_Completed;
                        action.RunAsync();
                        Program.MainWindow.UpdateToolbars();
                    }
                }
                catch (Failure exn)
                {
                    log.Error(exn, exn);
                }
       }

        protected override bool CanExecuteCore(SelectedItemCollection selection)
        {
            //Clearwater doesn't has WLB
            if (selection.Any(s => Helpers.IsClearwater(s.Connection)))
                return false;

            if (selection.Count == 1)
            {
                IXenConnection connection = selection[0].Connection;
                Pool pool = selection[0].PoolAncestor;
                bool inPool = pool != null;

                return inPool  && (Helpers.WlbConfigured(pool.Connection) && !Helpers.FeatureForbidden(connection, Host.RestrictWLB));
            }
            return false;
        }
    }
}
