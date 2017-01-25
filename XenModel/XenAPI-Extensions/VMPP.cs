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
    public partial class VMPP : IVMPolicy
    {
        public DateTime GetNextRunTime()
        {
            var time = Host.get_server_localtime(Connection.Session, Helpers.GetMaster(Connection).opaque_ref);

            if (backup_frequency == vmpp_backup_frequency.hourly)
            {
                return GetHourlyDate(time, Convert.ToInt32(backup_schedule_min));
            }
            if (backup_frequency == vmpp_backup_frequency.daily)
            {

                var hour = Convert.ToInt32(backup_schedule_hour);
                var min = Convert.ToInt32(backup_schedule_min);
                return GetDailyDate(time, min, hour);

            }
            if (backup_frequency == vmpp_backup_frequency.weekly)
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
                return GetDaysFromDictionary(backup_schedule);
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

        public IEnumerable<DayOfWeek> DaysOfWeekArchive
        {
            get
            {
                return GetDaysFromDictionary(archive_schedule);

            }
        }

        public override string Name
        {
            get { return name_label; }
        }

        public override string Description
        {
            get { return name_description; }
        }

        public bool is_enabled
        {
            get { return this.is_policy_enabled; }
        }

        public bool is_running
        {
            get { return this.is_backup_running; }
        }

        public bool is_archiving
        {
            get { return this.is_archive_running; }
        }

        public Type _Type
        {
            get {return typeof(VMPP);}
        }

        public List<PolicyAlert> PolicyAlerts
        {
            get { return RecentAlerts;}
        }

        public bool hasArchive
        {
            get { return true; }
        }
        
        public void set_vm_policy(Session session, string _vm, string _value)
        {
            VM.set_protection_policy(session, _vm, _value);
        }

        public void do_destroy(Session session, string _policy)
        {
            VMPP.destroy(session, _policy);
        }

        public string run_now(Session session, string _policy)
        {
            return VMPP.protect_now(session, _policy);
        }

        public void set_is_enabled(Session session, string _policy, bool _is_enabled)
        {
            VMPP.set_is_policy_enabled(session, _policy, _is_enabled);
        }

        public PureAsyncAction getAlertsAction(IVMPolicy policy, int hoursfromnow)
        {
            return new GetVMPPAlertsAction((VMPP)policy, hoursfromnow);
        }

        public policy_frequency policy_frequency
        {
            get 
            {
                switch(backup_frequency)
                {
                    case vmpp_backup_frequency.hourly:
                        return policy_frequency.hourly;
                    case vmpp_backup_frequency.daily:
                        return policy_frequency.daily;
                    case vmpp_backup_frequency.weekly:
                        return policy_frequency.weekly;
                    default:
                        return policy_frequency.unknown;
                }
            }

            set 
            {
                switch (value)
                {
                    case policy_frequency.hourly:
                        backup_frequency = vmpp_backup_frequency.hourly;
                        break;
                    case policy_frequency.daily:
                        backup_frequency = vmpp_backup_frequency.daily;
                        break;
                    case policy_frequency.weekly:
                        backup_frequency = vmpp_backup_frequency.weekly;
                        break;
                    default:
                        backup_frequency = vmpp_backup_frequency.unknown;
                        break;
                }
            }
        }

        public Dictionary<string, string> policy_schedule 
        {
            get { return backup_schedule; }
            set { backup_schedule = value; }
        }

        public long policy_retention 
        {
            get { return backup_retention_value; }
            set { backup_retention_value = value; }
        }

        public policy_backup_type policy_type 
        {
            get
            {
                switch(backup_type)
                {
                    case vmpp_backup_type.checkpoint:
                        return policy_backup_type.checkpoint;
                    case vmpp_backup_type.snapshot:
                        return policy_backup_type.snapshot;
                    default:
                        return policy_backup_type.unknown;
                }
            }

            set 
            {
                switch (value)
                {
                    case policy_backup_type.checkpoint:
                        backup_type = vmpp_backup_type.checkpoint;
                        break;
                    case policy_backup_type.snapshot:
                        backup_type = vmpp_backup_type.snapshot;
                        break;
                    default:
                        backup_type = vmpp_backup_type.unknown;
                        break;
                }
            }
        }

        public XenRef<Task> async_task_create(Session session)
        {
            return VMPP.async_create(session, (VMPP)this);
        }

        public void set_policy(Session session, string _vm, string _value)
        {
            VM.set_protection_policy(session, _vm, _value);
        }

        public string alarm_config_smtp_server
        {
            get
            {
                return TryGetKey(alarm_config, "smtp_server");
            }
        }

        public string alarm_config_smtp_port
        {
            get
            {
                return TryGetKey(alarm_config, "smtp_port");
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

        public string alarm_config_email_address
        {
            get
            {
                return TryGetKey(alarm_config, "email_address");
            }
        }

        public DateTime GetNextArchiveRunTime()
        {
            var time = Host.get_server_localtime(Connection.Session, Helpers.GetMaster(Connection).opaque_ref);
            if (archive_frequency == vmpp_archive_frequency.daily)
            {
                return GetDailyDate(time, 
                                    Convert.ToInt32(archive_schedule_min), 
                                    Convert.ToInt32(archive_schedule_hour));
            }
            if (archive_frequency == vmpp_archive_frequency.weekly)
            {
                var hour = Convert.ToInt32(archive_schedule_hour);
                var min = Convert.ToInt32(archive_schedule_min);
                return GetWeeklyDate(time, hour, min, new List<DayOfWeek>(DaysOfWeekArchive));
            }
            if (archive_frequency == vmpp_archive_frequency.always_after_backup)
                return GetNextRunTime();
            return DateTime.MinValue;

        }

        public string archive_target_config_location
        {
            get
            {

                return TryGetKey(archive_target_config, "location");
            }
        }

        public string archive_target_config_username
        {
            get
            {
                return TryGetKey(archive_target_config, "username");
            }
        }

        public string archive_target_config_password_uuid
        {
            get
            {

                return TryGetKey(archive_target_config, "password");
            }
        }

        public string archive_target_config_password_value
        {
            get
            {
                string uuid = archive_target_config_password_uuid;
                try
                {
                    string opaqueref = Secret.get_by_uuid(Connection.Session, uuid);
                    return Secret.get_value(Connection.Session, opaqueref);
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }


        public string backup_schedule_min
        {
            get
            {
                return TryGetKey(backup_schedule, "min");
            }
        }

        public string backup_schedule_hour
        {
            get
            {

                return TryGetKey(backup_schedule, "hour");
            }
        }

        public string backup_schedule_days
        {
            get
            {

                return TryGetKey(backup_schedule, "days");
            }
        }

        public string archive_schedule_min
        {
            get
            {
                return TryGetKey(archive_schedule, "min");
            }
        }

        public string archive_schedule_hour
        {
            get
            {

                return TryGetKey(archive_schedule, "hour");
            }
        }

        public string archive_schedule_days
        {
            get
            {

                return TryGetKey(archive_schedule, "days");
            }
        }
        private List<PolicyAlert> _alerts = new List<PolicyAlert>();

        public List<PolicyAlert> Alerts
        {
            get
            {
                foreach (var recent in RecentAlerts)
                {
                    if (!_alerts.Contains(recent))
                        _alerts.Add(recent);
                }
                return _alerts;
            }
            set { _alerts = value; }
        }

        public List<PolicyAlert> RecentAlerts
        {
            get
            {
                List<PolicyAlert> result = new List<PolicyAlert>();
                foreach (var body in recent_alerts)
                {
                    result.Add(new PolicyAlert(Connection, body));
                }
                return result;
            }
        }

        public string LastResult
        {
            get
            {
                if (_recent_alerts.Length > 0)
                {
                    var listRecentAlerts = new List<PolicyAlert>(RecentAlerts);
                    listRecentAlerts.Sort((x, y) => y.Time.CompareTo(x.Time));
                    if (listRecentAlerts[0].Type == "info")
                        return Messages.VM_PROTECTION_POLICY_SUCCEEDED;

                    return Messages.FAILED;
                }
                return Messages.NOT_YET_RUN;
            }
        }
    }
}
