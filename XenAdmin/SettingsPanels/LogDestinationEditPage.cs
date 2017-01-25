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
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using XenAdmin;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Actions;


namespace XenAdmin.SettingsPanels
{
    public partial class LogDestinationEditPage : UserControl, IEditPage
    {
        private Host TheHost;

        private bool _ValidToSave = true;
        private string _OrigLocation = null;

        private Regex regex = new Regex(@"^[a-zA-Z0-9]([-a-zA-Z0-9]{0,61}[a-zA-Z0-9])?(\.[a-zA-Z0-9]([-a-zA-Z0-9]{0,61}[a-zA-Z0-9])?)*$");

        private readonly ToolTip InvalidParamToolTip;

        public bool ValidToSave
        {
            get { return _ValidToSave; }
        }

        public LogDestinationEditPage()
        {
            InitializeComponent();

            Text = Messages.LOG_DESTINATION;

            InvalidParamToolTip = new ToolTip();
            InvalidParamToolTip.IsBalloon = true;
            InvalidParamToolTip.ToolTipIcon = ToolTipIcon.Warning;
            InvalidParamToolTip.ToolTipTitle = Messages.INVALID_PARAMETER;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            TheHost = clone as Host;
            Repopulate();
        }

        public Image Image
        {
            get { return Properties.Resources.log_destination_16; }
        }

        public void Repopulate()
        {
            if (TheHost != null)
            {
                string location = TheHost.SysLogDestination;
                if (location == null)
                {
                    LocalRadioButton.Checked = true;
                    ServerTextBox.Text = "";
                }
                else
                {
                    RemoteRadioButton.Checked = true;
                    ServerTextBox.Text = location;
                }
                _OrigLocation = location;
                _ValidToSave = true;
            }
            ServerLabel.Enabled = RemoteRadioButton.Checked;
            ServerTextBox.Enabled = RemoteRadioButton.Checked;
        }

        public bool HasChanged
        {
            get
            {
                // User changed remote location
                if (RemoteRadioButton.Checked == true && _OrigLocation != ServerTextBox.Text)
                {
                    return true;
                }
                // User switched from Server to Local
                if (LocalRadioButton.Checked == true && _OrigLocation != null)
                {
                    return true;
                }

                return false;
            }
        }

        public AsyncAction SaveSettings()
        {
            if (TheHost != null)
            {
                if (RemoteRadioButton.Checked)
                    TheHost.SysLogDestination = ServerTextBox.Text;
                else if (LocalRadioButton.Checked)
                    TheHost.SysLogDestination = null;

                return new DelegatedAsyncAction(
                    TheHost.Connection,
                    Messages.ACTION_CHANGE_LOG_DESTINATION,
                    string.Format(Messages.ACTION_CHANGING_LOG_DESTINATION_FOR, TheHost),
                    null,
                    delegate(Session session) { Host.syslog_reconfigure(session, TheHost.opaque_ref); },
                    true,
                    "host.syslog_reconfigure"
                );
            }
            else
                return null;
        }

        /** Show local validation balloon tooltips */
        public void ShowLocalValidationMessages()
        {
            if (RemoteRadioButton.Checked && (ServerTextBox.Text.Trim() == "" || !regex.IsMatch(ServerTextBox.Text)) && ServerTextBox.Text != _OrigLocation)
            {
                // Show invalid host message.
                HelpersGUI.ShowBalloonMessage(ServerTextBox, Messages.GENERAL_EDIT_INVALID_HOSTNAME, InvalidParamToolTip);
            }
        }

        /** Unregister listeners, dispose balloon tooltips, etc. */
        public void Cleanup()
        {
            InvalidParamToolTip.Dispose();
        }

        private void RemoteRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ServerLabel.Enabled = RemoteRadioButton.Checked;
            ServerTextBox.Enabled = RemoteRadioButton.Checked;
            if (RemoteRadioButton.Checked)
            {
                _ValidToSave = ServerTextBox.Text.Trim() != "" || ServerTextBox.Text == _OrigLocation;
            }
            else
            {
                _ValidToSave = true;
            }
        }

        private void ServerTextBox_TextChanged(object sender, EventArgs e)
        {
            _ValidToSave = ServerTextBox.Text.Trim() != "" && ServerTextBox.Text != _OrigLocation && regex.IsMatch(ServerTextBox.Text) && regex.IsMatch(ServerTextBox.Text);
        }

        public String SubText
        {
            get
            {
                return LocalRadioButton.Checked ? Messages.LOCAL : String.Format(Messages.REMOTE, ServerTextBox.Text);
            }
        }
    }
}
