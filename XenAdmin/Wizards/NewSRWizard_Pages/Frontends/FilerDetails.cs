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
using XenAPI;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAdmin.Controls;

namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class FilerDetails : XenTabPage
    {
        private const string TARGET = "target";
        private const string USERNAME = "username";
        private const string PASSWORD = "password";
        private const string CHAPUSER = "chapuser";
        private const string CHAPPASSWORD = "chappassword";

        public FilerDetails()
        {
            InitializeComponent();
        }

        #region XenTabPage overrides

        public override void PopulatePage()
        {
            // Setup some labels and names
            labelHostAddress.Text = IsNetApp ? Messages.NEWSR_NETAPP_FILER_ADDRESS : Messages.NEWSR_EQUAL_LOGIC_FILER_ADDRESS;

            textBoxNetappHostAddress.Text = textBoxNetappUsername.Text = textBoxNetappPassword.Text =
                textBoxNetappChapUser.Text = textBoxNetappChapSecret.Text = string.Empty;

            checkBoxNetappUseChap.Checked = false;

            // Clear NetApp invalid host label from previous scan
            labelInvalidHost.Visible = false;
            ToggleChapControlsEnabledState();
        }

        public override string PageTitle { get { return IsNetApp ? Messages.NEWSR_NETAPP_FILER_PAGE_TITLE : Messages.NEWSR_EQUAL_LOGIC_FILER_PAGE_TITLE; } }

        public override string Text { get { return IsNetApp ? Messages.NEWSR_NETAPP_FILER_DETAILS_TEXT : Messages.NEWSR_EQUAL_LOGIC_FILER_DETAILS_TEXT; } }

        public override string HelpID { get { return IsNetApp ? "SL_NETAPP_filer" : "SL_EQL_filer"; } }

        public override void PageLeave(PageLoadedDirection direction, ref bool cancel)
        {
            if (direction == PageLoadedDirection.Forward)
                cancel = !Scan();
            
            base.PageLeave(direction, ref cancel);
        }

        public override bool EnableNext()
        {
            return !string.IsNullOrEmpty(textBoxNetappHostAddress.Text)
                   && !string.IsNullOrEmpty(textBoxNetappUsername.Text)
                   && !(checkBoxNetappUseChap.Checked && string.IsNullOrEmpty(textBoxNetappChapUser.Text));
        }

        #endregion

        #region Accessors

        public SrScanAction SrScanAction { get; private set; }

        /// <summary>
        /// if true it's NetApp; if false it's EqualLogic.
        /// </summary>
        public bool IsNetApp { get; set; }

        public Dictionary<String, String> DeviceConfigParts
        {
            get
            {
                var dconf = new Dictionary<String, String>();

                dconf[TARGET] = textBoxNetappHostAddress.Text;
                dconf[USERNAME] = textBoxNetappUsername.Text;
                dconf[PASSWORD] = textBoxNetappPassword.Text;

                if (checkBoxNetappUseChap.Checked)
                {
                    dconf[CHAPUSER] = textBoxNetappChapUser.Text;
                    dconf[CHAPPASSWORD] = textBoxNetappChapSecret.Text;
                }

                return dconf;
            }
        }

        #endregion

        private bool Scan()
        {
            SrScanAction = null;

            // Now scan for aggregates on which to make new SRs
            SrScanAction scanAction = new SrScanAction(Connection, textBoxNetappHostAddress.Text,
                                                       textBoxNetappUsername.Text, textBoxNetappPassword.Text,
                                                       IsNetApp ? SR.SRTypes.netapp : SR.SRTypes.equal);
            using (var dialog = new ActionProgressDialog(scanAction, ProgressBarStyle.Marquee))
            {
                dialog.ShowCancel = true;
                dialog.ShowDialog(this);
            }

            if (scanAction.Succeeded)
            {
                SrScanAction = scanAction;
                return true;
            }

            if (!(scanAction.Exception is CancelledException))
            {
                Failure failure = scanAction.Exception as Failure;

                if (failure != null && failure.ErrorDescription.Count > 0)
                {
                    if (failure.ErrorDescription[0] == "SR_BACKEND_FAILURE_140")
                    {
                        // DNS lookup failure
                        labelInvalidHost.Visible = true;
                        textBoxNetappHostAddress.Select();
                    }
                }
            }

            return false;
        }

        private void ToggleChapControlsEnabledState()
        {
            labelNetappChapUser.Enabled = textBoxNetappChapUser.Enabled =
                                             labelNetappChapSecret.Enabled = textBoxNetappChapSecret.Enabled = checkBoxNetappUseChap.Checked;
        }

        #region Event handlers

        private void textBoxHostAddress_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void UseChapCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ToggleChapControlsEnabledState();
            OnPageUpdated();
        }

        private void textBoxNetappChapUser_TextChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        #endregion
    }
}
