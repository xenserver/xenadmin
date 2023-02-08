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

using System.Linq;
using System.Threading;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Dialogs;
using XenAdmin.Wizards.PatchingWizard;
using XenAdmin.Wizards.RollingUpgradeWizard;

namespace XenAdminTests.WizardTests.state5_xml
{

    [TestFixture, Category(TestCategories.UICategoryB), Category(TestCategories.SmokeTest)]
    class RollingUpgradeWizardTest : WizardTest<RollingUpgradeWizard>
    {
        public RollingUpgradeWizardTest()
            : base(new[] {"Before You Start", "Select Pools", "Upgrade Mode", "Upgrade Options", "Prechecks", "Apply Upgrade"}, doFinish: false)
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
                    var btnResolveAll = TestUtils.GetButton(wizard, "RollingUpgradeWizardPrecheckPage.buttonResolveAll");
                    if (btnResolveAll.Enabled)
                    {
                        MW(btnResolveAll.PerformClick);
                        Thread.Sleep(1000);
                    }
                    Assert.IsFalse(btnResolveAll.Enabled, "Upgrade prechecks failed.");
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
                
            }
            else if (pageName == "Upgrade Mode")
            {
                MW(TestUtils.GetRadioButton(wizard, "RollingUpgradeWizardUpgradeModePage.radioButtonManual").PerformClick);
            }
            else if (pageName == "Apply Upgrade")
            {
                MWWaitFor(() => wizard.OwnedForms.Length > 0);

                MW(() =>
                {
                    var dialog = wizard.OwnedForms.FirstOrDefault(w =>
                        w.Text == Messages.ROLLING_POOL_UPGRADE && w is NonModalThreeButtonDialog);

                    Assert.NotNull(dialog, "Manual upgrade prompt was not found");
                    dialog.CancelButton.PerformClick();
                });

                //since we clicked cancel the retry button will become visible
                var retryBtn = TestUtils.GetButton(wizard, "RollingUpgradeUpgradePage.buttonRetry");

                MWWaitFor(() => wizard.Visible && wizard.CanFocus &&
                                retryBtn.Visible && retryBtn.Enabled);
            }
        }

        protected override bool IsCancelButtonEnabledOnLastPage
        {
            get
            {
                var page = TestUtils.GetXenTabPage(wizard, "RollingUpgradeUpgradePage") as RollingUpgradeUpgradePage;
                return page.Status == Status.NotStarted || page.Status == Status.Started;
            }
        }
    }
}
