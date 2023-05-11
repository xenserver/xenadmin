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

using System.Drawing;
using System.Windows.Forms;
using XenAdmin.Core;
using XenCenterLib;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class UpdatesOptionsPage : UserControl, IOptionsPage
    {
        public UpdatesOptionsPage()
        {
            InitializeComponent();
            
            groupBoxClient.Text = string.Format(groupBoxClient.Text, BrandManager.BrandConsole);
            labelClientUpdates.Text = string.Format(labelClientUpdates.Text, BrandManager.BrandConsole);
            _checkBoxClientUpdates.Text = string.Format(_checkBoxClientUpdates.Text, BrandManager.BrandConsole);

            groupBoxServer.Text = string.Format(groupBoxServer.Text, BrandManager.ProductVersion821);
            labelServerUpdates.Text = string.Format(labelServerUpdates.Text, BrandManager.BrandConsole, BrandManager.ProductBrand);
            _checkBoxServerUpdates.Text = string.Format(_checkBoxServerUpdates.Text, BrandManager.ProductBrand);
            _checkBoxServerVersions.Text = string.Format(_checkBoxServerVersions.Text, BrandManager.ProductBrand);
        }

        #region IOptionsPage Members

        public void Build()
        {
            _checkBoxServerVersions.Checked = Properties.Settings.Default.AllowXenServerUpdates;
            _checkBoxServerUpdates.Checked = Properties.Settings.Default.AllowPatchesUpdates;
            _checkBoxClientUpdates.Checked = Properties.Settings.Default.AllowXenCenterUpdates;

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

        public void HideValidationMessages()
        {
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(clientIdControl1.FileServiceUsername))
                Properties.Settings.Default.FileServiceUsername = EncryptionUtils.Protect(clientIdControl1.FileServiceUsername);

            if (!string.IsNullOrEmpty(clientIdControl1.FileServiceClientId))
                Properties.Settings.Default.FileServiceClientId = EncryptionUtils.Protect(clientIdControl1.FileServiceClientId);

            if (_checkBoxClientUpdates.Checked != Properties.Settings.Default.AllowXenCenterUpdates)
            {
                Properties.Settings.Default.AllowXenCenterUpdates = _checkBoxClientUpdates.Checked;

                if (Properties.Settings.Default.AllowXenCenterUpdates)
                    Updates.CheckForClientUpdates(true);
            }

            if (_checkBoxServerUpdates.Checked != Properties.Settings.Default.AllowPatchesUpdates ||
                _checkBoxServerVersions.Checked != Properties.Settings.Default.AllowXenServerUpdates)
            {
                Properties.Settings.Default.AllowPatchesUpdates = _checkBoxServerUpdates.Checked;
                Properties.Settings.Default.AllowXenServerUpdates = _checkBoxServerVersions.Checked;

                if (Properties.Settings.Default.AllowPatchesUpdates || Properties.Settings.Default.AllowXenServerUpdates)
                    Updates.CheckForServerUpdates(true);
            }
        }

        #endregion

        #region IVerticalTab Members

        public override string Text => Messages.UPDATES;

        public string SubText => string.Format(Messages.UPDATES_DESC, BrandManager.ProductBrand, BrandManager.BrandConsole);

        public Image Image => Images.StaticImages._000_Patch_h32bit_16;

        #endregion
    }
}
