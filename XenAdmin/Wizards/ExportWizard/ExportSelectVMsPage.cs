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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Wizards.ExportWizard
{
    /// <summary>
    /// Class representing the page of the ExportAppliance wizard where the user specifies
    /// the VMs to be included in the exported appliance
    /// </summary>
    internal partial class ExportSelectVMsPage : XenTabPage
    {
        private bool m_buttonNextEnabled;

        public ExportSelectVMsPage()
        {
            InitializeComponent();
        }

        #region Accessors

        /// <summary>
        /// Gets a list of the VMs that will be included in the exported appliance
        /// </summary>
        public List<VM> VMsToExport { get; } = new List<VM>();

        /// <summary>
        /// The items selected on the main window treeview when the wizard was launched.
        /// These determine the VMs selected by default.
        /// </summary>
        public SelectedItemCollection SelectedItems { private get; set; }

        public bool IncludeMemorySnapshot => groupBoxSuspended.Enabled && radioButtonIncludeSnapshot.Checked;

        #endregion

        #region Base class (XenTabPage) overrides

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle =>  Messages.EXPORT_SELECTVMS_PAGE_TITLE;

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text => Messages.EXPORT_SELECTVMS_PAGE_TEXT;

        /// <summary>
        /// Gets the value by which the help files section for this page is identified
        /// </summary>
        public override string HelpID => "SelectVMs";

        protected override bool ImplementsIsDirty()
        {
            return true;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            EnableButtons();
        }

        public override void PopulatePage()
        {
            var pool = Helpers.GetPoolOfOne(Connection);
            label2.Text = string.Format(Helpers.IsPool(pool.Connection) ? Messages.VMS_IN_POOL : Messages.VMS_IN_SERVER,
                pool.Name().Ellipsise(60));
            VMsToExport.Clear();

            try
            {
                m_dataGridView.SuspendLayout();
                m_dataGridView.Rows.Clear();

                var applianceVMs = new List<XenRef<VM>>();
                if (SelectedItems != null && SelectedItems.FirstIs<VM_appliance>())
                    applianceVMs.AddRange(((VM_appliance)SelectedItems.FirstAsXenObject).VMs);

                var rows = new List<DataGridViewRow>();

                foreach (var vm in Connection.Cache.VMs.Where(vm => IsVmExportable(vm) && MatchesSearchText(vm)))
                {
                    VM curVm = vm; //closure below
                    bool selected = SelectedItems != null
                                    && (SelectedItems.AsXenObjects().Contains(vm) || applianceVMs.FirstOrDefault(xenref => xenref.opaque_ref == curVm.opaque_ref) != null);

                    if (vm.power_state == vm_power_state.Suspended)
                        groupBoxSuspended.Visible = true;

                    rows.Add(new VmRow(vm, selected));

                    if (selected)
                        VMsToExport.Add(vm);
                }

                m_dataGridView.Rows.AddRange(rows.ToArray());
                m_dataGridView.Sort(columnTick, ListSortDirection.Descending);
            }
            finally
            {
                m_dataGridView.ResumeLayout();
            }

            UpdateCounterLabel();
            EnableButtons();
        }

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

        #endregion

        #region Private methods

        private bool IsVmExportable(VM vm)
        {
            if (!vm.IsRealVm())
                return false;

            if (!vm.Show(Properties.Settings.Default.ShowHiddenVMs))
                return false;

            if (vm.power_state != vm_power_state.Halted && vm.power_state != vm_power_state.Suspended)
                return false;

            if (vm.Locked)
                return false;

            return vm.allowed_operations != null && vm.allowed_operations.Contains(vm_operations.export);
        }

        private bool MatchesSearchText(VM vm)
        {
            return m_searchTextBox.Matches(vm.Name());
        }

        private void EnableButtons()
        {
            var count = VMsToExport.Count;

            groupBoxSuspended.Enabled = count == 1 && VMsToExport[0].power_state == vm_power_state.Suspended;
            m_buttonNextEnabled = count > 0;
            m_buttonClearAll.Enabled = count > 0;
            m_buttonSelectAll.Enabled = count < m_dataGridView.RowCount;
            OnPageUpdated();
        }

        private void UpdateCounterLabel()
        {
            var count = VMsToExport.Count;
            m_labelCounter.Text = count == 1 ? Messages.ONE_VM_SELECTED : string.Format(Messages.MOREONE_VM_SELECTED, count);
        }

        #endregion

        private class VmRow : DataGridViewRow
        {
            private readonly DataGridViewCheckBoxCell _tickCell = new DataGridViewCheckBoxCell();
            private readonly DataGridViewImageCell _imageCell = new DataGridViewImageCell();
            private readonly DataGridViewTextBoxCell _nameCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _descriptionCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _sizeCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _applianceCell = new DataGridViewTextBoxCell();

            public VmRow(VM vm, bool selected)
            {
                Vm = vm;
                var appliance = vm.Connection.Resolve(vm.appliance);
                VmTotalSize = vm.GetTotalSize();

                _tickCell.Value = selected;
                _imageCell.Value = Images.GetImage16For(vm);
                _nameCell.Value = vm.Name();
                _descriptionCell.Value = vm.Description();
                _sizeCell.Value = Util.DiskSizeString(VmTotalSize);
                _applianceCell.Value = appliance == null ? Messages.NONE : appliance.Name();

                Cells.AddRange(_tickCell, _imageCell, _nameCell, _descriptionCell, _sizeCell, _applianceCell);
            }

            public VM Vm { get; }

            public ulong VmTotalSize { get; }

            public bool IsTicked
            {
                get => (bool)_tickCell.Value;
                set => _tickCell.Value = value;
            }
        }

        #region Control event handlers

        private void m_buttonSelectAll_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in m_dataGridView.Rows)
            {
                if (!(row is VmRow vmRow))
                    continue;

                vmRow.IsTicked = true;

                if (!VMsToExport.Contains(vmRow.Vm))
                    VMsToExport.Add(vmRow.Vm);
            }
        }

        private void m_buttonClearAll_Click(object sender, EventArgs e)
        {
            //clear all visible ones

            foreach (DataGridViewRow row in m_dataGridView.Rows)
            {
                if (!(row is VmRow vmRow))
                    continue;

                vmRow.IsTicked = false;
                VMsToExport.Remove(vmRow.Vm);
            }
        }

        private void m_searchTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var sortedColumn = m_dataGridView.SortedColumn;
                var sortOrder = m_dataGridView.SortOrder;

                m_dataGridView.SuspendLayout();
                m_dataGridView.Rows.Clear();

                foreach (var vm in Connection.Cache.VMs.Where(IsVmExportable))
                {
                    if (MatchesSearchText(vm))
                        m_dataGridView.Rows.Add(new VmRow(vm, VMsToExport.Contains(vm)));
                }

                if (sortOrder != SortOrder.None && sortedColumn != null)
                    m_dataGridView.Sort(sortedColumn, sortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
            }
            finally
            {
                m_dataGridView.ResumeLayout();
            }
        }

        private void m_dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (m_dataGridView.IsCurrentCellDirty)
                m_dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void m_dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0 || e.RowIndex < 0 || e.RowIndex >= m_dataGridView.RowCount)
                return;

            if (m_dataGridView.Rows[e.RowIndex] is VmRow row)
            {
                if (row.IsTicked)
                {
                    if (!VMsToExport.Contains(row.Vm))
                        VMsToExport.Add(row.Vm);
                }
                else
                {
                    VMsToExport.Remove(row.Vm);
                }
            }

            UpdateCounterLabel();
            IsDirty = true;
            EnableButtons();
        }

        private void m_dataGridView_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if ((e.Row.State & DataGridViewElementStates.Selected) != 0)
                e.Row.Selected = false;
        }

        private void m_dataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            var row1 = m_dataGridView.Rows[e.RowIndex1] as VmRow;
            var row2 = m_dataGridView.Rows[e.RowIndex2] as VmRow;

            if (row1 == null && row2 == null)
                e.SortResult = 0;
            else if (row1 == null)
                e.SortResult = 1;
            else if (row2 == null)
                e.SortResult = -1;
            else if (e.Column == columnTick)
                e.SortResult = row1.IsTicked.CompareTo(row2.IsTicked);
            else if (e.Column == columnImage)
                e.SortResult = row1.Vm.power_state.CompareTo(row2.Vm.power_state);
            else if (e.Column == columnSize)
                e.SortResult = row1.VmTotalSize.CompareTo(row2.VmTotalSize);
            else
                return;//in all other cases it should be handled automatically

            e.Handled = true;
        }

        private void radioButtonDiscardSnapshot_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDiscardSnapshot.Checked)
                IsDirty = true;
        }

        private void radioButtonIncludeSnapshot_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonIncludeSnapshot.Checked)
                IsDirty = true;
        }

        #endregion
    }
}
