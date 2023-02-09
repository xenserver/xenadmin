/* Copyright (c) Cloud Software Group, Inc. 
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
using XenAdmin;
using XenAdmin.Core;


namespace XenAPI
{
    public partial class Network : IComparable<Network>, IEquatable<Network>
    {
        public const string CREATE_IN_PROGRESS = "XenCenterCreateInProgress";
        public const long MTU_MIN = 1500;
        public const long MTU_DEFAULT = 1500;
        public const long MTU_MAX = 9216;


        public override string Description()
        {
            return name_description;
        }

        /// <returns>A friendly name for the network.</returns>
        public override string Name()
        {
            // Return the name_label, if its been changed by the user, or Network n where n is the
            // device number otherwise.  Take the device from the pool coordinator by default, or from
            // the first one we find otherwise.  If it's not attached anywhere, then give up and
            // return the name_label, which will be in the default form.

            if (name_label == "Host internal management network")
                return Messages.HOST_INTERNAL_MANAGEMENT_NETWORK;

            if (!name_label.StartsWith("Network associated with ") &&
                !name_label.StartsWith("Pool-wide network associated with "))
            {
                return name_label;
            }

            var pool = Helpers.GetPoolOfOne(Connection);
            if (pool == null)
                return name_label;

            var coordinator_ref = pool.master.opaque_ref;

            var pifs = Connection.ResolveAll(PIFs); 
            var pif = pifs.FirstOrDefault(p => p.host.opaque_ref == coordinator_ref); 
            if (pif != null)
                return PIFName(pif); 

            pif = pifs.FirstOrDefault(); 
            if (pif != null) 
                return PIFName(pif); 
            
            return name_label;
        }

        public bool AllHostsCanSeeNetwork()
        {
            foreach (Host host in Connection.Cache.Hosts)
            {
                if (!host.CanSeeNetwork(this))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return Name();
        }

        private string PIFName(PIF pif)
        {
            bool bond;

            if (pif.VLAN >= 0)
            {
                return string.Format(Messages.VLAN_NETWORK_NAME, pif.NICIdentifier(out bond), pif.VLAN);
            }
            else
            {
                return string.Format(Messages.NETWORK_NAME, pif.NICIdentifier(out bond));
            }
        }

        public bool GetAutoPlug()
        {
            // Note that this is not equivalent to BoolKey, because here
            // absence of the key gives AutoPlug=true, not false.
            return Get(other_config, "automatic") != "false";
        }

        public void SetAutoPlug(bool value)
        {
            other_config = SetDictionaryKey(other_config, "automatic", value ? "true" : "false");
        }

        public override bool Show(bool showHiddenVMs)
        {
                // CA-218956 - Expose HIMN when showing hidden objects
                if (IsGuestInstallerNetwork() && !showHiddenVMs)
                    return false;

                if (!ShowAllPifs(showHiddenVMs))
                    return false;

                if (showHiddenVMs)
                    return true;

                if (IsMember())
                    return false;

                return !IsHidden();
            
        }

        /// <summary>
        /// Returns whether the other_config.HideFromXenCenter flag is set to true.
        /// </summary>
        public override bool IsHidden()
        {
            return BoolKey(other_config, HIDE_FROM_XENCENTER);
        }

        public bool CreateInProgress()
        {
            return BoolKey(other_config, CREATE_IN_PROGRESS);
        }

        public bool IsGuestInstallerNetwork()
        {
            return BoolKey(other_config, "is_guest_installer_network");
        }

        /// <summary>
        /// Return true if the given network represents a bond, i.e. it has a PIF with
        /// IsBondNIC == true.
        /// </summary>
        public bool IsBond()
        {
            return TheBonds().Count > 0;
        }

        public List<XenRef<Bond>> TheBonds()
        {
            var ans = new List<XenRef<Bond>>();
            foreach (PIF pif in Connection.ResolveAll(PIFs))
            {
                if (pif.IsBondNIC())
                    ans.Add(pif.bond_master_of[0]);
            }
            return ans;
        }

        public bool IsMember()
        {
            if (Connection == null)
                return false;
            foreach (PIF pif in Connection.ResolveAll(PIFs))
            {
                if (pif.IsBondMember())
                    return true;
            }
            return false;
        }

        public bool IsInUseBondMember()
        {
            if (Connection == null)
                return false;
            foreach (PIF pif in Connection.ResolveAll(PIFs))
            {
                if (pif.IsInUseBondMember())
                    return true;
            }
            return false;
        }

        private bool ShowAllPifs(bool showHiddenVMs)
        {
            if (Connection == null)
                return true;

            foreach (PIF pif in Connection.ResolveAll(PIFs))
            {
                if (!pif.Show(showHiddenVMs))
                {
                    return false;
                }
            }
            return true;
        }

        public bool HasActiveVIFs()
        {
            if (Connection == null)
                return false;
            foreach (VIF vif in Connection.ResolveAll(VIFs))
            {
                if (vif.currently_attached)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Do not use for new networks, perform checks by hand:
        /// No jumbo frames on CHINs
        /// </summary>
        public bool CanUseJumboFrames()
        {
            if (Connection == null)
                return false;

            // not supported on CHINs
            if (Connection.ResolveAll(PIFs).Find(p => p.IsTunnelAccessPIF()) != null)
                return false;

            return true;
        }

        public string LinkStatusString()
        {
            if (PIFs.Count == 0)
                return Messages.NONE;

            List<PIF.LinkState> states = new List<PIF.LinkState>();
            foreach (PIF p in Connection.ResolveAll<PIF>(PIFs))
                states.Add(p.LinkStatus());

            bool Connected = states.Contains(PIF.LinkState.Connected);
            bool Disconnected = states.Contains(PIF.LinkState.Disconnected);
            bool Unknown = states.Contains(PIF.LinkState.Unknown);

            if (Connected)
            {
                if (Disconnected || Unknown)
                    return Messages.PARTIALLY_CONNECTED;
                else
                    return Messages.CONNECTED;
            }
            else if (Disconnected)
            {
                // "Partially disconnected" would a bit confusing as it might seem imply a known connected
                // PIF exists - go with Unknown if we just have d/c and unknown
                if (Unknown)
                    return Messages.UNKNOWN;
                else
                    return Messages.DISCONNECTED;
            }
            else
                return Messages.UNKNOWN;
        }

        public bool IsVLAN()
        {
            return Connection.ResolveAll(PIFs).Find(pif => pif.VLAN >= 0) != null;
        }

        public bool IsSriov()
        {
            return Connection.ResolveAll(PIFs).Find(pif => pif.NetworkSriov() != null) != null;
        }


        #region IEquatable<Network> Members

        /// <summary>
        /// Indicates whether the current object is equal to the specified object. This calls the implementation from XenObject.
        /// This implementation is required for ToStringWrapper.
        /// </summary>
        public bool Equals(Network other)
        {
            return base.Equals(other);
        }

        #endregion
    }
}
