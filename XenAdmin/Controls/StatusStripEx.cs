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
using System.Windows.Forms;

namespace XenAdmin.Controls
{
    /// <summary>
    /// A System.Windows.Forms.StatusStrip with the option of click-through
    /// (see https://learn.microsoft.com/en-us/archive/blogs/rickbrew/how-to-enable-click-through-for-net-2-0-toolstrip-and-menustrip)
    /// </summary>
    public class StatusStripEx : StatusStrip
    {
        /// <summary>
        /// Gets or sets whether the StatusStripEx honors item clicks when its containing form does
        /// not have input focus.
        /// </summary>
        /// <remarks>
        /// Default value is false, which is the same behavior provided by the base StatusStrip class.
        /// </remarks>
        public bool ClickThrough { get; set; }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (ClickThrough &&
                m.Msg == NativeConstants.WM_MOUSEACTIVATE &&
                m.Result == (IntPtr)NativeConstants.MA_ACTIVATEANDEAT)
            {
                m.Result = (IntPtr)NativeConstants.MA_ACTIVATE;
            }
        }
    }
}
