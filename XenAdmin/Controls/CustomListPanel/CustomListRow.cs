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
using System.Windows;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using XenAdmin.TabPages;

namespace XenAdmin.Controls
{
    public class CustomListRow
    {
        private Color ForeColor_ = SystemColors.ControlText;
        private Color BackColor_ = SystemColors.Control;
        private Color BorderColor_ = SystemColors.ActiveBorder;
        private Pen BorderPen = null;
        private Pen ForePen = null;

        public Color SelectedBackColor_ = Color.Black;
        private Brush BackBrush = null;
        private Brush SelectedBackBrush = null;

        public bool ShowBorder = true;
        public bool Enabled = true;
        public bool DrawChildren = true;
        private bool _selected = false;
        public bool Selectable = true;
        public bool DrawGroupBox = true;
        public bool ShowOnlyHorizBorder = false;

        public int Indent = 13;  // left-indent to any child rows
        public float BorderWidth = 1.0f;
        public Padding Padding = new Padding(0, 3, 0, 3);

        public List<ToolStripMenuItem> MenuItems = new List<ToolStripMenuItem>();
        public ToolStripMenuItem DefaultMenuItem;

        public CustomListRow Parent;
        public CustomListPanel ParentPanel;

        public List<CustomListRow> Children = new List<CustomListRow>();
        public List<CustomListItem> Items = new List<CustomListItem>();

        /// <summary>
        /// Construct a header.
        /// </summary>
        public CustomListRow(object tag, Color back, Color fore, Color border, Font font)
        {
            this.BackColor = back;
            this.ForeColor = fore;
            this.BorderColor = border;
            this.SelectedBackColor = back;
            this.DrawGroupBox = false;
            this.ShowOnlyHorizBorder = true;

            CustomListItem name = new CustomListItem(tag, font, fore, new Padding(10, 3, 3, 3));

            AddItem(name);

            init();
        }

        /// <summary>
        /// Construct child rows.
        /// </summary>
        public CustomListRow(Color back, params CustomListItem[] items)
        {
            Selectable = true;
            BackColor = back;
            this.SelectedBackColor = back;
            ShowBorder = false;
            foreach (CustomListItem item in items)
            {
                AddItem(item);
            }

            init();
        }

        /// <summary>
        /// Construct fixed row.
        /// </summary>
        public CustomListRow(Color back, int height)
        {
            Selectable = true;
            BackColor = back;
            this.SelectedBackColor = back;
            ShowBorder = false;
            this.Padding = new Padding(0, 0, 0, height);

            init();
        }

        private void init()
        {
            RecalculateBorderPen();
            RecalculateForePen();
            RecalculateBackBrushes();
        }

        public Color ForeColor
        {
            get { return ForeColor_; }
            set
            {
                ForeColor_ = value;
                RecalculateForePen();
            }
        }

        public Color BackColor
        {
            get { return BackColor_; }
            set
            {
                BackColor_ = value;
                RecalculateBorderPen();
            }
        }

        public Color BorderColor
        {
            get { return BorderColor_; }
            set
            {
                BorderColor_ = value;
                RecalculateBorderPen();
            }
        }

        private void RecalculateBorderPen()
        {
            if (BorderPen != null)
                BorderPen.Dispose();
            BorderPen = new Pen(BorderColor, BorderWidth);
        }

        private void RecalculateForePen()
        {
            if (ForePen != null)
                ForePen.Dispose();
            ForePen = new Pen(ForeColor);
        }

        public Color SelectedBackColor
        {
            get { return SelectedBackColor_; }
            set
            {
                SelectedBackColor_ = value;
                RecalculateBackBrushes();
            }
        }

        private void RecalculateBackBrushes()
        {
            if (BackBrush != null)
                BackBrush.Dispose();
            if (SelectedBackBrush != null)
                SelectedBackBrush.Dispose();
            BackBrush = new SolidBrush(BackColor);
            SelectedBackBrush = new SolidBrush(SelectedBackColor);
        }

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                if (!value)
                    foreach (CustomListRow child in Children)
                        child.Selected = false;
            }
        }

        public int RowAndChildrenHeight
        {
            get
            {
                int height = Padding.Vertical + RowHeight;

                if (DrawChildren)
                {
                    foreach (CustomListRow child in Children)
                    {
                        height += child.RowAndChildrenHeight;
                    }
                }

                return height;
            }
        }

        public int RowHeight
        {
            get
            {
                int height = 0;
                int headWidth = HeaderWidth;
                for (int i = 0; i < Items.Count; i++)
                {
                    CustomListItem item = Items[i];
                    Size cSize = item.WrappedSize(headWidth);
                    if (cSize.Height > height)
                    {
                        height = cSize.Height;
                    }
                    if(ParentPanel.level1ColWidths != null && i < ParentPanel.level1ColWidths.Length)
                        headWidth -= ParentPanel.level1ColWidths[i];
                }
                return height;
            }
        }

        public int HeaderWidth
        {
            get
            {
                if (Parent != null)
                    return Parent.HeaderWidth - (Padding.Horizontal + Indent);
                else
                    return ParentPanel.Width - (ParentPanel.Padding.Horizontal + Padding.Horizontal);
            }
        }

        public void Dispose()
        {
            if (ForePen != null)
                ForePen.Dispose();
            if (BorderPen != null)
                BorderPen.Dispose();
            if (BackBrush != null)
                BackBrush.Dispose();
            if (SelectedBackBrush != null)
                SelectedBackBrush.Dispose();

            foreach (CustomListRow r in Children)
            {
                r.Dispose();
            }
        }

        public void DrawSelf(Graphics g, Rectangle bounds)
        {
            int rowHeight = RowHeight;
            int headWidth = HeaderWidth;

            using (Brush ParentBackBrush = new SolidBrush(ParentPanel.BackColor))
            {
                g.FillRectangle(ParentBackBrush, bounds);
            }

            int top = bounds.Top + Padding.Top;
            int left = bounds.Left + Padding.Left;

            for(int i = 0; i < Items.Count; i++)
            {
                CustomListItem item = Items[i];
                Size itemSize = item.PreferredSize;

                int itemTop = top;

                if ((item.Anchor & AnchorStyles.Bottom) > 0)
                {
                    itemTop += rowHeight - itemSize.Height;
                }
                else if ((item.Anchor & AnchorStyles.Top) == 0)
                {
                    itemTop += Convert.ToInt32(Math.Ceiling(Convert.ToDouble(rowHeight - itemSize.Height) / 2.0d));
                }
                
                Rectangle itemBounds;
                if (Parent != null && (i == Items.Count - 1 || Items[i+1].InCorrectColumn))
                {
                    int colWidth = 1;
                    if (ParentPanel.level1ColWidths.Length > i)
                    {
                        colWidth = ParentPanel.level1ColWidths[i];
                    }
                    itemBounds = new Rectangle(new Point(left, itemTop), item.WrappedSize(headWidth - (left - bounds.Left)));
                    left += colWidth;
                }
                else
                {
                    itemBounds = new Rectangle(left, itemTop, itemSize.Width, itemSize.Height);
                    left += itemSize.Width;
                }

                if (itemBounds.Right > bounds.Right - Padding.Right)
                    itemBounds.Width -= itemBounds.Right - (bounds.Right - Padding.Right);
                if (itemBounds.Bottom > bounds.Bottom - Padding.Bottom)
                    itemBounds.Height -= itemBounds.Bottom - (bounds.Bottom - Padding.Bottom);

                if (i > 0 && Selected && !string.IsNullOrEmpty(item.Tag.ToString()))
                {
                    if ((i < Items.Count - 1 && !Items[i + 1].InCorrectColumn))
                    {
                        g.FillRectangle(SystemBrushes.Highlight, new Rectangle(itemBounds.Left - 2, itemBounds.Top - 3, itemBounds.Width + Items[i + 1].PreferredSize.Width + 4, Items[i + 1].PreferredSize.Height + 2));
                    }
                    else if (item.InCorrectColumn)
                    {
                        g.FillRectangle(SystemBrushes.Highlight, new Rectangle(itemBounds.Left - 2, itemBounds.Top - 1, itemBounds.Width + 4, itemBounds.Height));
                    }
                }

                item.DrawSelf(g, itemBounds, Selected && i > 0 && !(i < Items.Count - 1 && !Items[i + 1].InCorrectColumn));
            }

            top += rowHeight + Padding.Bottom;

            if (DrawChildren)
            {
                foreach (CustomListRow child in Children)
                {
                    int childHeight = child.RowAndChildrenHeight;
                    Rectangle childBounds = new Rectangle(bounds.Left + Padding.Left + Indent, top, bounds.Width - (Padding.Horizontal + Indent), childHeight);
                    child.DrawSelf(g, childBounds);
                    top += childHeight;
                }
            }

            if (ShowBorder)
            {
                if (ShowOnlyHorizBorder)
                {
                    int w = 1;
                    if(ParentPanel.level1ColWidths.Length >= 2)
                        w = 450;
                    
                    using (Brush brush = new LinearGradientBrush(Point.Empty, new Point(w, 0), BorderColor, BackColor))
                    {
                        g.FillRectangle(brush, bounds.Left, bounds.Top + rowHeight, w, 1);
                    }
                }
                else
                {
                    g.DrawRectangle(BorderPen, bounds.Left, bounds.Top, headWidth, rowHeight);
                }

                if (DrawGroupBox)
                    g.DrawRectangle(BorderPen, bounds.Left, bounds.Top, headWidth, RowAndChildrenHeight);
            }
        }

        public void AddItem(CustomListItem item)
        {
            item.Row = this;
            this.Items.Add(item);
        }

        public void AddChild(CustomListRow row)
        {
            row.Parent = this;
            if (this.Children.Count == 0)
                row.Padding.Top = 10;
            row.ParentPanel = ParentPanel;
            Children.Add(row);
        }

        internal CustomListRow SelectChild(Point point, out Point rowRelativePoint)
        {
            int top = Padding.Vertical + RowHeight;

            foreach (CustomListRow child in Children)
            {
                int height = child.RowAndChildrenHeight;

                if (point.Y >= top && point.Y < top + height)
                {
                    int leftPos = point.X - Padding.Left - Indent;
                    rowRelativePoint = new Point(leftPos, point.Y - top);
                    if (leftPos < 0)  // off the left hand side of the subrow
                        return null;
                    return child;
                }

                top += height;
            }

            rowRelativePoint = new Point();
            return null;
        }

        private CustomListItem GetItemFromPosition(Point point, out Point itemRelativePoint)
        {
            int left = Padding.Left;
            for (int i = 0; i < ParentPanel.level1ColWidths.Length; i++)
            {
                if (i >= Items.Count)
                    continue;

                CustomListItem item = Items[i];

                if (item == null)
                    continue;

                int width = item.itemBorder.Horizontal + ParentPanel.level1ColWidths[i];

                if (point.X >= left && point.X < left + width)
                {
                    itemRelativePoint = new Point(point.X - left, point.Y - Padding.Top);
                    return item;
                }

                left += width;
            }

            itemRelativePoint = new Point();
            return null;
        }

        public void OnMouseClick(MouseEventArgs e, Point point)
        {
            if (point.Y > RowHeight + Padding.Vertical)
            {
                Point rowRelativePoint;
                CustomListRow row = SelectChild(point, out rowRelativePoint);
                if (row == null)
                    return;
                
                row.OnMouseClick(e, rowRelativePoint);               
            }
            else
            {
                Point itemRelativePoint;
                CustomListItem item = GetItemFromPosition(point, out itemRelativePoint);
                if (item == null)
                    return;

                item.OnMouseClick(e, itemRelativePoint);
            }
        }

        public void OnMouseDoubleClick(MouseEventArgs e, Point point)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (DefaultMenuItem != null)
                    DefaultMenuItem.PerformClick();
            }

            if (point.Y > RowHeight + Padding.Vertical)
            {
                Point rowRelativePoint;
                CustomListRow row = SelectChild(point, out rowRelativePoint);
                if (row == null)
                    return;

                row.OnMouseDoubleClick(e, rowRelativePoint);
            }
            else
            {
                Point itemRelativePoint;
                CustomListItem item = GetItemFromPosition(point, out itemRelativePoint);
                if (item == null)
                    return;

                item.OnMouseDoubleClick(e, itemRelativePoint);
            }
        }

        public void OnMouseDown(Point point)
        {
            if (point.Y > RowHeight + Padding.Vertical)
            {
                Point rowRelativePoint;
                CustomListRow row = SelectChild(point, out rowRelativePoint);
                if (row == null)
                {
                    ParentPanel.ClearSelection();
                    return;
                }

                row.OnMouseDown(rowRelativePoint);
            }
            else
            {
                if (!Selectable)
                    return;

                ParentPanel.ClearSelection();

                Selected = true;
                ParentPanel.SelectedRow = this;
                ParentPanel.Invalidate();
            }
        }

        /*
         * This code is almost identical to the code for tracking mouse movements
         * in the CustomGridRow.
         */

        private CustomListItem currentItem;
        private CustomListRow currentRow;

        public virtual void OnMouseEnter(Point point)
        {
            System.Diagnostics.Trace.Assert(currentItem == null && currentRow == null);

            if (point.Y > RowHeight + Padding.Vertical)
            {
                Point rowRelativePoint;
                currentRow = SelectChild(point, out rowRelativePoint);
                if (currentRow == null)
                    return;
                
                currentRow.OnMouseEnter(rowRelativePoint);
            }
            else
            {
                Point itemRelativePoint;
                currentItem = GetItemFromPosition(point, out itemRelativePoint);
                if (currentItem == null)
                    return;

                currentItem.OnMouseEnter(itemRelativePoint);
            }
        }

        public virtual void OnMouseMove(Point point)
        {
            System.Diagnostics.Trace.Assert(currentItem == null || currentRow == null);

            if (point.Y > RowHeight + Padding.Vertical)
            {
                if (currentItem != null)
                {
                    currentItem.OnMouseLeave();
                    currentItem = null;
                }

                Point rowRelativePoint;
                CustomListRow row = SelectChild(point, out rowRelativePoint);

                if (row != null && currentRow == row)
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
            else
            {
                if(currentRow != null)
                {
                    currentRow.OnMouseLeave();
                    currentRow = null;
                }
                
                Point itemRelativePoint;
                CustomListItem item = GetItemFromPosition(point, out itemRelativePoint);

                if (item != null && currentItem == item)
                {
                    currentItem.OnMouseMove(itemRelativePoint);
                    return;
                }

                if (currentItem != null)
                    currentItem.OnMouseLeave();

                currentItem = item;

                if (currentItem != null)
                    currentItem.OnMouseEnter(itemRelativePoint);
            }
        }

        public virtual void OnMouseLeave()
        {
            System.Diagnostics.Trace.Assert(currentItem == null || currentRow == null);

            if (currentItem != null)
            {
                currentItem.OnMouseLeave();
                currentItem = null;
            }

            if (currentRow != null)
            {
                currentRow.OnMouseLeave();
                currentRow = null;
            }
        }
    }
}
