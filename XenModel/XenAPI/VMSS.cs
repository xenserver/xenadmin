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
    /// VM Schedule Snapshot
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
            bool is_schedule_snapshot_enabled,
            vmss_schedule_snapshot_type schedule_snapshot_type,
            long schedule_snapshot_retention_value,
            vmss_schedule_snapshot_frequency schedule_snapshot_frequency,
            Dictionary<string, string> snapshot_schedule,
            bool is_schedule_snapshot_running,
            DateTime schedule_snapshot_last_run_time,
            List<XenRef<VM>> VMs,
            bool is_alarm_enabled,
            Dictionary<string, string> alarm_config,
            string[] recent_alerts)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.is_schedule_snapshot_enabled = is_schedule_snapshot_enabled;
            this.schedule_snapshot_type = schedule_snapshot_type;
            this.schedule_snapshot_retention_value = schedule_snapshot_retention_value;
            this.schedule_snapshot_frequency = schedule_snapshot_frequency;
            this.snapshot_schedule = snapshot_schedule;
            this.is_schedule_snapshot_running = is_schedule_snapshot_running;
            this.schedule_snapshot_last_run_time = schedule_snapshot_last_run_time;
            this.VMs = VMs;
            this.is_alarm_enabled = is_alarm_enabled;
            this.alarm_config = alarm_config;
            this.recent_alerts = recent_alerts;
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
            is_schedule_snapshot_enabled = update.is_schedule_snapshot_enabled;
            schedule_snapshot_type = update.schedule_snapshot_type;
            schedule_snapshot_retention_value = update.schedule_snapshot_retention_value;
            schedule_snapshot_frequency = update.schedule_snapshot_frequency;
            snapshot_schedule = update.snapshot_schedule;
            is_schedule_snapshot_running = update.is_schedule_snapshot_running;
            schedule_snapshot_last_run_time = update.schedule_snapshot_last_run_time;
            VMs = update.VMs;
            is_alarm_enabled = update.is_alarm_enabled;
            alarm_config = update.alarm_config;
            recent_alerts = update.recent_alerts;
        }

        internal void UpdateFromProxy(Proxy_VMSS proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            is_schedule_snapshot_enabled = (bool)proxy.is_schedule_snapshot_enabled;
            schedule_snapshot_type = proxy.schedule_snapshot_type == null ? (vmss_schedule_snapshot_type) 0 : (vmss_schedule_snapshot_type)Helper.EnumParseDefault(typeof(vmss_schedule_snapshot_type), (string)proxy.schedule_snapshot_type);
            schedule_snapshot_retention_value = proxy.schedule_snapshot_retention_value == null ? 0 : long.Parse((string)proxy.schedule_snapshot_retention_value);
            schedule_snapshot_frequency = proxy.schedule_snapshot_frequency == null ? (vmss_schedule_snapshot_frequency) 0 : (vmss_schedule_snapshot_frequency)Helper.EnumParseDefault(typeof(vmss_schedule_snapshot_frequency), (string)proxy.schedule_snapshot_frequency);
            snapshot_schedule = proxy.snapshot_schedule == null ? null : Maps.convert_from_proxy_string_string(proxy.snapshot_schedule);
            is_schedule_snapshot_running = (bool)proxy.is_schedule_snapshot_running;
            schedule_snapshot_last_run_time = proxy.schedule_snapshot_last_run_time;
            VMs = proxy.VMs == null ? null : XenRef<VM>.Create(proxy.VMs);
            is_alarm_enabled = (bool)proxy.is_alarm_enabled;
            alarm_config = proxy.alarm_config == null ? null : Maps.convert_from_proxy_string_string(proxy.alarm_config);
            recent_alerts = proxy.recent_alerts == null ? new string[] {} : (string [])proxy.recent_alerts;
        }

        public Proxy_VMSS ToProxy()
        {
            Proxy_VMSS result_ = new Proxy_VMSS();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.is_schedule_snapshot_enabled = is_schedule_snapshot_enabled;
            result_.schedule_snapshot_type = vmss_schedule_snapshot_type_helper.ToString(schedule_snapshot_type);
            result_.schedule_snapshot_retention_value = schedule_snapshot_retention_value.ToString();
            result_.schedule_snapshot_frequency = vmss_schedule_snapshot_frequency_helper.ToString(schedule_snapshot_frequency);
            result_.snapshot_schedule = Maps.convert_to_proxy_string_string(snapshot_schedule);
            result_.is_schedule_snapshot_running = is_schedule_snapshot_running;
            result_.schedule_snapshot_last_run_time = schedule_snapshot_last_run_time;
            result_.VMs = (VMs != null) ? Helper.RefListToStringArray(VMs) : new string[] {};
            result_.is_alarm_enabled = is_alarm_enabled;
            result_.alarm_config = Maps.convert_to_proxy_string_string(alarm_config);
            result_.recent_alerts = recent_alerts;
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
            is_schedule_snapshot_enabled = Marshalling.ParseBool(table, "is_schedule_snapshot_enabled");
            schedule_snapshot_type = (vmss_schedule_snapshot_type)Helper.EnumParseDefault(typeof(vmss_schedule_snapshot_type), Marshalling.ParseString(table, "schedule_snapshot_type"));
            schedule_snapshot_retention_value = Marshalling.ParseLong(table, "schedule_snapshot_retention_value");
            schedule_snapshot_frequency = (vmss_schedule_snapshot_frequency)Helper.EnumParseDefault(typeof(vmss_schedule_snapshot_frequency), Marshalling.ParseString(table, "schedule_snapshot_frequency"));
            snapshot_schedule = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "snapshot_schedule"));
            is_schedule_snapshot_running = Marshalling.ParseBool(table, "is_schedule_snapshot_running");
            schedule_snapshot_last_run_time = Marshalling.ParseDateTime(table, "schedule_snapshot_last_run_time");
            VMs = Marshalling.ParseSetRef<VM>(table, "VMs");
            is_alarm_enabled = Marshalling.ParseBool(table, "is_alarm_enabled");
            alarm_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "alarm_config"));
            recent_alerts = Marshalling.ParseStringArray(table, "recent_alerts");
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
                Helper.AreEqual2(this._is_schedule_snapshot_enabled, other._is_schedule_snapshot_enabled) &&
                Helper.AreEqual2(this._schedule_snapshot_type, other._schedule_snapshot_type) &&
                Helper.AreEqual2(this._schedule_snapshot_retention_value, other._schedule_snapshot_retention_value) &&
                Helper.AreEqual2(this._schedule_snapshot_frequency, other._schedule_snapshot_frequency) &&
                Helper.AreEqual2(this._snapshot_schedule, other._snapshot_schedule) &&
                Helper.AreEqual2(this._is_schedule_snapshot_running, other._is_schedule_snapshot_running) &&
                Helper.AreEqual2(this._schedule_snapshot_last_run_time, other._schedule_snapshot_last_run_time) &&
                Helper.AreEqual2(this._VMs, other._VMs) &&
                Helper.AreEqual2(this._is_alarm_enabled, other._is_alarm_enabled) &&
                Helper.AreEqual2(this._alarm_config, other._alarm_config) &&
                Helper.AreEqual2(this._recent_alerts, other._recent_alerts);
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
                if (!Helper.AreEqual2(_is_schedule_snapshot_enabled, server._is_schedule_snapshot_enabled))
                {
                    VMSS.set_is_schedule_snapshot_enabled(session, opaqueRef, _is_schedule_snapshot_enabled);
                }
                if (!Helper.AreEqual2(_schedule_snapshot_type, server._schedule_snapshot_type))
                {
                    VMSS.set_schedule_snapshot_type(session, opaqueRef, _schedule_snapshot_type);
                }
                if (!Helper.AreEqual2(_schedule_snapshot_retention_value, server._schedule_snapshot_retention_value))
                {
                    VMSS.set_schedule_snapshot_retention_value(session, opaqueRef, _schedule_snapshot_retention_value);
                }
                if (!Helper.AreEqual2(_schedule_snapshot_frequency, server._schedule_snapshot_frequency))
                {
                    VMSS.set_schedule_snapshot_frequency(session, opaqueRef, _schedule_snapshot_frequency);
                }
                if (!Helper.AreEqual2(_snapshot_schedule, server._snapshot_schedule))
                {
                    VMSS.set_snapshot_schedule(session, opaqueRef, _snapshot_schedule);
                }
                if (!Helper.AreEqual2(_is_alarm_enabled, server._is_alarm_enabled))
                {
                    VMSS.set_is_alarm_enabled(session, opaqueRef, _is_alarm_enabled);
                }
                if (!Helper.AreEqual2(_alarm_config, server._alarm_config))
                {
                    VMSS.set_alarm_config(session, opaqueRef, _alarm_config);
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
        /// Get the is_schedule_snapshot_enabled field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static bool get_is_schedule_snapshot_enabled(Session session, string _vmss)
        {
            return (bool)session.proxy.vmss_get_is_schedule_snapshot_enabled(session.uuid, (_vmss != null) ? _vmss : "").parse();
        }

        /// <summary>
        /// Get the schedule_snapshot_type field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static vmss_schedule_snapshot_type get_schedule_snapshot_type(Session session, string _vmss)
        {
            return (vmss_schedule_snapshot_type)Helper.EnumParseDefault(typeof(vmss_schedule_snapshot_type), (string)session.proxy.vmss_get_schedule_snapshot_type(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Get the schedule_snapshot_retention_value field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static long get_schedule_snapshot_retention_value(Session session, string _vmss)
        {
            return long.Parse((string)session.proxy.vmss_get_schedule_snapshot_retention_value(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Get the schedule_snapshot_frequency field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static vmss_schedule_snapshot_frequency get_schedule_snapshot_frequency(Session session, string _vmss)
        {
            return (vmss_schedule_snapshot_frequency)Helper.EnumParseDefault(typeof(vmss_schedule_snapshot_frequency), (string)session.proxy.vmss_get_schedule_snapshot_frequency(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Get the snapshot_schedule field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static Dictionary<string, string> get_snapshot_schedule(Session session, string _vmss)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vmss_get_snapshot_schedule(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Get the is_schedule_snapshot_running field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static bool get_is_schedule_snapshot_running(Session session, string _vmss)
        {
            return (bool)session.proxy.vmss_get_is_schedule_snapshot_running(session.uuid, (_vmss != null) ? _vmss : "").parse();
        }

        /// <summary>
        /// Get the schedule_snapshot_last_run_time field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static DateTime get_schedule_snapshot_last_run_time(Session session, string _vmss)
        {
            return session.proxy.vmss_get_schedule_snapshot_last_run_time(session.uuid, (_vmss != null) ? _vmss : "").parse();
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
        /// Get the is_alarm_enabled field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static bool get_is_alarm_enabled(Session session, string _vmss)
        {
            return (bool)session.proxy.vmss_get_is_alarm_enabled(session.uuid, (_vmss != null) ? _vmss : "").parse();
        }

        /// <summary>
        /// Get the alarm_config field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static Dictionary<string, string> get_alarm_config(Session session, string _vmss)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vmss_get_alarm_config(session.uuid, (_vmss != null) ? _vmss : "").parse());
        }

        /// <summary>
        /// Get the recent_alerts field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static string[] get_recent_alerts(Session session, string _vmss)
        {
            return (string [])session.proxy.vmss_get_recent_alerts(session.uuid, (_vmss != null) ? _vmss : "").parse();
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
        /// Set the is_schedule_snapshot_enabled field of the given VMSS.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_is_schedule_snapshot_enabled">New value to set</param>
        public static void set_is_schedule_snapshot_enabled(Session session, string _vmss, bool _is_schedule_snapshot_enabled)
        {
            session.proxy.vmss_set_is_schedule_snapshot_enabled(session.uuid, (_vmss != null) ? _vmss : "", _is_schedule_snapshot_enabled).parse();
        }

        /// <summary>
        /// This call executes the schedule snapshot immediately
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        public static string snapshot_now(Session session, string _vmss)
        {
            return (string)session.proxy.vmss_snapshot_now(session.uuid, (_vmss != null) ? _vmss : "").parse();
        }

        /// <summary>
        /// This call fetches a history of alerts for a given schedule snapshot
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_hours_from_now">how many hours in the past the oldest record to fetch is</param>
        public static string[] get_alerts(Session session, string _vmss, long _hours_from_now)
        {
            return (string [])session.proxy.vmss_get_alerts(session.uuid, (_vmss != null) ? _vmss : "", _hours_from_now.ToString()).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the value to set</param>
        public static void set_schedule_snapshot_retention_value(Session session, string _vmss, long _value)
        {
            session.proxy.vmss_set_schedule_snapshot_retention_value(session.uuid, (_vmss != null) ? _vmss : "", _value.ToString()).parse();
        }

        /// <summary>
        /// Set the value of the schedule_snapshot_frequency field
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the schedule snapshot frequency</param>
        public static void set_schedule_snapshot_frequency(Session session, string _vmss, vmss_schedule_snapshot_frequency _value)
        {
            session.proxy.vmss_set_schedule_snapshot_frequency(session.uuid, (_vmss != null) ? _vmss : "", vmss_schedule_snapshot_frequency_helper.ToString(_value)).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the value to set</param>
        public static void set_snapshot_schedule(Session session, string _vmss, Dictionary<string, string> _value)
        {
            session.proxy.vmss_set_snapshot_schedule(session.uuid, (_vmss != null) ? _vmss : "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        /// <summary>
        /// Set the value of the is_alarm_enabled field
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">true if alarm is enabled for this schedule snapshot</param>
        public static void set_is_alarm_enabled(Session session, string _vmss, bool _value)
        {
            session.proxy.vmss_set_is_alarm_enabled(session.uuid, (_vmss != null) ? _vmss : "", _value).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the value to set</param>
        public static void set_alarm_config(Session session, string _vmss, Dictionary<string, string> _value)
        {
            session.proxy.vmss_set_alarm_config(session.uuid, (_vmss != null) ? _vmss : "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        public static void add_to_snapshot_schedule(Session session, string _vmss, string _key, string _value)
        {
            session.proxy.vmss_add_to_snapshot_schedule(session.uuid, (_vmss != null) ? _vmss : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_key">the key to add</param>
        /// <param name="_value">the value to add</param>
        public static void add_to_alarm_config(Session session, string _vmss, string _key, string _value)
        {
            session.proxy.vmss_add_to_alarm_config(session.uuid, (_vmss != null) ? _vmss : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_key">the key to remove</param>
        public static void remove_from_snapshot_schedule(Session session, string _vmss, string _key)
        {
            session.proxy.vmss_remove_from_snapshot_schedule(session.uuid, (_vmss != null) ? _vmss : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_key">the key to remove</param>
        public static void remove_from_alarm_config(Session session, string _vmss, string _key)
        {
            session.proxy.vmss_remove_from_alarm_config(session.uuid, (_vmss != null) ? _vmss : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the value to set</param>
        public static void set_schedule_snapshot_last_run_time(Session session, string _vmss, DateTime _value)
        {
            session.proxy.vmss_set_schedule_snapshot_last_run_time(session.uuid, (_vmss != null) ? _vmss : "", _value).parse();
        }

        /// <summary>
        /// 
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_vmss">The opaque_ref of the given vmss</param>
        /// <param name="_value">the schedule snapshot type</param>
        public static void set_schedule_snapshot_type(Session session, string _vmss, vmss_schedule_snapshot_type _value)
        {
            session.proxy.vmss_set_schedule_snapshot_type(session.uuid, (_vmss != null) ? _vmss : "", vmss_schedule_snapshot_type_helper.ToString(_value)).parse();
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
        /// enable or disable this schedule snapshot
        /// </summary>
        public virtual bool is_schedule_snapshot_enabled
        {
            get { return _is_schedule_snapshot_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _is_schedule_snapshot_enabled))
                {
                    _is_schedule_snapshot_enabled = value;
                    Changed = true;
                    NotifyPropertyChanged("is_schedule_snapshot_enabled");
                }
            }
        }
        private bool _is_schedule_snapshot_enabled;

        /// <summary>
        /// type of the snapshot schedule
        /// </summary>
        public virtual vmss_schedule_snapshot_type schedule_snapshot_type
        {
            get { return _schedule_snapshot_type; }
            set
            {
                if (!Helper.AreEqual(value, _schedule_snapshot_type))
                {
                    _schedule_snapshot_type = value;
                    Changed = true;
                    NotifyPropertyChanged("schedule_snapshot_type");
                }
            }
        }
        private vmss_schedule_snapshot_type _schedule_snapshot_type;

        /// <summary>
        /// maximum number of snapshots that should be stored at any time
        /// </summary>
        public virtual long schedule_snapshot_retention_value
        {
            get { return _schedule_snapshot_retention_value; }
            set
            {
                if (!Helper.AreEqual(value, _schedule_snapshot_retention_value))
                {
                    _schedule_snapshot_retention_value = value;
                    Changed = true;
                    NotifyPropertyChanged("schedule_snapshot_retention_value");
                }
            }
        }
        private long _schedule_snapshot_retention_value;

        /// <summary>
        /// frequency of taking snapshot from snapshot schedule
        /// </summary>
        public virtual vmss_schedule_snapshot_frequency schedule_snapshot_frequency
        {
            get { return _schedule_snapshot_frequency; }
            set
            {
                if (!Helper.AreEqual(value, _schedule_snapshot_frequency))
                {
                    _schedule_snapshot_frequency = value;
                    Changed = true;
                    NotifyPropertyChanged("schedule_snapshot_frequency");
                }
            }
        }
        private vmss_schedule_snapshot_frequency _schedule_snapshot_frequency;

        /// <summary>
        /// schedule of the snapshot containing 'hour', 'min', 'days'. Date/time-related information is in Local Timezone
        /// </summary>
        public virtual Dictionary<string, string> snapshot_schedule
        {
            get { return _snapshot_schedule; }
            set
            {
                if (!Helper.AreEqual(value, _snapshot_schedule))
                {
                    _snapshot_schedule = value;
                    Changed = true;
                    NotifyPropertyChanged("snapshot_schedule");
                }
            }
        }
        private Dictionary<string, string> _snapshot_schedule;

        /// <summary>
        /// true if this schedule snapshot is running
        /// </summary>
        public virtual bool is_schedule_snapshot_running
        {
            get { return _is_schedule_snapshot_running; }
            set
            {
                if (!Helper.AreEqual(value, _is_schedule_snapshot_running))
                {
                    _is_schedule_snapshot_running = value;
                    Changed = true;
                    NotifyPropertyChanged("is_schedule_snapshot_running");
                }
            }
        }
        private bool _is_schedule_snapshot_running;

        /// <summary>
        /// time of the last snapshot
        /// </summary>
        public virtual DateTime schedule_snapshot_last_run_time
        {
            get { return _schedule_snapshot_last_run_time; }
            set
            {
                if (!Helper.AreEqual(value, _schedule_snapshot_last_run_time))
                {
                    _schedule_snapshot_last_run_time = value;
                    Changed = true;
                    NotifyPropertyChanged("schedule_snapshot_last_run_time");
                }
            }
        }
        private DateTime _schedule_snapshot_last_run_time;

        /// <summary>
        /// all VMs attached to this schedule snapshot
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

        /// <summary>
        /// true if alarm is enabled for this schedule snapshot
        /// </summary>
        public virtual bool is_alarm_enabled
        {
            get { return _is_alarm_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _is_alarm_enabled))
                {
                    _is_alarm_enabled = value;
                    Changed = true;
                    NotifyPropertyChanged("is_alarm_enabled");
                }
            }
        }
        private bool _is_alarm_enabled;

        /// <summary>
        /// configuration for the alarm
        /// </summary>
        public virtual Dictionary<string, string> alarm_config
        {
            get { return _alarm_config; }
            set
            {
                if (!Helper.AreEqual(value, _alarm_config))
                {
                    _alarm_config = value;
                    Changed = true;
                    NotifyPropertyChanged("alarm_config");
                }
            }
        }
        private Dictionary<string, string> _alarm_config;

        /// <summary>
        /// recent alerts
        /// </summary>
        public virtual string[] recent_alerts
        {
            get { return _recent_alerts; }
            set
            {
                if (!Helper.AreEqual(value, _recent_alerts))
                {
                    _recent_alerts = value;
                    Changed = true;
                    NotifyPropertyChanged("recent_alerts");
                }
            }
        }
        private string[] _recent_alerts;
    }
}
