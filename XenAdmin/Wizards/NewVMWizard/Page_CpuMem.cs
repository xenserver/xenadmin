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
using System.Text;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Controls;
using XenAdmin.Controls.Ballooning;
using XenAdmin.Core;


namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_CpuMem : XenTabPage
    {
        private VM Template;
        int memoryMode = 1;  // number of spinners to show: 1 for just "memory", 2 for "minimum" and "maximum", 3 for "static max" too
        double memoryRatio = 0.0;  // the permitted ratio of dynamic_min / static_max
        bool initialising = true;
        private bool isVcpuHotplugSupported;

        // Please note that the comboBoxVCPUs control can represent two different VM properties, depending whether the VM supports vCPU hotplug or not: 
        // When vCPU hotplug is not supported, comboBoxVCPUs represents the initial number of vCPUs (VCPUs_at_startup). In this case we will also set the VM property VCPUs_max to the same value.
        // When vCPU hotplug is supported, comboBoxVCPUs represents the maximum number of vCPUs (VCPUs_max). And the initial number of vCPUs is represented in comboBoxInitialVCPUs (which is only visible in this case) 

        public Page_CpuMem()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get { return Messages.NEWVMWIZARD_CPUMEMPAGE_NAME; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMWIZARD_CPUMEMPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "CPU&Memory"; }
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (SelectedTemplate ==  Template)
                return;

            initialising = true;

            Template = SelectedTemplate;
            if (Template.has_ballooning && !Helpers.FeatureForbidden(Template, Host.RestrictDMC))
                memoryMode = (Template.memory_dynamic_max == Template.memory_static_max ? 2 : 3);
            else
                memoryMode = 1;

            memoryRatio = VMMemoryControlsEdit.GetMemoryRatio(Template);

            FreeSpinnerLimits();

            if (memoryMode == 1)
            {
                spinnerDynMin.Initialize(Messages.MEMORY_COLON, null, Template.memory_static_max, Template.memory_static_max);
                spinnerDynMax.Visible = spinnerStatMax.Visible = false;
            }
            else
            {
                spinnerDynMax.Visible = true;
                spinnerDynMin.Initialize(Messages.DYNAMIC_MIN_COLON, null, Template.memory_dynamic_min, Template.memory_static_max);
                FreeSpinnerLimits();  // same as CA-33831
                spinnerDynMax.Initialize(Messages.DYNAMIC_MAX_COLON, null, Template.memory_dynamic_max, Template.memory_static_max);
                if (memoryMode == 3)
                {
                    FreeSpinnerLimits();
                    spinnerStatMax.Initialize(Messages.STATIC_MAX_COLON, null, Template.memory_static_max, Template.memory_static_max);
                }
                else
                    spinnerStatMax.Visible = false;
            }

            isVcpuHotplugSupported = Template.SupportsVcpuHotplug;
            _prevVCPUsMax = Template.VCPUs_max;  // we use variable in RefreshCurrentVCPUs for checking if VcpusAtStartup and VcpusMax were equal before VcpusMax changed

            label5.Text = GetRubric();

            InitialiseVcpuControls();

            SetSpinnerLimitsAndIncrement();

            ValuesUpdated();

            initialising = false;
        }

        public override void SelectDefaultControl()
        {
            comboBoxVCPUs.Select();
        }

        private void InitialiseVcpuControls()
        {
            labelVCPUs.Text = isVcpuHotplugSupported
                ? Messages.VM_CPUMEMPAGE_MAX_VCPUS_LABEL
                : Messages.VM_CPUMEMPAGE_VCPUS_LABEL;

            labelInitialVCPUs.Text = Messages.VM_CPUMEMPAGE_INITIAL_VCPUS_LABEL;
            labelInitialVCPUs.Visible = comboBoxInitialVCPUs.Visible = isVcpuHotplugSupported;

            comboBoxTopology.Populate(Template.VCPUs_at_startup, Template.VCPUs_max, Template.CoresPerSocket, Template.MaxCoresPerSocket);
            PopulateVCPUs(Template.MaxVCPUsAllowed, isVcpuHotplugSupported ? Template.VCPUs_max : Template.VCPUs_at_startup);

            if (isVcpuHotplugSupported)
                PopulateVCPUsAtStartup(Template.VCPUs_max, Template.VCPUs_at_startup);
        }

        private void PopulateVCPUComboBox(ComboBox comboBox, long min, long max, long currentValue, Predicate<long> isValid)
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

        private void PopulateVCPUs(long maxVCPUs, long currentVCPUs)
        {
            PopulateVCPUComboBox(comboBoxVCPUs, 1, maxVCPUs, currentVCPUs, i => comboBoxTopology.IsValidVCPU(i));
        }

        private void PopulateVCPUsAtStartup(long max, long currentValue)
        {
            PopulateVCPUComboBox(comboBoxInitialVCPUs, 1, max, currentValue, i => true);
        }

        private string GetRubric()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Messages.NEWVMWIZARD_CPUMEMPAGE_RUBRIC);
            // add hotplug text
            if (isVcpuHotplugSupported)
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
                long msm = Template.memory_static_max;
                long mma = Template.MaxMemAllowed;
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
            spinnerDynMin.Increment = spinnerDynMax.Increment = spinnerStatMax.Increment = VMMemoryControlsEdit.CalcIncrement(Template.memory_static_max, spinnerDynMin.Units);
            
            long maxMemAllowed = MaxMemAllowed;
            double min = Template.memory_static_min;
            if (memoryMode == 1)
            {
                spinnerDynMin.SetRange(min, maxMemAllowed);
                return;
            }
            long min2 = (long)(SelectedMemoryStaticMax * memoryRatio);
            if (min < min2)
                min = min2;
            double max = SelectedMemoryDynamicMax;
            if (max < min)
                max = min;
            spinnerDynMin.SetRange(min, max);
            spinnerDynMax.SetRange(SelectedMemoryDynamicMin,
                memoryMode == 2 ? maxMemAllowed : SelectedMemoryStaticMax);
            spinnerStatMax.SetRange(SelectedMemoryDynamicMax, maxMemAllowed);
        }

        public void DisableMemoryControls()
        {
            spinnerDynMin.Enabled = false;
            spinnerDynMax.Enabled = false;
            spinnerStatMax.Enabled = false;
        }

        public double SelectedMemoryDynamicMin
        {
            get
            {
                return spinnerDynMin.Value;
            }
        }

        public double SelectedMemoryDynamicMax
        {
            get
            {
                return memoryMode == 1 ? spinnerDynMin.Value : spinnerDynMax.Value;
            }
        }

        public double SelectedMemoryStaticMax
        {
            get
            {
                return
                    memoryMode == 1 ? spinnerDynMin.Value :
                    memoryMode == 2 ? spinnerDynMax.Value :
                    spinnerStatMax.Value;
            }
        }

        public long SelectedVcpusMax
        {
            get
            {
                return (long)comboBoxVCPUs.SelectedItem;
            }
        }

        public long SelectedVcpusAtStartup
        {
            get
            {
                return isVcpuHotplugSupported ? (long)comboBoxInitialVCPUs.SelectedItem : (long)comboBoxVCPUs.SelectedItem;
            }
        }

        public long SelectedCoresPerSocket
        {
            get
            {
                return comboBoxTopology.CoresPerSocket;
            }
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> sum = new List<KeyValuePair<string, string>>();

                if (isVcpuHotplugSupported)
                {
                    sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_MAX_VCPUS, SelectedVcpusMax.ToString()));
                    sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_INITIAL_VCPUS, SelectedVcpusAtStartup.ToString()));
                }
                else
                {
                    sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_VCPUS, SelectedVcpusAtStartup.ToString()));
                }
                sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CPUMEMPAGE_TOPOLOGY, comboBoxTopology.Text));
                if (memoryMode == 1)
                    sum.Add(new KeyValuePair<string, string>(Messages.MEMORY, Util.MemorySizeStringSuitableUnits(SelectedMemoryStaticMax, false)));
                else
                {
                    sum.Add(new KeyValuePair<string, string>(Messages.DYNAMIC_MIN, Util.MemorySizeStringSuitableUnits(SelectedMemoryDynamicMin, false)));
                    sum.Add(new KeyValuePair<string, string>(Messages.DYNAMIC_MAX, Util.MemorySizeStringSuitableUnits(SelectedMemoryDynamicMax, false)));
                    if (memoryMode == 3)
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
            long max_mem_total = 0;
            long max_mem_free = 0;
            long max_vcpus = 0;
            Host max_mem_total_host = null;
            Host max_mem_free_host = null;
            Host max_vcpus_host = null;

            foreach (Host host in Connection.Cache.Hosts)
            {
                long host_cpus = 0;

                foreach (Host_cpu cpu in Connection.Cache.Host_cpus)
                {
                    if (cpu.host.opaque_ref.Equals(host.opaque_ref))
                        host_cpus++;
                }

                Host_metrics metrics = Connection.Resolve(host.metrics);

                if (host_cpus > max_vcpus)
                {
                    max_vcpus = host_cpus;
                    max_vcpus_host = host;
                }

                if (metrics != null && metrics.memory_total > max_mem_total)
                {
                    max_mem_total = metrics.memory_total;
                    max_mem_total_host = host;
                }

                // The available memory of a server is taken to be the current memory_free,
                // plus however much we can squeeze down the existing VMs. This assumes
                // that the overhead won't increase when we create the new VM, so it
                // has false negatives.
                long memory_free = host.memory_available_calc;

                if (metrics != null && memory_free > max_mem_free)
                {
                    max_mem_free = memory_free;
                    max_mem_free_host = host;
                }
            }

            if (max_mem_total_host != null && SelectedMemoryDynamicMin > max_mem_total)
            {
                ErrorPanel.Visible = true;
                ErrorLabel.Text = string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_MEMORYWARN1, Helpers.GetName(max_mem_total_host).Ellipsise(50), Util.MemorySizeStringSuitableUnits(max_mem_total, false));
            }
            else if (max_mem_free_host != null && SelectedMemoryDynamicMin > max_mem_free)
            {
                ErrorPanel.Visible = true;
                ErrorLabel.Text = string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_MEMORYWARN2, Helpers.GetName(max_mem_free_host).Ellipsise(50), Util.MemorySizeStringSuitableUnits(max_mem_free, false));
            }
            else if (max_vcpus_host != null && SelectedVcpusMax > max_vcpus)
            {
                ErrorPanel.Visible = true;
                ErrorLabel.Text = string.Format(Messages.NEWVMWIZARD_CPUMEMPAGE_VCPUSWARN, Helpers.GetName(max_vcpus_host).Ellipsise(50), max_vcpus);
            }
            else
            {
                ErrorPanel.Visible = false;
            }
        }

        private void vCPU_ValueChanged(object sender, EventArgs e)
        {
            comboBoxTopology.Update((long)comboBoxVCPUs.SelectedItem);
            ValuesUpdated();
            ValidateVCPUSettings();
            RefreshCurrentVCPUs();
        }

        private void memory_ValueChanged(object sender, EventArgs e)
        {
            if (initialising)
                return;

            SetSpinnerLimitsAndIncrement();
            ValuesUpdated();
        }

        private void ValidateVCPUSettings()
        {
            if (comboBoxVCPUs.SelectedItem != null)
                labelInvalidVCPUWarning.Text = VM.ValidVCPUConfiguration((long)comboBoxVCPUs.SelectedItem, comboBoxTopology.CoresPerSocket);
        }

        private long _prevVCPUsMax;

        private void RefreshCurrentVCPUs()
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
                else if (SelectedVcpusAtStartup == _prevVCPUsMax && SelectedVcpusMax != _prevVCPUsMax)
                    newValue = SelectedVcpusMax;

                PopulateVCPUsAtStartup(SelectedVcpusMax, newValue);
                _prevVCPUsMax = SelectedVcpusMax;
            }
        }

        private void comboBoxTopology_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateVCPUSettings();
        }
    }
}
