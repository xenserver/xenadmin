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

using XenAdmin.Controls.MainWindowControls;

using XenAPI;
using NUnit.Framework;
using XenAdmin.Commands;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class DeleteSnapshotCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public DeleteSnapshotCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            DeleteSnapshotCommandTest tester = new DeleteSnapshotCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class DeleteSnapshotCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public DeleteSnapshotCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            DeleteSnapshotCommandTest tester = new DeleteSnapshotCommandTest();
            tester.Test();
        }
    }


    public class DeleteSnapshotCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new DeleteSnapshotCommand();
        }

        protected override NavigationPane.NavigationMode NativeMode
        {
            get { return NavigationPane.NavigationMode.Objects; }
        }

        public void Test()
        {
            foreach (SelectedItemCollection selection in RunTest())
            {
                foreach (VM snapshot in selection.AsXenObjects<VM>())
                {
                    MWWaitFor(() => SnapshotExists(snapshot), "Could not find snapshot " + snapshot);
                }

                MW(Command.Execute);

                foreach (VM snapshot in selection.AsXenObjects<VM>())
                {
                    MWWaitFor(() => !SnapshotExists(snapshot), "Snapshot was not deleted: " + snapshot);
                }
            }
        }

        private bool SnapshotExists(VM snapshot)
        {
            List<VM> snapshots = GetAllTreeNodes().FindAll(n => n.Tag is VM).ConvertAll(n => (VM)n.Tag);

            return snapshots.Find(v => v.opaque_ref == snapshot.opaque_ref) != null;
        }
    }
}
