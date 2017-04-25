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

using System.ComponentModel;
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Controls
{
    public partial class GpuPlacementPolicyPanel : UserControl
    {
        public GpuPlacementPolicyPanel()
        {
            InitializeComponent();
            GPU_group_CollectionChangedWithInvoke = Program.ProgramInvokeHandler(GPU_group_CollectionChanged);
        }

        IXenObject xenObject;

        private readonly CollectionChangeEventHandler GPU_group_CollectionChangedWithInvoke;

        public IXenObject XenObject
        {
            set
            {
                System.Diagnostics.Trace.Assert(value is Pool || value is Host);
                xenObject = value;

                RegisterHandlers();

                PopulatePage();

                Refresh();
            }
        }

        private void PopulatePage()
        {
            var currentAllocationAlgorithm = allocation_algorithm.unknown;

            foreach (GPU_group gpu_group in xenObject.Connection.Cache.GPU_groups)
            {
                if (gpu_group.allocation_algorithm != currentAllocationAlgorithm && currentAllocationAlgorithm != allocation_algorithm.unknown)
                {
                    // a mixture of allocation algorithms
                    currentAllocationAlgorithm = allocation_algorithm.unknown;
                    break;
                }
                currentAllocationAlgorithm = gpu_group.allocation_algorithm;
            }
            placementPolicyLabel.Text = string.Format(Messages.GPU_PLACEMENT_POLICY_DESCRIPTION,
                                                      PlacementPolicyWrapper.ToString(currentAllocationAlgorithm));
        }


        private void GPU_group_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    RegisterGpuGroupHandlers((GPU_group) e.Element);
                    break;
                case CollectionChangeAction.Remove:
                    UnregisterGpuGroupHandlers((GPU_group) e.Element);
                    break;
            }
            PopulatePage();
        }

        private void gpu_group_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "allocation_algorithm")
            {
                PopulatePage();
            }
        }

        private void RegisterGpuGroupHandlers(GPU_group gpu_group)
        {
            gpu_group.PropertyChanged -= gpu_group_PropertyChanged;
            gpu_group.PropertyChanged += gpu_group_PropertyChanged;
        }

        private void UnregisterGpuGroupHandlers(GPU_group gpu_group)
        {
            gpu_group.PropertyChanged -= gpu_group_PropertyChanged;
        }

        private void RegisterHandlers()
        {
            if (xenObject == null)
                return;
            
            xenObject.Connection.Cache.DeregisterCollectionChanged<GPU_group>(GPU_group_CollectionChangedWithInvoke);
            xenObject.Connection.Cache.RegisterCollectionChanged<GPU_group>(GPU_group_CollectionChangedWithInvoke);

            foreach (GPU_group gpu_group in xenObject.Connection.Cache.GPU_groups)
            {
                UnregisterGpuGroupHandlers(gpu_group); 
                RegisterGpuGroupHandlers(gpu_group);
            }
        }

        internal void UnregisterHandlers()
        {
            if (xenObject == null)
                return;

            xenObject.Connection.Cache.DeregisterCollectionChanged<GPU_group>(GPU_group_CollectionChangedWithInvoke);
            
            foreach (GPU_group gpu_group in xenObject.Connection.Cache.GPU_groups)
                UnregisterGpuGroupHandlers(gpu_group);
        }

        private void editPlacementPolicyButton_Click(object sender, System.EventArgs e)
        {
            var pool = Helpers.GetPool(xenObject.Connection);

            using (PropertiesDialog propertiesDialog = new PropertiesDialog(pool ?? xenObject))
            {
                propertiesDialog.SelectPoolGpuEditPage();
                propertiesDialog.ShowDialog(this);
            }
        }

        private void GpuPlacementPolicyPanel_VisibleChanged(object sender, System.EventArgs e)
        {
            if (Visible)
                RegisterHandlers();
            else
                UnregisterHandlers();
        }
    }

    public static class PlacementPolicyWrapper
    {
        public static string ToString(allocation_algorithm x)
        {
            switch (x)
            {
                case allocation_algorithm.breadth_first:
                    return Messages.GPU_PLACEMENT_POLICY_MAX_PERFORMANCE_DESCRIPTION;
                case allocation_algorithm.depth_first:
                    return Messages.GPU_PLACEMENT_POLICY_MAX_DENSITY_DESCRIPTION;
                default:
                    return Messages.GPU_PLACEMENT_POLICY_MIXED_DESCRIPTION;
            }
        }
    }
}
