﻿/* Copyright (c) Cloud Software Group, Inc. 
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
using System.Collections.Generic;
using XenAdmin.Core;
using System.Text.RegularExpressions;

namespace XenAdmin.SettingsPanels
{
    public partial class NRPEEditPage : UserControl, IEditPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string ALLOW_HOSTS_PLACE_HOLDER = Messages.NRPE_ALLOW_HOSTS_PLACE_HOLDER;

        private static readonly Regex REGEX_IPV4 = new Regex("^((25[0-5]|2[0-4]\\d|[01]?\\d\\d?)\\.){3}(25[0-5]|2[0-4]\\d|[01]?\\d\\d?)");
        private static readonly Regex REGEX_IPV4_CIDR = new Regex("^([0-9]{1,3}\\.){3}[0-9]{1,3}(\\/([0-9]|[1-2][0-9]|3[0-2]))?$");
        private static readonly Regex REGEX_DOMAIN = new Regex("^(((?!-))(xn--|_)?[a-z0-9-]{0,61}[a-z0-9]{1,1}\\.)*(xn--)?([a-z0-9][a-z0-9\\-]{0,60}|[a-z0-9-]{1,30}\\.[a-z]{2,})$");

        private readonly bool IsHost = true;

        private IXenObject Clone;
        private readonly ToolTip InvalidParamToolTip;
        private string InvalidParamToolTipText = "";

        private readonly List<CheckGroup> CheckGroupList = new List<CheckGroup>();
        private readonly Dictionary<string, CheckGroup> CheckGroupDictByName = new Dictionary<string, CheckGroup>();
        private readonly Dictionary<string, CheckGroup> CheckGroupDictByLabel = new Dictionary<string, CheckGroup>();

        private readonly NRPEHostConfiguration NRPEOriginalConfig = new NRPEHostConfiguration();

        public string SubText
        {
            get
            {
                return Messages.NRPE_EDIT_PAGE_TEXT;
            }
        }

        public Image Image => Images.StaticImages._000_Network_h32bit_16;

        public NRPEEditPage(bool isHost)
        {
            IsHost = isHost;
            InitializeComponent();
            Text = Messages.NRPE_EDIT_PAGE_TEXT;
            EnableNRPECheckBox.Checked = false;
            UpdateOtherComponentBasedEnableNRPECheckBox(false);

            AllowHostsTextBox.Text = ALLOW_HOSTS_PLACE_HOLDER;
            AllowHostsTextBox.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
            AllowHostsTextBox.GotFocus += AllowHostsTextBox_GotFocus;
            AllowHostsTextBox.LostFocus += AllowHostsTextBox_LostFocus;

            if (isHost)
            {
                PoolTipsPicture.Hide();
                PoolTipsLabel.Hide();
            }
            InvalidParamToolTip = new ToolTip
            {
                IsBalloon = true,
                ToolTipIcon = ToolTipIcon.Warning,
                ToolTipTitle = Messages.INVALID_PARAMETER,
                Tag = AllowHostsTextBox
            };

            CheckGroupList.Add(new HostLoadCheckGroup("check_host_load", Messages.NRPE_CHECK_HOST_LOAD));
            CheckGroupList.Add(new CheckGroup("check_host_cpu", Messages.NRPE_CHECK_HOST_CPU));
            CheckGroupList.Add(new CheckGroup("check_host_memory", Messages.NRPE_CHECK_HOST_MEMORY));
            CheckGroupList.Add(new CheckGroup("check_vgpu", Messages.NRPE_CHECK_VGPU));
            CheckGroupList.Add(new CheckGroup("check_vgpu_memory", Messages.NRPE_CHECK_VGPU_MEMORY));
            CheckGroupList.Add(new Dom0LoadCheckGroup("check_load", Messages.NRPE_CHECK_LOAD));
            CheckGroupList.Add(new CheckGroup("check_cpu", Messages.NRPE_CHECK_CPU));
            CheckGroupList.Add(new CheckGroup("check_memory", Messages.NRPE_CHECK_MEMORY));
            CheckGroupList.Add(new FreeCheckGroup("check_swap", Messages.NRPE_CHECK_SWAP));
            CheckGroupList.Add(new FreeCheckGroup("check_disk_root", Messages.NRPE_CHECK_DISK_ROOT));
            CheckGroupList.Add(new FreeCheckGroup("check_disk_log", Messages.NRPE_CHECK_DISK_LOG));

            foreach (CheckGroup checkGroup in CheckGroupList)
            {
                CheckDataGridView.Rows.Add(checkGroup.CheckThresholdRow);
                CheckGroupDictByName.Add(checkGroup.Name, checkGroup);
                CheckGroupDictByLabel.Add(checkGroup.NameCell.Value.ToString(), checkGroup);
            }
        }

        public bool ValidToSave
        {
            get
            {
                InvalidParamToolTipText = "";
                InvalidParamToolTip.ToolTipTitle = "";
                CheckDataGridView.ShowCellToolTips = false;

                if (!EnableNRPECheckBox.Checked)
                {
                    return true;
                }

                bool valid = true;
                if (!IsAllowHostsValid())
                {
                    valid = false;
                }

                foreach (CheckGroup checkGroup in CheckGroupList)
                {
                    if (!checkGroup.CheckValue())
                    {
                        valid = false;
                    }
                }
                return valid;
            }
        }

        public bool HasChanged
        {
            get
            {
                return true;
            }
        }

        public void Cleanup()
        {
        }

        public void ShowLocalValidationMessages()
        {
            if (InvalidParamToolTip.Tag is Control ctrl)
            {
                InvalidParamToolTip.Hide(ctrl);
                if (!InvalidParamToolTipText.Equals(""))
                {
                    HelpersGUI.ShowBalloonMessage(ctrl, InvalidParamToolTip, InvalidParamToolTipText);
                }
            }
        }

        public void HideLocalValidationMessages()
        {
            if (InvalidParamToolTip.Tag is Control ctrl && ctrl != null)
            {
                InvalidParamToolTip.Hide(ctrl);
            }
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            Clone = clone;
            if (IsHost)
            {
                InitNRPEGeneralConfiguration();
                InitNRPEThreshold();
            }
        }

        public AsyncAction SaveSettings()
        {
            NRPEHostConfiguration NRPEHostConfiguration = new NRPEHostConfiguration
            {
                EnableNRPE = EnableNRPECheckBox.Checked,
                AllowHosts = AllowHostsTextBox.Text,
                Debug = DebugLogCheckBox.Checked ? NRPEHostConfiguration.DEBUG_ENABLE : NRPEHostConfiguration.DEBUG_DISABLE,
                SslLogging = SslDebugLogCheckBox.Checked ? NRPEHostConfiguration.SSL_LOGGING_ENABLE : NRPEHostConfiguration.SSL_LOGGING_DISABLE
            };

            foreach (KeyValuePair<string, CheckGroup> item in CheckGroupDictByName)
            {
                if (item.Value.WarningThresholdCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlText))
                    && item.Value.CriticalThresholdCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlText)))
                {
                    NRPEHostConfiguration.AddNRPECheck(new NRPEHostConfiguration.Check(item.Key,
                        item.Value.WarningThresholdCell.Value.ToString(), item.Value.CriticalThresholdCell.Value.ToString()));
                }
            }
            return new NRPEUpdateAction(Clone, NRPEHostConfiguration, NRPEOriginalConfig, false);
        }

        private void InitNRPEGeneralConfiguration()
        {
            string status = Host.call_plugin(Clone.Connection.Session, Clone.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME, 
                NRPEHostConfiguration.XAPI_NRPE_STATUS, null);
            log.InfoFormat("Execute nrpe {0}, return: {1}", NRPEHostConfiguration.XAPI_NRPE_STATUS, status);
            EnableNRPECheckBox.Checked = status.Trim().Equals("active enabled");
            NRPEOriginalConfig.EnableNRPE = EnableNRPECheckBox.Checked;
            UpdateOtherComponentBasedEnableNRPECheckBox(EnableNRPECheckBox.Checked);

            string nrpeConfig = Host.call_plugin(Clone.Connection.Session, Clone.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME, 
                NRPEHostConfiguration.XAPI_NRPE_GET_CONFIG, null);
            log.InfoFormat("Execute nrpe {0}, return: {1}", NRPEHostConfiguration.XAPI_NRPE_GET_CONFIG, nrpeConfig);

            string[] nrpeConfigArray = nrpeConfig.Split('\n');
            foreach (string nrpeConfigItem in nrpeConfigArray)
            {
                if (nrpeConfigItem.Trim().StartsWith("allowed_hosts:"))
                {
                    string allowHosts = nrpeConfigItem.Replace("allowed_hosts:", "").Trim();
                    if (!allowHosts.Equals(""))
                    {
                        AllowHostsTextBox.Text = AllowHostsWithoutLocalAddress(allowHosts);
                        AllowHostsTextBox.ForeColor = AllowHostsTextBox.Text.Equals(ALLOW_HOSTS_PLACE_HOLDER) ? 
                            Color.FromKnownColor(KnownColor.ControlDark) : Color.FromKnownColor(KnownColor.ControlText);
                        NRPEOriginalConfig.AllowHosts = AllowHostsTextBox.Text;
                    }
                }
                else if (nrpeConfigItem.Trim().StartsWith("debug:"))
                {
                    string enableDebug = nrpeConfigItem.Replace("debug:", "").Trim();
                    DebugLogCheckBox.Checked = enableDebug.Equals(NRPEHostConfiguration.DEBUG_ENABLE);
                    NRPEOriginalConfig.Debug = DebugLogCheckBox.Checked ? NRPEHostConfiguration.DEBUG_ENABLE : NRPEHostConfiguration.DEBUG_DISABLE;
                }
                else if (nrpeConfigItem.Trim().StartsWith("ssl_logging:"))
                {
                    string enableSslLogging = nrpeConfigItem.Replace("ssl_logging:", "").Trim();
                    SslDebugLogCheckBox.Checked = enableSslLogging.Equals(NRPEHostConfiguration.SSL_LOGGING_ENABLE);
                    NRPEOriginalConfig.SslLogging = SslDebugLogCheckBox.Checked ? NRPEHostConfiguration.SSL_LOGGING_ENABLE : NRPEHostConfiguration.SSL_LOGGING_DISABLE;
                }
            }
        }

        private void InitNRPEThreshold()
        {
            string nrpeThreshold = Host.call_plugin(Clone.Connection.Session, Clone.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME, 
                NRPEHostConfiguration.XAPI_NRPE_GET_THRESHOLD, null);
            log.InfoFormat("Execute nrpe {0}, return: {1}", NRPEHostConfiguration.XAPI_NRPE_GET_THRESHOLD, nrpeThreshold);

            string[] nrpeThresholdArray = nrpeThreshold.Split('\n');
            foreach (string nrpeThresholdItem in nrpeThresholdArray)
            {
                // Return string format for each line: check_cpu warning threshold - 50 critical threshold - 80
                string[] thresholdRtnArray = nrpeThresholdItem.Split(' ');
                string checkName = thresholdRtnArray[0];
                if (CheckGroupDictByName.TryGetValue(thresholdRtnArray[0], out CheckGroup thresholdCheck))
                {
                    string warningThreshold = thresholdRtnArray[4];
                    string criticalThreshold = thresholdRtnArray[8];
                    thresholdCheck.UpdateThreshold(warningThreshold, criticalThreshold);
                    NRPEOriginalConfig.AddNRPECheck(new NRPEHostConfiguration.Check(checkName, warningThreshold, criticalThreshold));
                }
            }
        }

        private bool IsAllowHostsValid()
        {
            InvalidParamToolTip.ToolTipTitle = Messages.NRPE_ALLOW_HOSTS_ERROR_TITLE;
            InvalidParamToolTip.Tag = AllowHostsTextBox;
            CheckDataGridView.ShowCellToolTips = true;

            string str = AllowHostsTextBox.Text;
            if (str.Trim().Length == 0 || str.Trim().Equals(ALLOW_HOSTS_PLACE_HOLDER))
            {
                InvalidParamToolTipText = Messages.NRPE_ALLOW_HOSTS_EMPTY_ERROR;
                return false;
            }

            string[] hostArray = str.Split(',');
            foreach (string host in hostArray)
            {
                if (!REGEX_IPV4.Match(host.Trim()).Success &&
                    !REGEX_IPV4_CIDR.Match(host.Trim()).Success &&
                    !REGEX_DOMAIN.Match(host.Trim()).Success)
                {
                    InvalidParamToolTipText = Messages.NRPE_ALLOW_HOSTS_FORMAT_ERROR;
                    return false;
                }
            }
            CheckDataGridView.ShowCellToolTips = false;
            return true;
        }

        private string AllowHostsWithoutLocalAddress(string allowHosts)
        {
            string UpdatedAllowHosts = "";
            string[] AllowHostArray = allowHosts.Split(',');
            foreach (string allowHost in AllowHostArray)
            {
                if (!allowHost.Trim().Equals("127.0.0.1") &&
                    !allowHost.Trim().Equals("::1"))
                {
                    UpdatedAllowHosts += allowHost + ",";
                }
            }
            return UpdatedAllowHosts.Length == 0 ? ALLOW_HOSTS_PLACE_HOLDER :
                UpdatedAllowHosts.Substring(0, UpdatedAllowHosts.Length - 1);
        }

        private void UpdateOtherComponentBasedEnableNRPECheckBox(bool check)
        {
            if (check)
            {
                AllowHostsTextBox.Enabled = true;
                AllowHostsLabel.Enabled = true;
                DebugLogCheckBox.Enabled = true;
                SslDebugLogCheckBox.Enabled = true;
                CheckDataGridView.Enabled = true;
                CheckDataGridView.BackgroundColor = Color.FromKnownColor(KnownColor.Window);
                CheckDataGridView.DefaultCellStyle.BackColor = Color.FromKnownColor(KnownColor.Window);
            }
            else
            {
                AllowHostsTextBox.Enabled = false;
                AllowHostsLabel.Enabled = false;
                DebugLogCheckBox.Enabled = false;
                SslDebugLogCheckBox.Enabled = false;
                CheckDataGridView.Enabled = false;
                CheckDataGridView.BackgroundColor = Color.FromKnownColor(KnownColor.Control);
                CheckDataGridView.DefaultCellStyle.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
        }

        private void EnableNRPECheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateOtherComponentBasedEnableNRPECheckBox(EnableNRPECheckBox.Checked);
        }

        private void AllowHostsTextBox_GotFocus(object sender, EventArgs e)
        {
            if (ALLOW_HOSTS_PLACE_HOLDER.Equals(AllowHostsTextBox.Text))
            {
                AllowHostsTextBox.Text = "";
                AllowHostsTextBox.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            }
        }

        private void AllowHostsTextBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AllowHostsTextBox.Text))
            {
                AllowHostsTextBox.Text = ALLOW_HOSTS_PLACE_HOLDER;
                AllowHostsTextBox.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
            }
        }

        private void CheckDataGridView_BeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewCell currentCell = CheckDataGridView.CurrentRow.Cells[e.ColumnIndex];
            if (!IsHost && currentCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlDark)))
            {
                currentCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                currentCell.Value = "";
            }
        }

        private void CheckDataGridView_EndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell currentCell = CheckDataGridView.CurrentRow.Cells[e.ColumnIndex];
            if (!IsHost && currentCell.Value.ToString().Trim().Equals(""))
            {
                currentCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
                CheckGroupDictByLabel.TryGetValue(CheckDataGridView.CurrentRow.Cells[0].Value.ToString(), out CheckGroup checkGroup);
                currentCell.Value = currentCell.ColumnIndex == 1 ? 
                    checkGroup.WarningThresholdDefault : (object)checkGroup.CriticalThresholdDefault;
            }
        }
    }
}
