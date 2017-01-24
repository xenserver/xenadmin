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

using NUnit.Framework;
using XenAdmin.Wizards.NewVMWizard;
using XenAPI;

namespace XenAdminTests.WizardTests.state1_xml
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardTestDisklessVM : WizardTest<NewVMWizard>
    {

        public NewVMWizardTestDisklessVM()
            : base(new string[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" }, true, true)
        {
        }


        protected override NewVMWizard NewWizard()
        {
            VM template = GetAnyTemplate(vm => vm.IsHVM);
            Assert.NotNull(template, "User template not found.");
            return new NewVMWizard(template.Connection, template, GetAnyHost());
        }

        protected override void RunAfter()
        {
            MWWaitFor(() => wizard.Action.IsCompleted && wizard.Action.Succeeded, "Wizard didn't succeed.");
           
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Installation Media")
            {
                MW(() => (TestUtils.GetRadioButton(wizard, "page_3_InstallationMedia.UrlRadioButton")).Checked = true);
            }

            if (pageName == "Storage")
            {
                MW(() => (TestUtils.GetRadioButton(wizard, "page_6_Storage.DisklessVMRadioButton")).Checked = true);
            }

            base.TestPage(pageName);
        }
    }
}
