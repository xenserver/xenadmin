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
using System.Text;
using XenAPI;
using XenAdmin.Network;
using XenAdmin.Actions.Wlb;
using XenAdmin.Wlb;


namespace XenAdmin.Actions
{
    public class SavePowerOnSettingsAction : PureAsyncAction
    {
        private readonly List<Host> hosts;
        private readonly string newMode, ip, user, password;
        private readonly Dictionary<string, string> customConfig;

        public SavePowerOnSettingsAction(IXenConnection connection, List<Host> hosts, string newMode, string ip, string user, string password, Dictionary<string, string> customConfig, bool suppressHistory)
            : base(connection, Messages.ACTION_CHANGE_POWER_ON, Messages.ACTION_CHANGING_POWER_ON, suppressHistory)
        {
            this.hosts = hosts;
            this.newMode = newMode;
            this.ip = ip;
            this.user = user;
            this.password = password;
            this.customConfig = customConfig;
        }

        public SavePowerOnSettingsAction(Host host, string newMode, string ip, string user, string password,  Dictionary<string, string> customConfig, bool suppressHistory)
            : this(host.Connection, new List<Host>(), newMode, ip, user, password, customConfig, suppressHistory)
        {
            hosts.Add(host);
        }

        protected override void Run()
        {
            foreach (Host host in hosts)
                SaveConfig(host);
        }

        protected void SaveConfig(Host host)
        {
            string secretuuid = "";
            try
            {
                Dictionary<string, string> powerONConfig = new Dictionary<string, string>();
                if (newMode == "DRAC" || newMode == "iLO")
                {
                    powerONConfig.Add("power_on_ip", ip);
                    powerONConfig.Add("power_on_user", user);
                    secretuuid = Secret.CreateSecret(Session, password);
                    powerONConfig.Add("power_on_password_secret", secretuuid);
                }
                else if (newMode != "wake-on-lan" && newMode != "")
                {
                    foreach (KeyValuePair<string, string> pair in customConfig)
                    {
                        if (pair.Key == "power_on_password_secret")
                        {
                            secretuuid = Secret.CreateSecret(Session, pair.Value);
                            powerONConfig.Add(pair.Key, secretuuid);
                        }
                        else
                            powerONConfig.Add(pair.Key, pair.Value);
                    }
                }
                else if (string.IsNullOrEmpty(newMode))
                {
                    // We don't want this bit to alter the function of the main action so...
                    try
                    {
                        Pool pool = Core.Helpers.GetPool(this.Connection);
                        if (null != pool)
                        {
                            if (WlbServerState.GetState(pool) == XenAdmin.Wlb.WlbServerState.ServerState.Enabled)
                            {
                                //Tell WLB to not allow this host to be powered down automatically, since we cannot turn it back on
                                WlbHostConfiguration hostConfig = new WlbHostConfiguration(host.uuid);
                                hostConfig.ParticipatesInPowerManagement = false;
                                //Use the existing SendWlbConfig action, as it wraps the config functionality
                                //  as well as the RBAC checks.
                                SendWlbConfigurationAction wlbAction = new SendWlbConfigurationAction(pool,
                                                                                       hostConfig.ToDictionary(),
                                                                                       SendWlbConfigurationKind.SetHostConfiguration);
                                wlbAction.RunExternal(Session);
                            }
                        }
                    }

                    catch (Exception)
                    {
                        //Do nothing on failure.
                    }
                }
                
                Host.set_power_on_mode(Session, host.opaque_ref, newMode, powerONConfig);

            }
            catch (Exception e)
            {
                if (!string.IsNullOrEmpty(secretuuid))
                {
                    string secretRef = Secret.get_by_uuid(Session, secretuuid);
                    Secret.destroy(Session, secretRef);
                }
                throw e;
            }
        }
    }
}
