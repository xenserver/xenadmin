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
    /// Pool-wide updates to the host software
    /// First published in .
    /// </summary>
    public partial class Pool_update : XenObject<Pool_update>
    {
        public Pool_update()
        {
        }

        public Pool_update(string uuid,
            string name_label,
            string name_description,
            string version,
            long installation_size,
            string key,
            List<update_after_apply_guidance> after_apply_guidance,
            XenRef<VDI> vdi,
            List<XenRef<Host>> hosts)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.version = version;
            this.installation_size = installation_size;
            this.key = key;
            this.after_apply_guidance = after_apply_guidance;
            this.vdi = vdi;
            this.hosts = hosts;
        }

        /// <summary>
        /// Creates a new Pool_update from a Proxy_Pool_update.
        /// </summary>
        /// <param name="proxy"></param>
        public Pool_update(Proxy_Pool_update proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Pool_update update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            version = update.version;
            installation_size = update.installation_size;
            key = update.key;
            after_apply_guidance = update.after_apply_guidance;
            vdi = update.vdi;
            hosts = update.hosts;
        }

        internal void UpdateFromProxy(Proxy_Pool_update proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            version = proxy.version == null ? null : (string)proxy.version;
            installation_size = proxy.installation_size == null ? 0 : long.Parse((string)proxy.installation_size);
            key = proxy.key == null ? null : (string)proxy.key;
            after_apply_guidance = proxy.after_apply_guidance == null ? null : Helper.StringArrayToEnumList<update_after_apply_guidance>(proxy.after_apply_guidance);
            vdi = proxy.vdi == null ? null : XenRef<VDI>.Create(proxy.vdi);
            hosts = proxy.hosts == null ? null : XenRef<Host>.Create(proxy.hosts);
        }

        public Proxy_Pool_update ToProxy()
        {
            Proxy_Pool_update result_ = new Proxy_Pool_update();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.version = (version != null) ? version : "";
            result_.installation_size = installation_size.ToString();
            result_.key = (key != null) ? key : "";
            result_.after_apply_guidance = (after_apply_guidance != null) ? Helper.ObjectListToStringArray(after_apply_guidance) : new string[] {};
            result_.vdi = (vdi != null) ? vdi : "";
            result_.hosts = (hosts != null) ? Helper.RefListToStringArray(hosts) : new string[] {};
            return result_;
        }

        /// <summary>
        /// Creates a new Pool_update from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Pool_update(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            version = Marshalling.ParseString(table, "version");
            installation_size = Marshalling.ParseLong(table, "installation_size");
            key = Marshalling.ParseString(table, "key");
            after_apply_guidance = Helper.StringArrayToEnumList<update_after_apply_guidance>(Marshalling.ParseStringArray(table, "after_apply_guidance"));
            vdi = Marshalling.ParseRef<VDI>(table, "vdi");
            hosts = Marshalling.ParseSetRef<Host>(table, "hosts");
        }

        public bool DeepEquals(Pool_update other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._version, other._version) &&
                Helper.AreEqual2(this._installation_size, other._installation_size) &&
                Helper.AreEqual2(this._key, other._key) &&
                Helper.AreEqual2(this._after_apply_guidance, other._after_apply_guidance) &&
                Helper.AreEqual2(this._vdi, other._vdi) &&
                Helper.AreEqual2(this._hosts, other._hosts);
        }

        public override string SaveChanges(Session session, string opaqueRef, Pool_update server)
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
        /// Get a record containing the current state of the given pool_update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static Pool_update get_record(Session session, string _pool_update)
        {
            return new Pool_update((Proxy_Pool_update)session.proxy.pool_update_get_record(session.uuid, (_pool_update != null) ? _pool_update : "").parse());
        }

        /// <summary>
        /// Get a reference to the pool_update instance with the specified UUID.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Pool_update> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Pool_update>.Create(session.proxy.pool_update_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get all the pool_update instances with the given label.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        public static List<XenRef<Pool_update>> get_by_name_label(Session session, string _label)
        {
            return XenRef<Pool_update>.Create(session.proxy.pool_update_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given pool_update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static string get_uuid(Session session, string _pool_update)
        {
            return (string)session.proxy.pool_update_get_uuid(session.uuid, (_pool_update != null) ? _pool_update : "").parse();
        }

        /// <summary>
        /// Get the name/label field of the given pool_update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static string get_name_label(Session session, string _pool_update)
        {
            return (string)session.proxy.pool_update_get_name_label(session.uuid, (_pool_update != null) ? _pool_update : "").parse();
        }

        /// <summary>
        /// Get the name/description field of the given pool_update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static string get_name_description(Session session, string _pool_update)
        {
            return (string)session.proxy.pool_update_get_name_description(session.uuid, (_pool_update != null) ? _pool_update : "").parse();
        }

        /// <summary>
        /// Get the version field of the given pool_update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static string get_version(Session session, string _pool_update)
        {
            return (string)session.proxy.pool_update_get_version(session.uuid, (_pool_update != null) ? _pool_update : "").parse();
        }

        /// <summary>
        /// Get the installation_size field of the given pool_update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static long get_installation_size(Session session, string _pool_update)
        {
            return long.Parse((string)session.proxy.pool_update_get_installation_size(session.uuid, (_pool_update != null) ? _pool_update : "").parse());
        }

        /// <summary>
        /// Get the key field of the given pool_update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static string get_key(Session session, string _pool_update)
        {
            return (string)session.proxy.pool_update_get_key(session.uuid, (_pool_update != null) ? _pool_update : "").parse();
        }

        /// <summary>
        /// Get the after_apply_guidance field of the given pool_update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static List<update_after_apply_guidance> get_after_apply_guidance(Session session, string _pool_update)
        {
            return Helper.StringArrayToEnumList<update_after_apply_guidance>(session.proxy.pool_update_get_after_apply_guidance(session.uuid, (_pool_update != null) ? _pool_update : "").parse());
        }

        /// <summary>
        /// Get the vdi field of the given pool_update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static XenRef<VDI> get_vdi(Session session, string _pool_update)
        {
            return XenRef<VDI>.Create(session.proxy.pool_update_get_vdi(session.uuid, (_pool_update != null) ? _pool_update : "").parse());
        }

        /// <summary>
        /// Get the hosts field of the given pool_update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static List<XenRef<Host>> get_hosts(Session session, string _pool_update)
        {
            return XenRef<Host>.Create(session.proxy.pool_update_get_hosts(session.uuid, (_pool_update != null) ? _pool_update : "").parse());
        }

        /// <summary>
        /// Introduce update VDI
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The VDI which contains a software update.</param>
        public static XenRef<Pool_update> introduce(Session session, string _vdi)
        {
            return XenRef<Pool_update>.Create(session.proxy.pool_update_introduce(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Introduce update VDI
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The VDI which contains a software update.</param>
        public static XenRef<Task> async_introduce(Session session, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_update_introduce(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Execute the precheck stage of the selected update on a host
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        /// <param name="_host">The host to run the prechecks on.</param>
        public static livepatch_status precheck(Session session, string _pool_update, string _host)
        {
            return (livepatch_status)Helper.EnumParseDefault(typeof(livepatch_status), (string)session.proxy.pool_update_precheck(session.uuid, (_pool_update != null) ? _pool_update : "", (_host != null) ? _host : "").parse());
        }

        /// <summary>
        /// Execute the precheck stage of the selected update on a host
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        /// <param name="_host">The host to run the prechecks on.</param>
        public static XenRef<Task> async_precheck(Session session, string _pool_update, string _host)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_update_precheck(session.uuid, (_pool_update != null) ? _pool_update : "", (_host != null) ? _host : "").parse());
        }

        /// <summary>
        /// Apply the selected update to a host
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        /// <param name="_host">The host to apply the update to.</param>
        public static void apply(Session session, string _pool_update, string _host)
        {
            session.proxy.pool_update_apply(session.uuid, (_pool_update != null) ? _pool_update : "", (_host != null) ? _host : "").parse();
        }

        /// <summary>
        /// Apply the selected update to a host
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        /// <param name="_host">The host to apply the update to.</param>
        public static XenRef<Task> async_apply(Session session, string _pool_update, string _host)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_update_apply(session.uuid, (_pool_update != null) ? _pool_update : "", (_host != null) ? _host : "").parse());
        }

        /// <summary>
        /// Apply the selected update to all hosts in the pool
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static void pool_apply(Session session, string _pool_update)
        {
            session.proxy.pool_update_pool_apply(session.uuid, (_pool_update != null) ? _pool_update : "").parse();
        }

        /// <summary>
        /// Apply the selected update to all hosts in the pool
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static XenRef<Task> async_pool_apply(Session session, string _pool_update)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_update_pool_apply(session.uuid, (_pool_update != null) ? _pool_update : "").parse());
        }

        /// <summary>
        /// Removes the update's files from all hosts in the pool, but does not revert the update
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static void pool_clean(Session session, string _pool_update)
        {
            session.proxy.pool_update_pool_clean(session.uuid, (_pool_update != null) ? _pool_update : "").parse();
        }

        /// <summary>
        /// Removes the update's files from all hosts in the pool, but does not revert the update
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static XenRef<Task> async_pool_clean(Session session, string _pool_update)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_update_pool_clean(session.uuid, (_pool_update != null) ? _pool_update : "").parse());
        }

        /// <summary>
        /// Removes the database entry. Only works on unapplied update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static void destroy(Session session, string _pool_update)
        {
            session.proxy.pool_update_destroy(session.uuid, (_pool_update != null) ? _pool_update : "").parse();
        }

        /// <summary>
        /// Removes the database entry. Only works on unapplied update.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool_update">The opaque_ref of the given pool_update</param>
        public static XenRef<Task> async_destroy(Session session, string _pool_update)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_update_destroy(session.uuid, (_pool_update != null) ? _pool_update : "").parse());
        }

        /// <summary>
        /// Return a list of all the pool_updates known to the system.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Pool_update>> get_all(Session session)
        {
            return XenRef<Pool_update>.Create(session.proxy.pool_update_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the pool_update Records at once, in a single XML RPC call
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Pool_update>, Pool_update> get_all_records(Session session)
        {
            return XenRef<Pool_update>.Create<Proxy_Pool_update>(session.proxy.pool_update_get_all_records(session.uuid).parse());
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
        /// Update version number
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
        /// Size of the update in bytes
        /// </summary>
        public virtual long installation_size
        {
            get { return _installation_size; }
            set
            {
                if (!Helper.AreEqual(value, _installation_size))
                {
                    _installation_size = value;
                    Changed = true;
                    NotifyPropertyChanged("installation_size");
                }
            }
        }
        private long _installation_size;

        /// <summary>
        /// GPG key of the update
        /// </summary>
        public virtual string key
        {
            get { return _key; }
            set
            {
                if (!Helper.AreEqual(value, _key))
                {
                    _key = value;
                    Changed = true;
                    NotifyPropertyChanged("key");
                }
            }
        }
        private string _key;

        /// <summary>
        /// What the client should do after this update has been applied.
        /// </summary>
        public virtual List<update_after_apply_guidance> after_apply_guidance
        {
            get { return _after_apply_guidance; }
            set
            {
                if (!Helper.AreEqual(value, _after_apply_guidance))
                {
                    _after_apply_guidance = value;
                    Changed = true;
                    NotifyPropertyChanged("after_apply_guidance");
                }
            }
        }
        private List<update_after_apply_guidance> _after_apply_guidance;

        /// <summary>
        /// VDI the update was uploaded to
        /// </summary>
        public virtual XenRef<VDI> vdi
        {
            get { return _vdi; }
            set
            {
                if (!Helper.AreEqual(value, _vdi))
                {
                    _vdi = value;
                    Changed = true;
                    NotifyPropertyChanged("vdi");
                }
            }
        }
        private XenRef<VDI> _vdi;

        /// <summary>
        /// The hosts that have applied this update.
        /// </summary>
        public virtual List<XenRef<Host>> hosts
        {
            get { return _hosts; }
            set
            {
                if (!Helper.AreEqual(value, _hosts))
                {
                    _hosts = value;
                    Changed = true;
                    NotifyPropertyChanged("hosts");
                }
            }
        }
        private List<XenRef<Host>> _hosts;
    }
}
