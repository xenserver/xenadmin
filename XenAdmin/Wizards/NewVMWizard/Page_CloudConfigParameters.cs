/* Copyright (c) Citrix Systems Inc. 
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
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Controls;
using XenAdmin.Actions;


namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_CloudConfigParameters : XenTabPage
    {
        private VM Template;

        public Host Affinity { get; set; }

        public Page_CloudConfigParameters()
        {
            InitializeComponent();
        }

        void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        public override string Text
        {
            get { return Messages.NEWVMWIZARD_CLOUD_CONFIG_PARAMETERS_PAGE; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMWIZARD_CLOUD_CONFIG_PARAMETERS_PAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "CloudConfigParameters"; } //TODO Usha?
        }

        public override bool EnableNext()
        {
            return true;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (SelectedTemplate == Template)
                return;

            Template = SelectedTemplate;

            GetDefaultParameters();
        }

        public VM SelectedTemplate { private get; set; }


        public string ConfigDriveTemplateText
        {
            get
            {
                return 
                    IncludeConfigDriveCheckBox.Checked ? ConfigDriveTemplateTextBox.Text.Trim() : string.Empty;
            }
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> sum = new List<KeyValuePair<string, string>>();
                sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_CLOUD_CONFIG_PARAMETERS_PAGE, IncludeConfigDriveCheckBox.Checked ? "Config drive included" : "Config drive not included"));
                return sum;
            }
        }

        private void IncludeConfigDriveCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ConfigDriveTemplateTextBox.Enabled = IncludeConfigDriveCheckBox.Checked;
        }

        private void GetDefaultParameters()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("templateuuid", Template.uuid);

            var action = new ExecutePluginAction(Connection, Affinity ?? Helpers.GetMaster(Connection),
                        "xscontainer",//plugin
                        "get_config_drive_default",//function
                        parameters,
                        true); //hidefromlogs

            action.RunExternal(Connection.Session);
            var result = action.Result.Replace("\n", Environment.NewLine);

            ConfigDriveTemplateTextBox.Text = result;
        }

        private void reloadDefaults_Click(object sender, EventArgs e)
        {
            GetDefaultParameters();
        }
    }
}
