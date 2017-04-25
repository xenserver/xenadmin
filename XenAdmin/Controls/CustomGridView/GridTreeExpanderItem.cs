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
using System.Windows.Forms.VisualStyles;
using XenAdmin.Core;

namespace XenAdmin.Controls.CustomGridView
{
    class GridTreeExpanderItem : GridItemBase
    {
        private const int IconWidth = 9;
        private const int IconHeight = 9;
        private const int TopMargin = 4;

        public GridTreeExpanderItem()
            : base(false, 1, false, null, null)
        {
        }

        public override void OnPaint(ItemPaintArgs itemPaintArgs)
        {
            if (!Row.HasChildren)
                return;  // nothing to draw

            Point loc = itemPaintArgs.Rectangle.Location;
            Rectangle rect = new Rectangle(loc.X, loc.Y + TopMargin, IconWidth, IconHeight);

            if (Application.RenderWithVisualStyles)
            {
                VisualStyleRenderer renderer = new VisualStyleRenderer(
                    Row.Expanded ?
                    VisualStyleElement.TreeView.Glyph.Opened :
                    VisualStyleElement.TreeView.Glyph.Closed);
                renderer.DrawBackground(itemPaintArgs.Graphics, rect);
            }

            else
            {
                itemPaintArgs.Graphics.DrawImage(Row.Expanded ? Properties.Resources.tree_minus : Properties.Resources.tree_plus, rect);
            }
        }

        private bool IsOverIcon(Point point)
        {
            return (Row.HasChildren &&
                point.X >= 0 && point.X < IconWidth &&
                point.Y >= TopMargin && point.Y < IconHeight + TopMargin); 
        }

        public override void OnClick(Point point)
        {
            if (IsOverIcon(point))
            {
                Row.Expanded = !Row.Expanded;
                Row.GridView.Refresh();
            }
        }
    }
}
