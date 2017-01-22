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
using System.Windows.Forms;

using XenAdmin.Core;

namespace XenAdmin.Controls
{
    /// <summary>
    /// A combobox that automatically resizes the dropdown's width to be as big
    /// as the longest string in the items list
    /// </summary>
    public partial class LongStringComboBox : ComboBox
    {
        public LongStringComboBox()
        {
            InitializeComponent();
        }

        protected override void OnDropDown(EventArgs e)
        {
            int totalWidth = MeasureLongestItem + VerticalScrollBarWidth;
            DropDownWidth = totalWidth > Width ? totalWidth : Width;
            base.OnDropDown(e);
        }

        protected override void OnCreateControl()
        {
            // Some strings can be very long, so we will show the value as a mouse over tooltip.
            // Initialize the tooltip here.
            toolTip.Active = true;
            toolTip.AutomaticDelay = 0;
            toolTip.AutoPopDelay = 50000;
            toolTip.InitialDelay = 50;
            toolTip.ReshowDelay = 50;
            toolTip.ShowAlways = true;

            base.OnCreateControl();
        }

        private int MeasureLongestItem
        {
            get
            {
                int longest = 0;
                foreach (var item in Items)
                {
                    //CA-113310: use TextRenderer rather than Graphics to measure
                    //the string because it will be placed on a winforms control
                    //with the default UseCompatibleTextRendering value (false)
                    int stringLength = TextRenderer.MeasureText(item.ToString(), Font).Width;
                    if (stringLength > longest)
                        longest = stringLength;
                }
                return longest;
            }
        }

        private int VerticalScrollBarWidth
        {
            get { return Items.Count > MaxDropDownItems ? SystemInformation.VerticalScrollBarWidth : 0; }
        }

        private const int DropDownButtonWidth = 10;

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            string itemText = SelectedItem != null ? SelectedItem.ToString() : string.Empty;
            int width = Drawing.MeasureText(itemText, Font).Width;

            int selectedItemTextWidth = !string.IsNullOrEmpty(itemText)
                                            ? width + DropDownButtonWidth
                                            : 0;
            toolTip.SetToolTip(this, selectedItemTextWidth > ClientRectangle.Width ? itemText : string.Empty);

            base.OnSelectedIndexChanged(e);
        }
    }
}
