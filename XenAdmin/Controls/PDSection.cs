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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using System.Linq;
using XenAdmin.Commands;


namespace XenAdmin.Controls
{
    public partial class PDSection : UserControl
    {
        #region Private fields

        /// <summary>
        /// Used to persist selection over a repaint, we set this when we clear the datagridview
        /// </summary>
        private string previousSelectionKey;

        private bool inLayout;

        /// <summary>
        /// Whether the mouse is over the chevron
        /// </summary>
        private bool chevronHot;

        #endregion

        #region Delegates and Events

        /// <summary>
        /// Event for when the datagridview received focus
        /// </summary>
        public event Action<PDSection> ContentReceivedFocus;

        /// <summary>
        /// Event for when the datagridview receives focus
        /// </summary>
        public event Action<PDSection> ContentChangedSelection;

        public event Action<PDSection> ExpandedChanged;

        #endregion

        public PDSection()
        {
            InitializeComponent();

            SectionTitle = Messages.PDSECTION_TITLE;
            IsExpanded = true;
            Collapse();
            MinimumSize = new Size(0, Height);

            dataGridViewEx1.LostFocus += dataGridViewEx1_LostFocus;
            dataGridViewEx1.GotFocus += dataGridViewEx1_GotFocus;

            if (!Application.RenderWithVisualStyles)
            {
                panel1.BackColor = SystemColors.Control;
                BackColor = SystemColors.ControlDark;
            }
        }

        #region Accessors

        [Browsable(false)]
        public bool DisableFocusEvent { private get; set; }

        [DefaultValue("Title")]
        [Browsable(true)]
        [Localizable(true)]
        public string SectionTitle
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        [Browsable(false)]
        public bool IsEmpty()
        {
            return dataGridViewEx1.Rows.Count == 0;
        }

        [DefaultValue(true)]
        [Browsable(false)]
        public bool IsExpanded { get; private set; }

        [Browsable(false)]
        public bool HasNoSelection()
        {
            return dataGridViewEx1.SelectedRows.Count <= 0;
        }

        [Browsable(false)]
        public Rectangle SelectedRowBounds
        {
            get
            {
                if (HasNoSelection())
                    return new Rectangle(0, 0, 0, 0);

                int x = dataGridViewEx1.Location.X;
                int y = dataGridViewEx1.RowDepth(dataGridViewEx1.SelectedRows[0].Index) + dataGridViewEx1.Location.Y;
                int w = dataGridViewEx1.Width;
                int h = dataGridViewEx1.SelectedRows[0].Height;
                return new Rectangle(x, y, w, h);
            }
        }

        [Browsable(false)]
        public bool ShowCellToolTips
        {
            get { return dataGridViewEx1.ShowCellToolTips; }
            set { dataGridViewEx1.ShowCellToolTips = value; }
        }

        #endregion

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            // For keyboard access
            if (DisableFocusEvent)
                return;

            if (!IsExpanded)
                Expand();
        }

        #region Control Event Handlers

        private void dataGridViewEx1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                RunCellCommandOrAction(dataGridViewEx1.Rows[e.RowIndex].Cells[e.ColumnIndex]);
        }

        private void dataGridViewEx1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                RunCellCommandOrAction(dataGridViewEx1.CurrentCell);
        }

        private void dataGridViewEx1_LostFocus(object sender, EventArgs e)
        {
            dataGridViewEx1.HideSelection = true;
        }

        private void dataGridViewEx1_GotFocus(object sender, EventArgs e)
        {
            dataGridViewEx1.HideSelection = false;
            ContentReceivedFocus?.Invoke(this);
        }

        private void dataGridViewEx1_SelectionChanged(object sender, EventArgs e)
        {
            if (inLayout)
                return;

            ContentChangedSelection?.Invoke(this);
        }

        private void dataGridViewEx1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            DataGridView.HitTestInfo i = dataGridViewEx1.HitTest(e.X, e.Y);

            if (i.RowIndex < 0 || i.RowIndex > dataGridViewEx1.RowCount)
                return;
            if (i.ColumnIndex < 0 || i.ColumnIndex > dataGridViewEx1.ColumnCount)
                return;

            dataGridViewEx1.Focus();
            var row = dataGridViewEx1.Rows[i.RowIndex];
            row.Selected = true;
            
            contextMenuStrip1.Items.Clear();
            contextMenuStrip1.Items.Add(copyToolStripMenuItem);

            if (row.Tag is ToolStripMenuItem[] menuItems && menuItems.Length > 0)
            {
                contextMenuStrip1.Items.Add(new ToolStripSeparator());
                contextMenuStrip1.Items.AddRange(menuItems.Cast<ToolStripItem>().ToArray());
            }
            contextMenuStrip1.Show(dataGridViewEx1, dataGridViewEx1.PointToClient(MousePosition));
        }

        
        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(dataGridViewEx1.SelectedRows[0].Cells[1].Value);
        }


        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (IsExpanded)
                Collapse();
            else
            {
                Expand();
                dataGridViewEx1.Focus();
            }
        }
        
        private void chevron_Click(object sender, EventArgs e)
        {
            if (IsExpanded)
                Collapse();
            else
            {
                Expand();
                dataGridViewEx1.Focus();
            }
        }

        private void chevron_MouseEnter(object sender, EventArgs e)
        {
            chevronHot = true;
            RefreshChevron();
        }

        private void chevron_MouseLeave(object sender, EventArgs e)
        {
            chevronHot = false;
            RefreshChevron();
        }
 
        #endregion

        #region Private Methods

        private void RunCellCommandOrAction(DataGridViewCell cell)
        {
            if (cell == null)
                return;

            if (cell.Tag is Command command)
                command.Run();
            else if (cell.Tag is Action action)
                action.Invoke();
        }

        private void RefreshHeight()
        {
            if (IsExpanded)
            {
                int newHeight = dataGridViewEx1.Rows.GetRowsHeight(DataGridViewElementStates.Visible);

                int actualWidth = dataGridViewEx1.Width - KeyColumn.Width;

                int preferredWidth = ValueColumn.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true) +
                                     ColumnNotes.GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);

                int horizontalScrollBarHeight = preferredWidth - 2 >= actualWidth
                    ? dataGridViewEx1.HorizontalScrollBarHeight
                    : 0;

                // 3px added so we have a border (one at top of control, one between title and grid, one at bottom)
                Height = panel1.Height + newHeight + horizontalScrollBarHeight + 3;
                // this correction is needed because the anchor property of the grid drags it to fill the space we want to be a border
                // when we go from contracted (2 borders) to expanded (3 borders)
            }
            else
            {
                Height = panel1.Height + 2;
            }
        }
        
        private void RefreshChevron()
        {
            if (IsExpanded)
            {
                chevron.Image = chevronHot ? Images.StaticImages.PDChevronUpOver : Images.StaticImages.PDChevronUp;
            }
            else
            {
                chevron.Image = chevronHot ? Images.StaticImages.PDChevronDownOver : Images.StaticImages.PDChevronDown;
            }
        }

        private void AddRow(DataGridViewCell keyCell, DataGridViewCell valueCell, DataGridViewCell noteCell, params ToolStripMenuItem[] contextMenuItems)
        {
            var r = new DataGridViewExRow();
            r.Cells.AddRange(keyCell, valueCell, noteCell ?? new DataGridViewTextBoxCell());
            r.Tag = contextMenuItems;

            dataGridViewEx1.Rows.Add(r);

            if (inLayout)
                return;

            RefreshHeight();
        }

        private static DataGridViewTextBoxCell CreateKeyCell(string key)
        {
            if (!string.IsNullOrEmpty(key))
                key += Messages.GENERAL_PAGE_KVP_SEPARATOR;
            var cell = new DataGridViewTextBoxCell { Value = key };
            return cell;
        }

        private void ToggleExpandedState(bool expand)
        {
            if (IsExpanded == expand)
                return;

            IsExpanded = expand;
            dataGridViewEx1.Visible = expand;
            RefreshHeight();
            RefreshChevron();

            if (ExpandedChanged != null)
                ExpandedChanged(this);
        }

        #endregion

        public void Collapse()
        {
            ToggleExpandedState(false);
        }

        public void Expand()
        {
            ToggleExpandedState(true);
        }

        public void AddEntry(string key, string value, params ToolStripMenuItem[] contextMenuItems)
        {
            var valueCell = new DataGridViewTextBoxCell { Value = value };
            AddRow(CreateKeyCell(key), valueCell, null, contextMenuItems);
        }

        public void AddEntry(string key, string value, Color fontColor, params ToolStripMenuItem[] contextMenuItems)
        {
            var valueCell = new DataGridViewTextBoxCell { Value = value };
            AddRow(CreateKeyCell(key), valueCell, null, contextMenuItems);
            valueCell.Style.ForeColor = fontColor;
        }

        public void AddEntry(string key, FolderListItem value, params ToolStripMenuItem[] contextMenuItems)
        {
            var valueCell = new FolderCell(value); // CA-33311
            AddRow(CreateKeyCell(key), valueCell, null, contextMenuItems);
        }

        internal void AddEntryLink(string key, string value, Command command, params ToolStripMenuItem[] contextMenuItems)
        {
            var valueCell = new DataGridViewLinkCell { Value = value, Tag = command };
            AddRow(CreateKeyCell(key), valueCell, null, contextMenuItems);
        }

        internal void AddEntryLink(string key, string value, Action action, params ToolStripMenuItem[] contextMenuItems)
        {
            var valueCell = new DataGridViewLinkCell { Value = value, Tag = action };
            AddRow(CreateKeyCell(key), valueCell, null, contextMenuItems);
        }

        internal void AddEntryWithNoteLink(string key, string value, string note, Action action, params ToolStripMenuItem[] contextMenuItems)
        {
            var valueCell = new DataGridViewTextBoxCell { Value = value };
            var noteCell = new DataGridViewLinkCell { Value = note, Tag = action };
            AddRow(CreateKeyCell(key), valueCell, noteCell, contextMenuItems);
        }

        public void UpdateEntryValueWithKey(string Key, string newValue, bool visible)
        {
            List<DataGridViewExRow> matchingRows = (from DataGridViewExRow row in dataGridViewEx1.Rows
                                                   where row.Cells[0].Value.ToString().Contains(Key)
                                                   select row).ToList();

            if (matchingRows.Count != 1 || matchingRows[0].Cells.Count < 2)
                return;

            matchingRows[0].Cells[1].Value = newValue;

            if (matchingRows[0].Visible != visible)
            {
                matchingRows[0].Visible = visible;
                RefreshHeight();
            }
        }

        internal void FixFirstColumnWidth(int width)
        {
            dataGridViewEx1.Columns[0].Width = width;
        }

        /// <summary>
        /// Clears the rows in the data grid view. If performing a repaint, you can attempt to restore the previous selection with a call to RestoreSelection()
        /// </summary>
        public void ClearData()
        {
            if (dataGridViewEx1.SelectedRows.Count > 0)
                previousSelectionKey = dataGridViewEx1.SelectedRows[0].Cells[0].Value.ToString();

            dataGridViewEx1.Rows.Clear();
        }

        /// <summary>
        /// Restores selection to the row stored from the last ClearData() call
        /// </summary>
        public void RestoreSelection()
        {
            if (string.IsNullOrEmpty(previousSelectionKey))
                return;

            foreach (DataGridViewRow r in dataGridViewEx1.Rows)
            {
                if (r.Cells[0].Value.ToString() == previousSelectionKey)
                {
                    r.Cells[0].Selected = true;
                    return;
                }
            }
        }

        public void PauseLayout()
        {
            inLayout = true;
        }

        public void StartLayout()
        {
            inLayout = false;
            RefreshHeight();
        }
    }
}
