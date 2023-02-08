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
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.Wlb;
using XenAdmin.Wlb;
using XenAPI;


namespace XenAdmin.Commands
{
    internal class WlbCommand : Command
    {
        public WlbCommand()
        {
        }

        public WlbCommand(IMainWindow mainWindow, IXenObject xenObject)
            : base(mainWindow, xenObject)
        {
        }

        public WlbCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return new DisconnectWlbServerCommand(MainWindowCommandInterface, selection).CanRun() ||
                   new ViewWorkloadReportsCommand(MainWindowCommandInterface, selection).CanRun();
        }

        protected bool IsLicensedFeature(SelectedItemCollection selection)
        {
            if (Helpers.FeatureForbidden(selection[0].XenObject, Host.RestrictWLB))
            {
                UpsellDialog.ShowUpsellDialog(Messages.UPSELL_BLURB_WLB, Parent);
                return false;
            }

            return true;
        }

        public override string MenuText => Messages.WLB_COMMAND_MENU_ITEM;
    }


    internal class DisconnectWlbServerCommand : WlbCommand
    {
        public DisconnectWlbServerCommand()
        {
        }

        public DisconnectWlbServerCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            if (!IsLicensedFeature(selection))
                return;

            using (var dialog = new WarningDialog(Messages.WLB_DISCONNECT_SERVER,
                ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo))
                if (dialog.ShowDialog(Program.MainWindow) == DialogResult.Yes)
                {
                    var action = new Actions.Wlb.DisableWLBAction(selection[0].PoolAncestor, true);
                    action.Completed += Program.MainWindow.action_Completed;
                    action.RunAsync();
                }
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            return selection.Count == 1 && selection[0].PoolAncestor != null &&
                   !string.IsNullOrEmpty(selection[0].PoolAncestor.wlb_url);
        }

        public override string MenuText => Messages.WLB_DISCONNECT_MENU_ITEM;
    }


    internal class ViewWorkloadReportsCommand : WlbCommand
    {
        private readonly string _reportFile = string.Empty;
        private readonly bool _run;

        public ViewWorkloadReportsCommand()
        {
        }

        public ViewWorkloadReportsCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        public ViewWorkloadReportsCommand(IMainWindow mainWindow, Pool pool,
            string reportFile = null, bool run = false)
            : base(mainWindow, pool)
        {
            _reportFile = reportFile ?? string.Empty;
            _run = run;
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            if (!IsLicensedFeature(selection))
                return;

            var wlbReports = new WorkloadReports(_reportFile, _run)
            {
                Pool = selection[0].PoolAncestor,
                Hosts = selection[0].Connection.Cache.Hosts
            };

            MainWindowCommandInterface.ShowPerConnectionWizard(selection[0].Connection, wlbReports);
        }

        protected override bool CanRunCore(SelectedItemCollection selection)
        {
            if (selection.Count != 1 || selection[0].PoolAncestor == null)
                return false;

            var state = WlbServerState.GetState(selection[0].PoolAncestor);
            return state != WlbServerState.ServerState.NotConfigured && state != WlbServerState.ServerState.Unknown;
        }

        public override string MenuText => Messages.WLB_REPORT_VIEW_MENU_ITEM;
    }
}
