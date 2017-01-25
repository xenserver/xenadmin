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

using Moq;
using NUnit.Framework;
using XenAdmin.Controls;
using XenAPI;

namespace XenAdminTests.Controls
{
    public class SrPickerLunPerVDIItemTests : UnitTester_SingleConnectionTestFixture
    {
        #region Helper classes disabling static method calls
        public class SrPickerLunPerVDIItemNoImage : SrPickerLunPerVDIItem
        {
            public SrPickerLunPerVDIItemNoImage(SR sr) : base(sr, null, 0, null) { }
            protected override bool CanBeEnabled { get { return true; } }
            protected override void SetImage() { } //Disable static method call
        }

        public class SrPickerLunPerVDIItemNoImageCanBeEnabled : SrPickerLunPerVDIItem
        {
            public SrPickerLunPerVDIItemNoImageCanBeEnabled(SR sr) : base(sr, null, 0, null) { }
            protected override void SetImage() { } //Disable static method call
        } 
        #endregion

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [Description("Test that the UnsupportedSR call doesn't check the SR is LunPerVDI")]
        public void SrPickerItemUpdateIgnoresUnsupportedSR(bool IsLunPerVdi)
        {
            //As the SrPickerLunPerVDIItem overides the base class call to HBALunePerVDI
            //then expect we don't get any calls to it when CanBeEnabled is also overridden

            //Setup the remainder of the mocks to return true for enabled
            Mock<SR> sr = ObjectManager.NewXenObject<SR>(id);
            sr.Setup(s => s.HBALunPerVDI).Returns(IsLunPerVdi);
            sr.Setup(s => s.ShowInVDISRList(false)).Returns(true); //ShowHiddenVDIs == true

            SrPickerLunPerVDIItem item = new SrPickerLunPerVDIItemNoImage(sr.Object);
            Assert.That(item.Show, Is.True);
            sr.Verify(s=>s.HBALunPerVDI, Times.Never());
        }

        [Test]
        [TestCase(true)]
        [Description("Test that the UnsupportedSR call doesn't check the SR is LunPerVDI, but CanBeEnabled does")]
        public void SrVmPickerItemCanBeEnabledChecksSrType(bool IsLunPerVdi)
        {
            Mock<SR> sr = ObjectManager.NewXenObject<SR>(id);
            sr.Setup(s => s.HBALunPerVDI).Returns(IsLunPerVdi);
            sr.Setup(s => s.ShowInVDISRList(false)).Returns(true); //ShowHiddenVDIs == true
            
            //Below sets CanBeEnabled for the picker items to be true
            sr.Setup(s => s.IsBroken(false)).Returns(false);
            sr.Setup(s => s.CanBeSeenFrom(It.IsAny<Host>())).Returns(true);

            SrPickerLunPerVDIItem item = new SrPickerLunPerVDIItemNoImageCanBeEnabled(sr.Object);
            Assert.That(item.Show, Is.True);

            //Expect only one call to this method as the Unsupported SR call is overridden in SrPickerLunPerVDIItem 
            //This one call coms from SrPickerLunPerVDIItem's CanBeEnabled
            sr.Verify(s => s.HBALunPerVDI, Times.Once());
        }
    }
}
