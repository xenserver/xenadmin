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
using System.Threading;
using XenAdmin.Commands;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Controls;
using NUnit.Framework;
using XenAPI;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class AddNewHostToPoolCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public AddNewHostToPoolCommandTestGeorge()
            : base(CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            AddNewHostToPoolCommandTest tester = new AddNewHostToPoolCommandTest();
            tester.TestRbacGeorge();
        }
    }

    public class AddNewHostToPoolCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new AddNewHostToPoolCommand(Program.MainWindow, GetAnyPool());
        }

        public void TestRbacGeorge()
        {
            foreach (Pool pool in RunTest(GetAnyPool))
            {
                MW(delegate
                {
                    Command.Execute();
                    AddServerDialogWrapper addServerDialogWrapper = new AddServerDialogWrapper(WaitForWindowToAppear("Add New Server"));
                    
                    Thread.Sleep(500);
                    Assert.IsNotNull(addServerDialogWrapper, "AddServerDialogWrapper was null");
                    Assert.IsNotNull(addServerDialogWrapper.ServerNameComboBox, "ServerNameComboBox was null");
                    Assert.IsNotNull(addServerDialogWrapper.AddButton, "AddButton was null");

                    addServerDialogWrapper.ServerNameComboBox.Text = GetTestResource(CommandTestsDatabase.SingleHost);
                    addServerDialogWrapper.AddButton.PerformClick();
                });

                Func<bool> finished = delegate
                {
                    VirtualTreeView.VirtualTreeNodeCollection poolNodes = MainWindowWrapper.TreeView.Nodes[0].Nodes[0].Nodes;
                    return poolNodes.Count > 2 && poolNodes[0].Text == "inflames" && poolNodes[1].Text == "incubus" && poolNodes[2].Text == "krakout";
                };

                // wait until command finished.
                MWWaitFor(finished, "AddNewHostToPoolCommandTest.TestRbacGeorge() didn't finish.");
            }
        }
    }
}
