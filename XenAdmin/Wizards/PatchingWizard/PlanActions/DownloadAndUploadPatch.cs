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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;
using System.Linq;
using System.IO;
using XenAdmin.Network;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    class DownloadAndUploadPatch : PlanActionWithSession
    {
        private readonly XenRef<Pool_patch> _patchRef;
        //private readonly string path;
        private readonly XenServerPatch patch;
        private readonly List<PoolPatchMapping> mappings;
        private Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();
        private string tempFileName = null;

        public DownloadAndUploadPatch(IXenConnection connection, XenServerPatch patch, List<PoolPatchMapping> mappings, Dictionary<XenServerPatch, string> allDownloadedPatches)
            : base(connection, string.Format("Downloading and uploading {0} to {1}...", patch.Name, connection.Name))
        {
            this.patch = patch;
            this.mappings = mappings;
            this.AllDownloadedPatches = allDownloadedPatches;
        }

        protected override void RunWithSession(ref Session session)
        {
            lock (patch)
            {
                //if it has not been already downloaded
                if (!AllDownloadedPatches.Any(dp => dp.Key == patch && !string.IsNullOrEmpty(dp.Value)) 
                    || !File.Exists(AllDownloadedPatches[patch]))
                {
                    DownloadFile(ref session);
                }
            }

            var path = AllDownloadedPatches[patch];

            var poolPatches = new List<Pool_patch>(session.Connection.Cache.Pool_patches);
            var conn = session.Connection;

            var existingMapping = mappings.Find(m => m.Host == Helpers.GetMaster(conn) && m.Pool_patch != null && m.XenServerPatch == patch);
            if (existingMapping == null
                || !poolPatches.Any(p => string.Equals(p.uuid, existingMapping.Pool_patch.uuid, StringComparison.OrdinalIgnoreCase)))
            {
                var action = new UploadPatchAction(session.Connection, path, true, false);
                action.RunExternal(session);

                var poolPatch = poolPatches.Find(p => string.Equals(p.uuid, patch.Uuid, StringComparison.OrdinalIgnoreCase));

                var newMapping = new PoolPatchMapping()
                {
                    Host = Helpers.GetMaster(session.Connection),
                    XenServerPatch = patch,
                    Pool_patch = poolPatch
                };

                if (!mappings.Any(m => m.Host == newMapping.Host && m.Pool_patch == newMapping.Pool_patch && m.XenServerPatch == patch))
                    mappings.Add(newMapping);
            }
        }

        private void DownloadFile(ref Session session)
        {
            string patchUri = patch.PatchUrl;
            if (string.IsNullOrEmpty(patchUri))
                return;

            Uri address = new Uri(patchUri);
            tempFileName = Path.GetTempFileName();

            var downloadAction = new DownloadAndUnzipXenServerPatchAction(patch.Name, address, tempFileName, Branding.Update);

            if (downloadAction != null)
            {
                downloadAction.Changed += downloadAndUnzipXenServerPatchAction_Changed;
                downloadAction.Completed += downloadAndUnzipXenServerPatchAction_Completed;
            }

            downloadAction.RunExternal(session);
        }

        private void downloadAndUnzipXenServerPatchAction_Changed(object sender)
        {
            var action = sender as AsyncAction;
            if (action == null)
                return;

            Program.Invoke(Program.MainWindow, () =>
            {
                //UpdateActionProgress(action);
                //flickerFreeListBox1.Refresh();
                //OnPageUpdated();
            });
        }

        private void downloadAndUnzipXenServerPatchAction_Completed(ActionBase sender)
        {
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
