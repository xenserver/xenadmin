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
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.WarningDialogs;
using XenAdmin.Network;
using XenAPI;
using System.Drawing;


namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class LVMoHBA : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
               
        private List<FibreChannelDevice> _selectedDevices = new List<FibreChannelDevice>();

        public LVMoHBA()
        {
            InitializeComponent();
        }

        public virtual SR.SRTypes SrType { get { return SR.SRTypes.lvmohba; } }

        public virtual bool ShowNicColumn { get { return false; } }

        public virtual LvmOhbaSrDescriptor CreateSrDescriptor(FibreChannelDevice device)
        {
            return new LvmOhbaSrDescriptor(device, Connection);
        }
        
        #region XenTabPage overrides

        public override string PageTitle { get { return Messages.NEWSR_SELECT_LUN; } }

        public override string Text { get { return Messages.NEWSR_LOCATION; } }

        public override string HelpID { get { return "Location_HBA"; } }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Back)
                return;

            Host master = Helpers.GetMaster(Connection);
            if (master == null)
            {
                cancel = true;
                return;
            }
            
            SrDescriptors = new List<LvmOhbaSrDescriptor>();

            var existingSrDescriptors = new List<LvmOhbaSrDescriptor>();
            var formatDiskDescriptors = new List<LvmOhbaSrDescriptor>();

            foreach (var device in _selectedDevices)
            {
                LvmOhbaSrDescriptor descr = CreateSrDescriptor(device);

                var action = new SrProbeAction(Connection, master, SrType, descr.DeviceConfig);
                using (var dlg = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                    dlg.ShowDialog(this);

                if (!action.Succeeded)
                {
                    cancel = true;
                    return;
                }

                descr.UUID = SrWizardHelpers.ExtractUUID(action.Result);

                if (!string.IsNullOrEmpty(SrWizardType.UUID))
                {
                    // Check LUN contains correct SR
                    if (descr.UUID == SrWizardType.UUID)
                    {
                        SrDescriptors.Add(descr);
                        continue;
                    }

                    using (var dlog = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Error,
                            String.Format(Messages.INCORRECT_LUN_FOR_SR, SrWizardType.SrName), Messages.XENCENTER)))
                    {
                        dlog.ShowDialog(this);
                    }

                    cancel = true;
                    return;
                }
                
                if (string.IsNullOrEmpty(descr.UUID))
                {
                    // No existing SRs were found on this LUN. If allowed to create
                    // a new SR, ask the user if they want to proceed and format.
                    if (!SrWizardType.AllowToCreateNewSr)
                    {
                        using (var dlog = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Error,
                                Messages.NEWSR_LUN_HAS_NO_SRS, Messages.XENCENTER)))
                        {
                            dlog.ShowDialog(this);
                        }

                        cancel = true;
                        return;
                    }

                    if (!Program.RunInAutomatedTestMode)
                        formatDiskDescriptors.Add(descr);
                }
                else
                {
                    // CA-17230: Check this isn't a detached SR. If it is then just continue
                    SR sr = SrWizardHelpers.SrInUse(descr.UUID);
                    if (sr != null)
                    {
                        SrDescriptors.Add(descr);
                        continue;
                    }

                    // We found a SR on this LUN. Will ask user for choice later.
                    existingSrDescriptors.Add(descr);
                }
            }

            if (!cancel && existingSrDescriptors.Count > 0)
            {
                var launcher = new LVMoHBAWarningDialogLauncher(this, existingSrDescriptors, true);
                launcher.ShowWarnings();
                cancel = launcher.Cancelled;
                if (!cancel && launcher.SrDescriptors.Count > 0)
                    SrDescriptors.AddRange(launcher.SrDescriptors);
            }

            if (!cancel && formatDiskDescriptors.Count > 0)
            {
                var launcher = new LVMoHBAWarningDialogLauncher(this, formatDiskDescriptors, false);
                launcher.ShowWarnings();
                cancel = launcher.Cancelled;
                if (!cancel && launcher.SrDescriptors.Count > 0)
                    SrDescriptors.AddRange(launcher.SrDescriptors);
            }
            
            base.PageLeave(direction, ref cancel);
        }

        public override bool EnableNext()
        {
            UpdateSelectionButtons();
            return _selectedDevices.Count > 0;
        }

        public override bool EnablePrevious()
        {
            if (SrWizardType.DisasterRecoveryTask && SrWizardType.SrToReattach == null)
                return false;

            return true;
        }

        public override void PopulatePage()
        {
            colNic.Visible = ShowNicColumn; 
            dataGridView.Rows.Clear();

            var vendorGroups = from device in FCDevices
                         group device by device.Vendor into g
                         orderby g.Key
                         select new { VendorName = g.Key, Devices = g.OrderBy(x => x.Serial) };

            foreach (var vGroup in vendorGroups)
            {
                var vendorRow = new VendorRow(vGroup.VendorName);
                dataGridView.Rows.Add(vendorRow);

                using (var font = new Font(dataGridView.DefaultCellStyle.Font, FontStyle.Bold))
                    vendorRow.DefaultCellStyle = new DataGridViewCellStyle(dataGridView.DefaultCellStyle)
                        {
                            Font = font,
                            SelectionBackColor = dataGridView.DefaultCellStyle.BackColor,
                            SelectionForeColor = dataGridView.DefaultCellStyle.ForeColor
                        };

                var deviceRows = from device in vGroup.Devices select new FCDeviceRow(device, ShowNicColumn);
                dataGridView.Rows.AddRange(deviceRows.ToArray());
            }
        }

        public override string NextText(bool isLastPage)
        {
            // for Dundee or greater connections, we have "Storage provisioning settings" page after this page, so the Next button should say "Next", not "Create"
            return Helpers.DundeeOrGreater(Connection) ?  Messages.WIZARD_BUTTON_NEXT : Messages.NEWSR_LVMOHBA_NEXT_TEXT;
        }

        public override void SelectDefaultControl()
        {
            dataGridView.Select();
        }

        #endregion

        #region Event handlers

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_srWizardType.SrToReattach != null)
                return;

            if (e.ColumnIndex != colCheck.Index || e.RowIndex < 0 || e.RowIndex > dataGridView.RowCount - 1)
                return;

            var deviceRow = dataGridView.Rows[e.RowIndex] as FCDeviceRow;
            if (deviceRow == null)
                return;

            deviceRow.Cells[colCheck.Index].Value = !(bool)deviceRow.Cells[colCheck.Index].Value;

            UpdateSelectedDevices();
            OnPageUpdated();
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (_srWizardType.SrToReattach == null)
                return;

            UpdateSelectedDevices();
            OnPageUpdated();
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            foreach (var row in dataGridView.Rows)
            {
                var deviceRow = row as FCDeviceRow;
                if (deviceRow != null && deviceRow.Cells.Count > 0)
                    deviceRow.Cells[colCheck.Index].Value = true;
            }

            UpdateSelectedDevices();
            OnPageUpdated();
        }

        private void buttonClearAll_Click(object sender, EventArgs e)
        {
            foreach (var row in dataGridView.Rows)
            {
                var deviceRow = row as FCDeviceRow;
                if (deviceRow != null && deviceRow.Cells.Count > 0)
                    deviceRow.Cells[colCheck.Index].Value = false;
            }

            UpdateSelectedDevices();
            OnPageUpdated();
        }

        #endregion

        private void UpdateSelectedDevices()
        {
            if (SrWizardType.SrToReattach == null)
            {
                //when creating a new SR the checkbox column is visible
                _selectedDevices = (from DataGridViewRow row in dataGridView.Rows
                                    let deviceRow = row as FCDeviceRow
                                    where deviceRow != null
                                          && deviceRow.Cells.Count > 0
                                          && (bool)(deviceRow.Cells[colCheck.Index].Value)
                                    select deviceRow.Device).ToList();
            }
            else
            {
                //when reattaching SR the checkbox column is hidden
                _selectedDevices = (from DataGridViewRow row in dataGridView.Rows
                                    let deviceRow = row as FCDeviceRow
                                    where deviceRow != null && deviceRow.Selected
                                    select deviceRow.Device).ToList();
            }
        }

        private void UpdateSelectionButtons()
        {
            if (buttonSelectAll.Visible)
                buttonSelectAll.Enabled = _selectedDevices.Count < FCDevices.Count;

            if (buttonClearAll.Visible)
                buttonClearAll.Enabled = _selectedDevices.Count > 0;
        }

        public bool FiberChannelScan(IWin32Window owner, IXenConnection connection, out List<FibreChannelDevice> devices)
        {
            devices = new List<FibreChannelDevice>();

            Host master = Helpers.GetMaster(connection);
            if (master == null)
                return false;

            FibreChannelProbeAction action = new FibreChannelProbeAction(master, SrType);
            using (var  dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                dialog.ShowDialog(owner); //Will block until dialog closes, action completed

            if (!action.Succeeded)
                return false;

            try
            {
                devices = FibreChannelProbeParsing.ProcessXML(action.Result);

                if (devices.Count == 0)
                {
                    using (var dlg = new ThreeButtonDialog(
                        new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.FIBRECHANNEL_NO_RESULTS, Messages.XENCENTER)))
                    {
                        dlg.ShowDialog();
                    }

                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                log.Debug("Exception parsing result of fibre channel scan", e);
                log.Debug(e, e);
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.FIBRECHANNEL_XML_ERROR, Messages.XENCENTER)))
                {
                    dlg.ShowDialog();
                }

                return false;
            }
        }
        #region Accessors

        public List<FibreChannelDevice> FCDevices { private get; set; }

        private SrWizardType _srWizardType;
        public SrWizardType SrWizardType
        {
            private get
            {
                return _srWizardType;
            }
            set
            {
                _srWizardType = value;

                bool creatingNew = _srWizardType.SrToReattach == null;
                
                colCheck.Visible = creatingNew;
                dataGridView.MultiSelect = creatingNew;
                buttonSelectAll.Visible = creatingNew;
                buttonClearAll.Visible = creatingNew;
                labelCreate.Visible = creatingNew;
                labelReattach.Visible = !creatingNew;
            }
        }

        public List<LvmOhbaSrDescriptor> SrDescriptors { get; private set; }

        /// <summary>
        /// min size
        /// </summary>
        public long SRSize
        {
            get
            {
                long size = long.MaxValue;
                foreach (var srDescriptor in SrDescriptors)
                {
                    if (srDescriptor.Device.Size < size)
                        size = srDescriptor.Device.Size;
                }
                return size;
            }
        }

        #endregion
        
        #region Nested classes

        private class FCDeviceRow : DataGridViewRow
        {
            public FibreChannelDevice Device { get; private set; }

            public FCDeviceRow(FibreChannelDevice device, bool showNicColumn)
            {
                Device = device;

                string id = string.IsNullOrEmpty(device.SCSIid) ? device.Path : device.SCSIid;
                string details = String.Format("{0}:{1}:{2}:{3}", device.adapter, device.channel, device.id, device.lun);

                Cells.AddRange(new DataGridViewCheckBoxCell{ThreeState = false, Value = false},
                    new DataGridViewTextBoxCell { Value = Util.DiskSizeString(device.Size) },
                    new DataGridViewTextBoxCell { Value = device.Serial },
                    new DataGridViewTextBoxCell { Value = id },
                    new DataGridViewTextBoxCell { Value = details });

                if (showNicColumn)
                    Cells.Add(new DataGridViewTextBoxCell {Value = device.eth});
            }
        }

        private class VendorRow : DataGridViewRow
        {
            public VendorRow(string vendor)
            {
                Cells.AddRange(new DataGridViewCheckBoxCellVendor(),
                    new DataGridViewTextBoxCell { Value = vendor },
                    new DataGridViewTextBoxCell(),
                    new DataGridViewTextBoxCell(),
                    new DataGridViewTextBoxCell());
            }

            private class DataGridViewCheckBoxCellVendor : DataGridViewCheckBoxCell
            {
                protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
                {
                    using (var normalBrush = new SolidBrush(OwningRow.DefaultCellStyle.BackColor))
                    using (var selectedBrush = new SolidBrush(OwningRow.DefaultCellStyle.SelectionBackColor))
                    {
                        graphics.FillRectangle(
                            (elementState & DataGridViewElementStates.Selected) != 0 ? selectedBrush : normalBrush,
                            cellBounds.X, cellBounds.Y, cellBounds.Width, cellBounds.Height);
                    }
                }
            }
        }

        public enum UserSelectedOption { Cancel, Reattach, Format, Skip }

        private class LVMoHBAWarningDialogLauncher
        {
            private readonly List<LvmOhbaSrDescriptor> inputSrDescriptors;
            private readonly bool foundExistingSRs;
            private readonly IWin32Window owner;
            
            public List<LvmOhbaSrDescriptor> SrDescriptors { get; private set; }
            public bool Cancelled { get; private set; }

            public LVMoHBAWarningDialogLauncher(IWin32Window owner, List<LvmOhbaSrDescriptor> srDescriptors,
                bool foundExistingSRs)
            {
                this.owner = owner;
                this.foundExistingSRs = foundExistingSRs;
                inputSrDescriptors = srDescriptors;
                SrDescriptors = new List<LvmOhbaSrDescriptor>();
            }

            private UserSelectedOption GetSelectedOption(LvmOhbaSrDescriptor lvmOhbaSrDescriptor,
                out bool repeatForRemainingLUNs)
            {
                int remainingCount = inputSrDescriptors.Count - 1 - inputSrDescriptors.IndexOf(lvmOhbaSrDescriptor);

                using (var dialog = new LVMoHBAWarningDialog(lvmOhbaSrDescriptor.Device, remainingCount, foundExistingSRs))
                {
                    dialog.ShowDialog(owner);
                    repeatForRemainingLUNs = dialog.RepeatForRemainingLUNs;
                    return dialog.SelectedOption;
                }
            }

            public void ShowWarnings()
            {
                bool repeatForRemainingLUNs = false;
                UserSelectedOption selectedOption = UserSelectedOption.Cancel;

                foreach (LvmOhbaSrDescriptor lvmOhbaSrDescriptor in inputSrDescriptors)
                {
                    if (!repeatForRemainingLUNs)
                    {
                        selectedOption = GetSelectedOption(lvmOhbaSrDescriptor, out repeatForRemainingLUNs);
                    }

                    switch (selectedOption)
                    {
                        case UserSelectedOption.Format:
                            lvmOhbaSrDescriptor.UUID = null;
                            SrDescriptors.Add(lvmOhbaSrDescriptor);
                            break;
                        case UserSelectedOption.Reattach:
                            SrDescriptors.Add(lvmOhbaSrDescriptor);
                            break;
                        case UserSelectedOption.Cancel:
                            SrDescriptors.Clear();
                            Cancelled = true;
                            return;
                    }
                }
            }
        }

        #endregion
    }
}
