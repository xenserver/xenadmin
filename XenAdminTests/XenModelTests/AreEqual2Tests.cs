using System.Collections.Generic;
using NUnit.Framework;
using XenAPI;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class AreEqual2Tests
    {
        [Test]
        public void Separate_null()
        {
            Assert.That(Helper.AreEqual2((List<string>)null, (List<string>)null), Is.True);
        }

        [Test]
        public void Same_null()
        {
            List<string> given = null;
            Assert.That(Helper.AreEqual2(given, given), Is.True);
        }

        [Test]
        public void Separate_same_dictionary()
        {
            Assert.That(
                Helper.AreEqual2(
                    new Dictionary<string, string> {
                        { "test", "a"},
                        { "other", "b"}
                    },
                    new Dictionary<string, string> {
                        { "test", "a" },
                        { "other", "b" }
                    }),
                Is.True);
        }

        [Test]
        public void Separate_not_same_list()
        {
            Assert.That(
                Helper.AreEqual2(
                    new List<string> {
                        "test",
                        "other"
                    },
                    new List<string> {
                        "something",
                        "else"
                    }),
                Is.False);
        }

        [Test]
        public void Same_list()
        {
            var given = new List<string> {
                "test",
                "other"
            };
            Assert.That(Helper.AreEqual2(given, given), Is.True);
        }

        [Test]
        public void Separate_same_list()
        {
            Assert.That(
                Helper.AreEqual2(
                    new List<string> {
                        "test",
                        "other"
                    },
                    new List<string> {
                        "test",
                        "other"
                    }),
            Is.True);
        }

        [Test]
        public void Separate_not_same_dictionary()
        {
            Assert.That(
                Helper.AreEqual2(
                    new Dictionary<string, string> {
                        { "test", "a"},
                        { "other", "b"}
                    },
                    new Dictionary<string, string> {
                        { "something", "c" },
                        { "else", "d" }
                    }),
                Is.False);
        }

        [Test]
        public void Same_dictionary()
        {
            var given = new Dictionary<string, string> {
                { "test", "a"},
                { "other", "b"}
            };
            Assert.That(Helper.AreEqual2(given, given), Is.True);
        }
    }
}