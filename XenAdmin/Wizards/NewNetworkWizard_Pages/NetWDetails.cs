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
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    public partial class NetWDetails : XenTabPage
    {
        private List<int> vlans;
        private bool _vlanError;
        private bool _mtuError;
        private bool _populatingNics;

        public NetWDetails()
        {
            InitializeComponent();
            //non-browsable events
            numericUpDownVLAN.TextChanged += numericUpDownVLAN_TextChanged;
            numericUpDownMTU.TextChanged += numericUpDownMTU_TextChanged;

            numericUpDownMTU.Maximum = XenAPI.Network.MTU_MAX;
            numericUpDownMTU.Minimum = XenAPI.Network.MTU_MIN;
            numericUpDownMTU.Value = XenAPI.Network.MTU_DEFAULT;
            numericUpDownVLAN.Maximum = 4094;
        }

        public override string Text => Messages.NETW_DETAILS_TEXT;

        public override string PageTitle =>
            SelectedNetworkType == NetworkTypes.External
                ? Messages.NETW_EXTERNAL_DETAILS_TITLE
                : Messages.NETW_INTERNAL_DETAILS_TITLE;

        public override bool EnableNext()
        {
            if (_vlanError || _mtuError)
                return false;

            return SelectedHostNic != null || !comboBoxNICList.Visible;
        }

        public override void PopulatePage()
        {
            var external = SelectedNetworkType == NetworkTypes.External;

            labelExternal.Visible = external;
            labelInternal.Visible = !external;
            labelNIC.Visible = external;
            comboBoxNICList.Visible = external;

            if (comboBoxNICList.Visible)
            {
                try
                {
                    _populatingNics = true;
                    comboBoxNICList.Items.Clear();

                    foreach (PIF ThePIF in Connection.Cache.PIFs)
                    {
                        if (ThePIF.host.opaque_ref == Host.opaque_ref && ThePIF.IsPhysical() &&
                            (Properties.Settings.Default.ShowHiddenVMs || ThePIF.Show(Properties.Settings.Default.ShowHiddenVMs)) &&
                            !ThePIF.IsBondMember())
                        {
                            comboBoxNICList.Items.Add(ThePIF);
                        }
                    }
                }
                finally
                {
                    _populatingNics = false;
                }

                if (comboBoxNICList.Items.Count > 0)
                    comboBoxNICList.SelectedIndex = 0;

                comboBoxNICList.Focus();
            }

            labelVLAN.Visible = external;
            numericUpDownVLAN.Visible = external;
            numericUpDownVLAN.Minimum = Helpers.VLAN0Allowed(Connection) ? 0 : 1;
            numericUpDownMTU.Visible = labelMTU.Visible = infoMtuPanel.Visible = external;

            checkBoxSriov.Visible = SelectedHostNic != null && SelectedHostNic.IsSriovPhysicalPIF();

            OnPageUpdated();
        }

        #region Accessors

        public NetworkTypes SelectedNetworkType { private get; set; }

        public Host Host { private get; set; }

        public PIF SelectedHostNic => comboBoxNICList.SelectedItem as PIF;

        public int VLAN => Convert.ToInt32(Math.Round(numericUpDownVLAN.Value, MidpointRounding.AwayFromZero));

        public bool AddNicToVmsAutomatically => checkBoxAutomatic.Checked;

        public bool CreateVlanOnSriovNetwork => checkBoxSriov.Visible && checkBoxSriov.Checked;

        /// <summary>
        /// Returns -1 if the custom MTU option is disabled
        /// </summary>
        public int MTU => numericUpDownMTU.Visible && numericUpDownMTU.Enabled
            ? Convert.ToInt32(Math.Round(numericUpDownMTU.Value, MidpointRounding.AwayFromZero))
            : -1;

        #endregion

        private List<int> GetVLANList(PIF nic)
        {
            List<int> vlans = new List<int>();
            foreach (PIF pif in nic.Connection.Cache.PIFs)
            {
                if (pif.device == nic.device)
                {
                    var pifIsSriov = pif.NetworkSriov() != null;
                    if ((CreateVlanOnSriovNetwork && pifIsSriov) || (!CreateVlanOnSriovNetwork && !pifIsSriov))
                        vlans.Add((int)pif.VLAN);
                }
            }

            return vlans;
        }

        private void ValidateVLANValue()
        {
            //CA-192746: do not call numericUpDown.Value or properties/methods that call it
            //in the validation method, because it auto-corrects what the user has typed
            
            _vlanError = false;
            string msg = null;

            if (!int.TryParse(numericUpDownVLAN.Text.Trim(), out int currentValue))
            {
                _vlanError = true;
                msg = Messages.INVALID_NUMBER;
            }
            else if (currentValue < numericUpDownVLAN.Minimum || numericUpDownVLAN.Maximum < currentValue)
            {
                _vlanError = true;
                msg = string.Format(Messages.NETW_DETAILS_VLAN_RANGE, numericUpDownVLAN.Minimum, numericUpDownVLAN.Maximum);
            }
            else if (vlans != null && vlans.Contains(currentValue))
            {
                _vlanError = true;
                msg = Messages.NETW_DETAILS_VLAN_NUMBER_IN_USE;
            }
            else if (currentValue == 0)
            {
                msg = Messages.NETW_VLAN_ZERO;
            }

            if (_vlanError)
            {
                pictureBoxVlan.Image = Images.StaticImages._000_error_h32bit_16;
                labelVlanMessage.Text = msg;
                infoVlanPanel.Visible = true;
            }
            else if (!string.IsNullOrEmpty(msg))
            {
                pictureBoxVlan.Image = Images.StaticImages._000_Info3_h32bit_16;
                labelVlanMessage.Text = msg;
                infoVlanPanel.Visible = true;
            }
            else
                infoVlanPanel.Visible = false;

            OnPageUpdated();
        }

        private void ValidateMtuValue()
        {
            //CA-192746: do not call numericUpDown.Value or properties/methods that call it
            //in the validation method, because it auto-corrects what the user has typed

            _mtuError = false;

            if (!int.TryParse(numericUpDownMTU.Text.Trim(), out int currentValue))
            {
                _mtuError = true;
                pictureBoxMtu.Image = Images.StaticImages._000_error_h32bit_16;
                labelMtuMessage.Text = Messages.INVALID_NUMBER;
            }
            else
            {
                if (currentValue < numericUpDownMTU.Minimum || numericUpDownMTU.Maximum < currentValue)
                    _mtuError = true;

                pictureBoxMtu.Image = Images.StaticImages._000_Info3_h32bit_16;
                labelMtuMessage.Text = numericUpDownMTU.Minimum == numericUpDownMTU.Maximum
                    ? string.Format(Messages.ALLOWED_MTU_VALUE, numericUpDownMTU.Minimum)
                    : string.Format(Messages.ALLOWED_MTU_RANGE, numericUpDownMTU.Minimum, numericUpDownMTU.Maximum);
            }

            infoMtuPanel.Visible = true;
            OnPageUpdated();
        }

        #region Event Handlers

        private void cmbHostNicList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_populatingNics || SelectedHostNic == null)
                return;

            checkBoxSriov.Visible = SelectedHostNic.IsSriovPhysicalPIF();

            numericUpDownMTU.Maximum = Math.Min(SelectedHostNic.MTU, XenAPI.Network.MTU_MAX);
            numericUpDownMTU.Enabled = numericUpDownMTU.Minimum != numericUpDownMTU.Maximum;
            ValidateMtuValue();

            vlans = GetVLANList(SelectedHostNic);

            //CA-72484: check whether the currently selected VLAN is available and keep it
            if (!vlans.Contains(VLAN))
            {
                ValidateVLANValue();
                return;
            }

            //CA-19111: VLAN values should only go up to the numericUpDownVLAN.Maximum (4094)
            for (int i = 1; i <= numericUpDownVLAN.Maximum; i++)
            {
                if (!vlans.Contains(i))
                {
                    numericUpDownVLAN.Value = i;
                    break;
                }
            }

            OnPageUpdated();
        }

        private void numericUpDownVLAN_Leave(object sender, EventArgs e)
        {
            if (numericUpDownVLAN.Text == "")
                numericUpDownVLAN.Text = VLAN.ToString();
        }

        private void numericUpDownVLAN_TextChanged(object sender, EventArgs e)
        {
            ValidateVLANValue();
        }

        private void numericUpDownVLAN_ValueChanged(object sender, EventArgs e)
        {
            ValidateVLANValue();
        }

        private void numericUpDownMTU_Leave(object sender, EventArgs e)
        {
            if (numericUpDownMTU.Text == "")
                numericUpDownMTU.Text = MTU.ToString();
        }

        private void numericUpDownMTU_TextChanged(object sender, EventArgs e)
        {
            ValidateMtuValue();
        }

        private void numericUpDownMTU_ValueChanged(object sender, EventArgs e)
        {
            ValidateMtuValue();
        }

        private void checkBoxSriov_CheckedChanged(object sender, EventArgs e)
        {
            vlans = GetVLANList(SelectedHostNic);
            ValidateVLANValue();
        }

        #endregion
    }
}
