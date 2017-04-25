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

namespace XenAPI
{
    partial class Bond
    {
        public override string Name
        {
            get
            {
                PIF pif = FindMaster();
                return pif == null ? "" : pif.Name;
            }
        }

        private PIF FindMaster()
        {
            return Connection == null ? null : Connection.Resolve(master);
        }

        public enum hashing_algoritm
        {
            src_mac,
            tcpudp_ports,
            unknown
        }

        public hashing_algoritm HashingAlgoritm 
        {
            get
            {
                if (properties.ContainsKey("hashing_algorithm"))
                    return StringToHashingAlgoritm(properties["hashing_algorithm"]);
                return hashing_algoritm.unknown;
            }
        }

        public static string HashingAlgoritmToString(hashing_algoritm hashingAlgoritm)
        {
            switch (hashingAlgoritm)
            {
                case hashing_algoritm.src_mac:
                    return "src_mac";
                case hashing_algoritm.tcpudp_ports:
                    return "tcpudp_ports";
                default:
                    return "";
            }
        }

        public static hashing_algoritm StringToHashingAlgoritm(string hashingAlgoritm)
        {
            switch (hashingAlgoritm)
            {
                case "src_mac":
                    return hashing_algoritm.src_mac;
                case "tcpudp_ports":
                    return hashing_algoritm.tcpudp_ports;
                default:
                    return hashing_algoritm.unknown;
            }
        }
    }
}
