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

using XenAdmin.Network.StorageLink;
using XenAdmin;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XenAdmin.Network;
using XenAdmin.Core;
using System;

namespace XenAPI
{
    public class StorageLinkRepository : StorageLinkObject<StorageLinkRepository>
    {
        public StorageLinkServer StorageLinkServer { get; private set; }
        public string StorageLinkSystemId { get; private set; }
        public string StorageLinkPoolId { get; private set; }
        public StorageLinkEnums.RaidType RaidType { get; private set; }
        public StorageLinkEnums.ProvisioningType ProvisioningType { get; private set; }
        public StorageLinkEnums.ProvisioningOptions ProvisioningOptions { get; private set; }
        public string HostGroupId { get; private set; }
        private SR _sr;

        public StorageLinkRepository(StorageLinkConnection storageLinkConnection, string opaque_ref, string friendlyName, StorageLinkServer server, string storageSystemId, string storagePoolId, StorageLinkEnums.RaidType raidType, StorageLinkEnums.ProvisioningType provisioningType, StorageLinkEnums.ProvisioningOptions provisioningOptions, string hostGroupId)
            : base(storageLinkConnection, opaque_ref, friendlyName)
        {
            Util.ThrowIfParameterNull(server, "server");
            Util.ThrowIfParameterNull(storageSystemId, "storageSystemId");
            Util.ThrowIfParameterNull(storagePoolId, "storagePoolId");
            Util.ThrowIfParameterNull(hostGroupId, "hostGroupId");

            StorageLinkServer = server;
            StorageLinkSystemId = storageSystemId;
            StorageLinkPoolId = storagePoolId;
            RaidType = raidType;
            ProvisioningType = provisioningType;
            ProvisioningOptions = provisioningOptions;
            HostGroupId = hostGroupId;
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

        public SR SR(IEnumerable<IXenConnection> connections)
        {
            if (_sr == null)
            {
                foreach (IXenConnection c in connections)
                {
                    _sr = Array.Find(c.Cache.SRs, s => s.uuid == opaque_ref);

                    if (_sr != null)
                    {
                        break;
                    }
                }
            }

            return _sr;

        }

        public override string Name
        {
            get
            {
                return _sr == null ? base.Name : _sr.Name;
            }
        }
    }
}
