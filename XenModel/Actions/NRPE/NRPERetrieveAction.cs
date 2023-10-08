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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using XenAPI;


namespace XenAdmin.Actions.NRPE
{
    public class NRPERetrieveAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly NRPEHostConfiguration _nrpeCurrentConfig;
        private readonly Dictionary<string, CheckGroup> _checkGroupDictByName;

        private readonly IXenObject _clone;

        public NRPERetrieveAction(IXenObject host, NRPEHostConfiguration nrpeHostConfiguration, Dictionary<string, CheckGroup> checkGroupDictByName, bool suppressHistory)
            : base(host.Connection, Messages.NRPE_ACTION_RETRIEVING, Messages.NRPE_ACTION_RETRIEVING, suppressHistory)
        {
            _clone = host;
            _nrpeCurrentConfig = nrpeHostConfiguration;
            _checkGroupDictByName = checkGroupDictByName;
        }

        protected override void Run()
        {
            _nrpeCurrentConfig.Status = NRPEHostConfiguration.RetrieveNRPEStatus.Successful;
            if (_clone is Pool p)
            {
                List<Host> hostList = p.Connection.Cache.Hosts.ToList();
                Host checkHost = hostList[0];
                try
                {
                    Host.call_plugin(checkHost.Connection.Session, checkHost.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME,
                        NRPEHostConfiguration.XAPI_NRPE_GET_THRESHOLD, null);
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Execute NRPE plugin failed, failed reason: {0}.", e.Message);
                    _nrpeCurrentConfig.Status = e.Message.Contains("UNKNOWN_XENAPI_PLUGIN_FUNCTION") ?
                        NRPEHostConfiguration.RetrieveNRPEStatus.Unsupport : NRPEHostConfiguration.RetrieveNRPEStatus.Failed;
                }
            }
            else
            {
                try
                {
                    InitNRPEGeneralConfiguration();
                    InitNRPEThreshold();
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Execute NRPE plugin failed, failed reason: {0}.", e.Message);
                    _nrpeCurrentConfig.Status = e.Message.Contains("UNKNOWN_XENAPI_PLUGIN_FUNCTION") ?
                        NRPEHostConfiguration.RetrieveNRPEStatus.Unsupport : NRPEHostConfiguration.RetrieveNRPEStatus.Failed;
                }
            }
        }

        private void InitNRPEGeneralConfiguration()
        {
            string status = Host.call_plugin(_clone.Connection.Session, _clone.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME,
            NRPEHostConfiguration.XAPI_NRPE_STATUS, null);
            log.InfoFormat("Execute nrpe {0}, return: {1}", NRPEHostConfiguration.XAPI_NRPE_STATUS, status);
            _nrpeCurrentConfig.EnableNRPE = status.Trim().Equals("active enabled");

            string nrpeConfig = Host.call_plugin(_clone.Connection.Session, _clone.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME,
                NRPEHostConfiguration.XAPI_NRPE_GET_CONFIG, null);
            log.InfoFormat("Execute nrpe {0}, return: {1}", NRPEHostConfiguration.XAPI_NRPE_GET_CONFIG, nrpeConfig);

            string[] nrpeConfigArray = nrpeConfig.Split('\n');
            foreach (string nrpeConfigItem in nrpeConfigArray)
            {
                if (nrpeConfigItem.Trim().StartsWith("allowed_hosts:"))
                {
                    string allowHosts = nrpeConfigItem.Replace("allowed_hosts:", "").Trim();
                    _nrpeCurrentConfig.AllowHosts = AllowHostsWithoutLocalAddress(allowHosts);
                }
                else if (nrpeConfigItem.Trim().StartsWith("debug:"))
                {
                    string enableDebug = nrpeConfigItem.Replace("debug:", "").Trim();
                    _nrpeCurrentConfig.Debug = enableDebug.Equals(NRPEHostConfiguration.DEBUG_ENABLE);
                }
                else if (nrpeConfigItem.Trim().StartsWith("ssl_logging:"))
                {
                    string enableSslLogging = nrpeConfigItem.Replace("ssl_logging:", "").Trim();
                    _nrpeCurrentConfig.SslLogging = enableSslLogging.Equals(NRPEHostConfiguration.SSL_LOGGING_ENABLE);
                }
            }
        }

        private void InitNRPEThreshold()
        {
            string nrpeThreshold = Host.call_plugin(_clone.Connection.Session, _clone.opaque_ref, NRPEHostConfiguration.XAPI_NRPE_PLUGIN_NAME,
                NRPEHostConfiguration.XAPI_NRPE_GET_THRESHOLD, null);
            log.InfoFormat("Execute nrpe {0}, return: {1}", NRPEHostConfiguration.XAPI_NRPE_GET_THRESHOLD, nrpeThreshold);

            string[] nrpeThresholdArray = nrpeThreshold.Split('\n');
            foreach (string nrpeThresholdItem in nrpeThresholdArray)
            {
                // Return string format for each line: check_cpu warning threshold - 50 critical threshold - 80
                string[] thresholdRtnArray = nrpeThresholdItem.Split(' ');
                string checkName = thresholdRtnArray[0];
                if (_checkGroupDictByName.TryGetValue(thresholdRtnArray[0], out CheckGroup thresholdCheck))
                {
                    string warningThreshold = thresholdRtnArray[4];
                    string criticalThreshold = thresholdRtnArray[8];
                    thresholdCheck.UpdateThreshold(warningThreshold, criticalThreshold);
                    _nrpeCurrentConfig.AddNRPECheck(new NRPEHostConfiguration.Check(checkName, warningThreshold, criticalThreshold));
                }
            }
        }

        private string AllowHostsWithoutLocalAddress(string allowHosts)
        {
            string UpdatedAllowHosts = "";
            string[] AllowHostArray = allowHosts.Split(',');
            foreach (string allowHost in AllowHostArray)
            {
                if (!allowHost.Trim().Equals("127.0.0.1") &&
                    !allowHost.Trim().Equals("::1"))
                {
                    UpdatedAllowHosts += allowHost + ",";
                }
            }
            return UpdatedAllowHosts.Length == 0 ? NRPEHostConfiguration.ALLOW_HOSTS_PLACE_HOLDER :
                UpdatedAllowHosts.Substring(0, UpdatedAllowHosts.Length - 1);
        }
    }
}
