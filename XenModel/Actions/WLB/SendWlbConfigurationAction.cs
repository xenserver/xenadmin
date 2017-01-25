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

using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.Wlb
{
    [Flags]
    public enum SendWlbConfigurationKind : int
    {
        None = 0,
        SetPoolConfiguration = 1,
        SetScheduledTask = 2,
        DeleteScheduledTask = 4,
        SetReportSubscription = 8,
        DeleteReportSubscription = 16,
        SetHostConfiguration = 32
    };

    public class SendWlbConfigurationAction : AsyncAction
    {
        private static string SET_HOST_CONFIGURATION = "set_host_configuration";
        private static string SET_SCHEDULED_TASK = "set_scheduled_task";
        private static string DELETE_SCHEDULED_TASK = "delete_scheduled_task";
        private static string SET_REPORT_SUBSCRIPTIONS = "set_report_subscription";
        private static string DELETE_REPORT_SUBSCRIPTIONS = "delete_report_subscription";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SendWlbConfigurationKind _kind;
        private Dictionary<string, string> WlbConfiguration = new Dictionary<string, string>();

        public SendWlbConfigurationAction(Pool pool, Dictionary<string, string> configuration, SendWlbConfigurationKind kind)
            : base(pool.Connection, string.Format(Messages.SAVING_WLB_CONFIGURATION_FOR, Helpers.GetName(pool).Ellipsise(50)), Messages.SAVING_WLB_CONFIGURATION, false)
        {
            this.Pool = pool;
            this.WlbConfiguration = configuration;
            this._kind = kind;			
			
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("pool.send_wlb_configuration");
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            #endregion
        }

        protected override void Run()
        {
            log.Debug("Sending Workload Balancing configuration for pool " + Pool.Name);

            ClearKeys();

            if ((_kind & SendWlbConfigurationKind.SetHostConfiguration) == SendWlbConfigurationKind.SetHostConfiguration)
            {
                this.WlbConfiguration.Add(SET_HOST_CONFIGURATION, "true");
            }
            if ((_kind & SendWlbConfigurationKind.SetScheduledTask) == SendWlbConfigurationKind.SetScheduledTask)
            {
                this.WlbConfiguration.Add(SET_SCHEDULED_TASK, "true");
            }
            if ((_kind & SendWlbConfigurationKind.DeleteScheduledTask) == SendWlbConfigurationKind.DeleteScheduledTask)
            {
                this.WlbConfiguration.Add(DELETE_SCHEDULED_TASK, "true");
            }
            if ((_kind & SendWlbConfigurationKind.SetReportSubscription) == SendWlbConfigurationKind.SetReportSubscription)
            {
                this.WlbConfiguration.Add(SET_REPORT_SUBSCRIPTIONS, "true");
            }
            if ((_kind & SendWlbConfigurationKind.DeleteReportSubscription) == SendWlbConfigurationKind.DeleteReportSubscription)
            {
                this.WlbConfiguration.Add(DELETE_REPORT_SUBSCRIPTIONS, "true");
            }

            try
            {
                XenAPI.Pool.send_wlb_configuration(this.Session, this.WlbConfiguration);
                log.Debug("Successfully sent Workload Balancing configuration on pool " + Pool.Name);
                this.Description = Messages.COMPLETED;
            }
            catch (Exception ex)
            {
                if (ex is Failure)
                {
                    WlbServerState.SetState(Pool, WlbServerState.ServerState.ConnectionError, (Failure)ex);

                    if (((Failure)ex).Message == FriendlyErrorNames.WLB_INTERNAL_ERROR)
                    {
                        Failure f = new Failure(new string[] { Messages.ResourceManager.GetString("WLB_ERROR_" + ((Failure)ex).ErrorDescription[1]) });
                        throw (f);
                    }
                    else
                    {
                        throw (ex);
                    }
                }
            }
        }

        private void ClearKeys()
        {
            this.WlbConfiguration.Remove(SET_HOST_CONFIGURATION);
            this.WlbConfiguration.Remove(SET_SCHEDULED_TASK);
            this.WlbConfiguration.Remove(DELETE_SCHEDULED_TASK);
            this.WlbConfiguration.Remove(SET_REPORT_SUBSCRIPTIONS);
            this.WlbConfiguration.Remove(DELETE_REPORT_SUBSCRIPTIONS);
        }
    }
}
