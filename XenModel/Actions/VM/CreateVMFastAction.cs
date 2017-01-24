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
using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.VMActions
{
    /// <summary>
    /// This is for the "Instant VM From Template" Command.
    /// </summary>
    public class CreateVMFastAction : AsyncAction
    {
        private readonly String _nameLabel;
        private readonly bool _markVmAsBeingCreated;

        public CreateVMFastAction(IXenConnection connection, VM template, bool markVmAsBeingCreated = true)
            : base(connection, Messages.INSTANT_VM_CREATE_TITLE, string.Format(Messages.INSTANT_VM_CREATE_DESCRIPTION, Helpers.DefaultVMName(Helpers.GetName(template), connection), Helpers.GetName(template)))
        {
            Template = template;
            _nameLabel = Helpers.DefaultVMName(Helpers.GetName(Template), Connection);
            _markVmAsBeingCreated = markVmAsBeingCreated;

            ApiMethodsToRoleCheck.AddRange(Role.CommonTaskApiList);
            ApiMethodsToRoleCheck.AddRange(Role.CommonSessionApiList);
            ApiMethodsToRoleCheck.Add("vm.clone");
            ApiMethodsToRoleCheck.Add("vm.provision");
            ApiMethodsToRoleCheck.Add("vm.start");
            ApiMethodsToRoleCheck.Add("vm.set_name_label");
        }

        protected override void Run()
        {
            RelatedTask = XenAPI.VM.async_clone(Session, Template.opaque_ref, Helpers.MakeHiddenName(_nameLabel));
            PollToCompletion(0, 80);

            string new_vm_ref = Result;

            if (_markVmAsBeingCreated)
            {
                VM = Connection.WaitForCache(new XenRef<VM>(new_vm_ref));
                VM.IsBeingCreated = true;
            }

            XenAdminConfigManager.Provider.HideObject(new_vm_ref);

            RelatedTask = XenAPI.VM.async_provision(Session, new_vm_ref);
            PollToCompletion(80, 90);

            VM.set_name_label(Session, new_vm_ref, _nameLabel);
            XenAdminConfigManager.Provider.ShowObject(new_vm_ref);

            if (_markVmAsBeingCreated)
            {
                VM.name_label = _nameLabel; //the set_name_label method takes some time, we want to show the VM right away
                VM.IsBeingCreated = false;
            }

            Result = new_vm_ref;
        }
    }
}
