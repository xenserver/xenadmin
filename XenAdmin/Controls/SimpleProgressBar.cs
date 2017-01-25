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

using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.Core;
using System;
using System.Drawing;

namespace XenAdmin.Controls
{
    internal partial class SimpleProgressBar : Control
    {
        private double _progress;
        private Drawing.SimpleProgressBarColor _color;

        public SimpleProgressBar()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            var rect = new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
            
            Drawing.DrawSimpleProgressBar(pe.Graphics, rect, Progress, Color);

            base.OnPaint(pe);
        }

        [DefaultValue(Drawing.SimpleProgressBarColor.Green)]
        public Drawing.SimpleProgressBarColor Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                Invalidate();
            }
        }

        [DefaultValue(0)]
        public double Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                if (value < 0 || value > 1.0)
                {
                    throw new ArgumentException("value out of range", "value");
                }
                _progress = value;
                Invalidate();
            }
        }
    }
}
