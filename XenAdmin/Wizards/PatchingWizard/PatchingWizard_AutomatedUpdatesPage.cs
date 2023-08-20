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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Alerts;
using XenAdmin.Wizards.PatchingWizard.PlanActions;


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_AutomatedUpdatesPage : AutomatedUpdatesBasePage
    {
        public XenServerPatchAlert UpdateAlert { private get; set; }
        public WizardMode WizardMode { private get; set; }
        public bool IsNewGeneration { get; set; }
        public KeyValuePair<XenServerPatch, string> PatchFromDisk { private get; set; }
        public bool PostUpdateTasksAutomatically { private get; set; }
        public Dictionary<Pool, StringBuilder> ManualTextInstructions { private get; set; }

        public PatchingWizard_AutomatedUpdatesPage()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override string Text => IsNewGeneration
            ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_TEXT_CDN
            : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_TEXT;

        public override string PageTitle => Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_TITLE;

        #endregion

        #region AutomatedUpdatesBesePage overrides

        protected override string BlurbText()
        {
            return string.Format(WizardMode == WizardMode.NewVersion
                    ? Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_NEW_VERSION_AUTOMATED_MODE
                    : Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_AUTOMATED_MODE,
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

            if (IsNewGeneration && !PostUpdateTasksAutomatically && ManualTextInstructions != null && ManualTextInstructions.ContainsKey(pool))
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
                        if (hostUpdateInfo?.UpdateIDs?.Length == 0)
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
            // pre-update tasks and, last in the list, the update itself
            var planActionsPerHost = new List<PlanAction>();
            // post-update tasks
            var delayedActionsPerHost = new List<PlanAction>();

            // hostUpdateInfo.RecommendedGuidance is what's prescribed by the metadata,
            // host.pending_guidances is what's left there from previous updates

            // evacuate host is a pre-update task and needs to be done either the user has
            // opted to carry out the post-update tasks automatically or manually, see CA-381225
            // restart toolstack should run before other post-update tasks, see CA-381718

            if (hostUpdateInfo.RecommendedGuidance.Contains(CdnGuidance.RestartToolstack) ||
                host.pending_guidances.Contains(update_guidances.restart_toolstack))
            {
                if (PostUpdateTasksAutomatically)
                    delayedActionsPerHost.Add(new RestartAgentPlanAction(host));
            }

            if (hostUpdateInfo.RecommendedGuidance.Contains(CdnGuidance.RebootHost) ||
                host.pending_guidances.Contains(update_guidances.reboot_host) ||
                host.pending_guidances.Contains(update_guidances.reboot_host_on_livepatch_failure))
            {
                planActionsPerHost.Add(new EvacuateHostPlanAction(host));

                if (PostUpdateTasksAutomatically)
                    delayedActionsPerHost.Add(new RestartHostPlanAction(host, host.GetRunningVMs()));
            }

            if (hostUpdateInfo.RecommendedGuidance.Contains(CdnGuidance.EvacuateHost) &&
                !planActionsPerHost.Any(a => a is EvacuateHostPlanAction))
            {
                planActionsPerHost.Add(new EvacuateHostPlanAction(host));
            }

            if (PostUpdateTasksAutomatically)
                delayedActionsPerHost.Add(new EnableHostPlanAction(host));

            if (hostUpdateInfo.RecommendedGuidance.Contains(CdnGuidance.RestartDeviceModel) ||
                host.pending_guidances.Contains(update_guidances.restart_device_model))
            {
                if (PostUpdateTasksAutomatically)
                    delayedActionsPerHost.Add(new RebootVMsPlanAction(host, host.GetRunningVMs()));
            }

            planActionsPerHost.Add(new ApplyCdnUpdatesPlanAction(host, poolUpdateInfo));
            delayedActionsPerHost.Add(new CheckForCdnUpdatesPlanAction(host.Connection));

            return new HostPlan(host, null, planActionsPerHost, delayedActionsPerHost);
        }
    }
}
