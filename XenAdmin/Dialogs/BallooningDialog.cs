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

using System;
using System.Collections.Generic;
using XenAdmin.Controls.Ballooning;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class BallooningDialog : XenDialogBase
    {
        private readonly VM _vm;
        private VMMemoryControlsEdit memoryControls;

        public BallooningDialog()
        {
            InitializeComponent();
        }

        public BallooningDialog(VM vm)
            : base(vm.Connection)
        {
            InitializeComponent();
            _vm = vm;

            if (!Helpers.FeatureForbidden(vm.Connection, Host.RestrictDMC) && vm.UsesBallooning())
            {
                memoryControls = new VMMemoryControlsAdvanced();

                labelRubric.Text = vm.is_a_template
                    ? Messages.BALLOONING_RUBRIC_ADVANCED_TEMPLATE
                    : Messages.BALLOONING_RUBRIC_ADVANCED;
            }
            else
            {
                memoryControls = new VMMemoryControlsBasic();
                ((VMMemoryControlsBasic)memoryControls).InstallTools += BallooningDialog_InstallTools;

                labelRubric.Text = vm.is_a_template
                    ? Messages.BALLOONING_RUBRIC_TEMPLATE
                    : Messages.BALLOONING_RUBRIC;
            }

            panel1.Controls.Add(memoryControls);
        }

        protected override void OnLoad(EventArgs e)
        {
            memoryControls.VMs = new List<VM> {_vm};
            base.OnLoad(e);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            memoryControls.UnfocusSpinners();

            if (memoryControls.ChangeMemorySettings())
                Close();
        }

        private void BallooningDialog_InstallTools()
        {
            Close();
        }
    }
}
