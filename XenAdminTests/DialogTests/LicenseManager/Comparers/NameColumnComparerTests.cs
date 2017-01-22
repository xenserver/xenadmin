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
using System.Text;
using Moq;
using NUnit.Framework;
using XenAdmin.Controls.CheckableDataGridView;
using XenAdmin.Dialogs.LicenseManagerLicenseRowComparers;
using XenAPI;

namespace XenAdminTests.DialogTests.LicenseManager.Comparers
{
    public class NameColumnComparerTests : UnitTester_SingleConnectionTestFixture
    {
        [Test]
        public void CompareStrings()
        {
            Mock<Host> hx = ObjectManager.NewXenObject<Host>(id);
            hx.Setup(h => h.Name).Returns("A");
            Mock<Host> hy = ObjectManager.NewXenObject<Host>(id);
            hy.Setup(h => h.Name).Returns("A");

            Mock<CheckableDataGridViewRow> x = new Mock<CheckableDataGridViewRow>(MockBehavior.Strict);
            x.Setup(r => r.XenObject).Returns(hx.Object);
            Mock<CheckableDataGridViewRow> y = new Mock<CheckableDataGridViewRow>(MockBehavior.Strict);
            y.Setup(r => r.XenObject).Returns(hy.Object);

            NameColumnComparer comparer = new NameColumnComparer();
            Assert.That(comparer.Compare(x.Object, y.Object), Is.EqualTo(0));
            x.VerifyAll();
            y.VerifyAll();
        }
    }
}
