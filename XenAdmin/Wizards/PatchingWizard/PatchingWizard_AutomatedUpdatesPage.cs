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
using System.IO;
using System.Reflection;
using System.Threading;
using log4net;
using XenAdmin.Controls;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;
using XenAdmin.Actions;
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Network;
using System.Text;
using System.Diagnostics;

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_AutomatedUpdatesPage : XenTabPage
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public XenAdmin.Core.Updates.UpgradeSequence UpgradeSequences { get; set; }
        private bool _thisPageIsCompleted = false;

        public List<Problem> ProblemsResolvedPreCheck { private get; set; }

        public List<Pool> SelectedPools { private get; set; }

        private List<PoolPatchMapping> patchMappings = new List<PoolPatchMapping>();
        public Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();

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

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            base.PageLeave(direction, ref cancel);
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (_thisPageIsCompleted)
            {
                return;
            }

            foreach (var pool in SelectedPools)
            {
                var master = Helpers.GetMaster(pool.Connection);

                var planActions = new List<PlanAction>();
                var delayedActionsByHost = new Dictionary<Host, List<PlanAction>>();

                foreach (var host in pool.Connection.Cache.Hosts)
                {
                    delayedActionsByHost.Add(host, new List<PlanAction>());
                }

                var hosts = pool.Connection.Cache.Hosts;

                var us = Updates.GetUpgradeSequence(pool.Connection);

                Debug.Assert(us != null, "Update sequence should not be null.");

                if (us != null)
                {
                    foreach (var patch in us.UniquePatches)
                    {
                        var hostsToApply = us.Where(u => u.Value.Contains(patch)).Select(u => u.Key).ToList();
                        hostsToApply.Sort();

                        planActions.Add(new DownloadPatchPlanAction(master.Connection, patch, AllDownloadedPatches));
                        planActions.Add(new UploadPatchToMasterPlanAction(master.Connection, patch, patchMappings, AllDownloadedPatches));
                        planActions.Add(new PatchPrechecksOnMultipleHostsInAPoolPlanAction(master.Connection, patch, hostsToApply, patchMappings));

                        foreach (var host in hostsToApply)
                        {
                            planActions.Add(new ApplyXenServerPatchPlanAction(host, patch, patchMappings));
                            planActions.AddRange(GetMandatoryActionListForPatch(delayedActionsByHost[host], host, patch));
                            UpdateDelayedAfterPatchGuidanceActionListForHost(delayedActionsByHost[host], host, patch);
                        }

                        //clean up master at the end:
                        planActions.Add(new RemoveUpdateFileFromMasterPlanAction(master, patchMappings, patch));

                    }//patch
                }

                //add all delayed actions to the end of the actions, per host
                var delayedActions = new List<PlanAction>();
                if (delayedActionsByHost.ContainsKey(master))
                {
                    delayedActions.AddRange(delayedActionsByHost[master]);
                }
                foreach (var kvp in delayedActionsByHost)
                {
                    if (kvp.Key != master)
                    {
                        delayedActions.AddRange(kvp.Value);
                    }
                }

                if (planActions.Count > 0)
                {
                    var bgw = new UpdateProgressBackgroundWorker(master, planActions, delayedActions);
                    backgroundWorkers.Add(bgw);

                }
            } //foreach in SelectedMasters

            foreach (var bgw in backgroundWorkers)
            {
                bgw.DoWork += new DoWorkEventHandler(WorkerDoWork);
                bgw.WorkerReportsProgress = true;
                bgw.ProgressChanged += new ProgressChangedEventHandler(WorkerProgressChanged);
                bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
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
                    sb.Append(pa);
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

            foreach (PlanAction action in bgw.AllActions)
            {
                try
                {
                    if (bgw.CancellationPending)
                    {
                        doWorkEventArgs.Cancel = true;
                        return;
                    }

                    InitializePlanAction(bgw, action);

                    bgw.ReportProgress(0, action);
                    action.Run();

                    Thread.Sleep(1000);

                    bgw.doneActions.Add(action);
                    bgw.ReportProgress((int)((1.0 / (double)bgw.AllActions.Count) * 100), action);
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
                        var positionOfFailedAction = bgw.AllActions.IndexOf(action);
                        if (positionOfFailedAction < bgw.AllActions.Count && !(action is DownloadPatchPlanAction || action is UploadPatchToMasterPlanAction))
                        {
                            int pos = positionOfFailedAction;

                            if (!(bgw.AllActions[pos] is RemoveUpdateFileFromMasterPlanAction)) //can't do anything if the remove action has failed
                            {
                                while (++pos < bgw.AllActions.Count)
                                {
                                    if (bgw.AllActions[pos] is RemoveUpdateFileFromMasterPlanAction) //find the next remove
                                    {
                                        bgw.AllActions[pos].Run();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        //already in an error case - best effort
                    }

                    bgw.ReportProgress(0);
                    break;
                }
            }
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

        private void UpdateDelayedAfterPatchGuidanceActionListForHost(List<PlanAction> delayedGuidances, Host host, XenServerPatch patch)
        {
            List<PlanAction> actions = GetAfterApplyGuidanceActionsForPatch(host, patch);

            //if this is a restart, clean delayed list
            if (patch.after_apply_guidance == after_apply_guidance.restartHost)
                delayedGuidances.Clear();
            
            if (!patch.GuidanceMandatory)
            {
                //not mandatory, so these actions will have to be run later
                //add the ones that are not yet there
                foreach (var ap in actions)
                    if (!delayedGuidances.Any(da => da.GetType() == ap.GetType()))
                    {
                        //special rules
                        //do not add restartXAPI if there is already a restartHost on the list
                        if (delayedGuidances.Any(da => da is RebootHostPlanAction) && ap is RestartAgentPlanAction)
                            continue;

                        delayedGuidances.Add(ap);
                    }
            }
            else
            {
                //remove delayed action of the same kind for this host (because it is mandatory and will run immediately)
                delayedGuidances.RemoveAll(dg => actions.Any(ma => ma.GetType() == dg.GetType()));                 
            }
        }

        private static List<PlanAction> GetAfterApplyGuidanceActionsForPatch(Host host, XenServerPatch patch)
        {
            List<PlanAction> actions = new List<PlanAction>();

            List<XenRef<VM>> runningVMs = RunningVMs(host);

            if (patch.after_apply_guidance == after_apply_guidance.restartHost)
            {
                actions.Add(new EvacuateHostPlanAction(host));
                actions.Add(new RebootHostPlanAction(host));
                actions.Add(new BringBabiesBackAction(runningVMs, host, false));
            }

            if (patch.after_apply_guidance == after_apply_guidance.restartXAPI)
            {
                actions.Add(new RestartAgentPlanAction(host));
            }

            if (patch.after_apply_guidance == after_apply_guidance.restartHVM)
            {
                actions.Add(new RebootVMsPlanAction(host, RunningHvmVMs(host)));
            }

            if (patch.after_apply_guidance == after_apply_guidance.restartPV)
            {
                actions.Add(new RebootVMsPlanAction(host, RunningPvVMs(host)));
            }

            return actions;
        }

        private List<PlanAction> GetMandatoryActionListForPatch(List<PlanAction> delayedGuidances, Host host, XenServerPatch patch)
        {
            var actions = new List<PlanAction>();

            if (!patch.GuidanceMandatory)
                return actions;

            actions = GetAfterApplyGuidanceActionsForPatch(host, patch);

            return actions;
        }

        private static List<XenRef<VM>> RunningHvmVMs(Host host)
        {
            List<XenRef<VM>> vms = new List<XenRef<VM>>();
            foreach (VM vm in host.Connection.ResolveAll(host.resident_VMs))
            {
                if (!vm.IsHVM || !vm.is_a_real_vm)
                    continue;
                vms.Add(new XenRef<VM>(vm.opaque_ref));
            }
            return vms;
        }

        private static List<XenRef<VM>> RunningPvVMs(Host host)
        {
            List<XenRef<VM>> vms = new List<XenRef<VM>>();
            foreach (VM vm in host.Connection.ResolveAll(host.resident_VMs))
            {
                if (vm.IsHVM || !vm.is_a_real_vm)
                    continue;
                vms.Add(new XenRef<VM>(vm.opaque_ref));
            }
            return vms;
        }

        private static List<XenRef<VM>> RunningVMs(Host host)
        {
            List<XenRef<VM>> vms = new List<XenRef<VM>>();
            foreach (VM vm in host.Connection.ResolveAll(host.resident_VMs))
            {
                if (!vm.is_a_real_vm)
                    continue;

                vms.Add(new XenRef<VM>(vm.opaque_ref));
            }
            return vms;
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
            labelTitle.Text = Messages.PATCHINGWIZARD_UPDATES_DONE_AUTOMATED_UPDATES_MODE;
            progressBar.Value = 100;
            pictureBox1.Image = null;
            labelError.Text = Messages.CLOSE_WIZARD_CLICK_FINISH;
        }
    }
}

