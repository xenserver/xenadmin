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

using System.Linq;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    public partial class ChooseSrProvisioningPage : XenTabPage

    {
        public ChooseSrProvisioningPage()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string Text { get { return Messages.PROVISIONING; } }

        public override string PageTitle { get { return Messages.CHOOSE_SR_PROVISIONING_PAGE_TITLE; } }

        public override string HelpID
        {
            get { return "Provisioning"; }
        }
        #endregion

        public bool IsGfs2
        {
            get
            {
                return radioButtonGfs2.Checked;
            }
        }

        public override void PopulatePage()
        {
            var clusteringEnabled = Connection.Cache.Clusters.Any();
            var gfs2Allowed = !Helpers.FeatureForbidden(Connection, Host.RestrictGfs2) && clusteringEnabled;

            radioButtonGfs2.Enabled = labelGFS2.Enabled = gfs2Allowed;
            tableLayoutInfo.Visible = radioButtonLvm.Checked = !gfs2Allowed;
            labelWarning.Text = Helpers.FeatureForbidden(Connection, Host.RestrictGfs2)
                ? Messages.GFS2_INCORRECT_POOL_LICENSE
                : Messages.GFS2_REQUIRES_CLUSTERING_ENABLED;
            linkLabelPoolProperties.Visible = !clusteringEnabled && !Helpers.FeatureForbidden(Connection, Host.RestrictGfs2);
        }

        private void linkLabelPoolProperties_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            var pool = Helpers.GetPoolOfOne(Connection);

            using (PropertiesDialog propertiesDialog = new PropertiesDialog(pool))
            {
                propertiesDialog.SelectClusteringEditPage();
                propertiesDialog.ShowDialog(this);
            }
        }
    }
}
