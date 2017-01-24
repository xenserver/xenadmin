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
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions
{

    public abstract class HostAbstractAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected Func<HostAbstractAction, Pool, long, long, bool> AcceptNTolChanges;
        protected Func<Pool, Host, long, long, bool> AcceptNTolChangesOnEnable;

        protected HostAbstractAction(IXenConnection connection, string title, string description, 
            Func<HostAbstractAction, Pool, long, long, bool> acceptNTolChanges,
            Func<Pool, Host, long, long, bool> acceptNTolChangesOnEnable)
            : base(connection, title, description)
        {
            this.AcceptNTolChanges = acceptNTolChanges;
            this.AcceptNTolChangesOnEnable = acceptNTolChangesOnEnable;
        }

        /// <summary>
        /// Disable the host, then shut down all the VMs on it.
        /// </summary>
        protected void ShutdownVMs(bool isForReboot)
        {
            XenAPI.Host.disable(Session, Host.opaque_ref);
            try
            {
                PercentComplete = 1;

                // Count the resident VMs that are not halted
                List<VM> toShutdown = new List<VM>();
                foreach (VM vm in Connection.ResolveAll(Host.resident_VMs))
                {
                    if (vm.power_state == vm_power_state.Running && !vm.is_control_domain)
                        toShutdown.Add(vm);
                }

                int n = toShutdown.Count;
                if (n == 0)
                    return;

                int step = 94 / n;
                int i = 0;
                foreach (VM vm in toShutdown)
                {
                    Description = String.Format(
                        isForReboot ? Messages.HOSTACTION_REBOOT_VM_SHUTDOWN : Messages.HOSTACTION_SHUTDOWN_VM_SHUTDOWN,
                        Helpers.GetName(vm), i + 1, n);
                    RelatedTask = vm.allowed_operations.Contains(vm_operations.clean_shutdown)
                        ? VM.async_clean_shutdown(Session, vm.opaque_ref)
                        : VM.async_hard_shutdown(Session, vm.opaque_ref);
                    PollToCompletion(PercentComplete, PercentComplete + step);
                    PercentComplete += step;
                    i++;
                }
            }
            catch (Exception e)
            {
                log.Error("Exception shutting down VMs before shutting down host.", e);

                try
                {
                    // At least re-enable the host so user can manually shutdown vms
                    XenAPI.Host.enable(Session, Host.opaque_ref);
                }
                catch (Exception ex)
                {
                    log.Error("Exception trying to re-enable host after error shutting down VMs.", ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Ensures we have at least 1 'slack' server failure. i.e. ensures ntol is such that removing 1 host from the pool
        /// (by reboot, shutdown or disable) will succeed.
        /// May throw CancelledException if the user says no.
        /// </summary>
        /// <param name="actionKind">Must be one of Evacuate, Reboot, RebootAndWait or Shutdown. Required so we can show the correct prompt.</param>
        protected void MaybeReduceNtolBeforeOp(HostActionKind actionKind)
        {
            System.Diagnostics.Trace.Assert(actionKind == HostActionKind.Reboot ||
                actionKind == HostActionKind.RebootAndWait || actionKind == HostActionKind.Shutdown||actionKind==HostActionKind.Evacuate);

            // We may need to drop ntol for this disable to succeed. 
            Pool pool = Helpers.GetPoolOfOne(Host.Connection);
            if (pool != null && pool.ha_enabled)
            {
                Dictionary<XenRef<VM>, string> config = Helpers.GetVmHaRestartPrioritiesForApi(Helpers.GetVmHaRestartPriorities(pool.Connection, true));
                long max = Pool.GetMaximumTolerableHostFailures(Session, config);

                long currentNtol = pool.ha_host_failures_to_tolerate;
                long targetNtol = Math.Max(0, max - 1);

                if (currentNtol > targetNtol)
                {
                    bool cancel = false;

                    // We need to lower ntol. Ask user.
                    cancel = (AcceptNTolChanges != null) ? AcceptNTolChanges(this, pool, currentNtol, targetNtol) : false;

                    if (cancel)
                    {
                        throw new CancelledException();
                    }

                    // This ensures we can tolerate at least one failure - i.e. the host we're about to disable
                    XenAPI.Pool.set_ha_host_failures_to_tolerate(this.Session, pool.opaque_ref, targetNtol);
                }
            }
        }

  

        /// <summary>
        /// Removes MAINTENANCE_MODE = true from the host's other_config, then does a Host.enable.
        /// If appropriate, then asks the user if they want to increase ntol (since hypothetical_max might have gone up).
        /// </summary>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        /// <param name="queryNtolIncrease">If true, the user will be asked if they would like to increase ntol after the host enable.</param>
        protected void Enable(int start, int finish, bool queryNtolIncrease)
        {
            XenAPI.Host.remove_from_other_config(Session, Host.opaque_ref, XenAPI.Host.MAINTENANCE_MODE);

            RelatedTask = XenAPI.Host.async_enable(Session, Host.opaque_ref);
            PollToCompletion(start, finish);

            // Offer to bump ntol back up, if we can
            Pool pool = Helpers.GetPoolOfOne(Host.Connection);
            if (queryNtolIncrease && pool != null && pool.ha_enabled)
            {
                Dictionary<XenRef<VM>, string> config = Helpers.GetVmHaRestartPrioritiesForApi(Helpers.GetVmHaRestartPriorities(pool.Connection, true));
                long max = Pool.GetMaximumTolerableHostFailures(Session, config);
                long currentNtol = pool.ha_host_failures_to_tolerate;

                if (currentNtol < max && AcceptNTolChangesOnEnable != null && AcceptNTolChangesOnEnable(pool, Host, currentNtol, max))
                    Pool.set_ha_host_failures_to_tolerate(this.Session, pool.opaque_ref, max);
            }
        }

      
    }


    public enum HostActionKind { Reboot, RebootAndWait, Evacuate, Shutdown };
}
