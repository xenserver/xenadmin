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

﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
 using XenAdmin.Controls;
 using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.PatchingWizard;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAdmin.Wizards.RollingUpgradeWizard.PlanActions;
using XenAPI;

namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeUpgradePage : AutomatedUpdatesBasePage
    {
        public RollingUpgradeUpgradePage()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides
        public override string PageTitle { get { return Messages.UPGRADE_PLAN; } }

        public override string Text { get { return Messages.UPGRADE_PLAN; } }

        public override string HelpID { get { return "Upgradepools"; } }
        #endregion

        #region Accessors
        public List<ThreeButtonDialog> Dialogs = new List<ThreeButtonDialog>();
        public bool ManualModeSelected { private get; set; }
        public Dictionary<string, string> InstallMethodConfig { private get; set; }
        #endregion

        #region AutomatedUpdatesBesePage overrides
        public override string BlurbText()
        {
            return Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_NEW_VERSION_AUTOMATED_MODE;
        }

        protected override void GeneratePlanActions(Pool pool, List<HostPlanActions> planActions, List<PlanAction> finalActions)
        {
            //Add masters first, then the slaves that are not ugpraded
            var hostNeedUpgrade = pool.HostsToUpgrade();
            
            foreach (var host in hostNeedUpgrade)
            {
                var hostUpgradePlanActions = GetSubTasksFor(host);
                if (hostUpgradePlanActions.Count > 0)
                {
                    var hostPlanActions = new HostPlanActions(host) {InitialPlanActions = hostUpgradePlanActions};
                    planActions.Add(hostPlanActions);
                }
            }

            //add a revert pre-check action for this pool
            var problemsToRevert = ProblemsResolvedPreCheck.Where(p => hostNeedUpgrade.ToList().Select(h => h.uuid).ToList().Contains(p.Check.Host.uuid)).ToList();
            if (problemsToRevert.Count > 0)
                finalActions.Add(new UnwindProblemsAction(problemsToRevert, string.Format(Messages.REVERTING_RESOLVED_PRECHECKS_POOL, pool.Connection.Name)));
        }
        
        protected override bool ManageSemiAutomaticPlanAction(UpdateProgressBackgroundWorker bgw, PlanAction planAction)
        {
            var upgradeHostPlanAction = planAction as UpgradeManualHostPlanAction;
            if (upgradeHostPlanAction == null || !upgradeHostPlanAction.IsManual)
                return false;

            //Show dialog prepare host boot from CD or PXE boot and click OK to reboot
            string msg = string.Format(Messages.ROLLING_UPGRADE_REBOOT_MESSAGE, upgradeHostPlanAction.GetResolvedHost().Name());

            UpgradeManualHostPlanAction action = upgradeHostPlanAction;

            Program.Invoke(this, () =>
            {
                using (var dialog = new NotModalThreeButtonDialog(SystemIcons.Information, msg, Messages.REBOOT, Messages.SKIP_SERVER))
                {
                    Dialogs.Add(dialog);
                    dialog.ShowDialog(this);
                    Dialogs.Remove(dialog);
                    if (dialog.DialogResult != DialogResult.OK) // Cancel or Unknown
                    {
                        if (action.GetResolvedHost().IsMaster())
                        {
                            action.Error = new ApplicationException(Messages.EXCEPTION_USER_CANCELLED_MASTER);
                            throw action.Error;
                        }

                        action.Error = new CancelledException();
                    }
                }
            });

            if (action.Error != null)
                return true;

            string beforeRebootProductVersion = upgradeHostPlanAction.GetResolvedHost().LongProductVersion();
            string hostName = upgradeHostPlanAction.GetResolvedHost().Name();
            upgradeHostPlanAction.Timeout += upgradeHostPlanAction_Timeout;
            try
            {
                do
                {
                    if (bgw.CancellationPending)
                        break;

                    //Reboot with timeout of 20 min
                    upgradeHostPlanAction.Run();

                    //if comes back and does not have a different product version
                    if (Helpers.SameServerVersion(upgradeHostPlanAction.GetResolvedHost(), beforeRebootProductVersion))
                    {
                        using (var dialog = new NotModalThreeButtonDialog(SystemIcons.Exclamation,
                            string.Format(Messages.ROLLING_UPGRADE_REBOOT_AGAIN_MESSAGE, hostName)
                            , Messages.REBOOT_AGAIN_BUTTON_LABEL, Messages.SKIP_SERVER))
                        {
                            Program.Invoke(this, () => dialog.ShowDialog(this));
                            if (dialog.DialogResult != DialogResult.OK) // Cancel or Unknown
                            {
                                if (upgradeHostPlanAction.GetResolvedHost().IsMaster())
                                {
                                    upgradeHostPlanAction.Error = new Exception(Messages.HOST_REBOOTED_SAME_VERSION);
                                    throw upgradeHostPlanAction.Error;
                                }
                                upgradeHostPlanAction.Error = new CancelledException();
                                break;
                            }
                            else
                                upgradeHostPlanAction = new UpgradeManualHostPlanAction(upgradeHostPlanAction.GetResolvedHost());
                        }
                    }

                } while (Helpers.SameServerVersion(upgradeHostPlanAction.GetResolvedHost(), beforeRebootProductVersion));
            }
            finally
            {
                upgradeHostPlanAction.Timeout -= upgradeHostPlanAction_Timeout;
            }
            return true; // the action has been handled here
        }

        protected override bool SkipInitialPlanActions(Host host)
        {
            //Skip hosts already upgraded
            if (host.IsMaster())
            {
                var pool = Helpers.GetPoolOfOne(host.Connection);
                if (pool != null && pool.IsMasterUpgraded())
                {
                    log.Debug(string.Format("Skipping master '{0}' because it is upgraded", host.Name()));
                    return true;
                }
            }
            else
            {
                var master = Helpers.GetMaster(host.Connection);
                if (master != null && host.LongProductVersion() == master.LongProductVersion())
                {
                    log.Debug(string.Format("Skipping host '{0}' because it is upgraded", host.Name()));
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Private methods
        private List<PlanAction> GetSubTasksFor(Host host)
        {
            var runningVMs = RunningVMs(host);

            var upgradeAction = ManualModeSelected
                ? new UpgradeManualHostPlanAction(host)
                : new UpgradeHostPlanAction(host, InstallMethodConfig);

            return new List<PlanAction>
            {
                new EvacuateHostPlanAction(host),
                upgradeAction,
                new BringBabiesBackAction(runningVMs, host, true)
            };
         }

        private static List<XenRef<VM>> RunningVMs(Host host)
        {
            var vms = new List<XenRef<VM>>();
            foreach (VM vm in host.Connection.ResolveAll(host.resident_VMs))
            {
                if (!vm.is_a_real_vm())
                    continue;

                vms.Add(new XenRef<VM>(vm.opaque_ref));
            }
            return vms;
        }
        private void upgradeHostPlanAction_Timeout(object sender, EventArgs e)
        {
            var dialog = new NotModalThreeButtonDialog(SystemIcons.Exclamation, Messages.ROLLING_UPGRADE_TIMEOUT.Replace("\\n", "\n"), Messages.KEEP_WAITING_BUTTON_LABEL.Replace("\\n", "\n"), Messages.CANCEL);
            Program.Invoke(this, () => dialog.ShowDialog(this));
            if (dialog.DialogResult != DialogResult.OK) // Cancel or Unknown
            {
                UpgradeHostPlanAction action = (UpgradeHostPlanAction)sender;
                action.Cancel();
            }
        }
        #endregion

        #region Nested classes
        private class NotModalThreeButtonDialog : ThreeButtonDialog
        {
            public NotModalThreeButtonDialog(Icon icon, string msg, string button1Text, string button2Text)
                : base(new Details(icon, msg),
                    new TBDButton(button1Text, DialogResult.OK),
                    new TBDButton(button2Text, DialogResult.Cancel))
            {
                Buttons[0].Click += new EventHandler(button1_Click);
                Buttons[1].Click += new EventHandler(button2_Click);
            }

            void button1_Click(object sender, EventArgs e)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
            void button2_Click(object sender, EventArgs e)
            {
                DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
        #endregion
    }
}
