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

namespace XenAdmin.Dialogs
{
    public partial class SetStorageLinkLicenseServerDialog : XenDialogBase
    {
        public SetStorageLinkLicenseServerDialog(string address, int port)
        {
            InitializeComponent();
            Host = address;
            Port = port;
            UpdateEnablement();
        }

        public string Host
        {
            get
            {
                return txtHost.Text.Trim();
            }
            set
            {
                txtHost.Text = value;
            }
        }

        public int Port
        {
            get
            {
                if (Util.IsValidPort(txtPort.Text.Trim()))
                {
                    return int.Parse(txtPort.Text.Trim());
                }
                return 27000;
            }
            set
            {
                txtPort.Text = value.ToString();
            }
        }

        private void HostnameTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void PortNumTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void UpdateEnablement()
        {
            btnOK.Enabled = txtHost.Text.Trim().Length > 0 && Util.IsValidPort(txtPort.Text.Trim());
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}