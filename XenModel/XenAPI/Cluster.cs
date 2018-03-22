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
    /// Cluster-wide Cluster metadata
    /// </summary>
    public partial class Cluster : XenObject<Cluster>
    {
        public Cluster()
        {
        }

        public Cluster(string uuid,
            List<XenRef<Cluster_host>> cluster_hosts,
            XenRef<Network> network,
            string cluster_token,
            string cluster_stack,
            List<cluster_operation> allowed_operations,
            Dictionary<string, cluster_operation> current_operations,
            bool pool_auto_join,
            long token_timeout,
            long token_timeout_coefficient,
            Dictionary<string, string> cluster_config,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.cluster_hosts = cluster_hosts;
            this.network = network;
            this.cluster_token = cluster_token;
            this.cluster_stack = cluster_stack;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.pool_auto_join = pool_auto_join;
            this.token_timeout = token_timeout;
            this.token_timeout_coefficient = token_timeout_coefficient;
            this.cluster_config = cluster_config;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Cluster from a Proxy_Cluster.
        /// </summary>
        /// <param name="proxy"></param>
        public Cluster(Proxy_Cluster proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        /// <summary>
        /// Updates each field of this instance with the value of
        /// the corresponding field of a given Cluster.
        /// </summary>
        public override void UpdateFrom(Cluster update)
        {
            uuid = update.uuid;
            cluster_hosts = update.cluster_hosts;
            network = update.network;
            cluster_token = update.cluster_token;
            cluster_stack = update.cluster_stack;
            allowed_operations = update.allowed_operations;
            current_operations = update.current_operations;
            pool_auto_join = update.pool_auto_join;
            token_timeout = update.token_timeout;
            token_timeout_coefficient = update.token_timeout_coefficient;
            cluster_config = update.cluster_config;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Cluster proxy)
        {
            uuid = proxy.uuid == null ? null : proxy.uuid;
            cluster_hosts = proxy.cluster_hosts == null ? null : XenRef<Cluster_host>.Create(proxy.cluster_hosts);
            network = proxy.network == null ? null : XenRef<Network>.Create(proxy.network);
            cluster_token = proxy.cluster_token == null ? null : proxy.cluster_token;
            cluster_stack = proxy.cluster_stack == null ? null : proxy.cluster_stack;
            allowed_operations = proxy.allowed_operations == null ? null : Helper.StringArrayToEnumList<cluster_operation>(proxy.allowed_operations);
            current_operations = proxy.current_operations == null ? null : Maps.convert_from_proxy_string_cluster_operation(proxy.current_operations);
            pool_auto_join = (bool)proxy.pool_auto_join;
            token_timeout = proxy.token_timeout == null ? 0 : long.Parse(proxy.token_timeout);
            token_timeout_coefficient = proxy.token_timeout_coefficient == null ? 0 : long.Parse(proxy.token_timeout_coefficient);
            cluster_config = proxy.cluster_config == null ? null : Maps.convert_from_proxy_string_string(proxy.cluster_config);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Cluster ToProxy()
        {
            Proxy_Cluster result_ = new Proxy_Cluster();
            result_.uuid = uuid ?? "";
            result_.cluster_hosts = cluster_hosts == null ? new string[] {} : Helper.RefListToStringArray(cluster_hosts);
            result_.network = network ?? "";
            result_.cluster_token = cluster_token ?? "";
            result_.cluster_stack = cluster_stack ?? "";
            result_.allowed_operations = allowed_operations == null ? new string[] {} : Helper.ObjectListToStringArray(allowed_operations);
            result_.current_operations = Maps.convert_to_proxy_string_cluster_operation(current_operations);
            result_.pool_auto_join = pool_auto_join;
            result_.token_timeout = token_timeout.ToString();
            result_.token_timeout_coefficient = token_timeout_coefficient.ToString();
            result_.cluster_config = Maps.convert_to_proxy_string_string(cluster_config);
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Cluster from a Hashtable.
        /// Note that the fields not contained in the Hashtable
        /// will be created with their default values.
        /// </summary>
        /// <param name="table"></param>
        public Cluster(Hashtable table) : this()
        {
            UpdateFrom(table);
        }

        /// <summary>
        /// Given a Hashtable with field-value pairs, it updates the fields of this Cluster
        /// with the values listed in the Hashtable. Note that only the fields contained
        /// in the Hashtable will be updated and the rest will remain the same.
        /// </summary>
        /// <param name="table"></param>
        public void UpdateFrom(Hashtable table)
        {
            if (table.ContainsKey("uuid"))
                uuid = Marshalling.ParseString(table, "uuid");
            if (table.ContainsKey("cluster_hosts"))
                cluster_hosts = Marshalling.ParseSetRef<Cluster_host>(table, "cluster_hosts");
            if (table.ContainsKey("network"))
                network = Marshalling.ParseRef<Network>(table, "network");
            if (table.ContainsKey("cluster_token"))
                cluster_token = Marshalling.ParseString(table, "cluster_token");
            if (table.ContainsKey("cluster_stack"))
                cluster_stack = Marshalling.ParseString(table, "cluster_stack");
            if (table.ContainsKey("allowed_operations"))
                allowed_operations = Helper.StringArrayToEnumList<cluster_operation>(Marshalling.ParseStringArray(table, "allowed_operations"));
            if (table.ContainsKey("current_operations"))
                current_operations = Maps.convert_from_proxy_string_cluster_operation(Marshalling.ParseHashTable(table, "current_operations"));
            if (table.ContainsKey("pool_auto_join"))
                pool_auto_join = Marshalling.ParseBool(table, "pool_auto_join");
            if (table.ContainsKey("token_timeout"))
                token_timeout = Marshalling.ParseLong(table, "token_timeout");
            if (table.ContainsKey("token_timeout_coefficient"))
                token_timeout_coefficient = Marshalling.ParseLong(table, "token_timeout_coefficient");
            if (table.ContainsKey("cluster_config"))
                cluster_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "cluster_config"));
            if (table.ContainsKey("other_config"))
                other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Cluster other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(this.current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._cluster_hosts, other._cluster_hosts) &&
                Helper.AreEqual2(this._network, other._network) &&
                Helper.AreEqual2(this._cluster_token, other._cluster_token) &&
                Helper.AreEqual2(this._cluster_stack, other._cluster_stack) &&
                Helper.AreEqual2(this._allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(this._pool_auto_join, other._pool_auto_join) &&
                Helper.AreEqual2(this._token_timeout, other._token_timeout) &&
                Helper.AreEqual2(this._token_timeout_coefficient, other._token_timeout_coefficient) &&
                Helper.AreEqual2(this._cluster_config, other._cluster_config) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        internal static List<Cluster> ProxyArrayToObjectList(Proxy_Cluster[] input)
        {
            var result = new List<Cluster>();
            foreach (var item in input)
                result.Add(new Cluster(item));

            return result;
        }

        public override string SaveChanges(Session session, string opaqueRef, Cluster server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    Cluster.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static Cluster get_record(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_record(session.opaque_ref, _cluster);
            else
                return new Cluster((Proxy_Cluster)session.proxy.cluster_get_record(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Get a reference to the Cluster instance with the specified UUID.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Cluster> get_by_uuid(Session session, string _uuid)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_by_uuid(session.opaque_ref, _uuid);
            else
                return XenRef<Cluster>.Create(session.proxy.cluster_get_by_uuid(session.opaque_ref, _uuid ?? "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static string get_uuid(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_uuid(session.opaque_ref, _cluster);
            else
                return session.proxy.cluster_get_uuid(session.opaque_ref, _cluster ?? "").parse();
        }

        /// <summary>
        /// Get the cluster_hosts field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static List<XenRef<Cluster_host>> get_cluster_hosts(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_cluster_hosts(session.opaque_ref, _cluster);
            else
                return XenRef<Cluster_host>.Create(session.proxy.cluster_get_cluster_hosts(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Get the network field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static XenRef<Network> get_network(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_network(session.opaque_ref, _cluster);
            else
                return XenRef<Network>.Create(session.proxy.cluster_get_network(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Get the cluster_token field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static string get_cluster_token(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_cluster_token(session.opaque_ref, _cluster);
            else
                return session.proxy.cluster_get_cluster_token(session.opaque_ref, _cluster ?? "").parse();
        }

        /// <summary>
        /// Get the cluster_stack field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static string get_cluster_stack(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_cluster_stack(session.opaque_ref, _cluster);
            else
                return session.proxy.cluster_get_cluster_stack(session.opaque_ref, _cluster ?? "").parse();
        }

        /// <summary>
        /// Get the allowed_operations field of the given Cluster.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static List<cluster_operation> get_allowed_operations(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_allowed_operations(session.opaque_ref, _cluster);
            else
                return Helper.StringArrayToEnumList<cluster_operation>(session.proxy.cluster_get_allowed_operations(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Get the current_operations field of the given Cluster.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static Dictionary<string, cluster_operation> get_current_operations(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_current_operations(session.opaque_ref, _cluster);
            else
                return Maps.convert_from_proxy_string_cluster_operation(session.proxy.cluster_get_current_operations(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Get the pool_auto_join field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static bool get_pool_auto_join(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_pool_auto_join(session.opaque_ref, _cluster);
            else
                return (bool)session.proxy.cluster_get_pool_auto_join(session.opaque_ref, _cluster ?? "").parse();
        }

        /// <summary>
        /// Get the token_timeout field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static long get_token_timeout(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_token_timeout(session.opaque_ref, _cluster);
            else
                return long.Parse(session.proxy.cluster_get_token_timeout(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Get the token_timeout_coefficient field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static long get_token_timeout_coefficient(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_token_timeout_coefficient(session.opaque_ref, _cluster);
            else
                return long.Parse(session.proxy.cluster_get_token_timeout_coefficient(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Get the cluster_config field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static Dictionary<string, string> get_cluster_config(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_cluster_config(session.opaque_ref, _cluster);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.cluster_get_cluster_config(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static Dictionary<string, string> get_other_config(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_other_config(session.opaque_ref, _cluster);
            else
                return Maps.convert_from_proxy_string_string(session.proxy.cluster_get_other_config(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Set the other_config field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _cluster, Dictionary<string, string> _other_config)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.cluster_set_other_config(session.opaque_ref, _cluster, _other_config);
            else
                session.proxy.cluster_set_other_config(session.opaque_ref, _cluster ?? "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _cluster, string _key, string _value)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.cluster_add_to_other_config(session.opaque_ref, _cluster, _key, _value);
            else
                session.proxy.cluster_add_to_other_config(session.opaque_ref, _cluster ?? "", _key ?? "", _value ?? "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given Cluster.  If the key is not in that Map, then do nothing.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _cluster, string _key)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.cluster_remove_from_other_config(session.opaque_ref, _cluster, _key);
            else
                session.proxy.cluster_remove_from_other_config(session.opaque_ref, _cluster ?? "", _key ?? "").parse();
        }

        /// <summary>
        /// Creates a Cluster object and one Cluster_host object as its first member
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">the single network on which corosync carries out its inter-host communications</param>
        /// <param name="_cluster_stack">simply the string 'corosync'. No other cluster stacks are currently supported</param>
        /// <param name="_pool_auto_join">true if xapi is automatically joining new pool members to the cluster</param>
        /// <param name="_token_timeout">Corosync token timeout in seconds</param>
        /// <param name="_token_timeout_coefficient">Corosync token timeout coefficient in seconds</param>
        public static XenRef<Cluster> create(Session session, string _network, string _cluster_stack, bool _pool_auto_join, double _token_timeout, double _token_timeout_coefficient)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_create(session.opaque_ref, _network, _cluster_stack, _pool_auto_join, _token_timeout, _token_timeout_coefficient);
            else
                return XenRef<Cluster>.Create(session.proxy.cluster_create(session.opaque_ref, _network ?? "", _cluster_stack ?? "", _pool_auto_join, _token_timeout, _token_timeout_coefficient).parse());
        }

        /// <summary>
        /// Creates a Cluster object and one Cluster_host object as its first member
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">the single network on which corosync carries out its inter-host communications</param>
        /// <param name="_cluster_stack">simply the string 'corosync'. No other cluster stacks are currently supported</param>
        /// <param name="_pool_auto_join">true if xapi is automatically joining new pool members to the cluster</param>
        /// <param name="_token_timeout">Corosync token timeout in seconds</param>
        /// <param name="_token_timeout_coefficient">Corosync token timeout coefficient in seconds</param>
        public static XenRef<Task> async_create(Session session, string _network, string _cluster_stack, bool _pool_auto_join, double _token_timeout, double _token_timeout_coefficient)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_cluster_create(session.opaque_ref, _network, _cluster_stack, _pool_auto_join, _token_timeout, _token_timeout_coefficient);
          else
              return XenRef<Task>.Create(session.proxy.async_cluster_create(session.opaque_ref, _network ?? "", _cluster_stack ?? "", _pool_auto_join, _token_timeout, _token_timeout_coefficient).parse());
        }

        /// <summary>
        /// Destroys a Cluster object and the one remaining Cluster_host member
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static void destroy(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.cluster_destroy(session.opaque_ref, _cluster);
            else
                session.proxy.cluster_destroy(session.opaque_ref, _cluster ?? "").parse();
        }

        /// <summary>
        /// Destroys a Cluster object and the one remaining Cluster_host member
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static XenRef<Task> async_destroy(Session session, string _cluster)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_cluster_destroy(session.opaque_ref, _cluster);
          else
              return XenRef<Task>.Create(session.proxy.async_cluster_destroy(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Attempt to create a Cluster from the entire pool
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">the single network on which corosync carries out its inter-host communications</param>
        /// <param name="_cluster_stack">simply the string 'corosync'. No other cluster stacks are currently supported</param>
        /// <param name="_token_timeout">Corosync token timeout in seconds</param>
        /// <param name="_token_timeout_coefficient">Corosync token timeout coefficient in seconds</param>
        public static XenRef<Cluster> pool_create(Session session, string _network, string _cluster_stack, double _token_timeout, double _token_timeout_coefficient)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_pool_create(session.opaque_ref, _network, _cluster_stack, _token_timeout, _token_timeout_coefficient);
            else
                return XenRef<Cluster>.Create(session.proxy.cluster_pool_create(session.opaque_ref, _network ?? "", _cluster_stack ?? "", _token_timeout, _token_timeout_coefficient).parse());
        }

        /// <summary>
        /// Attempt to create a Cluster from the entire pool
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_network">the single network on which corosync carries out its inter-host communications</param>
        /// <param name="_cluster_stack">simply the string 'corosync'. No other cluster stacks are currently supported</param>
        /// <param name="_token_timeout">Corosync token timeout in seconds</param>
        /// <param name="_token_timeout_coefficient">Corosync token timeout coefficient in seconds</param>
        public static XenRef<Task> async_pool_create(Session session, string _network, string _cluster_stack, double _token_timeout, double _token_timeout_coefficient)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_cluster_pool_create(session.opaque_ref, _network, _cluster_stack, _token_timeout, _token_timeout_coefficient);
          else
              return XenRef<Task>.Create(session.proxy.async_cluster_pool_create(session.opaque_ref, _network ?? "", _cluster_stack ?? "", _token_timeout, _token_timeout_coefficient).parse());
        }

        /// <summary>
        /// Attempt to force destroy the Cluster_host objects, and then destroy the Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static void pool_force_destroy(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.cluster_pool_force_destroy(session.opaque_ref, _cluster);
            else
                session.proxy.cluster_pool_force_destroy(session.opaque_ref, _cluster ?? "").parse();
        }

        /// <summary>
        /// Attempt to force destroy the Cluster_host objects, and then destroy the Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static XenRef<Task> async_pool_force_destroy(Session session, string _cluster)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_cluster_pool_force_destroy(session.opaque_ref, _cluster);
          else
              return XenRef<Task>.Create(session.proxy.async_cluster_pool_force_destroy(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Attempt to destroy the Cluster_host objects for all hosts in the pool and then destroy the Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static void pool_destroy(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.cluster_pool_destroy(session.opaque_ref, _cluster);
            else
                session.proxy.cluster_pool_destroy(session.opaque_ref, _cluster ?? "").parse();
        }

        /// <summary>
        /// Attempt to destroy the Cluster_host objects for all hosts in the pool and then destroy the Cluster.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static XenRef<Task> async_pool_destroy(Session session, string _cluster)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_cluster_pool_destroy(session.opaque_ref, _cluster);
          else
              return XenRef<Task>.Create(session.proxy.async_cluster_pool_destroy(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Resynchronise the cluster_host objects across the pool. Creates them where they need creating and then plugs them
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static void pool_resync(Session session, string _cluster)
        {
            if (session.JsonRpcClient != null)
                session.JsonRpcClient.cluster_pool_resync(session.opaque_ref, _cluster);
            else
                session.proxy.cluster_pool_resync(session.opaque_ref, _cluster ?? "").parse();
        }

        /// <summary>
        /// Resynchronise the cluster_host objects across the pool. Creates them where they need creating and then plugs them
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_cluster">The opaque_ref of the given cluster</param>
        public static XenRef<Task> async_pool_resync(Session session, string _cluster)
        {
          if (session.JsonRpcClient != null)
              return session.JsonRpcClient.async_cluster_pool_resync(session.opaque_ref, _cluster);
          else
              return XenRef<Task>.Create(session.proxy.async_cluster_pool_resync(session.opaque_ref, _cluster ?? "").parse());
        }

        /// <summary>
        /// Return a list of all the Clusters known to the system.
        /// Experimental. First published in Unreleased.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Cluster>> get_all(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_all(session.opaque_ref);
            else
                return XenRef<Cluster>.Create(session.proxy.cluster_get_all(session.opaque_ref).parse());
        }

        /// <summary>
        /// Get all the Cluster Records at once, in a single XML RPC call
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Cluster>, Cluster> get_all_records(Session session)
        {
            if (session.JsonRpcClient != null)
                return session.JsonRpcClient.cluster_get_all_records(session.opaque_ref);
            else
                return XenRef<Cluster>.Create<Proxy_Cluster>(session.proxy.cluster_get_all_records(session.opaque_ref).parse());
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
        /// A list of the cluster_host objects associated with the Cluster
        /// Experimental. First published in Unreleased.
        /// </summary>
        [JsonConverter(typeof(XenRefListConverter<Cluster_host>))]
        public virtual List<XenRef<Cluster_host>> cluster_hosts
        {
            get { return _cluster_hosts; }
            set
            {
                if (!Helper.AreEqual(value, _cluster_hosts))
                {
                    _cluster_hosts = value;
                    Changed = true;
                    NotifyPropertyChanged("cluster_hosts");
                }
            }
        }
        private List<XenRef<Cluster_host>> _cluster_hosts = new List<XenRef<Cluster_host>>() {};

        /// <summary>
        /// Reference to the single network on which corosync carries out its inter-host communications
        /// Experimental. First published in Unreleased.
        /// </summary>
        [JsonConverter(typeof(XenRefConverter<Network>))]
        public virtual XenRef<Network> network
        {
            get { return _network; }
            set
            {
                if (!Helper.AreEqual(value, _network))
                {
                    _network = value;
                    Changed = true;
                    NotifyPropertyChanged("network");
                }
            }
        }
        private XenRef<Network> _network = new XenRef<Network>("OpaqueRef:NULL");

        /// <summary>
        /// The secret key used by xapi-clusterd when it talks to itself on other hosts
        /// Experimental. First published in Unreleased.
        /// </summary>
        public virtual string cluster_token
        {
            get { return _cluster_token; }
            set
            {
                if (!Helper.AreEqual(value, _cluster_token))
                {
                    _cluster_token = value;
                    Changed = true;
                    NotifyPropertyChanged("cluster_token");
                }
            }
        }
        private string _cluster_token = "";

        /// <summary>
        /// Simply the string 'corosync'. No other cluster stacks are currently supported
        /// Experimental. First published in Unreleased.
        /// </summary>
        public virtual string cluster_stack
        {
            get { return _cluster_stack; }
            set
            {
                if (!Helper.AreEqual(value, _cluster_stack))
                {
                    _cluster_stack = value;
                    Changed = true;
                    NotifyPropertyChanged("cluster_stack");
                }
            }
        }
        private string _cluster_stack = "corosync";

        /// <summary>
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<cluster_operation> allowed_operations
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
        private List<cluster_operation> _allowed_operations = new List<cluster_operation>() {};

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, cluster_operation> current_operations
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
        private Dictionary<string, cluster_operation> _current_operations = new Dictionary<string, cluster_operation>() {};

        /// <summary>
        /// True if xapi is automatically joining new pool members to the cluster. This will be `true` in the first release
        /// Experimental. First published in Unreleased.
        /// </summary>
        public virtual bool pool_auto_join
        {
            get { return _pool_auto_join; }
            set
            {
                if (!Helper.AreEqual(value, _pool_auto_join))
                {
                    _pool_auto_join = value;
                    Changed = true;
                    NotifyPropertyChanged("pool_auto_join");
                }
            }
        }
        private bool _pool_auto_join = true;

        /// <summary>
        /// The corosync token timeout in ms
        /// Experimental. First published in Unreleased.
        /// </summary>
        public virtual long token_timeout
        {
            get { return _token_timeout; }
            set
            {
                if (!Helper.AreEqual(value, _token_timeout))
                {
                    _token_timeout = value;
                    Changed = true;
                    NotifyPropertyChanged("token_timeout");
                }
            }
        }
        private long _token_timeout = 20000;

        /// <summary>
        /// The corosync token timeout coefficient in ms
        /// Experimental. First published in Unreleased.
        /// </summary>
        public virtual long token_timeout_coefficient
        {
            get { return _token_timeout_coefficient; }
            set
            {
                if (!Helper.AreEqual(value, _token_timeout_coefficient))
                {
                    _token_timeout_coefficient = value;
                    Changed = true;
                    NotifyPropertyChanged("token_timeout_coefficient");
                }
            }
        }
        private long _token_timeout_coefficient = 1000;

        /// <summary>
        /// Contains read-only settings for the cluster, such as timeouts and other options. It can only be set at cluster create time
        /// Experimental. First published in Unreleased.
        /// </summary>
        [JsonConverter(typeof(StringStringMapConverter))]
        public virtual Dictionary<string, string> cluster_config
        {
            get { return _cluster_config; }
            set
            {
                if (!Helper.AreEqual(value, _cluster_config))
                {
                    _cluster_config = value;
                    Changed = true;
                    NotifyPropertyChanged("cluster_config");
                }
            }
        }
        private Dictionary<string, string> _cluster_config = new Dictionary<string, string>() {};

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
