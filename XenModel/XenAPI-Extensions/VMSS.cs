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
    public partial class VMSS : IVMPolicy
    {
        public bool is_enabled
        {
            get { return this.enabled; }
        }

        public bool is_running
        {
            get { return false; }
        }

        public bool is_archiving
        {
            get { return false; }
        }
        public DateTime GetNextArchiveRunTime()
        {
            /*Not supported*/
            return new DateTime();
        }
        public Type _Type
        {
            get { return typeof(VMSS); }
        }
        public List<PolicyAlert> PolicyAlerts
        {
            get { return Alerts; }
        }

        public bool hasArchive
        {
            get { return false; }
        }

        public void set_vm_policy(Session session, string _vm, string _value)
        {
            VM.set_snapshot_schedule(session, _vm, _value);
        }

        public void do_destroy(Session session, string _policy)
        {
            VMSS.destroy(session, _policy);
        }

        public string run_now(Session session, string _policy)
        {
            return VMSS.snapshot_now(session, _policy);
        }

        public void set_is_enabled(Session session, string _policy, bool _is_enabled)
        {
            VMSS.set_enabled(session, _policy, _is_enabled);
        }

        public PureAsyncAction getAlertsAction(IVMPolicy policy, int hoursfromnow)
        {
            return new GetVMSSAlertsAction((VMSS)policy, hoursfromnow);
        }

        public policy_frequency policy_frequency 
        {
            get 
            {
                switch(frequency)
                {
                    case vmss_frequency.hourly:
                        return policy_frequency.hourly;
                    case vmss_frequency.daily:
                        return policy_frequency.daily;
                    case vmss_frequency.weekly:
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
                        frequency = vmss_frequency.hourly;
                        break;
                    case policy_frequency.daily:
                        frequency = vmss_frequency.daily;
                        break;
                    case policy_frequency.weekly:
                        frequency = vmss_frequency.weekly;
                        break;
                    default:
                        frequency = vmss_frequency.unknown;
                        break;
                }
            }
        }

        public Dictionary<string, string> policy_schedule
        {
            get { return schedule; }
            set { schedule = value; }
        }

        public long policy_retention
        {
            get { return retained_snapshots; }
            set { retained_snapshots = value; }
        }

        public policy_backup_type policy_type
        {
            get
            {
                switch(type)
                {
                    case vmss_type.checkpoint:
                        return policy_backup_type.checkpoint;
                    case vmss_type.snapshot:
                        return policy_backup_type.snapshot;
                    case vmss_type.snapshot_with_quiesce:
                        return policy_backup_type.snapshot_with_quiesce;
                    default:
                        return policy_backup_type.unknown;
                }
            }

            set 
            {
                switch (value)
                {
                    case policy_backup_type.checkpoint:
                        type = vmss_type.checkpoint;
                        break;
                    case policy_backup_type.snapshot:
                        type = vmss_type.snapshot;
                        break;
                    case policy_backup_type.snapshot_with_quiesce:
                        type = vmss_type.snapshot_with_quiesce;
                        break;
                    default:
                        type = vmss_type.unknown;
                        break;
                }
            }
        }

        public XenRef<Task> async_task_create(Session session)
        {
            return VMSS.async_create(session, (VMSS)this);
        }

        public void set_policy(Session session, string _vm, string _value)
        {
            VM.set_snapshot_schedule(session, _vm, _value);
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

        private static DateTime GetDailyDate(DateTime time, int min, int hour)
        {
            return VMPP.GetDailyDate(time, min, hour);
        }

        private static DateTime GetHourlyDate(DateTime time, int min)
        {
            return VMPP.GetHourlyDate(time, min);
        }

        public static DateTime GetWeeklyDate(DateTime time, int hour, int min, List<DayOfWeek> listDaysOfWeek)
        {
            return VMPP.GetWeeklyDate(time, hour, min, listDaysOfWeek);
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
                return VMPP.TryGetKey(schedule, "min");
            }
        }

        public string backup_schedule_hour
        {
            get
            {

                return VMPP.TryGetKey(schedule, "hour");
            }
        }

        public string backup_schedule_days
        {
            get
            {

                return VMPP.TryGetKey(schedule, "days");
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
                        return Messages.VM_PROTECTION_POLICY_SUCCEEDED;

                    return Messages.FAILED;
                }
                return Messages.NOT_YET_RUN;
            }
        }
    }
}
