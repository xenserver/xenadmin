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

using XenAdmin.Core;
using XenAdmin.SettingsPanels;


namespace XenAdmin.Controls
{
    public class BootOrderListBox : ListBox
    {
        public BootOrderListBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count)
                return;

            e.DrawBackground();
            if (Items[e.Index] is BootDevice)
            {
                // Normal BootDevice row
                TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), e.Font, e.Bounds, e.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
            else
            {
                // Separator row
                string text = Messages.BOOT_DEVICE_DISABLED_ROW_TEXT;
                Size size = Drawing.MeasureText(text, e.Font);
                Rectangle rect = new Rectangle(e.Bounds.Left + (e.Bounds.Width - size.Width) / 2, e.Bounds.Top, size.Width, e.Bounds.Height);
                TextRenderer.DrawText(e.Graphics, text, e.Font, rect, SystemColors.GrayText);

                // Draw grey lines to either side of text
                int midRow = e.Bounds.Top + e.Bounds.Height / 2;
                e.Graphics.DrawLine(SystemPens.GrayText, new Point(e.Bounds.Left + 2, midRow), new Point(rect.Left - 2, midRow));
                e.Graphics.DrawLine(SystemPens.GrayText, new Point(rect.Right + 2, midRow), new Point(e.Bounds.Right - 3, midRow));
            }
        }
    }
}
