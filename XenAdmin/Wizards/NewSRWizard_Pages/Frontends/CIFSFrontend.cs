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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAPI;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;


namespace XenAdmin.Wizards.NewSRWizard_Pages.Frontends
{
    public partial class CifsFrontend : XenTabPage
    {
        private const string SERVER = "server";
        private const string SERVERPATH = "serverpath";
        private const string OPTIONS = "options";

        public CifsFrontend()
        {
            InitializeComponent();
            ToggleReattachControlsEnabledState(false);
        }

        #region XenTabPage overrides

        public override string Text { get { return Messages.NEWSR_LOCATION; } }

        public override string PageTitle { get { return Messages.NEWSR_PATH_CIFS; } }

        public override string HelpID { get { return "Location_CIFS"; } }

        public override bool EnableNext()
        {
            return SrWizardHelpers.ValidateCifsSharename(CifsServerPathTextBox.Text)
                && (radioButtonCifsNew.Checked || listBoxCifsSRs.SelectedIndex > -1);
        }
        
        public override bool EnablePrevious()
        {
            if (SrWizardType.DisasterRecoveryTask && SrWizardType.SrToReattach == null)
                return false;

            return true;
        }

        public override void PopulatePage()
        {
            if (!SrWizardType.AllowToCreateNewSr)
                HideCreateControls();

            if (SrWizardType.UUID != null)
                listBoxCifsSRs.SetMustSelectUUID(SrWizardType.UUID);
        }


        public override void SelectDefaultControl()
        {
            CifsServerPathTextBox.Select();
        }

        #endregion

        private void UpdateButtons()
        {
            CifsScanButton.Enabled = SrWizardHelpers.ValidateCifsSharename(CifsServerPathTextBox.Text);
            OnPageUpdated();
        }

        private void AnyCifsParameters_TextChanged(object sender, EventArgs e)
        {
            CifsScanButton.Enabled = SrWizardHelpers.ValidateCifsSharename(CifsServerPathTextBox.Text);

            listBoxCifsSRs.Items.Clear();
            ToggleReattachControlsEnabledState(false);

            if(radioButtonCifsNew.Enabled)
                radioButtonCifsNew.Checked = true;

            UpdateButtons();
        }

        private void radioButtonCifsReattach_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonCifsNew.Checked = !radioButtonCifsReattach.Checked;
            UpdateButtons();
        }

        private void radioButtonCifsNew_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCifsNew.Checked)
                listBoxCifsSRs.SelectedIndex = -1;
            
            radioButtonCifsReattach.Checked = !radioButtonCifsNew.Checked;
            UpdateButtons();
        }

        private void buttonCifsScan_Click(object sender, EventArgs e)
        {
            try
            {
                CifsScanButton.Enabled = false;

                // Perform an SR.probe to see if there is already an SR present
                Dictionary<String, String> dconf = new Dictionary<String, String>();
                string[] fullpath = CifsServerPathTextBox.Text.Split(new char[] { ':' });
                dconf[SERVER] = fullpath[0];
                if (fullpath.Length > 1)
                {
                    dconf[SERVERPATH] = fullpath[1];
                }

                if (userNameTextBox.Text.Trim().Length > 0 || passwordTextBox.Text.Trim().Length > 0)
                {
                    dconf["username"] = userNameTextBox.Text;
                    dconf["password"] = passwordTextBox.Text;
                }

                Host coordinator = Helpers.GetCoordinator(Connection);
                if (coordinator == null)
                    return;

                // Start probe
                SrProbeAction action = new SrProbeAction(Connection, coordinator, SR.SRTypes.smb, dconf);
                using (var dialog = new ActionProgressDialog(action, ProgressBarStyle.Marquee))
                {
                    dialog.ShowCancel = true;
                    dialog.ShowDialog(this);
                }

                if (radioButtonCifsNew.Enabled)
                    radioButtonCifsNew.Checked = true;

                listBoxCifsSRs.Items.Clear();

                if (!action.Succeeded)
                    return;

                var SRs = action.SRs ?? new List<SR.SRInfo>();
                if (SRs.Count == 0)
                {
                    // Disable box
                    ToggleReattachControlsEnabledState(false);
                    listBoxCifsSRs.Items.Add(Messages.NEWSR_NO_SRS_FOUND);
                    return;
                }

                // Fill box
                foreach(SR.SRInfo info in SRs)
                    listBoxCifsSRs.Items.Add(info);

                listBoxCifsSRs.TryAndSelectUUID();

                ToggleReattachControlsEnabledState(true);
            }
            finally
            {
                UpdateButtons();
            }
        }

        private void listBoxCifsSRs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxCifsSRs.SelectedIndex == -1)
                return;

            radioButtonCifsReattach.Checked = true;
            UpdateButtons();
        }

        #region Accessors

        public SrWizardType SrWizardType { private get; set; }

        public string UUID
        {
            get
            {
                if (radioButtonCifsNew.Checked)
                    return null;

                SR.SRInfo srInfo = listBoxCifsSRs.SelectedItem as SR.SRInfo;
                if (srInfo == null)
                    return null;

                return srInfo.UUID;
            }
        }

        public Dictionary<string, string> DeviceConfig
        {
            get
            {
                var dconf = new Dictionary<string, string>();

                string[] fullpath = CifsServerPathTextBox.Text.Split(new char[] { ':' });

                dconf[SERVER] = fullpath[0];

                if (fullpath.Length > 1)
                {
                    dconf[SERVERPATH] = fullpath[1];
                }

                if (userNameTextBox.Text.Trim().Length > 0 || passwordTextBox.Text.Trim().Length > 0)
                {
                    dconf["username"] = userNameTextBox.Text;
                    dconf["password"] = passwordTextBox.Text;
                }

                return dconf;
            }
        }

        public string SrDescription
        {
            get
            {
                return string.IsNullOrEmpty(CifsServerPathTextBox.Text)
                           ? null
                           : string.Format(Messages.NEWSR_CIFS_ACTION, CifsServerPathTextBox.Text);
            }
        }

        #endregion

        private void HideCreateControls()
        {
            radioButtonCifsNew.Checked = false;
            radioButtonCifsReattach.Checked = true;

            radioButtonCifsNew.Enabled = false;
            radioButtonCifsReattach.Enabled = true;
        }

        private void ToggleReattachControlsEnabledState(bool enable)
        {
            radioButtonCifsReattach.Enabled = enable;
            listBoxCifsSRs.Enabled = enable;
        }
    }
}
