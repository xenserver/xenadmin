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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using XenAdmin.Core;
using XenAPI;

namespace XenAdmin.Actions.SNMP
{
    public class SnmpUpdateAction : AsyncAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SnmpConfiguration _snmpConfiguration;

        public SnmpUpdateAction(IXenObject xenObject, SnmpConfiguration snmpConfiguration, bool suppressHistory)
            : base(xenObject.Connection, Messages.SNMP_ACTION_UPDATE, Messages.SNMP_ACTION_UPDATE, suppressHistory)
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
            try
            {
                var hostList = Pool.Connection.Cache.Hosts.ToList();
                hostList.ForEach(UpdateSnmpConfigurationForHost);
            }
            catch (Exception e)
            {
                Log.ErrorFormat("SNMP update error: {0}", e);
                throw;
            }
        }

        private void UpdateSnmpConfigurationForHost(IXenObject o)
        {
            var configArgObj = GetConfigArgObj();
            //Ignore null values
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            Dictionary<string, string> configArgDict = new Dictionary<string, string>
            {
                {"config", JsonConvert.SerializeObject(configArgObj, settings)}
            };
            var resultJson = Host.call_plugin(o.Connection.Session, o.opaque_ref, SnmpXapiConfig.XAPI_SNMP_PLUGIN_NAME,
                SnmpXapiConfig.XAPI_SNMP_SET_CONFIG, configArgDict);
            var resultObj = JsonConvert.DeserializeObject<SnmpConfigurationRes<string>>(resultJson);
            Log.ErrorFormat("SNMP update result: {0}", resultJson);
            switch (resultObj.code)
            {
                case 1:
                    throw new Exception(Messages.SNMP_UPDATE_ERROR1);
                case 2:
                    throw new Exception(Messages.SNMP_UPDATE_ERROR2);
                case 3:
                    throw new Exception(Messages.SNMP_UPDATE_ERROR3);
            }
        }

        private object GetConfigArgObj()
        {
            dynamic configArgObj;
            if (!_snmpConfiguration.IsSnmpEnabled)
            {
                configArgObj = new
                {
                    common = new
                    {
                        enabled = "no"
                    }
                };
                return configArgObj;
            }
            configArgObj = new
            {
                common = new
                {
                    enabled = "yes",
                    debug_log = _snmpConfiguration.IsLogEnabled ? "yes" : "no"
                },
                snmpd = new
                {
                    v2c = _snmpConfiguration.IsV2CEnabled ? "yes" : "no",
                    v3 = _snmpConfiguration.IsV3Enabled ? "yes" : "no"
                },
                snmpd_v2c = _snmpConfiguration.IsV2CEnabled ? new
                {
                    community = _snmpConfiguration.Community
                } : null,
                snmpd_v3 = _snmpConfiguration.IsV3Enabled ? new
                {
                    user_name = _snmpConfiguration.UserName,
                    authentication_password = _snmpConfiguration.AuthPass,
                    authentication_protocol = _snmpConfiguration.AuthProtocol,
                    privacy_password = _snmpConfiguration.PrivacyPass,
                    privacy_protocol = _snmpConfiguration.PrivacyProtocol
                } : null
            };
            return configArgObj;
        }
    }
}
