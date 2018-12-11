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
using XenAdmin;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class DaysOfWeekWithOffsetTests
    {
        [Test]
        public void Null_today()
        {
            Assert.That(Util.DaysOfWeekOffset(null, 0), Is.Null);
        }

        [Test]
        public void Null_tomorrow()
        {
            Assert.That(Util.DaysOfWeekOffset(null, 1), Is.Null);
        }

        [Test]
        public void Empty_list_today()
        {
            Assert.That(Util.DaysOfWeekOffset(new List<DayOfWeek>(), 0), Is.Empty);
        }

        [Test]
        public void Empty_list_tommorrow()
        {
            Assert.That(Util.DaysOfWeekOffset(new List<DayOfWeek>(), 1), Is.Empty);
        }

        [Test]
        public void Assorted_days_today()
        {
            var given = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Sunday };
            var expected = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Sunday };
            Assert.That(Util.DaysOfWeekOffset(given, 0), Is.EquivalentTo(expected));
        }

        [Test]
        public void Assorted_days_tommorrow()
        {
            var given = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Sunday };
            var expected = new List<DayOfWeek> { DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Monday };
            Assert.That(Util.DaysOfWeekOffset(given, 1), Is.EquivalentTo(expected));
        }

        [Test]
        public void Assorted_days_yesterday()
        {
            var given = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Sunday };
            var expected = new List<DayOfWeek> { DayOfWeek.Sunday, DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday };
            Assert.That(Util.DaysOfWeekOffset(given, -1), Is.EquivalentTo(expected));
        }
    }
}