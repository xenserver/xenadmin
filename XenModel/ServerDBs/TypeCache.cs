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
using System.Reflection;

namespace XenAdmin.ServerDBs
{
    /// <summary>
    /// Class for caching XAPI types by name. Used by <see cref="DbProxy"/>.
    /// </summary>
    internal static class TypeCache
    {
        private static readonly Dictionary<String, TypeCacheEntry> Entries = new Dictionary<string, TypeCacheEntry>();
        private static readonly object Lck = new object();

        private static TypeCacheEntry GetEntry(string className)
        {
            Util.ThrowIfStringParameterNullOrEmpty(className, "className");

            TypeCacheEntry entry;
            lock (Lck)
            {
                if (!Entries.TryGetValue(className, out entry))
                {
                    entry = new TypeCacheEntry(className);
                    Entries.Add(className, entry);
                }
            }
            return entry;
        }

        /// <summary>
        /// Gets the XAPI <see cref="Type"/> for the specified name.
        /// (assembly.GetType("XenAPI." + className, true, true))
        /// </summary>
        /// <param name="className">The name for which the XAPI <see cref="Type"/> is required.</param>
        /// <returns>The XAPI <see cref="Type"/> for the specified name.</returns>
        public static Type GetClassType(string className)
        {
            return GetEntry(className).ClassType;
        }

        /// <summary>
        /// Gets the XAPI Proxy <see cref="Type"/> for the specified name.
        /// (assembly.GetType("XenAPI.Proxy_" + className, true, true))
        /// </summary>
        /// <param name="className">The name for which the XAPI <see cref="Type"/> is required.</param>
        /// <returns>The XAPI <see cref="Type"/> for the specified name.</returns>
        public static Type GetProxyType(string className)
        {
            return GetEntry(className).ProxyType;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the field of the specified name on the specified class.
        /// </summary>
        /// <param name="className">Name of the class on which the field resides.</param>
        /// <param name="fieldName">Name of the field for which the XPI <see cref="Type"/> is required.</param>
        /// <returns>The <see cref="Type"/> of the field of the specified name on the specified class.</returns>
        public static Type GetFieldType(string className, string fieldName)
        {
            Util.ThrowIfStringParameterNullOrEmpty(fieldName, "fieldName");
            
            return GetEntry(className).GetFieldType(fieldName);
        }

        private class TypeCacheEntry
        {
            private readonly Dictionary<string, Type> _fieldTypes = new Dictionary<string, Type>();
            public readonly Type ProxyType;
            public readonly Type ClassType;
            private readonly object _lck = new object();

            public TypeCacheEntry(string className)
            {
                Assembly assembly = Assembly.Load(new AssemblyName("XenModel"));
                ProxyType = assembly.GetType("XenAPI.Proxy_" + className, true, true);
                ClassType = assembly.GetType("XenAPI." + className, true, true);
            }

            public Type GetFieldType(string fieldName)
            {
                Type output;
                lock (_lck)
                {
                    if (!_fieldTypes.TryGetValue(fieldName, out output))
                    {
                        output = null;
                        FieldInfo fi = ProxyType.GetField(fieldName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                        if (fi != null)
                        {
                            output = fi.FieldType;
                            _fieldTypes.Add(fieldName, output);
                        }
                    }
                }
                return output;
            }
        }
    }
}
