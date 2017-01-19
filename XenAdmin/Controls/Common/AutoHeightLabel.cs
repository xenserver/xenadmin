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
using System.Windows.Forms;

namespace XenAdmin.Controls.Common
{
    /// <summary>
    /// A label which automatically changes its height to accomodate its <see cref="Text"/> when in AutoSize mode. This is useful 
    /// as it enables a multi-line label to be put in a <see cref="TableLayoutPanel"/> - the table can then be made to vertically shrink/grow 
    /// depending on the label text. 
    /// </summary>
    public partial class AutoHeightLabel : Label
    {
        private const TextFormatFlags _flags = TextFormatFlags.WordBreak;
        private const int _maxHeight = 9999;

        public AutoHeightLabel()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            using (var g = Graphics.FromHwnd(Handle))
            {
                var textSize = TextRenderer.MeasureText(g, Text, Font, new Size(Width - Padding.Left - Padding.Right, _maxHeight), _flags);
                Height = textSize.Height + Padding.Top + Padding.Bottom;
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            using (var g = Graphics.FromHwnd(Handle))
            {
                var textSize = TextRenderer.MeasureText(g, Text, Font, new Size(proposedSize.Width - Padding.Left - Padding.Right, _maxHeight), _flags);
                return new Size(Width, textSize.Height + Padding.Top + Padding.Bottom);
            }
        }
    }
}
