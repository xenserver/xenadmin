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
using XenAdmin.Controls;
using XenAdmin.Core;


namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeWizardUpgradeModePage : XenTabPage
    {
        public RollingUpgradeWizardUpgradeModePage()
        {
            InitializeComponent();
            label3.Text = string.Format(label3.Text, BrandManager.ProductBrand);
            comboBoxUpgradeMethod.Items.AddRange(new object[] {new HttpItem(), new NfsItem(), new FtpItem()});
            comboBoxUpgradeMethod.SelectedIndex = 0;
        }

        #region XenTabPage overrides

        public override string Text => Messages.ROLLING_UPGRADE_TITLE_MODE;

        public override string PageTitle => Messages.ROLLING_UPGRADE_MODE_PAGE;

        public override string HelpID => "Upgrademode";

        public override bool EnableNext()
        {
            if (radioButtonAutomatic.Checked)
                return !string.IsNullOrWhiteSpace(watermarkTextBox1.Text);

            return true;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            OnPageUpdated();
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
                InstallMethodConfig = radioButtonAutomatic.Checked ? CalculateInstallMethodConfig() : null;
        }

        #endregion


        #region Accessors

        public Dictionary<string, string> InstallMethodConfig;

        public bool ManualModeSelected => radioButtonManual.Checked;

        #endregion

        private void UpdateInstallMethod()
        {
            if (string.IsNullOrWhiteSpace(watermarkTextBox1.Text))
                return;
            
            var url = watermarkTextBox1.Text.Replace(" ", "");

            foreach (var item in comboBoxUpgradeMethod.Items)
            {
                if (item is UpgradeMethodItem method && url.ToLower().StartsWith(method.Prefix))
                {
                    comboBoxUpgradeMethod.SelectedItem = item;
                    break;
                }
            }
        }

        private Dictionary<string, string> CalculateInstallMethodConfig()
        {
            if (string.IsNullOrWhiteSpace(watermarkTextBox1.Text))
                return null;

            var url = watermarkTextBox1.Text.Replace(" ", "");
            if (!url.EndsWith("/"))
                url = $"{url}/";

            foreach (var item in comboBoxUpgradeMethod.Items)
            {
                if (item is UpgradeMethodItem method && url.ToLower().StartsWith(method.Prefix))
                {
                    url = url.Substring(method.Prefix.Length);
                    break;
                }
            }

            watermarkTextBox1.Text = url;

            if (comboBoxUpgradeMethod.SelectedItem is UpgradeMethodItem umItem)
                return new Dictionary<string, string> {{"url", umItem.FormatUrl(url, textBoxUser.Text, textBoxPassword.Text)}};

            return null;
        }

        #region Control event handlers

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            radioButtonAutomatic.Checked = true;

            if (comboBoxUpgradeMethod.SelectedItem is UpgradeMethodItem item)
            {
                CueBannersManager.SetWatermark(watermarkTextBox1, item.ExampleUrl);

                labelUser.Visible = textBoxUser.Visible =
                    labelPassword.Visible = textBoxPassword.Visible = !(item is NfsItem);
            }

            OnPageUpdated();
        }

        private void watermarkTextBox1_TextChanged(object sender, EventArgs e)
        {
            radioButtonAutomatic.Checked = true;
            UpdateInstallMethod();
            OnPageUpdated();
        }

        private void textBoxUser_TextChanged(object sender, EventArgs e)
        {
            radioButtonAutomatic.Checked = true;
            OnPageUpdated();
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            radioButtonAutomatic.Checked = true;
            OnPageUpdated();
        }

        private void radioButtonAutomatic_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAutomatic.Checked)
                OnPageUpdated();
        }

        private void radioButtonManual_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonManual.Checked)
                OnPageUpdated();
        }

        #endregion

        #region Nested items

        private abstract class UpgradeMethodItem
        {
            public abstract string ExampleUrl { get; }

            protected abstract string Name{ get; }

            public string Prefix => Name.ToLower() + @"://";

            public override string ToString()
            {
                return Name;
            }

            public virtual string FormatUrl(string url, string username = null, string password = null)
            {
                return string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)
                    ? $"{Name.ToLower()}://{url}"
                    : $"{Name.ToLower()}://{username.UrlEncode()}:{password.UrlEncode()}@{url}";
            }
        }

        private class HttpItem : UpgradeMethodItem
        {
            protected override string Name => "HTTP";
            public override string ExampleUrl => "www.example.com/";
        }

        private class NfsItem : UpgradeMethodItem
        {
            protected override string Name => "NFS";
            public override string ExampleUrl => "server:path/";

            public override string FormatUrl(string url, string username = null, string password = null)
            {
                return $"{Name.ToLower()}://{url}";
            }
        }

        private class FtpItem : UpgradeMethodItem
        {
            protected override string Name => "FTP";
            public override string ExampleUrl => "ftp.example.com/";
        }

        #endregion
    }
}
