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

using System.Collections.Generic;
using XenAPI;
using System.Linq;
using System.Text;
using XenAdmin.Core;
using XenAdmin.Alerts;
using XenAdmin.Wizards.PatchingWizard.PlanActions;


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_AutomatedUpdatesPage : AutomatedUpdatesBasePage
    {
        public XenServerPatchAlert UpdateAlert { private get; set; }
        public WizardMode WizardMode { private get; set; }
        public KeyValuePair<XenServerPatch, string> PatchFromDisk { private get; set; }
        public bool PostUpdateTasksAutomatically { private get; set; }
        public Dictionary<Pool, StringBuilder> ManualTextInstructions { private get; set; }

        public PatchingWizard_AutomatedUpdatesPage()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string Text => WizardMode == WizardMode.UpdatesFromCdn
            ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_TEXT_CDN
            : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_TEXT;

        public override string PageTitle => Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_TITLE;

        public override string HelpID => string.Empty;

        #endregion

        #region AutomatedUpdatesBesePage overrides

        protected override string BlurbText()
        {
            return string.Format(WizardMode == WizardMode.AutomatedUpdates || WizardMode == WizardMode.UpdatesFromCdn
                    ? Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_AUTOMATED_MODE
                    : Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_NEW_VERSION_AUTOMATED_MODE,
                BrandManager.BrandConsole);
        }

        protected override string SuccessMessageOnCompletion(bool multiplePools)
        {
            return multiplePools ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_MANY : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_ONE;
        }

        protected override string FailureMessageOnCompletion(bool multiplePools)
        {
            return multiplePools ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_MANY : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_ONE;
        }

        protected override string WarningMessageOnCompletion(bool multiplePools)
        {
            return multiplePools ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_WARNING_MANY : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_WARNING_ONE;
        }

        protected override string SuccessMessagePerPool(Pool pool)
        {
            var sb = new StringBuilder(Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_ONE).AppendLine();

            if (WizardMode == WizardMode.UpdatesFromCdn && !PostUpdateTasksAutomatically && ManualTextInstructions != null && ManualTextInstructions.ContainsKey(pool))
            {
                sb.AppendLine(Messages.PATCHINGWIZARD_SINGLEUPDATE_MANUAL_POST_UPDATE);
                sb.Append(ManualTextInstructions[pool]).AppendLine();
            }

            return sb.ToString();
        }

        protected override string FailureMessagePerPool(bool multipleErrors)
        {
            return multipleErrors ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_POOL_MANY : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_POOL_ONE;
        }

        protected override string WarningMessagePerPool(Pool pool)
        {
            return LivePatchWarningMessagePerPool(pool);
        }

        protected override string UserCancellationMessage()
        {
            return Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_CANCELLATION;
        }

        protected override string ReconsiderCancellationMessage()
        {
            return Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_CANCELLATION_RECONSIDER;
        }

        protected override List<HostPlan> GenerateHostPlans(Pool pool, out List<Host> applicableHosts)
        {
            if (Helpers.CloudOrGreater(pool.Connection))
            {
                applicableHosts = new List<Host>();
                var hostPlans = new List<HostPlan>();

                if (Updates.CdnUpdateInfoPerConnection.TryGetValue(pool.Connection, out var updateInfo))
                {
                    var allHosts = pool.Connection.Cache.Hosts.ToList();
                    allHosts.Sort();

                    foreach (var server in allHosts)
                    {
                        var hostUpdateInfo = updateInfo.HostsWithUpdates.FirstOrDefault(c => c.HostOpaqueRef == server.opaque_ref);
                        if (hostUpdateInfo == null)
                            continue;

                        hostPlans.Add(GetCdnUpdatePlanActionsForHost(server, updateInfo, hostUpdateInfo));
                    }
                }

                return hostPlans;
            }

            bool automatedUpdatesRestricted = pool.Connection.Cache.Hosts.Any(Host.RestrictBatchHotfixApply);

            var minimalPatches = WizardMode == WizardMode.NewVersion
                ? Updates.GetMinimalPatches(UpdateAlert, ApplyUpdatesToNewVersion && !automatedUpdatesRestricted)
                : Updates.GetMinimalPatches(pool.Connection);

            if (minimalPatches == null)
            {
                applicableHosts = new List<Host>();
                return new List<HostPlan>();
            }

            var uploadedPatches = new List<XenServerPatch>();
            var hosts = pool.Connection.Cache.Hosts.ToList();
            hosts.Sort(); //coordinator first

            applicableHosts = new List<Host>(hosts);
            return hosts.Select(h => GetUpdatePlanActionsForHost(h, hosts, minimalPatches, uploadedPatches, PatchFromDisk)).ToList();
        }

        #endregion

        private HostPlan GetCdnUpdatePlanActionsForHost(Host host, CdnPoolUpdateInfo poolUpdateInfo, CdnHostUpdateInfo hostUpdateInfo)
        {
            var planActionsPerHost = new List<PlanAction>();
            var delayedActionsPerHost = new List<PlanAction>();

            if (hostUpdateInfo.RecommendedGuidance.Length > 0)
            {
                bool allLivePatches = true;

                foreach (var id in hostUpdateInfo.UpdateIDs)
                {
                    var update = poolUpdateInfo.Updates.FirstOrDefault(u => u.Id == id);
                    if (update != null && update.LivePatchGuidance == CdnLivePatchGuidance.None)
                    {
                        allLivePatches = false;
                        break;
                    }
                }

                if (hostUpdateInfo.RecommendedGuidance.Contains(CdnGuidance.RebootHost))
                {
                    if (!allLivePatches)
                        planActionsPerHost.Add(new EvacuateHostPlanAction(host));

                    if (PostUpdateTasksAutomatically)
                        delayedActionsPerHost.Add(new RestartHostPlanAction(host, host.GetRunningVMs()));
                }
                else if (hostUpdateInfo.RecommendedGuidance.Contains(CdnGuidance.RestartToolstack))
                {
                    if (!allLivePatches)
                        planActionsPerHost.Add(new EvacuateHostPlanAction(host));
                    
                    if (PostUpdateTasksAutomatically)
                        planActionsPerHost.Add(new RestartAgentPlanAction(host));
                }
                else if (hostUpdateInfo.RecommendedGuidance.Contains(CdnGuidance.EvacuateHost))
                {
                    if (!allLivePatches)
                        planActionsPerHost.Add(new EvacuateHostPlanAction(host));
                }
                else if (hostUpdateInfo.RecommendedGuidance.Contains(CdnGuidance.RestartDeviceModel))
                {
                    if (!allLivePatches)
                        planActionsPerHost.Add(new RebootVMsPlanAction(host, host.GetRunningVMs()));
                }
            }

            planActionsPerHost.Add(new ApplyCdnUpdatesPlanAction(host, poolUpdateInfo));

            return new HostPlan(host, null, planActionsPerHost, delayedActionsPerHost);
        }
    }
}
