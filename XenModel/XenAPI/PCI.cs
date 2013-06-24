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
    public partial class PCI : XenObject<PCI>
    {
        public PCI()
        {
        }

        public PCI(string uuid,
            string class_name,
            string vendor_name,
            string device_name,
            XenRef<Host> host,
            string pci_id,
            List<XenRef<PCI>> dependencies,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.class_name = class_name;
            this.vendor_name = vendor_name;
            this.device_name = device_name;
            this.host = host;
            this.pci_id = pci_id;
            this.dependencies = dependencies;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new PCI from a Proxy_PCI.
        /// </summary>
        /// <param name="proxy"></param>
        public PCI(Proxy_PCI proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(PCI update)
        {
            uuid = update.uuid;
            class_name = update.class_name;
            vendor_name = update.vendor_name;
            device_name = update.device_name;
            host = update.host;
            pci_id = update.pci_id;
            dependencies = update.dependencies;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_PCI proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            class_name = proxy.class_name == null ? null : (string)proxy.class_name;
            vendor_name = proxy.vendor_name == null ? null : (string)proxy.vendor_name;
            device_name = proxy.device_name == null ? null : (string)proxy.device_name;
            host = proxy.host == null ? null : XenRef<Host>.Create(proxy.host);
            pci_id = proxy.pci_id == null ? null : (string)proxy.pci_id;
            dependencies = proxy.dependencies == null ? null : XenRef<PCI>.Create(proxy.dependencies);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_PCI ToProxy()
        {
            Proxy_PCI result_ = new Proxy_PCI();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.class_name = (class_name != null) ? class_name : "";
            result_.vendor_name = (vendor_name != null) ? vendor_name : "";
            result_.device_name = (device_name != null) ? device_name : "";
            result_.host = (host != null) ? host : "";
            result_.pci_id = (pci_id != null) ? pci_id : "";
            result_.dependencies = (dependencies != null) ? Helper.RefListToStringArray(dependencies) : new string[] {};
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new PCI from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public PCI(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            class_name = Marshalling.ParseString(table, "class_name");
            vendor_name = Marshalling.ParseString(table, "vendor_name");
            device_name = Marshalling.ParseString(table, "device_name");
            host = Marshalling.ParseRef<Host>(table, "host");
            pci_id = Marshalling.ParseString(table, "pci_id");
            dependencies = Marshalling.ParseSetRef<PCI>(table, "dependencies");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(PCI other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._class_name, other._class_name) &&
                Helper.AreEqual2(this._vendor_name, other._vendor_name) &&
                Helper.AreEqual2(this._device_name, other._device_name) &&
                Helper.AreEqual2(this._host, other._host) &&
                Helper.AreEqual2(this._pci_id, other._pci_id) &&
                Helper.AreEqual2(this._dependencies, other._dependencies) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, PCI server)
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
                    PCI.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }

        public static PCI get_record(Session session, string _pci)
        {
            return new PCI((Proxy_PCI)session.proxy.pci_get_record(session.uuid, (_pci != null) ? _pci : "").parse());
        }

        public static XenRef<PCI> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<PCI>.Create(session.proxy.pci_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _pci)
        {
            return (string)session.proxy.pci_get_uuid(session.uuid, (_pci != null) ? _pci : "").parse();
        }

        public static string get_class_name(Session session, string _pci)
        {
            return (string)session.proxy.pci_get_class_name(session.uuid, (_pci != null) ? _pci : "").parse();
        }

        public static string get_vendor_name(Session session, string _pci)
        {
            return (string)session.proxy.pci_get_vendor_name(session.uuid, (_pci != null) ? _pci : "").parse();
        }

        public static string get_device_name(Session session, string _pci)
        {
            return (string)session.proxy.pci_get_device_name(session.uuid, (_pci != null) ? _pci : "").parse();
        }

        public static XenRef<Host> get_host(Session session, string _pci)
        {
            return XenRef<Host>.Create(session.proxy.pci_get_host(session.uuid, (_pci != null) ? _pci : "").parse());
        }

        public static string get_pci_id(Session session, string _pci)
        {
            return (string)session.proxy.pci_get_pci_id(session.uuid, (_pci != null) ? _pci : "").parse();
        }

        public static List<XenRef<PCI>> get_dependencies(Session session, string _pci)
        {
            return XenRef<PCI>.Create(session.proxy.pci_get_dependencies(session.uuid, (_pci != null) ? _pci : "").parse());
        }

        public static Dictionary<string, string> get_other_config(Session session, string _pci)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pci_get_other_config(session.uuid, (_pci != null) ? _pci : "").parse());
        }

        public static void set_other_config(Session session, string _pci, Dictionary<string, string> _other_config)
        {
            session.proxy.pci_set_other_config(session.uuid, (_pci != null) ? _pci : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _pci, string _key, string _value)
        {
            session.proxy.pci_add_to_other_config(session.uuid, (_pci != null) ? _pci : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _pci, string _key)
        {
            session.proxy.pci_remove_from_other_config(session.uuid, (_pci != null) ? _pci : "", (_key != null) ? _key : "").parse();
        }

        public static List<XenRef<PCI>> get_all(Session session)
        {
            return XenRef<PCI>.Create(session.proxy.pci_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<PCI>, PCI> get_all_records(Session session)
        {
            return XenRef<PCI>.Create<Proxy_PCI>(session.proxy.pci_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private string _class_name;
        public virtual string class_name {
             get { return _class_name; }
             set { if (!Helper.AreEqual(value, _class_name)) { _class_name = value; Changed = true; NotifyPropertyChanged("class_name"); } }
         }

        private string _vendor_name;
        public virtual string vendor_name {
             get { return _vendor_name; }
             set { if (!Helper.AreEqual(value, _vendor_name)) { _vendor_name = value; Changed = true; NotifyPropertyChanged("vendor_name"); } }
         }

        private string _device_name;
        public virtual string device_name {
             get { return _device_name; }
             set { if (!Helper.AreEqual(value, _device_name)) { _device_name = value; Changed = true; NotifyPropertyChanged("device_name"); } }
         }

        private XenRef<Host> _host;
        public virtual XenRef<Host> host {
             get { return _host; }
             set { if (!Helper.AreEqual(value, _host)) { _host = value; Changed = true; NotifyPropertyChanged("host"); } }
         }

        private string _pci_id;
        public virtual string pci_id {
             get { return _pci_id; }
             set { if (!Helper.AreEqual(value, _pci_id)) { _pci_id = value; Changed = true; NotifyPropertyChanged("pci_id"); } }
         }

        private List<XenRef<PCI>> _dependencies;
        public virtual List<XenRef<PCI>> dependencies {
             get { return _dependencies; }
             set { if (!Helper.AreEqual(value, _dependencies)) { _dependencies = value; Changed = true; NotifyPropertyChanged("dependencies"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }


    }
}
