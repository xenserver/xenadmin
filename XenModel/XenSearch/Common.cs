/* Copyright (c) Cloud Software Group, Inc. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using XenAdmin.CustomFields;
using XenAdmin.Model;
using XenAPI;
using XenAdmin.Core;
using System.Collections;
using XenAdmin.Network;
using XenCenterLib;


namespace XenAdmin.XenSearch
{
    //
    // We need to access delegate, i18n and icons via enums
    // to allow for serialisation
    //

    // The order of this enum determines the order of types in the
    // tree in Folder View (CA-28418). But on the search-for
    // menu, see SearchFor.PopulateSearchForComboButton().
    [Flags]
    public enum ObjectTypes
    {
        None = 0,
        Pool = 1 << 0,
        Server = 1 << 1,
        DisconnectedServer = 1 << 2,
        VM = 1 << 3,
        Snapshot = 1 << 4,
        UserTemplate = 1 << 5,
        DefaultTemplate = 1 << 6,
        RemoteSR = 1 << 7,
        LocalSR = 1 << 8,
        VDI = 1 << 9,
        Network = 1 << 10,
        Folder = 1 << 11,
        AllIncFolders = (1 << 12) - 1,
        AllExcFolders = AllIncFolders & ~ObjectTypes.Folder,
		Appliance = 1 << 13,
        DockerContainer = 1 << 14,
    }

    public enum PropertyNames
    {
        /// <summary>The type of the selected object, e.g. VM, Network</summary>
        type,
        /// <summary>The label of the selected object</summary>
        label,
        /// <summary>The UUID of the selected object, or the full pathname of a folder</summary>
        uuid,
        /// <summary>The description of the selected object</summary>
        description,
        /// <summary>Comma-separated list of the tags of the selected object</summary>
        tags,
        /// <summary>The host name</summary>
        host,
        /// <summary>The pool name</summary>
        pool,
        /// <summary>Comma-separated list of the names of the networks attached to the object</summary>
        networks,
        /// <summary>Comma-separated list of the names of the storage attached to the object</summary>
        storage,
        /// <summary>Comma-separated list of the types of the storage attached to the object</summary>
        disks,
        /// <summary>The host memory, in bytes</summary>
        memory,
        /// <summary>The name of the operating system that a VM is running</summary>
        os_name,
        /// <summary>The VM power state, e.g. Halted, Running</summary>
        power_state,
        /// <summary>The state of the pure virtualization drivers installed on a VM</summary>
        virtualisation_status,
        /// <summary>Date and time that the VM was started</summary>
        start_time,
        /// <summary>The HA restart priority of the VM</summary>
        ha_restart_priority,
        /// <summary>The size in bytes of the attached disks</summary>
        size,
        /// <summary>Comma-separated list of IP addresses associated with the selected object</summary>
        ip_address,
        /// <summary>Uptime of the object, in a form such as '2 days, 1 hour, 26 minutes'</summary>
        uptime,
        /// <summary>true if HA is enabled, false otherwise</summary>
        ha_enabled,
        /// <summary>true if at least one of the supporters has different XenServer version from the coordinator, false otherwise</summary>
        isNotFullyUpgraded,
		/// <summary>A logical set of VMs</summary>
		appliance,
        /// <summary>Applicable to storage, true if storage is shared, false otherwise</summary>
        shared,
        /// <summary>Comma-separated list of VM names</summary>
        vm,
        /// <summary>List of Docker host-VM names.</summary>
        dockervm,
        /// <summary>Whether a VM is using read caching</summary>
        read_caching_enabled,
        /// <summary>The immediate parent folder of the selected object</summary>
        folder,
        /// <summary>Comma-separated list of all the ancestor folders of the selected object</summary>
        folders,
        /// <summary>INTERNAL. Used for populating the query panel.</summary>
        memoryText,
        /// <summary>INTERNAL Used for populating the query panel.</summary>
        memoryValue,
        /// <summary>INTERNAL Used for populating the query panel.</summary>
        memoryRank,
        /// <summary>INTERNAL Used for populating the query panel.</summary>
        cpuText,
        /// <summary>INTERNAL Used for populating the query panel.</summary>
        cpuValue,
        /// <summary>INTERNAL Used for populating the query panel.</summary>
        diskText,
        /// <summary>INTERNAL Used for populating the query panel.</summary>
        networkText,
        /// <summary>INTERNAL Used for populating the query panel.</summary>
        haText,
        /// <summary>Comma-separated list of host names. Hidden property used for plug-ins.</summary>
        connection_hostname,
        /// <summary>Applicable to storage, the storage type. Hidden property used for plug-ins.</summary>
        sr_type,
        /// <summary>Applicable to pools, a String representation of the license. Hidden property used for plug-ins.</summary>
        license,
        /// <summary>Whether the object has any custom fields defined. Hidden property used for plug-ins.</summary>
        has_custom_fields,
        /// <summary>Whether the VM is in any vApp</summary>
		in_any_appliance,
        /// <summary>Windows Update capability</summary>
        vendor_device_state,
    }

    public enum ColumnNames
    {
        name,
        cpu,
        memory,
        disks,
        network,
        ha,
        ip,
        uptime,
    }

    public class PropertyAccessors
    {
        private static Dictionary<PropertyNames, Type> property_types = new Dictionary<PropertyNames, Type>();
        private static Dictionary<PropertyNames, Func<IXenObject, IComparable>> properties = new Dictionary<PropertyNames, Func<IXenObject, IComparable>>();

        public static readonly Dictionary<String, vm_power_state> VM_power_state_i18n = new Dictionary<string, vm_power_state>();
        public static readonly Dictionary<String, VM.VirtualisationStatus> VirtualisationStatus_i18n = new Dictionary<string, VM.VirtualisationStatus>();
        public static readonly Dictionary<String, ObjectTypes> ObjectTypes_i18n = new Dictionary<string, ObjectTypes>();
        public static readonly Dictionary<String, VM.HA_Restart_Priority> HARestartPriority_i18n = new Dictionary<string, VM.HA_Restart_Priority>();
        public static readonly Dictionary<String, SR.SRTypes> SRType_i18n = new Dictionary<string, SR.SRTypes>();

        public static readonly Dictionary<PropertyNames, String> PropertyNames_i18n = new Dictionary<PropertyNames, string>();
        public static readonly Dictionary<PropertyNames, String> PropertyNames_i18n_false = new Dictionary<PropertyNames, string>();

        public static readonly Dictionary<vm_power_state, Icons> VM_power_state_images = new Dictionary<vm_power_state, Icons>();
        public static readonly Dictionary<ObjectTypes, Icons> ObjectTypes_images = new Dictionary<ObjectTypes, Icons>();
        
        private static Dictionary<ColumnNames, PropertyNames> column_sort_by = new Dictionary<ColumnNames, PropertyNames>();

        static PropertyAccessors()
        {
            foreach (vm_power_state p in Enum.GetValues(typeof(vm_power_state)))
                VM_power_state_i18n[FriendlyNameManager.GetFriendlyName(string.Format("Label-VM.power_state-{0}", p.ToString()))] = p;
            foreach (SR.SRTypes type in Enum.GetValues(typeof(SR.SRTypes)))
                SRType_i18n[SR.GetFriendlyTypeName(type)] = type;

            VirtualisationStatus_i18n[Messages.VIRTUALIZATION_STATE_VM_NOT_OPTIMIZED] = VM.VirtualisationStatus.NOT_INSTALLED;
            VirtualisationStatus_i18n[Messages.OUT_OF_DATE] = VM.VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE;
            VirtualisationStatus_i18n[Messages.UNKNOWN] = VM.VirtualisationStatus.UNKNOWN;
            VirtualisationStatus_i18n[Messages.VIRTUALIZATION_STATE_VM_IO_OPTIMIZED_ONLY] = VM.VirtualisationStatus.IO_DRIVERS_INSTALLED;
            VirtualisationStatus_i18n[Messages.VIRTUALIZATION_STATE_VM_MANAGEMENT_AGENT_INSTALLED_ONLY] = VM.VirtualisationStatus.MANAGEMENT_INSTALLED;
            VirtualisationStatus_i18n[Messages.VIRTUALIZATION_STATE_VM_OPTIMIZED] = VM.VirtualisationStatus.IO_DRIVERS_INSTALLED | VM.VirtualisationStatus.MANAGEMENT_INSTALLED;

            ObjectTypes_i18n[Messages.VMS] = ObjectTypes.VM;
            ObjectTypes_i18n[string.Format(Messages.XENSERVER_TEMPLATES, BrandManager.ProductBrand)] = ObjectTypes.DefaultTemplate;
            ObjectTypes_i18n[Messages.CUSTOM_TEMPLATES] = ObjectTypes.UserTemplate;
            ObjectTypes_i18n[Messages.POOLS] = ObjectTypes.Pool;
            ObjectTypes_i18n[Messages.SERVERS] = ObjectTypes.Server;
            ObjectTypes_i18n[Messages.DISCONNECTED_SERVERS] = ObjectTypes.DisconnectedServer;
            ObjectTypes_i18n[Messages.LOCAL_SRS] = ObjectTypes.LocalSR;
            ObjectTypes_i18n[Messages.REMOTE_SRS] = ObjectTypes.RemoteSR;
            ObjectTypes_i18n[Messages.NETWORKS] = ObjectTypes.Network;
            ObjectTypes_i18n[Messages.SNAPSHOTS] = ObjectTypes.Snapshot;
            ObjectTypes_i18n[Messages.VIRTUAL_DISKS] = ObjectTypes.VDI;
            ObjectTypes_i18n[Messages.FOLDERS] = ObjectTypes.Folder;
        	ObjectTypes_i18n[Messages.VM_APPLIANCE] = ObjectTypes.Appliance;


            foreach (VM.HA_Restart_Priority p in VM.GetAvailableRestartPriorities(null)) //CA-57600 - From Boston onwards, the HA restart priorities list contains Restart instead of AlwaysRestartHighPriority and AlwaysRestart
            {
                HARestartPriority_i18n[Helpers.RestartPriorityI18n(p)] = p;
            }

            // This one is used for grouping and filtering i18n
            PropertyNames_i18n[PropertyNames.description] = Messages.DESCRIPTION;
            PropertyNames_i18n[PropertyNames.host] = Messages.SERVER;
            PropertyNames_i18n[PropertyNames.label] = Messages.NAME;
            PropertyNames_i18n[PropertyNames.uuid] = Messages.UUID;
            PropertyNames_i18n[PropertyNames.networks] = Messages.NETWORK;
            PropertyNames_i18n[PropertyNames.os_name] = Messages.OPERATING_SYSTEM;
            PropertyNames_i18n[PropertyNames.pool] = Messages.POOL;
            PropertyNames_i18n[PropertyNames.power_state] = Messages.POWER_STATE;
            PropertyNames_i18n[PropertyNames.start_time] = Messages.START_TIME;
            PropertyNames_i18n[PropertyNames.storage] = Messages.SR;
            PropertyNames_i18n[PropertyNames.disks] = Messages.VIRTUAL_DISK;
            PropertyNames_i18n[PropertyNames.type] = Messages.TYPE;
            PropertyNames_i18n[PropertyNames.virtualisation_status] = Messages.TOOLS_STATUS;
            PropertyNames_i18n[PropertyNames.ha_restart_priority] = Messages.HA_RESTART_PRIORITY;
			PropertyNames_i18n[PropertyNames.appliance] = Messages.VM_APPLIANCE;
            PropertyNames_i18n[PropertyNames.tags] = Messages.TAGS;
            PropertyNames_i18n[PropertyNames.shared] = Messages.SHARED;
            PropertyNames_i18n[PropertyNames.ha_enabled] = Messages.HA;
            PropertyNames_i18n[PropertyNames.isNotFullyUpgraded] = Messages.POOL_VERSIONS_LINK_TEXT_SHORT;
            PropertyNames_i18n[PropertyNames.ip_address] = Messages.ADDRESS;
            PropertyNames_i18n[PropertyNames.vm] = Messages.VM;
            PropertyNames_i18n[PropertyNames.dockervm] = "Docker VM";
            PropertyNames_i18n[PropertyNames.read_caching_enabled] = Messages.VM_READ_CACHING_ENABLED_SEARCH;
            PropertyNames_i18n_false[PropertyNames.read_caching_enabled] = Messages.VM_READ_CACHING_DISABLED_SEARCH;
            PropertyNames_i18n[PropertyNames.memory] = Messages.MEMORY;
            PropertyNames_i18n[PropertyNames.sr_type] = Messages.STORAGE_TYPE;
            PropertyNames_i18n[PropertyNames.folder] = Messages.PARENT_FOLDER;
            PropertyNames_i18n[PropertyNames.folders] = Messages.ANCESTOR_FOLDERS;
            PropertyNames_i18n[PropertyNames.has_custom_fields] = Messages.HAS_CUSTOM_FIELDS;
			PropertyNames_i18n[PropertyNames.in_any_appliance] = Messages.IN_ANY_APPLIANCE;
            PropertyNames_i18n[PropertyNames.vendor_device_state] = Messages.WINDOWS_UPDATE_CAPABLE;
            PropertyNames_i18n_false[PropertyNames.vendor_device_state] = Messages.WINDOWS_UPDATE_CAPABLE_NOT;

            VM_power_state_images[vm_power_state.Halted] = Icons.PowerStateHalted;
            VM_power_state_images[vm_power_state.Paused] = Icons.PowerStateSuspended;
            VM_power_state_images[vm_power_state.Running] = Icons.PowerStateRunning;
            VM_power_state_images[vm_power_state.Suspended] = Icons.PowerStateSuspended;
            VM_power_state_images[vm_power_state.unknown] = Icons.PowerStateUnknown;

            ObjectTypes_images[ObjectTypes.DefaultTemplate] = Icons.Template;
            ObjectTypes_images[ObjectTypes.UserTemplate] = Icons.TemplateUser;
            ObjectTypes_images[ObjectTypes.Pool] = Icons.Pool;
            ObjectTypes_images[ObjectTypes.Server] = Icons.Host;
            ObjectTypes_images[ObjectTypes.DisconnectedServer] = Icons.HostDisconnected;
            ObjectTypes_images[ObjectTypes.LocalSR] = Icons.Storage;
            ObjectTypes_images[ObjectTypes.RemoteSR] = Icons.Storage;
            ObjectTypes_images[ObjectTypes.LocalSR | ObjectTypes.RemoteSR] = Icons.Storage;
            ObjectTypes_images[ObjectTypes.VM] = Icons.VM;
            ObjectTypes_images[ObjectTypes.Network] = Icons.Network;
            ObjectTypes_images[ObjectTypes.Snapshot] = Icons.Snapshot;
            ObjectTypes_images[ObjectTypes.VDI] = Icons.VDI;
            ObjectTypes_images[ObjectTypes.Folder] = Icons.Folder;
            ObjectTypes_images[ObjectTypes.Appliance] = Icons.VmAppliance;

            property_types.Add(PropertyNames.pool, typeof(Pool));
            property_types.Add(PropertyNames.host, typeof(Host));
            property_types.Add(PropertyNames.os_name, typeof(string));
            property_types.Add(PropertyNames.power_state, typeof(vm_power_state));
            property_types.Add(PropertyNames.virtualisation_status, typeof(VM.VirtualisationStatus));
            property_types.Add(PropertyNames.type, typeof(ObjectTypes));
            property_types.Add(PropertyNames.networks, typeof(XenAPI.Network));
            property_types.Add(PropertyNames.storage, typeof(SR));
            property_types.Add(PropertyNames.ha_restart_priority, typeof(VM.HA_Restart_Priority));
            property_types.Add(PropertyNames.read_caching_enabled, typeof(bool));
			property_types.Add(PropertyNames.appliance, typeof(VM_appliance));
            property_types.Add(PropertyNames.tags, typeof(string));
            property_types.Add(PropertyNames.has_custom_fields, typeof(bool));
            property_types.Add(PropertyNames.ip_address, typeof(ComparableAddress));
            property_types.Add(PropertyNames.vm, typeof(VM));
            property_types.Add(PropertyNames.sr_type, typeof(SR.SRTypes));
            property_types.Add(PropertyNames.folder, typeof(Folder));
            property_types.Add(PropertyNames.folders, typeof(Folder));
			property_types.Add(PropertyNames.in_any_appliance, typeof(bool));
            property_types.Add(PropertyNames.disks, typeof(VDI));

            properties[PropertyNames.os_name] = o => o is VM vm && vm.IsRealVm() ? vm.GetOSName() : null;
            properties[PropertyNames.power_state] = o => o is VM vm && vm.IsRealVm() ? (IComparable)vm.power_state : null;
            properties[PropertyNames.vendor_device_state] = o => o is VM vm && vm.IsRealVm() ? (bool?)vm.WindowsUpdateCapable() : null;
            properties[PropertyNames.virtualisation_status] = o => o is VM vm && vm.IsRealVm() ? (IComparable)vm.GetVirtualisationStatus(out _) : null;
            properties[PropertyNames.start_time] = o => o is VM vm && vm.IsRealVm() ? (DateTime?)vm.GetStartTime() : null;
            properties[PropertyNames.read_caching_enabled] = o => o is VM vm && vm.IsRealVm() ? (bool?)vm.ReadCachingEnabled() : null;

            properties[PropertyNames.label] = Helpers.GetName;
            properties[PropertyNames.pool] = o => o == null ? null : Helpers.GetPool(o.Connection);
            properties[PropertyNames.host] = HostProperty;
            properties[PropertyNames.vm] = VMProperty;
            properties[PropertyNames.dockervm] = o => o is DockerContainer dc ? new ComparableList<VM> {dc.Parent} : new ComparableList<VM>();
            properties[PropertyNames.networks] = NetworksProperty;
            properties[PropertyNames.storage] = StorageProperty;
            properties[PropertyNames.disks] = DisksProperty;

            properties[PropertyNames.has_custom_fields] = delegate(IXenObject o)
            {
                // this needs to be tidied up so that CustomFields calls don't require the event thread.

                bool ret = false;
                InvokeHelper.Invoke(delegate { ret = CustomFieldsManager.HasCustomFields(o); });
                return ret;
            };

            properties[PropertyNames.memory] = o =>
            {
                if (o is VM vm && vm.IsRealVm() && vm.Connection != null)
                {
                    var metrics = vm.Connection.Resolve(vm.metrics);
                    if (metrics != null)
                        return metrics.memory_actual;
                }
                return null;
            };

            properties[PropertyNames.ha_restart_priority] = delegate(IXenObject o)
            {
                if (o is VM vm && vm.IsRealVm())
                {
                    Pool pool = Helpers.GetPool(vm.Connection);
                    if (pool != null && pool.ha_enabled)
                        return vm.HaPriorityIsRestart() ? VM.HA_Restart_Priority.Restart : vm.HARestartPriority();

                    // CA-57600 - From Boston onwards, the HA_restart_priority enum contains Restart instead of
                    // AlwaysRestartHighPriority and AlwaysRestart. When searching in a pre-Boston pool for VMs
                    // with HA_restart_priority.Restart, the search will return VMs with HA_restart_priority
                    // AlwaysRestartHighPriority or AlwaysRestart
                }
                return null;
            };

            properties[PropertyNames.appliance] = delegate(IXenObject o)
            {
                if (o is VM_appliance app)
                    return app;

                if (o is VM vm && vm.IsRealVm() && vm.Connection != null)
                    return vm.Connection.Resolve(vm.appliance);

                return null;
            };

			properties[PropertyNames.in_any_appliance] = delegate(IXenObject o)
			{
				if (o is VM_appliance)
					return true;
                if (o is VM vm && vm.IsRealVm() && vm.Connection != null)
                    return vm.Connection.Resolve(vm.appliance) != null;
                return null;
            };

            properties[PropertyNames.connection_hostname] = ConnectionHostnameProperty;
            properties[PropertyNames.cpuText] = CPUTextProperty;
            properties[PropertyNames.cpuValue] = CPUValueProperty;
            properties[PropertyNames.description] = DescriptionProperty;
            properties[PropertyNames.diskText] = DiskTextProperty;
            properties[PropertyNames.folder] = Folders.GetFolder;
            properties[PropertyNames.folders] = Folders.GetAncestorFolders;
            properties[PropertyNames.haText] = HATextProperty;
            properties[PropertyNames.ha_enabled] = o => o is Pool pool ? (IComparable)pool.ha_enabled : null;
            properties[PropertyNames.isNotFullyUpgraded] = o => o is Pool pool ? (IComparable)!pool.IsPoolFullyUpgraded() : null;
            properties[PropertyNames.ip_address] = IPAddressProperty;
            properties[PropertyNames.license] = LicenseProperty;
            properties[PropertyNames.memoryText] = MemoryTextProperty;
            properties[PropertyNames.memoryValue] = MemoryValueProperty;
            properties[PropertyNames.memoryRank] = MemoryRankProperty;
            properties[PropertyNames.networkText] = NetworkTextProperty;
            properties[PropertyNames.shared] = SharedProperty;
            properties[PropertyNames.size] = o => o is VDI vdi ? (IComparable)vdi.virtual_size : null;
            properties[PropertyNames.sr_type] = o => o is SR sr ? (IComparable)sr.GetSRType(false) : null;
            properties[PropertyNames.tags] = Tags.GetTagList;
            properties[PropertyNames.type] = TypeProperty;
            properties[PropertyNames.uptime] = UptimeProperty;
            properties[PropertyNames.uuid] = UUIDProperty;

            column_sort_by[ColumnNames.name] = PropertyNames.label;
            column_sort_by[ColumnNames.cpu] = PropertyNames.cpuValue;
            column_sort_by[ColumnNames.memory] = PropertyNames.memoryValue;
            column_sort_by[ColumnNames.disks] = PropertyNames.diskText;
            column_sort_by[ColumnNames.network] = PropertyNames.networkText;
            column_sort_by[ColumnNames.ha] = PropertyNames.haText;
            column_sort_by[ColumnNames.ip] = PropertyNames.ip_address;
            column_sort_by[ColumnNames.uptime] = PropertyNames.uptime;
        }

        private static IComparable DescriptionProperty(IXenObject o)
        {
            if (o is VM vm && vm.DescriptionType() == VM.VmDescriptionType.None)
            {
                //return string.Empty instead of null, or it prints hyphens and looks ugly
                return string.Empty;
            }

            return o.Description();
        }

        private static IComparable UptimeProperty(IXenObject o)
        {
            if (o is VM vm)
                return vm.IsRealVm() ? vm.RunningTime() : null;

            if (o is Host host)
                return host.Uptime();

            return null;
        }

        private static IComparable CPUTextProperty(IXenObject o)
        {
            if (o is VM vm)
                return vm.IsRealVm() && vm.power_state == vm_power_state.Running
                    ? PropertyAccessorHelper.vmCpuUsageString(vm)
                    : null;

            if (o is Host host)
                return host.Connection != null && host.Connection.IsConnected
                    ? PropertyAccessorHelper.hostCpuUsageString(host)
                    : null;

            return null;
        }

        private static IComparable CPUValueProperty(IXenObject o)
        {
            if (o is VM vm)
                return vm.IsRealVm() && vm.power_state == vm_power_state.Running
                    ? (IComparable)PropertyAccessorHelper.vmCpuUsageRank(vm)
                    : null;

            if (o is Host host)
                return host.Connection != null && host.Connection.IsConnected
                    ? (IComparable)PropertyAccessorHelper.hostCpuUsageRank(host)
                    : null;

            return null;
        }

        private static IComparable MemoryTextProperty(IXenObject o)
        {
            if (o is VM vm)
            {
                return vm.IsRealVm() &&
                       vm.power_state == vm_power_state.Running &&
                       vm.GetVirtualisationStatus(out _).HasFlag(VM.VirtualisationStatus.MANAGEMENT_INSTALLED)
                    ? PropertyAccessorHelper.vmMemoryUsageString(vm)
                    : null;
            }

            if (o is Host host)
                return host.Connection != null && host.Connection.IsConnected
                    ? PropertyAccessorHelper.hostMemoryUsageString(host)
                    : null;
            
            if (o is VDI vdi)
                return PropertyAccessorHelper.vdiMemoryUsageString(vdi);

            return null;
        }

        private static IComparable MemoryRankProperty(IXenObject o)
        {
            if (o is VM vm)
            {
                return vm.IsRealVm() &&
                       vm.power_state == vm_power_state.Running &&
                       vm.GetVirtualisationStatus(out _).HasFlag(VM.VirtualisationStatus.MANAGEMENT_INSTALLED)
                    ? (IComparable)PropertyAccessorHelper.vmMemoryUsageRank(vm)
                    : null;
            }

            if (o is Host host)
                return host.Connection != null && host.Connection.IsConnected
                    ? (IComparable)PropertyAccessorHelper.hostMemoryUsageRank(host)
                    : null;

            if (o is VDI vdi)
                return vdi.virtual_size == 0 ? 0 : (int)(vdi.physical_utilisation * 100 / vdi.virtual_size);

            return null;
        }

        private static IComparable MemoryValueProperty(IXenObject o)
        {
                if (o is VM vm)
                {
                    return vm.IsRealVm() &&
                           vm.power_state == vm_power_state.Running &&
                           vm.GetVirtualisationStatus(out _).HasFlag(VM.VirtualisationStatus.MANAGEMENT_INSTALLED)
                        ? (IComparable)PropertyAccessorHelper.vmMemoryUsageValue(vm)
                        : null;
                }

                if (o is Host host)
                    return host.Connection != null && host.Connection.IsConnected
                        ? (IComparable)PropertyAccessorHelper.hostMemoryUsageValue(host)
                        : null;

                if (o is VDI vdi)
                    return vdi.virtual_size == 0 ? 0.0 : vdi.physical_utilisation;

                return null;
        }

        private static IComparable NetworkTextProperty(IXenObject o)
        {
            if (o is VM vm)
            {
                return vm.IsRealVm() &&
                       vm.power_state == vm_power_state.Running &&
                       vm.GetVirtualisationStatus(out _).HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED)
                    ? PropertyAccessorHelper.vmNetworkUsageString(vm)
                    : null;
            }

            if (o is Host host)
                return host.Connection != null && host.Connection.IsConnected
                    ? PropertyAccessorHelper.hostNetworkUsageString(host)
                    : null;

            return null;
        }

        private static IComparable DiskTextProperty(IXenObject o)
        {
            return o is VM vm &&
                   vm.IsRealVm() &&
                   vm.power_state == vm_power_state.Running &&
                   vm.GetVirtualisationStatus(out _).HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED)
                ? PropertyAccessorHelper.vmDiskUsageString(vm)
                : null;
        }

        private static IComparable HATextProperty(IXenObject o)
        {
            if (o is VM vm)
                return PropertyAccessorHelper.GetVMHAStatus(vm);
            if (o is Pool pool)
                return PropertyAccessorHelper.GetPoolHAStatus(pool);
            if (o is SR sr)
                return PropertyAccessorHelper.GetSRHAStatus(sr);
            return null;
        }

        private static IComparable UUIDProperty(IXenObject o)
        {
            if (o == null)
                return null;

            // A folder has no UUID, so we return the pathname in that case
            if (o is Folder)
                return o.opaque_ref;

            return Helpers.GetUuid(o);
        }

        private static IComparable ConnectionHostnameProperty(IXenObject o)
        {
            IXenConnection xc = o?.Connection;
            if (xc != null && xc.IsConnected)
                return xc.Hostname;
            return null;
        }

        private static IComparable LicenseProperty(IXenObject o)
        {
            Pool pool = Helpers.GetPool(o.Connection);
            if (pool == null)
                return null;
            Host coordinator = Helpers.GetCoordinator(pool.Connection);
            if (coordinator == null)
                return null;

            return Helpers.GetFriendlyLicenseName(coordinator);
        }

        private static IComparable SharedProperty(IXenObject o)
        {
            if (o is SR sr)
                return sr.shared;

            if (o is VDI vdi && vdi.Connection != null)
            {
                int vms = 0;
                foreach (VBD vbd in vdi.Connection.ResolveAll(vdi.VBDs))
                {
                    VM vm = vbd.Connection.Resolve(vbd.VM);
                    if (vm != null)
                    {
                        if (++vms >= 2)
                            return true;
                    }
                }
                return false;
            }

            return null;
        }

        private static IComparable TypeProperty(IXenObject o)
        {
            if (o is VM vm)
            {
                if (vm.is_a_snapshot)
                    return ObjectTypes.Snapshot;

                if (vm.is_a_template)
                    return vm.DefaultTemplate() ? ObjectTypes.DefaultTemplate : ObjectTypes.UserTemplate;

                if (vm.is_control_domain)
                    return null;

                return ObjectTypes.VM;
            }

            if (o is VM_appliance)
                return ObjectTypes.Appliance;

            if (o is Host)
                return o.Connection != null && o.Connection.IsConnected ? ObjectTypes.Server : ObjectTypes.DisconnectedServer;

            if (o is Pool)
                return ObjectTypes.Pool;

            if (o is SR sr)
                return sr.IsLocalSR() ? ObjectTypes.LocalSR : ObjectTypes.RemoteSR;

            if (o is XenAPI.Network)
                return ObjectTypes.Network;

            if (o is VDI)
                return ObjectTypes.VDI;

            if (o is Folder)
                return ObjectTypes.Folder;

            if (o is DockerContainer)
                return ObjectTypes.DockerContainer;

            return null;
        }

        private static ComparableList<XenAPI.Network> NetworksProperty(IXenObject o)
        {
            var networks = new ComparableList<XenAPI.Network>();

            if (o is VM vm)
            {
                if (vm.IsRealVm() && vm.Connection != null)
                    foreach (VIF vif in vm.Connection.ResolveAll(vm.VIFs))
                    {
                        XenAPI.Network network = vm.Connection.Resolve(vif.network);
                        if (network == null)
                            continue;

                        networks.Add(network);
                    }
            }
            else if (o is XenAPI.Network network)
            {
                networks.Add(network);
            }

            return networks;
        }

        private static ComparableList<VM> VMProperty(IXenObject o)
        {
            var vms = new ComparableList<VM>();
            if (o.Connection == null)
                return vms;

            if (o is Pool)
            {
                vms.AddRange(o.Connection.Cache.VMs);
            }
            else if (o is Host host)
            {
                vms.AddRange(host.Connection.ResolveAll(host.resident_VMs));
            }
            else if (o is SR sr)
            {
                foreach (VDI vdi in sr.Connection.ResolveAll(sr.VDIs))
                    foreach (VBD vbd in vdi.Connection.ResolveAll(vdi.VBDs))
                    {
                        VM vm = vbd.Connection.Resolve(vbd.VM);
                        if (vm == null)
                            continue;

                        vms.Add(vm);
                    }
            }
            else if (o is XenAPI.Network network)
            {
                foreach (VIF vif in network.Connection.ResolveAll(network.VIFs))
                {
                    VM vm = vif.Connection.Resolve(vif.VM);
                    if (vm == null)
                        continue;

                    vms.Add(vm);
                }
            }
            else if (o is VDI vdi)
            {
                foreach (VBD vbd in vdi.Connection.ResolveAll(vdi.VBDs))
                {
                    VM vm = vbd.Connection.Resolve(vbd.VM);
                    if (vm == null)
                        continue;

                    vms.Add(vm);
                }
            }
            else if (o is VM vm)
            {
                if (vm.is_a_snapshot)
                {
                    VM from = vm.Connection.Resolve(vm.snapshot_of);
                    if (from != null)  // Can be null if VM has been deleted: CA-29249
                        vms.Add(from);
                }
                else
                    vms.Add(vm);
            }
            else if (o is DockerContainer container)
            {
                vms.Add(container.Parent);
            }

            vms.RemoveAll(vm => !vm.IsRealVm());
            return vms;
        }

        private static ComparableList<Host> HostProperty(IXenObject o)
        {
            var hosts = new ComparableList<Host>();
            if (o.Connection == null)
                return hosts;

            // If we're not in a pool then just group everything under the same host 
            Pool pool = Helpers.GetPool(o.Connection);

            if (pool == null)
            {
                hosts.AddRange(o.Connection.Cache.Hosts);
            }
            else if (o is VM vm)
            {
                Host host = vm.Home();
                if (host != null)
                    hosts.Add(host);
            }
            else if (o is SR sr)
            {
                Host host = sr.Home();
                if (host != null)
                    hosts.Add(host);
            }
            else if (o is XenAPI.Network network)
            {
                if (network.PIFs.Count == 0)
                    hosts.AddRange(network.Connection.Cache.Hosts);
            }
            else if (o is Host host)
            {
                hosts.Add(host);
            }
            else if (o is VDI vdi)
            {
                SR theSr = vdi.Connection.Resolve(vdi.SR);
                if (theSr != null)
                    hosts.Add(theSr.Home());
            }
            else if (o is DockerContainer container)
            {
                VM parent = container.Parent;
                Host homeHost = parent.Home();
                if (homeHost != null)
                    hosts.Add(homeHost);
            }

            return hosts;
        }

        private static ComparableList<SR> StorageProperty(IXenObject o)
        {
            var srs = new ComparableList<SR>();
            if (o.Connection == null)
                return srs;

            if (o is VM vm)
            {
                if (vm.IsRealVm())
                    foreach (VBD vbd in vm.Connection.ResolveAll(vm.VBDs))
                    {
                        VDI vdi = vbd.Connection.Resolve(vbd.VDI);
                        if (vdi == null)
                            continue;

                        SR sr = vdi.Connection.Resolve(vdi.SR);
                        if (sr != null && !srs.Contains(sr))
                                srs.Add(sr);
                    }
            }
            else if (o is SR sr)
            {
                srs.Add(sr);
            }
            else if (o is VDI vdi)
            {
                SR theSr = vdi.Connection.Resolve(vdi.SR);
                if (theSr == null)
                    return null;

                srs.Add(theSr);
            }

            return srs;
        }

        private static ComparableList<VDI> DisksProperty(IXenObject o)
        {
            var vdis = new ComparableList<VDI>();
            if (o.Connection == null)
                return vdis;

            if (o is VDI theVdi)
            {
                vdis.Add(theVdi);
            }
            else if (o is VM vm)
            {
                if (vm.IsRealVm())
                    foreach (VBD vbd in vm.Connection.ResolveAll(vm.VBDs))
                    {
                        VDI vdi = vbd.Connection.Resolve(vbd.VDI);
                        if (vdi != null && !vdis.Contains(vdi))
                            vdis.Add(vdi);
                    }
            }

            return vdis;
        }

        private static ComparableList<ComparableAddress> IPAddressProperty(IXenObject o)
        {
            var addresses = new ComparableList<ComparableAddress>();
            if (o.Connection == null)
                return addresses;

            if (o is VM vm)
            {
                if (!vm.IsRealVm())
                    return addresses;

                VM_guest_metrics metrics = vm.Connection.Resolve(vm.guest_metrics);
                if (metrics == null)
                    return null;

                List<VIF> vifs = vm.Connection.ResolveAll(vm.VIFs);

                foreach (VIF vif in vifs)
                {
                    foreach (var value in Helpers.FindIpAddresses(metrics.networks, vif.device))
                    {
                        if (ComparableAddress.TryParse(value, false, true, out var ipAddress))
                            addresses.Add(ipAddress);
                    }
                }

                addresses = new ComparableList<ComparableAddress>(addresses.Distinct());
            }
            else if (o is Host host)
            {
                foreach (PIF pif in host.Connection.ResolveAll(host.PIFs))
                {
                    if (ComparableAddress.TryParse(pif.IP, false, true, out var ipAddress))
                        addresses.Add(ipAddress);
                }
            }
            else if (o is SR sr)
            {
                string target = sr.Target();
                if (!string.IsNullOrEmpty(target))
                {
                    if (ComparableAddress.TryParse(target, false, true, out var ipAddress))
                        addresses.Add(ipAddress);
                }
            }

            return addresses.Count == 0 ? null : addresses;   // CA-28300
        }

        public static Func<IXenObject, IComparable> Get(PropertyNames p)
        {
            return properties[p];
        }

        public static Type GetType(PropertyNames p)
        {
            return property_types[p];
        }

        public static IDictionary Geti18nFor(PropertyNames p)
        {
            switch (p)
            {
                case PropertyNames.type:
                    return ObjectTypes_i18n;

                case PropertyNames.virtualisation_status:
                    return VirtualisationStatus_i18n;

                case PropertyNames.power_state:
                    return VM_power_state_i18n;

                case PropertyNames.ha_restart_priority:
                    return HARestartPriority_i18n;

                case PropertyNames.sr_type:
                    return SRType_i18n;

                default:
                    return null;
            }
        }

        public static object GetImagesFor(PropertyNames p)
        {
            switch (p)
            {
                case PropertyNames.type:
                    return (Func<ObjectTypes, Icons>)(type => ObjectTypes_images[type]);

                case PropertyNames.power_state:
                    return (Func<vm_power_state, Icons>)(state => VM_power_state_images[state]);

                case PropertyNames.networks:
                    return (Func<XenAPI.Network, Icons>)(_ => Icons.Network); 

                case PropertyNames.appliance:
                    return (Func<VM_appliance, Icons>)(o => Icons.VmAppliance);

                case PropertyNames.os_name:
                    return (Func<string, Icons>)(osName =>
                    {
                        string os = osName.ToLowerInvariant();

                        if (os.Contains("debian"))
                            return Icons.Debian;
                        if (os.Contains("gooroom"))
                            return Icons.Gooroom;
                        if (os.Contains("rocky"))
                            return Icons.Rocky;
                        if (os.Contains("linx"))
                            return Icons.Linx;
                        if (os.Contains("red"))
                            return Icons.RHEL;
                        if (os.Contains("cent"))
                            return Icons.CentOS;
                        if (os.Contains("oracle"))
                            return Icons.Oracle;
                        if (os.Contains("suse"))
                            return Icons.SUSE;
                        if (os.Contains("ubuntu"))
                            return Icons.Ubuntu;
                        if (os.Contains("scientific"))
                            return Icons.SciLinux;
                        if (os.Contains("yinhe"))
                            return Icons.YinheKylin;
                        if (os.Contains("kylin"))
                            return Icons.NeoKylin;
                        if (os.Contains("asianux"))
                            return Icons.Asianux;
                        if (os.Contains("turbo"))
                            return Icons.Turbo;
                        if (os.Contains("windows"))
                            return Icons.Windows;
                        if (os.Contains("coreos"))
                            return Icons.CoreOS;

                        return Icons.XenCenter;
                    });

                case PropertyNames.virtualisation_status:
                    return (Func<VM.VirtualisationStatus, Icons>)(status =>
                    {
                        if (status.HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED | VM.VirtualisationStatus.MANAGEMENT_INSTALLED))
                            return Icons.ToolInstalled;

                        if (status.HasFlag(VM.VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE))
                            return Icons.ToolsOutOfDate;

                        return Icons.ToolsNotInstalled;
                    });

                case PropertyNames.sr_type:
                    return (Func<SR.SRTypes, Icons>)(_ => Icons.Storage);

                case PropertyNames.tags:
                    return (Func<string, Icons>)(_ => Icons.Tag);

                case PropertyNames.ha_restart_priority:
                    return (Func<VM.HA_Restart_Priority, Icons>)(_ => Icons.HA);

                case PropertyNames.read_caching_enabled:
                    return (Func<bool, Icons>)(_ => Icons.VDI);

                default:
                    return null;
            }
        }

        public static PropertyNames GetSortPropertyName(ColumnNames c)
        {
            return column_sort_by[c];
        }
    }

    public class PropertyWrapper
    {
        private readonly Func<IXenObject, IComparable> property;
        private readonly IXenObject o;

        public PropertyWrapper(PropertyNames property, IXenObject o)
        {
            this.property = PropertyAccessors.Get(property);
            this.o = o;
        }

        public override String ToString()
        {
            Object obj = property(o);

            return obj != null ? obj.ToString() : Messages.HYPHEN;
        }
    }
}
