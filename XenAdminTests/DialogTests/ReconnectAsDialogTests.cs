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

using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Dialogs;

namespace XenAdminTests.DialogTests.boston.ReconnectAsDialogTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class ReconnectAsDialogTests : DialogTest<ReconnectAsDialog>
    {
        protected override ReconnectAsDialog NewDialog()
        {
            return new ReconnectAsDialog(GetAnyConnection());
        }

        protected override void RunAfter()
        {
            Button okButton = TestUtils.GetButton(dialog, "buttonOK");
            Button cancelButton = TestUtils.GetButton(dialog, "buttonCancel");
            Assert.IsFalse(okButton.Enabled);
            Assert.IsTrue(cancelButton.Enabled);

            var textBox = TestUtils.GetTextBox(dialog, "textBoxUsername");
            MW(delegate { textBox.Text = "username"; });
            textBox = TestUtils.GetTextBox(dialog, "textBoxPassword");
            MW(delegate { textBox.Text = "password"; });

            Assert.IsTrue(okButton.Enabled);
            Assert.IsTrue(cancelButton.Enabled);
        }
    }
}