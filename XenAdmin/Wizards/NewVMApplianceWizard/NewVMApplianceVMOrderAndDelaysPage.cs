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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.SettingsPanels;
using XenAPI;

namespace XenAdmin.Wizards.NewVMApplianceWizard
{
    public partial class NewVMApplianceVMOrderAndDelaysPage : XenTabPage, IEditPage
    {
        private Pool _pool;
        public Pool Pool
        {
            get { return _pool; }
            set { _pool = value; }
        }

        public NewVMApplianceVMOrderAndDelaysPage()
        {
            InitializeComponent();

            nudStartDelay.Maximum = long.MaxValue;
            nudOrder.Maximum = long.MaxValue;

            dataGridView1.SortCompare += dataGridView1_SortCompare;
        }

        public override string Text
        {
            get { return Messages.NEWVMAPPLIANCE_VMORDERANDDELAYSPAGE_TEXT; } 
        }

        public string SubText
        {
            get { return ""; }
        }

        public override string HelpID
        {
            get { return "VMOrderAndDelays"; } 
        }

        public Image Image
        {
            get { return Properties.Resources._000_RebootVM_h32bit_16; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMAPPLIANCE_VMORDERANDDELAYSPAGE_TITLE; }
        }

        /// <summary>
        /// Whether the user has made any changes to VM properties on the server.
        /// </summary>
        public bool ChangesMade { get; private set; }

        private List<VM> _selectedVMs = null;
        private Dictionary<VM, VMStartupOptions> _currentSettings = null; 

        public void SetSelectedVMs(List<VM> selectedVMs)
        {
            _selectedVMs = selectedVMs;
            _currentSettings = getCurrentSettings();
            RefreshTab();
        }

        private void RefreshTab()
        {
            dataGridView1.Rows.Clear();
            foreach (VM vm in _selectedVMs)
            {
                VMStartupOptions settings = _currentSettings != null && _currentSettings.ContainsKey(vm)
                                          ? new VMStartupOptions(_currentSettings[vm].Order, _currentSettings[vm].StartDelay)
                                          : new VMStartupOptions(vm.order, vm.start_delay);

                
                // Create a new row for this VM
                dataGridView1.Rows.Add(new VMDataGridViewRow(vm, settings));
            }
            dataGridView1.Sort(ColumnName, ListSortDirection.Ascending);
        }

        void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            // If sorting by name, actually sort by VM. This ensures that we use the
            // same sort as everywhere else.
            if (e.Column == ColumnName)
            {
                VM vm1 = ((VMDataGridViewRow)dataGridView1.Rows[e.RowIndex1]).Vm;
                VM vm2 = ((VMDataGridViewRow)dataGridView1.Rows[e.RowIndex2]).Vm;
                e.SortResult = vm1.CompareTo(vm2);
                e.Handled = true;
                return;
            }

            e.Handled = false;
        }

        private class VMDataGridViewRow : DataGridViewRow
        {
            private DataGridViewTextBoxCell _nameCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _descriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _orderCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _startDelayCell = new DataGridViewTextBoxCell();
            public readonly VM Vm;
            public VMStartupOptions Settings;
            public VMDataGridViewRow(VM vm, VMStartupOptions settings)
            {
                Vm = vm;
                Cells.Add(_nameCell);
                Cells.Add(_descriptionCell);
                Cells.Add(_orderCell);
                Cells.Add(_startDelayCell);;
                
                Settings = settings;
                _nameCell.Value = Vm.Name;
                _descriptionCell.Value = Vm.Description;
                UpdateRow();
            }

            public void UpdateRow()
            {
                _orderCell.Value = Settings.Order;
                _startDelayCell.Value = Settings.StartDelay;
            }
        }

        private void updateTextBoxes()
        {
            Program.AssertOnEventThread();

            if (dataGridView1.SelectedRows.Count == 0)
            {
                nudOrder.Enabled = false;
                nudOrder.Text = "";

                nudStartDelay.Enabled = false;
                nudStartDelay.Text = "";

                return;
            }

            nudOrder.Enabled = true;
            nudStartDelay.Enabled = true;

            long order = ((VMDataGridViewRow) dataGridView1.SelectedRows[0]).Settings.Order;
            bool sameOrder = true;

            long startDelay = ((VMDataGridViewRow)dataGridView1.SelectedRows[0]).Settings.StartDelay;
            bool sameStartDelay = true;

            foreach (VMDataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (order != row.Settings.Order)
                {
                    order = row.Settings.Order;
                    sameOrder = false;
                }

                if (startDelay != row.Settings.StartDelay)
                {
                    startDelay = row.Settings.StartDelay;
                    sameStartDelay = false;
                }
            }

            nudOrder.Text = sameOrder ? order.ToString() : "";

            nudStartDelay.Text = sameStartDelay ? startDelay.ToString() : "";
        }

        /// <summary>
        /// Gets the current (uncommitted) VM settings. Must be called on the GUI thread.
        /// </summary>
        /// <returns></returns>
        public Dictionary<VM, VMStartupOptions> getCurrentSettings()
        {
            Program.AssertOnEventThread();
            if (dataGridView1.RowCount == 0)
                return null;
            
            var result = new Dictionary<VM, VMStartupOptions>();
            foreach (VMDataGridViewRow row in dataGridView1.Rows)
            {
                result.Add(row.Vm, row.Settings);
            }
            return result;
        }

        #region Control event handlers

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            long order = (long)nudOrder.Value;
            bool changesMade = false;

            foreach (VMDataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.Settings.Order != order)
                {
                    changesMade = true;
                    row.Settings.Order = order;
                    row.UpdateRow();
                }
            }

            if (changesMade)
            {
                ChangesMade = true;
                updateTextBoxes();
            }
        }

        private void nudStartDelay_ValueChanged(object sender, EventArgs e)
        {
            long startDelay = (long)nudStartDelay.Value;
            bool changesMade = false;

            foreach (VMDataGridViewRow row in dataGridView1.SelectedRows)
            {
                if (row.Settings.StartDelay != startDelay)
                {
                    changesMade = true;
                    row.Settings.StartDelay = startDelay;
                    row.UpdateRow();
                }
            }

            if (changesMade)
            {
                ChangesMade = true;
                updateTextBoxes();
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            updateTextBoxes();
        }

        #endregion

        #region IEditPage implementation

        public AsyncAction SaveSettings()
        {
            return new SetVMStartupOptionsAction(Pool.Connection, getCurrentSettings(), true);
        }

        private VM_appliance _clone;
        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _clone = (VM_appliance)clone;
            Pool = Helpers.GetPoolOfOne(_clone.Connection);
            //RefreshTab(null);
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
            dataGridView1.Rows.Clear();
        }

        public bool HasChanged
        {
            get { return ChangesMade; }
        }

        #endregion
    }
}
