/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAdmin.Core;

namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    class RpmVersionTests
    {
        [Test]
        [TestCase("0.1", "0.2", ExpectedResult = -1)]
        [TestCase("0.1.2", "0.1.3", ExpectedResult = -1)]
        [TestCase("0.1.2", "0.2", ExpectedResult = -1)]
        [TestCase("0.1", "0.1.0", ExpectedResult = 0)]
        [TestCase("0.1", "0.1.0.0.0", ExpectedResult = 0)]
        [TestCase("7.1", "7.1.50", ExpectedResult = -1)]
        [TestCase("7.1.6", "7.1.50", ExpectedResult = -1)]
        [TestCase("22.19.0", "22.19.0-1.xs8", ExpectedResult = -1)]
        [TestCase("22.19.0-1.xs8", "22.19.0-1.xs9", ExpectedResult = 0)]
        [TestCase("22.19.0-1.xs8", "22.19.0-2.xs8", ExpectedResult = -1)]
        [TestCase("22.19.0-1.xs8", "22.19.0-1.2.xs8", ExpectedResult = -1)]
        [TestCase("22.19.0-1.xs8", "22.19.0-1.0.g1234abc.xs8", ExpectedResult = 0)]
        [TestCase("22.19.0-1.0.xs8", "22.19.0-1.0.g1234abc.xs8", ExpectedResult = 0)]
        [TestCase("22.19.0-1.1.g2234abc.xs8", "22.19.0-1.1.g1234abc.xs8", ExpectedResult = 0)]
        [TestCase("1:22.19.0-1.xs8", "22.19.0-1.xs8", ExpectedResult = 1)]
        [TestCase("1:22.19.0-1.xs8", "2:22.19.0-1.xs8", ExpectedResult = -1)]
        [TestCase(null, "22.19.0-1.xs8", ExpectedResult = -1)]
        [TestCase("22.19.0-1.xs8", null, ExpectedResult = 1)]
        public int TestVersionComparer(string version1, string version2)
        {
            return Helpers.ProductVersionCompare(version1, version2);
        }
    }
}
