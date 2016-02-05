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
using System.Text;
using XenAdmin.Actions;
using XenAdmin.Dialogs;
using XenAdmin.Dialogs.VMProtection_Recovery;
using XenAdmin.Dialogs.VMAppliances;
using XenAdmin.Dialogs.VMSSPolicy;
using XenAdmin.Network;
using XenAdmin.Wizards;
using XenAdmin.Wizards.NewPolicyWizard;
using XenAdmin.Wizards.NewVMApplianceWizard;

using XenAPI;

namespace XenAdmin.Core
{
    /// <summary>
    /// A helper class for dealing with a group of VMs (currently either a VMPP or a vApp);
    /// it contains all the functions necessary to abstract away what type of group it is.
    /// 
    /// In C++, we would use template specialization for this, but C# generics don't have that, so we
    /// end up switching on T. It's ugly, but it's the way that maximises the amount of shared code.
    /// </summary>

    internal static class VMGroup<T> where T : XenObject<T>
    {
        // This section covers all the functions that depend on the type of group we're talking about.

        internal static T[] GroupsInCache(ICache cache)
        {
            return typeof(T) == typeof(VMPP) ? cache.VMPPs as T[] : (typeof(T) == typeof(VMSS)? cache.VMSSs as T[] : cache.VM_appliances as T[]);
        }

        internal static XenRef<T> VmToGroup(VM vm)
        {
            return typeof(T) == typeof(VMPP) ? vm.protection_policy as XenRef<T> : (typeof(T) == typeof(VMSS) ? vm.schedule_snapshot as XenRef <T> : vm.appliance as XenRef<T>);
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
            get { return typeof(T) == typeof(VMPP) ? Messages.NEW_POLICY : (typeof(T) == typeof(VMSS) ? Messages.NEW_POLICY : Messages.NEW_VM_APPLIANCE); }
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
            get { return typeof(T) == typeof(VMPP) ? Messages.CURRENT_POLICY : (typeof(T) == typeof(VMSS) ? Messages.CURRENT_POLICY : Messages.CURRENT_VAPP); }
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
            return typeof(T) == typeof(VMPP) ? (XenDialogBase)(new VMProtectionPoliciesDialog(pool)) : (typeof(T) == typeof(VMSS) ? (XenDialogBase)(new VMSSDialog(pool)) : (XenDialogBase)(new VMAppliancesDialog(pool)));
        }

        internal static bool FeaturePossible(IXenConnection connection)
        {
            if (typeof(T) == typeof(VMPP) && (Helpers.ClearwaterOrGreater(connection)))
                return false;
            //VMSS is enabled Dundee onwards
            if ((typeof(T) == typeof(VMSS)) && !Helpers.DundeeOrGreater(connection))
                return false;
            
            return typeof(T) == typeof(VMPP) ?
                Registry.VMPRFeatureEnabled :
                true;
        }

        internal static Predicate<Host> FeatureRestricted
        {
            get { return typeof(T) == typeof(VMPP) ? (Predicate<Host>)Host.RestrictVMProtection : (typeof(T) == typeof(VMSS) ? ((Predicate<Host>)Host.RestrictVMSnapshotSchedule) : (Predicate<Host>)Host.RestrictVMAppliances); }
        }
    }
}
