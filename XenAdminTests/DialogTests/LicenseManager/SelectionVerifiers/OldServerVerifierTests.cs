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
    class OldServerVerifierTests : UnitTester_SingleConnectionTestFixture
    {
        [Test]
        [TestCase(0, 0, Status.Error)]
        [TestCase(1, 0, Status.OK)]
        [TestCase(0, 1, Status.OK)]
        [TestCase(1, 1, Status.Error)]
        [TestCase(2, 0, Status.OK)]
        [TestCase(0, 2, Status.Error)]
        [TestCase(2, 2, Status.Error)]
        public void CanActivateStatus(int canUseLicenseServer, int cannotUseLicenseServer, Status expected)
        {
            //Setup
            List<LicenseDataGridViewRow> data = CreateData(canUseLicenseServer, cannotUseLicenseServer);
            Assert.That(data.Count, Is.EqualTo(canUseLicenseServer + cannotUseLicenseServer), "Data not prepared properly");
            LicenseSelectionVerifier lv = new OlderServerVerifier(data);

            //Assert
            Assert.That(lv.Status, Is.EqualTo(Status.Unchecked), "Pre-Data validation status");
            lv.Verify();
            Assert.That(lv.Status, Is.EqualTo(expected),
                String.Format("Data validation: can use server = {0}, cannot use server = {1}", canUseLicenseServer, cannotUseLicenseServer));

            ObjectManager.ClearXenObjects(id);
        }

        [Test]
        [TestCase(0, 0, false)]
        [TestCase(1, 0, false)]
        [TestCase(0, 1, false)]
        [TestCase(1, 1, true)]
        [TestCase(2, 0, false)]
        [TestCase(0, 2, true)]
        [TestCase(2, 2, true)]
        public void CanActivateMessageSet(int canUseLicenseServer, int cannotUseLicenseServer, bool messageSet)
        {
            //Setup
            List<LicenseDataGridViewRow> data = CreateData(canUseLicenseServer, cannotUseLicenseServer);
            Assert.That(data.Count, Is.EqualTo(canUseLicenseServer + cannotUseLicenseServer), "Data not prepared properly");
            LicenseSelectionVerifier lv = new OlderServerVerifier(data);

            //Assert
            Assert.That(lv.Status, Is.EqualTo(Status.Unchecked), "Pre-Data validation status");
            lv.Verify();
            string message = lv.VerificationDetails();
            Assert.AreEqual(messageSet, !String.IsNullOrEmpty(message), 
                String.Format("Message set validation: can use server = {0}, cannot use server = {1}", canUseLicenseServer, cannotUseLicenseServer));

            ObjectManager.ClearXenObjects(id);
        }

        private List<LicenseDataGridViewRow> CreateData(int numberThatCanUseLicenseServer, int numberThatCannotUseLicenseServer)
        {
            List<LicenseDataGridViewRow> data = new List<LicenseDataGridViewRow>();
            data.AddRange(Enumerable.Repeat(CreateRow(true), numberThatCanUseLicenseServer));
            data.AddRange(Enumerable.Repeat(CreateRow(false), numberThatCannotUseLicenseServer));
            return data;
        }

        private LicenseDataGridViewRow CreateRow(bool canUseLicenseServer)
        {
            Mock<Host> host = ObjectManager.NewXenObject<Host>(id);
            string returnValue = String.Empty;
            if (canUseLicenseServer)
                returnValue = "yup";

            host.Setup(h => h.edition).Returns(returnValue);
            return new LicenseDataGridViewRow(host.Object);
        }
    }
}
