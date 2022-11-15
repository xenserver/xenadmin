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
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Controls
{
    public class SrPickerMigrateItem : SrPickerItem
    {
        public SrPickerMigrateItem(SR sr, Host aff, VDI[] vdis)
            : base(sr, aff, vdis)
        {
        }

        protected override bool CanBeEnabled(out string cannotEnableReason)
        {
            if (IsCurrentLocation(out cannotEnableReason))
                return false;

            if (TheSR.IsLocalSR())
            {
                foreach (var vdi in ExistingVDIs)
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
                            {
                                cannotEnableReason = vm.power_state == vm_power_state.Running
                                    ? Messages.SRPICKER_ERROR_LOCAL_SR_MUST_BE_RESIDENT_HOSTS
                                    : string.Format(Messages.SR_CANNOT_BE_SEEN, Helpers.GetName(homeHost));
                                return false;
                            }
                        }
                    }

                    if (homeHosts.Count == 0)
                    {
                        cannotEnableReason = Messages.SR_IS_LOCAL;
                        return false;
                    }
                }
            }

            return SupportsStorageMigration(out cannotEnableReason) &&
                   SupportsVdiCreate(out cannotEnableReason) &&
                   !IsDetached(out cannotEnableReason) &&
                   TheSR.CanFitDisks(out cannotEnableReason, ExistingVDIs);
        }
    }


    public class SrPickerCopyItem : SrPickerItem
    {
        public SrPickerCopyItem(SR sr, Host aff, VDI[] vdis)
            : base(sr, aff, vdis)
        {
        }

        protected override bool CanBeEnabled(out string cannotEnableReason)
        {
            return !IsDetached(out cannotEnableReason) &&
                   SupportsVdiCreate(out cannotEnableReason) &&
                   TheSR.CanFitDisks(out cannotEnableReason, ExistingVDIs);
        }
    }

    
    public class SrPickerMoveItem : SrPickerItem
    {
        public SrPickerMoveItem(SR sr, Host aff, VDI[] vdis)
            : base(sr, aff, vdis)
        {
        }

        protected override bool CanBeEnabled(out string cannotEnableReason)
        {
            return !IsDetached(out cannotEnableReason) &&
                   !IsCurrentLocation(out cannotEnableReason) &&
                   SupportsVdiCreate(out cannotEnableReason) &&
                   TheSR.CanFitDisks(out cannotEnableReason, ExistingVDIs);
        }
    }


    public class SrPickerInstallFromTemplateItem : SrPickerItem
    {
        public SrPickerInstallFromTemplateItem(SR sr, Host aff, VDI[] vdis)
            : base(sr, aff, vdis)
        {
        }

        protected override bool CanBeEnabled(out string cannotEnableReason)
        {
            return SupportsVdiCreate(out cannotEnableReason) &&
                   !IsDetached(out cannotEnableReason) &&
                   TheSR.CanFitDisks(out cannotEnableReason, ExistingVDIs);
        }
    }


    public class SrPickerVmItem : SrPickerItem
    {
        public SrPickerVmItem(SR sr, Host aff, VDI[] vdis)
            : base(sr, aff, vdis)
        {
        }

        protected override bool CanBeEnabled(out string cannotEnableReason)
        {
            return CanBeSeenFromAffinity(out cannotEnableReason) &&
                   SupportsVdiCreate(out cannotEnableReason) &&
                   !IsBroken(out cannotEnableReason) &&
                   TheSR.CanFitDisks(out cannotEnableReason, ExistingVDIs);
        }
    }


    public class SrPickerLunPerVDIItem : SrPickerVmItem
    {
        public SrPickerLunPerVDIItem(SR sr, Host aff, VDI[] vdis)
            : base(sr, aff, vdis)
        {
        }

        protected override bool CanBeEnabled(out string cannotEnableReason)
        {
            if (TheSR.HBALunPerVDI())
                return CanBeSeenFromAffinity(out cannotEnableReason) &&
                       !IsBroken(out cannotEnableReason);

            return base.CanBeEnabled(out cannotEnableReason);
        }

        protected override bool SupportsCurrentOperation => true;
    }


    public abstract class SrPickerItem : CustomTreeNode, IComparable<SrPickerItem>
    {
        private bool _scanning;
        public SR TheSR { get; }
        public bool Show { get; private set; } = true;
        protected readonly Host Affinity;
        protected VDI[] ExistingVDIs { get; private set; }

        public event Action<SrPickerItem> ItemUpdated;

        protected SrPickerItem(SR sr, Host aff, VDI[] vdis)
        {
            ExistingVDIs = vdis ?? Array.Empty<VDI>();
            TheSR = sr;
            Affinity = aff;
            Update();
        }

        public bool Scanning
        {
            get => _scanning;
            set
            {
                _scanning = value;
                Update();
            }
        }

        protected virtual bool SupportsCurrentOperation => !TheSR.HBALunPerVDI();

        protected abstract bool CanBeEnabled(out string cannotEnableReason);

        public void UpdateDisks(params VDI[] disks)
        {
            ExistingVDIs = disks;
            Update();
        }

        public void Update()
        {
            Text = TheSR.Name();
            Image = Images.GetImage16For(TheSR);

            if (!SupportsCurrentOperation || !SupportsVdiCreate(out _) ||
                !TheSR.Show(Properties.Settings.Default.ShowHiddenVMs))
            {
                Show = false;
                return;
            }

            if (Scanning)
            {
                Description = Messages.SR_REFRESH_ACTION_TITLE_GENERIC;
                Enabled = false;
            }
            else if (CanBeEnabled(out var cannotEnableReason))
            {
                Description = string.Format(Messages.SRPICKER_DISK_FREE, Util.DiskSizeString(TheSR.FreeSpace(), 2),
                    Util.DiskSizeString(TheSR.physical_size, 2));
                Enabled = true;
            }
            else
            {
                Description = cannotEnableReason;
                Enabled = false;
            }

            ItemUpdated?.Invoke(this);
        }

        protected bool IsCurrentLocation(out string cannotEnableReason)
        {
            if (ExistingVDIs.Length > 0 && ExistingVDIs.All(vdi => vdi.SR.opaque_ref == TheSR.opaque_ref))
            {
                cannotEnableReason = Messages.CURRENT_LOCATION;
                return true;
            }

            cannotEnableReason = string.Empty;
            return false;
        }

        protected bool IsBroken(out string cannotEnableReason)
        {
            if (TheSR.IsBroken(false))
            {
                cannotEnableReason = Messages.SR_IS_BROKEN;
                return true;
            }

            cannotEnableReason = string.Empty;
            return false;
        }

        protected bool IsDetached(out string cannotEnableReason)
        {
            if (TheSR.IsDetached())
            {
                cannotEnableReason = Messages.SR_DETACHED;
                return true;
            }

            cannotEnableReason = string.Empty;
            return false;
        }

        protected bool SupportsVdiCreate(out string cannotEnableReason)
        {
            if (TheSR.SupportsVdiCreate())
            {
                cannotEnableReason = string.Empty;
                return true;
            }

            cannotEnableReason = Messages.STORAGE_READ_ONLY;
            return false;
        }

        protected bool SupportsStorageMigration(out string cannotEnableReason)
        {
            if (TheSR.SupportsStorageMigration())
            {
                cannotEnableReason = string.Empty;
                return true;
            }

            cannotEnableReason = Messages.UNSUPPORTED_SR_TYPE;
            return false;
        }

        protected bool CanBeSeenFromAffinity(out string cannotEnableReason)
        {
            if (Affinity == null)
            {
                if (TheSR.shared)
                {
                    cannotEnableReason = string.Empty;
                    return true;
                }

                cannotEnableReason = Messages.SR_IS_LOCAL;
                return false;
            }

            foreach (var pbdRef in TheSR.PBDs)
            {
                var pbd = TheSR.Connection.Resolve(pbdRef);
                    
                if (pbd.host.opaque_ref == Affinity.opaque_ref)
                {
                    if (pbd.currently_attached)
                    {
                        cannotEnableReason = string.Empty;
                        return true;
                    }

                    cannotEnableReason = string.Format(Messages.SR_DETACHED_FROM_HOST, Helpers.GetName(Affinity));
                    return false;
                }
            }

            cannotEnableReason = string.Format(Messages.SR_CANNOT_BE_SEEN, Helpers.GetName(Affinity));
            return false;
        }

        public int CompareTo(SrPickerItem other)
        {
            return base.CompareTo(other);
        }

        protected override int SameLevelSortOrder(CustomTreeNode other)
        {
            SrPickerItem otherItem = other as SrPickerItem;
            if (otherItem == null) //shouldn't ever happen!!!
                return -1;

            if (!otherItem.Enabled && Enabled)
                return -1;
            if (otherItem.Enabled && !Enabled)
                return 1;

            return base.SameLevelSortOrder(otherItem);
        }


        public static SrPickerItem Create(SR sr, SrPicker.SRPickerType usage, Host aff, VDI[] vdis)
        {
            switch (usage)
            {
                case SrPicker.SRPickerType.Migrate:
                    return new SrPickerMigrateItem(sr, aff, vdis);
                case SrPicker.SRPickerType.Copy:
                    return new SrPickerCopyItem(sr, aff, vdis);
                case SrPicker.SRPickerType.Move:
                    return new SrPickerMoveItem(sr, aff, vdis);
                case SrPicker.SRPickerType.InstallFromTemplate:
                    return new SrPickerInstallFromTemplateItem(sr, aff, vdis);
                case SrPicker.SRPickerType.VM:
                    return new SrPickerVmItem(sr, aff, vdis);
                case SrPicker.SRPickerType.LunPerVDI:
                    return new SrPickerLunPerVDIItem(sr, aff, vdis);
                default:
                    throw new ArgumentException("There is no SRPickerItem for the type: " + usage);
            }
        }
    }
}
