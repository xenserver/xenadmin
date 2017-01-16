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
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Commands;
using XenAdmin.Wizards.ExportWizard;
using XenAPI;

namespace XenAdminTests.WizardTests.small_vms.ExportWizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    internal class ExportWizardTest_OVF : WizardTest<ExportApplianceWizard>
    {
        public ExportWizardTest_OVF()
            : base(new[] { "Export File Details", "Virtual Machines", "EULAs", "Advanced Options", "Transfer VM Settings", "Finish" }, true, false)
        {}

        protected override ExportApplianceWizard NewWizard()
        {
            var theVm = GetAnyVM(vm => vm.power_state == vm_power_state.Halted);
            return new ExportApplianceWizard(theVm.Connection, new SelectedItemCollection(new SelectedItem(theVm)));
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Export File Details")
            {
                //type an export location
                MW(() => (TestUtils.GetTextBox(wizard, "m_pageExportAppliance.m_textBoxFolderName")).Text = Path.GetTempPath());
            }
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    internal class ExportWizardTest_XVA : WizardTest<ExportApplianceWizard>
    {
        public ExportWizardTest_XVA()
            : base(new[] { "Export File Details", "Virtual Machines", "Finish" }, true, false)
        { }

        protected override ExportApplianceWizard NewWizard()
        {
            var theVm = GetAnyVM(vm => vm.power_state == vm_power_state.Halted);
            return new ExportApplianceWizard(theVm.Connection, new SelectedItemCollection(new SelectedItem(theVm)));
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Export File Details")
            {
                //type an export location
                MW(() => (TestUtils.GetTextBox(wizard, "m_pageExportAppliance.m_textBoxFolderName")).Text = Path.GetTempPath());
                //set export format to xva
                MW(() => (TestUtils.GetComboBox(wizard, "m_pageExportAppliance.m_comboBoxFormat")).SelectedIndex = 1);
            }
        }
    }
}
