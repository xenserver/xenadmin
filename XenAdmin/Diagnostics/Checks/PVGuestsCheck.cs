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

using System.Linq;
using System.Collections.Generic;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAPI;


namespace XenAdmin.Diagnostics.Checks
{
    class PVGuestsCheck : HostPostLivenessCheck
    {
        private readonly Pool _pool;
        private readonly bool _manualUpgrade;
        private readonly Dictionary<string, string> _installMethodConfig;
        private List<VM> _pvGuests = new List<VM>();

        public PVGuestsCheck(Host coordinator, bool manualUpgrade = false, Dictionary<string, string> installMethodConfig = null)
            : base(coordinator)
        {
            _pool = Helpers.GetPoolOfOne(Host?.Connection);
            _manualUpgrade = manualUpgrade;
            _installMethodConfig = installMethodConfig;
        }

        public override bool CanRun()
        {
            if (Helpers.YangtzeOrGreater(Host))
                return false;

            if (_pool == null)
                return false;

            _pvGuests = _pool.Connection.Cache.VMs.Where(vm => vm.IsPvVm()).ToList();
            if (_pvGuests.Count <= 0)
                return false;

            return true;
        }

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

            // we don't know the upgrade version, so add warning
            // (this is the case of the manual upgrade or when the rpu plugin doesn't have the function)
            if (string.IsNullOrEmpty(upgradePlatformVersion))
                return new PoolHasPVGuestWarningUrl(this, _pool, _pvGuests);

            if (Helpers.YangtzeOrGreater(upgradePlatformVersion))
                return new PoolHasPVGuestProblem(this, _pool, _pvGuests);

            if (Helpers.QuebecOrGreater(upgradePlatformVersion))
                return new PoolHasPVGuestWarningUrl(this, _pool, _pvGuests);

            return null;
        }

        public override string Description => Messages.PV_GUESTS_CHECK_DESCRIPTION;
    }
}
