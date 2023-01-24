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
using System.Linq;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Alerts
{
    public abstract class XenServerUpdateAlert : Alert
    {
        private readonly List<IXenConnection> connections = new List<IXenConnection>();
        private readonly List<Host> hosts = new List<Host>();

        public ClientVersion RequiredClientVersion;

        private readonly object connectionsLock = new object();
        private readonly object hostsLock = new object();
        
        protected List<IXenConnection> Connections
        {
            get
            {
                lock (connectionsLock)
                    return connections;
            }
        }

        protected List<Host> Hosts
        {
            get
            {
                lock (hostsLock)
                    return hosts;
            }
        }
        
        public bool CanIgnore
        {
            get
            {
                return (Connections.Count == 0 && Hosts.Count == 0) || IsDismissed();
            }
        }

        public List<Host> DistinctHosts
        {
            get
            {
                List<Host> result = new List<Host>();

                lock (hostsLock)
                {
                    result.AddRange(hosts);
                }
                lock (connectionsLock)
                {
                    foreach (IXenConnection connection in connections)
                        result.AddRange(connection.Cache.Hosts);
                }
                return result.Distinct().ToList();
            }
        }

        public void IncludeConnection(IXenConnection newConnection)
        {
            lock (connectionsLock)
            {
                if (!connections.Contains(newConnection))
                    connections.Add(newConnection);
            }
        }

        public void IncludeHosts(IEnumerable<Host> newHosts)
        {
            lock (hostsLock)
            {
                var notContained = newHosts.Where(h => !hosts.Contains(h));
                hosts.AddRange(notContained);
            }
        }

        public void CopyConnectionsAndHosts(XenServerUpdateAlert alert)
        {
            lock (connectionsLock)
            {
                connections.Clear();
                connections.AddRange(alert.Connections);
            }
            lock (hostsLock)
            {
                hosts.Clear();
                hosts.AddRange(alert.Hosts);
            }
        }

        public override string AppliesTo
        {
            get
            {
                List<string> names = new List<string>();

                lock (hostsLock)
                {
                    foreach (Host host in hosts)
                        names.Add(host.Name());
                }

                lock (connectionsLock)
                {
                    foreach (IXenConnection connection in connections)
                        names.Add(Helpers.GetName(connection));
                }

                return string.Join(", ", names.ToArray());
            }
        }

        private bool AllowedToDismiss(IXenConnection connection)
        {
            if (connection == null)
                return true;

            // if we are disconnected do not dismiss as the alert will disappear soon
            if (connection.Session == null)
                return false;

            // prevent dismissal if the alert is not relevant to the current connection
            if (!Connections.Contains(connection) && DistinctHosts.All(h => h.Connection != connection))
                return false;

            if (connection.Session.IsLocalSuperuser)
                return true;

            var allowedRoles = Role.ValidRoleList("pool.set_other_config", connection);
            return allowedRoles.Any(r => connection.Session.Roles.Contains(r));
        }

        public override bool AllowedToDismiss()
        {
            return !Dismissing && ConnectionsManager.XenConnectionsCopy.Any(AllowedToDismiss);
        }

        public override void Dismiss()
        {
            foreach (IXenConnection connection in ConnectionsManager.XenConnectionsCopy)
            {
                if (!AllowedToDismiss(connection))
                    continue;

                Pool pool = Helpers.GetPoolOfOne(connection);
                if (pool == null)
                    continue;

                var otherConfig = pool.other_config;
                Dismiss(otherConfig);
                Pool.set_other_config(connection.Session, pool.opaque_ref, otherConfig);
            }

            Updates.RemoveUpdate(this);
        }
        
        public override bool IsDismissed()
        {
            lock (connectionsLock)
            {
                return connections.Any(IsDismissed);
            }
        }

        protected abstract void Dismiss(Dictionary<string, string> otherConfig);

        protected abstract bool IsDismissed(IXenConnection connection);
    }
}
