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

using XenAdmin.Core;
using XenAdmin.Model;
using System.Text.RegularExpressions;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Controls;
using XenAdmin.Network;
using XenAdmin.Help;
using XenAdmin.Actions;
using XenAdmin.Commands;


namespace XenAdmin.TabPages
{
    internal partial class NICPage : BaseTabPage
    {
        private Host host = null;

        public NICPage()
        {
            InitializeComponent();


            base.Text = Messages.NIC_TAB_TITLE;
            PIF_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(PIF_CollectionChanged);
        }

        private readonly CollectionChangeEventHandler PIF_CollectionChangedWithInvoke;

        public Host Host
        {
            get
            {
                return host;
            }
            set
            {
                UnregisterHandlers();

                host = value;

                if (host != null)
                {
                    host.Connection.Cache.RegisterCollectionChanged<PIF>(PIF_CollectionChangedWithInvoke);
                }

                updateList();
                CreateBondButton.Enabled = host != null;
            }
        }

        void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            Program.Invoke(Program.MainWindow, updateList);
        }

        void PIF_PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "current_operations" &&
                e.PropertyName != "allowed_operations")
            {
                Program.Invoke(Program.MainWindow, updateList);
            }
        }

        void PIF_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Remove)
            {
                PIF pif = e.Element as PIF;
                UnregisterPIFEventHandlers(pif);
            }
            Program.Invoke(this, updateList);
        }

        private void RegisterPIFEventHandlers(PIF pif)
        {
            pif.PropertyChanged -= PIF_PropertyChangedEventHandler;
            pif.PropertyChanged += PIF_PropertyChangedEventHandler;

            if (pif.PIFMetrics != null)
            {
                pif.PIFMetrics.PropertyChanged -= PropertyChangedEventHandler;
                pif.PIFMetrics.PropertyChanged += PropertyChangedEventHandler;
            }
        }

        private void UnregisterPIFEventHandlers(PIF pif)
        {
            pif.PropertyChanged -= PIF_PropertyChangedEventHandler;
            if (pif.PIFMetrics != null)
            {
                pif.PIFMetrics.PropertyChanged -= PropertyChangedEventHandler;
            }
        }

        private void UnregisterHandlers()
        {
            if (host == null)
                return;

            host.Connection.Cache.DeregisterCollectionChanged<PIF>(PIF_CollectionChangedWithInvoke);
            foreach (PIF PIF in host.Connection.ResolveAll(host.PIFs))
            {
                UnregisterPIFEventHandlers(PIF);
            }
        }

        public override void PageHidden()
        {
            UnregisterHandlers();
        }


        private void updateList()
        {
            PIFRow selected = null;
            if (dataGridView1.SelectedRows.Count > 0)
                selected = dataGridView1.SelectedRows[0] as PIFRow;

            try
            {
                dataGridView1.SuspendLayout();
                dataGridView1.Rows.Clear();

                if (host != null)
                {
                    foreach (PIF PIF in host.Connection.ResolveAll(host.PIFs))
                    {
                        if (!PIF.IsPhysical)
                            continue;

                        RegisterPIFEventHandlers(PIF);

                        PIFRow p = new PIFRow(PIF);
                        dataGridView1.Rows.Add(p);
                        if (selected != null && p.pif == selected.pif)
                            p.Selected = true;
                    }

                    //show the FCoE column for Dundee or higher hosts only
                    ColumnFCoECapable.Visible = Helpers.DundeeOrGreater(host);

                    //CA-47050: the Device column should be autosized to Fill, but should not become smaller than a minimum
                    //width, which here is chosen to be the column header width. To find what this width is 
                    //set temporarily the column's autosize mode to ColumnHeader.
                    ColumnDeviceName.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                    int storedWidth = ColumnDeviceName.Width;
                    ColumnDeviceName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    ColumnDeviceName.MinimumWidth = storedWidth;

                    if (dataGridView1.SortedColumn != null)
                        dataGridView1.Sort(dataGridView1.SortedColumn, dataGridView1.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
                }
            }
            finally
            {
                dataGridView1.ResumeLayout();
            }
        }

        public class PIFRow : DataGridViewRow
        {
            public PIF pif;
            string vendor = Messages.HYPHEN;
            string device = Messages.HYPHEN;
            string busPath = Messages.HYPHEN;

            public PIFRow(PIF pif)
            {
                this.pif = pif;
                PIF_metrics PIFMetrics = pif.PIFMetrics;
                if (PIFMetrics != null)
                {
                    vendor = PIFMetrics.vendor_name;
                    device = PIFMetrics.device_name;
                    busPath = PIFMetrics.pci_bus_path;
                }
                for (int i = 0; i < 9; i++)
                {
                    Cells.Add(new DataGridViewTextBoxCell());
                    updateCell(i);
                }

            }

            private void updateCell(int index)
            {
                switch (index)
                {
                    case 0:
                        Cells[0].Value = pif.Name;
                        break;
                    case 1:
                        Cells[1].Value = pif.MAC;
                        break;
                    case 2:
                        Cells[2].Value = pif.Carrier ? Messages.CONNECTED : Messages.DISCONNECTED;
                        break;
                    case 3:
                        Cells[3].Value = pif.Carrier ? pif.Speed : Messages.HYPHEN;
                        break;
                    case 4:
                        Cells[4].Value = pif.Carrier ? pif.Duplex : Messages.HYPHEN;
                        break;
                    case 5:
                        Cells[5].Value = vendor;
                        break;
                    case 6:
                        Cells[6].Value = device;
                        break;
                    case 7:
                        Cells[7].Value = busPath;
                        break;
                    case 8:
                        Cells[8].Value = pif.FCoECapable ? Messages.YES : Messages.NO;
                        break;
                }
            }
        }

        private void datagridview_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                DeleteBondButton.Enabled = false;
                return;
            }
            PIF pif = ((PIFRow)dataGridView1.SelectedRows[0]).pif;
            DeleteBondButton.Enabled = pif != null && pif.BondMasterOf != null && !pif.BondMasterOf.Locked;
        }

        private void CreateBondButton_Click(object sender, EventArgs e)
        {
            BondProperties dialog = new BondProperties();
            dialog.SetHost(host);
            dialog.ShowDialog(Program.MainWindow);
        }

        private void DeleteBondButton_Click(object sender, EventArgs e)
        {
            PIF pif = ((PIFRow)dataGridView1.SelectedRows[0]).pif;
            System.Diagnostics.Trace.Assert(pif.IsBondNIC);
            XenAPI.Network network = pif.Connection.Resolve(pif.network);
            var destroyBondCommand = new DestroyBondCommand(Program.MainWindow, network);
            destroyBondCommand.Execute();
        }

        private void buttonRescan_Click(object sender, EventArgs e)
        {
            RescanPIFsCommand cmd = new RescanPIFsCommand(Program.MainWindow, host);
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void CopyItemClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(dataGridView1.SelectedRows[0].Cells[1].Value);
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            DataGridView.HitTestInfo i = dataGridView1.HitTest(e.X, e.Y);
            if (i == DataGridView.HitTestInfo.Nowhere||i.RowIndex<0||i.ColumnIndex<0)
                return;
            dataGridView1.Rows[i.RowIndex].Selected = true;
            dataGridView1.Focus();

            contextMenuStrip1.Items.Clear();
            ToolStripMenuItem deleteItem = new ToolStripMenuItem(DeleteBondButton.Text, null, DeleteBondButton_Click);
            ToolStripMenuItem copyItem = new ToolStripMenuItem(Messages.COPY_MENU_ITEM, null, CopyItemClick);
            deleteItem.Enabled = DeleteBondButton.Enabled;
            contextMenuStrip1.Items.Add(deleteItem);
            contextMenuStrip1.Items.Add(new ToolStripSeparator());
            contextMenuStrip1.Items.Add(copyItem);

            contextMenuStrip1.Show(dataGridView1, dataGridView1.PointToClient(Control.MousePosition));
        }
    }
}
