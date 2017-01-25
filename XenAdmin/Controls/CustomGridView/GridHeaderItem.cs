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
    public class GridHeaderItem : GridStringItem
    {
        public const int ResizeGutter = 3;   // the space between the columns within which we resize the columns rather than dragging them

        private static readonly Image AscendingTriangle = Properties.Resources.ascending_triangle;
        private static readonly Image DescendingTriangle = Properties.Resources.descending_triangle;

        private static readonly Font SortFont = new Font(Program.DefaultFont.FontFamily, 7.0f);
        private static readonly Pen LinePen = Pens.LightSteelBlue;

        public readonly int MinimumWidth;

        private readonly SortOrder defaultSortOrder;
        public SortOrder Sort = SortOrder.None;

        public int SortPriority = -1;
        public bool IsDefaultSortColumn;

        public bool GreyOut;
        public bool Immovable;
        public bool UnSizable;

        private int width;
        private String columnName;

        public GridHeaderItem(HorizontalAlignment hAlign, VerticalAlignment vAlign, Brush foreBrush,
            Font font, String text, SortOrder defaultSortOrder, int width, int minwidth, Brush hotbrush)
            : base(text, hAlign, vAlign, false, false, foreBrush, font, hotbrush, font)
        {
            this.defaultSortOrder = defaultSortOrder;
            this.width = width;
            MinimumWidth = minwidth;
        }

        public String ColumnName
        {
            get
            {
                return columnName;
            }
            set
            {
                columnName = value;
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public override void OnPaint(ItemPaintArgs itemPaintArgs)
        {
            if (!string.IsNullOrEmpty(sortdata.ToString()))
            {
                base.OnPaint(itemPaintArgs);
                DrawGlyphs(itemPaintArgs);
            }
        }

        public void DrawGlyphs(ItemPaintArgs itemPaintArgs)
        {
            Graphics g = itemPaintArgs.Graphics;

            if (SortPriority > -1 && itemPaintArgs.Rectangle.Width - (AscendingTriangle.Width + 16) > 0)
            {
                // Draw the number showing the precedence of sorting in this column versus sorting in other columns
                g.DrawString((SortPriority + 1).ToString(), SortFont, foreBrush, itemPaintArgs.Rectangle.Right - (AscendingTriangle.Width + 15), itemPaintArgs.Rectangle.Top + ((itemPaintArgs.Rectangle.Height / 2) - 7));
            }
            if (Sort != SortOrder.None && itemPaintArgs.Rectangle.Width - (AscendingTriangle.Width + 5) > 0)
            {
                // Draw the triangle showing ascending/descending sort order
                DrawTriangle(g, Sort == SortOrder.Ascending ? AscendingTriangle : DescendingTriangle, itemPaintArgs);
            }

            DrawSideLine(g, itemPaintArgs, itemPaintArgs.Rectangle.Left - 1);
            DrawSideLine(g, itemPaintArgs, itemPaintArgs.Rectangle.Right - 1);
        }

        private void DrawSideLine(Graphics g, ItemPaintArgs itemPaintArgs, int x)
        {
            g.DrawLine(LinePen, x, itemPaintArgs.Rectangle.Top, x, itemPaintArgs.Rectangle.Bottom - 1);
        }

        private void DrawTriangle(Graphics g, Image triangle, ItemPaintArgs itemPaintArgs)
        {
            g.DrawImageUnscaled(triangle, itemPaintArgs.Rectangle.Right - triangle.Width - 5, itemPaintArgs.Rectangle.Top + ((itemPaintArgs.Rectangle.Height - triangle.Height) / 2), triangle.Width, triangle.Height);
        }

        public override void OnEnter(Point p)
        {
            if (!Immovable && !string.IsNullOrEmpty(sortdata.ToString()))
            {
                Hot = true;
                Row.GridView.Refresh();
            }
        }

        public override void OnMouseMove(Point p)
        {
            if ((!UnSizable && (p.X >= CurrentRectangle.Width - ResizeGutter && p.X <= CurrentRectangle.Width)) || (!PreviousUnsizable() && (p.X < ResizeGutter && p.X >= 0)))
                Row.Cursor = Cursors.SizeWE;
            else
                Row.Cursor = Cursors.Hand;
        }

        private bool PreviousUnsizable()
        {
            GridHeaderRow hr = (GridHeaderRow)Row;
            int tindex = hr.Columns.IndexOf(ColumnName);
            if (tindex <= 0)
                return true;
            string tother = hr.Columns[tindex - 1];
            GridItemBase prevItem;
            if (!hr.Items.TryGetValue(tother, out prevItem))
                return true;
            GridHeaderItem prevHeader = prevItem as GridHeaderItem;
            if (prevHeader == null)
                return true;
            return prevHeader.UnSizable;
        }

        public override void OnLeave()
        {
            if (!string.IsNullOrEmpty(sortdata.ToString()))
            {
                Row.Cursor = Cursors.Default;
                Hot = false;
                Row.GridView.Refresh();
            }
        }

        public override void OnClick(Point p)
        {
            if (!string.IsNullOrEmpty(sortdata.ToString()))
            {
                NextSort();
                Row.GridView.Sort();
                Row.GridView.Refresh();
            }
        }

        /// <summary>
        /// Changes the sort order of this column to the next value
        /// </summary>
        private void NextSort()
        {
            switch (Sort)
            {
                case SortOrder.None:
                    SetSort(defaultSortOrder);
                    break;
                case SortOrder.Ascending:
                    SetSort(defaultSortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.None);
                    break;
                case SortOrder.Descending:
                    SetSort(defaultSortOrder == SortOrder.Ascending ? SortOrder.None : SortOrder.Ascending);
                    break;
            }
        }

        private void SetSort(SortOrder sortOrder)
        {
            GridHeaderRow ghr = (GridHeaderRow)Row;
            int oldPriority = SortPriority;
            SortOrder oldSort = Sort;

            Sort = sortOrder;
            if (Sort == SortOrder.None)  // removing sorting from this column
            {
                SortPriority = -1;
                foreach (GridHeaderItem other in ghr.Items.Values)
                {
                    if (other != this && other.SortPriority > oldPriority)
                        other.SortPriority--;
                }
            }
            else if (oldSort == SortOrder.None)  // starting to apply sorting to this column
            {
                SortPriority = 0;
                foreach (GridHeaderItem other in ghr.Items.Values)
                {
                    if (other != this && other.SortPriority > -1)
                        other.SortPriority++;
                }
            }

            ghr.UpdateCompareOrder();
        }

        // When using this function, it is the caller's responsibility to
        // make sure all the columns have a consistent set of priorities
        internal void SetSort(int priority, SortOrder sortOrder)
        {
            SortPriority = priority;
            Sort = sortOrder;
        }

        // When using this function, it is the caller's responsibility to
        // make sure all the columns have a consistent set of priorities
        internal void UnsetSort()
        {
            SortPriority = -1;
            Sort = SortOrder.None;
        }
    }
}
