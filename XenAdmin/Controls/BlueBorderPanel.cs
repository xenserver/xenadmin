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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using XenAdmin.Core;

namespace XenAdmin.Controls
{
    /// <summary>
    /// Taken from 
    /// http://209.85.165.104/search?q=cache:hnUUN2Zhi7YJ:www.developersdex.com/vb/message.asp%3Fp%3D2927%26r%3D5855234+NativeMethods.GetDCEx&hl=en&ct=clnk&cd=1&gl=uk&client=firefox-a
    /// </summary>
    public class BlueBorderPanel : DoubleBufferedPanel
    {
        private Color borderColor = Drawing.XPBorderColor;

        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    NativeMethods.SendMessage(this.Handle,
                        NativeMethods.WM_NCPAINT,
                        (IntPtr)1, IntPtr.Zero);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            NativeMethods.SendMessage(this.Handle,
                NativeMethods.WM_NCPAINT, (IntPtr)1, IntPtr.Zero);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_NCPAINT)
            {
                if (this.Parent != null)
                {
                    NCPaint();
                    m.WParam = GetHRegion();
                    base.DefWndProc(ref m);
                    NativeMethods.DeleteObject(m.WParam);
                    m.Result = (IntPtr)1;
                }
            }
            base.WndProc(ref m);
        }

        private void NCPaint()
        {
            if (this.Parent == null)
                return;
            if (this.Width <= 0 || this.Height <= 0)
                return;

            IntPtr windowDC = NativeMethods.GetDCEx(this.Handle,
                             IntPtr.Zero, NativeMethods.DCX_CACHE |
                             NativeMethods.DCX_WINDOW |
                             NativeMethods.DCX_CLIPSIBLINGS |
                             NativeMethods.DCX_LOCKWINDOWUPDATE);

            if (windowDC.Equals(IntPtr.Zero))
                return;

            using (Bitmap bm = new Bitmap(this.Width, this.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb))
            {

                using (Graphics g = Graphics.FromImage(bm))
                {

                    Rectangle borderRect = new Rectangle(0, 0,
                        Width - 1, Height - 1);

                    using (Pen borderPen = new Pen(this.borderColor, 1))
                    {
                        borderPen.Alignment = PenAlignment.Inset;
                        g.DrawRectangle(borderPen, borderRect);
                    }

                    // Create and Apply a Clip Region to the WindowDC
                    using (Region Rgn = new Region(new
                        Rectangle(0, 0, Width, Height)))
                    {
                        Rgn.Exclude(new Rectangle(1, 1, Width - 2, Height - 2));
                        IntPtr hRgn = Rgn.GetHrgn(g);
                        if (!hRgn.Equals(IntPtr.Zero))
                            NativeMethods.SelectClipRgn(windowDC, hRgn);

                        IntPtr bmDC = g.GetHdc();
                        IntPtr hBmp = bm.GetHbitmap();
                        IntPtr oldDC = NativeMethods.SelectObject(bmDC,
                            hBmp);
                        NativeMethods.BitBlt(windowDC, 0, 0, bm.Width,
                            bm.Height, bmDC, 0, 0, NativeMethods.SRCCOPY);

                        NativeMethods.SelectClipRgn(windowDC, IntPtr.Zero);
                        NativeMethods.DeleteObject(hRgn);

                        g.ReleaseHdc(bmDC);
                        NativeMethods.SelectObject(oldDC, hBmp);
                        NativeMethods.DeleteObject(hBmp);
                        bm.Dispose();
                    }
                }
            }

            NativeMethods.ReleaseDC(this.Handle, windowDC);

        }

        private IntPtr GetHRegion()
        {
            //Define a Clip Region to pass back to WM_NCPAINTs wParam.
            //Must be in Screen Coordinates.
            IntPtr hRgn;
            Rectangle winRect = this.Parent.RectangleToScreen(this.Bounds);
            Rectangle clientRect =
                this.RectangleToScreen(this.ClientRectangle);

            Region updateRegion = new Region(winRect);
            updateRegion.Complement(clientRect);

            using (Graphics g = this.CreateGraphics())
                hRgn = updateRegion.GetHrgn(g);
            updateRegion.Dispose();
            return hRgn;
        }

    }

    internal class NativeMethods
    {

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg,
                                                IntPtr wParam,
                                                IntPtr lParam);

        [DllImport("gdi32.dll")]
        public static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc,
                                                 IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdcDest,
                                         int nXDest, int nYDest,
                                         int nWidth, int nHeight,
                                         IntPtr hdcSrc,
                                         int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip,
                                            int flags);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public const int WM_NCPAINT = 0x85;

        public const int DCX_WINDOW = 0x1;
        public const int DCX_CACHE = 0x2;
        public const int DCX_CLIPCHILDREN = 0x8;
        public const int DCX_CLIPSIBLINGS = 0x10;
        public const int DCX_LOCKWINDOWUPDATE = 0x400;

        public const int SRCCOPY = 0xCC0020;

    }
}