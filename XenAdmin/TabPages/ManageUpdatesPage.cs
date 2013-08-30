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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Alerts;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Wizards.PatchingWizard;
using XenAPI;
using Timer = System.Windows.Forms.Timer;
using XenAdmin.Actions;

namespace XenAdmin.TabPages
{
    public partial class ManageUpdatesPage : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ManualCheckForUpdates availableUpdates = new ManualCheckForUpdates();
        private readonly List<Alert> updateAlerts = new List<Alert>();

        //Maintain a list of all the objects we currently have events on for clearing out on rebuild
        private List<IXenConnection> connectionsWithEvents = new List<IXenConnection>();
        private List<Pool> poolsWithEvents = new List<Pool>();
        private List<Host> hostsWithEvents = new List<Host>();

        private Timer spinningTimer = new Timer();
        private ImageList imageList = new ImageList();
        private bool checkForUpdatesSucceeded;

        Dictionary<string, bool> expandedState = new Dictionary<string, bool>();

        private void CheckForUpdates_CheckForUpdatesCompleted(object sender, CheckForUpdatesCompletedEventArgs e)
        {
            Program.Invoke(this, delegate
                                     {
                                         refreshButton.Enabled = true;
                                         ShowProgress(false);
                                         checkForUpdatesSucceeded = e.Succeeded;
                                         if (checkForUpdatesSucceeded)
                                             Rebuild();
                                         else
                                         {
                                             pictureBox.Image = SystemIcons.Error.ToBitmap();
                                             UpdateLabels(false, e.ErrorMessage);
                                             UpdateDownloadAndInstallButton(false);
                                         }
                                     });
        }

        public ManageUpdatesPage()
        {
            InitializeComponent();
            InitializeProgressControls();
            InformationHelperVisible = false;
            informationLabel.Click += informationLabel_Click;
            availableUpdates.CheckForUpdatesCompleted += CheckForUpdates_CheckForUpdatesCompleted;
        }

        public void RefreshUpdateList()
        {
            CheckForUpdates();
        }

        public void CancelUpdateCheck()
        {
            if (spinningTimer.Enabled)
                ShowProgress(false);
        }

        private void informationLabel_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(InvisibleMessages.UPSELL_SA);
            }
            catch (Exception)
            {
                new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       string.Format(Messages.LICENSE_SERVER_COULD_NOT_OPEN_LINK, InvisibleMessages.LICENSE_SERVER_DOWNLOAD_LINK),
                       Messages.XENCENTER)).ShowDialog(this);
            }
        }

        private void connection_CachePopulated(object sender, EventArgs e)
        {
            Program.Invoke(this, Rebuild);
        }

        private void RegisterEvents()
        {
            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
            {
                c.CachePopulated += connection_CachePopulated;
                connectionsWithEvents.Add(c);
                foreach (var pool in c.Cache.Pools)
                {
                    RegisterPoolEvents(pool);
                    poolsWithEvents.Add(pool);
                }
                foreach (Host host in c.Cache.Hosts)
                {
                    RegisterHostEvents(host);
                    hostsWithEvents.Add(host);
                }
            }
        }

        private void DeregisterEvents()
        {
            foreach (IXenConnection c in connectionsWithEvents)
            {
                c.CachePopulated -= connection_CachePopulated;
            }
            foreach (var pool in poolsWithEvents)
            {
                DeregisterPoolEvents(pool);
            }
            foreach (Host h in hostsWithEvents)
            {
                DeregisterHostEvents(h);
            }
            connectionsWithEvents.Clear();
            hostsWithEvents.Clear();
            poolsWithEvents.Clear();
        }
        
        private void RegisterHostEvents(Host host)
        {
            host.PropertyChanged += host_PropertyChanged;
        }

        private void DeregisterHostEvents(Host host)
        {
            host.PropertyChanged -= host_PropertyChanged;
        }

        private void RegisterPoolEvents(Pool pool)
        {
            pool.PropertyChanged += pool_PropertyChanged;
        }

        private void DeregisterPoolEvents(Pool pool)
        {
            pool.PropertyChanged -= pool_PropertyChanged;
        }

        private void host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "metrics")
                Program.Invoke(this, Rebuild);
        }

        private void pool_PropertyChanged(object obj, PropertyChangedEventArgs e)
        {
            Pool pool = (Pool)obj;
            if (e.PropertyName == "other_config" || e.PropertyName == "name_label")
                Program.Invoke(this, Rebuild);
        }

        private void RefreshAlertsList()
        {
            updateAlerts.Clear();
            foreach (var updateAlert in availableUpdates.UpdateAlerts)
            {
                updateAlerts.Add(updateAlert);
            }
        }

        private void UpdateLabels(bool updatesFound)
        {
            UpdateLabels(updatesFound, string.Empty);
        }

        private void UpdateLabels(bool updatesFound, string errorMessage)
        {
            tableLayoutPanel.SuspendLayout();
            try
            {
                if (updatesFound)
                {
                    labelUpdates.Text = Messages.AVAILABLE_UPDATES_FOUND;
                }
                else
                {
                    labelUpdates.Text = errorMessage == string.Empty ? Messages.AVAILABLE_UPDATES_NOT_FOUND : errorMessage;
                }
            }
            finally
            {
                tableLayoutPanel.ResumeLayout();
            }
        }

        private void RefreshEventHandlers()
        {
            DeregisterEvents();
            RegisterEvents();
        }

        private void Rebuild()
        {
            Program.AssertOnEventThread();

            if (!checkForUpdatesSucceeded)
                return;

            RefreshAlertsList();
            UpdateLabels(updateAlerts.Count > 0);
            RefreshEventHandlers();
            PopulateGrid();
        }

        private void PopulateGrid()
        {
            dataGridViewUpdates.SuspendLayout();
            try
            {
                DataGridViewColumn sortedColumn = dataGridViewUpdates.SortedColumn;
                ListSortDirection sortDirection = (dataGridViewUpdates.SortOrder == SortOrder.Descending)
                                                      ? ListSortDirection.Descending
                                                      : ListSortDirection.Ascending;

                Alert selectedAlert = null;
                if (dataGridViewUpdates.SelectedRows.Count > 0)
                    selectedAlert = (Alert)dataGridViewUpdates.SelectedRows[0].Tag;

                dataGridViewUpdates.Rows.Clear();

                if (updateAlerts.Count == 0)
                {
                    UpdateDownloadAndInstallButton(false);
                    return;
                }

                foreach (var myAlert in updateAlerts)
                {
                    dataGridViewUpdates.Rows.Add(NewUpdateRow(myAlert));
                }

                if (sortedColumn == null)
                    dataGridViewUpdates.Sort(ColumnDate, ListSortDirection.Descending);
                else
                    dataGridViewUpdates.Sort(sortedColumn, sortDirection);

                //restore selection
                bool selectionRestored = false;
                if (selectedAlert != null)
                {
                    foreach (DataGridViewRow row in dataGridViewUpdates.Rows)
                    {
                        if (selectedAlert.Equals((Alert) row.Tag))
                        {
                            row.Cells[ColumnWebPage.Index].Selected = true;
                            selectionRestored = true;
                            break;
                        }
                    }
                }

                //select first row if no selection
                if (selectedAlert == null || !selectionRestored)
                {
                    if (dataGridViewUpdates.Rows.Count > 0) dataGridViewUpdates.Rows[0].Cells[ColumnWebPage.Index].Selected = true;
                }
            }
            finally
            {
                dataGridViewUpdates.ResumeLayout();
            }

            UpdateDownloadAndInstallButton(true);
        }

        private void UpdateDownloadAndInstallButton(bool canEnable)
        {
            if (canEnable && (dataGridViewUpdates.SelectedRows.Count > 0))
            {
                XenServerPatchAlert alert = dataGridViewUpdates.SelectedRows[0].Tag as XenServerPatchAlert;
                if (alert != null)
                {
                    if(!alert.CanApply)
                    {
                        ShowInformationHelper = alert.CannotApplyReason;
                        downloadAndInstallButton.Enabled = false;
                        return;
                    }

                    ShowInformationHelper = alert.CannotApplyReason;
                    downloadAndInstallButton.Enabled = !string.IsNullOrEmpty(alert.Patch.PatchUrl);
                    return;
                }
            }
            
            downloadAndInstallButton.Enabled = false;
        }

        private string ShowInformationHelper
        {
            set 
            { 
                if(String.IsNullOrEmpty(value))
                {
                    InformationHelperVisible = false;
                    informationLabel.Text = value;
                }
                else
                {
                    InformationHelperVisible = true;
                    informationLabel.Text = value;
                }
            }
        }

        private bool InformationHelperVisible
        {
            set
            {
                informationLabel.Visible = value;
                informationLabelIcon.Visible = value;
            }
        }

        private DataGridViewRow NewUpdateRow(Alert alert)
        {
            DataGridViewImageCell expanderCell = new DataGridViewImageCell();
            DataGridViewTextBoxCell messageCell = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell appliesToCell = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell dateCell = new DataGridViewTextBoxCell();
            DataGridViewLinkCell webPageCell = new DataGridViewLinkCell();
           
            DataGridViewRow newRow = new DataGridViewRow();
            newRow.Tag = alert;

            // Set the detail cell content and expanding arrow
            if (expandedState.ContainsKey(alert.uuid))
            {
                // show the expanded arrow and the body detail
                expanderCell.Value = Properties.Resources.expanded_triangle;
                messageCell.Value = String.Format("{0}\n\n{1}", alert.Title, alert.Description);
            }
            else
            {
                // show the expand arrow and just the title
                expanderCell.Value = Properties.Resources.contracted_triangle;
                messageCell.Value = alert.Title;
            }

            dateCell.Value = HelpersGUI.DateTimeToString(alert.Timestamp.ToLocalTime(), Messages.DATEFORMAT_DMY, true);
            webPageCell.Value = alert.WebPageLabel;
            appliesToCell.Value = alert.AppliesTo;

            newRow.Cells.AddRange(expanderCell, messageCell, appliesToCell, dateCell, webPageCell);
            return newRow;
        }

        private void dataGridViewUpdates_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            Alert alert1 = (Alert)dataGridViewUpdates.Rows[e.RowIndex1].Tag;
            Alert alert2 = (Alert)dataGridViewUpdates.Rows[e.RowIndex2].Tag;

            if (e.Column.Index == ColumnMessage.Index)
            {
                e.SortResult = Alert.CompareOnTitle(alert1, alert2);
                e.Handled = true;
            }
            else if (e.Column.Index == ColumnDate.Index)
            {
                e.SortResult = Alert.CompareOnDate(alert1, alert2);
                e.Handled = true;
            }
            else if (e.Column.Index == ColumnWebPage.Index)
            {
                e.SortResult = Alert.CompareOnWebPage(alert1, alert2);
                e.Handled = true;
            }
            else if (e.Column.Index == ColumnLocation.Index)
            {
                e.SortResult = Alert.CompareOnAppliesTo(alert1, alert2);
                e.Handled = true;
            }
        }

        private void InitializeProgressControls()
        {
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(32, 32);
            imageList.Images.Add(Properties.Resources.SpinningFrame0);
            imageList.Images.Add(Properties.Resources.SpinningFrame1);
            imageList.Images.Add(Properties.Resources.SpinningFrame2);
            imageList.Images.Add(Properties.Resources.SpinningFrame3);
            imageList.Images.Add(Properties.Resources.SpinningFrame4);
            imageList.Images.Add(Properties.Resources.SpinningFrame5);
            imageList.Images.Add(Properties.Resources.SpinningFrame6);
            imageList.Images.Add(Properties.Resources.SpinningFrame7);

            spinningTimer.Tick += new EventHandler(timer_Tick);
            spinningTimer.Interval = 150;
        }

        private int currentSpinningFrame = 0;

        private void timer_Tick(object sender, EventArgs e)
        {
            int imageIndex = ++currentSpinningFrame <= 7 ? currentSpinningFrame : currentSpinningFrame = 0;
            pictureBoxProgress.Image = imageList.Images[imageIndex];
        }       

        private void ShowProgress(bool show)
        {
            if (show)
                spinningTimer.Start();
            else
                spinningTimer.Stop();
            panelProgress.Visible = show;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void CheckForUpdates()
        {
            checkForUpdatesSucceeded = false;
            ResetControls();
            ShowProgress(true);
            availableUpdates.RunCheck();
        }

        private void ResetControls()
        {
            pictureBox.Image = Properties.Resources._015_Download_h32bit_32;
            refreshButton.Enabled = false;
            labelUpdates.Text = Messages.AVAILABLE_UPDATES_SEARCHING;
            dataGridViewUpdates.Rows.Clear();
            UpdateDownloadAndInstallButton(false);
        }

        private void OpenGoToWebsiteLink()
        {
            if (dataGridViewUpdates.SelectedRows.Count > 0)
            {
                Alert alert = (Alert)dataGridViewUpdates.SelectedRows[0].Tag;
                if (alert.FixLinkAction != null)
                    alert.FixLinkAction.Invoke();
            }
        }

        private void DownloadAndInstall()
        {
            if (dataGridViewUpdates.SelectedRows.Count == 0)
                return;

            XenServerPatchAlert patchAlert = dataGridViewUpdates.SelectedRows[0].Tag as XenServerPatchAlert;
            if (patchAlert == null)
                return;

            string patchUri = patchAlert.Patch.PatchUrl;
            if (string.IsNullOrEmpty(patchUri))
                return;

            Uri address = new Uri(patchUri);
            string tempFile = Path.GetTempFileName();

            var action = new DownloadAndUnzipXenServerPatchAction(patchAlert.Description, address, tempFile);
            ActionProgressDialog dialog = new ActionProgressDialog(action, ProgressBarStyle.Continuous, false) { ShowCancel = true };
            dialog.ShowDialog(this);

            if (action.Succeeded)
            {
                var wizard = new PatchingWizard();
                wizard.Show();
                wizard.NextStep();
                wizard.AddFile(action.PatchPath);
                wizard.NextStep();
                if (patchAlert.Hosts.Count > 0)
                {
                    wizard.SelectServers(patchAlert.Hosts);
                    if (wizard.CurrentStepTabPage.EnableNext())
                        wizard.NextStep();
                }
                else
                {
                    string disconnectedServerNames =
                        dataGridViewUpdates.SelectedRows[0].Cells[ColumnLocation.Index].Value.ToString();

                    new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Warning,
                                                      string.Format(Messages.UPDATES_WIZARD_DISCONNECTED_SERVER,
                                                                    disconnectedServerNames),
                                                      Messages.UPDATES_WIZARD)).ShowDialog(this);
                }
            }
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            bool refreshVisible = refreshButton.Enabled;
            bool downloadAndInstallVisible = false;

            Point pt = dataGridViewUpdates.PointToClient(new Point(contextMenuStrip.Left, contextMenuStrip.Top));
            DataGridView.HitTestInfo info = dataGridViewUpdates.HitTest(pt.X, pt.Y);

            if (info != null && info.RowIndex >= 0 && info.RowIndex < dataGridViewUpdates.Rows.Count)
            {
                DataGridViewRow row = dataGridViewUpdates.Rows[info.RowIndex];
                if (row != null)
                {
                    row.Selected = true;
                    downloadAndInstallVisible = downloadAndInstallButton.Enabled;
                }
            }

            if (!refreshVisible && !downloadAndInstallVisible)
            {
                e.Cancel = true;
                return;
            }

            refreshToolStripMenuItem.Visible = refreshVisible;
            separatorToolStripMenuItem.Visible = downloadAndInstallVisible;
            downloadAndInstallToolStripMenuItem.Visible = downloadAndInstallVisible;
        }

        private void downloadAndInstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadAndInstall();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void dataGridViewUpdates_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDownloadAndInstallButton(true);
        }

        private void downloadAndInstallButton_Click(object sender, EventArgs e)
        {
            DownloadAndInstall();
        }

        private void dataGridViewUpdates_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == ColumnWebPage.Index)
                OpenGoToWebsiteLink();
        }

        private void dataGridViewUpdates_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.ColumnIndex != ColumnExpand.Index)
                return;

            toggleExpandedState(e.RowIndex);
        }

        private void dataGridViewUpdates_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;
            toggleExpandedState(e.RowIndex);
        }

        private void dataGridViewUpdates_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right) // expand all selected rows
            {
                foreach (DataGridViewBand row in dataGridViewUpdates.SelectedRows)
                {
                    Alert alert = (Alert)dataGridViewUpdates.Rows[row.Index].Tag;
                    if (!expandedState.ContainsKey(alert.uuid))
                    {
                        toggleExpandedState(row.Index);
                    }
                }
            }
            else if (e.KeyCode == Keys.Left) // collapse all selected rows
            {
                foreach (DataGridViewBand row in dataGridViewUpdates.SelectedRows)
                {
                    Alert alert = (Alert)dataGridViewUpdates.Rows[row.Index].Tag;
                    if (expandedState.ContainsKey(alert.uuid))
                    {
                        toggleExpandedState(row.Index);
                    }
                }
            }
            else if (e.KeyCode == Keys.Enter) // toggle expanded state for all selected rows
            {
                foreach (DataGridViewBand row in dataGridViewUpdates.SelectedRows)
                {
                    toggleExpandedState(row.Index);
                }
            }
        }

        /// <summary>
        /// Toggles the row specified between the expanded and contracted state
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="RowIndex"></param>
        private void toggleExpandedState(int RowIndex)
        {
            Alert alert = (Alert)dataGridViewUpdates.Rows[RowIndex].Tag;

            if (expandedState.ContainsKey(alert.uuid))
            {
                expandedState.Remove(alert.uuid);
                dataGridViewUpdates.Rows[RowIndex].Cells[ColumnMessage.Index].Value = alert.Title;
                dataGridViewUpdates.Rows[RowIndex].Cells[ColumnExpand.Index].Value = Properties.Resources.contracted_triangle;
            }
            else
            {
                expandedState.Add(alert.uuid, true);
                dataGridViewUpdates.Rows[RowIndex].Cells[ColumnMessage.Index].Value
                    = String.Format("{0}\n\n{1}", alert.Title, alert.Description);
                dataGridViewUpdates.Rows[RowIndex].Cells[ColumnExpand.Index].Value = Properties.Resources.expanded_triangle;
            }
        }
    }
}
