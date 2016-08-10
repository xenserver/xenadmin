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

using System.ComponentModel;
using System.Windows.Forms;

namespace XenAdmin.Controls.DataGridViewEx
{
    public partial class CollapsingPvsSiteServerDataGridView : DataGridViewEx
    {
        public CollapsingPvsSiteServerDataGridView()
        {
            InitializeComponent();
        }

        public CollapsingPvsSiteServerDataGridView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public bool Updating { get; set; }

        private const int expansionColumnIndex = 0;

        protected override void OnCellContentClick(DataGridViewCellEventArgs e)
        {
            base.OnCellContentClick(e);

            if (e.ColumnIndex == expansionColumnIndex)
                ExpandCollapseClicked(e.RowIndex);
        }

        public void ExpandCollapseClicked(int rowIndex)
        {
            var siteRow = (CollapsingPvsSiteServerDataGridViewRow)Rows[rowIndex];

            if (siteRow.UnderlyingSite != null)
            {
                for (int i = rowIndex + 1; i < Rows.Count; i++)
                {
                    var row = (CollapsingPvsSiteServerDataGridViewRow)Rows[i];

                    if (row.IsSiteRow)
                        break;

                    row.Visible = !row.Visible;

                    if (row.Visible)
                        siteRow.SetCollapseIcon();
                    else
                        siteRow.SetExpandIcon();
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (CurrentCell != null && CurrentCell.ColumnIndex == expansionColumnIndex)
            {
                if (e.KeyCode == Keys.Space)
                {
                    ExpandCollapseClicked(CurrentCell.RowIndex);
                    e.Handled = true;
                }
            }
            base.OnKeyDown(e);
        }
    }
}
