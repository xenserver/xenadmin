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
using XenAdmin.Actions.VMActions;
using XenAPI;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    class DestroyVMTests:ActionTestBase
    {

        [Test]
        //Test that if I pass a vm and delete no disks or snapshots the action deletes the vm
        public void VMDeleteActionTest()
        {
            mockProxy.Setup(x => x.vm_destroy(It.IsAny<string>(), "testvm")).Returns(new Response<string>(""));
            VM vm = GetVM();
            var action = new VMDestroyAction(vm, new List<VBD>(), new List<VM>());
            action.RunExternal(new Session(mockProxy.Object, vm.Connection));
            Assert.True(action.Succeeded);
            mockProxy.VerifyAll();
        }

        private VM GetVM()
        {
            var vmbuilder = new MockVMBuilder("testvm", mockConnection);
            vmbuilder.AddDisk("1", true,false);
            vmbuilder.AddDisk("2", false,false);
            return vmbuilder.VM;
        }
        [Test]
        //Test that if I pass a vm and delete all its disk it deletes all of them
        public void VMDeleteActionDeleteTwoTest()
        {
            mockProxy.Setup(x => x.vm_destroy(It.IsAny<string>(), "testvm")).Returns(new Response<string>(""));
            mockProxy.Setup(x => x.vdi_destroy(It.IsAny<string>(), It.IsAny<string>())).Returns(new Response<string>(""));
            VM vm = GetVM();
            var action = new VMDestroyAction(vm, vm.Connection.ResolveAll(vm.VBDs), new List<VM>());
            action.RunExternal(new Session(mockProxy.Object, vm.Connection));
            Assert.True(action.Succeeded);
            mockProxy.VerifyAll();
            mockProxy.Verify(x => x.vdi_destroy(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        //Test that if the DestroyVM static method used by VMWizard or Delete snapshot it only deletes owner==true actions
        public void VMDeleteActionDeleteOneTest()
        {
            mockProxy.Setup(x => x.vm_destroy(It.IsAny<string>(), "testvm")).Returns(new Response<string>(""));
            mockProxy.Setup(x => x.vdi_destroy(It.IsAny<string>(), "1")).Returns(new Response<string>(""));
            VM vm = GetVM();
            VMDestroyAction.DestroyVM(new Session(mockProxy.Object, vm.Connection), vm, true);
            mockProxy.VerifyAll();
            mockProxy.Verify(x => x.vdi_destroy(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }
    }
}
