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
using System.Drawing;
using System.Windows.Forms;

namespace XenAdmin.Controls.Common
{
    /// <summary>
    /// A RadioButton which automatically changes its height to accomodate its
    /// text when in AutoSize mode. This is useful as it enables a multi-line
    /// RadioButton to be put in a TableLayoutPanel. The table can then be made
    /// to vertically shrink/grow depending on the label text.
    /// Code based on http://blogs.msdn.com/b/jfoscoding/archive/2005/11/14/492554.aspx
    /// </summary>
    public class AutoHeightRadioButton : RadioButton
    {
        Size cachedSizeOfOneLineOfText = Size.Empty;
        Dictionary<Size, Size> preferredSizeHash = new Dictionary<Size, Size>();

        private void CacheTextSize()
        {
            preferredSizeHash.Clear();

            cachedSizeOfOneLineOfText = string.IsNullOrEmpty(Text)
                ? Size.Empty
                : TextRenderer.MeasureText(Text, Font, new Size(Int32.MaxValue, Int32.MaxValue), TextFormatFlags.WordBreak);
        }
        
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            CacheTextSize();
            //this seems to be necessary in non-EN environments so it can resize correctly
            PerformLayout();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            CacheTextSize();
            //this seems to be necessary in non-EN environments so it can resize correctly
            PerformLayout();
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size prefSize = base.GetPreferredSize(proposedSize);

            if (prefSize.Width > proposedSize.Width &&
                !string.IsNullOrEmpty(Text) &&
                (!proposedSize.Width.Equals(Int32.MaxValue) || !proposedSize.Height.Equals(Int32.MaxValue)))
            {
                Size bordersAndPadding = prefSize - cachedSizeOfOneLineOfText;
                Size newConstraints = proposedSize - bordersAndPadding;

                if (preferredSizeHash.ContainsKey(newConstraints))
                {
                    prefSize = preferredSizeHash[newConstraints];
                }
                else
                {
                    prefSize = bordersAndPadding + TextRenderer.MeasureText(Text, Font, newConstraints, TextFormatFlags.WordBreak);
                    preferredSizeHash[newConstraints] = prefSize;
                }
            }
            return prefSize;
        }
    }
}
