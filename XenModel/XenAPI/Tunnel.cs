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
    public partial class Tunnel : XenObject<Tunnel>
    {
        public Tunnel()
        {
        }

        public Tunnel(string uuid,
            XenRef<PIF> access_PIF,
            XenRef<PIF> transport_PIF,
            Dictionary<string, string> status,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.access_PIF = access_PIF;
            this.transport_PIF = transport_PIF;
            this.status = status;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new Tunnel from a Proxy_Tunnel.
        /// </summary>
        /// <param name="proxy"></param>
        public Tunnel(Proxy_Tunnel proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Tunnel update)
        {
            uuid = update.uuid;
            access_PIF = update.access_PIF;
            transport_PIF = update.transport_PIF;
            status = update.status;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_Tunnel proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            access_PIF = proxy.access_PIF == null ? null : XenRef<PIF>.Create(proxy.access_PIF);
            transport_PIF = proxy.transport_PIF == null ? null : XenRef<PIF>.Create(proxy.transport_PIF);
            status = proxy.status == null ? null : Maps.convert_from_proxy_string_string(proxy.status);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_Tunnel ToProxy()
        {
            Proxy_Tunnel result_ = new Proxy_Tunnel();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.access_PIF = (access_PIF != null) ? access_PIF : "";
            result_.transport_PIF = (transport_PIF != null) ? transport_PIF : "";
            result_.status = Maps.convert_to_proxy_string_string(status);
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new Tunnel from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Tunnel(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            access_PIF = Marshalling.ParseRef<PIF>(table, "access_PIF");
            transport_PIF = Marshalling.ParseRef<PIF>(table, "transport_PIF");
            status = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "status"));
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(Tunnel other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._access_PIF, other._access_PIF) &&
                Helper.AreEqual2(this._transport_PIF, other._transport_PIF) &&
                Helper.AreEqual2(this._status, other._status) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, Tunnel server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_status, server._status))
                {
                    Tunnel.set_status(session, opaqueRef, _status);
                }
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    Tunnel.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }

        public static Tunnel get_record(Session session, string _tunnel)
        {
            return new Tunnel((Proxy_Tunnel)session.proxy.tunnel_get_record(session.uuid, (_tunnel != null) ? _tunnel : "").parse());
        }

        public static XenRef<Tunnel> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Tunnel>.Create(session.proxy.tunnel_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _tunnel)
        {
            return (string)session.proxy.tunnel_get_uuid(session.uuid, (_tunnel != null) ? _tunnel : "").parse();
        }

        public static XenRef<PIF> get_access_PIF(Session session, string _tunnel)
        {
            return XenRef<PIF>.Create(session.proxy.tunnel_get_access_pif(session.uuid, (_tunnel != null) ? _tunnel : "").parse());
        }

        public static XenRef<PIF> get_transport_PIF(Session session, string _tunnel)
        {
            return XenRef<PIF>.Create(session.proxy.tunnel_get_transport_pif(session.uuid, (_tunnel != null) ? _tunnel : "").parse());
        }

        public static Dictionary<string, string> get_status(Session session, string _tunnel)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.tunnel_get_status(session.uuid, (_tunnel != null) ? _tunnel : "").parse());
        }

        public static Dictionary<string, string> get_other_config(Session session, string _tunnel)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.tunnel_get_other_config(session.uuid, (_tunnel != null) ? _tunnel : "").parse());
        }

        public static void set_status(Session session, string _tunnel, Dictionary<string, string> _status)
        {
            session.proxy.tunnel_set_status(session.uuid, (_tunnel != null) ? _tunnel : "", Maps.convert_to_proxy_string_string(_status)).parse();
        }

        public static void add_to_status(Session session, string _tunnel, string _key, string _value)
        {
            session.proxy.tunnel_add_to_status(session.uuid, (_tunnel != null) ? _tunnel : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_status(Session session, string _tunnel, string _key)
        {
            session.proxy.tunnel_remove_from_status(session.uuid, (_tunnel != null) ? _tunnel : "", (_key != null) ? _key : "").parse();
        }

        public static void set_other_config(Session session, string _tunnel, Dictionary<string, string> _other_config)
        {
            session.proxy.tunnel_set_other_config(session.uuid, (_tunnel != null) ? _tunnel : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _tunnel, string _key, string _value)
        {
            session.proxy.tunnel_add_to_other_config(session.uuid, (_tunnel != null) ? _tunnel : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _tunnel, string _key)
        {
            session.proxy.tunnel_remove_from_other_config(session.uuid, (_tunnel != null) ? _tunnel : "", (_key != null) ? _key : "").parse();
        }

        public static XenRef<Tunnel> create(Session session, string _transport_pif, string _network)
        {
            return XenRef<Tunnel>.Create(session.proxy.tunnel_create(session.uuid, (_transport_pif != null) ? _transport_pif : "", (_network != null) ? _network : "").parse());
        }

        public static XenRef<Task> async_create(Session session, string _transport_pif, string _network)
        {
            return XenRef<Task>.Create(session.proxy.async_tunnel_create(session.uuid, (_transport_pif != null) ? _transport_pif : "", (_network != null) ? _network : "").parse());
        }

        public static void destroy(Session session, string _self)
        {
            session.proxy.tunnel_destroy(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_tunnel_destroy(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static List<XenRef<Tunnel>> get_all(Session session)
        {
            return XenRef<Tunnel>.Create(session.proxy.tunnel_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<Tunnel>, Tunnel> get_all_records(Session session)
        {
            return XenRef<Tunnel>.Create<Proxy_Tunnel>(session.proxy.tunnel_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private XenRef<PIF> _access_PIF;
        public virtual XenRef<PIF> access_PIF {
             get { return _access_PIF; }
             set { if (!Helper.AreEqual(value, _access_PIF)) { _access_PIF = value; Changed = true; NotifyPropertyChanged("access_PIF"); } }
         }

        private XenRef<PIF> _transport_PIF;
        public virtual XenRef<PIF> transport_PIF {
             get { return _transport_PIF; }
             set { if (!Helper.AreEqual(value, _transport_PIF)) { _transport_PIF = value; Changed = true; NotifyPropertyChanged("transport_PIF"); } }
         }

        private Dictionary<string, string> _status;
        public virtual Dictionary<string, string> status {
             get { return _status; }
             set { if (!Helper.AreEqual(value, _status)) { _status = value; Changed = true; NotifyPropertyChanged("status"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }


    }
}
