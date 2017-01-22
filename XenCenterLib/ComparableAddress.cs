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
using System.Net;
using System.Text;

namespace XenAdmin.Core
{
    // A ComparableAddress represents either an IP address (IPv4 or IPv6) or a PartialIP or a hostname
    public class ComparableAddress : IComparable
    {
        IPAddress addressIP = null;
        PartialIP partialIP = null;
        string addressString = null;

        private ComparableAddress(byte[] address)
        {
            addressIP = new IPAddress(address);
        }

        private ComparableAddress(PartialIP pip)
        {
            partialIP = pip;
        }

        private ComparableAddress(string address)
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(address));  // caught in TryParse()
            addressString = address;
        }

        private bool IsIP
        {
            get { return addressIP != null; }
        }

        private bool IsPartialIP
        {
            get { return partialIP != null; }
        }

        public static bool TryParse(String candidate, bool allowPartialIP, bool allowName, out ComparableAddress address)
        {
            address = null;
            if (string.IsNullOrEmpty(candidate))
                return false;

            IPAddress ipAddress;
            PartialIP pip;

            if (IPAddress.TryParse(candidate, out ipAddress))
            {
                address = new ComparableAddress(ipAddress.GetAddressBytes());
                return true;
            }

            else if (allowPartialIP && PartialIP.TryParse(candidate, out pip))
            {
                address = new ComparableAddress(pip);
                return true;
            }

            else if (allowName)
            {
                address = new ComparableAddress(candidate);
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            if (IsIP)
                return addressIP.ToString();
            else if (IsPartialIP)
                return partialIP.ToString();
            else
                return addressString;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is IPAddress)
            {
                if (this.IsIP)
                    return obj.Equals(addressIP);
                else if (this.IsPartialIP)
                    return partialIP.Equals(obj);
                else
                    return false;
            }
            else if (obj is ComparableAddress)
            {
                ComparableAddress other = obj as ComparableAddress;
                if (other.IsPartialIP && this.IsIP)
                    return other.partialIP.Equals(this.addressIP);
                else if (other.IsIP && this.IsPartialIP)
                    return this.partialIP.Equals(other.addressIP);
                else
                    return (CompareTo(obj) == 0);
            }
            else
                return false;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            ComparableAddress other = obj as ComparableAddress;

            if (other == null)
                return -1;

            if (this.IsIP)
            {
                if (other.IsIP)
                {
                    byte[] thisBytes = this.addressIP.GetAddressBytes();
                    byte[] otherBytes = other.addressIP.GetAddressBytes();

                    if (thisBytes.Length != otherBytes.Length)  // IPv6 address are deemed to come first
                        return otherBytes.Length - thisBytes.Length;

                    for (int i = 0; i < thisBytes.Length; ++i)
                    {
                        if (thisBytes[i] != otherBytes[i])
                            return (int)thisBytes[i] - (int)otherBytes[i];
                    }

                    return 0;
                }
                else
                    return -1;
            }

            else if (this.IsPartialIP)
            {
                if (other.IsIP)
                    return 1;
                else if (other.IsPartialIP)
                    return StringUtility.NaturalCompare(this.partialIP.ToString(), other.partialIP.ToString());
                else
                    return -1;
            }

            else
            {
                if (other.IsIP || other.IsPartialIP)
                    return 1;
                else
                    return addressString.CompareTo(other.addressString);
            }
        }

        #endregion
    }
}
