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
using XenAdmin.Actions;
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.DR
{
    public class ShutdownAndDestroyVMsAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<VM> selectedToDelete;

        public ShutdownAndDestroyVMsAction(IXenConnection connection, List<VM> deleteVMs)
            : base(connection, deleteVMs.Count == 1 ? Messages.ACTION_SHUTDOWN_AND_DESTROY_VM_TITLE : Messages.ACTION_SHUTDOWN_AND_DESTROY_VMS_TITLE)
        {
            selectedToDelete = deleteVMs;

            #region RBAC Dependencies
            ApiMethodsToRoleCheck.Add("VM.hard_shutdown");
            ApiMethodsToRoleCheck.Add("VM.destroy");
            ApiMethodsToRoleCheck.Add("VBD.destroy");
            ApiMethodsToRoleCheck.Add("VIF.destroy");
            #endregion
        }

        protected override void Run()
        {
            int increment = 0;
            if (selectedToDelete.Count > 0) 
                increment = 100 / (selectedToDelete.Count * 2);

            foreach (var vm in selectedToDelete)
            {
                if (vm.power_state == vm_power_state.Running
                    || vm.power_state == vm_power_state.Paused
                    || vm.power_state == vm_power_state.Suspended)
                {
                    Description = Messages.ACTION_VM_SHUTTING_DOWN;
                    XenAPI.VM.hard_shutdown(Session, vm.opaque_ref);
                    Description = Messages.ACTION_VM_SHUT_DOWN;
                }
                PercentComplete += increment;

                DestroyVM(Session, vm);
                PercentComplete += increment;
            }
            Description = Messages.ACTION_VM_DESTROYED;
            PercentComplete = 100;
        }


        internal static void DestroyVM(Session session, VM vm)
        {
            Exception caught = null;
            log.DebugFormat("Destroying VM '{0}'", vm.Name);

            if (vm.snapshots.Count > 0)
            {
                log.Debug("Destroying snapshots");
                foreach (VM snapshot in vm.Connection.ResolveAll(vm.snapshots))
                    DestroyVM(session, snapshot);
                log.Debug("Snapshots destroyed");
            }

            log.Debug("Unplugging and destroying VBDs");
            foreach (VBD vbd in vm.Connection.ResolveAll<VBD>(vm.VBDs))
            {
                try
                {
                    if (vbd.currently_attached && vbd.allowed_operations.Contains(vbd_operations.unplug))
                        VBD.unplug(session, vbd.opaque_ref);
                }
                finally
                {
                    if (!vbd.currently_attached)
                        VBD.destroy(session, vbd.opaque_ref);
                }
            }
            log.Debug("VBDs unplugged and destroyed");

            log.Debug("Unplugging and destroying VIFs");
            foreach (XenAPI.VIF vif in vm.Connection.ResolveAll<XenAPI.VIF>(vm.VIFs))
            {
                try
                {
                    
                    if (vif.currently_attached && vif.allowed_operations.Contains(XenAPI.vif_operations.unplug))
                        XenAPI.VIF.unplug(session, vif.opaque_ref);
                }
                finally
                {
                    if (!vif.currently_attached) 
                        XenAPI.VIF.destroy(session, vif.opaque_ref);
                }
            }
            log.Debug("VIFs unplugged and destroyed");

            log.Debug("Destroying VM");
            try
            {
                VM.destroy(session, vm.opaque_ref);
            }
            catch (Exception e)
            {
                if (!e.Message.StartsWith("Object has been deleted"))
                {
                    log.Error(e);
                    throw;
                }
            }

            log.DebugFormat("VM '{0}' destroyed", vm.Name);

            if (caught != null)
                throw caught;
        }
    }
}
