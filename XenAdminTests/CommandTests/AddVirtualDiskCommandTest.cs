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
using XenAdmin.ServerDBs;
using XenAPI;
using XenAdmin.Core;
using NUnit.Framework;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class AddVirtualDiskCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public AddVirtualDiskCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            AddVirtualDiskCommandTest tester = new AddVirtualDiskCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class AddVirtualDiskCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public AddVirtualDiskCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            AddVirtualDiskCommandTest tester = new AddVirtualDiskCommandTest();
            tester.Test();
        }
    }

    public class AddVirtualDiskCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new AddVirtualDiskCommand();
        }

        public void Test()
        {
            foreach (VM vm in RunTest(GetSingleSelections()))
            {
                int vbdCount = DbProxy.proxys[vm.Connection].db.Tables["vbd"].Rows.Count;

                MW(delegate
                {
                    MainWindowWrapper.StorageMenuItems.AddVirtualDiskToolStripMenuItemInStorageMenu.PerformClick();
                    NewDiskDialogWrapper newDiskDialogWrapper = new NewDiskDialogWrapper(WaitForWindowToAppear("Add Virtual Disk"));
                    newDiskDialogWrapper.OkButton.PerformClick();
                });

                // wait until command finished.
                MWWaitFor(() => DbProxy.proxys[vm.Connection].db.Tables["vbd"].Rows.Count == vbdCount + 1, "AddVirtualDiskCommandTest didn't finish.");
            }
        }

        private IEnumerable<IXenObject> GetSingleSelections()
        {
            foreach (VM vm in GetAllXenObjects<VM>(v => v.is_a_real_vm))
            {
                yield return vm;
            }
        }
    }
}
