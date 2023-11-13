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

using System.Collections.Generic;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Dialogs;
using XenAPI;
using System.Linq;
using System.IO;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAdmin.Alerts;

namespace XenAdmin.Wizards.PatchingWizard
{
    public enum WizardMode { SingleUpdate, AutomatedUpdates, NewVersion }

    /// <summary>
    /// Remember that equals for patches don't work across connections because 
    /// we are not allow to override equals. YOU SHOULD NOT USE ANY OPERATION THAT IMPLIES CALL EQUALS OF Pool_patch or Host_patch
    /// You should do it manually or use delegates.
    /// </summary>
    public partial class PatchingWizard : XenWizardBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PatchingWizard_PatchingPage PatchingWizard_PatchingPage;
        private readonly PatchingWizard_SelectPatchPage PatchingWizard_SelectPatchPage;
        private readonly PatchingWizard_ModePage PatchingWizard_ModePage;
        private readonly PatchingWizard_SelectServers PatchingWizard_SelectServers;
        private readonly PatchingWizard_UploadPage PatchingWizard_UploadPage;
        private readonly PatchingWizard_PrecheckPage PatchingWizard_PrecheckPage;
        private readonly PatchingWizard_FirstPage PatchingWizard_FirstPage;
        private readonly PatchingWizard_AutomatedUpdatesPage PatchingWizard_AutomatedUpdatesPage;

        private bool _isNewGeneration;
        private WizardMode _wizardMode;

        public PatchingWizard()
        {
            InitializeComponent();

            PatchingWizard_FirstPage = new PatchingWizard_FirstPage();
            PatchingWizard_SelectPatchPage = new PatchingWizard_SelectPatchPage();
            PatchingWizard_SelectServers = new PatchingWizard_SelectServers();
            PatchingWizard_UploadPage = new PatchingWizard_UploadPage();
            PatchingWizard_PrecheckPage = new PatchingWizard_PrecheckPage();
            PatchingWizard_ModePage = new PatchingWizard_ModePage();
            PatchingWizard_PatchingPage = new PatchingWizard_PatchingPage();
            PatchingWizard_AutomatedUpdatesPage = new PatchingWizard_AutomatedUpdatesPage();

            AddPage(PatchingWizard_FirstPage);
            AddPage(PatchingWizard_SelectPatchPage);
            AddPage(PatchingWizard_SelectServers);
            AddPage(PatchingWizard_PrecheckPage);
            AddPage(PatchingWizard_PatchingPage);
        }

        public void PrepareToInstallUpdate(bool isNewGeneration)
        {
            if (!IsFirstPage())
                return;

            PatchingWizard_FirstPage.IsNewGeneration = isNewGeneration;
        }

        public void PrepareToInstallUpdate(XenServerPatchAlert alert, List<Host> hosts)
        {
            if (!IsFirstPage())
                return;

            //set the pages before landing on them so they are populated correctly
            PatchingWizard_FirstPage.IsNewGeneration = false;
            PatchingWizard_SelectPatchPage.UpdateAlertFromWeb = alert;
            PatchingWizard_SelectPatchPage.UpdateAlertFromWebSelected += page_UpdateAlertFromWebSelected;
            PatchingWizard_SelectServers.SelectedServers = hosts;
            NextStep(); //FirstPage -> SelectPatchPage
        }

        private void page_UpdateAlertFromWebSelected()
        {
            PatchingWizard_SelectPatchPage.UpdateAlertFromWebSelected -= page_UpdateAlertFromWebSelected;
            NextStep(); //SelectPatchPage -> SelectServers
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            var prevPageType = senderPage.GetType();

            if (prevPageType == typeof(PatchingWizard_FirstPage))
            {
                RemovePage(PatchingWizard_UploadPage);
                RemovePage(PatchingWizard_ModePage);
                RemovePage(PatchingWizard_PatchingPage);
                RemovePage(PatchingWizard_AutomatedUpdatesPage);

                _isNewGeneration = PatchingWizard_FirstPage.IsNewGeneration;

                PatchingWizard_SelectPatchPage.IsNewGeneration = _isNewGeneration;
                PatchingWizard_ModePage.IsNewGeneration = _isNewGeneration;
                PatchingWizard_AutomatedUpdatesPage.IsNewGeneration = _isNewGeneration;

                if (_isNewGeneration)
                {
                    AddAfterPage(PatchingWizard_PrecheckPage, PatchingWizard_ModePage);
                    AddAfterPage(PatchingWizard_ModePage, PatchingWizard_AutomatedUpdatesPage);
                }
                else
                {
                    AddAfterPage(PatchingWizard_PrecheckPage, PatchingWizard_PatchingPage);
                }
            }
            else if (prevPageType == typeof(PatchingWizard_SelectPatchPage))
            {
                _wizardMode = PatchingWizard_SelectPatchPage.WizardMode;
                var wizardIsInAutomatedUpdatesMode = _wizardMode == WizardMode.AutomatedUpdates;

                var updateType = wizardIsInAutomatedUpdatesMode ? UpdateType.Legacy : PatchingWizard_SelectPatchPage.SelectedUpdateType;
                var selectedPatchFilePath = wizardIsInAutomatedUpdatesMode ? null : PatchingWizard_SelectPatchPage.SelectedPatchFilePath;
                var alertFromWeb = wizardIsInAutomatedUpdatesMode ? null : PatchingWizard_SelectPatchPage.UpdateAlertFromWeb;
                var alertFromFileOnDisk = wizardIsInAutomatedUpdatesMode ? null : PatchingWizard_SelectPatchPage.AlertFromFileOnDisk;
                var fileFromDiskHasUpdateXml = !wizardIsInAutomatedUpdatesMode && PatchingWizard_SelectPatchPage.FileFromDiskHasUpdateXml;

                PatchingWizard_SelectServers.IsNewGeneration = _isNewGeneration;
                PatchingWizard_SelectServers.WizardMode = _wizardMode;
                PatchingWizard_SelectServers.SelectedUpdateType = updateType;
                PatchingWizard_SelectServers.UpdateAlertFromWeb = alertFromWeb;
                PatchingWizard_SelectServers.AlertFromFileOnDisk = alertFromFileOnDisk;
                PatchingWizard_SelectServers.FileFromDiskHasUpdateXml = fileFromDiskHasUpdateXml;

                RemovePage(PatchingWizard_UploadPage);
                RemovePage(PatchingWizard_ModePage);
                RemovePage(PatchingWizard_PatchingPage);
                RemovePage(PatchingWizard_AutomatedUpdatesPage);

                if (_wizardMode == WizardMode.SingleUpdate)
                {
                    AddAfterPage(PatchingWizard_SelectServers, PatchingWizard_UploadPage);
                    AddAfterPage(PatchingWizard_PrecheckPage, PatchingWizard_ModePage);
                    AddAfterPage(PatchingWizard_ModePage, PatchingWizard_PatchingPage);
                }
                else if (_isNewGeneration)
                {
                    AddAfterPage(PatchingWizard_PrecheckPage, PatchingWizard_ModePage);
                    AddAfterPage(PatchingWizard_ModePage, PatchingWizard_AutomatedUpdatesPage);
                }
                else // AutomatedUpdates or NewVersion
                {
                    AddAfterPage(PatchingWizard_PrecheckPage, PatchingWizard_AutomatedUpdatesPage);
                }

                PatchingWizard_UploadPage.SelectedUpdateType = updateType;
                PatchingWizard_UploadPage.SelectedPatchFilePath = selectedPatchFilePath;
                PatchingWizard_UploadPage.SelectedUpdateAlert = alertFromWeb ?? alertFromFileOnDisk;
                PatchingWizard_UploadPage.PatchFromDisk = PatchingWizard_SelectPatchPage.PatchFromDisk;

                PatchingWizard_ModePage.SelectedUpdateType = updateType;
                PatchingWizard_ModePage.WizardMode = _wizardMode;

                PatchingWizard_PrecheckPage.IsNewGeneration = _isNewGeneration;
                PatchingWizard_PrecheckPage.WizardMode = _wizardMode;
                PatchingWizard_PrecheckPage.PoolUpdate = null; //reset the PoolUpdate property; it will be updated on leaving the Upload page, if this page is visible
                PatchingWizard_PrecheckPage.UpdateAlert = alertFromWeb ?? alertFromFileOnDisk;

                PatchingWizard_AutomatedUpdatesPage.WizardMode = _wizardMode;
                PatchingWizard_AutomatedUpdatesPage.UpdateAlert = alertFromWeb ?? alertFromFileOnDisk;
                PatchingWizard_AutomatedUpdatesPage.PatchFromDisk = PatchingWizard_SelectPatchPage.PatchFromDisk;

                PatchingWizard_PatchingPage.SelectedUpdateType = updateType;
                PatchingWizard_PatchingPage.SelectedPatchFilePatch = selectedPatchFilePath;
            }
            else if (prevPageType == typeof(PatchingWizard_SelectServers))
            {
                var selectedServers = PatchingWizard_SelectServers.SelectedServers;
                var selectedPools = PatchingWizard_SelectServers.SelectedPools;
                var applyUpdatesToNewVersion = PatchingWizard_SelectServers.ApplyUpdatesToNewVersion;

                PatchingWizard_PrecheckPage.SelectedServers = selectedServers;
                PatchingWizard_PrecheckPage.ApplyUpdatesToNewVersion = applyUpdatesToNewVersion;

                PatchingWizard_ModePage.SelectedPools = selectedPools;
                PatchingWizard_ModePage.SelectedServers = selectedServers;

                PatchingWizard_PatchingPage.SelectedServers = selectedServers;
                PatchingWizard_PatchingPage.SelectedPools = selectedPools;

                PatchingWizard_UploadPage.SelectedServers = selectedServers;
                PatchingWizard_UploadPage.SelectedPools = selectedPools;

                PatchingWizard_AutomatedUpdatesPage.SelectedPools = selectedPools;
                PatchingWizard_AutomatedUpdatesPage.ApplyUpdatesToNewVersion = applyUpdatesToNewVersion;
            }
            else if (prevPageType == typeof(PatchingWizard_UploadPage))
            {
                var update = PatchingWizard_UploadPage.PoolUpdate;
                PatchingWizard_PrecheckPage.PoolUpdate = update;

                var srsWithUploadedUpdates = new Dictionary<Pool_update, Dictionary<Host, SR>>();
                foreach (var mapping in PatchingWizard_UploadPage.PatchMappings)
                {
                    if (mapping is PoolUpdateMapping pum)
                        srsWithUploadedUpdates[pum.PoolUpdate] = pum.SrsWithUploadedUpdatesPerHost;
                    else if (mapping is SuppPackMapping spm && spm.PoolUpdate != null)
                        srsWithUploadedUpdates[spm.PoolUpdate] = spm.SrsWithUploadedUpdatesPerHost;
                }
                PatchingWizard_PrecheckPage.SrUploadedUpdates = srsWithUploadedUpdates;

                PatchingWizard_ModePage.PoolUpdate = update;

                PatchingWizard_PatchingPage.PoolUpdate = update;
            }
            else if (prevPageType == typeof(PatchingWizard_ModePage))
            {
                if (_wizardMode == WizardMode.SingleUpdate)
                {
                    PatchingWizard_PatchingPage.ManualTextInstructions = PatchingWizard_ModePage.ManualTextInstructions;
                    PatchingWizard_PatchingPage.IsAutomaticMode = PatchingWizard_ModePage.IsAutomaticMode;
                    PatchingWizard_PatchingPage.RemoveUpdateFile = PatchingWizard_ModePage.RemoveUpdateFile;
                }
                else
                {
                    PatchingWizard_AutomatedUpdatesPage.PostUpdateTasksAutomatically = PatchingWizard_ModePage.IsAutomaticMode;
                    PatchingWizard_AutomatedUpdatesPage.ManualTextInstructions = PatchingWizard_ModePage.ManualTextInstructions;
                }
            }
            else if (prevPageType == typeof(PatchingWizard_PrecheckPage))
            {
                PatchingWizard_PatchingPage.PrecheckProblemsActuallyResolved = PatchingWizard_PrecheckPage.PrecheckProblemsActuallyResolved;
                PatchingWizard_PatchingPage.LivePatchCodesByHost = PatchingWizard_PrecheckPage.LivePatchCodesByHost;
                PatchingWizard_ModePage.LivePatchCodesByHost = PatchingWizard_PrecheckPage.LivePatchCodesByHost;
                PatchingWizard_AutomatedUpdatesPage.PrecheckProblemsActuallyResolved = PatchingWizard_PrecheckPage.PrecheckProblemsActuallyResolved;
            }
        }
        
        protected override void OnCancel(ref bool cancel)
        {
            base.OnCancel(ref cancel);

            if (cancel)
                return;

            RunMultipleActions(Messages.REVERT_WIZARD_CHANGES, Messages.REVERTING_WIZARD_CHANGES,
                Messages.REVERTED_WIZARD_CHANGES,
                Problem.GetUnwindChangesActions(PatchingWizard_PrecheckPage.PrecheckProblemsActuallyResolved));

            CleanUploadedPatches();
            RemoveDownloadedPatches();
        }
        
        protected override void FinishWizard()
        {
            CleanUploadedPatches();
            RemoveDownloadedPatches();
            Updates.RefreshUpdateAlerts(Updates.UpdateType.ServerPatches);
            base.FinishWizard();
        }

        protected override string WizardPaneHelpID()
        {
            return PatchingWizard_FirstPage.IsNewGeneration ? "PatchingWizard_xs" : "PatchingWizard_ch";
        }

        private void CleanUploadedPatches()
        {
            var list = new List<AsyncAction>();

            foreach (var mapping in PatchingWizard_UploadPage.PatchMappings)
            {
                if (mapping is PoolUpdateMapping updateMapping)
                {
                    var action = GetCleanActionForPoolUpdate(updateMapping.PoolUpdate);
                    if (action != null)
                        list.Add(action);
                    continue;
                }
                
                if (mapping is SuppPackMapping suppPackMapping)
                {
                    if (suppPackMapping.PoolUpdate!= null)
                    {
                        var action = GetCleanActionForPoolUpdate(suppPackMapping.PoolUpdate);
                        if (action != null)
                            list.Add(action);
                    }                        
                    else
                        list.AddRange(GetRemoveVdiActions(suppPackMapping.SuppPackVdis.Values.ToList()));
                }
            }

            RunMultipleActions(Messages.PATCHINGWIZARD_REMOVE_UPDATES, Messages.PATCHINGWIZARD_REMOVING_UPDATES, Messages.PATCHINGWIZARD_REMOVED_UPDATES, list);
        }

        private AsyncAction GetCleanActionForPoolUpdate(Pool_update update)
        {
            if (update == null || update.Connection == null || !update.Connection.IsConnected)
                return null;

            return new DelegatedAsyncAction(update.Connection, Messages.PATCHINGWIZARD_REMOVE_UPDATE, "", "", session =>
            {
                try
                {
                    Pool_update.pool_clean(session, update.opaque_ref);
                    if (!update.AppliedOnHosts().Any())
                        Pool_update.destroy(session, update.opaque_ref);
                }
                catch (Failure f)
                {
                    log.Error("Clean up failed", f);
                }
            });
        }

        private List<AsyncAction> GetRemoveVdiActions(List<VDI> vdisToRemove)
        {
            var list = new List<AsyncAction>();

            if (vdisToRemove != null)
                foreach (var vdi in vdisToRemove)
                {
                    if (vdi.Connection != null && vdi.Connection.IsConnected)
                        list.Add(new DestroyDiskAction(vdi));
                }

            return list;
        }

        private void RemoveDownloadedPatches()
        {
            List<string> listOfDownloadedFiles = new List<string>();

            listOfDownloadedFiles.AddRange(PatchingWizard_AutomatedUpdatesPage.AllDownloadedPatches.Values); // AutomatedUpdates or NewVersion
            listOfDownloadedFiles.AddRange(PatchingWizard_UploadPage.AllDownloadedPatches.Values); //SingleUpdate
            listOfDownloadedFiles.AddRange(PatchingWizard_SelectPatchPage.UnzippedUpdateFiles);

            foreach (string downloadedPatch in listOfDownloadedFiles)
            {
                try
                {
                    if (File.Exists(downloadedPatch))
                    {
                        File.Delete(downloadedPatch);
                    }
                }
                catch
                {  
                    log.DebugFormat("Could not remove downloaded patch {0} ", downloadedPatch);
                }
            }           
        }

        private void RunMultipleActions(string title, string startDescription, string endDescription,
            List<AsyncAction> subActions)
        {
            if (subActions != null && subActions.Count > 0)
            {
                using (MultipleAction multipleAction = new MultipleAction(xenConnection, title, startDescription,
                    endDescription, subActions, false, true))
                {
                    using (var dialog = new ActionProgressDialog(multipleAction, ProgressBarStyle.Blocks))
                        dialog.ShowDialog(Program.MainWindow);
                }
            }
        }
    }
}
