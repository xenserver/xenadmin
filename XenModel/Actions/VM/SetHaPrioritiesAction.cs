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
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{
    /// <summary>
    /// Sets HA restart priorities and ntol. Never sets ha_always_run, but will unset it for VMs given priority DoNotRestart.
    /// </summary>
    public class SetHaPrioritiesAction : AsyncAction
    {
        private readonly Dictionary<VM, VMStartupOptions> settings = new Dictionary<VM, VMStartupOptions>();
        private readonly long ntol;
        private readonly Pool pool;

        public SetHaPrioritiesAction(IXenConnection connection, Dictionary<VM, VMStartupOptions> settings, long ntol, bool suppressHistory)
            : base(connection, Messages.HA_SETTING_PRIORITIES, suppressHistory)
        {
            System.Diagnostics.Trace.Assert(connection != null);
            System.Diagnostics.Trace.Assert(settings != null);
            System.Diagnostics.Trace.Assert(ntol >= 0);

            this.settings = settings;
            this.ntol = ntol;
            pool = Helpers.GetPoolOfOne(Connection);
            Pool = pool ?? throw new Failure(Failure.INTERNAL_ERROR, string.Format(Messages.POOL_GONE, BrandManager.BrandConsole));

            ApiMethodsToRoleCheck.AddRange(
                "VM.set_ha_restart_priority",
                "VM.set_order",
                "VM.set_start_delay",
                "pool.set_ha_host_failures_to_tolerate",
                "pool.async_sync_database");
        }

        protected override void Run()
        {
            // First move any VMs from protected -> unprotected
            int i = 0;
            foreach (VM vm in settings.Keys)
            {
                if (VM.HaPriorityIsRestart(vm.Connection, (VM.HA_Restart_Priority)settings[vm].HaRestartPriority))
                    continue;

                Description = string.Format(Messages.HA_SETTING_PRIORITY_ON_X, Helpers.GetName(vm));

                VM.SetHaRestartPriority(Session, vm, (VM.HA_Restart_Priority)settings[vm].HaRestartPriority);
                VM.set_order(Session, vm.opaque_ref, settings[vm].Order);
                VM.set_start_delay(Session, vm.opaque_ref, settings[vm].StartDelay);

                PercentComplete = (int)(++i * (60.0 / settings.Count));
                if (Cancelling)
                    throw new CancelledException();
            }

            Pool.set_ha_host_failures_to_tolerate(Session, pool.opaque_ref, ntol);

            // Then move any VMs from unprotected -> protected
            foreach (VM vm in settings.Keys)
            {
                if (!VM.HaPriorityIsRestart(vm.Connection, (VM.HA_Restart_Priority)settings[vm].HaRestartPriority))
                    continue;

                Description = string.Format(Messages.HA_SETTING_PRIORITY_ON_X, Helpers.GetName(vm));

                VM.SetHaRestartPriority(Session, vm, (VM.HA_Restart_Priority)settings[vm].HaRestartPriority);
                VM.set_order(Session, vm.opaque_ref, settings[vm].Order);
                VM.set_start_delay(Session, vm.opaque_ref, settings[vm].StartDelay);

                PercentComplete = (int)(++i * (60.0 / settings.Count));
                if (Cancelling)
                    throw new CancelledException();
            }

            // Sync database to ensure new settings are saved to all hosts
            RelatedTask = Pool.async_sync_database(Session);
            PollToCompletion(60, 100);

            Description = Messages.COMPLETED;
        }
    }
}
