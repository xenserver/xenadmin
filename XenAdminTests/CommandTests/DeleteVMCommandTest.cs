﻿/* Copyright (c) Citrix Systems, Inc. 
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
using NUnit.Framework;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class DeleteVMCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public DeleteVMCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            DeleteVMCommandTest tester = new DeleteVMCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class DeleteVMCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public DeleteVMCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            DeleteVMCommandTest tester = new DeleteVMCommandTest();
            tester.Test();
        }
    }

    public class DeleteVMCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new DeleteVMCommand();
        }

        public void Test()
        {
            foreach (SelectedItemCollection selection in RunTest())
            {
                foreach (VM vm in selection.AsXenObjects<VM>())
                {
                    Assert.IsTrue(VMExists(vm), "Could not find VM " + vm);
                }

                MW(Command.Execute);

                foreach (VM vm in selection.AsXenObjects<VM>())
                {
                    MWWaitFor(() => !VMExists(vm), "VM was not deleted: " + vm);
                }
            }
        }

        private bool VMExists(VM vm)
        {
            List<VM> vms = GetAllTreeNodes().FindAll(n => n.Tag is VM && ((VM)n.Tag).is_a_real_vm()).ConvertAll(n => (VM)n.Tag);

            return vms.Find(v => v.opaque_ref == vm.opaque_ref) != null;
        }
    }
}
