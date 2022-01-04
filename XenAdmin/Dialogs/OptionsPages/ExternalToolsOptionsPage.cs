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

using System.Drawing;
using System.IO;
using System.Windows.Forms;
using XenAdmin.Core;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class ExternalToolsOptionsPage : UserControl, IOptionsPage
    {
        private TextBox _tooltipControl;
        public ExternalToolsOptionsPage()
        {
            InitializeComponent();
            sshConsoleInfoLabel.Text = string.Format(sshConsoleInfoLabel.Text, BrandManager.BrandConsole);

            var customSshConsole = Properties.Settings.Default.CustomSshConsole;

            radioButtonOpenSsh.Checked = customSshConsole == SshConsole.OpenSSH;
            radioButtonPutty.Checked = customSshConsole == SshConsole.Putty;
        }

        #region IOptionsPage Members

        public void Build()
        {

        }

        public bool IsValidToSave()
        {
            if (radioButtonPutty.Checked && !File.Exists(textBoxPutty.Text))
            {
                return false;
            }

            if (radioButtonOpenSsh.Checked && !File.Exists(textBoxOpenSsh.Text))
            {
                return false;
            }
            return true;
        }

        public void ShowValidationMessages()
        {
            if (radioButtonPutty.Checked && !File.Exists(textBoxPutty.Text))
            {
                _tooltipControl = textBoxPutty;
            }
            else if (radioButtonOpenSsh.Checked && !File.Exists(textBoxOpenSsh.Text))
            {
                _tooltipControl = textBoxOpenSsh;
            }

            if (_tooltipControl != null)
            {
                tooltipValidation.ToolTipTitle = Messages.FILE_NOT_FOUND;
                HelpersGUI.ShowBalloonMessage(_tooltipControl, tooltipValidation);
            }
        }

        public void HideValidationMessages()
        {
            if (_tooltipControl != null)
                tooltipValidation?.Hide(_tooltipControl);
        }

        public void Save()
        {
            Properties.Settings.Default.CustomSshConsole = radioButtonOpenSsh.Checked ? SshConsole.OpenSSH : SshConsole.Putty;
            Properties.Settings.Default.OpenSSHLocation = textBoxOpenSsh.Text;
            Properties.Settings.Default.PuttyLocation = textBoxPutty.Text;
        }

        #endregion

        #region IVerticalTab Members

        public override string Text => Messages.EXTERNAL_TOOLS;

        public string SubText => Messages.EXTERNAL_TOOLS_DETAILS;

        public Image Image => Images.StaticImages._001_Tools_h32bit_16;

        #endregion

        #region Event Handlers

        private void textBoxPutty_TextChanged(object sender, System.EventArgs e)
        {
            if (!radioButtonPutty.Checked)
                radioButtonPutty.Checked = true;
        }

        private void textBoxOpenSsh_TextChanged(object sender, System.EventArgs e)
        {
            if (!radioButtonOpenSsh.Checked)
                radioButtonOpenSsh.Checked = true;
        }

        private void buttonBrowsePutty_Click(object sender, System.EventArgs e)
        {
            OpenFileSelection(textBoxPutty);
        }

        private void buttonBrowseSsh_Click(object sender, System.EventArgs e)
        {
            OpenFileSelection(textBoxOpenSsh);
        }

        #endregion

        private void OpenFileSelection(TextBox textBox)
        {
            using (var dialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = Messages.EXTERNAL_TOOLS_OPEN_FILE_TITLE,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = Messages.EXTERNAL_TOOLS_OPEN_FILE_TYPE
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    textBox.Text = dialog.FileName;
                }
            }
        }
    }
}
