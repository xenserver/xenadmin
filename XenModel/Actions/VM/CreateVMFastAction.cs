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

using XenAdmin.Core;
using XenAdmin.Network;
using XenAPI;


namespace XenAdmin.Actions.VMActions
{
    public class CreateVMFastAction : AsyncAction
    {
        private readonly bool _markVmAsBeingCreated;

        public CreateVMFastAction(IXenConnection connection, VM template, bool markVmAsBeingCreated = true)
            : base(connection, string.Format(Messages.INSTANT_VM_CREATE_TITLE, Helpers.GetName(template)), "")
        {
            //CA-339370: the VM's name is calculated at a later stage to avoid duplicate
            //names in the case of creating multiple VMs in quick succession;
            //it comes with the downside that no VM name is shown on the title

            Template = template;
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
            var originalName = Helpers.GetName(Template);

            Description = string.Format(Messages.CLONING_TEMPLATE, originalName);
            RelatedTask = VM.async_clone(Session, Template.opaque_ref, Helpers.MakeHiddenName(originalName));
            PollToCompletion(0, 80);

            string new_vm_ref = Result;

            if (_markVmAsBeingCreated)
            {
                VM = Connection.WaitForCache(new XenRef<VM>(new_vm_ref));
                VM.IsBeingCreated = true;
            }

            XenAdminConfigManager.Provider.HideObject(new_vm_ref);

            Description = Messages.PROVISIONING_VM;
            RelatedTask = VM.async_provision(Session, new_vm_ref);
            PollToCompletion(80, 90);

            Description = Messages.SAVING_VM_PROPERTIES_ACTION_TITLE;
            var newName = Helpers.DefaultVMName(originalName, Connection);
            VM.set_name_label(Session, new_vm_ref, newName);
            XenAdminConfigManager.Provider.ShowObject(new_vm_ref);

            if (_markVmAsBeingCreated)
            {
                VM.name_label = newName; //the set_name_label method takes some time, we want to show the VM right away
                VM.IsBeingCreated = false;
            }

            Result = new_vm_ref;
            Description = string.Format(Messages.INSTANT_VM_CREATE_DESC_COMPLETED, newName);
        }
    }
}
