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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Core;

using XenAPI;
using XenAdmin.Actions;
using System.Globalization;


namespace XenAdmin.SettingsPanels
{
    public partial class VMAdvancedEditPage : UserControl, IEditPage
    {
        private static double SHADOW_MULTIPLIER_GENERAL_USE = 1.0;
        private static double SHADOW_MULTIPLIER_CPS = 4.0;
        private readonly ToolTip m_invalidParamToolTip;
        private VM vm = null;
        private bool showCpsOptimisation;

        public VMAdvancedEditPage()
        {
            InitializeComponent();

            Text = Messages.ADVANCED_OPTIONS;

            m_invalidParamToolTip = new ToolTip
                {
                    IsBalloon = true,
                    ToolTipIcon = ToolTipIcon.Warning,
                    ToolTipTitle = Messages.INVALID_PARAMETER
                };
            this.CPSOptimizationRadioButton.Visible = showCpsOptimisation = !HiddenFeatures.CPSOptimizationHidden;
        }

        public String SubText
        {
            get
            {
                if (GeneralOptimizationRadioButton.Checked)
                    return GeneralOptimizationRadioButton.Text.Replace("&", "");

                else if (CPSOptimizationRadioButton.Checked)
                    return CPSOptimizationRadioButton.Text.Replace("&", "");

                else
                {
                    return String.Format(Messages.SHADOW_MEMORY_MULTIPLIER, ShadowMultiplierTextBox.Text);
                }
            }
        }

        public Image Image
        {
            get
            {
                return Properties.Resources._002_Configure_h32bit_16;
            }
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            vm = clone as VM;
            Repopulate();
        }

        public void Repopulate()
        {
            if (vm == null)
                return;

            if (vm.power_state == vm_power_state.Suspended || vm.power_state == vm_power_state.Paused)
            {
                CPSOptimizationRadioButton.Enabled = GeneralOptimizationRadioButton.Enabled = ManualOptimizationRadioButton.Enabled = false;
                labelShadowMultiplier.Enabled = ShadowMultiplierTextBox.Enabled = false;
                iconWarning.Visible = labelWarning.Visible = true;
            }
            else
                iconWarning.Visible = labelWarning.Visible = false;

            double mul = vm.HVM_shadow_multiplier;
            if (mul == SHADOW_MULTIPLIER_GENERAL_USE)
            {
                GeneralOptimizationRadioButton.Checked = true;
            }
            else if (mul == SHADOW_MULTIPLIER_CPS && showCpsOptimisation)
            {
                CPSOptimizationRadioButton.Checked = true;
            }
            else
            {
                ManualOptimizationRadioButton.Checked = true;
            }
            ShadowValue = mul;
        }

        public bool HasChanged
        {
            get { return ShadowValue != vm.HVM_shadow_multiplier; }
        }

        private double ShadowValue
        {
            get
            {
                double v;
                return double.TryParse(ShadowMultiplierTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out v)
                    ? v
                    : -1;
            }
            set
            {
                ShadowMultiplierTextBox.Text = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public void ShowLocalValidationMessages()
        {
            if (!ValidToSave)
            {
                HelpersGUI.ShowBalloonMessage(ShadowMultiplierTextBox,
                   Messages.SHADOW_MEMORY_MULTIPLIER_VALUE,
                   m_invalidParamToolTip);
            }
        }

        public void Cleanup()
        {
            if (m_invalidParamToolTip != null)
                m_invalidParamToolTip.Dispose();
        }

        public bool ValidToSave
        {
            get
            {
                double v;
                return double.TryParse(ShadowMultiplierTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out v)
                       && v >= 1.0;
            }
        }

        public AsyncAction SaveSettings()
        {
            if (vm.power_state == vm_power_state.Running)
            {
                return new DelegatedAsyncAction(
                    vm.Connection,
                    Messages.ACTION_CHANGE_SHADOW_MULTIPLIER,
                    string.Format(Messages.ACTION_CHANGING_SHADOW_MULTIPLIER_FOR, vm),
                    null,
                    delegate(Session session) { VM.set_shadow_multiplier_live(session, vm.opaque_ref, ShadowValue); },
                    true,
                    "vm.set_shadow_multiplier_live"
                );
            }
            else
            {
                vm.HVM_shadow_multiplier = ShadowValue;
                return null;
            }
        }

        private void GeneralOptimizationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (GeneralOptimizationRadioButton.Checked) 
                ShadowValue = SHADOW_MULTIPLIER_GENERAL_USE;
        }

        private void CPSOptimizationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (CPSOptimizationRadioButton.Checked)
                ShadowValue = SHADOW_MULTIPLIER_CPS;
        }

        private void ShadowMultiplierTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ShadowMultiplierTextBox.Focused)
                ManualOptimizationRadioButton.Checked = true;
        }
    }
}
