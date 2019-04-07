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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;


namespace XenAdmin.SettingsPanels
{
    public partial class GpuEditPage : XenTabPage, IEditPage
    {
        public VM vm;
        private List<GpuTuple> currentGpuTuples = new List<GpuTuple>();
        private GPU_group[] gpu_groups;
        private bool gpusAvailable;

        public GpuEditPage()
        {
            InitializeComponent();
            imgRDP.Visible = labelRDP.Visible =
                imgNeedDriver.Visible = labelNeedDriver.Visible =
                imgNeedGpu.Visible = labelNeedGpu.Visible =
                imgStopVM.Visible = labelStopVM.Visible =
                imgHA.Visible = labelHA.Visible =
                    false;
        }

        public GPU_group GpuGroup 
        {
            get
            {
                var tuples = gpuGrid.Rows.Cast<object>().Select(row => row as VGpuDetailRow).Select(vGpuRow => vGpuRow?.GpuTuple).Where(tuple => tuple != null).ToList();
                GpuTuple firstTuple = tuples.FirstOrDefault();
                return firstTuple == null ? null : firstTuple.GpuGroup; // !!! temporary solution
            }
        }

        public VGPU_type VgpuType
        {
            get
            {
                var tuples = gpuGrid.Rows.Cast<object>().Select(row => row as VGpuDetailRow).Select(vGpuRow => vGpuRow?.GpuTuple).Where(tuple => tuple != null).ToList();
                GpuTuple firstTuple = tuples.FirstOrDefault();
                if (firstTuple == null || firstTuple.VgpuTypes == null || firstTuple.VgpuTypes.Length == 0)
                    return null;

                return firstTuple.VgpuTypes[0]; // !!! temporary solution
            }
        }

        public VM.HA_Restart_Priority SelectedPriority { private get; set; }

        #region IEditPage Members

        public AsyncAction SaveSettings()
        {
            return new GpuAssignAction(vm, GpuGroup, VgpuType);
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            Trace.Assert(clone is VM);  // only VMs should show this page
            Trace.Assert(!Helpers.FeatureForbidden(clone, Host.RestrictGpu));  // If license insufficient, we show upsell page instead

            vm = (VM)clone;
            SelectedPriority = vm.HARestartPriority();

            if (Connection == null) // on the PropertiesDialog
                Connection = vm.Connection;

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
                var tuples = gpuGrid.Rows.Cast<object>().Select(row => row as VGpuDetailRow).Select(vGpuRow => vGpuRow?.GpuTuple).Where(tuple => tuple != null).ToList();
                return !tuples.SequenceEqual(currentGpuTuples);
            }
        }

        #region IVerticalTab Members

        public string SubText
        {
            get
            {
                string txt = Messages.UNAVAILABLE;

                if (gpusAvailable)
                {
                    var tuples = gpuGrid.Rows.Cast<object>().Select(row => row as VGpuDetailRow).Select(vGpuRow => vGpuRow?.GpuTuple).Where(tuple => tuple != null).ToList();
                    if (tuples.Count > 0)
                        txt = string.Join(",", tuples.Select(t => t.ToString()));
                }

                return txt;
            }
        }

        public Image Image
        {
            get { return Resources._000_GetMemoryInfo_h32bit_16; }
        }

        #endregion

        #endregion

        #region XenTabPage overrides

        public override string Text
        {
            get { return Messages.GPU; }
        }

        public override string PageTitle
        {
            get { return Messages.NEWVMWIZARD_VGPUPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "GPU"; }
        }

        public override List<KeyValuePair<string, string>> PageSummary
        {
            get
            {
                var summ = new List<KeyValuePair<string, string>>();

                if (gpuGrid.Rows.Count > 0)
                    summ.Add(new KeyValuePair<string, string>(Messages.GPU, SubText));

                return summ;
            }
        }
        
        public override void PopulatePage()
        {
            currentGpuTuples.Clear();

            gpu_groups = Connection.Cache.GPU_groups.Where(g => g.PGPUs.Count > 0 && g.supported_VGPU_types.Count != 0).ToArray();
            //not showing empty groups

            gpusAvailable = gpu_groups.Length > 0;

            if (gpusAvailable)
            {
                PopulateGrid();
                ShowHideWarnings();
            }
            else
            {
                labelRubric.Text = Helpers.GetPool(Connection) == null
                                       ? Messages.GPU_RUBRIC_NO_GPUS_SERVER
                                       : Messages.GPU_RUBRIC_NO_GPUS_POOL;

                tableLayoutPanel1.Visible = false;
                warningsTable.Visible = false;
            }
        }

        public override void SelectDefaultControl()
        {
            if (addButton.CanSelect)
                addButton.Select();
        }

        #endregion

        private void PopulateGrid()
        {
            gpuGrid.SuspendLayout();
            gpuGrid.Rows.Clear();
            gpuGrid.Cursor = Cursors.WaitCursor;
            try
            {
                foreach(var vGpuRef in vm.VGPUs)
                {
                    VGPU vgpu = Connection.Resolve(vGpuRef);
                    GpuTuple gpuTuple = null;
                    if (vgpu != null)
                    {
                        var vgpuGroup = Connection.Resolve(vgpu.GPU_group);

                        if (Helpers.FeatureForbidden(Connection, Host.RestrictVgpu) || !vm.CanHaveVGpu())
                        {
                            if (vgpuGroup.HasPassthrough())
                                gpuTuple = new GpuTuple(vgpuGroup, null, null); //GPU pass-through item
                        }
                        else
                        {
                            VGPU_type vgpuType = Connection.Resolve(vgpu.type);
                            gpuTuple = new GpuTuple(vgpuGroup, vgpuType, null);
                        }
                    }

                    if (gpuTuple != null)
                    {
                        currentGpuTuples.Add(gpuTuple);
                        gpuGrid.Rows.Add(new VGpuDetailRow(gpuTuple));
                    }
                }
            }
            finally
            {
                gpuGrid.ResumeLayout();
                gpuGrid.Cursor = Cursors.Default;
            }
        }

        public void ShowHideWarnings()
        {
            if (!gpusAvailable)
                return;

            imgExperimental.Visible = labelExperimental.Visible =
                VgpuType != null && VgpuType.experimental;

            if (vm.power_state != vm_power_state.Halted)
            {
                imgRDP.Visible = labelRDP.Visible =
                imgNeedDriver.Visible = labelNeedDriver.Visible =
                imgNeedGpu.Visible = labelNeedGpu.Visible =
                imgHA.Visible = labelHA.Visible =
                    false;

                imgStopVM.Visible = labelStopVM.Visible = true;

                addButton.Enabled = deleteButton.Enabled = false;
                return;
            }

            Pool pool = Helpers.GetPool(Connection);
            if (pool != null && pool.ha_enabled && VM.HaPriorityIsRestart(Connection, SelectedPriority))
            {
                imgRDP.Visible = labelRDP.Visible =
                imgNeedDriver.Visible = labelNeedDriver.Visible =
                imgNeedGpu.Visible = labelNeedGpu.Visible =
                imgStopVM.Visible = labelStopVM.Visible =
                    false;

                imgHA.Visible = labelHA.Visible = true;

                addButton.Enabled = deleteButton.Enabled = false;
                return;
            }

            addButton.Enabled = deleteButton.Enabled = true;
            var tuples = gpuGrid.Rows.Cast<object>().Select(row => row as VGpuDetailRow).Select(vGpuRow => vGpuRow?.GpuTuple).Where(tuple => tuple != null).ToList();

            imgStopVM.Visible = labelStopVM.Visible =
            imgHA.Visible = labelHA.Visible = false;

            imgRDP.Visible = labelRDP.Visible =
                HasChanged && currentGpuTuples.Any(tuple => tuple.GpuGroup == null) &&
                tuples.Count > 0 && tuples.Any(tuple => !tuple.IsFractionalVgpu);

            imgNeedGpu.Visible = labelNeedGpu.Visible =
            labelNeedDriver.Visible = imgNeedDriver.Visible =
                tuples.Count > 0 && tuples.Any(tuple => tuple.GpuGroup != null);
        }

        private void warningsTable_SizeChanged(object sender, EventArgs e)
        {
            int[] columnsWidth = warningsTable.GetColumnWidths();
            int textColumnWidth = columnsWidth.Length > 1 ? columnsWidth[1] : 1;

            labelRDP.MaximumSize = labelStopVM.MaximumSize = new Size(textColumnWidth, 999);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            // to do: add code for adding vGPU
            warningsTable.SuspendLayout();
            ShowHideWarnings();
            warningsTable.ResumeLayout();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            // to do: add code for deleting vGPU
            warningsTable.SuspendLayout();
            ShowHideWarnings();
            warningsTable.ResumeLayout();
        }
    }

    public class VGpuDetailRow : DataGridViewExRow
    {
        private readonly DataGridViewTextBoxCell nameColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell vGpusPerGpuColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell maxResolutionColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell maxDisplaysColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell videoRamColumn = new DataGridViewTextBoxCell();

        public GpuTuple GpuTuple { get; }

        public VGpuDetailRow(GpuTuple gpuTuple)
        {
            GpuTuple = gpuTuple;

            SetCells();
            Cells.AddRange(nameColumn, vGpusPerGpuColumn, maxResolutionColumn, maxDisplaysColumn, videoRamColumn);
        }

        private void SetCells()
        {
            var vGpuType = GpuTuple.VgpuTypes[0];

            bool isPassThru = vGpuType.IsPassthrough();

            nameColumn.Value = isPassThru ? Messages.VGPU_PASSTHRU_TOSTRING : vGpuType.model_name;

            if (!isPassThru)
                vGpusPerGpuColumn.Value = vGpuType.Capacity();
            else
                vGpusPerGpuColumn.Value = string.Empty;

            if (!isPassThru)
            {
                var maxRes = vGpuType.MaxResolution();
                maxResolutionColumn.Value = maxRes == "0x0" || String.IsNullOrEmpty(maxRes) ? "" : maxRes;
            }

            if (!isPassThru)
                maxDisplaysColumn.Value = vGpuType.max_heads < 1 ? "" : String.Format("{0}", vGpuType.max_heads);
            else
                maxDisplaysColumn.Value = string.Empty;

            videoRamColumn.Value = vGpuType.framebuffer_size != 0 ? Util.MemorySizeStringSuitableUnits(vGpuType.framebuffer_size, true) : string.Empty;
        }
    }
}
