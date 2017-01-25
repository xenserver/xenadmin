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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;

namespace XenAdmin.Controls
{
    public partial class FlickerFreePanel : Panel
    {
        private Color _borderColor = Color.Black;
        private int _borderWidth = 1;

        public int BorderWidth
        {
            get { return _borderWidth; }
            set { _borderWidth = value; }
        }

        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        public FlickerFreePanel()
        {
            this.InitializeComponent();

            SetStyle
            (ControlStyles.DoubleBuffer,
                true);
            SetStyle
            (ControlStyles.AllPaintingInWmPaint,
                false);
            SetStyle
            (ControlStyles.ResizeRedraw,
                true);
            SetStyle
            (ControlStyles.UserPaint,
                true);
            SetStyle
            (ControlStyles.SupportsTransparentBackColor,
                true);

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.BorderStyle == BorderStyle.FixedSingle)
            {
                IntPtr hDC = Win32.GetWindowDC(this.Handle);
                Graphics g = Graphics.FromHdc(hDC);

                ControlPaint.DrawBorder(
                    g,
                    new Rectangle(0, 0, this.Width, this.Height),
                    _borderColor,
                    _borderWidth,
                    ButtonBorderStyle.Solid,
                    _borderColor,
                    _borderWidth,
                    ButtonBorderStyle.Solid,
                    _borderColor,
                    _borderWidth,
                    ButtonBorderStyle.Solid,
                    _borderColor,
                    _borderWidth,
                    ButtonBorderStyle.Solid);
                g.Dispose();
                Win32.ReleaseDC(Handle, hDC);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if (this.BorderStyle == BorderStyle.FixedSingle)
            {
                IntPtr hDC = Win32.GetWindowDC(this.Handle);
                Graphics g = Graphics.FromHdc(hDC);

                ControlPaint.DrawBorder(
                    g,
                    new Rectangle(0, 0, this.Width, this.Height),
                    _borderColor,
                    _borderWidth,
                    ButtonBorderStyle.Solid,
                    _borderColor,
                    _borderWidth,
                    ButtonBorderStyle.Solid,
                    _borderColor,
                    _borderWidth,
                    ButtonBorderStyle.Solid,
                    _borderColor,
                    _borderWidth,
                    ButtonBorderStyle.Solid);
                g.Dispose();
                Win32.ReleaseDC(Handle, hDC);
            }
        }
    }
}
