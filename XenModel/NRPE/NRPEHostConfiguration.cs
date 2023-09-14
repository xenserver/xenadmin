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

namespace XenAdmin
{
    public class NRPEHostConfiguration : ICloneable
    {
        public static readonly string XAPI_NRPE_PLUGIN_NAME = "nrpe";
        public static readonly string XAPI_NRPE_STATUS = "status";
        public static readonly string XAPI_NRPE_GET_CONFIG = "get-config";
        public static readonly string XAPI_NRPE_GET_THRESHOLD = "get-threshold";
        public static readonly string XAPI_NRPE_ENABLE = "enable";
        public static readonly string XAPI_NRPE_DISABLE = "disable";
        public static readonly string XAPI_NRPE_SET_CONFIG = "set-config";
        public static readonly string XAPI_NRPE_SET_THRESHOLD = "set-threshold";
        public static readonly string XAPI_NRPE_RESTART = "restart";

        public static readonly string DEBUG_ENABLE = "1";
        public static readonly string DEBUG_DISABLE = "0";

        public static readonly string SSL_LOGGING_ENABLE = "0xff";
        public static readonly string SSL_LOGGING_DISABLE = "0x00";

        public static readonly string ALLOW_HOSTS_PLACE_HOLDER = Messages.NRPE_ALLOW_HOSTS_PLACE_HOLDER;

        private bool enableNRPE;
        private string allowHosts;
        private bool debug;
        private bool sslLogging;
        private readonly Dictionary<string, Check> checkDict = new Dictionary<string, Check>();

        public bool EnableNRPE { get => enableNRPE; set => enableNRPE = value; }
        public string AllowHosts { get => allowHosts; set => allowHosts = value; }
        public bool Debug { get => debug; set => debug = value; }
        public bool SslLogging { get => sslLogging; set => sslLogging = value; }
        public Dictionary<string, Check> CheckDict { get => checkDict; }

        public class Check
        {
            public string Name;
            public string WarningThreshold;
            public string CriticalThreshold;

            public Check(string name, string warningThreshold, string criticalThreshold)
            {
                this.Name = name;
                this.WarningThreshold = warningThreshold;
                this.CriticalThreshold = criticalThreshold;
            }

        }

        public NRPEHostConfiguration()
        {
        }

        public void AddNRPECheck(Check checkItem)
        {
            checkDict.Add(checkItem.Name, checkItem);
        }

        public bool GetNRPECheck(string name, out Check check)
        {
            return checkDict.TryGetValue(name, out check);
        }

        public object Clone()
        {
            NRPEHostConfiguration cloned = new NRPEHostConfiguration
            {
                enableNRPE = enableNRPE,
                allowHosts = allowHosts,
                debug = debug,
                sslLogging = sslLogging
            };
            return cloned;
        }
    }
}
