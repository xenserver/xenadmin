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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using log4net;
using XenAdmin.Controls;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;
using System.Linq;
using XenAdmin.Core;
using System.Text;
using System.Diagnostics;
using XenAdmin.Alerts;
using HostActionTuple = System.Tuple<XenAPI.Host, System.Collections.Generic.List<XenAdmin.Wizards.PatchingWizard.PlanActions.PlanAction>, System.Collections.Generic.List<XenAdmin.Wizards.PatchingWizard.PlanActions.PlanAction>>;


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_AutomatedUpdatesPage : XenTabPage
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool _thisPageIsCompleted = false;
        private bool _someWorkersFailed = false;

        public List<Problem> ProblemsResolvedPreCheck { private get; set; }

        public List<Pool> SelectedPools { private get; set; }

        public XenServerPatchAlert UpdateAlert { private get; set; }
        public WizardMode WizardMode { private get; set; }
        public bool ApplyUpdatesToNewVersion { private get; set; }

        private List<PoolPatchMapping> patchMappings = new List<PoolPatchMapping>();
        public Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();
        public KeyValuePair<XenServerPatch, string> PatchFromDisk { private get; set; }

        private List<UpdateProgressBackgroundWorker> backgroundWorkers = new List<UpdateProgressBackgroundWorker>();
        private List<UpdateProgressBackgroundWorker> failedWorkers = new List<UpdateProgressBackgroundWorker>();

        public PatchingWizard_AutomatedUpdatesPage()
        {
            InitializeComponent();
            panel1.Visible = false;
        }

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

        public override bool EnablePrevious()
        {
            return false;
        }

        private bool _nextEnabled;
        public override bool EnableNext()
        {
            return _nextEnabled;
        }

        private bool _cancelEnabled = true;
        public override bool EnableCancel()
        {
            return _cancelEnabled;
        }

        public override void PageCancelled()
        {
            if (!_thisPageIsCompleted)
            {
                backgroundWorkers.ForEach(bgw => bgw.CancelAsync());
                backgroundWorkers.Clear();
            }

            base.PageCancelled();
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (_thisPageIsCompleted)
                return;

            Debug.Assert(WizardMode == WizardMode.AutomatedUpdates || WizardMode == WizardMode.NewVersion && UpdateAlert != null);

            if (WizardMode == WizardMode.AutomatedUpdates)
                labelTitle.Text = Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_AUTOMATED_MODE;
            else if (WizardMode == WizardMode.NewVersion)
                labelTitle.Text = Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_NEW_VERSION_AUTOMATED_MODE;
            
            ToggleRetryButton();

            if (!StartUpgradeWorkers())
            {
                _thisPageIsCompleted = true;
                _nextEnabled = true;
                OnPageUpdated();
            }
        }

        #region automatic_mode

        private bool StartUpgradeWorkers()
        {
            bool atLeastOneWorkerStarted = false;

            foreach (var pool in SelectedPools)
            {
                //if any host is not licensed for automated updates
                bool automatedUpdatesRestricted = pool.Connection.Cache.Hosts.Any(Host.RestrictBatchHotfixApply);

                var minimalPatches = WizardMode == WizardMode.NewVersion
                    ? Updates.GetMinimalPatches(pool.Connection, UpdateAlert, ApplyUpdatesToNewVersion && !automatedUpdatesRestricted)
                    : Updates.GetMinimalPatches(pool.Connection);

                if (minimalPatches == null)
                    continue;

                var master = Helpers.GetMaster(pool.Connection);
                var planActions = new List<HostActionTuple>();

                var uploadedPatches = new List<XenServerPatch>();
                var hosts = pool.Connection.Cache.Hosts.ToList();
                hosts.Sort();//master first

                foreach (var host in hosts)
                {
                    var patchSequence = Updates.GetPatchSequenceForHost(host, minimalPatches);
                    if (patchSequence == null)
                        continue;

                    var planActionsPerHost = new List<PlanAction>();
                    var delayedActionsPerHost = new List<PlanAction>();

                    foreach (var patch in patchSequence)
                    {
                        if (!uploadedPatches.Contains(patch))
                        {
                            planActionsPerHost.Add(new DownloadPatchPlanAction(master.Connection, patch, AllDownloadedPatches, PatchFromDisk));
                            planActionsPerHost.Add(new UploadPatchToMasterPlanAction(master.Connection, patch, patchMappings, AllDownloadedPatches, PatchFromDisk));
                            uploadedPatches.Add(patch);
                        }

                        planActionsPerHost.Add(new PatchPrecheckOnHostPlanAction(master.Connection, patch, host, patchMappings));
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
                            planActionsPerHost.Add(new RemoveUpdateFileFromMasterPlanAction(master, patchMappings, patch));
                        }
                    }

                    planActions.Add(new HostActionTuple(host, planActionsPerHost, delayedActionsPerHost));
                }

                var finalActions = new List<PlanAction>();
                //add a revert pre-check action for this pool
                var problemsToRevert = ProblemsResolvedPreCheck.Where(p => hosts.ToList().Select(h => h.uuid).ToList().Contains(p.Check.Host.uuid)).ToList();
                if (problemsToRevert.Count > 0)
                    finalActions.Add(new UnwindProblemsAction(problemsToRevert, string.Format(Messages.REVERTING_RESOLVED_PRECHECKS_POOL, pool.Connection.Name)));

                if (planActions.Count > 0)
                {
                    atLeastOneWorkerStarted = true;
                    StartNewWorker(pool.Name(), planActions, finalActions);
                }
            }

            return atLeastOneWorkerStarted;
        }

        private void StartNewWorker(string poolName, List<HostActionTuple> planActions, List<PlanAction> finalActions)
        {
            var bgw = new UpdateProgressBackgroundWorker(planActions, finalActions) { Name = poolName };
            backgroundWorkers.Add(bgw);
            bgw.DoWork += WorkerDoWork;
            bgw.WorkerReportsProgress = true;
            bgw.ProgressChanged += WorkerProgressChanged;
            bgw.RunWorkerCompleted += WorkerCompleted;
            bgw.WorkerSupportsCancellation = true;
            bgw.RunWorkerAsync();
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var actionsWorker = sender as UpdateProgressBackgroundWorker;
            if (actionsWorker == null)
                return;

            if (!actionsWorker.CancellationPending)
            {
                PlanAction action = (PlanAction)e.UserState;
                if (action != null)
                {
                    if (e.ProgressPercentage == 0)
                    {
                        actionsWorker.InProgressActions.Add(action);
                    }
                    else
                    {
                        actionsWorker.DoneActions.Add(action);
                        actionsWorker.InProgressActions.Remove(action);

                        if (action.Error == null)
                        {
                            // remove the successful action from the cleanup actions (we are running the cleanup actions in case of failures or if the user cancelled the process, but we shouldn't re-run the actions that have already been run)
                            actionsWorker.CleanupActions.Remove(action);

                            // only increase the progress if the action succeeded
                            progressBar.Value += e.ProgressPercentage/backgroundWorkers.Count;
                        }
                    }
                }

                UpdateStatusTextBox();
            }
        }

        private void UpdateStatusTextBox()
        {
            var allsb = new StringBuilder();

            foreach (var bgw in backgroundWorkers)
            {
                int bgwErrorCount = 0;
                var sb = new StringBuilder();
                var errorSb = new StringBuilder();

                if (!String.IsNullOrEmpty(bgw.Name))
                    sb.AppendLine(string.Format("{0}:", bgw.Name)).AppendLine();

                foreach (var pa in bgw.DoneActions)
                {
                    if (pa.Error != null)
                    {
                        sb.Append(pa.ProgressDescription ?? pa.ToString());
                        sb.AppendLine(Messages.ERROR);

                        var innerEx = pa.Error.InnerException as Failure;
                        if (innerEx != null)
                        {
                            log.Error(innerEx);
                            errorSb.AppendLine(innerEx.Message);
                        }
                        else
                        {
                            log.Error(pa.Error);
                            errorSb.AppendLine(pa.Error.Message);
                        }

                        bgwErrorCount++;
                    }
                    else if (pa.Visible)
                    {
                        sb.Append(pa.ProgressDescription ?? pa.ToString());
                        sb.AppendLine(Messages.DONE);
                    }
                }

                foreach (var pa in bgw.InProgressActions)
                {
                    if (pa.Visible)
                    {
                        sb.Append(pa.ProgressDescription ?? pa.ToString());
                        sb.AppendLine();
                    }
                }

                sb.AppendLine();

                if (bgwErrorCount > 0)
                {
                    sb.AppendLine(bgwErrorCount > 1
                        ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_POOL_MANY
                        : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_POOL_ONE);

                    sb.Append(errorSb);
                }
                else if (!bgw.IsBusy)
                {
                    sb.AppendLine(Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_ONE);
                }

                sb.AppendLine();
                allsb.Append(sb);
            }

            textBoxLog.Text = allsb.ToString();
            textBoxLog.SelectionStart = textBoxLog.Text.Length;
            textBoxLog.ScrollToCaret();
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var bgw = sender as UpdateProgressBackgroundWorker;
            if (bgw == null)
                return;

            PlanAction action = null;

            try
            {
                foreach (var hostActions in bgw.HostActions)
                {
                    var host = hostActions.Item1;
                    var planActions = hostActions.Item2; // priority actions

                    foreach (var a in planActions)
                    {
                        action = a;

                        if (bgw.CancellationPending)
                        {
                            doWorkEventArgs.Cancel = true;
                            return;
                        }

                        RunPlanAction(bgw, action);
                    }

                    // running delayed actions, but skipping the ones that should be skipped
                    var delayedActions = hostActions.Item3;
                    var restartActions = delayedActions.Where(a => a is RestartHostPlanAction).ToList();

                    foreach (var a in restartActions)
                    {
                        action = a;

                        if (bgw.CancellationPending)
                        {
                            doWorkEventArgs.Cancel = true;
                            return;
                        }

                        RunPlanAction(bgw, action);
                    }

                    var otherActions = delayedActions.Where(a => !(a is RestartHostPlanAction)).ToList();

                    foreach (var a in otherActions)
                    {
                        action = a;

                        if (bgw.CancellationPending)
                        {
                            doWorkEventArgs.Cancel = true;
                            return;
                        }

                        // any non-restart-alike delayed action needs to be run if:
                        // - this host is pre-Ely and there isn't any delayed restart plan action, or
                        // - this host is Ely or above and live patching must have succeeded or there isn't any delayed restart plan action
                        if (restartActions.Count <= 0 ||
                            (Helpers.ElyOrGreater(host) && host.Connection.TryResolveWithTimeout(new XenRef<Host>(host.opaque_ref)).updates_requiring_reboot.Count <= 0))
                        {
                            RunPlanAction(bgw, action);
                        }
                        else
                        {
                            //skip running it, but still need to report progress, mainly for the progress bar

                            action.Visible = false;
                            bgw.ReportProgress(100 / bgw.ActionsCount, action);
                        }
                    }
                }

                //running final actions (eg. revert pre-checks)
                foreach (var a in bgw.FinalActions)
                {
                    action = a;

                    if (bgw.CancellationPending)
                    {
                        doWorkEventArgs.Cancel = true;
                        return;
                    }

                    RunPlanAction(bgw, action);
                }
            }
            catch (Exception e)
            {
                if (action.Error == null)
                    action.Error = new Exception(Messages.ERROR_UNKNOWN);

                bgw.DoneActions.Add(action);
                bgw.InProgressActions.Remove(action);

                log.Error("Failed to carry out plan.", e);
                log.Debug(action.Title);

                doWorkEventArgs.Result = new Exception(action.Title, e);


                failedWorkers.Add(sender as UpdateProgressBackgroundWorker);
                bgw.ReportProgress(0);

                //this pool failed, we will stop here, but try to remove update files at least
                /*try
                {
                    if (action is DownloadPatchPlanAction || action is UploadPatchToMasterPlanAction)
                        return;

                    var pos = 0;
                    if (action is RemoveUpdateFileFromMasterPlanAction)
                        pos = bgw.FinalActions.IndexOf(action) + 1;

                    for (int i = pos; i < bgw.FinalActions.Count; i++)
                    {
                        action = bgw.FinalActions[i];

                        if (bgw.CancellationPending)
                        {
                            doWorkEventArgs.Cancel = true;
                            return;
                        }

                        if (action is RemoveUpdateFileFromMasterPlanAction)
                            RunPlanAction(bgw, action);
                    }
                }
                catch (Exception ex2)
                {
                    //already in an error case - best effort
                    log.Error("Failed to clean up (this was a best effort attempt)", ex2);
                }
                finally
                {
                    bgw.ReportProgress(0);
                }*/
            }
        }

        private void RunPlanAction(UpdateProgressBackgroundWorker bgw, PlanAction action)
        {
            if (bgw.DoneActions.Contains(action) && action.Error == null) // this action was completed successfully, do not run it again
                return;

            // if we retry a failed action, we need to firstly remove it from DoneActions and reset it's Error
            bgw.DoneActions.Remove(action); 
            action.Error = null; 

            action.OnProgressChange += action_OnProgressChange;
            bgw.ReportProgress(0, action);
            action.Run();

            Thread.Sleep(1000);

            action.OnProgressChange -= action_OnProgressChange;
            bgw.ReportProgress(100/bgw.ActionsCount, action);
        }

        private void action_OnProgressChange(object sender, EventArgs e)
        {
            Program.Invoke(Program.MainWindow, UpdateStatusTextBox);
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (!panel1.Visible)
                {
                    var bgw = sender as UpdateProgressBackgroundWorker;
                    if (bgw != null && bgw.DoneActions.Any(a => a.Error != null))
                    {
                        labelError.Text = backgroundWorkers.Count > 1
                            ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_MANY
                            : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_ONE;
                        pictureBox1.Image = Images.StaticImages._000_error_h32bit_16;
                        panel1.Visible = true;

                        _someWorkersFailed = true;
                    }
                }

                //if all finished
                if (backgroundWorkers.All(w => !w.IsBusy))
                {
                    if (!panel1.Visible)
                    {
                        labelError.Text = backgroundWorkers.Count > 1
                            ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_MANY
                            : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_ONE;
                        pictureBox1.Image = Images.StaticImages._000_Tick_h32bit_16;
                        panel1.Visible = true;
                        progressBar.Value = 100;
                    }
                    
                    // show the retry button, if needed
                    ToggleRetryButton();
                    
                    _thisPageIsCompleted = true;
                    _cancelEnabled = false;
                    _nextEnabled = true;
                }
            }

            UpdateStatusTextBox();
            OnPageUpdated();
        }

        private PlanAction GetAfterApplyGuidanceAction(Host host, after_apply_guidance guidance)
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

        private void RetryFailedActions()
        {
            _someWorkersFailed = false;
            panel1.Visible = false;
            ToggleRetryButton();

            var workers = new List<UpdateProgressBackgroundWorker>(failedWorkers);
            failedWorkers.Clear();

            foreach (var failedWorker in workers)
            {
                failedWorker.RunWorkerAsync();
            }
        }

        private void ToggleRetryButton()
        {
            labelRetry.Visible = buttonRetry.Visible = _someWorkersFailed;
        }

        #endregion

        private void retryButton_Click(object sender, EventArgs e)
        {
            RetryFailedActions();
        }
    }
}

