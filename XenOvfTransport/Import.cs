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
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DiscUtils;
using DiscUtils.BootConfig;
using DiscUtils.Ntfs;
using DiscUtils.Partitions;
using DiscUtils.Registry;
using DiscUtils.Wim;
using XenAdmin.Core;
using XenAPI;
using XenOvf;
using XenOvf.Definitions;
using XenOvf.Definitions.XENC;
using XenOvf.Definitions.VMC;
using XenCenterLib.Compression;

using XenOvf.Utilities;

namespace XenOvfTransport
{
    public class Import : XenOvfTransportBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly log4net.ILog auditLog = log4net.LogManager.GetLogger("Audit");
        private static readonly log4net.ILog traceLog = log4net.LogManager.GetLogger("Trace");

        private const long KB = 1024;
        private const long MB = (KB * 1024);
        private const long GB = (MB * 1024);
        private const long STATMEMMIN = 16 * MB;

        public enum TransferType 
        { 
            UploadRawVDI = 0,
            iSCSI = 1,
            SFTP = 2,
            Skip = 3,
            Unknown = 4 
        }

        public enum TransferMethod 
        {
            Image = 0,
            Files = 1,
            Unknown = 2
        }

		private string downloadupdatemsg;
        private string EncryptionClass;
        private int EncryptionKeySize;
        private List<XenRef<VM>> P2VVMServerList = new List<XenRef<VM>>();
        private XenRef<SR> DefaultSRUUID = null;
        private readonly Http http;
        private VirtualDisk vhdDisk = null;
        private WimFile wimDisk = null;
        private ulong wimFileCount = 0;
        private ulong wimFileIndex = 0;
        private ulong AdditionalSpace = 20 * GB;
        private int vifDeviceIndex = 0;  // used to count number of Networks attached and to be used as the device number.
        private string _currentfilename = null;
        private bool _metadataonly = false;
        private Exception _downloadexception = null;
        private ulong _filedownloadsize = 0;
        private int xvadisk = 0;
        private AutoResetEvent uridownloadcomplete = new AutoResetEvent(false);
        private string _appliancename = null;

        public string ApplianceName
        {
            get
            {
                return _appliancename;
            }
            set
            {
                _appliancename = value;
            }
        }

        public bool MetaDataOnly
        {
            get
            {
                return _metadataonly;
            }
            set
            {
                _metadataonly = value;
            }
		}

		#region Constructors

        public Import(Uri xenserver, Session session)
            : base(xenserver, session)
        {
            http = new Http { UpdateHandler = http_UpdateHandler };
        }

		#endregion

		#region PUBLIC

        public void Process(string ovfFileName)
        {

            Process(ovfFileName, null);
        }

        public void Process(string ovfFileName, string passcode)
        {
            if (XenSession == null)
                throw new InvalidOperationException(Messages.ERROR_NOT_CONNECTED);

            string ovfpath = Path.GetDirectoryName(ovfFileName);
            string ovffilename = Path.GetFileName(ovfFileName);
            string ovfname = Path.GetFileNameWithoutExtension(ovfFileName);
            
            string openovf = Path.Combine(ovfpath, string.Format(@"{0}.ovf", ovfname));

            if (!File.Exists(ovfFileName))
                throw new FileNotFoundException(string.Format(Messages.FILE_MISSING, ovfFileName));

            if (Path.GetExtension(ovffilename).ToLower().EndsWith("ova", StringComparison.InvariantCulture) ||
                Path.GetExtension(ovffilename).ToLower().EndsWith("gz", StringComparison.InvariantCulture))
            {
                log.InfoFormat("Import.Process: Opening OVF Archive: {0}", ovfFileName);
                OVF.OpenOva(ovfpath, ovffilename);
                if (!File.Exists(openovf))
                {
                    throw new FileNotFoundException(string.Format(Messages.FILE_MISSING, openovf));
                }
            }

            EnvelopeType ovfEnv = OVF.Load(openovf);
            Process(ovfEnv, ovfpath, passcode);
        }

        public void Process(EnvelopeType ovfObj, string pathToOvf, string passcode)
        {
            Process(XenSession, ovfObj, pathToOvf, passcode);
        }

		public void Process(Session xenSession, EnvelopeType ovfObj, string pathToOvf, string passcode)
        {
            if (xenSession == null)
                throw new InvalidOperationException(Messages.ERROR_NOT_CONNECTED);

            string ovfname = Guid.NewGuid().ToString();

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
                            string algoname = (securitytype.EncryptionMethod.Algorithm.Split(new char[] { '#' }))[1].ToLower().Replace('-', '_');
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

            //
            // So the process is the same below, change this
            //
            if (ovfObj.Item is VirtualSystem_Type)
            {
				VirtualSystem_Type vstemp = (VirtualSystem_Type)ovfObj.Item;
				ovfObj.Item = new VirtualSystemCollection_Type();
				((VirtualSystemCollection_Type)ovfObj.Item).Content = new Content_Type[] { vstemp };
            }

			#region Create appliance

        	XenRef<VM_appliance> applRef = null;
			if (ApplianceName != null)
			{
				var vmAppliance = new VM_appliance {name_label = ApplianceName, Connection = xenSession.Connection};
				applRef = VM_appliance.create(xenSession, vmAppliance);
			}

        	#endregion

			foreach (VirtualSystem_Type vSystem in ((VirtualSystemCollection_Type)ovfObj.Item).Content)
            {
               //FIND/SET THE NAME OF THE VM
				ovfname = OVF.FindSystemName(ovfObj, vSystem.id);
                auditLog.DebugFormat("Import: {0}, {1}", ovfname, pathToOvf);

				VirtualHardwareSection_Type vhs = OVF.FindVirtualHardwareSectionByAffinity(ovfObj, vSystem.id, "xen");

                XenRef<VM> vmRef = DefineSystem(xenSession, vhs, ovfname);
                if (vmRef == null)
                {
                    log.Error(Messages.ERROR_CREATE_VM_FAILED);
                    throw new ImportException(Messages.ERROR_CREATE_VM_FAILED);
				}

				HideSystem(xenSession, vmRef);
                log.DebugFormat("OVF.Import.Process: DefineSystem completed ({0})", VM.get_name_label(xenSession, vmRef));
				
				#region Set appliance
				if (applRef != null)
					VM.set_appliance(xenSession, vmRef.opaque_ref, applRef.opaque_ref);

				if (ovfObj.Sections != null)
				{
					StartupSection_Type[] startUpArray = OVF.FindSections<StartupSection_Type>(ovfObj.Sections);
					if (startUpArray != null && startUpArray.Length > 0)
					{
						var startupSection = startUpArray[0];
						var itemList = startupSection.Item;

						if (itemList != null)
						{
							var item = itemList.FirstOrDefault(it => it.id == vSystem.id);

							if (item != null)
							{
								VM.set_start_delay(xenSession, vmRef.opaque_ref, item.startDelay);
								VM.set_shutdown_delay(xenSession, vmRef.opaque_ref, item.stopDelay);
								VM.set_order(xenSession, vmRef.opaque_ref, item.order);
							}
						}
					}
				}

            	#endregion

                #region set has_vendor_device

                if (Helpers.DundeeOrGreater(xenSession.Connection))
                {
                    var data = vhs.VirtualSystemOtherConfigurationData;
                    if (data != null)
                    {
                        var datum = data.FirstOrDefault(s => s.Name == "VM_has_vendor_device");
                        if (datum != null)
                        {
                            bool hasVendorDevice;
                            if (bool.TryParse(datum.Value.Value, out hasVendorDevice) && hasVendorDevice)
                                VM.set_has_vendor_device(xenSession, vmRef.opaque_ref, hasVendorDevice);
                        }
                    }
                }

                #endregion

                #region Set vgpu
                
                GPU_group gpuGroup;              
                VGPU_type vgpuType;
                FindGpuGroupAndVgpuType(xenSession, vhs, out gpuGroup, out vgpuType);

                if (gpuGroup != null)
                {
                    var other_config = new Dictionary<string, string>();

                    if (Helpers.FeatureForbidden(xenSession, Host.RestrictVgpu))
                        VGPU.create(xenSession, vmRef.opaque_ref, gpuGroup.opaque_ref, "0", other_config);
                    else if (vgpuType != null)
                        VGPU.create(xenSession, vmRef.opaque_ref, gpuGroup.opaque_ref, "0", other_config, vgpuType.opaque_ref);
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
                                AdditionalSpace = OVF.ComputeCapacity(Convert.ToInt64(vdisk.capacity), vdisk.capacityAllocationUnits);  // used in Wim file imports only.
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
                        OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.MarqueeOn, "Import", Messages.START_POST_INSTALL_INSTRUCTIONS));
                        OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.ImportProgress, "Import", Messages.START_POST_INSTALL_INSTRUCTIONS));
                        HandleInstallSection(xenSession, vmRef, installSection[0]);
                    }
                    ShowSystem(xenSession, vmRef);
                }
                catch (Exception ex)
                {
					if (ex is OperationCanceledException)
						throw;
                    log.Error(Messages.ERROR_IMPORT_FAILED);
                    throw new Exception(Messages.ERROR_IMPORT_FAILED, ex);
                }
            }

            OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.MarqueeOff, "Import", ""));
            int _processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            string _touchFile = Path.Combine(pathToOvf, "xen__" + _processId);
			//added check again as Delete needs write permissions and even if the file does not exist import will fail if the user has read only permissions
			if (File.Exists(_touchFile))
				File.Delete(_touchFile);

            OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.ImportThreadComplete, "Import", Messages.COMPLETED_IMPORT));
        }

    	#endregion

        #region PRIVATE
		
		private void http_UpdateHandler(XenOvfTranportEventArgs e)
		{
			OnUpdate(e);
		}

        private void HandleInstallSection(Session xenSession, XenRef<VM> vm, InstallSection_Type installsection)
        {
            // Configure for XenServer as requested by OVF.SetRunOnceBootCDROM() with the presence of a post install operation that is specific to XenServer.
            if (installsection.PostInstallOperations != null)
                ConfigureForXenServer(xenSession, vm);

            // Run the VM for the requested duration if this appliance had its own install section -- one not added to fixup for XenServer.
            if (((installsection.Info == null)) ||
                ((installsection.Info != null) && (installsection.Info.Value.CompareTo("ConfigureForXenServer") != 0)))
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
            long fixupMemorySize = Properties.Settings.Default.FixupOsMemorySizeAsMB * MB;

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
            if (VM.get_power_state(xenSession, vm) != vm_power_state.Halted)
                VM.hard_shutdown(xenSession, vm);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                         Justification = "Logging mechanism")]
        internal List<XenRef<VDI>> ImportFile(Session xenSession, string vmname, string pathToOvf, string filename, string compression, string version, string passcode, string sruuid, string description, string vdiuuid)
        {
            List<XenRef<VDI>> vdiRef = new List<XenRef<VDI>>();

            // Get the disk transport method from the configuration in XenOvfTransport.Properties.Settings.TransferType.
            TransferType useTransport = (TransferType)Enum.Parse(typeof(TransferType), Properties.Settings.Default.TransferType, true);
            TransferMethod useTransferMethod = TransferMethod.Image;
            string sourcefile = filename;
            string encryptfilename = null;
            string uncompressedfilename = null;
            string destinationPath = Properties.Settings.Default.xenISOMount;
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
                            OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.MarqueeOn, "Security", statusMessage));
                            log.Debug(statusMessage);
                            OVF.DecryptToTempFile(EncryptionClass, filename, version, passcode, encryptfilename);
                            sourcefile = encryptfilename;
                            statusMessage += Messages.COMPLETE;
                            OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.MarqueeOff, "Security", statusMessage));
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
                            OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.MarqueeOn, "Compression", statusMessage));
                        	var ovfCompressor = new OvfCompressor();
							ovfCompressor.UncompressFile(sourcefile, uncompressedfilename, compression);
                            if (File.Exists(encryptfilename)) { File.Delete(encryptfilename); }
                            sourcefile = uncompressedfilename;
                            statusMessage += Messages.COMPLETE;
                            OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.MarqueeOff, "Compression", statusMessage));
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
                        else if (ext.ToLower().Contains("wim"))
                        {
                            log.WarnFormat("EXPERIMENTAL CODE: Found file {0} using WIM file structure", filename);
                            wimDisk = new DiscUtils.Wim.WimFile(new FileStream(sourcefile, FileMode.Open, FileAccess.Read));
                            //wimFS = wimDisk.GetImage(wimDisk.BootImage);

                            dataStream = null;

                            string manifest = wimDisk.Manifest;
                            Wim_Manifest wimManifest = (Wim_Manifest)Tools.Deserialize(manifest, typeof(Wim_Manifest));
                            ulong imagesize = wimManifest.Image[wimDisk.BootImage].TotalBytes; // <----<<< Image data size
                            wimFileCount = wimManifest.Image[wimDisk.BootImage].FileCount;
                            dataCapacity = (long)(imagesize + AdditionalSpace);
                            useTransferMethod = TransferMethod.Files;
                        }
                        else if (ext.ToLower().Contains("xva"))
                        {
                            log.WarnFormat("EXPERIMENTAL CODE: Found file {0} using XVA Stream (DISK {1} is being imported).", filename, xvadisk);
                            DiscUtils.Xva.VirtualMachine vm = new DiscUtils.Xva.VirtualMachine(new FileStream(sourcefile, FileMode.Open, FileAccess.Read));
                            int i = 0;
                            foreach (DiscUtils.Xva.Disk d in vm.Disks)
                            {
                                if (i == xvadisk)
                                {
                                    vhdDisk = d;
                                    break;
                                }
                            }
                            dataStream = vhdDisk.Content;
                            dataCapacity = vhdDisk.Capacity;
                        }
                        else if (ext.ToLower().EndsWith("iso"))
                        {
                            if (string.IsNullOrEmpty(sruuid))
                            {
                                useTransport = TransferType.Skip;
                            }
                            else
                            {
                                //DiscUtils.Iso9660.CDReader cdr = new DiscUtils.Iso9660.CDReader(File.OpenRead(filename), true);
                                dataStream = File.OpenRead(filename);
                                dataCapacity = dataStream.Length + (512 * KB);  // Xen does 512KB rounding this is to ensure it doesn't round down below size.
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        log.Error(Messages.ISCSI_ERROR_CANNOT_OPEN_DISK);
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
                log.Error(Messages.ERROR_FILE_NAME_NULL);
                throw new InvalidDataException(Messages.ERROR_FILE_NAME_NULL);
            }
            #endregion
                        
            try
            {
                #region SEE IF TARGET SR HAS ENOUGH SPACE
                if (useTransport == TransferType.UploadRawVDI ||
                    useTransport == TransferType.iSCSI)
                {
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
                        string message = string.Format(Messages.NOT_ENOUGH_SPACE_IN_SR, sruuid, Convert.ToString(vhdDisk.Capacity), filename);
                        log.Error(message);
                        throw new IOException(message);
                    }
                }
                #endregion

                #region UPLOAD FILE
                switch (useTransport)
                {
                    case TransferType.UploadRawVDI:
                        {
                            vdiRef.Add(UploadRawVDI(xenSession, sruuid, vmname, dataStream, dataCapacity, description));
                            break;
                        }
                    case TransferType.iSCSI:
                        {
                            if (useTransferMethod == TransferMethod.Image)
                            {
                                vdiRef.Add(UploadiSCSI(xenSession, sruuid, vmname, dataStream, dataCapacity, description, vdiuuid));
                            }
                            else
                            {
                                for (int i = 0; i < wimDisk.ImageCount; i++)
                                {
                                    Wim_Manifest wimManifest = (Wim_Manifest)Tools.Deserialize(wimDisk.Manifest, typeof(Wim_Manifest));
                                    wimFileCount = wimManifest.Image[i].FileCount;
                                    int wimArch = wimManifest.Image[i].Windows.Architecture;
                                    vdiRef.Add(UploadiSCSIbyWimFile(xenSession, sruuid, vmname, wimDisk, i, dataCapacity, wimFileCount, wimArch, ""));
                                }
                            }
                            break;
                        }
                    case TransferType.Skip:
                        {
                            log.Info("ImportFile: Upload Skipped");
                            break;
                        }
                    default:
                		{
                            log.Error(Messages.UNSUPPORTED_TRANSPORT);
                            throw new InvalidDataException(Messages.UNSUPPORTED_TRANSPORT);
                        }
                }
                #endregion
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
                if (wimDisk != null)
                {
                    wimDisk = null;
                }
                if (File.Exists(encryptfilename)) { File.Delete(encryptfilename); }
                if (File.Exists(uncompressedfilename)) { File.Delete(uncompressedfilename); }
            }

            Directory.SetCurrentDirectory(StartPath);
            log.DebugFormat("OVF.Import.ImportFile leave: created {0} VDIs", vdiRef.Count);
            return vdiRef;
        }
        private XenRef<VDI> UploadiSCSI(Session xenSession, string sruuid, string label, Stream filestream, long capacity, string description, string vdiuuid)
        {
            log.Debug("OVF.Import.UploadiSCSI Enter");
            log.DebugFormat("OVF.Import.UploadiSCSI SRUUID: {0}", sruuid);
            log.DebugFormat("OVF.Import.UploadiSCSI Label: {0}", label);
            log.DebugFormat("OVF.Import.UploadiSCSI Capacity: {0}", capacity);

            XenRef<VDI> vdiRef = null;

            //If no VDI uuid is provided create a VDI, otherwise use the one provided as
            //the target for the import. Used for SRs such as Lun per VDI (PR-1544)
            if(String.IsNullOrEmpty(vdiuuid))
            {
                vdiRef = CreateVDI(xenSession, sruuid, label, capacity, description);
                vdiuuid = VDI.get_uuid(xenSession, vdiRef);
            }
            else
            {
                vdiRef = new XenRef<VDI>(VDI.get_by_uuid(xenSession, vdiuuid));
            }
                

            #region UPLOAD iSCSI STREAM
            OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.FileStart, "Import", string.Format(Messages.FILES_TRANSPORT_SETUP, _currentfilename)));
        	m_iscsi = new iSCSI
        	        	{
        	        		UpdateHandler = iscsi_UpdateHandler,
        	        		Cancel = Cancel //in case it has already been cancelled
        	        	};
			m_iscsi.ConfigureTvmNetwork(m_networkUuid, m_isTvmIpStatic, m_tvmIpAddress, m_tvmSubnetMask, m_tvmGateway);
            try
            {
                using (Stream iSCSIStream = m_iscsi.Connect(xenSession, vdiuuid, false))
                {
					m_iscsi.Copy(filestream, iSCSIStream, label, false);
                    iSCSIStream.Flush();
                }
            }
            catch (Exception ex)
            {
				if (ex is OperationCanceledException)
					throw;
                log.ErrorFormat("{0} {1}", Messages.ERROR_ISCSI_UPLOAD_FAILED, ex.Message);
                vdiRef = null;
                throw new Exception(Messages.ERROR_ISCSI_UPLOAD_FAILED, ex);
            }
            finally
            {
                OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.FileStart, "Import", string.Format(Messages.FILES_TRANSPORT_CLEANUP,_currentfilename)));
				m_iscsi.Disconnect(xenSession);
            }
            #endregion

            log.Debug("OVF.Import.UploadiSCSI Leave");
            return vdiRef;
        }

        private XenRef<VDI> UploadiSCSIbyWimFile(Session xenSession, string sruuid, string label, WimFile wimDisk, int imageindex, long capacity, ulong wimFileCount, int arch, string description)
        {
            log.Debug("OVF.Import.UploadiSCSIbyWimFile Enter");
            log.DebugFormat("OVF.Import.UploadiSCSIbyWimFile SRUUID: {0}", sruuid);
            log.DebugFormat("OVF.Import.UploadiSCSIbyWimFile Label: {0}", label);
            log.DebugFormat("OVF.Import.UploadiSCSIbyWimFile ImageIndex: {0}", imageindex);
            log.DebugFormat("OVF.Import.UploadiSCSIbyWimFile Capacity: {0}", capacity);
            string vdilabel = string.Format("{0}{1}", label, imageindex);
            XenRef<VDI> vdiRef = CreateVDI(xenSession, sruuid, vdilabel, capacity, description);

            byte[] mbr = null;
            byte[] boot = null;
            //byte[] bcd = null;
            //byte[] bootmgr = null;

            string vhdfile = FindReferenceVHD(Properties.Settings.Default.ReferenceVHDName);
            if (File.Exists(vhdfile))
            {
                mbr = ExtractMBRFromVHD(vhdfile);
                boot = ExtractBootFromVHD(vhdfile);
                //bcd = ExtractBCDFromVHD(vhdfile, arch);
                //bootmgr = ExtractBootmgrFromVHD(vhdfile, arch);
            }
            else
            {
                log.WarnFormat("Refernce VHD not found [{0}]", Properties.Settings.Default.ReferenceVHDName);
            }

            #region UPLOAD iSCSI STREAM
            OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.FileStart, "Import", string.Format(Messages.FILES_TRANSPORT_SETUP, _currentfilename)));
			m_iscsi = new iSCSI
			{
				UpdateHandler = iscsi_UpdateHandler,
				Cancel = Cancel //in case it has already been cancelled
			};
			m_iscsi.ConfigureTvmNetwork(m_networkUuid, m_isTvmIpStatic, m_tvmIpAddress, m_tvmSubnetMask, m_tvmGateway);
            try
            {
                wimFileIndex = 0;
                string vdiuuid = VDI.get_uuid(xenSession, vdiRef);
				Stream iSCSIStream = m_iscsi.Connect(xenSession, vdiuuid, false);
                WimFileSystem w = wimDisk.GetImage(imageindex);

                if (mbr != null)
                {
					m_iscsi.ScsiDisk.SetMasterBootRecord(mbr);
                }
                else
                {
                    log.WarnFormat("System will not be bootable, cannot find [{0}] to extract master boot record.", vhdfile);
                    OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.FileStart, "Import", Messages.WARNING_TARGET_NOT_BOOTABLE));
                }
				m_iscsi.ScsiDisk.Signature = new Random().Next();

				BiosPartitionTable table = BiosPartitionTable.Initialize(m_iscsi.ScsiDisk, WellKnownPartitionType.WindowsNtfs);
				VolumeManager volmgr = new VolumeManager(m_iscsi.ScsiDisk);

                NtfsFileSystem ntfs = null;
                if (wimDisk.BootImage == imageindex && boot != null)
                {
                    table.SetActivePartition(0);
                    ntfs = NtfsFileSystem.Format(volmgr.GetLogicalVolumes()[0], "New Volume", boot);
                }
                else
                {
                    ntfs = NtfsFileSystem.Format(volmgr.GetLogicalVolumes()[0], "New Volume");
                }

                //AddBCD(ntfs, bcd);
                //AddBootMgr(ntfs, bootmgr);  // If it's not there it'll be created if it is it will not.. not below filecopy will overwrite if one it exists.

				FileCopy(m_iscsi, w.Root.GetFiles(), w, ntfs);
				FileCopy(m_iscsi, w.Root.GetDirectories(), w, ntfs);

                FixBCD(ntfs, volmgr);

                ntfs.Dispose();

            }
            catch (Exception ex)
            {
				if (ex is OperationCanceledException)
					throw;
                log.ErrorFormat("{0} {1}", Messages.ERROR_ISCSI_UPLOAD_FAILED, ex.Message);
                vdiRef = null;
                throw new Exception(Messages.ERROR_ISCSI_UPLOAD_FAILED, ex);
            }
            finally
            {
                OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.FileStart, "Import", string.Format(Messages.FILES_TRANSPORT_CLEANUP, _currentfilename)));
				m_iscsi.Disconnect(xenSession);
            }
            #endregion

            log.Debug("OVF.Import.UploadiSCSIbyWimFile Leave");
            return vdiRef;
        }

		private void iscsi_UpdateHandler(XenOvfTranportEventArgs e)
		{
			OnUpdate(e);
		}

        private XenRef<VDI> CreateVDI(Session xenSession, string sruuid, string label, long capacity, string description)
        {
            Hashtable vdiHash = new Hashtable();
            vdiHash.Add("uuid", Guid.NewGuid().ToString());
            vdiHash.Add("name_label", label);
            vdiHash.Add("name_description", description);
            if (sruuid.ToLower().StartsWith("opaque"))
            {
                vdiHash.Add("SR", sruuid);
            }
            else
            {
				vdiHash.Add("SR", SR.get_by_uuid(xenSession, sruuid).opaque_ref);
            }
            vdiHash.Add("virtual_size", Convert.ToString(capacity));
            vdiHash.Add("physical_utilisation", Convert.ToString(capacity));
            vdiHash.Add("type", "user");
            vdiHash.Add("shareable", false);
            vdiHash.Add("read_only", false);
            vdiHash.Add("storage_lock", false);
            vdiHash.Add("managed", true);
            vdiHash.Add("is_a_snapshot", false);

            VDI vdi = new VDI(vdiHash);
            XenRef<VDI> vdiRef = null;
            try
            {
                // Note that XenServer will round the capacity up to the nearest multiple of a 2 MB block.
                vdiRef = VDI.create(xenSession, vdi);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("{0} {1}", Messages.ERROR_CANNOT_CREATE_VDI, ex.Message);
                throw new Exception(Messages.ERROR_CANNOT_CREATE_VDI, ex);
            }
            log.Debug("Import.CeateVDI::VDI Created");
            return vdiRef;
        }

        private void FileCopy(iSCSI iscsi, DiscDirectoryInfo[] DirInfos, WimFileSystem w, NtfsFileSystem ntfs)
        {
            foreach (DiscDirectoryInfo dir in DirInfos)
            {
                if (IsExcluded(dir.FullName))
                {
                    log.InfoFormat("Directory Skip {0}", dir.FullName);
                    continue;
                }
                FileAttributes attr = dir.Attributes;
                if ((dir.Attributes & FileAttributes.ReparsePoint) == 0)
                {
                    ntfs.CreateDirectory(dir.FullName);
                    if ((attr & FileAttributes.Temporary) == FileAttributes.Temporary)
                        attr = attr & ~FileAttributes.Temporary;
                    if ((attr & FileAttributes.Offline) == FileAttributes.Offline)
                        attr = attr & ~FileAttributes.Offline;
                    ntfs.SetAttributes(dir.FullName, attr);

                    FileCopy(iscsi, dir.GetDirectories(), w, ntfs);
                    FileCopy(iscsi, dir.GetFiles(), w, ntfs);
                }
                else
                {
                    traceLog.InfoFormat("Directory ReparsePoint {0}", dir.FullName);
                    ReparsePoint rp = w.GetReparsePoint(dir.FullName);
                    ntfs.CreateDirectory(dir.FullName);
                    ntfs.SetReparsePoint(dir.FullName, rp);
                }
            }
        }
        private void FileCopy(iSCSI iscsi, DiscFileInfo[] FileInfos, WimFileSystem w, NtfsFileSystem ntfs)
        {
            foreach (DiscFileInfo file in FileInfos)
            {
                if (IsExcluded(file.FullName))
                {
                    log.InfoFormat("File Skip {0}", file.FullName);
                    continue;
                }
                FileAttributes attr = file.Attributes;
                if ((attr & FileAttributes.ReparsePoint) == 0)
                {
                    using (Stream source = w.OpenFile(file.FullName, FileMode.Open, FileAccess.Read))
                    {
                        using (Stream destin = ntfs.OpenFile(file.FullName, FileMode.Create, FileAccess.Write))
                        {
                            iscsi.WimCopy(source, destin, Path.GetFileName(file.FullName), false, wimFileIndex++, wimFileCount);
                        }
                    }

                    if ((attr & FileAttributes.Temporary) == FileAttributes.Temporary)
                        attr = attr & ~FileAttributes.Temporary;
                    if ((attr & FileAttributes.Offline) == FileAttributes.Offline)
                        attr = attr & ~FileAttributes.Offline;
                    ntfs.SetAttributes(file.FullName, attr);
                }
                else
                {
                    traceLog.InfoFormat("Reparse Point: {0}", file.FullName);
                    ReparsePoint rp = w.GetReparsePoint(file.FullName);
                    ntfs.SetReparsePoint(file.FullName, rp);
                }
            }
        }
        private bool IsExcluded(string path)
        {
            string pathUpper = path.ToUpperInvariant();

            foreach(string excluded in Properties.Settings.Default.EXCLUDED_FILES_COPY)
            {
                if (pathUpper == excluded.ToUpperInvariant())
                {
                    return true;
                }
            }

            return false;
        }

        private XenRef<VDI> UploadRawVDI(Session xenSession, string sruuid, string label, Stream filestream, long capacity, string description)
        {
            log.Debug("OVF.Import.UploadRawVDI Enter");
            log.DebugFormat("OVF.Import.UpdoadRadVDI SRUUID: {0}", sruuid);
            log.DebugFormat("OVF.Import.UpdoadRadVDI Label: {0}", label);
            log.DebugFormat("OVF.Import.UpdoadRadVDI Capacity: {0}", capacity);

            #region CREATE A VDI
            Hashtable vdiHash = new Hashtable();
            vdiHash.Add("uuid", Guid.NewGuid().ToString());
            vdiHash.Add("name_label", label);
            vdiHash.Add("name_description", description);
            if (sruuid.ToLower().StartsWith("opaque"))
            {
                vdiHash.Add("SR", sruuid);
            }
            else
            {
				vdiHash.Add("SR", SR.get_by_uuid(xenSession, sruuid).opaque_ref);
            }
            vdiHash.Add("virtual_size", Convert.ToString(capacity + (2 * MB))); // Add 2MB, VDIs appear to round down making it too small.
            vdiHash.Add("physical_utilisation", Convert.ToString(capacity + (2 * MB)));
            vdiHash.Add("type", "user");
            vdiHash.Add("shareable", false);
            vdiHash.Add("read_only", false);
            vdiHash.Add("storage_lock", false);
            vdiHash.Add("managed", true);
            vdiHash.Add("is_a_snapshot", false);

            VDI vdi = new VDI(vdiHash);
            XenRef<VDI> vdiRef = null;
            try
            {
                vdiRef = VDI.create(xenSession, vdi);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("{0} {1}", Messages.ERROR_CANNOT_CREATE_VDI, ex.Message);
                throw new Exception(Messages.ERROR_CANNOT_CREATE_VDI, ex);
            }
            log.Debug("Import.UploadRawVDI::VDI Created");
            #endregion

            #region UPLOAD HTTP STREAM
            XenRef<Task> taskRef = Task.create(xenSession, "UpdateStream", "UpdateStream");
			string p2VUri = string.Format("/import_raw_vdi?session_id={0}&task_id={1}&vdi={2}", xenSession.uuid, taskRef.opaque_ref, vdiRef.opaque_ref);
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Content-Length", Convert.ToString(capacity));
            headers.Add("Content-Type", "application/octet-stream");
            headers.Add("Expect", "100-continue");
            headers.Add("Accept", "*/*");
            headers.Add("Connection", "close");
            headers.Add("User-Agent", "XenP2VClient/1.5");
            try
            {
                http.Put(filestream, _XenServer, p2VUri, headers, 0, capacity, false);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("{0} {1}", Messages.ERROR_HTTP_UPLOAD_FAILED, ex.Message);
                if (vdiRef != null)
                    RemoveVDI(xenSession, vdiRef);
                vdiRef = null;
                throw new Exception(Messages.ERROR_HTTP_UPLOAD_FAILED, ex);
            }
            log.Debug("Import.UploadRawVDI::http.put complete");
            #endregion
            
            // give it time to catch up.
            Thread.Sleep(new TimeSpan(0, 0, 5));
            log.Debug("OVF.UploadRawVDI Leave");
            return vdiRef;

        }
        private XenRef<VM> DefineSystem(Session xenSession, VirtualHardwareSection_Type system, string ovfName)
        {
            // Set Defaults:
            Random rand = new Random();
            string vM_name_label = null;
            if (ovfName != null)
            {
                vM_name_label = ovfName;
            }

            string vmUuid = Guid.NewGuid().ToString();
            string systemType = "301";
            ulong memorySize = 512 * MB;  // default size.
            string description = Messages.DEFAULT_IMPORT_DESCRIPTION;

            if (system.System == null)
            {
                log.Debug("OVF.Import.DefineSystem: System VSSD is empty, guessing. HVM style");
                if (vM_name_label == null)
                {
                	vM_name_label = Messages.UNDEFINED_NAME_LABEL;
                }
                vmUuid = Guid.NewGuid().ToString();
                systemType = "hvm-3.0-unknown";
            }
            else
            {                
                if (vM_name_label == null)
                {
                	vM_name_label = Messages.UNDEFINED_NAME_LABEL;
                }
                vmUuid = Guid.NewGuid().ToString();

                if (system.System.VirtualSystemType != null && !string.IsNullOrEmpty(system.System.VirtualSystemType.Value))
                {
                    systemType = system.System.VirtualSystemType.Value;
                }

                if (system.System.Description != null)
                    description = system.System.Description.Value;

            }

            #region MEMORY
            // Get Memory, huh? what? oh.. ya..
            RASD_Type[] rasds = OVF.FindRasdByType(system, 4);
            if (rasds != null && rasds.Length > 0)
            {
                // hopefully only one. but if more ... then deal with it.
                memorySize = 0;
                // These are Default to MB... if other ensure RASD is correct.
                double memoryPower = 20.0;
                double memoryRaise = 2.0;
               
                foreach (RASD_Type rasd in rasds)
                {
                    if (rasd.AllocationUnits.Value.ToLower().StartsWith("bytes"))
                    {
                        // Format:  Bytes * 2 ^ 20
                        string[] a1 = rasd.AllocationUnits.Value.Split(new char[] { '*', '^' });
                        if (a1.Length == 3)
                        {
                            memoryRaise = Convert.ToDouble(a1[1]);
                            memoryPower = Convert.ToDouble(a1[2]);
                        }
                    }
                    double memoryMultiplier = Math.Pow(memoryRaise,memoryPower);
                    memorySize += rasd.VirtualQuantity.Value * Convert.ToUInt64(memoryMultiplier);
                }
            }
            #endregion

            #region CPU COUNT
            // Get Memory, huh? what? oh.. ya..
            rasds = OVF.FindRasdByType(system, 3);
            ulong CpuCount = 1;
            if (rasds != null && rasds.Length > 0)
            {
                //
                // Ok this can have more than one CPU and cores each so...
                // Normally each entry is a CPU, the VirtualQuatity is Cores.
                //
                CpuCount = 0;
                foreach (RASD_Type rasd in rasds)
                {
                    CpuCount += rasd.VirtualQuantity.Value;
                }
            }
            #endregion


            Hashtable table = new Hashtable();

            table.Add("uuid", vmUuid);
            table.Add("name_label", vM_name_label);
            table.Add("name_description", description);
            table.Add("user_version", "1");
            table.Add("is_a_template", false);
            table.Add("is_a_snapshot", false);
            table.Add("memory_target", Convert.ToString(memorySize));
            table.Add("memory_static_max", Convert.ToString(memorySize));
            table.Add("memory_dynamic_max", Convert.ToString(memorySize));
            table.Add("memory_dynamic_min", Convert.ToString(memorySize));
            table.Add("memory_static_min", Convert.ToString(STATMEMMIN));
            table.Add("VCPUs_max", Convert.ToString(CpuCount));
            table.Add("VCPUs_at_startup", Convert.ToString(CpuCount));
            table.Add("actions_after_shutdown", "destroy");
            table.Add("actions_after_reboot", "restart");
            table.Add("actions_after_crash", "restart");

        	double hvmshadowmultiplier = 1.0;
            table.Add("HVM_shadow_multiplier", hvmshadowmultiplier);
            table.Add("ha_always_run", false);

            #region XEN SPECIFIC CONFIGURATION INFORMATION
            if (system.VirtualSystemOtherConfigurationData == null || system.VirtualSystemOtherConfigurationData.Length <= 0)
            {
                // DEFAULT should work for all of HVM type or 301
                table.Add("HVM_boot_policy", Properties.Settings.Default.xenBootOptions);
                Hashtable hBoot = new Hashtable();
                hBoot.Add("order", Properties.Settings.Default.xenBootOrder);
                table.Add("HVM_boot_params", hBoot);
                Hashtable hPlatform = MakePlatformHash(Properties.Settings.Default.xenPlatformSetting);
                table.Add("platform", hPlatform);
            }
            else
            {
                foreach (Xen_ConfigurationSettingData_Type xcsd in system.VirtualSystemOtherConfigurationData)
                {
                    string key = xcsd.Name.Replace('-', '_');
                    switch (key.ToLower())
                    {
                        case "hvm_boot_params":
                            {
                                Hashtable hBoot = new Hashtable();
                                hBoot.Add("order", xcsd.Value.Value);
                                if (table.ContainsKey(key))
                                {
                                    table[key] = hBoot;
                                }
                                else
                                {
                                    table.Add(key, hBoot);
                                }
                                break;
                            }
                        case "hvm_shadow_multiplier":
                            {
                                if (table.ContainsKey(key))
                                {
                                    table[key] = Convert.ToDouble(xcsd.Value.Value);
                                }
                                else
                                {
                                    table.Add(key, Convert.ToDouble(xcsd.Value.Value));
                                }
                                break;
                            }
                        case "platform":
                            {
                                Hashtable hPlatform = MakePlatformHash(xcsd.Value.Value);
                                if (table.ContainsKey(key))
                                {
                                    table[key] = hPlatform;
                                }
                                else
                                {
                                    table.Add(key, hPlatform);
                                }
                                break;
                            }
                        default:
                            {
                                if (table.ContainsKey(key))
                                {
                                    table[key] = xcsd.Value.Value;
                                }
                                else
                                {
                                    table.Add(key, xcsd.Value.Value);
                                }
                                break;
                            }
                    }
                 
                }
            }
            #endregion

            try
            {
                VM newVM = new VM(table);
                return VM.create(xenSession, newVM);                
            }
            catch (Exception ex)
            {
                log.ErrorFormat("{0} {1}", Messages.ERROR_CREATE_VM_FAILED, ex.Message);
                throw new Exception(Messages.ERROR_CREATE_VM_FAILED, ex);
            }
        }

        public static Regex VGPU_REGEX = new Regex("^GPU_types={(.*)};VGPU_type_vendor_name=(.*);VGPU_type_model_name=(.*);$");

        private void FindGpuGroupAndVgpuType(Session xenSession, VirtualHardwareSection_Type system, out GPU_group gpuGroup, out VGPU_type vgpuType)
        {
            gpuGroup = null;
            vgpuType = null;

            var data = system.VirtualSystemOtherConfigurationData;
            if (data == null)
                return;

            var datum = data.FirstOrDefault(s => s.Name == "vgpu");
            if (datum == null)
                return;

            Match m = VGPU_REGEX.Match(datum.Value.Value);
            if (!m.Success)
                return;

            var types = m.Groups[1].Value.Split(';');

            var gpuGroups = GPU_group.get_all_records(xenSession);
            var gpuKvp = gpuGroups.FirstOrDefault(g =>
                g.Value.supported_VGPU_types.Count > 0 &&
                g.Value.GPU_types.Length == types.Length &&
                g.Value.GPU_types.Intersect(types).Count() == types.Length);

            if (gpuKvp.Equals(default(KeyValuePair<XenRef<GPU_group>, GPU_group>)))
                return;

            gpuGroup = gpuKvp.Value;
            gpuGroup.opaque_ref = gpuKvp.Key.opaque_ref;

            string vendorName = m.Groups[2].Value;
            string modelName = m.Groups[3].Value;

            var vgpuTypes = VGPU_type.get_all_records(xenSession);
            var vgpuKey = vgpuTypes.FirstOrDefault(v =>
                v.Value.vendor_name == vendorName && v.Value.model_name == modelName);

            if (vgpuKey.Equals(default(KeyValuePair<XenRef<VGPU_type>, VGPU_type>)))
                return;

            vgpuType = vgpuKey.Value;
            vgpuType.opaque_ref = vgpuKey.Key.opaque_ref;
        }

        private void RemoveSystem(Session xenSession, XenRef<VM> vm)
        {
            try
            {
                VM.destroy(xenSession, vm);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("{0} {1}", Messages.ERROR_REMOVE_VM_FAILED, ex.Message);
                throw new Exception(Messages.ERROR_REMOVE_VM_FAILED, ex);
            }
            return;
        }
        private void RemoveVDI(Session xenSession, XenRef<VDI> vdi)
        {
            try
            {
                log.Error("OVF.Import.RemoveVDI: Something went wrong deleting associated VDI");
				VDI.destroy(xenSession, vdi.opaque_ref);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("{0}, {1}", Messages.ERROR_REMOVE_VDI_FAILED, ex.Message);
                throw new Exception(Messages.ERROR_REMOVE_VDI_FAILED, ex);
            }
            return;
        }
        private Hashtable MakePlatformHash(string plaform)
        {
            Hashtable hPlatform = new Hashtable();
            string[] platformArray = plaform.Split(new char[] { '=', ';' });
            for (int i = 0; i < platformArray.Length - 1; i += 2)
            {
                string identifier = platformArray[i].Trim();
                string identvalue = platformArray[i + 1].Trim();
                hPlatform.Add(identifier, identvalue);
            }

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
                                    string[] s = rasd.Connection[0].Value.Split(new char[] { ',' });
                                    for (int i = 0; i < s.Length; i++)
                                    {
                                        if (s[i].StartsWith(Properties.Settings.Default.xenNetworkKey) ||
                                            s[i].StartsWith(Properties.Settings.Default.xenNetworkUuidKey))
                                        {
                                            string[] s1 = s[i].Split(new char[] { '=' } );
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
                        Hashtable vifHash = new Hashtable();
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
                            vifHash.Add("MAC", networkAddress.ToString());
                        }
                        vifHash.Add("uuid", Guid.NewGuid().ToString());
                        vifHash.Add("allowed_operations", new string[] { "attach" });
                        vifHash.Add("device", Convert.ToString(vifDeviceIndex++));
						vifHash.Add("network", net.opaque_ref);
						vifHash.Add("VM", vmRef.opaque_ref);
                        vifHash.Add("MTU", "1500");
                        vifHash.Add("locking_mode", "network_default");
                        VIF vif = new VIF(vifHash);
                        try
                        {
                            VIF.create(xenSession, vif);
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("{0} {1}", Messages.ERROR_CREATE_VIF_FAILED, ex.Message);
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
                        // Currenlty Xen Server can only have ONE CD, so we must 
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
                            List<XenRef<VDI>> vdiRef = new List<XenRef<VDI>>();
                            if (filename != null)
                            {
                                #region IS THE ISO SR IN THE OVF?
                                string isoUuid = null;
                                if (rasd.Connection != null && rasd.Connection.Length > 0)
                                {
                                    if (rasd.Connection[0].Value.ToLower().Contains("sr="))
                                    {
                                        string[] vpairs = rasd.Connection[0].Value.Split(new char[] { ',' });
                                        foreach (string vset in vpairs)
                                        {
                                            if (vset.ToLower().StartsWith("sr="))
                                            {
                                                isoUuid = vset.Substring(vset.LastIndexOf('=') + 1);
                                                try
                                                {
                                                    #region TRY IT AS UUID
                                                    try
                                                    {
                                                        XenRef<SR> srref = SR.get_by_uuid(xenSession, isoUuid);
                                                        if (srref == null)
                                                        {
                                                            isoUuid = null;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        traceLog.Debug("Import.AddResourceSettingData: iso sr uuid not found, trying name_label");
                                                    }
                                                    #endregion

                                                    #region TRY IT AS NAME_LABEL
                                                    try
                                                    {
                                                        List<XenRef<SR>> srrefList = SR.get_by_name_label(xenSession, isoUuid);
                                                        if (srrefList != null && srrefList.Count > 0)
                                                        {
                                                            isoUuid = SR.get_uuid(xenSession, srrefList[0]);
                                                            break;
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        traceLog.Debug("Import.AddResourceSettingData: iso sr uuid not found, looking for vdi...");
                                                    }
                                                    #endregion
                                                }
                                                catch (Exception ex)
                                                {
                                                    log.WarnFormat("Import.AddResourceSettingData: could not find SR: {0}", ex.Message);
                                                    isoUuid = null;
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
                                    vdiRef.Add(isoVDIlist[0]);
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
                                            vdiRef = ImportFileProc(new TaskInfo(xenSession, this, filename, pathToOvf, filename, isoUuid, version, passcode, compression, "", null));
                                        }
                                        catch (Exception ex)
                                        {
											if (ex is OperationCanceledException)
												throw;
                                        	var msg = string.Format(Messages.ERROR_ADDRESOURCESETTINGDATA_FAILED, Messages.ISO);
                                            log.ErrorFormat("{0}, {1}", msg, ex.Message);
                                            throw new Exception(msg, ex);
                                        }
                                        finally
                                        {
                                            if (vdiRef == null || vdiRef.Count <= 0)
                                            {
                                                log.Error(string.Format(Messages.ERROR_IMPORT_DISK_FAILED, filename, isoUuid));
                                                RemoveSystem(xenSession, vmRef);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                vdiRef.Add(XenRef<VDI>.Create(string.Empty));                                    
                            }

                            #region CREATE VBD CONNECTION
                            string booleans = "empty,bootable,unpluggable,attachable,storage-lock";
                            string skipvalues = "sr,vdi";

                            
                            foreach (XenRef<VDI> currentVDI in vdiRef)
                            {
                                Hashtable vbdHash = new Hashtable();

                                if (rasd.Connection != null && rasd.Connection.Length > 0)
                                {
                                    string[] valuepairs = rasd.Connection[0].Value.Split(new char[] { ',' });

                                    foreach (string valuepair in valuepairs)
                                    {
                                        string[] namevalue = valuepair.Split(new char[] { '=' });
                                        if (!skipvalues.ToLower().Contains(namevalue[0].ToLower()))
                                        {
                                            string name = namevalue[0];
                                            if (name.ToLower().Equals("device"))
                                            {
                                                name = "userdevice";
                                            }
                                            if (booleans.Contains(name))
                                            {
                                                vbdHash.Add(name, Convert.ToBoolean(namevalue[1]));
                                            }
                                            else
                                            {
                                                vbdHash.Add(name, namevalue[1]);
                                            }
                                        }
                                    }
                                }
                                if (!vbdHash.ContainsKey("vm-name-label")) vbdHash.Add("vm-name-label", VM.get_name_label(xenSession, vmRef));
								if (!vbdHash.ContainsKey("VM")) vbdHash.Add("VM", vmRef.opaque_ref);
								if (currentVDI != null && !string.IsNullOrEmpty(currentVDI.opaque_ref))
                                {
                                    // Override values.
									if (!vbdHash.ContainsKey("VDI")) vbdHash.Add("VDI", currentVDI.opaque_ref);
									else vbdHash["VDI"] = currentVDI.opaque_ref;
                                    if (!vbdHash.ContainsKey("empty")) vbdHash.Add("empty", false);
                                    else vbdHash["empty"] = false;
                                    if (!vbdHash.ContainsKey("bootable")) vbdHash.Add("bootable", true);
                                    else vbdHash["bootable"] = true;
                                    if (!vbdHash.ContainsKey("unpluggable")) vbdHash.Add("unpluggable", true);
                                    else vbdHash["unpluggable"] = true;
                                }
                                else
                                {
                                    // Override.
                                    if (!vbdHash.ContainsKey("empty")) vbdHash.Add("empty", true);
                                    else vbdHash["empty"] = true;
                                }
                                if (!vbdHash.ContainsKey("mode")) vbdHash.Add("mode", "RO");
                                if (!vbdHash.ContainsKey("userdevice")) vbdHash.Add("userdevice", "3");
                                if (!vbdHash.ContainsKey("type")) vbdHash.Add("type", "CD");
                                if (!vbdHash.ContainsKey("attachable")) vbdHash.Add("attachable", true);
                                if (!vbdHash.ContainsKey("storage-lock")) vbdHash.Add("storage-lock", false);
                                if (!vbdHash.ContainsKey("status-code")) vbdHash.Add("status-code", "0");

                                vbdHash["userdevice"] = VerifyUserDevice(xenSession, vmRef, (string)vbdHash["userdevice"]);

                                Hashtable hOtherConfig = new Hashtable();
                                hOtherConfig.Add("owner", "true");
                                vbdHash.Add("other_config", hOtherConfig);

                                if (!((string)vbdHash["userdevice"]).EndsWith("+"))
                                {
                                    VBD vbd = new VBD(vbdHash);
                                    try
                                    {
                                        VBD.create(xenSession, vbd);
                                    }
                                    catch (Exception ex)
                                    {
                                        log.ErrorFormat("Import.AddResourceSettingData: {0}", ex.Message);
                                    }
                                }
                                else
                                {
                                    log.WarnFormat("Import:  ================== ATTENTION NEEDED =======================");
                                    log.WarnFormat("Import:  Could not determine appropriate number of device placement.");
                                    log.WarnFormat("Import:  Please Start, Logon, Shut down, System ({0})", (string)vbdHash["vm_name_label"]);
                                    log.WarnFormat("Import:  Then attach disks with labels ending with \"+\" to the device number defined before the +.");
                                    log.Warn("Import:  ===========================================================");
                                    OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.Progress, "Import", Messages.WARNING_ADMIN_REQUIRED));
                                }
                            }
                            #endregion

                        }
                        #endregion
                        OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.ImportProgress, "CD/DVD Drive",
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
                        string namelabel = VM.get_name_label(xenSession, vmRef);
                        bool isbootable = false;
                        string mode = "RW";

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

                                List<XenRef<VDI>> vdiRef = null;

                                #region PARSE CONNECTION
                                if (Tools.ValidateProperty("Connection", rasd))
                                {
                                    string[] s = rasd.Connection[0].Value.Split(new char[] { '=', ',' });
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
                                                        mode = "RO";
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
                                        traceLog.Debug("Import.AddResourceSettingData: SR missing... still looking..");
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
                                            traceLog.Debug("Import.AddResourceSettingData: SR missing... still looking..");
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
                                        log.Error(Messages.ERROR_COULD_NOT_FIND_SR);
                                        throw new InvalidDataException(Messages.ERROR_COULD_NOT_FIND_SR);
                                    }

                                    Dictionary<XenRef<SR>, SR> srDict = SR.get_all_records(xenSession);
                                    if (vdiuuid != null)
                                    {
                                        //Try and get the SR that belongs to the VDI attached
                                        XenRef<VDI> tempVDI = VDI.get_by_uuid(xenSession, vdiuuid);
                                        if (tempVDI == null)
                                        {
                                            log.Error(Messages.ERROR_COULD_NOT_FIND_SR);
                                            throw new InvalidDataException(Messages.ERROR_COULD_NOT_FIND_SR);
                                        }

                                        XenRef<SR> tempSR = VDI.get_SR(xenSession, tempVDI.opaque_ref);
                                        sruuid = srDict[tempSR].uuid;
                                    }
                                    else
                                        sruuid = srDict[DefaultSRUUID].uuid;
                                }
                                #endregion

                                try
                                {
                                    string disklabel = string.Format("{0}_{1}",namelabel, userdeviceid);

                                    if ((rasd.ElementName != null) && (!string.IsNullOrEmpty(rasd.ElementName.Value)))
                                        disklabel = rasd.ElementName.Value;

                                    string description = "";

                                    if ((rasd.Description != null) && (!string.IsNullOrEmpty(rasd.Description.Value)))
                                        description = rasd.Description.Value;

                                    vdiRef = ImportFileProc(new TaskInfo(xenSession, this, disklabel, pathToOvf, filename, sruuid, version, passcode, compression, description, vdiuuid));
                                }
                                catch (Exception ex)
                                {
									if (ex is OperationCanceledException)
										throw;
									var msg = string.Format(Messages.ERROR_ADDRESOURCESETTINGDATA_FAILED, Messages.DISK_DEVICE);
                                    log.ErrorFormat("{0} {1}", msg, ex.Message);
                                    throw new InvalidDataException(msg, ex);
                                }
                                finally
                                {
                                    if (vdiRef == null)
                                    {
                                    	var msg = string.Format(Messages.ERROR_IMPORT_DISK_FAILED, filename, sruuid);
                                        log.Error(msg);
                                        RemoveSystem(xenSession, vmRef);
                                    }
                                }

                                log.DebugFormat("Import.AddResourceSettingData coung {0} VDIs", vdiRef.Count);


                                foreach (XenRef<VDI> currentVDI in vdiRef)
                                {
                                    Hashtable vbdHash = new Hashtable();
                                    if (userdeviceid != null)
                                    {
                                        vbdHash.Add("userdevice", VerifyUserDevice(xenSession, vmRef, userdeviceid));
                                    }
                                    else
                                    {
                                        vbdHash.Add("userdevice", VerifyUserDevice(xenSession, vmRef, "99"));
                                    }
                                    vbdHash.Add("bootable", isbootable);
									vbdHash.Add("VDI", currentVDI.opaque_ref);
                                    vbdHash.Add("mode", mode);
                                    vbdHash.Add("uuid", Guid.NewGuid().ToString());
                                    vbdHash.Add("vm_name_label", namelabel);
									vbdHash.Add("VM", vmRef.opaque_ref);
                                    vbdHash.Add("empty", false);
                                    vbdHash.Add("type", "Disk");
                                    vbdHash.Add("currently_attached", false);
                                    vbdHash.Add("attachable", true);
                                    vbdHash.Add("storage_lock", false);
                                    vbdHash.Add("status_code", "0");

                                    #region SET OTHER_CONFIG STUFF HERE !
                                    //
                                    // below other_config keys XS to delete the disk along with the VM.
                                    //
                                    Hashtable hOtherConfig = new Hashtable();
                                    hOtherConfig.Add("owner", "true");
                                    vbdHash.Add("other_config", hOtherConfig);
                                    #endregion


                                    if (!((string)vbdHash["userdevice"]).EndsWith("+"))
                                    {
                                        VBD vbd = new VBD(vbdHash);

                                        try
                                        {
                                            VBD.create(xenSession, vbd);
                                        }
                                        catch (Exception ex)
                                        {
                                            log.ErrorFormat("{0} {1}", Messages.ERROR_CREATE_VBD_FAILED, ex.Message);
                                            throw new Exception(Messages.ERROR_CREATE_VBD_FAILED, ex);
                                        }
                                    }
                                    else
                                    {
                                        log.WarnFormat("Import:  ================== ATTENTION NEEDED =======================");
                                        log.WarnFormat("Import:  Could not determine appropriate number for device placement.");
                                        log.WarnFormat("Import:  Please Start, Logon, Shut down, System ({0})", (string)vbdHash["vm_name_label"]);
                                        log.WarnFormat("Import:  Then manually attach disks with labels with {0}_# that are not attached to {0}", (string)vbdHash["vm_name_label"]);
                                        log.WarnFormat("Import:  ===========================================================");
                                        OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.Progress, "Import", Messages.WARNING_ADMIN_REQUIRED));
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
                                                         Justification = "Logging mechanism")]
        private string VerifyUserDevice(Session xenSession, XenRef<VM> vmRef, string device)
        {
            log.DebugFormat("Import.VerifyUserDevice, checking device: {0} (99 = autoselect)", device);
            string usethisdevice = null;
            List<XenRef<VBD>> vbds = VM.get_VBDs(xenSession, vmRef);
            string[] allowedVBDs = VM.get_allowed_VBD_devices(xenSession, vmRef);

            if (allowedVBDs == null || allowedVBDs.Length <= 0)
            {
                string message = string.Format("OVF.VerifyUserDevice: No more available devices, cannot add device: {0}", device);
                log.Error(message);
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
        private XenRef<VDI> CheckForISOVDI(Session xenSession, string filename)
        {
            XenRef<VDI> vdiref = null;

            Dictionary<XenRef<VDI>, VDI> vdidict = VDI.get_all_records(xenSession);

            foreach (XenRef<VDI> key in vdidict.Keys)
            {
                if (vdidict[key].name_label.ToLower().Equals(filename.ToLower()))
                {
                    vdiref = key;
                    break;
                }
            }

            return vdiref;
        }
        private class TaskInfo
        {
            public Session xenSession;
            public object _import;
            public string NameLabel;
            public string PathToOvf;
            public string Filename;
            public string Compression;
            public string SRuuid;
            public string Version;
            public string Passcode;
            public string Description;
            public string VDIuuid;

            public TaskInfo(Session xensession, object import, string namelabel, string pathToOvf, string filename, string sruuid,  string version, string passcode, string compression, string description, string vdiuuid)
            {
                xenSession = xensession;
                _import = import;
                NameLabel = namelabel;
                PathToOvf = pathToOvf;
                Filename = filename;
                SRuuid = sruuid;
                Passcode = passcode;
                Compression = compression;
                Version = version;
                Description = description;
                VDIuuid = vdiuuid;
            }
        }
        private List<XenRef<VDI>> ImportFileProc(object args)
        {
            List<XenRef<VDI>> vdiRef = null;
            if (args is TaskInfo)
            {
                log.InfoFormat("Import.ImportFileProc: ThreadID: {0}[{1}]", Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId);
                TaskInfo ti = (TaskInfo)args;
                try
                {
                    vdiRef = ((Import)ti._import).ImportFile(ti.xenSession, ti.NameLabel, ti.PathToOvf, ti.Filename, ti.Compression, ti.Version, ti.Passcode, ti.SRuuid, ti.Description, ti.VDIuuid);
                }
                catch (Exception ex)
                {
					if (ex is OperationCanceledException)
						throw;
                    log.Error(Messages.ERROR_IMPORT_FAILED);
                    throw new Exception(Messages.ERROR_IMPORT_FAILED, ex);
                }
            }
            log.Debug("OVF.ImportFileProc (worker thread) completed");
            return vdiRef;
        }
        
        private static bool IsNumber(string s)
        {
            Regex pattern = new Regex(@"[\d]");
            if (pattern.IsMatch(s)) { return true; }
            return false;
        }
        private static bool IsGUID(string expression)
        {
            if (expression != null)
            {
                Regex guidRegEx = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

                return guidRegEx.IsMatch(expression);
            }
            return false;
        }

        public bool IsKnownURIType(string filename)
        {
            bool IsURI = false;
            string expression = Properties.Settings.Default.uriRegex;
            RegexStringValidator rsv = new RegexStringValidator(expression);
            if (rsv.CanValidate(filename.GetType()))
            {
                try
                {
                    rsv.Validate(filename);
                    IsURI = true;
                    log.InfoFormat("Import.isURI: File: {0} is in URI format.", filename);
                }
                catch
                {
                    log.InfoFormat("Import.isURI: File: {0} is not in URI format.", filename);
                    IsURI = false;
                }
            }

            return IsURI;
        }
        
        private void HideSystem(Session xenSession, XenRef<VM> vmRef)
        {
            VM.add_to_other_config(xenSession, vmRef, "HideFromXenCenter", "true");
        }
        private void ShowSystem(Session xenSession, XenRef<VM> vmRef)
        {
            VM.remove_from_other_config(xenSession, vmRef, "HideFromXenCenter");
        }
        
        private byte[] ExtractMBRFromVHD(string referenceVHD)
        {
            byte[] mbr = null;
            using (VirtualDisk vhdx = DiscUtils.Vhd.Disk.OpenDisk(referenceVHD, FileAccess.Read))
            {
                mbr = vhdx.GetMasterBootRecord();
            }
            return mbr;
        }
        private byte[] ExtractBootFromVHD(string referenceVHD)
        {
            return ExtractFileFromVHD(@"$Boot", referenceVHD);
        }
        private byte[] ExtractBCDFromVHD(string referenceVHD, int arch)
        {
            string architecture = "x86";
            if (arch == 0) { architecture = "x86"; }
            else if (arch == 9) { architecture = "x64"; }
            string filename = string.Format("{0}{1}", Path.DirectorySeparatorChar, Path.Combine(architecture, "BCD"));
            return ExtractFileFromVHD(filename, referenceVHD);
        }
        private byte[] ExtractBootmgrFromVHD(string referenceVHD, int arch)
        {
            string architecture = "x86";
            if (arch == 0)  { architecture = "x86"; }
            else if (arch == 9) { architecture = "x64"; }
            string filename = string.Format("{0}{1}", Path.DirectorySeparatorChar, Path.Combine(architecture, "bootmgr"));
            return ExtractFileFromVHD(filename, referenceVHD);
        }
        private byte[] ExtractFileFromVHD(string filename, string referenceVHD)
        {
            byte[] file = null;
            using (VirtualDisk vhdx = DiscUtils.Vhd.Disk.OpenDisk(referenceVHD, FileAccess.Read))
            {
                NtfsFileSystem vhdbNtfs = new NtfsFileSystem(vhdx.Partitions[0].Open());
                using (Stream bootStream = vhdbNtfs.OpenFile(filename, FileMode.Open, FileAccess.Read))
                {
                    file = new byte[bootStream.Length];
                    int totalRead = 0;
                    while (totalRead < file.Length)
                    {
                        totalRead += bootStream.Read(file, totalRead, file.Length - totalRead);
                    }
                }
            }
            return file;
        }


        private void AddBCD(NtfsFileSystem ntfs, byte[] BCD)
        {
            if (!ntfs.DirectoryExists(@"\boot"))
            {
                ntfs.CreateDirectory(@"\boot");
            }
            if (!ntfs.FileExists(@"\boot\BCD"))
            {
                using (Stream bcdStream = ntfs.OpenFile(@"\boot\BCD", FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    bcdStream.Write(BCD, 0, BCD.Length);
                }
            }
        }
        private void AddBootMgr(NtfsFileSystem ntfs, byte[] BootMgr)
        {
            if (!ntfs.FileExists(@"\bootmgr"))
            {
                using (Stream bcdStream = ntfs.OpenFile(@"\bootmgr", FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    bcdStream.Write(BootMgr, 0, BootMgr.Length);
                }
            }
        }

        private void FixBCD(NtfsFileSystem ntfs, VolumeManager volMgr)
        {
            if (ntfs.FileExists(@"\boot\BCD"))
            {
                // Force all boot entries in the BCD to point to the newly created NTFS partition - does _not_ cope with
                // complex multi-volume / multi-boot scenarios at all.
                using (Stream bcdStream = ntfs.OpenFile(@"\boot\BCD", FileMode.Open, FileAccess.ReadWrite))
                {
                    using (RegistryHive hive = new RegistryHive(bcdStream))
                    {
                        Store store = new Store(hive.Root);
                        foreach (var obj in store.Objects)
                        {
                            foreach (var elem in obj.Elements)
                            {
                                if (elem.Format == DiscUtils.BootConfig.ElementFormat.Device)
                                {
                                    elem.Value = DiscUtils.BootConfig.ElementValue.ForDevice(elem.Value.ParentObject, volMgr.GetPhysicalVolumes()[0]);
                                }
                            }
                        }
                    }
                }
            }
        }
        private string FindReferenceVHD(string referenceVHD)
        {
            string bzip2ext = Properties.Settings.Default.bzip2ext;
            string reffilename = Path.GetFileName(referenceVHD);
            string refVHD = null;
            string appdata = null;
            string datapath = null;

            appdata = System.Environment.GetEnvironmentVariable("APPDATA");
            if (string.IsNullOrEmpty(appdata))
            {
                appdata = System.Environment.GetEnvironmentVariable("ProgramData");
            }

            if (string.IsNullOrEmpty(appdata))
            {
                appdata = Path.Combine("C:", "Temp");
            }

            datapath = Path.Combine(appdata, Path.Combine("citrix", "temp"));

            if (!Directory.Exists(datapath))
                Directory.CreateDirectory(datapath);

            refVHD = Path.Combine(datapath, reffilename);
            if (!File.Exists(refVHD))
            {

                string apath = Assembly.GetExecutingAssembly().Location;
                string assempath = Path.GetDirectoryName(apath);
                while (assempath.Length > 3)
                {
                    string testPath = Path.Combine(assempath, referenceVHD);
                    if (File.Exists(testPath))
                    {
                        refVHD = testPath;
                        break;
                    }
                    else if (File.Exists(testPath + bzip2ext))
                    {
                        refVHD = testPath + bzip2ext;
                        break;
                    }
                    assempath = Path.GetDirectoryName(assempath);
                }

                if (Path.GetExtension(refVHD) == bzip2ext)
                {
                    string outfile = Path.Combine(datapath, Path.GetFileNameWithoutExtension(refVHD));
                    if (!File.Exists(outfile))
                    {
                        try
                        {
                            using (CompressionStream bzos = CompressionFactory.Writer(CompressionFactory.Type.Bz2, File.OpenWrite(outfile)))
                            {
                                bzos.BufferedWrite(File.OpenRead(refVHD));
                            }
                        }
                        finally { }
                    }
                    refVHD = outfile;
                    log.Info("A Compressed Reference VHD was found and uncompressed.");
                }
            }
            log.InfoFormat("Reference VHD: {0}", refVHD);
            return refVHD;
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
                OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.FileStart, "Web Download Start", downloadupdatemsg, 0, _filedownloadsize));
                WebClient wc = new WebClient();
                wc.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                wc.DownloadFileAsync(filetodownload, tmpfilename);
                uridownloadcomplete.WaitOne();
                if (_downloadexception != null)
                {
                    if (!Path.GetExtension(tmpfilename).Equals(".pvp"))  // don't worry bout pvp files, we don't use them.
                        throw _downloadexception;
                }
                OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.FileComplete, "Web Download Completed", downloadupdatemsg, _filedownloadsize, _filedownloadsize));
            }
            return tmpfilename;
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            OnUpdate(new XenOvfTranportEventArgs(XenOvfTranportEventType.FileProgress, "Web Download Update", downloadupdatemsg, (ulong)e.BytesReceived, (ulong)_filedownloadsize));
        }
        private void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                log.ErrorFormat("DownloadFileAsync: {0} ", e.Error.Message);
                _downloadexception = e.Error;
            }
            log.Info("DownloadFileAsync: completed");
            uridownloadcomplete.Set();
        }

        private static int comparePostInstallCommand(Xen_PostInstallOperationCommand_Type postOpLeft, Xen_PostInstallOperationCommand_Type postOpRight)
        {
            return postOpLeft.Order.CompareTo(postOpRight.Order);
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

    [Serializable]
    public class ImportException : Exception
    {
        public ImportException() : base() { }

        public ImportException(string message) : base(message) { }

        public ImportException(string message, Exception exception) : base(message, exception) { }
 
        public ImportException(SerializationInfo serialinfo, StreamingContext context) : base(serialinfo, context) { }
    }
}
