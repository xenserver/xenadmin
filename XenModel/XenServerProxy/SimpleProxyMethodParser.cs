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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using XenAdmin.ServerDBs;

namespace XenAdmin.Core
{
    public static class SimpleProxyMethodParser
    {
        public static ProxyMethodInfo ParseTypeAndNameOnly(string proxyMethodName)
        {
            string typeName = GetTypeName(proxyMethodName);
            MethodType fakeMethodType = proxyMethodName.StartsWith(String.Format("{0}_", typeName)) ? MethodType.Sync : MethodType.Async;
            string methodName = fakeMethodType == MethodType.Sync ? proxyMethodName.Substring(typeName.Length + 1) : proxyMethodName.Substring(7 + typeName.Length);
            return new ProxyMethodInfo(methodName, typeName);
        }

        private static string GetTypeName(string proxyMethodName)
        {
            // This only works because DbProxy.AllTypes has longest names first,
            // so we never accidentally match a substring.
            foreach (string type in AllTypes)
            {
                if (proxyMethodName.StartsWith(string.Format("{0}_", type)) || proxyMethodName.StartsWith(string.Format("async_{0}_", type)))
                {
                    return type;
                }
            }
            return null;
        }
        // Find all XenObject types. Actually the ones we want are those with
        // associated Proxy_ types (basically, the ones in the public API).
        private static readonly object XenApiTypesLock = new object();
        private static ReadOnlyCollection<string> _xenApiTypeNames;
        public static ReadOnlyCollection<string> AllTypes
        {
            get
            {
                lock (XenApiTypesLock)
                {
                    if (_xenApiTypeNames == null)
                    {
                        List<string> names = new List<string>();

                        foreach (Type type in Assembly.Load(new AssemblyName("XenModel")).GetTypes())
                        {
                            if (type.Namespace == "XenAPI" && type.Name.StartsWith("Proxy_"))
                                names.Add(type.Name.Substring(6).ToLower());
                        }

                        // We sort the names in reverse alphabetical order, so that later in
                        // ProxyMethodNameParser.GetTypeName(), we find the longest matching name.
                        names.Sort((x, y) => -x.CompareTo(y));
                        _xenApiTypeNames = new ReadOnlyCollection<string>(names);
                    }
                    return _xenApiTypeNames;
                }
            }
        }
    }
}