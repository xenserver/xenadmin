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
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Wizards.CrossPoolMigrateWizard.Filters;

namespace XenAdminTests.WizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class CrossPoolMigrateVersionFilterTest : DatabaseTester_TestFixture
    {
        private const string tampaDb = "tampa_livevdimove_xapi-db.xml";
        private const string bostonDb = "boston-db.xml";

        public CrossPoolMigrateVersionFilterTest(): base(tampaDb, bostonDb){}

        [Test]
        public void VerifyFailureIsFoundBostonHost()
        {
            CrossPoolMigrateVersionFilter cmd = new CrossPoolMigrateVersionFilter(DatabaseManager.ConnectionFor(bostonDb).Cache.Hosts[0]);
            Assert.IsTrue(cmd.FailureFound, "Failure found for boston host");
            Assert.AreEqual(Messages.CPM_FAILURE_REASON_VERSION, cmd.Reason, "Failure found reason for boston host");
        }

        [Test]
        public void VerifyFailureIsFoundBostonPool()
        {
            CrossPoolMigrateVersionFilter cmd = new CrossPoolMigrateVersionFilter(DatabaseManager.ConnectionFor(bostonDb).Cache.Pools[0]);
            Assert.IsTrue(cmd.FailureFound, "Failure found for boston pool");
            Assert.AreEqual(Messages.CPM_FAILURE_REASON_VERSION, cmd.Reason, "Failure found reason for boston pool");
        }

        [Test]
        public void VerifyFailureIsNotFoundTampaHost()
        {
            CrossPoolMigrateVersionFilter cmd = new CrossPoolMigrateVersionFilter(DatabaseManager.ConnectionFor(tampaDb).Cache.Hosts[0]);
            Assert.IsFalse(cmd.FailureFound, "Failure found for tampa host");
            Assert.AreEqual(Messages.CPM_FAILURE_REASON_VERSION, cmd.Reason, "Failure found reason for tampa host");
        }

        [Test]
        public void VerifyFailureIsNotFoundTampaPool()
        {
            CrossPoolMigrateVersionFilter cmd = new CrossPoolMigrateVersionFilter(DatabaseManager.ConnectionFor(tampaDb).Cache.Pools[0]);
            Assert.IsFalse(cmd.FailureFound, "Failure found for tampa pool");
            Assert.AreEqual(Messages.CPM_FAILURE_REASON_VERSION, cmd.Reason, "Failure found reason for tampa pool");
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ExceptionIsThrownIfNotHostOrPool()
        {
            bool failureFound = new CrossPoolMigrateVersionFilter(DatabaseManager.ConnectionFor(tampaDb).Cache.VMs[0]).FailureFound;
        }
    }
}
