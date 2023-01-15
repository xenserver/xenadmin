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

using System;
using System.Collections.Generic;
using XenAdmin.Core;
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Diagnostics.Problems;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAPI;


namespace XenAdmin.Diagnostics.Checks
{
    class PowerOniLoCheck : HostPostLivenessCheck
    {
        private readonly Dictionary<string, string> _installMethodConfig;
        private XenServerVersion _newVersion;
        private readonly bool _manualUpgrade;

        public PowerOniLoCheck(Host host, XenServerVersion newVersion)
            : base(host)
        {
            _newVersion = newVersion;
        }

        public PowerOniLoCheck(Host host, Dictionary<string, string> installMethodConfig, bool manualUpgrade)
            : base(host)
        {
            _installMethodConfig = installMethodConfig;
            _manualUpgrade = manualUpgrade;
        }

        public override string Description => Messages.CHECKING_POWER_ON_MODE;

        public override bool CanRun()
        {
            if (Helpers.StockholmOrGreater(Host))
                return false;

            if (Host == null || string.IsNullOrEmpty(Host.power_on_mode))
                //check for disabled as this is when the check would raise an eyebrow
                return false;

            if (_newVersion != null && !Helpers.NaplesOrGreater(Host))
                return false;

            return true;
        }

        protected override Problem RunHostCheck()
        {
            //update case
            if (_newVersion != null)
            {
                if (_newVersion.Version.CompareTo(new Version(BrandManager.ProductVersion82Short)) >= 0)
                    return new PowerOniLoProblem(this, Host);
                return null;
            }

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

            // we don't know the upgrade version, so add generic warning
            // (this is the case of the manual upgrade or when the rpu plugin doesn't have the function)
            if (string.IsNullOrEmpty(upgradePlatformVersion))
                return new PowerOniLoWarning(this, Host);
                
            // we know they are upgrading to Stockholm or greater, so block them
            if (Helpers.StockholmOrGreater(upgradePlatformVersion))
                return new PowerOniLoProblem(this, Host);

            return null;
        }
    }
}
