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
    public partial class VDI : XenObject<VDI>
    {
        public VDI()
        {
        }

        public VDI(string uuid,
            string name_label,
            string name_description,
            List<vdi_operations> allowed_operations,
            Dictionary<string, vdi_operations> current_operations,
            XenRef<SR> SR,
            List<XenRef<VBD>> VBDs,
            List<XenRef<Crashdump>> crash_dumps,
            long virtual_size,
            long physical_utilisation,
            vdi_type type,
            bool sharable,
            bool read_only,
            Dictionary<string, string> other_config,
            bool storage_lock,
            string location,
            bool managed,
            bool missing,
            XenRef<VDI> parent,
            Dictionary<string, string> xenstore_data,
            Dictionary<string, string> sm_config,
            bool is_a_snapshot,
            XenRef<VDI> snapshot_of,
            List<XenRef<VDI>> snapshots,
            DateTime snapshot_time,
            string[] tags,
            bool allow_caching,
            on_boot on_boot,
            XenRef<Pool> metadata_of_pool,
            bool metadata_latest)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.SR = SR;
            this.VBDs = VBDs;
            this.crash_dumps = crash_dumps;
            this.virtual_size = virtual_size;
            this.physical_utilisation = physical_utilisation;
            this.type = type;
            this.sharable = sharable;
            this.read_only = read_only;
            this.other_config = other_config;
            this.storage_lock = storage_lock;
            this.location = location;
            this.managed = managed;
            this.missing = missing;
            this.parent = parent;
            this.xenstore_data = xenstore_data;
            this.sm_config = sm_config;
            this.is_a_snapshot = is_a_snapshot;
            this.snapshot_of = snapshot_of;
            this.snapshots = snapshots;
            this.snapshot_time = snapshot_time;
            this.tags = tags;
            this.allow_caching = allow_caching;
            this.on_boot = on_boot;
            this.metadata_of_pool = metadata_of_pool;
            this.metadata_latest = metadata_latest;
        }

        /// <summary>
        /// Creates a new VDI from a Proxy_VDI.
        /// </summary>
        /// <param name="proxy"></param>
        public VDI(Proxy_VDI proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(VDI update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            allowed_operations = update.allowed_operations;
            current_operations = update.current_operations;
            SR = update.SR;
            VBDs = update.VBDs;
            crash_dumps = update.crash_dumps;
            virtual_size = update.virtual_size;
            physical_utilisation = update.physical_utilisation;
            type = update.type;
            sharable = update.sharable;
            read_only = update.read_only;
            other_config = update.other_config;
            storage_lock = update.storage_lock;
            location = update.location;
            managed = update.managed;
            missing = update.missing;
            parent = update.parent;
            xenstore_data = update.xenstore_data;
            sm_config = update.sm_config;
            is_a_snapshot = update.is_a_snapshot;
            snapshot_of = update.snapshot_of;
            snapshots = update.snapshots;
            snapshot_time = update.snapshot_time;
            tags = update.tags;
            allow_caching = update.allow_caching;
            on_boot = update.on_boot;
            metadata_of_pool = update.metadata_of_pool;
            metadata_latest = update.metadata_latest;
        }

        internal void UpdateFromProxy(Proxy_VDI proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            allowed_operations = proxy.allowed_operations == null ? null : Helper.StringArrayToEnumList<vdi_operations>(proxy.allowed_operations);
            current_operations = proxy.current_operations == null ? null : Maps.convert_from_proxy_string_vdi_operations(proxy.current_operations);
            SR = proxy.SR == null ? null : XenRef<SR>.Create(proxy.SR);
            VBDs = proxy.VBDs == null ? null : XenRef<VBD>.Create(proxy.VBDs);
            crash_dumps = proxy.crash_dumps == null ? null : XenRef<Crashdump>.Create(proxy.crash_dumps);
            virtual_size = proxy.virtual_size == null ? 0 : long.Parse((string)proxy.virtual_size);
            physical_utilisation = proxy.physical_utilisation == null ? 0 : long.Parse((string)proxy.physical_utilisation);
            type = proxy.type == null ? (vdi_type) 0 : (vdi_type)Helper.EnumParseDefault(typeof(vdi_type), (string)proxy.type);
            sharable = (bool)proxy.sharable;
            read_only = (bool)proxy.read_only;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            storage_lock = (bool)proxy.storage_lock;
            location = proxy.location == null ? null : (string)proxy.location;
            managed = (bool)proxy.managed;
            missing = (bool)proxy.missing;
            parent = proxy.parent == null ? null : XenRef<VDI>.Create(proxy.parent);
            xenstore_data = proxy.xenstore_data == null ? null : Maps.convert_from_proxy_string_string(proxy.xenstore_data);
            sm_config = proxy.sm_config == null ? null : Maps.convert_from_proxy_string_string(proxy.sm_config);
            is_a_snapshot = (bool)proxy.is_a_snapshot;
            snapshot_of = proxy.snapshot_of == null ? null : XenRef<VDI>.Create(proxy.snapshot_of);
            snapshots = proxy.snapshots == null ? null : XenRef<VDI>.Create(proxy.snapshots);
            snapshot_time = proxy.snapshot_time;
            tags = proxy.tags == null ? new string[] {} : (string [])proxy.tags;
            allow_caching = (bool)proxy.allow_caching;
            on_boot = proxy.on_boot == null ? (on_boot) 0 : (on_boot)Helper.EnumParseDefault(typeof(on_boot), (string)proxy.on_boot);
            metadata_of_pool = proxy.metadata_of_pool == null ? null : XenRef<Pool>.Create(proxy.metadata_of_pool);
            metadata_latest = (bool)proxy.metadata_latest;
        }

        public Proxy_VDI ToProxy()
        {
            Proxy_VDI result_ = new Proxy_VDI();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.allowed_operations = (allowed_operations != null) ? Helper.ObjectListToStringArray(allowed_operations) : new string[] {};
            result_.current_operations = Maps.convert_to_proxy_string_vdi_operations(current_operations);
            result_.SR = (SR != null) ? SR : "";
            result_.VBDs = (VBDs != null) ? Helper.RefListToStringArray(VBDs) : new string[] {};
            result_.crash_dumps = (crash_dumps != null) ? Helper.RefListToStringArray(crash_dumps) : new string[] {};
            result_.virtual_size = virtual_size.ToString();
            result_.physical_utilisation = physical_utilisation.ToString();
            result_.type = vdi_type_helper.ToString(type);
            result_.sharable = sharable;
            result_.read_only = read_only;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.storage_lock = storage_lock;
            result_.location = (location != null) ? location : "";
            result_.managed = managed;
            result_.missing = missing;
            result_.parent = (parent != null) ? parent : "";
            result_.xenstore_data = Maps.convert_to_proxy_string_string(xenstore_data);
            result_.sm_config = Maps.convert_to_proxy_string_string(sm_config);
            result_.is_a_snapshot = is_a_snapshot;
            result_.snapshot_of = (snapshot_of != null) ? snapshot_of : "";
            result_.snapshots = (snapshots != null) ? Helper.RefListToStringArray(snapshots) : new string[] {};
            result_.snapshot_time = snapshot_time;
            result_.tags = tags;
            result_.allow_caching = allow_caching;
            result_.on_boot = on_boot_helper.ToString(on_boot);
            result_.metadata_of_pool = (metadata_of_pool != null) ? metadata_of_pool : "";
            result_.metadata_latest = metadata_latest;
            return result_;
        }

        /// <summary>
        /// Creates a new VDI from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public VDI(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            allowed_operations = Helper.StringArrayToEnumList<vdi_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            current_operations = Maps.convert_from_proxy_string_vdi_operations(Marshalling.ParseHashTable(table, "current_operations"));
            SR = Marshalling.ParseRef<SR>(table, "SR");
            VBDs = Marshalling.ParseSetRef<VBD>(table, "VBDs");
            crash_dumps = Marshalling.ParseSetRef<Crashdump>(table, "crash_dumps");
            virtual_size = Marshalling.ParseLong(table, "virtual_size");
            physical_utilisation = Marshalling.ParseLong(table, "physical_utilisation");
            type = (vdi_type)Helper.EnumParseDefault(typeof(vdi_type), Marshalling.ParseString(table, "type"));
            sharable = Marshalling.ParseBool(table, "sharable");
            read_only = Marshalling.ParseBool(table, "read_only");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            storage_lock = Marshalling.ParseBool(table, "storage_lock");
            location = Marshalling.ParseString(table, "location");
            managed = Marshalling.ParseBool(table, "managed");
            missing = Marshalling.ParseBool(table, "missing");
            parent = Marshalling.ParseRef<VDI>(table, "parent");
            xenstore_data = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "xenstore_data"));
            sm_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "sm_config"));
            is_a_snapshot = Marshalling.ParseBool(table, "is_a_snapshot");
            snapshot_of = Marshalling.ParseRef<VDI>(table, "snapshot_of");
            snapshots = Marshalling.ParseSetRef<VDI>(table, "snapshots");
            snapshot_time = Marshalling.ParseDateTime(table, "snapshot_time");
            tags = Marshalling.ParseStringArray(table, "tags");
            allow_caching = Marshalling.ParseBool(table, "allow_caching");
            on_boot = (on_boot)Helper.EnumParseDefault(typeof(on_boot), Marshalling.ParseString(table, "on_boot"));
            metadata_of_pool = Marshalling.ParseRef<Pool>(table, "metadata_of_pool");
            metadata_latest = Marshalling.ParseBool(table, "metadata_latest");
        }

        public bool DeepEquals(VDI other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(this.current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(this._SR, other._SR) &&
                Helper.AreEqual2(this._VBDs, other._VBDs) &&
                Helper.AreEqual2(this._crash_dumps, other._crash_dumps) &&
                Helper.AreEqual2(this._virtual_size, other._virtual_size) &&
                Helper.AreEqual2(this._physical_utilisation, other._physical_utilisation) &&
                Helper.AreEqual2(this._type, other._type) &&
                Helper.AreEqual2(this._sharable, other._sharable) &&
                Helper.AreEqual2(this._read_only, other._read_only) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._storage_lock, other._storage_lock) &&
                Helper.AreEqual2(this._location, other._location) &&
                Helper.AreEqual2(this._managed, other._managed) &&
                Helper.AreEqual2(this._missing, other._missing) &&
                Helper.AreEqual2(this._parent, other._parent) &&
                Helper.AreEqual2(this._xenstore_data, other._xenstore_data) &&
                Helper.AreEqual2(this._sm_config, other._sm_config) &&
                Helper.AreEqual2(this._is_a_snapshot, other._is_a_snapshot) &&
                Helper.AreEqual2(this._snapshot_of, other._snapshot_of) &&
                Helper.AreEqual2(this._snapshots, other._snapshots) &&
                Helper.AreEqual2(this._snapshot_time, other._snapshot_time) &&
                Helper.AreEqual2(this._tags, other._tags) &&
                Helper.AreEqual2(this._allow_caching, other._allow_caching) &&
                Helper.AreEqual2(this._on_boot, other._on_boot) &&
                Helper.AreEqual2(this._metadata_of_pool, other._metadata_of_pool) &&
                Helper.AreEqual2(this._metadata_latest, other._metadata_latest);
        }

        public override string SaveChanges(Session session, string opaqueRef, VDI server)
        {
            if (opaqueRef == null)
            {
                Proxy_VDI p = this.ToProxy();
                return session.proxy.vdi_create(session.uuid, p).parse();
            }
            else
            {
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    VDI.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_xenstore_data, server._xenstore_data))
                {
                    VDI.set_xenstore_data(session, opaqueRef, _xenstore_data);
                }
                if (!Helper.AreEqual2(_sm_config, server._sm_config))
                {
                    VDI.set_sm_config(session, opaqueRef, _sm_config);
                }
                if (!Helper.AreEqual2(_tags, server._tags))
                {
                    VDI.set_tags(session, opaqueRef, _tags);
                }
                if (!Helper.AreEqual2(_name_label, server._name_label))
                {
                    VDI.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    VDI.set_name_description(session, opaqueRef, _name_description);
                }
                if (!Helper.AreEqual2(_virtual_size, server._virtual_size))
                {
                    VDI.set_virtual_size(session, opaqueRef, _virtual_size);
                }
                if (!Helper.AreEqual2(_sharable, server._sharable))
                {
                    VDI.set_sharable(session, opaqueRef, _sharable);
                }
                if (!Helper.AreEqual2(_read_only, server._read_only))
                {
                    VDI.set_read_only(session, opaqueRef, _read_only);
                }

                return null;
            }
        }

        public static VDI get_record(Session session, string _vdi)
        {
            return new VDI((Proxy_VDI)session.proxy.vdi_get_record(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static XenRef<VDI> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static XenRef<VDI> create(Session session, VDI _record)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_create(session.uuid, _record.ToProxy()).parse());
        }

        public static XenRef<Task> async_create(Session session, VDI _record)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_create(session.uuid, _record.ToProxy()).parse());
        }

        public static void destroy(Session session, string _vdi)
        {
            session.proxy.vdi_destroy(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_destroy(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static List<XenRef<VDI>> get_by_name_label(Session session, string _label)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        public static string get_uuid(Session session, string _vdi)
        {
            return (string)session.proxy.vdi_get_uuid(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static string get_name_label(Session session, string _vdi)
        {
            return (string)session.proxy.vdi_get_name_label(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static string get_name_description(Session session, string _vdi)
        {
            return (string)session.proxy.vdi_get_name_description(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static List<vdi_operations> get_allowed_operations(Session session, string _vdi)
        {
            return Helper.StringArrayToEnumList<vdi_operations>(session.proxy.vdi_get_allowed_operations(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static Dictionary<string, vdi_operations> get_current_operations(Session session, string _vdi)
        {
            return Maps.convert_from_proxy_string_vdi_operations(session.proxy.vdi_get_current_operations(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static XenRef<SR> get_SR(Session session, string _vdi)
        {
            return XenRef<SR>.Create(session.proxy.vdi_get_sr(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static List<XenRef<VBD>> get_VBDs(Session session, string _vdi)
        {
            return XenRef<VBD>.Create(session.proxy.vdi_get_vbds(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static List<XenRef<Crashdump>> get_crash_dumps(Session session, string _vdi)
        {
            return XenRef<Crashdump>.Create(session.proxy.vdi_get_crash_dumps(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static long get_virtual_size(Session session, string _vdi)
        {
            return long.Parse((string)session.proxy.vdi_get_virtual_size(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static long get_physical_utilisation(Session session, string _vdi)
        {
            return long.Parse((string)session.proxy.vdi_get_physical_utilisation(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static vdi_type get_type(Session session, string _vdi)
        {
            return (vdi_type)Helper.EnumParseDefault(typeof(vdi_type), (string)session.proxy.vdi_get_type(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static bool get_sharable(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_sharable(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static bool get_read_only(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_read_only(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static Dictionary<string, string> get_other_config(Session session, string _vdi)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vdi_get_other_config(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static bool get_storage_lock(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_storage_lock(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static string get_location(Session session, string _vdi)
        {
            return (string)session.proxy.vdi_get_location(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static bool get_managed(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_managed(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static bool get_missing(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_missing(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static XenRef<VDI> get_parent(Session session, string _vdi)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_parent(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static Dictionary<string, string> get_xenstore_data(Session session, string _vdi)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vdi_get_xenstore_data(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static Dictionary<string, string> get_sm_config(Session session, string _vdi)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vdi_get_sm_config(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static bool get_is_a_snapshot(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_is_a_snapshot(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static XenRef<VDI> get_snapshot_of(Session session, string _vdi)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_snapshot_of(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static List<XenRef<VDI>> get_snapshots(Session session, string _vdi)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_snapshots(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static DateTime get_snapshot_time(Session session, string _vdi)
        {
            return session.proxy.vdi_get_snapshot_time(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static string[] get_tags(Session session, string _vdi)
        {
            return (string [])session.proxy.vdi_get_tags(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static bool get_allow_caching(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_allow_caching(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static on_boot get_on_boot(Session session, string _vdi)
        {
            return (on_boot)Helper.EnumParseDefault(typeof(on_boot), (string)session.proxy.vdi_get_on_boot(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static XenRef<Pool> get_metadata_of_pool(Session session, string _vdi)
        {
            return XenRef<Pool>.Create(session.proxy.vdi_get_metadata_of_pool(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static bool get_metadata_latest(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_metadata_latest(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static void set_other_config(Session session, string _vdi, Dictionary<string, string> _other_config)
        {
            session.proxy.vdi_set_other_config(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _vdi, string _key, string _value)
        {
            session.proxy.vdi_add_to_other_config(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _vdi, string _key)
        {
            session.proxy.vdi_remove_from_other_config(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "").parse();
        }

        public static void set_xenstore_data(Session session, string _vdi, Dictionary<string, string> _xenstore_data)
        {
            session.proxy.vdi_set_xenstore_data(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_xenstore_data)).parse();
        }

        public static void add_to_xenstore_data(Session session, string _vdi, string _key, string _value)
        {
            session.proxy.vdi_add_to_xenstore_data(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_xenstore_data(Session session, string _vdi, string _key)
        {
            session.proxy.vdi_remove_from_xenstore_data(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "").parse();
        }

        public static void set_sm_config(Session session, string _vdi, Dictionary<string, string> _sm_config)
        {
            session.proxy.vdi_set_sm_config(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_sm_config)).parse();
        }

        public static void add_to_sm_config(Session session, string _vdi, string _key, string _value)
        {
            session.proxy.vdi_add_to_sm_config(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_sm_config(Session session, string _vdi, string _key)
        {
            session.proxy.vdi_remove_from_sm_config(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "").parse();
        }

        public static void set_tags(Session session, string _vdi, string[] _tags)
        {
            session.proxy.vdi_set_tags(session.uuid, (_vdi != null) ? _vdi : "", _tags).parse();
        }

        public static void add_tags(Session session, string _vdi, string _value)
        {
            session.proxy.vdi_add_tags(session.uuid, (_vdi != null) ? _vdi : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_tags(Session session, string _vdi, string _value)
        {
            session.proxy.vdi_remove_tags(session.uuid, (_vdi != null) ? _vdi : "", (_value != null) ? _value : "").parse();
        }

        public static XenRef<VDI> snapshot(Session session, string _vdi, Dictionary<string, string> _driver_params)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_snapshot(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_driver_params)).parse());
        }

        public static XenRef<Task> async_snapshot(Session session, string _vdi, Dictionary<string, string> _driver_params)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_snapshot(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_driver_params)).parse());
        }

        public static XenRef<VDI> clone(Session session, string _vdi, Dictionary<string, string> _driver_params)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_clone(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_driver_params)).parse());
        }

        public static XenRef<Task> async_clone(Session session, string _vdi, Dictionary<string, string> _driver_params)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_clone(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_driver_params)).parse());
        }

        public static void resize(Session session, string _vdi, long _size)
        {
            session.proxy.vdi_resize(session.uuid, (_vdi != null) ? _vdi : "", _size.ToString()).parse();
        }

        public static XenRef<Task> async_resize(Session session, string _vdi, long _size)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_resize(session.uuid, (_vdi != null) ? _vdi : "", _size.ToString()).parse());
        }

        public static void resize_online(Session session, string _vdi, long _size)
        {
            session.proxy.vdi_resize_online(session.uuid, (_vdi != null) ? _vdi : "", _size.ToString()).parse();
        }

        public static XenRef<Task> async_resize_online(Session session, string _vdi, long _size)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_resize_online(session.uuid, (_vdi != null) ? _vdi : "", _size.ToString()).parse());
        }

        public static XenRef<VDI> introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config), _managed, _virtual_size.ToString(), _physical_utilisation.ToString(), (_metadata_of_pool != null) ? _metadata_of_pool : "", _is_a_snapshot, _snapshot_time, (_snapshot_of != null) ? _snapshot_of : "").parse());
        }

        public static XenRef<Task> async_introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config), _managed, _virtual_size.ToString(), _physical_utilisation.ToString(), (_metadata_of_pool != null) ? _metadata_of_pool : "", _is_a_snapshot, _snapshot_time, (_snapshot_of != null) ? _snapshot_of : "").parse());
        }

        public static XenRef<VDI> db_introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_db_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config), _managed, _virtual_size.ToString(), _physical_utilisation.ToString(), (_metadata_of_pool != null) ? _metadata_of_pool : "", _is_a_snapshot, _snapshot_time, (_snapshot_of != null) ? _snapshot_of : "").parse());
        }

        public static XenRef<Task> async_db_introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_db_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config), _managed, _virtual_size.ToString(), _physical_utilisation.ToString(), (_metadata_of_pool != null) ? _metadata_of_pool : "", _is_a_snapshot, _snapshot_time, (_snapshot_of != null) ? _snapshot_of : "").parse());
        }

        public static void db_forget(Session session, string _vdi)
        {
            session.proxy.vdi_db_forget(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static XenRef<Task> async_db_forget(Session session, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_db_forget(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static void update(Session session, string _vdi)
        {
            session.proxy.vdi_update(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static XenRef<Task> async_update(Session session, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_update(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static XenRef<VDI> copy(Session session, string _vdi, string _sr)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_copy(session.uuid, (_vdi != null) ? _vdi : "", (_sr != null) ? _sr : "").parse());
        }

        public static XenRef<Task> async_copy(Session session, string _vdi, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_copy(session.uuid, (_vdi != null) ? _vdi : "", (_sr != null) ? _sr : "").parse());
        }

        public static void set_managed(Session session, string _self, bool _value)
        {
            session.proxy.vdi_set_managed(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static void forget(Session session, string _vdi)
        {
            session.proxy.vdi_forget(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        public static XenRef<Task> async_forget(Session session, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_forget(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        public static void set_sharable(Session session, string _self, bool _value)
        {
            session.proxy.vdi_set_sharable(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static void set_read_only(Session session, string _self, bool _value)
        {
            session.proxy.vdi_set_read_only(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static void set_missing(Session session, string _self, bool _value)
        {
            session.proxy.vdi_set_missing(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static void set_virtual_size(Session session, string _self, long _value)
        {
            session.proxy.vdi_set_virtual_size(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void set_physical_utilisation(Session session, string _self, long _value)
        {
            session.proxy.vdi_set_physical_utilisation(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void set_is_a_snapshot(Session session, string _self, bool _value)
        {
            session.proxy.vdi_set_is_a_snapshot(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static void set_snapshot_of(Session session, string _self, string _value)
        {
            session.proxy.vdi_set_snapshot_of(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse();
        }

        public static void set_snapshot_time(Session session, string _self, DateTime _value)
        {
            session.proxy.vdi_set_snapshot_time(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static void set_metadata_of_pool(Session session, string _self, string _value)
        {
            session.proxy.vdi_set_metadata_of_pool(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse();
        }

        public static void set_name_label(Session session, string _self, string _value)
        {
            session.proxy.vdi_set_name_label(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse();
        }

        public static XenRef<Task> async_set_name_label(Session session, string _self, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_set_name_label(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse());
        }

        public static void set_name_description(Session session, string _self, string _value)
        {
            session.proxy.vdi_set_name_description(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse();
        }

        public static XenRef<Task> async_set_name_description(Session session, string _self, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_set_name_description(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse());
        }

        public static void set_on_boot(Session session, string _self, on_boot _value)
        {
            session.proxy.vdi_set_on_boot(session.uuid, (_self != null) ? _self : "", on_boot_helper.ToString(_value)).parse();
        }

        public static XenRef<Task> async_set_on_boot(Session session, string _self, on_boot _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_set_on_boot(session.uuid, (_self != null) ? _self : "", on_boot_helper.ToString(_value)).parse());
        }

        public static void set_allow_caching(Session session, string _self, bool _value)
        {
            session.proxy.vdi_set_allow_caching(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static XenRef<Task> async_set_allow_caching(Session session, string _self, bool _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_set_allow_caching(session.uuid, (_self != null) ? _self : "", _value).parse());
        }

        public static XenRef<Session> open_database(Session session, string _self)
        {
            return XenRef<Session>.Create(session.proxy.vdi_open_database(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static XenRef<Task> async_open_database(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_open_database(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static string read_database_pool_uuid(Session session, string _self)
        {
            return (string)session.proxy.vdi_read_database_pool_uuid(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_read_database_pool_uuid(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_read_database_pool_uuid(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static XenRef<VDI> pool_migrate(Session session, string _vdi, string _sr, Dictionary<string, string> _options)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_pool_migrate(session.uuid, (_vdi != null) ? _vdi : "", (_sr != null) ? _sr : "", Maps.convert_to_proxy_string_string(_options)).parse());
        }

        public static XenRef<Task> async_pool_migrate(Session session, string _vdi, string _sr, Dictionary<string, string> _options)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_pool_migrate(session.uuid, (_vdi != null) ? _vdi : "", (_sr != null) ? _sr : "", Maps.convert_to_proxy_string_string(_options)).parse());
        }

        public static List<XenRef<VDI>> get_all(Session session)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<VDI>, VDI> get_all_records(Session session)
        {
            return XenRef<VDI>.Create<Proxy_VDI>(session.proxy.vdi_get_all_records(session.uuid).parse());
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

        private List<vdi_operations> _allowed_operations;
        public virtual List<vdi_operations> allowed_operations {
             get { return _allowed_operations; }
             set { if (!Helper.AreEqual(value, _allowed_operations)) { _allowed_operations = value; Changed = true; NotifyPropertyChanged("allowed_operations"); } }
         }

        private Dictionary<string, vdi_operations> _current_operations;
        public virtual Dictionary<string, vdi_operations> current_operations {
             get { return _current_operations; }
             set { if (!Helper.AreEqual(value, _current_operations)) { _current_operations = value; Changed = true; NotifyPropertyChanged("current_operations"); } }
         }

        private XenRef<SR> _SR;
        public virtual XenRef<SR> SR {
             get { return _SR; }
             set { if (!Helper.AreEqual(value, _SR)) { _SR = value; Changed = true; NotifyPropertyChanged("SR"); } }
         }

        private List<XenRef<VBD>> _VBDs;
        public virtual List<XenRef<VBD>> VBDs {
             get { return _VBDs; }
             set { if (!Helper.AreEqual(value, _VBDs)) { _VBDs = value; Changed = true; NotifyPropertyChanged("VBDs"); } }
         }

        private List<XenRef<Crashdump>> _crash_dumps;
        public virtual List<XenRef<Crashdump>> crash_dumps {
             get { return _crash_dumps; }
             set { if (!Helper.AreEqual(value, _crash_dumps)) { _crash_dumps = value; Changed = true; NotifyPropertyChanged("crash_dumps"); } }
         }

        private long _virtual_size;
        public virtual long virtual_size {
             get { return _virtual_size; }
             set { if (!Helper.AreEqual(value, _virtual_size)) { _virtual_size = value; Changed = true; NotifyPropertyChanged("virtual_size"); } }
         }

        private long _physical_utilisation;
        public virtual long physical_utilisation {
             get { return _physical_utilisation; }
             set { if (!Helper.AreEqual(value, _physical_utilisation)) { _physical_utilisation = value; Changed = true; NotifyPropertyChanged("physical_utilisation"); } }
         }

        private vdi_type _type;
        public virtual vdi_type type {
             get { return _type; }
             set { if (!Helper.AreEqual(value, _type)) { _type = value; Changed = true; NotifyPropertyChanged("type"); } }
         }

        private bool _sharable;
        public virtual bool sharable {
             get { return _sharable; }
             set { if (!Helper.AreEqual(value, _sharable)) { _sharable = value; Changed = true; NotifyPropertyChanged("sharable"); } }
         }

        private bool _read_only;
        public virtual bool read_only {
             get { return _read_only; }
             set { if (!Helper.AreEqual(value, _read_only)) { _read_only = value; Changed = true; NotifyPropertyChanged("read_only"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }

        private bool _storage_lock;
        public virtual bool storage_lock {
             get { return _storage_lock; }
             set { if (!Helper.AreEqual(value, _storage_lock)) { _storage_lock = value; Changed = true; NotifyPropertyChanged("storage_lock"); } }
         }

        private string _location;
        public virtual string location {
             get { return _location; }
             set { if (!Helper.AreEqual(value, _location)) { _location = value; Changed = true; NotifyPropertyChanged("location"); } }
         }

        private bool _managed;
        public virtual bool managed {
             get { return _managed; }
             set { if (!Helper.AreEqual(value, _managed)) { _managed = value; Changed = true; NotifyPropertyChanged("managed"); } }
         }

        private bool _missing;
        public virtual bool missing {
             get { return _missing; }
             set { if (!Helper.AreEqual(value, _missing)) { _missing = value; Changed = true; NotifyPropertyChanged("missing"); } }
         }

        private XenRef<VDI> _parent;
        public virtual XenRef<VDI> parent {
             get { return _parent; }
             set { if (!Helper.AreEqual(value, _parent)) { _parent = value; Changed = true; NotifyPropertyChanged("parent"); } }
         }

        private Dictionary<string, string> _xenstore_data;
        public virtual Dictionary<string, string> xenstore_data {
             get { return _xenstore_data; }
             set { if (!Helper.AreEqual(value, _xenstore_data)) { _xenstore_data = value; Changed = true; NotifyPropertyChanged("xenstore_data"); } }
         }

        private Dictionary<string, string> _sm_config;
        public virtual Dictionary<string, string> sm_config {
             get { return _sm_config; }
             set { if (!Helper.AreEqual(value, _sm_config)) { _sm_config = value; Changed = true; NotifyPropertyChanged("sm_config"); } }
         }

        private bool _is_a_snapshot;
        public virtual bool is_a_snapshot {
             get { return _is_a_snapshot; }
             set { if (!Helper.AreEqual(value, _is_a_snapshot)) { _is_a_snapshot = value; Changed = true; NotifyPropertyChanged("is_a_snapshot"); } }
         }

        private XenRef<VDI> _snapshot_of;
        public virtual XenRef<VDI> snapshot_of {
             get { return _snapshot_of; }
             set { if (!Helper.AreEqual(value, _snapshot_of)) { _snapshot_of = value; Changed = true; NotifyPropertyChanged("snapshot_of"); } }
         }

        private List<XenRef<VDI>> _snapshots;
        public virtual List<XenRef<VDI>> snapshots {
             get { return _snapshots; }
             set { if (!Helper.AreEqual(value, _snapshots)) { _snapshots = value; Changed = true; NotifyPropertyChanged("snapshots"); } }
         }

        private DateTime _snapshot_time;
        public virtual DateTime snapshot_time {
             get { return _snapshot_time; }
             set { if (!Helper.AreEqual(value, _snapshot_time)) { _snapshot_time = value; Changed = true; NotifyPropertyChanged("snapshot_time"); } }
         }

        private string[] _tags;
        public virtual string[] tags {
             get { return _tags; }
             set { if (!Helper.AreEqual(value, _tags)) { _tags = value; Changed = true; NotifyPropertyChanged("tags"); } }
         }

        private bool _allow_caching;
        public virtual bool allow_caching {
             get { return _allow_caching; }
             set { if (!Helper.AreEqual(value, _allow_caching)) { _allow_caching = value; Changed = true; NotifyPropertyChanged("allow_caching"); } }
         }

        private on_boot _on_boot;
        public virtual on_boot on_boot {
             get { return _on_boot; }
             set { if (!Helper.AreEqual(value, _on_boot)) { _on_boot = value; Changed = true; NotifyPropertyChanged("on_boot"); } }
         }

        private XenRef<Pool> _metadata_of_pool;
        public virtual XenRef<Pool> metadata_of_pool {
             get { return _metadata_of_pool; }
             set { if (!Helper.AreEqual(value, _metadata_of_pool)) { _metadata_of_pool = value; Changed = true; NotifyPropertyChanged("metadata_of_pool"); } }
         }

        private bool _metadata_latest;
        public virtual bool metadata_latest {
             get { return _metadata_latest; }
             set { if (!Helper.AreEqual(value, _metadata_latest)) { _metadata_latest = value; Changed = true; NotifyPropertyChanged("metadata_latest"); } }
         }


    }
}
