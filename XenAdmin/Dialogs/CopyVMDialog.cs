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
using System.Linq;
using XenAdmin.Actions.VMActions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Dialogs
{
    public partial class CopyVMDialog : XenDialogBase
    {
        private readonly VM _vm;

        public CopyVMDialog(VM vm)
        {
            InitializeComponent();
            _vm = vm;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            labelSrHint.Text = _vm.is_a_template ? Messages.COPY_TEMPLATE_SELECT_SR : Messages.COPY_VM_SELECT_SR;

            NameTextBox.Text = GetDefaultCopyName(_vm);

            Text = _vm.is_a_template ? Messages.COPY_TEMPLATE : Messages.COPY_VM;

            bool allowCopy = !_vm.is_a_template || _vm.allowed_operations.Contains(vm_operations.copy);
            bool anyDiskFastCloneable = _vm.AnyDiskFastClonable();
            bool hasAtLeastOneDisk = _vm.HasAtLeastOneDisk();

            CopyRadioButton.Enabled = allowCopy && hasAtLeastOneDisk;
            FastClonePanel.Enabled = !allowCopy || anyDiskFastCloneable || !hasAtLeastOneDisk;

            if (!FastClonePanel.Enabled)
                CloneRadioButton.Checked = false;

            if (!CloneRadioButton.Enabled)
                CopyRadioButton.Checked = true;

            tableLayoutPanelSrPicker.Enabled = CopyRadioButton.Enabled && CopyRadioButton.Checked;

            toolTipContainer1.SetToolTip(Messages.FAST_CLONE_UNAVAILABLE);

            if (_vm.is_a_template && !(anyDiskFastCloneable || allowCopy))
            {
                CloneRadioButton.Text = Messages.COPY_VM_CLONE_TEMPLATE_SLOW;
                FastCloneDescription.Text = Messages.COPY_VM_SLOW_CLONE_DESCRIPTION;
            }
            else
            {
                FastCloneDescription.Text = !_vm.is_a_template
                    ? Messages.COPY_VM_FAST_CLONE_DESCRIPTION
                    : Messages.COPY_TEMPLATE_FAST_CLONE_DESCRIPTION;
            }

            if (_vm.DescriptionType() != VM.VmDescriptionType.None)
                DescriptionTextBox.Text = _vm.Description();

            EnableMoveButton();

            var vdis = (from VBD vbd in _vm.Connection.ResolveAll(_vm.VBDs)
                where vbd.type != vbd_type.CD
                let vdi = _vm.Connection.Resolve(vbd.VDI)
                where vdi != null
                select vdi).ToArray();

            srPicker1.Populate(SrPicker.SRPickerType.Copy, _vm.Connection, _vm.Home(), null, vdis);
        }

        private void EnableMoveButton()
        {
            MoveButton.Enabled = NameTextBox.Text.Trim().Length > 0 && srPicker1.SR != null;
        }

        private void EnableRescanButton()
        {
            buttonRescan.Enabled = tableLayoutPanelSrPicker.Enabled && srPicker1.CanBeScanned;
        }

        private static string GetDefaultCopyName(VM vmToCopy)
        {
            var takenNames = vmToCopy.Connection.Cache.VMs.Select(vm => vm.Name()).ToList();

            return Helpers.MakeUniqueName(string.Format(Messages.ACTION_TEMPLATE_CLONE_NEW_NAME, vmToCopy.Name()), takenNames);
        }

        #region Control event handlers

        private void srPicker1_CanBeScannedChanged()
        {
            EnableRescanButton();
            EnableMoveButton();
        }

        private void srPicker1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableMoveButton();
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            EnableMoveButton();
        }

        private void buttonRescan_Click(object sender, EventArgs e)
        {
            srPicker1.ScanSRs();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MoveButton_Click(object sender, EventArgs e)
        {
            if (!tableLayoutPanelSrPicker.Enabled || CloneRadioButton.Checked)
            {
                new VMCloneAction(_vm,NameTextBox.Text, DescriptionTextBox.Text).RunAsync();
                Close();
                return;
            }

            if (srPicker1.SR == null)
                return;

            var action = new VMCopyAction(_vm, _vm.GetStorageHost(false), srPicker1.SR, NameTextBox.Text, DescriptionTextBox.Text);

            action.RunAsync();
            Close();
        }

        private void CopyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanelSrPicker.Enabled = CopyRadioButton.Checked;
            EnableRescanButton();
            // Since the radiobuttons aren't in the same panel, we have to do manual mutual exclusion
            CloneRadioButton.Checked = !CopyRadioButton.Checked;
        }

        private void CloneRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Since the radiobuttons aren't in the same panel, we have to do manual mutual exclusion
            CopyRadioButton.Checked = !CloneRadioButton.Checked;
        }

        #endregion
    }
}
