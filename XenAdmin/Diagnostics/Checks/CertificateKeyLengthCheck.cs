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
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Diagnostics.Problems;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using XenAdmin.Diagnostics.Hotfixing;
using XenAdmin.Diagnostics.Problems.HostProblem;

namespace XenAdmin.Diagnostics.Checks
{
    class CertificateKeyLengthCheck : HostPostLivenessCheck
    {
        private readonly bool _manualUpgrade;
        private readonly Dictionary<string, string> _installMethodConfig;

        public CertificateKeyLengthCheck(Host coordinator, bool manualUpgrade = false, Dictionary<string, string> installMethodConfig = null)
            : base(coordinator)
        {
            _manualUpgrade = manualUpgrade;
            _installMethodConfig = installMethodConfig;
        }

        protected override Problem RunHostCheck()
        {
            //post-stockholm uses stunnel 5.60 which requires certificate key length
            //equal to or greater than 2048 bytes; yangtze uses stunnel 5.56

            if (!IsShortCertificateKey())
                return null;

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
                return new CertificateKeyLengthWarningUrl(this, Host);

            if (Helpers.CloudOrGreater(upgradePlatformVersion))
                return new CertificateKeyLengthProblem(this, Host);

            return null;
        }

        private bool IsShortCertificateKey()
        {
            var session = Host.Connection?.Session;
            if (session == null)
                return false;

            var certificate = Host.get_server_certificate(session, Host.opaque_ref);

            var lines = certificate.Split('\n').ToList();
            if (lines.Count > 0)
                lines.RemoveAt(0); //remove BEGIN CERTIFICATE header
            if (lines.Count > 0)
                lines.RemoveAt(lines.Count - 1); //remove END CERTIFICATE footer

            certificate = string.Join("\n", lines);

            try
            {
                byte[] bytes = Convert.FromBase64String(certificate);
                var x509Cert = new X509Certificate2(bytes);
                return x509Cert.PublicKey.Key.KeySize < 2048;
            }
            catch
            {
                return false;
            }
        }

        public override string Description => Messages.CERTIFICATE_KEY_LENGTH_CHECK_DESCRIPTION;
    }
}
