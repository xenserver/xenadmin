﻿/* Copyright (c) Citrix Systems, Inc. 
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
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Controls
{
    /// <summary>
    /// Decide which of the SRPickerItem to choose - depending on the usage required
    /// </summary>
    public class SrPickerItemFactory
    {
        public SrPickerItem PickerItem(SR sr, SrPicker.SRPickerType usage, Host aff, long diskSize, VDI[] vdis)
        {
            if (SrPicker.SRPickerType.Migrate == usage)
                return new SrPickerMigrateItem(sr, aff, diskSize, vdis);
            if (SrPicker.SRPickerType.MoveOrCopy == usage)
                return new SrPickerMoveCopyItem(sr, aff, diskSize, vdis);
            if (SrPicker.SRPickerType.InstallFromTemplate == usage)
                return new SrPickerInstallFromTemplateItem(sr,  aff, diskSize, vdis);
            if (SrPicker.SRPickerType.VM == usage)
                return new SrPickerVmItem(sr, aff, diskSize, vdis);
            if (SrPicker.SRPickerType.LunPerVDI == usage)
                return new SrPickerLunPerVDIItem(sr, aff, diskSize, vdis);

            throw new ArgumentException("There is no SRPickerItem for the type: " + usage);
        }
    }

    public class SrPickerLunPerVDIItem : SrPickerVmItem
    {
        public SrPickerLunPerVDIItem(SR sr, Host aff, long diskSize, VDI[] vdis)
            : base(sr, aff, diskSize, vdis)
        {
        }

        protected override bool CanBeEnabled
        {
            get
            {
                if(TheSR.HBALunPerVDI())
                    return !TheSR.IsBroken(false) && TheSR.CanBeSeenFrom(Affinity);
                return base.CanBeEnabled;
            }
        }

        protected override bool UnsupportedSR => false;
    }

    /// <summary>
    /// Stategy pattern for the SRPickerItem for migrate scenarios
    /// </summary>
    public class SrPickerMigrateItem : SrPickerItem
    {
        public SrPickerMigrateItem(SR sr, Host aff, long diskSize, VDI[] vdis) : base(sr, aff, diskSize, vdis)
        {
        }

        /// <summary>
        /// We can move VDIs to a local SR only if the VDI is attached to VMs
        /// that have a home server that can see the SR
        /// </summary>
        private static bool HomeHostCanSeeTargetSr(VDI vdi, SR targetSr)
        {
            var vms = vdi.GetVMs();
            var homeHosts = (from VM vm in vms
                let host = vm.Home()
                where host != null
                select host).ToList();

            return homeHosts.Count > 0 && homeHosts.All(targetSr.CanBeSeenFrom);
        }

        protected override string CannotBeShownReason
        {
            get
            {
                if (ExistingVDILocation())
                    return Messages.CURRENT_LOCATION;

                if (TheSR.IsLocalSR())
                {
                    foreach (var vdi in existingVDIs)
                    {
                        var homeHosts = new List<Host>();
                        var vms = vdi.GetVMs();
                        foreach (var vm in vms)
                        {
                            var homeHost = vm.Home();
                            if (homeHost != null)
                            {
                                homeHosts.Add(homeHost);

                                if (!TheSR.CanBeSeenFrom(homeHost))
                                    return vm.power_state == vm_power_state.Running
                                        ? Messages.SRPICKER_ERROR_LOCAL_SR_MUST_BE_RESIDENT_HOSTS
                                        : string.Format(Messages.SR_CANNOT_BE_SEEN, Helpers.GetName(homeHost));
                            }
                        }

                        if (homeHosts.Count == 0)
                            return Messages.SR_IS_LOCAL;
                    }
                }

                if (!TheSR.CanBeSeenFrom(Affinity))
                    return TheSR.Connection != null
                        ? string.Format(Messages.SR_CANNOT_BE_SEEN, Affinity == null ? Helpers.GetName(TheSR.Connection) : Helpers.GetName(Affinity))
                        : Messages.SR_DETACHED;

                return base.CannotBeShownReason;
            }
        }

        protected override bool CanBeEnabled
        {
            get
            {
                return existingVDIs.Length > 0 &&
                       !ExistingVDILocation() &&
                       (!TheSR.IsLocalSR() || existingVDIs.All(v => HomeHostCanSeeTargetSr(v, TheSR))) &&
                       TheSR.SupportsVdiCreate() &&
                       !TheSR.IsDetached() && TheSR.VdiCreationCanProceed(DiskSize);
            }
        }
    }

    /// <summary>
    /// Stategy pattern for the SRPickerItem for copy and move scenarios
    /// </summary>
    public class SrPickerMoveCopyItem : SrPickerItem
    {
        public SrPickerMoveCopyItem(SR sr, Host aff, long diskSize, VDI[] vdis) : base(sr, aff, diskSize, vdis)
        {
        }

        protected override bool CanBeEnabled
        {
            get
            {
                return !TheSR.IsDetached() && TheSR.SupportsVdiCreate() && !ExistingVDILocation() &&
                       TheSR.VdiCreationCanProceed(DiskSize);
            }
        }

        protected override string  CannotBeShownReason
        {   
	        get 
	        {
                if (TheSR.IsDetached())
                    return Messages.SR_DETACHED;
                if (ExistingVDILocation())
                    return Messages.CURRENT_LOCATION;
	            return base.CannotBeShownReason;
	        }
        }
                        
    }

    /// <summary>
    /// Stategy pattern for the SRPickerItem for installing from a template scenarios
    /// </summary>
    public class SrPickerInstallFromTemplateItem : SrPickerItem
    {
        public SrPickerInstallFromTemplateItem(SR sr, Host aff, long diskSize, VDI[] vdis) : base(sr, aff, diskSize, vdis)
        {
        }

        protected override bool CanBeEnabled
        {
            get { return TheSR.SupportsVdiCreate() && !TheSR.IsDetached() && TheSR.VdiCreationCanProceed(DiskSize); }
        }

        protected override string CannotBeShownReason
        {
            get
            {
                if (TheSR.IsDetached())
                    return Messages.SR_DETACHED;
                return base.CannotBeShownReason;
            }
        }
    }


    /// <summary>
    /// Stategy pattern for the SRPickerItem for VM scenarios
    /// </summary>
    public class SrPickerVmItem : SrPickerItem
    {
        public SrPickerVmItem(SR sr, Host aff, long diskSize, VDI[] vdis) : base(sr, aff, diskSize, vdis)
        {
        }

        protected override bool CanBeEnabled
        {
            get { return TheSR.CanBeSeenFrom(Affinity) && TheSR.CanCreateVmOn() && TheSR.VdiCreationCanProceed(DiskSize); }
        }

        protected override string CannotBeShownReason
        {
            get
            {
                if (Affinity == null && !TheSR.shared)
                    return Messages.SR_IS_LOCAL;
                if (!TheSR.CanBeSeenFrom(Affinity))
                    return TheSR.Connection != null
                        ? string.Format(Messages.SR_CANNOT_BE_SEEN, Affinity == null ? Helpers.GetName(TheSR.Connection) : Helpers.GetName(Affinity))
                        : Messages.SR_DETACHED;
                return base.CannotBeShownReason;
            }
        }
    }

    /// <summary>
    /// Base class for the SRPickerItem stategies
    /// </summary>
    public abstract class SrPickerItem : CustomTreeNode, IComparable<SrPickerItem>
    {

        public SR TheSR { get; }
        public bool Show { get; private set; }
        protected readonly Host Affinity;
        protected long DiskSize { get; private set; }
        protected readonly VDI[] existingVDIs;

        protected SrPickerItem(SR sr, Host aff, long diskSize, VDI[] vdis)
        {
            Show = true;
            existingVDIs = vdis ?? new VDI[0];
            TheSR = sr;
            Affinity = aff;
            DiskSize = diskSize;
            Update();
        }

        private bool ShowHiddenVDIs
        {
            get
            {
                return TheSR.ShowInVDISRList(Properties.Settings.Default.ShowHiddenVMs);
            }
        }

        protected virtual bool UnsupportedSR
        {
            get { return TheSR.HBALunPerVDI(); }
        }

        protected abstract bool CanBeEnabled { get; }

        protected virtual void SetImage()
        {
            Image = Images.GetImage16For(TheSR);
        }

        public void UpdateDiskSize(long diskSize)
        {
            DiskSize = diskSize;
            Update();
        }

        public void Update()
        {
            Text = TheSR.Name();
            SetImage();

            if(UnsupportedSR)
            {
                DoNotShowSR();
            }
            else if (ShowHiddenVDIs && CanBeEnabled && !ExistingVDILocation())
            {
                ShowSREnabled();
            }
            else if (TheSR.PBDs.Count > 0 && TheSR.SupportsVdiCreate())
            {
                ShowSRDisabled();
            }
            else
            {
                DoNotShowSR();
            }
        }

        private void DoNotShowSR()
        {
            Show = false;
        }

        private void ShowSRDisabled()
        {
            Description = CannotBeShownReason;
            CalculateDisabledSortReason();
            Enabled = false;
            Show = true;
        }

        private void ShowSREnabled()
        {
            Enabled = true;
            Description = string.Format(Messages.SRPICKER_DISK_FREE, Util.DiskSizeString(TheSR.FreeSpace(), 2),
                                        Util.DiskSizeString(TheSR.physical_size, 2));
            CalculateEnabledSortReason();
            Show = true;
        }

        protected bool ExistingVDILocation()
        {
            return existingVDIs.Length > 0 && existingVDIs.All(vdi => vdi.SR.opaque_ref == TheSR.opaque_ref);
        }

        protected virtual string CannotBeShownReason
        {
            get
            {
                if (TheSR.IsBroken(false))
                    return Messages.SR_IS_BROKEN;
                if (TheSR.IsFull())
                    return Messages.SRPICKER_SR_FULL;
                if (DiskSize > TheSR.physical_size)
                    return string.Format(Messages.SR_PICKER_DISK_TOO_BIG, Util.DiskSizeString(DiskSize, 2),
                                         Util.DiskSizeString(TheSR.physical_size, 2));
                if (DiskSize > (TheSR.FreeSpace()))
                    return string.Format(Messages.SR_PICKER_INSUFFICIENT_SPACE, Util.DiskSizeString(DiskSize, 2),
                                         Util.DiskSizeString(TheSR.FreeSpace(), 2));
                if (DiskSize > SR.DISK_MAX_SIZE)
                    return string.Format(Messages.SR_PICKER_DISKSIZE_EXCEEDS_DISK_MAX_SIZE,
                        Util.DiskSizeString(SR.DISK_MAX_SIZE, 0));
                return "";
            }

        }

        public int CompareTo(SrPickerItem other)
        {
            return base.CompareTo(other);
        }

        #region Sorting Details
        private enum SrNotEnabledReason { None, Broken, Full, NotSeen };
        private SrNotEnabledReason sortingReason;

        private void CalculateDisabledSortReason()
        {
            if (!TheSR.CanBeSeenFrom(Affinity))
                sortingReason = SrNotEnabledReason.NotSeen;
            else if (TheSR.IsFull() || TheSR.FreeSpace() < DiskSize)
                sortingReason = SrNotEnabledReason.Full;
            else
                sortingReason = SrNotEnabledReason.Broken;
        }
        
        private void CalculateEnabledSortReason()
        {
            sortingReason = SrNotEnabledReason.None;
        }

        protected override int SameLevelSortOrder(CustomTreeNode other)
        {
            SrPickerItem otherItem = other as SrPickerItem;
            if (otherItem == null) //shouldnt ever happen!!!
                return -1;

            if (!otherItem.Enabled && Enabled)
                return -1;
            if (otherItem.Enabled && !Enabled)
                return 1;
            if (otherItem.Enabled && Enabled)
                return base.SameLevelSortOrder(otherItem);
            return (int)sortingReason - (int)otherItem.sortingReason;
        }
        #endregion
    }
}
