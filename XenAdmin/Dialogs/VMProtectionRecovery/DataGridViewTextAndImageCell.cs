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
using System.Windows.Forms;
using System.Drawing;

namespace XenAdmin.Dialogs.VMProtectionRecovery
{
    public class DataGridViewTextAndImageCell : DataGridViewTextBoxCell
    {

      
        public Image Image { get; set; }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (Image != null)
            {
                base.Paint(graphics, clipBounds,
                           new Rectangle(cellBounds.X + Image.Width, cellBounds.Y, cellBounds.Width - Image.Height,cellBounds.Height),
                           rowIndex, cellState, value, formattedValue,errorText, cellStyle, advancedBorderStyle, paintParts);

                if ((cellState & DataGridViewElementStates.Selected) != 0)
                {
                    using (var brush = new SolidBrush(DataGridView.DefaultCellStyle.SelectionBackColor))
                        graphics.FillRectangle(
                            brush, cellBounds.X, cellBounds.Y, Image.Width, cellBounds.Height);
                }
                else
                {
                    using (var brush = new SolidBrush(DataGridView.DefaultCellStyle.BackColor))
                        graphics.FillRectangle(brush,
                                               cellBounds.X, cellBounds.Y, Image.Width, cellBounds.Height);
                }
                graphics.DrawImage(Image, cellBounds.X, cellBounds.Y+2, Image.Width,
                                         Math.Min(Image.Height,cellBounds.Height));

            }
            else
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            }

        }
    }
}
