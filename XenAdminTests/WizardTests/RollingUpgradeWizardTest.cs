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

using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Wizards.RollingUpgradeWizard;

namespace XenAdminTests.WizardTests.state5_xml
{

    [TestFixture, Category(TestCategories.UICategoryB), Category(TestCategories.SmokeTest)]
    class RollingUpgradeWizardTest : WizardTest<RollingUpgradeWizard>
    {
        public RollingUpgradeWizardTest()
            : base(new string[] { "Before You Start", "Select Pools", "Upgrade Mode", "Prechecks","Ready to Upgrade","Apply Upgrade" }
            , true, false)
        { }

        protected override RollingUpgradeWizard NewWizard()
        {
            return new RollingUpgradeWizard();
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Select Pools")
            {
                MW(TestUtils.GetButton(wizard, "RollingUpgradeWizardSelectPool.buttonSelectAll").PerformClick);
            }
            else if (pageName == "Prechecks")
            {
                while (!btnNext.Enabled)
                {
                    Assert.IsFalse(TestUtils.GetButton(wizard, "RollingUpgradeWizardPrecheckPage.buttonResolveAll").Enabled, "Upgrade prechecks failed.");
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
                
            }
            else if (pageName == "Apply Upgrade")
            {
                //It needs work
                var page = TestUtils.GetXenTabPage(wizard, "RollingUpgradeUpgradePage") as RollingUpgradeUpgradePage;
                while (page.Dialog == null)
                {
                    Thread.Sleep(500);
                }
                MW(page.Dialog.CancelButton.PerformClick);
                while (page.EnableNext() == false)
                {
                    Thread.Sleep(500);
                }
            }
            else if (pageName == "Upgrade Mode")
            {
                MW(TestUtils.GetRadioButton(wizard, "RollingUpgradeWizardUpgradeModePage.radioButtonManual").PerformClick);
            }
        }

        protected override bool CanCancelLastPage
        {
            get
            {
                var page = TestUtils.GetXenTabPage(wizard, "RollingUpgradeUpgradePage") as RollingUpgradeUpgradePage;
                return page.UpgradeStatus == RollingUpgradeStatus.NotStarted || page.UpgradeStatus == RollingUpgradeStatus.Started;
            }
        }
    }
}
