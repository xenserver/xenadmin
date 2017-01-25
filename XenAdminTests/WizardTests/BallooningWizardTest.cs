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
using XenAdmin.Wizards;
using XenAdmin.Wizards.BallooningWizard_Pages;
using XenAPI;

namespace XenAdminTests.WizardTests.state4_xml.BallooningWizardTests
{
    [TestFixture, Category(TestCategories.UICategoryB)]
    class BallooningWizardTest_OneOld : WizardTest<BallooningWizard>
    {
        public BallooningWizardTest_OneOld()
            : base(new string[] { "Adjust memory settings" }, true, false)
        { }

        protected override BallooningWizard NewWizard()
        {
            List<VM> vms = new List<VM>();
            vms.Add(GetAnyVM(v => v.name_label == "Windows XP SP2 (1)"));
            return new BallooningWizard(vms);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    class BallooningWizardTest_OneNew : WizardTest<BallooningWizard>
    {
        public BallooningWizardTest_OneNew()
            : base(new string[] { "Adjust memory settings" }, true, false)
        { }

        protected override BallooningWizard NewWizard()
        {
            List<VM> vms = new List<VM>();
            vms.Add(GetAnyVM(v => v.name_label == "Windows Server 2003 x64 (1)"));
            return new BallooningWizard(vms);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    class BallooningWizardTest_TwoOld : WizardTest<BallooningWizard>
    {
        public BallooningWizardTest_TwoOld()
            : base(new string[] { "Select VMs", "Adjust memory settings" }, true, false)
        { }

        protected override BallooningWizard NewWizard()
        {
            List<VM> vms = new List<VM>();
            vms.Add(GetAnyVM(v => v.name_label == "Windows XP SP2 (1)"));
            vms.Add(GetAnyVM(v => v.name_label == "Windows XP SP2 (2)"));
            return new BallooningWizard(vms);
        }
    }

    // Press the ClearAll button so we can't advance past the first page
    [TestFixture, Category(TestCategories.UICategoryB)]
    class BallooningWizardTest_TwoOld_Unselect : WizardTest<BallooningWizard>
    {
        public BallooningWizardTest_TwoOld_Unselect()
            : base(new string[] { "Select VMs" }, false, false)
        { }

        protected override BallooningWizard NewWizard()
        {
            List<VM> vms = new List<VM>();
            vms.Add(GetAnyVM(v => v.name_label == "Windows XP SP2 (1)"));
            vms.Add(GetAnyVM(v => v.name_label == "Windows XP SP2 (2)"));
            return new BallooningWizard(vms);
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Select VMs")
            {
                MW(TestUtils.GetButton(wizard, "xenTabPageVMs.clearAllButton").PerformClick);
            }
        }
    }

    [TestFixture, Category(TestCategories.UICategoryB)]
    class BallooningWizardTest_TwoOld_SelectOne : WizardTest<BallooningWizard>
    {
        public BallooningWizardTest_TwoOld_SelectOne()
            : base(new string[] { "Select VMs", "Adjust memory settings" }, true, false)
        { }

        protected override BallooningWizard NewWizard()
        {
            List<VM> vms = new List<VM>();
            vms.Add(GetAnyVM(v => v.name_label == "Windows XP SP2 (1)"));
            vms.Add(GetAnyVM(v => v.name_label == "Windows XP SP2 (2)"));
            return new BallooningWizard(vms);
        }

        protected override void TestPage(string pageName)
        {
            if (pageName == "Select VMs")
            {
                MW(TestUtils.GetButton(wizard, "xenTabPageVMs.clearAllButton").PerformClick);
                MW(() => TestUtils.GetCheckedListBox(wizard, "xenTabPageVMs.listBox").SetItemChecked(0, true));
            }
        }
    }
}
