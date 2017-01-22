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
using System.Windows.Forms;

using XenAdmin.Actions;


namespace XenAdmin.Dialogs
{
    public partial class ChangeServerPasswordDialog : XenDialogBase
    {
        private readonly XenAPI.Host host;
        private readonly XenAPI.Pool pool;

        public ChangeServerPasswordDialog(XenAPI.Host host)
        {
            InitializeComponent();
            
            this.host = host;

            host.PropertyChanged += new PropertyChangedEventHandler(Server_PropertyChanged);

            UpdateText();
            checkConfirmEnablement();
        }

        public ChangeServerPasswordDialog(XenAPI.Pool pool)
        {
            InitializeComponent();

            this.pool = pool;

            pool.PropertyChanged += new PropertyChangedEventHandler(Server_PropertyChanged);

            UpdateText();
            checkConfirmEnablement();
        }

        private void Server_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "name_label")
            {
                Program.Invoke(this, UpdateText);
            }
        }

        private void UpdateText()
        {
            Program.AssertOnEventThread();

            string name = "";

            if (host != null)
            {
                XenAPI.Pool thePool = Core.Helpers.GetPoolOfOne(host.Connection);
                name = thePool.Name;
            }

            if (pool != null)
                name = pool.Name;

            this.Text = string.Format(Messages.CHANGEPASS_DIALOG_TITLE, name);
            ServerNameLabel.Text = string.Format(Messages.CHANGEPASS_ROOT_PASS, name);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            bool isOldPasswordCorrect = (host == null)
                    ? oldPassBox.Text == pool.Connection.Password
                    : oldPassBox.Text == host.Connection.Password;

            // Check old password was correct
            if (!isOldPasswordCorrect)
            {
                currentPasswordError.Visible = true;
                oldPassBox.Focus();
                oldPassBox.SelectAll();
                return;
            }

            // Check new passwords match
            if (!newPassBox.Text.Equals(confirmBox.Text))
            {
                newPasswordError.Visible = true;
                newPasswordError.Error = Messages.PASSWORDS_DONT_MATCH;
                newPassBox.Focus();
                newPassBox.SelectAll();
                return;
            }

            ChangeHostPasswordAction action = (host == null)
                    ? new ChangeHostPasswordAction(pool.Connection, oldPassBox.Text.ToCharArray(), newPassBox.Text.ToCharArray())
                    : new ChangeHostPasswordAction(host.Connection, oldPassBox.Text.ToCharArray(), newPassBox.Text.ToCharArray());
            action.RunAsync();

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (host != null)
                host.PropertyChanged -= new PropertyChangedEventHandler(Server_PropertyChanged);
            if (pool != null)
                pool.PropertyChanged -= new PropertyChangedEventHandler(Server_PropertyChanged);
            base.OnClosing(e);
        }

        private void oldPassBox_TextChanged(object sender, EventArgs e)
        {
            currentPasswordError.Visible = false;
            checkConfirmEnablement();
        }

        private void newPassBox_TextChanged(object sender, EventArgs e)
        {
            newPasswordError.Visible = false;
            checkConfirmEnablement();
        }

        private void confirmBox_TextChanged(object sender, EventArgs e)
        {
            newPasswordError.Visible = false;
            checkConfirmEnablement();
        }

        private void checkConfirmEnablement()
        {
            okButton.Enabled = oldPassBox.Text != "" && newPassBox.Text != "" && confirmBox.Text != "";
        }
    }
}
