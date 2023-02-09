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
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Alerts
{
    public class XenServerVersionAlert : XenServerUpdateAlert
    {
        public const string LAST_SEEN_SERVER_VERSION_KEY = "XenCenter.LastSeenServerVersion";

        public readonly XenServerVersion Version;

        public XenServerVersionAlert(XenServerVersion version)
        {
            Version = version;
            RequiredClientVersion = Updates.GetRequiredClientVersion(Version);
            _timestamp = version.TimeStamp;
        }

        public override string WebPageLabel => Version.Url;

        public override string Name => Version.Name;

        public override string Title => string.Format(Messages.DOWLOAD_LATEST_XS_TITLE, Version.Name);

        public override string Description => string.Format(Messages.DOWNLOAD_LATEST_XS_BODY,
            Version.Name, BrandManager.ProductBrand);

        public override string FixLinkText => Messages.ALERT_NEW_VERSION_DOWNLOAD;

        public override AlertPriority Priority => AlertPriority.Priority5;

        public override Action FixLinkAction
        {
            get { return () => Program.OpenURL(Version.Url); }
        }

        public override string HelpID => "XenServerUpdateAlert";

        protected override bool IsDismissed(IXenConnection connection)
        {
            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return false;

            Dictionary<string, string> other_config = pool.other_config;

            if (other_config.ContainsKey(LAST_SEEN_SERVER_VERSION_KEY))
            {
                List<string> current = new List<string>(other_config[LAST_SEEN_SERVER_VERSION_KEY].Split(','));
                if (current.Contains(Version.Version.ToString()))
                    return true;
            }
            return false;
        }

        protected override void Dismiss(Dictionary<string, string> otherConfig)
        {
            if (otherConfig.ContainsKey(LAST_SEEN_SERVER_VERSION_KEY))
            {
                List<string> current = new List<string>(otherConfig[LAST_SEEN_SERVER_VERSION_KEY].Split(','));
                if (current.Contains(Version.Version.ToString()))
                    return;
                current.Add(Version.Version.ToString());
                otherConfig[LAST_SEEN_SERVER_VERSION_KEY] = string.Join(",", current.ToArray());
            }
            else
            {
                otherConfig.Add(LAST_SEEN_SERVER_VERSION_KEY, Version.Version.ToString());
            }
        }

        public override bool Equals(Alert other)
        {
            if (other is XenServerVersionAlert versionAlert)
                return Version.Version.ToString() == versionAlert.Version.Version.ToString();

            return base.Equals(other);
        }
    }
}
