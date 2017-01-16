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
using System.IO;
using System.Xml;
using XenOvf.Definitions;
using XenOvf.Utilities;

namespace XenOvf
{
    /// <summary>
    /// 
    /// </summary>
    public partial class OVF
    {
        /// <summary>
        /// ValidationFlags are binary flags for the individual selection of what to validate
        /// None = No validation
        /// Version = Check the OVF Version (if present)
        /// Files = Ensure the files listed in References.Files are present and accessible.
        /// Cpu = Ensure a CPU RASD is present
        /// Memory = Ensure a Memory RASD is present
        /// Networks = Validate Network RASD is connected to a NETWORK
        /// Capability = Ensure import can handle required RASDs and Sections.
        /// Schema = Validate the OVF XML against the Schema.
        /// </summary>
        [Flags]
        public enum ValidationFlags
        {
            /// <summary>
            /// 
            /// </summary>
            None = 0,
            /// <summary>
            /// 
            /// </summary>
            Version = 1,
            /// <summary>
            /// 
            /// </summary>
            Files = 2,
            /// <summary>
            /// 
            /// </summary>
            Cpu = 4,
            /// <summary>
            /// 
            /// </summary>
            Memory = 8,
            /// <summary>
            /// 
            /// </summary>
            Networks = 16,
            /// <summary>
            /// 
            /// </summary>
            Capability = 32,
            /// <summary>
            /// 
            /// </summary>
            Schema = 64
        };

        #region VALIDATION ROUTINES
        /// <summary>
        /// Statically validate the OVF, perform check and values based upon a static Envelope.
        /// A more through check could be done IF destination resources where available but that is out of scope for this method.
        /// </summary>
        /// <param name="ovffilename">OVF Filename with full Path to where vhd/ovf files currently exist</param>
        /// <returns>true=valid; false=not-valid;  true can also have warnings in ValidationErrorMessage</returns>
		public static bool Validate(string ovffilename, out List<string> validationErrorMessages)
        {
        	validationErrorMessages = new List<string>();
			
			var validationFlags = ValidationFlags.None;
			string[] lvls = Properties.Settings.Default.RequiredValidations.Split(new char[] { ',' });
			foreach (string lvl in lvls)
				validationFlags = validationFlags | (ValidationFlags)Enum.Parse(typeof(ValidationFlags), lvl, true);

            bool isValid = true;
            string StartPath = Directory.GetCurrentDirectory();

            if (!File.Exists(ovffilename))
            {
                throw new FileNotFoundException(ovffilename);
            }

            string ovfpath = Path.GetDirectoryName(ovffilename);

            if (!Directory.Exists(ovfpath))
            {
                throw new FileNotFoundException(string.Format(Messages.FILE_MISSING, ovfpath));
            }

            Directory.SetCurrentDirectory(ovfpath);

            EnvelopeType ovfEnv = Tools.LoadOvfXml(ovffilename);

            File_Type[] files = null;
            VirtualDiskDesc_Type[] disks = null;

            if (ovfEnv.References != null)
            {
                files = ovfEnv.References.File;
            }
            NetworkSection_TypeNetwork[] networks = null;
            Content_Type[] systems = null;
            RASD_Type[] rasds = null;

            #region : REQUIRED : CHECK IF VHD HARD DISK IMAGES
            // Cannot turn this OFF.
            if (files != null)
            {
                isValid = ValidateVHD(files);
            }
            #endregion

            #region : REQUIRED : CHECK VERSION
            isValid = ValidateVersion(ovfEnv, validationFlags, ref validationErrorMessages);
            #endregion

            #region : REQUIRED : VALIDATE AGAINST SCHEMA
			isValid = ValidateSchema(ovffilename, validationFlags, ref validationErrorMessages);
            #endregion

            #region : REQUIRED : GET OUTER STRUCTURE ELEMENTS
            foreach (var section in ovfEnv.Sections)
            {
                if (section is DiskSection_Type)
                {
                    disks = ((DiskSection_Type)section).Disk;
                }
                else if (section is NetworkSection_Type)
                {
                    networks = ((NetworkSection_Type)section).Network;
                }
            }

            if (ovfEnv.Item is VirtualSystemCollection_Type)
            {
                systems = ((VirtualSystemCollection_Type)ovfEnv.Item).Content;
            }
            else if (ovfEnv.Item is VirtualSystem_Type)
            {
                systems = new [] { ovfEnv.Item };
            }
            else
            {
                isValid = false;
                var message = string.Format(Messages.VALIDATION_INVALID_TYPE, ovfEnv.Item.GetType().ToString());
                log.Error(message);
                validationErrorMessages.Add(message);
            }
            #endregion

            #region  : REQUIRED : Validate : Files, Cpu, Memory, Networks, Capabilities
            if (isValid)
            {
                foreach (VirtualSystem_Type system in systems)
                {
					isValid = ValidateVHS(system, ref validationErrorMessages);

                    foreach (var section in system.Items)
                    {
                        if (section is VirtualHardwareSection_Type)
                        {
                            VirtualHardwareSection_Type vhs = (VirtualHardwareSection_Type)section;
                            rasds = vhs.Item;
							if ((!ValidateFiles(ovfpath, files, disks, rasds, validationFlags, ref validationErrorMessages)) ||
								(!ValidateCpu(rasds, validationFlags, ref validationErrorMessages)) ||
								(!ValidateMemory(rasds, validationFlags, ref validationErrorMessages)) ||
								(!ValidateNetworks(networks, rasds, validationFlags, ref validationErrorMessages)) ||
								(!ValidateCapability(rasds, validationFlags, ref validationErrorMessages))
                               )
                            {
                                validationErrorMessages.Add(string.Format(Messages.VALIDATION_FAILURE, ovffilename));
                                isValid = false;
                            }
                        }
                    }
                }
            }
            #endregion

            if (!isValid)
            {
				bool enforceValidation = Properties.Settings.Default.enforceValidation;

                log.Error(enforceValidation
                              ? "OVF Failed Validation, return failure"
                              : "OVF Failed Validation, OVERRIDE return success");

                if (!enforceValidation)
                    isValid = true;
            }

            Directory.SetCurrentDirectory(StartPath);

            return isValid;
        }

        #endregion
        
        #region PRIVATE
		private static bool ValidateVersion(EnvelopeType ovfEnv, ValidationFlags validationFlags, ref List<string> validationErrorMessages)
        {
            bool isValid = true;
            if ((validationFlags & ValidationFlags.Version) != 0)
            {
                if (ovfEnv.version == null)
                {
                    if (ovfEnv.AnyAttr != null && ovfEnv.AnyAttr.Length > 0)
                    {
                        foreach (XmlAttribute attr in ovfEnv.AnyAttr)
                        {
                            if (attr.Name.ToLower().Contains("version"))
                            {
                                ovfEnv.version = attr.Value;
                                break;
                            }
                        }
                    }
                    if (ovfEnv.version == null)
                    {
                        log.Warn("Version not set, applying 1.0.0");
                        ovfEnv.version = "1.0.0";
                    }
                }
                if (!Properties.Settings.Default.Versions.Contains(ovfEnv.version))
                {
                    isValid = false;
                    var message = string.Format(Messages.VALIDATION_INVALID_VERSION, ovfEnv.version);
                    log.Warn(message);
                    validationErrorMessages.Add(message);
                }
            }
            return isValid;
        }
        private static bool ValidateVHD(File_Type[] files)
        {
            // MUST BE Performed cannot skip.
            bool isValid = true;
            foreach (File_Type file in files)
            {
				string ext = Path.GetExtension(file.href).ToLower();
				if (ext == Properties.Settings.Default.manifestFileExtension || ext == Properties.Settings.Default.certificateFileExtension)
					continue;
                
				if (File.Exists(file.href))
                {    
                    if (!Properties.Settings.Default.knownFileExtensions.ToLower().Contains(ext))
                    {
                        log.WarnFormat(Messages.VALIDATION_INVALID_FILETYPE, file.href);
                    }
                }
                else
                {
                    var message = string.Format(Messages.VALIDATION_FILE_NOTFOUND, file.href);
                    log.Error(message);
                    throw new Exception(message);
                }
            }
            return isValid;
        }
		private static bool ValidateFiles(string ovfpath, File_Type[] files, VirtualDiskDesc_Type[] disks, RASD_Type[] rasds, ValidationFlags validationFlags, ref List<string> validationErrorMessages)
        {
            bool isValid = true;
            if ((validationFlags & ValidationFlags.Files) != 0)
            {
                if (files != null && files.Length > 0)
                {
                    foreach (File_Type file in files)
                    {
						string ext = Path.GetExtension(file.href).ToLower();
						if (ext == Properties.Settings.Default.manifestFileExtension || ext == Properties.Settings.Default.certificateFileExtension)
							continue;

						string filename = string.Format(@"{0}{1}{2}", string.IsNullOrEmpty(ovfpath) ? "" : ovfpath,
																	  string.IsNullOrEmpty(ovfpath) ? "" : @"\",
																	  file.href);
                        if (!File.Exists(filename))
                        {
                            var message = string.Format(Messages.VALIDATION_FILE_NOTFOUND, file.href);
                            validationErrorMessages.Add(message);
                            log.Error(message);
                            throw new Exception(message);
                        }
                    }
                }
                else
                {
                    log.Info("ValidateFiles: no attached files defined, continuing");
                    return isValid;
                }
                if (isValid)
                {
                    bool validlink = false;
                    foreach (File_Type file in files)
                    {
                        validlink = false;
                        foreach (VirtualDiskDesc_Type disk in disks)
                        {
                            if (file.id == disk.fileRef)
                            {
                                foreach (RASD_Type rasd in rasds)
                                {
                                    if (rasd.ResourceType.Value == 17 ||
                                        rasd.ResourceType.Value == 19 ||
                                        rasd.ResourceType.Value == 20 ||
                                        rasd.ResourceType.Value == 21)
                                    {
                                        if (rasd.HostResource != null && rasd.HostResource.Length > 0)
                                        {
                                            if (rasd.HostResource[0].Value.Contains(disk.diskId))
                                            {
                                                validlink = true;
                                                break;
                                            }
                                        }
                                        else if (disk.diskId == rasd.InstanceID.Value)
                                        {
                                            validlink = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (!validlink)
                        {
                            log.WarnFormat("Disk linkage [File to RASD] does not exist: {0}", file.href);
                            break;
                        }
                    }
                }
            }
            return isValid;
        }
		private static bool ValidateCpu(RASD_Type[] rasds, ValidationFlags validationFlags, ref List<string> validationErrorMessages)
        {
            bool isValid = true;
            if ((validationFlags & ValidationFlags.Cpu) != 0)
            {
                foreach (RASD_Type rasd in rasds)
                {
                    if (rasd.ResourceType.Value == 3)
                    {
                        if (rasd.VirtualQuantity == null || rasd.VirtualQuantity.Value <= 0)
                        {
                            var message = string.Format(Messages.VALIDATION_INVALID_CPU_QUANTITY, rasd.VirtualQuantity.Value);
                            log.Error(message);
                            validationErrorMessages.Add(message);
                            break;
                        }
                        if (rasd.Limit != null && rasd.VirtualQuantity.Value > rasd.Limit.Value)
                        {
                            var message = string.Format(Messages.VALIDATION_INVALID_CPU_EXCEEDS_LIMIT, rasd.VirtualQuantity.Value, rasd.Limit.Value);
                            log.Error(message);
                            validationErrorMessages.Add(message);
                            isValid = false;
                            break;
                        }
                        if (rasd.InstanceID == null || rasd.InstanceID.Value.Length <= 0)
                        {
                            log.Info("CPU has an invalid InstanceID, creating new.");
                            validationErrorMessages.Add(Messages.VALIDATION_INVALID_INSTANCEID);
                            rasd.InstanceID = new cimString(Guid.NewGuid().ToString());
                            break;
                        }
                    }
                }
            }
            return isValid;
        }
		private static bool ValidateMemory(RASD_Type[] rasds, ValidationFlags validationFlags, ref List<string> validationErrorMessages)
        {
            bool isValid = true;
            if ((validationFlags & ValidationFlags.Memory) != 0)
            {
                foreach (RASD_Type rasd in rasds)
                {
                    if (rasd.ResourceType.Value == 4)
                    {
                        if (rasd.VirtualQuantity == null || rasd.VirtualQuantity.Value <= 0)
                        {
                            log.Error("Memory invalid Virtual Quantity");
                            validationErrorMessages.Add(Messages.VALIDATION_INVALID_MEMORY_QUANTITY);
                            isValid = false;
                            break;
                        }
                        if (rasd.AllocationUnits == null || rasd.AllocationUnits.Value.Length <= 0)
                        {
                            log.Error("Memory AllocationUnits not valid");
                            validationErrorMessages.Add(Messages.VALIDATION_INVALID_MEMORY_ALLOCATIONUNITS);
                            isValid = false;
                            break;
                        }
                        if (rasd.InstanceID == null || rasd.InstanceID.Value.Length <= 0)
                        {
                            log.Info("Memory has an invalid InstanceID, creating new.");
                            validationErrorMessages.Add(Messages.VALIDATION_INVALID_INSTANCEID);
                            rasd.InstanceID = new cimString(Guid.NewGuid().ToString());
                            break;
                        }
                    }
                }
            }
            return isValid;
        }
		private static bool ValidateNetworks(NetworkSection_TypeNetwork[] networks, RASD_Type[] rasds, ValidationFlags validationFlags, ref List<string> validationErrorMessages)
        {
            bool isValid = true;
            if ((validationFlags & ValidationFlags.Networks) != 0)
            {
                foreach (RASD_Type rasd in rasds)
                {
                    if (rasd.ResourceType.Value == 10)
                    {
                        bool linkage = false;
                        if (rasd.InstanceID == null || rasd.InstanceID.Value.Length <= 0)
                        {
                            log.Info("Network has an invalid InstanceID, creating new.");
                            validationErrorMessages.Add(Messages.VALIDATION_INVALID_INSTANCEID);
                            rasd.InstanceID = new cimString(Guid.NewGuid().ToString());
                            break;
                        }
                        foreach (NetworkSection_TypeNetwork net in networks)
                        {
                            //
                            // this may only work for Citrix Created VOFs
                            // haven't looked at others, may need to use a different key to validate linkage.
                            //
                            if ((rasd.Connection != null && rasd.Connection.Length > 0 &&
                                 net.name == rasd.Connection[0].Value) ||
                                (net.Description.msgid == rasd.InstanceID.Value))
                            {
                                linkage = true;
                            }
                        }
                        if (!linkage)
                        {
                            log.Error(Messages.VALIDATION_NETWORK_NO_DEVICE);
                            validationErrorMessages.Add(Messages.VALIDATION_NETWORK_NO_DEVICE);
                            isValid = false;
                            break;
                        }
                    }
                }
            }
            return isValid;
        }
		private static bool ValidateCapability(RASD_Type[] rasds, ValidationFlags validationFlags, ref List<string> validationErrorMessages)
        {
            bool isValid = true;
            if ((validationFlags & ValidationFlags.Capability) != 0)
            {
                foreach (RASD_Type rasd in rasds)
                {
                    switch (rasd.ResourceType.Value)
                    {
                        // CIM SCHEMA 2.19.0
                        case 3:  // Processor
                        case 4:  // Memory
                        case 5:  // IDE Controller
                        case 6:  // Parallel SCSI HBA
                        case 7:  // FC HBA
                        case 8:  // iSCSI HBA
                        case 9:  // IB HCA
                        case 10: // Ehternet Adapter
                        case 15: // CD Drive
                        case 16: // DVD Drive
                        case 17: // Disk Drive
                        case 19: // Storage Extent
                        case 20: // Other storage Device
                        case 21: // Serial Port  // Microsoft uses this for Hard Disk Image also, based on an OLDER schema
                            {
                                if (rasd.required)
                                {
                                    isValid = true;
                                }
                                break;
                            }
                        case 1:  // Other
                        case 2:  // Computer System
                        case 11: // Other Network Adapter
                        case 12: // I/O Slot
                        case 13: // I/O Device
                        case 14: // Floppy Drive
                        case 18: // Tape Drive
                        case 22: // Parallel Port
                        case 23: // USB Controller
                        case 24: // Graphics Controller
                        case 25: // IEEE 1394 Controller
                        case 26: // Partitionable Unit
                        case 27: // Base Partitionable Unit
                        case 28: // Power
                        case 29: // Cooling Capacity
                        case 30: // Ethernet Switch Port
                        case 31: // Logical Disk
                        case 32: // Storage Volume
                        case 33: // Ethernet Connection
                        default:
                            {
                                if (rasd.required)
                                {
                                    var message = string.Format(Messages.VALIDATION_REQUIRED_ELEMENT_NOT_RECOGNIZED, rasd.ResourceType.Value, rasd.ElementName.Value);
                                    log.Error(message);
                                    validationErrorMessages.Add(message);
                                    isValid = false;
                                }
                                break;
                            }
                    }
                    if (!isValid)
                        break;
                }
            }
            return isValid;
        }
		private static bool ValidateSchema(string ovffilename, ValidationFlags validationFlags, ref List<string> validationErrorMessages)
        {
            bool isValid = true;
            if ((validationFlags & ValidationFlags.Schema) != 0)
            {
                try
                {
                    isValid = Tools.ValidateXmlToSchema(ovffilename);
                }
                catch (Exception ex)
                {
                    isValid = false;
                    validationErrorMessages.Add(ex.Message);
                }
                if (!isValid)
                {
                    isValid = false;
                    log.Error(Messages.VALIDATION_SCHEMA_FAILED);
                    validationErrorMessages.Add(Messages.VALIDATION_SCHEMA_FAILED);                    
                }
            }
            return isValid;
        }
		private static bool ValidateVHS(VirtualSystem_Type vs, ref List<string> validationErrorMessages)
        {
            bool isValid = true;
            VirtualHardwareSection_Type vhs = null;
            foreach (Section_Type section in vs.Items)
            {
                if (section is VirtualHardwareSection_Type)
                {
                    vhs = (VirtualHardwareSection_Type)section;
                    if (vhs.System != null && vhs.System.VirtualSystemIdentifier != null && vhs.System.VirtualSystemIdentifier.Value != null)
                    {
                        if (!Properties.Settings.Default.knownVirtualSystemTypes.Contains(vhs.System.VirtualSystemType.Value))
                        {
                            var message = string.Format(Messages.VALIDATION_UNKNOWN_HARDWARE_TYPE, vhs.System.VirtualSystemType.Value);
                            log.Warn(message);
                            validationErrorMessages.Add(message);
                            isValid = false;
                        }
                    }                    
                }
            }
            return isValid;     
        }
        #endregion
    }
}