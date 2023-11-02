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

using System;
using System.Collections.Generic;
using System.IO;
using XenAdmin.Actions;
using XenAdmin.Actions.Updates;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    class DownloadPatchPlanAction : PlanActionWithSession
    {
        private static object _lock = new object();
        private static readonly Dictionary<string, object> _patchLocks = new Dictionary<string, object>();
        private readonly XenServerPatch patch;
        private Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();
        private KeyValuePair<XenServerPatch, string> patchFromDisk;

        public DownloadPatchPlanAction(IXenConnection connection, XenServerPatch patch, Dictionary<XenServerPatch, string> allDownloadedPatches, KeyValuePair<XenServerPatch, string> patchFromDisk)
            : base(connection)
        {
            this.patch = patch;
            this.AllDownloadedPatches = allDownloadedPatches;
            this.patchFromDisk = patchFromDisk;
        }

        protected override void RunWithSession(ref Session session)
        {
            AddProgressStep(string.Format(Messages.PATCHINGWIZARD_DOWNLOADUPDATE_ACTION_TITLE_WAITING, patch.Name));

            object patchLock;
            lock (_lock)
            {
                if (!_patchLocks.TryGetValue(patch.Uuid, out patchLock))
                {
                    patchLock = new object();
                    _patchLocks[patch.Uuid] = patchLock;
                }
            }

            //if we are updating multiple pools at the same time, we only need to download the patch for
            // the first pool, hence we lock it to prevent the plan action of the other pools to run
            lock (patchLock)
            {
                if (Cancelling)
                    return;

                //skip the download if the patch has been already downloaded or we are using a patch from disk
                if (AllDownloadedPatches.ContainsKey(patch) && File.Exists(AllDownloadedPatches[patch]) 
                    || patchFromDisk.Key == patch && File.Exists(patchFromDisk.Value))
                {
                    ReplaceProgressStep(string.Format(Messages.PATCHINGWIZARD_DOWNLOADUPDATE_ACTION_TITLE_SKIPPING, patch.Name));
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

            var downloadAction = new DownloadAndUnzipUpdateAction(patch.Name, address, "iso");

            downloadAction.Changed += DownloadAction_Changed;
            downloadAction.Completed += DownloadAction_Completed;

            downloadAction.RunSync(session);
        }

        private void DownloadAction_Changed(ActionBase action)
        {
            if (action == null)
                return;

            if (Cancelling)
                action.Cancel();

            var bpAction = action as IByteProgressAction;
            if (bpAction == null)
                return;

            if (!string.IsNullOrEmpty(bpAction.ByteProgressDescription))
                ReplaceProgressStep(bpAction.ByteProgressDescription);
        }


        private void DownloadAction_Completed(ActionBase sender)
        {
            if (!(sender is DownloadAndUnzipUpdateAction action))
                return;

            action.Changed -= DownloadAction_Changed;
            action.Completed -= DownloadAction_Completed;

            if (action.Succeeded)
                AllDownloadedPatches[patch] = action.UpdatePath;
        }
    }
}
