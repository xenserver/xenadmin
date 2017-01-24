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
using System.Text;
using System.Windows.Forms;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using System.Collections;
using System.Linq;


namespace XenAdmin.Dialogs
{
    public partial class VIFDialog : XenDialogBase
    {
        private VIF ExistingVif;
        private int Device;
        private readonly bool vSwitchController;

        public VIFDialog(IXenConnection Connection, VIF ExistingVif, int Device)
            : base(Connection)
        {
            InitializeComponent();
            CueBannersManager.SetWatermark(promptTextBoxMac, "aa:bb:cc:dd:ee:ff");

            this.ExistingVif = ExistingVif;
            this.Device = Device;
            if (ExistingVif != null)
                changeToPropertiesTitle();

            // Check if vSwitch Controller is configured for the pool (CA-46299)
            Pool pool = Helpers.GetPoolOfOne(connection);
            vSwitchController = pool != null && pool.vSwitchController;

            label1.Text = vSwitchController ? Messages.VIF_VSWITCH_CONTROLLER : Messages.VIF_LICENSE_RESTRICTION; 
            LoadNetworks();
            LoadDetails();
            updateEnablement();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            promptTextBoxMac.GotFocus += new EventHandler(promptTextBoxMac_ReceivedFocus);
            promptTextBoxQoS.GotFocus += new EventHandler(promptTextBoxQoS_ReceivedFocus);
            promptTextBoxQoS.TextChanged += new EventHandler(promptTextBoxQoS_TextChanged);
            comboBoxNetwork.SelectedIndexChanged += new EventHandler(NetworkComboBox_SelectedIndexChanged);
            promptTextBoxMac.TextChanged += new EventHandler(promptTextBoxMac_TextChanged);
        }

        void promptTextBoxQoS_TextChanged(object sender, EventArgs e)
        {
            updateEnablement();
        }

        void promptTextBoxQoS_ReceivedFocus(object sender, EventArgs e)
        {
            checkboxQoS.Checked = true;
            updateEnablement();
        }

        void promptTextBoxMac_ReceivedFocus(object sender, EventArgs e)
        {
            radioButtonMac.Checked = true;
            updateEnablement();
        }

        private void changeToPropertiesTitle()
        {
            this.Text = Messages.VIRTUAL_INTERFACE_PROPERTIES;
            this.buttonOk.Text = Messages.OK;
        }

        private void LoadDetails()
        {
            if (vSwitchController) 
            {
                flowLayoutPanelQoS.Enabled = checkboxQoS.Enabled = checkboxQoS.Checked = false;
                panelLicenseRestriction.Visible = true;
            }
            else
            {
                if (ExistingVif == null)
                {
                    promptTextBoxQoS.Text = "";
                    checkboxQoS.Checked = false;
                }
                else
                {
                    promptTextBoxQoS.Text = ExistingVif.LimitString;
                    checkboxQoS.Checked = ExistingVif.RateLimited;
                }
                flowLayoutPanelQoS.Enabled = checkboxQoS.Enabled = true;

                panelLicenseRestriction.Visible = false;
            }

            if (ExistingVif == null)
            {
                radioButtonAutogenerate.Checked = true;
                return;
            }
            foreach (NetworkComboBoxItem item in comboBoxNetwork.Items)
            {
                if (item.Network != null && item.Network.opaque_ref == ExistingVif.network.opaque_ref)
                    comboBoxNetwork.SelectedItem = item;
            }
            promptTextBoxMac.Text = ExistingVif.MAC;
            if (!string.IsNullOrEmpty(ExistingVif.MAC))
                radioButtonMac.Checked = true;
            else
                radioButtonAutogenerate.Checked = true;
        }

        void promptTextBoxMac_TextChanged(object sender, EventArgs e)
        {
            ValidateMACAddress();
        }

        private void ValidateMACAddress()
        {
            if (MACAddressHasChanged() && !MACAddressIsAcceptable())
            {
                MarkMACAsInvalid();
                if (ExistingVif != null)
                    promptTextBoxMac.Text = ExistingVif.MAC;
                return;
            }

            updateEnablement();
        }

        private bool MACAddressHasChanged()
        {
            if (ExistingVif == null) 
                return true;
            return promptTextBoxMac.Text != ExistingVif.MAC;
        }

        private void MarkMACAsInvalid()
        {
            buttonOk.Enabled = false;
            toolTipContainerOkButton.SetToolTip(Messages.MAC_INVALID);
        }

        private void updateEnablement()
        {
            if (!Helpers.IsValidMAC(promptTextBoxMac.Text) && !AutogenerateMac)
            {
                MarkMACAsInvalid();
            }
            else if (comboBoxNetwork.SelectedItem == null || ((NetworkComboBoxItem)comboBoxNetwork.SelectedItem).Network == null)
            {
                buttonOk.Enabled = false;
                toolTipContainerOkButton.SetToolTip(Messages.SELECT_NETWORK_TOOLTIP);
            }
            else if (checkboxQoS.Checked && !isValidQoSLimit())
            {
                buttonOk.Enabled = false;
                toolTipContainerOkButton.SetToolTip(Messages.ENTER_VALID_QOS);
            }
            else
            {
                buttonOk.Enabled = true;
                toolTipContainerOkButton.RemoveAll();
            }
        }

        private void LoadNetworks()
        {
            List<XenAPI.Network> networks = new List<XenAPI.Network>(connection.Cache.Networks);
            networks.Sort();
            foreach (XenAPI.Network network in networks)
            {
                if (!network.Show(Properties.Settings.Default.ShowHiddenVMs) || network.IsSlave)
                    continue;

                comboBoxNetwork.Items.Add(new NetworkComboBoxItem(network));
            }

            if (comboBoxNetwork.Items.Count == 0)
            {
                comboBoxNetwork.Items.Add(new NetworkComboBoxItem(null));
            }
            comboBoxNetwork.SelectedIndex = 0;
        }

        private void NetworkComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateMACAddress();
            updateEnablement();
        }

        private XenAPI.Network SelectedNetwork
        {
            get
            {
                return ((NetworkComboBoxItem)comboBoxNetwork.SelectedItem).Network;
            }
        }

        private string SelectedMac
        {
            get
            {
                return AutogenerateMac ? "" : promptTextBoxMac.Text;
            }
        }

        private bool AutogenerateMac
        {
            get
            {
                return radioButtonAutogenerate.Checked;
            }
        }

        public VIF NewVif()
        {
            VIF vif = new VIF();
            vif.Connection = connection;
            vif.network = new XenRef<XenAPI.Network>(SelectedNetwork.opaque_ref);
            vif.MAC = SelectedMac;
            vif.device = Device.ToString();

            if (checkboxQoS.Checked)
                vif.RateLimited = true;

            // preserve this param even if we have decided not to turn on qos
            if (!string.IsNullOrEmpty(promptTextBoxQoS.Text))
            {
                Dictionary<String, String> qos_algorithm_params = new Dictionary<String, String>();
                qos_algorithm_params.Add(VIF.KBPS_QOS_PARAMS_KEY, promptTextBoxQoS.Text);
                vif.qos_algorithm_params = qos_algorithm_params;
            }
            return vif;
        }

        /// <summary>
        /// Retrieves the new settings as a proxy vif object. You will need to set the VM field to use these settings in a vif action
        /// </summary>
        /// <returns></returns>
        public Proxy_VIF GetNewSettings()
        {
            Proxy_VIF proxyVIF = ExistingVif != null ? ExistingVif.ToProxy() : new Proxy_VIF();
            proxyVIF.network = new XenRef<XenAPI.Network>(SelectedNetwork.opaque_ref);
            proxyVIF.MAC = SelectedMac;
            proxyVIF.device = Device.ToString();

            if (checkboxQoS.Checked)
            {
                proxyVIF.qos_algorithm_type = VIF.RATE_LIMIT_QOS_VALUE;
            }
            else if (ExistingVif != null && ExistingVif.RateLimited)
            {
                proxyVIF.qos_algorithm_type = "";
            }
            // else ... we leave it alone. Currently we only deal with "ratelimit" and "", don't overwrite the field if it's something else

            // preserve this param even if we turn off qos
            if (!string.IsNullOrEmpty(promptTextBoxQoS.Text))
            {
                Hashtable qos_algorithm_params = new Hashtable();
                qos_algorithm_params.Add(VIF.KBPS_QOS_PARAMS_KEY, promptTextBoxQoS.Text);
                proxyVIF.qos_algorithm_params = qos_algorithm_params;
            }

            return proxyVIF;
        }

        private bool ChangesHaveBeenMade
        {
            get
            {
                if (ExistingVif == null)
                    return true;

                if (ExistingVif.network.opaque_ref != SelectedNetwork.opaque_ref)
                    return true;

                if (ExistingVif.MAC != SelectedMac)
                    return true;

                if (ExistingVif.device != Device.ToString())
                    return true;

                if (ExistingVif.RateLimited)
                {
                    if (!checkboxQoS.Checked)
                        return true;
                    if (ExistingVif.qos_algorithm_params[VIF.KBPS_QOS_PARAMS_KEY] != promptTextBoxQoS.Text)
                        return true;
                }
                else if (string.IsNullOrEmpty(ExistingVif.qos_algorithm_type))
                {
                    if (checkboxQoS.Checked)
                        return true;
                }

                return false;
            }
        }

        private void AutogenerateRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            updateEnablement();
        }

        private void MacRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            updateEnablement();
        }

        private ThreeButtonDialog MacAddressDuplicationWarningDialog(string macAddress, string vmName)
        {
            return new ThreeButtonDialog(
                                  new ThreeButtonDialog.Details(SystemIcons.Warning,
                                      String.Format(Messages.PROBLEM_MAC_ADDRESS_IS_DUPLICATE, macAddress, vmName).Replace("\\n", "\n"),
                                      Messages.PROBLEM_MAC_ADDRESS_IS_DUPLICATE_TITLE),
                                      new[]
                                          {
                                            new ThreeButtonDialog.TBDButton( Messages.YES_BUTTON_CAPTION, DialogResult.Yes),
                                            new ThreeButtonDialog.TBDButton( Messages.NO_BUTTON_CAPTION, DialogResult.No, ThreeButtonDialog.ButtonType.CANCEL, true)
                                          }
                                      );
        }

        /// <summary>
        /// Determine if the MAC is accetable. It may not be if 
        /// other VIFs have the same MAC address as entered
        /// </summary>
        /// <returns>If the MAC address entered is acceptable</returns>
        private bool MACAddressIsAcceptable()
        {
            foreach (var xenConnection in ConnectionsManager.XenConnectionsCopy.Where(c => c.IsConnected))
            {
                foreach (VIF vif in xenConnection.Cache.VIFs)
                {
                    var vm = xenConnection.Resolve(vif.VM);
                    if (vif != ExistingVif && vif.MAC == SelectedMac && vm != null && vm.is_a_real_vm)
                    {
                        DialogResult result;
                        using (var dlg = MacAddressDuplicationWarningDialog(
                            SelectedMac,
                            vm.NameWithLocation))
                        {
                            result = dlg.ShowDialog(Program.MainWindow);
                        }
                        return (result == DialogResult.Yes);
                    }
                }
            }
            return true;
        }

        private void Okbutton_Click(object sender, EventArgs e)
        {
            DialogResult = !ChangesHaveBeenMade ? DialogResult.Cancel : DialogResult.OK;
            Close();
        }

        private void Cancelbutton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool isValidQoSLimit()
        {
            if (!checkboxQoS.Checked)
            {
                return true;
            }

            string value = promptTextBoxQoS.Text;

            if (value == null || value.Trim().Length == 0)
                return false;

            Int32 result;
            if (Int32.TryParse(value, out result))
            {
                return result > 0;
            }
            else
            {
                return false;
            }
        }

        private void checkboxQoS_CheckedChanged(object sender, EventArgs e)
        {
            updateEnablement();
        }

        internal override string HelpName
        {
            get
            {
                if (ExistingVif != null)
                    return "EditVmNetworkSettingsDialog";
                else
                    return "VIFDialog";
            }
        }
    }

    public class NetworkComboBoxItem : IEquatable<NetworkComboBoxItem>
    {
        public XenAPI.Network Network;

        public NetworkComboBoxItem(XenAPI.Network network)
        {
            Network = network;
        }

        public override string ToString()
        {
            return Network == null ? Messages.NONE : Helpers.GetName(Network);
        }

        public bool Equals(NetworkComboBoxItem other)
        {
            if (Network == null)
                return other.Network == null;
            return Network.Equals(other.Network);
        }
    }
}