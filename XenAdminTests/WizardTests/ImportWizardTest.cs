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

using System.IO;
using System.Threading;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Commands;
using XenAdmin.Wizards.ImportWizard;
using XenAPI;

namespace XenAdminTests.WizardTests.xapidb_app.ImportWizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    internal class ImportWizardTest_OVF : WizardTest<ImportWizard>
    {
        public ImportWizardTest_OVF()
            : base(new[] { "Import Source", "EULAs", "Location", "Storage", "Networking", "Security",
                "OS Fixup Settings", "Transfer VM Settings", "Finish" }, true, false)
        { }

        protected override ImportWizard NewWizard()
        {
            var host = GetAnyHost();
            return new ImportWizard(host.Connection, host, null, false);
        }

        protected override void RunBefore()
        {
            var toolsDir = Program.AssemblyDir + @"\External Tools";
            Directory.CreateDirectory(toolsDir);
            using (File.Create(toolsDir + @"\xenserver-linuxfixup-disk.iso"))
            {}
            using (File.Create(TestResource("sample_app.mf")))
            {}
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Import Source")
            {
                //select an ovf to import
                MW(() => (TestUtils.GetTextBox(wizard, "m_pageImportSource.m_textBoxFile")).Text = TestResource("sample_app.ovf"));
            }
            else if (pageName == "EULAs")
            {
                //accept EULAs
                MW(() => (TestUtils.GetCheckBox(wizard, "m_pageEula.m_checkBoxAccept")).Checked = true);
            }
            else if (pageName == "Location")
            {
                Thread.Sleep(2000);
                MW(() => (TestUtils.GetComboBox(wizard, "m_pageHost.m_comboBoxConnection")).SelectedIndex = 0);
            }
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    internal class ImportWizardTest_DiskImage : WizardTest<ImportWizard>
    {
        public ImportWizardTest_DiskImage()
            : base(new[] { "Import Source", "VM Definition", "Location", "Storage", "Networking",
                "OS Fixup Settings", "Transfer VM Settings", "Finish"}, true, false)
        { }

        protected override ImportWizard NewWizard()
        {
            var host = GetAnyHost();
            return new ImportWizard(host.Connection, host, null, false);
        }

        protected override void RunBefore()
        {
            var toolsDir = Program.AssemblyDir + @"\External Tools";
            Directory.CreateDirectory(toolsDir);
            using (File.Create(toolsDir + @"\xenserver-linuxfixup-disk.iso"))
            { }
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Import Source")
            {
                //select a disk image to import
                MW(() => (TestUtils.GetTextBox(wizard, "m_pageImportSource.m_textBoxFile")).Text = TestResource("disk-image.vhd"));
            }
            else if (pageName == "VM Definition")
            {
                //name the VM
                MW(() => (TestUtils.GetTextBox(wizard, "m_pageVMconfig.m_textBoxVMName")).Text = "myVM");
            }
            else if (pageName == "Location")
            {
                Thread.Sleep(2000);
                MW(() => (TestUtils.GetComboBox(wizard, "m_pageHost.m_comboBoxConnection")).SelectedIndex = 0);
            }
        }
    }
}
