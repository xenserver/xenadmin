﻿/* Copyright (c) Cloud Software Group, Inc. 
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

using Newtonsoft.Json;
using System;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Actions.SNMP
{
    public class SnmpRetrieveAction : AsyncAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SnmpConfiguration _snmpConfiguration;

        public SnmpRetrieveAction(IXenObject xenObject, SnmpConfiguration snmpConfiguration, bool suppressHistory)
            : base(xenObject.Connection, Messages.SNMP_ACTION_RETRIEVE, Messages.SNMP_ACTION_RETRIEVE, suppressHistory)
        {
            _snmpConfiguration = snmpConfiguration;
            if (xenObject is Pool p)
            {
                Pool = p;
                Host = Helpers.GetCoordinator(p);
            }
            else if (xenObject is Host h)
            {
                Host = h;
            }
            else
            {
                throw new ArgumentException($"{nameof(xenObject)} should be host or pool");
            }
        }

        protected override void Run()
        {
            _snmpConfiguration.IsSuccessful = true;
            try
            {
                InitSnmpGeneralConfiguration(Host);
                InitSnmpServiceStatus(Host);
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Run SNMP plugin failed, failed reason: {0}", e.Message);
                _snmpConfiguration.IsSuccessful = false;
            }
        }

        private void InitSnmpGeneralConfiguration(IXenObject o)
        {
            var resultJson = Host.call_plugin(o.Connection.Session, o.opaque_ref, SnmpXapiConfig.XAPI_SNMP_PLUGIN_NAME,
                SnmpXapiConfig.XAPI_SNMP_GET_CONFIG, null);
            Log.InfoFormat("Run SNMP {0}, return: {1}", SnmpXapiConfig.XAPI_SNMP_GET_CONFIG, resultJson);
            var resultObj = JsonConvert.DeserializeObject<SnmpConfigurationRes<GetConfigData>>(resultJson);
            if (resultObj == null || resultObj.code != 0)
            {
                _snmpConfiguration.IsSuccessful = false;
                return;
            }
            var resultData = resultObj.result;
            _snmpConfiguration.IsSnmpEnabled = resultData.common.enabled == "yes";
            _snmpConfiguration.IsLogEnabled = resultData.common.debug_log == "yes";
            _snmpConfiguration.IsV2CEnabled = resultData.snmpd.v2c == "yes";
            _snmpConfiguration.IsV3Enabled = resultData.snmpd.v3 == "yes";
            _snmpConfiguration.Community = resultData.snmpd_v2c.community;
            _snmpConfiguration.UserName = resultData.snmpd_v3.user_name;
            _snmpConfiguration.AuthPass = resultData.snmpd_v3.authentication_password;
            _snmpConfiguration.AuthProtocol = resultData.snmpd_v3.authentication_protocol;
            _snmpConfiguration.PrivacyPass = resultData.snmpd_v3.privacy_password;
            _snmpConfiguration.PrivacyProtocol = resultData.snmpd_v3.privacy_protocol;
        }

        private void InitSnmpServiceStatus(IXenObject o)
        {
            var resultJson = Host.call_plugin(o.Connection.Session, o.opaque_ref, SnmpXapiConfig.XAPI_SNMP_PLUGIN_NAME,
                SnmpXapiConfig.XAPI_SNMP_GET_STATUS, null);
            Log.InfoFormat("Run SNMP {0}, return: {1}", SnmpXapiConfig.XAPI_SNMP_GET_STATUS, resultJson);
            var resultObj = JsonConvert.DeserializeObject<SnmpConfigurationRes<GetServiceStatusData>>(resultJson);
            if (resultObj == null || resultObj.code != 0)
            {
                _snmpConfiguration.IsSuccessful = false;
                return;
            }
            var resultData = resultObj.result;
            _snmpConfiguration.ServiceStatus = resultData.enabled == "enabled" && resultData.active == "active";
        }
    }
}
