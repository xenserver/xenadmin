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
using Moq;
using NUnit.Framework;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;

namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    internal class RegexTests
    {
        private Mock<VM> _vm;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            var connection = new Mock<IXenConnection>(MockBehavior.Loose);
            var cache = new Cache();
            connection.Setup(x => x.Cache).Returns(cache);

            string vbdAref = "OpaqueRef:08975384-b37c-49c8-a10b-af5790d1bb2f";
            var vbdA = new Mock<VBD>(MockBehavior.Strict);
            vbdA.Setup(v => v.device).Returns("xvda");
            vbdA.Setup(v => v.userdevice).Returns("0");

            string vbdBref = "OpaqueRef:707c5aa5-4f89-432e-aa12-194e345a1053";
            var vbdB = new Mock<VBD>(MockBehavior.Strict);
            vbdB.Setup(v => v.device).Returns("xvdb");
            vbdB.Setup(v => v.userdevice).Returns("1");

            connection.Setup(c => c.Resolve(new XenRef<VBD>(vbdAref))).Returns(vbdA.Object);
            connection.Setup(c => c.Resolve(new XenRef<VBD>(vbdBref))).Returns(vbdB.Object);

            _vm = new Mock<VM>(MockBehavior.Strict);
            _vm.Object.Connection = connection.Object;
            _vm.Setup(v => v.VBDs).Returns(new List<XenRef<VBD>> {new XenRef<VBD>(vbdAref), new XenRef<VBD>(vbdBref)});
        }


        [Test]
        [TestCase("vbd_xvda_latency", ExpectedResult = "Disk 0 Average I/O Latency")]
        [TestCase("vbd_xvda_read", ExpectedResult = "Disk 0 Read")]
        [TestCase("vbd_xvda_read_latency", ExpectedResult = "Disk 0 Read Latency")]
        [TestCase("vbd_xvda_write", ExpectedResult = "Disk 0 Write")]
        [TestCase("vbd_xvda_write_latency", ExpectedResult = "Disk 0 Write Latency")]
        [TestCase("vbd_xvdb_latency", ExpectedResult = "Disk 1 Average I/O Latency")]
        [TestCase("vbd_xvdb_read", ExpectedResult = "Disk 1 Read")]
        [TestCase("vbd_xvdb_read_latency", ExpectedResult = "Disk 1 Read Latency")]
        [TestCase("vbd_xvdb_write", ExpectedResult = "Disk 1 Write")]
        [TestCase("vbd_xvdb_write_latency", ExpectedResult = "Disk 1 Write Latency")]
        public string TestDiskRegex(string input)
        {
            return Helpers.GetFriendlyDataSourceName(input, _vm.Object);
        }
    }
}
