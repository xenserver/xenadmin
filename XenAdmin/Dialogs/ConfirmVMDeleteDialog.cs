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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Dialogs
{
    public partial class ConfirmVMDeleteDialog : XenDialogBase
    {
        private const int MINIMUM_COL_WIDTH = 50;

        public ConfirmVMDeleteDialog(IEnumerable<VM> vms)
        {
            Util.ThrowIfParameterNull(vms, "vms");

            InitializeComponent();
            pictureBox1.Image = SystemIcons.Warning.ToBitmap();
            HelpButton = true;

            // We have to set the header text again here because they're in base64
            // encoding in the resx, so can't be easily localised: see CA-43371.
            listView.Groups["listViewGroupAttachedDisks"].Header = Messages.ATTACHED_VIRTUAL_DISKS;
            listView.Groups["listViewGroupSnapshots"].Header = Messages.SNAPSHOTS;

            List<VM> vmList = new List<VM>(vms);
            if (vmList.Count == 1)
            {
                String type = vmList[0].is_a_template ? Messages.TEMPLATE : Messages.VM;
                Text = String.Format(Messages.CONFIRM_DELETE_TITLE, type);
            }
            else
            {
                Text = Messages.CONFIRM_DELETE_ITEMS_TITLE;
            }

            List<VDI> sharedVDIsCouldBeDelete=new List<VDI>();
            foreach (VM vm in vmList)
            {
                foreach (VBD vbd in vm.Connection.ResolveAll(vm.VBDs))
                {
                    if (!vbd.IsCDROM)
                    {
                       
                        VDI vdi = vbd.Connection.Resolve(vbd.VDI);
                        if (vdi != null)
                        {
                            IList<VM> VMsUsingVDI = vdi.GetVMs();
                            bool allTheVMsAreDeleted = true;
                            if (VMsUsingVDI.Count > 1)
                            {
                                foreach (VM vmUsingVdi in VMsUsingVDI)
                                {
                                    if (!vmList.Contains(vmUsingVdi))
                                    {
                                        allTheVMsAreDeleted = false;
                                        break;
                                    }
                                }
                                if (allTheVMsAreDeleted&&!sharedVDIsCouldBeDelete.Contains(vdi))
                                {
                                    sharedVDIsCouldBeDelete.Add(vdi);
                                }

                            }
                            else
                            {
                                ListViewItem item = new ListViewItem();
                                item.Text = vdi.Name;
                                item.SubItems.Add(vm.Name);
                                item.Group = listView.Groups["listViewGroupAttachedDisks"];
                                item.Tag = vbd;
                                item.Checked = vbd.IsOwner;
                                foreach (ListViewItem.ListViewSubItem subitem in item.SubItems)
                                    subitem.Tag = subitem.Text;
                                listView.Items.Add(item);
                            }
                        }
                    }
                }
               
                foreach (VM snapshot in vm.Connection.ResolveAll(vm.snapshots))
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = snapshot.Name;
                    item.SubItems.Add(vm.Name);
                    item.Tag = snapshot;
                    item.Group = listView.Groups["listViewGroupSnapshots"];
                    foreach (ListViewItem.ListViewSubItem subitem in item.SubItems)
                        subitem.Tag = subitem.Text;
                    listView.Items.Add(item);
                }
            }
            foreach (VDI vdi in sharedVDIsCouldBeDelete)
            {
                ListViewItem item = new ListViewItem();
                item.Text = vdi.Name;
                item.SubItems.Add(vdi.VMsOfVDI);
                item.Group = listView.Groups["listViewGroupAttachedDisks"];
                item.Tag = vdi.Connection.ResolveAll(vdi.VBDs);
                item.Checked = false;
                foreach (ListViewItem.ListViewSubItem subitem in item.SubItems)
                    subitem.Tag = subitem.Text;
                listView.Items.Add(item);
            }
            EnableSelectAllClear();
            EllipsizeStrings();
        }

        public ConfirmVMDeleteDialog(VM vm)
            : this(new VM[] { vm })
        {
        }

        public List<VBD> DeleteDisks
        {
            get
            {
                List<VBD> vbds = new List<VBD>();
                foreach (ListViewItem item in listView.CheckedItems)
                {
                    VBD vbd = item.Tag as VBD;
                    if (vbd != null)
                        vbds.Add(vbd);
                    List<VBD> vbdsList = item.Tag as List<VBD>;
                    if (vbdsList != null)
                        vbds.AddRange(vbdsList);
                }
                return vbds;
            }
        }

        public List<VM> DeleteSnapshots
        {
            get
            {
                List<VM> snapshots = new List<VM>();
                foreach (ListViewItem item in listView.CheckedItems)
                {
                    VM snapshot = item.Tag as VM;
                    if (snapshot != null)
                        snapshots.Add(snapshot);
                }
                return snapshots;
            }
        }

        private void EllipsizeStrings()
        {
            foreach (ColumnHeader col in listView.Columns)
                EllipsizeStrings(col.Index);
        }

        private void EllipsizeStrings(int columnIndex)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (columnIndex < 0 || columnIndex >= item.SubItems.Count)
                    continue;

                var subItem = item.SubItems[columnIndex];

                string wholeText = subItem.Tag as string;
                if (wholeText == null)
                    continue;

                var rec = new Rectangle(subItem.Bounds.Left, subItem.Bounds.Top,
                                        listView.Columns[columnIndex].Width, subItem.Bounds.Height);
                
                subItem.Text = wholeText.Ellipsise(rec, subItem.Font);
                listView.Invalidate(rec);
            }
        }
        
        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = true;
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = false;
            }
        }

        private void EnableSelectAllClear()
        {
            bool allChecked = true;
            bool allUnChecked = true;

            foreach (ListViewItem item in listView.Items)
            {
                if (item.Checked)
                    allUnChecked = false;
                else
                    allChecked = false;
            }

            buttonSelectAll.Enabled = !allChecked;
            buttonClear.Enabled = !allUnChecked;
        }

        private void listView_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.ColumnIndex >= listView.Columns.Count)
                return;

            if (e.NewWidth < MINIMUM_COL_WIDTH)
            {
                e.NewWidth = MINIMUM_COL_WIDTH;
                e.Cancel = true;
            }
        }

        private void listView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            EllipsizeStrings(e.ColumnIndex);
        }

        private void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            EnableSelectAllClear();
        }

        private class ListViewDeleteDialog:ListView
        {
            private bool _b = false;
            private string _msg = Messages.EMPTY_LIST_DISK_SNAPSHOTS;
            protected override void WndProc(ref System.Windows.Forms.Message m)
            {
                base.WndProc(ref m);
                if (m.Msg == 20)
                {
                    if (Items.Count == 0)
                    {
                        _b = true;
                        using (Graphics g = CreateGraphics())
                        {
                            int w = (Width - g.MeasureString(_msg, Font).ToSize().Width) / 2;
                            g.DrawString(_msg, Font, SystemBrushes.ControlText, w, 30);
                        }
                    }
                    else
                    {
                        if (_b)
                        {
                            Invalidate();
                            _b = false;
                        }
                    }
                }

                if (m.Msg == 4127)
                    Invalidate();
            }
        }
    }
}