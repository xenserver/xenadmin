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
using XenAdmin.Wizards.NewVMWizard;
using XenAPI;
using XenAdmin.ServerDBs;
using XenAdmin;
using System.Threading;
using XenAdminTests.CommandTests;
using XenAdmin.Actions.VMActions;

namespace XenAdminTests.WizardTests.state5_xml
{
    /// <summary>
    /// Tests that VM.Clone is called when using the New VM Wizard with a user template with default storage settings.
    /// </summary>
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardTestUserTemplateClone : WizardTest<NewVMWizard>
    {
        private bool _cloneInvoked;

        public NewVMWizardTestUserTemplateClone()
            : base(new string[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" }, true, true)
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
            Assert.IsTrue(vbds.TrueForAll(v => v.IsOwner || v.IsCDROM), "IsOwner wasn't set");
        }
    }

    /// <summary>
    /// Tests that VM.Copy is called when using the New VM Wizard with a user-template when the user unchecks the clone checkbox.
    /// </summary>
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardTestUserTemplateCopy : WizardTest<NewVMWizard>
    {
        private bool _copyInvoked;

        public NewVMWizardTestUserTemplateCopy()
            : base(new string[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" }, true, true)
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
            Assert.IsTrue(vbds.TrueForAll(v => v.IsOwner || v.IsCDROM), "IsOwner wasn't set");
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Storage")
            {
                var cloneCheckBosx = TestUtils.GetCheckBox(wizard, "page_6_Storage.CloneCheckBox");

                MW(delegate
                {
                    Assert.IsTrue(cloneCheckBosx.Enabled, "Clone checkbox wasn't enabled.");
                    Assert.IsTrue(cloneCheckBosx.Checked, "Clone checkbox wasn't checked.");

                    cloneCheckBosx.Checked = false;
                });
            }

            base.TestPage(pageName);
        }
    }

    /// <summary>
    /// Tests that VM.Copy is called when using the New VM Wizard with a user-template when one of the 2 target disks has a
    /// different SR.
    /// </summary>
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardTestUserTemplateCopy2 : WizardTest<NewVMWizard>
    {
        private bool _cloneInvoked;

        public NewVMWizardTestUserTemplateCopy2()
            : base(new string[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" }, true, true)
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
            Assert.IsTrue(vbds.TrueForAll(v => v.IsOwner || v.IsCDROM), "IsOwner wasn't set");
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Storage")
            {
                var cloneCheckBosx = TestUtils.GetCheckBox(wizard, "page_6_Storage.CloneCheckBox");
                var propertiesButton = TestUtils.GetButton(wizard, "page_6_Storage.PropertiesButton");

                HandleModalDialog<NewDiskDialogWrapper>("Edit Disk", propertiesButton.PerformClick,
                    delegate(NewDiskDialogWrapper w)
                    {
                        w.SrListBox.selectSRorDefaultorAny(GetAnySR(sr => sr.Name.Contains("SCSI"))); //switch storage for new disk
                        w.OkButton.PerformClick();
                    });

                Assert.IsTrue(cloneCheckBosx.Enabled, "Clone checkbox wasn't enabled");
                Assert.IsTrue(cloneCheckBosx.Checked, "Clone checkbox wasn't checked");
            }

            base.TestPage(pageName);
        }
    }

    /// <summary>
    /// Tests that VM.Copy is called when using the New VM Wizard with a user-template when the user selects all disks to be a different
    /// storage than the source.
    /// </summary>
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardTestUserTemplateCopy3 : WizardTest<NewVMWizard>
    {
        private bool _copyInvoked;

        public NewVMWizardTestUserTemplateCopy3()
            : base(new string[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" }, true, true)
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
            Assert.IsTrue(vbds.TrueForAll(v => v.IsOwner || v.IsCDROM), "IsOwner wasn't set");
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Storage")
            {
                var cloneCheckBox = TestUtils.GetCheckBox(wizard, "page_6_Storage.CloneCheckBox");
                var propertiesButton = TestUtils.GetButton(wizard, "page_6_Storage.PropertiesButton");
                var gridView = TestUtils.GetDataGridView(wizard, "page_6_Storage.DisksGridView");

                foreach (DiskGridRowItem item in gridView.Rows)
                {
                    MW(() => item.Selected = true);

                    HandleModalDialog<NewDiskDialogWrapper>("Edit Disk", propertiesButton.PerformClick,
                        delegate(NewDiskDialogWrapper w)
                        {
                            w.SrListBox.selectSRorDefaultorAny(GetAnySR(sr => sr.Name.Contains("SCSI"))); //switch storage for new disk
                            w.OkButton.PerformClick();
                        });
                }

                MW(delegate
                {
                    Assert.IsFalse(cloneCheckBox.Enabled, "clone checkbox was enabled.");
                    Assert.IsFalse(cloneCheckBox.Checked, "clone checkbox was checked.");
                });
            }

            base.TestPage(pageName);
        }
    }

    /// <summary>
    /// Tests that VM.clone is called when using the New VM Wizard with a default-template when the user selects default storage settings.
    /// </summary>
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardTestDefaultTemplate : WizardTest<NewVMWizard>
    {
        private bool _cloneInvoked;

        public NewVMWizardTestDefaultTemplate()
            : base(new string[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" }, true, true)
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
            Assert.IsTrue(vbds.TrueForAll(v => v.IsOwner || v.IsCDROM), "IsOwner wasn't set");
        }
    }
}

