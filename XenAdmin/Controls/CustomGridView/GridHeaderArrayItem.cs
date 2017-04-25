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
    public class GridHeaderArrayItem : GridHeaderItem
    {
        private readonly GridStringItem[] items;

        // NB Brushes and fonts only apply to the sort indicators: those for the header text are declared within the items
        public GridHeaderArrayItem(GridStringItem[] items, HorizontalAlignment hAlign, VerticalAlignment vAlign,
            Brush backBrush, Font font, SortOrder defaultSortOrder, int width, int minwidth)
            : base(hAlign, vAlign, backBrush, font, "_", defaultSortOrder, width, minwidth, null)
        {
            this.items = items;
        }

        public override GridRow Row
        {
            set
            {
                base.Row = value;
                foreach (GridStringItem item in items)
                    item.Row = value;
            }
        }

        public override bool Hot
        {
            set
            {
                base.Hot = value;
                foreach (GridStringItem item in items)
                    item.Hot = value;
            }
        }

        public override void OnPaint(ItemPaintArgs itemPaintArgs)
        {
            if (!string.IsNullOrEmpty(sortdata.ToString()))
            {
                PaintManyInternal(itemPaintArgs);
                base.DrawGlyphs(itemPaintArgs);
            }
        }

        private void PaintManyInternal(ItemPaintArgs itemPaintArgs)
        {
            CurrentRectangle = new Rectangle();
            for (int i = 0; i < items.Length; i++)
            {
                GridStringItem item = items[i];
                Point locStr = itemPaintArgs.Rectangle.Location;
                locStr.Y += (itemPaintArgs.Rectangle.Height * i) / items.Length;
                item.OnPaint(
                    new ItemPaintArgs(itemPaintArgs.Graphics,
                        new Rectangle(locStr, new Size(itemPaintArgs.Rectangle.Width,
                        itemPaintArgs.Rectangle.Height / items.Length)), itemPaintArgs.State));

                if (item.DataSize.Width > DataSize.Width)
                {
                    DataSize.Width = item.DataSize.Width;
                }
                DataSize.Height += item.DataSize.Height;

                if (item.CurrentRectangle.Width > CurrentRectangle.Width)
                {
                    CurrentRectangle.Width = item.CurrentRectangle.Width;
                }

                CurrentRectangle.Height += item.CurrentRectangle.Height;
            }
        }
    }
}
