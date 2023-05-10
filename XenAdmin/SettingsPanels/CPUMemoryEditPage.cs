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
using System.Text;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Dialogs;


namespace XenAdmin.SettingsPanels
{
    public partial class CpuMemoryEditPage : UserControl, IEditPage
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private VM _vm;
        bool _showMemory = false;       // If this VM has DMC, we don't show the memory controls on this page.

        private bool _validToSave = true;
        private decimal _origMemory;
        private long _origVcpUs;
        private long _origVcpUsMax;
        private long _origVcpUsAtStartup;
        private decimal _origVcpuWeight;
        private decimal _currentVcpuWeight;
        private bool _isVcpuHotplugSupported;
        private int _minVcpUs;

        // Please note that the comboBoxVCPUs control can represent two different VM properties, depending whether the VM supports vCPU hotplug or not: 
        // If vCPU hotplug is supported, comboBoxVCPUs represents the maximum number of vCPUs (VCPUs_max). And the initial number of vCPUs is represented in comboBoxInitialVCPUs (which is only visible in this case) 
        // If vCPU hotplug is not supported, comboBoxVCPUs represents the initial number of vCPUs (VCPUs_at_startup). In this case we will also set the VM property VCPUs_max to the same value.
        // We use the _OrigVCPUs variable to store the original value that populates this combo box (VCPUs_max if hotplug is allowed, otherwise VCPUs_at_startup)

        private ChangeMemorySettingsAction _memoryAction;
        public bool ValidToSave
        {
            get
            {
                if (!_validToSave)
                    return false;

                // Also confirm whether the user wants to save memory changes.
                // If not, don't close the properties dialog.
                if (HasMemoryChanged)
                {
                    long mem = Convert.ToInt64(this.nudMemory.Value * Util.BINARY_MEGA);
                    _memoryAction = ConfirmAndCalcActions(mem);
                    if (_memoryAction == null)
                        return false;
                }

                return true;
            }
        }

        private ChangeMemorySettingsAction ConfirmAndCalcActions(long mem)
        {
            if (_vm.memory_static_max / Util.BINARY_MEGA == mem / Util.BINARY_MEGA)
            {
                // don't want to show warning dialog just for rounding errors
                mem = _vm.memory_static_max;
            }
            else if (_vm.power_state != vm_power_state.Halted)
            {
                var msg = _vm.SupportsBallooning() && !Helpers.FeatureForbidden(_vm, Host.RestrictDMC)
                    ? Messages.CONFIRM_CHANGE_MEMORY_MAX_SINGULAR
                    : Messages.CONFIRM_CHANGE_MEMORY_SINGULAR;

                using (var dlg = new WarningDialog(msg,
                    ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo))
                {
                    if (dlg.ShowDialog(this) != DialogResult.Yes)
                        return null;
                }
            }

            return new ChangeMemorySettingsAction(_vm,
                string.Format(Messages.ACTION_CHANGE_MEMORY_SETTINGS, _vm.Name()),
                _vm.memory_static_min, mem, mem, mem,
                VMOperationCommand.WarningDialogHAInvalidConfig, VMOperationCommand.StartDiagnosisForm, true);
        }

        public CpuMemoryEditPage()
        {
            InitializeComponent();

            Text = Messages.CPU_AND_MEMORY;

            transparentTrackBar1.Scroll += new EventHandler(tbPriority_Scroll);

            this.nudMemory.TextChanged += new EventHandler(nudMemory_TextChanged);
            this.nudMemory.LostFocus += new EventHandler(nudMemory_LostFocus);
        }

        public Image Image => Images.StaticImages._000_CPU_h32bit_16;

        void nudMemory_LostFocus(object sender, EventArgs e)
        {
            ValidateNud(nudMemory, (decimal)_vm.memory_static_max / Util.BINARY_MEGA);
        }

        private void ValidateNud(NumericUpDown nud, Decimal defaultValue)
        {
            if (!String.IsNullOrEmpty(nud.Text.Trim()))
                return;

            nud.Value = defaultValue >= nud.Minimum && defaultValue <= nud.Maximum ?
                defaultValue : nud.Maximum;

            nud.Text = nud.Value.ToString();
        }

        void nudMemory_TextChanged(object sender, EventArgs e)
        {
            decimal val;
            if (decimal.TryParse(nudMemory.Text, out val))
            {
                if (val >= nudMemory.Minimum && val <= nudMemory.Maximum)
                    nudMemory_ValueChanged(null, null);
                else if (val > nudMemory.Maximum)
                    ShowMemError(true, false);
                else
                    ShowMemError(false, false);
            }
            if (this.nudMemory.Text == "")
            {
                _validToSave = false;
            }
            else
            {
                _validToSave = true;
            }
        }

        private void tbPriority_Scroll(object sender, EventArgs e)
        {
            _currentVcpuWeight = Convert.ToDecimal(Math.Pow(4.0d, Convert.ToDouble(transparentTrackBar1.Value)));
            if (transparentTrackBar1.Value == transparentTrackBar1.Max)
                _currentVcpuWeight--;
        }

        /// <summary>
        /// Must be a VM.
        /// </summary>
        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _vm = (VM)clone;
            _showMemory = Helpers.FeatureForbidden(_vm, Host.RestrictDMC);
            Repopulate();
        }

        public void Repopulate()
        {
            VM vm = this._vm;

            Text = _showMemory ? Messages.CPU_AND_MEMORY : Messages.CPU;
            if (!_showMemory)
                lblMemory.Visible = panel2.Visible = MemWarningLabel.Visible = false;
            else if (vm.power_state != vm_power_state.Halted && vm.power_state != vm_power_state.Running)
            {
                panel2.Enabled = false;
                MemWarningLabel.Text = Messages.MEM_NOT_WHEN_SUSPENDED;
                MemWarningLabel.ForeColor = SystemColors.ControlText;
                MemWarningLabel.Visible = true;
            }

            // Since updates come in dribs and drabs, avoid error if new max and min arrive
            // out of sync and maximum < minimum.
            if (vm.memory_dynamic_max >= vm.memory_dynamic_min &&
                vm.memory_static_max >= vm.memory_static_min)
            {
                decimal min = Convert.ToDecimal(vm.memory_static_min / Util.BINARY_MEGA);
                decimal max = Convert.ToDecimal(vm.MaxMemAllowed() / Util.BINARY_MEGA);
                decimal value = Convert.ToDecimal(vm.memory_static_max / Util.BINARY_MEGA);
                // Avoid setting the range to exclude the current value: CA-40041
                if (value > max)
                    max = value;
                if (value < min)
                    min = value;
                this.nudMemory.Minimum = min;
                this.nudMemory.Maximum = max;
                this.nudMemory.Text = (this.nudMemory.Value = value).ToString();
            }

            Host currentHost = Helpers.GetCoordinator(this._vm.Connection);
            if (currentHost != null)
            {
                // Show the performance warning about vCPUs > pCPUs.
                // Don't show if the VM isn't running, since we don't know which server it will
                // run on (and so can't count the number of pCPUs).
                if ( vm.power_state == vm_power_state.Running
                    && vm.VCPUs_at_startup > currentHost.host_CPUs.Count
                    && !vm.GetIgnoreExcessiveVcpus())
                {
                    lblVcpuWarning.Visible = true;
                    this.tableLayoutPanel1.RowStyles[1].SizeType = SizeType.Absolute;
                    this.tableLayoutPanel1.RowStyles[1].Height = 30;
                }
                else
                {
                    lblVcpuWarning.Visible = false;
                }
            }
            else
            {
                lblVcpuWarning.Visible = false;
            }

            _isVcpuHotplugSupported = vm.SupportsVcpuHotplug();
            _minVcpUs = vm.MinVCPUs();

            label1.Text = GetRubric();

            _origMemory = nudMemory.Value;
            _origVcpUsMax = vm.VCPUs_max > 0 ? vm.VCPUs_max : 1;
            _origVcpUsAtStartup = vm.VCPUs_at_startup > 0 ? vm.VCPUs_at_startup : 1;
            _origVcpuWeight = _currentVcpuWeight;
            _origVcpUs = _isVcpuHotplugSupported ? _origVcpUsMax : _origVcpUsAtStartup;
            _prevVcpUsMax = _origVcpUsMax;  // we use variable in RefreshCurrentVCPUs for checking if VcpusAtStartup and VcpusMax were equal before VcpusMax changed

            _currentVcpuWeight = Convert.ToDecimal(vm.GetVcpuWeight());

            InitializeVcpuControls();
            
            _validToSave = true;
        }

        private void InitializeVcpuControls()
        {
            lblVCPUs.Text = _isVcpuHotplugSupported
                ? Messages.VM_CPUMEMPAGE_MAX_VCPUS_LABEL
                : Messages.VM_CPUMEMPAGE_VCPUS_LABEL;

            labelInitialVCPUs.Text = _vm.power_state == vm_power_state.Halted
                ? Messages.VM_CPUMEMPAGE_INITIAL_VCPUS_LABEL
                : Messages.VM_CPUMEMPAGE_CURRENT_VCPUS_LABEL;
            
            labelInitialVCPUs.Visible = comboBoxInitialVCPUs.Visible = _isVcpuHotplugSupported;
            comboBoxInitialVCPUs.Enabled = _isVcpuHotplugSupported &&
                                           (_vm.power_state == vm_power_state.Halted ||
                                            _vm.power_state == vm_power_state.Running);

            comboBoxVCPUs.Enabled = comboBoxTopology.Enabled = _vm.power_state == vm_power_state.Halted;

            comboBoxTopology.Populate(_vm.VCPUs_at_startup, _vm.VCPUs_max, _vm.GetCoresPerSocket(), _vm.MaxCoresPerSocket());

            // CA-12941 
            // We set a sensible maximum based on the template, but if the user sets something higher 
            // from the CLI then use that as the maximum.
            var maxAllowed = _vm.MaxVCPUsAllowed();
            long maxVcpUs = maxAllowed < _origVcpUs ? _origVcpUs : maxAllowed;
            PopulateVcpUs(maxVcpUs, _origVcpUs);

            if (_isVcpuHotplugSupported)
                PopulateVcpUsAtStartup(_origVcpUsMax, _origVcpUsAtStartup);

            transparentTrackBar1.Value = Convert.ToInt32(Math.Log(Convert.ToDouble(_vm.GetVcpuWeight())) / Math.Log(4.0d));
            panel1.Enabled = _vm.power_state == vm_power_state.Halted;
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
            long min = _vm.power_state == vm_power_state.Halted ? 1 : _origVcpUsAtStartup;
            PopulateVcpuComboBox(comboBoxInitialVCPUs, min, max, currentValue, i => true);
        }

        private string GetRubric()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Messages.VM_CPUMEMPAGE_RUBRIC);
            // add hotplug text
            if (_isVcpuHotplugSupported)
                sb.Append(Messages.VM_CPUMEMPAGE_RUBRIC_HOTPLUG);
            // add power state warning
            if (_vm.power_state != vm_power_state.Halted)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(_isVcpuHotplugSupported ? Messages.VM_CPUMEMPAGE_MAX_VCPUS_READONLY : Messages.VCPU_ONLY_WHEN_HALTED);
            }
            // add power state warning for Current number of vCPUs
            if (_isVcpuHotplugSupported && _vm.power_state != vm_power_state.Halted && _vm.power_state != vm_power_state.Running)
            {
                sb.Append(Messages.VM_CPUMEMPAGE_CURRENT_VCPUS_READONLY);
            }
            return sb.ToString();
        }

        public bool HasChanged
        {
            get { return HasVcpuChanged || HasMemoryChanged || HasTopologyChanged || HasVcpUsAtStartupChanged || HasVcpuWeightChanged; }
        }

        private bool HasMemoryChanged
        {
            get
            {
                return _origMemory != nudMemory.Value;
            }
        }

        private bool HasVcpuChanged
        {
            get
            {
                return _origVcpUs != (long)comboBoxVCPUs.SelectedItem;
            }
        }

        private bool HasVcpuWeightChanged
        {
            get
            {
                return _origVcpuWeight != _currentVcpuWeight;
            }
        }

        private bool HasVcpUsAtStartupChanged
        {
            get
            {
                return _isVcpuHotplugSupported && _origVcpUsAtStartup != (long)comboBoxInitialVCPUs.SelectedItem;
            }
        }

        private bool HasTopologyChanged
        {
            get
            {
                return _vm.GetCoresPerSocket() != comboBoxTopology.CoresPerSocket;
            }
        }

        private long SelectedVcpusMax
        {
            get
            {
                return (long)comboBoxVCPUs.SelectedItem;
            }
        }

        private long SelectedVcpusAtStartup
        {
            get
            {
                return _isVcpuHotplugSupported ? (long)comboBoxInitialVCPUs.SelectedItem : (long)comboBoxVCPUs.SelectedItem;
            }
        }

        public AsyncAction SaveSettings()
        {
            List<AsyncAction> actions = new List<AsyncAction>();

            if (HasVcpuWeightChanged)
            {
                _vm.SetVcpuWeight(Convert.ToInt32(_currentVcpuWeight));
            }

            if (HasVcpuChanged || HasVcpUsAtStartupChanged)
            {
                actions.Add(new ChangeVCPUSettingsAction(_vm, SelectedVcpusMax, SelectedVcpusAtStartup));
            }

            if (HasTopologyChanged)
            {
                _vm.SetCoresPerSocket(comboBoxTopology.CoresPerSocket);
            }
			
			if (HasMemoryChanged)
            {
                actions.Add(_memoryAction);  // Calculated in ValidToSave
            }

            if (actions.Count == 0)
                return null;
            else if (actions.Count == 1)
                return actions[0];
            else
            {
                MultipleAction multipleAction = new MultipleAction(_vm.Connection, "", "", "", actions, true);
                return multipleAction;
            }
        }

        /** Show local validation balloon tooltips */
        public void ShowLocalValidationMessages() { }

        public void HideLocalValidationMessages() { }

        /** Unregister listeners, dispose balloon tooltips, etc. */
        public void Cleanup() { }

        /// <summary>
        /// Shows the warning dialog about vCPUs > pCPUs.
        /// </summary>
        private void lblVcpuWarning_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_vm == null)
            {
                System.Diagnostics.Trace.Assert(false, "Selected object should be a vm");
                return;
            }

            using (var dialog = new WarningDialog(Messages.VCPUS_MORE_THAN_PCPUS)
            {
                ShowCheckbox = true,
                CheckboxCaption = Messages.DO_NOT_SHOW_THIS_MESSAGE
            })
            {
                dialog.ShowDialog(this);

                if (dialog.IsCheckBoxChecked)
                {
                    // User clicked 'ignore': set flag in VM.
                    Log.DebugFormat("Setting IgnoreExcessiveVcpus flag to true for VM {0}", _vm.Name());

                    VM copyVm = (VM)_vm.Clone();
                    copyVm.SetIgnoreExcessiveVcpus(true);

                    try
                    {
                        _vm.Locked = true;
                        copyVm.SaveChanges(_vm.Connection.Session);
                    }
                    finally
                    {
                        _vm.Locked = false;
                    }
                }
                else if (Program.MainWindow.SelectObjectInTree(_vm))
                {
                    Program.MainWindow.SwitchToTab(MainWindow.Tab.General);
                }
            }

            Refresh();
        }

        private void nudMemory_ValueChanged(object sender, EventArgs e)
        {
            ShowMemError(false, true);
        }

        private void ShowMemError(bool showAlways, bool testValue)
        {
            if (_vm == null || !_showMemory)
                return;

            Host selectedAffinity = _vm.Connection.Resolve<Host>(_vm.power_state == vm_power_state.Running ? _vm.resident_on : _vm.affinity);
            if (selectedAffinity != null)
            {
                Host_metrics hostMetrics = _vm.Connection.Resolve<Host_metrics>(selectedAffinity.metrics);
                if ((showAlways || (testValue && (hostMetrics != null && (double)hostMetrics.memory_total < (double)nudMemory.Value * (double)Util.BINARY_MEGA))))
                {
                    MemWarningLabel.Visible = true;
                }
                else
                {
                    MemWarningLabel.Visible = false;
                }
            }
        }

        private void comboBoxVCPUs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateVcpuSettings();
            comboBoxTopology.Update((long)comboBoxVCPUs.SelectedItem);
            ValidateTopologySettings();
            RefreshCurrentVcpUs();
        }

        private void ValidateVcpuSettings()
        {
            if (_vm == null || !comboBoxVCPUs.Enabled)
                return;
            var homeHost = _vm.Home();
            var maxPhysicalCpus = _vm.Connection.Cache.Hosts.Select(h => h.host_CPUs.Count).Max();
            var homeHostPhysicalCpus = homeHost?.host_CPUs.Count;


            if (comboBoxVCPUs.SelectedItem != null && maxPhysicalCpus < SelectedVcpusMax)
            {
                if (homeHostPhysicalCpus != null && homeHostPhysicalCpus < SelectedVcpusMax && maxPhysicalCpus >= SelectedVcpusMax)
                {
                    VCPUWarningLabel.Text = Messages.VM_CPUMEMPAGE_VCPU_HOME_HOST_WARNING;
                    VCPUWarningLabel.Visible = true;
                }
                else if (maxPhysicalCpus < SelectedVcpusMax)
                {
                    VCPUWarningLabel.Text = Messages.VM_CPUMEMPAGE_VCPU_WARNING;
                    VCPUWarningLabel.Visible = true;
                }
                else
                {
                    VCPUWarningLabel.Visible = false;
                }
            }
            else if (comboBoxVCPUs.SelectedItem != null && SelectedVcpusMax < _minVcpUs)
            {
                VCPUWarningLabel.Text = string.Format(Messages.VM_CPUMEMPAGE_VCPU_MIN_WARNING, _minVcpUs);
                VCPUWarningLabel.Visible = true;
            }
            else if (comboBoxVCPUs.SelectedItem != null && SelectedVcpusMax > VM.MAX_VCPUS_FOR_NON_TRUSTED_VMS)
            {
                VCPUWarningLabel.Text = string.Format(Messages.VCPUS_UNTRUSTED_VM_WARNING, VM.MAX_VCPUS_FOR_NON_TRUSTED_VMS, BrandManager.ProductBrand);
                VCPUWarningLabel.Visible = true;
            }
            else
            {
                VCPUWarningLabel.Visible = false;
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

        public String SubText
        {
            get
            {
                return _showMemory ?
                    String.Format(Messages.CPU_AND_MEMORY_SUB, SelectedVcpusAtStartup, nudMemory.Value) :
                    String.Format(Messages.CPU_SUB, SelectedVcpusAtStartup);
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
