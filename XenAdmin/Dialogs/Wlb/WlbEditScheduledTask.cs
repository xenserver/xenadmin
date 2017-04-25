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
using System.Globalization;
using XenAdmin.Wlb;
using XenAdmin.Controls.Wlb;
using XenAdmin.Core;


namespace XenAdmin.Dialogs.Wlb
{
    public partial class WlbEditScheduledTask : XenAdmin.Dialogs.XenDialogBase
    {

        #region Private Fields
        
        private WlbScheduledTask _task;

        #endregion

        #region Public Properties

        public WlbScheduledTask Task
        {
            get { return _task; }
        }

        #endregion

        #region Constructors
        public WlbEditScheduledTask(WlbScheduledTask Task)
        {
            InitializeComponent();
            _task = Task;
            PopulateControls();
            InitializeControls();
        }

        public WlbEditScheduledTask(int NewTaskId, WlbScheduledTask.WlbTaskActionType ActionType)
        {
            InitializeComponent();
            _task = new WlbScheduledTask(NewTaskId.ToString());
            _task.ActionType = ActionType;
            PopulateControls();
        }
        #endregion

        #region Private Methods
        private void PopulateControls()
        {
            comboDayOfWeek.DataSource = new BindingSource(BuildDays(), null);
            comboDayOfWeek.ValueMember = "key";
            comboDayOfWeek.DisplayMember = "value";

            comboOptMode.DataSource = new BindingSource(BuildOptModes(), null);
            comboOptMode.ValueMember = "key";
            comboOptMode.DisplayMember = "value";

            comboBoxHour.DataSource = new BindingSource(BuildHours(), null);
            comboBoxHour.ValueMember = "key";
            comboBoxHour.DisplayMember = "value";

            //comboBoxMinute.DataSource = new BindingSource(BuildMinutes(), null);
            //comboBoxMinute.ValueMember = "key";
            //comboBoxMinute.DisplayMember = "value";
        }

        public WlbScheduledTask.WlbTaskDaysOfWeek FindSelectedDay(WlbScheduledTask.WlbTaskDaysOfWeek daysOfWeek)
        {
            uint bitCount = WlbOptModeScheduler.Bitcount((int)daysOfWeek);
            if (bitCount == 2)
            {
                return WlbScheduledTask.WlbTaskDaysOfWeek.Weekends;
            }
            else if (bitCount == 5)
            {
                return WlbScheduledTask.WlbTaskDaysOfWeek.Weekdays;
            }
            else if (bitCount == 7)
            {
                return WlbScheduledTask.WlbTaskDaysOfWeek.All;
            }
            return daysOfWeek;
        }

        private void InitializeControls()
        {
            DateTime localExecuteTime;
            WlbScheduledTask.WlbTaskDaysOfWeek localDaysOfWeek;
            WlbScheduledTask.GetLocalTaskTimes(_task.DaysOfWeek, _task.ExecuteTime, out localDaysOfWeek, out localExecuteTime);

            //comboDayOfWeek.SelectedValue = (int)localDaysOfWeek;
            comboDayOfWeek.SelectedValue = (int)FindSelectedDay(localDaysOfWeek);
            comboBoxHour.SelectedValue = localExecuteTime.Hour;
            //comboBoxMinute.SelectedValue = (((int)(localExecuteTime.Minute * 4 / 60)) * 15);
            //dtExecuteTime.Value = localExecuteTime;
            comboOptMode.SelectedValue = (int)GetTaskOptMode(_task);
            checkBoxEnable.Checked = _task.Enabled;
        }

        private SortedDictionary<int, string> BuildHours()
        {
            SortedDictionary<int, string> hours = new SortedDictionary<int, string>();
            for (int hour = 0; hour <= 23; hour++)
            {
                DateTime time = new DateTime(DateTime.Now.Year, 1, 1, hour, 0, 0);
                hours.Add(hour, HelpersGUI.DateTimeToString(time, Messages.DATEFORMAT_HM, true));
            }
            return hours;
        }

        //private SortedDictionary<int, string> BuildMinutes()
        //{
        //    SortedDictionary<int, string> minutes = new SortedDictionary<int, string>();
        //    for (int minute = 0; minute <= 45; minute += 15)
        //    {
        //        minutes.Add(minute, minute.ToString().PadLeft(2, '0'));
        //    }
        //    return minutes;
        //}

        private Dictionary<int, string> BuildDays()
        {
            Dictionary<int, string> days = new Dictionary<int, string>();

            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.All, GetDayString(WlbScheduledTask.WlbTaskDaysOfWeek.All));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Weekdays, GetDayString(WlbScheduledTask.WlbTaskDaysOfWeek.Weekdays));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Weekends, GetDayString(WlbScheduledTask.WlbTaskDaysOfWeek.Weekends));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Sunday, GetDayString(WlbScheduledTask.WlbTaskDaysOfWeek.Sunday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Monday, GetDayString(WlbScheduledTask.WlbTaskDaysOfWeek.Monday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Tuesday, GetDayString(WlbScheduledTask.WlbTaskDaysOfWeek.Tuesday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday, GetDayString(WlbScheduledTask.WlbTaskDaysOfWeek.Wednesday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Thursday, GetDayString(WlbScheduledTask.WlbTaskDaysOfWeek.Thursday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Friday, GetDayString(WlbScheduledTask.WlbTaskDaysOfWeek.Friday));
            days.Add((int)WlbScheduledTask.WlbTaskDaysOfWeek.Saturday, GetDayString(WlbScheduledTask.WlbTaskDaysOfWeek.Saturday));

            return days;
        }

        private string GetDayString(WlbScheduledTask.WlbTaskDaysOfWeek day)
        {
            return WlbScheduledTask.DaysOfWeekL10N(day);
        }

        private Dictionary<int, string> BuildOptModes()
        {
            Dictionary<int, string> modes = new Dictionary<int, string>();
            foreach (WlbPoolPerformanceMode mode in Enum.GetValues(typeof(WlbPoolPerformanceMode)))
            {
                modes.Add((int)mode, Messages.ResourceManager.GetString("WLB_OPT_MODE_" + mode.ToString().ToUpper()));
            }
            return modes;
        }

        private static WlbPoolPerformanceMode GetTaskOptMode(WlbScheduledTask task)
        {
            WlbPoolPerformanceMode mode;

            if (task.TaskParameters["OptMode"] == "0")
            {
                mode = WlbPoolPerformanceMode.MaximizePerformance;
            }
            else
            {
                mode = WlbPoolPerformanceMode.MaximizeDensity;
            }
            return mode;
        }

        //private static WlbPoolPerformanceMode GetTaskOptMode(string OptModeValue)
        //{
        //    if (OptModeValue == "0")
        //    {
        //        return WlbPoolPerformanceMode.MaximizePerformance;
        //    }
        //    else
        //    {
        //        return WlbPoolPerformanceMode.MaximizeDensity;
        //    }
        //}
        #endregion

        #region Control Event Handlers
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (_task.TaskParameters.ContainsValue("OptMode"))
            {
                _task.TaskParameters["OptMode"] = comboOptMode.SelectedValue.ToString();
            }
            else
            {
                _task.AddTaskParameter("OptMode", comboOptMode.SelectedValue.ToString());
            }
            _task.Enabled = checkBoxEnable.Checked;

            DateTime utcExecuteTime;
            WlbScheduledTask.WlbTaskDaysOfWeek utcDaysOfWeek;

            // Have to do this to trim off seconds and milliseconds
            DateTime executeTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (int)comboBoxHour.SelectedValue, 0, 0);

            WlbScheduledTask.GetUTCTaskTimes((WlbScheduledTask.WlbTaskDaysOfWeek)comboDayOfWeek.SelectedValue, executeTime, out utcDaysOfWeek, out utcExecuteTime);

            _task.ExecuteTime = utcExecuteTime;
            _task.DaysOfWeek = utcDaysOfWeek;
        }
        #endregion
    }
}