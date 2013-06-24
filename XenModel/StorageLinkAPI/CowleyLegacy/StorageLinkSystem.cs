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

namespace XenAPI
{
    public class StorageLinkSystem : StorageLinkObject<StorageLinkSystem>
    {
        public StorageLinkServer StorageLinkServer { get; private set; }
        public string StorageSystemId { get; private set; }
        public string SerialNumber { get; private set; }
        public string Model { get; private set; }
        public string FullName { get; private set; }
        public string StorageAdapterId { get; private set; }
        public StorageLinkEnums.StorageSystemCapabilities Capabilities { get; private set; }

        public StorageLinkSystem()
        {
        }

        public StorageLinkSystem(StorageLinkConnection storageLinkConnection,
            string opaque_ref,
            string friendlyName,
            StorageLinkServer storageLinkServer,
            string storageSystemId,
            string serialNumber,
            string model,
            string fullName,
            string storageAdapterId,
            StorageLinkEnums.StorageSystemCapabilities capabilities)
            : base(storageLinkConnection, opaque_ref, friendlyName)
        {
            Util.ThrowIfParameterNull(storageLinkServer, "storageLinkServer");
            Util.ThrowIfStringParameterNullOrEmpty(storageSystemId, "storageSystemId");
            Util.ThrowIfStringParameterNullOrEmpty(serialNumber, "serialNumber");
            Util.ThrowIfStringParameterNullOrEmpty(model, "model");
            Util.ThrowIfStringParameterNullOrEmpty(fullName, "fullName");
            Util.ThrowIfStringParameterNullOrEmpty(storageAdapterId, "storageAdapterId");

            StorageLinkServer = storageLinkServer;
            StorageSystemId = storageSystemId;
            SerialNumber = serialNumber;
            Model = model;
            FullName = fullName;
            Capabilities = capabilities;
            StorageAdapterId = storageAdapterId;
        }

        public StorageLinkAdapter StorageLinkAdapter
        {
            get
            {
                foreach (var sla in this.StorageLinkConnection.Cache.StorageAdapters)
                {
                    if (StorageAdapterId == sla.opaque_ref)
                        return sla;
                }
                return null;
            }
        }
    }
}
