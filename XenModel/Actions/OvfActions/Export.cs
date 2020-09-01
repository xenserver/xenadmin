/* Copyright (c) Citrix Systems, Inc. 
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using XenOvf;
using XenOvf.Definitions;
using XenOvf.Utilities;
using XenAPI;
using XenAdmin.Network;


namespace XenAdmin.Actions.OvfActions
{
    public partial class ExportApplianceAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static EnvelopeType Export(IXenConnection connection, string targetPath, string ovfname, VM vm, Action<string> OnUpdate, HTTP.FuncBool onCancel, bool verifyDisks, bool metaDataOnly = false)
        {
                log.DebugFormat("Export: {0}, {1}", ovfname, targetPath);

                if (vm.power_state != vm_power_state.Halted && vm.power_state != vm_power_state.Suspended)
                {
                	var message = string.Format(Messages.ERROR_VM_NOT_HALTED, vm.Name());
                	OnUpdate(message);
                    log.Info($"VM {vm.Name()} ({vm.opaque_ref}) is neither halted nor suspended.");
                    throw new Exception(message);
                }

                #region CREATE ENVELOPE / ADD VIRTUAL SYSTEM
                EnvelopeType ovfEnv = OVF.CreateEnvelope(ovfname);
                string vsId = OVF.AddVirtualSystem(ovfEnv, vm.name_label);
                string vhsId = OVF.AddVirtualHardwareSection(ovfEnv, vsId);
                #endregion

                #region TRY TO ID OS
                VM_guest_metrics vmgm = connection.Resolve(vm.guest_metrics);

                if (vmgm != null && vmgm.os_version != null && vmgm.os_version.Count > 0)
                {
                    foreach (string key in vmgm.os_version.Keys)
                    {
                        if (key.ToLower().Equals("name"))
                        {
                            ushort osid = ValueMaps.OperatingSystem(vmgm.os_version[key]);
                            if (osid == 0xFFFF)
                                osid = 1; // change to OTHER since search failed.

                            string version = OVF.GetContentMessage("SECTION_OPERATINGSYSTEM_INFO");
                            if (vmgm.os_version.ContainsKey("major") && vmgm.os_version.ContainsKey("minor"))
                            {
                                version = string.Format(OVF.GetContentMessage("SECTION_OPERATINGSYSTEM_VERSION"), vmgm.os_version["major"], vmgm.os_version["minor"]);
                            }

                            string osname = vmgm.os_version[key].Split('|')[0];
                            OVF.UpdateOperatingSystemSection(ovfEnv, vsId, osname, version, osid);
                            break;
                        }
                    }
                }
                #endregion

                #region ADD VirtualSystemType identification

                var pv = vm.IsHVM() ? "hvm" : "xen";
                var arch = string.IsNullOrEmpty(vm.domarch) ? "unknown" : vm.domarch;
                var vmType = string.Format("{0}-3.0-{1}", pv, arch);

                OVF.AddVirtualSystemSettingData(ovfEnv, vsId, vhsId, vm.name_label, OVF.GetContentMessage("VSSD_CAPTION"), vm.name_description, Guid.NewGuid().ToString(), vmType);
                #endregion

                #region ADD CPUS
                OVF.SetCPUs(ovfEnv, vsId, (ulong)vm.VCPUs_max);
                #endregion

                #region ADD MEMORY
                OVF.SetMemory(ovfEnv, vsId, (ulong)(vm.memory_dynamic_max / Util.BINARY_MEGA), "MB");
                #endregion

                #region ADD NETWORKS
                foreach (XenRef<VIF> vifRef in vm.VIFs)
                {
                    VIF vif = connection.Resolve(vifRef);
                    if (vif == null)
                        continue;

                    XenAPI.Network net = connection.Resolve(vif.network);
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

                foreach (XenRef<VBD> vbdRef in vm.VBDs)
                {
                    VBD vbd = connection.Resolve(vbdRef);
                    if (vbd == null)
                        continue;

                    if (vbd.type == vbd_type.CD)
                    {
                        string rasdid = OVF.AddCDROM(ovfEnv, vsId, vbd.uuid, OVF.GetContentMessage("RASD_16_CAPTION"), OVF.GetContentMessage("RASD_16_DESCRIPTION"));
                        OVF.SetTargetDeviceInRASD(ovfEnv, vsId, rasdid, vbd.userdevice);
                    }
                    else
                    {
                        try
                        {
                            VDI vdi = connection.Resolve(vbd.VDI);
                            if (vdi != null)
                            {
                                var diskFilename = string.Format(@"{0}.vhd", vdi.uuid);
                                var diskPath = Path.Combine(targetPath, diskFilename);

                                if (File.Exists(diskPath))
                                {
                                    var oldFileName = diskFilename;
                                    diskFilename = string.Format(@"{0}_{1}.vhd", vdi.uuid, Thread.CurrentThread.ManagedThreadId);
                                    diskPath = Path.Combine(targetPath, diskFilename);
                                    log.InfoFormat("{0}: VHD Name collision, renamed {1} to {2}", ovfname, oldFileName, diskFilename);
                                }

                                if (!metaDataOnly)
                                {
                                    var taskRef = Task.create(connection.Session, "export_raw_vdi_task", "export_raw_vdi_task");
                                    HTTP_actions.get_export_raw_vdi(
                                        b => OnUpdate($"Exporting {diskFilename} ({Util.DiskSizeString(b)} done)"),
                                        onCancel, XenAdminConfigManager.Provider.GetProxyTimeout(true),
                                        connection.Hostname, XenAdminConfigManager.Provider.GetProxyFromSettings(connection),
                                        diskPath, taskRef, connection.Session.opaque_ref, vdi.uuid, "vhd");

                                    if (verifyDisks)
                                    {
                                        //TODO
                                    }
                                }

                                string diskId = Guid.NewGuid().ToString();
                                string diskName = vdi.name_label;
                                if (string.IsNullOrEmpty(diskName))
                                    diskName = string.Format("{0} {1}", OVF.GetContentMessage("RASD_19_CAPTION"), diskIndex);

                                OVF.AddDisk(ovfEnv, vsId, diskId, diskFilename, vbd.bootable, diskName,
                                    vdi.name_description, (ulong)vdi.physical_utilisation, (ulong)vdi.virtual_size);
                                OVF.SetTargetDeviceInRASD(ovfEnv, vsId, diskId, vbd.userdevice);

                                diskIndex++;
                            }
                        }
                        catch (HTTP.CancelledException)
                        {
                            throw new CancelledException();
                        }
                        catch (Exception ex)
                        {
                            log.Warn($"Failed to export disk for VBD {vbdRef}. Continuing.", ex);
                        }
                    }
                }
                #endregion

				#region ADD XEN SPECIFICS

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
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "HVM_shadow_multiplier", Convert.ToString(vm.HVM_shadow_multiplier), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
	            var platform = vm.platform;
				if (platform != null && platform.Count > 0)
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "platform", string.Join(";", platform.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_3"));
                }
				var nvram = vm.NVRAM;
                if (nvram != null && nvram.Count > 0)
				{
                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "NVRAM", string.Join(";", nvram.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_7"));
                }
                if (!string.IsNullOrEmpty(vm.PV_args))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_args", vm.PV_args, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
                if (!string.IsNullOrEmpty(vm.PV_bootloader))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_bootloader", vm.PV_bootloader, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
                if (!string.IsNullOrEmpty(vm.PV_bootloader_args))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_bootloader_args", vm.PV_bootloader_args, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
                if (!string.IsNullOrEmpty(vm.PV_kernel))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_kernel", vm.PV_kernel, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
                if (!string.IsNullOrEmpty(vm.PV_legacy_args))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_legacy_args", vm.PV_legacy_args, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }
                if (!string.IsNullOrEmpty(vm.PV_ramdisk))
                {
					OVF.AddOtherSystemSettingData(ovfEnv, vsId, "PV_ramdisk", vm.PV_ramdisk, OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }

                if (vm.hardware_platform_version >= 0)
                {
                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "hardware_platform_version", vm.hardware_platform_version.ToString(), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }

                if (!string.IsNullOrEmpty(vm.recommendations))
                {
                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "recommendations", vm.recommendations.ToString(), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }

                if (vm.has_vendor_device)
                {
                    //serialise it with a different name to avoid it being deserialised automatically and getting the wrong type
                    OVF.AddOtherSystemSettingData(ovfEnv, vsId, "VM_has_vendor_device", vm.has_vendor_device.ToString(), OVF.GetContentMessage("OTHER_SYSTEM_SETTING_DESCRIPTION_1"));
                }

                foreach(XenRef<VGPU> gpuRef in vm.VGPUs)
                {
                    VGPU vgpu = connection.Resolve(gpuRef);

                    if (vgpu != null)
                    {
                        var vgpuGroup = connection.Resolve(vgpu.GPU_group);
                        var vgpuType = connection.Resolve(vgpu.type);

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
                var allProxies = connection.Cache.PVS_proxies;

                foreach (var p in allProxies.Where(p => p != null && p.VIF != null))
                {
                    var vif = connection.Resolve(p.VIF);
                    if (vif != null)
                    {
                        var vmFromVif = connection.Resolve(vif.VM);
                        if (vmFromVif != null && vmFromVif.uuid == vm.uuid)
                        {
                            var pvsSite = connection.Resolve(p.site);
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
