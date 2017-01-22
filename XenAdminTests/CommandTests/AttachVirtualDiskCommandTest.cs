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
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.ServerDBs;
using NUnit.Framework;
using XenAdmin.Controls;
using System.Windows.Forms;
using XenAdmin;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class AttachVirtualDiskCommandTestGeorge : MainWindowLauncher_TestFixture
    {
        public AttachVirtualDiskCommandTestGeorge()
            : base(false, CommandTestsDatabase.George)
        { }

        [Test]
        public void Run()
        {
            AttachVirtualDiskCommandTest tester = new AttachVirtualDiskCommandTest();
            tester.Test();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class AttachVirtualDiskCommandTestMidnightRide : MainWindowLauncher_TestFixture
    {
        public AttachVirtualDiskCommandTestMidnightRide()
            : base(false, CommandTestsDatabase.MidnightRide)
        { }

        [Test]
        public void Run()
        {
            AttachVirtualDiskCommandTest tester = new AttachVirtualDiskCommandTest();
            tester.Test();
        }
    }

    public class AttachVirtualDiskCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new AttachVirtualDiskCommand();
        }

        public void Test()
        {
            foreach (VM vm in RunTest(GetSingleSelections()))
            {
                EnsureChecked(MainWindowWrapper.ViewMenuItems.TemplatesToolStripMenuItem, CheckState.Unchecked);

                int vbdCount = DbProxy.proxys[vm.Connection].db.Tables["vbd"].Rows.Count;

                VM colsureVM = vm;
                bool checkVBDs = true;
                MW(delegate
                {
                    MainWindowWrapper.StorageMenuItems.AttachVirtualDiskToolStripMenuItemInStorageMenu.PerformClick();
                    AttachDiskDialogWrapper attachDiskDialogWrapper = new AttachDiskDialogWrapper(WaitForWindowToAppear("Attach Disk"));
                    attachDiskDialogWrapper.DiskListTreeView.SelectedItem = attachDiskDialogWrapper.DiskListTreeView.Nodes.Find(n => n.Level == 1);
                    if (colsureVM.IsHVM)
                    {
                        Assert.IsFalse(attachDiskDialogWrapper.ReadOnlyCheckBox.Enabled, "ReadOnly Checkbox enabled");
                        if (attachDiskDialogWrapper.ReadOnlyCheckBox.Checked)
                        {
                            Assert.IsFalse(attachDiskDialogWrapper.OkBtn.Enabled,
                                       "OK button of AttachDiskDialog enabled for VM: " + colsureVM.name_label);
                            attachDiskDialogWrapper.CancelBtn.PerformClick();
                            checkVBDs = false;
                        }
                        else
                        {
                            Assert.IsTrue(attachDiskDialogWrapper.OkBtn.Enabled,
                                           "OK button of AttachDiskDialog enabled for VM: " + colsureVM.name_label);
                            attachDiskDialogWrapper.OkBtn.PerformClick();
                        }
                    }
                    else
                    {
                        if (attachDiskDialogWrapper.ReadOnlyCheckBox.Checked)
                            Assert.IsFalse(attachDiskDialogWrapper.ReadOnlyCheckBox.Enabled, "ReadOnly Checkbox enabled");
                        else
                            Assert.IsTrue(attachDiskDialogWrapper.ReadOnlyCheckBox.Enabled, "ReadOnly Checkbox enabled");

                        Assert.IsTrue(attachDiskDialogWrapper.OkBtn.Enabled,
                                      "OK button of AttachDiskDialog enabled for VM: " + colsureVM.name_label);
                        attachDiskDialogWrapper.OkBtn.PerformClick();
                    }
                });

                Func<bool> finished;
                if(checkVBDs)
                {
                    // ensure that the number of VBDs has increased.
                    finished = delegate
                    {
                        return DbProxy.proxys[vm.Connection].db.Tables["vbd"].Rows.Count == vbdCount + 1;
                    };
                }
                else
                {
                    finished = delegate
                    {
                        return DbProxy.proxys[vm.Connection].db.Tables["vbd"].Rows.Count == vbdCount;
                    }; 
                }

                // wait until command finished.
                MWWaitFor(finished, "AttachVirtualDiskCommandTest tried to check VBD count but couldn't finish.");

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
