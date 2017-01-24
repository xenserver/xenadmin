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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Dialogs;


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

        public Image Image
        {
            get
            {
                return Properties.Resources._000_VM_h32bit_16;
            }
        }

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

                    devicePositionComboBox.Items.AddRange(devices.ToArray());

                    // Make sure the userdevice for the selected VBD is the one selected in the combobox
                    foreach (String position in devicePositionComboBox.Items)
                    {
                        if (position != vbd.userdevice)
                            continue;

                        devicePositionComboBox.SelectedItem = position;
                        break;
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

            modeComboBox.SelectedIndex = vdi.read_only || vbd.read_only ? 1 : 0;

            //
            // Set QoS Value
            //

            diskAccessPriorityTrackBar.Value = vbd.IONice;

            Host master = Helpers.GetMaster(vbd.Connection);

            if (sr == null || master == null || !sr.other_config.ContainsKey("scheduler") || sr.other_config["scheduler"] != "cfq")
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

        private void comboBoxDevicePosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DevicePosition == vbd.userdevice)
            {
                labelDevicePositionMsg.Visible = false;
                return;
            }

            foreach (VBD otherVBD in vm.Connection.ResolveAll(vm.VBDs))
            {
                if (otherVBD.userdevice != DevicePosition)
                    continue;

                labelDevicePositionMsg.Text = string.Format(Messages.DEVICE_POSITION_IN_USE);
                labelDevicePositionMsg.Visible = true;
                return;
            }

            // No conflict found.
            labelDevicePositionMsg.Visible = false;
        }

        public bool HasChanged
        {
            get
            {
                return DevicePosition != vbd.userdevice
                    || (modeComboBox.SelectedItem.ToString() == Messages.READ_ONLY) != vbd.read_only
                    || (diskAccessPriorityTrackBar.Enabled && diskAccessPriorityTrackBar.Value != vbd.IONice);
            }
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
            InvalidParamToolTip.Dispose();
        }

        private String DevicePosition
        {
            get
            {
                if (devicePositionComboBox.SelectedItem != null)
                    return devicePositionComboBox.SelectedItem.ToString();

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
                using (var dlg = new ThreeButtonDialog(
                    new ThreeButtonDialog.Details(SystemIcons.Warning, Messages.EDIT_SYS_DISK_WARNING,
                        Messages.EDIT_SYS_DISK_WARNING_TITLE),
                    ThreeButtonDialog.ButtonYes,
                    ThreeButtonDialog.ButtonNo))
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

            int priorityToSet = vbd.IONice;
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
                    // The selected userdevice is already in use. Ask the user what to do about this.
                    DialogResult result = new UserDeviceDialog(devicePosition).ShowDialog(this);

                    changeDevicePosition = result != DialogResult.Cancel;

                    if (result == DialogResult.No || !changeDevicePosition)
                        other = null;
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
                    using (var dlg = new ThreeButtonDialog(
                                    new ThreeButtonDialog.Details(SystemIcons.Information, Messages.DEVICE_POSITION_RESTART_REQUIRED, Messages.XENCENTER)))
                    {
                        dlg.ShowDialog(Program.MainWindow);
                    }
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
    }
}
