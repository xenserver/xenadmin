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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Actions.OvfActions;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

using Tuple = System.Collections.Generic.KeyValuePair<string, string>;

namespace XenAdmin.Wizards.ExportWizard
{
	internal partial class ExportApplianceWizard : XenWizardBase
    {
        #region Wizard Pages
        private readonly ExportAppliancePage m_pageExportAppliance;
        private readonly RBACWarningPage m_pageRbac;
        private readonly ExportSelectVMsPage m_pageExportSelectVMs;
        private readonly ExportEulaPage m_pageExportEula;
	    private readonly ExportOptionsPage m_pageExportOptions;
        private readonly ExportFinishPage m_pageFinish;
        #endregion

        private bool _xvaMode = true;
        private bool _rbacRequired;

		public ExportApplianceWizard(IXenConnection con, SelectedItemCollection selection)
			: base(con)
		{
			InitializeComponent();

		    m_pageExportAppliance = new ExportAppliancePage();
            m_pageRbac = new RBACWarningPage();
		    m_pageExportSelectVMs = new ExportSelectVMsPage();
            m_pageExportEula = new ExportEulaPage();
		    m_pageExportOptions = new ExportOptionsPage();
            m_pageFinish = new ExportFinishPage();

			m_pageFinish.SummaryRetriever = GetSummary;
			m_pageExportSelectVMs.SelectedItems = selection;

            AddPages(m_pageExportSelectVMs, m_pageExportAppliance, m_pageFinish);

            _rbacRequired = Helpers.ConnectionRequiresRbac(xenConnection);

            if (_rbacRequired)
                AddAfterPage(m_pageExportAppliance, m_pageRbac);

            if (selection.Count == 1 && selection[0].XenObject is VM_appliance || selection.Count > 1)
                _xvaMode = false;

            UpdateMode();
        }

        protected override void FinishWizard()
		{
			if (_xvaMode)
			{
				var filename = Path.Combine(m_pageExportAppliance.ApplianceDirectory, m_pageExportAppliance.ApplianceFileName);
				if (!filename.EndsWith(".xva"))
					filename += ".xva";
				
                var vm = m_pageExportSelectVMs.VMsToExport.FirstOrDefault();

                if (vm != null)
                    new ExportVmAction(xenConnection, vm.Home(), vm, filename, m_pageFinish.VerifyExport, m_pageExportSelectVMs.IncludeMemorySnapshot).RunAsync();
			}
			else
			{
                new ExportApplianceAction(xenConnection,
                    m_pageExportAppliance.ApplianceDirectory,
                    m_pageExportAppliance.ApplianceFileName,
                    m_pageExportSelectVMs.VMsToExport,
                    m_pageExportEula.Eulas,
                    m_pageExportOptions.SignAppliance,
                    m_pageExportOptions.CreateManifest,
                    m_pageExportOptions.Certificate,
                    m_pageExportOptions.EncryptFiles,
                    m_pageExportOptions.EncryptPassword,
                    m_pageExportOptions.CreateOVA,
                    m_pageExportOptions.CompressOVFfiles,
                    m_pageFinish.VerifyExport).RunAsync();
			}

			base.FinishWizard();
		}

        protected override void UpdateWizardContent(XenTabPage page)
		{
			Type type = page.GetType();

            if (type == typeof(ExportSelectVMsPage))
            {
                m_pageExportAppliance.VMsToExport = m_pageExportSelectVMs.VMsToExport;
                m_pageExportAppliance.IncludeMemorySnapshot = m_pageExportSelectVMs.IncludeMemorySnapshot;

                if (_xvaMode && m_pageExportSelectVMs.VMsToExport.Count > 1)
                {
                    _xvaMode = false;
                    UpdateMode();
                    RemovePages(m_pageFinish);
                    AddPages(m_pageExportEula, m_pageExportOptions, m_pageFinish);
                }

                NotifyNextPagesOfChange(m_pageExportAppliance);
            }
            else if (type == typeof(ExportAppliancePage))
            {
                var oldXvaMode = _xvaMode;
                _xvaMode = m_pageExportAppliance.ExportAsXva; //this ensures that m_exportAsXva is assigned a value

                if (_xvaMode != oldXvaMode)
                {
                    UpdateMode();

                    if (_xvaMode)
                    {
                        RemovePages(m_pageExportEula, m_pageExportOptions);
                    }
                    else
                    {
                        RemovePages(m_pageFinish);
                        AddPages(m_pageExportEula, m_pageExportOptions, m_pageFinish);
                    }
                }

                m_pageFinish.ExportAsXva = _xvaMode;
			}

            if (type != typeof(ExportFinishPage))
				NotifyNextPagesOfChange(m_pageFinish);
		}

        protected override string WizardPaneHelpID()
        {
            var curPageType = CurrentStepTabPage.GetType();

            if (curPageType == typeof(RBACWarningPage))
                return FormatHelpId(_xvaMode ? "RbacExportXva" : "RbacExportOvf");

            return base.WizardPaneHelpID();
        }

        protected override IEnumerable<Tuple> GetSummary()
        {
            return _xvaMode ? GetSummaryXva() : GetSummaryOvf();
        }

		private IEnumerable<Tuple> GetSummaryOvf()
		{
			var temp = new List<Tuple>();
			temp.Add(new Tuple(Messages.FINISH_PAGE_REVIEW_APPLIANCE, m_pageExportAppliance.ApplianceFileName));
			temp.Add(new Tuple(Messages.FINISH_PAGE_REVIEW_DESTINATION, m_pageExportAppliance.ApplianceDirectory));

			bool first = true;
			foreach (var vm in m_pageExportSelectVMs.VMsToExport)
			{
				temp.Add(new Tuple(first ? Messages.FINISH_PAGE_REVIEW_VMS : "", vm.Name()));
				first = false;
			}

			first = true;
			foreach (var eula in m_pageExportEula.Eulas)
			{
				temp.Add(new Tuple(first ? Messages.FINISH_PAGE_REVIEW_EULA : "", eula));
				first = false;
			}

			temp.Add(new Tuple(Messages.FINISH_PAGE_CREATE_MANIFEST, m_pageExportOptions.CreateManifest.ToYesNoStringI18n()));
			temp.Add(new Tuple(Messages.FINISH_PAGE_DIGITAL_SIGNATURE, m_pageExportOptions.SignAppliance.ToYesNoStringI18n()));

			temp.Add(new Tuple(Messages.FINISH_PAGE_CREATE_OVA, m_pageExportOptions.CreateOVA.ToYesNoStringI18n()));
			temp.Add(new Tuple(Messages.FINISH_PAGE_COMPRESS, m_pageExportOptions.CompressOVFfiles.ToYesNoStringI18n()));

			return temp;
		}

        private IEnumerable<Tuple> GetSummaryXva()
        {
            var temp = new List<Tuple>
            {
                new Tuple(Messages.FINISH_PAGE_REVIEW_APPLIANCE, m_pageExportAppliance.ApplianceFileName),
                new Tuple(Messages.FINISH_PAGE_REVIEW_DESTINATION, m_pageExportAppliance.ApplianceDirectory),
                new Tuple(Messages.FINISH_PAGE_REVIEW_VMS, string.Join("\n", m_pageExportSelectVMs.VMsToExport.Select(vm => vm.Name()).ToArray()))
            };

            if (m_pageExportSelectVMs.IncludeMemorySnapshot)
                temp.Add(new Tuple(Messages.FINISH_PAGE_INCLUDE_MEM_SNAPSHOT, m_pageExportSelectVMs.IncludeMemorySnapshot.ToYesNoStringI18n()));

            return temp;
        }

        private void UpdateMode()
        {
            Text = _xvaMode ? Messages.MAINWINDOW_XVA_TITLE : Messages.EXPORT_APPLIANCE;

            pictureBoxWizard.Image = _xvaMode
                ? Images.StaticImages.export_32
                : Images.StaticImages._000_ExportVirtualAppliance_h32bit_32;

            if (_rbacRequired)
            {
                var dependencies = _xvaMode
                    ? ExportVmAction.StaticRBACDependencies
                    : ExportApplianceAction.StaticRBACDependencies;

                var message = _xvaMode 
                    ? Messages.RBAC_WARNING_EXPORT_WIZARD_XVA
                    : Messages.RBAC_WARNING_EXPORT_WIZARD_APPLIANCE;

                m_pageRbac.SetPermissionChecks(xenConnection,
                    new WizardRbacCheck(message, dependencies) { Blocking = true });
            }
        }
    }
}
