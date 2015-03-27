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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Xml;
using Citrix.XenCenter;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Model;
using XenAdmin.Network;


namespace XenAPI
{
    public enum SnapshotsView
    {
        ListView, TreeView, None
    }
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
        private const int DEFAULT_NUM_VBDS_ALLOWED = 16;
        public const long DEFAULT_MEM_ALLOWED = 128 * Util.BINARY_GIGA;
        public const int DEFAULT_CORES_PER_SOCKET = 1;
        private SnapshotsView _snapshotView = SnapshotsView.None;

        private XmlDocument xdRecommendations = null;

        public int MaxVCPUsAllowed
        {
            get
            {
                XmlDocument xd = GetRecommendations();

                if (xd == null)
                    return DEFAULT_NUM_VCPUS_ALLOWED;

                XmlNode xn = xd.SelectSingleNode(@"restrictions/restriction[@field='vcpus-max']");

                try
                {
                    return Convert.ToInt32(xn.Attributes["max"].Value);
                }
                catch
                {
                    return DEFAULT_NUM_VCPUS_ALLOWED;
                }
            }
        }

        public bool IsRunning
        {
            get
            {
                return power_state == vm_power_state.Running;
            }
        }

        /// <summary>
        /// Returns true if the VM's pool has HA enabled and the VM has a saved restart priority other than DoNotRestart.
        /// Does not take account of ha-always-run.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public bool HasSavedRestartPriority
        {
            get
            {
                Pool pool = Helpers.GetPoolOfOne(Connection);
                return pool != null && pool.ha_enabled && !String.IsNullOrEmpty(ha_restart_priority);
            }
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
                VM from = Connection.Resolve(snapshot_of);
                return (from == null ? null : from.Home()); // "from" can be null if VM has been deleted
            }

            if (is_a_template)  // Templates (apart from snapshots) don't have a "home", even if their affinity is set CA-36286
                return null;

            if (power_state == vm_power_state.Running)
                return Connection.Resolve(resident_on);

            Host storage_host = GetStorageHost(false);
            if (storage_host != null)
                return storage_host;

            Host affinityHost = Connection.Resolve(affinity);
            if (affinityHost != null && affinityHost.IsLive)
                return affinityHost;

            return null;
        }


        public long TotalVMSize
        {
            get
            {
                long size = 0;
                foreach (VBD vbd in Connection.ResolveAll<VBD>(VBDs))
                {
                    if (vbd.type == vbd_type.CD)
                        continue;

                    VDI vdi = Connection.Resolve<VDI>(vbd.VDI);
                    if (vdi == null)
                        continue;

                    size += vdi.physical_utilisation;
                }
                return size;
            }
        }

        public VBD FindVMCDROM()
        {
            if (Connection == null)
                return null;

            List<VBD> vbds = Connection.ResolveAll(VBDs).FindAll(delegate(VBD vbd) { return vbd.IsCDROM; });

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
            VBD vbd = FindVMCDROM();
            if (vbd != null)
            {
                VDI vdi = vbd.Connection.Resolve(vbd.VDI);
                if (vdi != null)
                {
                    return vdi.Connection.Resolve(vdi.SR);
                }
            }
            return null;
        }

        public override string Name
        {
            get
            {
                const string CONTROL_DOMAIN = "Control domain on host: ";
                if (name_label != null && name_label.StartsWith(CONTROL_DOMAIN))
                {
                    var hostName = name_label.Substring(CONTROL_DOMAIN.Length);
                    return string.Format(Messages.CONTROL_DOM_ON_HOST, hostName);
                }
                return name_label;
            }
        }

        public long MaxMemAllowed
        {
            get
            {
                XmlDocument xd = GetRecommendations();

                if (xd == null)
                    return DEFAULT_MEM_ALLOWED;

                XmlNode xn = xd.SelectSingleNode(@"restrictions/restriction[@field='memory-static-max']");

                try
                {
                    return Convert.ToInt64(xn.Attributes["max"].Value);
                }
                catch
                {
                    return DEFAULT_MEM_ALLOWED;
                }
            }
        }

        public int MaxVIFsAllowed
        {
            get
            {
                XmlDocument xd = GetRecommendations();

                if (xd == null)
                    return DEFAULT_NUM_VIFS_ALLOWED;

                XmlNode xn = xd.SelectSingleNode(@"restrictions/restriction[@property='number-of-vifs']");

                try
                {
                    return Convert.ToInt32(xn.Attributes["max"].Value);
                }
                catch
                {
                    return DEFAULT_NUM_VIFS_ALLOWED;
                }
            }
        }

        public int MaxVBDsAllowed
        {
            get
            {
                XmlDocument xd = GetRecommendations();

                if (xd == null)
                    return DEFAULT_NUM_VBDS_ALLOWED;

                XmlNode xn = xd.SelectSingleNode(@"restrictions/restriction[@property='number-of-vbds']");

                try
                {
                    return Convert.ToInt32(xn.Attributes["max"].Value);
                }
                catch
                {
                    return DEFAULT_NUM_VBDS_ALLOWED;
                }
            }
        }

        private XmlDocument GetRecommendations()
        {
            if (xdRecommendations != null)
                return xdRecommendations;

            if (string.IsNullOrEmpty(this.recommendations))
                return null;

            xdRecommendations = new XmlDocument();

            try
            {
                xdRecommendations.LoadXml(this.recommendations);
            }
            catch
            {
                xdRecommendations = null;
            }

            return xdRecommendations;
        }

        public Host GetStorageHost(bool ignoreCDs)
        {

            foreach (VBD TheVBD in Connection.ResolveAll(VBDs))
            {

                if (ignoreCDs && TheVBD.type == vbd_type.CD)
                    continue;
                VDI TheVDI = Connection.Resolve(TheVBD.VDI);

                if (TheVDI == null || !TheVDI.Show(true))
                    continue;
                SR TheSR = Connection.Resolve(TheVDI.SR);
                if (TheSR == null)
                    continue;
                Host host = TheSR.GetStorageHost();
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
        public string BootOrder
        {
            get
            {
                if (this.HVM_boot_params.ContainsKey("order"))
                    return this.HVM_boot_params["order"].ToUpper();

                return "CD";
            }
            set
            {
                if (value == BootOrder)
                    return;

                Dictionary<string, string> new_HVM_boot_params =
                        HVM_boot_params == null ?
                            new Dictionary<string, string>() :
                            new Dictionary<string, string>(HVM_boot_params);

                new_HVM_boot_params["order"] = value.ToLower();

                HVM_boot_params = new_HVM_boot_params;
            }
        }

        public long Memory
        {
            set
            {
                memory_dynamic_min = value;
                memory_dynamic_max = value;
                memory_static_max = value;
            }
        }

        public int VCPUWeight
        {
            get
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
            set
            {
                if (value != VCPUWeight)
                {
                    Dictionary<string, string> new_VCPUs_params =
                        VCPUs_params == null ?
                            new Dictionary<string, string>() :
                            new Dictionary<string, string>(VCPUs_params);
                    new_VCPUs_params["weight"] = value.ToString();
                    VCPUs_params = new_VCPUs_params;
                }
            }
        }

        public bool DefaultTemplate
        {
            get { return Get(other_config, "default_template") != null; }
        }

        public bool InternalTemplate
        {
            get { return Get(other_config, "xensource_internal") != null; }
        }

        public string InstallRepository
        {
            get { return Get(other_config, "install-repository"); }
            set { if (InstallRepository != value) { set_other_config("install-repository", value); } }
        }

        public string InstallDistro
        {
            get { return Get(other_config, "install-distro"); }
        }

        public string InstallMethods
        {
            get { return Get(other_config, "install-methods"); }
        }

        public bool IsHVM
        {
            get { return HVM_boot_policy != ""; }
        }

        public bool HasRDP
        {
            get
            {
                var metrics = Connection.Resolve(this.guest_metrics);
                if (metrics == null)
                    return false;

                return 0 != IntKey(metrics.other, "feature-ts", 0);
            }
        }

        public bool RDPEnabled
        {
            get
            {
                var metrics = Connection.Resolve(this.guest_metrics);
                if (metrics == null)
                    return false;

                return 0 != IntKey(metrics.other, "data-ts", 0);
            }
        }

        public bool RDPControlEnabled
        {
            get
            {
                var metrics = Connection.Resolve(this.guest_metrics);
                if (metrics == null)
                    return false;

                return 0 != IntKey(metrics.other, "feature-ts2", 0);
            }
        }

        /// <summary>Returns true if
        /// 1) the guest is HVM and
        ///   2a) the allow-gpu-passthrough restriction is absent or
        ///   2b) the allow-gpu-passthrough restriction is non-zero
        ///</summary>
        public bool CanHaveGpu
        {
            get
            {
                if (!IsHVM)
                    return false;

                XmlDocument xd = GetRecommendations();

                if (xd == null)
                    return true;

                try
                {
                    XmlNode xn = xd.SelectSingleNode(@"restrictions/restriction[@field='allow-gpu-passthrough']");
                    if (xn == null)
                        return true;

                    return 
                        Convert.ToInt32(xn.Attributes["value"].Value) != 0;
                }
                catch
                {
                    return true;
                }
            }
        }

        /// <summary>Returns true if
        /// 1) the guest is HVM and
        ///   2a) the allow-vgpu restriction is absent or
        ///   2b) the allow-vgpu restriction is non-zero
        ///</summary>
        public bool CanHaveVGpu
        {
            get
            {
                if (!IsHVM || !CanHaveGpu)
                    return false;

                XmlDocument xd = GetRecommendations();

                if (xd == null)
                    return true;

                try
                {
                    XmlNode xn = xd.SelectSingleNode(@"restrictions/restriction[@field='allow-vgpu']");
                    if (xn == null || xn.Attributes == null)
                        return true;

                    return
                        Convert.ToInt32(xn.Attributes["value"].Value) != 0;
                }
                catch
                {
                    return true;
                }
            }
        }

        void set_other_config(string key, string value)
        {
            Dictionary<string, string> new_other_config =
                other_config == null ?
                    new Dictionary<string, string>() :
                    new Dictionary<string, string>(other_config);
            new_other_config[key] = value;
            other_config = new_other_config;
        }

        public bool AutoPowerOn
        {
            get
            {
                return BoolKey(other_config, "auto_poweron");
            }
            set
            {
                if (value != AutoPowerOn)
                    set_other_config("auto_poweron", value.ToString().ToLower());
            }
        }

        public bool IgnoreExcessiveVcpus
        {
            get
            {
                return BoolKey(other_config, "ignore_excessive_vcpus");
            }
            set
            {
                if (value != IgnoreExcessiveVcpus)
                    set_other_config("ignore_excessive_vcpus", value.ToString().ToLower());
            }
        }

        public string IsOnSharedStorage()
        {
            foreach (XenRef<VBD> vbdRef in VBDs)
            {
                VBD vbd = Connection.Resolve<VBD>(vbdRef);
                if (vbd != null)
                {
                    VDI vdi = Connection.Resolve<VDI>(vbd.VDI);
                    if (vdi != null)
                    {
                        SR sr = Connection.Resolve<SR>(vdi.SR);
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

            foreach (VBD vbd in Connection.ResolveAll(VBDs))
            {
                if (!vbd.IsCDROM)
                {
                    VDI VDI = Connection.Resolve<VDI>(vbd.VDI);
                    if (VDI != null && VDI.Show(showHiddenVMs))
                    {
                        SR TheSR = Connection.Resolve(VDI.SR);
                        if (TheSR != null && !TheSR.IsToolsSR)
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
                myClass = DefaultTemplate ? 5 : 4;
            else
                myClass = 2;

            if (other.is_control_domain)
                otherClass = 1;
            else if (other.is_a_snapshot)
                otherClass = 3;
            else if (other.is_a_template)
                otherClass = other.DefaultTemplate ? 5 : 4;
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

        public DateTime BodgeStartupTime
        {
            get
            {
                return startuptime;
            }
            set
            {
                startuptime = value;
                // This has an impact on the virt state of the VM as we allow a set amount of time for tools to show up before assuming unvirt
                NotifyPropertyChanged("virtualisation_status");
                if (VirtualizationTimer != null)
                    VirtualizationTimer.Stop();
                // 2 minutes before we give up plus some breathing space
                VirtualizationTimer = new Timer(182000) { AutoReset = false };
                VirtualizationTimer.Elapsed += VirtualizationTimer_Elapsed;
                VirtualizationTimer.Start();
            }
        }

        void VirtualizationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NotifyPropertyChanged("virtualisation_status");
        }

        private Timer VirtualizationTimer = null;

        [Flags]
        public enum VirtualisationStatus { UNKNOWN = 0, OPTIMIZED = 1, PV_DRIVERS_NOT_INSTALLED = 2, PV_DRIVERS_OUT_OF_DATE = 4 };

        public VirtualisationStatus virtualisation_status
        {
            get
            {
                return GetVirtualisationStatus;
            }
        }

        public string VirtualisationVersion
        {
            get
            {
                if (Connection == null)
                    return "0.0";
                VM_guest_metrics metrics = Connection.Resolve<VM_guest_metrics>(guest_metrics);
                if (metrics == null || !metrics.PV_drivers_version.ContainsKey("major") || !metrics.PV_drivers_version.ContainsKey("minor"))
                    return "0.0";
                return string.Format("{0}.{1}", metrics.PV_drivers_version["major"], metrics.PV_drivers_version["minor"]);
            }
        }

        public string GetVirtualisationWarningMessages()
        {
            VirtualisationStatus status = GetVirtualisationStatus;

            switch (status)
            {
                case VirtualisationStatus.OPTIMIZED:
                case VirtualisationStatus.UNKNOWN:
                    // calling function shouldn't send us here if tools are, or might be, present: used to assert here but it can sometimes happen (CA-51460)
                    return "";

                case VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE:
                    VM_guest_metrics guestMetrics = Connection.Resolve(guest_metrics);
                    if (guestMetrics != null
                        && guestMetrics.PV_drivers_version.ContainsKey("major")
                        && guestMetrics.PV_drivers_version.ContainsKey("minor"))
                    {
                        return String.Format(Messages.PV_DRIVERS_OUT_OF_DATE, String.Format("{0}.{1}",
                            guestMetrics.PV_drivers_version["major"],
                            guestMetrics.PV_drivers_version["minor"]));
                    }
                    else
                        return Messages.PV_DRIVERS_OUT_OF_DATE_UNKNOWN_VERSION;

                default:  // VirtualisationStatus.PV_DRIVERS_NOT_INSTALLED:
                    return Messages.PV_DRIVERS_NOT_INSTALLED;
            }
        }

        public VirtualisationStatus GetVirtualisationStatus
        {
            get
            {
                if (Connection == null)
                    return VirtualisationStatus.UNKNOWN;

                VM_metrics vm_metrics = Connection.Resolve(metrics);
                if (vm_metrics == null || power_state != vm_power_state.Running)
                {
                    return VirtualisationStatus.UNKNOWN;
                }

                VM_guest_metrics vm_guest_metrics = Connection.Resolve(guest_metrics);

                if ((DateTime.UtcNow - BodgeStartupTime).TotalMinutes < 2)
                {
                    // check to see if the metrics object has appeared, if so cancel the timer, no need to notify the property changed as this should be picked up on vm_guest_metrics being created.
                    if (vm_guest_metrics != null && vm_guest_metrics.PV_drivers_installed)
                    {
                        if (vm_guest_metrics.PV_drivers_up_to_date)
                            return VirtualisationStatus.OPTIMIZED;
                        else
                            return VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE;
                    }

                    return VirtualisationStatus.UNKNOWN;
                }

                if (vm_guest_metrics == null || !vm_guest_metrics.PV_drivers_installed)
                {
                    return VirtualisationStatus.PV_DRIVERS_NOT_INSTALLED;
                }
                else if (!vm_guest_metrics.PV_drivers_up_to_date)
                {
                    return VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE;
                }
                else
                {
                    return VirtualisationStatus.OPTIMIZED;
                }

            }
        }

        /// <summary>
        /// Does this VM support ballooning? I.e., are tools installed, on a ballonable OS?
        /// Doesn't check for Midnight Ride or licensing constraints.
        /// </summary>
        public bool has_ballooning
        {
            get
            {
                if (Connection == null)
                    return false;

                // For templates see comments in CA-34258/CA-34260: we cannot tell whether tools
                // are installed so we offer ballooning if and only if the dynamic min != static_max.
                if (is_a_template)
                    return (memory_dynamic_min != memory_static_max);

                VM_guest_metrics metrics = Connection.Resolve<VM_guest_metrics>(guest_metrics);
                if (metrics == null)
                    return false;
                Dictionary<string, string> other_key = metrics.other;
                return (other_key != null && other_key.ContainsKey("feature-balloon"));
            }
        }

        /// <summary>
        /// Whether to show advanced ballooning UI (i.e., separate setting of dynamic_max and static_max)
        /// </summary>
        public bool advanced_ballooning
        {
            get
            {
                return (memory_dynamic_max != memory_static_max && has_ballooning);
            }
        }

        /// <summary>
        /// Whether the VM should be shown to the user in the GUI.
        /// </summary>
        public override bool Show(bool showHiddenVMs)
        {
            if (InternalTemplate)
                return false;

            if (name_label.StartsWith(Helpers.GuiTempObjectPrefix))
                return false;

            if (showHiddenVMs)
                return true;

            return !IsHidden;

        }

        /// <summary>
        /// Returns whether the other_config.HideFromXenCenter flag is set to true.
        /// </summary>
        public override bool IsHidden
        {
            get
            {
                return BoolKey(other_config, HIDE_FROM_XENCENTER);
            }
        }

        public bool HasNoDisksAndNoLocalCD
        {
            get
            {
                if (Connection == null)
                    return false;
                foreach (VBD vbd in Connection.ResolveAll<VBD>(VBDs))
                {
                    if (vbd.type == vbd_type.Disk)
                    {
                        return false;       // we have a disk :(
                    }
                    else
                    {
                        VDI vdi = Connection.Resolve<VDI>(vbd.VDI);
                        if (vdi == null)
                            continue;
                        SR sr = Connection.Resolve<SR>(vdi.SR);
                        if (sr == null || sr.shared)
                            continue;
                        return false;       // we have a shared cd
                    }
                }
                return true;        // we have no disks hooray!!
            }
        }

        private const string P2V_SOURCE_MACHINE = "p2v_source_machine";
        private const string P2V_IMPORT_DATE = "p2v_import_date";

        public bool IsP2V
        {
            get
            {
                return other_config != null && other_config.ContainsKey(P2V_SOURCE_MACHINE) && other_config.ContainsKey(P2V_IMPORT_DATE);
            }
        }

        /// <summary>
        /// Sort in the following order:
        /// 1) User Templates
        /// 2) Windows VMs
        /// 3) Other VMs (e.g. Linux)
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
            Centos = 3,
            CoreOS = 4,
            Debian = 5,
            Oracle = 6,
            RedHat = 7,
            SciLinux = 8,
            Suse = 9,
            Ubuntu = 10,
            Citrix = 11,
            Solaris = 12,
            Misc = 13,
            Snapshot = 14,
            SnapshotFromVmpp = 15,
            Count = 16  //bump this if values are added
        }

        public VmTemplateType TemplateType
        {
            get
            {
                if (!is_a_template)
                    return VmTemplateType.NoTemplate;

                if (is_snapshot_from_vmpp)
                    return VmTemplateType.SnapshotFromVmpp;

                if (is_a_snapshot)
                    return VmTemplateType.Snapshot;

                if (!DefaultTemplate)
                    return VmTemplateType.Custom;

                string os = name_label.ToLowerInvariant();

                if (os.Contains("citrix"))
                    return VmTemplateType.Citrix;

                if (os.Contains("debian"))
                    return VmTemplateType.Debian;

                if (os.Contains("centos"))
                    return VmTemplateType.Centos;

                if (os.Contains("red hat"))
                    return VmTemplateType.RedHat;

                if (os.Contains("oracle"))
                    return VmTemplateType.Oracle;

                if (os.Contains("suse"))
                    return VmTemplateType.Suse;

                if (os.Contains("scientific"))
                    return VmTemplateType.SciLinux;

                if (os.Contains("windows"))
                    return VmTemplateType.Windows;

                if (os.Contains("ubuntu"))
                    return VmTemplateType.Ubuntu;

                if (os.Contains("solaris"))
                    return VmTemplateType.Solaris;

                if (os.Contains("coreos"))
                    return VmTemplateType.CoreOS;

                return VmTemplateType.Misc;
            }
        }

        public VmDescriptionType DescriptionType
        {
            get
            {
                var templateType = TemplateType;

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
        }
        
        public enum VmDescriptionType { None, ReadOnly, ReadWrite }
        
        public override string Description
        {
            get
            {
                // Don't i18n this
                if (IsP2V && name_description.StartsWith("VM imported from physical machine"))
                    return "";
                if (DescriptionType == VmDescriptionType.ReadOnly)
                    return PropertyManager.GetFriendlyName("VM.TemplateDescription-" + name_label) ?? name_description;

                //if this assertion fails it means the code calling this property
                //should be checking beforehand what the DescriptionType is
                Debug.Assert(DescriptionType != VmDescriptionType.None);

                return name_description;
            }
        }

        public string P2V_SourceMachine
        {
            get
            {
                return other_config != null && other_config.ContainsKey(P2V_SOURCE_MACHINE) ? other_config[P2V_SOURCE_MACHINE] : "";
            }
        }

        public DateTime P2V_ImportDate
        {
            get
            {
                if (other_config == null || !other_config.ContainsKey(P2V_IMPORT_DATE))
                    return DateTime.MinValue;

                string importDate = other_config[P2V_IMPORT_DATE];
                return TimeUtil.ParseISO8601DateTime(importDate);
            }
        }

        public static XenRef<Task> async_live_migrate(Session session, string _vm, string _host)
        {
            Dictionary<string, string> options = new Dictionary<string, string>();
            options["live"] = "true";
            return XenAPI.VM.async_pool_migrate(session, _vm, _host, options);
        }

        public String GetOSName()
        {
            VM_guest_metrics guestMetrics = Connection.Resolve(guest_metrics);
            if (guestMetrics == null)
                return Messages.UNKNOWN;

            if (guestMetrics.os_version == null)
                return Messages.UNKNOWN;

            if (!guestMetrics.os_version.ContainsKey("name"))
                return Messages.UNKNOWN;

            String os_name = guestMetrics.os_version["name"];

            // This hack is to make the windows names look nicer
            int index = os_name.IndexOf("|");
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
            VM_metrics metrics = Connection.Resolve(this.metrics);
            if (metrics == null)
                return DateTime.MinValue;

            return metrics.start_time;
        }
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        public PrettyTimeSpan RunningTime
        {
            get
            {
                if (power_state != vm_power_state.Running &&
                    power_state != vm_power_state.Paused &&
                    power_state != vm_power_state.Suspended)
                {
                    return null;
                }

                DateTime startTime = GetStartTime();
                if (startTime == Epoch || startTime == DateTime.MinValue)
                    return null;
                return new PrettyTimeSpan(DateTime.UtcNow - startTime - Connection.ServerTimeOffset);
            }
        }

        /// <summary>
        /// Returns DateTime.MinValue if the date is not present in other_config.
        /// </summary>
        public DateTime LastShutdownTime
        {
            get
            {
                if (other_config.ContainsKey("last_shutdown_time"))
                {
                    return TimeUtil.ParseISO8601DateTime(other_config["last_shutdown_time"]);
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }

        /// <remarks>
        /// AlwaysRestartHighPriority and AlwaysRestart are replaced by Restart in Boston; we still keep them for backward compatibility
        /// </remarks>
        public enum HA_Restart_Priority { AlwaysRestartHighPriority, AlwaysRestart, Restart, BestEffort, DoNotRestart };

        /// <summary>
        /// An enum-ified version of ha_restart_priority: use this one instead.
        /// For pre-Boston VMs: If this VM is running and ha-always-run is false, returns DoNotRestart regardless of the underlying ha_restart_priority. 
        /// NB setting this property does not change ha-always-run.
        /// </summary>
        public HA_Restart_Priority HARestartPriority
        {
            get
            {
                if (this.power_state == vm_power_state.Running && !this.ha_always_run && !Helpers.BostonOrGreater(this.Connection))
                {
                    return HA_Restart_Priority.DoNotRestart;
                }
                return StringToPriority(this.ha_restart_priority);
            }
            set
            {
                this.ha_restart_priority = PriorityToString(value);
            }
        }

        public override string NameWithLocation
        {
            get
            {
                if (this.Connection != null)
                {
                    if (this.is_a_real_vm)
                    {
                        return base.NameWithLocation;
                    }
                    else if (this.is_a_snapshot)
                    {
                        var snapshotOf = this.Connection.Resolve(this.snapshot_of);
                        if (snapshotOf == null)
                            return base.NameWithLocation;

                        return string.Format(Messages.SNAPSHOT_OF_TITLE, Name, snapshotOf.Name, LocationString);
                    }
                    else if (this.is_a_template)
                    {
                        if (Helpers.IsPool(Connection))
                            return string.Format(Messages.OBJECT_IN_POOL, Name, Connection.Name);

                        return string.Format(Messages.OBJECT_ON_SERVER, Name, Connection.Name);
                    }
                }

                return base.NameWithLocation;
            }
        }

        internal override string LocationString
        {
            get 
            {
                Host server = this.Home();
                if (server != null)
                    return string.Format(Messages.ON_SERVER, server);

                Pool pool = Helpers.GetPool(this.Connection);
                if (pool != null)
                    return string.Format(Messages.IN_POOL, pool);

                return string.Empty;
            }
        }


        public static List<HA_Restart_Priority> GetAvailableRestartPriorities(IXenConnection connection)
        {
            var restartPriorities = new List<HA_Restart_Priority>();
            if (connection == null || Helpers.BostonOrGreater(connection))
            {
                restartPriorities.Add(HA_Restart_Priority.Restart);
            }
            else
            {
                restartPriorities.Add(HA_Restart_Priority.AlwaysRestartHighPriority);
                restartPriorities.Add(HA_Restart_Priority.AlwaysRestart);
            }
            restartPriorities.Add(HA_Restart_Priority.BestEffort);
            restartPriorities.Add(HA_Restart_Priority.DoNotRestart);
            return restartPriorities;
        }

        /// <summary>
        /// Returns true if VM's restart priority is AlwaysRestart or AlwaysRestartHighPriority.
        /// </summary>
        public bool HaPriorityIsRestart()
        {
            HA_Restart_Priority haRestartPriority = HARestartPriority;
            return HaPriorityIsRestart(Connection, HARestartPriority);
        }

        public static bool HaPriorityIsRestart(IXenConnection connection, HA_Restart_Priority haRestartPriority)
        {
            if (Helpers.BostonOrGreater(connection))
                return haRestartPriority == HA_Restart_Priority.Restart;
            return haRestartPriority == HA_Restart_Priority.AlwaysRestart || haRestartPriority == HA_Restart_Priority.AlwaysRestartHighPriority;
        }

        public static HA_Restart_Priority HaHighestProtectionAvailable(IXenConnection connection)
        {
            if (Helpers.BostonOrGreater(connection))
                return HA_Restart_Priority.Restart;
            return HA_Restart_Priority.AlwaysRestart;
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
            return is_a_real_vm && Show(showHiddenVMs);

        }

        /// <summary>
        /// True if this VM's ha_restart_priority is not "Do not restart" and its pool has ha_enabled true.
        /// For pre-Boston VMs: True if this VM has ha_always_run set and its pool has ha_enabled true.
        /// </summary>
        public bool HAIsProtected
        {
            get
            {
                if (Connection == null)
                    return false;
                Pool myPool = Helpers.GetPoolOfOne(Connection);
                if (myPool == null)
                    return false;
                if (Helpers.BostonOrGreater(Connection))
                    return myPool.ha_enabled && this.HARestartPriority != HA_Restart_Priority.DoNotRestart;
                return myPool.ha_enabled && this.ha_always_run;
            }
        }

        /// <summary>
        /// Calls set_ha_restart_priority and set_ha_always_run as appropriate.
        /// </summary>
        /// <param name="priority"></param>
        public static void SetHaRestartPriority(Session session, VM vm, HA_Restart_Priority priority)
        {
            if (priority == HA_Restart_Priority.DoNotRestart)
            {
                // We must avoid the invalid state of ha_always_run==true, ha_restart_priority==DoNotRestart
                if (!Helpers.BostonOrGreater(vm.Connection))
                    VM.set_ha_always_run(session, vm.opaque_ref, false);

                VM.set_ha_restart_priority(session, vm.opaque_ref, PriorityToString(priority));
            }
            else
            {
                // Set new VM restart priority
                VM.set_ha_restart_priority(session, vm.opaque_ref, PriorityToString(priority));

                if (!Helpers.BostonOrGreater(vm.Connection))
                {
                    // If VM is running, set ha_always_run to true to activate HA protection
                    VM.set_ha_always_run(session, vm.opaque_ref, vm.power_state == vm_power_state.Running);
                }
            }
        }

        public bool AnyDiskFastClonable
        {
            get
            {
                if (Connection == null)
                    return false;
                foreach (VBD vbd in Connection.ResolveAll(VBDs))
                {
                    if (vbd.type != vbd_type.Disk)
                        continue;

                    VDI vdi = Connection.Resolve(vbd.VDI);
                    if (vdi == null)
                        continue;

                    SR sr = Connection.Resolve(vdi.SR);
                    if (sr == null)
                        continue;

                    SM sm = SM.GetByType(Connection, sr.type);
                    if (sm == null)
                        continue;

                    if (Array.IndexOf(sm.capabilities, "VDI_CLONE") != -1)
                        return true;
                }
                return false;
            }
        }

        public bool HasAtLeastOneDisk
        {
            get
            {
                if (Connection == null)
                    return false;
                foreach (VBD vbd in Connection.ResolveAll(VBDs))
                {
                    if (vbd.type != vbd_type.Disk)
                        continue;

                    return true;
                }
                return false;
            }
        }

        public bool not_a_real_vm
        {
            get { return is_a_snapshot || is_a_template || is_control_domain; }
        }

        public bool is_a_real_vm
        {
            get { return !not_a_real_vm; }
        }

        public XmlNode ProvisionXml
        {
            get
            {
                try
                {
                    string xml = Get(other_config, "disks");
                    if (string.IsNullOrEmpty(xml))
                        return null;

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    return doc.FirstChild;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set
            {
                other_config["disks"] = value.OuterXml;
            }
        }

        public bool InstantTemplate
        {
            get
            {
                return BoolKey(other_config, "instant");
            }
        }

        public override string ToString()
        {
            return name_label;
        }

        /// <summary>
        /// The name label of the VM's affinity server, or None if it is not set
        /// (This is what the UI calls the "home server", but is not the same as VM.Home).
        /// </summary>
        public string AffinityServerString
        {
            get
            {
                Host host = Connection.Resolve(affinity);
                if (host == null)
                    return Messages.NONE;

                return host.Name;
            }
        }

        /// <summary>
        /// The virtualisation state of the vm as a friendly string (optimized, out of date, not installed, unknown)
        /// </summary>
        public string VirtualisationStatusString
        {
            get
            {
                switch (virtualisation_status)
                {
                    case VM.VirtualisationStatus.OPTIMIZED:
                        return string.Format(Messages.VIRTUALIZATION_OPTIMIZED, VirtualisationVersion);
                    case VM.VirtualisationStatus.PV_DRIVERS_OUT_OF_DATE:
                        return string.Format(Messages.VIRTUALIZATION_OUT_OF_DATE, VirtualisationVersion);
                    case VM.VirtualisationStatus.PV_DRIVERS_NOT_INSTALLED:
                        return Messages.PV_DRIVERS_NOT_INSTALLED;
                    default:
                        return Messages.VIRTUALIZATION_UNKNOWN;
                }
            }
        }

        public SnapshotsView SnapshotView
        {
            get { return _snapshotView; }
            set { _snapshotView = value; }
        }

        public bool HasProvisionXML
        {
            get
            {
                return other_config != null && other_config.ContainsKey("disks");
            }
        }

        public bool BiosStringsCopied
        {
            get
            {
                if (DefaultTemplate)
                {
                    return false;
                }

                if (!Helpers.MidnightRideOrGreater(Connection))
                {
                    return false;
                }

                if (bios_strings.Count == 0)
                {
                    return false;
                }

                bool value = bios_strings.ContainsKey("bios-vendor") && bios_strings["bios-vendor"] == "Xen"
                    && bios_strings.ContainsKey("system-manufacturer") && bios_strings["system-manufacturer"] == "Xen";

                return !value;
            }
        }

        public bool HasCD
        {
            get
            {
                foreach (var vbd in this.Connection.ResolveAll<VBD>(this.VBDs))
                {
                    if (vbd.IsCDROM)
                    {
                        return true;
                    }
                }
                return false;
            }

        }

        public bool HasVGPUs
        {
            get
            {
                return (VGPUs != null && VGPUs.Count > 0);
            }
        }

        public bool HasGPUPassthrough
        {
            get
            {
                if (VGPUs != null && VGPUs.Count > 0)
                {
                    var vGPUs = Connection.ResolveAll(VGPUs);
                    return vGPUs.Any(vGPU => vGPU != null && vGPU.IsPassthrough);
                }
                return false;
            }
        }


        public virtual IEnumerable<SR> SRs
        {
            get
            {
                List<VBD> vbds = Connection.ResolveAll(VBDs);
                foreach (var vbd in vbds)
                {
                    if (vbd != null)
                    {
                        VDI vdi = vbd.Connection.Resolve(vbd.VDI);
                        if (vdi != null)
                        {
                            yield return vdi.Connection.Resolve(vdi.SR);
                        }
                    }
                }
            }
        }

        public bool IsAssignedToVapp
        {
            get
            {
                //on pre-boston servers appliance is null
                return appliance != null && appliance.opaque_ref != null &&
                       appliance.opaque_ref.StartsWith("OpaqueRef:") &&
                       appliance.opaque_ref != "OpaqueRef:NULL";
            }
        }

        /// <summary>
        /// Try to determine if this VM is WSS - this is a best guess only
        /// </summary>
        public bool CouldBeWss
        {
            get
            {
                const string wssName = "Web Self Service";
                return name_label.Contains(wssName);
            }
        }

        public static List<XenRef<SR>> GetDRMissingSRs(Session session, string vm, Session sessionTo)
        {
            return Helpers.CreedenceOrGreater(sessionTo.Connection)
                       ? VM.get_SRs_required_for_recovery(session, vm, sessionTo.uuid)
                       : null;
        }

        public long CoresPerSocket
        {
            get
            {
                if (platform != null && platform.ContainsKey("cores-per-socket"))
                {
                    long coresPerSocket;
                    return long.TryParse(platform["cores-per-socket"], out coresPerSocket) ? coresPerSocket : DEFAULT_CORES_PER_SOCKET;
                }
                return DEFAULT_CORES_PER_SOCKET;
            }
            set
            {
                if (value != CoresPerSocket)
                {

                    Dictionary<string, string> newPlatform =
                        platform == null ?
                            new Dictionary<string, string>() :
                            new Dictionary<string, string>(platform);
                    newPlatform["cores-per-socket"] = value.ToString();
                    platform = newPlatform;
                }
            }
        }

        public long MaxCoresPerSocket
        {
            get
            {
                var homeServer = Home();
                if (homeServer != null)
                    return homeServer.CoresPerSocket;

                var maxCoresPerSocket = 0;
                foreach (var host in this.Connection.Cache.Hosts)
                {
                    if (host.CoresPerSocket > maxCoresPerSocket)
                        maxCoresPerSocket = host.CoresPerSocket;
                }
                return maxCoresPerSocket;
            }
        }

        public bool HasValidVCPUConfiguration
        {
            get { return ValidVCPUConfiguration(VCPUs_max, CoresPerSocket); }
        }

        public static bool ValidVCPUConfiguration(long noOfVCPUs, long coresPerSocket)
        {
            return coresPerSocket > 0 && noOfVCPUs % coresPerSocket == 0;
        }

        public string Topology
        {
            get
            {
                var cores = CoresPerSocket;
                var sockets = ValidVCPUConfiguration(VCPUs_max, cores) ? VCPUs_max/cores : 0;
                return GetTopology(sockets, cores);
            }
        }

        public static string GetTopology(long sockets, long cores)
        {
            if (sockets == 0) // invalid cores value
                return string.Format(Messages.CPU_TOPOLOGY_STRING_INVALID_VALUE, cores);
            if (sockets == 1 && cores == 1)
                return Messages.CPU_TOPOLOGY_STRING_1_SOCKET_1_CORE;
            if (sockets == 1)
                return string.Format(Messages.CPU_TOPOLOGY_STRING_1_SOCKET_N_CORE, cores);
            if (cores == 1)
                return string.Format(Messages.CPU_TOPOLOGY_STRING_N_SOCKET_1_CORE, sockets);
            return string.Format(Messages.CPU_TOPOLOGY_STRING_N_SOCKET_N_CORE, sockets, cores);
        }

        public bool CanBeEnlightened
        {
            get { return other_config.ContainsKey("xscontainer-monitor"); }
        }

        public bool IsEnlightened
        {
            get
            {
                var v = Get(other_config, "xscontainer-monitor");
                return v == null ? false : v.ToLower() == "true";
            }
        }
        
        public VDI CloudConfigDrive
        {
            get
            {
                var vbds = Connection.ResolveAll(VBDs);
                return vbds.Select(vbd => Connection.Resolve(vbd.VDI)).FirstOrDefault(vdi => vdi != null && vdi.IsCloudConfigDrive);
            }
        }

        public bool CanHaveCloudConfigDrive
        {
            get
            {
                if (is_a_template && TemplateType == VmTemplateType.CoreOS)
                    return true;
                //otherwise check if it has a config drive
                return CloudConfigDrive != null;
            }
        }

        public VM_Docker_Info DockerInfo
        {
            get
            {
                string xml = Get(other_config, "docker_info");
                if (string.IsNullOrEmpty(xml))
                    return null;
                VM_Docker_Info info = new VM_Docker_Info(xml);
                return info;
            }
        }

        public VM_Docker_Version DockerVersion
        {
            get
            {
                string xml = Get(other_config, "docker_version");
                if (string.IsNullOrEmpty(xml))
                    return null;
                VM_Docker_Version info = new VM_Docker_Version(xml);
                return info;
            }
        }

        public bool ReadCachingEnabled
        {
            get
            {
                return ReadCachingVDIs.Count > 0;
            }
        }

        /// <summary>
        /// Return the list of VDIs that have Read Caching enabled
        /// </summary>
        public List<VDI> ReadCachingVDIs
        {
            get
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
        }

        public string ReadCachingDisabledReason
        {
            // The code in VDI.ReadCachingDisabledReason returns the first matching reason from the list
            // (LICENSE_RESTRICTION, SR_NOT_SUPPORTED, NO_RO_IMAGE, SR_OVERRIDE). In the case that there
            // are several VDIs with different reasons, this function returns the last matching reason,
            // because that is the VDI that is nearest to being read-cachable in some sense. As the reasons
            // are stored in an enum, we can just use greater-than to find the last reason 
            get 
            {
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

}
