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
                log.Debug("Retrieving Workload Balancing configuration for pool " + Pool.Name);
                this.WlbConfiguration = XenAPI.Pool.retrieve_wlb_configuration(this.Session);

                if (this.WlbConfiguration.Count == 0)
                {
                    //We didn;t get a configuration, so there is somethign wrong
                    log.Debug("Failure retrieving Workload Balancing configuration on pool " + Pool.Name);
                    this.Description = Messages.FAILED;
                    Failure f = new Failure(FriendlyErrorNames.WLB_NOT_INITIALIZED);
                    throw f;

                }
                else
                {


                    log.Debug("Success retrieving Workload Balancing configuration on pool " + Pool.Name);
                    this.Description = Messages.COMPLETED;

                    //Retrieving the configuration was successful, so update the WlbServerState to report the current state
                    //  This is here in case there was a previous communication error which has been fixed.
                    if (Helpers.WlbEnabled(Pool.Connection))
                    {
                        WlbServerState.SetState(Pool, WlbServerState.ServerState.Enabled);
                    }
                    else
                    {
                        WlbServerState.SetState(Pool, WlbServerState.ServerState.Disabled);
                    }
                }
            }
            catch(Exception ex)
            {
                if (ex is Failure)
                {
                    // Retrieving the configuration error could also because WLB is not initialized
                    if (((Failure)ex).Message == FriendlyErrorNames.WLB_NOT_INITIALIZED)
                        WlbServerState.SetState(Pool, WlbServerState.ServerState.NotConfigured);

                    else
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
    }
}
