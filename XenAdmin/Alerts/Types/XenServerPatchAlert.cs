/* Copyright (c) Citrix Systems Inc. 
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
    public class XenServerPatchAlert : Alert
    {
        private readonly List<IXenConnection> connections = new List<IXenConnection>();
        private readonly List<Host> hosts = new List<Host>();
        public List<Host> Hosts
        {
            get
            {
                List<Host> result = new List<Host>();

                foreach (Host host in hosts)
                    result.Add(host);

                foreach (IXenConnection connection in connections)
                    result.AddRange(connection.Cache.Hosts);

                return result.Distinct().ToList();
            }
        }

        private bool canIgnore;
        public bool CanIgnore
        { get { return canIgnore; } }

        public XenServerPatch Patch;

        /// <summary>
        /// Can we apply this alert. Calling this sets the CannotApplyReason where applicable
        /// </summary>
        public override bool CanApply
        {
            get 
            { 
                if(Hosts != null)
                {
                    if(Hosts.All(IsHostLicenseRestricted))
                    {
                        CannotApplyReason = Messages.MANUAL_CHECK_FOR_UPDATES_UNLICENSED_INFO;
                        return false;
                    }

                    if (Hosts.Any(IsHostLicenseRestricted))
                    {
                        CannotApplyReason = Messages.MANUAL_CHECK_FOR_UPDATES_PARTIAL_UNLICENSED_INFO;
                        return true;
                    }
                }

                CannotApplyReason = string.Empty;
                return true;
            }
        }

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
            canIgnore = true;
        }
        
        public void IncludeConnection(IXenConnection newConnection)
        {
            connections.Add(newConnection);
            if (connections.Count > 0)
                canIgnore = false;
        }

        public void IncludeHosts(List<Host> newHosts)
        {
            hosts.AddRange(newHosts);
            if (hosts.Count > 0)
                canIgnore = false;
        }

        public void CopyConnectionsAndHosts(XenServerPatchAlert alert)
        {
            connections.Clear();
            connections.AddRange(alert.connections);
            hosts.Clear();
            hosts.AddRange(alert.hosts);
            canIgnore = connections.Count == 0 && hosts.Count == 0;
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

        public override string AppliesTo
        {
            get
            {
                List<string> names = new List<string>();

                foreach (Host host in hosts)
                    names.Add(host.Name);

                foreach (IXenConnection connection in connections)
                    names.Add(Helpers.GetName(connection));

                return string.Join(", ", names.ToArray());
            }
        }

        public override string Description
        {
            get
            {
                return string.Format("{0} ({1})", Patch.Description, HelpersGUI.DateTimeToString(Patch.TimeStamp, Messages.DATEFORMAT_DMY_LONG, true));
            }
        }

        public override string DescriptionInvariant
        {
            get
            {
                return string.Format("{0} ({1})", Patch.Description, HelpersGUI.DateTimeToString(Patch.TimeStamp, Messages.DATEFORMAT_DMY_LONG, false));
            }
        }

        public override FixLinkDelegate FixLinkAction
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
            get
            {
                return string.Format(Messages.NEW_UPDATE_AVAILABLE,Patch.Name);
            }
        }

        public override void Dismiss()
        {
            base.Dismiss();
            foreach (IXenConnection connection in connections)
            {
                new IgnorePatchAction(connection, Patch).RunAsync();
            }
        }

        public override bool Equals(Alert other)
        {
            if (other is XenServerPatchAlert)
            {
                return Patch.Uuid == ((XenServerPatchAlert)other).Patch.Uuid;
            }
            return base.Equals(other);
        }
    }
}
