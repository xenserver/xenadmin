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
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Network;

namespace XenAdmin.Wizards.NewNetworkWizard_Pages
{
    public partial class NetWSriovDetails : XenTabPage
    {
        internal Host Host;

        public NetWSriovDetails()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string Text => Messages.NETW_DETAILS_TEXT;
        public override string PageTitle => Messages.NETW_INTERNAL_DETAILS_TITLE;

        public override void PopulatePage()
        {
            PopulateHostNicList(Host, Connection);
        }

        public override bool EnableNext()
        {
            return SelectedHostNic != null;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Back)
                return;

            using (var dlg = new WarningDialog(string.Format(Messages.SRIOV_NETWORK_CREATE_WARNING, BrandManager.BrandConsole),
                new ThreeButtonDialog.TBDButton(Messages.SRIOV_NETWORK_CREATE, DialogResult.OK),
                ThreeButtonDialog.ButtonCancel))
            {
                var result = dlg.ShowDialog(this);
                if (result != DialogResult.OK)
                    cancel = true;
            }
        }

        #endregion

        private void PopulateHostNicList(Host host, IXenConnection conn)
        {
            comboBoxNicList.Items.Clear();
            foreach (PIF thePIF in conn.Cache.PIFs)
            {
                if (host != null && thePIF.host.opaque_ref == host.opaque_ref && thePIF.IsPhysical() && !thePIF.IsBondNIC() 
                    && thePIF.SriovCapable() && !thePIF.IsSriovPhysicalPIF())
                {
                    comboBoxNicList.Items.Add(thePIF);
                }
            }
            if (comboBoxNicList.Items.Count > 0)
                comboBoxNicList.SelectedIndex = 0;

            OnPageUpdated();
        }

        public PIF SelectedHostNic => (PIF)comboBoxNicList.SelectedItem;

        public bool AddNicToVmsAutomatically => cbxAutomatic.Checked;

        private void comboBoxNicList_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }
    }
}
