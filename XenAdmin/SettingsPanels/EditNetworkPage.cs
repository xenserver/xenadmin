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
using System.Drawing;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Commands;


namespace XenAdmin.SettingsPanels
{
    public partial class EditNetworkPage : UserControl, IEditPage
    {
        private XenAPI.Network network;
        private Host host;

        private bool _ValidToSave = true;
        private readonly ToolTip InvalidParamToolTip;
        private bool runningVMsWithoutTools = false;

        public EditNetworkPage()
        {
            InitializeComponent();

            Text = Messages.NETWORK_SETTINGS;

            InvalidParamToolTip = new ToolTip();
            InvalidParamToolTip.IsBalloon = true;
            InvalidParamToolTip.ToolTipIcon = ToolTipIcon.Warning;
            InvalidParamToolTip.ToolTipTitle = Messages.INVALID_PARAMETER;
        }

        public bool ValidToSave
        {
            get { return _ValidToSave; }
        }

        public Image Image
        {
            get
            {
                return Properties.Resources._000_Network_h32bit_16;
            }
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            network = clone as XenAPI.Network;
            if (network == null)
                return;

            // use the pif of the master to populate the controls. We use it later in the create_VLAN_from_PIF call in Network Action
            host = Helpers.GetMaster(network.Connection);
            
            Repopulate();
            EnableDisable();
        }

        // Can the network's NIC and VLAN be edited?
        private bool Editable(PIF pif)
        {
            return (pif == null || (!pif.IsPhysical && !pif.IsTunnelAccessPIF));
        }

        private void EnableDisable()
        {
            SetMTUControlEnablement();
            SetNetSettingsEnablement();
            panelDisruptionWarning.Visible = WillDisrupt();
            ShowHideLacpWarning();
            labelVLAN0Info.Visible = numUpDownVLAN.Enabled && numUpDownVLAN.Value == 0;
        }

        private bool VLANEnabled
        {
            set
            {
                numUpDownVLAN.Enabled = value;
            }
        }

        private bool NICSpinnerEnabled
        {
            set
            {
                HostPNICList.Enabled = value;
            }
        }

        private void SetNetSettingsEnablement()
        {
            // The non MTU controls block if any VMs are attached, presumably as their PIFs won't unplug
            bool blockDueToAttachedVMs = network.Connection.ResolveAll(network.VIFs).Exists(
                delegate(VIF vif)
                {
                    return vif.currently_attached;
                });

            bool blockDueToManagement = network.Connection.ResolveAll<PIF>(network.PIFs).Exists(
                delegate(PIF p)
                {
                    return p.IsManagementInterface(XenAdmin.Properties.Settings.Default.ShowHiddenVMs);
                });

            bool physical = network.Connection.ResolveAll(network.PIFs).Exists(
               delegate(PIF pif)
               {
                   return pif.physical;
               });

            bool bond = network.Connection.ResolveAll(network.PIFs).Exists(
               delegate(PIF pif)
               {
                   return pif.IsBondNIC;
               });

            bool tunnel = network.Connection.ResolveAll(network.PIFs).Exists(
               delegate(PIF pif)
               {
                   return pif.IsTunnelAccessPIF;
               });

            // If the original network restricts our settings we enable/disable controls here - this could actually only be run once if needed
            HostPNICList.Enabled = !blockDueToAttachedVMs && !blockDueToManagement;
            if (network.PIFs.Count == 0)
            {
                //internal - we can do what we want with these, even if they have VMs running on them
                VLANEnabled = true;
                NICSpinnerEnabled = true;
                warningText.Visible = false;
            }
            else if (!physical && !bond && !tunnel)
            {
                //external, not using physical pif           
                numUpDownVLAN.Enabled = !blockDueToAttachedVMs && !blockDueToManagement;
                warningText.Visible = blockDueToAttachedVMs || blockDueToManagement;
                warningText.Text =
                    blockDueToManagement ? string.Format(Messages.CANNOT_CONFIGURE_NET_DISTURB_MANAGEMENT, GetNetworksPIF().ManagementInterfaceNameOrUnknown) :
                    blockDueToAttachedVMs ? Messages.CANNOT_CONFIGURE_NET_VMS_ATTACHED :
                    "";
            }
            else
            {
                // physical or tunnel or bond: no warning needed as the controls are either all invisible or all allowed
                warningText.Visible = false;
            }

            //Now we additionally disable the VLAN spinner if we have virtual selected - this needs to be run every time we change the controls state
            if (SelectedIsInternal)
            {
                VLANEnabled = false;
            }

        }

        private void SetMTUControlEnablement()
        {
            if (!network.CanUseJumboFrames)
            {
                labelCannotConfigureMTU.Visible = false;
                labelMTU.Visible = numericUpDownMTU.Visible = false;
                return;
            }

            if (SelectedIsInternal)
            {
                // internal
                // MTU doesn't really do much here
                labelCannotConfigureMTU.Visible = false;
                numericUpDownMTU.Enabled = false;
                return;
            }

            PIF networksPIF = GetSelectedPIF();  // returns null for new VLAN

            if (networksPIF == null || !networksPIF.IsManagementInterface(XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
            {
                // non management external (could be bond)

                if (runningVMsWithoutTools)
                {
                    // The MTU controls have been designed to be more relaxed than the rest of the page, we will only block if we can't unplug the vifs
                    // due to lack of tools (which then lets us unplug the PIFs)
                    labelCannotConfigureMTU.Text = Messages.CANNOT_CONFIGURE_JUMBO_VM_NO_TOOLS;
                    labelCannotConfigureMTU.Visible = true;
                    numericUpDownMTU.Enabled = false;
                }
                else if (networksPIF != null && networksPIF.IsTunnelAccessPIF)
                {
                    // This branch is currently not in use as setting the MTU is disabled on CHINs.
                    // Left in in case future support is added

                    // with no other more danger warnings we should tell the user it's recommended that they set the MTU on the underlying networks to match
                    XenAPI.Network mainNetwork = FindCHINMainNetwork(networksPIF);
                    labelCannotConfigureMTU.Text = string.Format(Messages.SET_MTU_ON_CHINS_UNDER_NETWORK, mainNetwork.Name);
                    // incase some odd value has been set on the CLI
                    numericUpDownMTU.Maximum = Math.Max(network.MTU, XenAPI.Network.MTU_MAX);
                    numericUpDownMTU.Minimum = Math.Min(network.MTU, XenAPI.Network.MTU_MIN);
                    numericUpDownMTU.Enabled = true;
                    labelCannotConfigureMTU.Visible = true;
                }
                else
                {
                    labelCannotConfigureMTU.Visible = false;
                    // in case some odd value has been set on the CLI
                    numericUpDownMTU.Maximum = Math.Max(network.MTU, XenAPI.Network.MTU_MAX);
                    numericUpDownMTU.Minimum = Math.Min(network.MTU, XenAPI.Network.MTU_MIN);
                    numericUpDownMTU.Enabled = true;
                }
            }
            else
            {
                // physical or virtual external management (could be bond)
                numericUpDownMTU.Enabled = false;
                labelCannotConfigureMTU.Text = string.Format(Messages.CANNOT_CONFIGURE_JUMBO_DISTURB_MANAGEMENT, networksPIF.ManagementInterfaceNameOrUnknown);
                labelCannotConfigureMTU.Visible = true;
            }
        }

        private XenAPI.Network FindCHINMainNetwork(PIF networksPIF)
        {
            // Assumes that the pif is the access pif of only one CHIN. If there's more than one, you get the first.

            if (networksPIF.tunnel_access_PIF_of.Count < 1)
                return null;

            Tunnel t = networksPIF.Connection.Resolve<Tunnel>(networksPIF.tunnel_access_PIF_of[0]);
            PIF transportPIF = networksPIF.Connection.Resolve<PIF>(t.transport_PIF);
            if (transportPIF == null)
                return null; //unexepected

            return networksPIF.Connection.Resolve<XenAPI.Network>(transportPIF.network);
        }


        // Get the selected PIF.
        // Return null for single-host internal network, or a new VLAN.
        private PIF GetSelectedPIF()
        {
            // If we are on a physical network we don't look at the VLAN combo box, as it is obv not used
            PIF p = GetNetworksPIF();
            if (p != null && p.physical)
                return p;

            if (p != null && p.IsBondNIC)
                return p;

            // also no need to look in the combo box if we're a CHIN as they can't be edited either
            if (p != null && p.IsTunnelAccessPIF)
                return p;

            p = NICNameToVirtualPIF((string)HostPNICList.SelectedItem, (long)numUpDownVLAN.Value);

            // this is either now null (on an internal network or a new VLAN) or a non phys pif representing a vlan (external network)
            return p;
        }

        private bool SelectedIsInternal
        {
            get { return (string)HostPNICList.SelectedItem == Messages.NETWORKPANEL_INTERNAL; }
        }

        public void Repopulate()
        {
            if (network == null || host == null)
                return;

            populateHostNicList();

            //set minimum value for VLAN
            numUpDownVLAN.Minimum = Helpers.VLAN0Allowed(network.Connection) ? 0 : 1;

            PIF pif = GetNetworksPIF();

            if (pif != null)
            {
                bool editable = Editable(pif);

                HostVLanLabel.Visible = editable;
                HostNicLabel.Visible = editable;
                numUpDownVLAN.Visible = editable;
                HostPNICList.Visible = editable;
                nicHelpLabel.Visible = editable;

                if (editable)
                {
                    // virtual pif (external network on VLAN)
                    numUpDownVLAN.Value = pif.VLAN;
                    PIF ThePhysicalPIF = FindAssociatedPhysicalPIF();
                    if (ThePhysicalPIF != null)
                        HostPNICList.SelectedItem = ThePhysicalPIF.Name;
                    else
                        HostPNICList.SelectedItem = pif.Name;
                }

                bool hasBondMode = network.IsBond;
                groupBoxBondMode.Visible = hasBondMode;

                bool supportsLinkAggregation = Helpers.SupportsLinkAggregationBond(network.Connection);
                radioButtonLacpSrcMac.Visible = radioButtonLacpTcpudpPorts.Visible = supportsLinkAggregation;

                if (hasBondMode)
                {
                    switch (NetworkBondMode)
                    {
                        case bond_mode.balance_slb:
                            radioButtonBalanceSlb.Checked = true;
                            break;
                        case bond_mode.active_backup:
                            radioButtonActiveBackup.Checked = true;
                            break;
                        case bond_mode.lacp:
                            if (supportsLinkAggregation)
                            {
                                switch (HashingAlgorithm)
                                {
                                    case Bond.hashing_algoritm.tcpudp_ports:
                                        radioButtonLacpTcpudpPorts.Checked = true;
                                        break;
                                    default:
                                        radioButtonLacpSrcMac.Checked = true;
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                // internal network
                HostVLanLabel.Visible = true;
                HostNicLabel.Visible = true;
                numUpDownVLAN.Visible = true;
                HostVLanLabel.Visible = true;
                HostPNICList.Visible = true;
                nicHelpLabel.Visible = true;

                groupBoxBondMode.Visible = false;
                numUpDownVLAN.Enabled = false;
                HostPNICList.SelectedItem = HostPNICList.Items[0];
            }

            foreach (VIF v in network.Connection.ResolveAll<VIF>(network.VIFs))
            {
                VM vm = network.Connection.Resolve<VM>(v.VM);
                if (vm.power_state != vm_power_state.Running || vm.GetVirtualisationStatus.HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED))
                    continue;

                runningVMsWithoutTools = true;
                break;
            }

            // Populate Automatic checkbox
            autoCheckBox.Checked = network.AutoPlug;
            autoCheckBox.Enabled = !network.IsGuestInstallerNetwork;
            // in case some odd value has been set on the CLI
            numericUpDownMTU.Maximum = Math.Max(network.MTU, XenAPI.Network.MTU_MAX);
            numericUpDownMTU.Minimum = Math.Min(network.MTU, XenAPI.Network.MTU_MIN);
            numericUpDownMTU.Value = network.MTU;
            numericUpDownMTU.Visible = network.CanUseJumboFrames;
        }

        private PIF GetNetworksPIF()
        {
            foreach (PIF pif in network.Connection.ResolveAll(network.PIFs))
                if (host.opaque_ref == pif.host.opaque_ref)
                    return pif;

            return null;
        }

        private PIF FindAssociatedPhysicalPIF()
        {
            // Gets a pif from the network object
            PIF networkPif = GetNetworksPIF();

            // virtual network, no associated pif
            if (networkPif == null)
                return null;

            // so actually our network pif is a physical one, return that
            if (networkPif.physical)
                return networkPif;

            // try to find the physical counterpart to our virtual pif
            foreach (PIF pif in network.Connection.Cache.PIFs)
                if (pif.IsPhysical &&
                    pif.host.opaque_ref == host.opaque_ref &&
                    pif.device == networkPif.device &&
                    pif.opaque_ref != networkPif.opaque_ref)
                    return pif;

            return null;
        }

        private void populateHostNicList()
        {
            HostPNICList.BeginUpdate();

            try
            {
                HostPNICList.Items.Clear();

                HostPNICList.Items.Add(Messages.NETWORKPANEL_INTERNAL);

                foreach (PIF pif in network.Connection.Cache.PIFs)
                {
                    if (!Properties.Settings.Default.ShowHiddenVMs &&
                        !pif.Show(Properties.Settings.Default.ShowHiddenVMs))
                        continue;

                    if (!pif.IsPhysical || pif.IsBondSlave)
                        continue;

                    if (pif.host.opaque_ref != host.opaque_ref)
                        continue;
                    
                    HostPNICList.Items.Add(pif.Name);
                }
            }
            finally
            {
                HostPNICList.EndUpdate();
            }
        }

        private void HostPNICList_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private bool MtuHasChanged
        {
            get { return numericUpDownMTU.Enabled && numericUpDownMTU.Value != network.MTU; }
        }

        private bond_mode NewBondMode
        {
            get
            {
                return radioButtonBalanceSlb.Checked 
                    ? bond_mode.balance_slb 
                    : radioButtonActiveBackup.Checked ? bond_mode.active_backup : bond_mode.lacp;
            }
        }

        private bool BondModeHasChanged
        {
            get { return (radioButtonBalanceSlb.Visible && radioButtonBalanceSlb.Enabled && NetworkBondMode != NewBondMode); }
        }

        private Bond.hashing_algoritm NewHashingAlgorithm
        {
            get
            {
                return radioButtonLacpSrcMac.Checked 
                    ? Bond.hashing_algoritm.src_mac 
                    : radioButtonLacpTcpudpPorts.Checked ? Bond.hashing_algoritm.tcpudp_ports : Bond.hashing_algoritm.unknown;
            }
        }

        private bool HashingAlgorithmHasChanged
        {
            get
            {
                var newValue = NewHashingAlgorithm;
                // has the hashing algorithm (load balancing method) changed to a valid value (i.e. not hashing_algoritm.unknown) ?
                return radioButtonLacpSrcMac.Visible && radioButtonLacpSrcMac.Enabled && HashingAlgorithm != newValue &&
                       newValue != Bond.hashing_algoritm.unknown;
            }
        }

        public bool HasChanged
        {
            get
            {
                if (autoCheckBox.Checked != network.AutoPlug || MtuHasChanged || BondModeHasChanged || HashingAlgorithmHasChanged)
                    return true;

                PIF pif = GetNetworksPIF();

                if (pif != null)
                {
                    if (!Editable(pif))
                        return false;

                    if (pif.Name != (String)HostPNICList.SelectedItem)
                        return true;

                    if (pif.VLAN != (long)numUpDownVLAN.Value)
                        return true;
                }
                else if (HostPNICList.SelectedIndex != 0)
                    return true;

                return false;
            }
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
            InvalidParamToolTip.Dispose();
        }

        private bool WillDisrupt()
        {
            // This should follow the same logic as the HasChanged and SaveSettings() methods, and return true if we will do
            // a pif unplug/plug or a networkaction

            return MtuHasChanged || BondModeHasChanged || InternalToExternal || ExternalToInternal || ExternalChangedDeviceOrVlan || HashingAlgorithmHasChanged;
        }

        // Gone from a private network to an external one
        private bool InternalToExternal
        {
            get { return network.PIFs.Count == 0 && !SelectedIsInternal; }
        }

        // Gone from an external network to an internal one
        private bool ExternalToInternal
        {
            get { return network.PIFs.Count > 0 && SelectedIsInternal; }
        }

        // Gone from external to external on another device or another VLAN
        private bool ExternalChangedDeviceOrVlan
        {
            get 
            {
                if (network.PIFs.Count == 0)
                    return false;

                PIF pif = GetNetworksPIF();
                PIF selectedPif = NICNameToVirtualPIF((string)HostPNICList.SelectedItem, (long)numUpDownVLAN.Value);
                return pif != null && Editable(pif) && pif != selectedPif;
            }
        }

        public AsyncAction SaveSettings()
        {
            List<AsyncAction> actions = new List<AsyncAction>();

            network.AutoPlug = autoCheckBox.Checked;
            bool needPlugUnplug = MtuHasChanged;

            if (MtuHasChanged)
                network.MTU = (long)numericUpDownMTU.Value;
            if (BondModeHasChanged)
            {
                List<AsyncAction> bondActions = SetBondModeActions();
                if (bondActions != null && bondActions.Count > 0)
                    actions.AddRange(bondActions);
            }

            if (HashingAlgorithmHasChanged)
            {
                List<AsyncAction> bondPropertiesActions = SetBondPropertiesActions();
                if (bondPropertiesActions != null && bondPropertiesActions.Count > 0)
                    actions.AddRange(bondPropertiesActions);
            }

            // Have the pifs changed? Just key off the first PIF
            bool pifsChanged = false;
            bool external = false;
            PIF new_pif = null;
            long vlan = -1;

            if (InternalToExternal)
            {
                pifsChanged = true;
                external = true;
                new_pif = NICNameToPIF((string)HostPNICList.SelectedItem);
                vlan = (long)this.numUpDownVLAN.Value;
            }
            else if (ExternalToInternal)
            {
                pifsChanged = true;
                external = false;
            }
            else if (ExternalChangedDeviceOrVlan)
            {
                pifsChanged = true;
                external = true;
                new_pif = NICNameToPIF((string)HostPNICList.SelectedItem);
                vlan = (long)this.numUpDownVLAN.Value;
            }

            if (pifsChanged || external)
            {
                // even if we needPlugUnplug this is ok, the network update action destroys/recreates pifs anyway
                // ASSUMPTION: currently we don't allow network reconfigure that leads us here if ANY VMs are attached, so there are no VIFs to plug/unplug
                actions.Add(new NetworkAction(network.Connection, network, pifsChanged, external, new_pif, vlan, true));
            }
            else if (needPlugUnplug)
            {
                List<PIF> pifs = network.Connection.ResolveAll<PIF>(network.PIFs);
                AsyncAction a = new UnplugPlugNetworkAction(network, true);
                foreach (SelectedItem i in Program.MainWindow.SelectionManager.Selection)
                {
                    Host h = i.XenObject as Host;
                    if (h == null)
                        continue;

                    a.Host = h;
                    break;
                }
                actions.Add(a);
            }

            if (actions.Count == 0)
                return null;

            return
                new MultipleAction(network.Connection, Messages.ACTION_SAVE_CHANGES_TITLE,
                Messages.ACTION_SAVE_CHANGES_IN_PROGRESS, Messages.ACTION_SAVE_CHANGES_SUCCESSFUL,
                actions, true);
        }

        private PIF NICNameToPIF(string p)
        {
            foreach (PIF pif in network.Connection.Cache.PIFs)
            {
                if (pif.Name == p && pif.IsPhysical && pif.host.opaque_ref == host.opaque_ref)
                    return pif;
            }
            return null;
        }

        private PIF NICNameToVirtualPIF(string p, long vlan)
        {
            foreach (PIF pif in network.Connection.Cache.PIFs)
            {
                if (pif.Name == p && !pif.IsPhysical && !pif.IsTunnelAccessPIF && pif.VLAN == vlan && pif.host.opaque_ref == host.opaque_ref)
                    return pif;
            }
            return null;
        }

        /// <summary>
        /// The mode of the bond associated with this network. Assumes HasBondMode has already been tested,
        /// and that the Bond objects on each host have the same mode.
        /// </summary>
        private bond_mode NetworkBondMode
        {
            get
            {
                List<Bond> bonds = network.Connection.ResolveAll(network.TheBonds);
                return ((bonds == null || bonds.Count == 0) ? bond_mode.unknown : bonds[0].mode);
            }
        }

        /// <summary>
        /// The hashing algorithm of the bond associated with this network. Assumes HasBondMode and SupportsLinkAggregation has already been tested,
        /// and that the Bond objects on each host have the same hashing algorithm.
        /// </summary>
        private Bond.hashing_algoritm HashingAlgorithm
        {
            get
            {
                List<Bond> bonds = network.Connection.ResolveAll(network.TheBonds);
                return ((bonds == null || bonds.Count == 0) ? Bond.hashing_algoritm.unknown : bonds[0].HashingAlgoritm);
            }
        }

        private List<AsyncAction> SetBondModeActions()
        {
            var ans = new List<AsyncAction>();
            foreach (var bond in network.Connection.ResolveAll(network.TheBonds))
            {
                Bond b = bond;  // have to copy it otherwise it will change to the last bond before the delegates are called
                ans.Add(new DelegatedAsyncAction(bond.Connection,
                    Messages.SET_BOND_MODE_ACTION_TITLE,
                    Messages.SET_BOND_MODE_ACTION_START,
                    Messages.SET_BOND_MODE_ACTION_END,
                    session => Bond.set_mode(session, b.opaque_ref, NewBondMode),
                    true,
                    "bond.set_mode"));
            }
            return (ans.Count == 0 ? null : ans);
        }

        private List<AsyncAction> SetBondPropertiesActions()
        {
            var ans = new List<AsyncAction>();
            foreach (var bond in network.Connection.ResolveAll(network.TheBonds))
            {
                Bond b = bond;  // have to copy it otherwise it will change to the last bond before the delegates are called
                ans.Add(new DelegatedAsyncAction(bond.Connection,
                    Messages.SET_BOND_HASHING_ALGORITHM_ACTION_TITLE,
                    Messages.SET_BOND_HASHING_ALGORITHM_ACTION_START,
                    Messages.SET_BOND_HASHING_ALGORITHM_ACTION_END,
                    session => Bond.set_property(session, b.opaque_ref, "hashing_algorithm", Bond.HashingAlgoritmToString(NewHashingAlgorithm)),
                    true,
                    "bond.set_property"));
            }
            return (ans.Count == 0 ? null : ans);
        }

        public String SubText
        {
            get
            {
                if (network == null)
                    return "";

                PIF pif = GetNetworksPIF();
                if (pif != null && pif.IsPhysical)
                    return Messages.PHYSICAL_DEVICE;

                if (pif != null && pif.IsTunnelAccessPIF)
                    return Messages.CHIN;

                if (HostPNICList.SelectedIndex == 0)
                    return Messages.NETWORKPANEL_INTERNAL;

                return String.Format(Messages.NIC_VLAN, HostPNICList.SelectedItem, numUpDownVLAN.Value);
            }
        }

        private void numUpDownVLAN_ValueChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void numericUpDownMTU_ValueChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void BondMode_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisable();
        }

        private void ShowHideLacpWarning()
        {
            panelLACPWarning.Visible = radioButtonLacpSrcMac.Checked || radioButtonLacpTcpudpPorts.Checked;
        }
    }
}
