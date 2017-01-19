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

namespace XenAdmin.Dialogs.RestoreSession
{
    /// <summary>
    /// The dialog used to set whether or not to save server usernames and passwords and whether a master password should be set to protect these passwords
    /// </summary>
    public partial class SaveAndRestoreDialog : XenDialogBase
    {
        public SaveAndRestoreDialog(bool saveAllAfter)
        {
            InitializeComponent();
            // register the checked and clicked events after the inital values of the controls have been set, saves compications of events accidentally being fired programtically
            RegisterEvents();
            // call save serverlist on OK
            saveAndRestoreOptionsPage1.SaveAllAfter = saveAllAfter;
        }

        private void RegisterEvents()
        {
            okButton.Click += new EventHandler(okButton_Click);
            cancelButton.Click += new EventHandler(cancelButton_Click);
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        void okButton_Click(object sender, EventArgs e)
        {
            // ronseal
            SaveEverything();
            Close();
        }

        // all prompts for old password should have been made
        private void SaveEverything()
        {
            saveAndRestoreOptionsPage1.Save();
            Settings.TrySaveSettings();
        }
    }
}