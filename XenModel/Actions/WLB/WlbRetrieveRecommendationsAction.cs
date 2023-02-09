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
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;


namespace XenAdmin.Actions.Wlb
{
    public class WlbRetrieveRecommendationsAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Dictionary<XenRef<VM>, string[]> Recommendations { get; private set; } = new Dictionary<XenRef<VM>, string[]>();

        public WlbRetrieveRecommendationsAction(Pool pool)
            : base(pool.Connection, string.Format(Messages.WLB_RETRIEVING_RECOMMENDATIONS, pool.Name()), true)
        {
            Pool = pool;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("pool.retrieve_wlb_recommendations");
            #endregion
        }

        protected override void Run()
        {
            try
            {
                log.DebugFormat("Retrieving recommendations for pool {0}", Pool.Name());
                Recommendations = Pool.retrieve_wlb_recommendations(Session);
                log.DebugFormat("Success retrieving recommendations for pool {0}", Pool.Name());

                //Retrieving the recommendations was successful, so update the WlbServerState to report the current state
                //This is here in case there was a previous communication error which has been fixed.

                var state = WlbServerState.GetState(Pool);

                if (state == WlbServerState.ServerState.ConnectionError || state == WlbServerState.ServerState.Unknown)
                {
                    var newState = Helpers.WlbEnabled(Pool.Connection)
                        ? WlbServerState.ServerState.Enabled
                        : WlbServerState.ServerState.Disabled;
                    WlbServerState.SetState(Pool, newState);
                }
            }
            catch (Failure f)
            {
                WlbServerState.SetState(Pool, WlbServerState.ServerState.ConnectionError, f);
            }
        }
    }
}
