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
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Commands;
using XenAdmin.Model;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Wizards;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAdmin.Controls.DataGridViewEx;

namespace XenAdmin.Controls
{
    public partial class UsbList : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UsbList()
        {
            InitializeComponent();
        }

        private PUSB selectedUsb = null;

        private IXenObject _xenObject;
        public IXenObject XenObject
        {
            get
            {
                Program.AssertOnEventThread();
                return _xenObject;
            }
            set
            {
                Program.AssertOnEventThread();

                UnregisterHandlers();

                if (value is Host || value is VM)
                {
                    _xenObject = value;

                    if (null != _xenObject)
                    {
                        _xenObject.PropertyChanged += Server_PropertyChanged;

                        if (value is Host)
                        {
                            AddPUSBColumns();
                            _xenObject.Connection.Cache.RegisterBatchCollectionChanged<PUSB>(UsbCollectionChanged);
                        }
                        else  // value is VM
                        {
                            AddVUSBColumns();
                            _xenObject.Connection.Cache.RegisterBatchCollectionChanged<VUSB>(UsbCollectionChanged);
                        }
                    }
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
                if (XenObject == null || XenObject.Locked)
                    return;

                if (!XenObject.Connection.CacheIsPopulated)
                    return;

                if (XenObject is Host)
                {
                    Host host = XenObject as Host;

                    UsbGridView.SuspendLayout();
                    UsbGridView.Rows.Clear();

                    List<PUSB> pusbs = host.Connection.ResolveAll(host.PUSBs);

                    var HostUsbRowToAdd = new List<HostUsbRow>();
                    foreach (PUSB pusb in pusbs)
                    {
                        HostUsbRowToAdd.Add(new HostUsbRow(pusb));
                    }
                    UsbGridView.Rows.AddRange(HostUsbRowToAdd.ToArray());

                    buttonUsage.Visible = true;
                    buttonAttach.Visible = false;
                    buttonDetach.Visible = false;
                }
                else if (XenObject is VM)
                {
                    VM vm = XenObject as VM;

                    UsbGridView.SuspendLayout();
                    UsbGridView.Rows.Clear();

                    List<VUSB> vusbs = vm.Connection.ResolveAll(vm.VUSBs);

                    var VmUsbRowToAdd = new List<VMUsbRow>();
                    foreach (VUSB vusb in vusbs)
                    {
                        VmUsbRowToAdd.Add(new VMUsbRow(vusb));
                    }
                    UsbGridView.Rows.AddRange(VmUsbRowToAdd.ToArray());

                    buttonUsage.Visible = false;
                    buttonAttach.Visible = true;
                    buttonDetach.Visible = true;
                    buttonAttach.Enabled = true;
                    buttonDetach.Enabled = false;
                }
            }
            finally
            {
                if (UsbGridView.SortedColumn != null)
                {
                    UsbGridView.Sort(
                        UsbGridView.SortedColumn,
                        UsbGridView.SortOrder == SortOrder.Ascending
                            ? ListSortDirection.Ascending : ListSortDirection.Descending);
                }
                UsbGridView.ResumeLayout();
                InBuildList = false;
            }
        }

        private void AddPUSBColumns()
        {
            try
            {
                UsbGridView.SuspendLayout();

                UsbGridView.Columns.Clear();
                UsbGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                    this.LocationColumn,
                    this.DescriptionColumn,
                    this.UsageColumn,
                    this.VMColumn
                });

                this.LocationColumn.FillWeight = 10;
                this.DescriptionColumn.FillWeight = 43;
                this.UsageColumn.FillWeight = 25;
                this.VMColumn.FillWeight = 22;
            }
            finally
            {
                UsbGridView.ResumeLayout();
            }
        }

        private void AddVUSBColumns()
        {
            try
            {
                UsbGridView.SuspendLayout();

                UsbGridView.Columns.Clear();
                UsbGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                    this.LocationColumn,
                    this.DescriptionColumn,
                    this.AttachedColumn
                });
            }
            finally
            {
                UsbGridView.ResumeLayout();
            }
        }

        internal void UnregisterHandlers()
        {
            if (null != _xenObject)
            {
                _xenObject.PropertyChanged -= Server_PropertyChanged;
                _xenObject.Connection.Cache.DeregisterBatchCollectionChanged<PUSB>(UsbCollectionChanged);
            }

            foreach (DataGridViewExRow row in UsbGridView.Rows)
            {
                if (row is UsbRow)
                    ((UsbRow)row).DeregisterEvents();
            }
        }

        void UsbCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, BuildList);
        }

        void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Program.Invoke(this, RefreshAllItems);
        }

        private void RefreshAllItems()
        {
            foreach (DataGridViewExRow row in UsbGridView.Rows)
            {
                if (row is UsbRow)
                    ((UsbRow)row).UpdateDetails();
            }
        }

        private void buttonUsage_Click(object sender, EventArgs e)
        {
            UsbUsageDialog dialog = new UsbUsageDialog(selectedUsb);
            dialog.ShowDialog(Program.MainWindow);
        }

        private void buttonAttach_Click(object sender, EventArgs e)
        {
            if (_xenObject is VM)
            {
                AttachUsbDialog dialog = new AttachUsbDialog(_xenObject as VM);
                dialog.ShowDialog(Program.MainWindow);
            }
        }

        private void buttonDetach_Click(object sender, EventArgs e)
        {

        }

        private void UsbGridView_SelectionChanged(object sender, EventArgs e)
        {
            selectedUsb = null;

            if (UsbGridView.Rows.Count <= 0)
            {
                buttonUsage.Enabled = false;
            }
            else
            {
                try
                {
                    HostUsbRow row = (HostUsbRow)UsbGridView.SelectedRows[0];
                    selectedUsb = row.pusb;
                    buttonUsage.Enabled = true;
                }
                catch
                {
                    buttonUsage.Enabled = false;
                }
            }
        }

        class UsbRow : DataGridViewExRow
        {
            private IXenObject _xenObject;

            public UsbRow(IXenObject xo)
            {
                _xenObject = xo;
                xo.PropertyChanged += Server_PropertyChanged;
            }

            public virtual void UpdateDetails()
            {

            }

            public void DeregisterEvents()
            {
                _xenObject.PropertyChanged -= Server_PropertyChanged;
            }

            private void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                Program.Invoke(Program.MainWindow, UpdateDetails);
            }
        }

        class HostUsbRow : UsbRow
        {
            private PUSB _pusb;

            private DataGridViewTextBoxCell locationCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell descriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell usageCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell vmCell = new DataGridViewTextBoxCell();

            public PUSB pusb
            {
                get { return _pusb; }
            }

            public HostUsbRow(PUSB pusb): base(pusb)
            {
                _pusb = pusb;

                Cells.AddRange(locationCell,
                    descriptionCell,
                    usageCell,
                    vmCell);
                Program.Invoke(Program.MainWindow, UpdateDetails);
            }

            public override void UpdateDetails()
            {
                locationCell.Value = _pusb.path;
                descriptionCell.Value = _pusb.description;
                usageCell.Value = "No";
                vmCell.Value = "";

                if (_pusb.passthrough_enabled)
                    usageCell.Value = "Yes";

                VUSB vusb = _pusb.Connection.Resolve(_pusb.attached);
                if (null != vusb)
                {
                    VM vm = vusb.Connection.Resolve(vusb.VM);
                    if (null != vm)
                    {
                        vmCell.Value = vm.name_label;
                    }
                }
            }
        }

        class VMUsbRow : UsbRow
        {
            private VUSB _vusb;
            private USB_group _usbgroup;
            private PUSB _pusb;

            private DataGridViewTextBoxCell locationCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell descriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell attachedCell = new DataGridViewTextBoxCell();

            public VUSB vusb
            {
                get { return _vusb; }
            }

            public VMUsbRow(VUSB vusb): base(vusb)
            {
                _vusb = vusb;

                _usbgroup = _vusb.Connection.Resolve(_vusb.USB_group);
                _pusb = _vusb.Connection.Resolve(_usbgroup.PUSBs[0]);

                Cells.AddRange(locationCell,
                    descriptionCell,
                    attachedCell);
                Program.Invoke(Program.MainWindow, UpdateDetails);
            }

            public override void UpdateDetails()
            {
                locationCell.Value = _pusb.path;
                descriptionCell.Value = _pusb.description;

                if (null != _vusb.attached)
                    attachedCell.Value = "Yes";
                else
                    attachedCell.Value = "No";
            }
        }
    }
}
