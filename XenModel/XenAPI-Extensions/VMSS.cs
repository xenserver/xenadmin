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
using XenAdmin;
using XenAdmin.Alerts;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAdmin.Actions;

namespace XenAPI
{
    public partial class VMSS 
    {
        public bool is_running
        {
            get { return false; }
        }

        public List<PolicyAlert> PolicyAlerts
        {
            get { return Alerts; }
        }

        public DateTime GetNextRunTime()
        {
            var time = Host.get_server_localtime(Connection.Session, Helpers.GetMaster(Connection).opaque_ref);

            if (frequency == vmss_frequency.hourly)
            {
                return GetHourlyDate(time, Convert.ToInt32(backup_schedule_min));
            }
            if (frequency == vmss_frequency.daily)
            {

                var hour = Convert.ToInt32(backup_schedule_hour);
                var min = Convert.ToInt32(backup_schedule_min);
                return GetDailyDate(time, min, hour);

            }
            if (frequency == vmss_frequency.weekly)
            {
                var hour = Convert.ToInt32(backup_schedule_hour);
                var min = Convert.ToInt32(backup_schedule_min);
                return GetWeeklyDate(time, hour, min, new List<DayOfWeek>(DaysOfWeekBackup));
            }
            return new DateTime();

        }

        public static DateTime GetDailyDate(DateTime time, int min, int hour)
        {
            var nextDateTime = new DateTime(time.Year, time.Month, time.Day, hour, min, 0);
            if (time > nextDateTime)
                nextDateTime = nextDateTime.AddDays(1);
            return nextDateTime;
        }

        public static DateTime GetHourlyDate(DateTime time, int min)
        {
            var nextDateTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, min, 0);
            if (time > nextDateTime)
                nextDateTime = nextDateTime.AddHours(1);
            return nextDateTime;
        }

        public static DateTime GetWeeklyDate(DateTime time, int hour, int min, List<DayOfWeek> listDaysOfWeek)
        {
            listDaysOfWeek.Sort();

            int daysOfDifference;
            DayOfWeek today = time.DayOfWeek;

            int nextDay = listDaysOfWeek.FindIndex(x => x >= time.DayOfWeek);

            // No scheduled days later in the week: take first day next week
            if (nextDay < 0)
            {
                daysOfDifference = 7 - (today - listDaysOfWeek[0]);
            }
            else
            {
                daysOfDifference = listDaysOfWeek[nextDay] - today;

                // Today is a scheduled day: but is the time already past?
                if (daysOfDifference == 0)
                {
                    var todaysScheduledTime = new DateTime(time.Year, time.Month, time.Day, hour, min, 0);
                    if (time > todaysScheduledTime)
                    {
                        // Yes, the time is already past. Find the next day in the schedule instead.
                        if (listDaysOfWeek.Count == nextDay + 1)  // we're at the last scheduled day in the week: go to next week
                            daysOfDifference = 7 - (today - listDaysOfWeek[0]);
                        else
                            daysOfDifference = listDaysOfWeek[nextDay + 1] - today;
                    }
                }
            }
            return (new DateTime(time.Year, time.Month, time.Day, hour, min, 0)).AddDays(daysOfDifference);
        }

        public IEnumerable<DayOfWeek> DaysOfWeekBackup
        {
            get
            {
                return GetDaysFromDictionary(schedule);
            }
        }

        private static IEnumerable<DayOfWeek> GetDaysFromDictionary(Dictionary<string, string> dictionary)
        {
            if (dictionary.ContainsKey("days"))
            {
                if (dictionary["days"].IndexOf("monday", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    yield return DayOfWeek.Monday;
                if (dictionary["days"].IndexOf("tuesday", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    yield return DayOfWeek.Tuesday;
                if (dictionary["days"].IndexOf("wednesday", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    yield return DayOfWeek.Wednesday;
                if (dictionary["days"].IndexOf("thursday", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    yield return DayOfWeek.Thursday;
                if (dictionary["days"].IndexOf("friday", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    yield return DayOfWeek.Friday;
                if (dictionary["days"].IndexOf("saturday", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    yield return DayOfWeek.Saturday;
                if (dictionary["days"].IndexOf("sunday", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    yield return DayOfWeek.Sunday;
            }
        }
        
        public static string TryGetKey(Dictionary<string, string> dict, string key)
        {
            string r;
            if (dict.TryGetValue(key, out r))
            {
                return r;
            }
            return "";
        }

        public override string Name
        {
            get { return name_label; }
        }

        public override string Description
        {
            get { return name_description; }
        }

        public string backup_schedule_min
        {
            get
            {
                return TryGetKey(schedule, "min");
            }
        }

        public string backup_schedule_hour
        {
            get
            {

                return TryGetKey(schedule, "hour");
            }
        }

        public string backup_schedule_days
        {
            get
            {

                return TryGetKey(schedule, "days");
            }
        }
        private List<PolicyAlert> _alerts = new List<PolicyAlert>();

        public List<PolicyAlert> Alerts
        {
            get
            {
                return _alerts;
            }
            set { _alerts = value; }
        }

        public string LastResult
        {
            get
            {
                if (_alerts.Count > 0)
                {
                    var listRecentAlerts = new List<PolicyAlert>(_alerts);
                    listRecentAlerts.Sort((x, y) => y.Time.CompareTo(x.Time));
                    if (listRecentAlerts[0].Type == "info")
                        return Messages.VMSS_SUCCEEDED;

                    return Messages.FAILED;
                }
                return Messages.NOT_YET_RUN;
            }
        }
    }
}
