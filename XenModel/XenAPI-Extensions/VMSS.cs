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
using System.Linq;
using XenAdmin;
using XenAdmin.Alerts;
using XenAdmin.Core;


namespace XenAPI
{
    public partial class VMSS 
    {
        public DateTime GetNextRunTime()
        {
            var time = Host.get_server_localtime(Connection.Session, Helpers.GetMaster(Connection).opaque_ref);

            if (frequency == vmss_frequency.hourly)
            {
                return GetHourlyDate(time, BackupScheduleMin());
            }
            if (frequency == vmss_frequency.daily)
            {
                return GetDailyDate(time, BackupScheduleMin(), BackupScheduleHour());
            }
            if (frequency == vmss_frequency.weekly)
            {
                return GetWeeklyDate(time, BackupScheduleHour(), BackupScheduleMin(), new List<DayOfWeek>(DaysOfWeekBackup()));
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

        public IEnumerable<DayOfWeek> DaysOfWeekBackup()
        {
            return GetDaysFromDictionary(schedule);
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
        
        public override string Name()
        {
            return name_label;
        }

        public override string Description()
        {
            return name_description;
        }

        public int BackupScheduleMin()
        {
            string outStr;
            int result;
            if (schedule.TryGetValue("min", out outStr) && int.TryParse(outStr, out result))
                return result;

            return 0;
        }

        public int BackupScheduleHour()
        {
            string outStr;
            int result;
            if (schedule.TryGetValue("hour", out outStr) && int.TryParse(outStr, out result))
                return result;

            return 0;
        }

        public string BackupScheduleDays()
        {
            string outStr;
            if (schedule.TryGetValue("days", out outStr))
                return outStr;

            return string.Empty;
        }

        /// <summary>
        /// If hoursFromNow is 0, this returns only the top 10 messages regardless timestamp.
        /// Note the messages are ordered by descending timestamp.
        /// </summary>
        public static List<PolicyAlert> GetAlerts(VMSS vmss, int hoursFromNow)
        {
            var messages = vmss.Connection.Cache.Messages;

            var policyMessages = (from XenAPI.Message msg in messages
                    where msg.cls == cls.VMSS
                    group msg by msg.obj_uuid
                    into g
                    let gOrdered = g.OrderByDescending(m => m.timestamp).ToList()
                    select new {PolicyUuid = g.Key, PolicyMessages = gOrdered})
                .ToDictionary(x => x.PolicyUuid, x => x.PolicyMessages);

            var listAlerts = new List<PolicyAlert>();

            DateTime currentTime = DateTime.Now;
            DateTime offset = currentTime.Add(new TimeSpan(-hoursFromNow, 0, 0));

            List<XenAPI.Message> value;
            if (policyMessages.TryGetValue(vmss.uuid, out value))
            {
                if (hoursFromNow == 0)
                {
                    for (int i = 0; i < 10 && i < value.Count; i++)
                    {
                        var msg = value[i];
                        listAlerts.Add(new PolicyAlert(msg.priority, msg.name, msg.timestamp, msg.body, vmss.Name()));
                    }
                }
                else
                {
                    foreach (var msg in value)
                    {
                        if (msg.timestamp >= offset)
                            listAlerts.Add(new PolicyAlert(msg.priority, msg.name, msg.timestamp, msg.body, vmss.Name()));
                        else
                            break;
                    }
                }
            }

            return listAlerts;
        }
    }
}
