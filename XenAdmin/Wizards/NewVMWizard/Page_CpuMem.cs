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
    public partial class PageCpuMem : XenTabPage
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

        private double _memoryRatio = 0.0;  // the permitted ratio of dynamic_min / static_max
        private bool _initialising = true;
        private bool _isVcpuHotplugSupported;
        private int _minVcpUs;

        // Please note that the comboBoxVCPUs control can represent two different VM properties, depending whether the VM supports vCPU hotplug or not: 
        // When vCPU hotplug is not supported, comboBoxVCPUs represents the initial number of vCPUs (VCPUs_at_startup). In this case we will also set the VM property VCPUs_max to the same value.
        // When vCPU hotplug is supported, comboBoxVCPUs represents the maximum number of vCPUs (VCPUs_max). And the initial number of vCPUs is represented in comboBoxInitialVCPUs (which is only visible in this case) 

        public PageCpuMem()
        {
            InitializeComponent();
        }

        public override string Text => Messages.NEWVMWIZARD_CPUMEMPAGE_NAME;

        public override string PageTitle => Messages.NEWVMWIZARD_CPUMEMPAGE_TITLE;

        public override string HelpID => "CPU&Memory";

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (SelectedTemplate ==  _template)
                return;

            _initialising = true;

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

            _isVcpuHotplugSupported = _template.SupportsVcpuHotplug();
            _minVcpUs = _template.MinVCPUs();

            _prevVcpUsMax = _template.VCPUs_max;  // we use variable in RefreshCurrentVCPUs for checking if VcpusAtStartup and VcpusMax were equal before VcpusMax changed

            label5.Text = GetRubric();

            InitialiseVcpuControls();

            SetSpinnerLimitsAndIncrement();

            ValuesUpdated();

            _initialising = false;
        }

        public override void SelectDefaultControl()
        {
            comboBoxVCPUs.Select();
        }

        private void InitialiseVcpuControls()
        {
            labelVCPUs.Text = _isVcpuHotplugSupported
                ? Messages.VM_CPUMEMPAGE_MAX_VCPUS_LABEL
                : Messages.VM_CPUMEMPAGE_VCPUS_LABEL;

            labelInitialVCPUs.Text = Messages.VM_CPUMEMPAGE_INITIAL_VCPUS_LABEL;
            labelInitialVCPUs.Visible = comboBoxInitialVCPUs.Visible = _isVcpuHotplugSupported;

            comboBoxTopology.Populate(_template.VCPUs_at_startup, _template.VCPUs_max, _template.GetCoresPerSocket(), _template.MaxCoresPerSocket());
            PopulateVcpUs(_template.MaxVCPUsAllowed(), _isVcpuHotplugSupported ? _template.VCPUs_max : _template.VCPUs_at_startup);

            if (_isVcpuHotplugSupported)
                PopulateVcpUsAtStartup(_template.VCPUs_max, _template.VCPUs_at_startup);
        }

        private void PopulateVcpuComboBox(ComboBox comboBox, long min, long max, long currentValue, Predicate<long> isValid)
        {
            comboBox.BeginUpdate();
            comboBox.Items.Clear();
            for (long i = min; i <= max; ++i)
            {
                if (i == currentValue || isValid(i))
                    comboBox.Items.Add(i);
            }
            if (currentValue > max)
                comboBox.Items.Add(currentValue);
            comboBox.SelectedItem = currentValue;
            comboBox.EndUpdate();
        }

        private void PopulateVcpUs(long maxVcpUs, long currentVcpUs)
        {
            PopulateVcpuComboBox(comboBoxVCPUs, 1, maxVcpUs, currentVcpUs, i => comboBoxTopology.IsValidVCPU(i));
        }

        private void PopulateVcpUsAtStartup(long max, long currentValue)
        {
            PopulateVcpuComboBox(comboBoxInitialVCPUs, 1, max, currentValue, i => true);
        }

        private string GetRubric()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Messages.NEWVMWIZARD_CPUMEMPAGE_RUBRIC);
            // add hotplug text
            if (_isVcpuHotplugSupported)
                sb.Append(Messages.VM_CPUMEMPAGE_RUBRIC_HOTPLUG);
            return sb.ToString();
        }

        public VM SelectedTemplate { private get; set; }

        // Return the larger of the template's MaxMemAllowed and the template's static max,
        // to avoid crashes in the spinners (CA-40041).
        private long MaxMemAllowed
        {
            get
            {
                long msm = _template.memory_static_max;
                long mma = _template.MaxMemAllowed();
                return (msm > mma ? msm : mma);
            }
        }

        private void FreeSpinnerLimits()
        {
            long maxMemAllowed = MaxMemAllowed;
            spinnerDynMin.SetRange(0, maxMemAllowed);
            spinnerDynMax.SetRange(0, maxMemAllowed);
            spinnerStatMax.SetRange(0, maxMemAllowed);
        }

        private void SetSpinnerLimitsAndIncrement()
        {
            spinnerDynMin.Increment = spinnerDynMax.Increment = spinnerStatMax.Increment = VMMemoryControlsEdit.CalcIncrement(_template.memory_static_max, spinnerDynMin.Units);
            
            long maxMemAllowed = MaxMemAllowed;
            double min = _template.memory_static_min;
            if (_memoryMode == MemoryMode.JustMemory)
            {
                spinnerDynMin.SetRange(min, maxMemAllowed);
                ShowMemoryMinMaxInformation(labelDynMinInfo, min, maxMemAllowed);
                return;
            }
            long min2 = (long)(SelectedMemoryStaticMax * _memoryRatio);
            if (min < min2)
                min = min2;
            double max = SelectedMemoryDynamicMax;
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

        public double SelectedMemoryDynamicMin => spinnerDynMin.Value;

        public double SelectedMemoryDynamicMax => _memoryMode == MemoryMode.JustMemory ? spinnerDynMin.Value : spinnerDynMax.Value;

        public double SelectedMemoryStaticMax =>
            _memoryMode == MemoryMode.JustMemory ? spinnerDynMin.Value :
            _memoryMode == MemoryMode.MinimumAndMaximum ? spinnerDynMax.Value :
            spinnerStatMax.Value;

        public long SelectedVcpusMax => (long)comboBoxVCPUs.SelectedItem;

        public long SelectedVcpusAtStartup => _isVcpuHotplugSupported ? (long)comboBoxInitialVCPUs.SelectedItem : (long)comboBoxVCPUs.SelectedItem;

        public long SelectedCoresPerSocket => comboBoxTopology.CoresPerSocket;

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> sum = new List<KeyValuePair<string, string>>();

                if (_isVcpuHotplugSupported)
                {
                    sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_MAX_VCPUS, SelectedVcpusMax.ToString()));
                    sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_INITIAL_VCPUS, SelectedVcpusAtStartup.ToString()));
                }
                else
                {
                    sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_VCPUS, SelectedVcpusAtStartup.ToString()));
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
            long maxVcpus = 0;
            Host maxMemTotalHost = null;
            Host maxMemFreeHost = null;
            Host maxVcpusHost = null;

            foreach (Host host in Connection.Cache.Hosts)
            {
                long hostCpus = 0;

                foreach (Host_cpu cpu in Connection.Cache.Host_cpus)
                {
                    if (cpu.host.opaque_ref.Equals(host.opaque_ref))
                        hostCpus++;
                }

                Host_metrics metrics = Connection.Resolve(host.metrics);

                if (hostCpus > maxVcpus)
                {
                    maxVcpus = hostCpus;
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
                long memoryFree = host.memory_available_calc();

                if (metrics != null && memoryFree > maxMemFree)
                {
                    maxMemFree = memoryFree;
                    maxMemFreeHost = host;
                }
            }

            if (maxMemTotalHost != null && SelectedMemoryDynamicMin > maxMemTotal)
            {
                ErrorPanel.Visible = true;
                ErrorLabel.Text = string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_MEMORYWARN1, Helpers.GetName(maxMemTotalHost).Ellipsise(50), Util.MemorySizeStringSuitableUnits(maxMemTotal, false));
            }
            else if (maxMemFreeHost != null && SelectedMemoryDynamicMin > maxMemFree)
            {
                ErrorPanel.Visible = true;
                ErrorLabel.Text = string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_MEMORYWARN2, Helpers.GetName(maxMemFreeHost).Ellipsise(50), Util.MemorySizeStringSuitableUnits(maxMemFree, false));
            }
            else if (maxVcpusHost != null && SelectedVcpusMax > maxVcpus)
            {
                ErrorPanel.Visible = true;
                ErrorLabel.Text = string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_VCPUSWARN, Helpers.GetName(maxVcpusHost).Ellipsise(50), maxVcpus);
            }
            else
            {
                ErrorPanel.Visible = false;
            }
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
            ValidateVcpuSettings();
            RefreshCurrentVcpUs();
        }

        private void memory_ValueChanged(object sender, EventArgs e)
        {
            if (_initialising)
                return;

            SetSpinnerLimitsAndIncrement();
            ValuesUpdated();
        }
        
        private void ValidateVcpuSettings()
        {
            if (comboBoxVCPUs.SelectedItem != null && SelectedVcpusMax < _minVcpUs)
            {
                vCPUWarningLabel.Text = string.Format(Messages.VM_CPUMEMPAGE_VCPU_MIN_WARNING, _minVcpUs);
                vCPUWarningLabel.Visible = true;
            }
            else
            {
                vCPUWarningLabel.Visible = false;
            }

            if (comboBoxInitialVCPUs.SelectedItem != null && SelectedVcpusAtStartup < _minVcpUs)
            {
                initialVCPUWarningLabel.Text = string.Format(Messages.VM_CPUMEMPAGE_VCPU_MIN_WARNING, _minVcpUs);
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

        private long _prevVcpUsMax;

        private void RefreshCurrentVcpUs()
        {
            // refresh comboBoxInitialVCPUs if it's visible and populated
            if (comboBoxInitialVCPUs.Visible && comboBoxInitialVCPUs.Items.Count > 0)
            {
                // VcpusAtStartup is always <= VcpusMax
                // So if VcpusMax is decreased below VcpusAtStartup, then VcpusAtStartup is decreased to that number too
                // If VcpusAtStartup and VcpusMax are equal, and VcpusMax is changed, then VcpusAtStartup is changed to match
                // But if the numbers are unequal, and VcpusMax is changed but is still higher than VcpusAtStartup, then VcpusAtStartup is unchanged
                var newValue = SelectedVcpusAtStartup;

                if (SelectedVcpusMax < SelectedVcpusAtStartup)
                    newValue = SelectedVcpusMax;
                else if (SelectedVcpusAtStartup == _prevVcpUsMax && SelectedVcpusMax != _prevVcpUsMax)
                    newValue = SelectedVcpusMax;

                PopulateVcpUsAtStartup(SelectedVcpusMax, newValue);
                _prevVcpUsMax = SelectedVcpusMax;
            }
        }

        private void comboBoxTopology_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateTopologySettings();
        }

        private void comboBoxInitialVCPUs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateVcpuSettings();
        }
    }
}
