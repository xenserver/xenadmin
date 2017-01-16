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

using XenAdmin.Actions;
using NUnit.Framework;
using System.Threading;
using XenAdmin.Network;
using XenAPI;

namespace XenAdminTests.XenModelTests.ActionTests
{
    public abstract class ActionTest<T> : DatabaseTester_TestFixture where T : AsyncAction
    {
        protected const string dbName = "state1.xml";
        protected ActionTest() : base(dbName) { }

        [Test]
        public void RunActionTests()
        {
            T action = NewAction();
            action.RunAsync();

            while (!action.IsCompleted)
                Thread.Sleep(1000);

            
            Assert.True(action.Succeeded, "Action succeeded");

            Assert.True(VerifyResult(action), "Result verified");
        }

        protected abstract T NewAction();
        protected abstract bool VerifyResult(T action);
    }

    public abstract class ActionUnitTest<T> : UnitTester_SingleConnectionTestFixture where T : AsyncAction
    {
        [SetUp]
        public void SetupUnitTest()
        {
            Session s = ObjectManager.MockSessionFor(id).Object;
            ObjectManager.MockConfigProvider.SetupGet(c => c.SudoDialogDelegate).Returns(() => delegate{return new AsyncAction.SudoElevationResult(true, string.Empty, string.Empty, s);});
            ObjectManager.MockConfigProvider.SetupGet(c => c.DontSudo).Returns(true);
        }

        protected Response<string> ValidResponse { get { return new Response<string>("ok"); } }

        [Test]
        public void RunActionTests()
        {
            T action = NewAction();
            action.RunAsync();

            while (!action.IsCompleted)
                Thread.Sleep(1000);


            Assert.True(action.Succeeded, "Action succeeded");

            Assert.True(VerifyResult(action), "Result verified");
        }

        protected abstract T NewAction();
        protected abstract bool VerifyResult(T action);
    }
}
