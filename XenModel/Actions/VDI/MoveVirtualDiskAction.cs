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
using System.Linq;
using XenAdmin.Core;
using XenAPI;
using XenAdmin.Network;


namespace XenAdmin.Actions
{
    public class MoveVirtualDiskAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private VDI vdi;

        public MoveVirtualDiskAction(IXenConnection connection, VDI vdi, SR sr)
            : base(connection, string.Format(Messages.ACTION_MOVING_VDI_TO_SR, Helpers.GetName(vdi), Helpers.GetName(connection.Resolve(vdi.SR)), Helpers.GetName(sr)))
        {
            this.vdi = vdi;
            SR = sr;
            vdi.Locked = true;
            sr.Locked = true;
            ApiMethodsToRoleCheck.Add("vdi.destroy");
            ApiMethodsToRoleCheck.Add("vdi.copy");
            if (vdi.type == vdi_type.suspend) 
                ApiMethodsToRoleCheck.Add("vm.set_suspend_VDI");
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
        }

        protected override void Run()
        {
            PercentComplete = 10;
            log.DebugFormat("Moving VDI '{0}'", Helpers.GetName(vdi));
            RelatedTask = VDI.async_copy(Session, vdi.opaque_ref, SR.opaque_ref);
            PollToCompletion(PercentComplete, 60);

            VDI newVdi = Connection.WaitForCache(new XenRef<VDI>(Result));

            // if the original is a suspend VDI, link the suspended VM to the new VDI
            if (vdi.type == vdi_type.suspend)
            {
                var suspendedVm = (from vm in Connection.Cache.VMs
                                         let suspendVdi = Connection.Resolve(vm.suspend_VDI)
                                         where suspendVdi != null && suspendVdi.uuid == vdi.uuid
                                         select vm).FirstOrDefault();
                if (suspendedVm != null)
                {
                    VM.set_suspend_VDI(Session, suspendedVm.opaque_ref, newVdi.opaque_ref);
                }
            }
            PercentComplete = 60;

            var newVbds = new List<VBD>();
            foreach (var vbdRef in vdi.VBDs)
            {
                var oldVbd = Connection.Resolve(vbdRef);
                if (oldVbd == null)
                    continue;

                var newVbd = new VBD
                {
                    userdevice = oldVbd.userdevice,
                    bootable = oldVbd.bootable,
                    mode = oldVbd.mode,
                    type = oldVbd.type,
                    unpluggable = oldVbd.unpluggable,
                    other_config = oldVbd.other_config,
                    VDI = new XenRef<VDI>(newVdi.opaque_ref),
                    VM = new XenRef<VM>(oldVbd.VM)
                };
                newVbd.SetIsOwner(oldVbd.GetIsOwner());
                newVbds.Add(newVbd);

                try
                {
                    if (oldVbd.currently_attached && oldVbd.allowed_operations.Contains(vbd_operations.unplug))
                        VBD.unplug(Session, vbdRef);
                }
                finally
                {
                    if (!oldVbd.currently_attached)
                        VBD.destroy(Session, vbdRef);
                }
            }

            PercentComplete = 80;

            VDI.destroy(Session, vdi.opaque_ref);

            foreach (var newVbd in newVbds)
                Connection.WaitForCache(VBD.create(Session, newVbd));

            Tick(100, Messages.MOVED);
            log.DebugFormat("Moved VDI '{0}'", Helpers.GetName(vdi));
        }

        protected override void Clean()
        {
            SR.Locked = false;
            vdi.Locked = false;
        }
    }
}
