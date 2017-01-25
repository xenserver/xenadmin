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
using XenAPI;
using XenAdmin.Core;



namespace XenAdmin.Actions
{
    public class UnplugPlugNetworkAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<PIF> PIFs;
        private List<VIF> VIFs;
        private XenAPI.Network network;

        public UnplugPlugNetworkAction(XenAPI.Network network, bool suppressHistory)
            : base (network.Connection, Messages.ACTION_PIF_UNPLUG_PLUG_TITLE, Messages.ACTION_PIF_UNPLUG_PLUG_DESC, suppressHistory)
        {
            PIFs = Connection.ResolveAll<PIF>(network.PIFs);
            PIFs.RemoveAll(delegate(PIF p) { return !p.currently_attached; });
            VIFs = Connection.ResolveAll<VIF>(network.VIFs);
            VIFs.RemoveAll(delegate(VIF v)
                {
                    if (!v.currently_attached)
                        return true;
                    VM vm = v.Connection.Resolve(v.VM);
                    if (vm == null || vm.power_state != vm_power_state.Running)  // we only replug running VMs
                        return true;
                    return false;
                });

            network.Locked = true;
            foreach (PIF p in PIFs)
                p.Locked = true;
            foreach (VIF v in VIFs)
                v.Locked = true;

            #region RBAC
            ApiMethodsToRoleCheck.Add("pif.async_unplug");
            ApiMethodsToRoleCheck.Add("pif.async_plug");
            ApiMethodsToRoleCheck.Add("vif.async_plug");
            ApiMethodsToRoleCheck.Add("vif.async_unplug");
            #endregion
            Pool = Helpers.GetPool(Connection);

            this.network = network;
        }

        protected override void Run()
        {
            // Make note of exceptions but don't let them throw, we want to carry on and try to replug
            // whatever we can to leave the state in a better condition
            Exception error = null;
            PercentComplete = 10;
            int percentStep = (int)Math.Floor(90.0d / (PIFs.Count*2 + VIFs.Count*2));
            foreach (VIF v in VIFs)
            {
                try
                {
                    Unplug(v);
                }
                catch (Exception e)
                {
                    log.Error(e);
                    if (error == null)
                        error = e;
                }
                PercentComplete += percentStep;
            }
            foreach (PIF p in PIFs)
            {
                try
                {
                    Unplug(p);
                }
                catch (Exception e)
                {
                    log.Error(e);
                    if (error == null)
                        error = e;
                }
                PercentComplete += percentStep;   
            }
            foreach (PIF p in PIFs)
            {
                try
                {
                    Plug(p);
                }
                catch (Exception e)
                {
                    log.Error(e);
                    if (error == null)
                        error = e;
                }
                PercentComplete += percentStep;
            }     
            foreach (VIF v in VIFs)
            {
                try
                {
                    Plug(v);
                }
                catch (Exception e)
                {
                    log.Error(e);
                    if (error == null)
                        error = e;
                }
                PercentComplete += percentStep;
            }

            PercentComplete = 100;
            Description = error != null ? Messages.COMPLETED_WITH_ERRORS : Messages.COMPLETED;
            if (error != null)
            {
                throw error;
                // Ideally we would like to keep the previous description, but we need to throw the error to force the history switch or red highlighting
                // otherwise the user thinks everything is ok. Here we've picked the first one.
            }
        }

        private void Unplug(PIF p)
        {
            if (!p.currently_attached)
            {
                // We will try and unplug the PIF even if it seems to be already unplugged (CA-75969)
                log.DebugFormat("Unplugging PIF '{0}': this PIF is not currently attached. Will try to unplug anyway", p.uuid);
            }

            PIF.unplug(Session, p.opaque_ref);
        }

        private void Plug(PIF p)
        {
            if (p.currently_attached)
            {
                // We will try and plug the PIF even if it seems to be already plugged (CA-75969)
                log.DebugFormat("Plugging PIF '{0}': this PIF is currently attached. Will try to plug anyway", p.uuid);
            }

            PIF.plug(Session, p.opaque_ref);
        }

        private void Unplug(VIF v)
        {
            if (!v.currently_attached)
            {
                // We will try and unplug the VIF even if it seems to be already unplugged (CA-75969)
                log.DebugFormat("Unplugging VIF '{0}': this VIF is not currently attached. Will try to unplug anyway", v.uuid);
            }

            VM vm = v.Connection.Resolve(v.VM);
            if (vm == null || vm.power_state != vm_power_state.Running)
            {
                log.DebugFormat("Ignoring VIF '{0}' for unplug as its VM is not running", v.uuid);
                return;
            }

            VIF.unplug(Session, v.opaque_ref);
        }

        private void Plug(VIF v)
        {
            if (v.currently_attached)
            {
                // We will try and plug the VIF even if it seems to be already plugged (CA-75969)
                log.DebugFormat("Plugging VIF '{0}': this VIF is currently attached. Will try to plug anyway", v.uuid);
            }

            VM vm = v.Connection.Resolve(v.VM);
            if (vm == null || vm.power_state != vm_power_state.Running)
            {
                log.DebugFormat("Ignoring VIF '{0}' for plug as its VM is not running", v.uuid);
                return;
            }

            VIF.plug(Session, v.opaque_ref);
        }

        protected override void Clean()
        {
            network.Locked = false;
            foreach (PIF p in PIFs)
                p.Locked = false;
            foreach (VIF v in VIFs)
                v.Locked = false;
        }
    }

}
