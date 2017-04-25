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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;
using System.Collections.Generic;
using System.Linq;

namespace XenAdmin.SettingsPanels
{
    public partial class HostPowerONEditPage : UserControl, IEditPage
    {
        private enum PowerOnMode { Disabled, WakeOnLan, iLO, DRAC, Custom }
        private const string POWER_ON_IP = "power_on_ip";
        private const string POWER_ON_USER = "power_on_user";
        private const string POWER_ON_PASSWORD_SECRET = "power_on_password_secret";
        protected Host _host;
        private readonly ToolTip _invalidParamToolTip;

        public HostPowerONEditPage()
        {
            InitializeComponent();
            Text = Messages.POWER_ON;

            _invalidParamToolTip = new ToolTip
                                       {
                                           IsBalloon = true,
                                           ToolTipTitle = Messages.ERROR_INVALID_IP
                                       };
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public virtual string SubText
        {
            get
            {
                if (_host == null)
                    return "";

                return PoolPowerONEditPage.GetFullNameMode(_host.power_on_mode);
            }
        }

        public Image Image
        {
            get
            {
                return Resources._001_PowerOn_h32bit_16;
            }
        }

        public virtual AsyncAction SaveSettings()
        {
            string newMode, ip, user, password;
            Dictionary<string, string> customConfig;
            GetConfig(out newMode, out ip, out user, out password, out customConfig);
            return new SavePowerOnSettingsAction(_host, newMode, ip, user, password, customConfig, true);
        }

        protected void GetConfig(out string newMode, out string ip, out string user, out string password, out Dictionary<string, string> customConfig)
        {
            ip = textBoxInterface.Text;
            user = textBoxUser.Text;
            password = textBoxPassword.Text;
            customConfig = new Dictionary<string, string>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    string key = (row.Cells[0].Value == null ? null : row.Cells[0].Value.ToString());
                    string value = (row.Cells[1].Value == null ? null : row.Cells[1].Value.ToString());
                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value) && !customConfig.ContainsKey(key))
                        customConfig.Add(key, value);
                }
            }

            if (NewPowerOnMode == PowerOnMode.Disabled)
                newMode = string.Empty;
            else if (NewPowerOnMode == PowerOnMode.WakeOnLan)
                newMode = "wake-on-lan";
            else if (NewPowerOnMode == PowerOnMode.iLO)
                newMode = "iLO";
            else if (NewPowerOnMode == PowerOnMode.DRAC)
                newMode = "DRAC";
            else
                newMode = textBoxCustom.Text;
        }

        public virtual void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _host = (Host)clone;
            switch (HostPowerOnMode)
            {
                case PowerOnMode.Disabled:
                    radioButtonDisabled.Checked = true;
                    break;
                case PowerOnMode.WakeOnLan:
                    radioButtonWakeonLAN.Checked = true;
                    break;
                case PowerOnMode.iLO:
                    radioButtonILO.Checked = true;
                    break;
                case PowerOnMode.DRAC:
                    radioButtonDRAC.Checked = true;
                    break;
                default:
                    radioButtonCustom.Checked = true;
                    break;
            }
            FillCurrentCredentials();
        }

        protected void FillCurrentCredentials()
        {
            switch (HostPowerOnMode)
            {
                case PowerOnMode.iLO:
                case PowerOnMode.DRAC:
                    //Set power_on_ip
                    string power_on_ip = "";
                    _host.power_on_config.TryGetValue(POWER_ON_IP, out power_on_ip);
                    textBoxInterface.Text = power_on_ip;

                    //Set power_on_user
                    string power_on_user = "";
                    _host.power_on_config.TryGetValue(POWER_ON_USER, out power_on_user);
                    textBoxUser.Text = power_on_user;
                    try
                    {
                        string opaqueref = Secret.get_by_uuid(_host.Connection.Session, _host.power_on_config[POWER_ON_PASSWORD_SECRET]);
                        textBoxPassword.Text = Secret.get_value(_host.Connection.Session, opaqueref);
                    }
                    catch (Exception)
                    {
                        textBoxPassword.Text = "";
                    }
                    break;
                case PowerOnMode.Custom:
                    textBoxCustom.Text = _host.power_on_mode;
                    dataGridView1.Rows.Clear();
                    foreach (KeyValuePair<string, string> pair in _host.power_on_config)
                    {
                        if (pair.Key == POWER_ON_PASSWORD_SECRET)
                        {
                            string password = "";
                            try
                            {
                                string opaqueref = Secret.get_by_uuid(_host.Connection.Session, pair.Value);
                                password = Secret.get_value(_host.Connection.Session, opaqueref);
                            }
                            catch (Exception)
                            {
                            }
                            finally
                            {
                                dataGridView1.Rows.Add(pair.Key, password);
                            }
                        }
                        else
                            dataGridView1.Rows.Add(pair.Key, pair.Value);
                    }
                    break;
            }
        }

        public bool ValidToSave
        {
            get
            {
                if (NewPowerOnMode == PowerOnMode.iLO || NewPowerOnMode == PowerOnMode.DRAC)
                    return StringUtility.IsIPAddress(textBoxInterface.Text);
                return true;
            }
        }

        public void ShowLocalValidationMessages()
        {
            HelpersGUI.ShowBalloonMessage(textBoxInterface, _invalidParamToolTip);
        }

        public void Cleanup()
        {
            if (_invalidParamToolTip != null)
                _invalidParamToolTip.Dispose();
        }

        public bool HasChanged
        {
            get
            {
                string newMode, ip, user, password;
                Dictionary<string, string> customConfig;
                GetConfig(out newMode, out ip, out user, out password, out customConfig);
                if (_host.power_on_mode != newMode)
                    return true;
                if (!Helper.AreEqual(_host.power_on_config, customConfig))
                    return true;

                if (CheckConfig(POWER_ON_IP, ip))
                    return true;
                if (CheckConfig(POWER_ON_USER, user))
                    return true;
                if (CheckConfig(POWER_ON_PASSWORD_SECRET, password))
                    return true;

                return false;
            }
        }

        private bool CheckConfig(string key, string newvalue)
        {
            string value;
            if (_host.power_on_config.TryGetValue(key, out value))
            {
                if (newvalue != value)
                    return true;
            }
            return false;
        }

        private void resetGroupBoxCredentials(bool enabled, int height, Control newControl)
        {
            groupBoxCredentials.Controls.Clear();
            textBoxInterface.Text = textBoxUser.Text = textBoxPassword.Text = "";
            groupBoxCredentials.Height = height;
            groupBoxCredentials.Controls.Add(newControl);
            groupBoxCredentials.Enabled = enabled;
        }

        private PowerOnMode HostPowerOnMode
        {
            get
            {
                if (string.IsNullOrEmpty(_host.power_on_mode))
                    return PowerOnMode.Disabled;
                if (_host.power_on_mode == "wake-on-lan")
                    return PowerOnMode.WakeOnLan;
                if (_host.power_on_mode == "iLO")
                    return PowerOnMode.iLO;
                if (_host.power_on_mode == "DRAC")
                    return PowerOnMode.DRAC;
                
                return PowerOnMode.Custom;
            }
        }

        private readonly PowerOnMode[] PowerOnModeNeedsCredentials = new[]
                                                                         {
                                                                             PowerOnMode.iLO, PowerOnMode.DRAC,
                                                                             PowerOnMode.Custom
                                                                         };

        private bool CanRestoreCurrentCredentials(PowerOnMode powerOnMode)
        {
            return powerOnMode == HostPowerOnMode && PowerOnModeNeedsCredentials.Contains(powerOnMode);
        }

        private PowerOnMode _powerOnMode;
        private PowerOnMode NewPowerOnMode
        {
            get { return _powerOnMode; }
            set
            {
                _powerOnMode = value;
                switch (value)
                {
                    case PowerOnMode.Disabled:
                    case PowerOnMode.WakeOnLan:
                        resetGroupBoxCredentials(false, 100, tableLayoutPanelCredentials);
                        break;
                    case PowerOnMode.iLO:
                    case PowerOnMode.DRAC:
                        resetGroupBoxCredentials(true, 100, tableLayoutPanelCredentials);
                        break;
                    default: //custom power on mode
                        resetGroupBoxCredentials(true, 150, dataGridView1);
                        break;
                }

                if (CanRestoreCurrentCredentials(value))
                    FillCurrentCredentials();
            }
        }

        private bool RadioButtonChecked(object sender)
        {
            return sender is RadioButton && ((RadioButton) sender).Checked;
        }

        private void radioButtonDisabled_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButtonChecked(sender))
                NewPowerOnMode = PowerOnMode.Disabled;
        }
        
        private void radioButtonWakeonLAN_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButtonChecked(sender))
                NewPowerOnMode = PowerOnMode.WakeOnLan;
        }

        private void radioButtonILO_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButtonChecked(sender))
                NewPowerOnMode = PowerOnMode.iLO;
        }

        private void radioButtonDRAC_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButtonChecked(sender))
                NewPowerOnMode = PowerOnMode.DRAC;
        }

        private void radioButtonCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioButtonChecked(sender))
            {
                NewPowerOnMode = PowerOnMode.Custom;
                textBoxCustom.Select();
            }
        }


        public class DataGridViewKey : DataGridView
        {
            protected override bool ProcessDialogKey(Keys keyData)
            {
                Keys key = (keyData & Keys.KeyCode);
                if (key == Keys.Delete)
                {
                    removeSelected();
                    return true;
                }
                return base.ProcessDialogKey(keyData);
            }

            private void removeSelected()
            {
                foreach (DataGridViewRow row in SelectedRows)
                {
                    if (!row.IsNewRow)
                        this.Rows.Remove(row);
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string newkey = (string)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (e.ColumnIndex == 0)
            {
                bool foundRepeated = false;
                foreach (DataGridViewRow row in this.dataGridView1.Rows)
                {
                    if ((string)(row.Cells[0].Value) == newkey && row.Index != e.RowIndex)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = Messages.REPEATED_KEY;
                        foundRepeated = true;
                    }
                }
                if (!foundRepeated)
                {
                    foreach (DataGridViewRow row in this.dataGridView1.Rows)
                    {
                        row.Cells[0].ErrorText = "";
                    }
                }
                
            }
        }

        private void textBoxCustom_Click(object sender, EventArgs e)
        {
            radioButtonCustom.Checked = true;
        }
    }
}
