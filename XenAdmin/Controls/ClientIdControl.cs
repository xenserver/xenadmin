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
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using XenAdmin.Actions.Updates;
using XenAdmin.Core;

namespace XenAdmin.Controls
{
    public partial class ClientIdControl : UserControl
    {
        public ClientIdControl()
        {
            InitializeComponent();
            label1.Text = string.Format(label1.Text, BrandManager.BrandConsole,
                BrandManager.CompanyNameLegacy, BrandManager.ProductBrand);
        }

        public string FileServiceUsername { get; private set; }
        public string FileServiceClientId { get; private set; }

        public void Build(bool showLackAsError = false)
        {
            labelInfo.Text = Updates.CheckCanDownloadUpdates()
                ? Messages.FILESERVICE_CLIENTID_FOUND
                : Messages.FILESERVICE_CLIENTID_NOT_FOUND;

            if (showLackAsError)
            {
                pictureBoxInfo.Size = new Size(32, 32);
                pictureBoxInfo.Image = Images.StaticImages._000_error_h32bit_32;
            }
            else
            {
                pictureBoxInfo.Size = new Size(16, 16);
                pictureBoxInfo.Image = Images.StaticImages._000_Info3_h32bit_16;
            }
        }

        public bool IsValidToSave(out Control control, out string invalidReason, bool acceptEmpty = true)
        {
            FileServiceUsername = null;
            FileServiceClientId = null;
            control = null;
            invalidReason = null;

            if (string.IsNullOrWhiteSpace(textBoxClientIdFile.Text))
                return acceptEmpty;

            if (!File.Exists(textBoxClientIdFile.Text))
            {
                control = textBoxClientIdFile;
                invalidReason = Messages.FILE_NOT_FOUND;
                return false;
            }

            try
            {
                var json = File.ReadAllText(textBoxClientIdFile.Text);
                var jsonObject = new JavaScriptSerializer().Deserialize(json, typeof(FileServiceClientId)) as FileServiceClientId;
                FileServiceUsername = jsonObject?.username;
                FileServiceClientId = jsonObject?.apikey;
            }
            catch
            {
                //ignore
            }

            if (!string.IsNullOrEmpty(FileServiceUsername) && !string.IsNullOrEmpty(FileServiceClientId))
                return true;

            control = textBoxClientIdFile;
            invalidReason = Messages.FILESERVICE_CLIENTID_INVALID_FILE;
            return false;
        }

        public void ShowValidationMessages(Control control, string message)
        {
            if (control != null && !string.IsNullOrEmpty(message))
            {
                tooltipValidation.ToolTipTitle = message;
                HelpersGUI.ShowBalloonMessage(control, tooltipValidation);
            }
        }


        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = Messages.FILESERVICE_CLIENTID_TITLE,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = Messages.FILESERVICE_CLIENTID_FILETYPE
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    textBoxClientIdFile.Text = dialog.FileName;
            }
        }

        private void linkLabelClientIdUrl_Click(object sender, EventArgs e)
        {
            Program.OpenURL(Registry.GetCustomClientIdUrl() ?? InvisibleMessages.CLIENT_ID_URL);
        }
    }
}
