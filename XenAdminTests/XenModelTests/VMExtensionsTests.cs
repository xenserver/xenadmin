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

using Moq;
using NUnit.Framework;
using XenAPI;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class VMExtensionsTests:ActionTestBase
    {
        #region GetStorageHost
        [Test]
        public void TestGetStorageHost()
        {
            //Check that if the first vbd is connected to share storage but the rest are connected to localstorage it returns a host vm.GetStorageHost(false, false)

            var vmMock = new MockVMBuilder("testvm", mockConnection);
            vmMock.AddDisk("1",true,true);
            vmMock.AddDisk("2",true,false);
            VM vm = vmMock.VM;
            mockConnection.Setup(x => x.Resolve<Host>(It.IsAny<XenRef<Host>>())).Returns(new Host()).Verifiable();
            Assert.NotNull(vm.GetStorageHost(false));
            mockConnection.Verify();
        }

        [Test]
        public void TestGetStorageHostBothLocal()
        {
            //Check that if that if it does not have share storage it returns a host

            var vmMock = new MockVMBuilder("testvm", mockConnection);
            vmMock.AddDisk("1", true, false);
            vmMock.AddDisk("2", true, false);
            VM vm = vmMock.VM; mockConnection.Setup(x => x.Resolve<Host>(It.IsAny<XenRef<Host>>())).Returns(new Host()).Verifiable();
            Assert.NotNull(vm.GetStorageHost(false));
            mockConnection.Verify();
        }

        [Test]
        public void TestGetStorageHostBothSRShared()
        {
            //Check that if that if both are shared it returns null

            var vmMock = new MockVMBuilder("testvm", mockConnection);
            vmMock.AddDisk("1", true, true);
            vmMock.AddDisk("2", true, true);
            VM vm = vmMock.VM;
            mockConnection.Setup(x => x.Resolve<Host>(It.IsAny<XenRef<Host>>())).Returns(new Host());
            Assert.Null(vm.GetStorageHost(false));
        }
        #endregion
    }
}
