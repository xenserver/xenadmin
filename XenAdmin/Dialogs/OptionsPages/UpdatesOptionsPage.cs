﻿/* Copyright (c) Citrix Systems, Inc. 
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

using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Core;
using XenCenterLib;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class UpdatesOptionsPage : UserControl, IOptionsPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public UpdatesOptionsPage()
        {
            InitializeComponent();
        }

        #region IOptionsPage Members

        public void Build()
        {
            // XenCenter updates
            AllowXenCenterUpdatesCheckBox.Checked = Properties.Settings.Default.AllowXenCenterUpdates;

            // XenServer updates
            AllowXenServerPatchesCheckBox.Checked = Properties.Settings.Default.AllowPatchesUpdates;
            AllowXenServerUpdatesCheckBox.Checked = Properties.Settings.Default.AllowXenServerUpdates;

            clientIdControl1.Build();
        }

        public bool IsValidToSave(out Control control, out string invalidReason)
        {
            return clientIdControl1.IsValidToSave(out control, out invalidReason);
        }

        public void ShowValidationMessages(Control control, string message)
        {
            clientIdControl1.ShowValidationMessages(control, message);
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(clientIdControl1.FileServiceUsername))
                Properties.Settings.Default.FileServiceUsername = EncryptionUtils.Protect(clientIdControl1.FileServiceUsername);

            if (!string.IsNullOrEmpty(clientIdControl1.FileServiceClientId))
                Properties.Settings.Default.FileServiceClientId = EncryptionUtils.Protect(clientIdControl1.FileServiceClientId);

            bool checkXenCenterUpdates = AllowXenCenterUpdatesCheckBox.Checked != Properties.Settings.Default.AllowXenCenterUpdates;
            bool checkPatchUpdates = AllowXenServerPatchesCheckBox.Checked != Properties.Settings.Default.AllowPatchesUpdates;
            bool checkVersionUpdates = AllowXenServerUpdatesCheckBox.Checked != Properties.Settings.Default.AllowXenServerUpdates;

            if (checkXenCenterUpdates)
                Properties.Settings.Default.AllowXenCenterUpdates = AllowXenCenterUpdatesCheckBox.Checked;

            if (checkPatchUpdates)
                Properties.Settings.Default.AllowPatchesUpdates = AllowXenServerPatchesCheckBox.Checked;

            if (checkVersionUpdates)
                Properties.Settings.Default.AllowXenServerUpdates = AllowXenServerUpdatesCheckBox.Checked;

            if(checkXenCenterUpdates || checkPatchUpdates || checkVersionUpdates)
                Updates.CheckForUpdates(false, true);
        }

        #endregion

        #region IVerticalTab Members

        public override string Text => Messages.UPDATES;

        public string SubText => Messages.UPDATES_DESC;

        public Image Image => Images.StaticImages._000_Patch_h32bit_16;

        #endregion
    }
}
