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

using System.Windows.Forms;
using XenCenterLib;

namespace XenAdmin.Controls
{
    /// <summary>
    /// Extension of <see cref="TextBox"/> which implements extra event handlers.
    /// Currently:
    /// <list type="table">
    ///     <item>
    ///         <term>OnScrollChange</term>
    ///         <description>Called when user initiates scrolling</description>
    ///     </item>
    /// </list>
    /// </summary>
    internal class SmartScrollTextBox : TextBox
    {
        private bool _scrolledProgrammatically;

        protected internal bool IsVerticalScrollBarAtBottom => GetPositionFromCharIndex(Text.Length - 1).Y < Height;

        public void SetTextWithoutScrolling(string text)
        {
            var oldVerticalPosition = Win32.GetScrollBarPosition(Handle, Win32.ScrollBarConstants.SB_VERT);
            Text = text;
            Win32.SetScrollPos(Handle, (int)Win32.ScrollBarConstants.SB_VERT, oldVerticalPosition, true);

            _scrolledProgrammatically = true;
            Win32.PostMessageA(Handle, Win32.WM_VSCROLL, (int)Win32.ScrollBarCommands.SB_THUMBPOSITION + 0x10000 * oldVerticalPosition, 0);
        }

        #region Custom Events

        /// <summary>
        /// Delegate for <see cref="OnScrollChange"/> which is fired when the user initiates scrolling.
        /// <br />
        /// If the event is fired by <see cref="Keys.PageUp"/> or <see cref="Keys.PageDown"/> , consider hooking into the <see cref="Control.KeyUp"/> event, as the scrollbar position
        /// changes after the keys have been pressed.
        /// </summary>
        /// <param name="newPosition">The new position of the scrollbar.</param>
        /// <param name="scrollBarOrientation">The orientation of the scrollbar.</param>
        public delegate void ScrollChange(int newPosition, Orientation scrollBarOrientation);

        /// <summary>
        /// Event called when user initiates scrolling in the following ways:
        /// <list type="bullet">
        ///     <item>Moves the mouse wheel. This also supports horizontal scrolling (performed by holding shift)</item>
        ///     <item>Drags the scrollbar</item>
        ///     <item>Presses <see cref="Keys.PageUp"/> or <see cref="Keys.PageDown"/> keys</item>
        ///     <item>Presses <see cref="Keys.Up"/> or <see cref="Keys.Down"/> keys</item>
        /// </list>
        /// </summary>
        public event ScrollChange OnScrollChange;
        #endregion
        
        #region Control Overrides
        protected override void WndProc(ref Message m)
        {
            if (OnScrollChange != null && !_scrolledProgrammatically)
            {
                int newPosition;
                switch (m.Msg)
                {
                    case Win32.WM_VSCROLL:
                        newPosition = Win32.GetScrollBarPosition(Handle, Win32.ScrollBarConstants.SB_VERT);
                        OnScrollChange.Invoke(newPosition, Orientation.Vertical);
                        break;
                    case Win32.WM_HSCROLL:
                        newPosition = Win32.GetScrollBarPosition(Handle, Win32.ScrollBarConstants.SB_HORZ);
                        OnScrollChange.Invoke(newPosition, Orientation.Vertical);
                        break;
                    case Win32.WM_MOUSEWHEEL:
                        // holding shift is horizontal scrolling
                        newPosition = Win32.GetScrollBarPosition(Handle, ModifierKeys == Keys.Shift ? Win32.ScrollBarConstants.SB_HORZ : Win32.ScrollBarConstants.SB_VERT);
                        OnScrollChange.Invoke(newPosition, ModifierKeys == Keys.Shift ? Orientation.Horizontal : Orientation.Vertical);
                        break;

                }
            }

            if (_scrolledProgrammatically)
            {
                _scrolledProgrammatically = false;
            }

            base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (OnScrollChange != null && (m.Msg == Win32.WM_KEYDOWN || m.Msg == Win32.WM_SYSKEYDOWN))
            {
                switch (keyData)
                {
                    case Keys.PageDown:
                    case Keys.PageUp:
                    case Keys.Down:
                    case Keys.Up:
                        OnScrollChange?.Invoke(Win32.GetScrollBarPosition(Handle, Win32.ScrollBarConstants.SB_VERT), Orientation.Vertical);
                        break;
                }
            }
            return base.ProcessCmdKey(ref m, keyData);
        }
        #endregion
    }
}
