/* Copyright (c) Citrix Systems Inc. 
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
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.HAWizard_Pages
{
    public partial class HAFinishPage : XenTabPage
    {
        public HAFinishPage()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string Text { get { return Messages.FINISH_PAGE_TEXT; } }

        public override string PageTitle { get { return Messages.HA_WIZARD_FINISH_PAGE_TITLE; } }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            int alwaysRestartHighPriority = 0, alwaysRestart = 0, bestEffort = 0, doNotRestart = 0;
            foreach (VM.HA_Restart_Priority priority in RestartPriorities)
            {
                switch (priority)
                {
                    case VM.HA_Restart_Priority.AlwaysRestartHighPriority:
                        alwaysRestartHighPriority++;
                        break;
                    case VM.HA_Restart_Priority.AlwaysRestart:
                    case VM.HA_Restart_Priority.Restart:
                        alwaysRestart++;
                        break;
                    case VM.HA_Restart_Priority.BestEffort:
                        bestEffort++;
                        break;
                    case VM.HA_Restart_Priority.DoNotRestart:
                        doNotRestart++;
                        break;
                }
            }
            if (Helpers.BostonOrGreater(Connection))
                labelSummary.Text = String.Format(Messages.HAWIZ_SUMMARY_NEW,
                                                  HeartbeatSrName.Ellipsise(50),
                                                  Ntol,
                                                  GetVmNumber(alwaysRestart),
                                                  GetVmNumber(bestEffort),
                                                  GetVmNumber(doNotRestart));
            else
                labelSummary.Text = String.Format(Messages.HAWIZ_SUMMARY,
                                                  HeartbeatSrName.Ellipsise(50),
                                                  Ntol,
                                                  GetVmNumber(alwaysRestartHighPriority),
                                                  GetVmNumber(alwaysRestart),
                                                  GetVmNumber(bestEffort),
                                                  GetVmNumber(doNotRestart));

            // If the user hasn't protected any VMs, show a warning.
            labelNoVmsProtected.Visible = (bestEffort + alwaysRestart + alwaysRestartHighPriority == 0) && doNotRestart > 0;
            labelNoHaGuaranteed.Visible = Ntol == 0;
            pictureBox1.Visible = labelNoVmsProtected.Visible || labelNoHaGuaranteed.Visible;
        }

        #endregion

        public string HeartbeatSrName { private get; set; }
        public long Ntol { private get; set; }
        public IEnumerable<VM.HA_Restart_Priority> RestartPriorities { private get; set; }

        private string GetVmNumber(int number)
        {
            return string.Format("{0} {1}", number, number == 1 ? Messages.VM : Messages.VMS);
        }
    }
}
