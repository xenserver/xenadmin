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
    public partial class PIF_metrics : XenObject<PIF_metrics>
    {
        public PIF_metrics()
        {
        }

        public PIF_metrics(string uuid,
            double io_read_kbs,
            double io_write_kbs,
            bool carrier,
            string vendor_id,
            string vendor_name,
            string device_id,
            string device_name,
            long speed,
            bool duplex,
            string pci_bus_path,
            DateTime last_updated,
            Dictionary<string, string> other_config)
        {
            this.uuid = uuid;
            this.io_read_kbs = io_read_kbs;
            this.io_write_kbs = io_write_kbs;
            this.carrier = carrier;
            this.vendor_id = vendor_id;
            this.vendor_name = vendor_name;
            this.device_id = device_id;
            this.device_name = device_name;
            this.speed = speed;
            this.duplex = duplex;
            this.pci_bus_path = pci_bus_path;
            this.last_updated = last_updated;
            this.other_config = other_config;
        }

        /// <summary>
        /// Creates a new PIF_metrics from a Proxy_PIF_metrics.
        /// </summary>
        /// <param name="proxy"></param>
        public PIF_metrics(Proxy_PIF_metrics proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(PIF_metrics update)
        {
            uuid = update.uuid;
            io_read_kbs = update.io_read_kbs;
            io_write_kbs = update.io_write_kbs;
            carrier = update.carrier;
            vendor_id = update.vendor_id;
            vendor_name = update.vendor_name;
            device_id = update.device_id;
            device_name = update.device_name;
            speed = update.speed;
            duplex = update.duplex;
            pci_bus_path = update.pci_bus_path;
            last_updated = update.last_updated;
            other_config = update.other_config;
        }

        internal void UpdateFromProxy(Proxy_PIF_metrics proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            io_read_kbs = Convert.ToDouble(proxy.io_read_kbs);
            io_write_kbs = Convert.ToDouble(proxy.io_write_kbs);
            carrier = (bool)proxy.carrier;
            vendor_id = proxy.vendor_id == null ? null : (string)proxy.vendor_id;
            vendor_name = proxy.vendor_name == null ? null : (string)proxy.vendor_name;
            device_id = proxy.device_id == null ? null : (string)proxy.device_id;
            device_name = proxy.device_name == null ? null : (string)proxy.device_name;
            speed = proxy.speed == null ? 0 : long.Parse((string)proxy.speed);
            duplex = (bool)proxy.duplex;
            pci_bus_path = proxy.pci_bus_path == null ? null : (string)proxy.pci_bus_path;
            last_updated = proxy.last_updated;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
        }

        public Proxy_PIF_metrics ToProxy()
        {
            Proxy_PIF_metrics result_ = new Proxy_PIF_metrics();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.io_read_kbs = io_read_kbs;
            result_.io_write_kbs = io_write_kbs;
            result_.carrier = carrier;
            result_.vendor_id = (vendor_id != null) ? vendor_id : "";
            result_.vendor_name = (vendor_name != null) ? vendor_name : "";
            result_.device_id = (device_id != null) ? device_id : "";
            result_.device_name = (device_name != null) ? device_name : "";
            result_.speed = speed.ToString();
            result_.duplex = duplex;
            result_.pci_bus_path = (pci_bus_path != null) ? pci_bus_path : "";
            result_.last_updated = last_updated;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            return result_;
        }

        /// <summary>
        /// Creates a new PIF_metrics from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public PIF_metrics(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            io_read_kbs = Marshalling.ParseDouble(table, "io_read_kbs");
            io_write_kbs = Marshalling.ParseDouble(table, "io_write_kbs");
            carrier = Marshalling.ParseBool(table, "carrier");
            vendor_id = Marshalling.ParseString(table, "vendor_id");
            vendor_name = Marshalling.ParseString(table, "vendor_name");
            device_id = Marshalling.ParseString(table, "device_id");
            device_name = Marshalling.ParseString(table, "device_name");
            speed = Marshalling.ParseLong(table, "speed");
            duplex = Marshalling.ParseBool(table, "duplex");
            pci_bus_path = Marshalling.ParseString(table, "pci_bus_path");
            last_updated = Marshalling.ParseDateTime(table, "last_updated");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
        }

        public bool DeepEquals(PIF_metrics other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._io_read_kbs, other._io_read_kbs) &&
                Helper.AreEqual2(this._io_write_kbs, other._io_write_kbs) &&
                Helper.AreEqual2(this._carrier, other._carrier) &&
                Helper.AreEqual2(this._vendor_id, other._vendor_id) &&
                Helper.AreEqual2(this._vendor_name, other._vendor_name) &&
                Helper.AreEqual2(this._device_id, other._device_id) &&
                Helper.AreEqual2(this._device_name, other._device_name) &&
                Helper.AreEqual2(this._speed, other._speed) &&
                Helper.AreEqual2(this._duplex, other._duplex) &&
                Helper.AreEqual2(this._pci_bus_path, other._pci_bus_path) &&
                Helper.AreEqual2(this._last_updated, other._last_updated) &&
                Helper.AreEqual2(this._other_config, other._other_config);
        }

        public override string SaveChanges(Session session, string opaqueRef, PIF_metrics server)
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
                    PIF_metrics.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }

        public static PIF_metrics get_record(Session session, string _pif_metrics)
        {
            return new PIF_metrics((Proxy_PIF_metrics)session.proxy.pif_metrics_get_record(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse());
        }

        public static XenRef<PIF_metrics> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<PIF_metrics>.Create(session.proxy.pif_metrics_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _pif_metrics)
        {
            return (string)session.proxy.pif_metrics_get_uuid(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse();
        }

        public static double get_io_read_kbs(Session session, string _pif_metrics)
        {
            return Convert.ToDouble(session.proxy.pif_metrics_get_io_read_kbs(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse());
        }

        public static double get_io_write_kbs(Session session, string _pif_metrics)
        {
            return Convert.ToDouble(session.proxy.pif_metrics_get_io_write_kbs(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse());
        }

        public static bool get_carrier(Session session, string _pif_metrics)
        {
            return (bool)session.proxy.pif_metrics_get_carrier(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse();
        }

        public static string get_vendor_id(Session session, string _pif_metrics)
        {
            return (string)session.proxy.pif_metrics_get_vendor_id(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse();
        }

        public static string get_vendor_name(Session session, string _pif_metrics)
        {
            return (string)session.proxy.pif_metrics_get_vendor_name(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse();
        }

        public static string get_device_id(Session session, string _pif_metrics)
        {
            return (string)session.proxy.pif_metrics_get_device_id(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse();
        }

        public static string get_device_name(Session session, string _pif_metrics)
        {
            return (string)session.proxy.pif_metrics_get_device_name(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse();
        }

        public static long get_speed(Session session, string _pif_metrics)
        {
            return long.Parse((string)session.proxy.pif_metrics_get_speed(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse());
        }

        public static bool get_duplex(Session session, string _pif_metrics)
        {
            return (bool)session.proxy.pif_metrics_get_duplex(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse();
        }

        public static string get_pci_bus_path(Session session, string _pif_metrics)
        {
            return (string)session.proxy.pif_metrics_get_pci_bus_path(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse();
        }

        public static DateTime get_last_updated(Session session, string _pif_metrics)
        {
            return session.proxy.pif_metrics_get_last_updated(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse();
        }

        public static Dictionary<string, string> get_other_config(Session session, string _pif_metrics)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pif_metrics_get_other_config(session.uuid, (_pif_metrics != null) ? _pif_metrics : "").parse());
        }

        public static void set_other_config(Session session, string _pif_metrics, Dictionary<string, string> _other_config)
        {
            session.proxy.pif_metrics_set_other_config(session.uuid, (_pif_metrics != null) ? _pif_metrics : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _pif_metrics, string _key, string _value)
        {
            session.proxy.pif_metrics_add_to_other_config(session.uuid, (_pif_metrics != null) ? _pif_metrics : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _pif_metrics, string _key)
        {
            session.proxy.pif_metrics_remove_from_other_config(session.uuid, (_pif_metrics != null) ? _pif_metrics : "", (_key != null) ? _key : "").parse();
        }

        public static List<XenRef<PIF_metrics>> get_all(Session session)
        {
            return XenRef<PIF_metrics>.Create(session.proxy.pif_metrics_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<PIF_metrics>, PIF_metrics> get_all_records(Session session)
        {
            return XenRef<PIF_metrics>.Create<Proxy_PIF_metrics>(session.proxy.pif_metrics_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private double _io_read_kbs;
        public virtual double io_read_kbs {
             get { return _io_read_kbs; }
             set { if (!Helper.AreEqual(value, _io_read_kbs)) { _io_read_kbs = value; Changed = true; NotifyPropertyChanged("io_read_kbs"); } }
         }

        private double _io_write_kbs;
        public virtual double io_write_kbs {
             get { return _io_write_kbs; }
             set { if (!Helper.AreEqual(value, _io_write_kbs)) { _io_write_kbs = value; Changed = true; NotifyPropertyChanged("io_write_kbs"); } }
         }

        private bool _carrier;
        public virtual bool carrier {
             get { return _carrier; }
             set { if (!Helper.AreEqual(value, _carrier)) { _carrier = value; Changed = true; NotifyPropertyChanged("carrier"); } }
         }

        private string _vendor_id;
        public virtual string vendor_id {
             get { return _vendor_id; }
             set { if (!Helper.AreEqual(value, _vendor_id)) { _vendor_id = value; Changed = true; NotifyPropertyChanged("vendor_id"); } }
         }

        private string _vendor_name;
        public virtual string vendor_name {
             get { return _vendor_name; }
             set { if (!Helper.AreEqual(value, _vendor_name)) { _vendor_name = value; Changed = true; NotifyPropertyChanged("vendor_name"); } }
         }

        private string _device_id;
        public virtual string device_id {
             get { return _device_id; }
             set { if (!Helper.AreEqual(value, _device_id)) { _device_id = value; Changed = true; NotifyPropertyChanged("device_id"); } }
         }

        private string _device_name;
        public virtual string device_name {
             get { return _device_name; }
             set { if (!Helper.AreEqual(value, _device_name)) { _device_name = value; Changed = true; NotifyPropertyChanged("device_name"); } }
         }

        private long _speed;
        public virtual long speed {
             get { return _speed; }
             set { if (!Helper.AreEqual(value, _speed)) { _speed = value; Changed = true; NotifyPropertyChanged("speed"); } }
         }

        private bool _duplex;
        public virtual bool duplex {
             get { return _duplex; }
             set { if (!Helper.AreEqual(value, _duplex)) { _duplex = value; Changed = true; NotifyPropertyChanged("duplex"); } }
         }

        private string _pci_bus_path;
        public virtual string pci_bus_path {
             get { return _pci_bus_path; }
             set { if (!Helper.AreEqual(value, _pci_bus_path)) { _pci_bus_path = value; Changed = true; NotifyPropertyChanged("pci_bus_path"); } }
         }

        private DateTime _last_updated;
        public virtual DateTime last_updated {
             get { return _last_updated; }
             set { if (!Helper.AreEqual(value, _last_updated)) { _last_updated = value; Changed = true; NotifyPropertyChanged("last_updated"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }


    }
}
