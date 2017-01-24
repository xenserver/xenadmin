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
using XenAdmin.Controls.MainWindowControls;

using XenAPI;
using XenAdmin.Model;
using XenAdmin;
using XenAdmin.Controls;
using NUnit.Framework;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class RemoveFromFolderCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public RemoveFromFolderCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            RemoveFromFolderCommandTest tester = new RemoveFromFolderCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class RemoveFromFolderCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public RemoveFromFolderCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            RemoveFromFolderCommandTest tester = new RemoveFromFolderCommandTest();
            tester.Test();
        }
    }

    public class RemoveFromFolderCommandTest : CommandTest
    {
        private VirtualTreeNode _node;

        protected override NavigationPane.NavigationMode NativeMode
        {
            get { return NavigationPane.NavigationMode.Folders; }
        }

        internal override Command CreateCommand()
        {
            PutInNavigationMode(NativeMode);

            _node = GetAllTreeNodes().Find(n => n.Tag is IXenObject && !(n.Tag is Folder) && n.Parent.Tag is Folder);

            MW(() => _node.EnsureVisible());

            return new RemoveFromFolderCommand(Program.MainWindow, new List<VirtualTreeNode> { _node });
        }

        public void Test()
        {
            foreach (IXenObject x in RunTest(() => GetAnyXenObject(o => Folders.GetFolder(o) != null)))
            {
                MW(Command.Execute);

                IXenObject nodeXenObject = (IXenObject)_node.Tag;

                MWWaitFor(() => Folders.GetFolder(nodeXenObject) == null, "Item " + nodeXenObject + " not removed from folder.");
            }
        }
    }
}
