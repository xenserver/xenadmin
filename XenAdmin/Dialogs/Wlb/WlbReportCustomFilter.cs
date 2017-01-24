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
using System.Globalization;
using System.Drawing.Drawing2D;

using XenAdmin.Wlb;
using XenAdmin.Model;
using XenAdmin.Network;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Dialogs.Wlb
{
    public partial class WlbReportCustomFilter : XenAdmin.Dialogs.XenDialogBase
    {

        #region Private Fields
        
        private IXenConnection _connection;
        private Host[] _hosts;
        private VM[] _vms;
        private string[] _tags;
        private List<string> _filters;
        private WlbReportCustomFilterType _filterType;
        private Dictionary<string, List<IXenObject>> _tagDictionary;
                
        #endregion


        #region Public Properties

        public enum WlbReportCustomFilterType
        {
            host,
            vm,
            tags
        }
        #endregion


        #region Public Methods
        public List<string> GetSelectedFilters()
        {
            return _filters;
        }

        #endregion


        #region Constructors
        public WlbReportCustomFilter(IXenConnection connection)
        {
            InitializeComponent();

            this.listViewFilterItem.Columns.Clear();
            this.listViewFilterItem.Columns.Add(String.Empty, 25);
            this.listViewFilterItem.Columns.Add(Messages.WLB_REPORT_VIEW_TAG_COLUMN_HEADER, 250);

            this.panelListView.Enabled = false;
            this.panelListView.Visible = false;
            this.panelTags.Enabled = false;
            this.panelTags.Visible = false;

            _connection = connection;
        }

        public void InitializeFilterTypeIndex()
        {
            this.comboFilterType.SelectedIndex = 0;
        }

        #endregion


        #region Private Methods

        private void PopulateListViewFilterItem()
        {
            this.panelListView.Enabled = false;
            this.panelListView.Visible = false;
            this.panelTags.Enabled = true;
            this.panelTags.Visible = true;
            this.comboBoxTags.Items.Clear();

            if (_connection != null)
            {
                _hosts = _connection.Cache.Hosts;
                _vms = _connection.Cache.VMs;
                _tags = Tags.GetAllTags();
            }

            switch(_filterType)
            {
                case WlbReportCustomFilterType.host:

                    this.labelHost.Enabled = true;
                    this.labelHost.Visible = true;
                    this.labelTags.Enabled = false;
                    this.labelTags.Visible = false;

                    foreach(Host host in _hosts)
                    {
                        this.comboBoxTags.Items.Add(host.Name);
                    }
                    this.comboBoxTags.SelectedIndex = 0;
                    break;

                case WlbReportCustomFilterType.vm:

                    this.panelListView.Visible = true;
                    this.panelListView.Enabled = true;
                    this.panelTags.Enabled = false;
                    this.panelTags.Visible = false;
                    this.listViewFilterItem.Items.Clear();

                    foreach (VM vm in _vms)
                    {
                        ListViewItem lvItem = new ListViewItem();
                        lvItem.Tag = vm;
                        lvItem.SubItems.Add(vm.Name);
                        if (this.checkBoxCheckAll.Checked)
                        {
                            lvItem.Checked = true;
                        }
                        this.listViewFilterItem.Items.Add(lvItem);
                    }
                    break;

                case WlbReportCustomFilterType.tags:

                    this.labelHost.Enabled = false;
                    this.labelHost.Visible = false;
                    this.labelTags.Enabled = true;
                    this.labelTags.Visible = true;

                    _tagDictionary = new Dictionary<string, List<IXenObject>>();
                    foreach(string tag in _tags)
                    {
                        // Disable invalid tags that contains vms and hosts
                        List<IXenObject> tagUsers = Tags.Users(tag);
                        bool hasHost = false;
                        bool hasVM = false;
                        foreach (IXenObject tagUser in tagUsers)
                        {
                            if (tagUser is VM)
                            {
                                hasVM = true;
                            }
                            if(tagUser is Host)
                            {
                                hasHost = true;
                            }
                        }

                        if (!(hasHost && hasVM))
                        {
                            _tagDictionary.Add(tag, tagUsers);
                            this.comboBoxTags.Items.Add(tag);
                        }
                    }
                    this.comboBoxTags.SelectedIndex = 0;
                    break;
            }
        }

        #endregion


        #region Control Event Handlers
        
        private void listViewFilterItem_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listViewFilterItem_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if ((WlbReportCustomFilterType)this.comboFilterType.SelectedIndex == WlbReportCustomFilterType.vm)
            {
                if (e.Item.Checked)
                    ControlPaint.DrawCheckBox(e.Graphics, e.Bounds.Left + 3, e.Bounds.Top + 1, 15, 15, ButtonState.Checked);
                else
                    ControlPaint.DrawCheckBox(e.Graphics, e.Bounds.Left + 3, e.Bounds.Top + 1, 15, 15, ButtonState.Normal);
            }
        }

        private void listViewFilterItem_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawText();
        }

        private void comboFilterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            _filterType = (WlbReportCustomFilterType)this.comboFilterType.SelectedIndex;
            
            PopulateListViewFilterItem();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _filters = new List<string>();

            if ((WlbReportCustomFilterType)this.comboFilterType.SelectedIndex == WlbReportCustomFilterType.tags)
            {
                List<IXenObject> objLst = new List<IXenObject>();
                _tagDictionary.TryGetValue(this.comboBoxTags.SelectedItem.ToString(), out objLst);
                
                foreach (IXenObject obj in objLst)
                {
                    if (obj is VM)
                    {
                        _filters.Add(((VM)obj).uuid);
                    }
                }
            }
            else if ((WlbReportCustomFilterType)this.comboFilterType.SelectedIndex == WlbReportCustomFilterType.host)
            {
                Object host = _hosts.GetValue(this.comboBoxTags.SelectedIndex);
                List<VM> vms = ((Host)host).Connection.ResolveAll(((Host)host).resident_VMs);
                
                foreach (VM vm in vms)
                {
                    _filters.Add(vm.uuid);
                }
            }
            else
            {
                foreach (ListViewItem item in this.listViewFilterItem.Items)
                {
                    if (item.Checked)
                    {
                        /*if (item.Tag is Host)
                        {
                            _filters.Add(((Host)item.Tag).uuid);
                        }
                        else */ if (item.Tag is VM)
                        {
                            _filters.Add(((VM)item.Tag).uuid);
                        }
                    }
                }
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void checkBoxCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listViewFilterItem.Items)
            {
                if (this.checkBoxCheckAll.Checked)
                {
                    item.Checked = true;
                }
                else
                {
                    item.Checked = false;
                }
            }
        }
        
        #endregion
    }
}