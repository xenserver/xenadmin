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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Actions.GUIActions;
using XenAdmin.Actions.Updates;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenCenterLib;

namespace XenAdmin.Alerts
{
    public class ClientUpdateAlert : Alert
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly ClientVersion NewVersion;

        public ClientUpdateAlert(ClientVersion version)
        {
            NewVersion = version;
            _timestamp = NewVersion.TimeStamp;
            Checksum = version.Checksum;
        }

        public override AlertPriority Priority => AlertPriority.Priority5;

        public override string WebPageLabel => InvisibleMessages.RELEASE_NOTES_URL;

        public override string Name => NewVersion.Name;

        public override string Title => string.Format(Messages.ALERT_NEW_VERSION, NewVersion.Name);

        public override string Description => string.Format(Messages.ALERT_NEW_VERSION_DETAILS_CLIENT,
            NewVersion.Name);

        public override Action FixLinkAction
        {
            get { return () => Program.OpenURL(WebPageLabel); }
        }

        public override string FixLinkText => string.Format(Messages.ALERT_NEW_VERSION_DOWNLOAD_CLIENT, NewVersion.Version);

        public override string AppliesTo => BrandManager.BrandConsole;

        public override string HelpID => "XenCenterUpdateAlert";

        public string Checksum { get; }

        public override bool AllowedToDismiss()
        {
            return false;
        }

        public override void Dismiss()
        {
            //do not dismiss this alert
        }

        public override bool Equals(Alert other)
        {
            if (other is ClientUpdateAlert clientAlert)
                return NewVersion.VersionAndLang == clientAlert.NewVersion.VersionAndLang;

            return base.Equals(other);
        }

        public static void DownloadAndInstallNewClient(ClientUpdateAlert updateAlert, IWin32Window parent)
        {
            var outputPathAndFileName = Path.Combine(Path.GetTempPath(), $"{BrandManager.BrandConsole}.msi");

            var downloadAndInstallClientAction = new DownloadAndUpdateClientAction(updateAlert.Name, new Uri(updateAlert.NewVersion.Url), outputPathAndFileName, updateAlert.Checksum);

            using (var dlg = new ActionProgressDialog(downloadAndInstallClientAction, ProgressBarStyle.Continuous))
            {
                dlg.ShowCancel = true;
                dlg.ShowDialog(parent);
            }

            if (!downloadAndInstallClientAction.Succeeded)
                return;

            bool currentTasks = false;
            foreach (ActionBase a in ConnectionsManager.History)
            {
                if (a is MeddlingAction || a.IsCompleted)
                    continue;

                currentTasks = true;
                break;
            }

            if (currentTasks)
            {
                if (new Dialogs.WarningDialogs.CloseXenCenterWarningDialog(true).ShowDialog(parent) != DialogResult.OK)
                {
                    downloadAndInstallClientAction.ReleaseDownloadedContent();
                    return;
                }
            }

            try
            {
                Process.Start(outputPathAndFileName);
                log.DebugFormat("Update {0} found and install started", updateAlert.Name);
                downloadAndInstallClientAction.ReleaseDownloadedContent();
                Application.Exit();
            }
            catch (Exception e)
            {
                log.Error("Exception occurred when starting the installation process.", e);
                downloadAndInstallClientAction.ReleaseDownloadedContent(true);

                using (var dlg = new ErrorDialog(Messages.UPDATE_CLIENT_FAILED_INSTALLER_LAUNCH))
                    dlg.ShowDialog(parent);
            }
        }

        public static void DownloadSource(IWin32Window parent)
        {
            // If no update no need to show where to save dialog
            var clientVersion = Updates.ClientVersions.FirstOrDefault();

            DownloadSourceAction downloadSourceAction;
            if (clientVersion == null)
            {
                // There is no XCupdates.xml so direct to website.
                Program.OpenURL(InvisibleMessages.WEBSITE_DOWNLOADS);
                return;
            }
            else
            {
                string outputPathAndFileName;
                using (var saveSourceDialog = new SaveFileDialog())
                {
                    saveSourceDialog.FileName = BrandManager.BrandConsole + $"-v{clientVersion.Version}" + "-source.zip";
                    saveSourceDialog.DefaultExt = "zip";
                    saveSourceDialog.Filter = "(*.zip)|*.zip|All files (*.*)|*.*";
                    saveSourceDialog.InitialDirectory = Win32.GetKnownFolderPath(Win32.KnownFolders.Downloads);

                    if (saveSourceDialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    outputPathAndFileName = saveSourceDialog.FileName;
                }

                downloadSourceAction = new DownloadSourceAction(clientVersion.Name, clientVersion.Version, new Uri(clientVersion.SourceUrl), outputPathAndFileName);

                using (var dlg = new ActionProgressDialog(downloadSourceAction, ProgressBarStyle.Continuous))
                {
                    dlg.ShowCancel = true;
                    dlg.ShowDialog(parent);
                }

                if (!downloadSourceAction.Succeeded)
                    return;
            }           
        }
    }
}
