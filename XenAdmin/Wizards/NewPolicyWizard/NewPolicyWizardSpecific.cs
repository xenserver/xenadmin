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
using System.Globalization;
using System.Text;
using XenAdmin.Actions;
using XenAdmin.Controls;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Wizards.GenericPages;

namespace XenAdmin.Wizards.NewPolicyWizard
{
    //See notes in base class

    public class NewPolicyWizardSpecific<T> : NewPolicyWizard where T : XenObject<T>
    {
        private readonly NewVMGroupVMsPage<T> xenTabPageVMsPage;
        private readonly NewPolicySnapshotTypePageSpecific<T> xenTabPageSnapshotType;

        public NewPolicyWizardSpecific(Pool pool)
            :base(pool)
        {

            xenTabPagePolicy = typeof(T) == typeof(VMPP) ? new NewPolicyPolicyNamePage(Messages.NEW_VMPP_PAGE_TEXT) : new NewPolicyPolicyNamePage(Messages.NEW_VMSS_PAGE_TEXT);
            xenTabPageSnapshotType = new NewPolicySnapshotTypePageSpecific<T>();
            xenTabPageVMsPage = new NewVMGroupVMsPage<T>();
            xenTabPageFinish = typeof(T) == typeof(VMPP) ? new NewPolicyFinishPage(Messages.VMPP_FINISH_PAGE_TEXT, Messages.VMPP_FINISH_PAGE_CHECKBOX_TEXT)
                : new NewPolicyFinishPage(Messages.VMSS_FINISH_PAGE_TEXT, Messages.VMSS_FINISH_PAGE_CHECKBOX_TEXT);
            xenTabPageRBAC = new RBACWarningPage();
            xenTabPageVMsPage.Pool = pool;
            
            #region RBAC Warning Page Checks
            if (Pool.Connection.Session.IsLocalSuperuser || Helpers.GetMaster(Pool.Connection).external_auth_type == Auth.AUTH_TYPE_NONE)
            {
                //do nothing
            }
            else
            {
                RBACWarningPage.WizardPermissionCheck check;
                if (typeof(T) == typeof(VMPP))
                {
                    check = new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_VMPP);
                    check.AddApiCheck("VMPP.async_create");
                }
                else
                {
                    check = new RBACWarningPage.WizardPermissionCheck(Messages.RBAC_WARNING_VMSS);
                    check.AddApiCheck("VMSS.async_create");
                }
                check.Blocking = true;
                xenTabPageRBAC.AddPermissionChecks(xenConnection, check);
                AddPage(xenTabPageRBAC, 0);
            }
            #endregion

            AddPages(xenTabPagePolicy, xenTabPageVMsPage);
            AddPage(xenTabPageSnapshotType);
                       
            if (typeof(T) == typeof(VMPP))
            {
                xenTabPageSnapshotFrequency = new NewPolicySnapshotFrequencyPage(false);
                xenTabPageSnapshotFrequency.Pool = pool;
                AddPages(xenTabPageSnapshotFrequency);

                xenTabPageArchive = new NewPolicyArchivePage();
                xenTabPageArchive.Pool = pool;
                AddPage(xenTabPageArchive);

                xenTabPageEmail = new NewPolicyEmailPage();
                xenTabPageEmail.Pool = pool;
                AddPages(xenTabPageEmail);

                this.Text = Messages.VMPP_WIZARD_TITLE;
            }
            else /*VMSS*/
            {
                xenTabPageSnapshotFrequency = new NewPolicySnapshotFrequencyPage(true);
                xenTabPageSnapshotFrequency.Pool = pool;
                AddPages(xenTabPageSnapshotFrequency);

                this.Text = Messages.VMSS_WIZARD_TITLE;
            }
            AddPages(xenTabPageFinish);
        }

        public NewPolicyWizardSpecific(Pool pool, List<VM> selection)
            : this(pool)
        {
            this.xenTabPageVMsPage.SelectedVMs = selection;
        }

        private new string GetSummary()
        {

            if (typeof(T) == typeof(VMPP))
            {
                return string.Format(Messages.POLICY_SUMMARY.Replace("\\n", "\n").Replace("\\r", "\r"), xenTabPagePolicy.PolicyName, CommaSeparated(xenTabPageVMsPage.SelectedVMs),
                                     FormatBackupType(xenTabPageSnapshotType.BackupType),
                                     FormatSchedule(xenTabPageSnapshotFrequency.Schedule, xenTabPageSnapshotFrequency.Frequency, DaysWeekCheckboxes.DaysMode.L10N_LONG),
                                     FormatSchedule(xenTabPageArchive.Schedule, xenTabPageArchive.ArchiveFrequency, DaysWeekCheckboxes.DaysMode.L10N_LONG));
            }
            else
            {
                return string.Format(Messages.VMSS_POLICY_SUMMARY.Replace("\\n", "\n").Replace("\\r", "\r"), xenTabPagePolicy.PolicyName, CommaSeparated(xenTabPageVMsPage.SelectedVMs),
                                     FormatBackupType(xenTabPageSnapshotType.BackupType),
                                     FormatSchedule(xenTabPageSnapshotFrequency.Schedule, xenTabPageSnapshotFrequency.Frequency, DaysWeekCheckboxes.DaysMode.L10N_LONG));
            }
        }

        private int GetBackupDaysCount()
        {
            string days;
            return xenTabPageSnapshotFrequency.Schedule.TryGetValue("days", out days) ? days.Split(',').Length : 0;
        }

        private static string FormatBackupType(vmpp_backup_type backupType)
        {
            if (backupType == vmpp_backup_type.snapshot)
                return Messages.DISKS_ONLY;
            else if (backupType == vmpp_backup_type.checkpoint)
                return Messages.DISKS_AND_MEMORY;
            throw new ArgumentException("wrong argument");
        }

        private static string FormatBackupTypeVMSS(vmss_schedule_snapshot_type backupType)
        {
            if (backupType == vmss_schedule_snapshot_type.snapshot)
                return Messages.DISKS_ONLY;
            else if (backupType == vmss_schedule_snapshot_type.checkpoint)
                return Messages.DISKS_AND_MEMORY;
            else if (backupType == vmss_schedule_snapshot_type.snapshot_with_quiesce)
                return Messages.QUIESCED_SNAPSHOTS;

            throw new ArgumentException("wrong argument");
        }





        // These two instances of FormatSchedule used to be in the VMPP class. That's probably where
        // they really belong, but because of the way they're constructed (see DaysWeekCheckboxes.L10NDays())
        // they had to move into the View. (CA-51612).
        internal static string FormatSchedule(Dictionary<string, string> schedule, vmpp_archive_frequency archiveType, DaysWeekCheckboxes.DaysMode mode)
        {
            if (archiveType == vmpp_archive_frequency.always_after_backup)
            {
                return Messages.ASAPSNAPSHOTTAKEN;
            }
            else if (archiveType == vmpp_archive_frequency.never)
            {
                return Messages.NEVER;
            }
            else if (archiveType == vmpp_archive_frequency.daily)
            {
                DateTime value = DateTime.Parse(string.Format("{0}:{1}", schedule["hour"], schedule["min"]), CultureInfo.InvariantCulture);
                return string.Format(Messages.DAILY_SCHEDULE_FORMAT, HelpersGUI.DateTimeToString(value, Messages.DATEFORMAT_HM, true));
            }
            else if (archiveType == vmpp_archive_frequency.weekly)
            {
                DateTime value = DateTime.Parse(string.Format("{0}:{1}", schedule["hour"], schedule["min"]), CultureInfo.InvariantCulture);
                return string.Format(Messages.WEEKLY_SCHEDULE_FORMAT, HelpersGUI.DateTimeToString(value, Messages.DATEFORMAT_HM, true), DaysWeekCheckboxes.L10NDays(schedule["days"], mode));
            }
            return "";

        }

        internal static string FormatSchedule(Dictionary<string, string> schedule, vmpp_backup_frequency backupFrequency, DaysWeekCheckboxes.DaysMode mode)
        {
            if (backupFrequency == vmpp_backup_frequency.hourly)
            {
                return string.Format(Messages.HOURLY_SCHEDULE_FORMAT, schedule["min"]);
            }
            else if (backupFrequency == vmpp_backup_frequency.daily)
            {
                DateTime value = DateTime.Parse(string.Format("{0}:{1}", schedule["hour"], schedule["min"]), CultureInfo.InvariantCulture);
                return string.Format(Messages.DAILY_SCHEDULE_FORMAT, HelpersGUI.DateTimeToString(value, Messages.DATEFORMAT_HM, true));
            }
            else if (backupFrequency == vmpp_backup_frequency.weekly)
            {
                DateTime value = DateTime.Parse(string.Format("{0}:{1}", schedule["hour"], schedule["min"]), CultureInfo.InvariantCulture);
                return string.Format(Messages.WEEKLY_SCHEDULE_FORMAT, HelpersGUI.DateTimeToString(value, Messages.DATEFORMAT_HM, true), DaysWeekCheckboxes.L10NDays(schedule["days"], mode));
            }
            return "";
        }

        private static string CommaSeparated(IEnumerable<VM> selectedVMs)
        {
            var sb = new StringBuilder();
            foreach (var selectedVM in selectedVMs)
            {
                sb.Append(selectedVM.Name);
                sb.Append(", ");
            }
            if (sb.Length > 2)
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        protected override void UpdateWizardContent(XenTabPage senderPage)
        {
            var prevPageType = senderPage.GetType();

            if (prevPageType == typeof(NewPolicyPolicyNamePage))
            {
                xenTabPageVMsPage.GroupName = xenTabPagePolicy.PolicyName;
            }
            else if (typeof(T) == typeof(VMPP))
            {
                if (prevPageType == typeof(NewPolicySnapshotFrequencyPage))
                {
                    xenTabPageArchive.SnapshotFrequency = xenTabPageSnapshotFrequency.Frequency;
                    xenTabPageArchive.BackupDaysCount = GetBackupDaysCount();
                }
                else if (prevPageType == typeof(NewPolicyEmailPage))
                {
                    xenTabPageFinish.Summary = GetSummary();
                    xenTabPageFinish.SelectedVMsCount = xenTabPageVMsPage.SelectedVMs.Count;
                }
            }
            else if (typeof(T) == typeof(VMSS))
            {
                if (prevPageType == typeof(NewPolicySnapshotFrequencyPage))
                {
                    xenTabPageFinish.Summary = GetSummary();
                    xenTabPageFinish.SelectedVMsCount = xenTabPageVMsPage.SelectedVMs.Count;
                }
                else if (prevPageType == typeof(NewVMGroupVMsPage<VMSS>))
                {
                    xenTabPageSnapshotType.SelectedVMs = xenTabPageVMsPage.SelectedVMs;
                }
            }

        }

        protected override void FinishWizard()
        {
            if (typeof(T) == typeof(VMPP))
            {
                var vmpp = new VMPP
                {
                    name_label = xenTabPagePolicy.PolicyName,
                    name_description = xenTabPagePolicy.PolicyDescription,
                    backup_type = xenTabPageSnapshotType.BackupType,
                    backup_frequency = xenTabPageSnapshotFrequency.Frequency,
                    backup_schedule = xenTabPageSnapshotFrequency.Schedule,
                    backup_retention_value = xenTabPageSnapshotFrequency.BackupRetention,
                    archive_frequency = xenTabPageArchive.ArchiveFrequency,
                    archive_target_config = xenTabPageArchive.ArchiveConfig,
                    archive_target_type = xenTabPageArchive.ArchiveTargetType,
                    archive_schedule = xenTabPageArchive.Schedule,
                    is_alarm_enabled = xenTabPageEmail.EmailEnabled,
                    alarm_config = xenTabPageEmail.EmailSettings,
                    is_policy_enabled = xenTabPageVMsPage.SelectedVMs.Count == 0 ? false : true,
                    Connection = Pool.Connection
                };

                var action = new CreateVMPP(vmpp, xenTabPageVMsPage.SelectedVMs, xenTabPageFinish.RunNow);
                action.RunAsync();
            }
            else
            {
                var vmpp = new VMSS
                {
                    name_label = xenTabPagePolicy.PolicyName,
                    name_description = xenTabPagePolicy.PolicyDescription,
                    schedule_snapshot_type = xenTabPageSnapshotType.BackupTypeVMSS,
                    schedule_snapshot_frequency = (vmss_schedule_snapshot_frequency)xenTabPageSnapshotFrequency.Frequency,
                    snapshot_schedule = xenTabPageSnapshotFrequency.Schedule,
                    schedule_snapshot_retention_value = xenTabPageSnapshotFrequency.BackupRetention,
                    is_schedule_snapshot_enabled = xenTabPageVMsPage.SelectedVMs.Count == 0 ? false : true,
                    Connection = Pool.Connection
                };

                var action = new CreateVMSS(vmpp, xenTabPageVMsPage.SelectedVMs, xenTabPageFinish.RunNow);
                action.RunAsync();
            }
            base.FinishWizard();
        }

        protected override string WizardPaneHelpID()
        {
            return CurrentStepTabPage is RBACWarningPage ? FormatHelpId("Rbac") : base.WizardPaneHelpID();
        }

    }
}