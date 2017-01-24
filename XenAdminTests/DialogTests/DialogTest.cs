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
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Dialogs;

namespace XenAdminTests.DialogTests
{
    public abstract class DialogTest<T>: MainWindowTester where T: XenDialogBase
    {
        protected T dialog;

        [Test]
        public void RunDialogTests()
        {
            RunBefore();

            dialog = MW<T>(NewDialog);

            RunBeforeShow();

            MW(dialog.Show);

            // Check that if we have a help button, we have help
            if (dialog.HelpButton)
                Assert.IsTrue(dialog.HasHelp(), "Help missing");

            // Any subsequent testing defined in derived class
            RunAfter();

            MW(() =>
                {
                    dialog.Close();
                    dialog.Dispose();
                });
        }

        protected abstract T NewDialog();

        protected virtual void RunBefore()
        {
        }

        protected virtual void RunBeforeShow()
        {
        }

        protected virtual void RunAfter()
        {
        }
    }
}

namespace XenAdminTests.DialogTests.state1_xml
{
    [SetUpFixture]
    public class DialogTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public DialogTestSetUp()
            : base("state1.xml")
        { }
    }
}

namespace XenAdminTests.DialogTests.boston
{
    [SetUpFixture]
    public class DialogTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public DialogTestSetUp()
            : base("boston-db.xml")
        { }
    }
}

namespace XenAdminTests.DialogTests.state2_xml
{
    [SetUpFixture]
    public class DialogTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public DialogTestSetUp()
            : base("state2.xml")
        { }
    }
}

namespace XenAdminTests.DialogTests.cowleyPolicies_xml
{
    [SetUpFixture]
    public class DialogTestSetUp : MainWindowLauncher_SetUpFixture
    {
        public DialogTestSetUp()
            : base("cowleyPolicies.xml")
        { }
    }
}