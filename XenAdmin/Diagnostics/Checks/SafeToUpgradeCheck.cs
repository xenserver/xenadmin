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
using XenAdmin.Core;
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAPI;


namespace XenAdmin.Diagnostics.Checks
{
    class SafeToUpgradeCheck : HostPostLivenessCheck
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Dictionary<string, string> _installMethodConfig;

        public SafeToUpgradeCheck(Host host, Dictionary<string, string> installMethodConfig)
            : base(host)
        {           
            _installMethodConfig = installMethodConfig;
        }
        public override bool CanRun() => !Helpers.Post82X(Host);

        protected override Problem RunHostCheck()
        {
            var hotfix = HotfixFactory.Hotfix(Host);
            if (hotfix != null && hotfix.ShouldBeAppliedTo(Host))
                return new HostDoesNotHaveHotfixWarning(this, Host);

            try
            {
                var result = Host.call_plugin(Host.Connection.Session, Host.opaque_ref, "prepare_host_upgrade.py", "testSafe2Upgrade", _installMethodConfig);

                if (result.ToLowerInvariant() == "true")
                    return null;

                Host.TryGetUpgradeVersion(Host, _installMethodConfig, out var upgradePlatformVersion, out _);

                // block the upgrade to a post-8.2.X version
                if (Helpers.Post82X(upgradePlatformVersion))
                {
                    switch (result.ToLowerInvariant())
                    {
                        case "not_enough_space":
                            return new HostNotSafeToUpgradeProblem(this, Host, HostNotSafeToUpgradeReason.NotEnoughSpace);
                        case "vdi_present":
                            return new HostNotSafeToUpgradeProblem(this, Host, HostNotSafeToUpgradeReason.VdiPresent);
                        case "utility_part_present":
                            return new HostNotSafeToUpgradeProblem(this, Host, HostNotSafeToUpgradeReason.UtilityPartitionPresent);
                        case "legacy_partition_table":
                            return new HostNotSafeToUpgradeProblem(this, Host, HostNotSafeToUpgradeReason.LegacyPartitionTable);
                        default:
                            return new HostNotSafeToUpgradeProblem(this, Host, HostNotSafeToUpgradeReason.Default);
                    }
                }

                // add a warning for older or unknown upgrade version
                switch (result.ToLowerInvariant())
                {
                    case "not_enough_space":
                        return new HostNotSafeToUpgradeWarning(this, Host, HostNotSafeToUpgradeReason.NotEnoughSpace);
                    case "vdi_present":
                        return new HostNotSafeToUpgradeWarning(this, Host, HostNotSafeToUpgradeReason.VdiPresent);
                    case "utility_part_present":
                        return new HostNotSafeToUpgradeWarning(this, Host, HostNotSafeToUpgradeReason.UtilityPartitionPresent);
                    case "legacy_partition_table":
                        return new HostNotSafeToUpgradeWarning(this, Host, HostNotSafeToUpgradeReason.LegacyPartitionTable);
                    default:
                        return new HostNotSafeToUpgradeWarning(this, Host, HostNotSafeToUpgradeReason.Default);
                }
            }
            catch (Exception exception)
            {
                //note: handle the case when we get UNKNOWN_XENAPI_PLUGIN_FUNCTION - testSafe2Upgrade
                log.Warn($"Plugin call prepare_host_upgrade.testSafe2Upgrade on {Host.Name()} threw an exception.", exception);
            }

            return null;
        }

        public override string Description => Messages.CHECKING_SAFE_TO_UPGRADE_DESCRIPTION;
    }
}
