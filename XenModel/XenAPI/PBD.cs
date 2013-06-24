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
    public partial class PBD : XenObject<PBD>
    {
        public PBD()
        {
        }

        public PBD(string uuid,
            XenRef<Host> host,
            XenRef<SR> SR,
            Dictionary<string, string> device_config,
            bool currently_attached,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.host = host;
            this.SR = SR;
            this.device_config = device_config;
            this.currently_attached = currently_attached;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new PBD from a Proxy_PBD.
        /// </summary>
        /// <param name="proxy"></param>
        public PBD(Proxy_PBD proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(PBD update)
        {
            uuid = update.uuid;
            host = update.host;
            SR = update.SR;
            device_config = update.device_config;
            currently_attached = update.currently_attached;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_PBD proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            SR = proxy.SR == null ? null : XenRef<SR>.Create(proxy.SR);
            device_config = proxy.device_config == null ? null : Maps.convert_from_proxy_string_string(proxy.device_config);
            currently_attached = (bool)proxy.currently_attached;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_PBD ToProxy()
        {
            Proxy_PBD result_ = new Proxy_PBD();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.host = (host != null) ? host : "";
            result_.SR = (SR != null) ? SR : "";
            result_.device_config = Maps.convert_to_proxy_string_string(device_config);
            result_.currently_attached = currently_attached;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new PBD from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public PBD(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            host = Marshalling.ParseRef<Host>(table, "host");
            SR = Marshalling.ParseRef<SR>(table, "SR");
            device_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "device_config"));
            currently_attached = Marshalling.ParseBool(table, "currently_attached");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(PBD other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._SR, other._SR) &&
                Helper.AreEqual2(this._device_config, other._device_config) &&
                Helper.AreEqual2(this._currently_attached, other._currently_attached) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, PBD server)
        {
            if (opaqueRef == null)
            {
                Proxy_PBD p = this.ToProxy();
                return session.proxy.pbd_create(session.uuid, p).parse();
            }
            else
            {
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    PBD.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_device_config, server._device_config))
                {
                    PBD.set_device_config(session, opaqueRef, _device_config);
                }

                return null;
            }
        }

        public static PBD get_record(Session session, string _pbd)
        {
            return new PBD((Proxy_PBD)session.proxy.pbd_get_record(session.uuid, (_pbd != null) ? _pbd : "").parse());
        }

        public static XenRef<PBD> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<PBD>.Create(session.proxy.pbd_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static XenRef<PBD> create(Session session, PBD _record)
        {
            return XenRef<PBD>.Create(session.proxy.pbd_create(session.uuid, _record.ToProxy()).parse());
        }

        public static XenRef<Task> async_create(Session session, PBD _record)
        {
            return XenRef<Task>.Create(session.proxy.async_pbd_create(session.uuid, _record.ToProxy()).parse());
        }

        public static void destroy(Session session, string _pbd)
        {
            session.proxy.pbd_destroy(session.uuid, (_pbd != null) ? _pbd : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _pbd)
        {
            return XenRef<Task>.Create(session.proxy.async_pbd_destroy(session.uuid, (_pbd != null) ? _pbd : "").parse());
        }

        public static string get_uuid(Session session, string _pbd)
        {
            return (string)session.proxy.pbd_get_uuid(session.uuid, (_pbd != null) ? _pbd : "").parse();
        }

        public static XenRef<Host> get_host(Session session, string _pbd)
        {
            return XenRef<Host>.Create(session.proxy.pbd_get_host(session.uuid, (_pbd != null) ? _pbd : "").parse());
        }

        public static XenRef<SR> get_SR(Session session, string _pbd)
        {
            return XenRef<SR>.Create(session.proxy.pbd_get_sr(session.uuid, (_pbd != null) ? _pbd : "").parse());
        }

        public static Dictionary<string, string> get_device_config(Session session, string _pbd)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pbd_get_device_config(session.uuid, (_pbd != null) ? _pbd : "").parse());
        }

        public static bool get_currently_attached(Session session, string _pbd)
        {
            return (bool)session.proxy.pbd_get_currently_attached(session.uuid, (_pbd != null) ? _pbd : "").parse();
        }

        public static Dictionary<string, string> get_other_config(Session session, string _pbd)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pbd_get_other_config(session.uuid, (_pbd != null) ? _pbd : "").parse());
        }

        public static void set_other_config(Session session, string _pbd, Dictionary<string, string> _other_config)
        {
            session.proxy.pbd_set_other_config(session.uuid, (_pbd != null) ? _pbd : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _pbd, string _key, string _value)
        {
            session.proxy.pbd_add_to_other_config(session.uuid, (_pbd != null) ? _pbd : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _pbd, string _key)
        {
            session.proxy.pbd_remove_from_other_config(session.uuid, (_pbd != null) ? _pbd : "", (_key != null) ? _key : "").parse();
        }

        public static void plug(Session session, string _self)
        {
            session.proxy.pbd_plug(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_plug(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_pbd_plug(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void unplug(Session session, string _self)
        {
            session.proxy.pbd_unplug(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_unplug(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_pbd_unplug(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void set_device_config(Session session, string _self, Dictionary<string, string> _value)
        {
            session.proxy.pbd_set_device_config(session.uuid, (_self != null) ? _self : "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        public static XenRef<Task> async_set_device_config(Session session, string _self, Dictionary<string, string> _value)
        {
            return XenRef<Task>.Create(session.proxy.async_pbd_set_device_config(session.uuid, (_self != null) ? _self : "", Maps.convert_to_proxy_string_string(_value)).parse());
        }

        public static List<XenRef<PBD>> get_all(Session session)
        {
            return XenRef<PBD>.Create(session.proxy.pbd_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<PBD>, PBD> get_all_records(Session session)
        {
            return XenRef<PBD>.Create<Proxy_PBD>(session.proxy.pbd_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private XenRef<Host> _host;
        public virtual XenRef<Host> host {
             get { return _host; }
             set { if (!Helper.AreEqual(value, _host)) { _host = value; Changed = true; NotifyPropertyChanged("host"); } }
         }

        private XenRef<SR> _SR;
        public virtual XenRef<SR> SR {
             get { return _SR; }
             set { if (!Helper.AreEqual(value, _SR)) { _SR = value; Changed = true; NotifyPropertyChanged("SR"); } }
         }

        private Dictionary<string, string> _device_config;
        public virtual Dictionary<string, string> device_config {
             get { return _device_config; }
             set { if (!Helper.AreEqual(value, _device_config)) { _device_config = value; Changed = true; NotifyPropertyChanged("device_config"); } }
         }

        private bool _currently_attached;
        public virtual bool currently_attached {
             get { return _currently_attached; }
             set { if (!Helper.AreEqual(value, _currently_attached)) { _currently_attached = value; Changed = true; NotifyPropertyChanged("currently_attached"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }


    }
}
