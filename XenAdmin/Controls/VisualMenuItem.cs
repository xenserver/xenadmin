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
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;
using System.Drawing;


/*
 * This class provides the option of including an extra image on a menu item
 * after the text.
 * ToolStripMenuItems do not take their width from get preferred size, it is
 * necessary to pad the text with blank characters so the second image can be
 * drawn.
 * 
 * Assumes all menu items are in the same menu for alignment.
 * 
 * 
 */
 
namespace XenAdmin.Controls
{
    public class VisualMenuItem : ToolStripMenuItem
    {

        private static int leftMargin = 35;
        private static int topMargin = 4;
        private static int imagePadding = 5;

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                UpdateWidths();
                VisualMenuItemAlignData.Refresh();
                RefreshPadding();
            }
        }

        private double starRating = 0;
        public double StarRating
        {
            get { return starRating; }
            set { starRating = value; }
        }

        private int textWidth = 0;
        public int TextWidth
        {
            get { return textWidth; }
        }

        private Image secondImage;
        public Image SecondImage
        {
            get { return secondImage; }
            set
            {
                secondImage = value;
                VisualMenuItemAlignData.Refresh();
                RefreshPadding();
                this.Invalidate();
            }
        }

        public VisualMenuItem()
        {
        }

        public VisualMenuItem(string Text)
            : base(Text)
        {
            // spacePadding will be set when text is assigned
        }

        public VisualMenuItem(string Text, Image Image)
            : base(Text, Image)
        {
            // spacePadding will be set when text/image is assigned
        }

        private string spacePadding = "";


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (secondImage == null)
                return;
            e.Graphics.DrawImage(secondImage,
                   leftMargin + VisualMenuItemAlignData.MaxStringLength + imagePadding, 
                   topMargin, secondImage.Width, secondImage.Height);

        }

        protected override void OnParentChanged(ToolStrip oldParent, ToolStrip newParent)
        {
            base.OnParentChanged(oldParent, newParent);
            if (HasAlignData())
            {
                VisualMenuItemAlignData.Refresh();
            }
            RefreshPadding();
        }

        private void UpdateWidths()
        {
            textWidth = Drawing.MeasureText(base.Text.TrimEnd(), this.Font).Width;
        }

        private bool HasAlignData()
        {
            // at the moment we just use a single static object, so no need to distinguish instance ownership
            return true;
        }

        public void RefreshPadding()
        {
            spacePadding = "";
            if (HasAlignData())
            {
                while (textWidth + Drawing.MeasureText(spacePadding, this.Font).Width < VisualMenuItemAlignData.MaxCombinedWidth)
                {
                    spacePadding += " ";
                }
            }
            base.Text = base.Text.TrimEnd();
            base.Text += spacePadding;
        }

        public int ImageWidth()
        {
            if (secondImage == null)
            {
                return 0;
            }
            return (secondImage.Width + imagePadding * 2);
        }
    }

    public static class VisualMenuItemAlignData
    {
        private static ToolStripMenuItem parentStrip;
        public static ToolStripMenuItem ParentStrip
        {
            get { return parentStrip; }
            set
            {
                parentStrip = value;
                Refresh();
            }
        }

        private static int maxStringLength;
        public static int MaxStringLength
        {
            get { return maxStringLength; }
        }

        private static int maxCombinedWidth;
        public static int MaxCombinedWidth
        {
            get { return maxCombinedWidth; }
        }

        private static Object locker = new Object();

        public static void Refresh()
        {
            maxCombinedWidth = 0;
            maxStringLength = 0;
            if (parentStrip == null)
            {
                return;
            }
            lock (locker)
            {
                int maxImageWidth = 0;
                foreach (ToolStripItem item in parentStrip.DropDownItems)
                {
                    VisualMenuItem visItem;
                    if ((visItem = item as VisualMenuItem) != null)
                    {
                        int w = visItem.ImageWidth();
                        if (w > maxImageWidth)
                        {
                            maxImageWidth = w;
                        }
                        if (visItem.TextWidth > maxStringLength)
                        {
                            maxStringLength = visItem.TextWidth;
                        }
                    }
                }
                maxCombinedWidth = maxImageWidth + maxStringLength;
                foreach (ToolStripItem item in parentStrip.DropDownItems)
                {
                    VisualMenuItem visItem;
                    if ((visItem = item as VisualMenuItem) != null)
                    {
                        visItem.RefreshPadding();
                    }
                }
            }     
        }
    }
    
}
