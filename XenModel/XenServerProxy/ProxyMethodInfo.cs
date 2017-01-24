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
using System.Reflection;

namespace XenAdmin.ServerDBs
{

    public enum MethodType { Sync, Async, Unknown }
    public class ProxyMethodInfo
    {
        public readonly string TypeName;
        public readonly string MethodName;

        public readonly MethodType FakeMethodType;
        public readonly MethodInfo FakeMethod;
        public readonly Type FakeType;

        public ProxyMethodInfo(string methodName)
        {
            Util.ThrowIfStringParameterNullOrEmpty(methodName, "methodName");
            MethodName = methodName;
        }

        public ProxyMethodInfo(string methodName, string typeName)
            : this(methodName)
        {
            TypeName = typeName;
        }

        public ProxyMethodInfo(string methodName, string typeName, Type fakeType, MethodInfo fakeMethod, MethodType fakeMethodType)
            : this(methodName, typeName)
        {
            FakeMethodType = fakeMethodType;
            FakeMethod = fakeMethod;
            FakeType = fakeType;
        }
    }
}