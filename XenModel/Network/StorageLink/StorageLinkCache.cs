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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using XenAPI;

namespace XenAdmin.Network.StorageLink
{
    public class StorageLinkCache
    {
        private readonly object _syncObject = new object();
        private readonly List<StorageLinkSystem> _systems = new List<StorageLinkSystem>();
        private readonly List<StorageLinkPool> _pools = new List<StorageLinkPool>();
        private readonly List<StorageLinkAdapter> _adapters = new List<StorageLinkAdapter>();
        private readonly List<StorageLinkVolume> _volumes = new List<StorageLinkVolume>();
        private readonly List<StorageLinkRepository> _srs = new List<StorageLinkRepository>();
        private StorageLinkServer _server;

        public StorageLinkServer Server
        {
            get
            {
                lock (_syncObject)
                {
                    return _server;
                }
            }
            private set
            {
                lock (_syncObject)
                {
                    _server = value;
                }
            }
        }

        /// <summary>
        /// Occurs when the cache changes or any property of any item in the cache changes.
        /// </summary>
        public event EventHandler Changed;

        public ReadOnlyCollection<StorageLinkSystem> StorageSystems
        {
            get
            {
                lock (_syncObject)
                {
                    return new ReadOnlyCollection<StorageLinkSystem>(new List<StorageLinkSystem>(_systems));
                }
            }
        }

        public ReadOnlyCollection<StorageLinkPool> StoragePools
        {
            get
            {
                lock (_syncObject)
                {
                    return new ReadOnlyCollection<StorageLinkPool>(new List<StorageLinkPool>(_pools));
                }
            }
        }

        public ReadOnlyCollection<StorageLinkAdapter> StorageAdapters
        {
            get
            {
                lock (_syncObject)
                {
                    return new ReadOnlyCollection<StorageLinkAdapter>(new List<StorageLinkAdapter>(_adapters));
                }
            }
        }

        public ReadOnlyCollection<StorageLinkVolume> StorageVolumes
        {
            get
            {
                lock (_syncObject)
                {
                    return new ReadOnlyCollection<StorageLinkVolume>(new List<StorageLinkVolume>(_volumes));
                }
            }
        }

        public ReadOnlyCollection<StorageLinkRepository> StorageRespositories
        {
            get
            {
                lock (_syncObject)
                {
                    return new ReadOnlyCollection<StorageLinkRepository>(new List<StorageLinkRepository>(_srs));
                }
            }
        }

        public IEnumerable<IXenObject> XenSearchableObjects(IEnumerable<IXenConnection> connections)
        {
            StorageLinkServer server = Server;

            if (server != null)
            {
                yield return server;

                if (StorageSystems.Count == 0)
                {
                    yield break;
                }

                foreach (StorageLinkSystem system in StorageSystems)
                {
                    yield return system;
                }

                foreach (StorageLinkPool pool in StoragePools)
                {
                    // only return top level storage-pools
                    if (pool.Parent == null)
                    {
                        yield return pool;
                    }
                }

                foreach (StorageLinkRepository repo in StorageRespositories)
                {
                    // only display it if it belongs to a managed server. StorageLink is to be changed so that it won't listen to
                    // xapi events telling about an SR being forgotten (for xapi performance reasons.)
                    if (repo.SR(connections) != null)
                    {
                        yield return repo;
                    }
                }

            }
        }

        public void Update(string hostGroupId, List<StorageLinkRepository> repositories)
        {
            Util.ThrowIfStringParameterNullOrEmpty(hostGroupId, "hostGroupId");
            Util.ThrowIfParameterNull(repositories, "repositories");

            bool changed;

            lock (_syncObject)
            {
                changed = UpdateItems(_srs, repositories, r => r.HostGroupId == hostGroupId);
            }

            if (changed)
            {
                OnChanged(EventArgs.Empty);
            }
        }

        public void Update(string storageSystemId, List<StorageLinkPool> pools, List<StorageLinkVolume> volumes)
        {
            Util.ThrowIfParameterNull(pools, "pools");
            Util.ThrowIfParameterNull(volumes, "volumes");

            bool changed;

            lock (_syncObject)
            {
                changed = UpdateItems(_pools, pools, p => p.StorageLinkSystemId == storageSystemId);
                changed |= UpdateItems(_volumes, volumes, v => v.StorageLinkSystemId == storageSystemId);
            }

            if (changed)
            {
                OnChanged(EventArgs.Empty);
            }
        }

        public void Update(StorageLinkServer server, List<StorageLinkSystem> systems, List<StorageLinkPool> pools, List<StorageLinkAdapter> adapters, List<StorageLinkVolume> volumes, List<StorageLinkRepository> srs)
        {
            Util.ThrowIfParameterNull(systems, "systems");
            Util.ThrowIfParameterNull(pools, "pools");
            Util.ThrowIfParameterNull(adapters, "adapters");
            Util.ThrowIfParameterNull(volumes, "volumes");
            Util.ThrowIfParameterNull(srs, "srs");

            bool changed = false;

            lock (_syncObject)
            {
                if (Server == null || server == null)
                {
                    changed = Server != server;
                    Server = server;
                }
                else
                {
                    PropertyChangedEventHandler handler = (s, e) => changed = true;
                    Server.PropertyChanged += handler;
                    Server.UpdateFrom(server);
                    Server.PropertyChanged -= handler;
                }

                changed |= UpdateItems(_systems, systems);
                changed |= UpdateItems(_pools, pools);
                changed |= UpdateItems(_adapters, adapters);
                changed |= UpdateItems(_volumes, volumes);
                changed |= UpdateItems(_srs, srs);
            }

            if (changed)
            {
                OnChanged(EventArgs.Empty);
            }
        }

        private static bool UpdateItems<T>(List<T> to, List<T> from) where T : StorageLinkObject<T>
        {
            return UpdateItems<T>(to, from, t => true);
        }

        private static bool UpdateItems<T>(List<T> to, List<T> from, Predicate<T> match) where T : StorageLinkObject<T>
        {
            bool changed = false;

            foreach (T existingItem in to)
            {
                T newItem = from.Find(o => o.Equals(existingItem));

                if (newItem != null)
                {
                    PropertyChangedEventHandler handler = null;

                    if (!changed)
                    {
                        handler = (s, e) => changed = true;
                        existingItem.PropertyChanged += handler;
                    }
                    existingItem.UpdateFrom(newItem);

                    if (handler != null)
                    {
                        existingItem.PropertyChanged -= handler;
                    }
                }
            }

            changed |= to.RemoveAll(o => match(o) && !from.Contains(o)) > 0;
            List<T> newItems = from.FindAll(s => !to.Contains(s));
            changed |= newItems.Count > 0;
            to.AddRange(newItems);
            return changed;
        }

        protected virtual void OnChanged(EventArgs e)
        {
            EventHandler handler = Changed;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public T Resolve<T>(string opaque_ref) where T : class
        {
            foreach (System.Collections.IEnumerable enumerable in new System.Collections.IEnumerable[] { StoragePools, StorageVolumes, StorageAdapters, StorageRespositories, StorageSystems })
            {
                foreach (IStorageLinkObject x in enumerable)
                {
                    if (!(x is T))
                    {
                        break;
                    }
                    if (x.opaque_ref == opaque_ref)
                    {
                        return x as T;
                    }
                }
            }

            return null;
        }
    }
}
