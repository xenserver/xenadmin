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
using System.IO;
using CommandLib;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenCenterLib.Archive;
using XenCenterLib;

namespace XenAdmin.Actions
{
    public class ExportVmAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _filename;
        private readonly bool _verify;
        private readonly bool _preservePowerState;

        /// <summary>
        /// RBAC dependencies needed to import appliance/export an appliance/import disk image.
        /// </summary>
        public static RbacMethodList StaticRBACDependencies
        {
            get
            {
                var list = new RbacMethodList("task.create", "http/get_export");
                list.AddRange(Role.CommonTaskApiList);
                list.AddRange(Role.CommonSessionApiList);
                return list;
            }
        }

        public ExportVmAction(IXenConnection connection, Host host, VM vm, string filename, bool verify, bool preservePowerState = false)
            : base(connection, string.Empty, string.Empty)
        {
            Pool = Helpers.GetPool(vm.Connection);
            Host = host;
            VM = vm;

            _filename = filename;
            _verify = verify;
            _preservePowerState = preservePowerState;

            Title = string.Format(Messages.EXPORT_VM_TITLE, vm.Name(), Helpers.GetName(connection));
            Description = Messages.ACTION_EXPORT_DESCRIPTION_PREPARING;

            ApiMethodsToRoleCheck.AddRange(StaticRBACDependencies);
        }

        protected override void Run()
        {
            SafeToExit = false;

            log.DebugFormat("Exporting {0} to {1}", VM.Name(), _filename);

            var taskRef = Task.create(Session, "export", $"Exporting {VM.Name()} to backup file");

            var totalSize = VM.GetTotalSize();
            var pollingRange = _verify ? 50 : 100;

            try
            {
                long oldRead = 0;
                Description = string.Format(Messages.EXPORTING_VM, VM.Name(), Path.GetFileName(_filename),
                    Util.DiskSizeString(0, 2, "F2"), Util.DiskSizeString(totalSize));

                HTTP_actions.get_export(b =>
                    {
                        if (b - oldRead > 256 * Util.BINARY_MEGA)
                        {
                            oldRead = b;
                            int percent = (int)((float)b / totalSize * pollingRange);

                            Tick(percent, string.Format(Messages.EXPORTING_VM, VM.Name(), Path.GetFileName(_filename),
                                Util.DiskSizeString(b, 2, "F2"), Util.DiskSizeString(totalSize)));
                        }
                    },
                    () => Cancelling, XenAdminConfigManager.Provider.GetProxyTimeout(true),
                    Connection.Hostname, XenAdminConfigManager.Provider.GetProxyFromSettings(Connection),
                    _filename, taskRef, Connection.Session.opaque_ref, VM.uuid, null, _preservePowerState);

                if (_verify)
                {
                    if (Cancelling)
                        throw new CancelledException();

                    oldRead = 0;
                    long read = 0;
                    long fileSize = new FileInfo(_filename).Length;

                    using (FileStream fs = new FileStream(_filename, FileMode.Open, FileAccess.Read))
                    {
                        log.DebugFormat("Verifying export of {0} in {1}", VM.Name(), _filename);
                        Description = string.Format(Messages.ACTION_EXPORT_VERIFY, 0);

                        new Export().verify(fs, null,
                            () => Cancelling,
                            size =>
                            {
                                read += size;

                                if (read - oldRead > 256 * Util.BINARY_MEGA)
                                {
                                    oldRead = read;
                                    int percent = (int)(pollingRange + (float)read / fileSize * pollingRange);

                                    Tick(percent, string.Format(Messages.ACTION_EXPORT_VERIFY, (int)((float)read / fileSize * 100)));
                                }
                            });
                    }
                }

                log.InfoFormat("Export of VM {0} successful", VM.Name());
                Description = Messages.ACTION_EXPORT_DESCRIPTION_SUCCESSFUL;
            }
            catch (HTTP.CancelledException)
            {
                log.InfoFormat("Export of VM {0} cancelled", VM.Name());
                Description = Messages.ACTION_EXPORT_DESCRIPTION_CANCELLED;
                throw new CancelledException();
            }
            catch (Exception ex)
            {
                log.Warn($"Export of VM {VM.Name()} failed", ex);

                if (ex is HeaderChecksumFailed || ex is FormatException)
                    Description = Messages.ACTION_EXPORT_DESCRIPTION_HEADER_CHECKSUM_FAILED;
                else if (ex is BlockChecksumFailed)
                    Description = Messages.ACTION_EXPORT_DESCRIPTION_BLOCK_CHECKSUM_FAILED;
                else if (ex is IOException && Win32.GetHResult(ex) == Win32.ERROR_DISK_FULL)
                    Description = Messages.ACTION_EXPORT_DESCRIPTION_DISK_FULL;
                else if (ex is Failure failure && failure.ErrorDescription[0] == Failure.VDI_IN_USE)
                    Description = Messages.ACTION_EXPORT_DESCRIPTION_VDI_IN_USE;
                else
                    Description = Messages.ACTION_EXPORT_DESCRIPTION_FAILED;

                var fi = new FileInfo(_filename);
                log.DebugFormat("Progress of the action until exception: {0}. Size file exported until exception: {1}",
                    PercentComplete, fi.Length);

                try
                {
                    using (Stream stream = new FileStream(_filename, FileMode.Open, FileAccess.Read))
                    using (var iterator = ArchiveFactory.Reader(ArchiveFactory.Type.Tar, stream))
                    {
                        while (iterator.HasNext())
                            log.DebugFormat("Tar entry: {0} {1}", iterator.CurrentFileName(), iterator.CurrentFileSize());
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                throw new Exception(Description);
            }
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling && !IsCompleted;
        }

        protected override void CleanOnError()
        {
            try
            {
                log.DebugFormat("Deleting {0}", _filename);
                File.Delete(_filename);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
