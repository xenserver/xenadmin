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
using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Dialogs
{
    public partial class VmSnapshotDialog : XenDialogBase
    {
        private readonly VM _VM;

        public VmSnapshotDialog(VM vm)
        {
            InitializeComponent();
            _VM = vm;
            diskRadioButton.Enabled = _VM.allowed_operations.Contains(vm_operations.snapshot);
            pictureBoxSnapshotsInfo.Visible = !diskRadioButton.Enabled;
            quiesceCheckBox.Enabled = _VM.allowed_operations.Contains(vm_operations.snapshot_with_quiesce) 
                && !Helpers.FeatureForbidden(_VM, Host.RestrictVss);
            pictureBoxQuiesceInfo.Visible = !quiesceCheckBox.Enabled;
            memoryRadioButton.Enabled = _VM.allowed_operations.Contains(vm_operations.checkpoint)
                && !Helpers.FeatureForbidden(_VM, Host.RestrictCheckpoint);
            CheckpointInfoPictureBox.Visible = !memoryRadioButton.Enabled;
            UpdateOK();
        }

        /// <summary>
        /// Must be accessed on the GUI thread.
        /// </summary>
        public string SnapshotName
        {
            get
            {
                return textBoxName.Text;
            }
        }

        public string SnapshotDescription
        {
            get
            {
                return textBoxDescription.Text;
            }
        }

        public SnapshotType SnapshotType
        {
            get
            {
                if (quiesceCheckBox.Checked)
                    return SnapshotType.QUIESCED_DISK;
                else if (diskRadioButton.Checked)
                    return SnapshotType.DISK;
                else
                    return SnapshotType.DISK_AND_MEMORY;
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            UpdateOK();
        }

        private void UpdateOK()
        {
            buttonOk.Enabled = !String.IsNullOrEmpty(textBoxName.Text.Trim())&&(diskRadioButton.Enabled ||quiesceCheckBox.Enabled||memoryRadioButton.Enabled );
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioSender = (RadioButton)sender;
            if (radioSender.Checked)
            {
                if (radioSender == diskRadioButton)
                {
                    memoryRadioButton.Checked = false;
                }
                else
                {
                    diskRadioButton.Checked = false;
                    quiesceCheckBox.Checked = false;
                }
            }
        }

        private void VmSnapshotDialog_Load(object sender, EventArgs e)
        {
            textBoxName.Select();
        }

        private void quiesceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (quiesceCheckBox.Checked)
            {
                diskRadioButton.Checked = true;
            }
        }

        private void pictureBoxQuiesceInfo_Click(object sender, EventArgs e)
        {
            string tt;
            if (Helpers.FeatureForbidden(_VM, Host.RestrictVss))
                tt = Messages.FIELD_DISABLED;
            else if (_VM.power_state != vm_power_state.Running)
                tt = Messages.INFO_QUIESCE_MODE_POWER_STATE.Replace("\\n", "\n");
            else if (!_VM.GetVirtualisationStatus.HasFlag(VM.VirtualisationStatus.MANAGEMENT_INSTALLED))
                tt = (_VM.HasNewVirtualisationStates ? Messages.INFO_QUIESCE_MODE_NO_MGMNT : Messages.INFO_QUIESCE_MODE_NO_TOOLS).Replace("\\n", "\n");
            else
                tt = Messages.INFO_QUIESCE_MODE.Replace("\\n","\n");  // This says that VSS must be enabled. This is a guess, because we can't tell whether it is or not.
            toolTip.Show(tt ,pictureBoxQuiesceInfo, 20, 0);
        }

        private void pictureBoxQuiesceInfo_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(pictureBoxQuiesceInfo);
        }

        private void pictureBoxSnapshotsInfo_Click(object sender, EventArgs e)
        {
            toolTip.Show(Messages.INFO_DISK_MODE.Replace("\\n","\n"), pictureBoxSnapshotsInfo, 20, 0);
        }

        private void pictureBoxSnapshotsInfo_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(pictureBoxSnapshotsInfo);
        }

        private void CheckpointInfoPictureBox_Click(object sender, EventArgs e)
        {
            string tt;
            if (Helpers.FeatureForbidden(_VM, Host.RestrictCheckpoint))
                tt = Messages.FIELD_DISABLED;
            else if (_VM.power_state != vm_power_state.Running)
                tt = Messages.INFO_DISKMEMORY_MODE_POWER_STATE.Replace("\\n", "\n");
            else if (_VM.HasVGPUs)
                tt = Messages.INFO_DISKMEMORY_MODE_GPU.Replace("\\n", "\n");
            else if (!_VM.virtualisation_status.HasFlag(VM.VirtualisationStatus.IO_DRIVERS_INSTALLED))
                tt = (_VM.HasNewVirtualisationStates ? Messages.INFO_DISKMEMORY_MODE_NO_IO_DRIVERS : Messages.INFO_DISKMEMORY_MODE_NO_TOOLS).Replace("\\n", "\n");
            else
                tt = Messages.INFO_DISKMEMORY_MODE_MISC.Replace("\\n", "\n");
            toolTip.Show(tt, CheckpointInfoPictureBox, 20, 0);
        }

        private void CheckpointInfoPictureBox_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(CheckpointInfoPictureBox);
        }


    }
}
