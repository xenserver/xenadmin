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
using System.Windows.Forms;
using XenAdmin.Alerts;
using XenAPI;
using XenAdmin.Properties;
using System.ComponentModel;
using XenAdmin.Actions;
using System.Collections.Generic;
using XenAdmin.Core;


namespace XenAdmin.Dialogs.VMProtectionRecovery
{
    public partial class PolicyHistory : UserControl
    {
        public Pool pool;

        public PolicyHistory()
        {
            InitializeComponent();
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            ColumnExpand.DefaultCellStyle.NullValue = null;
            comboBox1.SelectedIndex = 0;
            dataGridView1.Columns[2].ValueType = typeof(DateTime);
            dataGridView1.Columns[2].DefaultCellStyle.Format = Messages.DATEFORMAT_DMY_HM;
        }



        void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                HistoryRow row = (HistoryRow)dataGridView1.Rows[e.RowIndex];
                if (row.Alert.Type != "info")
                {
                    row.Expanded = !row.Expanded;
                    row.RefreshRow();
                }
            }
        }

        private IVMPolicy _policy;
        public void StartRefreshTab()
        {
            /* hoursFromNow has 3 possible values:
                1) 0 -> top 10 messages (default)
                2) 24 -> messages from past 24 Hrs
                3) 7 * 24 -> messages from lst 7 days */

            var hoursFromNow = 0;
            switch (comboBox1.SelectedIndex)
            {
                case 0: /* default value*/
                    break;
                case 1:
                    hoursFromNow = 24;
                    break;
                case 2:
                    hoursFromNow = 7 * 24;
                    break;
            }

            PureAsyncAction action = _policy.getAlertsAction(_policy, hoursFromNow);
            action.Completed += action_Completed;
            action.RunAsync();
        }

        public void RefreshTab(IVMPolicy policy)
        {
            _policy = policy;
            if (_policy == null)
            {
                labelHistory.Text = "";
                comboBox1.Enabled = false;
            }
            else
            {
                comboBox1.Enabled = true;
                StartRefreshTab();
            }

        }

        private void RefreshGrid(List<PolicyAlert> alerts)
        {
            if (_policy != null)
            {
                ReloadHistoryLabel();
                dataGridView1.Rows.Clear();
                var readOnlyAlerts = alerts.AsReadOnly();
                foreach (var alert in readOnlyAlerts)
                {
                    dataGridView1.Rows.Add(new HistoryRow(alert));
                }
                dataGridView1.Sort(ColumnDateTime, System.ComponentModel.ListSortDirection.Descending);
            }
        }



        public class HistoryRow : DataGridViewRow
        {
            private DataGridViewImageCell _expand = new DataGridViewImageCell();
            private DataGridViewTextAndImageCell _result = new DataGridViewTextAndImageCell();
            private DataGridViewTextBoxCell _dateTime = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _description = new DataGridViewTextBoxCell();
            public readonly PolicyAlert Alert;

            public HistoryRow(PolicyAlert alert)
            {
                Alert = alert;
                Cells.Add(_expand);
                _expand.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                Cells.Add(_result);
                Cells.Add(_dateTime);
                Cells.Add(_description);

                RefreshRow();

            }
            [DefaultValue(false)]
            public bool Expanded { get; set; }
            public void RefreshRow()
            {

                _expand.Value = Expanded ? Resources.expanded_triangle : Resources.contracted_triangle;
                if (Alert.Type == "info")
                    _expand.Value = null;

                if (Alert.Type == "error")
                {
                    _result.Image = Properties.Resources._075_WarningRound_h32bit_16;
                    _result.Value = Messages.ERROR;
                }
                else if (Alert.Type == "warn")
                {
                    _result.Image = Properties.Resources._075_WarningRound_h32bit_16;
                    _result.Value = Messages.WARNING;
                }
                else if (Alert.Type == "info")
                {
                    _result.Image = Properties.Resources._075_TickRound_h32bit_16;
                    _result.Value = Messages.INFORMATION;
                }
                _dateTime.Value = Alert.Time;
                if (Alert.Type == "error")
                    _description.Value = Expanded ? string.Format("{0}\r\n{1}", Alert.ShortFormatBody, Alert.Text) : Alert.ShortFormatBody.Ellipsise(80);
                else
                    _description.Value = Expanded ? Alert.Text : Alert.ShortFormatBody.Ellipsise(90);
            }

        }


        public void Clear()
        {
            dataGridView1.Rows.Clear();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_policy != null)
            {
               StartRefreshTab();  
            }
        }

        void action_Completed(ActionBase sender)
        {
            var action = sender;
            Program.Invoke(Program.MainWindow, () =>
            {
                panelLoading.Visible = false;
                if (_policy._Type == typeof(VMPP))
                {
                    RefreshGrid(((GetVMPPAlertsAction)(action)).VMPP.Alerts);
                }
                else
                {
                    RefreshGrid(((GetVMSSAlertsAction)(action)).VMSS.Alerts);
                }
            });
        }

        private void ReloadHistoryLabel()
        {
            string Name;
            Name = _policy.Name;
            // ellipsise if necessary
            using (System.Drawing.Graphics g = labelHistory.CreateGraphics())
            {
                int maxWidth = label1.Left - labelHistory.Left;
                int availableWidth = maxWidth - (int)g.MeasureString(string.Format(Messages.HISTORY_FOR_POLICY, ""), labelHistory.Font).Width;
                Name = Name.Ellipsise(new System.Drawing.Rectangle(0, 0, availableWidth, labelHistory.Height), labelHistory.Font);
            }
            labelHistory.Text = string.Format(Messages.HISTORY_FOR_POLICY, Name);
        }
    }
}
