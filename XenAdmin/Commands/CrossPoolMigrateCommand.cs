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
        private bool _resumeAfter;

        public CrossPoolMigrateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        { }

        protected Host preSelectedHost = null;
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

        public override string ContextMenuText { get { return Messages.HOST_MENU_CPM_TEXT; } }

        public override Image MenuImage
        {
            get { return preSelectedHost == null ? Images.StaticImages._000_MigrateVM_h32bit_16 : Images.StaticImages._000_TreeConnected_h32bit_16; }
        }

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

        private readonly Dictionary<VM, string> cantRunReasons = new Dictionary<VM, string>();

        protected override bool CanRun(VM vm)
        {
            if (preSelectedHost == null)
                return CanRun(vm, preSelectedHost);

            var filter = new CrossPoolMigrateCanMigrateFilter(preSelectedHost, new List<VM> {vm}, WizardMode.Migrate);
            var canRun = CanRun(vm, preSelectedHost, filter);
            if (string.IsNullOrEmpty(filter.Reason))
                cantRunReasons.Remove(vm);
            else
                cantRunReasons[vm] = filter.Reason;
            return canRun;
        }

        public static bool CanRun(VM vm, Host preselectedHost, CrossPoolMigrateCanMigrateFilter filter = null)
        {
            bool failureFound = false;

            if (preselectedHost != null)
            {
                failureFound = filter == null 
                    ? new CrossPoolMigrateCanMigrateFilter(preselectedHost, new List<VM> {vm}, WizardMode.Migrate).FailureFound
                    : filter.FailureFound;
            }

            return !failureFound &&
                   vm.allowed_operations != null &&
                   vm.allowed_operations.Contains(vm_operations.migrate_send) &&
                   !Helpers.CrossPoolMigrationRestrictedWithWlb(vm.Connection) &&
                   vm.SRs().ToList().All(sr=> sr != null && !sr.HBALunPerVDI()) &&
                   (preselectedHost == null || vm.Connection.Resolve(vm.resident_on) != preselectedHost); //Not the same as the pre-selected host
        }

        public string CantRunReason
        {
            get
            {
                if (cantRunReasons.Count == GetSelection().Count) // none can run
                {
                    var uniqueReasons = cantRunReasons.Values.Distinct().ToList();

                    if (uniqueReasons.Count == 1)
                        return uniqueReasons[0];
                }
                return null;
            }
        }
    }
}
