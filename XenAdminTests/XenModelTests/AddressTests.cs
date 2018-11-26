using System.Collections.Generic;
using NUnit.Framework;
using XenAPI;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]

    public class AddressTests
    {
        private readonly Dictionary<string, string> _simpleNetworks = new Dictionary<string, string>
        {
            {"0/ip", "10.71.57.53"},
            {"0/ipv4/0", "10.71.57.53"},
            {"0/ipv6/0", "fe80:0000:0000:0000:1c1a:c2ff:fe2e:c823"}
        };

        [Test]
        public void TestSimple()
        {
            var expected = new List<string>
            {
                "10.71.57.53",
                "fe80:0000:0000:0000:1c1a:c2ff:fe2e:c823"
            };
            var actual = Address.FindIpAddresses(_simpleNetworks, "0");
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void TestUnusedDevice()
        {
            var actual = Address.FindIpAddresses(_simpleNetworks, "1");
            Assert.That(actual, Is.Empty);
        }
    }
}
