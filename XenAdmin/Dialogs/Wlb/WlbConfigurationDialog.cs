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
using System.Windows.Forms;
using XenAdmin.Actions.Wlb;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.SettingsPanels;
using XenAPI;


namespace XenAdmin.Dialogs.Wlb
{
    public partial class WlbConfigurationDialog : XenAdmin.Dialogs.VerticallyTabbedDialog
    {
        Pool _pool;
        WlbPoolConfiguration _poolConfiguration = null;

        public WlbConfigurationDialog(Pool pool)
        {
            _pool = pool;
            
            InitializeComponent();

            Text = String.Format(Messages.WLB_CONFIGURATION_DIALOG, Helpers.GetName(_pool));
            Build();
        }

        private void Build()
        {
            _poolConfiguration = RetrieveWLBConfiguration();

            if (null != _poolConfiguration)
            {
                verticalTabs.Items.Clear();
                verticalTabs.Items.Add(wlbOptimizationModePage);
                wlbOptimizationModePage.PoolConfiguration = _poolConfiguration;
                wlbOptimizationModePage.Pool = _pool;

                if (_poolConfiguration.IsMROrLater)
                {
                    verticalTabs.Items.Add(wlbAutomationPage);
                    wlbAutomationPage.Connection = _pool.Connection;
                    wlbAutomationPage.PoolConfiguration = _poolConfiguration;

                    //verticalTabs.Items.Add(wlbPowerManagementPage);
                    //wlbPowerManagementPage.Connection = _pool.Connection;
                    //wlbPowerManagementPage.PoolConfiguration = _poolConfiguration;
                }

                verticalTabs.Items.Add(wlbThresholdsPage);
                wlbThresholdsPage.Connection = _pool.Connection;
                wlbThresholdsPage.PoolConfiguration = _poolConfiguration;

                verticalTabs.Items.Add(wlbMetricWeightingPage);
                wlbMetricWeightingPage.Connection = _pool.Connection;
                wlbMetricWeightingPage.PoolConfiguration = _poolConfiguration;

                if (_poolConfiguration.IsMROrLater)
                {
                    verticalTabs.Items.Add(wlbHostExclusionPage);
                    wlbHostExclusionPage.PoolConfiguration = _poolConfiguration;
                    wlbHostExclusionPage.Connection = _pool.Connection;
                }

                verticalTabs.Items.Add(wlbAdvancedSettingsPage);
                wlbAdvancedSettingsPage.PoolConfiguration = _poolConfiguration;
                wlbAdvancedSettingsPage.Pool = _pool;

                verticalTabs.SelectedIndex = 0;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private WlbPoolConfiguration RetrieveWLBConfiguration()
        {
            RetrieveWlbConfigurationAction action = new RetrieveWlbConfigurationAction(_pool);
            using (ActionProgressDialog dialog = new ActionProgressDialog(action, ProgressBarStyle.Blocks))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);
            }

            if (!action.Succeeded || null == action.WlbConfiguration || action.WlbConfiguration.Count == 0)
            {
                return null;
            }
            else
            {
                return new WlbPoolConfiguration(StripReportSubscriptions(action.WlbConfiguration));
            }
        }

        public WlbPoolConfiguration WlbPoolConfiguration
        {
            get { return _poolConfiguration; }
        }

        private Dictionary<string,string> StripReportSubscriptions(Dictionary<string,string> baseConfig)
        {
            Dictionary<string, string> newConfig = new Dictionary<string, string>();
            foreach (string key in baseConfig.Keys)
            {
                if (!key.StartsWith(WlbConfigurationBase.WlbConfigurationKeyBase.rpSub.ToString()))
                {
                    newConfig.Add(key, baseConfig[key]);
                }
            }
            return newConfig;
        }
        
        private void okButton_Click(object sender, EventArgs e)
        {
            // Have any of the fields in the tab pages changed?
            if (!HasChanged)
            {
                Close();
                return;
            }

            if (!ValidToSave)
            {
                // Keep dialog open and allow user to correct the error as
                // indicated by the balloon tooltip.
                DialogResult = DialogResult.None;
                return;
            }

            SaveSettings();
            Close();
        }

        /*
        * Iterates through all of the tab pages checking for changes and
        * return the status.
        */
        public bool HasChanged
        {
            get
            {
                foreach (IEditPage editPage in verticalTabs.Items)
                    if (editPage.HasChanged)
                        return true;

                return false;
            }
        }

        /*
         * Iterate through all tab pages looking for local validation errors.  If
         * we encounter a local validation error on a TabPage, then make the TabPage
         * the selected, and have the inner control show one or more balloon tips.  Keep
         * the dialog open.
         */
        public bool ValidToSave
        {
            get
            {
                foreach (IEditPage editPage in verticalTabs.Items)
                {
                    if (editPage is XenAdmin.SettingsPanels.WlbAdvancedSettingsPage && !editPage.ValidToSave)
                    {
                        SelectPage(editPage);

                        // Show local validation balloon message for this tab page.
                        editPage.ShowLocalValidationMessages();

                        return false;
                    }
                }

                return true;
            }
        }

        public void SaveSettings()
        {
            foreach (IEditPage editPage in verticalTabs.Items)
            {
                if (editPage.HasChanged)
                {
                    editPage.SaveSettings();
                }
            }
        }

        private void WlbConfigurationDialog_SizeChanged(object sender, EventArgs e)
        {
            // When the size of configuration dialog is changed,
            // the SplitContainer panels should expand and contract correspondingly.
            // Originally the dialog height is 750, SplitContainer height is 674,
            // the difference is 76.
            // The SplitContainer height must track the change of the dialog height.
            splitContainer.Height = this.Height - 76;
        }

    }
}
