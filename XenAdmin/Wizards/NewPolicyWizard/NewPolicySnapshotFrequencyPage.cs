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
using System.Globalization;
using System.Windows.Forms;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAdmin.Dialogs;
using XenAdmin.SettingsPanels;
using XenAPI;

namespace XenAdmin.Wizards.NewPolicyWizard
{
    public partial class NewPolicySnapshotFrequencyPage : XenTabPage, IEditPage
    {
        public NewPolicySnapshotFrequencyPage()
        {
            InitializeComponent();
            InitializeProgressControls();
            radioButtonDaily.Checked = true;
            comboBoxMin.SelectedIndex = 1;
            daysWeekCheckboxes.CheckBoxChanged += checkBox_CheckedChanged;
            StatusChanged += PolicySnapshotFrequency_StatusChanged;
            MainTableLayoutPanel.Visible = false;
            LoadingBox.Visible = true;
            spinningTimer.Start();
        }

        private int currentSpinningFrame;
        private Timer spinningTimer = new Timer();
        private ImageList imageList = new ImageList();

        private void InitializeProgressControls()
        {
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(32, 32);
            imageList.Images.AddRange(new Image[]
            {
                Properties.Resources.SpinningFrame0,
                Properties.Resources.SpinningFrame1,
                Properties.Resources.SpinningFrame2,
                Properties.Resources.SpinningFrame3,
                Properties.Resources.SpinningFrame4,
                Properties.Resources.SpinningFrame5,
                Properties.Resources.SpinningFrame6,
                Properties.Resources.SpinningFrame7
            });

            spinningTimer.Tick += timer_Tick;
            spinningTimer.Interval = 150;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            int imageIndex = ++currentSpinningFrame <= 7 ? currentSpinningFrame : currentSpinningFrame = 0;
            pictureBoxProgress.Image = imageList.Images[imageIndex];
        }

        private Pool _pool;
        public Pool Pool
        {
            get { return _pool; }
            set
            {
                _pool = value;
                GetServerTime();
            }
        }

        public void GetServerTime()
        {
            var master = Helpers.GetMaster(Pool);
            if (master == null)
                return;

            var action = new GetServerTimeAction(master);
            action.Completed += action_CompletedTimeServer;
            action.RunAsync();
        }

        private TimeSpan? TimeZoneDiff { get; set; }

        public VMSS CurrentPolicy
        {
            get
            {
                return new VMSS
                {
                    frequency = Frequency,
                    schedule = Schedule,
                    retained_snapshots = BackupRetention
                };
            }
        }

        public override string Text
        {
            get { return Messages.SNAPSHOT_FREQUENCY; }
        }

        public string SubText
        {
            get { return NewPolicyWizard.FormatSchedule(Schedule, Frequency, DaysWeekCheckboxes.DaysMode.L10N_SHORT); }
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

                if (Frequency == vmss_frequency.hourly)
                    result.Add("min", comboBoxMin.SelectedItem.ToString());
                else if (Frequency == vmss_frequency.daily)
                {
                    result.Add("hour", dateTimePickerDaily.Value.Hour.ToString());
                    result.Add("min", dateTimePickerDaily.Value.Minute.ToString());
                }
                else if (Frequency == vmss_frequency.weekly)
                {
                    result.Add("hour", dateTimePickerWeekly.Value.Hour.ToString());
                    result.Add("min", dateTimePickerWeekly.Value.Minute.ToString());
                    result.Add("days", daysWeekCheckboxes.Days);
                }

                return result;
            }
        }

        public vmss_frequency Frequency
        {
            get
            {
                if (radioButtonHourly.Checked) return vmss_frequency.hourly;
                else if (radioButtonDaily.Checked) return vmss_frequency.daily;
                else if (radioButtonWeekly.Checked) return vmss_frequency.weekly;

                throw new ArgumentException("Wrong value");
            }
        }

        private void radioButtonHourly_CheckedChanged(object sender, System.EventArgs e)
        {
            if (sender == null || !((RadioButton) sender).Checked)
                return;

            ShowPanel(panelHourly);
            numericUpDownRetention.Value = 10;
            OnPageUpdated();
        }

        private void ShowPanel(Panel panel)
        {
            var list = new List<Control>{ panelHourly , panelDaily , panelWeekly };
            foreach (var p in list)
            {
                if (p == panel)
                {
                    p.Dock = DockStyle.Fill;
                    p.Visible = true;
                }
                else
                {
                    p.Visible = false;
                    p.Dock = DockStyle.None;
                }
            }
        }

        private void radioButtonDaily_CheckedChanged(object sender, System.EventArgs e)
        {
            if (sender == null || !((RadioButton) sender).Checked)
                return;

            ShowPanel(panelDaily);
            numericUpDownRetention.Value = 7;
            OnPageUpdated();
        }

        private void radioButtonWeekly_CheckedChanged(object sender, System.EventArgs e)
        {
            if (sender == null || !((RadioButton) sender).Checked)
                return;

            ShowPanel(panelWeekly);
            daysWeekCheckboxes.Days = "monday";
            numericUpDownRetention.Value = 4;
            OnPageUpdated();
        }

        private void RefreshTab(VMSS policy)
        {
            if (!TimeZoneDiff.HasValue)
            {
                _applyTimeZoneToPolicy = true;
                return;
            }

            var localPolicy = new VMSS
            {
                frequency = policy.frequency,
                retained_snapshots = policy.retained_snapshots
            };
            
            localPolicy.schedule = VMSS.FindScheduleWithGivenTimeOffset(
                -TimeZoneDiff.Value,
                policy.schedule);

            if (ParentForm != null)
            {
                var parentFormType = ParentForm.GetType();

                if (parentFormType == typeof(XenWizardBase))
                    sectionLabelSchedule.LineColor = sectionLabelNumber.LineColor = SystemColors.Window;
                else if (parentFormType == typeof(PropertiesDialog))
                    sectionLabelSchedule.LineColor = sectionLabelNumber.LineColor = SystemColors.ActiveBorder;
            }

            switch (localPolicy.frequency)
            {
                case vmss_frequency.hourly:
                    radioButtonHourly.Checked = true;
                    SetHourlyMinutes(localPolicy.BackupScheduleMin());
                    break;
                case vmss_frequency.daily:
                    radioButtonDaily.Checked = true;
                    dateTimePickerDaily.Value = new DateTime(1970, 1, 1,
                        localPolicy.BackupScheduleHour(), localPolicy.BackupScheduleMin(), 0);
                    break;
                case vmss_frequency.weekly:
                    radioButtonWeekly.Checked = true;
                    dateTimePickerWeekly.Value = new DateTime(1970, 1, 1,
                        localPolicy.BackupScheduleHour(), localPolicy.BackupScheduleMin(), 0);
                    daysWeekCheckboxes.Days = localPolicy.BackupScheduleDays();
                    break;
            }

            numericUpDownRetention.Value = localPolicy.retained_snapshots;
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
            OnPageUpdated();
        }

        public AsyncAction SaveSettings()
        {
            _policyCopy.frequency = Frequency;

            if (TimeZoneDiff.HasValue)
                _policyCopy.schedule = VMSS.FindScheduleWithGivenTimeOffset(
                    TimeZoneDiff.Value,
                    Schedule);
            else
                _policyCopy.schedule = Schedule;
            
            
            _policyCopy.retained_snapshots = BackupRetention;

            return null;
        }

        private bool _applyTimeZoneToPolicy;
        private VMSS _policyCopy;

        public void SetXenObjects(IXenObject orig, IXenObject clone)
        {
            _policyCopy = (VMSS)clone;
            RefreshTab(_policyCopy);
        }

        public bool ValidToSave
        {
            get
            {
                _policyCopy.frequency = Frequency;
                _policyCopy.schedule = Schedule;
                _policyCopy.retained_snapshots = BackupRetention;
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
                if (!Helper.AreEqual2(_policyCopy.frequency, Frequency))
                    return true;

                if (TimeZoneDiff.HasValue)
                {
                    if (!Helper.AreEqual2(
                        _policyCopy.schedule,
                        VMSS.FindScheduleWithGivenTimeOffset(TimeZoneDiff.Value, Schedule)))
                        return true;
                }
                else
                {
                    if (!Helper.AreEqual2(_policyCopy.schedule, Schedule))
                        return true;
                }

                if (!Helper.AreEqual2(_policyCopy.retained_snapshots, BackupRetention))
                    return true;

                return false;
            }
        }

        private string DateTimeToRequiredFormat(DateTime? datetime)
        {
            if (!datetime.HasValue)
                return Messages.UNKNOWN;

            return HelpersGUI.DateTimeToString(datetime.Value, Messages.DATEFORMAT_WDMY_HM_LONG, true);
        }

        void action_CompletedTimeServer(ActionBase sender)
        {
            var action = (GetServerTimeAction)sender;
            Program.Invoke(Program.MainWindow, () =>
            {
                string serverLocalTimeString = action.Result;
                if (serverLocalTimeString != "")
                {
                    var serverUtcTime = DateTime.Parse(serverLocalTimeString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                    TimeZoneDiff = Util.RoundToNearestMinute(DateTime.UtcNow - serverUtcTime + Pool.Connection.ServerTimeOffset);

                    if (_applyTimeZoneToPolicy)
                    {
                        RefreshTab(_policyCopy);
                        _applyTimeZoneToPolicy = false;
                    }

                    MainTableLayoutPanel.Visible = true;
                    LoadingBox.Visible = false;
                    spinningTimer.Stop();

                    OnPageUpdated();
                }
            });
        }

        private void PolicySnapshotFrequency_StatusChanged(XenTabPage sender)
        {
            DateTime? nextRunTime = null;
            DateTime? correspondingServerTime = null;
            if (TimeZoneDiff.HasValue)
            {
                var currentPolicy = CurrentPolicy;
                nextRunTime = currentPolicy.GetNextRunTime(DateTime.Now);
                correspondingServerTime = nextRunTime + TimeZoneDiff.Value;
            }

            TimeDetailsLabel.Text = string.Format(
                Messages.VMSS_NEXT_TIME_RUNNING,
                DateTimeToRequiredFormat(nextRunTime),
                DateTimeToRequiredFormat(correspondingServerTime));
        }

        private void dateTimePickerWeekly_ValueChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void comboBoxMin_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }

        private void dateTimePickerDaily_ValueChanged(object sender, EventArgs e)
        {
            OnPageUpdated();
        }
    }
}
