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
using XenCenterLib;

namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class ConnectionOptionsPage : UserControl, IOptionsPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event Action<bool> IsValidChanged;

        // used for preventing the event handlers from doing anything when changing controls through code
        private bool eventsDisabled = true;

        public ConnectionOptionsPage()
        {
            InitializeComponent();
            Build();
        }

        private void Build()
        {
            // Proxy server
            switch ((HTTPHelper.ProxyStyle)Properties.Settings.Default.ProxySetting)
            {
                case HTTPHelper.ProxyStyle.DirectConnection:
                    DirectConnectionRadioButton.Checked = true;
                    break;
                case HTTPHelper.ProxyStyle.SystemProxy:
                    UseIERadioButton.Checked = true;
                    break;
                case HTTPHelper.ProxyStyle.SpecifiedProxy:
                    UseProxyRadioButton.Checked = true;
                    break;
                default:
                    DirectConnectionRadioButton.Checked = true;
                    break;
            }

            ProxyAddressTextBox.Text = Properties.Settings.Default.ProxyAddress;
            ProxyPortTextBox.Text = Properties.Settings.Default.ProxyPort.ToString();
            BypassForServersCheckbox.Checked = Properties.Settings.Default.BypassProxyForServers;

            AuthenticationCheckBox.Checked = Properties.Settings.Default.ProvideProxyAuthentication;
                
            switch ((HTTP.ProxyAuthenticationMethod)Properties.Settings.Default.ProxyAuthenticationMethod)
            {
                case HTTP.ProxyAuthenticationMethod.Basic:
                    BasicRadioButton.Checked = true;
                    break;
                case HTTP.ProxyAuthenticationMethod.Digest:
                    DigestRadioButton.Checked = true;
                    break;
                default:
                    DigestRadioButton.Checked = true;
                    break;
            }

            // checks for empty default username/password which starts out unencrypted
            try
            {
                string protectedUsername = Properties.Settings.Default.ProxyUsername;
                ProxyUsernameTextBox.Text = string.IsNullOrEmpty(protectedUsername) ? "" : EncryptionUtils.Unprotect(protectedUsername);
            }
            catch (Exception e)
            {
                log.Warn("Could not unprotect internet proxy username.", e);
            }

            try
            {
                string protectedPassword = Properties.Settings.Default.ProxyPassword;
                ProxyPasswordTextBox.Text = string.IsNullOrEmpty(protectedPassword) ? "" : EncryptionUtils.Unprotect(protectedPassword);
            }
            catch (Exception e)
            {
                log.Warn("Could not unprotect internet proxy password.", e);
            }

            ConnectionTimeoutNud.Value = Properties.Settings.Default.ConnectionTimeout / 1000;

            eventsDisabled = false;
        }

        private void UseProxyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (eventsDisabled)
                return;

            CheckValid();
        }

        private void AuthenticationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (eventsDisabled)
                return;

            eventsDisabled = true;

            if (!AuthenticationCheckBox.Checked)
            {
                ProxyUsernameTextBox.Clear();
                ProxyPasswordTextBox.Clear();
            }
            SelectUseThisProxyServer();

            eventsDisabled = false;

            CheckValid();
        }

        private void GeneralProxySettingsChanged(object sender, EventArgs e)
        {
            if (eventsDisabled)
                return;
            SelectUseThisProxyServer();
            CheckValid();
        }

        private void ProxyAuthenticationSettingsChanged(object sender, EventArgs e)
        {
            if (eventsDisabled)
                return;
            SelectProvideCredentials();
            CheckValid();
        }

        private void SelectUseThisProxyServer()
        {
            UseProxyRadioButton.Checked = true;
        }

        private void SelectProvideCredentials()
        {
            AuthenticationCheckBox.Checked = true;
            UseProxyRadioButton.Checked = true;
        }

        private void CheckValid()
        {
            if (!UseProxyRadioButton.Checked)
            {
                IsValidChanged?.Invoke(true);
                return;
            }
            
            if (AuthenticationCheckBox.Checked && string.IsNullOrEmpty(ProxyUsernameTextBox.Text))
            {
                IsValidChanged?.Invoke(false);
                return;
            }

            if (!Util.IsValidPort(ProxyPortTextBox.Text))
            {
                IsValidChanged?.Invoke(false);
                return;
            }

            try
            {
                var uriHostNameType = Uri.CheckHostName(ProxyAddressTextBox.Text);
                IsValidChanged?.Invoke(uriHostNameType != UriHostNameType.Unknown && uriHostNameType != UriHostNameType.IPv6);
            }
            catch
            {
                IsValidChanged?.Invoke(false);
            }
        }

        #region IOptionsPage Members

        public void Save()
        {
            // Proxy server settings
            HTTPHelper.ProxyStyle new_proxy_style =
                DirectConnectionRadioButton.Checked
                    ? HTTPHelper.ProxyStyle.DirectConnection
                    : UseIERadioButton.Checked
                        ? HTTPHelper.ProxyStyle.SystemProxy
                        : HTTPHelper.ProxyStyle.SpecifiedProxy;
            
            if (Properties.Settings.Default.ProxySetting != (int)new_proxy_style)
                Properties.Settings.Default.ProxySetting = (int)new_proxy_style;
            
            if (ProxyAddressTextBox.Text != Properties.Settings.Default.ProxyAddress && !string.IsNullOrEmpty(ProxyAddressTextBox.Text))
                Properties.Settings.Default.ProxyAddress = ProxyAddressTextBox.Text;

            Properties.Settings.Default.ProxyUsername = EncryptionUtils.Protect(ProxyUsernameTextBox.Text);
            Properties.Settings.Default.ProxyPassword = EncryptionUtils.Protect(ProxyPasswordTextBox.Text);
            Properties.Settings.Default.ProvideProxyAuthentication = AuthenticationCheckBox.Checked;

            HTTP.ProxyAuthenticationMethod new_auth_method = BasicRadioButton.Checked
                ? HTTP.ProxyAuthenticationMethod.Basic
                : HTTP.ProxyAuthenticationMethod.Digest;

            if (Properties.Settings.Default.ProxyAuthenticationMethod != (int)new_auth_method)
                Properties.Settings.Default.ProxyAuthenticationMethod = (int)new_auth_method;

            if (int.TryParse(ProxyPortTextBox.Text, out int port))
            {
                if (port != Properties.Settings.Default.ProxyPort)
                    Properties.Settings.Default.ProxyPort = port;
            }
            else
                Properties.Settings.Default.ProxyPort = 80;

            if (BypassForServersCheckbox.Checked != Properties.Settings.Default.BypassProxyForServers)
                Properties.Settings.Default.BypassProxyForServers = BypassForServersCheckbox.Checked;

            // timeout settings
            int timeout = 1000* (int)ConnectionTimeoutNud.Value;
            if (timeout != Properties.Settings.Default.ConnectionTimeout)
                Properties.Settings.Default.ConnectionTimeout = timeout;

            Program.ReconfigureConnectionSettings();

            Core.HealthCheck.SendProxySettingsToHealthCheck();
        }

        #endregion

        #region IVerticalTab Members

        public override string Text => Messages.CONNECTION;

        public string SubText => Messages.CONNECTION_DESC;

        public Image Image => Images.StaticImages._000_Network_h32bit_16;

        #endregion
    }
}
