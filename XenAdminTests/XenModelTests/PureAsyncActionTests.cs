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
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using XenAdmin.Actions;
using XenAdmin.Actions.VMActions;
using XenAdmin.Core;
using XenAPI;
using System.Threading;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.UICategoryB), Category(TestCategories.SmokeTest)]
    public class PureAsyncActionTests : ActionTestBase
    {
        readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        [Test]
        public void VMTestPureAsyncAction()
        {
            mockProxy.Setup(x => x.vmpp_archive_now(It.IsAny<string>(), "testvm")).Returns(new Response<string>(""));
            mockProxy.Setup(x => x.task_add_to_other_config(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Response<string>(""));
            mockProxy.Setup(x => x.task_remove_from_other_config(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Response<string>(""));
            VM vm = GetVM();
            SetupPureAsyncAction(vm);
            //Async call pure async action extra setups needed
            ExtraAsyncMethodsSetup();
            var action = new ArchiveNowAction(vm);
            action.Completed += action_Completed;
            action.RunAsync();
            _autoResetEvent.WaitOne();
            Assert.True(action.Succeeded);
            mockProxy.VerifyAll();

        }

        private void ExtraAsyncMethodsSetup()
        {
            mockProxy.Setup(
                x =>
                x.task_add_to_other_config(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                           It.IsAny<string>())).Returns(new Response<string>(string.Empty));
            mockProxy.Setup(
                x =>
                x.task_remove_from_other_config(It.IsAny<string>(), It.IsAny<string>(),
                                           It.IsAny<string>())).Returns(new Response<string>(string.Empty));
            mockProxy.Setup(x => x.task_get_record(It.IsAny<string>(), It.IsAny<string>())).Returns(
                new Response<Proxy_Task>(new Proxy_Task()
                                             {
                                                 progress = 100,
                                                 status = XenAPI.task_status_type.success.ToString(),
                                                 result = ""
                                             }));
        }

        [Test]
        public void DeleteSnapshotPureAsyncActionTest()
        {

            mockProxy.Setup(x => x.vm_destroy(It.IsAny<string>(), "testvm")).Returns(new Response<string>(""));
            VM vm = GetVM();
            SetupPureAsyncAction(vm);
            vm.power_state = vm_power_state.Halted;
            var action = new VMSnapshotDeleteAction(vm);
            action.Completed += action_Completed;
            action.RunAsync();
            _autoResetEvent.WaitOne();
            Assert.True(action.Succeeded);
            mockProxy.VerifyAll();
        }

        [Test]
        public void ShutdownVMPureAsyncActionTest()
        {
            mockProxy.Setup(x => x.async_vm_clean_shutdown(It.IsAny<string>(), It.IsAny<string>())).Returns(new Response<string>(""));
            mockProxy.Setup(x => x.task_add_to_other_config(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Response<string>(""));
            mockProxy.Setup(x => x.task_remove_from_other_config(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Response<string>(""));
            VM vm = GetVM();
            SetupPureAsyncAction(vm);
            ExtraAsyncMethodsSetup();
            vm.power_state = vm_power_state.Halted;
            var action = new VMCleanShutdown(vm);
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
        private void SetupPureAsyncAction(VM vm)
        {
            
            mockProxy.Setup(x => x.auth_get_subject_information_from_identifier(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Response<object>(new Hashtable()));
            mockProxy.Setup(x => x.auth_get_group_membership(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Response<string[]>(new string[] { }));
            //Config roles
            Role role = new Role() { name_label = "pool-admin", subroles = new List<XenRef<Role>>() { new XenRef<Role>("2") } };
            cache.UpdateFrom(mockConnection.Object,
                             new List<ObjectChange>() { new ObjectChange(role.GetType(), "1", role) });

            mockConnection.Setup(x => x.Cache).Returns(cache);
            mockConnection.Setup(x => x.ResolveAll(It.IsAny<IEnumerable<XenRef<Role>>>()))
                .Returns(new List<Role>() { 
                    new Role() { name_label = "vmpp.archive_now" }
                    ,new Role() { name_label = "vm.destroy" }
                    ,new Role() { name_label = "vm.clean_shutdown" }
                    , new Role() { name_label = "task.add_to_other_config/key:xencenteruuid" }
                    ,new Role(){name_label ="task.add_to_other_config/key:applies_to" }});
            mockConnection.Setup(x => x.HostnameWithPort).Returns("");
            var sessionMock = new Mock<Session>(mockProxy.Object, mockConnection.Object);
            sessionMock.Setup(x => x.IsLocalSuperuser).Returns(false);
            UserDetails.UpdateDetails("user1", sessionMock.Object);
            sessionMock.Setup(x => x.CurrentUserDetails).Returns(UserDetails.Sid_To_UserDetails["user1"]);
            mockConnection.Setup(x => x.Session).Returns(sessionMock.Object);

            TestXenAdminConfigProvider.MockSession = vm.Connection.Session;

        }

        private VM GetVM()
        {
            return new MockVMBuilder("testvm", mockConnection).VM;
        }


    }
}