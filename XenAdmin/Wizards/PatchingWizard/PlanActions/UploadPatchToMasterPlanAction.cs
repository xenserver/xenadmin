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
using XenAdmin.Diagnostics.Problems.HostProblem;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    class UploadPatchToMasterPlanAction : PlanActionWithSession
    {
        private readonly XenServerPatch patch;
        private readonly List<PoolPatchMapping> mappings;
        private Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();
        private string tempFileName = null;
        private AsyncAction inProgressAction = null;

        public UploadPatchToMasterPlanAction(IXenConnection connection, XenServerPatch patch, List<PoolPatchMapping> mappings, Dictionary<XenServerPatch, string> allDownloadedPatches)
            : base(connection, string.Format("Uploading update {0} to {1}...", patch.Name, connection.Name))
        {
            this.patch = patch;
            this.mappings = mappings;
            this.AllDownloadedPatches = allDownloadedPatches;
        }

        protected override void RunWithSession(ref Session session)
        {
            var path = AllDownloadedPatches[patch];

            var poolPatches = new List<Pool_patch>(session.Connection.Cache.Pool_patches);
            var conn = session.Connection;
            var master = Helpers.GetMaster(conn);

            var existingMapping = mappings.Find(m => m.MasterHost.uuid == master.uuid && m.Pool_patch != null && m.XenServerPatch == patch);
            
            if (existingMapping == null
                || !poolPatches.Any(p => string.Equals(p.uuid, existingMapping.Pool_patch.uuid, StringComparison.OrdinalIgnoreCase)))
            {
                //free space check for upload:
                try
                {
                    var checkSpaceForUpload = new CheckDiskSpaceForPatchUploadAction(Helpers.GetMaster(conn), path, true);
                    inProgressAction = checkSpaceForUpload;
                    checkSpaceForUpload.RunExternal(session);

                    var uploadPatchAction = new UploadPatchAction(session.Connection, path, true, false);
                    inProgressAction = uploadPatchAction;
                    uploadPatchAction.RunExternal(session);

                    // this has to be run again to refresh poolPatches (to get the recently uploaded one as well)
                    poolPatches = new List<Pool_patch>(session.Connection.Cache.Pool_patches);

                    var poolPatch = poolPatches.Find(p => string.Equals(p.uuid, patch.Uuid, StringComparison.OrdinalIgnoreCase));
                    if (poolPatch == null)
                    {
                        log.ErrorFormat("Upload finished successfully, but Pool_patch object has not been found for patch (uuid={0}) on host (uuid={1}).", patch.Uuid, session.Connection);

                        throw new Exception(Messages.ACTION_UPLOADPATCHTOMASTERPLANACTION_FAILED);
                    }

                    var newMapping = new PoolPatchMapping()
                    {
                        MasterHost = Helpers.GetMaster(session.Connection),
                        XenServerPatch = patch,
                        Pool_patch = poolPatch
                    };

                    if (!mappings.Any(m => m.MasterHost.uuid == newMapping.MasterHost.uuid && m.Pool_patch == newMapping.Pool_patch && m.XenServerPatch == patch))
                        mappings.Add(newMapping);
                }
                catch (Exception ex)
                {
                    Error = ex;
                    throw;
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

            if (Cancelling)
                action.Cancel();

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

        public override void Cancel()
        {
            if (inProgressAction != null)
                inProgressAction.Cancel();

            base.Cancel();
        }
    }
}
