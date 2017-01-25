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

using System.Collections.Generic;
using System.Drawing;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.SettingsPanels;
using XenAPI;


namespace XenAdmin.Wizards.NewPolicyWizard
{
    //See notes in base class

    public partial class NewPolicySnapshotTypePageSpecific<T> : NewPolicySnapshotTypePage  where T : XenObject<T>
    {

        public NewPolicySnapshotTypePageSpecific() : base()
        {
            this.labelWarning.Text = string.Format(this.labelWarning.Text, VMGroup<T>.VMPolicyTypeName);
        }

        public NewPolicySnapshotTypePageSpecific(List<VM> selectedVMS)
            : base(selectedVMS)
        {

        }

        public override string SubText
        {
            get
            {
                if (BackupType == policy_backup_type.snapshot)
                    return Messages.DISKS_ONLY;
                else if (BackupType == policy_backup_type.snapshot_with_quiesce)
                    return Messages.QUIESCED_SNAPSHOTS;
                else
                    return Messages.DISKS_AND_MEMORY;
            }
        
        }

        public override string HelpID
        {
            get { return "Snapshottype"; }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.SNAPSHOT_TYPE_TITLE;
            }
        }
        public override string Text
        {
            get
            {
                return Messages.SNAPSHOT_TYPE;
            }
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            if (direction == PageLoadedDirection.Forward)
                EnableShapshotTypes(Connection, false);
        }

        public override Image Image
        {
            get { return Properties.Resources._000_VMSession_h32bit_16; }
        }

        public override bool ValidToSave
        {
            get { return true; }
        }

        public override void ShowLocalValidationMessages()
        {

        }

        public override void Cleanup()
        {
            radioButtonDiskOnly.Checked = true;
        }

        public override void checkpointInfoPictureBox_Click(object sender, System.EventArgs e)
        {
            toolTip.Show(Messages.FIELD_DISABLED, checkpointInfoPictureBox, 20, 0);
        }

        public override void checkpointInfoPictureBox_MouseLeave(object sender, System.EventArgs e)
        {
            toolTip.Hide(checkpointInfoPictureBox);
        }

        public override void pictureBoxVSS_Click(object sender, System.EventArgs e)
        {
            string tt = Messages.INFO_QUIESCE_MODE.Replace("\\n", "\n");  // This says that VSS must be enabled. This is a guess, because we can't tell whether it is or not.
            toolTip.Show(tt, pictureBoxVSS, 20, 0);
        }

        public override void pictureBoxVSS_MouseLeave(object sender, System.EventArgs e)
        {
            toolTip.Hide(pictureBoxVSS);
        }

        public override void quiesceCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            if (this.quiesceCheckBox.Checked)
            {
                this.radioButtonDiskOnly.Checked = true;
            }
        }

        public policy_backup_type BackupType
        {
            get
            {
                if (quiesceCheckBox.Checked)
                    return policy_backup_type.snapshot_with_quiesce;
                if (radioButtonDiskOnly.Checked)
                    return policy_backup_type.snapshot;
                else if (radioButtonDiskAndMemory.Checked)
                    return policy_backup_type.checkpoint;
                else
                {
                    return policy_backup_type.unknown;
                }
            }
        }

        public void ToggleQuiesceCheckBox(List <VM> SelectedVMs)
        {

            switch (BackupType)
            {
                case policy_backup_type.snapshot:
                    quiesceCheckBox.Enabled = true;
                    quiesceCheckBox.Checked = false;
                    break;

                case policy_backup_type.snapshot_with_quiesce:
                    quiesceCheckBox.Enabled = true;
                    quiesceCheckBox.Checked = true;
                    break;

                case policy_backup_type.checkpoint:
                    quiesceCheckBox.Enabled = true;
                    quiesceCheckBox.Checked = false;
                    break;
            }

            foreach (VM vm in SelectedVMs)
            {
                if (!vm.allowed_operations.Contains(vm_operations.snapshot_with_quiesce) || Helpers.FeatureForbidden(vm, Host.RestrictVss))
                {
                    quiesceCheckBox.Enabled = false;
                    quiesceCheckBox.Checked = false;
                    break;
                }
            }

        }

        private void RefreshTab(IVMPolicy policy)
        {
            /* when a policy does not have any VMs, irrespective of
             * the snapshot type, enable Quiesce 
             */

            quiesceCheckBox.Enabled = (policy.VMs.Count == 0);
 
            switch (policy.policy_type)
            {
                case policy_backup_type.checkpoint:
                    radioButtonDiskAndMemory.Checked = true;
                    quiesceCheckBox.Enabled = false;
                    break;
                case policy_backup_type.snapshot:
                    radioButtonDiskOnly.Checked = true;
                    break;
                case policy_backup_type.snapshot_with_quiesce:
                    radioButtonDiskOnly.Checked = true;

                    /* when the snapshot type itself is quiesce then we need to 
                     * enable it irrespective of the number of VMs ( > 1 condition)
                     */

                    quiesceCheckBox.Enabled = true;
                    quiesceCheckBox.Checked = true;
                    break;
            }
            EnableShapshotTypes(policy.Connection, quiesceCheckBox.Enabled);
        }

        private void EnableShapshotTypes(IXenConnection connection, bool isQuiesceEnabled)
        {
            radioButtonDiskAndMemory.Enabled = label3.Enabled = !Helpers.FeatureForbidden(connection, Host.RestrictCheckpoint);
            checkpointInfoPictureBox.Visible = !radioButtonDiskAndMemory.Enabled;
            pictureBoxWarning.Visible = labelWarning.Visible = radioButtonDiskAndMemory.Enabled;
            this.quiesceCheckBox.Enabled = true;
            
            if (VMGroup<T>.isQuescingSupported)
            {
                this.quiesceCheckBox.Visible = true;
                if (this._selectedVMs != null)
                {
                    if (this._selectedVMs.Count > 0)
                    {
                        foreach (VM vm in this._selectedVMs)
                        {
                            if (!vm.allowed_operations.Contains(vm_operations.snapshot_with_quiesce) || Helpers.FeatureForbidden(vm, Host.RestrictVss))
                            {
                                this.quiesceCheckBox.Enabled = false;
                                this.quiesceCheckBox.Checked = false;
                                break;
                            }
                        }

                    }

                }
                else /* we enter this block only when we are editing a policy, in that case the decision has already been taken in RefreshTab function */
                {
                    this.quiesceCheckBox.Enabled = isQuiesceEnabled;
                }
                this.pictureBoxVSS.Visible = !this.quiesceCheckBox.Enabled;
            }
            else /*quiescing snapshots are not supported in VMPP*/
            {
                this.quiesceCheckBox.Visible = false;
                this.pictureBoxVSS.Visible = false;
            }
            
        }

        public override AsyncAction SaveSettings()
        {
            _policy.policy_type = BackupType;         
            return null;
        }

        private IVMPolicy _policy;

        public override void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _policy = (IVMPolicy)clone;
            RefreshTab(_policy);
        }

        public override bool HasChanged
        {
            get
            {
                return BackupType != _policy.policy_type;
            }

        }

        public override void radioButtonDiskAndMemory_CheckedChanged(object sender, System.EventArgs e)
        {
            if(this.quiesceCheckBox.Enabled)
            {
                this.quiesceCheckBox.Checked = false;
            }
        }
        
    }
}
