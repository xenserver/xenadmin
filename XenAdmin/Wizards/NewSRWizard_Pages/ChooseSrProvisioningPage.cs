﻿/* Copyright (c) Citrix Systems, Inc. 
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

using System.ComponentModel;
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
            Cluster_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(Cluster_CollectionChanged);
        }
        private readonly CollectionChangeEventHandler Cluster_CollectionChangedWithInvoke;
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

        private void RefreshPage()
        {
            var clusteringEnabled = Connection.Cache.Clusters.Any();
            var restrictGfs2 = Helpers.FeatureForbidden(Connection, Host.RestrictCorosync);
            var gfs2Allowed = !restrictGfs2 && clusteringEnabled;

            radioButtonGfs2.Enabled = labelGFS2.Enabled = gfs2Allowed;

            if (!gfs2Allowed)
            {
                radioButtonLvm.Checked = true;
            }

            tableLayoutInfo.Visible = !gfs2Allowed;
            labelWarning.Text = restrictGfs2
                ? Messages.GFS2_INCORRECT_POOL_LICENSE
                : Messages.GFS2_REQUIRES_CLUSTERING_ENABLED;
            linkLabelPoolProperties.Visible = !clusteringEnabled && !restrictGfs2;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            RefreshPage();
            Connection.Cache.RegisterCollectionChanged<Cluster>(Cluster_CollectionChangedWithInvoke);
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            Connection.Cache.DeregisterCollectionChanged<Cluster>(Cluster_CollectionChangedWithInvoke);
            
            base.PageLeave(direction, ref cancel);
        }

        private void linkLabelPoolProperties_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            var pool = Helpers.GetPoolOfOne(Connection);

            if (pool == null)
                return;

            using (PropertiesDialog propertiesDialog = new PropertiesDialog(pool))
            {
                propertiesDialog.SelectClusteringEditPage();
                propertiesDialog.ShowDialog(this);
            }
        }

        /// <summary>
        /// Called when the current IXenConnection's VM dictionary changes.
        /// </summary>
        private void Cluster_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Program.AssertOnEventThread();

            RefreshPage();
        }
    }
}
