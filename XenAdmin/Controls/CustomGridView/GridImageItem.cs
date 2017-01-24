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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Core;

namespace XenAdmin.Controls.CustomGridView
{
    public delegate Image ImageDelegate();

    public class GridImageItem : GridItemBase
    {
        private readonly Object rankObject;
        
        /// <summary>
        /// Precisely one of imageDelegate or image should be non-null.
        /// </summary>
        private readonly ImageDelegate imageDelegate;
        private readonly Image image;
        private readonly HorizontalAlignment hAlign;
        private readonly VerticalAlignment vAlign;

        private GridImageItem(Object rankObject, Image image, ImageDelegate imageDelegate, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool clickSelectsRow, EventHandler onDoubleClickDelegate)
            : base(false, 1, clickSelectsRow, null, onDoubleClickDelegate)
        {
            this.rankObject = rankObject;
            this.imageDelegate = imageDelegate;
            this.image = image;
            this.vAlign = vAlign;
            this.hAlign = hAlign;
        }

        public GridImageItem(Object rankObject, Image image, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool clickSelectsRow)
            : this(rankObject, image, null, hAlign, vAlign, clickSelectsRow, null)
        {
        }

        public GridImageItem(Object rankObject, ImageDelegate imageDelegate, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool clickSelectsRow)
            : this(rankObject, null, imageDelegate, hAlign, vAlign, clickSelectsRow, null)
        {
        }

        public GridImageItem(Object rankObject, Image image, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool clickSelectsRow, EventHandler onDoubleClickDelegate)
            : this(rankObject, image, null, hAlign, vAlign, clickSelectsRow, onDoubleClickDelegate)
        {
        }

        public GridImageItem(Object rankObject, ImageDelegate imageDelegate, HorizontalAlignment hAlign, VerticalAlignment vAlign, bool clickSelectsRow, EventHandler onDoubleClickDelegate)
            : this(rankObject, null, imageDelegate, hAlign, vAlign, clickSelectsRow, onDoubleClickDelegate)
        {
        }

        public override void OnPaint(ItemPaintArgs itemPaintArgs)
        {
            Image im = image == null ? imageDelegate() : image;
            if (im == null)
                return;

            Point loc = itemPaintArgs.Rectangle.Location;

            if (hAlign == HorizontalAlignment.Center)
            {
                loc.X += ((itemPaintArgs.Rectangle.Width / 2) - (im.Width / 2));
            }
            else if (hAlign == HorizontalAlignment.Right)
            {
                loc.X += (itemPaintArgs.Rectangle.Width - im.Width);
            }
            
            if (vAlign == VerticalAlignment.Middle)
            {
                loc.Y += ((itemPaintArgs.Rectangle.Height / 2) - (im.Height / 2));
            }

            if(im.Width <= itemPaintArgs.Rectangle.Width)
                itemPaintArgs.Graphics.DrawImage(im, loc.X, loc.Y, im.Width, im.Height);
            else
                itemPaintArgs.Graphics.DrawImage(im, new Rectangle(itemPaintArgs.Rectangle.Left, loc.Y, itemPaintArgs.Rectangle.Width, im.Height), new Rectangle(0, 0, itemPaintArgs.Rectangle.Width, im.Height), GraphicsUnit.Pixel);
        }

        public override int CompareTo(GridItemBase other)
        {
            GridImageItem imageItem = other as GridImageItem;
            if (imageItem == null)
                return -1;

            return StringUtility.NaturalCompare(rankObject.ToString(), imageItem.rankObject.ToString());
        }
    }
}
