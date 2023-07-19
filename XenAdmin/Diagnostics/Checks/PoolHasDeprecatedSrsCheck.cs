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
using System.Linq;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    internal class PoolHasDeprecatedSrsCheck : HostPostLivenessCheck
    {
        private readonly Dictionary<string, string> _installMethodConfig;
        private readonly bool _manualUpgrade;

        public override string Description => string.Format(Messages.DEPRECATED_SRS_CHECK, Helpers.GetPoolOfOne(Host.Connection));

        public PoolHasDeprecatedSrsCheck(Host host, Dictionary<string, string> installMethodConfig, bool manualUpgrade)
            : base(host)
        {
            _installMethodConfig = installMethodConfig;
            _manualUpgrade = manualUpgrade;
        }

        public override bool CanRun() => Host.Connection.Cache.SRs.Any(sr => sr.GetSRType(true) == SR.SRTypes.lvmofcoe);

        protected override Problem RunHostCheck()
        {
            if (!_manualUpgrade)
            {
                var hotfix = HotfixFactory.Hotfix(Host);
                if (hotfix != null && hotfix.ShouldBeAppliedTo(Host))
                    return new HostDoesNotHaveHotfixWarning(this, Host);
            }

            string upgradePlatformVersion = null;

            if (_installMethodConfig != null)
                Host.TryGetUpgradeVersion(Host, _installMethodConfig, out upgradePlatformVersion, out _);

            if (string.IsNullOrEmpty(upgradePlatformVersion))
                return new PoolHasFCoESrWarning(this, Helpers.GetPoolOfOne(Host.Connection), false);

            if (Helpers.CloudOrGreater(upgradePlatformVersion))
                return new PoolHasFCoESrWarning(this, Helpers.GetPoolOfOne(Host.Connection), true);

            return null;
        }
    }
}
