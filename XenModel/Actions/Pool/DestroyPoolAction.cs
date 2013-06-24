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

namespace XenAdmin.Actions
{
    public class DestroyPoolAction: PureAsyncAction
    {
        public DestroyPoolAction(XenAPI.Pool pool)
            : base(pool.Connection, string.Format(Messages.DESTROYING_POOL, pool.Name))
        {

            System.Diagnostics.Trace.Assert(pool != null);
            Pool = pool;
            this.Description = Messages.WAITING;
        }

        protected override void Run()
        {

            this.Description = Messages.POOLCREATE_DESTROYING;
            int n = Connection.Cache.HostCount;
            string master = Pool.master;
            double p = 100.0 / n;  // We have n - 1 to eject, and then 1 to rename.
            int i = 0;
            foreach (XenAPI.Host host in Connection.Cache.Hosts)
            {
                if (host.opaque_ref != master)
                {
                    int lo = (int)(i * p);
                    int hi = (int)((i + 1) * p);
                    RelatedTask = XenAPI.Pool.async_eject(Session, host.opaque_ref);
                    PollToCompletion(lo, hi);
                    i++;
                }
            }
            XenAPI.Pool.set_name_label(Session, Pool.opaque_ref, "");
            XenAPI.Pool.set_name_description(Session, Pool.opaque_ref, "");
            this.Description = Messages.POOLCREATE_DESTROYED;
        }
        
    }
}