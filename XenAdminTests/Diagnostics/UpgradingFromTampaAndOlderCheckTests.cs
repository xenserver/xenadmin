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
using XenAdmin.Diagnostics.Checks;
using XenAdminTests.UnitTests.UnitTestHelper.MockObjectBuilders;
using XenAPI;

namespace XenAdminTests.Diagnostics
{
    public class UpgradingFromTampaAndOlderCheckTests : UnitTester_SingleConnectionTestFixture
    {
        #region Test Cases and Helpers
        public class TestCase
        {
            public ObjectBuilderType HostType { get; set; }
            public bool ExpectProblem { get; set; }
            public bool InGrace { get; set; }
            public bool IsFree { get; set; }
        }

        private IEnumerable<TestCase> TestCases
        {
            get
            {
                yield return new TestCase
                {
                    HostType = ObjectBuilderType.ClearwaterHost,
                    ExpectProblem = false,
                    InGrace = false,
                    IsFree = false,
                };
                yield return new TestCase
                {
                    HostType = ObjectBuilderType.ClearwaterHost,
                    ExpectProblem = false,
                    InGrace = true,
                    IsFree = true,
                };
                yield return new TestCase
                {
                    HostType = ObjectBuilderType.ClearwaterHost,
                    ExpectProblem = false,
                    InGrace = true,
                    IsFree = false,
                };
                yield return new TestCase
                {
                    HostType = ObjectBuilderType.ClearwaterHost,
                    ExpectProblem = false,
                    InGrace = false,
                    IsFree = true,
                };
                yield return new TestCase
                {
                    HostType = ObjectBuilderType.TampaHost,
                    ExpectProblem = true,
                    InGrace = true,
                    IsFree = true,
                };
                yield return new TestCase
                {
                    HostType = ObjectBuilderType.TampaHost,
                    ExpectProblem = false,
                    InGrace = false,
                    IsFree = true,
                };
                yield return new TestCase
                {
                    HostType = ObjectBuilderType.TampaHost,
                    ExpectProblem = true,
                    InGrace = false,
                    IsFree = false,
                };
                yield return new TestCase
                {
                    HostType = ObjectBuilderType.TampaHost,
                    ExpectProblem = true,
                    InGrace = true,
                    IsFree = false,
                };


            }
        }
        
        #endregion

        [Test, TestCaseSource("TestCases")]
        public void TestLicenseStatusForProblem(TestCase tc)
        {
            Mock<Host> host = ObjectFactory.BuiltObject<Host>(tc.HostType, id);
            host.Setup(h => h.IsFreeLicense()).Returns(tc.IsFree);
            host.Setup(h => h.InGrace).Returns(tc.InGrace);
            UpgradingFromTampaAndOlderCheck check = new UpgradingFromTampaAndOlderCheck(host.Object);
            Assert.AreEqual(tc.ExpectProblem, check.RunAllChecks().Count != 0, "Problem found");
        }
    }
}
