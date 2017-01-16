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
using XenAPI;


namespace XenAdmin.Controls.Ballooning
{
    public partial class VMMemoryControlsAdvanced : VMMemoryControlsEdit
    {
        public VMMemoryControlsAdvanced()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (vms == null || vms.Count == 0)
                return;

            // If !firstPaint, don't re-intialize, because it will pull the rug from under our own edits.
            if (!firstPaint)
                return;

            // Calculate the maximum legal value of dynamic minimum
            CalcMaxDynMin();

            // Spinners
            FreeSpinnerRanges();
            static_min = vm0.memory_static_min;

            memorySpinnerDynMin.Initialize(Messages.DYNAMIC_MIN_AMP, null, vm0.memory_dynamic_min, vm0.memory_static_max);
            memorySpinnerDynMax.Initialize(Messages.DYNAMIC_MAX_AMP, null, vm0.memory_dynamic_max, vm0.memory_static_max);
            memorySpinnerStatMax.Initialize(Messages.STATIC_MAX_AMP, null, vm0.memory_static_max, vm0.memory_static_max);
            SetIncrements();
            SetSpinnerRanges();
            firstPaint = false;
        }

        public override double dynamic_min
        {
            get
            {
                return memorySpinnerDynMin.Value;
            }
        }

        public override double dynamic_max
        {
            get
            {
                return memorySpinnerDynMax.Value;
            }
        }

        public override double static_max
        {
            get
            {
                return memorySpinnerStatMax.Value;
            }
        }

        private void SetIncrements()
        {
             memorySpinnerDynMin.Increment =  memorySpinnerDynMax.Increment = memorySpinnerStatMax.Increment = CalcIncrement(static_max, memorySpinnerDynMax.Units);
        }

        private void Spinners_ValueChanged(object sender, EventArgs e)
        {
            if (firstPaint)  // still initialising
                return;

            if (sender == memorySpinnerStatMax)
            {
                // Force supported envelope
                long min = (long)((double)static_max * GetMemoryRatio());
                if (memorySpinnerDynMin.Value < min)
                {
                    FreeSpinnerRanges();
                    memorySpinnerDynMin.Initialize(Messages.DYNAMIC_MIN_AMP, null, min, RoundingBehaviour.Up);  // This will also force DynMax up if necessary when its range is set in SetSpinnerRanges()
                }
                SetIncrements();
            }
            SetSpinnerRanges();
        }

        private void SetSpinnerRanges()
        {
            memorySpinnerDynMin.SetRange(DynMinSpinnerMin, DynMinSpinnerMax);
            memorySpinnerDynMax.SetRange(dynamic_min, static_max);
            memorySpinnerStatMax.SetRange(dynamic_max >= Util.BINARY_MEGA ? dynamic_max : Util.BINARY_MEGA, StatMaxSpinnerMax);
        }

        private void FreeSpinnerRanges()
        {
            memorySpinnerDynMin.SetRange(0, MemorySpinnerMax);
            memorySpinnerDynMax.SetRange(0, MemorySpinnerMax);
            memorySpinnerStatMax.SetRange(0, MemorySpinnerMax);
        }
    }
}

