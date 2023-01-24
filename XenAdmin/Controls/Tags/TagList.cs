/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Text;
using System.Windows.Forms;
using XenAdmin.Dialogs;


namespace XenAdmin.Controls
{
    public class TagList : CustomListItem
    {
        private List<string> _tags;
        private LinkLabel linkDialogLabel = new LinkLabel();
        private Label tagsLabel;
        private PictureBox icon = new PictureBox();
        private Panel _panel=null;
        public TagList(List<string> tags, Panel panel, int width)
            : base(null, Program.DefaultFontBold, Color.White)
        {
            this._tags = tags;
            tagsLabel = new Label();
            tagsLabel.Width = width;

            icon.Image = Images.StaticImages._000_Tag_h32bit_16;
            icon.Size = Images.StaticImages._000_Tag_h32bit_16.Size;

            linkDialogLabel.Text = Messages.NEW_TAG_LINK;
            linkDialogLabel.TabStop = true;
            linkDialogLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(linkDialogLabel_LinkClicked);

            UpdateList();
            _panel = panel;
            panel.Controls.Add(tagsLabel);
            panel.Controls.Add(linkDialogLabel);
            panel.Controls.Add(icon);
            UpdateList();
        }

        public List<string> Tags
        {
            get { return _tags; }
        }

        private void linkDialogLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (NewTagDialog dialog = new NewTagDialog(_tags))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _tags = dialog.GetSelectedTags();
                    UpdateList();
                    DrawSelf(null, new Rectangle(0, 0, _panel.Width, _panel.Height), false);
                }
            }
        }

        public void UpdateList()
        {
            _tags.Sort();
            StringBuilder sb = new StringBuilder();
            foreach (string tag in _tags)
            {
                sb.Append(tag);
                sb.Append(", ");
            }

            if (_tags.Count > 0)
                sb.Remove(sb.Length - 2, 2);
            else
                sb.Append(Messages.NONE);

            tagsLabel.Text = sb.ToString();
        }

        public override void DrawSelf(Graphics g, Rectangle bounds, bool selected)
        {
            tagsLabel.Width = bounds.Width;
            tagsLabel.Location = bounds.Location;
            Size size = TextRenderer.MeasureText(tagsLabel.Text, tagsLabel.Font, new Size(tagsLabel.Width, Int32.MaxValue), TextFormatFlags.WordBreak);
            tagsLabel.Height = size.Height + 5;
            icon.Location = new Point(tagsLabel.Location.X + tagsLabel.Margin.Left, tagsLabel.Location.Y + tagsLabel.Height + 5);
            linkDialogLabel.Location = new Point(icon.Location.X + icon.Width + 2, icon.Location.Y);
        }

        internal override Size WrappedSize(int maxwidth)
        {

            return new Size(maxwidth, tagsLabel.Height + 5 + linkDialogLabel.Height);
        }

        public override Size PreferredSize
        {
            get
            {
                return WrappedSize(Int32.MaxValue);
            }
        }
    }
}
