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
    public abstract class GridArrayItem : GridItemBase
    {
        protected readonly GridItemBase[] items;

        public GridArrayItem(GridItemBase[] items, bool clickSelectsRow)
            : base(false, 1, clickSelectsRow, null, null)
        {
            this.items = items;
        }

        // NB public abstract void OnPaint(ItemPaintArgs itemPaintArgs) still abstract from base class

        // Which subitem the mouse is in, and where it is in it.
        // May return null if we're in the padding between items.
        protected abstract GridItemBase CurrentItem(Point point, out Point subItemPoint);

        public override GridRow Row
        {
            set
            {
                base.Row = value;
                foreach (GridItemBase item in items)
                    item.Row = value;
            }
        }

        public override void OnMouseButtonAction(Point point, MouseButtonAction type)
        {
            Point subItemPoint;
            GridItemBase subItem = CurrentItem(point, out subItemPoint);
            if (subItem != null)
                subItem.OnMouseButtonAction(subItemPoint, type);
            else
                base.OnMouseButtonAction(point, type);
        }

        protected GridItemBase mouseTrackedItem;

        public override void OnEnter(Point point)
        {
            if (mouseTrackedItem != null)
                OnLeave();

            Point subItemPoint;
            mouseTrackedItem = CurrentItem(point, out subItemPoint);

            if (mouseTrackedItem != null)
                mouseTrackedItem.OnEnter(subItemPoint);
        }

        public override void OnMouseMove(Point point)
        {
            Point subItemPoint;
            GridItemBase subItem = CurrentItem(point, out subItemPoint);

            if (mouseTrackedItem != subItem)
            {
                if (mouseTrackedItem != null)
                    mouseTrackedItem.OnLeave();

                mouseTrackedItem = subItem;
                
                if (subItem != null)
                    subItem.OnEnter(subItemPoint);
            }

            if (mouseTrackedItem != null)
                mouseTrackedItem.OnMouseMove(subItemPoint);
        }

        public override void OnLeave()
        {
            if (mouseTrackedItem == null)
                return;

            mouseTrackedItem.OnLeave();
            mouseTrackedItem = null;
        }

        public override int CompareTo(GridItemBase other)
        {
            GridVerticalArrayItem otherArrayItem = other as GridVerticalArrayItem;
            if (otherArrayItem == null)
                return -1;

            for (int i = 0; i < items.Length; i++)
            {
                if (i >= otherArrayItem.items.Length)
                    return 1;

                int comp = items[i].CompareTo(otherArrayItem.items[i]);
                if (comp != 0)
                    return comp;
            }

            return items.Length == otherArrayItem.items.Length ? 0 : -1;
        }
    }

    public class GridVerticalArrayItem : GridArrayItem
    {
        private const int Padding = 1;
        private bool RegularSpacing;
        private int ItemHeight;

        public GridVerticalArrayItem(GridItemBase[] items, bool clickSelectsRow)
            : this(items, -1, clickSelectsRow)
        {
        }

        public GridVerticalArrayItem(GridItemBase[] items, int itemfixedheight, bool clickSelectsRow)
            : base(items, clickSelectsRow)
        {
            RegularSpacing = itemfixedheight != -1;
            ItemHeight = itemfixedheight;
        }

        /// <summary>
        /// Gets the total width of all the contents of the grid item. For this type of item
        /// it will be the width of the longest of the consisting items.
        /// </summary>
        /// <param name="column">The column this item belongs to</param>
        internal override int GetGridItemWidth(string column)
        {
            int maxWidth = 0;

            foreach (GridItemBase item in items)
            {
                int itemWidth = item.GetGridItemWidth(column);
                if (itemWidth > maxWidth)
                    maxWidth = itemWidth;
            }

            return maxWidth;
        }

        public override void OnPaint(ItemPaintArgs itemPaintArgs)
        {
            if (items.Length == 0)
                return;
            Rectangle rect = itemPaintArgs.Rectangle;

            // Padding at top, bottom and between each item.
            // If this changes, CurrentItem() needs to change.
            rect.Y += Padding;

            if (RegularSpacing)
                rect.Height = ItemHeight;
            else
            {
                rect.Height -= Padding * items.Length;
                rect.Height /= items.Length;
            }

            foreach (GridItemBase item in items)
            {
                item.OnPaint(new ItemPaintArgs(itemPaintArgs.Graphics, rect, itemPaintArgs.State));
                rect.Y += rect.Height + Padding;
            }
        }

        // Needs to be coordinated with OnPaint().
        protected override GridItemBase CurrentItem(Point point, out Point subItemPoint)
        {
            if (items.Length == 0)
            {
                subItemPoint = new Point(0, 0);
                return null;
            }

            int subItemHeight;
            if (RegularSpacing)
                subItemHeight = ItemHeight;
            else
                subItemHeight = (Row.RowHeight - (Padding * (items.Length + 1))) / items.Length;  // from OnPaint()

            int offset = 0;
            foreach (GridItemBase item in items)
            {
                // The padding above an item: return null
                if (point.Y < offset + Padding)
                {
                    subItemPoint = new Point(point.X, point.Y - offset);
                    return null;
                }
                offset += Padding;

                // The item itself
                if (point.Y < offset + subItemHeight)
                {
                    subItemPoint = new Point(point.X, point.Y - offset);
                    return item;
                }
                offset += subItemHeight;
            }

            // The padding below the last item: return null
            subItemPoint = new Point(point.X, point.Y - offset);
            return null;
        }
    }

    public class GridHorizontalArrayItem : GridArrayItem
    {
        private const int IndentDistance = 25;
        private const int MinimumWidth = 75;
        private const int Padding = 3;

        private int indent;
        private int[] widths;
        private Rectangle lastPaintRectangle;

        // There should be one fewer width than items
        public GridHorizontalArrayItem(int indent, GridItemBase[] items, int[] widths, bool clickSelectsRow)
            : base(items, clickSelectsRow)
        {
            this.indent = indent;
            this.widths = widths;
        }

        /// <summary>
        /// Gets the total width of all the contents of the grid item. For this type of item
        /// it will be the sum of the widths of the consisting items plus the indentation and paddings.
        /// </summary>
        /// <param name="column">The column this item belongs to</param>
        internal override int GetGridItemWidth(string column)
        {
            int itemWidth = indent * IndentDistance;
            for (int i = 0; i < widths.Length; i++)
                itemWidth += widths[i] + Padding;
            
            //items are one more than widths so the last one has index widths.Length
            itemWidth += items[widths.Length].GetGridItemWidth(column);

            if (itemWidth > MinimumWidth)
                return itemWidth;

            return MinimumWidth;
        }

        public override void OnPaint(ItemPaintArgs itemPaintArgs)
        {
            lastPaintRectangle = itemPaintArgs.Rectangle;

            if (items.Length == 0)
                return;
            Rectangle rect = itemPaintArgs.Rectangle;

            int ind = indent * IndentDistance;
            if (rect.Width - ind < MinimumWidth)  // not enough room to indent fully: do as much as we can
                ind = Math.Max(rect.Width - MinimumWidth, 0);
            rect.X += ind;
            rect.Width -= ind;

            for (int i = 0; i < items.Length; ++i)
            {
                items[i].OnPaint(new ItemPaintArgs(itemPaintArgs.Graphics, rect, itemPaintArgs.State));

                if (i < items.Length - 1)
                {
                    ind = widths[i] + Padding;
                    rect.X += ind;
                    rect.Width -= ind;
                }
            }
        }

        // Needs to be coordinated with OnPaint().
        protected override GridItemBase CurrentItem(Point point, out Point subItemPoint)
        {
            if (items.Length == 0)
            {
                subItemPoint = new Point(0, 0);
                return null;
            }

            // The padding before the first item: return null
            int offset = indent * IndentDistance;
            if (lastPaintRectangle.Width - offset < MinimumWidth)  // not enough room to indent fully: do as much as we can
                offset = Math.Max(lastPaintRectangle.Width - MinimumWidth, 0);
            if (point.X < offset)
            {
                subItemPoint = new Point(point.X, point.Y);
                return null;
            }

            for (int i = 0; i < items.Length; ++i)
            {
                // The item itself
                if (i == items.Length - 1 || point.X < offset + widths[i])
                {
                    subItemPoint = new Point(point.X - offset, point.Y);
                    return items[i];
                }
                offset += widths[i];

                // The padding right of an item: return null
                if (point.X < offset + Padding)
                {
                    subItemPoint = new Point(point.X - offset, point.Y);
                    return null;
                }
                offset += Padding;
            }

            // Can't get here, but the compiler isn't smart enough to know that
            subItemPoint = new Point(0, 0);
            return null;
        }
    }
}
