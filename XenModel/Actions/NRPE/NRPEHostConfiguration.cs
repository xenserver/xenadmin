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
using XenAPI;

namespace XenAdmin.Actions.NRPE
{
    public class NRPEHostConfiguration : ICloneable
    {
        public const string XAPI_NRPE_PLUGIN_NAME = "nrpe";
        public const string XAPI_NRPE_STATUS = "status";
        public const string XAPI_NRPE_GET_CONFIG = "get-config";
        public const string XAPI_NRPE_GET_THRESHOLD = "get-threshold";
        public const string XAPI_NRPE_ENABLE = "enable";
        public const string XAPI_NRPE_DISABLE = "disable";
        public const string XAPI_NRPE_SET_CONFIG = "set-config";
        public const string XAPI_NRPE_SET_THRESHOLD = "set-threshold";
        public const string XAPI_NRPE_RESTART = "restart";

        public const string DEBUG_ENABLE = "1";
        public const string DEBUG_DISABLE = "0";

        public const string SSL_LOGGING_ENABLE = "0x2f";
        public const string SSL_LOGGING_DISABLE = "0x00";

        public static readonly string ALLOW_HOSTS_PLACE_HOLDER = Messages.NRPE_ALLOW_HOSTS_PLACE_HOLDER;

        private Dictionary<string, Check> _checkDict = new Dictionary<string, Check>();

        public bool EnableNRPE { get; set; }

        public string AllowHosts { get; set; }

        public bool Debug { get; set; }

        public bool SslLogging { get; set; }

        public Dictionary<string, Check> CheckDict => _checkDict;

        public RetrieveNRPEStatus Status { get; set; }

        public enum RetrieveNRPEStatus
        {
            Retrieving,
            Successful,
            Failed,
            Unsupport
        }

        public class Check
        {
            public string Name { get; }
            public string WarningThreshold { get; }
            public string CriticalThreshold{ get; }

            public Check(string name, string warningThreshold, string criticalThreshold)
            {
                Name = name;
                WarningThreshold = warningThreshold;
                CriticalThreshold = criticalThreshold;
            }

        }

        public void AddNRPECheck(Check checkItem)
        {
            _checkDict.Add(checkItem.Name, checkItem);
        }

        public bool GetNRPECheck(string name, out Check check)
        {
            return _checkDict.TryGetValue(name, out check);
        }

        public object Clone()
        {
            NRPEHostConfiguration cloned = new NRPEHostConfiguration
            {
                EnableNRPE = EnableNRPE,
                AllowHosts = AllowHosts,
                Debug = Debug,
                SslLogging = SslLogging
            };
            foreach (KeyValuePair<string, NRPEHostConfiguration.Check> kvp in _checkDict)
            {
                NRPEHostConfiguration.Check CurrentCheck = kvp.Value;
                cloned.AddNRPECheck(new Check(CurrentCheck.Name, CurrentCheck.WarningThreshold, CurrentCheck.CriticalThreshold));
            }

            return cloned;
        }
    }
}
