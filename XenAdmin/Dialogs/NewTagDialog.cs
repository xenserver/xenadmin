﻿/* Copyright (c) Citrix Systems, Inc. 
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
using XenAdmin.Model;


namespace XenAdmin.Dialogs
{
    public partial class NewTagDialog : XenDialogBase
    {
        public NewTagDialog(List<string> tags)
            : this(tags, new List<string>())
        {
        }

        public NewTagDialog(List<string> tags, List<string> indeterminateTags)
        {
            InitializeComponent();
            LoadTags(tags, indeterminateTags);
        }

        public List<string> GetSelectedTags()
        {
            return GetTags(CheckState.Checked);
        }

        public List<string> GetIndeterminateTags()
        {
            return GetTags(CheckState.Indeterminate);
        }

        private List<string> GetTags(CheckState checkState)
        {
            Program.AssertOnEventThread();
            List<string> items = new List<string>();
            foreach (TagsDataGridViewRow item in tagsDataGrid.Rows)
            {
                if (item.Checked == checkState)
                {
                    items.Add(item.Text);
                }
            }
            return items;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && addButton.Enabled)
            {
                e.Handled = true;
                AddTag();
            }
        }

        private void SortList()
        {
            var listChecked = new List<TagsDataGridViewRow>();
            var listIndeterminate = new List<TagsDataGridViewRow>();
            var listNonChecked = new List<TagsDataGridViewRow>();

            foreach (TagsDataGridViewRow item in tagsDataGrid.Rows)
            {
                if (item.Checked == CheckState.Checked)
                {
                    listChecked.Add(item);
                }
                else if (item.Checked == CheckState.Unchecked)
                {
                    listNonChecked.Add(item);
                }
                else
                {
                    listIndeterminate.Add(item);
                }
            }
            listNonChecked.Sort(Comparison());
            listChecked.Sort(Comparison());
            listIndeterminate.Sort(Comparison());
            tagsDataGrid.Rows.Clear();
            foreach (var item in listChecked)
            {
                tagsDataGrid.Rows.Add(item);
            }
            foreach (var item in listIndeterminate)
            {
                tagsDataGrid.Rows.Add(item);
            }
            foreach (var item in listNonChecked)
            {
                tagsDataGrid.Rows.Add(item);
            }
        }

        private Comparison<TagsDataGridViewRow> Comparison()
        {
            return delegate (TagsDataGridViewRow x, TagsDataGridViewRow y)
                       {
                           return x.Text.CompareTo(y.Text);
                       };
        }

        private TagsDataGridViewRow FindTag(string tag)
        {
            // Used to use tagsListView.Items.ContainsKey(tag), but that uses the Name
            // instead of the Text, and is also not case sensitive, which caused a bug.
            foreach (TagsDataGridViewRow item in tagsDataGrid.Rows)
            {
                if (item.Text == tag)
                    return item;
            }
            return null;
        }

        private void AddTag()
        {
            string text = this.textBox1.Text.Trim();
            var item = FindTag(text);
            if (item == null)
            {
                item = new TagsDataGridViewRow { Checked = CheckState.Checked, Text = text };
                tagsDataGrid.Rows.Add(item);
            }
            else
            {
                item.Checked = CheckState.Checked;
            }

            this.textBox1.Text = "";
            addButton.Enabled = false;
            SortList();
        }

        private void LoadTags(List<string> tags, List<string> indeterminateTags)
        {
            Program.AssertOnEventThread();

            tagsDataGrid.Rows.Clear();

            foreach (string tag in Tags.GetAllTags())
            {
                var checkState = CheckState.Unchecked;
                if (tags.Contains(tag))
                {
                    checkState = CheckState.Checked;
                }
                else if (indeterminateTags.Contains(tag))
                {
                    checkState = CheckState.Indeterminate;
                }

                var item = new TagsDataGridViewRow { Checked = checkState, Text = tag };
                tagsDataGrid.Rows.Add(item);
            }
            foreach (string tag in tags)   // We need to include these too, because they may have been recently added and not yet got into GetAllTags()
            {
                if (FindTag(tag) == null)
                {
                    var item = new TagsDataGridViewRow { Checked = CheckState.Checked, Text = tag};
                    tagsDataGrid.Rows.Add(item);
                }
            }
            SortList();
        }

        private void NewTagDialog_Activated(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddTag();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            addButton.Enabled = (this.textBox1.Text.Trim() != string.Empty);
        }

        private void tagsDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                ToggleItems(tagsDataGrid.SelectedRows);
                e.Handled = true;
            }
        }

        private void ToggleItems(System.Collections.IList items)
        {
            if (items.Count < 1)
                return;

            CheckState firstCheckState = ((TagsDataGridViewRow) items[0]).Checked;
            foreach (TagsDataGridViewRow item in items)
            {
                item.Toggle(firstCheckState);
            }
        }

        public class TagsDataGridViewRow : DataGridViewRow
        {
            private readonly DataGridViewCheckBoxCell _cellCheckState;
            private readonly DataGridViewTextBoxCell _cellTag;

            public TagsDataGridViewRow()
            {
                _cellCheckState = new DataGridViewCheckBoxCell { Value = CheckState.Unchecked, ThreeState = true};
                _cellTag = new DataGridViewTextBoxCell();
                Cells.AddRange(_cellCheckState, _cellTag);
            }

            public void Toggle()
            {
                Toggle(Checked);
            }

            public void Toggle(CheckState stateToToggleFrom)
            {
                Checked = stateToToggleFrom == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked;
            }

            public CheckState Checked
            {
                get
                {
                    var value = _cellCheckState.Value;
                    if (value == null)
                        return CheckState.Unchecked;

                    if (value is CheckState)
                        return (CheckState)value;

                    if (value is bool)
                        return (bool)value ? CheckState.Checked : CheckState.Unchecked;

                    return CheckState.Indeterminate;
                }
                set { _cellCheckState.Value = value; }
            }

            public string Text
            {
                get { return _cellTag.Value.ToString(); }
                set { _cellTag.Value = value; }
            }
        }

        private void tagsDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var col = tagsDataGrid.Columns[e.ColumnIndex] as DataGridViewCheckBoxColumn;

            if (col == null || !col.ThreeState)
                return;

            var state = (CheckState)tagsDataGrid[e.ColumnIndex, e.RowIndex].EditedFormattedValue;
            if (state != CheckState.Indeterminate)
                return;

            tagsDataGrid[e.ColumnIndex, e.RowIndex].Value = CheckState.Unchecked;
            tagsDataGrid.RefreshEdit();
            tagsDataGrid.NotifyCurrentCellDirty(true);
        }
    }
}
