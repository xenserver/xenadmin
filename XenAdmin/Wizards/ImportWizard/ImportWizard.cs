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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using XenAdmin.Actions;
using XenAdmin.Actions.OvfActions;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAdmin.Wizards.GenericPages;
using XenAPI;
using XenOvf;
using XenOvf.Definitions;
using XenOvf.Utilities;
using Tuple = System.Collections.Generic.KeyValuePair<string, string>;

namespace XenAdmin.Wizards.ImportWizard
{
    internal partial class ImportWizard : XenWizardBase
    {
        #region Private fields
        private readonly ImportSelectStoragePage m_pageStorage;
        private readonly ImportSelectNetworkPage m_pageNetwork;
        private readonly ImportSelectHostPage m_pageHost;
        private readonly ImportSecurityPage m_pageSecurity;
        private readonly ImportEulaPage m_pageEula;
        private readonly ImportOptionsPage m_pageOptions;
        private readonly ImportFinishPage m_pageFinish;
        private readonly RBACWarningPage m_pageRbac;
        private readonly ImageVMConfigPage m_pageVMconfig;
        private readonly ImportSourcePage m_pageImportSource;
        private readonly StoragePickerPage m_pageXvaStorage;
        private readonly NetworkPickerPage m_pageXvaNetwork;
        private readonly GlobalSelectHost m_pageXvaHost;
        private readonly LunPerVdiImportPage lunPerVdiMappingPage;
        private readonly ImportBootOptionPage m_pageBootOptions;

        private IXenObject m_selectedObject;
        private Dictionary<string, VmMapping> m_vmMappings = new Dictionary<string, VmMapping>();
        /// <summary>
        /// Make this nullable and initialize with null so the pages are added correctly to the wizard progress the first time
        /// </summary>
        private ImportType? m_typeOfImport;
        private bool m_ignoreAffinitySet;
        private EnvelopeType m_envelopeFromVhd;
        private Package _selectedOvfPackage;
        private string _selectedImagePath;
        private Host _selectedAffinity;
        private IXenConnection _targetConnection;
        #endregion

        public ImportWizard(IXenConnection con, IXenObject xenObject, string filename, bool ovfModeOnly = false)
            : base(con)
        {
            InitializeComponent();

            m_pageStorage = new ImportSelectStoragePage();
            m_pageNetwork = new ImportSelectNetworkPage();
            m_pageHost = new ImportSelectHostPage();
            m_pageSecurity = new ImportSecurityPage();
            m_pageEula = new ImportEulaPage();
            m_pageOptions = new ImportOptionsPage();
            m_pageFinish = new ImportFinishPage();
            m_pageRbac = new RBACWarningPage();
            m_pageVMconfig = new ImageVMConfigPage();
            m_pageImportSource = new ImportSourcePage();
            m_pageXvaStorage = new StoragePickerPage();
            m_pageXvaNetwork = new NetworkPickerPage();
            m_pageXvaHost = new GlobalSelectHost();
            lunPerVdiMappingPage = new LunPerVdiImportPage { Connection = con };
            m_pageBootOptions = new ImportBootOptionPage();

            m_selectedObject = xenObject;
            m_pageFinish.SummaryRetriever = GetSummary;
            m_pageXvaStorage.ImportVmCompleted += m_pageXvaStorage_ImportVmCompleted;

            if (!string.IsNullOrEmpty(filename))
                m_pageImportSource.SetFileName(filename);

            m_pageImportSource.OvfModeOnly = ovfModeOnly;
            AddPages(m_pageImportSource, m_pageHost, m_pageStorage, m_pageNetwork, m_pageFinish);

            m_pageHost.ConnectionSelectionChanged += pageHost_ConnectionSelectionChanged;
            m_pageXvaHost.ConnectionSelectionChanged += pageHost_ConnectionSelectionChanged;
            ShowXenAppXenDesktopWarning(con);
        }

        #region Override (XenWizardBase) Methods

        protected override void FinishWizard()
        {
            switch (m_typeOfImport)
            {
                case ImportType.Xva:
                    if (m_pageXvaStorage.ImportXvaAction != null)
                        m_pageXvaStorage.ImportXvaAction.EndWizard(m_pageFinish.StartVmsAutomatically, m_pageXvaNetwork.VIFs);
                    break;
                case ImportType.Ovf:
                    new ImportApplianceAction(_targetConnection,
                        m_pageImportSource.SelectedOvfPackage,
                        m_vmMappings,
                        m_pageSecurity.VerifyManifest,
                        m_pageSecurity.VerifySignature,
                        m_pageSecurity.Password,
                        m_pageOptions.RunFixups,
                        m_pageOptions.SelectedIsoSR,
                        m_pageFinish.StartVmsAutomatically).RunAsync();
                    break;
                case ImportType.Vhd:
                    new ImportImageAction(_targetConnection,
                        m_envelopeFromVhd,
                        Path.GetDirectoryName(m_pageImportSource.FilePath),
                        m_vmMappings,
                        m_pageOptions.RunFixups,
                        m_pageOptions.SelectedIsoSR,
                        m_pageFinish.StartVmsAutomatically,
                        VMOperationCommand.WarningDialogHAInvalidConfig,
                        VMOperationCommand.StartDiagnosisForm).RunAsync();
                    break;
            }

            base.FinishWizard();
        }

        protected override void OnCancel(ref bool cancel)
        {
            base.OnCancel(ref cancel);

            if (cancel)
                return;

            if (m_pageXvaStorage.ImportXvaAction != null)
            {
                m_pageXvaStorage.ImportXvaAction.EndWizard(false, null);
                m_pageXvaStorage.ImportXvaAction.Cancel();
            }
        }

        protected override void UpdateWizardContent(XenTabPage page)
        {
            var type = page.GetType();

            if (type == typeof(ImportSourcePage))
            {
                var oldTypeOfImport = m_typeOfImport; //store previous type
                m_typeOfImport = m_pageImportSource.TypeOfImport;

                var appliancePages = new XenTabPage[] { m_pageEula, m_pageHost, m_pageStorage, m_pageNetwork, m_pageSecurity, m_pageOptions };
                var imagePages = new XenTabPage[] { m_pageVMconfig, m_pageHost, m_pageStorage, m_pageNetwork, m_pageOptions };
                var xvaPages = new XenTabPage[] { m_pageXvaHost, m_pageXvaStorage, m_pageXvaNetwork };

                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        if (oldTypeOfImport != ImportType.Ovf)
                        {
                            Text = Messages.WIZARD_TEXT_IMPORT_OVF;
                            pictureBoxWizard.Image = Images.StaticImages._000_ImportVirtualAppliance_h32bit_32;
                            RemovePages(imagePages);
                            RemovePage(m_pageBootOptions);
                            RemovePages(xvaPages);
                            AddAfterPage(m_pageImportSource, appliancePages);
                        }

                        var oldSelectedOvfPackage = _selectedOvfPackage;
                        _selectedOvfPackage = m_pageImportSource.SelectedOvfPackage;

                        if (oldTypeOfImport != ImportType.Ovf || oldSelectedOvfPackage != _selectedOvfPackage)
                        {
                            m_pageEula.SelectedOvfEnvelope = _selectedOvfPackage.OvfEnvelope;
                            m_pageSecurity.SelectedOvfPackage = _selectedOvfPackage;
                            CheckDisabledPages(m_pageEula, m_pageSecurity); //decide whether to disable these progress steps
                            
                            m_vmMappings.Clear();
                            string[] sysIds = OVF.FindSystemIds(_selectedOvfPackage.OvfEnvelope);
                            foreach (string sysId in sysIds)
                                m_vmMappings.Add(sysId, new VmMapping(sysId) {VmNameLabel = FindVMName(_selectedOvfPackage.OvfEnvelope, sysId)});

                            m_pageHost.SelectedOvfEnvelope = _selectedOvfPackage.OvfEnvelope;
                            m_pageHost.SetDefaultTarget(m_pageHost.ChosenItem ?? m_selectedObject);
                            m_pageHost.VmMappings = m_vmMappings;
                            m_pageStorage.SelectedOvfEnvelope = _selectedOvfPackage.OvfEnvelope;
                            lunPerVdiMappingPage.SelectedOvfEnvelope = _selectedOvfPackage.OvfEnvelope;
                            m_pageNetwork.SelectedOvfEnvelope = _selectedOvfPackage.OvfEnvelope;

                            NotifyNextPagesOfChange(m_pageEula, m_pageHost, m_pageStorage, m_pageNetwork, m_pageSecurity, m_pageOptions);
                        }
                        break;
                    case ImportType.Vhd:
                        if (oldTypeOfImport != ImportType.Vhd)
                        {
                            Text = Messages.WIZARD_TEXT_IMPORT_VHD;
                            pictureBoxWizard.Image = Images.StaticImages._000_ImportVM_h32bit_32;
                            RemovePages(appliancePages);
                            RemovePages(xvaPages);
                            AddAfterPage(m_pageImportSource, imagePages);

                            //if _targetConnection=null, i.e. we haven't selected a connection yet, do not add the page
                            if (_targetConnection != null && BootModesControl.ShowBootModeOptions(_targetConnection))
                                AddAfterPage(m_pageNetwork, m_pageBootOptions);
                        }

                        var oldSelectedImagePath = _selectedImagePath;
                        _selectedImagePath = m_pageImportSource.FilePath;

                        if (oldTypeOfImport != ImportType.Vhd || oldSelectedImagePath != _selectedImagePath)
                        {
                            m_vmMappings.Clear();
                            m_pageVMconfig.IsWim = m_pageImportSource.IsWIM;
                            NotifyNextPagesOfChange(m_pageVMconfig, m_pageHost, m_pageStorage, m_pageNetwork, m_pageOptions);
                        }
                        break;
                    case ImportType.Xva:
                        if (oldTypeOfImport != ImportType.Xva)
                        {
                            Text = Messages.WIZARD_TEXT_IMPORT_XVA;
                            pictureBoxWizard.Image = Images.StaticImages._000_ImportVM_h32bit_32;
                            RemovePages(imagePages);
                            RemovePage(m_pageBootOptions);
                            RemovePages(appliancePages);
                            AddAfterPage(m_pageImportSource, xvaPages);
                        }
                        m_pageXvaHost.SetDefaultTarget(m_selectedObject);
                        m_pageXvaStorage.FilePath = m_pageImportSource.FilePath;
                        break;
                }
            }
            else if (type == typeof(ImageVMConfigPage))
            {
                var sysId = Guid.NewGuid().ToString();

                var newMapping = new VmMapping(sysId)
                {
                    VmNameLabel = m_pageVMconfig.VmName,
                    CpuCount = m_pageVMconfig.CpuCount,
                    Capacity = m_pageImportSource.DiskCapacity + m_pageVMconfig.AdditionalSpace,
                    Memory = m_pageVMconfig.Memory,
                    BootParams = m_pageBootOptions.BootParams,
                    PlatformSettings = m_pageBootOptions.PlatformSettings
                };

                var oldMapping = m_vmMappings.Values.FirstOrDefault();
                if (oldMapping != null)
                {
                    newMapping.XenRef = oldMapping.XenRef;
                    newMapping.TargetName = oldMapping.TargetName;
                    newMapping.Storage = oldMapping.Storage;
                    newMapping.StorageToAttach = oldMapping.StorageToAttach;
                    newMapping.Networks = oldMapping.Networks;
                }

                if (!newMapping.Equals(oldMapping))
                {
                    m_envelopeFromVhd = OVF.CreateOvfEnvelope(sysId, newMapping.VmNameLabel,
                        newMapping.CpuCount, newMapping.Memory,
                        newMapping.BootParams, newMapping.PlatformSettings,
                        newMapping.Capacity,
                        m_pageImportSource.FilePath, m_pageImportSource.ImageLength, BrandManager.ProductBrand);

                    m_vmMappings.Clear();
                    m_vmMappings.Add(sysId, newMapping);

                    m_pageHost.VmMappings = m_vmMappings;
                    m_pageHost.SetDefaultTarget(m_pageHost.ChosenItem ?? m_selectedObject);

                    m_pageHost.SelectedOvfEnvelope = m_envelopeFromVhd;
                    m_pageStorage.SelectedOvfEnvelope = m_envelopeFromVhd;
                    lunPerVdiMappingPage.SelectedOvfEnvelope = m_envelopeFromVhd;
                    m_pageNetwork.SelectedOvfEnvelope = m_envelopeFromVhd;

                    NotifyNextPagesOfChange(m_pageHost, lunPerVdiMappingPage, m_pageStorage, m_pageNetwork);
                }
            }
            else if (type == typeof(ImportSelectHostPage))
            {
                var oldTargetConnection = _targetConnection;
                _targetConnection = m_pageHost.ChosenItem?.Connection;
                var oldHostSelection = m_vmMappings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.TargetName);
                m_vmMappings = m_pageHost.VmMappings;

                if (oldTargetConnection != _targetConnection)
                {
                    RemovePage(m_pageRbac);
                    ConfigureRbacPage(_targetConnection);

                    RemovePage(m_pageBootOptions);
                    if (m_typeOfImport == ImportType.Vhd && BootModesControl.ShowBootModeOptions(_targetConnection))
                        AddAfterPage(m_pageNetwork, m_pageBootOptions);
                }

                m_pageStorage.VmMappings = m_vmMappings;
                m_pageStorage.Connection = _targetConnection;
                m_pageNetwork.Connection = _targetConnection;
                m_pageOptions.Connection = _targetConnection;
                m_pageBootOptions.Connection = _targetConnection;

                if (oldTargetConnection != _targetConnection ||
                    oldHostSelection.Any(kvp=> !m_vmMappings.TryGetValue(kvp.Key, out var map) || map == null || map.TargetName != kvp.Value))
                    NotifyNextPagesOfChange(m_pageStorage, m_pageNetwork, m_pageOptions);
            }
            else if (type == typeof(ImportBootOptionPage))
            {
                var oldMapping = m_vmMappings.Values.First();

                if (oldMapping.BootParams != m_pageBootOptions.BootParams ||
                    oldMapping.PlatformSettings != m_pageBootOptions.PlatformSettings)
                {
                    string sysId = null;
                    if (m_envelopeFromVhd.Item is VirtualSystem_Type vs)
                        sysId = vs.id;
                    else if (m_envelopeFromVhd.Item is VirtualSystemCollection_Type vsc)
                        sysId = vsc.Content.FirstOrDefault()?.id;

                    if (oldMapping.BootParams != m_pageBootOptions.BootParams)
                    {
                        m_envelopeFromVhd = OVF.UpdateBootParams(m_envelopeFromVhd, sysId, m_pageBootOptions.BootParams);
                        m_vmMappings.Values.First().BootParams = m_pageBootOptions.BootParams;
                    }

                    if (oldMapping.PlatformSettings != m_pageBootOptions.PlatformSettings)
                    {
                        m_envelopeFromVhd = OVF.UpdatePlatform(m_envelopeFromVhd, sysId, m_pageBootOptions.PlatformSettings);
                        m_vmMappings.Values.First().PlatformSettings = m_pageBootOptions.PlatformSettings;
                    }
                }

                m_pageStorage.SelectedOvfEnvelope = m_envelopeFromVhd;
                lunPerVdiMappingPage.SelectedOvfEnvelope = m_envelopeFromVhd;
                m_pageNetwork.SelectedOvfEnvelope = m_envelopeFromVhd;
            }
            else if (type == typeof(ImportSelectStoragePage))
            {
                RemovePage(lunPerVdiMappingPage);
                lunPerVdiMappingPage.ClearPickerData();
                m_vmMappings = m_pageStorage.VmMappings;
                m_pageNetwork.VmMappings = m_vmMappings;
                lunPerVdiMappingPage.VmMappings = m_vmMappings;
                if (lunPerVdiMappingPage.IsAnyPickerDataMappable
                    && lunPerVdiMappingPage.MapLunsToVdisRequired
                    && m_typeOfImport == ImportType.Ovf)
                    AddAfterPage(m_pageStorage, lunPerVdiMappingPage);
            }
            else if (type == typeof(LunPerVdiImportPage))
            {
                m_vmMappings = lunPerVdiMappingPage.VmMappings;
                m_pageNetwork.VmMappings = m_vmMappings;
            }
            else if (type == typeof(ImportSelectNetworkPage))
            {
                m_vmMappings = m_pageNetwork.VmMappings;
                m_pageOptions.VmMappings = m_vmMappings;
            }
            else if (type == typeof(GlobalSelectHost))
            {
                var oldSelectedAffinity = _selectedAffinity;
                _selectedAffinity = m_pageXvaHost.SelectedHost;
                var oldTargetConnection = _targetConnection;
                _targetConnection = _selectedAffinity == null ? m_pageXvaHost.SelectedConnection : _selectedAffinity.Connection;

                if (oldTargetConnection != _targetConnection)
                {
                    RemovePage(m_pageRbac);
                    ConfigureRbacPage(_targetConnection);
                }

                m_pageXvaStorage.TargetConnection = _targetConnection;
                m_pageXvaStorage.TargetHost = m_ignoreAffinitySet ? null : _selectedAffinity;

                m_pageXvaNetwork.SelectedConnection = _targetConnection;
                m_pageXvaNetwork.SelectedAffinity = _selectedAffinity;

                if (oldTargetConnection != _targetConnection ||
                    oldSelectedAffinity?.opaque_ref != _selectedAffinity?.opaque_ref)
                    NotifyNextPagesOfChange(m_pageXvaStorage, m_pageXvaNetwork);
            }
            else if (type == typeof(StoragePickerPage))
            {
                m_pageFinish.ShowStartVmsGroupBox = m_pageXvaStorage.ImportedVm != null && !m_pageXvaStorage.ImportedVm.is_a_template;
                m_pageXvaNetwork.VM = m_pageXvaStorage.ImportedVm;
                NotifyNextPagesOfChange(m_pageXvaNetwork);
            }

            if (type != typeof(ImportFinishPage))
                NotifyNextPagesOfChange(m_pageFinish);
        }

        protected override string WizardPaneHelpID()
        {
            var curPageType = CurrentStepTabPage.GetType();

            if (curPageType == typeof(RBACWarningPage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("RbacImportOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("RbacImportImage");
                    case ImportType.Xva:
                        return FormatHelpId("RbacImportXva");
                }
            }
            if (curPageType == typeof(ImportSelectHostPage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("SelectHostOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("SelectHostImage");
                }
            }
            if (curPageType == typeof(ImportSelectStoragePage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("SelectStorageOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("SelectStorageImage");
                }
            }
            if (curPageType == typeof(ImportSelectNetworkPage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("SelectNetworkOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("SelectNetworkImage");
                }
            }
            if (curPageType == typeof(ImportOptionsPage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("ImportOptionsOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("ImportOptionsImage");
                }
            }

            if (curPageType == typeof(ImportFinishPage))
            {
                switch (m_typeOfImport)
                {
                    case ImportType.Ovf:
                        return FormatHelpId("ImportFinishOvf");
                    case ImportType.Vhd:
                        return FormatHelpId("ImportFinishImage");
                    case ImportType.Xva:
                        return FormatHelpId("ImportFinishXva");
                }
            }
            return base.WizardPaneHelpID();
        }

        #endregion

        #region Private methods

        private void ConfigureRbacPage(IXenConnection selectedConnection)
        {
            if(!Helpers.ConnectionRequiresRbac(selectedConnection)){
                return;
            }

            m_ignoreAffinitySet = false;

            switch (m_typeOfImport)
            {
                case ImportType.Ovf:
                case ImportType.Vhd:
                    {
                        var check = m_typeOfImport == ImportType.Ovf
                                        ? new WizardRbacCheck(Messages.RBAC_WARNING_IMPORT_WIZARD_APPLIANCE) {Blocking = true}
                                        : new WizardRbacCheck(Messages.RBAC_WARNING_IMPORT_WIZARD_IMAGE) {Blocking = true};
                        check.AddApiMethods(ApplianceAction.StaticRBACDependencies);
                        m_pageRbac.SetPermissionChecks(selectedConnection, check);

                        AddAfterPage(m_pageHost, m_pageRbac);
                    }
                    break;
                case ImportType.Xva:
                    {
                        //Check to see if they can import VMs at all
                        var importCheck = new WizardRbacCheck(Messages.RBAC_WARNING_IMPORT_WIZARD_XVA) {Blocking = true};
                        importCheck.AddApiMethods(ImportVmAction.ConstantRBACRequirements);
                        importCheck.AddApiMethods("sr.scan");//CA-337323

                        //Check to see if they can set the VM's affinity
                        var affinityCheck = new WizardRbacCheck(Messages.RBAC_WARNING_IMPORT_WIZARD_AFFINITY);
                        affinityCheck.AddApiMethods("vm.set_affinity");
                        affinityCheck.WarningAction = () =>
                        {
                            //We cannot allow them to set the affinity, so we are only going
                            //to offer them the choice of connection, not specific host
                            m_ignoreAffinitySet = true;
                        };
                        m_pageRbac.SetPermissionChecks(selectedConnection, importCheck, affinityCheck);
                        AddAfterPage(m_pageXvaHost, m_pageRbac);
                    }
                    break;
            }

            //set page Connection after the page has been added to the wizard
            //(because the Connection is reset when the page is added
            m_pageRbac.Connection = selectedConnection;
        }

        private void CheckDisabledPages(params XenTabPage[] pages)
        {
            foreach (var p in pages)
                p.CheckPageDisabled();
        }

        protected override IEnumerable<Tuple> GetSummary()
        {
            switch (m_typeOfImport)
            {
                case ImportType.Xva:
                    return GetSummaryXva();
                case ImportType.Ovf:
                    return GetSummaryOvf();
                case ImportType.Vhd:
                    return GetSummaryVhd();
                default:
                    return new List<Tuple>();
            }
        }

        private IEnumerable<Tuple> GetSummaryXva()
        {
            var temp = new List<Tuple>();
            temp.Add(new Tuple(Messages.FINISH_PAGE_VMNAME, m_pageXvaStorage.ImportedVm.Name()));
            temp.Add(new Tuple(Messages.FINISH_PAGE_TARGET, m_pageXvaHost.SelectedHost == null ? m_pageXvaHost.SelectedConnection.Name : m_pageXvaHost.SelectedHost.Name()));
            temp.Add(new Tuple(Messages.FINISH_PAGE_STORAGE, m_pageXvaStorage.SR.Name()));

            var con = m_pageXvaHost.SelectedHost == null ? m_pageXvaHost.SelectedConnection : m_pageXvaHost.SelectedHost.Connection;

            bool first = true;
            foreach (var vif in m_pageXvaNetwork.VIFs)
            {
                var netref = new XenRef<XenAPI.Network>(vif.network);
                var network = con.Resolve(netref);
                // CA-218956 - Expose HIMN when showing hidden objects
                if (network == null || (network.IsGuestInstallerNetwork() && !XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
                    continue;

                temp.Add(new Tuple(first ? Messages.FINISH_PAGE_NETWORK : "", network.Name()));
                first = false;
            }

            return temp;
        }

        private IEnumerable<Tuple> GetSummaryOvf()
        {
            var temp = new List<Tuple>();

            var appName = m_pageImportSource.SelectedOvfPackage.Name;
            temp.Add(new Tuple(Messages.FINISH_PAGE_REVIEW_APPLIANCE, appName));
            temp.Add(new Tuple(Messages.FINISH_PAGE_VERIFY_MANIFEST, m_pageSecurity.VerifyManifest.ToYesNoStringI18n()));
            temp.Add(new Tuple(Messages.FINISH_PAGE_VERIFY_SIGNATURE, m_pageSecurity.VerifySignature.ToYesNoStringI18n()));

            temp.Add(new Tuple(Messages.FINISH_PAGE_RUN_FIXUPS, m_pageOptions.RunFixups.ToYesNoStringI18n()));
            if (m_pageOptions.RunFixups)
                temp.Add(new Tuple(Messages.FINISH_PAGE_ISOSR, m_pageOptions.SelectedIsoSR.Name()));

            temp.AddRange(GetVmMappingsSummary());
            return temp;
        }

        private IEnumerable<Tuple> GetSummaryVhd()
        {
            var temp = new List<Tuple>();
            temp.Add(new Tuple(Messages.FINISH_PAGE_IMAGEPATH, m_pageImportSource.FilePath));
            temp.Add(new Tuple(Messages.FINISH_PAGE_VMNAME, m_pageVMconfig.VmName));
            temp.Add(new Tuple(Messages.FINISH_PAGE_CPUCOUNT, m_pageVMconfig.CpuCount.ToString()));
            temp.Add(new Tuple(Messages.FINISH_PAGE_MEMORY, string.Format(Messages.VAL_MB, m_pageVMconfig.Memory)));
            if (Helpers.NaplesOrGreater(_targetConnection))
                temp.Add(new Tuple(Messages.BOOT_MODE, m_pageBootOptions.SelectedBootMode.StringOf()));

            if (m_pageImportSource.IsWIM)
                temp.Add(new Tuple(Messages.FINISH_PAGE_ADDSPACE, Util.DiskSizeString(m_pageVMconfig.AdditionalSpace)));

            temp.Add(new Tuple(Messages.FINISH_PAGE_RUN_FIXUPS, m_pageOptions.RunFixups.ToYesNoStringI18n()));
            if (m_pageOptions.RunFixups)
                temp.Add(new Tuple(Messages.FINISH_PAGE_ISOSR, m_pageOptions.SelectedIsoSR.Name()));

            temp.AddRange(GetVmMappingsSummary());
            return temp;
        }

        private IEnumerable<Tuple> GetVmMappingsSummary()
        {
            var temp = new List<Tuple>();

            foreach (var mapping in m_vmMappings.Values)
            {
                var targetLbl = m_vmMappings.Count == 1 ? Messages.FINISH_PAGE_TARGET : string.Format(Messages.FINISH_PAGE_TARGET_FOR_VM, mapping.VmNameLabel);
                var storageLbl = m_vmMappings.Count == 1 ? Messages.FINISH_PAGE_STORAGE : string.Format(Messages.FINISH_PAGE_STORAGE_FOR_VM, mapping.VmNameLabel);
                var networkLbl = m_vmMappings.Count == 1 ? Messages.FINISH_PAGE_NETWORK : string.Format(Messages.FINISH_PAGE_NETWORK_FOR_VM, mapping.VmNameLabel);

                temp.Add(new Tuple(targetLbl, mapping.TargetName));
                bool first = true;
                foreach (var sr in mapping.Storage)
                {
                    temp.Add(new Tuple(first ? storageLbl : "", sr.Value.Name()));
                    first = false;
                }

                first = true;
                foreach (var net in mapping.Networks)
                {
                    temp.Add(new Tuple(first ? networkLbl : "", net.Value.Name()));
                    first = false;
                }
            }
            return temp;
        }

        private bool IsGUID(string expression)
        {
            if (expression != null)
            {
                Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

                return guidRegEx.IsMatch(expression);
            }
            return false;
        }

        // Find a name to use of a VM within an envelope that could have come from any hypervisor.
        // TODO: Consider refactoring this method because it is very similar to OVF.FindSystemName().
        private string FindVMName(EnvelopeType ovfEnv, string sysId)
        {
            VirtualSystem_Type vSystem = OVF.FindVirtualSystemById(ovfEnv, sysId);

            // Use the given name if present and valid.
            // The given name is Envelope.VirtualSystem.Name specified in the OVF Specification 1.1 clause 7.2.
            // XenServer sets the name property.
            // vSphere 4.1 and Virtual Box 4.0.6 do not.
            if ((Tools.ValidateProperty("Name", vSystem)) && !String.IsNullOrEmpty(vSystem.Name[0].Value))
                return vSystem.Name[0].Value;

            // The VM wasn't given a name.
            // Build a list of choices from various properties.
            var choices = new List<string>();

            // VirtualSystem.id is next preference because vSphere and Virtual Box typically set this property to the VM name.
            if (!string.IsNullOrEmpty(vSystem.id))
                choices.Add(vSystem.id);

            // VirtualHardwareSection_Type.VirtualSystemIdentifier is next preference because Virtual Box will also set this property to the VM name.
            VirtualHardwareSection_Type[] vhsList = OVF.FindVirtualHardwareSection(ovfEnv, sysId);

            foreach (VirtualHardwareSection_Type vhs in vhsList)
            {
                if (vhs == null || vhs.System == null)
                    continue;

                if (Tools.ValidateProperty("VirtualSystemIdentifier", vhs.System))
                    choices.Add(vhs.System.VirtualSystemIdentifier.Value);
            }

            // Operating system description is next preference.
            OperatingSystemSection_Type[] ossList = OVF.FindSections<OperatingSystemSection_Type>(vSystem.Items);

            foreach (OperatingSystemSection_Type oss in ossList)
            {
                if (Tools.ValidateProperty("Description", oss))
                    choices.Add(oss.Description.Value);
            }

            // Envelope name is the last preference for XenServer that can could be a path in some cases.
            // vSphere and Virtual Box usually don't set this property.
            choices.Add(Path.GetFileNameWithoutExtension(ovfEnv.Name));

            // Last preference is file name.
            choices.Add(Path.GetFileNameWithoutExtension(m_pageImportSource.SelectedOvfPackage.PackageSourceFile));

            // First choice is one that is not a GUID.
            foreach (var choice in choices)
            {
                if (!String.IsNullOrEmpty(choice) && !IsGUID(choice))
                    return choice;
            }

            // Second choice is the first GUID.
            foreach (var choice in choices)
            {
                if (!String.IsNullOrEmpty(choice))
                    return choice;
            }

            // Last resort is a new GUID.
            return Guid.NewGuid().ToString();
        }

        #endregion

        private void m_pageXvaStorage_ImportVmCompleted()
        {
            Program.Invoke(this, () =>
            {
                if (CurrentStepTabPage.GetType() == typeof(StoragePickerPage))
                {
                    m_pageFinish.ShowStartVmsGroupBox = m_pageXvaStorage.ImportedVm != null && !m_pageXvaStorage.ImportedVm.is_a_template;
                    m_pageXvaNetwork.VM = m_pageXvaStorage.ImportedVm;
                    NotifyNextPagesOfChange(m_pageXvaNetwork);
                    NextStep();
                }
            });
        }

        private void ShowXenAppXenDesktopWarning(IXenConnection connection)
        {
            if (connection != null && connection.Cache.Hosts.Any(h => h.DesktopFeaturesEnabled() || h.DesktopPlusFeaturesEnabled() || h.DesktopCloudFeaturesEnabled()))
            {
                var format = Helpers.GetPool(connection) != null
                    ? Messages.NEWVMWIZARD_XENAPP_XENDESKTOP_INFO_MESSAGE_POOL
                    : Messages.NEWVMWIZARD_XENAPP_XENDESKTOP_INFO_MESSAGE_SERVER;
                ShowInformationMessage(string.Format(format, BrandManager.CompanyNameShort));
            }
            else
                HideInformationMessage();
        }

        private void pageHost_ConnectionSelectionChanged(IXenConnection connection)
        {
            ShowXenAppXenDesktopWarning(connection);
        }

        #region Nested items

        /// <summary>
        /// Type of the object we want to import
        /// </summary>
        public enum ImportType
        {
            /// <summary>
            /// Exported VM or template; filetype *.xva, *.xva.gz
            /// </summary>
            Xva,
            /// <summary>
            /// Appliance; filetypes *.ovf, *.ova, *.ova.gz
            /// </summary>
            Ovf,
            /// <summary>
            /// Virtual disk image; filetypes *.vhd, *.vmdk (CA-61385: remove ".vdi", ".wim" support for Boston)
            /// </summary>
            Vhd
        }

        #endregion
    }
}
