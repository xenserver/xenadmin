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
using NUnit.Framework;
using XenAdmin.Commands;
using XenAdmin.Plugins;
using XenAdmin;
using XenAdmin.Network;
using XenAdmin.ServerDBs;
using System.Windows.Forms;
using XenAPI;
using System.IO;
using System.Reflection;
using XenAdmin.Core;
using System.Xml;

namespace XenAdminTests.MiscTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public partial class ContextMenuBuilderTests : MainWindowLauncher_TestFixture
    {
        private readonly MockMainWindow _mainWindow = new MockMainWindow();
        private readonly PluginManager _pluginManager = new PluginManager();

        public ContextMenuBuilderTests()
            : base("state4.xml")
        { }

        /// <summary>
        /// Gets every possible pair and triple of a set of the xen objects. This is fed into TestContextMenuForXenObjectSelectionCombination.
        /// </summary>
        private IEnumerable<Selection> XenObjectSelections
        {
            get
            {
                var list = new List<IXenObject>{
                GetAnyPool(p => p.name_label == "Hottub"),
                GetAnyHost(h => h.IsMaster()),
                GetAnyHost(h => !h.IsMaster()),
                GetAnyVM(v => v.name_label == "Windows Server 2003 x64 (1)"),
                GetAnyVM(v => v.name_label == "Windows XP SP2 (1)"),
                GetAnyVM(v => v.name_label == "Windows Server 2008 (1)"),
                GetAnySR(sr => sr.name_label == "NFS ISO library"),
                GetAnyDefaultTemplate(t => t.name_label == "CentOS 4.5"),
                GetAnyUserTemplate(t => t.name_label == "Red Hat Enterprise Linux 5.3 (1)")};

                List<Selection> output = new List<Selection>();
                foreach (IXenObject o in list)
                {
                    foreach (IXenObject oo in list)
                    {
                        Selection s = new Selection(o, oo);
                        if (!o.Equals(oo) && !output.Contains(s))
                        {
                            output.Add(s);

                            foreach (IXenObject ooo in list)
                            {
                                s = new Selection(o, oo, ooo);
                                if (!o.Equals(ooo) && !oo.Equals(ooo) && !output.Contains(s))
                                {
                                    output.Add(s);
                                }
                            }

                        }
                    }
                }
                return output;
            }
        }

        [Test]
        public void TestMultiselectContextMenu()
        {
            XmlDocument storedResults = new XmlDocument();
            storedResults.Load(TestResource("ContextMenuBuilderTestResults.xml"));

            foreach (Selection selection in XenObjectSelections)
            {
                MW(() =>
                {
                    ContextMenuBuilder cmd = new ContextMenuBuilder(_pluginManager, _mainWindow);
                    ToolStripItem[] items = cmd.Build(selection.Objects.ConvertAll(x => new SelectedItem(GetAnyXenObject(xx => xx.opaque_ref == x.opaque_ref))));
                    AssertItemsMatchStoredResults(storedResults, selection, items);
                });
            }

            // use this to save results
            // Serializer.SerializeTestResult(TestResource("ContextMenuBuilderTestResults.xml"), s, items);
        }

        private static void AssertItemsMatchStoredResults(XmlDocument storedResults, Selection selection, ToolStripItem[] items)
        {
            foreach (XmlNode testNode in storedResults.SelectNodes("/ContextMenuBuilderTests/Test"))
            {
                XmlNode selectionNode = testNode.SelectSingleNode("Selection");
                XmlNode toolStripItemsNode = testNode.SelectSingleNode("ToolStripItems");

                if (selectionNode.ChildNodes.Count == selection.Objects.Count)
                {
                    if(selection.Objects.TrueForAll(x => x.opaque_ref == selectionNode.ChildNodes[selection.Objects.IndexOf(x)].Attributes["OpaqueRef"].Value))
                    {
                        Assert.AreEqual(toolStripItemsNode.ChildNodes.Count, items.Length, "Incorrect number of items in context menu.");

                        for (int i = 0; i < items.Length; i++)
                        {
                            Assert.AreEqual(toolStripItemsNode.ChildNodes[i].Attributes["Text"].Value, items[i].Text, "Item has incorrect text.");
                            Assert.AreEqual(toolStripItemsNode.ChildNodes[i].Attributes["IsSeparator"].Value, (items[i] is ToolStripSeparator).ToString(), "Item has incorrect separator value");
                            Assert.AreEqual(toolStripItemsNode.ChildNodes[i].Attributes["Bold"].Value, items[i].Font.Bold.ToString(), "Item has incorrect bold value");
                            Assert.AreEqual(toolStripItemsNode.ChildNodes[i].Attributes["Available"].Value, items[i].Available.ToString(), "Item has incorrect Available value.");
                        }
                        return;
                    }
                }
            }

            Assert.Fail(string.Format("Couldn't find selection in stored results for: {0}", selection));
        }

        public class Selection
        {
            public readonly ComparableList<IXenObject> Objects = new ComparableList<IXenObject>();

            public Selection(params IXenObject[] objects)
            {
                Objects.AddRange(objects);
                Objects.Sort();
            }

            public override bool Equals(object obj)
            {
                Selection other = obj as Selection;
                return other != null && other.Objects.CompareTo(Objects) == 0;
            }

            public override int GetHashCode()
            {
                return Objects.GetHashCode();
            }

            public override string ToString()
            {
                return string.Join(", ", Objects.ConvertAll(x => x.ToString()).ToArray());
            }
        }
    }
}
