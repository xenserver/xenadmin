/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.GenericPages;
using XenAdmin.Wizards.HAWizard_Pages;
using XenAPI;


namespace XenAdmin.Wizards
{
    public partial class HAWizard : XenWizardBase
    {
        private readonly Intro xenTabPageIntro;
        private readonly RBACWarningPage m_pageRbac;
        private readonly AssignPriorities xenTabPageAssignPriorities;
        private readonly ChooseSR xenTabPageChooseSR;
        private readonly HAFinishPage xenTabPageHaFinish;

        private readonly bool _rbacNeeded;
        private readonly Pool pool;

        public HAWizard(Pool pool)
            : base(pool.Connection)
        {
            InitializeComponent();

            xenTabPageIntro = new Intro();
            m_pageRbac = new RBACWarningPage();
            xenTabPageAssignPriorities = new AssignPriorities();
            xenTabPageChooseSR = new ChooseSR();
            xenTabPageHaFinish = new HAFinishPage();

            this.pool = pool;
            _rbacNeeded = Helpers.ConnectionRequiresRbac(pool.Connection);

            AddPage(xenTabPageIntro);

            if (_rbacNeeded)
            {
                var methodsToCheck = new RbacMethodList(
                    "vm.set_ha_restart_priority",
                    "vm.set_order",
                    "vm.set_start_delay",
                    "pool.sync_database",
                    "pool.ha_compute_hypothetical_max_host_failures_to_tolerate",
                    "pool.set_ha_host_failures_to_tolerate",
                    "pool.enable_ha",
                    "sr.assert_can_host_ha_statefile");
                m_pageRbac.SetPermissionChecks(xenConnection,
                    new WizardRbacCheck(Messages.RBAC_HA_ENABLE_WARNING, methodsToCheck) {Blocking = true});
                AddPage(m_pageRbac);
            }

            AddPage(xenTabPageChooseSR);
            xenTabPageChooseSR.Pool = pool;
            AddPage(xenTabPageAssignPriorities);
            xenTabPageAssignPriorities.ProtectVmsByDefault = true;
            xenTabPageAssignPriorities.Connection = pool.Connection;//set the connection again after the page has been added
            AddPage(xenTabPageHaFinish);
        }

        protected override void OnShown(EventArgs e)
        {
            // Check for broken SRs
            List<string> brokenSRs = new List<string>();
            foreach (SR sr in xenConnection.Cache.SRs)
            {
                if (sr.HasPBDs() && sr.IsBroken() && !sr.IsToolsSR() && sr.shared)
                {
                    brokenSRs.Add(sr.NameWithoutHost());
                }
            }

            if (brokenSRs.Count > 0)
            {
                using (var dlg = new WarningDialog(String.Format(Messages.HA_SRS_BROKEN_WARNING, String.Join("\n", brokenSRs.ToArray())))
                    {WindowTitle = Messages.HIGH_AVAILABILITY})
                {
                    dlg.ShowDialog(this);
                }
            }

            base.OnShown(e);
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            if (senderPage.GetType() == typeof(AssignPriorities))
            {
                xenTabPageHaFinish.HeartbeatSrName = xenTabPageChooseSR.SelectedHeartbeatSR.Name();
                xenTabPageHaFinish.Ntol = xenTabPageAssignPriorities.Ntol;

                int alwaysRestartHighPriority = 0, alwaysRestart = 0, bestEffort = 0, doNotRestart = 0;
                foreach (VM.HaRestartPriority priority in xenTabPageAssignPriorities.CurrentSettings.Values)
                {
                    switch (priority)
                    {
                        case VM.HaRestartPriority.AlwaysRestartHighPriority:
                            alwaysRestartHighPriority++;
                            break;
                        case VM.HaRestartPriority.AlwaysRestart:
                        case VM.HaRestartPriority.Restart:
                            alwaysRestart++;
                            break;
                        case VM.HaRestartPriority.BestEffort:
                            bestEffort++;
                            break;
                        case VM.HaRestartPriority.DoNotRestart:
                            doNotRestart++;
                            break;
                    }
                }

                xenTabPageHaFinish.AlwaysRestartHighPriority = alwaysRestartHighPriority;
                xenTabPageHaFinish.AlwaysRestart = alwaysRestart;
                xenTabPageHaFinish.BestEffort = bestEffort;
                xenTabPageHaFinish.DoNotRestart = doNotRestart;
            }
        }

        protected override bool RunNextPagePrecheck(XenTabPage senderPage)
        {
            var pageType = senderPage.GetType();

            if (pageType == typeof(Intro) && !_rbacNeeded || pageType == typeof(RBACWarningPage))
            {
                // Start HB SR scan
                // If scan finds no suitable SRs ChooseSR will show sensible text and disallow progress.
                // If scan returns false user has cancelled and we should stay on intro.
                return xenTabPageChooseSR.ScanForHeartbeatSRs();
            }
            return true;
        }

        protected override void FinishWizard()
        {
            long ntol = xenTabPageAssignPriorities.Ntol;

            // Save configured restart priorities and enable HA.
            EnableHAAction action = new EnableHAAction(pool, xenTabPageAssignPriorities.GetChangedStartupOptions(), new List<SR> { xenTabPageChooseSR.SelectedHeartbeatSR } , ntol);
            // We will need to re-enable buttons when the action completes
            action.Completed += Program.MainWindow.action_Completed;
            action.RunAsync();
            base.FinishWizard();
        }

        protected override string WizardPaneHelpID()
        {
            return "HAWizard";
        }
    }
}
