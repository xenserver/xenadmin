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
using XenAdmin.Controls;

namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class NewPolicyFinishPage : XenTabPage
    {
        private readonly string _pageTitle;

        public NewPolicyFinishPage(string pageText, string checkBoxText, string finishPageTitle)
        {
            InitializeComponent();
            this.label13.Text = pageText;
            this.checkBox1.Text = checkBoxText;
            _pageTitle = finishPageTitle;
        }

        public override string Text
        {
            get
            {
                return Messages.FINISH_PAGE_TEXT;
            }
        }

        public override string PageTitle
        {
            get
            {
                return _pageTitle;
            }
        }

        public override string HelpID
        {
            get { return "Finish"; }
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            textBoxSummary.Text = Summary;
            checkBox1.Enabled = SelectedVMsCount > 0;
        }

        public string Summary { private get; set; }
        public int SelectedVMsCount { private get; set; }

        public bool RunNow
        {
            get
            {
                return checkBox1.Checked;
            }
        }
    }
}
