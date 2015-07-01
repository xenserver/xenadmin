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
using XenAdmin.Actions;
using XenAPI;


namespace XenAdmin.Dialogs.CallHome
{
    public partial class CallHomeEnrollNowDialog : XenDialogBase
    {
        private readonly Pool pool;
        private bool authenticated;

        public CallHomeEnrollNowDialog(Pool pool)
        {
            this.pool = pool;
            InitializeComponent();
            InitializeControls();
            UpdateButtons();
        }

        private void InitializeControls()
        {
            authenticated = false;

            Text = String.Format(Messages.CALLHOME_ENROLLMENT_TITLE, pool.Name);
            authenticationRubricLabel.Text = Messages.CALLHOME_AUTHENTICATION_RUBRIC_NO_TOKEN;
            callHomeAuthenticationPanel1.Pool = pool;
        }

        private void UpdateButtons()
        {
            okButton.Enabled = authenticated;
            okButton.Text = !authenticated
                ? Messages.OK
                : Messages.CALLHOME_ENROLLMENT_CONFIRMATION_BUTTON_LABEL;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var newCallHomeSettings = pool.CallHomeSettings;
            newCallHomeSettings.Status = CallHomeStatus.Enabled;
            var token = newCallHomeSettings.GetSecretyInfo(pool.Connection, CallHomeSettings.UPLOAD_TOKEN_SECRET);
            var user = newCallHomeSettings.GetSecretyInfo(pool.Connection, CallHomeSettings.UPLOAD_CREDENTIAL_USER_SECRET);
            var password = newCallHomeSettings.GetSecretyInfo(pool.Connection, CallHomeSettings.UPLOAD_CREDENTIAL_PASSWORD_SECRET);
            new SaveCallHomeSettingsAction(pool, newCallHomeSettings, token, user, password, false).RunAsync();
            new TransferCallHomeSettingsAction(pool, newCallHomeSettings, user, password, false).RunAsync();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        
        private void callHomeAuthenticationPanel1_AuthenticationChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, delegate
            {
                if (callHomeAuthenticationPanel1.Authenticated)
                {
                    authenticated = true;
                }
                UpdateButtons();
            });
        }
    }
}