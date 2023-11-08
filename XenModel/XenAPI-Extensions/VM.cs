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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Timers;
using System.Xml;
using Newtonsoft.Json;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;

namespace XenAPI
{
    public partial class VM
    {
        /// <remarks>
        ///     AlwaysRestartHighPriority and AlwaysRestart are replaced by Restart in Boston; we still keep them for backward
        ///     compatibility
        /// </remarks>
        public enum HaRestartPriority
        {
            AlwaysRestartHighPriority,
            AlwaysRestart,
            Restart,
            BestEffort,
            DoNotRestart
        }

        [Flags]
        public enum VirtualizationStatus
        {
            NotInstalled = 0,
            Unknown = 1,
            PvDriversOutOfDate = 2,
            IoDriversInstalled = 4,
            ManagementInstalled = 8
        }

        public enum VmDescriptionType
        {
            None,
            ReadOnly,
            ReadWrite
        }

        /// <summary>
        ///     Sort in the following order:
        ///     1) User Templates
        ///     2) Windows VMs
        ///     3) Other VMs (e.g. Linux . Names in alphabetical order)
        ///     4) Citrix VMs (e.g. XenApp templates)
        ///     5) Misc VMs
        ///     6) Regular snapshots
        ///     7) Snapshots from VMPP (CA-46206)
        ///     Last: Hidden VMs (only visible if "Show Hidden Objects" is on: see CA-39036).
        /// </summary>
        public enum VmTemplateType
        {
            NoTemplate = 0, //it's not a template
            Custom = 1,
            Windows = 2,
            WindowsServer = 3,
            LegacyWindows = 4,
            Asianux = 5,
            Centos = 6,
            CoreOS = 7,
            Debian = 8,
            Gooroom = 9,
            Linx = 10,
            NeoKylin = 11,
            Oracle = 12,
            RedHat = 13,
            Rocky = 14,
            SciLinux = 15,
            Suse = 16,
            Turbo = 17,
            Ubuntu = 18,
            YinheKylin = 19,
            Citrix = 20,
            Solaris = 21,
            Misc = 22,
            Snapshot = 23,
            SnapshotFromVmpp = 24,
            Count = 25 //bump this if values are added
        }

        // The following variables are only used when the corresponding variable is missing from
        // the recommendations field of the VM (which is inherited from the recommendations field
        // of the template it was created from). This should not normally happen, so we just use
        // the maximum that any VM can have as a backstop, without worrying about different OS's
        // or different XenServer versions.
        private const int DEFAULT_NUM_VCPUS_ALLOWED = 16;
        private const int DEFAULT_NUM_VIFS_ALLOWED = 7;
        private const int DEFAULT_NUM_VBDS_ALLOWED = 255;
        public const long DEFAULT_MEM_ALLOWED = 1 * Util.BINARY_TERA;
        public const long DEFAULT_MEM_MIN_IMG_IMPORT = 256 * Util.BINARY_MEGA;
        public const int DEFAULT_CORES_PER_SOCKET = 1;

        public const long MAX_SOCKETS = 16; // current hard limit in Xen: CA-198276

        // CP-41825: > 32 vCPUs is only supported for trusted VMs
        public const long MAX_VCPUS_FOR_NON_TRUSTED_VMS = 32;
        public const int MAX_ALLOWED_VTPMS = 1;

        private const string P2V_SOURCE_MACHINE = "p2v_source_machine";
        private const string P2V_IMPORT_DATE = "p2v_import_date";

        public const string RESTART_PRIORITY_ALWAYS_RESTART_HIGH_PRIORITY = "0"; //only used for Pre-Boston pools
        public const string RESTART_PRIORITY_ALWAYS_RESTART = "1"; //only used for Pre-Boston pools

        /// <summary>
        ///     This is the new "Restart" priority in Boston, and will replace RESTART_PRIORITY_ALWAYS_RESTART_HIGH_PRIORITY and
        ///     RESTART_PRIORITY_ALWAYS_RESTART
        /// </summary>
        public const string RESTART_PRIORITY_RESTART = "restart";

        public const string RESTART_PRIORITY_BEST_EFFORT = "best-effort";
        public const string RESTART_PRIORITY_DO_NOT_RESTART = "";

        /// <summary>
        ///     List of distros that we treat as Linux/Non-Windows (written in the VM.guest_metrics
        ///     by the Linux Guest Agent after evaluating xe-linux-distribution)
        /// </summary>
        private static readonly string[] LinuxDistros =
        {
            "debian", "rhel", "fedora", "centos", "scientific", "oracle", "sles",
            "lsb", "boot2docker", "freebsd", "ubuntu", "neokylin", "gooroom", "rocky"
        };

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        private bool _isBeingCreated;

        private DateTime _startupTime;

        private Timer _virtualizationTimer;

        [JsonIgnore]
        public bool IsBeingCreated
        {
            get => _isBeingCreated;
            set
            {
                _isBeingCreated = value;
                NotifyPropertyChanged("IsBeingCreated");
            }
        }

        #region Restriction Getters

        /// <summary>
        ///     Returns true if
        ///     1) the guest is HVM and
        ///     2a) the allow-gpu-passthrough restriction is absent or
        ///     2b) the allow-gpu-passthrough restriction is non-zero
        /// </summary>
        public bool CanHaveGpu()
        {
            if (!IsHVM())
                return false;

            return GetIntRestrictionValue(this, "allow-gpu-passthrough", 1) != 0;
        }

        public bool HasSriovRecommendation()
        {
            return GetIntRestrictionValue(this, "allow-network-sriov", 0) != 0;
        }

        public bool HasVendorDeviceRecommendation()
        {
            return GetBoolRestrictionValue(this, "has-vendor-device", false);
        }

        /// <summary>
        ///     Returns true if
        ///     1) the guest is HVM and
        ///     2a) the allow-vgpu restriction is absent or
        ///     2b) the allow-vgpu restriction is non-zero
        /// </summary>
        public bool CanHaveVGpu()
        {
            if (!IsHVM() || !CanHaveGpu())
                return false;


            return GetIntRestrictionValue(this, "allow-vgpu", 1) != 0;
        }

        public long MaxMemAllowed()
        {
            return GetMaxRestrictionValue(this, "memory-static-max", DEFAULT_MEM_ALLOWED);
        }

        public int MaxVIFsAllowed()
        {
            return GetMaxRestrictionValue(this, "number-of-vifs", DEFAULT_NUM_VIFS_ALLOWED);
        }

        public int MaxVBDsAllowed()
        {
            return GetMaxRestrictionValue(this, "number-of-vbds", DEFAULT_NUM_VBDS_ALLOWED);
        }

        public int MaxVCPUsAllowed()
        {
            return GetMaxRestrictionValue(this, "vcpus-max", DEFAULT_NUM_VCPUS_ALLOWED);
        }

        public int MinVCPUs()
        {
            return GetMinRestrictionValue(this, "vcpus-min", 1);
        }

        /// <summary>
        /// Attempt to fetch a restriction value from a VM template with the same <see cref="reference_label"/>
        /// as the input VM.<br />
        /// This ensures that restriction values are not limiting guest capabilities when restrictions change
        /// across server upgrades.<br />
        /// See CP-44766 for more info.
        /// </summary>
        /// <typeparam name="T">The nullable type of the value. For instance <see cref="Nullable{Int32}"/> should be used for vcpus-max</typeparam>
        /// <param name="vm">The VM whose restrictions we're looking for</param>
        /// <param name="field">The name of the field. For the max number of vCPUs it's vcpus-max</param>
        /// <param name="attribute">The XML attribute corresponding to the value we need. For instance, it's "max" for vcpus-max</param>
        /// <returns>The value if found. If not found, null is returned instead</returns>
        private static T? GetRestrictionValueFromMatchingTemplate<T>(VM vm, string field, string attribute) where T : struct
        {
            if(vm.is_a_template)
                return GetRestrictionValue<T>(vm, field, attribute);

            if (!string.IsNullOrEmpty(vm.reference_label))
            {
                var matchingTemplate = vm.Connection.Cache.VMs
                    .FirstOrDefault(v => v.is_a_template && v.reference_label == vm.reference_label);

                if (matchingTemplate != null)
                {
                    return GetRestrictionValue<T>(matchingTemplate, field, attribute);
                }
            }

            return null;
        }

        /// <summary>
        /// Get a value from the VM&apos;s restrictions stored in its recommendations parameter.
        /// </summary>
        /// <typeparam name="T">The type of the value. For instance <see cref="int"/> should be used for vcpus-max</typeparam>
        /// <param name="vm">The VM whose restrictions we're looking for</param>
        /// <param name="field">The name of the field. For the max number of vCPUs it's vcpus-max</param>
        /// <param name="attribute">The XML attribute of the restriction element corresponding to the value we need. For instance, it's "max" for vcpus-max</param>
        /// <returns>The value if found. If not found, null is returned instead</returns>
        private static T? GetRestrictionValue<T>(VM vm, string field, string attribute) where T : struct
        {
            var xd = vm.GetRecommendations();
            var xn = xd?.SelectSingleNode($@"restrictions/restriction[@field='{field}']");
            var resultString = xn?.Attributes?[attribute]?.Value;
            T? result = null;

            // avoid the expensive operation of throwing an exception
            if (string.IsNullOrEmpty(resultString))
            {
                return null;
            }

            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter.ConvertFromString(resultString) is T convertedResult)
                {
                    result = convertedResult;
                }
            }
            catch (NotSupportedException)
            {
                // either the XML value cannot be converted
                // or there is no converter
                // we simply fall back to the default value
            }
            return result;
        }

        /// <summary>
        /// If the a matching template can't be found, we get the value from all template restrictions,
        /// falling back to a default  if none is found.
        /// This can be used to fetch limits for resource counts on the VM&apos;s host.
        /// See CP-44767 for more information.<br />
        /// Try and create wrapper methods for this call such as <see cref="GetMaxRestrictionValue{TSource}"/> when possible.
        /// </summary>
        /// <typeparam name="T">The type of the value. For instance <see cref="int"/> should be used for vcpus-max</typeparam>
        /// <param name="vm">The VM whose restrictions we're looking for</param>
        /// <param name="field">The name of the field. For the max number of vCPUs it's vcpus-max</param>
        /// <param name="attribute">The XML attribute corresponding to the value we need. For instance, it's "max" for vcpus-max</param>
        /// <returns>A list of all the non-null values present in all cached templates. This list may be empty</returns>
        private static List<T> GetRestrictionValueAcrossTemplates<T>(IXenObject vm, string field, string attribute) where T : struct
        {
            return vm.Connection.Cache.VMs
                .AsParallel()
                .Where(v => v.is_a_template)
                .Select(v => GetRestrictionValue<T>(v, field, attribute))
                .Where(value => value != null)
                .Cast<T>()
                .ToList();
        }

        /// <summary>
        /// Get the maximum restriction value. Attempts to fetch the value using <see cref="GetRestrictionValueFromMatchingTemplate{TSource}" /> at first. <br />
        /// If nothing is found, it looks for the maximum value in all templates using <see cref="Enumerable.Max{TSource}(IEnumerable{TSource})"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value. For instance <see cref="int"/> should be used for vcpus-max</typeparam>
        /// <param name="vm">The VM whose restrictions we're looking for</param>
        /// <param name="field">The name of the field. For the max number of vCPUs it's vcpus-max</param>
        /// <param name="defaultValue">Fallback value. Defaults to this value if no valid alternatives are found.</param>
        /// <returns>The value of the matching template if found. Else the maximum non null value in all VM templates. Else, the given defaultValue</returns>
        private static T GetMaxRestrictionValue<T>(VM vm, string field, T defaultValue) where T : struct
        {
            var value = GetRestrictionValueFromMatchingTemplate<T>(vm, field, "max");
            if (value != null) 
                return (T) value;

            var templateValues = GetRestrictionValueAcrossTemplates<T>(vm, field, "max");
            templateValues.Add(defaultValue);
            return templateValues.Max();
        }

        /// <summary>
        /// Get the minimum restriction value. Attempts to fetch the value using <see cref="GetRestrictionValueFromMatchingTemplate{TSource}" /> at first. <br />
        /// If nothing is found, it looks for the minimum value in all templates using <see cref="Enumerable.Max{TSource}(IEnumerable{TSource})"/>.
        /// </summary>
        /// <typeparam name="T">The type of the value. For instance <see cref="int"/> should be used for vcpus-max</typeparam>
        /// <param name="vm">The VM whose restrictions we're looking for</param>
        /// <param name="field">The name of the field. For the max number of vCPUs it's vcpus-max</param>
        /// <param name="defaultValue">Fallback value. Defaults to this value if no valid alternatives are found.</param>
        /// <returns>The value of the matching template if found. Else the maximum non null value in all VM templates. Else, the given defaultValue</returns>
        private static T GetMinRestrictionValue<T>(VM vm, string field, T defaultValue) where T : struct
        {
            var value = GetRestrictionValueFromMatchingTemplate<T>(vm, field, "min");
            if (value != null)
                return (T)value;

            var templateValues = GetRestrictionValueAcrossTemplates<T>(vm, field, "min");
            templateValues.Add(defaultValue);
            return templateValues.Min();
        }

        /// <summary>
        /// Get the restriction value. Attempts to fetch the value using <see cref="GetRestrictionValueFromMatchingTemplate{TSource}" />.
        /// </summary>
        /// <param name="vm">The VM whose restrictions we're looking for</param>
        /// <param name="field">The name of the field. For the max number of vCPUs it's vcpus-max</param>
        /// <param name="defaultValue">Fallback value. Defaults to this value if no matching templates with a non-null value are found.</param>
        /// <returns>The value of the matching template if found.Else, the given defaultValue</returns>
        private static int GetIntRestrictionValue(VM vm, string field, int defaultValue)
        {
            return GetRestrictionValueFromMatchingTemplate<int>(vm, field, "value") ?? defaultValue;
        }

        /// <summary>
        /// Get the restriction value. Attempts to fetch the value using <see cref="GetRestrictionValueFromMatchingTemplate{TSource}" />.
        /// </summary>
        /// <param name="vm">The VM whose restrictions we're looking for</param>
        /// <param name="field">The name of the field. For the max number of vCPUs it's vcpus-max</param>
        /// <param name="defaultValue">Fallback value. Defaults to this value if no matching templates with a non-null value are found.</param>
        /// <returns>The value of the matching template if found.Else, the given defaultValue</returns>
        private static bool GetBoolRestrictionValue(VM vm, string field, bool defaultValue)
        {
            return GetRestrictionValueFromMatchingTemplate<bool>(vm, field, "value") ?? defaultValue;
        }

        private XmlDocument _xdRecommendations;

        /// <summary>
        /// Parse and return the content of the recommendations XML
        /// </summary>
        /// <returns>Parsed XML if found. null otherwise</returns>
        private XmlDocument GetRecommendations()
        {
            if (_xdRecommendations != null)
                return _xdRecommendations;

            if (string.IsNullOrEmpty(recommendations))
                return null;

            _xdRecommendations = new XmlDocument();

            try
            {
                _xdRecommendations.LoadXml(recommendations);
            }
            catch
            {
                _xdRecommendations = null;
            }

            return _xdRecommendations;
        }

        #endregion


        public bool IsRunning()
        {
            return power_state == vm_power_state.Running;
        }

        /// <summary>
        ///     Returns true if the VM's pool has HA enabled and the VM has a saved restart priority other than DoNotRestart.
        ///     Does not take account of ha-always-run.
        /// </summary>
        public bool HasSavedRestartPriority()
        {
            var pool = Helpers.GetPoolOfOne(Connection);
            return pool != null && pool.ha_enabled && !string.IsNullOrEmpty(ha_restart_priority);
        }

        /// <summary>
        ///     Get the given VM's home, i.e. the host under which we are going to display it.  May return null, if this VM should
        ///     live
        ///     at the pool level. For a normal VM, we look at (1) where it's running; (2) where its storage forces it to run;
        ///     (3) what its affinity is (its requested but not guaranteed host).
        /// </summary>
        public virtual Host Home()
        {
            if (is_a_snapshot) // Snapshots have the same "home" as their VM. This is necessary to make a pool-server-VM-snapshot tree (CA-76273).
            {
                var from = Connection.Resolve(snapshot_of);
                return from?.Home(); // "from" can be null if VM has been deleted
            }

            if (is_a_template) // Templates (apart from snapshots) don't have a "home", even if their affinity is set CA-36286
                return null;

            if (power_state == vm_power_state.Running)
                return Connection.Resolve(resident_on);

            var storageHost = GetStorageHost(false);
            if (storageHost != null)
                return storageHost;

            var affinityHost = Connection.Resolve(affinity);
            if (affinityHost != null && affinityHost.IsLive())
                return affinityHost;

            return null;
        }

        public VBD FindVMCDROM()
        {
            if (Connection == null)
                return null;

            var vbds = Connection.ResolveAll(VBDs).FindAll(vbd => vbd.IsCDROM());

            if (vbds.Count > 0)
            {
                vbds.Sort();
                return vbds[0];
            }

            return null;
        }

        public SR FindVMCDROMSR()
        {
            var vbd = FindVMCDROM();
            var vdi = vbd?.Connection.Resolve(vbd.VDI);
            return vdi?.Connection.Resolve(vdi.SR);
        }

        public override string Name()
        {
            const string controlDomain = "Control domain on host: ";
            if (name_label != null && name_label.StartsWith(controlDomain))
            {
                var hostName = name_label.Substring(controlDomain.Length);
                return string.Format(Messages.CONTROL_DOM_ON_HOST, hostName);
            }

            return name_label;
        }

        public Host GetStorageHost(bool ignoreCDs)
        {
            foreach (var theVBD in Connection.ResolveAll(VBDs))
            {
                if (ignoreCDs && theVBD.type == vbd_type.CD)
                    continue;
                var theVDI = Connection.Resolve(theVBD.VDI);

                if (theVDI == null || !theVDI.Show(true))
                    continue;
                var theSr = Connection.Resolve(theVDI.SR);
                var host = theSr?.GetStorageHost();
                if (host != null) return host;
            }

            return null;
        }

        /// <remarks>
        ///     Default on server is CD - disk then optical
        /// </remarks>
        public string GetBootOrder()
        {
            if (HVM_boot_params.ContainsKey("order"))
                return HVM_boot_params["order"].ToUpper();

            return "CD";
        }

        public void SetBootOrder(string value)
        {
            HVM_boot_params = SetDictionaryKey(HVM_boot_params, "order", value.ToLower());
        }

        public bool IsPvVm()
        {
            return IsRealVm() && !IsHVM() && !other_config.ContainsKey("pvcheckpass");
        }

        public int GetVcpuWeight()
        {
            if (VCPUs_params != null && VCPUs_params.ContainsKey("weight"))
            {
                int weight;
                if (int.TryParse(VCPUs_params["weight"],
                        out weight)) // if we cant parse it we assume its because it is too large, obviously if it isnt a number (ie a string) then we will still go to the else
                    return
                        weight > 0
                            ? weight
                            : 1; // because we perform a log on what is returned from this the weight must always be greater than 0
                return 65536; // could not parse number, assume max
            }

            return 256;
        }

        public void SetVcpuWeight(int value)
        {
            VCPUs_params = SetDictionaryKey(VCPUs_params, "weight", value.ToString());
        }

        public bool DefaultTemplate()
        {
            return Get(other_config, "default_template") != null;
        }

        public bool InternalTemplate()
        {
            return Get(other_config, "xensource_internal") != null;
        }

        public string InstallRepository()
        {
            return Get(other_config, "install-repository");
        }

        public string InstallDistro()
        {
            return Get(other_config, "install-distro");
        }

        public string InstallMethods()
        {
            return Get(other_config, "install-methods");
        }

        public bool IsHVM()
        {
            return HVM_boot_policy != "";
        }

        public bool HasRDP()
        {
            var metrics = Connection.Resolve(guest_metrics);
            if (metrics == null)
                return false;
            // feature-ts is the feature flag indicating the toolstack
            // can enable RDP remotely (by writing to control/ts)
            return 0 != IntKey(metrics.other, "feature-ts", 0);
        }

        public bool RDPEnabled()
        {
            var vmMetrics = Connection.Resolve(guest_metrics);
            if (vmMetrics == null)
                return false;
            // data/ts indicates the VM has RDP enabled
            return 0 != IntKey(vmMetrics.other, "data-ts", 0);
        }

        public bool RDPControlEnabled()
        {
            var metrics = Connection.Resolve(guest_metrics);
            if (metrics == null)
                return false;
            // feature-ts2 is the feature flag indicating that data/ts is valid
            return 0 != IntKey(metrics.other, "feature-ts2", 0);
        }

        public bool CanUseRDP()
        {
            var guestMetrics = Connection.Resolve(guest_metrics);
            if (guestMetrics == null)
                return false;

            return IntKey(guestMetrics.other, "feature-ts2", 0) != 0 &&
                   IntKey(guestMetrics.other, "feature-ts", 0) != 0 &&
                   IntKey(guestMetrics.other, "data-ts", 0) != 0 &&
                   // CA-322672: Can't connect using RDP if there are no networks.
                   // The network object contains the IP info written by the xenvif
                   // driver (which needs a 1st reboot to swap out the emulated network adapter)
                   guestMetrics.networks.Count > 0;
        }

        // AutoPowerOn is supposed to be unsupported. However, we advise customers how to
        // enable it (http://support.citrix.com/article/CTX133910), so XenCenter has to be
        // able to recognize it, and turn it off during Rolling Pool Upgrade.
        public bool GetAutoPowerOn()
        {
            return BoolKey(other_config, "auto_poweron");
        }

        public void SetAutoPowerOn(bool value)
        {
            other_config = SetDictionaryKey(other_config, "auto_poweron", value.ToString().ToLower());
        }

        public string IsOnSharedStorage()
        {
            foreach (var vbdRef in VBDs)
            {
                var vbd = Connection.Resolve(vbdRef);
                if (vbd != null)
                {
                    var vdi = Connection.Resolve(vbd.VDI);
                    if (vdi != null)
                    {
                        var sr = Connection.Resolve(vdi.SR);
                        if (sr != null && !sr.shared)
                        {
                            if (sr.content_type == SR.Content_Type_ISO) return Messages.EJECT_YOUR_CD;

                            return Messages.VM_USES_LOCAL_STORAGE;
                        }
                    }
                }
            }

            return "";
        }

        public ulong GetTotalSize()
        {
            ulong virtualSize = 0;

            foreach (var vbdRef in VBDs)
            {
                var vbd = Connection.Resolve(vbdRef);
                if (vbd == null)
                    continue;

                if (vbd.type == vbd_type.CD)
                    continue;

                VDI vdi = Connection.Resolve(vbd.VDI);
                if (vdi == null)
                    continue;

                virtualSize += (ulong)vdi.virtual_size;
            }

            return virtualSize;
        }

        public override int CompareTo(VM other)
        {
            // Sort in the following order:
            // 1) Control domain
            // 2) Normal VMs
            // 3) Snapshots
            // 4) User templates
            // 5) Default templates
            // Within each category, using CompareNames()

            int myClass, otherClass;

            if (is_control_domain)
                myClass = 1;
            else if (is_a_snapshot)
                myClass = 3;
            else if (is_a_template)
                myClass = DefaultTemplate() ? 5 : 4;
            else
                myClass = 2;

            if (other.is_control_domain)
                otherClass = 1;
            else if (other.is_a_snapshot)
                otherClass = 3;
            else if (other.is_a_template)
                otherClass = other.DefaultTemplate() ? 5 : 4;
            else
                otherClass = 2;

            if (myClass != otherClass)
                return myClass - otherClass;
            return base.CompareTo(other);
        }

        /// <summary>
        ///     These are the operations that make us show the orange icon for the VM in the tree
        ///     and on the Memory tab. It's shorter to add the ones that cause problems.
        /// </summary>
        public static bool is_lifecycle_operation(vm_operations op)
        {
            return op != vm_operations.changing_dynamic_range && op != vm_operations.changing_static_range &&
                   op != vm_operations.changing_memory_limits;
        }

        public DateTime GetBodgeStartupTime()
        {
            return _startupTime;
        }

        public void SetBodgeStartupTime(DateTime value)
        {
            _startupTime = value;
            // This has an impact on the virt state of the VM as we allow a set amount of time for tools to show up before assuming unvirt
            NotifyPropertyChanged("virtualisation_status");
            _virtualizationTimer?.Stop();
            // 2 minutes before we give up plus some breathing space
            _virtualizationTimer = new Timer(182000) { AutoReset = false };
            _virtualizationTimer.Elapsed += VirtualizationTimer_Elapsed;
            _virtualizationTimer.Start();
        }

        private void VirtualizationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NotifyPropertyChanged("virtualisation_status");
        }

        public string GetVirtualizationWarningMessages()
        {
            var status = GetVirtualizationStatus(out _);

            if ((status.HasFlag(VirtualizationStatus.IoDriversInstalled) &&
                 status.HasFlag(VirtualizationStatus.ManagementInstalled))
                || status.HasFlag(VirtualizationStatus.Unknown))
                // calling function shouldn't send us here if tools are, or might be, present: used to assert here but it can sometimes happen (CA-51460)
                return "";

            if (status.HasFlag(VirtualizationStatus.PvDriversOutOfDate))
            {
                var guestMetrics = Connection.Resolve(guest_metrics);
                if (guestMetrics != null
                    && guestMetrics.PV_drivers_version.ContainsKey("major")
                    && guestMetrics.PV_drivers_version.ContainsKey("minor"))
                    return string.Format(Messages.PV_DRIVERS_OUT_OF_DATE, BrandManager.VmTools,
                        guestMetrics.PV_drivers_version["major"], guestMetrics.PV_drivers_version["minor"]);

                return string.Format(Messages.PV_DRIVERS_OUT_OF_DATE_UNKNOWN_VERSION, BrandManager.VmTools);
            }

            return HasNewVirtualizationStates()
                ? Messages.VIRTUALIZATION_STATE_VM_MANAGEMENT_AGENT_NOT_INSTALLED
                : string.Format(Messages.PV_DRIVERS_NOT_INSTALLED, BrandManager.VmTools);
        }

        /// <summary>
        ///     Virtualization Status of the VM
        /// </summary>
        /// <remarks>
        ///     Following states are expected:
        ///     For Non-Windows VMs and for Windows VMs pre-Dundee:
        ///     0 = Not installed
        ///     1 = Unknown
        ///     2 = Out of date
        ///     12 = Tools installed (Optimized)
        ///     For Windows VMs on Dundee or higher:
        ///     0 = Not installed
        ///     1 = Unknown
        ///     4 = I/O Optimized
        ///     12 = I/O and Management installed
        /// </remarks>
        public VirtualizationStatus GetVirtualizationStatus(out string friendlyStatus)
        {
            friendlyStatus = Messages.VIRTUALIZATION_UNKNOWN;

            if (Connection == null || power_state != vm_power_state.Running || Connection.Resolve(metrics) == null)
                return VirtualizationStatus.Unknown;

            var vmGuestMetrics = Connection.Resolve(guest_metrics);
            var lessThanTwoMin = (DateTime.UtcNow - GetBodgeStartupTime()).TotalMinutes < 2;

            if (HasNewVirtualizationStates())
            {
                var flags = VirtualizationStatus.NotInstalled;

                if (vmGuestMetrics != null && vmGuestMetrics.PV_drivers_detected)
                    flags |= VirtualizationStatus.IoDriversInstalled;

                if (vmGuestMetrics != null && IntKey(vmGuestMetrics.other, "feature-static-ip-setting", 0) != 0)
                    flags |= VirtualizationStatus.ManagementInstalled;

                if (flags.HasFlag(VirtualizationStatus.IoDriversInstalled | VirtualizationStatus.ManagementInstalled))
                    friendlyStatus = Messages.VIRTUALIZATION_STATE_VM_IO_DRIVERS_AND_MANAGEMENT_AGENT_INSTALLED;
                else if (flags.HasFlag(VirtualizationStatus.IoDriversInstalled))
                    friendlyStatus = Messages.VIRTUALIZATION_STATE_VM_MANAGEMENT_AGENT_NOT_INSTALLED;
                else if (lessThanTwoMin)
                    flags = VirtualizationStatus.Unknown;
                else
                    friendlyStatus = string.Format(Messages.PV_DRIVERS_NOT_INSTALLED, BrandManager.VmTools);

                return flags;
            }

            if (vmGuestMetrics == null || !vmGuestMetrics.PV_drivers_installed())
                if (lessThanTwoMin)
                {
                    return VirtualizationStatus.Unknown;
                }
                else
                {
                    friendlyStatus = string.Format(Messages.PV_DRIVERS_NOT_INSTALLED, BrandManager.VmTools);
                    return VirtualizationStatus.NotInstalled;
                }

            if (!vmGuestMetrics.PV_drivers_version.TryGetValue("major", out var major))
                major = "0";
            if (!vmGuestMetrics.PV_drivers_version.TryGetValue("minor", out var minor))
                minor = "0";

            if (!vmGuestMetrics.PV_drivers_up_to_date)
            {
                friendlyStatus = string.Format(Messages.VIRTUALIZATION_OUT_OF_DATE, $"{major}.{minor}");
                return VirtualizationStatus.PvDriversOutOfDate;
            }

            friendlyStatus = string.Format(Messages.VIRTUALIZATION_OPTIMIZED, $"{major}.{minor}");
            return VirtualizationStatus.IoDriversInstalled | VirtualizationStatus.ManagementInstalled;
        }

        /// <summary>
        ///     Is this a Windows VM on Dundee or higher host?
        ///     We need to know this, because for those VMs virtualization status is defined differently.
        ///     This does not mean new(ly created) VM
        /// </summary>
        public bool HasNewVirtualizationStates()
        {
            return IsWindows();
        }

        /// <summary>
        ///     Returns whether this VM support ballooning.
        ///     Real VMs support ballooning if tools are installed on a balloonable OS.
        ///     For templates we cannot tell whether tools are installed, so ballooning is
        ///     supported if and only if dynamic min != static_max (CA-34258/CA-34260).
        /// </summary>
        public bool SupportsBallooning()
        {
            if (Connection == null)
                return false;

            if (is_a_template)
                return memory_dynamic_min != memory_static_max;

            var otherKey = Connection.Resolve(guest_metrics)?.other;
            return otherKey != null && otherKey.ContainsKey("feature-balloon");
        }

        /// <summary>
        ///     Whether the VM uses ballooning (has different setting of dynamic_max and static_max)
        /// </summary>
        public bool UsesBallooning()
        {
            return memory_dynamic_max != memory_static_max && SupportsBallooning();
        }

        /// <summary>
        ///     Whether the VM should be shown to the user in the GUI.
        /// </summary>
        public override bool Show(bool showHiddenVMs)
        {
            if (InternalTemplate())
                return false;

            if (name_label.StartsWith(Helpers.GuiTempObjectPrefix))
                return false;

            if (showHiddenVMs)
                return true;

            return !IsHidden();
        }

        /// <summary>
        ///     Returns whether the other_config.HideFromXenCenter flag is set to true.
        /// </summary>
        public override bool IsHidden()
        {
            return BoolKey(other_config, HIDE_FROM_XENCENTER);
        }

        public bool HasNoDisksAndNoLocalCD()
        {
            if (Connection == null)
                return false;
            foreach (var vbd in Connection.ResolveAll(VBDs))
            {
                if (vbd.type == vbd_type.Disk) return false; // we have a disk :(

                var vdi = Connection.Resolve(vbd.VDI);
                if (vdi == null)
                    continue;
                var sr = Connection.Resolve(vdi.SR);
                if (sr == null || sr.shared)
                    continue;
                return false; // we have a shared cd
            }

            return true; // we have no disks hooray!!
        }

        public bool IsP2V()
        {
            return other_config != null && other_config.ContainsKey(P2V_SOURCE_MACHINE) &&
                   other_config.ContainsKey(P2V_IMPORT_DATE);
        }

        public VmTemplateType TemplateType()
        {
            if (!is_a_template)
                return VmTemplateType.NoTemplate;

            if (is_snapshot_from_vmpp)
                return VmTemplateType.SnapshotFromVmpp;

            if (is_a_snapshot)
                return VmTemplateType.Snapshot;

            if (!DefaultTemplate())
                return VmTemplateType.Custom;

            var os = name_label.ToLowerInvariant();

            if (os.Contains("citrix"))
                return VmTemplateType.Citrix;

            if (os.Contains("debian"))
                return VmTemplateType.Debian;

            if (os.Contains("gooroom"))
                return VmTemplateType.Gooroom;

            if (os.Contains("rocky"))
                return VmTemplateType.Rocky;

            if (os.Contains("centos"))
                return VmTemplateType.Centos;

            if (os.Contains("linx"))
                return VmTemplateType.Linx;

            if (os.Contains("red hat"))
                return VmTemplateType.RedHat;

            if (os.Contains("oracle"))
                return VmTemplateType.Oracle;

            if (os.Contains("suse"))
                return VmTemplateType.Suse;

            if (os.Contains("scientific"))
                return VmTemplateType.SciLinux;

            if (os.Contains("legacy windows"))
                return VmTemplateType.LegacyWindows;

            if (os.Contains("windows") && os.Contains("server"))
                return VmTemplateType.WindowsServer;

            if (os.Contains("windows"))
                return VmTemplateType.Windows;

            if (os.Contains("ubuntu"))
                return VmTemplateType.Ubuntu;

            if (os.Contains("yinhe"))
                return VmTemplateType.YinheKylin;

            if (os.Contains("kylin"))
                return VmTemplateType.NeoKylin;

            if (os.Contains("asianux"))
                return VmTemplateType.Asianux;

            if (os.Contains("turbo"))
                return VmTemplateType.Turbo;

            if (os.Contains("solaris"))
                return VmTemplateType.Solaris;

            if (os.Contains("coreos"))
                return VmTemplateType.CoreOS;

            return VmTemplateType.Misc;
        }

        public VmDescriptionType DescriptionType()
        {
            var templateType = TemplateType();

            switch (templateType)
            {
                case VmTemplateType.NoTemplate:
                case VmTemplateType.Custom:
                case VmTemplateType.Snapshot:
                case VmTemplateType.SnapshotFromVmpp:
                    return VmDescriptionType.ReadWrite;

                case VmTemplateType.Misc:
                    return VmDescriptionType.ReadOnly;

                default:
                    return VmDescriptionType.None;
            }
        }

        public override string Description()
        {
            // Don't i18n this
            if (IsP2V() && name_description.StartsWith("VM imported from physical machine"))
                return "";

            if (DescriptionType() == VmDescriptionType.ReadOnly)
                return FriendlyNameManager.GetFriendlyName("VM.TemplateDescription-" + name_label) ?? name_description;

            //if this assertion fails it means the code calling this property
            //should be checking beforehand what the DescriptionType is
            Debug.Assert(DescriptionType() != VmDescriptionType.None);

            return name_description;
        }

        public string P2V_SourceMachine()
        {
            return other_config != null && other_config.ContainsKey(P2V_SOURCE_MACHINE)
                ? other_config[P2V_SOURCE_MACHINE]
                : "";
        }

        public DateTime P2V_ImportDate()
        {
            if (other_config == null || !other_config.ContainsKey(P2V_IMPORT_DATE))
                return DateTime.MinValue;

            var importDate = other_config[P2V_IMPORT_DATE];
            return Util.TryParseIso8601DateTime(importDate, out var result) ? result : DateTime.MinValue;
        }

        public string GetOSName()
        {
            var guestMetrics = Connection.Resolve(guest_metrics);

            if (guestMetrics?.os_version == null)
                return Messages.UNKNOWN;

            if (!guestMetrics.os_version.ContainsKey("name"))
                return Messages.UNKNOWN;

            var osName = guestMetrics.os_version["name"];

            // This hack is to make the windows names look nicer
            var index = osName.IndexOf("|");
            if (index >= 1)
                osName = osName.Substring(0, index);

            // CA-9631: conform to MS trademark guidelines
            if (osName.StartsWith("Microsoft®"))
            {
                if (osName != "Microsoft®")
                    osName = osName.Substring(10).Trim();
            }
            else if (osName.StartsWith("Microsoft"))
            {
                if (osName != "Microsoft")
                    osName = osName.Substring(9).Trim();
            }

            if (osName == "")
                return Messages.UNKNOWN;
            return osName;
        }

        /// <summary>
        ///     Gets the time this VM started, in server time, UTC.  Returns DateTime.MinValue if there are no VM_metrics
        ///     to read.
        /// </summary>
        public DateTime GetStartTime()
        {
            var metrics = Connection.Resolve(this.metrics);
            if (metrics == null)
                return DateTime.MinValue;

            return metrics.start_time;
        }

        public PrettyTimeSpan RunningTime()
        {
            if (power_state != vm_power_state.Running &&
                power_state != vm_power_state.Paused &&
                power_state != vm_power_state.Suspended)
                return null;

            var startTime = GetStartTime();
            if (startTime == Epoch || startTime == DateTime.MinValue)
                return null;
            return new PrettyTimeSpan(DateTime.UtcNow - startTime - Connection.ServerTimeOffset);
        }

        /// <summary>
        ///     Returns DateTime.MinValue if the date is not present in other_config.
        /// </summary>
        public DateTime LastShutdownTime()
        {
            return other_config.ContainsKey("last_shutdown_time") &&
                   Util.TryParseIso8601DateTime(other_config["last_shutdown_time"], out var result)
                ? result
                : DateTime.MinValue;
        }

        /// <summary>
        ///     An enum-ified version of ha_restart_priority: use this one instead.
        ///     NB setting this property does not change ha-always-run.
        /// </summary>
        public HaRestartPriority HARestartPriority()
        {
            return StringToPriority(ha_restart_priority);
        }

        public override string NameWithLocation()
        {
            if (Connection != null)
            {
                if (IsRealVm()) return base.NameWithLocation();

                if (is_a_snapshot)
                {
                    var snapshotOf = Connection.Resolve(snapshot_of);
                    if (snapshotOf == null)
                        return base.NameWithLocation();

                    return string.Format(Messages.SNAPSHOT_OF_TITLE, Name(), snapshotOf.Name(), LocationString());
                }

                if (is_a_template)
                {
                    if (Helpers.IsPool(Connection))
                        return string.Format(Messages.OBJECT_IN_POOL, Name(), Connection.Name);

                    return string.Format(Messages.OBJECT_ON_SERVER, Name(), Connection.Name);
                }
            }

            return base.NameWithLocation();
        }

        internal override string LocationString()
        {
            var server = Home();
            if (server != null)
                return string.Format(Messages.ON_SERVER, server);

            var pool = Helpers.GetPool(Connection);
            if (pool != null)
                return string.Format(Messages.IN_POOL, pool);

            return string.Empty;
        }

        public static List<HaRestartPriority> GetAvailableRestartPriorities(IXenConnection connection)
        {
            var restartPriorities = new List<HaRestartPriority>();
            restartPriorities.Add(HaRestartPriority.Restart);
            restartPriorities.Add(HaRestartPriority.BestEffort);
            restartPriorities.Add(HaRestartPriority.DoNotRestart);
            return restartPriorities;
        }

        /// <summary>
        ///     Returns true if VM's restart priority is AlwaysRestart or AlwaysRestartHighPriority.
        /// </summary>
        public bool HaPriorityIsRestart()
        {
            return HaPriorityIsRestart(Connection, HARestartPriority());
        }

        public static bool HaPriorityIsRestart(IXenConnection connection, HaRestartPriority haRestartPriority)
        {
            return haRestartPriority == HaRestartPriority.Restart;
        }

        public static HaRestartPriority HaHighestProtectionAvailable(IXenConnection connection)
        {
            return HaRestartPriority.Restart;
        }

        /// <summary>
        ///     Parses a HA_Restart_Priority into a string the server understands.
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        internal static string PriorityToString(HaRestartPriority priority)
        {
            switch (priority)
            {
                case HaRestartPriority.AlwaysRestartHighPriority:
                    return RESTART_PRIORITY_ALWAYS_RESTART_HIGH_PRIORITY;
                case HaRestartPriority.AlwaysRestart:
                    return RESTART_PRIORITY_ALWAYS_RESTART;
                case HaRestartPriority.Restart:
                    return RESTART_PRIORITY_RESTART;
                case HaRestartPriority.BestEffort:
                    return RESTART_PRIORITY_BEST_EFFORT;
                default:
                    return RESTART_PRIORITY_DO_NOT_RESTART;
            }
        }

        internal static HaRestartPriority StringToPriority(string priority)
        {
            switch (priority)
            {
                case RESTART_PRIORITY_ALWAYS_RESTART_HIGH_PRIORITY:
                    return HaRestartPriority.AlwaysRestartHighPriority;
                case RESTART_PRIORITY_RESTART:
                    return HaRestartPriority.Restart;
                case RESTART_PRIORITY_DO_NOT_RESTART:
                    return HaRestartPriority.DoNotRestart;
                case RESTART_PRIORITY_BEST_EFFORT:
                    return HaRestartPriority.BestEffort;
                default:
                    return HaRestartPriority.AlwaysRestart;
            }
        }

        /// <summary>
        ///     Whether HA is capable of restarting this VM (i.e. the VM is not a template or control domain).
        /// </summary>
        public bool HaCanProtect(bool showHiddenVMs)
        {
            return IsRealVm() && Show(showHiddenVMs);
        }

        /// <summary>
        ///     True if this VM's ha_restart_priority is not "Do not restart" and its pool has ha_enabled true.
        /// </summary>
        public bool HAIsProtected()
        {
            if (Connection == null)
                return false;
            var myPool = Helpers.GetPoolOfOne(Connection);
            if (myPool == null)
                return false;
            return myPool.ha_enabled && HARestartPriority() != HaRestartPriority.DoNotRestart;
        }

        /// <summary>
        ///     Calls set_ha_restart_priority
        /// </summary>
        /// <param name="priority"></param>
        public static void SetHaRestartPriority(Session session, VM vm, HaRestartPriority priority)
        {
            set_ha_restart_priority(session, vm.opaque_ref, PriorityToString(priority));
        }

        public bool AnyDiskFastClonable()
        {
            if (Connection == null)
                return false;
            foreach (var vbd in Connection.ResolveAll(VBDs))
            {
                if (vbd.type != vbd_type.Disk)
                    continue;

                var vdi = Connection.Resolve(vbd.VDI);
                if (vdi == null)
                    continue;

                var sr = Connection.Resolve(vdi.SR);
                if (sr == null)
                    continue;

                var sm = SM.GetByType(Connection, sr.type);
                if (sm == null)
                    continue;

                if (Array.IndexOf(sm.capabilities, "VDI_CLONE") != -1)
                    return true;
            }

            return false;
        }

        public bool HasAtLeastOneDisk()
        {
            if (Connection == null)
                return false;
            foreach (var vbd in Connection.ResolveAll(VBDs))
            {
                if (vbd.type != vbd_type.Disk)
                    continue;

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Checks whether the VM is the dom0 (the flag is_control_domain may also apply to other control domains)
        /// </summary>
        public bool IsControlDomainZero(out Host host)
        {
            host = null;
            if (!is_control_domain)
                return false;

            host = Connection.Resolve(resident_on);
            if (host == null)
                return false;

            if (!Helper.IsNullOrEmptyOpaqueRef(host.control_domain))
                return host.control_domain == opaque_ref;

            var vms = Connection.ResolveAll(host.resident_VMs);
            var first = vms.FirstOrDefault(vm => vm.is_control_domain && vm.domid == 0);
            return first != null && first.opaque_ref == opaque_ref;
        }

        public bool IsSrDriverDomain(out SR sr)
        {
            sr = null;

            if (!is_control_domain || IsControlDomainZero(out _))
                return false;

            foreach (var pbd in Connection.Cache.PBDs)
                if (pbd != null &&
                    pbd.other_config.TryGetValue("storage_driver_domain", out var vmRef) &&
                    vmRef == opaque_ref)
                {
                    sr = Connection.Resolve(pbd.SR);
                    if (sr != null)
                        return true;
                }

            return false;
        }

        public bool IsRealVm()
        {
            return !is_a_snapshot && !is_a_template && !is_control_domain;
        }

        public XmlNode ProvisionXml()
        {
            try
            {
                var xml = Get(other_config, "disks");
                if (string.IsNullOrEmpty(xml))
                    return null;

                var doc = new XmlDocument();
                doc.LoadXml(xml);
                return doc.FirstChild;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool InstantTemplate()
        {
            return BoolKey(other_config, "instant");
        }

        public bool IsConversionVM()
        {
            return IsRealVm() && BoolKey(other_config, "conversionvm");
        }

        public override string ToString()
        {
            return name_label;
        }

        /// <summary>
        ///     The name label of the VM's affinity server, or None if it is not set
        ///     (This is what the UI calls the "home server", but is not the same as VM.Home).
        /// </summary>
        public string AffinityServerString()
        {
            var host = Connection.Resolve(affinity);
            if (host == null)
                return Messages.NONE;

            return host.Name();
        }

        public bool HasProvisionXML()
        {
            return other_config != null && other_config.ContainsKey("disks");
        }

        public bool BiosStringsCopied()
        {
            if (DefaultTemplate()) return false;

            if (bios_strings.Count == 0) return false;

            var value = bios_strings.ContainsKey("bios-vendor") && bios_strings["bios-vendor"] == "Xen"
                                                                && bios_strings.ContainsKey("system-manufacturer") &&
                                                                bios_strings["system-manufacturer"] == "Xen";

            return !value;
        }

        public bool HasCD()
        {
            foreach (var vbd in Connection.ResolveAll(VBDs))
                if (vbd.IsCDROM())
                    return true;
            return false;
        }

        public bool HasVGPUs()
        {
            return VGPUs != null && VGPUs.Count > 0;
        }

        public bool HasGPUPassthrough()
        {
            if (VGPUs != null && VGPUs.Count > 0)
            {
                var vGPUs = Connection.ResolveAll(VGPUs);
                return vGPUs.Any(vGPU => vGPU != null && vGPU.IsPassthrough());
            }

            return false;
        }

        public virtual IEnumerable<SR> SRs()
        {
            var vbds = Connection.ResolveAll(VBDs);
            foreach (var vbd in vbds)
            {
                var vdi = vbd?.Connection.Resolve(vbd.VDI);
                if (vdi != null) yield return vdi.Connection.Resolve(vdi.SR);
            }
        }

        public bool IsAssignedToVapp()
        {
            //on pre-boston servers appliance is null
            return appliance != null && appliance.opaque_ref != null &&
                   appliance.opaque_ref.StartsWith("OpaqueRef:") &&
                   appliance.opaque_ref != "OpaqueRef:NULL";
        }

        public long GetCoresPerSocket()
        {
            if (platform != null && platform.ContainsKey("cores-per-socket"))
            {
                long coresPerSocket;
                return long.TryParse(platform["cores-per-socket"], out coresPerSocket)
                    ? coresPerSocket
                    : DEFAULT_CORES_PER_SOCKET;
            }

            return DEFAULT_CORES_PER_SOCKET;
        }

        public void SetCoresPerSocket(long value)
        {
            platform = SetDictionaryKey(platform, "cores-per-socket", value.ToString());
        }

        public long MaxCoresPerSocket()
        {
            var homeServer = Home();
            if (homeServer != null)
                return homeServer.CoresPerSocket();

            var maxCoresPerSocket = 0;
            foreach (var host in Connection.Cache.Hosts)
            {
                var coresPerSocket = host.CoresPerSocket();
                if (coresPerSocket > maxCoresPerSocket)
                    maxCoresPerSocket = coresPerSocket;
            }

            return maxCoresPerSocket;
        }

        public bool HasValidVCPUConfiguration()
        {
            return ValidVCPUConfiguration(VCPUs_max, GetCoresPerSocket()) == "";
        }

        public static string ValidVCPUConfiguration(long noOfVCPUs, long coresPerSocket)
        {
            if (coresPerSocket > 0)
            {
                if (noOfVCPUs % coresPerSocket != 0)
                    return Messages.CPU_TOPOLOGY_INVALID_REASON_MULTIPLE;
                if (noOfVCPUs / coresPerSocket > MAX_SOCKETS)
                    return Messages.CPU_TOPOLOGY_INVALID_REASON_SOCKETS;
            }

            return "";
        }

        public string Topology()
        {
            var cores = GetCoresPerSocket();
            var sockets = ValidVCPUConfiguration(VCPUs_max, cores) == "" ? VCPUs_max / cores : 0;
            return GetTopology(sockets, cores);
        }

        public static string GetTopology(long sockets, long cores)
        {
            if (sockets == 0) // invalid cores value
                return cores == 1
                    ? string.Format(Messages.CPU_TOPOLOGY_STRING_INVALID_VALUE_1)
                    : string.Format(Messages.CPU_TOPOLOGY_STRING_INVALID_VALUE, cores);
            if (sockets == 1 && cores == 1)
                return Messages.CPU_TOPOLOGY_STRING_1_SOCKET_1_CORE;
            if (sockets == 1)
                return string.Format(Messages.CPU_TOPOLOGY_STRING_1_SOCKET_N_CORE, cores);
            if (cores == 1)
                return string.Format(Messages.CPU_TOPOLOGY_STRING_N_SOCKET_1_CORE, sockets);
            return string.Format(Messages.CPU_TOPOLOGY_STRING_N_SOCKET_N_CORE, sockets, cores);
        }

        public bool CanBeEnlightened()
        {
            return other_config.ContainsKey("xscontainer-monitor");
        }

        public bool IsEnlightened()
        {
            var v = Get(other_config, "xscontainer-monitor");
            return v != null && v.ToLower() == "true";
        }

        public VDI CloudConfigDrive()
        {
            var vbds = Connection.ResolveAll(VBDs);
            return vbds.Select(vbd => Connection.Resolve(vbd.VDI))
                .FirstOrDefault(vdi => vdi != null && vdi.IsCloudConfigDrive());
        }

        public bool CanHaveCloudConfigDrive()
        {
            if (is_a_template && TemplateType() == VmTemplateType.CoreOS)
                return true;
            //otherwise check if it has a config drive
            return CloudConfigDrive() != null;
        }

        public VM_Docker_Info DockerInfo()
        {
            var xml = Get(other_config, "docker_info");
            if (string.IsNullOrEmpty(xml))
                return null;
            var info = new VM_Docker_Info(xml);
            return info;
        }

        public VM_Docker_Version DockerVersion()
        {
            var xml = Get(other_config, "docker_version");
            if (string.IsNullOrEmpty(xml))
                return null;
            var info = new VM_Docker_Version(xml);
            return info;
        }

        public bool ReadCachingEnabled()
        {
            return ReadCachingVDIs().Count > 0;
        }

        /// <summary>
        ///     Return the list of VDIs that have Read Caching enabled
        /// </summary>
        public List<VDI> ReadCachingVDIs()
        {
            var readCachingVdis = new List<VDI>();
            foreach (var vbd in Connection.ResolveAll(VBDs).Where(vbd => vbd != null && vbd.currently_attached))
            {
                var vdi = Connection.Resolve(vbd.VDI);
                var residentHost = Connection.Resolve(resident_on);
                if (vdi != null && residentHost != null && vdi.ReadCachingEnabled(residentHost))
                    readCachingVdis.Add(vdi);
            }

            return readCachingVdis;
        }

        public string ReadCachingDisabledReason()
        {
            // The code in VDI.ReadCachingDisabledReason returns the first matching reason from the list
            // (LICENSE_RESTRICTION, SR_NOT_SUPPORTED, NO_RO_IMAGE, SR_OVERRIDE). In the case that there
            // are several VDIs with different reasons, this function returns the last matching reason,
            // because that is the VDI that is nearest to being read-cachable in some sense. As the reasons
            // are stored in an enum, we can just use greater-than to find the last reason 
            var ans = VDI.ReadCachingDisabledReasonCode.UNKNOWN;
            foreach (var vbd in Connection.ResolveAll(VBDs).Where(vbd => vbd != null && vbd.currently_attached))
            {
                var vdi = Connection.Resolve(vbd.VDI);
                var residentHost = Connection.Resolve(resident_on);
                if (vdi != null && residentHost != null && !vdi.ReadCachingEnabled(residentHost))
                {
                    var reason = vdi.ReadCachingDisabledReason(residentHost);
                    if (reason > ans)
                        ans = reason;
                }
            }

            switch (ans)
            {
                case VDI.ReadCachingDisabledReasonCode.LICENSE_RESTRICTION:
                    if (Helpers.FeatureForbidden(Connection, Host.RestrictReadCaching))
                        return Messages.VM_READ_CACHING_DISABLED_REASON_LICENSE;
                    return Messages.VM_READ_CACHING_DISABLED_REASON_PREV_LICENSE;
                case VDI.ReadCachingDisabledReasonCode.SR_NOT_SUPPORTED:
                    return Messages.VM_READ_CACHING_DISABLED_REASON_SR_TYPE;
                case VDI.ReadCachingDisabledReasonCode.NO_RO_IMAGE:
                    return Messages.VM_READ_CACHING_DISABLED_REASON_NO_RO_IMAGE;
                case VDI.ReadCachingDisabledReasonCode.SR_OVERRIDE:
                    return Messages.VM_READ_CACHING_DISABLED_REASON_TURNED_OFF;
                default:
                    // should only happen transiently
                    return null;
            }
        }

        /// <summary>
        ///     Whether the VM can be moved inside the pool (vdi copy + destroy)
        /// </summary>
        public bool CanBeMoved()
        {
            if (SRs().Any(sr => sr != null && sr.HBALunPerVDI()))
                return false;

            if (!is_a_template && !Locked && allowed_operations != null &&
                allowed_operations.Contains(vm_operations.export) &&
                power_state != vm_power_state.Suspended)
                return Connection.ResolveAll(VBDs).Find(v => v.GetIsOwner()) != null;
            return false;
        }

        /// <summary>
        ///     Returns whether this is a Windows VM by checking the distro value in the
        ///     guest_metrics before falling back to the viridian flag. The result may not be
        ///     correct at all times (a Linux distro can be detected if the guest agent is
        ///     running on the VM). It is more reliable if the VM has already booted once, and
        ///     also works for the "Other Install Media" template and unbooted VMs made from it.
        /// </summary>
        public bool IsWindows()
        {
            var gm = Connection.Resolve(guest_metrics);
            if (CanTreatAsNonWindows(gm))
                return false;

            var gmFromLastBootedRecord = GuestMetricsFromLastBootedRecord();
            if (CanTreatAsNonWindows(gmFromLastBootedRecord))
                return false;

            return
                IsHVM() && BoolKey(platform, "viridian");
        }

        private static bool CanTreatAsNonWindows(VM_guest_metrics guestMetrics)
        {
            if (guestMetrics?.os_version == null)
                return false;

            if (guestMetrics.os_version.TryGetValue("distro", out var distro) &&
                !string.IsNullOrEmpty(distro) && LinuxDistros.Contains(distro.ToLowerInvariant()))
                return true;

            if (guestMetrics.os_version.TryGetValue("uname", out var uname) &&
                !string.IsNullOrEmpty(uname) && uname.ToLowerInvariant().Contains("netscaler"))
                return true;

            return false;
        }

        private VM_guest_metrics GuestMetricsFromLastBootedRecord()
        {
            if (!string.IsNullOrEmpty(last_booted_record))
            {
                var regex = new Regex(
                    "'guest_metrics' +'(OpaqueRef:[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})'");

                var v = regex.Match(last_booted_record);
                if (v.Success)
                {
                    var s = v.Groups[1].ToString();
                    return Connection.Resolve(new XenRef<VM_guest_metrics>(s));
                }
            }

            return null;
        }

        public bool WindowsUpdateCapable()
        {
            return has_vendor_device && IsWindows();
        }

        /// <summary>
        ///     Returns the VM IP address for SSH login.
        /// </summary>
        public string IPAddressForSSH()
        {
            var ipAddresses = new List<string>();

            if (!is_control_domain) //vm
            {
                var vifs = Connection.ResolveAll(VIFs);
                vifs.Sort();

                foreach (var vif in vifs)
                {
                    if (!vif.currently_attached)
                        continue;

                    var network = vif.Connection.Resolve(vif.network);
                    if (network != null && network.IsGuestInstallerNetwork())
                        continue;

                    ipAddresses.AddRange(vif.IPAddresses());
                }
            }
            else //control domain
            {
                var pifList = new List<PIF>(Connection.Cache.PIFs);
                pifList.Sort(); // This sort ensures that the primary PIF comes before other management PIFs

                foreach (var pif in pifList)
                {
                    if (pif.host.opaque_ref != resident_on.opaque_ref || !pif.currently_attached)
                        continue;

                    if (pif.IsManagementInterface(false)) ipAddresses.Add(pif.IP);
                }
            }

            //find first IPv4 address and return it - we would use it if there is one
            IPAddress addr;
            foreach (var addrString in ipAddresses)
                if (IPAddress.TryParse(addrString, out addr) && addr.AddressFamily == AddressFamily.InterNetwork)
                    return addrString;

            //return the first address (this will not be IPv4)
            return ipAddresses.FirstOrDefault() ?? string.Empty;
        }

        public bool HciWarnBeforeShutdown()
        {
            return other_config != null && other_config.ContainsKey("hci-warn-before-shutdown");
        }

        public bool SupportsVcpuHotplug()
        {
            return !IsWindows() && !Helpers.FeatureForbidden(Connection, Host.RestrictVcpuHotplug);
        }

        public PVS_proxy PvsProxy()
        {
            return Connection.Cache.PVS_proxies.FirstOrDefault(p =>
            {
                var vm = p.VM();
                return vm != null && vm.Equals(this);
            }); // null if none
        }

        public bool UsingUpstreamQemu()
        {
            return platform != null &&
                   platform.ContainsKey("device-model") &&
                   platform["device-model"] == "qemu-upstream-compat";
        }

        /// <summary>
        ///     Whether the VM's boot mode can be changed. A VM's boot mode cannot be changed once the VM has been started.
        /// </summary>
        public bool CanChangeBootMode()
        {
            return last_boot_CPU_flags == null || last_boot_CPU_flags.Count == 0;
        }

        public bool CanAddVtpm(out string cannotReason)
        {
            cannotReason = null;

            if (Helpers.XapiEqualOrGreater_23_11_0(Connection))
            {
                if (allowed_operations.Contains(vm_operations.create_vtpm))
                    return true;

                cannotReason = Messages.VTPM_OPERATION_DISALLOWED_ADD;
                return false;
            }

            if (VTPMs.Count >= MAX_ALLOWED_VTPMS)
            {
                cannotReason = string.Format(Messages.VTPM_MAX_REACHED, MAX_ALLOWED_VTPMS);
                return false;
            }

            if (power_state != vm_power_state.Halted)
            {
                cannotReason = Messages.VTPM_POWER_STATE_WRONG_ATTACH;
                return false;
            }

            return true;
        }

        public bool CanRemoveVtpm(out string cannotReason)
        {
            cannotReason = null;

            if (power_state != vm_power_state.Halted)
            {
                cannotReason = Messages.VTPM_POWER_STATE_WRONG_REMOVE;
                return false;
            }

            return true;
        }

        #region Boot Mode

        public bool IsDefaultBootModeUefi()
        {
            var firmware = Get(HVM_boot_params, "firmware")?.Trim().ToLower();
            return firmware == "uefi";
        }

        public string GetSecureBootMode()
        {
            return Get(platform, "secureboot")?.Trim().ToLower();
        }

        public bool SupportsUefiBoot()
        {
            return GetRecommendationByField("supports-uefi") == "yes";
        }

        public bool SupportsSecureUefiBoot()
        {
            return GetRecommendationByField("supports-secure-boot") == "yes";
        }

        private string GetRecommendationByField(string fieldName)
        {
            var xd = GetRecommendations();

            var xn = xd?.SelectSingleNode(@"restrictions/restriction[@field='" + fieldName + "']");

            return xn?.Attributes?["value"]?.Value ?? string.Empty;
        }

        #endregion
    }

    public struct VMStartupOptions
    {
        public long Order, StartDelay;
        public VM.HaRestartPriority? HaRestartPriority;

        public VMStartupOptions(long order, long startDelay)
        {
            Order = order;
            StartDelay = startDelay;
            HaRestartPriority = null;
        }

        public VMStartupOptions(long order, long startDelay, VM.HaRestartPriority haRestartPriority)
        {
            Order = order;
            StartDelay = startDelay;
            HaRestartPriority = haRestartPriority;
        }
    }

    public enum VmBootMode
    {
        Bios,
        Uefi,
        SecureUefi
    }

    public static class BootModeExtensions
    {
        public static string StringOf(this VmBootMode mode)
        {
            switch (mode)
            {
                case VmBootMode.Bios:
                    return Messages.BIOS_BOOT;
                case VmBootMode.Uefi:
                    return Messages.UEFI_BOOT;
                case VmBootMode.SecureUefi:
                    return Messages.UEFI_SECURE_BOOT;
                default:
                    return Messages.UNAVAILABLE;
            }
        }
    }
}