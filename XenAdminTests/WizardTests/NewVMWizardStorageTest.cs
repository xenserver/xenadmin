/* Copyright (c) Cloud Software Group, Inc. 
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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using XenAdmin.Wizards.NewVMWizard;
using XenAPI;
using XenAdmin.ServerDBs;
using XenAdmin;
using System.Windows.Forms;
using XenAdmin.Controls;

namespace XenAdminTests.WizardTests.state5_xml
{
    [TestFixture]
    [Category(TestCategories.UICategoryB)]
    [Description("Tests that VM.Clone is called when using the New VM Wizard " +
                 "with a user template with default storage settings.")]
    public class NewVMWizardTestUserTemplateClone : WizardTest<NewVMWizard>
    {
        private bool _cloneInvoked;

        public NewVMWizardTestUserTemplateClone()
            : base(new[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" })
        {
            Assert.AreEqual(1, DbProxy.proxys.Count);
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking += DbProxy_Invoking;
        }

        private void DbProxy_Invoking(object sender, DbProxyInvokingEventArgs e)
        {
            _cloneInvoked |= e.ProxyMethodInfo.MethodName == "clone" && e.ProxyMethodInfo.TypeName == "vm";
        }

        protected override NewVMWizard NewWizard()
        {
            VM template = GetAnyUserTemplate();
            Assert.NotNull(template, "User template not found.");
            return new NewVMWizard(template.Connection, template, GetAnyHost());
        }

        protected override void RunAfter()
        {
            MWWaitFor(() => wizard.Action.IsCompleted && wizard.Action.Succeeded, "Wizard didn't succeed.");
            Assert.IsTrue(_cloneInvoked, "VM.Clone wasn't called when using a User Template with default storage settings.");
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking -= DbProxy_Invoking;

            List<VBD> vbds = wizard.Action.VM.Connection.ResolveAll(wizard.Action.VM.VBDs);
            Assert.IsTrue(vbds.TrueForAll(v => v.GetIsOwner() || v.IsCDROM()), "IsOwner wasn't set");
        }
    }


    [TestFixture]
    [Category(TestCategories.UICategoryB)]
    [Description("Tests that VM.Copy is called when using the New VM Wizard " +
                 "with a user-template when the user unchecks the clone checkbox.")]
    public class NewVMWizardTestUserTemplateCopy : WizardTest<NewVMWizard>
    {
        private bool _copyInvoked;

        public NewVMWizardTestUserTemplateCopy()
            : base(new[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" })
        {
            Assert.AreEqual(1, DbProxy.proxys.Count);
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking += DbProxy_Invoking;
        }

        private void DbProxy_Invoking(object sender, DbProxyInvokingEventArgs e)
        {
            _copyInvoked |= e.ProxyMethodInfo.MethodName == "copy" && e.ProxyMethodInfo.TypeName == "vm";
        }

        protected override NewVMWizard NewWizard()
        {
            VM template = GetAnyUserTemplate();
            Assert.NotNull(template, "User template not found.");
            return new NewVMWizard(template.Connection, template, GetAnyHost());
        }

        protected override void RunAfter()
        {
            MWWaitFor(() => wizard.Action.IsCompleted && wizard.Action.Succeeded, "Wizard didn't succeed.");
            Assert.IsTrue(_copyInvoked, "VM.Copy wasn't called when using a User Template when the user unchecked the clone checkbox.");
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking -= DbProxy_Invoking;

            List<VBD> vbds = wizard.Action.VM.Connection.ResolveAll(wizard.Action.VM.VBDs);
            Assert.IsTrue(vbds.TrueForAll(v => v.GetIsOwner() || v.IsCDROM()), "IsOwner wasn't set");
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Storage")
            {
                var cloneCheckBox = TestUtils.GetCheckBox(wizard, "page_6_Storage.CloneCheckBox");

                MW(delegate
                {
                    Assert.IsTrue(cloneCheckBox.Enabled, "Clone checkbox wasn't enabled.");
                    Assert.IsTrue(cloneCheckBox.Checked, "Clone checkbox wasn't checked.");

                    cloneCheckBox.Checked = false;
                });
            }

            base.TestPage(pageName);
        }
    }


    [TestFixture]
    [Category(TestCategories.UICategoryB)]
    [Description("Tests that VM.Copy is called when using the New VM Wizard " +
                 "with a user-template when one of the 2 target disks has a different SR.")]
    public class NewVMWizardTestUserTemplateCopy2 : NewVMWizardTest
    {
        private bool _cloneInvoked;

        public NewVMWizardTestUserTemplateCopy2()
            : base(new[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" })
        {
            Assert.AreEqual(1, DbProxy.proxys.Count);
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking += DbProxy_Invoking;
        }

        private void DbProxy_Invoking(object sender, DbProxyInvokingEventArgs e)
        {
            _cloneInvoked |= e.ProxyMethodInfo.MethodName == "clone" && e.ProxyMethodInfo.TypeName == "vm";
        }

        protected override NewVMWizard NewWizard()
        {
            VM template = GetAnyUserTemplate();
            Assert.NotNull(template, "User template not found.");
            return new NewVMWizard(template.Connection, template, GetAnyHost());
        }

        protected override void RunAfter()
        {
            MWWaitFor(() => wizard.Action.IsCompleted && wizard.Action.Succeeded, "Wizard didn't succeed.");
            Assert.IsTrue(_cloneInvoked, "VM.clone wasn't called when copying to disks of mixed SRs.");
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking -= DbProxy_Invoking;

            List<VBD> vbds = wizard.Action.VM.Connection.ResolveAll(wizard.Action.VM.VBDs);
            Assert.IsTrue(vbds.TrueForAll(v => v.GetIsOwner() || v.IsCDROM()), "IsOwner wasn't set");
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Storage")
            {
                var cloneCheckBox = TestUtils.GetCheckBox(wizard, "page_6_Storage.CloneCheckBox");
                var gridView = TestUtils.GetDataGridView(wizard, "page_6_Storage.DisksGridView");

                foreach (DiskGridRowItem item in gridView.Rows)
                {
                    ChangeDiskStorageToIscsi(item);
                    break; //change only one disk
                }

                Assert.IsTrue(cloneCheckBox.Enabled, "Clone checkbox wasn't enabled");
                Assert.IsTrue(cloneCheckBox.Checked, "Clone checkbox wasn't checked");
            }

            base.TestPage(pageName);
        }
    }


    [TestFixture]
    [Category(TestCategories.UICategoryB)]
    [Description("Tests that VM.Copy is called when using the New VM Wizard with a user-template " +
                 "and the user selects all disks to be on a storage different from the source.")]
    public class NewVMWizardTestUserTemplateCopy3 : NewVMWizardTest
    {
        private bool _copyInvoked;

        public NewVMWizardTestUserTemplateCopy3()
            : base(new[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" })
        {
            Assert.AreEqual(1, DbProxy.proxys.Count);
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking += DbProxy_Invoking;
        }

        private void DbProxy_Invoking(object sender, DbProxyInvokingEventArgs e)
        {
            _copyInvoked |= e.ProxyMethodInfo.MethodName == "copy" && e.ProxyMethodInfo.TypeName == "vm";
        }

        protected override NewVMWizard NewWizard()
        {
            VM template = GetAnyUserTemplate();
            Assert.NotNull(template, "User template not found.");
            return new NewVMWizard(template.Connection, template, GetAnyHost());
        }

        protected override void RunAfter()
        {
            MWWaitFor(() => wizard.Action.IsCompleted && wizard.Action.Succeeded, "Wizard didn't succeed.");
            Assert.IsTrue(_copyInvoked, "VM.Copy wasn't called when using a User Template with full-copy");
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking -= DbProxy_Invoking;

            List<VBD> vbds = wizard.Action.VM.Connection.ResolveAll(wizard.Action.VM.VBDs);
            Assert.IsTrue(vbds.TrueForAll(v => v.GetIsOwner() || v.IsCDROM()), "IsOwner wasn't set");
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Storage")
            {
                var cloneCheckBox = TestUtils.GetCheckBox(wizard, "page_6_Storage.CloneCheckBox");
                var gridView = TestUtils.GetDataGridView(wizard, "page_6_Storage.DisksGridView");

                foreach (DiskGridRowItem item in gridView.Rows)
                    ChangeDiskStorageToIscsi(item);

                Assert.IsFalse(cloneCheckBox.Enabled, "clone checkbox was enabled.");
                Assert.IsFalse(cloneCheckBox.Checked, "clone checkbox was checked.");
            }

            base.TestPage(pageName);
        }
    }


    [TestFixture]
    [Category(TestCategories.UICategoryB)]
    [Description("Tests that VM.clone is called when using the New VM Wizard " +
                 "with a default-template when the user selects default storage settings.")]
    public class NewVMWizardTestDefaultTemplate : WizardTest<NewVMWizard>
    {
        private bool _cloneInvoked;

        public NewVMWizardTestDefaultTemplate()
            : base(new[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" })
        {
            Assert.AreEqual(1, DbProxy.proxys.Count);
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking += DbProxy_Invoking;
        }

        private void DbProxy_Invoking(object sender, DbProxyInvokingEventArgs e)
        {
            _cloneInvoked |= e.ProxyMethodInfo.MethodName == "clone" && e.ProxyMethodInfo.TypeName == "vm";
        }

        protected override NewVMWizard NewWizard()
        {
            VM template = GetAnyDefaultTemplate(vm => vm.name_label.Contains("Etch"));
            Assert.NotNull(template, "Default template not found.");
            return new NewVMWizard(template.Connection, template, GetAnyHost());
        }

        protected override void RunAfter()
        {
            MWWaitFor(() => wizard.Action.IsCompleted && wizard.Action.Succeeded, "Wizard didn't succeed.");
            Assert.IsTrue(_cloneInvoked, "VM.clone wasn't called when using a default template with default storge settings");
            DbProxy.proxys[ConnectionsManager.XenConnectionsCopy[0]].Invoking -= DbProxy_Invoking;

            List<VBD> vbds = wizard.Action.VM.Connection.ResolveAll(wizard.Action.VM.VBDs);
            Assert.IsTrue(vbds.TrueForAll(v => v.GetIsOwner() || v.IsCDROM()), "IsOwner wasn't set");
        }
    }

    public abstract class NewVMWizardTest : WizardTest<NewVMWizard>
    {
        protected NewVMWizardTest(string[] pageNames, bool canFinish = true, bool doFinish = true)
            : base(pageNames, canFinish, doFinish)
        {
        }

        protected void ChangeDiskStorageToIscsi(DiskGridRowItem item)
        {
            var propertiesButton = TestUtils.GetButton(wizard, "page_6_Storage.PropertiesButton");

            ThreadPool.QueueUserWorkItem(delegate
            {
                MWWaitFor(() => wizard.OwnedForms.Length > 0);

                var dialog = wizard.OwnedForms.FirstOrDefault(w => w.Text == "Edit Disk");
                var srPicker = TestUtils.GetSrPicker(dialog, "SrListBox");
                var okButton = TestUtils.GetButton(dialog, "OkButton");

                MW(() =>
                {
                    srPicker.selectSRorDefaultorAny(GetAnySR(sr => sr.Name().Contains("SCSI")));
                    okButton.PerformClick();
                });
            });
            MW(() =>
            {
                item.Selected = true;
                propertiesButton.PerformClick();
            });

            MWWaitFor(() => wizard.Visible && wizard.CanFocus);
        }
    }
}

