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
using System.Threading;
using XenAPI;
using System.Linq;

namespace XenAdmin.Actions
{
    public class DestroyHostAction : AsyncAction
    {
        public DestroyHostAction(Pool pool, Host hostToDestroy)
            : base(pool.Connection, string.Format(Messages.DESTROY_HOST_ACTION_TITLE, hostToDestroy.Name))
        {
            this.Pool = pool;
            this.Host = hostToDestroy;
            this.Description = Messages.WAITING;
            SetRBACPermissions();
        }

        private void SetRBACPermissions()
        {
            AddCommonAPIMethodsToRoleCheck();
            ApiMethodsToRoleCheck.Add("host.destroy");
            ApiMethodsToRoleCheck.Add("sr.forget");
        }

        private bool IsSRDetached(SR sr)
        {
            if (sr == null)
                return true;

            // wait 2 minutes for all SR's PBDs to detach
            const int max = 2 * 60;
            int i = 0;

            while (i++ < max)
            {
                if (!sr.HasPBDs)
                    return true;

                Thread.Sleep(1000);
            }

            return !sr.HasPBDs;
        }

        protected override void Run()
        {
            Description = Messages.DESTROY_HOST_ACTION_DESC;
           
            List<SR> srList = Connection.Cache.SRs.Where(sr => Host.Equals(sr.GetStorageHost()) && sr.IsLocalSR).ToList();
            // number of SRs to forget + 1 host to destroy
            int n = srList.Count + 1;
            double p = 100.0 / n;
            int i = 1;

            RelatedTask = XenAPI.Host.async_destroy(Session, Host.opaque_ref);
            PollToCompletion(0, p);

            Description = Messages.DESTROY_HOST_ACTION_REMOVE_SRS_DESC;
            // remove SRs which belonged to destroyed Host
            foreach (SR sr in srList)
            {
                if (!IsSRDetached(sr))
                {
                    Description = Messages.DESTROY_HOST_ACTION_COMPLETED_SRS_NOT_REMOVED_DESC;
                    return;
                }

                int lo = (int)(i * p);
                int hi = (int)((i + 1) * p);
                RelatedTask = XenAPI.SR.async_forget(Session, sr.opaque_ref);
                PollToCompletion(lo, hi);
                i++;
            }
            Description = Messages.DESTROY_HOST_ACTION_COMPLETED_DESC;
        }
    }
}
