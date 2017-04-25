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
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace XenAdmin.Controls
{
    public partial class CustomListPanel : Panel
    {
        private readonly List<CustomListRow> Rows = new List<CustomListRow>();

        public CustomListRow SelectedRow;

        public int[] level1ColWidths;

        public bool InUpdate = false;

        public Graphics Graphics;

        public CustomListPanel()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            Graphics = CreateGraphics();
            Graphics.TextRenderingHint = XenAdmin.Core.Drawing.TextRenderingHint;
        }

        public void BeginUpdate()
        {
            InUpdate = true;
        }

        public void EndUpdate()
        {
            InUpdate = false;

            // We need OnPaint to be called, because that's where we're going to compute the size of this whole panel.
            // If the panel is too small, then that will never happen.
            // This used to work by fluke, but once we put the Search panel on the top level it broke.
            if (Height <= 1)
                Height = 2;
            if (Width <= 1)
                Width = 2;
            
            Refresh();
        }

        public void AddRow(CustomListRow row)
        {
            level1ColWidths = null;
            row.ParentPanel = this;
            Rows.Add(row);
            if (!InUpdate)
                Refresh();
        }

        public void AddChildRow(CustomListRow parent, CustomListRow child)
        {
            level1ColWidths = null;
            child.ParentPanel = this;
            parent.AddChild(child);
            if (!InUpdate)
                Refresh();
        }

        public void ClearRows()
        {
            foreach (CustomListRow row in Rows)
            {
                row.Dispose();
            }
            Rows.Clear();
        }

        [Browsable(true)]
        public EventHandler<ListPanelItemClickedEventArgs> ContextMenuRequest;

        #region Mouse Handling

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (level1ColWidths == null)
                level1ColWidths = level1Widths();

            Point rowRelativePoint;
            CustomListRow selectedRow = getRowFromPoint(e.Location, out rowRelativePoint);
            if (selectedRow == null)
                return;

            selectedRow.OnMouseDoubleClick(e, rowRelativePoint);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            //
            // CA-14088: Clicking the general panel makes it scroll to the top
            // 
            // Don't focus this control, focus its parent.  Focusing this control
            // causes it to scroll into view, therefore flick to top on first click
            //
            if(this.Parent != null)
                this.Parent.Select();

            base.OnMouseClick(e);
            if (level1ColWidths == null)
                level1ColWidths = level1Widths();

            Point rowRelativePoint;
            CustomListRow selectedRow = getRowFromPoint(e.Location, out rowRelativePoint);
            if (selectedRow == null)
                return;

            selectedRow.OnMouseClick(e, rowRelativePoint);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (level1ColWidths == null)
                level1ColWidths = level1Widths();

            Point rowRelativePoint;
            CustomListRow selectedRow = getRowFromPoint(e.Location, out rowRelativePoint);
            if (selectedRow == null)
                return;

            selectedRow.OnMouseDown(rowRelativePoint);
        }

        private CustomListRow currentRow;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (level1ColWidths == null)
                level1ColWidths = level1Widths();

            Point rowRelativePoint;
            CustomListRow row = getRowFromPoint(e.Location, out rowRelativePoint);

            if (row != null && row == currentRow)
            {
                currentRow.OnMouseMove(rowRelativePoint);
                return;
            }

            if (currentRow != null)
                currentRow.OnMouseLeave();

            currentRow = row;

            if (currentRow != null)
                currentRow.OnMouseEnter(rowRelativePoint);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (currentRow != null)
            {
                currentRow.OnMouseLeave();
                currentRow = null;
            }
        }

        #endregion

        private CustomListRow getRowFromPoint(Point point, out Point rowRelativePoint)
        {
            int top = this.Padding.Top;
            foreach (CustomListRow row in Rows)
            {
                int height = row.RowAndChildrenHeight;

                if (point.Y >= top && point.Y < top + height)
                {
                    rowRelativePoint = new Point(point.X, point.Y - top);
                    return row;
                }

                top += height;
            }

            rowRelativePoint = new Point();
            return null;
        }

        public void AddItemToRow(CustomListRow row, CustomListItem item)
        {
            row.AddItem(item);
            Refresh();
        }

        private int Level1ColWidthIndex(int col)
        {
            int width = 0;
            foreach (CustomListRow row in Rows)
                foreach (CustomListRow child in row.Children)
                    if (child.Items.Count > col && child.Items[col].InCorrectColumn)
                    {
                        int testWidth = child.Items[col].PreferredSize.Width;
                        if (testWidth > width)
                            width = testWidth;
                    }

            return width;
        }

        public int max_cols
        {
            get
            {
                int x = 0;
                foreach (CustomListRow row in Rows)
                {
                    foreach (CustomListRow child in row.Children)
                    {
                        if (child.Items.Count > x)
                        {
                            x = child.Items.Count;
                        }
                    }
                }
                return x;
            }
        }

        public int[] level1Widths()
        {
            int[] cols = new int[max_cols];
            for (int n = 0; n < cols.Length; n++)
            {
                cols[n] = Level1ColWidthIndex(n);
            }
            return cols;
        }

        public void ClearSelection()
        {
            if (SelectedRow != null)
            {
                SelectedRow.Selected = false;
                SelectedRow = null;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (level1ColWidths == null)
                level1ColWidths = level1Widths();

            using (SolidBrush backBrush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(backBrush, new Rectangle(0, 0, Width, Height));
            }

            int itemTop = this.Padding.Top;

            foreach (CustomListRow row in Rows)
            {
                int rowHeight = row.RowAndChildrenHeight;
                Rectangle bounds = new Rectangle(Padding.Left, itemTop, Width - Padding.Horizontal, rowHeight);
                row.DrawSelf(e.Graphics, bounds);
                itemTop += rowHeight;
            }

            if (!DesignMode)
            {
                if(this.Height != 1 + itemTop + this.Padding.Bottom)
                    this.Height = 1 + itemTop + this.Padding.Bottom;
            }
        }
    }

    public class ListPanelItemClickedEventArgs : EventArgs
    {
        public CustomListItem Item;
        public ListPanelItemClickedEventArgs(CustomListItem item)
        {
            Item = item;
        }
    }
}
