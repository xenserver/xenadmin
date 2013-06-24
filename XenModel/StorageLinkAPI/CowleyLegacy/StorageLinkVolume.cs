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

using XenAdmin;
using XenAdmin.Network.StorageLink;
using System;
using XenAdmin.Network;
using System.Collections.Generic;

namespace XenAPI
{
    public class StorageLinkVolume : StorageLinkObject<StorageLinkVolume>
    {
        private uint _capacity;
        private uint _usedSpace;
        public StorageLinkServer StorageLinkServer { get; private set; }
        public string StorageLinkSystemId { get; private set; }
        public string StorageLinkPoolId { get; private set; }

        public StorageLinkVolume()
        {
        }

        public StorageLinkVolume(StorageLinkConnection storageLinkConnection, string opaque_ref, StorageLinkServer storageLinkServer, string friendlyName, string storageLinkSystemId, string storageLinkPoolId, uint capacity, uint usedSpace)
            : base(storageLinkConnection, opaque_ref, friendlyName)
        {
            Util.ThrowIfStringParameterNullOrEmpty(storageLinkPoolId, "storageLinkPoolId");
            Util.ThrowIfStringParameterNullOrEmpty(storageLinkSystemId, "storageLinkSystemId");

            StorageLinkServer = storageLinkServer;
            StorageLinkSystemId = storageLinkSystemId;
            StorageLinkPoolId = storageLinkPoolId;
            _capacity = capacity;
            _usedSpace = usedSpace;
        }

        public override void UpdateFrom(StorageLinkVolume update)
        {
            base.UpdateFrom(update);
            Capacity = update.Capacity;
            UsedSpace = update.UsedSpace;
        }

        public uint Capacity
        {
            get
            {
                return _capacity;
            }
            private set
            {
                if (_capacity != value)
                {
                    _capacity = value;
                    NotifyPropertyChanged("Capacity");
                    NotifyPropertyChanged("FreeSpace");
                }
            }
        }

        public uint UsedSpace
        {
            get
            {
                return _usedSpace;
            }
            private set
            {
                if (_usedSpace != value)
                {
                    _usedSpace = value;
                    NotifyPropertyChanged("UsedSpace");
                    NotifyPropertyChanged("FreeSpace");
                }
            }
        }

        public uint FreeSpace
        {
            get
            {
                return Math.Max(0, _capacity - _usedSpace);
            }
        }

        public StorageLinkSystem StorageLinkSystem
        {
            get
            {
                return StorageLinkConnection.Cache.Resolve<StorageLinkSystem>(StorageLinkSystemId);
            }
        }

        public StorageLinkPool StorageLinkPool
        {
            get
            {
                return StorageLinkConnection.Cache.Resolve<StorageLinkPool>(StorageLinkPoolId);
            }
        }

        public StorageLinkPool RootStorageLinkPool
        {
            get
            {
                var output = StorageLinkPool;

                while (output != null)
                {
                    var parent = output.Parent;
                    if (parent == null)
                    {
                        return output;
                    }
                    output = parent;
                }

                return output;
            }
        }

        public VDI VDI(IEnumerable<IXenConnection> connections)
        {
            foreach (IXenConnection c in connections)
            {
                VDI vdi = Array.Find(c.Cache.VDIs, v => v.sm_config != null && v.sm_config.ContainsKey("SVID") && v.sm_config["SVID"] == opaque_ref);

                if (vdi != null)
                {
                    return vdi;
                }
            }

            return null;

        }
    }
}
