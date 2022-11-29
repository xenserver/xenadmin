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
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Wizards.CrossPoolMigrateWizard
{
    public partial class IntraPoolCopyPage : XenTabPage
    {
        private bool _buttonNextEnabled;

        public IntraPoolCopyPage(List<VM> selectedVMs)
        {
            TheVM = selectedVMs[0]; 
            InitializeComponent();
        }

        public VM TheVM { get; }

        public bool CloneVM => !tableLayoutPanelSrPicker.Enabled || CloneRadioButton.Checked;

        public SR SelectedSR => srPicker1.SR;

        public string NewVmName => NameTextBox.Text;

        public string NewVMmDescription => DescriptionTextBox.Text;

        #region Base class (XenTabPage) overrides

        /// <summary>
        /// Gets the page's title (headline)
        /// </summary>
        public override string PageTitle => Messages.CPM_WIZARD_INTRA_POOL_COPY_TITLE;

        /// <summary>
        /// Gets the page's label in the (left hand side) wizard progress panel
        /// </summary>
        public override string Text => Messages.CPM_WIZARD_INTRA_POOL_COPY_TAB_TITLE;

        /// <summary>
        /// Gets the value by which the help files section for this page is identified
        /// </summary>
        public override string HelpID => "IntraPoolCopy";

        protected override bool ImplementsIsDirty()
        {
            return false;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            UpdateButtons();
        }

        public override void PopulatePage()
        {
            labelSrHint.Text = TheVM.is_a_template ? Messages.COPY_TEMPLATE_SELECT_SR : Messages.COPY_VM_SELECT_SR;

            NameTextBox.Text = GetDefaultCopyName(TheVM);

            bool allow_copy = !TheVM.is_a_template || TheVM.allowed_operations.Contains(vm_operations.copy);
            bool anyDiskFastCloneable = TheVM.AnyDiskFastClonable();
            bool hasAtLeastOneDisk = TheVM.HasAtLeastOneDisk();

            CopyRadioButton.Enabled = allow_copy && hasAtLeastOneDisk;
            FastClonePanel.Enabled = !allow_copy || anyDiskFastCloneable || !hasAtLeastOneDisk;

            if (!FastClonePanel.Enabled)
                CloneRadioButton.Checked = false;

            if (!CloneRadioButton.Enabled)
                CopyRadioButton.Checked = true;

            toolTipContainer1.SetToolTip(Messages.FAST_CLONE_UNAVAILABLE);

            if (TheVM.is_a_template && !(anyDiskFastCloneable || allow_copy))
            {
                CloneRadioButton.Text = Messages.COPY_VM_CLONE_TEMPLATE_SLOW;
                FastCloneDescription.Text = Messages.COPY_VM_SLOW_CLONE_DESCRIPTION;
            }
            else
            {
                FastCloneDescription.Text = !TheVM.is_a_template ? Messages.COPY_VM_FAST_CLONE_DESCRIPTION : Messages.COPY_TEMPLATE_FAST_CLONE_DESCRIPTION;
            }

            if (TheVM.DescriptionType() != VM.VmDescriptionType.None)
                DescriptionTextBox.Text = TheVM.Description();

            tableLayoutPanelSrPicker.Enabled = CopyRadioButton.Enabled && CopyRadioButton.Checked;

            labelRubric.Text = TheVM.is_a_template
                                   ? Messages.COPY_TEMPLATE_INTRA_POOL_RUBRIC
                                   : Messages.COPY_VM_INTRA_POOL_RUBRIC;

            UpdateButtons();

            var vdis = (from VBD vbd in TheVM.Connection.ResolveAll(TheVM.VBDs)
                where vbd.type != vbd_type.CD
                let vdi = TheVM.Connection.Resolve(vbd.VDI)
                where vdi != null
                select vdi).ToArray();

            srPicker1.Populate(SrPicker.SRPickerType.Copy, TheVM.Connection, TheVM.Home(), null, vdis);
        }

        public override bool EnableNext()
        {
            return _buttonNextEnabled;
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction != PageLoadedDirection.Forward)
                return;

            var l = new List<VM> {TheVM};
            if (!CrossPoolMigrateWizard.AllVMsAvailable(l))
                cancel = true;
        }

        #endregion

        private void UpdateButtons()
        {
            if (string.IsNullOrEmpty(NameTextBox.Text.Trim()))
                _buttonNextEnabled = false;
            else if (CopyRadioButton.Checked)
                _buttonNextEnabled = srPicker1.SR != null;
            else
                _buttonNextEnabled = true;

            OnPageUpdated();
        }

        private void EnableRescanButton()
        {
            buttonRescan.Enabled = tableLayoutPanelSrPicker.Enabled && srPicker1.CanBeScanned;
        }

        private void srPicker1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void srPicker1_CanBeScannedChanged()
        {
            EnableRescanButton();
            UpdateButtons();
        }

        private void buttonRescan_Click(object sender, EventArgs e)
        {
            srPicker1.ScanSRs();
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private static string GetDefaultCopyName(VM vmToCopy)
        {
            List<string> takenNames = new List<string>();
            foreach (VM vm in vmToCopy.Connection.Cache.VMs)
            {
                takenNames.Add(vm.Name());
            }
            return Helpers.MakeUniqueName(string.Format(Messages.ACTION_TEMPLATE_CLONE_NEW_NAME, vmToCopy.Name()), takenNames);
        }

        private void CloneRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Since the radiobuttons aren't in the same panel, we have to do manual mutual exclusion
            CopyRadioButton.Checked = !CloneRadioButton.Checked;
            UpdateButtons();
        }

        private void CopyRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            tableLayoutPanelSrPicker.Enabled = CopyRadioButton.Checked;
            EnableRescanButton();
            // Since the radiobuttons aren't in the same panel, we have to do manual mutual exclusion
            CloneRadioButton.Checked = !CopyRadioButton.Checked;
            UpdateButtons();
        }
    }
}
