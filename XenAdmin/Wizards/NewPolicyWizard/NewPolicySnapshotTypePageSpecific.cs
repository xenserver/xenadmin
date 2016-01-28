/* Copyright (c) Citrix Systems Inc. 
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
            
        }

        public NewPolicySnapshotTypePageSpecific(List<VM> selectedVMS)
            : base(selectedVMS)
        {
            if (typeof(T) == typeof(VMSS))
            {
                this.quiesceCheckBox.Visible = true;
                this.pictureBoxVSS.Visible = !this.quiesceCheckBox.Enabled;
                if (selectedVMS != null)
                {
                    foreach (VM vm in selectedVMS)
                    {
                        if (!vm.allowed_operations.Contains(vm_operations.snapshot_with_quiesce) || Helpers.FeatureForbidden(vm, Host.RestrictVss))
                        {
                            this.quiesceCheckBox.Enabled = false;
                            break;
                        }
                    }
                }
                else
                {
                    this.quiesceCheckBox.Enabled = false;
                }
            }
            else
            {
                this.quiesceCheckBox.Visible = false;
                this.pictureBoxVSS.Visible = false;
            }

        }

        public override string SubText
        {
            get
            {
                if (typeof(T) == typeof(VMPP))
                {
                    if (BackupType == vmpp_backup_type.snapshot)
                        return Messages.DISKS_ONLY;
                    else
                    {
                        return Messages.DISKS_AND_MEMORY;
                    }
                }
                else
                {
                    if (BackupTypeVMSS == vmss_backup_type.snapshot)
                        return Messages.DISKS_ONLY;
                    else if (BackupTypeVMSS == vmss_backup_type.snapshot_with_quiesce)
                        return Messages.QUIESCED_SNAPSHOTS;
                    else
                        return Messages.DISKS_AND_MEMORY;
                }
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
                EnableShapshotTypes(Connection);
        }


        public vmpp_backup_type BackupType
        {
            get
            {
                if (radioButtonDiskOnly.Checked)
                    return vmpp_backup_type.snapshot;
                else if (radioButtonDiskAndMemory.Checked)
                    return vmpp_backup_type.checkpoint;
                else
                {
                    return vmpp_backup_type.unknown;
                }
            }
        }

        public vmss_backup_type BackupTypeVMSS
        {
            get
            {
                if (quiesceCheckBox.Checked)
                    return vmss_backup_type.snapshot_with_quiesce;
                if (radioButtonDiskOnly.Checked)
                    return vmss_backup_type.snapshot;
                else if (radioButtonDiskAndMemory.Checked)
                    return vmss_backup_type.checkpoint;
                else
                {
                    return vmss_backup_type.unknown;
                }
            }
        }

        private void RefreshTab(VMPP vmpp)
        {
            switch (vmpp.backup_type)
            {
                case vmpp_backup_type.checkpoint:
                    radioButtonDiskAndMemory.Checked = true;
                    break;
                case vmpp_backup_type.snapshot:
                    radioButtonDiskOnly.Checked = true;
                    break;
            }
            EnableShapshotTypes(vmpp.Connection);
        }

        private void RefreshTabVMSS(VMSS vmss)
        {
            switch (vmss.backup_type)
            {
                case vmss_backup_type.checkpoint:
                    radioButtonDiskAndMemory.Checked = true;
                    quiesceCheckBox.Enabled = false;
                    break;
                case vmss_backup_type.snapshot:
                    radioButtonDiskOnly.Checked = true;
                    quiesceCheckBox.Enabled = true;
                    break;
            }
            EnableShapshotTypes(vmss.Connection);
        }

        private void EnableShapshotTypes(IXenConnection connection)
        {
            radioButtonDiskAndMemory.Enabled = label3.Enabled = !Helpers.FeatureForbidden(connection, Host.RestrictCheckpoint);
            checkpointInfoPictureBox.Visible = !radioButtonDiskAndMemory.Enabled;
            pictureBoxWarning.Visible = labelWarning.Visible = radioButtonDiskAndMemory.Enabled;

            if (typeof(T) == typeof(VMSS))
            {
                this.quiesceCheckBox.Visible = true;
                if (this._selectedVMs != null)
                {
                    foreach (VM vm in this._selectedVMs)
                    {
                        if (!vm.allowed_operations.Contains(vm_operations.snapshot_with_quiesce) || Helpers.FeatureForbidden(vm, Host.RestrictVss))
                        {
                            this.quiesceCheckBox.Enabled = false;
                            break;
                        }
                    }
                }
                else
                {
                    this.quiesceCheckBox.Enabled = false;
                }
                this.pictureBoxVSS.Visible = !this.quiesceCheckBox.Enabled;
            }
            else
            {
                this.quiesceCheckBox.Visible = false;
                this.pictureBoxVSS.Visible = false;
            }
            
        }

        public override AsyncAction SaveSettings()
        {
            if (typeof(T) == typeof(VMPP))
                _clone.backup_type = BackupType;
            else
                _cloneVMSS.backup_type = BackupTypeVMSS;
            
            return null;
        }

        private VMPP _clone;
        private VMSS _cloneVMSS;

        public override void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            if (typeof(T) == typeof(VMPP))
            {
                _clone = (VMPP)clone;
                RefreshTab(_clone);
            }
            else
            {
                _cloneVMSS = (VMSS)clone;
                RefreshTabVMSS(_cloneVMSS);
            }
        }

        public override bool HasChanged
        {
            get
            {
                if (typeof(T) == typeof(VMPP))
                    return BackupType != _clone.backup_type;
                else
                    return BackupTypeVMSS != _cloneVMSS.backup_type;
            }

        }

        
    }
}
