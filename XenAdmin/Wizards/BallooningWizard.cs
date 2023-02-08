/* Copyright (c) Cloud Software Group, Inc. 
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

using System.Collections.Generic;
using XenAdmin.Controls;
using XenAdmin.Wizards.BallooningWizard_Pages;
using XenAPI;

namespace XenAdmin.Wizards
{
    public partial class BallooningWizard : XenWizardBase
    {
        private readonly ChooseVMs xenTabPageVMs;
        private readonly MemorySettings xenTabPageSettings;

        public BallooningWizard(List<VM> vms)
            : base(vms[0].Connection)
        {
            InitializeComponent();

            xenTabPageVMs = new ChooseVMs {CheckedVMs = vms};
            xenTabPageSettings = new MemorySettings();
            xenTabPageSettings.InstallTools += xenTabPageSettings_InstallTools;

            AddPages(xenTabPageVMs, xenTabPageSettings);

            if (vms.Count == 1)  // if there is only one VM, don't offer a choice
            {
                xenTabPageVMs.DisableStep = true;
                NextStep();
            }
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            if (senderPage.GetType() == typeof(ChooseVMs))
                xenTabPageSettings.VMs = xenTabPageVMs.CheckedVMs;
        }

        private void xenTabPageSettings_InstallTools()
        {
            Close();
        }
    }
}

