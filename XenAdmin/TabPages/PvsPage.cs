/* Copyright (c) Citrix Systems Inc. 
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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Network;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Commands;

namespace XenAdmin.TabPages
{
    public partial class PvsPage : BaseTabPage
    {
        private IXenConnection connection;
        private SelectionManager enableSelectionManager;
        private SelectionManager disableSelectionManager;

        private DataGridViewVms_DefaultSort vmDefaultSort;
        
        public PvsPage()
        {
            InitializeComponent();

            enableButton.Command = new EnablePvsReadCachingCommand();
            disableButton.Command = new DisablePvsReadCachingCommand();

            enableSelectionManager = new SelectionManager();
            disableSelectionManager = new SelectionManager();

            vmDefaultSort = new DataGridViewVms_DefaultSort();
            
            base.Text = Messages.PVS_TAB_TITLE;
        }

        public IXenConnection Connection
        {
            get
            {
                return connection;
            }
            set
            {
                if (connection != null)
                {
                    connection.Cache.DeregisterBatchCollectionChanged<PVS_site>(PvsSiteBatchCollectionChanged);
                    connection.Cache.DeregisterBatchCollectionChanged<PVS_proxy>(PvsProxyBatchCollectionChanged);
                    connection.Cache.DeregisterBatchCollectionChanged<VM>(PvsProxyBatchCollectionChanged);
                }

                connection = value;

                if (connection != null)
                {
                    connection.Cache.RegisterBatchCollectionChanged<PVS_site>(PvsSiteBatchCollectionChanged);
                    connection.Cache.RegisterBatchCollectionChanged<PVS_proxy>(PvsProxyBatchCollectionChanged);
                    connection.Cache.RegisterBatchCollectionChanged<VM>(PvsProxyBatchCollectionChanged);
                }

                LoadVMs();
            }
        }

        #region VMs

        private void LoadVMs()
        {
            Program.AssertOnEventThread();

            if (!Visible)
                return;

            try
            {
                dataGridViewVms.SuspendLayout();

                var previousSelection = GetSelectedVMs();

                UnregisterVMHandlers();

                dataGridViewVms.SortCompare += dataGridViewVms_SortCompare;

                dataGridViewVms.Rows.Clear();

                enableSelectionManager.BindTo(enableButton, Program.MainWindow);
                disableSelectionManager.BindTo(disableButton, Program.MainWindow);

                //clear selection
                enableSelectionManager.SetSelection(new SelectedItemCollection());
                disableSelectionManager.SetSelection(new SelectedItemCollection());

                foreach (var vm in Connection.Cache.VMs.Where(vm => vm.is_a_real_vm && vm.Show(Properties.Settings.Default.ShowHiddenVMs)))
                    dataGridViewVms.Rows.Add(NewVmRow(vm));

                if (dataGridViewVms.SortedColumn == null)
                {
                    dataGridViewVms.Sort(vmDefaultSort);
                }
                else
                {

                    var order = dataGridViewVms.SortOrder;
                    ListSortDirection sortDirection = ListSortDirection.Ascending;
                    if (order.Equals(SortOrder.Descending))
                        sortDirection = ListSortDirection.Descending;

                    dataGridViewVms.Sort(dataGridViewVms.SortedColumn, sortDirection);
                }

                if (dataGridViewVms.Rows.Count > 0)
                {
                    dataGridViewVms.SelectionChanged += VmSelectionChanged;

                    UnselectAllVMs(); // Component defaults the first row to selected
                    if (previousSelection.Any())
                    {
                        foreach (var row in dataGridViewVms.Rows.Cast<DataGridViewRow>())
                        {
                            if (previousSelection.Contains((VM)row.Tag))
                            {
                                row.Selected = true;
                            }
                        }
                    }
                    else
                    {
                        dataGridViewVms.Rows[0].Selected = true;
                    }
                }
            }
            finally
            {
                dataGridViewVms.ResumeLayout();
            }
        }

        private void UnselectAllVMs()
        {
            foreach (var row in dataGridViewVms.SelectedRows.Cast<DataGridViewRow>())
            {
                row.Selected = false;
            }
        }

        private IList<VM> GetSelectedVMs()
        {
            return dataGridViewVms.SelectedRows.Cast<DataGridViewRow>().Select(row => (VM)row.Tag).ToList();
        }

        private void VmSelectionChanged(object sender, EventArgs e)
        {
            var selectedVMs = GetSelectedVMs().Select(vm => new SelectedItem(vm));
            enableSelectionManager.SetSelection(selectedVMs);
            disableSelectionManager.SetSelection(selectedVMs);
        }

        private DataGridViewRow NewVmRow(VM vm)
        {
            System.Diagnostics.Trace.Assert(vm != null);

            var pvsProxy = vm.PvsProxy;

            if (pvsProxy == null)
            {
                return NewVmRowWithNoProxy(vm);
            }
            return NewVmRowWithProxy(vm, pvsProxy);
        }

        private DataGridViewRow NewVmRowWithNoProxy(VM vm)
        {
            var vmCell = new DataGridViewTextBoxCell { Value = vm.Name };
            var cacheEnabledCell = new DataGridViewTextBoxCell { Value = Messages.NO };
            var pvsSiteCell = new DataGridViewTextBoxCell { Value = Messages.NO_VALUE };
            var statusCell = new DataGridViewTextBoxCell { Value = Messages.NO_VALUE };

            var newRow = new DataGridViewRow { Tag = vm };
            newRow.Cells.AddRange(vmCell, cacheEnabledCell, pvsSiteCell, statusCell);
            vm.PropertyChanged += VmPropertyChanged;

            return newRow;
        }

        private DataGridViewRow NewVmRowWithProxy(VM vm, PVS_proxy pvsProxy)
        {
            System.Diagnostics.Trace.Assert(pvsProxy != null);

            var vmCell = new DataGridViewTextBoxCell { Value = vm.Name };
            var cacheEnabledCell = new DataGridViewTextBoxCell { Value = Messages.YES };

            var pvsSiteCell = new DataGridViewTextBoxCell { Value = Connection.Resolve(pvsProxy.site) };

            var statusCell = new DataGridViewTextBoxCell
            {
                Value = pvs_proxy_status_extensions.ToFriendlyString(pvsProxy.status)
            };

            var newRow = new DataGridViewRow { Tag = vm };
            newRow.Cells.AddRange(vmCell, cacheEnabledCell, pvsSiteCell, statusCell);
            vm.PropertyChanged += VmPropertyChanged;

            return newRow;
        }

        private void UnregisterVMHandlers()
        {
            dataGridViewVms.SelectionChanged -= VmSelectionChanged;
            dataGridViewVms.SortCompare -= dataGridViewVms_SortCompare;

            foreach (DataGridViewRow row in dataGridViewVms.Rows)
            {
                var vm = row.Tag as VM;
                System.Diagnostics.Trace.Assert(vm != null);
                vm.PropertyChanged -= VmPropertyChanged;
            }
        }

        void dataGridViewVms_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            e.Handled = false;

            if (!e.Column.Equals(columnVM))
            {
                // All columns in this table except VM name are likely to contain a lot of duplicates
                // so always use VM as the tiebreaker for consistency
                var cellValue1 = e.CellValue1.ToString();
                var cellValue2 = e.CellValue2.ToString();

                if (cellValue1 != cellValue2)
                {
                    e.SortResult = cellValue1.CompareTo(cellValue2);
                    e.Handled = true;
                    return;
                }
            }

            // If we get here, either sorting by VM name column, or using it as tie breaker
            // In order to sort by name, we actually use the built-in VM sort.
            // This ensures we use the correct name sorting, and gives a consistent tie-breaker.
            VM vm1 = (VM)dataGridViewVms.Rows[e.RowIndex1].Tag;
            VM vm2 = (VM)dataGridViewVms.Rows[e.RowIndex2].Tag;
            e.SortResult = vm1.CompareTo(vm2);
            e.Handled = true;
        }
        #endregion
        
        private void PvsSiteBatchCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, LoadVMs); 
        }

        private void PvsProxyBatchCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, LoadVMs);
        }

        private void VmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "name_label")
                return;

            foreach (DataGridViewRow row in dataGridViewVms.Rows)
            {
                var vm = row.Tag as VM;
                if (vm != null && vm.Equals(sender))
                {
                    row.Cells["columnVM"].Value = vm.Name;
                    break;
                }
            }
        }

        private void ConfigureButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new PvsCacheConfigurationDialog(connection))
                dialog.ShowDialog(this);
        }
    }

    class DataGridViewVms_DefaultSort : System.Collections.IComparer
    {
        public int Compare(object first, object second)
        {
            // Default sort: Sort by whether caching is enabled (yes before no), using VM name (asc) as tiebreaker
            DataGridViewRow row1 = (DataGridViewRow)first;
            DataGridViewRow row2 = (DataGridViewRow)second;

            int cachingEnabled1 = row1.Cells[1].Value.ToString().Equals(Messages.YES) ? 0 : 1;
            int cachingEnabled2 = row2.Cells[1].Value.ToString().Equals(Messages.YES) ? 0 : 1;

            if (cachingEnabled1 != cachingEnabled2)
            {
                return cachingEnabled1.CompareTo(cachingEnabled2);
            }
            else
            {
                // VM name as tiebreaker
                var vm1 = row1.Tag as VM;
                var vm2 = row2.Tag as VM;

                return vm1.CompareTo(vm2);
            }
        }
    }
}
