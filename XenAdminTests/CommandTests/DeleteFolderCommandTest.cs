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
using XenAdmin.Model;
using NUnit.Framework;


namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class DeleteFolderCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public DeleteFolderCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            DeleteFolderCommandTest tester = new DeleteFolderCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class DeleteFolderCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public DeleteFolderCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            DeleteFolderCommandTest tester = new DeleteFolderCommandTest();
            tester.Test();
        }
    }

    public class DeleteFolderCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new DeleteFolderCommand();
        }

        protected override NavigationPane.NavigationMode NativeMode
        {
            get { return NavigationPane.NavigationMode.Folders; }
        }

        public void Test()
        {
            foreach (SelectedItemCollection selection in RunTest())
            {
                foreach (Folder folder in selection.AsXenObjects<Folder>())
                {
                    Assert.IsTrue(FolderExists(folder), "Could not find folder.");
                }
                
                MW(Command.Execute);
                
                foreach (Folder folder in selection.AsXenObjects<Folder>())
                {
                    MWWaitFor(() => !FolderExists(folder), "Folder was not deleted.");
                }
            }
        }

        private bool FolderExists(Folder folder)
        {
            List<Folder> folders = GetAllTreeNodes().FindAll(n => n.Tag is Folder).ConvertAll(n => (Folder)n.Tag);
            
            return folders.Find(f => f.opaque_ref == folder.opaque_ref) != null;
        }
    }
}
