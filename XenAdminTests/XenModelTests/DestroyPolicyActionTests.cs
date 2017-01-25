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
using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using XenAdmin.Actions;
using XenAPI;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class DestroyPolicyActionTests:ActionTestBase
    {
        private readonly AutoResetEvent _autoResetEvent=new AutoResetEvent(false);

        [Test]
        public void TestEmptyList()
        {
            var action = new DestroyPolicyAction<VMPP>(mockConnection.Object, new List<IVMPolicy>());
            action.Completed += action_Completed;
            action.RunAsync();
            _autoResetEvent.WaitOne();
            Assert.True(action.Succeeded);
            mockProxy.VerifyAll();
        }
        void action_Completed(ActionBase sender)
        {
            _autoResetEvent.Set();
        }

        [Test]
        public void TestOneInList()
        {
            mockProxy.Setup(x => x.vmpp_destroy(It.IsAny<string>(), "1")).Returns(new Response<string>(""));
            mockProxy.Setup(x => x.vm_set_protection_policy(It.IsAny<string>(), "1", It.IsAny<string>())).Returns(new Response<string>(""));
            var action = new DestroyPolicyAction<VMPP>(mockConnection.Object, new List<IVMPolicy>()
            {new VMPP(){opaque_ref = "1",VMs = new List<XenRef<VM>>(){new XenRef<VM>("1")}}});
            action.Completed += action_Completed;
            action.RunAsync();
            _autoResetEvent.WaitOne();
            Assert.True(action.Succeeded,action.Exception!=null?action.Exception.ToString():"");
            mockProxy.VerifyAll();
            mockProxy.Verify(x => x.vm_set_protection_policy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void TestOneWithTwonVMsInList()
        {
            mockProxy.Setup(x => x.vmpp_destroy(It.IsAny<string>(),"1")).Returns(new Response<string>(""));
            mockProxy.Setup(x => x.vm_set_protection_policy(It.IsAny<string>(),  It.Is<string>(s=>s=="1"||s=="2"), It.IsAny<string>())).Returns(new Response<string>(""));
            var action = new DestroyPolicyAction<VMPP>(mockConnection.Object, new List<IVMPolicy>() 
            { new VMPP() { opaque_ref = "1", VMs = new List<XenRef<VM>>() { new XenRef<VM>("1"), new XenRef<VM>("2") } } });
            action.Completed += action_Completed;
            action.RunAsync();
            _autoResetEvent.WaitOne();
            Assert.True(action.Succeeded, action.Exception != null ? action.Exception.ToString() : "");
            mockProxy.Verify(x => x.vm_set_protection_policy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockProxy.VerifyAll();
        }
       
    }
}
