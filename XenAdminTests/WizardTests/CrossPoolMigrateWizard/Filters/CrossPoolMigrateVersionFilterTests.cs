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
using Moq;
using NUnit.Framework;
using XenAdmin.Wizards.CrossPoolMigrateWizard.Filters;
using XenAPI;

namespace XenAdminTests.WizardTests
{
    public class CrossPoolMigrateVersionFilterTests : UnitTester_SingleConnectionTestFixture
    {
        [Test]
        [TestCase("1.0.99", true)]
        [TestCase("1.6.10", false)]
        [TestCase("999.999.999", false)]
        [TestCase("1.5.50", false)]
        [TestCase("1.5.49", true)]
        public void TestVersionsForPool(string platformVersion, bool expected)
        {
            Mock<Host> host = ObjectManager.NewXenObject<Host>(id);
            host.Setup(c => c.PlatformVersion).Returns(platformVersion);
            ObjectManager.NewXenObject<Pool>(id);
            ObjectManager.MockConnectionFor(id).Setup(c => c.Resolve(It.IsAny<XenRef<Host>>())).Returns(host.Object);

            CrossPoolMigrateVersionFilter filter = new CrossPoolMigrateVersionFilter(host.Object);
            Assert.That(filter.FailureFound, Is.EqualTo(expected));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestUnsupportedObject()
        {
            Mock<VM> vm = ObjectManager.NewXenObject<VM>(id);
            CrossPoolMigrateVersionFilter filter = new CrossPoolMigrateVersionFilter(vm.Object);
            bool b = filter.FailureFound;
        }

        [Test]
        [ExpectedException(typeof(OutOfMemoryException))]
        public void TestPlatformVersionThrows()
        {
            Mock<Host> host = ObjectManager.NewXenObject<Host>(id);
            host.SetupGet(c => c.PlatformVersion).Throws<OutOfMemoryException>();
            ObjectManager.NewXenObject<Pool>(id);
            ObjectManager.MockConnectionFor(id).Setup(c => c.Resolve(It.IsAny<XenRef<Host>>())).Returns(host.Object);

            CrossPoolMigrateVersionFilter filter = new CrossPoolMigrateVersionFilter(host.Object);
            bool b = filter.FailureFound;
        }
    }
} 

