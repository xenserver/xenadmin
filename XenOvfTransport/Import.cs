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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DiscUtils;
using XenAdmin;
using XenAdmin.Core;
using XenAPI;
using XenOvf;
using XenOvf.Definitions;
using XenOvf.Definitions.XENC;
using XenOvf.Utilities;


namespace XenOvfTransport
{
    public class Import : XenOvfTransportBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private string downloadupdatemsg;
        private string EncryptionClass;
        private int EncryptionKeySize;
        private XenRef<SR> DefaultSRUUID = null;
        private VirtualDisk vhdDisk = null;
        private int vifDeviceIndex = 0;  // used to count number of Networks attached and to be used as the device number.
        private string _currentfilename = null;
        private Exception _downloadexception = null;
        private ulong _filedownloadsize = 0;
        private AutoResetEvent uridownloadcomplete = new AutoResetEvent(false);

        public string ApplianceName { get; set; }

        public bool MetaDataOnly { get; set; }

		#region Constructors

        public Import(Session session)
            : base(session)
        {
        }

		#endregion

		#region PUBLIC

        public void Process(EnvelopeType ovfObj, string pathToOvf, string passcode)
        {
            var xenSession = XenSession;
            if (xenSession == null)
                throw new InvalidOperationException(Messages.ERROR_NOT_CONNECTED);

            vifDeviceIndex = 0;
            string encryptionVersion = null;

            #region CHECK ENCRYPTION
            if (OVF.HasEncryption(ovfObj))
            {
                if (passcode == null)
                {
                	throw new InvalidDataException(Messages.ERROR_NO_PASSWORD);
                }
                string fileuuids = null;
                SecuritySection_Type[] securitysection = OVF.FindSections<SecuritySection_Type>((ovfObj).Sections);
                if (securitysection != null && securitysection.Length >= 0)
                {
                    foreach (Security_Type securitytype in securitysection[0].Security)
                    {
                        if (securitytype.ReferenceList.Items != null && securitytype.ReferenceList.Items.Length > 0)
                        {
                            foreach (XenOvf.Definitions.XENC.ReferenceType dataref in securitytype.ReferenceList.Items)
                            {
                                if (dataref is DataReference)
                                {
                                    fileuuids += ":" + ((DataReference)dataref).ValueType;
                                }
                            }
                        }
                        if (securitytype.EncryptionMethod != null && securitytype.EncryptionMethod.Algorithm != null)
                        {
                            string algoname = (securitytype.EncryptionMethod.Algorithm.Split('#'))[1].ToLower().Replace('-', '_');
                            object x = OVF.AlgorithmMap(algoname);
                            if (x != null)
                            {
                                EncryptionClass = (string)x;
                                EncryptionKeySize = Convert.ToInt32(securitytype.EncryptionMethod.KeySize);
                            }
                        }

                        if (!string.IsNullOrEmpty(securitytype.version))
                        {
                            encryptionVersion = securitytype.version;                                
                        }
                    }
                }
            }
            #endregion

            #region FIND DEFAULT SR 
            Dictionary<XenRef<Pool>, Pool> pools = Pool.get_all_records(xenSession);
            foreach (XenRef<Pool> pool in pools.Keys)
            {
                DefaultSRUUID = pools[pool].default_SR;
                break;
            }
            #endregion

            //Normalise the process
            if (ovfObj.Item is VirtualSystem_Type vstemp)
            {
				ovfObj.Item = new VirtualSystemCollection_Type();
				((VirtualSystemCollection_Type)ovfObj.Item).Content = new Content_Type[] { vstemp };
            }

			#region Create appliance

        	XenRef<VM_appliance> applRef = null;
			if (ApplianceName != null)
			{
                var vmAppliance = new VM_appliance {name_label = ApplianceName};
				applRef = VM_appliance.create(xenSession, vmAppliance);
			}

            StartupSection_TypeItem[] vmStartupSections = null;
            if (ovfObj.Sections != null)
            {
                StartupSection_Type[] startUpArray = OVF.FindSections<StartupSection_Type>(ovfObj.Sections);
                if (startUpArray != null && startUpArray.Length > 0)
                    vmStartupSections = startUpArray[0]?.Item;
            }

        	#endregion

            foreach (Content_Type contentType in ((VirtualSystemCollection_Type)ovfObj.Item).Content)
            {
                if (!(contentType is VirtualSystem_Type vSystem))
                    continue;

                var vmName = OVF.FindSystemName(ovfObj, vSystem.id);
				VirtualHardwareSection_Type vhs = OVF.FindVirtualHardwareSectionByAffinity(ovfObj, vSystem.id, "xen");
                var vmStartupSection = vmStartupSections?.FirstOrDefault(it => it.id == vSystem.id);

                XenRef<VM> vmRef = CreateVm(xenSession, vmName, vhs, applRef, vmStartupSection);
                if (vmRef == null)
                {
                    log.Error("Failed to create a VM");
                    throw new Exception(Messages.ERROR_CREATE_VM_FAILED);
				}

                log.DebugFormat("OVF.Import.Process: DefineSystem completed ({0})", VM.get_name_label(xenSession, vmRef));

                #region Set vgpu
                
                List<Tuple<GPU_group, VGPU_type> >vgpus = FindGpuGroupAndVgpuType(xenSession, vhs);
                foreach (var item in vgpus)
                {
                    GPU_group gpuGroup = item.Item1;
                    VGPU_type vgpuType = item.Item2;
                    if (gpuGroup != null)
                    {
                        var other_config = new Dictionary<string, string>();

                        if (Helpers.FeatureForbidden(xenSession, Host.RestrictVgpu))
                        {
                            //If the current licence does not allow vGPU, we create one pass-through vGPU
                            //(default vGPU type) for the VM (multiple pass-through vGPUs are not supported yet)

                            log.Debug("The host license does not support vGPU, create one pass-through vGPU for the VM");
                            VGPU.create(xenSession, vmRef.opaque_ref, gpuGroup.opaque_ref, "0", other_config);
                            break;
                        }
                        else if (vgpuType != null)
                            VGPU.create(xenSession, vmRef.opaque_ref, gpuGroup.opaque_ref, "0", other_config, vgpuType.opaque_ref);
                    }
                }

                #endregion

                SetDeviceConnections(ovfObj, vhs);
                try
                {
                    foreach (RASD_Type rasd in vhs.Item)
                    {
                        string thisPassCode = null;
                        // Check to see if THIS rasd is encrypted, if so, set the passcode.
                        if (OVF.IsThisEncrypted(ovfObj, rasd))
                            thisPassCode = passcode;

                        string compression = "None";
                        if (rasd.ResourceType.Value == 17 || rasd.ResourceType.Value == 19 || rasd.ResourceType.Value == 21)
                        {
							bool skip = Tools.ValidateProperty("Caption", rasd) &&
								 (
								  rasd.Caption.Value.ToUpper().Contains("COM") ||
								  rasd.Caption.Value.ToUpper().Contains("FLOPPY") ||
								  rasd.Caption.Value.ToUpper().Contains("ISO")
								 );

                            if (!skip)
                            {
                                File_Type file = OVF.FindFileReferenceByRASD(ovfObj, rasd);
								if (file == null)
									continue;
                                
								if (IsKnownURIType(file.href))
                                    _filedownloadsize = file.size;

                                VirtualDiskDesc_Type vdisk = OVF.FindDiskReference(ovfObj, rasd);
								SetIfDeviceIsBootable(ovfObj, rasd);
								AddResourceSettingData(xenSession, vmRef, rasd, pathToOvf, OVF.FindRasdFileName(ovfObj, rasd, out compression), compression, encryptionVersion, thisPassCode);
                            }
                        }
                        else
                        {
							AddResourceSettingData(xenSession, vmRef, rasd, pathToOvf, OVF.FindRasdFileName(ovfObj, rasd, out compression), compression, encryptionVersion, thisPassCode);
                        }
                    }

                    InstallSection_Type[] installSection = OVF.FindSections<InstallSection_Type>(vSystem.Items);

                    if (installSection != null && installSection.Length == 1)
                    {
                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, Messages.START_POST_INSTALL_INSTRUCTIONS));
                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, Messages.START_POST_INSTALL_INSTRUCTIONS));
                        HandleInstallSection(xenSession, vmRef, installSection[0]);
                    }
                    ShowSystem(xenSession, vmRef);

                    #region PVS Proxy
                    var site = FindPvsSite(xenSession, vhs);

                    if (site != null)
                    {
                        var vm =  xenSession.Connection.Resolve(vmRef);
                        if (vm != null)
                        {
                            var vifs = xenSession.Connection.ResolveAll(vm.VIFs);
                            var firstVif = vifs.FirstOrDefault(v => v.device.Equals("0"));

                            if (firstVif != null)
                            {
                                var foundSite = PVS_site.get_by_uuid(xenSession, site.uuid);

                                if (foundSite != null)
                                {
                                    PVS_proxy.create(xenSession, foundSite.opaque_ref, firstVif.opaque_ref);
                                }
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
					if (ex is OperationCanceledException)
						throw;
                    log.Error("Import failed", ex);
                    throw new Exception(Messages.ERROR_IMPORT_FAILED, ex);
                }
            }

            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, Messages.COMPLETED_IMPORT));
        }

    	#endregion

        #region PRIVATE

        private void HandleInstallSection(Session xenSession, XenRef<VM> vm, InstallSection_Type installsection)
        {
            // Configure for XenServer as requested by OVF.SetRunOnceBootCDROM() with the presence of a post install operation that is specific to XenServer.
            if (installsection.PostInstallOperations != null)
                ConfigureForXenServer(xenSession, vm);

            // Run the VM for the requested duration if this appliance had its own install section -- one not added to fixup for XenServer.
            if (installsection.Info == null ||
                installsection.Info != null && installsection.Info.Value.CompareTo("ConfigureForXenServer") != 0)
                InstallSectionStartVirtualMachine(xenSession, vm, installsection.initialBootStopDelay);
        }

        private void ConfigureForXenServer(Session xenSession, XenRef<VM> vm)
        {
            // Ensure the new VM is down.
            if (VM.get_power_state(xenSession, vm) != vm_power_state.Halted)
                VM.hard_shutdown(xenSession, vm);

            while (VM.get_power_state(xenSession, vm) != vm_power_state.Halted)
                Thread.Sleep(Properties.Settings.Default.FixupPollTimeAsMs);

            // Save its original memory configuration.
            long staticMemoryMin  = VM.get_memory_static_min(xenSession, vm);
            long staticMemoryMax  = VM.get_memory_static_max(xenSession, vm);
            long dynamicMemoryMin = VM.get_memory_dynamic_min(xenSession, vm);
            long dynamicMemoryMax = VM.get_memory_dynamic_max(xenSession, vm);

            // Minimize the memory capacity for the fixup OS.
            long fixupMemorySize = Properties.Settings.Default.FixupOsMemorySizeAsMB * Util.BINARY_MEGA;

            VM.set_memory_limits(xenSession, vm, fixupMemorySize, fixupMemorySize, fixupMemorySize, fixupMemorySize);

            // Run the fixup OS on the VM.
            InstallSectionStartVirtualMachine(xenSession, vm, Properties.Settings.Default.FixupDurationAsSeconds);

            // Restore the original memory configuration.
            VM.set_memory_limits(xenSession, vm, staticMemoryMin, staticMemoryMax, dynamicMemoryMin, dynamicMemoryMax);

            // Eject the fixupOS CD.
            List<XenRef<VBD>> vbdList = VM.get_VBDs(xenSession, vm);
            foreach (XenRef<VBD> vbd in vbdList)
            {
                if (VBD.get_type(xenSession, vbd) == vbd_type.CD)
                    VBD.eject(xenSession, vbd);

                // Note that the original code did not destroy the VBD representing the CD drive.
            }

            // Restore the boot order.
            Dictionary<string, string> bootParameters = new Dictionary<string, string>();
            bootParameters.Add("order", "cnd");
            VM.set_HVM_boot_params(xenSession, vm, bootParameters);
        }

        private void InstallSectionStartVirtualMachine(Session xenSession, XenRef<VM> vm, int initialBootStopDelayAsSeconds)
        {
            log.InfoFormat("Running fixup on VM with opaque_ref {0}", vm.opaque_ref);

            // Start the VM.
            if (VM.get_power_state(xenSession, vm) != vm_power_state.Running)
                VM.start(xenSession, vm, false, true);

            // Finish early if requested to stop on its own.
            if (initialBootStopDelayAsSeconds == 0)
                return;

            // Wait for it to start.
            while (VM.get_power_state(xenSession, vm) != vm_power_state.Running)
                Thread.Sleep(Properties.Settings.Default.FixupPollTimeAsMs);

            // Let it run for the requested duration.
            int bootStopDelayAsMs = initialBootStopDelayAsSeconds * 1000;
            int msRunning = 0;

            while (VM.get_power_state(xenSession, vm) == vm_power_state.Running)
            {
                Thread.Sleep(Properties.Settings.Default.FixupPollTimeAsMs);
                msRunning += Properties.Settings.Default.FixupPollTimeAsMs;

                if (msRunning > bootStopDelayAsMs)
                    break;
            }

            // Ensure it is off.
            if (VM.get_power_state(xenSession, vm) == vm_power_state.Halted)
                return;

            try
            {
                VM.hard_shutdown(xenSession, vm);
            }
            catch (Exception e)
            {
                log.InfoFormat("Unable to hard-shutdown VM {0}. Will ignore error: {1}", vm.opaque_ref, e.Message);
            }
        }

        private XenRef<VDI> ImportFile(Session xenSession, string vmname, string pathToOvf, string filename, string compression, string version, string passcode, string sruuid, string description, string vdiuuid)
        {
            string sourcefile = filename;
            string encryptfilename = null;
            string uncompressedfilename = null;
            string StartPath = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(pathToOvf);
            Stream dataStream = null;
            long dataCapacity = 0;

            #region SET UP TRANSPORT
            if (filename != null)
            {
                if (IsKnownURIType(filename))
                {
                    Uri fileUri = new Uri(filename);
                    filename = DownloadFileAsync(fileUri, 0);
                    sourcefile = filename;
                }

                if (File.Exists(filename))
                {
                    string ext = Path.GetExtension(filename);

                    try
                    {
                        encryptfilename = "enc_" + filename;
                        uncompressedfilename = "unc_" + filename;
                        // OK.. lets see is the file encrypted?
                        #region ENCRYPTION
                        if (passcode != null)
                        {
                            var statusMessage = string.Format(Messages.START_FILE_DECRYPTION, filename);
                            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Security, statusMessage));
                            log.Debug($"Decrypting {filename}");
                            OVF.DecryptToTempFile(EncryptionClass, filename, version, passcode, encryptfilename);
                            sourcefile = encryptfilename;
                            statusMessage += Messages.COMPLETE;
                            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Security, statusMessage));
                        }
                        #endregion 

                        #region COMPRESSION
                        // Identity == no compression, it is meant when a URL is used to identify the compression during transport.
                        if (compression != null && 
                            !compression.ToLower().Equals("none") && 
                            !compression.ToLower().Equals("identity"))
                        {
                            // gz is the only understood 'compressed' format, strip it..
                            // the OVF is marked with "compressed=gzip" therefor it will get decompress
                            // correctly and use with its disk extension (vmdk/vhd/vdi)...
                            if (ext.ToLower().EndsWith(".gz"))
                            {
                                string newfilename = Path.GetFileNameWithoutExtension(uncompressedfilename);
                                uncompressedfilename = newfilename;
                                ext = Path.GetExtension(uncompressedfilename);
                            }
                            var statusMessage = string.Format(Messages.START_FILE_EXPANSION, filename);
                            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Compression, statusMessage));
                        	var ovfCompressor = new OvfCompressor();
							ovfCompressor.UncompressFile(sourcefile, uncompressedfilename, compression);
                            if (File.Exists(encryptfilename)) { File.Delete(encryptfilename); }
                            sourcefile = uncompressedfilename;
                            statusMessage += Messages.COMPLETE;
                            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Compression, statusMessage));
                        }
                        #endregion

                        #region DISK SELECTION
                        bool knownDisk = false;
                        foreach (string diskext in VirtualDisk.SupportedDiskFormats)
                        {
                            if (ext.ToLower().Contains(diskext.ToLower()))
                            {
                                knownDisk = true;
                                break;
                            }
                        }
                        if (knownDisk)
                        {
                            log.DebugFormat("Found file {0} using {1} Stream", filename, ext);
                            vhdDisk = VirtualDisk.OpenDisk(sourcefile, FileAccess.Read);
                            dataStream = vhdDisk.Content;
                            dataCapacity = vhdDisk.Capacity;
                        }
                        else if (ext.ToLower().EndsWith("iso"))
                        {
                            if (string.IsNullOrEmpty(sruuid))
                            {
                                log.Info("ImportFile: Upload Skipped");
                                return null;
                            }
                            else
                            {
                                dataStream = File.OpenRead(filename);
                                dataCapacity = dataStream.Length;
                            }
                        }
                        else
                        {
                            throw new IOException(string.Format(Messages.UNSUPPORTED_FILE_TYPE, ext));
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failed to open virtual disk", ex);
                        throw new Exception(Messages.ISCSI_ERROR_CANNOT_OPEN_DISK, ex);
                    }
                }
                else
                {
                    throw new FileNotFoundException(string.Format(Messages.FILE_MISSING, filename));
                }
            }
            else
            {
                log.Error("The file to import was not provided");
                throw new InvalidDataException(Messages.ERROR_FILE_NAME_NULL);
            }
            #endregion
                        
            try
            {
                #region SEE IF TARGET SR HAS ENOUGH SPACE
                long freespace;
                string contenttype = string.Empty;
                if(vdiuuid != null)
                {
                    XenRef<VDI> vdiLookup = VDI.get_by_uuid(xenSession, vdiuuid);
                    freespace = VDI.get_virtual_size(xenSession, vdiLookup);
                }
                else
                {
                    XenRef<SR> srRef = SR.get_by_uuid(xenSession, sruuid);
                    long size = SR.get_physical_size(xenSession, srRef);
                    long usage = SR.get_physical_utilisation(xenSession, srRef);
                    contenttype = SR.get_content_type(xenSession, srRef);
                    freespace = size - usage;
                }

                if (freespace <= dataCapacity)
                {
                    log.Error($"SR {sruuid} does not have {vhdDisk.Capacity} bytes of free space to import virtual disk {filename}.");
                    string message = string.Format(Messages.NOT_ENOUGH_SPACE_IN_SR, sruuid, Convert.ToString(vhdDisk.Capacity), filename);
                    throw new IOException(message);
                }
                #endregion

                #region UPLOAD FILE
                XenRef<VDI> vdiRef = null;

                //If no VDI uuid is provided create a VDI, otherwise use the one provided as
                //the target for the import. Used for SRs such as Lun per VDI (PR-1544)
                if (string.IsNullOrEmpty(vdiuuid))
                {
                    vdiRef = CreateVDI(xenSession, sruuid, vmname, dataCapacity, description);
                    vdiuuid = VDI.get_uuid(xenSession, vdiRef);
                }
                else
                {
                    vdiRef = new XenRef<VDI>(VDI.get_by_uuid(xenSession, vdiuuid));
                }

                var taskRef = Task.create(xenSession, "import_raw_vdi_task", "import_raw_vdi_task");
                var uriBuilder = new UriBuilder
                {
                    Scheme = XenSession.Connection.UriScheme,
                    Host = XenSession.Connection.Hostname,
                    Port = XenSession.Connection.Port,
                    Path = "/import_raw_vdi",
                    Query = string.Format("session_id={0}&task_id={1}&vdi={2}",
                        xenSession.opaque_ref, taskRef.opaque_ref, vdiuuid)
                };

                using (Stream outStream = HTTPHelper.PUT(uriBuilder.Uri, dataStream.Length, true))
                    HTTP.CopyStream(dataStream, outStream,
                        b => OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import,
                            $"Importing disk {filename} ({Util.DiskSizeString(b)} of {Util.DiskSizeString(dataCapacity)} done)")),
                        () => XenAdminConfigManager.Provider.ForcedExiting);
                #endregion

                return vdiRef;
            }
            catch (Exception ex)
            {
				if (ex is OperationCanceledException)
					throw;
				throw new Exception(Messages.FILE_TRANSPORT_FAILED, ex);
            }
            finally
            {
                if (vhdDisk != null)
                {
                    vhdDisk.Dispose();
                    vhdDisk = null;
                }

                try
                {
                    if (File.Exists(encryptfilename))
                        File.Delete(encryptfilename);

                    if (File.Exists(uncompressedfilename))
                        File.Delete(uncompressedfilename);
                }
                catch
                {
                    //ignore errors
                }

                Directory.SetCurrentDirectory(StartPath);
            }
        }

        private XenRef<VDI> CreateVDI(Session xenSession, string sruuid, string label, long capacity, string description)
        {
            VDI vdi = new VDI
            {
                name_label = label,
                name_description = description,
                SR = sruuid.ToLower().StartsWith("opaque") ? new XenRef<SR>(sruuid) : SR.get_by_uuid(xenSession, sruuid),
                virtual_size = capacity,
                physical_utilisation = capacity,
                type = vdi_type.user,
                sharable = false,
                read_only = false,
                storage_lock = false,
                managed = true,
                is_a_snapshot = false
            };

            try
            {
                // Note that XenServer will round the capacity up to the nearest multiple of a 2 MB block.
                var vdiRef = VDI.create(xenSession, vdi);
                log.Debug("Import.CeateVDI::VDI Created");
                return vdiRef;
            }
            catch (Exception ex)
            {
                log.Error("Failed to create VDI. ", ex);
                throw new Exception(Messages.ERROR_CANNOT_CREATE_VDI, ex);
            }
        }

        private XenRef<VM> CreateVm(Session xenSession, string vmName, VirtualHardwareSection_Type system, XenRef<VM_appliance> applRef, StartupSection_TypeItem vmStartupSection)
        {
            string description = system.System?.Description?.Value ?? Messages.DEFAULT_IMPORT_DESCRIPTION;

            #region MEMORY

            ulong memorySize = 0;
            RASD_Type[] rasds = OVF.FindRasdByType(system, 4);

            if (rasds != null && rasds.Length > 0)
            {
                //The default memory unit is MB (2^20), however, the RASD may contain a different
                //one with format Bytes*memoryBase^memoryPower (Bytes being a literal string)

                double memoryPower = 20.0;
                double memoryBase = 2.0;
               
                foreach (RASD_Type rasd in rasds)
                {
                    if (rasd.AllocationUnits.Value.ToLower().StartsWith("bytes"))
                    {
                        string[] a1 = rasd.AllocationUnits.Value.Split('*', '^');
                        if (a1.Length == 3)
                        {
                            memoryBase = Convert.ToDouble(a1[1]);
                            memoryPower = Convert.ToDouble(a1[2]);
                        }
                    }

                    double memoryMultiplier = Math.Pow(memoryBase, memoryPower);
                    memorySize += rasd.VirtualQuantity.Value * Convert.ToUInt64(memoryMultiplier);
                }
            }

            ulong minimumMemory = 512 * Util.BINARY_MEGA; //default minimum

            if (memorySize < minimumMemory)
                memorySize = minimumMemory;
            else if (memorySize > long.MaxValue)
                memorySize = long.MaxValue;

            #endregion

            #region CPU COUNT

            ulong cpuCount = 0;
            rasds = OVF.FindRasdByType(system, 3);

            if (rasds != null && rasds.Length > 0)
            {
                //There may be more than one entries corresponding to CPUs
                //The VirtualQuantity in each one is Cores

                foreach (RASD_Type rasd in rasds)
                    cpuCount += rasd.VirtualQuantity.Value;
            }

            if (cpuCount < 1) //default minimum
                cpuCount = 1;
            else if (cpuCount > long.MaxValue) //unlikely, but better be safe
                cpuCount = long.MaxValue;

            #endregion

            VM newVm = new VM
            {
                name_label = vmName ?? Messages.UNDEFINED_NAME_LABEL,
                name_description = description,
                user_version = 1,
                is_a_template = false,
                is_a_snapshot = false,
                memory_target = (long)memorySize,
                memory_static_max = (long)memorySize,
                memory_dynamic_max = (long)memorySize,
                memory_dynamic_min = (long)memorySize,
                memory_static_min = (long)memorySize,
                VCPUs_max = (long)cpuCount,
                VCPUs_at_startup = (long)cpuCount,
                actions_after_shutdown = on_normal_exit.destroy,
                actions_after_reboot = on_normal_exit.restart,
                actions_after_crash = on_crash_behaviour.restart,
                HVM_shadow_multiplier = 1.0,
                ha_always_run = false,
                other_config = new Dictionary<string, string> {{"HideFromXenCenter", "true"}}
            };

            //Note that the VM has to be created hidden.
            //We'll make it visible in the end, after all the setup is done

            #region XEN SPECIFIC CONFIGURATION INFORMATION

            if (system.VirtualSystemOtherConfigurationData == null || system.VirtualSystemOtherConfigurationData.Length <= 0)
            {
                // DEFAULT should work for all of HVM type or 301
                newVm.HVM_boot_policy = Properties.Settings.Default.xenBootOptions;
                newVm.HVM_boot_params = SplitStringIntoDictionary(Properties.Settings.Default.xenBootParams);
				newVm.platform = MakePlatformHash(Properties.Settings.Default.xenPlatformSetting);
            }
            else
            {
                var hashtable = new Hashtable();
                foreach (Xen_ConfigurationSettingData_Type xcsd in system.VirtualSystemOtherConfigurationData)
                {
                    string key = xcsd.Name.Replace('-', '_');
                    switch (key.ToLower())
                    {
                        case "hvm_boot_params":
                            var xcsdValue = xcsd.Value.Value;
                            //In new OVFs the xcsd.Value.Value is a dictionary string like "key1=value1;key2=value2"
                            //However, we want to be backwards compatible with old OVFs where it was a plain string
                            newVm.HVM_boot_params = xcsdValue.IndexOf('=') > -1
                                ? SplitStringIntoDictionary(xcsdValue)
                                : new Dictionary<string, string> {{"order", xcsdValue}};
							break;
                        case "platform":
                            newVm.platform = MakePlatformHash(xcsd.Value.Value);
                            break;
	                    case "nvram":
		                    newVm.NVRAM = SplitStringIntoDictionary(xcsd.Value.Value);
		                    break;
                        case "vgpu":
                            //Skip vGPUs here; we'll set them up after the VM is created
                            //because we need the VM's opaque_ref for them
                            break;
                        default:
                            hashtable.Add(key, xcsd.Value.Value);
                            break;
                    }
                }
                newVm.UpdateFrom(hashtable);
            }

            #endregion

            #region Set appliance

            if (applRef != null)
                newVm.appliance = applRef;

            if (vmStartupSection != null)
            {
                newVm.start_delay = vmStartupSection.startDelay;
                newVm.shutdown_delay = vmStartupSection.stopDelay;
                newVm.order = vmStartupSection.order;
            }

            #endregion

            #region set has_vendor_device

            if (Helpers.DundeeOrGreater(xenSession.Connection))
            {
                var data = system.VirtualSystemOtherConfigurationData;
                var datum = data?.FirstOrDefault(s => s.Name == "VM_has_vendor_device");
                if (datum != null)
                {
                    if (bool.TryParse(datum.Value.Value, out var hasVendorDevice) && hasVendorDevice)
                        newVm.has_vendor_device = hasVendorDevice;
                }
            }

            #endregion

            return VM.create(xenSession, newVm);
        }

        public static Regex VGPU_REGEX = new Regex("^GPU_types={(.*)};VGPU_type_vendor_name=(.*);VGPU_type_model_name=(.*);$");

        public static Regex PVS_SITE_REGEX = new Regex("^PVS_SITE={uuid=(.*)};$");

        private List<Tuple<GPU_group, VGPU_type>> FindGpuGroupAndVgpuType(Session xenSession, VirtualHardwareSection_Type system)
        {
            List<Tuple<GPU_group, VGPU_type>> vgpus = new List<Tuple<GPU_group, VGPU_type>>();

            var data = system.VirtualSystemOtherConfigurationData;
            if (data == null)
                return vgpus;

            var datum = data.Where(s => s.Name == "vgpu");

            foreach (var item in datum)
            {
                Match m = VGPU_REGEX.Match(item.Value.Value);
                if (!m.Success)
                    continue;
                var types = m.Groups[1].Value.Split(';');

                var gpuGroups = GPU_group.get_all_records(xenSession);
                var gpuKvp = gpuGroups.FirstOrDefault(g =>
                    g.Value.supported_VGPU_types.Count > 0 &&
                    g.Value.GPU_types.Length == types.Length &&
                    g.Value.GPU_types.Intersect(types).Count() == types.Length);

                if (gpuKvp.Equals(default(KeyValuePair<XenRef<GPU_group>, GPU_group>)))
                    continue;

                var gpuGroup = gpuKvp.Value;
                VGPU_type vgpuType = null;

                gpuGroup.opaque_ref = gpuKvp.Key.opaque_ref;

                string vendorName = m.Groups[2].Value;
                string modelName = m.Groups[3].Value;

                var vgpuTypes = VGPU_type.get_all_records(xenSession);
                var vgpuKey = vgpuTypes.FirstOrDefault(v =>
                    v.Value.vendor_name == vendorName && v.Value.model_name == modelName);

                if (!vgpuKey.Equals(default(KeyValuePair<XenRef<VGPU_type>, VGPU_type>)))
                {
                    vgpuType = vgpuKey.Value;
                    vgpuType.opaque_ref = vgpuKey.Key.opaque_ref;
                }
                vgpus.Add(new Tuple<GPU_group, VGPU_type>(gpuGroup, vgpuType));
            }
            return vgpus;
        }

        private PVS_site FindPvsSite(Session xenSession, VirtualHardwareSection_Type system)
        {
            var data = system.VirtualSystemOtherConfigurationData;
            if (data == null)
                return null;

            var datum = data.FirstOrDefault(s => s.Name == "pvssite");
            if (datum == null)
                return null;

            Match m = PVS_SITE_REGEX.Match(datum.Value.Value);
            if (!m.Success)
                return null;

            var siteUuid = m.Groups[1].Value;

            var allSites = PVS_site.get_all_records(xenSession);

            var site = allSites.Select(kvp => kvp.Value).FirstOrDefault(p => p.uuid == siteUuid);

            return site;
        }

        private void RemoveSystem(Session xenSession, XenRef<VM> vm)
        {
            try
            {
                VM.destroy(xenSession, vm);
            }
            catch (Exception ex)
            {
                log.Error("Failed to remove a virtual machine (VM). ", ex);
                throw new Exception(Messages.ERROR_REMOVE_VM_FAILED, ex);
            }
        }

        private static Dictionary<string, string> SplitStringIntoDictionary(string inputStr)
        {
			/* Comment for the usage o.Split(new[] { '=' }, 2)
			 * The second parameter "2" is used to handle the case when the delimiter '=' appears in the content of the string
			 *
			 * For example, inputStr = "EFI-variables-backend=xapidb;EFI-variabbles-on-boot=reset;EFI-variables=dGVzdA=="
			 * Notice there are 2 extra '=' in the last section
			 * The expected output of that section should be { 'EFI-variables':'dGVzdA==' }
			 * But if we do not the second parameter "2" in the Split function, the actual output will be { 'EFI-variables':'' }
			 */
			return inputStr.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Split(new[] { '=' }, 2))
				.ToDictionary(o => o.FirstOrDefault(), o => o.LastOrDefault());
        }

        private Dictionary<string, string> MakePlatformHash(string platform)
        {
            var hPlatform = SplitStringIntoDictionary(platform);

            // Handle the case when NX isn't in the platform string.
            if (!hPlatform.ContainsKey("nx"))
                hPlatform.Add("nx", "true");

            return hPlatform;
        }

        private void AddResourceSettingData(Session xenSession, XenRef<VM> vmRef, RASD_Type rasd, string pathToOvf, string filename, string compression, string version, string passcode)
        {
            switch (rasd.ResourceType.Value)
            {
                case 3: // Processor: Already set in DefineSystem
                case 4: // Memory: Already set in DefineSystem
                case 5: // Internal Disk Controller of one type or another.
                case 6:
                case 7:
                case 8:
                case 9:
                    {
                        // For Xen really nothing to do here, does not support the different
                        // controller types, therefore we must ensure
                        // via positional on controllers.
                        // IDE - #1
                        // SCSI - #2
                        // IDE 0 Disk  0 Goes to Xen: userdevice=0
                        // IDE 0 Disk  1 Goes to Xen: userdevice=1 
                        // IDE 1 Disk  0 Goes to Xen: userdevice=2
                        // IDE 1 CDDVD 1 Goes to Xen: userdevice=3
                        // SCSI 0 Disk 0 Goes to Xen: userdevice=4
                        // SCSI 0 Disk 1 Goes to Xen: userdevice=5
                        // and so forth.
                        break;
                    }
                case 10: // Network
                    {
                        XenRef<Network> net = null;
                        XenRef<Network> netDefault = null;
                        string netuuid = null;

                        #region SELECT NETWORK
                        Dictionary<XenRef<Network>, Network> networks = Network.get_all_records(xenSession);
                        if (rasd.Connection != null && rasd.Connection.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(rasd.Connection[0].Value))
                            {
                                // Ignore the NetworkSection/Network
                                // During Network Selection the UUID for Network was set in Connection Field
                                // Makes data self contained here.

                                if (rasd.Connection[0].Value.Contains(Properties.Settings.Default.xenNetworkKey) ||
                                    rasd.Connection[0].Value.Contains(Properties.Settings.Default.xenNetworkUuidKey))
                                {
                                    string[] s = rasd.Connection[0].Value.Split(',');
                                    for (int i = 0; i < s.Length; i++)
                                    {
                                        if (s[i].StartsWith(Properties.Settings.Default.xenNetworkKey) ||
                                            s[i].StartsWith(Properties.Settings.Default.xenNetworkUuidKey))
                                        {
                                            string[] s1 = s[i].Split('=');
                                            netuuid = s1[1];
                                        }
                                    }
                                }
                                foreach (XenRef<Network> netRef in networks.Keys)
                                {
                                    // if its a UUID and we find it... use it..
                                    if (net == null && netuuid != null && 
                                        netuuid.Equals(networks[netRef].uuid))
                                    {
                                        net = netRef;
                                    }
                                    // Ok second is to match it as a NAME_LABEL
                                    else if (net == null && netuuid != null && 
                                        networks[netRef].name_label.ToLower().Contains(netuuid))
                                    {
                                        net = netRef;
                                    }
                                    // hhmm neither... is it a BRIDGE name?
                                    else if (net == null && netuuid != null && 
                                        networks[netRef].bridge.ToLower().Contains(netuuid))
                                    {
                                        net = netRef;
                                    }
                                    // ok find the default.
                                    if (networks[netRef].bridge.ToLower().Contains(Properties.Settings.Default.xenDefaultNetwork))
                                    {
                                        netDefault = netRef;
                                    }
                                }
                                if (net == null)
                                {
                                    net = netDefault;
                                }
                            }
                        }
                        #endregion

                        #region ATTACH NETWORK TO VM

                        VIF vif = new VIF
                        {
                            uuid = Guid.NewGuid().ToString(),
                            allowed_operations = new List<vif_operations> {vif_operations.attach},
                            device = Convert.ToString(vifDeviceIndex++),
                            network = net,
                            VM = vmRef,
                            MTU = 1500,
                            locking_mode = vif_locking_mode.network_default
                        };

                        // This is MAC address if available use it.
                        // needs to be in form:  00:00:00:00:00:00
                        if (Tools.ValidateProperty("Address", rasd))
                        {
                            StringBuilder networkAddress = new StringBuilder();
                            if (!rasd.Address.Value.Contains(":"))
                            {
                                for (int i = 0; i < rasd.Address.Value.Length; i++)
                                {
                                    if ((i > 0) && (i % 2) == 0)
                                    {
                                        networkAddress.Append(":");
                                    }
                                    networkAddress.Append(rasd.Address.Value[i]);
                                }
                            }
                            if (networkAddress.Length == 0)
                            {
                                networkAddress.Append(rasd.Address.Value);
                            }
                            vif.MAC = networkAddress.ToString();
                        }

                        try
                        {
                            xenSession.Connection.WaitForCache(VIF.create(xenSession, vif));
                        }
                        catch (Exception ex)
                        {
                            log.Error("Failed to create a virtual network interface (VIF). ", ex);
                            throw new Exception(Messages.ERROR_CREATE_VIF_FAILED, ex);
                        }
                        #endregion
                        log.Debug("OVF.Import.AddResourceSettingData: Network Added");

                        break;
                    }
                case 15: // CD Drive
                case 16: // DVD Drive
                    {
                        // We always attach as "EMPTY".
                        // Currently Xen Server can only have ONE CD, so we must 
                        // Skip the others.
                        // If it's not necessary.. skip it.

                        #region Attach DVD to VM
                        bool SkipCD = false;
                        List<XenRef<VBD>> vbds = VM.get_VBDs(xenSession, vmRef);
                        foreach (XenRef<VBD> vbd in vbds)
                        {
                            vbd_type vbdType = VBD.get_type(xenSession, vbd);
                            if (vbdType == vbd_type.CD)
                            {
                                SkipCD = true;
                                break;
                            }
                        }

                        if (!SkipCD)
                        {
                            List<XenRef<VDI>> vdiRefs = new List<XenRef<VDI>>();
                            if (filename != null)
                            {
                                #region FIND THE ISO SR MAPPED IN THE OVF
                                string isoUuid = null;
                                if (rasd.Connection != null && rasd.Connection.Length > 0)
                                {
                                    if (rasd.Connection[0].Value.ToLower().Contains("sr="))
                                    {
                                        string[] vpairs = rasd.Connection[0].Value.Split(',');
                                        foreach (string vset in vpairs)
                                        {
                                            if (vset.ToLower().StartsWith("sr="))
                                            {
                                                var srToFind = vset.Substring(vset.LastIndexOf('=') + 1);

                                                try
                                                {
                                                    XenRef<SR> srRef = SR.get_by_uuid(xenSession, srToFind);
                                                    if (srRef != null && srRef != Helper.NullOpaqueRef)
                                                    {
                                                        isoUuid = srToFind;
                                                        break;
                                                    }
                                                }
                                                catch
                                                {
                                                    log.Debug("Import.AddResourceSettingData: iso sr uuid not found, trying name_label");
                                                }

                                                try
                                                {
                                                    var srRecords = SR.get_all_records(xenSession);

                                                    isoUuid = (from SR sr in srRecords.Values
                                                        where sr.name_label == srToFind
                                                        select sr.uuid).FirstOrDefault();
                                                }
                                                catch
                                                {
                                                    log.Debug("Import.AddResourceSettingData: iso sr uuid not found, looking for vdi...");
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                // VDI trumps SR
                                List<XenRef<VDI>> isoVDIlist = VDI.get_by_name_label(xenSession, filename);
                                if (isoVDIlist.Count > 0)
                                {
                                    vdiRefs.Add(isoVDIlist[0]);
                                }
                                else
                                {
                                    #region LAST CHANCE USE XENTOOLS ISO SR
                                    if (isoUuid == null)
                                    {
                                        Dictionary<XenRef<SR>, SR> srDictionary = SR.get_all_records(xenSession);
                                        foreach (XenRef<SR> key in srDictionary.Keys)
                                        {
                                            if (srDictionary[key].content_type.ToLower() == "iso" && srDictionary[key].type.ToLower() == "iso")
                                            {
                                                if (srDictionary[key].name_label.ToLower().Equals(Properties.Settings.Default.xenTools.ToLower()))
                                                {
                                                    isoUuid = srDictionary[key].uuid;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region DO IMPORT ISO FILE
                                    if (isoUuid != null && !MetaDataOnly)
                                    {
                                        _currentfilename = filename;
                                        try
                                        {
                                            var vdiRef = ImportFile(xenSession, filename, pathToOvf, filename, compression, version, passcode, isoUuid, "", null);
                                            if (vdiRef != null)
                                                vdiRefs.Add(vdiRef);
                                        }
                                        catch (Exception ex)
                                        {
                                            if (ex is OperationCanceledException)
                                                throw;
                                            var msg = string.Format(Messages.ERROR_ADDRESOURCESETTINGDATA_FAILED, Messages.ISO);
                                            log.Error("Failed to add resource ISO.", ex);
                                            throw new Exception(msg, ex);
                                        }
                                        finally
                                        {
                                            if (vdiRefs == null || vdiRefs.Count <= 0)
                                            {
                                                log.ErrorFormat("Failed to import virtual disk from file {0} to storage repository {1}.", filename, isoUuid);
                                                RemoveSystem(xenSession, vmRef);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                vdiRefs.Add(XenRef<VDI>.Create(string.Empty));
                            }

                            #region CREATE VBD CONNECTION
                         
                            foreach (XenRef<VDI> currentVDI in vdiRefs)
                            {
                                var hashtable = new Hashtable();

                                if (rasd.Connection != null && rasd.Connection.Length > 0)
                                {
                                    string[] valuepairs = rasd.Connection[0].Value.Split(',');

                                    foreach (string valuepair in valuepairs)
                                    {
                                        string[] namevalue = valuepair.Split('=');
                                        string name = namevalue[0].ToLower();

                                        switch (name)
                                        {
                                            case "sr":
                                            case "vdi":
                                                continue;
                                            case "device":
                                                hashtable.Add("userdevice", namevalue[1]);
                                                break;
                                            default:
                                                hashtable.Add(namevalue[0], namevalue[1]);
                                                break;
                                        }
                                    }
                                }

                                VBD vbd = new VBD
                                {
                                    VM = vmRef,
                                    mode = vbd_mode.RO,
                                    userdevice = "3",
                                    type = vbd_type.CD,
                                    storage_lock = false,
                                    status_code = 0
                                };

                                vbd.UpdateFrom(hashtable);

                                if (currentVDI != null && !string.IsNullOrEmpty(currentVDI.opaque_ref))
                                {
                                    vbd.VDI = currentVDI;
                                    vbd.empty = false;
                                    vbd.bootable = true;
                                    vbd.unpluggable = true;
                                }
                                else
                                {
                                    vbd.empty = true;
                                }

                                vbd.userdevice = VerifyUserDevice(xenSession, vmRef, vbd.userdevice);
                                vbd.other_config = new Dictionary<string, string> { { "owner", "true" } };

                                if (!vbd.userdevice.EndsWith("+"))
                                {
                                    try
                                    {
                                        VBD.create(xenSession, vbd);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error("Import.AddResourceSettingData: ", ex);
                                    }
                                }
                                else
                                {
                                    log.WarnFormat(
                                        "Import:  ================== ATTENTION NEEDED =======================" +
                                        "Import:  Could not determine appropriate number of device placement." +
                                        "Import:  Please Start, Logon, Shut down, System ({0})" +
                                        "Import:  Then attach disks with labels ending with \"+\" to the device number defined before the +." +
                                        "Import:  ===========================================================", vmRef);

                                    OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, Messages.WARNING_ADMIN_REQUIRED));
                                }
                            }
                            #endregion

                        }
                        #endregion
                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.CdDvdDrive,
							string.Format(Messages.DEVICE_ATTACHED, Messages.CD_DVD_DEVICE)));
                        log.Debug("Import.AddResourceSettingData: CD/DVD ROM Added");

                        break;
                    }
                case 17: // Disk Drive
                case 19: // Storage Extent
                case 21: // Microsoft: Harddisk/Floppy/ISO
                    {
                        #region ADD DISK
                        if (filename == null) // NO disk is available, why import RASD?
                        {
                            log.WarnFormat("No file available to import, skipping: RASD{0}: {1}", rasd.ResourceType.Value, rasd.InstanceID.Value);
                            break;
                        }
                        string sruuid = null;
                        string vdiuuid = null;
                        string userdeviceid = null;
                        string vmName = VM.get_name_label(xenSession, vmRef);
                        bool isbootable = false;
                        vbd_mode mode = vbd_mode.RW;

                        bool importThisRasd = true;
                        if (Tools.ValidateProperty("Caption", rasd)) // rasd.Caption != null && rasd.Caption.Value != null && rasd.Caption.Value.Length > 0)
                        {
                            if (
                                rasd.Caption.Value.ToUpper().Contains("COM") ||
                                rasd.Caption.Value.ToUpper().Contains("FLOPPY") ||
                                rasd.Caption.Value.ToUpper().Contains("ISO")
                                )
                            {
                                importThisRasd = false;
                            }
                        }

                        if (importThisRasd)
                        {
                            #region IMPORT DISKS
                            if (!MetaDataOnly)
                            {
                                _currentfilename = filename;

                                List<XenRef<VDI>> vdiRefs = new List<XenRef<VDI>>();

                                #region PARSE CONNECTION
                                if (Tools.ValidateProperty("Connection", rasd))
                                {
                                    string[] s = rasd.Connection[0].Value.Split('=', ',');
                                    for (int i = 0; i < s.Length; i++)
                                    {
                                        string checkme = s[i].ToLower().Trim();
                                        switch (checkme)
                                        {
                                            case "device":
                                                {
                                                    userdeviceid = s[++i];
                                                    break;
                                                }
                                            case "bootable":
                                                {
                                                    isbootable = Convert.ToBoolean(s[++i]);
                                                    break;
                                                }
                                            case "mode":
                                                {
                                                    if (s[++i].Equals("r"))
                                                    {
                                                        mode = vbd_mode.RO;
                                                    }
                                                    break;
                                                }
                                            case "vdi":
                                                {
                                                    vdiuuid = s[++i];
                                                    break;
                                                }
                                            case "sr":
                                                {
                                                    sruuid = s[++i];
                                                    break;
                                                }
                                        }
                                    }
                                }
                                #endregion

                                #region VERIFY SR UUID
                                if (!string.IsNullOrEmpty(sruuid))
                                {
                                    XenRef<SR> srref = null;
                                    try
                                    {
                                        srref = SR.get_by_uuid(xenSession, sruuid);
                                    }
                                    catch
                                    {
                                        log.Debug("Import.AddResourceSettingData: SR missing... still looking..");
                                    }
                                    if (srref == null)
                                    {
                                        List<XenRef<SR>> srlist = null;
                                        try
                                        {
                                            srlist = SR.get_by_name_label(xenSession, sruuid);
                                        }
                                        catch
                                        {
                                            log.Debug("Import.AddResourceSettingData: SR missing... still looking..");
                                        }
                                        if (srlist != null && srlist.Count > 0)
                                        {
                                            sruuid = SR.get_uuid(xenSession, srlist[0]);
                                        }
                                    }
                                }
                                else
                                {
                                    sruuid = null;
                                }
                                #endregion

                                #region LAST CHANGE TO FIND SR
                                if (sruuid == null)
                                {
                                    if (DefaultSRUUID == null)
                                    {
                                        log.Error("The SR was not found and a default was not assigned.");
                                        throw new InvalidDataException(Messages.ERROR_COULD_NOT_FIND_SR);
                                    }

                                    Dictionary<XenRef<SR>, SR> srDict = SR.get_all_records(xenSession);
                                    if (vdiuuid != null)
                                    {
                                        //Try and get the SR that belongs to the VDI attached
                                        XenRef<VDI> tempVDI = VDI.get_by_uuid(xenSession, vdiuuid);
                                        if (tempVDI == null)
                                        {
                                            log.Error("The SR was not found and a default was not assigned.");
                                            throw new InvalidDataException(Messages.ERROR_COULD_NOT_FIND_SR);
                                        }

                                        XenRef<SR> tempSR = VDI.get_SR(xenSession, tempVDI.opaque_ref);
                                        sruuid = srDict[tempSR].uuid;
                                    }
                                    else
                                        sruuid = srDict[DefaultSRUUID].uuid;
                                }
                                #endregion

                                XenRef<VDI> vdiRef = null;

                                try
                                {
                                    string disklabel = string.Format("{0}_{1}", vmName, userdeviceid);

                                    if ((rasd.ElementName != null) && (!string.IsNullOrEmpty(rasd.ElementName.Value)))
                                        disklabel = rasd.ElementName.Value;

                                    string description = "";

                                    if ((rasd.Description != null) && (!string.IsNullOrEmpty(rasd.Description.Value)))
                                        description = rasd.Description.Value;

                                    vdiRef = ImportFile(xenSession, disklabel, pathToOvf, filename, compression, version, passcode, sruuid, description, vdiuuid);
                                    if (vdiRef != null)
                                        vdiRefs.Add(vdiRef);
                                }
                                catch (Exception ex)
                                {
									if (ex is OperationCanceledException)
										throw;
									var msg = string.Format(Messages.ERROR_ADDRESOURCESETTINGDATA_FAILED, Messages.DISK_DEVICE);
                                    log.Error("Failed to add resource Hard Disk Image.", ex);
                                    throw new InvalidDataException(msg, ex);
                                }
                                finally
                                {
                                    if (vdiRef == null)
                                    {
                                    	log.ErrorFormat("Failed to import virtual disk from file {0} to storage repository {1}.", filename, sruuid);
                                        RemoveSystem(xenSession, vmRef);
                                    }
                                }

                                log.DebugFormat("Import.AddResourceSettingData counts {0} VDIs", vdiRefs.Count);


                                foreach (XenRef<VDI> currentVDI in vdiRefs)
                                {
                                    VBD vbd = new VBD
                                    {
                                        userdevice = VerifyUserDevice(xenSession, vmRef, userdeviceid ?? "99"),
                                        bootable = isbootable,
                                        VDI = currentVDI,
                                        mode = mode,
                                        uuid = Guid.NewGuid().ToString(),
                                        VM = vmRef,
                                        empty = false,
                                        type = vbd_type.Disk,
                                        currently_attached = false,
                                        storage_lock = false,
                                        status_code = 0,
                                        // below other_config keys XS to delete the disk along with the VM.
                                        other_config = new Dictionary<string, string> {{"owner", "true"}}
                                    };

                                    if (!vbd.userdevice.EndsWith("+"))
                                    {
                                        try
                                        {
                                            VBD.create(xenSession, vbd);
                                        }
                                        catch (Exception ex)
                                        {
                                            log.Error("Failed to create a virtual block device (VBD).", ex);
                                            throw new Exception(Messages.ERROR_CREATE_VBD_FAILED, ex);
                                        }
                                    }
                                    else
                                    {
                                        log.WarnFormat(
                                            "Import:  ================== ATTENTION NEEDED =======================" +
                                            "Import:  Could not determine appropriate number for device placement." +
                                            "Import:  Please Start, Logon, Shut down, System ({0})" +
                                            "Import:  Then manually attach disks with labels with {0}_# that are not attached to {0}" +
                                            "Import:  ===========================================================",
                                            vmName);
                                        OnUpdate(new XenOvfTransportEventArgs(TransportStep.Import, Messages.WARNING_ADMIN_REQUIRED));
                                    }
                                }
                            }
                            else
                            {
                                log.InfoFormat("Import: FILE SKIPPED (METADATA ONLY SELECTED)  {0}", _currentfilename);
                            }
                            #endregion
                        }
                        log.Debug("Import.AddResourceSettingData: Hard Disk Image Added");
                        break;
                        #endregion
                    }
            }
        }

        private string VerifyUserDevice(Session xenSession, XenRef<VM> vmRef, string device)
        {
            log.DebugFormat("Import.VerifyUserDevice, checking device: {0} (99 = autoselect)", device);
            string usethisdevice = null;

            string[] allowedVBDs = VM.get_allowed_VBD_devices(xenSession, vmRef);

            if (allowedVBDs == null || allowedVBDs.Length <= 0)
            {
                log.ErrorFormat("OVF.VerifyUserDevice: No more available devices, cannot add device: {0}", device);
                return device + "+";
            }

            if (!string.IsNullOrEmpty(device) && !device.StartsWith("99"))
            {
                foreach (string allowedvbd in allowedVBDs)
                {
                    if (device.ToLower() == allowedvbd.ToLower())
                    {
                        usethisdevice = device;
                        log.DebugFormat("Import.VerifyUserDevice, device: {0} will be used.", device);
                        break;
                    }
                }
            }
            else
            {
                usethisdevice = allowedVBDs[0];
                log.DebugFormat("Import.VerifyUserDevice, device [{0}] is not available, setting to: [{1}]", device, usethisdevice);
            }

            if (usethisdevice == null)
            {
                if (!device.EndsWith("+"))
                    usethisdevice = device + "+";                
            }
            return usethisdevice;
        }

        private void SetDeviceConnections(EnvelopeType ovfEnv, VirtualHardwareSection_Type vhs)
        {
            int[] connections = new int[16];
            int deviceoffset = 0;
            List<RASD_Type> rasdList = new List<RASD_Type>();
            
            rasdList.AddRange(vhs.Item);
            rasdList.Sort(compareControllerRasd);  // sorts based on ResourceType.Value

            // For Xen really nothing to do here, does not support the different
            // controller types, therefore we must ensure
            // via positional on controllers.
            // IDE - #1
            // SCSI - #2
            // IDE 0 Disk 0  Goes to Xen: userdevice=0
            // IDE 0 Disk 1  Goes to Xen: userdevice=1 
            // IDE 1 CD/DVD 0  Goes to Xen: userdevice=2
            // IDE 1 Disk 1  UnUsed
            // SCSI 0 Disk 0 Goes to Xen: userdevice=3
            // SCSI 0 Disk 1 Goes to Xen: userdevice=4
            // and so forth.

            foreach (RASD_Type rasd in rasdList)
            {
                switch (rasd.ResourceType.Value)
                {
                    case 5:  // IDE Controller #1
                    case 6:  // Parallel SCSI HBA #2
                    case 7:  // FC HBA #3
                    case 8:  // iSCSI HBA #4
                    case 9:  // IB HCA #5
                        {
                            List<RASD_Type> connectedrasds = FindConnectedItems(rasd.InstanceID.Value, vhs.Item, null);
                            foreach (RASD_Type _rasd in connectedrasds)
                            {
                                //if (_rasd.Connection != null &&
                                //    _rasd.Connection.Length > 0 &&
                                //    _rasd.Connection[0] != null &&
                                //    _rasd.Connection[0].Value != null &&
                                //    _rasd.Connection[0].Value.Length > 0)
                                if (_rasd.ResourceType.Value == 15 || _rasd.ResourceType.Value == 16)
                                {
                                    deviceoffset = 2;
                                }
                                else
                                {
                                    deviceoffset = 0;
                                }
                                if (Tools.ValidateProperty("Connection", _rasd))
                                {
                                    if (!_rasd.Connection[0].Value.ToLower().Contains("device="))
                                    {
                                       _rasd.Connection[0].Value = string.Format("{0},device={1}", _rasd.Connection[0].Value, FindNextAvailable(deviceoffset, connections, 0));
                                    }
                                }
                                else
                                {
                                    _rasd.Connection = new cimString[] { new cimString(string.Format("device={0}", FindNextAvailable(deviceoffset, connections, 0))) };
                                }
                            }
                            break;
                        }
                }
            }
        }

        private int FindNextAvailable(int offset, int[] ids, int unusedkey)
        {
            int available = 0;
            for (int i = offset; i < ids.Length; i++)
            {
                if (ids[i] == unusedkey)
                {
                    ids[i] = 1;
                    available = i;
                    break;
                }
            }
            return available;
        }

        private List<RASD_Type> FindConnectedItems(string instanceId, RASD_Type[] rasds, string value22)
        {
            List<RASD_Type> connectedRasds = new List<RASD_Type>();
            foreach (RASD_Type rasd in rasds)
            {
                if (rasd.Parent != null && !string.IsNullOrEmpty(rasd.Parent.Value) )
                {
                    string parent = rasd.Parent.Value.Replace(@"\", "");
                    string instance = instanceId.Replace(@"\", "");
                    if (parent.Contains(instance))
                    {
                        switch (rasd.ResourceType.Value)
                        {
                            case 15:
                            case 16:
                                {
                                    connectedRasds.Add(rasd);
                                    break;
                                }
                            case 22: // Check to see if it's Microsoft Synthetic Disk Drive
                                {
                                    if (Tools.ValidateProperty("ResourceSubType", rasd) && 
                                        rasd.ResourceSubType.Value.ToLower().Contains("synthetic")
                                        )
                                    {
                                        connectedRasds.AddRange(FindConnectedItems(rasd.InstanceID.Value, rasds, rasd.Address.Value));
                                    }
                                    break;
                                }
                            case 17: // VMware Hard Disk
                            case 19: // XenServer/XenConvert Storage Extent
                            case 21: // Microsoft Hard Disk Image
                                {
                                    if ((Tools.ValidateProperty("ElementName", rasd) && rasd.ElementName.Value.ToLower().Contains("hard disk")) ||
                                        (Tools.ValidateProperty("Caption", rasd) && rasd.Caption.Value.ToLower().Contains("hard disk")) ||
                                        (Tools.ValidateProperty("Caption", rasd) && rasd.Caption.Value.ToLower().StartsWith("disk"))
                                        )
                                    {
                                        if (value22 != null)
                                        {
                                            rasd.Address = new cimString(value22);
                                        }
                                        if (!connectedRasds.Contains(rasd))
                                            connectedRasds.Add(rasd);
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            Comparison<RASD_Type> diskcomparison = new Comparison<RASD_Type>(compareConnectedDisks);
            connectedRasds.Sort(diskcomparison);
            return connectedRasds;
        }

        private void SetIfDeviceIsBootable(EnvelopeType ovfEnv, RASD_Type rasd)
        {
            // This is a best guess algorithm. without opening the VHD itself, there is no guarrenteed method
            // to delineate this, so we guess.
            // IF it's created by Kensho/XenConvert there will be a chance of having a clue.
            // Otherwise it'll be based upon 'order' and device 0 will win the bootable device.
            bool isBootable = true;
            VirtualDiskDesc_Type[] disks = null;

            foreach (Section_Type sect in ovfEnv.Sections)
            {
                if (sect is DiskSection_Type)
                {
                    disks = ((DiskSection_Type)sect).Disk;
                }
            }

            if (disks == null)
                return;

            bool useHostResource = false;
            if (Tools.ValidateProperty("HostResource", rasd))
            {
                log.Debug("Using HostResource to find Disk");
                useHostResource = true;
            }
            else
            {
                log.Debug("Using InstanceID to find Disk");
            }
            
            foreach(VirtualDiskDesc_Type disk in disks)
            {
                if (useHostResource)
                {
                    if (rasd.HostResource[0].Value.Contains(disk.diskId))
                    {
                        isBootable = disk.isBootable;
                    }
                }
                else
                {
                    if (rasd.InstanceID.Value.Contains(disk.diskId))
                    {
                        isBootable = disk.isBootable;
                    }
                }
            }

            if (Tools.ValidateProperty("Address", rasd))
            {
                if ((rasd.ResourceType.Value == 21 ||
                    rasd.ResourceType.Value == 5) &&
                    rasd.Address.Value == "0")
                {
                    isBootable = true;
                }
            }

            if (Tools.ValidateProperty("AddressOnParent", rasd))
            {
                if ((rasd.ResourceType.Value == 17 ||
                    rasd.ResourceType.Value == 19) &&
                    rasd.AddressOnParent.Value == "0")
                {
                    isBootable = true;
                }
            }

            if (Tools.ValidateProperty("Connection", rasd))
            {
                if (rasd.Connection[0].Value.Contains("device=0"))
                {
                    isBootable = true;
                }
                if (!rasd.Connection[0].Value.Contains("bootable"))
                {
                    rasd.Connection[0].Value = string.Format("{0},bootable={1}", rasd.Connection[0].Value, isBootable);
                }
            }
            else
            {
                rasd.Connection = new cimString[] { new cimString(string.Format("bootable={0}", isBootable)) };
            }
        }

        public bool IsKnownURIType(string filename)
        {
            string expression = Properties.Settings.Default.uriRegex;
            RegexStringValidator rsv = new RegexStringValidator(expression);
            if (rsv.CanValidate(filename.GetType()))
            {
                try
                {
                    rsv.Validate(filename);
                    log.InfoFormat("Import.isURI: File {0} is in URI format.", filename);
                    return true;
                }
                catch
                {
                    log.InfoFormat("Import.isURI: File {0} is not in URI format.", filename);
                }
            }
            return false;
        }

        private void ShowSystem(Session xenSession, XenRef<VM> vmRef)
        {
            VM.remove_from_other_config(xenSession, vmRef, "HideFromXenCenter");
        }
        
    	public string DownloadFileAsync(Uri filetodownload, ulong totalsize)
        {
            log.InfoFormat("DownloadFileAsync: {0}", filetodownload);
            _downloadexception = null;
            string tmpfilename = filetodownload.AbsolutePath.Substring(filetodownload.AbsolutePath.LastIndexOf('/') + 1);
            if (!File.Exists(tmpfilename))
            {
                downloadupdatemsg = string.Format(Messages.ISCSI_COPY_PROGRESS, tmpfilename);
                _downloadexception = null;
                OnUpdate(new XenOvfTransportEventArgs(TransportStep.Download, downloadupdatemsg, 0, _filedownloadsize));
                WebClient wc = new WebClient();
                wc.Proxy = XenAdmin.XenAdminConfigManager.Provider.GetProxyFromSettings(null, false);
                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(filetodownload, tmpfilename);
                uridownloadcomplete.WaitOne();
                if (_downloadexception != null)
                {
                    if (!Path.GetExtension(tmpfilename).Equals(".pvp"))  // don't worry bout pvp files, we don't use them.
                        throw _downloadexception;
                }
                OnUpdate(new XenOvfTransportEventArgs(TransportStep.Download, downloadupdatemsg, _filedownloadsize, _filedownloadsize));
            }
            return tmpfilename;
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            OnUpdate(new XenOvfTransportEventArgs(TransportStep.Download, downloadupdatemsg, (ulong)e.BytesReceived, (ulong)_filedownloadsize));
        }

        private void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                log.Error("DownloadFileAsync failure.", e.Error);
                _downloadexception = e.Error;
            }
            log.Info("DownloadFileAsync: completed");
            uridownloadcomplete.Set();
        }

        private static int compareControllerRasd(RASD_Type rasd1, RASD_Type rasd2)
        {
            if (rasd1.ResourceType.Value >= 5 &&
                rasd1.ResourceType.Value <= 9 &&
                rasd2.ResourceType.Value >= 5 &&
                rasd2.ResourceType.Value <= 9 && 
                rasd1.Address != null &&
                rasd1.Address.Value != null &&
                rasd2.Address != null &&
                rasd2.Address.Value != null)
            {
                ushort address1 = Convert.ToUInt16(rasd1.Address.Value);
                ushort address2 = Convert.ToUInt16(rasd2.Address.Value);
                int left = (rasd1.ResourceType.Value * 10) + address1;
                int right = (rasd2.ResourceType.Value * 10) + address2;
                return (left).CompareTo(right);
            }
            else
            {
                return rasd1.ResourceType.Value.CompareTo(rasd2.ResourceType.Value);
            }
        }

        private static int compareConnectedDisks(RASD_Type rasd1, RASD_Type rasd2)
        {
            if (rasd1.AddressOnParent != null &&
                rasd1.AddressOnParent.Value != null &&
                rasd2.AddressOnParent != null &&
                rasd2.AddressOnParent.Value != null)
            {

                return (rasd1.AddressOnParent.Value).CompareTo(rasd2.AddressOnParent.Value);
            }
            else
            {
                if (rasd1.Address != null &&
                    rasd1.Address.Value != null &&
                    rasd2.Address != null &&
                    rasd2.Address.Value != null)
                {
                    ushort address1 = Convert.ToUInt16(rasd1.Address.Value);
                    ushort address2 = Convert.ToUInt16(rasd2.Address.Value);
                    return (address1).CompareTo(address2);
                }
                throw new ArgumentNullException("Cannot compare null values");
            }
        }
        #endregion

    }
}
