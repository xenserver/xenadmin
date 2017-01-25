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
using XenAdmin.Controls;
using XenAdmin.Controls.MainWindowControls;
using XenAdmin.Model;
using NUnit.Framework;
using XenAdmin.Core;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class PutFolderIntoRenameModeCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public PutFolderIntoRenameModeCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            PutFolderIntoRenameModeCommandTest tester = new PutFolderIntoRenameModeCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class PutFolderIntoRenameModeCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public PutFolderIntoRenameModeCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            PutFolderIntoRenameModeCommandTest tester = new PutFolderIntoRenameModeCommandTest();
            tester.Test();
        }
    }


    public class PutFolderIntoRenameModeCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new PutFolderIntoRenameModeCommand();
        }

        protected override NavigationPane.NavigationMode NativeMode
        {
            get { return NavigationPane.NavigationMode.Folders; }
        }

        public void Test()
        {
            foreach (SelectedItemCollection selection in RunTest())
            {
                Folder folder = (Folder)selection[0].XenObject;

                // choose new folder name
                string newFolderName = Guid.NewGuid().ToString();

                // execute rename folder command
                MW(Command.Execute);

                // get in-line edit box which is a child of the treeview.
                MWWaitFor(delegate
                {
                    foreach (Win32Window child in new Win32Window(MainWindowWrapper.TreeView.Handle).Children)
                    {
                        child.Text = newFolderName;
                        FindInTree(folder).EndEdit(false);
                        return true;
                    }
                    return false;
                }, "Couldn't find rename-node edit box.");

                // now check to see if folder has been renamed and is still selected, and is visible.
                MWWaitFor(delegate
                {
                    VirtualTreeNode n = MainWindowWrapper.TreeView.SelectedNode;
                    Folder f = n == null ? null : n.Tag as Folder;
                    return f != null && n.Text == newFolderName && f.Name == newFolderName && n.AllParentsAreExpanded;
                }, "Couldn't find renamed folder.");
            }
        }
    }
}
