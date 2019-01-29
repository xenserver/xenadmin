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
using System.Drawing;
using System.Linq;
using XenAdmin.Actions;
using XenAdmin.Actions.HostActions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;


namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeWizardInstallMethodPage : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private TestLocationInstallerAction testingAction;
        private bool testingUrl;
        private RollingUpgradeInstallMethod _installMethod = RollingUpgradeInstallMethod.http;

        public RollingUpgradeWizardInstallMethodPage()
        {
            InitializeComponent();
            comboBoxUpgradeMethod.SelectedIndex = 0;
        }

        public IEnumerable<Host> SelectedMasters { private get; set; }
        public Dictionary<string, string> InstallMethodConfig;

        #region XenTabPage overrides

        public override string Text => Messages.ROLLING_UPGRADE_METHOD_PAGE_TEXT;

        public override string PageTitle => Messages.ROLLING_UPGRADE_METHOD_PAGE_TITLE;

        public override string HelpID => "upgrademethod";

        public override bool EnableNext()
        {
            if (testingUrl)
                return false;

            return testingAction != null && testingAction.IsCompleted && testingAction.Succeeded;
        }

        protected override void PageLoadedCore(PageLoadedDirection direction)
        {
            ResetCheckControls();
        }

        protected override void PageLeaveCore(PageLoadedDirection direction, ref bool cancel)
        {
            StopUrlTesting();
        }

        public override void PageCancelled(ref bool cancel)
        {
            StopUrlTesting();
        }

        #endregion

        private void ShowBottomError(string msg)
        {
            pictureBox2.Visible = true;
            labelErrors.Visible = true;
            labelErrors.Text = msg;
        }

        private void ShowSideIcon(Image img)
        {
            pictureBox1.Visible = true;
            pictureBox1.Image = img;
        }

        private void HideBottomError()
        {
            pictureBox2.Visible = false;
            labelErrors.Visible = false;
        }

        private void HideSideIcon()
        {
            pictureBox1.Visible = false;
        }

        private void ResetCheckControls()
        {
            testingAction = null;
            buttonTest.Enabled = !string.IsNullOrWhiteSpace(watermarkTextBox1.Text);
            HideSideIcon();
            HideBottomError();
            OnPageUpdated();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                labelPassword.Visible = textBoxPassword.Visible =  _installMethod != RollingUpgradeInstallMethod.nfs;

            ResetCheckControls();
        }

        private string AutoCorrectUrl(out Dictionary<string, string> config)
        {
            var url = watermarkTextBox1.Text.Trim().Replace(" ", "");
            if (!url.EndsWith("/"))
                url = $"{url}/";

            foreach (string method in Enum.GetNames(typeof(RollingUpgradeInstallMethod)))
            {
                string prefix = method.ToLower() + @"://";
                if (url.ToLower().StartsWith(prefix))
                    url = url.Substring(prefix.Length);
            }

            config = new Dictionary<string, string>();

            if (_installMethod != RollingUpgradeInstallMethod.nfs &&
                !string.IsNullOrWhiteSpace(textBoxUser.Text) && !string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                config["url"]= $"{_installMethod}://{textBoxUser.Text.UrlEncode()}:{textBoxPassword.Text.UrlEncode()}@{url}";
            }
            else
                config["url"]=  $"{_installMethod}://{url}";

            return url;
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            if (testingUrl)
                StopUrlTesting();
            else
            {
                watermarkTextBox1.Text = AutoCorrectUrl(out InstallMethodConfig);
                StartUrlTesting();
            }
        }

        private void StartUrlTesting()
        {
            var host = SelectedMasters.First();
            Pool pool = host != null ? Helpers.GetPoolOfOne(host.Connection) : null;
            host = pool != null ? pool.HostsToUpgrade().First() : null;

            if (host == null)
            {
                ShowBottomError(Messages.UPDATES_WIZARD_CONNECTION_LOST);
                ShowSideIcon(Resources._000_Abort_h32bit_16);
                OnPageUpdated();
                return;
            }

            testingAction = new TestLocationInstallerAction(host, InstallMethodConfig);
            testingAction.Completed += action_Completed;
            testingUrl = true;
            ShowSideIcon(Resources.ajax_loader);
            ChangeInputEnablement();
            testingAction.RunAsync();
            OnPageUpdated();
        }

        /// <summary>
        /// The TestLocationInstallerAction cannot be cancelled because it is implemented as a plug-in
        /// on the server side. What we do is: 
        ///  1. Ignore the Completed event of the action, and 
        ///  2. Recover the button text and side icon.
        /// </summary>
        private void StopUrlTesting()
        {
            if (testingAction != null)
            {
                testingAction.Completed -= action_Completed;
                testingUrl = false;
                ChangeInputEnablement();
                HideSideIcon();
            }
        }

        private void action_Completed(ActionBase action)
        {
            action.Completed -= action_Completed;
            if (action != testingAction)
                return;

            Program.Invoke(this, () =>
            {
                testingUrl = false;
                ChangeInputEnablement();

                if (action.Succeeded)
                {
                    ShowSideIcon(Images.StaticImages._000_Tick_h32bit_16);
                    HideBottomError();
                }
                else
                {
                    ShowSideIcon(Images.StaticImages._000_Abort_h32bit_16);
                    ShowBottomError(action.Exception.Message);
                }
                OnPageUpdated();
            });
        }

        private void ChangeInputEnablement()
        {
            textBoxUser.Enabled = textBoxPassword.Enabled = watermarkTextBox1.Enabled = comboBoxUpgradeMethod.Enabled = !testingUrl;
            buttonTest.Text = testingUrl ? Messages.ROLLING_UPGRADE_BUTTON_LABEL_STOP : Messages.ROLLING_UPGRADE_BUTTON_LABEL_TEST;
        }

        private void watermarkTextBox1_TextChanged(object sender, EventArgs e)
        {
            ResetCheckControls();
        }

        private void textBoxUser_TextChanged(object sender, EventArgs e)
        {
            ResetCheckControls();
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            ResetCheckControls();
        }
    }

    public enum RollingUpgradeInstallMethod
    {
        http,
        nfs,
        ftp
    }
}
