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
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;

namespace XenAPI
{
    public partial class Pool_patch : IComparable<Pool_patch>
    {
        public override string Name
        {
            get { return name_label; }
        }

        public override string Description
        {
            get { return name_description; }
        }

        private void AppliedTo(List<XenRef<Host>> appliedHosts)
        {
            foreach (Host_patch host_patch in Connection.ResolveAll(host_patches))
                appliedHosts.Add(host_patch.host);
        }

        public List<XenRef<Host>> AppliedTo(IEnumerable<IXenConnection> connections)
        {
            IEnumerable<Pool_patch> patches = GetAllPatches(uuid, connections);
            List<XenRef<Host>> hosts = new List<XenRef<Host>>();

            foreach (Pool_patch patch in patches)
            {
                patch.AppliedTo(hosts);
            }

            return hosts;
        }

        public bool IsAppliedTo(Host host,IEnumerable<IXenConnection> connections)
        {
            List<Host> hostsApplied = new List<Host>();
            foreach (IXenConnection xenConnection in connections)
            {
                hostsApplied.AddRange(xenConnection.ResolveAll(AppliedTo(connections)));
            }
            if (hostsApplied.Contains(host))
                return true;
            return false;
        }

        public List<Host> HostsAppliedTo()
        {
            List<Host> hosts = new List<Host>();
            foreach (Host_patch patch in Connection.ResolveAll(host_patches))
            {
                hosts.Add(Connection.Resolve(patch.host));
            }
            return hosts;
        }

        public DateTime AppliedOn(Host host)
        {
            var hostPatches = host.Connection.ResolveAll(host.patches);

            if (hostPatches != null)
            {
                foreach (Host_patch hostPatch in hostPatches)
                {
                    Pool_patch patch = host.Connection.Resolve(hostPatch.pool_patch);

                    if (patch != null && string.Equals(patch.uuid, uuid, StringComparison.OrdinalIgnoreCase))
                        return hostPatch.timestamp_applied;
                }
            }

            return DateTime.MaxValue;
        }

        private static bool FindPatch(Pool_patch patch,
            IEnumerable<Pool_patch> patches)
        {
            foreach (Pool_patch p in patches)
            {
                if (string.Equals(p.uuid, patch.uuid, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public static List<Pool_patch> GetAllThatApply(Host host,IEnumerable<IXenConnection> connections)
        {
            List<Pool_patch> patches = new List<Pool_patch>();

            Pool pool = Helpers.GetPoolOfOne(host.Connection);
            if (pool == null || pool.RollingUpgrade)
            {
                foreach (Host_patch hostPatch in host.Connection.ResolveAll(host.patches))
                {
                    Pool_patch patch = host.Connection.Resolve(hostPatch.pool_patch);
                    if (patch == null)
                        continue;

                    patches.Add(patch);
                }

                return patches;
            }

            patches.AddRange(host.Connection.Cache.Pool_patches);

            foreach (IXenConnection connection in connections)
                foreach (Host otherHost in connection.Cache.Hosts)
                {
                    // ignore pools in rolling upgrade
                    pool = Helpers.GetPoolOfOne(connection);
                    if (pool != null && pool.RollingUpgrade)
                        continue;

                    // ignore this host
                    if (otherHost.opaque_ref == host.opaque_ref)
                        continue;

                    // ignore hosts of different version
                    if (Helpers.productVersionCompare(Helpers.HostProductVersion(otherHost),
                            Helpers.HostProductVersion(host)) != 0)
                        continue;

                    foreach (Host_patch hostPatch in connection.ResolveAll(otherHost.patches))
                    {
                        Pool_patch patch = connection.Resolve(hostPatch.pool_patch);
                        if (patch == null)
                            continue;

                        if (FindPatch(patch, patches))
                            continue;

                        patches.Add(patch);
                    }
                }

            return patches;
        }
        private static IEnumerable<Pool_patch> GetAllPatches(string uuid,IEnumerable<IXenConnection> connections)
        {
            List<Pool_patch> patches = new List<Pool_patch>();

            foreach (IXenConnection connection in connections)
            {
                foreach (Pool_patch patch in connection.Cache.Pool_patches)
                {
                    if (string.Equals(patch.uuid, uuid, StringComparison.OrdinalIgnoreCase))
                        patches.Add(patch);
                }
            }

            return patches;
        }

        public static IEnumerable<Pool_patch> GetAll(IEnumerable<IXenConnection> connections)
        {
            List<Pool_patch> patches = new List<Pool_patch>();

            foreach (IXenConnection connection in connections)
            {
                foreach (Pool_patch patch in connection.Cache.Pool_patches)
                {
                    if (!FindPatch(patch, patches))
                        patches.Add(patch);
                }
            }

            return patches;
        }

    }
}
