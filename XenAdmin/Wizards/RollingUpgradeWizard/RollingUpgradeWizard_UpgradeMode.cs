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
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeWizardUpgradeModePage : XenTabPage
    {
        private enum RollingUpgradeInstallMethod
        {
            http,
            nfs,
            ftp
        }

        private RollingUpgradeInstallMethod _installMethod = RollingUpgradeInstallMethod.http;

        public RollingUpgradeWizardUpgradeModePage()
        {
            InitializeComponent();
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

        private Dictionary<string, string> CalculateInstallMethodConfig()
        {
            if (string.IsNullOrWhiteSpace(watermarkTextBox1.Text))
                return null;

            var url = watermarkTextBox1.Text.Trim().Replace(" ", "");
            if (!url.EndsWith("/"))
                url = $"{url}/";

            foreach (string method in Enum.GetNames(typeof(RollingUpgradeInstallMethod)))
            {
                string prefix = method.ToLower() + @"://";
                if (url.ToLower().StartsWith(prefix))
                {
                    url = url.Substring(prefix.Length);
                    break;
                }
            }

            watermarkTextBox1.Text = url;

            var config = new Dictionary<string, string>();

            if (_installMethod != RollingUpgradeInstallMethod.nfs &&
                !string.IsNullOrWhiteSpace(textBoxUser.Text) && !string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                config["url"] = $"{_installMethod}://{textBoxUser.Text.UrlEncode()}:{textBoxPassword.Text.UrlEncode()}@{url}";
            }
            else
                config["url"] = $"{_installMethod}://{url}";

            return config;
        }

        #region Control event handlers

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            radioButtonAutomatic.Checked = true;

            switch (comboBoxUpgradeMethod.SelectedIndex)
            {
                case 0:
                    _installMethod = RollingUpgradeInstallMethod.http;
                    CueBannersManager.SetWatermark(watermarkTextBox1, "www.example.com/");
                    break;
                case 1:
                    _installMethod = RollingUpgradeInstallMethod.nfs;
                    CueBannersManager.SetWatermark(watermarkTextBox1, "server:path/");
                    break;
                case 2:
                    _installMethod = RollingUpgradeInstallMethod.ftp;
                    CueBannersManager.SetWatermark(watermarkTextBox1, "ftp.example.com/");
                    break;
                default:
                    throw new ArgumentException();
            }

            labelUser.Visible = textBoxUser.Visible =
                labelPassword.Visible = textBoxPassword.Visible = _installMethod != RollingUpgradeInstallMethod.nfs;

            OnPageUpdated();
        }

        private void watermarkTextBox1_TextChanged(object sender, EventArgs e)
        {
            radioButtonAutomatic.Checked = true;
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
    }
}
