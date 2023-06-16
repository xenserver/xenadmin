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
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Mappings;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib;

namespace XenAdmin.Wizards.GenericPages
{
    /// <summary>
    /// Class representing the page of the ImportAppliance wizard where the user specifies
    /// a network for the VMs in the appliance that require network access. 
    /// </summary>
    internal abstract partial class SelectMultipleVMNetworkPage : XenTabPage
    {
        private bool m_buttonNextEnabled;
        private bool m_buttonPreviousEnabled;
        private Dictionary<string, VmMapping> m_vmMappings;

        private bool _runWorkerAgain;
        private readonly object _workerRunLock = new object();

        private struct NetworkDetail
        {
            public string SysId { get; set; }
            public string NetworkId { get; set; }
            public string MACAddress { get; set; }
        }

        protected SelectMultipleVMNetworkPage()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            m_labelIntro.Text = IntroductionText;
            label2.Text = TableIntroductionText;
            m_colVmNetwork.HeaderText = NetworkColumnHeaderText;
            m_checkBoxMac.Visible = ShowReserveMacAddressesCheckBox;
            m_buttonRefresh.Visible = ShowRefreshButton;
        }

        protected virtual string NetworkColumnHeaderText => m_colVmNetwork.HeaderText;
        protected virtual bool ShowReserveMacAddressesCheckBox => false;
        protected virtual bool ShowRefreshButton => false;
        protected virtual bool LoadsRemoteData => false;

        protected abstract string IntroductionText { get; }
        protected abstract string TableIntroductionText { get; }
        protected abstract NetworkResourceContainer NetworkData(string sysId);

        protected virtual bool AllowSriovNetwork(XenAPI.Network network, string sysId)
        {
            return true;
        }

        protected virtual void LoadNetworkData()
        {
        }

        private IXenConnection targetConnection;

        /// <summary>
        /// The connection from which the target networks are selected
        /// Defaults to the base class connection if not set
        /// </summary>
        public IXenConnection TargetConnection
        {
            get
            {
                if (targetConnection == null)
                    return Connection;
                return targetConnection;
            }
            set => targetConnection = value;
        }

        public bool PreserveMAC => ShowReserveMacAddressesCheckBox && m_checkBoxMac.Checked;

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            targetConnection = null;
            backgroundWorker1.CancelAsync();
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            Populate();
            SetButtonPreviousEnabled(true);
            SetButtonNextEnabled(true);
        }

        public override void PageCancelled(ref bool cancel)
        {
            backgroundWorker1.CancelAsync();
        }

        private void Populate()
        {
            m_dataGridView.Rows.Clear();
            SetButtonNextEnabled(false);

            if (LoadsRemoteData)
            {
                m_buttonRefresh.Enabled = false;
                pictureBoxError.Image = Images.StaticImages.ajax_loader;
                labelError.Text = Messages.CONVERSION_NETWORK_PAGE_QUERYING_NETWORKS;
                tableLayoutPanelError.Visible = true;
                lock (_workerRunLock)
                {
                    if (backgroundWorker1.IsBusy)
                    {
                        _runWorkerAgain = true;
                    }
                    else
                    {
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
            }
            else
            {
                FillTableRows();
                HelpersGUI.ResizeGridViewColumnToAllCells(m_colTargetNet); //set properly the width of the last column
            }
        }

        protected virtual void FillTableRows()
        {
            foreach (var kvp in VmMappings)
            {
                var sysId = kvp.Key;
                var vmMapping = kvp.Value;

                FillTableRow(vmMapping.XenRef, sysId, vmMapping.VmNameLabel);
            }
        }

        protected void FillTableRow(object targetRef, string sysId, string vmName)
        {
            var cb = FillGridComboBox(targetRef, sysId);
            var networkData = NetworkData(sysId);

            foreach (INetworkResource networkResource in networkData)
            {
                var val = networkResource.NetworkName;
                var vmNameOverride = networkResource.VmNameOverride;

                if (!string.IsNullOrEmpty(vmNameOverride))
                    val = $"{vmNameOverride} - {val}";
                else if (!string.IsNullOrEmpty(vmName))
                    val = $"{vmName} - {val}";

                if (!string.IsNullOrEmpty(networkResource.MACAddress))
                    val = $"{val} ({networkResource.MACAddress})";

                var cellSourceNetwork = new DataGridViewTextBoxCell
                {
                    Tag = new NetworkDetail
                    {
                        SysId = sysId, NetworkId = networkResource.NetworkID, MACAddress = networkResource.MACAddress
                    },
                    Value = val
                };

                var row = new DataGridViewRow();
                row.Cells.Add(cellSourceNetwork);

                var cbClone = (DataGridViewComboBoxCell)cb.Clone();

                if (cbClone.Items.Count > 0)
                {
                    cbClone.DisplayMember = ToStringWrapper<XenAPI.Network>.DisplayMember; //ToStringProperty
                    cbClone.ValueMember =
                        ToStringWrapper<XenAPI.Network>.ValueMember; //ToStringWrapper<XenAPI.Network> object itself
                    cbClone.Value = cb.Items[0]; // Default selection of the combobox cell

                    //Select the network if the names of the target and source networks match in the combobox cell
                    foreach (ToStringWrapper<XenAPI.Network> item in cb.Items)
                    {
                        if (item.ToStringProperty == networkResource.NetworkName)
                            cbClone.Value = item;
                    }

                    row.Cells.Add(cbClone);
                }
                else
                {
                    var cellError = new DataGridViewTextBoxCell
                        { Value = Messages.IMPORT_SELECT_NETWORK_PAGE_NO_AVAIL_NETWORKS };
                    row.Cells.Add(cellError);
                    cellError.ReadOnly = true; //this has to be set after the cell is added to a row
                    SetButtonNextEnabled(false);
                }

                m_dataGridView.Rows.Add(row);
            }
        }

        public override bool EnableNext()
        {
            return m_buttonNextEnabled;
        }

        public override bool EnablePrevious()
        {
            return m_buttonPreviousEnabled;
        }

        public Dictionary<string, VmMapping> VmMappings
        {
            get
            {
                foreach (DataGridViewRow row in m_dataGridView.Rows)
                {
                    var networkDetail = (NetworkDetail)row.Cells[0].Tag;

                    if (m_vmMappings.ContainsKey(networkDetail.SysId))
                    {
                        var mapping = m_vmMappings[networkDetail.SysId];

                        if (row.Cells[1].Value is ToStringWrapper<XenAPI.Network> selectedItem)
                        {
                            mapping.Networks[networkDetail.NetworkId] = selectedItem.item;
                            mapping.VIFs[networkDetail.MACAddress] = selectedItem.item;
                        }
                    }
                }

                return m_vmMappings;
            }
            set => m_vmMappings = value;
        }

        public Dictionary<string, string> RawMappings
        {
            get
            {
                var mappings = new Dictionary<string, string>();

                foreach (DataGridViewRow row in m_dataGridView.Rows)
                {
                    var networkDetail = (NetworkDetail)row.Cells[0].Tag;
                    var selectedItem = row.Cells[1].Value as ToStringWrapper<XenAPI.Network>;
                    mappings.Add(networkDetail.NetworkId, selectedItem?.ToString());
                }

                return mappings;
            }
        }

        protected void SetButtonNextEnabled(bool enabled)
        {
            m_buttonNextEnabled = enabled;
            OnPageUpdated();
        }

        protected void SetButtonPreviousEnabled(bool enabled)
        {
            m_buttonPreviousEnabled = enabled;
            OnPageUpdated();
        }

        private DataGridViewComboBoxCell FillGridComboBox(object targetRef, string vsId)
        {
            var cb = new DataGridViewComboBoxCell { FlatStyle = FlatStyle.Flat, Sorted = true };

            var availableNetworks = TargetConnection.Cache.Networks.Where(net => ShowNetwork(targetRef, net, vsId));

            foreach (var netWork in availableNetworks)
            {
                if (!Messages.IMPORT_SELECT_NETWORK_PAGE_NETWORK_FILTER.Contains(netWork.Name()))
                {
                    var wrapperItem = new ToStringWrapper<XenAPI.Network>(netWork, netWork.Name());

                    if (!cb.Items.Contains(wrapperItem))
                        cb.Items.Add(wrapperItem);
                }
            }

            return cb;
        }

        private bool ShowNetwork(object targetRef, XenAPI.Network network, string vsId)
        {
            if (network.IsSriov() && !AllowSriovNetwork(network, vsId))
                return false;

            if (!network.Show(Properties.Settings.Default.ShowHiddenVMs))
                return false;

            if (network.IsMember())
                return false;

            var targetHostRef = targetRef as XenRef<Host>;
            var targetHost = targetHostRef == null ? null : TargetConnection.Resolve(targetHostRef);

            if (targetHost != null && !targetHost.CanSeeNetwork(network))
                return false;

            if (targetHost == null && !network.AllHostsCanSeeNetwork())
                return false;

            return true;
        }

        private void m_dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != m_colTargetNet.Index || e.RowIndex < 0 || e.RowIndex >= m_dataGridView.RowCount)
                return;

            m_dataGridView.BeginEdit(false);

            if (m_dataGridView.EditingControl is ComboBox box)
                box.DroppedDown = true;
        }

        private void m_dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            m_dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            IsDirty = true;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            PopulatePage();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadNetworkData();
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            lock (_workerRunLock)
            {
                if (_runWorkerAgain)
                {
                    _runWorkerAgain = false;
                    backgroundWorker1.RunWorkerAsync();
                    return;
                }
            }

            if (e.Cancelled)
            {
                tableLayoutPanelError.Visible = false;
            }
            else if (e.Error != null)
            {
                pictureBoxError.Image = Images.StaticImages._000_error_h32bit_16;
                labelError.Text = Messages.CONVERSION_NETWORK_PAGE_QUERYING_NETWORKS_FAILURE;
            }
            else
            {
                tableLayoutPanelError.Visible = false;
                FillTableRows();
                HelpersGUI.ResizeGridViewColumnToAllCells(m_colTargetNet); //set properly the width of the last column
            }

            m_buttonRefresh.Enabled = true;
        }
    }
}