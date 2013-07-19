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
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAdmin.Wizards.HAWizard_Pages;
using XenAPI;
using System.Drawing;


namespace XenAdmin.Wizards
{
    public partial class HAWizard : XenWizardBase
    {
        private readonly Intro xenTabPageIntro;
        private readonly AssignPriorities xenTabPageAssignPriorities;
        private readonly ChooseSR xenTabPageChooseSR;
        private readonly HAFinishPage xenTabPageHaFinish;
        
        private readonly Pool pool;

        public HAWizard(Pool pool)
            : base(pool.Connection)
        {
            InitializeComponent();

            xenTabPageIntro = new Intro();
            xenTabPageAssignPriorities = new AssignPriorities();
            xenTabPageChooseSR = new ChooseSR();
            xenTabPageHaFinish = new HAFinishPage();

            this.pool = pool;

            AddPage(xenTabPageIntro);
            AddPage(xenTabPageChooseSR);
            xenTabPageChooseSR.Pool = pool;
            AddPage(xenTabPageAssignPriorities);
            xenTabPageAssignPriorities.ProtectVmsByDefault = true;
            xenTabPageAssignPriorities.Connection = pool.Connection;//set the connection again after the pafe has been added
            AddPage(xenTabPageHaFinish);
        }

        protected override void OnShown(EventArgs e)
        {
            // Check all the hosts in the pool have xha licenses
            List<string> hostsWithoutLicense = new List<string>();
            foreach (Host host in xenConnection.Cache.Hosts)
            {
                if (host.RestrictHAOrlando)
                {
                    hostsWithoutLicense.Add(host.Name);
                }
            }

            if (hostsWithoutLicense.Count > 0)
            {
                new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       String.Format(Messages.HA_HOSTS_LACK_LICENSE, String.Join("\n", hostsWithoutLicense.ToArray())),
                       Messages.HIGH_AVAILABILITY)).ShowDialog(this);
                this.Close();
                return;
            }

            // Check for broken SRs
            List<string> brokenSRs = new List<string>();
            foreach (SR sr in xenConnection.Cache.SRs)
            {
                if (sr.HasPBDs && sr.IsBroken() && !sr.IsToolsSR && sr.shared)
                {
                    brokenSRs.Add(sr.NameWithoutHost);
                }
            }

            if (brokenSRs.Count > 0)
            {
                new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                        SystemIcons.Warning,
                        String.Format(Messages.HA_SRS_BROKEN_WARNING, String.Join("\n", brokenSRs.ToArray())),
                        Messages.HIGH_AVAILABILITY)).ShowDialog(this);
            }

            base.OnShown(e);
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            if (senderPage.GetType() == typeof(AssignPriorities))
            {
                xenTabPageHaFinish.HeartbeatSrName = xenTabPageChooseSR.SelectedHeartbeatSR.Name;
                xenTabPageHaFinish.Ntol = xenTabPageAssignPriorities.Ntol;
                xenTabPageHaFinish.RestartPriorities = xenTabPageAssignPriorities.getCurrentSettings().Values;
            }
        }

        protected override bool RunNextPagePrecheck(XenTabPage senderPage)
        {
            if (senderPage.GetType() == typeof(Intro))
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
            EnableHAAction action = new EnableHAAction(pool, xenTabPageAssignPriorities.GetCurrentStartupOptions(), new List<SR> { xenTabPageChooseSR.SelectedHeartbeatSR } , ntol);
            // We will need to re-enable buttons when the action completes
            action.Completed += Program.MainWindow.action_Completed;
            action.RunAsync();

            Program.MainWindow.UpdateToolbars();
            // Switch to logs tab, since this action takes a while and there's no feedback on the HA tab
            Program.MainWindow.SwitchToTab(MainWindow.Tab.History);
            base.FinishWizard();
        }

        protected override string WizardPaneHelpID()
        {
            return "HAWizard";
        }
    }
}
