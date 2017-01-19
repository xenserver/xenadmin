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


namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class EqualLogic : XenTabPage
    {
        private bool m_allowToCreateNewSr;
        private string m_uuid;
        private string m_hostAddress;

        private const string TARGET = "target";
        private const string STORAGE_POOL = "storagepool";
        private const string ALLOCATION = "allocation";
        private const string THICK = "thick";
        private const string THIN = "thin";

        public EqualLogic()
        {
            InitializeComponent();
        }

        #region Accessors

        public SrScanAction SrScanAction { private get; set; }
        
        public Dictionary<string, string> DeviceConfigParts
        {
            get
            {
                var dconf =new Dictionary<string, string>();

                if (!radioButtonReattach.Checked)
                {
                    DellStoragePoolRow selected = SelectedDataGridViewRow;
                    if (selected != null)
                        dconf[STORAGE_POOL] = selected.StoragePool.Name;
                    dconf[ALLOCATION] = ThinProvisioningCheckBox.Checked ? THIN : THICK;
                }

                return dconf;
            }
        }

        public string SrDescription
        {
            get
            {
                if (radioButtonReattach.Checked)// We're attaching an existing SR
                {
                    SR.SRInfo sr = listBoxSRs.SelectedItem as SR.SRInfo;
                    if (sr != null)
                        return string.Format(Messages.NEWSR_EQUAL_LOGIC_DESCRIPTION, m_hostAddress, m_hostAddress);
                }
                else
                {
                    DellStoragePoolRow selected = SelectedDataGridViewRow;
                    if (selected != null)
                        return string.Format(Messages.NEWSR_EQUAL_LOGIC_DESCRIPTION, m_hostAddress, selected.StoragePool.Name);
                }
                return null;
            }
        }

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

        private DellStoragePoolRow SelectedDataGridViewRow
        {
            get
            {
                if (dataGridView1.SelectedRows == null || dataGridView1.SelectedRows.Count == 0)
                    return null;

                return dataGridView1.SelectedRows[0] as DellStoragePoolRow;
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
                ThinProvisioningCheckBox.Enabled = false;
            }

            //fill storage pools list
            dataGridView1.Rows.Clear();

            List<DellStoragePool> pools = SrScanAction.StoragePools;
            foreach (DellStoragePool pool in pools)
                dataGridView1.Rows.Add(new DellStoragePoolRow(pool));

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

        public override string HelpID { get { return "SL_EQL_options"; } }

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

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && !radioButtonNew.Checked)
                radioButtonNew.Checked = true;
            OnPageUpdated();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            ThinProvisioningCheckBox.Enabled = radioButtonNew.Checked;
            if (!radioButtonNew.Checked)
                dataGridView1.ClearSelection();
            OnPageUpdated();
        }

        #endregion

        private class DellStoragePoolRow : DataGridViewRow
        {
            public DellStoragePool StoragePool;

            public DellStoragePoolRow(DellStoragePool StoragePool)
            {
                this.StoragePool = StoragePool;
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
                    case 0: Cells[0].Value = StoragePool.Name; break;
                    case 1: Cells[1].Value = Util.DiskSizeString(StoragePool.Capacity); break;
                    case 2: Cells[2].Value = Util.DiskSizeString(StoragePool.Freespace); break;
                    case 3: Cells[3].Value = StoragePool.Volumes.ToString(); break;
                    case 4: Cells[4].Value = StoragePool.Members.ToString(); break;
                }
            }
        }
    }
}

