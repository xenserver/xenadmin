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

using NUnit.Framework;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.WarningDialogs;
using XenAdmin.Dialogs.VMDialogs;

namespace XenAdminTests.DialogTests.state1_xml.DialogsForHost
{
    [TestFixture, Category(TestCategories.UICategoryA)]
    public class ChangeServerPasswordDialogTest : DialogTest<ChangeServerPasswordDialog>
    {
        protected override ChangeServerPasswordDialog NewDialog()
        {
            return new ChangeServerPasswordDialog(GetAnyHost());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class EvacuateHostDialogTest : DialogTest<EvacuateHostDialog>
    {
        protected override EvacuateHostDialog NewDialog()
        {
            return new EvacuateHostDialog(GetAnyHost());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class HostRequiresRebootDialogTest : DialogTest<HostRequiresRebootDialog>
    {
        protected override HostRequiresRebootDialog NewDialog()
        {
            return new HostRequiresRebootDialog(GetAnyHost());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class RemoveCrashDumpsWarningDialogTest : DialogTest<RemoveCrashDumpsWarningDialog>
    {
        protected override RemoveCrashDumpsWarningDialog NewDialog()
        {
            return new RemoveCrashDumpsWarningDialog(GetAnyHost());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class SelectVMsToSuspendDialogTest : DialogTest<SelectVMsToSuspendDialog>
    {
        protected override SelectVMsToSuspendDialog NewDialog()
        {
            return new SelectVMsToSuspendDialog(GetAnyHost());
        }
    }

    [TestFixture, Category(TestCategories.UICategoryA)]
    public class NewPoolDialogTest : DialogTest<NewPoolDialog>
    {
        protected override NewPoolDialog NewDialog()
        {
            return new NewPoolDialog(GetAnyHost());
        }
    }
}
