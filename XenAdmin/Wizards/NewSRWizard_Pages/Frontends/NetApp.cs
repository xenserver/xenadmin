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
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Help;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class NetApp : XenTabPage
    {
        #region Private fields
        private const string TARGET = "target";
        private const string AGGREGATE = "aggregate";
        private const string FLEXVOLS = "FlexVols";
        private const string ASIS = "asis";
        private const string PORT = "controlport";
        private const string USEHTTPS = "usehttps";

        private const string ALLOCATION = "allocation";
        private const string THICK = "thick";
        private const string THIN = "thin";

        private bool m_allowToCreateNewSr;
        private string m_uuid;
        private string m_hostAddress;
        #endregion

        public NetApp()
        {
            InitializeComponent();
        }

        #region Accessors

        public SrScanAction SrScanAction { private get; set; }

        public string UUID
        {
            get
            {
                if (radioButtonNew.Checked)
                    return null;

                SR.SRInfo sr = listBoxSRs.SelectedItem as SR.SRInfo;
                if (sr == null)
                    return null;

                return sr.UUID;
            }
        }

        public string SrDescription
        {
            get
            {
                if (radioButtonReattach.Checked)// We're attaching an existing NetApp SR
                {
                    SR.SRInfo sr = listBoxSRs.SelectedItem as SR.SRInfo;
                    if (sr != null)
                        return string.Format(Messages.NEWSR_NETAPP_DESCRIPTION, m_hostAddress, sr.Aggr);
                }
                else
                {
                    NetAppAggregateRow selected = SelectedDataGridViewRow;
                    if (selected != null)
                        return string.Format(Messages.NEWSR_NETAPP_DESCRIPTION, m_hostAddress, selected.aggregate.Name);
                }
                return null;
            }
        }

        public Dictionary<String, String> DeviceConfigParts
        {
            get
            {
                var dconf = new Dictionary<string, string>();

                if (!radioButtonReattach.Checked)
                {
                    // only set on new SRs
                    if (checkBoxDedup.Enabled)
                        dconf[ASIS] = checkBoxDedup.Checked ? "true" : "false";

                    NetAppAggregateRow selected = SelectedDataGridViewRow;
                    if (selected != null)
                        dconf[AGGREGATE] = SelectedDataGridViewRow.aggregate.Name;

                    // Ensure integral value
                    dconf[FLEXVOLS] = ((long)nudFlexvols.Value).ToString();
                    // Set full or sparse provisioning (CA-10497) - only set on new SRs
                    dconf[ALLOCATION] = checkBoxThin.Checked ? THIN : THICK;
                }

                dconf[PORT] = textBoxPort.Text;
                dconf[USEHTTPS] = checkBoxHttps.Checked ? "true" : "false";

                return dconf;
            }
        }

        public SrWizardType SrWizardType
        {
            set
            {
                m_allowToCreateNewSr = value.AllowToCreateNewSr;
                m_uuid = value.UUID;
                m_hostAddress = value.DeviceConfig[TARGET];
            }
        }

        #endregion

        /// <summary>
        /// Enables/disables the 'enable A-SIS deduplication' check box and associated tooltip.
        /// </summary>
        private void EnableAsisCheckBox()
        {
            if (radioButtonReattach.Checked)
            {
                checkBoxDedup.Enabled = false;
                toolTipContainerDedup.SuppressTooltip = true;
            }
            else if (SelectedDataGridViewRow == null)
            {
                //Suppress tooltip if no item is selected
                toolTipContainerDedup.SuppressTooltip = true;
            }
            else
            {
                // Enable ASIS checkbox if ASIS is enabled on the selected aggregate
                checkBoxDedup.Enabled = checkBoxThin.Checked
                    && (SelectedDataGridViewRow.aggregate).AsisCapable;
                toolTipContainerDedup.SuppressTooltip = checkBoxDedup.Enabled;
            }
        }

        private NetAppAggregateRow SelectedDataGridViewRow
        {
            get
            {
                if (dataGridView1.SelectedRows == null || dataGridView1.SelectedRows.Count == 0)
                    return null;

                return dataGridView1.SelectedRows[0] as NetAppAggregateRow;
            }
        }

        #region XenTabPage overrides

        public override void PopulatePage()
        {
            if (m_uuid != null)
                listBoxSRs.SetMustSelectUUID(m_uuid);

            if (!m_allowToCreateNewSr)
            {
                radioButtonNew.Enabled = false;
                dataGridView1.Enabled = false;
                labelFlexvols.Enabled = false;
                nudFlexvols.Enabled = false;
                helplinkFlexvols.Enabled = false;
                checkBoxThin.Enabled = false;
                checkBoxDedup.Enabled = false;
            }

            // Add tooltip that alerts user why NetApp A-SIS is disabled
            toolTipContainerDedup.SetToolTip(Messages.NEWSR_NETAPP_DEDUP_UNAVAILABLE);
            // Suppress tooltip until an item is selected in the NetApp aggregate dataGridView
            toolTipContainerDedup.SuppressTooltip = true;

            //Fill aggregates list
            dataGridView1.Rows.Clear();
            List<NetAppAggregate> netappAggregates = SrScanAction.Aggregates;
            foreach (NetAppAggregate aggr in netappAggregates)
                dataGridView1.Rows.Add(new NetAppAggregateRow(aggr));

            // Fill SR list
            listBoxSRs.Items.Clear();
            List<SR.SRInfo> SRs = SrScanAction.SRs;
            foreach (SR.SRInfo sr in SRs)
                listBoxSRs.Items.Add(sr);

            if (m_uuid != null)
                radioButtonReattach.Checked = true;
            else if (radioButtonNew.Enabled)
                radioButtonNew.Checked = true;
            else
                radioButtonReattach.Checked = true;

            OnPageUpdated();
        }

        public override string PageTitle { get { return Messages.NETAPP_EQUAL_PAGE_TITLE; } }

        public override string Text { get { return Messages.SETTINGS; } }

        public override string HelpID { get { return "SL_NETAPP_options"; } }

        public override bool EnableNext()
        {
            return ((radioButtonReattach.Checked && listBoxSRs.SelectedIndex > -1)
                    || (!radioButtonReattach.Checked && dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Index > -1));
        }

        #endregion

        #region Event handlers

        private void listBoxSRs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxSRs.SelectedItems.Count > 0 && !radioButtonReattach.Checked)
                radioButtonReattach.Checked = true;
            OnPageUpdated();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            labelFlexvols.Enabled = nudFlexvols.Enabled = helplinkFlexvols.Enabled = checkBoxThin.Enabled = !radioButtonReattach.Checked;
            EnableAsisCheckBox();
            if (!radioButtonNew.Checked)
                dataGridView1.ClearSelection();
            OnPageUpdated();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && !radioButtonNew.Checked)
                radioButtonNew.Checked = true;
            OnPageUpdated();
        }

        private void helplinkFlexvols_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HelpManager.Launch("NewSRWizard_FlexVols");
        }

        private void checkBoxHttps_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPort.Text = checkBoxHttps.Checked ? "" + 443 : "" + 80;
        }

        private bool blockNextPress;
        private void textBoxPort_KeyDown(object sender, KeyEventArgs e)
        {
            // Example code from microsoft that happened to do what we wanted
            // http://msdn.microsoft.com/en-us/library/system.windows.forms.control.keydown.aspx

            blockNextPress = false;
            // Determine whether the keystroke is a number from the top of the keyboard.
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)
            {
                // Determine whether the keystroke is a number from the keypad.
                if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)
                {
                    // Determine whether the keystroke is a backspace.
                    if (e.KeyCode != Keys.Back)
                    {
                        // A non-numerical keystroke was pressed.
                        // Set the flag to true and evaluate in KeyPress event.
                        blockNextPress = true;
                    }
                }
            }
            //If shift key was pressed, it's not a number.
            if (Control.ModifierKeys == Keys.Shift)
            {
                blockNextPress = true;
            }
        }

        private void textBoxPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (blockNextPress)
                e.Handled = true;
        }

        private void checkBoxThin_CheckedChanged(object sender, EventArgs e)
        {
            EnableAsisCheckBox();
        }

        #endregion

        private class NetAppAggregateRow : DataGridViewRow
        {
            public NetAppAggregate aggregate;

            public NetAppAggregateRow(NetAppAggregate aggr)
            {
                aggregate = aggr;
                for (int i = 0; i < 5; i++)
                {
                    Cells.Add(new DataGridViewTextBoxCell());
                    UpdateCell(i);
                }
            }

            private void UpdateCell(int index)
            {
                switch (index)
                {
                    case 0: Cells[0].Value = aggregate.Name; break;
                    case 1: Cells[1].Value = Util.DiskSizeString(aggregate.Size); break;
                    case 2: Cells[2].Value = aggregate.Disks.ToString(); break;
                    case 3: Cells[3].Value = aggregate.RaidType; break;
                    case 4: Cells[4].Value = aggregate.AsisCapable ? Messages.YES : Messages.NO; break;
                }
            }
        }
    }
}

