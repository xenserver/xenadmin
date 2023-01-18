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
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.VMActions
{
    public enum InstallMethod
    {
        None,
        CD,
        Network
    }

    public class CreateVMAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _nameLabel;
        private readonly string _nameDescription;
        private readonly InstallMethod _installMethod;
        private readonly string _pvArgs;
        private readonly VDI _cd;
        private readonly string _url;
        private readonly VmBootMode _bootMode;
        private readonly Host _homeServer;
        private readonly long _vcpusMax;
        private readonly long _vcpusAtStartup;
        private readonly long _memoryDynamicMin, _memoryDynamicMax, _memoryStaticMax;
        private readonly List<DiskDescription> _disks;
        private readonly List<VIF> _vifs;
        private readonly bool _startAfter;
        private readonly Host _copyBiosStringsFrom;
        private readonly SR _fullCopySr;
        private readonly List<VGPU> _vgpus;
        private readonly long _coresPerSocket;
        private readonly string _cloudConfigDriveTemplateText;
        private SR _firstSr;

        private Action<VMStartAbstractAction, Failure> _startDiagnosisForm;
        private Action<VM, bool> _warningDialogHAInvalidConfig;

        private bool _pointOfNoReturn;
        private readonly bool _assignVtpm;
        private readonly bool _assignOrRemoveVgpu;

        /// <summary>
        /// These are the RBAC dependencies that you always need to create a VM. Check CreateVMAction constructor for runtime dependent dependencies.
        /// </summary>
        public static RbacMethodList StaticRBACDependencies = new RbacMethodList(
            // provision VM
            "vm.provision",
            "vm.set_other_config",
            // set VM Params
            "vm.set_name_label",
            "vm.set_name_description",
            "vm.set_VCPUs_max",
            "vm.set_VCPUs_at_startup",
            // set VM Boot Params
            "vm.set_HVM_boot_params",
            "vm.set_PV_args",
            "vm.set_other_config",
            // Add CD Drive
            "vbd.eject",
            "vbd.insert",
            // Create CD Drive
            "vbd.create",
            // Add disks
            "vdi.destroy",
            "vdi.create",
            "vdi.set_sm_config",
            "vbd.create",
            "vbd.destroy",
            "vdi.copy",
            // Add networks
            "vif.create",
            "vm.set_platform",
            //assign vTPM
            "vtpm.create"
        );

        public CreateVMAction(IXenConnection connection, VM template, Host copyBiosStringsFrom,
            string name, string description, InstallMethod installMethod,
            string pvArgs, VDI cd, string url, VmBootMode bootMode, Host homeServer, long vcpusMax, long vcpusAtStartup,
            long memoryDynamicMin, long memoryDynamicMax, long memoryStaticMax,
            List<DiskDescription> disks, SR fullCopySR, List<VIF> vifs, bool startAfter,
            bool assignVtpm, Action<VM, bool> warningDialogHAInvalidConfig,
            Action<VMStartAbstractAction, Failure> startDiagnosisForm,
            List<VGPU> vGpus, bool modifyVgpuSettings, long coresPerSocket, string cloudConfigDriveTemplateText)
            : base(connection, string.Format(Messages.CREATE_VM, name),
            string.Format(Messages.CREATE_VM_FROM_TEMPLATE, name, Helpers.GetName(template)))
        {
            Template = template;
            _copyBiosStringsFrom = copyBiosStringsFrom;
            _fullCopySr = fullCopySR;
            _nameLabel = name;
            _nameDescription = description;
            _installMethod = installMethod;
            _pvArgs = pvArgs;
            _cd = cd;
            _url = url;
            _bootMode = bootMode;
            _homeServer = homeServer;
            _vcpusMax = vcpusMax;
            _vcpusAtStartup = vcpusAtStartup;
            _memoryDynamicMin = memoryDynamicMin;
            _memoryDynamicMax = memoryDynamicMax;
            _memoryStaticMax = memoryStaticMax;
            _disks = disks;
            _vifs = vifs;
            _assignVtpm = assignVtpm;
            _startAfter = startAfter;
            _warningDialogHAInvalidConfig = warningDialogHAInvalidConfig;
            _startDiagnosisForm = startDiagnosisForm;
            _vgpus = vGpus;
            _coresPerSocket = coresPerSocket;
            _cloudConfigDriveTemplateText = cloudConfigDriveTemplateText;

            Pool pool_of_one = Helpers.GetPoolOfOne(Connection);
            if (_homeServer != null || pool_of_one != null) // otherwise we have no where to put the action
                AppliesTo.Add(_homeServer != null ? _homeServer.opaque_ref : pool_of_one.opaque_ref);

            _assignOrRemoveVgpu = vGpus != null && vGpus.Count > 0 || modifyVgpuSettings && Helpers.GpuCapability(Connection);

            #region RBAC Dependencies

            if (_startAfter)
                ApiMethodsToRoleCheck.Add("vm.start");
            if (HomeServerChanged())
                ApiMethodsToRoleCheck.Add("vm.set_affinity");
            if (Template.memory_dynamic_min != _memoryDynamicMin || Template.memory_dynamic_max != _memoryDynamicMax || Template.memory_static_max != _memoryStaticMax)
                ApiMethodsToRoleCheck.Add("vm.set_memory_limits");

            if (_assignOrRemoveVgpu)
            {
                ApiMethodsToRoleCheck.Add("VGPU.destroy");
                ApiMethodsToRoleCheck.Add("VGPU.create");
            }

            ApiMethodsToRoleCheck.AddRange(StaticRBACDependencies);

            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);

            #endregion
        }

        protected override void Run()
        {
            if (_fullCopySr != null)
            {
                Description = string.Format(Messages.COPYING_TEMPLATE, Helpers.GetName(Template), _fullCopySr.Name());
                RelatedTask = VM.async_copy(Session, Template.opaque_ref, Helpers.MakeHiddenName(_nameLabel), _fullCopySr.opaque_ref);
            }
            else
            {
                Description = string.Format(Messages.CLONING_TEMPLATE, Helpers.GetName(Template));
                RelatedTask = VM.async_clone(Session, Template.opaque_ref, Helpers.MakeHiddenName(_nameLabel));
            }

            PollToCompletion(0, 10);

            VM = Connection.WaitForCache(new XenRef<VM>(Result));

            ApplyRecommendationsForVendorDevice();
            CopyBiosStrings();
            SetXenCenterProperties();
            ProvisionVM();
            SetVMParams();
            SetVMBootParams();
            AddCdDrive();
            AddDisks();
            AddNetworks();
            XenAdminConfigManager.Provider.ShowObject(VM.opaque_ref);
            VM.IsBeingCreated = false;
            _pointOfNoReturn = true;

            if (Helpers.ContainerCapability(Connection))
                CloudCreateConfigDrive();
           
            AssignVtpm();
            AssignVgpu();

            if (_startAfter)
            {
                Description = Messages.STARTING_VM;
                var startAction = new VMStartAction(VM, _warningDialogHAInvalidConfig, _startDiagnosisForm);
                startAction.RunAsync();
            }

            Description = Messages.VM_SUCCESSFULLY_CREATED;
        }

        private void ApplyRecommendationsForVendorDevice()
        {
            var pool = Helpers.GetPoolOfOne(Connection);
            bool poolPolicyNoVendorDevice = pool == null || pool.policy_no_vendor_device;
            bool hasVendorDeviceRecommendation = Template.HasVendorDeviceRecommendation();

            if (hasVendorDeviceRecommendation && !poolPolicyNoVendorDevice && !Helpers.FeatureForbidden(VM, Host.RestrictVendorDevice))
            {
                log.DebugFormat("Recommendation (has-vendor-device = true) has been found on the template ({0}) and the host is licensed, so applying it on VM ({1}) being created.", Template.opaque_ref, VM.opaque_ref);
                VM.set_has_vendor_device(Connection.Session, VM.opaque_ref, true);
            }
            else
            {
                log.DebugFormat("Recommendation (has-vendor-device = true) has not been applied on the VM ({0}) being created.", VM.opaque_ref);

                if (!hasVendorDeviceRecommendation)
                    log.DebugFormat("Recommendation (has-vendor-device) is not set or false on the template ({0}).", Template.opaque_ref);

                if (poolPolicyNoVendorDevice)
                    log.DebugFormat("pool.policy_no_vendor_device returned {0}", poolPolicyNoVendorDevice);

                if (Helpers.FeatureForbidden(VM, Host.RestrictVendorDevice))
                    log.DebugFormat("Helpers.FeatureForbidden(VM, Host.RestrictVendorDevice) returned {0}", Helpers.FeatureForbidden(VM, Host.RestrictVendorDevice));
            }
        }

        private void CloudCreateConfigDrive()
        {
            if (Template.CanHaveCloudConfigDrive() && !string.IsNullOrEmpty(_cloudConfigDriveTemplateText))
            {
                Description = Messages.CREATING_CLOUD_CONFIG_DRIVE; 

                var parameters = new Dictionary<string, string>();
                parameters.Add("vmuuid", VM.uuid);
                parameters.Add("sruuid", _firstSr.uuid);
                parameters.Add("configuration", _cloudConfigDriveTemplateText.Replace("\r\n", "\n"));

                var action = new RunPluginAction(Connection, _homeServer ?? Helpers.GetCoordinator(Connection),
                            "xscontainer",//plugin
                            "create_config_drive",//function
                            parameters,
                            true); //hidefromlogs

                action.RunSync(Connection.Session);
                var result = action.Result.Replace("\n", Environment.NewLine);
            }
        }

        private void AssignVtpm()
        {
            if (_assignVtpm)
                new NewVtpmAction(Connection, VM).RunSync(Session);
        }

        private void AssignVgpu()
        {
            if (_assignOrRemoveVgpu)
            {
                var newvGpus = new List<VGPU>();
                foreach (var vGpu in _vgpus)
                {
                    newvGpus.Add(new VGPU
                    {
                        GPU_group = new XenRef<GPU_group>(vGpu.GPU_group.opaque_ref),
                        type = new XenRef<VGPU_type>(vGpu.type.opaque_ref),
                        device = vGpu.device
                    });
                }
                var action = new GpuAssignAction(VM, newvGpus);
                action.RunSync(Session);
            }
        }

        private void CopyBiosStrings()
        {
            if (_copyBiosStringsFrom != null && Template.DefaultTemplate())
            {
                VM.copy_bios_strings(Session, VM.opaque_ref, _copyBiosStringsFrom.opaque_ref);
            }
        }

        private void SetXenCenterProperties()
        {
            VM.IsBeingCreated = true;
            XenAdminConfigManager.Provider.HideObject(VM.opaque_ref);
            AppliesTo.Add(VM.opaque_ref);
        }

        private void SetVMParams()
        {
            Description = Messages.SETTING_VM_PROPERTIES;
            VM.set_name_label(Session, VM.opaque_ref, _nameLabel);
            VM.set_name_description(Session, VM.opaque_ref, _nameDescription);
            ChangeVCPUSettingsAction vcpuAction = new ChangeVCPUSettingsAction(VM, _vcpusMax, _vcpusAtStartup);
            vcpuAction.RunSync(Session);

            // set cores-per-socket
            Dictionary<string, string> platform = VM.platform == null ?
                            new Dictionary<string, string>() :
                            new Dictionary<string, string>(VM.platform);
            platform["cores-per-socket"] = _coresPerSocket.ToString();
            VM.set_platform(Session, VM.opaque_ref, platform);

            // Check these values have changed before setting them, as they are RBAC protected
            if (HomeServerChanged())
                VM.set_affinity(Session, VM.opaque_ref, _homeServer != null ? _homeServer.opaque_ref : Helper.NullOpaqueRef);

            if (Template.memory_dynamic_min != _memoryDynamicMin || Template.memory_dynamic_max != _memoryDynamicMax || Template.memory_static_max != _memoryStaticMax)
                VM.set_memory_limits(Session, VM.opaque_ref, Template.memory_static_min, _memoryStaticMax, _memoryDynamicMin, _memoryDynamicMax);
        }

        private bool HomeServerChanged()
        {
            if (_homeServer == null)
            {
                return Template.affinity.opaque_ref != Helper.NullOpaqueRef;
            }
            return _homeServer.opaque_ref != Template.affinity.opaque_ref;
        }

        private void SetVMBootParams()
        {
            if (Template.IsHVM() && (_disks.Count == 0 || _installMethod == InstallMethod.Network)) // CA-46213
            {
                // boot from network
                Dictionary<string, string> hvm_params = VM.HVM_boot_params;
                hvm_params["order"] = GetBootOrderNetworkFirst();
                VM.set_HVM_boot_params(Session, VM.opaque_ref, hvm_params);
            }
            else if (IsEli() && _installMethod == InstallMethod.Network)
            {
                Dictionary<string, string> other_config = VM.other_config;
                string normal_url = IsRhel() ? NormalizeRepoUrlForRHEL(_url) : _url;
                other_config["install-repository"] = normal_url;
                VM.set_other_config(Session, VM.opaque_ref, other_config);
            }
            else if (IsEli() && _installMethod == InstallMethod.CD)
            {
                Dictionary<string, string> other_config = VM.other_config;
                other_config["install-repository"] = "cdrom";
                VM.set_other_config(Session, VM.opaque_ref, other_config);
            }

            if (!Template.IsHVM())
            {
                VM.set_PV_args(Session, VM.opaque_ref, _pvArgs);
            }
            else
            {
                var hvmParams = VM.HVM_boot_params;
                hvmParams["firmware"] = _bootMode == VmBootMode.Bios ? "bios" : "uefi";
                VM.set_HVM_boot_params(Session, VM.opaque_ref, hvmParams);

                var platform = VM.platform;
                platform["secureboot"] = _bootMode == VmBootMode.SecureUefi ? "true" : "false";
                VM.set_platform(Session, VM.opaque_ref, platform);
            }
        }

        private bool IsEli()
        {
            return !Template.IsHVM() && Template.PV_bootloader == "eliloader";
        }

        private bool IsRhel()
        {
            string distro = VM.InstallDistro();
            return distro == "rhel41" || distro == "rhel44" || distro == "rhlike";
        }

        private string NormalizeRepoUrlForRHEL(string url)
        {
            Uri uri = new Uri(url);
            return uri.Scheme == "nfs" ? string.Format("nfs:{0}:{1}", uri.Host, uri.PathAndQuery) : url;
        }

        private void ProvisionVM()
        {
            Description = Messages.PROVISIONING_VM;
            RewriteProvisionXML();
            RelatedTask = VM.async_provision(Session, VM.opaque_ref);

            PollToCompletion(10, 60);
        }

        private void RewriteProvisionXML()
        {
            XmlNode xml = VM.ProvisionXml();

            if (xml == null)
                return;

            // set the new vm's provision xml: remove "disks" entry, as we are going to explicitly create all the disks
            Dictionary<string, string> other_config = VM.other_config;
            other_config.Remove("disks");
            VM.set_other_config(Session, VM.opaque_ref, other_config);
        }

        private void AddCdDrive()
        {
            if (Helpers.CustomWithNoDVD(Template))
                return; // we have skipped the install media page because we are a custom template with no cd drive - the user doesnt want a cd drive

            Description = Messages.CREATE_CD_DRIVE;
            VBD cd_drive = null;
            foreach (VBD vbd in Connection.ResolveAll(VM.VBDs))
            {
                if (vbd.type != vbd_type.CD)
                    continue;

                if ("0123".IndexOf(vbd.userdevice) < 0)  // userdevice is not 0, 1, 2 or 3: these are the valid positions for CD drives
                    continue;

                cd_drive = vbd;
                break;
            }

            if (cd_drive == null)
            {
                cd_drive = CreateCdDrive();
            }

            if (!cd_drive.empty)
            {
                RelatedTask = VBD.async_eject(Session, cd_drive.opaque_ref);
                PollToCompletion(65, 67);
            }

            if (_installMethod == InstallMethod.CD && _cd != null) // obviously don't insert the empty cd
            {
                RelatedTask = VBD.async_insert(Session, cd_drive.opaque_ref, _cd.opaque_ref);
                PollToCompletion(67, 70);
            }
        }

        private VBD CreateCdDrive()
        {
            var devices = VM.get_allowed_VBD_devices(Session, VM.opaque_ref);
            if (devices.Length == 0)
                throw new Exception(Messages.NO_MORE_USERDEVICES);

            VBD vbd = new VBD
            {
                bootable = _installMethod == InstallMethod.CD,
                empty = true,
                unpluggable = true,
                mode = vbd_mode.RO,
                type = vbd_type.CD,
                userdevice = devices.Contains("3") ? "3" : devices[0],
                VM = new XenRef<VM>(VM.opaque_ref)
            };
            RelatedTask = VBD.async_create(Session, vbd);
            PollToCompletion(60, 65);

            return Connection.WaitForCache(new XenRef<VBD>(Result));
        }


        private void AddDisks()
        {
            Description = Messages.CREATING_DISKS;
            List<VBD> vbds = Connection.ResolveAll(VM.VBDs);

            bool firstDisk = true;
            string suspendSr = null;

            double progress = 70;
            double step = 20.0 / _disks.Count;
            foreach (DiskDescription disk in _disks)
            {
                VBD vbd = GetDiskVBD(disk, vbds);
                VDI vdi = null;
                if (vbd != null)
                {
                    vdi = Connection.Resolve(vbd.VDI);
                }
                if (!DiskOk(disk, vbd))
                {
                    if (vbd != null)
                        vdi = MoveDisk(disk, vbd, progress, step);
                    else
                        vdi = CreateDisk(disk, progress, step);
                }

                if (vdi == null)
                    continue;

                if (vdi.name_description != disk.Disk.name_description)
                    VDI.set_name_description(Session, vdi.opaque_ref, disk.Disk.name_description);
                if (vdi.name_label != disk.Disk.name_label)
                    VDI.set_name_label(Session, vdi.opaque_ref, disk.Disk.name_label);

                if (firstDisk)
                {
                    //use the first disk to set the VM.suspend_SR
                    SR vdiSR = Connection.Resolve(vdi.SR);
                    _firstSr = vdiSR;
                    if(vdiSR != null && !vdiSR.HBALunPerVDI())
                        suspendSr = vdi.SR;
                    firstDisk = false;
                }

                progress += step;
            }

			VM.set_suspend_SR(Session, VM.opaque_ref, suspendSr);
        }

        private VBD GetDiskVBD(DiskDescription disk, List<VBD> vbds)
        {
            foreach (VBD vbd in vbds)
            {
                if (disk.Device.userdevice == vbd.userdevice)
                    return vbd;
            }
            return null;
        }

        private bool DiskOk(DiskDescription disk, VBD vbd)
        {
            if (vbd == null)
                return false;

            VDI vdi = Connection.Resolve(vbd.VDI);

            return vdi != null && disk.Disk.SR.opaque_ref == vdi.SR.opaque_ref;
        }

        private VDI MoveDisk(DiskDescription disk, VBD vbd, double progress, double step)
        {
            string old_vdi_ref = vbd.VDI.opaque_ref;

            RelatedTask = VDI.async_copy(Session, vbd.VDI.opaque_ref, disk.Disk.SR.opaque_ref);
            PollToCompletion(progress, progress + 0.25 * step);
            AddVMHint(Connection.WaitForCache(new XenRef<VDI>(Result)));

            VDI new_vdi = Connection.Resolve(new XenRef<VDI>(Result));

            RelatedTask = VBD.async_destroy(Session, vbd.opaque_ref);
            PollToCompletion(progress + 0.25 * step, progress + 0.5 * step);

            RelatedTask = VDI.async_destroy(Session, old_vdi_ref);
            PollToCompletion(progress + 0.5 * step, progress + 0.75 * step);

            CreateVbd(disk, new_vdi, progress + 0.75 * step, progress + step, IsDeviceAtPositionZero(disk));
            return new_vdi;
        }

        /// <summary>
        /// Helper: Check if the disk is at the zeroth position in the VBD list
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        private bool IsDeviceAtPositionZero(DiskDescription disk)
        {
            return disk.Device.userdevice == "0";
        }

        /// <summary>
        /// Create a VDI/disk. 
        /// If disk type is existing use the VDI in disk description 
        /// Otherwise create a new disk (provision it from the SR)
        /// </summary>
        /// <param name="disk"></param>
        /// <param name="progress"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private VDI CreateDisk(DiskDescription disk, double progress, double step)
        {
            VDI vdi;
            bool bootable = false;
            if(disk.Type == DiskDescription.DiskType.Existing)
                vdi = disk.Disk;
            else
            {
                vdi = CreateVdi(disk, progress, progress + 0.75 * step);
                bootable = IsDeviceAtPositionZero(disk) && _installMethod != InstallMethod.CD;
            }

            AddVMHint(vdi);
            CreateVbd(disk, vdi, progress + 0.75 * step, progress + step, bootable);
            return vdi;
        }

        private void AddVMHint(VDI vdi)
        {
            Dictionary<string, string> sm_config = VDI.get_sm_config(Session, vdi.opaque_ref);
            sm_config["vmhint"] = VM.opaque_ref;
            VDI.set_sm_config(Session, vdi.opaque_ref, sm_config);
        }

        private VDI CreateVdi(DiskDescription disk, double progress1, double progress2)
        {
            VDI vdi = new VDI();
            vdi.name_label = disk.Disk.name_label;
            vdi.name_description = disk.Disk.name_description;
            vdi.read_only = false;
            vdi.sharable = false;
            vdi.SR = disk.Disk.SR;
            vdi.type = disk.Disk.type;
            vdi.virtual_size = disk.Disk.virtual_size;
            vdi.sm_config = disk.Disk.sm_config;

            RelatedTask = VDI.async_create(Session, vdi);
            PollToCompletion(progress1, progress2);
            return Connection.WaitForCache(new XenRef<VDI>(Result));
        }

        /// <summary>
        /// Create a VBD
        /// 
        /// ** vbd.bootable **
        /// 1. Windows ignores bootable flag
        /// 2. Eliloader changes the device "0" to bootable when booting linux
        /// </summary>
        /// <param name="disk"></param>
        /// <param name="vdi"></param>
        /// <param name="progress1"></param>
        /// <param name="progress2"></param>
        /// <param name="bootable">Set VBD.bootable to this value - see comments above</param>
        private void CreateVbd(DiskDescription disk, VDI vdi, double progress1, double progress2, bool bootable)
        {
            var devices = VM.get_allowed_VBD_devices(Session, VM.opaque_ref);
            if (devices.Length == 0)
                throw new Exception(Messages.NO_MORE_USERDEVICES);

            VBD vbd = new VBD();
            vbd.SetIsOwner(true);
            vbd.bootable = bootable;
            vbd.empty = false;
            vbd.unpluggable = true;
            vbd.mode = vbd_mode.RW;
            vbd.type = vbd_type.Disk;
            vbd.userdevice = devices.Contains(disk.Device.userdevice) ? disk.Device.userdevice : devices[0];
            vbd.device = "";
            vbd.VM = new XenRef<VM>(VM.opaque_ref);
            vbd.VDI = new XenRef<VDI>(vdi.opaque_ref);
            RelatedTask = VBD.async_create(Session, vbd);
            PollToCompletion(progress1, progress2);
            Connection.WaitForCache(new XenRef<VBD>(Result));
        }

        private void AddNetworks()
        {
            // first of all we need to clear any vifs that we have cloned from the template 
            double progress = 90;
            VIF vif;
            List<VIF> existingTemplateVifs = Connection.ResolveAll(VM.VIFs);
            double step = 5.0 / existingTemplateVifs.Count;
            for (int i = 0; i < existingTemplateVifs.Count; i++)
            {
                vif = existingTemplateVifs[i];
                RelatedTask = VIF.async_destroy(Session, vif.opaque_ref);

                PollToCompletion(progress, progress + step);
                progress += step;
            }

            // then we add the ones the user has specified
            step = 5.0 / _vifs.Count;
            for (int i = 0; i < _vifs.Count; i++)
            {
                vif = _vifs[i];
                var devices = VM.get_allowed_VIF_devices(Session, VM.opaque_ref);

                if (devices.Length < 1)
                {
                    // If we have assigned more VIFs than we have space for then don't try to create them
                    log.Warn("Tried to create more VIFs than the server allows. Ignoring remaining vifs");
                    return;
                }

                VIF new_vif = new VIF();
                new_vif.device = devices.Contains(vif.device) ? vif.device : devices[0];
                new_vif.MAC = vif.MAC;
                new_vif.network = vif.network;
                new_vif.VM = new XenRef<VM>(VM.opaque_ref);
                new_vif.qos_algorithm_type = vif.qos_algorithm_type;
                new_vif.qos_algorithm_params = vif.qos_algorithm_params;
                RelatedTask = VIF.async_create(Session, new_vif);

                PollToCompletion(progress, progress + step);
                progress += step;

                Connection.WaitForCache(new XenRef<VIF>(Result));
            }
        }

        protected override void CleanOnError()
        {
            if (VM != null && !_pointOfNoReturn && Connection.IsConnected)
            {
                try
                {
                    VMDestroyAction.DestroyVM(Session, VM, true);
                }
                catch (Exception e)
                {
                    // if the clean up has failed for whatever reason we just log it and give up.
                    log.Error(e);
                }
            }
        }

        private string GetBootOrderNetworkFirst()
        {
            // add "n" at the beginning of the order string
            if (VM.HVM_boot_params.ContainsKey("order"))
            {
                string order = VM.HVM_boot_params["order"].ToLower();
                int i = order.IndexOf("n");
                switch (i)
                {
                    case -1:
                        return order.Insert(0, "n");
                    case 0:
                        return order;
                    default:
                        return order.Remove(i, 1).Insert(0, "n");
                }
            }

            return "ncd";
        }
    }
    public class DiskDescription
    {
        public VDI Disk;
        public VBD Device;
        public DiskType Type;
        public enum DiskType { New, Existing }

        public DiskDescription(){}

        public DiskDescription(VDI disk, VBD device)
        {
            Disk = disk;
            Device = device;
            Type = DiskType.New;
        }
    }
}
