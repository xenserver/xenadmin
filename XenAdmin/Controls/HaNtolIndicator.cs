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
using System.Drawing;
using System.Windows.Forms;


namespace XenAdmin.Controls
{
    /// <summary>
    /// A control that displays Pool.ha_compute_max_host_failures_to_tolerate for a pool,
    /// automatically updating the displayed value
    /// </summary>
    public partial class HaNtolIndicator : HaNtolControl
    {
        public HaNtolIndicator()
        {
            InitializeComponent();
        }

        protected override void LoadCalculatingMode()
        {
            tableStatus.Enabled = false;
            labelNumberOfServers.Enabled = false;
            numericUpDownCapacity.Enabled = false;
            labelMax.Enabled = false;
            // load the spinner image if needed
            if (spinner.Image == null)
                spinner.Image = (Image)Properties.Resources.ajax_loader.Clone();
            spinner.Visible = true;
        }

        protected override void LoadCalculationSucceededMode(decimal value)
        {
            if (ntol == -1)
            {
                // first population or reset, set the ntol up-down to the pool
                //current or if HA is being initialised to the max possible
                numericUpDownCapacity.Value = value;
                ntol = (long)value;
                // Will normally be set by numericUpDownCapacity_ValueChanged:
                //but not if value = 0: this is the cause of CA-40907
            }

            labelMax.Text = string.Format(Messages.MAX_BRACKETS, ntolMax);

            labelNumberOfServers.Enabled = true;
            numericUpDownCapacity.Enabled = true;
            labelMax.Enabled = true;

            spinner.Visible = false;
            tableStatus.Visible = Overcommitted;
            tableStatus.Enabled = true;
        }

        protected override void LoadCalculationFailedMode()
        {
            groupBoxControls.Enabled = false;
            spinner.Visible = false;
            tableStatus.Visible = false;
        }

        private void numericUpDownCapacity_ValueChanged(object sender, EventArgs e)
        {
            ntol = (long)numericUpDownCapacity.Value;
            tableStatus.Visible = Overcommitted;
            OnNtolKnownChanged();
        }

        private void tableServerFailureLimit_Resize(object sender, EventArgs e)
        {
            if (ParentForm == null || ParentForm.WindowState != FormWindowState.Minimized) 
                groupBoxControls.Height = tableServerFailureLimit.Height + 20;
        }
    }
}
