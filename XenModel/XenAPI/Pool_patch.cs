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
    public partial class Pool_patch : XenObject<Pool_patch>
    {
        public Pool_patch()
        {
        }

        public Pool_patch(string uuid,
            string name_label,
            string name_description,
            string version,
            long size,
            bool pool_applied,
            List<XenRef<Host_patch>> host_patches,
            List<after_apply_guidance> after_apply_guidance,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.version = version;
            this.size = size;
            this.pool_applied = pool_applied;
            this.host_patches = host_patches;
            this.after_apply_guidance = after_apply_guidance;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Pool_patch from a Proxy_Pool_patch.
        /// </summary>
        /// <param name="proxy"></param>
        public Pool_patch(Proxy_Pool_patch proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Pool_patch update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            version = update.version;
            size = update.size;
            pool_applied = update.pool_applied;
            host_patches = update.host_patches;
            after_apply_guidance = update.after_apply_guidance;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Pool_patch proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            version = proxy.version == null ? null : (string)proxy.version;
            size = proxy.size == null ? 0 : long.Parse((string)proxy.size);
            pool_applied = (bool)proxy.pool_applied;
            host_patches = proxy.host_patches == null ? null : XenRef<Host_patch>.Create(proxy.host_patches);
            after_apply_guidance = proxy.after_apply_guidance == null ? null : Helper.StringArrayToEnumList<after_apply_guidance>(proxy.after_apply_guidance);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Pool_patch ToProxy()
        {
            Proxy_Pool_patch result_ = new Proxy_Pool_patch();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.version = (version != null) ? version : "";
            result_.size = size.ToString();
            result_.pool_applied = pool_applied;
            result_.host_patches = (host_patches != null) ? Helper.RefListToStringArray(host_patches) : new string[] {};
            result_.after_apply_guidance = (after_apply_guidance != null) ? Helper.ObjectListToStringArray(after_apply_guidance) : new string[] {};
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Pool_patch from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Pool_patch(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            version = Marshalling.ParseString(table, "version");
            size = Marshalling.ParseLong(table, "size");
            pool_applied = Marshalling.ParseBool(table, "pool_applied");
            host_patches = Marshalling.ParseSetRef<Host_patch>(table, "host_patches");
            after_apply_guidance = Helper.StringArrayToEnumList<after_apply_guidance>(Marshalling.ParseStringArray(table, "after_apply_guidance"));
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Pool_patch other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._version, other._version) &&
                Helper.AreEqual2(this._size, other._size) &&
                Helper.AreEqual2(this._pool_applied, other._pool_applied) &&
                Helper.AreEqual2(this._host_patches, other._host_patches) &&
                Helper.AreEqual2(this._after_apply_guidance, other._after_apply_guidance) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, Pool_patch server)
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
                    Pool_patch.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }

        public static Pool_patch get_record(Session session, string _pool_patch)
        {
            return new Pool_patch((Proxy_Pool_patch)session.proxy.pool_patch_get_record(session.uuid, (_pool_patch != null) ? _pool_patch : "").parse());
        }

        public static XenRef<Pool_patch> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Pool_patch>.Create(session.proxy.pool_patch_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static List<XenRef<Pool_patch>> get_by_name_label(Session session, string _label)
        {
            return XenRef<Pool_patch>.Create(session.proxy.pool_patch_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        public static string get_uuid(Session session, string _pool_patch)
        {
            return (string)session.proxy.pool_patch_get_uuid(session.uuid, (_pool_patch != null) ? _pool_patch : "").parse();
        }

        public static string get_name_label(Session session, string _pool_patch)
        {
            return (string)session.proxy.pool_patch_get_name_label(session.uuid, (_pool_patch != null) ? _pool_patch : "").parse();
        }

        public static string get_name_description(Session session, string _pool_patch)
        {
            return (string)session.proxy.pool_patch_get_name_description(session.uuid, (_pool_patch != null) ? _pool_patch : "").parse();
        }

        public static string get_version(Session session, string _pool_patch)
        {
            return (string)session.proxy.pool_patch_get_version(session.uuid, (_pool_patch != null) ? _pool_patch : "").parse();
        }

        public static long get_size(Session session, string _pool_patch)
        {
            return long.Parse((string)session.proxy.pool_patch_get_size(session.uuid, (_pool_patch != null) ? _pool_patch : "").parse());
        }

        public static bool get_pool_applied(Session session, string _pool_patch)
        {
            return (bool)session.proxy.pool_patch_get_pool_applied(session.uuid, (_pool_patch != null) ? _pool_patch : "").parse();
        }

        public static List<XenRef<Host_patch>> get_host_patches(Session session, string _pool_patch)
        {
            return XenRef<Host_patch>.Create(session.proxy.pool_patch_get_host_patches(session.uuid, (_pool_patch != null) ? _pool_patch : "").parse());
        }

        public static List<after_apply_guidance> get_after_apply_guidance(Session session, string _pool_patch)
        {
            return Helper.StringArrayToEnumList<after_apply_guidance>(session.proxy.pool_patch_get_after_apply_guidance(session.uuid, (_pool_patch != null) ? _pool_patch : "").parse());
        }

        public static Dictionary<string, string> get_other_config(Session session, string _pool_patch)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pool_patch_get_other_config(session.uuid, (_pool_patch != null) ? _pool_patch : "").parse());
        }

        public static void set_other_config(Session session, string _pool_patch, Dictionary<string, string> _other_config)
        {
            session.proxy.pool_patch_set_other_config(session.uuid, (_pool_patch != null) ? _pool_patch : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _pool_patch, string _key, string _value)
        {
            session.proxy.pool_patch_add_to_other_config(session.uuid, (_pool_patch != null) ? _pool_patch : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _pool_patch, string _key)
        {
            session.proxy.pool_patch_remove_from_other_config(session.uuid, (_pool_patch != null) ? _pool_patch : "", (_key != null) ? _key : "").parse();
        }

        public static string apply(Session session, string _self, string _host)
        {
            return (string)session.proxy.pool_patch_apply(session.uuid, (_self != null) ? _self : "", (_host != null) ? _host : "").parse();
        }

        public static XenRef<Task> async_apply(Session session, string _self, string _host)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_patch_apply(session.uuid, (_self != null) ? _self : "", (_host != null) ? _host : "").parse());
        }

        public static void pool_apply(Session session, string _self)
        {
            session.proxy.pool_patch_pool_apply(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_pool_apply(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_patch_pool_apply(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static string precheck(Session session, string _self, string _host)
        {
            return (string)session.proxy.pool_patch_precheck(session.uuid, (_self != null) ? _self : "", (_host != null) ? _host : "").parse();
        }

        public static XenRef<Task> async_precheck(Session session, string _self, string _host)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_patch_precheck(session.uuid, (_self != null) ? _self : "", (_host != null) ? _host : "").parse());
        }

        public static void clean(Session session, string _self)
        {
            session.proxy.pool_patch_clean(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_clean(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_patch_clean(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void pool_clean(Session session, string _self)
        {
            session.proxy.pool_patch_pool_clean(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_pool_clean(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_patch_pool_clean(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void destroy(Session session, string _self)
        {
            session.proxy.pool_patch_destroy(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_patch_destroy(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void clean_on_host(Session session, string _self, string _host)
        {
            session.proxy.pool_patch_clean_on_host(session.uuid, (_self != null) ? _self : "", (_host != null) ? _host : "").parse();
        }

        public static XenRef<Task> async_clean_on_host(Session session, string _self, string _host)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_patch_clean_on_host(session.uuid, (_self != null) ? _self : "", (_host != null) ? _host : "").parse());
        }

        public static List<XenRef<Pool_patch>> get_all(Session session)
        {
            return XenRef<Pool_patch>.Create(session.proxy.pool_patch_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<Pool_patch>, Pool_patch> get_all_records(Session session)
        {
            return XenRef<Pool_patch>.Create<Proxy_Pool_patch>(session.proxy.pool_patch_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private string _name_label;
        public virtual string name_label {
             get { return _name_label; }
             set { if (!Helper.AreEqual(value, _name_label)) { _name_label = value; Changed = true; NotifyPropertyChanged("name_label"); } }
         }

        private string _name_description;
        public virtual string name_description {
             get { return _name_description; }
             set { if (!Helper.AreEqual(value, _name_description)) { _name_description = value; Changed = true; NotifyPropertyChanged("name_description"); } }
         }

        private string _version;
        public virtual string version {
             get { return _version; }
             set { if (!Helper.AreEqual(value, _version)) { _version = value; Changed = true; NotifyPropertyChanged("version"); } }
         }

        private long _size;
        public virtual long size {
             get { return _size; }
             set { if (!Helper.AreEqual(value, _size)) { _size = value; Changed = true; NotifyPropertyChanged("size"); } }
         }

        private bool _pool_applied;
        public virtual bool pool_applied {
             get { return _pool_applied; }
             set { if (!Helper.AreEqual(value, _pool_applied)) { _pool_applied = value; Changed = true; NotifyPropertyChanged("pool_applied"); } }
         }

        private List<XenRef<Host_patch>> _host_patches;
        public virtual List<XenRef<Host_patch>> host_patches {
             get { return _host_patches; }
             set { if (!Helper.AreEqual(value, _host_patches)) { _host_patches = value; Changed = true; NotifyPropertyChanged("host_patches"); } }
         }

        private List<after_apply_guidance> _after_apply_guidance;
        public virtual List<after_apply_guidance> after_apply_guidance {
             get { return _after_apply_guidance; }
             set { if (!Helper.AreEqual(value, _after_apply_guidance)) { _after_apply_guidance = value; Changed = true; NotifyPropertyChanged("after_apply_guidance"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }


    }
}
