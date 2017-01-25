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
using System.Linq;
using System.Text;
using Moq;
using XenAPI;

namespace XenAdminTests.UnitTests.UnitTestHelper.MockObjectBuilders
{
    public class MockPoolOfVersionedHosts : MockObjectBuilder
    {
        private string PlatformVersion { get; set; }
        private string ProductVersion { get; set; }
        private int HostsInPool{ get; set;}
        private const string masterRef = "masterOpaqueRef";
        private const string slaveOpaqueRefStem = "slaveOpaqueRef";

        public MockPoolOfVersionedHosts(MockObjectManager manager, string connectionId, string platformVersion, string productVersion, int hostsInPool)
            : base(manager, connectionId)
        {
            PlatformVersion = platformVersion;
            ProductVersion = productVersion;
            HostsInPool = hostsInPool;
        }

        public override Type MockedType
        {
            get { return typeof (Pool); }
        }

        public override Mock BuildObject()
        {
            Mock<Pool> pool = ObjectManager.NewXenObject<Pool>(ConnectionId);
            Mock<Host> master = CreateHost();
            master.Object.opaque_ref = masterRef;
            ObjectManager.MockConnectionFor(ConnectionId).Setup(c => c.Resolve(new XenRef<Host>(masterRef))).Returns(
                master.Object);
            pool.Setup(p => p.master).Returns(new XenRef<Host>(masterRef));

            for (int i = 0; i < HostsInPool - 1; i++ )
            {
                Mock<Host> host = ObjectManager.NewXenObject<Host>(ConnectionId);
                host.Object.opaque_ref = String.Concat(slaveOpaqueRefStem, i);
            }

            return pool;
        }

        private Mock<Host> CreateHost()
        {
            Mock<Host> host = ObjectManager.NewXenObject<Host>(ConnectionId);
            host.Setup(h => h.ProductVersion).Returns(ProductVersion);
            host.Setup(h => h.PlatformVersion).Returns(PlatformVersion);
            return host;
        }

    }
}
