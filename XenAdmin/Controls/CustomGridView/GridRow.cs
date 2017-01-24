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
using System.Collections;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace XenAdmin.Controls.CustomGridView
{
    public class GridRow : IComparable<GridRow>
    {
        public readonly Dictionary<String, GridItemBase> Items = new Dictionary<string, GridItemBase>();
        public readonly List<GridRow> Rows = new List<GridRow>();

        public static Image ExpandedImage = Properties.Resources.expanded_triangle;
        public static Image ShrunkenImage = Properties.Resources.contracted_triangle;

        public string OpaqueRef;
        public object Tag;

        public int Priority = -1; // -1 => dont care, 0 => highest

        public readonly Color BackColor;
        public Pen BorderPen;

        protected RowState state = RowState.Expanded;

        private readonly int rowHeight;

        private GridRow parentrow;
        private GridView gridview;

        public GridRow(int rowHeight)
            : this(rowHeight, SystemColors.Window, null)
        {
        }

        public GridRow(int rowHeight, Color BackColor, Pen Borderpen)
        {
            this.rowHeight = rowHeight;
            this.BackColor = BackColor;
            this.BorderPen = Borderpen;
            this.Tag = null;
        }

        // Set the row's gridview and add the row to the gridview's row list
        public virtual GridView GridView
        {
            get 
            { 
                return gridview; 
            }
            set
            {
                gridview = value;

                foreach (GridRow row in Rows)
                {
                    row.GridView = value;
                }
            }
        }

        public GridRow ParentRow
        {
            get 
            { 
                return parentrow; 
            }
            set
            {
                parentrow = value;
            }
        }

        public bool Expanded
        {
            get 
            { 
                return (State & RowState.Expanded) > 0; 
            }
            set
            {
                if (value == Expanded)
                    return;
                State ^= RowState.Expanded;
            }
        }

        public bool Selected
        {
            get
            {
                return (State & RowState.Selected) > 0;
            }
            set
            {
                if (value == Selected)
                    return;
                State ^= RowState.Selected;
            }
        }

        public RowState State
        {
            get { return state; }
            set { state = value; }
        }

        // We return the path as a string like "foo::bar::baz". It would be more theoretically
        // correct to return a List<string> but then we would have to define comparison functions
        // and hash functions for it. This has a very small chance of clashes, and is much simpler.
        public string Path
        {
            get
            {
                string parentPath = (ParentRow == null ? String.Empty : (ParentRow.Path + "::"));
                return (parentPath + OpaqueRef);
            }
        }

        public bool HasChildren
        {
            get
            {
                return Rows.Count > 0;
            }
        }

        public bool HasVisibleChildren
        {
            get
            {
                return (Expanded && HasChildren);
            }
        }

        // Iterate over all rows including child rows, in display order
        public IEnumerable<GridRow> RowsAndChildren
        {
            get
            {
                foreach (GridRow row in Rows)
                {
                    yield return row;
                    foreach (GridRow child in row.RowsAndChildren)
                        yield return child;
                }
            }
        }

        private int RowSeparation = 4;  // leave this between most rows
        private int GroupSeparation = 8;  // leave this instead after the last row in a group of children
        public int SpaceAfter
        {
            get
            {
                if (HasVisibleChildren)
                    return GroupSeparation;
                else
                    return RowSeparation;
            }
        }

        private int LeftOffset
        {
            get
            {
                return GridView.LeftOffset;
            }
        }

        private bool HasLeftExpander
        {
            get
            {
                return (HasChildren && GridView.HasLeftExpanders);
            }
        }

        public virtual void AddItem(string colname, GridItemBase item)
        {
            Items[colname] = item;
            item.Row = this;
        }

        public virtual void RemoveItem(string colname)
        {
            Items.Remove(colname);
        }

        public void AddRow(GridRow row)
        {
            Rows.Add(row);
            row.ParentRow = this;
        }

        public int RowHeight
        {
            get
            {
                return rowHeight;
            }
        }

        public int RowAndChildrenHeight
        {
            get
            {
                int height = RowHeight;

                if (HasVisibleChildren)
                {
                    height += RowSeparation;

                    int children = Rows.Count;
                    for(int i = 0; i < children; i++)
                    {
                        height += Rows[i].RowAndChildrenHeight;
                        if (i < children - 1)
                            height += Rows[i].SpaceAfter;
                    }
                }

                return height;
            }
        }

        public void OnPaint(RowPaintArgs e)
        {
            if (GridView == null || GridView.HeaderRow == null)
                return;

            if (HasLeftExpander)
            {
                // paint background
                using (Brush brush = new SolidBrush(BackColor))
                {
                    e.Graphics.FillRectangle(brush, e.Rectangle);
                }

                if (BorderPen != null)
                    e.Graphics.DrawRectangle(BorderPen, e.Rectangle);

                Image im = Expanded ? ExpandedImage : ShrunkenImage;
                e.Graphics.DrawImage(im, e.Rectangle.X + im.Width, e.Rectangle.Y + im.Height, im.Width, im.Height);
            }

            // paint this row
            int totalHeight = RowHeight;
            int left = e.Rectangle.Left + LeftOffset;
            int top = e.Rectangle.Top;

            foreach(string col in GridView.HeaderRow.Columns)
            {
                GridItemBase item = GetItem(col);
                int itemwidth = GridView.GetColumnWidth(col, ItemColumnSpan(item,col));

                if (left >= e.ClipRectangle.Left - itemwidth && left <= e.ClipRectangle.Right + itemwidth)
                {
                    item.OnPaint(new ItemPaintArgs(e.Graphics, new Rectangle(left, top, itemwidth, totalHeight),
                        Selected ? ItemState.Selected : ItemState.Normal));
                }

                left += GridView.GetColumnWidth(col, 1);
            }

            // paint subrows
            if (HasVisibleChildren)
            {
                top += RowHeight + RowSeparation;

                foreach (GridRow row in Rows)
                {
                    int rowheight = row.RowAndChildrenHeight;
                    if (top >= e.ClipRectangle.Top - rowheight && top <= e.ClipRectangle.Bottom + rowheight)
                        row.OnPaint(new RowPaintArgs(e.Graphics, e.ClipRectangle, new Rectangle(e.Rectangle.Left, top, e.Rectangle.Width, rowheight)));
                    top += rowheight + row.SpaceAfter;
                }
            }
        }

        internal int ItemColumnSpan(GridItemBase item, string col)
        {
            if(GridView == null || GridView.HeaderRow == null)
                return 1;
            int colindex = GridView.HeaderRow.Columns.IndexOf(col);
            int span = 1;
            for (int i = colindex + 1; i < colindex + item.RowSpan; i++)
            {
                if (GridView.HeaderRow.Columns.Count <= i)
                    break;
                string name = GridView.HeaderRow.Columns[i];
                if (!Items.ContainsKey(name))
                {
                    span++;
                    continue;
                }

                if (Items[name].Empty)
                    span++;
                else
                    break;
            }
            return span;
        }

        internal GridItemBase GetItem(string col)
        {
            if (!Items.ContainsKey(col))
                return new GridEmptyItem();
            return Items[col];
        }

        // A mouse click has not be intercepted by a click handler on a cell,
        // so represents a row-select. Some actions have to be done at mouse-down
        // time and some at mouse-up time (this is important when dragging, because
        // we have to have the right stuff selected when the drag starts).
        //
        internal void OnSelectMouseDown()
        {
            // If CTRL is down (with any other keys), toggle the current row state.
            Keys keys = Control.ModifierKeys;
            if ((keys & Keys.Control) != Keys.None)
            {
                Selected = !Selected;
                GridView.LastClickedRow = this;
                GridView.Refresh();
            }

            // Else if Shift is down, ask GridView to select the range.
            // NB Don't reset the LastClickedRow.
            else if ((keys & Keys.Shift) != Keys.None)
            {
                GridView.SelectRowRange(this);
                GridView.Refresh();
            }

            // Otherwise unselect all other rows and select this row.
            // But if the row is already selected, we wait until mouse-up time.
            else if (!Selected)
            {
                GridView.UnselectAllRows();
                Selected = true;
                GridView.LastClickedRow = this;
                GridView.Refresh();
            }
        }

        internal void OnSelectMouseUp()
        {
            // If CTRL or Shift is down, we've already handled the action in OnSelectMouseDown().
            if ((Control.ModifierKeys & (Keys.Control | Keys.Shift)) != Keys.None)
                return;

            // With no modifier keys, we select this row and unselect all other rows.
            // (If we mouse-downed in the same place, the row is already selected, so we
            // don't select it again).
            GridView.UnselectAllRowsExcept(this);
            GridView.LastClickedRow = this;
        }

        internal void OnMouseButtonAction(Point point, MouseButtonAction type)
        {
            if (HasLeftExpander)
            {
                Image image = Expanded ? ExpandedImage : ShrunkenImage;
                Size s = new Size(image.Width * 3, image.Height * 3);
                Rectangle r = new Rectangle(new Point(), s);

                if (r.Contains(point))
                {
                    if (type == MouseButtonAction.MouseDown)
                    {
                        Expanded = !Expanded;
                        GridView.Refresh();
                    }
                    else
                        return;
                }
            }

            if (point.Y < RowHeight) // user has clicked on the row
            {
                Point p;
                GridItemBase item = FindItemFromPoint(point, out p);
                if (item == null)
                    return;
                item.OnMouseButtonAction(p, type);
                return;
            }
            else // user has clicked on a sub row of the row
            {
                Point p;
                GridRow row = FindRowFromPoint(point, out p);
                if (row == null)
                    return;
                row.OnMouseButtonAction(p, type);
            }
        }

        private GridRow FindRowFromPoint(Point point, out Point p)
        {
            if (HasVisibleChildren)
            {
                int height = RowHeight + RowSeparation;
                foreach (GridRow row in Rows)
                {
                    if (point.Y >= height && point.Y < height + row.RowAndChildrenHeight)
                    {
                        p = new Point(point.X, point.Y - height);
                        return row;
                    }
                    height += row.RowAndChildrenHeight + row.SpaceAfter;
                }
            }
            p = new Point();
            return null;
        }

        internal GridItemBase FindItemFromPoint(Point point, out Point p)
        {
            int width = LeftOffset;
            int skipping = 0;
            foreach (string col in GridView.HeaderRow.Columns)
            {
                GridItemBase item = GetItem(col);
                if (item is GridEmptyItem && skipping > 0)
                {
                    skipping--;
                    continue;
                }
                skipping = item.RowSpan - 1;
                int itwidth = GridView.GetColumnWidth(col, item.RowSpan);
                if (point.X >= width && point.X < width + itwidth)
                {
                    p = new Point(point.X - width, point.Y);
                    return item;
                }
                width += itwidth;
            }
            p = new Point();
            return null;
        }

        internal Cursor Cursor
        {
            set
            {
                GridView.Cursor = value;
            }
        }

        /*
         * These properties are used to track mouse events over items of subrows.
         * Only one of these can be valid at once
         */
        private GridItemBase LastItem;
        private GridRow LastSubRow;

        /*
         * Asserts are here to ensure these events happen in order.
         * That order being OnEnter, OnMouseMove, OnLeave.
         */

        internal void OnEnter(Point point)
        {
            System.Diagnostics.Trace.Assert(LastSubRow == null);
            System.Diagnostics.Trace.Assert(LastItem == null);

            Point p;

            if (point.Y > RowHeight)
            {
                LastSubRow = FindRowFromPoint(point, out p);
                if(LastSubRow == null)
                    return;

                LastSubRow.OnEnter(p);
            }
            else
            {
                LastItem = FindItemFromPoint(point, out p);
                if (LastItem == null)
                    return;

                LastItem.OnEnter(p);
            }
        }

        internal void OnMouseMove(Point point)
        {

            if (HasLeftExpander)
            {
                Image image = Expanded ? ExpandedImage : ShrunkenImage;
                Rectangle r = new Rectangle(image.Width, image.Height, image.Width, image.Height);
                Cursor = r.Contains(point) ? Cursors.Hand : Cursors.Default;
            }

            // Make sure only one is valid
            System.Diagnostics.Trace.Assert(LastItem == null || LastSubRow == null);

            Point p;

            if (point.Y > RowHeight)
            {
                if (LastItem != null)
                {
                    LastItem.OnLeave();
                    LastItem = null;
                }

                GridRow row = FindRowFromPoint(point, out p);

                if (row != null && LastSubRow == row)
                {
                    LastSubRow.OnMouseMove(p);
                    return;
                }

                if (LastSubRow != null)
                    LastSubRow.OnLeave();

                LastSubRow = row;

                if (LastSubRow != null)
                    LastSubRow.OnEnter(p);
            }
            else
            {
                if (LastSubRow != null)
                {
                    LastSubRow.OnLeave();
                    LastSubRow = null;
                }

                GridItemBase item = FindItemFromPoint(point, out p);

                if (item != null && LastItem == item)
                {
                    LastItem.OnMouseMove(p);
                    return;
                }

                if (LastItem != null)
                    LastItem.OnLeave();

                LastItem = item;

                if (LastItem != null)
                    LastItem.OnEnter(p);
            }
        }

        internal void OnLeave()
        {
            // Make sure only one is valid
            System.Diagnostics.Trace.Assert(LastItem == null || LastSubRow == null);

            if (LastItem != null)
            {
                LastItem.OnLeave();
                LastItem = null;
            }

            if (LastSubRow != null)
            {
                LastSubRow.OnLeave();
                LastSubRow = null;
            }
        }

        public virtual int CompareTo(GridRow other)
        {
            if (Priority != other.Priority)
            {
                if (Priority == -1)
                    return 1;
                if (other.Priority == -1)
                    return -1;
                return Priority - other.Priority;
            }

            GridItemBase _1, _2;
            int val;

            if (GridView != null && GridView.HeaderRow != null)
            {
                foreach (SortParams sp in GridView.HeaderRow.CompareOrder)
                {
                    if (!Items.ContainsKey(sp.Column) || !other.Items.ContainsKey(sp.Column))
                        continue;
                    _1 = Items[sp.Column];
                    _2 = other.Items[sp.Column];
                    val = _1.CompareTo(_2);

                    if (val != 0)
                        return sp.SortOrder == SortOrder.Descending ? -val : val;
                }

                if (GridView.HeaderRow.DefaultSortColumn != null)
                {
                    string def = GridView.HeaderRow.DefaultSortColumn.ColumnName;
                    _1 = Items[def];
                    _2 = other.Items[def];
                    val = _1.CompareTo(_2);
                    if (val != 0)
                        return GridView.HeaderRow.DefaultSortColumn.Sort == SortOrder.Descending ? -val : val;
                }
            }

            if (OpaqueRef == other.OpaqueRef)
                return 0;

            return OpaqueRef.CompareTo(other.OpaqueRef); // fall back on opaque ref
        }

        internal void Sort()
        {
            Rows.Sort();

            if (Expanded)
            {
                foreach (GridRow row in Rows)
                {
                    row.Sort();
                }
            }
        }

        internal void SaveExpandedState(List<String> state)
        {
            if (!Expanded)
                state.Add(OpaqueRef);

            foreach (GridRow row in Rows)
                row.SaveExpandedState(state);
        }

        internal void RestoreExpandedState(List<string> expandedState)
        {
            if (Rows.Count > 0 && expandedState.Contains(OpaqueRef))
                Expanded = false;

            foreach (GridRow row in Rows)
                row.RestoreExpandedState(expandedState);
        }

        public override bool Equals(object obj)
        {
            if(!(obj is GridRow))
                return base.Equals(obj);
            return OpaqueRef == ((GridRow)obj).OpaqueRef;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    // The state the row should be drawn in
    [Flags]
    public enum RowState { None = 0, /* Visible = 1, Disabled = 2, */ Selected = 4, Expanded = 8 }  // others not implemented

    public class RowPaintArgs
    {
        public readonly Graphics Graphics;
        public readonly Rectangle ClipRectangle;
        public readonly Rectangle Rectangle;

        public RowPaintArgs(Graphics graphics, Rectangle cliprectangle, Rectangle rectangle)
        {
            Graphics = graphics;
            ClipRectangle = cliprectangle;
            Rectangle = rectangle;
        }
    }

    // OK, I admit it, it's just a typedef
    public class GridRowCollection : List<GridRow>
    {
    }
}
