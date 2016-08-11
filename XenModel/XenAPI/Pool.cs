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
    /// Pool-wide information
    /// First published in XenServer 4.0.
    /// </summary>
    public partial class Pool : XenObject<Pool>
    {
        public Pool()
        {
        }

        public Pool(string uuid,
            string name_label,
            string name_description,
            XenRef<Host> master,
            XenRef<SR> default_SR,
            XenRef<SR> suspend_image_SR,
            XenRef<SR> crash_dump_SR,
            Dictionary<string, string> other_config,
            bool ha_enabled,
            Dictionary<string, string> ha_configuration,
            string[] ha_statefiles,
            long ha_host_failures_to_tolerate,
            long ha_plan_exists_for,
            bool ha_allow_overcommit,
            bool ha_overcommitted,
            Dictionary<string, XenRef<Blob>> blobs,
            string[] tags,
            Dictionary<string, string> gui_config,
            Dictionary<string, string> health_check_config,
            string wlb_url,
            string wlb_username,
            bool wlb_enabled,
            bool wlb_verify_cert,
            bool redo_log_enabled,
            XenRef<VDI> redo_log_vdi,
            string vswitch_controller,
            Dictionary<string, string> restrictions,
            List<XenRef<VDI>> metadata_VDIs,
            string ha_cluster_stack,
            List<pool_allowed_operations> allowed_operations,
            Dictionary<string, pool_allowed_operations> current_operations,
            Dictionary<string, string> guest_agent_config,
            Dictionary<string, string> cpu_info,
            bool policy_no_vendor_device,
            bool live_patching_disabled)
        {
            this.uuid = uuid;
            this.name_label = name_label;
            this.name_description = name_description;
            this.master = master;
            this.default_SR = default_SR;
            this.suspend_image_SR = suspend_image_SR;
            this.crash_dump_SR = crash_dump_SR;
            this.other_config = other_config;
            this.ha_enabled = ha_enabled;
            this.ha_configuration = ha_configuration;
            this.ha_statefiles = ha_statefiles;
            this.ha_host_failures_to_tolerate = ha_host_failures_to_tolerate;
            this.ha_plan_exists_for = ha_plan_exists_for;
            this.ha_allow_overcommit = ha_allow_overcommit;
            this.ha_overcommitted = ha_overcommitted;
            this.blobs = blobs;
            this.tags = tags;
            this.gui_config = gui_config;
            this.health_check_config = health_check_config;
            this.wlb_url = wlb_url;
            this.wlb_username = wlb_username;
            this.wlb_enabled = wlb_enabled;
            this.wlb_verify_cert = wlb_verify_cert;
            this.redo_log_enabled = redo_log_enabled;
            this.redo_log_vdi = redo_log_vdi;
            this.vswitch_controller = vswitch_controller;
            this.restrictions = restrictions;
            this.metadata_VDIs = metadata_VDIs;
            this.ha_cluster_stack = ha_cluster_stack;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.guest_agent_config = guest_agent_config;
            this.cpu_info = cpu_info;
            this.policy_no_vendor_device = policy_no_vendor_device;
            this.live_patching_disabled = live_patching_disabled;
        }

        /// <summary>
        /// Creates a new Pool from a Proxy_Pool.
        /// </summary>
        /// <param name="proxy"></param>
        public Pool(Proxy_Pool proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(Pool update)
        {
            uuid = update.uuid;
            name_label = update.name_label;
            name_description = update.name_description;
            master = update.master;
            default_SR = update.default_SR;
            suspend_image_SR = update.suspend_image_SR;
            crash_dump_SR = update.crash_dump_SR;
            other_config = update.other_config;
            ha_enabled = update.ha_enabled;
            ha_configuration = update.ha_configuration;
            ha_statefiles = update.ha_statefiles;
            ha_host_failures_to_tolerate = update.ha_host_failures_to_tolerate;
            ha_plan_exists_for = update.ha_plan_exists_for;
            ha_allow_overcommit = update.ha_allow_overcommit;
            ha_overcommitted = update.ha_overcommitted;
            blobs = update.blobs;
            tags = update.tags;
            gui_config = update.gui_config;
            health_check_config = update.health_check_config;
            wlb_url = update.wlb_url;
            wlb_username = update.wlb_username;
            wlb_enabled = update.wlb_enabled;
            wlb_verify_cert = update.wlb_verify_cert;
            redo_log_enabled = update.redo_log_enabled;
            redo_log_vdi = update.redo_log_vdi;
            vswitch_controller = update.vswitch_controller;
            restrictions = update.restrictions;
            metadata_VDIs = update.metadata_VDIs;
            ha_cluster_stack = update.ha_cluster_stack;
            allowed_operations = update.allowed_operations;
            current_operations = update.current_operations;
            guest_agent_config = update.guest_agent_config;
            cpu_info = update.cpu_info;
            policy_no_vendor_device = update.policy_no_vendor_device;
            live_patching_disabled = update.live_patching_disabled;
        }

        internal void UpdateFromProxy(Proxy_Pool proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            master = proxy.master == null ? null : XenRef<Host>.Create(proxy.master);
            default_SR = proxy.default_SR == null ? null : XenRef<SR>.Create(proxy.default_SR);
            suspend_image_SR = proxy.suspend_image_SR == null ? null : XenRef<SR>.Create(proxy.suspend_image_SR);
            crash_dump_SR = proxy.crash_dump_SR == null ? null : XenRef<SR>.Create(proxy.crash_dump_SR);
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            ha_enabled = (bool)proxy.ha_enabled;
            ha_configuration = proxy.ha_configuration == null ? null : Maps.convert_from_proxy_string_string(proxy.ha_configuration);
            ha_statefiles = proxy.ha_statefiles == null ? new string[] {} : (string [])proxy.ha_statefiles;
            ha_host_failures_to_tolerate = proxy.ha_host_failures_to_tolerate == null ? 0 : long.Parse((string)proxy.ha_host_failures_to_tolerate);
            ha_plan_exists_for = proxy.ha_plan_exists_for == null ? 0 : long.Parse((string)proxy.ha_plan_exists_for);
            ha_allow_overcommit = (bool)proxy.ha_allow_overcommit;
            ha_overcommitted = (bool)proxy.ha_overcommitted;
            blobs = proxy.blobs == null ? null : Maps.convert_from_proxy_string_XenRefBlob(proxy.blobs);
            tags = proxy.tags == null ? new string[] {} : (string [])proxy.tags;
            gui_config = proxy.gui_config == null ? null : Maps.convert_from_proxy_string_string(proxy.gui_config);
            health_check_config = proxy.health_check_config == null ? null : Maps.convert_from_proxy_string_string(proxy.health_check_config);
            wlb_url = proxy.wlb_url == null ? null : (string)proxy.wlb_url;
            wlb_username = proxy.wlb_username == null ? null : (string)proxy.wlb_username;
            wlb_enabled = (bool)proxy.wlb_enabled;
            wlb_verify_cert = (bool)proxy.wlb_verify_cert;
            redo_log_enabled = (bool)proxy.redo_log_enabled;
            redo_log_vdi = proxy.redo_log_vdi == null ? null : XenRef<VDI>.Create(proxy.redo_log_vdi);
            vswitch_controller = proxy.vswitch_controller == null ? null : (string)proxy.vswitch_controller;
            restrictions = proxy.restrictions == null ? null : Maps.convert_from_proxy_string_string(proxy.restrictions);
            metadata_VDIs = proxy.metadata_VDIs == null ? null : XenRef<VDI>.Create(proxy.metadata_VDIs);
            ha_cluster_stack = proxy.ha_cluster_stack == null ? null : (string)proxy.ha_cluster_stack;
            allowed_operations = proxy.allowed_operations == null ? null : Helper.StringArrayToEnumList<pool_allowed_operations>(proxy.allowed_operations);
            current_operations = proxy.current_operations == null ? null : Maps.convert_from_proxy_string_pool_allowed_operations(proxy.current_operations);
            guest_agent_config = proxy.guest_agent_config == null ? null : Maps.convert_from_proxy_string_string(proxy.guest_agent_config);
            cpu_info = proxy.cpu_info == null ? null : Maps.convert_from_proxy_string_string(proxy.cpu_info);
            policy_no_vendor_device = (bool)proxy.policy_no_vendor_device;
            live_patching_disabled = (bool)proxy.live_patching_disabled;
        }

        public Proxy_Pool ToProxy()
        {
            Proxy_Pool result_ = new Proxy_Pool();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.master = (master != null) ? master : "";
            result_.default_SR = (default_SR != null) ? default_SR : "";
            result_.suspend_image_SR = (suspend_image_SR != null) ? suspend_image_SR : "";
            result_.crash_dump_SR = (crash_dump_SR != null) ? crash_dump_SR : "";
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.ha_enabled = ha_enabled;
            result_.ha_configuration = Maps.convert_to_proxy_string_string(ha_configuration);
            result_.ha_statefiles = ha_statefiles;
            result_.ha_host_failures_to_tolerate = ha_host_failures_to_tolerate.ToString();
            result_.ha_plan_exists_for = ha_plan_exists_for.ToString();
            result_.ha_allow_overcommit = ha_allow_overcommit;
            result_.ha_overcommitted = ha_overcommitted;
            result_.blobs = Maps.convert_to_proxy_string_XenRefBlob(blobs);
            result_.tags = tags;
            result_.gui_config = Maps.convert_to_proxy_string_string(gui_config);
            result_.health_check_config = Maps.convert_to_proxy_string_string(health_check_config);
            result_.wlb_url = (wlb_url != null) ? wlb_url : "";
            result_.wlb_username = (wlb_username != null) ? wlb_username : "";
            result_.wlb_enabled = wlb_enabled;
            result_.wlb_verify_cert = wlb_verify_cert;
            result_.redo_log_enabled = redo_log_enabled;
            result_.redo_log_vdi = (redo_log_vdi != null) ? redo_log_vdi : "";
            result_.vswitch_controller = (vswitch_controller != null) ? vswitch_controller : "";
            result_.restrictions = Maps.convert_to_proxy_string_string(restrictions);
            result_.metadata_VDIs = (metadata_VDIs != null) ? Helper.RefListToStringArray(metadata_VDIs) : new string[] {};
            result_.ha_cluster_stack = (ha_cluster_stack != null) ? ha_cluster_stack : "";
            result_.allowed_operations = (allowed_operations != null) ? Helper.ObjectListToStringArray(allowed_operations) : new string[] {};
            result_.current_operations = Maps.convert_to_proxy_string_pool_allowed_operations(current_operations);
            result_.guest_agent_config = Maps.convert_to_proxy_string_string(guest_agent_config);
            result_.cpu_info = Maps.convert_to_proxy_string_string(cpu_info);
            result_.policy_no_vendor_device = policy_no_vendor_device;
            result_.live_patching_disabled = live_patching_disabled;
            return result_;
        }

        /// <summary>
        /// Creates a new Pool from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public Pool(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            master = Marshalling.ParseRef<Host>(table, "master");
            default_SR = Marshalling.ParseRef<SR>(table, "default_SR");
            suspend_image_SR = Marshalling.ParseRef<SR>(table, "suspend_image_SR");
            crash_dump_SR = Marshalling.ParseRef<SR>(table, "crash_dump_SR");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            ha_enabled = Marshalling.ParseBool(table, "ha_enabled");
            ha_configuration = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "ha_configuration"));
            ha_statefiles = Marshalling.ParseStringArray(table, "ha_statefiles");
            ha_host_failures_to_tolerate = Marshalling.ParseLong(table, "ha_host_failures_to_tolerate");
            ha_plan_exists_for = Marshalling.ParseLong(table, "ha_plan_exists_for");
            ha_allow_overcommit = Marshalling.ParseBool(table, "ha_allow_overcommit");
            ha_overcommitted = Marshalling.ParseBool(table, "ha_overcommitted");
            blobs = Maps.convert_from_proxy_string_XenRefBlob(Marshalling.ParseHashTable(table, "blobs"));
            tags = Marshalling.ParseStringArray(table, "tags");
            gui_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "gui_config"));
            health_check_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "health_check_config"));
            wlb_url = Marshalling.ParseString(table, "wlb_url");
            wlb_username = Marshalling.ParseString(table, "wlb_username");
            wlb_enabled = Marshalling.ParseBool(table, "wlb_enabled");
            wlb_verify_cert = Marshalling.ParseBool(table, "wlb_verify_cert");
            redo_log_enabled = Marshalling.ParseBool(table, "redo_log_enabled");
            redo_log_vdi = Marshalling.ParseRef<VDI>(table, "redo_log_vdi");
            vswitch_controller = Marshalling.ParseString(table, "vswitch_controller");
            restrictions = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "restrictions"));
            metadata_VDIs = Marshalling.ParseSetRef<VDI>(table, "metadata_VDIs");
            ha_cluster_stack = Marshalling.ParseString(table, "ha_cluster_stack");
            allowed_operations = Helper.StringArrayToEnumList<pool_allowed_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            current_operations = Maps.convert_from_proxy_string_pool_allowed_operations(Marshalling.ParseHashTable(table, "current_operations"));
            guest_agent_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "guest_agent_config"));
            cpu_info = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "cpu_info"));
            policy_no_vendor_device = Marshalling.ParseBool(table, "policy_no_vendor_device");
            live_patching_disabled = Marshalling.ParseBool(table, "live_patching_disabled");
        }

        public bool DeepEquals(Pool other, bool ignoreCurrentOperations)
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
                Helper.AreEqual2(this._master, other._master) &&
                Helper.AreEqual2(this._default_SR, other._default_SR) &&
                Helper.AreEqual2(this._suspend_image_SR, other._suspend_image_SR) &&
                Helper.AreEqual2(this._crash_dump_SR, other._crash_dump_SR) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._ha_enabled, other._ha_enabled) &&
                Helper.AreEqual2(this._ha_configuration, other._ha_configuration) &&
                Helper.AreEqual2(this._ha_statefiles, other._ha_statefiles) &&
                Helper.AreEqual2(this._ha_host_failures_to_tolerate, other._ha_host_failures_to_tolerate) &&
                Helper.AreEqual2(this._ha_plan_exists_for, other._ha_plan_exists_for) &&
                Helper.AreEqual2(this._ha_allow_overcommit, other._ha_allow_overcommit) &&
                Helper.AreEqual2(this._ha_overcommitted, other._ha_overcommitted) &&
                Helper.AreEqual2(this._blobs, other._blobs) &&
                Helper.AreEqual2(this._tags, other._tags) &&
                Helper.AreEqual2(this._gui_config, other._gui_config) &&
                Helper.AreEqual2(this._health_check_config, other._health_check_config) &&
                Helper.AreEqual2(this._wlb_url, other._wlb_url) &&
                Helper.AreEqual2(this._wlb_username, other._wlb_username) &&
                Helper.AreEqual2(this._wlb_enabled, other._wlb_enabled) &&
                Helper.AreEqual2(this._wlb_verify_cert, other._wlb_verify_cert) &&
                Helper.AreEqual2(this._redo_log_enabled, other._redo_log_enabled) &&
                Helper.AreEqual2(this._redo_log_vdi, other._redo_log_vdi) &&
                Helper.AreEqual2(this._vswitch_controller, other._vswitch_controller) &&
                Helper.AreEqual2(this._restrictions, other._restrictions) &&
                Helper.AreEqual2(this._metadata_VDIs, other._metadata_VDIs) &&
                Helper.AreEqual2(this._ha_cluster_stack, other._ha_cluster_stack) &&
                Helper.AreEqual2(this._allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(this._guest_agent_config, other._guest_agent_config) &&
                Helper.AreEqual2(this._cpu_info, other._cpu_info) &&
                Helper.AreEqual2(this._policy_no_vendor_device, other._policy_no_vendor_device) &&
                Helper.AreEqual2(this._live_patching_disabled, other._live_patching_disabled);
        }

        public override string SaveChanges(Session session, string opaqueRef, Pool server)
        {
            if (opaqueRef == null)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot create instances of this type on the server");
                return "";
            }
            else
            {
                if (!Helper.AreEqual2(_name_label, server._name_label))
                {
                    Pool.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    Pool.set_name_description(session, opaqueRef, _name_description);
                }
                if (!Helper.AreEqual2(_default_SR, server._default_SR))
                {
                    Pool.set_default_SR(session, opaqueRef, _default_SR);
                }
                if (!Helper.AreEqual2(_suspend_image_SR, server._suspend_image_SR))
                {
                    Pool.set_suspend_image_SR(session, opaqueRef, _suspend_image_SR);
                }
                if (!Helper.AreEqual2(_crash_dump_SR, server._crash_dump_SR))
                {
                    Pool.set_crash_dump_SR(session, opaqueRef, _crash_dump_SR);
                }
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    Pool.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_ha_allow_overcommit, server._ha_allow_overcommit))
                {
                    Pool.set_ha_allow_overcommit(session, opaqueRef, _ha_allow_overcommit);
                }
                if (!Helper.AreEqual2(_tags, server._tags))
                {
                    Pool.set_tags(session, opaqueRef, _tags);
                }
                if (!Helper.AreEqual2(_gui_config, server._gui_config))
                {
                    Pool.set_gui_config(session, opaqueRef, _gui_config);
                }
                if (!Helper.AreEqual2(_health_check_config, server._health_check_config))
                {
                    Pool.set_health_check_config(session, opaqueRef, _health_check_config);
                }
                if (!Helper.AreEqual2(_wlb_enabled, server._wlb_enabled))
                {
                    Pool.set_wlb_enabled(session, opaqueRef, _wlb_enabled);
                }
                if (!Helper.AreEqual2(_wlb_verify_cert, server._wlb_verify_cert))
                {
                    Pool.set_wlb_verify_cert(session, opaqueRef, _wlb_verify_cert);
                }
                if (!Helper.AreEqual2(_policy_no_vendor_device, server._policy_no_vendor_device))
                {
                    Pool.set_policy_no_vendor_device(session, opaqueRef, _policy_no_vendor_device);
                }
                if (!Helper.AreEqual2(_live_patching_disabled, server._live_patching_disabled))
                {
                    Pool.set_live_patching_disabled(session, opaqueRef, _live_patching_disabled);
                }

                return null;
            }
        }
        /// <summary>
        /// Get a record containing the current state of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static Pool get_record(Session session, string _pool)
        {
            return new Pool((Proxy_Pool)session.proxy.pool_get_record(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get a reference to the pool instance with the specified UUID.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_uuid">UUID of object to return</param>
        public static XenRef<Pool> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<Pool>.Create(session.proxy.pool_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        /// <summary>
        /// Get the uuid field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static string get_uuid(Session session, string _pool)
        {
            return (string)session.proxy.pool_get_uuid(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the name_label field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static string get_name_label(Session session, string _pool)
        {
            return (string)session.proxy.pool_get_name_label(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the name_description field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static string get_name_description(Session session, string _pool)
        {
            return (string)session.proxy.pool_get_name_description(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the master field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static XenRef<Host> get_master(Session session, string _pool)
        {
            return XenRef<Host>.Create(session.proxy.pool_get_master(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the default_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static XenRef<SR> get_default_SR(Session session, string _pool)
        {
            return XenRef<SR>.Create(session.proxy.pool_get_default_sr(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the suspend_image_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static XenRef<SR> get_suspend_image_SR(Session session, string _pool)
        {
            return XenRef<SR>.Create(session.proxy.pool_get_suspend_image_sr(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the crash_dump_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static XenRef<SR> get_crash_dump_SR(Session session, string _pool)
        {
            return XenRef<SR>.Create(session.proxy.pool_get_crash_dump_sr(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the other_config field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static Dictionary<string, string> get_other_config(Session session, string _pool)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pool_get_other_config(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the ha_enabled field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static bool get_ha_enabled(Session session, string _pool)
        {
            return (bool)session.proxy.pool_get_ha_enabled(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the ha_configuration field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static Dictionary<string, string> get_ha_configuration(Session session, string _pool)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pool_get_ha_configuration(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the ha_statefiles field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static string[] get_ha_statefiles(Session session, string _pool)
        {
            return (string [])session.proxy.pool_get_ha_statefiles(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the ha_host_failures_to_tolerate field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static long get_ha_host_failures_to_tolerate(Session session, string _pool)
        {
            return long.Parse((string)session.proxy.pool_get_ha_host_failures_to_tolerate(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the ha_plan_exists_for field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static long get_ha_plan_exists_for(Session session, string _pool)
        {
            return long.Parse((string)session.proxy.pool_get_ha_plan_exists_for(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the ha_allow_overcommit field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static bool get_ha_allow_overcommit(Session session, string _pool)
        {
            return (bool)session.proxy.pool_get_ha_allow_overcommit(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the ha_overcommitted field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static bool get_ha_overcommitted(Session session, string _pool)
        {
            return (bool)session.proxy.pool_get_ha_overcommitted(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the blobs field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static Dictionary<string, XenRef<Blob>> get_blobs(Session session, string _pool)
        {
            return Maps.convert_from_proxy_string_XenRefBlob(session.proxy.pool_get_blobs(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the tags field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static string[] get_tags(Session session, string _pool)
        {
            return (string [])session.proxy.pool_get_tags(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the gui_config field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static Dictionary<string, string> get_gui_config(Session session, string _pool)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pool_get_gui_config(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the health_check_config field of the given pool.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static Dictionary<string, string> get_health_check_config(Session session, string _pool)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pool_get_health_check_config(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the wlb_url field of the given pool.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static string get_wlb_url(Session session, string _pool)
        {
            return (string)session.proxy.pool_get_wlb_url(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the wlb_username field of the given pool.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static string get_wlb_username(Session session, string _pool)
        {
            return (string)session.proxy.pool_get_wlb_username(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the wlb_enabled field of the given pool.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static bool get_wlb_enabled(Session session, string _pool)
        {
            return (bool)session.proxy.pool_get_wlb_enabled(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the wlb_verify_cert field of the given pool.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static bool get_wlb_verify_cert(Session session, string _pool)
        {
            return (bool)session.proxy.pool_get_wlb_verify_cert(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the redo_log_enabled field of the given pool.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static bool get_redo_log_enabled(Session session, string _pool)
        {
            return (bool)session.proxy.pool_get_redo_log_enabled(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the redo_log_vdi field of the given pool.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static XenRef<VDI> get_redo_log_vdi(Session session, string _pool)
        {
            return XenRef<VDI>.Create(session.proxy.pool_get_redo_log_vdi(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the vswitch_controller field of the given pool.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static string get_vswitch_controller(Session session, string _pool)
        {
            return (string)session.proxy.pool_get_vswitch_controller(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the restrictions field of the given pool.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static Dictionary<string, string> get_restrictions(Session session, string _pool)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pool_get_restrictions(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the metadata_VDIs field of the given pool.
        /// First published in XenServer 6.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static List<XenRef<VDI>> get_metadata_VDIs(Session session, string _pool)
        {
            return XenRef<VDI>.Create(session.proxy.pool_get_metadata_vdis(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the ha_cluster_stack field of the given pool.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static string get_ha_cluster_stack(Session session, string _pool)
        {
            return (string)session.proxy.pool_get_ha_cluster_stack(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the allowed_operations field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static List<pool_allowed_operations> get_allowed_operations(Session session, string _pool)
        {
            return Helper.StringArrayToEnumList<pool_allowed_operations>(session.proxy.pool_get_allowed_operations(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the current_operations field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static Dictionary<string, pool_allowed_operations> get_current_operations(Session session, string _pool)
        {
            return Maps.convert_from_proxy_string_pool_allowed_operations(session.proxy.pool_get_current_operations(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the guest_agent_config field of the given pool.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static Dictionary<string, string> get_guest_agent_config(Session session, string _pool)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pool_get_guest_agent_config(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the cpu_info field of the given pool.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static Dictionary<string, string> get_cpu_info(Session session, string _pool)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pool_get_cpu_info(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Get the policy_no_vendor_device field of the given pool.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static bool get_policy_no_vendor_device(Session session, string _pool)
        {
            return (bool)session.proxy.pool_get_policy_no_vendor_device(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Get the live_patching_disabled field of the given pool.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static bool get_live_patching_disabled(Session session, string _pool)
        {
            return (bool)session.proxy.pool_get_live_patching_disabled(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Set the name_label field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name_label">New value to set</param>
        public static void set_name_label(Session session, string _pool, string _name_label)
        {
            session.proxy.pool_set_name_label(session.uuid, (_pool != null) ? _pool : "", (_name_label != null) ? _name_label : "").parse();
        }

        /// <summary>
        /// Set the name_description field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name_description">New value to set</param>
        public static void set_name_description(Session session, string _pool, string _name_description)
        {
            session.proxy.pool_set_name_description(session.uuid, (_pool != null) ? _pool : "", (_name_description != null) ? _name_description : "").parse();
        }

        /// <summary>
        /// Set the default_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_default_sr">New value to set</param>
        public static void set_default_SR(Session session, string _pool, string _default_sr)
        {
            session.proxy.pool_set_default_sr(session.uuid, (_pool != null) ? _pool : "", (_default_sr != null) ? _default_sr : "").parse();
        }

        /// <summary>
        /// Set the suspend_image_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_suspend_image_sr">New value to set</param>
        public static void set_suspend_image_SR(Session session, string _pool, string _suspend_image_sr)
        {
            session.proxy.pool_set_suspend_image_sr(session.uuid, (_pool != null) ? _pool : "", (_suspend_image_sr != null) ? _suspend_image_sr : "").parse();
        }

        /// <summary>
        /// Set the crash_dump_SR field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_crash_dump_sr">New value to set</param>
        public static void set_crash_dump_SR(Session session, string _pool, string _crash_dump_sr)
        {
            session.proxy.pool_set_crash_dump_sr(session.uuid, (_pool != null) ? _pool : "", (_crash_dump_sr != null) ? _crash_dump_sr : "").parse();
        }

        /// <summary>
        /// Set the other_config field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_other_config">New value to set</param>
        public static void set_other_config(Session session, string _pool, Dictionary<string, string> _other_config)
        {
            session.proxy.pool_set_other_config(session.uuid, (_pool != null) ? _pool : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the other_config field of the given pool.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_other_config(Session session, string _pool, string _key, string _value)
        {
            session.proxy.pool_add_to_other_config(session.uuid, (_pool != null) ? _pool : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the other_config field of the given pool.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_other_config(Session session, string _pool, string _key)
        {
            session.proxy.pool_remove_from_other_config(session.uuid, (_pool != null) ? _pool : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Set the ha_allow_overcommit field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_ha_allow_overcommit">New value to set</param>
        public static void set_ha_allow_overcommit(Session session, string _pool, bool _ha_allow_overcommit)
        {
            session.proxy.pool_set_ha_allow_overcommit(session.uuid, (_pool != null) ? _pool : "", _ha_allow_overcommit).parse();
        }

        /// <summary>
        /// Set the tags field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_tags">New value to set</param>
        public static void set_tags(Session session, string _pool, string[] _tags)
        {
            session.proxy.pool_set_tags(session.uuid, (_pool != null) ? _pool : "", _tags).parse();
        }

        /// <summary>
        /// Add the given value to the tags field of the given pool.  If the value is already in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">New value to add</param>
        public static void add_tags(Session session, string _pool, string _value)
        {
            session.proxy.pool_add_tags(session.uuid, (_pool != null) ? _pool : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given value from the tags field of the given pool.  If the value is not in that Set, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">Value to remove</param>
        public static void remove_tags(Session session, string _pool, string _value)
        {
            session.proxy.pool_remove_tags(session.uuid, (_pool != null) ? _pool : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Set the gui_config field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_gui_config">New value to set</param>
        public static void set_gui_config(Session session, string _pool, Dictionary<string, string> _gui_config)
        {
            session.proxy.pool_set_gui_config(session.uuid, (_pool != null) ? _pool : "", Maps.convert_to_proxy_string_string(_gui_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the gui_config field of the given pool.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_gui_config(Session session, string _pool, string _key, string _value)
        {
            session.proxy.pool_add_to_gui_config(session.uuid, (_pool != null) ? _pool : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the gui_config field of the given pool.  If the key is not in that Map, then do nothing.
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_gui_config(Session session, string _pool, string _key)
        {
            session.proxy.pool_remove_from_gui_config(session.uuid, (_pool != null) ? _pool : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Set the health_check_config field of the given pool.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_health_check_config">New value to set</param>
        public static void set_health_check_config(Session session, string _pool, Dictionary<string, string> _health_check_config)
        {
            session.proxy.pool_set_health_check_config(session.uuid, (_pool != null) ? _pool : "", Maps.convert_to_proxy_string_string(_health_check_config)).parse();
        }

        /// <summary>
        /// Add the given key-value pair to the health_check_config field of the given pool.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to add</param>
        /// <param name="_value">Value to add</param>
        public static void add_to_health_check_config(Session session, string _pool, string _key, string _value)
        {
            session.proxy.pool_add_to_health_check_config(session.uuid, (_pool != null) ? _pool : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Remove the given key and its corresponding value from the health_check_config field of the given pool.  If the key is not in that Map, then do nothing.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">Key to remove</param>
        public static void remove_from_health_check_config(Session session, string _pool, string _key)
        {
            session.proxy.pool_remove_from_health_check_config(session.uuid, (_pool != null) ? _pool : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Set the wlb_enabled field of the given pool.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_wlb_enabled">New value to set</param>
        public static void set_wlb_enabled(Session session, string _pool, bool _wlb_enabled)
        {
            session.proxy.pool_set_wlb_enabled(session.uuid, (_pool != null) ? _pool : "", _wlb_enabled).parse();
        }

        /// <summary>
        /// Set the wlb_verify_cert field of the given pool.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_wlb_verify_cert">New value to set</param>
        public static void set_wlb_verify_cert(Session session, string _pool, bool _wlb_verify_cert)
        {
            session.proxy.pool_set_wlb_verify_cert(session.uuid, (_pool != null) ? _pool : "", _wlb_verify_cert).parse();
        }

        /// <summary>
        /// Set the policy_no_vendor_device field of the given pool.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_policy_no_vendor_device">New value to set</param>
        public static void set_policy_no_vendor_device(Session session, string _pool, bool _policy_no_vendor_device)
        {
            session.proxy.pool_set_policy_no_vendor_device(session.uuid, (_pool != null) ? _pool : "", _policy_no_vendor_device).parse();
        }

        /// <summary>
        /// Set the live_patching_disabled field of the given pool.
        /// First published in .
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_live_patching_disabled">New value to set</param>
        public static void set_live_patching_disabled(Session session, string _pool, bool _live_patching_disabled)
        {
            session.proxy.pool_set_live_patching_disabled(session.uuid, (_pool != null) ? _pool : "", _live_patching_disabled).parse();
        }

        /// <summary>
        /// Instruct host to join a new pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_master_address">The hostname of the master of the pool to join</param>
        /// <param name="_master_username">The username of the master (for initial authentication)</param>
        /// <param name="_master_password">The password for the master (for initial authentication)</param>
        public static void join(Session session, string _master_address, string _master_username, string _master_password)
        {
            session.proxy.pool_join(session.uuid, (_master_address != null) ? _master_address : "", (_master_username != null) ? _master_username : "", (_master_password != null) ? _master_password : "").parse();
        }

        /// <summary>
        /// Instruct host to join a new pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_master_address">The hostname of the master of the pool to join</param>
        /// <param name="_master_username">The username of the master (for initial authentication)</param>
        /// <param name="_master_password">The password for the master (for initial authentication)</param>
        public static XenRef<Task> async_join(Session session, string _master_address, string _master_username, string _master_password)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_join(session.uuid, (_master_address != null) ? _master_address : "", (_master_username != null) ? _master_username : "", (_master_password != null) ? _master_password : "").parse());
        }

        /// <summary>
        /// Instruct host to join a new pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_master_address">The hostname of the master of the pool to join</param>
        /// <param name="_master_username">The username of the master (for initial authentication)</param>
        /// <param name="_master_password">The password for the master (for initial authentication)</param>
        public static void join_force(Session session, string _master_address, string _master_username, string _master_password)
        {
            session.proxy.pool_join_force(session.uuid, (_master_address != null) ? _master_address : "", (_master_username != null) ? _master_username : "", (_master_password != null) ? _master_password : "").parse();
        }

        /// <summary>
        /// Instruct host to join a new pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_master_address">The hostname of the master of the pool to join</param>
        /// <param name="_master_username">The username of the master (for initial authentication)</param>
        /// <param name="_master_password">The password for the master (for initial authentication)</param>
        public static XenRef<Task> async_join_force(Session session, string _master_address, string _master_username, string _master_password)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_join_force(session.uuid, (_master_address != null) ? _master_address : "", (_master_username != null) ? _master_username : "", (_master_password != null) ? _master_password : "").parse());
        }

        /// <summary>
        /// Instruct a pool master to eject a host from the pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host to eject</param>
        public static void eject(Session session, string _host)
        {
            session.proxy.pool_eject(session.uuid, (_host != null) ? _host : "").parse();
        }

        /// <summary>
        /// Instruct a pool master to eject a host from the pool
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host to eject</param>
        public static XenRef<Task> async_eject(Session session, string _host)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_eject(session.uuid, (_host != null) ? _host : "").parse());
        }

        /// <summary>
        /// Instruct host that's currently a slave to transition to being master
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static void emergency_transition_to_master(Session session)
        {
            session.proxy.pool_emergency_transition_to_master(session.uuid).parse();
        }

        /// <summary>
        /// Instruct a slave already in a pool that the master has changed
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_master_address">The hostname of the master</param>
        public static void emergency_reset_master(Session session, string _master_address)
        {
            session.proxy.pool_emergency_reset_master(session.uuid, (_master_address != null) ? _master_address : "").parse();
        }

        /// <summary>
        /// Instruct a pool master, M, to try and contact its slaves and, if slaves are in emergency mode, reset their master address to M.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Host>> recover_slaves(Session session)
        {
            return XenRef<Host>.Create(session.proxy.pool_recover_slaves(session.uuid).parse());
        }

        /// <summary>
        /// Instruct a pool master, M, to try and contact its slaves and, if slaves are in emergency mode, reset their master address to M.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static XenRef<Task> async_recover_slaves(Session session)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_recover_slaves(session.uuid).parse());
        }

        /// <summary>
        /// Create PIFs, mapping a network to the same physical interface/VLAN on each host. This call is deprecated: use Pool.create_VLAN_from_PIF instead.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device">physical interface on which to create the VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        public static List<XenRef<PIF>> create_VLAN(Session session, string _device, string _network, long _vlan)
        {
            return XenRef<PIF>.Create(session.proxy.pool_create_vlan(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", _vlan.ToString()).parse());
        }

        /// <summary>
        /// Create PIFs, mapping a network to the same physical interface/VLAN on each host. This call is deprecated: use Pool.create_VLAN_from_PIF instead.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_device">physical interface on which to create the VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        public static XenRef<Task> async_create_VLAN(Session session, string _device, string _network, long _vlan)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_create_vlan(session.uuid, (_device != null) ? _device : "", (_network != null) ? _network : "", _vlan.ToString()).parse());
        }

        /// <summary>
        /// Create a pool-wide VLAN by taking the PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">physical interface on any particular host, that identifies the PIF on which to create the (pool-wide) VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        public static List<XenRef<PIF>> create_VLAN_from_PIF(Session session, string _pif, string _network, long _vlan)
        {
            return XenRef<PIF>.Create(session.proxy.pool_create_vlan_from_pif(session.uuid, (_pif != null) ? _pif : "", (_network != null) ? _network : "", _vlan.ToString()).parse());
        }

        /// <summary>
        /// Create a pool-wide VLAN by taking the PIF.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pif">physical interface on any particular host, that identifies the PIF on which to create the (pool-wide) VLAN interface</param>
        /// <param name="_network">network to which this interface should be connected</param>
        /// <param name="_vlan">VLAN tag for the new interface</param>
        public static XenRef<Task> async_create_VLAN_from_PIF(Session session, string _pif, string _network, long _vlan)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_create_vlan_from_pif(session.uuid, (_pif != null) ? _pif : "", (_network != null) ? _network : "", _vlan.ToString()).parse());
        }

        /// <summary>
        /// Turn on High Availability mode
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_heartbeat_srs">Set of SRs to use for storage heartbeating</param>
        /// <param name="_configuration">Detailed HA configuration to apply</param>
        public static void enable_ha(Session session, List<XenRef<SR>> _heartbeat_srs, Dictionary<string, string> _configuration)
        {
            session.proxy.pool_enable_ha(session.uuid, (_heartbeat_srs != null) ? Helper.RefListToStringArray(_heartbeat_srs) : new string[] {}, Maps.convert_to_proxy_string_string(_configuration)).parse();
        }

        /// <summary>
        /// Turn on High Availability mode
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_heartbeat_srs">Set of SRs to use for storage heartbeating</param>
        /// <param name="_configuration">Detailed HA configuration to apply</param>
        public static XenRef<Task> async_enable_ha(Session session, List<XenRef<SR>> _heartbeat_srs, Dictionary<string, string> _configuration)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_enable_ha(session.uuid, (_heartbeat_srs != null) ? Helper.RefListToStringArray(_heartbeat_srs) : new string[] {}, Maps.convert_to_proxy_string_string(_configuration)).parse());
        }

        /// <summary>
        /// Turn off High Availability mode
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        public static void disable_ha(Session session)
        {
            session.proxy.pool_disable_ha(session.uuid).parse();
        }

        /// <summary>
        /// Turn off High Availability mode
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        public static XenRef<Task> async_disable_ha(Session session)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_disable_ha(session.uuid).parse());
        }

        /// <summary>
        /// Forcibly synchronise the database now
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static void sync_database(Session session)
        {
            session.proxy.pool_sync_database(session.uuid).parse();
        }

        /// <summary>
        /// Forcibly synchronise the database now
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static XenRef<Task> async_sync_database(Session session)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_sync_database(session.uuid).parse());
        }

        /// <summary>
        /// Perform an orderly handover of the role of master to the referenced host.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host who should become the new master</param>
        public static void designate_new_master(Session session, string _host)
        {
            session.proxy.pool_designate_new_master(session.uuid, (_host != null) ? _host : "").parse();
        }

        /// <summary>
        /// Perform an orderly handover of the role of master to the referenced host.
        /// First published in XenServer 4.1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host">The host who should become the new master</param>
        public static XenRef<Task> async_designate_new_master(Session session, string _host)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_designate_new_master(session.uuid, (_host != null) ? _host : "").parse());
        }

        /// <summary>
        /// When this call returns the VM restart logic will not run for the requested number of seconds. If the argument is zero then the restart thread is immediately unblocked
        /// First published in XenServer 5.0 Update 1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_seconds">The number of seconds to block the restart thread for</param>
        public static void ha_prevent_restarts_for(Session session, long _seconds)
        {
            session.proxy.pool_ha_prevent_restarts_for(session.uuid, _seconds.ToString()).parse();
        }

        /// <summary>
        /// Returns true if a VM failover plan exists for up to 'n' host failures
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_n">The number of host failures to plan for</param>
        public static bool ha_failover_plan_exists(Session session, long _n)
        {
            return (bool)session.proxy.pool_ha_failover_plan_exists(session.uuid, _n.ToString()).parse();
        }

        /// <summary>
        /// Returns the maximum number of host failures we could tolerate before we would be unable to restart configured VMs
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static long ha_compute_max_host_failures_to_tolerate(Session session)
        {
            return long.Parse((string)session.proxy.pool_ha_compute_max_host_failures_to_tolerate(session.uuid).parse());
        }

        /// <summary>
        /// Returns the maximum number of host failures we could tolerate before we would be unable to restart the provided VMs
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_configuration">Map of protected VM reference to restart priority</param>
        public static long ha_compute_hypothetical_max_host_failures_to_tolerate(Session session, Dictionary<XenRef<VM>, string> _configuration)
        {
            return long.Parse((string)session.proxy.pool_ha_compute_hypothetical_max_host_failures_to_tolerate(session.uuid, Maps.convert_to_proxy_XenRefVM_string(_configuration)).parse());
        }

        /// <summary>
        /// Return a VM failover plan assuming a given subset of hosts fail
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_failed_hosts">The set of hosts to assume have failed</param>
        /// <param name="_failed_vms">The set of VMs to restart</param>
        public static Dictionary<XenRef<VM>, Dictionary<string, string>> ha_compute_vm_failover_plan(Session session, List<XenRef<Host>> _failed_hosts, List<XenRef<VM>> _failed_vms)
        {
            return Maps.convert_from_proxy_XenRefVM_Dictionary_string_string(session.proxy.pool_ha_compute_vm_failover_plan(session.uuid, (_failed_hosts != null) ? Helper.RefListToStringArray(_failed_hosts) : new string[] {}, (_failed_vms != null) ? Helper.RefListToStringArray(_failed_vms) : new string[] {}).parse());
        }

        /// <summary>
        /// Set the maximum number of host failures to consider in the HA VM restart planner
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">New number of host failures to consider</param>
        public static void set_ha_host_failures_to_tolerate(Session session, string _pool, long _value)
        {
            session.proxy.pool_set_ha_host_failures_to_tolerate(session.uuid, (_pool != null) ? _pool : "", _value.ToString()).parse();
        }

        /// <summary>
        /// Set the maximum number of host failures to consider in the HA VM restart planner
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_value">New number of host failures to consider</param>
        public static XenRef<Task> async_set_ha_host_failures_to_tolerate(Session session, string _pool, long _value)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_set_ha_host_failures_to_tolerate(session.uuid, (_pool != null) ? _pool : "", _value.ToString()).parse());
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        public static XenRef<Blob> create_new_blob(Session session, string _pool, string _name, string _mime_type)
        {
            return XenRef<Blob>.Create(session.proxy.pool_create_new_blob(session.uuid, (_pool != null) ? _pool : "", (_name != null) ? _name : "", (_mime_type != null) ? _mime_type : "").parse());
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        public static XenRef<Task> async_create_new_blob(Session session, string _pool, string _name, string _mime_type)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_create_new_blob(session.uuid, (_pool != null) ? _pool : "", (_name != null) ? _name : "", (_mime_type != null) ? _mime_type : "").parse());
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <param name="_public">True if the blob should be publicly available First published in XenServer 6.1.</param>
        public static XenRef<Blob> create_new_blob(Session session, string _pool, string _name, string _mime_type, bool _public)
        {
            return XenRef<Blob>.Create(session.proxy.pool_create_new_blob(session.uuid, (_pool != null) ? _pool : "", (_name != null) ? _name : "", (_mime_type != null) ? _mime_type : "", _public).parse());
        }

        /// <summary>
        /// Create a placeholder for a named binary blob of data that is associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name associated with the blob</param>
        /// <param name="_mime_type">The mime type for the data. Empty string translates to application/octet-stream</param>
        /// <param name="_public">True if the blob should be publicly available First published in XenServer 6.1.</param>
        public static XenRef<Task> async_create_new_blob(Session session, string _pool, string _name, string _mime_type, bool _public)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_create_new_blob(session.uuid, (_pool != null) ? _pool : "", (_name != null) ? _name : "", (_mime_type != null) ? _mime_type : "", _public).parse());
        }

        /// <summary>
        /// This call enables external authentication on all the hosts of the pool
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_config">A list of key-values containing the configuration data</param>
        /// <param name="_service_name">The name of the service</param>
        /// <param name="_auth_type">The type of authentication (e.g. AD for Active Directory)</param>
        public static void enable_external_auth(Session session, string _pool, Dictionary<string, string> _config, string _service_name, string _auth_type)
        {
            session.proxy.pool_enable_external_auth(session.uuid, (_pool != null) ? _pool : "", Maps.convert_to_proxy_string_string(_config), (_service_name != null) ? _service_name : "", (_auth_type != null) ? _auth_type : "").parse();
        }

        /// <summary>
        /// This call disables external authentication on all the hosts of the pool
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_config">Optional parameters as a list of key-values containing the configuration data</param>
        public static void disable_external_auth(Session session, string _pool, Dictionary<string, string> _config)
        {
            session.proxy.pool_disable_external_auth(session.uuid, (_pool != null) ? _pool : "", Maps.convert_to_proxy_string_string(_config)).parse();
        }

        /// <summary>
        /// This call asynchronously detects if the external authentication configuration in any slave is different from that in the master and raises appropriate alerts
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static void detect_nonhomogeneous_external_auth(Session session, string _pool)
        {
            session.proxy.pool_detect_nonhomogeneous_external_auth(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Initializes workload balancing monitoring on this pool with the specified wlb server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_wlb_url">The ip address and port to use when accessing the wlb server</param>
        /// <param name="_wlb_username">The username used to authenticate with the wlb server</param>
        /// <param name="_wlb_password">The password used to authenticate with the wlb server</param>
        /// <param name="_xenserver_username">The username used by the wlb server to authenticate with the xenserver</param>
        /// <param name="_xenserver_password">The password used by the wlb server to authenticate with the xenserver</param>
        public static void initialize_wlb(Session session, string _wlb_url, string _wlb_username, string _wlb_password, string _xenserver_username, string _xenserver_password)
        {
            session.proxy.pool_initialize_wlb(session.uuid, (_wlb_url != null) ? _wlb_url : "", (_wlb_username != null) ? _wlb_username : "", (_wlb_password != null) ? _wlb_password : "", (_xenserver_username != null) ? _xenserver_username : "", (_xenserver_password != null) ? _xenserver_password : "").parse();
        }

        /// <summary>
        /// Initializes workload balancing monitoring on this pool with the specified wlb server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_wlb_url">The ip address and port to use when accessing the wlb server</param>
        /// <param name="_wlb_username">The username used to authenticate with the wlb server</param>
        /// <param name="_wlb_password">The password used to authenticate with the wlb server</param>
        /// <param name="_xenserver_username">The username used by the wlb server to authenticate with the xenserver</param>
        /// <param name="_xenserver_password">The password used by the wlb server to authenticate with the xenserver</param>
        public static XenRef<Task> async_initialize_wlb(Session session, string _wlb_url, string _wlb_username, string _wlb_password, string _xenserver_username, string _xenserver_password)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_initialize_wlb(session.uuid, (_wlb_url != null) ? _wlb_url : "", (_wlb_username != null) ? _wlb_username : "", (_wlb_password != null) ? _wlb_password : "", (_xenserver_username != null) ? _xenserver_username : "", (_xenserver_password != null) ? _xenserver_password : "").parse());
        }

        /// <summary>
        /// Permanently deconfigures workload balancing monitoring on this pool
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static void deconfigure_wlb(Session session)
        {
            session.proxy.pool_deconfigure_wlb(session.uuid).parse();
        }

        /// <summary>
        /// Permanently deconfigures workload balancing monitoring on this pool
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static XenRef<Task> async_deconfigure_wlb(Session session)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_deconfigure_wlb(session.uuid).parse());
        }

        /// <summary>
        /// Sets the pool optimization criteria for the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_config">The configuration to use in optimizing this pool</param>
        public static void send_wlb_configuration(Session session, Dictionary<string, string> _config)
        {
            session.proxy.pool_send_wlb_configuration(session.uuid, Maps.convert_to_proxy_string_string(_config)).parse();
        }

        /// <summary>
        /// Sets the pool optimization criteria for the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_config">The configuration to use in optimizing this pool</param>
        public static XenRef<Task> async_send_wlb_configuration(Session session, Dictionary<string, string> _config)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_send_wlb_configuration(session.uuid, Maps.convert_to_proxy_string_string(_config)).parse());
        }

        /// <summary>
        /// Retrieves the pool optimization criteria from the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<string, string> retrieve_wlb_configuration(Session session)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pool_retrieve_wlb_configuration(session.uuid).parse());
        }

        /// <summary>
        /// Retrieves the pool optimization criteria from the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static XenRef<Task> async_retrieve_wlb_configuration(Session session)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_retrieve_wlb_configuration(session.uuid).parse());
        }

        /// <summary>
        /// Retrieves vm migrate recommendations for the pool from the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<VM>, string[]> retrieve_wlb_recommendations(Session session)
        {
            return Maps.convert_from_proxy_XenRefVM_string_array(session.proxy.pool_retrieve_wlb_recommendations(session.uuid).parse());
        }

        /// <summary>
        /// Retrieves vm migrate recommendations for the pool from the workload balancing server
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static XenRef<Task> async_retrieve_wlb_recommendations(Session session)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_retrieve_wlb_recommendations(session.uuid).parse());
        }

        /// <summary>
        /// Send the given body to the given host and port, using HTTPS, and print the response.  This is used for debugging the SSL layer.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host"></param>
        /// <param name="_port"></param>
        /// <param name="_body"></param>
        public static string send_test_post(Session session, string _host, long _port, string _body)
        {
            return (string)session.proxy.pool_send_test_post(session.uuid, (_host != null) ? _host : "", _port.ToString(), (_body != null) ? _body : "").parse();
        }

        /// <summary>
        /// Send the given body to the given host and port, using HTTPS, and print the response.  This is used for debugging the SSL layer.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_host"></param>
        /// <param name="_port"></param>
        /// <param name="_body"></param>
        public static XenRef<Task> async_send_test_post(Session session, string _host, long _port, string _body)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_send_test_post(session.uuid, (_host != null) ? _host : "", _port.ToString(), (_body != null) ? _body : "").parse());
        }

        /// <summary>
        /// Install an SSL certificate pool-wide.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">A name to give the certificate</param>
        /// <param name="_cert">The certificate</param>
        public static void certificate_install(Session session, string _name, string _cert)
        {
            session.proxy.pool_certificate_install(session.uuid, (_name != null) ? _name : "", (_cert != null) ? _cert : "").parse();
        }

        /// <summary>
        /// Install an SSL certificate pool-wide.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">A name to give the certificate</param>
        /// <param name="_cert">The certificate</param>
        public static XenRef<Task> async_certificate_install(Session session, string _name, string _cert)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_certificate_install(session.uuid, (_name != null) ? _name : "", (_cert != null) ? _cert : "").parse());
        }

        /// <summary>
        /// Remove an SSL certificate.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The certificate name</param>
        public static void certificate_uninstall(Session session, string _name)
        {
            session.proxy.pool_certificate_uninstall(session.uuid, (_name != null) ? _name : "").parse();
        }

        /// <summary>
        /// Remove an SSL certificate.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The certificate name</param>
        public static XenRef<Task> async_certificate_uninstall(Session session, string _name)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_certificate_uninstall(session.uuid, (_name != null) ? _name : "").parse());
        }

        /// <summary>
        /// List all installed SSL certificates.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static string[] certificate_list(Session session)
        {
            return (string [])session.proxy.pool_certificate_list(session.uuid).parse();
        }

        /// <summary>
        /// List all installed SSL certificates.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static XenRef<Task> async_certificate_list(Session session)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_certificate_list(session.uuid).parse());
        }

        /// <summary>
        /// Install an SSL certificate revocation list, pool-wide.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">A name to give the CRL</param>
        /// <param name="_cert">The CRL</param>
        public static void crl_install(Session session, string _name, string _cert)
        {
            session.proxy.pool_crl_install(session.uuid, (_name != null) ? _name : "", (_cert != null) ? _cert : "").parse();
        }

        /// <summary>
        /// Install an SSL certificate revocation list, pool-wide.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">A name to give the CRL</param>
        /// <param name="_cert">The CRL</param>
        public static XenRef<Task> async_crl_install(Session session, string _name, string _cert)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_crl_install(session.uuid, (_name != null) ? _name : "", (_cert != null) ? _cert : "").parse());
        }

        /// <summary>
        /// Remove an SSL certificate revocation list.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The CRL name</param>
        public static void crl_uninstall(Session session, string _name)
        {
            session.proxy.pool_crl_uninstall(session.uuid, (_name != null) ? _name : "").parse();
        }

        /// <summary>
        /// Remove an SSL certificate revocation list.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_name">The CRL name</param>
        public static XenRef<Task> async_crl_uninstall(Session session, string _name)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_crl_uninstall(session.uuid, (_name != null) ? _name : "").parse());
        }

        /// <summary>
        /// List all installed SSL certificate revocation lists.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static string[] crl_list(Session session)
        {
            return (string [])session.proxy.pool_crl_list(session.uuid).parse();
        }

        /// <summary>
        /// List all installed SSL certificate revocation lists.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static XenRef<Task> async_crl_list(Session session)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_crl_list(session.uuid).parse());
        }

        /// <summary>
        /// Sync SSL certificates from master to slaves.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static void certificate_sync(Session session)
        {
            session.proxy.pool_certificate_sync(session.uuid).parse();
        }

        /// <summary>
        /// Sync SSL certificates from master to slaves.
        /// First published in XenServer 5.5.
        /// </summary>
        /// <param name="session">The session</param>
        public static XenRef<Task> async_certificate_sync(Session session)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_certificate_sync(session.uuid).parse());
        }

        /// <summary>
        /// Enable the redo log on the given SR and start using it, unless HA is enabled.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sr">SR to hold the redo log.</param>
        public static void enable_redo_log(Session session, string _sr)
        {
            session.proxy.pool_enable_redo_log(session.uuid, (_sr != null) ? _sr : "").parse();
        }

        /// <summary>
        /// Enable the redo log on the given SR and start using it, unless HA is enabled.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_sr">SR to hold the redo log.</param>
        public static XenRef<Task> async_enable_redo_log(Session session, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_enable_redo_log(session.uuid, (_sr != null) ? _sr : "").parse());
        }

        /// <summary>
        /// Disable the redo log if in use, unless HA is enabled.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        public static void disable_redo_log(Session session)
        {
            session.proxy.pool_disable_redo_log(session.uuid).parse();
        }

        /// <summary>
        /// Disable the redo log if in use, unless HA is enabled.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        public static XenRef<Task> async_disable_redo_log(Session session)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_disable_redo_log(session.uuid).parse());
        }

        /// <summary>
        /// Set the IP address of the vswitch controller.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_address">IP address of the vswitch controller.</param>
        public static void set_vswitch_controller(Session session, string _address)
        {
            session.proxy.pool_set_vswitch_controller(session.uuid, (_address != null) ? _address : "").parse();
        }

        /// <summary>
        /// Set the IP address of the vswitch controller.
        /// First published in XenServer 5.6.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_address">IP address of the vswitch controller.</param>
        public static XenRef<Task> async_set_vswitch_controller(Session session, string _address)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_set_vswitch_controller(session.uuid, (_address != null) ? _address : "").parse());
        }

        /// <summary>
        /// This call tests if a location is valid
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_config">Location config settings to test</param>
        public static string test_archive_target(Session session, string _pool, Dictionary<string, string> _config)
        {
            return (string)session.proxy.pool_test_archive_target(session.uuid, (_pool != null) ? _pool : "", Maps.convert_to_proxy_string_string(_config)).parse();
        }

        /// <summary>
        /// This call attempts to enable pool-wide local storage caching
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static void enable_local_storage_caching(Session session, string _pool)
        {
            session.proxy.pool_enable_local_storage_caching(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// This call attempts to enable pool-wide local storage caching
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static XenRef<Task> async_enable_local_storage_caching(Session session, string _pool)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_enable_local_storage_caching(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// This call disables pool-wide local storage caching
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static void disable_local_storage_caching(Session session, string _pool)
        {
            session.proxy.pool_disable_local_storage_caching(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// This call disables pool-wide local storage caching
        /// First published in XenServer 5.6 FP1.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static XenRef<Task> async_disable_local_storage_caching(Session session, string _pool)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_disable_local_storage_caching(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// This call returns the license state for the pool
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static Dictionary<string, string> get_license_state(Session session, string _pool)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.pool_get_license_state(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// This call returns the license state for the pool
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static XenRef<Task> async_get_license_state(Session session, string _pool)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_get_license_state(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Apply an edition to all hosts in the pool
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_edition">The requested edition</param>
        public static void apply_edition(Session session, string _pool, string _edition)
        {
            session.proxy.pool_apply_edition(session.uuid, (_pool != null) ? _pool : "", (_edition != null) ? _edition : "").parse();
        }

        /// <summary>
        /// Apply an edition to all hosts in the pool
        /// First published in XenServer 6.2.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_edition">The requested edition</param>
        public static XenRef<Task> async_apply_edition(Session session, string _pool, string _edition)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_apply_edition(session.uuid, (_pool != null) ? _pool : "", (_edition != null) ? _edition : "").parse());
        }

        /// <summary>
        /// Sets ssl_legacy true on each host, pool-master last. See Host.ssl_legacy and Host.set_ssl_legacy.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static void enable_ssl_legacy(Session session, string _pool)
        {
            session.proxy.pool_enable_ssl_legacy(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Sets ssl_legacy true on each host, pool-master last. See Host.ssl_legacy and Host.set_ssl_legacy.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static XenRef<Task> async_enable_ssl_legacy(Session session, string _pool)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_enable_ssl_legacy(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Sets ssl_legacy true on each host, pool-master last. See Host.ssl_legacy and Host.set_ssl_legacy.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static void disable_ssl_legacy(Session session, string _pool)
        {
            session.proxy.pool_disable_ssl_legacy(session.uuid, (_pool != null) ? _pool : "").parse();
        }

        /// <summary>
        /// Sets ssl_legacy true on each host, pool-master last. See Host.ssl_legacy and Host.set_ssl_legacy.
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        public static XenRef<Task> async_disable_ssl_legacy(Session session, string _pool)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_disable_ssl_legacy(session.uuid, (_pool != null) ? _pool : "").parse());
        }

        /// <summary>
        /// Return true if the extension is available on the pool
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name of the API call</param>
        public static bool has_extension(Session session, string _pool, string _name)
        {
            return (bool)session.proxy.pool_has_extension(session.uuid, (_pool != null) ? _pool : "", (_name != null) ? _name : "").parse();
        }

        /// <summary>
        /// Return true if the extension is available on the pool
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_name">The name of the API call</param>
        public static XenRef<Task> async_has_extension(Session session, string _pool, string _name)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_has_extension(session.uuid, (_pool != null) ? _pool : "", (_name != null) ? _name : "").parse());
        }

        /// <summary>
        /// Add a key-value pair to the pool-wide guest agent configuration
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">The key to add</param>
        /// <param name="_value">The value to add</param>
        public static void add_to_guest_agent_config(Session session, string _pool, string _key, string _value)
        {
            session.proxy.pool_add_to_guest_agent_config(session.uuid, (_pool != null) ? _pool : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        /// <summary>
        /// Add a key-value pair to the pool-wide guest agent configuration
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">The key to add</param>
        /// <param name="_value">The value to add</param>
        public static XenRef<Task> async_add_to_guest_agent_config(Session session, string _pool, string _key, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_add_to_guest_agent_config(session.uuid, (_pool != null) ? _pool : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse());
        }

        /// <summary>
        /// Remove a key-value pair from the pool-wide guest agent configuration
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">The key to remove</param>
        public static void remove_from_guest_agent_config(Session session, string _pool, string _key)
        {
            session.proxy.pool_remove_from_guest_agent_config(session.uuid, (_pool != null) ? _pool : "", (_key != null) ? _key : "").parse();
        }

        /// <summary>
        /// Remove a key-value pair from the pool-wide guest agent configuration
        /// First published in XenServer Dundee.
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="_pool">The opaque_ref of the given pool</param>
        /// <param name="_key">The key to remove</param>
        public static XenRef<Task> async_remove_from_guest_agent_config(Session session, string _pool, string _key)
        {
            return XenRef<Task>.Create(session.proxy.async_pool_remove_from_guest_agent_config(session.uuid, (_pool != null) ? _pool : "", (_key != null) ? _key : "").parse());
        }

        /// <summary>
        /// Return a list of all the pools known to the system.
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static List<XenRef<Pool>> get_all(Session session)
        {
            return XenRef<Pool>.Create(session.proxy.pool_get_all(session.uuid).parse());
        }

        /// <summary>
        /// Get all the pool Records at once, in a single XML RPC call
        /// First published in XenServer 4.0.
        /// </summary>
        /// <param name="session">The session</param>
        public static Dictionary<XenRef<Pool>, Pool> get_all_records(Session session)
        {
            return XenRef<Pool>.Create<Proxy_Pool>(session.proxy.pool_get_all_records(session.uuid).parse());
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
        /// Short name
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
        /// Description
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
        /// The host that is pool master
        /// </summary>
        public virtual XenRef<Host> master
        {
            get { return _master; }
            set
            {
                if (!Helper.AreEqual(value, _master))
                {
                    _master = value;
                    Changed = true;
                    NotifyPropertyChanged("master");
                }
            }
        }
        private XenRef<Host> _master;

        /// <summary>
        /// Default SR for VDIs
        /// </summary>
        public virtual XenRef<SR> default_SR
        {
            get { return _default_SR; }
            set
            {
                if (!Helper.AreEqual(value, _default_SR))
                {
                    _default_SR = value;
                    Changed = true;
                    NotifyPropertyChanged("default_SR");
                }
            }
        }
        private XenRef<SR> _default_SR;

        /// <summary>
        /// The SR in which VDIs for suspend images are created
        /// </summary>
        public virtual XenRef<SR> suspend_image_SR
        {
            get { return _suspend_image_SR; }
            set
            {
                if (!Helper.AreEqual(value, _suspend_image_SR))
                {
                    _suspend_image_SR = value;
                    Changed = true;
                    NotifyPropertyChanged("suspend_image_SR");
                }
            }
        }
        private XenRef<SR> _suspend_image_SR;

        /// <summary>
        /// The SR in which VDIs for crash dumps are created
        /// </summary>
        public virtual XenRef<SR> crash_dump_SR
        {
            get { return _crash_dump_SR; }
            set
            {
                if (!Helper.AreEqual(value, _crash_dump_SR))
                {
                    _crash_dump_SR = value;
                    Changed = true;
                    NotifyPropertyChanged("crash_dump_SR");
                }
            }
        }
        private XenRef<SR> _crash_dump_SR;

        /// <summary>
        /// additional configuration
        /// </summary>
        public virtual Dictionary<string, string> other_config
        {
            get { return _other_config; }
            set
            {
                if (!Helper.AreEqual(value, _other_config))
                {
                    _other_config = value;
                    Changed = true;
                    NotifyPropertyChanged("other_config");
                }
            }
        }
        private Dictionary<string, string> _other_config;

        /// <summary>
        /// true if HA is enabled on the pool, false otherwise
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool ha_enabled
        {
            get { return _ha_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _ha_enabled))
                {
                    _ha_enabled = value;
                    Changed = true;
                    NotifyPropertyChanged("ha_enabled");
                }
            }
        }
        private bool _ha_enabled;

        /// <summary>
        /// The current HA configuration
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual Dictionary<string, string> ha_configuration
        {
            get { return _ha_configuration; }
            set
            {
                if (!Helper.AreEqual(value, _ha_configuration))
                {
                    _ha_configuration = value;
                    Changed = true;
                    NotifyPropertyChanged("ha_configuration");
                }
            }
        }
        private Dictionary<string, string> _ha_configuration;

        /// <summary>
        /// HA statefile VDIs in use
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual string[] ha_statefiles
        {
            get { return _ha_statefiles; }
            set
            {
                if (!Helper.AreEqual(value, _ha_statefiles))
                {
                    _ha_statefiles = value;
                    Changed = true;
                    NotifyPropertyChanged("ha_statefiles");
                }
            }
        }
        private string[] _ha_statefiles;

        /// <summary>
        /// Number of host failures to tolerate before the Pool is declared to be overcommitted
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual long ha_host_failures_to_tolerate
        {
            get { return _ha_host_failures_to_tolerate; }
            set
            {
                if (!Helper.AreEqual(value, _ha_host_failures_to_tolerate))
                {
                    _ha_host_failures_to_tolerate = value;
                    Changed = true;
                    NotifyPropertyChanged("ha_host_failures_to_tolerate");
                }
            }
        }
        private long _ha_host_failures_to_tolerate;

        /// <summary>
        /// Number of future host failures we have managed to find a plan for. Once this reaches zero any future host failures will cause the failure of protected VMs.
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual long ha_plan_exists_for
        {
            get { return _ha_plan_exists_for; }
            set
            {
                if (!Helper.AreEqual(value, _ha_plan_exists_for))
                {
                    _ha_plan_exists_for = value;
                    Changed = true;
                    NotifyPropertyChanged("ha_plan_exists_for");
                }
            }
        }
        private long _ha_plan_exists_for;

        /// <summary>
        /// If set to false then operations which would cause the Pool to become overcommitted will be blocked.
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool ha_allow_overcommit
        {
            get { return _ha_allow_overcommit; }
            set
            {
                if (!Helper.AreEqual(value, _ha_allow_overcommit))
                {
                    _ha_allow_overcommit = value;
                    Changed = true;
                    NotifyPropertyChanged("ha_allow_overcommit");
                }
            }
        }
        private bool _ha_allow_overcommit;

        /// <summary>
        /// True if the Pool is considered to be overcommitted i.e. if there exist insufficient physical resources to tolerate the configured number of host failures
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual bool ha_overcommitted
        {
            get { return _ha_overcommitted; }
            set
            {
                if (!Helper.AreEqual(value, _ha_overcommitted))
                {
                    _ha_overcommitted = value;
                    Changed = true;
                    NotifyPropertyChanged("ha_overcommitted");
                }
            }
        }
        private bool _ha_overcommitted;

        /// <summary>
        /// Binary blobs associated with this pool
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual Dictionary<string, XenRef<Blob>> blobs
        {
            get { return _blobs; }
            set
            {
                if (!Helper.AreEqual(value, _blobs))
                {
                    _blobs = value;
                    Changed = true;
                    NotifyPropertyChanged("blobs");
                }
            }
        }
        private Dictionary<string, XenRef<Blob>> _blobs;

        /// <summary>
        /// user-specified tags for categorization purposes
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual string[] tags
        {
            get { return _tags; }
            set
            {
                if (!Helper.AreEqual(value, _tags))
                {
                    _tags = value;
                    Changed = true;
                    NotifyPropertyChanged("tags");
                }
            }
        }
        private string[] _tags;

        /// <summary>
        /// gui-specific configuration for pool
        /// First published in XenServer 5.0.
        /// </summary>
        public virtual Dictionary<string, string> gui_config
        {
            get { return _gui_config; }
            set
            {
                if (!Helper.AreEqual(value, _gui_config))
                {
                    _gui_config = value;
                    Changed = true;
                    NotifyPropertyChanged("gui_config");
                }
            }
        }
        private Dictionary<string, string> _gui_config;

        /// <summary>
        /// Configuration for the automatic health check feature
        /// First published in XenServer Dundee.
        /// </summary>
        public virtual Dictionary<string, string> health_check_config
        {
            get { return _health_check_config; }
            set
            {
                if (!Helper.AreEqual(value, _health_check_config))
                {
                    _health_check_config = value;
                    Changed = true;
                    NotifyPropertyChanged("health_check_config");
                }
            }
        }
        private Dictionary<string, string> _health_check_config;

        /// <summary>
        /// Url for the configured workload balancing host
        /// First published in XenServer 5.5.
        /// </summary>
        public virtual string wlb_url
        {
            get { return _wlb_url; }
            set
            {
                if (!Helper.AreEqual(value, _wlb_url))
                {
                    _wlb_url = value;
                    Changed = true;
                    NotifyPropertyChanged("wlb_url");
                }
            }
        }
        private string _wlb_url;

        /// <summary>
        /// Username for accessing the workload balancing host
        /// First published in XenServer 5.5.
        /// </summary>
        public virtual string wlb_username
        {
            get { return _wlb_username; }
            set
            {
                if (!Helper.AreEqual(value, _wlb_username))
                {
                    _wlb_username = value;
                    Changed = true;
                    NotifyPropertyChanged("wlb_username");
                }
            }
        }
        private string _wlb_username;

        /// <summary>
        /// true if workload balancing is enabled on the pool, false otherwise
        /// First published in XenServer 5.5.
        /// </summary>
        public virtual bool wlb_enabled
        {
            get { return _wlb_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _wlb_enabled))
                {
                    _wlb_enabled = value;
                    Changed = true;
                    NotifyPropertyChanged("wlb_enabled");
                }
            }
        }
        private bool _wlb_enabled;

        /// <summary>
        /// true if communication with the WLB server should enforce SSL certificate verification.
        /// First published in XenServer 5.5.
        /// </summary>
        public virtual bool wlb_verify_cert
        {
            get { return _wlb_verify_cert; }
            set
            {
                if (!Helper.AreEqual(value, _wlb_verify_cert))
                {
                    _wlb_verify_cert = value;
                    Changed = true;
                    NotifyPropertyChanged("wlb_verify_cert");
                }
            }
        }
        private bool _wlb_verify_cert;

        /// <summary>
        /// true a redo-log is to be used other than when HA is enabled, false otherwise
        /// First published in XenServer 5.6.
        /// </summary>
        public virtual bool redo_log_enabled
        {
            get { return _redo_log_enabled; }
            set
            {
                if (!Helper.AreEqual(value, _redo_log_enabled))
                {
                    _redo_log_enabled = value;
                    Changed = true;
                    NotifyPropertyChanged("redo_log_enabled");
                }
            }
        }
        private bool _redo_log_enabled;

        /// <summary>
        /// indicates the VDI to use for the redo-log other than when HA is enabled
        /// First published in XenServer 5.6.
        /// </summary>
        public virtual XenRef<VDI> redo_log_vdi
        {
            get { return _redo_log_vdi; }
            set
            {
                if (!Helper.AreEqual(value, _redo_log_vdi))
                {
                    _redo_log_vdi = value;
                    Changed = true;
                    NotifyPropertyChanged("redo_log_vdi");
                }
            }
        }
        private XenRef<VDI> _redo_log_vdi;

        /// <summary>
        /// address of the vswitch controller
        /// First published in XenServer 5.6.
        /// </summary>
        public virtual string vswitch_controller
        {
            get { return _vswitch_controller; }
            set
            {
                if (!Helper.AreEqual(value, _vswitch_controller))
                {
                    _vswitch_controller = value;
                    Changed = true;
                    NotifyPropertyChanged("vswitch_controller");
                }
            }
        }
        private string _vswitch_controller;

        /// <summary>
        /// Pool-wide restrictions currently in effect
        /// First published in XenServer 5.6.
        /// </summary>
        public virtual Dictionary<string, string> restrictions
        {
            get { return _restrictions; }
            set
            {
                if (!Helper.AreEqual(value, _restrictions))
                {
                    _restrictions = value;
                    Changed = true;
                    NotifyPropertyChanged("restrictions");
                }
            }
        }
        private Dictionary<string, string> _restrictions;

        /// <summary>
        /// The set of currently known metadata VDIs for this pool
        /// First published in XenServer 6.0.
        /// </summary>
        public virtual List<XenRef<VDI>> metadata_VDIs
        {
            get { return _metadata_VDIs; }
            set
            {
                if (!Helper.AreEqual(value, _metadata_VDIs))
                {
                    _metadata_VDIs = value;
                    Changed = true;
                    NotifyPropertyChanged("metadata_VDIs");
                }
            }
        }
        private List<XenRef<VDI>> _metadata_VDIs;

        /// <summary>
        /// The HA cluster stack that is currently in use. Only valid when HA is enabled.
        /// First published in XenServer Dundee.
        /// </summary>
        public virtual string ha_cluster_stack
        {
            get { return _ha_cluster_stack; }
            set
            {
                if (!Helper.AreEqual(value, _ha_cluster_stack))
                {
                    _ha_cluster_stack = value;
                    Changed = true;
                    NotifyPropertyChanged("ha_cluster_stack");
                }
            }
        }
        private string _ha_cluster_stack;

        /// <summary>
        /// list of the operations allowed in this state. This list is advisory only and the server state may have changed by the time this field is read by a client.
        /// </summary>
        public virtual List<pool_allowed_operations> allowed_operations
        {
            get { return _allowed_operations; }
            set
            {
                if (!Helper.AreEqual(value, _allowed_operations))
                {
                    _allowed_operations = value;
                    Changed = true;
                    NotifyPropertyChanged("allowed_operations");
                }
            }
        }
        private List<pool_allowed_operations> _allowed_operations;

        /// <summary>
        /// links each of the running tasks using this object (by reference) to a current_operation enum which describes the nature of the task.
        /// </summary>
        public virtual Dictionary<string, pool_allowed_operations> current_operations
        {
            get { return _current_operations; }
            set
            {
                if (!Helper.AreEqual(value, _current_operations))
                {
                    _current_operations = value;
                    Changed = true;
                    NotifyPropertyChanged("current_operations");
                }
            }
        }
        private Dictionary<string, pool_allowed_operations> _current_operations;

        /// <summary>
        /// Pool-wide guest agent configuration information
        /// First published in XenServer Dundee.
        /// </summary>
        public virtual Dictionary<string, string> guest_agent_config
        {
            get { return _guest_agent_config; }
            set
            {
                if (!Helper.AreEqual(value, _guest_agent_config))
                {
                    _guest_agent_config = value;
                    Changed = true;
                    NotifyPropertyChanged("guest_agent_config");
                }
            }
        }
        private Dictionary<string, string> _guest_agent_config;

        /// <summary>
        /// Details about the physical CPUs on the pool
        /// First published in XenServer Dundee.
        /// </summary>
        public virtual Dictionary<string, string> cpu_info
        {
            get { return _cpu_info; }
            set
            {
                if (!Helper.AreEqual(value, _cpu_info))
                {
                    _cpu_info = value;
                    Changed = true;
                    NotifyPropertyChanged("cpu_info");
                }
            }
        }
        private Dictionary<string, string> _cpu_info;

        /// <summary>
        /// The pool-wide policy for clients on whether to use the vendor device or not on newly created VMs. This field will also be consulted if the 'has_vendor_device' field is not specified in the VM.create call.
        /// First published in XenServer Dundee.
        /// </summary>
        public virtual bool policy_no_vendor_device
        {
            get { return _policy_no_vendor_device; }
            set
            {
                if (!Helper.AreEqual(value, _policy_no_vendor_device))
                {
                    _policy_no_vendor_device = value;
                    Changed = true;
                    NotifyPropertyChanged("policy_no_vendor_device");
                }
            }
        }
        private bool _policy_no_vendor_device;

        /// <summary>
        /// The pool-wide flag to show if the live patching feauture is disabled or not.
        /// First published in .
        /// </summary>
        public virtual bool live_patching_disabled
        {
            get { return _live_patching_disabled; }
            set
            {
                if (!Helper.AreEqual(value, _live_patching_disabled))
                {
                    _live_patching_disabled = value;
                    Changed = true;
                    NotifyPropertyChanged("live_patching_disabled");
                }
            }
        }
        private bool _live_patching_disabled;
    }
}
