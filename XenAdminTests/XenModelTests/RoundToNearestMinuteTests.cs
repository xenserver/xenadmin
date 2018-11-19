using System;
using NUnit.Framework;
using XenAdmin;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class RoundToNearestMinuteTests
    {
        private static void Helper(TimeSpan given, TimeSpan expected)
        {
            var actual = Util.RoundToNearestMinute(given);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_round_down()
        {
            Helper(
                new TimeSpan(1, 2, 3, 4, 5),
                new TimeSpan(1, 2, 3, 0, 0));
        }

        [Test]
        public void Test_round_up()
        {
            Helper(
                new TimeSpan(6, 23, 59, 30, 1),
                new TimeSpan(7, 0, 0, 0, 0));
        }
    }
}