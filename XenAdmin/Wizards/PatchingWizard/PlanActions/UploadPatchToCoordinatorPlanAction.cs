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
using XenAdmin.Core;
using XenAPI;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAdmin.Network;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    class UploadPatchToCoordinatorPlanAction : PlanActionWithSession
    {
        private readonly XenServerPatch xenServerPatch;
        private readonly List<HostUpdateMapping> mappings;
        private Dictionary<XenServerPatch, string> AllDownloadedPatches = new Dictionary<XenServerPatch, string>();
        private KeyValuePair<XenServerPatch, string> patchFromDisk;
        private AsyncAction inProgressAction;
        private bool skipDiskSpaceCheck;
        private readonly UpdateType updateType;
        private string updateFilePath;
        private readonly Control invokingControl;
        private readonly List<Host> selectedServers;

        public UploadPatchToCoordinatorPlanAction(Control invokingControl, IXenConnection connection, XenServerPatch xenServerPatch,
            List<HostUpdateMapping> mappings, Dictionary<XenServerPatch, string> allDownloadedPatches,
            KeyValuePair<XenServerPatch, string> patchFromDisk, bool skipDiskSpaceCheck = false)
            : base(connection)
        {
            this.invokingControl = invokingControl;
            this.xenServerPatch = xenServerPatch;
            this.mappings = mappings;
            AllDownloadedPatches = allDownloadedPatches;
            this.patchFromDisk = patchFromDisk;
            this.skipDiskSpaceCheck = skipDiskSpaceCheck;
        }

        public UploadPatchToCoordinatorPlanAction(Control invokingControl, IXenConnection connection, List<Host> selectedServers,
            string updateFilePath, UpdateType updateType, List<HostUpdateMapping> mappings, bool skipDiskSpaceCheck = false)
            : base(connection)
        {
            this.invokingControl = invokingControl;
            this.updateFilePath = updateFilePath;
            this.updateType = updateType;
            this.mappings = mappings;
            this.skipDiskSpaceCheck = skipDiskSpaceCheck;
            this.selectedServers = selectedServers;
        }

        protected override void RunWithSession(ref Session session)
        {   
            var conn = session.Connection;
            var coordinator = Helpers.GetCoordinator(conn);

            var existingMapping = FindExistingMapping(conn, coordinator);
            if (existingMapping != null && existingMapping.IsValid)
                return;

            string path = null;
            if (xenServerPatch != null)
            {
                path = AllDownloadedPatches.ContainsKey(xenServerPatch)
                    ? AllDownloadedPatches[xenServerPatch]
                    : patchFromDisk.Key == xenServerPatch
                        ? patchFromDisk.Value
                        : null;
            }
            else if (updateFilePath != null)
            {
                path = updateFilePath;
            }

            if (string.IsNullOrEmpty(path))
                return;

            AddProgressStep(string.Format(Messages.UPDATES_WIZARD_UPLOADING_UPDATE, GetUpdateName(), conn.Name));

            if (xenServerPatch != null)
            {
                if (Helpers.ElyOrGreater(coordinator))
                    UploadUpdate(conn, session, coordinator, path);
                else
                    UploadLegacyPatch(conn, session, path);
            }
            else if (updateFilePath != null)
            {
                if (updateType == UpdateType.ISO)
                    UploadSuppPack(conn, session, path);
                else
                    UploadLegacyPatch(conn, session, path);
            }
        }

        private HostUpdateMapping FindExistingMapping(IXenConnection conn, Host coordinator)
        {
            if (xenServerPatch != null)
            {
                if (Helpers.ElyOrGreater(coordinator))
                {
                    var poolUpdates = new List<Pool_update>(conn.Cache.Pool_updates);

                    return (from HostUpdateMapping hum in mappings
                        let pum = hum as PoolUpdateMapping
                        where pum != null && poolUpdates.Any(p => pum.Matches(coordinator, xenServerPatch, p))
                        select pum).FirstOrDefault();
                }
                else
                {
                    var poolPatches = new List<Pool_patch>(conn.Cache.Pool_patches);

                    return (from HostUpdateMapping hum in mappings
                        let ppm = hum as PoolPatchMapping
                        where ppm != null && poolPatches.Any(p => ppm.Matches(coordinator, xenServerPatch, p))
                        select ppm).FirstOrDefault();
                }
            }
            else if (updateFilePath != null)
            {
                if (Helpers.ElyOrGreater(coordinator))
                {
                    var poolUpdates = new List<Pool_update>(conn.Cache.Pool_updates);

                    return (from HostUpdateMapping hum in mappings
                        let spm = hum as SuppPackMapping
                        where spm != null && poolUpdates.Any(p => spm.Matches(coordinator, updateFilePath, p))
                        select spm).FirstOrDefault();
                }
                else
                {
                    return (from HostUpdateMapping hum in mappings
                        let spm = hum as SuppPackMapping
                        where spm != null && spm.Matches(coordinator, updateFilePath)
                        select spm).FirstOrDefault();
                }
            }

            return null;
        }

        private void UploadUpdate(IXenConnection conn, Session session, Host coordinator, string path)
        {
            var uploadIsoAction = new UploadSupplementalPackAction(conn, new List<Host> { coordinator }, path, true);
            uploadIsoAction.Changed += uploadAction_Changed;
            uploadIsoAction.Completed += uploadAction_Completed;
            inProgressAction = uploadIsoAction;
            uploadIsoAction.RunSync(session);

            var poolUpdate = uploadIsoAction.PoolUpdate;

            if (poolUpdate == null)
            {
                log.ErrorFormat("Upload finished successfully, but Pool_update object has not been found for update {0} on host (uuid={1}).",
                    xenServerPatch != null ? $"(uuid={xenServerPatch.Uuid})" : GetUpdateName(), conn);

                throw new Exception(Messages.ACTION_UPLOADPATCHTOCOORDINATORPLANACTION_FAILED);
            }

            var newMapping = new PoolUpdateMapping(xenServerPatch, poolUpdate, Helpers.GetCoordinator(conn),
                new Dictionary<Host, SR>(uploadIsoAction.SrsWithUploadedUpdatesPerHost));

            if (!mappings.Contains(newMapping))
                mappings.Add(newMapping);
        }

        private void UploadLegacyPatch(IXenConnection conn, Session session, string path)
        {
            if (!skipDiskSpaceCheck)
                CheckDiskSpace(conn, session, path);

            var uploadPatchAction = new UploadPatchAction(conn, path, true, false);
            uploadPatchAction.Changed += uploadAction_Changed;
            uploadPatchAction.Completed += uploadAction_Completed;
            inProgressAction = uploadPatchAction;
            uploadPatchAction.RunSync(session);

            // this has to be run again to refresh poolPatches (to get the recently uploaded one as well)
            var poolPatches = new List<Pool_patch>(conn.Cache.Pool_patches);

            Pool_patch poolPatch;
            if (xenServerPatch != null)
            {
                poolPatch = poolPatches.Find(p => string.Equals(p.uuid, xenServerPatch.Uuid, StringComparison.OrdinalIgnoreCase));

                if (poolPatch == null)
                {
                    log.ErrorFormat("Upload finished successfully, but Pool_patch object has not been found for patch (uuid={0}) on host (uuid={1}).",
                        xenServerPatch.Uuid, conn);
                    throw new Exception(Messages.ACTION_UPLOADPATCHTOCOORDINATORPLANACTION_FAILED);
                }

                var newMapping = new PoolPatchMapping(xenServerPatch, poolPatch, Helpers.GetCoordinator(conn));
                if (!mappings.Contains(newMapping))
                    mappings.Add(newMapping);
            }
            else
            {
                poolPatch = uploadPatchAction.Patch;
                if (poolPatch == null)
                {
                    log.ErrorFormat("Upload finished successfully, but Pool_patch object has not been found for patch {0} on host (uuid={1}).",
                        updateFilePath, conn);
                    throw new Exception(Messages.ACTION_UPLOADPATCHTOCOORDINATORPLANACTION_FAILED);
                }

                var newMapping = new OtherLegacyMapping(updateFilePath, poolPatch, Helpers.GetCoordinator(conn));
                if (!mappings.Contains(newMapping))
                    mappings.Add(newMapping);
            }
        }

        private void UploadSuppPack(IXenConnection conn, Session session, string path)
        {
            var uploadIsoAction = new UploadSupplementalPackAction(conn, selectedServers, path, true);
            uploadIsoAction.Changed += uploadAction_Changed;
            uploadIsoAction.Completed += uploadAction_Completed;
            inProgressAction = uploadIsoAction;
            uploadIsoAction.RunSync(session);

            var poolUpdate = uploadIsoAction.PoolUpdate;

            var suppPackVdis = new Dictionary<Host, VDI>();

            foreach (var kvp in uploadIsoAction.VdiRefsPerHost)
            {
                var vdi = kvp.Key.Connection.Resolve(kvp.Value);
                if (vdi != null)
                    suppPackVdis.Add(kvp.Key, vdi);
            }

            var newMapping = new SuppPackMapping(updateFilePath, poolUpdate, Helpers.GetCoordinator(conn),
                new Dictionary<Host, SR>(uploadIsoAction.SrsWithUploadedUpdatesPerHost), suppPackVdis);

            if (!mappings.Contains(newMapping))
                mappings.Add(newMapping);
        }

        private void CheckDiskSpace(IXenConnection conn, Session session, string path)
        {
            try
            {
                var checkSpaceForUpload = new CheckDiskSpaceForPatchUploadAction(Helpers.GetCoordinator(conn), path, true);
                inProgressAction = checkSpaceForUpload;
                checkSpaceForUpload.RunSync(session);
            }
            catch (NotEnoughSpaceException e)
            {
                if (!e.DiskSpaceRequirements.CanCleanup)
                    throw;

                var dialogResult = Program.Invoke(invokingControl, (Func<DialogResult>)(() =>
                        {
                            using (var d = new WarningDialog(e.DiskSpaceRequirements.GetSpaceRequirementsMessage(),
                                ThreeButtonDialog.ButtonOK, ThreeButtonDialog.ButtonCancel))
                            {
                                return d.ShowDialog(invokingControl);
                            }
                        }
                    ), null);

                if (dialogResult is DialogResult dr && dr == DialogResult.OK)
                    new CleanupDiskSpaceAction(e.DiskSpaceRequirements.Host, null, true).RunSync(session);
                else
                    throw;
            }
        }

        private void uploadAction_Changed(ActionBase action)
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

        private void uploadAction_Completed(ActionBase action)
        {
            if (action == null)
                return;

            action.Changed -= uploadAction_Changed;
            action.Completed -= uploadAction_Completed;
        }

        private string GetUpdateName()
        {
            if (xenServerPatch != null)
                return xenServerPatch.Name;

            try
            {
                return Path.GetFileName(updateFilePath);
            }
            catch
            {
                return string.Empty;
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
