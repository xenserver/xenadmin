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
using System.Drawing;
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Core;
using System.Text.RegularExpressions;
using XenAdmin.Actions.SNMP;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace XenAdmin.SettingsPanels
{
    public partial class SnmpEditPage : UserControl, IEditPage
    {
        public override string Text => Messages.SNMP;
        public string SubText => Messages.SNMP_EDIT_PAGE_TEXT;
        public Image Image => Images.StaticImages._000_Network_h32bit_16;
        private ToolTip _invalidParamToolTip = new ToolTip
        {
            IsBalloon = true,
            ToolTipIcon = ToolTipIcon.Warning,
        };
        private string _invalidParamToolTipText = "";
        private static readonly Regex RegexCommon = new Regex(@"^[a-zA-Z0-9-.\#@=:_]{6,32}$");
        private static readonly Regex RegexEncryptTextBox = new Regex(@"^([a-zA-Z0-9-.\#@=:_]{8,32}|[*]{8})$");
        private static bool _encryptTextBoxFlag = false;
        private IXenObject _clone;
        private readonly SnmpConfiguration _snmpConfiguration = new SnmpConfiguration();
        private readonly SnmpConfiguration _snmpCurrentConfiguration = new SnmpConfiguration();

        public bool HasChanged
        {
            get
            {
                UpdateCurrentSnmpConfiguration();
                return !_snmpCurrentConfiguration.Equals(_snmpConfiguration);
            }
        }

        public bool ValidToSave
        {
            get
            {
                _invalidParamToolTip.Dispose();
                _invalidParamToolTip = new ToolTip
                {
                    IsBalloon = true,
                    ToolTipIcon = ToolTipIcon.Warning,
                };
                _invalidParamToolTipText = " ";

                if (!EnableSnmpCheckBox.Checked)
                {
                    return true;
                }
                if (!SupportV2cCheckBox.Checked && !SupportV3CheckBox.Checked)
                {
                    _invalidParamToolTip.Tag = SupportV2cCheckBox;
                    _invalidParamToolTip.ToolTipTitle = Messages.SNMP_ALLOW_CHOOSE_TITLE;
                    return false;
                }
                if (SupportV2cCheckBox.Checked)
                {
                    var communityStr = CommunityTextBox.Text;
                    if (string.IsNullOrEmpty(communityStr) || !RegexCommon.Match(communityStr.Trim()).Success)
                    {
                        _invalidParamToolTip.Tag = CommunityTextBox;
                        _invalidParamToolTip.ToolTipTitle = Messages.SNMP_ALLOW_COMMUNITY_TITLE;
                        _invalidParamToolTipText = Messages.SNMP_ALLOW_COMMUNITY_TEXT;
                        return false;
                    }
                }
                if (SupportV3CheckBox.Checked)
                {
                    var usernameStr = UserNameTextBox.Text;
                    var authPassStr = AuthenticationPasswordLabelTextBox.Text;
                    var privacyPassStr = PrivacyPasswordTextBox.Text;
                    if (string.IsNullOrEmpty(usernameStr) || !RegexCommon.Match(usernameStr.Trim()).Success)
                    {
                        _invalidParamToolTip.Tag = UserNameTextBox;
                        _invalidParamToolTip.ToolTipTitle = Messages.SNMP_ALLOW_USER_TITLE;
                        _invalidParamToolTipText = Messages.SNMP_ALLOW_COMMUNITY_TEXT;
                        return false;
                    }

                    if (string.IsNullOrEmpty(authPassStr) || !RegexEncryptTextBox.Match(authPassStr.Trim()).Success)
                    {
                        _invalidParamToolTip.Tag = AuthenticationPasswordLabelTextBox;
                        _invalidParamToolTip.ToolTipTitle = Messages.SNMP_ALLOW_AUTH_TITLE;
                        _invalidParamToolTipText = Messages.SNMP_ALLOW_AUTH_TEXT;
                        return false;
                    }

                    if (string.IsNullOrEmpty(privacyPassStr) || !RegexEncryptTextBox.Match(privacyPassStr.Trim()).Success)
                    {
                        _invalidParamToolTip.Tag = PrivacyPasswordTextBox;
                        _invalidParamToolTip.ToolTipTitle = Messages.SNMP_ALLOW_PRIVACY_TITLE;
                        _invalidParamToolTipText = Messages.SNMP_ALLOW_AUTH_TEXT;
                        return false;
                    }
                }
                return true;
            }
        }

        public SnmpEditPage()
        {
            InitializeComponent();
        }

        public void Cleanup()
        {
            _invalidParamToolTip.Dispose();
        }

        public void ShowLocalValidationMessages()
        {
            if (_invalidParamToolTip.Tag is Control ctrl)
            {
                _invalidParamToolTip.Hide(ctrl);
                if (!string.IsNullOrEmpty(_invalidParamToolTip.ToolTipTitle))
                {
                    HelpersGUI.ShowBalloonMessage(ctrl, _invalidParamToolTip, _invalidParamToolTipText);
                }
            }
        }

        public void HideLocalValidationMessages()
        {
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _clone = clone;
            UpdateAllComponents(false);
            var action = new SnmpRetrieveAction(_clone, _snmpConfiguration, true);
            action.Completed += ActionCompleted;
            action.RunAsync();
        }

        private void ActionCompleted(ActionBase sender)
        {
            Program.Invoke(this.Parent, UpdateRetrieveStatus);
        }

        private void UpdateRetrieveStatus()
        {
            if (_snmpConfiguration.IsSuccessful)
            {
                _encryptTextBoxFlag = true;
                EnableSnmpCheckBox.Enabled = true;
                RetrieveSnmpPanel.Visible = false;
                ServiceStatusPicture.Visible = ServiceStatusLabel.Visible = 
                    !_snmpConfiguration.ServiceStatus && _snmpConfiguration.IsSnmpEnabled;
                EnableSnmpCheckBox.Checked = _snmpConfiguration.IsSnmpEnabled;
                DebugLogCheckBox.Checked = _snmpConfiguration.IsLogEnabled;
                SupportV2cCheckBox.Checked = _snmpConfiguration.IsV2CEnabled;
                SupportV3CheckBox.Checked = _snmpConfiguration.IsV3Enabled;
                CommunityTextBox.Text = _snmpConfiguration.Community;
                UserNameTextBox.Text = _snmpConfiguration.UserName;
                AuthenticationPasswordLabelTextBox.Text = _snmpConfiguration.AuthPass;
                AuthenticationProtocolComboBox.SelectedItem = _snmpConfiguration.AuthProtocol;
                PrivacyPasswordTextBox.Text = _snmpConfiguration.PrivacyPass;
                PrivacyProtocolComboBox.SelectedItem = _snmpConfiguration.PrivacyProtocol;
                if (EnableSnmpCheckBox.Checked)
                {
                    SnmpV2cGroupBox.Enabled = SupportV2cCheckBox.Checked;
                    SnmpV3GroupBox.Enabled = SupportV3CheckBox.Checked;
                }
                _encryptTextBoxFlag = false;
            }
            else
            {
                RetrieveSnmpLabel.Text = Messages.SNMP_RETRIEVE_FAILED;
                RetrieveSnmpPicture.Image = Images.StaticImages._000_error_h32bit_16;
            }

        }

        private void UpdateCurrentSnmpConfiguration()
        {
            _snmpCurrentConfiguration.IsSnmpEnabled = EnableSnmpCheckBox.Checked;
            _snmpCurrentConfiguration.IsLogEnabled = DebugLogCheckBox.Checked;
            _snmpCurrentConfiguration.IsV2CEnabled = SupportV2cCheckBox.Checked;
            _snmpCurrentConfiguration.IsV3Enabled = SupportV3CheckBox.Checked;
            _snmpCurrentConfiguration.Community = CommunityTextBox.Text;
            _snmpCurrentConfiguration.UserName = UserNameTextBox.Text;
            _snmpCurrentConfiguration.AuthPass = AuthenticationPasswordLabelTextBox.Text;
            _snmpCurrentConfiguration.AuthProtocol = AuthenticationProtocolComboBox.Text;
            _snmpCurrentConfiguration.PrivacyPass = PrivacyPasswordTextBox.Text;
            _snmpCurrentConfiguration.PrivacyProtocol = PrivacyProtocolComboBox.Text;
        }

        public AsyncAction SaveSettings()
        {
            return new SnmpUpdateAction(_clone, _snmpCurrentConfiguration, true);
        }

        private void UpdateAllComponents(bool status)
        {
            EnableSnmpCheckBox.Enabled = DebugLogCheckBox.Enabled = SupportV2cCheckBox.Enabled = SnmpV2cGroupBox.Enabled = SupportV3CheckBox.Enabled = SnmpV3GroupBox.Enabled = status;
        }

        private void EnableSNMPCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DebugLogCheckBox.Enabled = EnableSnmpCheckBox.Checked;
            SupportV2cCheckBox.Enabled = EnableSnmpCheckBox.Checked;
            SnmpV2cGroupBox.Enabled = EnableSnmpCheckBox.Checked && SupportV2cCheckBox.Checked;
            SupportV3CheckBox.Enabled = EnableSnmpCheckBox.Checked;
            SnmpV3GroupBox.Enabled = EnableSnmpCheckBox.Checked && SupportV3CheckBox.Checked;
        }

        private void EncryptTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_encryptTextBoxFlag) return;
            var textBox = (TextBox)sender;
            if (textBox.Text.Contains("*"))
            {
                textBox.Text = textBox.Text.Replace("*", "");
                textBox.SelectionStart = textBox.Text.Length;
                textBox.ScrollToCaret();
            }
            if (AuthenticationPasswordLabelTextBox.Text.Contains("*"))
            {
                AuthenticationPasswordLabelTextBox.Text = "";
            }
            if (PrivacyPasswordTextBox.Text.Contains("*"))
            {
                PrivacyPasswordTextBox.Text = "";
            }
        }

        private void V3Block_Changed(object sender, EventArgs e)
        {
            if (_encryptTextBoxFlag) return;
            if (AuthenticationPasswordLabelTextBox.Text.Contains("*") || PrivacyPasswordTextBox.Text.Contains("*"))
            {
                AuthenticationPasswordLabelTextBox.Text = "";
                PrivacyPasswordTextBox.Text = "";
            }
        }

        private void SupportV2CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SnmpV2cGroupBox.Enabled = SupportV2cCheckBox.Checked;
        }

        private void SupportV3CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SnmpV3GroupBox.Enabled = SupportV3CheckBox.Checked;
        }
    }
}
