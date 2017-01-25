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
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using System.Globalization;

namespace XenAdmin.Controls
{
    public partial class ThinProvisioningParametersControl : UserControl
    {
        private SR sr = null;

        private string previousUnitsValueInitAlloc;
        private string previousUnitsValueIncrAlloc;
        private const int DecimalPlacesMB = 0;
        private const int DecimalPlacesGB = 3;
        private const int IncrementMB = 256;
        private const int IncrementGB = 1;

        public ThinProvisioningParametersControl()
        {
            InitializeComponent();
            incremental_allocation_units.SelectedItem = previousUnitsValueIncrAlloc = Messages.VAL_MEGB;
            initial_allocation_units.SelectedItem = previousUnitsValueInitAlloc = Messages.VAL_MEGB;
            SR_incremental_allocation_limits_info_label.Text = string.Format(Messages.SR_QUANTUM_ALLOCATION_LIMITS_INFO, 
                                                                 Helpers.XLVHD_MIN_ALLOCATION_QUANTUM_DIVISOR,
                                                                 Util.MemorySizeStringSuitableUnits(Helpers.XLVHD_MIN_ALLOCATION_QUANTUM, false),
                                                                 Helpers.XLVHD_MAX_ALLOCATION_QUANTUM_DIVISOR);
        }

        public SR SR
        {
            set
            {
                sr = value;
                InitializeNumericUpDowns();
            }
        }

        public long SRSize
        {
            private get
            {
                if (sr != null)
                    return sr.physical_size;

                return 0;
            }
            set
            {
                SR = new SR() { physical_size = value };
            }
        }

        public Dictionary<string, string> SMConfig
        {
            get
            {
                var smconfig = new Dictionary<string, string>();

                smconfig["allocation"] = "xlvhd";
                smconfig["allocation_quantum"] = (incremental_allocation_units.SelectedItem.ToString() == Messages.VAL_MEGB ? (long)(allocationQuantumNumericUpDown.Value * Util.BINARY_MEGA)
                                                                                                                            : (long)(allocationQuantumNumericUpDown.Value * Util.BINARY_GIGA))
                                                                                                                            .ToString(CultureInfo.InvariantCulture);
                smconfig["initial_allocation"] = (initial_allocation_units.SelectedItem.ToString() == Messages.VAL_MEGB ? (long)(initialAllocationNumericUpDown.Value * Util.BINARY_MEGA)
                                                                                                                        : (long)(initialAllocationNumericUpDown.Value * Util.BINARY_GIGA))
                                                                                                                        .ToString(CultureInfo.InvariantCulture);
                return smconfig;
            }
        }


        private void InitializeNumericUpDowns()
        {
            // Because we do not initialize the NumericUpDown with values from an existing sm_config
            // the value passed to the setup functions is -1. 
            SetUpInitAllocationNumericUpDown(-1);
            SetUpIncrAllocationNumericUpDown(-1);
        }

        // The value parameter is -1 if we do not use an existing SM Config. Otherwise  
        // it is the value of the initial_allocation in the SM config. The value received is in bytes.
        private void SetUpInitAllocationNumericUpDown(decimal value)
        {
            Helpers.AllocationBounds allocBounds = Helpers.SRInitialAllocationBounds(SRSize);

            if (value != -1)
            {
                allocBounds = new Helpers.AllocationBounds(allocBounds.Min, allocBounds.Max, value);
            }

            initial_allocation_units.SelectedItem = previousUnitsValueInitAlloc = allocBounds.Unit;
            SetNumUpDownIncrementAndDecimals(initialAllocationNumericUpDown, allocBounds.Unit);

            initialAllocationNumericUpDown.Minimum = allocBounds.MinInUnits;
            initialAllocationNumericUpDown.Maximum = allocBounds.MaxInUnits;
            initialAllocationNumericUpDown.Value = allocBounds.DefaultValueInUnits;
        }

        // The value parameter is -1 if we do not use an existing SM Config. Otherwise  
        // it is the value of the allocation_quantum in the SM config. 
        private void SetUpIncrAllocationNumericUpDown(decimal value)
        {
            Helpers.AllocationBounds allocBounds = Helpers.SRIncrementalAllocationBounds(SRSize);
            if (value != -1)
            {
                allocBounds = new Helpers.AllocationBounds(allocBounds.Min, allocBounds.Max, value);
            }
            
            incremental_allocation_units.SelectedItem = previousUnitsValueInitAlloc = allocBounds.Unit;
            SetNumUpDownIncrementAndDecimals(allocationQuantumNumericUpDown, allocBounds.Unit);

            allocationQuantumNumericUpDown.Minimum = allocBounds.MinInUnits;
            allocationQuantumNumericUpDown.Maximum = allocBounds.MaxInUnits;
            allocationQuantumNumericUpDown.Value = allocBounds.DefaultValueInUnits;
        }

        private void SetNumUpDownIncrementAndDecimals(NumericUpDown upDown, string units)
        {
            if (units == Messages.VAL_GIGB)
            {
                upDown.DecimalPlaces = DecimalPlacesGB;
                upDown.Increment = IncrementGB;
            }
            else
            {
                upDown.DecimalPlaces = DecimalPlacesMB;
                upDown.Increment = IncrementMB;
            }
        }

        public void SetControlsUsingExistingSMConfig(Dictionary<string, string> smConfig)
        {
            long temp = 0;

            if (smConfig.ContainsKey("allocation") && smConfig["allocation"] == "xlvhd")
            {
                if (smConfig.ContainsKey("initial_allocation") && long.TryParse(smConfig["initial_allocation"], out temp))
                    SetUpInitAllocationNumericUpDown(temp);

                if (smConfig.ContainsKey("allocation_quantum") && long.TryParse(smConfig["allocation_quantum"], out temp))
                    SetUpIncrAllocationNumericUpDown(temp);
            }
        }

        private void initial_allocation_units_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateValuesWhenUnitsChanged(initialAllocationNumericUpDown,
                                         previousUnitsValueInitAlloc,
                                         initial_allocation_units.SelectedItem.ToString());
            previousUnitsValueInitAlloc = initial_allocation_units.SelectedItem.ToString();
        }

        private void incremental_allocation_units_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateValuesWhenUnitsChanged(allocationQuantumNumericUpDown,
                                         previousUnitsValueIncrAlloc,
                                         incremental_allocation_units.SelectedItem.ToString());
            previousUnitsValueIncrAlloc = incremental_allocation_units.SelectedItem.ToString();
        }

        public static void UpdateValuesWhenUnitsChanged(NumericUpDown upDown, string previousUnits, string newUnits)
        {
            if (previousUnits == newUnits)
                return;

            decimal min = upDown.Minimum;
            decimal max = upDown.Maximum;
            if (newUnits == Messages.VAL_MEGB)
            {
                upDown.Maximum *= Util.BINARY_KILO;
                upDown.Value *= Util.BINARY_KILO;
                upDown.Minimum *= Util.BINARY_KILO;
                upDown.DecimalPlaces = DecimalPlacesMB;
                upDown.Increment = IncrementMB;
            }
            else
            {
                upDown.Minimum /= Util.BINARY_KILO;
                upDown.Value /= Util.BINARY_KILO;
                upDown.Maximum /= Util.BINARY_KILO;
                upDown.DecimalPlaces = DecimalPlacesGB;
                upDown.Increment = IncrementGB;
            }
        }
    }
}
