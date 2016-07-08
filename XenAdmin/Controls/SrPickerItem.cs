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
                if(TheSR.HBALunPerVDI)
                    return !TheSR.IsBroken(false) && TheSR.CanBeSeenFrom(Affinity);
                return base.CanBeEnabled;
            }
        }

        protected override bool UnsupportedSR
        {
            get { return false; }
        }
    }

    /// <summary>
    /// Stategy pattern for the SRPickerItem for migrate scenarios
    /// </summary>
    public class SrPickerMigrateItem : SrPickerItem
    {
        public SrPickerMigrateItem(SR sr, Host aff, long diskSize, VDI[] vdis) : base(sr, aff, diskSize, vdis)
        {
        }

        private bool SrIsLocalToTheHostOnForExsistingVDIs()
        {
            return existingVDIs != null && existingVDIs.All(vdi => SrIsLocalToTheHostOnWhichTheVmIsRunning(TheSR, vdi));
        }

        private bool SrVdiCombinationIsSuitableForMigration(SR sr, VDI vdi)
        {
            if (sr == null || vdi == null)
                return false;

            bool toLocal = sr.IsLocalSR;
            bool fromLocal = vdi.Connection.Resolve(vdi.SR).IsLocalSR;

            if (!SrIsLocalToTheHostOnWhichTheVmIsRunning(sr, vdi))
                return false;

            if (fromLocal && toLocal)
                return false;

            return true;
        }

        /// <summary>
        /// We can only move VDIs to a local SR if that local SR belongs
        /// to the host on which the VM is running.
        /// </summary>
        private bool SrIsLocalToTheHostOnWhichTheVmIsRunning(SR sr, VDI vdi)
        {
            List<Host> hostOfVdi = vdi.GetVMs().Select(vm => vm.Home()).ToList();
            return hostOfVdi.Any(sr.CanBeSeenFrom);
        }

        protected override string CannotBeShownReason
        {
            get
            {
                if (ExistingVDILocation())
                    return Messages.CURRENT_LOCATION;
                if (LocalToLocalMove())
                    return Messages.LOCAL_TO_LOCAL_MOVE;
                if (TheSR.IsLocalSR && !SrIsLocalToTheHostOnForExsistingVDIs())
                    return Messages.SRPICKER_ERROR_LOCAL_SR_MUST_BE_RESIDENT_HOSTS;
                if (!TheSR.CanBeSeenFrom(Affinity))
                    return string.Format(Messages.SR_CANNOT_BE_SEEN, Affinity == null ? Helpers.GetName(TheSR.Connection) : Helpers.GetName(Affinity));
                return base.CannotBeShownReason;
            }
        }

        private bool LocalToLocalMove()
        {
            return existingVDIs != null && existingVDIs.All(vdi => vdi.Connection.Resolve(vdi.SR).IsLocalSR && TheSR.IsLocalSR);
        }

        protected override bool CanBeEnabled
        {
            get
            {
                return Array.TrueForAll(existingVDIs, v => SrVdiCombinationIsSuitableForMigration(TheSR, v))
                       && TheSR.SupportsVdiCreate() && !ExistingVDILocation() && !TheSR.IsDetached && TargetSRHasEnoughFreeSpace;
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
                return !TheSR.IsDetached && TheSR.SupportsVdiCreate() && !ExistingVDILocation() &&
                       TargetSRHasEnoughFreeSpace;
            }
        }

        protected override string  CannotBeShownReason
        {   
	        get 
	        {
                if (TheSR.IsDetached)
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
            get { return TheSR.SupportsVdiCreate() && !TheSR.IsDetached && TargetSRHasEnoughFreeSpace; }
        }

        protected override string CannotBeShownReason
        {
            get
            {
                if (TheSR.IsDetached)
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
            get { return TheSR.CanBeSeenFrom(Affinity) && TheSR.CanCreateVmOn() && TargetSRHasEnoughFreeSpace; }
        }

        protected override string CannotBeShownReason
        {
            get
            {
                if (Affinity == null && !TheSR.shared)
                    return Messages.SR_IS_LOCAL;
                if (!TheSR.CanBeSeenFrom(Affinity))
                    return string.Format(Messages.SR_CANNOT_BE_SEEN, Affinity == null ? Helpers.GetName(TheSR.Connection) : Helpers.GetName(Affinity));
                return base.CannotBeShownReason;
            }
        }
    }

    /// <summary>
    /// Base class for the SRPickerItem stategies
    /// </summary>
    public abstract class SrPickerItem : CustomTreeNode, IComparable<SrPickerItem>
    {

        public SR TheSR { get; private set; }
        public bool Show { get; private set; }
        protected Host Affinity { get; private set; }
        protected long DiskSize { get; private set; }
        protected VDI[] existingVDIs { get; private set; }

        private SrPickerItem()
        {
            Show = true;
        }

        protected SrPickerItem(SR sr, Host aff, long diskSize, VDI[] vdis) : this()
        {
            existingVDIs = vdis;
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

        protected bool TargetSRHasEnoughFreeSpace
        {
            get
            {
                return TheSR.FreeSpace >= DiskSize;
            }
        }

        protected virtual bool UnsupportedSR
        {
            get { return TheSR.HBALunPerVDI; }
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
            Text = TheSR.Name;
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
            Description = string.Format(Messages.SRPICKER_DISK_FREE, Util.DiskSizeString(TheSR.FreeSpace, 2),
                                        Util.DiskSizeString(TheSR.physical_size, 2));
            CalculateEnabledSortReason();
            Show = true;
        }

        protected bool ExistingVDILocation()
        {
            return existingVDIs != null && existingVDIs.All(vdi => vdi.SR.opaque_ref == TheSR.opaque_ref);
        }

        protected virtual string CannotBeShownReason
        {
            get
            {
                if (TheSR.IsBroken(false))
                    return Messages.SR_IS_BROKEN;
                if (TheSR.IsFull)
                    return Messages.SRPICKER_SR_FULL;
                if (DiskSize > TheSR.physical_size)
                    return string.Format(Messages.SR_PICKER_DISK_TOO_BIG, Util.DiskSizeString(DiskSize, 2),
                                         Util.DiskSizeString(TheSR.physical_size, 2));
                if (DiskSize > (TheSR.FreeSpace))
                    return string.Format(Messages.SR_PICKER_INSUFFICIENT_SPACE, Util.DiskSizeString(DiskSize, 2),
                                         Util.DiskSizeString(TheSR.FreeSpace, 2));
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
            else if (TheSR.IsFull || TheSR.FreeSpace < DiskSize)
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
