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
using System.Threading;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAPI;
using System.Collections;

namespace XenAdmin.Controls.DataGridViewEx
{
    [ToolboxBitmap(typeof(DataGridView))]
    public partial class PoolHostDataGridViewOneCheckbox : CollapsingPoolHostDataGridView
    {
        public PoolHostDataGridViewOneCheckbox()
        {
            InitializeComponent();
        }

        public PoolHostDataGridViewOneCheckbox(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);

            if (e.RowIndex >= 0 && Rows[e.RowIndex].Tag is Host)
            {
                PoolHostDataGridViewOneCheckboxRow row = (PoolHostDataGridViewOneCheckboxRow)Rows[e.RowIndex];
                //Paintout checkbox - note: it's still there
                if (!row.CheckBoxVisible && e.ColumnIndex == 1)
                {
                    e.PaintBackground(e.ClipBounds, true);
                    e.Handled = true;
                }
                else if (row.HasPool && (e.ColumnIndex == 0 || e.ColumnIndex == 1))
                {
                    e.PaintBackground(e.ClipBounds, true);
                    e.Handled = true;
                }
                else if (!row.HasPool && e.ColumnIndex == 0)
                {
                    e.PaintBackground(e.ClipBounds, true);
                    e.Handled = true;
                }
            }
        }

        public override void CheckBoxChange(int rowIndex, int columnIndex)
        {
            if (columnIndex != 1 || !((DataGridViewExRow)Rows[rowIndex]).Enabled)
                return;

            Rows[rowIndex].Cells[columnIndex].Value = (int)(Rows[rowIndex].Cells[columnIndex].Value) == 0 ? 1 : 0;

            PoolHostDataGridViewOneCheckboxRow row = (PoolHostDataGridViewOneCheckboxRow)Rows[rowIndex];

            if (row.CheckBoxVisible && row.IsCheckable)
            {
                OnCheckBoxClicked();
            }
            else
            {
                //Make sure the invisible checked box is unclicked
                row.Checked = CheckState.Unchecked;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<PoolHostDataGridViewOneCheckboxRow> CheckedRows
        {
            get
            {
                List<PoolHostDataGridViewOneCheckboxRow> checkedRows = new List<PoolHostDataGridViewOneCheckboxRow>();
                foreach (PoolHostDataGridViewOneCheckboxRow row in Rows)
                {
                    if (row.Enabled && row.Checked == CheckState.Checked)
                        checkedRows.Add(row);
                }
                return checkedRows;
            }
            set
            {
                foreach (PoolHostDataGridViewOneCheckboxRow row in value)
                {
                    foreach (PoolHostDataGridViewOneCheckboxRow storedRow in Rows)
                    {
                        if (storedRow.Equals(row))
                        {
                            storedRow.Checked = CheckState.Checked;
                        }
                    }
                }
            }
        }

        public void SetCheckStateOfHostRowsContaining(IEnumerable<Host> hostsToCheck, CheckState checkState)
        {
            foreach (PoolHostDataGridViewOneCheckboxRow row in Rows)
            {
                if (!row.IsCheckable)
                    continue;

                PoolHostDataGridViewOneCheckboxRow currentRow = row;
                bool canCheck = row.IsAPoolRow
                                    ? hostsToCheck.Any(
                                        host => currentRow.UnderlyingPool.Equals(Helpers.GetPoolOfOne(host.Connection)))
                                    : row.IsAHostRow ? hostsToCheck.Contains(row.UnderlyingHost) : false;
                if (canCheck)
                    row.Checked = checkState;
            } 
        }

        /// <summary>
        /// Set a check state for all rows in the view
        /// </summary>
        public CheckState CheckStateForAllRows
        {
            set
            {
                foreach (PoolHostDataGridViewOneCheckboxRow row in Rows)
                {
                    row.Checked = value;
                } 
            }
        }
    }
}
