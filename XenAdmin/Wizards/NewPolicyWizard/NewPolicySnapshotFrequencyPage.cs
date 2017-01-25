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
using System.Text;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Dialogs;
using XenAdmin.SettingsPanels;
using XenAPI;
using XenAdmin.Core;

namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class NewPolicySnapshotFrequencyPage : XenTabPage, IEditPage
    {
        public NewPolicySnapshotFrequencyPage()
        {
            InitializeComponent();
            radioButtonDaily.Checked = true;
            comboBoxMin.SelectedIndex = 1;
            daysWeekCheckboxes.CheckBoxChanged += checkBox_CheckedChanged;

        }
        private Pool _pool;
        public Pool Pool
        {
            get { return _pool; }
            set
            {
                _pool = value;
                localServerTime1.Pool = _pool;
            }
        }

        public override string Text
        {
            get { return Messages.SNAPSHOT_FREQUENCY; }
        }

        public string SubText
        {
            get { return NewPolicyWizardSpecific<VMPP>.FormatSchedule(Schedule, Frequency, DaysWeekCheckboxes.DaysMode.L10N_SHORT); }
        }

        public Image Image
        {
            get { return Properties.Resources.notif_events_16; }
        }

        public override string PageTitle
        {
            get { return Messages.SNAPSHOT_FREQUENCY_TITLE; }
        }

        public override string HelpID
        {
            get { return "Snapshotfrequency"; }
        }

        public long BackupRetention
        {
            get { return (long)numericUpDownRetention.Value; }
        }

        public Dictionary<string, string> Schedule
        {
            get
            {
                var result = new Dictionary<string, string>();

                if (Frequency == policy_frequency.hourly)
                    result.Add("min", comboBoxMin.SelectedItem.ToString());
                else if (Frequency == policy_frequency.daily)
                {
                    result.Add("hour", dateTimePickerDaily.Value.Hour.ToString());
                    result.Add("min", dateTimePickerDaily.Value.Minute.ToString());
                }
                else if (Frequency == policy_frequency.weekly)
                {
                    result.Add("hour", dateTimePickerWeekly.Value.Hour.ToString());
                    result.Add("min", dateTimePickerWeekly.Value.Minute.ToString());
                    result.Add("days", daysWeekCheckboxes.Days);
                }

                return result;
            }
        }

        public policy_frequency Frequency
        {
            get
            {
                if (radioButtonHourly.Checked) return policy_frequency.hourly;
                else if (radioButtonDaily.Checked) return policy_frequency.daily;
                else if (radioButtonWeekly.Checked) return policy_frequency.weekly;

                throw new ArgumentException("Wrong value");
            }
        }

        private void radioButtonHourly_CheckedChanged(object sender, System.EventArgs e)
        {
            ShowPanel(panelHourly);
            numericUpDownRetention.Value = 10;
            OnPageUpdated();
        }

        private void ShowPanel(Panel panel)
        {
            panelHourly.Visible = false;
            panelDaily.Visible = false;
            panelWeekly.Visible = false;
            panelHourly.Dock = DockStyle.None;
            panelDaily.Dock = DockStyle.None;
            panelHourly.Dock = DockStyle.None;
            panel.Dock = DockStyle.Fill;
            panel.Visible = true;

        }

        private void radioButtonDaily_CheckedChanged(object sender, System.EventArgs e)
        {
            ShowPanel(panelDaily);
            numericUpDownRetention.Value = 7;
            OnPageUpdated();
        }

        private void radioButtonWeekly_CheckedChanged(object sender, System.EventArgs e)
        {
            ShowPanel(panelWeekly);
            daysWeekCheckboxes.Days = "monday";
            numericUpDownRetention.Value = 4;
            OnPageUpdated();
        }

        private void RefreshTab(IVMPolicy policy)
        {
            if (ParentForm != null)
            {
                var parentFormType = ParentForm.GetType();

                if (parentFormType == typeof(XenWizardBase))
                    sectionLabelSchedule.LineColor = sectionLabelNumber.LineColor = SystemColors.Window;
                else if (parentFormType == typeof(PropertiesDialog))
                    sectionLabelSchedule.LineColor = sectionLabelNumber.LineColor = SystemColors.ActiveBorder;
            }

            switch (policy.policy_frequency)
            {
                case policy_frequency.hourly:
                    radioButtonHourly.Checked = true;
                    SetHourlyMinutes(Convert.ToDecimal(policy.backup_schedule_min));
                    break;
                case policy_frequency.daily:
                    radioButtonDaily.Checked = true;
                    dateTimePickerDaily.Value = new DateTime(1970, 1, 1, Convert.ToInt32(policy.backup_schedule_hour),
                                                                 Convert.ToInt32(policy.backup_schedule_min), 0);
                    break;
                case policy_frequency.weekly:
                    radioButtonWeekly.Checked = true;
                    dateTimePickerWeekly.Value = new DateTime(1970, 1, 1, Convert.ToInt32(policy.backup_schedule_hour),
                                                                 Convert.ToInt32(policy.backup_schedule_min), 0);
                    daysWeekCheckboxes.Days = policy.backup_schedule_days;
                    break;
            }

            numericUpDownRetention.Value = policy.policy_retention;
        }

        private void SetHourlyMinutes(decimal min)
        {
            if (min == 0)
                comboBoxMin.SelectedIndex = 0;
            else if (min == 15)
                comboBoxMin.SelectedIndex = 1;
            else if (min == 30)
                comboBoxMin.SelectedIndex = 2;
            else if (min == 45)
                comboBoxMin.SelectedIndex = 3;
            else comboBoxMin.SelectedIndex = 1;

        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (!checkBox.Checked && daysWeekCheckboxes.Days == "")
            {
                checkBox.Checked = true;
            }
        }

        public AsyncAction SaveSettings()
        {
            _policyCopy.policy_frequency = Frequency;
            _policyCopy.policy_schedule = Schedule;
            _policyCopy.policy_retention = BackupRetention;
            return null;
        }

        private IVMPolicy _policyCopy;

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _policyCopy = (IVMPolicy)clone;
            RefreshTab(_policyCopy);
        }

        public bool ValidToSave
        {
            get
            {
                _policyCopy.policy_frequency = Frequency;
                _policyCopy.policy_schedule = Schedule;
                _policyCopy.policy_retention = BackupRetention;
                return true;
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
                if (!Helper.AreEqual2(_policyCopy.policy_frequency, Frequency))
                    return true;
                if (!Helper.AreEqual2(_policyCopy.policy_schedule, Schedule))
                    return true;
                if (!Helper.AreEqual2(_policyCopy.policy_retention, BackupRetention))
                    return true;

                return false;
            }
        }
    }
}
