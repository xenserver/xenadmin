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
using XenAdmin.Core;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class StorageProvisioning : XenTabPage
    {
        private const int DecimalPlacesMB = 0;
        private const int DecimalPlacesGB = 3;
        private const int IncrementMB = 256;
        private const int IncrementGB = 1;
        private long srSize;

        public StorageProvisioning()
        {
            InitializeComponent();
        }

        #region Accessors



        public Dictionary<string, string> SMConfig
        {
            get
            {
                if (radioButtonThinProvisioning.Checked)
                {
                    return thinProvisioningAllocationsControl.SMConfig;
                }
                return new Dictionary<string, string>();
            }
        }

        public long SRSize 
        { 
            get
            {
                return srSize;
            }
             
            set
            {
                if(srSize != value)
                {
                    srSize = value;
                    thinProvisioningAllocationsControl.SRSize = SRSize;
                }
            }
        }

        #endregion



        #region XenTabPage overrides

        public override void PopulatePage()
        {            
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
        
        public void DisableControls()
        {
            foreach (Control c in Controls)
                c.Enabled = false;
        }

        public void ResetControls()
        {
            foreach (Control c in Controls)
                c.Enabled = true;
        }

        public void SetControlsUsingExistingSMConfig(Dictionary<string, string> smConfig)
        {
            if (smConfig.ContainsKey("allocation") && smConfig["allocation"] == "xlvhd")
            {
                radioButtonThickProvisioning.Checked = false;
                radioButtonThinProvisioning.Checked = true;

                thinProvisioningAllocationsControl.SetControlsUsingExistingSMConfig(smConfig);               
            }
            else
            {
                radioButtonThickProvisioning.Checked = true;
                radioButtonThinProvisioning.Checked = false;
                thinProvisioningAllocationsControl.ResetControlValues();
            }
        }

        private void thinProvisioningAllocationsControl_Enter(object sender, EventArgs e)
        {
            radioButtonThickProvisioning.Checked = false;
            radioButtonThinProvisioning.Checked = true;
        }       
    }
}

