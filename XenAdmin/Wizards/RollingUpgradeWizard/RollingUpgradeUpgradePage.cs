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

        protected override string BlurbText()
        {
            return Messages.ROLLING_UPGRADE_UPGRADE_IN_PROGRESS;
        }

        protected override string SuccessMessageOnCompletion(bool multiplePools)
        {
            return multiplePools ? Messages.ROLLING_UPGRADE_SUCCESS_MANY : Messages.ROLLING_UPGRADE_SUCCESS_ONE;
        }

        protected override string FailureMessageOnCompletion(bool multiplePools)
        {
            return multiplePools ? Messages.ROLLING_UPGRADE_ERROR_MANY : Messages.ROLLING_UPGRADE_ERROR_ONE;
        }

        protected override string SuccessMessagePerPool()
        {
            return Messages.ROLLING_UPGRADE_SUCCESS_ONE;
        }

        protected override string FailureMessagePerPool(bool multipleErrors)
        {
            return multipleErrors ? Messages.ROLLING_UPGRADE_ERROR_POOL_MANY : Messages.ROLLING_UPGRADE_ERROR_POOL_ONE;
        }

        protected override void GeneratePlanActions(Pool pool, List<HostPlanActions> planActions, List<PlanAction> finalActions)
        {
            //Add masters first, then the slaves that are not ugpraded
            var hostNeedUpgrade = pool.HostsToUpgrade();
            
            foreach (var host in hostNeedUpgrade)
            {
                planActions.Add(GetSubTasksFor(host));
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

        private Dictionary<UpdateProgressBackgroundWorker, List<XenServerPatch>> AllUploadedPatches = new Dictionary<UpdateProgressBackgroundWorker, List<XenServerPatch>>(); 
        private Dictionary<UpdateProgressBackgroundWorker, List<XenServerPatch>> MinimalPatches = new Dictionary<UpdateProgressBackgroundWorker, List<XenServerPatch>>(); // should be calculated only once per pool (to ensure update homogeneity)

        protected override void DoAfterInitialPlanActions(UpdateProgressBackgroundWorker bgw, Host host, List<Host> hosts)
        {
            var hostPlanActions = bgw.HostActions.FirstOrDefault(ha => ha.Host.Equals(host));
            if (hostPlanActions == null)
                return;

            if (hostPlanActions.UpdatesPlanActions.Count > 0) // this is a retry; do not recreate actions
                return; 

            if (!ApplyUpdatesToNewVersion || host.Connection.Cache.Hosts.Any(Host.RestrictBatchHotfixApply))
                return;
            
            if (!MinimalPatches.ContainsKey(bgw))
                MinimalPatches.Add(bgw, Updates.GetMinimalPatches(host.Connection));

            var minimalPatches = MinimalPatches[bgw];
            
            if (minimalPatches == null)
                return;

            if (!AllUploadedPatches.ContainsKey(bgw))
                AllUploadedPatches.Add(bgw, new List<XenServerPatch>());
            var uploadedPatches = AllUploadedPatches[bgw];

            var hostActions = GetUpdatePlanActionsForHost(host, hosts, minimalPatches, uploadedPatches, new KeyValuePair<XenServerPatch, string>());
            if (hostActions.UpdatesPlanActions != null && hostActions.UpdatesPlanActions.Count > 0)
            {
                if (hostPlanActions != null)
                {
                    hostPlanActions.UpdatesPlanActions = hostActions.UpdatesPlanActions;
                    hostPlanActions.DelayedActions.InsertRange(0, hostActions.DelayedActions);
                }
            }
        }
        #endregion

        #region Private methods
        private HostPlanActions GetSubTasksFor(Host host)
        {
            var hostPlanActions = new HostPlanActions(host);
            var runningVMs = RunningVMs(host);

            var upgradeAction = ManualModeSelected
                ? new UpgradeManualHostPlanAction(host)
                : new UpgradeHostPlanAction(host, InstallMethodConfig);

            hostPlanActions.InitialPlanActions = new List<PlanAction>()
            {
                new EvacuateHostPlanAction(host),
                upgradeAction
            };

            hostPlanActions.DelayedActions = new List<PlanAction>()
            {
                new BringBabiesBackAction(runningVMs, host, true)
            };
            return hostPlanActions;
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
