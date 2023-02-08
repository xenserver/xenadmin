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
using System.Net;
using XenAPI;
using XenAdmin.Network;
using XenCenterLib;


namespace XenAdmin.Actions
{

    public abstract class PatchAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PatchAction(IXenConnection conn, string title) : base(conn, title) { }
        public PatchAction(IXenConnection conn, string title, bool suppress) : base(conn, title, suppress) { }


        protected XenRef<Pool_patch> BringPatchToPoolForHost(Host host, Pool_patch patch)
        {
            // Check the patch exists on the pool this host is connected to
            XenRef<Pool_patch> patch_ref = host.Connection.Cache.FindRef(patch);
            if (patch_ref != null)
                return patch_ref;

            // 1st download patch from the pool that has it (the connection on the xenobject)

            string filename = Path.GetTempFileName();

            try
            {
                Connection = patch.Connection;
                Session = patch.Connection.DuplicateSession();

                try
                {
                    RelatedTask = Task.create(Session, "get_pool_patch_download_task", patch.Connection.Hostname);
                    log.DebugFormat("HTTP GETTING file from {0} to {1}", patch.Connection.Hostname, filename);

                    HTTP_actions.get_pool_patch_download(
                        bytes =>
                        {
                            Tick((int)(100 * (double)bytes / patch.size),
                                string.Format(Messages.DOWNLOADING_PATCH_FROM, patch.Connection.Name,
                                    Util.DiskSizeString(bytes, 1, "F1"), Util.DiskSizeString(patch.size)));
                        },
                        () => XenAdminConfigManager.Provider.ForcedExiting || GetCancelling(),
                        XenAdminConfigManager.Provider.GetProxyTimeout(true),
                        patch.Connection.Hostname,
                        XenAdminConfigManager.Provider.GetProxyFromSettings(Connection),
                        filename, RelatedTask.opaque_ref, Session.opaque_ref, patch.uuid);

                    PollToCompletion();
                }
                catch (Exception e)
                {
                    PollToCompletion(suppressFailures: true);

                    if (e is WebException && e.InnerException is IOException ioe && Win32.GetHResult(ioe) == Win32.ERROR_DISK_FULL)
                        throw new PatchDownloadFailedException(string.Format(Messages.PATCH_DOWNLOAD_FAILED, patch.name_label, patch.Connection.Name), e.InnerException);

                    if (e is CancelledException || e is HTTP.CancelledException || e.InnerException is CancelledException)
                        throw new CancelledException();

                    if (e.InnerException?.Message == "Received error code HTTP/1.1 403 Forbidden\r\n from the server")
                    {
                        // RBAC Failure
                        List<Role> roles = Connection.Session.Roles;
                        roles.Sort();
                        throw new Exception(String.Format(Messages.RBAC_HTTP_FAILURE, roles[0].FriendlyName()), e);
                    }

                    throw new PatchDownloadFailedException(string.Format(Messages.PATCH_DOWNLOAD_FAILED, patch.name_label, patch.Connection.Name), e.InnerException ?? e);
                }
                finally
                {
                    Session = null;
                    Connection = null;
                }

                // Then, put it on the pool that doesn't have it

                Description = String.Format(Messages.UPLOADING_PATCH_TO, host.Name());
                Connection = host.Connection;
                Session = host.Connection.DuplicateSession();

                try
                {
                    RelatedTask = Task.create(Session, "put_pool_patch_upload_task", host.Connection.Hostname);
                    log.DebugFormat("HTTP PUTTING file from {0} to {1}", filename, host.Connection.Hostname);

                    HTTP_actions.put_pool_patch_upload(percent => PercentComplete = percent,
                        () => XenAdminConfigManager.Provider.ForcedExiting || GetCancelling(),
                        XenAdminConfigManager.Provider.GetProxyTimeout(true),
                        host.Connection.Hostname,
                        XenAdminConfigManager.Provider.GetProxyFromSettings(Connection),
                        filename, RelatedTask.opaque_ref, Session.opaque_ref);

                    PollToCompletion();
                    Description = string.Format(Messages.PATCH_UPLOADED, host.Name());
                    return new XenRef<Pool_patch>(Result);
                }
                catch (Exception e)
                {
                    PollToCompletion(suppressFailures: true);

                    if (e is CancelledException || e is HTTP.CancelledException || e.InnerException is CancelledException)
                        throw new CancelledException();
                    throw;
                }
                finally
                {
                    Session = null;
                    Connection = null;
                }
            }
            finally
            {
                File.Delete(filename);
            }
        }
    }

    public class ApplyPatchAction : PatchAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Pool_patch patch;
        private readonly Host host;

        public ApplyPatchAction(Pool_patch patch, Host host)
            : base(host.Connection, string.Format(Messages.UPDATES_WIZARD_APPLYING_UPDATE, patch.Name(), host.Name()))
        {
            this.patch = patch;
            this.host = host;
        }

        protected override void Run()
        {
            SafeToExit = false;

            if (patch.AppliedOn(host) != DateTime.MaxValue)
                return;

            XenRef<Pool_patch> patchRef = BringPatchToPoolForHost(host, patch);

            Description = string.Format(Messages.APPLYING_PATCH, patch.Name(), host.Name());
            log.DebugFormat("Applying update '{0}' to server '{1}'", patch.Name(), host.Name());

            try
            {
                RelatedTask = Pool_patch.async_apply(Session, patchRef, host.opaque_ref);
                PollToCompletion();
            }
            catch (Failure f)
            {
                log.ErrorFormat("Failed to apply patch '{0}' on server '{1}': '{2}'",
                    patch.Name(), host.Name(), string.Join(", ", f.ErrorDescription)); //CA-339237
                throw;
            }

            log.DebugFormat("Applied update '{0}' to server '{1}'. Result: {2}.", patch.Name(), host.Name(), Result);
            Description = string.Format(Messages.PATCH_APPLIED, patch.Name(), host.Name());
        }
    }

    public class PatchDownloadFailedException : ApplicationException
    {
        public PatchDownloadFailedException(string message, Exception innerException) :
            base(message, innerException) { }
    }
}
