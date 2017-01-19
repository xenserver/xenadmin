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
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    public partial class NetWDetails : XenTabPage
    {
        List<int> vlans;

        public NetWDetails()
        {
            InitializeComponent();
            numericUpDownVLAN.LostFocus += checkVLAN;
            numericUpDownVLAN.TextChanged += numericUpDownVLAN_TextChanged;
            numericUpDownMTU.Maximum = XenAPI.Network.MTU_MAX;
            numericUpDownMTU.Minimum = XenAPI.Network.MTU_MIN;
            numericUpDownMTU.Value = XenAPI.Network.MTU_DEFAULT;
        }

        public override string Text { get { return Messages.NETW_DETAILS_TEXT; } }

        public override string PageTitle { get
        {
            return SelectedNetworkType == NetworkTypes.External
                       ? Messages.NETW_EXTERNAL_DETAILS_TITLE
                       : Messages.NETW_INTERNAL_DETAILS_TITLE;
        } }

        public override bool EnableNext()
        {
            return (SelectedHostNic != null || !comboBoxNICList.Visible) && !labelVlanError.Visible;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            HelpersGUI.FocusFirstControl(Controls);
        }

        public override void PopulatePage()
        {
            PopulateHostNicList(Host, Connection);
            UpdateEnablement(SelectedNetworkType == NetworkTypes.External, Host);
            //set minimum value for VLAN
            numericUpDownVLAN.Minimum = Helpers.VLAN0Allowed(Connection) ? 0 : 1; 
        }

        private int CurrentVLANValue
        {
            get { return Convert.ToInt32(Math.Round(numericUpDownVLAN.Value, MidpointRounding.AwayFromZero)); }
        }

        private void checkVLAN(object sender, EventArgs e)
        {
            if (numericUpDownVLAN.Text == "")
            {
                numericUpDownVLAN.Text = CurrentVLANValue.ToString();
            }
        }

        #region Accessors

        public NetworkTypes SelectedNetworkType { private get; set; }

        public Host Host { private get; set; }

        public PIF SelectedHostNic
        {
            get { return (PIF)comboBoxNICList.SelectedItem; }
        }

        public long VLAN
        {
            get { return CurrentVLANValue; }
        }

        public bool isAutomaticAddNicToVM
        {
            get { return checkBoxAutomatic.Checked; }
        }

        /// <summary>
        /// Null if the custom MTU option is disabled
        /// </summary>
        public long? MTU
        {
            get
            {
                if (numericUpDownMTU.Enabled)
                    return (long)numericUpDownMTU.Value;
                else
                    return null;
            }
        }

        #endregion

        private void UpdateEnablement(bool external, Host host)
        {
            lblNicHelp.Text = external ? Messages.WIZARD_DESC_NETWORK_SETTINGS_EXTERNAL : Messages.WIZARD_DESC_NETWORK_SETTINGS_INTERNAL;
            comboBoxNICList.Visible = external;
            labelVLAN.Visible = external;
            numericUpDownVLAN.Visible = external;
            numericUpDownMTU.Visible = labelMTU.Visible = infoMtuPanel.Visible = external;
            labelNIC.Visible = external;
            if (comboBoxNICList.Items.Count > 0)
                comboBoxNICList.SelectedIndex = external ? comboBoxNICList.Items.Count - 1 : -1;

            OnPageUpdated();
        }

        private void PopulateHostNicList(Host host, IXenConnection conn)
        {
            comboBoxNICList.Items.Clear();

            foreach (PIF ThePIF in conn.Cache.PIFs)
            {
                if (ThePIF.host.opaque_ref == host.opaque_ref && ThePIF.IsPhysical && (Properties.Settings.Default.ShowHiddenVMs || ThePIF.Show(Properties.Settings.Default.ShowHiddenVMs)) && !ThePIF.IsBondSlave)
                {
                    comboBoxNICList.Items.Add(ThePIF);
                }
            }
            if (comboBoxNICList.Items.Count > 0)
                comboBoxNICList.SelectedIndex = 0;

            cmbHostNicList_SelectedIndexChanged(null, null);
        }

        private List<int> GetVLANList(PIF nic)
        {
            List<int> vlans = new List<int>();

            foreach (PIF pif in nic.Connection.Cache.PIFs)
            {
                if (pif.device == nic.device)
                {
                    vlans.Add((int)pif.VLAN);
                }
            }

            return vlans;
        }

        private int GetFirstAvailableVLAN(List<int> vlans)
        {
            //CA-19111: VLAN values should only go up to the numericUpDownVLAN.Maximum (4094)
            for (int i = 1; i <= numericUpDownVLAN.Maximum; i++)
            {
                if (!vlans.Contains(i))
                    return i;
            }

            return -1;
        }

        private void cmbHostNicList_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnPageUpdated();

            if (SelectedHostNic == null)
                return;
            
            numericUpDownMTU.Maximum = Math.Min(SelectedHostNic.MTU, XenAPI.Network.MTU_MAX);

            numericUpDownMTU.Enabled = numericUpDownMTU.Minimum != numericUpDownMTU.Maximum;
            
            infoMtuMessage.Text = numericUpDownMTU.Minimum == numericUpDownMTU.Maximum
                                    ? string.Format(Messages.ALLOWED_MTU_VALUE, numericUpDownMTU.Minimum)
                                    : string.Format(Messages.ALLOWED_MTU_RANGE, numericUpDownMTU.Minimum, numericUpDownMTU.Maximum);

            vlans = GetVLANList(SelectedHostNic);

            //CA-72484: check whether the currently selected VLAN is available and keep it
            int curVlan = CurrentVLANValue;
            if (!vlans.Contains(curVlan))
            {
                SetError(null);
                return;
            }

            int avail_vlan = GetFirstAvailableVLAN(vlans);

            if (avail_vlan == -1)
                return;

            numericUpDownVLAN.Value = avail_vlan;
        }

        private void nudVLAN_ValueChanged(object sender, EventArgs e)
        {
            ValidateVLANValue();
        }

        void numericUpDownVLAN_TextChanged(object sender, EventArgs e)
        {
            ValidateVLANValue();
        }

        private void SetError(string error)
        {
            bool visible = !string.IsNullOrEmpty(error);
            bool updatePage = labelVlanError.Visible != visible;
            labelVlanError.Visible = visible;
            if (visible)
                labelVlanError.Text = error;
            labelVLAN0Info.Visible = !visible && numericUpDownVLAN.Value == 0;
            if (updatePage)
                OnPageUpdated();
        }

        private bool VLANValidNumber()
        {
            int result;
            return int.TryParse(numericUpDownVLAN.Text.Trim(), out result);
        }

        private bool VLANNumberUnique()
        {
            if (vlans == null)
                return true;
            return !vlans.Contains(CurrentVLANValue);
        }

        private void ValidateVLANValue()
        {
            if (!VLANValidNumber())
            {
                SetError(Messages.INVALID_NUMBER);
                return;
            }
            if (!VLANNumberUnique())
            {
                SetError(Messages.NETW_DETAILS_VLAN_NUMBER_IN_USE);
                return;
            }
            SetError(null);
        }
    }
}
