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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using XenAdmin.Core;


namespace XenAdmin.Controls
{
    /// <summary>
    /// A ComboBox allowing a combination of selectable and non-selectable DropDownItems.
    /// </summary>
    public class NonSelectableComboBox : LongStringComboBox
    {
        private IntPtr oldWndProc = IntPtr.Zero;
        private Win32.WndProcDelegate newWndProc;
        private IntPtr DropDownHandle = IntPtr.Zero;
        
        private bool skipDown;

        protected virtual bool IsItemNonSelectable(object o)
        {
            return false;
        }

        private bool HasSelectableItems()
        {
            foreach (var item in Items)
            {
                if (!IsItemNonSelectable(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// CA-12115
        /// The up and down arrow keys cause the combobox to change selection to
        /// the next or previous in the list. The page up and page down keys change
        /// the selection by one page at a time as defined by the size of the dropdown
        /// list. We need to make sure that when we arrow up or down that the item
        /// selection jumps over the non selectable items. We will change the skipDown
        /// direction here.
        /// Also the left and right arrows should navigate through list in the
        /// same way as the up and down arrows (CA-40779).
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.PageUp:
                case Keys.Up:
                    skipDown = false;
                    break;
                default:
                    skipDown = true;
                    break;
            }

            base.OnKeyDown(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            int i = SelectedIndex;

            if (i == -1 || !HasSelectableItems())
            {
                if (SelectedIndex != -1)
                    SelectedIndex = -1;
                return;
            }

            //Find the next selectable item in the appropriate (up/down) direction
            while (true)
            {
                if (i == 0)
                    skipDown = true;
                else if (i == Items.Count - 1)
                    skipDown = false;

                if (IsItemNonSelectable(Items[i]))
                {
                    i += skipDown ? 1 : -1;
                }
                else
                {
                    skipDown = true;
                    break;
                }
            }

            if (SelectedIndex != i)
            {
                //Calling this resends the event - you don't want to call the base
                //event or any registered handlers will be triggered twice
                SelectedIndex = i;
                return;
            }

            base.OnSelectedIndexChanged(e);
        }

        /// <summary>
        /// We need to prevent the mouse click from occuring when the user clicks
        /// on certain objects. The combo box creates another window for the dropdown
        /// list so we cannot use it's wndproc. When this other window is created
        /// at run time, we are told the window handle (LParam on WM_PARENTNOTIFY),
        /// so we then replace this window's wndproc with our own (ReplacementWndProc)
        /// </summary>
        /// <param name="m"></param>
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
            if (msg == Win32.WM_LBUTTONDOWN || msg == Win32.WM_LBUTTONDBLCLK)
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
                        if (IsItemNonSelectable(Items[index]))
                            return IntPtr.Zero;
                    }
                }
            }
            return Win32.CallWindowProc(oldWndProc, hWnd, msg, wParam, lParam);
        }
    }
}
