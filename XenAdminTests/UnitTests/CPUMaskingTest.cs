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
    public class CPUMaskingTest
    {
        const string cpu1 = "040ce33d-bfebfbff-00000001-20100800";
        const string cpu2 = "000ce3bd-bfebfbff-00000001-20100800";
        const string cpu3 = "040ce33d-bfebfbff-00000001-20100801";
        const string mask = "ffffff7f-ffffffff-ffffffff-ffffffff";

        [Test]
        // Can always mask a CPU to itself
        [TestCase("base", cpu1, cpu1, null, ExpectedResult = true)]
        [TestCase("base", cpu1, cpu1, mask, ExpectedResult = true)]
        [TestCase("base", cpu2, cpu2, null, ExpectedResult = true)]
        [TestCase("base", cpu2, cpu2, mask, ExpectedResult = true)]
        [TestCase("base", cpu3, cpu3, null, ExpectedResult = true)]
        [TestCase("base", cpu3, cpu3, mask, ExpectedResult = true)]
        [TestCase("full", cpu1, cpu1, null, ExpectedResult = true)]
        [TestCase("full", cpu1, cpu1, mask, ExpectedResult = true)]
        [TestCase("full", cpu2, cpu2, null, ExpectedResult = true)]
        [TestCase("full", cpu2, cpu2, mask, ExpectedResult = true)]
        [TestCase("full", cpu3, cpu3, null, ExpectedResult = true)]
        [TestCase("full", cpu3, cpu3, mask, ExpectedResult = true)]

        // Masking CPUs to other CPUs: base masking only
        [TestCase("base", cpu1, cpu2, null, ExpectedResult = false)] // cpu1 can only become cpu2 in the presence of the mask
        [TestCase("base", cpu1, cpu2, mask, ExpectedResult = true)]
        [TestCase("base", cpu1, cpu3, null, ExpectedResult = false)] // cpu1 is less than cpu3
        [TestCase("base", cpu1, cpu3, mask, ExpectedResult = false)]
        [TestCase("base", cpu2, cpu1, null, ExpectedResult = false)] // cpu2 is less than cpu1 and cpu3
        [TestCase("base", cpu2, cpu1, mask, ExpectedResult = false)]
        [TestCase("base", cpu2, cpu3, null, ExpectedResult = false)]
        [TestCase("base", cpu2, cpu3, mask, ExpectedResult = false)]
        [TestCase("base", cpu3, cpu1, null, ExpectedResult = false)] // cpu3 is greater than cpu1 and cpu2 in the extended bits, so can't be masked with only base masking
        [TestCase("base", cpu3, cpu1, mask, ExpectedResult = false)]
        [TestCase("base", cpu3, cpu2, null, ExpectedResult = false)]
        [TestCase("base", cpu3, cpu2, mask, ExpectedResult = false)]

        // Masking CPUs to other CPUs: full masking
        [TestCase("full", cpu1, cpu2, null, ExpectedResult = false)] // cpu1 can only become cpu2 in the presence of the mask
        [TestCase("full", cpu1, cpu2, mask, ExpectedResult = true)]
        [TestCase("full", cpu1, cpu3, null, ExpectedResult = false)] // cpu1 is less than cpu3
        [TestCase("full", cpu1, cpu3, mask, ExpectedResult = false)]
        [TestCase("full", cpu2, cpu1, null, ExpectedResult = false)] // cpu2 is less than cpu1 and cpu3
        [TestCase("full", cpu2, cpu1, mask, ExpectedResult = false)]
        [TestCase("full", cpu2, cpu3, null, ExpectedResult = false)]
        [TestCase("full", cpu2, cpu3, mask, ExpectedResult = false)]
        [TestCase("full", cpu3, cpu1, null, ExpectedResult = true)] // cpu3 is greater than cpu1
        [TestCase("full", cpu3, cpu1, mask, ExpectedResult = true)]
        [TestCase("full", cpu3, cpu2, null, ExpectedResult = false)] // cpu3 can only become cpu2 in the presence of the mask
        [TestCase("full", cpu3, cpu2, mask, ExpectedResult = true)]

        // Masking always fails with mask_type "no", even masking a CPU to itself.
        // (In the real program, masking a CPU to itself never gets to the "is it maskable?" code).
        [TestCase("no", cpu1, cpu1, null, ExpectedResult = false)]
        [TestCase("no", cpu1, cpu1, mask, ExpectedResult = false)]
        [TestCase("no", cpu2, cpu2, null, ExpectedResult = false)]
        [TestCase("no", cpu2, cpu2, mask, ExpectedResult = false)]
        [TestCase("no", cpu3, cpu3, null, ExpectedResult = false)]
        [TestCase("no", cpu3, cpu3, mask, ExpectedResult = false)]
        [TestCase("no", cpu1, cpu2, null, ExpectedResult = false)]
        [TestCase("no", cpu1, cpu2, mask, ExpectedResult = false)]
        [TestCase("no", cpu1, cpu3, null, ExpectedResult = false)]
        [TestCase("no", cpu1, cpu3, mask, ExpectedResult = false)]
        [TestCase("no", cpu2, cpu1, null, ExpectedResult = false)]
        [TestCase("no", cpu2, cpu1, mask, ExpectedResult = false)]
        [TestCase("no", cpu2, cpu3, null, ExpectedResult = false)]
        [TestCase("no", cpu2, cpu3, mask, ExpectedResult = false)]
        [TestCase("no", cpu3, cpu1, null, ExpectedResult = false)]
        [TestCase("no", cpu3, cpu1, mask, ExpectedResult = false)]
        [TestCase("no", cpu3, cpu2, null, ExpectedResult = false)]
        [TestCase("no", cpu3, cpu2, mask, ExpectedResult = false)]
        public bool Run(string maskType, string supporterCpu, string coordinatorCpu, string maskBits)
        {
            return PoolJoinRules.MaskableTo(maskType, supporterCpu, coordinatorCpu, maskBits);
        }
    }
}
