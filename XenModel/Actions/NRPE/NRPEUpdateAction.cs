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

using System.Collections.Generic;
using System.Linq;
using XenAPI;


namespace XenAdmin.Actions.NRPE
{
    public class NRPEUpdateAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly NRPEHostConfiguration _nrpeOriginalConfig;  // NRPE configuration fetched from host
        private readonly NRPEHostConfiguration _nrpeHostConfiguration;  // NRPE configuration after user modified

        private readonly IXenObject _clone;

        public NRPEUpdateAction(IXenObject host, NRPEHostConfiguration nrpeHostConfiguration, NRPEHostConfiguration nrpeOriginalConfig, bool suppressHistory)
            : base(host.Connection, Messages.NRPE_ACTION_CHANGING, Messages.NRPE_ACTION_CHANGING, suppressHistory)
        {
            _clone = host;
            _nrpeHostConfiguration = nrpeHostConfiguration;
            _nrpeOriginalConfig = nrpeOriginalConfig;
        }

        protected override void Run()
        {
            if (_clone is Host)
            {
                SetNRPEConfigureForHost(_clone);
            }
            else
            {
                SetNRPEConfigureForPool();
            }
        }

        private void SetNRPEConfigureForHost(IXenObject o)
        {
            // Enable/Disable NRPE
            if (!_nrpeHostConfiguration.EnableNRPE == _nrpeOriginalConfig.EnableNRPE)
            {
                SetNRPEStatus(o, _nrpeHostConfiguration.EnableNRPE);
            }
            if (!_nrpeHostConfiguration.EnableNRPE) // If disable, return
            {
                return;
            }

            // NRPE General Configuration
            if (!_nrpeHostConfiguration.AllowHosts.Equals(_nrpeOriginalConfig.AllowHosts)
                || !_nrpeHostConfiguration.Debug.Equals(_nrpeOriginalConfig.Debug)
                || !_nrpeHostConfiguration.SslLogging.Equals(_nrpeOriginalConfig.SslLogging))
            {
                SetNRPEGeneralConfiguration(o, _nrpeHostConfiguration.AllowHosts, _nrpeHostConfiguration.Debug, _nrpeHostConfiguration.SslLogging);
            }

            // NRPE Check Threshold
            foreach (KeyValuePair<string, NRPEHostConfiguration.Check> kvp in _nrpeHostConfiguration.CheckDict)
            {
                NRPEHostConfiguration.Check currentCheck = kvp.Value;
                _nrpeOriginalConfig.GetNRPECheck(kvp.Key, out NRPEHostConfiguration.Check OriginalCheck);
                if (currentCheck != null && OriginalCheck != null
                    && (!currentCheck.WarningThreshold.Equals(OriginalCheck.WarningThreshold)
                    || !currentCheck.CriticalThreshold.Equals(OriginalCheck.CriticalThreshold)))
                {
                    SetNRPEThreshold(o, currentCheck.Name, currentCheck.WarningThreshold, currentCheck.CriticalThreshold);
                }
            }

            RestartNRPE(o);
        }

        private void SetNRPEConfigureForPool()
        {
            List<Host> hostList = ((Pool) _clone).Connection.Cache.Hosts.ToList();
            hostList.ForEach(host =>
            {
                SetNRPEConfigureForHost(host);
            });
        }

        private void SetNRPEStatus(IXenObject host, bool enableNRPE)
        {
            string nrpeUpdateStatusMethod = enableNRPE ?
                NRPEHostConfiguration.XAPI_NRPE_ENABLE : NRPEHostConfiguration.XAPI_NRPE_DISABLE;
            string result = Host.call_plugin(host.Connection.Session, host.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME,
                nrpeUpdateStatusMethod, null);
            log.InfoFormat("Execute nrpe {0}, return: {1}", nrpeUpdateStatusMethod, result);
        }

        private void SetNRPEGeneralConfiguration(IXenObject host, string allowHosts, bool debug, bool sslLogging)
        {
            Dictionary<string, string> configArgDict = new Dictionary<string, string>
                {
                    { "allowed_hosts", "127.0.0.1,::1," + (allowHosts.Equals(NRPEHostConfiguration.ALLOW_HOSTS_PLACE_HOLDER) ? "" : allowHosts) },
                    { "debug", debug ? NRPEHostConfiguration.DEBUG_ENABLE : NRPEHostConfiguration.DEBUG_DISABLE },
                    { "ssl_logging", sslLogging ? NRPEHostConfiguration.SSL_LOGGING_ENABLE : NRPEHostConfiguration.SSL_LOGGING_DISABLE}
                };
            string result = Host.call_plugin(host.Connection.Session, host.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME,
                NRPEHostConfiguration.XAPI_NRPE_SET_CONFIG, configArgDict);
            log.InfoFormat("Execute nrpe {0}, allowed_hosts={1}, debug={2}, ssl_logging={3}, return: {4}",
                NRPEHostConfiguration.XAPI_NRPE_SET_CONFIG,
                _nrpeHostConfiguration.AllowHosts,
                _nrpeHostConfiguration.Debug,
                _nrpeHostConfiguration.SslLogging,
                result);
        }

        private void SetNRPEThreshold(IXenObject host, string checkName, string warningThreshold, string criticalThreshold)
        {
            Dictionary<string, string> thresholdArgDict = new Dictionary<string, string>
                        {
                            { checkName, null },
                            { "w", warningThreshold },
                            { "c", criticalThreshold }
                        };
            string result = Host.call_plugin(host.Connection.Session, host.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME,
                NRPEHostConfiguration.XAPI_NRPE_SET_THRESHOLD, thresholdArgDict);
            log.InfoFormat("Execute nrpe {0}, check={1}, w={2}, c={3}, return: {4}",
                NRPEHostConfiguration.XAPI_NRPE_SET_THRESHOLD,
                checkName,
                warningThreshold,
                criticalThreshold,
                result);
        }

        // After modified NRPE configuration, we need to restart NRPE to take effect
        private void RestartNRPE(IXenObject host)
        {
            string result = Host.call_plugin(host.Connection.Session, host.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME,
                NRPEHostConfiguration.XAPI_NRPE_RESTART, null);
            log.InfoFormat("Execute nrpe {0}, return: {1}", NRPEHostConfiguration.XAPI_NRPE_RESTART, result);
        }
    }
}
