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
using XenAdmin.XenSearch;
using XenAPI;

namespace XenAdmin.TestResources
{
    public class XenSearchQueryTest
    {
        // Generate a results file for XenAdminTests.SearchTests.ExpectedResults.
        // Run XenCenter, then in the immediate window run:
        //    XenAdmin.TestResources.XenSearchQueryTest.SaveResults(filename);
        // This will save the results of all the searches you have loaded.
        public static void SaveResults(string filename)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement resultsElement = doc.CreateElement("results");
            doc.AppendChild(resultsElement);

            foreach (Search search in Search.Searches)
            {
                XmlElement searchElement = doc.CreateElement("search");
                resultsElement.AppendChild(searchElement);
                XmlAttribute uuidAttr = doc.CreateAttribute("uuid");
                uuidAttr.Value = search.UUID;
                searchElement.Attributes.Append(uuidAttr);

                IAcceptGroups adapter = new XmlResultsAdapter(doc, searchElement);

                search.PopulateAdapters(adapter);
            }

            doc.Save(filename);
        }
    }

    public class XmlResultsAdapter : IAcceptGroups
    {
        private XmlDocument doc;
        private XmlElement parent;

        public XmlResultsAdapter(XmlDocument doc, XmlElement parent)
        {
            this.doc = doc;
            this.parent = parent;
        }

        public IAcceptGroups Add(Grouping grouping, object group, int indent)
        {
            IXenObject o = group as IXenObject;
            if (o != null)
            {
                XmlElement resultElement = doc.CreateElement("IXenObject");
                parent.AppendChild(resultElement);
                XmlAttribute uuidAttr = doc.CreateAttribute("opaqueref");
                uuidAttr.Value = (String)o.opaque_ref;
                resultElement.Attributes.Append(uuidAttr);

                return new XmlResultsAdapter(doc, resultElement);
            }
            else
            {
                XmlElement resultElement = doc.CreateElement("Object");
                parent.AppendChild(resultElement);
                XmlAttribute uuidAttr = doc.CreateAttribute("ToString");
                uuidAttr.Value = group.ToString();
                resultElement.Attributes.Append(uuidAttr);

                return new XmlResultsAdapter(doc, resultElement);
            }
        }

        public void FinishedInThisGroup(bool defaultExpand)
        {
        }
    }
}
