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
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Network;
using XenAPI;

namespace XenAdminTests.DialogTests.state1_xml.PropertiesDialogTests
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class PropertiesDialogTest_Pool : VerticallyTabbedDialogTest<PropertiesDialog>
    {
        public PropertiesDialogTest_Pool()
            : base(new string[] { "General", "Custom Fields", "Email Options", "Power On"})
        { }

        protected override PropertiesDialog NewDialog()
        {
            return new PropertiesDialog(GetAnyPool());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class PropertiesDialogTest_Host : VerticallyTabbedDialogTest<PropertiesDialog>
    {
        public PropertiesDialogTest_Host()
            : base(new string[] { "General", "Custom Fields", "Alerts", "Multipathing", "Power On", "Log Destination"})
        { }

        protected override PropertiesDialog NewDialog()
        {
            return new PropertiesDialog(GetAnyHost());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class PropertiesDialogTest_VM_HVM : VerticallyTabbedDialogTest<PropertiesDialog>
    {
        public PropertiesDialogTest_VM_HVM()
            : base(new string[] { "General", "Custom Fields", "CPU and Memory",
                "Boot Options", "Start Options", "Alerts",  "Home Server", "GPU", "Advanced Options" })
        { }

        private bool IsHVM(VM vm)
        {
            return vm.IsHVM;
        }

        protected override PropertiesDialog NewDialog()
        {
            return new PropertiesDialog(GetAnyVM(IsHVM));
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class PropertiesDialogTest_VM_NotHVM : VerticallyTabbedDialogTest<PropertiesDialog>
    {
        public PropertiesDialogTest_VM_NotHVM()
            : base(new string[] { "General", "Custom Fields", "CPU and Memory",
                "Boot Options", "Start Options", "Alerts",  "Home Server" })
        { }

        private bool IsNotHVM(VM vm)
        {
            return !vm.IsHVM;
        }

        protected override PropertiesDialog NewDialog()
        {
            return new PropertiesDialog(GetAnyVM(IsNotHVM));
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class PropertiesDialogTest_DefaultTemplate : VerticallyTabbedDialogTest<PropertiesDialog>
    {
        public PropertiesDialogTest_DefaultTemplate()
            : base(new string[] { "General", "Custom Fields", "CPU and Memory",
                "Boot Options", "Start Options", "Alerts",  "Home Server" })
        { }

        protected override PropertiesDialog NewDialog()
        {
            return new PropertiesDialog(GetAnyDefaultTemplate(v => !v.IsHVM));
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class PropertiesDialogTest_UserTemplate_HVM : VerticallyTabbedDialogTest<PropertiesDialog>
    {
        public PropertiesDialogTest_UserTemplate_HVM()
            : base(new string[] { "General", "Custom Fields", "CPU and Memory",
                "Boot Options", "Start Options", "Alerts", "Home Server", "GPU", "Advanced Options" })
        { }

        private bool IsHVM(VM vm)
        {
            return vm.IsHVM;
        }

        protected override PropertiesDialog NewDialog()
        {
            return new PropertiesDialog(GetAnyUserTemplate(IsHVM));
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class PropertiesDialogTest_UserTemplate_NotHVM : VerticallyTabbedDialogTest<PropertiesDialog>
    {
        public PropertiesDialogTest_UserTemplate_NotHVM()
            : base(new string[] { "General", "Custom Fields", "CPU and Memory",
                "Boot Options", "Start Options", "Alerts", "Home Server" })
        { }

        private bool IsNotHVM(VM vm)
        {
            return !vm.IsHVM;
        }

        protected override PropertiesDialog NewDialog()
        {
            return new PropertiesDialog(GetAnyUserTemplate(IsNotHVM));
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class PropertiesDialogTest_SR : VerticallyTabbedDialogTest<PropertiesDialog>
    {
        public PropertiesDialogTest_SR()
            : base(new string[] { "General", "Custom Fields" })
        { }

        protected override PropertiesDialog NewDialog()
        {
            return new PropertiesDialog(GetAnySR());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA), Category(TestCategories.SmokeTest)]
    public class PropertiesDialogTest_VDI : VerticallyTabbedDialogTest<PropertiesDialog>
    {
        public PropertiesDialogTest_VDI()
            : base(new string[] { "General", "Custom Fields", "Size and Location" })
        { }

        protected override PropertiesDialog NewDialog()
        {
            IXenConnection connection = GetAnyConnection();

            // VDIs have a tab for each VM they are connected to,
            // so we choose a VDI with a VM and add the appropriate tabs.
            VDI vdi = null;
            foreach (VDI v in connection.Cache.VDIs)
            {
                if (v.VBDs.Count > 0)
                {
                    foreach (VBD vbd in v.Connection.ResolveAll(v.VBDs))
                    {
                        VM vm = vbd.Connection.Resolve(vbd.VM);
                        AddTab(vm.Name);
                    }
                    vdi = v;
                    break;
                }
            }
            Assert.IsNotNull(vdi, "No suitable VDI found");
            return new PropertiesDialog(vdi);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class PropertiesDialogTest_Network : VerticallyTabbedDialogTest<PropertiesDialog>
    {
        public PropertiesDialogTest_Network()
            : base(new string[] { "General", "Custom Fields", "Network Settings" })
        { }

        protected override PropertiesDialog NewDialog()
        {
            return new PropertiesDialog(GetAnyNetwork());
        }
    }
}
