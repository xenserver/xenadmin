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
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.Ballooning;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_CpuMem : XenTabPage
    {
        private VM _template;

        // number of spinners to show
        private enum MemoryMode
        {
            JustMemory = 1,
            MinimumAndMaximum = 2,
            MinimumMaximumAndStaticMax = 3
        }

        private MemoryMode _memoryMode = MemoryMode.JustMemory;

        private double _memoryRatio;  // the permitted ratio of dynamic_min / static_max
        private bool _initializing = true;
        private bool _isVCpuHotplugSupported;
        private int _minVCpus;
        private long _maxVCpus;
        private long _prevVCpusMax;
        public VM SelectedTemplate { private get; set; }

        // Please note that the comboBoxVCPUs control can represent two different VM properties, depending whether the VM supports vCPU hotplug or not: 
        // When vCPU hotplug is not supported, comboBoxVCPUs represents the initial number of vCPUs (VCPUs_at_startup). In this case we will also set the VM property VCPUs_max to the same value.
        // When vCPU hotplug is supported, comboBoxVCPUs represents the maximum number of vCPUs (VCPUs_max). And the initial number of vCPUs is represented in comboBoxInitialVCPUs (which is only visible in this case) 

        public Page_CpuMem()
        {
            InitializeComponent();
        }

        public override string Text => Messages.NEWVMWIZARD_CPUMEMPAGE_NAME;

        public override string PageTitle => Messages.NEWVMWIZARD_CPUMEMPAGE_TITLE;

        public override string HelpID => "CPU&Memory";

        public double SelectedMemoryDynamicMin => spinnerDynMin.Value;

        public double SelectedMemoryDynamicMax => _memoryMode == MemoryMode.JustMemory ? spinnerDynMin.Value : spinnerDynMax.Value;

        public bool CanStartVm => _maxVCpus > 0 && SelectedVCpusMax <= _maxVCpus;

        public double SelectedMemoryStaticMax =>
            _memoryMode == MemoryMode.JustMemory ? spinnerDynMin.Value :
            _memoryMode == MemoryMode.MinimumAndMaximum ? spinnerDynMax.Value :
            spinnerStatMax.Value;

        public long SelectedVCpusMax => (long)comboBoxVCPUs.SelectedItem;

        public long SelectedVCpusAtStartup => _isVCpuHotplugSupported ? (long)comboBoxInitialVCPUs.SelectedItem : (long)comboBoxVCPUs.SelectedItem;

        public long SelectedCoresPerSocket => comboBoxTopology.CoresPerSocket;

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (SelectedTemplate == _template)
                return;

            _initializing = true;

            _template = SelectedTemplate;
            if (_template.SupportsBallooning() && !Helpers.FeatureForbidden(_template, Host.RestrictDMC))
                _memoryMode = _template.memory_dynamic_max == _template.memory_static_max ? MemoryMode.MinimumAndMaximum : MemoryMode.MinimumMaximumAndStaticMax;
            else
                _memoryMode = MemoryMode.JustMemory;

            _memoryRatio = VMMemoryControlsEdit.GetMemoryRatio(_template);

            FreeSpinnerLimits();

            if (_memoryMode == MemoryMode.JustMemory)
            {
                spinnerDynMin.Initialize(_template.memory_static_max, _template.memory_static_max);
                labelDynMin.Text = Messages.MEMORY_COLON;
            }
            else
            {
                labelDynMin.Text = Messages.DYNAMIC_MIN_COLON;
                spinnerDynMin.Initialize(_template.memory_dynamic_min, _template.memory_static_max);
                FreeSpinnerLimits();  // same as CA-33831
                spinnerDynMax.Initialize(_template.memory_dynamic_max, _template.memory_static_max);
                if (_memoryMode == MemoryMode.MinimumMaximumAndStaticMax)
                {
                    FreeSpinnerLimits();
                    spinnerStatMax.Initialize(_template.memory_static_max, _template.memory_static_max);
                }
            }
            labelDynMaxInfo.Visible = labelDynMax.Visible = spinnerDynMax.Visible = _memoryMode == MemoryMode.MinimumAndMaximum || _memoryMode == MemoryMode.MinimumMaximumAndStaticMax;
            labelStatMaxInfo.Visible = labelStatMax.Visible = spinnerStatMax.Visible = _memoryMode == MemoryMode.MinimumMaximumAndStaticMax;

            _isVCpuHotplugSupported = _template.SupportsVcpuHotplug();
            _minVCpus = _template.MinVCPUs();

            _prevVCpusMax = _template.VCPUs_max;  // we use variable in RefreshCurrentVCPUs for checking if VcpusAtStartup and VcpusMax were equal before VcpusMax changed

            label5.Text = GetRubric();

            InitialiseVCpuControls();

            SetSpinnerLimitsAndIncrement();

            ValuesUpdated();

            _initializing = false;
        }

        public override void SelectDefaultControl()
        {
            comboBoxVCPUs.Select();
        }

        private void InitialiseVCpuControls()
        {
            labelVCPUs.Text = _isVCpuHotplugSupported
                ? Messages.VM_CPUMEMPAGE_MAX_VCPUS_LABEL
                : Messages.VM_CPUMEMPAGE_VCPUS_LABEL;

            labelInitialVCPUs.Text = Messages.VM_CPUMEMPAGE_INITIAL_VCPUS_LABEL;
            labelInitialVCPUs.Visible = comboBoxInitialVCPUs.Visible = _isVCpuHotplugSupported;

            comboBoxTopology.Populate(_template.VCPUs_at_startup, _template.VCPUs_max, _template.GetCoresPerSocket(), _template.MaxCoresPerSocket());
            PopulateVCpus(_template.MaxVCPUsAllowed(), _isVCpuHotplugSupported ? _template.VCPUs_max : _template.VCPUs_at_startup);

            if (_isVCpuHotplugSupported)
                PopulateVCpusAtStartup(_template.VCPUs_max, _template.VCPUs_at_startup);
        }

        private void PopulateVCpuComboBox(ComboBox comboBox, long min, long max, long currentValue, Predicate<long> isValid)
        {
            comboBox.BeginUpdate();
            comboBox.Items.Clear();
            for (var i = min; i <= max; ++i)
            {
                if (i == currentValue || isValid(i))
                    comboBox.Items.Add(i);
            }
            if (currentValue > max)
                comboBox.Items.Add(currentValue);
            comboBox.SelectedItem = currentValue;
            comboBox.EndUpdate();
        }

        private void PopulateVCpus(long maxVCpus, long currentVCpus)
        {
            PopulateVCpuComboBox(comboBoxVCPUs, 1, maxVCpus, currentVCpus, i => comboBoxTopology.IsValidVCPU(i));
        }

        private void PopulateVCpusAtStartup(long max, long currentValue)
        {
            PopulateVCpuComboBox(comboBoxInitialVCPUs, 1, max, currentValue, i => true);
        }

        private string GetRubric()
        {
            var sb = new StringBuilder();
            sb.Append(Messages.NEWVMWIZARD_CPUMEMPAGE_RUBRIC);
            // add hotplug text
            if (_isVCpuHotplugSupported)
                sb.Append(Messages.VM_CPUMEMPAGE_RUBRIC_HOTPLUG);
            return sb.ToString();
        }

        // Return the larger of the template's MaxMemAllowed and the template's static max,
        // to avoid crashes in the spinners (CA-40041).
        private long MaxMemAllowed
        {
            get
            {
                var msm = _template.memory_static_max;
                var mma = _template.MaxMemAllowed();
                return (msm > mma ? msm : mma);
            }
        }

        private void FreeSpinnerLimits()
        {
            var maxMemAllowed = MaxMemAllowed;
            spinnerDynMin.SetRange(0, maxMemAllowed);
            spinnerDynMax.SetRange(0, maxMemAllowed);
            spinnerStatMax.SetRange(0, maxMemAllowed);
        }

        private void SetSpinnerLimitsAndIncrement()
        {
            spinnerDynMin.Increment = spinnerDynMax.Increment = spinnerStatMax.Increment = VMMemoryControlsEdit.CalcIncrement(_template.memory_static_max, spinnerDynMin.Units);

            var maxMemAllowed = MaxMemAllowed;
            double min = _template.memory_static_min;
            if (_memoryMode == MemoryMode.JustMemory)
            {
                spinnerDynMin.SetRange(min, maxMemAllowed);
                ShowMemoryMinMaxInformation(labelDynMinInfo, min, maxMemAllowed);
                return;
            }
            var min2 = (long)(SelectedMemoryStaticMax * _memoryRatio);
            if (min < min2)
                min = min2;
            var max = SelectedMemoryDynamicMax;
            if (max < min)
                max = min;
            spinnerDynMin.SetRange(min, max);
            ShowMemoryMinMaxInformation(labelDynMinInfo, min, max);

            spinnerDynMax.SetRange(SelectedMemoryDynamicMin,
                _memoryMode == MemoryMode.MinimumAndMaximum ? maxMemAllowed : SelectedMemoryStaticMax);
            ShowMemoryMinMaxInformation(labelDynMaxInfo, SelectedMemoryDynamicMin,
                _memoryMode == MemoryMode.MinimumAndMaximum ? maxMemAllowed : SelectedMemoryStaticMax);

            spinnerStatMax.SetRange(SelectedMemoryDynamicMax, maxMemAllowed);
            ShowMemoryMinMaxInformation(labelStatMaxInfo, SelectedMemoryDynamicMax, maxMemAllowed);
        }

        public void DisableMemoryControls()
        {
            spinnerDynMin.Enabled = false;
            spinnerDynMax.Enabled = false;
            spinnerStatMax.Enabled = false;
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                var sum = new List<KeyValuePair<string, string>>();

                if (_isVCpuHotplugSupported)
                {
                    sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_MAX_VCPUS, SelectedVCpusMax.ToString()));
                    sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_INITIAL_VCPUS, SelectedVCpusAtStartup.ToString()));
                }
                else
                {
                    sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_VCPUS, SelectedVCpusAtStartup.ToString()));
                }
                sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_TOPOLOGY, comboBoxTopology.Text));
                if (_memoryMode == MemoryMode.JustMemory)
                    sum.Add(new KeyValuePair<string, string>(Messages.MEMORY, Util.MemorySizeStringSuitableUnits(SelectedMemoryStaticMax, false)));
                else
                {
                    sum.Add(new KeyValuePair<string, string>(Messages.DYNAMIC_MIN, Util.MemorySizeStringSuitableUnits(SelectedMemoryDynamicMin, false)));
                    sum.Add(new KeyValuePair<string, string>(Messages.DYNAMIC_MAX, Util.MemorySizeStringSuitableUnits(SelectedMemoryDynamicMax, false)));
                    if (_memoryMode == MemoryMode.MinimumMaximumAndStaticMax)
                        sum.Add(new KeyValuePair<string, string>(Messages.STATIC_MAX, Util.MemorySizeStringSuitableUnits(SelectedMemoryStaticMax, false)));
                }
                return sum;
            }
        }

        private void ValuesUpdated()
        {
            CheckForError();
            OnPageUpdated();
        }

        private void CheckForError()
        {
            long maxMemTotal = 0;
            long maxMemFree = 0;
            Host maxMemTotalHost = null;
            Host maxMemFreeHost = null;
            Host maxVcpusHost = null;
            _maxVCpus = 0;
            foreach (var host in Connection.Cache.Hosts)
            {
                long hostCpus = 0;

                foreach (var cpu in Connection.Cache.Host_cpus)
                {
                    if (cpu.host.opaque_ref.Equals(host.opaque_ref))
                        hostCpus++;
                }

                var metrics = Connection.Resolve(host.metrics);

                if (hostCpus > _maxVCpus)
                {
                    _maxVCpus = hostCpus;
                    maxVcpusHost = host;
                }

                if (metrics != null && metrics.memory_total > maxMemTotal)
                {
                    maxMemTotal = metrics.memory_total;
                    maxMemTotalHost = host;
                }

                // The available memory of a server is taken to be the current memory_free,
                // plus however much we can squeeze down the existing VMs. This assumes
                // that the overhead won't increase when we create the new VM, so it
                // has false negatives.
                var memoryFree = host.memory_available_calc();

                if (metrics != null && memoryFree > maxMemFree)
                {
                    maxMemFree = memoryFree;
                    maxMemFreeHost = host;
                }
            }

            if (maxMemTotalHost != null && SelectedMemoryDynamicMin > maxMemTotal)
            {
                ShowMemoryWarning(string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_MEMORYWARN1, Helpers.GetName(maxMemTotalHost).Ellipsise(50), Util.MemorySizeStringSuitableUnits(maxMemTotal, false)));
            }
            else if (maxMemFreeHost != null && SelectedMemoryDynamicMin > maxMemFree)
            {
                ShowMemoryWarning(string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_MEMORYWARN2,
                    Helpers.GetName(maxMemFreeHost).Ellipsise(50),
                    Util.MemorySizeStringSuitableUnits(maxMemFree, false)));
            }
            else
            {
                ShowMemoryWarning();
            }
            
            if (maxVcpusHost != null && SelectedVCpusMax > _maxVCpus)
            {
                ShowCpuWarning(string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_VCPUSWARN, Helpers.GetName(maxVcpusHost).Ellipsise(50), _maxVCpus));
            }
            else if (SelectedVCpusMax > VM.MAX_VCPUS_FOR_NON_TRUSTED_VMS)
            {
                ShowCpuWarning(string.Format(Messages.VCPUS_UNTRUSTED_VM_WARNING, VM.MAX_VCPUS_FOR_NON_TRUSTED_VMS, BrandManager.ProductBrand));
            }
            else
            {
                ShowCpuWarning();
            }
        }

        private void ShowMemoryWarning(string text = null)
        {
            var show = !string.IsNullOrEmpty(text);

            memoryWarningLabel.Text = show ? text : null;
            memoryPictureBox.Visible = memoryWarningLabel.Visible = show;
        }

        private void ShowCpuWarning(string text = null)
        {
            var show = !string.IsNullOrEmpty(text);
            cpuWarningLabel.Text = show ? text : null;
            cpuWarningPictureBox.Visible = cpuWarningLabel.Visible = show;
        }

        private void ShowMemoryMinMaxInformation(Label label, double min, double max)
        {
            label.Text = string.Format(
                Messages.NEWVMWIZARD_CPUMEMPAGE_MEMORYINFO,
                FormatMemory(min),
                FormatMemory(max));
        }

        private string FormatMemory(double numberOfBytes)
        {
            return Util.MemorySizeStringSuitableUnits(numberOfBytes, true);
        }

        private void vCPU_ValueChanged(object sender, EventArgs e)
        {
            comboBoxTopology.Update((long)comboBoxVCPUs.SelectedItem);
            ValuesUpdated();
            ValidateVCpuSettings();
            RefreshCurrentVCpus();
        }

        private void memory_ValueChanged(object sender, EventArgs e)
        {
            if (_initializing)
                return;

            SetSpinnerLimitsAndIncrement();
            ValuesUpdated();
        }

        private void ValidateVCpuSettings()
        {
            if (comboBoxVCPUs.SelectedItem != null && SelectedVCpusMax < _minVCpus)
            {
                vCPUWarningLabel.Text = string.Format(Messages.VM_CPUMEMPAGE_VCPU_MIN_WARNING, _minVCpus);
                vCPUWarningLabel.Visible = true;
            }
            else
            {
                vCPUWarningLabel.Visible = false;
            }

            if (comboBoxInitialVCPUs.SelectedItem != null && SelectedVCpusAtStartup < _minVCpus)
            {
                initialVCPUWarningLabel.Text = string.Format(Messages.VM_CPUMEMPAGE_VCPU_MIN_WARNING, _minVCpus);
                initialVCPUWarningLabel.Visible = true;
            }
            else
            {
                initialVCPUWarningLabel.Visible = false;
            }
        }

        private void ValidateTopologySettings()
        {
            if (comboBoxVCPUs.SelectedItem != null)
                labelInvalidVCPUWarning.Text = VM.ValidVCPUConfiguration((long)comboBoxVCPUs.SelectedItem, comboBoxTopology.CoresPerSocket);
        }

        private void RefreshCurrentVCpus()
        {
            // refresh comboBoxInitialVCPUs if it's visible and populated
            if (comboBoxInitialVCPUs.Visible && comboBoxInitialVCPUs.Items.Count > 0)
            {
                // VcpusAtStartup is always <= VcpusMax
                // So if VcpusMax is decreased below VcpusAtStartup, then VcpusAtStartup is decreased to that number too
                // If VcpusAtStartup and VcpusMax are equal, and VcpusMax is changed, then VcpusAtStartup is changed to match
                // But if the numbers are unequal, and VcpusMax is changed but is still higher than VcpusAtStartup, then VcpusAtStartup is unchanged
                var newValue = SelectedVCpusAtStartup;

                if (SelectedVCpusMax < SelectedVCpusAtStartup)
                    newValue = SelectedVCpusMax;
                else if (SelectedVCpusAtStartup == _prevVCpusMax && SelectedVCpusMax != _prevVCpusMax)
                    newValue = SelectedVCpusMax;

                PopulateVCpusAtStartup(SelectedVCpusMax, newValue);
                _prevVCpusMax = SelectedVCpusMax;
            }
        }

        private void comboBoxTopology_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateTopologySettings();
        }

        private void comboBoxInitialVCPUs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateVCpuSettings();
        }
    }
}
