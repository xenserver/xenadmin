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
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.SettingsPanels;
using XenAdmin.Wizards.NewSRWizard_Pages;
using XenAPI;


namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class NewPolicyArchivePage : XenTabPage, IEditPage
    {
        public NewPolicyArchivePage()
        {
            InitializeComponent();

            CueBannersManager.SetWatermark(textBoxPath, Messages.ARCHIVE_VMPP_EXAMPLE_STORAGE_PATH);
            m_labelError.Visible = false;
            radioButtonDoNotArchive.Checked = true;
            ArchiveTargetType = vmpp_archive_target_type.none;
            daysWeekCheckboxes1.CheckBoxChanged += checkBox_CheckedChanged;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            RefreshEnablementCheckBoxes();
        }

        private void RefreshEnablementCheckBoxes()
        {
            if ((_clone == null && daysWeekCheckboxes1.Days != "" && BackupDaysCount <= daysWeekCheckboxes1.Days.Split(',').Length) && SnapshotFrequency == vmpp_backup_frequency.weekly
            || (_clone != null && daysWeekCheckboxes1.Days != "" && new List<DayOfWeek>(_clone.DaysOfWeekBackup).Count <= daysWeekCheckboxes1.Days.Split(',').Length) && _clone.backup_frequency == vmpp_backup_frequency.weekly)
                daysWeekCheckboxes1.DisableUnSelected();
            else
                daysWeekCheckboxes1.EnableAll();
            OnPageUpdated();
            EnableOkButton();
        }

        public override string Text
        {
            get
            {
                return Messages.ARCHIVE_SNAPSHOTS;
            }
        }

        public string SubText
        {
            get { return NewPolicyWizardSpecific<VMPP>.FormatSchedule(Schedule, ArchiveFrequency, DaysWeekCheckboxes.DaysMode.L10N_SHORT); }
        }

        public override string HelpID
        {
            get { return "Archivesnapshots"; }
        }

        public Image Image
        {
            get { return Properties.Resources._000_BackupMetadata_h32bit_16; }
        }

        public override string PageTitle
        {
            get
            {
                return Messages.ARCHIVE_SNAPSHOTS_TITLE;
            }
        }

        public override void PageLoaded(PageLoadedDirection direction)
        {
            base.PageLoaded(direction);
            RefreshDailyEnabled(SnapshotFrequency);
        }

        public vmpp_backup_frequency SnapshotFrequency { private get; set; }
        public int BackupDaysCount { private get; set; }

        private void RepositionArchiveDailyRadioButton()
        {
            tableLayoutPanel5.SuspendLayout();
            try
            {
                //Enabled radioButtonArchiveDaily should be in one container with other radio buttons
                if (radioButtonArchiveDaily.Enabled && radioButtonArchiveDaily.Parent == toolTipContainer1)
                {
                    toolTipContainer1.Controls.Remove(radioButtonArchiveDaily);
                    radioButtonArchiveDaily.Dock = DockStyle.None;
                    tableLayoutPanel5.Controls.Add(radioButtonArchiveDaily, 0, 2);
                    toolTipContainer1.Visible = false;
                }

                //Disabled radioButtonArchiveDaily should be in the toolTipContainer1 to display tool tip correctly
                if (!radioButtonArchiveDaily.Enabled && radioButtonArchiveDaily.Parent == tableLayoutPanel5)
                {
                    tableLayoutPanel5.Controls.Remove(radioButtonArchiveDaily);
                    radioButtonArchiveDaily.Dock = DockStyle.Fill;
                    toolTipContainer1.Controls.Add(radioButtonArchiveDaily);
                    toolTipContainer1.Visible = true;
                }
            }
            finally
            {
                tableLayoutPanel5.ResumeLayout();
            }
        }
        
        private void RefreshDailyEnabled(vmpp_backup_frequency snapshotFrequency)
        {
            bool controlsEnabled = snapshotFrequency != vmpp_backup_frequency.weekly;
            radioButtonArchiveDaily.Enabled = dateTimePickerDaily.Enabled = controlsEnabled;
            RepositionArchiveDailyRadioButton();
            if (controlsEnabled)
                toolTipContainer1.RemoveAll();
            else
                toolTipContainer1.SetToolTip(Messages.CANNOT_CONFIGURE_MORE_FREQ_ARCHIVE);
        }

        public vmpp_archive_frequency ArchiveFrequency
        {
            get
            {
                if (radioButtonDoNotArchive.Checked)
                    return vmpp_archive_frequency.never;
                else if (radioButtonArchiveASAP.Checked)
                    return vmpp_archive_frequency.always_after_backup;
                else if (radioButtonArchiveDaily.Checked)
                    return vmpp_archive_frequency.daily;
                else if (radioButtonArchiveWeekly.Checked)
                    return vmpp_archive_frequency.weekly;
                else return vmpp_archive_frequency.unknown;
            }
        }

        public Dictionary<string, string> ArchiveConfig
        {
            get
            {
                var result = new Dictionary<string, string>();
                if (ArchiveFrequency != vmpp_archive_frequency.never)
                {
                    result.Add("location", textBoxPath.Text);

                    if (ArchiveTargetType == vmpp_archive_target_type.cifs && checkBoxCredentials.Checked)
                    {
                        result.Add("username", textBoxUser.Text);
                        result.Add("password", textBoxPassword.Text);
                    }
                }
                return result;
            }
        }

        private bool ValidateArchiveScheduleDetails
        {
            get
            {
                return radioButtonArchiveDaily.Checked || radioButtonArchiveWeekly.Checked ||
                       radioButtonArchiveASAP.Checked;
            }
        }

        private bool IsPathValid
        {
            get
            {
                return (pathTestStatus == PathTestStatus.NotStarted || pathTestStatus == PathTestStatus.Success) &&
                       (ArchiveTargetType == vmpp_archive_target_type.cifs ||
                        ArchiveTargetType == vmpp_archive_target_type.nfs);
            }
        }

        private bool DifferentUserCredentialsRequired
        {
            get { return ArchiveTargetType == vmpp_archive_target_type.cifs && checkBoxCredentials.Checked; }
        }

        public override bool EnableNext()
        {
            if (pathTestStatus == PathTestStatus.InProgress)
                return false;

            if (ValidateArchiveScheduleDetails)
            {
                if (!IsPathValid)
                    return false;
                if (radioButtonArchiveWeekly.Checked && daysWeekCheckboxes1.Days == "")
                    return false;
                if (radioButtonArchiveDaily.Checked && !radioButtonArchiveDaily.Enabled)
                    return false;
                if (DifferentUserCredentialsRequired)
                    return !string.IsNullOrEmpty(textBoxUser.Text);
            }
            
            return true;
        }

        public Dictionary<string, string> Schedule
        {
            get
            {
                var result = new Dictionary<string, string>();
                if (ArchiveFrequency == vmpp_archive_frequency.daily || ArchiveFrequency == vmpp_archive_frequency.weekly)
                {
                    result.Add("hour", radioButtonArchiveDaily.Checked ? dateTimePickerDaily.Value.Hour.ToString() : dateTimePickerWeekly.Value.Hour.ToString());
                    result.Add("min", radioButtonArchiveDaily.Checked ? dateTimePickerDaily.Value.Minute.ToString() : dateTimePickerWeekly.Value.Minute.ToString());

                }
                if (ArchiveFrequency == vmpp_archive_frequency.weekly)
                {
                    result.Add("days", daysWeekCheckboxes1.Days);
                }
                return result;
            }
        }

        private void RefreshTab(VMPP vmpp)
        {
            sectionLabelSchedule.LineColor = sectionLabelDest.LineColor =
                                             PropertiesDialog == null ? SystemColors.Window : SystemColors.ActiveBorder;

            switch (vmpp.archive_frequency)
            {
                case vmpp_archive_frequency.always_after_backup:
                    radioButtonArchiveASAP.Checked = true;
                    break;
                case vmpp_archive_frequency.daily:

                    radioButtonArchiveDaily.Checked = true;
                    dateTimePickerDaily.Value = new DateTime(1970, 1, 1, Convert.ToInt32(vmpp.archive_schedule["hour"]),
                                                           Convert.ToInt32(vmpp.archive_schedule["min"]), 0);
                    break;
                case vmpp_archive_frequency.weekly:

                    radioButtonArchiveWeekly.Checked = true;
                    dateTimePickerWeekly.Value = new DateTime(1970, 1, 1, Convert.ToInt32(vmpp.archive_schedule["hour"]),
                                                             Convert.ToInt32(vmpp.archive_schedule["min"]), 0);
                    daysWeekCheckboxes1.Days = vmpp.archive_schedule["days"];


                    break;
                case vmpp_archive_frequency.never:
                    radioButtonDoNotArchive.Checked = true;
                    break;
            }
            if (vmpp.archive_frequency != vmpp_archive_frequency.never)
            {
                if (vmpp.archive_target_type == vmpp_archive_target_type.nfs)
                {
                    textBoxPath.Text = vmpp.archive_target_config_location;
                }
                else
                {
                    textBoxPath.Text = vmpp.archive_target_config_location;
                }
                if (vmpp.archive_target_type == vmpp_archive_target_type.cifs)
                {
                    if (vmpp.archive_target_config_username != "")
                    {
                        checkBoxCredentials.Checked = true;

                        textBoxUser.Text = vmpp.archive_target_config_username;
                        textBoxPassword.Text = vmpp.archive_target_config_password_value;
                    }
                }
            }

            RefreshDailyEnabled(vmpp.backup_frequency);
            RefreshEnablementCheckBoxes();
        }

        private Pool _pool;
        public Pool Pool
        {
            get { return _pool; }
            set
            {
                _pool = value;
                localServerTime1.Pool = value;
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            TestPath();
        }

        private enum PathTestStatus { NotStarted, InProgress, Success, Error }
        private PathTestStatus pathTestStatus = PathTestStatus.NotStarted;

        private void ResetPathTestStatus()
        {
            if (pathTestStatus != PathTestStatus.NotStarted)
            {
                m_labelError.Visible = false;
                pictureBoxTest.Visible = false;
                pathTestStatus = PathTestStatus.NotStarted;
            }
        }
        
        private void TestPath()
        {
            textBoxPath.Enabled = false;
            m_labelError.Visible = false;
            pictureBoxTest.Image = Properties.Resources.ajax_loader;
            pictureBoxTest.Visible = true;
            var archiveConfig = ArchiveConfig;
            
            //todo: get string for archive destination
            archiveConfig.Add("type", vmpp_archive_target_type_helper.ToString(ArchiveTargetType));
            var action = new TestArchiveTargetAction(Pool.Connection, archiveConfig);
            action.Completed += action_Completed;
            // mark test as in progress; this will disable the Next button (or OK button for Properties dialog) until the test is complete
            pathTestStatus = PathTestStatus.InProgress;
            OnPageUpdated();
            EnableOkButton();
            action.RunAsync();
        }

        private void action_Completed(ActionBase sender)
        {
            AsyncAction action = (AsyncAction)sender;

            Program.Invoke(this, () =>
                                     {
                                         if (action.Result != "True" || action.Result == null)
                                         {
                                             m_labelError.Visible = true;
                                             pathTestStatus = PathTestStatus.Error;
                                             pictureBoxTest.Image = Properties.Resources._000_Abort_h32bit_16;
                                         }
                                         else
                                         {
                                             pathTestStatus = PathTestStatus.Success;
                                             pictureBoxTest.Image = Properties.Resources._000_Tick_h32bit_16;
                                         }
                                         textBoxPath.Enabled = true;
                                         OnPageUpdated();
                                         EnableOkButton();
                                     });
        }

        public vmpp_archive_target_type ArchiveTargetType { get; private set; }

        public AsyncAction SaveSettings()
        {
            _clone.archive_frequency = ArchiveFrequency;
            _clone.archive_schedule = Schedule;
            _clone.archive_target_type = ArchiveTargetType;
            _clone.archive_target_config = ArchiveConfig;
            return null;
        }

        private VMPP _clone;
        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            if (_clone != null && clone == null)
            {
                RefreshDailyEnabled(_clone.backup_frequency);
                RefreshEnablementCheckBoxes();
            }
            else
            {
                _clone = (VMPP)clone;
                RefreshTab(_clone);
            }
        }

        public bool ValidToSave
        {
            get
            {
                return EnableNext();
            }
        }

        public void ShowLocalValidationMessages()
        {
        }

        public void Cleanup()
        {
        }

        public bool HasChanged
        {
            get
            {
                if (!Helper.AreEqual2(_clone.archive_frequency, ArchiveFrequency))
                    return true;
                if (!Helper.DictEquals<string, string>(_clone.archive_schedule, Schedule))
                    return true;
                if (!ArchiveConfigIsEqual)
                    return true;
                if (!Helper.AreEqual2(_clone.archive_target_type, ArchiveTargetType))
                    return true;
                return false;
            }
        }

        private bool ArchiveConfigIsEqual
        {
            get
            {
                var archiveConfig = ArchiveConfig;

                if (string.IsNullOrEmpty(_clone.archive_target_config_location))
                {
                    if (archiveConfig.ContainsKey("location") && !string.IsNullOrEmpty(archiveConfig["location"]))
                        return false;
                }
                else if (!archiveConfig.ContainsKey("location") || _clone.archive_target_config_location != archiveConfig["location"])
                    return false;

                if (string.IsNullOrEmpty(_clone.archive_target_config_username))
                {
                    if (archiveConfig.ContainsKey("username") && !string.IsNullOrEmpty(archiveConfig["username"]))
                        return false;
                }
                else if (!archiveConfig.ContainsKey("username") || _clone.archive_target_config_username != archiveConfig["username"])
                        return false;

                if (string.IsNullOrEmpty(_clone.archive_target_config_password_value))
                {
                    if (archiveConfig.ContainsKey("password") && !string.IsNullOrEmpty(archiveConfig["password"]))
                        return false;
                }
                else if (!archiveConfig.ContainsKey("password") || _clone.archive_target_config_password_value != archiveConfig["password"])
                        return false;

                return true;
            }
        }

        private vmpp_archive_target_type GetArchiveTargetType(string path)
        {
            if (SrWizardHelpers.ValidateNfsSharename(path))
                return vmpp_archive_target_type.nfs;
            if (SrWizardHelpers.ValidateCifsSharename(path))
                return vmpp_archive_target_type.cifs;
            return vmpp_archive_target_type.none;
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
            ResetPathTestStatus();
            ArchiveTargetType = GetArchiveTargetType(textBoxPath.Text);
            buttonTest.Enabled = ArchiveTargetType == vmpp_archive_target_type.cifs ||
                                 ArchiveTargetType == vmpp_archive_target_type.nfs;
            checkBoxCredentials.Enabled = m_tlpCredentials.Enabled = ArchiveTargetType == vmpp_archive_target_type.cifs;
            OnPageUpdated();
            EnableOkButton();
        }

        private void textBoxUser_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxUser.Text))
                checkBoxCredentials.Checked = true;
            OnPageUpdated();
            EnableOkButton();
        }

        private void checkBoxCredentials_CheckedChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
            EnableOkButton();
        }

        private void radioButtonArchiveASAP_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonArchiveASAP.Checked)
                return;

            //radioButtonArchiveDaily is in the toolTipContainer and therefore not grouped with the others
            radioButtonArchiveDaily.Checked = false;
            ToggleRecurranceVisible(false);
            ToggleDestinationVisible(true);
            OnPageUpdated();
            EnableOkButton();
        }

        private void radioButtonArchiveDaily_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonArchiveDaily.Checked)
                return;

            //radioButtonArchiveDaily is in the toolTipContainer and therefore not grouped with the others
            radioButtonArchiveASAP.Checked = radioButtonDoNotArchive.Checked = radioButtonArchiveWeekly.Checked = false;
            ToggleRecurranceVisible(true);
            ToggleDailyWeekly(true);
            ToggleDestinationVisible(true);
            OnPageUpdated();
            EnableOkButton();
        }

        private void radioButtonArchiveWeekly_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonArchiveWeekly.Checked)
                return;

            //radioButtonArchiveDaily is in the toolTipContainer and therefore not grouped with the others
            radioButtonArchiveDaily.Checked = false;
            ToggleRecurranceVisible(true);
            ToggleDailyWeekly(false);
            ToggleDestinationVisible(true);
            OnPageUpdated();
            EnableOkButton();
        }

        private void radioButtonDoNotArchive_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButtonDoNotArchive.Checked)
                return;

            //radioButtonArchiveDaily is in the toolTipContainer and therefore not grouped with the others
            radioButtonArchiveDaily.Checked = false;
            ToggleRecurranceVisible(false);
            ToggleDestinationVisible(false);
            OnPageUpdated();
            EnableOkButton();
        }

        private void ToggleDestinationVisible(bool show)
        {
            sectionLabelDest.Visible = m_tlpDestination.Visible = show;
        }

        private void ToggleRecurranceVisible(bool show)
        {
            labelDivider.Visible = m_tlpRecur.Visible = show;
        }

        private void ToggleDailyWeekly(bool showDaily)
        {
            m_tlpRecur.SuspendLayout();
            labelRecurDaily.Visible = dateTimePickerDaily.Visible = showDaily;
            labelRecurWeekly.Visible = dateTimePickerWeekly.Visible = labelDays.Visible = daysWeekCheckboxes1.Visible = !showDaily;
            m_tlpRecur.ResumeLayout();
        }

        private void EnableOkButton()
        {
            if (PropertiesDialog != null)
                PropertiesDialog.okButton.Enabled = EnableNext();
        }

        public PropertiesDialog PropertiesDialog { private get; set; }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxPassword.Text))
                checkBoxCredentials.Checked = true;
        }
    }
}
