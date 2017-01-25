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
using System.Text;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions
{
    public class HostBackupRestoreAction : AsyncAction
    {
        public enum HostBackupRestoreType {backup, restore};

        private readonly HostBackupRestoreType type; 
        private readonly string filename;

        public HostBackupRestoreAction(Host host, HostBackupRestoreType type, string filename)
            : base(host.Connection, type == HostBackupRestoreType.backup ?
            string.Format(Messages.BACKINGUP_HOST, host.Name) :
            string.Format(Messages.RESTORING_HOST, host.Name))
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
            try
            {
                switch (type)
                {
                    case HostBackupRestoreType.backup:
                        this.Description = String.Format(Messages.BACKINGUP_HOST_WITH_DATA, Host.Name, Util.MemorySizeStringSuitableUnits(0, false));

                        LogDescriptionChanges = false;
                        try
                        {
                            HTTPHelper.Get(this, true, DataReceived, filename, Host.address,
                                (HTTP_actions.get_ss)HTTP_actions.get_host_backup, Session.uuid);
                        }
                        finally
                        {
                            LogDescriptionChanges = true;
                        }

                        this.Description = String.Format(Messages.HOST_BACKEDUP, Host.Name);
                        break;

                    case HostBackupRestoreType.restore:
                        this.Description = String.Format(Messages.RESTORING_HOST, Host.Name);

                        HTTPHelper.Put(this, true, filename, Host.address,
                            (HTTP_actions.put_ss)HTTP_actions.put_host_restore, Session.uuid);

                        this.Description = String.Format(Messages.HOST_RESTORED, Host.Name);
                        break;
                }
            }
            catch (HTTP.CancelledException)
            {
                Description = Messages.CANCELLED_BY_USER;
                throw new CancelledException();
            }
        }

        private void DataReceived(long bytes)
        {
            this.Description = String.Format(Messages.BACKINGUP_HOST_WITH_DATA, Host.Name, Util.MemorySizeStringSuitableUnits(bytes, false));
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = !Cancelling;
        }
    }
}
