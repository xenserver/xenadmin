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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs.ServerUpdates;

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_FirstPage : XenTabPage
    {
        public PatchingWizard_FirstPage()
        {
            InitializeComponent();
            radioButtonCdn.Text = string.Format(Messages.CONFIG_CDN_UPDATES_TAB_TITLE, BrandManager.ProductBrand, BrandManager.ProductVersionPost82);
            radioButtonLcm.Text = string.Format(Messages.CONFIG_LCM_UPDATES_TAB_TITLE, BrandManager.ProductVersion821);
            label5.Text = string.Format(label5.Text, BrandManager.BrandConsole);
            label6.Text = string.Format(label6.Text, BrandManager.BrandConsole, BrandManager.CompanyNameLegacy);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            var connections = ConnectionsManager.XenConnectionsCopy;

            if (connections.Count > 0 && connections.All(c => c.IsConnected && !Helpers.CloudOrGreater(c)))
                radioButtonLcm.Checked = true;
            else
                radioButtonCdn.Checked = true;
        }

        public override string Text => Messages.BEFORE_YOU_START;

        public override string  PageTitle => Messages.BEFORE_YOU_START;

        public bool IsNewGeneration
        {
            get => radioButtonCdn.Checked;
            set
            {
                radioButtonCdn.Checked = value;
                radioButtonLcm.Checked = !value;
            }
        }

        private void ToggleClientIdInfo()
        {
            pictureBox4.Visible = label6.Visible = linkLabelClientId.Visible = radioButtonLcm.Checked;
        }

        private void radioButtonCdn_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCdn.Checked)
                ToggleClientIdInfo();
        }

        private void radioButtonLcm_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonLcm.Checked)
                ToggleClientIdInfo();
        }

        private void linkLabelClientId_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var dialog = new ConfigUpdatesDialog())
            {
                dialog.SelectLcmTab();
                dialog.ShowDialog(this);
            }
        }
    }
}
