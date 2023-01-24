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
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{

    public class CrossPoolMigrationStorageResourceContainer : StorageResourceContainer
    {
        private readonly List<VDI> vdis;
        private int currentLoc;

        public CrossPoolMigrationStorageResourceContainer(List<VDI> vdis)
        {
            this.vdis = vdis;
        }

        public override IStorageResource Next()
        {
            IStorageResource res = new CrossPoolMigrationStorageResource(vdis[currentLoc]);
            currentLoc++;
            return res;
        }

        public override bool IsNext
        {
            get 
            { 
                return currentLoc < vdis.Count;
            }
        }
    }

    public class CrossPoolMigrationStorageResource : IStorageResource
    {
        private readonly VDI vdi;
        private readonly SR sr;

        public CrossPoolMigrationStorageResource(VDI vdi)
        {
            this.vdi = vdi;
            sr = vdi.Connection.Cache.Resolve(vdi.SR);
        }

        public string DiskLabel => vdi.Name();
        public object Tag => vdi.opaque_ref;
        public bool SRTypeInvalid => false;
        public SR SR => sr;

        public bool TryCalcRequiredDiskCapacity(out ulong capacity)
        {
            capacity = Convert.ToUInt64(vdi.physical_utilisation);
            return true;
        }
    }
}

