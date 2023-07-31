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
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls.CheckableDataGridView;
using XenAdmin.Controls.SummaryPanel;
using XenAdmin.Core;
using XenAdmin.Dialogs.LicenseManagerSelectionVerifiers;
using XenAdmin.Network;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public class LicenseManagerController
    {
        public ILicenseManagerView View { private get; set; }

        public LicenseManagerController()
        {
            VerifierFactory = new LicenseSelectionVerifierFactory();
            ReadOnlyView = HiddenFeatures.LicenseOperationsHidden; 
        }

        public LicenseManagerController(ILicenseManagerView view)
        {
            View = view;
        }

        public SelectionVerifierFactory VerifierFactory { private get; set; }

        public bool ReadOnlyView { get; private set; }

        private void AddToGrid(List<IXenObject> dataToDraw)
        {
            View.DrawRowsInGrid(ConvertXenObjects(dataToDraw));
        }

        public void PopulateGrid(List<IXenObject> itemsToShow, List<IXenObject> selectedItems)
        {
            if(itemsToShow.Count < 1)
            {
                DisableAllButtons();
                return;
            }

            var allItemsToShow = itemsToShow.ToList();

            AddToGrid(allItemsToShow);

            foreach (LicenseDataGridViewRow row in ConvertXenObjects(allItemsToShow).ConvertAll(r => r as LicenseDataGridViewRow))
            {
                UpdateButtonEnablement(new List<LicenseDataGridViewRow>{row});
            }
            CheckPreSelectedRows(selectedItems);
            SelectAndSummariseSelectedRow(allItemsToShow, selectedItems);
        }

        public void Repopulate(List<IXenObject> itemsToShow, List<IXenObject> selectedItems)
        {
            View.ClearAllGridRows();
            PopulateGrid(itemsToShow, selectedItems);
        }

        private void CheckPreSelectedRows(List<IXenObject> dataToCheck)
        {
            if (dataToCheck.Count < 1 || ReadOnlyView)
            {
                DisableAllButtons();
                return;
            }

            View.DrawSelectedRowsAsChecked(ConvertXenObjects(dataToCheck).Where(r=>!r.Disabled).ToList());
            UpdateButtonEnablement();
        }

        private void SetSummaryInformation(string information)
        {
            if(String.IsNullOrEmpty(information))
                View.DrawSummaryInformation(String.Empty, false);
            View.DrawSummaryInformation(information, true);
        }

        public void SummariseSelectedRow(CheckableDataGridViewRow dataToSummarise)
        {
            if(!dataToSummarise.XenObject.Connection.IsConnected)
            {
                View.DrawSummaryForHighlightedRow(dataToSummarise, new LicenseManagerSummaryComponent(), LaunchUrl(InvisibleMessages.LICENSE_BUY_URL), LaunchUrl(InvisibleMessages.CSS_URL));
                SetSummaryInformation(Messages.POOL_OR_HOST_IS_NOT_CONNECTED);
                return;
            }

            SummaryTextComponent component = BuildSummaryComponent(dataToSummarise);
            View.DrawSummaryForHighlightedRow(dataToSummarise, component, LaunchUrl(InvisibleMessages.LICENSE_BUY_URL), LaunchUrl(InvisibleMessages.CSS_URL));
            if(dataToSummarise.Disabled)
                SetSummaryInformation(dataToSummarise.DisabledReason);
        }

        private void SelectAndSummariseSelectedRow(List<IXenObject> allData, List<IXenObject> selectedFromTree)
        {
            IXenObject xo = selectedFromTree.Count > 0 ? selectedFromTree.FirstOrDefault() : allData.FirstOrDefault();
            View.DrawHighlightedRow(new LicenseDataGridViewRow(xo));
            SummariseSelectedRow(new LicenseDataGridViewRow(xo));
        }

        public void SetStatusIcon(int rowIndex, LicenseDataGridViewRow.Status rowStatus)
        {
            View.DrawRowStatusIcon(rowIndex, rowStatus);
        }

        private void ShowPoolHostNotConnectedError()
        {
            using (var dlg = new ErrorDialog(Messages.SELECTED_HOST_POOL_NOT_CONNECTED))
                dlg.ShowDialog(View.Parent);
        }

        private void SummariseDisconnectedRows(List<CheckableDataGridViewRow> rowsChecked)
        {
            //Refresh current row's details if the pool/host is no longer connected
            CheckableDataGridViewRow row = rowsChecked.FirstOrDefault(r => r.Highlighted && !r.XenObject.Connection.IsConnected);
            if (row != null)
                SummariseSelectedRow(row);
        }

        public void AssignLicense(List<CheckableDataGridViewRow> rowsChecked)
        {
            if (rowsChecked.Any(r => !r.XenObject.Connection.IsConnected))
            {
                ShowPoolHostNotConnectedError();
                SummariseDisconnectedRows(rowsChecked);
                ResetButtonEnablement();
                return;
            }

            var licenseRows = rowsChecked.ConvertAll(r => r as LicenseDataGridViewRow);
            var row = licenseRows.FirstOrDefault();
            var xenObjects = licenseRows.ConvertAll(r => r.XenObject);

            if (row != null && xenObjects.Count > 0)
                using (var ald = new AssignLicenseDialog(xenObjects,
                    row.LicenseServerAddress, row.LicenseServerPort, row.LicenseEdition))
                    ald.ShowDialog(View.Parent);

            SummariseDisconnectedRows(rowsChecked);
            ResetButtonEnablement();
        }

        public void ReleaseLicenses(List<CheckableDataGridViewRow> rowsChecked)
        {
            Debug.Assert(rowsChecked.Count > 0, "There must be one license selected to perform this operation");
            List<LicenseDataGridViewRow> rowsUsingLicenseServer = rowsChecked.ConvertAll(r => r as LicenseDataGridViewRow).Where(
                        r => r.XenObject.Connection.IsConnected && r.HasLicenseServer).ToList();

            if (rowsUsingLicenseServer.Count > 0)
            {
                var action = new ApplyLicenseEditionAction(rowsUsingLicenseServer.ConvertAll(r => r.XenObject),
                    Host.Edition.Free, null, null);
            
                using (var actionProgress = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                    actionProgress.ShowDialog(View.Parent);
            }
            else
            {
                ShowPoolHostNotConnectedError();
            }

            SummariseDisconnectedRows(rowsChecked);
            ResetButtonEnablement();
        }

        public void DownloadLicenseManager()
        {
            LaunchUrl(InvisibleMessages.LICENSE_SERVER_DOWNLOAD_LINK).Invoke();
        }

        private Action LaunchUrl(string url)
        {
            return delegate
            {
                try
                {
                    Process.Start(url);
                }
                catch (Exception)
                {
                    using (var dlg = new ErrorDialog(string.Format(Messages.LICENSE_SERVER_COULD_NOT_OPEN_LINK, url)))
                        dlg.ShowDialog(View.Parent);
                }
            };
        }

        protected virtual IMainWindow CommandInterface
        {
            get { return Program.MainWindow; }
        }

        private void SetRowInformation(List<LicenseDataGridViewRow> rows, string information)
        {
            foreach (LicenseDataGridViewRow row in rows)
            {
                if(row.Disabled)
                    continue;
               
                View.DrawSelectedRowsAsChecked(rows.ConvertAll(r=>r as CheckableDataGridViewRow));
                View.SetRowDisabledRowInfo(row.Index, information, !string.IsNullOrEmpty(information));
            }
        }

        public void UpdateButtonEnablement()
        {
            List<LicenseDataGridViewRow> lRows = View.GetCheckedRows.ConvertAll(r => r as LicenseDataGridViewRow);
            UpdateButtonEnablement(lRows);
        }

        public void UpdateButtonEnablement(List<LicenseDataGridViewRow> lRows)
        {
            //All buttons disabled?
            if(lRows.Count < 1 || ReadOnlyView)
            {
                DisableAllButtons();
                View.DrawViewAsReadOnly(ReadOnlyView);
                return;
            }

            View.DrawViewAsReadOnly(ReadOnlyView);

            LicenseSelectionVerifier verifier;
            verifier = VerifierFactory.Verifier(SelectionVerifierFactory.Option.NotLive, lRows);
            if (verifier.Status  == LicenseSelectionVerifier.VerificationStatus.Error)
            {
                DisableAllButtons();
                SetRowInformation(lRows, verifier.VerificationDetails());
                return;
            }
                
            verifier = VerifierFactory.Verifier(SelectionVerifierFactory.Option.HaOn, lRows);
            if (verifier.Status == LicenseSelectionVerifier.VerificationStatus.Error)
            {
                DisableAllButtons();
                SetRowInformation(lRows, verifier.VerificationDetails());
                return;
            }
                
            View.DrawAssignButtonAsDisabled(verifier.Status == LicenseSelectionVerifier.VerificationStatus.Error);
            View.DrawReleaseButtonAsDisabled(!lRows.Any(r => r.IsUsingLicenseServer || r.CurrentLicenseState == LicenseStatus.HostState.PartiallyLicensed));
        }

        private void DisableAllButtons()
        {
            View.DrawAssignButtonAsDisabled(true);
            View.DrawReleaseButtonAsDisabled(true);
        }

        private void ResetButtonEnablement()
        {
            DisableAllButtons();
            View.DrawSelectedRowsAsChecked(View.GetCheckedRows);
        }

        private SummaryTextComponent BuildSummaryComponent(CheckableDataGridViewRow row)
        {
            LicenseManagerSummaryComponent component = new LicenseManagerSummaryComponent();
            LicenseManagerSummaryDecorator licenseTypeDecorator = new LicenseManagerSummaryLicenseTypeDecorator(component, row);
            LicenseManagerSummaryDecorator licenseSocketsDecorator = new LicenseManagerSummaryLicenseSocketsDecorator(licenseTypeDecorator, row);
            LicenseManagerSummaryDecorator licenseExpiresDecorator = new LicenseManagerSummaryLicenseExpiresDecorator(licenseSocketsDecorator, row);
            LicenseManagerSummaryDecorator licenseServerDecorator = new LicenseManagerSummaryLicenseServerDecorator(licenseExpiresDecorator, row);
            return licenseServerDecorator;
        }

        private List<CheckableDataGridViewRow> ConvertXenObjects(IEnumerable<IXenObject> xenObjects)
        {
            List<CheckableDataGridViewRow> rows = new List<CheckableDataGridViewRow>();
            foreach (IXenObject xenObject in xenObjects)
            {
                rows.Add(new LicenseDataGridViewRow(xenObject));
            }
            return rows;
        }

        private List<Host> RowsToHosts(IEnumerable<LicenseDataGridViewRow> rows)
        {
            List<Host> hosts = new List<Host>();
            if (rows == null)
                return hosts;

            foreach (LicenseDataGridViewRow row in rows)
            {
                if(row.XenObject is Host)
                    hosts.Add(row.XenObject as Host);
                if(row.XenObject is Pool)
                {
                    Pool pool = row.XenObject as Pool;
                    hosts.AddRange(pool.Connection.Cache.Hosts);
                }
            }
            return hosts;
        }

        public void Repopulate()
        {
            Repopulate(GetAllObjects(), new List<IXenObject>());
        }

        private List<IXenObject> GetAllObjects()
        {
            List<IXenObject> allObjects = new List<IXenObject>();
            foreach (IXenConnection conn in ConnectionsManager.XenConnections)
            {
                if (conn == null || !conn.IsConnected)
                    continue;

                Pool pool = Helpers.GetPool(conn);
                if (pool == null)
                    allObjects.AddRange(conn.Cache.Hosts);
                else
                    allObjects.Add(pool);
            }
            return allObjects;
        }
    }
}
