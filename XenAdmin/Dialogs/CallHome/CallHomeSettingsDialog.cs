/* Copyright (c) Citrix Systems Inc. 
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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Dialogs.CallHome
{
    public partial class CallHomeSettingsDialog : XenDialogBase
    {
        private readonly Pool pool;
        private CallHomeSettings callHomeSettings;
        private bool authenticationRequired;
        private bool authenticated;
        private string authenticationToken;

        public CallHomeSettingsDialog(Pool pool)
        {
            this.pool = pool;
            callHomeSettings = pool.CallHomeSettings;
            authenticationToken = callHomeSettings.GetExistingUploadToken(pool.Connection);
            InitializeComponent();
            PopulateControls();
            InitializeControls();
            UpdateButtons();
        }

        private void PopulateControls()
        {
            var list = BuildDays();
            var ds = new BindingSource(list, null);
            dayOfWeekComboBox.DataSource = ds;
            dayOfWeekComboBox.ValueMember = "key";
            dayOfWeekComboBox.DisplayMember = "value";

            var list1 = BuildHours();
            var ds1 = new BindingSource(list1, null);
            timeOfDayComboBox.DataSource = ds1;
            timeOfDayComboBox.ValueMember = "key";
            timeOfDayComboBox.DisplayMember = "value";
        }

        private Dictionary<int, string> BuildDays()
        {
            Dictionary<int, string> days = new Dictionary<int, string>();
            foreach (var dayOfWeek in Enum.GetValues(typeof(DayOfWeek)))
            {
                days.Add((int)dayOfWeek, dayOfWeek.ToString());
            }
            return days;
        }

        private SortedDictionary<int, string> BuildHours()
        {
            SortedDictionary<int, string> hours = new SortedDictionary<int, string>();
            for (int hour = 0; hour <= 23; hour++)
            {
                DateTime time = new DateTime(1900, 1, 1, hour, 0, 0);
                hours.Add(hour, HelpersGUI.DateTimeToString(time, Messages.DATEFORMAT_HM, true));
            }
            return hours;
        }

        private void InitializeControls()
        {
            authenticationRequired = string.IsNullOrEmpty(authenticationToken);
            authenticated = !authenticationRequired;

            Text = String.Format(Messages.CALLHOME_ENROLLMENT_TITLE, pool.Name);
            
            authenticationRubricLabel.Text = authenticationRequired 
                ? Messages.CALLHOME_AUTHENTICATION_RUBRIC_NO_TOKEN 
                : Messages.CALLHOME_AUTHENTICATION_RUBRIC_EXISTING_TOKEN;

            enrollmentCheckBox.Checked = callHomeSettings.Status != CallHomeStatus.Disabled;
            frequencyNumericBox.Value = callHomeSettings.IntervalInWeeks;
            dayOfWeekComboBox.SelectedValue = (int)callHomeSettings.DayOfWeek;
            timeOfDayComboBox.SelectedValue = callHomeSettings.TimeOfDay;
            existingAuthenticationRadioButton.Enabled = existingAuthenticationRadioButton.Checked = !authenticationRequired;
            newAuthenticationRadioButton.Checked = authenticationRequired;
            authenticateButton.Enabled = false;
        }

        private bool ChangesMade()
        {
            if (enrollmentCheckBox.Checked && callHomeSettings.Status != CallHomeStatus.Enabled)
                return true;
            if (!enrollmentCheckBox.Checked && callHomeSettings.Status != CallHomeStatus.Disabled)
                return true;
            if (frequencyNumericBox.Value != callHomeSettings.IntervalInWeeks)
                return true;
            if (dayOfWeekComboBox.SelectedIndex != (int)callHomeSettings.DayOfWeek)
                return true;
            if (timeOfDayComboBox.SelectedIndex != callHomeSettings.TimeOfDay)
                return true;
            if (authenticationToken != callHomeSettings.GetUploadToken(pool.Connection))
                return true;
            return false;
        }

        private void UpdateButtons()
        {
            okButton.Enabled = !enrollmentCheckBox.Checked || !authenticationRequired || authenticated;
            okButton.Text = callHomeSettings.Status == CallHomeStatus.Enabled || !enrollmentCheckBox.Checked
                ? Messages.OK
                : Messages.CALLHOME_ENROLLMENT_CONFIRMATION_BUTTON_LABEL;
        }

        private void credentials_TextChanged(object sender, EventArgs e)
        {
            authenticateButton.Enabled = !string.IsNullOrEmpty(usernameTextBox.Text.Trim()) &&
                                         !string.IsNullOrEmpty(passwordTextBox.Text.Trim());
            newAuthenticationRadioButton.Checked = !string.IsNullOrEmpty(usernameTextBox.Text) ||
                                                   !string.IsNullOrEmpty(passwordTextBox.Text);
        }

        private void authenticateButton_Click(object sender, EventArgs e)
        {
            HideAuthenticationStatusControls();

            spinnerIcon.StartSpinning();

            var action = new CallHomeAuthenticationAction(pool, usernameTextBox.Text.Trim(), passwordTextBox.Text.Trim(),
                Registry.CallHomeIdentityTokenDomainName, Registry.CallHomeUploadGrantTokenDomainName, Registry.CallHomeUploadTokenDomainName, false);
            action.Completed += CallHomeAuthenticationAction_Completed;
            authenticateButton.Enabled = false;
            action.RunAsync();
        }

        private void CallHomeAuthenticationAction_Completed(ActionBase action)
        {
            Program.Invoke(this, delegate
                                     {
                                         if (action.Succeeded)
                                         {
                                             spinnerIcon.DisplaySucceededImage();
                                             authenticated = true;
                                         }
                                         else
                                         {
                                             spinnerIcon.Visible = false;
                                             statusPictureBox.Visible = statusLabel.Visible = true;

                                             statusLabel.Text = action.Exception != null
                                                                    ? action.Exception.Message
                                                                    : Messages.ERROR_UNKNOWN;
                                         }
                                         authenticateButton.Enabled = true;
                                         UpdateButtons();
                                     });

        }

        private void HideAuthenticationStatusControls()
        {
            statusPictureBox.Visible = statusLabel.Visible = false;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (ChangesMade())
            {
                var newCallHomeSettings = new CallHomeSettings(
                    enrollmentCheckBox.Checked ? CallHomeStatus.Enabled : CallHomeStatus.Disabled, 
                    (int) (frequencyNumericBox.Value * 7),
                    (DayOfWeek) dayOfWeekComboBox.SelectedValue, 
                    (int) timeOfDayComboBox.SelectedValue,
                    CallHomeSettings.DefaultRetryInterval);

                new SaveCallHomeSettingsAction(pool, newCallHomeSettings, authenticationToken, false).RunAsync();
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void enrollmentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }
    }
}