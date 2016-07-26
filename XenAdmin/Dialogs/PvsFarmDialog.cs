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
using System.Linq;
using System.ComponentModel;
using XenAdmin.Network;
using XenAdmin.Controls.DataGridViewEx;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public partial class PvsFarmDialog : XenDialogBase
    {
        /// <summary>
        /// Creates a dialog for viewing the PVS farms and PVS servers on a particular connection.
        /// </summary>
        /// <param name="_connection">May not be null.</param>
        public PvsFarmDialog(IXenConnection _connection)
        {
            System.Diagnostics.Trace.Assert(_connection != null);
            connection = _connection;

            InitializeComponent();
            Text = string.Format(Messages.PVS_FARM_DIALOG_TITLE, this.connection.Name);

            System.Diagnostics.Trace.Assert(gridView.Columns.Count > 0);
            gridView.Columns[0].DefaultCellStyle.NullValue = null;

            RegisterEventHandlers();
            Rebuild();
        }

        private void RegisterEventHandlers()
        {
            UnregisterEventHandlers();
            connection.Cache.RegisterBatchCollectionChanged<PVS_farm>(PvsFarmBatchCollectionChanged);
            connection.Cache.RegisterBatchCollectionChanged<PVS_server>(PvsServerBatchCollectionChanged);
        }

        private void UnregisterEventHandlers()
        {
            connection.Cache.DeregisterBatchCollectionChanged<PVS_farm>(PvsFarmBatchCollectionChanged);
            connection.Cache.DeregisterBatchCollectionChanged<PVS_server>(PvsServerBatchCollectionChanged);
        }

        private void PvsFarmBatchCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, Rebuild);
        }

        private void PvsServerBatchCollectionChanged(object sender, EventArgs e)
        {
            Program.Invoke(this, Rebuild);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            UnregisterEventHandlers();

            base.OnClosing(e);
        }

        private void Rebuild()
        {
            Program.AssertOnEventThread();

            try
            {
                gridView.SuspendLayout();
                gridView.Rows.Clear();

                var pvsFarms = connection.Cache.PVS_farms.ToList();
                pvsFarms.Sort();

                foreach (var pvsFarm in pvsFarms)
                {
                    var farmRow = new CollapsingPvsFarmServerDataGridViewRow(pvsFarm);
                    gridView.Rows.Add(farmRow);

                    foreach (var pvsServer in connection.ResolveAll(pvsFarm.servers))
                    {
                        var serverRow = new CollapsingPvsFarmServerDataGridViewRow(pvsServer);
                        gridView.Rows.Add(serverRow);
                    }
                }
            }
            finally
            {
                gridView.ResumeLayout();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
