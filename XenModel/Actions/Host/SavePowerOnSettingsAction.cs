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
using XenAPI;
using XenAdmin.Network;
using XenAdmin.Actions.Wlb;
using XenAdmin.Core;
using XenAdmin.Wlb;


namespace XenAdmin.Actions
{
    public class SavePowerOnSettingsAction : PureAsyncAction
    {
        private readonly List<KeyValuePair<Host, Host.PowerOnMode>> hostModes;

        public SavePowerOnSettingsAction(IXenConnection connection, List<KeyValuePair<Host, Host.PowerOnMode>> hostModes, bool suppressHistory)
            : base(connection, Messages.ACTION_CHANGE_POWER_ON, Messages.ACTION_CHANGING_POWER_ON, suppressHistory)
        {
            this.hostModes = hostModes;
        }

        protected override void Run()
        {
            foreach (var kvp in hostModes)
                SaveConfig(kvp.Key, kvp.Value);
        }

        private void SaveConfig(Host host, Host.PowerOnMode mode)
        {
            var newMode = mode.ToString();
            var config = mode.Config;
            string secretuuid = "";

            
            if (newMode == "iLO" && Helpers.StockholmOrGreater(Connection))
            {
                //the UI should have already prevented us from getting here, but be defensive
                newMode = "";
                config = new Dictionary<string, string>();
            }

            try
            {
                if (newMode == "DRAC" || newMode == "iLO" || newMode != "wake-on-lan" && newMode != "")
                {
                    if (config.ContainsKey("power_on_password_secret"))
                    {
                        secretuuid = Secret.CreateSecret(Session, config["power_on_password_secret"]);
                        config["power_on_password_secret"] = secretuuid;
                    }
                }
                else if (string.IsNullOrEmpty(newMode))
                {
                    //if WLB is on, we need to exclude the host from WLB power management,
                    //since we cannot turn it back on if it is powered down automatically

                    try
                    {
                        Pool pool = Helpers.GetPool(Connection);
                        if (pool != null && WlbServerState.GetState(pool) == WlbServerState.ServerState.Enabled)
                        {
                            var hostConfig = new WlbHostConfiguration(host.uuid) {ParticipatesInPowerManagement = false};

                            var wlbAction = new SendWlbConfigurationAction(pool, hostConfig.ToDictionary(),
                                SendWlbConfigurationKind.SetHostConfiguration);
                            wlbAction.RunSync(Session);
                        }
                    }
                    catch
                    {
                        //Do nothing on failure.
                    }
                }

                Host.set_power_on_mode(Session, host.opaque_ref, newMode, config);
            }
            catch
            {
                if (!string.IsNullOrEmpty(secretuuid))
                {
                    string secretRef = Secret.get_by_uuid(Session, secretuuid);
                    Secret.destroy(Session, secretRef);
                }

                throw;
            }
        }
    }
}
