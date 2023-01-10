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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.GradientPanel;
using XenAdmin.Network;

namespace XenAdmin.Dialogs
{
    public partial class VerticallyTabbedDialog : XenDialogBase
    {
        private Font titleFont = new Font(DefaultFont.FontFamily, DefaultFont.Size + 1.75f, FontStyle.Bold);

        // Void constructor for the designer
        public VerticallyTabbedDialog()
        {
            Init();
        }

        public VerticallyTabbedDialog(IXenConnection connection)
            : base(connection)
        {
            Init();
        }

        private void Init()
        {
            InitializeComponent();

            TabTitle.ForeColor = HorizontalGradientPanel.TextColor;
            TabTitle.Font = titleFont;
        }

        public VerticalTabs.IVerticalTab[] Tabs => verticalTabs.Items.Cast<VerticalTabs.IVerticalTab>().ToArray();

        public VerticalTabs.IVerticalTab SelectedTab => verticalTabs.SelectedItem as VerticalTabs.IVerticalTab;

        protected void SelectPage(VerticalTabs.IVerticalTab page)
        {
            if (page == null || !verticalTabs.Items.Contains(page))
                return;

            verticalTabs.SelectedItem = page;
        }

        private void verticalTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ListBox listBox &&
                listBox.SelectedItem is VerticalTabs.IVerticalTab editPage &&
                editPage is Control control)
            {
                TabImage.Image = editPage.Image;
                TabTitle.Text = GetTabTitle(editPage);

                control.Show();
                control.BringToFront();

                foreach (Control other in ContentPanel.Controls)
                {
                    if (other != control)
                        other.Hide();
                }
            }
        }

        protected virtual string GetTabTitle(VerticalTabs.IVerticalTab verticalTab)
        {
            return verticalTab != null ? verticalTab.Text : string.Empty;
        }

        /// <summary>
        /// When in design mode, auto add tabs to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            if (!DesignMode)
                return;

            if (!(e.Control is VerticalTabs.IVerticalTab verticalTab))
                return;

            foreach (VerticalTabs.IVerticalTab vt in verticalTabs.Items)
                if(vt == verticalTab)
                    return;

            verticalTabs.Items.Add(verticalTab);
        }

        /// <summary>
        /// When in design mode, auto remove tabs from the list
        /// </summary>
        private void ContentPanel_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (!DesignMode)
                return;

            if (!(e.Control is VerticalTabs.IVerticalTab verticalTab))
                return;

            verticalTabs.Items.Remove(verticalTab);
        }
    }
}
