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
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdminTests.DialogTests.boston.RestoreVMsDialogTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class RestoreVMsDialogTests : DialogTest<RestoreVMsDialog>
    {
        protected override RestoreVMsDialog NewDialog()
        {
            Host host = GetAnyHost();
            return new RestoreVMsDialog(new List<VM>( host.Connection.Cache.VMs), host );
        }

        protected override void RunAfter()
        {
            Assert.AreEqual(58, TestUtils.GetDataGridView(dialog, "dataGridViewVms").Rows.Count);
            Assert.IsTrue(TestUtils.GetButton(dialog, "buttonNo").Enabled);
            Button yesButton = TestUtils.GetButton(dialog, "buttonYes");
            Assert.IsTrue(yesButton.Enabled);
            MW(yesButton.PerformClick);
        }
    }
}