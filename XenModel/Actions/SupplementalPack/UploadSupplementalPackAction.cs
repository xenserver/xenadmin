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
using System.IO;
using System.Linq;
using System.Reflection;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class UploadSupplementalPackAction : AsyncAction, IByteProgressAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string suppPackFilePath;
        private readonly long _totalUpdateSize;
        private readonly List<Host> servers;

        private Pool_update poolUpdate;
        private readonly string _updateName;

        public Dictionary<Host, SR> SrUploadedUpdates = new Dictionary<Host, SR>();

        public Pool_update PoolUpdate
        {
            get { return poolUpdate; }
        }


        public UploadSupplementalPackAction(IXenConnection connection, List<Host> selectedServers, string path, bool suppressHistory)
            : base(connection, null, Messages.SUPP_PACK_UPLOADING, suppressHistory)
        {
            Host master = Helpers.GetMaster(connection);
            if (master == null)
                throw new NullReferenceException();

            ApiMethodsToRoleCheck.Add("VDI.create");
            ApiMethodsToRoleCheck.Add("VDI.destroy");
            ApiMethodsToRoleCheck.Add("VDI.set_other_config");
            ApiMethodsToRoleCheck.Add("http/put_import_raw_vdi");
            
            Host = master;
            suppPackFilePath = path;
            _updateName = Path.GetFileNameWithoutExtension(suppPackFilePath);
            _totalUpdateSize = (new FileInfo(path)).Length;
            servers = selectedServers;
        }

        public readonly Dictionary<Host, XenRef<VDI>> VdiRefsToCleanUp = new Dictionary<Host, XenRef<VDI>>();

        public string ByteProgressDescription { get; set; }

        protected override void Run()
        {
            SafeToExit = false;

            var srList = SelectTargetSr();

            if (srList.Count == 0)
                throw new Exception(Messages.HOTFIX_APPLY_ERROR_NO_SR);

            totalCount = srList.Count;
            foreach (var sr in srList)
            {
                Result = UploadSupplementalPack(sr);
            }
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling;
        }

        private long totalCount;
        private long totalUploaded;

        private string UploadSupplementalPack(SR sr)
        {
            this.Description = String.Format(Messages.SUPP_PACK_UPLOADING_TO, _updateName, sr.Name());
            log.DebugFormat("Creating vdi of size {0} bytes on SR '{1}'", _totalUpdateSize, sr.Name());

            VDI vdi = NewVDI(sr);
            var vdiRef = VDI.create(Session, vdi);

            Host localStorageHost = sr.GetStorageHost();

            string hostUrl;
            if (localStorageHost == null)
            {
                Uri uri = new Uri(Session.Url);
                hostUrl = uri.Host;
            }
            else
            {
                log.DebugFormat("SR is not shared -- redirecting to {0}", localStorageHost.address);
                hostUrl = localStorageHost.address;
            }

            log.DebugFormat("Using {0} for import", hostUrl);

            string result;
            try
            {
                log.DebugFormat("Uploading file '{0}' to VDI '{1}' on SR '{2}'", suppPackFilePath, vdi.Name(), sr.Name());

                HTTP.UpdateProgressDelegate progressDelegate = delegate(int percent)
                {
                    var sr1 = sr;
                    var descr = string.Format(Messages.UPLOAD_PATCH_UPLOADING_TO_SR_PROGRESS_DESCRIPTION, _updateName, sr1.Name(),
                        Util.DiskSizeString(percent * _totalUpdateSize / 100, "F1"), Util.DiskSizeString(_totalUpdateSize));

                    var actionPercent = (int)((totalUploaded * 100 + percent) / totalCount);
                    ByteProgressDescription = descr;
                    Tick(actionPercent, descr);
                };

                Session session = NewSession();
                RelatedTask = Task.create(Session, "uploadTask", hostUrl);

                result = HTTPHelper.Put(progressDelegate, GetCancelling, true, Connection, RelatedTask, ref session,  suppPackFilePath, hostUrl,
                                        (HTTP_actions.put_sss)HTTP_actions.put_import_raw_vdi,
                                        session.opaque_ref, vdiRef.opaque_ref);
            }
            catch (Exception ex)
            {
                log.Error("Failed to import a virtual disk over HTTP", ex);

                if (vdiRef != null)
                {
                    try
                    {
                        log.ErrorFormat("Deleting VDI '{0}' on a best effort basis.", vdiRef);
                        VDI.destroy(Session, vdiRef);
                    }
                    catch (Exception removeEx)
                    {
                        log.Error("Failed to remove VDI.", removeEx);
                    }
                }

                //after having tried to remove the VDI, the original exception is thrown for the UI
                if (ex is TargetInvocationException && ex.InnerException != null)
                    throw ex.InnerException;
                else
                    throw; 
            }
            finally
            {
                Task.destroy(Session, RelatedTask);
                RelatedTask = null;
            }

            if (localStorageHost != null)
                VdiRefsToCleanUp.Add(localStorageHost, vdiRef);
            else // shared SR
                foreach (var server in servers)
                    VdiRefsToCleanUp.Add(server, vdiRef);

            //introduce ISO for Ely and higher
            if (Helpers.ElyOrGreater(Connection))
            {
                try
                {
                    var poolUpdateRef = Pool_update.introduce(Connection.Session, vdiRef);
                    poolUpdate = Connection.WaitForCache(poolUpdateRef);

                    if (poolUpdate == null)
                        throw new Exception(Messages.UPDATE_ERROR_INTRODUCE); // This should not happen, because such case will result in a XAPI Failure. But this code has to be protected at this point.

                    VdiRefsToCleanUp.Clear();
                }
                catch (Failure ex)
                {
                    if (ex.ErrorDescription != null && ex.ErrorDescription.Count > 1 && string.Equals("UPDATE_ALREADY_EXISTS", ex.ErrorDescription[0], StringComparison.InvariantCultureIgnoreCase))
                    {
                        string uuidFound = ex.ErrorDescription[1];

                        poolUpdate = Connection.Cache.Pool_updates.FirstOrDefault(pu => string.Equals(pu.uuid, uuidFound, System.StringComparison.InvariantCultureIgnoreCase));

                        //clean-up the VDI we've just created
                        try
                        {
                            log.ErrorFormat("Deleting VDI '{0}' on a best effor basis.", vdiRef);
                            VDI.destroy(Session, vdiRef);

                            //remove the vdi that have just been cleaned up
                            var remaining = VdiRefsToCleanUp.Where(kvp => kvp.Value != null && kvp.Value.opaque_ref != vdiRef.opaque_ref).ToList();
                            VdiRefsToCleanUp.Clear();
                            remaining.ForEach(rem => VdiRefsToCleanUp.Add(rem.Key, rem.Value));
                        }
                        catch (Exception removeEx)
                        {
                            log.Error("Failed to remove VDI", removeEx);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Upload failed when introducing update from VDI {0} on {1}: {2}", vdi.opaque_ref, Connection, ex.Message);
                    poolUpdate = null;

                    throw;
                }
            }
            else
            {
                poolUpdate = null;
            }

            totalUploaded++;
            Description = String.Format(Messages.SUPP_PACK_UPLOADED, sr.Name());

            foreach (Host host in servers)
                SrUploadedUpdates[host] = sr;

            return result;
        }

        private VDI NewVDI(SR sr)
        {
            VDI vdi = new VDI();
            vdi.Connection = Connection;
            vdi.read_only = false;
            vdi.SR = new XenRef<SR>(sr);
            vdi.virtual_size = _totalUpdateSize;
            vdi.name_label = new FileInfo(suppPackFilePath).Name;
            vdi.name_description = Helpers.ElyOrGreater(Connection) ? Messages.UPDATE_TEMP_VDI_DESCRIPTION : Messages.SUPP_PACK_TEMP_VDI_DESCRIPTION;
            vdi.sharable = false;
            vdi.type = vdi_type.user;
            vdi.SetVmHint("");
            //mark the vdi as being a temporary supp pack iso
            vdi.other_config = new Dictionary<string, string> {{"supp_pack_iso", "true"}};
            return vdi;
        }

        private List<SR> SelectTargetSr()
        {
            /* For ely or greater (update ISOs) we need an SR that can be seen from master;
             * that would be a shared SR or the master's local SR.
             * 
             * For earlier (supplemental packs) we need an SR that can be seen from all hosts;
             * that would be a shared SR, otherwise we have to upload to each hosts's local SR
             * 
             * The selection priority is default SRs over non-default and shared over local.
             */

            SR defaultSr = Pool != null ? Pool.Connection.Resolve(Pool.default_SR) : null;
            
            var serversToConsider = Helpers.ElyOrGreater(Connection)
                ? new List<Host> {Helpers.GetMaster(Connection)}
                : new List<Host>(servers);

            var srList = new List<SR>();

            foreach (var host in serversToConsider)
            {
                var visibleSRs = Connection.Cache.SRs.Where(sr => CanStoreUpdateForHost(sr, host)).ToList();

                if (defaultSr != null && visibleSRs.Contains(defaultSr) && !srList.Contains(defaultSr))
                {
                    srList.Add(defaultSr);
                    continue;
                }

                var sharedSr = visibleSRs.FirstOrDefault(sr => sr.shared);
                if (sharedSr != null && !srList.Contains(sharedSr))
                {
                    srList.Add(sharedSr);
                    continue;
                }

                if (visibleSRs.Count > 0)
                    srList.Add(visibleSRs[0]);
            }

            return srList;
        }

        private bool CanStoreUpdateForHost(SR sr, Host host)
        {
            if (!sr.SupportsVdiCreate())
                return false;

            if (sr.FreeSpace() < _totalUpdateSize)
                return false;

            var canBeSeen = false;
            foreach (var pbdRef in sr.PBDs)
            {
                var pbd = Connection.Resolve(pbdRef);
                if (pbd != null && pbd.currently_attached && pbd.host.opaque_ref == host.opaque_ref)
                {
                    canBeSeen = true;
                    break;
                }
            }
            return canBeSeen;
        }
    }
}
