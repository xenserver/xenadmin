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
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;
using System.IO;
using XenAdmin.Dialogs;
using System.Drawing;
using XenAdmin.Alerts;
using System.Linq;
using System.Xml;
using DiscUtils.Iso9660;


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_SelectPatchPage : XenTabPage
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool CheckForUpdatesInProgress;
        public XenServerPatchAlert SelectedUpdateAlert;
        public XenServerPatchAlert FileFromDiskAlert;
        public bool FileFromDiskHasUpdateXml { get; private set; }
        private bool firstLoad = true;
        private string unzippedUpdateFilePath;

        public PatchingWizard_SelectPatchPage()
        {
            InitializeComponent();
            tableLayoutPanelSpinner.Visible = false;

            labelWithAutomatedUpdates.Visible =
                automatedUpdatesOptionLabel.Visible = AutomatedUpdatesRadioButton.Visible = false;
            downloadUpdateRadioButton.Checked = true;
        }

        private void CheckForUpdates_CheckForUpdatesStarted()
        {
            Program.Invoke(Program.MainWindow, StartCheckForUpdates);
        }

        private void Updates_RestoreDismissedUpdatesStarted()
        {
            Program.Invoke(Program.MainWindow, StartCheckForUpdates);
        }

        private void StartCheckForUpdates()
        {
            if (CheckForUpdatesInProgress)
                return;

            CheckForUpdatesInProgress = true;
            dataGridViewPatches.Rows.Clear();
            tableLayoutPanelSpinner.Visible = true;
            RestoreDismUpdatesButton.Enabled = false;
            RefreshListButton.Enabled = false;
            OnPageUpdated();
        }

        private void CheckForUpdates_CheckForUpdatesCompleted(bool succeeded, string errorMessage)
        {
            Program.Invoke(Program.MainWindow, delegate
            {
                tableLayoutPanelSpinner.Visible = false;
                PopulatePatchesBox();
                RefreshListButton.Enabled = true;
                RestoreDismUpdatesButton.Enabled = true;
                CheckForUpdatesInProgress = false;
                OnPageUpdated();
            });
        }

        public void SelectDownloadAlert(XenServerPatchAlert alert)
        {
            downloadUpdateRadioButton.Checked = true;
            foreach (PatchGridViewRow row in dataGridViewPatches.Rows)
            {
                if (row.UpdateAlert.Equals(alert))
                {
                    row.Selected = true;
                }
            }
        }

        public override string Text
        {
            get { return Messages.PATCHINGWIZARD_SELECTPATCHPAGE_TEXT; }
        }

        public override string PageTitle
        {
            get { return Messages.PATCHINGWIZARD_SELECTPATCHPAGE_TITLE; }
        }

        public override string HelpID
        {
            get { return "SelectUpdate"; }
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            Updates.CheckForUpdatesStarted += CheckForUpdates_CheckForUpdatesStarted;
            Updates.CheckForUpdatesCompleted += CheckForUpdates_CheckForUpdatesCompleted;
            Updates.RestoreDismissedUpdatesStarted += Updates_RestoreDismissedUpdatesStarted;

            if (direction == PageLoadedDirection.Forward)
            {
                //if any connected host is licensed for automated updates
                bool automatedUpdatesPossible =
                    ConnectionsManager.XenConnectionsCopy.Any(
                        c => c != null && c.Cache.Hosts.Any(h => !Host.RestrictBatchHotfixApply(h)));

                labelWithAutomatedUpdates.Visible =
                    automatedUpdatesOptionLabel.Visible = AutomatedUpdatesRadioButton.Visible = automatedUpdatesPossible;
                labelWithoutAutomatedUpdates.Visible = !automatedUpdatesPossible;

                if (firstLoad)
                {
                    AutomatedUpdatesRadioButton.Checked = automatedUpdatesPossible;
                    downloadUpdateRadioButton.Checked = !automatedUpdatesPossible;
                }
                else if (!automatedUpdatesPossible && AutomatedUpdatesRadioButton.Checked)
                {
                    downloadUpdateRadioButton.Checked = true;
                }

                Updates.CheckServerPatches();
                PopulatePatchesBox();
                OnPageUpdated();
            }

            firstLoad = false;
        }

        private bool IsInAutomatedUpdatesMode
        {
            get { return AutomatedUpdatesRadioButton.Visible && AutomatedUpdatesRadioButton.Checked; }
        }

        public WizardMode WizardMode
        {
            get
            {
                if (AutomatedUpdatesRadioButton.Visible && AutomatedUpdatesRadioButton.Checked)
                    return WizardMode.AutomatedUpdates;
                var updateAlert = downloadUpdateRadioButton.Checked
                    ? SelectedUpdateAlert
                    : selectFromDiskRadioButton.Checked
                        ? FileFromDiskAlert
                        : null;
                if (updateAlert != null && updateAlert.NewServerVersion != null)
                    return WizardMode.NewVersion;
                return WizardMode.SingleUpdate;
            }
        }

        public KeyValuePair<XenServerPatch, string> PatchFromDisk
        {
            get
            {
                return selectFromDiskRadioButton.Checked && FileFromDiskAlert != null
                    ? new KeyValuePair<XenServerPatch, string>(FileFromDiskAlert.Patch, SelectedNewPatch)
                    : new KeyValuePair<XenServerPatch, string>(null, null);
            }
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
            {
                if (!IsInAutomatedUpdatesMode)
                {
                    if (selectFromDiskRadioButton.Checked && !string.IsNullOrEmpty(FilePath) &&
                        Path.GetExtension(FilePath).ToLowerInvariant().Equals(".zip"))
                    {
                        //check if we are installing update user sees in textbox
                        if (Path.GetFileNameWithoutExtension(unzippedUpdateFilePath) !=
                            Path.GetFileNameWithoutExtension(FilePath))
                        {
                            unzippedUpdateFilePath = WizardHelpers.ExtractUpdate(FilePath, this);
                            if (unzippedUpdateFilePath == null)
                                cancel = true;

                            unzippedFiles.Add(unzippedUpdateFilePath);
                        }
                    }
                    else
                        unzippedUpdateFilePath = null;

                    var fileName = WizardHelpers.IsValidFile(unzippedUpdateFilePath)
                        ? unzippedUpdateFilePath.ToLowerInvariant()
                        : FilePath.ToLowerInvariant();

                    SelectedUpdateAlert = downloadUpdateRadioButton.Checked &&
                                          dataGridViewPatches.SelectedRows.Count > 0
                        ? ((PatchGridViewRow) dataGridViewPatches.SelectedRows[0]).UpdateAlert
                        : null;

                    bool hasUpdateXml = false;
                    FileFromDiskAlert = selectFromDiskRadioButton.Checked
                        ? GetAlertFromFile(fileName, out hasUpdateXml)
                        : null;
                    FileFromDiskHasUpdateXml = hasUpdateXml;

                    if (downloadUpdateRadioButton.Checked)
                    {
                        var distinctHosts = SelectedUpdateAlert != null ? SelectedUpdateAlert.DistinctHosts : null;
                        if (distinctHosts != null && distinctHosts.Any(Helpers.ElyOrGreater))
                            // this is to check whether the Alert represents an ISO update (Ely or greater)
                        {
                            SelectedUpdateType = UpdateType.ISO;
                        }
                        else //legacy format
                        {
                            SelectedUpdateType = UpdateType.Legacy;
                        }
                    }
                    else
                    {
                        if (WizardHelpers.IsValidFile(fileName))
                        {
                            if (fileName.EndsWith("." + Branding.Update))
                                SelectedUpdateType = UpdateType.Legacy;
                            else if (fileName.EndsWith("." + Branding.UpdateIso))
                                SelectedUpdateType = UpdateType.ISO;
                            else
                                SelectedUpdateType = UpdateType.Existing;
                        }
                    }

                    if (SelectedExistingPatch != null && !SelectedExistingPatch.Connection.IsConnected)
                    {
                        cancel = true;

                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Warning,
                                string.Format(Messages.UPDATES_WIZARD_CANNOT_DOWNLOAD_PATCH, SelectedExistingPatch.Connection.Name),
                                Messages.UPDATES_WIZARD)))
                        {
                            dlg.ShowDialog(this);
                        }
                    }
                    else if (!string.IsNullOrEmpty(SelectedNewPatch) && !File.Exists(SelectedNewPatch))
                    {
                        cancel = true;

                        using (var dlg = new ThreeButtonDialog(
                            new ThreeButtonDialog.Details(SystemIcons.Warning,
                                string.Format(Messages.UPDATES_WIZARD_FILE_NOT_FOUND, SelectedNewPatch),
                                Messages.UPDATES_WIZARD)))
                        {
                            dlg.ShowDialog(this);
                        }
                    }
                }
                else //In Automatic Mode
                {
                    var succeed = Updates.CheckForUpdatesSync(this.Parent);

                    cancel = !succeed;
                }
            }

            if (!cancel) //unsubscribe only if we are really leaving this page
            {
                Updates.RestoreDismissedUpdatesStarted -= Updates_RestoreDismissedUpdatesStarted;
                Updates.CheckForUpdatesStarted -= CheckForUpdates_CheckForUpdatesStarted;
                Updates.CheckForUpdatesCompleted -= CheckForUpdates_CheckForUpdatesCompleted;
            }
        }

        private XenServerPatchAlert GetAlertFromFile(string fileName, out bool hasUpdateXml)
        {
            var alertFromIso = GetAlertFromIsoFile(fileName, out hasUpdateXml);
            if (alertFromIso != null)
                return alertFromIso;

            // couldn't find an alert from the information in the iso file, try matching by name
            return Updates.FindPatchAlertByName(Path.GetFileNameWithoutExtension(fileName));
        }

        private XenServerPatchAlert GetAlertFromIsoFile(string fileName, out bool hasUpdateXml)
        {
            hasUpdateXml = false;

            if (!fileName.EndsWith(Branding.UpdateIso))
                return null;

            var xmlDoc = new XmlDocument();

            try
            {
                using (var isoStream = File.Open(fileName, FileMode.Open))
                {
                    var cd = new CDReader(isoStream, true);
                    if (cd.Exists("Update.xml"))
                    {
                        using (var fileStream = cd.OpenFile("Update.xml", FileMode.Open))
                        {
                            xmlDoc.Load(fileStream);
                            hasUpdateXml = true;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                log.ErrorFormat("Exception while reading the update data from the iso file: {0}", exception.Message);
            }

            var elements = xmlDoc.GetElementsByTagName("update");
            var update = elements.Count > 0 ? elements[0] : null;

            if (update == null || update.Attributes == null)
                return null;

            var uuid = update.Attributes["uuid"];
            return uuid != null ? Updates.FindPatchAlertByUuid(uuid.Value) : null;
        }

        private void PopulatePatchesBox()
        {
            dataGridViewPatches.Rows.Clear();

            var updates = Updates.UpdateAlerts.ToList();

            if (dataGridViewPatches.SortedColumn != null)
            {
                if (dataGridViewPatches.SortedColumn.Index == ColumnUpdate.Index)
                    updates.Sort(Alert.CompareOnTitle);
                else if (dataGridViewPatches.SortedColumn.Index == ColumnDate.Index)
                    updates.Sort(Alert.CompareOnDate);
                else if (dataGridViewPatches.SortedColumn.Index == ColumnDescription.Index)
                    updates.Sort(Alert.CompareOnDescription);

                if (dataGridViewPatches.SortOrder == SortOrder.Descending)
                    updates.Reverse();
            }
            else
            {
                updates.Sort(new NewVersionPriorityAlertComparer());
            }

            foreach (Alert alert in updates)
            {
                var patchAlert = alert as XenServerPatchAlert;

                if (patchAlert != null)
                {
                    PatchGridViewRow row = new PatchGridViewRow(patchAlert);
                    if (!dataGridViewPatches.Rows.Contains(row))
                    {
                        dataGridViewPatches.Rows.Add(row);

                        if (patchAlert.RequiredXenCenterVersion != null)
                        {
                            row.Enabled = false;
                            row.SetToolTip(string.Format(Messages.UPDATES_WIZARD_NEWER_XENCENTER_REQUIRED,
                                patchAlert.RequiredXenCenterVersion.Version));
                        }
                    }
                }
            }
        }

        public override void PageCancelled()
        {
            Updates.RestoreDismissedUpdatesStarted -= Updates_RestoreDismissedUpdatesStarted;
            Updates.CheckForUpdatesStarted -= CheckForUpdates_CheckForUpdatesStarted;
            Updates.CheckForUpdatesCompleted -= CheckForUpdates_CheckForUpdatesCompleted;
        }

        public override bool EnableNext()
        {
            if (CheckForUpdatesInProgress)
            {
                return false;
            }

            if (IsInAutomatedUpdatesMode)
            {
                return true;
            }

            if (downloadUpdateRadioButton.Checked)
            {
                if (dataGridViewPatches.SelectedRows.Count == 1)
                {
                    DataGridViewExRow row = (DataGridViewExRow) dataGridViewPatches.SelectedRows[0];
                    if (row.Enabled)
                    {
                        return true;
                    }
                }
            }
            else if (selectFromDiskRadioButton.Checked)
            {
                if (WizardHelpers.IsValidFile(FilePath))
                    return true;
            }

            return false;
        }

        public override bool EnablePrevious()
        {
            return !CheckForUpdatesInProgress;
        }

        /// <summary>
        /// List to store unzipped files to be removed later by PatchingWizard
        /// </summary>
        private List<string> unzippedFiles = new List<string>();

        public List<string> UnzippedUpdateFiles
        {
            get { return unzippedFiles; }
        }

        public string FilePath
        {
            get { return fileNameTextBox.Text; }
            set { fileNameTextBox.Text = value; }
        }

        public UpdateType SelectedUpdateType { get; set; }

        public Pool_patch SelectedExistingPatch { get; set; }

        public string SelectedNewPatch
        {
            get
            {               
                if (selectFromDiskRadioButton.Checked)
                {
                    return SelectedUpdateType == UpdateType.Legacy || SelectedUpdateType == UpdateType.ISO
                        ? WizardHelpers.IsValidFile(unzippedUpdateFilePath) && Path.GetExtension(FilePath).ToLowerInvariant().Equals(".zip")
                            ? unzippedUpdateFilePath
                            : FilePath
                        : null;
                }
                
                return null;
            }
        }

        #region DataGridView

        private void dataGridViewPatches_SelectionChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void dataGridViewPatches_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            Alert alert1 = ((PatchGridViewRow) dataGridViewPatches.Rows[e.RowIndex1]).UpdateAlert;
            Alert alert2 = ((PatchGridViewRow) dataGridViewPatches.Rows[e.RowIndex2]).UpdateAlert;

            if (e.Column.Index == ColumnDate.Index)
            {
                e.SortResult = DateTime.Compare(alert1.Timestamp, alert2.Timestamp);
                e.Handled = true;
            }
        }

        private void dataGridViewPatches_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // The click is on a column header
            if (e.RowIndex == -1)
            {
                return;
            }
            PatchGridViewRow row = dataGridViewPatches.Rows[e.RowIndex] as PatchGridViewRow;
            if (row != null && e.ColumnIndex == 3)
            {
                row.UpdateAlert.FixLinkAction();
            }
        }

        private void dataGridViewPatches_Enter(object sender, EventArgs e)
        {
            downloadUpdateRadioButton.Checked = true;
            OnPageUpdated();
        }

        private class PatchGridViewRow : DataGridViewExRow, IEquatable<PatchGridViewRow>
        {
            private readonly XenServerPatchAlert _alert;

            private DataGridViewTextBoxCell _nameCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _descriptionCell = new DataGridViewTextBoxCell();
            private DataGridViewTextBoxCell _dateCell = new DataGridViewTextBoxCell();
            private DataGridViewLinkCell _webPageCell = new DataGridViewLinkCell();

            public PatchGridViewRow(XenServerPatchAlert alert)
            {
                _alert = alert;
                Cells.AddRange(_nameCell, _descriptionCell, _dateCell, _webPageCell);

                _nameCell.Value = String.Format(alert.Name);
                _descriptionCell.Value = String.Format(alert.Description);
                _dateCell.Value = HelpersGUI.DateTimeToString(alert.Timestamp.ToLocalTime(), Messages.DATEFORMAT_DMY,
                    true);
                _webPageCell.Value = Messages.PATCHING_WIZARD_WEBPAGE_CELL;
            }

            public XenServerPatchAlert UpdateAlert
            {
                get { return _alert; }
            }

            public bool Equals(PatchGridViewRow other)
            {
                if (other != null && other.UpdateAlert != null && UpdateAlert != null && UpdateAlert.uuid == other.UpdateAlert.uuid)
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
                    return this.Equals((PatchGridViewRow) obj);
                return false;
            }

            public void SetToolTip(string toolTip)
            {
                foreach (var c in Cells)
                {
                    if (c is DataGridViewLinkCell)
                        continue;

                    var cell = c as DataGridViewCell;
                    if (c != null)
                        ((DataGridViewCell) c).ToolTipText = toolTip;
                }
            }
        }

        #endregion


        #region Buttons

        private void RestoreDismUpdatesButton_Click(object sender, EventArgs e)
        {
            dataGridViewPatches.Focus();
            Updates.RestoreDismissedUpdates();
        }

        private void RefreshListButton_Click(object sender, EventArgs e)
        {
            dataGridViewPatches.Focus();
            Updates.CheckForUpdates(true);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            selectFromDiskRadioButton.Checked = true;
            var suppPack = WizardHelpers.GetSuppPackFromDisk(this);
            if (!string.IsNullOrEmpty(suppPack))
                FilePath = suppPack;
            OnPageUpdated();
        }

        #endregion


        #region TextBox

        private void fileNameTextBox_Enter(object sender, EventArgs e)
        {
            selectFromDiskRadioButton.Checked = true;
            OnPageUpdated();
        }

        private void fileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            selectFromDiskRadioButton.Checked = true;
            OnPageUpdated();
        }

        #endregion


        #region RadioButtons

        private void AutomaticRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void downloadUpdateRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            dataGridViewPatches.HideSelection = !downloadUpdateRadioButton.Checked;
            if (downloadUpdateRadioButton.Checked)
                dataGridViewPatches.Focus();
            OnPageUpdated();
        }

        private void selectFromDiskRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void AutomaticRadioButton_TabStopChanged(object sender, EventArgs e)
        {
            if (!AutomatedUpdatesRadioButton.TabStop)
                AutomatedUpdatesRadioButton.TabStop = true;
        }

        private void downloadUpdateRadioButton_TabStopChanged(object sender, EventArgs e)
        {
            if (!downloadUpdateRadioButton.TabStop)
                downloadUpdateRadioButton.TabStop = true;
        }

        private void selectFromDiskRadioButton_TabStopChanged(object sender, EventArgs e)
        {
            if (!selectFromDiskRadioButton.TabStop)
                selectFromDiskRadioButton.TabStop = true;
        }

        #endregion
    }        

    public enum UpdateType { Legacy, Existing, ISO}
}
