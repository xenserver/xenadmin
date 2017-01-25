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

namespace XenAdmin.Controls.CustomGridView
{
    public abstract class GridItemBase : IComparable<GridItemBase>
    {
        private readonly bool empty;
        private readonly int rowSpan;
        protected readonly bool clickSelectsRow;
        protected readonly EventHandler onClickDelegate;
        protected readonly EventHandler onDoubleClickDelegate;
        protected readonly EventHandler onRightClickDelegate;

        private GridRow row;

        public GridItemBase(bool empty, int rowSpan, bool clickSelectsRow, EventHandler onClickDelegate, EventHandler onDoubleClickDelegate)
            : this(empty, rowSpan, clickSelectsRow, onClickDelegate, onDoubleClickDelegate, null)
        {
        }

        public GridItemBase(bool empty, int rowSpan, bool clickSelectsRow, EventHandler onClickDelegate, EventHandler onDoubleClickDelegate, EventHandler onRightClickDelegate)
        {
            this.empty = empty;
            this.rowSpan = rowSpan;
            this.clickSelectsRow = clickSelectsRow;
            this.onClickDelegate = onClickDelegate;
            this.onDoubleClickDelegate = onDoubleClickDelegate;
            this.onRightClickDelegate = onRightClickDelegate;
        }

        public virtual bool Empty
        {
            get
            {
                return empty;
            }
        }

        public virtual int RowSpan
        {
            get
            {
                return rowSpan;
            }
        }

        public virtual GridRow Row
        {
            get
            {
                return row;
            }
            set
            {
                row = value;
            }
        }

        /// <summary>
        /// Gets the total width of all the contents of the grid item.
        /// </summary>
        /// <param name="column">The column this item belongs to</param>
        internal virtual int GetGridItemWidth(string column)
        {
            return 0;
        }

        public abstract void OnPaint(ItemPaintArgs itemPaintArgs);

        public virtual void OnMouseButtonAction(Point point, MouseButtonAction type)
        {
            if (type == MouseButtonAction.DoubleClick)
                OnDoubleClick(point);
            else if (type == MouseButtonAction.SingleClick)
                OnClick(point);
            else if (type == MouseButtonAction.StartDrag)
                OnStartDrag(point);
            else if (type == MouseButtonAction.SingleRightClick)
                OnRightClick(point);
            else
                OnMouseDown(point);
        }

        public virtual void OnMouseDown(Point point)
        {
            if (clickSelectsRow && onClickDelegate == null && Row != null)
                Row.OnSelectMouseDown();
        }

        public virtual void OnClick(Point point)
        {
            // onClickDelegate takes priority
            if (onClickDelegate != null)
            {
                onClickDelegate(this, EventArgs.Empty);
            }

            else if (Row == null)
                return;

            // If clickSelectsRow is true, we pass it back up to the row for the row-select thing
            else if (clickSelectsRow)
                Row.OnSelectMouseUp();

            // Otherwise we unselect all rows
            else
                Row.GridView.UnselectAllRows();
        }

        public virtual void OnRightClick(Point point)
        {
            if (Row != null)
            {
                if (!Row.Selected && clickSelectsRow)
                {
                    // select the row if it isn't selected
                    Row.OnSelectMouseDown();
                    Row.OnSelectMouseUp();
                }

                if (onRightClickDelegate != null)
                {
                    onRightClickDelegate(this, EventArgs.Empty);
                }
            }
        }

        public virtual void OnDoubleClick(Point point)
        {
            if (onDoubleClickDelegate != null)
                onDoubleClickDelegate(this, EventArgs.Empty);
        }

        public virtual void OnStartDrag(Point point)
        {
            if (clickSelectsRow && onClickDelegate == null && Row != null)
                Row.GridView.StartDragDrop();
        }

        public virtual void OnMouseMove(Point point)
        {
            // Don't do anything, and don't require sub classes to do anything.
        }

        public virtual void OnLeave()
        {
            // Don't do anything, and don't require sub classes to do anything.
        }

        public virtual void OnEnter(Point point)
        {
            // Don't do anything, and don't require sub classes to do anything.
        }

        #region IComparable<GridItem> Members

        public virtual int CompareTo(GridItemBase other)
        {
            return 0;
        }

        #endregion
    }

    // The state the item should be drawn in
    [Flags]
    public enum ItemState { Normal = 0, Selected = 1 }

    public class ItemPaintArgs
    {
        public readonly Graphics Graphics;
        public readonly ItemState State;
        public readonly Rectangle Rectangle;

        public ItemPaintArgs(Graphics graphics, Rectangle rectangle, ItemState itemstate)
        {
            Graphics = graphics;
            Rectangle = rectangle;
            State = itemstate;
        }
    }

    public enum VerticalAlignment { Top, /*Bottom,*/ Middle }  // Bottom currently not implemented
}
