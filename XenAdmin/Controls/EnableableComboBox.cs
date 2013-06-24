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
    public class EnableableComboBox : LongStringComboBox
    {
        private IntPtr oldWndProc = IntPtr.Zero;
        private Win32.WndProcDelegate newWndProc;
        private IntPtr DropDownHandle = IntPtr.Zero;

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
                using (SolidBrush textBrush = new SolidBrush(SystemColors.ControlText))
                {
                    //Paint disabled items grey - otherwise leave them black
                    if (item != null && !item.Enabled)
                        textBrush.Color = SystemColors.GrayText;

                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                        textBrush.Color = SystemColors.HighlightText;

                    e.Graphics.DrawString(cb.Items[index].ToString(), ((Control)sender).Font, textBrush, e.Bounds.X, e.Bounds.Y);
                }
            }
        }

        private bool AllItemsDisabled()
        {
            var list = Items.OfType<IEnableableComboBoxItem>().ToList();
            return (list.Count == Items.Count && list.All(item => !item.Enabled));
        }

        private bool skip;
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            int i = SelectedIndex;

            if (i == -1 || AllItemsDisabled())
                return;

            if (!(Items[i] is IEnableableComboBoxItem))
            {
                base.OnSelectedIndexChanged(e);
                return;
            }
            
            while (true)
            {
                if (i == 0)
                {
                    skip = true;
                }
                else if (i == Items.Count - 1)
                {
                    skip = false;
                }

                IEnableableComboBoxItem item = Items[i] as IEnableableComboBoxItem;
                if (item != null && !item.Enabled)
                {
                    i += skip ? 1 : -1;

                }
                else
                {
                    skip = true;
                    break;
                }
            }

            if(SelectedIndex != i)
            {
                //Calling this resends the event - you don't want to call the base
                //event or any registered handlers will be triggered twice
                SelectedIndex = i;
                return;
            }
            
            base.OnSelectedIndexChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.PageUp || e.KeyCode == Keys.Left)
            {
                skip = false;
            }
            else
            {
                skip = true;
            }
            base.OnKeyDown(e);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Win32.WM_PARENTNOTIFY:
                    DropDownHandle = m.LParam;
                    oldWndProc = Win32.GetWindowLong(DropDownHandle, Win32.GWL_WNDPROC);
                    newWndProc = new Win32.WndProcDelegate(ReplacementWndProc);
                    Win32.SetWindowLong(DropDownHandle, Win32.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newWndProc));
                    break;
            }

            base.WndProc(ref m);
        }

        private IntPtr ReplacementWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == (uint)Win32.WM_LBUTTONDOWN || msg == (uint)Win32.WM_LBUTTONDBLCLK)
            {
                Win32.POINT loc = new Win32.POINT();
                loc.X = MousePosition.X;
                loc.Y = MousePosition.Y;
                Win32.ScreenToClient(DropDownHandle, ref loc);
                Win32.RECT dropdown_rect = new Win32.RECT();
                Win32.GetClientRect(DropDownHandle, out dropdown_rect);
                if (dropdown_rect.Left <= loc.X && loc.X < dropdown_rect.Right && dropdown_rect.Top <= loc.Y && loc.Y < dropdown_rect.Bottom)
                {
                    int index = (int)Win32.SendMessage(DropDownHandle, Win32.LB_ITEMFROMPOINT, IntPtr.Zero, (IntPtr)(loc.X + (loc.Y << 16)));
                    if (index >> 16 == 0)
                    {
                        Object o = Items[index];
                        IEnableableComboBoxItem enableableComboBoxItem = o as IEnableableComboBoxItem;
                        if (enableableComboBoxItem != null && !enableableComboBoxItem.Enabled)
                            return IntPtr.Zero;
                    }
                }
            }
            return Win32.CallWindowProc(oldWndProc, hWnd, msg, wParam, lParam);
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
    }
}
