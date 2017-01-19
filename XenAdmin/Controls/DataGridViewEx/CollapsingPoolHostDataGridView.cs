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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAPI;

namespace XenAdmin.Controls.DataGridViewEx
{
    public partial class CollapsingPoolHostDataGridView : DataGridViewEx
    {
        public event EventHandler CheckBoxClicked;

        protected ListSortDirection direction;
        private DataGridViewColumn lastSortedColumn;
        protected int columnToBeSortedIndex;

        #region Constructors

        public CollapsingPoolHostDataGridView()
        {
            InitializeComponent();
            RegisterEvents();
        }

        public CollapsingPoolHostDataGridView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            RegisterEvents();
        }

        #endregion

        public bool Updating { get; set; }

        private const int expansionColumnIndex = 0;

        protected override void OnCellContentClick(DataGridViewCellEventArgs e)
        {
            base.OnCellContentClick(e);

            if (e.ColumnIndex == expansionColumnIndex)
                ExpandCollapseClicked(e.RowIndex);
            else
                CheckBoxChange(e.RowIndex, e.ColumnIndex);
        }

        protected void OnCheckBoxClicked()
        {
            if (CheckBoxClicked != null)
                CheckBoxClicked(this, EventArgs.Empty);
        }

        public virtual void CheckBoxChange(int rowIndex, int columnIndex)
        {}

        /// <summary>
        /// Hook in order that sorting may be added for additional columns rather than those
        /// provided by the base class
        /// </summary>
        protected virtual void SortAdditionalColumns() { }

        /// <summary>
        /// Sort the rows but then remove and read the rows that should be expandable to be placed back under
        /// the parent row as they may get moved out of place
        /// </summary>
        /// <param name="comparer">Implementation if IComparer used to sort the rows prior to tree rebuild</param>
        protected void SortAndRebuildTree(IComparer comparer)
        {
            Updating = true;
            try
            {
                if (comparer == null)
                    throw new ArgumentNullException("comparer", "Comparator must not be null");

                Sort(comparer);

                var poolRows = (from CollapsingPoolHostDataGridViewRow r in Rows where r.IsAPoolRow select r).ToList();

                foreach (CollapsingPoolHostDataGridViewRow poolRow in poolRows)
                {
                    //Select the rows that contain hosts represented by the pool row
                    List<Host> hosts = poolRow.UnderlyingPool.Connection.Cache.Hosts.ToList();

                    List<CollapsingPoolHostDataGridViewRow> poolHostRows = (from CollapsingPoolHostDataGridViewRow r in Rows
                                                              where r.IsAHostRow && hosts.Contains(r.UnderlyingHost)
                                                              select r).ToList();

                    poolHostRows.ForEach(r => Rows.RemoveAt(r.Index));

                    var rowstoInsert = new Queue<CollapsingPoolHostDataGridViewRow>();

                    foreach (CollapsingPoolHostDataGridViewRow unRow in poolHostRows.Where(row => row.IsAHostRow))
                    {
                        unRow.ToPooledHostRow();
                        rowstoInsert.Enqueue(unRow);
                    }

                    Rows.InsertRange(poolRow.Index + 1, rowstoInsert.ToArray());
                }
            }
            finally
            {
                Updating = false;
            }
        }

        /// <summary>
        /// Sort and rebuild tree using a default IComparer object.
        /// </summary>
        public void SortAndRebuildTree()
        {
            SortAndRebuildTree(new CollapsingPoolHostDataGridViewRowDefaultSorter());
        }

        private void RegisterEvents()
        {
            ColumnHeaderMouseClick += sortColumnHeader_Click;
        }

        private void sortColumnHeader_Click(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (Rows.Count < 1)
                return;

            columnToBeSortedIndex = e.ColumnIndex;
            if (Columns[columnToBeSortedIndex].SortMode == DataGridViewColumnSortMode.NotSortable)
                return;

            CollapsingPoolHostDataGridViewRow firstRow = Rows[0] as CollapsingPoolHostDataGridViewRow;
            if (firstRow == null)
                return;

            DetermineSortDirection(e);

            if (columnToBeSortedIndex == firstRow.NameCellIndex)
            {
                SortAndRebuildTree(new CollapsingPoolHostDataGridViewRowDefaultSorter(direction));
            }

            SortAdditionalColumns();

            Columns[columnToBeSortedIndex].HeaderCell.SortGlyphDirection =
                direction == ListSortDirection.Ascending
                    ? SortOrder.Ascending
                    : SortOrder.Descending;
        }

        private void DetermineSortDirection(DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn newColumn = Columns[e.ColumnIndex];
            DataGridViewColumn oldColumn = lastSortedColumn;

            //Never been sorted
            if (oldColumn == null)
            {
                direction = ListSortDirection.Ascending;
                lastSortedColumn = newColumn;
                return;
            }

            if (oldColumn == newColumn && direction == ListSortDirection.Ascending)
            {
                direction = ListSortDirection.Descending;
                lastSortedColumn = newColumn;
                return;
            }

            if (oldColumn == newColumn && direction == ListSortDirection.Descending)
            {
                direction = ListSortDirection.Ascending;
                lastSortedColumn = newColumn;
                return;
            }

            //Different column picked than last time
            direction = ListSortDirection.Ascending;
            oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
            lastSortedColumn = newColumn;
        }

        public void ExpandCollapseClicked(int rowIndex)
        {
            var poolRow = (CollapsingPoolHostDataGridViewRow)Rows[rowIndex];

            if (poolRow.UnderlyingPool != null)
            {
                for (int i = rowIndex + 1; i < Rows.Count; i++)
                {
                    var row = (CollapsingPoolHostDataGridViewRow)Rows[i];

                    if (row.IsCheckable)
                        break;

                    row.Visible = !row.Visible;

                    if (row.Visible)
                        poolRow.SetCollapseIcon();
                    else
                        poolRow.SetExpandIcon();
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (CurrentCell != null && CurrentCell.ColumnIndex == expansionColumnIndex)
            {
                if (e.KeyCode == Keys.Space)
                {
                    ExpandCollapseClicked(CurrentCell.RowIndex);
                    e.Handled = true;
                }
            }
            base.OnKeyDown(e);
        }
    }
}
