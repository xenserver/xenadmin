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
using XenAdmin;
using NUnit.Framework;

namespace XenAdminTests.StringExtensionsTests
{
    public class EllipsiseStringTests : UnitTester_TestFixture
    {
        [Test]
        public void TestEllipsisingToShortenAString()
        {
            const string testString = "this is a very long string, well long-ish, actually it's of moderate length really";
            Assert.That(testString.Ellipsise(5), Is.EqualTo("th..."));
            Assert.That(testString.Ellipsise(18), Is.EqualTo("this is a very ..."));
            Assert.That(testString.Ellipsise(3), Is.EqualTo("..."));
            
        }

        [Test]
        public void TestEllipsisingAShortEnoughString([Values(8, 9, 28, 10000)] int size)
        {
            const string testString = "short";
            Assert.That(testString.Ellipsise(size), Is.EqualTo(testString));
        }

        [Test]
        public void TestEllipsisingANullString()
        {
            const string testString = null;
            Assert.That(testString.Ellipsise(10), Is.EqualTo(""));
        }

        [Test]
        public void TestEllipsisingAnEmptyString()
        {
            Assert.That("".Ellipsise(10), Is.EqualTo(""));
        }

        [Test]
        public void TestAppendingEllipsisingWithMaxLengthLessThanEllipse([Values(1,2)] int size)
        {
            Assert.That("short".Ellipsise(size), Is.EqualTo("."));
        }

        [Test]
        [TestCase("junk", "junk...")]
        [TestCase("", "...")]
        [TestCase(null, "...")]
        [TestCase("junk...", "junk......")]
        [TestCase("junk.", "junk....")]
        public void AdditionOfEllipsis(string testString, string expectedResult)
        {
            Assert.That(testString.AddEllipsis(), Is.EqualTo(expectedResult));
        }

    }

}
