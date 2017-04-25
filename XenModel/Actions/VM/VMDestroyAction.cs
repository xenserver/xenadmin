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
using XenAPI;
using XenAdmin.Core;


namespace XenAdmin.Actions.VMActions
{
    public class VMDestroyAction : PureAsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<VBD> _deleteDisks;
        private List<VM> _deleteSnapshots;

        public VMDestroyAction(VM vm, List<VBD> deleteDisks, List<VM> deleteSnapshots)
            : base(vm.Connection, String.Format(Messages.ACTION_VM_DESTROYING_TITLE, vm.Name, vm.Home() == null ? Helpers.GetName(vm.Connection) : Helpers.GetName(vm.Home())))
        {
            VM = vm;
            Host = vm.Home();
            Pool = Helpers.GetPoolOfOne(vm.Connection);
            _deleteDisks = deleteDisks;
            _deleteSnapshots = deleteSnapshots;
        }


        protected override void Run()
        {
            Description = Messages.ACTION_VM_DESTROYING;
            DestroyVM(Session, VM, _deleteDisks, _deleteSnapshots);
            Description = Messages.ACTION_VM_DESTROYED;
        }

        public static void DestroyVM(Session session, VM vm, bool deleteAllOwnerDisks)
        {
            DestroyVM(session, vm, deleteAllOwnerDisks ? vm.Connection.ResolveAll(vm.VBDs).FindAll(x=>x.IsOwner) : new List<VBD>(), new List<VM>());
        }


        private static void DestroyVM(Session session, VM vm, List<VBD> deleteDisks, IEnumerable<VM> deleteSnapshots)
        {
            Exception caught = null;


            foreach (VM snapshot in deleteSnapshots)
            {
                VM snap = snapshot;
                BestEffort(ref caught, session.Connection.ExpectDisruption, () =>
                                           {
                                               if (snap.power_state == vm_power_state.Suspended)
                                               {
                                                   XenAPI.VM.hard_shutdown(session, snap.opaque_ref);
                                               }
                                               DestroyVM(session, snap, true);
                                           });
            }


            List<XenRef<VDI>> vdiRefs = new List<XenRef<VDI>>();

            foreach (XenRef<VBD> vbdRef in vm.VBDs)
            {
                VBD vbd = vm.Connection.Resolve(vbdRef);
                if (vbd == null)
                    continue;


                if (deleteDisks.Contains(vbd))
                {
                    if(vbd.Connection.Resolve(vbd.VDI)!=null)
                        vdiRefs.Add(vbd.VDI);
                }
            }

            //CA-91072: Delete Suspend image VDI
            VDI suspendVDI = vm.Connection.Resolve(vm.suspend_VDI);
            if (suspendVDI != null)
                vdiRefs.Add(vm.suspend_VDI);

            XenAPI.VM.destroy(session, vm.opaque_ref);


            foreach (XenRef<VDI> vdiRef in vdiRefs)
            {
                XenRef<VDI> vdi = vdiRef;
                BestEffort(ref caught, session.Connection.ExpectDisruption, () => XenAPI.VDI.destroy(session, vdi.opaque_ref));

                //CA-115249. XenAPI could have already deleted the VDI. Destroy suspended VM and destroy snapshot functions are affected.
                var failure = caught as Failure;
                if (failure != null && failure.ErrorDescription != null && failure.ErrorDescription.Count > 0 && failure.ErrorDescription[0] == "HANDLE_INVALID")
                {
                    log.InfoFormat("VDI:{0} has already been deleted -- ignoring exception.", vdi.opaque_ref);
                    caught = null;
                }
            }

            if (caught != null)
                throw caught;
        }
    }
}
