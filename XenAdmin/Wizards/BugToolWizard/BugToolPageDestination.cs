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
using System.IO;
using System.Windows.Forms;
using XenAdmin.Controls;
using XenAdmin.Controls.Common;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenCenterLib;
using XenModel;


namespace XenAdmin.Wizards.BugToolWizard
{
    public partial class BugToolPageDestination : XenTabPage
    {
        private bool _buttonNextEnabled;

        public BugToolPageDestination()
        {
            InitializeComponent();
        }

        public override string Text => Messages.BUGTOOL_PAGE_DESTINATION_TEXT;

        public override string PageTitle => Messages.BUGTOOL_PAGE_DESTINATION_PAGETITLE;

        public override string HelpID => "ReportDestination";

        public override bool EnableNext()
        {
            return _buttonNextEnabled;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            if (direction != PageLoadedDirection.Forward)
                return;

            m_textBoxName.Text =
                $"{Messages.BUGTOOL_FILE_PREFIX}{HelpersGUI.DateTimeToString(DateTime.Now, "yyyy-MM-dd-HH-mm-ss", false)}.zip";

            var initialDirectory = Properties.Settings.Default.ServerStatusPath;
            
            try
            {
                //previous versions were storing the zip file name rather than the directory; check if this is the case
                if (!string.IsNullOrEmpty(initialDirectory) && !Directory.Exists(initialDirectory))
                    initialDirectory = Path.GetDirectoryName(initialDirectory);
            }
            catch
            {
                //ignore
            }

            if (string.IsNullOrEmpty(initialDirectory) || !Directory.Exists(initialDirectory))
                m_textBoxLocation.Text = Win32.GetKnownFolderPath(Win32.KnownFolders.Downloads);
            else
                m_textBoxLocation.Text = initialDirectory;

            PerformCheck(CheckPathValid);
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction != PageLoadedDirection.Forward)
                return;

            if (!PerformCheck(CheckPathValid, CheckDestinationFolderExists))
            {
                cancel = true;
                return;
            }

            if (File.Exists(OutputFile)) //confirm ok to overwrite
            {
                using (var dlg = new WarningDialog(string.Format(Messages.FILE_X_EXISTS_OVERWRITE, OutputFile),
                    ThreeButtonDialog.ButtonOK,
                    new ThreeButtonDialog.TBDButton(Messages.CANCEL, DialogResult.Cancel, selected: true)))
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK)
                    {
                        cancel = true;
                        return;
                    }
                }
            }

            // Check we can write to the destination file - otherwise we only find out at the 
            // end of the ZipStatusReportAction, and the downloaded server files are lost,
            // and the user will have to run the wizard again.
            FileStream stream = null;
            try
            {
                stream = File.OpenWrite(OutputFile);
            }
            catch (Exception exn)
            {
                using (var dlg = new ErrorDialog(string.Format(Messages.COULD_NOT_WRITE_FILE, OutputFile, exn.Message)))
                    dlg.ShowDialog(this);

                cancel = true;
            }
            finally
            {
                stream?.Dispose();
            }

            // Save away the output directory for next time
            Properties.Settings.Default.ServerStatusPath = m_textBoxLocation.Text.Trim();
        }

        public override void SelectDefaultControl()
        {
            if (string.IsNullOrEmpty(m_textBoxLocation.Text))
                m_textBoxLocation.Select();
        }

        public string OutputFile
        {
            get
            {
                var name = m_textBoxName.Text.Trim();
                var folder = m_textBoxLocation.Text.Trim();
                if (!name.EndsWith(".zip"))
                    name = string.Concat(name, ".zip");

                return $@"{folder}\{name}";
            }
        }

        private bool PerformCheck(params CheckDelegate[] checks)
        {
            var success = m_ctrlError.PerformCheck(checks);
            _buttonNextEnabled = success;
            OnPageUpdated();
            return success;
        }

        private bool CheckPathValid(out string error)
        {
            error = string.Empty;

            var name = m_textBoxName.Text.Trim();
            var folder = m_textBoxLocation.Text.Trim();

            if (string.IsNullOrEmpty(name))
                return false;

            if (!PathValidator.IsFileNameValid(name, out var invalidNameMsg))
            {
                error = $"{Messages.BUGTOOL_PAGE_DESTINATION_INVALID_NAME} {invalidNameMsg}";
                return false;
            }

            if (string.IsNullOrEmpty(folder))
                return false;

            var path = $"{folder}\\{name}";

            if (PathValidator.IsPathValid(path, out var invalidPathMsg))
            {
                return true;
            }

            error = $"{Messages.BUGTOOL_PAGE_DESTINATION_INVALID_FOLDER} {invalidPathMsg}";
            return false;
        }

        private bool CheckDestinationFolderExists(out string error)
        {
            error = string.Empty;
            var folder = m_textBoxLocation.Text.Trim();

            if (Directory.Exists(folder))
                return true;

            error = Messages.ERROR_DESTINATION_DIR_NON_EXIST;
            return false;
        }

        #region Control event handlers

        private void m_textBoxName_TextChanged(object sender, EventArgs e)
        {
            PerformCheck(CheckPathValid);
        }

        private void m_textBoxLocation_TextChanged(object sender, EventArgs e)
        {
            PerformCheck(CheckPathValid);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog
            {
                SelectedPath = m_textBoxLocation.Text.Trim(),
                Description = Messages.FOLDER_BROWSER_BUG_TOOL
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    m_textBoxLocation.Text = dialog.SelectedPath;
            }
        }

        #endregion
    }
}
