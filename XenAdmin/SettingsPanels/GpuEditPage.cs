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
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using XenAdmin;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public partial class GpuEditPage : UserControl, IEditPage
    {
        VM vm;
        GPU_group current_gpu_group;
        GPU_group[] gpu_groups;
        bool gpusAvailable, vmStopped;

        public GpuEditPage()
        {
            InitializeComponent();
            Text = Messages.GPU;
        }

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            GPU_group selectedGroup = CurrentSelection as GPU_group;
            return new GpuAssignAction(vm, selectedGroup);
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            Trace.Assert(clone is VM);  // only VMs should show this page
            Trace.Assert(Helpers.BostonOrGreater(clone.Connection));  // If not Boston or greater, we shouldn't see this page
            Trace.Assert(!Helpers.FeatureForbidden(clone, Host.RestrictGpu));  // If license insufficient, we show upsell page instead

            vm = (VM)clone;
            if (vm.VGPUs.Count == 0)
                current_gpu_group = null;
            else
            {
                VGPU vgpu = vm.Connection.Resolve(vm.VGPUs[0]);
                current_gpu_group = (vgpu == null ? null : vgpu.Connection.Resolve(vgpu.GPU_group));
            }

            gpu_groups = clone.Connection.Cache.GPU_groups;
            Array.Sort(gpu_groups);
            gpusAvailable = (gpu_groups.Length > 0);

            vmStopped = (vm.power_state == vm_power_state.Halted);

            Populate();
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
                GPU_group selectedGroup = CurrentSelection as GPU_group;
                return (selectedGroup != current_gpu_group);
            }
        }

        #region VerticalTab Members

        public string SubText
        {
            get
            {
                return (CurrentSelection == null ? Messages.GPU_NONE : CurrentSelection.ToString());
            }
        }

        public Image Image
        {
            get { return Resources._000_GetMemoryInfo_h32bit_16; }
        }

        #endregion

        #endregion

        private void Populate()
        {
            if (!gpusAvailable)
            {
                labelGpuType.Visible = comboBoxGpus.Visible = false;
                labelRubric.Text = (Helpers.GetPool(vm.Connection) == null ? Messages.GPU_RUBRIC_NO_GPUS_SERVER : Messages.GPU_RUBRIC_NO_GPUS_POOL);
            }

            else
            {
                comboBoxGpus.Items.Add(Messages.GPU_NONE);
                foreach (GPU_group gpu_group in gpu_groups)
                    comboBoxGpus.Items.Add(gpu_group);

                if (current_gpu_group == null)
                    comboBoxGpus.SelectedItem = Messages.GPU_NONE;
                else
                    comboBoxGpus.SelectedItem = current_gpu_group;
            }

            ShowHideWarnings();
        }

        private void ShowHideWarnings()
        {
            if (!gpusAvailable)
            {
                imgTooManyVMs.Visible =
                labelTooManyVMs.Visible =
                imgRDP.Visible =
                labelRDP.Visible =
                imgNeedGpu.Visible =
                labelNeedGpu.Visible =
                imgStopVM.Visible =
                labelStopVM.Visible =
                labelNeedDriver.Visible = 
                imgNeedDriver.Visible =
                    false;
                return;
            }

            if (!vmStopped)
            {
                imgTooManyVMs.Visible =
                labelTooManyVMs.Visible =
                imgRDP.Visible =
                labelRDP.Visible =
                imgNeedGpu.Visible =
                labelNeedGpu.Visible =
                labelNeedDriver.Visible =
                imgNeedDriver.Visible =
                    false;

                imgStopVM.Visible =
                labelStopVM.Visible =
                    true;

                labelGpuType.Enabled =
                comboBoxGpus.Enabled =
                    false;

                return;
            }

            bool hasChanged = HasChanged;

            imgStopVM.Visible = labelStopVM.Visible = false;
            imgRDP.Visible = labelRDP.Visible = (hasChanged && current_gpu_group == null);  // changed from None to Some

            GPU_group selectedGroup = CurrentSelection as GPU_group;

            if (selectedGroup == null)
            {
                imgTooManyVMs.Visible =
                labelTooManyVMs.Visible =
                imgNeedGpu.Visible =
                labelNeedGpu.Visible =
                labelNeedDriver.Visible = 
                imgNeedDriver.Visible =
                    false;
                return;
            }

            // The number of GPUs owned by this GPU group
            int nPGPUs = selectedGroup.PGPUs.Count;

            // The number of real VMs (not templates) already assigned to this GPU group
            int nVGPUs = selectedGroup.VGPUs.FindAll(
                delegate(XenRef<VGPU> vgpuref)
                {
                    VGPU vgpu = vm.Connection.Resolve(vgpuref);
                    VM vm2 = (vgpu == null ? null : vgpu.Connection.Resolve(vgpu.VM));
                    return (vm2 != null && vm2.is_a_real_vm);
                }
            ).Count;

            // If the VM being edited isn't already assigned to this GPU group,
            // it will become one extra: but otherwise we've already counted it.
            if (hasChanged)
                nVGPUs++;

            imgTooManyVMs.Visible = labelTooManyVMs.Visible = (nVGPUs > nPGPUs);
            imgNeedGpu.Visible = labelNeedGpu.Visible = 
            labelNeedDriver.Visible = imgNeedDriver.Visible = !labelTooManyVMs.Visible;  // show the "Need GPU" message iff the "too many VMs" message is not shown
        }

        private object CurrentSelection
        {
            get
            {
                return comboBoxGpus.SelectedItem;
            }
        }

        private void comboBoxGpus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowHideWarnings();
        }

        private void warningsTable_SizeChanged(object sender, EventArgs e)
        {
            int[] columnsWidth = warningsTable.GetColumnWidths();
            int textColumnWidth = columnsWidth.Length>1?columnsWidth[1]:1;
            labelTooManyVMs.MaximumSize =
            labelRDP.MaximumSize =
            labelStopVM.MaximumSize =
                new Size(textColumnWidth, 999);
        }
    }
}
