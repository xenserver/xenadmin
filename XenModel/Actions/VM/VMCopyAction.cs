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
using XenAPI;


namespace XenAdmin.Actions.VMActions
{
    public class VMCopyAction : AsyncAction
    {
        private string _nameLabel;
        private string _namedescription;

        public VMCopyAction(VM vm, Host host, SR sr, string nameLabel, string description)
            : base(vm.Connection, string.Format(Messages.ACTION_VM_COPYING_TITLE, vm.NameWithLocation(), nameLabel, sr.NameWithLocation()))
        {
            VM = vm;
            Host = host;
            Pool = Core.Helpers.GetPool(vm.Connection);
            SR = sr;
            _nameLabel = nameLabel;
            if (vm.is_a_template)
                Template = vm;
            _namedescription = description;

            ApiMethodsToRoleCheck.AddRange(StaticRBACDependencies);
        }

        public static RbacMethodList StaticRBACDependencies
        {
            get
            {
                var list = new RbacMethodList("VM.copy", "VM.set_name_description");
                list.AddRange(Role.CommonSessionApiList);
                list.AddRange(Role.CommonTaskApiList);
                return list;
            }
        }

        protected override void Run()
        {
            RelatedTask = VM.async_copy(Session, VM.opaque_ref, _nameLabel, SR.opaque_ref);
            try
            {
                PollToCompletion();
            }
            catch (CancelledException)
            {
                Description = string.Format(Messages.COPY_TO_SHARED_CANCELLED, VM.NameWithLocation());
                throw;
            }
            {
                VM created = Connection.WaitForCache(new XenRef<VM>(Result));
                VM.set_name_description(Session, created.opaque_ref, _namedescription);
            }
            Description = Messages.ACTION_VM_COPIED;
        }
    }
}
