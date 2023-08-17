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
using XenAdmin.Dialogs.ServerUpdates;


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class UpdatesOptionsPage : UserControl, IOptionsPage
    {
        public UpdatesOptionsPage()
        {
            InitializeComponent();

            labelClientUpdates.Text = string.Format(labelClientUpdates.Text, BrandManager.BrandConsole);
            _checkBoxClientUpdates.Text = string.Format(_checkBoxClientUpdates.Text, BrandManager.BrandConsole);
            labelInfoCdn.Text = string.Format(labelInfoCdn.Text, BrandManager.ProductBrand);
        }

        #region IOptionsPage Members

        public void Build()
        {
            _checkBoxClientUpdates.Checked = Properties.Settings.Default.AllowXenCenterUpdates;
        }

        public bool IsValidToSave(out Control control, out string invalidReason)
        {
            control = null;
            invalidReason = string.Empty;
            return true;
        }

        public void ShowValidationMessages(Control control, string message)
        {
        }

        public void HideValidationMessages()
        {
        }

        public void Save()
        {
            if (_checkBoxClientUpdates.Checked != Properties.Settings.Default.AllowXenCenterUpdates)
            {
                Properties.Settings.Default.AllowXenCenterUpdates = _checkBoxClientUpdates.Checked;

                if (Properties.Settings.Default.AllowXenCenterUpdates)
                    Updates.CheckForClientUpdates(true);
            }
        }

        #endregion

        #region IVerticalTab Members

        public override string Text => string.Format(Messages.UPDATES_OPTIONS_TITLE, BrandManager.BrandConsole);

        public string SubText => string.Format(Messages.UPDATES_OPTIONS_DESC, BrandManager.BrandConsole);

        public Image Image => Images.StaticImages._015_Download_h32bit_16;

        #endregion

        private void linkLabelConfigUpdates_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var dialog = new ConfigUpdatesDialog())
                dialog.ShowDialog(this);
        }
    }
}
