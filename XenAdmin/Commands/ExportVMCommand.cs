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
using System.IO;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAPI;


namespace XenAdmin.Commands
{
    /// <summary>
    /// Shows the ExportVMDialog dialog for the selected VM.
    /// </summary>
    internal abstract class ExportVMCommand : Command
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected ExportVMCommand()
        {
        }

        protected ExportVMCommand(IMainWindow mainWindow, IEnumerable<SelectedItem> selection)
            : base(mainWindow, selection)
        {
        }

        protected ExportVMCommand(IMainWindow mainWindow, SelectedItem selection)
            : base(mainWindow, selection)
        {
        }

        protected override void RunCore(SelectedItemCollection selection)
        {
            if (!(selection[0].XenObject is VM vm))
                return;

            Host host = vm.Home();
            Pool pool = Helpers.GetPool(vm.Connection);

            if (host == null && pool != null)
                host = pool.Connection.Resolve(pool.master);

            var connection = selection[0].Connection;

            /*
             * These properties have not been copied over to the new save file dialog.
             *  
            dlg.AddExtension = true;
            dlg.CheckPathExists = true;
            dlg.CreatePrompt = false;
            dlg.CheckFileExists = false;
            dlg.OverwritePrompt = true;
            dlg.ValidateNames = true;*/

            string filename;
            bool verify;

            // Showing this dialog has the (undocumented) side effect of changing the working directory
            // to that of the file selected. This means a handle to the directory persists, making
            // it undeletable until the program exits, or the working dir moves on. So, save and
            // restore the working dir...
            String oldDir = "";
            try
            {
                oldDir = Directory.GetCurrentDirectory();
                while (true)
                {
                    ExportVMDialog dlg = new ExportVMDialog();
                    dlg.DefaultExt = "xva";
                    dlg.Filter = Messages.MAINWINDOW_XVA_BLURB;
                    dlg.Title = Messages.MAINWINDOW_XVA_TITLE;

                    if (dlg.ShowDialog(Parent) != DialogResult.OK)
                        return;

                    filename = dlg.FileName;
                    verify = dlg.Verify;

                    // CA-12975: Warn the user if the export operation does not have enough disk space to
                    // complete.  This is an approximation only.

                    ulong freeSpace;
                    bool isFAT;
                    try
                    {
                        string driveLetter = Path.GetPathRoot(filename).TrimEnd('\\');
                        var o = new System.Management.ManagementObject($"Win32_LogicalDisk.DeviceID=\"{driveLetter}\"");

                        string fsType = o.Properties["FileSystem"].Value.ToString();
                        isFAT = fsType == "FAT" || fsType == "FAT32";
                        
                        freeSpace = ulong.Parse(o.Properties["FreeSpace"].Value.ToString());
                    }
                    catch (Exception exn)
                    {
                        log.Warn(exn, exn);

                        // Could not determine free disk space. Carry on regardless.
                        break;
                    }

                    ulong neededSpace = vm.GetTotalSize();
                    ulong spaceLeft = 100 * Util.BINARY_MEGA; // We want the user to be left with some disk space afterwards

                    (Func<bool> check, string msg) c1 = (
                        () => neededSpace >= freeSpace - spaceLeft,
                        string.Format(Messages.CONFIRM_EXPORT_NOT_ENOUGH_MEMORY, Util.DiskSizeString(neededSpace), Util.DiskSizeString((long)freeSpace), vm.Name())
                    );

                    (Func<bool> check, string msg) c2 = (
                        () => isFAT && neededSpace > 4 * Util.BINARY_GIGA - 1,
                        string.Format(Messages.CONFIRM_EXPORT_FAT, Util.DiskSizeString(neededSpace), Util.DiskSizeString(4 * Util.BINARY_GIGA), vm.Name())
                    );

                    var checksWithMessages = new[] { c1, c2 };

                    foreach (var (check, msg) in checksWithMessages)
                    {
                        if (check.Invoke())
                        {
                            using (var d = new WarningDialog(msg,
                                       new ThreeButtonDialog.TBDButton(Messages.CONTINUE_WITH_EXPORT, DialogResult.OK),
                                       new ThreeButtonDialog.TBDButton(Messages.CHOOSE_ANOTHER_DESTINATION, DialogResult.Retry),
                                       ThreeButtonDialog.ButtonCancel))
                            {
                                d.HelpNameSetter = "ExportVmDialogFSLimitExceeded";
                                var dr = d.ShowDialog(Parent);

                                switch (dr)
                                {
                                    case DialogResult.Retry:
                                        continue;
                                    case DialogResult.Cancel:
                                        return;
                                }
                            }
                        }
                    }

                    break;
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(oldDir);
            }

            new ExportVmAction(connection, host, vm, filename, verify).RunAsync();
        }
    }
}
