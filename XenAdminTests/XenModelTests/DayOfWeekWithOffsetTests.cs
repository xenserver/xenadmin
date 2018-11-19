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
using NUnit.Framework;
using XenAdmin;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class DayOfWeekWithOffsetTests
    {
        [TestCase(DayOfWeek.Monday, 0, Result = DayOfWeek.Monday, Description = "Today")]
        [TestCase(DayOfWeek.Monday, 1, Result = DayOfWeek.Tuesday, Description = "Tomorrow")]
        [TestCase(DayOfWeek.Monday, -1, Result = DayOfWeek.Sunday, Description = "Yesterday")]
        [TestCase(DayOfWeek.Monday, 6, Result = DayOfWeek.Sunday, Description = "One week from now less a day")]
        [TestCase(DayOfWeek.Monday, 7, Result = DayOfWeek.Monday, Description = "One week from now")]
        [TestCase(DayOfWeek.Monday, -6, Result = DayOfWeek.Tuesday, Description = "One week ago less a day")]
        [TestCase(DayOfWeek.Monday, -7, Result = DayOfWeek.Monday, Description = "One week ago")]
        [TestCase((DayOfWeek)0, int.MinValue, Result = DayOfWeek.Friday, Description = "Lower range check")]
        [TestCase((DayOfWeek)(7 - 1), int.MaxValue, Result = DayOfWeek.Sunday, Description = "Upper range check")]

        public DayOfWeek DayOfWeekOffsetTest(DayOfWeek dayOfWeek, int daysDifference)
        {
            return Util.DayOfWeekWithOffset(dayOfWeek, daysDifference);
        }
    }
}
