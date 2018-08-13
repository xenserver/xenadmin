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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Wizards.BugToolWizardFiles;
using XenAdmin.Dialogs;
using XenAdmin.Actions;

namespace XenAdmin.Wizards
{
    public partial class BugToolWizard : XenWizardBase
    {
        private readonly GenericSelectHostsPage bugToolPageSelectHosts1;
        private readonly BugToolPageSelectCapabilities bugToolPageSelectCapabilities1;
        private readonly BugToolPageRetrieveData bugToolPageRetrieveData;
        private readonly BugToolPageDestination bugToolPageDestination1;

        public BugToolWizard(params IXenObject[] selectedObjects)
            : this()
        {
            bugToolPageSelectHosts1.SelectHosts(new List<IXenObject>(selectedObjects));
        }

        public BugToolWizard()
        {
            InitializeComponent();

            bugToolPageSelectHosts1 = new GenericSelectHostsPage();
            bugToolPageSelectCapabilities1 = new BugToolPageSelectCapabilities();
            bugToolPageRetrieveData = new BugToolPageRetrieveData();
            bugToolPageDestination1 = new BugToolPageDestination();

            AddPage(bugToolPageSelectHosts1);
            AddPage(bugToolPageSelectCapabilities1);
            AddPage(bugToolPageRetrieveData);
            AddPage(bugToolPageDestination1);
        }

        protected override void FinishWizard()
        {
            // If the user has chosen a file that already exists, get confirmation
            string path = bugToolPageDestination1.OutputFile;
            if (File.Exists(path))
            {
                DialogResult dialogResult;
                using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Warning, string.Format(Messages.FILE_X_EXISTS_OVERWRITE, path), Messages.XENCENTER),
                        ThreeButtonDialog.ButtonOK,
                        new ThreeButtonDialog.TBDButton(Messages.CANCEL, DialogResult.Cancel, ThreeButtonDialog.ButtonType.CANCEL, true)))
                {
                    dialogResult = dlg.ShowDialog(this);
                }
                if (dialogResult != DialogResult.OK)
                {
                    FinishCanceled();
                    return;
                }
            }

            // Check we can write to the destination file - otherwise we only find out at the 
            // end of the ZipStatusReportAction, and the downloaded server files are lost,
            // and the user will have to run the wizard again.
            try
            {
                using (FileStream temp = File.OpenWrite(path))
                {
                    // Yay, it worked
                }
            }
            catch (Exception exn)
            {
                // Failure
                using (var dlg = new ThreeButtonDialog(
                   new ThreeButtonDialog.Details(
                       SystemIcons.Error,
                       string.Format(Messages.COULD_NOT_WRITE_FILE, path, exn.Message),
                       Messages.XENCENTER)))
                {
                    dlg.ShowDialog(this);
                }
                FinishCanceled();
                return;
            }

            AsyncAction action;
            if (bugToolPageDestination1.Upload)
            {
                // The MultipleAction is only used as a wrapper, we will suppress its history and expose the sub-actions in the history
                List<AsyncAction> subActions = new List<AsyncAction>();
                ZipStatusReportAction zipAction = new ZipStatusReportAction(bugToolPageRetrieveData.OutputFolder, bugToolPageDestination1.OutputFile,false);
                subActions.Add(zipAction);
                UploadServerStatusReportAction uploadAction = new UploadServerStatusReportAction(bugToolPageDestination1.OutputFile,// tmp folder
                                                                                           bugToolPageDestination1.UploadToken,// upload token
                                                                                           bugToolPageDestination1.CaseNumber,// case id
                                                                                           Registry.HealthCheckUploadDomainName,// domain name
                                                                                           false); // suppressHistory
                subActions.Add(uploadAction);
                action = new MultipleAction(null, Messages.BUGTOOL_SAVING, Messages.BUGTOOL_SAVING, Messages.COMPLETED, subActions,true);
            }
            else
            {
                action = new ZipStatusReportAction(bugToolPageRetrieveData.OutputFolder, bugToolPageDestination1.OutputFile, false);
            }

            action.RunAsync();


            // Save away the output path for next time
            XenAdmin.Properties.Settings.Default.ServerStatusPath = bugToolPageDestination1.OutputFile;

            log.Debug("Cleaning up crash dump logs on server");
            var capabilities = bugToolPageSelectCapabilities1.Capabilities;
            foreach (Capability c in capabilities)
            {
                if (c.Key == "host-crashdump-dumps" && c.Checked)
                {
                    var hostList = bugToolPageSelectHosts1.SelectedHosts;
                    if (!hostList.Any(h => h.HasCrashDumps()))
                        break;

                    DialogResult result;
                    using (var dlg = new ThreeButtonDialog(
                                new ThreeButtonDialog.Details(null, Messages.REMOVE_CRASHDUMP_QUESTION, Messages.REMOVE_CRASHDUMP_FILES),
                                ThreeButtonDialog.ButtonYes,
                                ThreeButtonDialog.ButtonNo))
                    {
                        result = dlg.ShowDialog(this);
                    }
                    if (result == DialogResult.Yes)
                    {
                        foreach (Host host in hostList)
                        {
                            if (host != null && host.HasCrashDumps())
                            {
                                new Actions.DestroyHostCrashDumpAction(host).RunAsync();
                            }
                        }
                    }
                    break;
                }
            }

            base.FinishWizard();
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            var prevPageType = senderPage.GetType();

            if (prevPageType == typeof(GenericSelectHostsPage))
            {
                bugToolPageRetrieveData.SelectedHosts = bugToolPageSelectHosts1.SelectedHosts;
            }
            else if (prevPageType == typeof(BugToolPageSelectCapabilities))
            {
                bugToolPageRetrieveData.CapabilityList = bugToolPageSelectCapabilities1.Capabilities;
            }
        }

        protected override bool RunNextPagePrecheck(XenTabPage senderPage)
        {
            if (senderPage.GetType() == typeof(GenericSelectHostsPage))
            {
                var hostList = bugToolPageSelectHosts1.SelectedHosts;
                return bugToolPageSelectCapabilities1.GetCommonCapabilities(hostList);
            }

            return true;
        }
    }
}
