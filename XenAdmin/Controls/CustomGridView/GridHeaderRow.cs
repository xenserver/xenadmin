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
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace XenAdmin.Controls.CustomGridView
{
    public class GridHeaderRow : GridRow
    {
        public GridHeaderItem DefaultSortColumn;
        public List<String> Columns = new List<String>();

        public GridHeaderRow(int rowheight, Color backColor, Pen borderPen)
            : base(rowheight, backColor, borderPen)
        {
            OpaqueRef = "1";
        }

        public GridHeaderRow(int rowheight)
            : base(rowheight)
        {
            OpaqueRef = "1";
        }

        public int GetColumnWidth(string col)
        {
            if (Items.ContainsKey(col))
                return ((GridHeaderItem)Items[col]).Width;
            return 0;
        }

        public override GridView GridView
        {
            set
            {
                base.GridView = value;

                value.HeaderRow = this;
            }
        }

        public List<SortParams> CompareOrder = new List<SortParams>();

        public void UpdateCompareOrder()
        {
            List<SortParams> lst = new List<SortParams>();
            for (int i = 0; i < Items.Count; i++)
            {
                foreach (string col in Items.Keys)
                {
                    GridHeaderItem ghi = (GridHeaderItem)Items[col];
                    if (ghi.SortPriority == i)
                    {
                        lst.Add(new SortParams(col,ghi.Sort));
                        break;
                    }
                }
            }
            CompareOrder = lst;
        }

        public void UpdateCellSorts(List<SortParams> lst)
        {
            foreach (string col in Items.Keys)
            {
                GridHeaderItem ghi = (GridHeaderItem)Items[col];
                ghi.UnsetSort();
            }

            int i = 0;
            foreach (SortParams sp in lst)
            {
                if (!Items.ContainsKey(sp.Column))
                    continue;

                GridHeaderItem ghi = (GridHeaderItem)Items[sp.Column];
                ghi.SetSort(i++, sp.SortOrder);
            }

            UpdateCompareOrder();  // don't just set it to lst in case we had to leave some out
        }

        public override void AddItem(string col, GridItemBase item)
        {
            GridHeaderItem headerItem = item as GridHeaderItem;
            if (headerItem == null)
                return;

            if (headerItem.IsDefaultSortColumn)
                DefaultSortColumn = headerItem;

            Columns.Add(col);
            headerItem.ColumnName = col;
            base.AddItem(col, item);
        }

        public override void RemoveItem(string colname)
        {
            Columns.Remove(colname);
            base.RemoveItem(colname);
        }
    }

    public class SortParams
    {
        public readonly string Column;
        public readonly SortOrder SortOrder;

        public SortParams(string col, SortOrder sortOrder)
        {
            Column = col;
            SortOrder = sortOrder;
        }
    }
}
