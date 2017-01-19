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
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.ServerDBs
{
    /// <summary>
    /// Class for reading XenServer status report XML documents and populating a <see cref="Db"/> object with its contents.
    /// </summary>
    class StatusReportXmlDocReader
    {
        /// <summary>
        /// Populates the specified Db with the specified XML document.
        /// </summary>
        /// <param name="db">The <see cref="Db"/> to be populated.</param>
        /// <param name="doc">The <see cref="XmlDocument"/> to be read.</param>
        public void PopulateDbFromXml(Db db, XmlDocument doc)
        {
            Util.ThrowIfParameterNull(db, "db");
            Util.ThrowIfParameterNull(doc, "doc");

            if (db.Tables.Keys.Count > 0)
            {
                throw new ArgumentException("Specified Db should be empty.", "db");
            }

            XmlNode dataBaseNode = GetDatabaseNode(doc);
            IEnumerable hostNodes = GetHostNodes(dataBaseNode);

            foreach (XmlNode child in dataBaseNode.ChildNodes)
            {
                if (child.Name == "table")
                {
                    string tableName = child.Attributes["name"].Value.ToLower();
                    Db.Table table = db.Tables.Add(tableName);

                    foreach (XmlNode node in child.ChildNodes)
                    {
                        Db.Row row = table.Rows.Add(node.Attributes["ref"].Value);

                        foreach (XmlAttribute a in node.Attributes)
                        {
                            string name = ParsePropertyName(a.Name, tableName);
                            if(string.IsNullOrEmpty(name))
                                continue;

                            row.Props.Add(name, a.Value);
                        }
                    }

                    if (tableName == "host_metrics" && child.ChildNodes.Count == 0)  // host_metrics table used to be empty: see CA-31223
                    {
                        foreach (XmlNode host in hostNodes)
                        {
                            string opaque_ref = host.Attributes["metrics"].Value;
                            if (table.Rows.ContainsKey(opaque_ref))
                            {
                                // This doesn't happen with real databases, but for some of the hand-edited ones, we've got duplicate
                                // metrics opaquerefs.
                                continue;
                            }

                            Db.Row row = table.Rows.Add(opaque_ref);
                            row.Props.Add("live", "true");
                            row.Props.Add("memory_total", (2L * 1024 * 1024 * 1024).ToString());
                            row.Props.Add("memory_free", (1L * 1024 * 1024 * 1024).ToString());
                        }
                    }
                }
            }

            foreach (string xenApiType in SimpleProxyMethodParser.AllTypes)
            {
                if (!db.Tables.Keys.Contains(xenApiType))
                {
                    db.Tables.Add(xenApiType);
                }
            }
        }

        private string ParsePropertyName(string p, string table)
        {
            string prop = p.Replace("__", "_");

            switch (prop)
            {
                case "ref":
                    return table != "event" ? "" : prop;
                
                default:
                    return prop;
            }
        }

        private static XmlNode GetDatabaseNode(XmlNode doc)
        {
            foreach (XmlNode child in doc.ChildNodes)
            {
                if (child.Name == "database")
                {
                    return child;
                }
            }

            throw new InvalidOperationException("Couldn't find database tag in state db");
        }

        private static IEnumerable GetHostNodes(XmlNode databaseNode)
        {
            foreach (XmlNode child in databaseNode.ChildNodes)
            {
                if (child.Name == "table" && child.Attributes["name"].Value == "host")
                {
                    return child.ChildNodes;
                }
            }

            return null;
        }
    }
}
