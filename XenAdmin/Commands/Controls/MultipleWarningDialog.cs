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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Dialogs;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Properties;

namespace XenAdmin.Commands.Controls
{
    public partial class MultipleWarningDialog : XenDialogBase
    {
        /*
         * This class is for displaying multiple warning messages. It's principal use was for displaying multiple
         * different warning messages in a multiple select scenario.
         * 
         * We should monitor it's usefulness, it could be a good utility for displaying multiple messages in other situations.
         */

        private List<Warning> Warnings = new List<Warning>();

        /// <summary>
        /// This constructor is only for use with the designer and as a base for the other constructors. Use a different overload.
        /// </summary>
        public MultipleWarningDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// With this constructor you get to specify a particular keyword for the accept button
        /// as well as the title and main message.
        /// </summary>
        /// <param name="TitleMessage">The title of the dialog</param>
        /// <param name="MainMessage">A message informing the user what they are doing (e.g. You are about to delete multiple VDIs)</param>
        /// <param name="AcceptButtonLabel">The keyword from your instruction message for the accept button (e.g. &Proceed)</param>
        public MultipleWarningDialog(string TitleMessage, string MainMessage, string AcceptButtonLabel) 
            : this (TitleMessage, MainMessage)
        {
            buttonProceed.Text = AcceptButtonLabel;
        }

        /// <summary>
        /// With this constructor you get the default 'Do you wish to proceed?' instruction message.
        /// </summary>
        /// <param name="TitleMessage">The title of the dialog</param>
        /// <param name="MainMessage">A message informing the user what they are doing (e.g. You are about to delete multiple VDIs)</param>
        public MultipleWarningDialog(string TitleMessage, string MainMessage) 
            : this ()
        {
            Text = TitleMessage;
            labelMessage.Text = MainMessage;
        }

        public void AddWarningMessage(string ActionTitle, string WarningMessage, List<IXenObject> AppliesTo)
        {
            Warnings.Add(new Warning(ActionTitle, WarningMessage, AppliesTo));
        }

        private void BuildWarningList()
        {
            dataGridViewWarnings.SuspendLayout();
            try
            {
                dataGridViewWarnings.Rows.Clear();
                foreach (Warning w in Warnings)
                {
                    DataGridViewRow r = w.GetWarningRow();
                    r.Tag = w;
                    dataGridViewWarnings.Rows.Add(r);
                }
            }
            finally
            {
                dataGridViewWarnings.ResumeLayout();
            }
        }

        private void BuildAppliesToList()
        {
            Warning w = null;
            if (dataGridViewWarnings.SelectedRows.Count > 0)
            {
                w = dataGridViewWarnings.SelectedRows[0].Tag as Warning;
            }
            else if (dataGridViewWarnings.Rows.Count > 0)
            {
                w = dataGridViewWarnings.Rows[0].Tag as Warning;
            }
            else
            {
                // I don't think you should be here. Warning dialog with no warnings is unlikely.
                dataGridViewAppliesTo.Rows.Clear();
                return;
            }
            dataGridViewAppliesTo.SuspendLayout();
            try
            {
                dataGridViewAppliesTo.Rows.Clear();
                dataGridViewAppliesTo.Rows.AddRange(w.GetAppliesToRows().ToArray());
                dataGridViewAppliesTo.Columns[1].HeaderText = w.ActionTitle;
            }
            finally
            {
                dataGridViewAppliesTo.ResumeLayout();
            }
        }

        private void buttonProceed_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected class Warning
        {
            public List<IXenObject> AppliesTo = new List<IXenObject>();
            public string WarningMessage;
            public string ActionTitle;

            public Warning(string ActionTitle, string WarningMessage, List<IXenObject> AppliesTo)
            {
                this.ActionTitle = ActionTitle;
                this.WarningMessage = WarningMessage;
                this.AppliesTo = AppliesTo;
            }

            public DataGridViewRow GetWarningRow()
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewImageCell warningImageCell = new DataGridViewImageCell();
                warningImageCell.Value = Images.StaticImages._000_Alert2_h32bit_16;

                DataGridViewTextBoxCell warningCell = new DataGridViewTextBoxCell();
                warningCell.Value = string.Format("{0}\n\r\n\r{1}", ActionTitle, WarningMessage);

                row.Cells.Add(warningImageCell);
                row.Cells.Add(warningCell);
                return row;
            }

            public List<DataGridViewRow> GetAppliesToRows()
            {
                List<DataGridViewRow> rows = new List<DataGridViewRow>();
                foreach (IXenObject o in AppliesTo)
                {
                    DataGridViewRow r = new DataGridViewRow();
                    DataGridViewImageCell typeImageCell = new DataGridViewImageCell();
                    typeImageCell.Value = Images.GetImage16For(o);
                    DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
                    nameCell.Value = Helpers.GetName(o);
                    r.Cells.Add(typeImageCell);
                    r.Cells.Add(nameCell);
                    rows.Add(r);
                }
                return rows;
            }
        }

        private void MultipleWarningDialog_Shown(object sender, EventArgs e)
        {
            BuildWarningList();
            BuildAppliesToList();
            dataGridViewWarnings.SelectionChanged += new EventHandler(dataGridViewWarnings_SelectionChanged);
        }

        void dataGridViewWarnings_SelectionChanged(object sender, EventArgs e)
        {
            BuildAppliesToList();
        }
    }
}
