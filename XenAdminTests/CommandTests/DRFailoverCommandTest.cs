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

using System.Linq;
using XenAdmin.Commands;
using NUnit.Framework;

namespace XenAdminTests.CommandTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class DRFailoverCommandTestBoston : MainWindowLauncher_TestFixture
    {
        public DRFailoverCommandTestBoston()
            : base(false, CommandTestsDatabase.Boston)
        {
        }

        [Test]
        public void Run()
        {
            DRFailoverCommandTest tester = new DRFailoverCommandTest();
            tester.Test();
        }
    }

    public class DRFailoverCommandTest : CommandTest
    {
        internal override Command CreateCommand()
        {
            return new DRFailoverCommand();
        }

        public void Test()
        {
            if (RunTest().Any())
            {
                HandleModelessDialog(@"Disaster Recovery - Failover to pool 'New Pool'",
                                   Command.Execute,
                                   (DRFailoverWizardWrapper w) =>
                                   {
                                       Assert.IsTrue(w.FailoverRadioButton.Checked, "Failover radio button pre-checked");
                                       w.CancelButton.PerformClick();
                                   });
            }
            else
            {
                Assert.Fail("DR Failover Command was not tested as the desired selection was not found");
            }
        }
    }
}
