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

using System.Collections.Generic;
using XenAdmin.Wizards.GenericPages;
using XenAPI;

namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    internal class CrossPoolMigrationNetworkResource : INetworkResource
    {
        private readonly VIF _vif;

        public CrossPoolMigrationNetworkResource(VIF vif)
        {
            _vif = vif;
        }

        public string VmNameOverride => _vif.Connection?.Resolve(_vif.VM)?.Name();

        public string NetworkName => _vif.NetworkName();

        public string MACAddress => _vif.MAC;

        public string NetworkID => _vif.network.opaque_ref;
    }


    internal class CrossPoolMigrationNetworkResourceContainer : NetworkResourceContainer
    {
        private readonly List<VIF> _vifs;
        private int _counter;

        public CrossPoolMigrationNetworkResourceContainer(List<VIF> vifs)
        {
            _vifs = vifs;
        }

        public override INetworkResource Next()
        {
            INetworkResource res =  new CrossPoolMigrationNetworkResource(_vifs[_counter]);
            _counter++;
            return res;
        }

        public override bool IsNext => _counter < _vifs.Count;
    }
}
