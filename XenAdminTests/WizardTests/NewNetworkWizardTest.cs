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
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Wizards;

namespace XenAdminTests.WizardTests.cowley1_xml.NewNetworkWizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewExternalNetworkTest : WizardTest<NewNetworkWizard>
    {
        public NewExternalNetworkTest()
            : base(new string[] { "Select Type", "Name", "Network settings" }, true, false)
        { }

        protected override NewNetworkWizard NewWizard()
        {
            // Really we want the connection and pool of the chosen host, but we know
            // there is only one connection.
            return new NewNetworkWizard(GetAnyConnection(), GetAnyPool(), GetAnyHost());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Details")
                MW(TestUtils.GetRadioButton(wizard,"pageNetworkType.rbtnExternalNetwork").Select);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewInternalNetworkTest : WizardTest<NewNetworkWizard>
    {
        public NewInternalNetworkTest()
            : base(new string[] { "Select Type", "Name", "Network settings" }, true, false)
        { }

        protected override NewNetworkWizard NewWizard()
        {
            return new NewNetworkWizard(GetAnyConnection(), GetAnyPool(), GetAnyHost());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Select Type")
                MW(TestUtils.GetRadioButton(wizard,"pageNetworkType.rbtnInternalNetwork").Select);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewBondedNetworkTest : WizardTest<NewNetworkWizard>
    {
        public NewBondedNetworkTest()
            : base(new string[] { "Select Type", "Bond Members" }, false, false)
        { }

        protected override NewNetworkWizard NewWizard()
        {
            return new NewNetworkWizard(GetAnyConnection(), GetAnyPool(), GetAnyHost());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Select Type")
                MW(TestUtils.GetRadioButton(wizard, "pageNetworkType.rbtnBondedNetwork").Select);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    public class NewChinTest : WizardTest<NewNetworkWizard>
    {
        public NewChinTest()
            : base(new string[] { "Select Type", "Name", "Network settings" }, true, false)
        { }

        protected override NewNetworkWizard NewWizard()
        {
            return new NewNetworkWizard(GetAnyConnection(), GetAnyPool(), GetAnyHost());
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Select Type")
                MW(TestUtils.GetRadioButton(wizard, "pageNetworkType.rbtnCHIN").Select);
        }
    }
}
