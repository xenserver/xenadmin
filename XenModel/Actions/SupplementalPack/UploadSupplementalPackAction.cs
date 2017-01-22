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
using System.Linq;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class UploadSupplementalPackAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<SR> srList = new List<SR>();

        private readonly string suppPackFilePath;
        private readonly long diskSize;
        private readonly List<Host> servers;

        private Pool_update poolUpdate = null;
        public Pool_update PoolUpdate
        {
            get { return poolUpdate; }
        }


        /// <summary>
        /// This constructor is used to upload a single supplemental pack file
        /// </summary>
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
            diskSize = FileSize(suppPackFilePath);
            servers = selectedServers;
        }

        public readonly Dictionary<Host, XenRef<VDI>> VdiRefsToCleanUp = new Dictionary<Host, XenRef<VDI>>();

        private static long FileSize(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Length;
        }

        protected override void Run()
        {
            SafeToExit = false;

            SelectTargetSr();

            if (srList.Count == 0)
                throw new Failure(Failure.OUT_OF_SPACE);

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
            this.Description = String.Format(Messages.SUPP_PACK_UPLOADING_TO, sr.Name);

            String result;
            log.DebugFormat("Creating vdi of size {0} bytes on SR '{1}'", diskSize, sr.Name);

            VDI vdi = NewVDI(sr);
            XenRef<VDI> vdiRef = null;
            try
            {
                vdiRef = VDI.create(Session, vdi);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("{0} {1}", "Failed to create VDI", ex.Message);
                throw;
            }

            log.DebugFormat("Uploading file '{0}' to VDI '{1}' on SR '{2}'", suppPackFilePath, vdi.Name, sr.Name);

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

            try
            {
                HTTP.UpdateProgressDelegate progressDelegate = delegate(int percent)
                {
                    var actionPercent = (int)(((totalUploaded * 100) + percent) / totalCount);
                    Tick(actionPercent, Description);
                };

                Session session = NewSession();
                RelatedTask = Task.create(Session, "uploadTask", hostUrl);

                result = HTTPHelper.Put(progressDelegate, GetCancelling, true, Connection, RelatedTask, ref session,  suppPackFilePath, hostUrl,
                                        (HTTP_actions.put_sss)HTTP_actions.put_import_raw_vdi, 
                                        session.uuid, vdiRef.opaque_ref);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("{0} {1}", "Failed to import a virtual disk over HTTP.", ex.Message);

                if (vdiRef != null)
                {
                    log.DebugFormat("Removing the VDI on a best effort basis.");

                    try
                    {
                        RemoveVDI(Session, vdiRef);
                    }
                    catch (Exception removeEx)
                    {
                        //best effort
                        log.Error("Failed to remove the VDI.", removeEx);
                    }
                }
                
                throw ex; //after having tried to remove the VDI, the original exception is thrown for the UI
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
                            RemoveVDI(Session, vdiRef);

                            //remove the vdi that have just been cleaned up
                            var remaining = VdiRefsToCleanUp.Where(kvp => kvp.Value != null && kvp.Value.opaque_ref != vdiRef.opaque_ref).ToList();
                            VdiRefsToCleanUp.Clear();
                            remaining.ForEach(rem => VdiRefsToCleanUp.Add(rem.Key, rem.Value));
                        }
                        catch
                        { 
                            //best effort cleanup
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
            Description = String.Format(Messages.SUPP_PACK_UPLOADED, sr.Name);
            return result;
        }

        private VDI NewVDI(SR sr)
        {
            VDI vdi = new VDI();
            vdi.Connection = Connection;
            vdi.read_only = false;
            vdi.SR = new XenRef<SR>(sr);
            vdi.virtual_size = diskSize;
            vdi.name_label = new FileInfo(suppPackFilePath).Name;
            vdi.name_description = Helpers.ElyOrGreater(Connection) ? Messages.UPDATE_TEMP_VDI_DESCRIPTION : Messages.SUPP_PACK_TEMP_VDI_DESCRIPTION;
            vdi.sharable = false;
            vdi.type = vdi_type.user;
            vdi.VMHint = "";
            //mark the vdi as being a temporary supp pack iso
            vdi.other_config = new Dictionary<string, string> {{"supp_pack_iso", "true"}};
            return vdi;
        }

        private void RemoveVDI(Session session, XenRef<VDI> vdi)
        {
            try
            {
                log.ErrorFormat("Deleting VDI '{0}'", vdi.opaque_ref);
                VDI.destroy(session, vdi.opaque_ref);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("{0}, {1}", "Failed to remove a vdi", ex.Message);
                throw;
            }
            return;
        }

        private void SelectTargetSr()
        {
            SR defaultSr = Pool != null ? Pool.Connection.Resolve(Pool.default_SR) : null;
            if ((defaultSr != null && defaultSr.shared) && CanCreateVdi(defaultSr))
            {
                srList.Add(defaultSr);
                return;
            }
            
            // no default shared SR where we can upload the file -> find another shared SR
            
            var sharedSr = Connection.Cache.SRs.FirstOrDefault(sr => sr.shared && CanCreateVdi(sr));
            if (sharedSr != null)
            {
                srList.Add(sharedSr);
                return;
            }

            // no shared SR where we can upload the file -> will have to upload on the local SRs
                
            //For Ely or greater, the ISO has to be uploaded to exactly one SR, even if it's not a shared one
            if (Helpers.ElyOrGreater(Connection))
            {
                if (defaultSr != null && CanCreateVdi(defaultSr))
                {
                    srList.Add(defaultSr);
                }
                else
                {
                    var firstSrCanCreateVdi = Connection.Cache.SRs.First(sr => CanCreateVdi(sr));
                    if (firstSrCanCreateVdi != null)
                        srList.Add(firstSrCanCreateVdi);
                }
            }
            else //legacy case (supplemental packs)
            {
                SelectLocalSrs();
            }
        }

        private void SelectLocalSrs()
        {
            foreach (var host in servers)
            {
                // get the list of local SRs where we can create the vdi
                var localSrs = Connection.Cache.SRs.Where(sr => host.Equals(sr.GetStorageHost()) && CanCreateVdi(sr)).ToList();

                // if the default SR is in this list, then select it, otherwise select first SR from the list
                var defaultSr = Host.Connection.Resolve(Pool.default_SR);
                if (localSrs.Contains(defaultSr))
                    srList.Add(defaultSr);
                else if (localSrs.Count > 0)
                    srList.Add(localSrs[0]);
            }
        }

        private bool CanCreateVdi(SR sr)
        {
            return sr.SupportsVdiCreate() && !sr.IsDetached && SrHasEnoughFreeSpace(sr); 
        }

        private bool SrHasEnoughFreeSpace(SR sr)
        {
            return sr.FreeSpace >= diskSize; 
        }
    }
}