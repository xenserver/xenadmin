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
using System.Text;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.NewVMApplianceWizard
{
    public partial class NewVMApplianceWizard : XenWizardBase
    {
        private readonly RBACWarningPage xenTabPageRBAC;
        private readonly NewVMGroupVMsPage<VM_appliance> xenTabPageVMs;
        private readonly NewVMApplianceNamePage xenTabPageName;
        private readonly NewVMApplianceFinishPage xenTabPageFinish;
        private readonly NewVMApplianceVMOrderAndDelaysPage xenTabPageVMOrderAndDelays;

        private readonly Pool Pool;
        public NewVMApplianceWizard(Pool pool)
            : base(pool.Connection)
        {
            InitializeComponent();

            xenTabPageRBAC = new RBACWarningPage();
            xenTabPageVMs = new NewVMGroupVMsPage<VM_appliance>();
            xenTabPageName = new NewVMApplianceNamePage();
            xenTabPageFinish = new NewVMApplianceFinishPage();
            xenTabPageVMOrderAndDelays = new NewVMApplianceVMOrderAndDelaysPage();
            
            Pool = pool;
            xenTabPageVMs.Pool = pool;

            #region RBAC Warning Page Checks
            if (Pool.Connection.Session.IsLocalSuperuser || Helpers.GetMaster(Pool.Connection).external_auth_type == Auth.AUTH_TYPE_NONE)
            {
                //do nothing
            }
            else
            {
                RBACWarningPage.WizardPermissionCheck check = new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_VM_APPLIANCE); 
                check.AddApiCheck("VM_appliance.async_create");
                check.Blocking = true;
                xenTabPageRBAC.AddPermissionChecks(xenConnection, check);
                AddPage(xenTabPageRBAC, 0);
            }
            #endregion

            xenTabPageVMOrderAndDelays.Pool = pool;
            AddPages(xenTabPageName, xenTabPageVMs, xenTabPageVMOrderAndDelays, xenTabPageFinish);
        }

        public NewVMApplianceWizard(Pool pool, List<VM> selection)
            : this(pool)
        {
            this.xenTabPageVMs.SelectedVMs = selection;
        }

        private new string GetSummary()
        {
            return string.Format(Messages.VM_APPLIANCE_SUMMARY.Replace("\\n", "\n").Replace("\\r", "\r"), xenTabPageName.VMApplianceName, CommaSeparated(xenTabPageVMs.SelectedVMs));
        }

        private static string CommaSeparated(IEnumerable<VM> selectedVMs)
        {
            var sb = new StringBuilder();
            foreach (var selectedVM in selectedVMs)
            {
                sb.Append(selectedVM.Name);
                sb.Append(", ");
            }
            if (sb.Length > 2)
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        protected override void FinishWizard()
        {
            var vmAppliance = new VM_appliance
            {
                name_label = xenTabPageName.VMApplianceName,
                name_description = xenTabPageName.VMApplianceDescription,
                Connection = Pool.Connection
            };

            var action = new CreateVMApplianceAction(vmAppliance, xenTabPageVMs.SelectedVMs);
            action.RunAsync();

            var vmSettings = xenTabPageVMOrderAndDelays.getCurrentSettings();
            if (vmSettings != null && vmSettings.Count > 0)
                new SetVMStartupOptionsAction(Pool.Connection, xenTabPageVMOrderAndDelays.getCurrentSettings(), false).RunAsync();

            base.FinishWizard();
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            var prevPageType = senderPage.GetType();

            if (prevPageType == typeof(NewVMApplianceNamePage))
                xenTabPageVMs.GroupName = xenTabPageName.VMApplianceName;
            else if (prevPageType == typeof(NewVMGroupVMsPage<VM_appliance>))
                xenTabPageVMOrderAndDelays.SetSelectedVMs(xenTabPageVMs.SelectedVMs);
            else if (prevPageType == typeof(NewVMApplianceVMOrderAndDelaysPage))
                xenTabPageFinish.Summary = GetSummary();
        }

        protected override string WizardPaneHelpID()
        {
            return CurrentStepTabPage is RBACWarningPage ? FormatHelpId("Rbac") : base.WizardPaneHelpID();
        }
    }
}
