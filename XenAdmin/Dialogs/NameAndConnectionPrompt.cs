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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using System.Text.RegularExpressions;

namespace XenAdmin.Dialogs
{
    public partial class NameAndConnectionPrompt : XenDialogBase
    {
        public NameAndConnectionPrompt()
        {
            InitializeComponent();
        }

        protected override void  OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (IXenConnection connection in ConnectionsManager.XenConnections)
            {
                if (!connection.IsConnected)
                    continue;

                Pool pool = Helpers.GetPool(connection);
                if (pool != null)
                {
                    comboBox.Items.Add(pool);
                    continue;
                }

                Host master = Helpers.GetMaster(connection);
                if (master != null)
                {
                    comboBox.Items.Add(master);
                    continue;
                }
            }

            comboBox.Sorted = true;

            if (comboBox.Items.Count > 0)
            {
                // check to see if the user is readonly on connections, try and choose a sensible default
                int nonReadOnlyIndex = -1;
                for (int i = 0; i < comboBox.Items.Count; i++)
                {
                    IXenObject xo = comboBox.Items[i] as IXenObject;
                    if (xo != null 
                        && (xo.Connection.Session.IsLocalSuperuser 
                            || !XenAdmin.Commands.CrossConnectionCommand.IsReadOnly(xo.Connection)))
                    {
                        nonReadOnlyIndex = i;
                        break;
                    }
                }
                if (nonReadOnlyIndex == -1)
                    comboBox.SelectedIndex = 0;
                else
                    comboBox.SelectedIndex = nonReadOnlyIndex;
            }
                

            UpdateOK();
        }

        public String PromptedName
        {
            get
            {
                return textBox.Text;
            }

            set
            {
                textBox.Text = value;
            }
        }

        public string OKText
        {
            set
            {
                okButton.Text = value;
            }
        }

        private string helpID = null;
        internal string HelpID
        {
            set
            {
                helpID = value;
            }
        }
        internal override string HelpName
        {
            get
            {
                return helpID ?? base.HelpName;
            }
        }

        public IXenConnection Connection
        {
            get
            {
                IXenObject o = comboBox.SelectedItem as IXenObject;
                if (o == null)
                    return null;

                return o.Connection;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateOK();
        }

        private Regex invalid_folder = new Regex("^[ /]+$");

        private void UpdateOK()
        {
            okButton.Enabled = !String.IsNullOrEmpty(textBox.Text.Trim()) && !invalid_folder.IsMatch(textBox.Text);
        }

        private const int PADDING = 1;

        private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null)
                return;

            if (e.Index < 0 || e.Index >= comboBox.Items.Count)
                return;

            Graphics g = e.Graphics;

            e.DrawBackground();

            IXenObject o = comboBox.Items[e.Index] as IXenObject;
            if (o == null)
                return;
            
            Image image = Images.GetImage16For(o);

            Rectangle bounds = e.Bounds;

            if (image != null)
                g.DrawImage(image, bounds.X + PADDING, bounds.Y + PADDING,
                    bounds.Height - 2 * PADDING, bounds.Height - 2 * PADDING);

            String name = Helpers.GetName(o).Ellipsise(50);

            e.DrawFocusRectangle();

            if (name != null)
                using (Brush brush = new SolidBrush(e.ForeColor))
                    g.DrawString(name, Program.DefaultFont, brush, 
                        new Rectangle(bounds.X + bounds.Height, bounds.Y, 
                        bounds.Width - bounds.Height, bounds.Height));
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}