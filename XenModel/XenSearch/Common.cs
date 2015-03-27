/* Copyright (c) Citrix Systems Inc. 
 * All rights reserved. 
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

//#define SHOW_CONTROL_DOMAINS

using System;
using System.Collections.Generic;
using System.Linq;
using XenAdmin.CustomFields;
using XenAdmin.Model;
using XenAPI;
using XenAdmin.Core;
using System.Collections;
using XenAdmin.Network;


namespace XenAdmin.XenSearch
{
    //
    // We need to access delegate, i18n and icons via enums
    // to allow for serialisation
    //

    public delegate Icons ImageDelegate<T>(T o);

    public delegate IComparable PropertyAccessor(IXenObject o);

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
        //StorageLinkServer = 1 << 11,
        //StorageLinkSystem = 1 << 12,
        //StorageLinkPool = 1 << 13,
        //StorageLinkVolume = 1 << 14,
        //StorageLinkRepository = 1 << 15,
        Folder = 1 << 11,
        AllIncFolders = (1 << 12) - 1,
        AllExcFolders = AllIncFolders & ~ObjectTypes.Folder,
		Appliance = 1 << 13,
        DockerContainer = 1 << 14,
    }

    public enum PropertyNames
    {
        [HelpString("The type of the selected object, e.g. VM, Network")]
        type,
        [HelpString("The label of the selected object")]
        label,
        [HelpString("The UUID of the selected object, or the full pathname of a folder")]
        uuid,
        [HelpString("The description of the selected object")]
        description,
        [HelpString("Comma-separated list of the tags of the selected object")]
        tags,
        [HelpString("The host name")]
        host,
        [HelpString("The pool name")]
        pool,
        [HelpString("Comma-separated list of the names of the networks attached to the object")]
        networks,
        [HelpString("Comma-separated list of the names of the storage attached to the object")]
        storage,
        [HelpString("Comma-separated list of the types of the storage attached to the object")]
        disks,
        [HelpString("The host memory, in bytes")]
        memory,
        [HelpString("The name of the operating system that a VM is running")]
        os_name,
        [HelpString("The VM power state, e.g. Halted, Running")]
        power_state,
        [HelpString("The state of the pure virtualization drivers installed on a VM")]
        virtualisation_status,
        [HelpString("Date and time that the VM was started")]
        start_time,
        [HelpString("The HA restart priority of the VM")]
        ha_restart_priority,
        [HelpString("The size in bytes of the attached disks")]
        size,
        [HelpString("Comma-separated list of IP addresses associated with the selected object")]
        ip_address,
        [HelpString("Uptime of the object, in a form such as '2 days, 1 hour, 26 minutes'")]
        uptime,
        [HelpString("true if HA is enabled, false otherwise")]
        ha_enabled,
        [HelpString("true if at least one of the slaves has different XenServer version from the master, false otherwise")]
        isNotFullyUpgraded,
		[HelpString("A logical set of VMs")]
		appliance,
        [HelpString("Applicable to storage, true if storage is shared, false otherwise")]
        shared,
        [HelpString("Comma-separated list of VM names")]
        vm,
        [HelpString("List of Docker host-VM names.")]
        dockervm,
        [HelpString("Whether a VM is using read caching")]
        read_caching_enabled,
        [HelpString("The immediate parent folder of the selected object")]
        folder,
        [HelpString("Comma-separated list of all the ancestor folders of the selected object")]
        folders,


        // These properties are used for populating the query panel
        [HelpString("INTERNAL")]
        memoryText,
        [HelpString("INTERNAL")]
        memoryValue,
        [HelpString("INTERNAL")]
        memoryRank,

        [HelpString("INTERNAL")]
        cpuText,
        [HelpString("INTERNAL")]
        cpuValue,

        [HelpString("INTERNAL")]
        diskText,
        [HelpString("INTERNAL")]
        networkText,
        [HelpString("INTERNAL")]
        haText,

        // These properties are for plugins and are hidden
        [HelpString("Comma-separated list of host names")]
        connection_hostname,
        [HelpString("Applicable to storage, the storage type")]
        sr_type,
        [HelpString("Applicable to pools, a String representation of the license")]
        license,
        [HelpString("Whether the object has any custom fields defined")]
        has_custom_fields,

        [HelpString("The StorageLink Server that this object belongs to")]
        storageLinkServer,

        [HelpString("The StorageLink Storage System that this object belongs to")]
        storageLinkSystem,

        [HelpString("The StorageLink Storage Pool that this object belongs to")]
        storageLinkPool,

        [HelpString("The StorageLink Storage Volume that this object belongs to")]
        storageLinkVolume,

        [HelpString("The StorageLink Storage Repository that this object belongs to")]
        storageLinkRepository,

		[HelpString("Whether the VM is in any vApp")]
		in_any_appliance
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
        private static Dictionary<PropertyNames, PropertyAccessor> properties = new Dictionary<PropertyNames, PropertyAccessor>();

        public static readonly Dictionary<String, vm_power_state> VM_power_state_i18n = new Dictionary<string, vm_power_state>();
        public static readonly Dictionary<String, VM.VirtualisationStatus> VirtualisationStatus_i18n = new Dictionary<string, VM.VirtualisationStatus>();
        public static readonly Dictionary<String, ObjectTypes> ObjectTypes_i18n = new Dictionary<string, ObjectTypes>();
        public static readonly Dictionary<String, VM.HA_Restart_Priority> HARestartPriority_i18n = new Dictionary<string, VM.HA_Restart_Priority>();
        public static readonly Dictionary<String, SR.SRTypes> SRType_i18n = new Dictionary<string, SR.SRTypes>();

        public static readonly Dictionary<PropertyNames, String> PropertyNames_i18n = new Dictionary<PropertyNames, string>();

        public static readonly Dictionary<vm_power_state, Icons> VM_power_state_images = new Dictionary<vm_power_state, Icons>();
        public static readonly Dictionary<ObjectTypes, Icons> ObjectTypes_images = new Dictionary<ObjectTypes, Icons>();
        
        private static Dictionary<ColumnNames, PropertyNames> column_sort_by = new Dictionary<ColumnNames, PropertyNames>();

        static PropertyAccessors()
        {
            foreach (vm_power_state p in Enum.GetValues(typeof(vm_power_state)))
                VM_power_state_i18n[Core.PropertyManager.GetFriendlyName(string.Format("Label-VM.power_state-{0}", p.ToString()))] = p;
            foreach (SR.SRTypes type in Enum.GetValues(typeof(SR.SRTypes)))
                SRType_i18n[SR.getFriendlyTypeName(type)] = type;

            VirtualisationStatus_i18n[Messages.OPTIMIZED] = VM.VirtualisationStatus.OPTIMIZED;
            VirtualisationStatus_i18n[Messages.PV_DRIVERS_NOT_INSTALLED] = VM.VirtualisationStatus.PV_DRIVERS_NOT_INSTALLED;
            VirtualisationStatus_i18n[Messages.OUT_OF_DATE] = VM.VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE;
            VirtualisationStatus_i18n[Messages.UNKNOWN] = VM.VirtualisationStatus.UNKNOWN;

            ObjectTypes_i18n[Messages.VMS] = ObjectTypes.VM;
            ObjectTypes_i18n[Messages.XENSERVER_TEMPLATES] = ObjectTypes.DefaultTemplate;
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
            //ObjectTypes_i18n[Messages.STORAGELINKSERVERS] = ObjectTypes.StorageLinkServer;
            //ObjectTypes_i18n[Messages.STORAGELINKSYSTEMS] = ObjectTypes.StorageLinkSystem;
            //ObjectTypes_i18n[Messages.STORAGELINKPOOLS] = ObjectTypes.StorageLinkPool;
            //ObjectTypes_i18n[Messages.STORAGELINKVOLUMES] = ObjectTypes.StorageLinkVolume;
            //ObjectTypes_i18n[Messages.STORAGELINKSRS] = ObjectTypes.StorageLinkRepository;


            foreach (VM.HA_Restart_Priority p in VM.GetAvailableRestartPriorities(null)) //CA-57600 - From Boston onwards, the HA restart priorities list contains Restart instead of AlwaysRestartHighPriority and AlwaysRestart
            {
                HARestartPriority_i18n[Helpers.RestartPriorityI18n(p)] = p;
            }

            // This one is used for grouping and filtering i18n
            PropertyNames_i18n[PropertyNames.description] = Messages.DESCRIPTION;
            PropertyNames_i18n[PropertyNames.host] = Messages.SERVER;
            PropertyNames_i18n[PropertyNames.label] = Messages.NAME;
            PropertyNames_i18n[PropertyNames.uuid] = Messages.UUID_SEARCH;
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
            PropertyNames_i18n[PropertyNames.memory] = Messages.MEMORY;
            PropertyNames_i18n[PropertyNames.sr_type] = Messages.STORAGE_TYPE;
            PropertyNames_i18n[PropertyNames.folder] = Messages.PARENT_FOLDER;
            PropertyNames_i18n[PropertyNames.folders] = Messages.ANCESTOR_FOLDERS;
            PropertyNames_i18n[PropertyNames.has_custom_fields] = Messages.HAS_CUSTOM_FIELDS;
            PropertyNames_i18n[PropertyNames.storageLinkServer] = Messages.STORAGELINKSERVER;
            PropertyNames_i18n[PropertyNames.storageLinkSystem] = Messages.STORAGELINKSYSTEM;
            PropertyNames_i18n[PropertyNames.storageLinkPool] = Messages.STORAGELINKPOOL;
            PropertyNames_i18n[PropertyNames.storageLinkVolume] = Messages.STORAGELINKVOLUME;
            PropertyNames_i18n[PropertyNames.storageLinkRepository] = Messages.SR;
			PropertyNames_i18n[PropertyNames.in_any_appliance] = Messages.IN_ANY_APPLIANCE;

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
            //ObjectTypes_images[ObjectTypes.StorageLinkServer] = Icons.StorageLinkServer;
            //ObjectTypes_images[ObjectTypes.StorageLinkSystem] = Icons.StorageLinkSystem;
            //ObjectTypes_images[ObjectTypes.StorageLinkPool] = Icons.StorageLinkPool;
            //ObjectTypes_images[ObjectTypes.StorageLinkVolume] = Icons.StorageLinkVolume;
            //ObjectTypes_images[ObjectTypes.StorageLinkRepository] = Icons.StorageLinkRepository;

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
            property_types.Add(PropertyNames.storageLinkServer, typeof(StorageLinkServer));
            property_types.Add(PropertyNames.storageLinkSystem, typeof(StorageLinkSystem));
            property_types.Add(PropertyNames.storageLinkPool, typeof(StorageLinkPool));
            property_types.Add(PropertyNames.storageLinkRepository, typeof(StorageLinkRepository));
			property_types.Add(PropertyNames.in_any_appliance, typeof(bool));
            property_types.Add(PropertyNames.disks, typeof(VDI));

            properties[PropertyNames.storageLinkServer] = delegate(IXenObject o)
            {
                StorageLinkSystem system = o as StorageLinkSystem;
                if (system != null)
                {
                    return system.StorageLinkConnection.Cache.Server;
                }
                StorageLinkPool pool = o as StorageLinkPool;
                if (pool != null)
                {
                    return pool.StorageLinkConnection.Cache.Server;
                }
                StorageLinkRepository r = o as StorageLinkRepository;
                if (r != null)
                {
                    return r.StorageLinkConnection.Cache.Server;
                }
                return null;
            };

            properties[PropertyNames.storageLinkSystem] = delegate(IXenObject o)
            {
                StorageLinkPool pool = o as StorageLinkPool;

                if (pool != null)
                {
                    return pool.StorageLinkSystem;
                }

                StorageLinkRepository r = o as StorageLinkRepository;

                if (r != null)
                {
                    return r.StorageLinkSystem;
                }

                return null;
            };

            properties[PropertyNames.storageLinkPool] = delegate(IXenObject o)
            {
                StorageLinkRepository repo = o as StorageLinkRepository;
                StorageLinkPool pool = repo != null ? repo.StorageLinkPool : null;

                if (pool != null)
                {
                    var l = new List<StorageLinkPool> { pool };
                    l.AddRange(pool.GetAncestors());
                    return l[l.Count - 1];
                }

                return null;
            };

            properties[PropertyNames.os_name] = delegate(IXenObject o)
                {
                    return GetForRealVM(o, delegate(VM vm, IXenConnection conn)
                                        {
                                            return vm.GetOSName();
                                        });
                };
            properties[PropertyNames.power_state] = delegate(IXenObject o)
                {
                    return GetForRealVM(o, delegate(VM vm, IXenConnection _)
                                        {
                                            return vm.power_state;
                                        });
                };
            properties[PropertyNames.virtualisation_status] = delegate(IXenObject o)
                {
                    return GetForRealVM(o, delegate(VM vm, IXenConnection conn)
                                        {
                                            return vm.GetVirtualisationStatus;
                                        });
                };
            properties[PropertyNames.start_time] = delegate(IXenObject o)
                {
                    return GetForRealVM(o, delegate(VM vm, IXenConnection conn)
                                        {
                                            return vm.GetStartTime();
                                        });
                };

            properties[PropertyNames.label] = delegate(IXenObject o)
                {
                    return o == null ? null : Helpers.GetName(o);
                };

            properties[PropertyNames.pool] = delegate(IXenObject o)
                {
                    return o == null ? null : Helpers.GetPool(o.Connection);
                };

            properties[PropertyNames.host] = HostProperty;
            properties[PropertyNames.vm] = VMProperty;
            properties[PropertyNames.dockervm] = DockerVMProperty;
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

            properties[PropertyNames.memory] = delegate(IXenObject o)
            {
                return GetForRealVM(o, delegate(VM vm, IXenConnection conn)
                                   {
                                       VM_metrics metrics = conn.Resolve(vm.metrics);
                                       return metrics == null ? null : (IComparable)metrics.memory_actual;
                                   });
            };

            properties[PropertyNames.ha_restart_priority] = delegate(IXenObject o)
                {
                    return GetForRealVM(o, delegate(VM vm, IXenConnection conn)
                                        {
                                            Pool pool = Helpers.GetPool(conn);
                                            if (pool == null || !pool.ha_enabled)
                                                return null;

                                            //CA-57600 - From Boston onwards, the HA restart priority list contains Restart instead of AlwaysRestartHighPriority and AlwaysRestart
                                            // when searching in a pre-Boston pool for VMs with HA restart priority = Restart, the search will return VMs with restart priority = AlwaysRestartHighPriority or AlwaysRestart
                                            if (vm.HaPriorityIsRestart()) 
                                                return VM.HA_Restart_Priority.Restart;
                                            return vm.HARestartPriority;
                                        });
                };

        	properties[PropertyNames.appliance] = delegate(IXenObject o)
                {
                    if (o is VM_appliance)
                        return (VM_appliance)o;

                    return GetForRealVM(o, (vm, conn) =>
                                            	{
                                            		if (vm.appliance == null || vm.Connection == null)
                                            			return null;

                                            		var appl = vm.Connection.Resolve(vm.appliance);
                                            		return appl;
                                            	});
                };

			properties[PropertyNames.in_any_appliance] = delegate(IXenObject o)
			{
				if (o is VM_appliance)
					return o != null;

				return GetForRealVM(o, (vm, conn) =>
				{
					if (vm.appliance == null || vm.Connection == null)
						return false;

					var appl = vm.Connection.Resolve(vm.appliance);
					return appl != null;
				});
			};

            properties[PropertyNames.read_caching_enabled] = delegate(IXenObject o)
            {
                return GetForRealVM(o, delegate(VM vm, IXenConnection conn)
                    {
                        return Helpers.CreamOrGreater(conn) ? (bool?)vm.ReadCachingEnabled : null;
                    });
            };

            properties[PropertyNames.connection_hostname] = ConnectionHostnameProperty;
            properties[PropertyNames.cpuText] = CPUTextProperty;
            properties[PropertyNames.cpuValue] = CPUValueProperty;
            properties[PropertyNames.description] = DescriptionProperty;
            properties[PropertyNames.diskText] = DiskTextProperty;
            properties[PropertyNames.folder] = Folders.GetFolder;
            properties[PropertyNames.folders] = Folders.GetAncestorFolders;
            properties[PropertyNames.haText] = HATextProperty;
            properties[PropertyNames.ha_enabled] = HAEnabledProperty;
            properties[PropertyNames.isNotFullyUpgraded] = IsNotFullyUpgradedProperty;
            properties[PropertyNames.ip_address] = IPAddressProperty;
            properties[PropertyNames.license] = LicenseProperty;
            properties[PropertyNames.memoryText] = MemoryTextProperty;
            properties[PropertyNames.memoryValue] = MemoryValueProperty;
            properties[PropertyNames.memoryRank] = MemoryRankProperty;
            properties[PropertyNames.networkText] = NetworkTextProperty;
            properties[PropertyNames.shared] = SharedProperty;
            properties[PropertyNames.size] = SizeProperty;
            properties[PropertyNames.sr_type] = SRTypeProperty;
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
            VM vm = o as VM;
            if (vm != null && vm.DescriptionType == VM.VmDescriptionType.None)
            {
                //return string.Empty instead of null, or it prints hyphens and looks ugly
                return string.Empty;
            }

            return o.Description;
        }
        
        private static IComparable SizeProperty(IXenObject o)
        {
            if (o is VDI)
            {
                VDI vdi = o as VDI;

                return vdi.virtual_size;
            }

            return null;
        }

        private static IComparable UptimeProperty(IXenObject o)
        {
            VM vm = o as VM;
            if (vm != null && vm.is_a_real_vm)
                return vm.RunningTime;

            Host host = o as Host;
            if (host != null)
                return host.Uptime;

            return null;
        }

        private static IComparable CPUTextProperty(IXenObject o)
        {
            VM vm = o as VM;
            if (vm != null && vm.is_a_real_vm)
            {
                if (vm.power_state != vm_power_state.Running)
                    return null;

                return PropertyAccessorHelper.vmCpuUsageString(vm);
            }

            Host host = o as Host;
            if (host != null)
            {
                if (!host.Connection.IsConnected)
                    return null;

                return PropertyAccessorHelper.hostCpuUsageString(host);
            }

            return null;
        }

        private static IComparable CPUValueProperty(IXenObject o)
        {
            VM vm = o as VM;
            if (vm != null && vm.is_a_real_vm)
            {
                if (vm.power_state != vm_power_state.Running)
                    return null;

                return PropertyAccessorHelper.vmCpuUsageRank(vm);
            }

            Host host = o as Host;
            if (host != null)
            {
                if (!host.Connection.IsConnected)
                    return null;

                return PropertyAccessorHelper.hostCpuUsageRank(host);
            }

            return null;
        }

        private static IComparable MemoryTextProperty(IXenObject o)
        {
            return Switch<IComparable>(o,
                delegate(VM vm)
                {
                    if (vm.not_a_real_vm ||
                        vm.power_state != vm_power_state.Running)
                        return null;

                    if (vm.virtualisation_status != VM.VirtualisationStatus.OPTIMIZED)
                        return null;

                    return PropertyAccessorHelper.vmMemoryUsageString(vm);
                },
                delegate(Host host)
                {
                    if (!host.Connection.IsConnected)
                        return null;

                    return PropertyAccessorHelper.hostMemoryUsageString(host);
                },
                null,
                null,
                delegate(VDI vdi)
                {
                    return PropertyAccessorHelper.vdiMemoryUsageString(vdi);
                },
                delegate(Folder folder)
                {
                    return String.Empty;
                }
                );
        }

        private static IComparable MemoryRankProperty(IXenObject o)
        {
            return Switch<IComparable>(o,
                delegate(VM vm)
                {
                    if (vm.not_a_real_vm ||
                        vm.power_state != vm_power_state.Running)
                        return null;

                    if (vm.virtualisation_status != VM.VirtualisationStatus.OPTIMIZED)
                        return null;

                    return PropertyAccessorHelper.vmMemoryUsageRank(vm);

                },
                delegate(Host host)
                {
                    if (!host.Connection.IsConnected)
                        return null;

                    return PropertyAccessorHelper.hostMemoryUsageRank(host);
                },
                null,
                null,
                delegate(VDI vdi)
                {
                    if (vdi.virtual_size == 0)
                        return 0;

                    return (int)((vdi.physical_utilisation * 100) / vdi.virtual_size);
                },
                null);
        }

        private static IComparable MemoryValueProperty(IXenObject o)
        {
            return Switch<IComparable>(o,
                delegate(VM vm)
                {
                    if (vm.not_a_real_vm ||
                        vm.power_state != vm_power_state.Running)
                        return null;

                    if (vm.virtualisation_status != VM.VirtualisationStatus.OPTIMIZED)
                        return null;

                    return PropertyAccessorHelper.vmMemoryUsageValue(vm);

                },
                delegate(Host host)
                {
                    if (!host.Connection.IsConnected)
                        return null;

                    return PropertyAccessorHelper.hostMemoryUsageValue(host);
                },
                null,
                null,
                delegate(VDI vdi)
                {
                    if (vdi.virtual_size == 0)
                        return 0.0;

                    return (double)vdi.physical_utilisation;
                },
                null);
        }

        private static IComparable NetworkTextProperty(IXenObject o)
        {
            VM vm = o as VM;
            if (vm != null)
            {
                if (vm.not_a_real_vm ||
                    vm.power_state != vm_power_state.Running)
                    return null;

                if (vm.virtualisation_status != VM.VirtualisationStatus.OPTIMIZED)
                    return null;

                return PropertyAccessorHelper.vmNetworkUsageString(vm);
            }

            Host host = o as Host;
            if (host != null)
            {
                if (!host.Connection.IsConnected)
                    return null;

                return PropertyAccessorHelper.hostNetworkUsageString(host);
            }

            return null;
        }

        private static IComparable DiskTextProperty(IXenObject o)
        {
            VM vm = o as VM;
            if (vm == null || vm.not_a_real_vm)
                return null;

            if (vm.power_state != vm_power_state.Running)
                return null;

            if (vm.virtualisation_status != VM.VirtualisationStatus.OPTIMIZED)
                return null;

            return PropertyAccessorHelper.vmDiskUsageString(vm);
        }

        private static IComparable HATextProperty(IXenObject o)
        {
            return Switch<IComparable>(o,
                PropertyAccessorHelper.GetVMHAStatus,
                null,
                PropertyAccessorHelper.GetPoolHAStatus,
                PropertyAccessorHelper.GetSRHAStatus,
                null,
                delegate(Folder folder)
                {
                    return "";
                });
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
            if (o == null)
                return null;

            IXenConnection xc = o.Connection;
            if (xc != null && xc.IsConnected)
                return xc.Hostname;
            return null;
        }

        private static IComparable SRTypeProperty(IXenObject o)
        {
            if (!(o is SR))
                return null;

            SR sr = (SR)o;

            return sr.GetSRType(false);
        }

        private static IComparable LicenseProperty(IXenObject o)
        {
            Pool pool = Helpers.GetPool(o.Connection);
            if (pool == null)
                return null;
            Host master = Helpers.GetMaster(pool.Connection);
            if (master == null)
                return null;

            return Helpers.GetFriendlyLicenseName(master);
        }

        private static IComparable SharedProperty(IXenObject o)
        {
            SR sr = o as SR;
            if (sr != null)
                return sr.shared;

            VDI vdi = o as VDI;
            if (vdi != null)
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

        private static IComparable HAEnabledProperty(IXenObject o)
        {
            Pool pool = o as Pool;
            return pool == null ? null : (IComparable)pool.ha_enabled;
        }

        private static IComparable IsNotFullyUpgradedProperty(IXenObject o)
        {
            Pool pool = o as Pool;
            return pool == null ? null : (IComparable)(!pool.IsPoolFullyUpgraded);
        }

        private static IComparable TypeProperty(IXenObject o)
        {
            if (o is VM)
            {
                VM vm = o as VM;
                if (vm.is_a_snapshot)
                {
                    return (IComparable)ObjectTypes.Snapshot;
                }
                else if (vm.is_a_template)
                {
                    return (IComparable)(vm.DefaultTemplate ? ObjectTypes.DefaultTemplate : ObjectTypes.UserTemplate);
                }
                else if (vm.is_control_domain)
                {
                    return null;
                }
                else
                {
                    return (IComparable)ObjectTypes.VM;
                }
            }
			else if (o is VM_appliance)
			{
				return (IComparable)ObjectTypes.Appliance;
			}
            else if (o is Host)
            {
                return (IComparable)((o.Connection.IsConnected) ? ObjectTypes.Server : ObjectTypes.DisconnectedServer);
            }
            else if (o is Pool)
            {
                return (IComparable)ObjectTypes.Pool;
            }
            else if (o is SR)
            {
                SR sr = o as SR;
                return (IComparable)(sr.IsLocalSR ? ObjectTypes.LocalSR : ObjectTypes.RemoteSR);
            }
            else if (o is XenAPI.Network)
            {
                return (IComparable)ObjectTypes.Network;
            }
            else if (o is VDI)
            {
                return (IComparable)ObjectTypes.VDI;
            }
            else if (o is Folder)
            {
                return (IComparable)ObjectTypes.Folder;
            }
            //else if (o is StorageLinkServer)
            //{
            //    return ObjectTypes.StorageLinkServer;
            //}
            //else if (o is StorageLinkSystem)
            //{
            //    return ObjectTypes.StorageLinkSystem;
            //}
            //else if (o is StorageLinkPool)
            //{
            //    return ObjectTypes.StorageLinkPool;
            //}
            //else if (o is StorageLinkVolume)
            //{
            //    return ObjectTypes.StorageLinkVolume;
            //}
            //else if (o is StorageLinkRepository)
            //{
            //    return ObjectTypes.StorageLinkRepository;
            //}
            else if (o is DockerContainer)
            {
                return (IComparable)ObjectTypes.DockerContainer;
            }

            return null;
        }

        private static ComparableList<XenAPI.Network> NetworksProperty(IXenObject o)
        {
            ComparableList<XenAPI.Network> networks = new ComparableList<XenAPI.Network>();

            if (o is VM)
            {
                VM vm = o as VM;
                if (vm.not_a_real_vm)
                    return null;

                foreach (VIF vif in vm.Connection.ResolveAll(vm.VIFs))
                {
                    XenAPI.Network network = vm.Connection.Resolve(vif.network);
                    if (network == null)
                        continue;

                    networks.Add(network);
                }


            }
            else if (o is XenAPI.Network)
            {
                networks.Add((XenAPI.Network)o);
            }

            return networks;
        }

        private static ComparableList<VM> VMProperty(IXenObject o)
        {
            ComparableList<VM> vms = new ComparableList<VM>();

            if (o is Pool)
            {
                vms.AddRange(o.Connection.Cache.VMs);
            }
            else if (o is Host)
            {
                Host host = o as Host;

                vms.AddRange(host.Connection.ResolveAll(host.resident_VMs));
            }
            else if (o is SR)
            {
                SR sr = o as SR;
                foreach (VDI vdi in sr.Connection.ResolveAll(sr.VDIs))
                    foreach (VBD vbd in vdi.Connection.ResolveAll(vdi.VBDs))
                    {
                        VM vm = vbd.Connection.Resolve(vbd.VM);
                        if (vm == null)
                            continue;

                        vms.Add(vm);
                    }
            }
            else if (o is XenAPI.Network)
            {
                XenAPI.Network network = o as XenAPI.Network;

                foreach (VIF vif in network.Connection.ResolveAll(network.VIFs))
                {
                    VM vm = vif.Connection.Resolve(vif.VM);
                    if (vm == null)
                        continue;

                    vms.Add(vm);
                }
            }
            else if (o is VDI)
            {
                VDI vdi = o as VDI;

                foreach (VBD vbd in vdi.Connection.ResolveAll(vdi.VBDs))
                {
                    VM vm = vbd.Connection.Resolve(vbd.VM);
                    if (vm == null)
                        continue;

                    vms.Add(vm);
                }
            }
            else if (o is VM)
            {
                VM vm = o as VM;

                if (vm.is_a_snapshot)
                {
                    VM from = vm.Connection.Resolve(vm.snapshot_of);
                    if (from != null)  // Can be null if VM has been deleted: CA-29249
                        vms.Add(from);
                }
                else
                    vms.Add(vm);
            }
            else if (o as DockerContainer != null)
            {
                vms.Add(((DockerContainer)o).Parent);
            }
            vms.RemoveAll(new Predicate<VM>(delegate(VM vm)
                {
                    return vm.not_a_real_vm;
                }
                              ));
            return vms;
        }

        private static ComparableList<VM> DockerVMProperty(IXenObject o)
        {
            ComparableList<VM> vms = new ComparableList<VM>();

            if (o as DockerContainer != null)
            {
                vms.Add(((DockerContainer)o).Parent);
            }

            return vms;
        }

        private static ComparableList<Host> HostProperty(IXenObject o)
        {
            ComparableList<Host> hosts = new ComparableList<Host>();

            if (o is IStorageLinkObject)
            {
                return null;
            }

            // If we're not in a pool then just group everything under the same host 
            Pool pool = Helpers.GetPool(o.Connection);
            if (pool == null)
            {
                // If pool == null there should only be one host
                foreach (Host host in o.Connection.Cache.Hosts)
                {
                    hosts.Add(host);
                }
            }
            else if (o is VM)
            {
                Host host = ((VM)o).Home();
                if (host != null)
                    hosts.Add(host);
            }
            else if (o is SR)
            {
                Host host = ((SR)o).Home;
                if (host != null)
                    hosts.Add(host);
            }
            else if (o is XenAPI.Network)
            {
                XenAPI.Network network = o as XenAPI.Network;

                if (network.PIFs.Count == 0)
                    hosts.AddRange(network.Connection.Cache.Hosts);
            }
            else if (o is Host)
            {
                hosts.Add(o as Host);
            }
            else if (o is VDI)
            {
                VDI vdi = o as VDI;
                SR sr = vdi.Connection.Resolve(vdi.SR);
                if (sr == null)
                    return null;

                hosts.Add(sr.Home);
            }
            else if (o is DockerContainer)
            {
                VM vm = (o as DockerContainer).Parent;
                Host host = vm.Home();
                if (host != null)
                    hosts.Add(host);
            }

            return hosts;
        }

        private static ComparableList<SR> StorageProperty(IXenObject o)
        {
            ComparableList<SR> srs = new ComparableList<SR>();

            if (o is VM)
            {
                VM vm = o as VM;
                if (vm.not_a_real_vm)
                    return null;

                foreach (VBD vbd in vm.Connection.ResolveAll(vm.VBDs))
                {
                    VDI vdi = vbd.Connection.Resolve(vbd.VDI);
                    if (vdi == null)
                        continue;

                    SR sr = vdi.Connection.Resolve(vdi.SR);
                    if (sr == null)
                        continue;

                    if (!srs.Contains(sr))
                        srs.Add(sr);
                }
            }
            else if (o is SR)
            {
                srs.Add((SR)o);
            }
            else if (o is VDI)
            {
                VDI vdi = o as VDI;
                SR sr = vdi.Connection.Resolve(vdi.SR);
                if (sr == null)
                    return null;

                srs.Add(sr);
            }

            return srs;
        }

        private static ComparableList<VDI> DisksProperty(IXenObject o)
        {
            ComparableList<VDI> vdis = new ComparableList<VDI>();

            if (o is VDI)
            {
                vdis.Add(o as VDI);
            }
            else if (o is VM)
            {
                VM vm = o as VM;
                if (vm.not_a_real_vm)
                    return null;

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
            ComparableList<ComparableAddress> addresses = new ComparableList<ComparableAddress>();

            if (o is VM)
            {
                VM vm = o as VM;
                if (vm.not_a_real_vm)
                    return null;

                VM_guest_metrics metrics = vm.Connection.Resolve(vm.guest_metrics);
                if (metrics == null)
                    return null;

                List<VIF> vifs = vm.Connection.ResolveAll(vm.VIFs);

                foreach (VIF vif in vifs)
                {
                    // PR-1373 - VM_guest_metrics.networks is a dictionary of IP addresses in the format:
                    // [["0/ip", <IPv4 address>], ["0/ipv6/0", <IPv6 address>], ["0/ipv6/1", <IPv6 address>]]
                    foreach (var network in metrics.networks.Where(n => n.Key.StartsWith(String.Format("{0}/ip", vif.device))))
                    {
                        ComparableAddress ipAddress;
                        if (!ComparableAddress.TryParse(network.Value, false, true, out ipAddress)) 
                            continue;

                        addresses.Add(ipAddress);
                    }
                }
            }
            else if (o is Host)
            {
                Host host = o as Host;

                foreach (PIF pif in host.Connection.ResolveAll(host.PIFs))
                {
                    ComparableAddress ipAddress;
                    if (!ComparableAddress.TryParse(pif.IP, false, true, out ipAddress))
                        continue;

                    addresses.Add(ipAddress);
                }
            }
            else if (o is SR)
            {
                SR sr = (SR)o;

                string target = sr.Target;
                if (!string.IsNullOrEmpty(target))
                {
                    ComparableAddress ipAddress;
                    if (ComparableAddress.TryParse(target, false, true, out ipAddress))
                        addresses.Add(ipAddress);
                }
            }
            else if (o is StorageLinkServer)
            {
                StorageLinkServer storageLinkServer = (StorageLinkServer)o;
                ComparableAddress ipAddress;
                if (ComparableAddress.TryParse(storageLinkServer.FriendlyName, false, true, out ipAddress))
                    addresses.Add(ipAddress);
            }

            return (addresses.Count == 0 ? null : addresses);   // CA-28300
        }

        private delegate IComparable VMGetter(VM vm, IXenConnection conn);
        private static IComparable GetForRealVM(IXenObject o, VMGetter getter)
        {
            VM vm = o as VM;
            if (vm == null || vm.not_a_real_vm)
                return null;
            return getter(vm, vm.Connection);
        }

        public static PropertyAccessor Get(PropertyNames p)
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

        public static Object GetImagesFor(PropertyNames p)
        {
            switch (p)
            {
                case PropertyNames.type:
                    return (ImageDelegate<ObjectTypes>)delegate(ObjectTypes type)
                        {
                            return ObjectTypes_images[type];
                        };

                case PropertyNames.power_state:
                    return (ImageDelegate<vm_power_state>)delegate(vm_power_state type)
                        {
                            return VM_power_state_images[type];
                        };

                case PropertyNames.networks:
                    return (ImageDelegate<XenAPI.Network>)delegate 
                        {
                            return Icons.Network;
                        }; 

                case PropertyNames.appliance:
                    return (ImageDelegate<VM_appliance>)delegate
                        {
                            return Icons.VmAppliance;
                        };

                case PropertyNames.os_name:
                    return (ImageDelegate<String>)delegate(String os_name)
                        {
                            String os = os_name.ToLowerInvariant();

                            if (os.Contains("debian"))
                                return Icons.Debian;
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
                            if (os.Contains("windows"))
                                return Icons.Windows;
                            if (os.Contains("coreos"))
                                return Icons.CoreOS;

                            return Icons.XenCenter;
                        };

                case PropertyNames.virtualisation_status:
                    return (ImageDelegate<VM.VirtualisationStatus>)delegate(VM.VirtualisationStatus status)
                        {
                            switch (status)
                            {
                                case VM.VirtualisationStatus.OPTIMIZED:
                                    return Icons.ToolInstalled;

                                case VM.VirtualisationStatus.PV_DRIVERS_NOT_INSTALLED:
                                    return Icons.ToolsNotInstalled;

                                case VM.VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE:
                                    return Icons.ToolsOutOfDate;

                                default:
                                    return Icons.ToolsNotInstalled;
                            }
                        };

                case PropertyNames.sr_type:
                    return (ImageDelegate<SR.SRTypes>)delegate(SR.SRTypes type)
                        {
                            return Icons.Storage;
                        };

                case PropertyNames.tags:
                    return (ImageDelegate<string>)delegate(string tag)
                        {
                            return Icons.Tag;
                        };

                case PropertyNames.ha_restart_priority:
                    return (ImageDelegate<VM.HA_Restart_Priority>)delegate(VM.HA_Restart_Priority _)
                        {
                            return Icons.HA;
                        };

                case PropertyNames.read_caching_enabled:
                    return (ImageDelegate<bool>)delegate(bool _)
                        {
                            return Icons.VDI;
                        };

                default:
                    return null;
            }
        }

        public delegate S WithDelegate<S, T>(T t) where T : XenObject<T>;

        public static Object With<S, T>(IXenObject o, WithDelegate<S, T> withDelegate) where T : XenObject<T>
        {
            T t = o as T;

            if (t != null)
                return withDelegate(t);

            return default(S);
        }

        public static S Switch<S>(IXenObject o, WithDelegate<S, VM> vmDelegate,
            WithDelegate<S, Host> hostDelegate, WithDelegate<S, Pool> poolDelegate,
            WithDelegate<S, SR> srDelegate, WithDelegate<S, VDI> vdiDelegate,
            WithDelegate<S, Folder> folderDelegate)
        {
            VM vm = o as VM;
            if (vm != null && vmDelegate != null)
                return vmDelegate(vm);

            Host host = o as Host;
            if (host != null && hostDelegate != null)
                return hostDelegate(host);

            Pool pool = o as Pool;
            if (pool != null && poolDelegate != null)
                return poolDelegate(pool);

            SR sr = o as SR;
            if (sr != null && srDelegate != null)
                return srDelegate(sr);

            VDI vdi = o as VDI;
            if (vdi != null && vdiDelegate != null)
                return vdiDelegate(vdi);

            Folder folder = o as Folder;
            if (folder != null && folderDelegate != null)
                return folderDelegate(folder);

            return default(S);
        }

        public static PropertyNames GetSortPropertyName(ColumnNames c)
        {
            return column_sort_by[c];
        }
    }

    public class PropertyWrapper
    {
        private readonly PropertyAccessor property;
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
