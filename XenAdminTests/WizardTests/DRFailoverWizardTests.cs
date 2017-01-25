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
using XenAdmin.Wizards.DRWizards;

namespace XenAdminTests.WizardTests.boston_db.DRWizardTests
{
    /* 
     * TODO: Finish of DR wizard tests if it becomes possible to load the mirrored SR states from the iSCSI
     * device while using a xml database to drive the test
     */
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class DRFailoverWizardTest_FailoverMode : WizardTest<DRFailoverWizard>
    {
        public DRFailoverWizardTest_FailoverMode()
            : base(new[] { "Welcome", "Before You Start", "Locate Mirrored SRs" 
                           /* , "Select vApps & VMs", "Pre-checks", "Progress", "Summary"*/ }, false, false)
        { }

        protected override DRFailoverWizard NewWizard()
        {
            return new DRFailoverWizard(GetAnyPoolOfOne());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Welcome")
            {
                MW( delegate 
                {
                    TestUtils.GetRadioButton(wizard, "DRFailoverWizardWelcomePage.radioButtonFailover").Checked = true;
                    Assert.IsTrue(btnNext.Enabled, "Next button is enabled");
                } );
            }

            if (pageName == "Locate Mirrored SRs")
            {
                MW(delegate
                {
                    Assert.IsTrue(TestUtils.GetButton(wizard, "DRFailoverWizardStoragePage1.FindSrsButton").Enabled);
                });
            }

        }
    }


    [TestFixture, Category(TestCategories.UICategoryB)]
    public class DRFailoverWizardTest_FailbackMode : WizardTest<DRFailoverWizard>
    {
        public DRFailoverWizardTest_FailbackMode()
            : base(new[] { "Welcome", "Before You Start", "Locate Mirrored SRs" 
                           /*, "Select vApps & VMs", "Pre-checks", "Progress", "Summary"*/ }, false, false)
        { }

        protected override DRFailoverWizard NewWizard()
        {
            return new DRFailoverWizard(GetAnyPoolOfOne());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Welcome")
            {
                MW(delegate
                {
                    TestUtils.GetRadioButton(wizard, "DRFailoverWizardWelcomePage.radioButtonFailback").Checked = true;
                    Assert.IsTrue(btnNext.Enabled, "Next button is enabled");
                });

                if (pageName == "Locate Mirrored SRs")
                {
                    MW(delegate
                    {
                        Assert.IsTrue(TestUtils.GetButton(wizard, "DRFailoverWizardStoragePage1.FindSrsButton").Enabled);
                    });
                }
            }

        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class DRFailoverWizardTest_DryRunMode : WizardTest<DRFailoverWizard>
    {
        public DRFailoverWizardTest_DryRunMode()
            : base(new[] { "Welcome", "Before You Start", "Locate Mirrored SRs" 
                           /*, "Select vApps & VMs", "Pre-checks", "Progress", "Summary"*/ }, false, false)
        { }

        protected override DRFailoverWizard NewWizard()
        {
            return new DRFailoverWizard(GetAnyPoolOfOne());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Welcome")
            {
                MW( delegate 
                {
                    TestUtils.GetRadioButton(wizard, "DRFailoverWizardWelcomePage.radioButtonDryrun").Checked = true;
                    Assert.IsTrue(btnNext.Enabled, "Next button is enabled");
                } );
            }

            if (pageName == "Locate Mirrored SRs")
            {
                MW(delegate
                {
                    Assert.IsTrue(TestUtils.GetButton(wizard, "DRFailoverWizardStoragePage1.FindSrsButton").Enabled);
                });
            }

        }
    }
}
