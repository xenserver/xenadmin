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
using XenAdmin.Model;
using XenAPI;


namespace XenAdmin.Dialogs.HealthCheck
{
    public partial class HealthCheckSettingsDialog : XenDialogBase
    {
        private readonly Pool pool;
        private HealthCheckSettings healthCheckSettings;
        private bool authenticationRequired;
        private bool authenticated;
        private string authenticationToken;
        private string xsUserName;
        private string xsPassword;

        public HealthCheckSettingsDialog(Pool pool, bool enrollNow)
        {
            this.pool = pool;
            healthCheckSettings = pool.HealthCheckSettings;
            if (enrollNow)
                healthCheckSettings.Status = HealthCheckStatus.Enabled;
            authenticationToken = healthCheckSettings.GetExistingSecretyInfo(pool.Connection, HealthCheckSettings.UPLOAD_TOKEN_SECRET);
            xsUserName = healthCheckSettings.GetSecretyInfo(pool.Connection, HealthCheckSettings.UPLOAD_CREDENTIAL_USER_SECRET);
            xsPassword = healthCheckSettings.GetSecretyInfo(pool.Connection, HealthCheckSettings.UPLOAD_CREDENTIAL_PASSWORD_SECRET);
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

            Text = String.Format(Messages.HEALTHCHECK_ENROLLMENT_TITLE, pool.Name);
            
            authenticationRubricLabel.Text = authenticationRequired
                ? Messages.HEALTHCHECK_AUTHENTICATION_RUBRIC_NO_TOKEN
                : Messages.HEALTHCHECK_AUTHENTICATION_RUBRIC_EXISTING_TOKEN;

            enrollmentCheckBox.Checked = healthCheckSettings.Status != HealthCheckStatus.Disabled;
            frequencyNumericBox.Value = healthCheckSettings.IntervalInWeeks;
            dayOfWeekComboBox.SelectedValue = (int)healthCheckSettings.DayOfWeek;
            timeOfDayComboBox.SelectedValue = healthCheckSettings.TimeOfDay;
            
            existingAuthenticationRadioButton.Enabled = existingAuthenticationRadioButton.Checked = !authenticationRequired;
            newAuthenticationRadioButton.Checked = authenticationRequired;
            SetMyCitrixCredentials(existingAuthenticationRadioButton.Checked);

            bool useCurrentXsCredentials = string.IsNullOrEmpty(xsUserName) || xsUserName == pool.Connection.Username;
            newXsCredentialsRadioButton.Checked = !useCurrentXsCredentials;
            currentXsCredentialsRadioButton.Checked = useCurrentXsCredentials;
            SetXSCredentials(currentXsCredentialsRadioButton.Checked);
        }

        private bool ChangesMade()
        {
            if (enrollmentCheckBox.Checked && healthCheckSettings.Status != HealthCheckStatus.Enabled)
                return true;
            if (!enrollmentCheckBox.Checked && healthCheckSettings.Status != HealthCheckStatus.Disabled)
                return true;
            if (frequencyNumericBox.Value != healthCheckSettings.IntervalInWeeks)
                return true;
            if (dayOfWeekComboBox.SelectedIndex != (int)healthCheckSettings.DayOfWeek)
                return true;
            if (timeOfDayComboBox.SelectedIndex != healthCheckSettings.TimeOfDay)
                return true;
            if (authenticationToken != healthCheckSettings.GetSecretyInfo(pool.Connection, HealthCheckSettings.UPLOAD_TOKEN_SECRET))
                return true;
            if (textboxXSUserName.Text != xsUserName)
                return true;
            if (textboxXSPassword.Text != xsPassword)
                return true;
            return false;
        }

        private void UpdateButtons()
        {
            okButton.Enabled = m_ctrlError.PerformCheck(CheckCredentialsEntered);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            okButton.Enabled = false;
            if (enrollmentCheckBox.Checked && newAuthenticationRadioButton.Checked 
                && !m_ctrlError.PerformCheck(CheckUploadAuthentication))
            {
                okButton.Enabled = true;
                return;
            }

            if (ChangesMade())
            {
                var newHealthCheckSettings = new HealthCheckSettings(
                    enrollmentCheckBox.Checked ? HealthCheckStatus.Enabled : HealthCheckStatus.Disabled,
                    (int)(frequencyNumericBox.Value * 7),
                    (DayOfWeek)dayOfWeekComboBox.SelectedValue,
                    (int)timeOfDayComboBox.SelectedValue,
                    HealthCheckSettings.DefaultRetryInterval);

                new SaveHealthCheckSettingsAction(pool, newHealthCheckSettings, authenticationToken, textboxXSUserName.Text, textboxXSPassword.Text, false).RunAsync();
                new TransferHealthCheckSettingsAction(pool, newHealthCheckSettings, textboxXSUserName.Text, textboxXSPassword.Text, true).RunAsync();
            }
            okButton.Enabled = true;
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

        private void newAuthenticationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetMyCitrixCredentials(existingAuthenticationRadioButton.Checked);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            SetXSCredentials(currentXsCredentialsRadioButton.Checked);
        }

        private void SetXSCredentials(bool useCurrent)
        {
            if (useCurrent)
            {
                textboxXSUserName.Text = pool.Connection.Username;
                textboxXSPassword.Text = pool.Connection.Password;
                textboxXSUserName.Enabled = false;
                textboxXSPassword.Enabled = false;
            }
            else
            {
                textboxXSUserName.Text = xsUserName;
                textboxXSPassword.Text = xsPassword;
                textboxXSUserName.Enabled = true;
                textboxXSPassword.Enabled = true;
            }
        }

        private void SetMyCitrixCredentials(bool useExisting)
        {
            if (useExisting)
            {
                //textBoxMyCitrixUsername.Text = String.Empty;
                //textBoxMyCitrixPassword.Text = String.Empty;
                textBoxMyCitrixUsername.Enabled = false;
                textBoxMyCitrixPassword.Enabled = false;
            }
            else
            {
                //textBoxMyCitrixUsername.Text = String.Empty;
                //textBoxMyCitrixPassword.Text = String.Empty;
                textBoxMyCitrixUsername.Enabled = true;
                textBoxMyCitrixPassword.Enabled = true;
            }
        }
        
        private bool CheckCredentialsEntered()
        {
            if (!enrollmentCheckBox.Checked || !newAuthenticationRadioButton.Checked)
                return true;

            if (newAuthenticationRadioButton.Checked && 
                (string.IsNullOrEmpty(textBoxMyCitrixUsername.Text) || string.IsNullOrEmpty(textBoxMyCitrixPassword.Text)))
                return false;

            if (newXsCredentialsRadioButton.Checked &&
                (string.IsNullOrEmpty(textboxXSUserName.Text) || string.IsNullOrEmpty(textboxXSPassword.Text)))
                return false;

            return true;
        }

        private bool CheckCredentialsEntered(out string error)
        {
            error = string.Empty;
            return CheckCredentialsEntered();
        }

        private bool CheckUploadAuthentication(out string error)
        {
            error = string.Empty;

            if (!CheckCredentialsEntered())
                return false;

            var action = new HealthCheckAuthenticationAction(pool, textBoxMyCitrixUsername.Text.Trim(), textBoxMyCitrixPassword.Text.Trim(),
                Registry.HealthCheckIdentityTokenDomainName, Registry.HealthCheckUploadGrantTokenDomainName, Registry.HealthCheckUploadTokenDomainName,
                Registry.HealthCheckProductKey, true, 0, false);

            try
            {
                action.RunExternal(null);
            }
            catch
            {
                error = action.Exception != null ? action.Exception.Message : Messages.ERROR_UNKNOWN;
                authenticationToken = null;
                authenticated = false;
                return authenticated;
            }

            authenticationToken = action.UploadToken;  // curent upload token
            authenticated = !string.IsNullOrEmpty(authenticationToken);
            authenticationToken = pool.HealthCheckSettings.GetExistingSecretyInfo(pool.Connection, HealthCheckSettings.UPLOAD_TOKEN_SECRET);
            return authenticated;
        }

        private void credentials_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }
    }
}