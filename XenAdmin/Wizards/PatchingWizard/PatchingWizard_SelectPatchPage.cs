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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Properties;
using XenAPI;
using System.ComponentModel;
using System.IO;
using XenAdmin.Dialogs;
using System.Drawing;
using XenAdmin.Alerts;


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_SelectPatchPage : XenTabPage
    {
        private bool CheckForUpdatesInProgress;
        public XenServerPatchAlert SelectedUpdateAlert;
        public XenServerPatchAlert FileFromDiskAlert;
        
        public PatchingWizard_SelectPatchPage()
        {
            InitializeComponent();
            PopulatePatchesBox();
            dataGridViewPatches.Sort(ColumnDate, ListSortDirection.Descending);
        }

        private void CheckForUpdates_CheckForUpdatesCompleted(bool succeeded, string errorMessage)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                PopulatePatchesBox();               
                RefreshListButton.Enabled = true;
                CheckForUpdatesInProgress = false;
                OnPageUpdated();
            });
        }

        public void SelectDownloadAlert(XenServerPatchAlert alert)
        {
            downloadUpdateRadioButton.Checked = true;
            foreach (PatchGridViewRow row in dataGridViewPatches.Rows)
            {
                if(row.UpdateAlert.Equals(alert))
                {
                    row.Selected = true;
                }
            }
        }

        public override string Text
        {
            get
            {
                return Messages.PATCHINGWIZARD_SELECTPATCHPAGE_TEXT;
            }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.PATCHINGWIZARD_SELECTPATCHPAGE_TITLE;
            }
        }

        public override string HelpID
        {
            get { return "SelectUpdate"; }
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {           
            base.PageLoaded(direction);
            RefreshListButton.Enabled = true;
            Updates.CheckForUpdatesCompleted += CheckForUpdates_CheckForUpdatesCompleted;
         
            if (direction == PageLoadedDirection.Forward)
            {
                PopulatePatchesBox();
                UpdateEnablement();
            }
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
            {
                var fileName = fileNameTextBox.Text;
                if (downloadUpdateRadioButton.Checked)
                {
                    if (dataGridViewPatches.SelectedRows.Count > 0 && ((PatchGridViewRow)(dataGridViewPatches.SelectedRows[0])).UpdateAlert.WebPageLabel.EndsWith(".xsoem"))
                    {
                        SelectedUpdateType = UpdateType.NewOem;
                    }
                    else
                    {
                        SelectedUpdateType = UpdateType.NewRetail;
                    }                   
                }
                else
                {                    
                    if (isValidFile())
                    {
                        if (fileName.EndsWith(".xsoem"))
                            SelectedUpdateType = UpdateType.NewOem;
                        else if (fileName.EndsWith(".xsupdate"))
                            SelectedUpdateType = UpdateType.NewRetail;
                        else if (fileName.EndsWith(".iso"))
                            SelectedUpdateType = UpdateType.NewSuppPack;
                        else
                            SelectedUpdateType = UpdateType.Existing;
                    }
                }
                SelectedUpdateAlert = downloadUpdateRadioButton.Checked
                                             ? (XenServerPatchAlert)((PatchGridViewRow)dataGridViewPatches.SelectedRows[0]).UpdateAlert
                                             : null;
                FileFromDiskAlert = selectFromDiskRadioButton.Checked
                                             ? GetAlertFromFileName(fileName)
                                             : null;


                if (SelectedExistingPatch != null && !SelectedExistingPatch.Connection.IsConnected)
                {
                    cancel = true;
                    PageLeaveCancelled(string.Format(Messages.UPDATES_WIZARD_CANNOT_DOWNLOAD_PATCH,
                                                     SelectedExistingPatch.Connection.Name));
                }
                else if (!string.IsNullOrEmpty(SelectedNewPatch) && !File.Exists(SelectedNewPatch))
                {
                    cancel = true;
                    PageLeaveCancelled(string.Format(Messages.UPDATES_WIZARD_FILE_NOT_FOUND, SelectedNewPatch));
                }
            }
            Updates.CheckForUpdatesCompleted -= CheckForUpdates_CheckForUpdatesCompleted;
            base.PageLeave(direction, ref cancel);
        }

        private XenServerPatchAlert GetAlertFromFileName(string fileName)
        {
            foreach (PatchGridViewRow row in dataGridViewPatches.Rows)
            {
                if (row.UpdateAlert.Name == Path.GetFileNameWithoutExtension(fileName))
                {
                    return (XenServerPatchAlert)row.UpdateAlert;
                }
            }
            return null;
        }

        private void PageLeaveCancelled(string message)
        {
            new ThreeButtonDialog(
                new ThreeButtonDialog.Details(SystemIcons.Warning, message, Messages.UPDATES_WIZARD)).ShowDialog(this);
            
            ((PatchGridViewRow)dataGridViewPatches.SelectedRows[0]).UpdateDetails();
        }

        private void PopulatePatchesBox()
        {
            dataGridViewPatches.Rows.Clear();
            var updates = new List<Alert>(Updates.UpdateAlerts);
            if (dataGridViewPatches.SortedColumn != null)
            {
                if (dataGridViewPatches.SortedColumn.Index == ColumnUpdate.Index)
                    updates.Sort(Alert.CompareOnTitle);
                else if (dataGridViewPatches.SortedColumn.Index == ColumnDate.Index)
                    updates.Sort(Alert.CompareOnDate);
                else if (dataGridViewPatches.SortedColumn.Index == ColumnDescription.Index)
                    updates.Sort(Alert.CompareOnAppliesTo);

                if (dataGridViewPatches.SortOrder == SortOrder.Descending)
                    updates.Reverse();
            }
           foreach (Alert alert in updates)
           {
               if (alert is XenServerPatchAlert)
               {
                   PatchGridViewRow row = new PatchGridViewRow(alert);
                   if (!dataGridViewPatches.Rows.Contains(row))
                   {
                       dataGridViewPatches.Rows.Add(row);
                   }
               }
            }
        }
        
        public override void PageCancelled()
        {
            Updates.CheckForUpdatesCompleted -= CheckForUpdates_CheckForUpdatesCompleted;
        }

        public override bool EnableNext()
        {
            if (CheckForUpdatesInProgress)
            {
                return false;
            }
            if (downloadUpdateRadioButton.Checked)
            {                
                if (dataGridViewPatches.SelectedRows.Count == 1)
                {
                    DataGridViewExRow row = (DataGridViewExRow)dataGridViewPatches.SelectedRows[0];
                    if (row.Enabled)
                    {
                        return true;
                    }
                }
            }
            else if (selectFromDiskRadioButton.Checked)
            {
                if (isValidFile())
                {
                    return true;
                }
            }
            return false;
        }

        private bool isValidFile()
        {
            var fileName = fileNameTextBox.Text;
            return !string.IsNullOrEmpty(fileName) && File.Exists(fileName) && (fileName.EndsWith(".xsoem") || fileName.EndsWith(".xsupdate") || fileName.EndsWith(".iso"));
        }


        private void UpdateEnablement()
        {
            dataGridViewPatches.HideSelection = !downloadUpdateRadioButton.Checked;
            OnPageUpdated();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            // Showing this dialog has the (undocumented) side effect of changing the working directory
            // to that of the file selected. This means a handle to the directory persists, making
            // it undeletable until the program exits, or the working dir moves on. So, save and
            // restore the working dir...
            selectFromDiskRadioButton.Checked = true;
            String oldDir = "";
            try
            {
                oldDir = Directory.GetCurrentDirectory();
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Multiselect = false;
                dlg.ShowReadOnly = false;
                dlg.Filter = Messages.PATCHINGWIZARD_SELECTPATCHPAGE_UPDATESEXT;
                dlg.FilterIndex = 0;
                dlg.CheckFileExists = true;
                dlg.ShowHelp = false;
                dlg.Title = Messages.PATCHINGWIZARD_SELECTPATCHPAGE_CHOOSE;

                if (dlg.ShowDialog(this) == DialogResult.OK && dlg.CheckFileExists)
                {
                    AddFile(dlg.FileName);
                }
                OnPageUpdated();
            }
            finally
            {
                Directory.SetCurrentDirectory(oldDir);
            }
        }

        public void AddFile(string fileName)
        {
            if (fileName.EndsWith(".xsoem") || fileName.EndsWith(".xsupdate") || fileName.EndsWith(".iso"))
            {
                fileNameTextBox.Text = fileName;
                selectFromDiskRadioButton.Checked = true;
            }
            else
            {
                new ThreeButtonDialog(new ThreeButtonDialog.Details(SystemIcons.Error, Messages.UPDATES_WIZARD_NOTVALID_EXTENSION, Messages.UPDATES)).ShowDialog(this);
            }
        }

        public UpdateType SelectedUpdateType { get; set; }

        public Pool_patch SelectedExistingPatch { get; set; }

        public string SelectedNewPatch
        {
            get
            {
                if (downloadUpdateRadioButton.Checked)
                {
                    return SelectedUpdateType == UpdateType.NewRetail || SelectedUpdateType == UpdateType.NewOem || SelectedUpdateType == UpdateType.NewSuppPack
                               ? ((PatchGridViewRow)dataGridViewPatches.SelectedRows[0]).PathPatch
                               : null;
                }
                else if (selectFromDiskRadioButton.Checked)
                {
                    return SelectedUpdateType == UpdateType.NewRetail || SelectedUpdateType == UpdateType.NewOem || SelectedUpdateType == UpdateType.NewSuppPack
                              ? fileNameTextBox.Text
                               : null;
                }
                else return null;
            }
        }

        private void dataGridViewPatches_SelectionChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void fileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            selectFromDiskRadioButton.Checked = true;
            UpdateEnablement();
        }

        private void dataGridViewPatches_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (e.RowIndex < 0)
                // The click is on a column header
                return;
            PatchGridViewRow row = (PatchGridViewRow)dataGridViewPatches.Rows[e.RowIndex];
            row.toggleExpandedState();
            UpdateEnablement();
        }

        private void RefreshListButton_Click(object sender, EventArgs e)
        {
            CheckForUpdatesInProgress = true;
            RefreshListButton.Enabled = false;
            Updates.CheckForUpdates(true);
            PopulatePatchesBox();
        }

        private void selectFromDiskRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private void dataGridViewPatches_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {

            Alert alert1 = ((PatchGridViewRow)dataGridViewPatches.Rows[e.RowIndex1]).UpdateAlert;
            Alert alert2 = ((PatchGridViewRow)dataGridViewPatches.Rows[e.RowIndex2]).UpdateAlert;

            if (e.Column.Index == ColumnDate.Index)
            {
                int sortResult = DateTime.Compare(alert1.Timestamp, alert2.Timestamp);
                e.SortResult = (dataGridViewPatches.SortOrder == SortOrder.Descending) ? sortResult *= -1 : sortResult;
                e.Handled = true;
            }
        }

        private void dataGridViewPatches_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridViewPatches.Columns[e.ColumnIndex].SortMode == DataGridViewColumnSortMode.Automatic)
                PopulatePatchesBox();
        }

        private void fileNameTextBox_Enter(object sender, EventArgs e)
        {
            selectFromDiskRadioButton.Checked = true;
            UpdateEnablement();
        }

        private void dataGridViewPatches_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {   // The click is on a column header
            if (e.RowIndex == -1)
            {
                return;
            }
            PatchGridViewRow row = dataGridViewPatches.Rows[e.RowIndex] as PatchGridViewRow;
            if (row != null && e.ColumnIndex == 3)
            {
                row.UpdateAlert.FixLinkAction();
                return;
            }
        }

        private void dataGridViewPatches_Enter(object sender, EventArgs e)
        {
            downloadUpdateRadioButton.Checked = true;
            UpdateEnablement();
        }

        private class PatchGridViewRow : DataGridViewExRow, IEquatable<PatchGridViewRow>
        {
            private readonly Alert _alert;

            private bool expanded = false;

            private bool _isFile = false;
            private string _patchPath;

            private DataGridViewImageCell _imageCell;
            private DataGridViewTextBoxCell _nameCell;
            private DataGridViewTextBoxCell _descriptionCell;
            private DataGridViewTextBoxCell _dateCell;
            private DataGridViewTextBoxCell _statusCell;
            private DataGridViewLinkCell _webPageCell;

            public PatchGridViewRow(Alert alert)
            {   
                _alert = alert;
                _nameCell = new DataGridViewTextBoxCell();
                _descriptionCell = new DataGridViewTextBoxCell();
                _dateCell = new DataGridViewTextBoxCell();
                _webPageCell = new DataGridViewLinkCell();

                Cells.Add(_nameCell);
                Cells.Add(_descriptionCell);
                Cells.Add(_dateCell);
                Cells.Add(_webPageCell);

                _nameCell.Value = String.Format(alert.Name);
                _descriptionCell.Value = String.Format(alert.Description);
                _dateCell.Value = HelpersGUI.DateTimeToString(alert.Timestamp.ToLocalTime(), Messages.DATEFORMAT_DMY, true);
                _webPageCell.Value = Messages.PATCHING_WIZARD_WEBPAGE_CELL;
            }

            public PatchGridViewRow(string patchPath)
            {
                _isFile = true;
                _patchPath = patchPath;
                SetupCells();
            }

            public Alert UpdateAlert
            { 
                get { return _alert; }
            }

            public string PathPatch
            {
                get { return _patchPath; }
            }

            private void SetupCells()
            {
                _imageCell = new DataGridViewExImageCell();
                _nameCell = new DataGridViewTextBoxCell();
                _descriptionCell = new DataGridViewTextBoxCell();
                _statusCell = new DataGridViewTextBoxCell();
                _webPageCell = new DataGridViewLinkCell();

                Cells.Add(_imageCell);
                Cells.Add(_nameCell);
                Cells.Add(_descriptionCell);
                Cells.Add(_statusCell);
                Cells.Add(_webPageCell);
                this.UpdateDetails();
            }

            private void UpdateFileDetails(string description, string status)
            {
                _descriptionCell.Value = description;
                _statusCell.Value = status;
            }

            public void UpdateDetails()
            {
                _imageCell.Value = expanded ? Resources.expanded_triangle : Resources.contracted_triangle;
                _webPageCell.Value = Messages.PATCHING_WIZARD_WEBPAGE_CELL;

                if (_isFile)
                {
                    _nameCell.Value = System.IO.Path.GetFileName(_patchPath);
                    FileInfo fileInfo = new FileInfo(_patchPath);

                    string description = expanded
                                             ? fileInfo.Exists
                                                   ? String.Format(Messages.PATCH_EXPANDED_DESCRIPTION
                                                                   , _patchPath, fileInfo.CreationTime,
                                                                   fileInfo.LastWriteTime, Util.DiskSizeString(fileInfo.Length))
                                                   : String.Format(Messages.PATCH_NOT_FOUND_EXPANDED_DESCRIPTION,
                                                                   _patchPath)
                                             : _patchPath;

                    UpdateFileDetails(description, fileInfo.Exists ? Messages.NOT_UPLOADED : Messages.PATCH_NOT_FOUND);
                }         
            }

            public void toggleExpandedState()
            {
                expanded = !expanded;
            }


            public bool Equals(PatchGridViewRow other)
            {
                if (other.UpdateAlert != null && this.UpdateAlert != null && this.UpdateAlert.uuid == other.UpdateAlert.uuid)
                    return true;
                if (other.PathPatch != null && this.PathPatch != null && this.PathPatch == other.PathPatch)
                    return true;
                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is PatchGridViewRow)
                    return this.Equals((PatchGridViewRow)obj);
                return false;
            }
        }       
    }        

    public enum UpdateType { NewRetail, NewOem, Existing, NewSuppPack}
}