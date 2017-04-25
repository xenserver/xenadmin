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

using System.Collections.Generic;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Actions
{
    /// <summary>
    /// Live migrate a VDI
    /// </summary>
    public class MigrateVirtualDiskAction : PureAsyncAction
    {
        private readonly VDI vdi;

        public MigrateVirtualDiskAction(IXenConnection connection, VDI vdi, SR sr)
            : base(connection, string.Format(Messages.ACTION_MOVING_VDI_TITLE, Helpers.GetName(vdi), Helpers.GetName(sr)))
        {
            Description = Messages.ACTION_PREPARING;
            this.vdi = vdi;
            SR = sr;
        }

        protected override void Run()
        {
            try
            {
                Title = Description = string.Format(Messages.ACTION_MOVING_VDI_STATUS, Helpers.GetName(vdi));
                RelatedTask = VDI.async_pool_migrate(Session, vdi.opaque_ref, SR.opaque_ref, new Dictionary<string, string>());
                PollToCompletion();
            }
            catch (CancelledException)
            {
                Description = string.Format(Messages.ACTION_VM_MIGRATE_CANCELLED, vdi.Name);
                throw;
            }
            catch (Failure boo)
            {
                Description = boo.Message;
                throw;
            }

            Description = Messages.ACTION_VM_MIGRATED;
        }
    }
}
