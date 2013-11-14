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

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.SettingsPanels
{
    public partial class PoolGpuEditPage : UserControl, IEditPage
    {
        public Pool pool;
        private GPU_group[] gpu_groups;
        private allocation_algorithm currentAllocationAlgorithm;

        public PoolGpuEditPage()
        {
            InitializeComponent();
        }

        public allocation_algorithm AllocationAlgorithm
        {
            get
            {
                return radioButtonDepth.Checked ?
                    allocation_algorithm.depth_first :
                    radioButtonBreadth.Checked ? allocation_algorithm.breadth_first : allocation_algorithm.unknown;
            }
        }

        #region Implementation of VerticalTab

        public override string Text
        {
            get { return Messages.GPU; }
        }

        public string SubText
        {
            get 
            { 
                return radioButtonDepth.Checked ? Messages.GPU_PLACEMENT_POLICY_MAX_DENSITY : 
                    radioButtonBreadth.Checked ? Messages.GPU_PLACEMENT_POLICY_MAX_PERFORMANCE : Messages.GPU_PLACEMENT_POLICY_MIXED; }
        }

        public Image Image
        {
            get { return Resources._000_GetMemoryInfo_h32bit_16; }
        }

        #endregion

        #region Implementation of IEditPage

        public AsyncAction SaveSettings()
        {
            return new SetGpuPlacementPolicyAction(pool, AllocationAlgorithm);
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            Trace.Assert(clone is Pool || clone is Host);  // only Pools and Hosts should show this page

            pool = Helpers.GetPoolOfOne(clone.Connection);
            gpu_groups = pool.Connection.Cache.GPU_groups;
            PopulatePage();
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        {
            
        }

        public void Cleanup()
        {
            
        }

        public bool HasChanged
        {
            get
            {
                var allocationAlgorithm = radioButtonDepth.Checked ? 
                    allocation_algorithm.depth_first : 
                    radioButtonBreadth.Checked ? allocation_algorithm.breadth_first : allocation_algorithm.unknown;

                return !allocationAlgorithm.Equals(currentAllocationAlgorithm);
            }
        }

        #endregion

        private void PopulatePage()
        {
            currentAllocationAlgorithm = allocation_algorithm.unknown;

            foreach (GPU_group gpu_group in gpu_groups)
            {
                if (gpu_group.allocation_algorithm != currentAllocationAlgorithm && currentAllocationAlgorithm != allocation_algorithm.unknown)
                {
                    // a mixture of allocation algorithms
                    currentAllocationAlgorithm = allocation_algorithm.unknown;
                    break;
                }
                currentAllocationAlgorithm = gpu_group.allocation_algorithm;
            }

            radioButtonDepth.Checked = currentAllocationAlgorithm == allocation_algorithm.depth_first;
            radioButtonBreadth.Checked = currentAllocationAlgorithm == allocation_algorithm.breadth_first;

            radioButtonMixture.Checked = currentAllocationAlgorithm == allocation_algorithm.unknown;
            radioButtonMixture.Visible = currentAllocationAlgorithm == allocation_algorithm.unknown;
            radioButtonMixture.Enabled = currentAllocationAlgorithm == allocation_algorithm.unknown;
        }
    }
}
