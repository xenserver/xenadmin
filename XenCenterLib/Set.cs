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

namespace XenAdmin.Core
{

    //[Obsolete("Don't use anymore", false)]
    public class Set<T> : HashSet<T>, IEquatable<Set<T>> where T : IComparable
    {

        public Set()
        {

        }

        public Set(IEnumerable<T> keys)
        {

            foreach (T t in keys)
            {
                if (!this.Contains(t))
                {
                    this.Add(t);
                }
            }

        }

        public Set(params T[] ts)
        {
            foreach (T t in ts)
            {

                Add(t);
            }
        }

        public override bool Equals(object obj)
        {
            var set = obj as Set<T>;
            if (set != null)
                return Equals(set);
            return false;
        }

        public bool Equals(Set<T> set)
        {
            return set.IsSubsetOf(this) && this.IsSubsetOf(set);
        }

        public override int GetHashCode()
        {
            var enumerator = GetEnumerator();
            if (enumerator.MoveNext())
                return enumerator.Current.GetHashCode();
            return 0;
        }

        public override String ToString()
        {
            String result = "";

            foreach (T t in this)
            {
                result += t.ToString();
                result += " ";
            }

            return result;
        }
    }
}
