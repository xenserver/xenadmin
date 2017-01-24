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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Alerts;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.PatchingWizard;
using Timer = System.Windows.Forms.Timer;


namespace XenAdmin.TabPages
{
    public partial class ManageUpdatesPage : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private int currentSpinningFrame;
        private Timer spinningTimer = new Timer();
        private ImageList imageList = new ImageList();

        Dictionary<string, bool> expandedState = new Dictionary<string, bool>();
        private List<string> selectedUpdates = new List<string>();
        private int checksQueue;
        private bool PageWasRefreshed;
        private bool CheckForUpdatesInProgress;

        public ManageUpdatesPage()
        {
            InitializeComponent();
            InitializeProgressControls();
            tableLayoutPanel1.Visible = false;
            UpdateButtonEnablement();
            dataGridViewUpdates.Sort(ColumnDate, ListSortDirection.Descending);
            informationLabel.Click += informationLabel_Click;
            Updates.RegisterCollectionChanged(UpdatesCollectionChanged);
            Updates.RestoreDismissedUpdatesStarted += Updates_RestoreDismissedUpdatesStarted;
            Updates.CheckForUpdatesStarted += CheckForUpdates_CheckForUpdatesStarted;
            Updates.CheckForUpdatesCompleted += CheckForUpdates_CheckForUpdatesCompleted;
            pictureBox1.Image = SystemIcons.Information.ToBitmap();
            toolStripSplitButtonDismiss.DefaultItem = dismissAllToolStripMenuItem;
            toolStripSplitButtonDismiss.Text = dismissAllToolStripMenuItem.Text;
            PageWasRefreshed = false;
        }

        public void RefreshUpdateList()
        {
            toolStripDropDownButtonServerFilter.InitializeHostList();
            toolStripDropDownButtonServerFilter.BuildFilterList();
            Rebuild();
        }

        private void UpdatesCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(Program.MainWindow, Rebuild);
        }

        private void CheckForUpdates_CheckForUpdatesStarted()
        {
            Program.Invoke(Program.MainWindow, StartCheckForUpdates);
        }

        private void Updates_RestoreDismissedUpdatesStarted()
        {
            Program.Invoke(Program.MainWindow, StartCheckForUpdates);
        }

        private void StartCheckForUpdates()
        {
            if (CheckForUpdatesInProgress)
                return;

            CheckForUpdatesInProgress = true;

            checksQueue++;
            if (checksQueue > 1)
                return;

            toolStripButtonRefresh.Enabled = false;
            toolStripButtonRestoreDismissed.Enabled = false;

            StoreSelectedUpdates();
            dataGridViewUpdates.Rows.Clear();
            dataGridViewUpdates.Refresh();

            spinningTimer.Start();
            tableLayoutPanel3.Visible = true;
            labelProgress.Text = Messages.AVAILABLE_UPDATES_SEARCHING;
        }

        private void CheckForUpdates_CheckForUpdatesCompleted(bool succeeded, string errorMessage)
        {
            Program.Invoke(Program.MainWindow, delegate
                {
                    checksQueue--;
                    toolStripButtonRefresh.Enabled = true;
                    toolStripButtonRestoreDismissed.Enabled = true;
                    spinningTimer.Stop();

                    if (succeeded)
                    {
                        int alertCount = Updates.UpdateAlertsCount;

                        if (alertCount > 0)
                        {                            
                            tableLayoutPanel3.Visible = false;
                        }
                        else
                        {
                            pictureBoxProgress.Image = SystemIcons.Information.ToBitmap();
                            labelProgress.Text = Messages.AVAILABLE_UPDATES_NOT_FOUND;
                        }

                        Rebuild();
                    }
                    else
                    {
                        pictureBoxProgress.Image = SystemIcons.Error.ToBitmap();
                        labelProgress.Text = string.IsNullOrEmpty(errorMessage)
                                                 ? Messages.AVAILABLE_UPDATES_NOT_FOUND
                                                 : errorMessage;
                    }

                    CheckForUpdatesInProgress = false;
                });
        }

        private void InitializeProgressControls()
        {
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(32, 32);
            imageList.Images.AddRange(new Image[]
                {
                    Properties.Resources.SpinningFrame0,
                    Properties.Resources.SpinningFrame1,
                    Properties.Resources.SpinningFrame2,
                    Properties.Resources.SpinningFrame3,
                    Properties.Resources.SpinningFrame4,
                    Properties.Resources.SpinningFrame5,
                    Properties.Resources.SpinningFrame6,
                    Properties.Resources.SpinningFrame7
                });

            spinningTimer.Tick += timer_Tick;
            spinningTimer.Interval = 150;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            int imageIndex = ++currentSpinningFrame <= 7 ? currentSpinningFrame : currentSpinningFrame = 0;
            pictureBoxProgress.Image = imageList.Images[imageIndex];
        }

        private void SetFilterLabel()
        {
            toolStripLabelFiltersOnOff.Text = FilterIsOn
                                                  ? Messages.FILTERS_ON
                                                  : Messages.FILTERS_OFF;
        }
        
        private bool FilterIsOn
        {
            get
            {
                return toolStripDropDownButtonDateFilter.FilterIsOn ||
                    toolStripDropDownButtonServerFilter.FilterIsOn;
            }
        }

        private void Rebuild()
        {
            Program.AssertOnEventThread();

            if (!Visible)
                return;

            if (checksQueue > 0)
                return;

            SetFilterLabel();

            this.dataGridViewUpdates.Location = new Point(this.dataGridViewUpdates.Location.X, 51);
            
            try
            {
                dataGridViewUpdates.SuspendLayout();

                if (dataGridViewUpdates.RowCount > 0)
                {
                    StoreSelectedUpdates();
                    dataGridViewUpdates.Rows.Clear();
                    dataGridViewUpdates.Refresh();
                }

                var updates = new List<Alert>(Updates.UpdateAlerts);           

                if (updates.Count == 0)
                {
                    tableLayoutPanel3.Visible = true;
                    pictureBoxProgress.Image = SystemIcons.Information.ToBitmap();

                    if (SomeOrAllUpdatesDisabled())
                    {
                        labelProgress.Text = Messages.DISABLED_UPDATE_AUTOMATIC_CHECK_WARNING;
                        checkForUpdatesNowButton.Visible = true;
                        MakeWarningInvisible();
                    }
                    else
                    {
                        labelProgress.Text = Messages.AVAILABLE_UPDATES_NOT_FOUND;
                    }
                    return;
                }

                checkForUpdatesNowButton.Visible = false;

                if (SomeButNotAllUpdatesDisabled())
                {
                    this.dataGridViewUpdates.Location = new Point(this.dataGridViewUpdates.Location.X, 72);
                    MakeWarningVisible();
                }
                else
                {
                    MakeWarningInvisible();
                }

                updates.RemoveAll(FilterAlert);
                tableLayoutPanel3.Visible = false;

                if (dataGridViewUpdates.SortedColumn != null)
                {
                    if (dataGridViewUpdates.SortedColumn.Index == ColumnMessage.Index)
                        updates.Sort(Alert.CompareOnTitle);
                    else if (dataGridViewUpdates.SortedColumn.Index == ColumnDate.Index)
                        updates.Sort(Alert.CompareOnDate);
                    else if (dataGridViewUpdates.SortedColumn.Index == ColumnLocation.Index)
                        updates.Sort(Alert.CompareOnAppliesTo);

                    if (dataGridViewUpdates.SortOrder == SortOrder.Descending)
                        updates.Reverse();
                }

                var rowList = new List<DataGridViewRow>();

                foreach (var myAlert in updates)
                    rowList.Add(NewUpdateRow(myAlert));

                dataGridViewUpdates.Rows.AddRange(rowList.ToArray());

                foreach (DataGridViewRow row in dataGridViewUpdates.Rows)
                    row.Selected = selectedUpdates.Contains(((Alert)row.Tag).uuid);

                if (dataGridViewUpdates.SelectedRows.Count == 0 && dataGridViewUpdates.Rows.Count > 0)
                    dataGridViewUpdates.Rows[0].Selected = true;
            }
            finally
            {
                dataGridViewUpdates.ResumeLayout();
                UpdateButtonEnablement();
            }
        }

        /// <summary>
        /// Makes the warning that appears above the grid saying: "Automatic checking for updates 
        /// is disabled for some types of updates" visible.
        /// </summary>
        private void MakeWarningVisible()
        {
            pictureBox1.Visible = true;
            AutoCheckForUpdatesDisabledLabel.Visible = true;
            checkForUpdatesNowButton2.Visible = true;
        }

        /// <summary>
        /// Makes the warning that appears above the grid saying: "Automatic checking for updates 
        /// is disabled for some types of updates" invisible.
        /// </summary>
        private void MakeWarningInvisible()
        {
            pictureBox1.Visible = false;
            AutoCheckForUpdatesDisabledLabel.Visible = false;
            checkForUpdatesNowButton2.Visible = false;
        }

        /// <summary>
        /// Checks if the automatic checking for updates in the Updates Options Page is disabled for some, but not all types of updates.
        /// </summary>
        /// <returns></returns>
        private bool SomeButNotAllUpdatesDisabled()
        {
            return (!Properties.Settings.Default.AllowPatchesUpdates ||
                    !Properties.Settings.Default.AllowXenCenterUpdates ||
                    !Properties.Settings.Default.AllowXenServerUpdates) &&
                    (Properties.Settings.Default.AllowPatchesUpdates ||
                    Properties.Settings.Default.AllowXenCenterUpdates ||
                    Properties.Settings.Default.AllowXenServerUpdates) &&
                    !PageWasRefreshed;
        }

        /// <summary>
        /// Checks if the automatic checking for updates in the Updates Options Page is disabled for some or all types of updates.
        /// </summary>
        /// <returns></returns>
        private bool SomeOrAllUpdatesDisabled()
        {
            return !((Properties.Settings.Default.AllowPatchesUpdates &&
                   Properties.Settings.Default.AllowXenCenterUpdates &&
                   Properties.Settings.Default.AllowXenServerUpdates) ||
                   PageWasRefreshed);
        }

        /// <summary>
        /// Runs all the current filters on the alert to determine if it should be shown in the list or not.
        /// </summary>
        /// <param name="alert"></param>
        private bool FilterAlert(Alert alert)
        {
            var hosts = new List<string>();
            var serverUpdate = alert as XenServerUpdateAlert;
            if (serverUpdate != null)
                hosts = serverUpdate.DistinctHosts.Select(h => h.uuid).ToList();

            bool hide = false; 

            Program.Invoke(Program.MainWindow, () =>
                                 hide = toolStripDropDownButtonDateFilter.HideByDate(alert.Timestamp.ToLocalTime())
                                        || toolStripDropDownButtonServerFilter.HideByLocation(hosts) || alert.IsDismissed());
            return hide;
        }

        private void StoreSelectedUpdates()
        {
            selectedUpdates = (dataGridViewUpdates.SelectedRows.Cast<DataGridViewRow>().Select(
                    selectedRow => ((Alert)selectedRow.Tag).uuid)).ToList();
        }

        private void UpdateButtonEnablement()
        {
            toolStripButtonExportAll.Enabled = toolStripSplitButtonDismiss.Enabled = Updates.UpdateAlertsCount > 0;            
        }

        private void ShowInformationHelper(string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                tableLayoutPanel1.Visible = false;
            }
            else
            {
                informationLabel.Text = reason;
                tableLayoutPanel1.Visible = !HiddenFeatures.LinkLabelHidden;
            }
        }

        private DataGridViewRow NewUpdateRow(Alert alert)
        {
            var expanderCell = new DataGridViewImageCell();
            var appliesCell = new DataGridViewTextBoxCell();
            var detailCell = new DataGridViewTextBoxCell();
            var dateCell = new DataGridViewTextBoxCell();

            var actionItems = GetAlertActionItems(alert);
            var actionCell = new DataGridViewDropDownSplitButtonCell(actionItems.ToArray());
            var newRow = new DataGridViewRow { Tag = alert, MinimumHeight = DataGridViewDropDownSplitButtonCell.MIN_ROW_HEIGHT };

            // Set the detail cell content and expanding arrow
            if (expandedState.ContainsKey(alert.uuid))
            {
                // show the expanded arrow and the body detail
                expanderCell.Value = Properties.Resources.expanded_triangle;
                detailCell.Value = String.Format("{0}\n\n{1}", alert.Title, alert.Description);
            }
            else
            {
                // show the expand arrow and just the title
                expanderCell.Value = Properties.Resources.contracted_triangle;
                detailCell.Value = alert.Title;
            }

            appliesCell.Value = alert.AppliesTo;
            dateCell.Value = HelpersGUI.DateTimeToString(alert.Timestamp.ToLocalTime(), Messages.DATEFORMAT_DMY, true);
            newRow.Cells.AddRange(expanderCell, detailCell, appliesCell, dateCell, actionCell);

            return newRow;
        }

        private List<ToolStripItem> GetAlertActionItems(Alert alert)
        {
            var items = new List<ToolStripItem>();

            var patchAlert = alert as XenServerPatchAlert;

            if (Alert.AllowedToDismiss(alert))
            {
                var dismiss = new ToolStripMenuItem(Messages.ALERT_DISMISS);
                dismiss.Click += ToolStripMenuItemDismiss_Click;
                items.Add(dismiss);
            }

            if (patchAlert != null && patchAlert.CanApply && !string.IsNullOrEmpty(patchAlert.Patch.PatchUrl))
            {
                var download = new ToolStripMenuItem(Messages.UPDATES_DOWNLOAD_AND_INSTALL);
                download.Click += ToolStripMenuItemDownload_Click;
                items.Add(download);
            }

            if (!string.IsNullOrEmpty(alert.WebPageLabel))
            {
                var fix = new ToolStripMenuItem(alert.FixLinkText);
                fix.Click += ToolStripMenuItemGoToWebPage_Click;
                items.Add(fix);
            }

            if (items.Count > 0)
                items.Add(new ToolStripSeparator());

            var copy = new ToolStripMenuItem(Messages.COPY);
            copy.Click += ToolStripMenuItemCopy_Click;
            items.Add(copy);

            return items;
        }

      
        #region Update dismissal
 
      private void DismissUpdates(IEnumerable<Alert> alerts)
        {
            var groups = from Alert alert in alerts
                         where alert != null && !alert.Dismissing
                         group alert by alert.Connection
                         into g
                         select new { Connection = g.Key, Alerts = g };

            foreach (var g in groups)
            {
                if (Alert.AllowedToDismiss(g.Connection))
                {
                    foreach (Alert alert in g.Alerts)
                    {
                        alert.Dismissing = true;
                    }
                    toolStripButtonRestoreDismissed.Enabled = false;
                    DeleteAllAlertsAction action = new DeleteAllAlertsAction(g.Connection, g.Alerts);
                    action.RunAsync();
                    action.Completed += DeleteAllAllertAction_Completed;                    
                }
            }
        }

        /// <summary>
        /// After the Delete action is completed the page is refreshed and the restore dismissed 
        /// button is enabled again.
        /// </summary>
        /// <param name="sender"></param>
        private void DeleteAllAllertAction_Completed(ActionBase sender)
        {
            Program.Invoke(Program.MainWindow, () =>
            {                
                Rebuild();
                toolStripButtonRestoreDismissed.Enabled = true;
            });
        }

        #endregion


        #region Actions DropDown event handlers

        private void toolStripSplitButtonDismiss_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStripSplitButtonDismiss.DefaultItem = e.ClickedItem;
            toolStripSplitButtonDismiss.Text = toolStripSplitButtonDismiss.DefaultItem.Text;
        }

        /// <summary>
        /// If the answer of the user to the dialog is YES, then make a list with all the updates and call 
        /// DismissUpdates on that list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dismissAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.Yes;

            if (FilterIsOn)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.UPDATE_DISMISS_ALL_CONTINUE),
                    new ThreeButtonDialog.TBDButton(Messages.DISMISS_ALL_CONFIRM_BUTTON, DialogResult.Yes),
                    new ThreeButtonDialog.TBDButton(Messages.DISMISS_FILTERED_CONFIRM_BUTTON, DialogResult.No, ThreeButtonDialog.ButtonType.NONE),
                    ThreeButtonDialog.ButtonCancel))
                {
                    result = dlog.ShowDialog(this);
                }
            }
            else if (!Properties.Settings.Default.DoNotConfirmDismissUpdates)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.UPDATE_DISMISS_ALL_NO_FILTER_CONTINUE),
                    new ThreeButtonDialog.TBDButton(Messages.DISMISS_ALL_YES_CONFIRM_BUTTON, DialogResult.Yes),
                    ThreeButtonDialog.ButtonCancel)
                {
                    ShowCheckbox = true,
                    CheckboxCaption = Messages.DO_NOT_SHOW_THIS_MESSAGE
                })
                {
                    result = dlog.ShowDialog(this);
                    Properties.Settings.Default.DoNotConfirmDismissUpdates = dlog.IsCheckBoxChecked;
                    Settings.TrySaveSettings();
                }
            }

            if (result == DialogResult.Cancel)
                return;

            var alerts = result == DialogResult.No
                         ? from DataGridViewRow row in dataGridViewUpdates.Rows select row.Tag as Alert
                         : Updates.UpdateAlerts;

            DismissUpdates(alerts);
        }

        /// <summary>
        /// If the answer of the user to the dialog is YES, then make a list of all the selected rows
        /// and call DismissUpdates on that list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dismissSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.DoNotConfirmDismissUpdates)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.UPDATE_DISMISS_SELECTED_CONFIRM, Messages.XENCENTER),
                    ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                {
                    ShowCheckbox = true,
                    CheckboxCaption = Messages.DO_NOT_SHOW_THIS_MESSAGE
                })
                {
                    var result = dlog.ShowDialog(this);
                    Properties.Settings.Default.DoNotConfirmDismissUpdates = dlog.IsCheckBoxChecked;
                    Settings.TrySaveSettings();
                    
                    if (result != DialogResult.Yes)
                        return;
                }
            }

            if (dataGridViewUpdates.SelectedRows.Count > 0)
            {
                var selectedAlerts = from DataGridViewRow row in dataGridViewUpdates.SelectedRows select row.Tag as Alert;
                DismissUpdates(selectedAlerts);
            }
        }

        private void ToolStripMenuItemDismiss_Click(object sender, EventArgs e)
        {
            if (dataGridViewUpdates.SelectedRows.Count != 1)
                log.DebugFormat("Only 1 update can be dismissed at a time (Attempted to dismiss {0}). Dismissing the clicked item.", dataGridViewUpdates.SelectedRows.Count);

            DataGridViewRow clickedRow = FindAlertRow(sender as ToolStripMenuItem);
            if (clickedRow == null)
            {
                log.Debug("Attempted to dismiss update with no update selected.");
                return;
            }

            Alert alert = (Alert)clickedRow.Tag;
            if (alert == null)
                return;

            if (!Properties.Settings.Default.DoNotConfirmDismissUpdates)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.UPDATE_DISMISS_CONFIRM, Messages.XENCENTER),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo)
                {
                    ShowCheckbox = true,
                    CheckboxCaption = Messages.DO_NOT_SHOW_THIS_MESSAGE
                })
                {
                    var result = dlog.ShowDialog(this);
                    Properties.Settings.Default.DoNotConfirmDismissUpdates = dlog.IsCheckBoxChecked;
                    Settings.TrySaveSettings();
                    
                    if (result != DialogResult.Yes)
                        return;
                }
            }

            DismissUpdates(new List<Alert> { (Alert)clickedRow.Tag });
        }

        private DataGridViewRow FindAlertRow(ToolStripMenuItem toolStripMenuItem)
        {
            if (toolStripMenuItem == null)
                return null;

            return (from DataGridViewRow row in dataGridViewUpdates.Rows
                    where row.Cells.Count > 0
                    let actionCell = row.Cells[row.Cells.Count - 1] as DataGridViewDropDownSplitButtonCell
                    where actionCell != null && actionCell.ContextMenu.Items.Cast<object>().Any(item => item is ToolStripMenuItem && item == toolStripMenuItem)
                    select row).FirstOrDefault();
        }

        private void ToolStripMenuItemGoToWebPage_Click(object sender, EventArgs e)
        {
            DataGridViewRow clickedRow = FindAlertRow(sender as ToolStripMenuItem);
            if (clickedRow == null)
                return;

            Alert alert = (Alert) clickedRow.Tag;
            if (alert != null && alert.FixLinkAction != null)
                alert.FixLinkAction.Invoke();
        }

        private void ToolStripMenuItemDownload_Click(object sender, EventArgs e)
        {
            DataGridViewRow clickedRow = FindAlertRow(sender as ToolStripMenuItem);
            if (clickedRow == null)
                return;

            XenServerPatchAlert patchAlert = (XenServerPatchAlert) clickedRow.Tag;

            if (patchAlert == null)
                return;

            string patchUri = patchAlert.Patch.PatchUrl;
            if (string.IsNullOrEmpty(patchUri))
                return;

            Program.Invoke(Program.MainWindow, () =>
            {
                var wizard = new PatchingWizard();
                wizard.Show();
                wizard.NextStep();
                wizard.AddAlert(patchAlert);
                wizard.NextStep();

                var hosts = patchAlert.DistinctHosts;
                if (hosts.Count > 0)
                {                          
                    wizard.SelectServers(hosts);
                }
                else
                {
                    string disconnectedServerNames =
                           clickedRow.Cells[ColumnLocation.Index].Value.ToString();

                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Warning,
                                                      string.Format(Messages.UPDATES_WIZARD_DISCONNECTED_SERVER, 
                                                                   disconnectedServerNames),
                                                      Messages.UPDATES_WIZARD)))
                    {
                        dlg.ShowDialog(this);
                    }
                 }
            });
        }

        private void ToolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            DataGridViewRow clickedRow = FindAlertRow(sender as ToolStripMenuItem);
            if (clickedRow == null)
                return;

            StringBuilder sb = new StringBuilder();
            
            if (dataGridViewUpdates.SelectedRows.Count > 1 && dataGridViewUpdates.SelectedRows.Contains(clickedRow))
            {
                foreach (DataGridViewRow r in dataGridViewUpdates.SelectedRows)
                {
                    Alert alert = (Alert) r.Tag;
                    sb.AppendLine(alert.GetUpdateDetailsCSVQuotes());
                }
            }
            else
            {
                Alert alert = (Alert) clickedRow.Tag;
                if (alert != null) 
                    sb.AppendLine(alert.GetUpdateDetailsCSVQuotes());
            }

            Clip.SetClipboardText(sb.ToString());
        }

        #endregion

        #region DataGridView event handlers

        private void dataGridViewUpdates_SelectionChanged(object sender, EventArgs e)
        {
            string reason = null;

            if (dataGridViewUpdates.SelectedRows.Count > 0)
            {
                var alert = dataGridViewUpdates.SelectedRows[0].Tag as XenServerPatchAlert;
                
                if (alert != null)
                    reason = alert.CannotApplyReason;
            }

            ShowInformationHelper(reason);
        }

        private void dataGridViewUpdates_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewUpdates.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.Automatic)
                Rebuild();
        }

        /// <summary>
        /// Handles the automatic sorting of the AlertsGridView for the non-string columns
        /// </summary>
        private void dataGridViewUpdates_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            Alert alert1 = (Alert)dataGridViewUpdates.Rows[e.RowIndex1].Tag;
            Alert alert2 = (Alert)dataGridViewUpdates.Rows[e.RowIndex2].Tag;

            if (e.Column.Index == ColumnDate.Index)
            {
                int sortResult = DateTime.Compare(alert1.Timestamp, alert2.Timestamp);
                e.SortResult = (dataGridViewUpdates.SortOrder == SortOrder.Descending) ? sortResult *= -1 : sortResult;
                e.Handled = true;
            }
        }

        private void dataGridViewUpdates_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.ColumnIndex != ColumnExpand.Index)
                return;

            ToggleExpandedState(e.RowIndex);
        }

        private void dataGridViewUpdates_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;
            
            ToggleExpandedState(e.RowIndex);
        }

        private void dataGridViewUpdates_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right) // expand all selected rows
            {
                foreach (DataGridViewBand row in dataGridViewUpdates.SelectedRows)
                {
                    Alert alert = (Alert)dataGridViewUpdates.Rows[row.Index].Tag;
                    
                    if (!expandedState.ContainsKey(alert.uuid))
                        ToggleExpandedState(row.Index);
                }
            }
            else if (e.KeyCode == Keys.Left) // collapse all selected rows
            {
                foreach (DataGridViewBand row in dataGridViewUpdates.SelectedRows)
                {
                    Alert alert = (Alert)dataGridViewUpdates.Rows[row.Index].Tag;
                    
                    if (expandedState.ContainsKey(alert.uuid))
                        ToggleExpandedState(row.Index);
                }
            }
            else if (e.KeyCode == Keys.Enter) // toggle expanded state for all selected rows
            {
                foreach (DataGridViewBand row in dataGridViewUpdates.SelectedRows)
                    ToggleExpandedState(row.Index);
            }
        }

        /// <summary>
        /// Toggles the row specified between the expanded and contracted state
        /// </summary>
        private void ToggleExpandedState(int rowIndex)
        {
            Alert alert = (Alert)dataGridViewUpdates.Rows[rowIndex].Tag;

            if (expandedState.ContainsKey(alert.uuid))
            {
                expandedState.Remove(alert.uuid);
                dataGridViewUpdates.Rows[rowIndex].Cells[ColumnMessage.Index].Value = alert.Title;
                dataGridViewUpdates.Rows[rowIndex].Cells[ColumnExpand.Index].Value = Properties.Resources.contracted_triangle;
            }
            else
            {
                expandedState.Add(alert.uuid, true);
                dataGridViewUpdates.Rows[rowIndex].Cells[ColumnMessage.Index].Value
                    = string.Format("{0}\n\n{1}", alert.Title, alert.Description);
                dataGridViewUpdates.Rows[rowIndex].Cells[ColumnExpand.Index].Value = Properties.Resources.expanded_triangle;
            }
        }
        
        #endregion

        
        #region Top ToolStripButtons event handlers

        private void toolStripDropDownButtonDateFilter_FilterChanged()
        {
            Rebuild();
        }

        private void toolStripDropDownButtonServerFilter_FilterChanged()
        {
            Rebuild();
        }
        
        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            PageWasRefreshed = true;
            checkForUpdatesNowButton.Visible = false;
            Updates.CheckForUpdates(true);
        }

        private void toolStripButtonExportAll_Click(object sender, EventArgs e)
        {
            bool exportAll = true;

            if (FilterIsOn)
            {
                using (var dlog = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.UPDATE_EXPORT_ALL_OR_FILTERED),
                    new ThreeButtonDialog.TBDButton(Messages.ALERT_EXPORT_ALL_BUTTON, DialogResult.Yes),
                    new ThreeButtonDialog.TBDButton(Messages.ALERT_EXPORT_FILTERED_BUTTON, DialogResult.No, ThreeButtonDialog.ButtonType.NONE),
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
                Filter = string.Format("{0} (*.csv)|*.csv|{1} (*.*)|*.*",
                                       Messages.CSV_DESCRIPTION, Messages.ALL_FILES),
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
                    using (StreamWriter stream = new StreamWriter(fileName, false, UTF8Encoding.UTF8))
                    {
                        stream.WriteLine("{0},{1},{2},{3},{4}", Messages.TITLE,
                                         Messages.DESCRIPTION, Messages.APPLIES_TO,
                                         Messages.TIMESTAMP, Messages.WEB_PAGE);

                        if (exportAll)
                        {
                            foreach (Alert a in Updates.UpdateAlerts)
                                stream.WriteLine(a.GetUpdateDetailsCSVQuotes());
                        }
                        else
                        {
                            foreach (DataGridViewRow row in dataGridViewUpdates.Rows)
                            {
                                var a = row.Tag as Alert;
                                if (a != null)
                                    stream.WriteLine(a.GetUpdateDetailsCSVQuotes());
                            }
                        }
                    }
                }).RunAsync();
        }

        #endregion


        private void informationLabel_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(InvisibleMessages.UPSELL_SA);
            }
            catch (Exception)
            {
                using (var dlg = new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       string.Format(Messages.LICENSE_SERVER_COULD_NOT_OPEN_LINK, InvisibleMessages.LICENSE_SERVER_DOWNLOAD_LINK),
                       Messages.XENCENTER)))
                {
                    dlg.ShowDialog(this);
                }
            }
        }
        
        private void checkForUpdatesNowButton_Click(object sender, EventArgs e)
        {
            checkForUpdatesNowButton.Visible = false;
            Updates.CheckForUpdates(true);
            PageWasRefreshed = true;
        }

        private void toolStripButtonRestoreDismissed_Click(object sender, EventArgs e)
        {
            PageWasRefreshed = true;
            checkForUpdatesNowButton.Visible = false;
            Updates.RestoreDismissedUpdates();
        }        

        private void checkForUpdatesNowButton2_Click(object sender, EventArgs e)
        {            
            MakeWarningInvisible();
            PageWasRefreshed = true;
            Updates.CheckForUpdates(true);
        }

        private void tableLayoutPanel3_Resize(object sender, EventArgs e)
        {
            labelProgress.MaximumSize = new Size(tableLayoutPanel3.Width - 60, tableLayoutPanel3.Size.Height);
        }
    }
}
