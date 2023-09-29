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
using XenAdmin.Actions.Wlb;
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;


namespace XenAdmin.Actions.HostActions
{
    public class HostPowerOnAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public HostPowerOnAction(Host host)
            : base(host.Connection, Messages.HOST_POWER_ON)
        {
            Host = host;
            AddCommonAPIMethodsToRoleCheck();

            ApiMethodsToRoleCheck.Add("pool.send_wlb_configuration");
            ApiMethodsToRoleCheck.Add("host.power_on");
        }

        protected override void Run()
        {
            bool succeeded = false;
            string name = Helpers.GetName(Host);
            Host coordinator = Helpers.GetCoordinator(Connection);
            AppliesTo.Add(coordinator.opaque_ref);
            Title = string.Format(Messages.ACTION_HOST_START_TITLE, name);
            Description = Messages.ACTION_HOST_STARTING;

            try
            {
                Host.power_on(Session, Host.opaque_ref);
                Description = Messages.ACTION_HOST_STARTED;
                succeeded = true;
            }
            catch (Exception e)
            {
                if (e is Failure f)
                {
                    if (f.ErrorDescription.Count > 2)
                    {
                        switch (f.ErrorDescription[2])
                        {
                            case "DRAC_NO_SUPP_PACK":
                                throw new Exception(Messages.DRAC_NO_SUPP_PACK);
                            case "DRAC_POWERON_FAILED":
                                throw new Exception(Messages.DRAC_POWERON_FAILED);
                            case "ILO_CONNECTION_ERROR":
                                throw new Exception(Messages.ILO_CONNECTION_ERROR);
                            case "ILO_POWERON_FAILED":
                                throw new Exception(Messages.ILO_POWERON_FAILED);
                        }
                    }

                    throw new Exception(string.Format(Messages.POWER_ON_REQUEST_FAILED, Host));
                }
                throw;
            }
            finally
            {
                if (Helpers.WlbConfigured(Connection) && Helpers.WlbEnabledAndConfigured(Connection))
                {
                    UpdateHostLastPowerOnSucceeded(succeeded, Host);
                }
            }
        }

        /// <summary>
        /// Attempts to set the LastPowerOnSucceeded flag in the WLB Host configuration
        /// </summary>
        private void UpdateHostLastPowerOnSucceeded(bool succeeded, Host host)
        {
            try
            {
                WlbHostConfiguration hostConfig = new WlbHostConfiguration(host.uuid);
                hostConfig.LastPowerOnSucceeded = succeeded;
                if (!succeeded)
                {
                    hostConfig.ParticipatesInPowerManagement = false;
                }

                Pool pool = Helpers.GetPoolOfOne(host.Connection);
                if (null != pool)
                {
                    SendWlbConfigurationAction action = new SendWlbConfigurationAction(pool, hostConfig.ToDictionary(), SendWlbConfigurationKind.SetHostConfiguration);
                    action.RunSync(Session);
                }
                else
                {
                    throw new Failure(Failure.INTERNAL_ERROR, string.Format(Messages.POOL_GONE, BrandManager.BrandConsole));
                }
            }
            catch (Exception ex)
            {
                log.Error("Unable to set the host's LastPowerOnSucceeded status.", ex);
            }
        }
    }
}
