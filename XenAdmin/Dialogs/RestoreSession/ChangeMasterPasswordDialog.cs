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

using XenAdmin.Core;


namespace XenAdmin.Dialogs.RestoreSession
{
    public partial class ChangeMasterPasswordDialog : XenDialogBase
    {
        private byte[] OldProposedPassword;
        private byte[] passHash;

        public ChangeMasterPasswordDialog(byte[] proposedPassword)
        {
            InitializeComponent();
            OldProposedPassword = proposedPassword;
            currentPasswordError.Visible = false;
            currentPasswordError.Error = Messages.PASSWORD_INCORRECT;
            newPasswordError.Visible = false;
        }

        public byte[] NewPassword
        {
            get
            {
                return passHash;
            }
        }

        private void currentTextBox_TextChanged(object sender, EventArgs e)
        {
            currentPasswordError.Visible = false;
        }

        private void masterTextBox_TextChanged(object sender, EventArgs e)
        {
            newPasswordError.Visible = false;
        }

        private void reEnterMasterTextBox_TextChanged(object sender, EventArgs e)
        {
            newPasswordError.Visible = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(masterTextBox.Text) && masterTextBox.Text == reEnterMasterTextBox.Text)
            {
                if (Settings.PassCorrect(currentTextBox.Text,OldProposedPassword))
                {
                    passHash = EncryptionUtils.ComputeHash(masterTextBox.Text);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    currentPasswordError.Visible = true;
                    currentTextBox.Focus();
                    currentTextBox.SelectAll();
                }
            }
            else if (masterTextBox.Text != reEnterMasterTextBox.Text)
            {
                newPasswordError.Error = Messages.PASSWORDS_DONT_MATCH;
                newPasswordError.Visible = true;
                masterTextBox.Focus();
                masterTextBox.SelectAll();
            }
            else
            {
                newPasswordError.Error = Messages.PASSWORDS_EMPTY;
                newPasswordError.Visible = true;
                masterTextBox.Focus();
                masterTextBox.SelectAll();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}