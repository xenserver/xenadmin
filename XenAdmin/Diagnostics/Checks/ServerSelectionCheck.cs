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
using System.Linq;
using XenAdmin.Alerts;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    class ServerSelectionCheck : Check
    {
        private readonly Pool_update update;
        private readonly XenServerPatchAlert alert;
        private readonly bool isNewVersionUpdate;
        private readonly Pool pool;
        private readonly List<Host> selectedServers;

        public ServerSelectionCheck(Pool pool, Pool_update update, List<Host> selectedServers)
            : base(Helpers.GetMaster(pool.Connection))
        {
            this.pool = pool;
            this.update = update;
            this.selectedServers = selectedServers;
            isNewVersionUpdate = false;
        }

        public ServerSelectionCheck(Pool pool, XenServerPatchAlert alert, List<Host> selectedServers)
            : base(Helpers.GetMaster(pool.Connection))
        {
            this.pool = pool;
            this.alert = alert;
            this.selectedServers = selectedServers;
            isNewVersionUpdate = alert != null && alert.NewServerVersion != null;
        }

        protected override Problem RunCheck()
        {
            if (!Host.IsLive())
                return new HostNotLiveWarning(this, Host);

            if (isNewVersionUpdate)
            {
                if (pool.IsPoolFullyUpgraded())
                {
                    //Must select all hosts in a homogeneous pool
                    if (pool.Connection.Cache.Hosts.Any(h => !selectedServers.Contains(h)))
                        return new ServerSelectionProblem(this, pool);
                    else
                        return null;
                }
                else
                {
                    //For mixed pool, precheck blocks update to a new version which is higher than master without selecting it
                    if (Helpers.productVersionCompare(alert.NewServerVersion.Version.ToString(), Host.ProductVersion()) > 0 &&
                        !selectedServers.Contains(Host))
                        return new MasterVersionNotCompatibleProblem(this, pool);
                    //Otherwise, skip the precheck for a mixed pool and issue warning,
                    //because the version update may not be compatible to all servers.
                    else
                        return new MixedPoolServerSelectionWarning(this, pool);
                }
            }

            if (update == null || !update.EnforceHomogeneity()) 
                return null;

            //If mixed pool, skip the precheck and issue warning, because the update may not be compatible to all servers.
            if (!pool.IsPoolFullyUpgraded())
                return new MixedPoolServerSelectionWarning(this, pool);
            
            if (pool.Connection.Cache.Hosts.Any(h => !update.AppliedOn(h) && !selectedServers.Contains(h)))
                return new ServerSelectionProblem(this, pool);

            return null;
        }

        public override string Description
        {
            get { return Messages.SERVER_SELECTION_CHECK_DESCRIPTION; }
        }
    }
}

