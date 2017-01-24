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

using System.Drawing;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.SettingsPanels;
using XenAPI;


namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class NewPolicySnapshotTypePage : XenTabPage, IEditPage
    {
        public NewPolicySnapshotTypePage()
        {
            InitializeComponent();
        }

        public override string Text
        {
            get
            {
                return Messages.SNAPSHOT_TYPE;
            }
        }

        public string SubText
        {
            get
            {
                if (BackupType == vmpp_backup_type.snapshot)
                    return Messages.DISKS_ONLY;
                else
                {
                    return Messages.DISKS_AND_MEMORY;
                }
            }
        }

        public override string HelpID
        {
            get { return "Snapshottype"; }
        }

        public Image Image
        {
            get { return Properties.Resources._000_VMSession_h32bit_16; }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.SNAPSHOT_TYPE_TITLE;
            }
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

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            if (direction == PageLoadedDirection.Forward)
                EnableShapshotTypes(Connection);
        }

        private void EnableShapshotTypes(IXenConnection connection)
        {
            radioButtonDiskAndMemory.Enabled = label3.Enabled = !Helpers.FeatureForbidden(connection, Host.RestrictCheckpoint);
            checkpointInfoPictureBox.Visible = !radioButtonDiskAndMemory.Enabled;
            pictureBoxWarning.Visible = labelWarning.Visible = radioButtonDiskAndMemory.Enabled;
        }

        public AsyncAction SaveSettings()
        {
            _clone.backup_type = BackupType;
            return null;
        }

        private VMPP _clone;
        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _clone = (VMPP)clone;
            RefreshTab(_clone);
        }

        public bool ValidToSave
        {
            get { return true; }
        }

        public void ShowLocalValidationMessages()
        {

        }

        public void Cleanup()
        {
            radioButtonDiskOnly.Checked = true;
        }

        public bool HasChanged
        {
            get { return BackupType != _clone.backup_type; }
        }

        private void checkpointInfoPictureBox_Click(object sender, System.EventArgs e)
        {
            toolTip.Show(Messages.FIELD_DISABLED, checkpointInfoPictureBox, 20, 0);
        }

        private void checkpointInfoPictureBox_MouseLeave(object sender, System.EventArgs e)
        {
            toolTip.Hide(checkpointInfoPictureBox);
        }
    }
}
