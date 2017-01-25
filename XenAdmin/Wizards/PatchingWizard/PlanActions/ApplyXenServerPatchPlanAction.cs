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
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public class ApplyXenServerPatchPlanAction : PlanActionWithSession
    {
        private readonly Host host;
        private readonly XenServerPatch xenServerPatch;
        private readonly List<PoolPatchMapping> mappings;

        public ApplyXenServerPatchPlanAction(Host host, XenServerPatch xenServerPatch, List<PoolPatchMapping> mappings)
            : base(host.Connection, string.Format(Messages.UPDATES_WIZARD_APPLYING_UPDATE, xenServerPatch.Name, host.Name))
        {
            this.host = host;
            this.xenServerPatch = xenServerPatch;
            this.mappings = mappings;
        }

        protected override void RunWithSession(ref Session session)
        {

            var master = Helpers.GetMaster(host.Connection);
            var mapping = mappings.Find(m => m.XenServerPatch.Equals(xenServerPatch)
                                             && m.MasterHost != null && master != null && m.MasterHost.uuid == master.uuid);

            if (mapping != null && (mapping.Pool_patch != null || mapping.Pool_update != null))
            {
                XenRef<Task> task = null;

                if (mapping.Pool_patch != null)
                {
                    task = Pool_patch.async_apply(session, mapping.Pool_patch.opaque_ref, host.opaque_ref);
                }
                else
                {
                    task = Pool_update.async_apply(session, mapping.Pool_update.opaque_ref, host.opaque_ref);
                }

                PollTaskForResultAndDestroy(Connection, ref session, task);
            }
            else
            {
                if (xenServerPatch != null && master != null)
                    log.ErrorFormat("Mapping not found for patch {0} on master {1}", xenServerPatch.Uuid, master.uuid);

                throw new Exception("Pool_patch or Pool_update not found.");
            }
        }
    }
}