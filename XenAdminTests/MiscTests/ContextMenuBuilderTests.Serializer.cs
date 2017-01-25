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
using System.Windows.Forms;
using XenAPI;
using System.IO;
using NUnit.Framework;

namespace XenAdminTests.MiscTests
{
    partial class ContextMenuBuilderTests
    {
        public static class Serializer
        {
            public static void SerializeTestResult(string filename, Selection selection, ToolStripItem[] items)
            {
                XmlDocument doc = new XmlDocument();
                XmlNode root;

                if (!File.Exists(filename) || File.ReadAllText(filename).Length == 0)
                {
                    root = doc.CreateNode(XmlNodeType.Element, "ContextMenuBuilderTests", "");
                    doc.AppendChild(root);
                }
                else
                {
                    doc.Load(filename);
                    root = doc.ChildNodes[1];
                }

                XmlNode testNode = doc.CreateNode(XmlNodeType.Element, "Test", "");
                root.AppendChild(testNode);

                XmlNode selectedItemsNode = doc.CreateNode(XmlNodeType.Element, "Selection", "");
                testNode.AppendChild(selectedItemsNode);

                foreach (IXenObject o in selection.Objects)
                {
                    XmlNode selectedItemNode = doc.CreateNode(XmlNodeType.Element, "SelectedItem", "");
                    selectedItemsNode.AppendChild(selectedItemNode);
                    XmlAttribute a = doc.CreateAttribute("OpaqueRef");
                    a.Value = o.opaque_ref;
                    selectedItemNode.Attributes.Append(a);
                    a = doc.CreateAttribute("ToString");
                    a.Value = o.ToString();
                    selectedItemNode.Attributes.Append(a);
                }

                XmlNode contextMenuItemsNode = doc.CreateNode(XmlNodeType.Element, "ToolStripItems", "");
                testNode.AppendChild(contextMenuItemsNode);

                foreach (ToolStripItem item in items)
                {
                    XmlNode itemNode = doc.CreateNode(XmlNodeType.Element, "ToolStripItem", "");
                    contextMenuItemsNode.AppendChild(itemNode);

                    XmlAttribute a = doc.CreateAttribute("Text");
                    a.Value = item.Text;
                    itemNode.Attributes.Append(a);
                    a = doc.CreateAttribute("IsSeparator");
                    a.Value = (item is ToolStripSeparator).ToString();
                    itemNode.Attributes.Append(a);
                    a = doc.CreateAttribute("Bold");
                    a.Value = item.Font.Bold.ToString();
                    itemNode.Attributes.Append(a);
                    a = doc.CreateAttribute("Available");
                    a.Value = item.Available.ToString();
                    itemNode.Attributes.Append(a);
                }

                using (XmlWriter w = XmlTextWriter.Create(filename, new XmlWriterSettings { OmitXmlDeclaration = false, Encoding = Encoding.ASCII }))
                {
                    doc.WriteTo(w);
                }
            }
        }
    }
}
