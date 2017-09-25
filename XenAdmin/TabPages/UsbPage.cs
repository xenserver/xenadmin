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
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;


namespace XenAdmin.TabPages
{
    public partial class UsbPage : BaseTabPage
    {
        private Host _host;

        public UsbPage()
        {
            InitializeComponent();
            Text = Messages.USB;
        }

        public IXenObject XenObject
        {
            get
            {
                Program.AssertOnEventThread();
                return _host;
            }
            set
            {
                Program.AssertOnEventThread();

                UnregisterHandlers();

                if (value != null && value is Host)
                {
                    _host = (Host)value;

                    _host.PropertyChanged += Host_PropertyChanged;
                    _host.Connection.Cache.RegisterBatchCollectionChanged<PUSB>(UsbCollectionChanged);
                }

                BuildList();
            }
        }

        public bool InBuildList = false;
        private void BuildList()
        {
            Program.AssertOnEventThread();

            if (InBuildList)
                return;

            InBuildList = true;

            try
            {
                if (_host == null || _host.Locked)
                    return;

                if (!_host.Connection.CacheIsPopulated)
                    return;

                dataGridViewUsbList.SuspendLayout();
                dataGridViewUsbList.Rows.Clear();

                List<PUSB> pusbs = _host.Connection.ResolveAll(_host.PUSBs);

                var HostUsbRowToAdd = new List<HostUsbRow>();
                foreach (PUSB pusb in pusbs)
                {
                    HostUsbRowToAdd.Add(new HostUsbRow(pusb));
                }
                dataGridViewUsbList.Rows.AddRange(HostUsbRowToAdd.ToArray());
            }
            finally
            {
                if (dataGridViewUsbList.SortedColumn != null)
                {
                    dataGridViewUsbList.Sort(
                        dataGridViewUsbList.SortedColumn,
                        dataGridViewUsbList.SortOrder == SortOrder.Ascending
                            ? ListSortDirection.Ascending : ListSortDirection.Descending);
                }
                dataGridViewUsbList.ResumeLayout();
                InBuildList = false;
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }

        public override void PageHidden()
        {
            base.PageHidden();
        }

        void Host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshAllItems();
        }

        void UsbCollectionChanged(object sender, EventArgs e)
        {
            BuildList();
        }

        private void RefreshAllItems()
        {
            foreach (DataGridViewExRow row in dataGridViewUsbList.Rows)
            {
                if (row is HostUsbRow)
                    ((HostUsbRow)row).UpdateDetails();
            }
        }

        internal void UnregisterHandlers()
        {
            if (_host != null)
            {
                _host.PropertyChanged -= Host_PropertyChanged;
                _host.Connection.Cache.DeregisterBatchCollectionChanged<PUSB>(UsbCollectionChanged);
            }

            foreach (DataGridViewExRow row in dataGridViewUsbList.Rows)
            {
                if (row is HostUsbRow)
                    ((HostUsbRow)row).DeregisterEvents();
            }
        }

        private HostUsbRow selectedRow = null;
        private void dataGridViewUsbList_SelectionChanged(object sender, EventArgs e)
        {
            selectedRow = null;
            buttonPassthrough.Enabled = false;

            if (dataGridViewUsbList.SelectedRows != null && dataGridViewUsbList.SelectedRows.Count > 0)
            {
                selectedRow = (HostUsbRow)dataGridViewUsbList.SelectedRows[0];
                buttonPassthrough.Enabled = true;
            }

            if (selectedRow != null && ((HostUsbRow)selectedRow).Pusb.passthrough_enabled)
                buttonPassthrough.Text = Messages.USBLIST_DISABLE_PASSTHROUGH_HOTKEY;
            else
                buttonPassthrough.Text = Messages.USBLIST_ENABLE_PASSTHROUGH_HOTKEY;
        }

        private void buttonPassthrough_Click(object sender, EventArgs e)
        {
            if (selectedRow != null)
            {
                UsbUsageDialog dialog = new UsbUsageDialog(((HostUsbRow)selectedRow).Pusb);
                dialog.ShowDialog(Program.MainWindow);
            }
        }

        private class HostUsbRow : DataGridViewExRow
        {
            private PUSB _pusb;
            private DataGridViewTextBoxCell locationCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell descriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell passthroughCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell vmCell = new DataGridViewTextBoxCell();

            public HostUsbRow(PUSB pusb)
            {
                _pusb = pusb;
                _pusb.PropertyChanged += Pusb_PropertyChanged;

                Cells.AddRange(locationCell,
                    descriptionCell,
                    passthroughCell,
                    vmCell);
                UpdateDetails();
            }

            public PUSB Pusb
            {
                get { return _pusb; }
            }

            public void UpdateDetails()
            {
                locationCell.Value = _pusb.path;
                descriptionCell.Value = _pusb.description;
                passthroughCell.Value = _pusb.passthrough_enabled ? Messages.USB_ENABLED : Messages.USB_DISABLED;

                vmCell.Value = "";

                VUSB vusb = _pusb.Connection.Resolve(_pusb.attached);
                if (vusb != null)
                {
                    VM vm = vusb.Connection.Resolve(vusb.VM);
                    if (vm != null)
                    {
                        vmCell.Value = vm.name_label;
                    }
                }
            }

            public void DeregisterEvents()
            {
                _pusb.PropertyChanged -= Pusb_PropertyChanged;
            }

            private void Pusb_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                UpdateDetails();
            }
        }
    }
}
