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

using System;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Alerts.Types
{
    public class HotfixEligibilityAlert: Alert
    {
        private readonly Pool pool;
        private readonly XenServerVersion version;

        public HotfixEligibilityAlert(IXenConnection connection, XenServerVersion version)
        {
            Connection = connection;
            this.version = version;
            pool = Helpers.GetPoolOfOne(connection);
            _timestamp = DateTime.Now;
         }

        #region Overrides of Alert

        public override string Title
        {
            get
            {
                if (pool == null || version == null)
                    return string.Empty;

                var productVersionText = string.Format(Messages.STRING_SPACE_STRING,
                    Helpers.NaplesOrGreater(Connection) ? Messages.XENSERVER : Messages.XENSERVER_LEGACY,
                    Helpers.GetMaster(Connection)?.ProductVersionText());

                switch (version.HotfixEligibility)
                {
                    case hotfix_eligibility.premium:
                        return string.Format(Messages.HOTFIX_ELIGIBILITY_ALERT_TITLE_FREE, productVersionText);
                    case hotfix_eligibility.cu:
                        return pool.IsFreeLicenseOrExpired() 
                            ? string.Format(Messages.HOTFIX_ELIGIBILITY_ALERT_TITLE_FREE, productVersionText) 
                            : Messages.HOTFIX_ELIGIBILITY_ALERT_TITLE_CU;
                    case hotfix_eligibility.none:
                        return string.Format(Messages.HOTFIX_ELIGIBILITY_ALERT_TITLE_EOL, productVersionText);
                    default:
                        return string.Empty;
                }
            }
        }

        public override string Description
        {
            get
            {
                if (pool == null)
                    return string.Empty;

                var versionText = Helpers.GetMaster(Connection)?.ProductVersionText();
                var productVersionText = string.Format(Messages.STRING_SPACE_STRING, 
                    Helpers.NaplesOrGreater(Connection) ? Messages.XENSERVER : Messages.XENSERVER_LEGACY,
                    versionText);

                switch (version.HotfixEligibility)
                {
                    case hotfix_eligibility.premium:
                        return string.Format(Messages.HOTFIX_ELIGIBILITY_ALERT_DESCRIPTION_FREE, productVersionText, HelpersGUI.DateTimeToString(version.HotfixEligibilityPremiumDate.ToLocalTime(), Messages.DATEFORMAT_DMY, true));
                    case hotfix_eligibility.cu:
                        return pool.IsFreeLicenseOrExpired()
                            ? string.Format(Messages.HOTFIX_ELIGIBILITY_ALERT_DESCRIPTION_FREE, productVersionText, HelpersGUI.DateTimeToString(version.HotfixEligibilityPremiumDate.ToLocalTime(), Messages.DATEFORMAT_DMY, true))
                            : string.Format(Messages.HOTFIX_ELIGIBILITY_ALERT_DESCRIPTION_CU, productVersionText, HelpersGUI.DateTimeToString(version.HotfixEligibilityNoneDate.ToLocalTime(), Messages.DATEFORMAT_DMY, true), versionText);
                    case hotfix_eligibility.none:
                        return pool.IsFreeLicenseOrExpired()
                            ? string.Format(Messages.HOTFIX_ELIGIBILITY_ALERT_DESCRIPTION_EOL_FREE, productVersionText, HelpersGUI.DateTimeToString(version.EolDate.ToLocalTime(), Messages.DATEFORMAT_DMY, true))
                            : string.Format(Messages.HOTFIX_ELIGIBILITY_ALERT_DESCRIPTION_EOL, productVersionText, HelpersGUI.DateTimeToString(version.EolDate.ToLocalTime(), Messages.DATEFORMAT_DMY, true));
                    default:
                        return string.Empty;
                }
            }
        }

        public override AlertPriority Priority => AlertPriority.Priority3;

        public override string AppliesTo => Helpers.GetName(Helpers.GetPoolOfOne(Connection));

        public override string FixLinkText => null;

        public override Action FixLinkAction => null;

        public override string HelpID => "HotfixEligibilityAlert";
        
        public override bool Equals(Alert other)
        {
            if (other is HotfixEligibilityAlert alert)
            {
                return Connection == alert.Connection;
            }
            return base.Equals(other);
        }
        #endregion
    }
}
