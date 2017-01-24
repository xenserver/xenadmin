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
using Moq;
using XenAPI;

namespace XenAdminTests.UnitTests.UnitTestHelper.MockObjectBuilders
{
    public enum ObjectBuilderType
    {
        VmWithHomeServerHost,
        VdiWithVbds,
        ClearwaterHost,
        TampaHost,
        SanibelHost, 
        BostonHost,
        CowleyHost,
        FloodgateHost,
        OrlandoHost,
        RioHost,
        PoolOfTwoClearwaterHosts
    }

    public class MockObjectBuilderFactory
    {
        private readonly MockObjectManager objectManager;
        public MockObjectBuilderFactory(IObjectManager objectManager)
        {
            this.objectManager = objectManager as MockObjectManager;
        }

        public MockObjectBuilder Builder(ObjectBuilderType builderType, string connectionId)
        {
            MockObjectBuilder builder = null;
            switch(builderType)
            {
                case ObjectBuilderType.VmWithHomeServerHost:
                    builder = new MockVmWithHomeServer(objectManager, connectionId); 
                    break;
                case ObjectBuilderType.VdiWithVbds:
                    builder = new MockVdiWithVbds(objectManager, connectionId);
                    break;
                case ObjectBuilderType.ClearwaterHost:
                    builder = new MockVersionedHost(objectManager, connectionId, ClearwaterPlatformVersion, ClearwaterProductVersion);
                    break;
                case ObjectBuilderType.TampaHost:
                    builder = new MockVersionedHost(objectManager, connectionId, "1.6.10", "6.1.0");
                    break;
                case ObjectBuilderType.SanibelHost:
                    builder = new MockVersionedHost(objectManager, connectionId, "1.0.99", "6.0.2");
                    break;
                case ObjectBuilderType.BostonHost:
                    builder = new MockVersionedHost(objectManager, connectionId, "1.0.99", "6.0.0");
                    break;
                case ObjectBuilderType.CowleyHost:
                    builder = new MockVersionedHost(objectManager, connectionId, String.Empty, "5.6.100");
                    break;
                case ObjectBuilderType.FloodgateHost:
                case ObjectBuilderType.OrlandoHost:
                    builder = new MockVersionedHost(objectManager, connectionId, String.Empty, "5.0.0");
                    break;
                case ObjectBuilderType.RioHost:
                    builder = new MockVersionedHost(objectManager, connectionId, String.Empty, "4.0.0");
                    break;
                case ObjectBuilderType.PoolOfTwoClearwaterHosts:
                    builder = new MockPoolOfVersionedHosts(objectManager, connectionId, ClearwaterPlatformVersion, ClearwaterProductVersion, 2);
                    break;
            }
            return builder;
        }

        private const string ClearwaterPlatformVersion = "1.7";
        private const string ClearwaterProductVersion = "6.2.0";

        public Mock<T> BuiltObject<T>(ObjectBuilderType builderType, string connectionId) where T : XenObject<T>
        {
            MockObjectBuilder builder = Builder(builderType, connectionId);
            if (builder.MockedType == typeof(T))
                return builder.BuildObject() as Mock<T>;
            
            throw new ArgumentException("Type mismatch between template and builder output");
        }

    }
}
