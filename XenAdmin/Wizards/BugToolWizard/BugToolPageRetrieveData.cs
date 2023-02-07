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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Dialogs;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Wizards.BugToolWizard
{
    public partial class BugToolPageRetrieveData : XenTabPage
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int MAX_DOWNLOADS_PER_CONNECTION = 3;
        public string OutputFile { get; set; }

        public BugToolPageRetrieveData()
        {
            InitializeComponent();
            labelBlurb.Text = string.Format(labelBlurb.Text, BrandManager.BrandConsole);
            labelBlurbCis.Text = string.Format(labelBlurbCis.Text, BrandManager.Cis);
            linkLabelBlurb.Text = InvisibleMessages.CIS_URL;
        }

        #region XenTabPage overrides

        public override string Text => Messages.BUGTOOL_PAGE_RETRIEVEDATA_TEXT;

        public override string PageTitle => Messages.BUGTOOL_PAGE_RETRIEVEDATA_PAGE_TITLE;

        public override string HelpID => "CompileReport";

        public override bool EnableNext()
        {
            var allCompleted = AllActionsCompleted(out var successExists, out _, out var packageStatusReportFailed, considerDownloadReportRow: _packagedReport);
            return allCompleted && successExists && !packageStatusReportFailed;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction == PageLoadedDirection.Forward)
                RunActions();
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            _packagedReport = false;
            if (direction == PageLoadedDirection.Forward)
                return;

            CancelActions(ref cancel);
        }

        public override void PageCancelled(ref bool cancel)
        {
            CancelActions(ref cancel);
        }

        #endregion

        #region Properties
        public List<Host> SelectedHosts { private get; set; }
        public List<Capability> CapabilityList { private get; set; }
        public string OutputFolder { get; private set; }
        private bool _packagedReport;
        #endregion

        /// <summary>
        /// Check if all actions attached to the rows in the DataGridView have been run (successfully or otherwise).
        /// </summary>
        /// <param name="successExists">True if at least one row actions has <see cref="StatusReportRow.IsSuccessful"/> set to True</param>
        /// <param name="failureExists">True if at least one row actions has failed</param>
        /// <param name="packageStatusReportFailed">True if the packaging status report action has failed</param>
        /// <param name="considerDownloadReportRow">If true, <see cref="PackageStatusReportRow"/> actions will contribute to the check.
        /// If false, <see cref="PackageStatusReportRow"/> will be ignored and the method will ignore their state.</param>
        /// <returns>True if all actions are in a successful state.</returns>
        private bool AllActionsCompleted(out bool successExists, out bool failureExists, out bool packageStatusReportFailed, bool considerDownloadReportRow = false)
        {
            var allComplete = true;
            successExists = false;
            failureExists = false;
            packageStatusReportFailed = false;

            foreach (var row in dataGridViewEx1.Rows)
            {
                var srRow = (StatusReportRow)row;

                if (!considerDownloadReportRow && srRow is PackageStatusReportRow)
                {
                    continue;
                }

                if (!srRow.IsCompleted)
                {
                    allComplete = false;
                }
                else if (srRow.IsSuccessful)
                {
                    successExists = true;
                }
                else if (srRow is PackageStatusReportRow)
                {
                    packageStatusReportFailed = true;
                    failureExists = true;
                }
                else
                {
                    failureExists = true;
                }
            }
            return allComplete;
        }

        /// <summary>
        /// Run all non-completed actions that are still queued, if they can be run.
        /// Heuristic is based on the result of <see cref="CanRunRowAction"/>.
        /// </summary>
        /// <param name="considerDownloadReportRow">Needs to be set to true if you want to also actions queued under <see cref="PackageStatusReportRow"/> rows</param>
        private void RunQueuedActions(bool considerDownloadReportRow = false)
        {
            foreach (var row in dataGridViewEx1.Rows)
            {
                var srRow = (StatusReportRow)row;

                if (!considerDownloadReportRow && srRow is PackageStatusReportRow)
                {
                    continue;
                }

                if (!srRow.IsCompleted && CanRunRowAction(srRow))
                {
                    RegisterRowEvents(srRow);
                    RunRowAction(srRow);
                }
            }
        }

        private void CancelActions(ref bool cancel)
        {
            var allCompleted = AllActionsCompleted(out _, out _, out _, considerDownloadReportRow: true);
            if (allCompleted)
                return;

            using (var warningDialog = new WarningDialog(Messages.BUGTOOL_PAGE_RETRIEVEDATA_CONFIRM_CANCEL,
                ThreeButtonDialog.ButtonYes, ThreeButtonDialog.ButtonNo)
                {WindowTitle = Messages.BUGTOOL_PAGE_RETRIEVEDATA_PAGE_TITLE})
            {
                if (warningDialog.ShowDialog(this) != DialogResult.Yes)
                {
                    cancel = true;
                    return;
                }
            }

            foreach (var r in dataGridViewEx1.Rows)
            {
                if (r is StatusReportRow row)
                {
                    row.DeRegisterEvents();
                    DeRegisterRowEvents(row);
                    row.CancelAction();
                }
            }
        }

        private void RunActions()
        {
            try
            {
                labelError.Text = "";
                progressBar1.Value = 0;
                dataGridViewEx1.SuspendLayout();
                dataGridViewEx1.Rows.Clear();
                OutputFolder = CreateOutputFolder();

                var capabilityKeys = new List<string>();
                long size = 0;
                foreach (Capability c in CapabilityList)
                {
                    if (c.Key != "client-logs")
                        size += c.MaxSize;

                    capabilityKeys.Add(c.Key);
                }

                var rowList = new List<DataGridViewRow>();

                var includeClientLogs = capabilityKeys.Contains("client-logs");
                if (includeClientLogs || SelectedHosts.Count > 0)
                    rowList.Add(new ClientSideDataRow(SelectedHosts, includeClientLogs));

                foreach (var host in SelectedHosts)
                {
                    rowList.Add(new HostStatusRow(host, size, capabilityKeys));
                }

                rowList.Add(new PackageStatusReportRow(OutputFile));

                dataGridViewEx1.Rows.AddRange(rowList.ToArray());
            }
            finally
            {
                dataGridViewEx1.ResumeLayout();
            }

            foreach (var r in dataGridViewEx1.Rows)
            {
                var row = r as StatusReportRow;

                if (!CanRunRowAction(row))
                {
                    continue;
                }

                RegisterRowEvents(row);

                // PackageStatusReportRow must be run at the very end in a synchronous manner.
                // this is handled within the RowStatusCompleted method
                if (!(row is PackageStatusReportRow))
                {
                    RunRowAction(row);
                }
            }

            OnPageUpdated();
        }

        private bool CanRunRowAction(StatusReportRow row)
        {
            if (row.Action != null)
                return false;

            return row is ClientSideDataRow ||
                   row is PackageStatusReportRow ||
                   row is HostStatusRow hostRow &&
                   dataGridViewEx1.Rows.Cast<StatusReportRow>().Count(r =>
                       r is HostStatusRow hsr && hsr.Connection == hostRow.Connection &&
                       hsr.Action != null && !hsr.IsCompleted) < MAX_DOWNLOADS_PER_CONNECTION;
        }

        private void RunRowAction(StatusReportRow row)
        {
            row.RunAction(OutputFolder, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        }

        private void RunPackageStatusReportAction(bool successExists)
        {
            var lastRow = (StatusReportRow)dataGridViewEx1.Rows[dataGridViewEx1.Rows.Count - 1];

            if (!successExists)
            {
                // no part of the report was generated, we don't need to
                // start the packaging action at all
                lastRow.CancelAction();
            }
            else
            {
                RunRowAction(lastRow);
            }
            _packagedReport = true;
        }

        private void Row_RowStatusChanged(StatusReportRow row)
        {
            UpdateTotalPercentComplete();
            OnPageUpdated();
        }

        private void Row_RowStatusCompleted(StatusReportRow row)
        {
            DeRegisterRowEvents(row);
            UpdateTotalPercentComplete();

            var allCompleted = AllActionsCompleted(out var successExists, out var failureExists, out var packageStatusReportFailed, considerDownloadReportRow: _packagedReport);

            if (allCompleted && !_packagedReport)
            {
                RunPackageStatusReportAction(successExists);
            }
            else if (allCompleted)
            {
                if (packageStatusReportFailed)
                {
                    labelError.Text = Messages.ACTION_SYSTEM_STATUS_SAVE_FAILED;
                }
                else if (!successExists)
                {
                    labelError.Text = Messages.ACTION_SYSTEM_STATUS_COMPILE_FAILED;
                }
                else if (!failureExists)
                {
                    labelError.Text = Messages.ACTION_SYSTEM_STATUS_COMPILE_SUCCESSFUL;
                }
                else
                {
                    labelError.Text = Messages.ACTION_SYSTEM_STATUS_COMPILE_PARTIAL;
                }
                
                _packagedReport = false;
            }
            else
            {
                RunQueuedActions(_packagedReport);
            }

            OnPageUpdated();
        }

        private void RegisterRowEvents(StatusReportRow row)
        {
            row.RowStatusChanged += Row_RowStatusChanged;
            row.RowStatusCompleted += Row_RowStatusCompleted;
        }

        private void DeRegisterRowEvents(StatusReportRow row)
        {
            row.RowStatusChanged -= Row_RowStatusChanged;
            row.RowStatusCompleted -= Row_RowStatusCompleted;
        }

        private void UpdateTotalPercentComplete()
        {
            int total = 0;
            foreach (var r in dataGridViewEx1.Rows)
            {
                var row = (StatusReportRow)r;
                total += row.PercentComplete;
            }

            var percentage = total / dataGridViewEx1.RowCount;

            if (percentage < 0)
                percentage = 0;
            if (percentage > 100)
                percentage = 100;

            if (percentage > progressBar1.Value)
                progressBar1.Value = percentage;
        }

        private static string CreateOutputFolder()
        {
            var folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            if (Directory.Exists(folder))
                Directory.Delete(folder);
            Directory.CreateDirectory(folder);

            return folder;
        }

        private void linkLabelBlurb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(InvisibleMessages.CIS_URL);
            }
            catch (Exception exception)
            {
                Log.Error($"Error while opening {InvisibleMessages.CIS_URL}.", exception);
                using (var dlg = new ErrorDialog(string.Format(Messages.COULD_NOT_OPEN_URL, InvisibleMessages.CIS_URL)))
                    dlg.ShowDialog(Program.MainWindow);
            }
        }
    }
}
