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
using System.Text;
using XenAPI;

namespace XenAdmin.Core
{
    public class ObjectChange
    {
        public readonly Type type;
        public readonly String xenref;
        public readonly object value;

        public ObjectChange(Type type, string xenref, object value)
        {
            this.type = type;
            this.xenref = xenref;
            this.value = value;
        }

        /// <summary>
        /// Looks in a list of ObjectChanges and returns the first one that pertains to a Pool object.
        /// </summary>
        /// <param name="changes"></param>
        /// <param name="OpaqueRef"></param>
        /// <returns></returns>
        public static Pool GetPool(IEnumerable<ObjectChange> changes, out string OpaqueRef)
        {
            foreach (ObjectChange change in changes)
            {
                if (change.type == typeof(Pool))
                {
                    OpaqueRef = change.xenref;
                    return (Pool)change.value;
                }
            }

            OpaqueRef = "";
            return null;
        }

        public static Host GetMaster(IEnumerable<ObjectChange> changes)
        {
            String _;
            Pool pool = GetPool(changes, out _);
            if (pool == null)
                return null;

            foreach (ObjectChange change in changes)
            {
                if (change.type != typeof(Host))
                    continue;

                if (change.xenref != pool.master.opaque_ref)
                    continue;

                return change.value as Host;
            }

            return null;
        }

        /// <summary>
        /// Looks in a list of ObjectChanges and returns any pertaining to Host objects.
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        public static List<Host> GetHosts(IEnumerable<ObjectChange> changes)
        {
            return Filter<Host>(changes);
        }

        public static List<T> Filter<T>(IEnumerable<ObjectChange> changes) where T : XenObject<T>
        {
            List<T> filtered = new List<T>();

            foreach (ObjectChange change in changes)
            {
                if (change.type == typeof(T))
                {
                    filtered.Add((T)change.value);
                }
            }

            return filtered;
        }
    }
}
