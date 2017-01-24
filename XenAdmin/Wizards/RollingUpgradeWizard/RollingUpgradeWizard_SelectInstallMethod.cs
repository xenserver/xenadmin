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
using System.Web;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Properties;
using XenAPI;

using XenAdmin.Actions.HostActions;

namespace XenAdmin.Wizards.RollingUpgradeWizard
{
    public partial class RollingUpgradeWizardInstallMethodPage : XenTabPage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Bitmap testOK = Resources._000_Tick_h32bit_16;

        public RollingUpgradeWizardInstallMethodPage()
        {
            InitializeComponent();
            comboBoxUpgradeMethod.SelectedIndex = 0;
        }

        #region XenTabPage overrides

        public override string Text
        {
            get
            {
                return Messages.READY_UPGRADE;
            }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.ROLLING_UPGRADE_METHOD_PAGE_TEXT;
            }
        }

        public override bool EnableNext()
        {
            return pictureBox1.Visible && pictureBox1.Image == testOK;
        }

        public override string NextText(bool isLastPage)
        {
            return Messages.START_UPGRADE;
        }

        public override string HelpID { get { return "upgrademethod"; } }

        #endregion

        private void ShowBottomError(string msg)
        {
            pictureBox2.Visible = true;
            labelErrors.Visible = true;
            labelErrors.Text = msg;
        }

        private void ShowSideIcon(Bitmap bitmap)
        {
            pictureBox1.Visible = true;
            pictureBox1.Image = bitmap;
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
            buttonTest.Enabled = !string.IsNullOrEmpty(watermarkTextBox1.Text);
            HideSideIcon();
            HideBottomError();
            OnPageUpdated();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (InstallMethod == RollingUpgradeInstallMethod.nfs)
            {
                labelUser.Visible = labelPassword.Visible = textBoxPassword.Visible = textBoxUser.Visible = false;
            }
            //FTP or HTTP
            else
            {
                labelUser.Visible = labelPassword.Visible = textBoxPassword.Visible = textBoxUser.Visible = true;
            }

            switch (InstallMethod)
            {
                case RollingUpgradeInstallMethod.nfs:
                    CueBannersManager.SetWatermark(watermarkTextBox1, "server:path/");
                    break;
                case RollingUpgradeInstallMethod.ftp:
                    CueBannersManager.SetWatermark(watermarkTextBox1, "ftp.example.com/");
                    break;
                case RollingUpgradeInstallMethod.http:
                    CueBannersManager.SetWatermark(watermarkTextBox1, "www.example.com/");
                    break;
            }

            ResetCheckControls();
        }

        private RollingUpgradeInstallMethod InstallMethod
        {
            get
            {
                switch (comboBoxUpgradeMethod.SelectedIndex)
                {
                    case 0:
                        return RollingUpgradeInstallMethod.http;
                    case 1:
                        return RollingUpgradeInstallMethod.nfs;
                    case 2:
                        return RollingUpgradeInstallMethod.ftp;
                }
                throw new ArgumentException();
            }
        }

        public Dictionary<string, string> InstallMethodConfig
        {
            get
            {
                var dict = new Dictionary<string, string>();
                watermarkTextBox1.Text = watermarkTextBox1.Text.Trim().Replace(" ","");
                var url = watermarkTextBox1.Text.EndsWith("/")
                              ? watermarkTextBox1.Text
                              : string.Format("{0}/", watermarkTextBox1.Text);
                
                url = ExtractBaseUrl(url);
                watermarkTextBox1.Text = url;

                if (InstallMethod != RollingUpgradeInstallMethod.nfs && textBoxUser.Text != string.Empty && textBoxPassword.Text != string.Empty)
                {
                    dict.Add("url", string.Format("{0}://{1}:{2}@{3}", InstallMethod, textBoxUser.Text.UrlEncode(), textBoxPassword.Text.UrlEncode(), url));
                }
                else
                    dict.Add("url", string.Format("{0}://{1}", InstallMethod,  url));
                return dict;
            }
        }

        /// <summary>
        /// Strip away any prepended URL directives such as http:// etc.. based on the 
        /// entries of the RollingUpgradeInstallMethod enum
        /// </summary>
        /// <param name="url">Some url with/without prepended directives</param>
        /// <returns>base URL without any prepended directives</returns>
        private string ExtractBaseUrl(string url)
        {
            string baseUrl = url;
            foreach (string method in Enum.GetNames(typeof(RollingUpgradeInstallMethod)))
            {
                string prependedText = method.ToLower() + @"://";
                baseUrl = baseUrl.ToLower().StartsWith(prependedText) ? baseUrl.Substring(prependedText.Length) : baseUrl;
            }
            return baseUrl;
        }

        public IEnumerable<Host> SelectedMasters { private get; set; }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            var host = SelectedMasters.First();
            Pool pool = host != null ? Helpers.GetPoolOfOne(host.Connection) : null;
            host = pool != null ? pool.HostsToUpgrade.First() : null;

            if (host == null)
            {
                ShowBottomError(Messages.UPDATES_WIZARD_CONNECTION_LOST);
                ShowSideIcon(Resources._000_Abort_h32bit_16);
                OnPageUpdated();
                return;
            }

            var action = new TestLocationInstallerAction(host, InstallMethodConfig);
            action.Completed += action_Completed;
            ShowSideIcon(Resources.ajax_loader);
            ChangeInputEnablement(false);
            action.RunAsync();
        }

        private void action_Completed(ActionBase sender)
        {
            Program.BeginInvoke(this, () => ChangeInputEnablement(true));
            var action = (AsyncAction)sender;
            bool result = false;
            string resultMessage = string.Empty;

            try
            {
                result = action.Result.ToLower() == "true";
            }
            catch (Failure failure)
            {
                if (failure.ErrorDescription.Count == 4)
                {
                    string fromResources = Messages.ResourceManager.GetString(failure.ErrorDescription[3]);
                    resultMessage = string.IsNullOrEmpty(fromResources) ? failure.Message : fromResources;
                }
                else
                    resultMessage = failure.Message;

                log.Error("Error testing upgrade hotfix", failure);
            }
            finally
            {
                Program.BeginInvoke(this, () =>
                {
                    if (result)
                    {
                        ShowSideIcon(testOK);
                        HideBottomError();
                    }
                    else
                    {
                        ShowBottomError(string.IsNullOrEmpty(resultMessage) ? Messages.INSTALL_FILES_CANNOT_BE_FOUND : resultMessage);
                        ShowSideIcon(Resources._000_Abort_h32bit_16);
                    }
                    OnPageUpdated();
                });
            }
        }

        private void ChangeInputEnablement(bool value)
        {
            textBoxUser.Enabled = textBoxPassword.Enabled = watermarkTextBox1.Enabled = comboBoxUpgradeMethod.Enabled = buttonTest.Enabled = value;
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
