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


namespace XenAdmin.Dialogs.OptionsPages
{
    public partial class SecurityOptionsPage : UserControl, IOptionsPage
    {
        public SecurityOptionsPage()
        {
            InitializeComponent();
            SSLLabel.Text = string.Format(SSLLabel.Text, BrandManager.BrandConsole);
            labelReminder.Text = string.Format(labelReminder.Text, BrandManager.BrandConsole);
        }

        #region IOptionsPage Members

        public void Build()
        {
            // SSL Certificates
            CertificateFoundCheckBox.Checked = Properties.Settings.Default.WarnUnrecognizedCertificate ||
                                               Registry.SSLCertificateTypes == SSLCertificateTypes.All;
            CertificateChangedCheckBox.Checked = Properties.Settings.Default.WarnChangedCertificate ||
                                                 Registry.SSLCertificateTypes != SSLCertificateTypes.None;
            CertificateFoundCheckBox.Enabled = Registry.SSLCertificateTypes != SSLCertificateTypes.All;
            CertificateChangedCheckBox.Enabled = Registry.SSLCertificateTypes == SSLCertificateTypes.None;

            checkBoxReminder.Checked = Properties.Settings.Default.RemindChangePassword;
        }

        public bool IsValidToSave()
        {
            return true;
        }

        public void ShowValidationMessages()
        {
        }

        public void HideValidationMessages()
        {
        }
        public void Save()
        {
            // SSL Certificates
            if (CertificateFoundCheckBox.Enabled && CertificateFoundCheckBox.Checked != Properties.Settings.Default.WarnUnrecognizedCertificate)
                Properties.Settings.Default.WarnUnrecognizedCertificate = CertificateFoundCheckBox.Checked;
            if (CertificateChangedCheckBox.Enabled && CertificateChangedCheckBox.Checked != Properties.Settings.Default.WarnChangedCertificate)
                Properties.Settings.Default.WarnChangedCertificate = CertificateChangedCheckBox.Checked;

            if (Properties.Settings.Default.RemindChangePassword != checkBoxReminder.Checked)
                Properties.Settings.Default.RemindChangePassword = checkBoxReminder.Checked;
        }

        #endregion

        #region IVerticalTab Members

        public override string Text => Messages.SECURITY;

        public string SubText => Messages.SECURITY_DESC;

        public Image Image => Images.StaticImages.padlock;

        #endregion
    }
}
