/* Copyright (c) Citrix Systems Inc. 
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Model;

namespace XenAdmin.TabPages
{
    public partial class TagCloudPage : UserControl
    {
        public TagCloudPage()
        {
            InitializeComponent();
            this.SetStyle( ControlStyles.OptimizedDoubleBuffer, true);

            TitleLabel.ForeColor = Program.HeaderGradientForeColor;
            TitleLabel.Font = Program.HeaderGradientFont;

            OtherConfigAndTagsWatcher.TagsChanged += new EventHandler(OtherConfigAndTagsWatcher_TagsChanged);

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }
        public void LoadTags()
        {
           
            flowLayoutPanel.Controls.Clear();
            foreach (KeyValuePair<string, int> itemPair in Tags.GetTagCounts())
            {
                TagButton tagButton = new TagButton();
                tagButton.Text = itemPair.Key;
                int fontSize = Math.Min(41, itemPair.Value);
                tagButton.Font = new Font(tagButton.Font.Name, fontSize + 9);
                tagButton.MouseClick += new MouseEventHandler(tagButton_MouseClick);
                tagButton.KeyPress += new KeyPressEventHandler(tagButton_KeyPress);
                flowLayoutPanel.Controls.Add(tagButton);
                flowLayoutPanel.PerformLayout();
            }
        }

        void tagButton_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ClickAction(sender);
            }
        }
      

        void tagButton_MouseClick(object sender, MouseEventArgs e)
        {
            ClickAction(sender);
        }

        private void ClickAction(object sender)
        {
            TagButton tagButton = (TagButton) sender;
            Program.MainWindow.SearchForTag(tagButton.Text);
        }

        void OtherConfigAndTagsWatcher_TagsChanged(object sender, System.EventArgs e)
        {
            if (!this.Visible)
                return;
            LoadTags();
        }

        //public void Loader(IList<string> list)
        //{
        //    flowLayoutPanel.Controls.Clear();
        //    foreach (string item in list)
        //    {
        //        TagButton tagButton = new TagButton(false);
        //        tagButton.Text = item;
        //        tagButton.Font = new Font(tagButton.Font.Name,10);
        //        tagButton.MouseClick += new MouseEventHandler(tagButton_MouseClick);
        //        tagButton.KeyPress += new KeyPressEventHandler(tagButton_KeyPress);
        //        flowLayoutPanel.Controls.Add(tagButton);
        //        flowLayoutPanel.PerformLayout();
        //    }
        //}
    }
}
