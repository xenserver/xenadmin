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
using System.IO;
using System.Management;
using System.Reflection;
using XenOvf.Definitions;
using XenOvf.Definitions.VMC;
using XenOvf.Utilities;
using XenCenterLib.Compression;
using XenCenterLib.Archive;

namespace XenOvf
{
    public partial class OVF
    {
        private ManagementObject Win32_ComputerSystem = null;
        private List<ManagementObject> Win32_Processor = new List<ManagementObject>();
        private List<ManagementObject> Win32_CDROMDrive = new List<ManagementObject>();
        private List<ManagementObject> Win32_DiskDrive = new List<ManagementObject>();
        private List<ManagementObject> Win32_NetworkAdapter = new List<ManagementObject>();
        private List<ManagementObject> Win32_IDEController = new List<ManagementObject>();
        private List<ManagementObject> Win32_SCSIController = new List<ManagementObject>();
        private List<ManagementObject> Win32_IDEControllerDevice = new List<ManagementObject>();
        private List<ManagementObject> Win32_SCSIControllerDevice = new List<ManagementObject>();
        private List<ManagementObject> Win32_DiskPartition = new List<ManagementObject>();
        private List<ManagementObject> Win32_DiskDriveToDiskPartition = new List<ManagementObject>();

        private Dictionary<string, string> mappings = new Dictionary<string, string>();

        #region CONVERSIONS

        public static void ConvertOVFtoOVA(EnvelopeType ovfEnv, string ovfPath, Action cancellingDelegate,
            bool compress, CompressionFactory.Type method = CompressionFactory.Type.Gz, bool deleteOriginalFiles = true)
        {
            string origDir = "";
            Stream ovaStream = null;
            FileStream fsStream = null;

            try
            {
                origDir = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(Path.GetDirectoryName(ovfPath));

                var ovfFileName = Path.GetFileName(ovfPath);
                var applianceName = Path.GetFileNameWithoutExtension(ovfFileName);
                string ovafilename = $"{applianceName}.ova";
                string manifestfile = $"{applianceName}.mf";
                string signaturefile = $"{applianceName}.cert";

                #region COMPRESSION STREAM

                if (compress)
                {
                    fsStream = new FileStream(ovafilename + method.FileExtension(), FileMode.CreateNew, FileAccess.Write, FileShare.None);
                    ovaStream = CompressionFactory.Writer(method, fsStream);
                }
                else
                {
                    ovaStream = new FileStream(ovafilename, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                }

                #endregion

                #region TAR

                // File Order is:
                // 1. OVF File
                // 2. Manifest (if exists)
                // 3. Signature File (if it and the manifest exist)
                // 4. All files listed in References.File.

                using (var tar = ArchiveFactory.Writer(ArchiveFactory.Type.Tar, ovaStream))
                {
                    AddFileToArchiveWriter(tar, ovfFileName, deleteOriginalFiles, cancellingDelegate);

                    if (File.Exists(manifestfile))
                    {
                        AddFileToArchiveWriter(tar, manifestfile, deleteOriginalFiles, cancellingDelegate);
                       
                        // Cannot exist without manifest file.
                        if (File.Exists(signaturefile))
                            AddFileToArchiveWriter(tar, signaturefile, deleteOriginalFiles, cancellingDelegate);
                    }

                    File_Type[] files = ovfEnv.References.File;
                    if (files != null)
                    {
                        foreach (File_Type file in files)
                            AddFileToArchiveWriter(tar, file.href, deleteOriginalFiles, cancellingDelegate);
                    }
                }

                #endregion
            }
            finally
            {
                ovaStream?.Close();
                fsStream?.Dispose();
                Directory.SetCurrentDirectory(origDir);
            }
        }

        private static void AddFileToArchiveWriter(ArchiveWriter tar, string fileName,
            bool deleteOriginalFile, Action cancellingDelegate)
        {
            using (FileStream fs = File.OpenRead(fileName))
                tar.Add(fs, fileName, File.GetCreationTime(fileName), cancellingDelegate);

            log.InfoFormat("Added file {0} to OVA archive", fileName);

            if (deleteOriginalFile)
            {
                try
                {
                    File.Delete(fileName);
                    log.InfoFormat("Deleted original file {0}", fileName);
                }
                catch
                {
                    // ignored
                }
            }
        }

        #endregion

        #region PRIVATE

        private void CollectInformation()
        {
            Win32_ComputerSystem = null;
            Win32_Processor.Clear();
            Win32_CDROMDrive.Clear();
            Win32_DiskDrive.Clear();
            Win32_NetworkAdapter.Clear();
            Win32_IDEController.Clear();
            Win32_SCSIController.Clear();
            Win32_SCSIControllerDevice.Clear();
            Win32_IDEControllerDevice.Clear();
            Win32_DiskPartition.Clear();


            ManagementObjectSearcher searcher = null;

            #region Win32_ComputerSystem

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_ComputerSystem");
                var mgtObjs = searcher.Get();

                // only want one.
                if (mgtObjs.Count > 0)
                {
                    using (var iterator = mgtObjs.GetEnumerator())
                    {
                        iterator.MoveNext();
                        Win32_ComputerSystem = (ManagementObject) iterator.Current;
                    }
                }
                log.Debug("OVF.CollectionInformation Win32_ComputerSystem.1");
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call to Win32_ComputerSystem failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_Processor

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_Processor");
                var mgtObjs = searcher.Get();

                // only want one.
                if (mgtObjs.Count > 0)
                {
                    using (var iterator = mgtObjs.GetEnumerator())
                    {
                        iterator.MoveNext();
                        Win32_Processor.Add((ManagementObject) iterator.Current);
                    }
                }

                log.DebugFormat("OVF.CollectionInformation Win32_Processor.{0}", Win32_Processor.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call to Win32_Processor failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_CDROMDrive

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_CDROMDrive");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_CDROMDrive.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_CDROMDrive.{0}", Win32_CDROMDrive.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call to Win32_CDROMDrive failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_DiskDrive

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_DiskDrive");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_DiskDrive.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_DiskDrive.{0}", Win32_DiskDrive.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call to Win32_CDROMDrive failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_NetworkAdapter

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_NetworkAdapter");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_NetworkAdapter.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_NetworkAdapter.{0}", Win32_NetworkAdapter.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call to Win32_NetworkAdapter failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_IDEController

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_IDEController");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_IDEController.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_IDEController.{0}", Win32_IDEController.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call for Win32_IDEController failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_SCSIController

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_SCSIController");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_SCSIController.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_SCSIController.{0}", Win32_SCSIController.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call for Win32_SCSIController failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_DiskPartition

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_DiskPartition");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_DiskPartition.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_DiskPartition.{0}", Win32_DiskPartition.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call for Win32_DiskPartition failed. Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

            #region Win32_DiskDriveToDiskPartition

            try
            {
                searcher = new ManagementObjectSearcher(@"select * from Win32_DiskDriveToDiskPartition");
                foreach (ManagementObject mgtobj in searcher.Get())
                {
                    Win32_DiskDriveToDiskPartition.Add(mgtobj);
                }
                log.DebugFormat("OVF.CollectionInformation Win32_DiskDriveToDiskPartition.{0}", Win32_DiskDriveToDiskPartition.Count);
            }
            catch (Exception ex)
            {
                log.WarnFormat("OVF.CollectionInformation: call for Win32_DiskDriveToDiskPartition failed, Exception: {0}", ex.Message);
            }
            finally
            {
                if (searcher != null)
                    searcher.Dispose();
                searcher = null;
            }

            #endregion

        }

        private void AddVssd(EnvelopeType ovfEnv, string vsId, string vhsId)
        {
            AddVssd(ovfEnv, vsId, vhsId, LANGUAGE);
        }

        private void AddVssd(EnvelopeType ovfEnv, string vsId, string vhsId, string lang)
        {

            if (Win32_ComputerSystem != null)
            {
                #region FIND BY PROPERTIES NOT EXPLICID

                string name = "Generic Computer";
                string caption = "Generic Caption";
                string description = "Autogenerated OVF/OVA Package";

                foreach (PropertyData pd in Win32_ComputerSystem.Properties)
                {
                    if (pd.Name.ToLower().Equals("name") && pd.Value != null)
                    {
                        name = (string)pd.Value;
                    }
                    else if (pd.Name.ToLower().Equals("caption") && pd.Value != null)
                    {
                        caption = (string)pd.Value;
                    }
                    else if (pd.Name.ToLower().Equals("description") && pd.Value != null)
                    {
                        description = (string)pd.Value;
                    }
                }

                #endregion

                UpdateVirtualSystemName(ovfEnv, vsId, lang, name);
                AddVirtualSystemSettingData(ovfEnv,
                    vsId,
                    vhsId,
                    name,
                    caption,
                    description,
                    Guid.NewGuid().ToString(),
                    "301"); // 301 == Microsoft (source), hvm-3.0-unknown == (xen source) Microsoft && Linux NOT PV'd, xen-3.0-unknown == PV'd

            }
            else
            {
                Random rand = new Random();
                string name = string.Format(Messages.AUTOGENERATED, rand.Next());
                string caption = string.Format(Messages.UNKNOWN);
                string description = string.Format(Messages.UNKNOWN);
                UpdateVirtualSystem(ovfEnv, vsId, lang, name);
                AddVirtualSystemSettingData(ovfEnv,
                    vsId,
                    vhsId,
                    name,
                    caption,
                    description,
                    Guid.NewGuid().ToString(),
                    "301");
                log.Warn("System definition not available, created defaults");
            }
            log.Debug("OVF.AddVssd completed");
        }

        private void AddNetworks(EnvelopeType ovfEnv, string vsId)
        {
            AddNetworks(ovfEnv, vsId, LANGUAGE);
        }

        private void AddNetworks(EnvelopeType ovfEnv, string vsId, string lang)
        {
            if (Win32_NetworkAdapter != null)
            {
                foreach (ManagementObject mo in Win32_NetworkAdapter)
                {
                    // Only get the physical adapters, not logical (which there are numerous)
                    //if ((bool)mo["PhysicalAdapter"])
                    bool addThisNetwork = false;
                    string macaddress = null;
                    string description = null;
                    //
                    // setPriority is used to determine the description
                    // 0 = unset
                    // 1 highest priority
                    // 2 next
                    // 3 next
                    // ...
                    //
                    int setPriority = 0;
                    foreach (PropertyData pd in mo.Properties)
                    {
                        if (pd.Name != null && pd.Name.Length > 0)
                        {
                            if (pd.Name.ToLower().Equals("macaddress") && pd.Value != null && ((string)pd.Value).Length > 0)
                            {
                                macaddress = (string)pd.Value;
                            }
                            else if (pd.Name.ToLower().Equals("netconnectionid") && pd.Value != null && ((string)pd.Value).Length > 0)
                            {
                                description = (string)pd.Value;
                                setPriority = 1;
                            }
                            else if (pd.Name.ToLower().Equals("name") && pd.Value != null && ((string)pd.Value).Length > 0)
                            {
                                if (setPriority == 0 || setPriority > 2)
                                {
                                    description = (string)pd.Value;
                                    setPriority = 2;
                                }
                            }
                            else if (pd.Name.ToLower().Equals("description") && pd.Value != null && ((string)pd.Value).Length > 0)
                            {
                                if (setPriority == 0 || setPriority > 3)
                                {
                                    description = (string)pd.Value;
                                    setPriority = 3;
                                }
                            }
                            // Below is trying to figure out if this is a Network Connection that
                            // is to be exported/defined.
                            // The issue is WMI has different value sets for different types of hardware
                            // such as hardware as we know it... pci ethernet
                            // or blade style, which WMI gives a different result.
                            // WAN/RAS connections.. etc.
                            // ANY one of the values can set this to true:
                            //      netconnectionstatus
                            //      pnpdeviceid
                            //      physicaladapter
                            //
                            else if (pd.Name.ToLower().Equals("netconnectionstatus") && pd.Value != null)
                            {
                                if ((UInt16)pd.Value == 0x2)
                                {
                                    addThisNetwork = true;
                                }
                            }
                            else if (pd.Name.ToLower().Equals("pnpdeviceid") && pd.Value != null && ((string)pd.Value).Length > 0)
                            {
                                if ((((string)pd.Value).ToLower().StartsWith("pci") || ((string)pd.Value).ToLower().StartsWith("scsi")))
                                {
                                    addThisNetwork = true;
                                }
                            }
                            else if (pd.Name.ToLower().Equals("physicaladapter") && pd.Value != null)
                            {
                                addThisNetwork = (bool)pd.Value;
                            }
                        }
                    }
                    if (addThisNetwork)
                    {
                        AddNetwork(ovfEnv, vsId, lang, Guid.NewGuid().ToString(), description, macaddress);
                    }
                }
            }
            else
            {
                log.Warn("No networks defined, If a network interface is required, the administrator will need to add it after import of OVF/OVA Package.");
            }
            log.DebugFormat("OVF.AddNetworks completed {0}", vsId);
        }

        private void AddCPUs(EnvelopeType ovfEnv, string vsId)
        {
            AddCPUs(ovfEnv, vsId, LANGUAGE);
        }

        private void AddCPUs(EnvelopeType ovfEnv, string vsId, string lang)
        {
            UInt64 cpucount = 0;
            if (Win32_Processor != null && Win32_Processor.Count > 0)
            {
                foreach (ManagementObject mo in Win32_Processor)
                {
                    #region FIND BY PROPERTIES NOT EXPLICID

                    uint numberofcores = 1;

                    foreach (PropertyData pd in mo.Properties)
                    {
                        if (pd.Name.ToLower().Equals("numberofcores") && pd.Value != null)
                        {
                            numberofcores = (uint)pd.Value;
                        }
                    }

                    #endregion

                    cpucount += Convert.ToUInt64(numberofcores);
                }

                SetCPUs(ovfEnv, vsId, cpucount);
            }
            else
            {
                SetCPUs(ovfEnv, vsId, 1);
                log.Warn("OVF.AddCPUs, set using default (1) CPU");
            }
            log.DebugFormat("OVF.AddCPUs completed {0} cpus {1}", vsId, cpucount);
        }

        private void AddMemory(EnvelopeType ovfEnv, string vsId)
        {
            AddMemory(ovfEnv, vsId, LANGUAGE);
        }

        private void AddMemory(EnvelopeType ovfEnv, string vsId, string lang)
        {
            ulong divisor = 1024*1024;
            ulong totalphysicalmemory = divisor*512; // 512MB Default Memory
            ulong memory = 0;

            if (Win32_ComputerSystem != null)
            {
                #region FIND BY PROPERTIES NOT EXPLICID

                foreach (PropertyData pd in Win32_ComputerSystem.Properties)
                {
                    if (pd.Name.ToLower().Equals("totalphysicalmemory") && pd.Value != null)
                    {
                        totalphysicalmemory = (ulong)pd.Value;
                        break;
                    }
                }

                #endregion

                memory = totalphysicalmemory/divisor;
            }
            else
            {
                log.Warn("OVF.AddMemory: could not determine system memory, defaulting to 512MB");
                memory = 512L;
            }
            SetMemory(ovfEnv, vsId, memory, "byte * 2^20");
            log.DebugFormat("OVF.AddMemory completed {0} memory {1} (byte * 2 ^ 20)", vsId, memory);
        }

        private void CreateConnectedDevices(EnvelopeType ovfEnv, string vsId, DiskInfo[] vhdExports)
        {
            CreateConnectedDevices(ovfEnv, vsId, LANGUAGE, vhdExports);
        }

        private void CreateConnectedDevices(EnvelopeType ovfEnv, string vsId, string lang, DiskInfo[] vhdExports)
        {
            //VirtualHardwareSection_Type vhs = FindVirtualHardwareSection(ovfEnv, vsId);
            bool guessPosition = true;
            int i = 0;

            #region IDE

            if (Win32_IDEController != null && Win32_IDEController.Count > 0)
            {
                foreach (ManagementObject mo in Win32_IDEController)
                {
                    #region FIND BY PROPERTIES NOT EXPLICID

                    string deviceid = null;
                    foreach (PropertyData pd in mo.Properties)
                    {
                        if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                        {
                            deviceid = (string)pd.Value;
                        }
                    }

                    #endregion

                    if (deviceid == null)
                    {
                        log.Debug("No device id defined, continuing");
                        continue;
                    }
                    List<ManagementObject> ControllerAssociations = FindDeviceReferences("Win32_IDEControllerDevice", deviceid);
                    string controllerInstanceId = Guid.NewGuid().ToString();

                    AddController(ovfEnv, vsId, DeviceType.IDE, controllerInstanceId, i++);
                    foreach (ManagementObject ca in ControllerAssociations)
                    {
                        #region FIND BY PROPERTIES NOT EXPLICID

                        string _dependent = null;
                        foreach (PropertyData pd in ca.Properties)
                        {
                            if (pd.Name.ToLower().Equals("dependent") && pd.Value != null)
                            {
                                _dependent = (string)pd.Value;
                            }
                        }
                        if (_dependent == null)
                        {
                            log.Debug("PCI Association not available, continuing.");
                            continue;
                        }

                        #endregion

                        string[] dependent = _dependent.Split(new char[] {'='});
                        string dependentId = dependent[dependent.Length - 1].Replace("\"", "");
                        dependentId = dependentId.Replace(@"\", "");
                        string startswith = dependentId; //.Replace(@"\", "");

                        if (startswith.ToUpper().StartsWith(@"IDEDISK"))
                        {
                            log.Debug("OVF.CreateConnectedDevices Checking IDEDISK");
                            foreach (ManagementObject md in Win32_DiskDrive)
                            {
                                #region FIND BY PROPERTIES NOT EXPLICID

                                string _deviceid = null;
                                string _pnpdeviceid = null;
                                UInt64 _size = 0;
                                foreach (PropertyData pd in md.Properties)
                                {
                                    if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                                    {
                                        _deviceid = (string)pd.Value;
                                    }
                                    else if (pd.Name.ToLower().Equals("pnpdeviceid") && pd.Value != null)
                                    {
                                        _pnpdeviceid = (string)pd.Value;
                                    }
                                    else if (pd.Name.ToLower().Equals("size") && pd.Value != null)
                                    {
                                        _size = (UInt64)pd.Value;
                                    }

                                }

                                #endregion

                                _pnpdeviceid = _pnpdeviceid.Replace(@"\", "");
                                if (_pnpdeviceid.Equals(dependentId))
                                {
                                    foreach (DiskInfo di in vhdExports)
                                    {
                                        if (_deviceid.Contains(di.DriveId))
                                        {
                                            try
                                            {
                                                log.DebugFormat("OVF.CreateConnectedDevices: Dependent: {0}  Device: {1}", dependentId, _deviceid);
                                                string diskInstanceId = Guid.NewGuid().ToString();
                                                int lastAmp = dependentId.LastIndexOf('&');
                                                if (lastAmp < 0) lastAmp = 0;
                                                string[] tmp = dependentId.Substring(lastAmp + 1).Split(new char[] {'.'});
                                                string address = null;
                                                if (tmp.Length >= 2)
                                                {
                                                    address = tmp[1];
                                                }
                                                else
                                                {
                                                    address = (guessPosition) ? "0" : "1";
                                                    guessPosition = !guessPosition;
                                                }
                                                address = address.Replace("&", "_");
                                                bool bootable = IsBootDisk(di.DriveId);
                                                AddDisk(ovfEnv, vsId, diskInstanceId, lang, di.VhdFileName, bootable,
                                                    _ovfrm.GetString("RASD_19_CAPTION"),
                                                    _ovfrm.GetString("RASD_19_DESCRIPTION"),
                                                    Convert.ToUInt64(di.PhysicalSize), Convert.ToUInt64(di.CapacitySize));
                                                AddDeviceToController(ovfEnv, vsId, diskInstanceId, controllerInstanceId, address);
                                                di.Added = true;
                                                log.DebugFormat("OVF.CreateConnectedDevices: {0} ({1}) added to {2}", di.DriveId, di.VhdFileName, dependentId);
                                            }
                                            catch (Exception ex)
                                            {
                                                string msg = string.Format("{0} [{1}] controller connection could not be identified.", "IDEDISK", _pnpdeviceid);
                                                log.ErrorFormat("OVF.CreateConnectedDevices: {0}", msg);
                                                throw new Exception(msg, ex);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (startswith.ToUpper().StartsWith(@"IDECDROM"))
                        {
                            log.Debug("OVF.CreateConnectedDevices Checking IDECDROM");
                            foreach (ManagementObject md in Win32_CDROMDrive)
                            {
                                #region FIND BY PROPERTIES NOT EXPLICID

                                string _pnpdeviceid = null;
                                foreach (PropertyData pd in md.Properties)
                                {
                                    if (pd.Name.ToLower().Equals("pnpdeviceid") && pd.Value != null)
                                    {
                                        _pnpdeviceid = (string)pd.Value;
                                    }
                                }
                                if (_pnpdeviceid == null)
                                {
                                    log.Debug("PNPDeviceID not available, continuing.");
                                    continue;
                                }

                                #endregion

                                _pnpdeviceid = _pnpdeviceid.Replace(@"\", "");
                                if (_pnpdeviceid.Equals(dependentId))
                                {
                                    log.DebugFormat("OVF.CreateConnectedDevices: Dependent: {0}  Device: {1}", dependentId, _pnpdeviceid);
                                    try
                                    {
                                        string diskInstanceId = Guid.NewGuid().ToString();
                                        int lastAmp = dependentId.LastIndexOf('&');
                                        if (lastAmp < 0) lastAmp = 0;
                                        string[] tmp = dependentId.Substring(lastAmp + 1).Split(new char[] {'.'});
                                        //string[] tmp = dependentId.Split(new char[] { '.' });
                                        string address = tmp[1];
                                        int idetest = Convert.ToInt32(address);
                                        if (idetest != 0 && idetest != 1)
                                        {
                                            address = "0";
                                        }
                                        AddCDROM(ovfEnv, vsId, diskInstanceId, _ovfrm.GetString("RASD_16_CAPTION"), _ovfrm.GetString("RASD_16_ELEMENTNAME"));
                                        AddDeviceToController(ovfEnv, vsId, diskInstanceId, controllerInstanceId, address);
                                        log.DebugFormat("OVF.CreateConnectedDevices: CDROM added to {0}", dependentId);
                                    }
                                    catch
                                    {
                                        log.WarnFormat("OVF.CreateConnectedDevices: CDROM [{0}] controller connection could not be identified, skipped.", _pnpdeviceid);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                log.Info("OVF.CreateConnectedDevices NO IDE controllers detected.");
            }
            log.Debug("OVF.CreateConnectedDevices IDE Controllers completed.");

            #endregion

            #region SCSI

            if (Win32_SCSIController != null && Win32_SCSIController.Count > 0)
            {
                foreach (ManagementObject device in Win32_SCSIController)
                {
                    #region FIND BY PROPERTIES NOT EXPLICID

                    string _deviceid = null;
                    foreach (PropertyData pd in device.Properties)
                    {
                        if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                        {
                            _deviceid = (string)pd.Value;
                        }
                    }
                    if (_deviceid == null)
                    {
                        log.Debug("SCSI DeviceID not available, continuing.");
                        continue;
                    }

                    #endregion

                    List<ManagementObject> ControllerAssociations = FindDeviceReferences("Win32_SCSIControllerDevice", _deviceid);
                    string controllerInstanceId = Guid.NewGuid().ToString();

                    if (ControllerAssociations == null || ControllerAssociations.Count <= 0)
                    {
                        log.DebugFormat("No Controller associations for {0}", _deviceid);
                        continue;
                    }

                    AddController(ovfEnv, vsId, DeviceType.SCSI, controllerInstanceId, i++);
                    foreach (ManagementObject ca in ControllerAssociations)
                    {
                        #region FIND BY PROPERTIES NOT EXPLICID

                        string _dependent = null;
                        foreach (PropertyData pd in ca.Properties)
                        {
                            if (pd.Name.ToLower().Equals("dependent") && pd.Value != null)
                            {
                                _dependent = (string)pd.Value;
                            }
                        }
                        if (_dependent == null)
                        {
                            log.Debug("SCSI Association not available, continuing.");
                            continue;
                        }

                        #endregion

                        string[] dependent = _dependent.Split(new char[] {'='});
                        string dependentId = dependent[dependent.Length - 1].Replace("\"", "");
                        dependentId = dependentId.Replace(@"\", "");
                        string startswith = dependentId; //.Replace(@"\", "");

                        if (startswith.ToUpper().StartsWith(@"SCSIDISK"))
                        {
                            foreach (ManagementObject md in Win32_DiskDrive)
                            {
                                #region FIND BY PROPERTIES NOT EXPLICID

                                string __deviceid = null;
                                string __pnpdeviceid = null;
                                UInt32 __scsibus = 0;
                                UInt16 __scsilogicalunit = 0;
                                UInt16 __scsiport = 0;
                                UInt16 __scsitargetid = 0;
                                UInt64 __size = 0;
                                foreach (PropertyData pd in md.Properties)
                                {
                                    if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                                    {
                                        __deviceid = (string)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("pnpdeviceid") && pd.Value != null)
                                    {
                                        __pnpdeviceid = (string)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsibus") && pd.Value != null)
                                    {
                                        __scsibus = (UInt32)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsilogicalunit") && pd.Value != null)
                                    {
                                        __scsilogicalunit = (UInt16)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsiport") && pd.Value != null)
                                    {
                                        __scsiport = (UInt16)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsitargetid") && pd.Value != null)
                                    {
                                        __scsitargetid = (UInt16)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("size") && pd.Value != null)
                                    {
                                        __size = (UInt64)pd.Value;
                                    }
                                }
                                if (__deviceid == null)
                                {
                                    log.Debug("SCSI DeviceID not available, continuing.");
                                    continue;
                                }

                                #endregion

                                __pnpdeviceid = __pnpdeviceid.Replace(@"\", "");
                                if (__pnpdeviceid.Equals(dependentId))
                                {
                                    foreach (DiskInfo di in vhdExports)
                                    {
                                        if (__deviceid.Contains(di.DriveId))
                                        {
                                            string diskInstanceId = Guid.NewGuid().ToString();
                                            string _description = string.Format(_ovfrm.GetString("RASD_CONTROLLER_SCSI_DESCRIPTION"), __scsibus, __scsilogicalunit, __scsiport, __scsitargetid);
                                            bool bootable = IsBootDisk(di.DriveId);
                                            AddDisk(ovfEnv, vsId, diskInstanceId, lang, di.VhdFileName, bootable, _ovfrm.GetString("RASD_19_CAPTION"), _description, Convert.ToUInt64(di.PhysicalSize), Convert.ToUInt64(di.CapacitySize));
                                            AddDeviceToController(ovfEnv, vsId, diskInstanceId, controllerInstanceId, Convert.ToString(__scsiport));
                                            di.Added = true;
                                            log.DebugFormat("CreateConnectedDevices: {0} ({1}) added to {2}", di.DriveId, di.VhdFileName, dependentId);
                                        }
                                    }
                                }
                            }
                        }
                        else if (startswith.ToUpper().StartsWith(@"SCSICDROM"))
                        {
                            foreach (ManagementObject md in Win32_CDROMDrive)
                            {
                                #region FIND BY PROPERTIES NOT EXPLICID

                                string __deviceid = null;
                                string __pnpdeviceid = null;
                                UInt32 __scsibus = 0;
                                UInt16 __scsilogicalunit = 0;
                                UInt16 __scsiport = 0;
                                UInt16 __scsitargetid = 0;
                                foreach (PropertyData pd in md.Properties)
                                {
                                    if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                                    {
                                        __deviceid = (string)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("pnpdeviceid") && pd.Value != null)
                                    {
                                        __pnpdeviceid = (string)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsibus") && pd.Value != null)
                                    {
                                        __scsibus = (UInt32)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsilogicalunit") && pd.Value != null)
                                    {
                                        __scsilogicalunit = (UInt16)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsiport") && pd.Value != null)
                                    {
                                        __scsiport = (UInt16)pd.Value;
                                    }
                                    if (pd.Name.ToLower().Equals("scsitargetid") && pd.Value != null)
                                    {
                                        __scsitargetid = (UInt16)pd.Value;
                                    }
                                }
                                if (__deviceid == null)
                                {
                                    log.Debug("SCSI DeviceID not available, continuing.");
                                    continue;
                                }

                                #endregion

                                __pnpdeviceid = __pnpdeviceid.Replace(@"\", "");
                                if (__pnpdeviceid.Equals(dependentId))
                                {
                                    string diskInstanceId = Guid.NewGuid().ToString();
                                    string caption = string.Format(_ovfrm.GetString("RASD_CONTROLLER_SCSI_DESCRIPTION"), __scsibus, __scsilogicalunit, __scsiport, __scsitargetid);
                                    AddCDROM(ovfEnv, vsId, diskInstanceId, caption, _ovfrm.GetString("RASD_16_DESCRIPTION"));
                                    AddDeviceToController(ovfEnv, vsId, diskInstanceId, controllerInstanceId, Convert.ToString(__scsiport));
                                    log.DebugFormat("CreateConnectedDevices: CDROM added to {0}", dependentId);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                log.Info("OVF.CreateConnectedDevices no SCSI Controllers detected.");
            }
            log.DebugFormat("OVF.CreateConnectedDevices SCSI Controllers completed {0} ", vsId);

            #endregion

            #region OTHER CONTROLLER DISKS

            // These are disks that were not found on an IDE or SCSI, but exist and want to be exported.
            // these could be USB, 1394 etc.
            foreach (DiskInfo di in vhdExports)
            {
                if (!di.Added)
                {
                    UInt64 _size = 0;
                    string diskInstanceId = Guid.NewGuid().ToString();
                    string _deviceid = null;
                    string _mediatype = null;
                    foreach (ManagementObject md in Win32_DiskDrive)
                    {
                        #region FIND BY PROPERTIES NOT EXPLICID

                        foreach (PropertyData pd in md.Properties)
                        {
                            if (pd.Name.ToLower().Equals("deviceid") && pd.Value != null)
                            {
                                _deviceid = (string)pd.Value;
                            }
                            else if (pd.Name.ToLower().Equals("mediatype") && pd.Value != null)
                            {
                                _mediatype = (string)pd.Value;
                            }
                            else if (pd.Name.ToLower().Equals("size") && pd.Value != null)
                            {
                                _size = (UInt64)pd.Value;
                            }

                        }

                        #endregion
                    }

                    bool bootable = IsBootDisk(di.DriveId);
                    AddDisk(ovfEnv, vsId, diskInstanceId, lang, di.VhdFileName, bootable, _ovfrm.GetString("RASD_19_CAPTION"), _mediatype, Convert.ToUInt64(di.PhysicalSize), Convert.ToUInt64(di.CapacitySize));
                    di.Added = true;
                    log.DebugFormat("CreateConnectedDevices: {0} ({1}) added to {2}", di.DriveId, di.VhdFileName, _mediatype);
                }
            }
            log.DebugFormat("OVF.CreateConnectedDevices OTHER Controllers completed {0} ", vsId);

            #endregion

            #region CHECK ALL DISKS WERE DEFINED

            foreach (DiskInfo di in vhdExports)
            {
                if (!di.Added)
                {
                    AddDisk(ovfEnv, vsId, Guid.NewGuid().ToString(), lang, di.VhdFileName, false, _ovfrm.GetString("RASD_19_CAPTION"), _ovfrm.GetString("RASD_19_DESCRIPTION"), Convert.ToUInt64(di.PhysicalSize), Convert.ToUInt64(di.CapacitySize));
                    di.Added = true;
                    log.Warn("CreateConnectedDevices: MANUAL Update of OVF REQUIRED TO DEFINE: Disk Size and Capacity");
                    log.WarnFormat("CreateConnectedDevices: {0} ({1}) NOT FOUND, added as Unknown with 0 Size", di.DriveId, di.VhdFileName);
                }
            }

            #endregion

            log.DebugFormat("OVF.CreateConnectedDevices completed {0} ", vsId);
        }

        #endregion
    }
}
