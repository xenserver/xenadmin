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
    public class RetrieveWlbConfigurationAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Dictionary<string,string> WlbConfiguration = new Dictionary<string,string>();

        public RetrieveWlbConfigurationAction(Pool pool)
            : base(pool.Connection, string.Format(Messages.RETRIEVING_WLB_CONFIGURATION_FOR, Helpers.GetName(pool).Ellipsise(50)), Messages.RETRIEVING_WLB_CONFIGURATION, true)
        {
            this.Pool = pool;
        }

        protected override void Run()
        {
            try
            {
                log.Debug("Retrieving Workload Balancing configuration for pool " + Pool.Name());
                WlbConfiguration = Pool.retrieve_wlb_configuration(this.Session);

                if (WlbConfiguration.Count == 0)
                {
                    log.Debug("Failure retrieving Workload Balancing configuration on pool " + Pool.Name());
                    Description = Messages.FAILED;
                    throw new Failure(FriendlyErrorNames.WLB_NOT_INITIALIZED);
                }

                log.Debug("Success retrieving Workload Balancing configuration on pool " + Pool.Name());
                Description = Messages.COMPLETED;

                //Retrieving the configuration was successful, so update the WlbServerState to report the current state
                //This is here in case there was a previous communication error which has been fixed.
                var state = Helpers.WlbEnabled(Pool.Connection) ? WlbServerState.ServerState.Enabled : WlbServerState.ServerState.Disabled;
                WlbServerState.SetState(Pool, state);
            }
            catch (Failure ex)
            {
                if (ex.Message == FriendlyErrorNames.WLB_NOT_INITIALIZED)
                    WlbServerState.SetState(Pool, WlbServerState.ServerState.NotConfigured);
                else
                    WlbServerState.SetState(Pool, WlbServerState.ServerState.ConnectionError, ex);

                if (ex.Message == FriendlyErrorNames.WLB_INTERNAL_ERROR)
                {
                    var wlbError = WlbServerState.ConvertWlbError(ex.ErrorDescription[1]);

                    if (wlbError != null)
                        throw new Failure(wlbError);
                }

                throw;
            }
        }
    }
}
