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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Actions.GUIActions;
using XenAdmin.Actions.Updates;
using XenAdmin.Core;
using XenAdmin.Dialogs.OptionsPages;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib;


namespace XenAdmin.Dialogs.ServerUpdates
{
    public partial class ConfigCdnUpdatesPage : UserControl, IOptionsPage
    {
        private Control _invalidControl;
        private string _origPassword;

        public ConfigCdnUpdatesPage()
        {
            InitializeComponent();
            labelNoConnections.Text = string.Format(labelNoConnections.Text, BrandManager.ProductVersionPost82);
            toolTip1.ToolTipTitle = Messages.INVALID_PARAMETER;
            buttonApply.Enabled = buttonDiscard.Enabled = false;

            comboBoxRepo.Items.Add(new RepoComboItem(RepoDescriptor.NormalRepo));
            comboBoxRepo.Items.Add(new RepoComboItem(RepoDescriptor.EarlyAccessRepo));

            if (Registry.ShowExtraYumRepos)
            {
                comboBoxRepo.Items.Add(new RepoComboItem(RepoDescriptor.DevTeamRepo));
                comboBoxRepo.Items.Add(new RepoComboItem(RepoDescriptor.InternalRepo));
            }
        }

        private List<IXenConnection> SelectedConnections => dataGridViewExPools.Rows
            .Cast<CheckableConnectionRow>().Where(r => r.IsChecked).Select(r => r.Connection).ToList();

        #region IVerticalTab Members

        public override string Text => string.Format(Messages.CONFIG_CDN_UPDATES_TAB_TITLE, BrandManager.ProductBrand, BrandManager.ProductVersionPost82);
        public string SubText { get; }
        public Image Image { get; }

        #endregion

        private void ToggleConfigPanelEditState()
        {
            bool active = dataGridViewExPools.Rows.Cast<CheckableConnectionRow>().Any(r => r.IsChecked);

            buttonApply.Enabled = buttonDiscard.Enabled = active;
            comboBoxRepo.Enabled = active;

            textBoxProxyUrl.ReadOnly = textBoxProxyUsername.ReadOnly = textBoxProxyPassword.ReadOnly = !active;
            checkBoxPeriodicSync.Enabled = radioButtonDaily.Enabled = radioButtonWeekly.Enabled = comboBoxWeekday.Enabled = active;

            if (!active)
                UpdateConfigPanel();
        }

        private void UpdateConfigPanel()
        {
            var conn = dataGridViewExPools.SelectedRows.Cast<CheckableConnectionRow>().FirstOrDefault()?.Connection;
            var pool = Helpers.GetPoolOfOne(conn);
            if (pool == null)
                return;

            labelPool.Text = pool.Connection.Name.Ellipsise(60);

            //yum repo

            object foundItem = null;

            foreach (var repoRef in pool.repositories)
            {
                var repo = pool.Connection.Resolve(repoRef);
                if (repo == null)
                    continue;

                foreach (var item in comboBoxRepo.Items)
                {
                    var repoDescriptor = (item as RepoComboItem)?.Repo;
                    if (repoDescriptor != null && repoDescriptor.MatchesRepository(repo))
                    {
                        foundItem = item;
                        break;
                    }
                }
            }

            comboBoxRepo.SelectedItem = foundItem;

            //proxy

            textBoxProxyUrl.Text = pool.repository_proxy_url;
            textBoxProxyUsername.Text = pool.repository_proxy_username;
            _origPassword = textBoxProxyPassword.Text = pool.repository_proxy_password.opaque_ref == "OpaqueRef:NULL" ? string.Empty : RepositoryProxyAction.DUMMY_PASSWORD;

            //schedule

            groupBoxSchedule.Visible = Helpers.XapiEqualOrGreater_23_18_0(conn);

            checkBoxPeriodicSync.Checked = pool.update_sync_enabled;

            if (pool.update_sync_frequency == update_sync_frequency.daily)
                radioButtonDaily.Checked = true;
            else
                radioButtonWeekly.Checked = true;

            comboBoxWeekday.SelectedIndex = (int)pool.update_sync_day;
        }

        #region IOptionsPage Members

        public void Build()
        {
            var connections = ConnectionsManager.XenConnectionsCopy;

            if (!connections.Any(c => c.IsConnected && Helpers.CloudOrGreater(c)))
            {
                labelNoConnections.Visible = true;
                tableLayoutPanel5.Visible = false;
                return;
            }

            labelNoConnections.Visible = false;
            tableLayoutPanel5.Visible = true;

            CheckableConnectionRow selectedRow = null;
            if (dataGridViewExPools.SelectedRows.Count > 0)
                selectedRow = dataGridViewExPools.SelectedRows[0] as CheckableConnectionRow;

            var rows = connections
                .Where(c => c.IsConnected && Helpers.CloudOrGreater(c)).OrderBy(c => c.Name)
                .Select(c => new CheckableConnectionRow(c)).Cast<DataGridViewRow>().ToArray();

            //unregister and re-register afterwards to prevent multiple event triggering
            //which means multiple calls to repopulate the overview panel
            dataGridViewExPools.SelectionChanged -= dataGridViewExPools_SelectionChanged;
            try
            {
                dataGridViewExPools.Rows.Clear();
                dataGridViewExPools.Rows.AddRange(rows);

                if (selectedRow != null)
                {
                    foreach (DataGridViewRow row in dataGridViewExPools.Rows)
                    {
                        if (row is CheckableConnectionRow ccRow && ccRow.Connection == selectedRow.Connection)
                        {
                            row.Selected = true;
                            break;
                        }
                    }
                }
            }
            finally
            {
                dataGridViewExPools.SelectionChanged += dataGridViewExPools_SelectionChanged;
            }

            ToggleConfigPanelEditState();
        }

        public bool IsValidToSave(out Control control, out string invalidReason)
        {
            if (!string.IsNullOrWhiteSpace(textBoxProxyUrl.Text) &&
                !Uri.TryCreate(textBoxProxyUrl.Text, UriKind.Absolute, out _))
            {
                _invalidControl = control = textBoxProxyUrl;
                invalidReason = Messages.INVALID_PARAMETER;
                return false;
            }

            _invalidControl = control = null;
            invalidReason = string.Empty;
            return true;
        }

        public void ShowValidationMessages(Control control, string message)
        {
            HelpersGUI.ShowBalloonMessage(control, toolTip1);
        }

        public void HideValidationMessages()
        {
            if (_invalidControl != null)
                toolTip1.Hide(_invalidControl);
        }

        public void Save()
        {
            var connections = SelectedConnections;

            var changedRepoPools = new List<Pool>();

            foreach (var conn in connections)
            {
                var pool = Helpers.GetPoolOfOne(conn);
                if (pool == null)
                    continue;

                var actions = new List<AsyncAction>();

                //yum repo

                var selectedRepo = (comboBoxRepo.SelectedItem as RepoComboItem)?.Repo;
                if (selectedRepo != null)
                {
                    if (pool.repositories.Count == 0 || pool.repositories
                             .Select(repoRef => pool.Connection.Resolve(repoRef))
                             .Any(repo => repo != null && !selectedRepo.MatchesRepository(repo)))
                    {
                        actions.Add(new ConfigYumRepoAction(conn, selectedRepo));
                    }
                }

                //proxy

                var proxyUrl = textBoxProxyUrl.Text;
                var username = textBoxProxyUsername.Text;
                var password = textBoxProxyPassword.Text;

                if (pool.repository_proxy_url != proxyUrl || pool.repository_proxy_username != username ||
                    _origPassword != password)
                {
                    actions.Add(new RepositoryProxyAction(conn, proxyUrl, username, password));
                }

                //schedule

                if (Helpers.XapiEqualOrGreater_23_18_0(conn))
                {
                    var syncDay = comboBoxWeekday.SelectedIndex;
                    var frequency = radioButtonDaily.Checked ? update_sync_frequency.daily : update_sync_frequency.weekly;

                    if (pool.update_sync_frequency != frequency || pool.update_sync_day != syncDay)
                        actions.Add(new ConfigCdnSyncAction(conn, frequency, syncDay));

                    var periodicCheck = checkBoxPeriodicSync.Checked;

                    if (pool.update_sync_enabled != periodicCheck)
                        actions.Add(new ToggleCdnSyncAction(conn, checkBoxPeriodicSync.Checked));
                }

                var action = new MultipleAction(conn,
                    string.Format(Messages.ACTION_SAVING_SETTINGS_FOR, conn.Name),
                    string.Empty, string.Empty, actions, true, true);
                
                using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                    dialog.ShowDialog(this);

                if (action.SubActions.Any(a => a is ConfigYumRepoAction && a.Succeeded))
                    changedRepoPools.Add(pool);
            }

            if (changedRepoPools.Count > 0)
            {
                using (var dlog = new NoIconDialog(string.Format(Messages.YUM_REPO_SYNC_AFTER_CONFIG, BrandManager.BrandConsole),
                           new ThreeButtonDialog.TBDButton(Messages.YUM_REPO_SYNC_YES_BUTTON, DialogResult.Yes, ThreeButtonDialog.ButtonType.ACCEPT, true),
                           ThreeButtonDialog.ButtonNo))
                {
                    if (dlog.ShowDialog(this) == DialogResult.Yes)
                        changedRepoPools.ForEach(p => new SyncWithCdnAction(p).RunAsync());
                }
            }
        }

        #endregion

        private void dataGridViewExPools_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != ColumnCheck.Index ||
                e.RowIndex < 0 || dataGridViewExPools.RowCount <= e.RowIndex ||
                !(dataGridViewExPools.Rows[e.RowIndex] is CheckableConnectionRow row))
                return;

            row.IsChecked = !row.IsChecked;

            ToggleConfigPanelEditState();
        }

        private void dataGridViewExPools_SelectionChanged(object sender, EventArgs e)
        {
            UpdateConfigPanel();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (!IsValidToSave(out var control, out var invalidReason))
            {
                ShowValidationMessages(control, invalidReason);
                return;
            }

            buttonApply.Enabled = buttonDiscard.Enabled = false;
            Save();
            Build();
        }

        private void buttonDiscard_Click(object sender, EventArgs e)
        {
            buttonApply.Enabled = buttonDiscard.Enabled = false;
            Build();
        }

        private class CheckableConnectionRow : DataGridViewRow
        {
            private readonly DataGridViewCheckBoxCell _checkCell = new DataGridViewCheckBoxCell { ThreeState = false };
            private readonly DataGridViewTextBoxCell _nameCell = new DataGridViewTextBoxCell();

            public CheckableConnectionRow(IXenConnection connection)
            {
                Connection = connection;

                _nameCell.Value = connection.Name;
                _checkCell.Value = false;
                Cells.AddRange(_checkCell, _nameCell);
            }

            public IXenConnection Connection { get; }

            public bool IsChecked
            {
                get => (bool)_checkCell.Value;
                set => _checkCell.Value = value;
            }
        }

        private class RepoComboItem : ToStringWrapper<RepoDescriptor>
        {
            public RepoComboItem(RepoDescriptor descriptor)
                : base(descriptor, descriptor.FriendlyName)
            {
                Repo = descriptor;
            }

            public RepoDescriptor Repo { get; }
        }
    }
}
