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
using System.Text;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Controls.MainWindowControls;

namespace XenAdminTests.TreeTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class MainWindowTreeBuilderTests : MainWindowLauncher_TestFixture
    {
        public MainWindowTreeBuilderTests()
            : base("state1.xml")
        { }

        [Test]
        public void TestState1CorrectlyPopulatedInServerView()
        {
            MW(()=>
            {
                MainWindowTreeBuilder _treeBuilder = new MainWindowTreeBuilder(MainWindowWrapper.TreeView);
                VirtualTreeNode newRoot = _treeBuilder.CreateNewRootNode(new NavigationPane().Search, NavigationPane.NavigationMode.Infrastructure);
                MainWindowWrapper.TreeView.UpdateRootNodes(new[] {newRoot});
                MainWindowWrapper.TreeView.EndUpdate();
                AssertTreeViewsSame(DeserializeTreeView("state1.treeview.serverview.xml").Nodes, MainWindowWrapper.TreeView.Nodes);
            });
        }

        [Test]
        public void TestState1CorrectlyPopulatedInOrgView()
        {
            MW(() =>
            {
                MainWindowTreeBuilder _treeBuilder = new MainWindowTreeBuilder(MainWindowWrapper.TreeView);
                VirtualTreeNode newRoot = _treeBuilder.CreateNewRootNode(new NavigationPane().Search, NavigationPane.NavigationMode.Objects);
                MainWindowWrapper.TreeView.UpdateRootNodes(new[] { newRoot });
                MainWindowWrapper.TreeView.EndUpdate();
                AssertTreeViewsSame(DeserializeTreeView("state1.treeview.orgview.xml").Nodes, MainWindowWrapper.TreeView.Nodes);
            });

        }

        private VirtualTreeView DeserializeTreeView(string testResource)
        {
            VirtualTreeView treeView = new VirtualTreeView();
            new TreeViewSerializer().DeserializeTreeView(treeView, GetTestResource(testResource));
            return treeView;
        }

        private static void AssertTreeViewsSame(VirtualTreeView.VirtualTreeNodeCollection nodes1, VirtualTreeView.VirtualTreeNodeCollection nodes2)
        {
            if (nodes1.Count != nodes2.Count)
            {
                var info = new StringBuilder();
                for (int i = 0; i < Math.Max(nodes1.Count, nodes2.Count); i++)
                {
                    info.AppendFormat("[{0},{1}]",
                        nodes1.Count > i ? nodes1[i].Text : "",
                        nodes2.Count > i ? nodes2[i].Text : "");
                }

                Assert.AreEqual(nodes1.Count, nodes2.Count, "The number of nodes is different between the 2 NodeCollections: " + info);
            }

            for (int i = 0; i < nodes1.Count; i++)
            {
                Assert.AreEqual(nodes1[i].Text, nodes2[i].Text, "The text is different between the nodes.");

                if (nodes1[i].Tag == null)
                {
                    Assert.IsNull(nodes2[i].Tag, "The tags are different between the nodes.");
                }
                else
                {
                    Assert.AreEqual(nodes1[i].Tag.ToString(), nodes2[i].Tag.ToString(), "The tags are different between the nodes.");
                }

                Assert.AreEqual(nodes1[i].ImageIndex, nodes2[i].ImageIndex, "The image indexes are different between the nodes for " + nodes1[i].Text);

                AssertTreeViewsSame(nodes1[i].Nodes, nodes2[i].Nodes);
            }
        }
    }
}
