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
    public class CPUFeaturesTest
    {
        const string cpu1 = "000ce3bd-bfebfbff-00000001-20100800";
        const string cpu2 = "040ce3bd-bfebfbff-00000001-20100800";
        const string cpu3 = "040ce33d-bfebfbff-00000001-20100801";
        const string cpu4 = "000ce3bd-bfebfbff-00000001";      

        [Test]
        [TestCase(cpu1, cpu1, ExpectedResult = false)]
        [TestCase(cpu2, cpu2, ExpectedResult = false)]
        [TestCase(cpu3, cpu3, ExpectedResult = false)]
        [TestCase(cpu4, cpu4, ExpectedResult = false)]

        // cpu1 has fewer features than cpu2 and cpu3
        [TestCase(cpu1, cpu2, ExpectedResult = true)]
        [TestCase(cpu1, cpu3, ExpectedResult = true)]
        [TestCase(cpu1, cpu4, ExpectedResult = false)]

        // cpu2 has fewer features than cpu3 (and some extra ones)
        [TestCase(cpu2, cpu1, ExpectedResult = false)]
        [TestCase(cpu2, cpu3, ExpectedResult = true)]
        [TestCase(cpu2, cpu4, ExpectedResult = false)]

        // cpu3 is missing one feature (and has some extra ones)
        [TestCase(cpu3, cpu1, ExpectedResult = true)]
        [TestCase(cpu3, cpu2, ExpectedResult = true)]
        [TestCase(cpu3, cpu4, ExpectedResult = true)]

        // cpu4 has fewer features than all because is missing the last set
        [TestCase(cpu4, cpu1, ExpectedResult = true)]
        [TestCase(cpu4, cpu2, ExpectedResult = true)]
        [TestCase(cpu4, cpu3, ExpectedResult = true)]
        public bool Run(string featureSet1, string featureSet2)
        {
            return PoolJoinRules.FewerFeatures(featureSet1, featureSet2);
        }
    }
}