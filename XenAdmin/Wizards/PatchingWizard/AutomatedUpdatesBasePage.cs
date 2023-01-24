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
using System.ComponentModel;
using System.Diagnostics;
using XenAdmin.Controls;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;
using System.Linq;
using XenAdmin.Core;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.RollingUpgradeWizard.PlanActions;


namespace XenAdmin.Wizards.PatchingWizard
{
    public enum Status { NotStarted, Started, Cancelled, Completed }
    public abstract partial class AutomatedUpdatesBasePage : XenTabPage
    {
        private List<UpdateProgressBackgroundWorker> _backgroundWorkers = new List<UpdateProgressBackgroundWorker>();
        private List<UpdateProgressBackgroundWorker> _failedWorkers = new List<UpdateProgressBackgroundWorker>();
        private readonly List<HostUpdateMapping> _patchMappings = new List<HostUpdateMapping>();

        private bool _userMovedVerticalScrollbar;
        private bool _cancelEnabled;

        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);
        protected List<string> HostsThatWillRequireReboot = new List<string>();
        protected Dictionary<string, List<string>> LivePatchAttempts = new Dictionary<string, List<string>>();
        protected bool ThisPageIsCompleted;
        protected bool IsSuccess => ThisPageIsCompleted && !_failedWorkers.Any();

        public Dictionary<XenServerPatch, string> AllDownloadedPatches { get; } = new Dictionary<XenServerPatch, string>();
        public List<Problem> PrecheckProblemsActuallyResolved { private get; set; }
        public List<Pool> SelectedPools { private get; set; }
        public Status Status { get; private set; }

        protected AutomatedUpdatesBasePage()
        {
            InitializeComponent();
            panel1.Visible = false;
        }

        #region XenTabPage overrides
        public override bool EnablePrevious()
        {
            return false;
        }

        public override bool EnableNext()
        {
            return ThisPageIsCompleted;
        }

        public override bool EnableCancel()
        {
            return _cancelEnabled;
        }

        public override void PageCancelled(ref bool cancel)
        {
            if (ThisPageIsCompleted)
                return;

            using (var dialog = new WarningDialog(ReconsiderCancellationMessage(),
                ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
            { WindowTitle = Text })
            {
                if (dialog.ShowDialog(this) != DialogResult.Yes)
                {
                    cancel = true;
                    return;
                }
            }

            Status = Status.Cancelled;
            _backgroundWorkers.ForEach(bgw => bgw.CancelAsync());
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (ThisPageIsCompleted)
                return;

            panel1.Visible = false;
            Status = Status.NotStarted;
            labelTitle.Text = BlurbText();

            if (!StartUpgradeWorkers())
            {
                Status = Status.Completed;
                ThisPageIsCompleted = true;
                UpdateStatusOnCompletion();
                UpdateStatus();
                OnPageUpdated();
            }
            else
            {
                Status = Status.Started;
            }
        }

        #endregion

        #region Virtual members

        protected abstract string BlurbText();
        protected abstract string SuccessMessageOnCompletion(bool multiplePools);
        protected abstract string FailureMessageOnCompletion(bool multiplePools);
        protected abstract string WarningMessageOnCompletion(bool multiplePools);
        protected abstract string SuccessMessagePerPool(Pool pool);
        protected abstract string FailureMessagePerPool(bool multipleErrors);
        protected abstract string WarningMessagePerPool(Pool pool);
        protected abstract string UserCancellationMessage();
        protected abstract string ReconsiderCancellationMessage();

        protected abstract List<HostPlan> GenerateHostPlans(Pool pool, out List<Host> applicableHosts);

        protected virtual bool SkipInitialPlanActions(Host host)
        {
            return false;
        }

        protected virtual void DoAfterInitialPlanActions(UpdateProgressBackgroundWorker bgw, Host host, List<Host> hosts) { }
        #endregion

        #region Backround workers
        private bool StartUpgradeWorkers()
        {
            //reset the background workers
            _backgroundWorkers = new List<UpdateProgressBackgroundWorker>();
            _failedWorkers = new List<UpdateProgressBackgroundWorker>();
            var atLeastOneWorkerStarted = false;

            foreach (var pool in SelectedPools)
            {
                var planActions = GenerateHostPlans(pool, out _);

                var finalActions = new List<PlanAction>();

                if (PrecheckProblemsActuallyResolved != null)
                {
                    //add a revert pre-check action for this pool
                    var curPool = pool;
                    var problemsToRevert = PrecheckProblemsActuallyResolved.Where(a =>
                        a.SolutionAction != null && Helpers.GetPoolOfOne(a.SolutionAction.Connection).Equals(curPool)).ToList();

                    if (problemsToRevert.Count > 0)
                        finalActions.Add(new UnwindProblemsAction(problemsToRevert, pool.Connection));
                }

                if (planActions.Count > 0)
                {
                    atLeastOneWorkerStarted = true;
                    StartNewWorker(pool, planActions, finalActions);
                }
            }

            return atLeastOneWorkerStarted;
        }

        private void StartNewWorker(Pool pool, List<HostPlan> planActions, List<PlanAction> finalActions)
        {
            var bgw = new UpdateProgressBackgroundWorker(pool, planActions, finalActions)
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            bgw.DoWork += WorkerDoWork;
            bgw.ProgressChanged += WorkerProgressChanged;
            bgw.RunWorkerCompleted += WorkerCompleted;
            _backgroundWorkers.Add(bgw);
            bgw.RunWorkerAsync();
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!(sender is UpdateProgressBackgroundWorker bgw))
                return;

            if (bgw.CancellationPending)
                return;

            if (e.UserState is PlanAction action)
            {
                if (!action.IsComplete)
                {
                    if (!bgw.InProgressActions.Contains(action))
                        bgw.InProgressActions.Add(action);
                }
                else
                {
                    if (!bgw.DoneActions.Contains(action))
                        bgw.DoneActions.Add(action);
                    bgw.InProgressActions.Remove(action);

                    if (action.Error == null)
                    {
                        // remove the successful action from the cleanup actions (we are running the cleanup actions in case of failures or if the user cancelled the process, but we shouldn't re-run the actions that have already been run)
                        bgw.CleanupActions.Remove(action);
                    }
                    else
                    {
                        if (!_failedWorkers.Contains(bgw))
                            _failedWorkers.Add(bgw);
                    }
                }
            }

            UpdateStatus();
        }

        private void UpdateStatus()
        {
            var newVal = _backgroundWorkers.Count > 0
                ? _backgroundWorkers.Sum(b => b.PercentComplete) / _backgroundWorkers.Count
                : 100;

            if (newVal < 0)
                newVal = 0;
            else if (newVal > 100)
                newVal = 100;
            progressBar.Value = (int)newVal;

            var stringBuilder = new StringBuilder();

            var addTimestamp = Properties.Settings.Default.ShowTimestampsInUpdatesLog;

            foreach (var bgw in _backgroundWorkers)
            {
                var bgwErrorCount = 0;
                var bgwCancellationCount = 0;
                var sb = new StringBuilder();
                var errorSb = new StringBuilder();

                if (!string.IsNullOrEmpty(bgw.Name))
                    sb.AppendLine($"{bgw.Name}:");

                foreach (var pa in bgw.DoneActions)
                {
                    pa.ProgressHistory.ForEach(step => sb.AppendFormattedLine(value: step.Description, timestamp: step.Timestamp, showTimestamp: addTimestamp ,indent: true));

                    if (pa.Error == null)
                        continue;

                    if (pa.Error is CancelledException)
                    {
                        bgwCancellationCount++;
                        continue;
                    }
                    errorSb.AppendFormattedLine(pa.Error.InnerException is Failure innerEx ? innerEx.Message : pa.Error.Message , timestamp: pa.Timestamp, showTimestamp: addTimestamp, indent: true);

                    if (pa.IsSkippable)
                    {
                        Debug.Assert(!string.IsNullOrEmpty(pa.Title));
                        errorSb.AppendFormattedLine(string.Format(Messages.RPU_WIZARD_ERROR_SKIP_MSG, pa.Title), timestamp : pa.Timestamp, showTimestamp: addTimestamp, indent: true, addExtraLine: true);
                    }

                    bgwErrorCount++;
                }

                foreach (var pa in bgw.InProgressActions)
                {
                    pa.ProgressHistory.ForEach(step => sb.AppendFormattedLine(value: step.Description, timestamp: step.Timestamp, showTimestamp: addTimestamp, indent: true));
                }

                sb.AppendLine();

                if (bgwCancellationCount > 0)
                {
                    sb.AppendFormattedLine(UserCancellationMessage(), timestamp: DateTime.Now, showTimestamp: addTimestamp, indent: true);
                }
                else if (bgwErrorCount > 0)
                {
                    sb.AppendFormattedLine(FailureMessagePerPool(bgwErrorCount > 1), timestamp: DateTime.Now, showTimestamp: addTimestamp, indent: true);

                    // we don't add formatting since errorSb has its own
                    sb.Append(errorSb);
                }
                else if (!bgw.IsBusy)
                {
                    sb.AppendFormattedLine(WarningMessagePerPool(bgw.Pool) ?? SuccessMessagePerPool(bgw.Pool), DateTime.Now, showTimestamp: addTimestamp, indent: true);
                }

                sb.AppendLine();
                stringBuilder.Append(sb);
            }

            var newText = stringBuilder.ToString();

            if (!_userMovedVerticalScrollbar)
            {
                textBoxLog.Text = newText;
                textBoxLog.SelectionStart = textBoxLog.Text.Length;
                textBoxLog.ScrollToCaret();
            }
            else
            {
                textBoxLog.SetTextWithoutScrolling(newText);
            }
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            if (!(sender is UpdateProgressBackgroundWorker bgw))
                return;

            PlanAction action = null;

            try
            {
                foreach (var hp in bgw.HostPlans)
                {
                    var host = hp.Host;

                    // Step 1: InitialPlanActions  (e.g. upgrade the host in the RPU case)
                    bgw.ProgressIncrement = bgw.InitialActionsIncrement(hp);
                    if (!SkipInitialPlanActions(host))
                    {
                        var initialActions = hp.InitialPlanActions;

                        foreach (var a in initialActions)
                        {
                            action = a;
                            bgw.RunPlanAction(action, ref doWorkEventArgs);
                        }
                    }

                    DoAfterInitialPlanActions(bgw, host, bgw.HostPlans.Select(h => h.Host).ToList());

                    // Step 2: UpdatesPlanActions  (priority update action)
                    bgw.ProgressIncrement = bgw.UpdatesActionsIncrement(hp);
                    var planActions = hp.UpdatesPlanActions;
                    foreach (var a in planActions)
                    {
                        action = a;
                        bgw.RunPlanAction(action, ref doWorkEventArgs);
                    }

                    // Step 3: Rearrange DelayedActions
                    var suppPackPlanAction = (RpuUploadAndApplySuppPackPlanAction)planActions.FirstOrDefault(pa => pa is RpuUploadAndApplySuppPackPlanAction);

                    if (suppPackPlanAction != null)
                    {
                        foreach (var dpa in suppPackPlanAction.DelayedPlanActions)
                        {
                            if (!hp.DelayedPlanActions.Exists(a => a.GetType() == dpa.GetType()))
                                hp.DelayedPlanActions.Add(dpa);
                        }
                    }

                    var restartHostPlanAction = (RestartHostPlanAction)hp.DelayedPlanActions.FirstOrDefault(a => a is RestartHostPlanAction);
                    if (restartHostPlanAction != null && !restartHostPlanAction.SkipRestartHost(host))
                    {
                        hp.DelayedPlanActions.RemoveAll(a => a is RestartAgentPlanAction);
                    }

                    // Step 4: DelayedActions
                    bgw.ProgressIncrement = bgw.DelayedActionsIncrement(hp);
                    // running delayed actions
                    var delayedActions = hp.DelayedPlanActions;
                    var restartActions = delayedActions.Where(a => a is RestartHostPlanAction).ToList();

                    foreach (var a in restartActions)
                    {
                        action = a;
                        bgw.RunPlanAction(action, ref doWorkEventArgs);
                    }

                    var otherActions = delayedActions.Where(a => !(a is RestartHostPlanAction)).ToList();

                    foreach (var a in otherActions)
                    {
                        action = a;
                        bgw.RunPlanAction(action, ref doWorkEventArgs);
                    }
                }

                // Step 5: FinalActions (eg. revert pre-checks)
                bgw.ProgressIncrement = bgw.FinalActionsIncrement;
                foreach (var a in bgw.FinalActions)
                {
                    action = a;
                    bgw.RunPlanAction(action, ref doWorkEventArgs);
                }
            }
            catch (Exception e)
            {
                if (action != null)
                {
                    if (action.Error == null)
                        action.Error = new Exception(Messages.ERROR_UNKNOWN);

                    log.Error($"Failed to carry out plan {action.CurrentProgressStep}.", e);
                    doWorkEventArgs.Result = new Exception(action.CurrentProgressStep, e);
                }
            }
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Status = Status.Cancelled;
                panel1.Visible = true;
                labelError.Text = UserCancellationMessage();
                pictureBox1.Image = Images.StaticImages.cancelled_action_16;
                buttonRetry.Visible = buttonSkip.Visible = false;
                ThisPageIsCompleted = true;
                _cancelEnabled = false;
            }
            else
            {
                var someWorkersCancelled = false;
                if (sender is UpdateProgressBackgroundWorker bgw)
                {
                    var workerSucceeded = true;

                    var failedAction = bgw.DoneActions.FirstOrDefault(a => a.Error != null && !(a.Error is CancelledException));
                    if (failedAction != null)
                    {
                        workerSucceeded = false;
                        if (failedAction.IsSkippable)
                        {
                            Debug.Assert(!string.IsNullOrEmpty(failedAction.Title));
                            bgw.FirstFailedSkippableAction = failedAction;
                        }
                    }

                    if (bgw.DoneActions.Any(a => a.Error is CancelledException))
                    {
                        workerSucceeded = false;
                        someWorkersCancelled = true;
                    }

                    if (workerSucceeded)
                        bgw.PercentComplete = 100;
                }

                //if all finished
                if (_backgroundWorkers.All(w => !w.IsBusy))
                    UpdateStatusOnCompletion(someWorkersCancelled);
            }

            UpdateStatus();
            OnPageUpdated();
        }

        private void UpdateStatusOnCompletion(bool someWorkersCancelled = false)
        {
            Status = Status.Completed;
            panel1.Visible = true;

            if (_failedWorkers.Any())
            {
                labelError.Text = FailureMessageOnCompletion(_backgroundWorkers.Count > 1);
                pictureBox1.Image = Images.StaticImages._000_error_h32bit_16;
                buttonRetry.Visible = true;
                buttonSkip.Visible = _failedWorkers.Any(w => w.FirstFailedSkippableAction != null);
            }
            else if (someWorkersCancelled)
            {
                labelError.Text = UserCancellationMessage();
                pictureBox1.Image = Images.StaticImages.cancelled_action_16;
                buttonRetry.Visible = buttonSkip.Visible = false;
            }
            else if (_backgroundWorkers.Any(w => WarningMessagePerPool(w.Pool) != null))
            {
                labelError.Text = WarningMessageOnCompletion(_backgroundWorkers.Count > 1);
                pictureBox1.Image = Images.StaticImages._000_Alert2_h32bit_16;
                buttonRetry.Visible = buttonSkip.Visible = false;
            }
            else
            {
                labelError.Text = SuccessMessageOnCompletion(_backgroundWorkers.Count > 1);
                pictureBox1.Image = Images.StaticImages._000_Tick_h32bit_16;
                buttonRetry.Visible = buttonSkip.Visible = false;
            }

            ThisPageIsCompleted = true;
            _cancelEnabled = false;
        }

        #endregion

        private void RetryFailedActions()
        {
            panel1.Visible = false;
            _failedWorkers.ForEach(bgw => bgw.FirstFailedSkippableAction = null);

            var workers = new List<UpdateProgressBackgroundWorker>(_failedWorkers);
            _failedWorkers.Clear();

            foreach (var failedWorker in workers)
            {
                failedWorker.RunWorkerAsync();
            }

            ThisPageIsCompleted = false;
            _cancelEnabled = true;
            OnPageUpdated();
        }

        private void SkipFailedActions()
        {
            var skippableWorkers = _failedWorkers.Where(w => w.FirstFailedSkippableAction != null).ToList();
            var msg = string.Join(Environment.NewLine, skippableWorkers.Select(w => w.FirstFailedSkippableAction.Title));

            using (var dlg = new WarningDialog(string.Format(skippableWorkers.Count > 1 ? Messages.MESSAGEBOX_SKIP_RPU_STEPS : Messages.MESSAGEBOX_SKIP_RPU_STEP, msg),
                    ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
            { WindowTitle = ParentForm != null ? ParentForm.Text : BrandManager.BrandConsole })
            {
                if (dlg.ShowDialog(this) != DialogResult.Yes)
                    return;
            }

            panel1.Visible = false;

            foreach (var worker in skippableWorkers)
            {
                _failedWorkers.Remove(worker);
                worker.RunWorkerAsync();
            }

            ThisPageIsCompleted = false;
            _cancelEnabled = true;
            OnPageUpdated();
        }

        #region Event handlers
        private void buttonRetry_Click(object sender, EventArgs e)
        {
            RetryFailedActions();
        }

        private void buttonSkip_Click(object sender, EventArgs e)
        {
            SkipFailedActions();
        }

        private void TextBoxLog_OnScrollChange(int _, Orientation orientation)
        {
            if (orientation == Orientation.Vertical)
            {
                // if the user scrolls all the way to the bottom, we reset the status
                _userMovedVerticalScrollbar = !textBoxLog.IsVerticalScrollBarAtBottom;
            }
        }
        #endregion

        protected HostPlan GetUpdatePlanActionsForHost(Host host, List<Host> hosts, List<XenServerPatch> minimalPatches,
            List<XenServerPatch> uploadedPatches, KeyValuePair<XenServerPatch, string> patchFromDisk, bool repatriateVms = true)
        {
            var patchSequence = Updates.GetPatchSequenceForHost(host, minimalPatches);
            if (patchSequence == null)
                return new HostPlan(host, null, null, null);

            var planActionsPerHost = new List<PlanAction>();
            var delayedActionsPerHost = new List<PlanAction>();

            foreach (var patch in patchSequence)
            {
                // if the patchSequence contains a patch that requires a host reboot (excluding livepatches), then add the Evacuate action as the first action in the sequence
                if (patch.after_apply_guidance == after_apply_guidance.restartHost
                    && !patch.ContainsLivepatch
                    && (planActionsPerHost.Count == 0 || !(planActionsPerHost[0] is EvacuateHostPlanAction)))
                {
                    planActionsPerHost.Insert(0, new EvacuateHostPlanAction(host));
                }

                if (!uploadedPatches.Contains(patch))
                {
                    planActionsPerHost.Add(new DownloadPatchPlanAction(host.Connection, patch, AllDownloadedPatches, patchFromDisk));
                    planActionsPerHost.Add(new UploadPatchToCoordinatorPlanAction(this, host.Connection, patch, _patchMappings, AllDownloadedPatches, patchFromDisk, true));
                    uploadedPatches.Add(patch);
                }

                planActionsPerHost.Add(new PatchPrecheckOnHostPlanAction(host.Connection, patch, host, _patchMappings, HostsThatWillRequireReboot, LivePatchAttempts));
                planActionsPerHost.Add(new ApplyXenServerPatchPlanAction(host, patch, _patchMappings));

                var action = GetAfterApplyGuidanceAction(host, patch.after_apply_guidance);
                if (action != null)
                {
                    if (patch.GuidanceMandatory)
                    {
                        // if this update requires a mandatory toolstack restart and there is a pending host reboot in the delayed actions,
                        // then the pending reboot should be carried out instead
                        if (patch.after_apply_guidance == after_apply_guidance.restartXAPI && delayedActionsPerHost.Any(a => a is RestartHostPlanAction))
                        {
                            // replace the action with a host reboot action which will fall back to a toolstack restart if the reboot is not needed because the live patching succedeed
                            action = new RestartHostPlanAction(host, host.GetRunningVMs(), true, true, HostsThatWillRequireReboot);
                        }

                        planActionsPerHost.Add(action);
                        // remove all delayed actions of the same kind that has already been added
                        // (because this action is guidance-mandatory=true, therefore
                        // it will run immediately, making delayed ones obsolete)
                        delayedActionsPerHost.RemoveAll(a => action.GetType() == a.GetType());
                    }
                    else
                    {
                        // add the action if it's not already in the list
                        if (delayedActionsPerHost.All(a => a.GetType() != action.GetType()))
                            delayedActionsPerHost.Add(action);
                    }
                }

                var isLastHostInPool = hosts.IndexOf(host) == hosts.Count - 1;
                if (isLastHostInPool)
                {
                    // add cleanup action for current patch at the end of the update sequence for the last host in the pool
                    var coordinator = Helpers.GetCoordinator(host.Connection);
                    planActionsPerHost.Add(new RemoveUpdateFileFromCoordinatorPlanAction(coordinator, _patchMappings, patch));
                }
            }

            if (repatriateVms)
            {
                var lastRestart = delayedActionsPerHost.FindLast(a => a is RestartHostPlanAction)
                                  ?? planActionsPerHost.FindLast(a => a is RestartHostPlanAction);

                if (lastRestart != null)
                    ((RestartHostPlanAction)lastRestart).EnableOnly = false;
            }

            return new HostPlan(host, null, planActionsPerHost, delayedActionsPerHost);
        }

        private PlanAction GetAfterApplyGuidanceAction(Host host, after_apply_guidance guidance)
        {
            switch (guidance)
            {
                case after_apply_guidance.restartHost:
                    return new RestartHostPlanAction(host, host.GetRunningVMs(), true, false, HostsThatWillRequireReboot);
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

        protected string LivePatchWarningMessagePerPool(Pool pool)
        {
            var sb = new StringBuilder();

            var poolHosts = pool.Connection.Cache.Hosts.ToList();

            var livePatchingFailedHosts = new List<Host>();
            foreach (var host in poolHosts)
            {
                if (LivePatchAttempts.ContainsKey(host.uuid) && host.updates_requiring_reboot != null && host.updates_requiring_reboot.Count > 0)
                {
                    foreach (var updateUuid in LivePatchAttempts[host.uuid])
                    {
                        if (host.updates_requiring_reboot.Select(uRef => host.Connection.Resolve(uRef)).Any(u => u != null && u.uuid.Equals(updateUuid)))
                        {
                            livePatchingFailedHosts.Add(host);
                            break;
                        }
                    }
                }
            }

            if (livePatchingFailedHosts.Count == 1)
            {
                sb.AppendFormat(Messages.LIVE_PATCHING_FAILED_ONE_HOST, livePatchingFailedHosts[0].Name()).AppendLine();
                return sb.ToString();
            }

            if (livePatchingFailedHosts.Count > 1)
            {
                var hostnames = string.Join(", ", livePatchingFailedHosts.Select(h => string.Format("'{0}'", h.Name())));
                sb.AppendFormat(Messages.LIVE_PATCHING_FAILED_MULTI_HOST, hostnames).AppendLine();
                return sb.ToString();
            }

            return null;
        }
    }
}
