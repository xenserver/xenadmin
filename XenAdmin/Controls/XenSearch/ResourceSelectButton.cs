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
using System.Windows.Forms;
using System.Drawing;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.XenSearch;
using XenAPI;

namespace XenAdmin.Controls.XenSearch
{
    public class ResourceSelectButton : DropDownComboButton, IAcceptGroups
    {
        private class ResourceRenderer : ToolStripProfessionalRenderer
        {
            private static int RIGHT_MARGIN = 2;
            private static int TOP_MARGIN = 2;

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                // The TextColor for a disabled item is ignored, so we have to temporarily enable the item.
                bool enabled = e.Item.Enabled;
                e.Item.Enabled = true;
                base.OnRenderItemText(e);
                e.Item.Enabled = enabled;

                // Draw an icon just before the text starts
                String s = e.Text;
                String sSpaces = String.Empty;  // sSpaces will be the spaces at the beginning of s
                for (int i = 0; i < s.Length && s[i] == ' '; ++i)
                    sSpaces += ' ';
                Size indent = Drawing.MeasureText(sSpaces, e.TextFont);
                Image im = Images.GetImage16For(e.Item.Tag as IXenObject);
                e.Graphics.DrawImage(im, indent.Width - im.Width - RIGHT_MARGIN, e.TextRectangle.Top + TOP_MARGIN, im.Width, im.Height);
            }
        }

        private static string INDENT = "       ";

        private QueryScope scope = null;
        private FolderListItem folderListItem = null;

        public ResourceSelectButton() : base()
        {
            contextMenuStrip.ShowImageMargin = false;
            contextMenuStrip.Renderer = new ResourceRenderer();
        }

        protected override void SetButtonTextFromItem(ToolStripItem item)
        {
            IXenObject o = item.Tag as IXenObject;
            if (o is Folder)  // for folders we want to show the full pathname using a FolderListItem
            {
                Text = String.Empty;
                folderListItem = new FolderListItem(o.opaque_ref, FolderListItem.AllowSearch.None, false);
                folderListItem.Parent = this;
            }
            else
            {
                Text = Helpers.GetName(o);
                folderListItem = null;
            }
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            if (folderListItem != null)
            {
                Rectangle r = pevent.ClipRectangle;
                r.Location = new Point(6, 4);
                r.Width -= 21;
                r.Height -= 6;
                folderListItem.DrawSelf(pevent.Graphics, r, false);
            }
        }

        public IAcceptGroups Add(Grouping grouping, object group, int indent)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            IXenObject o = group as IXenObject;
            if (o != null)
            {
                // Pad the item name with spaces on the left. This is nasty, but we
                // don't know any way to specify a width or padding in pixels and have
                // it affect the area on which we can write or the width of the tool
                // strip; so it seems necessary to fool the framework in this way.
                menuItem.Text = INDENT;
                for (int i = 0; i < indent; ++i)
                    menuItem.Text += INDENT;
                menuItem.Text += Helpers.GetName(o).EscapeAmpersands();
                menuItem.Tag = o;
                if (scope != null && !scope.WantType(o))
                {
                    menuItem.Enabled = false;
                    menuItem.BackColor = System.Drawing.Color.Gainsboro;
                    menuItem.ForeColor = DefaultForeColor;
                }
                AddItem(menuItem);
            }
            return this;
        }

        public void FinishedInThisGroup(bool defaultExpand)
        {
        }

        public void Populate(Search search)
        {
            ClearItems();
            if (search != null && search.Query != null)
                scope = search.Query.QueryScope;
            else
                scope = null;
            search.PopulateAdapters(this);
        }
    }
}