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

using System.Drawing;


namespace XenAdmin.Controls
{
    /// <summary>
    /// A control that displays Pool.ha_compute_max_host_failures_to_tolerate for a pool,
    /// automatically updating the displayed value
    /// </summary>
    public partial class HaNtolIndicatorSimple : HaNtolControl
    {
        public HaNtolIndicatorSimple()
        {
            InitializeComponent();
        }
          
		private void ToggleToleranceWarning(bool show)
		{
			pictureBoxStatus.Visible = show;
			labelStatus.Visible = show;
		}

        protected override void LoadCalculatingMode()
        {
            // load the spinner image if needed
            if (spinner.Image == null)
                spinner.Image = (Image)Properties.Resources.ajax_loader.Clone();
            spinner.Visible = true;
        }

        protected override void LoadCalculationSucceededMode(decimal value)
        {
            if (ntol == -1)
            {
                labelNumberOfServers.Text = string.Format(Messages.FAILOVER_TOLERANCE, value);
                ntol = (long)value;
            }

            labelMax.Text = string.Format(Messages.FAILOVER_MAX_CAPACITY, ntolMax);

            spinner.Visible = false;
            m_tlpCalFailure.Visible = false;
            ToggleToleranceWarning(Overcommitted);
        }

        protected override void LoadCalculationFailedMode()
        {
            m_tlpMaxCapacity.Visible = m_tlpTolerance.Visible = false;
            spinner.Visible = false;
            m_tlpCalFailure.Visible = true;
            ToggleToleranceWarning(false);
        }
    }
}
