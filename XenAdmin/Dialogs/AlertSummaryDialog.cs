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

        private DateFilterDialog dateFilterDialog = new DateFilterDialog();

        private static readonly int ALERT_CAP = 1000;

        public AlertSummaryDialog()
        {
            InitializeComponent();
            toolStripDropDownButtonDateFilter.FilterChanged += toolStripDropDownButtonDateFilter_FilterChanged;
            GridViewAlerts.Sort(ColumnDate, ListSortDirection.Descending);
            Host_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(Host_CollectionChanged);
            LabelCappingEntries.Text = String.Format(Messages.ALERT_CAP_LABEL, ALERT_CAP);
            GridViewAlerts.ScrollBars = ScrollBars.Vertical;
            UpdateActionEnablement();

            // Build the list of hosts to filter by for the first time and set all of them to be checked
            CheckStates = new Dictionary<string, bool>();
            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (Host h in c.Cache.Hosts)
                    CheckStates[h.uuid] = true;

                foreach (Pool p in c.Cache.Pools)
                    CheckStates[p.uuid] = true;
            }
            buildFilterList();
            ConnectionsManager.XenConnections.CollectionChanged += new CollectionChangeEventHandler(XenConnections_CollectionChanged);
            Alert.XenCenterAlerts.CollectionChanged += CollectionChanged;
        }

        void AlertSummaryDialog_Load(object sender, EventArgs e)
        {
            Rebuild();
        }

        void XenConnections_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add)
            {
                IXenConnection c = e.Element as IXenConnection;
                foreach (Host h in c.Cache.Hosts)
                    CheckStates[h.uuid] = true;

                foreach (Pool p in c.Cache.Pools)
                    CheckStates[p.uuid] = true;
            }
        }


        #region Host Filter List code

        private Dictionary<string, bool> CheckStates;
        private static bool inFilterListUpdate;
        private static bool retryFilterListUpdate;
        private Dictionary<string, int> alertCounts;

        //Maintain a list of all the objects we currently have events on for clearing out on rebuild
        private List<IXenConnection> connectionsWithEvents = new List<IXenConnection>();
        private List<Host> hostsWithEvents = new List<Host>();

        private void buildFilterList()
        {
            Program.AssertOnEventThread();
            if (inFilterListUpdate)
            {
                // queue up an update after the current one has finished, in case the update has missed 
                // the relevant change
                retryFilterListUpdate = true;
                return;
            }
            inFilterListUpdate = true;
            alertCounts = new Dictionary<string, int>();
            foreach (Alert alert in Alert.Alerts)
            {
                if (alert.Dismissing)
                    continue;
                MessageAlert m = alert as MessageAlert;
                if (m == null)
                    continue;
                if (m.HostUuid != null)
                {
                    if (alertCounts.ContainsKey(m.HostUuid))
                        alertCounts[m.HostUuid] += 1;
                    else
                        alertCounts[m.HostUuid] = 1;
                }
            }
            try
            {
                toolStripDropDownButtonServerFilter.DropDownItems.Clear();
                DeregisterEvents();
                RegisterEvents();
                foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
                {
                    Pool p = Helpers.GetPool(c);
                    if (p == null)
                    {
                        // Stand alone host
                        foreach (Host h in c.Cache.Hosts)
                        {
                            toolStripDropDownButtonServerFilter.DropDownItems.Add(GenerateHostFilterItem(h));
                            break;
                        }
                    }
                    else
                        toolStripDropDownButtonServerFilter.DropDownItems.Add(GeneratePoolFilterItem(p));
                }
            }
            finally
            {
                inFilterListUpdate = false;
                if (retryFilterListUpdate)
                {
                    // there was a request to update while-st we were building, rebuild in case we missed something
                    retryFilterListUpdate = false;
                    buildFilterList();
                }
            }
        }

        private ToolStripMenuItem GeneratePoolFilterItem(Pool p)
        {
            int poolAlertCount = 0;
            List<ToolStripMenuItem> subItems = new List<ToolStripMenuItem>();
            bool allChecked = true;
            foreach (Host h in p.Connection.Cache.Hosts)
            {
                if (alertCounts.ContainsKey(h.uuid))
                {
                    poolAlertCount += alertCounts[h.uuid];
                }
                subItems.Add(GenerateHostFilterItem(h));
                if (!CheckStates.ContainsKey(h.uuid))
                {
                    allChecked = false;
                }
            }
            //TODO: Is there some way we can use these alert counts? Uncomment to see them in the filter menu... looks a bit odd.
            //ToolStripMenuItem t = new ToolStripMenuItem(string.Format("{0} ({1} alerts)", Helpers.GetName(p), poolAlertCount));
            ToolStripMenuItem t = new ToolStripMenuItem(Helpers.GetName(p));
            t.Tag = p.uuid;
            t.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            t.DropDownItems.AddRange(subItems.ToArray());
            t.Checked = allChecked;
            t.Click += new EventHandler(t_Click);
            t.Image = Images.GetImage16For(p);
            return t;
        }

        void t_Click(object sender, EventArgs e)
        {
            ToolStripItem t = sender as ToolStripItem;
            ToolStripItemClickedEventArgs args = new ToolStripItemClickedEventArgs(t);
            toolStripDropDownButtonServerFilter_DropDownItemClicked(null, args);
        }

        private ToolStripMenuItem GenerateHostFilterItem(Host h)
        {
            string name = Helpers.GetName(h);
            //TODO: Is there some way we can use these alert counts? Uncomment to see them in the filter menu... looks a bit odd.
            //if (alertCounts.ContainsKey(h.uuid))
            //{
            //    name = string.Format("{0} ({1} alerts)",
            //        Helpers.TrimStringIfRequired(Helpers.GetName(h), 50),
            //        alertCounts[h.uuid].ToString());
            //}
            //else
            //{
            //    name = string.Format("{0} (0 alerts)",
            //         Helpers.TrimStringIfRequired(Helpers.GetName(h), 50));
            //}
            ToolStripMenuItem t = new ToolStripMenuItem(name);
            t.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            t.Tag = h.uuid;
            t.Checked = CheckStates.ContainsKey(h.uuid);
            t.Click += new EventHandler(t_Click);
            t.Image = Images.GetImage16For(h);
            return t;
        }

        private void DeregisterEvents()
        {
            foreach (IXenConnection c in connectionsWithEvents)
            {
                c.ConnectionStateChanged -= connection_ConnectionStateChanged;
                c.Cache.DeregisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
                c.CachePopulated -= connection_CachePopulated;
            }
            foreach (Host h in hostsWithEvents)
            {
                DeregisterHostEvents(h);
            }
            connectionsWithEvents.Clear();
            hostsWithEvents.Clear();
        }

        private readonly CollectionChangeEventHandler Host_CollectionChangedWithInvoke;
        private void RegisterEvents()
        {
            foreach (IXenConnection c in ConnectionsManager.XenConnectionsCopy)
            {
                c.ConnectionStateChanged += connection_ConnectionStateChanged;
                c.Cache.RegisterCollectionChanged<Host>(Host_CollectionChangedWithInvoke);
                c.CachePopulated += connection_CachePopulated;
                connectionsWithEvents.Add(c);
                foreach (Host host in c.Cache.Hosts)
                {
                    RegisterHostEvents(host);
                    hostsWithEvents.Add(host);
                }
            }
        }

        private void RegisterHostEvents(Host host)
        {
            Host_metrics metrics = host.Connection.Resolve<Host_metrics>(host.metrics);
            if (metrics != null)
                metrics.PropertyChanged += new PropertyChangedEventHandler(hostMetrics_PropertyChanged);
            host.PropertyChanged += new PropertyChangedEventHandler(host_PropertyChanged);
        }

        private void DeregisterHostEvents(Host host)
        {
            Host_metrics metrics = host.Connection.Resolve<Host_metrics>(host.metrics);
            if (metrics != null)
                metrics.PropertyChanged -= new PropertyChangedEventHandler(hostMetrics_PropertyChanged);
            host.PropertyChanged -= new PropertyChangedEventHandler(host_PropertyChanged);
        }

        void connection_CachePopulated(object sender, EventArgs e)
        {
            Program.Invoke(this, refreshLists);
        }

        private void Host_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add)
            {
                CheckStates[((Host)e.Element).uuid] = true;
            }
            Program.Invoke(this, refreshLists);
        }

        private void hostMetrics_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "live")
                Program.Invoke(this, refreshLists);
        }

        private void host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "metrics")

                Program.Invoke(this, refreshLists);
        }

        void connection_ConnectionStateChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, refreshLists);
        }

        private void refreshLists()
        {
            buildFilterList();
            Rebuild();
        }

        private void toolStripDropDownButtonServerFilter_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem t = e.ClickedItem as ToolStripMenuItem;
            if (t == null)
                return;

            string uuid = (string)t.Tag;
            if (uuid == null)
                return;

            // toggle the check state
            // CA-103341 check the key exists when getting it; if it doesn't consider
            // its checkstate as false, in which case toggle to true
            bool check = CheckStates.ContainsKey(uuid) ? !CheckStates[uuid] : true;
            CheckStates[uuid] = check;
            t.Checked = check;

            if (t.HasDropDownItems)
            {
                //this is a pool check/uncheck its hosts
                foreach (ToolStripItem child in t.DropDownItems)
                {
                    ToolStripMenuItem h = child as ToolStripMenuItem;
                    if (h == null)
                        continue;

                    string hUuid = (string)h.Tag;
                    if (hUuid == null)
                        continue;

                    h.Checked = check;
                    CheckStates[hUuid] = check;
                }
            }
            else if (t.OwnerItem != null && t.OwnerItem is ToolStripMenuItem)
            {
                // this is a host, look and see if we need to check/uncheck the pool
                ToolStripMenuItem p = t.OwnerItem as ToolStripMenuItem;
                string pUuid = (string)p.Tag;
                if (pUuid == null)
                    return;

                bool allChecked = true;
                foreach (ToolStripItem child in p.DropDownItems)
                {
                    ToolStripMenuItem h = child as ToolStripMenuItem;
                    if (h == null)
                        continue;

                    string hUuid = (string)h.Tag;
                    if (hUuid == null)
                        continue;

                    if (!CheckStates.ContainsKey(hUuid) || !CheckStates[hUuid])
                    {
                        allChecked = false;
                        break;
                    }
                }

                p.Checked = allChecked;
                CheckStates[pUuid] = allChecked;
            }
            Rebuild();
        }

        #endregion

        #region AlertListCode

        Dictionary<string, bool> expandedState = new Dictionary<string, bool>();
        private bool inAlertBuild;
        private bool retryAlertBuild;


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

                List<Alert> alerts = Alert.NonDismissingAlerts;
                alerts.RemoveAll(filterAlert);
                
                log.DebugFormat("Rebuilding alertList: there are {0} alerts in total. After filtering we have {1}",
                    Alert.AlertCount,
                    alerts.Count);
                
                if (GridViewAlerts.SortedColumn != null)
                {
                    if (GridViewAlerts.SortedColumn.Index == ColumnDetails.Index)
                    {
                        alerts.Sort(CompareAlertsOnDetails);
                    }
                    else if (GridViewAlerts.SortedColumn.Index == ColumnDate.Index)
                    {
                        alerts.Sort(CompareAlertsOnDate);
                    }
                    else if (GridViewAlerts.SortedColumn.Index == ColumnAppliesTo.Index)
                    {
                        alerts.Sort(CompareAlertsOnAppliesTo);
                    }
                    else if (GridViewAlerts.SortedColumn.Index == ColumnType.Index)
                    {
                        alerts.Sort(CompareAlertsOnPriority);
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
            DataGridViewImageCell expanderCell = new DataGridViewImageCell();
            DataGridViewImageCell imageCell = new DataGridViewImageCell();
            DataGridViewTextBoxCell appliesCell = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell detailCell = new DataGridViewTextBoxCell();
            DataGridViewTextBoxCell dateCell = new DataGridViewTextBoxCell();
            DataGridViewRow newRow = new DataGridViewRow();
            newRow.Tag = alert;

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

            newRow.Cells.Add(expanderCell);
            newRow.Cells.Add(imageCell);
            newRow.Cells.Add(appliesCell);
            newRow.Cells.Add(detailCell);
            newRow.Cells.Add(dateCell);

            return newRow;

        }

        #region Alert Row Filtering code
        /// <summary>
        /// Runs all the current filters on the alert to determine if it should be shown in the list or not.
        /// </summary>
        /// <param name="alert"></param>
        private bool filterAlert(Alert alert)
        {
            bool hiddenByDate = false;
            Program.Invoke(this, delegate
            {
                hiddenByDate = toolStripDropDownButtonDateFilter.HideByDate(alert.Timestamp.ToLocalTime());
            });
            return hiddenByDate || hiddenByHostFilter(alert) || HiddenBySeverityFilter(alert);
        }

        /// <summary>
        /// Filter returns true if the alert came from a host and the host is checked in the filter list, or is not a host alert.
        /// Filter returns false if the alert is a host alert, but its relevant host entry is either unchecked or missing from the filter list.
        /// </summary>
        /// <param name="alert"></param>
        /// <returns></returns>
        private bool hiddenByHostFilter(Alert alert)
        {
            if (alert.HostUuid == null)
                return false;
            bool value;
            // if the host checkbox is not ticked then the message is hidden
            if (CheckStates.TryGetValue(alert.HostUuid, out value))
                return !value;
            else
                return false;
        }

        private bool HiddenBySeverityFilter(Alert alert)
        {
            return !((dataLossImminentToolStripMenuItem.Checked && alert.Priority == AlertPriority.Priority1)
                     || (serviceLossImminentToolStripMenuItem.Checked && alert.Priority == AlertPriority.Priority2)
                     || (serviceDegradedToolStripMenuItem.Checked && alert.Priority == AlertPriority.Priority3)
                     || (serviceRecoveredToolStripMenuItem.Checked && alert.Priority == AlertPriority.Priority4)
                     || (informationalToolStripMenuItem.Checked && alert.Priority == AlertPriority.Priority5)
                     || unknownToolStripMenuItem.Checked && alert.Priority == AlertPriority.Unknown);
        }

        #endregion

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

        private void GridViewAlerts_MouseDown(object sender, MouseEventArgs e)
        {
            //CA-66824, CA-71820
            if (e.Button == MouseButtons.Right)
            {
                var hitTest = GridViewAlerts.HitTest(e.X, e.Y);

                if (hitTest.RowIndex < 0 || hitTest.RowIndex >= GridViewAlerts.RowCount)
                    return;
                if (hitTest.ColumnIndex < 0 || hitTest.ColumnIndex >= GridViewAlerts.ColumnCount)
                    return;

                GridViewAlerts.ClearSelection();
                GridViewAlerts[hitTest.ColumnIndex, hitTest.RowIndex].Selected = true;
            }
        }

        private void AlertsGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.ColumnIndex != ColumnExpand.Index)
                return;

            toggleExpandedState(e.RowIndex);
        }

        private void AlertsGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;
            toggleExpandedState(e.RowIndex);
        }

        /// <summary>
        /// Toggles the row specified between the expanded and contracted state
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="RowIndex"></param>
        private void toggleExpandedState(int RowIndex)
        {

            Alert alert = (Alert)GridViewAlerts.Rows[RowIndex].Tag;
            if (expandedState.ContainsKey(alert.uuid))
            {
                expandedState.Remove(alert.uuid);
                GridViewAlerts.Rows[RowIndex].Cells[ColumnDetails.Index].Value = alert.Title;
                GridViewAlerts.Rows[RowIndex].Cells[ColumnExpand.Index].Value = Properties.Resources.contracted_triangle;
            }
            else
            {
                expandedState.Add(alert.uuid, true);
                GridViewAlerts.Rows[RowIndex].Cells[ColumnDetails.Index].Value
                    = String.Format("{0}\n\n{1}", alert.Title, alert.Description);
                GridViewAlerts.Rows[RowIndex].Cells[ColumnExpand.Index].Value = Properties.Resources.expanded_triangle;
            }
        }

        #region sorting code

        private static int CompareAlertsOnDate(Alert alert1, Alert alert2)
        {
            int SortResult = DateTime.Compare(alert1.Timestamp, alert2.Timestamp);
            if (SortResult == 0)
                SortResult = (String.Compare(alert1.uuid, alert2.uuid));
            return SortResult;
        }

        /// <summary>
        /// Internal method used for sorting two alerts by priority based on their type, tie broken by uuid
        /// </summary>
        private static int CompareAlertsOnPriority(Alert alert1, Alert alert2)
        {
            //the Unknown priority is lowest of all
            //it's only given the value 0 because this is the default for integers
            if (alert1.Priority < alert2.Priority)
                return alert1.Priority == 0 ? 1 : -1;

            if (alert1.Priority > alert2.Priority)
                return alert2.Priority == 0 ? -1 : 1;

            return (String.Compare(alert1.uuid, alert2.uuid));
        }

        private static int CompareAlertsOnDetails(Alert alert1, Alert alert2)
        {
            int SortResult = String.Compare(alert1.Title, alert2.Title);
            if (SortResult == 0)
                SortResult = (String.Compare(alert1.uuid, alert2.uuid));
            return SortResult;
        }

        private static int CompareAlertsOnAppliesTo(Alert alert1, Alert alert2)
        {
            int SortResult = String.Compare(alert1.AppliesTo, alert2.AppliesTo);
            if (SortResult == 0)
                SortResult = (String.Compare(alert1.uuid, alert2.uuid));
            return SortResult;
        }

        /// <summary>
        /// Handles the automatic sorting of the AlertsGridView for the non-string columns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AlertsGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            Alert alert1 = (Alert)GridViewAlerts.Rows[e.RowIndex1].Tag;
            Alert alert2 = (Alert)GridViewAlerts.Rows[e.RowIndex2].Tag;
            if (e.Column.Index == ColumnDate.Index)
            {
                int SortResult = DateTime.Compare(alert1.Timestamp, alert2.Timestamp);
                e.SortResult = (GridViewAlerts.SortOrder == SortOrder.Descending) ? SortResult *= -1 : SortResult;
                e.Handled = true;
            }
            else if (e.Column.Index == ColumnType.Index)
            {
                e.SortResult = CompareAlertsOnPriority(alert1, alert2);
                e.Handled = true;
            }
        }

        #endregion

        #endregion

        #region Alert Actions Logic

        private void ButtonHelp_Click(object sender, EventArgs e)
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

        private void ButtonFix_Click(object sender, EventArgs e)
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

        private void ButtonDismiss_Click(object sender, EventArgs e)
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

        private void DismissAllButton_Click(object sender, EventArgs e)
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
            dismissingAlerts.RemoveAll(delegate(Alert a)
            {
                return !AllowedToDismiss(a.Connection);
            });
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
            refreshLists();
            foreach (IXenConnection c in alertGroups.Keys)
            {
                _DismissAlerts(c, alertGroups[c]);
            }

            if (local_alerts.Count > 0)
                _DismissAlerts(null, local_alerts);
        }

        /// <param name="connection">May be null, in which case this is expected to be for client-side alerts.</param>
        private static void _DismissAlerts(IXenConnection connection, List<Alert> alerts)
        {
            new DeleteAllAlertsAction(connection, alerts).RunAsync();
        }
        #endregion

        private void CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(this, (CollectionChangeEventHandler)CollectionChanged_, sender, e);
        }

        private void CollectionChanged_(object sender, CollectionChangeEventArgs e)
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
            Alert.XenCenterAlerts.CollectionChanged -= CollectionChanged;
            ConnectionsManager.XenConnections.CollectionChanged -= new CollectionChangeEventHandler(XenConnections_CollectionChanged);
            base.OnFormClosing(e);
        }

        private void AlertsGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
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

        private void UpdateActionEnablement()
        {
            if (GridViewAlerts.SelectedRows.Count == 0)
            {
                toolStripButtonFix.Visible = toolStripButtonHelp.Visible = false;
                toolStripButtonDismiss.Visible = true;
                toolStripButtonDismiss.Enabled = false;
            }
            else if (GridViewAlerts.SelectedRows.Count > 1)
            {
                toolStripButtonDismiss.Visible = true;
                // the only multiselect action we support is a dismiss
                toolStripButtonDismiss.Enabled = AllowedToDismissSelected();
                toolStripButtonDismiss.AutoToolTip = !toolStripButtonDismiss.Enabled;
                toolStripButtonDismiss.ToolTipText = toolStripButtonDismiss.Enabled ? string.Empty : Messages.DELETE_MESSAGE_RBAC_BLOCKED;
                toolStripButtonFix.Visible = toolStripButtonHelp.Visible = false;
            }
            else
            {
                toolStripButtonDismiss.Visible = true;
                toolStripButtonDismiss.Enabled = AllowedToDismissSelected();
                toolStripButtonDismiss.AutoToolTip = !toolStripButtonDismiss.Enabled;
                toolStripButtonDismiss.ToolTipText = toolStripButtonDismiss.Enabled ? string.Empty : Messages.DELETE_MESSAGE_RBAC_BLOCKED;
                Alert alert = (Alert)GridViewAlerts.SelectedRows[0].Tag;
                if (!string.IsNullOrEmpty(alert.HelpID))
                {
                    toolStripButtonHelp.Visible = true;
                    toolStripButtonHelp.Text = alert.HelpLinkText;
                }
                else
                {
                    toolStripButtonHelp.Visible = false;
                }

                if (string.IsNullOrEmpty(alert.FixLinkText) || alert.FixLinkAction == null)
                {
                    toolStripButtonFix.Visible = false;
                }
                else
                {
                    toolStripButtonFix.Visible = true;
                    toolStripButtonFix.Text = alert.FixLinkText;
                }
            }

            toolStripButtonExportAll.Enabled = GridViewAlerts.Rows.Count > 0;

            toolStripButtonDismissAll.Enabled = AllowedToDismissAtLeastOne();
            // We use the nondismissing alert count here because we dont want to offer people the chance to dismiss alerts which are already being dismissed... they aren't even shown in the datagridview
            toolStripButtonDismissAll.AutoToolTip = !toolStripButtonDismissAll.Enabled;
            toolStripButtonDismissAll.ToolTipText = toolStripButtonDismissAll.Enabled ? string.Empty :
                Alert.NonDismissingAlertCount > 0 ? Messages.DELETE_ANY_MESSAGE_RBAC_BLOCKED : Messages.NO_MESSAGES_TO_DISMISS;
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

        private void ToolStripMenuItemFix_Click(object sender, EventArgs e)
        {
            ButtonFix_Click(null, null);
        }

        private void ToolStripMenuItemHelp_Click(object sender, EventArgs e)
        {
            ButtonHelp_Click(null, null);
        }

        private void ToolStripMenuItemDismiss_Click(object sender, EventArgs e)
        {
            ButtonDismiss_Click(null, null);
        }

        private void ContextMenuAlertGridView_Opening(object sender, CancelEventArgs e)
        {
            Program.Invoke(this, delegate
            {
                UpdateActionEnablement();
                ContextMenuAlertGridView.Items.Clear();
                if (toolStripButtonFix.Visible)
                {
                    ContextMenuAlertGridView.Items.Add(ToolStripMenuItemFix);
                    ToolStripMenuItemFix.Text = toolStripButtonFix.Text;
                }
                if (ToolStripMenuItemHelp.Visible)
                {
                    ContextMenuAlertGridView.Items.Add(ToolStripMenuItemHelp);
                    ToolStripMenuItemHelp.Text = toolStripButtonHelp.Text;
                }
                if (toolStripButtonDismiss.Visible)
                {
                    ContextMenuAlertGridView.Items.Add(ToolStripMenuItemDismiss);
                    ToolStripMenuItemDismiss.Enabled = toolStripButtonDismiss.Enabled;
                    ToolStripMenuItemDismiss.ToolTipText = toolStripButtonDismiss.ToolTipText;
                }
                if (ContextMenuAlertGridView.Items.Count > 0)
                    ContextMenuAlertGridView.Items.Add(toolStripSeparator2);

                ContextMenuAlertGridView.Items.Add(copyToolStripMenuItem);

            });
        }

        private void toolStripDropDownButtonDateFilter_FilterChanged()
        {
            Rebuild();
        }

        private void toolStripDropDownSeveritiesFilter_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
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
                delegate(Session _)
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

        private string EscapeQuotes(string s)
        {
            if (s == null)
                return null;

            return s.Replace("\"", "\"\"");
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // We should only be here if one item is selected, we dont do multi-fix
            if (GridViewAlerts.SelectedRows.Count == 0)
            {
                log.ErrorFormat("Attempted to copy alert with no alert selected.");
                return;
            }


            Alert alert;
            StringBuilder sb = new StringBuilder();
            foreach (DataGridViewRow r in GridViewAlerts.SelectedRows)
            {
                alert = (Alert)r.Tag;
                sb.AppendLine(GetAlertDetailsCSVQuotes(alert));
            }
            string text = sb.ToString().Trim();
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

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            Rebuild();
        }

        private void AlertSummaryDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            DeregisterEvents();
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
                        toggleExpandedState(row.Index);
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
                        toggleExpandedState(row.Index);
                    }
                }
            }
            else if (e.KeyCode == Keys.Enter) // toggle expanded state for all selected rows
            {   
                foreach (DataGridViewBand row in GridViewAlerts.SelectedRows)
                {
                    toggleExpandedState(row.Index);
                }
            }
        }
    }
}
