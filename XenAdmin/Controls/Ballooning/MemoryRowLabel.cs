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
using System.Drawing;
using System.Windows.Forms;

using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Controls.Ballooning
{
    public partial class MemoryRowLabel : UserControl
    {
        const int INTRA_GAP = 7;   // Horizontal gap between the icon and the text
        const int INTER_GAP = 15;  // Horizontal gap between one item and the next
        const int INTER_ROW = 7;   // Vertical gap between adjacent rows

        public MemoryRowLabel()
        {
            InitializeComponent();
        }

        IXenObject[] objs = new IXenObject[0];
        private bool expanded;
        private bool Expanded
        {
            set
            {
                expanded = value;
                Refresh();
            }
        }
        public bool MultiLine
        {
            get
            {
                return linkHide.Visible;
            }
        }

        public void Initialize(bool expanded, params IXenObject[] objs)
        {
            UnsubscribeEvents();
            this.objs = objs;
            SubscribeEvents();
            Expanded = expanded;
        }

        private void SubscribeEvents()
        {
            foreach (IXenObject o in objs)
            {
                if (o is VM)
                    o.PropertyChanged += vm_PropertyChanged;
                else if (o is Host)
                    o.PropertyChanged += host_PropertyChanged;
                else
                    System.Diagnostics.Trace.Assert(false);
            }
        }

        internal void UnsubscribeEvents()
        {
            foreach (IXenObject o in objs)
            {
                if (o is VM)
                    o.PropertyChanged -= vm_PropertyChanged;
                else if (o is Host)
                    o.PropertyChanged -= host_PropertyChanged;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int ellipsisWidth = Drawing.MeasureText(g, Messages.ELLIPSIS, Font, TextFormatFlags.NoPadding).Width;
            int showLinksLeft = linkShowAll.Left;
            if (linkHide.Left < showLinksLeft)
                showLinksLeft = linkHide.Left;

            int x = Padding.Left;
            int y = Padding.Top;
            bool firstRow = true;
            int height = 0;
            int i;
            for (i = 0; i < objs.Length; ++i)
            {
                IXenObject o = objs[i];
                bool first = (i == 0);
                bool last = (i == objs.Length - 1);
                string name = Helpers.GetName(o);
                if (!last)
                    name += ",";
                Image icon = Images.GetImage16For(o);
                int width = icon.Width + INTRA_GAP + Drawing.MeasureText(g, name, Font, TextFormatFlags.NoPadding).Width;
                if (icon.Height > height)
                    height = icon.Height;
                if (!last)
                    width += INTER_GAP;

                // Calculate the width we need to fit this item on.
                int availableWidth;
                // If this is the last item and we're still on the first row, we can go up to the end of the row.
                if (last && firstRow)
                    availableWidth = (this.Width - Padding.Right);
                // If we're expanded, we can go right up to the Show All link.
                else if (expanded)
                    availableWidth = showLinksLeft;
                // Otherwise, we need to be able to fit its width plus an ellipsis before the Show All link.
                else
                    availableWidth = showLinksLeft - ellipsisWidth - INTER_GAP;
                bool enoughRoom = (x + width <= availableWidth);

                // If there's not enough room, it depends what mode we're in. If we're not expanded, we draw an ellipsis and quit.
                // If we are expanded, we move on to the next row.
                if (!first && !enoughRoom)
                {
                    if (expanded)
                    {
                        x = Padding.Left;
                        y += height + INTER_ROW;
                        firstRow = false;
                        availableWidth = showLinksLeft;
                        enoughRoom = (x + width <= availableWidth);
                    }
                    else
                    {
                        Drawing.DrawText(g, Messages.ELLIPSIS, Font, new Rectangle(x, y, ellipsisWidth, icon.Height), ForeColor, TextFormatFlags.NoPadding);
                        break;
                    }
                }

                g.DrawImage(icon, new Rectangle(x, y, icon.Width, icon.Height));
                int textMaxWidth = availableWidth - x - icon.Width - INTRA_GAP;
                if (!enoughRoom)
                {
                    int nameWidth = textMaxWidth;
                    if (!last)
                        nameWidth -= INTER_GAP + Drawing.MeasureText(g, ",", Font, TextFormatFlags.NoPadding).Width;
                    name = name.Ellipsise(new Rectangle(0, 0, nameWidth, icon.Height), Font);
                    if (!last)
                        name += ",";
                    width = icon.Width + INTRA_GAP + Drawing.MeasureText(g, name, Font, TextFormatFlags.NoPadding).Width;  // actual width after ellipsisation
                    if (!last)
                        width += INTER_GAP;
                }
                Drawing.DrawText(g, name, Font, new Rectangle(x + icon.Width + INTRA_GAP, y, textMaxWidth, icon.Height), ForeColor, TextFormatFlags.NoPadding);
                x += width;
            }

            linkShowAll.Visible = (i < objs.Length);
            linkHide.Visible = !firstRow;
            this.Height = y + height + Padding.Bottom;
        }

        void vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label" || e.PropertyName == "current_operations")
                Refresh();
        }

        void host_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label")
                Refresh();
        }

        private void linkShowAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Expanded = true;
        }

        private void linkHide_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Expanded = false;
        }
    }
}
