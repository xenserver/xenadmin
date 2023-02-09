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

using System;
using System.Collections.Generic;
using System.Linq;
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions.VMActions
{
    public class VMDestroyAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<VBD> _disksToDelete;
        private List<VM> _snapshotsToDelete;

        public VMDestroyAction(VM vm, List<VBD> disksToDelete, List<VM> snapshotsToDelete)
            : base(vm.Connection, "")
        {
            VM = vm;
            Host = vm.Home();
            Pool = Helpers.GetPoolOfOne(vm.Connection);
            _disksToDelete = disksToDelete;
            _snapshotsToDelete = snapshotsToDelete;

            Title = string.Format(Messages.ACTION_VM_DESTROYING_TITLE,
                vm.Name(),
                vm.Home() == null ? Helpers.GetName(vm.Connection) : Helpers.GetName(vm.Home()));

            Description = Messages.ACTION_VM_DESTROYING;

            ApiMethodsToRoleCheck.AddRange("VM.destroy", "VDI.destroy");

            if (_snapshotsToDelete.Any(s => s.power_state == vm_power_state.Suspended))
                ApiMethodsToRoleCheck.Add("VM.hard_shutdown");
        }


        protected override void Run()
        {
            DestroyVM(Session, VM, _disksToDelete, _snapshotsToDelete);
            Description = Messages.ACTION_VM_DESTROYED;
        }

        public static void DestroyVM(Session session, VM vm, bool deleteAllOwnerDisks)
        {
            DestroyVM(session, vm, deleteAllOwnerDisks ? vm.Connection.ResolveAll(vm.VBDs).FindAll(x => x.GetIsOwner()) : new List<VBD>(), new List<VM>());
        }


        private static void DestroyVM(Session session, VM vm, List<VBD> disksToDelete, IEnumerable<VM> snapshotsToDelete)
        {
            var caught = new List<Exception>();

            foreach (VM snapshot in snapshotsToDelete)
            {
                VM snap = snapshot;
                try
                {
                    if (snap.power_state == vm_power_state.Suspended)
                        VM.hard_shutdown(session, snap.opaque_ref);
                    DestroyVM(session, snap, true);
                }
                catch (Exception e)
                {
                    log.Error($"Failed to delete snapshot {snap.opaque_ref}", e);
                    caught.Add(e);
                }
            }

            List<XenRef<VDI>> vdiRefs = new List<XenRef<VDI>>();

            foreach (XenRef<VBD> vbdRef in vm.VBDs)
            {
                VBD vbd = vm.Connection.Resolve(vbdRef);
                if (vbd == null)
                    continue;

                if (disksToDelete.Contains(vbd))
                {
                    if(vbd.Connection.Resolve(vbd.VDI)!=null)
                        vdiRefs.Add(vbd.VDI);
                }
            }

            //CA-91072: Delete Suspend image VDI
            VDI suspendVDI = vm.Connection.Resolve(vm.suspend_VDI);
            if (suspendVDI != null)
                vdiRefs.Add(vm.suspend_VDI);

            VM.destroy(session, vm.opaque_ref);

            foreach (XenRef<VDI> vdiRef in vdiRefs)
            {
                try
                {
                    VDI.destroy(session, vdiRef.opaque_ref);
                }
                catch (Exception e)
                {
                    //CA-115249. XenAPI could have already deleted the VDI.
                    //Destroy suspended VM and destroy snapshot functions are affected.

                    if (e is Failure failure && failure.ErrorDescription != null &&
                        failure.ErrorDescription.Count > 0 && failure.ErrorDescription[0] == "HANDLE_INVALID")
                    {
                        log.InfoFormat($"VDI {vdiRef.opaque_ref} has already been deleted; ignoring API failure.");
                    }
                    else
                    {
                        log.Error($"Failed to delete VDI {vdiRef.opaque_ref}", e);
                        caught.Add(e);
                    }
                }
            }

            if (caught.Count > 0)
                throw caught[0];
        }
    }
}
