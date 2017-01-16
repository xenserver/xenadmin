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

using System.Xml;
using XenAPI;

namespace XenAdmin.XenSearch
{
    public class Query
    {
        private QueryScope scope;
        private QueryFilter filter;

        public Query(QueryScope scope, QueryFilter filter)
        {
            if (scope == null)
                this.scope = new QueryScope(ObjectTypes.AllExcFolders);
            else
                this.scope = scope;

            this.filter = filter;  // null is OK
        }

        public Query(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element)
                    continue;
                else if (child.Name == "QueryScope")
                    this.scope = new QueryScope(child);
                else
                    this.filter = (QueryFilter)SearchMarshalling.FromXmlNode(child);
            }
        }

        public QueryScope QueryScope
        {
            get
            {
                return scope;
            }
        }

        public QueryFilter QueryFilter
        {
            get
            {
                return filter;
            }
        }

        public bool Match(IXenObject o)
        {
            return (scope.WantType(o) && o.Show(XenAdminConfigManager.Provider.ShowHiddenVMs) && (filter == null || filter.Match(o) != false));
        }

        public QueryFilter GetSubQueryFor(PropertyNames property)
        {
            return (filter == null ? null : filter.GetSubQueryFor(property));
        }

        public override bool Equals(object obj)
        {
            Query other = obj as Query;
            if (other == null)
                return false;

            if (!((filter == null && other.filter == null) || (filter != null && filter.Equals(other.filter))))
                return false;

            return scope.Equals(other.scope);
        }

        public override int GetHashCode()
        {
            return filter == null ? scope.GetHashCode() : (filter.GetHashCode() + 1) * scope.GetHashCode();
        }

        #region Marshalling

        public virtual XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(SearchMarshalling.GetClassName(this));
            node.AppendChild(scope.ToXmlNode(doc));
            XmlNode filterNode = (filter == null ? null : filter.ToXmlNode(doc));
            if (filterNode != null)
                node.AppendChild(filterNode);
            return node;
        }

        #endregion
    }
}
