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
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Controls;


namespace XenAdmin.Wizards.NewVMWizard
{
    public partial class Page_Name : XenTabPage
    {
        private VM Template;

        public Page_Name()
        {
            InitializeComponent();
            NameTextBox.TextChanged += new EventHandler(NameTextBox_TextChanged);
        }

        void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        public override string Text
        {
            get { return Messages.NEWVMWIZARD_NAMEPAGE_NAME; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMWIZARD_NAMEPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "Name"; }
        }

        public override bool EnableNext()
        {
            return !string.IsNullOrEmpty(SelectedName);
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            if (SelectedTemplate == Template)
                return;

            Template = SelectedTemplate;

            NameTextBox.Text = Helpers.DefaultVMName(Helpers.GetName(Template), Connection);
        }

        public override void SelectDefaultControl()
        {
            NameTextBox.Select();
        }

        public VM SelectedTemplate { private get; set; }

        public string SelectedName
        {
            get
            {
                return NameTextBox.Text.Trim();
            }
        }

        public string SelectedDescription
        {
            get
            {
                return DescriptionTextBox.Text.Trim();
            }
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                List<KeyValuePair<string, string>> sum = new List<KeyValuePair<string, string>>();
                sum.Add(new KeyValuePair<string, string>(Messages.NEWVMWIZARD_NAMEPAGE_NAME2, SelectedName));
                return sum;
            }
        }
    }
}
