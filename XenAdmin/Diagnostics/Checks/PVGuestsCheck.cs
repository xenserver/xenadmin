﻿/* Copyright (c) Citrix Systems, Inc. 
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

using System.Linq;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using System.Collections.Generic;
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Diagnostics.Problems.HostProblem;

namespace XenAdmin.Diagnostics.Checks
{
    class PVGuestsCheck : HostPostLivenessCheck
    {
        private readonly Pool _pool;
        private readonly bool _upgrade;
        private readonly bool _manualUpgrade;
        private readonly Dictionary<string, string> _installMethodConfig;

        public PVGuestsCheck(Host master, bool upgrade, bool manualUpgrade = false, Dictionary<string, string> installMethodConfig = null)
            : base(master)
        {
            _pool = Helpers.GetPoolOfOne(Host?.Connection);
            _upgrade = upgrade;
            _manualUpgrade = manualUpgrade;
            _installMethodConfig = installMethodConfig;
        }

        public override bool CanRun()
        {
            if (Helpers.QuebecOrGreater(Host))
                return false;

            if (_pool == null || !_pool.Connection.Cache.VMs.Any(vm => vm.IsPvVm()))
                return false;

            if (!_upgrade && !Helpers.NaplesOrGreater(Host))
                return false;

            return true;
        }

        protected override Problem RunHostCheck()
        {
            //update case
            if (!_upgrade)
                return new PoolHasPVGuestWarningUrl(this, _pool);

            //upgrade case

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
                return new PoolHasPVGuestWarningUrl(this, _pool);

            if (Helpers.QuebecOrGreater(upgradePlatformVersion))
                return new PoolHasPVGuestWarningUrl(this, _pool);

            return null;
        }

        public override string Description => Messages.PV_GUESTS_CHECK_DESCRIPTION;
    }
}
