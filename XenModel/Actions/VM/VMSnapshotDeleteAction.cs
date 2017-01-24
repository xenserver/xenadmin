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
using XenAdmin.Actions.VMActions;
using XenAPI;


namespace XenAdmin.Actions
{
    public class VMSnapshotDeleteAction : PureAsyncAction
    {

        private VM m_Snapshot;
        public VMSnapshotDeleteAction(VM snapshot)
            : base(snapshot.Connection, String.Format(Messages.ACTION_VM_DELETE_SNAPSHOT_TITLE, snapshot.Name))
        {
            this.VM = Connection.Resolve<VM>(snapshot.snapshot_of);
            this.m_Snapshot = snapshot;
        }

        protected override void Run()
        {
            Description = String.Format(Messages.ACTION_VM_DELETE_SNAPSHOT_TITLE, m_Snapshot.Name);
            if (m_Snapshot.power_state == vm_power_state.Suspended)
            {
                XenAPI.VM.hard_shutdown(Session, m_Snapshot.opaque_ref);
            }
            VMDestroyAction.DestroyVM(Session, m_Snapshot, true);
            Description = String.Format(Messages.SNAPSHOT_DELETED, m_Snapshot.Name);
        }
    }
}