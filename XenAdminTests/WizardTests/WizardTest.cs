/* Copyright (c) Citrix Systems Inc. 
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
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin;
using XenAdmin.Controls;
using XenAdmin.Wizards;
using XenAdmin.Commands;

namespace XenAdminTests.WizardTests
{
    public abstract class WizardTest<T> : MainWindowTester where T : XenWizardBase
    {
        protected T wizard;
        private string[] pageNames;
        private bool canFinish;
        private bool doFinish;

        protected Button btnNext;
        protected Button btnPrevious;
        protected Button btnCancel;

        protected WizardTest(string[] pageNames, bool canFinish, bool doFinish)
        {
            this.pageNames = pageNames;
            this.canFinish = canFinish;
            this.doFinish = doFinish;
        }

        [Test]
        [Timeout(100 * 1000)]
        public void RunWizardTests()
        {
            RunBefore();

            wizard = MW<T>(NewWizard);
            MW(wizard.Show);

            btnNext = TestUtils.GetButton(wizard, "buttonNext");
            btnPrevious = TestUtils.GetButton(wizard, "buttonPrevious");
            btnCancel = TestUtils.GetButton(wizard, "buttonCancel");

            for (int i = 0; i < pageNames.Length; ++i)
            {
                bool lastPage = (i == pageNames.Length - 1);

                // Check we're on the right tab
                Assert.AreEqual(pageNames[i], CurrentPageName(wizard), "Wrong page name");

                // Check that we have help
                Assert.IsTrue(wizard.HasHelp(), "Help missing for page: " + pageNames[i]);

                // Any specific setup or testing for this page defined in derived class
                TestPage(pageNames[i]);

                // Check button enablement
                //Assert.AreEqual(i != 0, btnPrevious.Enabled, "Previous button wrong on page: " + pageNames[i]);
                if (lastPage)
                    Assert.AreEqual(canFinish, btnNext.Enabled, "Next button wrong on page: " + pageNames[i]);
                else
                    Assert.AreEqual(true, btnNext.Enabled, "Next button wrong on page: " + pageNames[i]);
                Assert.AreEqual(lastPage && !CanCancelLastPage, !btnCancel.Enabled, "Cancel button wrong on page: " + pageNames[i]);

                // Test that Next takes us forward and Prev takes us back here
                if (!lastPage)
                {
                    MW(btnNext.PerformClick);
                    Assert.AreEqual(pageNames[i + 1], CurrentPageName(wizard), "Next button didn't get from page: " + pageNames[i] + " to page: " + pageNames[i + 1]);

                    
                    if (btnPrevious.Enabled)
                    {
                        MW(btnPrevious.PerformClick);
                        Assert.AreEqual(pageNames[i], CurrentPageName(wizard), "Next then Previous didn't get back to page: " + pageNames[i]);
                        while (!btnNext.Enabled)
                            Thread.Sleep(1000);
                        MW(btnNext.PerformClick);
                    }
                }
                else
                {
                    if (doFinish)
                        MW(btnNext.PerformClick);
                    else
                        MW(btnCancel.PerformClick);
                }
            }

            // Any subsequent testing defined in derived class
            RunAfter();

            MW(() => wizard.Dispose());
        }

        private string CurrentPageName(T wizard)
        {
            return wizard.CurrentStepTabPage.Text;
        }

        protected abstract T NewWizard();

        protected virtual void RunBefore()
        {
        }

        protected virtual void RunAfter()
        {
        }

        protected virtual void TestPage(string pageName)
        {
        }

        protected virtual bool CanCancelLastPage
        {
            get { return true; }
        }
    }
}

namespace XenAdminTests.WizardTests.state1_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("state1.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.state4_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("state4.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.cowley1_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("cowley1.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.updatesState_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("updatesState.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.cowleyPolicies_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("cowleyPolicies.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.state5_xml
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("state5.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.small_vms
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("small_vms.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.xapidb_app
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("xapidb_app.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.boston_db
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("boston-db.xml")
        { }
    }
}

namespace XenAdminTests.WizardTests.tampa_cpm_one_and_two_host_pools
{
    [SetUpFixture]
    public class WizardTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public WizardTestSetUp()
            : base("tampa_poolofone_40.xml", "tampa-poolof16and23-xapi-db.xml")
        { }
    }
}