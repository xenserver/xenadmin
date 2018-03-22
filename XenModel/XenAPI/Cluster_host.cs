/*
 * Copyright (c) Citrix Systems, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 *   1) Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *
 *   2) Redistributions in binary form must reproduce the above
 *      copyright notice, this list of conditions and the following
 *      disclaimer in the documentation and/or other materials
 *      provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace XenAPI
{
    /// <summary>
    /// Cluster member metadata
    /// </summary>
    public partial class Cluster_host : XenObject<Cluster_host>
    {
        public Cluster_host()
        {
        }

        public Cluster_host(string uuid,
            XenRef<Cluster> cluster,
            XenRef<Host> host,
            bool enabled,
            List<cluster_host_operation> allowed_operations,
            Dictionary<string, cluster_host_operation> current_operations,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.cluster = cluster;
            this.host = host;
            this.enabled = enabled;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Cluster_host from a Proxy_Cluster_host.
        /// </summary>
        /// <param name="proxy"></param>
        public Cluster_host(Proxy_Cluster_host proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Cluster_host.
        /// </summary>
        public override void UpdateFrom(Cluster_host update)
        {
            uuid = update.uuid;
            cluster = update.cluster;
            host = update.host;
            enabled = update.enabled;
            allowed_operations = update.allowed_operations;
            current_operations = update.current_operations;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Cluster_host proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            cluster = proxy.cluster == null ? null : XenRef<Cluster>.Create(proxy.cluster);
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            enabled = (bool)proxy.enabled;
            allowed_operations = proxy.allowed_operations == null ? null : Helper.StringArrayToEnumList<cluster_host_operation>(proxy.allowed_operations);
            current_operations = proxy.current_operations == null ? null : Maps.convert_from_proxy_string_cluster_host_operation(proxy.current_operations);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Cluster_host ToProxy()
        {
            Proxy_Cluster_host result_ = new Proxy_Cluster_host();
            result_.uuid = uuid ?? "";
            result_.cluster = cluster ?? "";
            result_.host = host ?? "";
            result_.enabled = enabled;
            result_.allowed_operations = allowed_operations == null ? new string[] {} : Helper.ObjectListToStringArray(allowed_operations);
            result_.current_operations = Maps.convert_to_proxy_string_cluster_host_operation(current_operations);
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Cluster_host from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Cluster_host(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Cluster_host
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("cluster"))
                cluster = Marshalling.ParseRef<Cluster>(table, "cluster");
            if (table.ContainsKey("host"))
                host = Marshalling.ParseRef<Host>(table, "host");
            if (table.ContainsKey("enabled"))
                enabled = Marshalling.ParseBool(table, "enabled");
            if (table.ContainsKey("allowed_operations"))
                allowed_operations = Helper.StringArrayToEnumList<cluster_host_operation>(Marshalling.ParseStringArray(table, "allowed_operations"));
            if (table.ContainsKey("current_operations"))
                current_operations = Maps.convert_from_proxy_string_cluster_host_operation(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Cluster_host other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(this.current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._cluster, other._cluster) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._enabled, other._enabled) &&
                Helper.AreEqual2(this._allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        internal static List<Cluster_host> ProxyArrayToObjectList(Proxy_Cluster_host[] input)
        {
            var result = new List<Cluster_host>();
            foreach (var item in input)
                result.Add(new Cluster_host(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Cluster_host server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
              throw new InvalidOperationException("This type has no read/write properties");
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given Cluster_host.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static Cluster_host get_record(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_get_record(session.opaque_ref, _cluster_host);
            else
                return new Cluster_host((Proxy_Cluster_host)session.proxy.cluster_host_get_record(session.opaque_ref, _cluster_host ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the Cluster_host instance with the specified UUID.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Cluster_host> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<Cluster_host>.Create(session.proxy.cluster_host_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given Cluster_host.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static string get_uuid(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_get_uuid(session.opaque_ref, _cluster_host);
            else
                return session.proxy.cluster_host_get_uuid(session.opaque_ref, _cluster_host ?? "").parse();
        }

        /// <summary>
        /// Get the cluster field of the given Cluster_host.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static XenRef<Cluster> get_cluster(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_get_cluster(session.opaque_ref, _cluster_host);
            else
                return XenRef<Cluster>.Create(session.proxy.cluster_host_get_cluster(session.opaque_ref, _cluster_host ?? "").parse());
        }

        /// <summary>
        /// Get the host field of the given Cluster_host.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static XenRef<Host> get_host(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_get_host(session.opaque_ref, _cluster_host);
            else
                return XenRef<Host>.Create(session.proxy.cluster_host_get_host(session.opaque_ref, _cluster_host ?? "").parse());
        }

        /// <summary>
        /// Get the enabled field of the given Cluster_host.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static bool get_enabled(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_get_enabled(session.opaque_ref, _cluster_host);
            else
                return (bool)session.proxy.cluster_host_get_enabled(session.opaque_ref, _cluster_host ?? "").parse();
        }

        /// <summary>
        /// Get the allowed_operations field of the given Cluster_host.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static List<cluster_host_operation> get_allowed_operations(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_get_allowed_operations(session.opaque_ref, _cluster_host);
            else
                return Helper.StringArrayToEnumList<cluster_host_operation>(session.proxy.cluster_host_get_allowed_operations(session.opaque_ref, _cluster_host ?? "").parse());
        }

        /// <summary>
        /// Get the current_operations field of the given Cluster_host.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static Dictionary<string, cluster_host_operation> get_current_operations(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_get_current_operations(session.opaque_ref, _cluster_host);
            else
                return Maps.convert_from_proxy_string_cluster_host_operation(session.proxy.cluster_host_get_current_operations(session.opaque_ref, _cluster_host ?? "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given Cluster_host.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static Dictionary<string, string> get_other_config(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_get_other_config(session.opaque_ref, _cluster_host);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.cluster_host_get_other_config(session.opaque_ref, _cluster_host ?? "").parse());
        }

        /// <summary>
        /// Add a new host to an existing cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">Cluster to join</param>
        /// <param name="_host">new cluster member</param>
        public static XenRef<Cluster_host> create(Session session, string _cluster, string _host)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_create(session.opaque_ref, _cluster, _host);
            else
                return XenRef<Cluster_host>.Create(session.proxy.cluster_host_create(session.opaque_ref, _cluster ?? "", _host ?? "").parse());
        }

        /// <summary>
        /// Add a new host to an existing cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">Cluster to join</param>
        /// <param name="_host">new cluster member</param>
        public static XenRef<Task> async_create(Session session, string _cluster, string _host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_cluster_host_create(session.opaque_ref, _cluster, _host);
          else
              return XenRef<Task>.Create(session.proxy.async_cluster_host_create(session.opaque_ref, _cluster ?? "", _host ?? "").parse());
        }

        /// <summary>
        /// Remove a host from an existing cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static void destroy(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.cluster_host_destroy(session.opaque_ref, _cluster_host);
            else
                session.proxy.cluster_host_destroy(session.opaque_ref, _cluster_host ?? "").parse();
        }

        /// <summary>
        /// Remove a host from an existing cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static XenRef<Task> async_destroy(Session session, string _cluster_host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_cluster_host_destroy(session.opaque_ref, _cluster_host);
          else
              return XenRef<Task>.Create(session.proxy.async_cluster_host_destroy(session.opaque_ref, _cluster_host ?? "").parse());
        }

        /// <summary>
        /// Enable cluster membership for a disabled cluster host.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static void enable(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.cluster_host_enable(session.opaque_ref, _cluster_host);
            else
                session.proxy.cluster_host_enable(session.opaque_ref, _cluster_host ?? "").parse();
        }

        /// <summary>
        /// Enable cluster membership for a disabled cluster host.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static XenRef<Task> async_enable(Session session, string _cluster_host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_cluster_host_enable(session.opaque_ref, _cluster_host);
          else
              return XenRef<Task>.Create(session.proxy.async_cluster_host_enable(session.opaque_ref, _cluster_host ?? "").parse());
        }

        /// <summary>
        /// Remove a host from an existing cluster forcefully.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static void force_destroy(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.cluster_host_force_destroy(session.opaque_ref, _cluster_host);
            else
                session.proxy.cluster_host_force_destroy(session.opaque_ref, _cluster_host ?? "").parse();
        }

        /// <summary>
        /// Remove a host from an existing cluster forcefully.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static XenRef<Task> async_force_destroy(Session session, string _cluster_host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_cluster_host_force_destroy(session.opaque_ref, _cluster_host);
          else
              return XenRef<Task>.Create(session.proxy.async_cluster_host_force_destroy(session.opaque_ref, _cluster_host ?? "").parse());
        }

        /// <summary>
        /// Disable cluster membership for an enabled cluster host.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static void disable(Session session, string _cluster_host)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.cluster_host_disable(session.opaque_ref, _cluster_host);
            else
                session.proxy.cluster_host_disable(session.opaque_ref, _cluster_host ?? "").parse();
        }

        /// <summary>
        /// Disable cluster membership for an enabled cluster host.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster_host">The opaque_ref of the given cluster_host</param>
        public static XenRef<Task> async_disable(Session session, string _cluster_host)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_cluster_host_disable(session.opaque_ref, _cluster_host);
          else
              return XenRef<Task>.Create(session.proxy.async_cluster_host_disable(session.opaque_ref, _cluster_host ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the Cluster_hosts known to the system.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Cluster_host>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_get_all(session.opaque_ref);
            else
                return XenRef<Cluster_host>.Create(session.proxy.cluster_host_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the Cluster_host Records at once, in a single XML RPC call
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Cluster_host>, Cluster_host> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_host_get_all_records(session.opaque_ref);
            else
                return XenRef<Cluster_host>.Create<Proxy_Cluster_host>(session.proxy.cluster_host_get_all_records(session.opaque_ref).parse());
        }

        /// <summary>
        /// Unique identifier/object reference
        /// Experimental. First published in Unreleased.
        /// </summary>
        public virtual string uuid
        {
            get { return _uuid; }
            set
            {
                if (!Helper.AreEqual(value, _uuid))
                {
                    _uuid = value;
                    Changed = true;
                    NotifyPropertyChanged("uuid");
                }
            }
        }
        private string _uuid = "";

        /// <summary>
        /// Reference to the Cluster object
        /// Experimental. First published in Unreleased.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Cluster>))]
        public virtual XenRef<Cluster> cluster
        {
            get { return _cluster; }
            set
            {
                if (!Helper.AreEqual(value, _cluster))
                {
                    _cluster = value;
                    Changed = true;
                    NotifyPropertyChanged("cluster");
                }
            }
        }
        private XenRef<Cluster> _cluster = new XenRef<Cluster>("OpaqueRef:NULL");

        /// <summary>
        /// Reference to the Host object
        /// Experimental. First published in Unreleased.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Host>))]
        public virtual XenRef<Host> host
        {
            get { return _host; }
            set
            {
                if (!Helper.AreEqual(value, _host))
                {
                    _host = value;
                    Changed = true;
                    NotifyPropertyChanged("host");
                }
            }
        }
        private XenRef<Host> _host = new XenRef<Host>("OpaqueRef:NULL");

        /// <summary>
        /// Whether the cluster host believes that clustering should be enabled on this host
        /// Experimental. First published in Unreleased.
        /// </summary>
        public virtual bool enabled
        {
            get { return _enabled; }
            set
            {
                if (!Helper.AreEqual(value, _enabled))
                {
                    _enabled = value;
                    Changed = true;
                    NotifyPropertyChanged("enabled");
                }
            }
        }
        private bool _enabled = false;

        /// <summary>
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<cluster_host_operation> allowed_operations
        {
            get { return _allowed_operations; }
            set
            {
                if (!Helper.AreEqual(value, _allowed_operations))
                {
                    _allowed_operations = value;
                    Changed = true;
                    NotifyPropertyChanged("allowed_operations");
                }
            }
        }
        private List<cluster_host_operation> _allowed_operations = new List<cluster_host_operation>() {};

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, cluster_host_operation> current_operations
        {
            get { return _current_operations; }
            set
            {
                if (!Helper.AreEqual(value, _current_operations))
                {
                    _current_operations = value;
                    Changed = true;
                    NotifyPropertyChanged("current_operations");
                }
            }
        }
        private Dictionary<string, cluster_host_operation> _current_operations = new Dictionary<string, cluster_host_operation>() {};

        /// <summary>
        /// Additional configuration
        /// Experimental. First published in Unreleased.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> other_config
        {
            get { return _other_config; }
            set
            {
                if (!Helper.AreEqual(value, _other_config))
                {
                    _other_config = value;
                    Changed = true;
                    NotifyPropertyChanged("other_config");
                }
            }
        }
        private Dictionary<string, string> _other_config = new Dictionary<string, string>() {};
    }
}
