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

using XenAdmin.Controls;


namespace XenAdmin.Wizards.DRWizards
{
    public partial class DRFailoverWizardFirstPage : XenTabPage
    {
        public DRFailoverWizardFirstPage()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get { return Messages.DR_WIZARD_FIRSTPAGE_TEXT; }
        }

        public override string PageTitle
        {
            get
            {
                switch (WizardType)
                {
                    case DRWizardType.Failover:
                        return Messages.DR_WIZARD_FIRSTPAGE_TITLE_FAILOVER;
                    case DRWizardType.Failback:
                        return Messages.DR_WIZARD_FIRSTPAGE_TITLE_FAILBACK;
                    case DRWizardType.Dryrun:
                        return Messages.DR_WIZARD_FIRSTPAGE_TITLE_DRYRUN;
                }
                return Messages.DR_WIZARD_FIRSTPAGE_TEXT;
            }
        }

        public override string HelpID
        {
            get
            {
                switch (WizardType)
                {
                    case DRWizardType.Failback:
                        return "Failback_BeforeYouStart";
                    case DRWizardType.Dryrun:
                        return "Dryrun_BeforeYouStart";
                    default:
                        return "Failover_BeforeYouStart";
                }
            }
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            
            if (direction == PageLoadedDirection.Forward)
                SetupLabels();
        }

        public DRWizardType WizardType { private get; set; }

        void SetupLabels()
        {
            switch (WizardType)
            {
                case DRWizardType.Failover:
                    panelWarning.Visible = true;
                    labelInformation1.Text = Messages.DR_WIZARD_FIRSTPAGE_FAILOVER_LINE1;
                    labelInformation2.Text = Messages.DR_WIZARD_FIRSTPAGE_FAILOVER_LINE2;
                    labelInformation3.Text = Messages.DR_WIZARD_FIRSTPAGE_FAILOVER_LINE3;
                    labelInformation4.Text = Messages.DR_WIZARD_FIRSTPAGE_FAILOVER_LINE4;
                    break;
                case DRWizardType.Failback:
                    panelWarning.Visible = true;
                    labelInformation1.Text = Messages.DR_WIZARD_FIRSTPAGE_FAILBACK_LINE1;
                    labelInformation2.Text = Messages.DR_WIZARD_FIRSTPAGE_FAILBACK_LINE2;
                    labelInformation3.Text = Messages.DR_WIZARD_FIRSTPAGE_FAILBACK_LINE3;
                    labelInformation4.Text = Messages.DR_WIZARD_FIRSTPAGE_FAILBACK_LINE4;
                    break;
                case DRWizardType.Dryrun:
                    panelWarning.Visible = false;
                    labelInformation1.Text = string.Format(Messages.DR_WIZARD_FIRSTPAGE_DRYRUN_LINE1, Connection.Name);
                    labelInformation2.Text = Messages.DR_WIZARD_FIRSTPAGE_DRYRUN_LINE2;
                    labelInformation3.Text = Messages.DR_WIZARD_FIRSTPAGE_DRYRUN_LINE3;
                    break;
            }
            labelInformation3.Text = labelInformation3.Text.Replace("*", " \u2022");
        }
    }
}
