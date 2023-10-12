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
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Actions.NRPE;
using XenAPI;
using System.Linq;

namespace XenAdmin.SettingsPanels
{
    public partial class NRPEEditPage : UserControl, IEditPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Regex REGEX_IPV4 = new Regex("^((25[0-5]|2[0-4]\\d|[01]?\\d\\d?)\\.){3}(25[0-5]|2[0-4]\\d|[01]?\\d\\d?)");
        private static readonly Regex REGEX_IPV4_CIDR = new Regex("^([0-9]{1,3}\\.){3}[0-9]{1,3}(\\/([0-9]|[1-2][0-9]|3[0-2]))?$");
        private static readonly Regex REGEX_DOMAIN = new Regex("^(((?!-))(xn--|_)?[a-z0-9-]{0,61}[a-z0-9]{1,1}\\.)*(xn--)?([a-z0-9][a-z0-9\\-]{0,60}|[a-z0-9-]{1,30}\\.[a-z]{2,})$");

        private bool _isHost;

        private IXenObject _clone;
        private readonly ToolTip _invalidParamToolTip;
        private string _invalidParamToolTipText = "";

        private readonly List<CheckGroup> _checkGroupList = new List<CheckGroup>();
        private readonly Dictionary<string, CheckGroup> _checkGroupDictByName = new Dictionary<string, CheckGroup>();
        private readonly Dictionary<string, CheckGroup> _checkGroupDictByLabel = new Dictionary<string, CheckGroup>();

        private NRPEHostConfiguration _nrpeOriginalConfig;
        private NRPEHostConfiguration _nrpeCurrentConfig;

        public Image Image => Images.StaticImages._000_Module_h32bit_16;

        public NRPEEditPage()
        {
            InitializeComponent();
            Text = Messages.NRPE;

            _nrpeOriginalConfig = new NRPEHostConfiguration
            {
                EnableNRPE = false,
                AllowHosts = NRPEHostConfiguration.ALLOW_HOSTS_PLACE_HOLDER,
                Debug = false,
                SslLogging = false
            };

            _checkGroupList.Add(new HostLoadCheckGroup("check_host_load", Messages.NRPE_CHECK_HOST_LOAD));
            _checkGroupList.Add(new CheckGroup("check_host_cpu", Messages.NRPE_CHECK_HOST_CPU));
            _checkGroupList.Add(new CheckGroup("check_host_memory", Messages.NRPE_CHECK_HOST_MEMORY));
            _checkGroupList.Add(new CheckGroup("check_vgpu", Messages.NRPE_CHECK_VGPU));
            _checkGroupList.Add(new CheckGroup("check_vgpu_memory", Messages.NRPE_CHECK_VGPU_MEMORY));
            _checkGroupList.Add(new Dom0LoadCheckGroup("check_load", Messages.NRPE_CHECK_LOAD));
            _checkGroupList.Add(new CheckGroup("check_cpu", Messages.NRPE_CHECK_CPU));
            _checkGroupList.Add(new CheckGroup("check_memory", Messages.NRPE_CHECK_MEMORY));
            _checkGroupList.Add(new FreeCheckGroup("check_swap", Messages.NRPE_CHECK_SWAP));
            _checkGroupList.Add(new FreeCheckGroup("check_disk_root", Messages.NRPE_CHECK_DISK_ROOT));
            _checkGroupList.Add(new FreeCheckGroup("check_disk_log", Messages.NRPE_CHECK_DISK_LOG));

            foreach (CheckGroup checkGroup in _checkGroupList)
            {
                CheckDataGridView.Rows.Add(checkGroup.CheckThresholdRow);
                _checkGroupDictByName.Add(checkGroup.Name, checkGroup);
                _checkGroupDictByLabel.Add(checkGroup.NameCell.Value.ToString(), checkGroup);
            }

            AllowHostsTextBox.Text = NRPEHostConfiguration.ALLOW_HOSTS_PLACE_HOLDER;
            AllowHostsTextBox.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
            AllowHostsTextBox.GotFocus += AllowHostsTextBox_GotFocus;
            AllowHostsTextBox.LostFocus += AllowHostsTextBox_LostFocus;

            _invalidParamToolTip = new ToolTip
            {
                IsBalloon = true,
                ToolTipIcon = ToolTipIcon.Warning,
                ToolTipTitle = Messages.INVALID_PARAMETER,
                Tag = AllowHostsTextBox
            };
            DisableAllComponent();
        }

        public string SubText
        {
            get
            {
                if (_isHost)
                {
                    return Messages.NRPE_EDIT_PAGE_TEXT;
                }
                else
                {
                    return Messages.NRPE_BATCH_CONFIGURATION;
                }
            }
        }

        public bool ValidToSave
        {
            get
            {
                _invalidParamToolTipText = "";
                _invalidParamToolTip.ToolTipTitle = "";
                CheckDataGridView.ShowCellToolTips = false;
                CheckDataGridView.ShowCellErrors = false;

                if (!EnableNRPECheckBox.Checked)
                {
                    return true;
                }

                bool valid = IsAllowHostsValid();

                foreach (CheckGroup checkGroup in _checkGroupList)
                {
                    if (!checkGroup.CheckValue())
                    {
                        CheckDataGridView.ShowCellToolTips = true;
                        CheckDataGridView.ShowCellErrors = true;
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
                if (_isHost && IsNRPEConfigurationChanged() ||
                    !_isHost && BatchConfigurationCheckBox.Checked)
                {
                    UpdateCurrentNRPEConfiguration();
                    return true;
                }
                return false;
            }
        }

        public void Cleanup()
        {
        }

        public void ShowLocalValidationMessages()
        {
            if (_invalidParamToolTip.Tag is Control ctrl)
            {
                _invalidParamToolTip.Hide(ctrl);
                if (!_invalidParamToolTipText.Equals(""))
                {
                    HelpersGUI.ShowBalloonMessage(ctrl, _invalidParamToolTip, _invalidParamToolTipText);
                }
            }
        }

        public void HideLocalValidationMessages()
        {
            if (_invalidParamToolTip.Tag is Control ctrl)
            {
                _invalidParamToolTip.Hide(ctrl);
            }
        }

        public AsyncAction SaveSettings()
        {
            return new NRPEUpdateAction(_clone, _nrpeCurrentConfig, _nrpeOriginalConfig, true);
        }

        public bool IsNRPEAvailable(IXenObject clone)
        {
            IXenObject checkHost;
            if (clone is Host h)
            {
                checkHost = h;
            }
            else if (clone is Pool p)
            {
                List<Host> hostList = p.Connection.Cache.Hosts.ToList();
                checkHost = hostList[0];
            }
            else
            {
                return false;
            }
            try
            {
                Host.call_plugin(checkHost.Connection.Session, checkHost.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME,
                    NRPEHostConfiguration.XAPI_NRPE_GET_THRESHOLD, null);
            }
            catch (Exception e)
            {
                log.InfoFormat("Execute NRPE plugin failed, failed reason: {0}. It may not support NRPE.", e.Message);
                return false;
            }
            return true;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _clone = clone;
            _isHost = _clone is Host;

            DescLabelHost.Visible = _isHost;
            DescLabelPool.Visible = !_isHost;
            BatchConfigurationCheckBox.Visible = !_isHost;
            UpdateRetrievingNRPETip(NRPEHostConfiguration.RetrieveNRPEStatus.Retrieving);
            DisableAllComponent();

            NRPERetrieveAction action = new NRPERetrieveAction(_clone, _nrpeOriginalConfig, _checkGroupDictByName, true);
            action.Completed += ActionCompleted;
            action.RunAsync();
        }

        private void ActionCompleted(ActionBase sender)
        {
            Program.Invoke(this.Parent, UpdateRetrieveStatus);
        }

        private void UpdateRetrieveStatus()
        {
            if (_nrpeOriginalConfig.Status == NRPEHostConfiguration.RetrieveNRPEStatus.Successful)
            {
                if (_isHost)
                {
                    EnableNRPECheckBox.Enabled = true;
                    UpdateComponentValueBasedConfiguration();
                    UpdateComponentStatusBasedEnableNRPECheckBox();
                }
                else
                {
                    BatchConfigurationCheckBox.Enabled = true;
                }
            }
            UpdateRetrievingNRPETip(_nrpeOriginalConfig.Status);
        }

        private void UpdateRetrievingNRPETip(NRPEHostConfiguration.RetrieveNRPEStatus status)
        {
            switch (status)
            {
                case NRPEHostConfiguration.RetrieveNRPEStatus.Retrieving:
                    RetrieveNRPELabel.Text = Messages.NRPE_RETRIEVING_CONFIGURATION;
                    RetrieveNRPEPicture.Image = Images.StaticImages.ajax_loader;
                    RetrieveNRPEPicture.Visible = true;
                    break;
                case NRPEHostConfiguration.RetrieveNRPEStatus.Successful:
                    RetrieveNRPELabel.Text = "";
                    RetrieveNRPEPicture.Image = Images.StaticImages._000_Tick_h32bit_16;
                    RetrieveNRPEPicture.Visible = false;
                    break;
                case NRPEHostConfiguration.RetrieveNRPEStatus.Failed:
                    RetrieveNRPELabel.Text = Messages.NRPE_RETRIEVE_FAILED;
                    RetrieveNRPEPicture.Image = Images.StaticImages._000_error_h32bit_16;
                    RetrieveNRPEPicture.Visible = true;
                    break;
                case NRPEHostConfiguration.RetrieveNRPEStatus.Unsupport:
                    RetrieveNRPELabel.Text = Messages.NRPE_UNSUPPORT;
                    RetrieveNRPEPicture.Image = Images.StaticImages._000_error_h32bit_16;
                    RetrieveNRPEPicture.Visible = true;
                    break;
                default:
                    break;
            }
        }

        private void DisableAllComponent()
        {
            EnableNRPECheckBox.Enabled = false;
            GeneralConfigureGroupBox.Enabled = false;
            CheckDataGridView.Enabled = false;
        }

        private void UpdateComponentValueBasedConfiguration()
        {
            EnableNRPECheckBox.Checked = _nrpeOriginalConfig.EnableNRPE;
            AllowHostsTextBox.Text = AllowHostsWithoutLocalAddress(_nrpeOriginalConfig.AllowHosts);
            AllowHostsTextBox.ForeColor = AllowHostsTextBox.Text.Equals(NRPEHostConfiguration.ALLOW_HOSTS_PLACE_HOLDER) ?
                Color.FromKnownColor(KnownColor.ControlDark) : AllowHostsTextBox.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            DebugLogCheckBox.Checked = _nrpeOriginalConfig.Debug;
            SslDebugLogCheckBox.Checked = _nrpeOriginalConfig.SslLogging;
        }

        private void UpdateComponentStatusBasedBatchConfigurationCheckBox()
        {
            if (BatchConfigurationCheckBox.Checked)
            {
                EnableNRPECheckBox.Enabled = true;
                UpdateComponentStatusBasedEnableNRPECheckBox();
            }
            else
            {
                DisableAllComponent();
            }
        }

        private void UpdateComponentStatusBasedEnableNRPECheckBox()
        {
            GeneralConfigureGroupBox.Enabled = EnableNRPECheckBox.Checked;
            CheckDataGridView.Enabled = EnableNRPECheckBox.Checked;
            CheckDataGridView.DefaultCellStyle.BackColor = EnableNRPECheckBox.Checked ?
                Color.FromKnownColor(KnownColor.Window) : Color.FromKnownColor(KnownColor.Control);
            if (_isHost)
            {
                foreach (var checkGroup in _checkGroupList)
                {
                    if (EnableNRPECheckBox.Checked)
                    {
                        checkGroup.WarningThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                        checkGroup.CriticalThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                    }
                    else
                    {
                        checkGroup.WarningThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
                        checkGroup.CriticalThresholdCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
                    }
                }
            }
        }

        private bool IsAllowHostsValid()
        {
            _invalidParamToolTip.ToolTipTitle = Messages.NRPE_ALLOW_HOSTS_ERROR_TITLE;
            _invalidParamToolTip.Tag = AllowHostsTextBox;
            CheckDataGridView.ShowCellToolTips = true;

            string str = AllowHostsTextBox.Text;
            if (str.Trim().Length == 0 || str.Trim().Equals(NRPEHostConfiguration.ALLOW_HOSTS_PLACE_HOLDER))
            {
                _invalidParamToolTipText = Messages.NRPE_ALLOW_HOSTS_EMPTY_ERROR;
                return false;
            }

            string[] hostArray = str.Split(',');
            for ( int i = 0; i < hostArray.Length; i++ )
            {
                if (!REGEX_IPV4.Match(hostArray[i].Trim()).Success &&
                    !REGEX_IPV4_CIDR.Match(hostArray[i].Trim()).Success &&
                    !REGEX_DOMAIN.Match(hostArray[i].Trim()).Success)
                {
                    _invalidParamToolTipText = Messages.NRPE_ALLOW_HOSTS_FORMAT_ERROR;
                    return false;
                }
                for ( int j = 0; j < i; j++ )
                {
                    if (hostArray[i].Trim().Equals(hostArray[j].Trim()))
                    {
                        _invalidParamToolTipText = Messages.NRPE_ALLOW_HOSTS_SAME_ADDRESS;
                        return false;
                    }
                }
            }
            foreach (string host in hostArray)
            {
                if (!REGEX_IPV4.Match(host.Trim()).Success &&
                    !REGEX_IPV4_CIDR.Match(host.Trim()).Success &&
                    !REGEX_DOMAIN.Match(host.Trim()).Success)
                {
                    _invalidParamToolTipText = Messages.NRPE_ALLOW_HOSTS_FORMAT_ERROR;
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
            return UpdatedAllowHosts.Length == 0 ? NRPEHostConfiguration.ALLOW_HOSTS_PLACE_HOLDER :
                UpdatedAllowHosts.Substring(0, UpdatedAllowHosts.Length - 1);
        }

        private bool IsNRPEConfigurationChanged()
        {
            if (_nrpeCurrentConfig.EnableNRPE != _nrpeOriginalConfig.EnableNRPE ||
                !_nrpeCurrentConfig.AllowHosts.Equals(_nrpeOriginalConfig.AllowHosts) ||
                _nrpeCurrentConfig.Debug != _nrpeOriginalConfig.Debug ||
                _nrpeCurrentConfig.SslLogging != _nrpeOriginalConfig.SslLogging)
            {
                return true;
            }
            foreach (KeyValuePair<string, NRPEHostConfiguration.Check> kvp in _nrpeCurrentConfig.CheckDict)
            {
                NRPEHostConfiguration.Check CurrentCheck = kvp.Value;
                _nrpeOriginalConfig.GetNRPECheck(kvp.Key, out NRPEHostConfiguration.Check OriginalCheck);
                if (CurrentCheck != null && OriginalCheck != null
                    && (!CurrentCheck.WarningThreshold.Equals(OriginalCheck.WarningThreshold)
                    || !CurrentCheck.CriticalThreshold.Equals(OriginalCheck.CriticalThreshold)))
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateCurrentNRPEConfiguration()
        {
            _nrpeCurrentConfig = new NRPEHostConfiguration
            {
                EnableNRPE = EnableNRPECheckBox.Checked,
                AllowHosts = AllowHostsTextBox.Text,
                Debug = DebugLogCheckBox.Checked,
                SslLogging = SslDebugLogCheckBox.Checked
            };
            foreach (KeyValuePair<string, CheckGroup> item in _checkGroupDictByName)
            {
                if (item.Value.WarningThresholdCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlText))
                    && item.Value.CriticalThresholdCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlText)))
                {
                    _nrpeCurrentConfig.AddNRPECheck(new NRPEHostConfiguration.Check(item.Key,
                        item.Value.WarningThresholdCell.Value.ToString(), item.Value.CriticalThresholdCell.Value.ToString()));
                }
            }
        }

        private void BatchConfigurationCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComponentStatusBasedBatchConfigurationCheckBox();
        }

        private void EnableNRPECheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComponentStatusBasedEnableNRPECheckBox();
        }

        private void AllowHostsTextBox_GotFocus(object sender, EventArgs e)
        {
            if (AllowHostsTextBox.Text.Equals(NRPEHostConfiguration.ALLOW_HOSTS_PLACE_HOLDER))
            {
                AllowHostsTextBox.Text = "";
            }
            AllowHostsTextBox.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
        }

        private void AllowHostsTextBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AllowHostsTextBox.Text))
            {
                AllowHostsTextBox.Text = NRPEHostConfiguration.ALLOW_HOSTS_PLACE_HOLDER;
                AllowHostsTextBox.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
            }
        }

        private void CheckDataGridView_BeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewCell currentCell = CheckDataGridView.CurrentRow?.Cells[e.ColumnIndex];
            
            if (currentCell != null && !_isHost && currentCell.Style.ForeColor.Equals(Color.FromKnownColor(KnownColor.ControlDark)))
            {
                currentCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                currentCell.Value = "";
            }
        }

        private void CheckDataGridView_EndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell currentCell = CheckDataGridView.CurrentRow?.Cells[e.ColumnIndex];
            
            if (currentCell != null &&!_isHost && currentCell.Value.ToString().Trim().Equals(""))
            {
                currentCell.Style.ForeColor = Color.FromKnownColor(KnownColor.ControlDark);
                _checkGroupDictByLabel.TryGetValue(CheckDataGridView.CurrentRow.Cells[0].Value.ToString(), out CheckGroup checkGroup);

                if (checkGroup != null)
                {
                    if (currentCell.ColumnIndex == WarningThresholdColumn.Index)
                        currentCell.Value = checkGroup.WarningThresholdDefault;
                    else if (currentCell.ColumnIndex == CriticalThresholdColumn.Index)
                        currentCell.Value = checkGroup.CriticalThresholdDefault;
                }
                _nrpeOriginalConfig.CheckDict.TryGetValue(checkGroup.Name, out NRPEHostConfiguration.Check check);
            }
        }
    }
}
