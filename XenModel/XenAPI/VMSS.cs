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
    /// VM Snapshot Schedule
    /// First published in XenServer Dundee.
    /// </summary>
    public partial class VMSS : XenObject<VMSS>
    {
        public VMSS()
        {
        }

        public VMSS(string uuid,
            string name_label,
            string name_description,
            bool enabled,
            vmss_type type,
            long retained_snapshots,
            vmss_frequency frequency,
            Dictionary<string, string> schedule,
            DateTime last_run_time,
            List<XenRef<VM>> VMs)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.enabled = enabled;
            this.type = type;
            this.retained_snapshots = retained_snapshots;
            this.frequency = frequency;
            this.schedule = schedule;
            this.last_run_time = last_run_time;
            this.VMs = VMs;
        }

        /// <summary>
        /// Creates a new VMSS from a Proxy_VMSS.
        /// </summary>
        /// <param name="proxy"></param>
        public VMSS(Proxy_VMSS proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(VMSS update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            enabled = update.enabled;
            type = update.type;
            retained_snapshots = update.retained_snapshots;
            frequency = update.frequency;
            schedule = update.schedule;
            last_run_time = update.last_run_time;
            VMs = update.VMs;
        }

        internal void UpdateFromProxy(Proxy_VMSS proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            enabled = (bool)proxy.enabled;
            type = proxy.type == null ? (vmss_type) 0 : (vmss_type)Helper.EnumParseDefault(typeof(vmss_type), (string)proxy.type);
            retained_snapshots = proxy.retained_snapshots == null ? 0 : long.Parse((string)proxy.retained_snapshots);
            frequency = proxy.frequency == null ? (vmss_frequency) 0 : (vmss_frequency)Helper.EnumParseDefault(typeof(vmss_frequency), (string)proxy.frequency);
            schedule = proxy.schedule == null ? null : Maps.convert_from_proxy_string_string(proxy.schedule);
            last_run_time = proxy.last_run_time;
            VMs = proxy.VMs == null ? null : XenRef<VM>.Create(proxy.VMs);
        }

        public Proxy_VMSS ToProxy()
        {
            Proxy_VMSS result_ = new Proxy_VMSS();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.enabled = enabled;
            result_.type = vmss_type_helper.ToString(type);
            result_.retained_snapshots = retained_snapshots.ToString();
            result_.frequency = vmss_frequency_helper.ToString(frequency);
            result_.schedule = Maps.convert_to_proxy_string_string(schedule);
            result_.last_run_time = last_run_time;
            result_.VMs = (VMs != null) ? Helper.RefListToStringArray(VMs) : new string[] {};
            return result_;
        }

        /// <summary>
        /// Creates a new VMSS from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public VMSS(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            enabled = Marshalling.ParseBool(table, "enabled");
            type = (vmss_type)Helper.EnumParseDefault(typeof(vmss_type), Marshalling.ParseString(table, "type"));
            retained_snapshots = Marshalling.ParseLong(table, "retained_snapshots");
            frequency = (vmss_frequency)Helper.EnumParseDefault(typeof(vmss_frequency), Marshalling.ParseString(table, "frequency"));
            schedule = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "schedule"));
            last_run_time = Marshalling.ParseDateTime(table, "last_run_time");
            VMs = Marshalling.ParseSetRef<VM>(table, "VMs");
        }

        public bool DeepEquals(VMSS other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._enabled, other._enabled) &&
                Helper.AreEqual2(this._type, other._type) &&
                Helper.AreEqual2(this._retained_snapshots, other._retained_snapshots) &&
                Helper.AreEqual2(this._frequency, other._frequency) &&
                Helper.AreEqual2(this._schedule, other._schedule) &&
                Helper.AreEqual2(this._last_run_time, other._last_run_time) &&
                Helper.AreEqual2(this._VMs, other._VMs);
        }

        public override string SaveChanges(Session session, string opaqueRef, VMSS server)
        {
            if (opaqueRef == null)
            {
                Proxy_VMSS p = this.ToProxy();
                return session.proxy.vmss_create(session.uuid, p).parse();
            }
            else
            {
                if (!Helper.AreEqual2(_name_label, server._name_label))
                {
                    VMSS.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    VMSS.set_name_description(session, opaqueRef, _name_description);
                }
                if (!Helper.AreEqual2(_enabled, server._enabled))
                {
                    VMSS.set_enabled(session, opaqueRef, _enabled);
                }
                if (!Helper.AreEqual2(_type, server._type))
                {
                    VMSS.set_type(session, opaqueRef, _type);
                }
                if (!Helper.AreEqual2(_retained_snapshots, server._retained_snapshots))
                {
                    VMSS.set_retained_snapshots(session, opaqueRef, _retained_snapshots);
                }
                if (!Helper.AreEqual2(_frequency, server._frequency))
                {
                    VMSS.set_frequency(session, opaqueRef, _frequency);
                }
                if (!Helper.AreEqual2(_schedule, server._schedule))
                {
                    VMSS.set_schedule(session, opaqueRef, _schedule);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static VMSS get_record(Session session, string _vmss)
        {
            return new VMSS((Proxy_VMSS)session.proxy.vmss_get_record(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Get a reference to the VMSS instance with the specified UUID.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<VMSS> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VMSS>.Create(session.proxy.vmss_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Create a new VMSS instance, and return its handle.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<VMSS> create(Session session, VMSS _record)
        {
            return XenRef<VMSS>.Create(session.proxy.vmss_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Create a new VMSS instance, and return its handle.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_record">All constructor arguments</param>
        public static XenRef<Task> async_create(Session session, VMSS _record)
        {
            return XenRef<Task>.Create(session.proxy.async_vmss_create(session.uuid, _record.ToProxy()).parse());
        }

        /// <summary>
        /// Destroy the specified VMSS instance.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static void destroy(Session session, string _vmss)
        {
            session.proxy.vmss_destroy(session.uuid, (_vmss != null) ? _vmss : "").parse();
        }

        /// <summary>
        /// Destroy the specified VMSS instance.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static XenRef<Task> async_destroy(Session session, string _vmss)
        {
            return XenRef<Task>.Create(session.proxy.async_vmss_destroy(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Get all the VMSS instances with the given label.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_label">label of object to return</param>
        public static List<XenRef<VMSS>> get_by_name_label(Session session, string _label)
        {
            return XenRef<VMSS>.Create(session.proxy.vmss_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static string get_uuid(Session session, string _vmss)
        {
            return (string)session.proxy.vmss_get_uuid(session.uuid, (_vmss != null) ? _vmss : "").parse();
        }

        /// <summary>
        /// Get the name/label field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static string get_name_label(Session session, string _vmss)
        {
            return (string)session.proxy.vmss_get_name_label(session.uuid, (_vmss != null) ? _vmss : "").parse();
        }

        /// <summary>
        /// Get the name/description field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static string get_name_description(Session session, string _vmss)
        {
            return (string)session.proxy.vmss_get_name_description(session.uuid, (_vmss != null) ? _vmss : "").parse();
        }

        /// <summary>
        /// Get the enabled field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static bool get_enabled(Session session, string _vmss)
        {
            return (bool)session.proxy.vmss_get_enabled(session.uuid, (_vmss != null) ? _vmss : "").parse();
        }

        /// <summary>
        /// Get the type field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static vmss_type get_type(Session session, string _vmss)
        {
            return (vmss_type)Helper.EnumParseDefault(typeof(vmss_type), (string)session.proxy.vmss_get_type(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Get the retained_snapshots field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static long get_retained_snapshots(Session session, string _vmss)
        {
            return long.Parse((string)session.proxy.vmss_get_retained_snapshots(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Get the frequency field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static vmss_frequency get_frequency(Session session, string _vmss)
        {
            return (vmss_frequency)Helper.EnumParseDefault(typeof(vmss_frequency), (string)session.proxy.vmss_get_frequency(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Get the schedule field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static Dictionary<string, string> get_schedule(Session session, string _vmss)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vmss_get_schedule(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Get the last_run_time field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static DateTime get_last_run_time(Session session, string _vmss)
        {
            return session.proxy.vmss_get_last_run_time(session.uuid, (_vmss != null) ? _vmss : "").parse();
        }

        /// <summary>
        /// Get the VMs field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static List<XenRef<VM>> get_VMs(Session session, string _vmss)
        {
            return XenRef<VM>.Create(session.proxy.vmss_get_vms(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Set the name/label field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_label">New value to set</param>
        public static void set_name_label(Session session, string _vmss, string _label)
        {
            session.proxy.vmss_set_name_label(session.uuid, (_vmss != null) ? _vmss : "", (_label != null) ? _label : "").parse();
        }

        /// <summary>
        /// Set the name/description field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_description">New value to set</param>
        public static void set_name_description(Session session, string _vmss, string _description)
        {
            session.proxy.vmss_set_name_description(session.uuid, (_vmss != null) ? _vmss : "", (_description != null) ? _description : "").parse();
        }

        /// <summary>
        /// Set the enabled field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_enabled">New value to set</param>
        public static void set_enabled(Session session, string _vmss, bool _enabled)
        {
            session.proxy.vmss_set_enabled(session.uuid, (_vmss != null) ? _vmss : "", _enabled).parse();
        }

        /// <summary>
        /// This call executes the snapshot schedule immediately
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static string snapshot_now(Session session, string _vmss)
        {
            return (string)session.proxy.vmss_snapshot_now(session.uuid, (_vmss != null) ? _vmss : "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the value to set</param>
        public static void set_retained_snapshots(Session session, string _vmss, long _value)
        {
            session.proxy.vmss_set_retained_snapshots(session.uuid, (_vmss != null) ? _vmss : "", _value.ToString()).parse();
        }

        /// <summary>
        /// Set the value of the frequency field
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the snapshot schedule frequency</param>
        public static void set_frequency(Session session, string _vmss, vmss_frequency _value)
        {
            session.proxy.vmss_set_frequency(session.uuid, (_vmss != null) ? _vmss : "", vmss_frequency_helper.ToString(_value)).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the value to set</param>
        public static void set_schedule(Session session, string _vmss, Dictionary<string, string> _value)
        {
            session.proxy.vmss_set_schedule(session.uuid, (_vmss != null) ? _vmss : "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        public static void add_to_schedule(Session session, string _vmss, string _key, string _value)
        {
            session.proxy.vmss_add_to_schedule(session.uuid, (_vmss != null) ? _vmss : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_key">the key to remove</param>
        public static void remove_from_schedule(Session session, string _vmss, string _key)
        {
            session.proxy.vmss_remove_from_schedule(session.uuid, (_vmss != null) ? _vmss : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the value to set</param>
        public static void set_last_run_time(Session session, string _vmss, DateTime _value)
        {
            session.proxy.vmss_set_last_run_time(session.uuid, (_vmss != null) ? _vmss : "", _value).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the snapshot schedule type</param>
        public static void set_type(Session session, string _vmss, vmss_type _value)
        {
            session.proxy.vmss_set_type(session.uuid, (_vmss != null) ? _vmss : "", vmss_type_helper.ToString(_value)).parse();
        }

        /// <summary>
        /// Return a list of all the VMSSs known to the system.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<VMSS>> get_all(Session session)
        {
            return XenRef<VMSS>.Create(session.proxy.vmss_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the VMSS Records at once, in a single XML RPC call
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VMSS>, VMSS> get_all_records(Session session)
        {
            return XenRef<VMSS>.Create<Proxy_VMSS>(session.proxy.vmss_get_all_records(session.uuid).parse());
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
        /// enable or disable this snapshot schedule
        /// </summary>
        public virtual bool enabled
        {
            get { return _enabled; }
            set
            {
                if (!Helper.AreEqual(value, _enabled))
                {
                    _enabled = value;
                    Changed = true;
                    NotifyPropertyChanged("enabled");
                }
            }
        }
        private bool _enabled;

        /// <summary>
        /// type of the snapshot schedule
        /// </summary>
        public virtual vmss_type type
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
        private vmss_type _type;

        /// <summary>
        /// maximum number of snapshots that should be stored at any time
        /// </summary>
        public virtual long retained_snapshots
        {
            get { return _retained_snapshots; }
            set
            {
                if (!Helper.AreEqual(value, _retained_snapshots))
                {
                    _retained_snapshots = value;
                    Changed = true;
                    NotifyPropertyChanged("retained_snapshots");
                }
            }
        }
        private long _retained_snapshots;

        /// <summary>
        /// frequency of taking snapshot from snapshot schedule
        /// </summary>
        public virtual vmss_frequency frequency
        {
            get { return _frequency; }
            set
            {
                if (!Helper.AreEqual(value, _frequency))
                {
                    _frequency = value;
                    Changed = true;
                    NotifyPropertyChanged("frequency");
                }
            }
        }
        private vmss_frequency _frequency;

        /// <summary>
        /// schedule of the snapshot containing 'hour', 'min', 'days'. Date/time-related information is in Local Timezone
        /// </summary>
        public virtual Dictionary<string, string> schedule
        {
            get { return _schedule; }
            set
            {
                if (!Helper.AreEqual(value, _schedule))
                {
                    _schedule = value;
                    Changed = true;
                    NotifyPropertyChanged("schedule");
                }
            }
        }
        private Dictionary<string, string> _schedule;

        /// <summary>
        /// time of the last snapshot
        /// </summary>
        public virtual DateTime last_run_time
        {
            get { return _last_run_time; }
            set
            {
                if (!Helper.AreEqual(value, _last_run_time))
                {
                    _last_run_time = value;
                    Changed = true;
                    NotifyPropertyChanged("last_run_time");
                }
            }
        }
        private DateTime _last_run_time;

        /// <summary>
        /// all VMs attached to this snapshot schedule
        /// </summary>
        public virtual List<XenRef<VM>> VMs
        {
            get { return _VMs; }
            set
            {
                if (!Helper.AreEqual(value, _VMs))
                {
                    _VMs = value;
                    Changed = true;
                    NotifyPropertyChanged("VMs");
                }
            }
        }
        private List<XenRef<VM>> _VMs;
    }
}
