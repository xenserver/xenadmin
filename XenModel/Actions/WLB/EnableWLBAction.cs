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
    public class EnableWLBAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string OPTIMIZINGPOOL = "wlb_optimizing_pool";

        public Dictionary<string, string> WlbConfiguration = new Dictionary<string, string>();

        public EnableWLBAction(Pool pool)
            : base(pool.Connection, string.Format(Messages.ENABLING_WLB_ON, Helpers.GetName(pool).Ellipsise(50)), Messages.ENABLING_WLB, false)
        {
            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("pool.retrieve_wlb_configuration");
            ApiMethodsToRoleCheck.Add("pool.set_wlb_enabled");
            #endregion

            this.Pool = pool;
        }

        protected override void Run()
        {
            try
            {
                log.Debug("Resuming WLB on pool " + Pool.Name);
                XenAPI.Pool.set_wlb_enabled(this.Session, Pool.opaque_ref, true);
                log.Debug("Success resuming WLB on pool " + Pool.Name);
                this.Description = Messages.COMPLETED;

                WlbServerState.SetState(this.Pool, WlbServerState.ServerState.Enabled);

                //Clear the Optimizing Pool flag in case it was left behind
                Helpers.SetOtherConfig(this.Session, this.Pool, OPTIMIZINGPOOL, string.Empty);

                log.Debug("Retrieving Workload Balancing configuration for pool " + Pool.Name);
                this.WlbConfiguration = XenAPI.Pool.retrieve_wlb_configuration(this.Session);
                log.Debug("Success retrieving Workload Balancing configuration on pool " + Pool.Name);
                this.Description = Messages.COMPLETED;
            }
            catch (Exception ex)
            {
                if (ex is Failure)
                {
                    WlbServerState.SetState(Pool, WlbServerState.ServerState.ConnectionError, (Failure)ex);
                }
                throw ex;
            }
        }
    }
}
