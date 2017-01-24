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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Dialogs;
using XenAPI;

namespace XenAdmin.Wizards.HAWizard_Pages
{
    public partial class ChooseSR : XenTabPage
    {
        public ChooseSR()
        {
            InitializeComponent();
        }

        private Pool pool;
        public Pool Pool
        {
            set
            {
                this.pool = value;
            }
        }

        private SR selectedHeartbeatSR;
        public SR SelectedHeartbeatSR
        {
            get
            {
                return selectedHeartbeatSR;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whether any SRs suitable for heartbeating were found.</returns>
        internal bool ScanForHeartbeatSRs()
        {
            System.Diagnostics.Trace.Assert(pool != null);

            // Start action and show progress with a dialog
            GetHeartbeatSRsAction action = new GetHeartbeatSRsAction(pool);
            using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Blocks))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);
            }
            if (action.Cancelled || action.Cancelling || !action.IsCompleted)
                return false;

            if (!action.Succeeded || action.SRs.Count == 0)
            {
                dataGridViewExSRs.Enabled = false;
                return true;
            }

            dataGridViewExSRs.Enabled = true;

            List<SRWrapper> srs = action.SRs;
            srs.Sort((a, b) => a.enabled != b.enabled
                                   ? b.enabled.CompareTo(a.enabled)
                                   : (a.sr.NameWithoutHost.CompareTo(b.sr.NameWithoutHost)));

            dataGridViewExSRs.Rows.Clear();

            foreach (SRWrapper srWrapper in srs)
            {
                var row = new StorageRow(srWrapper);
                dataGridViewExSRs.Rows.Add(row);
                row.Enabled = srWrapper.enabled;

                // coming forward to this page after having already been through it once
                if (selectedHeartbeatSR != null && srWrapper.sr.opaque_ref == selectedHeartbeatSR.opaque_ref)
                {

                    if (srWrapper.enabled)
                        row.Selected = true;
                    else
                    {
                        selectedHeartbeatSR = null;
                        OnPageUpdated();
                    }
                }
            }

            return true;
        }

        public override string Text { get { return Messages.HA_CHOOSESR_PAGE_TEXT; } }

        public override string PageTitle { get { return Messages.HA_CHOOSESR_PAGE_PAGETITLE; } }

        public override bool EnableNext()
        {
            return selectedHeartbeatSR != null;
        }

        private void dataGridViewExSRs_Paint(object sender, PaintEventArgs e)
        {
            if (!dataGridViewExSRs.Enabled)
            {
                TextRenderer.DrawText(e.Graphics, Messages.HA_NO_SHARED_SRS, dataGridViewExSRs.Font,
                                      dataGridViewExSRs.ClientRectangle, SystemColors.WindowText,
                                      TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.WordBreak);
            }
        }

        private void dataGridViewExSRs_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewExSRs.SelectedRows.Count == 1)
            {
                SRWrapper srw = ((StorageRow)dataGridViewExSRs.SelectedRows[0]).SrWrapper;
                selectedHeartbeatSR = srw.enabled ? srw.sr : null;
            }
            else
                selectedHeartbeatSR = null;

            OnPageUpdated();
        }

        private class StorageRow : DataGridViewExRow
        {
            private DataGridViewImageCell cellImage;
            private DataGridViewTextBoxCell cellSr;
            private DataGridViewTextBoxCell cellDescription;
            private DataGridViewTextBoxCell cellComment;

            public SRWrapper SrWrapper { get; private set; }

            public StorageRow(SRWrapper srw)
            {
                SrWrapper = srw;

                cellImage = new DataGridViewImageCell(false)
                                {
                                    ValueType = typeof(Image),
                                    Value =  Images.GetImage16For(srw.sr)
                                };
                cellSr = new DataGridViewTextBoxCell {Value = srw.sr.NameWithoutHost};
                cellDescription = new DataGridViewTextBoxCell {Value = srw.sr.Description};
                cellComment = new DataGridViewTextBoxCell {Value = srw.ReasonUnsuitable};

                Cells.AddRange(new DataGridViewCell[] {cellImage, cellSr, cellDescription, cellComment});
            }
        }
    }
}
