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

using System.Windows.Forms;
using XenAdmin.Properties;
using XenAPI;

namespace XenAdmin.Controls.DataGridViewEx
{
    public class CollapsingPvsFarmServerDataGridViewRow : DataGridViewExRow
    {
        protected DataGridViewImageCell expansionCell;
        protected DataGridViewTextBoxCell farmCell;
        protected DataGridViewTextBoxCell ipAddressesCell;
        protected DataGridViewTextBoxCell firstPortCell;
        protected DataGridViewTextBoxCell lastPortCell;
        protected bool isPvsFarmRow;

        public CollapsingPvsFarmServerDataGridViewRow(PVS_farm farm)
        {
            Tag = farm;
            isPvsFarmRow = true;
            
            SetupCells();
        }

        public CollapsingPvsFarmServerDataGridViewRow(PVS_server server)
        {
            Tag = server;
            isPvsFarmRow = false;

            SetupCells();
        }

        private void SetupCells()
        {
            expansionCell = new DataGridViewImageCell();
            farmCell = new DataGridViewTextBoxCell();
            ipAddressesCell = new DataGridViewTextBoxCell();
            firstPortCell = new DataGridViewTextBoxCell();
            lastPortCell = new DataGridViewTextBoxCell();

            Cells.Add(expansionCell);
            Cells.Add(farmCell);
            Cells.Add(ipAddressesCell);
            Cells.Add(firstPortCell);
            Cells.Add(lastPortCell);

            UpdateDetails();
        }

        protected void UpdateDetails()
        {
            if (isPvsFarmRow)
            {
                PVS_farm farm = (PVS_farm)Tag;

                if (farm.servers.Count == 0)
                    SetNoIcon();
                else
                    SetCollapseIcon();

                farmCell.Value = farm.name;
            }
            else
            {
                PVS_server server = (PVS_server)Tag;

                SetNoIcon();

                ipAddressesCell.Value = string.Join(", ", server.addresses);
                firstPortCell.Value = server.first_port;
                lastPortCell.Value = server.last_port;
            }
        }

        public bool IsFarmRow
        {
            get { return isPvsFarmRow; }
        }

        public bool IsServerRow
        {
            get { return !isPvsFarmRow; }
        }

        public PVS_farm UnderlyingFarm
        {
            get { return Tag as PVS_farm; }
        }

        public PVS_server UnderlyingServer
        {
            get { return Tag as PVS_server; }
        }

        public void SetCollapseIcon()
        {
            expansionCell.Value = Resources.tree_minus;
        }

        public void SetExpandIcon()
        {
            expansionCell.Value = Resources.tree_plus;
        }

        public void SetNoIcon()
        {
            expansionCell.Value = null;
        }
    }
}
