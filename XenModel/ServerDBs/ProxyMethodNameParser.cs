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
using XenAdmin.Core;
using XenAdmin.ServerDBs.FakeAPI;

namespace XenAdmin.ServerDBs
{

    internal static class ProxyMethodNameParser
    {
        public static ProxyMethodInfo Parse(String proxyMethodName)
        {
            Util.ThrowIfStringParameterNullOrEmpty(proxyMethodName, "proxyMethodName");

            string typeName = GetTypeName(proxyMethodName);

            if (typeName != null)
            {
                MethodType fakeMethodType = proxyMethodName.StartsWith(String.Format("{0}_", typeName)) ? MethodType.Sync : MethodType.Async;
                string methodName = fakeMethodType == MethodType.Sync ? proxyMethodName.Substring(typeName.Length + 1) : proxyMethodName.Substring(7 + typeName.Length);

                Type fakeType = Type.GetType(String.Format("XenAdmin.ServerDBs.FakeAPI.fake{0}", typeName), false, true) ?? typeof(fakeUnknown);
                MethodInfo fakeMethod = fakeType.GetMethod(methodName);

                if (fakeType != null && fakeMethod != null)
                {
                    return new ProxyMethodInfo(methodName, typeName, fakeType, fakeMethod, fakeMethodType);
                }

                return new ProxyMethodInfo(methodName, typeName);
            }
            
            return new ProxyMethodInfo(proxyMethodName);
        }

        private static string GetTypeName(string proxyMethodName)
        {
            // This only works because DbProxy.AllTypes has longest names first,
            // so we never accidentally match a substring.
            foreach (string type in SimpleProxyMethodParser.AllTypes)
            {
                if (proxyMethodName.StartsWith(string.Format("{0}_", type)) || proxyMethodName.StartsWith(string.Format("async_{0}_", type)))
                {
                    return type;
                }
            }
            return null;
        }
    }
}
