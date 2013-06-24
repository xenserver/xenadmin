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

        public MemorySpinner()
        {
            InitializeComponent();
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
            long amountMB = Util.ToMB(amount, rounding);
            Spinner.Value = amountMB;
        }

        [Browsable(false)]
        public long Value
        {
            get
            {
                return (long)Spinner.Value * Util.BINARY_MEGA;
            }
        }

        public static void CalcMBRanges(long minBytes, long maxBytes, out long minMB, out long maxMB)
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

        public void SetRange(long min, long max)
        {
            if (min > max)
                return;  // Can happen when we are adjusting several simultaneously: can cause a stack overflow

            long minMB, maxMB;
            CalcMBRanges(min, max, out minMB, out maxMB);

            Spinner.Minimum = minMB;
            Spinner.Maximum = maxMB;
        }

        [Browsable(false)]
        public int Increment
        {
            set { Spinner.Increment = value; }
        }

        private void Spinner_ValueChanged(object sender, EventArgs e)
        {
            if (SpinnerValueChanged != null)
                SpinnerValueChanged(this, e);
        }

        private void Spinner_Leave(object sender, EventArgs e)
        {
            if (sender is NumericUpDown)
                ((Control)sender).Text = ((NumericUpDown)sender).Value.ToString();
        }
    }
}
