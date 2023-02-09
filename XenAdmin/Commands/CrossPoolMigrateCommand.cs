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
using System.Drawing;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.CrossPoolMigrateWizard;
using XenAdmin.Wizards.CrossPoolMigrateWizard.Filters;
using XenAPI;


namespace XenAdmin.Commands
{
    internal class CrossPoolMigrateCommand : VMOperationCommand
    {
        private readonly Dictionary<string, Dictionary<string, string>> cantRunReasons = new Dictionary<string, Dictionary<string, string>>();

        private readonly bool _resumeAfter;
        protected Host preSelectedHost;

        public CrossPoolMigrateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        { }

        public CrossPoolMigrateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection, Host preSelectedHost, bool resumeAfter=false)
            : base(mainWindow, selection)
        {
            this.preSelectedHost = preSelectedHost;
            _resumeAfter = resumeAfter;
        }

        public override string MenuText
        {
            get
            {
                if (preSelectedHost == null)
                    return Messages.HOST_MENU_CPM_TEXT;

                var cantRunReason = CantRunReason;
                return string.IsNullOrEmpty(cantRunReason)
                    ? preSelectedHost.Name().EscapeAmpersands()
                    : string.Format(Messages.MAINWINDOW_CONTEXT_REASON, preSelectedHost.Name().EscapeAmpersands(), cantRunReason);
            }
        }

        public override string ContextMenuText => Messages.HOST_MENU_CPM_TEXT;

        public override Image MenuImage => preSelectedHost == null
            ? Images.StaticImages._000_MigrateVM_h32bit_16
            : Images.StaticImages._000_TreeConnected_h32bit_16;

        protected override void RunCore(SelectedItemCollection selection)
        {
            var con = selection.GetConnectionOfFirstItem();

            if (Helpers.FeatureForbidden(con, Host.RestrictCrossPoolMigrate))
            {
                UpsellDialog.ShowUpsellDialog(Messages.UPSELL_BLURB_CPM, Parent);
            }
            else
            {
                var wizard = new CrossPoolMigrateWizard(con, selection, preSelectedHost, WizardMode.Migrate, _resumeAfter);
                MainWindowCommandInterface.ShowPerConnectionWizard(con, wizard); 
            }
        }

        protected override Host GetHost(VM vm)
        {
            return Helpers.GetCoordinator(vm.Connection);
        }

        protected override bool CanRun(VM vm)
        {
            var canRun = CanRun(vm, preSelectedHost, out var failureReason, cantRunReasons);
            CantRunReason = failureReason;
            return canRun;
        }

        public static bool CanRun(VM vm, Host preselectedHost, out string failureReason, Dictionary<string, Dictionary<string, string>> cache = null)
        {
            if (vm.allowed_operations == null || !vm.allowed_operations.Contains(vm_operations.migrate_send))
            {
                failureReason = Messages.MIGRATION_NOT_ALLOWED;
                return false;
            }

            if (!vm.SRs().All(sr => sr != null && !sr.HBALunPerVDI()))
            {
                failureReason = Messages.MIGRATION_NOT_ALLOWED_USUPPORTED_SR;
                return false;
            }

            var vms = new List<VM> {vm};

            if (preselectedHost != null && new ResidentHostIsSameAsSelectionFilter(preselectedHost, vms).FailureFound(out failureReason))
                return false;

            if (new WlbEnabledFilter(preselectedHost, vms).FailureFound(out failureReason))
                return false;

            if (preselectedHost != null && new CrossPoolMigrateCanMigrateFilter(preselectedHost, new List<VM> {vm},
                    WizardMode.Migrate, cache).FailureFound(out failureReason))
                return false;

            return true;
        }

        public string CantRunReason { get; private set; }
    }
}
