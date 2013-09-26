/* Copyright (c) Citrix Systems Inc. 
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using XenAdmin.Core;
using System.Drawing;
using System.Linq;

namespace XenAdmin.Controls
{
    public interface IEnableableComboBoxItem
    {
        bool Enabled { get; }
    }

    /// <summary>
    /// Adding Items which extend IEnableableComboBoxItem will mean that the mouse click
    /// will be disabled on those items where the Enabled value is false
    /// </summary>
    public class EnableableComboBox : NonSelectableComboBox
    {
        public EnableableComboBox()
        {
            DrawItem += m_comboBoxConnection_DrawItem;
            DrawMode = DrawMode.OwnerDrawVariable;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private bool disposed;
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(!disposed)
                {
                    DrawItem -= m_comboBoxConnection_DrawItem;
                }
                disposed = true;
            }
            base.Dispose(disposing);
        }

        private void m_comboBoxConnection_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            ComboBox cb = (ComboBox)sender;
            int index = e.Index;

            if (index > -1 && cb != null)
            {
                IEnableableComboBoxItem item = cb.Items[index] as IEnableableComboBoxItem;
                Color textColor = SystemColors.ControlText;

                //Paint disabled items grey - otherwise leave them black
                if (item != null && !item.Enabled)
                    textColor = SystemColors.GrayText;    

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    textColor = SystemColors.HighlightText;

                Drawing.DrawText(e.Graphics, cb.Items[index].ToString(), cb.Font, e.Bounds.Location, textColor);
            }
        }

        public new bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                BackColor = value ? SystemColors.Window : SystemColors.Control;
            }
        }

        protected override bool IsItemNonSelectable(object o)
        {
            var item = o as IEnableableComboBoxItem;
            return item != null && !item.Enabled;
        }
    }
}
