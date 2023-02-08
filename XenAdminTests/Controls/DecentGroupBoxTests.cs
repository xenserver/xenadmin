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


using NUnit.Framework;
using XenAdmin.Controls;

namespace XenAdminTests.Controls
{
    [TestFixture, Category(TestCategories.Unit)]
    public class DecentGroupBoxTests
    {
        private DecentGroupBox gb;

        [SetUp]
        public void TestSetUp()
        {
            gb = null;
        }

        [TearDown]
        public void TestTearDown()
        {
            gb?.Dispose();
        }


        [Test, Description("Default case")]
        [TestCase("junk", 100, ExpectedResult = "junk")]
        [TestCase("junk&", 50, ExpectedResult = "junk&&")]
        [TestCase("junk&", 100, ExpectedResult = "junk&&")]
        [TestCase("junk&&", 100, ExpectedResult = "junk&&&&")]
        [TestCase("junk&&junk", 100, ExpectedResult = "junk&&&&junk")]
        [TestCase("junk&junk", 100, ExpectedResult = "junk&&junk")]
        [TestCase("junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", 100, ExpectedResult = "junk&&&&&&&&&&&&&&&&...")]
        [TestCase("jnk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", 100, ExpectedResult = "jnk&&&&&&&&&&&&&&&&&&...")]
        [TestCase("AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
            50, ExpectedResult = "All...")]
        [TestCase("AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
            100, ExpectedResult = "AllWorkAndN...")]
        public string TestTextManipulationNoMnemonic(string text, int width)
        {
            gb = new DecentGroupBox {Width = width, Text = text};
            Assert.IsTrue(gb.AutoEllipsis, "auto ellipse setting");
            Assert.IsFalse(gb.UseMnemonic, "use mnemonic setting");
            Assert.AreEqual(gb.Text, text, $"Raw text TestCase: {text}");
            return gb.EscapedText;
        }

        [Test]
        [TestCase("junk", 100, ExpectedResult = "junk")]
        [TestCase("junk&", 50, ExpectedResult = "junk&")]
        [TestCase("junk&", 100, ExpectedResult = "junk&")]
        [TestCase("junk&&", 100, ExpectedResult = "junk&&")]
        [TestCase("junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", 100, ExpectedResult = "junk&&&&&&&&&&&&&&&&&...")]
        [TestCase("jnk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", 100, ExpectedResult = "jnk&&&&&&&&&&&&&&&&&&&...")]
        [TestCase("junk&&junk", 100, ExpectedResult = "junk&&junk")]
        [TestCase("junk&junk", 100, ExpectedResult = "junk&junk")]
        [TestCase("AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
            50, ExpectedResult = "All...")]
        [TestCase("AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy",
            100, ExpectedResult = "AllWorkAndN...")]
        public string TestTextManipulationWithMnemonic(string text, int width)
        {
            gb = new DecentGroupBox {Width = width, UseMnemonic = true, Text = text};
            Assert.IsTrue(gb.AutoEllipsis, "auto ellipse setting");
            Assert.IsTrue(gb.UseMnemonic, "use mnemonic setting");
            Assert.AreEqual(gb.Text, text, $"Raw text TestCase: {text}");
            return gb.EscapedText;
        }

        [Test]
        [TestCase("junk", 100, ExpectedResult = "junk")]
        [TestCase("junk&", 50, ExpectedResult = "junk&")]
        [TestCase("junk&", 100, ExpectedResult = "junk&")]
        [TestCase("junk&&", 100, ExpectedResult = "junk&&")]
        [TestCase("junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", 100, ExpectedResult = "junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&")]
        [TestCase("junk&&junk", 100, ExpectedResult = "junk&&junk")]
        [TestCase("junk&junk", 100, ExpectedResult = "junk&junk")]
        [TestCase("AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy", 50,
            ExpectedResult = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy")]
        [TestCase("AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy", 100,
            ExpectedResult = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy")]
        public string TestTextManipulationWithMnemonicNoEllipsing(string text, int width)
        {
            gb = new DecentGroupBox {Width = width, UseMnemonic = true, AutoEllipsis = false, Text = text};
            Assert.IsFalse(gb.AutoEllipsis, "auto ellipse setting");
            Assert.IsTrue(gb.UseMnemonic, "use mnemonic setting");
            Assert.AreEqual(gb.Text, text, $"Raw text TestCase: {text}");
            return gb.EscapedText;
        }

        [Test]
        [TestCase("junk", 100, ExpectedResult = "junk")]
        [TestCase("junk&", 50, ExpectedResult = "junk&&")]
        [TestCase("junk&", 100, ExpectedResult = "junk&&")]
        [TestCase("junk&&", 100, ExpectedResult = "junk&&&&")]
        [TestCase("junk&&junk", 100, ExpectedResult = "junk&&&&junk")]
        [TestCase("junk&junk", 100, ExpectedResult = "junk&&junk")]
        [TestCase("junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&", 100,
            ExpectedResult = "junk&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&")]
        [TestCase("AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy", 50,
            ExpectedResult = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy")]
        [TestCase("AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy", 100,
            ExpectedResult = "AllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoyAllWorkAndNoPlayMakesJackADullBoy")]
        public string TestTextManipulationNoMnemonicNoAutoEllipse(string text, int width)
        {
            gb = new DecentGroupBox {Width = width, UseMnemonic = false, AutoEllipsis = false, Text = text};
            Assert.IsFalse(gb.AutoEllipsis, "auto ellipse setting");
            Assert.IsFalse(gb.UseMnemonic, "use mnemonic setting");
            Assert.AreEqual(gb.Text, text, $"Raw text TestCase: {text}");
            return gb.EscapedText;
        }

        [Test]
        public void TextBoxWidthNearFudgeFactor([Values(10, 14, 15, 16, 18)] int width)
        {
            const string text = "SomeTextOrOther";
            gb = new DecentGroupBox {Width = width, Text = text};
            Assert.IsTrue(gb.AutoEllipsis, "auto ellipse setting");
            Assert.IsFalse(gb.UseMnemonic, "use mnemonic setting");
            Assert.AreEqual(gb.Text, text, $"Raw text TestCase width: {width}");
            Assert.AreEqual(gb.EscapedText, string.Empty, $"Escaped text TestCase width: {width}");
        }
    }
}
