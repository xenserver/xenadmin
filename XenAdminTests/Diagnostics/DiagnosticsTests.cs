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

using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Diagnostics.Problems.HostProblem;
using XenAdmin.Diagnostics.Problems.PoolProblem;
using XenAdmin.Network;
using XenAdminTests.XenModelTests;
using XenAPI;
using XenAdmin.Core;

namespace XenAdminTests.Diagnostics
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    class DiagnosticsTests
    {
        #region HostLivenessCheck
        [Test]
        //Check that the host is live and it does not return any problem.
        public void CheckHostEnabled()
        {
            var fake = new Mock<IXenConnection>(); 
            var host = GetHostLiveness(fake,true,false,true);

            //Execute the check
            var check = new HostLivenessCheck(host);
            var problems = check.RunAllChecks();
            Assert.IsEmpty(problems);
            fake.VerifyAll();
        }

        [Test]
        public void CheckHostDisabled()
        {
            var fake = new Mock<IXenConnection>(); 
            var host = GetHostLiveness(fake, false, false, true);

            //Execute the check
            var check = new HostMaintenanceModeCheck(host);
            var problems = check.RunAllChecks();
            Assert.IsNotEmpty(problems);
            Assert.AreEqual(typeof(HostMaintenanceMode), problems[0].GetType());
            fake.VerifyAll();
        }

        [Test]
        public void CheckHostMaintenanceMode()
        {
            var mock = new Mock<IXenConnection>(); 
            var host = GetHostLiveness(mock, true, true, true);

            //Execute the check
            var check = new HostMaintenanceModeCheck(host);
            var problems = check.RunAllChecks();
            Assert.IsNotEmpty(problems);
            Assert.AreEqual(typeof(HostMaintenanceMode), problems[0].GetType());
            mock.VerifyAll();
        }

        [Test]
        public void CheckHostNotLive()
        {
            var mock = new Mock<IXenConnection>(); 
            var host = GetHostLiveness(mock, true, false, false);

            //Execute the check
            var check = new HostLivenessCheck(host);
            var problems = check.RunAllChecks();
            Assert.IsNotEmpty(problems);
            Assert.AreEqual(typeof(HostNotLive), problems[0].GetType());
            mock.VerifyAll();
        }
        #endregion

        #region HAOffCheck

        [Test]
        public void CheckHAOff()
        {
            var host = GetHostHA(false);

            //Execute the check
            var check = new HAOffCheck(host);
            var problems = check.RunAllChecks();
            Assert.IsEmpty(problems);
        }

        [Test]
        public void CheckHAOffNoPool()
        {
            var host = GetHostLiveness(GetMockConnectionWithCache(), true, false, true);
            //Execute the check
            var check = new HAOffCheck(host);
            var problems = check.RunAllChecks();
            Assert.IsEmpty(problems);
        }

        [Test]
        public void CheckHAON()
        {
            var host = GetHostHA(true);

            //Execute the check
            var check = new HAOffCheck(host);
            var problems = check.RunAllChecks();
            Assert.IsNotEmpty(problems);
            Assert.AreEqual(typeof(HAEnabledProblem), problems[0].GetType());
        }

        #endregion

        #region PBDsCheck

        [Test]
        public void CheckPBDsCorrectNoVMs()
        {
            var mockConnection = GetMockConnectionWithCache();
            var host = GetHostLiveness(mockConnection, true, false, true);
            //Execute the check
            var check = new PBDsPluggedCheck(host);
            var problems = check.RunAllChecks();
            Assert.IsEmpty(problems);
        }

        [Test]
        public void CheckPBDsCorrectVM()
        {
            var mockConnection = GetMockConnectionWithCache();
            var host = GetHostLiveness(mockConnection, true, false, true);
            var vmMock = new MockVMBuilder("0", mockConnection);
            var vm = vmMock.VM;
            vm.power_state = vm_power_state.Running;
            var listofchanges = new List<ObjectChange>() {new ObjectChange(vm.GetType(), "0", vm)};
            host.Connection.Cache.UpdateFrom(host.Connection, listofchanges);
            //Execute the check
            var check = new PBDsPluggedCheck(host);
            var problems = check.RunAllChecks();
            Assert.IsEmpty(problems);
        }

        #endregion

        #region Helper functions

        private static Host GetHostLiveness(Mock<IXenConnection> fake,bool hostEnabled,bool hostMaintenanceMode,bool hostLive)
        {
            var host = new Host();
            var metrics = new XenRef<Host_metrics>("0");
            host.metrics = metrics;
            //Metric live
            var metric = new Host_metrics {live = hostLive};
            host.other_config = new Dictionary<string, string>();
            //Maintenance mode
            host.other_config.Add("MAINTENANCE_MODE", hostMaintenanceMode.ToString().ToLower());
            Assert.AreEqual(hostMaintenanceMode,host.MaintenanceMode);
            fake.Setup(x => x.Resolve(It.IsAny<XenRef<Host_metrics>>())).Returns(metric);
            //Host enabled
            host.enabled = hostEnabled;
            host.Connection = fake.Object;
            return host;
        }

        private static Host GetHostHA( bool HAEnabled)
        {
            var host = GetHostLiveness(GetMockConnectionWithCache(), true, false, true);
            var pool = new Pool() {ha_enabled = HAEnabled,ha_statefiles = new string[0]};
            var listofChanges = new List<ObjectChange>() {new ObjectChange(pool.GetType(), "0", pool)};
            host.Connection.Cache.UpdateFrom(host.Connection, listofChanges);
            return host;
        }

        private static Mock<IXenConnection> GetMockConnectionWithCache()
        {
            var mockConnection = new Mock<IXenConnection>();
            mockConnection.Setup(x => x.Cache).Returns(new Cache());
            return mockConnection;
        }

        #endregion
    }
}
