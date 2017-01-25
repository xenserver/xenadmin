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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Properties;
using XenAdmin.SettingsPanels;
using XenAPI;

namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class NewPolicyPolicyNamePage : XenTabPage, IEditPage
    {
        private readonly string _tabName;
        private readonly string _tabTitle;

        public NewPolicyPolicyNamePage(string pageText, string pageTextMore, string tabName, string tabTitle, string nameFieldText)
        {
            InitializeComponent();
            this.labelwizard.Text = pageText;
            this.autoHeightLabel1.Text = pageTextMore;
            this.label2.Text = nameFieldText;
            _tabName = tabName;
            _tabTitle = tabTitle;
        }
        public override string Text
        {
            get
            {
                return _tabName;
            }
        }

        public string SubText
        {
            get { return string.Empty; }
        }

        public Image Image
        {
            get { return Resources.edit_16; }
        }

        public override string PageTitle
        {
            get
            {
                return _tabTitle;
            }
        }

        public override string HelpID
        {
            get { return "PolicyName"; }
        }

        public string PolicyName
        {
            get { return textBoxName.Text; }
        }

        public string PolicyDescription
        {
            get { return textBoxDescription.Text; }
        }

        public override bool EnableNext()
        {
            return textBoxName.Text.Trim() != "";
        }

        private void textBoxName_TextChanged(object sender, System.EventArgs e)
        {
            OnPageUpdated();
        }

        private void RefreshTab(VMPP vmpp)
        {
            textBoxName.Text = vmpp.Name;
            textBoxDescription.Text = vmpp.Description;
            labelwizard.Visible = false;
        }

        public AsyncAction SaveSettings()
        {
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            RefreshTab((VMPP) orig);
        }

        public bool ValidToSave
        {
            get { return false; }
        }

        public void ShowLocalValidationMessages()
        {

        }

        public void Cleanup()
        {

        }

        public bool HasChanged
        {
            get { return false; }
        }
    }
}
