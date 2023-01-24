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

using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using XenAdmin.Network;
using XenAdmin.XenSearch;
using XenAPI;


namespace XenAdminTests.SearchTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class VirtualisationStatePropertyQueryTests
    {
        private List<Mock<VM>> _allVms = new List<Mock<VM>>();

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            var conn = new Mock<IXenConnection>(MockBehavior.Strict);
            conn.Setup(x => x.Name).Returns("conn");
            var cache = new Cache();
            conn.Setup(x => x.Cache).Returns(cache);

            var host = new Mock<Host>();
            host.Object.Connection = conn.Object;
            host.Object.opaque_ref = "OpaqueRef:host";
            host.Setup(x => x.PlatformVersion()).Returns("3.2.0");

            var pool = new Mock<Pool>();
            pool.Object.Connection = conn.Object;
            pool.Setup(x => x.master).Returns(new XenRef<Host>(host.Object.opaque_ref));

            //Linux VM with outdated tools

            var m0 = new Mock<VM_metrics>();
            m0.Object.Connection = conn.Object;
            m0.Object.opaque_ref = "OpaqueRef:m0";

            var g0 = new Mock<VM_guest_metrics>();
            g0.Object.Connection = conn.Object;
            g0.Object.opaque_ref = "OpaqueRef:g0";
            g0.Setup(x => x.PV_drivers_detected).Returns(true);
            g0.Setup(x => x.PV_drivers_up_to_date).Returns(false);
            g0.Setup(x => x.PV_drivers_version).Returns(new Dictionary<string, string> {{"major", "1"}, {"minor", "1"}});

            var vm0 = new Mock<VM>();
            vm0.Object.Connection = conn.Object;
            vm0.Setup(x => x.Name()).Returns("vm0");
            vm0.Setup(x => x.power_state).Returns(vm_power_state.Running);
            vm0.Setup(x => x.guest_metrics).Returns(new XenRef<VM_guest_metrics>(g0.Object.opaque_ref));
            vm0.Setup(x => x.metrics).Returns(new XenRef<VM_metrics>(m0.Object.opaque_ref));

            //Linux VM with up to date tools

            var m1 = new Mock<VM_metrics>();
            m1.Object.Connection = conn.Object;
            m1.Object.opaque_ref = "OpaqueRef:m1";

            var g1 = new Mock<VM_guest_metrics>();
            g1.Object.Connection = conn.Object;
            g1.Object.opaque_ref = "OpaqueRef:g1";
            g1.Setup(x => x.PV_drivers_detected).Returns(true);
            g1.Setup(x => x.PV_drivers_up_to_date).Returns(true);
            g1.Setup(x => x.PV_drivers_version).Returns(new Dictionary<string, string> {{"major", "9"}, {"minor", "0"}});

            var vm1 = new Mock<VM>();
            vm1.Object.Connection = conn.Object;
            vm1.Setup(x => x.Name()).Returns("vm1");
            vm1.Setup(x => x.power_state).Returns(vm_power_state.Running);
            vm1.Setup(x => x.guest_metrics).Returns(new XenRef<VM_guest_metrics>(g1.Object.opaque_ref));
            vm1.Setup(x => x.metrics).Returns(new XenRef<VM_metrics>(m1.Object.opaque_ref));

            //Windows VM without tools

            var m2 = new Mock<VM_metrics>();
            m2.Object.Connection = conn.Object;
            m2.Object.opaque_ref = "OpaqueRef:m2";

            var g2 = new Mock<VM_guest_metrics>();
            g2.Object.Connection = conn.Object;
            g2.Object.opaque_ref = "OpaqueRef:g2";
            g2.Setup(x => x.PV_drivers_detected).Returns(false);

            var vm2 = new Mock<VM>();
            vm2.Object.Connection = conn.Object;
            vm2.Setup(x => x.Name()).Returns("vm2");
            vm2.Setup(x => x.power_state).Returns(vm_power_state.Running);
            vm2.Setup(x => x.HVM_boot_policy).Returns("BIOS order");
            vm2.Setup(x => x.platform).Returns(new Dictionary<string, string> {{"viridian", "true"}});
            vm2.Setup(x => x.guest_metrics).Returns(new XenRef<VM_guest_metrics>(g2.Object.opaque_ref));
            vm2.Setup(x => x.metrics).Returns(new XenRef<VM_metrics>(m2.Object.opaque_ref));

            //Windows VM IO optimised only

            var m3 = new Mock<VM_metrics>();
            m3.Object.Connection = conn.Object;
            m3.Object.opaque_ref = "OpaqueRef:m3";

            var g3 = new Mock<VM_guest_metrics>();
            g3.Object.Connection = conn.Object;
            g3.Object.opaque_ref = "OpaqueRef:g3";
            g3.Setup(x => x.PV_drivers_detected).Returns(true);

            var vm3 = new Mock<VM>();
            vm3.Object.Connection = conn.Object;
            vm3.Setup(x => x.Name()).Returns("vm3");
            vm3.Setup(x => x.power_state).Returns(vm_power_state.Running);
            vm3.Setup(x => x.HVM_boot_policy).Returns("BIOS order");
            vm3.Setup(x => x.platform).Returns(new Dictionary<string, string> {{"viridian", "true"}});
            vm3.Setup(x => x.guest_metrics).Returns(new XenRef<VM_guest_metrics>(g3.Object.opaque_ref));
            vm3.Setup(x => x.metrics).Returns(new XenRef<VM_metrics>(m3.Object.opaque_ref));

            //Windows VM with management agent installed only

            var m4 = new Mock<VM_metrics>();
            m4.Object.Connection = conn.Object;
            m4.Object.opaque_ref = "OpaqueRef:m4";

            var g4 = new Mock<VM_guest_metrics>();
            g4.Object.Connection = conn.Object;
            g4.Object.opaque_ref = "OpaqueRef:g4";
            g4.Setup(x => x.PV_drivers_detected).Returns(false);
            g4.Setup(x => x.other).Returns(new Dictionary<string, string> {{"feature-static-ip-setting", "1"}});

            var vm4 = new Mock<VM>();
            vm4.Object.Connection = conn.Object;
            vm4.Setup(x => x.Name()).Returns("vm4");
            vm4.Setup(x => x.power_state).Returns(vm_power_state.Running);
            vm4.Setup(x => x.HVM_boot_policy).Returns("BIOS order");
            vm4.Setup(x => x.platform).Returns(new Dictionary<string, string> {{"viridian", "true"}});
            vm4.Setup(x => x.guest_metrics).Returns(new XenRef<VM_guest_metrics>(g4.Object.opaque_ref));
            vm4.Setup(x => x.metrics).Returns(new XenRef<VM_metrics>(m4.Object.opaque_ref));

            //Windows VM fully optimised

            var m5 = new Mock<VM_metrics>();
            m5.Object.Connection = conn.Object;
            m5.Object.opaque_ref = "OpaqueRef:m5";

            var g5 = new Mock<VM_guest_metrics>();
            g5.Object.Connection = conn.Object;
            g5.Object.opaque_ref = "OpaqueRef:g5";
            g5.Setup(x => x.PV_drivers_detected).Returns(true);
            g5.Setup(x => x.other).Returns(new Dictionary<string, string> {{"feature-static-ip-setting", "1"}});

            var vm5 = new Mock<VM>();
            vm5.Object.Connection = conn.Object;
            vm5.Setup(x => x.Name()).Returns("vm5");
            vm5.Setup(x => x.power_state).Returns(vm_power_state.Running);
            vm5.Setup(x => x.HVM_boot_policy).Returns("BIOS order");
            vm5.Setup(x => x.platform).Returns(new Dictionary<string, string> {{"viridian", "true"}});
            vm5.Setup(x => x.guest_metrics).Returns(new XenRef<VM_guest_metrics>(g5.Object.opaque_ref));
            vm5.Setup(x => x.metrics).Returns(new XenRef<VM_metrics>(m5.Object.opaque_ref));

            //Linux VM with unknown state

            var m6 = new Mock<VM_metrics>();
            m6.Object.Connection = conn.Object;
            m6.Object.opaque_ref = "OpaqueRef:m6";

            var vm6 = new Mock<VM>();
            vm6.Object.Connection = conn.Object;
            vm6.Setup(x => x.Name()).Returns("vm6");
            vm6.Setup(x => x.power_state).Returns(vm_power_state.Running);
            vm6.Setup(x => x.metrics).Returns(new XenRef<VM_metrics>(m6.Object.opaque_ref));

            _allVms.AddRange(new[] {vm0, vm1, vm2, vm3, vm4, vm5, vm6});

            conn.Setup(x => x.Resolve(new XenRef<VM_metrics>("OpaqueRef:m0"))).Returns(m0.Object);
            conn.Setup(x => x.Resolve(new XenRef<VM_metrics>("OpaqueRef:m1"))).Returns(m1.Object);
            conn.Setup(x => x.Resolve(new XenRef<VM_metrics>("OpaqueRef:m2"))).Returns(m2.Object);
            conn.Setup(x => x.Resolve(new XenRef<VM_metrics>("OpaqueRef:m3"))).Returns(m3.Object);
            conn.Setup(x => x.Resolve(new XenRef<VM_metrics>("OpaqueRef:m4"))).Returns(m4.Object);
            conn.Setup(x => x.Resolve(new XenRef<VM_metrics>("OpaqueRef:m5"))).Returns(m5.Object);
            conn.Setup(x => x.Resolve(new XenRef<VM_metrics>("OpaqueRef:m6"))).Returns(null as VM_metrics);

            conn.Setup(x => x.Resolve(new XenRef<VM_guest_metrics>("OpaqueRef:g0"))).Returns(g0.Object);
            conn.Setup(x => x.Resolve(new XenRef<VM_guest_metrics>("OpaqueRef:g1"))).Returns(g1.Object);
            conn.Setup(x => x.Resolve(new XenRef<VM_guest_metrics>("OpaqueRef:g2"))).Returns(g2.Object);
            conn.Setup(x => x.Resolve(new XenRef<VM_guest_metrics>("OpaqueRef:g3"))).Returns(g3.Object);
            conn.Setup(x => x.Resolve(new XenRef<VM_guest_metrics>("OpaqueRef:g4"))).Returns(g4.Object);
            conn.Setup(x => x.Resolve(new XenRef<VM_guest_metrics>("OpaqueRef:g5"))).Returns(g5.Object);
        }

        [Test, Sequential]
        [TestCase("Not optimized", new[] {"vm2"})]
        [TestCase("Unknown", new[] {"vm6"})]
        [TestCase("Out of date", new[] {"vm0"})]
        [TestCase("I/O optimized only", new[] {"vm3"})]
        [TestCase("Management Agent installed only", new[] {"vm4"})]
        [TestCase("Fully optimized", new[] {"vm1", "vm5"})]
        public void TestVirtStatusIs(string filter, string[] expectedVmNames)
        {
            var dict = PropertyAccessors.Geti18nFor(PropertyNames.virtualisation_status) as Dictionary<string, VM.VirtualisationStatus>;

            Assert.NotNull(dict, "Did not find i18n for VM.VirtualisationStatus");
            Assert.IsTrue(dict.TryGetValue(filter, out var status), $"Did not find i18n for {filter}");

            var query = new EnumPropertyQuery<VM.VirtualisationStatus>(PropertyNames.virtualisation_status, status, true);
            CheckMatch(query, expectedVmNames);
        }

        [Test, Sequential]
        [TestCase("Not optimized", new[] {"vm0", "vm1", "vm3", "vm4", "vm5", "vm6"})]
        [TestCase("Unknown", new[] {"vm0", "vm1", "vm2", "vm3", "vm4", "vm5"})]
        [TestCase("Out of date", new[] {"vm1", "vm2", "vm3", "vm4", "vm5", "vm6"})]
        [TestCase("I/O optimized only", new[] {"vm0", "vm1", "vm2", "vm4", "vm5", "vm6"})]
        [TestCase("Management Agent installed only", new[] {"vm0", "vm1", "vm2", "vm3", "vm5", "vm6"})]
        [TestCase("Fully optimized", new[] {"vm0", "vm2", "vm3", "vm4", "vm6"})]
        public void TestVirtStatusIsNot(string filter, string[] expectedVmNames)
        {
            var dict = PropertyAccessors.Geti18nFor(PropertyNames.virtualisation_status) as Dictionary<string, VM.VirtualisationStatus>;

            Assert.NotNull(dict, "Did not find i18n for VM.VirtualisationStatus");
            Assert.IsTrue(dict.TryGetValue(filter, out var status), $"Did not find i18n for {filter}");

            var query = new EnumPropertyQuery<VM.VirtualisationStatus>(PropertyNames.virtualisation_status, status, false);
            CheckMatch(query, expectedVmNames);
        }

        private void CheckMatch(EnumPropertyQuery<VM.VirtualisationStatus> query, string[] expectedVmNames)
        {
            foreach (var moqVm in _allVms)
            {
                var vm = moqVm.Object;
                bool? match = query.MatchProperty(vm.GetVirtualisationStatus(out _));
                Assert.True(match.HasValue);

                var name = vm.Name();
                if (expectedVmNames.Contains(name))
                    Assert.True(match.Value, $"VM {name} was not in the expected query results");
                else
                    Assert.False(match.Value, $"Found unexpected VM {name} in the query results");
            }
        }
    }
}
