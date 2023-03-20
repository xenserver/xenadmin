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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XenAdmin.Actions.OvfActions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Wizards.GenericPages;
using XenAdmin.Wizards.ImportWizard.Filters;
using XenAPI;
using XenOvf;
using XenOvf.Definitions;
using static XenAdmin.ServerDBs.Db;


namespace XenAdmin.Wizards.ImportWizard
{
    class ImportSelectHostPage : SelectMultipleVMDestinationPage
    {
        private EnvelopeType _selectedOvfEnvelope;
        private List<Xen_ConfigurationSettingData_Type> vgpuSettings = new List<Xen_ConfigurationSettingData_Type>();
        private List<Xen_ConfigurationSettingData_Type> hardwarePlatformSettings = new List<Xen_ConfigurationSettingData_Type>();
        private List<Xen_ConfigurationSettingData_Type> vendorDeviceSettings = new List<Xen_ConfigurationSettingData_Type>();
        private int? _ovfMaxVCpusCount;
        public event Action<IXenConnection> ConnectionSelectionChanged;

        public ImportSelectHostPage()
        {
            InitializeText();
        }

        #region XenTabPage overrides

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle => Messages.IMPORT_SELECT_HOST_PAGE_TITLE;

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text => Messages.NEWSR_LOCATION;

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        #endregion

        public EnvelopeType SelectedOvfEnvelope
        {
            private get
            {
                return _selectedOvfEnvelope;
            }
            set
            {
                _selectedOvfEnvelope = value;

                vgpuSettings.Clear();
                hardwarePlatformSettings.Clear();
                vendorDeviceSettings.Clear();

                if (_selectedOvfEnvelope == null)
                    return;

                var vsColl = SelectedOvfEnvelope.Item as VirtualSystemCollection_Type;

                if (vsColl == null && SelectedOvfEnvelope.Item is VirtualSystemCollection_Type)
                    vsColl = new VirtualSystemCollection_Type {Content = new[] {SelectedOvfEnvelope.Item}};

                if (vsColl == null)
                    return;

                foreach (var vsType in vsColl.Content)
                {
                    var vhs = OVF.FindVirtualHardwareSectionByAffinity(SelectedOvfEnvelope, vsType.id, "xen");
                    var data = vhs.VirtualSystemOtherConfigurationData;
                    if (data == null)
                        continue;
                    
                    foreach (var rasdType in vhs.Item)
                    {
                        if (rasdType.ResourceType.Value == 3 &&
                            int.TryParse(rasdType.VirtualQuantity.Value.ToString(), out var vCpusCount) &&
                            (_ovfMaxVCpusCount == null || _ovfMaxVCpusCount < vCpusCount)
                            )
                        {
                            _ovfMaxVCpusCount = vCpusCount;
                        }
                    }

                    foreach (var s in data)
                    {
                        if (s.Name == "vgpu")
                            vgpuSettings.Add(s);
                        else if (s.Name == "hardware_platform_version")
                            hardwarePlatformSettings.Add(s);
                        else if (s.Name == "VM_has_vendor_device")
                            vendorDeviceSettings.Add(s);
                    }
                }
            }
        }

        protected override string InstructionText => Messages.IMPORT_WIZARD_DESTINATION_INSTRUCTIONS;

        protected override string TargetPoolText => Messages.IMPORT_WIZARD_DESTINATION_DESTINATION;

        protected override string TargetServerSelectionIntroText => Messages.IMPORT_WIZARD_DESTINATION_TABLE_INTRO;

        protected override void OnChosenItemChanged()
        {
            var warnings = new List<string>();

            if (SelectedTargetPool != null)
            {
                if (!CheckDestinationSupportsVendorDevice())
                {
                    //it shouldn't come to this as the hardware incompatibility filter
                    //will have already caught it but just to be on the safe side
                    if (VmMappings.Count == 1)
                        warnings.Add(Messages.IMPORT_VM_WITH_VENDOR_DEVICE_WARNING_ONE);
                    else if (VmMappings.Count > 1)
                        warnings.Add(Messages.IMPORT_VM_WITH_VENDOR_DEVICE_WARNING_MANY);
                }
                
                if (!CheckRightGpuExists())
                {
                    if (VmMappings.Count == 1)
                        warnings.Add(Messages.IMPORT_VM_WITH_VGPU_WARNING_ONE);
                    else if (VmMappings.Count > 1)
                        warnings.Add(Messages.IMPORT_VM_WITH_VGPU_WARNING_MANY);
                }

                if (!CheckDestinationHasEnoughPhysicalCpus(out var warningMessage))
                    warnings.Add(warningMessage);
            }

            ShowWarning(string.Join("\n", warnings));

            ConnectionSelectionChanged?.Invoke(SelectedTargetPool?.Connection);
        }

        protected override DelayLoadingOptionComboBoxItem CreateDelayLoadingOptionComboBoxItem(IXenObject xenItem)
        {
            var filters = new List<ReasoningFilter>
            {
                new HardwareCompatibilityFilter(xenItem, hardwarePlatformSettings)
            };
            return new DelayLoadingOptionComboBoxItem(xenItem, filters);
        }

        protected override List<ReasoningFilter> CreateTargetServerFilterList(IXenObject xenObject, List<string> vmOpaqueRefs)
        {
            var filters = new List<ReasoningFilter>();

            if (xenObject != null)
                filters.Add(new HardwareCompatibilityFilter(xenObject, hardwarePlatformSettings));

            return filters;
        }

        private bool CheckRightGpuExists()
        {
            foreach (var vgpuSetting in vgpuSettings)
            {
                Match m = ImportApplianceAction.VGPU_REGEX.Match(vgpuSetting.Value.Value);
                if (!m.Success)
                    continue;

                var types = m.Groups[1].Value.Split(';');

                var gpuGroup = SelectedTargetPool.Connection.Cache.GPU_groups.FirstOrDefault(g =>
                    g.GPU_types.Length == types.Length &&
                    g.GPU_types.Intersect(types).Count() == types.Length);

                if (gpuGroup == null)
                    return false;

                string vendorName = m.Groups[2].Value;
                string modelName = m.Groups[3].Value;

                var vgpuType = SelectedTargetPool.Connection.Cache.VGPU_types.FirstOrDefault(v =>
                    v.vendor_name == vendorName && v.model_name == modelName);

                if (vgpuType == null)
                    return false;
            }

            return true;
        }

        public bool ApplianceCanBeStarted => _ovfMaxVCpusCount == null ||
                                             GetPhysicalCpus(SelectedTarget ?? SelectedTargetPool) < 0 ||
                                             GetPhysicalCpus(SelectedTarget ?? SelectedTargetPool) >= _ovfMaxVCpusCount;

        private bool CheckDestinationHasEnoughPhysicalCpus(out string warningMessage)
        {
            warningMessage = string.Empty;
            
            if (ApplianceCanBeStarted)
                return true;

            var selectedTarget = SelectedTarget ?? SelectedTargetPool;
            var physicalCpusCount = GetPhysicalCpus(selectedTarget);

            if (physicalCpusCount < 0 || physicalCpusCount >= _ovfMaxVCpusCount)
                return true;

            if (selectedTarget is Pool)
                warningMessage = string.Format(Messages.IMPORT_WIZARD_CPUS_COUNT_MISMATCH_POOL, _ovfMaxVCpusCount, physicalCpusCount);
            else if (selectedTarget is Host)
                warningMessage = string.Format(Messages.IMPORT_WIZARD_CPUS_COUNT_MISMATCH_HOST, _ovfMaxVCpusCount, physicalCpusCount);
            else
                return true;

            return false;
        }

        /// <summary>
        /// Returns the number of physical CPUs on the specified <paramref name="xenObject"/> (<see cref="Host"/> or <see cref="Pool"/>) or -1 if the value cannot be determined.
        /// </summary>
        /// <param name="xenObject">The XenObject for which to determine the number of physical CPUs.</param>
        /// <returns>The number of physical CPUs on the specified <paramref name="xenObject"/> or -1 if the value cannot be determined.</returns>
        private int GetPhysicalCpus(IXenObject xenObject)
        {
            var physicalCpusCount = -1;

            switch (xenObject)
            {
                case Host host:
                {
                    var hostCpuCount = host.CpuCount();
                    if(hostCpuCount > 0)
                        physicalCpusCount = hostCpuCount;
                    break;
                }
                case Pool pool:
                {
                    var hosts = pool.Connection.Cache.Hosts;
                    var maxCpuCounts = hosts
                        .Select(h => h.CpuCount())
                        .ToList();
                    if (maxCpuCounts.Count > 0)
                    {
                        physicalCpusCount = maxCpuCounts.Max();
                    }

                    break;
                }
            }

            return physicalCpusCount;
        }

        private bool CheckDestinationSupportsVendorDevice()
        {
            var dundeeOrNewerHosts = Helpers.DundeeOrGreater(SelectedTargetPool.Connection) ? SelectedTargetPool.Connection.Cache.Hosts : new Host[] {};

            foreach (var setting in vendorDeviceSettings)
            {
                if (!bool.TryParse(setting.Value.Value, out var hasVendorDevice))
                    continue;
                
                if (hasVendorDevice && dundeeOrNewerHosts.Length == 0)
                    return false;
            }
            return true;
        }
    }
}
