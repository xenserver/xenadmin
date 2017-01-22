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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.Ballooning;
using XenAPI;

namespace XenAdmin.Wizards.BallooningWizard_Pages
{
    public partial class MemorySettings : XenTabPage
    {
        public event EventHandler InstallTools;

        private VMMemoryControlsEdit memoryControls;

        public MemorySettings()
        {
            InitializeComponent();
        }

        public override string Text { get { return Messages.BALLOONING_PAGE_MEMORY_TEXT; } }

        public override string PageTitle { get { return Messages.BALLOONING_PAGE_MEMORY_PAGETITLE; } }

        public override string HelpID
        {
            get { return "Settings"; }
        }

        bool alreadyChosen = false;
        public List<VM> VMs
        {
            set
            {
                // Use any VM to decide which UI to show: they all have the same memory settings
                if (!alreadyChosen)
                    ChooseControls(value == null ? false : value[0].advanced_ballooning);
                alreadyChosen = true;
                memoryControls.VMs = value;
            }
        }

        private void ChooseControls(bool advanced)
        {
            if (advanced)
            {
                memoryControlsBasic.Visible = false;
                memoryControls = memoryControlsAdvanced;
            }
            else
            {
                memoryControlsAdvanced.Visible = false;
                memoryControls = memoryControlsBasic;
            }
            memoryControls.Visible = true;
        }

        public double static_max
        {
            get { return memoryControls.static_max; }
        }

        public double dynamic_min
        {
            get { return memoryControls.dynamic_min; }
        }

        public double dynamic_max
        {
            get { return memoryControls.dynamic_max; }
        }

        public bool AdvancedMode
        {
            get { return memoryControls == memoryControlsAdvanced; }
        }

        private void memoryControlsBasic_InstallTools(object sender, EventArgs e)
        {
            if (InstallTools != null)
                InstallTools(sender, e);  // just pass it on
        }

        public void UnfocusSpinners()
        {
            memoryControls.UnfocusSpinners();
        }
    }
}
