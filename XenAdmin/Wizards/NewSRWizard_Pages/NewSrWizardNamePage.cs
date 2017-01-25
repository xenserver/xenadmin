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
using System.Text.RegularExpressions;
using XenAdmin.Controls;
using XenAdmin.Core;


namespace XenAdmin.Wizards.NewSRWizard_Pages
{
    public partial class NewSrWizardNamePage : XenTabPage
    {
        public NewSrWizardNamePage()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string Text
        {
            get { return Messages.NEWSR_NAMEPAGE_TEXT; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWSR_NAMEPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "SrName"; }
        }

        public override bool EnableNext()
        {
            return textBoxName.Text.Trim() != "";
        }

        public override bool EnablePrevious()
        {
            return m_srWizardType.SrToReattach == null || MatchingFrontends > 1;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (direction == PageLoadedDirection.Forward)
                HelpersGUI.FocusFirstControl(Controls);
        }

        public override void PopulatePage()
        {
            textBoxName.Text = m_srWizardType.SrName;

            if (!string.IsNullOrEmpty(m_srWizardType.Description))
            {
                textBoxDescription.Text = m_srWizardType.Description;
                checkBoxAutoDescription.Checked = false;
            }

            ToggleDescriptionControlsEnabledState();
            OnPageUpdated();
        }

        #endregion

        public string SrName{get{return textBoxName.Text.Trim();}}
        public string SrDescription { get { return checkBoxAutoDescription.Checked ? null : textBoxDescription.Text; } }
        
        public bool AutoDescriptionRequired
        {
            get { return checkBoxAutoDescription.Checked;  }
        }

        private SrWizardType m_srWizardType;
        public SrWizardType SrWizardType { set { m_srWizardType = value; } }

        private void ToggleDescriptionControlsEnabledState()
        {
            textBoxDescription.Enabled = labelDescription.Enabled = !checkBoxAutoDescription.Checked;
        }

        public int MatchingFrontends { private get; set; }

        #region Event handlers

        private void checkBoxAutoDescription_CheckedChanged(object sender, EventArgs e)
        {
            ToggleDescriptionControlsEnabledState();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        #endregion
    }
}
