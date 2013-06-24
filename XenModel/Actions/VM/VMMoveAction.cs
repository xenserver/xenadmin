/* Copyright (c) Citrix Systems Inc. 
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
using XenAPI;
using System.Text.RegularExpressions;


namespace XenAdmin.Actions.VMActions
{
    public class VMMoveAction:AsyncAction
    {

        private string _nameLabel;
        private string _description;
        public VMMoveAction(VM vm, SR sr, Host host, string namelabel,string description)
            : base(vm.Connection, string.Format(Messages.ACTION_VM_MOVING_TITLE, vm.Name, namelabel, sr.NameWithoutHost))
        {
            this.Description = Messages.ACTION_PREPARING;
            this.VM = vm;
            this.Host = host;
            this.Pool = Core.Helpers.GetPool(vm.Connection);
            this.SR = sr;
            _nameLabel = namelabel;
            if (vm.is_a_template)
                this.Template = vm;
            _description = description;

            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.Add("vm.copy");
            ApiMethodsToRoleCheck.Add("vm.set_name_description");
            ApiMethodsToRoleCheck.Add("vm.set_suspend_SR");
            ApiMethodsToRoleCheck.Add("vm.destroy");
            ApiMethodsToRoleCheck.Add("vdi.destroy");
        }

        protected override void Run()
        {
            this.Description = Messages.ACTION_VM_MOVING;
            try
            {
                var vbds = Connection.ResolveAll(VM.VBDs);
                int halfstep = (int)(180/vbds.Count);
                // move the progress bar above 0, it's more reassuring to see than a blank bar as we copy the first disk
                PercentComplete += 10;
                Exception exn = null;

                foreach (VBD oldVBD in vbds)
                {
                    if (!oldVBD.IsOwner)
                        continue;

                    var curVdi = Connection.Resolve(oldVBD.VDI);
                    if (curVdi == null || curVdi.SR.opaque_ref == this.SR.opaque_ref)
                        continue;

                    var newVDI = Connection.WaitForCache<VDI>(XenAPI.VDI.copy(Session, oldVBD.VDI.opaque_ref, this.SR.opaque_ref));
                    PercentComplete += halfstep;

                    var newVBD = new VBD
                                     {
                                         IsOwner = oldVBD.IsOwner,
                                         userdevice = oldVBD.userdevice,
                                         bootable = oldVBD.bootable,
                                         mode = oldVBD.mode,
                                         type = oldVBD.type,
                                         unpluggable = oldVBD.unpluggable,
                                         other_config = oldVBD.other_config,
                                         VDI = new XenRef<VDI>(newVDI.opaque_ref),
                                         VM = new XenRef<VM>(VM.opaque_ref)
                                     };

                    VBD vbd = oldVBD;
                    BestEffort(ref exn, () => VDI.destroy(Session, vbd.VDI.opaque_ref));
                    Connection.WaitForCache<VBD>(VBD.create(Session, newVBD));

                    PercentComplete += halfstep;
                }

                if (Helpers.BostonOrGreater(Connection))
                    VM.set_suspend_SR(Session, VM.opaque_ref, SR.opaque_ref);

                if (exn != null)
                    throw exn;

            }
            catch (CancelledException)
            {
                this.Description = string.Format(Messages.MOVE_CANCELLED, VM.Name);
                throw;
            }
            this.Description = Messages.MOVED;
        }
    }
}
