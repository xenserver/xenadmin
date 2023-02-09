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
 
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using XenAdmin.Alerts;

namespace XenAdminTests.UnitTests.AlertTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class MultipathAlertTest
    {
        [Test]
        public void TestFindHostUuidsTestGivenNull()
        {
            FindHostUuidsTestHelper(null, Enumerable.Empty<string>());
        }

        [Test]
        public void TestFindHostUuidsTestGivenMultipleWithoutDuplicates()
        {
            var given = new List<string>
            {
                "[20181016T09:55:52Z] host=0d5b647c-d913-40e0-a94f-4b9c94dd41eb; host-name=\"xrtuk-12-05\"; pbd=bf08dedc-2564-df8c-3f61-b07654cf824a; scsi_id=3600a098038303973743f486833396d41; current=2; max=4",
                "[20181016T09:55:52Z] host=2e257cd5-b1cb-4f90-908b-e939e8730924; host-name=\"xrtuk-13-06\"; pbd=bf08dedc-2564-df8c-3f61-b07654cf824a; scsi_id=3600a098038303973743f486833396d41; current=2; max=4",
            };
            var expected = new List<string> { "0d5b647c-d913-40e0-a94f-4b9c94dd41eb", "2e257cd5-b1cb-4f90-908b-e939e8730924" };
            FindHostUuidsTestHelper(given, expected);
        }

        [Test]
        public void TestFindHostUuidsTestGivenMultipleWithDuplicates()
        {
            var given = new List<string>
            {
                "[20181016T09:55:52Z] host=2e257cd5-b1cb-4f90-908b-e939e8730924; host-name=\"xrtuk-13-06\"; pbd=bf08dedc-2564-df8c-3f61-b07654cf824a; scsi_id=3600a098038303973743f486833396d41; current=2; max=4",
                "[20181016T09:55:52Z] host=2e257cd5-b1cb-4f90-908b-e939e8730924; host-name=\"xrtuk-13-06\"; root=true; current=2; max=4",
                "[20181016T09:55:52Z] host=0d5b647c-d913-40e0-a94f-4b9c94dd41eb; host-name=\"xrtuk-12-05\"; pbd=bf08dedc-2564-df8c-3f61-b07654cf824a; scsi_id=3600a098038303973743f486833396d41; current=2; max=4"
            };
            var expected = new List<string> { "2e257cd5-b1cb-4f90-908b-e939e8730924", "0d5b647c-d913-40e0-a94f-4b9c94dd41eb" };
            FindHostUuidsTestHelper(given, expected);
        }

        [Test]
        public void TestFindHostUuidsTestGivenUnmatchedLinesShouldNotInterfere()
        {
            var given = new List<string>
            {
                "unmatched",
                "[20181016T09:55:52Z] host=2e257cd5-b1cb-4f90-908b-e939e8730924; host-name=\"xrtuk-13-06\"; pbd=bf08dedc-2564-df8c-3f61-b07654cf824a; scsi_id=3600a098038303973743f486833396d41; current=2; max=4",
                "unmatched",
                "[20181016T09:55:52Z] host=2e257cd5-b1cb-4f90-908b-e939e8730924; host-name=\"xrtuk-13-06\"; root=true; current=2; max=4",
                "unmatched",
                "[20181016T09:55:52Z] host=0d5b647c-d913-40e0-a94f-4b9c94dd41eb; host-name=\"xrtuk-12-05\"; pbd=bf08dedc-2564-df8c-3f61-b07654cf824a; scsi_id=3600a098038303973743f486833396d41; current=2; max=4",
                "unmatched"
            };
            var expected = new List<string> { "2e257cd5-b1cb-4f90-908b-e939e8730924", "0d5b647c-d913-40e0-a94f-4b9c94dd41eb" };
            FindHostUuidsTestHelper(given, expected);
        }

        private static void FindHostUuidsTestHelper(IEnumerable<string> given, IEnumerable<string> expected)
        {
            var actual = MessageAlert.FindHostUuids(given);
            Assert.IsNotNull(actual, "FindHostUuids should not return null.");
            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}
