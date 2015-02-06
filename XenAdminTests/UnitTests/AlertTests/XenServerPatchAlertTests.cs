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
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using XenAdmin.Alerts;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdminTests.UnitTests.UnitTestHelper;
using XenAPI;

namespace XenAdminTests.UnitTests.AlertTests
{
    [TestFixture, Category(TestCategories.Unit), Category(TestCategories.SmokeTest)]
    public class XenServerPatchAlertTests
    {
        private Mock<IXenConnection> connA;
        private Mock<IXenConnection> connB;
        private Mock<Host> hostA;
        private Mock<Host> hostB;

        [Test]
        public void TestAlertWithConnectionAndHosts()
        {
            XenServerPatch p = new XenServerPatch("uuid", "name", "My description", "guidance", "6.0.1", "http://url", "http://patchUrl", new DateTime(2011, 4, 1).ToString(), "", "");
            XenServerPatchAlert alert = new XenServerPatchAlert(p);
            alert.IncludeConnection(connA.Object);
            alert.IncludeConnection(connB.Object);
            alert.IncludeHosts(new List<Host> { hostA.Object, hostB.Object });

            IUnitTestVerifier validator = new VerifyGetters(alert);

            validator.Verify(new AlertClassUnitTestData
            {
                AppliesTo = "HostAName, HostBName, ConnAName, ConnBName",
                FixLinkText = "Go to Web Page",
                HelpID = "XenServerPatchAlert",
                Description = "My description",
                HelpLinkText = "Help",
                Title = "New Update Available - name",
                Priority = "Priority2"
            });

            Assert.IsFalse(alert.CanIgnore);

            VerifyConnExpectations(Times.Once);
            VerifyHostsExpectations(Times.Once);
        }

        [Test]
        public void TestAlertWithHostsAndNoConnection()
        {
            XenServerPatch p = new XenServerPatch("uuid", "name", "My description", "guidance", "6.0.1", "http://url", "http://patchUrl", new DateTime(2011, 4, 1).ToString(), "1", "");
            XenServerPatchAlert alert = new XenServerPatchAlert(p);
            alert.IncludeHosts(new List<Host>() { hostA.Object, hostB.Object });

            IUnitTestVerifier validator = new VerifyGetters(alert);

            validator.Verify(new AlertClassUnitTestData
            {
                AppliesTo = "HostAName, HostBName",
                FixLinkText = "Go to Web Page",
                HelpID = "XenServerPatchAlert",
                Description = "My description",
                HelpLinkText = "Help",
                Title = "New Update Available - name",
                Priority = "Priority1"
            });

            Assert.IsFalse(alert.CanIgnore);

            VerifyConnExpectations(Times.Never);
            VerifyHostsExpectations(Times.Once);
        }

        [Test]
        public void TestAlertWithConnectionAndNoHosts()
        {
            XenServerPatch p = new XenServerPatch("uuid", "name", "My description", "guidance", "6.0.1", "http://url", "http://patchUrl", new DateTime(2011, 4, 1).ToString(), "0", "");
            XenServerPatchAlert alert = new XenServerPatchAlert(p);
            alert.IncludeConnection(connA.Object);
            alert.IncludeConnection(connB.Object);

            IUnitTestVerifier validator = new VerifyGetters(alert);

            validator.Verify(new AlertClassUnitTestData
            {
                AppliesTo = "ConnAName, ConnBName",
                FixLinkText = "Go to Web Page",
                HelpID = "XenServerPatchAlert",
                Description = "My description",
                HelpLinkText = "Help",
                Title = "New Update Available - name",
                Priority = "Unknown"
            });

            Assert.IsFalse(alert.CanIgnore);

            VerifyConnExpectations(Times.Once);
            VerifyHostsExpectations(Times.Never);
        }

        [Test]
        public void TestAlertWithNoConnectionAndNoHosts()
        {
            XenServerPatch p = new XenServerPatch("uuid", "name", "My description", "guidance", "6.0.1", "http://url", "http://patchUrl", new DateTime(2011, 4, 1).ToString(), "5", "");
            XenServerPatchAlert alert = new XenServerPatchAlert(p);

            IUnitTestVerifier validator = new VerifyGetters(alert);

            validator.Verify(new AlertClassUnitTestData
            {
                AppliesTo = string.Empty,
                FixLinkText = "Go to Web Page",
                HelpID = "XenServerPatchAlert",
                Description = "My description",
                HelpLinkText = "Help",
                Title = "New Update Available - name",
                Priority = "Priority5"
            });

            Assert.IsTrue(alert.CanIgnore);

            VerifyConnExpectations(Times.Never);
            VerifyHostsExpectations(Times.Never);
        }

        [Test, ExpectedException(typeof(NullReferenceException))]
        public void TestAlertWithNullPatch()
        {
            XenServerPatchAlert alert = new XenServerPatchAlert(null);
        }

        private void VerifyConnExpectations(Func<Times> times)
        {
            connA.VerifyGet(n => n.Name, times());
            connB.VerifyGet(n => n.Name, times());
        }

        private void VerifyHostsExpectations(Func<Times> times)
        {
            hostA.VerifyGet(n => n.Name, times());
            hostB.VerifyGet(n => n.Name, times());
        }

        [SetUp]
        public void TestSetUp()
        {
            connA = new Mock<IXenConnection>(MockBehavior.Strict);
            connA.Setup(n => n.Name).Returns("ConnAName");

            connB = new Mock<IXenConnection>(MockBehavior.Strict);
            connB.Setup(n => n.Name).Returns("ConnBName");
            
            hostA = new Mock<Host>(MockBehavior.Strict);
            hostA.Setup(n => n.Name).Returns("HostAName");
            hostA.Setup(n => n.Equals(It.IsAny<object>())).Returns((object o) => ReferenceEquals(o, hostA.Object));

            hostB = new Mock<Host>(MockBehavior.Strict);
            hostB.Setup(n => n.Name).Returns("HostBName");
            hostB.Setup(n => n.Equals(It.IsAny<object>())).Returns((object o) => ReferenceEquals(o, hostB.Object));
        }

        [TearDown]
        public void TestTearDown()
        {
            connA = null;
            connB = null;
            hostA = null;
            hostB = null;
        }
    }
}