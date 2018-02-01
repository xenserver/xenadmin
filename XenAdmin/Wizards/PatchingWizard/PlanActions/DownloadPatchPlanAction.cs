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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using System.Linq;
using System.IO;
using XenAdmin.Network;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    class DownloadPatchPlanAction : PlanActionWithSession
    {
        private readonly XenServerPatch patch;
        private Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();
        private KeyValuePair<XenServerPatch, string> patchFromDisk;
        private string tempFileName = null;

        public DownloadPatchPlanAction(IXenConnection connection, XenServerPatch patch, Dictionary<XenServerPatch, string> allDownloadedPatches, KeyValuePair<XenServerPatch, string> patchFromDisk)
            : base(connection, string.Format(Messages.PATCHINGWIZARD_DOWNLOADUPDATE_ACTION_TITLE_WAITING, patch.Name))
        {
            this.patch = patch;
            this.AllDownloadedPatches = allDownloadedPatches;
            this.patchFromDisk = patchFromDisk;
        }

        protected override void RunWithSession(ref Session session)
        {
            this.visible = false;

            lock (patch)
            {
                this.visible = true;
                this._title = string.Format(Messages.PATCHINGWIZARD_DOWNLOADUPDATE_ACTION_TITLE_DOWNLOADING, patch.Name);

                if (Cancelling)
                    return;

                //skip the download if the patch has been already downloaded or we are using a patch from disk
                if ((AllDownloadedPatches.ContainsKey(patch) && File.Exists(AllDownloadedPatches[patch])) 
                    || (patchFromDisk.Key == patch && File.Exists(patchFromDisk.Value)))
                {
                    this.visible = false;
                    this._title = string.Format(Messages.PATCHINGWIZARD_DOWNLOADUPDATE_ACTION_TITLE_SKIPPING, patch.Name);
                }
                else
                {
                    DownloadFile(ref session);
                }
            }

        }

        private void DownloadFile(ref Session session)
        {
            string patchUri = patch.PatchUrl;
            if (string.IsNullOrEmpty(patchUri))
                return;

            Uri address = new Uri(patchUri);
            tempFileName = Path.GetTempFileName();

            var downloadAction = new DownloadAndUnzipXenServerPatchAction(patch.Name, address, tempFileName, false, Helpers.ElyOrGreater(Connection) ? Branding.UpdateIso : Branding.Update);

            if (downloadAction != null)
            {
                downloadAction.Changed += downloadAndUnzipXenServerPatchAction_Changed;
                downloadAction.Completed += downloadAndUnzipXenServerPatchAction_Completed;
            }

            downloadAction.RunExternal(session);
        }

        private void downloadAndUnzipXenServerPatchAction_Changed(object sender)
        {
            var downloadAction = sender as DownloadAndUnzipXenServerPatchAction;
            if (downloadAction == null)
                return;

            if (Cancelling)
                downloadAction.Cancel();

            Program.Invoke(Program.MainWindow, () =>
            {
                if (!string.IsNullOrEmpty(downloadAction.DownloadProgressDescription))
                    ProgressDescription = string.Format(Messages.PATCHINGWIZARD_AUTOUPDATINGPAGE_IN_PROGRESS_DOTDOTDOT, downloadAction.DownloadProgressDescription);
            });
        }


        private void downloadAndUnzipXenServerPatchAction_Completed(ActionBase sender)
        {
            ProgressDescription = null;

            var action = sender as AsyncAction;
            if (action == null)
                return;

            action.Changed -= downloadAndUnzipXenServerPatchAction_Changed;
            action.Completed -= downloadAndUnzipXenServerPatchAction_Completed;

            if (action.Succeeded)
            {
                if (action is DownloadAndUnzipXenServerPatchAction)
                {
                    Host master = Helpers.GetMaster(action.Connection);

                    AllDownloadedPatches[patch] = (action as DownloadAndUnzipXenServerPatchAction).PatchPath;
                }
            }
           
        }
    }
}
