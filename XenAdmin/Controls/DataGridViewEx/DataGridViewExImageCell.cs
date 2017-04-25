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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XenAdmin.Controls.DataGridViewEx
{
    public class DataGridViewExImageCell : DataGridViewImageCell
    {
        private bool show = true;

        public DataGridViewExImageCell()
            : base(false)
        {
            ValueType = typeof(Image);
        }

        public bool Show
        {
            get
            {
                return show;
            }
            set
            {
                show = value;
                RaiseCellValueChanged(new DataGridViewCellEventArgs(ColumnIndex, RowIndex));
            }
        }

        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            if (!Show)
                return new Size(16, 16);
            return base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (!Show)
            {
                using (Brush b = new SolidBrush(Selected ? cellStyle.SelectionBackColor : cellStyle.BackColor))
                    graphics.FillRectangle(b, cellBounds);
            }
            else if (DataGridView.Enabled)
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText,
                           cellStyle, advancedBorderStyle, paintParts);
            }
            else
            {
                Image img = (Image)value;
                using (Brush b = new SolidBrush(Selected ? cellStyle.SelectionBackColor : cellStyle.BackColor))
                    graphics.FillRectangle(b, cellBounds);
                if (img == null)
                    return;
                ControlPaint.DrawImageDisabled(graphics, img, cellBounds.X + ((cellBounds.Width - img.Width) / 2), cellBounds.Y + ((cellBounds.Height - img.Height) / 2),
                                               Selected ? cellStyle.SelectionBackColor : cellStyle.BackColor);
            }
        }
    }
}
