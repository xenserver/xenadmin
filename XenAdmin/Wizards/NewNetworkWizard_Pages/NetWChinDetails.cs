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
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    public partial class NetWChinDetails : XenTabPage
    {
        public NetWChinDetails()
        {
            InitializeComponent();
            // Currently setting MTU on CHINS is not supported. Remove this line to re-enable.
            numericUpDownMTU.Maximum = XenAPI.Network.MTU_MAX;
            numericUpDownMTU.Minimum = XenAPI.Network.MTU_MIN;
            numericUpDownMTU.Visible = labelMTU.Visible = tableLayoutPanelMTUWarning.Visible = false;
        }

        public override string Text => Messages.NETW_DETAILS_TEXT;

        public override string PageTitle => Messages.NETW_CHIN_DETAILS_TITLE;

        public override bool EnableNext()
        {
            return SelectedInterface != null;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            comboInterfaces.Focus();
        }

        public override void PopulatePage()
        {
            PopulateInterfaces(Pool, Host, Connection);
        }

        public Host Host { private get; set; }
        public Pool Pool { private get; set; }

        public XenAPI.Network SelectedInterface => (XenAPI.Network)comboInterfaces.SelectedItem;

        public bool isAutomaticAddNicToVM => cbxAutomatic.Checked;

        public long MTU => (long)numericUpDownMTU.Value;

        private void PopulateInterfaces(Pool pool, Host host, IXenConnection connection)
        {
            Dictionary<XenAPI.Network, bool> networksAdded = new Dictionary<XenAPI.Network, bool>();

            comboInterfaces.Items.Clear();

            foreach (PIF pif in connection.Cache.PIFs)
            {
                // If the current selection is for a host, rather than a pool, only show
                // management interfaces with PIFs on that host.
                if (pool == null && host != null && pif.host.opaque_ref != host.opaque_ref)
                    continue;

                if (pif.IsManagementInterface(XenAdmin.Properties.Settings.Default.ShowHiddenVMs))
                {
                    XenAPI.Network network = connection.Resolve(pif.network);
                    if (network != null &&  // this should have been checked already by pif.IsManagementInterface, but...
                        !networksAdded.ContainsKey(network))
                    {
                        comboInterfaces.Items.Add(network);
                        networksAdded[network] = true;
                    }
                }
            }
            comboInterfaces.SelectedIndexChanged += comboInterfaces_SelectedIndexChanged;

            if (comboInterfaces.Items.Count > 0)
                comboInterfaces.SelectedIndex = 0;
        }

        private void comboInterfaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            // just in case people have been fiddling around on the CLI
            numericUpDownMTU.Maximum = Math.Max(SelectedInterface.MTU, XenAPI.Network.MTU_MAX);
            numericUpDownMTU.Minimum = Math.Min(SelectedInterface.MTU, XenAPI.Network.MTU_MIN);
            numericUpDownMTU.Value = SelectedInterface.MTU;
            OnPageUpdated();
        }

        private void numericUpDownMTU_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedInterface != null)
                tableLayoutPanelMTUWarning.Visible = numericUpDownMTU.Value != SelectedInterface.MTU;
        }
    }
}
