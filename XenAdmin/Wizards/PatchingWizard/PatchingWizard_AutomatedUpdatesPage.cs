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

        private bool _cancelEnabled;
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

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (_thisPageIsCompleted)
            {
                return;
            }

            Debug.Assert(WizardMode == WizardMode.AutomatedUpdates || WizardMode == WizardMode.NewVersion && UpdateAlert != null);

            if (WizardMode == WizardMode.AutomatedUpdates)
            {
                labelTitle.Text = Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_AUTOMATED_MODE;
            }
            else if (WizardMode == WizardMode.NewVersion)
            {
                labelTitle.Text = Messages.PATCHINGWIZARD_UPLOAD_AND_INSTALL_TITLE_NEW_VERSION_AUTOMATED_MODE;
            }

            foreach (var pool in SelectedPools)
            {
                var master = Helpers.GetMaster(pool.Connection);

                var planActions = new List<PlanAction>();
                var delayedActionsByHost = new Dictionary<Host, List<PlanAction>>();
                var finalActions = new List<PlanAction>();

                foreach (var host in pool.Connection.Cache.Hosts)
                {
                    delayedActionsByHost.Add(host, new List<PlanAction>());
                }

                var hosts = pool.Connection.Cache.Hosts;

                //if any host is not licensed for automated updates
                bool automatedUpdatesRestricted = pool.Connection.Cache.Hosts.Any(Host.RestrictBatchHotfixApply);

                var us = WizardMode == WizardMode.NewVersion
                    ? Updates.GetUpgradeSequence(pool.Connection, UpdateAlert, ApplyUpdatesToNewVersion && !automatedUpdatesRestricted)
                    : Updates.GetUpgradeSequence(pool.Connection);

                Debug.Assert(us != null, "Update sequence should not be null.");

                if (us != null)
                {
                    foreach (var patch in us.UniquePatches)
                    {
                        var hostsToApply = us.Where(u => u.Value.Contains(patch)).Select(u => u.Key).ToList();
                        hostsToApply.Sort();

                        planActions.Add(new DownloadPatchPlanAction(master.Connection, patch, AllDownloadedPatches, PatchFromDisk));
                        planActions.Add(new UploadPatchToMasterPlanAction(master.Connection, patch, patchMappings, AllDownloadedPatches, PatchFromDisk));
                        planActions.Add(new PatchPrechecksOnMultipleHostsInAPoolPlanAction(master.Connection, patch, hostsToApply, patchMappings));

                        foreach (var host in hostsToApply)
                        {
                            planActions.Add(new ApplyXenServerPatchPlanAction(host, patch, patchMappings));

                            var actions = GetAfterApplyGuidanceActionsForPatch(host, patch);
                            if (patch.GuidanceMandatory)
                            {
                                planActions.AddRange(actions);
                                // remove all delayed actions of the same kind that have already been added
                                // (because these actions are guidance-mandatory=true, therefore
                                // they will run immediately, making delayed ones obsolete)
                                delayedActionsByHost[host].RemoveAll(dg => actions.Any(ma => ma.GetType() == dg.GetType()));
                            }
                            else
                            {
                                // add any action that is not already in the list
                                delayedActionsByHost[host].AddRange(actions.Where(a => !delayedActionsByHost[host].Any(dg => a.GetType() == dg.GetType())));
                            }
                        }

                        //clean up master at the end:
                        planActions.Add(new RemoveUpdateFileFromMasterPlanAction(master, patchMappings, patch));

                    }//patch
                }

                //add a revert pre-check action for this pool
                var problemsToRevert = ProblemsResolvedPreCheck.Where(p => hosts.ToList().Select(h => h.uuid).ToList().Contains(p.Check.Host.uuid)).ToList();
                if (problemsToRevert.Count > 0)
                {
                    finalActions.Add(new UnwindProblemsAction(problemsToRevert, string.Format(Messages.REVERTING_RESOLVED_PRECHECKS_POOL, pool.Connection.Name)));
                }

                if (planActions.Count > 0)
                {
                    var bgw = new UpdateProgressBackgroundWorker(master, planActions, delayedActionsByHost, finalActions);
                    backgroundWorkers.Add(bgw);

                }
            } //foreach in SelectedMasters

            foreach (var bgw in backgroundWorkers)
            {
                bgw.DoWork += WorkerDoWork;
                bgw.WorkerReportsProgress = true;
                bgw.ProgressChanged += WorkerProgressChanged;
                bgw.RunWorkerCompleted += WorkerCompleted;
                bgw.WorkerSupportsCancellation = true;
                bgw.RunWorkerAsync();
            }

            if (backgroundWorkers.Count == 0)
            {
                _thisPageIsCompleted = true;
                _nextEnabled = true;

                OnPageUpdated();
            }
        }

        #region automatic_mode

        private List<PlanAction> doneActions = new List<PlanAction>();
        private List<PlanAction> inProgressActions = new List<PlanAction>();
        private List<PlanAction> errorActions = new List<PlanAction>();

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var actionsWorker = sender as BackgroundWorker;

            Program.Invoke(Program.MainWindow, () =>
                {
                    if (!actionsWorker.CancellationPending)
                    {
                        PlanAction action = (PlanAction)e.UserState;
                        if (action != null)
                        {
                            if (e.ProgressPercentage == 0)
                            {
                                inProgressActions.Add(action);
                            }
                            else
                            {
                                doneActions.Add(action);
                                inProgressActions.Remove(action);

                                progressBar.Value += (int)((float)e.ProgressPercentage / (float)backgroundWorkers.Count); //extend with error handling related numbers
                            }
                        }

                        UpdateStatusTextBox();
                    }
                });
        }

        private void UpdateStatusTextBox()
        {
            var sb = new StringBuilder();

            foreach (var pa in doneActions)
            {
                if (pa.Visible)
                {
                    sb.Append(pa);
                    sb.AppendLine(Messages.DONE);
                }
            }

            foreach (var pa in errorActions)
            {
                sb.Append(pa);
                sb.AppendLine(Messages.ERROR);
            }

            foreach (var pa in inProgressActions)
            {
                if (pa.Visible)
                {
                    sb.Append(pa.ProgressDescription ?? pa.ToString());
                    sb.AppendLine();
                }
            }

            textBoxLog.Text = sb.ToString();

            textBoxLog.SelectionStart = textBoxLog.Text.Length;
            textBoxLog.ScrollToCaret();
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var bgw = sender as UpdateProgressBackgroundWorker;
            PlanAction action = null;

            try
            {
                //running actions (non-delayed)
                foreach (var a in bgw.PlanActions)
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
                // iterating through hosts, master first
                var hostsOrdered = bgw.DelayedActionsByHost.Keys.ToList();
                hostsOrdered.Sort(); //master first

                foreach (var h in hostsOrdered)
                {
                    var actions = bgw.DelayedActionsByHost[h];

                    //run all restart-alike plan actions
                    foreach (var a in actions.Where(a => a.IsRestartRelatedPlanAction()))
                    {
                        action = a;

                        if (bgw.CancellationPending)
                        {
                            doWorkEventArgs.Cancel = true;
                            return;
                        }

                        RunPlanAction(bgw, action);
                    }

                    //run the rest
                    foreach (var a in actions.Where(a => !a.IsRestartRelatedPlanAction()))
                    {
                        action = a;

                        if (bgw.CancellationPending)
                        {
                            doWorkEventArgs.Cancel = true;
                            return;
                        }

                        // any non-restart-alike delayed action needs to be run if:
                        // - this host is pre-Ely and there isn't any restart plan action among the delayed actions, or
                        // - this host is Ely or above and bgw.AvoidRestartHosts contains the host's uuid (shows that live patching must have succeeded) or there isn't any restart plan action among the delayed actions
                        if (!Helpers.ElyOrGreater(h) && !actions.Any(pa => pa.IsRestartRelatedPlanAction())
                            || Helpers.ElyOrGreater(h) && (bgw.AvoidRestartHosts != null && bgw.AvoidRestartHosts.Contains(h.uuid) || !actions.Any(pa => pa.IsRestartRelatedPlanAction())))
                        {
                            RunPlanAction(bgw, action);
                        }
                        else
                        {
                            //skip running it

                            action.Visible = false;
                            bgw.ReportProgress((int)((1.0 / (double)bgw.ActionsCount) * 100), action); //still need to report progress, mainly for the progress bar
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
                bgw.FailedWithExceptionAction = action;
                errorActions.Add(action);
                inProgressActions.Remove(action);

                log.Error("Failed to carry out plan.", e);
                log.Debug(action.Title);

                doWorkEventArgs.Result = new Exception(action.Title, e);

                //this pool failed, we will stop here, but try to remove update files at least
                try
                {
                    var positionOfFailedAction = bgw.PlanActions.IndexOf(action);

                    // can try to clean up the host after a failed PlanAction from bgw.PlanActions only
                    if (positionOfFailedAction != -1 && !(action is DownloadPatchPlanAction || action is UploadPatchToMasterPlanAction))
                    {
                        int pos = positionOfFailedAction;

                        if (!(bgw.PlanActions[pos] is RemoveUpdateFileFromMasterPlanAction)) //can't do anything if the remove action has failed
                        {
                            while (++pos < bgw.PlanActions.Count)
                            {
                                if (bgw.PlanActions[pos] is RemoveUpdateFileFromMasterPlanAction) //find the next remove
                                {
                                    bgw.PlanActions[pos].Run();
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex2)
                {
                    //already in an error case - best effort

                    log.Error("Failed to clean up (this was a best effort attempt)", ex2);
                }

                bgw.ReportProgress(0);
            }

        }

        private void RunPlanAction(UpdateProgressBackgroundWorker bgw, PlanAction action)
        {
            InitializePlanAction(bgw, action);

            action.OnProgressChange += action_OnProgressChange;

            bgw.ReportProgress(0, action);
            action.Run();

            Thread.Sleep(1000);

            action.OnProgressChange -= action_OnProgressChange;
            bgw.doneActions.Add(action);
            bgw.ReportProgress((int)((1.0 / (double)bgw.ActionsCount) * 100), action);
        }

        private void action_OnProgressChange(object sender, EventArgs e)
        {
            Program.Invoke(Program.MainWindow, UpdateStatusTextBox);
        }

        private static void InitializePlanAction(UpdateProgressBackgroundWorker bgw, PlanAction action)
        {
            if (action is IAvoidRestartHostsAware)
            {
                var avoidRestartAction = action as IAvoidRestartHostsAware;
                avoidRestartAction.AvoidRestartHosts = bgw.AvoidRestartHosts;
            }
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var bgw = (UpdateProgressBackgroundWorker)sender;

            Program.Invoke(Program.MainWindow, () =>
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
                });

            OnPageUpdated();
        }

        private static List<PlanAction> GetAfterApplyGuidanceActionsForPatch(Host host, XenServerPatch patch)
        {
            List<PlanAction> actions = new List<PlanAction>();

            if (patch.after_apply_guidance == after_apply_guidance.restartHost)
            {
                actions.Add(new EvacuateHostPlanAction(host));
                actions.Add(new RebootHostPlanAction(host));
                actions.Add(new BringBabiesBackAction(host.GetRunningVMs(), host, false));
            }

            if (patch.after_apply_guidance == after_apply_guidance.restartXAPI)
            {
                actions.Add(new RestartAgentPlanAction(host));
            }

            if (patch.after_apply_guidance == after_apply_guidance.restartHVM)
            {
                actions.Add(new RebootVMsPlanAction(host, host.GetRunningHvmVMs()));
            }

            if (patch.after_apply_guidance == after_apply_guidance.restartPV)
            {
                actions.Add(new RebootVMsPlanAction(host, host.GetRunningPvVMs()));
            }

            return actions;
        }

        #endregion

        private void ShowErrors()
        {
            if (ErrorMessages != null)
            {
                labelTitle.Text = Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_FAILED;
                labelError.Text = Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR;

                textBoxLog.Text += ErrorMessages;

                log.ErrorFormat("Error message displayed: {0}", labelError.Text);
                pictureBox1.Image = SystemIcons.Error.ToBitmap();
                panel1.Visible = true;
            }
        }

        private string ErrorMessages
        {
            get
            {
                if (errorActions.Count == 0)
                    return null;

                var sb = new StringBuilder();

                sb.AppendLine();
                sb.AppendLine(errorActions.Count > 1 ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERRORS_OCCURRED : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_OCCURRED);

                foreach (var action in errorActions)
                {
                    var exception = action.Error;
                    if (exception == null)
                    {
                        log.ErrorFormat("An action has failed with an empty exception. Action: {0}", action.ToString());
                        continue;
                    }

                    log.Error(exception);

                    if (exception != null && exception.InnerException != null && exception.InnerException is Failure)
                    {
                        var innerEx = exception.InnerException as Failure;
                        log.Error(innerEx);

                        sb.AppendLine(innerEx.Message);
                    }
                    else
                    {
                        sb.AppendLine(exception.Message);
                    }
                }

                return sb.ToString();
            }
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

    public static class Extensions
    {
        public static bool IsRestartRelatedPlanAction(this PlanAction a)
        {
            return
                a is EvacuateHostPlanAction || a is RebootHostPlanAction || a is BringBabiesBackAction;
        }
    }
}

