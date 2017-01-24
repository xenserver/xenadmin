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
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;
using XenAdmin.Controls.XenSearch;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.RestoreSession;
using XenAdmin.Dialogs.WarningDialogs;
using XenAdmin.Dialogs.Wlb;
using XenAdmin.Network;
using XenAdmin.Plugins;
using XenAdmin.XenSearch;
using XenAPI;

namespace XenAdminTests.DialogTests.state1_xml.OtherDialogs
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class ChangeMasterPasswordDialogTest : DialogTest<ChangeMasterPasswordDialog>
    {
        protected override ChangeMasterPasswordDialog NewDialog()
        {
            return new ChangeMasterPasswordDialog(new byte[] { 1, 2, 3 });
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class DisableWLBDialogTest : DialogTest<DisableWLBDialog>
    {
        protected override DisableWLBDialog NewDialog()
        {
            return new DisableWLBDialog("foo");
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class EnterMasterPasswordDialogTest : DialogTest<EnterMasterPasswordDialog>
    {
        protected override EnterMasterPasswordDialog NewDialog()
        {
            return new EnterMasterPasswordDialog(new byte[] { 1, 2, 3 });
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class FolderChangeDialogTest_NoFolder : DialogTest<FolderChangeDialog>
    {
        protected override FolderChangeDialog NewDialog()
        {
            return new FolderChangeDialog("");
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class FolderChangeDialogTest_WithFolder : DialogTest<FolderChangeDialog>
    {
        protected override FolderChangeDialog NewDialog()
        {
            return new FolderChangeDialog(GetAnyFolder().Name);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class LicenseWarningDialogTest : DialogTest<LicenseWarningDialog>
    {
        protected override LicenseWarningDialog NewDialog()
        {
            return new LicenseWarningDialog("foo", "bar");
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class NewTagDialogTest : DialogTest<NewTagDialog>
    {
        protected override NewTagDialog NewDialog()
        {
            return new NewTagDialog(new List<string>());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class RepairSRDialogTest : DialogTest<RepairSRDialog>
    {
        protected override RepairSRDialog NewDialog()
        {
            return new RepairSRDialog(GetAnySR());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class SaveAndRestoreDialogTest : DialogTest<SaveAndRestoreDialog>
    {
        protected override SaveAndRestoreDialog NewDialog()
        {
            return new SaveAndRestoreDialog(false);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class SearchForCustomTest : DialogTest<SearchForCustom>
    {
        protected override SearchForCustom NewDialog()
        {
            // Setup copied from SearchFor.InitializeDictionaries()
            Dictionary<ObjectTypes, String> typeNames = new Dictionary<ObjectTypes, String>();
            Dictionary<String, ObjectTypes> dict = (Dictionary<String, ObjectTypes>)PropertyAccessors.Geti18nFor(PropertyNames.type);
            foreach (KeyValuePair<String, ObjectTypes> kvp in dict)
                typeNames[kvp.Value] = kvp.Key;
            return new SearchForCustom(typeNames, ObjectTypes.Server | ObjectTypes.VM);
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class ThreeButtonDialogTest_ctor1 : DialogTest<ThreeButtonDialog>
    {
        protected override ThreeButtonDialog NewDialog()
        {
            return new ThreeButtonDialog(
                new ThreeButtonDialog.Details(SystemIcons.Question, "foo"),
                new ThreeButtonDialog.TBDButton("bar", DialogResult.OK),
                new ThreeButtonDialog.TBDButton("baz", DialogResult.Cancel));
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class ThreeButtonDialogTest_ctor2 : DialogTest<ThreeButtonDialog>
    {
        protected override ThreeButtonDialog NewDialog()
        {
            return new ThreeButtonDialog(
                new ThreeButtonDialog.Details(SystemIcons.Warning, "foo", "bar"),
                new ThreeButtonDialog.TBDButton("baz", DialogResult.Yes, ThreeButtonDialog.ButtonType.ACCEPT),
                new ThreeButtonDialog.TBDButton("thing1", DialogResult.No, ThreeButtonDialog.ButtonType.NONE), 
                new ThreeButtonDialog.TBDButton("thing2", DialogResult.Cancel, ThreeButtonDialog.ButtonType.CANCEL));
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class UserDeviceDialogTest : DialogTest<UserDeviceDialog>
    {
        protected override UserDeviceDialog NewDialog()
        {
            return new UserDeviceDialog("1");
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class AllowUpdatesDialogTest : DialogTest<AllowUpdatesDialog>
    {
        protected override AllowUpdatesDialog NewDialog()
        {
            return new AllowUpdatesDialog(new PluginManager());
        }
    }
}
