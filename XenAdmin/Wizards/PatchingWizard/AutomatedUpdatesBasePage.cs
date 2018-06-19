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

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class AutomatedUpdatesBasePage : XenTabPage
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool _thisPageIsCompleted = false;
        private bool _someWorkersFailed = false;

        public List<Problem> ProblemsResolvedPreCheck { get; set; }

        public List<Pool> SelectedPools { private get; set; }
        
        private List<UpdateProgressBackgroundWorker> backgroundWorkers = new List<UpdateProgressBackgroundWorker>();
        private List<UpdateProgressBackgroundWorker> failedWorkers = new List<UpdateProgressBackgroundWorker>();

        public AutomatedUpdatesBasePage()
        {
            InitializeComponent();
            panel1.Visible = false;
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

        public virtual string BlurbText
        {
            get { return ""; }
        }

        protected virtual void GeneratePlanActions(Pool pool, List<HostPlanActions> planActions, List<PlanAction> finalActions) { }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (_thisPageIsCompleted)
                return;

            labelTitle.Text = BlurbText;

            if (!StartUpgradeWorkers())
            {
                _thisPageIsCompleted = true;
                _nextEnabled = true;
                OnPageUpdated();
            }
        }

        #region background workers

        private bool StartUpgradeWorkers()
        {
            bool atLeastOneWorkerStarted = false;

            foreach (var pool in SelectedPools)
            {
                var planActions = new List<HostPlanActions>();
                var finalActions = new List<PlanAction>();

                GeneratePlanActions(pool, planActions, finalActions);

                if (planActions.Count > 0)
                {
                    atLeastOneWorkerStarted = true;
                    StartNewWorker(pool.Name(), planActions, finalActions);
                }
            }

            return atLeastOneWorkerStarted;
        }

        private void StartNewWorker(string poolName, List<HostPlanActions> planActions, List<PlanAction> finalActions)
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
                    if (!action.IsComplete)
                    {
                        if (!actionsWorker.InProgressActions.Contains(action))
                            actionsWorker.InProgressActions.Add(action);
                    }
                    else
                    {
                        if (!actionsWorker.DoneActions.Contains(action))
                            actionsWorker.DoneActions.Add(action);
                        actionsWorker.InProgressActions.Remove(action);

                        if (action.Error == null)
                        {
                            // remove the successful action from the cleanup actions (we are running the cleanup actions in case of failures or if the user cancelled the process, but we shouldn't re-run the actions that have already been run)
                            actionsWorker.CleanupActions.Remove(action);

                            // only increase the progress if the action succeeded
                            progressBar.Value += e.ProgressPercentage / backgroundWorkers.Count;
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
                    sb.AppendLine(string.Format("{0}:", bgw.Name));

                foreach (var pa in bgw.DoneActions)
                {
                    if (pa.Error != null)
                    {
                        sb.AppendIndented(pa.ProgressDescription ?? pa.ToString());
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
                        sb.AppendIndented(pa.ProgressDescription ?? pa.ToString());
                        sb.AppendLine(Messages.DONE);
                    }
                }

                foreach (var pa in bgw.InProgressActions)
                {
                    if (pa.Visible)
                    {
                        sb.AppendIndented(pa.ProgressDescription ?? pa.ToString());
                        sb.AppendLine();
                    }
                }

                sb.AppendLine();

                if (bgwErrorCount > 0)
                {
                    sb.AppendIndented(bgwErrorCount > 1
                        ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_POOL_MANY
                        : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_POOL_ONE).AppendLine();
                    sb.AppendIndented(errorSb);
                }
                else if (!bgw.IsBusy)
                {
                    sb.AppendIndented(Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_ONE).AppendLine();
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
                foreach (var ha in bgw.HostActions)
                {
                    var hostActions = ha;
                    var host = hostActions.Host;

                    var initialActions = hostActions.InitialPlanActions; // initial actions (e.g. upgrade the host in the RPU case)

                    foreach (var a in initialActions)
                    {
                        action = a;

                        if (bgw.CancellationPending)
                        {
                            doWorkEventArgs.Cancel = true;
                            return;
                        }

                        RunPlanAction(bgw, action);
                    }

                    var planActions = hostActions.UpdatesPlanActions; // priority update actions
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
                    var delayedActions = hostActions.DelayedActions;
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

                if (!bgw.DoneActions.Contains(action))
                    bgw.DoneActions.Add(action);
                bgw.InProgressActions.Remove(action);

                log.Error("Failed to carry out plan.", e);
                log.Debug(action.Title);

                doWorkEventArgs.Result = new Exception(action.Title, e);


                failedWorkers.Add(bgw);
                bgw.ReportProgress(0);
            }
        }

        private void RunPlanAction(UpdateProgressBackgroundWorker bgw, PlanAction action)
        {
            if (bgw.DoneActions.Contains(action) && action.Error == null) // this action was completed successfully, do not run it again
                return;

            // if we retry a failed action, we need to firstly remove it from DoneActions and reset its Error
            bgw.DoneActions.Remove(action);
            action.Error = null;

            action.OnProgressChange += action_OnProgressChange;
            bgw.ReportProgress(0, action);
            action.Run();

            Thread.Sleep(1000);

            action.OnProgressChange -= action_OnProgressChange;
            bgw.ReportProgress(100 / bgw.ActionsCount, action);
        }

        private void action_OnProgressChange(object sender, EventArgs e)
        {
            Program.Invoke(Program.MainWindow, UpdateStatusTextBox);
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                var bgw = sender as UpdateProgressBackgroundWorker;
                if (bgw != null && bgw.DoneActions.Any(a => a.Error != null))
                {
                    _someWorkersFailed = true;
                }
                //if all finished
                if (backgroundWorkers.All(w => !w.IsBusy))
                {
                    panel1.Visible = true;
                    if (_someWorkersFailed)
                    {
                        labelError.Text = backgroundWorkers.Count > 1
                            ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_MANY
                            : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_ERROR_ONE;
                        pictureBox1.Image = Images.StaticImages._000_error_h32bit_16;
                        buttonRetry.Visible = true;
                    }
                    else
                    {
                        labelError.Text = backgroundWorkers.Count > 1
                            ? Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_MANY
                            : Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_SUCCESS_ONE;
                        pictureBox1.Image = Images.StaticImages._000_Tick_h32bit_16;
                        buttonRetry.Visible = false;
                        progressBar.Value = 100;
                    }

                    _thisPageIsCompleted = true;
                    _cancelEnabled = false;
                    _nextEnabled = true;
                }
            }

            UpdateStatusTextBox();
            OnPageUpdated();
        }

        private void RetryFailedActions()
        {
            _someWorkersFailed = false;
            panel1.Visible = false;

            var workers = new List<UpdateProgressBackgroundWorker>(failedWorkers);
            failedWorkers.Clear();

            foreach (var failedWorker in workers)
            {
                failedWorker.RunWorkerAsync();
            }

            _thisPageIsCompleted = false;
            _cancelEnabled = true;
            _nextEnabled = false;
            OnPageUpdated();
        }

        #endregion

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            RetryFailedActions();
        }
    }
}
