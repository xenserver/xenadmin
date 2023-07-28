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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.TabPages.CdnUpdates;
using XenAdmin.Wizards.PatchingWizard;
using XenAPI;


namespace XenAdmin.TabPages
{
    public partial class ManageCdnUpdatesPage : NotificationsBasePage
    {
        private int checksQueue;
        private int _rowInsertionIndex;

        private volatile bool _buildInProgress;
        private volatile bool _buildRequired;

        public ManageCdnUpdatesPage()
        {
            InitializeComponent();
            tsSplitButtonSynchronize.DefaultItem = tsmiSynchronizeSelected;
            tsSplitButtonSynchronize.Text = tsmiSynchronizeSelected.Text;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            try
            {
                //ensure we won't try to rebuild the list while setting the initial view
                checksQueue++;
                Rebuild();
            }
            finally
            {
                checksQueue--;
            }

            UpdateButtonEnablement();
        }

        #region NotificationPage overrides

        protected override void RefreshPage()
        {
            toolStripDropDownButtonServerFilter.InitializeHostList(Helpers.CloudOrGreater);
            Rebuild(); 
        }

        protected override void RegisterEventHandlers()
        {
            Updates.CdnUpdateInfoChanged += Cdn_UpdateInfoChanged;
        }

        protected override void DeregisterEventHandlers()
        {
            Updates.CdnUpdateInfoChanged -= Cdn_UpdateInfoChanged;
        }

        public override string HelpID => "ManageUpdatesDialog";

        public override NotificationsSubMode NotificationsSubMode => NotificationsSubMode.UpdatesFromCdn;

        #endregion

        private void Cdn_UpdateInfoChanged(IXenConnection obj)
        {
            Rebuild();
        }

        public override bool FilterIsOn => toolStripDropDownButtonServerFilter.FilterIsOn;

        private void Rebuild()
        {
            if (_buildInProgress)
            {
                _buildRequired = true;
                return;
            }

            Program.Invoke(Program.MainWindow, () =>
            {
                if (!Visible || checksQueue > 0)
                    return;

                try
                {
                    _buildInProgress = true;

                    OnFiltersChanged();
                    BuildList();
                }
                finally
                {
                    _buildInProgress = false;

                    if (_buildRequired)
                    {
                        _buildRequired = false;
                        Rebuild();
                    }
                }
            });
        }

        private void BuildList()
        {
            dataGridViewEx1.SuspendLayout();

            try
            {
                dataGridViewEx1.Rows.Clear();
                var connections = ConnectionsManager.XenConnectionsCopy
                    .Where(c => c.IsConnected && Helpers.CloudOrGreater(c)).ToList();
                connections.Sort();

                foreach (var connection in connections)
                {
                    Updates.CdnUpdateInfoPerConnection.TryGetValue(connection, out var poolUpdateInfo);

                    CdnExpandableRow row = null;

                    if (Helpers.GetPool(connection) == null)
                    {
                        if (poolUpdateInfo == null)
                        {
                            var host = connection.Cache.Hosts.FirstOrDefault();

                            if (host != null && !toolStripDropDownButtonServerFilter.HideByLocation(host.uuid))
                                row = new HostUpdateInfoRow(connection, host, null, null);
                        }
                        else
                        {
                            var hostUpdateInfo = poolUpdateInfo.HostsWithUpdates.FirstOrDefault();
                            var host = connection.Resolve(new XenRef<Host>(hostUpdateInfo?.HostOpaqueRef));

                            if (host != null && !toolStripDropDownButtonServerFilter.HideByLocation(host.uuid))
                                row = new HostUpdateInfoRow(connection, host, poolUpdateInfo, hostUpdateInfo);
                        }
                    }
                    else
                    {
                        var hostUuidList = connection.Cache.Hosts.Select(h => h.uuid).ToList();

                        if (!toolStripDropDownButtonServerFilter.HideByLocation(hostUuidList))
                            row = new PoolUpdateInfoRow(connection, poolUpdateInfo);
                    }

                    if (row != null)
                        dataGridViewEx1.Rows.Add(row);
                }
            }
            finally
            {
                dataGridViewEx1.ResumeLayout();
            }
        }

        private void UpdateButtonEnablement()
        {
            toolStripButtonExportAll.Enabled = dataGridViewEx1.RowCount > 0;

            Pool selectedPool = null;

            if (dataGridViewEx1.SelectedRows.Count > 0)
            {
                switch (dataGridViewEx1.SelectedRows[0])
                {
                    case PoolUpdateInfoRow poolRow:
                        selectedPool = poolRow.Pool;
                        break;
                    case HostUpdateInfoRow hostRow when Helpers.GetPool(hostRow.Connection) == null:
                        selectedPool = Helpers.GetPoolOfOne(hostRow.Connection);
                        break;
                }
            }

            tsmiSynchronizeSelected.Enabled = selectedPool != null && selectedPool.allowed_operations.Contains(pool_allowed_operations.sync_updates);

            var allAllowed = true;

            foreach (DataGridViewRow row in dataGridViewEx1.Rows)
            {
                if (row is PoolUpdateInfoRow puiRow &&
                    !puiRow.Pool.allowed_operations.Contains(pool_allowed_operations.sync_updates))
                {
                    allAllowed = false;
                    break;
                }

                if (row is HostUpdateInfoRow huiRow && Helpers.GetPool(huiRow.Connection) == null)
                {
                    var pool = Helpers.GetPoolOfOne(huiRow.Connection);
                    if (pool != null && !pool.allowed_operations.Contains(pool_allowed_operations.sync_updates))
                    {
                        allAllowed = false;
                        break;
                    }
                }
            }

            tsmiSynchronizeAll.Enabled = allAllowed;

            tsSplitButtonSynchronize.Enabled = tsmiSynchronizeSelected.Enabled || tsmiSynchronizeAll.Enabled;

            if (tsSplitButtonSynchronize.DefaultItem != null && !tsSplitButtonSynchronize.DefaultItem.Enabled)
            {
                foreach (ToolStripItem item in tsSplitButtonSynchronize.DropDownItems)
                {
                    if (item.Enabled)
                    {
                        tsSplitButtonSynchronize.DefaultItem = item;
                        tsSplitButtonSynchronize.Text = item.Text;
                        break;
                    }
                }
            }
        }

        private void ToggleRow(CdnExpandableRow row)
        {
            row.IsExpanded = !row.IsExpanded;

            dataGridViewEx1.SuspendLayout();

            try
            {
                if (row.IsExpanded)
                    ExpandRow(row);
                else
                    CollapseRow(row);
            }
            finally
            {
                dataGridViewEx1.ResumeLayout();
            }
        }

        private void ExpandRow(CdnExpandableRow row)
        {
            _rowInsertionIndex = row.Index;

            foreach (var childRow in row.ChildRows)
            {
                if (row is HostUpdateInfoRow huiRow && toolStripDropDownButtonServerFilter.HideByLocation(huiRow.Host.uuid))
                    continue;

                dataGridViewEx1.Rows.Insert(++_rowInsertionIndex, childRow);
                childRow.ParentRow = row;
                childRow.Level = row.Level + 1;

                if (childRow.IsExpanded)
                    ExpandRow(childRow);
            }
        }

        private void CollapseRow(CdnExpandableRow row)
        {
            foreach (var childRow in row.ChildRows)
            {
                dataGridViewEx1.Rows.Remove(childRow);
                childRow.ParentRow = null;
                childRow.Level = 0;

                if (childRow.IsExpanded)
                    CollapseRow(childRow);
            }
        }

        #region Toolstrip handlers

        private void toolStripDropDownButtonServerFilter_FilterChanged()
        {
            Rebuild();
        }

        private void tsSplitButtonSynchronize_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            tsSplitButtonSynchronize.DefaultItem = e.ClickedItem;
            tsSplitButtonSynchronize.Text = tsSplitButtonSynchronize.DefaultItem.Text;
        }

        private void tsmiSynchronizeSelected_Click(object sender, EventArgs e)
        {
            if (dataGridViewEx1.SelectedRows.Count > 0 &&
                dataGridViewEx1.SelectedRows[0] is PoolUpdateInfoRow row)
            {
                var syncAction = new SyncWithCdnAction(row.Pool);
                syncAction.Completed += a => Updates.CheckForCdnUpdates(a.Connection);
                syncAction.RunAsync();
            }
        }

        private void tsmiSynchronizeAll_Click(object sender, EventArgs e)
        {
            DialogResult result;

            if (FilterIsOn)
            {
                using (var dlog = new NoIconDialog(Messages.YUM_REPO_SYNC_FILTER_CONFIRMATION,
                           new ThreeButtonDialog.TBDButton(Messages.YUM_REPO_SYNC_YES_ALL_BUTTON, DialogResult.Yes),
                           new ThreeButtonDialog.TBDButton(Messages.YUM_REPO_SYNC_YES_VISIBLE_BUTTON, DialogResult.No, ThreeButtonDialog.ButtonType.NONE),
                           ThreeButtonDialog.ButtonCancel))
                {
                    result = dlog.ShowDialog(this);
                }
            }
            else
            {
                using (var dlog = new NoIconDialog(Messages.YUM_REPO_SYNC_ALL_CONFIRMATION,
                           new ThreeButtonDialog.TBDButton(Messages.YUM_REPO_SYNC_YES_ALL_BUTTON, DialogResult.Yes),
                           ThreeButtonDialog.ButtonCancel))
                {
                    result = dlog.ShowDialog(this);
                    Settings.TrySaveSettings();
                }
            }

            switch (result)
            {
                case DialogResult.Cancel:
                    return;
                case DialogResult.Yes:
                    var connections = ConnectionsManager.XenConnectionsCopy
                        .Where(c => c.IsConnected && Helpers.CloudOrGreater(c)).ToList();

                    foreach (var connection in connections)
                    {
                        var pool = Helpers.GetPoolOfOne(connection);
                        if (pool == null)
                            continue;

                        var syncAction = new SyncWithCdnAction(pool);
                        syncAction.Completed += a => Updates.CheckForCdnUpdates(a.Connection);
                        syncAction.RunAsync();
                    }

                    return;
                case DialogResult.No:
                    foreach (DataGridViewRow row in dataGridViewEx1.Rows)
                    {
                        if (row is PoolUpdateInfoRow puiRow)
                        {
                            var syncAction = new SyncWithCdnAction(puiRow.Pool);
                            syncAction.Completed += a => Updates.CheckForCdnUpdates(a.Connection);
                            syncAction.RunAsync();
                        }
                        else if (row is HostUpdateInfoRow huiRow)
                        {
                            if (Helpers.GetPool(huiRow.Connection) != null)
                                continue;

                            var pool = Helpers.GetPoolOfOne(huiRow.Connection);
                            if (pool == null)
                                continue;

                            var syncAction = new SyncWithCdnAction(pool);
                            syncAction.Completed += a => Updates.CheckForCdnUpdates(a.Connection);
                            syncAction.RunAsync();
                        }
                    }

                    return;
            }
        }

        private void toolStripButtonUpdate_Click(object sender, EventArgs e)
        {
            Program.MainWindow.ShowForm(typeof(PatchingWizard));
        }

        private void toolStripButtonExportAll_Click(object sender, EventArgs e)
        {
            bool exportAll = true;

            if (FilterIsOn)
            {
                using (var dlog = new NoIconDialog(Messages.UPDATE_EXPORT_ALL_OR_FILTERED,
                    new ThreeButtonDialog.TBDButton(Messages.EXPORT_ALL_BUTTON, DialogResult.Yes),
                    new ThreeButtonDialog.TBDButton(Messages.EXPORT_FILTERED_BUTTON, DialogResult.No, ThreeButtonDialog.ButtonType.NONE),
                    ThreeButtonDialog.ButtonCancel))
                {
                    var result = dlog.ShowDialog(this);
                    if (result == DialogResult.No)
                        exportAll = false;
                    else if (result == DialogResult.Cancel)
                        return;
                }
            }

            string fileName;
            using (SaveFileDialog dialog = new SaveFileDialog
            {
                AddExtension = true,
                Filter = string.Format("{0} (*.txt)|*.txt|{1} (*.*)|*.*", Messages.TXT_DESCRIPTION, Messages.ALL_FILES),
                FilterIndex = 0,
                Title = Messages.EXPORT_ALL,
                RestoreDirectory = true,
                DefaultExt = "csv",
                CheckPathExists = false,
                OverwritePrompt = true
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                fileName = dialog.FileName;
            }

            new DelegatedAsyncAction(null,
                string.Format(Messages.EXPORT_UPDATES, fileName),
                string.Format(Messages.EXPORTING_UPDATES, fileName),
                string.Format(Messages.EXPORTED_UPDATES, fileName),
                delegate
                {
                    using (StreamWriter stream = new StreamWriter(fileName, false, Encoding.UTF8))
                    {
                        if (exportAll)
                        {
                            foreach (var kvp in Updates.CdnUpdateInfoPerConnection)
                            {
                                var connection = kvp.Key;
                                var poolUpdateInfo = kvp.Value;

                                foreach (var hostUpdateInfo in poolUpdateInfo.HostsWithUpdates)
                                {
                                    stream.WriteLine(connection.Resolve(new XenRef<Host>(hostUpdateInfo.HostOpaqueRef))?.Name());
                                    stream.WriteLine(string.Join("\n", hostUpdateInfo.RecommendedGuidance.Select(g => g.StringOf())));

                                    var categories = hostUpdateInfo.GetUpdateCategories(poolUpdateInfo);
                                    foreach (var category in categories)
                                    {
                                        stream.WriteLine($"{category.Item1.GetCategoryTitle(category.Item2.Count)}");

                                        foreach (var update in category.Item2)
                                        {
                                            stream.WriteLine(update.Summary);
                                            stream.WriteLine(update.CollateDetails());
                                        }
                                    }

                                    stream.WriteLine(string.Join(", ", hostUpdateInfo.Rpms));
                                }
                            }
                        }
                        else
                        {
                            foreach (var row in dataGridViewEx1.Rows)
                            {
                                if (!(row is PoolUpdateInfoRow puiRow))
                                    continue;

                                foreach (var childRow in puiRow.ChildRows)
                                {
                                    if (!(childRow is HostUpdateInfoRow huiRow) ||
                                        toolStripDropDownButtonServerFilter.HideByLocation(huiRow.Host.uuid))
                                        continue;

                                    foreach (var export in huiRow.Export())
                                        stream.WriteLine(export);
                                }
                            }
                        }
                    }
                }).RunAsync();
        }

        #endregion

        #region DataGridView event handlers

        private void dataGridViewEx1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _columnName.Index &&
                e.RowIndex >= 0 &&
                e.RowIndex < dataGridViewEx1.RowCount &&
                dataGridViewEx1.Rows[e.RowIndex] is CdnExpandableRow row)
            {
                ToggleRow(row);
            }
        }

        private void dataGridViewEx1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Clicks == 1 &&
                e.ColumnIndex == _columnName.Index &&
                e.RowIndex >= 0 &&
                e.RowIndex < dataGridViewEx1.RowCount &&
                dataGridViewEx1.Rows[e.RowIndex] is CdnExpandableRow row &&
                row.Cells[e.ColumnIndex] is CdnExpandableTextAndImageCell cell &&
                cell.IsPointInExpander(e.Location))
            {
                ToggleRow(row);
            }
        }

        private void dataGridViewEx1_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridViewEx1.SelectedRows.Count > 0 &&
                dataGridViewEx1.SelectedRows[0] is CdnExpandableRow row)
            {
                switch (e.KeyCode)
                {
                    case Keys.Right when !row.IsExpanded:
                    case Keys.Left when row.IsExpanded:
                        break;
                    default:
                        return;
                }

                ToggleRow(row);
            }
        }

        private void dataGridViewEx1_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonEnablement();
        }

        #endregion
    }
}
