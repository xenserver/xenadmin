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
using XenAdmin.Core;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAdmin.Model;
using XenAPI;

namespace XenAdmin.Diagnostics.Checks
{
    class HealthCheckServiceCheck : PoolCheck
    {
        private readonly Dictionary<string, string> _installMethodConfig;

        public HealthCheckServiceCheck(Pool pool, Dictionary<string, string> installMethodConfig)
            : base(pool)
        {
            _installMethodConfig = installMethodConfig;
        }

        public override string Description => Messages.CHECKING_HEALTH_CHECK_SERVICE;

        public override bool CanRun()
        {
            if (Helpers.Post82X(Pool.Connection))
                return false;

            return Pool.HealthCheckStatus() == HealthCheckStatus.Enabled;
        }

        protected override Problem RunCheck()
        {
            string upgradePlatformVersion = null;

            if (_installMethodConfig != null)
                Host.TryGetUpgradeVersion(Helpers.GetCoordinator(Pool.Connection), _installMethodConfig, out upgradePlatformVersion, out _);

            if (Helpers.Post82X(upgradePlatformVersion))
                return new HealthCheckServiceProblem(this, Pool);

            if (string.IsNullOrEmpty(upgradePlatformVersion))
                return new HealthCheckServiceWarning(this, Pool);

            return null;
        }
    }
}
