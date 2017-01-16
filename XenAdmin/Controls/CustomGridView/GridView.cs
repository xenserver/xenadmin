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
using System.Runtime.InteropServices;
using System.Drawing.Text;
using XenAdmin.Core;
using XenAdmin.XenSearch;

namespace XenAdmin.Controls.CustomGridView
{
    public enum MouseButtonAction
    {
        SingleClick, DoubleClick, MouseDown, StartDrag, SingleRightClick
    }

    public partial class GridView : Panel
    {
        public List<GridRow> Rows = new List<GridRow>();
        // If currently painting the refresh call will set the PaintingRequired flag which will cause the panel to repaint after
        public bool Painting = false;
        public bool PaintingRequired = false;

        // This draws to the control
        private Graphics Graphics;
        // This draws to the BackBuffer
        private Graphics BufferGraphics;
        // Stores the image while being drawn
        private Bitmap BackBuffer;

        /// <summary>
        /// The width of the column containing the expander triangle, if required.
        /// </summary>
        private bool hasLeftExpanders = true;
        public bool HasLeftExpanders
        {
            get { return hasLeftExpanders; }
            set { hasLeftExpanders = value; }
        }
        private const int LEFT_OFFSET = 20;
        public int LeftOffset
        {
            get
            {
                return HasLeftExpanders ? LEFT_OFFSET : 0;
            }
        }

        // Row selection
        protected string lastClickedRowPath = null;
        public GridRow LastClickedRow
        {
            set
            {
                lastClickedRowPath = (value == null ? null : value.Path);
            }
        }
        Point lastClickedPoint = new Point(0, 0);

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

        public GridView()
        {
            InitBuffer();
            InitializeComponent();
        }

        // The top row in the list, NEVER set this! <- no need to dispose explicitly as will be disposed when the contents of Rows is disposed
        public GridHeaderRow HeaderRow;

        private bool hold_cursor_change = false;
        private Cursor new_cursor = null;
        public override Cursor Cursor
        {
            set
            {
                if (hold_cursor_change)
                    new_cursor = value;
                else
                    base.Cursor = value;
            }
        }

        /// <summary>
        /// Set the size of the back buffer to the size of the control
        /// </summary>
        private void InitBuffer()
        {
            Graphics = this.CreateGraphics();

            Rectangle disp = RealRectangle;
            BackBuffer = new Bitmap(disp.Width > 0 ? disp.Width : 1, disp.Height > 0 ? disp.Height : 1);
            BufferGraphics = Graphics.FromImage(BackBuffer);
            BufferGraphics.TextRenderingHint = Graphics.TextRenderingHint;
        }

        /// <summary>
        /// Update all values in the grid
        /// </summary>
        public override void Refresh()
        {
            //if (Painting)
            //{
            // we are currently painting, set a flag and return
            // the painting will occur when the current paint has finished
            // the idea is that if we get loads of calls to this then we only repaint at most once after each paint
            // this is why i dont think we should have a lock as they would wait then paint all afterwards
            //    PaintingRequired = true;
            //}
            //else
            //{
            // we are painting
            //Painting = true;
            Draw(RealRectangle);
            Painting = false; // slight chance of a race occuring but if we end up painting 3 times instead of just 2 it wont be the world
            //    if (PaintingRequired)
            //    {
            //        // we need to paint again
            //        PaintingRequired = false;
            //        Refresh();
            //    }
            //}
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            //OnPaint(null);
        }

        /// <summary>
        /// Gets the currently displayed rectangle of the control (ie includes how far we have scrolled)
        /// </summary>
        protected Rectangle RealRectangle
        {
            get
            {
                Rectangle r = ClientRectangle;
                r.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);
                return r;
            }
        }

        // refresh on scroll
        protected override void OnScroll(ScrollEventArgs se)
        {
            Refresh();
        }

        protected override void OnResize(EventArgs eventargs)
        {
            // ignore this size changed
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            // ignore this size changed
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            // use this one instead
            base.OnResize(e); // do size change <= calling on resize here may seem barking but it appears to be the only combination that works...
            DisposeBuffer(); // reinitalise buffer
            InitBuffer();
            Refresh();  //refresh
        }

        // dispose
        private void DisposeBuffer()
        {
            BackBuffer.Dispose();
            BufferGraphics.Dispose();
            Graphics.Dispose();
        }

        /// <summary>
        /// Render the contents of the rectangle to the buffer
        /// </summary>
        /// <param name="rectangle"></param>
        protected void Draw(Rectangle rectangle)
        {
            RenderToBuffer(new PaintEventArgs(BufferGraphics, rectangle));
            OnPaint(null);
        }

        private void RenderToBuffer(PaintEventArgs paintEventArgs)
        {
            if (Disposing || IsDisposed || Program.Exiting)
                return;
            // make rectangle a little bigger just in case
            Rectangle invalid = paintEventArgs.ClipRectangle;
            invalid.Offset(AutoScrollPosition);

            // paint background
            base.OnPaintBackground(new PaintEventArgs(paintEventArgs.Graphics, invalid));

            // paint rows
            int height = Padding.Top + AutoScrollPosition.Y;
            int width = TotalWidth();
            int left = Padding.Left + AutoScrollPosition.X;

            // draw header

            if (HeaderRow != null)
            {
                int headerrowheight = HeaderRow.RowAndChildrenHeight;
                if (height >= invalid.Top - headerrowheight && height <= invalid.Bottom + headerrowheight)
                    HeaderRow.OnPaint(new RowPaintArgs(paintEventArgs.Graphics, invalid, new Rectangle(left, height, width, headerrowheight)));
                height += headerrowheight + HeaderRow.SpaceAfter;
            }

            int numRows = Rows.Count;
            for (int i = 0; i < numRows; ++i)
            {
                GridRow row = Rows[i];
                int rowheight = row.RowAndChildrenHeight;
                if (height >= invalid.Top - rowheight && height <= invalid.Bottom + rowheight)
                    row.OnPaint(new RowPaintArgs(paintEventArgs.Graphics, invalid, new Rectangle(left, height, width, rowheight)));
                height += rowheight;
                if (i < numRows - 1)
                    height += row.SpaceAfter;
            }
            height += Padding.Bottom;

            // set the clientrecangle to update the autoscrollbar
            AutoScrollMinSize = new Size(GetMinColumn(), height - AutoScrollPosition.Y);
            //base.OnPaint(paintEventArgs);
        }

        private int TotalWidth()
        {
            int allowedwidth = ClientSize.Width - Padding.Horizontal;
            if (HeaderRow == null)
                return allowedwidth;
            int actualwidth = GetMinColumn() - Padding.Horizontal;
            return actualwidth > allowedwidth ? actualwidth : allowedwidth;
        }

        private int GetMinColumn()
        {
            if (HeaderRow == null)
                return 0;
            int total = Padding.Horizontal + LeftOffset;
            foreach (string col in HeaderRow.Columns)
            {
                total += HeaderRow.GetColumnWidth(col);
            }
            return total;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Disposing || IsDisposed || Program.Exiting)
                return;

            if (!DraggingColumns)
            {
                // we dont need to render again apparently :)
                Core.Drawing.QuickDraw(Graphics, BackBuffer);
            }
            else
            {
                //paint control
                IntPtr pTarget = DragColumnsGraphics.GetHdc();
                IntPtr pSource = Core.Drawing.CreateCompatibleDC(pTarget);
                IntPtr pOrig = Core.Drawing.SelectObject(pSource, BackBuffer.GetHbitmap());
                Core.Drawing.BitBlt(pTarget, 0, 0, BackBuffer.Width, BackBuffer.Height, pSource, 0, 0, Core.Drawing.TernaryRasterOperations.SRCCOPY);
                IntPtr pNew = Core.Drawing.SelectObject(pSource, pOrig);
                Core.Drawing.DeleteObject(pNew);
                Core.Drawing.DeleteDC(pSource);
                //DragGraphics.ReleaseHdc(pTarget);

                // paint dragged stuff
                //IntPtr pTarget2 = DragGraphics.GetHdc();
                IntPtr pSource2 = Core.Drawing.CreateCompatibleDC(pTarget);
                IntPtr pOrig2 = Core.Drawing.SelectObject(pSource2, DragColumnsData.GetHbitmap());
                Core.Drawing.BitBlt(pTarget, DragColumnsWindowPosition.X, DragColumnsWindowPosition.Y, DragColumnsData.Width, DragColumnsData.Height, pSource2, 0, 0, Core.Drawing.TernaryRasterOperations.SRCAND);
                IntPtr pNew2 = Core.Drawing.SelectObject(pSource2, pOrig2);
                Core.Drawing.DeleteObject(pNew2);
                Core.Drawing.DeleteDC(pSource2);
                DragColumnsGraphics.ReleaseHdc(pTarget);

                // paint everything to screen
                IntPtr pTarget3 = Graphics.GetHdc();
                IntPtr pSource3 = Core.Drawing.CreateCompatibleDC(pTarget3);
                IntPtr pOrig3 = Core.Drawing.SelectObject(pSource3, DragColumnsBuffer.GetHbitmap());
                Core.Drawing.BitBlt(pTarget3, 0, 0, DragColumnsBuffer.Width, DragColumnsBuffer.Height, pSource3, 0, 0, Core.Drawing.TernaryRasterOperations.SRCCOPY);
                IntPtr pNew3 = Core.Drawing.SelectObject(pSource3, pOrig3);
                Core.Drawing.DeleteObject(pNew3);
                Core.Drawing.DeleteDC(pSource3);
                Graphics.ReleaseHdc(pTarget3);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        private bool DraggingItems;
        private bool DraggingColumns = false;
        private Bitmap DragColumnsData;
        private Point DragColumnsWindowPosition;
        private Point DragColumnsMouseLocation;
        private string ActiveColumn = "";
        private Bitmap DragColumnsBuffer;
        Graphics DragColumnsGraphics;
        string DragColumnsLastEntered = "";
        GridRow LastRow;
        bool ResizingColumns = false;
        Point ResizeColumnsInitialMouseLocation;

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && DraggingColumns)
            {
                string swapwith = "";
                int left = Padding.Left;
                foreach (string index in HeaderRow.Columns)
                {
                    int width = GetColumnWidth(index, 1);
                    if (e.X - AutoScrollPosition.X < left + (width / 2) && !((GridHeaderItem)HeaderRow.Items[index]).Immovable)
                    {
                        swapwith = index;
                        break;
                    }
                    left += width;
                }

                if (string.IsNullOrEmpty(swapwith))
                    swapwith = "end";

                DraggingColumns = false;

                if (!HeaderRow.Items.ContainsKey(ActiveColumn))
                    return;

                ((GridHeaderItem)HeaderRow.Items[ActiveColumn]).GreyOut = false;
                if (DragColumnsData != null)
                {
                    DragColumnsData.Dispose();
                    DragColumnsData = null;
                }
                if (DragColumnsBuffer != null)
                {
                    DragColumnsBuffer.Dispose();
                    DragColumnsBuffer = null;
                }
                if (DragColumnsGraphics != null)
                {
                    DragColumnsGraphics.Dispose();
                    DragColumnsGraphics = null;
                }
                if (swapwith == "end")
                {
                    HeaderRow.Columns.Remove(ActiveColumn);
                    HeaderRow.Columns.Add(ActiveColumn);
                }
                else
                {
                    int movedn = HeaderRow.Columns.IndexOf(ActiveColumn);
                    int replacedn = HeaderRow.Columns.IndexOf(swapwith);
                    if (movedn > replacedn)
                    {
                        HeaderRow.Columns.Remove(ActiveColumn);
                        HeaderRow.Columns.Insert(replacedn, ActiveColumn);
                    }
                    else if (movedn < replacedn)
                    {
                        HeaderRow.Columns.Insert(replacedn, ActiveColumn);
                        HeaderRow.Columns.Remove(ActiveColumn);
                    }
                }
                DragColumnsLastEntered = "";
                Refresh();
            }
            else if (e.Button == MouseButtons.Left && ResizingColumns)
            {
                ResizingColumns = false;
            }
            ActiveColumn = "";
        }

        public void AddRow(GridRow row)
        {
            Rows.Add(row);
            row.GridView = this;
        }

        public void SetHeaderRow(GridHeaderRow hr)
        {
            hr.GridView = this;
        }

        /// <summary>
        /// Returns the width of the specified columns (ie index to index + span)
        /// </summary>
        internal int GetColumnWidth(string col, int span)
        {
            if (HeaderRow == null) return 0;
            int total = 0;
            int index = HeaderRow.Columns.IndexOf(col);
            for (int i = index; i < index + span; i++)
            {
                if (HeaderRow.Columns.Count <= i)
                    break;
                total += HeaderRow.GetColumnWidth(HeaderRow.Columns[i]);
            }
            return total;
        }

        /// <summary>
        /// Scroll up or down the list only if it has been focused
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            AutoScrollPosition = new Point(-AutoScrollPosition.X, -AutoScrollPosition.Y - e.Delta);
            OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, 0));
        }

        protected virtual void OnMouseButtonAction(MouseEventArgs e, MouseButtonAction type)
        {
            if (HeaderRow == null)
                return;

            if (!DraggingColumns && !ResizingColumns)
            {
                this.Focus();
                Point p;
                GridRow row = FindRowFromPoint(e.Location, out p);
                if (row == null)
                    return;

                if (e.Button == MouseButtons.Right && row is GridHeaderRow)
                {
                    OpenChooseColumnsMenu(e.Location);
                }
                else if (e.Button == MouseButtons.Left)
                {
                    if (Cursor == Cursors.SizeWE)
                    {
                        if (type == MouseButtonAction.DoubleClick)
                            FitColumnWidthToHeaderAndContents(row, p);
                    }
                    else
                    {
                        row.OnMouseButtonAction(p, type);
                    }
                }
                else if (type == MouseButtonAction.SingleRightClick)
                {
                    row.OnMouseButtonAction(p, type);
                }
            }
        }

        private void FitColumnWidthToHeaderAndContents(GridRow currentRow, Point p)
        {
            GridHeaderItem headerItem = (GridHeaderItem)currentRow.FindItemFromPoint(new Point(p.X - GridHeaderItem.ResizeGutter, p.Y), out ResizeColumnsInitialMouseLocation);
            ResizeColumnsInitialMouseLocation.X += GridHeaderItem.ResizeGutter;
            ActiveColumn = headerItem.ColumnName;

            if (HeaderRow.Items.ContainsKey(ActiveColumn))
            {
                int maxWidth = headerItem.MinimumWidth;

                foreach (GridRow row in RowsAndChildren)
                {
                    GridItemBase _item = row.GetItem(ActiveColumn);
                    int itemWidth = _item.GetGridItemWidth(ActiveColumn);

                    if (itemWidth > maxWidth)
                        maxWidth = itemWidth;
                }

                headerItem.Width = maxWidth;
                Refresh();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            OnMouseButtonAction(e, e.Button == MouseButtons.Left ? MouseButtonAction.SingleClick : MouseButtonAction.SingleRightClick);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            OnMouseButtonAction(e, MouseButtonAction.DoubleClick);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            lastClickedPoint = e.Location;
            OnMouseButtonAction(e, MouseButtonAction.MouseDown);
        }

        public virtual void OpenChooseColumnsMenu(Point p)
        {

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            HoldCursorChange(delegate() { OnMouseMove_(e); });
        }

        private void HoldCursorChange(MethodInvoker handler)
        {
            hold_cursor_change = true;
            try
            {
                handler();
            }
            finally
            {
                hold_cursor_change = false;
                if (new_cursor != null)
                {
                    Cursor = new_cursor;
                    new_cursor = null;
                }
            }
        }

        private void OnMouseMove_(MouseEventArgs e)
        {
            if (Disposing || IsDisposed || Program.Exiting)
                return;

            if (HeaderRow == null)
                return;

            if (DraggingItems)
                return;

            if (DraggingColumns)
            {
                int x = e.Location.X - DragColumnsMouseLocation.X;
                int xmax = TotalWidth() - DragColumnsData.Width;
                DragColumnsWindowPosition = new Point(x <= 0 ? 0 : x < xmax ? x : xmax, 0);
                string swapwith = "";
                int left = Padding.Left;
                foreach (string index in HeaderRow.Columns)
                {
                    int width = GetColumnWidth(index, 1);
                    if (((GridHeaderItem)HeaderRow.Items[index]).Immovable)
                    {
                        left += width;
                        continue;
                    }
                    if (e.X - AutoScrollPosition.X < left + (width / 2))
                    {
                        swapwith = index;
                        break;
                    }
                    left += width;
                }

                if (string.IsNullOrEmpty(swapwith))
                    swapwith = "end";

                if (swapwith != DragColumnsLastEntered)
                {
                    if (swapwith == "end")
                    {
                        HeaderRow.Columns.Remove(ActiveColumn);
                        HeaderRow.Columns.Add(ActiveColumn);
                    }
                    else
                    {
                        int movedn = HeaderRow.Columns.IndexOf(ActiveColumn);
                        int replacedn = HeaderRow.Columns.IndexOf(swapwith);
                        if (movedn > replacedn)
                        {
                            HeaderRow.Columns.Remove(ActiveColumn);
                            HeaderRow.Columns.Insert(replacedn, ActiveColumn);
                        }
                        else if (movedn < replacedn)
                        {
                            HeaderRow.Columns.Insert(replacedn, ActiveColumn);
                            HeaderRow.Columns.Remove(ActiveColumn);
                        }
                    }
                    DragColumnsLastEntered = swapwith;
                    Refresh();
                }
                else
                {
                    OnPaint(null);
                }
            }
            else if (ResizingColumns)
            {
                if (HeaderRow.Items.ContainsKey(ActiveColumn))
                {
                    GridHeaderItem item = (HeaderRow.Items[ActiveColumn] as GridHeaderItem);
                    int newWidth = item.Width + (e.Location.X - ResizeColumnsInitialMouseLocation.X);
                    if (newWidth > item.MinimumWidth)
                    {
                        item.Width = newWidth;
                        ResizeColumnsInitialMouseLocation = e.Location;
                    }
                    else
                    {
                        item.Width = item.MinimumWidth;
                    }
                    Refresh();
                }
            }
            else if (e.Button == MouseButtons.Left && string.IsNullOrEmpty(ActiveColumn) &&
                DragDistance(lastClickedPoint, e.Location) >= 2)  // don't start dragging unles we've moved a little distance
            // TODO: consider replacing the above with SystemParameters.MinimumHorizontalDragDistance etc. when we move to .NET 3.0
            {
                Point p;
                GridRow row = FindRowFromPoint(lastClickedPoint, out p);  // the drag is then based on the mouse-down location
                if (row is GridHeaderRow)
                {
                    GridHeaderItem item = (GridHeaderItem)row.FindItemFromPoint(new Point(p.X - GridHeaderItem.ResizeGutter, p.Y), out DragColumnsMouseLocation);
                    DragColumnsMouseLocation.X += GridHeaderItem.ResizeGutter;
                    if (item == null)
                        return;

                    ActiveColumn = item.ColumnName;
                    int colwidth = GetColumnWidth(ActiveColumn, 1);

                    if (!item.UnSizable && DragColumnsMouseLocation.X >= colwidth - GridHeaderItem.ResizeGutter)
                    {
                        ResizingColumns = true;
                        ResizeColumnsInitialMouseLocation = lastClickedPoint;
                    }
                    else if (!item.Immovable)
                    {
                        DraggingColumns = true;
                        item.GreyOut = true;
                        DragColumnsData = new Bitmap(GetColumnWidth(ActiveColumn, 1), ClientSize.Height);

                        Graphics ddGraphics = Graphics.FromImage(DragColumnsData);
                        ddGraphics.Clear(BackColor);
                        XenAdmin.Core.Drawing.QuickDraw(ddGraphics, BackBuffer, new Point((lastClickedPoint.X - DragColumnsMouseLocation.X), 0), new Rectangle(0, 0, DragColumnsData.Width, DragColumnsData.Height));
                        ddGraphics.Dispose();

                        DragColumnsBuffer = new Bitmap(ClientSize.Width, ClientSize.Height);
                        DragColumnsGraphics = Graphics.FromImage(DragColumnsBuffer);
                    }
                }
                else if (row != null)
                    row.OnMouseButtonAction(p, MouseButtonAction.StartDrag);
            }
            else
            {
                Point p;
                GridRow row = FindRowFromPoint(e.Location, out p);
                if (LastRow == null && row == null)
                    return;

                if (LastRow == row)
                {
                    LastRow.OnMouseMove(p);
                }
                else
                {
                    if (LastRow != null)
                        LastRow.OnLeave();

                    LastRow = row;

                    if (LastRow != null)
                        LastRow.OnEnter(p);
                }
            }
        }

        // The L_1 distance between two points
        private int DragDistance(Point p1, Point p2)
        {
            int xDisp = p1.X - p2.X;
            int yDisp = p1.Y - p2.Y;
            int totDisp = (xDisp >= 0 ? xDisp : -xDisp) + (yDisp >= 0 ? yDisp : -yDisp);
            return totDisp;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            HoldCursorChange(delegate()
            {
                if (LastRow != null)
                    LastRow.OnLeave();
            });
        }

        /// <summary>
        /// Takes the point in question (eg location of the mouseclick) and returns the row 
        /// that was clicked and the point on that row (ie adjusted so that 0,0 is top left of that row)
        /// </summary>
        private GridRow FindRowFromPoint(Point point, out Point p)
        {
            int height = Padding.Top + AutoScrollPosition.Y;

            p = Point.Empty;

            if (HeaderRow != null)
            {
                if (RowContainsPoint(HeaderRow, point, ref height, ref p))
                    return HeaderRow;
            }

            foreach (GridRow row in Rows)
            {
                if (RowContainsPoint(row, point, ref height, ref p))
                    return row;
            }

            return null;
        }

        private bool RowContainsPoint(GridRow row, Point point, ref int height, ref Point p)
        {
            int rowheight = row.RowAndChildrenHeight;
            if (point.Y >= height && point.Y < height + rowheight)
            {
                p = new Point(point.X - Padding.Left - AutoScrollPosition.X, point.Y - height);
                return true;
            }
            else
            {
                height += rowheight + row.SpaceAfter;
                return false;
            }
        }

        private Dictionary<string, RowState> saveState;

        protected void SaveRowStates()
        {
            saveState = new Dictionary<string, RowState>();
            foreach (GridRow row in RowsAndChildren)
                saveState[row.Path] = row.State;
        }

        protected void RestoreRowStates()
        {
            RowState state;
            foreach (GridRow row in RowsAndChildren)
            {
                if (saveState.TryGetValue(row.Path, out state))
                    row.State = state;
            }
        }

        internal void UnselectAllRowsExcept(GridRow notThisOne)
        {
            foreach (GridRow row in RowsAndChildren)
            {
                if (row != notThisOne)
                    row.Selected = false;
            }
            Refresh();
        }

        internal void UnselectAllRows()
        {
            UnselectAllRowsExcept(null);
        }

        internal void SelectRowRange(GridRow newRow)
        {
            UnselectAllRows();

            // Once through to check we haven't lost either of the endpoints
            // for some reason
            bool foundLast = false;
            bool foundNew = false;
            if (lastClickedRowPath != null)
            {
                foreach (GridRow row in RowsAndChildren)
                {
                    if (row.Path == lastClickedRowPath)
                        foundLast = true;
                    if (row == newRow)
                        foundNew = true;
                    if (foundLast && foundNew)
                        break;
                }
            }

            // If endpoint is missing, turn it into a normal select
            if (!foundLast || !foundNew)
            {
                newRow.Selected = true;
                LastClickedRow = newRow;
            }

            // Otherwise do the proper range select.
            // Make sure to include both endpoints.
            foundLast = foundNew = false;
            foreach (GridRow row in RowsAndChildren)
            {
                if (row.Path == lastClickedRowPath)
                    foundLast = true;
                if (row == newRow)
                    foundNew = true;
                if (foundLast || foundNew)
                    row.Selected = true;
                if (foundLast && foundNew)
                    break;
            }
        }



        public virtual void StartDragDrop()
        {
            GridRowCollection rows = new GridRowCollection();
            foreach (GridRow row in RowsAndChildren)
            {
                if (row.Selected && IsDraggableRow(row))
                    rows.Add(row);
            }
            if (rows.Count > 0)
                DoDragDrop(rows, DragDropEffects.Move);
        }

        public virtual bool IsDraggableRow(GridRow row)
        {
            return true;
        }

        /// <summary>
        /// Equivalent to Clear(false).
        /// </summary>
        internal void Clear()
        {
            Clear(false);
        }

        /// <summary>
        /// Removes all rows from the GridView.
        /// </summary>
        /// <param name="removeHeaders">If true, removes GridHeaderRows too.</param>
        internal void Clear(bool removeHeaders)
        {
            Rows.RemoveAll(new Predicate<GridRow>(delegate(GridRow row)
            {
                return removeHeaders || !(row is GridHeaderRow);
            }));
        }

        internal virtual void Sort()
        {
            Rows.Sort();
            foreach (GridRow row in Rows)
            {
                row.Sort();
            }
            LastClickedRow = null;
        }

        public List<KeyValuePair<String, int>> ColumnsAndWidths
        {
            get
            {
                List<KeyValuePair<String, int>> columns = new List<KeyValuePair<String, int>>();

                if (HeaderRow == null)
                    return columns;

                foreach (String columnName in HeaderRow.Columns)
                {
                    if (!HeaderRow.Items.ContainsKey(columnName))
                        continue;
                    GridHeaderItem item = HeaderRow.Items[columnName] as GridHeaderItem;
                    if (item == null)
                        continue;

                    columns.Add(new KeyValuePair<String, int>(columnName, item.Width));
                }

                return columns;
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            DraggingItems = true;
            e.Effect = DragDropEffects.None;
            base.OnDragEnter(e);
        }

        protected override void OnDragLeave(EventArgs e)
        {
            DraggingItems = false;
            base.OnDragLeave(e);
        }
    }
}
