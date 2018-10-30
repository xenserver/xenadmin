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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using log4net;
using XenAdmin.Controls;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;
using System.Linq;
using XenAdmin.Core;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Wizards.RollingUpgradeWizard.PlanActions;


namespace XenAdmin.Wizards.PatchingWizard
{
    public enum Status { NotStarted, Started, Cancelled, Completed }
    public abstract partial class AutomatedUpdatesBasePage : XenTabPage
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool _thisPageIsCompleted = false;
        private bool _someWorkersFailed = false;

        public List<Problem> PrecheckProblemsActuallyResolved { private get; set; }
        public List<Pool> SelectedPools { private get; set; }
        public bool ApplyUpdatesToNewVersion { get; set; }
        public Status Status { get; private set; }

        private List<UpdateProgressBackgroundWorker> backgroundWorkers = new List<UpdateProgressBackgroundWorker>();
        private List<UpdateProgressBackgroundWorker> failedWorkers = new List<UpdateProgressBackgroundWorker>();

        private List<PoolPatchMapping> patchMappings = new List<PoolPatchMapping>();
        protected List<string> hostsThatWillRequireReboot = new List<string>();
        public Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();

        public AutomatedUpdatesBasePage()
        {
            InitializeComponent();
            panel1.Visible = false;
        }

        #region XenTabPage overrides
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
                Status = Status.Cancelled;
                backgroundWorkers.ForEach(bgw => bgw.CancelAsync());
            }

            base.PageCancelled();
        }
        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (_thisPageIsCompleted)
                return;

            Status = Status.NotStarted;
            labelTitle.Text = BlurbText();

            if (!StartUpgradeWorkers())
            {
                Status = Status.Completed;
                _thisPageIsCompleted = true;
                _nextEnabled = true;
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
        protected abstract string SuccessMessagePerPool(Pool pool);
        protected abstract string FailureMessagePerPool(bool multipleErrors);
        protected abstract string UserCancellationMessage();

        protected abstract List<HostPlan> GenerateHostPlans(Pool pool, out List<Host> applicableHosts);

        protected virtual bool SkipInitialPlanActions(Host host)
        {
            return false;
        }

        protected virtual void DoAfterInitialPlanActions(UpdateProgressBackgroundWorker bgw, Host host, List<Host> hosts) { }
        #endregion

        #region background workers
        private bool StartUpgradeWorkers()
        {
            //reset the background workers
            backgroundWorkers = new List<UpdateProgressBackgroundWorker>();
            bool atLeastOneWorkerStarted = false;
            
            foreach (var pool in SelectedPools)
            {
                List<Host> applicableHosts;
                var planActions = GenerateHostPlans(pool, out applicableHosts);

                var finalActions = new List<PlanAction>();

                //add a revert pre-check action for this pool
                var curPool = pool;
                var problemsToRevert = PrecheckProblemsActuallyResolved.Where(a =>
                    a.SolutionAction != null && Helpers.GetPoolOfOne(a.SolutionAction.Connection).Equals(curPool)).ToList();

                if (problemsToRevert.Count > 0)
                    finalActions.Add(new UnwindProblemsAction(problemsToRevert, pool.Connection));

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
            backgroundWorkers.Add(bgw);
            bgw.RunWorkerAsync();
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var bgw = sender as UpdateProgressBackgroundWorker;
            if (bgw == null)
                return;

            if (!bgw.CancellationPending)
            {
                PlanAction action = (PlanAction)e.UserState;
                if (action != null)
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
                            if (!failedWorkers.Contains(bgw))
                                failedWorkers.Add(bgw);
                        }
                    }
                }

                UpdateStatus(bgw);
            }
        }

        private void UpdateProgressBar()
        {
            var newVal = backgroundWorkers.Sum(b => b.PercentComplete) / backgroundWorkers.Count;
            if (newVal < 0)
                newVal = 0;
            else if (newVal > 100)
                newVal = 100;
            progressBar.Value = (int)newVal;
        }

        public StringBuilder FindBackgroundWorkerInfo(UpdateProgressBackgroundWorker bgw)
        {
            int bgwErrorCount = 0;
            int bgwCancellationCount = 0;
            var sb = new StringBuilder();
            var errorSb = new StringBuilder();

            if (!string.IsNullOrEmpty(bgw.Name))
                sb.AppendLine(string.Format("{0}:", bgw.Name));

            foreach (var pa in bgw.DoneActions)
            {
                pa.ProgressHistory.ForEach(step => sb.AppendIndented(step).AppendLine());

                if (pa.Error != null)
                {
                    if (pa.Error is CancelledException)
                    {
                        bgwCancellationCount++;
                        continue;
                    }

                    var innerEx = pa.Error.InnerException as Failure;
                    errorSb.AppendLine(innerEx == null ? pa.Error.Message : innerEx.Message);
                    bgwErrorCount++;
                }
            }

            foreach (var pa in bgw.InProgressActions)
            {
                pa.ProgressHistory.ForEach(step => sb.AppendIndented(step).AppendLine());
            }

            sb.AppendLine();

            if (bgwCancellationCount > 0)
            {
                sb.AppendIndented(UserCancellationMessage()).AppendLine();
            }
            else if (bgwErrorCount > 0)
            {
                sb.AppendIndented(FailureMessagePerPool(bgwErrorCount > 1)).AppendLine();
                sb.AppendIndented(errorSb);
            }
            else if (!bgw.IsBusy)
            {
                sb.AppendIndented(SuccessMessagePerPool(bgw.Pool)).AppendLine();
            }

            sb.AppendLine();

            return sb;
        }

        public string FindBackgroundWorkerTitle(UpdateProgressBackgroundWorker bgw)
        {
            int bgwErrorCount = 0;
            int bgwCancellationCount = 0;
            //var sb = new StringBuilder();
            //var errorSb = new StringBuilder();

            //if (!string.IsNullOrEmpty(bgw.Name))
            //    sb.AppendLine(string.Format("{0}:", bgw.Name));

            foreach (var pa in bgw.DoneActions)
            {
                //pa.ProgressHistory.ForEach(step => sb.AppendIndented(step).AppendLine());

                if (pa.Error != null)
                {
                    if (pa.Error is CancelledException)
                    {
                        bgwCancellationCount++;
                        continue;
                    }

                    //var innerEx = pa.Error.InnerException as Failure;
                    //errorSb.AppendLine(innerEx == null ? pa.Error.Message : innerEx.Message);
                    bgwErrorCount++;
                }
            }

            //foreach (var pa in bgw.InProgressActions)
            //{
            //    pa.ProgressHistory.ForEach(step => sb.AppendIndented(step).AppendLine());
            //}

            //sb.AppendLine();

            if (bgwCancellationCount > 0)
            {
                //sb.AppendIndented(UserCancellationMessage()).AppendLine();
                return UserCancellationMessage();
            }
            else if (bgwErrorCount > 0)
            {
                //sb.AppendIndented(FailureMessagePerPool(bgwErrorCount > 1)).AppendLine();
                //sb.AppendIndented(errorSb);
                return FailureMessagePerPool(bgwErrorCount > 1);
            }
            else if (!bgw.IsBusy)
            {
                //sb.AppendIndented(SuccessMessagePerPool(bgw.Pool)).AppendLine();
                return SuccessMessagePerPool(bgw.Pool);
            }

            return BlurbText();

            //sb.AppendLine();

            //return sb;
        }

        public string FindBackgroundWorkerDetails(UpdateProgressBackgroundWorker bgw)
        {
            return FindBackgroundWorkerInfo(bgw).ToString().TrimEnd();
        }

        private void UpdateStatus(UpdateProgressBackgroundWorker bgwToUpdate = null)
        {
            UpdateProgressBar();

            var backgroundWorkersInfo = backgroundWorkers.Select(FindBackgroundWorkerInfo).ToList();

            var allsb = new StringBuilder();
            foreach (var sb in backgroundWorkersInfo)
            {
                allsb.Append(sb);
            }
            textBoxLog.Text = allsb.ToString();
            textBoxLog.SelectionStart = textBoxLog.Text.Length;
            textBoxLog.ScrollToCaret();

            Rebuild();
        }

        private void WorkerDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var bgw = sender as UpdateProgressBackgroundWorker;
            if (bgw == null)
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
                    if (restartHostPlanAction != null)
                    {
                        if (restartHostPlanAction.SkipRestartHost(host))
                        {
                            log.Debug("Did not find patches requiring reboot (live patching succeeded)."
                                      + " Skipping scheduled restart.");
                            hp.DelayedPlanActions.RemoveAll(a => a is RestartHostPlanAction);
                        }
                        else
                        {
                            hp.DelayedPlanActions.RemoveAll(a => a is RestartAgentPlanAction);
                        }
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
                if (action.Error == null)
                    action.Error = new Exception(Messages.ERROR_UNKNOWN);

                log.ErrorFormat("Failed to carry out plan. {0} {1}", action.CurrentProgressStep, e);
                doWorkEventArgs.Result = new Exception(action.CurrentProgressStep, e);
            }
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var bgw = sender as UpdateProgressBackgroundWorker;

            if (e.Cancelled)
            {
                Status = Status.Completed;
                panel1.Visible = true;
                labelError.Text = UserCancellationMessage();
                pictureBox1.Image = Images.StaticImages.cancelled_action_16;
                buttonRetry.Visible = false;
                _thisPageIsCompleted = true;
                _cancelEnabled = false;
                _nextEnabled = true;
            }
            else
            {
                var someWorkersCancelled = false;
                if (bgw != null)
                {
                    var workerSucceeded = true;

                    if (bgw.DoneActions.Any(a => a.Error != null && !(a.Error is CancelledException)))
                    {
                        workerSucceeded = false;
                        _someWorkersFailed = true;
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
                if (backgroundWorkers.All(w => !w.IsBusy))
                {
                    Status = Status.Completed;
                    panel1.Visible = true;
                    if (_someWorkersFailed)
                    {
                        labelError.Text = FailureMessageOnCompletion(backgroundWorkers.Count > 1);
                        pictureBox1.Image = Images.StaticImages._000_error_h32bit_16;
                        buttonRetry.Visible = true;
                    }
                    else if (someWorkersCancelled)
                    {
                        labelError.Text = UserCancellationMessage();
                        pictureBox1.Image = Images.StaticImages.cancelled_action_16;
                        buttonRetry.Visible = false;
                    }
                    else
                    {
                        labelError.Text = SuccessMessageOnCompletion(backgroundWorkers.Count > 1);
                        pictureBox1.Image = Images.StaticImages._000_Tick_h32bit_16;
                        buttonRetry.Visible = false;
                    }

                    _thisPageIsCompleted = true;
                    _cancelEnabled = false;
                    _nextEnabled = true;
                }
            }

            UpdateStatus(bgw);
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

        public void RetryFailedActionsOfAWorker(UpdateProgressBackgroundWorker bgw)
        {
            if (bgw == null)
                return;

            if (!failedWorkers.Remove(bgw))
                return;

            _someWorkersFailed = !failedWorkers.Any();
            panel1.Visible = !failedWorkers.Any();

            bgw.RunWorkerAsync();

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
                if (!uploadedPatches.Contains(patch))
                {
                    planActionsPerHost.Add(new DownloadPatchPlanAction(host.Connection, patch, AllDownloadedPatches, patchFromDisk));
                    planActionsPerHost.Add(new UploadPatchToMasterPlanAction(host.Connection, patch, patchMappings, AllDownloadedPatches, patchFromDisk));
                    uploadedPatches.Add(patch);
                }

                planActionsPerHost.Add(new PatchPrecheckOnHostPlanAction(host.Connection, patch, host, patchMappings, hostsThatWillRequireReboot));
                planActionsPerHost.Add(new ApplyXenServerPatchPlanAction(host, patch, patchMappings));

                var action = GetAfterApplyGuidanceAction(host, patch.after_apply_guidance);
                if (action != null)
                {
                    if (patch.GuidanceMandatory)
                    {
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
                    // add cleanup action for current patch at the end of the update seuence for the last host in the pool
                    var master = Helpers.GetMaster(host.Connection);
                    planActionsPerHost.Add(new RemoveUpdateFileFromMasterPlanAction(master, patchMappings, patch));
                }
            }

            if (repatriateVms)
            {
                var lastRestart = delayedActionsPerHost.FindLast(a => a is RestartHostPlanAction)
                                  ?? planActionsPerHost.FindLast(a => a is RestartHostPlanAction);

                if (lastRestart != null)
                    ((RestartHostPlanAction) lastRestart).EnableOnly = false;
            }

            return new HostPlan(host, null, planActionsPerHost, delayedActionsPerHost);
        }

        private PlanAction GetAfterApplyGuidanceAction(Host host, after_apply_guidance guidance)
        {
            switch (guidance)
            {
                case after_apply_guidance.restartHost:
                    return new RestartHostPlanAction(host, host.GetRunningVMs(), true, hostsThatWillRequireReboot);
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

        private DataGridViewUpdateLocationRow CreateUpdateLocationRow(UpdateProgressBackgroundWorker bgw)
        {
            var row = new DataGridViewUpdateLocationRow(bgw.Pool, this, bgw);
            //row.Visible = !FilterAction(action);
            //row.DismissalRequested += row_DismissalRequested;
            //row.GoToXenObjectRequested += row_GoToXenObjectRequested;
            return row;
        }

        private void ToggleExpandedState(int rowIndex)
        {
            var row = dataGridLog.Rows[rowIndex] as DataGridViewUpdateLocationRow;
            if (row == null)
                return;

            if (row.IsACollapsedRow)
                row.SetCollapseIcon();
            else
                row.SetExpandIcon();
        }

        private void dataGridLog_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // If you click on the headers you can get -1 as the index.
            if (e.ColumnIndex < 0 || e.RowIndex < 0 || e.ColumnIndex != ColumnExpansion.Index)
                return;

            ToggleExpandedState(e.RowIndex);
        }

        private void dataGridLog_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridLog_CellClick(sender, e);
        }

        #region ResizableColumnsForOldNewPanel
        //From: https://www.codeproject.com/Questions/289580/TableLayout-Panel-resize
        //TODO: Use this only to compare the old and new interfaces, remove before considering inclusion in the mainline.

        bool resizing = false;
        
        //TableLayoutRowStyleCollection rowStyles;
        TableLayoutColumnStyleCollection columnStyles;
        /*
        public Form1()
        {
            InitializeComponent();
        }
        */
        private void Form1_Load(object sender, EventArgs e)
        {
            //rowStyles = tableLayoutPanel1.RowStyles;
            columnStyles = tableLayoutPanel1.ColumnStyles;
        }
        
        private void tableLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                resizing = true;
            }
        }

        private void tableLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizing)
            {
                columnStyles[0].SizeType = SizeType.Absolute;
                //rowStyles[0].SizeType = SizeType.Absolute;
                //rowStyles[0].Height = Math.Max(e.Y, 0);
                columnStyles[0].Width = Math.Max(e.X, 0);
            }
        }

        private void tableLayoutPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                resizing = false;
            }
        }

        #endregion

        private List<string> selectedUpdates = new List<string>();
        private List<string> collapsedPoolRowsList = new List<string>();

        private void Rebuild()
        {
            Program.AssertOnEventThread();

            if (!Visible)
                return;

            //TODO: Replace this temporary hack designed to prevent crashes when the workers finish after the form is no longer shown and there are no columns to fit the cells in.
            if (dataGridLog.ColumnCount == 0)
                return;

            //if (checksQueue > 0)
            //    return;

            //SetFilterLabel();

            try
            {
                dataGridLog.SuspendLayout();

                if (dataGridLog.RowCount > 0)
                {
                    StoreStateOfRows();
                    dataGridLog.Rows.Clear();
                    dataGridLog.Refresh();
                }

                var xenConnections = ConnectionsManager.XenConnectionsCopy;
                xenConnections.Sort();

                var rowList = new List<DataGridViewRow>();

                foreach (var bgw in this.backgroundWorkers)
                {
                    var pool = Helpers.GetPool(bgw.Pool.Connection);

                    if (pool != null) // pool row
                        rowList.Add(new DataGridViewUpdateLocationRow(pool, this, bgw));

                    var hosts = bgw.Pool.Connection.Cache.Hosts;
                    Array.Sort(hosts);
                    foreach (var host in hosts) // host row
                        rowList.Add(new DataGridViewUpdateLocationRow(host, pool != null, this, bgw));
                }

                dataGridLog.Rows.AddRange(rowList.ToArray());

                // restore selected state and pool collapsed state
                var checkHostRow = false;
                foreach (DataGridViewUpdateLocationRow row in dataGridLog.Rows)
                {
                    if (checkHostRow)
                    {
                        if (!row.IsPoolOrStandaloneHost)
                        {
                            row.Visible = !row.Visible;
                            continue;
                        }
                        else
                            checkHostRow = false;
                    }

                    var pool = row.UnderlyingPool;
                    if (pool != null)
                    {
                        row.Selected = selectedUpdates.Contains(pool.uuid);
                        if (collapsedPoolRowsList.Contains(pool.uuid))
                        {
                            row.SetExpandIcon();
                            checkHostRow = true;
                        }
                    }
                    else
                    {
                        var host = row.UnderlyingHost;
                        if (host != null)
                            row.Selected = selectedUpdates.Contains(host.uuid);
                    }
                }

                if (dataGridLog.SelectedRows.Count == 0 && dataGridLog.Rows.Count > 0)
                    dataGridLog.Rows[0].Selected = true;
            }
            finally
            {
                dataGridLog.ResumeLayout();
                //UpdateButtonEnablement();
            }
        }

        private void StoreStateOfRows()
        {
            selectedUpdates.Clear();

            collapsedPoolRowsList.Clear();
            foreach (DataGridViewUpdateLocationRow row in dataGridLog.Rows)
            {
                var pool = row.UnderlyingPool;
                if (pool != null)
                {
                    if (row.Selected)
                        selectedUpdates.Add(pool.uuid);
                    if (row.IsACollapsedRow)
                        collapsedPoolRowsList.Add(pool.uuid);
                }
                else
                {
                    var host = row.UnderlyingHost;
                    if (host != null && row.Selected)
                        selectedUpdates.Add(host.uuid);
                }
            }
        }
    }

    public class DataGridViewUpdateLocationRow : CollapsingPoolHostDataGridViewRow
    {
        private DataGridViewImageCell _poolIconCell;
        private DataGridViewTextBoxCell _messageCell;
        private DataGridViewDropDownSplitButtonCell _actionCell;

        private readonly ToolStripMenuItem _retryItem = new ToolStripMenuItem(Messages.RETRY);
        private readonly ToolStripMenuItem _skipItem = new ToolStripMenuItem(Messages.SKIP);

        public DataGridViewUpdateLocationRow(Pool pool, AutomatedUpdatesBasePage Owner, UpdateProgressBackgroundWorker BackgroundWorker)
            : base(pool)
        {
            SetupCells(Owner, BackgroundWorker);
        }

        public DataGridViewUpdateLocationRow(Host host, bool hasPool, AutomatedUpdatesBasePage Owner, UpdateProgressBackgroundWorker BackgroundWorker)
            : base(host, hasPool)
        {
            SetupCells(Owner, BackgroundWorker);
        }

        private void SetupCells(AutomatedUpdatesBasePage Owner, UpdateProgressBackgroundWorker BackgroundWorker)
        {
            this.Owner = Owner;
            this.BackgroundWorker = BackgroundWorker;
            
            _expansionCell = new DataGridViewImageCell();
            _poolIconCell = new DataGridViewImageCell();
            _nameCell = new DataGridViewTextAndImageCell();
            _messageCell = new DataGridViewTextBoxCell();
            _actionCell = new DataGridViewDropDownSplitButtonCell();

            Cells.AddRange(_expansionCell, _poolIconCell, _nameCell, _messageCell, _actionCell);

            _retryItem.Click += ToolStripMenuItemRetry_Click;
            _skipItem.Click += ToolStripMenuItemSkip_Click;

            SetExpandIcon(); //todo: fix NullBitmaps and remove.

            UpdateDetails();
            RefreshSelf();
        }
        
        private void UpdateDetails()
        {
            var pool = UnderlyingPool;
            if (pool != null)
            {
                SetCollapseIcon();
                _poolIconCell.Value = Images.GetImage16For(pool);

                var nc = _nameCell as DataGridViewTextAndImageCell;
                if (nc != null)
                    nc.Image = null;

                _nameCell.Value = pool.Name();
            }
            else
            {
                var host = UnderlyingHost;
                if (host != null)
                {
                    var nc = _nameCell as DataGridViewTextAndImageCell;

                    if (_hasPool && nc != null) // host in pool
                    {
                        nc.Image = Images.GetImage16For(host);
                    }
                    else if (!_hasPool && nc != null) // standalone host
                    {
                        _poolIconCell.Value = Images.GetImage16For(host);
                        nc.Image = null;
                    }

                    _nameCell.Value = host.Name();
                }
            }
        }

        public void RefreshSelf()
        {
            CurrentlyShownMessage = Owner.FindBackgroundWorkerTitle(BackgroundWorker);
            //Location = BackgroundWorker.Name;

            var actions = new List<ToolStripItem>();
            actions.Add(_retryItem);
            actions.Add(_skipItem);
            Actions = actions;
        }

        protected AutomatedUpdatesBasePage Owner
        {
            get;
            private set;
        }

        public UpdateProgressBackgroundWorker BackgroundWorker
        {
            get;
            private set;
        }

        public string CurrentlyShownMessage
        {
            get
            {
                return _messageCell.Value != null ? _messageCell.Value.ToString() : string.Empty;
            }
            private set
            {
                if (value == CurrentlyShownMessage)
                    return;

                _messageCell.Value = value;
            }
        }
        /*
        public string Location
        {
            get
            {
                return _locationCell.Value != null ? _locationCell.Value.ToString() : string.Empty;
            }
            private set
            {
                if (value == Location)
                    return;

                _locationCell.Value = value;
            }
        }
        */
        private List<ToolStripItem> Actions
        {
            get { return _actionCell.ContextMenu.Items.Cast<ToolStripItem>().ToList(); }
            set
            {
                value = value ?? new List<ToolStripItem>();
                if (Actions.SequenceEqual(value))
                    return;
                _actionCell.RefreshItems(value.ToArray());
            }
        }

        public override bool IsCheckable
        {
            get { return IsPoolOrStandaloneHost; }
        }

        private void ToolStripMenuItemRetry_Click(object sender, EventArgs e)
        {
            //Action.Retry();
            Owner.RetryFailedActionsOfAWorker(BackgroundWorker);
        }

        private void ToolStripMenuItemSkip_Click(object sender, EventArgs e)
        {
            //Action.Skip();
        }
    }

    public class DataGridViewUpdateLocationGrid : DataGridViewEx
    {
        //Based on CollapsingPoolHostDataGridView.

        public bool Updating;

        /// <summary>
        /// Sort the rows but then remove and read the rows that should be expandable to be placed back under
        /// the parent row as they may get moved out of place
        /// </summary>
        /// <param name="comparer">Implementation if IComparer used to sort the rows prior to tree rebuild</param>
        protected void SortAndRebuildTree(IComparer comparer)
        {
            /*
            Updating = true;
            try
            {
                if (comparer == null)
                    throw new ArgumentNullException("comparer", "Comparator must not be null");

                Sort(comparer);

                var poolRows = (from DataGridViewUpdateLocationRow r in Rows where r.IsAPoolRow select r).ToList();

                foreach (DataGridViewUpdateLocationRow poolRow in poolRows)
                {
                    //Select the rows that contain hosts represented by the pool row
                    List<Host> hosts = poolRow.UnderlyingPool.Connection.Cache.Hosts.ToList();

                    List<DataGridViewUpdateLocationRow> poolHostRows = (from DataGridViewUpdateLocationRow r in Rows
                                                                            where r.IsAHostRow && hosts.Contains(r.UnderlyingHost)
                                                                            select r).ToList();

                    poolHostRows.ForEach(r => Rows.RemoveAt(r.Index));

                    var rowstoInsert = new Queue<CollapsingPoolHostDataGridViewRow>();

                    foreach (DataGridViewUpdateLocationRow unRow in poolHostRows.Where(row => row.IsAHostRow))
                    {
                        unRow.ToPooledHostRow();
                        rowstoInsert.Enqueue(unRow);
                    }

                    Rows.InsertRange(poolRow.Index + 1, rowstoInsert.ToArray());
                }
            }
            finally
            {
                Updating = false;
            }
            */
        }
    }
}
