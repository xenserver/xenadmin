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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Collections;
using XenAdmin.Network.StorageLink;


namespace XenAdmin.Dialogs
{
    public partial class ChangeStorageLinkPasswordDialog : XenDialogBase
    {
        private StorageLinkConnection _connection;
        public ChangeStorageLinkPasswordDialog(StorageLinkConnection connection)
        {
            Util.ThrowIfParameterNull(connection, "connection");
            _connection = connection;
            
            InitializeComponent();
            UsernameTextBox.Text = "admin";
            label1.Text = string.Format(Messages.CHANGE_SL_SERVER_PASSWORD_DIALOG_TEXT, connection.Host);
        }

        protected override void OnShown(EventArgs e)
        {
            if (OldPasswordTextBox.CanSelect)
            {
                OldPasswordTextBox.Focus();
            }
            base.OnShown(e);
        }

        private void AnyTextBox_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = IsValidToSubmit();
        }

        private bool IsValidToSubmit()
        {
            if (UsernameTextBox.Text.Trim().Length == 0)
            {
                return false;
            }
            else if (OldPasswordTextBox.Text.Trim().Length == 0)
            {
                return false;
            }
            else if (NewPasswordTextBox.Text.Trim().Length == 0)
            {
                return false;
            }
            else if (ConfirmPasswordTextBox.Text.Trim().Length == 0)
            {
                return false;
            }
            else if (ConfirmPasswordTextBox.Text != NewPasswordTextBox.Text)
            {
                return false;
            }
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (IsValidToSubmit())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public string Username
        {
            get
            {
                return UsernameTextBox.Text.Trim();
            }
        }

        public string NewPassword
        {
            get
            {
                return NewPasswordTextBox.Text.Trim();
            }
        }

        public string OldPassword
        {
            get
            {
                return OldPasswordTextBox.Text.Trim();
            }
        }
    }
}