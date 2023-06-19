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
using System.Text.RegularExpressions;
using XenAdmin.Actions.OvfActions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Wizards.GenericPages;
using XenAdmin.Wizards.ImportWizard.Filters;
using XenAPI;
using XenOvf;
using XenOvf.Definitions;


namespace XenAdmin.Wizards.ImportWizard
{
    class ImportSelectHostPage : SelectMultipleVMDestinationPage
    {
        private EnvelopeType _selectedOvfEnvelope;
        private List<Xen_ConfigurationSettingData_Type> vgpuSettings = new List<Xen_ConfigurationSettingData_Type>();
        private List<Xen_ConfigurationSettingData_Type> hardwarePlatformSettings = new List<Xen_ConfigurationSettingData_Type>();
        private List<Xen_ConfigurationSettingData_Type> vendorDeviceSettings = new List<Xen_ConfigurationSettingData_Type>();
        private int _ovfMaxVCpusCount;
        private long _ovfMemory;
        private readonly List<long> _ovfVCpusCount;
        public event Action<IXenConnection> ConnectionSelectionChanged;

        public ImportSelectHostPage()
        {
            InitializeText();
            ShowWarning(null);
            _ovfVCpusCount = new List<long>();
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

                _ovfVCpusCount.Clear();
                _ovfMaxVCpusCount = 0;
                foreach (var vsType in vsColl.Content)
                {
                    var vhs = OVF.FindVirtualHardwareSectionByAffinity(SelectedOvfEnvelope, vsType.id, "xen");
                    var data = vhs.VirtualSystemOtherConfigurationData;
                    if (data == null)
                        continue;

                    foreach (var rasdType in vhs.Item)
                    {
                        // Processor
                        if (rasdType.ResourceType.Value == 3 &&
                            int.TryParse(rasdType.VirtualQuantity.Value.ToString(), out var vCpusCount))
                        {
                            _ovfVCpusCount.Add(vCpusCount);
                            if (_ovfMaxVCpusCount < vCpusCount)
                            {
                                _ovfMaxVCpusCount = vCpusCount;
                            }
                        }
                        // Memory
                        if (rasdType.ResourceType.Value == 4 && double.TryParse(rasdType.VirtualQuantity.Value.ToString(), out var memory))
                        {
                            //The default memory unit is MB (2^20), however, the RASD may contain a different
                            //one with format byte*memoryBase^memoryPower (byte being a literal string)

                            double memoryBase = 2.0;
                            double memoryPower = 20.0;

                            if (rasdType.AllocationUnits.Value.ToLower().StartsWith("byte"))
                            {
                                string[] a1 = rasdType.AllocationUnits.Value.Split('*', '^');

                                if (a1.Length == 3)
                                {
                                    if (!double.TryParse(a1[1].Trim(), out memoryBase))
                                        memoryBase = 2.0;
                                    if (!double.TryParse(a1[2].Trim(), out memoryPower))
                                        memoryPower = 20.0;
                                }
                            }

                            double memoryMultiplier = Math.Pow(memoryBase, memoryPower);
                            memory *= memoryMultiplier;

                            if (memory > long.MaxValue)
                                memory = long.MaxValue;

                            if (_ovfMemory < memory)
                                _ovfMemory = (long)memory;
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

        protected override void OnSelectedTargetPoolChanged()
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

                var ovfCountsAboveLimit = _ovfVCpusCount.Count(vCpusCount => vCpusCount > VM.MAX_VCPUS_FOR_NON_TRUSTED_VMS);
                if (ovfCountsAboveLimit > 0)
                {
                    warnings.Add(string.Format(Messages.IMPORT_VM_CPUS_COUNT_UNTRUSTED_WARNING, ovfCountsAboveLimit, VM.MAX_VCPUS_FOR_NON_TRUSTED_VMS, BrandManager.BrandConsole));
                }
            }

            ApplianceCanBeStarted = true;

            if (!CheckDestinationHasEnoughPhysicalCpus(out var physicalCpusWarningMessage, out var applianceCanBeStarted))
            {
                warnings.Add(physicalCpusWarningMessage);
                ApplianceCanBeStarted = applianceCanBeStarted;
            }

            if (!CheckDestinationHasEnoughMemory(out var memoryWarningMessage))
            {
                warnings.Add(memoryWarningMessage);
                ApplianceCanBeStarted = false;
            }

            ShowWarning(string.Join("\n\n", warnings));

            ConnectionSelectionChanged?.Invoke(SelectedTargetPool?.Connection);
        }

        protected override void OnSelectedTargetChanged()
        {
            OnSelectedTargetPoolChanged();
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

        /// <summary>
        /// Check if the appliance can be started on the selected host or pool. Note that if the user selects
        /// a shared SR in other pages, the VM could still start. The check considers both vCPU count and memory requirements
        /// of the appliance.
        /// </summary>
        public bool ApplianceCanBeStarted { get; private set; } = true;

        private bool CheckDestinationHasEnoughPhysicalCpus(out string warningMessage, out bool applianceCanBeStarted)
        {
            warningMessage = string.Empty;
            applianceCanBeStarted = false;
            
            // check if the current host selection
            // can accomodate the VMs in the appliance
            if (AllSelectedTargets.Count > 0)
            {
                var physicalCpus = AllSelectedTargets.Select(GetPhysicalCpus).ToList();
                var minPhysicalCpus = physicalCpus.Min();
                var maxPhysicalCpus = physicalCpus.Max();
               
                if (maxPhysicalCpus < _ovfMaxVCpusCount)
                {
                    // there are no hosts that can accomodate the VM with the largest amount of vCPUs
                    warningMessage = string.Format(Messages.IMPORT_WIZARD_CPUS_COUNT_MISMATCH_HOST_ALL, _ovfMaxVCpusCount, maxPhysicalCpus);
                    return false;
                }

                applianceCanBeStarted = true;
                if (minPhysicalCpus < _ovfMaxVCpusCount)
                {
                    // there is a host that cannot accomodate the VM with the largest amount of vCPUs
                    // we ask the user to make sure that the mapping they are picking is correct
                    warningMessage = string.Format(Messages.IMPORT_WIZARD_CPUS_COUNT_MISMATCH_HOST, _ovfMaxVCpusCount, minPhysicalCpus);
                    return false;
                }
            }

            // there is no host selection, check the pool
            var poolPhysicalCpus = GetPhysicalCpus(SelectedTargetPool);

            if (poolPhysicalCpus < 0 || poolPhysicalCpus >= _ovfMaxVCpusCount)
            {
                applianceCanBeStarted = true;
                return true;
            }
            
            warningMessage = string.Format(Messages.IMPORT_WIZARD_CPUS_COUNT_MISMATCH_POOL, _ovfMaxVCpusCount, poolPhysicalCpus);
            return false;
        }

       
        private bool CheckDestinationHasEnoughMemory(out string warningMessage)
        {
            warningMessage = string.Empty;

            var selectedTarget = SelectedTarget ?? SelectedTargetPool;
            var memory = GetFreeMemory(selectedTarget);

            if (memory >= _ovfMemory)
                return true;

            if (selectedTarget is Pool)
                warningMessage = string.Format(Messages.IMPORT_WIZARD_INSUFFICIENT_MEMORY_POOL, Util.MemorySizeStringSuitableUnits(_ovfMemory, true), Util.MemorySizeStringSuitableUnits(memory, true));
            else if (selectedTarget is Host)
                warningMessage = string.Format(Messages.IMPORT_WIZARD_INSUFFICIENT_MEMORY_HOST, Util.MemorySizeStringSuitableUnits(_ovfMemory, true), Util.MemorySizeStringSuitableUnits(memory, true));
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

        private long GetFreeMemory(IXenObject xenObject)
        {
            long memory = 0;

            switch (xenObject)
            {
                case Host host:
                {
                    var hostMemory = host.memory_available_calc();
                    if (hostMemory > 0)
                        memory = hostMemory;
                    break;
                }
                case Pool pool:
                {
                    var hosts = pool.Connection.Cache.Hosts;
                    var maxMemories = hosts
                        .Select(h => h.memory_available_calc())
                        .ToList();
                    if (maxMemories.Count > 0)
                    {
                        memory = maxMemories.Max();
                    }

                    break;
                }
            }

            return memory;
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
