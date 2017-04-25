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
using System.Linq;
using XenAPI;

namespace XenAdmin.Actions
{
    public class DeletePvsSiteAction : PureAsyncAction
    {
        private readonly PVS_site pvsSite;

        public DeletePvsSiteAction(PVS_site pvsSite)
            : base(pvsSite.Connection, string.Format(Messages.ACTION_DELETE_PVS_SITE_TITLE, pvsSite.Name.Ellipsise(50)),
                    Messages.ACTION_DELETE_PVS_SITE_DESCRIPTION, false)
        {
            System.Diagnostics.Trace.Assert(pvsSite != null);
            this.pvsSite = pvsSite;
        }
        
        protected override void Run()
        {
            // check if there are any running proxies
            var pvsProxies = Connection.Cache.PVS_proxies.Where(s => s.site.opaque_ref == pvsSite.opaque_ref).ToList();
            if (pvsProxies.Count > 0)
            {
                throw new Failure(Failure.PVS_SITE_CONTAINS_RUNNING_PROXIES);
            }

            // delete PVS_servers
            var pvsServers = Connection.Cache.PVS_servers.Where(s => s.site.opaque_ref == pvsSite.opaque_ref).ToList();
            int inc = pvsServers.Count > 0 ? 50 / pvsServers.Count : 50;
            foreach (var pvsServer in pvsServers)
            {
                RelatedTask = PVS_server.async_forget(Session, pvsServer.opaque_ref);
                PollToCompletion(PercentComplete, PercentComplete + inc);
            }

            RelatedTask = PVS_site.async_forget(Session, pvsSite.opaque_ref);
            PollToCompletion();
            Description = Messages.ACTION_DELETE_PVS_SITE_DONE;
            PercentComplete = 100;
        }
    }
}
