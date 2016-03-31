/* Copyright (c) Citrix Systems Inc. 
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

namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_AutoUpdatingPage : XenTabPage
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public XenAdmin.Core.Updates.UpgradeSequence UpgradeSequences { get; set; }
        private BackgroundWorker actionsWorker = null;
        private bool _thisPageHasBeenCompleted = false;

        public List<Problem> ProblemsResolvedPreCheck { private get; set; }

        public List<Host> SelectedMasters { private get; set; }

        private List<PoolPatchMapping> patchMappings = new List<PoolPatchMapping>();
        private Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();

        public PatchingWizard_AutoUpdatingPage()
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

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);

            if (_thisPageHasBeenCompleted)
            {
                actionsWorker = null;
                return;
            }

            Dictionary<Host, List<PlanAction>> planActionsPerPool = new Dictionary<Host, List<PlanAction>>();

            foreach (var master in SelectedMasters)
            {
                Dictionary<Host, List<PlanAction>> planActionsForAPool = new Dictionary<Host, List<PlanAction>>();
                Dictionary<Host, List<PlanAction>> delayedActionsForAPool = new Dictionary<Host, List<PlanAction>>();

                foreach (var host in master.Connection.Cache.Hosts)
                {
                    planActionsForAPool.Add(host, new List<PlanAction>());
                    delayedActionsForAPool.Add(host, new List<PlanAction>());
                }

                //var master = Helpers.GetMaster(pool.Connection);
                var hosts = master.Connection.Cache.Hosts;

                var us = Updates.GetUpgradeSequence(master.Connection);

                foreach (var patch in us.UniquePatches)
                {
                    if (master != null && us.ContainsKey(master) && us[master].Any(p => string.Equals(p.Uuid, patch.Uuid, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        planActionsForAPool[master].AddRange(CompileActionList(master, patch));
                        if (patch.GuidanceMandatory)
                        {
                            var mandatoryGuidance = CompileAfterPatchGuidanceActionList(master, patch);
                            planActionsForAPool[master].AddRange(mandatoryGuidance);
                            
                            //remove delayed action of the same kind for this host
                            foreach (var mg in mandatoryGuidance)
                                delayedActionsForAPool[master].RemoveAll(a => a.GetType() == mg.GetType());
                        }
                        else
                        {
                            var afterPatch = CompileAfterPatchGuidanceActionList(master, patch);

                            foreach (var ap in afterPatch)
                                if (!delayedActionsForAPool.ContainsKey(master) || !delayedActionsForAPool[master].Any(da => da.GetType() == ap.GetType()))
                                    delayedActionsForAPool[master].Add(ap);
                        }
                    }

                    foreach (var host in master.Connection.Cache.Hosts)
                    {
                        if (!host.IsMaster() && us.ContainsKey(host) && us[host].Any(p => string.Equals(p.Uuid, patch.Uuid, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            planActionsForAPool[host].AddRange(CompileActionList(host, patch)); 
                            
                            if (patch.GuidanceMandatory)
                            {
                                var mandatoryGuidance = CompileAfterPatchGuidanceActionList(host, patch);

                                planActionsForAPool[host].AddRange(mandatoryGuidance);

                                //remove delayed action of the same kind for this host
                                foreach (var mg in mandatoryGuidance)
                                    delayedActionsForAPool[host].RemoveAll(a => a.GetType() == mg.GetType());
                            }
                            else
                            {
                                var afterPatch = CompileAfterPatchGuidanceActionList(host, patch);

                                foreach (var ap in afterPatch)
                                    if (!delayedActionsForAPool.ContainsKey(host) || !delayedActionsForAPool[host].Any(da => da.GetType() == ap.GetType()))
                                        delayedActionsForAPool[host].Add(ap);
                            }
                        }
                    }

                }//patch

                //add all delayed actions to the end of the actions, per host
                foreach (var kvp in delayedActionsForAPool)
                {
                    planActionsForAPool[kvp.Key].AddRange(kvp.Value);
                }

                //add all actions for the pool to the main actions list
                //master first
                foreach (var kvp in planActionsForAPool)
                {
                    if (!planActionsPerPool.ContainsKey(master))
                        planActionsPerPool.Add(master, new List<PlanAction>());

                    planActionsPerPool[master].AddRange(kvp.Value);
                }

                planActionsPerPool[master].Add(new RemoveUpdateFilesFromMaster(master, patchMappings));

            } //foreach in SelectedMasters

            var planActions = new List<PlanAction>();

            //actions list for serial execution
            foreach (var actionListPerPool in planActionsPerPool)
            {
                planActions.AddRange(actionListPerPool.Value);
            }

            planActions.Add(new UnwindProblemsAction(ProblemsResolvedPreCheck));

            actionsWorker = new BackgroundWorker();
            actionsWorker.DoWork += new DoWorkEventHandler(PatchingWizardAutomaticPatchWork);
            actionsWorker.WorkerReportsProgress = true;
            actionsWorker.ProgressChanged += new ProgressChangedEventHandler(actionsWorker_ProgressChanged);
            actionsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(actionsWorker_RunWorkerCompleted);
            actionsWorker.WorkerSupportsCancellation = true;
            actionsWorker.RunWorkerAsync(planActions);
        }

        #region automatic_mode

        private void actionsWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!actionsWorker.CancellationPending)
            {
                PlanAction action = (PlanAction)e.UserState;
                if (e.ProgressPercentage == 0)
                    textBoxLog.Text += action;
                else
                {
                    textBoxLog.Text += string.Format("{0}\r\n", Messages.DONE);
                    progressBar.Value += e.ProgressPercentage;
                }
            }
        }

        private void PatchingWizardAutomaticPatchWork(object obj, DoWorkEventArgs doWorkEventArgs)
        {
            List<PlanAction> actionList = (List<PlanAction>)doWorkEventArgs.Argument;
            foreach (PlanAction action in actionList)
            {
                try
                {
                    if (actionsWorker.CancellationPending)
                    {
                        doWorkEventArgs.Cancel = true;
                        return;
                    }
                    this.actionsWorker.ReportProgress(0, action);
                    action.Run();
                    Thread.Sleep(1000);

                    this.actionsWorker.ReportProgress((int)((1.0 / (double)actionList.Count) * 100), action);

                }
                catch (Exception e)
                {

                    log.Error("Failed to carry out plan.", e);
                    log.Debug(actionList);
                    doWorkEventArgs.Result = new Exception(action.Title, e);
                    break;
                }
            }
        }

        private void actionsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                Exception exception = e.Result as Exception;
                if (exception != null)
                {
                    FinishedWithErrors(exception);

                }
                else
                {
                    FinishedSuccessfully();
                }

                progressBar.Value = 100;
                _cancelEnabled = false;
                _nextEnabled = true;
            }
            OnPageUpdated();

            _thisPageHasBeenCompleted = true;
        }

        private List<PlanAction> CompileActionList(Host host, XenServerPatch patch)
        {
            List<PlanAction> actions = new List<PlanAction>();

            if (patch == null)
                return actions;

            //download (if needed), upload to master (if needed), set mappings
            actions.Add(new DownloadAndUploadPatch(host.Connection, patch, patchMappings, AllDownloadedPatches));

            actions.Add(new ApplyXenServerPatchPlanAction(host, patch, patchMappings));

            return actions;
        }

        private List<PlanAction> CompileAfterPatchGuidanceActionList(Host host, XenServerPatch patch)
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

        private static List<XenRef<VM>> RunningVMs(Host host, Pool_patch patch)
        {
            List<XenRef<VM>> vms = new List<XenRef<VM>>();
            foreach (VM vm in patch.Connection.ResolveAll(host.resident_VMs))
            {
                if (!vm.is_a_real_vm)
                    continue;

                vms.Add(new XenRef<VM>(vm.opaque_ref));
            }
            return vms;
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

        private void FinishedWithErrors(Exception exception)
        {
            //labelTitle.Text = string.Format(Messages.UPDATE_WAS_NOT_COMPLETED, GetUpdateName());

            string errorMessage = null;

            if (exception != null && exception.InnerException != null && exception.InnerException is Failure)
            {
                var innerEx = exception.InnerException as Failure;
                errorMessage = innerEx.Message;

                if (innerEx.ErrorDescription != null && innerEx.ErrorDescription.Count > 0)
                    log.Error(string.Concat(innerEx.ErrorDescription.ToArray()));
            }

            labelError.Text = errorMessage ?? string.Format(Messages.PATCHING_WIZARD_ERROR, exception.Message);

            log.ErrorFormat("Error message displayed: {0}", labelError.Text);

            pictureBox1.Image = SystemIcons.Error.ToBitmap();
            if (exception.InnerException is SupplementalPackInstallFailedException)
            {
                errorLinkLabel.Visible = true;
                errorLinkLabel.Tag = exception.InnerException;
            }
            panel1.Visible = true;
        }

        private void FinishedSuccessfully()
        {
            labelTitle.Text = Messages.PATCHINGWIZARD_UPDATES_DONE_AUTOMATIC_MODE;
            pictureBox1.Image = null;
            labelError.Text = Messages.CLOSE_WIZARD_CLICK_FINISH;
        }
    }
}

