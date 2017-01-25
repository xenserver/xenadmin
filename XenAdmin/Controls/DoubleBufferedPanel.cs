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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XenAdmin.Core;

namespace XenAdmin.Controls
{
    [Designer("System.Windows.Forms.Design.UserControlDocumentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.ComponentModel.Design.IRootDesigner))]
    [Designer("System.Windows.Forms.Design.ControlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [DesignerCategory("UserControl")]
    public partial class DoubleBufferedPanel : Panel
    {
        private Bitmap BackBuffer;
        private Graphics BufferGraphics;

        public DoubleBufferedPanel()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint, true);
            InitializeBuffer();
        }

        private void DisposeBuffer()
        {
            if (BufferGraphics != null)
            {
                BufferGraphics.Dispose();
                BufferGraphics = null;
            }

            if (BackBuffer != null)
            {
                BackBuffer.Dispose();
                BackBuffer = null;
            }
        }

        private void InitializeBuffer()
        {
            Trace.Assert(BackBuffer == null,string.Format("{0}: BackBuffer should be null before initializing",Name));
            Trace.Assert(BufferGraphics == null, string.Format("{0}: BufferGraphics should be null before initializing", Name));

            BackBuffer = new Bitmap(ClientSize.Width > 1 ? ClientSize.Width : 1, ClientSize.Height > 1 ? ClientSize.Height : 1);

            BufferGraphics = Graphics.FromImage(BackBuffer);
            BufferGraphics.TextRenderingHint = Drawing.TextRenderingHint;
            BufferGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            BufferGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            BufferGraphics.Clear(BackColor);

            RefreshBuffer();
        }

        private bool valid = false;
        private bool visible = false;

        public void RefreshBuffer()
        {
            if (!visible)
            {
                valid = false;
                return;
            }

            if (Disposing || IsDisposed || Program.Exiting)
                return;

            valid = true;

            Rectangle r = new Rectangle(Point.Empty, ClientSize);
            base.OnPaintBackground(new PaintEventArgs(BufferGraphics, r));
            OnDrawToBuffer(new PaintEventArgs(BufferGraphics, r));

            Refresh();
        }
        
        [Browsable(true)]
        public event PaintEventHandler DrawToBuffer;

        protected virtual void OnDrawToBuffer(PaintEventArgs paintEventArgs)
        {
            if(DrawToBuffer != null)
                DrawToBuffer(this, paintEventArgs);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Disposing || IsDisposed || Program.Exiting)
                return;

            if (!valid)
            {
                visible = true;
                RefreshBuffer();
            }

            Drawing.QuickDraw(e.Graphics, BackBuffer);
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnResize(e);

            DisposeBuffer();
            InitializeBuffer();
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se); 
            RefreshBuffer();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            RefreshBuffer();
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            base.OnParentBackColorChanged(e);
            RefreshBuffer();
        }

        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);
            RefreshBuffer();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            visible = Visible;

            base.OnVisibleChanged(e);
            RefreshBuffer();
        }
    }
}
