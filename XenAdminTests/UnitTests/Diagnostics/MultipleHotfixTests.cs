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
using XenAdmin.Diagnostics.Hotfixing;
using XenAPI;
using System.Collections.Generic;

namespace XenAdminTests.UnitTests.Diagnostics
{
    [TestFixture, Category(TestCategories.Unit), Category(TestCategories.SmokeTest)]
    public class MultipleHotfixTests
    {
        private Mock<Hotfix> hotfixA;
        private Mock<Hotfix> hotfixB;
        private Hotfix compositeHotfix;

        private void SetUpCompositeForTwoMocks(bool applyA, bool applyB)
        {
            hotfixA = new Mock<Hotfix>();
            hotfixA.Setup(h => h.ShouldBeAppliedTo(It.IsAny<Host>())).Returns(applyA);
            hotfixA.SetupProperty(p => p.UUID, "aaaa");
            hotfixA.SetupProperty(p => p.Filename, "filenameA");

            hotfixB = new Mock<Hotfix>();
            hotfixB.Setup(h => h.ShouldBeAppliedTo(It.IsAny<Host>())).Returns(applyB);
            hotfixB.SetupProperty(p => p.UUID, "bbbb");
            hotfixB.SetupProperty(p => p.Filename, "filenameB");

            compositeHotfix = new MultipleHotfix()
                                  {
                                      ComponentHotfixes = new List<Hotfix>
                                                              {
                                                                  hotfixA.Object,
                                                                  hotfixB.Object
                                                              }
                                  };
        }

        private void VerifyCompositeForTwoMocksShouldBeApplied()
        {
            hotfixB.Verify(h => h.ShouldBeAppliedTo(It.IsAny<Host>()), Times.AtMostOnce(), "hostfix B should be applied");
            hotfixA.Verify(h => h.ShouldBeAppliedTo(It.IsAny<Host>()), Times.AtMostOnce(), "hostfix A should be applied");
        }

        [Test]
        public void BothCompositeHotfixesNeedToBeApplied()
        {
            SetUpCompositeForTwoMocks(true, true);
            Assert.IsTrue(compositeHotfix.ShouldBeAppliedTo(new Host()));
            VerifyCompositeForTwoMocksShouldBeApplied();

        }

        [Test]
        public void FirstCompositeHotfixesNeedToBeApplied()
        {
            SetUpCompositeForTwoMocks(true, false);
            Assert.IsTrue(compositeHotfix.ShouldBeAppliedTo(new Host()));
            VerifyCompositeForTwoMocksShouldBeApplied();
        }

        [Test]
        public void SecondCompositeHotfixesNeedToBeApplied()
        {
            SetUpCompositeForTwoMocks(false, true);
            Assert.IsTrue(compositeHotfix.ShouldBeAppliedTo(new Host()));
            VerifyCompositeForTwoMocksShouldBeApplied();
        }

        [Test]
        public void NoCompositeHotfixesNeedToBeApplied()
        {
            SetUpCompositeForTwoMocks(false, false);
            Assert.IsFalse(compositeHotfix.ShouldBeAppliedTo(new Host()));
            VerifyCompositeForTwoMocksShouldBeApplied();
        }

        [Test]
        public void NoCompositeHotfixesApply()
        {
            SetUpCompositeForTwoMocks(false, false);
            compositeHotfix.Apply(new Host(), new Session(1, "url", 1));
            VerifyCompositeForTwoMocksShouldBeApplied();
            hotfixA.Verify(h => h.Apply(It.IsAny<Host>(), It.IsAny<Session>()), Times.Never());
            hotfixB.Verify(h => h.Apply(It.IsAny<Host>(), It.IsAny<Session>()), Times.Never());
        }

        [Test]
        public void OneCompositeHotfixesApply()
        {
            SetUpCompositeForTwoMocks(false, true);
            compositeHotfix.Apply(new Host(), new Session(1, "url", 1));
            VerifyCompositeForTwoMocksShouldBeApplied();
            hotfixA.Verify(h => h.Apply(It.IsAny<Host>(), It.IsAny<Session>()), Times.Never());
            hotfixB.Verify(h => h.Apply(It.IsAny<Host>(), It.IsAny<Session>()), Times.Once());
        }

        [Test]
        public void BothCompositeHotfixesApply()
        {
            SetUpCompositeForTwoMocks(true, true);
            compositeHotfix.Apply(new Host(), new Session(1, "url", 1));
            VerifyCompositeForTwoMocksShouldBeApplied();
            hotfixA.Verify(h => h.Apply(It.IsAny<Host>(), It.IsAny<Session>()), Times.Once());
            hotfixB.Verify(h => h.Apply(It.IsAny<Host>(), It.IsAny<Session>()), Times.Once());
        }

        [Test]
        public void ConcatOfUUIDs()
        {
            SetUpCompositeForTwoMocks(true, true);
            Assert.AreEqual("aaaa;bbbb", compositeHotfix.UUID);
        }

        [Test]
        public void ConcatOfFilenames()
        {
            SetUpCompositeForTwoMocks(true, true);
            Assert.AreEqual("filenameA;filenameB", compositeHotfix.Filename);
        }
    }
}