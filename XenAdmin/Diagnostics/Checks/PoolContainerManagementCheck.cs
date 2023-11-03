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
using XenAdmin.Actions.Updates;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    class PoolContainerManagementCheck : HostPostLivenessCheck
    {
        private readonly Dictionary<string, string> _installMethodConfig;
        private readonly Pool _pool;
        private readonly XenServerVersion _newVersion;
        private readonly bool _manualUpgrade;

        public PoolContainerManagementCheck(Host host, XenServerVersion newVersion)
            : base(host)
        {
            _newVersion = newVersion;
            _pool = Helpers.GetPoolOfOne(Host?.Connection);
        }

        public PoolContainerManagementCheck(Host host, Dictionary<string, string> installMethodConfig, bool manualUpgrade)
            : base(host)
        {
            _installMethodConfig = installMethodConfig;
            _pool = Helpers.GetPoolOfOne(Host?.Connection);
            _manualUpgrade = manualUpgrade;
        }

        public override string Description => Messages.CHECKING_CONTAINER_MANAGEMENT;

        public override bool CanRun()
        {
            return _pool != null && Helpers.ContainerCapability(_pool.Connection);
        }

        protected override Problem RunHostCheck()
        {
            if ( _pool == null || !Helpers.ContainerCapability(_pool.Connection))
                return null;

            //update case
            if (_newVersion != null)
            {
                if (_newVersion.Version.CompareTo(new Version(BrandManager.ProductVersion82)) >= 0)
                    return new ContainerManagementWarning(this, _pool, false);
                return null;
            }

            //upgrade case

            if (!_manualUpgrade)
            {
                if (RpuHotfix.Exists(Host, out var hotfix) && hotfix.ShouldBeAppliedTo(Host))
                    return new HostDoesNotHaveHotfixWarning(this, Host);
            }

            string upgradePlatformVersion = null;

            if (_installMethodConfig != null)
                Host.TryGetUpgradeVersion(Host, _installMethodConfig, out upgradePlatformVersion, out _);

            // we don't know the upgrade version, so add generic warning
            // (this is the case of the manual upgrade or when the rpu plugin doesn't have the function)
            // also show the warning if we know they are upgrading to Stockholm or greater

            if (string.IsNullOrEmpty(upgradePlatformVersion) ||
                Helpers.StockholmOrGreater(upgradePlatformVersion))
                return new ContainerManagementWarning(this, _pool, true);

            return null;
        }
    }
}
