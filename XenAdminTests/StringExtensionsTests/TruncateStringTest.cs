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

namespace XenAdminTests.StringExtensionsTests
{
    class TruncateStringTest : UnitTester_TestFixture
    {
        [Test]
        public void RunTest()
        {
            string s1 = "abcde";
            string s2 = "短𠀁𪛕";  // The first character is 2 bytes, but the other two are 4-byte characters (UTF-16 surrogate pairs).

            Assert.AreEqual("", s1.Truncate(0));
            Assert.AreEqual("a", s1.Truncate(1));
            Assert.AreEqual("ab", s1.Truncate(2));
            Assert.AreEqual("abc", s1.Truncate(3));
            Assert.AreEqual("abcd", s1.Truncate(4));
            Assert.AreEqual("abcde", s1.Truncate(5));

            Assert.AreEqual("", s2.Truncate(0));
            Assert.AreEqual("短", s2.Truncate(1));
            Assert.AreEqual("短", s2.Truncate(2));
            Assert.AreEqual("短𠀁", s2.Truncate(3));
            Assert.AreEqual("短𠀁", s2.Truncate(4));
            Assert.AreEqual("短𠀁𪛕", s2.Truncate(5));
        }
    }
}
