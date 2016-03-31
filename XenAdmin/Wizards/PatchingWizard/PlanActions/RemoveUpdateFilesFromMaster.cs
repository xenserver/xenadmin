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

using System.Collections.Generic;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    class RemoveUpdateFilesFromMaster : PlanActionWithSession
    {
        private readonly List<PoolPatchMapping> patchMappings = new List<PoolPatchMapping>();
        private readonly Host master = null;

        public RemoveUpdateFilesFromMaster(Host master, List<PoolPatchMapping> patchMappings)
            : base(master.Connection, string.Format(Messages.UPDATES_WIZARD_REMOVING_UPDATES_FROM_POOL, master.Name))
        {
            this.master = master;
            this.patchMappings = patchMappings;
        }

        protected override void RunWithSession(ref Session session)
        {
            foreach (var m in patchMappings)
            {
                if (m.Host == master)
                {
                    var task = Pool_patch.async_pool_clean(session, m.Pool_patch.opaque_ref);
                    PollTaskForResultAndDestroy(Connection, ref session, task);
                }
            }

        }
    }
}
