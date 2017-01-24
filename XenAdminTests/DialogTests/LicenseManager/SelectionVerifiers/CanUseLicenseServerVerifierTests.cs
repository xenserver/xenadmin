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
using System.Linq;
using Moq;
using NUnit.Framework;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.LicenseManagerSelectionVerifiers;
using XenAPI;
using Status = XenAdmin.Dialogs.LicenseManagerSelectionVerifiers.LicenseSelectionVerifier.VerificationStatus;

namespace XenAdminTests.DialogTests.LicenseManager.SelectionVerifiers
{
    public class CanUseLicenseServerVerifierTests : UnitTester_SingleConnectionTestFixture
    {
        [Test, Category(TestCategories.SmokeTest)]
        [TestCase(0, 0, Status.Error)]
        [TestCase(1, 0, Status.OK)]
        [TestCase(0, 1, Status.Error)]
        [TestCase(2, 0, Status.Error)]
        [TestCase(0, 2, Status.Error)]
        [TestCase(2, 2, Status.Error)]
        [TestCase(1, 1, Status.Error)]
        public void CanActivateStatus(int canActivate, int cannotActivate, Status expected)
        {
            //Setup
            List<LicenseDataGridViewRow> data = CreateData(canActivate, cannotActivate);
            Assert.That(data.Count, Is.EqualTo(canActivate + cannotActivate), "Data not prepared properly");
            LicenseSelectionVerifier lv = new CanUseLicenseServerVerifier(data);

            //Assert
            Assert.That(lv.Status, Is.EqualTo(Status.Unchecked), "Pre-Data validation status");
            lv.Verify();
            Assert.That(lv.Status, Is.EqualTo(expected), 
                String.Format("Data validation: can activate = {0}, cannot activate = {1}", canActivate, cannotActivate));

            ObjectManager.ClearXenObjects(id);
        }

        private List<LicenseDataGridViewRow> CreateData(int numberThatCanActivate, int numberThatCannotActivate)
        {
            List<LicenseDataGridViewRow> data = new List<LicenseDataGridViewRow>();
            data.AddRange(Enumerable.Repeat(CreateRow(true), numberThatCanActivate));
            data.AddRange(Enumerable.Repeat(CreateRow(false), numberThatCannotActivate));
            return data;
        }

        private LicenseDataGridViewRow CreateRow(bool canActivate)
        {
            Mock<Host> host = ObjectManager.NewXenObject<Host>(id);
            string returnValue = String.Empty;
            if (canActivate)
                returnValue = "yup";

            host.Setup(h => h.edition).Returns(returnValue);
            return new LicenseDataGridViewRow(host.Object);
        }
    }
}
