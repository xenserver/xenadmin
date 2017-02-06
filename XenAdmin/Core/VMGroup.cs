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
using System.Text;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.VMPolicies;
using XenAdmin.Dialogs.VMAppliances;
using XenAdmin.Network;
using XenAdmin.Wizards;
using XenAdmin.Wizards.NewPolicyWizard;
using XenAdmin.Wizards.NewVMApplianceWizard;

using XenAPI;

namespace XenAdmin.Core
{
    /// <summary>
    /// A helper class for dealing with a group of VMs (currently either a VMPP, VMSS or a vApp);
    /// it contains all the functions necessary to abstract away what type of group it is.
    /// 
    /// In C++, we would use template specialization for this, but C# generics don't have that, so we
    /// end up switching on T. It's ugly, but it's the way that maximises the amount of shared code.
    /// </summary>

    static class VMGroup<T> where T : XenObject<T>
    {
        // This section covers all the functions that depend on the type of group we're talking about.

        internal static T[] GroupsInCache(ICache cache)
        {
            return typeof(T) == typeof(VMPP) ? cache.VMPPs as T[] : (typeof(T) == typeof(VMSS)? cache.VMSSs as T[] : cache.VM_appliances as T[]);
        }

        internal static XenRef<T> VmToGroup(VM vm)
        {
            return typeof(T) == typeof(VMPP) ? vm.protection_policy as XenRef<T> : (typeof(T) == typeof(VMSS) ? vm.snapshot_schedule as XenRef<T> : vm.appliance as XenRef<T>);
        }

        internal static List<XenRef<VM>> GroupToVMs(T group)
        {
            return typeof(T) == typeof(VMPP) ? (group as VMPP).VMs : (typeof(T) == typeof(VMSS) ? (group as VMSS).VMs : (group as VM_appliance).VMs);
        }

        internal static string ChangeOneWarningString
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.CHANGE_VMS_POLICY_WARNING : (typeof(T) == typeof(VMSS) ? Messages.CHANGE_VM_SNAPSHOT_SCHEDULE_WARNING : Messages.CHANGE_VMS_APPLIANCE_WARNING); }
        }

        internal static string ChangeMultipleWarningString
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.CHANGE_VMS_POLICIES_WARNING : (typeof(T) == typeof(VMSS) ? Messages.CHANGE_VMS_SNAPSHOT_SCHEDULE_WARNING : Messages.CHANGE_VMS_APPLIANCES_WARNING); }
        }

        internal static string NewGroupString
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.NEW_POLICY : (typeof(T) == typeof(VMSS) ? Messages.NEW_SCHEDULE : Messages.NEW_VM_APPLIANCE); }
        }

        internal static string ChangeVMsGroupString
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.CHANGE_VMS_POLICY : (typeof(T) == typeof(VMSS) ? Messages.CHANGE_VMSS_POLICY : Messages.CHANGE_VMS_APPLIANCE); }
        }

        internal static string AssignMainMenuString
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.ASSIGN_PROTECTION_POLICY : (typeof(T) == typeof(VMSS) ? Messages.ASSIGN_VMSS_POLICY : Messages.ASSIGN_VM_APPLIANCE); }
        }

        internal static string AssignContextMenuString
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.ASSIGN_PROTECTION_POLICY_CONTEXT_MENU : (typeof(T) == typeof(VMSS) ? Messages.ASSIGN_VMSS_POLICY_CONTEXT_MENU : Messages.ASSIGN_VM_APPLIANCE); }
        }

        internal static string ManageMainMenuString
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.VM_PROTECTION_MAIN_MENU : (typeof(T) == typeof(VMSS) ? Messages.VMSS_MAIN_MENU : Messages.VM_APPLIANCES_MENU); }
        }

        internal static string ManageContextMenuString
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.VM_PROTECTION_CONTEXT_MENU : (typeof(T) == typeof(VMSS) ? Messages.VMSS_CONTEXT_MENU : Messages.VM_APPLIANCES_MENU); }
        }

        internal static string ChooseVMsPage_Text
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.PROTECTED_VMS : (typeof(T) == typeof(VMSS) ? Messages.VMSS_VMS : Messages.NEWVMAPPLIANCE_VMSPAGE_TEXT); }
        }

        internal static string ChooseVMsPage_Title
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.PROTECTED_VMS_TITLE : (typeof(T) == typeof(VMSS) ? Messages.VMSS_VMS_TITLE : Messages.NEWVMAPPLIANCE_VMSPAGE_TITLE); }
        }

        internal static string ChooseVMsPage_Rubric
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.CHOOSE_VMS_VMPP_RUBRIC : (typeof(T) == typeof(VMSS) ? Messages.CHOOSE_VMS_VMSS_RUBRIC : Messages.CHOOSE_VMS_VAPP_RUBRIC); }
        }

        internal static string ChooseVMsPage_CurrentGroup
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.CURRENT_POLICY : (typeof(T) == typeof(VMSS) ? Messages.CURRENT_SCHEDULE : Messages.CURRENT_VAPP); }
        }

        internal static string VMPolicyTypeName
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.VMPP_TYPE : Messages.VMSS_TYPE; }
        }

        internal static string ChooseVMsPage_HelpID
        {
            get { return typeof(T) == typeof(VMPP) ? "VirtualMachines" : "VMs"; }  // these are only different for historical reasons
        }

        internal static string UpsellBlurb
        {
            get
            {
                if (HiddenFeatures.LinkLabelHidden)
                    return typeof(T) == typeof(VMPP) ? Messages.UPSELL_BLURB_VM_PROTECTION : (typeof(T) == typeof(VMSS) ? Messages.UPSELL_BLURB_VMSS : Messages.UPSELL_BLURB_VM_APPLIANCES);
                else
                    return typeof(T) == typeof(VMPP) ? Messages.UPSELL_BLURB_VM_PROTECTION + Messages.UPSELL_BLURB_VM_PROTECTION_MORE : (typeof(T) == typeof(VMSS) ? Messages.UPSELL_BLURB_VMSS + Messages.UPSELL_BLURB_VMSS_MORE : Messages.UPSELL_BLURB_VM_APPLIANCES + Messages.UPSELL_BLURB_VM_APPLIANCES_MORE);
                
            }
        }

        internal static string UpsellLearnMoreUrl
        {
            get { return typeof(T) == typeof(VMPP) ? InvisibleMessages.UPSELL_LEARNMOREURL_VM_PROTECTION : (typeof(T) == typeof(VMSS) ? InvisibleMessages.UPSELL_LEARNMOREURL_VMSS : InvisibleMessages.UPSELL_LEARNMOREURL_VM_APPLIANCES); }
        }

        internal static AsyncAction AssignVMsToGroupAction(T group, List<XenRef<VM>> vms, bool suppressHistory)
        {
            return typeof(T) == typeof(VMPP) ?
                (AsyncAction)(new AssignVMsToPolicyAction<VMPP>(group as VMPP, vms, suppressHistory)) :
                (typeof(T) == typeof(VMSS) ? (AsyncAction)(new AssignVMsToPolicyAction<VMSS>(group as VMSS, vms, suppressHistory)) :
                (AsyncAction)(new AssignVMsToVMApplianceAction(group as VM_appliance, vms, suppressHistory)));
        }

        internal static AsyncAction RemoveVMsFromGroupAction(T group, List<XenRef<VM>> vms)
        {
            return typeof(T) == typeof(VMPP) ?
                (AsyncAction)(new RemoveVMsFromPolicyAction<VMPP>(group as VMPP, vms)) :
                (typeof(T) == typeof(VMSS) ? (AsyncAction)(new RemoveVMsFromPolicyAction<VMSS>(group as VMSS, vms)) :
                (AsyncAction)(new RemoveVMsFromVMApplianceAction(group as VM_appliance, vms)));
        }

        internal static XenWizardBase NewGroupWizard(Pool pool)
        {
            return typeof(T) == typeof(VMPP) ? (XenWizardBase)(new NewPolicyWizardSpecific<VMPP>(pool)) : (typeof(T) == typeof(VMSS) ? (XenWizardBase)(new NewPolicyWizardSpecific<VMSS>(pool)) : (XenWizardBase)(new NewVMApplianceWizard(pool)));
        }

        internal static XenWizardBase NewGroupWizard(Pool pool, List<VM> vms)
        {
            return typeof(T) == typeof(VMPP) ? (XenWizardBase)(new NewPolicyWizardSpecific<VMPP>(pool, vms)) : (typeof(T) == typeof(VMSS) ? (XenWizardBase)(new NewPolicyWizardSpecific<VMSS>(pool, vms)) : (XenWizardBase)(new NewVMApplianceWizard(pool, vms)));
        }

        internal static XenDialogBase ManageGroupsDialog(Pool pool)
        {
            return typeof(T) == typeof(VMPP) ? (XenDialogBase)(new VMPoliciesDialogSpecific<VMPP>(pool)) : (typeof(T) == typeof(VMSS) ? (XenDialogBase)(new VMPoliciesDialogSpecific<VMSS>(pool)) : (XenDialogBase)(new VMAppliancesDialog(pool)));
        }

        internal static bool FeaturePossible(IXenConnection connection)
        {
            if (typeof(T) == typeof(VMPP) && (Helpers.ClearwaterOrGreater(connection)))
                return false;
            //VMSS is enabled Falcon onwards
            if ((typeof(T) == typeof(VMSS)) && !Helpers.FalconOrGreater(connection))
                return false;
            
            return typeof(T) == typeof(VMPP) ?
                Registry.VMPRFeatureEnabled :
                true;
        }

        internal static Predicate<Host> FeatureRestricted
        {
            get { return typeof(T) == typeof(VMPP) ? (Predicate<Host>)Host.RestrictVMProtection : (typeof(T) == typeof(VMSS) ? ((Predicate<Host>)Host.RestrictVMSnapshotSchedule) : (Predicate<Host>)Host.RestrictVMAppliances); }
        }

        internal static string VMPolicyDialogTitle
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.VMPP_DIALOG_TITLE : Messages.VMSS_DIALOG_TITLE; }
        }

        internal static string VMPolicyDialogText
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.VMPP_DIALOG_TEXT : Messages.VMSS_DIALOG_TEXT; }
        }

        internal static string VMPolicyDialogSchedulesInPool
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.VMPP_SCHEDULED_SNAPSHOTS_DEFINED_FOR_POOL : Messages.VMSS_SCHEDULED_SNAPSHOTS_DEFINED_FOR_POOL; }
        }

        internal static string VMPolicyDialogSchedulesInServer
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.VMPP_SCHEDULED_SNAPSHOTS_DEFINED_FOR_SERVER : Messages.VMSS_SCHEDULED_SNAPSHOTS_DEFINED_FOR_SERVER; }
        }

        internal static IVMPolicy[] VMPolicies (ICache cache)
        {
            if (typeof(T) == typeof(VMPP))
                return cache.VMPPs;
            else
                return cache.VMSSs;
        }

        internal static bool isQuescingSupported
        { 
            get { return typeof(T) == typeof(VMPP) ? false : true; } 
        }

        internal static string VMPolicyNamePageText
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.NEW_VMPP_PAGE_TEXT : Messages.NEW_VMSS_PAGE_TEXT; }
        }

        internal static string VMPolicyNamePageTextMore
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.NEW_VMPP_PAGE_TEXT_MORE : Messages.NEW_VMSS_PAGE_TEXT_MORE; }
        }

        internal static string VMPolicyNamePageTabName
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.POLICY_NAME : Messages.VMSS_NAME; }
        }

        internal static string VMPolicyNamePageTabText
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.POLICY_NAME_TITLE : Messages.VMSS_NAME_TITLE; }
        }

        internal static string VMPolicyNamePageNameFieldText
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.POLICY_NAME_FIELD_TEXT : Messages.VMSS_NAME_FIELD_TEXT; }
        }

        internal static string VMPolicyFinishPageText
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.VMPP_FINISH_PAGE_TEXT : Messages.VMSS_FINISH_PAGE_TEXT; }
        }

        internal static string VMPolicyFinishPageTitle
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.VMPP_FINISH_TITLE : Messages.VMSS_FINISH_TITLE; }
        }

        internal static string VMPolicyFinishPageCheckboxText
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.VMPP_FINISH_PAGE_CHECKBOX_TEXT : Messages.VMSS_FINISH_PAGE_CHECKBOX_TEXT; }
        }

        internal static string VMPolicyRBACWarning
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.RBAC_WARNING_VMPP : Messages.RBAC_WARNING_VMSS; }
        }

        internal static string VMPolicyRBACapiCheck
        {
            get { return typeof(T) == typeof(VMPP) ? "VMPP.async_create" : "VMSS.async_create"; }
        }

        internal static string VMPolicyWizardTitle
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.VMPP_WIZARD_TITLE : Messages.VMSS_WIZARD_TITLE; }
        }

        internal static bool isVMPolicyVMPP
        {
            get { return typeof(T) == typeof(VMPP) ? true : false; }
        }

        internal static string VMAssigningPolicy
        {
            get { return typeof(T) == typeof(VMPP) ? Messages.ASSIGNING_PROTECTION_POLICY : Messages.ASSIGNING_VMSS_POLICY; }
        }

        internal static AsyncAction VMCreateObjectAction(
            string _name_label,
            string _name_description,
            policy_backup_type _backup_type,
            policy_frequency _backup_frequency,
            Dictionary<string, string> _backup_schedule,
            long _backup_retention_value,
            vmpp_archive_frequency _archive_frequency,
            Dictionary<string, string> _archive_target_config,
            vmpp_archive_target_type _archive_target_type,
            Dictionary<string, string> _archive_schedule,
            bool _is_alarm_enabled,           
            Dictionary<string, string> _alarm_config,
            bool _is_policy_enabled,
            List<VM> vms, 
            bool runNow,
            IXenConnection _connection)
        {
            if (typeof(T) == typeof(VMPP))
            {
                var vmpp = new VMPP
                {
                    name_label = _name_label,
                    name_description = _name_description,
                    backup_type = (vmpp_backup_type)_backup_type,
                    backup_frequency = (vmpp_backup_frequency)_backup_frequency,
                    backup_schedule = _backup_schedule,
                    backup_retention_value = _backup_retention_value,
                    archive_frequency = _archive_frequency,
                    archive_target_config = _archive_target_config,
                    archive_target_type = _archive_target_type,
                    archive_schedule = _archive_schedule,
                    is_alarm_enabled = _is_alarm_enabled,
                    alarm_config = _alarm_config,
                    is_policy_enabled = _is_policy_enabled,
                    Connection = _connection
                };
                return new CreateVMPolicy<VMPP>(vmpp, vms, runNow);
            }
            else
            {
                var vmss = new VMSS
                {
                    name_label = _name_label,
                    name_description = _name_description,
                    type = (vmss_type)_backup_type,
                    frequency = (vmss_frequency)_backup_frequency,
                    schedule = _backup_schedule,
                    retained_snapshots = _backup_retention_value,
                    enabled = _is_policy_enabled,
                    Connection = _connection
                };

                return new CreateVMPolicy<VMSS>(vmss, vms, runNow);
            }
        }
    }
}
