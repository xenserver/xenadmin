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
    public partial class Bond : XenObject<Bond>
    {
        public Bond()
        {
        }

        public Bond(string uuid,
            XenRef<PIF> master,
            List<XenRef<PIF>> slaves,
            Dictionary<string, string> other_config,
            XenRef<PIF> primary_slave,
            bond_mode mode,
            Dictionary<string, string> properties,
            long links_up)
        {
            this.uuid = uuid;
            this.master = master;
            this.slaves = slaves;
            this.other_config = other_config;
            this.primary_slave = primary_slave;
            this.mode = mode;
            this.properties = properties;
            this.links_up = links_up;
        }

        /// <summary>
        /// Creates a new Bond from a Proxy_Bond.
        /// </summary>
        /// <param name="proxy"></param>
        public Bond(Proxy_Bond proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Bond update)
        {
            uuid = update.uuid;
            master = update.master;
            slaves = update.slaves;
            other_config = update.other_config;
            primary_slave = update.primary_slave;
            mode = update.mode;
            properties = update.properties;
            links_up = update.links_up;
        }

        internal void UpdateFromProxy(Proxy_Bond proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            master = proxy.master == null ? null : XenRef<PIF>.Create(proxy.master);
            slaves = proxy.slaves == null ? null : XenRef<PIF>.Create(proxy.slaves);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            primary_slave = proxy.primary_slave == null ? null : XenRef<PIF>.Create(proxy.primary_slave);
            mode = proxy.mode == null ? (bond_mode) 0 : (bond_mode)Helper.EnumParseDefault(typeof(bond_mode), (string)proxy.mode);
            properties = proxy.properties == null ? null : Maps.convert_from_proxy_string_string(proxy.properties);
            links_up = proxy.links_up == null ? 0 : long.Parse((string)proxy.links_up);
        }

        public Proxy_Bond ToProxy()
        {
            Proxy_Bond result_ = new Proxy_Bond();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.master = (master != null) ? master : "";
            result_.slaves = (slaves != null) ? Helper.RefListToStringArray(slaves) : new string[] {};
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.primary_slave = (primary_slave != null) ? primary_slave : "";
            result_.mode = bond_mode_helper.ToString(mode);
            result_.properties = Maps.convert_to_proxy_string_string(properties);
            result_.links_up = links_up.ToString();
            return result_;
        }

        /// <summary>
        /// Creates a new Bond from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Bond(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            master = Marshalling.ParseRef<PIF>(table, "master");
            slaves = Marshalling.ParseSetRef<PIF>(table, "slaves");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            primary_slave = Marshalling.ParseRef<PIF>(table, "primary_slave");
            mode = (bond_mode)Helper.EnumParseDefault(typeof(bond_mode), Marshalling.ParseString(table, "mode"));
            properties = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "properties"));
            links_up = Marshalling.ParseLong(table, "links_up");
        }

        public bool DeepEquals(Bond other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._master, other._master) &&
                Helper.AreEqual2(this._slaves, other._slaves) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._primary_slave, other._primary_slave) &&
                Helper.AreEqual2(this._mode, other._mode) &&
                Helper.AreEqual2(this._properties, other._properties) &&
                Helper.AreEqual2(this._links_up, other._links_up);
        }

        public override string SaveChanges(Session session, string opaqueRef, Bond server)
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
                    Bond.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }

        public static Bond get_record(Session session, string _bond)
        {
            return new Bond((Proxy_Bond)session.proxy.bond_get_record(session.uuid, (_bond != null) ? _bond : "").parse());
        }

        public static XenRef<Bond> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Bond>.Create(session.proxy.bond_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _bond)
        {
            return (string)session.proxy.bond_get_uuid(session.uuid, (_bond != null) ? _bond : "").parse();
        }

        public static XenRef<PIF> get_master(Session session, string _bond)
        {
            return XenRef<PIF>.Create(session.proxy.bond_get_master(session.uuid, (_bond != null) ? _bond : "").parse());
        }

        public static List<XenRef<PIF>> get_slaves(Session session, string _bond)
        {
            return XenRef<PIF>.Create(session.proxy.bond_get_slaves(session.uuid, (_bond != null) ? _bond : "").parse());
        }

        public static Dictionary<string, string> get_other_config(Session session, string _bond)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.bond_get_other_config(session.uuid, (_bond != null) ? _bond : "").parse());
        }

        public static XenRef<PIF> get_primary_slave(Session session, string _bond)
        {
            return XenRef<PIF>.Create(session.proxy.bond_get_primary_slave(session.uuid, (_bond != null) ? _bond : "").parse());
        }

        public static bond_mode get_mode(Session session, string _bond)
        {
            return (bond_mode)Helper.EnumParseDefault(typeof(bond_mode), (string)session.proxy.bond_get_mode(session.uuid, (_bond != null) ? _bond : "").parse());
        }

        public static Dictionary<string, string> get_properties(Session session, string _bond)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.bond_get_properties(session.uuid, (_bond != null) ? _bond : "").parse());
        }

        public static long get_links_up(Session session, string _bond)
        {
            return long.Parse((string)session.proxy.bond_get_links_up(session.uuid, (_bond != null) ? _bond : "").parse());
        }

        public static void set_other_config(Session session, string _bond, Dictionary<string, string> _other_config)
        {
            session.proxy.bond_set_other_config(session.uuid, (_bond != null) ? _bond : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _bond, string _key, string _value)
        {
            session.proxy.bond_add_to_other_config(session.uuid, (_bond != null) ? _bond : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _bond, string _key)
        {
            session.proxy.bond_remove_from_other_config(session.uuid, (_bond != null) ? _bond : "", (_key != null) ? _key : "").parse();
        }

        public static XenRef<Bond> create(Session session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode, Dictionary<string, string> _properties)
        {
            return XenRef<Bond>.Create(session.proxy.bond_create(session.uuid, (_network != null) ? _network : "", (_members != null) ? Helper.RefListToStringArray(_members) : new string[] {}, (_mac != null) ? _mac : "", bond_mode_helper.ToString(_mode), Maps.convert_to_proxy_string_string(_properties)).parse());
        }

        public static XenRef<Task> async_create(Session session, string _network, List<XenRef<PIF>> _members, string _mac, bond_mode _mode, Dictionary<string, string> _properties)
        {
            return XenRef<Task>.Create(session.proxy.async_bond_create(session.uuid, (_network != null) ? _network : "", (_members != null) ? Helper.RefListToStringArray(_members) : new string[] {}, (_mac != null) ? _mac : "", bond_mode_helper.ToString(_mode), Maps.convert_to_proxy_string_string(_properties)).parse());
        }

        public static void destroy(Session session, string _self)
        {
            session.proxy.bond_destroy(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_bond_destroy(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void set_mode(Session session, string _self, bond_mode _value)
        {
            session.proxy.bond_set_mode(session.uuid, (_self != null) ? _self : "", bond_mode_helper.ToString(_value)).parse();
        }

        public static XenRef<Task> async_set_mode(Session session, string _self, bond_mode _value)
        {
            return XenRef<Task>.Create(session.proxy.async_bond_set_mode(session.uuid, (_self != null) ? _self : "", bond_mode_helper.ToString(_value)).parse());
        }

        public static void set_property(Session session, string _self, string _name, string _value)
        {
            session.proxy.bond_set_property(session.uuid, (_self != null) ? _self : "", (_name != null) ? _name : "", (_value != null) ? _value : "").parse();
        }

        public static XenRef<Task> async_set_property(Session session, string _self, string _name, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_bond_set_property(session.uuid, (_self != null) ? _self : "", (_name != null) ? _name : "", (_value != null) ? _value : "").parse());
        }

        public static List<XenRef<Bond>> get_all(Session session)
        {
            return XenRef<Bond>.Create(session.proxy.bond_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<Bond>, Bond> get_all_records(Session session)
        {
            return XenRef<Bond>.Create<Proxy_Bond>(session.proxy.bond_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private XenRef<PIF> _master;
        public virtual XenRef<PIF> master {
             get { return _master; }
             set { if (!Helper.AreEqual(value, _master)) { _master = value; Changed = true; NotifyPropertyChanged("master"); } }
         }

        private List<XenRef<PIF>> _slaves;
        public virtual List<XenRef<PIF>> slaves {
             get { return _slaves; }
             set { if (!Helper.AreEqual(value, _slaves)) { _slaves = value; Changed = true; NotifyPropertyChanged("slaves"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }

        private XenRef<PIF> _primary_slave;
        public virtual XenRef<PIF> primary_slave {
             get { return _primary_slave; }
             set { if (!Helper.AreEqual(value, _primary_slave)) { _primary_slave = value; Changed = true; NotifyPropertyChanged("primary_slave"); } }
         }

        private bond_mode _mode;
        public virtual bond_mode mode {
             get { return _mode; }
             set { if (!Helper.AreEqual(value, _mode)) { _mode = value; Changed = true; NotifyPropertyChanged("mode"); } }
         }

        private Dictionary<string, string> _properties;
        public virtual Dictionary<string, string> properties {
             get { return _properties; }
             set { if (!Helper.AreEqual(value, _properties)) { _properties = value; Changed = true; NotifyPropertyChanged("properties"); } }
         }

        private long _links_up;
        public virtual long links_up {
             get { return _links_up; }
             set { if (!Helper.AreEqual(value, _links_up)) { _links_up = value; Changed = true; NotifyPropertyChanged("links_up"); } }
         }


    }
}
