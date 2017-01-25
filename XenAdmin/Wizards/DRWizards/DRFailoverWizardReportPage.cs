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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;


namespace XenAdmin.Wizards.DRWizards
{
    public partial class DRFailoverWizardReportPage : XenTabPage
    {
        public DRFailoverWizardReportPage()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get
            {
                return Messages.DR_WIZARD_REPORTPAGE_TEXT;
            }
        }

        public override string PageTitle
        {
            get
            {
                switch (WizardType)
                {
                    case DRWizardType.Failback:
                        return String.Format(Messages.DR_WIZARD_REPORTPAGE_TITLE_FAILBACK, Connection.Name);
                    case DRWizardType.Dryrun:
                        return String.Format(Messages.DR_WIZARD_REPORTPAGE_TITLE_DRYRUN, Connection.Name);
                    default:
                        return String.Format(Messages.DR_WIZARD_REPORTPAGE_TITLE_FAILOVER, Connection.Name);
                }
            }
        }

        public override string HelpID
        {
            get 
            {
                switch (WizardType)
                {
                    case DRWizardType.Failback:
                        return "Failback_Report";
                    case DRWizardType.Dryrun:
                        return "Dryrun_Report";
                    default:
                        return "Failover_Report";
                }
            }
        }

        public override bool EnableCancel()
        {
            return false;
        }

        public override bool EnablePrevious()
        {
            return true;
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (SummaryRetreiver == null)
                return;

            textBoxSummary.Text = SummaryRetreiver.Invoke();
        }

        public Func<string> SummaryRetreiver { private get; set; }
        public DRWizardType WizardType { private get; set; }
    }
}
