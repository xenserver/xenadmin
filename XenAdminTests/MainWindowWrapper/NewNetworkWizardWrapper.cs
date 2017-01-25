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

using XenAdmin.Controls.NetworkingTab;
using XenAdmin.Core;
using XenAdmin.TabPages;
using System.Windows.Forms;
using XenAdmin.Wizards;
using XenAdmin.Wizards.NewNetworkWizard_Pages;

namespace XenAdminTests
{
    internal class NewNetworkWizardWrapper : TestWrapper<NewNetworkWizard>
    {
        public NewNetworkWizardWrapper(XenWizardBase item)
            : base(item)
        {
        }

        public NewNetworkWizardWrapper(Win32Window window)
            : base(window)
        {
        }

        public Button NextButton
        {
            get
            {
                return GetBaseClassField<Button>("buttonNext");
            }
        }

        public RadioButton SSPNButton
        {
            get
            {
                var page = new NetWTypeSelectWrapper(GetField<NetWTypeSelect>("pageNetworkType"));
                return page.SSPNButton;
            }
        }
    }

    internal class NetWTypeSelectWrapper : TestWrapper<NetWTypeSelect>
    {
        public NetWTypeSelectWrapper(NetWTypeSelect item)
            : base(item)
        { }

        public RadioButton SSPNButton
        {
            get { return GetField<RadioButton>("rbtnInternalNetwork"); }
        }
    }
}