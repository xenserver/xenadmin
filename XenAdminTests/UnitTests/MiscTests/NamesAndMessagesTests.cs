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

using NUnit.Framework;
using XenAdmin;
using XenAdminTests.UnitTests.UnitTestHelper;
using XenAPI;

namespace XenAdminTests.UnitTests.MiscTests
{
    [TestFixture, Category(TestCategories.Unit)]
    internal class NamesAndMessagesTests
    {
        [Test]
        public void FriendlyNamesStaticMethodsReturnStrings()
        {
            IUnitTestVerifier validator = new VerifyStaticMethodReturn(typeof(FriendlyNames)); 
            validator.Verify(typeof(string));
        }

        [Test]
        public void MessagesStaticMethodsReturnStrings()
        {
            IUnitTestVerifier validator = new VerifyStaticMethodReturn(typeof(Messages));
            validator.Verify(typeof(string));
        }

        [Test]
        public void InvisibleMessagesStaticMethodsReturnStrings()
        {
            IUnitTestVerifier validator = new VerifyStaticMethodReturn(typeof(InvisibleMessages));
            validator.Verify(typeof(string));
        }

        [Test]
        public void XapiFriendlyNameStaticMethodsReturnStrings()
        {
            IUnitTestVerifier validator = new VerifyStaticMethodReturn(typeof(FriendlyErrorNames));
            validator.Verify(typeof(string));
        }
    }
}