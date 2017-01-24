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


namespace XenAdmin.Wizards.DRWizards
{
    public partial class DRFailoverWizardWelcomePage : XenTabPage
    {
        public event Action<DRWizardType> WizardTypeChanged;
        private DRWizardType m_wizardType;

        public DRFailoverWizardWelcomePage()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get { return Messages.DR_WIZARD_WELCOMEPAGE_TEXT; }
        }

        public override string PageTitle
        {
            get { return Messages.DR_WIZARD_WELCOMEPAGE_TITLE; }
        }

        public override string HelpID
        {
            get
            {
                switch (m_wizardType)
                {
                    case DRWizardType.Failover:
                        return "Failover_Welcome";
                    case DRWizardType.Failback:
                        return "Failback_Welcome";
                    case DRWizardType.Dryrun:
                        return "Dryrun_Welcome";
                    default:
                        return "Welcome";
                }
            }
        }

        public override bool EnableNext()
        {
            return radioButtonFailover.Checked || radioButtonFailback.Checked || radioButtonDryrun.Checked;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            if (direction == PageLoadedDirection.Forward)
            {
                labelFailoverDescription.Text = String.Format(labelFailoverDescription.Text, Connection.Name);
                labelDryrunDescription.Text = String.Format(labelDryrunDescription.Text, Connection.Name);
            }
        }

        public void SetWizardType(DRWizardType wizardType)
        {
            switch (wizardType)
            {
                case DRWizardType.Failover:
                    radioButtonFailover.Checked = true;
                    break;
                case DRWizardType.Failback:
                    radioButtonFailback.Checked = true;
                    break;
                case DRWizardType.Dryrun:
                    radioButtonDryrun.Checked = true;
                    break;
                default:
                    radioButtonFailover.Checked = radioButtonDryrun.Checked = radioButtonDryrun.Checked = false;
                    break;
            }
            OnWizardTypeChaged(wizardType);
        }

        private void OnWizardTypeChaged(DRWizardType wizardType)
        {
            m_wizardType = wizardType;

            if (WizardTypeChanged != null)
                WizardTypeChanged(wizardType);

            OnPageUpdated();
        }

        private void radioButtonFailover_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFailover.Checked)
                OnWizardTypeChaged(DRWizardType.Failover);
        }

        private void radioButtonFailback_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonFailback.Checked)
                OnWizardTypeChaged(DRWizardType.Failback);
        }

        private void radioButtonDryrun_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDryrun.Checked)
                OnWizardTypeChaged(DRWizardType.Dryrun);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!radioButtonFailover.Checked)
                radioButtonFailover.Checked = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (!radioButtonFailback.Checked)
                radioButtonFailback.Checked = true;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!radioButtonDryrun.Checked)
                radioButtonDryrun.Checked = true;
        }
    }
}
