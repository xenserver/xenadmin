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
    public partial class VBD : XenObject<VBD>
    {
        public VBD()
        {
        }

        public VBD(string uuid,
            List<vbd_operations> allowed_operations,
            Dictionary<string, vbd_operations> current_operations,
            XenRef<VM> VM,
            XenRef<VDI> VDI,
            string device,
            string userdevice,
            bool bootable,
            vbd_mode mode,
            vbd_type type,
            bool unpluggable,
            bool storage_lock,
            bool empty,
            Dictionary<string, string> other_config,
            bool currently_attached,
            long status_code,
            string status_detail,
            Dictionary<string, string> runtime_properties,
            string qos_algorithm_type,
            Dictionary<string, string> qos_algorithm_params,
            string[] qos_supported_algorithms,
            XenRef<VBD_metrics> metrics)
        {
            this.uuid = uuid;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.VM = VM;
            this.VDI = VDI;
            this.device = device;
            this.userdevice = userdevice;
            this.bootable = bootable;
            this.mode = mode;
            this.type = type;
            this.unpluggable = unpluggable;
            this.storage_lock = storage_lock;
            this.empty = empty;
            this.other_config = other_config;
            this.currently_attached = currently_attached;
            this.status_code = status_code;
            this.status_detail = status_detail;
            this.runtime_properties = runtime_properties;
            this.qos_algorithm_type = qos_algorithm_type;
            this.qos_algorithm_params = qos_algorithm_params;
            this.qos_supported_algorithms = qos_supported_algorithms;
            this.metrics = metrics;
        }

        /// <summary>
        /// Creates a new VBD from a Proxy_VBD.
        /// </summary>
        /// <param name="proxy"></param>
        public VBD(Proxy_VBD proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(VBD update)
        {
            uuid = update.uuid;
            allowed_operations = update.allowed_operations;
            current_operations = update.current_operations;
            VM = update.VM;
            VDI = update.VDI;
            device = update.device;
            userdevice = update.userdevice;
            bootable = update.bootable;
            mode = update.mode;
            type = update.type;
            unpluggable = update.unpluggable;
            storage_lock = update.storage_lock;
            empty = update.empty;
            other_config = update.other_config;
            currently_attached = update.currently_attached;
            status_code = update.status_code;
            status_detail = update.status_detail;
            runtime_properties = update.runtime_properties;
            qos_algorithm_type = update.qos_algorithm_type;
            qos_algorithm_params = update.qos_algorithm_params;
            qos_supported_algorithms = update.qos_supported_algorithms;
            metrics = update.metrics;
        }

        internal void UpdateFromProxy(Proxy_VBD proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            allowed_operations = proxy.allowed_operations == null ? null : Helper.StringArrayToEnumList<vbd_operations>(proxy.allowed_operations);
            current_operations = proxy.current_operations == null ? null : Maps.convert_from_proxy_string_vbd_operations(proxy.current_operations);
            VM = proxy.VM == null ? null : XenRef<VM>.Create(proxy.VM);
            VDI = proxy.VDI == null ? null : XenRef<VDI>.Create(proxy.VDI);
            device = proxy.device == null ? null : (string)proxy.device;
            userdevice = proxy.userdevice == null ? null : (string)proxy.userdevice;
            bootable = (bool)proxy.bootable;
            mode = proxy.mode == null ? (vbd_mode) 0 : (vbd_mode)Helper.EnumParseDefault(typeof(vbd_mode), (string)proxy.mode);
            type = proxy.type == null ? (vbd_type) 0 : (vbd_type)Helper.EnumParseDefault(typeof(vbd_type), (string)proxy.type);
            unpluggable = (bool)proxy.unpluggable;
            storage_lock = (bool)proxy.storage_lock;
            empty = (bool)proxy.empty;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            currently_attached = (bool)proxy.currently_attached;
            status_code = proxy.status_code == null ? 0 : long.Parse((string)proxy.status_code);
            status_detail = proxy.status_detail == null ? null : (string)proxy.status_detail;
            runtime_properties = proxy.runtime_properties == null ? null : Maps.convert_from_proxy_string_string(proxy.runtime_properties);
            qos_algorithm_type = proxy.qos_algorithm_type == null ? null : (string)proxy.qos_algorithm_type;
            qos_algorithm_params = proxy.qos_algorithm_params == null ? null : Maps.convert_from_proxy_string_string(proxy.qos_algorithm_params);
            qos_supported_algorithms = proxy.qos_supported_algorithms == null ? new string[] {} : (string [])proxy.qos_supported_algorithms;
            metrics = proxy.metrics == null ? null : XenRef<VBD_metrics>.Create(proxy.metrics);
        }

        public Proxy_VBD ToProxy()
        {
            Proxy_VBD result_ = new Proxy_VBD();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.allowed_operations = (allowed_operations != null) ? Helper.ObjectListToStringArray(allowed_operations) : new string[] {};
            result_.current_operations = Maps.convert_to_proxy_string_vbd_operations(current_operations);
            result_.VM = (VM != null) ? VM : "";
            result_.VDI = (VDI != null) ? VDI : "";
            result_.device = (device != null) ? device : "";
            result_.userdevice = (userdevice != null) ? userdevice : "";
            result_.bootable = bootable;
            result_.mode = vbd_mode_helper.ToString(mode);
            result_.type = vbd_type_helper.ToString(type);
            result_.unpluggable = unpluggable;
            result_.storage_lock = storage_lock;
            result_.empty = empty;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.currently_attached = currently_attached;
            result_.status_code = status_code.ToString();
            result_.status_detail = (status_detail != null) ? status_detail : "";
            result_.runtime_properties = Maps.convert_to_proxy_string_string(runtime_properties);
            result_.qos_algorithm_type = (qos_algorithm_type != null) ? qos_algorithm_type : "";
            result_.qos_algorithm_params = Maps.convert_to_proxy_string_string(qos_algorithm_params);
            result_.qos_supported_algorithms = qos_supported_algorithms;
            result_.metrics = (metrics != null) ? metrics : "";
            return result_;
        }

        /// <summary>
        /// Creates a new VBD from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public VBD(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            allowed_operations = Helper.StringArrayToEnumList<vbd_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            current_operations = Maps.convert_from_proxy_string_vbd_operations(Marshalling.ParseHashTable(table, "current_operations"));
            VM = Marshalling.ParseRef<VM>(table, "VM");
            VDI = Marshalling.ParseRef<VDI>(table, "VDI");
            device = Marshalling.ParseString(table, "device");
            userdevice = Marshalling.ParseString(table, "userdevice");
            bootable = Marshalling.ParseBool(table, "bootable");
            mode = (vbd_mode)Helper.EnumParseDefault(typeof(vbd_mode), Marshalling.ParseString(table, "mode"));
            type = (vbd_type)Helper.EnumParseDefault(typeof(vbd_type), Marshalling.ParseString(table, "type"));
            unpluggable = Marshalling.ParseBool(table, "unpluggable");
            storage_lock = Marshalling.ParseBool(table, "storage_lock");
            empty = Marshalling.ParseBool(table, "empty");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            currently_attached = Marshalling.ParseBool(table, "currently_attached");
            status_code = Marshalling.ParseLong(table, "status_code");
            status_detail = Marshalling.ParseString(table, "status_detail");
            runtime_properties = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "runtime_properties"));
            qos_algorithm_type = Marshalling.ParseString(table, "qos_algorithm_type");
            qos_algorithm_params = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "qos_algorithm_params"));
            qos_supported_algorithms = Marshalling.ParseStringArray(table, "qos_supported_algorithms");
            metrics = Marshalling.ParseRef<VBD_metrics>(table, "metrics");
        }

        public bool DeepEquals(VBD other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(this.current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(this._VM, other._VM) &&
                Helper.AreEqual2(this._VDI, other._VDI) &&
                Helper.AreEqual2(this._device, other._device) &&
                Helper.AreEqual2(this._userdevice, other._userdevice) &&
                Helper.AreEqual2(this._bootable, other._bootable) &&
                Helper.AreEqual2(this._mode, other._mode) &&
                Helper.AreEqual2(this._type, other._type) &&
                Helper.AreEqual2(this._unpluggable, other._unpluggable) &&
                Helper.AreEqual2(this._storage_lock, other._storage_lock) &&
                Helper.AreEqual2(this._empty, other._empty) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached) &&
                Helper.AreEqual2(this._status_code, other._status_code) &&
                Helper.AreEqual2(this._status_detail, other._status_detail) &&
                Helper.AreEqual2(this._runtime_properties, other._runtime_properties) &&
                Helper.AreEqual2(this._qos_algorithm_type, other._qos_algorithm_type) &&
                Helper.AreEqual2(this._qos_algorithm_params, other._qos_algorithm_params) &&
                Helper.AreEqual2(this._qos_supported_algorithms, other._qos_supported_algorithms) &&
                Helper.AreEqual2(this._metrics, other._metrics);
        }

        public override string SaveChanges(Session session, string opaqueRef, VBD server)
        {
            if (opaqueRef == null)
            {
                Proxy_VBD p = this.ToProxy();
                return session.proxy.vbd_create(session.uuid, p).parse();
            }
            else
            {
                if (!Helper.AreEqual2(_userdevice, server._userdevice))
                {
                    VBD.set_userdevice(session, opaqueRef, _userdevice);
                }
                if (!Helper.AreEqual2(_bootable, server._bootable))
                {
                    VBD.set_bootable(session, opaqueRef, _bootable);
                }
                if (!Helper.AreEqual2(_mode, server._mode))
                {
                    VBD.set_mode(session, opaqueRef, _mode);
                }
                if (!Helper.AreEqual2(_type, server._type))
                {
                    VBD.set_type(session, opaqueRef, _type);
                }
                if (!Helper.AreEqual2(_unpluggable, server._unpluggable))
                {
                    VBD.set_unpluggable(session, opaqueRef, _unpluggable);
                }
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    VBD.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_qos_algorithm_type, server._qos_algorithm_type))
                {
                    VBD.set_qos_algorithm_type(session, opaqueRef, _qos_algorithm_type);
                }
                if (!Helper.AreEqual2(_qos_algorithm_params, server._qos_algorithm_params))
                {
                    VBD.set_qos_algorithm_params(session, opaqueRef, _qos_algorithm_params);
                }

                return null;
            }
        }

        public static VBD get_record(Session session, string _vbd)
        {
            return new VBD((Proxy_VBD)session.proxy.vbd_get_record(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static XenRef<VBD> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VBD>.Create(session.proxy.vbd_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static XenRef<VBD> create(Session session, VBD _record)
        {
            return XenRef<VBD>.Create(session.proxy.vbd_create(session.uuid, _record.ToProxy()).parse());
        }

        public static XenRef<Task> async_create(Session session, VBD _record)
        {
            return XenRef<Task>.Create(session.proxy.async_vbd_create(session.uuid, _record.ToProxy()).parse());
        }

        public static void destroy(Session session, string _vbd)
        {
            session.proxy.vbd_destroy(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _vbd)
        {
            return XenRef<Task>.Create(session.proxy.async_vbd_destroy(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static string get_uuid(Session session, string _vbd)
        {
            return (string)session.proxy.vbd_get_uuid(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static List<vbd_operations> get_allowed_operations(Session session, string _vbd)
        {
            return Helper.StringArrayToEnumList<vbd_operations>(session.proxy.vbd_get_allowed_operations(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static Dictionary<string, vbd_operations> get_current_operations(Session session, string _vbd)
        {
            return Maps.convert_from_proxy_string_vbd_operations(session.proxy.vbd_get_current_operations(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static XenRef<VM> get_VM(Session session, string _vbd)
        {
            return XenRef<VM>.Create(session.proxy.vbd_get_vm(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static XenRef<VDI> get_VDI(Session session, string _vbd)
        {
            return XenRef<VDI>.Create(session.proxy.vbd_get_vdi(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static string get_device(Session session, string _vbd)
        {
            return (string)session.proxy.vbd_get_device(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static string get_userdevice(Session session, string _vbd)
        {
            return (string)session.proxy.vbd_get_userdevice(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static bool get_bootable(Session session, string _vbd)
        {
            return (bool)session.proxy.vbd_get_bootable(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static vbd_mode get_mode(Session session, string _vbd)
        {
            return (vbd_mode)Helper.EnumParseDefault(typeof(vbd_mode), (string)session.proxy.vbd_get_mode(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static vbd_type get_type(Session session, string _vbd)
        {
            return (vbd_type)Helper.EnumParseDefault(typeof(vbd_type), (string)session.proxy.vbd_get_type(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static bool get_unpluggable(Session session, string _vbd)
        {
            return (bool)session.proxy.vbd_get_unpluggable(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static bool get_storage_lock(Session session, string _vbd)
        {
            return (bool)session.proxy.vbd_get_storage_lock(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static bool get_empty(Session session, string _vbd)
        {
            return (bool)session.proxy.vbd_get_empty(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static Dictionary<string, string> get_other_config(Session session, string _vbd)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vbd_get_other_config(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static bool get_currently_attached(Session session, string _vbd)
        {
            return (bool)session.proxy.vbd_get_currently_attached(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static long get_status_code(Session session, string _vbd)
        {
            return long.Parse((string)session.proxy.vbd_get_status_code(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static string get_status_detail(Session session, string _vbd)
        {
            return (string)session.proxy.vbd_get_status_detail(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static Dictionary<string, string> get_runtime_properties(Session session, string _vbd)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vbd_get_runtime_properties(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static string get_qos_algorithm_type(Session session, string _vbd)
        {
            return (string)session.proxy.vbd_get_qos_algorithm_type(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static Dictionary<string, string> get_qos_algorithm_params(Session session, string _vbd)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vbd_get_qos_algorithm_params(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static string[] get_qos_supported_algorithms(Session session, string _vbd)
        {
            return (string [])session.proxy.vbd_get_qos_supported_algorithms(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static XenRef<VBD_metrics> get_metrics(Session session, string _vbd)
        {
            return XenRef<VBD_metrics>.Create(session.proxy.vbd_get_metrics(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static void set_userdevice(Session session, string _vbd, string _userdevice)
        {
            session.proxy.vbd_set_userdevice(session.uuid, (_vbd != null) ? _vbd : "", (_userdevice != null) ? _userdevice : "").parse();
        }

        public static void set_bootable(Session session, string _vbd, bool _bootable)
        {
            session.proxy.vbd_set_bootable(session.uuid, (_vbd != null) ? _vbd : "", _bootable).parse();
        }

        public static void set_mode(Session session, string _vbd, vbd_mode _mode)
        {
            session.proxy.vbd_set_mode(session.uuid, (_vbd != null) ? _vbd : "", vbd_mode_helper.ToString(_mode)).parse();
        }

        public static void set_type(Session session, string _vbd, vbd_type _type)
        {
            session.proxy.vbd_set_type(session.uuid, (_vbd != null) ? _vbd : "", vbd_type_helper.ToString(_type)).parse();
        }

        public static void set_unpluggable(Session session, string _vbd, bool _unpluggable)
        {
            session.proxy.vbd_set_unpluggable(session.uuid, (_vbd != null) ? _vbd : "", _unpluggable).parse();
        }

        public static void set_other_config(Session session, string _vbd, Dictionary<string, string> _other_config)
        {
            session.proxy.vbd_set_other_config(session.uuid, (_vbd != null) ? _vbd : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _vbd, string _key, string _value)
        {
            session.proxy.vbd_add_to_other_config(session.uuid, (_vbd != null) ? _vbd : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _vbd, string _key)
        {
            session.proxy.vbd_remove_from_other_config(session.uuid, (_vbd != null) ? _vbd : "", (_key != null) ? _key : "").parse();
        }

        public static void set_qos_algorithm_type(Session session, string _vbd, string _algorithm_type)
        {
            session.proxy.vbd_set_qos_algorithm_type(session.uuid, (_vbd != null) ? _vbd : "", (_algorithm_type != null) ? _algorithm_type : "").parse();
        }

        public static void set_qos_algorithm_params(Session session, string _vbd, Dictionary<string, string> _algorithm_params)
        {
            session.proxy.vbd_set_qos_algorithm_params(session.uuid, (_vbd != null) ? _vbd : "", Maps.convert_to_proxy_string_string(_algorithm_params)).parse();
        }

        public static void add_to_qos_algorithm_params(Session session, string _vbd, string _key, string _value)
        {
            session.proxy.vbd_add_to_qos_algorithm_params(session.uuid, (_vbd != null) ? _vbd : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_qos_algorithm_params(Session session, string _vbd, string _key)
        {
            session.proxy.vbd_remove_from_qos_algorithm_params(session.uuid, (_vbd != null) ? _vbd : "", (_key != null) ? _key : "").parse();
        }

        public static void eject(Session session, string _vbd)
        {
            session.proxy.vbd_eject(session.uuid, (_vbd != null) ? _vbd : "").parse();
        }

        public static XenRef<Task> async_eject(Session session, string _vbd)
        {
            return XenRef<Task>.Create(session.proxy.async_vbd_eject(session.uuid, (_vbd != null) ? _vbd : "").parse());
        }

        public static void insert(Session session, string _vbd, string _vdi)
        {
            session.proxy.vbd_insert(session.uuid, (_vbd != null) ? _vbd : "", (_vdi != null) ? _vdi : "").parse();
        }

        public static XenRef<Task> async_insert(Session session, string _vbd, string _vdi)
        {
            return XenRef<Task>.Create(session.proxy.async_vbd_insert(session.uuid, (_vbd != null) ? _vbd : "", (_vdi != null) ? _vdi : "").parse());
        }

        public static void plug(Session session, string _self)
        {
            session.proxy.vbd_plug(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_plug(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vbd_plug(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void unplug(Session session, string _self)
        {
            session.proxy.vbd_unplug(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_unplug(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vbd_unplug(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void unplug_force(Session session, string _self)
        {
            session.proxy.vbd_unplug_force(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_unplug_force(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vbd_unplug_force(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void assert_attachable(Session session, string _self)
        {
            session.proxy.vbd_assert_attachable(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_assert_attachable(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vbd_assert_attachable(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static List<XenRef<VBD>> get_all(Session session)
        {
            return XenRef<VBD>.Create(session.proxy.vbd_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<VBD>, VBD> get_all_records(Session session)
        {
            return XenRef<VBD>.Create<Proxy_VBD>(session.proxy.vbd_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private List<vbd_operations> _allowed_operations;
        public virtual List<vbd_operations> allowed_operations {
             get { return _allowed_operations; }
             set { if (!Helper.AreEqual(value, _allowed_operations)) { _allowed_operations = value; Changed = true; NotifyPropertyChanged("allowed_operations"); } }
         }

        private Dictionary<string, vbd_operations> _current_operations;
        public virtual Dictionary<string, vbd_operations> current_operations {
             get { return _current_operations; }
             set { if (!Helper.AreEqual(value, _current_operations)) { _current_operations = value; Changed = true; NotifyPropertyChanged("current_operations"); } }
         }

        private XenRef<VM> _VM;
        public virtual XenRef<VM> VM {
             get { return _VM; }
             set { if (!Helper.AreEqual(value, _VM)) { _VM = value; Changed = true; NotifyPropertyChanged("VM"); } }
         }

        private XenRef<VDI> _VDI;
        public virtual XenRef<VDI> VDI {
             get { return _VDI; }
             set { if (!Helper.AreEqual(value, _VDI)) { _VDI = value; Changed = true; NotifyPropertyChanged("VDI"); } }
         }

        private string _device;
        public virtual string device {
             get { return _device; }
             set { if (!Helper.AreEqual(value, _device)) { _device = value; Changed = true; NotifyPropertyChanged("device"); } }
         }

        private string _userdevice;
        public virtual string userdevice {
             get { return _userdevice; }
             set { if (!Helper.AreEqual(value, _userdevice)) { _userdevice = value; Changed = true; NotifyPropertyChanged("userdevice"); } }
         }

        private bool _bootable;
        public virtual bool bootable {
             get { return _bootable; }
             set { if (!Helper.AreEqual(value, _bootable)) { _bootable = value; Changed = true; NotifyPropertyChanged("bootable"); } }
         }

        private vbd_mode _mode;
        public virtual vbd_mode mode {
             get { return _mode; }
             set { if (!Helper.AreEqual(value, _mode)) { _mode = value; Changed = true; NotifyPropertyChanged("mode"); } }
         }

        private vbd_type _type;
        public virtual vbd_type type {
             get { return _type; }
             set { if (!Helper.AreEqual(value, _type)) { _type = value; Changed = true; NotifyPropertyChanged("type"); } }
         }

        private bool _unpluggable;
        public virtual bool unpluggable {
             get { return _unpluggable; }
             set { if (!Helper.AreEqual(value, _unpluggable)) { _unpluggable = value; Changed = true; NotifyPropertyChanged("unpluggable"); } }
         }

        private bool _storage_lock;
        public virtual bool storage_lock {
             get { return _storage_lock; }
             set { if (!Helper.AreEqual(value, _storage_lock)) { _storage_lock = value; Changed = true; NotifyPropertyChanged("storage_lock"); } }
         }

        private bool _empty;
        public virtual bool empty {
             get { return _empty; }
             set { if (!Helper.AreEqual(value, _empty)) { _empty = value; Changed = true; NotifyPropertyChanged("empty"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }

        private bool _currently_attached;
        public virtual bool currently_attached {
             get { return _currently_attached; }
             set { if (!Helper.AreEqual(value, _currently_attached)) { _currently_attached = value; Changed = true; NotifyPropertyChanged("currently_attached"); } }
         }

        private long _status_code;
        public virtual long status_code {
             get { return _status_code; }
             set { if (!Helper.AreEqual(value, _status_code)) { _status_code = value; Changed = true; NotifyPropertyChanged("status_code"); } }
         }

        private string _status_detail;
        public virtual string status_detail {
             get { return _status_detail; }
             set { if (!Helper.AreEqual(value, _status_detail)) { _status_detail = value; Changed = true; NotifyPropertyChanged("status_detail"); } }
         }

        private Dictionary<string, string> _runtime_properties;
        public virtual Dictionary<string, string> runtime_properties {
             get { return _runtime_properties; }
             set { if (!Helper.AreEqual(value, _runtime_properties)) { _runtime_properties = value; Changed = true; NotifyPropertyChanged("runtime_properties"); } }
         }

        private string _qos_algorithm_type;
        public virtual string qos_algorithm_type {
             get { return _qos_algorithm_type; }
             set { if (!Helper.AreEqual(value, _qos_algorithm_type)) { _qos_algorithm_type = value; Changed = true; NotifyPropertyChanged("qos_algorithm_type"); } }
         }

        private Dictionary<string, string> _qos_algorithm_params;
        public virtual Dictionary<string, string> qos_algorithm_params {
             get { return _qos_algorithm_params; }
             set { if (!Helper.AreEqual(value, _qos_algorithm_params)) { _qos_algorithm_params = value; Changed = true; NotifyPropertyChanged("qos_algorithm_params"); } }
         }

        private string[] _qos_supported_algorithms;
        public virtual string[] qos_supported_algorithms {
             get { return _qos_supported_algorithms; }
             set { if (!Helper.AreEqual(value, _qos_supported_algorithms)) { _qos_supported_algorithms = value; Changed = true; NotifyPropertyChanged("qos_supported_algorithms"); } }
         }

        private XenRef<VBD_metrics> _metrics;
        public virtual XenRef<VBD_metrics> metrics {
             get { return _metrics; }
             set { if (!Helper.AreEqual(value, _metrics)) { _metrics = value; Changed = true; NotifyPropertyChanged("metrics"); } }
         }


    }
}
