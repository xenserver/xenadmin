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
using XenAPI;

namespace XenAdmin.TabPages
{
    public partial class PvsPage : BaseTabPage
    {
        private IXenConnection connection;
        
        public PvsPage()
        {
            InitializeComponent();
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
                    connection.Cache.DeregisterBatchCollectionChanged<PVS_farm>(PvsFarmBatchCollectionChanged);
                    connection.Cache.DeregisterBatchCollectionChanged<PVS_proxy>(PvsProxyBatchCollectionChanged);
                }

                connection = value;

                if (connection != null)
                {
                    connection.Cache.RegisterBatchCollectionChanged<PVS_farm>(PvsFarmBatchCollectionChanged);
                    connection.Cache.RegisterBatchCollectionChanged<PVS_proxy>(PvsProxyBatchCollectionChanged);
                }

                LoadFarms();
                LoadVMs();
            }
        }

        private void LoadFarms()
        {
            Program.AssertOnEventThread();

            if (!Visible)
                return;

            try
            {
                dataGridViewFarms.SuspendLayout();
                dataGridViewFarms.Rows.Clear();

                var rowList = new List<DataGridViewRow>();

                foreach (var pvsFarm in Connection.Cache.PVS_farms)
                    rowList.Add(NewPvsFarmRow(pvsFarm));

                dataGridViewFarms.Rows.AddRange(rowList.ToArray());

                if (dataGridViewFarms.SelectedRows.Count == 0 && dataGridViewFarms.Rows.Count > 0)
                    dataGridViewFarms.Rows[0].Selected = true;
            }
            finally
            {
                dataGridViewFarms.ResumeLayout();
            }
        }

        private void LoadVMs()
        {
            Program.AssertOnEventThread();

            if (!Visible)
                return;

            try
            {
                dataGridViewVms.SuspendLayout();
                
                UnregisterVMHandlers();
                dataGridViewVms.Rows.Clear();
                
                foreach (var pvsProxy in Connection.Cache.PVS_proxies.Where(p => p.VM != null))
                    dataGridViewVms.Rows.Add(NewVmRow(pvsProxy));

                if (dataGridViewVms.SelectedRows.Count == 0 && dataGridViewVms.Rows.Count > 0)
                    dataGridViewVms.Rows[0].Selected = true;
            }
            finally
            {
                dataGridViewVms.ResumeLayout();
            }
        }

        private DataGridViewRow NewPvsFarmRow(PVS_farm pvsFarm)
        {
            var farmCell = new DataGridViewTextBoxCell {Value = pvsFarm.name};
            var configurationCell = new DataGridViewTextBoxCell
            {
                Value = pvsFarm.cache_storage.Count > 0 ? Messages.PVS_CACHE_MEMORY_AND_DISK: Messages.PVS_CACHE_MEMORY_ONLY
            };
            var cacheSrsCell = new DataGridViewTextBoxCell
            {
                Value = string.Join(",  ", Connection.ResolveAll(pvsFarm.cache_storage))
            };

            var newRow = new DataGridViewRow { Tag = pvsFarm };
            newRow.Cells.AddRange(farmCell, configurationCell, cacheSrsCell);

            return newRow;
        }

        private DataGridViewRow NewVmRow(PVS_proxy pvsProxy)
        {
            var vm = pvsProxy.VM;
            System.Diagnostics.Trace.Assert(vm != null);
            var vmCell = new DataGridViewTextBoxCell {Value = pvsProxy.VM.Name};
            var cachedCell = new DataGridViewTextBoxCell
            {
                Value = pvsProxy.currently_attached ? Messages.YES : Messages.NO
            };
            var srCell = new DataGridViewTextBoxCell {Value = Connection.Resolve(pvsProxy.cache_SR)};
            var prepopulationCell = new DataGridViewTextBoxCell
            {
                Value = pvsProxy.prepopulate ? Messages.YES : Messages.NO
            };

            var newRow = new DataGridViewRow { Tag = vm };
            newRow.Cells.AddRange(vmCell, cachedCell, srCell, prepopulationCell);
            vm.PropertyChanged += VmPropertyChanged;

            return newRow;
        }

        private void UnregisterVMHandlers()
        {
            foreach (DataGridViewRow row in dataGridViewVms.Rows)
            {
                var vm = row.Tag as VM;
                if (vm == null) 
                    continue;
                vm.PropertyChanged -= VmPropertyChanged;
            }
        }

        private void PvsFarmBatchCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, LoadFarms); 
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
                    row.Cells["columnVM"].Value = vm.Name;
            }
        }
    }
}
