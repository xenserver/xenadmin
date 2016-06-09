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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Properties;
using XenAdmin.Actions;
using XenAPI;
using System.Text.RegularExpressions;
using XenAdmin.Core;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class ConnectionOptionsPage : UserControl, IOptionsPage
    {
        private const string ConnectionTabSettingsHeader = "Connection Tab Settings -";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private OptionsDialog optionsDialog;

        // used for preventing the event handlers (mainly the SelectUseThisProxyServer function)  from being called when loading the settings into the text/check boxes
        private bool built = false;

        public ConnectionOptionsPage()
        {
            InitializeComponent();

            build();
        }

        public OptionsDialog OptionsDialog
        {
            set { this.optionsDialog = value; }
        }

        private void build()
        {
            // Proxy server
            switch (Properties.Settings.Default.ProxySetting)
            {
                case 0:
                    DirectConnectionRadioButton.Checked = true;
                    break;
                case 1:
                    UseIERadioButton.Checked = true;
                    break;
                case 2:
                    UseProxyRadioButton.Checked = true;
                    break;
                default:
                    DirectConnectionRadioButton.Checked = true;
                    break;
            }

            ProxyAddressTextBox.Text = Properties.Settings.Default.ProxyAddress;
            ProxyPortTextBox.Text = Properties.Settings.Default.ProxyPort.ToString();
            BypassLocalCheckBox.Checked = Properties.Settings.Default.BypassProxyForLocal;
            BypassAllCheckbox.Checked = Properties.Settings.Default.BypassProxyForServers;
            AuthenticationCheckBox.Checked = Properties.Settings.Default.ProvideProxyAuthentication;

            // checks for empty default username/password which starts out unencrypted
            string protectedUsername = Properties.Settings.Default.ProxyUsername;
            ProxyUsernameTextBox.Text = protectedUsername == "" ? "" : EncryptionUtils.Unprotect(Properties.Settings.Default.ProxyUsername);
            string protectedPassword = Properties.Settings.Default.ProxyPassword;
            ProxyPasswordTextBox.Text = protectedPassword == "" ? "" : EncryptionUtils.Unprotect(Properties.Settings.Default.ProxyPassword);

            ConnectionTimeoutNud.Value = Properties.Settings.Default.ConnectionTimeout / 1000;

            built = true;
        }

        private void UseProxyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            enableOK();
        }

        private void AuthenticationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!AuthenticationCheckBox.Checked)
            {
                ProxyUsernameTextBox.Clear();
                ProxyPasswordTextBox.Clear();
                AuthenticationCheckBox.Checked = false; // have to redo this as the 2 Clears above cause the checkbox to recheck
            }

            SelectUseThisProxyServer();

            enableOK();
        }

        private void ProxyAddressTextBox_TextChanged(object sender, EventArgs e)
        {
            SelectUseThisProxyServer();
            enableOK();
        }

        private void ProxyPortTextBox_TextChanged(object sender, EventArgs e)
        {
            SelectUseThisProxyServer();
            enableOK();
        }

        private void ProxyUsernameTextBox_TextChanged(object sender, EventArgs e)
        {
            SelectUseThisProxyServer();
            if (!AuthenticationCheckBox.Checked)
                AuthenticationCheckBox.Checked = true;
            enableOK();
        }

        private void ProxyPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            SelectUseThisProxyServer();
            if (!AuthenticationCheckBox.Checked)
                AuthenticationCheckBox.Checked = true;
            enableOK();
        }

        private void BypassLocalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SelectUseThisProxyServer();
            enableOK(); // not needed at the moment, but more future-proof
        }

        private void BypassAllCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            SelectUseThisProxyServer();
            enableOK(); // not needed at the moment, but more future-proof
        }

        private void SelectUseThisProxyServer()
        {
            if (!UseProxyRadioButton.Checked && built)
                UseProxyRadioButton.Checked = true;
        }

        private void enableOK()
        {
            if (optionsDialog == null)
                return;

            if (!UseProxyRadioButton.Checked)
            {
                optionsDialog.okButton.Enabled = true;
                return;
            }
            else if (AuthenticationCheckBox.Checked && ProxyUsernameTextBox.Text == "")
            {
                optionsDialog.okButton.Enabled = false;
                return;
            }

            try
            {
                if (!Util.IsValidPort(ProxyPortTextBox.Text))
                {
                    optionsDialog.okButton.Enabled = false;
                    return;
                }

                var uriHostNameType = Uri.CheckHostName(ProxyAddressTextBox.Text);

                optionsDialog.okButton.Enabled =  uriHostNameType != UriHostNameType.Unknown && uriHostNameType != UriHostNameType.IPv6;
                return;
            }
            catch
            {
                optionsDialog.okButton.Enabled = false;
            }
        }

        public static void Log()
        {
            log.Info(ConnectionTabSettingsHeader);
            // Proxy server
            log.Info("=== ProxySetting: " + Properties.Settings.Default.ProxySetting.ToString());
            log.Info("=== ProxyAddress: " + Properties.Settings.Default.ProxyAddress.ToString());
            log.Info("=== ProxyPort: " + Properties.Settings.Default.ProxyPort.ToString());
            log.Info("=== BypassProxyForLocal: " + Properties.Settings.Default.BypassProxyForLocal.ToString());
            log.Info("=== ByPassProxyForServers: " + Properties.Settings.Default.BypassProxyForServers.ToString());
            log.Info("=== ProvideProxyAuthentication: " + Properties.Settings.Default.ProvideProxyAuthentication.ToString());
            //log.Info("=== ProxyUsername: " + Properties.Settings.Default.ProxyUsername.ToString()); // No use in logging encrypted string
            //log.Info("=== ProxyPassword: " + Properties.Settings.Default.ProxyPassword.ToString()); // No use in logging encrypted string
            log.Info("=== ConnectionTimeout: " + Properties.Settings.Default.ConnectionTimeout.ToString());
        }

        #region IOptionsPage Members

        public void Save()
        {
            // Proxy server settings
            HTTPHelper.ProxyStyle new_proxy_style =
                DirectConnectionRadioButton.Checked ? HTTPHelper.ProxyStyle.DirectConnection :
                UseIERadioButton.Checked ? HTTPHelper.ProxyStyle.SystemProxy :
                                                      HTTPHelper.ProxyStyle.SpecifiedProxy;
            
            if (Properties.Settings.Default.ProxySetting != (int)new_proxy_style)
                Properties.Settings.Default.ProxySetting = (int)new_proxy_style;
            
            if (ProxyAddressTextBox.Text != Properties.Settings.Default.ProxyAddress && ProxyAddressTextBox.Text != "")
                Properties.Settings.Default.ProxyAddress = ProxyAddressTextBox.Text;

            Properties.Settings.Default.ProxyUsername = EncryptionUtils.Protect(ProxyUsernameTextBox.Text);
            Properties.Settings.Default.ProxyPassword = EncryptionUtils.Protect(ProxyPasswordTextBox.Text);

            if (int.Parse(ProxyPortTextBox.Text) != Properties.Settings.Default.ProxyPort)
            {
                try
                {
                    Properties.Settings.Default.ProxyPort = int.Parse(ProxyPortTextBox.Text);
                }
                catch
                {
                    Properties.Settings.Default.ProxyPort = 80;
                }
            }

            if (BypassLocalCheckBox.Checked != Properties.Settings.Default.BypassProxyForLocal)
                Properties.Settings.Default.BypassProxyForLocal = BypassLocalCheckBox.Checked;

            if (BypassAllCheckbox.Checked != Properties.Settings.Default.BypassProxyForServers)
                Properties.Settings.Default.BypassProxyForServers = BypassAllCheckbox.Checked;

            if (AuthenticationCheckBox.Checked != Properties.Settings.Default.ProvideProxyAuthentication)
                Properties.Settings.Default.ProvideProxyAuthentication = AuthenticationCheckBox.Checked;

            // timeout settings
            int timeout = (int)ConnectionTimeoutNud.Value;
            if (timeout * 1000 != Properties.Settings.Default.ConnectionTimeout)
            {
                Properties.Settings.Default.ConnectionTimeout = timeout * 1000;
            }

            Program.ReconfigureConnectionSettings();
            new TransferProxySettingsAction((HTTPHelper.ProxyStyle)Properties.Settings.Default.ProxySetting,
                                            Properties.Settings.Default.ProxyAddress,
                                            Properties.Settings.Default.ProxyPort,
                                            Properties.Settings.Default.ConnectionTimeout,
                                            Properties.Settings.Default.BypassProxyForLocal,
                                            true).RunAsync();
        }

        #endregion

        #region VerticalTab Members

        public override string Text
        {
            get { return Messages.CONNECTION; }
        }

        public string SubText
        {
            get { return Messages.CONNECTION_DESC; }
        }

        public Image Image
        {
            get { return Resources._000_Network_h32bit_16; }
        }

        #endregion
    }
}
