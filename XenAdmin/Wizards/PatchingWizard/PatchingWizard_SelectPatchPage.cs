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
using XenAdmin.Controls;
using XenAdmin.Core;
using System.IO;
using XenAdmin.Dialogs;
using XenAdmin.Alerts;
using System.Xml;
using DiscUtils.Iso9660;
using XenCenterLib;


namespace XenAdmin.Wizards.PatchingWizard
{
    public partial class PatchingWizard_SelectPatchPage : XenTabPage
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public XenServerPatchAlert AlertFromFileOnDisk;
        public bool FileFromDiskHasUpdateXml { get; private set; }
        private string unzippedUpdateFilePath;

        public PatchingWizard_SelectPatchPage()
        {
            InitializeComponent();
        }

        public override string Text => Messages.PATCHINGWIZARD_SELECTPATCHPAGE_TEXT;

        public override string PageTitle => Messages.PATCHINGWIZARD_SELECTPATCHPAGE_TITLE;

        public override string HelpID => "SelectUpdate";

        public KeyValuePair<XenServerPatch, string> PatchFromDisk { get; private set; }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
            {
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

        public override bool EnableNext()
        {
            return WizardHelpers.IsValidFile(FilePath, out _);
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


        private void BrowseButton_Click(object sender, EventArgs e)
        {
            var suppPack = WizardHelpers.GetSuppPackFromDisk(this);
            if (!string.IsNullOrEmpty(suppPack))
                FilePath = suppPack;
            OnPageUpdated();
        }

        private void fileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }
    }

    public enum UpdateType { Legacy, ISO }
}
