﻿/* Copyright (c) Citrix Systems, Inc. 
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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Alerts;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_PrecheckPage : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly object _lock = new object();
        private readonly object _update_grid_lock = new object();
        private BackgroundWorker _worker;
        public List<Host> SelectedServers = new List<Host>();
        public List<Problem> ProblemsResolvedPreCheck = new List<Problem>();
        private AsyncAction resolvePrechecksAction;

        protected List<Pool> SelectedPools
        {
            get 
            { 
                return SelectedServers.Select(host => Helpers.GetPoolOfOne(host.Connection)).Where(pool => pool != null).Distinct().ToList();
            }
        }

        public XenServerPatchAlert UpdateAlert { private get; set; }
        public WizardMode WizardMode { private get; set; }
        public bool ApplyUpdatesToNewVersion { private get; set; }

        public PatchingWizard_PrecheckPage()
        {
            InitializeComponent();
            dataGridView1.BackgroundColor = dataGridView1.DefaultCellStyle.BackColor;
        }

        public override string PageTitle
        {
            get
            {
                return Messages.PATCHINGWIZARD_PRECHECKPAGE_TITLE;
            }
        }

        public override string Text
        {
            get
            {
                return Messages.PATCHINGWIZARD_PRECHECKPAGE_TEXT;
            }
        }

        public override string HelpID
        {
            get { return "UpdatePrechecks"; }
        }

        void Connection_ConnectionStateChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, RefreshRechecks);
        }

        private void RegisterEventHandlers()
        {
            foreach (Host selectedServer in SelectedServers)
            {
                selectedServer.Connection.ConnectionStateChanged += Connection_ConnectionStateChanged;
            }            
        }

        private void DeregisterEventHandlers()
        {
            foreach (Host selectedServer in SelectedServers)
            {
                selectedServer.Connection.ConnectionStateChanged -= Connection_ConnectionStateChanged;
            }
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            try
            {
                RegisterEventHandlers();
                if (direction == PageLoadedDirection.Back)
                    return;

                if (WizardMode == WizardMode.AutomatedUpdates)
                {
                    labelPrechecksFirstLine.Text = Messages.PATCHINGWIZARD_PRECHECKPAGE_FIRSTLINE_AUTOMATED_UPDATES_MODE;
                }
                else
                {
                    string patchName = null;
                    if (Patch != null)
                        patchName = Patch.Name();
                    if (PoolUpdate != null)
                        patchName = PoolUpdate.Name();

                    labelPrechecksFirstLine.Text = patchName != null
                        ? string.Format(Messages.PATCHINGWIZARD_PRECHECKPAGE_FIRSTLINE, patchName)
                        : Messages.PATCHINGWIZARD_PRECHECKPAGE_FIRSTLINE_NO_PATCH_NAME;
                }

                RefreshRechecks();
            }
            catch (Exception e)
            {
                log.Error(e, e);
                throw;//better throw an exception rather than closing the wizard suddenly and silently
            }
        }

        public override void SelectDefaultControl()
        {
            dataGridView1.Select();
        }

        protected void RefreshRechecks()
        {
            _worker = null;
            _worker = new BackgroundWorker();
            _worker.DoWork += worker_DoWork;
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.ProgressChanged += worker_ProgressChanged;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            
            if (Patch != null)
                _worker.RunWorkerAsync(Patch);
            else
                _worker.RunWorkerAsync(PoolUpdate);
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                progressBar1.Value = 100;
                labelProgress.Text = string.Empty;
                OnPageUpdated();
            }
        }

        private void AddRowToGridView(DataGridViewRow row)
        {
            lock (_update_grid_lock)
            {
                dataGridView1.Rows.Add(row);
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                PreCheckHostRow rowHost = e.UserState as PreCheckHostRow;
                if (rowHost != null)
                {
                    if (checkBoxViewPrecheckFailuresOnly.Checked && rowHost.Problem != null || !checkBoxViewPrecheckFailuresOnly.Checked)
                        AddRowToGridView(rowHost);
                }
                else
                {
                    var row = e.UserState as DataGridViewRow;
                    if (row != null && !dataGridView1.Rows.Contains(row))
                    {
                        AddRowToGridView(row);
                    }
                }

                progressBar1.Value = e.ProgressPercentage;
            }
            catch (Exception) { }
        }

        private bool IsCheckInProgress
        {
            get { return _worker != null && _worker.IsBusy; }
        }

        private bool IsResolveActionInProgress
        {
            get { return resolvePrechecksAction != null && !resolvePrechecksAction.IsCompleted; }
        }

        private List<PreCheckHostRow> ExecuteCheck(Check check)
        {
            var rows = new List<PreCheckHostRow>();

            var problems = check.RunAllChecks();
            if (problems.Count == 0)
            {
                rows.Add(new PreCheckHostRow(check));
                return rows;
            }

            foreach (var pr in problems)
            {
                var problem = pr;  // we need this line because we sometimes reassign it below
                if (problem is HostNotLive)
                {
                    // this host is no longer live -> remove all previous problems regarding this host
                    Problem curProblem = problem;
                    ProblemsResolvedPreCheck.RemoveAll(p => p.Check.Host == curProblem.Check.Host);
                }

                if (ProblemsResolvedPreCheck.Contains(problem))
                {
                    Problem curProblem = problem;
                    problem = ProblemsResolvedPreCheck.Find(p => p.Equals(curProblem));
                }
                else
                    ProblemsResolvedPreCheck.Add(problem);

                rows.Add(new PreCheckHostRow(problem));
            }
            return rows;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (_lock)
            {
                _worker.ReportProgress(0, null);
                Program.Invoke(this, () =>
                                         {
                                             dataGridView1.Rows.Clear();
                                             labelProgress.Text = Messages.PATCHING_WIZARD_RUNNING_PRECHECKS;
                                             OnPageUpdated();
                                         });
                Pool_patch patch = e.Argument as Pool_patch;
                Pool_update update = e.Argument as Pool_update;

                LivePatchCodesByHost = new Dictionary<string, livepatch_status>();

                var groups = update != null ? GenerateChecks(update) : GenerateChecks(patch); //patch is expected to be null for RPU

                int totalChecks = groups.Values.Sum(c => c.Count);
                int doneCheckIndex = 0;

                foreach (var group in groups)
                {
                    if (_worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var headerRow = new PreCheckHeaderRow(string.Format(Messages.PATCHING_WIZARD_PRECHECK_STATUS, group.Key));
                    //multiply with 100 first, otherwise the quotient is 0
                    _worker.ReportProgress(doneCheckIndex*100/totalChecks, headerRow);

                    PreCheckResult precheckResult = PreCheckResult.OK;
                    var checks = group.Value;

                    foreach (var check in checks)
                    {
                        if (_worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        var rows = ExecuteCheck(check);
                        doneCheckIndex++;

                        foreach (PreCheckHostRow row in rows)
                        {
                            if (precheckResult != PreCheckResult.Failed && row.Problem != null)
                                precheckResult = row.PrecheckResult;

                            //multiply with 100 first, otherwise the quotient is 0
                            _worker.ReportProgress(doneCheckIndex*100/totalChecks, row);
                        }
                    }

                    lock (_update_grid_lock)
                    {
                        headerRow.UpdateDescription(precheckResult);
                    }
                }
            }
        }

        public Dictionary<string, livepatch_status> LivePatchCodesByHost
        {
            get;
            set;
        }

        protected virtual Dictionary<string, List<Check>> GenerateCommonChecks(List<Host> applicableServers)
        {
            var groups = new Dictionary<string, List<Check>>();

            //XenCenter version check
            if (UpdateAlert != null && UpdateAlert.NewServerVersion != null)
                groups[Messages.CHECKING_XENCENTER_VERSION] = new List<Check> { new XenCenterVersionCheck(UpdateAlert.NewServerVersion) };

            //HostLivenessCheck checks
            var livenessChecks = new List<Check>();
            foreach (Host host in applicableServers)
                livenessChecks.Add(new HostLivenessCheck(host));

            groups[Messages.CHECKING_HOST_LIVENESS_STATUS] = livenessChecks;

            //HA checks

            var haChecks = new List<Check>();
            foreach (Host host in SelectedServers)
            {
                if (Helpers.HostIsMaster(host))
                    haChecks.Add(new HAOffCheck(host));
            }
            groups[Messages.CHECKING_HA_STATUS] = haChecks;

            //PBDsPluggedCheck
            var pbdChecks = new List<Check>();
            foreach (Host host in applicableServers)
                pbdChecks.Add(new PBDsPluggedCheck(host));

            groups[Messages.CHECKING_STORAGE_CONNECTIONS_STATUS] = pbdChecks;


            //Disk space check for automated updates
            if (WizardMode != WizardMode.SingleUpdate)
            {
                var diskChecks = new List<Check>();

                foreach (Pool pool in SelectedPools)
                {
                    //if any host is not licensed for automated updates
                    bool automatedUpdatesRestricted = pool.Connection.Cache.Hosts.Any(Host.RestrictBatchHotfixApply);

                    var us = WizardMode == WizardMode.NewVersion
                        ? Updates.GetUpgradeSequence(pool.Connection, UpdateAlert, ApplyUpdatesToNewVersion && !automatedUpdatesRestricted)
                        : Updates.GetUpgradeSequence(pool.Connection);

                    log.InfoFormat("Minimal patches for {0}: {1}", pool.Name(), us != null && us.MinimalPatches != null ? string.Join(",", us.MinimalPatches.Select(p => p.Name)) : "");

                    if (us == null)
                        continue;

                    bool elyOrGreater = Helpers.ElyOrGreater(pool.Connection);

                    foreach (Host host in us.Keys)
                    {
                        diskChecks.Add(
                            new DiskSpaceForAutomatedUpdatesCheck(
                                host,
                                elyOrGreater
                                    ? us[host].Sum(p => p.InstallationSize) // all updates on this host
                                    : host.IsMaster()
                                        ? us[host].Sum(p => p.InstallationSize) + us.Values.SelectMany(a => a).Max(p => p.InstallationSize) // master: all updates on master + largest update in pool
                                        : us[host].Sum(p => p.InstallationSize) + us[host].Max(p => p.InstallationSize) // non-master: all updates on this host + largest on this host
                        ));
                    }
                }
                groups[Messages.PATCHINGWIZARD_PRECHECKPAGE_CHECKING_DISK_SPACE] = diskChecks;
            }

            //Checking reboot required and can evacuate host for version updates
            if (WizardMode == WizardMode.NewVersion && UpdateAlert != null && UpdateAlert.Patch != null &&  UpdateAlert.Patch.after_apply_guidance == after_apply_guidance.restartHost)
            {
                var guidance = new List<after_apply_guidance> {UpdateAlert.Patch.after_apply_guidance};

                var rebootChecks = new List<Check>();
                foreach (var host in SelectedServers)
                    rebootChecks.Add(new HostNeedsRebootCheck(host, guidance, LivePatchCodesByHost));
                groups[Messages.CHECKING_SERVER_NEEDS_REBOOT] = rebootChecks;

                var evacuateChecks = new List<Check>();
                foreach (Host host in SelectedServers)
                    evacuateChecks.Add(new AssertCanEvacuateCheck(host, LivePatchCodesByHost));

                groups[Messages.CHECKING_CANEVACUATE_STATUS] = evacuateChecks;
            }

            return groups;
        }

        protected virtual Dictionary<string, List<Check>> GenerateChecks(Pool_patch patch)
        {
            List<Host> applicableServers = patch != null ? SelectedServers.Where(h => patch.AppliedOn(h) == DateTime.MaxValue).ToList() : SelectedServers;

            var groups = GenerateCommonChecks(applicableServers);

            //Checking other things
            if (patch != null)
            {
                var serverChecks = new List<Check>();
                foreach (Host host in SelectedServers)
                {
                    List<Pool_patch> poolPatches = new List<Pool_patch>(host.Connection.Cache.Pool_patches);
                    Pool_patch poolPatchFromHost = poolPatches.Find(otherPatch => string.Equals(otherPatch.uuid, patch.uuid, StringComparison.OrdinalIgnoreCase));
                    serverChecks.Add(new PatchPrecheckCheck(host, poolPatchFromHost, LivePatchCodesByHost));
                }
                groups[Messages.CHECKING_SERVER_SIDE_STATUS] = serverChecks;
            }

            //Checking if the host needs a reboot
            if (WizardMode == WizardMode.SingleUpdate)
            {
                var rebootChecks = new List<Check>();
                var guidance = patch != null
                    ? patch.after_apply_guidance
                    : new List<after_apply_guidance> {after_apply_guidance.restartHost};

                foreach (var host in applicableServers)
                    rebootChecks.Add(new HostNeedsRebootCheck(host, guidance, LivePatchCodesByHost));

                groups[Messages.CHECKING_SERVER_NEEDS_REBOOT] = rebootChecks;
            }

            //Checking can evacuate host
            //CA-97061 - evacuate host -> suspended VMs. This is only needed for restartHost
            //Also include this check for the supplemental packs (patch == null), as their guidance is restartHost
            if (WizardMode != WizardMode.NewVersion && (patch == null || patch.after_apply_guidance.Contains(after_apply_guidance.restartHost)))
            {
                var evacuateChecks = new List<Check>();
                foreach (Host host in applicableServers)
                    evacuateChecks.Add(new AssertCanEvacuateCheck(host, LivePatchCodesByHost));

                groups[Messages.CHECKING_CANEVACUATE_STATUS] = evacuateChecks;
            }

            if (patch != null || WizardMode == WizardMode.AutomatedUpdates)
            {
                var restartChecks = new List<Check>();
                foreach (var pool in SelectedPools)
                    restartChecks.Add(new RestartHostOrToolstackPendingOnMasterCheck(pool, WizardMode == WizardMode.AutomatedUpdates ? null : patch.uuid));

                groups[Messages.CHECKING_FOR_PENDING_RESTART] = restartChecks;
            }

            return groups;
        }

        protected virtual Dictionary<string, List<Check>> GenerateChecks(Pool_update update)
        {
            List<Host> applicableServers = update != null ? SelectedServers.Where(h => !update.AppliedOn(h)).ToList() : SelectedServers;
            var groups = GenerateCommonChecks(applicableServers);

            //Update Homogeneity check for InvernessOrGreater
            if (update != null)
            {
                var homogeneityChecks = new List<Check>();
                foreach (var pool in SelectedPools.Where(pool => Helpers.InvernessOrGreater(pool.Connection)))
                    homogeneityChecks.Add(new ServerSelectionCheck(pool, update, SelectedServers));

                if (homogeneityChecks.Count > 0)
                    groups[Messages.CHECKING_SERVER_SELECTION] = homogeneityChecks;
            }

            //Checking other things
            if (update != null)
            {
                var serverChecks = new List<Check>();
                foreach (Host host in SelectedServers)
                {
                    List<Pool_update> updates = new List<Pool_update>(host.Connection.Cache.Pool_updates);
                    Pool_update poolUpdateFromHost = updates.Find(otherPatch => string.Equals(otherPatch.uuid, update.uuid, StringComparison.OrdinalIgnoreCase));
                    serverChecks.Add(new PatchPrecheckCheck(host, poolUpdateFromHost, LivePatchCodesByHost));
                }
                groups[Messages.CHECKING_SERVER_SIDE_STATUS] = serverChecks;
            }

            //Checking if the host needs a reboot
            if (WizardMode == WizardMode.SingleUpdate)
            {
                var rebootChecks = new List<Check>();
                var guidance = update != null
                    ? update.after_apply_guidance
                    : new List<update_after_apply_guidance> {update_after_apply_guidance.restartHost};
                foreach (var host in applicableServers)
                    rebootChecks.Add(new HostNeedsRebootCheck(host, guidance, LivePatchCodesByHost));

                groups[Messages.CHECKING_SERVER_NEEDS_REBOOT] = rebootChecks;
            }

            //Checking can evacuate host
            if (update == null || update.after_apply_guidance.Contains(update_after_apply_guidance.restartHost))
            {
                var evacuateChecks = new List<Check>();
                foreach (Host host in applicableServers)
                    evacuateChecks.Add(new AssertCanEvacuateCheck(host, LivePatchCodesByHost));

                groups[Messages.CHECKING_CANEVACUATE_STATUS] = evacuateChecks;
            }

            if (update != null || WizardMode == WizardMode.AutomatedUpdates)
            {
                var restartChecks = new List<Check>();

                foreach (var pool in SelectedPools)
                    restartChecks.Add(new RestartHostOrToolstackPendingOnMasterCheck(pool, WizardMode == WizardMode.AutomatedUpdates ? null : update.uuid));

                groups[Messages.CHECKING_FOR_PENDING_RESTART] = restartChecks;
            }

            return groups;
        }

        [DefaultValue(true)]
        protected bool ManualUpgrade { set; get; }

        public override void PageCancelled()
        {
            DeregisterEventHandlers();
            if (_worker != null)
                _worker.CancelAsync();
            if (resolvePrechecksAction != null && !resolvePrechecksAction.IsCompleted)
                resolvePrechecksAction.Cancel();
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            DeregisterEventHandlers();
            if (direction == PageLoadedDirection.Back && _worker != null)
            {
                _worker.CancelAsync();
                _worker = null;
            }

            base.PageLeave(direction, ref cancel);
        }

        public override bool EnablePrevious()
        {
            return !IsCheckInProgress && !IsResolveActionInProgress;
        }
        
        public override bool EnableNext()
        {
            // CA-65508: if the problem cannot be solved immediately there's no point in
            // enabling the Resolve All button
            // CA-136211: Changed the code below to enable the Resolve All button only
            // when there is at least one problem and all the problems have solution/fix.

            bool problemsFound = false;
            bool allFixable = true;

            foreach (PreCheckGridRow row in dataGridView1.Rows)
            {
                PreCheckHostRow hostRow = row as PreCheckHostRow;
                if (hostRow != null && hostRow.IsProblem)
                {
                    problemsFound = true;

                    if (!hostRow.IsFixable)
                    {
                        allFixable = false;
                        break;
                    }
                }
            }

            bool actionInProgress = IsResolveActionInProgress;
            bool checkInProgress = IsCheckInProgress;

            buttonResolveAll.Enabled = !actionInProgress && !checkInProgress && problemsFound && allFixable;
            buttonReCheckProblems.Enabled = !actionInProgress && !checkInProgress;
            checkBoxViewPrecheckFailuresOnly.Enabled = !actionInProgress && !checkInProgress;
            
            labelProgress.Visible = actionInProgress || checkInProgress;
            pictureBoxIssues.Visible = labelIssues.Visible = problemsFound && !actionInProgress && !checkInProgress;

            return !IsCheckInProgress && !IsResolveActionInProgress && !problemsFound;
        }

        public UpdateType SelectedUpdateType { private get; set; }
        public Pool_patch Patch { private get; set; }
        public Pool_update PoolUpdate { private get; set; }

        #region Nested classes and enums

        internal enum PreCheckResult { OK, Info, Warning, Failed }

        private abstract class PreCheckGridRow : XenAdmin.Controls.DataGridViewEx.DataGridViewExRow
        {
            protected DataGridViewImageCell _iconCell = new DataGridViewImageCell();
            protected DataGridViewTextBoxCell _descriptionCell = new DataGridViewTextBoxCell();
            protected DataGridViewCell _solutionCell = null;
            protected PreCheckGridRow(DataGridViewCell solutionCell)
            {
                _solutionCell = solutionCell;
                Cells.Add(_iconCell);
                Cells.Add(_descriptionCell);
                Cells.Add(_solutionCell);
            }
        }

        private class PreCheckHeaderRow : PreCheckGridRow
        {
            private string description;

            public PreCheckHeaderRow(string text)
                : base(new DataGridViewTextBoxCell())
            {
                _iconCell.Value = new Bitmap(1, 1);
                description = text;
                _descriptionCell.Value = text;
                _descriptionCell.Style.Font = new Font(Program.DefaultFont, FontStyle.Bold);
            }

            public void UpdateDescription(PreCheckResult precheckResult)
            {
                string result;
                switch (precheckResult)
                {
                    case PreCheckResult.OK:
                        result = Messages.GENERAL_STATE_OK;
                        break;
                    case PreCheckResult.Info:
                        result = Messages.INFORMATION;
                        break;
                    case PreCheckResult.Warning:
                        result = Messages.WARNING;
                        break;
                    default:
                        result = Messages.FAILED;
                        break;
                }

                _descriptionCell.Value = string.Format("{0} {1}", description, result);
            }
        }

        private class PreCheckHostRow : PreCheckGridRow
        {
            private Problem _problem = null;
            private Check _check = null;
            public PreCheckHostRow(Problem problem)
                : base(new DataGridViewTextBoxCell())
            {
                _problem = problem;
                _check = problem.Check;
                UpdateRowFields();
            }

            public PreCheckHostRow(Check check)
                : base(new DataGridViewTextBoxCell())
            {
                _check = check;
                UpdateRowFields();
            }

            public bool IsFixable 
            {
                get
                {
                    return Problem != null && Problem.IsFixable && !string.IsNullOrEmpty(this.Solution);
                }
            }

            private void UpdateRowFields()
            {
                _iconCell.Value = Problem == null
                    ? Images.GetImage16For(Icons.Ok)
                    : Problem.Image;

                string description = string.Empty;

                if (Problem != null)
                    description = Problem.Description;
                else if (_check != null)
                {
                    description = _check.SuccessfulCheckDescription;
                    if (string.IsNullOrEmpty(description))
                        description = _check.Host != null
                            ? String.Format(Messages.PATCHING_WIZARD_HOST_CHECK_OK, _check.Host.Name(), _check.Description)
                            : String.Format(Messages.PATCHING_WIZARD_CHECK_OK, _check.Description);
                }
                
                if (description != string.Empty)
                    _descriptionCell.Value = String.Format(Messages.PATCHING_WIZARD_DESC_CELL_INDENT, null, description);

                _solutionCell.Value = Problem == null ? string.Empty : Problem.HelpMessage;

                if (Problem is WarningWithInformationUrl)
                    _solutionCell.Value = (Problem as WarningWithInformationUrl).LinkText;

                if (Problem is ProblemWithInformationUrl)
                    _solutionCell.Value = (Problem as ProblemWithInformationUrl).LinkText;

                UpdateSolutionCellStyle();
            }

            private void UpdateSolutionCellStyle()
            {
                if (_solutionCell == null)
                    return;
                if (Enabled) 
                {
                    _solutionCell.Style.Font = new Font(Program.DefaultFont, FontStyle.Underline);
                    _solutionCell.Style.ForeColor = Color.Blue;
                }
                else
                    _solutionCell.Style = DefaultCellStyle;
            }

            public Problem Problem
            {
                get
                {
                    return _problem;
                }
            }

            public string Solution
            {
                get { return (string)_solutionCell.Value; }
            }
            
            public bool IsProblem
            {
                get { return _problem != null && !(_problem is Warning); }
            }
            
            public PreCheckResult PrecheckResult
            {
                get
                {
                    if (Problem == null)
                        return PreCheckResult.OK;
                    if (_problem is Information) 
                        return PreCheckResult.Info;
                    if (_problem is Warning) 
                        return PreCheckResult.Warning;
                    return PreCheckResult.Failed;
                }
            }

            public override bool Equals(object obj)
            {
                PreCheckHostRow other = obj as PreCheckHostRow;
                if (other != null && other.Problem != null && _problem != null)
                    return _problem.CompareTo(other.Problem) == 0;
                return false;
            }

            public override int GetHashCode()
            {
                return (_problem != null ? _problem.GetHashCode() : 0);
            }

            public override bool Enabled
            {
                get
                {
                    return base.Enabled;
                }
                set
                {
                    base.Enabled = value;
                    UpdateSolutionCellStyle();
                }
            }
        }

        #endregion

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            PreCheckHostRow preCheckHostRow = dataGridView1.Rows[e.RowIndex] as PreCheckHostRow;
            if (preCheckHostRow != null && preCheckHostRow.Enabled && e.ColumnIndex == 2)
            {
                ExecuteSolution(preCheckHostRow);
            }
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && dataGridView1.CurrentCell != null)
            {
                PreCheckHostRow preCheckHostRow = dataGridView1.CurrentCell.OwningRow as PreCheckHostRow;
                int columnIndex = dataGridView1.CurrentCell.ColumnIndex;

                if (preCheckHostRow != null && preCheckHostRow.Enabled && columnIndex == 2)
                    ExecuteSolution(preCheckHostRow);
            }
        }

        private void ExecuteSolution(PreCheckHostRow preCheckHostRow)
        {
            bool cancelled;
            resolvePrechecksAction = preCheckHostRow.Problem.SolveImmediately(out cancelled);

            if (resolvePrechecksAction != null)
            {
                // disable all problems 
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    PreCheckHostRow preCheckRow = row as PreCheckHostRow;
                    if (preCheckRow != null && preCheckRow.Problem != null)
                    {
                        preCheckRow.Enabled = false;
                    }
                }
                StartResolvePrechecksAction();
            }
            else
            {
                if (preCheckHostRow.Problem is WarningWithInformationUrl)
                    (preCheckHostRow.Problem as WarningWithInformationUrl).LaunchUrlInBrowser();
                else if (preCheckHostRow.Problem is ProblemWithInformationUrl)
                    (preCheckHostRow.Problem as ProblemWithInformationUrl).LaunchUrlInBrowser();
                    
                else if (!cancelled)
                    using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Information,
                                                                        string.Format(Messages.PATCHING_WIZARD_SOLVE_MANUALLY, preCheckHostRow.Problem.Description).Replace("\\n", "\n"),
                                                                        Messages.PATCHINGWIZARD_PRECHECKPAGE_TEXT)))
                    {
                        dlg.ShowDialog(this);
                    }
            }
        }

        private void buttonReCheckProblems_Click(object sender, EventArgs e)
        {
            RefreshRechecks();
        }

        private void buttonResolveAll_Click(object sender, EventArgs e)
        {
            List<AsyncAction> actions = new List<AsyncAction>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                PreCheckHostRow preCheckHostRow = row as PreCheckHostRow;
                if (preCheckHostRow != null && preCheckHostRow.Problem != null)
                {
                    bool cancelled;
                    AsyncAction action = preCheckHostRow.Problem.SolveImmediately(out cancelled);
                    if (action != null)
                    {
                        preCheckHostRow.Enabled = false;
                        actions.Add(action);
                    }
                }
            }
            resolvePrechecksAction = new ParallelAction(Messages.PATCHINGWIZARD_PRECHECKPAGE_RESOLVING_ALL, Messages.PATCHINGWIZARD_PRECHECKPAGE_RESOLVING_ALL, Messages.COMPLETED, actions, true, false);
            StartResolvePrechecksAction();
        }

        private void resolvePrecheckAction_Changed(object sender)
        {
            var action = sender as AsyncAction;
            if (action == null)
                return;

            Program.Invoke(this, () => UpdateActionProgress(action));
        }

        private void resolvePrecheckAction_Completed(object sender)
        {
            var action = sender as AsyncAction;
            if (action == null)
                return;

            action.Changed -= resolvePrecheckAction_Changed;
            action.Completed -= resolvePrecheckAction_Completed;

            Program.Invoke(this,  RefreshRechecks);
        }

        private void StartResolvePrechecksAction()
        {
            if (resolvePrechecksAction == null)
                return;
            resolvePrechecksAction.Changed += resolvePrecheckAction_Changed;
            resolvePrechecksAction.Completed += resolvePrecheckAction_Completed;
            resolvePrechecksAction.RunAsync();
            UpdateActionProgress(resolvePrechecksAction);
            OnPageUpdated();
        }

        private void UpdateActionProgress(AsyncAction action)
        {
            progressBar1.Value = action == null ? 0 : action.PercentComplete;
            labelProgress.Text = action == null ? string.Empty : action.Description;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            RefreshRechecks();
        }

        private void dataGridView1_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            PreCheckHostRow preCheckHostRow = dataGridView1.Rows[e.RowIndex] as PreCheckHostRow;
            if (preCheckHostRow != null && preCheckHostRow.Enabled && e.ColumnIndex == 2 && !string.IsNullOrEmpty(preCheckHostRow.Solution))
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Arrow;
        }
    }
}
