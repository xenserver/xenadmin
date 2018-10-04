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

using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Dialogs;


namespace XenAdmin.Wizards
{
    public static class WizardHelpers
    {
        public static string GetSuppPackFromDisk(Control control)
        {
            string oldDir = string.Empty;
            try
            {
                oldDir = Directory.GetCurrentDirectory();
                using (var dlg = new OpenFileDialog
                {
                    Multiselect = false,
                    ShowReadOnly = false,
                    Filter = string.Format(Messages.PATCHINGWIZARD_SELECTPATCHPAGE_UPDATESEXT, Branding.Update),
                    FilterIndex = 0,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    ShowHelp = false,
                    Title = Messages.PATCHINGWIZARD_SELECTPATCHPAGE_CHOOSE
                })
                {
                    dlg.FileOk += dlg_FileOk;

                    if (dlg.ShowDialog(control) == DialogResult.OK)
                        return dlg.FileName;
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(oldDir);
            }

            return null;
        }

        private static void dlg_FileOk(object sender, CancelEventArgs e)
        {
            var dlg = sender as OpenFileDialog;
            if (dlg == null)
                return;

            if (!IsValidFile(dlg.FileName))
                using (var popup = new ThreeButtonDialog(new ThreeButtonDialog.Details(
                    SystemIcons.Error, string.Format(Messages.UPDATES_WIZARD_NOTVALID_EXTENSION, Branding.Update), Messages.UPDATES)))
                {
                    popup.ShowDialog();
                    e.Cancel = true;
                }
        }       

        public static void ParseSuppPackFile(string path, Control control, ref bool cancel, out string suppPackPath)
        {
            string unzippedPath;

            if (Path.GetExtension(path).ToLowerInvariant().Equals(".zip"))
            {
                unzippedPath = ExtractUpdate(path, control);
                if (unzippedPath == null)
                    cancel = true;
            }
            else
                unzippedPath = null;

            var fileName = IsValidFile(unzippedPath)
                ? unzippedPath.ToLowerInvariant()
                : path.ToLowerInvariant();

            if (IsValidFile(fileName))
            {
                if (!fileName.EndsWith("." + Branding.Update)
                    && !fileName.EndsWith("." + Branding.UpdateIso)
                    && !cancel)
                {
                    using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(
                        SystemIcons.Error,
                        string.Format(Messages.UPDATES_WIZARD_NOTVALID_ZIPFILE, Path.GetFileName(fileName)),
                        Messages.UPDATES)))
                    {
                        dlg.ShowDialog(control);
                    }
                    cancel = true;
                }
                suppPackPath = fileName;
            }
            else
                suppPackPath = string.Empty;
        }

        public static string ExtractUpdate(string zippedUpdatePath, Control control)
        {
            var unzipAction =
                new DownloadAndUnzipXenServerPatchAction(Path.GetFileNameWithoutExtension(zippedUpdatePath), null,
                    zippedUpdatePath, true, Branding.Update, Branding.UpdateIso);
            using (var dlg = new ActionProgressDialog(unzipAction, ProgressBarStyle.Marquee))
            {
                dlg.ShowDialog(control);
            }

            if (string.IsNullOrEmpty(unzipAction.PatchPath))
            {
                using (var dlg = new ThreeButtonDialog(new ThreeButtonDialog.Details(
                    SystemIcons.Error,
                    string.Format(Messages.UPDATES_WIZARD_NOTVALID_ZIPFILE, Path.GetFileName(zippedUpdatePath)),
                    Messages.UPDATES)))
                {
                    dlg.ShowDialog(control);
                }
                return null;
            }
            else
            {
                return unzipAction.PatchPath;
            }
        }

        public static bool IsValidFile(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) && File.Exists(fileName)
                && (fileName.ToLowerInvariant().EndsWith("." + Branding.Update.ToLowerInvariant())
                || fileName.ToLowerInvariant().EndsWith(".zip")
                || fileName.ToLowerInvariant().EndsWith(".iso")); //this iso is supplemental pack iso for XS, not branded
        }
    }
}
