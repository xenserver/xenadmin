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
using XenAdmin.Core;
using XenAPI;


namespace XenAdmin.Actions.VMActions
{
    public class VMMoveAction : AsyncAction
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, SR> _storageMapping;

        public VMMoveAction(VM vm, Dictionary<string, SR> storageMapping, Host host)
             : base(vm.Connection, "")
        {
            VM = vm;
            Host = host;
            Pool = Helpers.GetPool(vm.Connection);
            if (vm.is_a_template)
                Template = vm;

            _storageMapping = storageMapping;
            SR = _storageMapping.Values.FirstOrDefault();

            PopulateApiMethodsToRoleCheck();

            var sourceHost = vm.Home();
            if (sourceHost != null && !sourceHost.Equals(host))
                Title = string.Format(Messages.ACTION_VM_MOVING_HOST, vm.Name(), sourceHost, host.Name());
            else if (storageMapping.Count == 1)
                Title = string.Format(Messages.ACTION_VM_MOVING_SR, vm.Name(), storageMapping.Values.ElementAt(0).Name());
            else
                Title = string.Format(Messages.ACTION_VM_MOVING, vm.Name());
        }

        public VMMoveAction(VM vm, SR sr, Host host)
            : this(vm, GetStorageMapping(vm, sr), host)
        {
        }

        private static Dictionary<string, SR> GetStorageMapping(VM vm, SR sr)
        {
            var storageMapping = new Dictionary<string, SR>();
            foreach (var vbdRef in vm.VBDs)
            {
                var vbd = vm.Connection.Resolve(vbdRef);
                if (vbd != null)
                    storageMapping.Add(vbd.VDI.opaque_ref, sr);
            }
            return storageMapping;

        }

        #region RBAC Dependencies
        private void PopulateApiMethodsToRoleCheck()
        {
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.Add("vm.copy");
            ApiMethodsToRoleCheck.Add("vm.set_name_description");
            ApiMethodsToRoleCheck.Add("vm.set_suspend_SR");
            ApiMethodsToRoleCheck.Add("vm.destroy");
            ApiMethodsToRoleCheck.Add("vdi.destroy");
        }
        #endregion

        protected override void Run()
        {
            // move the progress bar above 0, it's more reassuring to see than a blank bar as we copy the first disk
            PercentComplete += 10;
            int halfstep = 90 / (VM.VBDs.Count * 2);

            var exceptions = new List<Exception>();

            foreach (var vbdRef in VM.VBDs)
            {
                if (Cancelling)
                    throw new CancelledException();

                var oldVBD = Connection.Resolve(vbdRef);

                if (oldVBD ==  null || !oldVBD.GetIsOwner())
                    continue;

                if (_storageMapping == null ||
                    !_storageMapping.TryGetValue(oldVBD.VDI.opaque_ref, out SR sr) || sr == null)
                    continue;

                var curVdi = Connection.Resolve(oldVBD.VDI);
                if (curVdi == null)
                    continue;

                if (curVdi.SR.opaque_ref == sr.opaque_ref)
                    continue;

                Description = string.Format(Messages.ACTION_MOVING_VDI_TO_SR,
                    Helpers.GetName(curVdi), Helpers.GetName(Connection.Resolve(curVdi.SR)), Helpers.GetName(sr));

                RelatedTask = VDI.async_copy(Session, oldVBD.VDI.opaque_ref, sr.opaque_ref);
                PollToCompletion(PercentComplete, PercentComplete + halfstep);
                var newVDI = Connection.WaitForCache(new XenRef<VDI>(Result));

                var newVBD = new VBD
                {
                    userdevice = oldVBD.userdevice,
                    bootable = oldVBD.bootable,
                    mode = oldVBD.mode,
                    type = oldVBD.type,
                    unpluggable = oldVBD.unpluggable,
                    other_config = oldVBD.other_config,
                    VDI = new XenRef<VDI>(newVDI.opaque_ref),
                    VM = new XenRef<VM>(VM.opaque_ref)
                };
                newVBD.SetIsOwner(oldVBD.GetIsOwner());

                try
                {
                    VDI.destroy(Session, oldVBD.VDI.opaque_ref);
                }
                catch (Exception e)
                {
                    log.ErrorFormat("Failed to destroy old VDI {0}", oldVBD.VDI.opaque_ref);
                    exceptions.Add(e);
                }

                Connection.WaitForCache(VBD.create(Session, newVBD));

                PercentComplete += halfstep;
            }

            Description = string.Empty;

            if (SR != null)
                VM.set_suspend_SR(Session, VM.opaque_ref, SR.opaque_ref);

            if (exceptions.Count > 0)
                throw new Exception(string.Format(Messages.ACTION_VM_MOVING_VDI_DESTROY_FAILURE, VM.NameWithLocation()));

            Description = Messages.MOVED;
        }
    }
}
