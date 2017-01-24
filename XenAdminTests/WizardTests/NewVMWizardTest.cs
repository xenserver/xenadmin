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
using XenAdmin.Network;
using XenAdmin.Wizards;
using XenAPI;
using XenAdmin.Wizards.NewVMWizard;
using XenAdmin.Core;
using System.Threading;
using System.Xml;

namespace XenAdminTests.WizardTests.state1_xml.NewVMWizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB), Category(TestCategories.SmokeTest)]
    public class NewVMWizardTestSpecificHost : WizardTest<NewVMWizard>
    {
        public NewVMWizardTestSpecificHost()
            : base(new string[] {"Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" }, true, true)
        { }

        protected override NewVMWizard NewWizard()
        {
            Host host = GetAnyHost();
            return new NewVMWizard(host.Connection, null, host);
        }

        protected override void RunAfter()
        {
            while (!wizard.Action.IsCompleted)
                Thread.Sleep(1000);

            Assert.True(wizard.Action.Succeeded);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardTestUnspecifiedHost : WizardTest<NewVMWizard>
    {
        public NewVMWizardTestUnspecifiedHost()
            : base(new string[] {"Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" }, true, true)
        { }

        protected override NewVMWizard NewWizard()
        {
            IXenConnection connection = GetAnyConnection();
            return new NewVMWizard(connection, null, null);
        }

        protected override void RunAfter()
        {
            while (!wizard.Action.IsCompleted)
                Thread.Sleep(1000);

            Assert.True(wizard.Action.Succeeded);
        }
    }
}

namespace XenAdminTests.WizardTests.state4_xml.NewVMWizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardTestLinuxTemplate : WizardTest<NewVMWizard>
    {
        public NewVMWizardTestLinuxTemplate()
            : base(new string[] {"Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" }, true, true)
        { }

        protected override NewVMWizard NewWizard()
        {
            VM template =
                GetAnyDefaultTemplate(new Predicate<VM>(delegate(VM vm)
                                                                         {
                                                                             return Helpers.GetName(vm).ToLowerInvariant().Contains("centos");
                                                                         }));
            Assert.NotNull(template, "CentOS template not found.");
            return new NewVMWizard(template.Connection, template, null);
        }

        protected override void RunAfter()
        {
            while (!wizard.Action.IsCompleted)
                Thread.Sleep(1000);

            Assert.True(wizard.Action.Succeeded);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB), Category(TestCategories.SmokeTest)]
    public class NewVMWizardTestWindowsTemplate : WizardTest<NewVMWizard>
    {
        public NewVMWizardTestWindowsTemplate()
            : base(new string[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" }, true, true)
        { }

        protected override NewVMWizard NewWizard()
        {
            VM template = GetAnyDefaultTemplate(new Predicate<VM>(
                delegate(VM vm)
                {
                    if (!Helpers.GetName(vm).ToLowerInvariant().Contains("windows"))
                        return false;
                    XmlNode xml = vm.ProvisionXml;
                    return (xml != null && xml.FirstChild != null && 
                        long.Parse(xml.FirstChild.Attributes["size"].Value) < (long)20 * (1 << 30));  // less than 20GB
                }));
            Assert.NotNull(template, "Windows template not found.");
            return new NewVMWizard(template.Connection, template, null);
        }

        protected override void RunAfter()
        {
            while (!wizard.Action.IsCompleted)
                Thread.Sleep(1000);

            Assert.True(wizard.Action.Succeeded);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardTestWindowsTemplate_TooBig : WizardTest<NewVMWizard>
    {
        public NewVMWizardTestWindowsTemplate_TooBig()
            : base(new string[] { "Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage" }, false, false)
        { }

        protected override NewVMWizard NewWizard()
        {
            VM template = GetAnyDefaultTemplate(new Predicate<VM>(
                delegate(VM vm)
                {
                    XmlNode xml = vm.ProvisionXml;
                    return (xml != null && xml.FirstChild != null &&
                        long.Parse(xml.FirstChild.Attributes["size"].Value) > (long)23 * (1 << 30));  // over 23GB
                }));
            Assert.NotNull(template, "Large template not found.");
            return new NewVMWizard(template.Connection, template, null);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewVMWizardTestUserTemplate : WizardTest<NewVMWizard>
    {
        public NewVMWizardTestUserTemplate()
            : base(new string[] {"Template", "Name", "Installation Media", "Home Server", "CPU && Memory", "Storage", "Networking", "Finish" }, true, true)
        { }

        protected override NewVMWizard NewWizard()
        {
            VM template = GetAnyUserTemplate();
            Assert.NotNull(template, "User template not found.");
            return new NewVMWizard(template.Connection, template, null);
        }

        protected override void RunAfter()
        {
            while (!wizard.Action.IsCompleted)
                Thread.Sleep(1000);

            Assert.True(wizard.Action.Succeeded);
        }
    }

}
