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
using System.Runtime.Remoting.Messaging;
using XenAdmin.Alerts;
using HostActionTuple = System.Tuple<XenAPI.Host, System.Collections.Generic.List<XenAdmin.Wizards.PatchingWizard.PlanActions.PlanAction>, System.Collections.Generic.List<XenAdmin.Wizards.PatchingWizard.PlanActions.PlanAction>>;


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_AutomatedUpdatesPage : XenTabPage
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool _thisPageIsCompleted = false;

        public List<Problem> ProblemsResolvedPreCheck { private get; set; }

        public List<Pool> SelectedPools { private get; set; }

        public XenServerPatchAlert UpdateAlert { private get; set; }
        public WizardMode WizardMode { private get; set; }
        public bool ApplyUpdatesToNewVersion { private get; set; }

        private List<PoolPatchMapping> patchMappings = new List<PoolPatchMapping>();
        public Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();
        public KeyValuePair<XenServerPatch, string> PatchFromDisk { private get; set; }

        private List<UpdateProgressBackgroundWorker> backgroundWorkers = new List<UpdateProgressBackgroundWorker>();

        public PatchingWizard_AutomatedUpdatesPage()
        {
            InitializeComponent();
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
                    }

                    planActions.Add(new HostActionTuple(host, planActionsPerHost, delayedActionsPerHost));
                }

                var finalActions = new List<PlanAction>();

                //clean up master at the end
                foreach (var patch in uploadedPatches)
                    finalActions.Add(new RemoveUpdateFileFromMasterPlanAction(master, patchMappings, patch));

                //add a revert pre-check action for this pool
                var problemsToRevert = ProblemsResolvedPreCheck.Where(p => hosts.ToList().Select(h => h.uuid).ToList().Contains(p.Check.Host.uuid)).ToList();
                if (problemsToRevert.Count > 0)
                    finalActions.Add(new UnwindProblemsAction(problemsToRevert, string.Format(Messages.REVERTING_RESOLVED_PRECHECKS_POOL, pool.Connection.Name)));

                if (planActions.Count > 0)
                {
                    atLeastOneWorkerStarted = true;
                    StartNewWorker(planActions, finalActions);
                }
            }

            return atLeastOneWorkerStarted;
        }

        private void StartNewWorker(List<HostActionTuple> planActions, List<PlanAction> finalActions)
        {
            var bgw = new UpdateProgressBackgroundWorker(planActions, finalActions);
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

                        progressBar.Value += e.ProgressPercentage / backgroundWorkers.Count; //extend with error handling related numbers
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
                var sb = new StringBuilder();

                foreach (var pa in bgw.DoneActions)
                {
                    if (pa.Error != null)
                    {
                        sb.Append(pa.ProgressDescription ?? pa.ToString());
                        sb.AppendLine(Messages.ERROR);
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
                bgw.DoneActions.Add(action);
                bgw.InProgressActions.Remove(action);

                log.Error("Failed to carry out plan.", e);
                log.Debug(action.Title);

                doWorkEventArgs.Result = new Exception(action.Title, e);

                //this pool failed, we will stop here, but try to remove update files at least
                try
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
                }
            }
        }

        private void RunPlanAction(UpdateProgressBackgroundWorker bgw, PlanAction action)
        {
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
                Exception exception = e.Result as Exception;
                if (exception != null)
                {
                    //not showing exceptions in the meantime
                }

                //if all finished
                if (backgroundWorkers.All(w => !w.IsBusy))
                {
                    AllWorkersFinished();
                    ShowErrors();

                    _thisPageIsCompleted = true;

                    _cancelEnabled = false;
                    _nextEnabled = true;
                }
            }

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

        #endregion

        private void ShowErrors()
        {
            var sb = new StringBuilder();
            sb.AppendLine();

            int errorCount = 0;

            foreach (var bgw in backgroundWorkers)
            {
                foreach (var action in bgw.DoneActions)
                {
                    var exception = action.Error;
                    if (exception == null)
                    {
                        log.ErrorFormat("An action has failed with an empty exception. Action: {0}", action.ToString());
                        continue;
                    }

                    log.Error(exception);

                    var innerEx = exception.InnerException as Failure;
                    if (innerEx != null)
                    {
                        log.Error(innerEx);
                        sb.AppendLine(innerEx.Message);
                    }
                    else
                    {
                        sb.AppendLine(exception.Message);
                    }
                    errorCount++;
                }
            }

            if (errorCount == 0)
                return;

            labelTitle.Text = Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_FAILED;
            labelError.Text = Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR;

            textBoxLog.Text += "\r\n";
            textBoxLog.Text += errorCount > 1
                ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERRORS_OCCURRED
                : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_OCCURRED;
            textBoxLog.Text += sb.ToString();

            log.ErrorFormat("Error message displayed: {0}", labelError.Text);
            pictureBox1.Image = SystemIcons.Error.ToBitmap();
            panel1.Visible = true;
        }

        private void AllWorkersFinished()
        {
            if (WizardMode == WizardMode.AutomatedUpdates)
            {
                labelTitle.Text = Messages.PATCHINGWIZARD_UPDATES_DONE_AUTOMATED_UPDATES_MODE;
            }
            else if (WizardMode == WizardMode.NewVersion)
            {
                labelTitle.Text = Messages.PATCHINGWIZARD_UPDATES_DONE_AUTOMATED_NEW_VERSION_MODE;
            }

            progressBar.Value = 100;
            pictureBox1.Image = null;
            labelError.Text = Messages.CLOSE_WIZARD_CLICK_FINISH;
        }
    }
}

