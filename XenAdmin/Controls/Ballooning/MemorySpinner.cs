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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace XenAdmin.Controls.Ballooning
{
    public partial class MemorySpinner : UserControl
    {
        public event EventHandler SpinnerValueChanged;
        private decimal spinnerValue;
        private string previousUnitsValue;

        public MemorySpinner()
        {
            InitializeComponent();
            Units.SelectedItem = "GB";
            previousUnitsValue = Units.SelectedItem.ToString();
        }

        public void Initialize(string name, Image icon, long amount)
        {
            Initialize(name, icon, amount, RoundingBehaviour.Nearest);
        }

        public void Initialize(string name, Image icon, long amount, RoundingBehaviour rounding)
        {
            NameLabel.Text = name;
            if (icon != null && iconBox.Image == null)  // without this line, setting iconBox.Image causes another repaint, hence an infinite loop
                iconBox.Image = icon;
            SpinnerValue = Util.ToMB(amount, rounding);
            SpinnerValueDisplay = amount;
        }

        [Browsable(false)]
        public long Value
        {
            get
            {
                return (long)SpinnerValue * Util.BINARY_MEGA;
            }
        }

        decimal SpinnerValue
        {
            get
            {
                return spinnerValue;
            }

            set
            {
                spinnerValue = value;
            }
        }


        private decimal SpinnerValueDisplay
        {
            get
            {
                return Spinner.Value;
            }

            set
            {
                SpinnerValue = Util.ToMB((long)value, RoundingBehaviour.Nearest);
                if(Units.SelectedItem.ToString() == "GB")
                {
                    Spinner.Value = Util.ToGB((long)value, 1);
                }
                else
                {
                    Spinner.Value = Util.ToMB((long)value, RoundingBehaviour.Nearest);
                }
            }
        }

        public static void CalcMBRanges(long minBytes, long maxBytes, out decimal minMB, out decimal maxMB)
        {
            // Round ranges inwards to avoid bugs like CA-34487 and CA-34996
            minMB = Util.ToMB(minBytes, RoundingBehaviour.Up);
            maxMB = Util.ToMB(maxBytes, RoundingBehaviour.Down);
            if (minMB > maxMB)  // just in case...
            {
                minMB = Util.ToMB(minBytes);
                maxMB = Util.ToMB(maxBytes);
            }
        }

        public static void CalcGBRanges(long minBytes, long maxBytes, out decimal minGB, out decimal maxGB)
        {
            minGB = Util.ToGB(minBytes, 1);
            maxGB = Util.ToGB(maxBytes, 1);
        }

        public void SetRange(long min, long max)
        {
            if (min > max)
                return;  // Can happen when we are adjusting several simultaneously: can cause a stack overflow
            
            decimal spinnerMin, spinnerMax;
            if (Units.SelectedItem.ToString() == "MB")
            {                
                CalcMBRanges(min, max, out spinnerMin, out spinnerMax);
            }
            else
            {
                CalcGBRanges(min, max, out spinnerMin, out spinnerMax);               
            }
            Spinner.Minimum = spinnerMin;
            Spinner.Maximum = spinnerMax;
        }

        [Browsable(false)]
        public int Increment
        {
            set
            {
                if (Units.SelectedItem.ToString() == "MB")
                {
                    Spinner.Increment = value;
                }
                else
                {
                    Spinner.Increment = 1;
                }
            }
        }

        private void Spinner_ValueChanged(object sender, EventArgs e)
        {
            if (Units.SelectedItem.ToString() == "GB")
            {
                SpinnerValue = Spinner.Value * Util.BINARY_KILO;
            }
            else
            {
                SpinnerValue = Spinner.Value;
            }

            if (SpinnerValueChanged != null)
                SpinnerValueChanged(this, e);
        }

        private void Spinner_Leave(object sender, EventArgs e)
        {
            if (sender is NumericUpDown)
                ((Control)sender).Text = ((NumericUpDown)sender).Value.ToString();
        }

        private void Units_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if the user chose a different unit. If the chosen unit is not different form the previous, then nothing changes.
            if (Units.SelectedItem.ToString() != previousUnitsValue)
            {
                decimal min = Spinner.Minimum;
                decimal max = Spinner.Maximum;
                decimal val = Spinner.Value;
                if (Units.SelectedItem.ToString() == "GB")
                {
                    // In this situation we are changing from MB to GB. In order for the new value to be between the spinner's minimum
                    // and maximum, we need to set the minimum. The minimum's value will be now in GB, instead of MB.
                    Spinner.Minimum = min / Util.BINARY_KILO;
                    SpinnerValueDisplay = val * Util.BINARY_MEGA;
                }
                else
                {
                    // In this situation we are changing from GB to MB. In order for the new value to be between the spinner's minimum 
                    // and maximum, we need to set the maximum. The maximum's value will be now in MB, instead of GB.
                    Spinner.Maximum = max * Util.BINARY_KILO;                    
                    SpinnerValueDisplay = val * Util.BINARY_GIGA;
                }
                previousUnitsValue = Units.SelectedItem.ToString();
            }
        }
    }
}
