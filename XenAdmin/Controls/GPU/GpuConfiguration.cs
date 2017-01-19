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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls.DataGridViewEx;
using XenAPI;
using XenAdmin.Dialogs;
using XenAdmin.Actions.GPU;
using System.Text.RegularExpressions;

namespace XenAdmin.Controls.GPU
{
    public partial class GpuConfiguration : XenDialogBase
    {
        private List<PGPU> PGpuList { get; set; }

        internal override string HelpName
        {
            get
            {
                return "GpuConfigurationDialog";
            }
        }

        private GpuConfiguration()
        {
            InitializeComponent();
        }

        public GpuConfiguration(IEnumerable<PGPU> pGpuList)
            : this()
        {
            if (pGpuList == null)
                throw new ArgumentNullException("pGpuList");
            if (pGpuList.Count() == 0)
                throw new ArgumentOutOfRangeException("pGpuList", "pGpuList list should not be empty");
            if (pGpuList.ElementAt(0) == null)
                throw new ArgumentOutOfRangeException("pGpuList[0]", "first element of the pGpuList list should not be null");

            PGpuList = pGpuList.ToList();
            connection = PGpuList[0].Connection;
            PopulateGrid(pGpuList);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            SetWindowTitle();
        }

        private void PopulateGrid(IEnumerable<PGPU> pGpuList)
        {
            var pGpu = PGpuList[0];

            dataGridViewEx1.SuspendLayout();
            dataGridViewEx1.Rows.Clear();
            dataGridViewEx1.Cursor = Cursors.WaitCursor;
            try
            {
                if (pGpu.supported_VGPU_types != null)
                {
                    dataGridViewEx1.Rows.AddRange((from supportedVGpuTypeRef in pGpu.supported_VGPU_types
                                                   let supportedVGpuType = pGpu.Connection.Resolve(supportedVGpuTypeRef)
                                                   let enabled = pGpu.enabled_VGPU_types.Contains(supportedVGpuTypeRef)
                                                   let isInUse = pGpuList.Any(p => p.Connection.ResolveAll(p.resident_VGPUs).Any(v => v.type.opaque_ref == supportedVGpuTypeRef.opaque_ref))
                                                   orderby supportedVGpuType descending 
                                                   
                                                   select new VGpuDetailWithCheckBoxRow(supportedVGpuTypeRef, supportedVGpuType, enabled, isInUse)
                                                   )
                                                   .ToArray());
                }
                SetCheckedValues();
            }
            finally
            {
                dataGridViewEx1.ResumeLayout();
                dataGridViewEx1.Cursor = Cursors.Default;
            }
        }

        private void SetCheckedValues()
        {
            dataGridViewEx1.Rows.Cast<VGpuDetailWithCheckBoxRow>().ToList().ForEach(r => r.Enabled = !r.IsReadOnly);
        }
        
        private void okButton_Click(object sender, System.EventArgs e)
        {
            var updatedEnabledVGpuListByPGpu = new Dictionary<PGPU, List<XenRef<VGPU_type>>>();
            PGpuList.ForEach(pgpu => updatedEnabledVGpuListByPGpu.Add(pgpu, new List<XenRef<VGPU_type>>(pgpu.enabled_VGPU_types)));
            var changes = dataGridViewEx1.Rows.Cast<VGpuDetailWithCheckBoxRow>().ToList().FindAll(r => r.NeedsSave);
            if (changes.Count > 0)
            {
                changes.ForEach(r => updatedEnabledVGpuListByPGpu.Keys.ToList()
                                               .ForEach(pGpu =>
                                                            {
                                                                var vGpuList = updatedEnabledVGpuListByPGpu[pGpu];
                                                                if (vGpuList.Contains(r.VGpuTypeRef) && !r.CheckBoxChecked)
                                                                    vGpuList.Remove(r.VGpuTypeRef);
                                                                else if (r.CheckBoxChecked)
                                                                    vGpuList.Add(r.VGpuTypeRef);
                                                            }));

                new VgpuConfigurationAction(updatedEnabledVGpuListByPGpu, connection).RunAsync();
            }
            Close();
        }

        private void dataGridViewEx1_SelectionChanged(object sender, System.EventArgs e)
        {
            dataGridViewEx1.ClearSelection();
        }

        private void SetWindowTitle()
        {
            var name = PGpuList[0].Name;
            Text = PGpuList.Count == 1
                ? String.Format(Messages.GPU_GROUP_NAME_AND_NO_OF_GPUS_ONE, name)
                : String.Format(Messages.GPU_GROUP_NAME_AND_NO_OF_GPUS, name, PGpuList.Count);

            rubricLabel.Text = PGpuList.Count == 1
                                   ? Messages.GPU_RUBRIC_PLEASE_SELECT_WHICH_GPU_ONE
                                   : Messages.GPU_RUBRIC_PLEASE_SELECT_WHICH_GPU_MULTIPLE;
        }
    }

    class VGpuDetailWithCheckBoxRow : DataGridViewExRow
    {
        private readonly bool allowed;
        private readonly DataGridViewCheckBoxCell checkBoxCell = new DataGridViewCheckBoxCell(false);
        private readonly DataGridViewTextBoxCell nameColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell vGpusPerGpuColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell maxResolutionColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell maxDisplaysColumn = new DataGridViewTextBoxCell();
        private readonly DataGridViewTextBoxCell videoRamColumn = new DataGridViewTextBoxCell();

        public VGPU_type VGpuType { get; private set; }
        public XenRef<VGPU_type> VGpuTypeRef { get; private set; }
        public bool IsReadOnly { get; private set; }

        public bool NeedsSave
        {
            get { return (allowed != CheckBoxChecked); }
        }

        public bool CheckBoxChecked
        {
            get { return (bool)checkBoxCell.Value; }
        }

        public VGpuDetailWithCheckBoxRow(XenRef<VGPU_type> vGpuTypeRef, VGPU_type vGpuType, bool allowed, bool isInUse)
        {
            VGpuTypeRef = vGpuTypeRef;
            VGpuType = vGpuType;
            this.allowed = allowed;
            IsReadOnly = isInUse && allowed;

            SetCells();
            Cells.AddRange(checkBoxCell, nameColumn, vGpusPerGpuColumn, maxResolutionColumn, maxDisplaysColumn, videoRamColumn);
            SetupCheckBoxCell();
        }

        private void SetCells()
        {
            bool isPassThru = VGpuType.IsPassthrough;

            nameColumn.Value = isPassThru ? Messages.VGPU_PASSTHRU_TOSTRING : VGpuType.model_name;

            if (!isPassThru)
                vGpusPerGpuColumn.Value = VGpuType.Capacity;
            else
                vGpusPerGpuColumn.Value = string.Empty;

            if (!isPassThru)
                maxResolutionColumn.Value = VGpuType.MaxResolution;

            if (!isPassThru)
                maxDisplaysColumn.Value = VGpuType.max_heads;
            else
                maxDisplaysColumn.Value = string.Empty;

            videoRamColumn.Value = VGpuType.framebuffer_size != 0 ? Util.MemorySizeStringSuitableUnits(VGpuType.framebuffer_size, true) : string.Empty;
        }

        private void SetupCheckBoxCell()
        {
            checkBoxCell.TrueValue = true;
            checkBoxCell.FalseValue = false;
            checkBoxCell.ValueType = typeof(bool);
            checkBoxCell.Value = allowed;
            checkBoxCell.ReadOnly = IsReadOnly;
        }
    }
}
