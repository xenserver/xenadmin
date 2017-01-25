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

using XenAdmin.Wlb;

namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the WLB report window.
    /// </summary>
    internal class ViewWorkloadReportsCommand : Command
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _reportFile = string.Empty;
        private readonly bool _run;

        /// <summary>
        /// Initializes a new instance of this Command. The parameter-less constructor is required if 
        /// this Command is to be attached to a ToolStrip menu item or button. It should not be used in any other scenario.
        /// </summary>
        public ViewWorkloadReportsCommand()
        {
        }

        public ViewWorkloadReportsCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ViewWorkloadReportsCommand(IMainWindow mainWindow, Pool pool)
            : base(mainWindow, pool)
        {
        }

        public ViewWorkloadReportsCommand(IMainWindow mainWindow, Pool pool, string reportFile, bool run)
            : base(mainWindow, pool)
        {
            _reportFile = reportFile;
            _run = run;
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
                WorkloadReports WlbReportWin = new WorkloadReports(_reportFile, _run)
                    {
                        Pool = selection[0].PoolAncestor,
                        Hosts = selection[0].Connection.Cache.Hosts
                    };

                MainWindowCommandInterface.ShowPerConnectionWizard(selection[0].Connection, WlbReportWin);
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
                bool inPool = selection[0].PoolAncestor != null;

                return inPool  &&
                       (((WlbServerState.GetState(selection[0].PoolAncestor) != WlbServerState.ServerState.NotConfigured) &&
                       (WlbServerState.GetState(selection[0].PoolAncestor) != WlbServerState.ServerState.Unknown)) || 
                       Helpers.FeatureForbidden(connection, Host.RestrictWLB));
            }
            return false;
        }
    }
}
