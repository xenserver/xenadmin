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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Commands;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public class VDIMigrateDialog : MoveVirtualDiskDialog
    {
        private readonly List<VDI> vdis;
        public VDIMigrateDialog(IXenConnection connection, List<VDI> vdis) : base(connection)
        {
            this.vdis = vdis;
            SRPicker.DiskSize = vdis.Sum(d => d.physical_utilisation);
            SRPicker.SetUsageAsMigrateVDI(vdis.ToArray());
            SRPicker.SrHint.Text = Messages.MIGRATE_VDI_DIALOG_SRHINT;
            SRPicker.Connection = connection;
            SetupEventHandlers();
        }

        public SR SelectedSR
        {
            get { return SRPicker.SR; }
        }

        protected override void buttonMove_Click(object sender, EventArgs e)
        {
            CreateAndRunParallelActions();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CreateAndRunParallelActions()
        {
            BatchingMigrateVirtualDiskManager manager = 
                new BatchingMigrateVirtualDiskManager(connection);

            if (VdisToMigrate().Count > 0 )
                manager.BatchAs(BatchingMigrateVirtualDiskManager.Batching.VdiMigrate, 
                                VdisToMigrate(), SelectedSR).RunAsync();

            if (VdisToMove().Count > 0)
                manager.BatchAs(BatchingMigrateVirtualDiskManager.Batching.VdiMove, 
                                VdisToMove(), SelectedSR).RunAsync();
        }

        private List<VDI> VdisToMove()
        {
           return (from vdi in vdis
                   let moveCmd = new MoveVirtualDiskCommand(Program.MainWindow, vdi)
                   where moveCmd.CanExecute()
                   select vdi).ToList();
        }

        private List<VDI> VdisToMigrate()
        {
            return (from vdi in vdis
                    let cmd = new MigrateVirtualDiskCommand(Program.MainWindow, vdi)
                    where cmd.CanExecute()
                    select vdi).ToList();
        }

        internal override string HelpName
        {
            get { return "VDIMigrateDialog"; }
        }
    }
}
