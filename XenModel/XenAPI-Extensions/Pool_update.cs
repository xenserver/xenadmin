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
using System.Collections.Generic;
using XenAdmin.Core;
using System.Linq;

namespace XenAPI
{
    public partial class Pool_update : IComparable<Pool_update>
    {
        public override string Name()
        {
            return name_label;
        }

        public override string Description()
        {
            return name_description;
        }

        public bool AppliedOn(Host host)
        {
            var hostUpdates = host.Connection.ResolveAll(host.updates);

            if (hostUpdates != null)
            {
                foreach (var hostUpdate in hostUpdates)
                {
                    if (hostUpdate != null && string.Equals(hostUpdate.uuid, uuid, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        public List<Host> AppliedOnHosts()
        {
            return Connection.Cache.Hosts.Where(AppliedOn).ToList();
        }

        private const string ENFORCE_HOMOGENEITY = "enforce_homogeneity";

        public bool EnforceHomogeneity()
        {
            if (Helpers.InvernessOrGreater(Connection))
                return enforce_homogeneity;
            var poolPatchOfUpdate = Connection.Cache.Pool_patches.FirstOrDefault(p => p.pool_update != null && p.pool_update.opaque_ref == opaque_ref);
            return poolPatchOfUpdate != null && BoolKey(poolPatchOfUpdate.other_config, ENFORCE_HOMOGENEITY);
        }
    }
}
