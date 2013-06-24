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
    public partial class Network : XenObject<Network>
    {
        public Network()
        {
        }

        public Network(string uuid,
            string name_label,
            string name_description,
            List<network_operations> allowed_operations,
            Dictionary<string, network_operations> current_operations,
            List<XenRef<VIF>> VIFs,
            List<XenRef<PIF>> PIFs,
            long MTU,
            Dictionary<string, string> other_config,
            string bridge,
            Dictionary<string, XenRef<Blob>> blobs,
            string[] tags,
            network_default_locking_mode default_locking_mode)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.VIFs = VIFs;
            this.PIFs = PIFs;
            this.MTU = MTU;
            this.other_config = other_config;
            this.bridge = bridge;
            this.blobs = blobs;
            this.tags = tags;
            this.default_locking_mode = default_locking_mode;
        }

        /// <summary>
        /// Creates a new Network from a Proxy_Network.
        /// </summary>
        /// <param name="proxy"></param>
        public Network(Proxy_Network proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Network update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            allowed_operations = update.allowed_operations;
            current_operations = update.current_operations;
            VIFs = update.VIFs;
            PIFs = update.PIFs;
            MTU = update.MTU;
            other_config = update.other_config;
            bridge = update.bridge;
            blobs = update.blobs;
            tags = update.tags;
            default_locking_mode = update.default_locking_mode;
        }

        internal void UpdateFromProxy(Proxy_Network proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            allowed_operations = proxy.allowed_operations == null ? null : Helper.StringArrayToEnumList<network_operations>(proxy.allowed_operations);
            current_operations = proxy.current_operations == null ? null : Maps.convert_from_proxy_string_network_operations(proxy.current_operations);
            VIFs = proxy.VIFs == null ? null : XenRef<VIF>.Create(proxy.VIFs);
            PIFs = proxy.PIFs == null ? null : XenRef<PIF>.Create(proxy.PIFs);
            MTU = proxy.MTU == null ? 0 : long.Parse((string)proxy.MTU);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            bridge = proxy.bridge == null ? null : (string)proxy.bridge;
            blobs = proxy.blobs == null ? null : Maps.convert_from_proxy_string_XenRefBlob(proxy.blobs);
            tags = proxy.tags == null ? new string[] {} : (string [])proxy.tags;
            default_locking_mode = proxy.default_locking_mode == null ? (network_default_locking_mode) 0 : (network_default_locking_mode)Helper.EnumParseDefault(typeof(network_default_locking_mode), (string)proxy.default_locking_mode);
        }

        public Proxy_Network ToProxy()
        {
            Proxy_Network result_ = new Proxy_Network();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.allowed_operations = (allowed_operations != null) ? Helper.ObjectListToStringArray(allowed_operations) : new string[] {};
            result_.current_operations = Maps.convert_to_proxy_string_network_operations(current_operations);
            result_.VIFs = (VIFs != null) ? Helper.RefListToStringArray(VIFs) : new string[] {};
            result_.PIFs = (PIFs != null) ? Helper.RefListToStringArray(PIFs) : new string[] {};
            result_.MTU = MTU.ToString();
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.bridge = (bridge != null) ? bridge : "";
            result_.blobs = Maps.convert_to_proxy_string_XenRefBlob(blobs);
            result_.tags = tags;
            result_.default_locking_mode = network_default_locking_mode_helper.ToString(default_locking_mode);
            return result_;
        }

        /// <summary>
        /// Creates a new Network from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Network(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            allowed_operations = Helper.StringArrayToEnumList<network_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            current_operations = Maps.convert_from_proxy_string_network_operations(Marshalling.ParseHashTable(table, "current_operations"));
            VIFs = Marshalling.ParseSetRef<VIF>(table, "VIFs");
            PIFs = Marshalling.ParseSetRef<PIF>(table, "PIFs");
            MTU = Marshalling.ParseLong(table, "MTU");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            bridge = Marshalling.ParseString(table, "bridge");
            blobs = Maps.convert_from_proxy_string_XenRefBlob(Marshalling.ParseHashTable(table, "blobs"));
            tags = Marshalling.ParseStringArray(table, "tags");
            default_locking_mode = (network_default_locking_mode)Helper.EnumParseDefault(typeof(network_default_locking_mode), Marshalling.ParseString(table, "default_locking_mode"));
        }

        public bool DeepEquals(Network other, bool ignoreCurrentOperations)
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
                Helper.AreEqual2(this._VIFs, other._VIFs) &&
                Helper.AreEqual2(this._PIFs, other._PIFs) &&
                Helper.AreEqual2(this._MTU, other._MTU) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._bridge, other._bridge) &&
                Helper.AreEqual2(this._blobs, other._blobs) &&
                Helper.AreEqual2(this._tags, other._tags) &&
                Helper.AreEqual2(this._default_locking_mode, other._default_locking_mode);
        }

        public override string SaveChanges(Session session, string opaqueRef, Network server)
        {
            if (opaqueRef == null)
            {
                Proxy_Network p = this.ToProxy();
                return session.proxy.network_create(session.uuid, p).parse();
            }
            else
            {
                if (!Helper.AreEqual2(_name_label, server._name_label))
                {
                    Network.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    Network.set_name_description(session, opaqueRef, _name_description);
                }
                if (!Helper.AreEqual2(_MTU, server._MTU))
                {
                    Network.set_MTU(session, opaqueRef, _MTU);
                }
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    Network.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_tags, server._tags))
                {
                    Network.set_tags(session, opaqueRef, _tags);
                }

                return null;
            }
        }

        public static Network get_record(Session session, string _network)
        {
            return new Network((Proxy_Network)session.proxy.network_get_record(session.uuid, (_network != null) ? _network : "").parse());
        }

        public static XenRef<Network> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Network>.Create(session.proxy.network_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static XenRef<Network> create(Session session, Network _record)
        {
            return XenRef<Network>.Create(session.proxy.network_create(session.uuid, _record.ToProxy()).parse());
        }

        public static XenRef<Task> async_create(Session session, Network _record)
        {
            return XenRef<Task>.Create(session.proxy.async_network_create(session.uuid, _record.ToProxy()).parse());
        }

        public static void destroy(Session session, string _network)
        {
            session.proxy.network_destroy(session.uuid, (_network != null) ? _network : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _network)
        {
            return XenRef<Task>.Create(session.proxy.async_network_destroy(session.uuid, (_network != null) ? _network : "").parse());
        }

        public static List<XenRef<Network>> get_by_name_label(Session session, string _label)
        {
            return XenRef<Network>.Create(session.proxy.network_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        public static string get_uuid(Session session, string _network)
        {
            return (string)session.proxy.network_get_uuid(session.uuid, (_network != null) ? _network : "").parse();
        }

        public static string get_name_label(Session session, string _network)
        {
            return (string)session.proxy.network_get_name_label(session.uuid, (_network != null) ? _network : "").parse();
        }

        public static string get_name_description(Session session, string _network)
        {
            return (string)session.proxy.network_get_name_description(session.uuid, (_network != null) ? _network : "").parse();
        }

        public static List<network_operations> get_allowed_operations(Session session, string _network)
        {
            return Helper.StringArrayToEnumList<network_operations>(session.proxy.network_get_allowed_operations(session.uuid, (_network != null) ? _network : "").parse());
        }

        public static Dictionary<string, network_operations> get_current_operations(Session session, string _network)
        {
            return Maps.convert_from_proxy_string_network_operations(session.proxy.network_get_current_operations(session.uuid, (_network != null) ? _network : "").parse());
        }

        public static List<XenRef<VIF>> get_VIFs(Session session, string _network)
        {
            return XenRef<VIF>.Create(session.proxy.network_get_vifs(session.uuid, (_network != null) ? _network : "").parse());
        }

        public static List<XenRef<PIF>> get_PIFs(Session session, string _network)
        {
            return XenRef<PIF>.Create(session.proxy.network_get_pifs(session.uuid, (_network != null) ? _network : "").parse());
        }

        public static long get_MTU(Session session, string _network)
        {
            return long.Parse((string)session.proxy.network_get_mtu(session.uuid, (_network != null) ? _network : "").parse());
        }

        public static Dictionary<string, string> get_other_config(Session session, string _network)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.network_get_other_config(session.uuid, (_network != null) ? _network : "").parse());
        }

        public static string get_bridge(Session session, string _network)
        {
            return (string)session.proxy.network_get_bridge(session.uuid, (_network != null) ? _network : "").parse();
        }

        public static Dictionary<string, XenRef<Blob>> get_blobs(Session session, string _network)
        {
            return Maps.convert_from_proxy_string_XenRefBlob(session.proxy.network_get_blobs(session.uuid, (_network != null) ? _network : "").parse());
        }

        public static string[] get_tags(Session session, string _network)
        {
            return (string [])session.proxy.network_get_tags(session.uuid, (_network != null) ? _network : "").parse();
        }

        public static network_default_locking_mode get_default_locking_mode(Session session, string _network)
        {
            return (network_default_locking_mode)Helper.EnumParseDefault(typeof(network_default_locking_mode), (string)session.proxy.network_get_default_locking_mode(session.uuid, (_network != null) ? _network : "").parse());
        }

        public static void set_name_label(Session session, string _network, string _label)
        {
            session.proxy.network_set_name_label(session.uuid, (_network != null) ? _network : "", (_label != null) ? _label : "").parse();
        }

        public static void set_name_description(Session session, string _network, string _description)
        {
            session.proxy.network_set_name_description(session.uuid, (_network != null) ? _network : "", (_description != null) ? _description : "").parse();
        }

        public static void set_MTU(Session session, string _network, long _mtu)
        {
            session.proxy.network_set_mtu(session.uuid, (_network != null) ? _network : "", _mtu.ToString()).parse();
        }

        public static void set_other_config(Session session, string _network, Dictionary<string, string> _other_config)
        {
            session.proxy.network_set_other_config(session.uuid, (_network != null) ? _network : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _network, string _key, string _value)
        {
            session.proxy.network_add_to_other_config(session.uuid, (_network != null) ? _network : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _network, string _key)
        {
            session.proxy.network_remove_from_other_config(session.uuid, (_network != null) ? _network : "", (_key != null) ? _key : "").parse();
        }

        public static void set_tags(Session session, string _network, string[] _tags)
        {
            session.proxy.network_set_tags(session.uuid, (_network != null) ? _network : "", _tags).parse();
        }

        public static void add_tags(Session session, string _network, string _value)
        {
            session.proxy.network_add_tags(session.uuid, (_network != null) ? _network : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_tags(Session session, string _network, string _value)
        {
            session.proxy.network_remove_tags(session.uuid, (_network != null) ? _network : "", (_value != null) ? _value : "").parse();
        }

        public static XenRef<Blob> create_new_blob(Session session, string _network, string _name, string _mime_type, bool _public)
        {
            return XenRef<Blob>.Create(session.proxy.network_create_new_blob(session.uuid, (_network != null) ? _network : "", (_name != null) ? _name : "", (_mime_type != null) ? _mime_type : "", _public).parse());
        }

        public static XenRef<Task> async_create_new_blob(Session session, string _network, string _name, string _mime_type, bool _public)
        {
            return XenRef<Task>.Create(session.proxy.async_network_create_new_blob(session.uuid, (_network != null) ? _network : "", (_name != null) ? _name : "", (_mime_type != null) ? _mime_type : "", _public).parse());
        }

        public static void set_default_locking_mode(Session session, string _network, network_default_locking_mode _value)
        {
            session.proxy.network_set_default_locking_mode(session.uuid, (_network != null) ? _network : "", network_default_locking_mode_helper.ToString(_value)).parse();
        }

        public static XenRef<Task> async_set_default_locking_mode(Session session, string _network, network_default_locking_mode _value)
        {
            return XenRef<Task>.Create(session.proxy.async_network_set_default_locking_mode(session.uuid, (_network != null) ? _network : "", network_default_locking_mode_helper.ToString(_value)).parse());
        }

        public static List<XenRef<Network>> get_all(Session session)
        {
            return XenRef<Network>.Create(session.proxy.network_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<Network>, Network> get_all_records(Session session)
        {
            return XenRef<Network>.Create<Proxy_Network>(session.proxy.network_get_all_records(session.uuid).parse());
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

        private List<network_operations> _allowed_operations;
        public virtual List<network_operations> allowed_operations {
             get { return _allowed_operations; }
             set { if (!Helper.AreEqual(value, _allowed_operations)) { _allowed_operations = value; Changed = true; NotifyPropertyChanged("allowed_operations"); } }
         }

        private Dictionary<string, network_operations> _current_operations;
        public virtual Dictionary<string, network_operations> current_operations {
             get { return _current_operations; }
             set { if (!Helper.AreEqual(value, _current_operations)) { _current_operations = value; Changed = true; NotifyPropertyChanged("current_operations"); } }
         }

        private List<XenRef<VIF>> _VIFs;
        public virtual List<XenRef<VIF>> VIFs {
             get { return _VIFs; }
             set { if (!Helper.AreEqual(value, _VIFs)) { _VIFs = value; Changed = true; NotifyPropertyChanged("VIFs"); } }
         }

        private List<XenRef<PIF>> _PIFs;
        public virtual List<XenRef<PIF>> PIFs {
             get { return _PIFs; }
             set { if (!Helper.AreEqual(value, _PIFs)) { _PIFs = value; Changed = true; NotifyPropertyChanged("PIFs"); } }
         }

        private long _MTU;
        public virtual long MTU {
             get { return _MTU; }
             set { if (!Helper.AreEqual(value, _MTU)) { _MTU = value; Changed = true; NotifyPropertyChanged("MTU"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }

        private string _bridge;
        public virtual string bridge {
             get { return _bridge; }
             set { if (!Helper.AreEqual(value, _bridge)) { _bridge = value; Changed = true; NotifyPropertyChanged("bridge"); } }
         }

        private Dictionary<string, XenRef<Blob>> _blobs;
        public virtual Dictionary<string, XenRef<Blob>> blobs {
             get { return _blobs; }
             set { if (!Helper.AreEqual(value, _blobs)) { _blobs = value; Changed = true; NotifyPropertyChanged("blobs"); } }
         }

        private string[] _tags;
        public virtual string[] tags {
             get { return _tags; }
             set { if (!Helper.AreEqual(value, _tags)) { _tags = value; Changed = true; NotifyPropertyChanged("tags"); } }
         }

        private network_default_locking_mode _default_locking_mode;
        public virtual network_default_locking_mode default_locking_mode {
             get { return _default_locking_mode; }
             set { if (!Helper.AreEqual(value, _default_locking_mode)) { _default_locking_mode = value; Changed = true; NotifyPropertyChanged("default_locking_mode"); } }
         }


    }
}
