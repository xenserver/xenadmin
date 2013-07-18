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
using System.Text;
using System.Windows.Forms;

using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Alerts;
using XenAdmin.Help;
using System.Threading;
using XenAdmin.Actions;
using System.IO;


namespace XenAdmin.Dialogs
{
    public partial class AlertSummaryDialog : XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly int ALERT_CAP = 1000;

        private readonly CollectionChangeEventHandler m_alertCollectionChangedWithInvoke;
        Dictionary<string, bool> expandedState = new Dictionary<string, bool>();
        private bool inAlertBuild;
        private bool retryAlertBuild;

        public AlertSummaryDialog()
        {
            InitializeComponent();
            toolStripDropDownButtonDateFilter.FilterChanged += toolStripDropDownButtonDateFilter_FilterChanged;
            toolStripDropDownButtonServerFilter.FilterChanged += toolStripDropDownButtonServerFilter_FilterChanged;
            toolStripDropDownSeveritiesFilter.FilterChanged += toolStripDropDownSeveritiesFilter_FilterChanged;
            GridViewAlerts.Sort(ColumnDate, ListSortDirection.Descending);
            LabelCappingEntries.Text = String.Format(Messages.ALERT_CAP_LABEL, ALERT_CAP);
            GridViewAlerts.ScrollBars = ScrollBars.Vertical;
            UpdateActionEnablement();

            m_alertCollectionChangedWithInvoke = Program.ProgramInvokeHandler(AlertsCollectionChanged);
            Alert.XenCenterAlerts.CollectionChanged += m_alertCollectionChangedWithInvoke;

            toolStripDropDownButtonServerFilter.InitializeHostList();
            toolStripDropDownButtonServerFilter.BuildFilterList();
            toolStripSplitButtonDismiss.DefaultItem = tsmiDismissAll;
            toolStripSplitButtonDismiss.Text = tsmiDismissAll.Text;
        }

        private void AlertSummaryDialog_Load(object sender, EventArgs e)
        {
            Rebuild();
        }
        
        private void SetFilterLabel()
        {
            bool filterIsOn = toolStripDropDownButtonDateFilter.FilterIsOn
                              || toolStripDropDownButtonServerFilter.FilterIsOn
                              || toolStripDropDownSeveritiesFilter.FilterIsOn;

            toolStripLabelFiltersOnOff.Text = filterIsOn
                                                  ? Messages.FILTERS_ON
                                                  : Messages.FILTERS_OFF;
        }

        #region AlertListCode

        private void Rebuild()
        {
            log.Debug("Rebuilding alertList");
            Thread t = new Thread(_Rebuild);
            t.Name = "Building alert list";
            t.IsBackground = true;
            t.Start();
        }

        private void _Rebuild()
        {
            log.Debug("Rebuilding alertList: Starting background thread");
            Program.AssertOffEventThread();
            lock (GridViewAlerts)
            {
                if (inAlertBuild)
                {
                    // queue up a rebuild after the current one has completed
                    log.Debug("Rebuilding alertList: In build already, exiting");
                    retryAlertBuild = true;
                    return;
                }
                inAlertBuild = true;
                log.Debug("Rebuilding alertList: taking inAlertBuild lock");
            }
            try
            {
                // 1) Add all the alerts that have not been filtered out to an array 
                // 2) Create rows for each of these
                // 3) Sort them 
                // 4) Take the top n as set by the filters
                // 5) Add them to the control using the optimized AddRange()

                Program.Invoke(this, SetFilterLabel);
                
                List<Alert> alerts = Alert.NonDismissingAlerts;
                alerts.RemoveAll(FilterAlert);
                
                log.DebugFormat("Rebuilding alertList: there are {0} alerts in total. After filtering we have {1}",
                    Alert.AlertCount,
                    alerts.Count);
                
                if (GridViewAlerts.SortedColumn != null)
                {
                    if (GridViewAlerts.SortedColumn.Index == ColumnMessage.Index)
                    {
                        alerts.Sort(Alert.CompareOnTitle);
                    }
                    else if (GridViewAlerts.SortedColumn.Index == ColumnDate.Index)
                    {
                        alerts.Sort(Alert.CompareOnDate);
                    }
                    else if (GridViewAlerts.SortedColumn.Index == ColumnLocation.Index)
                    {
                        alerts.Sort(Alert.CompareOnAppliesTo);
                    }
                    else if (GridViewAlerts.SortedColumn.Index == ColumnSeverity.Index)
                    {
                        alerts.Sort(Alert.CompareOnPriority);
                    }
                    if (GridViewAlerts.SortOrder == SortOrder.Descending)
                    {
                        alerts.Reverse();
                    }
                }
                int alertsFound = alerts.Count;

                if (ALERT_CAP < alerts.Count)
                {
                    log.DebugFormat("Rebuilding alertList: hit alert cap, hiding {0} alerts", alerts.Count - ALERT_CAP);
                    alerts.RemoveRange(ALERT_CAP, alerts.Count - ALERT_CAP);
                }

                Program.Invoke(this, delegate
                {
                    List<DataGridViewRow> gridRows = new List<DataGridViewRow>();
                    log.Debug("Rebuilding alertList: Adding alert rows");
                    foreach (Alert alert in alerts)
                        gridRows.Add(NewAlertRow(alert));
                    log.DebugFormat("Rebuilding alertList: Added {0} rows", gridRows.Count);

                    List<string> selection = (GridViewAlerts.SelectedRows.Cast<DataGridViewRow>().Select(
                        selectedRow => ((Alert)selectedRow.Tag).uuid)).ToList();

                    GridViewAlerts.Rows.Clear();
                    log.Debug("Rebuilding alertList: Cleared rows");
                    GridViewAlerts.Rows.AddRange(gridRows.ToArray());
                    log.DebugFormat("Rebuilding alertList: Added {0} rows to the grid", GridViewAlerts.Rows.Count);
                    LabelCappingEntries.Visible = (alertsFound > ALERT_CAP);

                    //restore selection
                    if (selection.Count > 0)
                    {
                        log.Debug("Rebuilding alertList: Restoring alert selection");
                        foreach (DataGridViewRow alertRow in GridViewAlerts.Rows)
                        {
                            alertRow.Selected = selection.Contains(((Alert)alertRow.Tag).uuid);
                        }
                        if (GridViewAlerts.SelectedRows.Count == 0 && GridViewAlerts.Rows.Count > 0)
                        {
                            GridViewAlerts.Rows[0].Selected = true;
                        }
                        log.DebugFormat("Rebuilding alertList: Selected {0} alerts", selection.Count);
                    }

                    UpdateActionEnablement();
                });
            }
            catch (Exception e)
            {
                log.ErrorFormat("Encountered exception when building list: {0}", e);
            }
            finally
            {
                log.Debug("Rebuilding alertList: Waiting for lock to clear inAlertBuild");
                lock (GridViewAlerts)
                {
                    inAlertBuild = false;
                    log.Debug("Rebuilding alertList: cleared inAlertBuild");
                    if (retryAlertBuild)
                    {
                        // we received a request to build while we were building, rebuild in case we missed something
                        retryAlertBuild = false;
                        log.Debug("Rebuilding alertList: we received a request to build while we were building, rebuild in case we missed something");
                        _Rebuild();
                    }
                }
            }
        }

        private DataGridViewRow NewAlertRow(Alert alert)
        {
            var expanderCell = new DataGridViewImageCell();
            var imageCell = new DataGridViewImageCell();
            var appliesCell = new DataGridViewTextBoxCell();
            var detailCell = new DataGridViewTextBoxCell();
            var dateCell = new DataGridViewTextBoxCell();

            var actionItems = GetAlertActionItems(alert);
            var actionCell = new DataGridViewDropDownSplitButtonCell(actionItems.ToArray());
            var newRow = new DataGridViewRow { Tag = alert, MinimumHeight = DataGridViewDropDownSplitButtonCell.MIN_ROW_HEIGHT };

            // Get the relevant image for the row depending on the type of the alert
            Image typeImage = alert is MessageAlert && ((MessageAlert)alert).Message.ShowOnGraphs
                                  ? Images.GetImage16For(((MessageAlert)alert).Message.Type)
                                  : Images.GetImage16For(alert.Priority);

            imageCell.Value = typeImage;

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
            dateCell.Value = HelpersGUI.DateTimeToString(alert.Timestamp.ToLocalTime(), Messages.DATEFORMAT_DMY_HM, true);
            newRow.Cells.AddRange(expanderCell, imageCell, detailCell, appliesCell, dateCell, actionCell);
            
            return newRow;
        }

        /// <summary>
        /// Runs all the current filters on the alert to determine if it should be shown in the list or not.
        /// </summary>
        /// <param name="alert"></param>
        private bool FilterAlert(Alert alert)
        {
            bool hide = false;
            Program.Invoke(this, () =>
                                 hide = toolStripDropDownButtonDateFilter.HideByDate(alert.Timestamp.ToLocalTime())
                                        || toolStripDropDownButtonServerFilter.HideByLocation(alert.HostUuid)
                                        || toolStripDropDownSeveritiesFilter.HideBySeverity(alert.Priority));
            return hide;
        }

        private void RemoveAlertRow(Alert a)
        {
            for (int i = 0; i < GridViewAlerts.Rows.Count; i++)
            {
                if (((Alert)GridViewAlerts.Rows[i].Tag).uuid == a.uuid)
                    GridViewAlerts.Rows.RemoveAt(i);
            }
            if (GridViewAlerts.Rows.Count < ALERT_CAP)
                LabelCappingEntries.Visible = false;
        }

        private void GridViewAlerts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.ColumnIndex != ColumnExpand.Index)
                return;

            ToggleExpandedState(e.RowIndex);
        }

        private void GridViewAlerts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            if (e.ColumnIndex != ColumnActions.Index)
                ToggleExpandedState(e.RowIndex);
        }

        private void GridViewAlerts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right) // expand all selected rows
            {
                foreach (DataGridViewBand row in GridViewAlerts.SelectedRows)
                {
                    Alert alert = (Alert)GridViewAlerts.Rows[row.Index].Tag;
                    if (!expandedState.ContainsKey(alert.uuid))
                    {
                        ToggleExpandedState(row.Index);
                    }
                }
            }
            else if (e.KeyCode == Keys.Left) // collapse all selected rows
            {
                foreach (DataGridViewBand row in GridViewAlerts.SelectedRows)
                {
                    Alert alert = (Alert)GridViewAlerts.Rows[row.Index].Tag;
                    if (expandedState.ContainsKey(alert.uuid))
                    {
                        ToggleExpandedState(row.Index);
                    }
                }
            }
            else if (e.KeyCode == Keys.Enter) // toggle expanded state for all selected rows
            {
                foreach (DataGridViewBand row in GridViewAlerts.SelectedRows)
                {
                    ToggleExpandedState(row.Index);
                }
            }
        }

        private void GridViewAlerts_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (GridViewAlerts.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.Automatic)
            {
                Rebuild();
            }
        }

        private void GridViewAlerts_SelectionChanged(object sender, EventArgs e)
        {
            // stop the buttons getting enabled/disabled during refresh, the rebuild will set them once it's finished
            if (inAlertBuild)
                return;
            UpdateActionEnablement();
        }

        /// <summary>
        /// Handles the automatic sorting of the AlertsGridView for the non-string columns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridViewAlerts_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            Alert alert1 = (Alert)GridViewAlerts.Rows[e.RowIndex1].Tag;
            Alert alert2 = (Alert)GridViewAlerts.Rows[e.RowIndex2].Tag;
            if (e.Column.Index == ColumnDate.Index)
            {
                int SortResult = DateTime.Compare(alert1.Timestamp, alert2.Timestamp);
                e.SortResult = (GridViewAlerts.SortOrder == SortOrder.Descending) ? SortResult *= -1 : SortResult;
                e.Handled = true;
            }
            else if (e.Column.Index == ColumnSeverity.Index)
            {
                e.SortResult = Alert.CompareOnPriority(alert1, alert2);
                e.Handled = true;
            }
        }

        private void ToggleExpandedState(int rowIndex)
        {
            var row = GridViewAlerts.Rows[rowIndex];
            Alert alert = row.Tag as Alert;
            if (alert == null)
                return;

            if (expandedState.ContainsKey(alert.uuid))
            {
                expandedState.Remove(alert.uuid);
                row.Cells[ColumnExpand.Index].Value = Properties.Resources.contracted_triangle;
                row.Cells[ColumnMessage.Index].Value = alert.Title;
            }
            else
            {
                expandedState.Add(alert.uuid, true);
                row.Cells[ColumnExpand.Index].Value = Properties.Resources.expanded_triangle;
                row.Cells[ColumnMessage.Index].Value = string.Format("{0}\n\n{1}", alert.Title, alert.Description);
            }
        }

        #endregion

        private void ToolStripMenuItemHelp_Click(object sender, EventArgs e)
        {
            // We should only be here if one item is selected, we dont do multi-help
            if (GridViewAlerts.SelectedRows.Count != 1)
                log.ErrorFormat("Can only launch help for 1 alert at a time (Attempted to launch {0}). Launching for the first one in the list)", GridViewAlerts.SelectedRows.Count);

            Alert alert = (Alert)GridViewAlerts.SelectedRows[0].Tag;
            if (alert.HelpID == null)
            {
                log.ErrorFormat("Attempted to launch help for alert {0} ({1}) but no helpID available. Not launching.", alert.Title, alert.uuid);
                return;
            }
            HelpManager.Launch(alert.HelpID);

        }

        private void ToolStripMenuItemFix_Click(object sender, EventArgs e)
        {
            // We should only be here if one item is selected, we dont do multi-fix
            if (GridViewAlerts.SelectedRows.Count == 0)
            {
                log.ErrorFormat("Attempted to fix alert with no alert selected.");
                return;
            }

            if (GridViewAlerts.SelectedRows.Count != 1)
                log.ErrorFormat("Only 1 alert can be fixed at a time (Attempted to fix {0}). Fixing the first one in the list.", GridViewAlerts.SelectedRows.Count);

            Alert alert = (Alert)GridViewAlerts.SelectedRows[0].Tag;
            if (alert.FixLinkAction == null)
            {
                log.ErrorFormat("Attempted to fix alert {0} ({1}) but no fix link action available. Not fixing.", alert.Title, alert.uuid);
                return;
            }
            alert.FixLinkAction.Invoke();
        }

        private void ToolStripMenuItemDismiss_Click(object sender, EventArgs e)
        {
            if (new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.ALERT_DISMISS_CONFIRM, Messages.XENCENTER),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo).ShowDialog(this) != DialogResult.Yes)
                return;

            Alert[] selectedAlerts = new Alert[GridViewAlerts.SelectedRows.Count];
            for (int i = 0; i < selectedAlerts.Length; i++)
                selectedAlerts[i] = (Alert)GridViewAlerts.SelectedRows[i].Tag;

            DismissAlerts(selectedAlerts);
        }

        private void tsmiDismissAll_Click(object sender, EventArgs e)
        {
            DialogResult result;
            if (GridViewAlerts.Rows.Count == Alert.Alerts.Length) //no filter, only two buttons
                result = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.ALERT_DISMISS_ALL_NO_FILTER_CONTINUE),
                    "DismissAllAlertsConfirmationDialog",
                    new ThreeButtonDialog.TBDButton(Messages.DISMISS_ALL_YES_CONFIRM_BUTTON, DialogResult.Yes),
                    ThreeButtonDialog.ButtonCancel).ShowDialog(this);
            else
                result = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(null, Messages.ALERT_DISMISS_ALL_CONTINUE),
                    "DismissAllAlertsConfirmationDialog",
                    new ThreeButtonDialog.TBDButton(Messages.DISMISS_ALL_CONFIRM_BUTTON, DialogResult.Yes),
                    new ThreeButtonDialog.TBDButton(Messages.DISMISS_FILTERED_CONFIRM_BUTTON, DialogResult.No, ThreeButtonDialog.ButtonType.NONE),
                    ThreeButtonDialog.ButtonCancel).ShowDialog(this);

            if (result == DialogResult.Cancel)
                return;

            List<Alert> dismissingAlerts = new List<Alert>();

            if (result == DialogResult.No)
            {
                //Dismiss Filtered
                for (int i = 0; i < GridViewAlerts.Rows.Count; i++)
                    dismissingAlerts.Add((Alert)GridViewAlerts.Rows[i].Tag);
            }
            else
            {
                //Dismiss All
                dismissingAlerts.AddRange(Alert.Alerts);
            }
            dismissingAlerts.RemoveAll(a => !AllowedToDismiss(a.Connection));
            DismissAlerts(dismissingAlerts.ToArray());
        }

        private void DismissAlerts(Alert[] alerts)
        {
            List<Alert> local_alerts = new List<Alert>();
            // we group the alerts by connection ready for dismissal
            Dictionary<IXenConnection, List<Alert>> alertGroups = new Dictionary<IXenConnection, List<Alert>>();

            foreach (Alert a in alerts)
            {
                if (a.Dismissing)
                    continue;
                IXenConnection c = a.Connection;
                if (c == null)
                {
                    local_alerts.Add(a);
                }
                else
                {
                    if (!alertGroups.ContainsKey(c))
                        alertGroups[c] = new List<Alert>();
                    List<Alert> l = alertGroups[c];
                    l.Add(a);
                }
                a.Dismissing = true;
            }
            // When we refresh now the alerts we are dismissing will be removed from the GridViewAlerts control as their dismissing flag
            // is set. This also updates the numbers in the filter list correctly.
            toolStripDropDownButtonServerFilter.RefreshLists();

            foreach (IXenConnection c in alertGroups.Keys)
            {
                _DismissAlerts(c, alertGroups[c]);
            }

            if (local_alerts.Count > 0)
                _DismissAlerts(null, local_alerts);
        }

        /// <param name="connection">
        /// May be null, in which case this is expected to be for client-side alerts.
        /// </param>
        private static void _DismissAlerts(IXenConnection connection, List<Alert> alerts)
        {
            new DeleteAllAlertsAction(connection, alerts).RunAsync();
        }

        private void AlertsCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();
            if (e.Element == null)
            {
                // We take the null element to mean there has been a batch remove
                Rebuild();
                return;
            }
            Alert a = e.Element as Alert;
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    Rebuild(); // rebuild entire alert list to ensure filtering and sorting
                    break;
                case CollectionChangeAction.Remove:
                    RemoveAlertRow(a);
                    break;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Alert.XenCenterAlerts.CollectionChanged -= m_alertCollectionChangedWithInvoke;
            base.OnFormClosing(e);
        }

        private void UpdateActionEnablement()
        {
            toolStripButtonExportAll.Enabled = GridViewAlerts.Rows.Count > 0;

            // We use the nondismissing alert count here because we dont wan't to
            // offer people the chance to dismiss alerts which are already being
            // dismissed... they aren't even shown in the datagridview

            tsmiDismissAll.Enabled = AllowedToDismissAtLeastOne();
            tsmiDismissAll.AutoToolTip = !tsmiDismissAll.Enabled;
            tsmiDismissAll.ToolTipText = tsmiDismissAll.Enabled
                                                          ? string.Empty
                                                          : Alert.NonDismissingAlertCount > 0
                                                                ? Messages.DELETE_ANY_MESSAGE_RBAC_BLOCKED
                                                                : Messages.NO_MESSAGES_TO_DISMISS;

            tsmiDismissSelected.Enabled = AllowedToDismissSelected();
            tsmiDismissSelected.AutoToolTip = !tsmiDismissSelected.Enabled;
            tsmiDismissSelected.ToolTipText = tsmiDismissSelected.Enabled
                                                  ? string.Empty
                                                  : Messages.DELETE_MESSAGE_RBAC_BLOCKED;
        }

        /// <summary>
        /// Check that there exists a non dismissing alert that can be dismissed.
        /// </summary>
        /// <returns></returns>
        private bool AllowedToDismissAtLeastOne()
        {
            Dictionary<IXenConnection, bool> connectionsChecked = new Dictionary<IXenConnection, bool>();
            foreach (Alert a in Alert.Alerts)
            {
                if (a.Connection != null && connectionsChecked.ContainsKey(a.Connection))
                    continue;

                if (a.Dismissing)
                    continue;

                if (AllowedToDismiss(a.Connection))
                    return true;

                // we can't dismiss from this connection, so don't bother checking in future
                connectionsChecked.Add(a.Connection, true);
            }
            return false;
        }

        /// <summary>
        /// Checks that each alert which is currently selected comes from a connection where the user has sufficient RBAC
        /// privileges to clear the alert.
        /// </summary>
        /// <returns></returns>
        private bool AllowedToDismissSelected()
        {
            List<IXenConnection> selectedAlertConnections = new List<IXenConnection>();
            foreach (DataGridViewRow r in GridViewAlerts.SelectedRows)
            {
                Alert alert = (Alert)r.Tag;
                if (alert.Connection != null && !selectedAlertConnections.Contains(alert.Connection))
                {
                    selectedAlertConnections.Add(alert.Connection);
                }
            }
            foreach (IXenConnection c in selectedAlertConnections)
            {
                // Check they are allowed to dismiss server alerts.
                if (!AllowedToDismiss(c))
                    return false;
            }
            return true;
        }

        private bool AllowedToDismiss(IXenConnection c)
        {
            // check if local alert
            if (c == null)
                return true;

            // have we disconnected? Alert will dissapear soon, but for now block dismissal.
            if (c.Session == null)
                return false;

            if (c.Session.IsLocalSuperuser || !Helpers.MidnightRideOrGreater(c))
                return true;

            List<Role> rolesAbleToCompleteAction = Role.ValidRoleList("Message.destroy", c);
            foreach (Role possibleRole in rolesAbleToCompleteAction)
            {
                if (c.Session.Roles.Contains(possibleRole))
                {
                    return true;
                }
            }
            return false;
        }

        private List<ToolStripItem> GetAlertActionItems(Alert alert)
        {
            var items = new List<ToolStripItem>();

            if (AllowedToDismissSelected())
            {
                var dismiss = new ToolStripMenuItem(Messages.ALERT_DISMISS);
                dismiss.Click += ToolStripMenuItemDismiss_Click;
                items.Add(dismiss);
            }

            if (!string.IsNullOrEmpty(alert.FixLinkText) && alert.FixLinkAction != null)
            {
                var fix = new ToolStripMenuItem(alert.FixLinkText);
                fix.Click += ToolStripMenuItemFix_Click;
                items.Add(fix);
            }

            if (!string.IsNullOrEmpty(alert.HelpID))
            {
                var help = new ToolStripMenuItem(alert.HelpLinkText);
                help.Click += ToolStripMenuItemHelp_Click;
                items.Add(help);
            }

            if (items.Count > 0)
                items.Add(new ToolStripSeparator());

            var copy = new ToolStripMenuItem(Messages.COPY);
            copy.Click += copyToolStripMenuItem_Click;
            items.Add(copy);

            return items;
        }

        #region Top ToolStrip event handlers

        private void toolStripDropDownButtonDateFilter_FilterChanged()
        {
            Rebuild();
        }

        private void toolStripDropDownButtonServerFilter_FilterChanged()
        {
            Rebuild();
        }

        private void toolStripDropDownSeveritiesFilter_FilterChanged()
        {
            Rebuild();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            Rebuild();
        }

        private void toolStripButtonExportAll_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.Filter = string.Format("{0} (*.csv)|*.csv|{1} (*.*)|*.*", Messages.CSV_DESCRIPTION, Messages.ALL_FILES);
            dialog.FilterIndex = 0;
            dialog.Title = Messages.EXPORT_ALL;
            dialog.RestoreDirectory = true;
            dialog.DefaultExt = "csv";
            dialog.CheckPathExists = false;
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            new DelegatedAsyncAction(null,
                String.Format(Messages.EXPORT_SYSTEM_ALERTS, dialog.FileName),
                String.Format(Messages.EXPORTING_SYSTEM_ALERTS, dialog.FileName),
                String.Format(Messages.EXPORTED_SYSTEM_ALERTS, dialog.FileName),
                delegate
                {
                    StreamWriter stream = new StreamWriter(dialog.FileName, false, UTF8Encoding.UTF8);
                    try
                    {
                        stream.WriteLine("{0},{1},{2},{3},{4}", Messages.TITLE,
                            Messages.SEVERITY, Messages.DESCRIPTION, Messages.APPLIES_TO, Messages.TIMESTAMP);
                        foreach (Alert a in Alert.Alerts)
                        {
                            stream.WriteLine(GetAlertDetailsCSVQuotes(a));
                        }
                    }
                    finally
                    {
                        stream.Close();
                        stream.Dispose();
                    }
                }).RunAsync();
        }

        private void toolStripSplitButtonDismiss_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStripSplitButtonDismiss.DefaultItem = e.ClickedItem;
            toolStripSplitButtonDismiss.Text = toolStripSplitButtonDismiss.DefaultItem.Text;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // We should only be here if one item is selected, we don't do multi-fix
            if (GridViewAlerts.SelectedRows.Count == 0)
            {
                log.ErrorFormat("Attempted to copy alert with no alert selected.");
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (DataGridViewRow r in GridViewAlerts.SelectedRows)
            {
                Alert alert = (Alert)r.Tag;
                sb.AppendLine(GetAlertDetailsCSVQuotes(alert));
            }

            try
            {
                Clipboard.SetText(sb.ToString());
            }
            catch (Exception ex)
            {
                log.Error("Exception while trying to set clipboard text.", ex);
                log.Error(ex, ex);
            }
        }

        private string GetAlertDetailsCSVQuotes(Alert a)
        {
            string date = String.Empty;
            string description = String.Empty;

            Program.Invoke(Program.MainWindow, delegate
                           {
                               date = HelpersGUI.DateTimeToString(
                                   a.Timestamp.ToLocalTime(),
                                   Messages.DATEFORMAT_DMY_HM, true);
                               description = a.Description;
                           });

            return String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"",
                                 EscapeQuotes(a.Title),
                                 EscapeQuotes(a.Priority.GetString()),
                                 EscapeQuotes(description),
                                 EscapeQuotes(a.AppliesTo),
                                 EscapeQuotes(date));
        }

        private string EscapeQuotes(string s)
        {
            return s == null ? null : s.Replace("\"", "\"\"");
        }

        #endregion
    }
}
