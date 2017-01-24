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
using System.Globalization;
using System.Text;
using System.Xml;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.XenSearch;
using XenAPI;

namespace XenAdminTests.SearchTests
{
    public class ComparerAdapter : IAcceptGroups
    {
        private Search search;
        private XmlNode parent;
        private int i = 0;

        public static void CompareResults(Search search, XmlNode xmlNode)
        {
            ComparerAdapter adapter = new ComparerAdapter(search, xmlNode);
            search.PopulateAdapters(adapter);
        }

        private ComparerAdapter(Search search, XmlNode parent)
        {
            this.search = search;
            this.parent = parent;
        }

        public IAcceptGroups Add(Grouping grouping, object group, int indent)
        {
            XmlNode child = parent.ChildNodes[i];
            Assert.IsNotNull(child, "Too many results under " + parent.OuterXml + " in the search '" + search.Name + "'");

            IXenObject o = group as IXenObject;
            XmlAttribute opaqueref = child.Attributes["opaqueref"];
            XmlAttribute toString = child.Attributes["ToString"];
            if (o != null)
            {
                Assert.IsNotNull(opaqueref, "Expected group " + (toString == null ? "" : toString.Value) + ", found resource " + o.opaque_ref + " in the search '" + search.Name + "'");
                Assert.AreEqual(opaqueref.Value, o.opaque_ref, "Wrong resource found in the search '" + search.Name + "'");
            }
            else
            {
                string expected = grouping.GetGroupName(group);
                Assert.IsNotNull(toString, "Expected resource " + (opaqueref == null ? "" : opaqueref.Value) + ", found group " + expected + " in the search '" + search.Name + "'");
                Assert.AreEqual(toString.Value, expected, "Wrong group found in the search '" + search.Name + "'");
            }

            ++i;
            return new ComparerAdapter(search, child);
        }

        public void FinishedInThisGroup(bool defaultExpand)
        {
            Assert.AreEqual(parent.ChildNodes.Count, i, "Too few results under " + parent.OuterXml + " in the search '" + search.Name + "'");
        }
    }
}
