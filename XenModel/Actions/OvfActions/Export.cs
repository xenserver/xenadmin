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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using DiscUtils;
using DiscUtils.Vhd;
using XenAdmin.Core;
using XenOvf;
using XenOvf.Definitions;
using XenOvf.Utilities;
using XenAPI;


namespace XenAdmin.Actions.OvfActions
{
    public partial class ExportApplianceAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private EnvelopeType Export(string targetDir, VM vm, Action<float> updatePercentage)
        {
            if (vm.power_state != vm_power_state.Halted && vm.power_state != vm_power_state.Suspended)
            {
                log.Error($"Cannot export VM {vm.Name()} ({vm.opaque_ref}); it is neither halted nor suspended.");
                throw new Exception(string.Format(Messages.ERROR_VM_NOT_HALTED, vm.Name()));
            }

            log.Info($"Exporting metadata for {vm.name_label}...");

            #region CREATE ENVELOPE / ADD VIRTUAL SYSTEM
            EnvelopeType ovfEnv = OVF.CreateEnvelope(m_applianceFileName);
            string vsId = Guid.NewGuid().ToString();
            OVF.AddVirtualSystem(ovfEnv, vm.name_label, vsId);
            string vhsId = OVF.AddVirtualHardwareSection(ovfEnv, vsId);
            #endregion

            #region TRY TO ID OS

            VM_guest_metrics vmgm = Connection.Resolve(vm.guest_metrics);

            if (vmgm?.os_version != null && vmgm.os_version.TryGetValue("name", out string osName))
            {
                ushort osId = ValueMaps.OperatingSystem(osName);
                if (osId == 0xFFFF)
                    osId = 1; // change to OTHER since search failed.

                string version = OVF.GetContentMessage("SECTION_OPERATINGSYSTEM_INFO");

                if (vmgm.os_version.TryGetValue("major", out string major) &&
                    vmgm.os_version.TryGetValue("minor", out string minor))
                {
                    version = string.Format(OVF.GetContentMessage("SECTION_OPERATINGSYSTEM_VERSION"), major, minor);
                }

                string[] osNameParts = osName.Split('|');
                if (osNameParts.Length > 0)
                    OVF.UpdateOperatingSystemSection(ovfEnv, vsId, osNameParts[0], version, osId);
            }

            #endregion

            #region ADD VirtualSystemType identification
            var pv = vm.IsHVM() ? "hvm" : "xen";
            var arch = string.IsNullOrEmpty(vm.domarch) ? "unknown" : vm.domarch;
            var vmType = string.Format("{0}-3.0-{1}", pv, arch);

            OVF.AddVirtualSystemSettingData(ovfEnv, vsId, vhsId, vm.name_label, OVF.GetContentMessage("VSSD_CAPTION"), vm.name_description, Guid.NewGuid().ToString(), vmType);
            #endregion

            #region ADD CPUS
            OVF.SetCPUs(ovfEnv, vsId, (ulong)vm.VCPUs_at_startup, (ulong)vm.VCPUs_max);
            #endregion

            #region ADD MEMORY
            OVF.SetMemory(ovfEnv, vsId, (ulong)(vm.memory_dynamic_max / Util.BINARY_MEGA), "MB");
            #endregion

            #region ADD NETWORKS

            foreach (XenRef<VIF> vifRef in vm.VIFs)
            {
                VIF vif = Connection.Resolve(vifRef);
                if (vif == null)
                    continue;

                XenAPI.Network net = Connection.Resolve(vif.network);
                if (net == null)
                    continue;

                OVF.AddNetwork(ovfEnv, vsId, net.uuid, net.name_label, net.name_description, vif.MAC);
            }

            #endregion

            #region SET STARTUP OPTIONS
            OVF.AddStartupSection(ovfEnv, true, vsId, vm.order, vm.start_delay, vm.shutdown_delay);
            #endregion

            #region EXPORT DISKS

            int diskIndex = 0;
            var vbdRefs = vm.VBDs;

            for (int i = 0; i < vbdRefs.Count; i++)
            {
                int curVbd = i;
                XenRef<VBD> vbdRef = vbdRefs[i];

                VBD vbd = Connection.Resolve(vbdRef);
                if (vbd == null)
                    continue;

                if (vbd.type == vbd_type.CD)
                {
                    string rasdid = OVF.AddCDROM(ovfEnv, vsId, vbd.uuid, OVF.GetContentMessage("RASD_16_CAPTION"), OVF.GetContentMessage("RASD_16_DESCRIPTION"));
                    OVF.SetTargetDeviceInRASD(ovfEnv, vsId, rasdid, vbd.userdevice);
                    continue;
                }

                VDI vdi = Connection.Resolve(vbd.VDI);
                if (vdi == null)
                    continue;

                try
                {
                    var diskFilename = $"{vdi.uuid}.vhd";
                    var diskPath = Path.Combine(targetDir, diskFilename);

                    if (File.Exists(diskPath))
                    {
                        var oldFileName = diskFilename;
                        diskFilename = $"{vdi.uuid}_{Thread.CurrentThread.ManagedThreadId}.vhd";
                        diskPath = Path.Combine(targetDir, diskFilename);
                        log.InfoFormat("VHD Name collision, renamed {0} to {1}", oldFileName, diskFilename);
                    }

                    string diskName = vdi.name_label;
                    if (string.IsNullOrEmpty(diskName))
                        diskName = $"{OVF.GetContentMessage("RASD_19_CAPTION")} {diskIndex}";

                    if (!MetaDataOnly)
                    {
                        log.Info($"Exporting disk {diskName} to {diskFilename} for {vm.name_label}...");

                        var taskRef = Task.create(Connection.Session, "export_raw_vdi_task",
                            $"Exporting VDI {vdi.uuid} to {diskFilename}");

                        HTTP_actions.get_export_raw_vdi(b =>
                            {
                                Description = string.Format(Messages.EXPORTING_VDI, diskName, diskFilename,
                                    Util.DiskSizeString(b, 2, "F2"), Util.DiskSizeString(vdi.virtual_size));
                                updatePercentage((curVbd + (float)b / vdi.virtual_size) / vbdRefs.Count);
                            },
                            () => Cancelling, XenAdminConfigManager.Provider.GetProxyTimeout(true),
                            Connection.Hostname, XenAdminConfigManager.Provider.GetProxyFromSettings(Connection),
                            diskPath, taskRef, Connection.Session.opaque_ref, vdi.uuid, "vhd");

                        if (m_shouldVerify)
                        {
                            Description = string.Format(Messages.EXPORTING_VDI_VERIFICATION, diskFilename);

                            using (var stream = new FileStream(diskPath, FileMode.Open, FileAccess.Read))
                            using (var sw = new StringWriter())
                            {
                                var vhdChecker = new FileChecker(stream);
                                var result = vhdChecker.Check(sw, ReportLevels.All);
                                log.InfoFormat("Verifying disk {0}:\n{1}", diskFilename, sw.ToString().Replace("\0", ""));
                                if (!result)
                                    throw new Exception(string.Format(Messages.EXPORTING_VDI_VERIFICATION_FAILURE, diskFilename));
                            }
                        }
                    }

                    string diskId = Guid.NewGuid().ToString();

                    OVF.AddDisk(ovfEnv, vsId, diskId, diskFilename, vbd.bootable, diskName,
                        vdi.name_description, (ulong)vdi.physical_utilisation, (ulong)vdi.virtual_size);
                    OVF.SetTargetDeviceInRASD(ovfEnv, vsId, diskId, vbd.userdevice);

                    diskIndex++;
                }
                catch (HTTP.CancelledException)
                {
                    throw new CancelledException();
                }
            }

            #endregion

            #region ADD XEN SPECIFICS

            var hypervisorInfo = string.Format(OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"), BrandManager.ProductBrand);
            var platformInfo = string.Format(OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_3"), BrandManager.ProductBrand);

            var _params = vm.HVM_boot_params;
            if (_params != null && _params.Count > 0)
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "HVM_boot_params", string.Join(";", _params.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_6"));
            }

            if (!string.IsNullOrEmpty(vm.HVM_boot_policy))
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "HVM_boot_policy", vm.HVM_boot_policy, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_2"));
            }

            if (vm.HVM_shadow_multiplier != 1.0)
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "HVM_shadow_multiplier", Convert.ToString(vm.HVM_shadow_multiplier), hypervisorInfo);
            }

            var platform = vm.platform;
            if (platform != null && platform.Count > 0)
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "platform", string.Join(";",
                    platform.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))), platformInfo);
            }

            var nvram = vm.NVRAM;
            if (nvram != null && nvram.Count > 0)
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "NVRAM", string.Join(";", nvram.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_7"));
            }

            if (!string.IsNullOrEmpty(vm.PV_args))
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_args", vm.PV_args, hypervisorInfo);
            }

            if (!string.IsNullOrEmpty(vm.PV_bootloader))
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_bootloader", vm.PV_bootloader, hypervisorInfo);
            }

            if (!string.IsNullOrEmpty(vm.PV_bootloader_args))
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_bootloader_args", vm.PV_bootloader_args, hypervisorInfo);
            }

            if (!string.IsNullOrEmpty(vm.PV_kernel))
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_kernel", vm.PV_kernel, hypervisorInfo);
            }

            if (!string.IsNullOrEmpty(vm.PV_legacy_args))
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_legacy_args", vm.PV_legacy_args, hypervisorInfo);
            }

            if (!string.IsNullOrEmpty(vm.PV_ramdisk))
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_ramdisk", vm.PV_ramdisk, hypervisorInfo);
            }

            if (vm.hardware_platform_version >= 0)
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "hardware_platform_version", vm.hardware_platform_version.ToString(), hypervisorInfo);
            }

            if (!string.IsNullOrEmpty(vm.recommendations))
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "recommendations", vm.recommendations, hypervisorInfo);
            }

            if (!string.IsNullOrEmpty(vm.reference_label))
            {
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "reference_label", vm.reference_label, hypervisorInfo);
            }

            if (vm.has_vendor_device)
            {
                //serialise it with a different name to avoid it being deserialised automatically and getting the wrong type
                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "VM_has_vendor_device", vm.has_vendor_device.ToString(), hypervisorInfo);
            }

            foreach (XenRef<VGPU> gpuRef in vm.VGPUs)
            {
                VGPU vgpu = Connection.Resolve(gpuRef);

                if (vgpu != null)
                {
                    var vgpuGroup = Connection.Resolve(vgpu.GPU_group);
                    var vgpuType = Connection.Resolve(vgpu.type);

                    var sb = new StringBuilder();
                    sb.AppendFormat("GPU_types={{{0}}};",
                        vgpuGroup?.GPU_types == null || vgpuGroup.GPU_types.Length < 1
                            ? ""
                            : string.Join(";", vgpuGroup.GPU_types));
                    sb.AppendFormat("VGPU_type_vendor_name={0};", vgpuType?.vendor_name ?? "");
                    sb.AppendFormat("VGPU_type_model_name={0};", vgpuType?.model_name ?? "");
                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "vgpu", sb.ToString(), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_4"), true);
                }
            }

            string pvsSiteUuid = string.Empty;
            var allProxies = Connection.Cache.PVS_proxies;

            foreach (var p in allProxies.Where(p => p != null && p.VIF != null))
            {
                var vif = Connection.Resolve(p.VIF);
                if (vif != null)
                {
                    var vmFromVif = Connection.Resolve(vif.VM);
                    if (vmFromVif != null && vmFromVif.uuid == vm.uuid)
                    {
                        var pvsSite = Connection.Resolve(p.site);
                        if (pvsSite != null)
                            pvsSiteUuid = pvsSite.uuid;

                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(pvsSiteUuid))
            {
                var sb = new StringBuilder();
                sb.AppendFormat("PVS_SITE={{{0}}};", string.Format("uuid={0}", pvsSiteUuid));

                OVF.AddOtherSystemSettingData(ovfEnv, vsId, "pvssite", sb.ToString(), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_5"));
            }

            #endregion

            OVF.FinalizeEnvelope(ovfEnv);
            return ovfEnv;
        }
    }
}
