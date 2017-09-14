/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 * 
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Linq;
using XenAdmin;

namespace XenAPI
{
    public partial class PVS_site : IEquatable<PVS_site>
    {
        public override string ToString()
        {
            return name_label;
        }

        public override string Name()
        {
            return name_label;
        }

        public PVS_cache_storage PvsCacheStorage(Host host)
        {
            if (host == null)
                return null;

            return Connection.Cache.PVS_cache_storages.FirstOrDefault(pvsCacheStorage => 
                pvsCacheStorage.site.opaque_ref == opaque_ref && pvsCacheStorage.host.opaque_ref == host.opaque_ref);
        }

        public string NameWithWarning()
        {
            if (!IsCacheConfigured())
            {
                return string.Format(Messages.PVS_CACHE_INCOMPLETE_CONFIGURATION, Name());
            }

            if (!IsStorageConfigured())
            {
                return string.Format(Messages.PVS_CACHE_STORAGE_NOT_CONFIGURED, Name());
            }

            return Name();
        }

        private bool IsCacheConfigured()
        {
            return !string.IsNullOrEmpty(PVS_uuid);
        }

        private bool IsStorageConfigured()
        {
            var connectionHosts = Connection.Cache.Hosts;

            var siteStorages = Connection.ResolveAll(cache_storage);
            var storageHosts = Connection.ResolveAll(siteStorages.Select(storage => storage.host));

            return connectionHosts.All(host => storageHosts.Contains(host));
        }

        #region IEquatable<PVS_site> Members

        /// <summary>
        /// Indicates whether the current object is equal to the specified object. This calls the implementation from XenObject.
        /// This implementation is required for ToStringWrapper.
        /// </summary>
        public bool Equals(PVS_site other)
        {
            return base.Equals(other);
        }

        #endregion
    }
}
