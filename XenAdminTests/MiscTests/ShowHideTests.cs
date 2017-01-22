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
using XenAdmin.Controls;
using XenAdmin.Controls.MainWindowControls;

using XenAPI;
using XenAdmin;
using XenAdmin.Core;


namespace XenAdminTests.MiscTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class ShowHideTests : MainWindowLauncher_TestFixture
    {
        public ShowHideTests()
            : base(true, "TampaTwoHostPoolSelectioniSCSI.xml")
        { }

        private ComparableList<IXenObject> Populate()
        {
            VirtualTreeNode rootNode = MW(() => new MainWindowTreeBuilder(new FlickerFreeTreeView()).CreateNewRootNode(new NavigationPane().Search, NavigationPane.NavigationMode.Infrastructure));
            List<VirtualTreeNode> nodes = new List<VirtualTreeNode>(rootNode.Descendants);
            return new ComparableList<IXenObject>(nodes.ConvertAll(n => (IXenObject)n.Tag));
        }

        /// <summary>
        /// Calculates the nodes that should be visible in the main window tree view when the specified xen object is hidden.
        /// </summary>
        /// <param name="hiddenObject">The hidden object.</param>
        private ComparableList<IXenObject> CalculatePopulateWithHiddenObject(IXenObject hiddenObject)
        {
            VirtualTreeNode rootNode = MW(() => new MainWindowTreeBuilder(new FlickerFreeTreeView()).CreateNewRootNode(new NavigationPane().Search, NavigationPane.NavigationMode.Infrastructure));

            List<VirtualTreeNode> nodes = new List<VirtualTreeNode>(rootNode.Descendants);

            nodes.RemoveAll(n => hiddenObject.Equals(n.Tag));
            nodes.RemoveAll(n => new List<VirtualTreeNode>(n.Ancestors).Find(nn => hiddenObject.Equals(nn.Tag)) != null);
            return new ComparableList<IXenObject>(nodes.ConvertAll(n => (IXenObject)n.Tag));
        }

        [Test]
        public void TestHideXenObject()
        {
            var xenObjectsToHide = Populate();

            foreach (var xenObject in xenObjectsToHide)
            {
                ComparableList<IXenObject> calc = CalculatePopulateWithHiddenObject(xenObject);

                Program.HideObject(xenObject.opaque_ref);
                Assert.AreEqual(0, calc.CompareTo(Populate()), "Incorrect hidden nodes.");
                Program.ShowObject(xenObject.opaque_ref);
            }
        }
    }
}
