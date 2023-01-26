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

using System.Collections.Generic;
using System.Linq;
using XenAdmin.Actions.VMActions;
using XenAPI;


namespace XenAdmin.Actions
{
    public class VMSnapshotDeleteAction : VMDestroyAction
    {
        public VMSnapshotDeleteAction(VM snapshot)
            : base(snapshot, GetDisksToDelete(snapshot), new List<VM>())
        {
            // the value of VM should be the snapshot and not the parent VM;
            // first, because it makes sense to set VM to the actual object
            // handled by the action; second and more important, because Run()
            // calls base.Run() which would result to deleting the parent VM;
            // note that the VM setter has already code that set AppliesTo to
            // the parent VM for snapshots
            VM = snapshot;

            Title = string.Format(Messages.ACTION_VM_DELETE_SNAPSHOT_TITLE, VM.Name());
            Description = string.Format(Messages.ACTION_VM_DELETE_SNAPSHOT_TITLE, VM.Name());

            if (VM.power_state == vm_power_state.Suspended)
                ApiMethodsToRoleCheck.Add("VM.hard_shutdown");
        }

        private static List<VBD> GetDisksToDelete(VM snapshot)
        {
            return snapshot.Connection.ResolveAll(snapshot.VBDs).FindAll(x => x.GetIsOwner()).ToList();
        }

        protected override void Run()
        {
            if (VM.power_state == vm_power_state.Suspended)
                VM.hard_shutdown(Session, VM.opaque_ref);

            base.Run();
            Description = string.Format(Messages.SNAPSHOT_DELETED, VM.Name());
        }
    }
}