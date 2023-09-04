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
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;


namespace XenAdmin.Actions.Wlb
{
    [Flags]
    public enum SendWlbConfigurationKind
    {
        None = 0,
        SetPoolConfiguration = 1,
        SetScheduledTask = 2,
        DeleteScheduledTask = 4,
        SetReportSubscription = 8,
        DeleteReportSubscription = 16,
        SetHostConfiguration = 32
    }

    public class SendWlbConfigurationAction : AsyncAction
    {
        private const string SET_HOST_CONFIGURATION = "set_host_configuration";
        private const string SET_SCHEDULED_TASK = "set_scheduled_task";
        private const string DELETE_SCHEDULED_TASK = "delete_scheduled_task";
        private const string SET_REPORT_SUBSCRIPTIONS = "set_report_subscription";
        private const string DELETE_REPORT_SUBSCRIPTIONS = "delete_report_subscription";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly SendWlbConfigurationKind _kind;
        private readonly Dictionary<string, string> _wlbConfiguration;

        public SendWlbConfigurationAction(Pool pool, Dictionary<string, string> configuration, SendWlbConfigurationKind kind)
            : base(pool.Connection, string.Format(Messages.SAVING_WLB_CONFIGURATION_FOR, Helpers.GetName(pool).Ellipsise(50)), Messages.SAVING_WLB_CONFIGURATION, false)
        {
            Pool = pool;
            _wlbConfiguration = configuration;
            _kind = kind;			
			
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("pool.send_wlb_configuration");
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            #endregion
        }

        protected override void Run()
        {
            log.Debug("Sending Workload Balancing configuration for pool " + Pool.Name());

            ClearKeys();

            if (_kind.HasFlag(SendWlbConfigurationKind.SetHostConfiguration))
            {
                _wlbConfiguration.Add(SET_HOST_CONFIGURATION, "true");
            }
            if (_kind.HasFlag(SendWlbConfigurationKind.SetScheduledTask))
            {
                _wlbConfiguration.Add(SET_SCHEDULED_TASK, "true");
            }
            if (_kind.HasFlag(SendWlbConfigurationKind.DeleteScheduledTask))
            {
                _wlbConfiguration.Add(DELETE_SCHEDULED_TASK, "true");
            }
            if (_kind.HasFlag(SendWlbConfigurationKind.SetReportSubscription))
            {
                _wlbConfiguration.Add(SET_REPORT_SUBSCRIPTIONS, "true");
            }
            if (_kind.HasFlag(SendWlbConfigurationKind.DeleteReportSubscription))
            {
                _wlbConfiguration.Add(DELETE_REPORT_SUBSCRIPTIONS, "true");
            }

            try
            {
                Pool.send_wlb_configuration(Session, _wlbConfiguration);
                log.Debug("Successfully sent Workload Balancing configuration on pool " + Pool.Name());
                Description = Messages.COMPLETED;
            }
            catch (Exception ex)
            {
                if (ex is Failure f)
                {
                    WlbServerState.SetState(Pool, WlbServerState.ServerState.ConnectionError, f);

                    if (f.Message == FriendlyErrorNames.WLB_INTERNAL_ERROR)
                    {
                        var wlbError = WlbServerState.ConvertWlbError(f.ErrorDescription[1]);

                        if (wlbError != null)
                            throw new Failure(wlbError);
                    }
                }

                throw;
            }
        }

        private void ClearKeys()
        {
            _wlbConfiguration.Remove(SET_HOST_CONFIGURATION);
            _wlbConfiguration.Remove(SET_SCHEDULED_TASK);
            _wlbConfiguration.Remove(DELETE_SCHEDULED_TASK);
            _wlbConfiguration.Remove(SET_REPORT_SUBSCRIPTIONS);
            _wlbConfiguration.Remove(DELETE_REPORT_SUBSCRIPTIONS);
        }
    }
}
