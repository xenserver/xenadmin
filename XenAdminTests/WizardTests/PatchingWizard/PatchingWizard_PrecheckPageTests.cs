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
using XenAdmin;
using XenAdmin.Diagnostics.Checks;
using XenAdmin.Wizards.PatchingWizard;
using XenAPI;

namespace XenAdminTests.WizardTests
{
    class PatchingWizard_PrecheckPageTests : UnitTester_SingleConnectionTestFixture
    {
        private class AccessiblePreCheckPage : PatchingWizard_PrecheckPage
        {
            public IEnumerable<KeyValuePair<string, List<Check>>> GetGenerateChecks(Pool_patch patch)
            {
                return GenerateChecks(patch);
            }
        }

        [Test]
        [TestCase(after_apply_guidance.restartHost, true)]
        [TestCase(after_apply_guidance.restartXAPI, false)]
        [TestCase(after_apply_guidance.restartHVM, false)]
        [TestCase(after_apply_guidance.restartPV, false)]
        [TestCase(after_apply_guidance.unknown, false)]
        public void CheckCanEvacuatePreCheckPresence(after_apply_guidance guidance, bool expected)
        {
            AccessiblePreCheckPage page = new AccessiblePreCheckPage();
            Mock<Pool_patch> patch = ObjectManager.NewXenObject<Pool_patch>(id);
            patch.Setup(p => p.after_apply_guidance).Returns(new List<after_apply_guidance> {guidance});
            IEnumerable<KeyValuePair<string, List<Check>>> generatedChecks = page.GetGenerateChecks(patch.Object);
            Assert.That(generatedChecks.Any(kvp=>kvp.Key == Messages.CHECKING_CANEVACUATE_STATUS), Is.EqualTo(expected));
        }
    }
}
