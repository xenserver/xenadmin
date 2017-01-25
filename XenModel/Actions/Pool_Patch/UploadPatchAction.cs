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
using System.IO;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class UploadPatchAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<Host, Pool_patch> patches = new Dictionary<Host, Pool_patch>();

        private readonly IList<Host> retailHosts;
        private readonly string retailPatchPath;
        private readonly bool deleteFileOnCancel;

        /// <summary>
        /// This constructor is used to upload a single 'normal' patch
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="path"></param>
        public UploadPatchAction(IXenConnection connection, string path)
            : this(connection, path, false, false)
        {}

        public UploadPatchAction(IXenConnection connection, string path, bool suppressHistory, bool deleteFileOnCancel)
            : base(connection, null, Messages.UPLOADING_PATCH, suppressHistory)
        {
            Host master = Helpers.GetMaster(connection);
            this.deleteFileOnCancel = deleteFileOnCancel;
            if (master == null)
                throw new NullReferenceException();

            ApiMethodsToRoleCheck.Add("pool.sync_database");
            ApiMethodsToRoleCheck.Add("http/put_pool_patch_upload");

            retailHosts = new List<Host>(new Host[] { master });
            retailPatchPath = path;

            Host = master;
        }

        public Dictionary<Host, Pool_patch> PatchRefs
        {
            get
            {
                return patches;
            }
        }

        private static long FileSize(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Length;
        }

        private long TotalSize = 0;
        private long TotalUploaded = 0;

        protected override void Run()
        {
            SafeToExit = false;
            TotalSize = retailHosts.Count * FileSize(retailPatchPath);

            foreach (Host host in retailHosts)
            {
                try
                {
                    patches[host] = UploadRetailPatch(host);
                }
                catch (Failure f)
                {
                    // Need to check if the patch already exists.  
                    // If it does then we use it and ignore the PATCH_ALREADY_EXISTS error (CA-110209).
                    if (f.ErrorDescription != null
                        && f.ErrorDescription.Count > 1
                        && f.ErrorDescription[0] == XenAPI.Failure.PATCH_ALREADY_EXISTS)
                    {
                        string uuid = f.ErrorDescription[1];
                        Session session = host.Connection.DuplicateSession();
                        patches[host] = Connection.Resolve(Pool_patch.get_by_uuid(session, uuid));
                    }
                    else
                        throw;
                }
            }

            if (retailHosts.Count > 1)
                this.Description = Messages.ALL_UPDATES_UPLOADED;
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling;
        }

        private Pool_patch UploadRetailPatch(Host host)
        {
            log.InfoFormat("Uploading file '{0}' to server '{1}'", Path.GetFileName(retailPatchPath), host.Name);
            this.Description = Messages.UPLOAD_PATCH_DESCRIPTION;

            long size = FileSize(retailPatchPath);

            Session session = NewSession();

            try
            {
                Host h = Helpers.GetMaster(Connection);

                HTTP.UpdateProgressDelegate progressDelegate = delegate(int percent)
                    {
                        int actionPercent = (int)(((TotalUploaded * 90) + (size * percent)) / TotalSize);
                        this.Tick(actionPercent, this.Description);
                    };

                RelatedTask = XenAPI.Task.create(session, "uploadTask", retailPatchPath);

                String result;

                try
                {
                    result = HTTPHelper.Put(progressDelegate, GetCancelling, true, Connection, RelatedTask, ref session, retailPatchPath,
                        h.address, (HTTP_actions.put_ss)HTTP_actions.put_pool_patch_upload, session.uuid);
                }
                catch(CancelledException)
                {
                     if(deleteFileOnCancel && File.Exists(retailPatchPath))
                     {
                        File.Delete(retailPatchPath);
                     }
                     throw;
                }

                finally
                {
                    Task.destroy(session, RelatedTask);
                    RelatedTask = null;
                }

                TotalUploaded += size;
                return Connection.WaitForCache(new XenRef<Pool_patch>(result));
            }
            finally
            {
                log.InfoFormat("File '{0}' uploaded to server '{1}'", Path.GetFileName(retailPatchPath), host.Name);
                Description = Messages.UPLOAD_PATCH_UPLOADED;
            }
        }
    }
}
