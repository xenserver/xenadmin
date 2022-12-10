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
using System.Collections.Generic;
using System.Windows.Forms;
using XenAdmin.Core;
using System.Linq;


namespace XenAdmin.Dialogs
{
    public partial class AboutDialog : XenDialogBase
    {
        private LegalNoticesDialog _theLegalDialog;

        public AboutDialog()
        {
            InitializeComponent();

            var revision = Program.Version.Revision;

            VersionLabel.Text = string.Format(Messages.VERSION_NUMBER, BrandManager.BrandConsole,
                BrandManager.XenCenterVersion, revision, IntPtr.Size * 8);

            if (Helpers.CommonCriteriaCertificationRelease)
                VersionLabel.Text += string.Format(Messages.COMMON_CRITERIA_TEXT, BrandManager.ProductBrand);

            label2.Text = string.Format(label2.Text, BrandManager.CompanyNameLegal);
            label2.Visible = !HiddenFeatures.CopyrightHidden;

            licenseDetailsLabel.Text = string.Format(licenseDetailsLabel.Text, BrandManager.ProductBrand);
            showAgainCheckBox.Text = string.Format(showAgainCheckBox.Text, BrandManager.BrandConsole);

            showAgainCheckBox.Checked = Properties.Settings.Default.ShowAboutDialog;
            var showLicenseNag = HiddenFeatures.LicenseNagVisible;
            LicenseDetailsTextBox.Text = showLicenseNag ? GetLicenseDetails() : "";
            licenseDetailsLabel.Visible = LicenseDetailsTextBox.Visible = showLicenseNag;
            showAgainCheckBox.Visible = showLicenseNag;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Text = string.Format(Text, BrandManager.BrandConsole);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_theLegalDialog == null || _theLegalDialog.IsDisposed)
            {
                _theLegalDialog = new LegalNoticesDialog();
                _theLegalDialog.Show(this);
            }
            else
            {
                _theLegalDialog.BringToFront();
                _theLegalDialog.Focus();
            }
        }

        private string GetLicenseDetails()
        {
            List<string> companies = new List<string>();
            foreach (var xenConnection in ConnectionsManager.XenConnectionsCopy.Where(c => c.IsConnected))
            {
                foreach (var host in xenConnection.Cache.Hosts.Where(h => h.license_params != null && h.license_params.ContainsKey("company")))
                {
                    if (!string.IsNullOrEmpty(host.license_params["company"]) && !companies.Contains(host.license_params["company"]))
                    {
                        companies.Add(host.license_params["company"]);
                    }
                }
            }
            return string.Join("\r\n", companies);
        }

        private void showAgainCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.ShowAboutDialog != showAgainCheckBox.Checked)
            {
                Properties.Settings.Default.ShowAboutDialog = showAgainCheckBox.Checked;
                Settings.TrySaveSettings();
            }
        }

        private void AboutDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            _theLegalDialog?.Dispose();
            _theLegalDialog = null;
        }
    }
}
