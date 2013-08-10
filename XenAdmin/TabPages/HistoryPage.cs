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

using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.TabPages
{
    public partial class HistoryPage : UserControl
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public HistoryPage()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            ConnectionsManager.History.CollectionChanged += History_CollectionChanged;
            toolStripTop.Renderer = new CustomToolStripRenderer();
            toolStripSplitButtonDismiss.DefaultItem = tsmiDismissAll;
            toolStripSplitButtonDismiss.Text = tsmiDismissAll.Text;
        }

        private void History_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.BeginInvoke(Program.MainWindow, () =>
            {
                ActionBase action = (ActionBase)e.Element;
                switch (e.Action)
                {
                    case CollectionChangeAction.Add:
                        AddActionRow(action);
                        break;
                    case CollectionChangeAction.Remove:
                        RemoveActionRow(action);
                        break;
                    case CollectionChangeAction.Refresh:
                        BuildRowList();
                        break;
                }
                UpdateButtons();
            });
        }

        private void action_Changed(ActionBase sender)
        {
            Program.Invoke(Program.MainWindow, () =>
                {
                    var row = FindRowFromAction(sender);
                    if (row != null)
                        row.RefreshSelf();

                    UpdateButtons();
                });
        }

        private void BuildRowList()
        {
            try
            {
                dataGridView.SuspendLayout();
                dataGridView.Rows.Clear();

                var rows = new List<DataGridViewActionRow>();
                foreach (ActionBase action in ConnectionsManager.History)
                {
                    var row = CreateActionRow(action);
                    rows.Add(row);
                }
                dataGridView.Rows.AddRange(rows.ToArray());

                foreach(var actionRow in rows)
                    RegisterActionEvents(actionRow.Action);
            }
            finally
            {
                dataGridView.ResumeLayout();
            }
        }

        private void AddActionRow(ActionBase action)
        {
            var row = CreateActionRow(action);
            dataGridView.Rows.Add(row);
            RegisterActionEvents(action);
        }

        private DataGridViewActionRow CreateActionRow(ActionBase action)
        {
            var row = new DataGridViewActionRow(action);
            row.DismissalRequested += row_DismissalRequested;
            row.GoToXenObjectRequested += row_GoToXenObjectRequested;
            return row;
        }

        private void RegisterActionEvents(ActionBase action)
        {
            action.Changed -= action_Changed;
            action.Completed -= action_Changed;
            action.Changed += action_Changed;
            action.Completed += action_Changed;
        }

        private void RemoveActionRow(ActionBase action)
        {
            var row = FindRowFromAction(action);
            if (row != null)
            {
                action.Changed -= action_Changed;
                action.Completed -= action_Changed;
                dataGridView.Rows.Remove(row);
            }
        }

        private DataGridViewActionRow FindRowFromAction(ActionBase action)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                var actionRow = row as DataGridViewActionRow;
                if (actionRow == null)
                    continue;

                if (actionRow.Action == action)
                    return actionRow;
            }

            return null;
        }

        private void ToggleExpandedState(int rowIndex)
        {
            var row = dataGridView.Rows[rowIndex] as DataGridViewActionRow;
            if (row == null)
                return;

            if (row.Expanded)
            {
                row.Cells[columnExpander.Index].Value = Properties.Resources.contracted_triangle;
                row.Cells[columnMessage.Index].Value = row.Action.GetTitle();
            }
            else
            {
                row.Cells[columnExpander.Index].Value = Properties.Resources.expanded_triangle;
                row.Cells[columnMessage.Index].Value = row.Action.GetDetails();
            }
            row.Expanded = !row.Expanded;
        }

        private void UpdateButtons()
        {
            tsmiDismissAll.Enabled = dataGridView.Rows.Cast<DataGridViewActionRow>().Any(row => row.Action.IsCompleted);
            tsmiDismissSelected.Enabled = dataGridView.SelectedRows.Cast<DataGridViewActionRow>().Any(row => row.Action.IsCompleted);
        }

        #region Control handlers

        private void row_DismissalRequested(DataGridViewActionRow row)
        {
            if (ConnectionsManager.History.Count > 0 &&
                (Program.RunInAutomatedTestMode ||
                 new ThreeButtonDialog(
                     new ThreeButtonDialog.Details(null, Messages.MESSAGEBOX_LOG_DELETE, Messages.MESSAGEBOX_LOG_DELETE_TITLE),
                     ThreeButtonDialog.ButtonYes,
                     ThreeButtonDialog.ButtonNo).ShowDialog(this) == DialogResult.Yes))
            {
                ConnectionsManager.History.Remove(row.Action);   
            }
        }

        private void row_GoToXenObjectRequested(IXenObject obj)
        {
            throw new NotImplementedException();
        }


        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.ColumnIndex != columnExpander.Index)
                return;

            ToggleExpandedState(e.RowIndex);
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            tsmiDismissSelected.Enabled = dataGridView.SelectedRows.Cast<DataGridViewActionRow>().Any(row => row.Action.IsCompleted);
        }


        private void toolStripSplitButtonDismiss_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            toolStripSplitButtonDismiss.DefaultItem = e.ClickedItem;
            toolStripSplitButtonDismiss.Text = toolStripSplitButtonDismiss.DefaultItem.Text;
        }

        private void tsmiDismissAll_Click(object sender, EventArgs e)
        {
            if (ConnectionsManager.History.Count > 0 &&
                (Program.RunInAutomatedTestMode ||
                 new ThreeButtonDialog(
                     new ThreeButtonDialog.Details(null, Messages.MESSAGEBOX_LOGS_DELETE, Messages.MESSAGEBOX_LOGS_DELETE_TITLE),
                     ThreeButtonDialog.ButtonYes,
                     ThreeButtonDialog.ButtonNo).ShowDialog(this) == DialogResult.Yes))
            {
                ConnectionsManager.History.RemoveAll(a => a.IsCompleted);
            }
        }

        private void tsmiDismissSelected_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Nested classes

        private class DataGridViewActionRow : DataGridViewRow
        {
            private DataGridViewImageCell expanderCell = new DataGridViewImageCell();
            private DataGridViewImageCell statusCell = new DataGridViewImageCell();
            private DataGridViewTextBoxCell messageCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell locationCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell dateCell = new DataGridViewTextBoxCell();
            private DataGridViewDropDownSplitButtonCell actionCell = new DataGridViewDropDownSplitButtonCell();

            public event Action<DataGridViewActionRow> DismissalRequested;
            public event Action<IXenObject> GoToXenObjectRequested;

            public ActionBase Action { get; private set; }
            public bool Expanded { get; set; }

            public DataGridViewActionRow(ActionBase action)
            {
                Action = action;
                MinimumHeight = DataGridViewDropDownSplitButtonCell.MIN_ROW_HEIGHT;
                Cells.AddRange(expanderCell, statusCell, messageCell, locationCell, dateCell, actionCell);
                RefreshSelf();
            }

            public void RefreshSelf()
            {
                var actionItems = GetActionItems(Action);
                actionCell.RefreshItems(actionItems.ToArray());

                statusCell.Value = Action.GetImage();

                if (Expanded)
                {
                    expanderCell.Value = Properties.Resources.expanded_triangle;
                    messageCell.Value = Action.GetDetails();
                }
                else
                {
                    expanderCell.Value = Properties.Resources.contracted_triangle;
                    messageCell.Value = Action.GetTitle();
                }
                locationCell.Value = Action.GetLocation();
                dateCell.Value = HelpersGUI.DateTimeToString(Action.Started.ToLocalTime(), Messages.DATEFORMAT_DMY_HM, true);
            }

            private List<ToolStripItem> GetActionItems(ActionBase action)
            {
                var items = new List<ToolStripItem>();

                if (!action.IsCompleted)
                {
                    var cancel = new ToolStripMenuItem(Messages.CANCEL);
                    cancel.Enabled = action.CanCancel;
                    cancel.Click += ToolStripMenuItemCancel_Click;
                    items.Add(cancel);
                }

                var obj = action.GetRelevantXenObject();
                if (obj != null)
                {
                    var goTo = new ToolStripMenuItem(Messages.HISTORYPAGE_GOTO);
                    goTo.Click += ToolStripMenuItemGoTo_Click;
                    items.Add(goTo);
                }

                if (action.IsCompleted)
                {
                    var dismiss = new ToolStripMenuItem(Messages.ALERT_DISMISS);
                    dismiss.Click += ToolStripMenuItemDismiss_Click;
                    items.Add(dismiss);
                }

                return items;
            }

            private void ToolStripMenuItemCancel_Click(object sender, EventArgs e)
            {
                Action.Cancel();
            }

            private void ToolStripMenuItemDismiss_Click(object sender, EventArgs e)
            {
                if (DismissalRequested != null)
                    DismissalRequested(this);
            }

            private void ToolStripMenuItemGoTo_Click(object sender, EventArgs e)
            {
                if (GoToXenObjectRequested != null)
                    GoToXenObjectRequested(Action.GetRelevantXenObject());
            }
        }

        private class CustomToolStripRenderer : ToolStripProfessionalRenderer
        {
            public CustomToolStripRenderer()
                : base(new CustomToolstripColourTable())
            {
                RoundedEdges = false;
            }
        }

        private class CustomToolstripColourTable : ProfessionalColorTable
        {
            public override Color ToolStripBorder
            {
                get { return Color.Gainsboro; }
            }
        }

        #endregion
    }
}
