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
using XenAdmin.Model;
using XenAPI;
using System.Drawing;
using XenAdmin.Core;
using XenAdmin.Actions;
using System.Windows.Forms.VisualStyles;


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
            InitDialog();
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

        private void InitDialog()
        {
            InitializeComponent();

            // CheckBoxes is false as we draw them ourselves for tri-state.
            tagsListView.CheckBoxes = false;
            tagsListView.HideSelection = false;
            tagsListView.MultiSelect = true;
            tagsListView.LabelEdit = true;
            tagsListView.AfterLabelEdit += tagsListView_AfterLabelEdit;
            tagsListView.Activation = ItemActivation.TwoClick;
            textBox1.KeyPress += textBox1_KeyPress;

            // populate state image list for tri-state checkboxes
            using (Graphics listViewGraphics = Graphics.FromHwnd(tagsListView.Handle))
            {
                stateImageList.ImageSize = CheckBoxRenderer.GetGlyphSize(listViewGraphics, CheckBoxState.CheckedNormal);
            }

            foreach (CheckBoxState state in new CheckBoxState[] { CheckBoxState.UncheckedNormal, CheckBoxState.CheckedNormal, CheckBoxState.MixedNormal })
            {
                Bitmap bitmap = new Bitmap(stateImageList.ImageSize.Width, stateImageList.ImageSize.Height);

                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.Transparent);
                    CheckBoxRenderer.DrawCheckBox(g, new Point(0, 0), state);
                }
                stateImageList.Images.Add(bitmap);
            }
        }

        private void tagsListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            okButton.Enabled = true;

            TagsListViewItem itemRenaming = (TagsListViewItem)tagsListView.Items[e.Item];
            if (itemRenaming != null)
            {
                string oldName = itemRenaming.Text;
                string newName = (e.Label == null ? oldName : e.Label.Trim());   // null indicates no change

                if (newName == "")
                {
                    e.CancelEdit = true;
                    return;
                }

                foreach (TagsListViewItem currentItem in tagsListView.Items)
                {
                    if (currentItem != itemRenaming && currentItem.Text == newName)
                    {
                        e.CancelEdit = true;
                        return;
                    }
                }

                // Rename the tag in the list view ourselves (necessary if we've trimmed whitespace from the ends of the name)
                itemRenaming.Text = newName;
                e.CancelEdit = true;

                // Rename the tag on all the servers
                DelegatedAsyncAction action = new DelegatedAsyncAction(null,
                    String.Format(Messages.RENAME_TAG, oldName),
                    String.Format(Messages.RENAMING_TAG, oldName),
                    String.Format(Messages.RENAMED_TAG, oldName),
                    delegate(Session session)
                    {
                        Tags.RenameTagGlobally(oldName, newName);
                    });
                action.RunAsync();
            }
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

        private bool TagPresent(string tag)
        {
            // Used to use tagsListView.Items.ContainsKey(tag), but that uses the Name
            // instead of the Text, and is also not case sensitive, which caused a bug.
            foreach (TagsDataGridViewRow item in tagsDataGrid.Rows)
            {
                if (item.Text == tag)
                    return true;
            }
            return false;
        }

        private void AddTag()
        {
            string text = this.textBox1.Text.Trim();
            if (!TagPresent(text))
            {
                var item = new TagsDataGridViewRow();
                tagsDataGrid.Rows.Add(item);
                item.Checked = CheckState.Checked;
                item.Text = text;
            }
            else
            {
                foreach (TagsDataGridViewRow item in tagsDataGrid.Rows)
                {
                    if (item.Text == text)
                    {
                        item.Checked = CheckState.Checked;
                        break;
                    }
                }
            }

            this.textBox1.Text = "";
            addButton.Enabled = false;
            SortList();
        }

        private void LoadTags(List<string> tags, List<string> indeterminateTags)
        {
            Program.AssertOnEventThread();

            tagsListView.Items.Clear();

            foreach (string tag in Tags.GetAllTags())
            {
                var item = new TagsDataGridViewRow();
                tagsDataGrid.Rows.Add(item);

                if (tags.Contains(tag))
                {
                    item.Checked = CheckState.Checked;
                }
                else if (indeterminateTags.Contains(tag))
                {
                    item.Checked = CheckState.Indeterminate;
                }

                item.Text = tag;
            }
            foreach (string tag in tags)   // We need to include these too, because they may have been recently added and not yet got into GetAllTags()
            {
                if (!TagPresent(tag))
                {
                    var item = new TagsDataGridViewRow();
                    tagsDataGrid.Rows.Add(item);
                    item.Checked = CheckState.Checked;
                    item.Text = tag;
                }
            }
            SortList();
        }

        private void NewTagDialog_Activated(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddTag();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            addButton.Enabled = (this.textBox1.Text.Trim() != string.Empty);
        }

        private void tagsListView_MouseClick(object sender, MouseEventArgs e)
        {
            TagsListViewItem item = (TagsListViewItem)tagsListView.GetItemAt(e.X, e.Y);

            if (item != null)
            {
                Rectangle iconRect = tagsListView.GetItemRect(item.Index, ItemBoundsPortion.Icon);
                Rectangle checkRect = new Rectangle(0, iconRect.Top, iconRect.Left, iconRect.Height);

                // if there's a selection and the clicked item is in the selection, then toggle the
                // entire selection

                if (checkRect.Contains(e.Location))
                {
                    if (tagsListView.SelectedItems.Count > 0 && tagsListView.SelectedItems.Contains(item))
                    {
                        ToggleItems(tagsListView.SelectedItems);
                    }
                    else
                    {
                        item.Toggle();
                    }
                }
            }
        }

        private void tagsListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                ToggleItems(tagsListView.SelectedItems);
            }
        }

        private void tagsDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                ToggleItemsNew(tagsDataGrid.SelectedRows);
                e.Handled = true;
            }
        }

        private void ToggleItems(System.Collections.IList items)
        {
            if(items.Count < 1)
                return;

            CheckState firstCheckState = ((TagsListViewItem)items[0]).Checked;
            foreach (TagsListViewItem item in items)
            {
                item.Toggle(firstCheckState);
            }
        }

        private void ToggleItemsNew(System.Collections.IList items)
        {
            if (items.Count < 1)
                return;

            CheckState firstCheckState = ((TagsDataGridViewRow) items[0]).Checked;
            foreach (TagsDataGridViewRow item in items)
            {
                item.Toggle(firstCheckState);
            }
        }

        private class TagsListViewItem : ListViewItem
        {
            public TagsListViewItem(string text)
                : base(text)
            {
                Checked = CheckState.Unchecked;
            }

            public void Toggle()
            {
                Toggle(Checked);
            }

            public void Toggle(CheckState stateToToggleFrom)
            {
                StateImageIndex = Convert.ToInt32(!Convert.ToBoolean(stateToToggleFrom));
            }

            public new CheckState Checked
            {
                get
                {
                    if (StateImageIndex == 0)
                    {
                        return CheckState.Unchecked;
                    }
                    else if (StateImageIndex == 1)
                    {
                        return CheckState.Checked;
                    }
                    else
                    {
                        return CheckState.Indeterminate;
                    }
                }
                set
                {
                    if (value == CheckState.Unchecked)
                    {
                        StateImageIndex = 0;
                    }
                    else if (value == CheckState.Checked)
                    {
                        StateImageIndex = 1;
                    }
                    else
                    {
                        StateImageIndex = 2;
                    }
                }
            }
        }

        private class TagsDataGridViewRow : DataGridViewRow
        {
            public TagsDataGridViewRow()
            {
                var cellEnabled = new DataGridViewCheckBoxCell { Value = CheckState.Unchecked };
                var cellTag = new DataGridViewTextBoxCell();
                Cells.AddRange(cellEnabled, cellTag);
            }

            public void Toggle()
            {
                Toggle(Checked);
            }

            public void Toggle(CheckState stateToToggleFrom)
            {
                Checked = Opposite(stateToToggleFrom);
            }

            public CheckState Checked
            {
                get
                {
                    var value = CellEnabled.Value;
                    if (value == null)
                        return CheckState.Unchecked;

                    if (value is CheckState)
                        return (CheckState)value;

                    if (value is bool)
                        return (bool)value ? CheckState.Checked : CheckState.Unchecked;

                    return CheckState.Indeterminate;
                }
                set { CellEnabled.Value = value; }
            }

            public string Text
            {
                get { return CellTag.Value.ToString(); }
                set { CellTag.Value = value; }
            }

            private DataGridViewCheckBoxCell CellEnabled
            {
                get { return (DataGridViewCheckBoxCell)Cells[0]; }
            }

            private DataGridViewTextBoxCell CellTag
            {
                get { return (DataGridViewTextBoxCell)Cells[1]; }
            }

            private CheckState Opposite(CheckState state)
            {
                return state == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked;
            }
        }
    }
}
