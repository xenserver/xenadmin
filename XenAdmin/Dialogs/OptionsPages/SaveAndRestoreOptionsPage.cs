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
using System.Windows.Forms;
using XenAdmin.Core;
using XenAdmin.Dialogs.RestoreSession;


namespace XenAdmin.Dialogs.OptionsPages
{
    /// <summary>
    /// The page is used to set whether or not to save server usernames and passwords
    /// and whether a main password should be set to protect these passwords
    /// </summary>
    public partial class SaveAndRestoreOptionsPage : UserControl, IOptionsPage
    {
        private byte[] _mainPassword;

        /// <summary>
        /// Whether to save the server list on OK
        /// </summary>
        protected internal bool SaveAllAfter { get; set; }

        public SaveAndRestoreOptionsPage() 
        {
            InitializeComponent();
            saveStateLabel.Text = string.Format(saveStateLabel.Text, BrandManager.BrandConsole);
        }

        private void SaveEverything()
        {
            if (!Registry.AllowCredentialSave)
                return;

            if (!saveStateCheckBox.Checked)
            {
                Properties.Settings.Default.SaveSession = false;
                Properties.Settings.Default.RequirePass = false;

                Program.MainPassword = null;
            }
            else if (!requireMainPasswordCheckBox.Checked)
            {
                Properties.Settings.Default.SaveSession = true;
                Properties.Settings.Default.RequirePass = false;

                Program.MainPassword = null;
            }
            else
            {
                Properties.Settings.Default.SaveSession = true;
                Properties.Settings.Default.RequirePass = true;

                Program.MainPassword = _mainPassword;
            }

            if (SaveAllAfter)
                Settings.SaveServerList();
        }
        
        #region Control event handlers

        private void changeMainPasswordButton_Click(object sender, EventArgs e)
        {
            using (var changePassword = new ChangeMainPasswordDialog(_mainPassword))
            {
                if (changePassword.ShowDialog(this) == DialogResult.OK)
                {
                    _mainPassword = changePassword.NewPassword;
                }
            }
        }

        private void requireMainPasswordCheckBox_Click(object sender, EventArgs e)
        {
            // requireCoordinatorPasswordCheckBox.Checked was the state before the click
            // if previously checked, the user is trying to clear it => request authorization
            // if previously unchecked, the user is trying to set a password

            if (requireMainPasswordCheckBox.Checked)
            {
                using (var enterPassword = new EnterMainPasswordDialog(_mainPassword))
                {
                    if (enterPassword.ShowDialog(this) == DialogResult.OK)
                    {
                        _mainPassword = null;
                        requireMainPasswordCheckBox.Checked = false;
                        changeMainPasswordButton.Enabled = false;
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(_mainPassword == null, "Main password is set, but not reflected on GUI");

                if (_mainPassword == null)
                {
                    // no previous password existed => set a new one
                    using (var setPassword = new SetMainPasswordDialog())
                    {
                        if (setPassword.ShowDialog(this) == DialogResult.OK)
                        {
                            _mainPassword = setPassword.NewPassword;
                            requireMainPasswordCheckBox.Checked = true;
                            changeMainPasswordButton.Enabled = true;
                        }
                    }
                }
                else
                {
                    // a previous password existed (should never get here but just in case)
                    // enable button to facilitate password change
                    requireMainPasswordCheckBox.Checked = true;
                    changeMainPasswordButton.Enabled = true;
                }
            }
        }

        private void saveStateCheckBox_Click(object sender, EventArgs e)
        {
            // need to prevent the user from going to an open terminal and clearing
            // the save state, then setting the coordinator password to anything they like

            // saveStateCheckBox.Checked was the state before the click
            // if previously checked, the user is trying to clear it => authorization maybe required
            // (depending on the state of the requireCoordinatorPasswordCheckBox; this should be cleared too if checked)

            if (saveStateCheckBox.Checked && requireMainPasswordCheckBox.Checked)
            {
                using (var enterPassword = new EnterMainPasswordDialog(_mainPassword))
                {
                    if (enterPassword.ShowDialog(this) == DialogResult.OK)
                    {
                        _mainPassword = null;
                        saveStateCheckBox.Checked = false;
                        requireMainPasswordCheckBox.Checked = false;
                        mainPasswordGroupBox.Enabled = false;
                    }
                }
            }
            else
            {
                saveStateCheckBox.Checked = !saveStateCheckBox.Checked;
                mainPasswordGroupBox.Enabled = saveStateCheckBox.Checked;
                changeMainPasswordButton.Enabled = requireMainPasswordCheckBox.Checked;
            }
        }

        #endregion

        #region IOptionsPage Members

        public void Build()
        {
            bool allowCredSave = Registry.AllowCredentialSave;
            bool saveSession = Properties.Settings.Default.SaveSession;
            bool reqPass = Properties.Settings.Default.RequirePass;

            saveStateLabel.Enabled = allowCredSave;
            saveStateCheckBox.Enabled = allowCredSave;
            
            saveStateCheckBox.Checked = saveSession && allowCredSave;
            mainPasswordGroupBox.Enabled = saveSession && allowCredSave;
            
            requireMainPasswordCheckBox.Checked = reqPass && Program.MainPassword != null && allowCredSave;
            changeMainPasswordButton.Enabled = reqPass && Program.MainPassword != null && allowCredSave;
            
            _mainPassword = Program.MainPassword;
        }

        public bool IsValidToSave(out Control control, out string invalidReason)
        {
            control = null;
            invalidReason = null;
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
            SaveEverything();
        }

        #endregion

        #region IVerticalTab Members

        public override string Text => Messages.SAVE_AND_RESTORE;

        public string SubText => Messages.SAVE_AND_RESTORE_DESC;

        public Image Image => Images.StaticImages._000_BackupMetadata_h32bit_16;

        #endregion
    }
}
