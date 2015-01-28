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


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_SelectPatchPage : XenTabPage
    {
        public PatchingWizard_SelectPatchPage()
        {
            InitializeComponent();
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
            if (direction == PageLoadedDirection.Back)
            {
                if (dataGridViewPatches.SelectedRows.Count == 0)
                    return;
                
                // refresh selected row if the update type is Existing (i.e. the update has been uploaded)
                int selectedIndex = dataGridViewPatches.SelectedRows[0].Index;
                string updateName = dataGridViewPatches.Rows[selectedIndex].Cells[ColumnUpdate.Index].Value.ToString();
                
                if (SelectedUpdateType == UpdateType.Existing && (updateName.EndsWith(".xsoem") || updateName.EndsWith(".xsupdate")))
                {
                    // remove selected row and add a new one
                    dataGridViewPatches.Rows.RemoveAt(selectedIndex);
                    var row = new PatchGridViewRow(SelectedExistingPatch);
                    if (!dataGridViewPatches.Rows.Contains(row))
                    {
                        dataGridViewPatches.Rows.Add(row);
                        row.UpdateDetails();
                    }
                    row.Selected = true;
                }
                return;
            }

            PopulatePatchesBox();
            UpdateEnablement();
        }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
            {
                if (dataGridViewPatches.SelectedRows.Count > 0 && dataGridViewPatches.SelectedRows[0].Cells[ColumnUpdate.Index].Value.ToString().EndsWith(".xsoem"))
                    SelectedUpdateType = UpdateType.NewOem;
                else if (dataGridViewPatches.SelectedRows.Count > 0 && dataGridViewPatches.SelectedRows[0].Cells[ColumnUpdate.Index].Value.ToString().EndsWith(".xsupdate"))
                    SelectedUpdateType = UpdateType.NewRetail;
                else if (dataGridViewPatches.SelectedRows.Count > 0 && dataGridViewPatches.SelectedRows[0].Cells[ColumnUpdate.Index].Value.ToString().EndsWith(".iso"))
                    SelectedUpdateType = UpdateType.NewSuppPack;
                else
                    SelectedUpdateType = UpdateType.Existing;

                SelectedExistingPatch = SelectedUpdateType == UpdateType.Existing
                                             ? ((PatchGridViewRow)dataGridViewPatches.SelectedRows[0]).Patch
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
            base.PageLeave(direction, ref cancel);
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
            foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
            {
                foreach (Pool_patch patch in xenConnection.Cache.Pool_patches)
                {
                    if (patch.size > 0)
                    {
                        PatchGridViewRow row = new PatchGridViewRow(patch);
                        if (!dataGridViewPatches.Rows.Contains(row))
                        {
                            dataGridViewPatches.Rows.Add(row);
                            row.UpdateDetails();
                        }
                    }
                }
            }
            dataGridViewPatches.Sort(ColumnUpdate, ListSortDirection.Ascending);
        }

        public override bool EnableNext()
        {
            if (dataGridViewPatches.SelectedRows.Count == 1)
            {
                DataGridViewExRow row = (DataGridViewExRow)dataGridViewPatches.SelectedRows[0];
                if (row.Enabled)
                    return true;
            }
            return false;
        }

        private void UpdateEnablement()
        {
            OnPageUpdated();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            // Showing this dialog has the (undocumented) side effect of changing the working directory
            // to that of the file selected. This means a handle to the directory persists, making
            // it undeletable until the program exits, or the working dir moves on. So, save and
            // restore the working dir...
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
                PatchGridViewRow row = new PatchGridViewRow(fileName);
                int index = dataGridViewPatches.Rows.IndexOf(row);
                if (index < 0)
                {
                    dataGridViewPatches.Rows.Insert(0, row);
                    index = 0;
                }
                dataGridViewPatches.Rows[index].Selected = true;
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
                return SelectedUpdateType == UpdateType.NewRetail || SelectedUpdateType == UpdateType.NewOem || SelectedUpdateType == UpdateType.NewSuppPack
                           ? ((PatchGridViewRow)dataGridViewPatches.SelectedRows[0]).PathPatch
                           : null;
            }
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

        private void dataGridViewPatches_SelectionChanged(object sender, EventArgs e)
        {
            UpdateEnablement();
        }

        private class PatchGridViewRow : DataGridViewExRow, IEquatable<PatchGridViewRow>
        {
            private readonly Pool_patch _patch;

            private bool expanded = false;

            private bool _isFile = false;
            private string _patchPath;

            private DataGridViewImageCell _imageCell;
            private DataGridViewTextBoxCell _nameCell;
            private DataGridViewTextBoxCell _descriptionCell;
            private DataGridViewTextBoxCell _statusCell;


            public PatchGridViewRow(Pool_patch patch)
            {
                if (patch == null)
                    throw new ArgumentNullException();
                _patch = patch;
                SetupCells();
            }

            public PatchGridViewRow(string patchPath)
            {
                _isFile = true;
                _patchPath = patchPath;
                SetupCells();
            }

            public Pool_patch Patch
            {
                get { return _patch; }
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

                Cells.Add(_imageCell);
                Cells.Add(_nameCell);
                Cells.Add(_descriptionCell);
                Cells.Add(_statusCell);
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
                else
                {

                    //It works across connections

                    _nameCell.Value = Helpers.GetName(Patch);
                    StringBuilder appliedOn = new StringBuilder(Messages.APPLIED_ON);
                    List<Host> hostsAppliedTo = new List<Host>();
                    foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
                    {
                        Pool_patch poolPatch = xenConnection.Cache.Find_By_Uuid<Pool_patch>(Patch.uuid);
                        if (poolPatch != null)
                        {
                            hostsAppliedTo.AddRange(poolPatch.HostsAppliedTo());
                        }
                    }
                    if (hostsAppliedTo.Count > 0)
                        appliedOn.Append(Helpers.GetListOfNames(hostsAppliedTo));
                    else
                    {
                        appliedOn.Append(Messages.NONE);
                    }
                    _descriptionCell.Value = expanded ? String.Format("{0}\n{1}", _patch.Description, appliedOn.ToString())
                                                        : _patch.Description;
                    int totalNumberHostsRunning = 0;

                    foreach (IXenConnection xenConnection in ConnectionsManager.XenConnectionsCopy)
                    {
                        foreach (Host host in xenConnection.Cache.Hosts)
                        {
                            totalNumberHostsRunning++;
                        }
                    }

                    if (hostsAppliedTo.Count == 0)
                        _statusCell.Value = Messages.NOT_APPLIED;
                    else if (hostsAppliedTo.Count == totalNumberHostsRunning)
                    {
                        _statusCell.Value = Messages.FULLY_APPLIED;
                        this.Enabled = false;
                    }
                    else
                    {
                        List<Host> appliedTo = Patch.HostsAppliedTo();
                        if (appliedTo.Count > 0 && appliedTo[0].isOEM)
                        {
                            _statusCell.Value = Messages.FULLY_APPLIED;
                            this.Enabled = false;
                        }
                        else
                            _statusCell.Value = Messages.PARTIALLY_APPLIED;
                    }
                }

            }

            public void toggleExpandedState()
            {
                expanded = !expanded;
                UpdateDetails();
            }


            public bool Equals(PatchGridViewRow other)
            {
                if (other.Patch != null && this.Patch != null && this.Patch.uuid == other.Patch.uuid)
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
