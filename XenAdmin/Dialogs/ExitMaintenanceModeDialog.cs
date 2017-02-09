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
using System.Windows.Forms;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Dialogs
{
    public partial class RestoreVMsDialog : XenDialogBase
    {
        private List<VM> VMsToRestore;
        private Host TargetHost;
        /// <summary>
        /// A dialog which shows a list of VMs, their current locations and state and asks the user whether they wish to restore them to their original locations.
        /// </summary>
        /// <param name="VMsToRestore">List of VMs that would be restored. Do not pass null or an empty list, this dialog makes no sense otherwise.</param>
        /// <param name="Host">The host which is exiting maintenance mode</param>
        public RestoreVMsDialog(List<VM> VMsToRestore, Host Host)
        {
            InitializeComponent();
            System.Diagnostics.Trace.Assert(VMsToRestore != null && VMsToRestore.Count > 0, "There are no VMs to restore");

            this.VMsToRestore = VMsToRestore;
            TargetHost = Host;
            labelBlurb.Text = String.Format(labelBlurb.Text, Helpers.GetName(Host).Ellipsise(50));
            this.connection = VMsToRestore[0].Connection;

            foreach (VM v in VMsToRestore)
                v.PropertyChanged += v_PropertyChanged;

            BuildList();
        }

        void v_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "power_state" || e.PropertyName == "resident_on" || e.PropertyName == "name_label")
                BuildList();
        }

        private void BuildList()
        {
            List<VMRestoreRow> rows = new List<VMRestoreRow>();
            foreach (VM v in VMsToRestore)
            {
                if (v.resident_on == TargetHost.opaque_ref && v.power_state == vm_power_state.Running)
                    continue;

                rows.Add(new VMRestoreRow(v));
            }
            dataGridViewVms.SuspendLayout();
            try
            {
                dataGridViewVms.Rows.Clear();
                dataGridViewVms.Rows.AddRange(rows.ToArray());
            }
            finally
            {
                dataGridViewVms.ResumeLayout();
            }
        }

        protected class VMRestoreRow : DataGridViewRow
        {
            public VMRestoreRow(VM vm)
            {
                // The image cell, shows the current state of the VM
                DataGridViewImageCell iconCell = new DataGridViewImageCell();
                iconCell.Value = Images.GetImage16For(vm);
                Cells.Add(iconCell);
                
                // The VM name cell
                DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
                nameCell.Value = Helpers.GetName(vm);
                Cells.Add(nameCell);

                // The current location cell
                DataGridViewTextBoxCell locationCell = new DataGridViewTextBoxCell();
                locationCell.Value = Helpers.GetName(vm.Connection.Resolve(vm.resident_on));
                Cells.Add(locationCell);
            }
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void RestoreVMsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (VM v in VMsToRestore)
                v.PropertyChanged -= v_PropertyChanged;
        }
    }
}
