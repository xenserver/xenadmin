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
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Commands;

namespace XenAdmin.Dialogs
{
    public partial class SelectHostDialog : XenDialogBase
    {
        public string HelpString = "License"; // don't i18n

        public SelectHostDialog()
        {
            InitializeComponent();
            poolHostPicker1.AllowPoolSelect = false;
            poolHostPicker1.SupressErrors = true;
            poolHostPicker1.SelectedItemChanged += selectionChanged;
            poolHostPicker1.buildList();
        }

        public string TopBlurb
        {
            set { label1.Text = value; }
        }

        /// <summary>
        /// set image, must be 32x32
        /// </summary>
        public Image TopPicture
        {
            set { pictureBox1.Image = value; }
        }

        public string OkButtonText
        {
            set { okbutton.Text = value; }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new AddHostCommand(Program.MainWindow, this).Execute();
        }

        public Host TheHost
        {
            get
            {
                return poolHostPicker1.ChosenHost;
            }
            set
            {
                poolHostPicker1.SelectHost(value);
            }
        }

        private void selectionChanged(object sender, XenAdmin.Controls.SelectedItemEventArgs e)
        {
            okbutton.Enabled = e.SomethingSelected;
        }

        private void okbutton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelbutton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        internal override string HelpName
        {
            get
            {
                return Name + HelpString;
            }
        }
    }
}