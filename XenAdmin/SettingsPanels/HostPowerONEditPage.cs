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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XenAdmin.Network;
using XenCenterLib;


namespace XenAdmin.SettingsPanels
{
    public partial class HostPowerONEditPage : UserControl, IEditPage
    {
        private IXenObject _clone;
        private IXenConnection _connection;
        private readonly ToolTip _invalidParamToolTip;
        private readonly Dictionary<Host, List<Host.PowerOnMode>> _hostModes = new Dictionary<Host, List<Host.PowerOnMode>>();
        private bool _isLoadedOnce;
        private bool _programmaticUpdate;

        public HostPowerONEditPage()
        {
            InitializeComponent();
            _invalidParamToolTip = new ToolTip {IsBalloon = true};
        }

        public sealed override string Text => Messages.POWER_ON;

        public virtual string SubText
        {
            get
            {
                var specifiedModes = (from List<Host.PowerOnMode> modes in _hostModes.Values
                    let active = modes.FirstOrDefault(m => m.Active)
                    where active != null
                    select active).Distinct().ToList();

                switch (specifiedModes.Count)
                {
                    case 0:
                        return "";
                    case 1:
                        return specifiedModes[0].FriendlyName;
                    default:
                        return Messages.MIXED_POWER_ON_MODE;
                }
            }
        }

        public Image Image => Images.StaticImages._001_PowerOn_h32bit_16;

        public virtual AsyncAction SaveSettings()
        {
            var changedHosts = from KeyValuePair<Host, List<Host.PowerOnMode>> kvp in _hostModes
                let activeMode = kvp.Value.FirstOrDefault(m => m.Active)
                where HasHostChanged(kvp.Key, activeMode)
                select new KeyValuePair<Host, Host.PowerOnMode>(kvp.Key, activeMode);

            return new SavePowerOnSettingsAction(_connection, changedHosts.ToList(), true);
        }

        public virtual void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _clone = clone;
            _connection = clone.Connection;

            var hosts = new List<Host>();
            if (_clone is Host h)
                hosts = new List<Host> {h};
            else if (_clone is Pool p)
                hosts = p.Connection.Cache.Hosts.ToList();

            if (Helpers.StockholmOrGreater(_connection))
                radioButtonILO.Visible = false;

            foreach (var host in hosts)
            {
                var mode = Host.PowerOnMode.Create(host);
                mode.Active = true;
                _hostModes.Add(host, new List<Host.PowerOnMode> {mode});
            }
        }

        public bool HasChanged =>
            _isLoadedOnce && _hostModes.Any(kvp => HasHostChanged(kvp.Key, kvp.Value.FirstOrDefault(m => m.Active)));
        
        public bool ValidToSave
        {
            get
            {
                if (!_isLoadedOnce)
                    return true;

                foreach (var kvp in _hostModes)
                {
                    var mode = kvp.Value.FirstOrDefault(m => m.Active);
                    
                    if (mode is Host.PowerOnModeiLO ilo && !StringUtility.IsIPAddress(ilo.IpAddress) ||
                        mode is Host.PowerOnModeDRAC drac && !StringUtility.IsIPAddress(drac.IpAddress))
                    {
                        _invalidParamToolTip.ToolTipTitle = _hostModes.Count == 1
                            ? Messages.ERROR_INVALID_IP
                            : $"{kvp.Key.Name()}: {Messages.ERROR_INVALID_IP}";
                        _invalidParamToolTip.Tag = textBoxInterface;
                        return false;
                    }

                    if (mode is Host.PowerOnModeCustom && string.IsNullOrEmpty(textBoxCustom.Text.Trim()))
                    {
                        _invalidParamToolTip.ToolTipTitle = _hostModes.Count == 1
                            ? Messages.POWER_ON_CUSTOM_MODE_ERROR
                            : $"{kvp.Key.Name()}: {Messages.POWER_ON_CUSTOM_MODE_ERROR}";
                        _invalidParamToolTip.Tag = textBoxCustom;
                        return false;
                    }
                }

                return true;
            }
        }

        public void ShowLocalValidationMessages()
        {
            if (_invalidParamToolTip.Tag is Control ctrl)
                HelpersGUI.ShowBalloonMessage(ctrl, _invalidParamToolTip);
        }
        public void HideLocalValidationMessages()
        {
            if (_invalidParamToolTip.Tag is Control ctrl)
            {
                if (ctrl != null)
                {
                    _invalidParamToolTip.Hide(ctrl);
                }
            }
        }


        public void Cleanup()
        {
            if (bgWorker.IsBusy)
                bgWorker.CancelAsync();

            if (_invalidParamToolTip != null)
                _invalidParamToolTip.Dispose();
        }

        public void LoadPowerOnMode()
        {
            if (!_isLoadedOnce)
            {
                labelServer.Visible = _clone is Host;
                labelPool.Visible = tableLayoutPanelHosts.Visible = !labelServer.Visible;
                
                Enabled = false;
                spinnerIcon1.Visible = true;
                spinnerIcon1.StartSpinning();
                bgWorker.RunWorkerAsync();

                _isLoadedOnce = true;
            }
        }

        private List<Host> SelectedHosts =>
            (from DataGridViewRow row in dataGridViewHosts.SelectedRows select row.Tag as Host).ToList();

        private bool HasHostChanged(Host host, Host.PowerOnMode activeMode)
        {
            if (activeMode == null)
                return false;

            if (host.power_on_mode != activeMode.ToString())
                return true;

            var customConfig = activeMode.Config;

            if (host.power_on_config.Count != customConfig.Count)
                return true;

            foreach (var kvp in host.power_on_config)
            {
                if (!customConfig.ContainsKey(kvp.Key) || kvp.Value != customConfig[kvp.Key])
                    return true;
            }

            return false;
        }

        private void UpdateModeFromCredentials()
        {
            if (_programmaticUpdate)
                return;

            var selectedHosts = SelectedHosts;

            foreach (var selectedHost in selectedHosts)
            {
                var mode = _hostModes[selectedHost].FirstOrDefault(m => m.Active);

                if (mode != null)
                    _hostModes[selectedHost].RemoveAll(m => m.Active);

                if (radioButtonILO.Checked)
                    _hostModes[selectedHost].Add(new Host.PowerOnModeiLO
                    {
                        IpAddress = textBoxInterface.Text,
                        Username = textBoxUser.Text,
                        Password = textBoxPassword.Text,
                        Active = true
                    });
                else if (radioButtonDRAC.Checked)
                    _hostModes[selectedHost].Add(new Host.PowerOnModeDRAC
                    {
                        IpAddress = textBoxInterface.Text,
                        Username = textBoxUser.Text,
                        Password = textBoxPassword.Text,
                        Active = true
                    });
            }
        }

        private void UpdateModeFromGridView()
        {
            if (_programmaticUpdate)
                return;

            var selectedHosts = SelectedHosts;

            foreach (var selectedHost in selectedHosts)
            {
                var mode = _hostModes[selectedHost].FirstOrDefault(m => m.Active);

                if (mode != null)
                    _hostModes[selectedHost].RemoveAll(m => m.Active);

                var newMode = new Host.PowerOnModeCustom {CustomMode = textBoxCustom.Text.Trim(), Active = true};
            
                foreach (DataGridViewRow row in dataGridViewCustom.Rows)
                {
                    if (row.IsNewRow)
                        continue;
                
                    string key = row.Cells[0].Value?.ToString();
                    string value = row.Cells[1].Value?.ToString();

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                        newMode.CustomConfig[key] = value;
                }

                _hostModes[selectedHost].Add(newMode);
            }
        }

        private void UpdateHostGridView(Host selectedHost)
        {
            foreach (DataGridViewRow row in dataGridViewHosts.Rows)
            {
                if (selectedHost.Equals(row.Tag))
                {
                    row.Cells[1].Value = _hostModes[selectedHost].FirstOrDefault(m => m.Active)?.FriendlyName;
                    break;
                }
            }
        }
        
        #region Control event handlers

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var kvp in _hostModes)
            foreach (var mode in kvp.Value)
            {
                mode.Load(kvp.Key);
                mode.Active = true;
            }
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            spinnerIcon1.StopSpinning();
            spinnerIcon1.Visible = false;
            Enabled = true;

            var rows = new List<DataGridViewRow>();
            foreach (var kvp in _hostModes)
            {
                var row = new DataGridViewRow();
                row.Cells.AddRange(new DataGridViewTextBoxCell {Value = kvp.Key.Name()},
                    new DataGridViewTextBoxCell {Value = kvp.Value[0].FriendlyName});
                row.Tag = kvp.Key;
                rows.Add(row);
            }

            dataGridViewHosts.Rows.AddRange(rows.ToArray());

            if (dataGridViewHosts.RowCount > 0)
                dataGridViewHosts.Rows[0].Selected = true;
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            dataGridViewHosts.SelectAll();
        }

        private void dataGridViewHosts_SelectionChanged(object sender, EventArgs e)
        {
            var selectedHosts = SelectedHosts;

            var specifiedModes = (from KeyValuePair<Host, List<Host.PowerOnMode>> hm in _hostModes
                where selectedHosts.Contains(hm.Key)
                let active = hm.Value.FirstOrDefault(m => m.Active)
                where active != null
                select active).Distinct().ToList();

            //If a RadioButton is already checked, uncheck it first, otherwise the handler
            //won't be called and the _hostModes won't be updated

            if (specifiedModes.Count == 1)
            {
                var type = specifiedModes[0].GetType();

                if (type == typeof(Host.PowerOnModeDisabled))
                {
                    radioButtonDisabled.Checked = false;
                    radioButtonDisabled.Checked = true;
                }
                else if (type == typeof(Host.PowerOnModeWakeOnLan))
                {
                    radioButtonWakeonLAN.Checked = false;
                    radioButtonWakeonLAN.Checked = true;
                }
                else if (type == typeof(Host.PowerOnModeiLO))
                {
                    radioButtonILO.Checked = false;
                    radioButtonILO.Checked = true;
                }
                else if (type == typeof(Host.PowerOnModeDRAC))
                {
                    radioButtonDRAC.Checked = false;
                    radioButtonDRAC.Checked = true;
                }
                else
                {
                    radioButtonCustom.Checked = false;
                    radioButtonCustom.Checked = true;
                }
            }
            else if (dataGridViewHosts.SelectedRows.Count > 1)
            {
                radioButtonDisabled.Checked = false;
                radioButtonDisabled.Checked = true;
            }
        }

        private void textBoxInterface_TextChanged(object sender, EventArgs e)
        {
            UpdateModeFromCredentials();
        }

        private void textBoxUser_TextChanged(object sender, EventArgs e)
        {
            UpdateModeFromCredentials();
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            UpdateModeFromCredentials();
        }

        private void textBoxCustom_TextChanged(object sender, EventArgs e)
        {
            //update the mode first because it is used for updating dataGridViewHosts
            UpdateModeFromGridView();

            var selectedHosts = SelectedHosts;
            foreach (var selectedHost in selectedHosts)
                UpdateHostGridView(selectedHost);
        }

        private void textBoxCustom_Click(object sender, EventArgs e)
        {
            radioButtonCustom.Checked = true;
        }

        private void radioButtonDisabled_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonDisabled.Checked)
                return;

            var selectedHosts = SelectedHosts;

            foreach (var selectedHost in selectedHosts)
            {
                if (_hostModes[selectedHost].All(m => m.GetType() != typeof(Host.PowerOnModeDisabled)))
                    _hostModes[selectedHost].Add(new Host.PowerOnModeDisabled());

                foreach (var m in _hostModes[selectedHost].ToList())
                    m.Active = m.GetType() == typeof(Host.PowerOnModeDisabled);

                UpdateHostGridView(selectedHost);
            }

            textBoxCustom.Visible = false;
            groupBoxCredentials.Visible = false;
        }
        
        private void radioButtonWakeonLAN_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonWakeonLAN.Checked)
                return;

            var selectedHosts = SelectedHosts;

            foreach (var selectedHost in selectedHosts)
            {
                if (_hostModes[selectedHost].All(m => m.GetType() != typeof(Host.PowerOnModeWakeOnLan)))
                    _hostModes[selectedHost].Add(new Host.PowerOnModeWakeOnLan());

                foreach (var m in _hostModes[selectedHost].ToList())
                    m.Active = m.GetType() == typeof(Host.PowerOnModeWakeOnLan);

                UpdateHostGridView(selectedHost);
            }

            textBoxCustom.Visible = false;
            groupBoxCredentials.Visible = false;
        }

        private void radioButtonILO_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonILO.Checked)
                return;

            var selectedHosts = SelectedHosts;

            try
            {
                _programmaticUpdate = true;

                foreach (var selectedHost in selectedHosts)
                {
                    if (_hostModes[selectedHost].All(m => m.GetType() != typeof(Host.PowerOnModeiLO)))
                        _hostModes[selectedHost].Add(new Host.PowerOnModeiLO());

                    textBoxInterface.Text = textBoxUser.Text = textBoxPassword.Text = "";

                    foreach (var m in _hostModes[selectedHost].ToList())
                    {
                        var mode = m as Host.PowerOnModeiLO;
                        m.Active = mode != null;
                        if (mode != null)
                        {
                            textBoxInterface.Text = mode.IpAddress;
                            textBoxUser.Text = mode.Username;
                            textBoxPassword.Text = mode.Password;
                        }
                    }

                    UpdateHostGridView(selectedHost);
                }
            }
            finally
            {
                _programmaticUpdate = false;
            }

            textBoxCustom.Visible = false;
            dataGridViewCustom.Visible = false;
            tableLayoutPanelCredentials.Visible = true;
            groupBoxCredentials.Visible = true;
        }

        private void radioButtonDRAC_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonDRAC.Checked)
                return;

            var selectedHosts = SelectedHosts;

            try
            {
                _programmaticUpdate = true;

                foreach (var selectedHost in selectedHosts)
                {
                    if (_hostModes[selectedHost].All(m => m.GetType() != typeof(Host.PowerOnModeDRAC)))
                        _hostModes[selectedHost].Add(new Host.PowerOnModeDRAC());

                    textBoxInterface.Text = textBoxUser.Text = textBoxPassword.Text = "";

                    foreach (var m in _hostModes[selectedHost].ToList())
                    {
                        var mode = m as Host.PowerOnModeDRAC;
                        m.Active = mode != null;
                        if (mode != null)
                        {
                            textBoxInterface.Text = mode.IpAddress;
                            textBoxUser.Text = mode.Username;
                            textBoxPassword.Text = mode.Password;
                        }
                    }

                    UpdateHostGridView(selectedHost);
                }
            }
            finally
            {
                _programmaticUpdate = false;
            }

            textBoxCustom.Visible = false;
            dataGridViewCustom.Visible = false;
            tableLayoutPanelCredentials.Visible = true;
            groupBoxCredentials.Visible = true;
        }

        private void radioButtonCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonCustom.Checked)
                return;

            var selectedHosts = SelectedHosts;

            try
            {
                _programmaticUpdate = true;

                foreach (var selectedHost in selectedHosts)
                {
                    if (_hostModes[selectedHost].All(m => m.GetType() != typeof(Host.PowerOnModeCustom)))
                        _hostModes[selectedHost].Add(new Host.PowerOnModeCustom());

                    textBoxCustom.Text = "";
                    textBoxCustom.Select();
                    dataGridViewCustom.Rows.Clear();

                    foreach (var m in _hostModes[selectedHost].ToList())
                    {
                        var mode = m as Host.PowerOnModeCustom;
                        m.Active = mode != null;
                        if (mode != null)
                            textBoxCustom.Text = mode.CustomMode;

                        if (mode != null)
                            foreach (var pair in mode.CustomConfig)
                                dataGridViewCustom.Rows.Add(pair.Key, pair.Value);
                    }

                    UpdateHostGridView(selectedHost);
                }
            }
            finally
            {
                _programmaticUpdate = false;
            }

            textBoxCustom.Visible = true;
            dataGridViewCustom.Visible = true;
            tableLayoutPanelCredentials.Visible = false;
            groupBoxCredentials.Visible = true;
        }

        private void dataGridViewCustom_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            UpdateModeFromGridView();
        }

        private void dataGridViewCustom_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpdateModeFromGridView();
        }

        private void dataGridViewCustom_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                var keys = new Dictionary<string, int>();

                foreach (DataGridViewRow row in dataGridViewCustom.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    var key = (string)row.Cells[0].Value;
                    if (string.IsNullOrEmpty(key))
                        continue;

                    if (keys.ContainsKey(key))
                        keys[key]++;
                    else
                        keys[key] = 1;
                }

                var duplicates = keys.Where(k => k.Value > 1).Select(k => k.Key).ToList();

                foreach (DataGridViewRow row in dataGridViewCustom.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    var key = row.Cells[0].Value as string;

                    if (string.IsNullOrEmpty(key))
                        row.Cells[0].ErrorText = Messages.EMPTY_KEY;
                    else if (duplicates.Contains(key))
                        row.Cells[0].ErrorText = Messages.REPEATED_KEY;
                    else
                        row.Cells[0].ErrorText = string.Empty;
                }
            }

            UpdateModeFromGridView();
        }

        private void dataGridViewCustom_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dataGridViewCustom.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dataGridViewCustom_KeyDown(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyData & Keys.KeyCode;

            if (key != Keys.Delete)
                return;

            foreach (DataGridViewCell cell in dataGridViewCustom.SelectedCells)
            {
                var row = cell.OwningRow;
                if (row != null && !row.IsNewRow)
                    dataGridViewCustom.Rows.Remove(row);
            }
        }

        #endregion
    }
}
