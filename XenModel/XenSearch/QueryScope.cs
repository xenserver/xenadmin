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
using System.Xml;
using XenAPI;

namespace XenAdmin.XenSearch
{
    public class QueryScope: IEquatable<QueryScope>
    {
        private ObjectTypes types;

        public QueryScope(ObjectTypes types)
        {
            this.types = types;
        }

        public QueryScope(XmlNode node)
        {
            ObjectTypes t = ObjectTypes.None;

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "NONE" || child.Name.StartsWith("ALL"))
                    continue;

                Object o = Enum.Parse(typeof(ObjectTypes), child.Name);
                if (o != null)
                    t |= (ObjectTypes)o;
            }

            types = t;
        }

        public ObjectTypes ObjectTypes
        {
            get
            {
                return types;
            }
        }

        public bool WantType(IXenObject o)
        {
            ObjectTypes? type = ObjectTypeOf(o);
            return (type == null ? false : WantType(type.Value));
        }

        // Want type t: or if t is a bitwise OR, want *all* types in t
        // I.e., the types "this" includes are a superset of t
        public bool WantType(ObjectTypes t)
        {
            return ((types & t) == t);
        }

        public bool WantType(QueryScope q)
        {
            return (q != null && WantType(q.ObjectTypes));
        }

        // The types "this" includes are a subset of t
        public bool WantSubsetOf(ObjectTypes t)
        {
            return ((types & t) == types);
        }

        public bool WantSubsetOf(QueryScope q)
        {
            return (q != null && WantSubsetOf(q.ObjectTypes));
        }

        // Want any of the types in t: i.e., the types "this" includes
        // overlap with t
        public bool WantAnyOf(ObjectTypes t)
        {
            return ((types & t) != ObjectTypes.None);
        }

        public bool WantAnyOf(QueryScope q)
        {
            return (q != null && WantAnyOf(q.ObjectTypes));
        }

        // Two query scopes want exactly the same types
        public bool Equals(ObjectTypes t)
        {
            return types == t;
        }

        public bool Equals(QueryScope q)
        {
            return (q != null && Equals(q.ObjectTypes));
        }

        public override bool Equals(object obj)
        {
            QueryScope other = obj as QueryScope;
            if (other == null)
                return false;

            return Equals(other);
        }

        private ObjectTypes? ObjectTypeOf(IXenObject o)
        {
            PropertyAccessor pa = PropertyAccessors.Get(PropertyNames.type);
            Object obj = pa(o);
            return (ObjectTypes?)obj;
        }

        public override int GetHashCode()
        {
            return (int)types;
        }
 
        #region Marshalling

        public virtual XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(SearchMarshalling.GetClassName(this));

            foreach (ObjectTypes t in Enum.GetValues(typeof(ObjectTypes)))
            {
                if (countBits((int)t) == 1 && WantType(t))
                    node.AppendChild(doc.CreateElement(t.ToString()));
            }

            return node;
        }

        private int countBits(int v)
        {
            // Brian Kernighan's method, from http://graphics.stanford.edu/~seander/bithacks.html
            int c; // c accumulates the total bits set in v
            for (c = 0; v != 0; c++)
            {
                v &= v - 1; // clear the least significant bit set
            }
            return c;
        }

        #endregion
    }
}
