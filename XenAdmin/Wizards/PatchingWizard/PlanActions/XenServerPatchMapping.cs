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
using System.Linq;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Wizards.PatchingWizard.PlanActions
{
    public abstract class HostUpdateMapping
    {
        protected readonly Host CoordinatorHost;
        public readonly List<string> HostsThatNeedEvacuation;

        protected HostUpdateMapping(Host coordinatorHost, List<string> hostsThatNeedEvacuation)
        {
            CoordinatorHost = coordinatorHost ?? throw new ArgumentNullException(nameof(coordinatorHost));
            HostsThatNeedEvacuation = hostsThatNeedEvacuation ?? new List<string>();
        }

        protected bool Matches(Host coordinatorHost)
        {
            return CoordinatorHost != null && coordinatorHost != null && CoordinatorHost.uuid == coordinatorHost.uuid;
        }

        public override bool Equals(object obj)
        {
            return obj is HostUpdateMapping other && Matches(other.CoordinatorHost);
        }

        public override int GetHashCode()
        {
            return CoordinatorHost.GetHashCode();
        }

        public abstract bool IsValid { get; }

        /// <summary>
        /// Refresh the update/patch record based on uuid
        /// </summary>
        public abstract HostUpdateMapping RefreshUpdate();
    }

    public class PoolUpdateMapping : HostUpdateMapping
    {
        public XenServerPatch XenServerPatch { get; }
        public Pool_update PoolUpdate { get; }
        public Dictionary<Host, SR> SrsWithUploadedUpdatesPerHost { get; }

        public PoolUpdateMapping(XenServerPatch xenServerPatch, Pool_update poolUpdate, Host coordinatorHost,
            Dictionary<Host, SR> srsWithUploadedUpdatesPerHost, List<string> hostsThatNeedEvacuation = null)
            : base(coordinatorHost, hostsThatNeedEvacuation)
        {
            XenServerPatch = xenServerPatch ?? throw new ArgumentNullException(nameof(xenServerPatch));
            PoolUpdate = poolUpdate ?? throw new ArgumentNullException(nameof(poolUpdate));
            SrsWithUploadedUpdatesPerHost = srsWithUploadedUpdatesPerHost ?? new Dictionary<Host, SR>();
        }

        public bool Matches(Host coordinatorHost, XenServerPatch xenServerPatch)
        {
            return Matches(coordinatorHost) && XenServerPatch.Equals(xenServerPatch);
        }

        public bool Matches(Host coordinatorHost, XenServerPatch xenServerPatch, Pool_update update)
        {
            return Matches(coordinatorHost, xenServerPatch) && update != null &&
                   string.Equals(update.uuid, PoolUpdate.uuid, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is PoolUpdateMapping other && Matches(other.CoordinatorHost, other.XenServerPatch, other.PoolUpdate);
        }

        public override int GetHashCode()
        {
            return PoolUpdate.GetHashCode() ^ base.GetHashCode();
        }

        public override bool IsValid => PoolUpdate != null && PoolUpdate.opaque_ref != null;

        public override HostUpdateMapping RefreshUpdate()
        {
            if (PoolUpdate != null)
            {
                var update = PoolUpdate.Connection?.Cache.Pool_updates.FirstOrDefault(u =>
                    string.Equals(u.uuid, PoolUpdate.uuid, StringComparison.OrdinalIgnoreCase));

                if (update != null && update.opaque_ref != PoolUpdate.opaque_ref)
                    return new PoolUpdateMapping(XenServerPatch, update, CoordinatorHost,
                        SrsWithUploadedUpdatesPerHost, HostsThatNeedEvacuation);
            }

            return this;
        }
    }

    public class SuppPackMapping : HostUpdateMapping
    {
        public string Path  { get; }
        public Pool_update PoolUpdate { get; }
        public Dictionary<Host, SR> SrsWithUploadedUpdatesPerHost { get; }
        public Dictionary<Host, VDI> SuppPackVdis { get; }

        public SuppPackMapping(string path, Pool_update poolUpdate, Host coordinatorHost,
            Dictionary<Host, SR> srsWithUploadedUpdatesPerHost, Dictionary<Host, VDI> suppPackVdis,
            List<string> hostsThatNeedEvacuation = null)
            : base(coordinatorHost, hostsThatNeedEvacuation)
        {
            Path = !string.IsNullOrEmpty(path) ? path : throw new ArgumentNullException(nameof(path));
            PoolUpdate = poolUpdate;
            SrsWithUploadedUpdatesPerHost = srsWithUploadedUpdatesPerHost ?? new Dictionary<Host, SR>();
            SuppPackVdis = suppPackVdis ?? new Dictionary<Host, VDI>();
        }

        public bool Matches(Host coordinatorHost, string path, Pool_update update = null)
        {
            if (update == null)
                return Matches(coordinatorHost) && Path == path;

            return Matches(coordinatorHost) && Path == path && PoolUpdate != null &&
                   string.Equals(update.uuid, PoolUpdate.uuid, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is SuppPackMapping other && Matches(other.CoordinatorHost, other.Path, other.PoolUpdate);
        }

        public override int GetHashCode()
        {
            if (PoolUpdate == null)
                return Path.GetHashCode() ^ base.GetHashCode();

            return Path.GetHashCode() ^ PoolUpdate.GetHashCode() ^ base.GetHashCode();
        }

        public override bool IsValid
        {
            get
            {
                if (PoolUpdate == null)
                    return true;

                return PoolUpdate.opaque_ref != null;
            }
        }

        public override HostUpdateMapping RefreshUpdate()
        {
            if (PoolUpdate != null)
            {
                var update = CoordinatorHost.Connection?.Cache.Pool_updates.FirstOrDefault(u =>
                    string.Equals(u.uuid, PoolUpdate.uuid, StringComparison.OrdinalIgnoreCase));

                if (update != null && update.opaque_ref != PoolUpdate.opaque_ref)
                    return new SuppPackMapping(Path, update, CoordinatorHost, SrsWithUploadedUpdatesPerHost,
                        SuppPackVdis, HostsThatNeedEvacuation);
            }

            return this;
        }
    }
}
