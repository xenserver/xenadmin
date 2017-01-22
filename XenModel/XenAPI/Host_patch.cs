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

using CookComputing.XmlRpc;


namespace XenAPI
{
    /// <summary>
    /// Represents a patch stored on a server
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Host_patch : XenObject<Host_patch>
    {
        public Host_patch()
        {
        }

        public Host_patch(string uuid,
            string name_label,
            string name_description,
            string version,
            XenRef<Host> host,
            bool applied,
            DateTime timestamp_applied,
            long size,
            XenRef<Pool_patch> pool_patch,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.version = version;
            this.host = host;
            this.applied = applied;
            this.timestamp_applied = timestamp_applied;
            this.size = size;
            this.pool_patch = pool_patch;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Host_patch from a Proxy_Host_patch.
        /// </summary>
        /// <param name="proxy"></param>
        public Host_patch(Proxy_Host_patch proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Host_patch update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            version = update.version;
            host = update.host;
            applied = update.applied;
            timestamp_applied = update.timestamp_applied;
            size = update.size;
            pool_patch = update.pool_patch;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Host_patch proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            version = proxy.version == null ? null : (string)proxy.version;
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            applied = (bool)proxy.applied;
            timestamp_applied = proxy.timestamp_applied;
            size = proxy.size == null ? 0 : long.Parse((string)proxy.size);
            pool_patch = proxy.pool_patch == null ? null : XenRef<Pool_patch>.Create(proxy.pool_patch);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Host_patch ToProxy()
        {
            Proxy_Host_patch result_ = new Proxy_Host_patch();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.version = (version != null) ? version : "";
            result_.host = (host != null) ? host : "";
            result_.applied = applied;
            result_.timestamp_applied = timestamp_applied;
            result_.size = size.ToString();
            result_.pool_patch = (pool_patch != null) ? pool_patch : "";
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Host_patch from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Host_patch(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            version = Marshalling.ParseString(table, "version");
            host = Marshalling.ParseRef<Host>(table, "host");
            applied = Marshalling.ParseBool(table, "applied");
            timestamp_applied = Marshalling.ParseDateTime(table, "timestamp_applied");
            size = Marshalling.ParseLong(table, "size");
            pool_patch = Marshalling.ParseRef<Pool_patch>(table, "pool_patch");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Host_patch other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._version, other._version) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._applied, other._applied) &&
                Helper.AreEqual2(this._timestamp_applied, other._timestamp_applied) &&
                Helper.AreEqual2(this._size, other._size) &&
                Helper.AreEqual2(this._pool_patch, other._pool_patch) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, Host_patch server)
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
                    Host_patch.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given host_patch.
        /// First published in XenServer 4.0.
        /// Deprecated since .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        [Deprecated("")]
        public static Host_patch get_record(Session session, string _host_patch)
        {
            return new Host_patch((Proxy_Host_patch)session.proxy.host_patch_get_record(session.uuid, (_host_patch != null) ? _host_patch : "").parse());
        }

        /// <summary>
        /// Get a reference to the host_patch instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// Deprecated since .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        [Deprecated("")]
        public static XenRef<Host_patch> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Host_patch>.Create(session.proxy.host_patch_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get all the host_patch instances with the given label.
        /// First published in XenServer 4.0.
        /// Deprecated since .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        [Deprecated("")]
        public static List<XenRef<Host_patch>> get_by_name_label(Session session, string _label)
        {
            return XenRef<Host_patch>.Create(session.proxy.host_patch_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given host_patch.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        public static string get_uuid(Session session, string _host_patch)
        {
            return (string)session.proxy.host_patch_get_uuid(session.uuid, (_host_patch != null) ? _host_patch : "").parse();
        }

        /// <summary>
        /// Get the name/label field of the given host_patch.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        public static string get_name_label(Session session, string _host_patch)
        {
            return (string)session.proxy.host_patch_get_name_label(session.uuid, (_host_patch != null) ? _host_patch : "").parse();
        }

        /// <summary>
        /// Get the name/description field of the given host_patch.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        public static string get_name_description(Session session, string _host_patch)
        {
            return (string)session.proxy.host_patch_get_name_description(session.uuid, (_host_patch != null) ? _host_patch : "").parse();
        }

        /// <summary>
        /// Get the version field of the given host_patch.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        public static string get_version(Session session, string _host_patch)
        {
            return (string)session.proxy.host_patch_get_version(session.uuid, (_host_patch != null) ? _host_patch : "").parse();
        }

        /// <summary>
        /// Get the host field of the given host_patch.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        public static XenRef<Host> get_host(Session session, string _host_patch)
        {
            return XenRef<Host>.Create(session.proxy.host_patch_get_host(session.uuid, (_host_patch != null) ? _host_patch : "").parse());
        }

        /// <summary>
        /// Get the applied field of the given host_patch.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        public static bool get_applied(Session session, string _host_patch)
        {
            return (bool)session.proxy.host_patch_get_applied(session.uuid, (_host_patch != null) ? _host_patch : "").parse();
        }

        /// <summary>
        /// Get the timestamp_applied field of the given host_patch.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        public static DateTime get_timestamp_applied(Session session, string _host_patch)
        {
            return session.proxy.host_patch_get_timestamp_applied(session.uuid, (_host_patch != null) ? _host_patch : "").parse();
        }

        /// <summary>
        /// Get the size field of the given host_patch.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        public static long get_size(Session session, string _host_patch)
        {
            return long.Parse((string)session.proxy.host_patch_get_size(session.uuid, (_host_patch != null) ? _host_patch : "").parse());
        }

        /// <summary>
        /// Get the pool_patch field of the given host_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        public static XenRef<Pool_patch> get_pool_patch(Session session, string _host_patch)
        {
            return XenRef<Pool_patch>.Create(session.proxy.host_patch_get_pool_patch(session.uuid, (_host_patch != null) ? _host_patch : "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given host_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        public static Dictionary<string, string> get_other_config(Session session, string _host_patch)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.host_patch_get_other_config(session.uuid, (_host_patch != null) ? _host_patch : "").parse());
        }

        /// <summary>
        /// Set the other_config field of the given host_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _host_patch, Dictionary<string, string> _other_config)
        {
            session.proxy.host_patch_set_other_config(session.uuid, (_host_patch != null) ? _host_patch : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given host_patch.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _host_patch, string _key, string _value)
        {
            session.proxy.host_patch_add_to_other_config(session.uuid, (_host_patch != null) ? _host_patch : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given host_patch.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _host_patch, string _key)
        {
            session.proxy.host_patch_remove_from_other_config(session.uuid, (_host_patch != null) ? _host_patch : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Destroy the specified host patch, removing it from the disk. This does NOT reverse the patch
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        [Deprecated("XenServer 4.1")]
        public static void destroy(Session session, string _host_patch)
        {
            session.proxy.host_patch_destroy(session.uuid, (_host_patch != null) ? _host_patch : "").parse();
        }

        /// <summary>
        /// Destroy the specified host patch, removing it from the disk. This does NOT reverse the patch
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        [Deprecated("XenServer 4.1")]
        public static XenRef<Task> async_destroy(Session session, string _host_patch)
        {
            return XenRef<Task>.Create(session.proxy.async_host_patch_destroy(session.uuid, (_host_patch != null) ? _host_patch : "").parse());
        }

        /// <summary>
        /// Apply the selected patch and return its output
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        [Deprecated("XenServer 4.1")]
        public static string apply(Session session, string _host_patch)
        {
            return (string)session.proxy.host_patch_apply(session.uuid, (_host_patch != null) ? _host_patch : "").parse();
        }

        /// <summary>
        /// Apply the selected patch and return its output
        /// First published in XenServer 4.0.
        /// Deprecated since XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host_patch">The opaque_ref of the given host_patch</param>
        [Deprecated("XenServer 4.1")]
        public static XenRef<Task> async_apply(Session session, string _host_patch)
        {
            return XenRef<Task>.Create(session.proxy.async_host_patch_apply(session.uuid, (_host_patch != null) ? _host_patch : "").parse());
        }

        /// <summary>
        /// Return a list of all the host_patchs known to the system.
        /// First published in XenServer 4.0.
        /// Deprecated since .
        /// </summary>
        /// <param name="session">The session</param>
        [Deprecated("")]
        public static List<XenRef<Host_patch>> get_all(Session session)
        {
            return XenRef<Host_patch>.Create(session.proxy.host_patch_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the host_patch Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Host_patch>, Host_patch> get_all_records(Session session)
        {
            return XenRef<Host_patch>.Create<Proxy_Host_patch>(session.proxy.host_patch_get_all_records(session.uuid).parse());
        }

        /// <summary>
        /// Unique identifier/object reference
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
        private string _uuid;

        /// <summary>
        /// a human-readable name
        /// </summary>
        public virtual string name_label
        {
            get { return _name_label; }
            set
            {
                if (!Helper.AreEqual(value, _name_label))
                {
                    _name_label = value;
                    Changed = true;
                    NotifyPropertyChanged("name_label");
                }
            }
        }
        private string _name_label;

        /// <summary>
        /// a notes field containing human-readable description
        /// </summary>
        public virtual string name_description
        {
            get { return _name_description; }
            set
            {
                if (!Helper.AreEqual(value, _name_description))
                {
                    _name_description = value;
                    Changed = true;
                    NotifyPropertyChanged("name_description");
                }
            }
        }
        private string _name_description;

        /// <summary>
        /// Patch version number
        /// </summary>
        public virtual string version
        {
            get { return _version; }
            set
            {
                if (!Helper.AreEqual(value, _version))
                {
                    _version = value;
                    Changed = true;
                    NotifyPropertyChanged("version");
                }
            }
        }
        private string _version;

        /// <summary>
        /// Host the patch relates to
        /// </summary>
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
        private XenRef<Host> _host;

        /// <summary>
        /// True if the patch has been applied
        /// </summary>
        public virtual bool applied
        {
            get { return _applied; }
            set
            {
                if (!Helper.AreEqual(value, _applied))
                {
                    _applied = value;
                    Changed = true;
                    NotifyPropertyChanged("applied");
                }
            }
        }
        private bool _applied;

        /// <summary>
        /// Time the patch was applied
        /// </summary>
        public virtual DateTime timestamp_applied
        {
            get { return _timestamp_applied; }
            set
            {
                if (!Helper.AreEqual(value, _timestamp_applied))
                {
                    _timestamp_applied = value;
                    Changed = true;
                    NotifyPropertyChanged("timestamp_applied");
                }
            }
        }
        private DateTime _timestamp_applied;

        /// <summary>
        /// Size of the patch
        /// </summary>
        public virtual long size
        {
            get { return _size; }
            set
            {
                if (!Helper.AreEqual(value, _size))
                {
                    _size = value;
                    Changed = true;
                    NotifyPropertyChanged("size");
                }
            }
        }
        private long _size;

        /// <summary>
        /// The patch applied
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual XenRef<Pool_patch> pool_patch
        {
            get { return _pool_patch; }
            set
            {
                if (!Helper.AreEqual(value, _pool_patch))
                {
                    _pool_patch = value;
                    Changed = true;
                    NotifyPropertyChanged("pool_patch");
                }
            }
        }
        private XenRef<Pool_patch> _pool_patch;

        /// <summary>
        /// additional configuration
        /// First published in XenServer 4.1.
        /// </summary>
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
        private Dictionary<string, string> _other_config;
    }
}
