/* Copyright (c) Cloud Software Group, Inc. 
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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.XCM;


namespace XenAdmin.Wizards.ConversionWizard
{
    public partial class VmSelectionPage : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        private bool _buttonNextEnabled;
        private bool _updating;
        private string _currentXSVersion;
        private bool _supportVersionChecking;
        

        public VmSelectionPage()
        {
            InitializeComponent();
        }

        #region XenTabPage implementation

        public override string Text => Messages.CONVERSION_VM_PAGE_TEXT;

        public override string PageTitle => Messages.CONVERSION_VM_PAGE_TITLE;

        public override string HelpID => "VmSelection";

        public override bool EnableNext()
        {
            return _buttonNextEnabled;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            tableLayoutPanelError.Visible = false;
            _supportVersionChecking = IsSupportGuestVersionChecking();
            if (_supportVersionChecking)
            {
                tableLayoutPanelVersion.Visible = true;
                columnRemarks.Visible = true;
                showOnlySupportedGuestCheckBox.Checked = true;
            }
            else
            {
                tableLayoutPanelVersion.Visible = false;
                columnRemarks.Visible = false;
                showOnlySupportedGuestCheckBox.Checked = false;
            }
            _currentXSVersion = Helpers.HostProductVersion(Helpers.GetCoordinator(Connection));
            if (direction == PageLoadedDirection.Forward)
                Build();
        }

        public override void PageCancelled(ref bool cancel)
        {
            _backgroundWorker.CancelAsync();
        }

        #endregion

        public ConversionClient ConversionClient { private get; set; }
        public VmInstance[] VMwareVMs { private get; set; }
        public ServerInfo VmwareCredInfo { private get; set; }

        public VmInstance[] SelectedVms
        {
            get
            {
                return (from SourceVmRow row in dataGridViewVms.Rows
                    where row.IsChecked && row.Visible
                    select row.Vm).ToArray();
            }
        }

        private bool SelectedVmsExist
        {
            get
            {
                foreach (SourceVmRow row in dataGridViewVms.Rows)
                {
                    if (row.Visible && row.IsChecked)
                        return true;
                }

                return false;
            }
        }

        // The feature of unsupported guest version warning is available on 8.3.1
        private bool IsSupportGuestVersionChecking()
        {
            string currentConversionVersion = ConversionClient.GetVpxVersion();
            return Version.TryParse(currentConversionVersion, out Version result) 
                   && result.CompareTo(new Version("8.3.1")) >= 0;
        }

        private void Build()
        {
            try
            {
                dataGridViewVms.SuspendLayout();
                dataGridViewVms.Rows.Clear();

                if (VMwareVMs == null)
                    return;

                foreach (VmInstance vm in VMwareVMs)
                {
                    if (vm.PowerState != (int)VmPowerState.Off)
                    {
                        continue;
                    }

                    bool supported = !_supportVersionChecking || IsSupportedGuest(vm);
                    SourceVmRow row = new SourceVmRow(vm, supported);
                    dataGridViewVms.Rows.Add(row);
                    row.Visible = !showOnlySupportedGuestCheckBox.Checked || supported;
                }
            }
            finally
            {
                dataGridViewVms.ResumeLayout();
                UpdateButtons();
            }
        }

        private void UpdateButtons()
        {
            _buttonNextEnabled = SelectedVmsExist;
            UpdateSelectAllAndClearAllButton();
            OnPageUpdated();
        }

        private void BulkCheck(bool check)
        {
            try
            {
                _updating = true;
                foreach (SourceVmRow row in dataGridViewVms.Rows)
                {
                    row.IsChecked = check;
                }
            }
            finally
            {
                _updating = false;
                CheckIfExistUnsupportedVersion();
                UpdateButtons();
            }
        }

        private void UpdateSelectAllAndClearAllButton()
        {
            int total = 0;
            int check = 0;
            foreach (SourceVmRow row in dataGridViewVms.Rows)
            {
                if (row.Visible)
                {
                    total++;
                    if (row.IsChecked)
                    {
                        check++;
                    }
                }
            }
            
            buttonSelectAll.Enabled = total != check;
            buttonClearAll.Enabled = check != 0;
        }

        private void CheckIfExistUnsupportedVersion()
        {
            if (!_supportVersionChecking)
            {
                return;
            }
            foreach (SourceVmRow row in dataGridViewVms.Rows)
            {
                if (row.Visible && row.IsChecked && !row.IsSupportedGuest())
                {
                    pictureBoxError.Image = Images.StaticImages._000_Alert2_h32bit_16;
                    labelError.Text = Messages.CONVERSION_UNSUPPORTED_VM_SELECTED_WARNING;
                    tableLayoutPanelError.Visible = true;
                    return;
                }
            }
            tableLayoutPanelError.Visible = false;
        }

        private void UpdateComponentEnabledStatusWhenRefresh(bool enabled)
        {
            buttonSelectAll.Enabled = enabled;
            buttonClearAll.Enabled = enabled;
            buttonRefresh.Enabled = enabled;
            showOnlySupportedGuestCheckBox.Enabled = enabled;
        }

        private bool IsSupportedGuest(VmInstance vm)
        {
            if (vm.SupportedXSVersions == null)
            {
                return true;
            }
            foreach (string supportedVersion in vm.SupportedXSVersions)
            {
                if (_currentXSVersion.StartsWith(supportedVersion))
                {
                    return true;
                }
            }
            return false;
        }

        private void RefreshData()
        {
            UpdateComponentEnabledStatusWhenRefresh(false);
            dataGridViewVms.Rows.Clear();
            VMwareVMs = null;

            pictureBoxError.Image = Images.StaticImages.ajax_loader;
            labelError.Text = Messages.CONVERSION_CONNECTING_VMWARE;
            tableLayoutPanelError.Visible = true;

            _backgroundWorker.RunWorkerAsync();
            UpdateButtons();
        }

        #region Control event handlers

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = ConversionClient.GetSourceVMs(VmwareCredInfo);
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                tableLayoutPanelError.Visible = false;
            }
            if (e.Error != null)
            {
                log.Error(e.Error);
                pictureBoxError.Image = Images.StaticImages._000_error_h32bit_16;
                labelError.Text = Messages.CONVERSION_CONNECTING_VMWARE_FAILURE;
                tableLayoutPanelError.Visible = true;
            }
            else
            {
                VMwareVMs = e.Result as VmInstance[];
                tableLayoutPanelError.Visible = false;
                Build();
            }

            UpdateComponentEnabledStatusWhenRefresh(true);
            UpdateButtons();
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            BulkCheck(true);
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            BulkCheck(false);
        }

        private void dataGridViewVms_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewVms.IsCurrentCellDirty)
                dataGridViewVms.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dataGridViewVms_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_updating)
                return;

            if (e.ColumnIndex != 0 || e.RowIndex < 0 || e.RowIndex >= dataGridViewVms.RowCount)
                return;
            CheckIfExistUnsupportedVersion();
            UpdateButtons();
        }

        private void showOnlySupportedGuestCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool existCheckedUnsupportedGuest = false;
            foreach (SourceVmRow row in dataGridViewVms.Rows)
            {
                if (!row.IsSupportedGuest())
                {
                    row.Visible = !showOnlySupportedGuestCheckBox.Checked;
                    existCheckedUnsupportedGuest = row.IsChecked;
                }
            }
            if (!showOnlySupportedGuestCheckBox.Checked && existCheckedUnsupportedGuest)
            {
                pictureBoxError.Image = Images.StaticImages._000_Alert2_h32bit_16;
                labelError.Text = Messages.CONVERSION_UNSUPPORTED_VM_SELECTED_WARNING;
                tableLayoutPanelError.Visible = true;
            }
            else
            {
                tableLayoutPanelError.Visible = false;
            }

            UpdateButtons();
        }

        private void supportedOSLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_currentXSVersion.StartsWith("8.2"))
            {
                System.Diagnostics.Process.Start(InvisibleMessages.CONVERSION_DOC_PATH_82);
            }
            else
            {
                System.Diagnostics.Process.Start(InvisibleMessages.CONVERSION_DOC_PATH_LATEST);
            }
        }

        #endregion

        private class SourceVmRow : DataGridViewRow
        {
            private readonly DataGridViewCheckBoxCell cellCheck = new DataGridViewCheckBoxCell();
            private readonly DataGridViewTextBoxCell cellVm = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellOs = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellDiskSize = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell cellRemarks = new DataGridViewTextBoxCell();
            private readonly bool _supportedGuest;

            public SourceVmRow(VmInstance vm, bool isSupportedGuest)
            {
                Cells.AddRange(cellCheck, cellVm, cellOs, cellDiskSize, cellRemarks);

                Vm = vm;
                cellCheck.Value = false;
                cellVm.Value = vm.Template ? string.Format(Messages.CONVERSION_TEMPLATE, vm.Name) : vm.Name;
                cellOs.Value = vm.OSType;
                cellDiskSize.Value = Util.DiskSizeString(vm.CommittedStorage + vm.UncommittedStorage);
                _supportedGuest = isSupportedGuest;
                cellRemarks.Value = _supportedGuest ? "" : Messages.CONVERSION_UNSUPPORTED;

            }

            public bool IsChecked
            {
                get => (bool)cellCheck.Value;
                set => cellCheck.Value = value;
            }

            public bool IsSupportedGuest()
            {
                return _supportedGuest;
            }

            public VmInstance Vm { get; }
        }
    }
}
