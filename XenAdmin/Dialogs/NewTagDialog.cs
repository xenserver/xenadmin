/* Copyright (c) Cloud Software Group, Inc. 
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
using XenCenterLib;


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
            foreach (var item in ExtractList())
            {
                if (item.Checked == checkState)
                {
                    items.Add(item.Text);
                }
            }
            return items;
        }

        private List<TagsDataGridViewRow> ExtractList()
        {
            var rows = new List<TagsDataGridViewRow>();
            foreach (TagsDataGridViewRow item in tagsDataGrid.Rows)
            {
                rows.Add(item);
            }
            return rows;
        }

        private void DisplayList(List<TagsDataGridViewRow> rows)
        {
            rows.Sort();

            try
            {
                tagsDataGrid.SuspendLayout();
                tagsDataGrid.Rows.Clear();
                foreach (var item in rows)
                {
                    tagsDataGrid.Rows.Add(item);
                }
            }
            finally
            {
                tagsDataGrid.ResumeLayout();
            }
        }

        private TagsDataGridViewRow FindTag(string tag, List<TagsDataGridViewRow> rows)
        {
            // Used to use tagsListView.Items.ContainsKey(tag), but that uses the Name
            // instead of the Text, and is also not case sensitive, which caused a bug.
            foreach (var item in rows)
            {
                if (item.Text == tag)
                    return item;
            }
            return null;
        }

        private void AddTag()
        {
            var rows = ExtractList();
            string text = this.textBox1.Text.Trim();
            var item = FindTag(text, rows);
            if (item == null)
            {
                item = new TagsDataGridViewRow { Checked = CheckState.Checked, Text = text };
                rows.Add(item);
            }
            else
            {
                item.Checked = CheckState.Checked;
            }

            this.textBox1.Text = "";
            addButton.Enabled = false;
            DisplayList(rows);
        }

        private void LoadTags(List<string> tags, List<string> indeterminateTags)
        {
            Program.AssertOnEventThread();

            var rows = new List<TagsDataGridViewRow>();

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
                rows.Add(item);
            }
            foreach (string tag in tags)   // We need to include these too, because they may have been recently added and not yet got into GetAllTags()
            {
                if (FindTag(tag, rows) == null)
                {
                    var item = new TagsDataGridViewRow { Checked = CheckState.Checked, Text = tag};
                    rows.Add(item);
                }
            }
            DisplayList(rows);
        }

        private void ToggleItems(System.Collections.IList items)
        {
            var allChecked = true;
            foreach (TagsDataGridViewRow item in items)
            {
                if (item.Checked != CheckState.Checked)
                {
                    allChecked = false;
                    break;
                }
            }

            foreach (TagsDataGridViewRow item in items)
                item.Checked = allChecked ? CheckState.Unchecked : CheckState.Checked;
        }

        #region Event handlers
        
        private void NewTagDialog_Activated(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddTag();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Return) && addButton.Enabled)
            {
                e.Handled = true;
                AddTag();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            addButton.Enabled = textBox1.Text.Trim() != string.Empty;
        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return;
        }

        private void tagsDataGrid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Space)
            {
                ToggleItems(tagsDataGrid.SelectedRows);
                e.Handled = true;
            }
        }

        private void tagsDataGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != ColumnChecked.Index || e.RowIndex < 0 || e.RowIndex >= tagsDataGrid.RowCount)
                return;

            var row = tagsDataGrid.Rows[e.RowIndex] as TagsDataGridViewRow;
            if (row != null)
                row.Toggle();
        }

        private void tagsDataGrid_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.RowIndex1 < 0 || e.RowIndex1 >= tagsDataGrid.RowCount || e.RowIndex2 < 0 || e.RowIndex2 >= tagsDataGrid.RowCount)
                return;

            var row1 = tagsDataGrid.Rows[e.RowIndex1] as TagsDataGridViewRow;
            var row2 = tagsDataGrid.Rows[e.RowIndex2] as TagsDataGridViewRow;

            if (row1 != null && row2 != null)
            {
                e.SortResult = row1.CompareTo(row2);
                e.Handled = true;
            }
        }

        #endregion

        public class TagsDataGridViewRow : DataGridViewRow, IComparable<TagsDataGridViewRow>
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
                Checked = Checked == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked;
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

            public int CompareTo(TagsDataGridViewRow other)
            {
                if (other == null)
                    throw new ArgumentNullException(string.Format("Compared {0} must not be null.", GetType().Name));

                var checkStateComparer = new SortCheckedStateForTagsHelper();
                var output = checkStateComparer.Compare(Checked, other.Checked);
                if (output != 0)
                    return output;

                return StringUtility.NaturalCompare(Text, other.Text);
            }
        }

        private class SortCheckedStateForTagsHelper : IComparer<CheckState>
        {
            private static readonly IList<CheckState> Priority = new List<CheckState>
            {
                CheckState.Checked,
                CheckState.Indeterminate,
                CheckState.Unchecked
            };

            public int Compare(CheckState a, CheckState b)
            {
                var priorityA = Priority.IndexOf(a);
                var priorityB = Priority.IndexOf(b);
                return priorityA.CompareTo(priorityB);
            }
        }
    }
}
