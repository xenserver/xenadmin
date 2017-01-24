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
using XenAdmin.TabPages;
using XenAdmin.Core;

namespace XenAdmin.Controls
{
    public class CustomListItem
    {
        public CustomListRow Row;

        public string Text = "a_header";

        public object Tag;

        protected readonly Font Font;
        public AnchorStyles Anchor = AnchorStyles.None;

        public bool Enabled = true;
        public bool InCorrectColumn = true;

        public Padding itemBorder;

        public Color ForeColor = SystemColors.ControlText;
        public Color BackColor = SystemColors.Control;

        public Color DisabledForeColor = SystemColors.GrayText;
        public Color DisabledBackColor = SystemColors.ControlDark;

        public CustomListItem(object tag, Font font, Color fore)
            : this(tag, font, fore, new Padding(0))
        {
        }
        
        public CustomListItem(object tag, Font font, Color fore, Padding padding)
        {
            Tag = tag;
            Font = font;
            ForeColor = fore;
            itemBorder = padding;
        }

        public virtual void DrawSelf(Graphics g, Rectangle bounds, bool selected)
        {
            Rectangle rect = new Rectangle(bounds.Left + itemBorder.Left, bounds.Top + itemBorder.Top, bounds.Width - itemBorder.Horizontal, bounds.Height - itemBorder.Vertical);
            if (Tag == null)
                Drawing.DrawText(g, Text, Font, rect, !selected ? ForeColor : SystemColors.HighlightText);
            else if (Tag is Image)
            {
                Image im = Tag as Image;
                g.DrawImage(im, rect);
            }
            else
            {
                Drawing.DrawText(g, Tag.ToString(), Font, rect, 
                    !selected ? ForeColor : SystemColors.HighlightText, 
                    TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
            }
        }

        public virtual Size PreferredSize
        {
            get
            {
                Size theSize;
                if (Tag == null)
                {
                    theSize = Drawing.MeasureText(Row.ParentPanel.Graphics, Text, Font);
                }
                else if (Tag is Image)
                {
                    Image pic = Tag as Image;
                    theSize = new Size(pic.Width, pic.Height);
                }
                else
                {
                    theSize = Drawing.MeasureText(Row.ParentPanel.Graphics, Tag.ToString(), Font);
                }
                theSize.Width += this.itemBorder.Left + this.itemBorder.Right;
                theSize.Height += this.itemBorder.Top + this.itemBorder.Bottom;
                return theSize;
            }
        }

        internal virtual Size WrappedSize(int maxwidth)
        {
            if (!(Tag == null || Tag is Image || !InCorrectColumn))
            {
                Size theSize = Drawing.MeasureText(Row.ParentPanel.Graphics, Tag.ToString(), Font, new Size(maxwidth, Int32.MaxValue), TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
                theSize.Width += this.itemBorder.Left + this.itemBorder.Right;
                theSize.Height += this.itemBorder.Top + this.itemBorder.Bottom;
                return theSize;
            }
            else
            {
                return PreferredSize;
            }
        }

        public virtual void OnMouseClick(MouseEventArgs e, Point point)
        {
            if (e.Button != MouseButtons.Right ||
                Row == null ||
                Row.ParentPanel == null ||
                Row.ParentPanel.ContextMenuRequest == null)
                return;

            Row.ParentPanel.ContextMenuRequest(this, new ListPanelItemClickedEventArgs(this));
        }

        public virtual void OnMouseDoubleClick(MouseEventArgs e, Point point)
        {
        }

        public virtual void OnMouseEnter(Point point)
        {
        }

        public virtual void OnMouseMove(Point point)
        {
        }

        public virtual void OnMouseLeave()
        {
        }
    }
}
