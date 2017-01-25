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
using XenAdmin;


namespace XenAPI
{
    public partial class VIF : IComparable<VIF>
    {
        public const string RATE_LIMIT_QOS_VALUE = "ratelimit";
        public const string KBPS_QOS_PARAMS_KEY = "kbps";

        public string IPAddressesAsString()
        {
            var addresses = IPAddresses;
            if (addresses.Count > 0)
                return String.Join(", ", addresses.ToArray());
                
            return Messages.IP_ADDRESS_UNKNOWN;
        }

        public List<string> IPAddresses
        {
            get
            {
                VM vm = Connection.Resolve(this.VM);
                if (vm != null && !vm.is_a_template)
                {
                    VM_guest_metrics vmGuestMetrics = Connection.Resolve(vm.guest_metrics);

                    if (vmGuestMetrics != null)
                    {
                        // PR-1373 - VM_guest_metrics.networks is a dictionary of IP addresses in the format:
                        // [["0/ip", <IPv4 address>], ["0/ipv6/0", <IPv6 address>], ["0/ipv6/1", <IPv6 address>]]

                        return
                            (from network in vmGuestMetrics.networks
                             where network.Key.StartsWith(string.Format("{0}/ip", this.device))
                             orderby network.Key
                             select network.Value).ToList();
                    }
                }

                return new List<string>();
            }
        }

        public string NetworkName()
        {
            if (Connection == null) throw new NullReferenceException("NetworkName the VIF does not have connection");
            Network network = Connection.Resolve(this.network);
            if (network != null)
            {
                return network.Name;
            }
            else
            {
                return "-";
            }
        }

        public override int CompareTo(VIF other)
        {
            return device.CompareTo(other.device);
        }

        /// <summary>
        /// Tries to obtain a unique device id, re-claiming id's that have been removed.
        /// </summary>
        /// <returns></returns>
        public static int GetDeviceId(VM vm)
        {
            int i = 0;
            while (true)
            {
                bool found = false;
                foreach (XenAPI.XenRef<XenAPI.VIF> VIFRef in vm.VIFs)
                {
                    VIF TheVIF = vm.Connection.Resolve<XenAPI.VIF>(VIFRef);
                    if (TheVIF != null)
                    {
                        if (TheVIF.device == i.ToString())
                        {
                            found = true;
                            break;
                        }
                    }
                    else
                    {
                        found = true;
                    }
                }

                if (!found)
                    break;

                i++;
            }
            return i;
        }

        /// <summary>
        /// Gets the kbps of a rate limited VIF.
        /// Will return the empty string if no qos_params have been set on the VIF.
        /// </summary>
        public string LimitString
        {
            get
            {
                string result = null;
                if (qos_algorithm_params != null)
                {
                    if (qos_algorithm_params.ContainsKey(VIF.KBPS_QOS_PARAMS_KEY))
                        result = qos_algorithm_params[VIF.KBPS_QOS_PARAMS_KEY];
                }

                if (result == null) result = "";

                return result;
            }
        }

        public bool RateLimited
        {
            get
            {
                return qos_algorithm_type == VIF.RATE_LIMIT_QOS_VALUE;
            }
            set
            {
                qos_algorithm_type = value ? VIF.RATE_LIMIT_QOS_VALUE : "";
            }
        }
    }
}
