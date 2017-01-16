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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Properties;
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
            get { return preSelectedHost == null ? Messages.HOST_MENU_CPM_TEXT : preSelectedHost.Name.EscapeAmpersands(); }
        }

        public override string ContextMenuText { get { return Messages.HOST_MENU_CPM_TEXT; } }

        public override Image MenuImage
        {
            get { return preSelectedHost == null ? Images.StaticImages._000_MigrateVM_h32bit_16 : Images.StaticImages._000_TreeConnected_h32bit_16; }
        }

        protected override void ExecuteCore(SelectedItemCollection selection)
        {
            var con = selection.GetConnectionOfFirstItem();

            if (Helpers.FeatureForbidden(con, Host.RestrictCrossPoolMigrate))
            {
                ShowUpsellDialog(Parent);
            }
            else
            {
                var wizard = new CrossPoolMigrateWizard(con, selection, preSelectedHost, WizardMode.Migrate, _resumeAfter);
                MainWindowCommandInterface.ShowPerConnectionWizard(con, wizard); 
            }
        }

        protected override Host GetHost(VM vm)
        {
            return Helpers.GetMaster(vm.Connection);
        }

        public static void ShowUpsellDialog(IWin32Window parent)
        {
            using (var  dlg = new UpsellDialog(HiddenFeatures.LinkLabelHidden ? Messages.UPSELL_BLURB_CPM : Messages.UPSELL_BLURB_CPM + Messages.UPSELL_BLURB_CPM_MORE,
                                                InvisibleMessages.UPSELL_LEARNMOREURL_CPM))
                dlg.ShowDialog(parent);
        }

        protected override bool CanExecute(VM vm)
        {
            return CanExecute(vm, preSelectedHost);
        }

        public static bool CanExecute(VM vm, Host preselectedHost)
        {
            bool failureFound = false;

            if (preselectedHost != null)
            {
                failureFound = new CrossPoolMigrateCanMigrateFilter(preselectedHost, new List<VM> {vm}, WizardMode.Migrate).FailureFound;
            }

            return !failureFound &&
                   vm.allowed_operations != null &&
                   vm.allowed_operations.Contains(vm_operations.migrate_send) &&
                   !Helpers.CrossPoolMigrationRestrictedWithWlb(vm.Connection) &&
                   vm.SRs.ToList().All(sr=> sr != null && !sr.HBALunPerVDI) &&
                   (preselectedHost == null || vm.Connection.Resolve(vm.resident_on) != preselectedHost); //Not the same as the pre-selected host
        }
    }
}
