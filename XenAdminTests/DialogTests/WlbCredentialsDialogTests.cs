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

using System.Reflection;
using System.Threading;
using NUnit.Framework;
using XenAdmin.Dialogs.Wlb;

namespace XenAdminTests.DialogTests.boston.WlbCredentialsDialogTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class WlbCredentialsDialogTests : DialogTest<WlbCredentialsDialog>
    {
        private struct TestData
        {
            public string URL;
            public string WlbUserName;
            public string WlbPassword;
            public string XSUserName;
            public string XSPassword;
            public bool UseCurrentXSCredentials;
            public string TestComment;
        }
        protected override WlbCredentialsDialog NewDialog()
        {
            return new WlbCredentialsDialog(GetAnyPool());
        }

        protected override void RunAfter()
        {
            //Change the contents of the dialog text boxes to test the OK button enablement
            CheckOKButtonEnablementAllTextFieldsEntered();
            CheckOKButtonEnablementXenServerCredentialsChecked();
            CheckOKButtonEnablementTestUrlMissing();
            CheckOKButtonEnablementWlbPasswordMissing();
            CheckOKButtonEnablementXSPasswordMissing();
        }


        private void CheckOKButtonEnablementAllTextFieldsEntered()
        {
            OKButtonStateTest(new TestData()
                                  {
                                      URL = "test.url",
                                      WlbUserName = "me",
                                      WlbPassword = "password",
                                      XSUserName = "me",
                                      XSPassword = "password",
                                      UseCurrentXSCredentials = false,
                                      TestComment = MethodBase.GetCurrentMethod().ToString()
                                  }, true);
        }

        private void CheckOKButtonEnablementWlbPasswordMissing()
        {
            OKButtonStateTest(new TestData()
            {
                URL = "test.url",
                WlbUserName = "me",
                WlbPassword = string.Empty,
                XSUserName = "me",
                XSPassword = "password",
                UseCurrentXSCredentials = false,
                TestComment = MethodBase.GetCurrentMethod().ToString()
            }, false);
        }

        private void CheckOKButtonEnablementXSPasswordMissing()
        {
            OKButtonStateTest(new TestData()
            {
                URL = "test.url",
                WlbUserName = "me",
                WlbPassword = "password",
                XSUserName = "me",
                XSPassword = string.Empty,
                UseCurrentXSCredentials = false,
                TestComment = MethodBase.GetCurrentMethod().ToString()
            }, false);
        }

        private void CheckOKButtonEnablementTestUrlMissing()
        {
            OKButtonStateTest(new TestData()
            {
                URL = string.Empty,
                WlbUserName = "me",
                WlbPassword = "password",
                XSUserName = "me",
                XSPassword = "password",
                UseCurrentXSCredentials = false,
                TestComment = MethodBase.GetCurrentMethod().ToString()
            }, false);
        }

        private void CheckOKButtonEnablementXenServerCredentialsChecked()
        {
            OKButtonStateTest(new TestData()
            {
                URL = "test.url",
                WlbUserName = "me",
                WlbPassword = "password",
                XSUserName = string.Empty,
                XSPassword = string.Empty,
                UseCurrentXSCredentials = true,
                TestComment = MethodBase.GetCurrentMethod().ToString()
            }, false);
        }

        private void OKButtonStateTest( TestData data, bool expectedState )
        {
            MW(delegate
                   {
                       SetupDialog(data);
                       Assert.AreEqual(expectedState, TestUtils.GetButton(dialog, "buttonOK").Enabled, "OK button state: " + data.TestComment);
                   });
        }

        private void SetupDialog(TestData data)
        {
            TestUtils.GetTextBox(dialog, "textboxWlbUrl").Text = data.URL;
            TestUtils.GetTextBox(dialog, "textboxWlbUserName").Text = data.WlbUserName;
            TestUtils.GetTextBox(dialog, "textboxWlbPassword").Text = data.WlbPassword;
            TestUtils.GetTextBox(dialog, "textboxXSUserName").Text = data.XSUserName;
            TestUtils.GetTextBox(dialog, "textboxXSPassword").Text = data.XSPassword;
            TestUtils.GetCheckBox(dialog, "checkboxUseCurrentXSCredentials").Checked = data.UseCurrentXSCredentials;
            //Wait for event trigger
            Thread.Sleep(100);
        }
    }
}