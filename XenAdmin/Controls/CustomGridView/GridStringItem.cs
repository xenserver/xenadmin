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
using System.Runtime.InteropServices;
using System.Drawing.Text;
using XenAdmin.Core;


namespace XenAdmin.Controls.CustomGridView
{
    public class GridStringItem : GridItemBase
    {
        protected readonly HorizontalAlignment hAlign = HorizontalAlignment.Left;
        protected readonly VerticalAlignment vAlign = VerticalAlignment.Top;
        protected readonly Font font, hotFont;
        protected readonly Brush foreBrush, hotBrush;
        public Object sortdata;
        private readonly Object data;
        protected readonly bool respondToSelect;
        protected bool hot = false;

        public GridStringItem(object data, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool respondToSelect,
            bool clickSelectsRow, Brush foreBrush, Font font, Brush hotBrush, Font hotFont)
            : this(data, hAlign, vAlign, respondToSelect, clickSelectsRow, foreBrush, font, hotBrush, hotFont, 1, null, null)
        {
        }

        public GridStringItem(object data, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool respondToSelect,
            bool clickSelectsRow, Brush foreBrush, Font font)
            : this(data, hAlign, vAlign, respondToSelect, clickSelectsRow, foreBrush, font, foreBrush, font, 1, null, null)
        {
        }

        public GridStringItem(object data, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool respondToSelect,
            bool clickSelectsRow, Brush foreBrush, Font font, EventHandler onClickDelegate)
            : this(data, hAlign, vAlign, respondToSelect, clickSelectsRow, foreBrush, font, foreBrush, font, 1, onClickDelegate, null)
        {
        }

        public GridStringItem(object data, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool respondToSelect,
            bool clickSelectsRow, Brush foreBrush, Font font, int rowspan)
            : this(data, hAlign, vAlign, respondToSelect, clickSelectsRow, foreBrush, font, foreBrush, font, rowspan, null, null)
        {
        }

        public GridStringItem(object data, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool respondToSelect,
            bool clickSelectsRow, Brush foreBrush, Font font, int rowspan, EventHandler onClickDelegate)
            : this(data, hAlign, vAlign, respondToSelect, clickSelectsRow, foreBrush, font, foreBrush, font, rowspan, onClickDelegate, null)
        {
        }

        public GridStringItem(object data, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool respondToSelect,
            bool clickSelectsRow, Brush foreBrush, Font font, Brush hotBrush, Font hotFont, EventHandler onClickDelegate, EventHandler onRightClickDelegate)
            : this(data, hAlign, vAlign, respondToSelect, clickSelectsRow, foreBrush, font, hotBrush, hotFont, 1, onClickDelegate, onRightClickDelegate)
        {
        }

        // We use the convention that the click delegate is for single-click or double-click depending whether single-click already selects the row
        public GridStringItem(object data, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool respondToSelect,
            bool clickSelectsRow, Brush foreBrush, Font font, Brush hotBrush, Font hotFont, int rowSpan, EventHandler onClickDelegate, EventHandler onRightClickDelegate)
            : base(data == null, rowSpan, clickSelectsRow, clickSelectsRow ? null : onClickDelegate, clickSelectsRow ? onClickDelegate: null, onRightClickDelegate)
        {
            this.data = data;
            this.sortdata = data;
            this.hAlign = hAlign;
            this.vAlign = vAlign;
            this.respondToSelect = respondToSelect;
            this.foreBrush = foreBrush;
            this.font = font;
            this.hotBrush = hotBrush;
            this.hotFont = hotFont;
        }

        public virtual bool Hot
        {
            get { return hot; }
            set { hot = value; }
        }

        public override void OnPaint(ItemPaintArgs itemPaintArgs)
        {
            OnPaintInternal(itemPaintArgs,
                Hot ? hotFont : font,
                Hot ? hotBrush : foreBrush);
        }

        protected virtual void OnPaintInternal(ItemPaintArgs itemPaintArgs, Font localFont, Brush localForeBrush)
        {
            if (data == null)
                return;

            string output = (data is DateTime ? HelpersGUI.DateTimeToString((DateTime)data, Messages.DATEFORMAT_DMY_HM, true) : data.ToString());  // exception for DateTime: CA-46983
            // Replace either form of line ending with a space
            output = output.Replace("\n", " ").Replace("\r", "");
            Point loc = new Point();
            string text = Ellipsise(itemPaintArgs.Graphics, output, itemPaintArgs.Rectangle, localFont, out loc);
            if (!string.IsNullOrEmpty(text))
            {
                // Change text and background colour for selected rows
                bool selected = (respondToSelect && (itemPaintArgs.State & ItemState.Selected) > 0);
                if (selected)
                {
                    Size textSize = Drawing.MeasureText(text, localFont);
                    Rectangle textRect;
                    textRect = new Rectangle(loc, textSize);
                    itemPaintArgs.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), textRect);
                    localForeBrush = new SolidBrush(SystemColors.HighlightText);
                }

                itemPaintArgs.Graphics.DrawString(text, localFont, localForeBrush, loc);
            }
        }

        private bool IsOverText(Point point)
        {
            Rectangle rect = new Rectangle();
            if (hAlign == HorizontalAlignment.Center)
                rect.X = (CurrentRectangle.Width / 2) - (int)(DataSize.Width / 2);
            if (vAlign == VerticalAlignment.Middle)
                rect.Y = (CurrentRectangle.Height / 2) - (int)(DataSize.Height / 2);
            rect.Width = (int)DataSize.Width;
            rect.Height = (int)DataSize.Height;
            return rect.Contains(point);
        }

        public override void OnMouseDown(Point point)
        {
            if (IsOverText(point))
                base.OnMouseDown(point);
        }

        public override void OnClick(Point point)
        {
            if (!IsOverText(point))
                Row.GridView.UnselectAllRows();
            else
                base.OnClick(point);
        }

        public override void OnDoubleClick(Point point)
        {
            if (IsOverText(point))
                base.OnDoubleClick(point);
        }

        public override void OnStartDrag(Point point)
        {
            if (IsOverText(point))
                base.OnStartDrag(point);
        }

        internal SizeF DataSize;
        internal Rectangle CurrentRectangle;

        /// <summary>
        /// Gets the total width of all the contents of the grid item.
        /// </summary>
        /// <param name="column">The column this item belongs to</param>
        internal override int GetGridItemWidth(string column)
        {
            if (Row.ItemColumnSpan(this, column) == 1)
                return (int)Math.Ceiling((double)DataSize.Width);

            return base.GetGridItemWidth(column);
        }

        private string Ellipsise(Graphics g, String text, Rectangle rectangle, Font font, out Point loc)
        {
            DataSize = g.MeasureString(text, font);
            CurrentRectangle = rectangle;
            int width = (int)DataSize.Width;
            if (hAlign == HorizontalAlignment.Center && width < rectangle.Width)
                loc = new Point(rectangle.Left + (rectangle.Width - width) / 2, rectangle.Top);
            else
                loc = rectangle.Location;

            if (vAlign == VerticalAlignment.Middle)
                loc.Y += (int)((rectangle.Height - DataSize.Height) / 2);

            return text.Ellipsise(rectangle, font);
        }

        public override void OnMouseMove(Point point)
        {
            System.Diagnostics.Trace.Assert(Row != null);

            if (onClickDelegate != null && IsOverText(point))
            {
                Row.Cursor = Cursors.Hand;
                Hot = true;
            }
            else
            {
                Row.Cursor = Cursors.Default;
                Hot = false;
            }
            Row.GridView.Refresh();
        }

        public override void OnLeave()
        {
            Row.Cursor = Cursors.Default;
            Hot = false;
            Row.GridView.Refresh();
        }

        public override int CompareTo(GridItemBase gridItem)
        {
            GridStringItem other = gridItem as GridStringItem;

            Object otherdata = other == null ? null : other.sortdata;

            if (sortdata == null)
                return otherdata == null ? 0 : 1;
            
            if (otherdata == null)
                return -1;

            return StringUtility.NaturalCompare(sortdata.ToString(), other.sortdata.ToString());
        }
    }
}
