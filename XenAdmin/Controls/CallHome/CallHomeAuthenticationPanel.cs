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
using System.Windows.Forms;

using XenAPI;
using XenAdmin.Core;
using XenAdmin.Actions;


namespace XenAdmin.Controls
{
    public partial class CallHomeAuthenticationPanel : UserControl
    {
        public event EventHandler AuthenticationChanged;

        public Pool Pool { get; set; }

        private bool authenticated = false;
        internal bool Authenticated
        {
            get
            {
                return authenticated;
            }
            set
            {
                if (authenticated != value)
                {
                    authenticated = value;
                    if (AuthenticationChanged != null)
                        AuthenticationChanged(this, null);
                }
            }
        }

        public CallHomeAuthenticationPanel()
        {
            InitializeComponent();
            authenticateButton.Enabled = false;
        }

        private void authenticateButton_Click(object sender, EventArgs e)
        {
            HideAuthenticationStatusControls();

            spinnerIcon.StartSpinning();

            var action = new CallHomeAuthenticationAction(Pool, usernameTextBox.Text.Trim(), passwordTextBox.Text.Trim(),
                Registry.CallHomeIdentityTokenDomainName, Registry.CallHomeUploadGrantTokenDomainName, Registry.CallHomeUploadTokenDomainName,
                true, 0, false);
            action.Completed += CallHomeAuthenticationAction_Completed;
            authenticateButton.Enabled = false;
            action.RunAsync();
        }

        private void CallHomeAuthenticationAction_Completed(ActionBase action)
        {
            Program.Invoke(this, delegate
            {
                if (action.Succeeded)
                {
                    spinnerIcon.DisplaySucceededImage();
                    Authenticated = true;
                }
                else
                {
                    spinnerIcon.Visible = false;
                    statusPictureBox.Visible = statusLabel.Visible = true;

                    statusLabel.Text = action.Exception != null
                                           ? action.Exception.Message
                                           : Messages.ERROR_UNKNOWN;
                    Authenticated = false;
                }
                authenticateButton.Enabled = true;
            });

        }

        private void HideAuthenticationStatusControls()
        {
            statusPictureBox.Visible = statusLabel.Visible = false;
        }

        private void credentials_TextChanged(object sender, EventArgs e)
        {
            authenticateButton.Enabled = !string.IsNullOrEmpty(usernameTextBox.Text.Trim()) &&
                                         !string.IsNullOrEmpty(passwordTextBox.Text.Trim());
        }

        private void CallHomeAuthenticationPanel_EnabledChanged(object sender, EventArgs e)
        {
            if (!Enabled)
                HideAuthenticationStatusControls();
        }
    }
}
