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
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    internal class CrossPoolMigrationNetworkResource : INetworkResource
    {
        private readonly VIF vif;
        public CrossPoolMigrationNetworkResource(VIF vif)
        {
            this.vif = vif;
        }
        public string NetworkName
        {
            get { return vif.NetworkName(); }
        }

        public string MACAddress
        {
            get { return vif.MAC; }
        }

        public string NetworkID
        {
            get { return vif.network.opaque_ref; }
        }
    }

    internal class CrossPoolMigrationNetworkResourceContainer : NetworkResourceContainer
    {
        private readonly List<VIF> vifs;
        private int counter;

        public CrossPoolMigrationNetworkResourceContainer(List<VIF> vifs)
        {
            this.vifs = vifs;
        }
        public override INetworkResource Next()
        {
            INetworkResource res =  new CrossPoolMigrationNetworkResource(vifs[counter]);
            counter++;
            return res;
        }

        public override bool IsNext
        {
            get { return counter < vifs.Count; }
        }
    }
}
