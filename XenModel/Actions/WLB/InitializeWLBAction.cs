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
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.Wlb
{
    public class InitializeWLBAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _wlbUrl;
        private readonly string _wlbUserName;
        private readonly string _wlbPassword;
        private readonly string _xenServerUserName;
        private readonly string _xenServerPassword;
        private static string OPTIMIZINGPOOL = "wlb_optimizing_pool";

        public InitializeWLBAction(Pool pool, string wlbUrl, string wlbUserName, string wlbPassword, string xenServerUserName, string xenServerPassword)
            : base(pool.Connection, string.Format(Messages.INITIALIZING_WLB_ON, Helpers.GetName(pool).Ellipsise(50)), Messages.INITIALIZING_WLB, false)
        {
            this.Pool = pool;
            _wlbUrl = wlbUrl;
            _wlbUserName = wlbUserName;
            _wlbPassword = wlbPassword;
            _xenServerUserName = xenServerUserName;
            _xenServerPassword = xenServerPassword;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("vm.assert_agile");
            ApiMethodsToRoleCheck.Add("pool.initialize_wlb");
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            #endregion

        }

        protected override void Run()
        {
            try
            {
                log.Debug("Initializing Workload Balancing for pool " + Pool.Name);
                RelatedTask = XenAPI.Pool.async_initialize_wlb(this.Session, _wlbUrl, _wlbUserName, _wlbPassword, _xenServerUserName, _xenServerPassword);
                PollToCompletion();
                log.Debug("Success initializing WLB on pool " + Pool.Name);
                this.Description = Messages.COMPLETED;

                //Clear the Optimizing Pool flag in case it was left behind
                Helpers.SetOtherConfig(this.Session, this.Pool, OPTIMIZINGPOOL, string.Empty);
            }
            catch (Failure e)
            {
                if (e.Message == FriendlyErrorNames.WLB_INTERNAL_ERROR)
                {
                    Failure f = new Failure(new string[] { Messages.ResourceManager.GetString("WLB_ERROR_" + e.ErrorDescription[1]) });
                    throw (f);
                }
                else if (e.ErrorDescription[0] == FriendlyErrorNames.INTERNAL_ERROR)
                {
                    Failure f = new Failure(new string[] { Messages.ResourceManager.GetString("WLB_ERROR_SERVER_NOT_FOUND") });
                }
                else
                {
                    throw (e);
                }
            }
        }

    }
}
