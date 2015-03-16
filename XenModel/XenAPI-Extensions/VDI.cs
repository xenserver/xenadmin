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
using System.Text;
using XenAdmin;
using XenAdmin.Core;
using XenAdmin.Network.StorageLink;


namespace XenAPI
{
    public partial class VDI : IComparable<VDI>, IEquatable<VDI>
    {
        public override string Name
        {
            get
            {
                if (Connection != null)
                {
                    SR sr = Connection.Resolve(this.SR);
                    if (sr != null)
                    {
                        Host host = sr.GetStorageHost();
                        if (sr.Physical && host != null)
                            return string.Format(Messages.CD_DRIVE, host.Name);

                    }
                }
                return name_label;

            }
        }

        public override string Description
        {
            get
            {
                return name_description;
            }
        }


        public string VMsOfVDI
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                bool comma = false;

                foreach (VM vm in this.GetVMs().Where(vm => vm.Show(true)))
                {
                    if (comma)
                        sb.Append(", ");
                    else
                        comma = true;

                    sb.Append(vm.is_a_snapshot ? String.Format(Messages.SNAPSHOT_BRACKETS, vm.Name) : vm.Name);
                }

                return sb.ToString();
            }
        }

        public override bool Show(bool showHiddenVMs)
        {
            return (!missing && managed && (showHiddenVMs || !IsHidden));
        }

        public override bool IsHidden
        {
            get
            {
                return BoolKey(other_config, HIDE_FROM_XENCENTER);
            }
        }

        public IList<VM> GetVMs()
        {
            List<VM> vms = new List<VM>();

            if (this.type == vdi_type.crashdump)
            {
                foreach (Crashdump crashdump in Connection.ResolveAll<Crashdump>(crash_dumps))
                {
                    VM vm = Connection.Resolve<VM>(crashdump.VM);

                    if (vm != null)
                        vms.Add(vm);
                }
            }
            else if (this.type == vdi_type.suspend)
            {
                foreach (VM vm in Connection.Cache.VMs)
                {
                    VDI vdi = Connection.Resolve<VDI>(vm.suspend_VDI);

                    if (vdi != null && vdi.uuid == this.uuid)
                        vms.Add(vm);
                }
            }
            else
            {
                foreach (VBD vbd in Connection.ResolveAll(this.VBDs))
                {
                    VM vm = Connection.Resolve(vbd.VM);
                    if (vm != null)
                        vms.Add(vm);
                }
            }

            vms.Sort();

            return vms;
        }

        public string VMHint
        {
            get
            {
                if (sm_config != null && sm_config.ContainsKey("vmhint"))
                    return sm_config["vmhint"];
                else
                    return "";
            }
            set
            {
                if (value != VMHint)
                {
                    Dictionary<string, string> new_sm_config =
                        sm_config == null ?
                            new Dictionary<string, string>() :
                            new Dictionary<string, string>(sm_config);
                    new_sm_config["vmhint"] = value;
                    sm_config = new_sm_config;
                }
            }
        }

        public StorageLinkVolume StorageLinkVolume(IEnumerable<StorageLinkConnection> connections)
        {
            string svid;
            if (sm_config.TryGetValue("SVID", out svid) && !string.IsNullOrEmpty(svid))
            {
                foreach (StorageLinkConnection slCon in connections)
                {
                    StorageLinkVolume volume = new List<StorageLinkVolume>(slCon.Cache.StorageVolumes).Find(v => v.opaque_ref == svid);

                    if (volume != null)
                    {
                        return volume;
                    }
                }
            }
            return null;

        }

        public override string ToString()
        {
            return Name;
        }

        public virtual string SizeText
        {
            get
            {
                if (is_a_snapshot)
                    return String.Empty;
                else
                    return Util.DiskSizeString(virtual_size);
            }
        }

        public override string NameWithLocation
        {
            get
            {
                if (Connection != null)
                {
                    var srOfVdi = Connection.Resolve(SR);
                    if (srOfVdi == null)
                        return base.NameWithLocation;

                    return string.Format(Messages.VDI_ON_SR_TITLE, Name, srOfVdi.Name, srOfVdi.LocationString);
                }

                return base.NameWithLocation;
            }
        }

        #region IEquatable<VDI> Members

        /// <summary>
        /// Indicates whether the current object is equal to the specified object. This calls the implementation from XenObject.
        /// This implementation is required for ToStringWrapper.
        /// </summary>
        public bool Equals(VDI other)
        {
            return base.Equals(other);
        }

        #endregion

        /// <summary>
        /// These are some types of VDI we care to distinguish between in the GUI. Determining what type a VDI is for XC depends on not
        /// just the vdi.type fields supplied by the server but some other logic. This is contained in VDI.VDIType, which returns one of
        /// these enums.
        /// </summary>
        public enum FriendlyType { SNAPSHOT, ISO, SYSTEM_DISK, VIRTUAL_DISK, NONE };

        /// <summary>
        /// These are some types of VDI we care to distinguish between in the GUI. Determining what type a VDI is for XC depends on not
        /// just the vdi.type fields supplied by the server but some other logic. 
        /// </summary>
        public FriendlyType VDIType
        {
            get
            {
                SR sr = Connection.Resolve<SR>(SR);
                if (sr == null)
                    return FriendlyType.NONE;

                if (is_a_snapshot && GetVMs().Count >= 1)
                    return FriendlyType.SNAPSHOT;

                if (sr.content_type == XenAPI.SR.Content_Type_ISO)
                    return FriendlyType.ISO;

                if (type == vdi_type.system)
                    return FriendlyType.SYSTEM_DISK;

                return FriendlyType.VIRTUAL_DISK;
            }
        }

        /// <summary>
        /// Is the disk of a type used by HA?
        /// </summary>
        public bool IsHaType
        {
            get
            {
                // The HA types changed in Boston
                if (Helpers.BostonOrGreater(Connection))
                    return (type == vdi_type.ha_statefile || type == vdi_type.redo_log);
                else
                    return (type == vdi_type.ha_statefile || type == vdi_type.metadata);
            }
        }

        /// <summary>
        /// Is the disk currently being used by HA?
        /// </summary>
        public bool IsUsedByHA
        {
            get
            {
                return (IsHaType && Helpers.GetPoolOfOne(Connection).ha_enabled);
            }
        }

        /// <summary>
        /// Is the disk used by DR?
        /// </summary>
        public bool IsMetadataForDR
        {
            get { return Helpers.BostonOrGreater(Connection) && type == vdi_type.metadata; }
        }

        /// <summary>
        /// VDIs associated with storage motion are specially marked. This
        /// bool allows you to know if the current VDI is such.
        /// </summary>
        public bool IsAnIntermediateStorageMotionSnapshot
        {
            get { return sm_config.ContainsKey("base_mirror"); }
        }


        /// <summary>
        /// Try to determine if this VDI belongs to a WSS VM - this is a best guess only
        /// </summary>
        public bool CouldBeWss
        {
            get
            {
                const string wssName = "Webss-disk";
                return name_label.Contains(wssName);
            }
        }

        public bool IsCloudConfigDrive
        {
            get { return other_config.ContainsKey("config-drive") && other_config["config-drive"].ToLower() == "true"; }

        }
        
        /// <summary>
        /// Whether read caching is enabled on this disk on a specific host
        /// </summary>
        public bool ReadCachingEnabled(Host host)
        {
            return BoolKey(sm_config, "read-caching-enabled-on-" + host.uuid);
        }

        /// <summary>
        /// ... and if not, why not
        /// </summary>
        public ReadCachingDisabledReasonCode ReadCachingDisabledReason(Host host)
        {
            string reasonstr;
            if (sm_config.TryGetValue("read-caching-reason-" + host.uuid, out reasonstr))
            {
                ReadCachingDisabledReasonCode reason;
                if (Enum.TryParse(reasonstr, out reason))
                    return reason;
            }
            return ReadCachingDisabledReasonCode.UNKNOWN;
        }

        /// <summary>
        /// Possible reasons for read caching being disabled
        /// If there are multiple reasons, the topmost reason will be returned
        /// </summary>
        public enum ReadCachingDisabledReasonCode
        {
            UNKNOWN,  // catch-all, shouldn't occur if read caching is disabled
            LICENSE_RESTRICTION,  // self-explanatory
            SR_NOT_SUPPORTED,  // the SR is not NFS or EXT
            NO_RO_IMAGE,  // no part of the VDI is read-only => nothing to cache
            SR_OVERRIDE,  // the feature has been explicitly disabled for the VDI's SR
        }
    }
}
