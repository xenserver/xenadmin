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
using XenAdmin.Controls;
using XenAdmin.Controls.DataGridViewEx;
using XenAdmin.Core;
using XenAPI;
using System.IO;
using XenAdmin.Dialogs;
using XenAdmin.Alerts;
using System.Linq;
using System.Xml;
using DiscUtils.Iso9660;
using XenCenterLib;


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_SelectPatchPage : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event Action UpdateAlertFromWebSelected;

        private bool CheckForUpdatesInProgress;
        public XenServerPatchAlert UpdateAlertFromWeb;
        public XenServerPatchAlert AlertFromFileOnDisk;
        public bool FileFromDiskHasUpdateXml { get; private set; }
        private bool firstLoad = true;
        private string unzippedUpdateFilePath;

        public PatchingWizard_SelectPatchPage()
        {
            InitializeComponent();

            automatedUpdatesOptionLabel.Text = string.Format(automatedUpdatesOptionLabel.Text, BrandManager.BrandConsole, BrandManager.CompanyNameShort);
            
            tableLayoutPanelSpinner.Visible = false;

            labelWithAutomatedUpdates.Visible =
                automatedUpdatesOptionLabel.Visible = AutomatedUpdatesRadioButton.Visible = false;
            downloadUpdateRadioButton.Checked = true;
        }

        private void RegisterEvents()
        {
            Updates.CheckForServerUpdatesStarted += CheckForUpdates_CheckForUpdatesStarted;
            Updates.CheckForServerUpdatesCompleted += CheckForUpdates_CheckForUpdatesCompleted;
            Updates.RestoreDismissedUpdatesStarted += Updates_RestoreDismissedUpdatesStarted;
        }

        private void UnRegisterEvents()
        {
            Updates.RestoreDismissedUpdatesStarted -= Updates_RestoreDismissedUpdatesStarted;
            Updates.CheckForServerUpdatesStarted -= CheckForUpdates_CheckForUpdatesStarted;
            Updates.CheckForServerUpdatesCompleted -= CheckForUpdates_CheckForUpdatesCompleted;
        }
        
        private void CheckForUpdates_CheckForUpdatesStarted()
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                StartCheckForUpdates(); //call this before setting CheckForUpdatesInProgress
                CheckForUpdatesInProgress = true;
            });
        }

        private void Updates_RestoreDismissedUpdatesStarted()
        {
            Program.Invoke(Program.MainWindow, () =>
            {
                StartCheckForUpdates(); //call this before setting CheckForUpdatesInProgress
                CheckForUpdatesInProgress = true;
            });
        }

        private void CheckForUpdates_CheckForUpdatesCompleted(bool succeeded, string errorMessage)
        {
            Program.Invoke(Program.MainWindow, ()=>
            {
                CheckForUpdatesInProgress = false;
                FinishCheckForUpdates(); //call this after setting CheckForUpdatesInProgress
            });
        }

        private void StartCheckForUpdates()
        {
            if (CheckForUpdatesInProgress || _backgroundWorker.IsBusy)
                return;

            dataGridViewPatches.Rows.Clear();
            tableLayoutPanelSpinner.Visible = true;
            RestoreDismUpdatesButton.Enabled = false;
            RefreshListButton.Enabled = false;
            OnPageUpdated();
        }

        private void FinishCheckForUpdates()
        {
            if (CheckForUpdatesInProgress || _backgroundWorker.IsBusy)
                return;

            tableLayoutPanelSpinner.Visible = false;
            PopulatePatchesBox();
            RefreshListButton.Enabled = true;
            RestoreDismUpdatesButton.Enabled = true;
            OnPageUpdated();
        }

        public override string Text => Messages.PATCHINGWIZARD_SELECTPATCHPAGE_TEXT;

        public override string PageTitle => Messages.PATCHINGWIZARD_SELECTPATCHPAGE_TITLE;

        public override string HelpID => "SelectUpdate";

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            RegisterEvents();

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
                    if (automatedUpdatesPossible && UpdateAlertFromWeb == null)
                        AutomatedUpdatesRadioButton.Checked = true;
                    else if (!string.IsNullOrEmpty(FilePath))
                        selectFromDiskRadioButton.Checked = true;
                    else
                        downloadUpdateRadioButton.Checked = true;
                }
                else if (!automatedUpdatesPossible && AutomatedUpdatesRadioButton.Checked)
                {
                    downloadUpdateRadioButton.Checked = true;
                }

                StartCheckForUpdates(); //call this before starting the _backgroundWorker
                _backgroundWorker.RunWorkerAsync();
            }

            firstLoad = false;
        }

        private bool IsInAutomatedUpdatesMode =>
            AutomatedUpdatesRadioButton.Visible && AutomatedUpdatesRadioButton.Checked;

        public WizardMode WizardMode
        {
            get
            {
                if (AutomatedUpdatesRadioButton.Visible && AutomatedUpdatesRadioButton.Checked)
                    return WizardMode.AutomatedUpdates;
                var updateAlert = downloadUpdateRadioButton.Checked
                    ? UpdateAlertFromWeb
                    : selectFromDiskRadioButton.Checked
                        ? AlertFromFileOnDisk
                        : null;
                if (updateAlert != null && updateAlert.NewServerVersion != null)
                    return WizardMode.NewVersion;
                return WizardMode.SingleUpdate;
            }
        }

        public KeyValuePair<XenServerPatch, string> PatchFromDisk { get; private set; }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
            {
                if ((IsInAutomatedUpdatesMode || downloadUpdateRadioButton.Checked) &&
                    !Updates.CheckCanDownloadUpdates())
                {
                    cancel = true;
                    using (var errDlg = new ClientIdDialog())
                        errDlg.ShowDialog(ParentForm);
                    return;
                }

                if (IsInAutomatedUpdatesMode)
                {
                    if (!Updates.CheckForServerUpdates(userRequested: true, asynchronous: false, this))
                    {
                        cancel = true;
                        return;
                    }
                }
                else if (downloadUpdateRadioButton.Checked)
                {
                    UpdateAlertFromWeb = dataGridViewPatches.SelectedRows.Count > 0
                        ? ((PatchGridViewRow) dataGridViewPatches.SelectedRows[0]).UpdateAlert
                        : null;

                    var distinctHosts = UpdateAlertFromWeb?.DistinctHosts;
                    SelectedUpdateType = distinctHosts != null && distinctHosts.Any(Helpers.ElyOrGreater)
                        ? UpdateType.ISO
                        : UpdateType.Legacy;

                    AlertFromFileOnDisk = null;
                    FileFromDiskHasUpdateXml = false;
                    unzippedUpdateFilePath = null;
                    SelectedPatchFilePath = null;
                    PatchFromDisk = new KeyValuePair<XenServerPatch, string>(null, null);
                }
                else
                {
                    UpdateAlertFromWeb = null;
                    SelectedPatchFilePath = null;

                    if (!WizardHelpers.IsValidFile(FilePath, out var pathFailure))
                        using (var dlg = new ErrorDialog(pathFailure) {WindowTitle = Messages.UPDATES})
                        {
                            cancel = true;
                            dlg.ShowDialog();
                            return;
                        }

                    SelectedPatchFilePath = FilePath;

                    if (Path.GetExtension(FilePath).ToLowerInvariant().Equals(".zip"))
                    {
                        //check if we are installing the update the user sees in the textbox
                        if (unzippedUpdateFilePath == null || !File.Exists(unzippedUpdateFilePath) ||
                            Path.GetFileNameWithoutExtension(unzippedUpdateFilePath) != Path.GetFileNameWithoutExtension(FilePath))
                        {
                            unzippedUpdateFilePath = WizardHelpers.ExtractUpdate(FilePath, this);
                        }

                        if (!WizardHelpers.IsValidFile(unzippedUpdateFilePath, out var zipFailure))
                        {
                            using (var dlg = new ErrorDialog(zipFailure) {WindowTitle = Messages.UPDATES})
                            {
                                cancel = true;
                                dlg.ShowDialog();
                                return;
                            }
                        }
                        
                        if (!UnzippedUpdateFiles.Contains(unzippedUpdateFilePath))
                            UnzippedUpdateFiles.Add(unzippedUpdateFilePath);

                        SelectedPatchFilePath = unzippedUpdateFilePath;
                    }
                    else
                        unzippedUpdateFilePath = null;

                    if (SelectedPatchFilePath.ToLowerInvariant().EndsWith($".{BrandManager.ExtensionUpdate.ToLowerInvariant()}"))
                        SelectedUpdateType = UpdateType.Legacy;
                    else if (SelectedPatchFilePath.ToLowerInvariant().EndsWith(".iso"))
                        SelectedUpdateType = UpdateType.ISO;

                    AlertFromFileOnDisk = GetAlertFromFile(SelectedPatchFilePath, out var hasUpdateXml, out var isUpgradeIso);

                    if (isUpgradeIso)
                    {
                        using (var dlg = new ErrorDialog(Messages.PATCHINGWIZARD_SELECTPATCHPAGE_ERROR_MAINISO))
                            dlg.ShowDialog(this);

                        cancel = true;
                        return;
                    }

                    FileFromDiskHasUpdateXml = hasUpdateXml;
                    PatchFromDisk = AlertFromFileOnDisk == null
                        ? new KeyValuePair<XenServerPatch, string>(null, null)
                        : new KeyValuePair<XenServerPatch, string>(AlertFromFileOnDisk.Patch, SelectedPatchFilePath);
                }
            }

            if (!cancel) //unsubscribe only if we are really leaving this page
                UnRegisterEvents();
        }

        private XenServerPatchAlert GetAlertFromFile(string fileName, out bool hasUpdateXml, out bool isUpgradeIso)
        {
            var alertFromIso = GetAlertFromIsoFile(fileName, out hasUpdateXml, out isUpgradeIso);
            if (alertFromIso != null)
                return alertFromIso;

            // couldn't find an alert from the information in the iso file, try matching by name
            return Updates.FindPatchAlertByName(Path.GetFileNameWithoutExtension(fileName));
        }

        private XenServerPatchAlert GetAlertFromIsoFile(string fileName, out bool hasUpdateXml, out bool isUpgradeIso)
        {
            hasUpdateXml = false;
            isUpgradeIso = false;

            if (!fileName.ToLowerInvariant().EndsWith(".iso"))
                return null;

            var xmlDoc = new XmlDocument();

            try
            {
                using (var isoStream = File.OpenRead(fileName))
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

                    if (cd.Exists(@"repodata\repomd.xml") && cd.FileExists(".treeinfo"))
                    {
                        using (var fileStream = cd.OpenFile(".treeinfo", FileMode.Open))
                        {
                            var iniDoc = new IniDocument(fileStream);
                            var entry = iniDoc.FindEntry("platform", "name");
                            if (entry != null && entry.Value == "XCP")
                            {
                                isUpgradeIso = true;
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error("Exception while reading the update data from the iso file:", exception);
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
            try
            {
                var updates = Updates.UpdateAlerts;

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

                dataGridViewPatches.SuspendLayout();
                dataGridViewPatches.Rows.Clear();

                var rowList = new List<DataGridViewRow>();

                foreach (Alert alert in updates)
                {
                    if (!(alert is XenServerPatchAlert patchAlert))
                        continue;

                    PatchGridViewRow row = new PatchGridViewRow(patchAlert);
                    if (!rowList.Contains(row))
                    {
                        rowList.Add(row);

                        if (patchAlert.RequiredClientVersion != null)
                        {
                            row.Enabled = false;
                            row.SetToolTip(string.Format(Messages.UPDATES_WIZARD_NEWER_XENCENTER_REQUIRED,
                                BrandManager.BrandConsole, patchAlert.RequiredClientVersion.Version));
                        }
                    }
                }

                dataGridViewPatches.Rows.AddRange(rowList.ToArray());

                if (UpdateAlertFromWeb != null)
                {
                    var foundRow = dataGridViewPatches.Rows.Cast<PatchGridViewRow>()
                        .FirstOrDefault(r => r.UpdateAlert.Equals(UpdateAlertFromWeb));
                    if (foundRow != null)
                    {
                        foundRow.Selected = true;
                        UpdateAlertFromWebSelected?.Invoke();
                    }
                }
            }
            finally
            {
                dataGridViewPatches.ResumeLayout();
            }
        }

        public override void PageCancelled(ref bool cancel)
        {
            UnRegisterEvents();
            
            if (_backgroundWorker.IsBusy)
                _backgroundWorker.CancelAsync();
        }

        public override bool EnableNext()
        {
            if (CheckForUpdatesInProgress || _backgroundWorker.IsBusy)
                return false;

            if (IsInAutomatedUpdatesMode)
                return true;

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
                if (WizardHelpers.IsValidFile(FilePath, out _))
                    return true;
            }

            return false;
        }

        public override bool EnablePrevious()
        {
            return !CheckForUpdatesInProgress && !_backgroundWorker.IsBusy;
        }

        /// <summary>
        /// List to store unzipped files to be removed later by PatchingWizard
        /// </summary>
        public List<string> UnzippedUpdateFiles { get; } = new List<string>();

        public string FilePath
        {
            get => fileNameTextBox.Text;
            set => fileNameTextBox.Text = value;
        }

        public UpdateType SelectedUpdateType { get; private set; }

        public string SelectedPatchFilePath { get; private set; }

        private void _backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Updates.RefreshUpdateAlerts(Updates.UpdateType.ServerPatches);
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            FinishCheckForUpdates();
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
            private readonly DataGridViewTextBoxCell _nameCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _descriptionCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewTextBoxCell _dateCell = new DataGridViewTextBoxCell();
            private readonly DataGridViewLinkCell _webPageCell = new DataGridViewLinkCell();

            public PatchGridViewRow(XenServerPatchAlert alert)
            {
                UpdateAlert = alert;
                Cells.AddRange(_nameCell, _descriptionCell, _dateCell, _webPageCell);

                _nameCell.Value = String.Format(alert.Name);
                _descriptionCell.Value = String.Format(alert.Description);
                _dateCell.Value = HelpersGUI.DateTimeToString(alert.Timestamp.ToLocalTime(), Messages.DATEFORMAT_DMY,
                    true);
                _webPageCell.Value = Messages.PATCHING_WIZARD_WEBPAGE_CELL;
                _webPageCell.ToolTipText = alert.WebPageLabel;
            }

            public XenServerPatchAlert UpdateAlert { get; }

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
                if (obj is PatchGridViewRow row)
                    return Equals(row);
                return false;
            }

            public void SetToolTip(string toolTip)
            {
                foreach (var c in Cells)
                {
                    if (c is DataGridViewLinkCell)
                        continue;

                    if (c is DataGridViewCell cell)
                        cell.ToolTipText = toolTip;
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
            Updates.CheckForServerUpdates(userRequested: true);
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

    public enum UpdateType { Legacy, ISO }
}
