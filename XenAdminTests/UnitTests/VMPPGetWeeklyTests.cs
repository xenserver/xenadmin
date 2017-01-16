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
using XenAPI;
using NUnit.Framework;

namespace XenAdminTests.UnitTests
{

    class VMPPGetWeeklyTests
    {

        [Test]
        public void Test1()
        {
            //Test when the day this week already past
            var time = VMPP.GetWeeklyDate(new DateTime(2010, 10, 20, 10, 59, 0), 10, 15, new List<DayOfWeek> { DayOfWeek.Tuesday });
            Assert.AreEqual(new DateTime(2010, 10, 26, 10, 15, 0), time);
        }

        [Test]
        public void Test2()
        {
            //Test when the day is today but the time alreday past
            var time = VMPP.GetWeeklyDate(new DateTime(2010, 10, 20, 10, 59, 0), 10, 15, new List<DayOfWeek> { DayOfWeek.Wednesday });
            Assert.AreEqual(new DateTime(2010, 10, 27, 10, 15, 0), time);
        }

        [Test]
        public void Test3()
        {
            //Test when the day is today but the time did not pass
            var time = VMPP.GetWeeklyDate(new DateTime(2010, 10, 20, 10, 00, 0), 10, 15, new List<DayOfWeek> { DayOfWeek.Wednesday });
            Assert.AreEqual(new DateTime(2010, 10, 20, 10, 15, 0), time);
        }

        [Test]
        public void Test4()
        {
            //Test when the day is today but the time did not pass with more than one day
            var time = VMPP.GetWeeklyDate(new DateTime(2010, 10, 20, 10, 00, 0), 10, 15, new List<DayOfWeek> { DayOfWeek.Wednesday,DayOfWeek.Monday });
            Assert.AreEqual(new DateTime(2010, 10, 20, 10, 15, 0), time);
        }

        [Test]
        public void Test5()
        {
            //Today already past take next day in same week
            var time = VMPP.GetWeeklyDate(new DateTime(2010, 10, 20, 10, 30, 0), 10, 15, new List<DayOfWeek> { DayOfWeek.Wednesday, DayOfWeek.Thursday });
            Assert.AreEqual(new DateTime(2010, 10, 21, 10, 15, 0), time);
        }

        [Test]
        public void Test6()
        {
            //Today already past take next day in next week
            var time = VMPP.GetWeeklyDate(new DateTime(2010, 10, 20, 10, 30, 0), 10, 15, new List<DayOfWeek> { DayOfWeek.Wednesday, DayOfWeek.Monday });
            Assert.AreEqual(new DateTime(2010, 10, 25, 10, 15, 0), time);
        }

        [Test]
        public void Test7()
        {
            //Today already past take next day in next week
            var time = VMPP.GetWeeklyDate(new DateTime(2010, 10, 20, 10, 15, 0), 10, 15, new List<DayOfWeek> { DayOfWeek.Wednesday, DayOfWeek.Monday });
            Assert.AreEqual(new DateTime(2010, 10, 20, 10, 15, 0), time);
        }

        [Test]
        public void Test8()
        {
            //Check Sunday and unsorted list
            var time = VMPP.GetWeeklyDate(new DateTime(2010, 10, 24, 10, 16, 0), 10, 15, new List<DayOfWeek> { DayOfWeek.Sunday, DayOfWeek.Monday });
            Assert.AreEqual(new DateTime(2010, 10, 25, 10, 15, 0), time);
        }

    }
}
