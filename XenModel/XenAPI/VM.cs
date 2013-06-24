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
    public partial class VM : XenObject<VM>
    {
        public VM()
        {
        }

        public VM(string uuid,
            List<vm_operations> allowed_operations,
            Dictionary<string, vm_operations> current_operations,
            vm_power_state power_state,
            string name_label,
            string name_description,
            long user_version,
            bool is_a_template,
            XenRef<VDI> suspend_VDI,
            XenRef<Host> resident_on,
            XenRef<Host> affinity,
            long memory_overhead,
            long memory_target,
            long memory_static_max,
            long memory_dynamic_max,
            long memory_dynamic_min,
            long memory_static_min,
            Dictionary<string, string> VCPUs_params,
            long VCPUs_max,
            long VCPUs_at_startup,
            on_normal_exit actions_after_shutdown,
            on_normal_exit actions_after_reboot,
            on_crash_behaviour actions_after_crash,
            List<XenRef<Console>> consoles,
            List<XenRef<VIF>> VIFs,
            List<XenRef<VBD>> VBDs,
            List<XenRef<Crashdump>> crash_dumps,
            List<XenRef<VTPM>> VTPMs,
            string PV_bootloader,
            string PV_kernel,
            string PV_ramdisk,
            string PV_args,
            string PV_bootloader_args,
            string PV_legacy_args,
            string HVM_boot_policy,
            Dictionary<string, string> HVM_boot_params,
            double HVM_shadow_multiplier,
            Dictionary<string, string> platform,
            string PCI_bus,
            Dictionary<string, string> other_config,
            long domid,
            string domarch,
            Dictionary<string, string> last_boot_CPU_flags,
            bool is_control_domain,
            XenRef<VM_metrics> metrics,
            XenRef<VM_guest_metrics> guest_metrics,
            string last_booted_record,
            string recommendations,
            Dictionary<string, string> xenstore_data,
            bool ha_always_run,
            string ha_restart_priority,
            bool is_a_snapshot,
            XenRef<VM> snapshot_of,
            List<XenRef<VM>> snapshots,
            DateTime snapshot_time,
            string transportable_snapshot_id,
            Dictionary<string, XenRef<Blob>> blobs,
            string[] tags,
            Dictionary<vm_operations, string> blocked_operations,
            Dictionary<string, string> snapshot_info,
            string snapshot_metadata,
            XenRef<VM> parent,
            List<XenRef<VM>> children,
            Dictionary<string, string> bios_strings,
            XenRef<VMPP> protection_policy,
            bool is_snapshot_from_vmpp,
            XenRef<VM_appliance> appliance,
            long start_delay,
            long shutdown_delay,
            long order,
            List<XenRef<VGPU>> VGPUs,
            List<XenRef<PCI>> attached_PCIs,
            XenRef<SR> suspend_SR,
            long version,
            string generation_id)
        {
            this.uuid = uuid;
            this.allowed_operations = allowed_operations;
            this.current_operations = current_operations;
            this.power_state = power_state;
            this.name_label = name_label;
            this.name_description = name_description;
            this.user_version = user_version;
            this.is_a_template = is_a_template;
            this.suspend_VDI = suspend_VDI;
            this.resident_on = resident_on;
            this.affinity = affinity;
            this.memory_overhead = memory_overhead;
            this.memory_target = memory_target;
            this.memory_static_max = memory_static_max;
            this.memory_dynamic_max = memory_dynamic_max;
            this.memory_dynamic_min = memory_dynamic_min;
            this.memory_static_min = memory_static_min;
            this.VCPUs_params = VCPUs_params;
            this.VCPUs_max = VCPUs_max;
            this.VCPUs_at_startup = VCPUs_at_startup;
            this.actions_after_shutdown = actions_after_shutdown;
            this.actions_after_reboot = actions_after_reboot;
            this.actions_after_crash = actions_after_crash;
            this.consoles = consoles;
            this.VIFs = VIFs;
            this.VBDs = VBDs;
            this.crash_dumps = crash_dumps;
            this.VTPMs = VTPMs;
            this.PV_bootloader = PV_bootloader;
            this.PV_kernel = PV_kernel;
            this.PV_ramdisk = PV_ramdisk;
            this.PV_args = PV_args;
            this.PV_bootloader_args = PV_bootloader_args;
            this.PV_legacy_args = PV_legacy_args;
            this.HVM_boot_policy = HVM_boot_policy;
            this.HVM_boot_params = HVM_boot_params;
            this.HVM_shadow_multiplier = HVM_shadow_multiplier;
            this.platform = platform;
            this.PCI_bus = PCI_bus;
            this.other_config = other_config;
            this.domid = domid;
            this.domarch = domarch;
            this.last_boot_CPU_flags = last_boot_CPU_flags;
            this.is_control_domain = is_control_domain;
            this.metrics = metrics;
            this.guest_metrics = guest_metrics;
            this.last_booted_record = last_booted_record;
            this.recommendations = recommendations;
            this.xenstore_data = xenstore_data;
            this.ha_always_run = ha_always_run;
            this.ha_restart_priority = ha_restart_priority;
            this.is_a_snapshot = is_a_snapshot;
            this.snapshot_of = snapshot_of;
            this.snapshots = snapshots;
            this.snapshot_time = snapshot_time;
            this.transportable_snapshot_id = transportable_snapshot_id;
            this.blobs = blobs;
            this.tags = tags;
            this.blocked_operations = blocked_operations;
            this.snapshot_info = snapshot_info;
            this.snapshot_metadata = snapshot_metadata;
            this.parent = parent;
            this.children = children;
            this.bios_strings = bios_strings;
            this.protection_policy = protection_policy;
            this.is_snapshot_from_vmpp = is_snapshot_from_vmpp;
            this.appliance = appliance;
            this.start_delay = start_delay;
            this.shutdown_delay = shutdown_delay;
            this.order = order;
            this.VGPUs = VGPUs;
            this.attached_PCIs = attached_PCIs;
            this.suspend_SR = suspend_SR;
            this.version = version;
            this.generation_id = generation_id;
        }

        /// <summary>
        /// Creates a new VM from a Proxy_VM.
        /// </summary>
        /// <param name="proxy"></param>
        public VM(Proxy_VM proxy)
        {
            this.UpdateFromProxy(proxy);
        }

        public override void UpdateFrom(VM update)
        {
            uuid = update.uuid;
            allowed_operations = update.allowed_operations;
            current_operations = update.current_operations;
            power_state = update.power_state;
            name_label = update.name_label;
            name_description = update.name_description;
            user_version = update.user_version;
            is_a_template = update.is_a_template;
            suspend_VDI = update.suspend_VDI;
            resident_on = update.resident_on;
            affinity = update.affinity;
            memory_overhead = update.memory_overhead;
            memory_target = update.memory_target;
            memory_static_max = update.memory_static_max;
            memory_dynamic_max = update.memory_dynamic_max;
            memory_dynamic_min = update.memory_dynamic_min;
            memory_static_min = update.memory_static_min;
            VCPUs_params = update.VCPUs_params;
            VCPUs_max = update.VCPUs_max;
            VCPUs_at_startup = update.VCPUs_at_startup;
            actions_after_shutdown = update.actions_after_shutdown;
            actions_after_reboot = update.actions_after_reboot;
            actions_after_crash = update.actions_after_crash;
            consoles = update.consoles;
            VIFs = update.VIFs;
            VBDs = update.VBDs;
            crash_dumps = update.crash_dumps;
            VTPMs = update.VTPMs;
            PV_bootloader = update.PV_bootloader;
            PV_kernel = update.PV_kernel;
            PV_ramdisk = update.PV_ramdisk;
            PV_args = update.PV_args;
            PV_bootloader_args = update.PV_bootloader_args;
            PV_legacy_args = update.PV_legacy_args;
            HVM_boot_policy = update.HVM_boot_policy;
            HVM_boot_params = update.HVM_boot_params;
            HVM_shadow_multiplier = update.HVM_shadow_multiplier;
            platform = update.platform;
            PCI_bus = update.PCI_bus;
            other_config = update.other_config;
            domid = update.domid;
            domarch = update.domarch;
            last_boot_CPU_flags = update.last_boot_CPU_flags;
            is_control_domain = update.is_control_domain;
            metrics = update.metrics;
            guest_metrics = update.guest_metrics;
            last_booted_record = update.last_booted_record;
            recommendations = update.recommendations;
            xenstore_data = update.xenstore_data;
            ha_always_run = update.ha_always_run;
            ha_restart_priority = update.ha_restart_priority;
            is_a_snapshot = update.is_a_snapshot;
            snapshot_of = update.snapshot_of;
            snapshots = update.snapshots;
            snapshot_time = update.snapshot_time;
            transportable_snapshot_id = update.transportable_snapshot_id;
            blobs = update.blobs;
            tags = update.tags;
            blocked_operations = update.blocked_operations;
            snapshot_info = update.snapshot_info;
            snapshot_metadata = update.snapshot_metadata;
            parent = update.parent;
            children = update.children;
            bios_strings = update.bios_strings;
            protection_policy = update.protection_policy;
            is_snapshot_from_vmpp = update.is_snapshot_from_vmpp;
            appliance = update.appliance;
            start_delay = update.start_delay;
            shutdown_delay = update.shutdown_delay;
            order = update.order;
            VGPUs = update.VGPUs;
            attached_PCIs = update.attached_PCIs;
            suspend_SR = update.suspend_SR;
            version = update.version;
            generation_id = update.generation_id;
        }

        internal void UpdateFromProxy(Proxy_VM proxy)
        {
            uuid = proxy.uuid == null ? null : (string)proxy.uuid;
            allowed_operations = proxy.allowed_operations == null ? null : Helper.StringArrayToEnumList<vm_operations>(proxy.allowed_operations);
            current_operations = proxy.current_operations == null ? null : Maps.convert_from_proxy_string_vm_operations(proxy.current_operations);
            power_state = proxy.power_state == null ? (vm_power_state) 0 : (vm_power_state)Helper.EnumParseDefault(typeof(vm_power_state), (string)proxy.power_state);
            name_label = proxy.name_label == null ? null : (string)proxy.name_label;
            name_description = proxy.name_description == null ? null : (string)proxy.name_description;
            user_version = proxy.user_version == null ? 0 : long.Parse((string)proxy.user_version);
            is_a_template = (bool)proxy.is_a_template;
            suspend_VDI = proxy.suspend_VDI == null ? null : XenRef<VDI>.Create(proxy.suspend_VDI);
            resident_on = proxy.resident_on == null ? null : XenRef<Host>.Create(proxy.resident_on);
            affinity = proxy.affinity == null ? null : XenRef<Host>.Create(proxy.affinity);
            memory_overhead = proxy.memory_overhead == null ? 0 : long.Parse((string)proxy.memory_overhead);
            memory_target = proxy.memory_target == null ? 0 : long.Parse((string)proxy.memory_target);
            memory_static_max = proxy.memory_static_max == null ? 0 : long.Parse((string)proxy.memory_static_max);
            memory_dynamic_max = proxy.memory_dynamic_max == null ? 0 : long.Parse((string)proxy.memory_dynamic_max);
            memory_dynamic_min = proxy.memory_dynamic_min == null ? 0 : long.Parse((string)proxy.memory_dynamic_min);
            memory_static_min = proxy.memory_static_min == null ? 0 : long.Parse((string)proxy.memory_static_min);
            VCPUs_params = proxy.VCPUs_params == null ? null : Maps.convert_from_proxy_string_string(proxy.VCPUs_params);
            VCPUs_max = proxy.VCPUs_max == null ? 0 : long.Parse((string)proxy.VCPUs_max);
            VCPUs_at_startup = proxy.VCPUs_at_startup == null ? 0 : long.Parse((string)proxy.VCPUs_at_startup);
            actions_after_shutdown = proxy.actions_after_shutdown == null ? (on_normal_exit) 0 : (on_normal_exit)Helper.EnumParseDefault(typeof(on_normal_exit), (string)proxy.actions_after_shutdown);
            actions_after_reboot = proxy.actions_after_reboot == null ? (on_normal_exit) 0 : (on_normal_exit)Helper.EnumParseDefault(typeof(on_normal_exit), (string)proxy.actions_after_reboot);
            actions_after_crash = proxy.actions_after_crash == null ? (on_crash_behaviour) 0 : (on_crash_behaviour)Helper.EnumParseDefault(typeof(on_crash_behaviour), (string)proxy.actions_after_crash);
            consoles = proxy.consoles == null ? null : XenRef<Console>.Create(proxy.consoles);
            VIFs = proxy.VIFs == null ? null : XenRef<VIF>.Create(proxy.VIFs);
            VBDs = proxy.VBDs == null ? null : XenRef<VBD>.Create(proxy.VBDs);
            crash_dumps = proxy.crash_dumps == null ? null : XenRef<Crashdump>.Create(proxy.crash_dumps);
            VTPMs = proxy.VTPMs == null ? null : XenRef<VTPM>.Create(proxy.VTPMs);
            PV_bootloader = proxy.PV_bootloader == null ? null : (string)proxy.PV_bootloader;
            PV_kernel = proxy.PV_kernel == null ? null : (string)proxy.PV_kernel;
            PV_ramdisk = proxy.PV_ramdisk == null ? null : (string)proxy.PV_ramdisk;
            PV_args = proxy.PV_args == null ? null : (string)proxy.PV_args;
            PV_bootloader_args = proxy.PV_bootloader_args == null ? null : (string)proxy.PV_bootloader_args;
            PV_legacy_args = proxy.PV_legacy_args == null ? null : (string)proxy.PV_legacy_args;
            HVM_boot_policy = proxy.HVM_boot_policy == null ? null : (string)proxy.HVM_boot_policy;
            HVM_boot_params = proxy.HVM_boot_params == null ? null : Maps.convert_from_proxy_string_string(proxy.HVM_boot_params);
            HVM_shadow_multiplier = Convert.ToDouble(proxy.HVM_shadow_multiplier);
            platform = proxy.platform == null ? null : Maps.convert_from_proxy_string_string(proxy.platform);
            PCI_bus = proxy.PCI_bus == null ? null : (string)proxy.PCI_bus;
            other_config = proxy.other_config == null ? null : Maps.convert_from_proxy_string_string(proxy.other_config);
            domid = proxy.domid == null ? 0 : long.Parse((string)proxy.domid);
            domarch = proxy.domarch == null ? null : (string)proxy.domarch;
            last_boot_CPU_flags = proxy.last_boot_CPU_flags == null ? null : Maps.convert_from_proxy_string_string(proxy.last_boot_CPU_flags);
            is_control_domain = (bool)proxy.is_control_domain;
            metrics = proxy.metrics == null ? null : XenRef<VM_metrics>.Create(proxy.metrics);
            guest_metrics = proxy.guest_metrics == null ? null : XenRef<VM_guest_metrics>.Create(proxy.guest_metrics);
            last_booted_record = proxy.last_booted_record == null ? null : (string)proxy.last_booted_record;
            recommendations = proxy.recommendations == null ? null : (string)proxy.recommendations;
            xenstore_data = proxy.xenstore_data == null ? null : Maps.convert_from_proxy_string_string(proxy.xenstore_data);
            ha_always_run = (bool)proxy.ha_always_run;
            ha_restart_priority = proxy.ha_restart_priority == null ? null : (string)proxy.ha_restart_priority;
            is_a_snapshot = (bool)proxy.is_a_snapshot;
            snapshot_of = proxy.snapshot_of == null ? null : XenRef<VM>.Create(proxy.snapshot_of);
            snapshots = proxy.snapshots == null ? null : XenRef<VM>.Create(proxy.snapshots);
            snapshot_time = proxy.snapshot_time;
            transportable_snapshot_id = proxy.transportable_snapshot_id == null ? null : (string)proxy.transportable_snapshot_id;
            blobs = proxy.blobs == null ? null : Maps.convert_from_proxy_string_XenRefBlob(proxy.blobs);
            tags = proxy.tags == null ? new string[] {} : (string [])proxy.tags;
            blocked_operations = proxy.blocked_operations == null ? null : Maps.convert_from_proxy_vm_operations_string(proxy.blocked_operations);
            snapshot_info = proxy.snapshot_info == null ? null : Maps.convert_from_proxy_string_string(proxy.snapshot_info);
            snapshot_metadata = proxy.snapshot_metadata == null ? null : (string)proxy.snapshot_metadata;
            parent = proxy.parent == null ? null : XenRef<VM>.Create(proxy.parent);
            children = proxy.children == null ? null : XenRef<VM>.Create(proxy.children);
            bios_strings = proxy.bios_strings == null ? null : Maps.convert_from_proxy_string_string(proxy.bios_strings);
            protection_policy = proxy.protection_policy == null ? null : XenRef<VMPP>.Create(proxy.protection_policy);
            is_snapshot_from_vmpp = (bool)proxy.is_snapshot_from_vmpp;
            appliance = proxy.appliance == null ? null : XenRef<VM_appliance>.Create(proxy.appliance);
            start_delay = proxy.start_delay == null ? 0 : long.Parse((string)proxy.start_delay);
            shutdown_delay = proxy.shutdown_delay == null ? 0 : long.Parse((string)proxy.shutdown_delay);
            order = proxy.order == null ? 0 : long.Parse((string)proxy.order);
            VGPUs = proxy.VGPUs == null ? null : XenRef<VGPU>.Create(proxy.VGPUs);
            attached_PCIs = proxy.attached_PCIs == null ? null : XenRef<PCI>.Create(proxy.attached_PCIs);
            suspend_SR = proxy.suspend_SR == null ? null : XenRef<SR>.Create(proxy.suspend_SR);
            version = proxy.version == null ? 0 : long.Parse((string)proxy.version);
            generation_id = proxy.generation_id == null ? null : (string)proxy.generation_id;
        }

        public Proxy_VM ToProxy()
        {
            Proxy_VM result_ = new Proxy_VM();
            result_.uuid = (uuid != null) ? uuid : "";
            result_.allowed_operations = (allowed_operations != null) ? Helper.ObjectListToStringArray(allowed_operations) : new string[] {};
            result_.current_operations = Maps.convert_to_proxy_string_vm_operations(current_operations);
            result_.power_state = vm_power_state_helper.ToString(power_state);
            result_.name_label = (name_label != null) ? name_label : "";
            result_.name_description = (name_description != null) ? name_description : "";
            result_.user_version = user_version.ToString();
            result_.is_a_template = is_a_template;
            result_.suspend_VDI = (suspend_VDI != null) ? suspend_VDI : "";
            result_.resident_on = (resident_on != null) ? resident_on : "";
            result_.affinity = (affinity != null) ? affinity : "";
            result_.memory_overhead = memory_overhead.ToString();
            result_.memory_target = memory_target.ToString();
            result_.memory_static_max = memory_static_max.ToString();
            result_.memory_dynamic_max = memory_dynamic_max.ToString();
            result_.memory_dynamic_min = memory_dynamic_min.ToString();
            result_.memory_static_min = memory_static_min.ToString();
            result_.VCPUs_params = Maps.convert_to_proxy_string_string(VCPUs_params);
            result_.VCPUs_max = VCPUs_max.ToString();
            result_.VCPUs_at_startup = VCPUs_at_startup.ToString();
            result_.actions_after_shutdown = on_normal_exit_helper.ToString(actions_after_shutdown);
            result_.actions_after_reboot = on_normal_exit_helper.ToString(actions_after_reboot);
            result_.actions_after_crash = on_crash_behaviour_helper.ToString(actions_after_crash);
            result_.consoles = (consoles != null) ? Helper.RefListToStringArray(consoles) : new string[] {};
            result_.VIFs = (VIFs != null) ? Helper.RefListToStringArray(VIFs) : new string[] {};
            result_.VBDs = (VBDs != null) ? Helper.RefListToStringArray(VBDs) : new string[] {};
            result_.crash_dumps = (crash_dumps != null) ? Helper.RefListToStringArray(crash_dumps) : new string[] {};
            result_.VTPMs = (VTPMs != null) ? Helper.RefListToStringArray(VTPMs) : new string[] {};
            result_.PV_bootloader = (PV_bootloader != null) ? PV_bootloader : "";
            result_.PV_kernel = (PV_kernel != null) ? PV_kernel : "";
            result_.PV_ramdisk = (PV_ramdisk != null) ? PV_ramdisk : "";
            result_.PV_args = (PV_args != null) ? PV_args : "";
            result_.PV_bootloader_args = (PV_bootloader_args != null) ? PV_bootloader_args : "";
            result_.PV_legacy_args = (PV_legacy_args != null) ? PV_legacy_args : "";
            result_.HVM_boot_policy = (HVM_boot_policy != null) ? HVM_boot_policy : "";
            result_.HVM_boot_params = Maps.convert_to_proxy_string_string(HVM_boot_params);
            result_.HVM_shadow_multiplier = HVM_shadow_multiplier;
            result_.platform = Maps.convert_to_proxy_string_string(platform);
            result_.PCI_bus = (PCI_bus != null) ? PCI_bus : "";
            result_.other_config = Maps.convert_to_proxy_string_string(other_config);
            result_.domid = domid.ToString();
            result_.domarch = (domarch != null) ? domarch : "";
            result_.last_boot_CPU_flags = Maps.convert_to_proxy_string_string(last_boot_CPU_flags);
            result_.is_control_domain = is_control_domain;
            result_.metrics = (metrics != null) ? metrics : "";
            result_.guest_metrics = (guest_metrics != null) ? guest_metrics : "";
            result_.last_booted_record = (last_booted_record != null) ? last_booted_record : "";
            result_.recommendations = (recommendations != null) ? recommendations : "";
            result_.xenstore_data = Maps.convert_to_proxy_string_string(xenstore_data);
            result_.ha_always_run = ha_always_run;
            result_.ha_restart_priority = (ha_restart_priority != null) ? ha_restart_priority : "";
            result_.is_a_snapshot = is_a_snapshot;
            result_.snapshot_of = (snapshot_of != null) ? snapshot_of : "";
            result_.snapshots = (snapshots != null) ? Helper.RefListToStringArray(snapshots) : new string[] {};
            result_.snapshot_time = snapshot_time;
            result_.transportable_snapshot_id = (transportable_snapshot_id != null) ? transportable_snapshot_id : "";
            result_.blobs = Maps.convert_to_proxy_string_XenRefBlob(blobs);
            result_.tags = tags;
            result_.blocked_operations = Maps.convert_to_proxy_vm_operations_string(blocked_operations);
            result_.snapshot_info = Maps.convert_to_proxy_string_string(snapshot_info);
            result_.snapshot_metadata = (snapshot_metadata != null) ? snapshot_metadata : "";
            result_.parent = (parent != null) ? parent : "";
            result_.children = (children != null) ? Helper.RefListToStringArray(children) : new string[] {};
            result_.bios_strings = Maps.convert_to_proxy_string_string(bios_strings);
            result_.protection_policy = (protection_policy != null) ? protection_policy : "";
            result_.is_snapshot_from_vmpp = is_snapshot_from_vmpp;
            result_.appliance = (appliance != null) ? appliance : "";
            result_.start_delay = start_delay.ToString();
            result_.shutdown_delay = shutdown_delay.ToString();
            result_.order = order.ToString();
            result_.VGPUs = (VGPUs != null) ? Helper.RefListToStringArray(VGPUs) : new string[] {};
            result_.attached_PCIs = (attached_PCIs != null) ? Helper.RefListToStringArray(attached_PCIs) : new string[] {};
            result_.suspend_SR = (suspend_SR != null) ? suspend_SR : "";
            result_.version = version.ToString();
            result_.generation_id = (generation_id != null) ? generation_id : "";
            return result_;
        }

        /// <summary>
        /// Creates a new VM from a Hashtable.
        /// </summary>
        /// <param name="table"></param>
        public VM(Hashtable table)
        {
            uuid = Marshalling.ParseString(table, "uuid");
            allowed_operations = Helper.StringArrayToEnumList<vm_operations>(Marshalling.ParseStringArray(table, "allowed_operations"));
            current_operations = Maps.convert_from_proxy_string_vm_operations(Marshalling.ParseHashTable(table, "current_operations"));
            power_state = (vm_power_state)Helper.EnumParseDefault(typeof(vm_power_state), Marshalling.ParseString(table, "power_state"));
            name_label = Marshalling.ParseString(table, "name_label");
            name_description = Marshalling.ParseString(table, "name_description");
            user_version = Marshalling.ParseLong(table, "user_version");
            is_a_template = Marshalling.ParseBool(table, "is_a_template");
            suspend_VDI = Marshalling.ParseRef<VDI>(table, "suspend_VDI");
            resident_on = Marshalling.ParseRef<Host>(table, "resident_on");
            affinity = Marshalling.ParseRef<Host>(table, "affinity");
            memory_overhead = Marshalling.ParseLong(table, "memory_overhead");
            memory_target = Marshalling.ParseLong(table, "memory_target");
            memory_static_max = Marshalling.ParseLong(table, "memory_static_max");
            memory_dynamic_max = Marshalling.ParseLong(table, "memory_dynamic_max");
            memory_dynamic_min = Marshalling.ParseLong(table, "memory_dynamic_min");
            memory_static_min = Marshalling.ParseLong(table, "memory_static_min");
            VCPUs_params = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "VCPUs_params"));
            VCPUs_max = Marshalling.ParseLong(table, "VCPUs_max");
            VCPUs_at_startup = Marshalling.ParseLong(table, "VCPUs_at_startup");
            actions_after_shutdown = (on_normal_exit)Helper.EnumParseDefault(typeof(on_normal_exit), Marshalling.ParseString(table, "actions_after_shutdown"));
            actions_after_reboot = (on_normal_exit)Helper.EnumParseDefault(typeof(on_normal_exit), Marshalling.ParseString(table, "actions_after_reboot"));
            actions_after_crash = (on_crash_behaviour)Helper.EnumParseDefault(typeof(on_crash_behaviour), Marshalling.ParseString(table, "actions_after_crash"));
            consoles = Marshalling.ParseSetRef<Console>(table, "consoles");
            VIFs = Marshalling.ParseSetRef<VIF>(table, "VIFs");
            VBDs = Marshalling.ParseSetRef<VBD>(table, "VBDs");
            crash_dumps = Marshalling.ParseSetRef<Crashdump>(table, "crash_dumps");
            VTPMs = Marshalling.ParseSetRef<VTPM>(table, "VTPMs");
            PV_bootloader = Marshalling.ParseString(table, "PV_bootloader");
            PV_kernel = Marshalling.ParseString(table, "PV_kernel");
            PV_ramdisk = Marshalling.ParseString(table, "PV_ramdisk");
            PV_args = Marshalling.ParseString(table, "PV_args");
            PV_bootloader_args = Marshalling.ParseString(table, "PV_bootloader_args");
            PV_legacy_args = Marshalling.ParseString(table, "PV_legacy_args");
            HVM_boot_policy = Marshalling.ParseString(table, "HVM_boot_policy");
            HVM_boot_params = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "HVM_boot_params"));
            HVM_shadow_multiplier = Marshalling.ParseDouble(table, "HVM_shadow_multiplier");
            platform = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "platform"));
            PCI_bus = Marshalling.ParseString(table, "PCI_bus");
            other_config = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "other_config"));
            domid = Marshalling.ParseLong(table, "domid");
            domarch = Marshalling.ParseString(table, "domarch");
            last_boot_CPU_flags = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "last_boot_CPU_flags"));
            is_control_domain = Marshalling.ParseBool(table, "is_control_domain");
            metrics = Marshalling.ParseRef<VM_metrics>(table, "metrics");
            guest_metrics = Marshalling.ParseRef<VM_guest_metrics>(table, "guest_metrics");
            last_booted_record = Marshalling.ParseString(table, "last_booted_record");
            recommendations = Marshalling.ParseString(table, "recommendations");
            xenstore_data = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "xenstore_data"));
            ha_always_run = Marshalling.ParseBool(table, "ha_always_run");
            ha_restart_priority = Marshalling.ParseString(table, "ha_restart_priority");
            is_a_snapshot = Marshalling.ParseBool(table, "is_a_snapshot");
            snapshot_of = Marshalling.ParseRef<VM>(table, "snapshot_of");
            snapshots = Marshalling.ParseSetRef<VM>(table, "snapshots");
            snapshot_time = Marshalling.ParseDateTime(table, "snapshot_time");
            transportable_snapshot_id = Marshalling.ParseString(table, "transportable_snapshot_id");
            blobs = Maps.convert_from_proxy_string_XenRefBlob(Marshalling.ParseHashTable(table, "blobs"));
            tags = Marshalling.ParseStringArray(table, "tags");
            blocked_operations = Maps.convert_from_proxy_vm_operations_string(Marshalling.ParseHashTable(table, "blocked_operations"));
            snapshot_info = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "snapshot_info"));
            snapshot_metadata = Marshalling.ParseString(table, "snapshot_metadata");
            parent = Marshalling.ParseRef<VM>(table, "parent");
            children = Marshalling.ParseSetRef<VM>(table, "children");
            bios_strings = Maps.convert_from_proxy_string_string(Marshalling.ParseHashTable(table, "bios_strings"));
            protection_policy = Marshalling.ParseRef<VMPP>(table, "protection_policy");
            is_snapshot_from_vmpp = Marshalling.ParseBool(table, "is_snapshot_from_vmpp");
            appliance = Marshalling.ParseRef<VM_appliance>(table, "appliance");
            start_delay = Marshalling.ParseLong(table, "start_delay");
            shutdown_delay = Marshalling.ParseLong(table, "shutdown_delay");
            order = Marshalling.ParseLong(table, "order");
            VGPUs = Marshalling.ParseSetRef<VGPU>(table, "VGPUs");
            attached_PCIs = Marshalling.ParseSetRef<PCI>(table, "attached_PCIs");
            suspend_SR = Marshalling.ParseRef<SR>(table, "suspend_SR");
            version = Marshalling.ParseLong(table, "version");
            generation_id = Marshalling.ParseString(table, "generation_id");
        }

        public bool DeepEquals(VM other, bool ignoreCurrentOperations)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (!ignoreCurrentOperations && !Helper.AreEqual2(this.current_operations, other.current_operations))
                return false;

            return Helper.AreEqual2(this._uuid, other._uuid) &&
                Helper.AreEqual2(this._allowed_operations, other._allowed_operations) &&
                Helper.AreEqual2(this._power_state, other._power_state) &&
                Helper.AreEqual2(this._name_label, other._name_label) &&
                Helper.AreEqual2(this._name_description, other._name_description) &&
                Helper.AreEqual2(this._user_version, other._user_version) &&
                Helper.AreEqual2(this._is_a_template, other._is_a_template) &&
                Helper.AreEqual2(this._suspend_VDI, other._suspend_VDI) &&
                Helper.AreEqual2(this._resident_on, other._resident_on) &&
                Helper.AreEqual2(this._affinity, other._affinity) &&
                Helper.AreEqual2(this._memory_overhead, other._memory_overhead) &&
                Helper.AreEqual2(this._memory_target, other._memory_target) &&
                Helper.AreEqual2(this._memory_static_max, other._memory_static_max) &&
                Helper.AreEqual2(this._memory_dynamic_max, other._memory_dynamic_max) &&
                Helper.AreEqual2(this._memory_dynamic_min, other._memory_dynamic_min) &&
                Helper.AreEqual2(this._memory_static_min, other._memory_static_min) &&
                Helper.AreEqual2(this._VCPUs_params, other._VCPUs_params) &&
                Helper.AreEqual2(this._VCPUs_max, other._VCPUs_max) &&
                Helper.AreEqual2(this._VCPUs_at_startup, other._VCPUs_at_startup) &&
                Helper.AreEqual2(this._actions_after_shutdown, other._actions_after_shutdown) &&
                Helper.AreEqual2(this._actions_after_reboot, other._actions_after_reboot) &&
                Helper.AreEqual2(this._actions_after_crash, other._actions_after_crash) &&
                Helper.AreEqual2(this._consoles, other._consoles) &&
                Helper.AreEqual2(this._VIFs, other._VIFs) &&
                Helper.AreEqual2(this._VBDs, other._VBDs) &&
                Helper.AreEqual2(this._crash_dumps, other._crash_dumps) &&
                Helper.AreEqual2(this._VTPMs, other._VTPMs) &&
                Helper.AreEqual2(this._PV_bootloader, other._PV_bootloader) &&
                Helper.AreEqual2(this._PV_kernel, other._PV_kernel) &&
                Helper.AreEqual2(this._PV_ramdisk, other._PV_ramdisk) &&
                Helper.AreEqual2(this._PV_args, other._PV_args) &&
                Helper.AreEqual2(this._PV_bootloader_args, other._PV_bootloader_args) &&
                Helper.AreEqual2(this._PV_legacy_args, other._PV_legacy_args) &&
                Helper.AreEqual2(this._HVM_boot_policy, other._HVM_boot_policy) &&
                Helper.AreEqual2(this._HVM_boot_params, other._HVM_boot_params) &&
                Helper.AreEqual2(this._HVM_shadow_multiplier, other._HVM_shadow_multiplier) &&
                Helper.AreEqual2(this._platform, other._platform) &&
                Helper.AreEqual2(this._PCI_bus, other._PCI_bus) &&
                Helper.AreEqual2(this._other_config, other._other_config) &&
                Helper.AreEqual2(this._domid, other._domid) &&
                Helper.AreEqual2(this._domarch, other._domarch) &&
                Helper.AreEqual2(this._last_boot_CPU_flags, other._last_boot_CPU_flags) &&
                Helper.AreEqual2(this._is_control_domain, other._is_control_domain) &&
                Helper.AreEqual2(this._metrics, other._metrics) &&
                Helper.AreEqual2(this._guest_metrics, other._guest_metrics) &&
                Helper.AreEqual2(this._last_booted_record, other._last_booted_record) &&
                Helper.AreEqual2(this._recommendations, other._recommendations) &&
                Helper.AreEqual2(this._xenstore_data, other._xenstore_data) &&
                Helper.AreEqual2(this._ha_always_run, other._ha_always_run) &&
                Helper.AreEqual2(this._ha_restart_priority, other._ha_restart_priority) &&
                Helper.AreEqual2(this._is_a_snapshot, other._is_a_snapshot) &&
                Helper.AreEqual2(this._snapshot_of, other._snapshot_of) &&
                Helper.AreEqual2(this._snapshots, other._snapshots) &&
                Helper.AreEqual2(this._snapshot_time, other._snapshot_time) &&
                Helper.AreEqual2(this._transportable_snapshot_id, other._transportable_snapshot_id) &&
                Helper.AreEqual2(this._blobs, other._blobs) &&
                Helper.AreEqual2(this._tags, other._tags) &&
                Helper.AreEqual2(this._blocked_operations, other._blocked_operations) &&
                Helper.AreEqual2(this._snapshot_info, other._snapshot_info) &&
                Helper.AreEqual2(this._snapshot_metadata, other._snapshot_metadata) &&
                Helper.AreEqual2(this._parent, other._parent) &&
                Helper.AreEqual2(this._children, other._children) &&
                Helper.AreEqual2(this._bios_strings, other._bios_strings) &&
                Helper.AreEqual2(this._protection_policy, other._protection_policy) &&
                Helper.AreEqual2(this._is_snapshot_from_vmpp, other._is_snapshot_from_vmpp) &&
                Helper.AreEqual2(this._appliance, other._appliance) &&
                Helper.AreEqual2(this._start_delay, other._start_delay) &&
                Helper.AreEqual2(this._shutdown_delay, other._shutdown_delay) &&
                Helper.AreEqual2(this._order, other._order) &&
                Helper.AreEqual2(this._VGPUs, other._VGPUs) &&
                Helper.AreEqual2(this._attached_PCIs, other._attached_PCIs) &&
                Helper.AreEqual2(this._suspend_SR, other._suspend_SR) &&
                Helper.AreEqual2(this._version, other._version) &&
                Helper.AreEqual2(this._generation_id, other._generation_id);
        }

        public override string SaveChanges(Session session, string opaqueRef, VM server)
        {
            if (opaqueRef == null)
            {
                Proxy_VM p = this.ToProxy();
                return session.proxy.vm_create(session.uuid, p).parse();
            }
            else
            {
                if (!Helper.AreEqual2(_name_label, server._name_label))
                {
                    VM.set_name_label(session, opaqueRef, _name_label);
                }
                if (!Helper.AreEqual2(_name_description, server._name_description))
                {
                    VM.set_name_description(session, opaqueRef, _name_description);
                }
                if (!Helper.AreEqual2(_user_version, server._user_version))
                {
                    VM.set_user_version(session, opaqueRef, _user_version);
                }
                if (!Helper.AreEqual2(_is_a_template, server._is_a_template))
                {
                    VM.set_is_a_template(session, opaqueRef, _is_a_template);
                }
                if (!Helper.AreEqual2(_affinity, server._affinity))
                {
                    VM.set_affinity(session, opaqueRef, _affinity);
                }
                if (!Helper.AreEqual2(_VCPUs_params, server._VCPUs_params))
                {
                    VM.set_VCPUs_params(session, opaqueRef, _VCPUs_params);
                }
                if (!Helper.AreEqual2(_actions_after_shutdown, server._actions_after_shutdown))
                {
                    VM.set_actions_after_shutdown(session, opaqueRef, _actions_after_shutdown);
                }
                if (!Helper.AreEqual2(_actions_after_reboot, server._actions_after_reboot))
                {
                    VM.set_actions_after_reboot(session, opaqueRef, _actions_after_reboot);
                }
                if (!Helper.AreEqual2(_actions_after_crash, server._actions_after_crash))
                {
                    VM.set_actions_after_crash(session, opaqueRef, _actions_after_crash);
                }
                if (!Helper.AreEqual2(_PV_bootloader, server._PV_bootloader))
                {
                    VM.set_PV_bootloader(session, opaqueRef, _PV_bootloader);
                }
                if (!Helper.AreEqual2(_PV_kernel, server._PV_kernel))
                {
                    VM.set_PV_kernel(session, opaqueRef, _PV_kernel);
                }
                if (!Helper.AreEqual2(_PV_ramdisk, server._PV_ramdisk))
                {
                    VM.set_PV_ramdisk(session, opaqueRef, _PV_ramdisk);
                }
                if (!Helper.AreEqual2(_PV_args, server._PV_args))
                {
                    VM.set_PV_args(session, opaqueRef, _PV_args);
                }
                if (!Helper.AreEqual2(_PV_bootloader_args, server._PV_bootloader_args))
                {
                    VM.set_PV_bootloader_args(session, opaqueRef, _PV_bootloader_args);
                }
                if (!Helper.AreEqual2(_PV_legacy_args, server._PV_legacy_args))
                {
                    VM.set_PV_legacy_args(session, opaqueRef, _PV_legacy_args);
                }
                if (!Helper.AreEqual2(_HVM_boot_policy, server._HVM_boot_policy))
                {
                    VM.set_HVM_boot_policy(session, opaqueRef, _HVM_boot_policy);
                }
                if (!Helper.AreEqual2(_HVM_boot_params, server._HVM_boot_params))
                {
                    VM.set_HVM_boot_params(session, opaqueRef, _HVM_boot_params);
                }
                if (!Helper.AreEqual2(_platform, server._platform))
                {
                    VM.set_platform(session, opaqueRef, _platform);
                }
                if (!Helper.AreEqual2(_PCI_bus, server._PCI_bus))
                {
                    VM.set_PCI_bus(session, opaqueRef, _PCI_bus);
                }
                if (!Helper.AreEqual2(_other_config, server._other_config))
                {
                    VM.set_other_config(session, opaqueRef, _other_config);
                }
                if (!Helper.AreEqual2(_recommendations, server._recommendations))
                {
                    VM.set_recommendations(session, opaqueRef, _recommendations);
                }
                if (!Helper.AreEqual2(_xenstore_data, server._xenstore_data))
                {
                    VM.set_xenstore_data(session, opaqueRef, _xenstore_data);
                }
                if (!Helper.AreEqual2(_tags, server._tags))
                {
                    VM.set_tags(session, opaqueRef, _tags);
                }
                if (!Helper.AreEqual2(_blocked_operations, server._blocked_operations))
                {
                    VM.set_blocked_operations(session, opaqueRef, _blocked_operations);
                }
                if (!Helper.AreEqual2(_suspend_SR, server._suspend_SR))
                {
                    VM.set_suspend_SR(session, opaqueRef, _suspend_SR);
                }
                if (!Helper.AreEqual2(_memory_static_max, server._memory_static_max))
                {
                    VM.set_memory_static_max(session, opaqueRef, _memory_static_max);
                }
                if (!Helper.AreEqual2(_memory_dynamic_max, server._memory_dynamic_max))
                {
                    VM.set_memory_dynamic_max(session, opaqueRef, _memory_dynamic_max);
                }
                if (!Helper.AreEqual2(_memory_dynamic_min, server._memory_dynamic_min))
                {
                    VM.set_memory_dynamic_min(session, opaqueRef, _memory_dynamic_min);
                }
                if (!Helper.AreEqual2(_memory_static_min, server._memory_static_min))
                {
                    VM.set_memory_static_min(session, opaqueRef, _memory_static_min);
                }
                if (!Helper.AreEqual2(_VCPUs_max, server._VCPUs_max))
                {
                    VM.set_VCPUs_max(session, opaqueRef, _VCPUs_max);
                }
                if (!Helper.AreEqual2(_VCPUs_at_startup, server._VCPUs_at_startup))
                {
                    VM.set_VCPUs_at_startup(session, opaqueRef, _VCPUs_at_startup);
                }
                if (!Helper.AreEqual2(_HVM_shadow_multiplier, server._HVM_shadow_multiplier))
                {
                    VM.set_HVM_shadow_multiplier(session, opaqueRef, _HVM_shadow_multiplier);
                }
                if (!Helper.AreEqual2(_ha_always_run, server._ha_always_run))
                {
                    VM.set_ha_always_run(session, opaqueRef, _ha_always_run);
                }
                if (!Helper.AreEqual2(_ha_restart_priority, server._ha_restart_priority))
                {
                    VM.set_ha_restart_priority(session, opaqueRef, _ha_restart_priority);
                }
                if (!Helper.AreEqual2(_protection_policy, server._protection_policy))
                {
                    VM.set_protection_policy(session, opaqueRef, _protection_policy);
                }
                if (!Helper.AreEqual2(_appliance, server._appliance))
                {
                    VM.set_appliance(session, opaqueRef, _appliance);
                }
                if (!Helper.AreEqual2(_start_delay, server._start_delay))
                {
                    VM.set_start_delay(session, opaqueRef, _start_delay);
                }
                if (!Helper.AreEqual2(_shutdown_delay, server._shutdown_delay))
                {
                    VM.set_shutdown_delay(session, opaqueRef, _shutdown_delay);
                }
                if (!Helper.AreEqual2(_order, server._order))
                {
                    VM.set_order(session, opaqueRef, _order);
                }

                return null;
            }
        }

        public static VM get_record(Session session, string _vm)
        {
            return new VM((Proxy_VM)session.proxy.vm_get_record(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static XenRef<VM> get_by_uuid(Session session, string _uuid)
        {
            return XenRef<VM>.Create(session.proxy.vm_get_by_uuid(session.uuid, (_uuid != null) ? _uuid : "").parse());
        }

        public static XenRef<VM> create(Session session, VM _record)
        {
            return XenRef<VM>.Create(session.proxy.vm_create(session.uuid, _record.ToProxy()).parse());
        }

        public static XenRef<Task> async_create(Session session, VM _record)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_create(session.uuid, _record.ToProxy()).parse());
        }

        public static void destroy(Session session, string _vm)
        {
            session.proxy.vm_destroy(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<Task> async_destroy(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_destroy(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static List<XenRef<VM>> get_by_name_label(Session session, string _label)
        {
            return XenRef<VM>.Create(session.proxy.vm_get_by_name_label(session.uuid, (_label != null) ? _label : "").parse());
        }

        public static string get_uuid(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_uuid(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static List<vm_operations> get_allowed_operations(Session session, string _vm)
        {
            return Helper.StringArrayToEnumList<vm_operations>(session.proxy.vm_get_allowed_operations(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static Dictionary<string, vm_operations> get_current_operations(Session session, string _vm)
        {
            return Maps.convert_from_proxy_string_vm_operations(session.proxy.vm_get_current_operations(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static vm_power_state get_power_state(Session session, string _vm)
        {
            return (vm_power_state)Helper.EnumParseDefault(typeof(vm_power_state), (string)session.proxy.vm_get_power_state(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static string get_name_label(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_name_label(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static string get_name_description(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_name_description(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static long get_user_version(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_user_version(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static bool get_is_a_template(Session session, string _vm)
        {
            return (bool)session.proxy.vm_get_is_a_template(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<VDI> get_suspend_VDI(Session session, string _vm)
        {
            return XenRef<VDI>.Create(session.proxy.vm_get_suspend_vdi(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static XenRef<Host> get_resident_on(Session session, string _vm)
        {
            return XenRef<Host>.Create(session.proxy.vm_get_resident_on(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static XenRef<Host> get_affinity(Session session, string _vm)
        {
            return XenRef<Host>.Create(session.proxy.vm_get_affinity(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_memory_overhead(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_memory_overhead(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_memory_target(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_memory_target(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_memory_static_max(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_memory_static_max(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_memory_dynamic_max(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_memory_dynamic_max(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_memory_dynamic_min(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_memory_dynamic_min(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_memory_static_min(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_memory_static_min(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static Dictionary<string, string> get_VCPUs_params(Session session, string _vm)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_get_vcpus_params(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_VCPUs_max(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_vcpus_max(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_VCPUs_at_startup(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_vcpus_at_startup(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static on_normal_exit get_actions_after_shutdown(Session session, string _vm)
        {
            return (on_normal_exit)Helper.EnumParseDefault(typeof(on_normal_exit), (string)session.proxy.vm_get_actions_after_shutdown(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static on_normal_exit get_actions_after_reboot(Session session, string _vm)
        {
            return (on_normal_exit)Helper.EnumParseDefault(typeof(on_normal_exit), (string)session.proxy.vm_get_actions_after_reboot(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static on_crash_behaviour get_actions_after_crash(Session session, string _vm)
        {
            return (on_crash_behaviour)Helper.EnumParseDefault(typeof(on_crash_behaviour), (string)session.proxy.vm_get_actions_after_crash(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static List<XenRef<Console>> get_consoles(Session session, string _vm)
        {
            return XenRef<Console>.Create(session.proxy.vm_get_consoles(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static List<XenRef<VIF>> get_VIFs(Session session, string _vm)
        {
            return XenRef<VIF>.Create(session.proxy.vm_get_vifs(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static List<XenRef<VBD>> get_VBDs(Session session, string _vm)
        {
            return XenRef<VBD>.Create(session.proxy.vm_get_vbds(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static List<XenRef<Crashdump>> get_crash_dumps(Session session, string _vm)
        {
            return XenRef<Crashdump>.Create(session.proxy.vm_get_crash_dumps(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static List<XenRef<VTPM>> get_VTPMs(Session session, string _vm)
        {
            return XenRef<VTPM>.Create(session.proxy.vm_get_vtpms(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static string get_PV_bootloader(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_pv_bootloader(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static string get_PV_kernel(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_pv_kernel(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static string get_PV_ramdisk(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_pv_ramdisk(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static string get_PV_args(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_pv_args(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static string get_PV_bootloader_args(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_pv_bootloader_args(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static string get_PV_legacy_args(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_pv_legacy_args(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static string get_HVM_boot_policy(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_hvm_boot_policy(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static Dictionary<string, string> get_HVM_boot_params(Session session, string _vm)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_get_hvm_boot_params(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static double get_HVM_shadow_multiplier(Session session, string _vm)
        {
            return Convert.ToDouble(session.proxy.vm_get_hvm_shadow_multiplier(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static Dictionary<string, string> get_platform(Session session, string _vm)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_get_platform(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static string get_PCI_bus(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_pci_bus(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static Dictionary<string, string> get_other_config(Session session, string _vm)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_get_other_config(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_domid(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_domid(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static string get_domarch(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_domarch(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static Dictionary<string, string> get_last_boot_CPU_flags(Session session, string _vm)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_get_last_boot_cpu_flags(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static bool get_is_control_domain(Session session, string _vm)
        {
            return (bool)session.proxy.vm_get_is_control_domain(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<VM_metrics> get_metrics(Session session, string _vm)
        {
            return XenRef<VM_metrics>.Create(session.proxy.vm_get_metrics(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static XenRef<VM_guest_metrics> get_guest_metrics(Session session, string _vm)
        {
            return XenRef<VM_guest_metrics>.Create(session.proxy.vm_get_guest_metrics(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static string get_last_booted_record(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_last_booted_record(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static string get_recommendations(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_recommendations(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static Dictionary<string, string> get_xenstore_data(Session session, string _vm)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_get_xenstore_data(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static bool get_ha_always_run(Session session, string _vm)
        {
            return (bool)session.proxy.vm_get_ha_always_run(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static string get_ha_restart_priority(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_ha_restart_priority(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static bool get_is_a_snapshot(Session session, string _vm)
        {
            return (bool)session.proxy.vm_get_is_a_snapshot(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<VM> get_snapshot_of(Session session, string _vm)
        {
            return XenRef<VM>.Create(session.proxy.vm_get_snapshot_of(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static List<XenRef<VM>> get_snapshots(Session session, string _vm)
        {
            return XenRef<VM>.Create(session.proxy.vm_get_snapshots(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static DateTime get_snapshot_time(Session session, string _vm)
        {
            return session.proxy.vm_get_snapshot_time(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static string get_transportable_snapshot_id(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_transportable_snapshot_id(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static Dictionary<string, XenRef<Blob>> get_blobs(Session session, string _vm)
        {
            return Maps.convert_from_proxy_string_XenRefBlob(session.proxy.vm_get_blobs(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static string[] get_tags(Session session, string _vm)
        {
            return (string [])session.proxy.vm_get_tags(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static Dictionary<vm_operations, string> get_blocked_operations(Session session, string _vm)
        {
            return Maps.convert_from_proxy_vm_operations_string(session.proxy.vm_get_blocked_operations(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static Dictionary<string, string> get_snapshot_info(Session session, string _vm)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_get_snapshot_info(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static string get_snapshot_metadata(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_snapshot_metadata(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<VM> get_parent(Session session, string _vm)
        {
            return XenRef<VM>.Create(session.proxy.vm_get_parent(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static List<XenRef<VM>> get_children(Session session, string _vm)
        {
            return XenRef<VM>.Create(session.proxy.vm_get_children(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static Dictionary<string, string> get_bios_strings(Session session, string _vm)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_get_bios_strings(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static XenRef<VMPP> get_protection_policy(Session session, string _vm)
        {
            return XenRef<VMPP>.Create(session.proxy.vm_get_protection_policy(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static bool get_is_snapshot_from_vmpp(Session session, string _vm)
        {
            return (bool)session.proxy.vm_get_is_snapshot_from_vmpp(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<VM_appliance> get_appliance(Session session, string _vm)
        {
            return XenRef<VM_appliance>.Create(session.proxy.vm_get_appliance(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_start_delay(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_start_delay(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_shutdown_delay(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_shutdown_delay(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_order(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_order(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static List<XenRef<VGPU>> get_VGPUs(Session session, string _vm)
        {
            return XenRef<VGPU>.Create(session.proxy.vm_get_vgpus(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static List<XenRef<PCI>> get_attached_PCIs(Session session, string _vm)
        {
            return XenRef<PCI>.Create(session.proxy.vm_get_attached_pcis(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static XenRef<SR> get_suspend_SR(Session session, string _vm)
        {
            return XenRef<SR>.Create(session.proxy.vm_get_suspend_sr(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static long get_version(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_get_version(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static string get_generation_id(Session session, string _vm)
        {
            return (string)session.proxy.vm_get_generation_id(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static void set_name_label(Session session, string _vm, string _label)
        {
            session.proxy.vm_set_name_label(session.uuid, (_vm != null) ? _vm : "", (_label != null) ? _label : "").parse();
        }

        public static void set_name_description(Session session, string _vm, string _description)
        {
            session.proxy.vm_set_name_description(session.uuid, (_vm != null) ? _vm : "", (_description != null) ? _description : "").parse();
        }

        public static void set_user_version(Session session, string _vm, long _user_version)
        {
            session.proxy.vm_set_user_version(session.uuid, (_vm != null) ? _vm : "", _user_version.ToString()).parse();
        }

        public static void set_is_a_template(Session session, string _vm, bool _is_a_template)
        {
            session.proxy.vm_set_is_a_template(session.uuid, (_vm != null) ? _vm : "", _is_a_template).parse();
        }

        public static void set_affinity(Session session, string _vm, string _affinity)
        {
            session.proxy.vm_set_affinity(session.uuid, (_vm != null) ? _vm : "", (_affinity != null) ? _affinity : "").parse();
        }

        public static void set_VCPUs_params(Session session, string _vm, Dictionary<string, string> _params)
        {
            session.proxy.vm_set_vcpus_params(session.uuid, (_vm != null) ? _vm : "", Maps.convert_to_proxy_string_string(_params)).parse();
        }

        public static void add_to_VCPUs_params(Session session, string _vm, string _key, string _value)
        {
            session.proxy.vm_add_to_vcpus_params(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_VCPUs_params(Session session, string _vm, string _key)
        {
            session.proxy.vm_remove_from_vcpus_params(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "").parse();
        }

        public static void set_actions_after_shutdown(Session session, string _vm, on_normal_exit _after_shutdown)
        {
            session.proxy.vm_set_actions_after_shutdown(session.uuid, (_vm != null) ? _vm : "", on_normal_exit_helper.ToString(_after_shutdown)).parse();
        }

        public static void set_actions_after_reboot(Session session, string _vm, on_normal_exit _after_reboot)
        {
            session.proxy.vm_set_actions_after_reboot(session.uuid, (_vm != null) ? _vm : "", on_normal_exit_helper.ToString(_after_reboot)).parse();
        }

        public static void set_actions_after_crash(Session session, string _vm, on_crash_behaviour _after_crash)
        {
            session.proxy.vm_set_actions_after_crash(session.uuid, (_vm != null) ? _vm : "", on_crash_behaviour_helper.ToString(_after_crash)).parse();
        }

        public static void set_PV_bootloader(Session session, string _vm, string _bootloader)
        {
            session.proxy.vm_set_pv_bootloader(session.uuid, (_vm != null) ? _vm : "", (_bootloader != null) ? _bootloader : "").parse();
        }

        public static void set_PV_kernel(Session session, string _vm, string _kernel)
        {
            session.proxy.vm_set_pv_kernel(session.uuid, (_vm != null) ? _vm : "", (_kernel != null) ? _kernel : "").parse();
        }

        public static void set_PV_ramdisk(Session session, string _vm, string _ramdisk)
        {
            session.proxy.vm_set_pv_ramdisk(session.uuid, (_vm != null) ? _vm : "", (_ramdisk != null) ? _ramdisk : "").parse();
        }

        public static void set_PV_args(Session session, string _vm, string _args)
        {
            session.proxy.vm_set_pv_args(session.uuid, (_vm != null) ? _vm : "", (_args != null) ? _args : "").parse();
        }

        public static void set_PV_bootloader_args(Session session, string _vm, string _bootloader_args)
        {
            session.proxy.vm_set_pv_bootloader_args(session.uuid, (_vm != null) ? _vm : "", (_bootloader_args != null) ? _bootloader_args : "").parse();
        }

        public static void set_PV_legacy_args(Session session, string _vm, string _legacy_args)
        {
            session.proxy.vm_set_pv_legacy_args(session.uuid, (_vm != null) ? _vm : "", (_legacy_args != null) ? _legacy_args : "").parse();
        }

        public static void set_HVM_boot_policy(Session session, string _vm, string _boot_policy)
        {
            session.proxy.vm_set_hvm_boot_policy(session.uuid, (_vm != null) ? _vm : "", (_boot_policy != null) ? _boot_policy : "").parse();
        }

        public static void set_HVM_boot_params(Session session, string _vm, Dictionary<string, string> _boot_params)
        {
            session.proxy.vm_set_hvm_boot_params(session.uuid, (_vm != null) ? _vm : "", Maps.convert_to_proxy_string_string(_boot_params)).parse();
        }

        public static void add_to_HVM_boot_params(Session session, string _vm, string _key, string _value)
        {
            session.proxy.vm_add_to_hvm_boot_params(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_HVM_boot_params(Session session, string _vm, string _key)
        {
            session.proxy.vm_remove_from_hvm_boot_params(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "").parse();
        }

        public static void set_platform(Session session, string _vm, Dictionary<string, string> _platform)
        {
            session.proxy.vm_set_platform(session.uuid, (_vm != null) ? _vm : "", Maps.convert_to_proxy_string_string(_platform)).parse();
        }

        public static void add_to_platform(Session session, string _vm, string _key, string _value)
        {
            session.proxy.vm_add_to_platform(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_platform(Session session, string _vm, string _key)
        {
            session.proxy.vm_remove_from_platform(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "").parse();
        }

        public static void set_PCI_bus(Session session, string _vm, string _pci_bus)
        {
            session.proxy.vm_set_pci_bus(session.uuid, (_vm != null) ? _vm : "", (_pci_bus != null) ? _pci_bus : "").parse();
        }

        public static void set_other_config(Session session, string _vm, Dictionary<string, string> _other_config)
        {
            session.proxy.vm_set_other_config(session.uuid, (_vm != null) ? _vm : "", Maps.convert_to_proxy_string_string(_other_config)).parse();
        }

        public static void add_to_other_config(Session session, string _vm, string _key, string _value)
        {
            session.proxy.vm_add_to_other_config(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_other_config(Session session, string _vm, string _key)
        {
            session.proxy.vm_remove_from_other_config(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "").parse();
        }

        public static void set_recommendations(Session session, string _vm, string _recommendations)
        {
            session.proxy.vm_set_recommendations(session.uuid, (_vm != null) ? _vm : "", (_recommendations != null) ? _recommendations : "").parse();
        }

        public static void set_xenstore_data(Session session, string _vm, Dictionary<string, string> _xenstore_data)
        {
            session.proxy.vm_set_xenstore_data(session.uuid, (_vm != null) ? _vm : "", Maps.convert_to_proxy_string_string(_xenstore_data)).parse();
        }

        public static void add_to_xenstore_data(Session session, string _vm, string _key, string _value)
        {
            session.proxy.vm_add_to_xenstore_data(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_from_xenstore_data(Session session, string _vm, string _key)
        {
            session.proxy.vm_remove_from_xenstore_data(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "").parse();
        }

        public static void set_tags(Session session, string _vm, string[] _tags)
        {
            session.proxy.vm_set_tags(session.uuid, (_vm != null) ? _vm : "", _tags).parse();
        }

        public static void add_tags(Session session, string _vm, string _value)
        {
            session.proxy.vm_add_tags(session.uuid, (_vm != null) ? _vm : "", (_value != null) ? _value : "").parse();
        }

        public static void remove_tags(Session session, string _vm, string _value)
        {
            session.proxy.vm_remove_tags(session.uuid, (_vm != null) ? _vm : "", (_value != null) ? _value : "").parse();
        }

        public static void set_blocked_operations(Session session, string _vm, Dictionary<vm_operations, string> _blocked_operations)
        {
            session.proxy.vm_set_blocked_operations(session.uuid, (_vm != null) ? _vm : "", Maps.convert_to_proxy_vm_operations_string(_blocked_operations)).parse();
        }

        public static void add_to_blocked_operations(Session session, string _vm, vm_operations _key, string _value)
        {
            session.proxy.vm_add_to_blocked_operations(session.uuid, (_vm != null) ? _vm : "", vm_operations_helper.ToString(_key), (_value != null) ? _value : "").parse();
        }

        public static void remove_from_blocked_operations(Session session, string _vm, vm_operations _key)
        {
            session.proxy.vm_remove_from_blocked_operations(session.uuid, (_vm != null) ? _vm : "", vm_operations_helper.ToString(_key)).parse();
        }

        public static void set_suspend_SR(Session session, string _vm, string _suspend_sr)
        {
            session.proxy.vm_set_suspend_sr(session.uuid, (_vm != null) ? _vm : "", (_suspend_sr != null) ? _suspend_sr : "").parse();
        }

        public static XenRef<VM> snapshot(Session session, string _vm, string _new_name)
        {
            return XenRef<VM>.Create(session.proxy.vm_snapshot(session.uuid, (_vm != null) ? _vm : "", (_new_name != null) ? _new_name : "").parse());
        }

        public static XenRef<Task> async_snapshot(Session session, string _vm, string _new_name)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_snapshot(session.uuid, (_vm != null) ? _vm : "", (_new_name != null) ? _new_name : "").parse());
        }

        public static XenRef<VM> snapshot_with_quiesce(Session session, string _vm, string _new_name)
        {
            return XenRef<VM>.Create(session.proxy.vm_snapshot_with_quiesce(session.uuid, (_vm != null) ? _vm : "", (_new_name != null) ? _new_name : "").parse());
        }

        public static XenRef<Task> async_snapshot_with_quiesce(Session session, string _vm, string _new_name)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_snapshot_with_quiesce(session.uuid, (_vm != null) ? _vm : "", (_new_name != null) ? _new_name : "").parse());
        }

        public static XenRef<VM> clone(Session session, string _vm, string _new_name)
        {
            return XenRef<VM>.Create(session.proxy.vm_clone(session.uuid, (_vm != null) ? _vm : "", (_new_name != null) ? _new_name : "").parse());
        }

        public static XenRef<Task> async_clone(Session session, string _vm, string _new_name)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_clone(session.uuid, (_vm != null) ? _vm : "", (_new_name != null) ? _new_name : "").parse());
        }

        public static XenRef<VM> copy(Session session, string _vm, string _new_name, string _sr)
        {
            return XenRef<VM>.Create(session.proxy.vm_copy(session.uuid, (_vm != null) ? _vm : "", (_new_name != null) ? _new_name : "", (_sr != null) ? _sr : "").parse());
        }

        public static XenRef<Task> async_copy(Session session, string _vm, string _new_name, string _sr)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_copy(session.uuid, (_vm != null) ? _vm : "", (_new_name != null) ? _new_name : "", (_sr != null) ? _sr : "").parse());
        }

        public static void revert(Session session, string _snapshot)
        {
            session.proxy.vm_revert(session.uuid, (_snapshot != null) ? _snapshot : "").parse();
        }

        public static XenRef<Task> async_revert(Session session, string _snapshot)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_revert(session.uuid, (_snapshot != null) ? _snapshot : "").parse());
        }

        public static XenRef<VM> checkpoint(Session session, string _vm, string _new_name)
        {
            return XenRef<VM>.Create(session.proxy.vm_checkpoint(session.uuid, (_vm != null) ? _vm : "", (_new_name != null) ? _new_name : "").parse());
        }

        public static XenRef<Task> async_checkpoint(Session session, string _vm, string _new_name)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_checkpoint(session.uuid, (_vm != null) ? _vm : "", (_new_name != null) ? _new_name : "").parse());
        }

        public static void provision(Session session, string _vm)
        {
            session.proxy.vm_provision(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<Task> async_provision(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_provision(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void start(Session session, string _vm, bool _start_paused, bool _force)
        {
            session.proxy.vm_start(session.uuid, (_vm != null) ? _vm : "", _start_paused, _force).parse();
        }

        public static XenRef<Task> async_start(Session session, string _vm, bool _start_paused, bool _force)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_start(session.uuid, (_vm != null) ? _vm : "", _start_paused, _force).parse());
        }

        public static void start_on(Session session, string _vm, string _host, bool _start_paused, bool _force)
        {
            session.proxy.vm_start_on(session.uuid, (_vm != null) ? _vm : "", (_host != null) ? _host : "", _start_paused, _force).parse();
        }

        public static XenRef<Task> async_start_on(Session session, string _vm, string _host, bool _start_paused, bool _force)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_start_on(session.uuid, (_vm != null) ? _vm : "", (_host != null) ? _host : "", _start_paused, _force).parse());
        }

        public static void pause(Session session, string _vm)
        {
            session.proxy.vm_pause(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<Task> async_pause(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_pause(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void unpause(Session session, string _vm)
        {
            session.proxy.vm_unpause(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<Task> async_unpause(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_unpause(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void clean_shutdown(Session session, string _vm)
        {
            session.proxy.vm_clean_shutdown(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<Task> async_clean_shutdown(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_clean_shutdown(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void shutdown(Session session, string _vm)
        {
            session.proxy.vm_shutdown(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<Task> async_shutdown(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_shutdown(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void clean_reboot(Session session, string _vm)
        {
            session.proxy.vm_clean_reboot(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<Task> async_clean_reboot(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_clean_reboot(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void hard_shutdown(Session session, string _vm)
        {
            session.proxy.vm_hard_shutdown(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<Task> async_hard_shutdown(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_hard_shutdown(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void power_state_reset(Session session, string _vm)
        {
            session.proxy.vm_power_state_reset(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<Task> async_power_state_reset(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_power_state_reset(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void hard_reboot(Session session, string _vm)
        {
            session.proxy.vm_hard_reboot(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<Task> async_hard_reboot(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_hard_reboot(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void suspend(Session session, string _vm)
        {
            session.proxy.vm_suspend(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static XenRef<Task> async_suspend(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_suspend(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void resume(Session session, string _vm, bool _start_paused, bool _force)
        {
            session.proxy.vm_resume(session.uuid, (_vm != null) ? _vm : "", _start_paused, _force).parse();
        }

        public static XenRef<Task> async_resume(Session session, string _vm, bool _start_paused, bool _force)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_resume(session.uuid, (_vm != null) ? _vm : "", _start_paused, _force).parse());
        }

        public static void resume_on(Session session, string _vm, string _host, bool _start_paused, bool _force)
        {
            session.proxy.vm_resume_on(session.uuid, (_vm != null) ? _vm : "", (_host != null) ? _host : "", _start_paused, _force).parse();
        }

        public static XenRef<Task> async_resume_on(Session session, string _vm, string _host, bool _start_paused, bool _force)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_resume_on(session.uuid, (_vm != null) ? _vm : "", (_host != null) ? _host : "", _start_paused, _force).parse());
        }

        public static void pool_migrate(Session session, string _vm, string _host, Dictionary<string, string> _options)
        {
            session.proxy.vm_pool_migrate(session.uuid, (_vm != null) ? _vm : "", (_host != null) ? _host : "", Maps.convert_to_proxy_string_string(_options)).parse();
        }

        public static XenRef<Task> async_pool_migrate(Session session, string _vm, string _host, Dictionary<string, string> _options)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_pool_migrate(session.uuid, (_vm != null) ? _vm : "", (_host != null) ? _host : "", Maps.convert_to_proxy_string_string(_options)).parse());
        }

        public static void set_VCPUs_number_live(Session session, string _self, long _nvcpu)
        {
            session.proxy.vm_set_vcpus_number_live(session.uuid, (_self != null) ? _self : "", _nvcpu.ToString()).parse();
        }

        public static XenRef<Task> async_set_VCPUs_number_live(Session session, string _self, long _nvcpu)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_set_vcpus_number_live(session.uuid, (_self != null) ? _self : "", _nvcpu.ToString()).parse());
        }

        public static void add_to_VCPUs_params_live(Session session, string _self, string _key, string _value)
        {
            session.proxy.vm_add_to_vcpus_params_live(session.uuid, (_self != null) ? _self : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse();
        }

        public static XenRef<Task> async_add_to_VCPUs_params_live(Session session, string _self, string _key, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_add_to_vcpus_params_live(session.uuid, (_self != null) ? _self : "", (_key != null) ? _key : "", (_value != null) ? _value : "").parse());
        }

        public static void set_ha_restart_priority(Session session, string _self, string _value)
        {
            session.proxy.vm_set_ha_restart_priority(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse();
        }

        public static void set_ha_always_run(Session session, string _self, bool _value)
        {
            session.proxy.vm_set_ha_always_run(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static long compute_memory_overhead(Session session, string _vm)
        {
            return long.Parse((string)session.proxy.vm_compute_memory_overhead(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static XenRef<Task> async_compute_memory_overhead(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_compute_memory_overhead(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void set_memory_dynamic_max(Session session, string _self, long _value)
        {
            session.proxy.vm_set_memory_dynamic_max(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void set_memory_dynamic_min(Session session, string _self, long _value)
        {
            session.proxy.vm_set_memory_dynamic_min(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void set_memory_dynamic_range(Session session, string _self, long _min, long _max)
        {
            session.proxy.vm_set_memory_dynamic_range(session.uuid, (_self != null) ? _self : "", _min.ToString(), _max.ToString()).parse();
        }

        public static XenRef<Task> async_set_memory_dynamic_range(Session session, string _self, long _min, long _max)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_set_memory_dynamic_range(session.uuid, (_self != null) ? _self : "", _min.ToString(), _max.ToString()).parse());
        }

        public static void set_memory_static_max(Session session, string _self, long _value)
        {
            session.proxy.vm_set_memory_static_max(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void set_memory_static_min(Session session, string _self, long _value)
        {
            session.proxy.vm_set_memory_static_min(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void set_memory_static_range(Session session, string _self, long _min, long _max)
        {
            session.proxy.vm_set_memory_static_range(session.uuid, (_self != null) ? _self : "", _min.ToString(), _max.ToString()).parse();
        }

        public static XenRef<Task> async_set_memory_static_range(Session session, string _self, long _min, long _max)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_set_memory_static_range(session.uuid, (_self != null) ? _self : "", _min.ToString(), _max.ToString()).parse());
        }

        public static void set_memory_limits(Session session, string _self, long _static_min, long _static_max, long _dynamic_min, long _dynamic_max)
        {
            session.proxy.vm_set_memory_limits(session.uuid, (_self != null) ? _self : "", _static_min.ToString(), _static_max.ToString(), _dynamic_min.ToString(), _dynamic_max.ToString()).parse();
        }

        public static XenRef<Task> async_set_memory_limits(Session session, string _self, long _static_min, long _static_max, long _dynamic_min, long _dynamic_max)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_set_memory_limits(session.uuid, (_self != null) ? _self : "", _static_min.ToString(), _static_max.ToString(), _dynamic_min.ToString(), _dynamic_max.ToString()).parse());
        }

        public static void set_memory_target_live(Session session, string _self, long _target)
        {
            session.proxy.vm_set_memory_target_live(session.uuid, (_self != null) ? _self : "", _target.ToString()).parse();
        }

        public static XenRef<Task> async_set_memory_target_live(Session session, string _self, long _target)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_set_memory_target_live(session.uuid, (_self != null) ? _self : "", _target.ToString()).parse());
        }

        public static void wait_memory_target_live(Session session, string _self)
        {
            session.proxy.vm_wait_memory_target_live(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_wait_memory_target_live(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_wait_memory_target_live(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static bool get_cooperative(Session session, string _self)
        {
            return (bool)session.proxy.vm_get_cooperative(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_get_cooperative(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_get_cooperative(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void set_HVM_shadow_multiplier(Session session, string _self, double _value)
        {
            session.proxy.vm_set_hvm_shadow_multiplier(session.uuid, (_self != null) ? _self : "", _value).parse();
        }

        public static void set_shadow_multiplier_live(Session session, string _self, double _multiplier)
        {
            session.proxy.vm_set_shadow_multiplier_live(session.uuid, (_self != null) ? _self : "", _multiplier).parse();
        }

        public static XenRef<Task> async_set_shadow_multiplier_live(Session session, string _self, double _multiplier)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_set_shadow_multiplier_live(session.uuid, (_self != null) ? _self : "", _multiplier).parse());
        }

        public static void set_VCPUs_max(Session session, string _self, long _value)
        {
            session.proxy.vm_set_vcpus_max(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void set_VCPUs_at_startup(Session session, string _self, long _value)
        {
            session.proxy.vm_set_vcpus_at_startup(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static void send_sysrq(Session session, string _vm, string _key)
        {
            session.proxy.vm_send_sysrq(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "").parse();
        }

        public static XenRef<Task> async_send_sysrq(Session session, string _vm, string _key)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_send_sysrq(session.uuid, (_vm != null) ? _vm : "", (_key != null) ? _key : "").parse());
        }

        public static void send_trigger(Session session, string _vm, string _trigger)
        {
            session.proxy.vm_send_trigger(session.uuid, (_vm != null) ? _vm : "", (_trigger != null) ? _trigger : "").parse();
        }

        public static XenRef<Task> async_send_trigger(Session session, string _vm, string _trigger)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_send_trigger(session.uuid, (_vm != null) ? _vm : "", (_trigger != null) ? _trigger : "").parse());
        }

        public static long maximise_memory(Session session, string _self, long _total, bool _approximate)
        {
            return long.Parse((string)session.proxy.vm_maximise_memory(session.uuid, (_self != null) ? _self : "", _total.ToString(), _approximate).parse());
        }

        public static XenRef<Task> async_maximise_memory(Session session, string _self, long _total, bool _approximate)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_maximise_memory(session.uuid, (_self != null) ? _self : "", _total.ToString(), _approximate).parse());
        }

        public static void migrate_send(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
            session.proxy.vm_migrate_send(session.uuid, (_vm != null) ? _vm : "", Maps.convert_to_proxy_string_string(_dest), _live, Maps.convert_to_proxy_XenRefVDI_XenRefSR(_vdi_map), Maps.convert_to_proxy_XenRefVIF_XenRefNetwork(_vif_map), Maps.convert_to_proxy_string_string(_options)).parse();
        }

        public static XenRef<Task> async_migrate_send(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_migrate_send(session.uuid, (_vm != null) ? _vm : "", Maps.convert_to_proxy_string_string(_dest), _live, Maps.convert_to_proxy_XenRefVDI_XenRefSR(_vdi_map), Maps.convert_to_proxy_XenRefVIF_XenRefNetwork(_vif_map), Maps.convert_to_proxy_string_string(_options)).parse());
        }

        public static void assert_can_migrate(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
            session.proxy.vm_assert_can_migrate(session.uuid, (_vm != null) ? _vm : "", Maps.convert_to_proxy_string_string(_dest), _live, Maps.convert_to_proxy_XenRefVDI_XenRefSR(_vdi_map), Maps.convert_to_proxy_XenRefVIF_XenRefNetwork(_vif_map), Maps.convert_to_proxy_string_string(_options)).parse();
        }

        public static XenRef<Task> async_assert_can_migrate(Session session, string _vm, Dictionary<string, string> _dest, bool _live, Dictionary<XenRef<VDI>, XenRef<SR>> _vdi_map, Dictionary<XenRef<VIF>, XenRef<Network>> _vif_map, Dictionary<string, string> _options)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_assert_can_migrate(session.uuid, (_vm != null) ? _vm : "", Maps.convert_to_proxy_string_string(_dest), _live, Maps.convert_to_proxy_XenRefVDI_XenRefSR(_vdi_map), Maps.convert_to_proxy_XenRefVIF_XenRefNetwork(_vif_map), Maps.convert_to_proxy_string_string(_options)).parse());
        }

        public static VM get_boot_record(Session session, string _self)
        {
            return new VM((Proxy_VM)session.proxy.vm_get_boot_record(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static List<Data_source> get_data_sources(Session session, string _self)
        {
            return Helper.Proxy_Data_sourceArrayToData_sourceList(session.proxy.vm_get_data_sources(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static void record_data_source(Session session, string _self, string _data_source)
        {
            session.proxy.vm_record_data_source(session.uuid, (_self != null) ? _self : "", (_data_source != null) ? _data_source : "").parse();
        }

        public static double query_data_source(Session session, string _self, string _data_source)
        {
            return Convert.ToDouble(session.proxy.vm_query_data_source(session.uuid, (_self != null) ? _self : "", (_data_source != null) ? _data_source : "").parse());
        }

        public static void forget_data_source_archives(Session session, string _self, string _data_source)
        {
            session.proxy.vm_forget_data_source_archives(session.uuid, (_self != null) ? _self : "", (_data_source != null) ? _data_source : "").parse();
        }

        public static void assert_operation_valid(Session session, string _self, vm_operations _op)
        {
            session.proxy.vm_assert_operation_valid(session.uuid, (_self != null) ? _self : "", vm_operations_helper.ToString(_op)).parse();
        }

        public static XenRef<Task> async_assert_operation_valid(Session session, string _self, vm_operations _op)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_assert_operation_valid(session.uuid, (_self != null) ? _self : "", vm_operations_helper.ToString(_op)).parse());
        }

        public static void update_allowed_operations(Session session, string _self)
        {
            session.proxy.vm_update_allowed_operations(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_update_allowed_operations(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_update_allowed_operations(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static string[] get_allowed_VBD_devices(Session session, string _vm)
        {
            return (string [])session.proxy.vm_get_allowed_vbd_devices(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static string[] get_allowed_VIF_devices(Session session, string _vm)
        {
            return (string [])session.proxy.vm_get_allowed_vif_devices(session.uuid, (_vm != null) ? _vm : "").parse();
        }

        public static List<XenRef<Host>> get_possible_hosts(Session session, string _vm)
        {
            return XenRef<Host>.Create(session.proxy.vm_get_possible_hosts(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static XenRef<Task> async_get_possible_hosts(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_get_possible_hosts(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void assert_can_boot_here(Session session, string _self, string _host)
        {
            session.proxy.vm_assert_can_boot_here(session.uuid, (_self != null) ? _self : "", (_host != null) ? _host : "").parse();
        }

        public static XenRef<Task> async_assert_can_boot_here(Session session, string _self, string _host)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_assert_can_boot_here(session.uuid, (_self != null) ? _self : "", (_host != null) ? _host : "").parse());
        }

        public static XenRef<Blob> create_new_blob(Session session, string _vm, string _name, string _mime_type, bool _public)
        {
            return XenRef<Blob>.Create(session.proxy.vm_create_new_blob(session.uuid, (_vm != null) ? _vm : "", (_name != null) ? _name : "", (_mime_type != null) ? _mime_type : "", _public).parse());
        }

        public static XenRef<Task> async_create_new_blob(Session session, string _vm, string _name, string _mime_type, bool _public)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_create_new_blob(session.uuid, (_vm != null) ? _vm : "", (_name != null) ? _name : "", (_mime_type != null) ? _mime_type : "", _public).parse());
        }

        public static void assert_agile(Session session, string _self)
        {
            session.proxy.vm_assert_agile(session.uuid, (_self != null) ? _self : "").parse();
        }

        public static XenRef<Task> async_assert_agile(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_assert_agile(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static Dictionary<XenRef<Host>, string[]> retrieve_wlb_recommendations(Session session, string _vm)
        {
            return Maps.convert_from_proxy_XenRefHost_string_array(session.proxy.vm_retrieve_wlb_recommendations(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static XenRef<Task> async_retrieve_wlb_recommendations(Session session, string _vm)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_retrieve_wlb_recommendations(session.uuid, (_vm != null) ? _vm : "").parse());
        }

        public static void copy_bios_strings(Session session, string _vm, string _host)
        {
            session.proxy.vm_copy_bios_strings(session.uuid, (_vm != null) ? _vm : "", (_host != null) ? _host : "").parse();
        }

        public static XenRef<Task> async_copy_bios_strings(Session session, string _vm, string _host)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_copy_bios_strings(session.uuid, (_vm != null) ? _vm : "", (_host != null) ? _host : "").parse());
        }

        public static void set_protection_policy(Session session, string _self, string _value)
        {
            session.proxy.vm_set_protection_policy(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse();
        }

        public static void set_start_delay(Session session, string _self, long _value)
        {
            session.proxy.vm_set_start_delay(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static XenRef<Task> async_set_start_delay(Session session, string _self, long _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_set_start_delay(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse());
        }

        public static void set_shutdown_delay(Session session, string _self, long _value)
        {
            session.proxy.vm_set_shutdown_delay(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static XenRef<Task> async_set_shutdown_delay(Session session, string _self, long _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_set_shutdown_delay(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse());
        }

        public static void set_order(Session session, string _self, long _value)
        {
            session.proxy.vm_set_order(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse();
        }

        public static XenRef<Task> async_set_order(Session session, string _self, long _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_set_order(session.uuid, (_self != null) ? _self : "", _value.ToString()).parse());
        }

        public static void set_suspend_VDI(Session session, string _self, string _value)
        {
            session.proxy.vm_set_suspend_vdi(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse();
        }

        public static XenRef<Task> async_set_suspend_VDI(Session session, string _self, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_set_suspend_vdi(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse());
        }

        public static void assert_can_be_recovered(Session session, string _self, string _session_to)
        {
            session.proxy.vm_assert_can_be_recovered(session.uuid, (_self != null) ? _self : "", (_session_to != null) ? _session_to : "").parse();
        }

        public static XenRef<Task> async_assert_can_be_recovered(Session session, string _self, string _session_to)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_assert_can_be_recovered(session.uuid, (_self != null) ? _self : "", (_session_to != null) ? _session_to : "").parse());
        }

        public static void recover(Session session, string _self, string _session_to, bool _force)
        {
            session.proxy.vm_recover(session.uuid, (_self != null) ? _self : "", (_session_to != null) ? _session_to : "", _force).parse();
        }

        public static XenRef<Task> async_recover(Session session, string _self, string _session_to, bool _force)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_recover(session.uuid, (_self != null) ? _self : "", (_session_to != null) ? _session_to : "", _force).parse());
        }

        public static void import_convert(Session session, string _type, string _username, string _password, string _sr, Dictionary<string, string> _remote_config)
        {
            session.proxy.vm_import_convert(session.uuid, (_type != null) ? _type : "", (_username != null) ? _username : "", (_password != null) ? _password : "", (_sr != null) ? _sr : "", Maps.convert_to_proxy_string_string(_remote_config)).parse();
        }

        public static XenRef<Task> async_import_convert(Session session, string _type, string _username, string _password, string _sr, Dictionary<string, string> _remote_config)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_import_convert(session.uuid, (_type != null) ? _type : "", (_username != null) ? _username : "", (_password != null) ? _password : "", (_sr != null) ? _sr : "", Maps.convert_to_proxy_string_string(_remote_config)).parse());
        }

        public static void set_appliance(Session session, string _self, string _value)
        {
            session.proxy.vm_set_appliance(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse();
        }

        public static XenRef<Task> async_set_appliance(Session session, string _self, string _value)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_set_appliance(session.uuid, (_self != null) ? _self : "", (_value != null) ? _value : "").parse());
        }

        public static Dictionary<string, string> query_services(Session session, string _self)
        {
            return Maps.convert_from_proxy_string_string(session.proxy.vm_query_services(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static XenRef<Task> async_query_services(Session session, string _self)
        {
            return XenRef<Task>.Create(session.proxy.async_vm_query_services(session.uuid, (_self != null) ? _self : "").parse());
        }

        public static List<XenRef<VM>> get_all(Session session)
        {
            return XenRef<VM>.Create(session.proxy.vm_get_all(session.uuid).parse());
        }

        public static Dictionary<XenRef<VM>, VM> get_all_records(Session session)
        {
            return XenRef<VM>.Create<Proxy_VM>(session.proxy.vm_get_all_records(session.uuid).parse());
        }

        private string _uuid;
        public virtual string uuid {
             get { return _uuid; }
             set { if (!Helper.AreEqual(value, _uuid)) { _uuid = value; Changed = true; NotifyPropertyChanged("uuid"); } }
         }

        private List<vm_operations> _allowed_operations;
        public virtual List<vm_operations> allowed_operations {
             get { return _allowed_operations; }
             set { if (!Helper.AreEqual(value, _allowed_operations)) { _allowed_operations = value; Changed = true; NotifyPropertyChanged("allowed_operations"); } }
         }

        private Dictionary<string, vm_operations> _current_operations;
        public virtual Dictionary<string, vm_operations> current_operations {
             get { return _current_operations; }
             set { if (!Helper.AreEqual(value, _current_operations)) { _current_operations = value; Changed = true; NotifyPropertyChanged("current_operations"); } }
         }

        private vm_power_state _power_state;
        public virtual vm_power_state power_state {
             get { return _power_state; }
             set { if (!Helper.AreEqual(value, _power_state)) { _power_state = value; Changed = true; NotifyPropertyChanged("power_state"); } }
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

        private long _user_version;
        public virtual long user_version {
             get { return _user_version; }
             set { if (!Helper.AreEqual(value, _user_version)) { _user_version = value; Changed = true; NotifyPropertyChanged("user_version"); } }
         }

        private bool _is_a_template;
        public virtual bool is_a_template {
             get { return _is_a_template; }
             set { if (!Helper.AreEqual(value, _is_a_template)) { _is_a_template = value; Changed = true; NotifyPropertyChanged("is_a_template"); } }
         }

        private XenRef<VDI> _suspend_VDI;
        public virtual XenRef<VDI> suspend_VDI {
             get { return _suspend_VDI; }
             set { if (!Helper.AreEqual(value, _suspend_VDI)) { _suspend_VDI = value; Changed = true; NotifyPropertyChanged("suspend_VDI"); } }
         }

        private XenRef<Host> _resident_on;
        public virtual XenRef<Host> resident_on {
             get { return _resident_on; }
             set { if (!Helper.AreEqual(value, _resident_on)) { _resident_on = value; Changed = true; NotifyPropertyChanged("resident_on"); } }
         }

        private XenRef<Host> _affinity;
        public virtual XenRef<Host> affinity {
             get { return _affinity; }
             set { if (!Helper.AreEqual(value, _affinity)) { _affinity = value; Changed = true; NotifyPropertyChanged("affinity"); } }
         }

        private long _memory_overhead;
        public virtual long memory_overhead {
             get { return _memory_overhead; }
             set { if (!Helper.AreEqual(value, _memory_overhead)) { _memory_overhead = value; Changed = true; NotifyPropertyChanged("memory_overhead"); } }
         }

        private long _memory_target;
        public virtual long memory_target {
             get { return _memory_target; }
             set { if (!Helper.AreEqual(value, _memory_target)) { _memory_target = value; Changed = true; NotifyPropertyChanged("memory_target"); } }
         }

        private long _memory_static_max;
        public virtual long memory_static_max {
             get { return _memory_static_max; }
             set { if (!Helper.AreEqual(value, _memory_static_max)) { _memory_static_max = value; Changed = true; NotifyPropertyChanged("memory_static_max"); } }
         }

        private long _memory_dynamic_max;
        public virtual long memory_dynamic_max {
             get { return _memory_dynamic_max; }
             set { if (!Helper.AreEqual(value, _memory_dynamic_max)) { _memory_dynamic_max = value; Changed = true; NotifyPropertyChanged("memory_dynamic_max"); } }
         }

        private long _memory_dynamic_min;
        public virtual long memory_dynamic_min {
             get { return _memory_dynamic_min; }
             set { if (!Helper.AreEqual(value, _memory_dynamic_min)) { _memory_dynamic_min = value; Changed = true; NotifyPropertyChanged("memory_dynamic_min"); } }
         }

        private long _memory_static_min;
        public virtual long memory_static_min {
             get { return _memory_static_min; }
             set { if (!Helper.AreEqual(value, _memory_static_min)) { _memory_static_min = value; Changed = true; NotifyPropertyChanged("memory_static_min"); } }
         }

        private Dictionary<string, string> _VCPUs_params;
        public virtual Dictionary<string, string> VCPUs_params {
             get { return _VCPUs_params; }
             set { if (!Helper.AreEqual(value, _VCPUs_params)) { _VCPUs_params = value; Changed = true; NotifyPropertyChanged("VCPUs_params"); } }
         }

        private long _VCPUs_max;
        public virtual long VCPUs_max {
             get { return _VCPUs_max; }
             set { if (!Helper.AreEqual(value, _VCPUs_max)) { _VCPUs_max = value; Changed = true; NotifyPropertyChanged("VCPUs_max"); } }
         }

        private long _VCPUs_at_startup;
        public virtual long VCPUs_at_startup {
             get { return _VCPUs_at_startup; }
             set { if (!Helper.AreEqual(value, _VCPUs_at_startup)) { _VCPUs_at_startup = value; Changed = true; NotifyPropertyChanged("VCPUs_at_startup"); } }
         }

        private on_normal_exit _actions_after_shutdown;
        public virtual on_normal_exit actions_after_shutdown {
             get { return _actions_after_shutdown; }
             set { if (!Helper.AreEqual(value, _actions_after_shutdown)) { _actions_after_shutdown = value; Changed = true; NotifyPropertyChanged("actions_after_shutdown"); } }
         }

        private on_normal_exit _actions_after_reboot;
        public virtual on_normal_exit actions_after_reboot {
             get { return _actions_after_reboot; }
             set { if (!Helper.AreEqual(value, _actions_after_reboot)) { _actions_after_reboot = value; Changed = true; NotifyPropertyChanged("actions_after_reboot"); } }
         }

        private on_crash_behaviour _actions_after_crash;
        public virtual on_crash_behaviour actions_after_crash {
             get { return _actions_after_crash; }
             set { if (!Helper.AreEqual(value, _actions_after_crash)) { _actions_after_crash = value; Changed = true; NotifyPropertyChanged("actions_after_crash"); } }
         }

        private List<XenRef<Console>> _consoles;
        public virtual List<XenRef<Console>> consoles {
             get { return _consoles; }
             set { if (!Helper.AreEqual(value, _consoles)) { _consoles = value; Changed = true; NotifyPropertyChanged("consoles"); } }
         }

        private List<XenRef<VIF>> _VIFs;
        public virtual List<XenRef<VIF>> VIFs {
             get { return _VIFs; }
             set { if (!Helper.AreEqual(value, _VIFs)) { _VIFs = value; Changed = true; NotifyPropertyChanged("VIFs"); } }
         }

        private List<XenRef<VBD>> _VBDs;
        public virtual List<XenRef<VBD>> VBDs {
             get { return _VBDs; }
             set { if (!Helper.AreEqual(value, _VBDs)) { _VBDs = value; Changed = true; NotifyPropertyChanged("VBDs"); } }
         }

        private List<XenRef<Crashdump>> _crash_dumps;
        public virtual List<XenRef<Crashdump>> crash_dumps {
             get { return _crash_dumps; }
             set { if (!Helper.AreEqual(value, _crash_dumps)) { _crash_dumps = value; Changed = true; NotifyPropertyChanged("crash_dumps"); } }
         }

        private List<XenRef<VTPM>> _VTPMs;
        public virtual List<XenRef<VTPM>> VTPMs {
             get { return _VTPMs; }
             set { if (!Helper.AreEqual(value, _VTPMs)) { _VTPMs = value; Changed = true; NotifyPropertyChanged("VTPMs"); } }
         }

        private string _PV_bootloader;
        public virtual string PV_bootloader {
             get { return _PV_bootloader; }
             set { if (!Helper.AreEqual(value, _PV_bootloader)) { _PV_bootloader = value; Changed = true; NotifyPropertyChanged("PV_bootloader"); } }
         }

        private string _PV_kernel;
        public virtual string PV_kernel {
             get { return _PV_kernel; }
             set { if (!Helper.AreEqual(value, _PV_kernel)) { _PV_kernel = value; Changed = true; NotifyPropertyChanged("PV_kernel"); } }
         }

        private string _PV_ramdisk;
        public virtual string PV_ramdisk {
             get { return _PV_ramdisk; }
             set { if (!Helper.AreEqual(value, _PV_ramdisk)) { _PV_ramdisk = value; Changed = true; NotifyPropertyChanged("PV_ramdisk"); } }
         }

        private string _PV_args;
        public virtual string PV_args {
             get { return _PV_args; }
             set { if (!Helper.AreEqual(value, _PV_args)) { _PV_args = value; Changed = true; NotifyPropertyChanged("PV_args"); } }
         }

        private string _PV_bootloader_args;
        public virtual string PV_bootloader_args {
             get { return _PV_bootloader_args; }
             set { if (!Helper.AreEqual(value, _PV_bootloader_args)) { _PV_bootloader_args = value; Changed = true; NotifyPropertyChanged("PV_bootloader_args"); } }
         }

        private string _PV_legacy_args;
        public virtual string PV_legacy_args {
             get { return _PV_legacy_args; }
             set { if (!Helper.AreEqual(value, _PV_legacy_args)) { _PV_legacy_args = value; Changed = true; NotifyPropertyChanged("PV_legacy_args"); } }
         }

        private string _HVM_boot_policy;
        public virtual string HVM_boot_policy {
             get { return _HVM_boot_policy; }
             set { if (!Helper.AreEqual(value, _HVM_boot_policy)) { _HVM_boot_policy = value; Changed = true; NotifyPropertyChanged("HVM_boot_policy"); } }
         }

        private Dictionary<string, string> _HVM_boot_params;
        public virtual Dictionary<string, string> HVM_boot_params {
             get { return _HVM_boot_params; }
             set { if (!Helper.AreEqual(value, _HVM_boot_params)) { _HVM_boot_params = value; Changed = true; NotifyPropertyChanged("HVM_boot_params"); } }
         }

        private double _HVM_shadow_multiplier;
        public virtual double HVM_shadow_multiplier {
             get { return _HVM_shadow_multiplier; }
             set { if (!Helper.AreEqual(value, _HVM_shadow_multiplier)) { _HVM_shadow_multiplier = value; Changed = true; NotifyPropertyChanged("HVM_shadow_multiplier"); } }
         }

        private Dictionary<string, string> _platform;
        public virtual Dictionary<string, string> platform {
             get { return _platform; }
             set { if (!Helper.AreEqual(value, _platform)) { _platform = value; Changed = true; NotifyPropertyChanged("platform"); } }
         }

        private string _PCI_bus;
        public virtual string PCI_bus {
             get { return _PCI_bus; }
             set { if (!Helper.AreEqual(value, _PCI_bus)) { _PCI_bus = value; Changed = true; NotifyPropertyChanged("PCI_bus"); } }
         }

        private Dictionary<string, string> _other_config;
        public virtual Dictionary<string, string> other_config {
             get { return _other_config; }
             set { if (!Helper.AreEqual(value, _other_config)) { _other_config = value; Changed = true; NotifyPropertyChanged("other_config"); } }
         }

        private long _domid;
        public virtual long domid {
             get { return _domid; }
             set { if (!Helper.AreEqual(value, _domid)) { _domid = value; Changed = true; NotifyPropertyChanged("domid"); } }
         }

        private string _domarch;
        public virtual string domarch {
             get { return _domarch; }
             set { if (!Helper.AreEqual(value, _domarch)) { _domarch = value; Changed = true; NotifyPropertyChanged("domarch"); } }
         }

        private Dictionary<string, string> _last_boot_CPU_flags;
        public virtual Dictionary<string, string> last_boot_CPU_flags {
             get { return _last_boot_CPU_flags; }
             set { if (!Helper.AreEqual(value, _last_boot_CPU_flags)) { _last_boot_CPU_flags = value; Changed = true; NotifyPropertyChanged("last_boot_CPU_flags"); } }
         }

        private bool _is_control_domain;
        public virtual bool is_control_domain {
             get { return _is_control_domain; }
             set { if (!Helper.AreEqual(value, _is_control_domain)) { _is_control_domain = value; Changed = true; NotifyPropertyChanged("is_control_domain"); } }
         }

        private XenRef<VM_metrics> _metrics;
        public virtual XenRef<VM_metrics> metrics {
             get { return _metrics; }
             set { if (!Helper.AreEqual(value, _metrics)) { _metrics = value; Changed = true; NotifyPropertyChanged("metrics"); } }
         }

        private XenRef<VM_guest_metrics> _guest_metrics;
        public virtual XenRef<VM_guest_metrics> guest_metrics {
             get { return _guest_metrics; }
             set { if (!Helper.AreEqual(value, _guest_metrics)) { _guest_metrics = value; Changed = true; NotifyPropertyChanged("guest_metrics"); } }
         }

        private string _last_booted_record;
        public virtual string last_booted_record {
             get { return _last_booted_record; }
             set { if (!Helper.AreEqual(value, _last_booted_record)) { _last_booted_record = value; Changed = true; NotifyPropertyChanged("last_booted_record"); } }
         }

        private string _recommendations;
        public virtual string recommendations {
             get { return _recommendations; }
             set { if (!Helper.AreEqual(value, _recommendations)) { _recommendations = value; Changed = true; NotifyPropertyChanged("recommendations"); } }
         }

        private Dictionary<string, string> _xenstore_data;
        public virtual Dictionary<string, string> xenstore_data {
             get { return _xenstore_data; }
             set { if (!Helper.AreEqual(value, _xenstore_data)) { _xenstore_data = value; Changed = true; NotifyPropertyChanged("xenstore_data"); } }
         }

        private bool _ha_always_run;
        public virtual bool ha_always_run {
             get { return _ha_always_run; }
             set { if (!Helper.AreEqual(value, _ha_always_run)) { _ha_always_run = value; Changed = true; NotifyPropertyChanged("ha_always_run"); } }
         }

        private string _ha_restart_priority;
        public virtual string ha_restart_priority {
             get { return _ha_restart_priority; }
             set { if (!Helper.AreEqual(value, _ha_restart_priority)) { _ha_restart_priority = value; Changed = true; NotifyPropertyChanged("ha_restart_priority"); } }
         }

        private bool _is_a_snapshot;
        public virtual bool is_a_snapshot {
             get { return _is_a_snapshot; }
             set { if (!Helper.AreEqual(value, _is_a_snapshot)) { _is_a_snapshot = value; Changed = true; NotifyPropertyChanged("is_a_snapshot"); } }
         }

        private XenRef<VM> _snapshot_of;
        public virtual XenRef<VM> snapshot_of {
             get { return _snapshot_of; }
             set { if (!Helper.AreEqual(value, _snapshot_of)) { _snapshot_of = value; Changed = true; NotifyPropertyChanged("snapshot_of"); } }
         }

        private List<XenRef<VM>> _snapshots;
        public virtual List<XenRef<VM>> snapshots {
             get { return _snapshots; }
             set { if (!Helper.AreEqual(value, _snapshots)) { _snapshots = value; Changed = true; NotifyPropertyChanged("snapshots"); } }
         }

        private DateTime _snapshot_time;
        public virtual DateTime snapshot_time {
             get { return _snapshot_time; }
             set { if (!Helper.AreEqual(value, _snapshot_time)) { _snapshot_time = value; Changed = true; NotifyPropertyChanged("snapshot_time"); } }
         }

        private string _transportable_snapshot_id;
        public virtual string transportable_snapshot_id {
             get { return _transportable_snapshot_id; }
             set { if (!Helper.AreEqual(value, _transportable_snapshot_id)) { _transportable_snapshot_id = value; Changed = true; NotifyPropertyChanged("transportable_snapshot_id"); } }
         }

        private Dictionary<string, XenRef<Blob>> _blobs;
        public virtual Dictionary<string, XenRef<Blob>> blobs {
             get { return _blobs; }
             set { if (!Helper.AreEqual(value, _blobs)) { _blobs = value; Changed = true; NotifyPropertyChanged("blobs"); } }
         }

        private string[] _tags;
        public virtual string[] tags {
             get { return _tags; }
             set { if (!Helper.AreEqual(value, _tags)) { _tags = value; Changed = true; NotifyPropertyChanged("tags"); } }
         }

        private Dictionary<vm_operations, string> _blocked_operations;
        public virtual Dictionary<vm_operations, string> blocked_operations {
             get { return _blocked_operations; }
             set { if (!Helper.AreEqual(value, _blocked_operations)) { _blocked_operations = value; Changed = true; NotifyPropertyChanged("blocked_operations"); } }
         }

        private Dictionary<string, string> _snapshot_info;
        public virtual Dictionary<string, string> snapshot_info {
             get { return _snapshot_info; }
             set { if (!Helper.AreEqual(value, _snapshot_info)) { _snapshot_info = value; Changed = true; NotifyPropertyChanged("snapshot_info"); } }
         }

        private string _snapshot_metadata;
        public virtual string snapshot_metadata {
             get { return _snapshot_metadata; }
             set { if (!Helper.AreEqual(value, _snapshot_metadata)) { _snapshot_metadata = value; Changed = true; NotifyPropertyChanged("snapshot_metadata"); } }
         }

        private XenRef<VM> _parent;
        public virtual XenRef<VM> parent {
             get { return _parent; }
             set { if (!Helper.AreEqual(value, _parent)) { _parent = value; Changed = true; NotifyPropertyChanged("parent"); } }
         }

        private List<XenRef<VM>> _children;
        public virtual List<XenRef<VM>> children {
             get { return _children; }
             set { if (!Helper.AreEqual(value, _children)) { _children = value; Changed = true; NotifyPropertyChanged("children"); } }
         }

        private Dictionary<string, string> _bios_strings;
        public virtual Dictionary<string, string> bios_strings {
             get { return _bios_strings; }
             set { if (!Helper.AreEqual(value, _bios_strings)) { _bios_strings = value; Changed = true; NotifyPropertyChanged("bios_strings"); } }
         }

        private XenRef<VMPP> _protection_policy;
        public virtual XenRef<VMPP> protection_policy {
             get { return _protection_policy; }
             set { if (!Helper.AreEqual(value, _protection_policy)) { _protection_policy = value; Changed = true; NotifyPropertyChanged("protection_policy"); } }
         }

        private bool _is_snapshot_from_vmpp;
        public virtual bool is_snapshot_from_vmpp {
             get { return _is_snapshot_from_vmpp; }
             set { if (!Helper.AreEqual(value, _is_snapshot_from_vmpp)) { _is_snapshot_from_vmpp = value; Changed = true; NotifyPropertyChanged("is_snapshot_from_vmpp"); } }
         }

        private XenRef<VM_appliance> _appliance;
        public virtual XenRef<VM_appliance> appliance {
             get { return _appliance; }
             set { if (!Helper.AreEqual(value, _appliance)) { _appliance = value; Changed = true; NotifyPropertyChanged("appliance"); } }
         }

        private long _start_delay;
        public virtual long start_delay {
             get { return _start_delay; }
             set { if (!Helper.AreEqual(value, _start_delay)) { _start_delay = value; Changed = true; NotifyPropertyChanged("start_delay"); } }
         }

        private long _shutdown_delay;
        public virtual long shutdown_delay {
             get { return _shutdown_delay; }
             set { if (!Helper.AreEqual(value, _shutdown_delay)) { _shutdown_delay = value; Changed = true; NotifyPropertyChanged("shutdown_delay"); } }
         }

        private long _order;
        public virtual long order {
             get { return _order; }
             set { if (!Helper.AreEqual(value, _order)) { _order = value; Changed = true; NotifyPropertyChanged("order"); } }
         }

        private List<XenRef<VGPU>> _VGPUs;
        public virtual List<XenRef<VGPU>> VGPUs {
             get { return _VGPUs; }
             set { if (!Helper.AreEqual(value, _VGPUs)) { _VGPUs = value; Changed = true; NotifyPropertyChanged("VGPUs"); } }
         }

        private List<XenRef<PCI>> _attached_PCIs;
        public virtual List<XenRef<PCI>> attached_PCIs {
             get { return _attached_PCIs; }
             set { if (!Helper.AreEqual(value, _attached_PCIs)) { _attached_PCIs = value; Changed = true; NotifyPropertyChanged("attached_PCIs"); } }
         }

        private XenRef<SR> _suspend_SR;
        public virtual XenRef<SR> suspend_SR {
             get { return _suspend_SR; }
             set { if (!Helper.AreEqual(value, _suspend_SR)) { _suspend_SR = value; Changed = true; NotifyPropertyChanged("suspend_SR"); } }
         }

        private long _version;
        public virtual long version {
             get { return _version; }
             set { if (!Helper.AreEqual(value, _version)) { _version = value; Changed = true; NotifyPropertyChanged("version"); } }
         }

        private string _generation_id;
        public virtual string generation_id {
             get { return _generation_id; }
             set { if (!Helper.AreEqual(value, _generation_id)) { _generation_id = value; Changed = true; NotifyPropertyChanged("generation_id"); } }
         }


    }
}
