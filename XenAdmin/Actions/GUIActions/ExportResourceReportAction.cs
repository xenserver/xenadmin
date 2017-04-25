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
using XenAdmin.Core;
using System.IO;
using XenAdmin.Network;
using System.Threading;
using XenAPI;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using XenAdmin.XenSearch;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace XenAdmin.Actions
{
    public class ExportResourceReportAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _filename;
        private readonly int _fileType;
        private Exception _exception = null;
        
        private List<HostInfo> m_Hosts;
        private List<SRInfo> m_SRs;
        private List<NetworkInfo> m_Networks;
        private List<VMInfo> m_VMs;
        private List<PGPUInfo> m_PGPUs;
        private List<VDIInfo> m_VDIs;
        private static MetricUpdater MetricUpdater;
        
        private long itemCount = 0;
        private long itemIndex = 0;
        private long baseIndex = 60;
        
        enum FILE_TYPE_INDEX { XLS = 1, CSV = 2 };
        
        /// <summary> 
        /// used for generate resource report
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="filename"></param>
        public ExportResourceReportAction(IXenConnection connection, string filename, int fileType)
            : base(connection, string.Format(Messages.ACTION_EXPORT_POOL_RESOURCE_LIST_FROM_X, Helpers.GetName(connection)),
            Messages.ACTION_EXPORT_DESCRIPTION_PREPARING)
        {
            Pool = Helpers.GetPool(connection);
            _filename = filename;
            _fileType = fileType;
            MetricUpdater = new MetricUpdater();
            MetricUpdater.SetXenObjects(connection.Cache.Hosts);
            MetricUpdater.SetXenObjects(connection.Cache.VMs);
            MetricUpdater.UpdateMetricsOnce();
            itemCount = connection.Cache.Hosts.Length + connection.Cache.Networks.Length + connection.Cache.SRs.Length + connection.Cache.VMs.Length;
        }

        protected override void Run()
        {
            SafeToExit = false;
            Description = Messages.ACTION_EXPORT_DESCRIPTION_IN_PROGRESS;

            if (Cancelling)
                throw new CancelledException();

            log.DebugFormat("Exporting resource list report from {1} to {2}", this.Connection.Cache.Pools[0].Name, _filename);

            try
            {
                DoExport();
            }
            catch (Exception e)
            {
                // Test for null: don't overwrite a previous exception
                if (_exception == null)
                    _exception = e;
            }

            PercentComplete = 100;
            if (Cancelling || _exception is CancelledException)
            {
                log.InfoFormat("Export of Pool {0} cancelled", this.Connection.Cache.Pools[0].Name);
                this.Description = Messages.ACTION_EXPORT_DESCRIPTION_CANCELLED;

                log.DebugFormat("Deleting {0}", _filename);
                try
                {
                    File.Delete(_filename);
                }
                catch (Exception e)
                {
                    this.Description = string.Format(Messages.ACTION_EXPORT_POOL_RESOURCE_CANCEL_AND_FILE_UNDELETE, _filename);
                    log.Warn(string.Format(Messages.ACTION_EXPORT_POOL_RESOURCE_CANCEL_AND_FILE_UNDELETE, _filename), e);
                }
                throw new CancelledException();
            }
            else if (_exception != null)
            {
                log.Warn(string.Format("Export of Pool {0} failed", this.Connection.Cache.Pools[0].Name), _exception);
                log.DebugFormat("Progress of the action until exception: {0}", PercentComplete);

                if (_exception is IOException)
                {
                    this.Description = _exception.Message;
                }
                else
                {
                    try
                    {
                        File.Delete(_filename);
                    }
                    catch (Exception e)
                    {
                        log.Warn(string.Format("deleting file {0} failed", _filename), e);
                    }
                    this.Description = Messages.ACTION_EXPORT_DESCRIPTION_FAILED;
                }
                throw new Exception(Description);
            }
            else
            {
                log.InfoFormat("Export of Pool {0} successful", this.Connection.Cache.Pools[0].Name);
                this.Description = Messages.ACTION_EXPORT_DESCRIPTION_SUCCESSFUL;
            }
        }

        private class HostInfo
        {
            public HostInfo(string hostName, string hostAddress, string hostUUID, string hostCpuUsage,
                string hostRole, string hostnetworkUsage, string hostMemUsage, string hostUptime, string srSizeString, string hostDescription)
            {
                _name = hostName;
                _address = hostAddress;
                _uuid = hostUUID;
                _cpuUsage = hostCpuUsage;
                _role = hostRole;
                _networkUsage = hostnetworkUsage;
                _memUsage = hostMemUsage;
                _uptime = hostUptime;
                _description = hostDescription;
                _srSizeString = srSizeString;
            }
            public virtual string Address
            {
                get { return _address; }
            }
            public virtual string Name
            {
                get { return _name; }
            }
            public virtual string UUID
            {
                get { return _uuid; }
            }
            public virtual string CpuUsage
            {
                get { return _cpuUsage; }
            }
            public virtual string Role
            {
                get { return _role; }
            }
            public virtual string NetworkUsage
            {
                get { return _networkUsage; }
            }
            public virtual string MemUsage
            {
                get { return _memUsage; }
            }
            public virtual string Uptime
            {
                get { return _uptime; }
            }
            public virtual string Description
            {
                get { return _description; }
            }
            public virtual string SRSizeString
            {
                get { return _srSizeString; }
            }
            private string _name;
            private string _role;
            private string _address;
            private string _uuid;
            private string _cpuUsage;
            private string _networkUsage;
            private string _memUsage;
            private string _uptime;
            private string _description;
            private string _srSizeString;
        }

        private class SRInfo
        {
            public SRInfo(string name, string SRUuid, string SRType,
                string SRSize, string SRRemark, string SRDescription)
            {
                _name = name;
                _uuid = SRUuid;
                _type = SRType;
                _size = SRSize;
                _remark = SRRemark;
                _description = SRDescription;
            }
            public virtual string Name
            {
                get { return _name; }
            }
            public virtual string UUID
            {
                get { return _uuid; }
            }
            public virtual string Type
            {
                get { return _type; }
            }
            public virtual string Size
            {
                get { return _size; }
            }
            public virtual string Remark
            {
                get { return _remark; }
            }
            public virtual string Description
            {
                get { return _description; }
            }
            private string _name;
            private string _uuid;
            private string _type;
            private string _size;
            private string _remark;
            private string _description;
        }

        private class NetworkInfo
        {
            public NetworkInfo(string name, string networkVlanID, string networkLinkStatus,
                string NetworkMac, string NetworkMtu, string Type, string location)
            {
                _name = name;
                _vlanID = networkVlanID;
                _linkStatus = networkLinkStatus;
                _mac = NetworkMac;
                _mtu = NetworkMtu;
                _networkType = Type;
                _location = location;
            }
            public virtual string Name
            {
                get { return _name; }
            }
            public virtual string VlanID
            {
                get { return _vlanID; }
            }
            public virtual string LinkStatus
            {
                get { return _linkStatus; }
            }
            public virtual string MAC
            {
                get { return _mac; }
            }
            public virtual string MTU
            {
                get { return _mtu; }
            }
            public virtual string NetworkType
            {
                get { return _networkType; }
            }
            public virtual string Location
            {
                get { return _location; }
            }
            private string _name;
            private string _vlanID;
            private string _linkStatus;
            private string _mac;
            private string _mtu;
            private string _networkType;
            private string _location;
        }

        private class VMInfo
        {
            public VMInfo(string Name, string VMuuid, string VMvCpuNum, string VMmemSize, string VMsrInfo,
                string VMnicNum, string VMip, string VMmac, string VMosInfo,
                string VMpowerStatus, string VMuptime, string VMhostInfo, string VMTemplateName, string VMDescription)
            {
                _name = Name;
                _uuid = VMuuid;
                _vCpuNum = VMvCpuNum;
                _memSize = VMmemSize;
                _srInfo = VMsrInfo;
                _nicNum = VMnicNum;
                _ip = VMip;
                _mac = VMmac;
                _osInfo = VMosInfo;
                _powerStatus = VMpowerStatus;
                _uptime = VMuptime;
                _hostInfo = VMhostInfo;
                _templateName = VMTemplateName;
                _description = VMDescription;
            }
            public virtual string Name
            {
                get { return _name; }
            }
            public virtual string UUID
            {
                get { return _uuid; }
            }
            public virtual string VCpuNum
            {
                get { return _vCpuNum; }
            }
            public virtual string MemSize
            {
                get { return _memSize; }
            }
            public virtual string SRInfo
            {
                get { return _srInfo; }
            }
            public virtual string NicNum
            {
                get { return _nicNum; }
            }
            public virtual string IP
            {
                get { return _ip; }
            }
            public virtual string MAC
            {
                get { return _mac; }
            }
            public virtual string OSInfo
            {
                get { return _osInfo; }
            }
            public virtual string PowerStatus
            {
                get { return _powerStatus; }
            }
            public virtual string Uptime
            {
                get { return _uptime; }
            }
            public virtual string HostInfo
            {
                get { return _hostInfo; }
            }
            public virtual string TemplateName
            {
                get { return _templateName; }
            }
            public virtual string Description
            {
                get { return _description; }
            }
            private string _name;
            private string _uuid;
            private string _vCpuNum;
            private string _memSize;
            private string _srInfo;
            private string _nicNum;
            private string _ip;
            private string _mac;
            private string _osInfo;
            private string _powerStatus;
            private string _uptime;
            private string _hostInfo;
            private string _templateName;
            private string _description;
        }

        private class PGPUInfo
        {
            public PGPUInfo(string name, string PGPUuuid, string PGUPHost,
                string BusAddress, string utilization, string memoryUtilization, string Temperature, string PowerStatus)
            {
                _name = name;
                _uuid = PGPUuuid;
                _host = PGUPHost;
                _busAddress = BusAddress;
                _utilization = utilization;
                _MemUtilization = memoryUtilization;
                _temperature = Temperature;
                _powerStatus = PowerStatus;
            }
            public virtual string Name
            {
                get { return _name; }
            }
            public virtual string UUID
            {
                get { return _uuid; }
            }
            public virtual string Host
            {
                get { return _host; }
            }
            public virtual string BusAddress
            {
                get { return _busAddress; }
            }
            public virtual string Utilization
            {
                get { return _utilization; }
            }
            public virtual string MemoryUtilization
            {
                get { return _MemUtilization; }
            }
            public virtual string Temperature
            {
                get { return _temperature; }
            }
            public virtual string PowerStatus
            {
                get { return _powerStatus; }
            }
            private string _name;
            private string _uuid;
            private string _host;
            private string _busAddress;
            private string _utilization;
            private string _MemUtilization;
            private string _temperature;
            private string _powerStatus;
        }

        private class VDIInfo
        {
            public VDIInfo(string name, string VDIUuid, string VDIType,
                string VDISize, string VDIDescription, string VDIResideon)
            {
                _name = name;
                _uuid = VDIUuid;
                _type = VDIType;
                _size = VDISize;
                _resideon = VDIResideon;
                _description = VDIDescription;
            }
            public virtual string Name
            {
                get { return _name; }
            }
            public virtual string UUID
            {
                get { return _uuid; }
            }
            public virtual string Type
            {
                get { return _type; }
            }
            public virtual string Size
            {
                get { return _size; }
            }
            public virtual string Resideon
            {
                get { return _resideon; }
            }
            public virtual string Description
            {
                get { return _description; }
            }
            private string _name;
            private string _uuid;
            private string _type;
            private string _size;
            private string _resideon;
            private string _description;
        }

        private void ComposeParameters(ReportViewer viewer, IXenConnection connection)
        {
            string ParamLabelsStr;
            string ParamValuesStr;

            ParamLabelsStr = "LBL_POOLINFO|";
            ParamValuesStr = Messages.POOL + ":" + connection.Cache.Pools[0].Name + "|";
            ParamLabelsStr += "LBL_POOLUUID|";
            ParamValuesStr += Messages.UUID + ":" + connection.Cache.Pools[0].uuid + "|";
            //Host Infor
            ParamLabelsStr += "LBL_SERVERS|";
            ParamValuesStr += Messages.SERVERS + "|";
            ParamLabelsStr += "LBL_HOSTNAME|";
            ParamValuesStr += Messages.NAME + "|";
            ParamLabelsStr += "LBL_POOLMASTER|";
            ParamValuesStr += Messages.POOL_MASTER + "|";
            ParamLabelsStr += "LBL_ADDRESS|";
            ParamValuesStr += Messages.ADDRESS + "|";
            ParamLabelsStr += "LBL_UUID|";
            ParamValuesStr += Messages.UUID + "|";
            ParamLabelsStr += "LBL_CPUUSAGE|";
            ParamValuesStr += Messages.OVERVIEW_CPU_USAGE + "|";
            ParamLabelsStr += "LBL_NETWORKUSAGE|";
            ParamValuesStr += Messages.OVERVIEW_NETWORK + " " + Messages.OVERVIEW_UNITS + "|";
            ParamLabelsStr += "LBL_MEMORYUSAGE|";
            ParamValuesStr += Messages.OVERVIEW_MEMORY_USAGE + "|";
            ParamLabelsStr += "LBL_UPTIME|";
            ParamValuesStr += Messages.UPTIME + "|";

            //network Info
            ParamLabelsStr += "LBL_NETWORKS|";
            ParamValuesStr += Messages.NETWORKS + "|";
            ParamLabelsStr += "LBL_LINKSTATUS|";
            ParamValuesStr += Messages.LINK_STATUS + "|";
            ParamLabelsStr += "LBL_MAC|";
            ParamValuesStr += Messages.MAC + "|";
            ParamLabelsStr += "LBL_MTU|";
            ParamValuesStr += Messages.MTU + "|";
            ParamLabelsStr += "LBL_VLAN|";
            ParamValuesStr += Messages.NETWORKPANEL_VLAN + "|";

            //storage Info
            ParamLabelsStr += "LBL_STORAGE|";
            ParamValuesStr += Messages.DATATYPE_STORAGE + "|";
            ParamLabelsStr += "LBL_STORAGETYPE|";
            ParamValuesStr += Messages.STORAGE_TYPE + "|";
            ParamLabelsStr += "LBL_STORAGETYPE|";
            ParamValuesStr += Messages.STORAGE_TYPE + "|";
            ParamLabelsStr += "LBL_SIZE|";
            ParamValuesStr += Messages.SIZE + "|";
            ParamLabelsStr += "LBL_LOCATION|";
            ParamValuesStr += Messages.NEWSR_LOCATION + "|";
            ParamLabelsStr += "LBL_DESCRIPTION|";
            ParamValuesStr += Messages.DESCRIPTION + "|";

            //PGPU Info
            ParamLabelsStr += "LBL_GPU|";
            ParamValuesStr += Messages.GPU + "|";
            ParamLabelsStr += "LBL_BUSADDRESS|";
            ParamValuesStr += Messages.BUS_PATH + "|";
            ParamLabelsStr += "LBL_POWERUSAGE|";
            ParamValuesStr += Messages.POWER_USAGE + "|";
            ParamLabelsStr += "LBL_TEMPERATURE|";
            ParamValuesStr += Messages.TEMPERATURE + "|";
            ParamLabelsStr += "LBL_UTILIZATION|";
            ParamValuesStr += Messages.UTILIZATION + "|";

            //VDI Info
            ParamLabelsStr += "LBL_VDI|";
            ParamValuesStr += Messages.VDI + "|";

            //VM Info
            ParamLabelsStr += "LBL_VMS|";
            ParamValuesStr += Messages.VMS + "|";
            ParamLabelsStr += "LBL_POWERSTATE|";
            ParamValuesStr += Messages.POWER_STATE + "|";
            ParamLabelsStr += "LBL_OPERATINGSYSTEM|";
            ParamValuesStr += Messages.OPERATING_SYSTEM + "|";
            ParamLabelsStr += "LBL_NIC|";
            ParamValuesStr += Messages.NIC + "|";
            ParamLabelsStr += "LBL_SERVER|";
            ParamValuesStr += Messages.SERVER + "|";
            ParamLabelsStr += "LBL_TEMPLATE|";
            ParamValuesStr += Messages.TEMPLATE + "|";
            ParamLabelsStr += "LBL_RUNNING_ON|";
            ParamValuesStr += Messages.RUNNING_ON;
            

            ReportParameter ParamLabels = new ReportParameter("ParamLabels", ParamLabelsStr);
            ReportParameter ParamValues = new ReportParameter("ParamValues", ParamValuesStr);
            viewer.LocalReport.SetParameters(new ReportParameter[] { ParamLabels, ParamValues });
        }

        private string SRSizeString(XenAPI.SR sr)
        {
            int percent = 0;
            double ratio = 0;
            if (sr.physical_size > 0)
            {
                ratio = sr.physical_utilisation / (double)sr.physical_size;
                percent = (int)(100.0 * ratio);
            }
            string percentString = string.Format(Messages.DISK_PERCENT_USED, percent.ToString(), Util.DiskSizeString(sr.physical_utilisation));
            return string.Format(Messages.SR_SIZE_USED, percentString, Util.DiskSizeString(sr.physical_size), Util.DiskSizeString(sr.virtual_allocation));
        }

        private void ComposeHostData()
        {
            m_Hosts = new List<HostInfo>();
            var hosts = new List<XenAPI.Host>(Connection.Cache.Hosts);
            hosts.Sort();
            
            foreach (XenAPI.Host host in hosts)
            {
                if (Cancelling)
                    throw new CancelledException();
                string srSizeString = "";
                var PBDs = host.Connection.ResolveAll(host.PBDs);
                foreach (XenAPI.PBD pbd in PBDs)
                {
                    SR sr = pbd.Connection.Resolve(pbd.SR);
                    if(sr.IsLocalSR && sr.type.ToLower() == "lvm")
                    {
                        srSizeString += SRSizeString(sr) + ";";
                    }
                }
                if (srSizeString.Length == 0)
                    srSizeString = Messages.HYPHEN;
                string cpu_usage = PropertyAccessorHelper.hostCpuUsageStringByMetric(host, MetricUpdater);
                string usage = PropertyAccessorHelper.hostMemoryUsagePercentageStringByMetric(host, MetricUpdater);
                string network_usage = PropertyAccessorHelper.hostNetworkUsageStringByMetric(host, MetricUpdater);

                
                HostInfo buf = new HostInfo(host.name_label, host.address, host.uuid, cpu_usage,
                    host.IsMaster() ? Messages.YES : Messages.NO, network_usage, usage,
                    Convert.ToString(host.Uptime), srSizeString, host.Description);
                m_Hosts.Add(buf);
                PercentComplete = Convert.ToInt32((++itemIndex) * baseIndex / itemCount);
            }
        }

        private void ComposeNetworkData()
        {
            m_Networks = new List<NetworkInfo>();
            var networks = new List<XenAPI.Network>(Connection.Cache.Networks);
            networks.Sort();
            foreach (XenAPI.Network network in networks)
            {
                if (Cancelling)
                    throw new CancelledException();

                // CA-218956 - Expose HIMN when showing hidden objects
                if (network.IsGuestInstallerNetwork && !XenAdmin.Properties.Settings.Default.ShowHiddenVMs)
                {
                    PercentComplete = Convert.ToInt32((++itemIndex) * baseIndex / itemCount);
                    continue;
                }

                List<PIF> pifs = network.Connection.ResolveAll(network.PIFs);
                string type;
                if (Cancelling)
                    throw new CancelledException();
                if (network.IsBond)
                    type = Messages.BOND;
                else if (network.IsVLAN)
                    type = Messages.EXTERNAL_NETWORK;
                else if (pifs.Count != 0 && pifs[0].IsPhysical)
                    type = Messages.BUILTIN_NETWORK;
                else if (pifs.Count != 0 && pifs[0].IsTunnelAccessPIF)
                    type = Messages.CHIN;
                else if (pifs.Count == 0)
                    type = Messages.SINGLE_SERVER_PRIVATE_NETWORK;
                else
                    type = Messages.HYPHEN;

                string location = "";
                foreach (XenAPI.Host host in Connection.Cache.Hosts)
                {
                    if (host.CanSeeNetwork(network))
                        location += host.name_label + ":" + host.uuid + ";";
                }
                if (location.Length == 0)
                    location = Messages.HYPHEN;

                NetworkInfo buf;
                if (pifs.Count != 0)
                    buf = new NetworkInfo(network.Name, Helpers.VlanString(pifs[0]), network.LinkStatusString, pifs[0].MAC, network.MTU.ToString(), type, location);
                else
                    buf = new NetworkInfo(network.Name, Messages.HYPHEN, network.LinkStatusString, Messages.HYPHEN, network.MTU.ToString(), type, location);
                
                m_Networks.Add(buf);
                
                PercentComplete = Convert.ToInt32((++itemIndex) * baseIndex / itemCount);
            }
        }
        private void ComposeSRData()
        {
            m_SRs = new List<SRInfo>();
            var SRs = new List<XenAPI.SR>(Connection.Cache.SRs);
            SRs.Sort();
            foreach (XenAPI.SR sr in SRs)
            {
                if (Cancelling)
                    throw new CancelledException();
                
                string srSizeString = SRSizeString(sr);
                
                string locationStr = "";
                bool haveLocation = false;
                bool haveServerPath = false;
                bool haveDevice = false;
                bool haveTargetIQN = false;
                bool haveSCSIid = false;
                foreach (XenRef<PBD> pbdRef in sr.PBDs)
                {
                    PBD pbd = sr.Connection.Resolve(pbdRef);

                    if (!haveLocation && pbd.device_config.ContainsKey("location"))
                    {
                       haveLocation = true;
                       locationStr += "location:" + pbd.device_config["location"] + ";";
                    }
                    if (!haveDevice && pbd.device_config.ContainsKey("device"))
                    {
                        haveDevice = true;
                        locationStr += "device:" + pbd.device_config["device"] + ";";
                    }
                    if (!haveSCSIid && pbd.device_config.ContainsKey("SCSIid"))
                    {
                        haveSCSIid = true;
                        locationStr += "SCSIid:" + pbd.device_config["SCSIid"] + ";";
                    }
                    if (!haveTargetIQN && pbd.device_config.ContainsKey("targetIQN"))
                    {
                        haveTargetIQN = true;
                        locationStr += "targetIQN:" + pbd.device_config["targetIQN"] + ";";
                    }
                    if (!haveServerPath && pbd.device_config.ContainsKey("server"))
                    {
                        haveServerPath = true;
                        locationStr += "server:" + pbd.device_config["server"];
                        if(pbd.device_config.ContainsKey("serverpath"))
                            locationStr += pbd.device_config["serverpath"] + ";";
                        else
                            locationStr += ";";
                    }
                }
                if (locationStr.Length == 0)
                    locationStr = Messages.HYPHEN;

                SRInfo buf = new SRInfo(sr.Name, sr.uuid, sr.type, srSizeString, locationStr, sr.Description);
                m_SRs.Add(buf);
                PercentComplete = Convert.ToInt32((++itemIndex) * baseIndex / itemCount);
            }
        }
        private void ComposeVMData()
        {
            m_VMs = new List<VMInfo>();
            var VMs = new List<XenAPI.VM>(Connection.Cache.VMs);
            VMs.Sort();
            foreach (XenAPI.VM vm in VMs)
            {
                string OSinfo = vm.GetOSName();
                string srInfo = "";
                string MacInfo = "";
                string running_on = Messages.HYPHEN;

                if (Cancelling)
                    throw new CancelledException();

                if (!vm.is_a_real_vm)
                {
                    PercentComplete = Convert.ToInt32((++itemIndex) * baseIndex / itemCount);
                    continue;
                }

                ComparableList<ComparableAddress> addresses = new ComparableList<ComparableAddress>();
                if (vm.guest_metrics != null && !string.IsNullOrEmpty(vm.guest_metrics.opaque_ref) && !(vm.guest_metrics.opaque_ref.ToLower().Contains("null")))
                {
                    VM_guest_metrics metrics = vm.Connection.Resolve(vm.guest_metrics);

                    List<VIF> vifs = vm.Connection.ResolveAll(vm.VIFs);
                    foreach (VIF vif in vifs)
                    {
                        MacInfo += vif.MAC + " ";
                        foreach (var network in metrics.networks.Where(n => n.Key.StartsWith(String.Format("{0}/ip", vif.device))))
                        {
                            ComparableAddress ipAddress;
                            if (!ComparableAddress.TryParse(network.Value, false, true, out ipAddress))
                                continue;

                            addresses.Add(ipAddress);
                        }
                    }
                }
                if (MacInfo.Length == 0)
                    MacInfo = Messages.HYPHEN;

                foreach (XenRef<VBD> vbdRef in vm.VBDs)
                {
                    var vbd = vm.Connection.Resolve(vbdRef);
                    if (vbd != null && !vbd.IsCDROM && !vbd.IsFloppyDrive && vbd.bootable)
                    {
                        VDI vdi = vm.Connection.Resolve(vbd.VDI);
                        srInfo += vdi.name_label + ":" + vdi.SizeText + ";";
                    }
                }
                if (srInfo.Length == 0)
                    srInfo = Messages.HYPHEN;
                
                if (vm.resident_on != null && !string.IsNullOrEmpty(vm.resident_on.opaque_ref) && !(vm.resident_on.opaque_ref.ToLower().Contains("null")))
                {
                    running_on = vm.Connection.Resolve(vm.resident_on).Name;
                }

                string default_template_name = Messages.HYPHEN;
                if(vm.other_config.ContainsKey("base_template_name"))
                    default_template_name = vm.other_config["base_template_name"];

                VMInfo buf = new VMInfo(vm.Name, vm.uuid, PropertyAccessorHelper.vmCpuUsageStringByMetric(vm, MetricUpdater),
                    PropertyAccessorHelper.vmMemoryUsagePercentageStringByMetric(vm, MetricUpdater), srInfo, Convert.ToString(vm.VIFs.Count),
                    Convert.ToString(addresses), MacInfo, OSinfo, Convert.ToString(vm.power_state),
                    Convert.ToString(vm.RunningTime), running_on, default_template_name, vm.Description);
                
                m_VMs.Add(buf);

                PercentComplete = Convert.ToInt32((++itemIndex) * baseIndex / itemCount);
            }
        }

        

        private void ComposeGPUData()
        {
            m_PGPUs = new List<PGPUInfo>();

            var PGPUs = new List<XenAPI.PGPU>(Connection.Cache.PGPUs);
            PGPUs.Sort();

            foreach (XenAPI.PGPU pGpu in PGPUs)
            {
                Host host= Connection.Resolve(pGpu.host);
                if (host == null)
                    continue;
                PCI pci = Connection.Resolve(pGpu.PCI);

                string temperature = PropertyAccessorHelper.PGPUTemperatureString(pGpu, MetricUpdater);
                string powerStatus = PropertyAccessorHelper.PGPUPowerUsageString(pGpu, MetricUpdater);
                string utilisation = PropertyAccessorHelper.PGPUUtilisationString(pGpu, MetricUpdater);
                string memInfo = PropertyAccessorHelper.PGPUMemoryUsageString(pGpu, MetricUpdater);
                PGPUInfo buf = new PGPUInfo(pGpu.Name, pGpu.uuid, host.Name, pci.pci_id, utilisation,
                    memInfo, temperature, powerStatus);
                
                m_PGPUs.Add(buf);
            }
        }

        private void ComposeVDIData()
        {
            m_VDIs = new List<VDIInfo>();

            var VDIs = new List<XenAPI.VDI>(Connection.Cache.VDIs);
            VDIs.Sort();

            foreach (XenAPI.VDI vdi in VDIs)
            {
                XenAPI.SR sr = Connection.Resolve(vdi.SR);
                VDIInfo buf = new VDIInfo(vdi.Name, vdi.uuid, Convert.ToString(vdi.type), vdi.SizeText, vdi.Description, sr.Name);

                m_VDIs.Add(buf);
            }
        }

        public override void RecomputeCanCancel()
        {
            CanCancel = true;
        }

        private void export2XLS()
        {
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            FileStream fs = null;
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = "resource_report.rdlc";
            ReportDataSource HostDataSource = new ReportDataSource("Report_HostInfo", m_Hosts);
            ReportDataSource NetworkDataSource = new ReportDataSource("Report_NetworkInfo", m_Networks);
            ReportDataSource SRDataSource = new ReportDataSource("Report_SRInfo", m_SRs);
            ReportDataSource VMDataSource = new ReportDataSource("Report_VMInfo", m_VMs);
            ReportDataSource PGPUDataSource = new ReportDataSource("Report_PGPUInfo", m_PGPUs);
            ReportDataSource VDIDataSource = new ReportDataSource("Report_VDIInfo", m_VDIs);
            viewer.LocalReport.DataSources.Add(HostDataSource);
            viewer.LocalReport.DataSources.Add(NetworkDataSource);
            viewer.LocalReport.DataSources.Add(SRDataSource);
            viewer.LocalReport.DataSources.Add(VMDataSource);
            viewer.LocalReport.DataSources.Add(PGPUDataSource);
            viewer.LocalReport.DataSources.Add(VDIDataSource);

            ComposeParameters(viewer, Connection);
            byte[] bytes = viewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            fs = new FileStream(_filename, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            PercentComplete = 100;
            if (fs != null)
                fs.Close();
            try
            {
                Process xlProcess = Process.Start(_filename);
            }
            catch (Exception ex)
            {
                log.Debug(ex, ex);
            }
        }

        private void ComposeCSVRow(ref FileStream fs, ref List<string> items)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            byte[] info;
            foreach (string value in items)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(',');
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            info = new UTF8Encoding(true).GetBytes(builder.ToString() + "\n");
            fs.Write(info, 0, info.Length);
            items.Clear();
        }

        private void HostInfoCSVMaker(ref FileStream fs)
        {
            List<string> items = new List<string>();
            items.Add("\n");
            ComposeCSVRow(ref fs, ref items);

            items.Add(Messages.SERVER);
            ComposeCSVRow(ref fs, ref items);

            items.Add(Messages.NAME);
            items.Add(Messages.UUID);
            items.Add(Messages.POOL_MASTER);
            items.Add(Messages.ADDRESS);
            items.Add(Messages.OVERVIEW_CPU_USAGE);
            items.Add(Messages.OVERVIEW_MEMORY_USAGE);
            items.Add(Messages.OVERVIEW_NETWORK + Messages.OVERVIEW_UNITS);
            items.Add(Messages.STORAGE_DISK);
            items.Add(Messages.UPTIME);
            items.Add(Messages.DESCRIPTION);
            ComposeCSVRow(ref fs, ref items);

            foreach (HostInfo host in m_Hosts)
            {
                items.Add(host.Name);
                items.Add(host.UUID);
                items.Add(host.Role);
                items.Add(host.Address);
                items.Add(host.CpuUsage);
                items.Add(host.MemUsage);
                items.Add(host.NetworkUsage);
                items.Add(host.SRSizeString);
                items.Add(host.Uptime);
                items.Add(host.Description);
                ComposeCSVRow(ref fs, ref items);
            }
        }

        private void NetworkInfoCSVMaker(ref FileStream fs)
        {
            List<string> items = new List<string>();
            items.Add("\n");
            ComposeCSVRow(ref fs, ref items);
            items.Add(Messages.NETWORKS);
            ComposeCSVRow(ref fs, ref items);

            items.Add(Messages.NAME);
            items.Add(Messages.LINK_STATUS);
            items.Add(Messages.MAC);
            items.Add(Messages.MTU);
            items.Add(Messages.NETWORKPANEL_VLAN);
            items.Add(Messages.NEWSR_LOCATION);
            ComposeCSVRow(ref fs, ref items);

            foreach (NetworkInfo network in m_Networks)
            {
                items.Add(network.Name);
                items.Add(network.LinkStatus);
                items.Add(network.MAC);
                items.Add(network.MTU);
                items.Add(network.VlanID);
                items.Add(network.Location);
                ComposeCSVRow(ref fs, ref items);
            }
        }

        private void SRInfoCSVMaker(ref FileStream fs)
        {
            List<string> items = new List<string>();
            items.Add("\n");
            ComposeCSVRow(ref fs, ref items);
            items.Add(Messages.DATATYPE_STORAGE);
            ComposeCSVRow(ref fs, ref items);

            items.Add(Messages.NAME);
            items.Add(Messages.UUID);
            items.Add(Messages.STORAGE_TYPE);
            items.Add(Messages.SIZE);
            items.Add(Messages.NEWSR_LOCATION);
            items.Add(Messages.DESCRIPTION);
            ComposeCSVRow(ref fs, ref items);

            foreach (SRInfo sr in m_SRs)
            {
                items.Add(sr.Name);
                items.Add(sr.UUID);
                items.Add(sr.Type);
                items.Add(sr.Size);
                items.Add(sr.Remark);
                items.Add(sr.Description);
                ComposeCSVRow(ref fs, ref items);
            }
        }

        private void PGPUInfoCSVMaker(ref FileStream fs)
        {
            List<string> items = new List<string>();
            items.Add("\n");
            ComposeCSVRow(ref fs, ref items);
            items.Add(Messages.GPU);
            ComposeCSVRow(ref fs, ref items);

            items.Add(Messages.NAME);
            items.Add(Messages.UUID);
            items.Add(Messages.BUS_PATH);
            items.Add(Messages.SERVER);
            items.Add(Messages.OVERVIEW_MEMORY_USAGE);
            items.Add(Messages.POWER_USAGE);
            items.Add(Messages.TEMPERATURE);
            items.Add(Messages.UTILIZATION);
            ComposeCSVRow(ref fs, ref items);

            foreach (PGPUInfo pGpu in m_PGPUs)
            {
                items.Add(pGpu.Name);
                items.Add(pGpu.UUID);
                items.Add(pGpu.BusAddress);
                items.Add(pGpu.Host);
                items.Add(pGpu.MemoryUtilization);
                items.Add(pGpu.PowerStatus);
                items.Add(pGpu.Temperature);
                items.Add(pGpu.Utilization);
                ComposeCSVRow(ref fs, ref items);
            }
        }

        private void VMInfoCSVMaker(ref FileStream fs)
        {
            List<string> items = new List<string>();
            items.Add("\n");
            ComposeCSVRow(ref fs, ref items);
            items.Add(Messages.VMS);
            ComposeCSVRow(ref fs, ref items);

            items.Add(Messages.NAME);
            items.Add(Messages.POWER_STATE);
            items.Add(Messages.UUID);
            items.Add(Messages.RUNNING_ON);
            items.Add(Messages.ADDRESS);
            items.Add(Messages.MAC);
            items.Add(Messages.NIC);
            items.Add(Messages.OVERVIEW_MEMORY_USAGE);
            items.Add(Messages.OPERATING_SYSTEM);
            items.Add(Messages.STORAGE_DISK);
            items.Add(Messages.TEMPLATE);
            items.Add(Messages.OVERVIEW_CPU_USAGE);
            items.Add(Messages.UPTIME);
            items.Add(Messages.DESCRIPTION);
            ComposeCSVRow(ref fs, ref items);

            foreach (VMInfo vm in m_VMs)
            {
                items.Add(vm.Name);
                items.Add(vm.PowerStatus);
                items.Add(vm.UUID);
                items.Add(vm.HostInfo);
                items.Add(vm.IP);
                items.Add(vm.MAC);
                items.Add(vm.NicNum);
                items.Add(vm.MemSize);
                items.Add(vm.OSInfo);
                items.Add(vm.SRInfo);
                items.Add(vm.TemplateName);
                items.Add(vm.VCpuNum);
                items.Add(vm.Uptime);
                items.Add(vm.Description);
                ComposeCSVRow(ref fs, ref items);
            }
        }

        private void VDIInfoCSVMaker(ref FileStream fs)
        {
            List<string> items = new List<string>();
            items.Add("\n");
            ComposeCSVRow(ref fs, ref items);
            items.Add(Messages.VDI);
            ComposeCSVRow(ref fs, ref items);

            items.Add(Messages.NAME);
            items.Add(Messages.UUID);
            items.Add(Messages.STORAGE_TYPE);
            items.Add(Messages.SIZE);
            items.Add(Messages.NEWSR_LOCATION);
            items.Add(Messages.DESCRIPTION);
            ComposeCSVRow(ref fs, ref items);

            foreach (VDIInfo vdi in m_VDIs)
            {
                items.Add(vdi.Name);
                items.Add(vdi.UUID);
                items.Add(vdi.Type);
                items.Add(vdi.Size);
                items.Add(vdi.Resideon);
                items.Add(vdi.Description);
                ComposeCSVRow(ref fs, ref items);
            }
        }

        private void export2CSV()
        {
            FileStream fs = null;
            List<string> items = new List<string>();
            fs = new FileStream(_filename, FileMode.Create);

            //pool information part
            items.Add(Messages.POOL + ":" + Connection.Cache.Pools[0].Name);
            ComposeCSVRow(ref fs, ref items);
            items.Add(Messages.UUID + ":" + Connection.Cache.Pools[0].uuid);
            ComposeCSVRow(ref fs, ref items);

            //TABLES
            HostInfoCSVMaker(ref fs);
            NetworkInfoCSVMaker(ref fs);
            SRInfoCSVMaker(ref fs);
            VDIInfoCSVMaker(ref fs);
            VMInfoCSVMaker(ref fs);
            PGPUInfoCSVMaker(ref fs);
            
            PercentComplete = 100;
            if (fs != null)
                fs.Close();
            try
            {
                Process xlProcess = Process.Start(_filename);
            }
            catch (Exception ex)
            {
                log.Debug(ex, ex);
            }
        }

        private void DoExport()
        {
            CanCancel = true;
            PercentComplete = 0;
            
            ComposeHostData();
            ComposeNetworkData();
            ComposeSRData();
            ComposeVMData();
            ComposeGPUData();
            ComposeVDIData();

            if (_fileType == Convert.ToInt32(FILE_TYPE_INDEX.XLS))
            {
                export2XLS();
            }
            else
            {
                export2CSV();
            }
        }
    }
}
