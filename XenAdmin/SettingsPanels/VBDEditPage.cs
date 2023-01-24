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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Core;
using XenCenterLib;
using XenAPI;
using XenAdmin.Dialogs;
using System.Linq;


namespace XenAdmin.SettingsPanels
{
    public partial class VBDEditPage : UserControl, IEditPage
    {
        private VBD vbd;
        private VDI vdi;
        private SR sr;
        private VM vm;
        private const int MAXIMUM_PRIORITY = 7;
        private const int MINIMUM_PRIORITY = 0;

        private bool _ValidToSave = true;
        private readonly ToolTip InvalidParamToolTip;

        public VBDEditPage()
        {
            InitializeComponent();

            Text = String.Empty;

            modeComboBox.Items.Add(Messages.READ_WRITE);
            modeComboBox.Items.Add(Messages.READ_ONLY);
            diskAccessPriorityTrackBar.Max = MAXIMUM_PRIORITY;
            diskAccessPriorityTrackBar.Min = MINIMUM_PRIORITY;

            InvalidParamToolTip = new ToolTip();
            InvalidParamToolTip.IsBalloon = true;
            InvalidParamToolTip.ToolTipIcon = ToolTipIcon.Warning;
            InvalidParamToolTip.ToolTipTitle = Messages.INVALID_PARAMETER;
        }

        public Image Image => Images.StaticImages._000_VM_h32bit_16;

        public bool ValidToSave
        {
            get { return _ValidToSave; }
        }

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            if (!(clone is VBD))
                return;

            vbd = clone as VBD;
            if (vbd == null)
                return;

            vdi = vbd.Connection.Resolve(vbd.VDI);
            if (vdi == null)
                return;

            sr = vdi.Connection.Resolve(vdi.SR);
            if (sr == null)
                return;

            Repopulate();

            vm = vbd.Connection.Resolve(vbd.VM);
            if (vm == null)
                return;

            Text = Helpers.GetName(vm);

            devicePositionComboBox.Enabled = false;
        }

        // Work out what values to put in the device position combobox
        public void UpdateDevicePositions(Session session)
        {
            Program.AssertOffEventThread();

            List<String> devices = new List<String>();

            // Make the list of possible userdevice positions: the union of the spare get_allowed_VBD_devices...
            devices.AddRange(VM.get_allowed_VBD_devices(session, vm.opaque_ref));

            // Never allow the DVD drive device number to be used, unless it's already in use (it will be added below)
            devices.Remove("3");

            // ...with the userdevice values already in use
            foreach (VBD vbd in vm.Connection.ResolveAll(vm.VBDs))
            {
                if (devices.Contains(vbd.userdevice))
                    continue;

                devices.Add(vbd.userdevice);
            }

            // Put the userdevices values into the combobox
            devices.Sort(StringUtility.NaturalCompare);

            Program.Invoke(Program.MainWindow, delegate()
            {
                devicePositionComboBox.BeginUpdate();

                try
                {
                    devicePositionComboBox.Items.Clear();

                    var comboBoxItems = vm.Connection.ResolveAll(vm.VBDs).Select(d => new DevicePositionComboBoxItem(d.userdevice, d));

                    foreach (var devicePosition in devices)
                    {
                        var comboBoxItemWithVdi = comboBoxItems.FirstOrDefault(ci => ci.VBD.userdevice == devicePosition);
                        if (comboBoxItemWithVdi != null)
                        {
                            devicePositionComboBox.Items.Add(comboBoxItemWithVdi);

                            if (vbd != null && comboBoxItemWithVdi.VBD.userdevice == vbd.userdevice)
                                devicePositionComboBox.SelectedItem = comboBoxItemWithVdi;
                        }
                        else
                        {
                            devicePositionComboBox.Items.Add(new DevicePositionComboBoxItem(devicePosition));
                        }
                    }
                }
                finally
                {
                    devicePositionComboBox.EndUpdate();
                    devicePositionComboBox.Enabled = true;
                }
            });
        }

        public void Repopulate()
        {
            if (vbd == null || vdi == null)
                return;

            //
            // R/O field is disabled if VDI is inherently R/O (ie ISOs) 
            //

            if (vbd.currently_attached)
            {
                modeComboBox.Enabled = false;
                toolTipContainer1.SetToolTip(Messages.VBD_EDIT_CURRENTLY_ATTACHED);
            }
            else
            {
                modeComboBox.Enabled = !vdi.read_only;
                toolTipContainer1.RemoveAll();
            }

            modeComboBox.SelectedIndex = vdi.read_only || vbd.IsReadOnly() ? 1 : 0;

            //
            // Set QoS Value
            //

            diskAccessPriorityTrackBar.Value = vbd.GetIoNice();

            Host coordinator = Helpers.GetCoordinator(vbd.Connection);

            if (sr == null || coordinator == null || !sr.other_config.ContainsKey("scheduler") || sr.other_config["scheduler"] != "cfq")
            {
                DiskPriorityPanel.Visible = false;
                label1.Visible = false;
            }
            else
            {
                DiskPriorityPanel.Visible = true;
                label1.Visible = true;
                DiskPriorityPanel.Enabled = true;
            }
        }

        public bool HasChanged
        {
            get
            {
                return DevicePosition != vbd.userdevice
                    || (modeComboBox.SelectedItem.ToString() == Messages.READ_ONLY) != vbd.IsReadOnly()
                    || (diskAccessPriorityTrackBar.Enabled && diskAccessPriorityTrackBar.Value != vbd.GetIoNice());
            }
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void HideLocalValidationMessages()
        { }

        public void Cleanup()
        {
            InvalidParamToolTip.Dispose();
        }

        private String DevicePosition
        {
            get
            {
                if (devicePositionComboBox.SelectedItem != null)
                    return ((DevicePositionComboBoxItem)devicePositionComboBox.SelectedItem).Position;

                return String.Empty;
            }
        }

        private bool DevicePositionChanged
        {
            get
            {
                return DevicePosition != vbd.userdevice;
            }
        }

        public AsyncAction SaveSettings()
        {
            // Check user has entered valid params
            if (DevicePositionChanged &&
                vdi.type == vdi_type.system)
            {
                DialogResult dialogResult;
                using (var dlg = new WarningDialog(Messages.EDIT_SYS_DISK_WARNING,
                    ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                    {WindowTitle = Messages.EDIT_SYS_DISK_WARNING_TITLE})
                {
                    dialogResult = dlg.ShowDialog(this);
                }
                if (DialogResult.Yes != dialogResult)
                {
                    return null;
                }
            }

            bool diskAccessPriorityEnabled = diskAccessPriorityTrackBar.Enabled;
            int diskAccessPriority = diskAccessPriorityTrackBar.Value;
            vbd_mode vbdMode = modeComboBox.SelectedIndex == 0 ? vbd_mode.RW : vbd_mode.RO;
            string devicePosition = DevicePosition;

            int priorityToSet = vbd.GetIoNice();
            if (diskAccessPriorityEnabled)
            {
                priorityToSet = diskAccessPriority;
            }


            bool changeDevicePosition = false;
            VBD other = null;

            if (devicePosition != vbd.userdevice)
            {
                foreach (VBD otherVBD in vm.Connection.ResolveAll(vm.VBDs))
                {
                    if (otherVBD.userdevice != devicePosition ||
                        vbd.opaque_ref == otherVBD.opaque_ref)
                        continue;

                    other = otherVBD;
                    break;
                }

                if (other == null)
                {
                    changeDevicePosition = true;
                }
                else
                {
                    using (var dialog = new WarningDialog(string.Format(Messages.DEVICE_POSITION_CONFLICT, devicePosition),
                        new ThreeButtonDialog.TBDButton(Messages.DEVICE_POSITION_CONFLICT_SWAP, DialogResult.Yes),
                        new ThreeButtonDialog.TBDButton(Messages.DEVICE_POSITION_CONFLICT_CONFIGURE, DialogResult.No),
                        ThreeButtonDialog.ButtonCancel))
                    {
                        var result = dialog.ShowDialog(this);
                        changeDevicePosition = result != DialogResult.Cancel;

                        if (result == DialogResult.No || !changeDevicePosition)
                            other = null;
                    }
                }
            }
            WarnUserSwap(vbd, other);

            return new VbdEditAction(vbd, vbdMode, priorityToSet, changeDevicePosition, other, devicePosition, true);
        }

        private static void WarnUserSwap(VBD vbd, VBD other)
        {
            VM VBDvm = vbd.Connection.Resolve(vbd.VM);
            if ((other != null && VBDvm.power_state != XenAPI.vm_power_state.Halted) &&
                (
                    (vbd.currently_attached && !vbd.allowed_operations.Contains(vbd_operations.unplug))
                    || (other.currently_attached && !other.allowed_operations.Contains(vbd_operations.unplug))

                )
                )
            {
                Program.Invoke(Program.MainWindow, () =>
                {
                    using (var dlg = new InformationDialog(Messages.DEVICE_POSITION_RESTART_REQUIRED))
                        dlg.ShowDialog(Program.MainWindow);
                });
            }
        }

        public String SubText
        {
            get
            {
                return String.Format(Messages.DEVICE_POSITION, DevicePosition,
                    modeComboBox.SelectedItem);
            }
        }

        public class DevicePositionComboBoxItem
        {
            private VBD vbd;
            private string position;

            public VBD VBD { get { return vbd; } }
            public string Position { get { return position; } }

            public DevicePositionComboBoxItem(string position, VBD vbd = null)
            {
                this.vbd = vbd;
                this.position = position;
            }

            public override string ToString()
            {
                VDI vdi = null;

                if (vbd != null && vbd.Connection != null)
                {
                    vdi = vbd.Connection.Resolve(vbd.VDI);
                }

                if (vbd == null)
                    return position;

                if (vdi != null)
                    return string.Format(Messages.VBD_EDIT_CURRENTLY_IN_USE_BY, position, vdi.ToString().Ellipsise(30));

                return string.Format(Messages.VBD_EDIT_CURRENTLY_IN_USE, position);
            }
        }
    }
}
