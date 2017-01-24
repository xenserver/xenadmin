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
using XenAdmin.Core;
using XenAdmin.Wlb;
using XenAPI;


namespace XenAdmin.Actions
{
    public class ShutdownHostAction:HostAbstractAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ShutdownHostAction(Host host, Func<HostAbstractAction, Pool, long, long, bool> acceptNTolChanges)
            : base(host.Connection, Messages.HOST_SHUTDOWN, Messages.WAITING, acceptNTolChanges, null)
        {
            if (host == null)
                throw new ArgumentNullException("host");
            Host = host;
            ApiMethodsToRoleCheck.Add("pool.ha_compute_hypothetical_max_host_failures_to_tolerate");
            ApiMethodsToRoleCheck.Add("pool.set_ha_host_failures_to_tolerate");

            ApiMethodsToRoleCheck.Add("vm.assert_agile");
            ApiMethodsToRoleCheck.Add("vm.clean_shutdown");

            ApiMethodsToRoleCheck.Add("host.disable");
            ApiMethodsToRoleCheck.Add("host.enable");
            ApiMethodsToRoleCheck.Add("host.shutdown");
        }

        protected override void Run()
        {
            bool wasEnabled = Host.enabled;
            this.Description = string.Format(Messages.ACTION_HOST_SHUTTING_DOWN, Helpers.GetName(Host));

            MaybeReduceNtolBeforeOp(HostActionKind.Shutdown);
            ShutdownVMs(false);
            try
            {
                // set host poweroff task key values for wlb reporting purpose
                string wlbRecId = String.Empty;

                if (Host.other_config.ContainsKey(WlbOptimizationRecommendation.OPTIMIZINGPOOL))
                {
                    wlbRecId = Host.other_config[WlbOptimizationRecommendation.OPTIMIZINGPOOL];
                }

                string hostopaque_ref = Host.opaque_ref;

                RelatedTask = XenAPI.Host.async_shutdown(Session, Host.opaque_ref);

                // set host poweroff task key values for wlb reporting purpose
                if (Helpers.WlbEnabled(this.Connection) && !String.IsNullOrEmpty(wlbRecId))
                {
                    Task.add_to_other_config(this.Session, this.RelatedTask.opaque_ref, "wlb_advised", wlbRecId);
                    Task.add_to_other_config(this.Session, this.RelatedTask.opaque_ref, "wlb_action", "host_poweroff");
                    Task.add_to_other_config(this.Session, this.RelatedTask.opaque_ref, "wlb_action_obj_ref", hostopaque_ref);
                    Task.add_to_other_config(this.Session, this.RelatedTask.opaque_ref, "wlb_action_obj_type", "host");
                }

                PollToCompletion(95, 100);
            }
            catch (Exception e)
            {
                log.Error("Exception shutting down host: ", e);
                try
                {
                    // Try to re-enable the host 
                    if (wasEnabled)
                        XenAPI.Host.enable(Session, Host.opaque_ref);
                }
                catch (Exception edash)
                {
                    log.Error("Exception trying to re-enable host after error shutting down Host.", edash);
                }
                throw;
            }

            // Close the IXenConnection if it is not to a pool, or is to the master of a pool
            if (Helpers.HostIsMaster(Host))
            {
                Host.Connection.EndConnect();
            }

            this.Description = string.Format(Messages.ACTION_HOST_SHUTDOWN, Helpers.GetName(Host));
        }
    }

  
}
