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
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Xml;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


namespace XenAPI
{
    public partial class VM : IComparable<VM>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
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
        public const long MAX_SOCKETS = 16;  // current hard limit in Xen: CA-198276
        // CP-41825: > 32 vCPUs is only supported for trusted VMs
        public const long MAX_VCPUS_FOR_NON_TRUSTED_VMS = 32; 

        private XmlDocument xdRecommendations;
        public const int MAX_ALLOWED_VTPMS = 1;

        public int MaxVCPUsAllowed()
        {
            var xd = GetRecommendations();

            var xn = xd?.SelectSingleNode(@"restrictions/restriction[@field='vcpus-max']");
            if (int.TryParse(xn?.Attributes?["max"]?.Value, out var result))
                return result;

            return DEFAULT_NUM_VCPUS_ALLOWED;
        }

        public int MinVCPUs()
        {
            var xd = GetRecommendations();

            var xn = xd?.SelectSingleNode(@"restrictions/restriction[@field='vcpus-min']");
            if (int.TryParse(xn?.Attributes?["min"]?.Value, out var result))
                return result;

            return 1;
        }

        public bool IsRunning()
        {
            return power_state == vm_power_state.Running;
        }

        /// <summary>
        /// Returns true if the VM's pool has HA enabled and the VM has a saved restart priority other than DoNotRestart.
        /// Does not take account of ha-always-run.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public bool HasSavedRestartPriority()
        {
            var pool = Helpers.GetPoolOfOne(Connection);
            return pool != null && pool.ha_enabled && !string.IsNullOrEmpty(ha_restart_priority);
        }

        /// <summary>
        /// Get the given VM's home, i.e. the host under which we are going to display it.  May return null, if this VM should live
        /// at the pool level. For a normal VM, we look at (1) where it's running; (2) where its storage forces it to run;
        /// (3) what its affinity is (its requested but not guaranteed host).
        /// </summary>
        public virtual Host Home()
        {
            if (is_a_snapshot)  // Snapshots have the same "home" as their VM. This is necessary to make a pool-server-VM-snapshot tree (CA-76273).
            {
                var from = Connection.Resolve(snapshot_of);
                return from?.Home(); // "from" can be null if VM has been deleted
            }

            if (is_a_template)  // Templates (apart from snapshots) don't have a "home", even if their affinity is set CA-36286
                return null;

            if (power_state == vm_power_state.Running)
                return Connection.Resolve(resident_on);

            var storage_host = GetStorageHost(false);
            if (storage_host != null)
                return storage_host;

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
            else
            {
                return null;
            }
        }

        public SR FindVMCDROMSR()
        {
            var vbd = FindVMCDROM();
            if (vbd != null)
            {
                var vdi = vbd.Connection.Resolve(vbd.VDI);
                if (vdi != null)
                {
                    return vdi.Connection.Resolve(vdi.SR);
                }
            }
            return null;
        }

        public override string Name()
        {
            const string CONTROL_DOMAIN = "Control domain on host: ";
            if (name_label != null && name_label.StartsWith(CONTROL_DOMAIN))
            {
                var hostName = name_label.Substring(CONTROL_DOMAIN.Length);
                return string.Format(Messages.CONTROL_DOM_ON_HOST, hostName);
            }
            return name_label;
        }

        public long MaxMemAllowed()
        {
            var xd = GetRecommendations();

            var xn = xd?.SelectSingleNode(@"restrictions/restriction[@field='memory-static-max']");
            if (long.TryParse(xn?.Attributes?["max"]?.Value, out var result))
                return result;

            return DEFAULT_MEM_ALLOWED;
        }

        public int MaxVIFsAllowed()
        {
            var xd = GetRecommendations();

            var xn = xd?.SelectSingleNode(@"restrictions/restriction[@property='number-of-vifs']");
            if (int.TryParse(xn?.Attributes?["max"]?.Value, out var result))
                return result;

            return DEFAULT_NUM_VIFS_ALLOWED;
        }

        public int MaxVBDsAllowed()
        {
            var xd = GetRecommendations();

            var xn = xd?.SelectSingleNode(@"restrictions/restriction[@property='number-of-vbds']");
            if (int.TryParse(xn?.Attributes?["max"]?.Value, out var result))
                return result;

            return DEFAULT_NUM_VBDS_ALLOWED;
        }

        private XmlDocument GetRecommendations()
        {
            if (xdRecommendations != null)
                return xdRecommendations;

            if (string.IsNullOrEmpty(recommendations))
                return null;

            xdRecommendations = new XmlDocument();

            try
            {
                xdRecommendations.LoadXml(recommendations);
            }
            catch
            {
                xdRecommendations = null;
            }

            return xdRecommendations;
        }

        public Host GetStorageHost(bool ignoreCDs)
        {

            foreach (var TheVBD in Connection.ResolveAll(VBDs))
            {

                if (ignoreCDs && TheVBD.type == vbd_type.CD)
                    continue;
                var TheVDI = Connection.Resolve(TheVBD.VDI);

                if (TheVDI == null || !TheVDI.Show(true))
                    continue;
                var TheSR = Connection.Resolve(TheVDI.SR);
                if (TheSR == null)
                    continue;
                var host = TheSR.GetStorageHost();
                if (host != null)
                {
                    return host;
                }
            }

            return null;
        }

        /// <remarks>
        /// Default on server is CD - disk then optical
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
                if (int.TryParse(VCPUs_params["weight"], out weight)) // if we cant parse it we assume its because it is too large, obviously if it isnt a number (ie a string) then we will still go to the else
                    return weight > 0 ? weight : 1; // because we perform a log on what is returned from this the weight must always be greater than 0
                else
                    return 65536; // could not parse number, assume max
            }
            else
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

        /// <summary>Returns true if
        /// 1) the guest is HVM and
        ///   2a) the allow-gpu-passthrough restriction is absent or
        ///   2b) the allow-gpu-passthrough restriction is non-zero
        ///</summary>
        public bool CanHaveGpu()
        {
            if (!IsHVM())
                return false;

            var xd = GetRecommendations();

            var xn = xd?.SelectSingleNode(@"restrictions/restriction[@field='allow-gpu-passthrough']");
            if (int.TryParse(xn?.Attributes?["value"]?.Value, out var result))
                return result != 0;

            return true;
        }

        public bool HasSriovRecommendation()
        {
            var xd = GetRecommendations();

            var xn = xd?.SelectSingleNode(@"restrictions/restriction[@field='allow-network-sriov']");
            if (int.TryParse(xn?.Attributes?["value"]?.Value, out var result))
                return result != 0;

            return false;
        }

        public bool HasVendorDeviceRecommendation()
        {
            var xd = GetRecommendations();

            var xn = xd?.SelectSingleNode(@"restrictions/restriction[@field='has-vendor-device']");
            if (bool.TryParse(xn?.Attributes?["value"]?.Value, out var result))
                return result;

            log.Error("Error parsing has-vendor-device on the template.");
            return false;
        }

        /// <summary>Returns true if
        /// 1) the guest is HVM and
        ///   2a) the allow-vgpu restriction is absent or
        ///   2b) the allow-vgpu restriction is non-zero
        ///</summary>
        public bool CanHaveVGpu()
        {
            if (!IsHVM() || !CanHaveGpu())
                return false;

            var xd = GetRecommendations();

            var xn = xd?.SelectSingleNode(@"restrictions/restriction[@field='allow-vgpu']");
            if (int.TryParse(xn?.Attributes?["value"]?.Value, out var result))
                return result != 0;

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
                            if (sr.content_type == SR.Content_Type_ISO)
                            {
                                return Messages.EJECT_YOUR_CD;
                            }
                            else
                            {
                                return Messages.VM_USES_LOCAL_STORAGE;
                            }
                        }
                    }
                }
            }
            return "";
        }

        public decimal GetRecommendedExportSpace(bool showHiddenVMs)
        {
            decimal totalSpace = 0;

            foreach (var vbd in Connection.ResolveAll(VBDs))
            {
                if (!vbd.IsCDROM())
                {
                    var VDI = Connection.Resolve(vbd.VDI);
                    if (VDI != null && VDI.Show(showHiddenVMs))
                    {
                        var TheSR = Connection.Resolve(VDI.SR);
                        if (TheSR != null && !TheSR.IsToolsSR())
                        {
                            totalSpace += VDI.virtual_size;
                        }
                    }
                }
            }
            return totalSpace;
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
                return (myClass - otherClass);
            else
                return base.CompareTo(other);
        }

        /// <summary>
        /// These are the operations that make us show the orange icon for the VM in the tree
        /// and on the Memory tab. It's shorter to add the ones that cause problems.
        /// </summary>
        public static bool is_lifecycle_operation(vm_operations op)
        {
            return op != vm_operations.changing_dynamic_range && op != vm_operations.changing_static_range && op != vm_operations.changing_memory_limits;
        }

        private DateTime startuptime;

        public DateTime GetBodgeStartupTime()
        {
            return startuptime;
        }

        public void SetBodgeStartupTime(DateTime value)
        {
            startuptime = value;
            // This has an impact on the virt state of the VM as we allow a set amount of time for tools to show up before assuming unvirt
            NotifyPropertyChanged("virtualisation_status");
            if (VirtualizationTimer != null)
                VirtualizationTimer.Stop();
            // 2 minutes before we give up plus some breathing space
            VirtualizationTimer = new Timer(182000) {AutoReset = false};
            VirtualizationTimer.Elapsed += VirtualizationTimer_Elapsed;
            VirtualizationTimer.Start();
        }

        void VirtualizationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NotifyPropertyChanged("virtualisation_status");
        }

        private Timer VirtualizationTimer;

        [Flags]
        public enum VirtualisationStatus
        {
            NOT_INSTALLED               = 0,
            UNKNOWN                     = 1,
            PV_DRIVERS_OUT_OF_DATE      = 2,
            IO_DRIVERS_INSTALLED        = 4,
            MANAGEMENT_INSTALLED        = 8,
        };

        public string GetVirtualisationWarningMessages()
        {
            var status = GetVirtualisationStatus(out _);

            if (status.HasFlag(VirtualisationStatus.IO_DRIVERS_INSTALLED) && status.HasFlag(VirtualisationStatus.MANAGEMENT_INSTALLED)
                || status.HasFlag(VirtualisationStatus.UNKNOWN))
                    // calling function shouldn't send us here if tools are, or might be, present: used to assert here but it can sometimes happen (CA-51460)
                    return "";

            if (status.HasFlag(VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE))
            {
                    var guestMetrics = Connection.Resolve(guest_metrics);
                    if (guestMetrics != null
                        && guestMetrics.PV_drivers_version.ContainsKey("major")
                        && guestMetrics.PV_drivers_version.ContainsKey("minor"))
                    {
                        return string.Format(Messages.PV_DRIVERS_OUT_OF_DATE, BrandManager.VmTools,
                            guestMetrics.PV_drivers_version["major"], guestMetrics.PV_drivers_version["minor"]);
                    }
                    
                    return string.Format(Messages.PV_DRIVERS_OUT_OF_DATE_UNKNOWN_VERSION, BrandManager.VmTools);
            }

            return HasNewVirtualisationStates()
                ? Messages.VIRTUALIZATION_STATE_VM_MANAGEMENT_AGENT_NOT_INSTALLED
                : string.Format(Messages.PV_DRIVERS_NOT_INSTALLED, BrandManager.VmTools);
        }

        /// <summary>
        /// Virtualization Status of the VM
        /// </summary>
        /// 
        /// <remarks>
        /// Following states are expected:
        /// 
        /// For Non-Windows VMs and for Windows VMs pre-Dundee:
        ///   0 = Not installed
        ///   1 = Unknown
        ///   2 = Out of date
        ///  12 = Tools installed (Optimized)
        ///  
        /// For Windows VMs on Dundee or higher:
        ///    0 = Not installed
        ///    1 = Unknown
        ///    4 = I/O Optimized
        ///   12 = I/O and Management installed
        /// </remarks>
        public VirtualisationStatus GetVirtualisationStatus(out string friendlyStatus)
        {
            friendlyStatus = Messages.VIRTUALIZATION_UNKNOWN;

            if (Connection == null || power_state != vm_power_state.Running || Connection.Resolve(metrics) == null)
                return VirtualisationStatus.UNKNOWN;

            var vmGuestMetrics = Connection.Resolve(guest_metrics);
            var lessThanTwoMin = (DateTime.UtcNow - GetBodgeStartupTime()).TotalMinutes < 2;

            if (HasNewVirtualisationStates())
            {
                var flags = VirtualisationStatus.NOT_INSTALLED;

                if (vmGuestMetrics != null && vmGuestMetrics.PV_drivers_detected)
                    flags |= VirtualisationStatus.IO_DRIVERS_INSTALLED;

                if (vmGuestMetrics != null && IntKey(vmGuestMetrics.other, "feature-static-ip-setting", 0) != 0)
                    flags |= VirtualisationStatus.MANAGEMENT_INSTALLED;

                if (flags.HasFlag(VirtualisationStatus.IO_DRIVERS_INSTALLED | VirtualisationStatus.MANAGEMENT_INSTALLED))
                    friendlyStatus = Messages.VIRTUALIZATION_STATE_VM_IO_DRIVERS_AND_MANAGEMENT_AGENT_INSTALLED;
                else if (flags.HasFlag(VirtualisationStatus.IO_DRIVERS_INSTALLED))
                    friendlyStatus = Messages.VIRTUALIZATION_STATE_VM_MANAGEMENT_AGENT_NOT_INSTALLED;
                else if (lessThanTwoMin)
                    flags = VirtualisationStatus.UNKNOWN;
                else
                    friendlyStatus = string.Format(Messages.PV_DRIVERS_NOT_INSTALLED, BrandManager.VmTools);

                return flags;
            }

            if (vmGuestMetrics == null || !vmGuestMetrics.PV_drivers_installed())
                if (lessThanTwoMin)
                    return VirtualisationStatus.UNKNOWN;
                else
                {
                    friendlyStatus = string.Format(Messages.PV_DRIVERS_NOT_INSTALLED, BrandManager.VmTools);
                    return VirtualisationStatus.NOT_INSTALLED;
                }

            if (!vmGuestMetrics.PV_drivers_version.TryGetValue("major", out var major))
                major = "0";
            if (!vmGuestMetrics.PV_drivers_version.TryGetValue("minor", out var minor))
                minor = "0";

            if (!vmGuestMetrics.PV_drivers_up_to_date)
            {
                friendlyStatus = string.Format(Messages.VIRTUALIZATION_OUT_OF_DATE, $"{major}.{minor}");
                return VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE;
            }

            friendlyStatus = string.Format(Messages.VIRTUALIZATION_OPTIMIZED, $"{major}.{minor}");
            return VirtualisationStatus.IO_DRIVERS_INSTALLED | VirtualisationStatus.MANAGEMENT_INSTALLED;
        }

        /// <summary>
        /// Is this a Windows VM on Dundee or higher host?
        /// We need to know this, because for those VMs virtualization status is defined differently.
        /// This does not mean new(ly created) VM
        /// </summary>
        public bool HasNewVirtualisationStates()
        {
            return IsWindows();
        }

        /// <summary>
        /// Returns whether this VM support ballooning.
        /// Real VMs support ballooning if tools are installed on a balloonable OS.
        /// For templates we cannot tell whether tools are installed, so ballooning is
        /// supported if and only if dynamic min != static_max (CA-34258/CA-34260).
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
        /// Whether the VM uses ballooning (has different setting of dynamic_max and static_max)
        /// </summary>
        public bool UsesBallooning()
        {
            return memory_dynamic_max != memory_static_max && SupportsBallooning();
        }

        /// <summary>
        /// Whether the VM should be shown to the user in the GUI.
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
        /// Returns whether the other_config.HideFromXenCenter flag is set to true.
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
                if (vbd.type == vbd_type.Disk)
                {
                    return false; // we have a disk :(
                }
                else
                {
                    var vdi = Connection.Resolve(vbd.VDI);
                    if (vdi == null)
                        continue;
                    var sr = Connection.Resolve(vdi.SR);
                    if (sr == null || sr.shared)
                        continue;
                    return false; // we have a shared cd
                }
            }
            return true; // we have no disks hooray!!
        }

        private const string P2V_SOURCE_MACHINE = "p2v_source_machine";
        private const string P2V_IMPORT_DATE = "p2v_import_date";

        public bool IsP2V()
        {
            return other_config != null && other_config.ContainsKey(P2V_SOURCE_MACHINE) && other_config.ContainsKey(P2V_IMPORT_DATE);
        }

        /// <summary>
        /// List of distros that we treat as Linux/Non-Windows (written in the VM.guest_metrics
        /// by the Linux Guest Agent after evaluating xe-linux-distribution)
        /// </summary>
        private static string[] linuxDistros =
        {
            "debian", "rhel", "fedora", "centos", "scientific", "oracle", "sles",
            "lsb", "boot2docker", "freebsd", "ubuntu", "neokylin", "gooroom", "rocky"
        };

        /// <summary>
        /// Sort in the following order:
        /// 1) User Templates
        /// 2) Windows VMs
        /// 3) Other VMs (e.g. Linux . Names in alphabetical order)
        /// 4) Citrix VMs (e.g. XenApp templates)
        /// 5) Misc VMs
        /// 6) Regular snapshots
        /// 7) Snapshots from VMPP (CA-46206)
        /// Last: Hidden VMs (only visible if "Show Hidden Objects" is on: see CA-39036).
        /// </summary>
        public enum VmTemplateType
        {
            NoTemplate = 0,//it's not a template
            Custom = 1,
            Windows = 2,
            WindowsServer = 3,
            LegacyWindows = 4,
            Asianux  = 5,
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
            Count = 25  //bump this if values are added
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

        public enum VmDescriptionType { None, ReadOnly, ReadWrite }

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
            return other_config != null && other_config.ContainsKey(P2V_SOURCE_MACHINE) ? other_config[P2V_SOURCE_MACHINE] : "";
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
            if (guestMetrics == null)
                return Messages.UNKNOWN;

            if (guestMetrics.os_version == null)
                return Messages.UNKNOWN;

            if (!guestMetrics.os_version.ContainsKey("name"))
                return Messages.UNKNOWN;

            var os_name = guestMetrics.os_version["name"];

            // This hack is to make the windows names look nicer
            var index = os_name.IndexOf("|");
            if (index >= 1)
                os_name = os_name.Substring(0, index);

            // CA-9631: conform to MS trademark guidelines
            if (os_name.StartsWith("Microsoft®"))
            {
                if (os_name != "Microsoft®")
                    os_name = os_name.Substring(10).Trim();
            }
            else if (os_name.StartsWith("Microsoft"))
            {
                if (os_name != "Microsoft")
                    os_name = os_name.Substring(9).Trim();
            }

            if (os_name == "")
                return Messages.UNKNOWN;
            else
                return os_name;
        }

        /// <summary>
        /// Gets the time this VM started, in server time, UTC.  Returns DateTime.MinValue if there are no VM_metrics
        /// to read.
        /// </summary>
        public DateTime GetStartTime()
        {
            var metrics = Connection.Resolve(this.metrics);
            if (metrics == null)
                return DateTime.MinValue;

            return metrics.start_time;
        }
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        public PrettyTimeSpan RunningTime()
        {
            if (power_state != vm_power_state.Running &&
                power_state != vm_power_state.Paused &&
                power_state != vm_power_state.Suspended)
            {
                return null;
            }

            var startTime = GetStartTime();
            if (startTime == Epoch || startTime == DateTime.MinValue)
                return null;
            return new PrettyTimeSpan(DateTime.UtcNow - startTime - Connection.ServerTimeOffset);
        }

        /// <summary>
        /// Returns DateTime.MinValue if the date is not present in other_config.
        /// </summary>
        public DateTime LastShutdownTime()
        {
            return other_config.ContainsKey("last_shutdown_time") &&
                   Util.TryParseIso8601DateTime(other_config["last_shutdown_time"], out var result)
                ? result
                : DateTime.MinValue;
        }

        /// <remarks>
        /// AlwaysRestartHighPriority and AlwaysRestart are replaced by Restart in Boston; we still keep them for backward compatibility
        /// </remarks>
        public enum HA_Restart_Priority { AlwaysRestartHighPriority, AlwaysRestart, Restart, BestEffort, DoNotRestart };

        /// <summary>
        /// An enum-ified version of ha_restart_priority: use this one instead.
        /// NB setting this property does not change ha-always-run.
        /// </summary>
        public HA_Restart_Priority HARestartPriority()
        {
            return StringToPriority(ha_restart_priority);
        }

         public override string NameWithLocation()
        {
            if (Connection != null)
            {
                if (IsRealVm())
                {
                    return base.NameWithLocation();
                }
                else if (is_a_snapshot)
                {
                    var snapshotOf = Connection.Resolve(snapshot_of);
                    if (snapshotOf == null)
                        return base.NameWithLocation();

                    return string.Format(Messages.SNAPSHOT_OF_TITLE, Name(), snapshotOf.Name(), LocationString());
                }
                else if (is_a_template)
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

        public static List<HA_Restart_Priority> GetAvailableRestartPriorities(IXenConnection connection)
        {
            var restartPriorities = new List<HA_Restart_Priority>();
            restartPriorities.Add(HA_Restart_Priority.Restart);
            restartPriorities.Add(HA_Restart_Priority.BestEffort);
            restartPriorities.Add(HA_Restart_Priority.DoNotRestart);
            return restartPriorities;
        }

        /// <summary>
        /// Returns true if VM's restart priority is AlwaysRestart or AlwaysRestartHighPriority.
        /// </summary>
        public bool HaPriorityIsRestart()
        {
            return HaPriorityIsRestart(Connection, HARestartPriority());
        }

        public static bool HaPriorityIsRestart(IXenConnection connection, HA_Restart_Priority haRestartPriority)
        {
            return haRestartPriority == HA_Restart_Priority.Restart;
        }

        public static HA_Restart_Priority HaHighestProtectionAvailable(IXenConnection connection)
        {
            return HA_Restart_Priority.Restart;
        }

        public const string RESTART_PRIORITY_ALWAYS_RESTART_HIGH_PRIORITY = "0"; //only used for Pre-Boston pools
        public const string RESTART_PRIORITY_ALWAYS_RESTART = "1"; //only used for Pre-Boston pools
        /// <summary>
        /// This is the new "Restart" priority in Boston, and will replace RESTART_PRIORITY_ALWAYS_RESTART_HIGH_PRIORITY and RESTART_PRIORITY_ALWAYS_RESTART
        /// </summary>
        public const string RESTART_PRIORITY_RESTART = "restart";
        public const string RESTART_PRIORITY_BEST_EFFORT = "best-effort";
        public const string RESTART_PRIORITY_DO_NOT_RESTART = "";

        /// <summary>
        /// Parses a HA_Restart_Priority into a string the server understands.
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        internal static string PriorityToString(HA_Restart_Priority priority)
        {
            switch (priority)
            {
                case HA_Restart_Priority.AlwaysRestartHighPriority:
                    return RESTART_PRIORITY_ALWAYS_RESTART_HIGH_PRIORITY;
                case HA_Restart_Priority.AlwaysRestart:
                    return RESTART_PRIORITY_ALWAYS_RESTART;
                case HA_Restart_Priority.Restart:
                    return RESTART_PRIORITY_RESTART;
                case HA_Restart_Priority.BestEffort:
                    return RESTART_PRIORITY_BEST_EFFORT;
                default:
                    return RESTART_PRIORITY_DO_NOT_RESTART;
            }
        }

        internal static HA_Restart_Priority StringToPriority(string priority)
        {
            switch (priority)
            {
                case RESTART_PRIORITY_ALWAYS_RESTART_HIGH_PRIORITY:
                    return HA_Restart_Priority.AlwaysRestartHighPriority;
                case RESTART_PRIORITY_RESTART:
                    return HA_Restart_Priority.Restart;
                case RESTART_PRIORITY_DO_NOT_RESTART:
                    return HA_Restart_Priority.DoNotRestart;
                case RESTART_PRIORITY_BEST_EFFORT:
                    return HA_Restart_Priority.BestEffort;
                default:
                    return HA_Restart_Priority.AlwaysRestart;
            }
        }

        /// <summary>
        /// Whether HA is capable of restarting this VM (i.e. the VM is not a template or control domain).
        /// </summary>
        public bool HaCanProtect(bool showHiddenVMs)
        {
            return IsRealVm() && Show(showHiddenVMs);

        }

        /// <summary>
        /// True if this VM's ha_restart_priority is not "Do not restart" and its pool has ha_enabled true.
        /// </summary>
        public bool HAIsProtected()
        {
            if (Connection == null)
                return false;
            var myPool = Helpers.GetPoolOfOne(Connection);
            if (myPool == null)
                return false;
            return myPool.ha_enabled && HARestartPriority() != HA_Restart_Priority.DoNotRestart;
        }

        /// <summary>
        /// Calls set_ha_restart_priority
        /// </summary>
        /// <param name="priority"></param>
        public static void SetHaRestartPriority(Session session, VM vm, HA_Restart_Priority priority)
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
        /// Checks whether the VM is the dom0 (the flag is_control_domain may also apply to other control domains)
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
            {
                if (pbd != null &&
                    pbd.other_config.TryGetValue("storage_driver_domain", out var vmRef) &&
                    vmRef == opaque_ref)
                {
                    sr = Connection.Resolve(pbd.SR);
                    if (sr != null)
                        return true;
                }
            }

            return false;
        }

        public bool IsRealVm()
        {
            return !is_a_snapshot && !is_a_template && !is_control_domain;
        }

        private bool _isBeingCreated;
        [JsonIgnore]
        public bool IsBeingCreated
        {
            get { return _isBeingCreated; }
            set
            {
                _isBeingCreated = value;
                NotifyPropertyChanged("IsBeingCreated");
            }
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
        /// The name label of the VM's affinity server, or None if it is not set
        /// (This is what the UI calls the "home server", but is not the same as VM.Home).
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
            if (DefaultTemplate())
            {
                return false;
            }

            if (bios_strings.Count == 0)
            {
                return false;
            }

            var value = bios_strings.ContainsKey("bios-vendor") && bios_strings["bios-vendor"] == "Xen"
                                                                && bios_strings.ContainsKey("system-manufacturer") && bios_strings["system-manufacturer"] == "Xen";

            return !value;
        }

        public bool HasCD()
        {
            foreach (var vbd in Connection.ResolveAll(VBDs))
            {
                if (vbd.IsCDROM())
                {
                    return true;
                }
            }
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
                if (vbd != null)
                {
                    var vdi = vbd.Connection.Resolve(vbd.VDI);
                    if (vdi != null)
                    {
                        yield return vdi.Connection.Resolve(vdi.SR);
                    }
                }
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
                return long.TryParse(platform["cores-per-socket"], out coresPerSocket) ? coresPerSocket : DEFAULT_CORES_PER_SOCKET;
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
            var sockets = ValidVCPUConfiguration(VCPUs_max, cores) == "" ? VCPUs_max/cores : 0;
            return GetTopology(sockets, cores);
        }

        public static string GetTopology(long sockets, long cores)
        {
            if (sockets == 0) // invalid cores value
                return cores == 1 ? string.Format(Messages.CPU_TOPOLOGY_STRING_INVALID_VALUE_1) : string.Format(Messages.CPU_TOPOLOGY_STRING_INVALID_VALUE, cores);
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
            return vbds.Select(vbd => Connection.Resolve(vbd.VDI)).FirstOrDefault(vdi => vdi != null && vdi.IsCloudConfigDrive());
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
        /// Return the list of VDIs that have Read Caching enabled
        /// </summary>
        public List<VDI> ReadCachingVDIs()
        {
            var readCachingVdis = new List<VDI>();
            foreach (var vbd in Connection.ResolveAll(VBDs).Where(vbd => vbd != null && vbd.currently_attached))
            {
                var vdi = Connection.Resolve(vbd.VDI);
                var resident_host = Connection.Resolve(resident_on);
                if (vdi != null && resident_host != null && vdi.ReadCachingEnabled(resident_host))
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
                var resident_host = Connection.Resolve(resident_on);
                if (vdi != null && resident_host != null && !vdi.ReadCachingEnabled(resident_host))
                {
                    var reason = vdi.ReadCachingDisabledReason(resident_host);
                    if (reason > ans)
                        ans = reason;
                }
            }

            switch (ans)
            {
                case VDI.ReadCachingDisabledReasonCode.LICENSE_RESTRICTION:
                    if (Helpers.FeatureForbidden(Connection, Host.RestrictReadCaching))
                        return Messages.VM_READ_CACHING_DISABLED_REASON_LICENSE;
                    else
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
        /// Whether the VM can be moved inside the pool (vdi copy + destroy) 
        /// </summary>
        public bool CanBeMoved()
        {
            if (SRs().Any(sr => sr != null && sr.HBALunPerVDI()))
                return false;

            if (!is_a_template && !Locked && allowed_operations != null && allowed_operations.Contains(vm_operations.export) && power_state != vm_power_state.Suspended)
            {
                return Connection.ResolveAll(VBDs).Find(v => v.GetIsOwner()) != null;
            }
            return false;
        }

        /// <summary>
        /// Returns whether this is a Windows VM by checking the distro value in the
        /// guest_metrics before falling back to the viridian flag. The result may not be
        /// correct at all times (a Linux distro can be detected if the guest agent is
        /// running on the VM). It is more reliable if the VM has already booted once, and
        /// also works for the "Other Install Media" template and unbooted VMs made from it.
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
                !string.IsNullOrEmpty(distro) && linuxDistros.Contains(distro.ToLowerInvariant()))
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
                var regex = new Regex("'guest_metrics' +'(OpaqueRef:[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12})'");

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
        /// Returns the VM IP address for SSH login.
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

                    if (pif.IsManagementInterface(false))
                    {
                        ipAddresses.Add(pif.IP);
                    }
                }
            }

            //find first IPv4 address and return it - we would use it if there is one
            IPAddress addr;
            foreach (var addrString in ipAddresses)
                if (IPAddress.TryParse(addrString, out addr) && addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
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
            return (platform != null) &&
                platform.ContainsKey("device-model") &&
                platform["device-model"] == "qemu-upstream-compat";
        }
        /// <summary>
        /// Whether the VM's boot mode can be changed. A VM's boot mode cannot be changed once the VM has been started.
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
    }

    public struct VMStartupOptions
    {
        public long Order, StartDelay;
        public VM.HA_Restart_Priority? HaRestartPriority;

        public VMStartupOptions(long order, long startDelay)
        {
            Order = order;
            StartDelay = startDelay;
            HaRestartPriority = null;
        }

        public VMStartupOptions(long order, long startDelay, VM.HA_Restart_Priority haRestartPriority)
        {
            Order = order;
            StartDelay = startDelay;
            HaRestartPriority = haRestartPriority;
        }
    }

    public enum VmBootMode { Bios, Uefi, SecureUefi }

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
