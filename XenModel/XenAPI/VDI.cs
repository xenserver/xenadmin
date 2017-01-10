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
    /// A virtual disk image
    /// First published in XenServer 4.0.
    /// </summary>
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
            bool metadata_latest,
            bool is_tools_iso)
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
            this.is_tools_iso = is_tools_iso;
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
            is_tools_iso = update.is_tools_iso;
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
            is_tools_iso = (bool)proxy.is_tools_iso;
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
            result_.is_tools_iso = is_tools_iso;
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
            is_tools_iso = Marshalling.ParseBool(table, "is_tools_iso");
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
                Helper.AreEqual2(this._metadata_latest, other._metadata_latest) &&
                Helper.AreEqual2(this._is_tools_iso, other._is_tools_iso);
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
        /// <summary>
        /// Get a record containing the current state of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static VDI get_record(Session session, string _vdi)
        {
            return new VDI((Proxy_VDI)session.proxy.vdi_get_record(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get a reference to the VDI instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VDI> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Create a new VDI instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<VDI> create(Session session, VDI _record)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new VDI instance, and return its handle.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, VDI _record)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified VDI instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static void destroy(Session session, string _vdi)
        {
            session.proxy.vdi_destroy(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Destroy the specified VDI instance.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static XenRef<Task> async_destroy(Session session, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_destroy(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get all the VDI instances with the given label.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        public static List<XenRef<VDI>> get_by_name_label(Session session, string _label)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static string get_uuid(Session session, string _vdi)
        {
            return (string)session.proxy.vdi_get_uuid(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the name/label field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static string get_name_label(Session session, string _vdi)
        {
            return (string)session.proxy.vdi_get_name_label(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the name/description field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static string get_name_description(Session session, string _vdi)
        {
            return (string)session.proxy.vdi_get_name_description(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the allowed_operations field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static List<vdi_operations> get_allowed_operations(Session session, string _vdi)
        {
            return Helper.StringArrayToEnumList<vdi_operations>(session.proxy.vdi_get_allowed_operations(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the current_operations field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static Dictionary<string, vdi_operations> get_current_operations(Session session, string _vdi)
        {
            return Maps.convert_from_proxy_string_vdi_operations(session.proxy.vdi_get_current_operations(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the SR field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static XenRef<SR> get_SR(Session session, string _vdi)
        {
            return XenRef<SR>.Create(session.proxy.vdi_get_sr(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the VBDs field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static List<XenRef<VBD>> get_VBDs(Session session, string _vdi)
        {
            return XenRef<VBD>.Create(session.proxy.vdi_get_vbds(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the crash_dumps field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static List<XenRef<Crashdump>> get_crash_dumps(Session session, string _vdi)
        {
            return XenRef<Crashdump>.Create(session.proxy.vdi_get_crash_dumps(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the virtual_size field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static long get_virtual_size(Session session, string _vdi)
        {
            return long.Parse((string)session.proxy.vdi_get_virtual_size(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the physical_utilisation field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static long get_physical_utilisation(Session session, string _vdi)
        {
            return long.Parse((string)session.proxy.vdi_get_physical_utilisation(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the type field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static vdi_type get_type(Session session, string _vdi)
        {
            return (vdi_type)Helper.EnumParseDefault(typeof(vdi_type), (string)session.proxy.vdi_get_type(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the sharable field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static bool get_sharable(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_sharable(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the read_only field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static bool get_read_only(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_read_only(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the other_config field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static Dictionary<string, string> get_other_config(Session session, string _vdi)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vdi_get_other_config(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the storage_lock field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static bool get_storage_lock(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_storage_lock(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the location field of the given VDI.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static string get_location(Session session, string _vdi)
        {
            return (string)session.proxy.vdi_get_location(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the managed field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static bool get_managed(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_managed(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the missing field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static bool get_missing(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_missing(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the parent field of the given VDI.
        /// First published in XenServer 4.0.
        /// Deprecated since .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        [Deprecated("")]
        public static XenRef<VDI> get_parent(Session session, string _vdi)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_parent(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the xenstore_data field of the given VDI.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static Dictionary<string, string> get_xenstore_data(Session session, string _vdi)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vdi_get_xenstore_data(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the sm_config field of the given VDI.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static Dictionary<string, string> get_sm_config(Session session, string _vdi)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vdi_get_sm_config(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the is_a_snapshot field of the given VDI.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static bool get_is_a_snapshot(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_is_a_snapshot(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the snapshot_of field of the given VDI.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static XenRef<VDI> get_snapshot_of(Session session, string _vdi)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_snapshot_of(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the snapshots field of the given VDI.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static List<XenRef<VDI>> get_snapshots(Session session, string _vdi)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_snapshots(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the snapshot_time field of the given VDI.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static DateTime get_snapshot_time(Session session, string _vdi)
        {
            return session.proxy.vdi_get_snapshot_time(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the tags field of the given VDI.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static string[] get_tags(Session session, string _vdi)
        {
            return (string [])session.proxy.vdi_get_tags(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the allow_caching field of the given VDI.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static bool get_allow_caching(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_allow_caching(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the on_boot field of the given VDI.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static on_boot get_on_boot(Session session, string _vdi)
        {
            return (on_boot)Helper.EnumParseDefault(typeof(on_boot), (string)session.proxy.vdi_get_on_boot(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the metadata_of_pool field of the given VDI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static XenRef<Pool> get_metadata_of_pool(Session session, string _vdi)
        {
            return XenRef<Pool>.Create(session.proxy.vdi_get_metadata_of_pool(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Get the metadata_latest field of the given VDI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static bool get_metadata_latest(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_metadata_latest(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Get the is_tools_iso field of the given VDI.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static bool get_is_tools_iso(Session session, string _vdi)
        {
            return (bool)session.proxy.vdi_get_is_tools_iso(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Set the other_config field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _vdi, Dictionary<string, string> _other_config)
        {
            session.proxy.vdi_set_other_config(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _vdi, string _key, string _value)
        {
            session.proxy.vdi_add_to_other_config(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given VDI.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _vdi, string _key)
        {
            session.proxy.vdi_remove_from_other_config(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Set the xenstore_data field of the given VDI.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_xenstore_data">New value to set</param>
        public static void set_xenstore_data(Session session, string _vdi, Dictionary<string, string> _xenstore_data)
        {
            session.proxy.vdi_set_xenstore_data(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_xenstore_data)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the xenstore_data field of the given VDI.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_xenstore_data(Session session, string _vdi, string _key, string _value)
        {
            session.proxy.vdi_add_to_xenstore_data(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the xenstore_data field of the given VDI.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_xenstore_data(Session session, string _vdi, string _key)
        {
            session.proxy.vdi_remove_from_xenstore_data(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Set the sm_config field of the given VDI.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_sm_config">New value to set</param>
        public static void set_sm_config(Session session, string _vdi, Dictionary<string, string> _sm_config)
        {
            session.proxy.vdi_set_sm_config(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_sm_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the sm_config field of the given VDI.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_sm_config(Session session, string _vdi, string _key, string _value)
        {
            session.proxy.vdi_add_to_sm_config(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the sm_config field of the given VDI.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_sm_config(Session session, string _vdi, string _key)
        {
            session.proxy.vdi_remove_from_sm_config(session.uuid, (_vdi != null) ? _vdi : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Set the tags field of the given VDI.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_tags">New value to set</param>
        public static void set_tags(Session session, string _vdi, string[] _tags)
        {
            session.proxy.vdi_set_tags(session.uuid, (_vdi != null) ? _vdi : "", _tags).parse();
        }

        /// <summary>
        /// Add the given value to the tags field of the given VDI.  If the value is already in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">New value to add</param>
        public static void add_tags(Session session, string _vdi, string _value)
        {
            session.proxy.vdi_add_tags(session.uuid, (_vdi != null) ? _vdi : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given value from the tags field of the given VDI.  If the value is not in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">Value to remove</param>
        public static void remove_tags(Session session, string _vdi, string _value)
        {
            session.proxy.vdi_remove_tags(session.uuid, (_vdi != null) ? _vdi : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Take a read-only snapshot of the VDI, returning a reference to the snapshot. If any driver_params are specified then these are passed through to the storage-specific substrate driver that takes the snapshot. NB the snapshot lives in the same Storage Repository as its parent.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_driver_params">Optional parameters that can be passed through to backend driver in order to specify storage-type-specific snapshot options First published in XenServer 4.1.</param>
        public static XenRef<VDI> snapshot(Session session, string _vdi, Dictionary<string, string> _driver_params)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_snapshot(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_driver_params)).parse());
        }

        /// <summary>
        /// Take a read-only snapshot of the VDI, returning a reference to the snapshot. If any driver_params are specified then these are passed through to the storage-specific substrate driver that takes the snapshot. NB the snapshot lives in the same Storage Repository as its parent.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_driver_params">Optional parameters that can be passed through to backend driver in order to specify storage-type-specific snapshot options First published in XenServer 4.1.</param>
        public static XenRef<Task> async_snapshot(Session session, string _vdi, Dictionary<string, string> _driver_params)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_snapshot(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_driver_params)).parse());
        }

        /// <summary>
        /// Take an exact copy of the VDI and return a reference to the new disk. If any driver_params are specified then these are passed through to the storage-specific substrate driver that implements the clone operation. NB the clone lives in the same Storage Repository as its parent.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_driver_params">Optional parameters that are passed through to the backend driver in order to specify storage-type-specific clone options First published in XenServer 4.1.</param>
        public static XenRef<VDI> clone(Session session, string _vdi, Dictionary<string, string> _driver_params)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_clone(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_driver_params)).parse());
        }

        /// <summary>
        /// Take an exact copy of the VDI and return a reference to the new disk. If any driver_params are specified then these are passed through to the storage-specific substrate driver that implements the clone operation. NB the clone lives in the same Storage Repository as its parent.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_driver_params">Optional parameters that are passed through to the backend driver in order to specify storage-type-specific clone options First published in XenServer 4.1.</param>
        public static XenRef<Task> async_clone(Session session, string _vdi, Dictionary<string, string> _driver_params)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_clone(session.uuid, (_vdi != null) ? _vdi : "", Maps.convert_to_proxy_string_string(_driver_params)).parse());
        }

        /// <summary>
        /// Resize the VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_size">The new size of the VDI</param>
        public static void resize(Session session, string _vdi, long _size)
        {
            session.proxy.vdi_resize(session.uuid, (_vdi != null) ? _vdi : "", _size.ToString()).parse();
        }

        /// <summary>
        /// Resize the VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_size">The new size of the VDI</param>
        public static XenRef<Task> async_resize(Session session, string _vdi, long _size)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_resize(session.uuid, (_vdi != null) ? _vdi : "", _size.ToString()).parse());
        }

        /// <summary>
        /// Resize the VDI which may or may not be attached to running guests.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_size">The new size of the VDI</param>
        public static void resize_online(Session session, string _vdi, long _size)
        {
            session.proxy.vdi_resize_online(session.uuid, (_vdi != null) ? _vdi : "", _size.ToString()).parse();
        }

        /// <summary>
        /// Resize the VDI which may or may not be attached to running guests.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_size">The new size of the VDI</param>
        public static XenRef<Task> async_resize_online(Session session, string _vdi, long _size)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_resize_online(session.uuid, (_vdi != null) ? _vdi : "", _size.ToString()).parse());
        }

        /// <summary>
        /// Create a new VDI record in the database only
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">The uuid of the disk to introduce</param>
        /// <param name="_name_label">The name of the disk record</param>
        /// <param name="_name_description">The description of the disk record</param>
        /// <param name="_sr">The SR that the VDI is in</param>
        /// <param name="_type">The type of the VDI</param>
        /// <param name="_sharable">true if this disk may be shared</param>
        /// <param name="_read_only">true if this disk may ONLY be mounted read-only</param>
        /// <param name="_other_config">additional configuration</param>
        /// <param name="_location">location information</param>
        /// <param name="_xenstore_data">Data to insert into xenstore</param>
        /// <param name="_sm_config">Storage-specific config</param>
        public static XenRef<VDI> introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config)).parse());
        }

        /// <summary>
        /// Create a new VDI record in the database only
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">The uuid of the disk to introduce</param>
        /// <param name="_name_label">The name of the disk record</param>
        /// <param name="_name_description">The description of the disk record</param>
        /// <param name="_sr">The SR that the VDI is in</param>
        /// <param name="_type">The type of the VDI</param>
        /// <param name="_sharable">true if this disk may be shared</param>
        /// <param name="_read_only">true if this disk may ONLY be mounted read-only</param>
        /// <param name="_other_config">additional configuration</param>
        /// <param name="_location">location information</param>
        /// <param name="_xenstore_data">Data to insert into xenstore</param>
        /// <param name="_sm_config">Storage-specific config</param>
        public static XenRef<Task> async_introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config)).parse());
        }

        /// <summary>
        /// Create a new VDI record in the database only
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">The uuid of the disk to introduce</param>
        /// <param name="_name_label">The name of the disk record</param>
        /// <param name="_name_description">The description of the disk record</param>
        /// <param name="_sr">The SR that the VDI is in</param>
        /// <param name="_type">The type of the VDI</param>
        /// <param name="_sharable">true if this disk may be shared</param>
        /// <param name="_read_only">true if this disk may ONLY be mounted read-only</param>
        /// <param name="_other_config">additional configuration</param>
        /// <param name="_location">location information</param>
        /// <param name="_xenstore_data">Data to insert into xenstore</param>
        /// <param name="_sm_config">Storage-specific config</param>
        /// <param name="_managed">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_virtual_size">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_physical_utilisation">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_metadata_of_pool">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_is_a_snapshot">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_snapshot_time">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_snapshot_of">Storage-specific config First published in XenServer 6.1.</param>
        public static XenRef<VDI> introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config), _managed, _virtual_size.ToString(), _physical_utilisation.ToString(), (_metadata_of_pool != null) ? _metadata_of_pool : "", _is_a_snapshot, _snapshot_time, (_snapshot_of != null) ? _snapshot_of : "").parse());
        }

        /// <summary>
        /// Create a new VDI record in the database only
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">The uuid of the disk to introduce</param>
        /// <param name="_name_label">The name of the disk record</param>
        /// <param name="_name_description">The description of the disk record</param>
        /// <param name="_sr">The SR that the VDI is in</param>
        /// <param name="_type">The type of the VDI</param>
        /// <param name="_sharable">true if this disk may be shared</param>
        /// <param name="_read_only">true if this disk may ONLY be mounted read-only</param>
        /// <param name="_other_config">additional configuration</param>
        /// <param name="_location">location information</param>
        /// <param name="_xenstore_data">Data to insert into xenstore</param>
        /// <param name="_sm_config">Storage-specific config</param>
        /// <param name="_managed">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_virtual_size">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_physical_utilisation">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_metadata_of_pool">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_is_a_snapshot">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_snapshot_time">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_snapshot_of">Storage-specific config First published in XenServer 6.1.</param>
        public static XenRef<Task> async_introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config), _managed, _virtual_size.ToString(), _physical_utilisation.ToString(), (_metadata_of_pool != null) ? _metadata_of_pool : "", _is_a_snapshot, _snapshot_time, (_snapshot_of != null) ? _snapshot_of : "").parse());
        }

        /// <summary>
        /// Create a new VDI record in the database only
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">The uuid of the disk to introduce First published in XenServer 4.1.</param>
        /// <param name="_name_label">The name of the disk record First published in XenServer 4.1.</param>
        /// <param name="_name_description">The description of the disk record First published in XenServer 4.1.</param>
        /// <param name="_sr">The SR that the VDI is in First published in XenServer 4.1.</param>
        /// <param name="_type">The type of the VDI First published in XenServer 4.1.</param>
        /// <param name="_sharable">true if this disk may be shared First published in XenServer 4.1.</param>
        /// <param name="_read_only">true if this disk may ONLY be mounted read-only First published in XenServer 4.1.</param>
        /// <param name="_other_config">additional configuration First published in XenServer 4.1.</param>
        /// <param name="_location">location information First published in XenServer 4.1.</param>
        /// <param name="_xenstore_data">Data to insert into xenstore First published in XenServer 4.1.</param>
        /// <param name="_sm_config">Storage-specific config First published in XenServer 4.1.</param>
        public static XenRef<VDI> db_introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_db_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config)).parse());
        }

        /// <summary>
        /// Create a new VDI record in the database only
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">The uuid of the disk to introduce First published in XenServer 4.1.</param>
        /// <param name="_name_label">The name of the disk record First published in XenServer 4.1.</param>
        /// <param name="_name_description">The description of the disk record First published in XenServer 4.1.</param>
        /// <param name="_sr">The SR that the VDI is in First published in XenServer 4.1.</param>
        /// <param name="_type">The type of the VDI First published in XenServer 4.1.</param>
        /// <param name="_sharable">true if this disk may be shared First published in XenServer 4.1.</param>
        /// <param name="_read_only">true if this disk may ONLY be mounted read-only First published in XenServer 4.1.</param>
        /// <param name="_other_config">additional configuration First published in XenServer 4.1.</param>
        /// <param name="_location">location information First published in XenServer 4.1.</param>
        /// <param name="_xenstore_data">Data to insert into xenstore First published in XenServer 4.1.</param>
        /// <param name="_sm_config">Storage-specific config First published in XenServer 4.1.</param>
        public static XenRef<Task> async_db_introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_db_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config)).parse());
        }

        /// <summary>
        /// Create a new VDI record in the database only
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">The uuid of the disk to introduce First published in XenServer 4.1.</param>
        /// <param name="_name_label">The name of the disk record First published in XenServer 4.1.</param>
        /// <param name="_name_description">The description of the disk record First published in XenServer 4.1.</param>
        /// <param name="_sr">The SR that the VDI is in First published in XenServer 4.1.</param>
        /// <param name="_type">The type of the VDI First published in XenServer 4.1.</param>
        /// <param name="_sharable">true if this disk may be shared First published in XenServer 4.1.</param>
        /// <param name="_read_only">true if this disk may ONLY be mounted read-only First published in XenServer 4.1.</param>
        /// <param name="_other_config">additional configuration First published in XenServer 4.1.</param>
        /// <param name="_location">location information First published in XenServer 4.1.</param>
        /// <param name="_xenstore_data">Data to insert into xenstore First published in XenServer 4.1.</param>
        /// <param name="_sm_config">Storage-specific config First published in XenServer 4.1.</param>
        /// <param name="_managed">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_virtual_size">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_physical_utilisation">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_metadata_of_pool">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_is_a_snapshot">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_snapshot_time">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_snapshot_of">Storage-specific config First published in XenServer 6.1.</param>
        public static XenRef<VDI> db_introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_db_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config), _managed, _virtual_size.ToString(), _physical_utilisation.ToString(), (_metadata_of_pool != null) ? _metadata_of_pool : "", _is_a_snapshot, _snapshot_time, (_snapshot_of != null) ? _snapshot_of : "").parse());
        }

        /// <summary>
        /// Create a new VDI record in the database only
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">The uuid of the disk to introduce First published in XenServer 4.1.</param>
        /// <param name="_name_label">The name of the disk record First published in XenServer 4.1.</param>
        /// <param name="_name_description">The description of the disk record First published in XenServer 4.1.</param>
        /// <param name="_sr">The SR that the VDI is in First published in XenServer 4.1.</param>
        /// <param name="_type">The type of the VDI First published in XenServer 4.1.</param>
        /// <param name="_sharable">true if this disk may be shared First published in XenServer 4.1.</param>
        /// <param name="_read_only">true if this disk may ONLY be mounted read-only First published in XenServer 4.1.</param>
        /// <param name="_other_config">additional configuration First published in XenServer 4.1.</param>
        /// <param name="_location">location information First published in XenServer 4.1.</param>
        /// <param name="_xenstore_data">Data to insert into xenstore First published in XenServer 4.1.</param>
        /// <param name="_sm_config">Storage-specific config First published in XenServer 4.1.</param>
        /// <param name="_managed">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_virtual_size">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_physical_utilisation">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_metadata_of_pool">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_is_a_snapshot">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_snapshot_time">Storage-specific config First published in XenServer 6.1.</param>
        /// <param name="_snapshot_of">Storage-specific config First published in XenServer 6.1.</param>
        public static XenRef<Task> async_db_introduce(Session session, string _uuid, string _name_label, string _name_description, string _sr, vdi_type _type, bool _sharable, bool _read_only, Dictionary<string, string> _other_config, string _location, Dictionary<string, string> _xenstore_data, Dictionary<string, string> _sm_config, bool _managed, long _virtual_size, long _physical_utilisation, string _metadata_of_pool, bool _is_a_snapshot, DateTime _snapshot_time, string _snapshot_of)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_db_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_sr != null) ? _sr : "", vdi_type_helper.ToString(_type), _sharable, _read_only, Maps.convert_to_proxy_string_string(_other_config), (_location != null) ? _location : "", Maps.convert_to_proxy_string_string(_xenstore_data), Maps.convert_to_proxy_string_string(_sm_config), _managed, _virtual_size.ToString(), _physical_utilisation.ToString(), (_metadata_of_pool != null) ? _metadata_of_pool : "", _is_a_snapshot, _snapshot_time, (_snapshot_of != null) ? _snapshot_of : "").parse());
        }

        /// <summary>
        /// Removes a VDI record from the database
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static void db_forget(Session session, string _vdi)
        {
            session.proxy.vdi_db_forget(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Removes a VDI record from the database
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static XenRef<Task> async_db_forget(Session session, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_db_forget(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Ask the storage backend to refresh the fields in the VDI object
        /// First published in XenServer 4.1.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static void update(Session session, string _vdi)
        {
            session.proxy.vdi_update(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Ask the storage backend to refresh the fields in the VDI object
        /// First published in XenServer 4.1.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static XenRef<Task> async_update(Session session, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_update(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Copy either a full VDI or the block differences between two VDIs into either a fresh VDI or an existing VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_sr">The destination SR (only required if the destination VDI is not specified</param>
        public static XenRef<VDI> copy(Session session, string _vdi, string _sr)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_copy(session.uuid, (_vdi != null) ? _vdi : "", (_sr != null) ? _sr : "").parse());
        }

        /// <summary>
        /// Copy either a full VDI or the block differences between two VDIs into either a fresh VDI or an existing VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_sr">The destination SR (only required if the destination VDI is not specified</param>
        public static XenRef<Task> async_copy(Session session, string _vdi, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_copy(session.uuid, (_vdi != null) ? _vdi : "", (_sr != null) ? _sr : "").parse());
        }

        /// <summary>
        /// Copy either a full VDI or the block differences between two VDIs into either a fresh VDI or an existing VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_sr">The destination SR (only required if the destination VDI is not specified</param>
        /// <param name="_base_vdi">The base VDI (only required if copying only changed blocks, by default all blocks will be copied) First published in XenServer 6.2 SP1 Hotfix XS62ESP1004.</param>
        /// <param name="_into_vdi">The destination VDI to copy blocks into (if omitted then a destination SR must be provided and a fresh VDI will be created) First published in XenServer 6.2 SP1 Hotfix XS62ESP1004.</param>
        public static XenRef<VDI> copy(Session session, string _vdi, string _sr, string _base_vdi, string _into_vdi)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_copy(session.uuid, (_vdi != null) ? _vdi : "", (_sr != null) ? _sr : "", (_base_vdi != null) ? _base_vdi : "", (_into_vdi != null) ? _into_vdi : "").parse());
        }

        /// <summary>
        /// Copy either a full VDI or the block differences between two VDIs into either a fresh VDI or an existing VDI.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_sr">The destination SR (only required if the destination VDI is not specified</param>
        /// <param name="_base_vdi">The base VDI (only required if copying only changed blocks, by default all blocks will be copied) First published in XenServer 6.2 SP1 Hotfix XS62ESP1004.</param>
        /// <param name="_into_vdi">The destination VDI to copy blocks into (if omitted then a destination SR must be provided and a fresh VDI will be created) First published in XenServer 6.2 SP1 Hotfix XS62ESP1004.</param>
        public static XenRef<Task> async_copy(Session session, string _vdi, string _sr, string _base_vdi, string _into_vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_copy(session.uuid, (_vdi != null) ? _vdi : "", (_sr != null) ? _sr : "", (_base_vdi != null) ? _base_vdi : "", (_into_vdi != null) ? _into_vdi : "").parse());
        }

        /// <summary>
        /// Sets the VDI's managed field
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The new value of the VDI's managed field</param>
        public static void set_managed(Session session, string _vdi, bool _value)
        {
            session.proxy.vdi_set_managed(session.uuid, (_vdi != null) ? _vdi : "", _value).parse();
        }

        /// <summary>
        /// Removes a VDI record from the database
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static void forget(Session session, string _vdi)
        {
            session.proxy.vdi_forget(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Removes a VDI record from the database
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static XenRef<Task> async_forget(Session session, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_forget(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Sets the VDI's sharable field
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The new value of the VDI's sharable field</param>
        public static void set_sharable(Session session, string _vdi, bool _value)
        {
            session.proxy.vdi_set_sharable(session.uuid, (_vdi != null) ? _vdi : "", _value).parse();
        }

        /// <summary>
        /// Sets the VDI's read_only field
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The new value of the VDI's read_only field</param>
        public static void set_read_only(Session session, string _vdi, bool _value)
        {
            session.proxy.vdi_set_read_only(session.uuid, (_vdi != null) ? _vdi : "", _value).parse();
        }

        /// <summary>
        /// Sets the VDI's missing field
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The new value of the VDI's missing field</param>
        public static void set_missing(Session session, string _vdi, bool _value)
        {
            session.proxy.vdi_set_missing(session.uuid, (_vdi != null) ? _vdi : "", _value).parse();
        }

        /// <summary>
        /// Sets the VDI's virtual_size field
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The new value of the VDI's virtual size</param>
        public static void set_virtual_size(Session session, string _vdi, long _value)
        {
            session.proxy.vdi_set_virtual_size(session.uuid, (_vdi != null) ? _vdi : "", _value.ToString()).parse();
        }

        /// <summary>
        /// Sets the VDI's physical_utilisation field
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The new value of the VDI's physical utilisation</param>
        public static void set_physical_utilisation(Session session, string _vdi, long _value)
        {
            session.proxy.vdi_set_physical_utilisation(session.uuid, (_vdi != null) ? _vdi : "", _value.ToString()).parse();
        }

        /// <summary>
        /// Sets whether this VDI is a snapshot
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The new value indicating whether this VDI is a snapshot</param>
        public static void set_is_a_snapshot(Session session, string _vdi, bool _value)
        {
            session.proxy.vdi_set_is_a_snapshot(session.uuid, (_vdi != null) ? _vdi : "", _value).parse();
        }

        /// <summary>
        /// Sets the VDI of which this VDI is a snapshot
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The VDI of which this VDI is a snapshot</param>
        public static void set_snapshot_of(Session session, string _vdi, string _value)
        {
            session.proxy.vdi_set_snapshot_of(session.uuid, (_vdi != null) ? _vdi : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Sets the snapshot time of this VDI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The snapshot time of this VDI.</param>
        public static void set_snapshot_time(Session session, string _vdi, DateTime _value)
        {
            session.proxy.vdi_set_snapshot_time(session.uuid, (_vdi != null) ? _vdi : "", _value).parse();
        }

        /// <summary>
        /// Records the pool whose metadata is contained by this VDI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The pool whose metadata is contained by this VDI</param>
        public static void set_metadata_of_pool(Session session, string _vdi, string _value)
        {
            session.proxy.vdi_set_metadata_of_pool(session.uuid, (_vdi != null) ? _vdi : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Set the name label of the VDI. This can only happen when then its SR is currently attached.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The name lable for the VDI</param>
        public static void set_name_label(Session session, string _vdi, string _value)
        {
            session.proxy.vdi_set_name_label(session.uuid, (_vdi != null) ? _vdi : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Set the name label of the VDI. This can only happen when then its SR is currently attached.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The name lable for the VDI</param>
        public static XenRef<Task> async_set_name_label(Session session, string _vdi, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_set_name_label(session.uuid, (_vdi != null) ? _vdi : "", (_value != null) ? _value : "").parse());
        }

        /// <summary>
        /// Set the name description of the VDI. This can only happen when its SR is currently attached.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The name description for the VDI</param>
        public static void set_name_description(Session session, string _vdi, string _value)
        {
            session.proxy.vdi_set_name_description(session.uuid, (_vdi != null) ? _vdi : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Set the name description of the VDI. This can only happen when its SR is currently attached.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The name description for the VDI</param>
        public static XenRef<Task> async_set_name_description(Session session, string _vdi, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_set_name_description(session.uuid, (_vdi != null) ? _vdi : "", (_value != null) ? _value : "").parse());
        }

        /// <summary>
        /// Set the value of the on_boot parameter. This value can only be changed when the VDI is not attached to a running VM.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The value to set</param>
        public static void set_on_boot(Session session, string _vdi, on_boot _value)
        {
            session.proxy.vdi_set_on_boot(session.uuid, (_vdi != null) ? _vdi : "", on_boot_helper.ToString(_value)).parse();
        }

        /// <summary>
        /// Set the value of the on_boot parameter. This value can only be changed when the VDI is not attached to a running VM.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The value to set</param>
        public static XenRef<Task> async_set_on_boot(Session session, string _vdi, on_boot _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_set_on_boot(session.uuid, (_vdi != null) ? _vdi : "", on_boot_helper.ToString(_value)).parse());
        }

        /// <summary>
        /// Set the value of the allow_caching parameter. This value can only be changed when the VDI is not attached to a running VM. The caching behaviour is only affected by this flag for VHD-based VDIs that have one parent and no child VHDs. Moreover, caching only takes place when the host running the VM containing this VDI has a nominated SR for local caching.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The value to set</param>
        public static void set_allow_caching(Session session, string _vdi, bool _value)
        {
            session.proxy.vdi_set_allow_caching(session.uuid, (_vdi != null) ? _vdi : "", _value).parse();
        }

        /// <summary>
        /// Set the value of the allow_caching parameter. This value can only be changed when the VDI is not attached to a running VM. The caching behaviour is only affected by this flag for VHD-based VDIs that have one parent and no child VHDs. Moreover, caching only takes place when the host running the VM containing this VDI has a nominated SR for local caching.
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_value">The value to set</param>
        public static XenRef<Task> async_set_allow_caching(Session session, string _vdi, bool _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_set_allow_caching(session.uuid, (_vdi != null) ? _vdi : "", _value).parse());
        }

        /// <summary>
        /// Load the metadata found on the supplied VDI and return a session reference which can be used in XenAPI calls to query its contents.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static XenRef<Session> open_database(Session session, string _vdi)
        {
            return XenRef<Session>.Create(session.proxy.vdi_open_database(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Load the metadata found on the supplied VDI and return a session reference which can be used in XenAPI calls to query its contents.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static XenRef<Task> async_open_database(Session session, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_open_database(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Check the VDI cache for the pool UUID of the database on this VDI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static string read_database_pool_uuid(Session session, string _vdi)
        {
            return (string)session.proxy.vdi_read_database_pool_uuid(session.uuid, (_vdi != null) ? _vdi : "").parse();
        }

        /// <summary>
        /// Check the VDI cache for the pool UUID of the database on this VDI.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        public static XenRef<Task> async_read_database_pool_uuid(Session session, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_read_database_pool_uuid(session.uuid, (_vdi != null) ? _vdi : "").parse());
        }

        /// <summary>
        /// Migrate a VDI, which may be attached to a running guest, to a different SR. The destination SR must be visible to the guest.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_sr">The destination SR</param>
        /// <param name="_options">Other parameters</param>
        public static XenRef<VDI> pool_migrate(Session session, string _vdi, string _sr, Dictionary<string, string> _options)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_pool_migrate(session.uuid, (_vdi != null) ? _vdi : "", (_sr != null) ? _sr : "", Maps.convert_to_proxy_string_string(_options)).parse());
        }

        /// <summary>
        /// Migrate a VDI, which may be attached to a running guest, to a different SR. The destination SR must be visible to the guest.
        /// First published in XenServer 6.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vdi">The opaque_ref of the given vdi</param>
        /// <param name="_sr">The destination SR</param>
        /// <param name="_options">Other parameters</param>
        public static XenRef<Task> async_pool_migrate(Session session, string _vdi, string _sr, Dictionary<string, string> _options)
        {
            return XenRef<Task>.Create(session.proxy.async_vdi_pool_migrate(session.uuid, (_vdi != null) ? _vdi : "", (_sr != null) ? _sr : "", Maps.convert_to_proxy_string_string(_options)).parse());
        }

        /// <summary>
        /// Return a list of all the VDIs known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VDI>> get_all(Session session)
        {
            return XenRef<VDI>.Create(session.proxy.vdi_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the VDI Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VDI>, VDI> get_all_records(Session session)
        {
            return XenRef<VDI>.Create<Proxy_VDI>(session.proxy.vdi_get_all_records(session.uuid).parse());
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
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<vdi_operations> allowed_operations
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
        private List<vdi_operations> _allowed_operations;

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, vdi_operations> current_operations
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
        private Dictionary<string, vdi_operations> _current_operations;

        /// <summary>
        /// storage repository in which the VDI resides
        /// </summary>
        public virtual XenRef<SR> SR
        {
            get { return _SR; }
            set
            {
                if (!Helper.AreEqual(value, _SR))
                {
                    _SR = value;
                    Changed = true;
                    NotifyPropertyChanged("SR");
                }
            }
        }
        private XenRef<SR> _SR;

        /// <summary>
        /// list of vbds that refer to this disk
        /// </summary>
        public virtual List<XenRef<VBD>> VBDs
        {
            get { return _VBDs; }
            set
            {
                if (!Helper.AreEqual(value, _VBDs))
                {
                    _VBDs = value;
                    Changed = true;
                    NotifyPropertyChanged("VBDs");
                }
            }
        }
        private List<XenRef<VBD>> _VBDs;

        /// <summary>
        /// list of crash dumps that refer to this disk
        /// </summary>
        public virtual List<XenRef<Crashdump>> crash_dumps
        {
            get { return _crash_dumps; }
            set
            {
                if (!Helper.AreEqual(value, _crash_dumps))
                {
                    _crash_dumps = value;
                    Changed = true;
                    NotifyPropertyChanged("crash_dumps");
                }
            }
        }
        private List<XenRef<Crashdump>> _crash_dumps;

        /// <summary>
        /// size of disk as presented to the guest (in bytes). Note that, depending on storage backend type, requested size may not be respected exactly
        /// </summary>
        public virtual long virtual_size
        {
            get { return _virtual_size; }
            set
            {
                if (!Helper.AreEqual(value, _virtual_size))
                {
                    _virtual_size = value;
                    Changed = true;
                    NotifyPropertyChanged("virtual_size");
                }
            }
        }
        private long _virtual_size;

        /// <summary>
        /// amount of physical space that the disk image is currently taking up on the storage repository (in bytes)
        /// </summary>
        public virtual long physical_utilisation
        {
            get { return _physical_utilisation; }
            set
            {
                if (!Helper.AreEqual(value, _physical_utilisation))
                {
                    _physical_utilisation = value;
                    Changed = true;
                    NotifyPropertyChanged("physical_utilisation");
                }
            }
        }
        private long _physical_utilisation;

        /// <summary>
        /// type of the VDI
        /// </summary>
        public virtual vdi_type type
        {
            get { return _type; }
            set
            {
                if (!Helper.AreEqual(value, _type))
                {
                    _type = value;
                    Changed = true;
                    NotifyPropertyChanged("type");
                }
            }
        }
        private vdi_type _type;

        /// <summary>
        /// true if this disk may be shared
        /// </summary>
        public virtual bool sharable
        {
            get { return _sharable; }
            set
            {
                if (!Helper.AreEqual(value, _sharable))
                {
                    _sharable = value;
                    Changed = true;
                    NotifyPropertyChanged("sharable");
                }
            }
        }
        private bool _sharable;

        /// <summary>
        /// true if this disk may ONLY be mounted read-only
        /// </summary>
        public virtual bool read_only
        {
            get { return _read_only; }
            set
            {
                if (!Helper.AreEqual(value, _read_only))
                {
                    _read_only = value;
                    Changed = true;
                    NotifyPropertyChanged("read_only");
                }
            }
        }
        private bool _read_only;

        /// <summary>
        /// additional configuration
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

        /// <summary>
        /// true if this disk is locked at the storage level
        /// </summary>
        public virtual bool storage_lock
        {
            get { return _storage_lock; }
            set
            {
                if (!Helper.AreEqual(value, _storage_lock))
                {
                    _storage_lock = value;
                    Changed = true;
                    NotifyPropertyChanged("storage_lock");
                }
            }
        }
        private bool _storage_lock;

        /// <summary>
        /// location information
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual string location
        {
            get { return _location; }
            set
            {
                if (!Helper.AreEqual(value, _location))
                {
                    _location = value;
                    Changed = true;
                    NotifyPropertyChanged("location");
                }
            }
        }
        private string _location;

        /// <summary>
        /// 
        /// </summary>
        public virtual bool managed
        {
            get { return _managed; }
            set
            {
                if (!Helper.AreEqual(value, _managed))
                {
                    _managed = value;
                    Changed = true;
                    NotifyPropertyChanged("managed");
                }
            }
        }
        private bool _managed;

        /// <summary>
        /// true if SR scan operation reported this VDI as not present on disk
        /// </summary>
        public virtual bool missing
        {
            get { return _missing; }
            set
            {
                if (!Helper.AreEqual(value, _missing))
                {
                    _missing = value;
                    Changed = true;
                    NotifyPropertyChanged("missing");
                }
            }
        }
        private bool _missing;

        /// <summary>
        /// This field is always null. Deprecated
        /// </summary>
        public virtual XenRef<VDI> parent
        {
            get { return _parent; }
            set
            {
                if (!Helper.AreEqual(value, _parent))
                {
                    _parent = value;
                    Changed = true;
                    NotifyPropertyChanged("parent");
                }
            }
        }
        private XenRef<VDI> _parent;

        /// <summary>
        /// data to be inserted into the xenstore tree (/local/domain/0/backend/vbd/<domid>/<device-id>/sm-data) after the VDI is attached. This is generally set by the SM backends on vdi_attach.
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual Dictionary<string, string> xenstore_data
        {
            get { return _xenstore_data; }
            set
            {
                if (!Helper.AreEqual(value, _xenstore_data))
                {
                    _xenstore_data = value;
                    Changed = true;
                    NotifyPropertyChanged("xenstore_data");
                }
            }
        }
        private Dictionary<string, string> _xenstore_data;

        /// <summary>
        /// SM dependent data
        /// First published in XenServer 4.1.
        /// </summary>
        public virtual Dictionary<string, string> sm_config
        {
            get { return _sm_config; }
            set
            {
                if (!Helper.AreEqual(value, _sm_config))
                {
                    _sm_config = value;
                    Changed = true;
                    NotifyPropertyChanged("sm_config");
                }
            }
        }
        private Dictionary<string, string> _sm_config;

        /// <summary>
        /// true if this is a snapshot.
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool is_a_snapshot
        {
            get { return _is_a_snapshot; }
            set
            {
                if (!Helper.AreEqual(value, _is_a_snapshot))
                {
                    _is_a_snapshot = value;
                    Changed = true;
                    NotifyPropertyChanged("is_a_snapshot");
                }
            }
        }
        private bool _is_a_snapshot;

        /// <summary>
        /// Ref pointing to the VDI this snapshot is of.
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual XenRef<VDI> snapshot_of
        {
            get { return _snapshot_of; }
            set
            {
                if (!Helper.AreEqual(value, _snapshot_of))
                {
                    _snapshot_of = value;
                    Changed = true;
                    NotifyPropertyChanged("snapshot_of");
                }
            }
        }
        private XenRef<VDI> _snapshot_of;

        /// <summary>
        /// List pointing to all the VDIs snapshots.
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual List<XenRef<VDI>> snapshots
        {
            get { return _snapshots; }
            set
            {
                if (!Helper.AreEqual(value, _snapshots))
                {
                    _snapshots = value;
                    Changed = true;
                    NotifyPropertyChanged("snapshots");
                }
            }
        }
        private List<XenRef<VDI>> _snapshots;

        /// <summary>
        /// Date/time when this snapshot was created.
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual DateTime snapshot_time
        {
            get { return _snapshot_time; }
            set
            {
                if (!Helper.AreEqual(value, _snapshot_time))
                {
                    _snapshot_time = value;
                    Changed = true;
                    NotifyPropertyChanged("snapshot_time");
                }
            }
        }
        private DateTime _snapshot_time;

        /// <summary>
        /// user-specified tags for categorization purposes
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual string[] tags
        {
            get { return _tags; }
            set
            {
                if (!Helper.AreEqual(value, _tags))
                {
                    _tags = value;
                    Changed = true;
                    NotifyPropertyChanged("tags");
                }
            }
        }
        private string[] _tags;

        /// <summary>
        /// true if this VDI is to be cached in the local cache SR
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        public virtual bool allow_caching
        {
            get { return _allow_caching; }
            set
            {
                if (!Helper.AreEqual(value, _allow_caching))
                {
                    _allow_caching = value;
                    Changed = true;
                    NotifyPropertyChanged("allow_caching");
                }
            }
        }
        private bool _allow_caching;

        /// <summary>
        /// The behaviour of this VDI on a VM boot
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        public virtual on_boot on_boot
        {
            get { return _on_boot; }
            set
            {
                if (!Helper.AreEqual(value, _on_boot))
                {
                    _on_boot = value;
                    Changed = true;
                    NotifyPropertyChanged("on_boot");
                }
            }
        }
        private on_boot _on_boot;

        /// <summary>
        /// The pool whose metadata is contained in this VDI
        /// First published in XenServer 6.0.
        /// </summary>
        public virtual XenRef<Pool> metadata_of_pool
        {
            get { return _metadata_of_pool; }
            set
            {
                if (!Helper.AreEqual(value, _metadata_of_pool))
                {
                    _metadata_of_pool = value;
                    Changed = true;
                    NotifyPropertyChanged("metadata_of_pool");
                }
            }
        }
        private XenRef<Pool> _metadata_of_pool;

        /// <summary>
        /// Whether this VDI contains the latest known accessible metadata for the pool
        /// First published in XenServer 6.0.
        /// </summary>
        public virtual bool metadata_latest
        {
            get { return _metadata_latest; }
            set
            {
                if (!Helper.AreEqual(value, _metadata_latest))
                {
                    _metadata_latest = value;
                    Changed = true;
                    NotifyPropertyChanged("metadata_latest");
                }
            }
        }
        private bool _metadata_latest;

        /// <summary>
        /// Whether this VDI is a Tools ISO
        /// First published in XenServer Dundee.
        /// </summary>
        public virtual bool is_tools_iso
        {
            get { return _is_tools_iso; }
            set
            {
                if (!Helper.AreEqual(value, _is_tools_iso))
                {
                    _is_tools_iso = value;
                    Changed = true;
                    NotifyPropertyChanged("is_tools_iso");
                }
            }
        }
        private bool _is_tools_iso;
    }
}
