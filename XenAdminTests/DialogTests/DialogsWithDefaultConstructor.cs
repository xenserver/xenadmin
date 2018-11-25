﻿/* Copyright (c) Citrix Systems, Inc. 
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
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.Network;
using XenAdmin.Dialogs.RestoreSession;
using XenAdmin.Dialogs.WarningDialogs;
using XenAdmin.Dialogs.Wlb;

namespace XenAdminTests.DialogTests.state1_xml.DialogsWithDefaultConstructor
{
    public abstract class DialogWithDefaultConstructorTest<T> : DialogTest<T> where T: XenDialogBase, new()
    {
        protected override T NewDialog()
        {
            return new T();
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class AboutDialogTest : DialogWithDefaultConstructorTest<AboutDialog> { }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class BondPropertiesTest : DialogWithDefaultConstructorTest<BondProperties> { }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class CloseXenCenterWarningDialogTest : DialogWithDefaultConstructorTest<CloseXenCenterWarningDialog> { }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class ConfigureGraphsDialogTest : DialogWithDefaultConstructorTest<GraphDetailsDialog> { }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class ConfirmDeconfigureWLBDialogTest : DialogWithDefaultConstructorTest<ConfirmDeconfigureWLBDialog> { }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class InputPromptDialogTest : DialogWithDefaultConstructorTest<InputPromptDialog>
    {
        protected override void RunBeforeShow()
        {
            dialog.Text = "Foo";
            dialog.PromptText = "Bar";
            dialog.InputText = "stuff";
            dialog.HelpID = "NewFolderDialog";
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class LegalNoticesDialogTest : DialogWithDefaultConstructorTest<LegalNoticesDialog> { }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class LoadSessionDialogTest : DialogWithDefaultConstructorTest<LoadSessionDialog> { }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class NameAndConnectionPromptTest : DialogWithDefaultConstructorTest<NameAndConnectionPrompt> { }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class PasswordsRequestDialogTest : DialogWithDefaultConstructorTest<PasswordsRequestDialog> { }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class SelectHostDialogTest : DialogWithDefaultConstructorTest<SelectHostDialog> { }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class SetMasterPasswordDialogTest : DialogWithDefaultConstructorTest<SetMasterPasswordDialog> { }

}
