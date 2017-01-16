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
using NUnit.Framework;
using XenAdmin.Actions;

namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class HealthCheckAnalysisProgressTest
    {
        private Dictionary<string, double> expectedResults = new Dictionary<string, double>()
                                         { 
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : 0 }", 0 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : 10 }", 10 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : 100 }", 100 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : 100.0 }", 100 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : 0.01 }", 0.01 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : 99.99 }", 99.99 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : 10,  \"131-435\" : 12 }", 10 },
                                            { "{ \"30253b07-138a-1b17-80a1\" : 12, \"30253b07-138a-1b17-80a1-117317ded3ca\" : 10 }", 10 },
                                            { "{ 30253b07-138a-1b17-80a1-117317ded3ca : 10 }", 10 },
                                            { "{ 30253b07-138a-1b17-80a1-117317ded3ca : 10, 2345: 25 }", 10 },
                                            { "{ 2345: 25, 30253b07-138a-1b17-80a1-117317ded3ca : 10 }", 10 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : not a number }", -1 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : \"not a number\" }", -1 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : \"10\" }", 10 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : \"-10\" }", -1},
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : -0.56 }", -1 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : -10 }", -1 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : -100 }", -1 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : 123 }", -1 },
                                            { "{ \"30253b07-138a-1b17-80a1-117317ded3ca\" : 123.89 }", -1 },
                                            { string.Empty, -1 },
                                            { " ", -1 },
                                            { "{ }", -1 }
                                         };

        [Test]
        public void Run()
        {
            foreach (var expectedResult in expectedResults)
            {
                Assert.AreEqual(expectedResult.Value, GetHealthCheckAnalysisResultAction.ParseAnalysisProgress(expectedResult.Key, "30253b07-138a-1b17-80a1-117317ded3ca"));
            }
        }
    }
}
