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
    public partial class VMPP : XenObject<VMPP>
    {
        public VMPP()
        {
        }

        public VMPP(string uuid,
            string name_label,
            string name_description,
            bool is_policy_enabled,
            vmpp_backup_type backup_type,
            long backup_retention_value,
            vmpp_backup_frequency backup_frequency,
            Dictionary<string, string> backup_schedule,
            bool is_backup_running,
            DateTime backup_last_run_time,
            vmpp_archive_target_type archive_target_type,
            Dictionary<string, string> archive_target_config,
            vmpp_archive_frequency archive_frequency,
            Dictionary<string, string> archive_schedule,
            bool is_archive_running,
            DateTime archive_last_run_time,
            List<XenRef<VM>> VMs,
            bool is_alarm_enabled,
            Dictionary<string, string> alarm_config,
            string[] recent_alerts)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.is_policy_enabled = is_policy_enabled;
            this.backup_type = backup_type;
            this.backup_retention_value = backup_retention_value;
            this.backup_frequency = backup_frequency;
            this.backup_schedule = backup_schedule;
            this.is_backup_running = is_backup_running;
            this.backup_last_run_time = backup_last_run_time;
            this.archive_target_type = archive_target_type;
            this.archive_target_config = archive_target_config;
            this.archive_frequency = archive_frequency;
            this.archive_schedule = archive_schedule;
            this.is_archive_running = is_archive_running;
            this.archive_last_run_time = archive_last_run_time;
            this.VMs = VMs;
            this.is_alarm_enabled = is_alarm_enabled;
            this.alarm_config = alarm_config;
            this.recent_alerts = recent_alerts;
        }

        /// <summary>
        /// Creates a new VMPP from a Proxy_VMPP.
        /// </summary>
        /// <param name="proxy"></param>
        public VMPP(Proxy_VMPP proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(VMPP update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            is_policy_enabled = update.is_policy_enabled;
            backup_type = update.backup_type;
            backup_retention_value = update.backup_retention_value;
            backup_frequency = update.backup_frequency;
            backup_schedule = update.backup_schedule;
            is_backup_running = update.is_backup_running;
            backup_last_run_time = update.backup_last_run_time;
            archive_target_type = update.archive_target_type;
            archive_target_config = update.archive_target_config;
            archive_frequency = update.archive_frequency;
            archive_schedule = update.archive_schedule;
            is_archive_running = update.is_archive_running;
            archive_last_run_time = update.archive_last_run_time;
            VMs = update.VMs;
            is_alarm_enabled = update.is_alarm_enabled;
            alarm_config = update.alarm_config;
            recent_alerts = update.recent_alerts;
        }

        internal void UpdateFromProxy(Proxy_VMPP proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            is_policy_enabled = (bool)proxy.is_policy_enabled;
            backup_type = proxy.backup_type == null ? (vmpp_backup_type) 0 : (vmpp_backup_type)Helper.EnumParseDefault(typeof(vmpp_backup_type), (string)proxy.backup_type);
            backup_retention_value = proxy.backup_retention_value == null ? 0 : long.Parse((string)proxy.backup_retention_value);
            backup_frequency = proxy.backup_frequency == null ? (vmpp_backup_frequency) 0 : (vmpp_backup_frequency)Helper.EnumParseDefault(typeof(vmpp_backup_frequency), (string)proxy.backup_frequency);
            backup_schedule = proxy.backup_schedule == null ? null : Maps.convert_from_proxy_string_string(proxy.backup_schedule);
            is_backup_running = (bool)proxy.is_backup_running;
            backup_last_run_time = proxy.backup_last_run_time;
            archive_target_type = proxy.archive_target_type == null ? (vmpp_archive_target_type) 0 : (vmpp_archive_target_type)Helper.EnumParseDefault(typeof(vmpp_archive_target_type), (string)proxy.archive_target_type);
            archive_target_config = proxy.archive_target_config == null ? null : Maps.convert_from_proxy_string_string(proxy.archive_target_config);
            archive_frequency = proxy.archive_frequency == null ? (vmpp_archive_frequency) 0 : (vmpp_archive_frequency)Helper.EnumParseDefault(typeof(vmpp_archive_frequency), (string)proxy.archive_frequency);
            archive_schedule = proxy.archive_schedule == null ? null : Maps.convert_from_proxy_string_string(proxy.archive_schedule);
            is_archive_running = (bool)proxy.is_archive_running;
            archive_last_run_time = proxy.archive_last_run_time;
            VMs = proxy.VMs == null ? null : XenRef<VM>.Create(proxy.VMs);
            is_alarm_enabled = (bool)proxy.is_alarm_enabled;
            alarm_config = proxy.alarm_config == null ? null : Maps.convert_from_proxy_string_string(proxy.alarm_config);
            recent_alerts = proxy.recent_alerts == null ? new string[] {} : (string [])proxy.recent_alerts;
        }

        public Proxy_VMPP ToProxy()
        {
            Proxy_VMPP result_ = new Proxy_VMPP();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.is_policy_enabled = is_policy_enabled;
            result_.backup_type = vmpp_backup_type_helper.ToString(backup_type);
            result_.backup_retention_value = backup_retention_value.ToString();
            result_.backup_frequency = vmpp_backup_frequency_helper.ToString(backup_frequency);
            result_.backup_schedule = Maps.convert_to_proxy_string_string(backup_schedule);
            result_.is_backup_running = is_backup_running;
            result_.backup_last_run_time = backup_last_run_time;
            result_.archive_target_type = vmpp_archive_target_type_helper.ToString(archive_target_type);
            result_.archive_target_config = Maps.convert_to_proxy_string_string(archive_target_config);
            result_.archive_frequency = vmpp_archive_frequency_helper.ToString(archive_frequency);
            result_.archive_schedule = Maps.convert_to_proxy_string_string(archive_schedule);
            result_.is_archive_running = is_archive_running;
            result_.archive_last_run_time = archive_last_run_time;
            result_.VMs = (VMs != null) ? Helper.RefListToStringArray(VMs) : new string[] {};
            result_.is_alarm_enabled = is_alarm_enabled;
            result_.alarm_config = Maps.convert_to_proxy_string_string(alarm_config);
            result_.recent_alerts = recent_alerts;
            return result_;
        }

        /// <summary>
        /// Creates a new VMPP from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public VMPP(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            is_policy_enabled = Marshalling.ParseBool(table, "is_policy_enabled");
            backup_type = (vmpp_backup_type)Helper.EnumParseDefault(typeof(vmpp_backup_type), Marshalling.ParseString(table, "backup_type"));
            backup_retention_value = Marshalling.ParseLong(table, "backup_retention_value");
            backup_frequency = (vmpp_backup_frequency)Helper.EnumParseDefault(typeof(vmpp_backup_frequency), Marshalling.ParseString(table, "backup_frequency"));
            backup_schedule = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "backup_schedule"));
            is_backup_running = Marshalling.ParseBool(table, "is_backup_running");
            backup_last_run_time = Marshalling.ParseDateTime(table, "backup_last_run_time");
            archive_target_type = (vmpp_archive_target_type)Helper.EnumParseDefault(typeof(vmpp_archive_target_type), Marshalling.ParseString(table, "archive_target_type"));
            archive_target_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "archive_target_config"));
            archive_frequency = (vmpp_archive_frequency)Helper.EnumParseDefault(typeof(vmpp_archive_frequency), Marshalling.ParseString(table, "archive_frequency"));
            archive_schedule = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "archive_schedule"));
            is_archive_running = Marshalling.ParseBool(table, "is_archive_running");
            archive_last_run_time = Marshalling.ParseDateTime(table, "archive_last_run_time");
            VMs = Marshalling.ParseSetRef<VM>(table, "VMs");
            is_alarm_enabled = Marshalling.ParseBool(table, "is_alarm_enabled");
            alarm_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "alarm_config"));
            recent_alerts = Marshalling.ParseStringArray(table, "recent_alerts");
        }

        public bool DeepEquals(VMPP other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._is_policy_enabled, other._is_policy_enabled) &&
                Helper.AreEqual2(this._backup_type, other._backup_type) &&
                Helper.AreEqual2(this._backup_retention_value, other._backup_retention_value) &&
                Helper.AreEqual2(this._backup_frequency, other._backup_frequency) &&
                Helper.AreEqual2(this._backup_schedule, other._backup_schedule) &&
                Helper.AreEqual2(this._is_backup_running, other._is_backup_running) &&
                Helper.AreEqual2(this._backup_last_run_time, other._backup_last_run_time) &&
                Helper.AreEqual2(this._archive_target_type, other._archive_target_type) &&
                Helper.AreEqual2(this._archive_target_config, other._archive_target_config) &&
                Helper.AreEqual2(this._archive_frequency, other._archive_frequency) &&
                Helper.AreEqual2(this._archive_schedule, other._archive_schedule) &&
                Helper.AreEqual2(this._is_archive_running, other._is_archive_running) &&
                Helper.AreEqual2(this._archive_last_run_time, other._archive_last_run_time) &&
                Helper.AreEqual2(this._VMs, other._VMs) &&
                Helper.AreEqual2(this._is_alarm_enabled, other._is_alarm_enabled) &&
                Helper.AreEqual2(this._alarm_config, other._alarm_config) &&
                Helper.AreEqual2(this._recent_alerts, other._recent_alerts);
        }

        public override string SaveChanges(Session session, string opaqueRef, VMPP server)
        {
            if (opaqueRef == null)
            {
                Proxy_VMPP p = this.ToProxy();
                return session.proxy.vmpp_create(session.uuid, p).parse();
            }
            else
            {
                if (!Helper.AreEqual2(_name_label, server._name_label))
                {
                    VMPP.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    VMPP.set_name_description(session, opaqueRef, _name_description);
                }
                if (!Helper.AreEqual2(_is_policy_enabled, server._is_policy_enabled))
                {
                    VMPP.set_is_policy_enabled(session, opaqueRef, _is_policy_enabled);
                }
                if (!Helper.AreEqual2(_backup_type, server._backup_type))
                {
                    VMPP.set_backup_type(session, opaqueRef, _backup_type);
                }
                if (!Helper.AreEqual2(_backup_retention_value, server._backup_retention_value))
                {
                    VMPP.set_backup_retention_value(session, opaqueRef, _backup_retention_value);
                }
                if (!Helper.AreEqual2(_backup_frequency, server._backup_frequency))
                {
                    VMPP.set_backup_frequency(session, opaqueRef, _backup_frequency);
                }
                if (!Helper.AreEqual2(_backup_schedule, server._backup_schedule))
                {
                    VMPP.set_backup_schedule(session, opaqueRef, _backup_schedule);
                }
                if (!Helper.AreEqual2(_archive_target_type, server._archive_target_type))
                {
                    VMPP.set_archive_target_type(session, opaqueRef, _archive_target_type);
                }
                if (!Helper.AreEqual2(_archive_target_config, server._archive_target_config))
                {
                    VMPP.set_archive_target_config(session, opaqueRef, _archive_target_config);
                }
                if (!Helper.AreEqual2(_archive_frequency, server._archive_frequency))
                {
                    VMPP.set_archive_frequency(session, opaqueRef, _archive_frequency);
                }
                if (!Helper.AreEqual2(_archive_schedule, server._archive_schedule))
                {
                    VMPP.set_archive_schedule(session, opaqueRef, _archive_schedule);
                }
                if (!Helper.AreEqual2(_is_alarm_enabled, server._is_alarm_enabled))
                {
                    VMPP.set_is_alarm_enabled(session, opaqueRef, _is_alarm_enabled);
                }
                if (!Helper.AreEqual2(_alarm_config, server._alarm_config))
                {
                    VMPP.set_alarm_config(session, opaqueRef, _alarm_config);
                }

                return null;
            }
        }

        public static VMPP get_record(Session session, string _vmpp)
        {
            return new VMPP((Proxy_VMPP)session.proxy.vmpp_get_record(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static XenRef<VMPP> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VMPP>.Create(session.proxy.vmpp_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static XenRef<VMPP> create(Session session, VMPP _record)
        {
            return XenRef<VMPP>.Create(session.proxy.vmpp_create(session.uuid, _record.ToProxy()).parse());
        }

        public static XenRef<Task> async_create(Session session, VMPP _record)
        {
            return XenRef<Task>.Create(session.proxy.async_vmpp_create(session.uuid, _record.ToProxy()).parse());
        }

        public static void destroy(Session session, string _vmpp)
        {
            session.proxy.vmpp_destroy(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _vmpp)
        {
            return XenRef<Task>.Create(session.proxy.async_vmpp_destroy(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static List<XenRef<VMPP>> get_by_name_label(Session session, string _label)
        {
            return XenRef<VMPP>.Create(session.proxy.vmpp_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        public static string get_uuid(Session session, string _vmpp)
        {
            return (string)session.proxy.vmpp_get_uuid(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static string get_name_label(Session session, string _vmpp)
        {
            return (string)session.proxy.vmpp_get_name_label(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static string get_name_description(Session session, string _vmpp)
        {
            return (string)session.proxy.vmpp_get_name_description(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static bool get_is_policy_enabled(Session session, string _vmpp)
        {
            return (bool)session.proxy.vmpp_get_is_policy_enabled(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static vmpp_backup_type get_backup_type(Session session, string _vmpp)
        {
            return (vmpp_backup_type)Helper.EnumParseDefault(typeof(vmpp_backup_type), (string)session.proxy.vmpp_get_backup_type(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static long get_backup_retention_value(Session session, string _vmpp)
        {
            return long.Parse((string)session.proxy.vmpp_get_backup_retention_value(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static vmpp_backup_frequency get_backup_frequency(Session session, string _vmpp)
        {
            return (vmpp_backup_frequency)Helper.EnumParseDefault(typeof(vmpp_backup_frequency), (string)session.proxy.vmpp_get_backup_frequency(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static Dictionary<string, string> get_backup_schedule(Session session, string _vmpp)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vmpp_get_backup_schedule(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static bool get_is_backup_running(Session session, string _vmpp)
        {
            return (bool)session.proxy.vmpp_get_is_backup_running(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static DateTime get_backup_last_run_time(Session session, string _vmpp)
        {
            return session.proxy.vmpp_get_backup_last_run_time(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static vmpp_archive_target_type get_archive_target_type(Session session, string _vmpp)
        {
            return (vmpp_archive_target_type)Helper.EnumParseDefault(typeof(vmpp_archive_target_type), (string)session.proxy.vmpp_get_archive_target_type(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static Dictionary<string, string> get_archive_target_config(Session session, string _vmpp)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vmpp_get_archive_target_config(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static vmpp_archive_frequency get_archive_frequency(Session session, string _vmpp)
        {
            return (vmpp_archive_frequency)Helper.EnumParseDefault(typeof(vmpp_archive_frequency), (string)session.proxy.vmpp_get_archive_frequency(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static Dictionary<string, string> get_archive_schedule(Session session, string _vmpp)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vmpp_get_archive_schedule(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static bool get_is_archive_running(Session session, string _vmpp)
        {
            return (bool)session.proxy.vmpp_get_is_archive_running(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static DateTime get_archive_last_run_time(Session session, string _vmpp)
        {
            return session.proxy.vmpp_get_archive_last_run_time(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static List<XenRef<VM>> get_VMs(Session session, string _vmpp)
        {
            return XenRef<VM>.Create(session.proxy.vmpp_get_vms(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static bool get_is_alarm_enabled(Session session, string _vmpp)
        {
            return (bool)session.proxy.vmpp_get_is_alarm_enabled(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static Dictionary<string, string> get_alarm_config(Session session, string _vmpp)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vmpp_get_alarm_config(session.uuid, (_vmpp != null) ? _vmpp : "").parse());
        }

        public static string[] get_recent_alerts(Session session, string _vmpp)
        {
            return (string [])session.proxy.vmpp_get_recent_alerts(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static void set_name_label(Session session, string _vmpp, string _label)
        {
            session.proxy.vmpp_set_name_label(session.uuid, (_vmpp != null) ? _vmpp : "", (_label != null) ? _label : "").parse();
        }

        public static void set_name_description(Session session, string _vmpp, string _description)
        {
            session.proxy.vmpp_set_name_description(session.uuid, (_vmpp != null) ? _vmpp : "", (_description != null) ? _description : "").parse();
        }

        public static void set_is_policy_enabled(Session session, string _vmpp, bool _is_policy_enabled)
        {
            session.proxy.vmpp_set_is_policy_enabled(session.uuid, (_vmpp != null) ? _vmpp : "", _is_policy_enabled).parse();
        }

        public static void set_backup_type(Session session, string _vmpp, vmpp_backup_type _backup_type)
        {
            session.proxy.vmpp_set_backup_type(session.uuid, (_vmpp != null) ? _vmpp : "", vmpp_backup_type_helper.ToString(_backup_type)).parse();
        }

        public static string protect_now(Session session, string _vmpp)
        {
            return (string)session.proxy.vmpp_protect_now(session.uuid, (_vmpp != null) ? _vmpp : "").parse();
        }

        public static string archive_now(Session session, string _snapshot)
        {
            return (string)session.proxy.vmpp_archive_now(session.uuid, (_snapshot != null) ? _snapshot : "").parse();
        }

        public static string[] get_alerts(Session session, string _vmpp, long _hours_from_now)
        {
            return (string [])session.proxy.vmpp_get_alerts(session.uuid, (_vmpp != null) ? _vmpp : "", _hours_from_now.ToString()).parse();
        }

        public static void set_backup_retention_value(Session session, string _self, long _value)
        {
            session.proxy.vmpp_set_backup_retention_value(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void set_backup_frequency(Session session, string _self, vmpp_backup_frequency _value)
        {
            session.proxy.vmpp_set_backup_frequency(session.uuid, (_self != null) ? _self : "", vmpp_backup_frequency_helper.ToString(_value)).parse();
        }

        public static void set_backup_schedule(Session session, string _self, Dictionary<string, string> _value)
        {
            session.proxy.vmpp_set_backup_schedule(session.uuid, (_self != null) ? _self : "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        public static void set_archive_frequency(Session session, string _self, vmpp_archive_frequency _value)
        {
            session.proxy.vmpp_set_archive_frequency(session.uuid, (_self != null) ? _self : "", vmpp_archive_frequency_helper.ToString(_value)).parse();
        }

        public static void set_archive_schedule(Session session, string _self, Dictionary<string, string> _value)
        {
            session.proxy.vmpp_set_archive_schedule(session.uuid, (_self != null) ? _self : "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        public static void set_archive_target_type(Session session, string _self, vmpp_archive_target_type _value)
        {
            session.proxy.vmpp_set_archive_target_type(session.uuid, (_self != null) ? _self : "", vmpp_archive_target_type_helper.ToString(_value)).parse();
        }

        public static void set_archive_target_config(Session session, string _self, Dictionary<string, string> _value)
        {
            session.proxy.vmpp_set_archive_target_config(session.uuid, (_self != null) ? _self : "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        public static void set_is_alarm_enabled(Session session, string _self, bool _value)
        {
            session.proxy.vmpp_set_is_alarm_enabled(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static void set_alarm_config(Session session, string _self, Dictionary<string, string> _value)
        {
            session.proxy.vmpp_set_alarm_config(session.uuid, (_self != null) ? _self : "", Maps.convert_to_proxy_string_string(_value)).parse();
        }

        public static void add_to_backup_schedule(Session session, string _self, string _key, string _value)
        {
            session.proxy.vmpp_add_to_backup_schedule(session.uuid, (_self != null) ? _self : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void add_to_archive_target_config(Session session, string _self, string _key, string _value)
        {
            session.proxy.vmpp_add_to_archive_target_config(session.uuid, (_self != null) ? _self : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void add_to_archive_schedule(Session session, string _self, string _key, string _value)
        {
            session.proxy.vmpp_add_to_archive_schedule(session.uuid, (_self != null) ? _self : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void add_to_alarm_config(Session session, string _self, string _key, string _value)
        {
            session.proxy.vmpp_add_to_alarm_config(session.uuid, (_self != null) ? _self : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_backup_schedule(Session session, string _self, string _key)
        {
            session.proxy.vmpp_remove_from_backup_schedule(session.uuid, (_self != null) ? _self : "", (_key != null) ? _key : "").parse();
        }

        public static void remove_from_archive_target_config(Session session, string _self, string _key)
        {
            session.proxy.vmpp_remove_from_archive_target_config(session.uuid, (_self != null) ? _self : "", (_key != null) ? _key : "").parse();
        }

        public static void remove_from_archive_schedule(Session session, string _self, string _key)
        {
            session.proxy.vmpp_remove_from_archive_schedule(session.uuid, (_self != null) ? _self : "", (_key != null) ? _key : "").parse();
        }

        public static void remove_from_alarm_config(Session session, string _self, string _key)
        {
            session.proxy.vmpp_remove_from_alarm_config(session.uuid, (_self != null) ? _self : "", (_key != null) ? _key : "").parse();
        }

        public static void set_backup_last_run_time(Session session, string _self, DateTime _value)
        {
            session.proxy.vmpp_set_backup_last_run_time(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static void set_archive_last_run_time(Session session, string _self, DateTime _value)
        {
            session.proxy.vmpp_set_archive_last_run_time(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static List<XenRef<VMPP>> get_all(Session session)
        {
            return XenRef<VMPP>.Create(session.proxy.vmpp_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<VMPP>, VMPP> get_all_records(Session session)
        {
            return XenRef<VMPP>.Create<Proxy_VMPP>(session.proxy.vmpp_get_all_records(session.uuid).parse());
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

        private bool _is_policy_enabled;
        public virtual bool is_policy_enabled {
             get { return _is_policy_enabled; }
             set { if (!Helper.AreEqual(value, _is_policy_enabled)) { _is_policy_enabled = value; Changed = true; NotifyPropertyChanged("is_policy_enabled"); } }
         }

        private vmpp_backup_type _backup_type;
        public virtual vmpp_backup_type backup_type {
             get { return _backup_type; }
             set { if (!Helper.AreEqual(value, _backup_type)) { _backup_type = value; Changed = true; NotifyPropertyChanged("backup_type"); } }
         }

        private long _backup_retention_value;
        public virtual long backup_retention_value {
             get { return _backup_retention_value; }
             set { if (!Helper.AreEqual(value, _backup_retention_value)) { _backup_retention_value = value; Changed = true; NotifyPropertyChanged("backup_retention_value"); } }
         }

        private vmpp_backup_frequency _backup_frequency;
        public virtual vmpp_backup_frequency backup_frequency {
             get { return _backup_frequency; }
             set { if (!Helper.AreEqual(value, _backup_frequency)) { _backup_frequency = value; Changed = true; NotifyPropertyChanged("backup_frequency"); } }
         }

        private Dictionary<string, string> _backup_schedule;
        public virtual Dictionary<string, string> backup_schedule {
             get { return _backup_schedule; }
             set { if (!Helper.AreEqual(value, _backup_schedule)) { _backup_schedule = value; Changed = true; NotifyPropertyChanged("backup_schedule"); } }
         }

        private bool _is_backup_running;
        public virtual bool is_backup_running {
             get { return _is_backup_running; }
             set { if (!Helper.AreEqual(value, _is_backup_running)) { _is_backup_running = value; Changed = true; NotifyPropertyChanged("is_backup_running"); } }
         }

        private DateTime _backup_last_run_time;
        public virtual DateTime backup_last_run_time {
             get { return _backup_last_run_time; }
             set { if (!Helper.AreEqual(value, _backup_last_run_time)) { _backup_last_run_time = value; Changed = true; NotifyPropertyChanged("backup_last_run_time"); } }
         }

        private vmpp_archive_target_type _archive_target_type;
        public virtual vmpp_archive_target_type archive_target_type {
             get { return _archive_target_type; }
             set { if (!Helper.AreEqual(value, _archive_target_type)) { _archive_target_type = value; Changed = true; NotifyPropertyChanged("archive_target_type"); } }
         }

        private Dictionary<string, string> _archive_target_config;
        public virtual Dictionary<string, string> archive_target_config {
             get { return _archive_target_config; }
             set { if (!Helper.AreEqual(value, _archive_target_config)) { _archive_target_config = value; Changed = true; NotifyPropertyChanged("archive_target_config"); } }
         }

        private vmpp_archive_frequency _archive_frequency;
        public virtual vmpp_archive_frequency archive_frequency {
             get { return _archive_frequency; }
             set { if (!Helper.AreEqual(value, _archive_frequency)) { _archive_frequency = value; Changed = true; NotifyPropertyChanged("archive_frequency"); } }
         }

        private Dictionary<string, string> _archive_schedule;
        public virtual Dictionary<string, string> archive_schedule {
             get { return _archive_schedule; }
             set { if (!Helper.AreEqual(value, _archive_schedule)) { _archive_schedule = value; Changed = true; NotifyPropertyChanged("archive_schedule"); } }
         }

        private bool _is_archive_running;
        public virtual bool is_archive_running {
             get { return _is_archive_running; }
             set { if (!Helper.AreEqual(value, _is_archive_running)) { _is_archive_running = value; Changed = true; NotifyPropertyChanged("is_archive_running"); } }
         }

        private DateTime _archive_last_run_time;
        public virtual DateTime archive_last_run_time {
             get { return _archive_last_run_time; }
             set { if (!Helper.AreEqual(value, _archive_last_run_time)) { _archive_last_run_time = value; Changed = true; NotifyPropertyChanged("archive_last_run_time"); } }
         }

        private List<XenRef<VM>> _VMs;
        public virtual List<XenRef<VM>> VMs {
             get { return _VMs; }
             set { if (!Helper.AreEqual(value, _VMs)) { _VMs = value; Changed = true; NotifyPropertyChanged("VMs"); } }
         }

        private bool _is_alarm_enabled;
        public virtual bool is_alarm_enabled {
             get { return _is_alarm_enabled; }
             set { if (!Helper.AreEqual(value, _is_alarm_enabled)) { _is_alarm_enabled = value; Changed = true; NotifyPropertyChanged("is_alarm_enabled"); } }
         }

        private Dictionary<string, string> _alarm_config;
        public virtual Dictionary<string, string> alarm_config {
             get { return _alarm_config; }
             set { if (!Helper.AreEqual(value, _alarm_config)) { _alarm_config = value; Changed = true; NotifyPropertyChanged("alarm_config"); } }
         }

        private string[] _recent_alerts;
        public virtual string[] recent_alerts {
             get { return _recent_alerts; }
             set { if (!Helper.AreEqual(value, _recent_alerts)) { _recent_alerts = value; Changed = true; NotifyPropertyChanged("recent_alerts"); } }
         }


    }
}
