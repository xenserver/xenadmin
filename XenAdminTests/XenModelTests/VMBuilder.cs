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
using XenAdmin.Network;
using XenAPI;

namespace XenAdminTests.XenModelTests
{
    class MockVMBuilder
    {
        private readonly VM _vm = null;
        private readonly Mock<IXenConnection> _mockConnection;
        public VM VM { get { return _vm; } }


        public MockVMBuilder(string opaqueref, Mock<IXenConnection> mockConnection)
        {
            _mockConnection = mockConnection;
            _vm = new VM { Connection = mockConnection.Object, opaque_ref = opaqueref, VBDs = new List<XenRef<VBD>>() };
            _mockConnection.Setup(x => x.ResolveAll(It.IsAny<IEnumerable<XenRef<VBD>>>()))
               .Returns(_vbds);
        }


        private readonly List<VBD> _vbds = new List<VBD>();
        public void AddDisk(string opaqueref, bool owner, bool isShared)
        {
            VM.VBDs.Add(new XenRef<VBD>(opaqueref));

            _mockConnection.Setup(x => x.Resolve<VDI>(new XenRef<VDI>(opaqueref))).Returns(new VDI()
            {
                opaque_ref = opaqueref,
                Connection = _mockConnection.Object,
                SR = new XenRef<SR>(opaqueref),
                managed = true,
                missing = false
            });
            VBD vbd = new VBD
                          {
                              opaque_ref = opaqueref,
                              VDI = new XenRef<VDI>(opaqueref),
                              other_config = new Dictionary<string, string>(),
                              Connection = _mockConnection.Object,
                              IsOwner = owner
                          };
            _vbds.Add(vbd);
            _mockConnection.Setup(x => x.Resolve<VBD>(new XenRef<VBD>(opaqueref))).Returns(vbd);
            _mockConnection.Setup(x => x.Resolve<SR>(new XenRef<SR>(opaqueref))).Returns(new SR()
            {
                Connection = _mockConnection.Object,
                shared = isShared,
                PBDs = new List<XenRef<PBD>>() { new XenRef<PBD>(opaqueref) }
            });
            _mockConnection.Setup(x => x.Resolve<PBD>(It.IsAny<XenRef<PBD>>())).Returns(new PBD());
        }
    }
}
