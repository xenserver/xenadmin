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
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Properties;
using XenAdmin.SettingsPanels;
using XenAPI;


namespace XenAdmin.Wizards.NewVMApplianceWizard
{
    public partial class NewVMApplianceNamePage : XenTabPage, IEditPage
    {
        public NewVMApplianceNamePage()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get
            {
                return Messages.NEWVMAPPLIANCE_NAMEPAGE_TEXT;
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
                return Messages.NEWVMAPPLIANCE_NAMEPAGE_TITLE;
            }
        }

        public override string HelpID
        {
            get { return "vAppName"; } 
        }

        public string VMApplianceName
        {
            get { return textBoxName.Text; }
        }

        public string VMApplianceDescription
        {
            get { return textBoxDescription.Text; }
        }

        public override bool EnableNext()
        {
            return textBoxName.Text.Trim() != "";
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void RefreshTab(VM_appliance vmAppliance)
        {
            textBoxName.Text = vmAppliance.Name;
            textBoxDescription.Text = vmAppliance.Description;
            labelwizard.Visible = false;
        }

        public AsyncAction SaveSettings()
        {
            return null;
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            RefreshTab((VM_appliance)orig);
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
