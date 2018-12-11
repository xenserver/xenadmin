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
using XenAPI;

namespace XenAdminTests.UnitTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class VMSSClientTimeTests
    {
        private readonly Dictionary<string, string> _givenSettings = new Dictionary<string, string>
        {
            { "hour", "0" },
            { "min", "15" },
            { "days", "Monday" }
        };

        [Test]
        public void Hourly_with_zero_diff()
        {
            var actual = VMSSClient.FindScheduleWithGivenTimeOffset(
                TimeSpan.Zero,
                vmss_frequency.hourly,
                _givenSettings);
            var expected = _givenSettings;
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void Hourly_with_quarter_of_an_hour_diff()
        {
            var actual = VMSSClient.FindScheduleWithGivenTimeOffset(
                TimeSpan.FromMinutes(15),
                vmss_frequency.hourly,
                _givenSettings);
            var expected = new Dictionary<string, string>
            {
                { "hour", "0" },
                { "min", "30" },
                { "days", "Monday" }
            };
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void Hourly_with_minus_half_an_hour_diff()
        {
            var actual = VMSSClient.FindScheduleWithGivenTimeOffset(
                TimeSpan.FromMinutes(-30),
                vmss_frequency.hourly,
                _givenSettings);
            var expected = new Dictionary<string, string>
            {
                { "hour", "0" },
                { "min", "45" },
                { "days", "Monday" }
            };
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void Hourly_with_three_quarters_of_an_hour_diff()
        {
            var actual = VMSSClient.FindScheduleWithGivenTimeOffset(
                TimeSpan.FromMinutes(45),
                vmss_frequency.hourly,
                _givenSettings);
            var expected = new Dictionary<string, string>
            {
                { "hour", "0" },
                { "min", "0" },
                { "days", "Monday" }
            };
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void Hourly_with_an_hour_diff()
        {
            var actual = VMSSClient.FindScheduleWithGivenTimeOffset(
                TimeSpan.FromMinutes(60),
                vmss_frequency.hourly,
                _givenSettings);
            var expected = _givenSettings;
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void Daily_with_zero_diff()
        {
            var actual = VMSSClient.FindScheduleWithGivenTimeOffset(
                TimeSpan.Zero,
                vmss_frequency.daily,
                _givenSettings);
            var expected = _givenSettings;
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void Daily_with_minus_half_an_hour_diff()
        {
            var actual = VMSSClient.FindScheduleWithGivenTimeOffset(
                TimeSpan.FromMinutes(-30),
                vmss_frequency.daily,
                _givenSettings);
            var expected = new Dictionary<string, string>
            {
                { "hour", "23" },
                { "min", "45" },
                { "days", "Monday" }
            };
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void Daily_with_three_quarters_of_an_hour_diff()
        {
            var actual = VMSSClient.FindScheduleWithGivenTimeOffset(
                TimeSpan.FromMinutes(45),
                vmss_frequency.daily,
                _givenSettings);
            var expected = new Dictionary<string, string>
            {
                { "hour", "1" },
                { "min", "0" },
                { "days", "Monday" }
            };
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void Weekly_with_zero_diff()
        {
            var actual = VMSSClient.FindScheduleWithGivenTimeOffset(
                TimeSpan.Zero,
                vmss_frequency.weekly,
                _givenSettings);
            var expected = _givenSettings;
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void Weekly_with_minus_half_an_hour_diff()
        {
            var actual = VMSSClient.FindScheduleWithGivenTimeOffset(
                TimeSpan.FromMinutes(-30),
                vmss_frequency.weekly,
                _givenSettings);
            var expected = new Dictionary<string, string>
            {
                { "hour", "23" },
                { "min", "45" },
                { "days", "Sunday" }
            };
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void Weekly_with_three_quarters_of_an_hour_diff()
        {
            var actual = VMSSClient.FindScheduleWithGivenTimeOffset(
                TimeSpan.FromMinutes(45),
                vmss_frequency.weekly,
                _givenSettings);
            var expected = new Dictionary<string, string>
            {
                { "hour", "1" },
                { "min", "0" },
                { "days", "Monday" }
            };
            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}
