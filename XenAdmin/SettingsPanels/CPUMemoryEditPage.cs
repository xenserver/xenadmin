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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class CpuMemoryEditPage : UserControl, IEditPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private VM _vm;
        private bool _validToSave = true;
        private long _origVCpus;
        private long _origVCpusMax;
        private long _origVCpusAtStartup;
        private decimal _origVCpuWeight;
        private decimal _currentVCpuWeight;
        private bool _isVCpuHotplugSupported;
        private int _minVCpus;
        private long _prevVCpusMax;

        // Please note that the comboBoxVCPUs control can represent two different VM properties, depending whether the VM supports vCPU hotplug or not: 
        // If vCPU hotplug is supported, comboBoxVCPUs represents the maximum number of vCPUs (VCPUs_max). And the initial number of vCPUs is represented in comboBoxInitialVCPUs (which is only visible in this case) 
        // If vCPU hotplug is not supported, comboBoxVCPUs represents the initial number of vCPUs (VCPUs_at_startup). In this case we will also set the VM property VCPUs_max to the same value.
        // We use the _OrigVCPUs variable to store the original value that populates this combo box (VCPUs_max if hotplug is allowed, otherwise VCPUs_at_startup)

        private bool HasVCpuChanged => _origVCpus != (long)comboBoxVCPUs.SelectedItem;

        private bool HasVCpuWeightChanged => _origVCpuWeight != _currentVCpuWeight;

        private bool HasVCpusAtStartupChanged =>
            _isVCpuHotplugSupported && _origVCpusAtStartup != (long)comboBoxInitialVCPUs.SelectedItem;

        private bool HasTopologyChanged => _vm.GetCoresPerSocket() != comboBoxTopology.CoresPerSocket;

        private long SelectedVCpusMax => (long)comboBoxVCPUs.SelectedItem;

        private long SelectedVCpusAtStartup => _isVCpuHotplugSupported
            ? (long)comboBoxInitialVCPUs.SelectedItem
            : (long)comboBoxVCPUs.SelectedItem;

        public Image Image => Images.StaticImages._000_CPU_h32bit_16;

        public string SubText => string.Format(Messages.CPU_SUB, SelectedVCpusAtStartup);

        public CpuMemoryEditPage()
        {
            InitializeComponent();
            transparentTrackBar1.Scroll += tbPriority_Scroll;
            Text = Messages.CPU;
        }

        private void InitializeVCpuControls()
        {
            lblVCPUs.Text = _isVCpuHotplugSupported
                ? Messages.VM_CPUMEMPAGE_MAX_VCPUS_LABEL
                : Messages.VM_CPUMEMPAGE_VCPUS_LABEL;

            labelInitialVCPUs.Text = _vm.power_state == vm_power_state.Halted
                ? Messages.VM_CPUMEMPAGE_INITIAL_VCPUS_LABEL
                : Messages.VM_CPUMEMPAGE_CURRENT_VCPUS_LABEL;

            labelInitialVCPUs.Visible = comboBoxInitialVCPUs.Visible = _isVCpuHotplugSupported;
            comboBoxInitialVCPUs.Enabled = _isVCpuHotplugSupported &&
                                           (_vm.power_state == vm_power_state.Halted ||
                                            _vm.power_state == vm_power_state.Running);

            comboBoxVCPUs.Enabled = comboBoxTopology.Enabled = _vm.power_state == vm_power_state.Halted;

            comboBoxTopology.Populate(_vm.VCPUs_at_startup, _vm.VCPUs_max, _vm.GetCoresPerSocket(),
                _vm.MaxCoresPerSocket());

            // CA-12941 
            // We set a sensible maximum based on the template, but if the user sets something higher 
            // from the CLI then use that as the maximum.
            var maxAllowed = _vm.MaxVCPUsAllowed();
            var maxVCpus = maxAllowed < _origVCpus ? _origVCpus : maxAllowed;
            PopulateVCpus(maxVCpus, _origVCpus);

            if (_isVCpuHotplugSupported)
                PopulateVCpusAtStartup(_origVCpusMax, _origVCpusAtStartup);

            transparentTrackBar1.Value =
                Convert.ToInt32(Math.Log(Convert.ToDouble(_vm.GetVcpuWeight())) / Math.Log(4.0d));
            panel1.Enabled = _vm.power_state == vm_power_state.Halted;
        }

        private void Repopulate()
        {
            var vm = _vm;

            _isVCpuHotplugSupported = vm.SupportsVcpuHotplug();
            _minVCpus = vm.MinVCPUs();

            label1.Text = Messages.VM_CPUMEMPAGE_RUBRIC;

            if (_isVCpuHotplugSupported)
                label1.Text += Messages.VM_CPUMEMPAGE_RUBRIC_HOTPLUG;

            if (_vm.power_state != vm_power_state.Halted)
            {
                if (_isVCpuHotplugSupported)
                {
                    labelInfo.Text = Messages.VM_CPUMEMPAGE_MAX_VCPUS_READONLY;

                    if (_vm.power_state != vm_power_state.Running)
                        labelInfo.Text += Messages.VM_CPUMEMPAGE_CURRENT_VCPUS_READONLY;
                }
                else
                {
                    labelInfo.Text = Messages.VCPU_ONLY_WHEN_HALTED;
                }

                tableLayoutPanelInfo.Visible = true;
            }
            else
            {
                tableLayoutPanelInfo.Visible = false;
            }

            _origVCpusMax = vm.VCPUs_max > 0 ? vm.VCPUs_max : 1;
            _origVCpusAtStartup = vm.VCPUs_at_startup > 0 ? vm.VCPUs_at_startup : 1;
            _origVCpuWeight = _currentVCpuWeight;
            _origVCpus = _isVCpuHotplugSupported ? _origVCpusMax : _origVCpusAtStartup;
            _prevVCpusMax = _origVCpusMax; // we use variable in RefreshCurrentVCPUs for checking if VcpusAtStartup and VcpusMax were equal before VcpusMax changed

            _currentVCpuWeight = Convert.ToDecimal(vm.GetVcpuWeight());

            InitializeVCpuControls();

            _validToSave = true;
        }

        private void PopulateVCpuComboBox(ComboBox comboBox, long min, long max, long currentValue,
            Predicate<long> isValid)
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
            var min = _vm.power_state == vm_power_state.Halted ? 1 : _origVCpusAtStartup;
            PopulateVCpuComboBox(comboBoxInitialVCPUs, min, max, currentValue, i => true);
        }

        private void ShowCpuWarnings(IReadOnlyCollection<string> warnings)
        {
            var show = warnings.Count > 0;
            cpuWarningLabel.Text = show ? string.Join($"{Environment.NewLine}{Environment.NewLine}", warnings) : null;
            cpuWarningPictureBox.Visible = cpuWarningLabel.Visible = show;
        }

        private void ShowTopologyWarnings(IReadOnlyCollection<string> warnings)
        {
            var show = warnings.Count > 0;
            topologyWarningLabel.Text = show ? string.Join($"{Environment.NewLine}{Environment.NewLine}", warnings) : null;
            topologyPictureBox.Visible = topologyWarningLabel.Visible = show;
        }

        private void ValidateVCpuSettings()
        {
            if (_vm == null || !comboBoxVCPUs.Enabled)
                return;

            var homeHost = _vm.Home();
            var maxPhysicalCpus = _vm.Connection.Cache.Hosts.Select(h => h.host_CPUs.Count).Max();
            var homeHostPhysicalCpus = homeHost?.host_CPUs.Count;

            var warnings = new List<string>();

            if (comboBoxVCPUs.SelectedItem != null && maxPhysicalCpus < SelectedVCpusMax)
            {
                if (homeHostPhysicalCpus != null && homeHostPhysicalCpus < SelectedVCpusMax &&
                    maxPhysicalCpus >= SelectedVCpusMax)
                {
                    warnings.Add(Messages.VM_CPUMEMPAGE_VCPU_HOME_HOST_WARNING);
                }
                else if (maxPhysicalCpus < SelectedVCpusMax)
                {
                    warnings.Add(Messages.VM_CPUMEMPAGE_VCPU_WARNING);
                }
            }

            if (comboBoxVCPUs.SelectedItem != null && SelectedVCpusMax < _minVCpus)
            {
                warnings.Add(string.Format(Messages.VM_CPUMEMPAGE_VCPU_MIN_WARNING, _minVCpus));
            }

            if (comboBoxVCPUs.SelectedItem != null && SelectedVCpusMax > VM.MAX_VCPUS_FOR_NON_TRUSTED_VMS)
            {
                warnings.Add(string.Format(Messages.VCPUS_UNTRUSTED_VM_WARNING, VM.MAX_VCPUS_FOR_NON_TRUSTED_VMS, BrandManager.ProductBrand));
            }

            if (comboBoxInitialVCPUs.SelectedItem != null && SelectedVCpusAtStartup < _minVCpus)
            {
                warnings.Add(string.Format(Messages.VM_CPUMEMPAGE_VCPU_MIN_WARNING, _minVCpus));
            }

            ShowCpuWarnings(warnings);
        }

        private void ValidateTopologySettings()
        {
            var warnings = new List<string>();
            if (comboBoxVCPUs.SelectedItem != null)
            {
                var topologyWarning = VM.ValidVCPUConfiguration((long)comboBoxVCPUs.SelectedItem, comboBoxTopology.CoresPerSocket);

                if (!string.IsNullOrEmpty(topologyWarning))
                {
                    warnings.Add($"{topologyWarning}.");
                }
            }
            ShowTopologyWarnings(warnings);
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

        #region IEditPage

        public AsyncAction SaveSettings()
        {
            var actions = new List<AsyncAction>();

            if (HasVCpuWeightChanged)
            {
                _vm.SetVcpuWeight(Convert.ToInt32(_currentVCpuWeight));
            }

            if (HasVCpuChanged || HasVCpusAtStartupChanged)
            {
                actions.Add(new ChangeVCPUSettingsAction(_vm, SelectedVCpusMax, SelectedVCpusAtStartup));
            }

            if (HasTopologyChanged)
            {
                _vm.SetCoresPerSocket(comboBoxTopology.CoresPerSocket);
            }

            switch (actions.Count)
            {
                case 0:
                    return null;
                case 1:
                    return actions[0];
                default:
                    {
                        var multipleAction = new MultipleAction(_vm.Connection, "", "", "", actions, true);
                        return multipleAction;
                    }
            }
        }

        /// <summary>
        /// Must be a VM.
        /// </summary>
        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _vm = (VM)clone;
            Repopulate();
        }

        public bool ValidToSave => _validToSave;

        /** Show local validation balloon tooltips */
        public void ShowLocalValidationMessages()
        {
            // not applicable
        }

        public void HideLocalValidationMessages()
        {
            // not applicable
        }

        /** Unregister listeners, dispose balloon tooltips, etc. */
        public void Cleanup()
        {
            // not applicable
        }

        public bool HasChanged => HasVCpuChanged || HasTopologyChanged ||
                                  HasVCpusAtStartupChanged || HasVCpuWeightChanged;

        #endregion

        #region Events

        private void comboBoxTopology_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateTopologySettings();
        }

        private void comboBoxInitialVCPUs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateVCpuSettings();
        }

        private void comboBoxVCPUs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateVCpuSettings();
            comboBoxTopology.Update((long)comboBoxVCPUs.SelectedItem);
            ValidateTopologySettings();
            RefreshCurrentVCpus();
        }

        private void tbPriority_Scroll(object sender, EventArgs e)
        {
            _currentVCpuWeight = Convert.ToDecimal(Math.Pow(4.0d, Convert.ToDouble(transparentTrackBar1.Value)));
            if (transparentTrackBar1.Value == transparentTrackBar1.Max)
                _currentVCpuWeight--;
        }

        #endregion
    }
}