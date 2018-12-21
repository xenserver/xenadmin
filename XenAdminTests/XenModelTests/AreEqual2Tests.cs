﻿using System.Collections.Generic;
using NUnit.Framework;
using XenAPI;

namespace XenAdminTests.XenModelTests
{
    [TestFixture, Category(TestCategories.Unit)]
    public class AreEqual2Tests
    {
        [Test]
        public void A_null_is_considered_equal_to_itself()
        {
            List<string> given = null;
            Assert.IsTrue(Helper.AreEqual2(given, given));
        }

        [Test]
        public void Separate_nulls_are_considered_equal_to_each_other()
        {
            Assert.IsTrue(Helper.AreEqual2((List<string>)null, (List<string>)null));
        }

        [Test]
        public void A_list_is_considered_equal_to_itself()
        {
            var given = new List<string> {
                "test",
                "other"
            };
            Assert.IsTrue(Helper.AreEqual2(given, given));
        }

        [Test]
        public void Different_lists_are_not_considered_equal_to_each_other()
        {
            Assert.IsFalse(
                Helper.AreEqual2(
                    new List<string> {
                        "test",
                        "other"
                    },
                    new List<string> {
                        "something",
                        "else"
                    }));
        }

        [Test]
        public void Separate_though_equal_lists_are_considered_equal_to_each_other()
        {
            Assert.IsTrue(
                Helper.AreEqual2(
                    new List<string> {
                        "test",
                        "other"
                    },
                    new List<string> {
                        "test",
                        "other"
                    }));
        }

        [Test]
        public void A_dictionary_is_considered_equal_to_itself()
        {
            var given = new Dictionary<string, string> {
                { "test", "a"},
                { "other", "b"}
            };
            Assert.IsTrue(Helper.AreEqual2(given, given));
        }

        [Test]
        public void Different_dictionaries_are_not_considered_equal_to_each_other()
        {
            Assert.IsFalse(
                Helper.AreEqual2(
                    new Dictionary<string, string> {
                        { "test", "a"},
                        { "other", "b"}
                    },
                    new Dictionary<string, string> {
                        { "something", "c" },
                        { "else", "d" }
                    }));
        }

        [Test]
        public void Separate_though_equal_dictionaries_are_considered_equal_to_each_other()
        {
            Assert.IsTrue(
                Helper.AreEqual2(
                    new Dictionary<string, string> {
                        { "test", "a"},
                        { "other", "b"}
                    },
                    new Dictionary<string, string> {
                        { "test", "a" },
                        { "other", "b" }
                    }));
        }
    }
}
