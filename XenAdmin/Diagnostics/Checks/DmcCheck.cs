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
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    internal class DmcCheck : PoolCheck
    {
        private readonly Pool _pool;
        private readonly Dictionary<string, string> _installMethodConfig;
        private readonly bool _manualUpgrade;
        private readonly Control _control;

        public DmcCheck(Control control, Pool pool, Dictionary<string, string> installMethodConfig, bool manualUpgrade)
            : base(pool)
        {
            _control = control;
            _pool = pool;
            _installMethodConfig = installMethodConfig;
            _manualUpgrade = manualUpgrade;
        }

        public override bool CanRun()
        {
            return !Helpers.Post82X(_pool.Connection) && !Helpers.FeatureForbidden(_pool.Connection, Host.RestrictDMC);
        }

        protected override Problem RunCheck()
        {
            var coordinator = Helpers.GetCoordinator(Pool.Connection);

            if (!_manualUpgrade)
            {
                var hotfix = HotfixFactory.Hotfix(coordinator);
                if (hotfix != null && hotfix.ShouldBeAppliedTo(coordinator))
                    return new HostDoesNotHaveHotfixWarning(this, coordinator);
            }

            var vms = _pool.Connection.Cache.VMs.Where(v =>
                (v.power_state == vm_power_state.Running ||
                 v.power_state == vm_power_state.Paused ||
                 v.power_state == vm_power_state.Suspended) &&
                (v.memory_static_min > v.memory_dynamic_min || //corner case, probably unlikely
                 v.memory_dynamic_min != v.memory_dynamic_max ||
                 v.memory_dynamic_max != v.memory_static_max)).ToList();

            if (vms.Count == 0)
                return null;

            string upgradePlatformVersion = null;

            if (_installMethodConfig != null)
                Host.TryGetUpgradeVersion(coordinator, _installMethodConfig, out upgradePlatformVersion, out _);

            if (string.IsNullOrEmpty(upgradePlatformVersion))
                return new PoolHasVmsWithDmcWarning(_control, this, _pool, vms);
                
            if (Helpers.Post82X(upgradePlatformVersion))
                return new PoolHasVmsWithDmcProblem(_control, this, _pool, vms);

            return null;
        }

        public override string Description => Messages.DMC_CHECK_ENABLED;
    }
}
