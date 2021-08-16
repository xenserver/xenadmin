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
using XenAdmin.Core;
using XenCenterLib;


namespace XenAdmin.Dialogs.RestoreSession
{
    public partial class ChangeMainPasswordDialog : XenDialogBase
    {
        private readonly byte[] _currentPasswordHash;

        public ChangeMainPasswordDialog(byte[] currentPasswordHash)
        {
            InitializeComponent();
            _currentPasswordHash = currentPasswordHash;
            currentPasswordError.Visible = false;
            newPasswordError.Visible = false;
        }

        public byte[] NewPassword { get; private set; }

        private void currentTextBox_TextChanged(object sender, EventArgs e)
        {
            currentPasswordError.Visible = false;
        }

        private void mainTextBox_TextChanged(object sender, EventArgs e)
        {
            newPasswordError.Visible = false;
        }

        private void reEnterMainTextBox_TextChanged(object sender, EventArgs e)
        {
            newPasswordError.Visible = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var currentPasswordCorrect = !string.IsNullOrEmpty(currentTextBox.Text) &&
                                     Helpers.ArrayElementsEqual(EncryptionUtils.ComputeHash(currentTextBox.Text), _currentPasswordHash);

            if (currentPasswordCorrect && !string.IsNullOrEmpty(mainTextBox.Text) &&
                mainTextBox.Text == reEnterMainTextBox.Text)
            {
                NewPassword = EncryptionUtils.ComputeHash(mainTextBox.Text);
                DialogResult = DialogResult.OK;
                return;
            }

            if (!currentPasswordCorrect)
                currentPasswordError.ShowError(Messages.PASSWORD_INCORRECT);
            else if (mainTextBox.Text != reEnterMainTextBox.Text)
                newPasswordError.ShowError(Messages.PASSWORDS_DONT_MATCH);
            else
                newPasswordError.ShowError(Messages.PASSWORDS_EMPTY);

            currentTextBox.Focus();
            currentTextBox.SelectAll();
        }
    }
}
