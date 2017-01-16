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
using XenAdmin.Controls;
using XenAdmin.Commands;
using XenAdmin.Controls.MainWindowControls;

using XenAPI;
using XenAdmin;
using XenAdmin.Model;
using NUnit.Framework;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class UntagCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public UntagCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            UntagCommandTest tester = new UntagCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class UntagCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public UntagCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            UntagCommandTest tester = new UntagCommandTest();
            tester.Test();
        }
    }

    public class UntagCommandTest : CommandTest
    {
        private VirtualTreeNode _node;
        private string _tag;

        protected override NavigationPane.NavigationMode NativeMode
        {
            get { return NavigationPane.NavigationMode.Tags; }
        }

        internal override Command CreateCommand()
        {
            PutInNavigationMode(NativeMode);

            _node = GetAllTreeNodes().Find(delegate(VirtualTreeNode n)
            {
                GroupingTag groupingTag = n.Parent == null ? null : n.Parent.Tag as GroupingTag;

                if (n.Tag is IXenObject && groupingTag != null && groupingTag.Grouping.GroupingName == "Tags")
                {
                    _tag = (string)groupingTag.Group;
                    return true;
                }
                return false;
            });


            MW(() => _node.EnsureVisible());

            return new UntagCommand(Program.MainWindow, new List<VirtualTreeNode> { _node });
        }

        public void Test()
        {
            foreach (IXenObject x in RunTest(() => GetAnyXenObject(o =>
                {
                    string[] tags = o.Get("tags") as string[];
                    return tags != null && tags.Length > 0;
                })))
            {
                MW(Command.Execute);

                IXenObject nodeXenObject = (IXenObject)_node.Tag;

                Assert.IsNotNull(_node);
                Assert.IsNotNull(_tag);
                Assert.IsNotNull(nodeXenObject);


                MWWaitFor(() => !Tags.GetTagList(nodeXenObject).Contains(_tag), "Item " + nodeXenObject + " was not untagged.");
            }
        }
    }
}
