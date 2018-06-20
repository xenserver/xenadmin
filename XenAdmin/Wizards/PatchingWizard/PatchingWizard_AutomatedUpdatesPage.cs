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

using System.Collections.Generic;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Alerts;

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_AutomatedUpdatesPage : AutomatedUpdatesBasePage
    {
        public XenServerPatchAlert UpdateAlert { private get; set; }
        public WizardMode WizardMode { private get; set; }
        public bool ApplyUpdatesToNewVersion { private get; set; }

        private List<PoolPatchMapping> patchMappings = new List<PoolPatchMapping>();
        public Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();
        public KeyValuePair<XenServerPatch, string> PatchFromDisk { private get; set; }

        public PatchingWizard_AutomatedUpdatesPage()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides
        public override string Text
        {
            get
            {
                return Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_TEXT;
            }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_TITLE;
            }
        }

        public override string HelpID
        {
            get { return ""; }
        }
        #endregion

        #region AutomatedUpdatesBesePage overrides
        public override string BlurbText()
        {
            return WizardMode == WizardMode.AutomatedUpdates
                ? Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_AUTOMATED_MODE
                : Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_NEW_VERSION_AUTOMATED_MODE;
        }

        protected override void GeneratePlanActions(Pool pool, List<HostPlanActions> planActions, List<PlanAction> finalActions)
        {
            bool automatedUpdatesRestricted = pool.Connection.Cache.Hosts.Any(Host.RestrictBatchHotfixApply);

            var minimalPatches = WizardMode == WizardMode.NewVersion
                ? Updates.GetMinimalPatches(pool.Connection, UpdateAlert, ApplyUpdatesToNewVersion && !automatedUpdatesRestricted)
                : Updates.GetMinimalPatches(pool.Connection);

            if (minimalPatches == null)
                return;

            var uploadedPatches = new List<XenServerPatch>();
            var hosts = pool.Connection.Cache.Hosts.ToList();
            hosts.Sort();//master first

            foreach (var host in hosts)
            {
                var hostActions = GetUpdatePlanActionsForHost(host, hosts, minimalPatches, uploadedPatches);
                if (hostActions.UpdatesPlanActions != null && hostActions.UpdatesPlanActions.Count > 0)
                    planActions.Add(hostActions);
            }

            //add a revert pre-check action for this pool
            var problemsToRevert = ProblemsResolvedPreCheck.Where(p => hosts.ToList().Select(h => h.uuid).ToList().Contains(p.Check.Host.uuid)).ToList();
            if (problemsToRevert.Count > 0)
                finalActions.Add(new UnwindProblemsAction(problemsToRevert, string.Format(Messages.REVERTING_RESOLVED_PRECHECKS_POOL, pool.Connection.Name)));
        }
        #endregion

        private HostPlanActions GetUpdatePlanActionsForHost(Host host, List<Host> hosts, List<XenServerPatch> minimalPatches, List<XenServerPatch> uploadedPatches)
        {
            var hostPlanActions = new HostPlanActions(host);

            var patchSequence = Updates.GetPatchSequenceForHost(host, minimalPatches);
            if (patchSequence == null)
                return hostPlanActions;

            var planActionsPerHost = new List<PlanAction>();
            var delayedActionsPerHost = new List<PlanAction>();

            foreach (var patch in patchSequence)
            {
                if (!uploadedPatches.Contains(patch))
                {
                    planActionsPerHost.Add(new DownloadPatchPlanAction(host.Connection, patch, AllDownloadedPatches, PatchFromDisk));
                    planActionsPerHost.Add(new UploadPatchToMasterPlanAction(host.Connection, patch, patchMappings, AllDownloadedPatches, PatchFromDisk));
                    uploadedPatches.Add(patch);
                }

                planActionsPerHost.Add(new PatchPrecheckOnHostPlanAction(host.Connection, patch, host, patchMappings));
                planActionsPerHost.Add(new ApplyXenServerPatchPlanAction(host, patch, patchMappings));

                if (patch.GuidanceMandatory)
                {
                    var action = patch.after_apply_guidance == after_apply_guidance.restartXAPI && delayedActionsPerHost.Any(a => a is RestartHostPlanAction)
                        ? new RestartHostPlanAction(host, host.GetRunningVMs(), restartAgentFallback: true)
                        : GetAfterApplyGuidanceAction(host, patch.after_apply_guidance);

                    if (action != null)
                    {
                        planActionsPerHost.Add(action);
                        // remove all delayed actions of the same kind that has already been added
                        // (because this action is guidance-mandatory=true, therefore
                        // it will run immediately, making delayed ones obsolete)
                        delayedActionsPerHost.RemoveAll(a => action.GetType() == a.GetType());
                    }
                }
                else
                {
                    var action = GetAfterApplyGuidanceAction(host, patch.after_apply_guidance);
                    // add the action if it's not already in the list
                    if (action != null && delayedActionsPerHost.All(a => a.GetType() != action.GetType()))
                        delayedActionsPerHost.Add(action);
                }

                var isLastHostInPool = hosts.IndexOf(host) == hosts.Count - 1;
                if (isLastHostInPool)
                {
                    // add cleanup action for current patch at the end of the update seuence for the last host in the pool
                    var master = Helpers.GetMaster(host.Connection);
                    planActionsPerHost.Add(new RemoveUpdateFileFromMasterPlanAction(master, patchMappings, patch));
                }
            }

            hostPlanActions.UpdatesPlanActions = planActionsPerHost;
            hostPlanActions.DelayedActions = delayedActionsPerHost;
            return hostPlanActions;
        }

        private static PlanAction GetAfterApplyGuidanceAction(Host host, after_apply_guidance guidance)
        {
            switch (guidance)
            {
                case after_apply_guidance.restartHost:
                    return new RestartHostPlanAction(host, host.GetRunningVMs());
                case after_apply_guidance.restartXAPI:
                    return new RestartAgentPlanAction(host);
                case after_apply_guidance.restartHVM:
                    return new RebootVMsPlanAction(host, host.GetRunningHvmVMs());
                case after_apply_guidance.restartPV:
                    return new RebootVMsPlanAction(host, host.GetRunningPvVMs());
                default:
                    return null;
            }
        }
    }
}
