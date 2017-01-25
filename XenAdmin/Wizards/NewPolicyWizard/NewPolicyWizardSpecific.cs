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
            this.Text = VMGroup<T>.VMPolicyWizardTitle;

            xenTabPagePolicy = new NewPolicyPolicyNamePage(VMGroup<T>.VMPolicyNamePageText, VMGroup<T>.VMPolicyNamePageTextMore, 
                                                            VMGroup<T>.VMPolicyNamePageTabName, VMGroup<T>.VMPolicyNamePageTabText, VMGroup<T>.VMPolicyNamePageNameFieldText);
            xenTabPageSnapshotType = new NewPolicySnapshotTypePageSpecific<T>();
            xenTabPageVMsPage = new NewVMGroupVMsPage<T>();
            xenTabPageFinish = new NewPolicyFinishPage(VMGroup<T>.VMPolicyFinishPageText, VMGroup<T>.VMPolicyFinishPageCheckboxText, VMGroup<T>.VMPolicyFinishPageTitle);
            xenTabPageRBAC = new RBACWarningPage();
            xenTabPageVMsPage.Pool = pool;
            xenTabPageSnapshotFrequency = new NewPolicySnapshotFrequencyPage();
            xenTabPageSnapshotFrequency.Pool = pool;
            
            #region RBAC Warning Page Checks
            if (Pool.Connection.Session.IsLocalSuperuser || Helpers.GetMaster(Pool.Connection).external_auth_type == Auth.AUTH_TYPE_NONE)
            {
                //do nothing
            }
            else
            {
                RBACWarningPage.WizardPermissionCheck check;
                check = new RBACWarningPage.WizardPermissionCheck(VMGroup<T>.VMPolicyRBACWarning);
                check.AddApiCheck(VMGroup<T>.VMPolicyRBACapiCheck);
                check.Blocking = true;
                xenTabPageRBAC.AddPermissionChecks(xenConnection, check);
                AddPage(xenTabPageRBAC, 0);
            }
            #endregion

            AddPages(xenTabPagePolicy, xenTabPageVMsPage);
            AddPage(xenTabPageSnapshotType);
            AddPages(xenTabPageSnapshotFrequency);
                       
            if (VMGroup<T>.isVMPolicyVMPP)
            {
                xenTabPageArchive = new NewPolicyArchivePage();
                xenTabPageArchive.Pool = pool;
                AddPage(xenTabPageArchive);

                xenTabPageEmail = new NewPolicyEmailPage();
                xenTabPageEmail.Pool = pool;
                AddPages(xenTabPageEmail);

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

            if (VMGroup<T>.isVMPolicyVMPP)
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

        private static string FormatBackupType(policy_backup_type backupType)
        {
            if (backupType == policy_backup_type.snapshot)
                return Messages.DISKS_ONLY;
            else if (backupType == policy_backup_type.checkpoint)
                return Messages.DISKS_AND_MEMORY;
            else if (backupType == policy_backup_type.snapshot_with_quiesce)
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

        internal static string FormatSchedule(Dictionary<string, string> schedule, policy_frequency backupFrequency, DaysWeekCheckboxes.DaysMode mode)
        {
            if (backupFrequency == policy_frequency.hourly)
            {
                return string.Format(Messages.HOURLY_SCHEDULE_FORMAT, schedule["min"]);
            }
            else if (backupFrequency == policy_frequency.daily)
            {
                DateTime value = DateTime.Parse(string.Format("{0}:{1}", schedule["hour"], schedule["min"]), CultureInfo.InvariantCulture);
                return string.Format(Messages.DAILY_SCHEDULE_FORMAT, HelpersGUI.DateTimeToString(value, Messages.DATEFORMAT_HM, true));
            }
            else if (backupFrequency == policy_frequency.weekly)
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
            else if (VMGroup<T>.isVMPolicyVMPP)
            {
                if (prevPageType == typeof(NewPolicySnapshotFrequencyPage))
                {
                    xenTabPageArchive.SnapshotFrequency = (vmpp_backup_frequency)xenTabPageSnapshotFrequency.Frequency;
                    xenTabPageArchive.BackupDaysCount = GetBackupDaysCount();
                }
                else if (prevPageType == typeof(NewPolicyEmailPage))
                {
                    xenTabPageFinish.Summary = GetSummary();
                    xenTabPageFinish.SelectedVMsCount = xenTabPageVMsPage.SelectedVMs.Count;
                }
            }
            else
            {
                if (prevPageType == typeof(NewPolicySnapshotFrequencyPage))
                {
                    xenTabPageFinish.Summary = GetSummary();
                    xenTabPageFinish.SelectedVMsCount = xenTabPageVMsPage.SelectedVMs.Count;
                }
                else if (prevPageType == typeof(NewVMGroupVMsPage<T>))
                {
                    xenTabPageSnapshotType.SelectedVMs = xenTabPageVMsPage.SelectedVMs;
                }
            }

        }

        protected override void FinishWizard()
        {
            var action = VMGroup<T>.VMCreateObjectAction(
                xenTabPagePolicy.PolicyName,
                xenTabPagePolicy.PolicyDescription,
                xenTabPageSnapshotType.BackupType,
                xenTabPageSnapshotFrequency.Frequency,
                xenTabPageSnapshotFrequency.Schedule,
                xenTabPageSnapshotFrequency.BackupRetention,
                xenTabPageArchive != null ? xenTabPageArchive.ArchiveFrequency : vmpp_archive_frequency.unknown,
                xenTabPageArchive != null ? xenTabPageArchive.ArchiveConfig : null,
                xenTabPageArchive != null ? xenTabPageArchive.ArchiveTargetType : vmpp_archive_target_type.unknown,
                xenTabPageArchive != null ? xenTabPageArchive.Schedule : null,
                xenTabPageEmail != null ? xenTabPageEmail.EmailEnabled : false,
                xenTabPageEmail != null ? xenTabPageEmail.EmailSettings : null,
                xenTabPageVMsPage.SelectedVMs.Count == 0 ? false : true,
                xenTabPageVMsPage.SelectedVMs,
                xenTabPageFinish.RunNow,
                Pool.Connection);

            action.RunAsync();
            base.FinishWizard();
        }

        protected override string WizardPaneHelpID()
        {
            if (CurrentStepTabPage is RBACWarningPage)
            {
                return FormatHelpId("Rbac");
            }

            else if (!VMGroup<T>.isVMPolicyVMPP)
            {
                return "NewPolicyWizardVMSS_" + CurrentStepTabPage.HelpID + "Pane";    
            }

            return "NewPolicyWizard_" + CurrentStepTabPage.HelpID + "Pane";
        }

    }
}
