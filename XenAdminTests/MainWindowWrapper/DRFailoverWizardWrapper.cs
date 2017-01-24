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

using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Wizards;
using XenAdmin.Wizards.DRWizards;

namespace XenAdminTests 
{
    class DRFailoverWizardWrapper : TestWrapper<XenWizardBase>
    {
        public DRFailoverWizardWrapper(XenWizardBase item)
            : base(item)
        {
        }

        public DRFailoverWizardWrapper(Win32Window window)
            : base(window)
        {
        }

        public RadioButton DryRunRadioButton
        {
            get
            {
                DRFailoverWizardWelcomePage wp = GetField<DRFailoverWizardWelcomePage>("DRFailoverWizardWelcomePage");
                return TestUtils.GetRadioButton(wp, "radioButtonDryrun");
            }
        }

        public RadioButton FailbackRadioButton
        {
            get
            {
                DRFailoverWizardWelcomePage wp = GetField<DRFailoverWizardWelcomePage>("DRFailoverWizardWelcomePage");
                return TestUtils.GetRadioButton(wp, "radioButtonFailback");
            }
        }

        public RadioButton FailoverRadioButton
        {
            get
            {
                DRFailoverWizardWelcomePage wp = GetField<DRFailoverWizardWelcomePage>("DRFailoverWizardWelcomePage");
                return TestUtils.GetRadioButton(wp, "radioButtonFailover");
            }
        }
        
        public Button CancelButton
        {
            get
            {
                return GetBaseClassField<Button>("buttonCancel");
            }
        }
    }
}
