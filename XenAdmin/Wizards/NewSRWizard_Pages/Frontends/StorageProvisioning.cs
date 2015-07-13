/* Copyright (c) Citrix Systems Inc. 
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
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Help;
using System.Linq;
using System.Globalization;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class StorageProvisioning : XenTabPage
    {
        public StorageProvisioning()
        {
            InitializeComponent();
        }

        #region Accessors



        public Dictionary<string, string> SMConfig
        {
            get
            {
                var smconfig = new Dictionary<string, string>();

                if (radioButtonThinProvisioning.Checked)
                {
                    smconfig["allocation"] = "dynamic";
                    smconfig["allocation_quantum"] = (allocationQuantumNumericUpDown.Value / 100).ToString(CultureInfo.InvariantCulture);
                    smconfig["initial_allocation"] = (initialAllocationNumericUpDown.Value / 100).ToString(CultureInfo.InvariantCulture);
                }

                return smconfig;
            }
        }


        #endregion



        #region XenTabPage overrides

        public override void PopulatePage()
        {
            UpdateControls();

            OnPageUpdated();
        }

        public override string PageTitle { get { return Messages.STORAGE_PROVISIONING_METHOD_TITLE; } }

        public override string Text { get { return Messages.STORAGE_PROVISIONING_SETTINGS; } }

        public override string HelpID { get { return "helpid "; } }

        public override bool EnableNext()
        {
            return true;
        }

        #endregion

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();

        }

        public void DisableControls()
        {
            foreach (Control c in Controls)
                c.Enabled = false;
        }

        public void ResetControls()
        {
            foreach (Control c in Controls)
                c.Enabled = true;

            UpdateControls();
        }

        public void SetControlsUsingExistingSMConfig(Dictionary<string, string> smConfig)
        {
            decimal temp = 0;

            if (smConfig.ContainsKey("allocation") && smConfig["allocation"] == "dynamic")
            {
                radioButtonThickProvisioning.Checked = false;
                radioButtonThinProvisioning.Checked = true;

                if (smConfig.ContainsKey("initial_allocation") && decimal.TryParse(smConfig["initial_allocation"], out temp))
                    initialAllocationNumericUpDown.Value = temp * 100;

                if (smConfig.ContainsKey("allocation_quantum") && decimal.TryParse(smConfig["allocation_quantum"], out temp))
                    allocationQuantumNumericUpDown.Value = temp * 100;
            }
            else
            {
                radioButtonThickProvisioning.Checked = true;
                radioButtonThinProvisioning.Checked = false;
                initialAllocationNumericUpDown.ResetText();
                allocationQuantumNumericUpDown.ResetText();
            }
        }

        private void UpdateControls()
        {
            labelAllocationQuantum.Enabled =
            labelInitialAllocation.Enabled =
            labelPercent1.Enabled =
            labelPercent2.Enabled =
            allocationQuantumNumericUpDown.Enabled =
            initialAllocationNumericUpDown.Enabled = radioButtonThinProvisioning.Checked;
        }
    }
}

