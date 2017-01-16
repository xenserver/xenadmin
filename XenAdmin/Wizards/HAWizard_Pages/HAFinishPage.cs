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
using System.Linq;
using System.Text;
using XenAdmin.Controls;
using XenAPI;

namespace XenAdmin.Wizards.HAWizard_Pages
{
    public partial class HAFinishPage : XenTabPage
    {
        public HAFinishPage()
        {
            InitializeComponent();
            ClearControls();
        }

        #region XenTabPage overrides

        public override string Text { get { return Messages.FINISH_PAGE_TEXT; } }

        public override string PageTitle { get { return Messages.HA_WIZARD_FINISH_PAGE_TITLE; } }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Back)
                ClearControls();
            
            base.PageLeave(direction, ref cancel);
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            
            labelSr.Text = HeartbeatSrName.Ellipsise(50);
            labelNtol.Text = Ntol.ToString();
            labelRestart.Text = GetVmNumber(AlwaysRestart);
            labelBestEffort.Text = GetVmNumber(BestEffort);
            labelDoNotRestart.Text = GetVmNumber(DoNotRestart);

            // If the user hasn't protected any VMs, show a warning.
            labelNoVmsProtected.Visible = (BestEffort + AlwaysRestart + AlwaysRestartHighPriority == 0) && DoNotRestart > 0;
            labelNoHaGuaranteed.Visible = Ntol == 0;
            pictureBox1.Visible = labelNoVmsProtected.Visible || labelNoHaGuaranteed.Visible;
        }

        #endregion

        public string HeartbeatSrName { private get; set; }
        public long Ntol { private get; set; }
        public int AlwaysRestartHighPriority = 0;
        public int AlwaysRestart = 0;
        public int BestEffort = 0;
        public int DoNotRestart = 0;

        private string GetVmNumber(int number)
        {
            return number == 1 ? Messages.VMS_ONE : string.Format(Messages.VMS_MANY, number);
        }

        private void ClearControls()
        {
            pictureBox1.Visible = labelNoVmsProtected.Visible = labelNoHaGuaranteed.Visible = false;
            labelSr.Text = labelNtol.Text = labelRestart.Text = labelBestEffort.Text = labelDoNotRestart.Text = string.Empty;
        }
    }
}
