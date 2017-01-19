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
using System.Text;
using XenAdmin.Controls;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Commands;

namespace XenAdmin.Dialogs
{
    public partial class MoveVirtualDiskDialog : XenDialogBase
    {
        private const int BATCH_SIZE = 3;
        
        private readonly List<VDI> _vdisToMove;
        private readonly List<VDI> _vdisToMigrate;

        /// <summary>
        /// Designer support only. Do not use.
        /// </summary>
        public MoveVirtualDiskDialog()
        {
            InitializeComponent();
        }

        public MoveVirtualDiskDialog(IXenConnection connection, List<VDI> vdisToMove, List<VDI> vdisToMigrate)
            : base(connection)
        {
            InitializeComponent();

            //set those so we don't have to bother with null checks further down
            _vdisToMove = vdisToMove ?? new List<VDI>();
            _vdisToMigrate = vdisToMigrate ?? new List<VDI>();

            if (_vdisToMove.Count > 0)
            {
                srPicker1.Usage = SrPicker.SRPickerType.MoveOrCopy;
                srPicker1.SetExistingVDIs(_vdisToMove.ToArray());
                srPicker1.DiskSize = _vdisToMove.Sum(d => d.physical_utilisation);
            }
            else if (_vdisToMigrate.Count > 0)
            {
                srPicker1.Usage = SrPicker.SRPickerType.Migrate;
                srPicker1.SetExistingVDIs(_vdisToMigrate.ToArray());
                srPicker1.DiskSize = _vdisToMigrate.Sum(d => d.physical_utilisation);
            }
            
            srPicker1.SrHint.Visible = false;
            srPicker1.Connection = connection;
            srPicker1.srListBox.Invalidate();
            srPicker1.selectDefaultSROrAny();
        }

        private SR SelectedSR
        {
            get { return srPicker1.SR; }
        }
        
        private void srPicker1_ItemSelectionNull()
        {
            updateButtons();
        }

        private void srPicker1_ItemSelectionNotNull()
        {
            updateButtons();
        }

        void SRPicker_DoubleClickOnRow(object sender, EventArgs e)
        {
            if (buttonMove.Enabled)
                buttonMove.PerformClick();
        }

        private void updateButtons()
        {
            buttonMove.Enabled = srPicker1.SR != null;
        }

        private void buttonMove_Click(object sender, EventArgs e)
        {
            CreateAndRunParallelActions();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CreateAndRunParallelActions()
        {
            if (_vdisToMigrate.Count == 1)
            {
                new MigrateVirtualDiskAction(connection, _vdisToMigrate[0], SelectedSR).RunAsync();
            }
            else if (_vdisToMigrate.Count > 1)
            {
                string title = string.Format(Messages.ACTION_MIGRATING_X_VDIS, _vdisToMigrate.Count, SelectedSR.name_label);

                var batch = from VDI vdi in _vdisToMigrate
                    select (AsyncAction)new MigrateVirtualDiskAction(connection, vdi, SelectedSR);

                new ParallelAction(connection, title, Messages.ACTION_MIGRATING_X_VDIS_STARTED,
                    Messages.ACTION_MIGRATING_X_VDIS_COMPLETED, batch.ToList(), BATCH_SIZE).RunAsync();
            }

            if (_vdisToMove.Count == 1)
            {
                new MoveVirtualDiskAction(connection, _vdisToMove[0], SelectedSR).RunAsync();
            }
            else if (_vdisToMove.Count > 1)
            {
                string title = string.Format(Messages.ACTION_MOVING_X_VDIS, _vdisToMove.Count, SelectedSR.name_label);

                var batch = from VDI vdi in _vdisToMove
                    select (AsyncAction)new MoveVirtualDiskAction(connection, vdi, SelectedSR);

                new ParallelAction(connection, title, Messages.ACTION_MOVING_X_VDIS_STARTED,
                    Messages.ACTION_MOVING_X_VDIS_COMPLETED, batch.ToList(), BATCH_SIZE).RunAsync();
            }
        }

        internal override string HelpName
        {
            get { return "VDIMigrateDialog"; }
        }

        internal static Command MoveMigrateCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
        {
            var cmd = new MigrateVirtualDiskCommand(mainWindow, selection);

            if (cmd.CanExecute())
                return cmd;

            return new MoveVirtualDiskCommand(mainWindow, selection);
        }
    }
}
