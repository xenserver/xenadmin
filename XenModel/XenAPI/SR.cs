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
    public partial class SR : XenObject<SR>
    {
        public SR()
        {
        }

        public SR(string uuid,
            string name_label,
            string name_description,
            List<storage_operations> allowed_operations,
            Dictionary<string, storage_operations> current_operations,
            List<XenRef<VDI>> VDIs,
            List<XenRef<PBD>> PBDs,
            long virtual_allocation,
            long physical_utilisation,
            long physical_size,
            string type,
            string content_type,
            bool shared,
            Dictionary<string, string> other_config,
            string[] tags,
            Dictionary<string, string> sm_config,
            Dictionary<string, XenRef<Blob>> blobs,
            bool local_cache_enabled,
            XenRef<DR_task> introduced_by)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.VDIs = VDIs;
            this.PBDs = PBDs;
            this.virtual_allocation = virtual_allocation;
            this.physical_utilisation = physical_utilisation;
            this.physical_size = physical_size;
            this.type = type;
            this.content_type = content_type;
            this.shared = shared;
            this.other_config = other_config;
            this.tags = tags;
            this.sm_config = sm_config;
            this.blobs = blobs;
            this.local_cache_enabled = local_cache_enabled;
            this.introduced_by = introduced_by;
        }

        /// <summary>
        /// Creates a new SR from a Proxy_SR.
        /// </summary>
        /// <param name="proxy"></param>
        public SR(Proxy_SR proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(SR update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            allowed_operations = update.allowed_operations;
            current_operations = update.current_operations;
            VDIs = update.VDIs;
            PBDs = update.PBDs;
            virtual_allocation = update.virtual_allocation;
            physical_utilisation = update.physical_utilisation;
            physical_size = update.physical_size;
            type = update.type;
            content_type = update.content_type;
            shared = update.shared;
            other_config = update.other_config;
            tags = update.tags;
            sm_config = update.sm_config;
            blobs = update.blobs;
            local_cache_enabled = update.local_cache_enabled;
            introduced_by = update.introduced_by;
        }

        internal void UpdateFromProxy(Proxy_SR proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            allowed_operations = proxy.allowed_operations == null ? null : Helper.StringArrayToEnumList<storage_operations>(proxy.allowed_operations);
            current_operations = proxy.current_operations == null ? null : Maps.convert_from_proxy_string_storage_operations(proxy.current_operations);
            VDIs = proxy.VDIs == null ? null : XenRef<VDI>.Create(proxy.VDIs);
            PBDs = proxy.PBDs == null ? null : XenRef<PBD>.Create(proxy.PBDs);
            virtual_allocation = proxy.virtual_allocation == null ? 0 : long.Parse((string)proxy.virtual_allocation);
            physical_utilisation = proxy.physical_utilisation == null ? 0 : long.Parse((string)proxy.physical_utilisation);
            physical_size = proxy.physical_size == null ? 0 : long.Parse((string)proxy.physical_size);
            type = proxy.type == null ? null : (string)proxy.type;
            content_type = proxy.content_type == null ? null : (string)proxy.content_type;
            shared = (bool)proxy.shared;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            tags = proxy.tags == null ? new string[] {} : (string [])proxy.tags;
            sm_config = proxy.sm_config == null ? null : Maps.convert_from_proxy_string_string(proxy.sm_config);
            blobs = proxy.blobs == null ? null : Maps.convert_from_proxy_string_XenRefBlob(proxy.blobs);
            local_cache_enabled = (bool)proxy.local_cache_enabled;
            introduced_by = proxy.introduced_by == null ? null : XenRef<DR_task>.Create(proxy.introduced_by);
        }

        public Proxy_SR ToProxy()
        {
            Proxy_SR result_ = new Proxy_SR();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.allowed_operations = (allowed_operations != null) ? Helper.ObjectListToStringArray(allowed_operations) : new string[] {};
            result_.current_operations = Maps.convert_to_proxy_string_storage_operations(current_operations);
            result_.VDIs = (VDIs != null) ? Helper.RefListToStringArray(VDIs) : new string[] {};
            result_.PBDs = (PBDs != null) ? Helper.RefListToStringArray(PBDs) : new string[] {};
            result_.virtual_allocation = virtual_allocation.ToString();
            result_.physical_utilisation = physical_utilisation.ToString();
            result_.physical_size = physical_size.ToString();
            result_.type = (type != null) ? type : "";
            result_.content_type = (content_type != null) ? content_type : "";
            result_.shared = shared;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.tags = tags;
            result_.sm_config = Maps.convert_to_proxy_string_string(sm_config);
            result_.blobs = Maps.convert_to_proxy_string_XenRefBlob(blobs);
            result_.local_cache_enabled = local_cache_enabled;
            result_.introduced_by = (introduced_by != null) ? introduced_by : "";
            return result_;
        }

        /// <summary>
        /// Creates a new SR from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public SR(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            allowed_operations = Helper.StringArrayToEnumList<storage_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            current_operations = Maps.convert_from_proxy_string_storage_operations(Marshalling.ParseHashTable(table, "current_operations"));
            VDIs = Marshalling.ParseSetRef<VDI>(table, "VDIs");
            PBDs = Marshalling.ParseSetRef<PBD>(table, "PBDs");
            virtual_allocation = Marshalling.ParseLong(table, "virtual_allocation");
            physical_utilisation = Marshalling.ParseLong(table, "physical_utilisation");
            physical_size = Marshalling.ParseLong(table, "physical_size");
            type = Marshalling.ParseString(table, "type");
            content_type = Marshalling.ParseString(table, "content_type");
            shared = Marshalling.ParseBool(table, "shared");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            tags = Marshalling.ParseStringArray(table, "tags");
            sm_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "sm_config"));
            blobs = Maps.convert_from_proxy_string_XenRefBlob(Marshalling.ParseHashTable(table, "blobs"));
            local_cache_enabled = Marshalling.ParseBool(table, "local_cache_enabled");
            introduced_by = Marshalling.ParseRef<DR_task>(table, "introduced_by");
        }

        public bool DeepEquals(SR other, bool ignoreCurrentOperations)
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
                Helper.AreEqual2(this._VDIs, other._VDIs) &&
                Helper.AreEqual2(this._PBDs, other._PBDs) &&
                Helper.AreEqual2(this._virtual_allocation, other._virtual_allocation) &&
                Helper.AreEqual2(this._physical_utilisation, other._physical_utilisation) &&
                Helper.AreEqual2(this._physical_size, other._physical_size) &&
                Helper.AreEqual2(this._type, other._type) &&
                Helper.AreEqual2(this._content_type, other._content_type) &&
                Helper.AreEqual2(this._shared, other._shared) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._tags, other._tags) &&
                Helper.AreEqual2(this._sm_config, other._sm_config) &&
                Helper.AreEqual2(this._blobs, other._blobs) &&
                Helper.AreEqual2(this._local_cache_enabled, other._local_cache_enabled) &&
                Helper.AreEqual2(this._introduced_by, other._introduced_by);
        }

        public override string SaveChanges(Session session, string opaqueRef, SR server)
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
                    SR.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_tags, server._tags))
                {
                    SR.set_tags(session, opaqueRef, _tags);
                }
                if (!Helper.AreEqual2(_sm_config, server._sm_config))
                {
                    SR.set_sm_config(session, opaqueRef, _sm_config);
                }
                if (!Helper.AreEqual2(_name_label, server._name_label))
                {
                    SR.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    SR.set_name_description(session, opaqueRef, _name_description);
                }
                if (!Helper.AreEqual2(_physical_size, server._physical_size))
                {
                    SR.set_physical_size(session, opaqueRef, _physical_size);
                }

                return null;
            }
        }

        public static SR get_record(Session session, string _sr)
        {
            return new SR((Proxy_SR)session.proxy.sr_get_record(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static XenRef<SR> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<SR>.Create(session.proxy.sr_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static List<XenRef<SR>> get_by_name_label(Session session, string _label)
        {
            return XenRef<SR>.Create(session.proxy.sr_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        public static string get_uuid(Session session, string _sr)
        {
            return (string)session.proxy.sr_get_uuid(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static string get_name_label(Session session, string _sr)
        {
            return (string)session.proxy.sr_get_name_label(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static string get_name_description(Session session, string _sr)
        {
            return (string)session.proxy.sr_get_name_description(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static List<storage_operations> get_allowed_operations(Session session, string _sr)
        {
            return Helper.StringArrayToEnumList<storage_operations>(session.proxy.sr_get_allowed_operations(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static Dictionary<string, storage_operations> get_current_operations(Session session, string _sr)
        {
            return Maps.convert_from_proxy_string_storage_operations(session.proxy.sr_get_current_operations(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static List<XenRef<VDI>> get_VDIs(Session session, string _sr)
        {
            return XenRef<VDI>.Create(session.proxy.sr_get_vdis(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static List<XenRef<PBD>> get_PBDs(Session session, string _sr)
        {
            return XenRef<PBD>.Create(session.proxy.sr_get_pbds(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static long get_virtual_allocation(Session session, string _sr)
        {
            return long.Parse((string)session.proxy.sr_get_virtual_allocation(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static long get_physical_utilisation(Session session, string _sr)
        {
            return long.Parse((string)session.proxy.sr_get_physical_utilisation(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static long get_physical_size(Session session, string _sr)
        {
            return long.Parse((string)session.proxy.sr_get_physical_size(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static string get_type(Session session, string _sr)
        {
            return (string)session.proxy.sr_get_type(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static string get_content_type(Session session, string _sr)
        {
            return (string)session.proxy.sr_get_content_type(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static bool get_shared(Session session, string _sr)
        {
            return (bool)session.proxy.sr_get_shared(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static Dictionary<string, string> get_other_config(Session session, string _sr)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.sr_get_other_config(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static string[] get_tags(Session session, string _sr)
        {
            return (string [])session.proxy.sr_get_tags(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static Dictionary<string, string> get_sm_config(Session session, string _sr)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.sr_get_sm_config(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static Dictionary<string, XenRef<Blob>> get_blobs(Session session, string _sr)
        {
            return Maps.convert_from_proxy_string_XenRefBlob(session.proxy.sr_get_blobs(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static bool get_local_cache_enabled(Session session, string _sr)
        {
            return (bool)session.proxy.sr_get_local_cache_enabled(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static XenRef<DR_task> get_introduced_by(Session session, string _sr)
        {
            return XenRef<DR_task>.Create(session.proxy.sr_get_introduced_by(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static void set_other_config(Session session, string _sr, Dictionary<string, string> _other_config)
        {
            session.proxy.sr_set_other_config(session.uuid, (_sr != null) ? _sr : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _sr, string _key, string _value)
        {
            session.proxy.sr_add_to_other_config(session.uuid, (_sr != null) ? _sr : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _sr, string _key)
        {
            session.proxy.sr_remove_from_other_config(session.uuid, (_sr != null) ? _sr : "", (_key != null) ? _key : "").parse();
        }

        public static void set_tags(Session session, string _sr, string[] _tags)
        {
            session.proxy.sr_set_tags(session.uuid, (_sr != null) ? _sr : "", _tags).parse();
        }

        public static void add_tags(Session session, string _sr, string _value)
        {
            session.proxy.sr_add_tags(session.uuid, (_sr != null) ? _sr : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_tags(Session session, string _sr, string _value)
        {
            session.proxy.sr_remove_tags(session.uuid, (_sr != null) ? _sr : "", (_value != null) ? _value : "").parse();
        }

        public static void set_sm_config(Session session, string _sr, Dictionary<string, string> _sm_config)
        {
            session.proxy.sr_set_sm_config(session.uuid, (_sr != null) ? _sr : "", Maps.convert_to_proxy_string_string(_sm_config)).parse();
        }

        public static void add_to_sm_config(Session session, string _sr, string _key, string _value)
        {
            session.proxy.sr_add_to_sm_config(session.uuid, (_sr != null) ? _sr : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_sm_config(Session session, string _sr, string _key)
        {
            session.proxy.sr_remove_from_sm_config(session.uuid, (_sr != null) ? _sr : "", (_key != null) ? _key : "").parse();
        }

        public static XenRef<SR> create(Session session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type, bool _shared, Dictionary<string, string> _sm_config)
        {
            return XenRef<SR>.Create(session.proxy.sr_create(session.uuid, (_host != null) ? _host : "", Maps.convert_to_proxy_string_string(_device_config), _physical_size.ToString(), (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_type != null) ? _type : "", (_content_type != null) ? _content_type : "", _shared, Maps.convert_to_proxy_string_string(_sm_config)).parse());
        }

        public static XenRef<Task> async_create(Session session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type, bool _shared, Dictionary<string, string> _sm_config)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_create(session.uuid, (_host != null) ? _host : "", Maps.convert_to_proxy_string_string(_device_config), _physical_size.ToString(), (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_type != null) ? _type : "", (_content_type != null) ? _content_type : "", _shared, Maps.convert_to_proxy_string_string(_sm_config)).parse());
        }

        public static XenRef<SR> introduce(Session session, string _uuid, string _name_label, string _name_description, string _type, string _content_type, bool _shared, Dictionary<string, string> _sm_config)
        {
            return XenRef<SR>.Create(session.proxy.sr_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_type != null) ? _type : "", (_content_type != null) ? _content_type : "", _shared, Maps.convert_to_proxy_string_string(_sm_config)).parse());
        }

        public static XenRef<Task> async_introduce(Session session, string _uuid, string _name_label, string _name_description, string _type, string _content_type, bool _shared, Dictionary<string, string> _sm_config)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_introduce(session.uuid, (_uuid != null) ? _uuid : "", (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_type != null) ? _type : "", (_content_type != null) ? _content_type : "", _shared, Maps.convert_to_proxy_string_string(_sm_config)).parse());
        }

        public static string make(Session session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type, Dictionary<string, string> _sm_config)
        {
            return (string)session.proxy.sr_make(session.uuid, (_host != null) ? _host : "", Maps.convert_to_proxy_string_string(_device_config), _physical_size.ToString(), (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_type != null) ? _type : "", (_content_type != null) ? _content_type : "", Maps.convert_to_proxy_string_string(_sm_config)).parse();
        }

        public static XenRef<Task> async_make(Session session, string _host, Dictionary<string, string> _device_config, long _physical_size, string _name_label, string _name_description, string _type, string _content_type, Dictionary<string, string> _sm_config)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_make(session.uuid, (_host != null) ? _host : "", Maps.convert_to_proxy_string_string(_device_config), _physical_size.ToString(), (_name_label != null) ? _name_label : "", (_name_description != null) ? _name_description : "", (_type != null) ? _type : "", (_content_type != null) ? _content_type : "", Maps.convert_to_proxy_string_string(_sm_config)).parse());
        }

        public static void destroy(Session session, string _sr)
        {
            session.proxy.sr_destroy(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_destroy(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static void forget(Session session, string _sr)
        {
            session.proxy.sr_forget(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static XenRef<Task> async_forget(Session session, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_forget(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static void update(Session session, string _sr)
        {
            session.proxy.sr_update(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static XenRef<Task> async_update(Session session, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_update(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static string[] get_supported_types(Session session)
        {
            return (string [])session.proxy.sr_get_supported_types(session.uuid).parse();
        }

        public static void scan(Session session, string _sr)
        {
            session.proxy.sr_scan(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static XenRef<Task> async_scan(Session session, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_scan(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static string probe(Session session, string _host, Dictionary<string, string> _device_config, string _type, Dictionary<string, string> _sm_config)
        {
            return (string)session.proxy.sr_probe(session.uuid, (_host != null) ? _host : "", Maps.convert_to_proxy_string_string(_device_config), (_type != null) ? _type : "", Maps.convert_to_proxy_string_string(_sm_config)).parse();
        }

        public static XenRef<Task> async_probe(Session session, string _host, Dictionary<string, string> _device_config, string _type, Dictionary<string, string> _sm_config)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_probe(session.uuid, (_host != null) ? _host : "", Maps.convert_to_proxy_string_string(_device_config), (_type != null) ? _type : "", Maps.convert_to_proxy_string_string(_sm_config)).parse());
        }

        public static void set_shared(Session session, string _sr, bool _value)
        {
            session.proxy.sr_set_shared(session.uuid, (_sr != null) ? _sr : "", _value).parse();
        }

        public static XenRef<Task> async_set_shared(Session session, string _sr, bool _value)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_set_shared(session.uuid, (_sr != null) ? _sr : "", _value).parse());
        }

        public static void set_name_label(Session session, string _sr, string _value)
        {
            session.proxy.sr_set_name_label(session.uuid, (_sr != null) ? _sr : "", (_value != null) ? _value : "").parse();
        }

        public static XenRef<Task> async_set_name_label(Session session, string _sr, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_set_name_label(session.uuid, (_sr != null) ? _sr : "", (_value != null) ? _value : "").parse());
        }

        public static void set_name_description(Session session, string _sr, string _value)
        {
            session.proxy.sr_set_name_description(session.uuid, (_sr != null) ? _sr : "", (_value != null) ? _value : "").parse();
        }

        public static XenRef<Task> async_set_name_description(Session session, string _sr, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_set_name_description(session.uuid, (_sr != null) ? _sr : "", (_value != null) ? _value : "").parse());
        }

        public static XenRef<Blob> create_new_blob(Session session, string _sr, string _name, string _mime_type, bool _public)
        {
            return XenRef<Blob>.Create(session.proxy.sr_create_new_blob(session.uuid, (_sr != null) ? _sr : "", (_name != null) ? _name : "", (_mime_type != null) ? _mime_type : "", _public).parse());
        }

        public static XenRef<Task> async_create_new_blob(Session session, string _sr, string _name, string _mime_type, bool _public)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_create_new_blob(session.uuid, (_sr != null) ? _sr : "", (_name != null) ? _name : "", (_mime_type != null) ? _mime_type : "", _public).parse());
        }

        public static void set_physical_size(Session session, string _self, long _value)
        {
            session.proxy.sr_set_physical_size(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void set_virtual_allocation(Session session, string _self, long _value)
        {
            session.proxy.sr_set_virtual_allocation(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void set_physical_utilisation(Session session, string _self, long _value)
        {
            session.proxy.sr_set_physical_utilisation(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void assert_can_host_ha_statefile(Session session, string _sr)
        {
            session.proxy.sr_assert_can_host_ha_statefile(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static XenRef<Task> async_assert_can_host_ha_statefile(Session session, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_assert_can_host_ha_statefile(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static void assert_supports_database_replication(Session session, string _sr)
        {
            session.proxy.sr_assert_supports_database_replication(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static XenRef<Task> async_assert_supports_database_replication(Session session, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_assert_supports_database_replication(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static void enable_database_replication(Session session, string _sr)
        {
            session.proxy.sr_enable_database_replication(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static XenRef<Task> async_enable_database_replication(Session session, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_enable_database_replication(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static void disable_database_replication(Session session, string _sr)
        {
            session.proxy.sr_disable_database_replication(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        public static XenRef<Task> async_disable_database_replication(Session session, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_sr_disable_database_replication(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        public static List<XenRef<SR>> get_all(Session session)
        {
            return XenRef<SR>.Create(session.proxy.sr_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<SR>, SR> get_all_records(Session session)
        {
            return XenRef<SR>.Create<Proxy_SR>(session.proxy.sr_get_all_records(session.uuid).parse());
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

        private List<storage_operations> _allowed_operations;
        public virtual List<storage_operations> allowed_operations {
             get { return _allowed_operations; }
             set { if (!Helper.AreEqual(value, _allowed_operations)) { _allowed_operations = value; Changed = true; NotifyPropertyChanged("allowed_operations"); } }
         }

        private Dictionary<string, storage_operations> _current_operations;
        public virtual Dictionary<string, storage_operations> current_operations {
             get { return _current_operations; }
             set { if (!Helper.AreEqual(value, _current_operations)) { _current_operations = value; Changed = true; NotifyPropertyChanged("current_operations"); } }
         }

        private List<XenRef<VDI>> _VDIs;
        public virtual List<XenRef<VDI>> VDIs {
             get { return _VDIs; }
             set { if (!Helper.AreEqual(value, _VDIs)) { _VDIs = value; Changed = true; NotifyPropertyChanged("VDIs"); } }
         }

        private List<XenRef<PBD>> _PBDs;
        public virtual List<XenRef<PBD>> PBDs {
             get { return _PBDs; }
             set { if (!Helper.AreEqual(value, _PBDs)) { _PBDs = value; Changed = true; NotifyPropertyChanged("PBDs"); } }
         }

        private long _virtual_allocation;
        public virtual long virtual_allocation {
             get { return _virtual_allocation; }
             set { if (!Helper.AreEqual(value, _virtual_allocation)) { _virtual_allocation = value; Changed = true; NotifyPropertyChanged("virtual_allocation"); } }
         }

        private long _physical_utilisation;
        public virtual long physical_utilisation {
             get { return _physical_utilisation; }
             set { if (!Helper.AreEqual(value, _physical_utilisation)) { _physical_utilisation = value; Changed = true; NotifyPropertyChanged("physical_utilisation"); } }
         }

        private long _physical_size;
        public virtual long physical_size {
             get { return _physical_size; }
             set { if (!Helper.AreEqual(value, _physical_size)) { _physical_size = value; Changed = true; NotifyPropertyChanged("physical_size"); } }
         }

        private string _type;
        public virtual string type {
             get { return _type; }
             set { if (!Helper.AreEqual(value, _type)) { _type = value; Changed = true; NotifyPropertyChanged("type"); } }
         }

        private string _content_type;
        public virtual string content_type {
             get { return _content_type; }
             set { if (!Helper.AreEqual(value, _content_type)) { _content_type = value; Changed = true; NotifyPropertyChanged("content_type"); } }
         }

        private bool _shared;
        public virtual bool shared {
             get { return _shared; }
             set { if (!Helper.AreEqual(value, _shared)) { _shared = value; Changed = true; NotifyPropertyChanged("shared"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }

        private string[] _tags;
        public virtual string[] tags {
             get { return _tags; }
             set { if (!Helper.AreEqual(value, _tags)) { _tags = value; Changed = true; NotifyPropertyChanged("tags"); } }
         }

        private Dictionary<string, string> _sm_config;
        public virtual Dictionary<string, string> sm_config {
             get { return _sm_config; }
             set { if (!Helper.AreEqual(value, _sm_config)) { _sm_config = value; Changed = true; NotifyPropertyChanged("sm_config"); } }
         }

        private Dictionary<string, XenRef<Blob>> _blobs;
        public virtual Dictionary<string, XenRef<Blob>> blobs {
             get { return _blobs; }
             set { if (!Helper.AreEqual(value, _blobs)) { _blobs = value; Changed = true; NotifyPropertyChanged("blobs"); } }
         }

        private bool _local_cache_enabled;
        public virtual bool local_cache_enabled {
             get { return _local_cache_enabled; }
             set { if (!Helper.AreEqual(value, _local_cache_enabled)) { _local_cache_enabled = value; Changed = true; NotifyPropertyChanged("local_cache_enabled"); } }
         }

        private XenRef<DR_task> _introduced_by;
        public virtual XenRef<DR_task> introduced_by {
             get { return _introduced_by; }
             set { if (!Helper.AreEqual(value, _introduced_by)) { _introduced_by = value; Changed = true; NotifyPropertyChanged("introduced_by"); } }
         }


    }
}
