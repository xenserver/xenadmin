﻿/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAdmin.Actions;
using XenAPI;

namespace XenAdminTests.XenModelTests.ActionTests.Hosts
{
    [TestFixture]
    class GetServerTimeActionTests : ActionUnitTest<GetServerLocalTimeAction>
    {
        private readonly DateTime time = new DateTime(2012, 3, 15, 1, 1, 1);

        protected override GetServerLocalTimeAction NewAction()
        {
            Mock<Host> host = ObjectManager.NewXenObject<Host>(id);
            ObjectManager.MockProxyFor(id).Setup(p =>
                p.host_get_server_localtime(It.IsAny<string>(), It.IsAny<string>())).Returns(new Response<DateTime>(time));
            return new GetServerLocalTimeAction(host.Object);
        }

        protected override bool VerifyResult(GetServerLocalTimeAction action)
        {
            ObjectManager.MockProxyFor(id).Verify(p =>
                p.host_get_server_localtime(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            return action.Result == "2012-03-15 01:01:01Z";
        }
    }
}
