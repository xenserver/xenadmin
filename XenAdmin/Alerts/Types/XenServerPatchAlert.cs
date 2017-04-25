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
using System.Linq;
using XenAdmin.Network;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Alerts
{
    public class XenServerPatchAlert : XenServerUpdateAlert
    {
        public XenServerPatch Patch;

        /// <summary>
        /// Can we apply this alert. Calling this sets the CannotApplyReason where applicable
        /// </summary>
        public bool CanApply
        {
            get
            {
                var distinctHosts = DistinctHosts;

                if (distinctHosts != null)
                {
                    if (distinctHosts.All(IsHostLicenseRestricted))
                    {
                        CannotApplyReason = Messages.MANUAL_CHECK_FOR_UPDATES_UNLICENSED_INFO;
                        return false;
                    }

                    if (distinctHosts.Any(IsHostLicenseRestricted))
                    {
                        CannotApplyReason = Messages.MANUAL_CHECK_FOR_UPDATES_PARTIAL_UNLICENSED_INFO;
                        return true;
                    }
                }

                CannotApplyReason = null;
                return true;
            }
        }

        public string CannotApplyReason { get; set; }

        private bool IsHostLicenseRestricted(Host host)
        {
            if(host == null)
                return false;
            
            return !host.CanApplyHotfixes;
        }

        public XenServerPatchAlert(XenServerPatch patch)
        {
            Patch = patch;
            _priority = patch.Priority;
            _timestamp = Patch.TimeStamp;
        }

        public override string WebPageLabel
        {
            get
            {
                Uri uri = new Uri(Patch.Url);
                return uri.Segments.Last();
            }
        }

        public override AlertPriority Priority
        {
            get
            {
                if (Enum.IsDefined(typeof(AlertPriority), _priority))
                    return (AlertPriority)_priority;

                return AlertPriority.Priority2;
            }
        }

        public override string Description
        {
            get
            {
                if (Patch.InstallationSize != 0)
                    return string.Format(Messages.PATCH_DESCRIPTION_AND_INSTALLATION_SIZE, Patch.Description, Util.DiskSizeString(Patch.InstallationSize));
                return Patch.Description;
            }
        }

        public override string Name
        {
            get { return Patch.Name; }
        }

        public override Action FixLinkAction
        {
            get { return () => Program.OpenURL(Patch.Url); }
        }

        public override string FixLinkText
        {
            get
            {
                return Messages.ALERT_NEW_PATCH_DOWNLOAD;
            }
        }

        public override string HelpID
        {
            get
            {
                return "XenServerPatchAlert";
            }
        }

        public override string Title
        {
            get { return string.Format(Messages.NEW_UPDATE_AVAILABLE, Patch.Name); }
        }

        public override bool IsDismissed()
        {
            foreach (var connection in connections)
                if (IsDismissed(connection))
                    return true;

            return false;
        }

        private bool IsDismissed(IXenConnection connection)
        {
            XenAPI.Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return false;

            Dictionary<string, string> other_config = pool.other_config;

            if (other_config.ContainsKey(IgnorePatchAction.IgnorePatchKey))
            {
                List<string> current = new List<string>(other_config[IgnorePatchAction.IgnorePatchKey].Split(','));
                if (current.Contains(Patch.Uuid, StringComparer.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public override bool Equals(Alert other)
        {
            if (other is XenServerPatchAlert)
            {
                return string.Equals(Patch.Uuid, ((XenServerPatchAlert)other).Patch.Uuid, StringComparison.OrdinalIgnoreCase);
            }
            return base.Equals(other);
        }
    }
}
