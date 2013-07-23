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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Actions;

using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Commands;

namespace XenAdmin.TabPages
{
    internal partial class SrStoragePage : BaseTabPage
    {
        private SR sr;
        private readonly DataGridViewColumn sizeColumn;
        private readonly DataGridViewColumn storageLinkVolumeColumn;
        private bool rebuildRequired;

        private readonly VDIsDataGridViewBuilder dataGridViewBuilder;

        public SrStoragePage()
        {
            InitializeComponent();

            storageLinkVolumeColumn = ColumnVolume;
            sizeColumn = ColumnSize;
            for (int i = 0; i < 5; i++)
            {
                dataGridViewVDIs.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
            }

            dataGridViewVDIs.SortCompare += DataGridViewObject_SortCompare;
            dataGridViewVDIs.SelectionChanged += dataGridViewVDIs_SelectedIndexChanged;
            dataGridViewVDIs.MouseUp += dataGridViewVDIs_MouseUp;
            dataGridViewVDIs.KeyUp += dataGridViewVDIs_KeyUp;

            ConnectionsManager.History.CollectionChanged += new CollectionChangeEventHandler(History_CollectionChanged);

            base.Text = Messages.VIRTUAL_DISKS;

            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;
            
            dataGridViewBuilder = new VDIsDataGridViewBuilder(this);
        }

        private bool disposed;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                // Deregister listeners.
                SR = null;

                ConnectionsManager.History.CollectionChanged -= History_CollectionChanged;
                Properties.Settings.Default.PropertyChanged -= Default_PropertyChanged;

                if (disposing)
                {
                    if (components != null)
                        components.Dispose();

                    dataGridViewBuilder.Stop();
                }

                disposed = true;
            }
            base.Dispose(disposing);
        }

        private void SetupDeprecationBanner()
        {
            if (sr != null && SR.IsIslOrIslLegacy(sr))
            {
                Banner.AppliesToVersion = Messages.XENSERVER_6_2;
                Banner.BannerType = DeprecationBanner.Type.Deprecation;
                Banner.FeatureName = Messages.ISL_SR;
                Banner.LinkUri = new Uri(InvisibleMessages.ISL_DEPRECATION_URL);
                Banner.Visible = true;
            }
            else
                Banner.Visible = false;
        }

        public SR SR
        {
            set
            {
                Program.AssertOnEventThread();

                if (sr != null)
                {
                    sr.PropertyChanged -= sr_PropertyChanged;
                    sr.Connection.Cache.DeregisterBatchCollectionChanged<VDI>(VDI_BatchCollectionChanged);
                    sr.Connection.XenObjectsUpdated -= Connection_XenObjectsUpdated;
                }

                sr = value;
                
                if (sr != null)
                {
                    sr.PropertyChanged += sr_PropertyChanged;
                    sr.Connection.Cache.RegisterBatchCollectionChanged<VDI>(VDI_BatchCollectionChanged);
                    addVirtualDiskButton.Visible = sr.SupportsVdiCreate();
                    sr.Connection.XenObjectsUpdated += Connection_XenObjectsUpdated;
                }

                // removeDropDownButton and addDropDownButton are for storagelink. They contain extra stuff for adding
                // and removing SL volumes. 

                addDropDownButton.Visible = sr != null && sr.type == "cslg" && sr.StorageLinkRepository(Program.StorageLinkConnections) != null;
                addVirtualDiskButton.Visible &= !addDropDownButton.Visible;

                removeDropDownButton.Visible = addDropDownButton.Visible;
                RemoveButtonContainer.Visible = !removeDropDownButton.Visible;

                BuildList(true);
                SetupDeprecationBanner();
            }
        }

        private void RefreshDataGridView(VDIsData data)
        {
            dataGridViewVDIs.SuspendLayout();
            try
            {
                storageLinkVolumeColumn.Visible = data.ShowStorageLink;

                // Update existing rows
                foreach (var vdiRow in data.VdiRowsToUpdate)
                {
                    vdiRow.RefreshRowDetails(data.ShowStorageLink);
                }

                // Remove rows for deleted VDIs
                foreach (var vdiRow in data.VdiRowsToRemove)
                {
                    dataGridViewVDIs.Rows.RemoveAt(vdiRow.Index);
                }

                // Add rows for new VDIs
                foreach (var vdi in data.VdisToAdd)
                {
                    VDIRow newRow = new VDIRow(vdi, data.ShowStorageLink);
                    dataGridViewVDIs.Rows.Add(newRow);   
                }
            }
            finally
            {
                if (dataGridViewVDIs.SortedColumn != null && dataGridViewVDIs.SortOrder != SortOrder.None)
                    dataGridViewVDIs.Sort(dataGridViewVDIs.SortedColumn,
                                          dataGridViewVDIs.SortOrder == SortOrder.Ascending
                                              ? ListSortDirection.Ascending
                                              : ListSortDirection.Descending);

                dataGridViewVDIs.ResumeLayout();
            }

            RefreshButtons();
        }

        private IEnumerable<VDIRow> GetCurrentVDIRows()
        {
            return dataGridViewVDIs.Rows.OfType<VDIRow>();
        }

        private void BuildList(bool reset)
        {
            Program.AssertOnEventThread();

            if (sr == null)
                return;

            dataGridViewBuilder.AddRequest(new RefreshGridRequest(sr, reset));
        }

        private SelectedItemCollection SelectedVDIs
        {
            get
            {
                List<SelectedItem> vdis = new List<SelectedItem>();
                foreach (DataGridViewRow r in dataGridViewVDIs.SelectedRows)
                {
                    VDIRow row = r as VDIRow;
                    if (row != null)
                        vdis.Add(new SelectedItem(row.VDI));
                }
                return new SelectedItemCollection(vdis);
            }
        }

        #region events
        void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(Program.MainWindow, () =>
                                                        {
                                                            SrRefreshAction a = e.Element as SrRefreshAction;
                                                            if (a == null)
                                                                return;

                                                            if (e.Action == CollectionChangeAction.Add)
                                                                a.Completed += new EventHandler<EventArgs>(a_Completed);

                                                            if (e.Action == CollectionChangeAction.Remove)
                                                                a.Completed -= new EventHandler<EventArgs>(a_Completed);

                                                            RefreshButtons();
                                                        });
        }

        void a_Completed(object sender, EventArgs e)
        {
            Program.Invoke(Program.MainWindow, RefreshButtons);
        }

        void Connection_XenObjectsUpdated(object sender, EventArgs e)
        {
            if (rebuildRequired)
                BuildList(false);

            rebuildRequired = false;
        }

        void sr_PropertyChanged(object sender1, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VDIs")
                rebuildRequired = true;
        }

        private void VDI_BatchCollectionChanged(object sender, EventArgs e)
        {
            rebuildRequired = true;
        }

        private void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ShowHiddenVMs")
                return;
            Program.Invoke(this, () => BuildList(false));
        }
        #endregion

        #region datagridviewevents
        void DataGridViewObject_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == sizeColumn.Index)
            {
                VDI vdi1 = ((VDIRow)dataGridViewVDIs.Rows[e.RowIndex1]).VDI;
                VDI vdi2 = ((VDIRow)dataGridViewVDIs.Rows[e.RowIndex2]).VDI;
                long diff = vdi1.virtual_size - vdi2.virtual_size;
                e.SortResult =
                    diff > 0 ? 1 :
                    diff < 0 ? -1 :
                               0;
                e.Handled = true;
            }
        }

        private void dataGridViewVDIs_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshButtons();
        }

        private void dataGridViewVDIs_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Apps)
                return;

            if (dataGridViewVDIs.SelectedRows.Count <= 0)
                return;

            DataGridViewRow row = dataGridViewVDIs.SelectedRows[0];

            contextMenuStrip1.Show(dataGridViewVDIs, 5, row.Height * (row.Index + 2));
        }

        private void dataGridViewVDIs_MouseUp(object sender, MouseEventArgs e)
        {
            // Load context menus on right mouse click
            DataGridView.HitTestInfo hitTestInfo;
            if (e.Button == MouseButtons.Right)
            {
                hitTestInfo = dataGridViewVDIs.HitTest(e.X, e.Y);
                if (hitTestInfo.Type == DataGridViewHitTestType.Cell)
                {
                    if (dataGridViewVDIs.Rows.Count >= 0)
                    {
                        if (!dataGridViewVDIs.Rows[hitTestInfo.RowIndex].Selected)
                        {
                            // Select the row that the user right clicked on (similiar to outlook) if it's not already in the selection
                            // (avoids clearing a multiselect if you right click inside it)
                            dataGridViewVDIs.CurrentCell = dataGridViewVDIs[hitTestInfo.ColumnIndex, hitTestInfo.RowIndex];
                        }
                        // Show the menus.  We will always have at least one (Add).
                        contextMenuStrip1.Show(dataGridViewVDIs, new Point(e.X, e.Y));
                    }
                }
            }
        }
        #endregion

        #region Button and context menu population
        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            SelectedItemCollection vdis = SelectedVDIs;
            if (vdis.Count == 0)
                return;

            contextMenuStrip1.Items.Clear();

            DeleteVirtualDiskCommand deleteCmd = new DeleteVirtualDiskCommand(Program.MainWindow.CommandInterface, vdis);
            contextMenuStrip1.Items.Add(new CommandToolStripMenuItem(deleteCmd, true));

            contextMenuStrip1.Items.Add(new CommandToolStripMenuItem(MoveMigrateCommand(vdis), true));

            contextMenuStrip1.Items.Add(editVirtualDiskToolStripMenuItem);
            editVirtualDiskToolStripMenuItem.Enabled = vdis.Count == 1 && !vdis.AsXenObjects<VDI>()[0].is_a_snapshot;
        }

        private Command MoveMigrateCommand(IEnumerable<SelectedItem> selection)
        {
            MoveVirtualDiskCommand moveCmd = new MoveVirtualDiskCommand(Program.MainWindow.CommandInterface, selection);

            if (moveCmd.CanExecute())
                return moveCmd;
            
            return new MigrateVirtualDiskCommand(Program.MainWindow.CommandInterface, selection);
        }

        private void RefreshButtons()
        {
            bool srLocked = sr == null || sr.Locked;
            SelectedItemCollection vdis = SelectedVDIs;

            // Delete button
            DeleteVirtualDiskCommand deleteCmd = new DeleteVirtualDiskCommand(Program.MainWindow.CommandInterface, vdis);
            // User has visibility that this disk is related to all it's VM. Allow deletion with multiple VBDs (non default behaviour),
            // but don't allow them to delete if a running vm is using the disk (default behaviour).
            deleteCmd.AllowMultipleVBDDelete = true;
            if (deleteCmd.CanExecute())
            {
                RemoveButton.Enabled = true;
                RemoveButtonContainer.RemoveAll();
            }
            else
            {
                RemoveButton.Enabled = false;
                RemoveButtonContainer.SetToolTip(deleteCmd.ToolTipText);
            }

            // Move button
            Command moveCmd = MoveMigrateCommand(vdis);
            if (moveCmd.CanExecute())
            {
                buttonMove.Enabled = true;
                toolTipContainerMove.RemoveAll();
            }
            else
            {
                buttonMove.Enabled = false;
                toolTipContainerMove.SetToolTip(moveCmd.ToolTipText);
            }

            // Rescan button
            if (srLocked)
            {
                buttonRefresh.Enabled = false;
            }
            else if (HelpersGUI.BeingScanned(sr))
            {
                buttonRefresh.Enabled = false;
                toolTipContainerRescan.SetToolTip(Messages.SCAN_IN_PROGRESS_TOOLTIP);
            }
            else
            {
                buttonRefresh.Enabled = true;
                toolTipContainerRescan.RemoveAll();
            }

            // Add VDI button
            addVirtualDiskButton.Enabled = !srLocked;

            // Properties button
            if (vdis.Count == 1)
            {
                VDI vdi = vdis.AsXenObjects<VDI>()[0];
                EditButton.Enabled = !srLocked && !vdi.is_a_snapshot && !vdi.Locked;
            }
            else
                EditButton.Enabled = false;

            // Remove drop down button
            removeDropDownButton.Enabled = deleteCmd.CanExecute();

        }

        private void removeContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            removeContextMenuStrip.Items.Clear();
            SelectedItemCollection vdis = SelectedVDIs;
            if (vdis.Count > 0)
            {
                Command cmd = new DeleteVirtualDiskCommand(Program.MainWindow.CommandInterface, vdis);
                removeContextMenuStrip.Items.Add(new CommandToolStripMenuItem(cmd, Messages.DELETE_VIRTUAL_DISK));

                cmd = new RemoveStorageLinkVolumeCommand(Program.MainWindow.CommandInterface, sr.StorageLinkRepository(Program.StorageLinkConnections), vdis);
                removeContextMenuStrip.Items.Add(new CommandToolStripMenuItem(cmd, Messages.DELETE_SL_VOLUME_CONTEXT_MENU_ELIPS));
            }
        }

        private void addContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            addContextMenuStrip.Items.Clear();

            Command cmd = new AddVirtualDiskCommand(Program.MainWindow.CommandInterface, sr);
            addContextMenuStrip.Items.Add(new CommandToolStripMenuItem(cmd, Messages.ADD_VIRTUAL_DISK));

            cmd = new ImportStorageLinkVolumeCommand(Program.MainWindow.CommandInterface, sr.StorageLinkRepository(Program.StorageLinkConnections));
            addContextMenuStrip.Items.Add(new CommandToolStripMenuItem(cmd, Messages.ADD_SL_VOLUME));
        }
        #endregion

        #region buttonHandlers
        private void removeVirtualDisk_Click(object sender, EventArgs e)
        {
            SelectedItemCollection vdis = SelectedVDIs;
            DeleteVirtualDiskCommand cmd = new DeleteVirtualDiskCommand(Program.MainWindow.CommandInterface, vdis);
            cmd.AllowMultipleVBDDelete = true;
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void editVirtualDiskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedItemCollection vdis = SelectedVDIs;
            if (vdis.Count != 1)
                return;

            VDI vdi = vdis.AsXenObjects<VDI>()[0];
            if (vdi.is_a_snapshot)
                return;

            new PropertiesDialog(vdi).ShowDialog(this);
        }

        private void addVirtualDiskButton_Click(object sender, EventArgs e)
        {
            if (sr != null)
                Program.MainWindow.ShowPerConnectionWizard(sr.Connection, new Dialogs.NewDiskDialog(sr.Connection, sr));
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            removeVirtualDisk_Click(sender, e);
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            editVirtualDiskToolStripMenuItem_Click(sender, e);
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            SrRefreshAction a = new SrRefreshAction(sr);
            a.RunAsync();
        }

        private void moveVirtualDiskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedItemCollection vdis = SelectedVDIs;
            Command cmd = MoveMigrateCommand(vdis);
            if (cmd.CanExecute())
                cmd.Execute();
        }

        private void buttonMove_Click(object sender, EventArgs e)
        {
            moveVirtualDiskToolStripMenuItem_Click(sender, e);
        }
        #endregion

        private void dataGridViewVDIs_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Select the row that the user right clicked on if it's not already in the selection
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                if (!dataGridViewVDIs.Rows[e.RowIndex].Selected)
                {
                    // Check if the CurrentCell is the cell the user right clicked on (but the row is not Selected) [CA-64954]
                    // This happens when the grid is initially shown: the current cell is the first cell in the first column, but the row is not selected
                    if (dataGridViewVDIs.CurrentCell == dataGridViewVDIs[e.ColumnIndex, e.RowIndex])
                        dataGridViewVDIs.Rows[e.RowIndex].Selected = true;
                    else
                        dataGridViewVDIs.CurrentCell = dataGridViewVDIs[e.ColumnIndex, e.RowIndex];
                }
            }
        }

        public class VDIRow : DataGridViewRow
        {
            public VDI VDI { get; private set; }

            private bool showStorageLink = false;

            public VDIRow(VDI vdi, bool showStorageLink)
            {
                this.showStorageLink = showStorageLink;
                VDI = vdi;
                for (int i = 0; i < 5; i++)
                {
                    Cells.Add(new DataGridViewTextBoxCell());
                    Cells[i].Value = GetCellText(i);
                }
            }

            private string GetCellText(int cellIndex)
            {
                switch (cellIndex)
                {
                    case 0:
                        return VDI.Name;
                    case 1:
                        if (Helpers.BostonOrGreater(VDI.Connection))
                        {
                            string name;
                            if (VDI.sm_config.TryGetValue("displayname", out name))
                            {
                                return name;
                            }
                            return string.Empty;
                        }
                        StorageLinkVolume volume = showStorageLink ? VDI.StorageLinkVolume(Program.StorageLinkConnections.GetCopy()) : null;
                        return volume == null ? string.Empty : volume.Name;
                    case 2:
                        return VDI.Description;
                    case 3:
                        return VDI.SizeText;
                    case 4:
                        return VDI.VMsOfVDI;
                    default:
                        return "";
                }
            }

            public void RefreshRowDetails(bool showSL)
            {
                showStorageLink = showSL;
                for (int i = 0; i < 5; i++)
                {
                    Cells[i].Value = GetCellText(i);
                }
            }
        }

        public struct VDIsData
        {
            public List<VDIRow> VdiRowsToUpdate { get; private set; }
            public List<VDIRow> VdiRowsToRemove { get; private set; }
            public List<VDI> VdisToAdd { get; private set; }
            public bool ShowStorageLink { get; private set; }

            public VDIsData(List<VDIRow> vdiRowsToUpdate, List<VDIRow> vdiRowsToRemove, List<VDI> vdisToAdd,
                bool showStorageLink) : this()
            {
                VdiRowsToUpdate = vdiRowsToUpdate;
                VdiRowsToRemove = vdiRowsToRemove;
                VdisToAdd = vdisToAdd;
                ShowStorageLink = showStorageLink;
            }
        }

        public class RefreshGridRequest
        {
            public SR SR { get; private set; }
            public bool Reset { get; private set; }

            public RefreshGridRequest(SR sr, bool reset)
            {
                SR = sr;
                Reset = reset;
            }
        }

        public class VDIsDataGridViewBuilder
        {
            private readonly Control owner;
            private readonly object _locker = new object();
            private BackgroundWorker worker;
            private Queue<RefreshGridRequest> queue = new Queue<RefreshGridRequest>();

            public VDIsDataGridViewBuilder(Control owner)
            {
                this.owner = owner;
                worker = new BackgroundWorker {WorkerSupportsCancellation = true};
                worker.DoWork += DoWork;
                worker.RunWorkerCompleted += (RunWorkerCompleted);
            }

            private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                if (e.Cancelled || e.Error != null)
                    return;

                lock (_locker)
                {
                    if (queue.Count > 0 && !worker.IsBusy)
                        worker.RunWorkerAsync();
                }
            }

            private VDIsData GetCurrentData(SR sr, IEnumerable<VDIRow> currentVDIRows)
            {
                List<VDI> vdis =
                        sr.Connection.ResolveAll(sr.VDIs).Where(
                            vdi =>
                            vdi.Show(Properties.Settings.Default.ShowHiddenVMs &&
                                     !vdi.IsAnIntermediateStorageMotionSnapshot))
                            .ToList();

                bool showStorageLink = vdis.Find(v => v.sm_config.ContainsKey("SVID")) != null;

                var vdiRowsToRemove =
                    currentVDIRows.Where(vdiRow => !vdis.Contains(vdiRow.VDI)).OrderByDescending(row => row.Index).ToList();

                var vdiRowsToUpdate = currentVDIRows.Except(vdiRowsToRemove).ToList();

                var vdisToAdd = vdis.Except(vdiRowsToUpdate.ConvertAll(vdiRow => vdiRow.VDI)).ToList();

                return new VDIsData(vdiRowsToUpdate, vdiRowsToRemove, vdisToAdd, showStorageLink);
            }

            private void DoWork(object sender, DoWorkEventArgs e)
            {
                RefreshGridRequest refreshRequest;
                lock (_locker)
                {
                    refreshRequest = queue.Count > 0 ? queue.Dequeue() : null;
                }

                if (worker.CancellationPending)
                    return;

                if (refreshRequest == null || refreshRequest.SR == null)
                    return;

                SrStoragePage page = owner as SrStoragePage;
                if (page == null)
                    return;

                if (refreshRequest.Reset)
                    Program.Invoke(owner, page.dataGridViewVDIs.Rows.Clear);

                Program.Invoke(owner, page.RefreshButtons);

                IEnumerable<VDIRow> currentVDIRows = Enumerable.Empty<VDIRow>();
                Program.Invoke(owner, () => currentVDIRows = page.GetCurrentVDIRows());
                VDIsData data = GetCurrentData(refreshRequest.SR, currentVDIRows);

                if (worker.CancellationPending)
                    return;

                Program.Invoke(owner, () => ((SrStoragePage)owner).RefreshDataGridView(data));
            }

            public void AddRequest(RefreshGridRequest refreshGridRequest)
            {
                lock (_locker)
                {
                    if (refreshGridRequest.Reset)
                        queue.Clear();

                    queue.Enqueue(refreshGridRequest);

                    if (!worker.IsBusy)
                        worker.RunWorkerAsync();
                }
            }

            public void Stop()
            {
                if (!worker.CancellationPending)
                    worker.CancelAsync();
            }
        }
    }
}
