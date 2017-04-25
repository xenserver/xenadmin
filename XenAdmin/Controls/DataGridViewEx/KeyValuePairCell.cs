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

namespace XenAdmin.Controls.DataGridViewEx
{
    /// <summary>
    /// This cell expects a List of String,String KeyValuePairs as its Value field. It renders them in a 2 column table and a 
    /// pair of empty strings is printed as vertical space.
    /// </summary>
    public class KeyValuePairCell : DataGridViewTextBoxCell
    {
        private static readonly int KVP_SPACE = 2;
        private static readonly int KVP_VERTICAL_SPACE_DIVIDER = 5;

        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            List<KeyValuePair<String, String>> kvps = Value as List<KeyValuePair<String, String>>;
            if (kvps == null)
                return base.GetPreferredSize(graphics, cellStyle, rowIndex, constraintSize);

            int indent = 0;
            int height = 0;
            int width = 0;
            // Work through each key, looking for the largest so we know how far to indent the values
            // Also keep track of how much vertical space it will take to lay out all the kvps
            for (int i = 0; i < kvps.Count; i++)
            {
                KeyValuePair<String, String> kvp = kvps[i];
                if (kvp.Key == String.Empty && kvp.Value == String.Empty)
                {
                    // Empty key and value means we want a vertical gap
                    height += KVP_VERTICAL_SPACE_DIVIDER + KVP_SPACE;
                    continue;
                }
                Size s = Drawing.MeasureText(kvp.Key, cellStyle.Font);
                if (s.Width > indent)
                    indent = s.Width;
                height += s.Height + KVP_SPACE;
            }
            // Add the spacing between the keys and the values
            indent += KVP_SPACE;
            for (int i = 0; i < kvps.Count; i++)
            {
                KeyValuePair<String, String> kvp = kvps[i];
                Size s = Drawing.MeasureText(kvp.Value, cellStyle.Font);
                if (s.Width + indent > width)
                    width = s.Width + indent;
            }
            // Add the final space at the bottom so the top margin and the bottom margin are equal
            height += KVP_SPACE;
            return new Size(width + Style.Padding.Horizontal, height + Style.Padding.Vertical);

        }

        public override string ToString()
        {
            // Turn the list of kvps into one a string with one kvp per line and a blank line per 
            // space element (blank k and v) in the list
            List<KeyValuePair<String, String>> kvps = Value as List<KeyValuePair<String, String>>;
            if (kvps == null)
                return "";

            string s = "";
            foreach (KeyValuePair<String, String> kvp in kvps)
            {
                if (kvp.Key == String.Empty && kvp.Value == String.Empty)
                {
                    s += "\n";
                    continue;
                }
                s += String.Format("{0} {1}\n", kvp.Key, kvp.Value);
            }
            s = s.Trim();
            return s;
        }

        protected override object GetClipboardContent(int rowIndex, bool firstCell, bool lastCell, bool inFirstRow, bool inLastRow, string format)
        {
            return ToString();
        }

        protected override bool SetValue(int rowIndex, object value)
        {
            // This is only overridden so we can set the tooltip text to a sensible string on value change
            bool result = base.SetValue(rowIndex, value);
            ToolTipText = ToString();
            return result;
        }

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates cellState,
            object value,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, "", errorText, cellStyle, advancedBorderStyle, paintParts);

            List<KeyValuePair<String, String>> kvps = value as List<KeyValuePair<String, String>>;
            if (kvps == null)
                return;

            // Paint the background
            using (var brush = new SolidBrush(Selected ? cellStyle.SelectionBackColor : cellStyle.BackColor))
                graphics.FillRectangle(brush, cellBounds.X + cellStyle.Padding.Left,
                    cellBounds.Y + cellStyle.Padding.Top, cellBounds.Width - cellStyle.Padding.Horizontal,
                    cellBounds.Height - cellStyle.Padding.Vertical);

            // Go through drawing the keys, making note of their widths so we know how much to indent the values by
            // and keeping track of the Y positions so we can place the value labels without measuring.
            int indent = 0;
            int currentY = 0;
            int[] heights = new int[kvps.Count];
            for (int i = 0; i < kvps.Count; i++)
            {
                KeyValuePair<String, String> kvp = kvps[i];
                if (kvp.Key == String.Empty && kvp.Value == String.Empty)
                {
                    // empty key and value means we want a vertical gap
                    currentY += KVP_VERTICAL_SPACE_DIVIDER + KVP_SPACE;
                    heights[i] = KVP_VERTICAL_SPACE_DIVIDER;
                    continue;
                }
                Size s = Drawing.MeasureText(kvp.Key, cellStyle.Font);
                if (s.Width > indent)
                    indent = s.Width;

                using (var brush = new SolidBrush(Selected ? cellStyle.SelectionForeColor : cellStyle.ForeColor))
                    graphics.DrawString(
                        kvp.Key,
                        cellStyle.Font, brush,
                        (float)(cellBounds.X + cellStyle.Padding.Left),
                        (float)(cellBounds.Y + currentY));

                currentY += s.Height + KVP_SPACE;
                heights[i] = s.Height;
            }
            currentY = 0;
            // Add in the space between the key and value text
            indent += KVP_SPACE;
            // Now print the values
            for (int i = 0; i < kvps.Count; i++)
            {
                KeyValuePair<String, String> kvp = kvps[i];

                using (var brush = new SolidBrush(Selected ? cellStyle.SelectionForeColor : cellStyle.ForeColor))
                    graphics.DrawString(kvp.Value, cellStyle.Font, brush,
                        (float)(cellBounds.X + cellStyle.Padding.Left + indent),
                        (float)(cellBounds.Y + currentY));

                currentY += heights[i] + KVP_SPACE;
            }
        }
    }
}
