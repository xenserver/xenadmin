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
using XenCenterLib;


namespace XenAdmin.Actions
{
    public class HostBackupRestoreAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public enum HostBackupRestoreType {backup, restore};

        private readonly HostBackupRestoreType type; 
        private readonly string filename;

        public HostBackupRestoreAction(Host host, HostBackupRestoreType type, string filename)
            : base(host.Connection, type == HostBackupRestoreType.backup ?
            string.Format(Messages.BACKINGUP_HOST, host.Name()) :
            string.Format(Messages.RESTORING_HOST, host.Name()))
        {
            #region RBAC Dependencies
            switch (type)
            {
                case HostBackupRestoreType.backup:
                    ApiMethodsToRoleCheck.Add("http/get_host_backup");
                    break;
                case HostBackupRestoreType.restore:
                    ApiMethodsToRoleCheck.Add("http/put_host_restore");
                    break;
            }
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            #endregion
            Host = host;
            this.filename = filename;
            this.type = type;
            if (type == HostBackupRestoreType.backup)
                this.ShowProgress = false;  // CA-13065, CA-66475
        }

        protected override void Run()
        {
            LogDescriptionChanges = false;

            try
            {
                switch (type)
                {
                    case HostBackupRestoreType.backup:
                        Description = string.Format(Messages.BACKINGUP_HOST, Host.Name());
                        RelatedTask = Task.create(Session, "get_host_backup_task", Host.address);
                        log.DebugFormat("HTTP GETTING file from {0} to {1}", Host.address, filename);

                        HTTP_actions.get_host_backup(DataReceived,
                            () => XenAdminConfigManager.Provider.ForcedExiting || GetCancelling(),
                            XenAdminConfigManager.Provider.GetProxyTimeout(true),
                            Host.address,
                            XenAdminConfigManager.Provider.GetProxyFromSettings(Connection),
                            filename, RelatedTask.opaque_ref, Session.opaque_ref);

                        PollToCompletion();
                        Description = string.Format(Messages.HOST_BACKEDUP, Host.Name());
                        break;

                    case HostBackupRestoreType.restore:
                        Description = string.Format(Messages.RESTORING_HOST, Host.Name());
                        RelatedTask = Task.create(Session, "put_host_restore_task", Host.address);
                        log.DebugFormat("HTTP PUTTING file from {0} to {1}", filename, Host.address);

                        HTTP_actions.put_host_restore(percent => PercentComplete = percent,
                            () => XenAdminConfigManager.Provider.ForcedExiting || GetCancelling(),
                            XenAdminConfigManager.Provider.GetProxyTimeout(true),
                            Host.address,
                            XenAdminConfigManager.Provider.GetProxyFromSettings(Connection),
                            filename, RelatedTask.opaque_ref, Session.opaque_ref);

                        PollToCompletion();
                        Description = string.Format(Messages.HOST_RESTORED, Host.Name());
                        break;
                }
            }
            catch (Exception e)
            {
                PollToCompletion(suppressFailures: true);

                if (e is WebException && e.InnerException is IOException ioe && Win32.GetHResult(ioe) == Win32.ERROR_DISK_FULL)
                    throw e.InnerException;

                if (e is CancelledException || e is HTTP.CancelledException || e.InnerException is CancelledException)
                    throw new CancelledException();

                if (e.InnerException?.Message == "Received error code HTTP/1.1 403 Forbidden\r\n from the server")
                {
                    // RBAC Failure
                    List<Role> roles = Connection.Session.Roles;
                    roles.Sort();
                    throw new Exception(String.Format(Messages.RBAC_HTTP_FAILURE, roles[0].FriendlyName()), e);
                }

                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
            finally
            {
                LogDescriptionChanges = true;
            }
        }

        private void DataReceived(long bytes)
        {
            Description = string.Format(Messages.BACKINGUP_HOST_WITH_DATA, Host.Name(), Util.MemorySizeStringSuitableUnits(bytes, false));
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling;
        }
    }
}
