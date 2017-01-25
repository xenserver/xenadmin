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
using XenAdmin;
using System;
using System.Collections.ObjectModel;

namespace XenAPI
{
    public class StorageLinkPool
    {
        private string _friendlyName;
        private uint _capacity;
        private uint _usedSpace;

        public string StorageLinkSystemId { get; private set; }
        public string ParentStorageLinkPoolId { get; private set; }
        public StorageLinkEnums.RaidType RaidTypes { get; private set; }
        public StorageLinkEnums.ProvisioningType ProvisioningTypes { get; private set; }

        public StorageLinkPool()
        {
        }

        public StorageLinkPool(string opaque_ref, string friendlyName, string parentStorageLinkPoolId, string storageLinkSystemId, uint capacity, uint usedSpace, StorageLinkEnums.RaidType raidTypes, StorageLinkEnums.ProvisioningType provisioningTypes)
        {
            Util.ThrowIfStringParameterNullOrEmpty(opaque_ref, "opaque_ref");
            Util.ThrowIfParameterNull(friendlyName, "friendlyName");
            Util.ThrowIfParameterNull(parentStorageLinkPoolId, "parentStorageLinkPoolId");
            Util.ThrowIfParameterNull(storageLinkSystemId, "storageLinkSystemId");

            _friendlyName = friendlyName;
            ParentStorageLinkPoolId = parentStorageLinkPoolId;
            StorageLinkSystemId = storageLinkSystemId;
            _capacity = capacity;
            _usedSpace = usedSpace;
            RaidTypes = raidTypes;
            ProvisioningTypes = provisioningTypes;
        }

        public uint Capacity
        {
            get
            {
                return _capacity;
            }
        }

        public uint FreeSpace
        {
            get
            {
                return Math.Max(0, _capacity - _usedSpace);
            }
        }
    }
}
