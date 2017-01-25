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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public class PoolPatchMapping
    {
        public XenServerPatch XenServerPatch { get; private set; }
        public Pool_patch Pool_patch { get; private set; }
        public Pool_update Pool_update { get; private set; }
        public Host MasterHost { get; private set; }

        public PoolPatchMapping(XenServerPatch xenServerPatch, Pool_update pool_update, Host masterHost)
        {
            if (xenServerPatch == null)
                throw new ArgumentNullException("xenServerPatch");

            if (pool_update == null)
                throw new ArgumentNullException("pool_update");

            if (masterHost == null)
                throw new ArgumentNullException("masterHost");

            this.XenServerPatch = xenServerPatch;
            this.Pool_update = pool_update;
            this.MasterHost = masterHost;
        }

        public PoolPatchMapping(XenServerPatch xenServerPatch, Pool_patch pool_patch, Host masterHost)
        {
            if (xenServerPatch == null)
                throw new ArgumentNullException("xenServerPatch");

            if (pool_patch == null)
                throw new ArgumentNullException("pool_patch");

            if (masterHost == null)
                throw new ArgumentNullException("masterHost");

            this.XenServerPatch = xenServerPatch;
            this.Pool_patch = pool_patch;
            this.MasterHost = masterHost;
        }

        public override bool Equals(object obj)
        {
            var that = obj as PoolPatchMapping;

            if (that == null)
                return false;

            return
                this.XenServerPatch != null && this.XenServerPatch.Equals(that.XenServerPatch)
                && (this.Pool_patch != null && this.Pool_patch.Equals(that.Pool_patch) || this.Pool_update != null && this.Pool_update.Equals(that.Pool_update))
                && this.MasterHost != null && that.MasterHost != null & this.MasterHost.uuid == that.MasterHost.uuid;
        }

        public override int GetHashCode()
        {
            return XenServerPatch.GetHashCode() ^ Pool_patch.GetHashCode() ^ MasterHost.GetHashCode();
        }
    }
}
