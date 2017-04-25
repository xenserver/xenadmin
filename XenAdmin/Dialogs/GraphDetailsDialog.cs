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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls.CustomDataGraph;
using XenAdmin.Core;


namespace XenAdmin.Dialogs
{
    public partial class GraphDetailsDialog : XenDialogBase
    {
        private const int DisplayOnGraphColumnIndex = 0;
        private const int NameColumnIndex = 1;
        private const int TypeColumnIndex = 2;
        private const int ColorColumnIndex = 3;
        private const int ColorSquareSize = 12;

        private static readonly int[] SortableColumnIndexes = new int[] {DisplayOnGraphColumnIndex, NameColumnIndex, TypeColumnIndex};

        private DesignedGraph designedGraph;
        private GraphList graphList;
        private bool isNew;
        private List<DataSourceItem> _dataSources = new List<DataSourceItem>();

        public GraphDetailsDialog(): this(null, null)
        {
        }

        public GraphDetailsDialog(GraphList graphList, DesignedGraph designedGraph)
        {
            InitializeComponent();
            tableLayoutPanel1.Visible = false;

            this.graphList = graphList;
            isNew = (designedGraph == null);
            if (isNew)
            {
                this.designedGraph = new DesignedGraph();
                // Generate an unique suggested name for the graph
                if (graphList != null)
                    this.designedGraph.DisplayName = Helpers.MakeUniqueName(Messages.GRAPH_NAME, graphList.DisplayNames);
                base.Text = Messages.GRAPHS_NEW_TITLE;
            }
            else
            {
                this.designedGraph = new DesignedGraph(designedGraph);
                base.Text = string.Format(Messages.GRAPHS_DETAILS_TITLE, this.designedGraph.DisplayName);
                SaveButton.Text = Messages.OK;
            }
            ActiveControl = GraphNameTextBox;
            GraphNameTextBox.Text = this.designedGraph.DisplayName;
            EnableControls();
        }

        void getDataSorucesAction_Completed(ActionBase sender)
        {
            Program.Invoke(this, delegate
            {
                tableLayoutPanel1.Visible = false;
                GetDataSourcesAction action = sender as GetDataSourcesAction;
                if (action != null)
                {
                    _dataSources = DataSourceItemList.BuildList(action.IXenObject, action.DataSources);
                    PopulateDataGridView();
                }
                searchTextBox.Enabled = true;
                EnableControls();
            });
        }

        private void PopulateDataGridView()
        {
            try
            {
                dataGridView.SuspendLayout();
                dataGridView.Rows.Clear();
                
                foreach (DataSourceItem dataSourceItem in _dataSources)
                {
                    if (!searchTextBox.Matches(dataSourceItem.ToString()))
                        continue;

                    bool displayOnGraph = designedGraph.DataSources.Contains(dataSourceItem);
                    dataGridView.Rows.Add(new DataSourceGridViewRow(dataSourceItem, displayOnGraph));
                }

                dataGridView.Sort(dataGridView.Columns[DisplayOnGraphColumnIndex], ListSortDirection.Ascending);
                if (dataGridView.Rows.Count > 0)
                {
                    dataGridView.Rows[0].Cells[DisplayOnGraphColumnIndex].Selected = true;
                }
            }
            finally
            {
                dataGridView.ResumeLayout();
            }
        }

        private void datasourcesGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if ((e.ColumnIndex == ColorColumnIndex) && (e.RowIndex > -1))
            {   
                Rectangle rect = dataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                e.PaintBackground(rect, true);

                rect.X += 3;
                rect.Y += 5;
                
                rect.Width = ColorSquareSize;
                rect.Height = ColorSquareSize;

                using (var brush = new SolidBrush((Color)e.Value))
                    e.Graphics.FillRectangle(brush, rect);

                e.Graphics.DrawRectangle(SystemPens.Window, rect);
                
                using (var pen = new Pen(SystemColors.ControlDark, 1.5f))
                    e.Graphics.DrawRectangle(pen, Rectangle.Inflate(rect, 1, 1));

                e.Handled = true;
            }
        }

        private void EnableControls()
        {
            int selectedRows = dataGridView.Rows.Cast<DataGridViewRow>().Count(row =>
                {
                    var boolCell = row.Cells[DisplayOnGraphColumnIndex] as DataGridViewCheckBoxCell;
                    return boolCell != null && (bool)boolCell.Value;
                });
            
            bool canEnable = selectedRows > 0;
            SaveButton.Enabled = canEnable;
            ClearAllButton.Enabled = canEnable;
            clearAllToolStripMenuItem.Enabled = canEnable;
        }

        private void datasourcesGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                switch (e.ColumnIndex)
                {
                    case DisplayOnGraphColumnIndex:
                    case NameColumnIndex:
                    case TypeColumnIndex:
                        var boolCell =
                            dataGridView.Rows[e.RowIndex].Cells[DisplayOnGraphColumnIndex] as DataGridViewCheckBoxCell;
                        if (boolCell != null)
                        {
                            bool value = (bool)boolCell.Value;
                            boolCell.Value = !value;

                            DataSourceItem dsi = ((DataSourceGridViewRow)dataGridView.Rows[e.RowIndex]).Dsi;
                            if (dsi != null)
                            {                             
                                if (!value)
                                {
                                    designedGraph.DataSources.Add(dsi);
                                }
                                else
                                {
                                    designedGraph.DataSources.Remove(dsi);
                                }
                            }
                        }
                        EnableControls();
                        break;
                    case ColorColumnIndex:
                        var colorCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewTextBoxCell;
                        if (colorCell != null)
                        {
                            ColorDialog cd = new ColorDialog
                            {
                                Color = (Color)colorCell.Value,
                                AllowFullOpen = true,
                                AnyColor = true,
                                FullOpen = true,
                                SolidColorOnly = true
                            };
                            if (cd.ShowDialog() != DialogResult.OK)
                                return;
                            colorCell.Value = cd.Color;

                            DataSourceItem dsi = ((DataSourceGridViewRow)dataGridView.Rows[e.RowIndex]).Dsi;
                            if (dsi != null)
                            {
                                dsi.Color = cd.Color;
                                dsi.ColorChanged = true;
                            }
                        }
                        break;
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (graphList == null || graphList.XenObject == null)
                return;

            designedGraph.DisplayName = GraphNameTextBox.Text;
            if (isNew)
            {
                graphList.AddGraph(designedGraph);
            }
            else
            {
                graphList.ReplaceGraphAt(graphList.SelectedGraphIndex, designedGraph);
            }

            List<DataSourceItem> dataSources = new List<DataSourceItem>();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                DataSourceItem dsi = ((DataSourceGridViewRow)row).Dsi;
                if (dsi.ColorChanged)
                    Palette.SetCustomColor(Palette.GetUuid(dsi.DataSource.name_label, graphList.XenObject), dsi.Color);
                dataSources.Add(dsi);
            }
            graphList.SaveGraphs(dataSources);
        }

        private int SortCompare(DataGridViewRow row1, DataGridViewRow row2, int[] orderByColumnIndexes,
            IEnumerable<int> alwaysAscendingOrderColumnIndexes, SortOrder currentSortOrder)
        {
            int result = 0;
            if (orderByColumnIndexes.Length > 0)
            {
                int columnIndex = orderByColumnIndexes[0];
                if (row1.Cells[columnIndex].ValueType.Equals(typeof(bool)))
                {
                    bool value1 = (bool)row1.Cells[columnIndex].Value;
                    bool value2 = (bool)row2.Cells[columnIndex].Value;
                    result = value1 ^ value2 ? value1 ? -1 : 1 : 0;
                }
                else
                {
                    result = StringUtility.NaturalCompare(row1.Cells[columnIndex].Value.ToString(),
                                            row2.Cells[columnIndex].Value.ToString());
                }

                if (result == 0)
                {
                    if (orderByColumnIndexes.Length > 1)
                    {
                        result = SortCompare(row1, row2, orderByColumnIndexes.Where((val, idx) => idx != 0).ToArray(),
                                             alwaysAscendingOrderColumnIndexes, currentSortOrder);
                    }
                }
                else if (currentSortOrder == SortOrder.Descending && alwaysAscendingOrderColumnIndexes.Contains(columnIndex))
                {
                    result = -result;
                }
            }
            return result;
        }

        private void dataGridView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (!SortableColumnIndexes.Contains(e.Column.Index)) return;

            int[] orderByColumnIndexes;

            if (e.Column.Index == DisplayOnGraphColumnIndex)
                orderByColumnIndexes = new int[] { DisplayOnGraphColumnIndex, NameColumnIndex, TypeColumnIndex };
            else if (e.Column.Index == NameColumnIndex)
                orderByColumnIndexes = new int[] { NameColumnIndex, DisplayOnGraphColumnIndex, TypeColumnIndex };
            else if (e.Column.Index == TypeColumnIndex)
                orderByColumnIndexes = new int[] { TypeColumnIndex, DisplayOnGraphColumnIndex, NameColumnIndex };
            else
                return;

            e.SortResult = SortCompare(dataGridView.Rows[e.RowIndex1], dataGridView.Rows[e.RowIndex2],
                                       orderByColumnIndexes,
                                       SortableColumnIndexes.Where((val, idx) => idx != e.Column.Index),
                                       dataGridView.SortOrder);
            e.Handled = true;
        }

        private class DataSourceGridViewRow : DataGridViewRow
        {
            private DataSourceItem dsi;
            internal DataSourceItem Dsi { get { return dsi; } }

            public DataSourceGridViewRow(DataSourceItem dataSourceItem, bool displayOnGraph)
            {
                this.dsi = dataSourceItem;
                var displayOnGraphCell = new DataGridViewCheckBoxCell { Value = displayOnGraph };
                var datasourceCell = new DataGridViewTextBoxCell { Value = dsi.ToString() };
                var typeCell = new DataGridViewTextBoxCell { Value = dsi.DataType.ToStringI18N() };
                var colourCell = new DataGridViewTextBoxCell { Value = dsi.Color };
                Cells.AddRange(displayOnGraphCell, datasourceCell, typeCell, colourCell);
            }
        }

        private void ClearAll()
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    var boolCell = row.Cells[DisplayOnGraphColumnIndex] as DataGridViewCheckBoxCell;
                    if (boolCell != null)
                    {
                        boolCell.Value = false;
                        DataSourceItem dsi = ((DataSourceGridViewRow)row).Dsi;
                        if (dsi != null)
                        {
                            designedGraph.DataSources.Remove(dsi);
                        }
                    }
                }
            }
            finally
            {
                EnableControls();
            }            
        }

        private void ClearAllButton_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void dataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStrip.Show(dataGridView, e.X, e.Y);
        }

        private void GraphDetailsDialog_Load(object sender, EventArgs e)
        {
            if (graphList != null)
            {
                tableLayoutPanel1.Visible = true;
                searchTextBox.Enabled = false;
                graphList.LoadDataSources(getDataSorucesAction_Completed);
            }
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }
    }
}
