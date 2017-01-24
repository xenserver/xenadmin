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
using XenAPI;
using XenAdmin;
using NUnit.Framework;


namespace XenAdminTests.CommandTests
{

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class ShutDownHostCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public ShutDownHostCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            ShutDownHostCommandTest tester = new ShutDownHostCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class ShutDownHostCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public ShutDownHostCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            ShutDownHostCommandTest tester = new ShutDownHostCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class ShutDownHostCommandTestRbacMidnightRide : MainWindowLauncher_TestFixture
    {
        public ShutDownHostCommandTestRbacMidnightRide()
            : base(true, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            ShutDownHostCommandTest tester = new ShutDownHostCommandTest();
            tester.TestRbacMidnightRide();
        }
    }

    public class ShutDownHostCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new ShutDownHostCommand();
        }

        public void Test()
        {
            foreach (SelectedItemCollection selection in RunTest(GetSelections()))
            {
                MW(Command.Execute);

                foreach (Host host in selection.AsXenObjects<Host>())
                {
                    MWWaitFor(() => host.allowed_operations.Contains(host_allowed_operations.power_on), "Host " + host + "didn't shutdown");
                }
            }
        }

        public void TestRbacMidnightRide()
        {
            foreach (SelectedItemCollection selection in RunTest(GetSelections()))
            {
                HandleModalDialog<RoleElevationDialogWrapper>(Messages.XENCENTER, Command.Execute, d => d.ButtonCancel.PerformClick());
            }
        }

        private IEnumerable<SelectedItemCollection> GetSelections()
        {
            yield return new SelectedItemCollection(new SelectedItem(GetAnyHost(h => !h.IsMaster())));
        }
    }
}
