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

namespace XenAdmin.Actions.SNMP
{
    public class SnmpConfiguration : IEquatable<SnmpConfiguration>
    {
        public bool IsSuccessful { get; set; }
        public bool ServiceStatus { get; set; }
        public bool IsSnmpEnabled { get; set; }
        public bool IsLogEnabled { get; set; }
        public bool IsV2CEnabled { get; set; }
        public bool IsV3Enabled { get; set; }
        public string Community { get; set; }
        public string UserName { get; set; }
        public string AuthPass { get; set; }
        public string AuthProtocol { get; set; }
        public string PrivacyPass { get; set; }
        public string PrivacyProtocol { get; set; }
        public SnmpConfiguration() { }

        public bool Equals(SnmpConfiguration other)
        {
            return other != null &&
                   IsSnmpEnabled == other.IsSnmpEnabled &&
                   IsLogEnabled == other.IsLogEnabled &&
                   IsV2CEnabled == other.IsV2CEnabled &&
                   IsV3Enabled == other.IsV3Enabled &&
                   Community == other.Community &&
                   UserName == other.UserName &&
                   AuthPass == other.AuthPass &&
                   AuthProtocol == other.AuthProtocol &&
                   PrivacyPass == other.PrivacyPass &&
                   PrivacyProtocol == other.PrivacyProtocol;
        }
    }

    public class SnmpXapiConfig
    {
        public const string XAPI_SNMP_PLUGIN_NAME = "snmp";
        public const string XAPI_SNMP_GET_CONFIG = "get-config";
        public const string XAPI_SNMP_GET_STATUS = "status";
        public const string XAPI_SNMP_SET_CONFIG = "set-config";
    }

    [Serializable]
    public class SnmpConfigurationRes<T>
    {
        public int code { get; set; }
        public T result { get; set; }
        public string message { get; set; }
    }

    [Serializable]
    public class GetConfigData
    {
        public GetConfigCommonData common { get; set; }
        public GetConfigSnmpData snmpd { get; set; }
        public GetConfigSnmpV2CData snmpd_v2c { get; set; }
        public GetConfigSnmpV3Data snmpd_v3 { get; set; }
    }

    [Serializable]
    public class GetConfigCommonData
    {
        public string enabled { get; set; }
        public string debug_log { get; set; }
    }

    [Serializable]
    public class GetConfigSnmpData
    {
        public string v2c { get; set; }
        public string v3 { get; set; }
    }

    [Serializable]
    public class GetConfigSnmpV2CData
    {
        public string community { get; set; }
    }

    [Serializable]
    public class GetConfigSnmpV3Data
    {
        public string user_name { get; set; }
        public string authentication_password { get; set; }
        public string authentication_protocol { get; set; }
        public string privacy_password { get; set; }
        public string privacy_protocol { get; set; }
    }

    [Serializable]
    public class GetServiceStatusData
    {
        public string enabled { get; set; }
        public string active { get; set; }
    }
}
