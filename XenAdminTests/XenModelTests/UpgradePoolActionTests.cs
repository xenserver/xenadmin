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
using System.Linq;
using NUnit.Framework;
using XenAdmin.Core;
using XenAdmin.Wizards.PatchingWizard.PlanActions;
using XenAPI;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class UpgradePoolActionTests : ActionTestBase
    {

        [Test]
        public void TestFirstMaster()
        {
            //Add master
            Pool pool = GetPool("1","1");
            Assert.AreEqual(2,pool.HostsToUpgrade.Count);
            Assert.IsTrue(pool.HostsToUpgrade.First().IsMaster());
            Assert.IsFalse(pool.HostsToUpgrade.Last().IsMaster());
        }

        [Test]
        public void TestOnlySlave()
        {
            //Add master
            Pool pool = GetPool("2","1");
            Assert.AreEqual(1, pool.HostsToUpgrade.Count);
            Assert.IsFalse(pool.HostsToUpgrade.First().IsMaster());
        }

        private Pool GetPool(string masterVersion,string slaveVersion)
        {
            Host master = new Host { uuid = "master", opaque_ref = "master", Connection = mockConnection.Object, software_version = new Dictionary<string, string> { { "product_version", masterVersion } } };
           
            cache.UpdateFrom(mockConnection.Object,
                             new List<ObjectChange>() { new ObjectChange(master.GetType(), "master", master) });

            Pool pool = new Pool { Connection = mockConnection.Object, opaque_ref = "pool", master = new XenRef<Host>(master) };
            //Add pool to the cache
            cache.UpdateFrom(mockConnection.Object,
                             new List<ObjectChange>() { new ObjectChange(pool.GetType(), "pool", pool) });
           
            //Add slave
            Host slave = new Host { uuid = "slave",opaque_ref = "slave", Connection = mockConnection.Object, software_version = new Dictionary<string, string> { { "product_version", slaveVersion } } };
            cache.UpdateFrom(mockConnection.Object,
                             new List<ObjectChange>() { new ObjectChange(slave.GetType(), "slave", slave) });

            mockConnection.Setup(x => x.Resolve(new XenRef<Host>("master"))).Returns(master);
            return pool;
        }
    }
}
