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
    public partial class VM_guest_metrics : XenObject<VM_guest_metrics>
    {
        public VM_guest_metrics()
        {
        }

        public VM_guest_metrics(string uuid,
            Dictionary<string, string> os_version,
            Dictionary<string, string> PV_drivers_version,
            bool PV_drivers_up_to_date,
            Dictionary<string, string> memory,
            Dictionary<string, string> disks,
            Dictionary<string, string> networks,
            Dictionary<string, string> other,
            DateTime last_updated,
            Dictionary<string, string> other_config,
            bool live)
        {
            this.uuid = uuid;
            this.os_version = os_version;
            this.PV_drivers_version = PV_drivers_version;
            this.PV_drivers_up_to_date = PV_drivers_up_to_date;
            this.memory = memory;
            this.disks = disks;
            this.networks = networks;
            this.other = other;
            this.last_updated = last_updated;
            this.other_config = other_config;
            this.live = live;
        }

        /// <summary>
        /// Creates a new VM_guest_metrics from a Proxy_VM_guest_metrics.
        /// </summary>
        /// <param name="proxy"></param>
        public VM_guest_metrics(Proxy_VM_guest_metrics proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(VM_guest_metrics update)
        {
            uuid = update.uuid;
            os_version = update.os_version;
            PV_drivers_version = update.PV_drivers_version;
            PV_drivers_up_to_date = update.PV_drivers_up_to_date;
            memory = update.memory;
            disks = update.disks;
            networks = update.networks;
            other = update.other;
            last_updated = update.last_updated;
            other_config = update.other_config;
            live = update.live;
        }

        internal void UpdateFromProxy(Proxy_VM_guest_metrics proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            os_version = proxy.os_version == null ? null : Maps.convert_from_proxy_string_string(proxy.os_version);
            PV_drivers_version = proxy.PV_drivers_version == null ? null : Maps.convert_from_proxy_string_string(proxy.PV_drivers_version);
            PV_drivers_up_to_date = (bool)proxy.PV_drivers_up_to_date;
            memory = proxy.memory == null ? null : Maps.convert_from_proxy_string_string(proxy.memory);
            disks = proxy.disks == null ? null : Maps.convert_from_proxy_string_string(proxy.disks);
            networks = proxy.networks == null ? null : Maps.convert_from_proxy_string_string(proxy.networks);
            other = proxy.other == null ? null : Maps.convert_from_proxy_string_string(proxy.other);
            last_updated = proxy.last_updated;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            live = (bool)proxy.live;
        }

        public Proxy_VM_guest_metrics ToProxy()
        {
            Proxy_VM_guest_metrics result_ = new Proxy_VM_guest_metrics();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.os_version = Maps.convert_to_proxy_string_string(os_version);
            result_.PV_drivers_version = Maps.convert_to_proxy_string_string(PV_drivers_version);
            result_.PV_drivers_up_to_date = PV_drivers_up_to_date;
            result_.memory = Maps.convert_to_proxy_string_string(memory);
            result_.disks = Maps.convert_to_proxy_string_string(disks);
            result_.networks = Maps.convert_to_proxy_string_string(networks);
            result_.other = Maps.convert_to_proxy_string_string(other);
            result_.last_updated = last_updated;
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.live = live;
            return result_;
        }

        /// <summary>
        /// Creates a new VM_guest_metrics from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public VM_guest_metrics(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            os_version = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "os_version"));
            PV_drivers_version = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "PV_drivers_version"));
            PV_drivers_up_to_date = Marshalling.ParseBool(table, "PV_drivers_up_to_date");
            memory = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "memory"));
            disks = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "disks"));
            networks = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "networks"));
            other = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other"));
            last_updated = Marshalling.ParseDateTime(table, "last_updated");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            live = Marshalling.ParseBool(table, "live");
        }

        public bool DeepEquals(VM_guest_metrics other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._os_version, other._os_version) &&
                Helper.AreEqual2(this._PV_drivers_version, other._PV_drivers_version) &&
                Helper.AreEqual2(this._PV_drivers_up_to_date, other._PV_drivers_up_to_date) &&
                Helper.AreEqual2(this._memory, other._memory) &&
                Helper.AreEqual2(this._disks, other._disks) &&
                Helper.AreEqual2(this._networks, other._networks) &&
                Helper.AreEqual2(this._other, other._other) &&
                Helper.AreEqual2(this._last_updated, other._last_updated) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._live, other._live);
        }

        public override string SaveChanges(Session session, string opaqueRef, VM_guest_metrics server)
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
                    VM_guest_metrics.set_other_config(session, opaqueRef, _other_config);
                }

                return null;
            }
        }

        public static VM_guest_metrics get_record(Session session, string _vm_guest_metrics)
        {
            return new VM_guest_metrics((Proxy_VM_guest_metrics)session.proxy.vm_guest_metrics_get_record(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse());
        }

        public static XenRef<VM_guest_metrics> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VM_guest_metrics>.Create(session.proxy.vm_guest_metrics_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static string get_uuid(Session session, string _vm_guest_metrics)
        {
            return (string)session.proxy.vm_guest_metrics_get_uuid(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse();
        }

        public static Dictionary<string, string> get_os_version(Session session, string _vm_guest_metrics)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_os_version(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse());
        }

        public static Dictionary<string, string> get_PV_drivers_version(Session session, string _vm_guest_metrics)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_pv_drivers_version(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse());
        }

        public static bool get_PV_drivers_up_to_date(Session session, string _vm_guest_metrics)
        {
            return (bool)session.proxy.vm_guest_metrics_get_pv_drivers_up_to_date(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse();
        }

        public static Dictionary<string, string> get_memory(Session session, string _vm_guest_metrics)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_memory(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse());
        }

        public static Dictionary<string, string> get_disks(Session session, string _vm_guest_metrics)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_disks(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse());
        }

        public static Dictionary<string, string> get_networks(Session session, string _vm_guest_metrics)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_networks(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse());
        }

        public static Dictionary<string, string> get_other(Session session, string _vm_guest_metrics)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_other(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse());
        }

        public static DateTime get_last_updated(Session session, string _vm_guest_metrics)
        {
            return session.proxy.vm_guest_metrics_get_last_updated(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse();
        }

        public static Dictionary<string, string> get_other_config(Session session, string _vm_guest_metrics)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_guest_metrics_get_other_config(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse());
        }

        public static bool get_live(Session session, string _vm_guest_metrics)
        {
            return (bool)session.proxy.vm_guest_metrics_get_live(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "").parse();
        }

        public static void set_other_config(Session session, string _vm_guest_metrics, Dictionary<string, string> _other_config)
        {
            session.proxy.vm_guest_metrics_set_other_config(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _vm_guest_metrics, string _key, string _value)
        {
            session.proxy.vm_guest_metrics_add_to_other_config(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _vm_guest_metrics, string _key)
        {
            session.proxy.vm_guest_metrics_remove_from_other_config(session.uuid, (_vm_guest_metrics != null) ? _vm_guest_metrics : "", (_key != null) ? _key : "").parse();
        }

        public static List<XenRef<VM_guest_metrics>> get_all(Session session)
        {
            return XenRef<VM_guest_metrics>.Create(session.proxy.vm_guest_metrics_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<VM_guest_metrics>, VM_guest_metrics> get_all_records(Session session)
        {
            return XenRef<VM_guest_metrics>.Create<Proxy_VM_guest_metrics>(session.proxy.vm_guest_metrics_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private Dictionary<string, string> _os_version;
        public virtual Dictionary<string, string> os_version {
             get { return _os_version; }
             set { if (!Helper.AreEqual(value, _os_version)) { _os_version = value; Changed = true; NotifyPropertyChanged("os_version"); } }
         }

        private Dictionary<string, string> _PV_drivers_version;
        public virtual Dictionary<string, string> PV_drivers_version {
             get { return _PV_drivers_version; }
             set { if (!Helper.AreEqual(value, _PV_drivers_version)) { _PV_drivers_version = value; Changed = true; NotifyPropertyChanged("PV_drivers_version"); } }
         }

        private bool _PV_drivers_up_to_date;
        public virtual bool PV_drivers_up_to_date {
             get { return _PV_drivers_up_to_date; }
             set { if (!Helper.AreEqual(value, _PV_drivers_up_to_date)) { _PV_drivers_up_to_date = value; Changed = true; NotifyPropertyChanged("PV_drivers_up_to_date"); } }
         }

        private Dictionary<string, string> _memory;
        public virtual Dictionary<string, string> memory {
             get { return _memory; }
             set { if (!Helper.AreEqual(value, _memory)) { _memory = value; Changed = true; NotifyPropertyChanged("memory"); } }
         }

        private Dictionary<string, string> _disks;
        public virtual Dictionary<string, string> disks {
             get { return _disks; }
             set { if (!Helper.AreEqual(value, _disks)) { _disks = value; Changed = true; NotifyPropertyChanged("disks"); } }
         }

        private Dictionary<string, string> _networks;
        public virtual Dictionary<string, string> networks {
             get { return _networks; }
             set { if (!Helper.AreEqual(value, _networks)) { _networks = value; Changed = true; NotifyPropertyChanged("networks"); } }
         }

        private Dictionary<string, string> _other;
        public virtual Dictionary<string, string> other {
             get { return _other; }
             set { if (!Helper.AreEqual(value, _other)) { _other = value; Changed = true; NotifyPropertyChanged("other"); } }
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

        private bool _live;
        public virtual bool live {
             get { return _live; }
             set { if (!Helper.AreEqual(value, _live)) { _live = value; Changed = true; NotifyPropertyChanged("live"); } }
         }


    }
}
