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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Actions.Wlb;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;

namespace XenAdmin.Dialogs.Wlb
{
    public partial class WlbCredentialsDialog : XenAdmin.Dialogs.XenDialogBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Pool _pool;

        //private WlbServerState _serverStatus;

        public WlbCredentialsDialog(Pool pool)
        {
            InitializeComponent();

            _pool = pool;
            //_serverStatus = new WlbServerState(_pool);

            PopulateControls();

            SetControlState();
        }

        private void PopulateControls()
        {
            string hostname;
            int port;

            if (!StringUtility.TryParseHostname(_pool.wlb_url, Program.DEFAULT_WLB_PORT, out hostname, out port))
            {
                hostname = _pool.wlb_url;
                port = Program.DEFAULT_WLB_PORT;
            }

            textboxWlbUrl.Text = hostname ?? "";
            textboxWLBPort.Text = port.ToString() ?? "";
            textboxWlbUserName.Text = _pool.wlb_username ?? "";

        }

        private void SetControlState()
        {
            switch (WlbServerState.GetState(_pool))
            {
                case WlbServerState.ServerState.NotConfigured:
                case WlbServerState.ServerState.ConnectionError:
                    {
                        decentGroupBoxWLBServerAddress.Enabled = true;
                        break;
                    }
            }
        }

        private void checkboxUseCurrentXSCredentials_CheckedChanged(object sender, EventArgs e)
        {
            SetXSCredentials(checkboxUseCurrentXSCredentials.Checked);
        }

        private void SetXSCredentials(bool useCurrent)
        {
            if (useCurrent)
            {
                textboxXSUserName.Text = _pool.Connection.Username;
                textboxXSPassword.Text = _pool.Connection.Password;
                textboxXSUserName.Enabled = false;
                textboxXSPassword.Enabled = false;
            }
            else
            {
                textboxXSUserName.Text = String.Empty;
                textboxXSPassword.Text = String.Empty;
                textboxXSUserName.Enabled = true;
                textboxXSPassword.Enabled = true;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (WlbServerState.GetState(_pool) == WlbServerState.ServerState.ConnectionError || WlbServerState.GetState(_pool) == WlbServerState.ServerState.NotConfigured)
            {
                try
                {
                    if (InitializeWLB())
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch
                {
                    log.Warn("Error Intializing WLB");
                }
            }
            else 
            {
                if (WlbServerState.GetState(_pool) == WlbServerState.ServerState.Disabled)
                {
                    if (EnableWlb())
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
        }

        private bool EnableWlb()
        {

            // Enable WLB.
            EnableWLBAction action = new EnableWLBAction(_pool);
            // We will need to re-enable buttons when the action completes
            action.Completed += Program.MainWindow.action_Completed;
            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Blocks))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);
            }

            Program.MainWindow.UpdateToolbars();
            return action.Succeeded;
        }

        private bool InitializeWLB()
        {
            //combine url and port 
            string wlbHost = textboxWlbUrl.Text;
            string wlbPort = textboxWLBPort.Text;
            IPAddress address;
            string wlbUrl = (IPAddress.TryParse(wlbHost, out address) && address.AddressFamily == AddressFamily.InterNetworkV6) ?
                ("[" + wlbHost + "]:" + wlbPort) : (wlbHost + ":" + wlbPort);

            //handle the wlb creds
            string wlbUserName = textboxWlbUserName.Text;
            string wlbPassword = textboxWlbPassword.Text;

            //handle the xenserver creds
            string xsUserName = textboxXSUserName.Text;
            string xsPassword = textboxXSPassword.Text;

            InitializeWLBAction action = new InitializeWLBAction(_pool, wlbUrl, wlbUserName, wlbPassword, xsUserName, xsPassword);
            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Blocks))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);
            }

            Program.MainWindow.UpdateToolbars();
            return action.Succeeded;
        }

        private void textboxWlbUrl_TextChanged(object sender, EventArgs e)
        {

            buttonOK.Enabled = checkEnabled_OkButton();
        }

        private bool checkEnabled_OkButton()
        {
            int dummy;
            return (textboxWlbUrl.Text.Length > 0) &&
                   (IsValidServerAddress(textboxWlbUrl.Text)) &&
                   (textboxWLBPort.Text.Length > 0) &&
                   (int.TryParse(textboxWLBPort.Text, out dummy)) &&
                   (textboxWlbUserName.Text.Length > 0) &&
                   (textboxWlbPassword.Text.Length > 0) &&
                   (textboxXSUserName.Text.Length>0) &&
                   (textboxXSPassword.Text.Length>0);
        }

        private bool IsValidServerAddress(string addr)
        {
            // A valid server address should be an IPv4 / IPv6 address or a valid domain name

            IPAddress address;
            if (IPAddress.TryParse(addr, out address))
            {
                return address.AddressFamily == AddressFamily.InterNetwork || address.AddressFamily == AddressFamily.InterNetworkV6;
            }
            else
            {
                try
                {
                    // adopt UriBuilder as a quick validator
                    UriBuilder ub = new UriBuilder(string.Format("http://user:password@{0}:80/", addr));
                    return ub.Host == addr;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void textboxWLBPort_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = checkEnabled_OkButton();
        }

        private void textboxWlbUserName_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = checkEnabled_OkButton();
        }

        private void textboxWlbPassword_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = checkEnabled_OkButton();
        }

        private void textboxXSUserName_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = checkEnabled_OkButton();
        }

        private void textboxXSPassword_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = checkEnabled_OkButton();
        }
    }
}

