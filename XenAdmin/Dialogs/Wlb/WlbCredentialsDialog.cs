/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Actions.Wlb;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;
using XenCenterLib;

namespace XenAdmin.Dialogs.Wlb
{
    public partial class WlbCredentialsDialog : XenDialogBase
    {
        private const int DEFAULT_WLB_PORT = 8012;
        private readonly Pool _pool;
        private readonly WlbServerState.ServerState _wlbServerState;
        private string _wlbUrl;

        public WlbCredentialsDialog(Pool pool)
        {
            InitializeComponent();
            checkboxUseCurrentXSCredentials.Text = string.Format(checkboxUseCurrentXSCredentials.Text, BrandManager.BrandConsole);
            decentGroupBoxXSCredentials.Text = string.Format(decentGroupBoxXSCredentials.Text, BrandManager.ProductBrand);

            _pool = pool;
            _wlbServerState = WlbServerState.GetState(_pool);

            PopulateControls();
            CheckEnabledOkButton();
        }

        private void PopulateControls()
        {
            StringUtility.ParseHostnamePort(_pool.wlb_url, out var hostname, out var port);
            
            if (port == 0)
                port = DEFAULT_WLB_PORT;

            textboxWlbUrl.Text = hostname;
            textboxWLBPort.Text = port.ToString();
            textboxWlbUserName.Text = _pool.wlb_username;
            _wlbUrl = GetWlbUrl();

            decentGroupBoxWLBServerAddress.Enabled = _wlbServerState == WlbServerState.ServerState.NotConfigured ||
                                                     _wlbServerState == WlbServerState.ServerState.ConnectionError;
        }

        private void SetXsCredentials()
        {
            if (checkboxUseCurrentXSCredentials.Checked)
            {
                textboxXSUserName.Text = _pool.Connection.Username;
                textboxXSPassword.Text = _pool.Connection.Password;
                textboxXSUserName.Enabled = false;
                textboxXSPassword.Enabled = false;
            }
            else
            {
                textboxXSUserName.Text = string.Empty;
                textboxXSPassword.Text = string.Empty;
                textboxXSUserName.Enabled = true;
                textboxXSPassword.Enabled = true;
            }
        }

        private string GetWlbUrl()
        {
            if (string.IsNullOrWhiteSpace(textboxWlbUrl.Text) || string.IsNullOrWhiteSpace(textboxWLBPort.Text))
                return null;

            string wlbHost = textboxWlbUrl.Text.Trim();
            string wlbPort = textboxWLBPort.Text.Trim();

            if (!int.TryParse(wlbPort, out _))
                return null;

            // A valid server address should be an IPv4 / IPv6 address or a valid domain name

            if (IPAddress.TryParse(wlbHost, out var address))
            {
                switch (address.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        return $"{wlbHost}:{wlbPort}";
                    case AddressFamily.InterNetworkV6:
                        return $"[{wlbHost}]:{wlbPort}";
                }
            }

            try
            {
                var url = $"{wlbHost}:{wlbPort}";
                var uri = new Uri(url); //used as a quick validator
                return url;
            }
            catch
            {
                return null;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            AsyncAction action = null;

            switch (_wlbServerState)
            {
                case WlbServerState.ServerState.ConnectionError:
                case WlbServerState.ServerState.NotConfigured:
                {
                    action = new InitializeWLBAction(_pool, _wlbUrl,
                        textboxWlbUserName.Text.Trim(), textboxWlbPassword.Text.Trim(),
                        textboxXSUserName.Text.Trim(), textboxXSPassword.Text.Trim());
                    action.Completed += InitializeWLBAction_Completed;
                    break;
                }
                case WlbServerState.ServerState.Disabled:
                {
                    action = new EnableWLBAction(_pool);
                    action.Completed += Program.MainWindow.action_Completed;
                    break;
                }
            }

            if (action == null)
                return;

            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Blocks))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);
            }

            if (action.Succeeded)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void InitializeWLBAction_Completed(ActionBase obj)
        {
            Program.Invoke(Program.MainWindow, Program.MainWindow.UpdateToolbars);
        }

        private void CheckEnabledOkButton()
        {
            buttonOK.Enabled = _wlbUrl != null &&
                               !string.IsNullOrWhiteSpace(textboxWlbUserName.Text) &&
                               !string.IsNullOrWhiteSpace(textboxWlbPassword.Text) &&
                               !string.IsNullOrWhiteSpace(textboxXSUserName.Text) &&
                               !string.IsNullOrWhiteSpace(textboxXSPassword.Text);
        }

        private void textboxWlbUrl_TextChanged(object sender, EventArgs e)
        {
            _wlbUrl = GetWlbUrl();
            CheckEnabledOkButton();
        }

        private void textboxWLBPort_TextChanged(object sender, EventArgs e)
        {
            _wlbUrl = GetWlbUrl();
            CheckEnabledOkButton();
        }

        private void textboxWlbUserName_TextChanged(object sender, EventArgs e)
        {
            CheckEnabledOkButton();
        }

        private void textboxWlbPassword_TextChanged(object sender, EventArgs e)
        {
            CheckEnabledOkButton();
        }

        private void textboxXSUserName_TextChanged(object sender, EventArgs e)
        {
            CheckEnabledOkButton();
        }

        private void textboxXSPassword_TextChanged(object sender, EventArgs e)
        {
            CheckEnabledOkButton();
        }

        private void checkboxUseCurrentXSCredentials_CheckedChanged(object sender, EventArgs e)
        {
            SetXsCredentials();
        }
    }
}
