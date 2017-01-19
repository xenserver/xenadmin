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

using System.Collections.Generic;
using NUnit.Framework;
using XenAdmin.Commands.Controls;
using XenAPI;

namespace XenAdminTests.DialogTests.boston
{
    
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class MultipleWarningDialogTwoStringCtorTest : DialogTest<MultipleWarningDialog>
    {
        protected override MultipleWarningDialog NewDialog()
        {
            return new MultipleWarningDialog("Title", "Message");
        }

        protected override void RunAfter()
        {
            Assert.AreEqual("Message", TestUtils.GetLabel(dialog, "labelMessage").Text);
            Assert.AreEqual("&Proceed", TestUtils.GetButton(dialog, "buttonProceed").Text);
            Assert.AreEqual("Cancel", TestUtils.GetButton(dialog, "buttonCancel").Text);
            Assert.AreEqual(0, TestUtils.GetDataGridView(dialog, "dataGridViewWarnings").RowCount);
            Assert.AreEqual(0, TestUtils.GetDataGridView(dialog, "dataGridViewAppliesTo").RowCount);
            var button = TestUtils.GetButton(dialog, "buttonCancel");
            MW(button.PerformClick);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class MultipleWarningDialogThreeStringCtorTest : DialogTest<MultipleWarningDialog>
    {
        protected override MultipleWarningDialog NewDialog()
        {
            return new MultipleWarningDialog("Title", "Message", "Button");
        }

        protected override void RunAfter()
        {
            Assert.AreEqual("Message", TestUtils.GetLabel(dialog, "labelMessage").Text);
            Assert.AreEqual("Button", TestUtils.GetButton(dialog, "buttonProceed").Text);
            Assert.AreEqual("Cancel", TestUtils.GetButton(dialog, "buttonCancel").Text);
            Assert.AreEqual(0, TestUtils.GetDataGridView(dialog, "dataGridViewWarnings").RowCount);
            Assert.AreEqual(0, TestUtils.GetDataGridView(dialog, "dataGridViewAppliesTo").RowCount);
            var button = TestUtils.GetButton(dialog, "buttonProceed");
            MW(button.PerformClick);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class MultipleWarningDialogThreeStringAddedWarningsTest : DialogTest<MultipleWarningDialog>
    {
        protected override MultipleWarningDialog NewDialog()
        {
            MultipleWarningDialog d = new MultipleWarningDialog("Title", "Message", "Button");
            d.AddWarningMessage("Action", "Boom!!!", new List<IXenObject>(){GetAnyVM()});
            return d;
        }

        protected override void RunAfter()
        {
            Assert.AreEqual("Message", TestUtils.GetLabel(dialog, "labelMessage").Text);
            Assert.AreEqual("Button", TestUtils.GetButton(dialog, "buttonProceed").Text);
            Assert.AreEqual("Cancel", TestUtils.GetButton(dialog, "buttonCancel").Text);
            Assert.AreEqual(1, TestUtils.GetDataGridView(dialog, "dataGridViewWarnings").RowCount);
            Assert.AreEqual(1, TestUtils.GetDataGridView(dialog, "dataGridViewAppliesTo").RowCount);
            
        }
    }
}