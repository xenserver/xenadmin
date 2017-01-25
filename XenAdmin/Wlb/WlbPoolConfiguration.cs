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
using System.Text;

using XenAPI;

namespace XenAdmin.Wlb
{

#region Public Enums

    public enum WlbPoolPerformanceMode : int
    {
        MaximizePerformance = 0,
        MaximizeDensity = 1
    }

    public enum WlbPoolAutoBalanceSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum WlbPoolAutoBalanceAggressiveness
    {
        Low,
        Medium,
        High
    }

    public enum WlbAuditTrailLogGranularity
    {
        Minimum,
        Medium,
        Maximum
    }

#endregion

    public class WlbPoolConfiguration : WlbConfigurationBase
    {

#region Private Constants

        private const int WlbVersion_Major_MNR = 2;
        private const int WlbVersion_Minor_MNR = 0;
        private const int WlbVersion_Major_Cowley = 2;
        private const int WlbVersion_Minor_Cowley = 1;
        private const int WlbVersion_Major_Boston = 6;
        private const int WlbVersion_Minor_Boston = 0;
        private const int WlbVersion_Major_Tampa = 6;
        private const int WlbVersion_Minor_Tampa = 1;
        private const int WlbVersion_Major_Creedence = 6;
        private const int WlbVersion_Minor_Creedence = 5;
        private const int WlbVersion_Major_Dundee = 7;
        private const int WlbVersion_Minor_Dundee = 0;

#endregion

#region Private Members

        // these are the factors used to calculate the indirectly set values
        // initial values are defaults in case we don't get them from the wlb server
        private double _cpuHighThresholdFactor = .85;
        private double _cpuMediumThresholdFactor = .50;
        private double _cpuLowThresholdFactor = .15;
        private double _memoryHighThresholdFactor = 1.25;
        private double _memoryMediumThresholdFactor = 10.00;
        private double _memoryLowThresholdFactor = 20.00;
        private double _networkReadHighThresholdFactor = .85;
        private double _networkReadMediumThresholdFactor = .50;
        private double _networkReadLowThresholdFactor = .15;
        private double _networkWriteHighThresholdFactor = .85;
        private double _networkWriteMediumThresholdFactor = .50;
        private double _networkWriteLowThresholdFactor = .15;
        private double _diskReadHighThresholdFactor = .85;
        private double _diskReadMediumThresholdFactor = .50;
        private double _diskReadLowThresholdFactor = .15;
        private double _diskWriteHighThresholdFactor = .85;
        private double _diskWriteMediumThresholdFactor = .50;
        private double _diskWriteLowThresholdFactor = .15;

        private double _cpuMediumWeightFactor = .60;
        private double _cpuLowWeightFactor = .30;
        private double _memoryMediumWeightFactor = .60;
        private double _memoryLowWeightFactor = .30;
        private double _networkReadMediumWeightFactor = .60;
        private double _networkReadLowWeightFactor = .30;
        private double _networkWriteMediumWeightFactor = .60;
        private double _networkWriteLowWeightFactor = .30;
        private double _diskReadMediumWeightFactor = .60;
        private double _diskReadLowWeightFactor = .30;
        private double _diskWriteMediumWeightFactor = .60;
        private double _diskWriteLowWeightFactor = .30;

        private double _metricGroomingPeriod = 0; //0 indicates grroming disabled (SQLExpress)

        private WlbHostConfigurations _hostConfigurations;
        private WlbScheduledTasks _scheduledTasks;
#endregion

#region Constructor
        public WlbPoolConfiguration(Dictionary<string, string> configuration)
        {
            if (null != configuration)
            {
                base.Configuration = configuration;
                PopulateKeyValues();
                _hostConfigurations = new WlbHostConfigurations(base.Configuration);
                _scheduledTasks = new WlbScheduledTasks(base.Configuration);
            }
            else
            {
                throw new Exception("WLBPoolConfiguration is null");
            }
        }

#endregion

#region Private Methods

        private void PopulateKeyValues()
        {
            _cpuHighThresholdFactor = GetConfigValueDouble("CpuHighThresholdFactor", _cpuHighThresholdFactor);
            _cpuMediumThresholdFactor = GetConfigValueDouble("CpuMediumThresholdFactor", _cpuMediumThresholdFactor);
            _cpuLowThresholdFactor = GetConfigValueDouble("CpuLowThresholdFactor", _cpuLowThresholdFactor);
            _memoryHighThresholdFactor = GetConfigValueDouble("MemoryHighThresholdFactor", _memoryHighThresholdFactor);
            _memoryMediumThresholdFactor = GetConfigValueDouble("MemoryMediumThresholdFactor", _memoryMediumThresholdFactor);
            _memoryLowThresholdFactor = GetConfigValueDouble("MemoryLowThresholdFactor", _memoryLowThresholdFactor);
            _networkReadHighThresholdFactor = GetConfigValueDouble("NetworkReadHighThresholdFactor", _networkReadHighThresholdFactor);
            _networkReadMediumThresholdFactor = GetConfigValueDouble("NetworkReadMediumThresholdFactor", _networkReadMediumThresholdFactor);
            _networkReadLowThresholdFactor = GetConfigValueDouble("NetworkReadLowThresholdFactor", _networkReadLowThresholdFactor);
            _networkWriteHighThresholdFactor = GetConfigValueDouble("networkWriteHighThresholdFactor", _networkWriteHighThresholdFactor);
            _networkWriteMediumThresholdFactor = GetConfigValueDouble("networkWriteMediumThresholdFactor", _networkWriteMediumThresholdFactor);
            _networkWriteLowThresholdFactor = GetConfigValueDouble("NetworkWriteLowThresholdFactor", _networkWriteLowThresholdFactor);
            _diskReadHighThresholdFactor = GetConfigValueDouble("DiskReadHighThresholdFactor", _diskReadHighThresholdFactor);
            _diskReadMediumThresholdFactor = GetConfigValueDouble("DiskReadMediumThresholdFactor", _diskReadMediumThresholdFactor);
            _diskReadLowThresholdFactor = GetConfigValueDouble("DiskReadLowThresholdFactor", _diskReadLowThresholdFactor);
            _diskWriteHighThresholdFactor = GetConfigValueDouble("DiskWriteHighThresholdFactor", _diskWriteHighThresholdFactor);
            _diskWriteMediumThresholdFactor = GetConfigValueDouble("DiskWriteMediumThresholdFactor", _diskWriteMediumThresholdFactor);
            _diskWriteLowThresholdFactor = GetConfigValueDouble("DiskWriteLowThresholdFactor", _diskWriteLowThresholdFactor);

            _cpuMediumWeightFactor = GetConfigValueDouble("CpuMediumWeightFactor", _cpuMediumWeightFactor);
            _cpuLowWeightFactor = GetConfigValueDouble("CpuLowWeightFactor", _cpuLowWeightFactor);
            _memoryMediumWeightFactor = GetConfigValueDouble("MemoryMediumWeightFactor", _memoryMediumWeightFactor);
            _memoryLowWeightFactor = GetConfigValueDouble("MemoryLowWeightFactor", _memoryLowWeightFactor);
            _networkReadMediumWeightFactor = GetConfigValueDouble("NetworkReadMediumWeightFactor", _networkReadMediumWeightFactor);
            _networkReadLowWeightFactor = GetConfigValueDouble("NetworkReadLowWeightFactor", _networkReadLowWeightFactor);
            _networkWriteMediumWeightFactor = GetConfigValueDouble("NetworkWriteMediumWeightFactor", _networkWriteMediumWeightFactor);
            _networkWriteLowWeightFactor = GetConfigValueDouble("NetworkWriteLowWeightFactor", _networkWriteLowWeightFactor);
            _diskReadMediumWeightFactor = GetConfigValueDouble("DiskReadMediumWeightFactor", _diskReadMediumWeightFactor);
            _diskReadLowWeightFactor = GetConfigValueDouble("DiskReadLowWeightFactor", _diskReadLowWeightFactor);
            _diskWriteMediumWeightFactor = GetConfigValueDouble("diskWriteMediumWeightFactor", _diskWriteMediumWeightFactor);
            _diskWriteLowWeightFactor = GetConfigValueDouble("DiskWriteLowWeightFactor", _diskWriteLowWeightFactor);
          }
#endregion

#region Properties

        public WlbPoolPerformanceMode PerformanceMode
        {
            get { return (WlbPoolPerformanceMode)GetConfigValueDouble("OptimizationMode"); }
            set
            {
                SetConfigValueDouble("OptimizationMode", (double)value);
            }
        }

        public bool OvercommitCPUs
        {
            get
            {
                if (this.PerformanceMode == WlbPoolPerformanceMode.MaximizeDensity)
                {
                    return GetConfigValueBool("OverCommitCpuInDensityMode");
                }
                else
                {
                    return GetConfigValueBool("OverCommitCpuInPerfMode");
                }
            }
            set
            {
                if (this.PerformanceMode == WlbPoolPerformanceMode.MaximizeDensity)
                {
                    SetConfigValueBool("OverCommitCpuInDensityMode", value);
                }
                else
                {
                    SetConfigValueBool("OverCommitCpuInPerfMode", value);
                }
            }
        }

        public int HostCpuThresholdCritical
        {
            get { return GetConfigValuePercent("HostCpuThresholdCritical"); }
            set 
            {
                SetConfigValuePercent("HostCpuThresholdCritical", value);
                SetConfigValuePercent("HostCpuThresholdHigh", (int)(value * _cpuHighThresholdFactor));
                SetConfigValuePercent("HostCpuThresholdMedium", (int)(value * _cpuMediumThresholdFactor));
                SetConfigValuePercent("HostCpuThresholdLow", (int)(value * _cpuLowThresholdFactor));
            }
        }

        public double HostMemoryThresholdCritical
        {
            get { return GetConfigValueDouble("HostMemoryThresholdCritical"); }
            set
            {
                SetConfigValueDouble("HostMemoryThresholdCritical", value);
                SetConfigValueDouble("HostMemoryThresholdHigh", value * _memoryHighThresholdFactor);
                SetConfigValueDouble("HostMemoryThresholdMedium", value * _memoryMediumThresholdFactor);
                SetConfigValueDouble("HostMemoryThresholdLow", value * _memoryLowThresholdFactor);
            }
        }

        public double HostNetworkReadThresholdCritical
        {
            get { return GetConfigValueDouble("HostPifReadThresholdCritical"); }
            set
            {
                SetConfigValueDouble("HostPifReadThresholdCritical", value);
                SetConfigValueDouble("HostPifReadThresholdHigh", value * _networkReadHighThresholdFactor);
                SetConfigValueDouble("HostPifReadThresholdMedium", value * _networkReadMediumThresholdFactor);
                SetConfigValueDouble("HostPifReadThresholdLow", value * _networkReadLowThresholdFactor);
            }
        }

        public double HostNetworkWriteThresholdCritical
        {
            get { return GetConfigValueDouble("HostPifWriteThresholdCritical"); }
            set
            {
                SetConfigValueDouble("HostPifWriteThresholdCritical", value);
                SetConfigValueDouble("HostPifWriteThresholdHigh", value * _networkWriteHighThresholdFactor);
                SetConfigValueDouble("HostPifWriteThresholdMedium", value * _networkWriteMediumThresholdFactor);
                SetConfigValueDouble("HostPifWriteThresholdLow", value * _networkWriteLowThresholdFactor);
            }
        }

        public double HostDiskReadThresholdCritical
        {
            get { return GetConfigValueDouble("HostPbdReadThresholdCritical"); }
            set
            { 
                SetConfigValueDouble("HostPbdReadThresholdCritical", value);
                SetConfigValueDouble("HostPbdReadThresholdHigh", value * _diskReadHighThresholdFactor);
                SetConfigValueDouble("HostPbdReadThresholdMedium", value * _diskReadMediumThresholdFactor);
                SetConfigValueDouble("HostPbdReadThresholdLow", value * _diskReadLowThresholdFactor);
            }
        }

        public double HostDiskWriteThresholdCritical
        {
            get { return GetConfigValueDouble("HostPbdWriteThresholdCritical"); }
            set 
            {
                SetConfigValueDouble("HostPbdWriteThresholdCritical", value);
                SetConfigValueDouble("HostPbdWriteThresholdHigh", value * _diskWriteHighThresholdFactor);
                SetConfigValueDouble("HostPbdWriteThresholdMedium", value * _diskWriteMediumThresholdFactor);
                SetConfigValueDouble("HostPbdWriteThresholdLow", value * _diskWriteLowThresholdFactor);
            }
        }

        public int VmCpuUtilizationWeightHigh
        {
            get { return GetConfigValuePercent("VmCpuUtilizationWeightHigh"); }
            set 
            {
                SetConfigValuePercent("VmCpuUtilizationWeightHigh", value);
                SetConfigValuePercent("VmCpuUtilizationWeightMedium", (int)(value * _cpuMediumWeightFactor));
                SetConfigValuePercent("VmCpuUtilizationWeightLow", (int)(value * _cpuLowWeightFactor));
            }
        }

        public int VmMemoryWeightHigh
        {
            get { return GetConfigValuePercent("VmMemoryWeightHigh"); }
            set
            {
                SetConfigValuePercent("VmMemoryWeightHigh", value);
                SetConfigValuePercent("VmMemoryWeightMedium", (int)(value * _memoryMediumWeightFactor));
                SetConfigValuePercent("VmMemoryWeightLow", (int)(value * _memoryLowWeightFactor));
            }
        }

        public int VmDiskReadWeightHigh
        {
            get { return GetConfigValuePercent("VmDiskReadWeightHigh"); }
            set
            {
                SetConfigValuePercent("VmDiskReadWeightHigh", value);
                SetConfigValuePercent("VmDiskReadWeightMedium", (int)(value * _diskReadMediumWeightFactor));
                SetConfigValuePercent("VmDiskReadWeightLow", (int)(value * _diskWriteLowWeightFactor));
            }
        }

        public int VmDiskWriteWeightHigh
        {
            get { return GetConfigValuePercent("VmDiskWriteWeightHigh"); }
            set
            {
                SetConfigValuePercent("VmDiskWriteWeightHigh", value);
                SetConfigValuePercent("VmDiskWriteWeightMedium", (int)(value * _diskWriteMediumWeightFactor));
                SetConfigValuePercent("VmDiskWriteWeightLow", (int)(value * _diskWriteLowWeightFactor));
            }
        }

        public int VmNetworkReadWeightHigh
        {
            get { return GetConfigValuePercent("VmNetworkReadWeightHigh"); }
            set
            {
                SetConfigValuePercent("VmNetworkReadWeightHigh", value);
                SetConfigValuePercent("VmNetworkReadWeightMedium", (int)(value * _networkReadMediumWeightFactor));
                SetConfigValuePercent("VmNetworkReadWeightLow", (int)(value * _networkReadLowWeightFactor));
            }
        }

        public int VmNetworkWriteWeightHigh
        {
            get { return GetConfigValuePercent("VmNetworkWriteWeightHigh"); }
            set
            {
                SetConfigValuePercent("VmNetworkWriteWeightHigh", value);
                SetConfigValuePercent("VmNetworkWriteWeightMedium", (int)(value * _networkWriteMediumWeightFactor));
                SetConfigValuePercent("VmNetworkWriteWeightLow", (int)(value* _networkWriteLowWeightFactor));
            }
        }

        // Removed from WLB Web Service in Tampa
        public double MetricGroomingPeriod
        {
            get { return GetConfigValueDouble("MetricGroomingPeriod", _metricGroomingPeriod); }
            set { SetConfigValueDouble("MetricGroomingPeriod", value); }
        }

        public bool CredentialsValid
        {
            get { return GetConfigValueBool("CredentialsValid", true); }
        }

        public double RecentMoveMinutes
        {
            get { return GetConfigValueDouble("RecentMoveMinutes"); }
            set { SetConfigValueDouble("RecentMoveMinutes", value); }
        }

        public bool SupportsAutoBalance
        {
            get { return IsMROrLater; }
        }

        public bool AutoBalanceEnabled
        {
            get { return GetConfigValueBool("AutoBalanceEnabled"); }
            set
            {
                if (IsMROrLater)
                {
                    SetConfigValueBool("AutoBalanceEnabled", value);
                }
            }
        }

        public double AutoBalancePollIntervals
        {
            get { return GetConfigValueDouble("AutoBalancePollIntervals"); }
            set
            {
                if (IsMROrLater)
                {
                    SetConfigValueDouble("AutoBalancePollIntervals", value);
                }
            }
        }

        public WlbPoolAutoBalanceSeverity AutoBalanceSeverity
        {
            get { return (WlbPoolAutoBalanceSeverity)Enum.Parse(typeof(WlbPoolAutoBalanceSeverity), GetConfigValueString("AutoBalanceSeverity")); }
            set
            {
                if (IsMROrLater)
                {
                    SetConfigValueString("AutoBalanceSeverity", value.ToString());
                }
            }
        }

        public WlbPoolAutoBalanceAggressiveness AutoBalanceAggressiveness
        {
            get { return (WlbPoolAutoBalanceAggressiveness)Enum.Parse(typeof(WlbPoolAutoBalanceAggressiveness), GetConfigValueString("AutoBalanceAggressiveness")); }
            set
            {
                if (IsMROrLater)
                {
                    SetConfigValueString("AutoBalanceAggressiveness", value.ToString());
                }
            }
        }

        public WlbAuditTrailLogGranularity PoolAuditGranularity
        {
            get { return (WlbAuditTrailLogGranularity)Enum.Parse(typeof(WlbAuditTrailLogGranularity), GetConfigValueString("PoolAuditLogGranularity")); }
            set
            {
                if (IsCreedenceOrLater)
                {
                    SetConfigValueString("PoolAuditLogGranularity", value.ToString());
                }
            }
        }

        public bool IsCreedenceOrLater
        {
            get { return ((this.WlbMajorVersion > WlbVersion_Major_Creedence) 
                || ((this.WlbMajorVersion == WlbVersion_Major_Creedence) && (this.WlbMinorVersion >= WlbVersion_Minor_Creedence))); }
        }

        public bool IsTampaOrLater
        {
            get { return ((this.WlbMajorVersion >= WlbVersion_Major_Tampa) && (this.WlbMinorVersion >= WlbVersion_Minor_Tampa)); }
        }

        public bool IsBostonOrLater
        {
            get { return (this.WlbMajorVersion >= WlbVersion_Major_Boston); }
        }

        public bool IsMROrLater
        {
            get
            {
                // WLB Boston and later have a nice WLB Version value to check.
                if (this.WlbMajorVersion != -1)
                {
                    return (this.WlbMajorVersion >= WlbVersion_Major_MNR);
                }
                //older version of WLB do not.
                else
                {
                    return (base.Configuration.ContainsKey("AutoBalanceAggressiveness"));
                }
            }
        }

        public int WlbMajorVersion
        {
            get
            {
                int majorVersion = -1;
                string versionString = GetConfigValueString("WlbVersion");
                if (!string.IsNullOrEmpty(versionString))
                {
                    int.TryParse(versionString.Split('.')[0], out majorVersion);
                }
                return majorVersion;
            }
        }

        public int WlbMinorVersion
        {
            get
            {
                int majorVersion = -1;
                string versionString = GetConfigValueString("WlbVersion");
                if (!string.IsNullOrEmpty(versionString))
                {
                    int.TryParse(versionString.Split('.')[1], out majorVersion);
                }
                return majorVersion;
            }
        }

        public bool PowerManagementEnabled
        {
            get {return GetConfigValueBool("PowerManagementEnabled");}
            set
            {
                if (IsMROrLater)
                {
                    SetConfigValueBool("PowerManagementEnabled", value);
                }
            }
        }

        public double PowerManagementPollIntervals
        {
            get { return GetConfigValueDouble("PowerManagementPollIntervals"); }
            set
            {
                if (IsMROrLater)
                {
                    SetConfigValueDouble("PowerManagementPollIntervals", value);
                }
            }
        }

        /* Not in use, removed from WLB web service in Tampa
        public bool ReportingUseRSServer
        {
            get { return GetConfigValueBool("ReportingUseRSServer"); }
            set { SetConfigValueBool("ReportingUseRSServer", value); }
        }
        */

        // Removed from WLB Web Service in Tampa
        public string ReportingSMTPServer
        {
            get { return GetConfigValueString("ReportingSMTPServer"); }
            set { SetConfigValueString("ReportingSMTPServer", value); }
        }

        public bool AutomateOptimizationMode
        {
            get { return GetConfigValueBool("EnableOptimizationModeSchedules"); }
            set 
            {
                if (IsMROrLater)
                {
                    SetConfigValueBool("EnableOptimizationModeSchedules", value);
                }
            }
        }

        public WlbHostConfigurations HostConfigurations
        {
            get { return _hostConfigurations; }
        }

        public WlbScheduledTasks ScheduledTasks
        {
            get { return _scheduledTasks; }
        }

#endregion

    }
}
