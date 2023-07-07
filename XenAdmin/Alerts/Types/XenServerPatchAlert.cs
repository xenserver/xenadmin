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
using System.Linq;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;
using System.Text;


namespace XenAdmin.Alerts
{
    public class XenServerPatchAlert : XenServerUpdateAlert
    {
        public const string IgnorePatchKey = "XenCenter.IgnorePatches";

        public readonly XenServerPatch Patch;
        public readonly XenServerVersion NewServerVersion;
       
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
                    if (distinctHosts.All(h => Helpers.FeatureForbidden(h, Host.RestrictHotfixApply)))
                    {
                        CannotApplyReason = Messages.MANUAL_CHECK_FOR_UPDATES_UNLICENSED_INFO;
                        return false;
                    }

                    if (distinctHosts.Any(h => Helpers.FeatureForbidden(h, Host.RestrictHotfixApply)))
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

        /// <summary>
        /// Creates a patch alert
        ///  </summary>
        /// <param name="patch">The patch</param>
        /// <param name="newServerVersion">The version that the patch installs, or null if the patch doesn't update the server version</param>
        public XenServerPatchAlert(XenServerPatch patch, XenServerVersion newServerVersion = null)
        {
            Patch = patch;
            NewServerVersion = newServerVersion;
            if (NewServerVersion != null)
                RequiredClientVersion = Updates.GetRequiredClientVersion(NewServerVersion);
            _priority = patch.Priority;
            _timestamp = Patch.TimeStamp;
        }

        public override string WebPageLabel => Patch.Url;

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
                StringBuilder sb = new StringBuilder();
                sb.Append(Patch.Description);
                if (Patch.InstallationSize != 0)
                {
                    sb.AppendLine();
                    sb.AppendFormat(Messages.PATCH_INSTALLATION_SIZE, Util.DiskSizeString(Patch.InstallationSize));
                }
                if (RequiredClientVersion != null)
                {
                    sb.AppendLine();
                    sb.AppendFormat(Messages.PATCH_NEEDS_NEW_XENCENTER, BrandManager.BrandConsole, RequiredClientVersion.Version);
                }
                return sb.ToString();
            }
        }

        public override string Name
        {
            get
            {
                if (ShowAsNewVersion)
                    return NewServerVersion.Name; 
                return Patch.Name;
            }
        }

        public override Action FixLinkAction
        {
            get { return () => Program.OpenURL(Patch.Url); }
        }

        public override string FixLinkText => Messages.ALERT_NEW_PATCH_DOWNLOAD;

        public override string HelpID => "XenServerPatchAlert";

        public override string Title
        {
            get
            {
                if (ShowAsNewVersion)
                    return string.Format(Messages.DOWLOAD_LATEST_XS_TITLE, NewServerVersion.Name); 
                return string.Format(Messages.NEW_UPDATE_AVAILABLE, Patch.Name);
            }
        }

        protected override bool IsDismissed(IXenConnection connection)
        {
            Pool pool = Helpers.GetPoolOfOne(connection);
            if (pool == null)
                return false;

            Dictionary<string, string> other_config = pool.other_config;

            if (other_config.ContainsKey(IgnorePatchKey))
            {
                List<string> current = new List<string>(other_config[IgnorePatchKey].Split(','));
                if (current.Contains(Patch.Uuid, StringComparer.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        protected override void Dismiss(Dictionary<string, string> otherConfig)
        {
            if (otherConfig.ContainsKey(IgnorePatchKey))
            {
                var current = new List<string>(otherConfig[IgnorePatchKey].Split(','));
                if (current.Contains(Patch.Uuid, StringComparer.OrdinalIgnoreCase))
                    return;

                current.Add(Patch.Uuid);
                otherConfig[IgnorePatchKey] = string.Join(",", current.ToArray());
            }
            else
            {
                otherConfig.Add(IgnorePatchKey, Patch.Uuid);
            }
        }

        public override bool Equals(Alert other)
        {
            if (other is XenServerPatchAlert patchAlert)
                return string.Equals(Patch.Uuid, patchAlert.Patch.Uuid, StringComparison.OrdinalIgnoreCase);

            return base.Equals(other);
        }

        public bool ShowAsNewVersion => NewServerVersion != null && !NewServerVersion.PresentAsUpdate;
    }
}
