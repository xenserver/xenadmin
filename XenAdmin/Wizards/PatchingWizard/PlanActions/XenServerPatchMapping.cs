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

    
    public abstract class XenServerPatchMapping : HostUpdateMapping
    {
        public readonly XenServerPatch XenServerPatch;

        protected XenServerPatchMapping(XenServerPatch xenServerPatch, Host coordinatorHost, List<string> hostsThatNeedEvacuation)
            : base(coordinatorHost, hostsThatNeedEvacuation)
        {
            XenServerPatch = xenServerPatch ?? throw new ArgumentNullException(nameof(xenServerPatch));
        }

        public bool Matches(Host coordinatorHost, XenServerPatch xenServerPatch)
        {
            return Matches(coordinatorHost) && XenServerPatch.Equals(xenServerPatch);
        }

        public override bool Equals(object obj)
        {
            return obj is XenServerPatchMapping other && Matches(other.CoordinatorHost, other.XenServerPatch);
        }

        public override int GetHashCode()
        {
            return XenServerPatch.GetHashCode() ^ base.GetHashCode();
        }
    }


    public class PoolPatchMapping : XenServerPatchMapping
    {
        public readonly Pool_patch Pool_patch;

        public PoolPatchMapping(XenServerPatch xenServerPatch, Pool_patch poolPatch, Host coordinatorHost, List<string> hostsThatNeedEvacuation = null)
            : base(xenServerPatch, coordinatorHost, hostsThatNeedEvacuation)
        {
            Pool_patch = poolPatch ?? throw new ArgumentNullException(nameof(poolPatch));
        }

        public bool Matches(Host coordinatorHost, XenServerPatch xenServerPatch, Pool_patch patch )
        {
            return Matches(coordinatorHost, xenServerPatch) && patch != null &&
                   string.Equals(patch.uuid, Pool_patch.uuid, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is PoolPatchMapping other && Matches(other.CoordinatorHost, other.XenServerPatch, other.Pool_patch);
        }

        public override int GetHashCode()
        {
            return Pool_patch.GetHashCode() ^ base.GetHashCode();
        }

        public override bool IsValid
        {
            get { return Pool_patch != null && Pool_patch.opaque_ref != null; }
        }

        public override HostUpdateMapping RefreshUpdate()
        {
            if (Pool_patch != null)
            {
                var patch = CoordinatorHost.Connection?.Cache.Pool_patches.FirstOrDefault(u =>
                    string.Equals(u.uuid, Pool_patch.uuid, StringComparison.OrdinalIgnoreCase));

                if (patch != null && patch.opaque_ref != Pool_patch.opaque_ref)
                    return new PoolPatchMapping(XenServerPatch, patch, CoordinatorHost, HostsThatNeedEvacuation);
            }

            return this;
        }
    }

    
    public class PoolUpdateMapping : XenServerPatchMapping
    {
        public readonly Pool_update Pool_update;
        public readonly Dictionary<Host, SR> SrsWithUploadedUpdatesPerHost;

        public PoolUpdateMapping(XenServerPatch xenServerPatch, Pool_update poolUpdate, Host coordinatorHost,
            Dictionary<Host, SR> srsWithUploadedUpdatesPerHost, List<string> hostsThatNeedEvacuation = null)
            : base(xenServerPatch, coordinatorHost, hostsThatNeedEvacuation)
        {
            Pool_update = poolUpdate ?? throw new ArgumentNullException(nameof(poolUpdate));
            SrsWithUploadedUpdatesPerHost = srsWithUploadedUpdatesPerHost ?? new Dictionary<Host, SR>();
        }

        public bool Matches(Host coordinatorHost, XenServerPatch xenServerPatch, Pool_update update)
        {
            return Matches(coordinatorHost, xenServerPatch) && update != null &&
                   string.Equals(update.uuid, Pool_update.uuid, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is PoolUpdateMapping other && Matches(other.CoordinatorHost, other.XenServerPatch, other.Pool_update);
        }

        public override int GetHashCode()
        {
            return Pool_update.GetHashCode() ^ base.GetHashCode();
        }

        public override bool IsValid
        {
            get { return Pool_update != null && Pool_update.opaque_ref != null; }
        }

        public override HostUpdateMapping RefreshUpdate()
        {
            if (Pool_update != null)
            {
                var update = Pool_update.Connection?.Cache.Pool_updates.FirstOrDefault(u =>
                    string.Equals(u.uuid, Pool_update.uuid, StringComparison.OrdinalIgnoreCase));

                if (update != null && update.opaque_ref != Pool_update.opaque_ref)
                    return new PoolUpdateMapping(XenServerPatch, update, CoordinatorHost,
                        SrsWithUploadedUpdatesPerHost, HostsThatNeedEvacuation);
            }

            return this;
        }
    }


    public class OtherLegacyMapping : HostUpdateMapping
    {
        public readonly string Path;
        public readonly Pool_patch Pool_patch;

        public OtherLegacyMapping(string path, Pool_patch pool_patch, Host coordinatorHost, List<string> hostsThatNeedEvacuation = null)
            : base(coordinatorHost, hostsThatNeedEvacuation)
        {
            Path = !string.IsNullOrEmpty(path) ? path : throw new ArgumentNullException(nameof(path));
            Pool_patch = pool_patch;
        }

        public bool Matches(Host coordinatorHost, string path, Pool_patch patch = null)
        {
            if (patch == null)
                return Matches(coordinatorHost) && Path == path;

            return Matches(coordinatorHost) && Path == path && Pool_patch != null &&
                   string.Equals(patch.uuid, Pool_patch.uuid, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is OtherLegacyMapping other && Matches(other.CoordinatorHost, other.Path, other.Pool_patch);
        }

        public override int GetHashCode()
        {
            if (Pool_patch == null)
                return Path.GetHashCode() ^ base.GetHashCode();

            return Path.GetHashCode() ^ Pool_patch.GetHashCode() ^ base.GetHashCode();
        }

        public override bool IsValid
        {
            get
            {
                if (Pool_patch == null)
                    return true;

                return Pool_patch.opaque_ref != null;
            }
        }

        public override HostUpdateMapping RefreshUpdate()
        {
            if (Pool_patch != null)
            {
                var patch = CoordinatorHost.Connection?.Cache.Pool_patches.FirstOrDefault(u =>
                    string.Equals(u.uuid, Pool_patch.uuid, StringComparison.OrdinalIgnoreCase));

                if (patch != null && patch.opaque_ref != Pool_patch.opaque_ref)
                    return new OtherLegacyMapping(Path, patch, CoordinatorHost, HostsThatNeedEvacuation);
            }

            return this;
        }
    }


    public class SuppPackMapping : HostUpdateMapping
    {
        public readonly string Path;
        public readonly Pool_update Pool_update;
        public readonly Dictionary<Host, SR> SrsWithUploadedUpdatesPerHost;
        public readonly Dictionary<Host, VDI> SuppPackVdis;

        public SuppPackMapping(string path, Pool_update poolUpdate, Host coordinatorHost,
            Dictionary<Host, SR> srsWithUploadedUpdatesPerHost, Dictionary<Host, VDI> suppPackVdis,
            List<string> hostsThatNeedEvacuation = null)
            : base(coordinatorHost, hostsThatNeedEvacuation)
        {
            Path = !string.IsNullOrEmpty(path) ? path : throw new ArgumentNullException(nameof(path));
            Pool_update = poolUpdate;
            SrsWithUploadedUpdatesPerHost = srsWithUploadedUpdatesPerHost ?? new Dictionary<Host, SR>();
            SuppPackVdis = suppPackVdis ?? new Dictionary<Host, VDI>();
        }

        public bool Matches(Host coordinatorHost, string path, Pool_update update = null)
        {
            if (update == null)
                return Matches(coordinatorHost) && Path == path;

            return Matches(coordinatorHost) && Path == path && Pool_update != null &&
                   string.Equals(update.uuid, Pool_update.uuid, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is SuppPackMapping other && Matches(other.CoordinatorHost, other.Path, other.Pool_update);
        }

        public override int GetHashCode()
        {
            if (Pool_update == null)
                return Path.GetHashCode() ^ base.GetHashCode();

            return Path.GetHashCode() ^ Pool_update.GetHashCode() ^ base.GetHashCode();
        }

        public override bool IsValid
        {
            get
            {
                if (Pool_update == null)
                    return true;

                return Pool_update.opaque_ref != null;
            }
        }

        public override HostUpdateMapping RefreshUpdate()
        {
            if (Pool_update != null)
            {
                var update = CoordinatorHost.Connection?.Cache.Pool_updates.FirstOrDefault(u =>
                    string.Equals(u.uuid, Pool_update.uuid, StringComparison.OrdinalIgnoreCase));

                if (update != null && update.opaque_ref != Pool_update.opaque_ref)
                    return new SuppPackMapping(Path, update, CoordinatorHost, SrsWithUploadedUpdatesPerHost,
                        SuppPackVdis, HostsThatNeedEvacuation);
            }

            return this;
        }
    }
}
