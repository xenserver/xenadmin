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

using System.Drawing;
using XenAdmin.Controls;
using XenAdmin.Properties;

namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class NewPolicyPolicyNamePage : XenTabPage
    {
        public NewPolicyPolicyNamePage()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get { return Messages.VMSS_NAME; }
        }

        public override string PageTitle
        {
            get { return Messages.VMSS_NAME_TITLE; }
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
    }
}
