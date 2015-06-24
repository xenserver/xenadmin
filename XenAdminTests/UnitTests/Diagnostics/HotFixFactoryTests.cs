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
using Moq;
using NUnit.Framework;
using XenAdmin.Diagnostics.Hotfixing;
using XenAPI;

namespace XenAdminTests.UnitTests.Diagnostics
{
    [TestFixture, Category(TestCategories.SmokeTest)]
    public class HotFixFactoryTests : UnitTester_TestFixture
    {
        private const string id = "test";

        public HotFixFactoryTests() : base(id){}

        private readonly HotfixFactory factory = new HotfixFactory();

        [TearDown]
        public void TearDownPerTest()
        {
            ObjectManager.ClearXenObjects(id);
            ObjectManager.RefreshCache(id);
        }

        [Test]
        public void HotfixableServerVersionHasExpectedMembers()
        {
            string[] enumNames = Enum.GetNames(typeof (HotfixFactory.HotfixableServerVersion));
            Array.Sort(enumNames);

            string[] expectedNames = new []{"Cowley", "MNR", "Boston", "SanibelToClearwater", "Creedence"};
            Array.Sort(expectedNames);

            CollectionAssert.AreEqual(expectedNames, enumNames, "Expected contents of HotfixableServerVersion enum");
        }

        [Test]
        public void UUIDLookedUpFromEnum()
        {
            Assert.AreEqual("95ac709c-e408-423f-8d22-84b8134a149e;b412a910-0453-42ed-bae0-982cc48b00d6", 
                            factory.Hotfix(HotfixFactory.HotfixableServerVersion.Boston).UUID,
                            "Boston UUID lookup from enum");

            Assert.AreEqual("ca0ca2c6-cc96-4e4b-946b-39ebe6652fd6", 
                            factory.Hotfix(HotfixFactory.HotfixableServerVersion.Cowley).UUID,
                            "Cowley UUID lookup from enum");

            Assert.AreEqual("e2cb047b-66ed-4fa0-882a-67ff1726f4b9", 
                            factory.Hotfix(HotfixFactory.HotfixableServerVersion.MNR).UUID,
                            "MNR UUID lookup from enum");

            Assert.AreEqual("b412a910-0453-42ed-bae0-982cc48b00d6", 
                            factory.Hotfix(HotfixFactory.HotfixableServerVersion.SanibelToClearwater).UUID,
                            "SanibelToClearwater UUID lookup from enum");

            Assert.AreEqual("0c8accf4-060f-47fb-85a1-3470f815fd88",
                            factory.Hotfix(HotfixFactory.HotfixableServerVersion.Creedence).UUID,
                            "Creedence UUID lookup from enum");
        }

        [Test]
        public void FilenameLookedUpFromEnum()
        {
            Assert.AreEqual("XS60E001.xsupdate;XS62E006.xsupdate",
                            factory.Hotfix(HotfixFactory.HotfixableServerVersion.Boston).Filename,
                            "Boston Filename lookup from enum");

            Assert.AreEqual("XS56EFP1002.xsupdate",
                            factory.Hotfix(HotfixFactory.HotfixableServerVersion.Cowley).Filename,
                            "Cowley Filename lookup from enum");

            Assert.AreEqual("XS56E008.xsupdate",
                            factory.Hotfix(HotfixFactory.HotfixableServerVersion.MNR).Filename,
                            "MNR Filename lookup from enum");

            Assert.AreEqual("XS62E006.xsupdate",
                            factory.Hotfix(HotfixFactory.HotfixableServerVersion.SanibelToClearwater).Filename,
                            "SanibelToClearwater Filename lookup from enum");

            Assert.AreEqual("XS65ESP1006.xsupdate",
                            factory.Hotfix(HotfixFactory.HotfixableServerVersion.Creedence).Filename,
                            "Creedence Filename lookup from enum");
        }

        [Test]
        [TestCase("2.0.0", Description = "Dundee")]
        [TestCase("9999.9999.9999", Description = "Future")]
        public void TestPlatformVersionNumbersDundeeOrGreaterGiveNulls(string platformVersion)
        {
            Mock<Host> host = ObjectManager.NewXenObject<Host>(id);
            host.Setup(h => h.PlatformVersion).Returns(platformVersion);
            Assert.IsNull(factory.Hotfix(host.Object));
        }

        [Test]
        [TestCase("6.0.2", "XS62E006.xsupdate", Description = "Sanibel")]
        [TestCase("6.0.0", "XS60E001.xsupdate;XS62E006.xsupdate", Description = "Boston")]
        [TestCase("5.6.100", "XS56EFP1002.xsupdate", Description = "Cowley")]
        [TestCase("5.6.0", "XS56E008.xsupdate", Description = "MNR")]
        public void TestProductVersionNumbersWithHotfixes(string productVersion, string filenames)
        {
            Mock<Host> host = ObjectManager.NewXenObject<Host>(id);
            host.Setup(h => h.ProductVersion).Returns(productVersion);
            Assert.That(filenames, Is.EqualTo(factory.Hotfix(host.Object).Filename));
        }

        [Test]
        [TestCase("2.0.0", Description = "Dundee", Result = false)]
        [TestCase("1.9.0", Description = "Creedence", Result = true)]
        [TestCase("1.8.0", Description = "Clearwater", Result = true)]
        [TestCase("1.6.10", Description = "Tampa", Result = true)]
        [TestCase("9999.9999.9999", Description = "Future", Result = false)]
        public bool TestIsHotfixRequiredBasedOnPlatformVersion(string version)
        {
            Mock<Host> host = ObjectManager.NewXenObject<Host>(id);
            host.Setup(h => h.PlatformVersion).Returns(version);
            return factory.IsHotfixRequired(host.Object);
        }

        [Test]
        [TestCase("6.0.2", Description = "Sanibel",  Result = true)]
        [TestCase("6.0.0", Description = "Boston", Result = true)]
        [TestCase("5.6.100", Description = "Cowley", Result = true)]
        [TestCase("5.6.0", Description = "MNR", Result = true)]
        [TestCase("3.0.0", Description = "Burbank", Result = false)]
        public bool TestIsHotfixRequiredBasedOnProductVersion(string productVersion)
        {
            Mock<Host> host = ObjectManager.NewXenObject<Host>(id);
            host.Setup(h => h.ProductVersion).Returns(productVersion);
            return factory.IsHotfixRequired(host.Object);
        }

    }
}