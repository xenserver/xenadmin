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
using System.Reflection;
using NUnit.Framework;
using XenAdmin.Wlb;


namespace XenAdminTests.UnitTests.WlbTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class WlbPoolConfigurationTests
    {
        private WlbPoolConfiguration wlbPool;
        private const int NUMBER_OF_PROPERTIES = 32;

        [Test]
        public void NullCtorThrows()
        {
            Assert.Throws(typeof(Exception), () => new WlbPoolConfiguration(null));
        }

        [Test]
        public void ConstructedWithEmptyDictionaryVerifyGettersReturnValues()
        {
            wlbPool = new WlbPoolConfiguration(new Dictionary<string, string>());
            PropertyInfo[] pi = wlbPool.GetType().GetProperties();

            Assert.AreEqual(NUMBER_OF_PROPERTIES, pi.Length, "Number of properties");

            List<string> fieldsToSkip = new List<string>
            {
                "AutoBalanceAggressiveness", //Enum
                "AutoBalanceSeverity",       //Enum
                "ReportingSMTPServer",       //Not set
                "PoolAuditGranularity"       //Enum
            };

            foreach (PropertyInfo propertyInfo in pi)
            {
                if( !fieldsToSkip.Contains( propertyInfo.Name ))
                {
                    string extractedValue = propertyInfo.GetValue(wlbPool, null).ToString();
                    Assert.That(extractedValue, Is.Not.Null.And.Not.Empty, $"PB: {propertyInfo.Name}");
                }
            }

            Assert.That(wlbPool.ReportingSMTPServer, Is.Null.Or.Empty);
        }

        [Test]
        public void SettersTest()
        {
            var inputData = new Dictionary<string, string>
            {
                {"WlbVersion", "6.0"},
                {"AutoBalanceEnabled", "false"},
                {"AutoBalancePollIntervals", "0.333333"},
                {"AutoBalanceSeverity", "High"},
                {"AutoBalanceAggressiveness", "High"},
                {"PowerManagementEnabled", "false"},
                {"PowerManagementPollIntervals", "5.321264"},
                {"EnableOptimizationModeSchedules", "false"} //Equal to AutomateOptimizationMode
            };

            var expectedData = new MRSensitiveData
            {
                AutoBalanceEnabled = true,
                AutoBalancePollIntervals = 2.0,
                AutoBalanceSeverity = WlbPoolAutoBalanceSeverity.Low,
                AutoBalanceAggressiveness = WlbPoolAutoBalanceAggressiveness.Medium,
                PowerManagementEnabled = true,
                PowerManagementPollIntervals = 7.0,
                AutomateOptimizationMode = true
            };

            ClassVerifiers.VerifySettersAndGetters(new WlbPoolConfiguration(inputData), expectedData);
        }

        [Test]
        public void SettersThatSetVerbatim()
        {
            var inputData = new Dictionary<string, string>
            {
                //Simple setters.....
                {"OptimizationMode", "MaximizeDensity"}, //PerformanceMode
                {"RecentMoveMinutes", "2.0"},
                //{"ReportingUseRSServer", "false"},
                {"ReportingSMTPServer", "some string"},
                //These set ranges of values.....
                {"HostCpuThresholdCritical", "3"},
                {"HostMemoryThresholdCritical", "3.0"},
                {"HostPifReadThresholdCritical", "4.0"}, //HostNetworkReadThresholdCritical
                {"HostPifWriteThresholdCritical", "5.0"}, //HostNetworkWriteThresholdCritical
                {"HostPbdReadThresholdCritical", "6.0"}, //HostDiskReadThresholdCritical
                {"HostPbdWriteThresholdCritical", "7.0"}, //HostDiskWriteThresholdCritical
                {"VmCpuUtilizationWeightHigh", "8"},
                {"VmMemoryWeightHigh", "9"},
                {"VmDiskReadWeightHigh", "10"},
                {"VmDiskWriteWeightHigh", "11"},
                {"VmNetworkReadWeightHigh", "12"},
                {"VmNetworkWriteWeightHigh", "13"}
            };

            var data = new MRInSensitiveData
            {
                PerformanceMode = WlbPoolPerformanceMode.MaximizePerformance,
                RecentMoveMinutes = 6.0,
                //ReportingUseRSServer = true,
                ReportingSMTPServer = "who knows what?!",
                HostCpuThresholdCritical = 1,
                HostMemoryThresholdCritical = 2,
                HostNetworkReadThresholdCritical = 1.0,
                HostNetworkWriteThresholdCritical = 2.0,
                HostDiskReadThresholdCritical = 3.0,
                HostDiskWriteThresholdCritical = 4.0,
                VmCpuUtilizationWeightHigh = 5,
                VmMemoryWeightHigh = 6,
                VmDiskReadWeightHigh = 7,
                VmDiskWriteWeightHigh = 8,
                VmNetworkReadWeightHigh = 9,
                VmNetworkWriteWeightHigh = 10
            };

            ClassVerifiers.VerifySettersAndGetters(new WlbPoolConfiguration(inputData), data);
        }

        [Test]
        public void OvercommitCPUsGetTheCorrectValueForDifferentModes()
        {
           var inputData = new Dictionary<string, string>
            {
                {"OverCommitCpuInDensityMode", "false"},
                {"OverCommitCpuInPerfMode", "true"}
            };

            wlbPool = new WlbPoolConfiguration(inputData);

            //Setter drops through if the key has not been added to the dictionary so won't set the values
            wlbPool.PerformanceMode = WlbPoolPerformanceMode.MaximizeDensity;
            //This should be false if the key were there
            Assert.IsTrue(wlbPool.OvercommitCPUs, "OvercommitCPUs in MaximizeDensity mode without key");

            //Add the key and set the data - this is now the default behaviour
            wlbPool.AddParameter("OptimizationMode", "MaximizeDensity");
            wlbPool.PerformanceMode = WlbPoolPerformanceMode.MaximizeDensity;
            Assert.IsFalse(wlbPool.OvercommitCPUs, "OvercommitCPUs in MaximizeDensity mode");

            wlbPool.PerformanceMode = WlbPoolPerformanceMode.MaximizePerformance;
            Assert.IsTrue(wlbPool.OvercommitCPUs, "OvercommitCPUs in MaximizePerformance");
        }

        #region Helper structs

        private struct MRSensitiveData
        {
            public bool AutoBalanceEnabled;
            public double AutoBalancePollIntervals;
            public WlbPoolAutoBalanceSeverity AutoBalanceSeverity;
            public WlbPoolAutoBalanceAggressiveness AutoBalanceAggressiveness;
            public bool PowerManagementEnabled;
            public double PowerManagementPollIntervals;
            public bool AutomateOptimizationMode;
        }

        private struct MRInSensitiveData
        {
            public WlbPoolPerformanceMode PerformanceMode;
            public double RecentMoveMinutes;
            public string ReportingSMTPServer;
            public int HostCpuThresholdCritical;
            public double HostMemoryThresholdCritical;
            public double HostNetworkReadThresholdCritical;
            public double HostNetworkWriteThresholdCritical;
            public double HostDiskReadThresholdCritical;
            public double HostDiskWriteThresholdCritical;
            public int VmCpuUtilizationWeightHigh;
            public int VmMemoryWeightHigh;
            public int VmDiskReadWeightHigh;
            public int VmDiskWriteWeightHigh;
            public int VmNetworkReadWeightHigh;
            public int VmNetworkWriteWeightHigh;
        }

        #endregion
    }
}
