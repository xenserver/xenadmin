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
using XenAdmin.Commands;
using XenAdmin;
using XenAdmin.Controls;
using NUnit.Framework;

using XenAdmin.Controls.MainWindowControls;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class DeleteTagCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public DeleteTagCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            DeleteTagCommandTest tester = new DeleteTagCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class DeleteTagCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public DeleteTagCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            DeleteTagCommandTest tester = new DeleteTagCommandTest();
            tester.Test();
        }
    }

    public class DeleteTagCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new DeleteTagCommand();
        }

        protected override NavigationPane.NavigationMode NativeMode
        {
            get { return NavigationPane.NavigationMode.Tags; }
        }

        public void Test()
        {
            foreach (SelectedItemCollection selection in RunTest(GetSelections()))
            {
                foreach (GroupingTag groupingTag in selection.AsGroupingTags())
                {
                    MWWaitFor(() => TagExists(groupingTag), "Could not find GroupingTag " + groupingTag.Group);
                }

                MW(Command.Execute);

                foreach (GroupingTag groupingTag in selection.AsGroupingTags())
                {
                    MWWaitFor(() => !TagExists(groupingTag), "GroupingTag was not deleted: " + groupingTag.Group);
                }
            }
        }

        private bool TagExists(GroupingTag tag)
        {
            return GetAllTreeNodes().ConvertAll(n => n.Tag as GroupingTag).Contains(tag);
        }

        private IEnumerable<SelectedItemCollection> GetSelections()
        {
            // just delete one tag for this test as it's quite slow.

            PutInNavigationMode(NativeMode);

            VirtualTreeNode node = GetAllTreeNodes().Find(n =>
                {
                    GroupingTag g = n.Tag as GroupingTag;
                    return g != null && n.Parent != null && g.Grouping.GroupingName == "Tags";
                });

            GroupingTag gt = (GroupingTag)node.Tag;

            var rootNode = GetAllTreeNodes().Find(n => n.Parent == null);

            yield return new SelectedItemCollection(new SelectedItem(gt, rootNode));
        }
    }
}
