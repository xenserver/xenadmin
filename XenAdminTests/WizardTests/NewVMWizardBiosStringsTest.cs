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
using NUnit.Framework;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Wizards.NewVMWizard;
using XenAdmin.ServerDBs;
using XenAPI;
using XenAdmin;
using System.Windows.Forms;

namespace XenAdminTests.WizardTests.state4_xml
{
    /// <summary>
    /// Tests that VM.Clone is called when using the New VM Wizard with a user template with default storage settings.
    /// </summary>
    public abstract class NewVMWizardBiosStringsTest : WizardTest<NewVMWizard>
    {
        protected bool _copyBiosStringsInvoked;

        protected NewVMWizardBiosStringsTest(string[] pages)
            : base(pages, true, true)
        {
            Assert.AreEqual(1, DbProxy.proxys.Count);
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking += DbProxy_Invoking;
        }

        protected void DbProxy_Invoking(object sender, DbProxyInvokingEventArgs e)
        {
            _copyBiosStringsInvoked |= e.ProxyMethodInfo.MethodName == "copy_bios_strings" && e.ProxyMethodInfo.TypeName == "vm";
        }

        protected static int FindRow(DataGridView gridView, VM v)
        {
            DataGridViewRowCollection rows = gridView.Rows;
            for (int i = 0; i < rows.Count; ++i)
            {
                Page_Template.TemplatesGridViewItem row = (Page_Template.TemplatesGridViewItem)rows[i];
                if (row.Template == v)
                    return i;
            }
            return -1;  // no matches
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardBiosStringsSnapshotTest : NewVMWizardBiosStringsTest
    {
        public NewVMWizardBiosStringsSnapshotTest()
            : base(new[] { "Template", "BIOS Strings", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" })
        {
            Assert.AreEqual(1, DbProxy.proxys.Count);
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking += DbProxy_Invoking;
        }

        protected override NewVMWizard NewWizard()
        {
            VM snapshot = GetAnySnapshot(v=>v.name_label != "orphaned snapshot");
            Assert.NotNull(snapshot, "Snapshot not found.");
            List<Host> hosts = new List<Host>(snapshot.Connection.Cache.Hosts);
            Assert.IsTrue( hosts.Count > 0 );
			hosts.Sort();
            return new NewVMWizard(snapshot.Connection, snapshot, hosts[0]);
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Template")
            {
                var gridView = TestUtils.GetDataGridView(wizard, "page_1_Template.TemplatesGridView");
                var checkBox = TestUtils.GetCheckBox(wizard, "page_1_Template.checkBoxCopyBiosStrings");

                MWWaitFor(() => gridView.SelectedRows.Count == 1, "First item (snapshot) wasn't selected by default.");
                int row1 = gridView.SelectedRows[0].Index;

                MW(() =>
                       {
                           Assert.IsTrue(checkBox.Checked, "Checkbox should be checked for BIOS locked snapshot.");
                           Assert.IsFalse(checkBox.Enabled, "Checkbox should be disabled for BIOS locked snapshot.");
                       });

                // select a Windows template
                VM defaultTemplate = GetAnyDefaultTemplate(v => v.IsHVM && v.name_label != "orphaned snapshot");
                int row2 = FindRow(gridView, defaultTemplate);
                MW(() => gridView.Rows[row2].Selected = true);
                MWWaitFor(() => !checkBox.Checked, "Checkbox was checked for default-template.");
                MW(() => Assert.IsTrue(checkBox.Enabled, "Checkbox should be enabled when default-template selected."));

                // check the checkbox and move to the previous selection
                MW(() =>
                    {
                        checkBox.Checked = true;
                        gridView.Rows[row1].Selected = true;
                    });

                MWWaitFor(() => checkBox.Checked, "Checkbox should remain checked when moving from default-template to BIOS locked snapshot");
                MW(() => Assert.IsFalse(checkBox.Enabled, "Checkbox should be disabled for for BIOS locked snapshot"));
            }
            else if (pageName == "BIOS Strings")
            {
                MWWaitFor(() => TestUtils.GetDataGridView(wizard, "page_1b_BiosLocking.ServersGridView").RowCount == 0);
            }
            else if (pageName == "Name")
            {
                Assert.IsFalse(TestUtils.GetXenTabPage(wizard,"page_4_HomeServer").DisableStep, "Home Server page should be enabled when BIOS locking checkbox is checked.");
            }

            base.TestPage(pageName);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardBiosStringsUserTemplateTest : NewVMWizardBiosStringsTest
    {
        public NewVMWizardBiosStringsUserTemplateTest()
            : base(new[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" })
        {
            Assert.AreEqual(1, DbProxy.proxys.Count);
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking += DbProxy_Invoking;
        }

        protected override NewVMWizard NewWizard()
        {
            VM template = GetAnyUserTemplate();
            Assert.NotNull(template, "User template not found not found.");
            List<Host> hosts = new List<Host>(template.Connection.Cache.Hosts);
            Assert.IsTrue(hosts.Count > 0);
            hosts.Sort();
            return new NewVMWizard(template.Connection, template, hosts[0]);
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Template")
            {
                var gridView = TestUtils.GetDataGridView(wizard, "page_1_Template.TemplatesGridView");
                var checkBox = TestUtils.GetCheckBox(wizard, "page_1_Template.checkBoxCopyBiosStrings");

                // select the non-bios locked user template
                MWWaitFor(() => gridView.SelectedRows.Count == 1, "First item (user template) wasn't selected by default.");
                int row1 = gridView.SelectedRows[0].Index;

                MWWaitFor(() => !checkBox.Checked, "Checkbox was checked for non-BIOS locked user-template.");
                MW(() => Assert.IsFalse(checkBox.Enabled, "Checkbox should be disabled when user-template selected."));

                // select a Windows template
                VM defaultTemplate = GetAnyDefaultTemplate(v => v.IsHVM);
                int row2 = FindRow(gridView, defaultTemplate);
                MW(() => gridView.Rows[row2].Selected = true);
                MWWaitFor(() => !checkBox.Checked, "Checkbox was checked for default-template.");
                MW(() => Assert.IsTrue(checkBox.Enabled, "Checkbox should be enabled when default-template selected."));

                // check the checkbox and move to the previous selection
                MW(() =>
                {
                    checkBox.Checked = true;
                    gridView.Rows[row1].Selected = true;
                });

                MWWaitFor(() => !checkBox.Checked, "Checkbox should not remain checked when moving from default-template to non-BIOS locked user-template.");

                MW(() => Assert.IsFalse(checkBox.Enabled, "Checkbox should be disabled when user-template selected."));
            }
            else if (pageName == "Name")
            {
                Assert.IsFalse(TestUtils.GetXenTabPage(wizard, "page_4_HomeServer").DisableStep, "Home Server page should be enabled for non-BIOS locked user-template.");
            }

            base.TestPage(pageName);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardBiosStringsWindowsTemplateTest : NewVMWizardBiosStringsTest
    {
        public NewVMWizardBiosStringsWindowsTemplateTest()
            : base(new[] { "Template", "BIOS Strings", "Name", "Installation Media", "CPU && Memory", "Storage", "Networking", "Finish" })
        {
            Assert.AreEqual(1, DbProxy.proxys.Count);
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking += DbProxy_Invoking;
        }

        protected override NewVMWizard NewWizard()
        {
            VM defTemplate = GetAnyDefaultTemplate(v => v.IsHVM);
            Assert.NotNull(defTemplate, "Default template not found.");
            List<Host> hosts = new List<Host>(defTemplate.Connection.Cache.Hosts);
            Assert.IsTrue(hosts.Count > 0);
            hosts.Sort();
            return new NewVMWizard(defTemplate.Connection, defTemplate, hosts[0]);
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Template")
            {
                var gridView = TestUtils.GetDataGridView(wizard, "page_1_Template.TemplatesGridView");
                var checkBox = TestUtils.GetCheckBox(wizard, "page_1_Template.checkBoxCopyBiosStrings");

                //select the windows template
                MWWaitFor(() => gridView.SelectedRows.Count == 1, "First item (windows template) wasn't selected by default.");
                
                MWWaitFor(() => !checkBox.Checked, "Checkbox was checked for default-template.");
                MW(() => Assert.IsTrue(checkBox.Enabled, "Checkbox should be enabled when default-template selected."));

                // select another default template
                VM otherDefTemplate = GetAnyDefaultTemplate(v => v.IsHVM && v != ((Page_Template.TemplatesGridViewItem)gridView.SelectedRows[0]).Template);
                int row2 = FindRow(gridView, otherDefTemplate);

                // check the checkbox and move to another default template
                MW(() =>
                {
                    checkBox.Checked = true;
                    gridView.Rows[row2].Selected = true;
                });

                MWWaitFor(() => checkBox.Checked, "Checkbox should remain checked when moving from default-template to default-template");

                MW(() => Assert.IsTrue(checkBox.Enabled, "Checkbox should be enabled for default-template"));
            }
            else if (pageName == "BIOS Strings")
            {
                MWWaitFor(() => TestUtils.GetDataGridView(wizard, "page_1b_BiosLocking.ServersGridView").Rows[0].Selected, "First row wasn't selected.");
            }
            else if (pageName == "Name")
            {
                Assert.IsTrue(TestUtils.GetXenTabPage(wizard, "page_4_HomeServer").DisableStep, "Home Server page should be disabled when BIOS locking checkbox is checked.");
            }

            base.TestPage(pageName);
        }

        protected override void RunAfter()
        {
            MWWaitFor(() => wizard.Action.IsCompleted && wizard.Action.Succeeded, "Wizard didn't succeed.");
            Assert.IsTrue(_copyBiosStringsInvoked, "VM.copy_bios_strings wasn't called.");
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking -= DbProxy_Invoking;
        }
    }
}
