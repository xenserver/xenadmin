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
using System.Text;
using NUnit.Framework;
using XenAdmin.Plugins;
using XenAPI;
using XenAdmin.Network;
using XenAdmin;

namespace XenAdminTests.PluginTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class PlaceholdersTests : MainWindowLauncher_TestFixture
    {
        public PlaceholdersTests()
            : base("state4.xml")
        { }

        [Test]
        public void TestSessionIdWithNullConnection()
        {
            string text = @"hello {$session_id} there";
            string output = Placeholders.Substitute(text, new Host());
            Assert.AreEqual("hello null there", output, "null connection session_id didn't work");
        }
        
        [Test]
        public void TestSessionIdWithNonNullConnection()
        {
            IXenObject host = GetAnyHost();
            string output = Placeholders.Substitute(@"hello {$session_id} there", host);
            Assert.AreEqual("hello dummy there", output, "null connection session_id didn't work");
        }

        [Test]
        public void TestInvalidSessionIdPlaceholder()
        {
            IXenObject host = GetAnyHost();
            string output = Placeholders.Substitute(@"hello {$sesion_id} there", host);
            Assert.AreEqual("hello {$sesion_id} there", output, "null connection session_id didn't work");
        }

        [Test]
        public void TestIPAddress()
        {
            IXenObject vmWith2Networks = GetAnyVM(v => v.name_label == "Iscsi Box");

            List<Uri> urls = Placeholders.SubstituteUri(@"http://{$ip_address}/hello_there", vmWith2Networks);

            Assert.AreEqual(2, urls.Count, "Wrong number of urls returned");
            Assert.IsTrue(urls.Contains(new Uri(@"http://10.80.237.36/hello_there")), "Wrong url returned.");
            Assert.IsTrue(urls.Contains(new Uri(@"http://10.80.238.198/hello_there")), "Wrong url returned.");
        }

        [Test]
        public void TestInvalidIPAddressPlaceholder()
        {
            IXenObject vmWith2Networks = GetAnyVM(v => v.name_label == "Iscsi Box");

            List<Uri> urls = Placeholders.SubstituteUri(@"http://{$ip_adress}/hello_there", vmWith2Networks);

            Assert.AreEqual(1, urls.Count, "Wrong number of urls returned");
            Assert.IsTrue(urls.Contains(new Uri(@"about:blank")), "Wrong url returned.");
        }

        [Test]
        public void TestIPAddressOfShutdownVM()
        {
            IXenObject shutdownVM = GetAnyVM(v => v.is_a_real_vm && v.power_state == vm_power_state.Halted);

            List<Uri> urls = Placeholders.SubstituteUri(@"http://{$ip_address}/hello_there", shutdownVM);

            Assert.AreEqual(1, urls.Count, "Wrong number of urls returned");
            Assert.IsTrue(urls.Contains(new Uri(@"about:blank")), "Wrong url returned.");
        }

        [Test]
        public void TestIPAddressOfStorage()
        {
            IXenObject sr = GetAnySR(s=>s.name_label == "NFS ISO library");

            List<Uri> urls = Placeholders.SubstituteUri(@"http://{$ip_address}/hello_there", sr);

            Assert.AreEqual(1, urls.Count, "Wrong number of urls returned");
            Assert.IsTrue(urls.Contains(new Uri(@"http://telos/hello_there")), "Wrong url returned.");
        }

        [Test]
        public void TestIPAddressOfHost()
        {
            IXenObject host = GetAnyHost(h => h.name_label == "inflames");

            List<Uri> urls = Placeholders.SubstituteUri(@"http://{$ip_address}/hello_there", host);

            Assert.AreEqual(1, urls.Count, "Wrong number of urls returned");
            Assert.IsTrue(urls.Contains(new Uri(@"http://10.80.224.75/hello_there")), "Wrong url returned.");
        }

        [Test]
        public void TestIPAddressTemplate()
        {
            IXenObject template = GetAnyUserTemplate();

            List<Uri> urls = Placeholders.SubstituteUri(@"http://{$ip_address}/hello_there", template);

            Assert.AreEqual(1, urls.Count, "Wrong number of urls returned");
            Assert.IsTrue(urls.Contains(new Uri(@"about:blank")), "Wrong url returned.");
        }
    }
}
