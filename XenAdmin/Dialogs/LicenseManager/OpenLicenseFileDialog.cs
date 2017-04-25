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

using System;
using System.IO;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAPI;

namespace XenAdmin.Dialogs
{
    public abstract class LicenseFileDialog
    {
        protected abstract bool CanRunAction(DialogResult result);
        protected abstract void RunActions();
    }

    public class OpenLicenseFileDialog : LicenseFileDialog
    {
        private OpenFileDialog Dialog { get; set; }
        private readonly IWin32Window parent;
        private readonly Host host;
        private readonly string title;
        private readonly bool activateFreeLicense;

        public OpenLicenseFileDialog(IWin32Window parent,  Host host, string title, bool activateFreeLicense)
        {
            this.parent = parent;
            this.host = host;
            this.title = title;
            this.activateFreeLicense = activateFreeLicense;
            Dialog = new OpenFileDialog();
            Dialog.Multiselect = false;
            Dialog.Title = title;
            Dialog.CheckFileExists = true;
            Dialog.CheckPathExists = true;
            Dialog.Filter = string.Format("{0} (*.xslic)|*.xslic|{1} (*.*)|*.*", Messages.XS_LICENSE_FILES, Messages.ALL_FILES);
            Dialog.ShowHelp = true;
            Dialog.HelpRequest += delegate { Help.HelpManager.Launch("LicenseKeyDialog"); };
        }

        public void Dispose()
        {
            Dialog.Dispose();
        }

        public void ShowDialogAndRunAction()
        {
            if(CanRunAction(Dialog.ShowDialog(parent)))
            {
                RunActions();
            }
        }

        protected override bool CanRunAction(DialogResult result)
        {
            return result == DialogResult.OK && host.Connection.IsConnected;
        }

        protected override void RunActions()
        {
            // Showing this dialog has the (undocumented) side effect of changing the working directory
            // to that of the file selected. This means a handle to the directory persists, making
            // it undeletable until the program exits, or the working dir moves on. So, save and
            // restore the working dir...
            string oldDir = String.Empty;
            try
            {
                oldDir = Directory.GetCurrentDirectory();
                ApplyLicenseAction action = new ApplyLicenseAction(host.Connection, host, Dialog.FileName, activateFreeLicense);
                using (var actionProgress = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                {
                    actionProgress.Text = title;
                    actionProgress.ShowDialog(parent);
                }
            }
            finally
            {
                Directory.SetCurrentDirectory(oldDir);
            }
        }
    }
}
