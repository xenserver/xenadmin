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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls.CustomDataGraph;
using XenAdmin.Core;
using XenCenterLib;

namespace XenAdmin.Dialogs
{
    public partial class GraphDetailsDialog : XenDialogBase
    {
        private const int COLOR_SQUARE_SIZE = 12;

        private readonly DesignedGraph _designedGraph;
        private readonly GraphList _graphList;
        private readonly bool _isNew;
        private bool _updating;
        private List<DataSourceItem> _dataSourceItems = new List<DataSourceItem>();

        public GraphDetailsDialog(GraphList graphList, DesignedGraph designedGraph = null)
            : base(graphList.XenObject?.Connection)
        {
            InitializeComponent();
            tableLayoutPanel1.Visible = false;

            _graphList = graphList;

            if (designedGraph == null)
            {
                _isNew = true;
                _designedGraph = new DesignedGraph
                {
                    // Generate an unique suggested name for the graph
                    DisplayName = Helpers.MakeUniqueName(Messages.GRAPH_NAME, graphList.DisplayNames)
                };
                base.Text = Messages.GRAPHS_NEW_TITLE;
            }
            else
            {
                _designedGraph = new DesignedGraph(designedGraph);
                base.Text = string.Format(Messages.GRAPHS_EDIT_TITLE, _designedGraph.DisplayName);
            }

            ActiveControl = GraphNameTextBox;
            GraphNameTextBox.Text = _designedGraph.DisplayName;
            EnableControlsAfterCheckChanged();
            EnableControlsAfterSelectionChanged();
        }

        private void LoadDataSources()
        {
            if (_graphList.XenObject == null)
                return;

            tableLayoutPanel1.Visible = true;
            searchTextBox.Enabled = false;

            var action = new GetDataSourcesAction(_graphList.XenObject);
            action.Completed += getDataSourcesAction_Completed;
            action.RunAsync();
        }

        private void PopulateDataGridView()
        {
            try
            {
                _updating = true;
                dataGridView.SuspendLayout();
                dataGridView.Rows.Clear();

                var rowList = new List<DataSourceGridViewRow>();

                foreach (var dataSourceItem in _dataSourceItems)
                {
                    if (!toolStripMenuItemHidden.Checked && dataSourceItem.Hidden)
                        continue;

                    if (!toolStripMenuItemDisabled.Checked && !dataSourceItem.Enabled)
                        continue;

                    if (!searchTextBox.Matches(dataSourceItem.ToString()))
                        continue;

                    var displayOnGraph = _designedGraph.DataSourceItems.Contains(dataSourceItem);
                    rowList.Add(new DataSourceGridViewRow(dataSourceItem, displayOnGraph));
                }


                dataGridView.Rows.AddRange(rowList.Cast<DataGridViewRow>().ToArray());
                dataGridView.Sort(dataGridView.Columns[ColumnDisplayOnGraph.Index], ListSortDirection.Ascending);

                if (dataGridView.Rows.Count > 0)
                    dataGridView.Rows[0].Cells[ColumnDisplayOnGraph.Index].Selected = true;
            }
            finally
            {
                dataGridView.ResumeLayout();
                _updating = false;
                EnableControlsAfterCheckChanged();
                EnableControlsAfterSelectionChanged();
            }
        }

        private void EnableControlsAfterCheckChanged()
        {
            var checkedRowCount = dataGridView.Rows.Cast<DataSourceGridViewRow>().Count(row => row.IsChecked);
            buttonClearAll.Enabled = checkedRowCount > 0;
            SaveButton.Enabled = checkedRowCount > 0;
        }

        private void EnableControlsAfterSelectionChanged()
        {
            if (_updating)
                return;

            var selectedRows = dataGridView.SelectedRows.Cast<DataSourceGridViewRow>().ToList();
            buttonEnable.Enabled = selectedRows.Count == 1 && selectedRows.TrueForAll(r => !r.Recording);
        }

        private void ClearAll()
        {
            try
            {
                foreach (var row in dataGridView.Rows)
                {
                    if (!(row is DataSourceGridViewRow dsRow))
                        continue;

                    dsRow.ClearCheck();
                    _designedGraph.DataSourceItems.Remove(dsRow.Dsi);
                }
            }
            finally
            {
                EnableControlsAfterCheckChanged();
            }
        }

        private void EnableDataSource()
        {
            if (_graphList.XenObject?.Connection == null || dataGridView.SelectedRows.Count != 1)
                return;

            if (!(dataGridView.SelectedRows[0] is DataSourceGridViewRow row))
                return;
            
            var dataSource = row.Dsi.DataSource;
            if (dataSource == null)
                return;

            buttonEnable.Enabled = false;
            var action = new EnableDataSourceAction(_graphList.XenObject, dataSource, row.Dsi.ToString());

            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee)
            {
                ShowTryAgainMessage = false,
                ShowException = false
            })
            {
                dialog.ShowDialog(this);
            }

            EnableControlsAfterSelectionChanged();

            if (action.Succeeded && action is EnableDataSourceAction enableAction && enableAction.DataSources != null)
            {
                var updated = enableAction.DataSources.FirstOrDefault(d => d.name_label == dataSource.name_label);
                row.Recording = updated != null && updated.enabled;
            }
        }


        #region Event handlers

        private void getDataSourcesAction_Completed(ActionBase sender)
        {
            if (!(sender is GetDataSourcesAction action) || !action.Succeeded)
                return;

            Program.Invoke(this, () =>
            {
                tableLayoutPanel1.Visible = false;
                _dataSourceItems = DataSourceItemList.BuildList(action.XenObject, action.DataSources);
                PopulateDataGridView();
                searchTextBox.Enabled = true;
            });
        }

        private void datasourcesGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == ColumnColour.Index && 0 <= e.RowIndex && e.RowIndex <= dataGridView.RowCount -1)
            {   
                var rect = dataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                e.PaintBackground(rect, true);

                rect.X += 3;
                rect.Y += 5;
                
                rect.Width = COLOR_SQUARE_SIZE;
                rect.Height = COLOR_SQUARE_SIZE;

                using (var brush = new SolidBrush((Color)e.Value))
                    e.Graphics.FillRectangle(brush, rect);

                e.Graphics.DrawRectangle(SystemPens.Window, rect);
                
                using (var pen = new Pen(SystemColors.ControlDark, 1.5f))
                    e.Graphics.DrawRectangle(pen, Rectangle.Inflate(rect, 1, 1));

                e.Handled = true;
            }
        }

        private void datasourcesGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex > dataGridView.RowCount - 1)
                return;

            if (!(dataGridView.Rows[e.RowIndex] is DataSourceGridViewRow row))
                return;

            if (e.ColumnIndex == ColumnDisplayOnGraph.Index)
            {
                if (row.IsChecked)
                {
                    row.ClearCheck();
                    _designedGraph.DataSourceItems.Remove(row.Dsi);
                }
                else
                {
                    row.Check();
                    _designedGraph.DataSourceItems.Add(row.Dsi);
                }

                EnableControlsAfterCheckChanged();
            }
            else if (e.ColumnIndex == ColumnColour.Index)
            {
                using (var cd = new ColorDialog
                {
                    Color = row.Color,
                    AllowFullOpen = true,
                    AnyColor = true,
                    FullOpen = true,
                    SolidColorOnly = true
                })
                {
                    if (cd.ShowDialog() == DialogResult.OK)
                        row.Color = cd.Color;
                }
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableControlsAfterSelectionChanged();
        }

        private void dataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            var row1 = dataGridView.Rows[e.RowIndex1] as DataSourceGridViewRow;
            var row2 = dataGridView.Rows[e.RowIndex2] as DataSourceGridViewRow;

            int result;

            if (row1 == null && row2 == null)
                result = 0;
            else if (row1 == null)
                result = 1;
            else if (row2 == null)
                result = -1;
            else
            {
                result = row1.CompareByCell(row2, e.Column.Index);

                if (result == 0)
                {
                    for (var i = 0; i < dataGridView.ColumnCount; i++)
                    {
                        if (i == e.Column.Index)
                            continue;

                        result = row1.CompareByCell(row2, i);
                        if (result == 0)
                            continue;

                        //the columns other than the clicked one should always be ascending
                        if (dataGridView.SortOrder == SortOrder.Descending)
                            result = -result;

                        break;
                    }
                }
            }

            e.SortResult = result;
            e.Handled = true;
        }


        private void GraphDetailsDialog_Load(object sender, EventArgs e)
        {
            LoadDataSources();
        }


        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }


        private void toolStripMenuItemDisabled_CheckedChanged(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }

        private void toolStripMenuItemHidden_CheckedChanged(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            ClearAll();
        }
        
        private void buttonEnable_Click(object sender, EventArgs e)
        {
            EnableDataSource();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (_graphList.XenObject == null)
                return;

            _designedGraph.DisplayName = GraphNameTextBox.Text;
            
            if (_isNew)
                _graphList.AddGraph(_designedGraph);
            else
                _graphList.ReplaceGraphAt(_graphList.SelectedGraphIndex, _designedGraph);

            var dataSourceItems = new List<DataSourceItem>();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (!(row is DataSourceGridViewRow dsRow))
                    continue;

                if (dsRow.Dsi.ColorChanged)
                    Palette.SetCustomColor(Palette.GetUuid(dsRow.Dsi.DataSource.name_label, _graphList.XenObject), dsRow.Dsi.Color);
                dataSourceItems.Add(dsRow.Dsi);
            }
            _graphList.SaveGraphs(dataSourceItems);
        }

        #endregion


        #region Nested classes

        private sealed class DataSourceGridViewRow : DataGridViewRow
        {
            private readonly DataGridViewCheckBoxCell _checkBoxCell = new DataGridViewCheckBoxCell();
            private readonly DataGridViewTextBoxCell _dataSourceCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _nameDescription = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _typeCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _enabledCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _colorCell = new DataGridViewTextBoxCell();

            public DataSourceGridViewRow(DataSourceItem dataSourceItem, bool displayOnGraph)
            {
                Dsi = dataSourceItem;

                _checkBoxCell.Value = displayOnGraph;
                _dataSourceCell.Value = Dsi.ToString();
                _nameDescription.Value = Dsi.DataSource.name_description;
                _typeCell.Value = Dsi.Category.ToStringI18N();
                _enabledCell.Value = Dsi.Enabled.ToYesNoStringI18n();
                _colorCell.Value = Dsi.Color;

                Cells.AddRange(_checkBoxCell, _dataSourceCell, _nameDescription, _typeCell, _enabledCell, _colorCell);

                if (Dsi.Hidden)
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        ForeColor = SystemColors.GrayText,
                        SelectionForeColor = SystemColors.GrayText,
                        SelectionBackColor = SystemColors.ControlLight
                    };
            }

            internal DataSourceItem Dsi { get; }

            internal Color Color
            {
                get => (Color)_colorCell.Value;
                set
                {
                    Dsi.ColorChanged = Dsi.Color != value;
                    Dsi.Color = value;
                    _colorCell.Value = value;
                }
            }

            internal bool IsChecked => (bool)_checkBoxCell.Value;

            internal bool Recording
            {
                get =>  Dsi.Enabled;
                set
                {
                    Dsi.Enabled = value;
                    _enabledCell.Value = value.ToYesNoStringI18n();
                }
            }

            internal void Check()
            {
                _checkBoxCell.Value = true;
            }
            
            internal void ClearCheck()
            {
                _checkBoxCell.Value = false;
            }

            public int CompareByCell(DataSourceGridViewRow other, int index)
            {
                if (other == null)
                    return -1;

                //In .NET false precedes true, but we want the checked rows to appear first
                if (index == _checkBoxCell.ColumnIndex)
                    return -IsChecked.CompareTo(other.IsChecked);

                if (_checkBoxCell.ColumnIndex < index && index < _colorCell.ColumnIndex)
                    return StringUtility.NaturalCompare(Cells[index].Value.ToString(),
                        other.Cells[index].Value.ToString());

                return 0;
            }
        }

        #endregion
    }
}
