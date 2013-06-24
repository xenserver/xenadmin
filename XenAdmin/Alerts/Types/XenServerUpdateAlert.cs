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
using System.Text;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Actions;
using XenAPI;


namespace XenAdmin.Alerts
{
    public class XenServerUpdateAlert : Alert
    {
        private readonly List<IXenConnection> connections = new List<IXenConnection>();
        private readonly List<Host> hosts = new List<Host>();

        private bool canIgnore;
        public bool CanIgnore
        { get { return canIgnore; } }

        public XenServerVersion Version;

        public XenServerUpdateAlert(XenServerVersion version)
        {
            Version = version;
            _timestamp = version.TimeStamp;
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

        public void CopyConnectionsAndHosts(XenServerUpdateAlert alert)
        {
            connections.Clear();
            connections.AddRange(alert.connections);
            hosts.Clear();
            hosts.AddRange(alert.hosts);
            canIgnore = connections.Count == 0 && hosts.Count == 0;
        }

        public override string Title
        {
            get
            {
                return string.Format(Messages.DOWLOAD_LATEST_XS_TITLE,Version.Name);
            }
        }

        public override string Description
        {
            get
            {
                return string.Format(Messages.DOWNLOAD_LATEST_XS_BODY, Version.Name, HelpersGUI.DateTimeToString(Version.TimeStamp, Messages.DATEFORMAT_DMY_LONG, true));
            }
        }

        public override string DescriptionInvariant
        {
            get
            {
                return string.Format(Messages.DOWNLOAD_LATEST_XS_BODY, Version.Name, HelpersGUI.DateTimeToString(Version.TimeStamp, Messages.DATEFORMAT_DMY_LONG, false));
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

        public override string FixLinkText
        {
            get
            {
                return Messages.ALERT_NEW_VERSION_DOWNLOAD;
            }
        }

        public override AlertPriority Priority
        {
            get { return AlertPriority.Priority5; }
        }
        
        public override FixLinkDelegate FixLinkAction
        {
            get { return () => Program.OpenURL(Version.Url); }
        }

        public override string HelpID
        {
            get
            {
                return "XenServerUpdateAlert";
            }
        }

        public override void Dismiss()
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
                new IgnoreServerAction(connection, Version).RunAsync();
            base.Dismiss();
        }

        public override bool Equals(Alert other)
        {
            if (other is XenServerUpdateAlert)
            {
                return Version.VersionAndOEM == ((XenServerUpdateAlert)other).Version.VersionAndOEM;
            }
            return base.Equals(other);
        }
    }
}
