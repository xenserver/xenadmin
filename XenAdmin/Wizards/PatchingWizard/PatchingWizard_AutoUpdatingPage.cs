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

        public List<Host> SelectedMasters { private get; set; }
        //public List<Host> SelectedServers { private get; set; }

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

            //if (_thisPageHasBeenCompleted)
            //{
            //    actionsWorker = null;
            //    return;
            //}

            Dictionary<Host, List<PlanAction>> planActionsPerPool = new Dictionary<Host, List<PlanAction>>();

            foreach (var master in SelectedMasters)
            {
                List<PlanAction> planActionsForAPool = new List<PlanAction>();
                Dictionary<Host, List<PlanAction>> delayedActionsForAPool = new Dictionary<Host, List<PlanAction>>();

                foreach (var host in master.Connection.Cache.Hosts)
                    delayedActionsForAPool.Add(host, new List<PlanAction>());

                //var master = Helpers.GetMaster(pool.Connection);
                var hosts = master.Connection.Cache.Hosts;

                var us = Updates.GetUpgradeSequence(master.Connection);

                foreach (var patch in us.UniquePatches)
                {
                    if (master != null && us.ContainsKey(master) && us[master].Any(p => string.Equals(p.Uuid, patch.Uuid, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        planActionsForAPool.AddRange(CompileActionList(master, patch));
                        if (patch.GuidanceMandatory)
                        {
                            var mandatoryGuidance = CompileAfterPatchGuidanceActionList(master, patch);
                            planActionsForAPool.AddRange(mandatoryGuidance);
                            
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
                            planActionsForAPool.AddRange(CompileActionList(host, patch)); 
                            
                            if (patch.GuidanceMandatory)
                            {
                                var mandatoryGuidance = CompileAfterPatchGuidanceActionList(host, patch);

                                planActionsForAPool.AddRange(mandatoryGuidance);

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

                //add all delayed actions to the end of the actions
                foreach (var delayedActionsPerHost in delayedActionsForAPool.Values)
                    planActionsForAPool.AddRange(delayedActionsPerHost);

                //todo cleanup

                //if (RemoveUpdateFile)
                //{
                //    planActionsForAPool.Add(new RemoveUpdateFile(pool, poolPatch));
                //}

                //add all actions for the pool to the main actions list
                planActionsPerPool[master] = planActionsForAPool;
            }


            //todo run the planactions

            //planActions.Add(new UnwindProblemsAction(ProblemsResolvedPreCheck));

            //actionsWorker = new BackgroundWorker();
            //actionsWorker.DoWork += new DoWorkEventHandler(PatchingWizardAutomaticPatchWork);
            //actionsWorker.WorkerReportsProgress = true;
            //actionsWorker.ProgressChanged += new ProgressChangedEventHandler(actionsWorker_ProgressChanged);
            //actionsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(actionsWorker_RunWorkerCompleted);
            //actionsWorker.WorkerSupportsCancellation = true;
            //actionsWorker.RunWorkerAsync(planActions);
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
            //todo below
            //_thisPageHasBeenCompleted = true;

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
            //labelTitle.Text = string.Format(Messages.UPDATE_WAS_SUCCESSFULLY_INSTALLED, GetUpdateName());
            pictureBox1.Image = null;
            labelError.Text = Messages.CLOSE_WIZARD_CLICK_FINISH;
        }

        /*
        private List<XenAdmin.Core.XenServerPatch> GetPatchesForHost(Host h)
        {
            if (h == null)
                return null;

            return UpgradeSequences[h];
        }

        private List<after_apply_guidance> DelayedGuidancesForHost(Host h)
        {
            if (h == null)
                return null;

            return null;
            //return UpgradeSequences[h].Where(p => !string.IsNullOrEmpty(p.Guidance)).Select(p => ); //todo change from string
        }


        private List<PlanAction> GetAllActionsForPool(IXenConnection conn)
        {
            var master = Helpers.GetMaster(conn);
            var hosts = Connection.Cache.Hosts;

            var us = Updates.GetUpgradeSequence(conn);

            var actions = new List<AsyncAction>();

            foreach (var patch in us.UniquePatches)
            {
             //master first

                actions.Add(GetDownloadFileAction(patch));
                actions.Add(PrepareUploadToHostActions(master, patch));
               
                
                    
                //////RUN actions
                //now we should have pool_patch on master
                
                var poolPatches = new List<Pool_patch>(master.Connection.Cache.Pool_patches);
                var poolPatch = poolPatches.Find(pp => string.Equals(pp.uuid, patch.Uuid));
                
                var precheck = new XenAdmin.Diagnostics.Checks.PatchPrecheckCheck(master, poolPatch);
                var problem = precheck.RunCheck();
                if (problem != null)
                    //there is a problem
                    throw Exception(); //todo stop

                //precheck
                actions.Add(new ApplyPatchAction(patch, new List<Host>() { master }));

                if (patch.GuidanceMandatory)
                {
                    var afterGuidancePlanActions = CompileAfterApplyGuidanceActionListForPatch(master, patch);
                    
                    actionsWorker = new BackgroundWorker();
                    actionsWorker.DoWork += new DoWorkEventHandler(PatchingWizardAutomaticPatchWork);
                    actionsWorker.WorkerReportsProgress = true;
                    actionsWorker.ProgressChanged += new ProgressChangedEventHandler(actionsWorker_ProgressChanged);
                    actionsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(actionsWorker_RunWorkerCompleted);
                    actionsWorker.WorkerSupportsCancellation = true;
                    actionsWorker.RunWorkerAsync(afterGuidancePlanActions);

                }
                
                //guidances master

             //hosts after
                //apply
                //precheck
                //after patch guidances
                //celanup host

             //cleanup master
            }

            *
             *  Patch
             *      Master
             *          Download
             *          Upload to master
             *          Precheck
             *          ApplyPatchAction
             *          Remove
             *          Restart, Other Actions
             *      Host1
             *      ...
             *      Host2
             *      ...
             *



            var patches = GetPatchesForHost(host);
            foreach (var patch in patches)
            {


                //List<Pool_patch> poolPatches = new List<Pool_patch>(host.Connection.Cache.Pool_patches);
                //Pool_patch poolPatch = poolPatches.Find(pp => string.Equals(pp.uuid, patch.Uuid));


                //ClearUpHostAfterPatch(patch, host);      PoolPatchCleanAction
                
                
                ApplyAfterPatchGuidancesIfMandatoryNow(patch, host);
            }

            actions.AddRange(ApplyAfterPatchGuidancesIfDelayedNeeded(host));

            
            
            using (var multipleAction = new MultipleAction(Connection, "", "", "", actions, true, true, true))
            {
                multipleAction.Changed += action_Changed;
                multipleAction.Completed += action_Completed;
                multipleAction.RunAsync();
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



        private void FinishedWithErrors(Exception exception)
        {
            //labelTitle.Text = string.Format(Messages.UPDATE_WAS_NOT_COMPLETED, GetUpdateName());

            //string errorMessage = null;

            //if (exception != null && exception.InnerException != null && exception.InnerException is Failure)
            //{
            //    var innerEx = exception.InnerException as Failure;
            //    errorMessage = innerEx.Message;

            //    if (innerEx.ErrorDescription != null && innerEx.ErrorDescription.Count > 0)
            //        log.Error(string.Concat(innerEx.ErrorDescription.ToArray()));
            //}

            //labelError.Text = errorMessage ?? string.Format(Messages.PATCHING_WIZARD_ERROR, exception.Message);

            //log.ErrorFormat("Error message displayed: {0}", labelError.Text);

            //pictureBox1.Image = SystemIcons.Error.ToBitmap();
            //if (exception.InnerException is SupplementalPackInstallFailedException)
            //{
            //    errorLinkLabel.Visible = true;
            //    errorLinkLabel.Tag = exception.InnerException;
            //}
            //panel1.Visible = true;
        }


        private void FinishedSuccessfully()
        {
            //labelTitle.Text = string.Format(Messages.UPDATE_WAS_SUCCESSFULLY_INSTALLED, GetUpdateName());
            //pictureBox1.Image = null;
            //labelError.Text = Messages.CLOSE_WIZARD_CLICK_FINISH;
        }
























        private void action_Changed(ActionBase sender)
        {
            //AsyncAction action = (AsyncAction)sender;
            //Program.Invoke(Program.MainWindow, delegate { progressBar.Value = action.PercentComplete; });
        }

        private void action_Completed(ActionBase sender)
        {
            //_nextEnabled = true;
            //_cancelEnabled = false;

            //try
            //{
            //    if (sender.Exception != null)
            //    {
            //        Program.Invoke(Program.MainWindow, () => FinishedWithErrors(new Exception(sender.Title, sender.Exception)));
            //    }
            //    else
            //        Program.Invoke(Program.MainWindow, FinishedSuccessfully);
            //}
            //catch (Exception except)
            //{
            //    Program.Invoke(Program.MainWindow, () => FinishedWithErrors(except));
            //}
            //Program.Invoke(Program.MainWindow, OnPageUpdated);
            //_thisPageHasBeenCompleted = true;
        }



        private List<PlanAction> ApplyPatchAction(Host host, Pool_patch patch)
        {





        }

        private List<PlanAction> CompileAfterApplyGuidanceActionListForPatch(Host host, XenServerPatch patch)
        {
            List<PlanAction> actions = new List<PlanAction>();

            if (patch == null)
                return actions;

            List<XenRef<VM>> runningVMs = RunningVMs(host);

            //actions.Add(new ApplyPatchPlanAction(host, patch));

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

        //private List<PlanAction> ApplyAfterPatchGuidancesIfDelayedNeeded(Host host)
        //{
        //    List<PlanAction> actions = new List<PlanAction>();

        //    var guidances = DelayedGuidancesForHost(host);
            
        //    List<XenRef<VM>> runningVMs = RunningVMs(host);

        //    if (guidances.Any(g => g == after_apply_guidance.restartHost))
        //    {
        //        actions.Add(new EvacuateHostPlanAction(host));
        //        actions.Add(new RebootHostPlanAction(host));
        //        actions.Add(new BringBabiesBackAction(runningVMs, host, false));
        //    }

        //    if (guidances.Any(g => g == after_apply_guidance.restartXAPI))
        //    {
        //        actions.Add(new RestartAgentPlanAction(host));
        //    }

        //    if (guidances.Any(g => g == after_apply_guidance.restartHVM))
        //    {
        //        actions.Add(new RebootVMsPlanAction(host, RunningHvmVMs(host)));
        //    }

        //    if (guidances.Any(g => g == after_apply_guidance.restartPV))
        //    {
        //        actions.Add(new RebootVMsPlanAction(host, RunningPvVMs(host)));
        //    }

        //    return actions;
        //}

        private void AddToUploadedUpdates(string patchPath, Host host)
        {
            if (!uploadedUpdates.ContainsKey(patchPath))
            {
                List<Host> hosts = new List<Host>();
                hosts.Add(host);
                uploadedUpdates.Add(patchPath, hosts);
            }
            else if (!uploadedUpdates[patchPath].Contains(host))
            {
                uploadedUpdates[patchPath].Add(host);
            }
        }

        private AsyncAction PrepareUploadToHostActions(Host host, XenServerPatch patch)
        {
            AsyncAction action = null;

            var master = Helpers.GetMaster(host.Connection);
            
            var patchTempFile = downloadedPatches[patch.Uuid];

            if (!PatchExistsOnPool(patch, master) 
                || !uploadedUpdates.ContainsKey(patchTempFile) 
                || !uploadedUpdates[patchTempFile].Contains(master))
            {
                action = new UploadPatchAction(master.Connection, patchTempFile, true, false);
                AddToUploadedUpdates(patchTempFile, master);

                
            }
            
            

            return action;

                
                //if (action != null)
                //{
                //    action.Changed += singleAction_Changed;
                //    action.Completed += singleAction_Completed;
                //}
                //else
                //{
                //    _patch = GetPatchFromPatchPath();
                //}
                //uploadActions.Add(selectedServer, action);
            

            //foreach (KeyValuePair<Host, AsyncAction> uploadAction in uploadActions)
            //{
            //    flickerFreeListBox1.Items.Add(uploadAction);
            //}

            //flickerFreeListBox1.Refresh();
            //OnPageUpdated();
        }


        private static bool PatchExistsOnPool(XenServerPatch patch, Host poolMaster)
        {
            var poolPatches = new List<Pool_patch>(poolMaster.Connection.Cache.Pool_patches);

            return (poolPatches.Exists(p => string.Equals(p.uuid, patch.Uuid, StringComparison.OrdinalIgnoreCase)));
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





        private Dictionary<string, List<Host>> uploadedUpdates = new Dictionary<string, List<Host>>();

        private Dictionary<string, string> downloadedPatches = new Dictionary<string, string>();

        private AsyncAction GetDownloadFileAction(XenServerPatch patch)
        {
            string patchUri = patch.PatchUrl;
            if (string.IsNullOrEmpty(patchUri))
                throw new NullReferenceException("PatchUrl");

            Uri address = new Uri(patchUri);
            string tempFile = Path.GetTempFileName();

            var downloadAction = new DownloadAndUnzipXenServerPatchAction(patch.Name, address, tempFile, Branding.Update);
            downloadedPatches[patch.Uuid] = tempFile;
            return downloadAction;

            //if (downloadAction != null)
            //{
            //    downloadAction.Changed += singleAction_Changed;
            //    downloadAction.Completed += singleAction_Completed;
            //}

            //downloadAction.RunAsync();

            //flickerFreeListBox1.Items.Clear();
            //flickerFreeListBox1.Items.Add(downloadAction);
            //flickerFreeListBox1.Refresh();
            //OnPageUpdated();

            //UpdateActionProgress(downloadAction);
        }








        */
    }
}
