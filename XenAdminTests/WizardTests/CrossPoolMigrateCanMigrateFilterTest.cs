/* Copyright (c) Citrix Systems Inc. 
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
using System.Linq;
using NUnit.Framework;
using XenAdmin.Wizards.CrossPoolMigrateWizard;
using XenAdmin.Wizards.CrossPoolMigrateWizard.Filters;
using XenAPI;

namespace XenAdminTests.WizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class CrossPoolMigrateCanMigrateFilterTest : DatabaseTester_TestFixture
    {
        private const string oneHostPool = "tampa_livevdimove_xapi-db.xml";
        private const string twoHostPool = "tampa-poolof16and23-xapi-db.xml";
        private Host singlePoolHost;
        private Pool singlePool;
        private List<VM> singleResidentVm;
        private List<VM> singleNonResidentVm;

        public CrossPoolMigrateCanMigrateFilterTest()
            : base(oneHostPool, twoHostPool)
        {

        }

        private void Init()
        {
            singlePool = DatabaseManager.ConnectionFor(oneHostPool).Cache.Pools.First(p => p.name_label == "40");
            Assert.IsNotNull(singlePool, "Resolved pool");

            singlePoolHost = DatabaseManager.ConnectionFor(oneHostPool).Cache.Hosts.First(h => h.name_label == "dt40");
            Assert.IsNotNull(singlePoolHost, "Resolved host");

            singleResidentVm = new List<VM> { DatabaseManager.ConnectionFor(oneHostPool).Cache.VMs.First(h => h.name_label == "local") };
            Assert.IsNotEmpty(singleResidentVm, "Resolved VM missing");

            singleNonResidentVm = new List<VM> { DatabaseManager.ConnectionFor(twoHostPool).Cache.VMs.First(h => h.name_label == "16local") };
            Assert.IsNotEmpty(singleNonResidentVm, "Resolved VM missing");
        }

        private class TestData
        {
            public TestData(IXenObject ixo, List<VM> vms)
            {
                this.ixo = ixo;
                this.vms = vms;
            }

            public IXenObject ixo { get; private set; }
            public List<VM> vms { get; private set; }
        }

        private IEnumerable TestCasesMigrationAllowed
        {
            get
            {
                //Use host/pool with VMs from a same pool
                yield return new TestData(singlePoolHost, singleResidentVm);
                yield return new TestData(singlePool, singleResidentVm);
                yield return new TestData(singlePoolHost, new List<VM>());
                yield return new TestData(singlePool, new List<VM>());

                //Unsupported IXenObjects
                yield return new TestData(singleResidentVm.First(), singleResidentVm);
                yield return new TestData(singleResidentVm.First(), singleNonResidentVm);
                yield return new TestData(singleNonResidentVm.First(), singleResidentVm);
                yield return new TestData(singleNonResidentVm.First(), singleNonResidentVm);
                yield return new TestData(singleResidentVm.First(), new List<VM>());

                //Null host/pool
                yield return new TestData(null, singleResidentVm);
                yield return new TestData(null, singleNonResidentVm);
                yield return new TestData(null, new List<VM>());
            }
        }

        private IEnumerable TestCasesMigrationNotAllowedIfAssertion
        {
            get
            {
                //Use host/pool with VMs from a same pool
                yield return new TestData(singlePoolHost, singleNonResidentVm);
                yield return new TestData(singlePool, singleNonResidentVm);
            }
        }

        [Test]
        public void VerifyMigrationAllowed()
        {
            Init();
            foreach (TestData data in TestCasesMigrationAllowed)
            {
                VerifyMigrationAllowed(data.ixo, data.vms);
            }
        }

        private void VerifyMigrationAllowed(IXenObject ixo, List<VM> vms)
        {
            CrossPoolMigrateCanMigrateFilter cmd = new CrossPoolMigrateCanMigrateFilter(ixo, vms, WizardMode.Migrate);
            Assert.That(cmd.FailureFound, Is.False, "failure found");
            Assert.That(cmd.Reason, Is.Null, "failure message");
        }

        [Test, ExpectedException(typeof(NullReferenceException))]
        public void VerifyNullListArgsThrow()
        {
            bool failureFound = new CrossPoolMigrateCanMigrateFilter(singlePool, null, WizardMode.Migrate).FailureFound;
        }

        [Test]
        public void VerifyMigrationNotAllowed()
        {
            Init();
            foreach (TestData data in TestCasesMigrationNotAllowedIfAssertion)
            {
                Assert.That(data.vms.Count, Is.AtLeast(1), "VM count needs to be at least 1 for this test");

                //fakeVM will say a failure is found if vm ref is set to Failure.INTERNAL_ERROR
                data.vms[0].opaque_ref = Failure.INTERNAL_ERROR;
                Assert.That(data.vms[0].opaque_ref, Is.EqualTo(Failure.INTERNAL_ERROR));

                CrossPoolMigrateCanMigrateFilter cmd = new CrossPoolMigrateCanMigrateFilter(data.ixo, data.vms, WizardMode.Migrate);
                Assert.That(cmd.FailureFound, Is.True, "failure found");
                Assert.That(cmd.Reason, Is.EqualTo(Failure.INTERNAL_ERROR), "failure message");

            }

        }
    }
}
