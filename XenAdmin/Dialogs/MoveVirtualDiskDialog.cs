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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Network;
using XenAPI;
using XenAdmin.Core;
using XenAdmin.Actions;

namespace XenAdmin.Dialogs
{
    public partial class MoveVirtualDiskDialog : XenDialogBase
    {
        private VDI vdi;

        /// <summary>
        /// Designer support only. Do not use.
        /// </summary>
        public MoveVirtualDiskDialog()
        {
            InitializeComponent();
        }

        protected MoveVirtualDiskDialog(IXenConnection connection): base(connection)
        {
            InitializeComponent();
        }

        public MoveVirtualDiskDialog(VDI vdi) : this(vdi.Connection)
        {
            this.vdi = vdi;
            srPicker1.SetUsageAsMovingVDI(new[] {vdi});
            srPicker1.SrHint.Visible = false;
            srPicker1.Connection = vdi.Connection;
            srPicker1.DiskSize = vdi.virtual_size;
            srPicker1.srListBox.Invalidate();
            srPicker1.selectDefaultSROrAny();
            SetupEventHandlers();
        }

        protected void SetupEventHandlers()
        {
            SRPicker.ItemSelectionNotNull += srPicker1_ItemSelectionNotNull;
            SRPicker.ItemSelectionNull += srPicker1_ItemSelectionNull;
        }

        protected SrPicker SRPicker
        {
            get { return srPicker1; }
        }

        private void srPicker1_ItemSelectionNull(object sender, EventArgs e)
        {
            updateButtons();
        }

        private void srPicker1_ItemSelectionNotNull(object sender, EventArgs e)
        {
            updateButtons();
        }

        private void updateButtons()
        {
            buttonMove.Enabled = srPicker1.SR != null;
        }

        protected virtual void buttonMove_Click(object sender, EventArgs e)
        {
            MoveVirtualDiskAction action = new MoveVirtualDiskAction(connection, vdi, srPicker1.SR);
            action.RunAsync();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
