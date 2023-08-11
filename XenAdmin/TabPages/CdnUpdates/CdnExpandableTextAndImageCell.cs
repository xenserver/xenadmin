/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Drawing;
using System.Windows.Forms;

namespace XenAdmin.TabPages.CdnUpdates
{
    internal class CdnExpandableTextAndImageCell : DataGridViewTextBoxCell
    {
        private const int IMAGE_WIDTH = 22;

        public Image Image { get; set; }

        public bool IsPointInExpander(Point p)
        {
            if (OwningRow is CdnExpandableRow row && row.ChildRows.Count > 0)
            {
                var indent = row.Level * IMAGE_WIDTH;
                var rect = new Rectangle(ContentBounds.X + indent, ContentBounds.Y, IMAGE_WIDTH, ContentBounds.Height);
                return rect.Contains(p);
            }

            return false;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            var indent = 0;

            var color = (cellState & DataGridViewElementStates.Selected) != 0
                ? DataGridView.DefaultCellStyle.SelectionBackColor
                : DataGridView.DefaultCellStyle.BackColor;

            if (OwningRow is CdnExpandableRow row)
            {
                indent = row.Level * IMAGE_WIDTH;

                using (var brush = new SolidBrush(color))
                    graphics.FillRectangle(brush, cellBounds.X, cellBounds.Y, indent + IMAGE_WIDTH, cellBounds.Height);

                if (row.ChildRows.Count > 0)
                {
                    //the dimensions of the expander are 9x9, but we want it in the middle of a 18x16 square

                    var expander = row.IsExpanded ? Images.StaticImages.tree_minus : Images.StaticImages.tree_plus;
                    graphics.DrawImage(expander, cellBounds.X + indent + 5, cellBounds.Y + 5, 9, 9);
                }

                indent += IMAGE_WIDTH;
            }

            if (Image != null)
            {
                using (var brush = new SolidBrush(color))
                    graphics.FillRectangle(brush, cellBounds.X + indent, cellBounds.Y, IMAGE_WIDTH, cellBounds.Height);

                graphics.DrawImage(Image, cellBounds.X + indent, cellBounds.Y + 2,
                    Math.Min(Image.Width, IMAGE_WIDTH), Math.Min(Image.Height, cellBounds.Height));

                indent += Image.Width;
            }

            var textBounds = new Rectangle(cellBounds.X + indent, cellBounds.Y, cellBounds.Width - indent, cellBounds.Height);

            base.Paint(graphics, clipBounds, textBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }
    }
}
