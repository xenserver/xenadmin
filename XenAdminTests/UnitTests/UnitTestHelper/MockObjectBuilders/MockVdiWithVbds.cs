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
    public class MockVdiWithVbds : MockObjectBuilder
    {
        public MockVdiWithVbds(MockObjectManager mom, string connectionId) : base(mom, connectionId)
        {
            NumberOfVbdsToAdd = 1;
        }

        public override Type MockedType
        {
            get { return typeof (VDI); }
        }

        public int NumberOfVbdsToAdd { private get; set; }

        public override Mock BuildObject()
        {
            Mock<VDI> vdi = ObjectManager.NewXenObject<VDI>(ConnectionId);
            List<XenRef<VBD>> vbdRefs = Enumerable.Repeat(new XenRef<VBD>("opaqueref"), NumberOfVbdsToAdd).ToList();
            vdi.Setup(v => v.VBDs).Returns(vbdRefs);
            return vdi;
        }
    }
}
